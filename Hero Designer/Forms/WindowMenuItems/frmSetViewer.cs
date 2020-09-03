using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Base.Display;
using Base.Master_Classes;
using midsControls;

namespace Hero_Designer.Forms.WindowMenuItems
{
    public partial class frmSetViewer : Form
    {
        private readonly frmMain myParent;
        private ImageButton btnClose;

        private ImageButton btnSmall;

        private ImageButton chkOnTop;
        private ColumnHeader ColumnHeader1;
        private ColumnHeader ColumnHeader2;
        private ColumnHeader ColumnHeader3;
        private ImageList ilSet;
        private Label Label1;
        private Label Label2;

        private ListView lstSets;
        private RichTextBox rtApplied;
        private RichTextBox rtxtFX;
        private RichTextBox rtxtInfo;

        public frmSetViewer(frmMain iParent)
        {
            Move += frmSetViewer_Move;
            FormClosed += frmSetViewer_FormClosed;
            Load += frmSetViewer_Load;
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmSetViewer));
            Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
            Name = nameof(frmSetViewer);
            myParent = iParent;
        }

        private void btnClose_Click()
        {
            Close();
        }

        private void btnSmall_Click()
        {
            if (Width > 600)
            {
                Width = 387;
                rtxtInfo.Height = btnClose.Top - (rtxtInfo.Top + 8);
                btnSmall.Left = rtxtInfo.Width + rtxtInfo.Left - btnSmall.Width;
                btnClose.Left = btnSmall.Left - (btnClose.Width + 8);
                chkOnTop.Left = btnClose.Left - (chkOnTop.Width + 4);
                chkOnTop.Top = (int) Math.Round(btnClose.Top + (btnClose.Height - chkOnTop.Height) / 2.0);
                btnSmall.TextOff = "Expand >>";
            }
            else
            {
                Width = 681;
                rtxtInfo.Height = 132;
                btnClose.Left = 558;
                btnClose.Top = 418;
                btnSmall.Left = 384;
                btnSmall.Top = 418;
                chkOnTop.Left = 558;
                chkOnTop.Top = 392;
                btnSmall.TextOff = "<< Shrink";
            }

            StoreLocation();
        }

        private void chkOnTop_CheckedChanged()
        {
            TopMost = chkOnTop.Checked;
        }

        private void DisplayList()
        {
            var items = new string[3];
            lstSets.BeginUpdate();
            lstSets.Items.Clear();
            var imageIndex = -1;
            FillImageList();
            var num1 = MidsContext.Character.CurrentBuild.SetBonus.Count - 1;
            for (var index1 = 0; index1 <= num1; ++index1)
            {
                var num2 = MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo.Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    var setInfo = MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo;
                    var index3 = index2;
                    items[0] = DatabaseAPI.Database.EnhancementSets[setInfo[index3].SetIDX].DisplayName;
                    items[1] =
                        MidsContext.Character.CurrentBuild
                            .Powers[MidsContext.Character.CurrentBuild.SetBonus[index1].PowerIndex]
                            .NIDPowerset <= -1
                            ? ""
                            : DatabaseAPI.Database
                                .Powersets[
                                    MidsContext.Character.CurrentBuild
                                        .Powers[MidsContext.Character.CurrentBuild.SetBonus[index1].PowerIndex]
                                        .NIDPowerset].Powers[
                                    MainModule.MidsController.Toon.CurrentBuild
                                        .Powers[MidsContext.Character.CurrentBuild.SetBonus[index1].PowerIndex]
                                        .IDXPower].DisplayName;
                    items[2] = Convert.ToString(setInfo[index3].SlottedCount);
                    ++imageIndex;
                    lstSets.Items.Add(new ListViewItem(items, imageIndex));
                    lstSets.Items[lstSets.Items.Count - 1].Tag = setInfo[index3].SetIDX;
                }
            }

            lstSets.EndUpdate();
            if (lstSets.Items.Count > 0)
                lstSets.Items[0].Selected = true;
            FillEffectView();
        }

        private void FillEffectView()
        {
            var str1 = "";
            var numArray = new int[DatabaseAPI.NidPowers("set_bonus").Length - 1 + 1];
            var hasOvercap = false;
            var num1 = MidsContext.Character.CurrentBuild.SetBonus.Count - 1;
            for (var index1 = 0; index1 <= num1; ++index1)
            {
                var num2 = MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo.Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    if (MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].Powers.Length <= 0)
                        continue;
                    var setInfo = MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo;
                    var index3 = index2;
                    var enhancementSet = DatabaseAPI.Database.EnhancementSets[setInfo[index3].SetIDX];
                    var str2 = str1 + RTF.Color(RTF.ElementID.Invention) +
                               RTF.Underline(RTF.Bold(enhancementSet.DisplayName));
                    if (MidsContext.Character.CurrentBuild
                            .Powers[MidsContext.Character.CurrentBuild.SetBonus[index1].PowerIndex].NIDPowerset >
                        -1)
                        str2 = str2 + RTF.Crlf() + RTF.Color(RTF.ElementID.Faded) + "(" + DatabaseAPI.Database
                            .Powersets[
                                MidsContext.Character.CurrentBuild
                                    .Powers[MidsContext.Character.CurrentBuild.SetBonus[index1].PowerIndex].NIDPowerset]
                            .Powers[
                                MidsContext.Character.CurrentBuild
                                    .Powers[MidsContext.Character.CurrentBuild.SetBonus[index1].PowerIndex].IDXPower]
                            .DisplayName + ")";
                    var str3 = str2 + RTF.Crlf() + RTF.Color(RTF.ElementID.Text);
                    var str4 = "";
                    var num3 = enhancementSet.Bonus.Length - 1;
                    for (var index4 = 0; index4 <= num3; ++index4)
                    {
                        if (!((setInfo[index3].SlottedCount >= enhancementSet.Bonus[index4].Slotted) &
                              ((enhancementSet.Bonus[index4].PvMode == Enums.ePvX.Any) |
                               ((enhancementSet.Bonus[index4].PvMode == Enums.ePvX.PvE) &
                                !MidsContext.Config.Inc.DisablePvE) |
                               ((enhancementSet.Bonus[index4].PvMode == Enums.ePvX.PvP) &
                                MidsContext.Config.Inc.DisablePvE))))
                            continue;
                        if (str4 != "")
                            str4 += RTF.Crlf();
                        var localOverCap = false;
                        var str5 = "  " + enhancementSet.GetEffectString(index4, false, true);
                        var num4 = enhancementSet.Bonus[index4].Index.Length - 1;
                        for (var index5 = 0; index5 <= num4; ++index5)
                        {
                            if (enhancementSet.Bonus[index4].Index[index5] <= -1)
                                continue;
                            ++numArray[enhancementSet.Bonus[index4].Index[index5]];
                            if (numArray[enhancementSet.Bonus[index4].Index[index5]] > 5)
                                localOverCap = true;
                        }

                        if (localOverCap)
                            str5 = RTF.Italic(RTF.Color(RTF.ElementID.Warning) + str5 + " >Cap" +
                                              RTF.Color(RTF.ElementID.Text));
                        if (localOverCap)
                            hasOvercap = true;
                        str4 += str5;
                    }

                    var num5 = MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].EnhIndexes.Length -
                               1;
                    for (var index4 = 0; index4 <= num5; ++index4)
                    {
                        var index5 = DatabaseAPI.IsSpecialEnh(MidsContext.Character.CurrentBuild.SetBonus[index1]
                            .SetInfo[index2]
                            .EnhIndexes[index4]);
                        if (index5 <= -1)
                            continue;
                        if (str4 != "")
                            str4 += RTF.Crlf();
                        var str5 = str4 + RTF.Color(RTF.ElementID.Enhancement);
                        var localOverCap = false;
                        var str6 = "  " + DatabaseAPI.Database
                            .EnhancementSets[MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX]
                            .GetEffectString(index5, true, true);
                        var num4 = DatabaseAPI.Database
                            .EnhancementSets[MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX]
                            .SpecialBonus[index5].Index.Length - 1;
                        for (var index6 = 0; index6 <= num4; ++index6)
                        {
                            if (DatabaseAPI.Database
                                .EnhancementSets[
                                    MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX]
                                .SpecialBonus[index5].Index[index6] <= -1)
                                continue;
                            ++numArray[
                                DatabaseAPI.Database
                                    .EnhancementSets[
                                        MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX]
                                    .SpecialBonus[index5].Index[index6]];
                            if (numArray[
                                DatabaseAPI.Database
                                    .EnhancementSets[
                                        MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX]
                                    .SpecialBonus[index5].Index[index6]] > 5)
                                localOverCap = true;
                        }

                        if (localOverCap)
                            str6 = RTF.Italic(RTF.Color(RTF.ElementID.Warning) + str6 + " >Cap" +
                                              RTF.Color(RTF.ElementID.Text));
                        if (localOverCap)
                            hasOvercap = true;
                        str4 = str5 + str6;
                    }

                    str1 = str3 + str4 + RTF.Crlf() + RTF.Crlf();
                }
            }

            string str7;
            if (hasOvercap)
                str7 = RTF.Color(RTF.ElementID.Invention) + RTF.Underline(RTF.Bold("Information:")) + RTF.Crlf() +
                       RTF.Color(RTF.ElementID.Text) +
                       "One or more set bonuses have exceeded the 5 bonus cap, and will not affect your stats. Scroll down this list to find bonuses marked as '" +
                       RTF.Italic(RTF.Color(RTF.ElementID.Warning) + ">Cap") + RTF.Color(RTF.ElementID.Text) + "'" +
                       RTF.Crlf() + RTF.Crlf();
            else
                str7 = "";
            var str8 = RTF.StartRTF() + str7 + str1 + RTF.EndRTF();
            if (rtxtFX.Rtf != str8)
                rtxtFX.Rtf = str8;
            var cumulativeSetBonuses = MidsContext.Character.CurrentBuild.GetCumulativeSetBonuses();
            Array.Sort(cumulativeSetBonuses);
            var iStr = "";
            var num6 = cumulativeSetBonuses.Length - 1;
            for (var index = 0; index <= num6; ++index)
            {
                if (iStr != "")
                    iStr += RTF.Crlf();
                var str2 = cumulativeSetBonuses[index].BuildEffectString(true);
                if (!str2.StartsWith("+"))
                    str2 = "+" + str2;
                if (str2.IndexOf("Endurance", StringComparison.Ordinal) > -1)
                    str2 = str2.Replace("Endurance", "Max Endurance");
                iStr += str2;
            }

            var str9 = RTF.StartRTF() + RTF.ToRTF(iStr) + RTF.EndRTF();
            if (rtApplied.Rtf == str9)
                return;
            rtApplied.Rtf = str9;
        }

        private void FillImageList()
        {
            var imageSize1 = ilSet.ImageSize;
            var width1 = imageSize1.Width;
            imageSize1 = ilSet.ImageSize;
            var height1 = imageSize1.Height;
            var extendedBitmap = new ExtendedBitmap(width1, height1);
            ilSet.Images.Clear();
            var setBonusCount = MidsContext.Character.CurrentBuild.SetBonus.Count - 1;
            for (var index1 = 0; index1 <= setBonusCount; ++index1)
            {
                var num2 = MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo.Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    if (MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX <= -1)
                        continue;
                    var enhancementSet =
                        DatabaseAPI.Database.EnhancementSets[
                            MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX];
                    if (enhancementSet.ImageIdx > -1)
                    {
                        extendedBitmap.Graphics.Clear(Color.White);
                        var graphics = extendedBitmap.Graphics;
                        I9Gfx.DrawEnhancementSet(ref graphics, enhancementSet.ImageIdx);
                        ilSet.Images.Add(extendedBitmap.Bitmap);
                    }
                    else
                    {
                        var images = ilSet.Images;
                        var imageSize2 = ilSet.ImageSize;
                        var width2 = imageSize2.Width;
                        imageSize2 = ilSet.ImageSize;
                        var height2 = imageSize2.Height;
                        var bitmap = new Bitmap(width2, height2);
                        images.Add(bitmap);
                    }
                }
            }
        }

        private void frmSetViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            myParent.FloatSets(false);
        }

        private void frmSetViewer_Load(object sender, EventArgs e)
        {
        }

        private void frmSetViewer_Move(object sender, EventArgs e)
        {
            StoreLocation();
        }

        private void lstSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSets.SelectedItems.Count < 1)
                return;
            rtxtInfo.Rtf = RTF.StartRTF() + EnhancementSetCollection.GetSetInfoLongRTF(
                Convert.ToInt32(lstSets.SelectedItems[0].Tag),
                Convert.ToInt32(lstSets.SelectedItems[0].SubItems[2].Text)) + RTF.EndRTF();
        }

        public void SetLocation()
        {
            var rectangle = new Rectangle
            {
                X = MainModule.MidsController.SzFrmSets.X, Y = MainModule.MidsController.SzFrmSets.Y
            };
            if (rectangle.X < 1)
                rectangle.X = myParent.Left + 8;
            if (rectangle.Y < 32)
                rectangle.Y = myParent.Top + (myParent.Height - myParent.ClientSize.Height) +
                              myParent.GetPrimaryBottom();
            if (MidsContext.Config.ShrinkFrmSets & (Width > 600))
                btnSmall_Click();
            else if (!MidsContext.Config.ShrinkFrmSets & (Width < 600))
                btnSmall_Click();
            Top = rectangle.Y;
            Left = rectangle.X;
        }

        private void StoreLocation()
        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            MainModule.MidsController.SzFrmSets.X = Left;
            MainModule.MidsController.SzFrmSets.Y = Top;
            MidsContext.Config.ShrinkFrmSets = Width < 600;
        }

        public void UpdateData()
        {
            if (myParent == null)
                return;
            BackColor = myParent.BackColor;
            if (rtApplied.BackColor != BackColor)
                rtApplied.BackColor = BackColor;
            if (rtxtFX.BackColor != BackColor)
                rtxtFX.BackColor = BackColor;
            if (rtxtInfo.BackColor != BackColor)
                rtxtInfo.BackColor = BackColor;
            btnClose.IA = myParent.Drawing.pImageAttributes;
            btnClose.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            btnClose.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            chkOnTop.IA = myParent.Drawing.pImageAttributes;
            chkOnTop.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            chkOnTop.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            btnSmall.IA = myParent.Drawing.pImageAttributes;
            btnSmall.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            btnSmall.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            DisplayList();
        }
    }
}