using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using MRBResourceLib;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;

namespace Mids_Reborn.Forms.WindowMenuItems
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

        private void frmAbout_Load(object sender, EventArgs e)
        {
            pbAppIcon.Image = Resources.MRB_Icon_Concept.ToBitmap();
            switch (MidsContext.Character?.Alignment)
            {
                case Enums.Alignment.Villain:
                case Enums.Alignment.Rogue:
                case Enums.Alignment.Loyalist:
                    btnCopy.UseAlt = true;
                    btnClose.UseAlt = true;
                    break;

                default:
                    btnCopy.UseAlt = false;
                    btnClose.UseAlt = false;
                    break;
            }

            Refresh();
        }

        private void DrawOutlineText(Graphics g, string text, Point location, Font font, Color textColor,
            Color outlineColor, float outlineWidth, StringFormat sFormat)
        {
            using var outlinePen = new Pen(outlineColor, outlineWidth) {LineJoin = LineJoin.Round};
            using var brush = new SolidBrush(textColor);
            using var gfxPath = new GraphicsPath();

            gfxPath.AddString(text, font.FontFamily, (int) font.Style, font.Size, new Rectangle(location.X, location.Y, 300, 30), sFormat);
            outlinePen.LineJoin = LineJoin.Round;

            g.DrawPath(outlinePen, gfxPath);
            g.FillPath(brush, gfxPath);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var bgImage = MidsContext.Character?.Alignment switch
            {
                Enums.Alignment.Villain or Enums.Alignment.Rogue or Enums.Alignment.Loyalist => Resources.HeroesSilhouettesV,
                _ => Resources.HeroesSilhouettesH
            };

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            e.Graphics.Clear(Color.Black);
            e.Graphics.DrawImage(bgImage, 0, 0, 612, 344);

            using var fontBig = new Font("MS Sans Serif", 17, FontStyle.Bold, GraphicsUnit.Pixel);
            using var fontNormal = new Font("MS Sans Serif", 14, FontStyle.Bold, GraphicsUnit.Pixel);
            using var sFormat = new StringFormat
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near,
                Trimming = StringTrimming.None
            };

            const float outlineWidth = 3;

            // App name
            DrawOutlineText(e.Graphics, MidsContext.AppName, new Point(366, 21), fontBig, Color.WhiteSmoke, Color.Black, outlineWidth, sFormat);

            // App version
            DrawOutlineText(e.Graphics, GetAppVersionString(), new Point(337, 52), fontNormal, Color.WhiteSmoke, Color.Black, outlineWidth, sFormat);

            // DB name
            DrawOutlineText(e.Graphics, "Database:", new Point(356, 100), fontNormal, Color.WhiteSmoke, Color.Black, outlineWidth, sFormat);
            DrawOutlineText(e.Graphics, GetDatabaseName(), new Point(467, 100), fontNormal, Color.WhiteSmoke, Color.Black, outlineWidth, sFormat);

            // DB issue
            DrawOutlineText(e.Graphics, "DB Issue:", new Point(356, 130), fontNormal, Color.WhiteSmoke, Color.Black, outlineWidth, sFormat);
            DrawOutlineText(e.Graphics, GetDatabaseIssuePageVol(), new Point(467, 130), fontNormal, Color.WhiteSmoke, Color.Black, outlineWidth, sFormat);

            // DB version label
            DrawOutlineText(e.Graphics, "DB Version:", new Point(356, 160), fontNormal, Color.WhiteSmoke, Color.Black, outlineWidth, sFormat);
            DrawOutlineText(e.Graphics, GetDatabaseVersionString(), new Point(467, 160), fontNormal, Color.WhiteSmoke, Color.Black, outlineWidth, sFormat);
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

        private void btnCopy_Click(object sender, EventArgs e)
        {
            var appStr = $"{MidsContext.AppName} {GetAppVersionString()}";
            appStr += $"\r\n\r\nDatabase: {GetDatabaseName()}";
            appStr += $"\r\nDatabase Issue: {GetDatabaseIssuePageVol()}";
            appStr += $"\r\nDatabase Version: {GetDatabaseVersionString()}";
            appStr += $"\r\n\r\nRunning under {Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName} / {RuntimeInformation.FrameworkDescription}";

            Clipboard.ContainsText();
            Clipboard.SetText(appStr);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}