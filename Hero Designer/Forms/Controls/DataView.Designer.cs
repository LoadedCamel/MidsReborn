using System.Drawing;
using System.Windows.Forms;
using midsControls;

namespace Hero_Designer.Forms.Controls
{
    partial class DataView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pnlTabs = new Panel();
            pnlInfo = new Panel();
            PowerScaler = new ctlMultiGraph();
            info_txtSmall = new RichTextBox();
            lblDmg = new Label();
            Info_Damage = new ctlDamageDisplay();
            info_DataList = new ctlPairedList();
            Info_txtLarge = new RichTextBox();
            info_Title = new GFXLabel();
            pnlFX = new Panel();
            fx_Title = new GFXLabel();
            fx_LblHead3 = new Label();
            fx_List3 = new ctlPairedList();
            fx_lblHead2 = new Label();
            fx_lblHead1 = new Label();
            fx_List2 = new ctlPairedList();
            fx_List1 = new ctlPairedList();
            pnlTotal = new Panel();
            lblTotal = new Label();
            gRes2 = new ctlMultiGraph();
            gRes1 = new ctlMultiGraph();
            gDef2 = new ctlMultiGraph();
            gDef1 = new ctlMultiGraph();
            total_Title = new GFXLabel();
            total_lblMisc = new Label();
            total_Misc = new ctlPairedList();
            total_lblRes = new Label();
            total_lblDef = new Label();
            pnlEnh = new Panel();
            pnlEnhInactive = new Panel();
            pnlEnhActive = new Panel();
            enhNameDisp = new GFXLabel();
            enhListing = new ctlPairedList();
            Enh_Title = new GFXLabel();
            dbTip = new ToolTip(components);
            lblFloat = new Label();
            lblShrink = new Label();
            lblLock = new Label();
            pnlInfo.SuspendLayout();
            pnlFX.SuspendLayout();
            pnlTotal.SuspendLayout();
            pnlEnh.SuspendLayout();
            SuspendLayout();

            pnlTabs.BackColor = Color.FromArgb(64, 64, 64);
            pnlTabs.Location = new Point(0, 0);
            pnlTabs.Name = "pnlTabs";
            pnlTabs.Size = new Size(300, 20);
            pnlTabs.TabIndex = 61;

            pnlInfo.BackColor = Color.Navy;
            pnlInfo.Controls.Add(PowerScaler);
            pnlInfo.Controls.Add(info_txtSmall);
            pnlInfo.Controls.Add(lblDmg);
            pnlInfo.Controls.Add(Info_Damage);
            pnlInfo.Controls.Add(info_DataList);
            pnlInfo.Controls.Add(Info_txtLarge);
            pnlInfo.Controls.Add(info_Title);
            pnlInfo.Location = new Point(0, 20);
            pnlInfo.Name = "pnlInfo";
            pnlInfo.Size = new Size(300, 428);
            pnlInfo.TabIndex = 62;

            PowerScaler.BackColor = Color.Black;
            PowerScaler.Border = true;
            PowerScaler.Clickable = true;
            PowerScaler.ColorBase = Color.FromArgb(64, byte.MaxValue, 64);
            PowerScaler.ColorEnh = Color.Yellow;
            PowerScaler.ColorFadeEnd = Color.FromArgb(0, 192, 0);
            PowerScaler.ColorFadeStart = Color.Black;
            PowerScaler.ColorHighlight = Color.Gray;
            PowerScaler.ColorLines = Color.Black;
            PowerScaler.ColorMarkerInner = Color.Red;
            PowerScaler.ColorMarkerOuter = Color.Black;
            PowerScaler.Dual = false;
            PowerScaler.Font = new Font("Arial", 9f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            PowerScaler.ForcedMax = 0.0f;
            PowerScaler.ForeColor = Color.FromArgb(192, 192, byte.MaxValue);
            PowerScaler.Highlight = true;
            PowerScaler.ItemHeight = 10;
            PowerScaler.Lines = true;
            PowerScaler.Location = new Point(4, 145);
            PowerScaler.MarkerValue = 0.0f;
            PowerScaler.Max = 100f;
            PowerScaler.Name = "PowerScaler";
            PowerScaler.PaddingX = 2f;
            PowerScaler.PaddingY = 2f;
            PowerScaler.ScaleHeight = 32;
            PowerScaler.ScaleIndex = 8;
            PowerScaler.ShowScale = false;
            PowerScaler.Size = new Size(292, 15);
            PowerScaler.Style = Enums.GraphStyle.baseOnly;
            PowerScaler.TabIndex = 71;
            PowerScaler.TextWidth = 80;

            info_txtSmall.BackColor = Color.FromArgb(64, 64, 64);
            info_txtSmall.BorderStyle = BorderStyle.None;
            info_txtSmall.Cursor = Cursors.Arrow;
            info_txtSmall.ForeColor = Color.White;
            info_txtSmall.Location = new Point(4, 24);
            info_txtSmall.Name = "info_txtSmall";
            info_txtSmall.ReadOnly = true;
            info_txtSmall.ScrollBars = RichTextBoxScrollBars.None;
            info_txtSmall.Size = new Size(292, 32);
            info_txtSmall.TabIndex = 70;
            info_txtSmall.Text = "info_Rich";

            lblDmg.Font = new Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDmg.ForeColor = Color.White;
            lblDmg.Location = new Point(4, 272);
            lblDmg.Name = "lblDmg";
            lblDmg.Size = new Size(292, 15);
            lblDmg.TabIndex = 15;
            lblDmg.Text = "Damage:";
            lblDmg.TextAlign = ContentAlignment.MiddleCenter;

            Info_Damage.BackColor = Color.FromArgb(64, 64, 64);
            Info_Damage.ColorBackEnd = Color.Red;
            Info_Damage.ColorBackStart = Color.Black;
            Info_Damage.ColorBaseEnd = Color.Blue;
            Info_Damage.ColorBaseStart = Color.Blue;
            Info_Damage.ColorEnhEnd = Color.Yellow;
            Info_Damage.ColorEnhStart = Color.Yellow;
            Info_Damage.Font = new Font("Arial", 11.5f, FontStyle.Bold, GraphicsUnit.Pixel, 1);
            Info_Damage.GraphType = Enums.eDDGraph.Enhanced;
            Info_Damage.Location = new Point(2, 284);
            Info_Damage.Name = "Info_Damage";
            Info_Damage.nBaseVal = 100f;
            Info_Damage.nEnhVal = 150f;
            Info_Damage.nHighBase = 200f;
            Info_Damage.nHighEnh = 214f;
            Info_Damage.nMaxEnhVal = 207f;
            Info_Damage.PaddingH = 1;
            Info_Damage.PaddingV = 1;
            Info_Damage.Size = new Size(295, 56);
            Info_Damage.Style = Enums.eDDStyle.Text;
            Info_Damage.TabIndex = 20;
            Info_Damage.TextAlign = Enums.eDDAlign.Left;
            Info_Damage.TextColor = Color.FromArgb(192, 192, byte.MaxValue);

            info_DataList.BackColor = Color.FromArgb(0, 0, 32);
            info_DataList.Columns = 2;
            info_DataList.DoHighlight = true;
            info_DataList.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Pixel, 1);
            info_DataList.ForceBold = false;
            info_DataList.HighlightColor = Color.FromArgb(128, 128, byte.MaxValue);
            info_DataList.HighlightTextColor = Color.Black;
            info_DataList.ItemColor = Color.White;
            info_DataList.ItemColorAlt = Color.Lime;
            info_DataList.ItemColorSpecCase = Color.Red;
            info_DataList.ItemColorSpecial = Color.FromArgb(128, 128, byte.MaxValue);
            info_DataList.Location = new Point(4, 164);
            info_DataList.Name = "info_DataList";
            info_DataList.NameColor = Color.FromArgb(192, 192, byte.MaxValue);
            info_DataList.Size = new Size(292, 104);
            info_DataList.TabIndex = 19;
            info_DataList.ValueWidth = 55;

            Info_txtLarge.BackColor = Color.FromArgb(64, 64, 64);
            Info_txtLarge.BorderStyle = BorderStyle.None;
            Info_txtLarge.Cursor = Cursors.Arrow;
            Info_txtLarge.ForeColor = Color.White;
            Info_txtLarge.Location = new Point(4, 60);
            Info_txtLarge.Name = "Info_txtLarge";
            Info_txtLarge.ReadOnly = true;
            Info_txtLarge.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
            Info_txtLarge.Size = new Size(292, 87);
            Info_txtLarge.TabIndex = 69;
            Info_txtLarge.Text = "info_Rich";

            info_Title.BackColor = Color.FromArgb(64, 64, 64);
            info_Title.Font = new Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            info_Title.ForeColor = Color.White;
            info_Title.InitialText = "Title";
            info_Title.Location = new Point(24, 4);
            info_Title.Name = "info_Title";
            info_Title.Size = new Size(252, 16);
            info_Title.TabIndex = 69;
            info_Title.TextAlign = ContentAlignment.TopCenter;

            pnlFX.BackColor = Color.Indigo;
            pnlFX.Controls.Add(fx_Title);
            pnlFX.Controls.Add(fx_LblHead3);
            pnlFX.Controls.Add(fx_List3);
            pnlFX.Controls.Add(fx_lblHead2);
            pnlFX.Controls.Add(fx_lblHead1);
            pnlFX.Controls.Add(fx_List2);
            pnlFX.Controls.Add(fx_List1);
            pnlFX.Location = new Point(144, 148);
            pnlFX.Name = "pnlFX";
            pnlFX.Size = new Size(300, 420);
            pnlFX.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Pixel);
            pnlFX.TabIndex = 63;

            fx_Title.BackColor = Color.FromArgb(64, 64, 64);
            fx_Title.Font = new Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            fx_Title.ForeColor = Color.White;
            fx_Title.InitialText = "Title";
            fx_Title.Location = new Point(24, 4);
            fx_Title.Name = "fx_Title";
            fx_Title.Size = new Size(252, 16);
            fx_Title.TabIndex = 70;
            fx_Title.TextAlign = ContentAlignment.TopCenter;

            fx_LblHead3.Font = new Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            fx_LblHead3.ForeColor = Color.White;
            fx_LblHead3.Location = new Point(4, 228);
            fx_LblHead3.Name = "fx_LblHead3";
            fx_LblHead3.Size = new Size(292, 16);
            fx_LblHead3.TabIndex = 28;
            fx_LblHead3.Text = "Misc Effects:";
            fx_LblHead3.TextAlign = ContentAlignment.MiddleLeft;

            fx_List3.BackColor = Color.FromArgb(64, 64, 64);
            fx_List3.Columns = 2;
            fx_List3.DoHighlight = true;
            fx_List3.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Pixel, 1);
            fx_List3.ForceBold = false;
            fx_List3.HighlightColor = Color.FromArgb(128, 128, byte.MaxValue);
            fx_List3.HighlightTextColor = Color.Black;
            fx_List3.ItemColor = Color.White;
            fx_List3.ItemColorAlt = Color.Lime;
            fx_List3.ItemColorSpecCase = Color.Red;
            fx_List3.ItemColorSpecial = Color.FromArgb(128, 128, byte.MaxValue);
            fx_List3.Location = new Point(4, 244);
            fx_List3.Name = "fx_List3";
            fx_List3.NameColor = Color.FromArgb(192, 192, byte.MaxValue);
            fx_List3.Size = new Size(292, 72);
            fx_List3.TabIndex = 27;
            fx_List3.ValueWidth = 55;

            fx_lblHead2.Font = new Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            fx_lblHead2.ForeColor = Color.White;
            fx_lblHead2.Location = new Point(4, 136);
            fx_lblHead2.Name = "fx_lblHead2";
            fx_lblHead2.Size = new Size(292, 16);
            fx_lblHead2.TabIndex = 26;
            fx_lblHead2.Text = "Secondary Effects:";
            fx_lblHead2.TextAlign = ContentAlignment.MiddleLeft;

            fx_lblHead1.Font = new Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            fx_lblHead1.ForeColor = Color.White;
            fx_lblHead1.Location = new Point(4, 24);
            fx_lblHead1.Name = "fx_lblHead1";
            fx_lblHead1.Size = new Size(292, 16);
            fx_lblHead1.TabIndex = 25;
            fx_lblHead1.Text = "Primary Effects:";
            fx_lblHead1.TextAlign = ContentAlignment.MiddleLeft;

            fx_List2.BackColor = Color.FromArgb(64, 64, 64);
            fx_List2.Columns = 2;
            fx_List2.DoHighlight = true;
            fx_List2.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Pixel, 1);
            fx_List2.ForceBold = false;
            fx_List2.HighlightColor = Color.FromArgb(128, 128, byte.MaxValue);
            fx_List2.HighlightTextColor = Color.Black;
            fx_List2.ItemColor = Color.White;
            fx_List2.ItemColorAlt = Color.Lime;
            fx_List2.ItemColorSpecCase = Color.Red;
            fx_List2.ItemColorSpecial = Color.FromArgb(128, 128, byte.MaxValue);
            fx_List2.Location = new Point(4, 152);
            fx_List2.Name = "fx_List2";
            fx_List2.NameColor = Color.FromArgb(192, 192, byte.MaxValue);
            fx_List2.Size = new Size(292, 72);
            fx_List2.TabIndex = 24;
            fx_List2.ValueWidth = 55;

            fx_List1.BackColor = Color.FromArgb(64, 64, 64);
            fx_List1.Columns = 2;
            fx_List1.DoHighlight = true;
            fx_List1.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Pixel, 1);
            fx_List1.ForceBold = false;
            fx_List1.HighlightColor = Color.FromArgb(128, 128, byte.MaxValue);
            fx_List1.HighlightTextColor = Color.Black;
            fx_List1.ItemColor = Color.White;
            fx_List1.ItemColorAlt = Color.Lime;
            fx_List1.ItemColorSpecCase = Color.Red;
            fx_List1.ItemColorSpecial = Color.FromArgb(128, 128, byte.MaxValue);
            fx_List1.Location = new Point(4, 40);
            fx_List1.Name = "fx_List1";
            fx_List1.NameColor = Color.FromArgb(192, 192, byte.MaxValue);
            fx_List1.Size = new Size(292, 92);
            fx_List1.TabIndex = 23;
            fx_List1.ValueWidth = 60;

            pnlTotal.BackColor = Color.Green;
            pnlTotal.Controls.Add(lblTotal);
            pnlTotal.Controls.Add(gRes2);
            pnlTotal.Controls.Add(gRes1);
            pnlTotal.Controls.Add(gDef2);
            pnlTotal.Controls.Add(gDef1);
            pnlTotal.Controls.Add(total_Title);
            pnlTotal.Controls.Add(total_lblMisc);
            pnlTotal.Controls.Add(total_Misc);
            pnlTotal.Controls.Add(total_lblRes);
            pnlTotal.Controls.Add(total_lblDef);
            pnlTotal.Location = new Point(248, 15);
            pnlTotal.Name = "pnlTotal";
            pnlTotal.Size = new Size(300, 420);
            pnlTotal.TabIndex = 64;

            lblTotal.Font = new Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotal.ForeColor = Color.White;
            lblTotal.Location = new Point(4, 300);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(292, 16);
            lblTotal.TabIndex = 75;
            lblTotal.Text = "Click the 'View Totals' button for more.";
            lblTotal.TextAlign = ContentAlignment.MiddleCenter;

            gRes2.BackColor = Color.Black;
            gRes2.Border = true;
            gRes2.Clickable = false;
            gRes2.ColorBase = Color.FromArgb(0, 192, 192);
            gRes2.ColorEnh = Color.FromArgb(byte.MaxValue, 128, 128);
            gRes2.ColorFadeEnd = Color.Teal;
            gRes2.ColorFadeStart = Color.Black;
            gRes2.ColorHighlight = Color.Gray;
            gRes2.ColorLines = Color.Black;
            gRes2.ColorMarkerInner = Color.Black;
            gRes2.ColorMarkerOuter = Color.Yellow;
            gRes2.Dual = false;
            gRes2.Font = new Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            gRes2.ForcedMax = 0.0f;
            gRes2.ForeColor = Color.FromArgb(192, 192, byte.MaxValue);
            gRes2.Highlight = true;
            gRes2.ItemHeight = 13;
            gRes2.Lines = true;
            gRes2.Location = new Point(150, 152);
            gRes2.MarkerValue = 0.0f;
            gRes2.Max = 100f;
            gRes2.Name = "gRes2";
            gRes2.PaddingX = 2f;
            gRes2.PaddingY = 4f;
            gRes2.ScaleHeight = 32;
            gRes2.ScaleIndex = 8;
            gRes2.ShowScale = false;
            gRes2.Size = new Size(146, 72);
            gRes2.Style = Enums.GraphStyle.Stacked;
            gRes2.TabIndex = 74;
            gRes2.TextWidth = 100;

            gRes1.BackColor = Color.Black;
            gRes1.Border = true;
            gRes1.Clickable = false;
            gRes1.ColorBase = Color.FromArgb(0, 192, 192);
            gRes1.ColorEnh = Color.FromArgb(byte.MaxValue, 128, 128);
            gRes1.ColorFadeEnd = Color.Teal;
            gRes1.ColorFadeStart = Color.Black;
            gRes1.ColorHighlight = Color.Gray;
            gRes1.ColorLines = Color.Black;
            gRes1.ColorMarkerInner = Color.Black;
            gRes1.ColorMarkerOuter = Color.Yellow;
            gRes1.Dual = false;
            gRes1.Font = new Font("Arial", 10f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            gRes1.ForcedMax = 0.0f;
            gRes1.ForeColor = Color.FromArgb(192, 192, byte.MaxValue);
            gRes1.Highlight = true;
            gRes1.ItemHeight = 13;
            gRes1.Lines = true;
            gRes1.Location = new Point(4, 152);
            gRes1.MarkerValue = 0.0f;
            gRes1.Max = 100f;
            gRes1.Name = "gRes1";
            gRes1.PaddingX = 2f;
            gRes1.PaddingY = 4f;
            gRes1.ScaleHeight = 32;
            gRes1.ScaleIndex = 8;
            gRes1.ShowScale = false;
            gRes1.Size = new Size(146, 72);
            gRes1.Style = Enums.GraphStyle.Stacked;
            gRes1.TabIndex = 73;
            gRes1.TextWidth = 100;

            gDef2.BackColor = Color.Black;
            gDef2.Border = true;
            gDef2.Clickable = false;
            gDef2.ColorBase = Color.FromArgb(192, 0, 192);
            gDef2.ColorEnh = Color.Yellow;
            gDef2.ColorFadeEnd = Color.Purple;
            gDef2.ColorFadeStart = Color.Black;
            gDef2.ColorHighlight = Color.Gray;
            gDef2.ColorLines = Color.Black;
            gDef2.ColorMarkerInner = Color.Black;
            gDef2.ColorMarkerOuter = Color.Yellow;
            gDef2.Dual = false;
            gDef2.Font = new Font("Arial", 10f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            gDef2.ForcedMax = 0.0f;
            gDef2.ForeColor = Color.FromArgb(192, 192, byte.MaxValue);
            gDef2.Highlight = true;
            gDef2.ItemHeight = 13;
            gDef2.Lines = true;
            gDef2.Location = new Point(150, 40);
            gDef2.MarkerValue = 0.0f;
            gDef2.Max = 100f;
            gDef2.Name = "gDef2";
            gDef2.PaddingX = 2f;
            gDef2.PaddingY = 4f;
            gDef2.ScaleHeight = 32;
            gDef2.ScaleIndex = 8;
            gDef2.ShowScale = false;
            gDef2.Size = new Size(146, 92);
            gDef2.Style = Enums.GraphStyle.baseOnly;
            gDef2.TabIndex = 72;
            gDef2.TextWidth = 100;

            gDef1.BackColor = Color.Black;
            gDef1.Border = true;
            gDef1.Clickable = false;
            gDef1.ColorBase = Color.FromArgb(192, 0, 192);
            gDef1.ColorEnh = Color.Yellow;
            gDef1.ColorFadeEnd = Color.Purple;
            gDef1.ColorFadeStart = Color.Black;
            gDef1.ColorHighlight = Color.Gray;
            gDef1.ColorLines = Color.Black;
            gDef1.ColorMarkerInner = Color.Black;
            gDef1.ColorMarkerOuter = Color.Yellow;
            gDef1.Dual = false;
            gDef1.Font = new Font("Arial", 10f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            gDef1.ForcedMax = 0.0f;
            gDef1.ForeColor = Color.FromArgb(192, 192, byte.MaxValue);
            gDef1.Highlight = true;
            gDef1.ItemHeight = 13;
            gDef1.Lines = true;
            gDef1.Location = new Point(4, 40);
            gDef1.MarkerValue = 0.0f;
            gDef1.Max = 100f;
            gDef1.Name = "gDef1";
            gDef1.PaddingX = 2f;
            gDef1.PaddingY = 4f;
            gDef1.ScaleHeight = 32;
            gDef1.ScaleIndex = 8;
            gDef1.ShowScale = false;
            gDef1.Size = new Size(146, 92);
            gDef1.Style = Enums.GraphStyle.baseOnly;
            gDef1.TabIndex = 71;
            gDef1.TextWidth = 100;

            total_Title.BackColor = Color.FromArgb(64, 64, 64);
            total_Title.Font = new Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            total_Title.ForeColor = Color.White;
            total_Title.InitialText = "Cumulative Totals (For Self)";
            total_Title.Location = new Point(24, 4);
            total_Title.Name = "total_Title";
            total_Title.Size = new Size(252, 16);
            total_Title.TabIndex = 70;
            total_Title.TextAlign = ContentAlignment.TopCenter;

            total_lblMisc.Font = new Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            total_lblMisc.ForeColor = Color.White;
            total_lblMisc.Location = new Point(4, 228);
            total_lblMisc.Name = "total_lblMisc";
            total_lblMisc.Size = new Size(292, 16);
            total_lblMisc.TabIndex = 28;
            total_lblMisc.Text = "Misc Effects:";
            total_lblMisc.TextAlign = ContentAlignment.MiddleLeft;

            total_Misc.BackColor = Color.FromArgb(64, 64, 64);
            total_Misc.Columns = 2;
            total_Misc.DoHighlight = true;
            total_Misc.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 178);
            total_Misc.ForceBold = false;
            total_Misc.HighlightColor = Color.FromArgb(128, 128, byte.MaxValue);
            total_Misc.HighlightTextColor = Color.Black;
            total_Misc.ItemColor = Color.White;
            total_Misc.ItemColorAlt = Color.Lime;
            total_Misc.ItemColorSpecCase = Color.Red;
            total_Misc.ItemColorSpecial = Color.FromArgb(128, 128, byte.MaxValue);
            total_Misc.Location = new Point(4, 244);
            total_Misc.Name = "total_Misc";
            total_Misc.NameColor = Color.FromArgb(192, 192, byte.MaxValue);
            total_Misc.Size = new Size(292, 60);
            total_Misc.TabIndex = 27;
            total_Misc.ValueWidth = 55;

            total_lblRes.BackColor = Color.Green;
            total_lblRes.Font = new Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            total_lblRes.ForeColor = Color.White;
            total_lblRes.Location = new Point(4, 136);
            total_lblRes.Name = "total_lblRes";
            total_lblRes.Size = new Size(292, 16);
            total_lblRes.TabIndex = 26;
            total_lblRes.Text = "Resistance:";
            total_lblRes.TextAlign = ContentAlignment.MiddleLeft;

            total_lblDef.Font = new Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            total_lblDef.ForeColor = Color.White;
            total_lblDef.Location = new Point(4, 24);
            total_lblDef.Name = "total_lblDef";
            total_lblDef.Size = new Size(292, 16);
            total_lblDef.TabIndex = 25;
            total_lblDef.Text = "Defense:";
            total_lblDef.TextAlign = ContentAlignment.MiddleLeft;

            pnlEnh.BackColor = Color.Teal;
            pnlEnh.Controls.Add(pnlEnhInactive);
            pnlEnh.Controls.Add(pnlEnhActive);
            pnlEnh.Controls.Add(enhNameDisp);
            pnlEnh.Controls.Add(enhListing);
            pnlEnh.Controls.Add(Enh_Title);
            pnlEnh.Location = new Point(188, 156);
            pnlEnh.Name = "pnlEnh";
            pnlEnh.Size = new Size(300, 420);
            pnlEnh.TabIndex = 65;

            pnlEnhInactive.BackColor = Color.Black;
            pnlEnhInactive.Location = new Point(4, 279);
            pnlEnhInactive.Name = "pnlEnhInactive";
            pnlEnhInactive.Size = new Size(292, 38);
            pnlEnhInactive.TabIndex = 74;

            pnlEnhActive.BackColor = Color.Black;
            pnlEnhActive.Font = new Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            pnlEnhActive.Location = new Point(4, 239);
            pnlEnhActive.Name = "pnlEnhActive";
            pnlEnhActive.Size = new Size(292, 38);
            pnlEnhActive.TabIndex = 73;

            enhNameDisp.BackColor = Color.FromArgb(64, 64, 64);
            enhNameDisp.Font = new Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            enhNameDisp.ForeColor = Color.White;
            enhNameDisp.InitialText = "Title";
            enhNameDisp.Location = new Point(4, 24);
            enhNameDisp.Name = "enhNameDisp";
            enhNameDisp.Size = new Size(292, 16);
            enhNameDisp.TabIndex = 72;
            enhNameDisp.TextAlign = ContentAlignment.TopCenter;

            enhListing.BackColor = Color.FromArgb(0, 0, 32);
            enhListing.Columns = 1;
            enhListing.DoHighlight = true;
            enhListing.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 178);
            enhListing.ForceBold = false;
            enhListing.HighlightColor = Color.FromArgb(128, 128, byte.MaxValue);
            enhListing.HighlightTextColor = Color.Black;
            enhListing.ItemColor = Color.White;
            enhListing.ItemColorAlt = Color.Yellow;
            enhListing.ItemColorSpecCase = Color.Red;
            enhListing.ItemColorSpecial = Color.FromArgb(128, 128, byte.MaxValue);
            enhListing.Location = new Point(4, 44);
            enhListing.Name = "enhListing";
            enhListing.NameColor = Color.FromArgb(192, 192, byte.MaxValue);
            enhListing.Size = new Size(292, 192);
            enhListing.TabIndex = 71;
            enhListing.ValueWidth = 65;

            Enh_Title.BackColor = Color.FromArgb(64, 64, 64);
            Enh_Title.Font = new Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            Enh_Title.ForeColor = Color.White;
            Enh_Title.InitialText = "Title";
            Enh_Title.Location = new Point(24, 4);
            Enh_Title.Name = "Enh_Title";
            Enh_Title.Size = new Size(252, 16);
            Enh_Title.TabIndex = 70;
            Enh_Title.TextAlign = ContentAlignment.TopCenter;

            dbTip.AutoPopDelay = 15000;
            dbTip.InitialDelay = 350;
            dbTip.ReshowDelay = 100;
            lblFloat.BackColor = Color.FromArgb(64, 64, 64);
            lblFloat.BorderStyle = BorderStyle.FixedSingle;
            lblFloat.Font = new Font("Arial", 11f, FontStyle.Bold, GraphicsUnit.Pixel, 2);
            lblFloat.ForeColor = Color.White;

            lblFloat.Location = new Point(4, 24);
            lblFloat.Name = "lblFloat";

            lblFloat.Size = new Size(16, 16);
            lblFloat.TabIndex = 66;
            lblFloat.Text = "F";
            lblFloat.TextAlign = ContentAlignment.MiddleCenter;
            dbTip.SetToolTip(lblFloat, "Make Floating Window");
            lblFloat.UseCompatibleTextRendering = true;
            lblShrink.BackColor = Color.FromArgb(64, 64, 64);
            lblShrink.BorderStyle = BorderStyle.FixedSingle;
            lblShrink.Font = new Font("Wingdings", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 2);
            lblShrink.ForeColor = Color.White;

            lblShrink.Location = new Point(280, 24);
            lblShrink.Name = "lblShrink";

            lblShrink.Size = new Size(16, 16);
            lblShrink.TabIndex = 67;
            lblShrink.Text = "y";
            lblShrink.TextAlign = ContentAlignment.MiddleCenter;
            dbTip.SetToolTip(lblShrink, "Shrink / Expand the Info Display");
            lblShrink.UseCompatibleTextRendering = true;
            lblLock.BackColor = Color.Red;
            lblLock.BorderStyle = BorderStyle.FixedSingle;
            lblLock.Font = new Font("Arial", 8.25f, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLock.ForeColor = Color.White;

            lblLock.Location = new Point(220, 24);
            lblLock.Name = "lblLock";

            lblLock.Size = new Size(56, 16);
            lblLock.TabIndex = 68;
            lblLock.Text = "[Unlock]";
            lblLock.TextAlign = ContentAlignment.MiddleCenter;
            dbTip.SetToolTip(lblLock, "The info display is currently locked to display a specific power, click here to unlock it to display powers as you hover the mouse over them.");

            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = Color.FromArgb(0, 0, 32);
            Controls.Add(lblLock);
            Controls.Add(lblFloat);
            Controls.Add(lblShrink);
            Controls.Add(pnlTabs);
            Controls.Add(pnlInfo);
            Controls.Add(pnlTotal);
            Controls.Add(pnlEnh);
            Controls.Add(pnlFX);
            Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Pixel, 1);

            Size = new Size(564, 650);
            pnlInfo.ResumeLayout(false);
            pnlFX.ResumeLayout(false);
            pnlTotal.ResumeLayout(false);
            pnlEnh.ResumeLayout(false);
            //adding events

            // Enh_Title events
            Enh_Title.MouseMove += Title_MouseMove;
            Enh_Title.MouseDown += Title_MouseDown;

            PowerScaler.BarClick += PowerScaler_BarClick;
            enhListing.ItemHover += PairedList_Hover;
            fx_List1.ItemHover += PairedList_Hover;
            fx_List2.ItemHover += PairedList_Hover;
            fx_List3.ItemHover += PairedList_Hover;

            // fx_Title events
            fx_Title.MouseMove += Title_MouseMove;
            fx_Title.MouseDown += Title_MouseDown;

            info_DataList.ItemHover += PairedList_Hover;

            // info_Title events
            info_Title.MouseMove += Title_MouseMove;
            info_Title.MouseDown += Title_MouseDown;

            lblFloat.Click += lblFloat_Click;
            lblLock.Click += lblLock_Click;

            // lblShrink events
            lblShrink.DoubleClick += lblShrink_DoubleClick;
            lblShrink.Click += lblShrink_Click;


            // pnlEnhActive events
            pnlEnhActive.Paint += pnlEnhActive_Paint;
            pnlEnhActive.MouseMove += pnlEnhActive_MouseMove;
            pnlEnhActive.MouseClick += pnlEnhActive_MouseClick;


            // pnlEnhInactive events
            pnlEnhInactive.Paint += pnlEnhInactive_Paint;
            pnlEnhInactive.MouseMove += pnlEnhInactive_MouseMove;
            pnlEnhInactive.MouseClick += pnlEnhInactive_MouseClick;


            // pnlTabs events
            pnlTabs.MouseDown += pnlTabs_MouseDown;
            pnlTabs.Paint += pnlTabs_Paint;

            total_Misc.ItemHover += PairedList_Hover;

            // total_Title events
            total_Title.MouseMove += Title_MouseMove;
            total_Title.MouseDown += Title_MouseDown;

            // finished with events
            ResumeLayout(false);
        }

        #endregion

        private ToolTip dbTip;
        private GFXLabel Enh_Title;
        private ctlPairedList enhListing;
        private GFXLabel enhNameDisp;
        private Label fx_lblHead1;
        private Label fx_lblHead2;
        private Label fx_LblHead3;
        private ctlPairedList fx_List1;
        private ctlPairedList fx_List2;
        private ctlPairedList fx_List3;
        private GFXLabel fx_Title;
        private ctlMultiGraph gDef1;
        private ctlMultiGraph gDef2;
        private ctlMultiGraph gRes1;
        private ctlMultiGraph gRes2;
        private ctlPairedList info_DataList;
        private GFXLabel info_Title;
        private RichTextBox info_txtSmall;
        private Label lblDmg;
        private Label lblFloat;
        private Label lblLock;
        private Label lblShrink;
        private Label lblTotal;
        private Panel pnlEnh;
        private Panel pnlEnhActive;
        private Panel pnlEnhInactive;
        private Panel pnlFX;
        private Panel pnlInfo;
        private Panel pnlTabs;
        private Panel pnlTotal;
        private ctlMultiGraph PowerScaler;
        private Label total_lblDef;
        private Label total_lblMisc;
        private Label total_lblRes;
        private ctlPairedList total_Misc;
        private GFXLabel total_Title;
        internal ctlDamageDisplay Info_Damage;
        internal RichTextBox Info_txtLarge;
    }
}
