using System.Drawing.Drawing2D;
using Microsoft.Web.WebView2.Core;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Ai;
using Mids_Reborn.Core.Base.IO_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.UI.Design.Extensions;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Forms
{
    public partial class Loader : PerPixelAlpha, IMessenger
    {
        private delegate void LoadDataHandler(object? sender, bool success);
        private event LoadDataHandler? LoadData;

        private bool _useWebView;
        private bool _webViewReady;

        private static bool FirstRun
           {
               get
               {
                   var path = AppDataPaths.JsonConfig;
                   if (!File.Exists(path)) return true;
                   try
                   {
                       var len = new FileInfo(path).Length;
                       return len <= 8; // keep your heuristic if needed
                   }
                   catch
                   {
                       // If we can't read length, treat as first run rather than crashing.
                       return true;
                   }
               }
           }

        private readonly TaskCompletionSource<bool> _loadCompleteSource = new();

        public Task LoadCompleted => _loadCompleteSource.Task;

        public Loader()
        {
            InitializeComponent();
            Load += OnLoad;
            LoadData += OnLoadData;
            Shown += OnShown;
            FormClosed += OnFormClosed;
            webView.CoreWebView2InitializationCompleted += WebViewOnCoreWebView2InitializationCompleted;
            webView.NavigationCompleted += WebViewOnNavigationCompleted;
        }

        private void OnFormClosed(object? sender, FormClosedEventArgs e) => webView.Dispose();

        private void OnShown(object? sender, EventArgs e)
        {
            InitializeWebView();
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            RoundControlCorners(mainPanel);
            RoundControlCorners(webView);

            ConfigData.Initialize();
            ThemeManager.Initialize();
        }

        private async void InitializeWebView()
        {
            try
            {
                await webView.EnsureCoreWebView2Async(null);
            }
            catch (Exception)
            {
                _useWebView = false;
            }
            switch (_useWebView)
            {
                case false:
                    LoadData?.Invoke(this, false);
                    break;
                case true:
                    webView.CoreWebView2?.NavigateToString(Consts.InitHtml);
                    break;
            }
        }

        private void WebViewOnCoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping("appassets.mrb", AppContext.BaseDirectory, CoreWebView2HostResourceAccessKind.DenyCors);
                _useWebView = true;
            }
            else
            {
                _useWebView = false;
            }
        }

        private void WebViewOnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            LoadData?.Invoke(this, e.IsSuccess);
        }

        private async void OnLoadData(object? sender, bool success)
        {
            _webViewReady = success switch
            {
                true => true,
                false => false
            };

            if (MidsContext.Config is null) return;

            SetMessage("Rebuilding icon cache...");
            await IconCacheBuilder.RunAsync();

            switch (MidsContext.Config.FirstRun)
            {
                case true:
                    MainModule.MidsController.SelectDefaultDatabase(this);
                    break;
                default:
                    await MainModule.MidsController.LoadData(this, MidsContext.Config.DataPath);
                    break;
            }

            MidsContext.Config.SaveConfig();
            _loadCompleteSource.SetResult(true);
            DialogResult = DialogResult.OK;
            Close();
        }

        public async void SetMessage(string text)
        {
            if (label.InvokeRequired)
            {
                label.Invoke(new Action(() => label.Text = text));
            }
            else
            {
                if (label.Text != text)
                {
                    label.Text = text;
                }

                label.Refresh();
                if (_webViewReady)
                {
                    await webView.CoreWebView2.ExecuteScriptAsync(
                        $@"var messageDiv = document.querySelector(""div#message"");
messageDiv.style.opacity = 0;
setTimeout(() => {{ messageDiv.textContent = ""{text.Replace("\"", "\\\"")}""; }}, 100);
setTimeout(() => {{ messageDiv.style.opacity = 1; }}, 100);");
                }

                await Task.Delay(500);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style |= 0x20000;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using var path = new GraphicsPath();
            path.AddRoundRectangle(new Rectangle(0, 0, Width, Height), 20);
            Region = new Region(path);
        }

        private static void RoundControlCorners(Control control)
        {
            using var path = new GraphicsPath();
            path.AddRoundRectangle(new Rectangle(0, 0, control.Width, control.Height), 20); // 20 is the corner radius
            control.Region = new Region(path);
        }
    }
}
