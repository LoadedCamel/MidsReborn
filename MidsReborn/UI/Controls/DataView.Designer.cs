using Mids_Reborn.Core;
using Mids_Reborn.UI.Controls;

namespace Mids_Reborn.UI.Forms.Controls
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataView));
            pnlTabs = new Panel();
            pnlInfo = new Panel();
            PowerScaler = new CtlMultiGraph();
            info_txtSmall = new RichTextBox();
            lblDmg = new Label();
            Info_Damage = new ctlDamageDisplay();
            info_DataList = new PairedListEx();
            Info_txtLarge = new RichTextBox();
            info_Title = new Label();
            pnlFX = new Panel();
            fx_Title = new Label();
            fx_LblHead3 = new Label();
            fx_List3 = new PairedListEx();
            fx_lblHead2 = new Label();
            fx_lblHead1 = new Label();
            fx_List2 = new PairedListEx();
            fx_List1 = new PairedListEx();
            pnlTotal = new Panel();
            lblTotal = new Label();
            gRes2 = new CtlMultiGraph();
            gRes1 = new CtlMultiGraph();
            gDef2 = new CtlMultiGraph();
            gDef1 = new CtlMultiGraph();
            total_Title = new Label();
            total_lblMisc = new Label();
            total_Misc = new PairedListEx();
            total_lblRes = new Label();
            total_lblDef = new Label();
            pnlEnh = new Panel();
            pnlEnhInactive = new Panel();
            pnlEnhActive = new Panel();
            enhNameDisp = new Label();
            enhListing = new PairedListEx();
            Enh_Title = new Label();
            dbTip = new ToolTip(components);
            lblFloat = new Label();
            lblShrink = new Label();
            lblLock = new Label();
            pnlInfo.SuspendLayout();
            pnlFX.SuspendLayout();
            pnlTotal.SuspendLayout();
            pnlEnh.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTabs
            // 
            pnlTabs.BackColor = Color.FromArgb(64, 64, 64);
            pnlTabs.Location = new Point(0, 0);
            pnlTabs.Name = "pnlTabs";
            pnlTabs.Size = new Size(300, 20);
            pnlTabs.TabIndex = 61;
            pnlTabs.Paint += pnlTabs_Paint;
            pnlTabs.MouseDown += pnlTabs_MouseDown;
            // 
            // pnlInfo
            // 
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
            pnlInfo.Size = new Size(300, 439);
            pnlInfo.TabIndex = 62;
            // 
            // PowerScaler
            // 
            PowerScaler.BackColor = Color.Black;
            PowerScaler.BackgroundImage = (Image)resources.GetObject("PowerScaler.BackgroundImage");
            PowerScaler.BaseBarColors = (List<Color>)resources.GetObject("PowerScaler.BaseBarColors");
            PowerScaler.Border = true;
            PowerScaler.BorderColor = Color.Black;
            PowerScaler.Clickable = true;
            PowerScaler.ColorAbsorbed = Color.Gainsboro;
            PowerScaler.ColorBase = Color.FromArgb(64, 255, 64);
            PowerScaler.ColorEnh = Color.Yellow;
            PowerScaler.ColorFadeEnd = Color.FromArgb(0, 192, 0);
            PowerScaler.ColorFadeStart = Color.Black;
            PowerScaler.ColorHighlight = Color.Gray;
            PowerScaler.ColorLines = Color.Black;
            PowerScaler.ColorMarkerInner = Color.Red;
            PowerScaler.ColorMarkerOuter = Color.Black;
            PowerScaler.ColorOvercap = Color.Black;
            PowerScaler.DifferentiateColors = false;
            PowerScaler.DrawRuler = false;
            PowerScaler.Dual = false;
            PowerScaler.EnhBarColors = (List<Color>)resources.GetObject("PowerScaler.EnhBarColors");
            PowerScaler.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold, GraphicsUnit.Pixel);
            PowerScaler.ForcedMax = 0F;
            PowerScaler.ForeColor = Color.FromArgb(192, 192, 255);
            PowerScaler.Highlight = false;
            PowerScaler.ItemFontSizeOverride = 0F;
            PowerScaler.ItemHeight = 10;
            PowerScaler.Lines = true;
            PowerScaler.Location = new Point(4, 145);
            PowerScaler.MarkerValue = 0F;
            PowerScaler.Max = 100F;
            PowerScaler.MaxItems = 1;
            PowerScaler.Name = "PowerScaler";
            PowerScaler.OuterBorder = false;
            PowerScaler.Overcap = false;
            PowerScaler.OvercapColors = (List<Color>)resources.GetObject("PowerScaler.OvercapColors");
            PowerScaler.PaddingX = 2F;
            PowerScaler.PaddingY = 2F;
            PowerScaler.PerItemScales = (List<float>)resources.GetObject("PowerScaler.PerItemScales");
            PowerScaler.RulerPos = CtlMultiGraph.RulerPosition.Top;
            PowerScaler.ScaleHeight = 32;
            PowerScaler.ScaleIndex = 8;
            PowerScaler.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            PowerScaler.ShowScale = false;
            PowerScaler.SingleLineLabels = true;
            PowerScaler.Size = new Size(292, 15);
            PowerScaler.Style = Enums.GraphStyle.baseOnly;
            PowerScaler.TabIndex = 71;
            PowerScaler.TextWidth = 80;
            PowerScaler.BarClick += PowerScaler_BarClick;
            // 
            // info_txtSmall
            // 
            info_txtSmall.BackColor = Color.FromArgb(64, 64, 64);
            info_txtSmall.BorderStyle = BorderStyle.None;
            info_txtSmall.ForeColor = Color.White;
            info_txtSmall.Location = new Point(4, 24);
            info_txtSmall.Name = "info_txtSmall";
            info_txtSmall.ReadOnly = true;
            info_txtSmall.ScrollBars = RichTextBoxScrollBars.None;
            info_txtSmall.Size = new Size(292, 32);
            info_txtSmall.TabIndex = 70;
            info_txtSmall.Text = "info_Rich";
            // 
            // lblDmg
            // 
            lblDmg.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            lblDmg.ForeColor = Color.White;
            lblDmg.Location = new Point(3, 287);
            lblDmg.Name = "lblDmg";
            lblDmg.Size = new Size(292, 17);
            lblDmg.TabIndex = 15;
            lblDmg.Text = "Damage (Green = Base | Red = Enhanced)";
            lblDmg.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Info_Damage
            // 
            Info_Damage.BackColor = Color.FromArgb(64, 64, 64);
            Info_Damage.ColorBackEnd = Color.Red;
            Info_Damage.ColorBackStart = Color.Black;
            Info_Damage.ColorBaseEnd = Color.Blue;
            Info_Damage.ColorBaseStart = Color.Blue;
            Info_Damage.ColorEnhEnd = Color.Yellow;
            Info_Damage.ColorEnhStart = Color.Yellow;
            Info_Damage.Font = new Font("Segoe UI", 12.25F, FontStyle.Bold, GraphicsUnit.Pixel);
            Info_Damage.GraphType = Enums.eDDGraph.Enhanced;
            Info_Damage.Location = new Point(2, 307);
            Info_Damage.Name = "Info_Damage";
            Info_Damage.nBaseVal = 100F;
            Info_Damage.nEnhVal = 150F;
            Info_Damage.nHighBase = 200F;
            Info_Damage.nHighEnh = 214F;
            Info_Damage.nMaxEnhVal = 207F;
            Info_Damage.PaddingH = 1;
            Info_Damage.PaddingV = 1;
            Info_Damage.Size = new Size(295, 114);
            Info_Damage.Style = Enums.eDDStyle.Text;
            Info_Damage.TabIndex = 20;
            Info_Damage.TextAlign = Enums.eDDAlign.Left;
            Info_Damage.TextColor = Color.FromArgb(192, 192, 255);
            // 
            // info_DataList
            // 
            info_DataList.AutoScroll = true;
            info_DataList.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            info_DataList.BackColor = Color.FromArgb(0, 0, 32);
            info_DataList.Font = new Font("Segoe UI", 9.25F);
            info_DataList.HighlightColor = Color.FromArgb(128, 128, 255);
            info_DataList.HighlightTextColor = Color.Black;
            info_DataList.ItemColor = Color.White;
            info_DataList.Location = new Point(4, 164);
            info_DataList.Margin = new Padding(4);
            info_DataList.Name = "info_DataList";
            info_DataList.SetItemsBold = false;
            info_DataList.Size = new Size(292, 122);
            info_DataList.TabIndex = 19;
            info_DataList.UseHighlighting = true;
            info_DataList.ValueAlternateColor = Color.Chartreuse;
            info_DataList.ValueColor = Color.WhiteSmoke;
            info_DataList.ValueConditionColor = Color.Firebrick;
            info_DataList.ValueSpecialColor = Color.SlateBlue;
            info_DataList.ItemHover += PairedList_Hover;
            info_DataList.ItemOut += PairedList_ItemOut;
            // 
            // Info_txtLarge
            // 
            Info_txtLarge.BackColor = Color.FromArgb(64, 64, 64);
            Info_txtLarge.BorderStyle = BorderStyle.None;
            Info_txtLarge.ForeColor = Color.White;
            Info_txtLarge.Location = new Point(4, 60);
            Info_txtLarge.Name = "Info_txtLarge";
            Info_txtLarge.ReadOnly = true;
            Info_txtLarge.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
            Info_txtLarge.Size = new Size(292, 87);
            Info_txtLarge.TabIndex = 69;
            Info_txtLarge.Text = "info_Rich";
            // 
            // info_Title
            // 
            info_Title.BackColor = Color.FromArgb(64, 64, 64);
            info_Title.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Pixel);
            info_Title.ForeColor = Color.White;
            info_Title.Location = new Point(24, 4);
            info_Title.Name = "info_Title";
            info_Title.Size = new Size(252, 16);
            info_Title.TabIndex = 69;
            info_Title.Text = "Title";
            info_Title.TextAlign = ContentAlignment.TopCenter;
            info_Title.MouseDown += Title_MouseDown;
            info_Title.MouseMove += Title_MouseMove;
            // 
            // pnlFX
            // 
            pnlFX.BackColor = Color.Indigo;
            pnlFX.Controls.Add(fx_Title);
            pnlFX.Controls.Add(fx_LblHead3);
            pnlFX.Controls.Add(fx_List3);
            pnlFX.Controls.Add(fx_lblHead2);
            pnlFX.Controls.Add(fx_lblHead1);
            pnlFX.Controls.Add(fx_List2);
            pnlFX.Controls.Add(fx_List1);
            pnlFX.Font = new Font("Segoe UI", 9.25F, FontStyle.Regular, GraphicsUnit.Pixel);
            pnlFX.Location = new Point(306, 3);
            pnlFX.Name = "pnlFX";
            pnlFX.Size = new Size(300, 357);
            pnlFX.TabIndex = 63;
            // 
            // fx_Title
            // 
            fx_Title.BackColor = Color.FromArgb(64, 64, 64);
            fx_Title.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            fx_Title.ForeColor = Color.White;
            fx_Title.Location = new Point(24, 4);
            fx_Title.Name = "fx_Title";
            fx_Title.Size = new Size(252, 16);
            fx_Title.TabIndex = 70;
            fx_Title.Text = "Title";
            fx_Title.TextAlign = ContentAlignment.TopCenter;
            // 
            // fx_LblHead3
            // 
            fx_LblHead3.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            fx_LblHead3.ForeColor = Color.White;
            fx_LblHead3.Location = new Point(4, 249);
            fx_LblHead3.Name = "fx_LblHead3";
            fx_LblHead3.Size = new Size(292, 16);
            fx_LblHead3.TabIndex = 28;
            fx_LblHead3.Text = "Misc Effects:";
            fx_LblHead3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // fx_List3
            // 
            fx_List3.BackColor = Color.FromArgb(64, 64, 64);
            fx_List3.Font = new Font("Segoe UI", 9.25F, FontStyle.Regular, GraphicsUnit.Pixel);
            fx_List3.HighlightColor = Color.FromArgb(128, 128, 255);
            fx_List3.HighlightTextColor = Color.Black;
            fx_List3.ItemColor = Color.White;
            fx_List3.Location = new Point(4, 265);
            fx_List3.Margin = new Padding(4);
            fx_List3.Name = "fx_List3";
            fx_List3.SetItemsBold = false;
            fx_List3.Size = new Size(292, 86);
            fx_List3.TabIndex = 27;
            fx_List3.UseHighlighting = true;
            fx_List3.ValueAlternateColor = Color.Chartreuse;
            fx_List3.ValueColor = Color.WhiteSmoke;
            fx_List3.ValueConditionColor = Color.Firebrick;
            fx_List3.ValueSpecialColor = Color.SlateBlue;
            fx_List3.ItemClick += Fx_ListItemClick;
            fx_List3.ItemHover += PairedList_Hover;
            fx_List3.ItemOut += PairedList_ItemOut;
            // 
            // fx_lblHead2
            // 
            fx_lblHead2.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            fx_lblHead2.ForeColor = Color.White;
            fx_lblHead2.Location = new Point(4, 146);
            fx_lblHead2.Name = "fx_lblHead2";
            fx_lblHead2.Size = new Size(292, 16);
            fx_lblHead2.TabIndex = 26;
            fx_lblHead2.Text = "Secondary Effects:";
            fx_lblHead2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // fx_lblHead1
            // 
            fx_lblHead1.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            fx_lblHead1.ForeColor = Color.White;
            fx_lblHead1.Location = new Point(4, 24);
            fx_lblHead1.Name = "fx_lblHead1";
            fx_lblHead1.Size = new Size(292, 16);
            fx_lblHead1.TabIndex = 25;
            fx_lblHead1.Text = "Primary Effects:";
            fx_lblHead1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // fx_List2
            // 
            fx_List2.BackColor = Color.FromArgb(64, 64, 64);
            fx_List2.Font = new Font("Segoe UI", 9.25F, FontStyle.Regular, GraphicsUnit.Pixel);
            fx_List2.HighlightColor = Color.FromArgb(128, 128, 255);
            fx_List2.HighlightTextColor = Color.Black;
            fx_List2.ItemColor = Color.White;
            fx_List2.Location = new Point(4, 162);
            fx_List2.Margin = new Padding(4);
            fx_List2.Name = "fx_List2";
            fx_List2.SetItemsBold = false;
            fx_List2.Size = new Size(292, 84);
            fx_List2.TabIndex = 24;
            fx_List2.UseHighlighting = true;
            fx_List2.ValueAlternateColor = Color.Chartreuse;
            fx_List2.ValueColor = Color.WhiteSmoke;
            fx_List2.ValueConditionColor = Color.Firebrick;
            fx_List2.ValueSpecialColor = Color.SlateBlue;
            fx_List2.ItemClick += Fx_ListItemClick;
            fx_List2.ItemHover += PairedList_Hover;
            fx_List2.ItemOut += PairedList_ItemOut;
            // 
            // fx_List1
            // 
            fx_List1.BackColor = Color.FromArgb(64, 64, 64);
            fx_List1.Font = new Font("Segoe UI", 9.25F, FontStyle.Regular, GraphicsUnit.Pixel);
            fx_List1.HighlightColor = Color.FromArgb(128, 128, 255);
            fx_List1.HighlightTextColor = Color.Black;
            fx_List1.ItemColor = Color.White;
            fx_List1.Location = new Point(4, 40);
            fx_List1.Margin = new Padding(4);
            fx_List1.Name = "fx_List1";
            fx_List1.SetItemsBold = false;
            fx_List1.Size = new Size(292, 110);
            fx_List1.TabIndex = 23;
            fx_List1.UseHighlighting = true;
            fx_List1.ValueAlternateColor = Color.Chartreuse;
            fx_List1.ValueColor = Color.WhiteSmoke;
            fx_List1.ValueConditionColor = Color.Firebrick;
            fx_List1.ValueSpecialColor = Color.SlateBlue;
            fx_List1.ItemClick += Fx_ListItemClick;
            fx_List1.ItemHover += PairedList_Hover;
            fx_List1.ItemOut += PairedList_ItemOut;
            // 
            // pnlTotal
            // 
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
            pnlTotal.Location = new Point(918, 3);
            pnlTotal.Name = "pnlTotal";
            pnlTotal.Size = new Size(300, 357);
            pnlTotal.TabIndex = 64;
            // 
            // lblTotal
            // 
            lblTotal.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            lblTotal.ForeColor = Color.White;
            lblTotal.Location = new Point(3, 338);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(292, 16);
            lblTotal.TabIndex = 75;
            lblTotal.Text = "Click the 'View Totals' button for more.";
            lblTotal.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // gRes2
            // 
            gRes2.BackColor = Color.Black;
            gRes2.BackgroundImage = (Image)resources.GetObject("gRes2.BackgroundImage");
            gRes2.BaseBarColors = (List<Color>)resources.GetObject("gRes2.BaseBarColors");
            gRes2.Border = true;
            gRes2.BorderColor = Color.Black;
            gRes2.Clickable = false;
            gRes2.ColorAbsorbed = Color.Gainsboro;
            gRes2.ColorBase = Color.FromArgb(0, 192, 192);
            gRes2.ColorEnh = Color.FromArgb(255, 128, 128);
            gRes2.ColorFadeEnd = Color.Teal;
            gRes2.ColorFadeStart = Color.Black;
            gRes2.ColorHighlight = Color.Gray;
            gRes2.ColorLines = Color.Black;
            gRes2.ColorMarkerInner = Color.Black;
            gRes2.ColorMarkerOuter = Color.Yellow;
            gRes2.ColorOvercap = Color.Black;
            gRes2.DifferentiateColors = false;
            gRes2.DrawRuler = false;
            gRes2.Dual = true;
            gRes2.EnhBarColors = (List<Color>)resources.GetObject("gRes2.EnhBarColors");
            gRes2.Font = new Font("Microsoft Sans Serif", 9.25F);
            gRes2.ForcedMax = 0F;
            gRes2.ForeColor = Color.FromArgb(192, 192, 255);
            gRes2.Highlight = true;
            gRes2.ItemFontSizeOverride = 11.25F;
            gRes2.ItemHeight = 13;
            gRes2.Lines = true;
            gRes2.Location = new Point(150, 166);
            gRes2.MarkerValue = 0F;
            gRes2.Max = 100F;
            gRes2.MaxItems = 4;
            gRes2.Name = "gRes2";
            gRes2.OuterBorder = false;
            gRes2.Overcap = false;
            gRes2.OvercapColors = (List<Color>)resources.GetObject("gRes2.OvercapColors");
            gRes2.PaddingX = 2F;
            gRes2.PaddingY = 4F;
            gRes2.PerItemScales = (List<float>)resources.GetObject("gRes2.PerItemScales");
            gRes2.RulerPos = CtlMultiGraph.RulerPosition.Top;
            gRes2.ScaleHeight = 32;
            gRes2.ScaleIndex = 8;
            gRes2.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            gRes2.ShowScale = false;
            gRes2.SingleLineLabels = true;
            gRes2.Size = new Size(146, 72);
            gRes2.Style = Enums.GraphStyle.Stacked;
            gRes2.TabIndex = 74;
            gRes2.TextWidth = 100;
            // 
            // gRes1
            // 
            gRes1.BackColor = Color.Black;
            gRes1.BackgroundImage = (Image)resources.GetObject("gRes1.BackgroundImage");
            gRes1.BaseBarColors = (List<Color>)resources.GetObject("gRes1.BaseBarColors");
            gRes1.Border = true;
            gRes1.BorderColor = Color.Black;
            gRes1.Clickable = false;
            gRes1.ColorAbsorbed = Color.Gainsboro;
            gRes1.ColorBase = Color.FromArgb(0, 192, 192);
            gRes1.ColorEnh = Color.FromArgb(255, 128, 128);
            gRes1.ColorFadeEnd = Color.Teal;
            gRes1.ColorFadeStart = Color.Black;
            gRes1.ColorHighlight = Color.Gray;
            gRes1.ColorLines = Color.Black;
            gRes1.ColorMarkerInner = Color.Black;
            gRes1.ColorMarkerOuter = Color.Yellow;
            gRes1.ColorOvercap = Color.Black;
            gRes1.DifferentiateColors = false;
            gRes1.DrawRuler = false;
            gRes1.Dual = true;
            gRes1.EnhBarColors = (List<Color>)resources.GetObject("gRes1.EnhBarColors");
            gRes1.Font = new Font("Segoe UI", 9.25F);
            gRes1.ForcedMax = 0F;
            gRes1.ForeColor = Color.FromArgb(192, 192, 255);
            gRes1.Highlight = true;
            gRes1.ItemFontSizeOverride = 11.25F;
            gRes1.ItemHeight = 13;
            gRes1.Lines = true;
            gRes1.Location = new Point(4, 166);
            gRes1.MarkerValue = 0F;
            gRes1.Max = 100F;
            gRes1.MaxItems = 4;
            gRes1.Name = "gRes1";
            gRes1.OuterBorder = false;
            gRes1.Overcap = false;
            gRes1.OvercapColors = (List<Color>)resources.GetObject("gRes1.OvercapColors");
            gRes1.PaddingX = 2F;
            gRes1.PaddingY = 4F;
            gRes1.PerItemScales = (List<float>)resources.GetObject("gRes1.PerItemScales");
            gRes1.RulerPos = CtlMultiGraph.RulerPosition.Top;
            gRes1.ScaleHeight = 32;
            gRes1.ScaleIndex = 8;
            gRes1.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            gRes1.ShowScale = false;
            gRes1.SingleLineLabels = true;
            gRes1.Size = new Size(146, 72);
            gRes1.Style = Enums.GraphStyle.Stacked;
            gRes1.TabIndex = 73;
            gRes1.TextWidth = 100;
            // 
            // gDef2
            // 
            gDef2.BackColor = Color.Black;
            gDef2.BackgroundImage = (Image)resources.GetObject("gDef2.BackgroundImage");
            gDef2.BaseBarColors = (List<Color>)resources.GetObject("gDef2.BaseBarColors");
            gDef2.Border = true;
            gDef2.BorderColor = Color.Black;
            gDef2.Clickable = false;
            gDef2.ColorAbsorbed = Color.Gainsboro;
            gDef2.ColorBase = Color.FromArgb(192, 0, 192);
            gDef2.ColorEnh = Color.Yellow;
            gDef2.ColorFadeEnd = Color.Purple;
            gDef2.ColorFadeStart = Color.Black;
            gDef2.ColorHighlight = Color.Gray;
            gDef2.ColorLines = Color.Black;
            gDef2.ColorMarkerInner = Color.Black;
            gDef2.ColorMarkerOuter = Color.Yellow;
            gDef2.ColorOvercap = Color.Black;
            gDef2.DifferentiateColors = false;
            gDef2.DrawRuler = false;
            gDef2.Dual = true;
            gDef2.EnhBarColors = (List<Color>)resources.GetObject("gDef2.EnhBarColors");
            gDef2.Font = new Font("Segoe UI", 9.25F);
            gDef2.ForcedMax = 0F;
            gDef2.ForeColor = Color.FromArgb(192, 192, 255);
            gDef2.Highlight = true;
            gDef2.ItemFontSizeOverride = 11.25F;
            gDef2.ItemHeight = 13;
            gDef2.Lines = true;
            gDef2.Location = new Point(150, 40);
            gDef2.MarkerValue = 0F;
            gDef2.Max = 100F;
            gDef2.MaxItems = 6;
            gDef2.Name = "gDef2";
            gDef2.OuterBorder = false;
            gDef2.Overcap = true;
            gDef2.OvercapColors = (List<Color>)resources.GetObject("gDef2.OvercapColors");
            gDef2.PaddingX = 2F;
            gDef2.PaddingY = 4F;
            gDef2.PerItemScales = (List<float>)resources.GetObject("gDef2.PerItemScales");
            gDef2.RulerPos = CtlMultiGraph.RulerPosition.Top;
            gDef2.ScaleHeight = 32;
            gDef2.ScaleIndex = 8;
            gDef2.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            gDef2.ShowScale = false;
            gDef2.SingleLineLabels = true;
            gDef2.Size = new Size(146, 107);
            gDef2.Style = Enums.GraphStyle.baseOnly;
            gDef2.TabIndex = 72;
            gDef2.TextWidth = 100;
            // 
            // gDef1
            // 
            gDef1.BackColor = Color.Black;
            gDef1.BackgroundImage = (Image)resources.GetObject("gDef1.BackgroundImage");
            gDef1.BaseBarColors = (List<Color>)resources.GetObject("gDef1.BaseBarColors");
            gDef1.Border = true;
            gDef1.BorderColor = Color.Black;
            gDef1.Clickable = false;
            gDef1.ColorAbsorbed = Color.Gainsboro;
            gDef1.ColorBase = Color.FromArgb(192, 0, 192);
            gDef1.ColorEnh = Color.Yellow;
            gDef1.ColorFadeEnd = Color.Purple;
            gDef1.ColorFadeStart = Color.Black;
            gDef1.ColorHighlight = Color.Gray;
            gDef1.ColorLines = Color.Black;
            gDef1.ColorMarkerInner = Color.Black;
            gDef1.ColorMarkerOuter = Color.Yellow;
            gDef1.ColorOvercap = Color.Black;
            gDef1.DifferentiateColors = false;
            gDef1.DrawRuler = false;
            gDef1.Dual = true;
            gDef1.EnhBarColors = (List<Color>)resources.GetObject("gDef1.EnhBarColors");
            gDef1.Font = new Font("Segoe UI", 9.25F);
            gDef1.ForcedMax = 0F;
            gDef1.ForeColor = Color.FromArgb(192, 192, 255);
            gDef1.Highlight = true;
            gDef1.ItemFontSizeOverride = 11.25F;
            gDef1.ItemHeight = 13;
            gDef1.Lines = true;
            gDef1.Location = new Point(4, 40);
            gDef1.MarkerValue = 0F;
            gDef1.Max = 100F;
            gDef1.MaxItems = 6;
            gDef1.Name = "gDef1";
            gDef1.OuterBorder = false;
            gDef1.Overcap = true;
            gDef1.OvercapColors = (List<Color>)resources.GetObject("gDef1.OvercapColors");
            gDef1.PaddingX = 2F;
            gDef1.PaddingY = 4F;
            gDef1.PerItemScales = (List<float>)resources.GetObject("gDef1.PerItemScales");
            gDef1.RulerPos = CtlMultiGraph.RulerPosition.Top;
            gDef1.ScaleHeight = 32;
            gDef1.ScaleIndex = 8;
            gDef1.SecondaryLabelPosition = CtlMultiGraph.Alignment.Right;
            gDef1.ShowScale = false;
            gDef1.SingleLineLabels = true;
            gDef1.Size = new Size(146, 107);
            gDef1.Style = Enums.GraphStyle.baseOnly;
            gDef1.TabIndex = 71;
            gDef1.TextWidth = 100;
            // 
            // total_Title
            // 
            total_Title.BackColor = Color.FromArgb(64, 64, 64);
            total_Title.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            total_Title.ForeColor = Color.White;
            total_Title.Location = new Point(24, 4);
            total_Title.Name = "total_Title";
            total_Title.Size = new Size(252, 16);
            total_Title.TabIndex = 70;
            total_Title.Text = "Cumulative Totals (For Self)";
            total_Title.TextAlign = ContentAlignment.TopCenter;
            total_Title.MouseDown += Title_MouseDown;
            total_Title.MouseMove += Title_MouseMove;
            // 
            // total_lblMisc
            // 
            total_lblMisc.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            total_lblMisc.ForeColor = Color.White;
            total_lblMisc.Location = new Point(4, 242);
            total_lblMisc.Name = "total_lblMisc";
            total_lblMisc.Size = new Size(292, 16);
            total_lblMisc.TabIndex = 28;
            total_lblMisc.Text = "Misc Effects:";
            total_lblMisc.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // total_Misc
            // 
            total_Misc.BackColor = Color.FromArgb(64, 64, 64);
            total_Misc.Font = new Font("Segoe UI", 9.25F);
            total_Misc.HighlightColor = Color.FromArgb(128, 128, 255);
            total_Misc.HighlightTextColor = Color.Black;
            total_Misc.ItemColor = Color.White;
            total_Misc.Location = new Point(4, 258);
            total_Misc.Margin = new Padding(4);
            total_Misc.Name = "total_Misc";
            total_Misc.SetItemsBold = false;
            total_Misc.Size = new Size(292, 77);
            total_Misc.TabIndex = 27;
            total_Misc.UseHighlighting = true;
            total_Misc.ValueAlternateColor = Color.Chartreuse;
            total_Misc.ValueColor = Color.WhiteSmoke;
            total_Misc.ValueConditionColor = Color.Firebrick;
            total_Misc.ValueSpecialColor = Color.SlateBlue;
            total_Misc.ItemHover += PairedList_Hover;
            total_Misc.ItemOut += PairedList_ItemOut;
            // 
            // total_lblRes
            // 
            total_lblRes.BackColor = Color.Green;
            total_lblRes.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            total_lblRes.ForeColor = Color.White;
            total_lblRes.Location = new Point(4, 150);
            total_lblRes.Name = "total_lblRes";
            total_lblRes.Size = new Size(292, 16);
            total_lblRes.TabIndex = 26;
            total_lblRes.Text = "Resistance:";
            total_lblRes.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // total_lblDef
            // 
            total_lblDef.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            total_lblDef.ForeColor = Color.White;
            total_lblDef.Location = new Point(4, 24);
            total_lblDef.Name = "total_lblDef";
            total_lblDef.Size = new Size(292, 16);
            total_lblDef.TabIndex = 25;
            total_lblDef.Text = "Defense:";
            total_lblDef.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlEnh
            // 
            pnlEnh.BackColor = Color.Teal;
            pnlEnh.Controls.Add(pnlEnhInactive);
            pnlEnh.Controls.Add(pnlEnhActive);
            pnlEnh.Controls.Add(enhNameDisp);
            pnlEnh.Controls.Add(enhListing);
            pnlEnh.Controls.Add(Enh_Title);
            pnlEnh.Location = new Point(612, 3);
            pnlEnh.Name = "pnlEnh";
            pnlEnh.Size = new Size(300, 357);
            pnlEnh.TabIndex = 65;
            // 
            // pnlEnhInactive
            // 
            pnlEnhInactive.BackColor = Color.Black;
            pnlEnhInactive.Location = new Point(4, 279);
            pnlEnhInactive.Name = "pnlEnhInactive";
            pnlEnhInactive.Size = new Size(292, 38);
            pnlEnhInactive.TabIndex = 74;
            pnlEnhInactive.Paint += pnlEnhInactive_Paint;
            pnlEnhInactive.MouseClick += pnlEnhInactive_MouseClick;
            pnlEnhInactive.MouseMove += pnlEnhInactive_MouseMove;
            // 
            // pnlEnhActive
            // 
            pnlEnhActive.BackColor = Color.Black;
            pnlEnhActive.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            pnlEnhActive.Location = new Point(4, 239);
            pnlEnhActive.Name = "pnlEnhActive";
            pnlEnhActive.Size = new Size(292, 38);
            pnlEnhActive.TabIndex = 73;
            pnlEnhActive.Paint += pnlEnhActive_Paint;
            pnlEnhActive.MouseClick += pnlEnhActive_MouseClick;
            pnlEnhActive.MouseMove += pnlEnhActive_MouseMove;
            // 
            // enhNameDisp
            // 
            enhNameDisp.BackColor = Color.FromArgb(64, 64, 64);
            enhNameDisp.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            enhNameDisp.ForeColor = Color.White;
            enhNameDisp.Location = new Point(4, 24);
            enhNameDisp.Name = "enhNameDisp";
            enhNameDisp.Size = new Size(292, 16);
            enhNameDisp.TabIndex = 72;
            enhNameDisp.Text = "Title";
            enhNameDisp.TextAlign = ContentAlignment.TopCenter;
            // 
            // enhListing
            // 
            enhListing.AutoSizeLineHeight = false;
            enhListing.BackColor = Color.FromArgb(0, 0, 32);
            enhListing.Columns = 1;
            enhListing.Font = new Font("Segoe UI", 9.25F);
            enhListing.HighlightColor = Color.FromArgb(128, 128, 255);
            enhListing.HighlightTextColor = Color.Black;
            enhListing.ItemColor = Color.White;
            enhListing.Location = new Point(4, 44);
            enhListing.Margin = new Padding(4);
            enhListing.Name = "enhListing";
            enhListing.SetItemsBold = false;
            enhListing.Size = new Size(292, 192);
            enhListing.TabIndex = 71;
            enhListing.UseHighlighting = true;
            enhListing.ValueAlternateColor = Color.Chartreuse;
            enhListing.ValueColor = Color.WhiteSmoke;
            enhListing.ValueConditionColor = Color.Firebrick;
            enhListing.ValueSpecialColor = Color.SlateBlue;
            enhListing.ItemHover += PairedList_Hover;
            enhListing.ItemOut += PairedList_ItemOut;
            // 
            // Enh_Title
            // 
            Enh_Title.BackColor = Color.FromArgb(64, 64, 64);
            Enh_Title.Font = new Font("Segoe UI", 9.25F, FontStyle.Bold);
            Enh_Title.ForeColor = Color.White;
            Enh_Title.Location = new Point(24, 4);
            Enh_Title.Name = "Enh_Title";
            Enh_Title.Size = new Size(252, 16);
            Enh_Title.TabIndex = 70;
            Enh_Title.Text = "Title";
            Enh_Title.TextAlign = ContentAlignment.TopCenter;
            Enh_Title.MouseDown += Title_MouseDown;
            Enh_Title.MouseMove += Title_MouseMove;
            // 
            // dbTip
            // 
            dbTip.AutoPopDelay = 15000;
            dbTip.InitialDelay = 350;
            dbTip.ReshowDelay = 100;
            // 
            // lblFloat
            // 
            lblFloat.BackColor = Color.FromArgb(64, 64, 64);
            lblFloat.BorderStyle = BorderStyle.FixedSingle;
            lblFloat.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Pixel);
            lblFloat.ForeColor = Color.White;
            lblFloat.Location = new Point(4, 24);
            lblFloat.Name = "lblFloat";
            lblFloat.Size = new Size(16, 16);
            lblFloat.TabIndex = 66;
            lblFloat.Text = "F";
            lblFloat.TextAlign = ContentAlignment.MiddleCenter;
            dbTip.SetToolTip(lblFloat, "Make Floating Window");
            lblFloat.UseCompatibleTextRendering = true;
            lblFloat.Click += lblFloat_Click;
            // 
            // lblShrink
            // 
            lblShrink.BackColor = Color.FromArgb(64, 64, 64);
            lblShrink.BorderStyle = BorderStyle.FixedSingle;
            lblShrink.Font = new Font("Wingdings", 9.25F, FontStyle.Bold);
            lblShrink.ForeColor = Color.White;
            lblShrink.Location = new Point(280, 24);
            lblShrink.Name = "lblShrink";
            lblShrink.Size = new Size(16, 16);
            lblShrink.TabIndex = 67;
            lblShrink.Text = "y";
            lblShrink.TextAlign = ContentAlignment.MiddleCenter;
            dbTip.SetToolTip(lblShrink, "Shrink / Expand the Info Display");
            lblShrink.UseCompatibleTextRendering = true;
            lblShrink.Click += lblShrink_Click;
            lblShrink.DoubleClick += lblShrink_DoubleClick;
            // 
            // lblLock
            // 
            lblLock.BackColor = Color.Red;
            lblLock.BorderStyle = BorderStyle.FixedSingle;
            lblLock.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold);
            lblLock.ForeColor = Color.White;
            lblLock.Location = new Point(220, 24);
            lblLock.Name = "lblLock";
            lblLock.Size = new Size(56, 16);
            lblLock.TabIndex = 68;
            lblLock.Text = "[Unlock]";
            lblLock.TextAlign = ContentAlignment.MiddleCenter;
            dbTip.SetToolTip(lblLock, "The info display is currently locked to display a specific power, click here to unlock it to display powers as you hover the mouse over them.");
            lblLock.Click += lblLock_Click;
            // 
            // DataView
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(0, 0, 32);
            Controls.Add(lblLock);
            Controls.Add(lblFloat);
            Controls.Add(lblShrink);
            Controls.Add(pnlTabs);
            Controls.Add(pnlInfo);
            Controls.Add(pnlTotal);
            Controls.Add(pnlEnh);
            Controls.Add(pnlFX);
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 9.25F, FontStyle.Regular, GraphicsUnit.Pixel);
            Name = "DataView";
            Size = new Size(1221, 479);
            pnlInfo.ResumeLayout(false);
            pnlFX.ResumeLayout(false);
            pnlTotal.ResumeLayout(false);
            pnlEnh.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        public ToolTip dbTip;
        private Label Enh_Title;
        private PairedListEx enhListing;
        private Label enhNameDisp;
        private Label fx_lblHead1;
        private Label fx_lblHead2;
        private Label fx_LblHead3;
        private PairedListEx fx_List1;
        private PairedListEx fx_List2;
        private PairedListEx fx_List3;
        private Label fx_Title;
        private CtlMultiGraph gDef1;
        private CtlMultiGraph gDef2;
        private CtlMultiGraph gRes1;
        private CtlMultiGraph gRes2;
        private PairedListEx info_DataList;
        private Label info_Title;
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
        private CtlMultiGraph PowerScaler;
        private Label total_lblDef;
        private Label total_lblMisc;
        private Label total_lblRes;
        private PairedListEx total_Misc;
        private Label total_Title;

        internal ctlDamageDisplay Info_Damage;
        internal RichTextBox Info_txtLarge;
    }
}
