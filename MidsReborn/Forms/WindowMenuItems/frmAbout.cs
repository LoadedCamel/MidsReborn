using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using MRBResourceLib;

namespace Forms.WindowMenuItems
{
    public partial class frmAbout : Form
    {
        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;
        private const int WM_NCLBUTTONDBLCLK = 0x00A3;

        public frmAbout()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.FixedWidth | ControlStyles.FixedHeight, true);
            InitializeComponent();
        }

        private void frmAbout_Load(object sender, System.EventArgs e)
        {
            pbAppIcon.BackgroundImage = Resources.MRB_Icon_Concept.ToBitmap();
            lblAppName.Text = MidsContext.AppName;
            lblAppVersion.Text = GetAppVersionString();
            lblDbName.Text = GetDatabaseName();
            lblDbIssue.Text = GetDatabaseIssuePageVol();
            lblDbVersion.Text = GetDatabaseVersionString();

            switch (MidsContext.Character?.Alignment)
            {
                case Enums.Alignment.Villain:
                case Enums.Alignment.Rogue:
                case Enums.Alignment.Loyalist:
                    BackgroundImage = Resources.HeroesSilhouettesV;
                    btnCopy.UseAlt = true;
                    btnClose.UseAlt = true;
                    break;

                default:
                    BackgroundImage = Resources.HeroesSilhouettesH;
                    btnCopy.UseAlt = false;
                    btnClose.UseAlt = false;
                    break;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x00000020;
                return cp;
            }
        }

        // Make the window movable by click-drag
        protected override void WndProc(ref Message message)
        {
            if (message.Msg == WM_NCLBUTTONDBLCLK)
            {
                message.Result = IntPtr.Zero;
                return;
            }
            base.WndProc(ref message);

            if (message.Msg == WM_NCHITTEST && (int)message.Result == HTCLIENT)
                message.Result = (IntPtr)HTCAPTION;
        }

        private string GetAppVersionString()
        {
            return $"v{MidsContext.AppFileVersion.Major}.{MidsContext.AppFileVersion.Minor}.{MidsContext.AppFileVersion.Build}{(MidsContext.AppFileVersion.Revision <= 0 ? "" : $" rev {MidsContext.AppFileVersion.Revision}")}";
        }

        private string GetDatabaseName()
        {
            return DatabaseAPI.DatabaseName;
        }

        private string GetDatabaseIssuePageVol()
        {
            return $"{DatabaseAPI.Database.Issue}{(DatabaseAPI.Database.PageVol <= 0 ? "" : $" {DatabaseAPI.Database.PageVolText} {DatabaseAPI.Database.PageVol}")}";
        }

        private string GetDatabaseVersionString()
        {
            return $"{DatabaseAPI.Database.Version.Major}.{DatabaseAPI.Database.Version.Minor}.{DatabaseAPI.Database.Version.Build}{(DatabaseAPI.Database.Version.Revision <= 0 ? "" : $" rev {DatabaseAPI.Database.Version.Revision}")}";
        }

        private void btnCopy_Click(object sender, System.EventArgs e)
        {
            var appStr = $"{MidsContext.AppName} {GetAppVersionString()}";
            appStr += $"\r\n\r\nDatabase: {GetDatabaseName()}";
            appStr += $"\r\nDatabase Issue: {GetDatabaseIssuePageVol()}";
            appStr += $"\r\nDatabase Version: {GetDatabaseVersionString()}";
            appStr += $"\r\n\r\nRunning under .NET {Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName} / {RuntimeInformation.FrameworkDescription}";

            Clipboard.ContainsText();
            Clipboard.SetText(appStr);
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}