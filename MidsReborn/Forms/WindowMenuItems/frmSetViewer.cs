using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Display;
using mrbBase.Base.Master_Classes;
using mrbControls;

namespace Mids_Reborn.Forms.WindowMenuItems
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
            Icon = Resources.reborn;
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

        private (string enhSet, string power) GetEffectSourceData(string powerFullName,
            Enums.eEffectType effectType,
            ref Dictionary<string, List<(string enhSet, string power, Enums.eEffectType effectTypeLock)>> sourceFxDict)
        {
            if (!sourceFxDict.ContainsKey(powerFullName)) return ("", "");
            for (var i = 0; i < sourceFxDict[powerFullName].Count; i++)
            {
                if (sourceFxDict[powerFullName][i].effectTypeLock != Enums.eEffectType.None &
                    sourceFxDict[powerFullName][i].effectTypeLock != effectType
                ) continue;

                effectType = effectType switch
                {
                    Enums.eEffectType.JumpHeight => Enums.eEffectType.SpeedRunning,
                    Enums.eEffectType.SpeedJumping => Enums.eEffectType.SpeedRunning,
                    Enums.eEffectType.SpeedFlying => Enums.eEffectType.SpeedRunning,
                    _ => effectType
                };

                sourceFxDict[powerFullName][i] = (sourceFxDict[powerFullName][i].enhSet, sourceFxDict[powerFullName][i].power, effectType);
                return (sourceFxDict[powerFullName][i].enhSet, sourceFxDict[powerFullName][i].power);
            }

            return ("", "");
        }

        private Dictionary<string, List<(string enhSet, string power, Enums.eEffectType effectTypeLock)>> GetEffectSources()
        {
            var ret = new Dictionary<string, List<(string enhSet, string power, Enums.eEffectType effectTypeLock)>>();

            foreach (var s in MidsContext.Character.CurrentBuild.SetBonus)
            {
                for (var i = 0; i < s.SetInfo.Length; i++)
                {
                    var enhSet = DatabaseAPI.Database.EnhancementSets[s.SetInfo[i].SetIDX];
                    var linkedPower = MidsContext.Character.CurrentBuild
                        .Powers[s.PowerIndex]
                        .NIDPowerset <= -1
                        ? ""
                        : DatabaseAPI.Database
                            .Powersets[
                                MidsContext.Character.CurrentBuild
                                    .Powers[s.PowerIndex]
                                    .NIDPowerset].Powers[
                                MainModule.MidsController.Toon.CurrentBuild
                                    .Powers[s.PowerIndex]
                                    .IDXPower].DisplayName;

                    for (var j = 0; j < enhSet.Bonus.Length; j++)
                    {
                        foreach (var b in enhSet.Bonus[j].Name)
                        {
                            Debug.WriteLine($"Bonus name: {b}, power: {linkedPower}, set: {enhSet.DisplayName}");
                            if (!ret.ContainsKey(b))
                            {
                                ret.Add(b, new List<(string enhSet, string power, Enums.eEffectType effectTypeLock)>());
                            }

                            ret[b] = ret[b].Append((enhSet.DisplayName, linkedPower, Enums.eEffectType.None)).ToList();
                        }
                    }
                }
            }

            return ret;
        }

        private void DisplayList()
        {
            var items = new string[3];
            lstSets.BeginUpdate();
            lstSets.Items.Clear();
            var imageIndex = -1;
            FillImageList();
            foreach (var s in MidsContext.Character.CurrentBuild.SetBonus)
            {
                for (var index2 = 0; index2 < s.SetInfo.Length; index2++)
                {
                    var setInfo = s.SetInfo;
                    var index3 = index2;
                    items[0] = DatabaseAPI.Database.EnhancementSets[setInfo[index3].SetIDX].DisplayName;
                    items[1] =
                        MidsContext.Character.CurrentBuild
                            .Powers[s.PowerIndex]
                            .NIDPowerset <= -1
                            ? ""
                            : DatabaseAPI.Database
                                .Powersets[
                                    MidsContext.Character.CurrentBuild
                                        .Powers[s.PowerIndex]
                                        .NIDPowerset].Powers[
                                    MainModule.MidsController.Toon.CurrentBuild
                                        .Powers[s.PowerIndex]
                                        .IDXPower].DisplayName;
                    items[2] = Convert.ToString(setInfo[index3].SlottedCount);
                    imageIndex++;
                    lstSets.Items.Add(new ListViewItem(items, imageIndex));
                    lstSets.Items[lstSets.Items.Count - 1].Tag = setInfo[index3].SetIDX;
                }
            }

            lstSets.EndUpdate();
            if (lstSets.Items.Count > 0)
                lstSets.Items[0].Selected = true;
            FillEffectView();
        }

        private Dictionary<(Enums.eEffectType effectType, Enums.eMez mezType, Enums.eDamage damageType, Enums.eEffectType targetEffectType),
            List<(IEffect fx, float mag, string enhSet, string power, Enums.ePvX pvMode, bool isFromEnh)>>
            GetEffectSources2()
        {
            var ret = new Dictionary<(Enums.eEffectType effectType, Enums.eMez mezType, Enums.eDamage damageType, Enums.eEffectType targetEffectType),
                List<(IEffect fx, float mag, string enhSet, string power, Enums.ePvX pvMode, bool isFromEnh)>>();
            
            foreach (var s in MidsContext.Character.CurrentBuild.SetBonus)
            {
                for (var i = 0; i < s.SetInfo.Length; i++)
                {
                    if (s.SetInfo[i].Powers.Length <= 0) continue;

                    var enhancementSet = DatabaseAPI.Database.EnhancementSets[s.SetInfo[i].SetIDX];
                    var powerName = DatabaseAPI.Database
                        .Powersets[MidsContext.Character.CurrentBuild.Powers[s.PowerIndex].NIDPowerset]
                        .Powers[MidsContext.Character.CurrentBuild.Powers[s.PowerIndex].IDXPower]
                        .DisplayName;

                    for (var j = 0; j < enhancementSet.Bonus.Length; j++)
                    {
                        var pvMode = enhancementSet.Bonus[j].PvMode;
                        if (!((s.SetInfo[i].SlottedCount >= enhancementSet.Bonus[j].Slotted) &
                              ((pvMode == Enums.ePvX.Any) |
                               ((pvMode == Enums.ePvX.PvE) & !MidsContext.Config.Inc.DisablePvE) |
                               ((pvMode == Enums.ePvX.PvP) & MidsContext.Config.Inc.DisablePvE))))
                            continue;

                        //var setEffectString = enhancementSet.GetEffectString(j, false, true);
                        var setEffectsData = enhancementSet.GetEffectDetailedData(j, false);
                        foreach (var e in setEffectsData)
                        {
                            var tupleKey = (effectType: e.EffectType,
                                mezType: e.MezType,
                                damageType: e.DamageType,
                                targetEffectType: e.ETModifies);
                            if (!ret.ContainsKey(tupleKey))
                            {
                                ret.Add(tupleKey, new List<(IEffect fx, float mag, string enhSet, string power, Enums.ePvX pvMode, bool isFromEnh)>());
                            }

                            ret[tupleKey].Add((e, e.Mag, enhancementSet.DisplayName, powerName, pvMode, false));
                        }
                    }

                    foreach (var si in s.SetInfo[i].EnhIndexes)
                    {
                        var specialEnhIdx = DatabaseAPI.IsSpecialEnh(si);
                        if (specialEnhIdx <= -1) continue;
                        
                        //var enhEffectString = DatabaseAPI.Database.EnhancementSets[s.SetInfo[i].SetIDX].GetEffectString(specialEnhIdx, true, true);
                        var enhEffectsData = enhancementSet.GetEffectDetailedData(specialEnhIdx, true);
                        foreach (var e in enhEffectsData)
                        {
                            var tupleKey = (effectType: e.EffectType,
                                mezType: e.MezType,
                                damageType: e.DamageType,
                                targetEffectType: e.ETModifies);
                            if (!ret.ContainsKey(tupleKey))
                            {
                                ret.Add(tupleKey, new List<(IEffect fx, float mag, string enhSet, string power, Enums.ePvX pvMode, bool isFromEnh)>());
                            }

                            ret[tupleKey].Add((e, e.Mag, enhancementSet.DisplayName, powerName, Enums.ePvX.Any, true));
                        }
                    }
                }
            }

            return ret;
        }

        private void FillEffectView(bool getDetails = false)
        {
            var str1 = "";
            var numArray = new int[DatabaseAPI.NidPowers("set_bonus").Length];
            var hasOvercap = false;
            foreach (var s in MidsContext.Character.CurrentBuild.SetBonus)
            {
                for (var index2 = 0; index2 < s.SetInfo.Length; index2++)
                {
                    if (s.SetInfo[index2].Powers.Length <= 0) continue;

                    var setInfo = s.SetInfo;
                    var index3 = index2;
                    var enhancementSet = DatabaseAPI.Database.EnhancementSets[setInfo[index3].SetIDX];
                    var str2 = str1 + RTF.Color(RTF.ElementID.Invention) + RTF.Underline(RTF.Bold(enhancementSet.DisplayName));
                    if (MidsContext.Character.CurrentBuild.Powers[s.PowerIndex].NIDPowerset > -1)
                        str2 += RTF.Crlf() + RTF.Color(RTF.ElementID.Faded) + "(" + DatabaseAPI.Database
                            .Powersets[
                                MidsContext.Character.CurrentBuild
                                    .Powers[s.PowerIndex].NIDPowerset]
                            .Powers[
                                MidsContext.Character.CurrentBuild
                                    .Powers[s.PowerIndex].IDXPower]
                            .DisplayName + ")";
                    var str3 = str2 + RTF.Crlf() + RTF.Color(RTF.ElementID.Text);
                    var str4 = "";
                    for (var index4 = 0; index4 < enhancementSet.Bonus.Length; index4++)
                    {
                        if (!((setInfo[index3].SlottedCount >= enhancementSet.Bonus[index4].Slotted) &
                              ((enhancementSet.Bonus[index4].PvMode == Enums.ePvX.Any) |
                               ((enhancementSet.Bonus[index4].PvMode == Enums.ePvX.PvE) &
                                !MidsContext.Config.Inc.DisablePvE) |
                               ((enhancementSet.Bonus[index4].PvMode == Enums.ePvX.PvP) &
                                MidsContext.Config.Inc.DisablePvE))))
                            continue;
                        if (str4 != "") str4 += RTF.Crlf();
                        var localOverCap = false;
                        var str5 = "  " + enhancementSet.GetEffectString(index4, false, true);
                        foreach (var esb in enhancementSet.Bonus[index4].Index)
                        {
                            if (esb <= -1) continue;

                            numArray[esb]++;
                            if (numArray[esb] > 5)
                            {
                                localOverCap = true;
                            }
                        }

                        if (localOverCap)
                        {
                            str5 = RTF.Italic(RTF.Color(RTF.ElementID.Warning) + str5 + " >Cap" +
                                              RTF.Color(RTF.ElementID.Text));
                            hasOvercap = true;
                        }

                        str4 += str5;
                    }

                    foreach (var si in s.SetInfo[index2].EnhIndexes)
                    {
                        var index5 = DatabaseAPI.IsSpecialEnh(si);
                        if (index5 <= -1) continue;
                        if (str4 != "")
                            str4 += RTF.Crlf();
                        var str5 = str4 + RTF.Color(RTF.ElementID.Enhancement);
                        var localOverCap = false;
                        var str6 = "  " + DatabaseAPI.Database
                            .EnhancementSets[s.SetInfo[index2].SetIDX]
                            .GetEffectString(index5, true, true);
                        foreach (var sb in DatabaseAPI.Database.EnhancementSets[s.SetInfo[index2].SetIDX].SpecialBonus[index5].Index)
                        {
                            if (sb <= -1) continue;
                            numArray[sb]++;
                            if (numArray[sb] > 5)
                            {
                                localOverCap = true;
                            }
                        }

                        if (localOverCap)
                        {
                            str6 = RTF.Italic(RTF.Color(RTF.ElementID.Warning) + str6 + " >Cap" + RTF.Color(RTF.ElementID.Text));
                            hasOvercap = true;
                        }

                        str4 = str5 + str6;
                    }

                    str1 = str3 + str4 + RTF.Crlf() + RTF.Crlf();
                }
            }

            var str7 = hasOvercap
                ? RTF.Color(RTF.ElementID.Invention) + RTF.Underline(RTF.Bold("Information:")) + RTF.Crlf() +
                  RTF.Color(RTF.ElementID.Text) +
                  "One or more set bonuses have exceeded the 5 bonus cap, and will not affect your stats. Scroll down this list to find bonuses marked as '" +
                  RTF.Italic(RTF.Color(RTF.ElementID.Warning) + ">Cap") + RTF.Color(RTF.ElementID.Text) + "'" +
                  RTF.Crlf() + RTF.Crlf()
                : "";
            var str8 = RTF.StartRTF() + str7 + str1 + RTF.EndRTF();
            if (rtxtFX.Rtf != str8) rtxtFX.Rtf = str8;
            var iStr = "";
            if (!getDetails)
            {
                var cumulativeSetBonuses = MidsContext.Character.CurrentBuild.GetCumulativeSetBonuses().ToArray();
                Array.Sort(cumulativeSetBonuses);

                foreach (var csb in cumulativeSetBonuses)
                {
                    if (iStr != "") iStr += RTF.Crlf();
                    var str2 = csb.BuildEffectString(true);
                    if (!str2.StartsWith("+"))
                        str2 = "+" + str2;
                    if (str2.IndexOf("Endurance", StringComparison.Ordinal) > -1)
                        str2 = str2.Replace("Endurance", "Max Endurance");
                    iStr += str2;
                }
            }
            else
            {
                //var effectSources = GetEffectSources();
                //var setBonusesDetail = MidsContext.Character.CurrentBuild.GetCumulativeSetBonusesDetail();
                //foreach (var csb in setBonusesDetail)
                var effectSources = GetEffectSources2();
                foreach (var fxGroup in effectSources)
                {
                    if (fxGroup.Key.effectType == Enums.eEffectType.GrantPower |
                        fxGroup.Key.effectType == Enums.eEffectType.Null |
                        fxGroup.Key.effectType == Enums.eEffectType.NullBool) continue;
                    if (iStr != "") iStr += RTF.Crlf();
                    var fxBlockStr = "";
                    fxBlockStr += fxGroup.Key.effectType.ToString();
                    fxBlockStr += fxGroup.Key.mezType != Enums.eMez.None ? $" ({fxGroup.Key.mezType})" : "";
                    fxBlockStr += fxGroup.Key.damageType != Enums.eDamage.None ? $" ({fxGroup.Key.damageType})" : "";
                    fxBlockStr += fxGroup.Key.targetEffectType != Enums.eEffectType.None ? $" ({fxGroup.Key.targetEffectType})" : "";

                    foreach (var e in effectSources[fxGroup.Key])
                    {
                        fxBlockStr += RTF.Crlf();
                        var effectString = e.fx.BuildEffectString(true);
                        if (!effectString.StartsWith("+"))
                        {
                            effectString = "+" + effectString;
                        }

                        if (effectString.IndexOf("Endurance", StringComparison.Ordinal) > -1)
                        {
                            effectString = effectString.Replace("Endurance", "Max Endurance");
                        }

                        fxBlockStr += $"    {(e.pvMode == Enums.ePvX.PvP ? "[PVP] " : "")}{effectString}";
                        if (e.enhSet != "" & e.power != "" & !e.isFromEnh)
                        {
                            fxBlockStr += $" ({e.enhSet}, on {e.power})";
                        }
                        else if (e.isFromEnh)
                        {
                            fxBlockStr += $" (Enh. special, on {e.power})";
                        }
                    }

                    iStr += fxBlockStr;
                }
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
            foreach (var sb in MidsContext.Character.CurrentBuild.SetBonus)
            {
                for (var index2 = 0; index2 < sb.SetInfo.Length; index2++)
                {
                    if (sb.SetInfo[index2].SetIDX <= -1) continue;
                    var enhancementSet = DatabaseAPI.Database.EnhancementSets[sb.SetInfo[index2].SetIDX];
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
            if (lstSets.SelectedItems.Count < 1) return;
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
            {
                rectangle.X = myParent.Left + 8;
            }

            if (rectangle.Y < 32)
            {
                rectangle.Y = myParent.Top + (myParent.Height - myParent.ClientSize.Height) +
                              myParent.GetPrimaryBottom();
            }

            if (MidsContext.Config.ShrinkFrmSets & (Width > 600) |
                !MidsContext.Config.ShrinkFrmSets & (Width < 600))
            {
                btnSmall_Click();
            }

            Top = rectangle.Y;
            Left = rectangle.X;
        }

        private void StoreLocation()
        {
            if (!MainModule.MidsController.IsAppInitialized) return;
            MainModule.MidsController.SzFrmSets.X = Left;
            MainModule.MidsController.SzFrmSets.Y = Top;
            MidsContext.Config.ShrinkFrmSets = Width < 600;
        }

        public void UpdateData()
        {
            if (myParent == null) return;

            BackColor = myParent.BackColor;
            if (rtApplied.BackColor != BackColor)
            {
                rtApplied.BackColor = BackColor;
            }

            if (rtxtFX.BackColor != BackColor)
            {
                rtxtFX.BackColor = BackColor;
            }

            if (rtxtInfo.BackColor != BackColor)
            {
                rtxtInfo.BackColor = BackColor;
            }

            var imageOffIdx = MidsContext.Character.IsHero() ? 2 : 4;
            var imageOnIdx = imageOffIdx + 1;

            btnClose.IA = myParent.Drawing.pImageAttributes;
            btnClose.ImageOff = myParent.Drawing.bxPower[imageOffIdx].Bitmap;
            btnClose.ImageOn = myParent.Drawing.bxPower[imageOnIdx].Bitmap;

            chkOnTop.IA = myParent.Drawing.pImageAttributes;
            chkOnTop.ImageOff = myParent.Drawing.bxPower[imageOffIdx].Bitmap;
            chkOnTop.ImageOn = myParent.Drawing.bxPower[imageOnIdx].Bitmap;

            btnSmall.IA = myParent.Drawing.pImageAttributes;
            btnSmall.ImageOff = myParent.Drawing.bxPower[imageOffIdx].Bitmap;
            btnSmall.ImageOn = myParent.Drawing.bxPower[imageOnIdx].Bitmap;

            btnDetailFx.IA = myParent.Drawing.pImageAttributes;
            btnDetailFx.ImageOff = myParent.Drawing.bxPower[imageOffIdx].Bitmap;
            btnDetailFx.ImageOn = myParent.Drawing.bxPower[imageOnIdx].Bitmap;

            DisplayList();
        }

        private void btnDetailFx_ButtonClicked()
        {
            FillEffectView(btnDetailFx.Checked);
        }
    }
}