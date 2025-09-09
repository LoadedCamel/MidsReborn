using SkiaSharp;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace Mids_Reborn.Core.Ai;

public static class IconCacheBuilder
{
    private static string BaseDir => AppContext.BaseDirectory;
    private static string ModelPath => Path.Combine(BaseDir, "Models", "RealESRGAN_x4plus.onnx");
    private static string VersionMarker => Path.Combine(BaseDir, "enh_upscale.done");

    public static async Task RunAsync()
    {
        if (File.Exists(VersionMarker))
            return;

        var searchRoots = new List<string>
        {
            AppDataPaths.BaseAssetsPath
        };


        var dataPath = AppDataPaths.BaseDataPath;
        if (Directory.Exists(dataPath))
        {
            var dbImages = Directory.GetDirectories(dataPath)
                .Select(subDir => Path.Combine(subDir, "Images"))
                .Where(Directory.Exists);

            searchRoots.AddRange(dbImages);
        }

        var skippedFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "defaultAvatar.png", "locked.png", "MRBLoading.gif", "pSlot0.png", "pSlot1.png",
            "pSlot2.png", "pSlot3.png", "pSlot4.png", "pSlot5.png",
            "InfoBackDropH.png", "InfoBackDropH2.png", "InfoBackDropV.png", "InfoBackDropV2.png"
        };

        var legacyFiles = searchRoots
            .SelectMany(root => Directory.GetFiles(root, "*.png", SearchOption.AllDirectories))
            .Where(file =>
            {
                var name = Path.GetFileName(file);
                return !skippedFiles.Contains(name) && IsValidInputImage(file);
            })
            .ToList();

        if (legacyFiles.Count == 0)
        {
            await File.WriteAllTextAsync(VersionMarker, "v1");
            return;
        }

        using var upscaler = new OnnxUpscaler(ModelPath);
        int concurrency = upscaler.IsGpuEnabled
            ? 6
            : Math.Clamp(Environment.ProcessorCount / 2, 1, 2);

        Debug.WriteLine($"Upscaling {legacyFiles.Count} icons using {(upscaler.IsGpuEnabled ? "GPU" : "CPU")} with {concurrency} threads...");

        var errors = new ConcurrentBag<string>();

        var semaphore = new SemaphoreSlim(concurrency);
        try
        {
            var tasks = legacyFiles.Select(async file =>
            {
                await semaphore.WaitAsync();
                try
                {
                    using var input = SKBitmap.Decode(file);
                    using var icon128 = upscaler.UpscaleTo128(input);

                    icon128.Save(file, ImageFormat.Png);
                }
                catch (Exception ex)
                {
                    errors.Add($"{file}: {ex.Message}");
                }
                finally
                {
                    semaphore.Release();
                }
            }).ToList();

            await Task.WhenAll(tasks);
        }
        finally
        {
            semaphore.Dispose();
        }

        if (errors.Count == 0)
        {
            await File.WriteAllTextAsync(VersionMarker, "v1");
            Debug.WriteLine("Enhancement icon cache successfully built.");
        }
        else
        {
            Debug.WriteLine("Errors during enhancement icon upscaling:");
            foreach (var error in errors)
                Debug.WriteLine(error);
        }
    }

    private static bool IsValidInputImage(string filePath)
    {
        try
        {
            using var img = SKBitmap.Decode(filePath);
            return (img.Width == 30 && img.Height == 30) || img is { Width: 48, Height: 48 };
        }
        catch
        {
            return false;
        }
    }
}
