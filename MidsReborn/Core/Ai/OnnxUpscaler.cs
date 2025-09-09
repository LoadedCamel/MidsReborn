using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Mids_Reborn.Core.Ai;

public sealed class OnnxUpscaler : IDisposable
{
    private readonly InferenceSession _session;
    private const int ScaleFactor = 4;

    public bool IsGpuEnabled { get; private set; }

    public OnnxUpscaler(string modelPath)
    {
        var options = new SessionOptions();
        try
        {
            options.AppendExecutionProvider_DML(); // Use DirectML (GPU)
            IsGpuEnabled = true;
            Debug.WriteLine("ONNX initialized with DirectML (GPU).");
        }
        catch
        {
            IsGpuEnabled = false;
            Debug.WriteLine("ONNX falling back to CPU.");
        }

        _session = new InferenceSession(modelPath, options);
    }

    public Bitmap UpscaleTo128(SKBitmap input)
    {
        var skUpscaled = RunModel(input); // 120x120 or 192x192
        var sysBitmap = ConvertToSystemBitmap(skUpscaled);
        return ResizeTo128(sysBitmap); // Always returns 128x128
    }

    private SKBitmap RunModel(SKBitmap input)
    {
        int width = input.Width;
        int height = input.Height;

        var alphaChannel = new byte[width * height];
        var inputTensor = new DenseTensor<float>(new[] { 1, 3, height, width });

        unsafe
        {
            byte* ptr = (byte*)input.GetPixels().ToPointer();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int offset = (y * input.RowBytes) + (x * 4);
                    byte r = ptr[offset + 0];
                    byte g = ptr[offset + 1];
                    byte b = ptr[offset + 2];
                    byte a = ptr[offset + 3];

                    inputTensor[0, 0, y, x] = b / 255f;
                    inputTensor[0, 1, y, x] = g / 255f;
                    inputTensor[0, 2, y, x] = r / 255f;
                    alphaChannel[y * width + x] = a;
                }
            }
        }

        using var results = _session.Run([NamedOnnxValue.CreateFromTensor("input", inputTensor)]);
        var output = results.First().AsTensor<float>();

        int outWidth = width * ScaleFactor;
        int outHeight = height * ScaleFactor;
        var result = new SKBitmap(outWidth, outHeight, SKColorType.Bgra8888, SKAlphaType.Premul);

        unsafe
        {
            byte* dst = (byte*)result.GetPixels().ToPointer();
            for (int y = 0; y < outHeight; y++)
            {
                for (int x = 0; x < outWidth; x++)
                {
                    byte r = (byte)Math.Clamp(output[0, 2, y, x] * 255f, 0, 255);
                    byte g = (byte)Math.Clamp(output[0, 1, y, x] * 255f, 0, 255);
                    byte b = (byte)Math.Clamp(output[0, 0, y, x] * 255f, 0, 255);

                    float srcX = (float)x / ScaleFactor;
                    float srcY = (float)y / ScaleFactor;
                    byte a = BilinearSampleAlpha(alphaChannel, width, height, srcX, srcY);

                    int offset = (y * result.RowBytes) + (x * 4);
                    dst[offset + 0] = r;
                    dst[offset + 1] = g;
                    dst[offset + 2] = b;
                    dst[offset + 3] = a;
                }
            }
        }

        return result;
    }

    private static byte BilinearSampleAlpha(byte[] alpha, int width, int height, float fx, float fy)
    {
        int x = (int)fx;
        int y = (int)fy;
        float dx = fx - x;
        float dy = fy - y;

        int x1 = Math.Clamp(x, 0, width - 1);
        int x2 = Math.Clamp(x + 1, 0, width - 1);
        int y1 = Math.Clamp(y, 0, height - 1);
        int y2 = Math.Clamp(y + 1, 0, height - 1);

        float a11 = alpha[y1 * width + x1];
        float a12 = alpha[y1 * width + x2];
        float a21 = alpha[y2 * width + x1];
        float a22 = alpha[y2 * width + x2];

        float top = a11 * (1 - dx) + a12 * dx;
        float bottom = a21 * (1 - dx) + a22 * dx;
        float interpolated = top * (1 - dy) + bottom * dy;

        return (byte)Math.Clamp(interpolated, 0, 255);
    }

    private static Bitmap ConvertToSystemBitmap(SKBitmap skBitmap)
    {
        using var image = SKImage.FromBitmap(skBitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = new MemoryStream(data.ToArray());
        return new Bitmap(stream); // System.Drawing.Bitmap
    }

    private static Bitmap ResizeTo128(Bitmap source)
    {
        var result = new Bitmap(128, 128, PixelFormat.Format32bppPArgb);
        using var g = Graphics.FromImage(result);
        g.Clear(Color.Transparent);
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        g.DrawImage(source, new Rectangle(0, 0, 128, 128), new Rectangle(0, 0, source.Width, source.Height), GraphicsUnit.Pixel);
        return result;
    }

    public void Dispose() => _session.Dispose();
}