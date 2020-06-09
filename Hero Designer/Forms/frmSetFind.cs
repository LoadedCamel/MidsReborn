
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Base.Data_Classes;
using Base.Display;
using Base.Master_Classes;
using midsControls;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmSetFind : Form
    {
        int[] setBonusList;

        ImageButton ibClose;
        ImageButton ibTopmost;
        ctlPopUp SetInfo;
        readonly frmMain myParent;

        public frmSetFind(frmMain iParent)
        {
            FormClosed += frmSetFind_FormClosed;
            Load += frmSetFind_Load;
            setBonusList = new int[0];
            InitializeComponent();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmSetFind));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            Name = nameof(frmSetFind);
            ibClose.ButtonClicked += ibClose_ButtonClicked;
            ibTopmost.ButtonClicked += ibTopmost_ButtonClicked;
            myParent = iParent;
        }

        void AddEffect(ref string[] List, ref int[] nIDList, string Effect, int nID)

        {
            int num = List.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (string.Equals(List[index], Effect, StringComparison.OrdinalIgnoreCase))
                    return;
            }
            List = (string[])Utils.CopyArray(List, new string[List.Length + 1]);
            nIDList = (int[])Utils.CopyArray(nIDList, new int[List.Length + 1]);
            List[List.Length - 1] = Effect;
            nIDList[List.Length - 1] = nID;
        }

        void AddSetString(int nIDSet, int BonusID)

        {
            lvSet.Items.Add(new ListViewItem(new[]
            {
        DatabaseAPI.Database.EnhancementSets[nIDSet].DisplayName,
        Convert.ToString(DatabaseAPI.Database.EnhancementSets[nIDSet].LevelMin + 1) + " - " + Convert.ToString(DatabaseAPI.Database.EnhancementSets[nIDSet].LevelMax + 1),
        DatabaseAPI.Database.SetTypeStringLong[(int) DatabaseAPI.Database.EnhancementSets[nIDSet].SetType],
        BonusID >= 0 ? Convert.ToString(DatabaseAPI.Database.EnhancementSets.GetSetBonusEnhCount(nIDSet, BonusID)) : "Special"
            }, nIDSet));
            lvSet.Items[lvSet.Items.Count - 1].Tag = nIDSet;
        }

        private void FillEffectList()
        {
            string[] List = Array.Empty<string>();
            int[] nIDList = Array.Empty<int>();
            lvBonus.BeginUpdate();
            lvBonus.Items.Clear();
            int num1 = setBonusList.Length - 1;
            for (int index = 0; index <= num1; ++index)
            {
                if ((DatabaseAPI.Database.Power[setBonusList[index]].EntitiesAutoHit & Enums.eEntity.Caster) > Enums.eEntity.None)
                    AddEffect(ref List, ref nIDList, GetPowerString(setBonusList[index]), -1);
            }
            int num2 = List.Length - 1;
            for (int index = 0; index <= num2; ++index)
                lvBonus.Items.Add(new ListViewItem(List[index]));
            lvBonus.Sorting = SortOrder.Ascending;
            lvBonus.Sort();
            if (lvBonus.Items.Count > 0)
                lvBonus.Items[0].Selected = true;
            lvBonus.EndUpdate();
        }

        private void FillImageList()
        {
            Size imageSize1 = ilSets.ImageSize;
            int width1 = imageSize1.Width;
            imageSize1 = ilSets.ImageSize;
            int height1 = imageSize1.Height;
            using ExtendedBitmap extendedBitmap = new ExtendedBitmap(width1, height1);
            ilSets.Images.Clear();
            int num = DatabaseAPI.Database.EnhancementSets.Count - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (DatabaseAPI.Database.EnhancementSets[index].ImageIdx > -1)
                {
                    extendedBitmap.Graphics.Clear(Color.White);
                    Graphics graphics = extendedBitmap.Graphics;
                    I9Gfx.DrawEnhancementSet(ref graphics, DatabaseAPI.Database.EnhancementSets[index].ImageIdx);
                    ilSets.Images.Add(extendedBitmap.Bitmap);
                }
                else
                {
                    ImageList.ImageCollection images = ilSets.Images;
                    Size imageSize2 = ilSets.ImageSize;
                    int width2 = imageSize2.Width;
                    imageSize2 = ilSets.ImageSize;
                    int height2 = imageSize2.Height;
                    Bitmap bitmap = new Bitmap(width2, height2);
                    images.Add(bitmap);
                }
            }
        }

        private void FillMagList()
        {
            if (lvBonus.SelectedItems.Count < 1)
            {
                lvMag.Items.Clear();
            }
            else
            {
                string[] List = Array.Empty<string>();
                int[] nIDList = Array.Empty<int>();
                string text = lvBonus.SelectedItems[0].Text;
                int num1 = setBonusList.Length - 1;
                for (int index = 0; index <= num1; ++index)
                {
                    if (DatabaseAPI.Database.Power[setBonusList[index]].Effects.Length <= 0)
                        continue;
                    string powerString = GetPowerString(setBonusList[index]);
                    if (text != powerString)
                        continue;
                    string Effect = (DatabaseAPI.Database.Power[setBonusList[index]].Effects[0].EffectType != Enums.eEffectType.HitPoints ? (DatabaseAPI.Database.Power[setBonusList[index]].Effects[0].EffectType != Enums.eEffectType.Endurance ? Strings.Format(DatabaseAPI.Database.Power[setBonusList[index]].Effects[0].MagPercent, "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00") : Strings.Format(DatabaseAPI.Database.Power[setBonusList[index]].Effects[0].Mag, "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00")) : Strings.Format((float)(DatabaseAPI.Database.Power[setBonusList[index]].Effects[0].Mag / (double)MidsContext.Archetype.Hitpoints * 100.0), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00")) + "%";
                    AddEffect(ref List, ref nIDList, Effect, setBonusList[index]);
                }
                lvMag.BeginUpdate();
                lvMag.Items.Clear();
                lvMag.Items.Add("All");
                int num2 = List.Length - 1;
                for (int index = 0; index <= num2; ++index)
                    lvMag.Items.Add(new ListViewItem(List[index])
                    {
                        Tag = nIDList[index]
                    });
                if (lvMag.Items.Count > 0)
                    lvMag.Items[0].Selected = true;
                lvMag.EndUpdate();
            }
        }

        void FillSetList()

        {
            if (lvBonus.SelectedItems.Count < 1 | lvMag.SelectedItems.Count < 1)
            {
                lvSet.Items.Clear();
            }
            else
            {
                lvSet.BeginUpdate();
                lvSet.Items.Clear();
                string[] List = new string[0];
                int[] nIDList = new int[0];
                string text = lvBonus.SelectedItems[0].Text;
                bool flag = lvMag.Items[0].Selected;
                if (!flag)
                {
                    if (Conversion.Val(RuntimeHelpers.GetObjectValue(lvMag.SelectedItems[0].Tag)) > -1.0)
                        AddEffect(ref List, ref nIDList, DatabaseAPI.Database.Power[Convert.ToInt32(lvMag.SelectedItems[0].Tag)].PowerName, Convert.ToInt32(lvMag.SelectedItems[0].Tag));
                }
                else
                {
                    int num = setBonusList.Length - 1;
                    for (int index = 0; index <= num; ++index)
                    {
                        if (DatabaseAPI.Database.Power[setBonusList[index]].Effects.Length <= 0)
                            continue;
                        string powerString = GetPowerString(setBonusList[index]);
                        if (text == powerString)
                            AddEffect(ref List, ref nIDList, DatabaseAPI.Database.Power[setBonusList[index]].PowerName, setBonusList[index]);
                    }
                }
                int num1 = DatabaseAPI.Database.EnhancementSets.Count - 1;
                for (int nIDSet = 0; nIDSet <= num1; ++nIDSet)
                {
                    int num2 = DatabaseAPI.Database.EnhancementSets[nIDSet].Bonus.Length - 1;
                    for (int BonusID = 0; BonusID <= num2; ++BonusID)
                    {
                        int num3 = DatabaseAPI.Database.EnhancementSets[nIDSet].Bonus[BonusID].Index.Length - 1;
                        for (int index1 = 0; index1 <= num3; ++index1)
                        {
                            int num4 = nIDList.Length - 1;
                            for (int index2 = 0; index2 <= num4; ++index2)
                            {
                                if (DatabaseAPI.Database.EnhancementSets[nIDSet].Bonus[BonusID].Index[index1] == nIDList[index2])
                                    AddSetString(nIDSet, BonusID);
                            }
                        }
                    }
                    int num5 = DatabaseAPI.Database.EnhancementSets[nIDSet].SpecialBonus.Length - 1;
                    for (int BonusID = 0; BonusID <= num5; ++BonusID)
                    {
                        int num3 = DatabaseAPI.Database.EnhancementSets[nIDSet].SpecialBonus[BonusID].Index.Length - 1;
                        for (int index1 = 0; index1 <= num3; ++index1)
                        {
                            int num4 = nIDList.Length - 1;
                            for (int index2 = 0; index2 <= num4; ++index2)
                            {
                                if (DatabaseAPI.Database.EnhancementSets[nIDSet].SpecialBonus[BonusID].Index[index1] == nIDList[index2])
                                    AddSetString(nIDSet, BonusID);
                            }
                        }
                    }
                }
                if (lvSet.Items.Count > 0)
                    lvSet.Items[0].Selected = true;
                lvSet.EndUpdate();
            }
        }

        void frmSetFind_FormClosed(object sender, FormClosedEventArgs e)

        {
            myParent.FloatSetFinder(false);
        }

        void frmSetFind_Load(object sender, EventArgs e)

        {
            setBonusList = DatabaseAPI.NidPowers("Set_Bonus.Set_Bonus");
            BackColor = myParent.BackColor;
            ibClose.IA = myParent.Drawing.pImageAttributes;
            ibClose.ImageOff = myParent.Drawing.bxPower[2].Bitmap;
            ibClose.ImageOn = myParent.Drawing.bxPower[3].Bitmap;
            ibTopmost.IA = myParent.Drawing.pImageAttributes;
            ibTopmost.ImageOff = myParent.Drawing.bxPower[2].Bitmap;
            ibTopmost.ImageOn = myParent.Drawing.bxPower[3].Bitmap;
            SetInfo.SetPopup(new PopUp.PopupData());
            FillImageList();
            FillEffectList();
        }

        string GetPowerString(int nIDPower)

        {
            string str1 = "";
            string returnString = "";
            int[] returnMask = new int[0];
            DatabaseAPI.Database.Power[nIDPower].GetEffectStringGrouped(0, ref returnString, ref returnMask, true, true, true);
            string str2;
            if (returnString != "")
            {
                str2 = returnString;
            }
            else
            {
                int num1 = DatabaseAPI.Database.Power[nIDPower].Effects.Length - 1;
                for (int index1 = 0; index1 <= num1; ++index1)
                {
                    bool flag = false;
                    int num2 = returnMask.Length - 1;
                    for (int index2 = 0; index2 <= num2; ++index2)
                    {
                        if (index1 == returnMask[index2])
                            flag = true;
                    }

                    if (flag)
                        continue;
                    if (str1 != "")
                        str1 += ", ";
                    string str3 = Strings.Trim(DatabaseAPI.Database.Power[nIDPower].Effects[index1].BuildEffectString(true, "", true));
                    if (str3.Contains("Res("))
                        str3 = str3.Replace("Res(", "Resistance(");
                    if (str3.Contains("Def("))
                        str3 = str3.Replace("Def(", "Defense(");
                    if (str3.Contains("EndRec"))
                        str3 = str3.Replace("EndRec", "Recovery");
                    if (str3.Contains("Endurance"))
                        str3 = str3.Replace("Endurance", "Max End");
                    else if (str3.Contains("End") & !str3.Contains("Max End"))
                        str3 = str3.Replace("End", "Max End");
                    str1 += str3;
                }
                str2 = str1;
            }
            return str2;
        }

        void ibClose_ButtonClicked()

        {
            Close();
        }

        void ibTopmost_ButtonClicked()

        {
            TopMost = ibTopmost.Checked;
            if (!TopMost)
                return;
            BringToFront();
        }

        [DebuggerStepThrough]

        void lvBonus_SelectedIndexChanged(object sender, EventArgs e)

        {
            FillMagList();
        }

        void lvMag_SelectedIndexChanged(object sender, EventArgs e)

        {
            FillSetList();
        }

        void lvSet_SelectedIndexChanged(object sender, EventArgs e)

        {
            if (lvSet.SelectedItems.Count <= 0)
                return;
            SetInfo.SetPopup(Character.PopSetInfo(Convert.ToInt32(lvSet.SelectedItems[0].Tag)));
        }
    }
}