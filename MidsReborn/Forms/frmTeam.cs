using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Forms
{
    public partial class FrmTeam : Form
    {
        private readonly frmMain _myParent;
        private const int MaxMembers = 7;
        private int TotalMembers { get; set; }

        public FrmTeam(ref frmMain iParent)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            Load += frmTeam_OnLoad;
            InitializeComponent();
            _myParent = iParent;
        }

        private void frmTeam_OnLoad(object? sender, EventArgs e)
        {
            switch (DatabaseAPI.DatabaseName)
            {
                case "Homecoming":
                    label9.Text = @"Sentinel";
                    label9.Visible = true;
                    udSentGuard.Visible = true;
                    udSentGuard.Enabled = true;
                    break;
                case "Rebirth":
                    label9.Text = @"Guardian";
                    label9.Visible = true;
                    udSentGuard.Visible = true;
                    udSentGuard.Enabled = true;
                    break;
                default:
                    label9.Visible = false;
                    udSentGuard.Visible = false;
                    udSentGuard.Enabled = false;
                    break;
            }
            if (MidsContext.Config != null && MidsContext.Config.TeamMembers.Count > 0)
            {
                foreach (var mVp in MidsContext.Config.TeamMembers)
                {
                    TotalMembers += mVp.Value;
                }

                var udControls = tableLayoutPanel1.Controls.OfType<EnhancedUpDown>();
                foreach (var udControl in udControls)
                {
                    var name = udControl.Name;
                    var archetype = string.Empty;
                    archetype = name switch
                    {
                        "udWidow" or "udSoldier" => name.Replace("ud", "Arachnos "),
                        "SentGuard" => DatabaseAPI.DatabaseName switch
                        {
                            "Homecoming" => name.Replace("udSentGuard", "Sentinel"),
                            "Rebirth" => name.Replace("udSentGuard", "Guardian"),
                            _ => archetype
                        },
                        _ => name.Replace("ud", "")
                    };
                    if (MidsContext.Config.TeamMembers.ContainsKey(archetype))
                    {
                        udControl.Value = MidsContext.Config.TeamMembers[archetype];
                    }

                    /*if (name != "udWidow" && name != "udSoldier")
                    {
                        archetype = name.Replace("ud", "");
                        if (MidsContext.Config.TeamMembers.ContainsKey(archetype))
                        {
                            udControl.Value = MidsContext.Config.TeamMembers[archetype];
                        }
                    }
                    else
                    {
                        archetype = name.Replace("ud", "Arachnos ");
                        if (MidsContext.Config.TeamMembers.ContainsKey(archetype))
                        {
                            udControl.Value = MidsContext.Config.TeamMembers[archetype];
                        }
                    }*/
                }
            }
            else
            {
                TotalMembers = 0;
            }
            tbTotalTeam.Text = Convert.ToString(TotalMembers);
        }

        private void btnSave_Paint(object sender, PaintEventArgs e)
        {
            const string iStr = "Save & Close";
            var rectangle = new Rectangle();
            ref var local = ref rectangle;
            var size = MidsContext.Character.IsHero()
                ? _myParent.Drawing.bxPower[2].Size
                : _myParent.Drawing.bxPower[4].Size;
            var width = size.Width;
            size = MidsContext.Character.IsHero()
                ? _myParent.Drawing.bxPower[2].Size
                : _myParent.Drawing.bxPower[4].Size;
            var height1 = size.Height;
            local = new Rectangle(0, 0, width, height1);
            var destRect = new Rectangle(0, 0, 105, 22);
            using var stringFormat = new StringFormat();
            using var bFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold, GraphicsUnit.Point);
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            using var extendedBitmap = new ExtendedBitmap(destRect.Width, destRect.Height);
            extendedBitmap.Graphics.Clear(BackColor);
            extendedBitmap.Graphics.DrawImage(
                MidsContext.Character.IsHero()
                    ? _myParent.Drawing.bxPower[2].Bitmap
                    : _myParent.Drawing.bxPower[4].Bitmap, destRect, 0, 0, rectangle.Width, rectangle.Height,
                GraphicsUnit.Pixel, _myParent.Drawing.pImageAttributes);
            var height2 = bFont.GetHeight(e.Graphics) + 2f;
            var bounds = new RectangleF(0.0f, (float)((22 - (double)height2) / 2.0), 105, height2);
            var graphics = extendedBitmap.Graphics;
            clsDrawX.DrawOutlineText(iStr, bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1f, graphics);
            e.Graphics.DrawImage(extendedBitmap.Bitmap, 0, 0);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MidsContext.Config?.SaveConfig(Serializer.GetSerializer());
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void btnCancel_Paint(object sender, PaintEventArgs e)
        {
            const string iStr = "Cancel";
            var rectangle = new Rectangle();
            ref var local = ref rectangle;
            var size = MidsContext.Character.IsHero()
                ? _myParent.Drawing.bxPower[2].Size
                : _myParent.Drawing.bxPower[4].Size;
            var width = size.Width;
            size = MidsContext.Character.IsHero()
                ? _myParent.Drawing.bxPower[2].Size
                : _myParent.Drawing.bxPower[4].Size;
            var height1 = size.Height;
            local = new Rectangle(0, 0, width, height1);
            var destRect = new Rectangle(0, 0, 105, 22);
            using var stringFormat = new StringFormat();
            using var bFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold, GraphicsUnit.Point);
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            using var extendedBitmap = new ExtendedBitmap(destRect.Width, destRect.Height);
            extendedBitmap.Graphics.Clear(BackColor);
            extendedBitmap.Graphics.DrawImage(
                MidsContext.Character.IsHero()
                    ? _myParent.Drawing.bxPower[2].Bitmap
                    : _myParent.Drawing.bxPower[4].Bitmap, destRect, 0, 0, rectangle.Width, rectangle.Height,
                GraphicsUnit.Pixel, _myParent.Drawing.pImageAttributes);
            var height2 = bFont.GetHeight(e.Graphics) + 2f;
            var bounds = new RectangleF(0.0f, (float)((22 - (double)height2) / 2.0), 105, height2);
            var graphics = extendedBitmap.Graphics;
            clsDrawX.DrawOutlineText(iStr, bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1f, graphics);
            e.Graphics.DrawImage(extendedBitmap.Bitmap, 0, 0);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void OnUpClicked(object sender, EventArgs e)
        {
            var udControl = (NumericUpDown)sender;
            if (TotalMembers < MaxMembers)
            {
                udControl.Value += 1;
                TotalMembers += 1;
            }
            tbTotalTeam.Text = Convert.ToString(TotalMembers);
            UpdateMembers(udControl.Name, Convert.ToInt32(udControl.Value));
        }

        private void OnDownClicked(object sender, EventArgs e)
        {
            var udControl = (NumericUpDown)sender;
            if (TotalMembers > 0 && udControl.Value != 0)
            {
                udControl.Value -= 1;
                TotalMembers -= 1;
            }
            tbTotalTeam.Text = Convert.ToString(TotalMembers);
            UpdateMembers(udControl.Name, Convert.ToInt32(udControl.Value));
        }

        private static void UpdateMembers(string name, int value)
        {
            string archetype;
            var dictTm = MidsContext.Config?.TeamMembers;
            if (name != "udWidow" && name != "udSoldier")
            {
                archetype = name.Replace("ud", "");
                if (dictTm != null && dictTm.ContainsKey(archetype))
                {
                    if (value != 0)
                    {
                        dictTm[archetype] = value;
                    }
                    else
                    {
                        dictTm.Remove(archetype);
                    }
                }
                else
                {
                    if (value != 0)
                    {
                        dictTm?.Add(archetype, value);
                    }
                }
            }
            else
            {
                archetype = name.Replace("ud", "Arachnos ");
                if (dictTm != null && dictTm.ContainsKey(archetype))
                {
                    if (value != 0)
                    {
                        dictTm[archetype] = value;
                    }
                    else
                    {
                        dictTm.Remove(archetype);
                    }
                }
                else
                {
                    if (value != 0)
                    {
                        dictTm?.Add(archetype, value);
                    }
                }

            }
        }
    }

    public class EnhancedUpDown : NumericUpDown
    {
        public event EventHandler? UpButtonClicked;
        public event EventHandler? DownButtonClicked;

        public override void UpButton()
        {
            UpButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        public override void DownButton()
        {
            DownButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
