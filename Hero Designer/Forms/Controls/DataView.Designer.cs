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
            this.components = new System.ComponentModel.Container();
            this.pnlTabs = new System.Windows.Forms.Panel();
            this.pnlInfo = new System.Windows.Forms.Panel();
            this.PowerScaler = new midsControls.ctlMultiGraph();
            this.info_txtSmall = new System.Windows.Forms.RichTextBox();
            this.lblDmg = new System.Windows.Forms.Label();
            this.Info_Damage = new midsControls.ctlDamageDisplay();
            this.info_DataList = new midsControls.ctlPairedList();
            this.Info_txtLarge = new System.Windows.Forms.RichTextBox();
            this.info_Title = new midsControls.GFXLabel();
            this.pnlFX = new System.Windows.Forms.Panel();
            this.fx_Title = new midsControls.GFXLabel();
            this.fx_LblHead3 = new System.Windows.Forms.Label();
            this.fx_List3 = new midsControls.ctlPairedList();
            this.fx_lblHead2 = new System.Windows.Forms.Label();
            this.fx_lblHead1 = new System.Windows.Forms.Label();
            this.fx_List2 = new midsControls.ctlPairedList();
            this.fx_List1 = new midsControls.ctlPairedList();
            this.pnlTotal = new System.Windows.Forms.Panel();
            this.lblTotal = new System.Windows.Forms.Label();
            this.gRes2 = new midsControls.ctlMultiGraph();
            this.gRes1 = new midsControls.ctlMultiGraph();
            this.gDef2 = new midsControls.ctlMultiGraph();
            this.gDef1 = new midsControls.ctlMultiGraph();
            this.total_Title = new midsControls.GFXLabel();
            this.total_lblMisc = new System.Windows.Forms.Label();
            this.total_Misc = new midsControls.ctlPairedList();
            this.total_lblRes = new System.Windows.Forms.Label();
            this.total_lblDef = new System.Windows.Forms.Label();
            this.pnlEnh = new System.Windows.Forms.Panel();
            this.pnlEnhInactive = new System.Windows.Forms.Panel();
            this.pnlEnhActive = new System.Windows.Forms.Panel();
            this.enhNameDisp = new midsControls.GFXLabel();
            this.enhListing = new midsControls.ctlPairedList();
            this.Enh_Title = new midsControls.GFXLabel();
            this.dbTip = new System.Windows.Forms.ToolTip(this.components);
            this.lblFloat = new System.Windows.Forms.Label();
            this.lblShrink = new System.Windows.Forms.Label();
            this.lblLock = new System.Windows.Forms.Label();
            this.pnlInfo.SuspendLayout();
            this.pnlFX.SuspendLayout();
            this.pnlTotal.SuspendLayout();
            this.pnlEnh.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTabs
            // 
            this.pnlTabs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pnlTabs.Location = new System.Drawing.Point(0, 0);
            this.pnlTabs.Name = "pnlTabs";
            this.pnlTabs.Size = new System.Drawing.Size(300, 20);
            this.pnlTabs.TabIndex = 61;
            // 
            // pnlInfo
            // 
            this.pnlInfo.BackColor = System.Drawing.Color.Navy;
            this.pnlInfo.Controls.Add(this.PowerScaler);
            this.pnlInfo.Controls.Add(this.info_txtSmall);
            this.pnlInfo.Controls.Add(this.lblDmg);
            this.pnlInfo.Controls.Add(this.Info_Damage);
            this.pnlInfo.Controls.Add(this.info_DataList);
            this.pnlInfo.Controls.Add(this.Info_txtLarge);
            this.pnlInfo.Controls.Add(this.info_Title);
            this.pnlInfo.Location = new System.Drawing.Point(0, 20);
            this.pnlInfo.Name = "pnlInfo";
            this.pnlInfo.Size = new System.Drawing.Size(300, 403);
            this.pnlInfo.TabIndex = 62;
            // 
            // PowerScaler
            // 
            this.PowerScaler.BackColor = System.Drawing.Color.Black;
            this.PowerScaler.Border = true;
            this.PowerScaler.Clickable = true;
            this.PowerScaler.ColorBase = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(255)))), ((int)(((byte)(64)))));
            this.PowerScaler.ColorEnh = System.Drawing.Color.Yellow;
            this.PowerScaler.ColorFadeEnd = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.PowerScaler.ColorFadeStart = System.Drawing.Color.Black;
            this.PowerScaler.ColorHighlight = System.Drawing.Color.Gray;
            this.PowerScaler.ColorLines = System.Drawing.Color.Black;
            this.PowerScaler.ColorMarkerInner = System.Drawing.Color.Red;
            this.PowerScaler.ColorMarkerOuter = System.Drawing.Color.Black;
            this.PowerScaler.Dual = false;
            this.PowerScaler.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.PowerScaler.ForcedMax = 0F;
            this.PowerScaler.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.PowerScaler.Highlight = true;
            this.PowerScaler.ItemHeight = 10;
            this.PowerScaler.Lines = true;
            this.PowerScaler.Location = new System.Drawing.Point(4, 145);
            this.PowerScaler.MarkerValue = 0F;
            this.PowerScaler.Max = 100F;
            this.PowerScaler.Name = "PowerScaler";
            this.PowerScaler.PaddingX = 2F;
            this.PowerScaler.PaddingY = 2F;
            this.PowerScaler.ScaleHeight = 32;
            this.PowerScaler.ScaleIndex = 8;
            this.PowerScaler.ShowScale = false;
            this.PowerScaler.Size = new System.Drawing.Size(292, 15);
            this.PowerScaler.Style = Enums.GraphStyle.baseOnly;
            this.PowerScaler.TabIndex = 71;
            this.PowerScaler.TextWidth = 80;
            // 
            // info_txtSmall
            // 
            this.info_txtSmall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.info_txtSmall.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.info_txtSmall.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.info_txtSmall.ForeColor = System.Drawing.Color.White;
            this.info_txtSmall.Location = new System.Drawing.Point(4, 24);
            this.info_txtSmall.Name = "info_txtSmall";
            this.info_txtSmall.ReadOnly = true;
            this.info_txtSmall.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.info_txtSmall.Size = new System.Drawing.Size(292, 32);
            this.info_txtSmall.TabIndex = 70;
            this.info_txtSmall.Text = "info_Rich";
            // 
            // lblDmg
            // 
            this.lblDmg.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDmg.ForeColor = System.Drawing.Color.White;
            this.lblDmg.Location = new System.Drawing.Point(4, 272);
            this.lblDmg.Name = "lblDmg";
            this.lblDmg.Size = new System.Drawing.Size(292, 15);
            this.lblDmg.TabIndex = 15;
            this.lblDmg.Text = "Damage:";
            this.lblDmg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Info_Damage
            // 
            this.Info_Damage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Info_Damage.ColorBackEnd = System.Drawing.Color.Red;
            this.Info_Damage.ColorBackStart = System.Drawing.Color.Black;
            this.Info_Damage.ColorBaseEnd = System.Drawing.Color.Blue;
            this.Info_Damage.ColorBaseStart = System.Drawing.Color.Blue;
            this.Info_Damage.ColorEnhEnd = System.Drawing.Color.Yellow;
            this.Info_Damage.ColorEnhStart = System.Drawing.Color.Yellow;
            this.Info_Damage.Font = new System.Drawing.Font("Arial", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.Info_Damage.GraphType = Enums.eDDGraph.Enhanced;
            this.Info_Damage.Location = new System.Drawing.Point(2, 284);
            this.Info_Damage.Name = "Info_Damage";
            this.Info_Damage.nBaseVal = 100F;
            this.Info_Damage.nEnhVal = 150F;
            this.Info_Damage.nHighBase = 200F;
            this.Info_Damage.nHighEnh = 214F;
            this.Info_Damage.nMaxEnhVal = 207F;
            this.Info_Damage.PaddingH = 1;
            this.Info_Damage.PaddingV = 1;
            this.Info_Damage.Size = new System.Drawing.Size(295, 56);
            this.Info_Damage.Style = Enums.eDDStyle.Text;
            this.Info_Damage.TabIndex = 20;
            this.Info_Damage.TextAlign = Enums.eDDAlign.Left;
            this.Info_Damage.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            // 
            // info_DataList
            // 
            this.info_DataList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(32)))));
            this.info_DataList.Columns = 2;
            this.info_DataList.DoHighlight = true;
            this.info_DataList.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.info_DataList.ForceBold = false;
            this.info_DataList.HighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.info_DataList.HighlightTextColor = System.Drawing.Color.Black;
            this.info_DataList.ItemColor = System.Drawing.Color.White;
            this.info_DataList.ItemColorAlt = System.Drawing.Color.Lime;
            this.info_DataList.ItemColorSpecCase = System.Drawing.Color.Red;
            this.info_DataList.ItemColorSpecial = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.info_DataList.Location = new System.Drawing.Point(4, 164);
            this.info_DataList.Name = "info_DataList";
            this.info_DataList.NameColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.info_DataList.Size = new System.Drawing.Size(292, 104);
            this.info_DataList.TabIndex = 19;
            this.info_DataList.ValueWidth = 55;
            // 
            // Info_txtLarge
            // 
            this.Info_txtLarge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Info_txtLarge.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Info_txtLarge.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.Info_txtLarge.ForeColor = System.Drawing.Color.White;
            this.Info_txtLarge.Location = new System.Drawing.Point(4, 60);
            this.Info_txtLarge.Name = "Info_txtLarge";
            this.Info_txtLarge.ReadOnly = true;
            this.Info_txtLarge.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.Info_txtLarge.Size = new System.Drawing.Size(292, 87);
            this.Info_txtLarge.TabIndex = 69;
            this.Info_txtLarge.Text = "info_Rich";
            // 
            // info_Title
            // 
            this.info_Title.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.info_Title.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.info_Title.ForeColor = System.Drawing.Color.White;
            this.info_Title.InitialText = "Title";
            this.info_Title.Location = new System.Drawing.Point(24, 4);
            this.info_Title.Name = "info_Title";
            this.info_Title.Size = new System.Drawing.Size(252, 16);
            this.info_Title.TabIndex = 69;
            this.info_Title.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pnlFX
            // 
            this.pnlFX.BackColor = System.Drawing.Color.Indigo;
            this.pnlFX.Controls.Add(this.fx_Title);
            this.pnlFX.Controls.Add(this.fx_LblHead3);
            this.pnlFX.Controls.Add(this.fx_List3);
            this.pnlFX.Controls.Add(this.fx_lblHead2);
            this.pnlFX.Controls.Add(this.fx_lblHead1);
            this.pnlFX.Controls.Add(this.fx_List2);
            this.pnlFX.Controls.Add(this.fx_List1);
            this.pnlFX.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.pnlFX.Location = new System.Drawing.Point(306, 3);
            this.pnlFX.Name = "pnlFX";
            this.pnlFX.Size = new System.Drawing.Size(300, 420);
            this.pnlFX.TabIndex = 63;
            // 
            // fx_Title
            // 
            this.fx_Title.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.fx_Title.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fx_Title.ForeColor = System.Drawing.Color.White;
            this.fx_Title.InitialText = "Title";
            this.fx_Title.Location = new System.Drawing.Point(24, 4);
            this.fx_Title.Name = "fx_Title";
            this.fx_Title.Size = new System.Drawing.Size(252, 16);
            this.fx_Title.TabIndex = 70;
            this.fx_Title.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // fx_LblHead3
            // 
            this.fx_LblHead3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fx_LblHead3.ForeColor = System.Drawing.Color.White;
            this.fx_LblHead3.Location = new System.Drawing.Point(4, 228);
            this.fx_LblHead3.Name = "fx_LblHead3";
            this.fx_LblHead3.Size = new System.Drawing.Size(292, 16);
            this.fx_LblHead3.TabIndex = 28;
            this.fx_LblHead3.Text = "Misc Effects:";
            this.fx_LblHead3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fx_List3
            // 
            this.fx_List3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.fx_List3.Columns = 2;
            this.fx_List3.DoHighlight = true;
            this.fx_List3.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.fx_List3.ForceBold = false;
            this.fx_List3.HighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.fx_List3.HighlightTextColor = System.Drawing.Color.Black;
            this.fx_List3.ItemColor = System.Drawing.Color.White;
            this.fx_List3.ItemColorAlt = System.Drawing.Color.Lime;
            this.fx_List3.ItemColorSpecCase = System.Drawing.Color.Red;
            this.fx_List3.ItemColorSpecial = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.fx_List3.Location = new System.Drawing.Point(4, 244);
            this.fx_List3.Name = "fx_List3";
            this.fx_List3.NameColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.fx_List3.Size = new System.Drawing.Size(292, 72);
            this.fx_List3.TabIndex = 27;
            this.fx_List3.ValueWidth = 55;
            // 
            // fx_lblHead2
            // 
            this.fx_lblHead2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fx_lblHead2.ForeColor = System.Drawing.Color.White;
            this.fx_lblHead2.Location = new System.Drawing.Point(4, 136);
            this.fx_lblHead2.Name = "fx_lblHead2";
            this.fx_lblHead2.Size = new System.Drawing.Size(292, 16);
            this.fx_lblHead2.TabIndex = 26;
            this.fx_lblHead2.Text = "Secondary Effects:";
            this.fx_lblHead2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fx_lblHead1
            // 
            this.fx_lblHead1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fx_lblHead1.ForeColor = System.Drawing.Color.White;
            this.fx_lblHead1.Location = new System.Drawing.Point(4, 24);
            this.fx_lblHead1.Name = "fx_lblHead1";
            this.fx_lblHead1.Size = new System.Drawing.Size(292, 16);
            this.fx_lblHead1.TabIndex = 25;
            this.fx_lblHead1.Text = "Primary Effects:";
            this.fx_lblHead1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fx_List2
            // 
            this.fx_List2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.fx_List2.Columns = 2;
            this.fx_List2.DoHighlight = true;
            this.fx_List2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.fx_List2.ForceBold = false;
            this.fx_List2.HighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.fx_List2.HighlightTextColor = System.Drawing.Color.Black;
            this.fx_List2.ItemColor = System.Drawing.Color.White;
            this.fx_List2.ItemColorAlt = System.Drawing.Color.Lime;
            this.fx_List2.ItemColorSpecCase = System.Drawing.Color.Red;
            this.fx_List2.ItemColorSpecial = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.fx_List2.Location = new System.Drawing.Point(4, 152);
            this.fx_List2.Name = "fx_List2";
            this.fx_List2.NameColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.fx_List2.Size = new System.Drawing.Size(292, 72);
            this.fx_List2.TabIndex = 24;
            this.fx_List2.ValueWidth = 55;
            // 
            // fx_List1
            // 
            this.fx_List1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.fx_List1.Columns = 2;
            this.fx_List1.DoHighlight = true;
            this.fx_List1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.fx_List1.ForceBold = false;
            this.fx_List1.HighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.fx_List1.HighlightTextColor = System.Drawing.Color.Black;
            this.fx_List1.ItemColor = System.Drawing.Color.White;
            this.fx_List1.ItemColorAlt = System.Drawing.Color.Lime;
            this.fx_List1.ItemColorSpecCase = System.Drawing.Color.Red;
            this.fx_List1.ItemColorSpecial = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.fx_List1.Location = new System.Drawing.Point(4, 40);
            this.fx_List1.Name = "fx_List1";
            this.fx_List1.NameColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.fx_List1.Size = new System.Drawing.Size(292, 92);
            this.fx_List1.TabIndex = 23;
            this.fx_List1.ValueWidth = 60;
            // 
            // pnlTotal
            // 
            this.pnlTotal.BackColor = System.Drawing.Color.Green;
            this.pnlTotal.Controls.Add(this.lblTotal);
            this.pnlTotal.Controls.Add(this.gRes2);
            this.pnlTotal.Controls.Add(this.gRes1);
            this.pnlTotal.Controls.Add(this.gDef2);
            this.pnlTotal.Controls.Add(this.gDef1);
            this.pnlTotal.Controls.Add(this.total_Title);
            this.pnlTotal.Controls.Add(this.total_lblMisc);
            this.pnlTotal.Controls.Add(this.total_Misc);
            this.pnlTotal.Controls.Add(this.total_lblRes);
            this.pnlTotal.Controls.Add(this.total_lblDef);
            this.pnlTotal.Location = new System.Drawing.Point(918, 3);
            this.pnlTotal.Name = "pnlTotal";
            this.pnlTotal.Size = new System.Drawing.Size(300, 420);
            this.pnlTotal.TabIndex = 64;
            // 
            // lblTotal
            // 
            this.lblTotal.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.White;
            this.lblTotal.Location = new System.Drawing.Point(4, 300);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(292, 16);
            this.lblTotal.TabIndex = 75;
            this.lblTotal.Text = "Click the \'View Totals\' button for more.";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gRes2
            // 
            this.gRes2.BackColor = System.Drawing.Color.Black;
            this.gRes2.Border = true;
            this.gRes2.Clickable = false;
            this.gRes2.ColorBase = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.gRes2.ColorEnh = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.gRes2.ColorFadeEnd = System.Drawing.Color.Teal;
            this.gRes2.ColorFadeStart = System.Drawing.Color.Black;
            this.gRes2.ColorHighlight = System.Drawing.Color.Gray;
            this.gRes2.ColorLines = System.Drawing.Color.Black;
            this.gRes2.ColorMarkerInner = System.Drawing.Color.Black;
            this.gRes2.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.gRes2.Dual = false;
            this.gRes2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.gRes2.ForcedMax = 0F;
            this.gRes2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.gRes2.Highlight = true;
            this.gRes2.ItemHeight = 13;
            this.gRes2.Lines = true;
            this.gRes2.Location = new System.Drawing.Point(150, 152);
            this.gRes2.MarkerValue = 0F;
            this.gRes2.Max = 100F;
            this.gRes2.Name = "gRes2";
            this.gRes2.PaddingX = 2F;
            this.gRes2.PaddingY = 4F;
            this.gRes2.ScaleHeight = 32;
            this.gRes2.ScaleIndex = 8;
            this.gRes2.ShowScale = false;
            this.gRes2.Size = new System.Drawing.Size(146, 72);
            this.gRes2.Style = Enums.GraphStyle.Stacked;
            this.gRes2.TabIndex = 74;
            this.gRes2.TextWidth = 100;
            // 
            // gRes1
            // 
            this.gRes1.BackColor = System.Drawing.Color.Black;
            this.gRes1.Border = true;
            this.gRes1.Clickable = false;
            this.gRes1.ColorBase = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.gRes1.ColorEnh = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.gRes1.ColorFadeEnd = System.Drawing.Color.Teal;
            this.gRes1.ColorFadeStart = System.Drawing.Color.Black;
            this.gRes1.ColorHighlight = System.Drawing.Color.Gray;
            this.gRes1.ColorLines = System.Drawing.Color.Black;
            this.gRes1.ColorMarkerInner = System.Drawing.Color.Black;
            this.gRes1.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.gRes1.Dual = false;
            this.gRes1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.gRes1.ForcedMax = 0F;
            this.gRes1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.gRes1.Highlight = true;
            this.gRes1.ItemHeight = 13;
            this.gRes1.Lines = true;
            this.gRes1.Location = new System.Drawing.Point(4, 152);
            this.gRes1.MarkerValue = 0F;
            this.gRes1.Max = 100F;
            this.gRes1.Name = "gRes1";
            this.gRes1.PaddingX = 2F;
            this.gRes1.PaddingY = 4F;
            this.gRes1.ScaleHeight = 32;
            this.gRes1.ScaleIndex = 8;
            this.gRes1.ShowScale = false;
            this.gRes1.Size = new System.Drawing.Size(146, 72);
            this.gRes1.Style = Enums.GraphStyle.Stacked;
            this.gRes1.TabIndex = 73;
            this.gRes1.TextWidth = 100;
            // 
            // gDef2
            // 
            this.gDef2.BackColor = System.Drawing.Color.Black;
            this.gDef2.Border = true;
            this.gDef2.Clickable = false;
            this.gDef2.ColorBase = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.gDef2.ColorEnh = System.Drawing.Color.Yellow;
            this.gDef2.ColorFadeEnd = System.Drawing.Color.Purple;
            this.gDef2.ColorFadeStart = System.Drawing.Color.Black;
            this.gDef2.ColorHighlight = System.Drawing.Color.Gray;
            this.gDef2.ColorLines = System.Drawing.Color.Black;
            this.gDef2.ColorMarkerInner = System.Drawing.Color.Black;
            this.gDef2.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.gDef2.Dual = false;
            this.gDef2.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.gDef2.ForcedMax = 0F;
            this.gDef2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.gDef2.Highlight = true;
            this.gDef2.ItemHeight = 13;
            this.gDef2.Lines = true;
            this.gDef2.Location = new System.Drawing.Point(150, 40);
            this.gDef2.MarkerValue = 0F;
            this.gDef2.Max = 100F;
            this.gDef2.Name = "gDef2";
            this.gDef2.PaddingX = 2F;
            this.gDef2.PaddingY = 4F;
            this.gDef2.ScaleHeight = 32;
            this.gDef2.ScaleIndex = 8;
            this.gDef2.ShowScale = false;
            this.gDef2.Size = new System.Drawing.Size(146, 92);
            this.gDef2.Style = Enums.GraphStyle.baseOnly;
            this.gDef2.TabIndex = 72;
            this.gDef2.TextWidth = 100;
            // 
            // gDef1
            // 
            this.gDef1.BackColor = System.Drawing.Color.Black;
            this.gDef1.Border = true;
            this.gDef1.Clickable = false;
            this.gDef1.ColorBase = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.gDef1.ColorEnh = System.Drawing.Color.Yellow;
            this.gDef1.ColorFadeEnd = System.Drawing.Color.Purple;
            this.gDef1.ColorFadeStart = System.Drawing.Color.Black;
            this.gDef1.ColorHighlight = System.Drawing.Color.Gray;
            this.gDef1.ColorLines = System.Drawing.Color.Black;
            this.gDef1.ColorMarkerInner = System.Drawing.Color.Black;
            this.gDef1.ColorMarkerOuter = System.Drawing.Color.Yellow;
            this.gDef1.Dual = false;
            this.gDef1.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.gDef1.ForcedMax = 0F;
            this.gDef1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.gDef1.Highlight = true;
            this.gDef1.ItemHeight = 13;
            this.gDef1.Lines = true;
            this.gDef1.Location = new System.Drawing.Point(4, 40);
            this.gDef1.MarkerValue = 0F;
            this.gDef1.Max = 100F;
            this.gDef1.Name = "gDef1";
            this.gDef1.PaddingX = 2F;
            this.gDef1.PaddingY = 4F;
            this.gDef1.ScaleHeight = 32;
            this.gDef1.ScaleIndex = 8;
            this.gDef1.ShowScale = false;
            this.gDef1.Size = new System.Drawing.Size(146, 92);
            this.gDef1.Style = Enums.GraphStyle.baseOnly;
            this.gDef1.TabIndex = 71;
            this.gDef1.TextWidth = 100;
            // 
            // total_Title
            // 
            this.total_Title.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.total_Title.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.total_Title.ForeColor = System.Drawing.Color.White;
            this.total_Title.InitialText = "Cumulative Totals (For Self)";
            this.total_Title.Location = new System.Drawing.Point(24, 4);
            this.total_Title.Name = "total_Title";
            this.total_Title.Size = new System.Drawing.Size(252, 16);
            this.total_Title.TabIndex = 70;
            this.total_Title.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // total_lblMisc
            // 
            this.total_lblMisc.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.total_lblMisc.ForeColor = System.Drawing.Color.White;
            this.total_lblMisc.Location = new System.Drawing.Point(4, 228);
            this.total_lblMisc.Name = "total_lblMisc";
            this.total_lblMisc.Size = new System.Drawing.Size(292, 16);
            this.total_lblMisc.TabIndex = 28;
            this.total_lblMisc.Text = "Misc Effects:";
            this.total_lblMisc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // total_Misc
            // 
            this.total_Misc.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.total_Misc.Columns = 2;
            this.total_Misc.DoHighlight = true;
            this.total_Misc.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.total_Misc.ForceBold = false;
            this.total_Misc.HighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.total_Misc.HighlightTextColor = System.Drawing.Color.Black;
            this.total_Misc.ItemColor = System.Drawing.Color.White;
            this.total_Misc.ItemColorAlt = System.Drawing.Color.Lime;
            this.total_Misc.ItemColorSpecCase = System.Drawing.Color.Red;
            this.total_Misc.ItemColorSpecial = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.total_Misc.Location = new System.Drawing.Point(4, 244);
            this.total_Misc.Name = "total_Misc";
            this.total_Misc.NameColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.total_Misc.Size = new System.Drawing.Size(292, 60);
            this.total_Misc.TabIndex = 27;
            this.total_Misc.ValueWidth = 55;
            // 
            // total_lblRes
            // 
            this.total_lblRes.BackColor = System.Drawing.Color.Green;
            this.total_lblRes.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.total_lblRes.ForeColor = System.Drawing.Color.White;
            this.total_lblRes.Location = new System.Drawing.Point(4, 136);
            this.total_lblRes.Name = "total_lblRes";
            this.total_lblRes.Size = new System.Drawing.Size(292, 16);
            this.total_lblRes.TabIndex = 26;
            this.total_lblRes.Text = "Resistance:";
            this.total_lblRes.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // total_lblDef
            // 
            this.total_lblDef.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.total_lblDef.ForeColor = System.Drawing.Color.White;
            this.total_lblDef.Location = new System.Drawing.Point(4, 24);
            this.total_lblDef.Name = "total_lblDef";
            this.total_lblDef.Size = new System.Drawing.Size(292, 16);
            this.total_lblDef.TabIndex = 25;
            this.total_lblDef.Text = "Defense:";
            this.total_lblDef.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlEnh
            // 
            this.pnlEnh.BackColor = System.Drawing.Color.Teal;
            this.pnlEnh.Controls.Add(this.pnlEnhInactive);
            this.pnlEnh.Controls.Add(this.pnlEnhActive);
            this.pnlEnh.Controls.Add(this.enhNameDisp);
            this.pnlEnh.Controls.Add(this.enhListing);
            this.pnlEnh.Controls.Add(this.Enh_Title);
            this.pnlEnh.Location = new System.Drawing.Point(612, 3);
            this.pnlEnh.Name = "pnlEnh";
            this.pnlEnh.Size = new System.Drawing.Size(300, 420);
            this.pnlEnh.TabIndex = 65;
            // 
            // pnlEnhInactive
            // 
            this.pnlEnhInactive.BackColor = System.Drawing.Color.Black;
            this.pnlEnhInactive.Location = new System.Drawing.Point(4, 279);
            this.pnlEnhInactive.Name = "pnlEnhInactive";
            this.pnlEnhInactive.Size = new System.Drawing.Size(292, 38);
            this.pnlEnhInactive.TabIndex = 74;
            // 
            // pnlEnhActive
            // 
            this.pnlEnhActive.BackColor = System.Drawing.Color.Black;
            this.pnlEnhActive.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlEnhActive.Location = new System.Drawing.Point(4, 239);
            this.pnlEnhActive.Name = "pnlEnhActive";
            this.pnlEnhActive.Size = new System.Drawing.Size(292, 38);
            this.pnlEnhActive.TabIndex = 73;
            // 
            // enhNameDisp
            // 
            this.enhNameDisp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.enhNameDisp.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.enhNameDisp.ForeColor = System.Drawing.Color.White;
            this.enhNameDisp.InitialText = "Title";
            this.enhNameDisp.Location = new System.Drawing.Point(4, 24);
            this.enhNameDisp.Name = "enhNameDisp";
            this.enhNameDisp.Size = new System.Drawing.Size(292, 16);
            this.enhNameDisp.TabIndex = 72;
            this.enhNameDisp.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // enhListing
            // 
            this.enhListing.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(32)))));
            this.enhListing.Columns = 1;
            this.enhListing.DoHighlight = true;
            this.enhListing.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.enhListing.ForceBold = false;
            this.enhListing.HighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.enhListing.HighlightTextColor = System.Drawing.Color.Black;
            this.enhListing.ItemColor = System.Drawing.Color.White;
            this.enhListing.ItemColorAlt = System.Drawing.Color.Yellow;
            this.enhListing.ItemColorSpecCase = System.Drawing.Color.Red;
            this.enhListing.ItemColorSpecial = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.enhListing.Location = new System.Drawing.Point(4, 44);
            this.enhListing.Name = "enhListing";
            this.enhListing.NameColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.enhListing.Size = new System.Drawing.Size(292, 192);
            this.enhListing.TabIndex = 71;
            this.enhListing.ValueWidth = 65;
            // 
            // Enh_Title
            // 
            this.Enh_Title.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Enh_Title.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Enh_Title.ForeColor = System.Drawing.Color.White;
            this.Enh_Title.InitialText = "Title";
            this.Enh_Title.Location = new System.Drawing.Point(24, 4);
            this.Enh_Title.Name = "Enh_Title";
            this.Enh_Title.Size = new System.Drawing.Size(252, 16);
            this.Enh_Title.TabIndex = 70;
            this.Enh_Title.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // dbTip
            // 
            this.dbTip.AutoPopDelay = 15000;
            this.dbTip.InitialDelay = 350;
            this.dbTip.ReshowDelay = 100;
            // 
            // lblFloat
            // 
            this.lblFloat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblFloat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFloat.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(2)));
            this.lblFloat.ForeColor = System.Drawing.Color.White;
            this.lblFloat.Location = new System.Drawing.Point(4, 24);
            this.lblFloat.Name = "lblFloat";
            this.lblFloat.Size = new System.Drawing.Size(16, 16);
            this.lblFloat.TabIndex = 66;
            this.lblFloat.Text = "F";
            this.lblFloat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.dbTip.SetToolTip(this.lblFloat, "Make Floating Window");
            this.lblFloat.UseCompatibleTextRendering = true;
            // 
            // lblShrink
            // 
            this.lblShrink.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblShrink.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblShrink.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.lblShrink.ForeColor = System.Drawing.Color.White;
            this.lblShrink.Location = new System.Drawing.Point(280, 24);
            this.lblShrink.Name = "lblShrink";
            this.lblShrink.Size = new System.Drawing.Size(16, 16);
            this.lblShrink.TabIndex = 67;
            this.lblShrink.Text = "y";
            this.lblShrink.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.dbTip.SetToolTip(this.lblShrink, "Shrink / Expand the Info Display");
            this.lblShrink.UseCompatibleTextRendering = true;
            // 
            // lblLock
            // 
            this.lblLock.BackColor = System.Drawing.Color.Red;
            this.lblLock.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLock.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLock.ForeColor = System.Drawing.Color.White;
            this.lblLock.Location = new System.Drawing.Point(220, 24);
            this.lblLock.Name = "lblLock";
            this.lblLock.Size = new System.Drawing.Size(56, 16);
            this.lblLock.TabIndex = 68;
            this.lblLock.Text = "[Unlock]";
            this.lblLock.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.dbTip.SetToolTip(this.lblLock, "The info display is currently locked to display a specific power, click here to u" +
        "nlock it to display powers as you hover the mouse over them.");
            // 
            // DataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(32)))));
            this.Controls.Add(this.lblLock);
            this.Controls.Add(this.lblFloat);
            this.Controls.Add(this.lblShrink);
            this.Controls.Add(this.pnlTabs);
            this.Controls.Add(this.pnlInfo);
            this.Controls.Add(this.pnlTotal);
            this.Controls.Add(this.pnlEnh);
            this.Controls.Add(this.pnlFX);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.Name = "DataView";
            this.Size = new System.Drawing.Size(1221, 436);
            this.pnlInfo.ResumeLayout(false);
            this.pnlFX.ResumeLayout(false);
            this.pnlTotal.ResumeLayout(false);
            this.pnlEnh.ResumeLayout(false);
            this.ResumeLayout(false);

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
