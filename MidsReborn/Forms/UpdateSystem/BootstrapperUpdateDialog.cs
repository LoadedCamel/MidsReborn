using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using Mids_Reborn.Core;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public partial class BootstrapperUpdateDialog : Form
    {
        private string BootstrapperUriFile;
        private string VersionTuple;
        private string TargetPath;
        private float WheelAngleOffset;
        private int DownloadPercent = 0;
        private string TmpFile = "";

        public BootstrapperUpdateDialog(string bootstrapperUriFile, string versionTuple, string targetPath)
        {
            BootstrapperUriFile = bootstrapperUriFile;
            VersionTuple = versionTuple;
            TargetPath = targetPath;

            InitializeComponent();
        }

        private async void BootstrapperUpdateDialog_Load(object sender, EventArgs e)
        {
            try
            {
                skControl1.Invalidate();
                var uri = new Uri(BootstrapperUriFile);
                label1.Text = $"Downloading {uri.Segments[^1]} from {uri.Host} ...";

                // WebClient is obsolete. Upgrade to HttpClient
                using var webClient = new WebClient();
                webClient.DownloadFileCompleted += (s, e) =>
                {
                    if (e.Error == null)
                    {
                        ExtractFiles();

                        return;
                    }

                    label1.Text = e.Error.Message.Length > 150
                        ? $"{e.Error.Message.Substring(0, 150)}..."
                        : e.Error.Message;

                    imageButtonEx1.Visible = true;
                };

                webClient.DownloadProgressChanged += (s, e) =>
                {
                    DownloadPercent = e.ProgressPercentage;
                    skControl1.Invalidate();
                };

                TmpFile = $"{Path.Combine(Path.GetTempPath(), $"{new Random().Next():x2}-{uri.Segments[^1]}")}";
                await webClient.DownloadFileTaskAsync(BootstrapperUriFile, TmpFile);
            }
            catch (Exception ex)
            {

            }
        }

        private void ExtractFiles()
        {
            label1.Text = "Download complete. Extracting files...";

            try
            {
                using var fsInput = File.OpenRead(TmpFile);
                using var zf = new ZipFile(fsInput);
                var nFiles = zf.Count;
                var k = 0;
                foreach (ZipEntry zEntry in zf)
                {
                    if (!zEntry.IsFile)
                    {
                        continue;
                    }

                    DownloadPercent = (int)Math.Round(k * 100 / (float)nFiles);
                    label1.Text = $"Processing archive file: {k + 1} / {nFiles} [{zEntry.Name}]";
                    skControl1.Invalidate();

                    var outputFile = Path.Combine(TargetPath, zEntry.Name);
                    var buffer = new byte[4096];

                    using var zStream = zf.GetInputStream(zEntry);
                    using var fsOutput = File.Create(outputFile);
                    StreamUtils.Copy(zStream, fsOutput, buffer);

                    k++;
                }

                label1.Text = "Bootstrapper update complete.";
                DownloadPercent = 100;
                skControl1.Invalidate();

                Thread.Sleep(1500);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                DownloadPercent = 0;
                label1.Text = ex.Message.Length > 150 ? $"{ex.Message.Substring(0, 150)}..." : ex.Message;
                skControl1.Invalidate();
                imageButtonEx1.Visible = true;
            }
        }

        private void skControl1_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(BackColor.ToSKColor());

            var centerPoint = new SKPoint(e.Info.Width / 2f, e.Info.Height / 2f);
            var colors = new SKColor[] { // ??
                new(0x00, 0x1b, 0x45),
                new(0x00, 0x99, 0xca)
            };

            using var plasmaPaint = new SKPaint
            {
                IsAntialias = true,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 8,
                Shader = SKShader.CreateSweepGradient(centerPoint, colors, SKShaderTileMode.Mirror, WheelAngleOffset % float.Tau, float.Tau + WheelAngleOffset % float.Tau),
            };

            canvas.DrawCircle(centerPoint, e.Info.Width / 2f * 0.85f, plasmaPaint);

            if (DownloadPercent <= float.Epsilon)
            {
                return;
            }

            using var textPaint = new SKPaint
            {
                Color = new SKColor(0x00, 0xb9, 0xdd),
                IsAntialias = true
            };

            using var textFont = new SKFont(SKTypeface.Default, 14);
            canvas.DrawText($"{DownloadPercent}%", centerPoint, SKTextAlign.Center, textFont, textPaint);
        }

        private void imageButtonEx1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }
    }
}
