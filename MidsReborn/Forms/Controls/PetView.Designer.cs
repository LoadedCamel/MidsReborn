using Mids_Reborn.Controls;

namespace Mids_Reborn.Forms.Controls
{
    partial class PetView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PetView));
            this.info_Damage = new Mids_Reborn.Controls.ctlDamageDisplay();
            this.lblDmg = new System.Windows.Forms.Label();
            this.powerScaler = new Mids_Reborn.Controls.ctlMultiGraph();
            this.info_TxtLarge = new System.Windows.Forms.RichTextBox();
            this.info_TxtSmall = new System.Windows.Forms.RichTextBox();
            this.info_Title = new Mids_Reborn.Controls.GFXLabel();
            this.lockIcon = new System.Windows.Forms.PictureBox();
            this.unlockIcon = new System.Windows.Forms.PictureBox();
            this.infoTip = new System.Windows.Forms.ToolTip(this.components);
            this.info_DataList = new Mids_Reborn.Controls.PairedListEx();
            ((System.ComponentModel.ISupportInitialize)(this.lockIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.unlockIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // info_Damage
            // 
            this.info_Damage.BackColor = System.Drawing.Color.Black;
            this.info_Damage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.info_Damage.ColorBackEnd = System.Drawing.Color.Black;
            this.info_Damage.ColorBackStart = System.Drawing.Color.Black;
            this.info_Damage.ColorBaseEnd = System.Drawing.Color.Blue;
            this.info_Damage.ColorBaseStart = System.Drawing.Color.DodgerBlue;
            this.info_Damage.ColorEnhEnd = System.Drawing.Color.Goldenrod;
            this.info_Damage.ColorEnhStart = System.Drawing.Color.Gold;
            this.info_Damage.Dock = System.Windows.Forms.DockStyle.Top;
            this.info_Damage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.info_Damage.GraphType = Mids_Reborn.Core.Enums.eDDGraph.Both;
            this.info_Damage.Location = new System.Drawing.Point(0, 361);
            this.info_Damage.Name = "info_Damage";
            this.info_Damage.nBaseVal = 100F;
            this.info_Damage.nEnhVal = 150F;
            this.info_Damage.nHighBase = 200F;
            this.info_Damage.nHighEnh = 214F;
            this.info_Damage.nMaxEnhVal = 207F;
            this.info_Damage.PaddingH = 1;
            this.info_Damage.PaddingV = 1;
            this.info_Damage.Size = new System.Drawing.Size(436, 102);
            this.info_Damage.Style = Mids_Reborn.Core.Enums.eDDStyle.TextUnderGraph;
            this.info_Damage.TabIndex = 21;
            this.info_Damage.TextAlign = Mids_Reborn.Core.Enums.eDDAlign.Center;
            this.info_Damage.TextColor = System.Drawing.Color.White;
            // 
            // lblDmg
            // 
            this.lblDmg.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDmg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDmg.ForeColor = System.Drawing.Color.White;
            this.lblDmg.Location = new System.Drawing.Point(0, 337);
            this.lblDmg.Name = "lblDmg";
            this.lblDmg.Size = new System.Drawing.Size(436, 24);
            this.lblDmg.TabIndex = 22;
            this.lblDmg.Text = "Damage (Blue = Base | Gold = Enhanced)";
            this.lblDmg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // powerScaler
            // 
            this.powerScaler.BackColor = System.Drawing.Color.Black;
            this.powerScaler.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("powerScaler.BackgroundImage")));
            this.powerScaler.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.powerScaler.BaseBarColors = null;
            this.powerScaler.Border = true;
            this.powerScaler.BorderColor = System.Drawing.Color.Black;
            this.powerScaler.Clickable = true;
            this.powerScaler.ColorAbsorbed = System.Drawing.Color.Gainsboro;
            this.powerScaler.ColorBase = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(255)))), ((int)(((byte)(64)))));
            this.powerScaler.ColorEnh = System.Drawing.Color.Yellow;
            this.powerScaler.ColorFadeEnd = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.powerScaler.ColorFadeStart = System.Drawing.Color.Black;
            this.powerScaler.ColorHighlight = System.Drawing.Color.Gray;
            this.powerScaler.ColorLines = System.Drawing.Color.Black;
            this.powerScaler.ColorMarkerInner = System.Drawing.Color.Red;
            this.powerScaler.ColorMarkerOuter = System.Drawing.Color.Black;
            this.powerScaler.ColorOvercap = System.Drawing.Color.Black;
            this.powerScaler.DifferentiateColors = false;
            this.powerScaler.Dock = System.Windows.Forms.DockStyle.Top;
            this.powerScaler.DrawRuler = false;
            this.powerScaler.Dual = false;
            this.powerScaler.EnhBarColors = null;
            this.powerScaler.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.powerScaler.ForcedMax = 0F;
            this.powerScaler.ForeColor = System.Drawing.Color.Azure;
            this.powerScaler.Highlight = false;
            this.powerScaler.ItemFontSizeOverride = 0F;
            this.powerScaler.ItemHeight = 12;
            this.powerScaler.Lines = true;
            this.powerScaler.Location = new System.Drawing.Point(0, 173);
            this.powerScaler.MarkerValue = 0F;
            this.powerScaler.Max = 100F;
            this.powerScaler.MaxItems = 1;
            this.powerScaler.Name = "powerScaler";
            this.powerScaler.OuterBorder = true;
            this.powerScaler.Overcap = false;
            this.powerScaler.OvercapColors = null;
            this.powerScaler.PaddingX = 2F;
            this.powerScaler.PaddingY = 2F;
            this.powerScaler.PerItemScales = null;
            this.powerScaler.RulerPos = Mids_Reborn.Controls.ctlMultiGraph.RulerPosition.Top;
            this.powerScaler.ScaleHeight = 32;
            this.powerScaler.ScaleIndex = 8;
            this.powerScaler.ShowScale = true;
            this.powerScaler.Size = new System.Drawing.Size(436, 19);
            this.powerScaler.Style = Mids_Reborn.Core.Enums.GraphStyle.baseOnly;
            this.powerScaler.TabIndex = 72;
            this.powerScaler.TextWidth = 100;
            this.powerScaler.BarClick += new Mids_Reborn.Controls.ctlMultiGraph.BarClickEventHandler(this.powerScaler_BarClick);
            // 
            // info_TxtLarge
            // 
            this.info_TxtLarge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(245)))), ((int)(((byte)(229)))));
            this.info_TxtLarge.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.info_TxtLarge.Dock = System.Windows.Forms.DockStyle.Top;
            this.info_TxtLarge.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.info_TxtLarge.ForeColor = System.Drawing.Color.Black;
            this.info_TxtLarge.Location = new System.Drawing.Point(0, 78);
            this.info_TxtLarge.Name = "info_TxtLarge";
            this.info_TxtLarge.ReadOnly = true;
            this.info_TxtLarge.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.info_TxtLarge.Size = new System.Drawing.Size(436, 95);
            this.info_TxtLarge.TabIndex = 73;
            this.info_TxtLarge.Text = "info_Rich";
            // 
            // info_TxtSmall
            // 
            this.info_TxtSmall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(245)))), ((int)(((byte)(229)))));
            this.info_TxtSmall.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.info_TxtSmall.Dock = System.Windows.Forms.DockStyle.Top;
            this.info_TxtSmall.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.info_TxtSmall.ForeColor = System.Drawing.Color.Black;
            this.info_TxtSmall.Location = new System.Drawing.Point(0, 37);
            this.info_TxtSmall.Name = "info_TxtSmall";
            this.info_TxtSmall.ReadOnly = true;
            this.info_TxtSmall.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.info_TxtSmall.Size = new System.Drawing.Size(436, 41);
            this.info_TxtSmall.TabIndex = 74;
            this.info_TxtSmall.Text = "info_Rich";
            // 
            // info_Title
            // 
            this.info_Title.BackColor = System.Drawing.Color.Black;
            this.info_Title.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.info_Title.Dock = System.Windows.Forms.DockStyle.Top;
            this.info_Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.info_Title.ForeColor = System.Drawing.Color.White;
            this.info_Title.InitialText = "Title";
            this.info_Title.Location = new System.Drawing.Point(0, 0);
            this.info_Title.Name = "info_Title";
            this.info_Title.Size = new System.Drawing.Size(436, 37);
            this.info_Title.TabIndex = 75;
            this.info_Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lockIcon
            // 
            this.lockIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lockIcon.BackColor = System.Drawing.Color.Black;
            this.lockIcon.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("lockIcon.BackgroundImage")));
            this.lockIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lockIcon.Location = new System.Drawing.Point(401, 4);
            this.lockIcon.Name = "lockIcon";
            this.lockIcon.Size = new System.Drawing.Size(30, 30);
            this.lockIcon.TabIndex = 77;
            this.lockIcon.TabStop = false;
            this.lockIcon.Click += new System.EventHandler(this.Lock_Click);
            // 
            // unlockIcon
            // 
            this.unlockIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.unlockIcon.BackColor = System.Drawing.Color.Black;
            this.unlockIcon.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("unlockIcon.BackgroundImage")));
            this.unlockIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.unlockIcon.Location = new System.Drawing.Point(401, 4);
            this.unlockIcon.Name = "unlockIcon";
            this.unlockIcon.Size = new System.Drawing.Size(30, 30);
            this.unlockIcon.TabIndex = 78;
            this.unlockIcon.TabStop = false;
            this.unlockIcon.Click += new System.EventHandler(this.Lock_Click);
            // 
            // info_DataList
            // 
            this.info_DataList.AutoScroll = true;
            this.info_DataList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.info_DataList.Columns = 2;
            this.info_DataList.Dock = System.Windows.Forms.DockStyle.Top;
            this.info_DataList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.info_DataList.HighlightColor = System.Drawing.Color.CornflowerBlue;
            this.info_DataList.HighlightTextColor = System.Drawing.Color.Black;
            this.info_DataList.ItemColor = System.Drawing.Color.Silver;
            this.info_DataList.Location = new System.Drawing.Point(0, 192);
            this.info_DataList.Name = "info_DataList";
            this.info_DataList.Rows = 5;
            this.info_DataList.Size = new System.Drawing.Size(436, 145);
            this.info_DataList.TabIndex = 79;
            this.info_DataList.UseHighlighting = true;
            this.info_DataList.ValueAlternateColor = System.Drawing.Color.Chartreuse;
            this.info_DataList.ValueColor = System.Drawing.Color.Azure;
            this.info_DataList.ValueConditionColor = System.Drawing.Color.Firebrick;
            this.info_DataList.ValueSpecialColor = System.Drawing.Color.SlateBlue;
            // 
            // PetView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.info_Damage);
            this.Controls.Add(this.lblDmg);
            this.Controls.Add(this.info_DataList);
            this.Controls.Add(this.lockIcon);
            this.Controls.Add(this.unlockIcon);
            this.Controls.Add(this.powerScaler);
            this.Controls.Add(this.info_TxtLarge);
            this.Controls.Add(this.info_TxtSmall);
            this.Controls.Add(this.info_Title);
            this.Name = "PetView";
            this.Size = new System.Drawing.Size(436, 464);
            ((System.ComponentModel.ISupportInitialize)(this.lockIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.unlockIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        internal Mids_Reborn.Controls.ctlDamageDisplay info_Damage;
        private System.Windows.Forms.Label lblDmg;
        private Mids_Reborn.Controls.ctlMultiGraph powerScaler;
        internal System.Windows.Forms.RichTextBox info_TxtLarge;
        private System.Windows.Forms.RichTextBox info_TxtSmall;
        private Mids_Reborn.Controls.GFXLabel info_Title;
        private System.Windows.Forms.PictureBox lockIcon;
        private System.Windows.Forms.PictureBox unlockIcon;
        private System.Windows.Forms.ToolTip infoTip;
        private PairedListEx info_DataList;
    }
}
