using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Extensions;
using Mids_Reborn.Core.Base.IO_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;

namespace Mids_Reborn.Forms
{
    public partial class Loader : PerPixelAlpha, IMessenger
    {
        private delegate void LoadDataHandler(object? sender, bool success);
        private event LoadDataHandler? LoadData;

        private frmMain? _mainForm;
        private const int BorderSize = 2;
        private bool _useWebView;
        private bool _webViewReady;
        private readonly string[]? _passedArgs;

        private static bool FirstRun
        {
            get
            {
                if (!File.Exists(Files.FNameJsonConfig))
                {
                    File.Create(Files.FNameJsonConfig);
                }
                var fileInfo = new FileInfo(Files.FNameJsonConfig);
                return fileInfo.Length <= 8;
            }
        }
        
        public Loader(string[]? args)
        {
            _passedArgs = args;
            InitializeComponent();
            Padding = new Padding(BorderSize);
            RoundControlCorners(mainPanel);
            RoundControlCorners(webView);
            RoundControlCorners(pictureBox);
            Load += OnLoad;
            LoadData += OnLoadData;
        }

        private async void OnLoad(object? sender, EventArgs e)
        {
            switch (FirstRun)
            {
                case true:
                    ConfigData.Initialize(true);
                    if (MidsContext.Config != null)
                    {
                        MidsContext.Config.IsInitialized = true;
                        MidsContext.Config.FirstRun = true;
                    }

                    break;
                default:
                    ConfigData.Initialize();
                    break;
            }
            webView.CoreWebView2InitializationCompleted += WebViewOnCoreWebView2InitializationCompleted;
            webView.NavigationCompleted += WebViewOnNavigationCompleted;
            await webView.EnsureCoreWebView2Async(null);
            if (_useWebView)
            {
                webView.CoreWebView2?.NavigateToString(Consts.InitHtml);
            }
            else
            {
                LoadData?.Invoke(this, false);
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
                webView.Visible = false;
                pictureBox.Visible = true;
                _useWebView = false;
            }
        }

        private void WebViewOnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            LoadData?.Invoke(this, e.IsSuccess);
        }

        private async void OnLoadData(object? sender, bool success)
        {
            switch (success)
            {
                case true:
                    _webViewReady = true;
                    break;
                case false:
                    webView.Visible = false;
                    pictureBox.Visible = true;
                    break;
            }

            if (MidsContext.Config == null) return;
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
            _mainForm = new frmMain(_passedArgs)
            {
                Loader = this,
                Location = MidsContext.Config.Bounds.Location
            };
            _mainForm.Load += MainFormOnLoad;
            _mainForm.Shown += MainFormOnShown;
            _mainForm.Show();
        }

        private void MainFormOnShown(object? sender, EventArgs e)
        {
            Hide();
            _mainForm?.Focus();
        }

        private void MainFormOnLoad(object? sender, EventArgs e)
        {
            SetMessage("Initialization Complete");
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
            // Create a rounded region for the form
            using var path = new GraphicsPath();
            path.AddRoundRectangle(new Rectangle(0, 0, Width, Height), 20); // 20 is the corner radius
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
