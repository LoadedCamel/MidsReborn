using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastDeepCloner;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Forms.Controls
{
    public partial class DataView : UserControl
    {
        /*internal ctlDamageDisplay Info_Damage
        {
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            private set;
        }

        internal override RichTextBox Info_txtLarge
        {
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            private set;
        }*/

        public delegate void FloatChangeEventHandler();
        public delegate void MovedEventHandler();
        public delegate void SizeChangeEventHandler(Size newSize, bool isCompact);
        public delegate void SlotFlipEventHandler(int PowerIndex);
        public delegate void SlotUpdateEventHandler();
        public delegate void TabChangedEventHandler(int Index);
        public delegate void Unlock_ClickEventHandler();
        public delegate void EntityDetailsEventHandler(string entityUid, HashSet<string> powers, int basePowerHistoryIdx, PetInfo petInfo);

        private const int SnapDistance = 10;

        protected const int szDataList = 104;
        protected const int szLargeText = 100;
        protected const int szLineHeight = -10;
        protected const int szPadding = 4;
        private readonly string[] Pages;

        private bool bFloating;
        private ExtendedBitmap bxFlip;
        private bool Compact;
        private int HistoryIDX;
        private bool Lock;
        private Point mouse_offset;
        public bool MoveDisable;
        private IPower? pBase;
        private IPower? pEnh;
        private int pLastScaleVal;
        private Rectangle ScreenBounds;
        public Rectangle SnapLocation;
        public int TabPage;
        private bool VillainColor;
        public bool[]? TabsMask;

        public PetInfo PetInfo;

        public DataView()
        {
            MoveDisable = false;
            TabPage = 0;
            Pages = new[]
            {
                "INFO", "EFFECTS", "TOTALS", "ENHANCE"
            };
            pLastScaleVal = -1;
            Lock = false;
            bFloating = false;
            HistoryIDX = -1;
            Compact = false;
            bxFlip = null;
            BackColorChanged += DataView_BackColorChanged;
            Load += DataView_Load;
            Name = nameof(DataView);
            PetInfo = new PetInfo();
            InitializeComponent();
        }

        public bool DrawVillain
        {
            get => VillainColor;
            set
            {
                VillainColor = value;
                if (MidsContext.Config.DisableVillainColors) VillainColor = false;

                pnlInfo.BackColor = VillainColor
                    ? MidsContext.Config.RtFont.ColorPowerTakenDarkVillain
                    : MidsContext.Config.RtFont.ColorPowerTakenDarkHero;
                DoPaint();
            }
        }

        public bool Floating
        {
            get => bFloating;
            set
            {
                bFloating = value;
                if (bFloating)
                {
                    dbTip.SetToolTip(lblFloat, "Dock Info Display");
                    lblFloat.Text = "D";
                }
                else
                {
                    dbTip.SetToolTip(lblFloat, "Make Floating Window");
                    lblFloat.Text = "F";
                }
            }
        }

        public bool IsDocked => (SnapLocation.X == Location.X) & (SnapLocation.Y == Location.Y);

        public int TabPageIndex => TabPage;

        public Enums.eVisibleSize VisibleSize
        {
            get => !Compact ? Enums.eVisibleSize.Full : Enums.eVisibleSize.Compact;
            // why is this ignored?
            set
            {
                if (!Enum.IsDefined(typeof(Enums.eVisibleSize), value))
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Enums.eVisibleSize));
            }
        }

        public event FloatChangeEventHandler FloatChange;
        public event MovedEventHandler Moved;
        public event SizeChangeEventHandler SizeChange;
        public event SlotFlipEventHandler SlotFlip;
        public event SlotUpdateEventHandler SlotUpdate;
        public event TabChangedEventHandler TabChanged;
        public event Unlock_ClickEventHandler Unlock_Click;
        public event EntityDetailsEventHandler EntityDetails;

        private static PairedList.ItemPair BuildEDItem(int index, float[] value, Enums.eSchedule[] schedule, string Name, float[] afterED)
        {
            var flag1 = value[index] > (double)DatabaseAPI.Database.MultED[(int)schedule[index]][0];
            var flag2 = value[index] > (double)DatabaseAPI.Database.MultED[(int)schedule[index]][1];
            var iSpecialCase = value[index] > (double)DatabaseAPI.Database.MultED[(int)schedule[index]][2];
            PairedList.ItemPair itemPair;
            if (value[index] <= 0.0)
            {
                itemPair = new PairedList.ItemPair(string.Empty, string.Empty, false, false, false, string.Empty);
            }
            else
            {
                var iName = Name + ":";
                var num1 = value[index] * 100f;
                var num2 = Enhancement.ApplyED(schedule[index], value[index]) * 100f;
                var num3 = num2 + afterED[index] * 100f;
                var num4 = (float)Math.Round(num1 - (double)num2, 3);
                var str1 = num1.ToString("##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00") + "%";
                var str2 = num4.ToString("##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00") + "%";
                var str3 = num3.ToString("##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00") + "%";
                var str4 = $"Total Effect: {Convert.ToDecimal(num1 + afterED[index] * 100.0):0.##}%\r\nWith ED Applied: {str3}\r\n\r\n";
                //var str4 = "Total Effect: " + Strings.Format((float) (num1 + afterED[index] * 100.0), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00") + "%\r\nWith ED Applied: " + str3 + "\r\n\r\n";
                string iValue;
                string iTip;
                if (num4 > 0.0)
                {
                    iValue = $"{str3} (Pre-ED: {Convert.ToDecimal(num1 + afterED[index] * 100.0):0.##}%)";
                    if (afterED[index] > 0.0)
                        str4 = str4 + "Amount from pre-ED sources: " + str1 + "\r\n";
                    iTip = $"{str4} ED reduction: {str2} ({Convert.ToDecimal(num4 / (double)num1 * 100.0):0.##}% of total)\r\n";
                    //iTip = str4 + "ED reduction: " + str2 + " (" + Strings.Format((float) (num4 / (double) num1 * 100.0), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00") + "% of total)\r\n";
                    if (iSpecialCase)
                        iTip = $"{iTip} The highest level of ED reduction is being applied.\r\nThreshold: {Convert.ToDecimal(DatabaseAPI.Database.MultED[(int)schedule[index]][2] * 100.0):0.##} %\r\n";
                    //iTip = iTip + "The highest level of ED reduction is being applied.\r\nThreshold: " + Strings.Format((float) (DatabaseAPI.Database.MultED[(int) schedule[index]][2] * 100.0), "##0") + "%\r\n";
                    else if (flag2)
                        iTip = $"{iTip} The middle level of ED reduction is being applied.\r\nThreshold: {Convert.ToDecimal(DatabaseAPI.Database.MultED[(int)schedule[index]][1] * 100.0):0.##} %\r\n";
                    //iTip = iTip + "The middle level of ED reduction is being applied.\r\nThreshold: " + Strings.Format((float) (DatabaseAPI.Database.MultED[(int) schedule[index]][1] * 100.0), "##0") + "%\r\n";
                    else if (flag1)
                        iTip = $"{iTip} The lowest level of ED reduction is being applied.\r\nThreshold: {Convert.ToDecimal(DatabaseAPI.Database.MultED[(int)schedule[index]][0] * 100.0):0.##} %\r\n";
                    //iTip = iTip + "The lowest level of ED reduction is being applied.\r\nThreshold: " + Strings.Format((float) (DatabaseAPI.Database.MultED[(int) schedule[index]][0] * 100.0), "##0") + "%\r\n";
                    if (afterED[index] > 0.0)
                        iTip = $"{iTip} Amount from post-ED sources: {Convert.ToDecimal(afterED[index] * 100.0):0.##} %\r\n";
                    //iTip = iTip + "Amount from post-ED sources: " + Strings.Format((float) (afterED[index] * 100.0), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00") + "%\r\n";
                }
                else
                {
                    iValue = str3;
                    if (afterED[index] > 0.0)
                        str4 = $"{str4} Amount from post-ED sources: {Convert.ToDecimal(afterED[index] * 100.0):0.##} %\r\n";
                    iTip = str4 + "This effect has not been affected by ED.\r\n";
                }

                itemPair = new PairedList.ItemPair(iName, iValue, flag2 & !iSpecialCase, flag1 & !flag2,
                    iSpecialCase, iTip);
            }

            return itemPair;
        }

        private static string CapString(string iString, int capLength)
        {
            return iString.Length >= capLength ? iString[..capLength] : iString;
        }

        public void Clear()
        {
            info_DataList.Clear(true);
            info_Title.Text = Pages[0];
            Info_txtLarge.Text = string.Empty;
            info_txtSmall.Text = "Hold the mouse over a power to see its description.";
            PowerScaler.Visible = false;
            fx_lblHead1.Text = string.Empty;
            fx_lblHead2.Text = string.Empty;
            fx_LblHead3.Text = string.Empty;
            fx_List1.Clear(true);
            fx_List2.Clear(true);
            fx_List3.Clear(true);
            fx_Title.Text = Pages[1];
            enhListing.Clear(true);
            Enh_Title.Text = "Enhancements";
            total_Misc.Clear(true);
        }

        private void CompactSize()
        {
            var size = Size;
            info_txtSmall.Height = 16;
            Info_txtLarge.Top = info_txtSmall.Bottom + 4;
            if (PowerScaler.Visible)
            {
                Info_txtLarge.Height = 48 - PowerScaler.Height;
                PowerScaler.Top = Info_txtLarge.Bottom;
                info_DataList.Top = PowerScaler.Bottom + 4;
            }
            else
            {
                Info_txtLarge.Height = 48;
                PowerScaler.Top = Info_txtLarge.Bottom - PowerScaler.Height;
                info_DataList.Top = Info_txtLarge.Bottom + 4;
            }

            info_DataList.Height = 76;
            lblDmg.Visible = true;
            lblDmg.Top = info_DataList.Bottom + 4;
            Info_Damage.Top = lblDmg.Bottom + 4;
            Info_Damage.PaddingV = 1;
            Info_Damage.Height = 30;
            enhListing.Height = Info_Damage.Bottom - (enhListing.Top + (pnlEnhActive.Height + 4) * 2);
            pnlEnhActive.Top = enhListing.Bottom + 4;
            pnlEnhInactive.Top = pnlEnhActive.Bottom + 4;
            pnlInfo.Height = Info_Damage.Bottom + 4;
            pnlEnh.Height = pnlInfo.Height;
            Height = pnlInfo.Bottom;
            Compact = true;
            if (!(Size != size))
                return;
            SizeChange?.Invoke(Size, Compact);
        }

        private void DataView_BackColorChanged(object sender, EventArgs e)
        {
            SetBackColor();
        }

        private void DataView_Load(object sender, EventArgs e)
        {
            pnlInfo.Top = 20;
            pnlInfo.Left = 0;
            pnlFX.Top = 20;
            pnlFX.Left = 0;
            pnlTotal.Top = 20;
            pnlTotal.Left = 0;
            pnlEnh.Top = 20;
            pnlEnh.Left = 0;
            Info_Damage.nBaseVal = 0;
            Info_Damage.nEnhVal = 0;
            Info_Damage.nHighBase = 0;
            Info_Damage.nHighEnh = 0;
            Info_Damage.nMaxEnhVal = 0;
            Info_txtLarge.Height = 100;
            Floating = bFloating;
            info_DataList.LinePadding = 1;
            fx_List1.LinePadding = 1;
            fx_List2.LinePadding = 2;
            fx_List3.LinePadding = 3;

            var useToxicDefOffSet = DatabaseAPI.RealmUsesToxicDef() ? 15 : 0;
            total_lblRes.Location = new Point(4, 135 + useToxicDefOffSet);
            gRes1.Location = new Point(4, 151 + useToxicDefOffSet);
            gRes2.Location = new Point(150, 151 + useToxicDefOffSet);
            total_lblMisc.Location = new Point(4, 227 + useToxicDefOffSet);
            total_Misc.Location = new Point(4, 243 + useToxicDefOffSet);
            lblTotal.Location = new Point(3, 323 + useToxicDefOffSet);

            TabsMask ??= new[] {true, true, true, true};

            Clear();
        }

        private void Display_EDFigures()
        {
            if (pBase == null)
                return;
            Enh_Title.Text = pBase.DisplayName;
            enhListing.Clear();
            if (MidsContext.Character == null)
            {
                enhListing.Draw();
            }
            else
            {
                var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(pBase.PowerIndex);
                if (inToonHistory < 0)
                {
                    enhListing.Draw();
                }
                else
                {
                    var eEnhance = Enums.eEnhance.None;
                    var eMez = Enums.eMez.None;
                    var eEnhs = Enum.GetValues(eEnhance.GetType()).Length;
                    var numArray1 = new float[eEnhs];
                    var numArray2 = new float[eEnhs];
                    var numArray3 = new float[eEnhs];
                    var schedule1 = new Enums.eSchedule[eEnhs];
                    var schedule2 = new Enums.eSchedule[eEnhs];
                    var schedule3 = new Enums.eSchedule[eEnhs];
                    var afterED1 = new float[eEnhs];
                    var afterED2 = new float[eEnhs];
                    var afterED3 = new float[eEnhs];
                    var numArray4 = new float[Enum.GetValues(eMez.GetType()).Length];
                    var schedule4 = new Enums.eSchedule[eEnhs];
                    var afterED4 = new float[eEnhs];
                    for (var index = 0; index <= numArray1.Length - 1; ++index)
                    {
                        numArray1[index] = 0.0f;
                        numArray2[index] = 0.0f;
                        numArray3[index] = 0.0f;
                        schedule1[index] = Enhancement.GetSchedule((Enums.eEnhance)index);
                        schedule2[index] = schedule1[index];
                        schedule3[index] = schedule1[index];
                    }

                    schedule2[3] = Enums.eSchedule.A;
                    var num2 = numArray4.Length - 1;
                    for (var tSub = 0; tSub <= num2; ++tSub)
                    {
                        numArray4[tSub] = 0.0f;
                        schedule4[tSub] = Enhancement.GetSchedule(Enums.eEnhance.Mez, tSub);
                    }

                    var num3 = MidsContext.Character.CurrentBuild.Powers[inToonHistory].SlotCount - 1;
                    for (var index1 = 0; index1 <= num3; ++index1)
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index1].Enhancement.Enh <= -1) continue;

                        var num4 = DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index1].Enhancement.Enh].Effect.Length - 1;
                        for (var index2 = 0; index2 <= num4; ++index2)
                        {
                            var effect = DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index1].Enhancement.Enh].Effect;
                            var index3 = index2;
                            if (effect[index3].Mode != Enums.eEffMode.Enhancement)
                                continue;
                            if (effect[index3].Enhance.ID == 12)
                            {
                                numArray4[effect[index3].Enhance.SubID] += MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index1].Enhancement.GetEnhancementEffect(Enums.eEnhance.Mez, effect[index3].Enhance.SubID, 1f);
                            }
                            else
                            {
                                switch (DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index1].Enhancement.Enh].Effect[index2].BuffMode)
                                {
                                    case Enums.eBuffDebuff.BuffOnly:
                                        numArray1[effect[index3].Enhance.ID] += MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index1].Enhancement.GetEnhancementEffect((Enums.eEnhance)effect[index3].Enhance.ID, -1, 1f);
                                        break;
                                    case Enums.eBuffDebuff.DeBuffOnly:
                                        if ((effect[index3].Enhance.ID != 6) & (effect[index3].Enhance.ID != 19) & (effect[index3].Enhance.ID != 11))
                                        {
                                            numArray2[effect[index3].Enhance.ID] += MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index1].Enhancement.GetEnhancementEffect((Enums.eEnhance)effect[index3].Enhance.ID, -1, -1f);
                                        }

                                        break;
                                    default:
                                        numArray3[effect[index3].Enhance.ID] += MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index1].Enhancement.GetEnhancementEffect((Enums.eEnhance)effect[index3].Enhance.ID, -1, 1f);
                                        break;
                                }
                            }
                        }
                    }

                    var num5 = MidsContext.Character.CurrentBuild.Powers.Count - 1;
                    for (var index1 = 0; index1 <= num5; ++index1)
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[index1] == null) continue;
                        if (!MidsContext.Character.CurrentBuild.Powers[index1].StatInclude)
                            continue;
                        IPower power1 = new Power(MidsContext.Character.CurrentBuild.Powers[index1].Power);
                        power1.AbsorbPetEffects();
                        power1.ApplyGrantPowerEffects();
                        foreach (var effect in power1.Effects)
                        {
                            if ((power1.PowerType != Enums.ePowerType.GlobalBoost) & (!effect.Absorbed_Effect | (effect.Absorbed_PowerType != Enums.ePowerType.GlobalBoost)))
                                continue;
                            var power2 = power1;
                            if (effect.Absorbed_Effect & (effect.Absorbed_Power_nID > -1))
                            {
                                power2 = DatabaseAPI.Database.Power[effect.Absorbed_Power_nID];
                            }

                            var eBuffDebuff = Enums.eBuffDebuff.Any;
                            var flag = false;
                            if (MidsContext.Character.CurrentBuild.Powers[inToonHistory] == null) continue;
                            foreach (var str1 in MidsContext.Character.CurrentBuild.Powers[inToonHistory].Power.BoostsAllowed)
                            {
                                if (power2 != null && power2.BoostsAllowed.Any(str2 => str1 == str2))
                                {
                                    if (str1.Contains("Buff"))
                                        eBuffDebuff = Enums.eBuffDebuff.BuffOnly;
                                    if (str1.Contains("Debuff"))
                                        eBuffDebuff = Enums.eBuffDebuff.DeBuffOnly;
                                    flag = true;
                                }

                                if (flag)
                                    break;
                            }

                            if (!flag)
                                continue;
                            if (effect.EffectType == Enums.eEffectType.Enhancement)
                            {
                                switch (effect.ETModifies)
                                {
                                    case Enums.eEffectType.Defense:
                                        if (effect.DamageType == Enums.eDamage.Smashing)
                                        {
                                            if (effect.IgnoreED)
                                                switch (eBuffDebuff)
                                                {
                                                    case Enums.eBuffDebuff.BuffOnly:
                                                        afterED1[3] += effect.BuffedMag;
                                                        break;
                                                    case Enums.eBuffDebuff.DeBuffOnly:
                                                        afterED2[3] += effect.BuffedMag;
                                                        break;
                                                    default:
                                                        afterED3[3] += effect.BuffedMag;
                                                        break;
                                                }
                                            else
                                                switch (eBuffDebuff)
                                                {
                                                    case Enums.eBuffDebuff.BuffOnly:
                                                        numArray1[3] += effect.BuffedMag;
                                                        break;
                                                    case Enums.eBuffDebuff.DeBuffOnly:
                                                        numArray2[3] += effect.BuffedMag;
                                                        break;
                                                    default:
                                                        numArray3[3] += effect.BuffedMag;
                                                        break;
                                                }
                                        }

                                        break;
                                    case Enums.eEffectType.Mez:
                                        if (effect.IgnoreED)
                                        {
                                            afterED4[(int)effect.MezType] += effect.BuffedMag;
                                            break;
                                        }

                                        numArray4[(int)effect.MezType] += effect.BuffedMag;
                                        break;
                                    default:
                                        var index2 = effect.ETModifies != Enums.eEffectType.RechargeTime ? Convert.ToInt32(Enum.Parse(typeof(Enums.eEnhance), effect.ETModifies.ToString())) : 14;
                                        if (effect.IgnoreED)
                                        {
                                            afterED3[index2] += effect.BuffedMag;
                                            break;
                                        }

                                        numArray3[index2] += effect.BuffedMag;
                                        break;
                                }
                            }
                            else if ((effect.EffectType == Enums.eEffectType.DamageBuff) & (effect.DamageType == Enums.eDamage.Smashing))
                            {
                                switch (effect.IgnoreED)
                                {
                                    case true:
                                    {
                                        foreach (var str in power2?.BoostsAllowed)
                                        {
                                            if (str.StartsWith("Res_Damage"))
                                            {
                                                afterED3[18] += effect.BuffedMag;
                                                break;
                                            }

                                            if (!str.StartsWith("Damage"))
                                            {
                                                continue;
                                            }

                                            afterED3[2] += effect.BuffedMag;
                                            break;
                                        }

                                        break;
                                    }
                                    default:
                                    {
                                        foreach (var str in power2?.BoostsAllowed)
                                        {
                                            if (str.StartsWith("Res_Damage"))
                                            {
                                                numArray3[18] += effect.BuffedMag;
                                                break;
                                            }

                                            if (!str.StartsWith("Damage"))
                                            {
                                                continue;
                                            }

                                            numArray3[2] += effect.BuffedMag;
                                            break;
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                    }

                    numArray1[8] = 0.0f;
                    numArray2[8] = 0.0f;
                    numArray3[8] = 0.0f;
                    numArray1[17] = 0.0f;
                    numArray2[17] = 0.0f;
                    numArray3[17] = 0.0f;
                    numArray1[16] = 0.0f;
                    numArray2[16] = 0.0f;
                    numArray3[16] = 0.0f;
                    var num6 = numArray1.Length - 1;
                    for (var index = 0; index <= num6; ++index)
                    {
                        if (numArray1[index] > 0.0)
                        {
                            enhListing.AddItem(BuildEDItem(index, numArray1, schedule1, Enum.GetName(eEnhance.GetType(), index), afterED1));
                            if (enhListing.IsSpecialColor())
                                enhListing.SetUnique();
                        }

                        if (numArray2[index] > 0.0)
                        {
                            enhListing.AddItem(BuildEDItem(index, numArray2, schedule2,
                                Enum.GetName(eEnhance.GetType(), index) + " Debuff",
                                afterED2));
                            if (enhListing.IsSpecialColor())
                                enhListing.SetUnique();
                        }

                        if (!(numArray3[index] > 0.0))
                            continue;
                        enhListing.AddItem(BuildEDItem(index, numArray3, schedule3,
                            Enum.GetName(eEnhance.GetType(), index), afterED3));
                        if (enhListing.IsSpecialColor())
                            enhListing.SetUnique();
                    }

                    var num7 = numArray4.Length - 1;
                    for (var index = 0; index <= num7; ++index)
                    {
                        if (!(numArray4[index] > 0.0))
                            continue;
                        enhListing.AddItem(BuildEDItem(index, numArray4, schedule4, Enum.GetName(eMez.GetType(), index),
                            afterED4));
                        if (enhListing.IsSpecialColor())
                            enhListing.SetUnique();
                    }

                    enhListing.Draw();
                    DisplayFlippedEnhancements();
                }
            }
        }

        private void DisplayInfo(bool noLevel = false, int iEnhLvl = -1)
        {
            if (pBase == null)
            {
                return;
            }

            var defianceFound = false;
            var enhancedPower = pEnh.PowerIndex == -1 ? pBase : pEnh;

            info_Title.Text = !noLevel & pBase.Level > 0
                ? $"[{pBase.Level}] {pBase.DisplayName}"
                : pBase.DisplayName;

            if (iEnhLvl > -1)
            {
                var infoTitle = info_Title;
                infoTitle.Text = $"{infoTitle.Text} (Slot Level {iEnhLvl + 1})";
            }

            enhNameDisp.Text = "Enhancement Values";
            var longInfo = Regex.Replace(pBase.DescLong.Trim().Replace("\0", "").Replace("<br>", RTF.Crlf()), @"\s{2,}", " ");
            info_txtSmall.Rtf = RTF.StartRTF() + RTF.ToRTF(pBase.DescShort.Trim()) + RTF.EndRTF();
            Info_txtLarge.Rtf = RTF.StartRTF() + RTF.ToRTF(longInfo) + RTF.EndRTF();
            var suffix1 = pBase.PowerType != Enums.ePowerType.Toggle ? "" : "/s";
            
            info_DataList.Clear();
            var tip1 = string.Empty;
            if (pBase.PowerType == Enums.ePowerType.Click)
            {
                if (enhancedPower.ToggleCost > 0 & enhancedPower.RechargeTime + enhancedPower.CastTime + enhancedPower.InterruptTime > 0)
                {
                    tip1 = $"Effective end drain per second: {Utilities.FixDP(enhancedPower.ToggleCost / (enhancedPower.RechargeTime + enhancedPower.CastTime + enhancedPower.InterruptTime))}/s";
                }

                if (enhancedPower.ToggleCost > 0 &
                    MidsContext.Config?.DamageMath.ReturnValue == ConfigData.EDamageReturn.Numeric)
                {
                    var damageValue = enhancedPower.FXGetDamageValue(pEnh == null);
                    if (damageValue > 0)
                    {
                        if (!string.IsNullOrEmpty(tip1))
                        {
                            tip1 += "\r\n";
                        }

                        tip1 = $"{tip1}Effective damage per unit of end: {Utilities.FixDP(damageValue / enhancedPower.ToggleCost)}";
                    }
                }
            }

            /*foreach (var effect in pBase.Effects)
            {
                effect.UpdateAttrib();
                SetDamageTip();
            }*/
            info_DataList.AddItem(FastItem(ShortStr("End Cost", "End"), pBase.ToggleCost, enhancedPower.ToggleCost, suffix1, tip1));
            var flag1 = pBase.HasAbsorbedEffects && pBase.PowerIndex > -1 && DatabaseAPI.Database.Power[pBase.PowerIndex].EntitiesAutoHit == Enums.eEntity.None;
            var flag2 = pBase.Effects.Any(t => t.RequiresToHitCheck);

            if (pBase.EntitiesAutoHit == Enums.eEntity.None | flag2 | flag1 | pBase.Range > 20 & pBase.I9FXPresentP(Enums.eEffectType.Mez, Enums.eMez.Taunt))
            {
                var accuracy1 = pBase.Accuracy;
                var accuracy2 = enhancedPower.Accuracy;
                var num2 = MidsContext.Config.ScalingToHit * pBase.Accuracy;
                var str = string.Empty;
                var suffix2 = "%";
                if (pBase.EntitiesAutoHit != Enums.eEntity.None & flag2)
                {
                    str = "\r\n* This power is autohit, but has an effect that requires a ToHit roll.";
                    suffix2 += "*";
                }

                if (Math.Abs(accuracy1 - accuracy2) > float.Epsilon & Math.Abs(num2 - accuracy2) > float.Epsilon)
                {
                    var tip2 = $"Accuracy multiplier without other buffs (Real Numbers style): {pBase.Accuracy + (enhancedPower.Accuracy - MidsContext.Config.ScalingToHit):##0.00000}x{str}";
                    info_DataList.AddItem(FastItem(ShortStr("Accuracy", "Acc"),
                        MidsContext.Config.ScalingToHit * pBase.Accuracy * 100, enhancedPower.Accuracy * 100, suffix2, tip2));
                }
                else
                {
                    var tip2 = $"Accuracy multiplier without other buffs (Real Numbers style): {pBase.AccuracyMult:##0.00}x{str}";
                    info_DataList.AddItem(FastItem(ShortStr("Accuracy", "Acc"),
                        MidsContext.Config.ScalingToHit * pBase.Accuracy * 100,
                        MidsContext.Config.ScalingToHit * pBase.Accuracy * 100, suffix2, tip2));
                }
            }
            else
            {
                info_DataList.AddItem(new PairedList.ItemPair(string.Empty, string.Empty, false, false, false, string.Empty));
            }

            info_DataList.AddItem(FastItem(ShortStr("Recharge", "Rchg"), pBase.RechargeTime, enhancedPower.RechargeTime, "s"));
            var s1 = 0f;
            var s2 = 0f;
            var durationEffectId = pBase.GetDurationEffectID();
            if (durationEffectId > -1)
            {
                if (pBase.Effects[durationEffectId].EffectType == Enums.eEffectType.EntCreate &
                    pBase.Effects[durationEffectId].Duration >= 9999)
                {
                    s1 = 0f;
                    s2 = 0f;
                }
                else
                {
                    s1 = pBase.Effects[durationEffectId].Duration;
                    s2 = enhancedPower.Effects[durationEffectId].Duration;
                }
            }

            info_DataList.AddItem(FastItem(ShortStr("Duration", "Durtn"), s1, s2, "s"));
            info_DataList.AddItem(FastItem(ShortStr("Range", "Range"), pBase.Range, enhancedPower.Range, "ft"));
            info_DataList.AddItem(pBase.Arc > 0
                ? FastItem("Arc", pBase.Arc, enhancedPower.Arc, "°")
                : FastItem("Radius", pBase.Radius, enhancedPower.Radius, "ft"));
            info_DataList.AddItem(FastItem(ShortStr("Cast Time", "Cast"), enhancedPower.CastTime, pBase.CastTime, "s", $"CastTime: {pBase.CastTime}s\r\nArcana CastTime: {(Math.Ceiling(enhancedPower.CastTime / 0.132f) + 1) * 0.132:####0.###}s", false, true, false, false, 3));
            info_DataList.AddItem(pBase.PowerType == Enums.ePowerType.Toggle
                ? FastItem(ShortStr("Activate", "Act"), pBase.ActivatePeriod, enhancedPower.ActivatePeriod, "s", "The effects of this toggle power are applied at this interval.")
                : FastItem(ShortStr("Interrupt", "Intrpt"), enhancedPower.InterruptTime, pBase.InterruptTime, "s", "After activating this power, it can be interrupted for this amount of time."));
            if (durationEffectId > -1 &&
                pBase.Effects[durationEffectId].EffectType == Enums.eEffectType.Mez &
                pBase.Effects[durationEffectId].MezType != Enums.eMez.Taunt &
                !((pBase.Effects[durationEffectId].MezType == Enums.eMez.Knockback |
                   pBase.Effects[durationEffectId].MezType == Enums.eMez.Knockup) &
                  pBase.Effects[durationEffectId].Mag < 0))
            {
                info_DataList.AddItem(new PairedList.ItemPair("Effect:",
                    Enum.GetName(Enums.eMez.None.GetType(), pBase.Effects[durationEffectId].MezType), false,
                    pBase.Effects[durationEffectId].Probability < 1,
                    pBase.Effects[durationEffectId].CanInclude(),
                    durationEffectId));

                info_DataList.AddItem(new PairedList.ItemPair("Mag:",
                    $"{enhancedPower.Effects[durationEffectId].BuffedMag:####0.##}",
                    Math.Abs(pBase.Effects[durationEffectId].BuffedMag - enhancedPower.Effects[durationEffectId].BuffedMag) > float.Epsilon,
                    pBase.Effects[durationEffectId].Probability < 1));
            }

            var rankedEffects = pEnh.GetRankedEffects(true);
            var defiancePower = DatabaseAPI.GetPowerByFullName("Inherent.Inherent.Defiance");
            var groupedRankedEffects = new List<GroupedFx>();
            var ignoredEffects = new List<int>();
            for (var id = 0; id < rankedEffects.Length; id++)
            {
                if (rankedEffects[id] <= -1)
                {
                    continue;
                }

                if (ignoredEffects.Contains(rankedEffects[id]))
                {
                    continue;
                }

                if (pEnh.Effects[rankedEffects[id]].EffectType is Enums.eEffectType.Mez or Enums.eEffectType.Null
                    or Enums.eEffectType.NullBool or Enums.eEffectType.ModifyAttrib or Enums.eEffectType.MaxRunSpeed
                    or Enums.eEffectType.MaxFlySpeed or Enums.eEffectType.MaxJumpSpeed or Enums.eEffectType.Damage
                    or Enums.eEffectType.SetMode or Enums.eEffectType.Null or Enums.eEffectType.NullBool
                    or Enums.eEffectType.GlobalChanceMod or Enums.eEffectType.ExecutePower)
                {
                    continue;
                }

                if (pEnh.Effects[rankedEffects[id]].EffectType == Enums.eEffectType.ResEffect &
                    pEnh.Effects[rankedEffects[id]].ETModifies is Enums.eEffectType.Null or Enums.eEffectType.NullBool)
                {
                    continue;
                }

                //var rankedEffect = GetRankedEffect(rankedEffects, id);

                if (!(pEnh.Effects[rankedEffects[id]].Probability > 0 &
                      (MidsContext.Config?.Suppression & pEnh.Effects[rankedEffects[id]].Suppression) ==
                      Enums.eSuppress.None & pEnh.Effects[rankedEffects[id]].CanInclude()))
                {
                    continue;
                }

                if (pEnh.Effects[rankedEffects[id]].EffectType == Enums.eEffectType.RevokePower &&
                    pEnh.Effects[rankedEffects[id]].nSummon <= -1 &&
                    string.IsNullOrWhiteSpace(pEnh.Effects[rankedEffects[id]].Summon))
                {
                    continue;
                }

                if (pEnh.Effects[rankedEffects[id]].EffectType == Enums.eEffectType.GrantPower &&
                    pEnh.Effects[rankedEffects[id]].nSummon <= -1)
                {
                    continue;
                }

                if (pEnh.Effects[rankedEffects[id]].PvMode == Enums.ePvX.PvP & !MidsContext.Config.Inc.DisablePvE |
                    pEnh.Effects[rankedEffects[id]].PvMode == Enums.ePvX.PvE & MidsContext.Config.Inc.DisablePvE)
                {
                    continue;
                }

                var similarFxIds = new List<int>();

                switch (pEnh.Effects[rankedEffects[id]].EffectType)
                {
                    case Enums.eEffectType.DamageBuff:
                        var isDefiance = pEnh.Effects[rankedEffects[id]].SpecialCase == Enums.eSpecialCase.Defiance &&
                                         pEnh.Effects[rankedEffects[id]].ValidateConditional("Active", "Defiance") |
                                         MidsContext.Character.CurrentBuild.PowerActive(defiancePower);


                        similarFxIds = GroupedFx.GetSimilarEffects(pEnh,
                            new fxIdentifier
                            {
                                DamageType = Enums.eDamage.None,
                                EffectType = Enums.eEffectType.DamageBuff,
                                ETModifies = Enums.eEffectType.None,
                                MezType = Enums.eMez.None,
                                ToWho = pEnh.Effects[rankedEffects[id]].ToWho
                            }, pEnh.Effects[rankedEffects[id]].BuffedMag,
                            isDefiance ? Enums.eSpecialCase.Defiance : Enums.eSpecialCase.None);

                        ignoredEffects.AddRangeUnique(similarFxIds);

                        groupedRankedEffects.Add(
                            new GroupedFx(new fxIdentifier
                                {
                                    DamageType = Enums.eDamage.None,
                                    EffectType = Enums.eEffectType.DamageBuff,
                                    ETModifies = Enums.eEffectType.None,
                                    MezType = Enums.eMez.None,
                                    ToWho = pEnh.Effects[rankedEffects[id]].ToWho
                                },
                                pEnh.Effects[rankedEffects[id]].BuffedMag,
                                isDefiance ? "Defiance" : $"{pEnh.Effects[rankedEffects[id]].EffectType}",
                                similarFxIds,
                                !isDefiance && pEnh.Effects[rankedEffects[id]].isEnhancementEffect,
                                Enums.eSpecialCase.Defiance));

                        break;

                    case Enums.eEffectType.Defense:
                    case Enums.eEffectType.Resistance:
                    case Enums.eEffectType.Elusivity:
                    case Enums.eEffectType.MezResist:
                    case Enums.eEffectType.ResEffect:
                    case Enums.eEffectType.Enhancement:
                        similarFxIds = GroupedFx.GetSimilarEffects(pEnh,
                            new fxIdentifier
                            {
                                EffectType = pEnh.Effects[rankedEffects[id]].EffectType,
                                ETModifies = pEnh.Effects[rankedEffects[id]].ETModifies,
                                MezType = Enums.eMez.None,
                                DamageType = Enums.eDamage.None,
                                ToWho = pEnh.Effects[rankedEffects[id]].ToWho
                            },
                            pEnh.Effects[rankedEffects[id]].BuffedMag,
                            Enums.eSpecialCase.None,
                            pEnh.Effects[rankedEffects[id]].isEnhancementEffect);
                        
                        ignoredEffects.AddRangeUnique(similarFxIds);

                        groupedRankedEffects.Add(
                            new GroupedFx(new fxIdentifier
                                {
                                    EffectType = pEnh.Effects[rankedEffects[id]].EffectType,
                                    ETModifies = pEnh.Effects[rankedEffects[id]].ETModifies,
                                    MezType = Enums.eMez.None,
                                    DamageType = Enums.eDamage.None,
                                    ToWho = pEnh.Effects[rankedEffects[id]].ToWho
                                },
                                pEnh.Effects[rankedEffects[id]].BuffedMag,
                                pEnh.Effects[rankedEffects[id]].EffectType == Enums.eEffectType.Enhancement
                                    ? $"{pEnh.Effects[rankedEffects[id]].EffectType}({pEnh.Effects[rankedEffects[id]].ETModifies})"
                                    : $"{pEnh.Effects[rankedEffects[id]].EffectType}",
                                similarFxIds,
                                pEnh.Effects[rankedEffects[id]].isEnhancementEffect
                            ));

                        break;

                    default:
                        groupedRankedEffects.Add(new GroupedFx(pEnh.Effects[rankedEffects[id]], rankedEffects[id]));
                        break;
                }
            }

            foreach (var gre in groupedRankedEffects)
            {
                var greIndex = gre.GetRankedEffectIndex(rankedEffects, 0);
                if (greIndex < 0) continue;

                var rankedEffect = GetRankedEffect(rankedEffects, greIndex);
                var effectType = gre.EffectType;
                var effectSource = gre.GetEffectAt(pEnh);
                var tooltip = gre.GetTooltip(pEnh);
                //var statName = gre.GetStatName(pEnh);

                switch (effectType)
                {
                    case Enums.eEffectType.Recovery:
                    case Enums.eEffectType.Endurance:
                        rankedEffect.Name = $"{effectType}";
                        var fxTarget = effectSource.ToWho switch
                        {
                            Enums.eToWho.Self => "(Self)",
                            Enums.eToWho.Target => "(Tgt)",
                            _ => ""
                        };

                        rankedEffect.Value = effectSource.DisplayPercentage ? $"{effectSource.BuffedMag * 100:###0.##}% {fxTarget}" : $"{effectSource.BuffedMag:###0.##} {fxTarget}";

                        if (effectSource.EffectType == Enums.eEffectType.Endurance)
                        {
                            rankedEffect.SpecialTip = effectSource.BuildEffectString(false, "", false, false, false, false, false, true);
                        }

                        break;

                    case Enums.eEffectType.EntCreate when !pEnh.AbsorbSummonEffects | !pEnh.AbsorbSummonAttributes:
                        rankedEffect.Name = "Summon";
                        rankedEffect.Value = effectSource.nSummon > -1
                            ? DatabaseAPI.Database.Entities[effectSource.nSummon].DisplayName
                            : Regex.Replace(effectSource.Summon, @"^(MastermindPets|Pets|Villain_Pets)_", string.Empty);

                        break;
                    case Enums.eEffectType.GrantPower:
                        rankedEffect.Name = "Grant";
                        if (effectSource.nSummon > -1)
                        {
                            rankedEffect.Value = DatabaseAPI.Database.Power[effectSource.nSummon].DisplayName;
                            var mainEffectTip = effectSource.BuildEffectString(false, "", false, false, false, false, false, true);
                            var subEffectsTip = string.Join("\r\n",
                                DatabaseAPI.Database.Power[effectSource.nSummon].Effects
                                    .Where(e => (e.PvMode == Enums.ePvX.Any ||
                                                e.PvMode == Enums.ePvX.PvE & !MidsContext.Config.Inc.DisablePvE ||
                                                e.PvMode == Enums.ePvX.PvP & MidsContext.Config.Inc.DisablePvE) & (e.ActiveConditionals.Count <= 0 || e.ValidateConditional()))
                                    .Select(e => e.BuildEffectString(false, "", false, false, false, false, false, true)
                                            .Replace("\r\n", "\n").Replace("\n", " -- ").Replace("  ", " ")));
                            rankedEffect.SpecialTip = $"{mainEffectTip}\r\n----------\r\n{subEffectsTip}";
                        }

                        break;

                    case Enums.eEffectType.CombatModShift:
                        rankedEffect.Name = "LvlShift";
                        rankedEffect.Value = $"{(effectSource.Mag > 0 ? "+" : "")}{effectSource.Mag:##0.##}";

                        break;

                    case Enums.eEffectType.RevokePower:
                        rankedEffect.Name = "Revoke";
                        rankedEffect.Value = effectSource.nSummon > -1
                            ? DatabaseAPI.Database.Entities[effectSource.nSummon].DisplayName
                            : Regex.Replace(effectSource.Summon, @"^(MastermindPets|Pets|Villain_Pets)_", string.Empty);

                        break;

                    case Enums.eEffectType.DamageBuff:
                        var isDefiance = effectSource.SpecialCase == Enums.eSpecialCase.Defiance &&
                                         effectSource.ValidateConditional("Active", "Defiance") |
                                         MidsContext.Character.CurrentBuild.PowerActive(defiancePower);
                        rankedEffect.Name = isDefiance
                            ? "Defiance"
                            : ShortStr(Enums.GetEffectName(effectSource.EffectType),
                                Enums.GetEffectNameShort(effectSource.EffectType));
                        rankedEffect.SpecialTip = isDefiance
                            ? effectSource.BuildEffectString(false, "DamageBuff (Defiance)", false, false, false, true)
                            : tooltip; // pEnh.BuildTooltipStringAllVectorsEffects(effectSource.EffectType);

                        break;

                    case Enums.eEffectType.Resistance:
                    case Enums.eEffectType.Defense:
                    case Enums.eEffectType.Elusivity:
                    case Enums.eEffectType.MezResist:
                    case Enums.eEffectType.Enhancement:
                    case Enums.eEffectType.ResEffect:
                        rankedEffect.Name = effectType == Enums.eEffectType.Enhancement & gre.ToWho == Enums.eToWho.Target & effectSource.Mag < 0
                                ? "Debuff"
                                : ShortStr(Enums.GetEffectName(effectSource.EffectType), Enums.GetEffectNameShort(effectSource.EffectType));
                        rankedEffect.SpecialTip = tooltip;

                        break;

                    case Enums.eEffectType.PerceptionRadius:
                        rankedEffect.Name = $"Pceptn({effectSource.ToWho})";
                        rankedEffect.Value = $"{(effectSource.DisplayPercentage ? $"{effectSource.BuffedMag * 100:###0.##}%" : $"{effectSource.BuffedMag:###0.##}")} ({Statistics.BasePerception * effectSource.BuffedMag:###0.##}ft)";

                        break;

                    default:
                        var configDisablePvE = MidsContext.Config != null && MidsContext.Config.Inc.DisablePvE;
                        var magSumEnh = pEnh.Effects
                            .Where(e => (configDisablePvE & e.PvMode == Enums.ePvX.PvP |
                                         !configDisablePvE & e.PvMode == Enums.ePvX.PvE |
                                         e.PvMode == Enums.ePvX.Any) &
                                        effectSource.ToWho == e.ToWho &
                                        effectSource.EffectType == e.EffectType &
                                        effectSource.MezType == e.MezType &
                                        effectSource.ETModifies == e.ETModifies)
                            .Select(e => e.BuffedMag * (e.DisplayPercentage ? 100 : 1))
                            .Sum();

                        var magSumBase = pBase.Effects
                            .Where(e => (configDisablePvE & e.PvMode == Enums.ePvX.PvP |
                                         !configDisablePvE & e.PvMode == Enums.ePvX.PvE |
                                         e.PvMode == Enums.ePvX.Any) &
                                        effectSource.ToWho == e.ToWho &
                                        effectSource.EffectType == e.EffectType &
                                        effectSource.MezType == e.MezType &
                                        effectSource.ETModifies == e.ETModifies)
                            .Select(e => e.BuffedMag * (e.DisplayPercentage ? 100 : 1))
                            .Sum();

                        rankedEffect.Value = $"{magSumEnh:####0.##}{(effectSource.DisplayPercentage ? "%" : "")}";
                        rankedEffect.Value += effectSource.ToWho switch
                        {
                            Enums.eToWho.Self => " (Self)",
                            Enums.eToWho.Target => " (Tgt)",
                            _ => ""
                        };

                        rankedEffect.AlternateColor = Math.Abs(magSumEnh - magSumBase) > float.Epsilon;
                        rankedEffect.Name = ShortStr(Enums.GetEffectName(effectSource.EffectType), Enums.GetEffectNameShort(effectSource.EffectType));
                        rankedEffect.SpecialTip = string.Join("\r\n", pEnh.Effects
                            .Where(e => (configDisablePvE & e.PvMode == Enums.ePvX.PvP |
                                         !configDisablePvE & e.PvMode == Enums.ePvX.PvE |
                                         e.PvMode == Enums.ePvX.Any) &
                                        Math.Abs(e.BuffedMag) > float.Epsilon &
                                        effectSource.ToWho == e.ToWho &
                                        effectSource.EffectType == e.EffectType &
                                        effectSource.MezType == e.MezType &
                                        effectSource.ETModifies == e.ETModifies)
                            .Select(e => e.BuildEffectString(false, "", false, false, false, true)));

                        break;
                }

                info_DataList.AddItem(rankedEffect);
                if (effectSource.isEnhancementEffect)
                {
                    info_DataList.SetUnique();
                }
            }

            info_DataList.Draw();
            var str1 = "Damage";
            switch (MidsContext.Config.DamageMath.ReturnValue)
            {
                case ConfigData.EDamageReturn.DPS:
                    str1 += " Per Second";
                    break;
                case ConfigData.EDamageReturn.DPA:
                    str1 += " Per Animation Second";
                    break;
            }

            if (MidsContext.Config.DataDamageGraphPercentageOnly)
            {
                str1 += " (% only)";
            }
            
            var baseDamage = Math.Abs(pBase.FXGetDamageValue(pBase.PowerIndex > -1 & pEnh.PowerIndex > -1));
            var enhancedDamage = pEnh.PowerIndex == -1
                ? baseDamage
                : Math.Abs(enhancedPower.FXGetDamageValue());

            if (pBase.NIDSubPower.Length > 0 & baseDamage == 0 && enhancedDamage == 0)
            {
                lblDmg.Text = string.Empty;
                Info_Damage.nBaseVal = 0;
                Info_Damage.nEnhVal = 0;
                Info_Damage.nMaxEnhVal = 0;
                Info_Damage.nHighEnh = 0;
                Info_Damage.Text = string.Empty;
            }
            else
            {
                lblDmg.Text = $@"{str1}:";
                //Info_Damage.nBaseVal = damageValue1;
                //Info_Damage.nMaxEnhVal = baseDamage * (1f + Enhancement.ApplyED(Enums.eSchedule.A, 2.277f));
                //Info_Damage.nEnhVal = damageValue2;

                // Mids has no awareness of target data.
                // When using damage %, estimate damage value based on character HP.
                var hasPercentDamage = pEnh.Effects
                    .Any(e => e.EffectType == Enums.eEffectType.Damage && e.DisplayPercentage | e.Aspect == Enums.eAspect.Str);
                var dmgMultiplier = hasPercentDamage ? MidsContext.Character.Totals.HPMax : 1;
                
                Info_Damage.nBaseVal = Math.Max(0, baseDamage * dmgMultiplier); // Negative damage ? (see Toxins)
                Info_Damage.nEnhVal = Math.Max(0, enhancedDamage * dmgMultiplier);
                Info_Damage.nMaxEnhVal = Math.Max(baseDamage * dmgMultiplier * (1 + Enhancement.ApplyED(Enums.eSchedule.A, 2.277f)), enhancedDamage * dmgMultiplier);
                Info_Damage.nHighEnh = Math.Max(414, enhancedDamage * dmgMultiplier); // Maximum graph value
                Info_Damage.Text = Math.Abs(enhancedDamage - baseDamage) > float.Epsilon
                    ? $"{enhancedPower.FXGetDamageString(pEnh.PowerIndex == -1)} ({(hasPercentDamage ? $"{Utilities.FixDP(baseDamage * 100)}%" : Utilities.FixDP(baseDamage))})"
                    : pBase.FXGetDamageString(pBase.PowerIndex > -1 & pEnh.PowerIndex > -1);
            }

            SetPowerScaler();
        }

        private void DisplayData(bool noLevel = false, int iEnhLevel = -1)
        {
            if (!MidsContext.Config.DisableDataDamageGraph)
            {
                Info_Damage.GraphType = MidsContext.Config.DataGraphType;
                Info_Damage.TextAlign = Enums.eDDAlign.Center;
                Info_Damage.Style = Enums.eDDStyle.TextUnderGraph;
            }
            else
            {
                Info_Damage.TextAlign = Enums.eDDAlign.Center;
                Info_Damage.Style = Enums.eDDStyle.Text;
            }

            if (pBase != null && pEnh != null)
            {
                // Ensure pEnh has at least as many effects as pBase
                if (pBase.Effects.Length > pEnh.Effects.Length)
                {
                    var swappedFx = SwapExtraEffects(pBase.Effects, pEnh.Effects);
                    pBase.Effects = (IEffect[])swappedFx[0].Clone();
                    pEnh.Effects = (IEffect[])swappedFx[1].Clone();
                }
            }

            lblLock.Visible = Lock & (TabPage != 2);
            DisplayInfo(noLevel, iEnhLevel);
            DisplayEffects(noLevel, iEnhLevel);
            Display_EDFigures();
        }

        private void DisplayEffects(bool noLevel = false, int iEnhLvl = -1)
        {
            if (pBase == null)
            {
                return;
            }

            fx_Title.Text = !noLevel & (pBase.Level > 0)
                ? $"[{pBase.Level}] {pBase.DisplayName}"
                : pBase.DisplayName;

            if (iEnhLvl > -1)
            {
                var fxTitle = fx_Title;
                fxTitle.Text = $"{fxTitle.Text} (Slot Level {iEnhLvl + 1})";
            }

            PairedList[] PairedListArray =
            {
                fx_List1, fx_List2, fx_List3
            };

            Label[] labelArray =
            {
                fx_lblHead1, fx_lblHead2, fx_LblHead3
            };

            fx_List1.Clear();
            fx_List2.Clear();
            fx_List3.Clear();
            fx_lblHead1.Text = string.Empty;
            fx_lblHead2.Text = string.Empty;
            fx_LblHead3.Text = string.Empty;

            var index = EffectsDrh();
            var num1 = 0;
            var hasStatusFx = false;
            var hasHealFx = false;
            if (index < PairedListArray.Length)
            {
                PairedListArray[index].Clear();
                num1 = EffectsHeal(labelArray[index], PairedListArray[index]);
                if (num1 > 0)
                {
                    hasHealFx = true;
                    index++;
                    if (index < PairedListArray.Length)
                    {
                        PairedListArray[index].Clear();
                    }
                }
            }

            if (index < PairedListArray.Length)
            {
                num1 = EffectsStatus(labelArray[index], PairedListArray[index]);
                if (num1 > 0)
                {
                    hasStatusFx = true;
                }

                if (num1 > 2 | num1 > 0 & index == 0)
                {
                    index++;
                    if (index < PairedListArray.Length)
                    {
                        PairedListArray[index].Clear();
                    }
                }
            }

            if (!hasStatusFx & hasHealFx & num1 < 3)
            {
                --index;
            }

            if (index < PairedListArray.Length &&
                EffectsBuffDebuff(labelArray[index], PairedListArray[index]) > 0 &
                PairedListArray[index].ItemCount > 2 & index + 1 < PairedListArray.Length)
            {
                index++;
                if (index < PairedListArray.Length)
                {
                    PairedListArray[index].Clear();
                }
            }

            if (index < PairedListArray.Length)
            {
                index += EffectsMovement(labelArray[index], PairedListArray[index]);
            }

            if (index < PairedListArray.Length)
            {
                index += EffectsSummon(labelArray[index], PairedListArray[index]);
            }

            if (index < PairedListArray.Length)
            {
                index += EffectsGrantPower(labelArray[index], PairedListArray[index]);
            }

            if (index < PairedListArray.Length)
            {
                index += EffectsModifyEffect(labelArray[index], PairedListArray[index]);
            }

            if (index < PairedListArray.Length)
            {
                index += EffectsElusivity(labelArray[index], PairedListArray[index]);
            }

            if (fx_lblHead1.Text != string.Empty)
            {
                fx_lblHead1.Text += ":";
            }

            if (fx_lblHead2.Text != string.Empty)
            {
                fx_lblHead2.Text += ":";
            }

            if (fx_LblHead3.Text != string.Empty)
            {
                fx_LblHead3.Text += ":";
            }

            fx_List1.Draw();
            fx_List2.Draw();
            fx_List3.Draw();
        }

        private void DisplayFlippedEnhancements()
        {
            var pen = enhListing.BackColor.B <= 10
                ? new Pen(Color.FromArgb(byte.MaxValue, 0, 0))
                : new Pen(Color.FromArgb(0, 0, byte.MaxValue));
            bxFlip ??= new ExtendedBitmap(pnlEnhActive.Width, pnlEnhInactive.Height * 2);
            bxFlip.Graphics.Clear(enhListing.BackColor);
            bxFlip.Graphics.DrawRectangle(pen, 0, 0, pnlEnhActive.Width - 1, pnlEnhInactive.Height - 1);
            bxFlip.Graphics.DrawRectangle(pen, 0, pnlEnhInactive.Height, pnlEnhActive.Width - 1,
                pnlEnhInactive.Height - 1);
            if (pBase == null)
                return;
            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(pBase.PowerIndex);
            if (inToonHistory < 0)
            {
                RedrawFlip();
            }
            else
            {
                using var format = new StringFormat();
                var num1 = bxFlip.Size.Width - 188;
                var rectangle1 = new Rectangle();
                ref var local1 = ref rectangle1;
                var width = num1;
                var size = bxFlip.Size;
                var height = (int)Math.Round(size.Height / 2.0);
                local1 = new Rectangle(-4, 0, width, height);
                using var solidBrush1 = new SolidBrush(enhListing.NameColor);
                format.Alignment = StringAlignment.Far;
                format.LineAlignment = StringAlignment.Center;
                bxFlip.Graphics.DrawString("Active Slotting:", pnlEnhActive.Font, solidBrush1, rectangle1, format);
                rectangle1.Y += rectangle1.Height;
                bxFlip.Graphics.DrawString("Alternate:", pnlEnhActive.Font, solidBrush1, rectangle1, format);
                //ImageAttributes recolorIa = clsDrawX.GetRecolorIa(MidsContext.Character.IsHero());
                using var solidBrush2 = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                var num2 = MidsContext.Character.CurrentBuild.Powers[inToonHistory].SlotCount - 1;
                for (var index = 0; index <= num2; ++index)
                {
                    var iDest = new Rectangle();
                    ref var local2 = ref iDest;
                    var x1 = num1 + 30 * index;
                    size = bxFlip.Size;
                    var y1 = (int)Math.Round((size.Height / 2.0 - 30.0) / 2.0);
                    local2 = new Rectangle(x1, y1, 30, 30);
                    var rectangle2 = new Rectangle();
                    ref var local3 = ref rectangle2;
                    var x2 = num1 + 30 * index;
                    size = bxFlip.Size;
                    var num3 = size.Height / 2.0;
                    size = bxFlip.Size;
                    var num4 = (size.Height / 2.0 - 30.0) / 2.0;
                    var y2 = (int)Math.Round(num3 + num4);
                    local3 = new Rectangle(x2, y2, 30, 30);
                    RectangleF bounds;
                    Rectangle destRect;
                    if (MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index].Enhancement.Enh > -1)
                    {
                        var graphics1 = bxFlip.Graphics;
                        I9Gfx.DrawEnhancementAt(ref graphics1, iDest,
                            DatabaseAPI.Database
                                .Enhancements[
                                    MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index].Enhancement
                                        .Enh].ImageIdx,
                            I9Gfx.ToGfxGrade(
                                DatabaseAPI.Database
                                    .Enhancements[
                                        MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index]
                                            .Enhancement.Enh]
                                    .TypeID,
                                MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index].Enhancement
                                    .Grade));
                        if (MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index].Enhancement.Enh > -1)
                        {
                            if (!MidsContext.Config.I9.HideIOLevels &
                                ((DatabaseAPI.Database
                                     .Enhancements[
                                         MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index]
                                             .Enhancement.Enh]
                                     .TypeID == Enums.eType.SetO) |
                                 (DatabaseAPI.Database
                                     .Enhancements[
                                         MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index]
                                             .Enhancement.Enh]
                                     .TypeID == Enums.eType.InventO)))
                            {
                                bounds = iDest;
                                bounds.Y -= 3f;
                                bounds.Height = DefaultFont.GetHeight(bxFlip.Graphics);
                                var graphics2 = bxFlip.Graphics;
                                clsDrawX.DrawOutlineText(
                                    Convert.ToString(MidsContext.Character.CurrentBuild.Powers[inToonHistory]
                                        .Slots[index].Enhancement.IOLevel + 1), bounds, Color.Cyan,
                                    Color.FromArgb(128, 0, 0, 0), pnlEnhActive.Font, 1f, graphics2);
                            }
                            else if (MidsContext.Config.ShowEnhRel &
                                     ((DatabaseAPI.Database
                                          .Enhancements[
                                              MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index]
                                                  .Enhancement.Enh]
                                          .TypeID == Enums.eType.Normal) |
                                      (DatabaseAPI.Database
                                          .Enhancements[
                                              MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index]
                                                  .Enhancement.Enh]
                                          .TypeID == Enums.eType.SpecialO)))
                            {
                                bounds = iDest;
                                bounds.Y -= 3f;
                                bounds.Height = DefaultFont.GetHeight(bxFlip.Graphics);
                                var text =
                                    MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index].Enhancement
                                        .RelativeLevel !=
                                    Enums.eEnhRelative.None
                                        ? MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index]
                                              .Enhancement.RelativeLevel >=
                                          Enums.eEnhRelative.Even
                                            ? MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index]
                                                .Enhancement
                                                .RelativeLevel <= Enums.eEnhRelative.Even
                                                ? Color.White
                                                : Color.FromArgb(0, byte.MaxValue, byte.MaxValue)
                                            : Color.Yellow
                                        : Color.Red;
                                var graphics2 = bxFlip.Graphics;
                                clsDrawX.DrawOutlineText(
                                    Enums.GetRelativeString(
                                        MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index]
                                            .Enhancement.RelativeLevel,
                                        MidsContext.Config.ShowRelSymbols), bounds, text, Color.FromArgb(128, 0, 0, 0),
                                    pnlEnhActive.Font, 1f,
                                    graphics2);
                            }
                        }
                    }
                    else
                    {
                        destRect = new Rectangle(iDest.X, iDest.Y, 30, 30);
                        bxFlip.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, destRect, 0, 0, 30, 30, GraphicsUnit.Pixel);
                    }

                    if (MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index].FlippedEnhancement.Enh >
                        -1)
                    {
                        var graphics1 = bxFlip.Graphics;
                        I9Gfx.DrawEnhancementAt(ref graphics1, rectangle2,
                            DatabaseAPI.Database
                                .Enhancements[
                                    MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index]
                                        .FlippedEnhancement.Enh]
                                .ImageIdx,
                            I9Gfx.ToGfxGrade(
                                DatabaseAPI.Database
                                    .Enhancements[
                                        MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index]
                                            .FlippedEnhancement.Enh]
                                    .TypeID,
                                MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index].FlippedEnhancement
                                    .Grade));
                        if (MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index].FlippedEnhancement
                            .Enh > -1)
                        {
                            if (!MidsContext.Config.I9.HideIOLevels &
                                ((DatabaseAPI.Database
                                     .Enhancements
                                         [MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index].FlippedEnhancement.Enh]
                                     .TypeID == Enums.eType.SetO) |
                                 (DatabaseAPI.Database
                                     .Enhancements[
                                         MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index]
                                             .FlippedEnhancement.Enh]
                                     .TypeID == Enums.eType.InventO)))
                            {
                                bounds = rectangle2;
                                bounds.Y -= 3f;
                                bounds.Height = DefaultFont.GetHeight(bxFlip.Graphics);
                                var graphics2 = bxFlip.Graphics;
                                clsDrawX.DrawOutlineText(
                                    Convert.ToString(MidsContext.Character.CurrentBuild.Powers[inToonHistory]
                                        .Slots[index].FlippedEnhancement.IOLevel + 1), bounds, Color.Cyan,
                                    Color.FromArgb(128, 0, 0, 0), pnlEnhActive.Font, 1f, graphics2);
                            }
                            else if (MidsContext.Config.ShowEnhRel &
                                     ((DatabaseAPI.Database
                                          .Enhancements
                                              [MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index].FlippedEnhancement.Enh]
                                          .TypeID == Enums.eType.Normal) |
                                      (DatabaseAPI.Database
                                          .Enhancements[
                                              MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index]
                                                  .FlippedEnhancement.Enh]
                                          .TypeID == Enums.eType.SpecialO)))
                            {
                                bounds = rectangle2;
                                bounds.Y -= 3f;
                                bounds.Height = DefaultFont.GetHeight(bxFlip.Graphics);
                                var text =
                                    MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index]
                                        .FlippedEnhancement.RelativeLevel !=
                                    Enums.eEnhRelative.None
                                        ? MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index]
                                            .FlippedEnhancement
                                            .RelativeLevel >= Enums.eEnhRelative.Even
                                            ? MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index]
                                                .FlippedEnhancement
                                                .RelativeLevel <= Enums.eEnhRelative.Even
                                                ? Color.White
                                                : Color.FromArgb(0, byte.MaxValue, byte.MaxValue)
                                            : Color.Yellow
                                        : Color.Red;
                                var graphics2 = bxFlip.Graphics;
                                clsDrawX.DrawOutlineText(
                                    Enums.GetRelativeString(
                                        MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[index]
                                            .FlippedEnhancement.RelativeLevel,
                                        MidsContext.Config.ShowRelSymbols), bounds, text, Color.FromArgb(128, 0, 0, 0),
                                    pnlEnhActive.Font, 1f,
                                    graphics2);
                            }
                        }
                    }
                    else
                    {
                        destRect = new Rectangle(rectangle2.X, rectangle2.Y, 30, 30);
                        bxFlip.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, destRect, 0, 0, 30, 30, GraphicsUnit.Pixel);
                    }

                    rectangle2.Inflate(2, 2);
                    bxFlip.Graphics.FillEllipse(solidBrush2, rectangle2);
                }

                RedrawFlip();
            }
        }

        public void DisplayTotals()
        {
            if (MidsContext.Character == null)
            {
                return;
            }

            var dmgNames = Enum.GetNames(typeof(Enums.eDamage));
            var displayStats = MidsContext.Character.DisplayStats;
            total_Misc.Clear();
            gDef1.Clear();
            gDef2.Clear();
            var numArray1 = new[]
            {
                0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0
            };

            var unusedVectors = new List<Enums.eDamage>
            {
                Enums.eDamage.Special,
                Enums.eDamage.Unique1,
                Enums.eDamage.Unique2,
                Enums.eDamage.Unique3
            }.Cast<int>();
            const int toxicVector = (int) Enums.eDamage.Toxic;

            for (var dType = 1; dType < dmgNames.Length; dType++)
            {
                var iTip = $"{displayStats.Defense(dType):0.##}% {dmgNames[dType]} defense";
                if (dType == toxicVector && !DatabaseAPI.RealmUsesToxicDef())
                {
                    continue;
                }

                if (unusedVectors.Contains(dType))
                {
                    continue;
                }

                var targetGraph = numArray1[dType] == 0 ? gDef1 : gDef2;
                //var targetGraph = dType % 2 == 1 ? gDef1 : gDef2;
                targetGraph.AddItem($"{dmgNames[dType]}:|{displayStats.Defense(dType):0.#}%", displayStats.Defense(dType), 0, iTip);
            }

            var maxValue1 = Math.Max(gDef1.GetMaxValue(), gDef2.GetMaxValue());
            gDef1.Max = maxValue1;
            gDef2.Max = maxValue1;
            gDef1.Draw();
            gDef2.Draw();
            
            var atResCap = $"{MidsContext.Character.Archetype.DisplayName} resistance cap: {MidsContext.Character.Archetype.ResCap * 100:0.##}%";
            gRes1.Clear();
            gRes2.Clear();
            var numArray2 = new[]
            {
                0, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 1
            };

            unusedVectors = new List<Enums.eDamage>
            {
                Enums.eDamage.Melee,
                Enums.eDamage.Ranged,
                Enums.eDamage.AoE,
                Enums.eDamage.Special,
                Enums.eDamage.Unique1,
                Enums.eDamage.Unique2,
                Enums.eDamage.Unique3
            }.Cast<int>();

            for (var dType = 1; dType < dmgNames.Length; dType++)
            {
                if (unusedVectors.Contains(dType))
                {
                    continue;
                }

                var iTip = MidsContext.Character.TotalsCapped.Res[dType] < MidsContext.Character.Totals.Res[dType]
                    ? $"{displayStats.DamageResistance(dType, true):0.##}% {dmgNames[dType]} resistance capped at {displayStats.DamageResistance(dType, false):0.##}%"
                    : $"{displayStats.DamageResistance(dType, true):0.##}% {dmgNames[dType]} resistance. ({atResCap})";

                var targetGraph = numArray2[dType] == 0 ? gRes1 : gRes2;
                targetGraph.AddItem($"{dmgNames[dType]}:|{displayStats.DamageResistance(dType, false):0.#}%", displayStats.DamageResistance(dType, false), displayStats.DamageResistance(dType, true), iTip);
            }

            var maxValue2 = Math.Max(gRes1.GetMaxValue(), gRes2.GetMaxValue());
            gRes1.Max = maxValue2;
            gRes2.Max = maxValue2;
            gRes1.Draw();
            gRes2.Draw();

            var iTip1 = string.Empty;
            var iTip2 = $"Time to go from 0-100% end: {Utilities.FixDP(displayStats.EnduranceTimeToFull)}s.\r\nHover the mouse over the End Drain stats for more info.";
            switch (displayStats.EnduranceRecoveryNet)
            {
                case > 0:
                {
                    iTip1 = $"Net Endurance Gain (Recovery - Drain): {Utilities.FixDP(displayStats.EnduranceRecoveryNet)}/s.";
                    if (Math.Abs(displayStats.EnduranceRecoveryNet - displayStats.EnduranceRecoveryNumeric) > float.Epsilon)
                    {
                        iTip1 += $"\r\nTime to go from 0-100% end (using net gain): {Utilities.FixDP(displayStats.EnduranceTimeToFullNet)}s.";
                    }

                    break;
                }
                case < 0:
                    iTip1 = $"With current end drain, you will lose end at a rate of: {Utilities.FixDP(displayStats.EnduranceRecoveryLossNet)}/s.\r\nFrom 100% you would run out of end in: {Utilities.FixDP(displayStats.EnduranceTimeToZero)}s.";
                    break;
            }

            var iTip3 = $"Time to go from 0-100% health: {Utilities.FixDP(displayStats.HealthRegenTimeToFull)}s.\r\nHealth regenerated per second: {Utilities.FixDP(displayStats.HealthRegenHealthPerSec)}%\r\nHitPoints regenerated per second at level 50: {Utilities.FixDP(displayStats.HealthRegenHPPerSec)} HP";
            total_Misc.AddItem(new PairedList.ItemPair("Recovery:", $"{displayStats.EnduranceRecoveryPercentage(false):0.#}% ({displayStats.EnduranceRecoveryNumeric:0.#}/s)", false, false, false, iTip2));
            total_Misc.AddItem(new PairedList.ItemPair("Regen:", $"{displayStats.HealthRegenPercent(false):0.#}%", false, false, false, iTip3));
            total_Misc.AddItem(new PairedList.ItemPair("EndDrain:", $"{displayStats.EnduranceUsage:0.#}/s", false, false, false, iTip1));
            total_Misc.AddItem(new PairedList.ItemPair("+ToHit:", $"{displayStats.BuffToHit:0.#}%", false, false, false, "This effect is increasing the accuracy of all your powers."));
            total_Misc.AddItem(new PairedList.ItemPair("+EndRdx:", $"{displayStats.BuffEndRdx:0.#}%", false, false, false, "The end cost of all your powers is being reduced by this effect.\r\nThis is applied like an end-reduction enhancement."));
            total_Misc.AddItem(new PairedList.ItemPair("+Recharge:", $"{displayStats.BuffHaste(false) - 100.0:0.#}%", false, false, false, "The recharge time of your powers is being altered by this effect.\r\nThe higher the value, the faster the recharge."));
            total_Misc.Draw();
        }

        /*protected override void Dispose(bool disposing)
        {
            if (disposing)
                components?.Dispose();
            try
            {
                bxFlip?.Dispose();
            }
            catch
            {
                // ignored
            }

            base.Dispose(disposing);
        }*/

        private void DoPaint()
        {
            if (pnlTabs.IsDisposed) return;

            var graphics = pnlTabs.CreateGraphics();
            var pen = new Pen(Color.Black);
            var font1 = new Font("Segoe UI", 9.25f, FontStyle.Regular);//
            var font2 = new Font("Segoe UI", 9.25f, FontStyle.Bold);
            var format = new StringFormat(StringFormatFlags.NoWrap);
            var solidBrush1 = new SolidBrush(Color.White);
            var solidBrush2 = new SolidBrush(BackColor);
            var solidBrush3 = new SolidBrush(Color.Black);
            var extendedBitmap = new ExtendedBitmap(pnlTabs.Size);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            var rect = new Rectangle(0, 0, 75, pnlTabs.Height);
            extendedBitmap.Graphics.FillRectangle(solidBrush2, extendedBitmap.ClipRect);
            switch (TabPage)
            {
                case 0:
                    pnlInfo.Visible = true;
                    pnlFX.Visible = false;
                    pnlTotal.Visible = false;
                    pnlEnh.Visible = false;
                    lblLock.Visible = Lock;
                    solidBrush3 = new SolidBrush(pnlInfo.BackColor);
                    pen = new Pen(pnlInfo.BackColor, 1f);
                    break;
                case 1:
                    pnlInfo.Visible = false;
                    pnlFX.Visible = true;
                    pnlTotal.Visible = false;
                    pnlEnh.Visible = false;
                    lblLock.Visible = Lock;
                    solidBrush3 = new SolidBrush(pnlFX.BackColor);
                    pen = new Pen(pnlFX.BackColor, 1f);
                    break;
                case 2:
                    pnlInfo.Visible = false;
                    pnlFX.Visible = false;
                    pnlTotal.Visible = true;
                    pnlEnh.Visible = false;
                    lblLock.Visible = false;
                    solidBrush3 = new SolidBrush(pnlTotal.BackColor);
                    pen = new Pen(pnlTotal.BackColor, 1f);
                    break;
                case 3:
                    pnlInfo.Visible = false;
                    pnlFX.Visible = false;
                    pnlTotal.Visible = false;
                    pnlEnh.Visible = true;
                    lblLock.Visible = Lock;
                    pen = new Pen(pnlEnh.BackColor, 1f);
                    solidBrush3 = new SolidBrush(pnlEnh.BackColor);
                    break;
            }

            RectangleF layoutRectangle;
            for (var index = 0; index < Pages.Length; index++)
            {
                rect = new Rectangle(rect.Width * index, 2, 70, pnlTabs.Height - 2);
                layoutRectangle = new RectangleF(rect.X, rect.Y + (float)((rect.Height - (double)font1.GetHeight(graphics)) / 2.0), rect.Width, font1.GetHeight(graphics));
                if (TabsMask != null && !TabsMask[index])
                {
                    continue;
                }

                extendedBitmap.Graphics.DrawRectangle(pen, rect);
                extendedBitmap.Graphics.DrawString(Pages[index], font1, solidBrush1, layoutRectangle, format);
            }

            rect = new Rectangle(70 * TabPage, 0, 70, pnlTabs.Height);
            layoutRectangle = new RectangleF(rect.X, (float)((rect.Height - (double)font1.GetHeight(graphics)) / 2.0), rect.Width, font1.GetHeight(graphics));
            extendedBitmap.Graphics.FillRectangle(solidBrush3, rect);
            extendedBitmap.Graphics.DrawRectangle(pen, rect);
            extendedBitmap.Graphics.DrawString(Pages[TabPage], font2, solidBrush1, layoutRectangle, format);
            graphics.DrawImageUnscaled(extendedBitmap.Bitmap, 0, 0);
        }

        private string GetToWhoShort(IEffect fx)
        {
            return fx.ToWho switch
            {
                Enums.eToWho.Target => " (Tgt)",
                Enums.eToWho.Self => " (Self)",
                _ => ""
            };
        }

        private int EffectsBuffDebuff(Control iLabel, PairedList iList)
        {
            var baseSumToHit = pBase.GetEffectMagSum(Enums.eEffectType.ToHit);
            var enhSumToHit = pEnh.GetEffectMagSum(Enums.eEffectType.ToHit);
            var baseSumDmgBuff = pBase.GetEffectMagSum(Enums.eEffectType.DamageBuff);
            var enhSumDmgBuff = pEnh.GetEffectMagSum(Enums.eEffectType.DamageBuff);
            var baseSumPerception = pBase.GetEffectMagSum(Enums.eEffectType.PerceptionRadius);
            var enhSumPerception = pEnh.GetEffectMagSum(Enums.eEffectType.PerceptionRadius);
            var baseSumStealth = pBase.GetEffectMagSum(Enums.eEffectType.StealthRadius);
            var enhSumStealth = pEnh.GetEffectMagSum(Enums.eEffectType.StealthRadius);
            var baseSumResEffect = pBase.GetEffectMagSum(Enums.eEffectType.ResEffect);
            var enhSumResEffect = pEnh.GetEffectMagSum(Enums.eEffectType.ResEffect);
            var baseSumEnhancement = pBase.GetEffectMagSum(Enums.eEffectType.Enhancement);
            var enhSumEnhancement = pEnh.GetEffectMagSum(Enums.eEffectType.Enhancement);
            
            var baseThreat = pBase.GetEffectMag(Enums.eEffectType.ThreatLevel);
            var enhThreat = pEnh.GetEffectMag(Enums.eEffectType.ThreatLevel);
            var baseDropToggles = pBase.GetEffectMag(Enums.eEffectType.DropToggles);
            var enhDropToggles = pEnh.GetEffectMag(Enums.eEffectType.DropToggles);
            var baseRechargeTimeSelf = pBase.GetEffectMag(Enums.eEffectType.RechargeTime, Enums.eToWho.Self);
            var enhRechargeTimeSelf = pEnh.GetEffectMag(Enums.eEffectType.RechargeTime, Enums.eToWho.Self);
            var baseRechargeTimeTarget = pBase.GetEffectMag(Enums.eEffectType.RechargeTime, Enums.eToWho.Target);
            var enhRechargeTimeTarget = pEnh.GetEffectMag(Enums.eEffectType.RechargeTime, Enums.eToWho.Target);

            baseSumToHit.Multiply();
            enhSumToHit.Multiply();
            enhSumDmgBuff.Multiply();
            baseSumPerception.Multiply();
            enhSumPerception.Multiply();

            baseThreat.Multiply();
            enhThreat.Multiply();
            baseRechargeTimeSelf.Multiply();
            enhRechargeTimeSelf.Multiply();
            baseRechargeTimeTarget.Multiply();
            enhRechargeTimeTarget.Multiply();

            baseSumResEffect.Multiply();
            enhSumResEffect.Multiply();
            baseSumEnhancement.Multiply();
            enhSumEnhancement.Multiply();
            baseSumDmgBuff.Multiply();

            var str1 = string.Empty;
            if (baseSumEnhancement.Present)
            {
                str1 = pBase.Effects[baseSumEnhancement.Index[0]].ETModifies == Enums.eEffectType.Mez
                    ? Enums.GetMezNameShort((Enums.eMezShort)pBase.Effects[baseSumEnhancement.Index[0]].MezType)
                    : Enums.GetEffectNameShort(pBase.Effects[baseSumEnhancement.Index[0]].ETModifies);
            }

            iList.ValueWidth = 55;
            int num1;
            if (!(baseSumToHit.Present |
                  enhSumDmgBuff.Present |
                  baseSumToHit.Present |
                  baseSumPerception.Present |
                  baseSumStealth.Present |
                  baseThreat.Present |
                  baseDropToggles.Present |
                  baseRechargeTimeSelf.Present |
                  baseRechargeTimeTarget.Present |
                  baseSumEnhancement.Present |
                  baseSumResEffect.Present))
            {
                num1 = 0;
            }
            else
            {
                if (iLabel.Text != string.Empty)
                {
                    iLabel.Text += @" / ";
                }

                iLabel.Text += @"Misc. Effects";
                if (baseSumToHit.Present && pBase.Effects[baseSumToHit.Index[0]].ValidateConditional(true))
                {
                    var toWho = GetToWhoShort(pBase.Effects[baseSumToHit.Index[0]]);
                    if (pBase.Effects[baseSumToHit.Index[0]].SpecialCase != Enums.eSpecialCase.None)
                    {
                        iList.AddItem(FastItem($"ToHit{toWho}", baseSumToHit, enhSumToHit, "%", false, false,
                            pBase.Effects[baseSumToHit.Index[0]].SpecialCase == Enums.eSpecialCase.Combo, false,
                            baseSumToHit, pEnh));
                    }
                    else
                    {
                        iList.AddItem(FastItem($"ToHit{toWho}", baseSumToHit, enhSumToHit, "%", false, false,
                            pBase.Effects[baseSumToHit.Index[0]].ValidateConditional("Active", "Combo"), false,
                            baseSumToHit, pEnh));
                    }
                }
                
                if (sFXCheck(baseSumToHit))
                {
                    iList.SetUnique();
                }

                // Original damage buff code (Effects Tab)
                var shortFxArray = Power.SplitFX(ref enhSumDmgBuff, ref pEnh);
                var defiancePower = DatabaseAPI.GetPowerByFullName("Inherent.Inherent.Defiance");
                for (var index1 = 0; index1 < shortFxArray.Length; index1++)
                {
                    if (!shortFxArray[index1].Present)
                    {
                        continue;
                    }

                    if (pEnh.Effects[shortFxArray[index1].Index[0]].ActiveConditionals.Count > 0)
                    {
                        if (!pEnh.Effects[shortFxArray[index1].Index[0]].ValidateConditional())
                        {
                            continue;
                        }
                    }

                    var isDefiance = shortFxArray[index1].Index.All(sFx =>
                        pEnh.Effects[sFx].SpecialCase == Enums.eSpecialCase.Defiance &&
                        pEnh.Effects[sFx].ValidateConditional("Active", "Defiance") |
                        MidsContext.Character.CurrentBuild.PowerActive(defiancePower));

                    if (isDefiance)
                    {
                        iList.AddItem(new PairedList.ItemPair("Defiance:", $"{Utilities.FixDP(shortFxArray[index1].Value[0])}%", false, false, false, pEnh.Effects[shortFxArray[index1].Index[0]].BuildEffectString(false, "DamageBuff (Defiance)")));
                    }
                    else
                    {
                        var toWho = GetToWhoShort(pEnh.Effects[shortFxArray[index1].Index[0]]);
                        if (pEnh.Effects[shortFxArray[index1].Index[0]].SpecialCase != Enums.eSpecialCase.None && pEnh.Effects[shortFxArray[index1].Index[0]].SpecialCase != Enums.eSpecialCase.Defiance)
                        {
                            iList.AddItem(new PairedList.ItemPair($"DamageBuff{toWho}:", $"{Utilities.FixDP(shortFxArray[index1].Value[0])}%", false, pEnh.Effects[shortFxArray[index1].Index[0]].SpecialCase == Enums.eSpecialCase.Combo, false, Power.SplitFXGroupTip(ref shortFxArray[index1], ref pEnh, false)));
                        }
                        else if (pEnh.Effects[shortFxArray[index1].Index[0]].ActiveConditionals.Count > 0)
                        {
                            iList.AddItem(new PairedList.ItemPair($"DamageBuff{toWho}:", $"{Utilities.FixDP(shortFxArray[index1].Value[0])}%", false, false, pEnh.Effects[shortFxArray[index1].Index[0]].ValidateConditional("Active", "Combo"), Power.SplitFXGroupTip(ref shortFxArray[index1], ref pEnh, false)));
                        }
                        else
                        {
                            iList.AddItem(new PairedList.ItemPair($"DamageBuff{toWho}:", $"{Utilities.FixDP(shortFxArray[index1].Value[0])}%", false, false, false, Power.SplitFXGroupTip(ref shortFxArray[index1], ref pEnh, false)));
                        }
                    }

                    if (pEnh.Effects[shortFxArray[index1].Index[0]].isEnhancementEffect)
                    {
                        iList.SetUnique();
                    }
                }

                if (sFXCheck(baseSumDmgBuff))
                {
                    iList.SetUnique();
                }

                if (baseSumPerception.Present)
                {
                    var toWho = GetToWhoShort(pEnh.Effects[enhSumPerception.Index[0]]);
                    var tip = string.Join("\r\n", pEnh.Effects
                            .Where(e => e.EffectType == Enums.eEffectType.PerceptionRadius)
                            .Where(e => e.ActiveConditionals.Count <= 0 || e.ValidateConditional())
                            .Select(e => e.BuildEffectString(false, "", false, false, false, false, false, true)))
                        .Trim();
                    var perceptionDistance = Statistics.BasePerception * enhSumPerception.Sum / 100;
                    iList.AddItem(new PairedList.ItemPair($"Pceptn({toWho})",
                        $"{(pEnh.Effects[enhSumPerception.Index[0]].DisplayPercentage ? $"{enhSumPerception.Sum:###0.##}% ({perceptionDistance:###0.##}ft)" : $"{perceptionDistance:###0.##}ft")}",
                        Math.Abs(enhSumPerception.Sum - baseSumPerception.Sum) > 0, false, false, tip));
                }

                if (sFXCheck(baseSumPerception))
                {
                    iList.SetUnique();
                }

                if (baseSumStealth.Present && enhSumStealth.Index is {Length: > 0})
                {
                    if (pEnh.Effects[enhSumStealth.Index[0]].ValidateConditional(true))
                    {
                        var toWho = GetToWhoShort(pEnh.Effects[enhSumStealth.Index[0]]);
                        iList.AddItem(new PairedList.ItemPair($"Stealth{toWho}",
                            $"{baseSumStealth.Sum}ft", false, false, false,
                            pEnh.Effects[enhSumStealth.Index[0]].BuildEffectString(false, "Stealth Radius")));
                    }
                }

                if (sFXCheck(baseSumStealth))
                {
                    iList.SetUnique();
                }

                if (baseThreat.Present)
                {
                    iList.AddItem(FastItem("ThreatLvl", baseThreat, enhThreat, "%", baseThreat, pEnh));
                }

                if (sFXCheck(baseThreat))
                {
                    iList.SetUnique();
                }

                if (baseDropToggles.Present && baseDropToggles.Index is {Length: > 0})
                {
                    if (pBase.Effects[baseDropToggles.Index[0]].ValidateConditional(true))
                    {
                        iList.AddItem(FastItem("TogDrop", baseDropToggles, enhDropToggles, "%", false, false,
                            pBase.Effects[baseDropToggles.Index[0]].Probability < 1, false, baseDropToggles, pEnh));
                    }
                }

                if (sFXCheck(baseDropToggles))
                {
                    iList.SetUnique();
                }

                if (baseRechargeTimeTarget.Present)
                {
                    iList.AddItem(FastItem("Rchrg (Tgt)", baseRechargeTimeTarget, enhRechargeTimeTarget, "%", baseRechargeTimeTarget, pEnh));
                }

                if (sFXCheck(baseRechargeTimeTarget))
                {
                    iList.SetUnique();
                }

                if (baseRechargeTimeSelf.Present)
                {
                    iList.AddItem(FastItem("Rchrg (Self)", baseRechargeTimeSelf, enhRechargeTimeSelf, "%", baseRechargeTimeSelf, pEnh));
                }

                if (sFXCheck(baseRechargeTimeSelf))
                {
                    iList.SetUnique();
                }

                if (baseSumResEffect.Present && baseSumResEffect.Index is {Length: > 0})
                {
                    if (pBase.Effects[baseSumResEffect.Index[0]].ValidateConditional(true))
                    {
                        var toWho = GetToWhoShort(pBase.Effects[baseSumResEffect.Index[0]]);
                        if (!baseSumResEffect.Multiple)
                        {
                            if (baseSumResEffect.Present)
                            {
                                iList.AddItem(FastItem($"Res({Enums.GetEffectNameShort(pBase.Effects[baseSumResEffect.Index[0]].ETModifies)}){toWho}",
                                    baseSumResEffect, enhSumResEffect, "%", baseSumResEffect, pEnh));
                            }

                            if (sFXCheck(baseSumResEffect))
                            {
                                iList.SetUnique();
                            }
                        }
                        else
                        {
                            var baseFx = pBase.Effects[baseSumResEffect.Index[0]];
                            var similarFxList = pBase.Effects
                                .Where(e => (e.PvMode == Enums.ePvX.Any |
                                             e.PvMode == Enums.ePvX.PvE & !MidsContext.Config.Inc.DisablePvE |
                                             e.PvMode == Enums.ePvX.PvP & MidsContext.Config.Inc.DisablePvE) &
                                            e.EffectType == baseFx.EffectType &
                                            e.ToWho == baseFx.ToWho)
                                .ToList();
                            var tip = string.Join("\r\n",similarFxList.Select(e => e.BuildEffectString(false, "", false, false, false, true)));
                            var minFx = similarFxList
                                .OrderBy(e => e.BuffedMag)
                                .First();

                            iList.AddItem(new PairedList.ItemPair($"Res(Multi){toWho}:", $"{(minFx.DisplayPercentage ? $"{minFx.BuffedMag * 100:####0.##}%" : $"{minFx.BuffedMag:####0.##}")}", false, false, false, tip));
                            if (sFXCheck(baseSumResEffect))
                            {
                                iList.SetUnique();
                            }
                        }
                    }
                }

                if (baseSumEnhancement.Present & str1 != string.Empty)
                {
                    var str2 = !string.Equals(str1, "EnduranceDiscount", StringComparison.OrdinalIgnoreCase)
                        ? !string.Equals(str1, "RechargeTime", StringComparison.OrdinalIgnoreCase)
                            ? !IsMezEffect(str1) ? CapString(str1, 7) : "Effects"
                            : "RechRdx"
                        : "EndRdx";
                    
                    if (baseSumEnhancement.Multiple)
                    {
                        if (baseSumEnhancement.Index.Length < 5)
                        {
                            for (var index = 0; index < baseSumEnhancement.Index.Length; index++)
                            {
                                if (!pBase.Effects[baseSumEnhancement.Index[index]].ValidateConditional(true))
                                {
                                    continue;
                                }

                                var str3 = pBase.Effects[baseSumEnhancement.Index[index]].ETModifies == Enums.eEffectType.Mez
                                    ? Enums.GetMezNameShort((Enums.eMezShort)pBase.Effects[baseSumEnhancement.Index[index]].MezType)
                                    : Enums.GetEffectNameShort(pBase.Effects[baseSumEnhancement.Index[index]].ETModifies);
                                var str4 = !string.Equals(str3, "EnduranceDiscount", StringComparison.OrdinalIgnoreCase)
                                    ? !string.Equals(str3, "RechargeTime", StringComparison.OrdinalIgnoreCase)
                                        ? CapString(str3, 7)
                                        : "RechRdx"
                                    : "EndRdx";
                                var str5 = pEnh.Effects[baseSumEnhancement.Index[index]].ToWho switch
                                {
                                    Enums.eToWho.Target => " (Tgt)",
                                    Enums.eToWho.Self => " (Self)",
                                    _ => ""
                                };

                                if (str4.IndexOf("Jump", StringComparison.OrdinalIgnoreCase) > -1)
                                {
                                    continue;
                                }

                                // Prevent crash when switching primary specs
                                if (enhSumEnhancement.Value == null)
                                {
                                    continue;
                                }

                                iList.AddItem(new PairedList.ItemPair($"+{str4}:",
                                    $"{enhSumEnhancement.Value[index]}%{str5}", false, false, false,
                                    pEnh.Effects[enhSumEnhancement.Index[index]].BuildEffectString()));
                                if (pEnh.Effects[baseSumEnhancement.Index[index]].isEnhancementEffect)
                                {
                                    iList.SetUnique();
                                }
                            }
                        }
                        else
                        {
                            var iIndex = 0;
                            var num3 = -1;
                            var multipleVariableFx = false;
                            while (iIndex < baseSumEnhancement.Index.Length)
                            {
                                if (!pEnh.Effects[baseSumEnhancement.Index[iIndex]].ValidateConditional())
                                {
                                    ++iIndex;
                                    continue;
                                }

                                if (string.Equals(pEnh.Effects[baseSumEnhancement.Index[iIndex]].Special, "RechargeTime", StringComparison.OrdinalIgnoreCase))
                                {
                                    var str4 = pEnh.Effects[baseSumEnhancement.Index[iIndex]].ToWho switch
                                    {
                                        Enums.eToWho.Target => " (Tgt)",
                                        Enums.eToWho.Self => " (Self)",
                                        _ => ""
                                    };
                                    
                                    iList.AddItem(new PairedList.ItemPair("+RechRdx:",
                                        $"{enhSumEnhancement.Value[iIndex]}%{str4}", false, false, false,
                                        pEnh.Effects[enhSumEnhancement.Index[iIndex]].BuildEffectString()));
                                    if (pEnh.Effects[enhSumEnhancement.Index[iIndex]].isEnhancementEffect)
                                    {
                                        iList.SetUnique();
                                    }

                                    baseSumEnhancement.Remove(iIndex);
                                }
                                else
                                {
                                    if (num3 == -1)
                                    {
                                        num3 = (int)Math.Round(enhSumEnhancement.Value[0]);
                                    }
                                    else if (num3 > 0 & Math.Abs(num3 - enhSumEnhancement.Value[iIndex]) > float.Epsilon)
                                    {
                                        num3 = -2;
                                        multipleVariableFx = true;
                                    }

                                    ++iIndex;
                                }
                            }

                            if (baseSumEnhancement.Present)
                            {
                                var iValue = multipleVariableFx ? "Varies" : $"{num3}%";
                                var similarFxList = new List<IEffect>();
                                if (enhSumEnhancement.Multiple)
                                {
                                    var tip = "";
                                    foreach (var fx in pEnh.Effects)
                                    {
                                        if (fx.EffectType != Enums.eEffectType.Enhancement)
                                        {
                                            continue;
                                        }

                                        if (fx.ActiveConditionals is {Count: > 0})
                                        {
                                            if (!fx.ValidateConditional())
                                            {
                                                continue;
                                            }
                                        }

                                        tip += $"{(tip != "" ? "\r\n" : "")}{fx.BuildEffectString(false, "", false, false, false, true)}";
                                        similarFxList.Add(fx);
                                    }

                                    var minFx = similarFxList
                                        .OrderBy(e => e.BuffedMag)
                                        .First();

                                    iValue = similarFxList
                                        .Select(e => e.BuffedMag)
                                        .Any(e => Math.Abs(e - minFx.BuffedMag) > float.Epsilon)
                                        ? "Varies"
                                        : $"{(minFx.DisplayPercentage ? $"{minFx.BuffedMag * 100:####0.##}" : $"{minFx.BuffedMag}")}";

                                    var multipleFxSign = similarFxList
                                        .Select(e => e.BuffedMag)
                                        .Any(e => e > 0);

                                    iList.AddItem(new PairedList.ItemPair($"{(multipleFxSign ? "+" : "-")}Multiple:", iValue, false, false, false, tip.Trim()));
                                }
                                else
                                {
                                    var str4 = CapString(pBase.Effects[baseSumEnhancement.Index[0]].Special, 7);
                                    if (str4.IndexOf("Jump", StringComparison.OrdinalIgnoreCase) < 0)
                                    {
                                        iList.AddItem(new PairedList.ItemPair($"+{str4}:", iValue, false, false, false, enhSumEnhancement));
                                        if (sFXCheck(baseSumEnhancement))
                                        {
                                            iList.SetUnique();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (str2.IndexOf("Jump", StringComparison.OrdinalIgnoreCase) < 0 && enhSumEnhancement.Value != null)
                    {
                        if (pEnh.Effects[enhSumEnhancement.Index[0]].ValidateConditional(true))
                        {
                            // Hasten +Recharge will fall here
                            //iList.AddItem(new PairedList.ItemPair($"+{str2}:", $"{enhSumEnhancement.Value[0]}%", false, false, false, enhSumEnhancement));
                            var tip = GenerateTipFromEffect(pEnh, pEnh.Effects[enhSumEnhancement.Index[0]]);
                            iList.AddItem(FastItem($"+{str2}:", baseSumEnhancement.Value[0], enhSumEnhancement.Value[0], "%", tip));
                            if (sFXCheck(baseSumEnhancement))
                            {
                                iList.SetUnique();
                            }
                        }
                    }
                }

                num1 = 1;
            }

            return num1;
        }

        private int EffectsElusivity(Label iLabel, PairedList iList)
        {
            var flag = iList.ItemCount == 0;
            var num1 = 0;
            for (var idEffect = 0; idEffect < pBase.Effects.Length; idEffect++)
            {
                if (!((pBase.Effects[idEffect].EffectType == Enums.eEffectType.Elusivity) &
                      (pBase.Effects[idEffect].Probability > 0)))
                {
                    continue;
                }

                if (!pBase.Effects[idEffect].ValidateConditional())
                {
                    continue;
                }

                var empty = string.Empty;
                var returnMask = new int[Enum.GetValues(pBase.Effects[idEffect].DamageType.GetType()).Length + 1];
                pBase.GetEffectStringGrouped(idEffect, ref empty, ref returnMask, false, true, true);
                var iTip = empty;
                var num3 = pBase.Effects[idEffect].MagPercent;
                if ((pBase.Effects[idEffect].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                {
                    num3 = 0;
                }

                var iItem = new PairedList.ItemPair("Elusivity:", $"{num3:0.##}%", false, pBase.Effects[idEffect].Probability < 1, false, iTip);
                iList.AddItem(iItem);
                var num4 = num1 + 1;
                if (flag)
                {
                    iLabel.Text = "Elusivity (PvP)";
                }

                return num4;
            }

            if (num1 > 0 && flag)
            {
                iLabel.Text = "Elusivity (PvP)";
            }

            return num1;
        }

        private int EffectsGrantPower(Label iLabel, PairedList iList)
        {
            var flag = iList.ItemCount == 0;
            var num1 = 0;
            for (var index = 0; index < pBase.Effects.Length; index++)
            {
                if (!((pBase.Effects[index].EffectType == Enums.eEffectType.GrantPower) &
                      (pBase.Effects[index].Probability > 0.0) &
                      (pBase.Effects[index].EffectClass != Enums.eEffectClass.Ignored)))
                {
                    continue;
                }

                if (!pBase.Effects[index].ValidateConditional())
                {
                    continue;
                }

                var iValue = "[Power]";
                if (pEnh.Effects[index].nSummon > -1)
                {
                    iValue = DatabaseAPI.Database.Power[pEnh.Effects[index].nSummon].DisplayName;
                }
                else
                {
                    var startIndex = pEnh.Effects[index].Summon.LastIndexOf(".", StringComparison.Ordinal) + 1;
                    if (startIndex < pEnh.Effects[index].Summon.Length)
                    {
                        iValue = pEnh.Effects[index].Summon.Substring(startIndex);
                    }
                }

                var iTip = pEnh.Effects[index].BuildEffectString();
                if ((pBase.Effects[index].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                {
                    iValue = "(suppressed)";
                }

                var iItem = new PairedList.ItemPair("GrantPwr:", iValue, false, pBase.Effects[index].Probability < 1.0, false, iTip);
                iList.AddItem(iItem);
                if (pBase.Effects[index].isEnhancementEffect)
                {
                    iList.SetUnique();
                }

                ++num1;
            }

            if (num1 > 0 && flag)
            {
                iLabel.Text = "GrantPower Effects";
            }

            return num1;
        }

        private int EffectsHeal(Label iLabel, PairedList iList)
        {
            var BaseSFX1 = new Enums.ShortFX();
            var EnhSFX1 = new Enums.ShortFX();
            var BaseSFX2 = new Enums.ShortFX();
            var EnhSFX2 = new Enums.ShortFX();
            var BaseSFX3 = new Enums.ShortFX();
            var EnhSFX3 = new Enums.ShortFX();
            var BaseSFX4 = new Enums.ShortFX();
            var EnhSFX4 = new Enums.ShortFX();
            var BaseSFX5 = new Enums.ShortFX();
            var EnhSFX5 = new Enums.ShortFX();
            var BaseSFX6 = new Enums.ShortFX();
            var EnhSFX6 = new Enums.ShortFX();
            var BaseSFX7 = new Enums.ShortFX();
            var EnhSFX7 = new Enums.ShortFX();
            var BaseSFX8 = new Enums.ShortFX();
            var EnhSFX8 = new Enums.ShortFX();
            var BaseSFX9 = new Enums.ShortFX();
            var EnhSFX9 = new Enums.ShortFX();
            var BaseSFX10 = new Enums.ShortFX();
            var EnhSFX10 = new Enums.ShortFX();
            var BaseSFX11 = new Enums.ShortFX();
            var EnhSFX11 = new Enums.ShortFX();
            var BaseSFX12 = new Enums.ShortFX();
            var EnhSFX12 = new Enums.ShortFX();
            var shortFx = new Enums.ShortFX();
            BaseSFX1.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Heal));
            EnhSFX1.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Heal));
            BaseSFX2.Assign(pBase.GetEffectMagSum(Enums.eEffectType.HitPoints));
            EnhSFX2.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.HitPoints));
            BaseSFX3.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Absorb));
            EnhSFX3.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Absorb));
            BaseSFX5.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Regeneration, false, true));
            EnhSFX5.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Regeneration, false, true));
            BaseSFX4.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Regeneration, true, false, true));
            EnhSFX4.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Regeneration, true, false, true));
            BaseSFX10.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Regeneration, true, false, false, true));
            EnhSFX10.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Regeneration, true, false, false, true));
            BaseSFX6.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Recovery, true, true));
            EnhSFX6.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Recovery, true, true));
            BaseSFX7.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Recovery, true, false, true));
            EnhSFX7.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Recovery, true, false, true));
            BaseSFX11.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Recovery, true, false, false, true));
            EnhSFX11.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Recovery, true, false, false, true));
            BaseSFX8.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Endurance, true, true));
            EnhSFX8.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Endurance, true, true));
            BaseSFX9.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Endurance, true, false, true));
            EnhSFX9.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Endurance, true, false, true));
            BaseSFX12.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Endurance, true, false, false, true));
            EnhSFX12.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Endurance, true, false, false, true));
            if (!pBase.DisplayName.Contains("Particle"))
            {
                BaseSFX3.Multiply();
                EnhSFX3.Multiply();
            }

            BaseSFX5.Multiply();
            EnhSFX5.Multiply();
            BaseSFX4.Multiply();
            EnhSFX4.Multiply();
            BaseSFX6.Multiply();
            EnhSFX6.Multiply();
            BaseSFX7.Multiply();
            EnhSFX7.Multiply();
            BaseSFX10.Multiply();
            EnhSFX10.Multiply();
            BaseSFX11.Multiply();
            EnhSFX11.Multiply();
            BaseSFX12.Multiply();
            EnhSFX12.Multiply();
            for (var iIndex = 0; iIndex < pBase.Effects.Length; iIndex++)
            {
                if ((pBase.Effects[iIndex].EffectType == Enums.eEffectType.Damage) &
                    (pBase.Effects[iIndex].DamageType == Enums.eDamage.Special) &
                    (pBase.Effects[iIndex].ToWho == Enums.eToWho.Self) &
                    (pBase.Effects[iIndex].Probability > 0.0) &
                    ((pBase.Effects[iIndex].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None))
                {
                    if (!pBase.Effects[iIndex].ValidateConditional())
                    {
                        continue;
                    }

                    shortFx.Add(iIndex, pBase.Effects[iIndex].BuffedMag);
                }
            }

            iList.ValueWidth = 55;
            int num2;
            if (!(shortFx.Present |
                BaseSFX1.Present |
                BaseSFX2.Present |
                BaseSFX5.Present |
                BaseSFX4.Present |
                BaseSFX6.Present |
                BaseSFX7.Present |
                BaseSFX8.Present |
                BaseSFX9.Present |
                BaseSFX10.Present |
                BaseSFX11.Present |
                BaseSFX12.Present))
            {
                num2 = 0;
            }
            else
            {
                var flag1 = pBase.AffectsTarget(Enums.eEffectType.Heal) |
                            pBase.AffectsTarget(Enums.eEffectType.HitPoints) |
                            pBase.AffectsTarget(Enums.eEffectType.Regeneration) |
                            pBase.AffectsTarget(Enums.eEffectType.Recovery) |
                            pBase.AffectsTarget(Enums.eEffectType.Endurance) |
                            pBase.AffectsTarget(Enums.eEffectType.Absorb);
                var flag2 = pBase.AffectsSelf(Enums.eEffectType.Heal) | pBase.AffectsSelf(Enums.eEffectType.HitPoints) |
                            pBase.AffectsSelf(Enums.eEffectType.Regeneration) |
                            pBase.AffectsSelf(Enums.eEffectType.Recovery) |
                            pBase.AffectsSelf(Enums.eEffectType.Endurance) |
                            pBase.AffectsSelf(Enums.eEffectType.Absorb);
                if (flag1 & !flag2)
                {
                    iLabel.Text = @"Health / Endurance (Target)";
                }
                else if (flag2 & !flag1)
                {
                    iLabel.Text = @"Health / Endurance (Self)";
                }
                else
                {
                    iLabel.Text = @"Health / Endurance";
                }

                if (shortFx.Present)
                {
                    for (var index = 0; index < shortFx.Index.Length; index++)
                    {
                        if (pBase.Effects[shortFx.Index[index]].Aspect != Enums.eAspect.Cur)
                        {
                            continue;
                        }

                        if (Math.Abs(shortFx.Value[index] + 1) < float.Epsilon)
                        {
                            shortFx.Value[index] = 0;
                        }

                        if (Math.Abs(shortFx.Value[index] - 100) > float.Epsilon)
                        {
                            shortFx.Value[index] *= 100;
                        }
                    }

                    shortFx.ReSum();
                    iList.AddItem(FastItem("Damage", shortFx, shortFx, " (Self)", shortFx, pEnh));
                }

                SplitFX_AddToList(ref BaseSFX1, ref EnhSFX1, ref iList);
                SplitFX_AddToList(ref BaseSFX2, ref EnhSFX2, ref iList);
                SplitFX_AddToList(ref BaseSFX3, ref EnhSFX3, ref iList);
                SplitFX_AddToList(ref BaseSFX5, ref EnhSFX5, ref iList, "Regen(S)");
                SplitFX_AddToList(ref BaseSFX4, ref EnhSFX4, ref iList, "Regen(T)");
                SplitFX_AddToList(ref BaseSFX6, ref EnhSFX6, ref iList, "Recvry(S)");
                SplitFX_AddToList(ref BaseSFX7, ref EnhSFX7, ref iList, "Recvry(T)");
                SplitFX_AddToList(ref BaseSFX9, ref EnhSFX9, ref iList, "End (Tgt)");
                SplitFX_AddToList(ref BaseSFX8, ref EnhSFX8, ref iList, "End (Self)");
                num2 = iList.ItemCount;
            }

            return num2;
        }

        private int EffectsModifyEffect(Label iLabel, PairedList iList)
        {
            var flag = iList.ItemCount == 0;
            var num1 = 0;
            for (var index = 0; index < pBase.Effects.Length; index++)
            {
                if (!((pBase.Effects[index].EffectType == Enums.eEffectType.GlobalChanceMod) & (pBase.Effects[index].Probability > 0)))
                {
                    continue;
                }

                if (!pBase.Effects[index].ValidateConditional())
                {
                    continue;
                }

                var iTip = pEnh.Effects[index].BuildEffectString();
                var iValue = $"{pBase.Effects[index].MagPercent}%";
                if ((pBase.Effects[index].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                {
                    iValue = "(suppressed)";
                }

                var iItem = new PairedList.ItemPair($"{pBase.Effects[index].Reward}:", iValue, false, pBase.Effects[index].Probability < 1, false, iTip);
                iList.AddItem(iItem);
                ++num1;
            }

            if (num1 > 0 && flag)
            {
                iLabel.Text = "Modify Effect";
            }

            return num1;
        }

        private int EffectsMovement(Label iLabel, PairedList iList)
        {
            var sFxFlyBase = new Enums.ShortFX();
            var sFxSpeedFlyingBase = new Enums.ShortFX();
            var sFxSpeedFlyingEnh = new Enums.ShortFX();
            var sFxJumpHeightBase = new Enums.ShortFX();
            var sFxJumpHeightEnh = new Enums.ShortFX();
            var sFxSpeedJumpingBase = new Enums.ShortFX();
            var sFxSpeedJumpingEnh = new Enums.ShortFX();
            var sFxSpeedRunningBase = new Enums.ShortFX();
            var sFxSpeedRunningEnh = new Enums.ShortFX();
            var sFxEnhancementBase = new Enums.ShortFX();
            var sFxJump = new Enums.ShortFX();
            var sFxSpeedFlyingMaxBase1 = new Enums.ShortFX();
            var sFxSpeedJumpingMaxBase1 = new Enums.ShortFX();
            var sFxSpeedRunningMaxBase1 = new Enums.ShortFX();
            var sFxSpeedFlyingMaxBase2 = new Enums.ShortFX();
            var sFxSpeedJumpingMaxBase2 = new Enums.ShortFX();
            var sFxSpeedRunningMaxBase2 = new Enums.ShortFX();

            sFxSpeedFlyingBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.SpeedFlying));
            sFxSpeedFlyingEnh.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.SpeedFlying));
            sFxFlyBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Fly));
            sFxJumpHeightBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.JumpHeight));
            sFxJumpHeightEnh.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.JumpHeight));
            sFxSpeedJumpingBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.SpeedJumping));
            sFxSpeedJumpingEnh.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.SpeedJumping));
            sFxSpeedRunningBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.SpeedRunning));
            sFxSpeedRunningEnh.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.SpeedRunning));
            sFxEnhancementBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Enhancement));
            sFxSpeedFlyingMaxBase1.Assign(pBase.GetEffectMagSum(Enums.eEffectType.SpeedFlying, false, false, false, true));
            sFxSpeedJumpingMaxBase1.Assign(pBase.GetEffectMagSum(Enums.eEffectType.SpeedJumping, false, false, false, true));
            sFxSpeedRunningMaxBase1.Assign(pBase.GetEffectMagSum(Enums.eEffectType.SpeedRunning, false, false, false, true));
            sFxSpeedFlyingMaxBase2.Assign(pBase.GetEffectMagSum(Enums.eEffectType.MaxFlySpeed));
            sFxSpeedJumpingMaxBase2.Assign(pBase.GetEffectMagSum(Enums.eEffectType.MaxJumpSpeed));
            sFxSpeedRunningMaxBase2.Assign(pBase.GetEffectMagSum(Enums.eEffectType.MaxRunSpeed));
            sFxSpeedFlyingBase.Multiply();
            sFxSpeedFlyingEnh.Multiply();
            sFxJumpHeightBase.Multiply();
            sFxJumpHeightEnh.Multiply();
            sFxSpeedJumpingBase.Multiply();
            sFxSpeedJumpingEnh.Multiply();
            sFxSpeedRunningBase.Multiply();
            sFxSpeedRunningEnh.Multiply();
            sFxSpeedFlyingMaxBase2.Multiply();
            sFxSpeedJumpingMaxBase2.Multiply();
            sFxSpeedRunningMaxBase2.Multiply();

            if (sFxEnhancementBase.Present)
            {
                for (var index = 0; index < sFxEnhancementBase.Index.Length; index++)
                {
                    if (!pBase.Effects[index].ValidateConditional())
                    {
                        continue;
                    }

                    if (pBase.Effects[sFxEnhancementBase.Index[index]].Special.IndexOf("Jump", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        sFxJump.Add(sFxEnhancementBase.Index[index], pBase.Effects[sFxEnhancementBase.Index[index]].BuffedMag);
                    }
                }
            }

            iList.ValueWidth = 55;
            if (!(sFxJump.Present | sFxFlyBase.Present | sFxSpeedFlyingBase.Present | sFxJumpHeightBase.Present | sFxSpeedJumpingBase.Present |
                  sFxSpeedRunningBase.Present | sFxSpeedFlyingMaxBase2.Present | sFxSpeedJumpingMaxBase2.Present | sFxSpeedRunningMaxBase2.Present))
            {
                return 0;
            }

            var flag = (sFxSpeedFlyingBase.Present & pBase.AffectsTarget(Enums.eEffectType.SpeedFlying)) |
                       (sFxJumpHeightBase.Present & pBase.AffectsTarget(Enums.eEffectType.JumpHeight)) |
                       (sFxSpeedJumpingBase.Present & pBase.AffectsTarget(Enums.eEffectType.SpeedJumping)) |
                       (sFxSpeedRunningBase.Present & pBase.AffectsTarget(Enums.eEffectType.SpeedRunning)) |
                       (sFxSpeedFlyingMaxBase2.Present & pBase.AffectsTarget(Enums.eEffectType.MaxFlySpeed)) |
                       (sFxSpeedJumpingMaxBase2.Present & pBase.AffectsTarget(Enums.eEffectType.MaxJumpSpeed)) |
                       (sFxSpeedRunningMaxBase2.Present & pBase.AffectsTarget(Enums.eEffectType.MaxRunSpeed));

            if (iList.ItemCount == 0)
            {
                iLabel.Text = flag ? "Movement (Target)" : "Movement (Self)";
            }

            if (sFxFlyBase.Present)
            {
                iList.AddItem(FastItem("Fly", sFxFlyBase, sFxFlyBase, string.Empty, sFxFlyBase, pEnh));
            }

            if (sFXCheck(sFxFlyBase))
            {
                iList.SetUnique();
            }

            if (sFxSpeedFlyingBase.Present)
            {
                iList.AddItem(FastItem("FlySpd", sFxSpeedFlyingBase, sFxSpeedFlyingEnh, "%", sFxSpeedFlyingBase, pEnh));
            }

            if (sFXCheck(sFxSpeedFlyingBase))
            {
                iList.SetUnique();
            }

            if (sFxJumpHeightBase.Present)
            {
                iList.AddItem(FastItem("JmpHeight", sFxJumpHeightBase, sFxJumpHeightEnh, "%", sFxJumpHeightBase, pEnh));
            }

            if (sFXCheck(sFxJumpHeightBase))
            {
                iList.SetUnique();
            }

            if (sFxSpeedJumpingBase.Present)
            {
                iList.AddItem(FastItem("JmpSpd", sFxSpeedJumpingBase, sFxSpeedJumpingEnh, "%", sFxSpeedJumpingBase, pEnh));
            }

            if (sFXCheck(sFxSpeedJumpingBase))
            {
                iList.SetUnique();
            }

            if (sFxJump.Present)
            {
                iList.AddItem(FastItem("+JmpHeight", sFxJump, sFxJump, "%", sFxJump, pEnh));
            }

            if (sFXCheck(sFxJump))
            {
                iList.SetUnique();
            }

            if (sFxSpeedRunningBase.Present)
            {
                iList.AddItem(FastItem("RunSpd", sFxSpeedRunningBase, sFxSpeedRunningEnh, "%", sFxSpeedRunningBase, pEnh));
            }

            if (sFXCheck(sFxSpeedRunningBase))
            {
                iList.SetUnique();
            }

            if (sFxSpeedRunningMaxBase1.Present)
            {
                iList.AddItem(FastItem("MaxRun", sFxSpeedRunningMaxBase1, sFxSpeedRunningMaxBase1, string.Empty, sFxSpeedRunningMaxBase1, pEnh));
            }

            if (sFxSpeedJumpingMaxBase1.Present)
            {
                iList.AddItem(FastItem("MaxJmp", sFxSpeedJumpingMaxBase1, sFxSpeedJumpingMaxBase1, string.Empty, sFxSpeedJumpingMaxBase1, pEnh));
            }

            if (sFxSpeedFlyingMaxBase1.Present)
            {
                iList.AddItem(FastItem("MaxFly", sFxSpeedFlyingMaxBase1, sFxSpeedFlyingMaxBase1, string.Empty, sFxSpeedFlyingMaxBase1, pEnh));
            }

            if (sFxSpeedRunningMaxBase2.Present)
            {
                iList.AddItem(FastItem("MaxRun", sFxSpeedRunningMaxBase2, sFxSpeedRunningMaxBase2, "%", sFxSpeedRunningMaxBase2, pEnh));
            }

            if (sFxSpeedJumpingMaxBase2.Present)
            {
                iList.AddItem(FastItem("MaxJmp", sFxSpeedJumpingMaxBase2, sFxSpeedJumpingMaxBase2, "%", sFxSpeedJumpingMaxBase2, pEnh));
            }

            if (sFxSpeedFlyingMaxBase2.Present)
            {
                iList.AddItem(FastItem("MaxFly", sFxSpeedFlyingMaxBase2, sFxSpeedFlyingMaxBase2, "%", sFxSpeedFlyingMaxBase2, pEnh));
            }

            return 1;
        }

        // Move the extra effects from the longest array (pBase) to the shortest (pEnh),
        // Resulting in pEnh always being the longest one.
        private List<IEffect[]> SwapExtraEffects(IEffect[] baseEffects, IEffect[] enhEffects)
        {
            var enhFxList = enhEffects.ToList();
            for (var i = enhEffects.Length; i < baseEffects.Length; i++)
            {
                enhFxList.Add((IEffect)baseEffects[i].Clone());
            }

            var baseFxList = new List<IEffect>();
            for (var i = 0; i < enhEffects.Length; i++)
            {
                baseFxList.Add((IEffect)baseEffects[i].Clone());
            }

            baseEffects = baseFxList.ToArray();
            enhEffects = enhFxList.ToArray();

            return new List<IEffect[]> { baseEffects, enhEffects };
        }

        private List<Enums.ShortFX[]> SwapExtraEffects(Enums.ShortFX[] baseEffects, Enums.ShortFX[] enhEffects)
        {
            var enhFxList = enhEffects.ToList();
            for (var i = enhEffects.Length; i < baseEffects.Length; i++)
            {
                enhFxList.Add((Enums.ShortFX)baseEffects[i].Clone());
            }

            var baseFxList = new List<Enums.ShortFX>();
            for (var i = 0; i < enhEffects.Length; i++)
            {
                baseFxList.Add((Enums.ShortFX)baseEffects[i].Clone());
            }

            baseEffects = baseFxList.ToArray();
            enhEffects = enhFxList.ToArray();

            return new List<Enums.ShortFX[]> { baseEffects, enhEffects };
        }

        /// <summary>
        /// Add Mez effects to the effects tab. 
        /// </summary>
        /// <param name="sourcePower">Source power to fetch effects from. Can be pBase, pEnh, or a virtual set bonuses power <see cref="Build.BuildVirtualSetBonusesPower"></see></param>
        /// <param name="iList">Paired list to add items into</param>
        /// <param name="specialEffects">Set to true if sourcePower is a virtual set bonuses power, otherwise false</param>
        /// <param name="effectsCount">Number of effects processed</param>
        /// <param name="iAlternate">Force value to show as enhanced (true) or equal to base (false). Set to null for auto</param>
        /// <param name="startIndex">Effect index to start processing from. Set to 0 unless processing pEnh effects to avoid duplicates</param>
        private void ProcessMezEffects(IPower sourcePower, ref PairedList iList, bool specialEffects, ref int effectsCount, bool? iAlternate = null, int startIndex = 0)
        {
            var names = Enum.GetNames(typeof(Enums.eMezShort));
            var enhancedPower = specialEffects ? sourcePower : pEnh;
            var n = 0;

            for (var tagId = startIndex; tagId < sourcePower.Effects.Length; tagId++)
            {
                if (!(sourcePower.Effects[tagId].EffectType == Enums.eEffectType.Mez &
                      sourcePower.Effects[tagId].Probability > 0 &
                      sourcePower.Effects[tagId].CanInclude()) || !sourcePower.Effects[tagId].PvXInclude())
                {
                    continue;
                }

                if (sourcePower.Effects[tagId].ActiveConditionals is {Count: > 0})
                {
                    if (!sourcePower.Effects[tagId].ValidateConditional())
                    {
                        continue;
                    }
                }

                var mezDuration = !(enhancedPower.Effects[tagId].Duration < 2 | enhancedPower.PowerType == Enums.ePowerType.Auto_)
                    ? $" - {enhancedPower.Effects[tagId].Duration:#0.#}s"
                    : "";

                if (sourcePower.Effects[tagId].BuffedMag > 0)
                {
                    var iAlternate2 = iAlternate ?? Math.Abs(sourcePower.Effects[tagId].Duration - enhancedPower.Effects[tagId].Duration) > float.Epsilon |
                        !Enums.MezDurationEnhanceable(sourcePower.Effects[tagId].MezType) &
                        Math.Abs(enhancedPower.Effects[tagId].BuffedMag - sourcePower.Effects[tagId].BuffedMag) > float.Epsilon;
                    var iValue = (sourcePower.Effects[tagId].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None
                        ? "0"
                        : $"Mag {Utilities.FixDP(enhancedPower.Effects[tagId].BuffedMag):####0.##}{mezDuration}";
                    if ((sourcePower.Effects[tagId].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                    {
                        iValue = "0";
                    }

                    var tip = GenerateTipFromEffect(enhancedPower, enhancedPower.Effects[tagId]);
                    var iItem = new PairedList.ItemPair($"{CapString(names[(int)sourcePower.Effects[tagId].MezType], 7)}:", iValue, iAlternate2,
                        sourcePower.Effects[tagId].Probability < 1 | sourcePower.Effects[tagId].ValidateConditional("Active", "Combo"),
                        sourcePower.Effects[tagId].ActiveConditionals.Count > 0, tip);
                    iList.AddItem(iItem);
                    if (sourcePower.Effects[tagId].isEnhancementEffect)
                    {
                        iList.SetUnique();
                    }
                }
                else if (sourcePower.Effects[tagId].MezType == Enums.eMez.ToggleDrop & sourcePower.Effects[tagId].Probability > 0)
                {
                    var iValue = (sourcePower.Effects[tagId].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None
                        ? "0%"
                        : $"{sourcePower.Effects[tagId].Probability * 100}%";

                    var tip = GenerateTipFromEffect(enhancedPower, enhancedPower.Effects[tagId]);
                    var iItem = new PairedList.ItemPair($"{CapString(names[(int)sourcePower.Effects[tagId].MezType], 7)}:", iValue, false,
                        sourcePower.Effects[tagId].Probability < 1 | sourcePower.Effects[tagId].ValidateConditional("Active", "Combo"),
                        sourcePower.Effects[tagId].ActiveConditionals.Count > 0, tip);
                    iList.AddItem(iItem);
                    if (sourcePower.Effects[tagId].isEnhancementEffect)
                    {
                        iList.SetUnique();
                    }
                }
                else
                {
                    var iAlternate2 = iAlternate ?? Math.Abs(sourcePower.Effects[tagId].Duration - enhancedPower.Effects[tagId].Duration) > float.Epsilon |
                        !Enums.MezDurationEnhanceable(sourcePower.Effects[tagId].MezType) &
                        Math.Abs(enhancedPower.Effects[tagId].BuffedMag - sourcePower.Effects[tagId].BuffedMag) > float.Epsilon;
                    var iValue = (enhancedPower.Effects[tagId].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None
                        ? "0"
                        : $"Mag {Utilities.FixDP(enhancedPower.Effects[tagId].BuffedMag):####0.##}{mezDuration}";

                    var tip = GenerateTipFromEffect(enhancedPower, enhancedPower.Effects[tagId]);
                    var iItem = new PairedList.ItemPair(
                        $"{CapString(names[(int)sourcePower.Effects[tagId].MezType], 7)}:", iValue, iAlternate2,
                        sourcePower.Effects[tagId].Probability < 1,
                        sourcePower.Effects[tagId].ActiveConditionals.Count > 0, tip);
                    iList.AddItem(iItem);
                    if (sourcePower.Effects[tagId].isEnhancementEffect)
                    {
                        iList.SetUnique();
                    }
                }

                n++;
            }

            effectsCount += n;
        }

        /// <summary>
        /// Add MezResist effects to the effects tab
        /// </summary>
        ///
        /// <param name="sourcePower">Source power to fetch effects from. Can be pBase, pEnh, or a virtual set bonuses power <see cref="Build.BuildVirtualSetBonusesPower"></see></param>
        /// <param name="iList">Paired list to add items into</param>
        /// <param name="specialEffects">Set to true if sourcePower is a virtual set bonuses power, otherwise false</param>
        /// <param name="effectsCount">Number of effects processed</param>
        /// <param name="startIndex">Effect index to start processing from. Set to 0 unless processing pEnh effects to avoid duplicates</param>
        private void ProcessMezResistEffects(IPower sourcePower, ref PairedList iList, bool specialEffects, ref int effectsCount, int startIndex = 0)
        {
            var names = Enum.GetNames(typeof(Enums.eMezShort));
            var enhancedPower = specialEffects ? sourcePower : pEnh;
            for (var tagId = startIndex; tagId < sourcePower.Effects.Length; tagId++)
            {
                if (!(sourcePower.Effects[tagId].PvMode != Enums.ePvX.PvP & !MidsContext.Config.Inc.DisablePvE |
                      sourcePower.Effects[tagId].PvMode != Enums.ePvX.PvE & MidsContext.Config.Inc.DisablePvE) ||
                    !(sourcePower.Effects[tagId].EffectType == Enums.eEffectType.MezResist &
                      sourcePower.Effects[tagId].Probability > 0))
                {
                    continue;
                }

                if (sourcePower.Effects[tagId].ETModifies == Enums.eEffectType.Null)
                {
                    continue;
                }

                if (sourcePower.Effects[tagId].ActiveConditionals is {Count: > 0})
                {
                    if (!sourcePower.Effects[tagId].ValidateConditional())
                    {
                        continue;
                    }
                }

                var mezDuration = enhancedPower.Effects[tagId].Duration >= 15
                    ? $" - {Utilities.FixDP(enhancedPower.Effects[tagId].Duration)}s"
                    : "";
                var iValue = $"{sourcePower.Effects[tagId].MagPercent:####0.##}%{mezDuration}";
                if ((sourcePower.Effects[tagId].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                {
                    iValue = "0%";
                }

                var tip = GenerateTipFromEffect(enhancedPower, enhancedPower.Effects[tagId]);
                var iItem = new PairedList.ItemPair(
                    $"{CapString($"-{names[(int) sourcePower.Effects[tagId].MezType]}", 7)}:", iValue, false, false,
                    false, tip);
                iList.AddItem(iItem);
                if (sourcePower.Effects[tagId].isEnhancementEffect)
                {
                    iList.SetUnique();
                }

                effectsCount++;
            }
        }

        private int EffectsStatus(Label iLabel, PairedList iList)
        {
            /*if (pEnh.Effects.Length < pBase.Effects.Length)
            {
                var swappedFX = SwapExtraEffects(pBase.Effects, pEnh.Effects);
                pBase.Effects = (IEffect[]) swappedFX[0].Clone();
                pEnh.Effects = (IEffect[]) swappedFX[1].Clone();
            }*/

            var sFxBaseMezResist = new Enums.ShortFX();
            var sFxEnhMezResist = new Enums.ShortFX();
            var sFxBaseMez = new Enums.ShortFX();
            var sFxEnhMez = new Enums.ShortFX();
            var effectsCount = 0;
            sFxBaseMezResist.Assign(pBase.GetEffectMag(Enums.eEffectType.MezResist));
            sFxEnhMezResist.Assign(pEnh.GetEffectMag(Enums.eEffectType.MezResist));
            sFxBaseMez.Assign(pBase.GetEffectMag(Enums.eEffectType.Mez, Enums.eToWho.Unspecified, true));
            sFxEnhMez.Assign(pEnh.GetEffectMag(Enums.eEffectType.Mez, Enums.eToWho.Unspecified, true));
            sFxBaseMezResist.Multiply();
            sFxEnhMezResist.Multiply();
            iList.ValueWidth = 60;
            if (sFxBaseMezResist.Present | sFxBaseMez.Present)
            {
                iLabel.Text =
                    pBase.AffectsTarget(Enums.eEffectType.MezResist) | pBase.AffectsTarget(Enums.eEffectType.Mez)
                        ? @"Status Effects (Target)"
                        : @"Status Effects (Self)";
            }

            if (sFxBaseMez.Present)
            {
                ProcessMezEffects(pBase, ref iList, false, ref effectsCount);
            }

            if (sFxEnhMez.Present)
            {
                ProcessMezEffects(pEnh, ref iList, false, ref effectsCount, true, pBase.Effects.Length);
            }

            if (sFxBaseMezResist.Present)
            {
                ProcessMezResistEffects(pBase, ref iList, false, ref effectsCount);
            }

            if (sFxEnhMezResist.Present)
            {
                ProcessMezResistEffects(pEnh, ref iList, false, ref effectsCount, pBase.Effects.Length);
            }

            return effectsCount;
        }

        private int EffectsSummon(Label iLabel, PairedList iList)
        {
            var num1 = 0;
            var flag = iList.ItemCount == 0;
            for (var index = 0; index < pBase.Effects.Length; index++)
            {
                if (!((pBase.Effects[index].EffectType == Enums.eEffectType.EntCreate) & (pBase.Effects[index].Probability > 0)))
                {
                    continue;
                }

                if (pBase.AbsorbSummonEffects & pBase.AbsorbSummonAttributes)
                {
                    continue;
                }

                if (pBase.Effects[index].ActiveConditionals is {Count: > 0})
                {
                    if (!pBase.Effects[index].ValidateConditional())
                    {
                        continue;
                    }
                }

                var iValue = pEnh.Effects[index].SummonedEntityName;
                if (iValue.StartsWith("MastermindPets_"))
                {
                    iValue = iValue.Replace("MastermindPets_", string.Empty);
                }

                if (iValue.StartsWith("Pets_"))
                {
                    iValue = iValue.Replace("Pets_", string.Empty);
                }

                if (iValue.StartsWith("Villain_Pets_"))
                {
                    iValue = iValue.Replace("Villain_Pets_", string.Empty);
                }

                var iTip = pEnh.Effects[index].BuildEffectString().Trim();
                if ((pBase.Effects[index].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                {
                    iValue = "(suppressed)";
                }

                if (!string.IsNullOrEmpty(iTip))
                {
                    iTip += "\r\n\r\nClick to show details about this entity.";
                }

                var iItem = pEnh.Effects[index].nSummon < 0
                    ? new PairedList.ItemPair("Summon:", iValue, false, pBase.Effects[index].Probability < 1, false, iTip)
                    : new PairedList.ItemPair("Summon:", iValue, false, pBase.Effects[index].Probability < 1, false, iTip, DatabaseAPI.Database.Entities[pEnh.Effects[index].nSummon]);

                iList.AddItem(iItem);
                if (pBase.Effects[index].isEnhancementEffect)
                {
                    iList.SetUnique();
                }

                num1++;
            }

            if (num1 > 0 && flag)
            {
                iLabel.Text = @"Summoned Entities";
            }

            return num1;
        }

        private void EffectsDef()
        {
            var effectMagSum = pEnh.GetEffectMagSum(Enums.eEffectType.Defense, true);
            if (effectMagSum.Value == null)
            {
                return;
            }

            effectMagSum.Multiply();
            var flag1 = false;
            var flag2 = false;
            if (effectMagSum.Value != null)
            {
                foreach (var v in effectMagSum.Value)
                {
                    switch (v)
                    {
                        case > 0:
                            flag1 = true;
                            break;
                        case < 0:
                            flag2 = true;
                            break;
                    }
                }
            }

            var buffDebuff = !(flag1 & flag2) ? !flag1 ? !flag2 ? 0 : -1 : 1 : 1;
            var def1 = pBase.GetDef(buffDebuff).Select(e => e * 100).ToArray();
            var def2 = pEnh.GetDef(buffDebuff).Select(e => e * 100).ToArray();
            var names = Enum.GetNames(typeof(Enums.eDamage));
            
            if (pBase.AffectsTarget(Enums.eEffectType.Defense))
            {
                fx_lblHead1.Text = @"Defense (Target)";
            }
            else if (pBase.AffectsSelf(Enums.eEffectType.Defense))
            {
                fx_lblHead1.Text = @"Defense (Self)";
            }
            
            fx_List1.ValueWidth = 55;
            
            var defTypes = new List<Enums.eDamage> {
                Enums.eDamage.Smashing,
                Enums.eDamage.Lethal,
                Enums.eDamage.Fire,
                Enums.eDamage.Cold,
                Enums.eDamage.Energy,
                Enums.eDamage.Negative,
                Enums.eDamage.Psionic
            };

            if (DatabaseAPI.RealmUsesToxicDef())
            {
                defTypes.Add(Enums.eDamage.Toxic);
            }

            defTypes.AddRange(new List<Enums.eDamage>
            {
                Enums.eDamage.Melee,
                Enums.eDamage.Ranged,
                Enums.eDamage.AoE
            });

            foreach (var defType in defTypes)
            {
                if (effectMagSum.Multiple)
                {
                    effectMagSum.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Defense, defType, true));
                }

                var dmgIdentifier = (int) defType;
                var tip = GenerateTipFromEffect(pEnh, Enums.eEffectType.Defense, defType);

                fx_List1.AddItem(FastItem(names[dmgIdentifier], def1[dmgIdentifier], def2[dmgIdentifier], "%", false, true, false, false, tip));
                if (sFXCheck(effectMagSum))
                {
                    fx_List1.SetUnique();
                }
            }
        }

        private int EffectsDrh()
        {
            var hasDefEffects = pBase.HasDefEffects();
            var hasResEffects = pBase.HasResEffects();
            if (hasDefEffects)
            {
                EffectsDef();
            }

            if (hasResEffects)
            {
                EffectsRes(hasDefEffects ? 2 : 1);
            }

            return (hasDefEffects ? 1 : 0) + (hasResEffects ? 1 : 0);
        }

        private void EffectsRes(int listToUse)
        {
            var res1 = pBase.GetRes(!MidsContext.Config.Inc.DisablePvE);
            var res2 = pEnh.GetRes(!MidsContext.Config.Inc.DisablePvE);
            var dmgNames = Enum.GetNames(typeof(Enums.eDamage));
            var label = listToUse == 1 ? fx_lblHead1 : fx_lblHead2;
            var pairedList = listToUse == 1 ? fx_List1 : fx_List2;

            if (pBase.AffectsTarget(Enums.eEffectType.Resistance))
            {
                label.Text = @"Resistance (Target)";
            }
            else if (pBase.AffectsSelf(Enums.eEffectType.Resistance))
            {
                label.Text = @"Resistance (Self)";
            }

            fx_List2.ValueWidth = 55;
            var shortFx = new Enums.ShortFX();
            shortFx.Multiply();
            for (var index1 = 0; index1 < res1.Length; index1++)
            {
                res1[index1] *= 100;
            }

            for (var index1 = 0; index1 < res2.Length; index1++)
            {
                res2[index1] *= 100;
            }

            var resTypes = new List<Enums.eDamage> {
                Enums.eDamage.Smashing,
                Enums.eDamage.Lethal,
                Enums.eDamage.Fire,
                Enums.eDamage.Cold,
                Enums.eDamage.Energy,
                Enums.eDamage.Negative,
                Enums.eDamage.Psionic,
                Enums.eDamage.Toxic
            };

            foreach (var resType in resTypes)
            {
                if (shortFx.Multiple)
                {
                    shortFx.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Resistance, resType, true));
                }

                var resId = (int)resType;
                var tip = GenerateTipFromEffect(pEnh, Enums.eEffectType.Resistance, resType);
                pairedList.AddItem(FastItem(dmgNames[resId], res1[resId], res2[resId], "%", false, true, false, false, tip));
                if (sFXCheck(shortFx))
                {
                    pairedList.SetUnique();
                }
            }
        }

        private static string GenerateTipFromEffect(IPower basePower, IEffect baseFx)
        {
            return (string.Join("\n",
                        basePower.Effects
                            .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                            .Where(e =>
                                e.Value.EffectType == baseFx.EffectType &
                                e.Value.DamageType == baseFx.DamageType &
                                e.Value.MezType == baseFx.MezType &
                                e.Value.ETModifies == baseFx.ETModifies &
                                e.Value.ToWho == Enums.eToWho.Self &
                                (e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                    ? Enums.ePvX.PvE
                                    : Enums.ePvX.PvP)) &
                                (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                            .Select(e =>
                                (e.Value.BuildEffectString(false, "", false, false, false, true) +
                                 basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ",")))
                    + "\n\n"
                    + string.Join("\n",
                        basePower.Effects
                            .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                            .Where(e =>
                                e.Value.EffectType == baseFx.EffectType &
                                e.Value.DamageType == baseFx.DamageType &
                                e.Value.MezType == baseFx.MezType &
                                e.Value.ETModifies == baseFx.ETModifies &
                                e.Value.ToWho == Enums.eToWho.Target &
                                (e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                    ? Enums.ePvX.PvE
                                    : Enums.ePvX.PvP)) &
                                (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                            .Select(e =>
                                (e.Value.BuildEffectString(false, "", false, false, false, true) +
                                 basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ",")))).Trim();
        }

        private static string GenerateTipFromEffect(IPower basePower, Enums.ShortFX tag)
        {
            var effects = tag.Index.Select(e => basePower.Effects[e]).ToList();
            var effectTypes = effects.Select(e => e.EffectType).ToList();
            var effectDmgTypes = effects.Select(e => e.DamageType).ToList();
            var effectETModifies = effects.Select(e => e.ETModifies).ToList();
            var effectMezTypes = effects.Select(e => e.MezType).ToList();

            return string.Join("\n",
                       basePower.Effects
                           .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                           .Where(e =>
                               effectTypes.Contains(e.Value.EffectType) &
                               effectDmgTypes.Contains(e.Value.DamageType) &
                               effectETModifies.Contains(e.Value.ETModifies) &
                               effectMezTypes.Contains(e.Value.MezType) &
                               e.Value.ToWho == Enums.eToWho.Self &
                               (e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                   ? Enums.ePvX.PvE
                                   : Enums.ePvX.PvP)) &
                               (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                           .Select(e =>
                               (e.Value.BuildEffectString(false, "", false, false, false, true) +
                                basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ",")))
                   + "\n\n"
                   + string.Join("\n",
                       basePower.Effects
                           .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                           .Where(e =>
                               effectTypes.Contains(e.Value.EffectType) &
                               effectDmgTypes.Contains(e.Value.DamageType) &
                               effectETModifies.Contains(e.Value.ETModifies) &
                               effectMezTypes.Contains(e.Value.MezType) &
                               e.Value.ToWho == Enums.eToWho.Target &
                               (e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                   ? Enums.ePvX.PvE
                                   : Enums.ePvX.PvP)) &
                               (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                           .Select(e =>
                               (e.Value.BuildEffectString(false, "", false, false, false, true) +
                                basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ","))).Trim();
        }

        // Def/Res/Elusivity
        private static string GenerateTipFromEffect(IPower basePower, Enums.eEffectType effectType, Enums.eDamage dmgType)
        {
            return (string.Join("\n",
                        basePower.Effects
                            .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                            .Where(e =>
                                e.Value.EffectType == effectType &
                                e.Value.DamageType == dmgType &
                                e.Value.ToWho == Enums.eToWho.Self &
                                (e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                    ? Enums.ePvX.PvE
                                    : Enums.ePvX.PvP)) &
                                (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                            .Select(e => (e.Value.BuildEffectString(false, "", false, false, false, true) +
                                          basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ",")))
                    + "\n\n"
                    + string.Join("\n",
                        basePower.Effects
                            .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                            .Where(e =>
                                e.Value.EffectType == effectType &
                                e.Value.DamageType == dmgType &
                                e.Value.ToWho == Enums.eToWho.Target &
                                (e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                    ? Enums.ePvX.PvE
                                    : Enums.ePvX.PvP)) &
                                (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                            .Select(e => (e.Value.BuildEffectString(false, "", false, false, false, true) +
                                          basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ",")))).Trim();
        }

        private static PairedList.ItemPair FastItem(string title, Enums.ShortFX s1, Enums.ShortFX s2, string suffix, Enums.ShortFX tag, IPower basePower)
        {
            return FastItem(title, s1, s2, suffix, false, false, false, false, tag, basePower);
        }

        private static PairedList.ItemPair FastItem(string title, float s1, float s2, string suffix, string tip)
        {
            return FastItem(title, s1, s2, suffix, false, false, false, false, tip.Trim());
        }
        
        private static PairedList.ItemPair FastItem(string title, Enums.ShortFX s1, Enums.ShortFX s2, string suffix, bool skipBase, bool alwaysShow, bool isChance, bool isSpecial, string tip)
        {
            var iValue = Utilities.FixDP(s2.Sum) + suffix;
            PairedList.ItemPair iItem;
            if ((Math.Abs(s1.Sum) < float.Epsilon) & !alwaysShow)
            {
                iItem = new PairedList.ItemPair(string.Empty, string.Empty, false);
            }
            else if (Math.Abs(s1.Sum) < float.Epsilon)
            {
                iItem = new PairedList.ItemPair($"{title}:", string.Empty, false);
            }
            else
            {
                var iAlternate = false;
                if (Math.Abs(s1.Sum - s2.Sum) > float.Epsilon)
                {
                    if (!skipBase)
                    {
                        var iValue2 = $"({Utilities.FixDP(s2.Sum)}{suffix})";
                        iValue += iValue2.Replace("%", "");
                    }

                    iAlternate = true;
                }

                iItem = new PairedList.ItemPair(title, iValue, iAlternate, isChance, isSpecial, tip.Trim());
            }

            return iItem;
        }

        private static PairedList.ItemPair FastItem(string title, Enums.ShortFX s1, Enums.ShortFX s2, string suffix, bool skipBase, bool alwaysShow, bool isChance, bool isSpecial, Enums.ShortFX tag, IPower basePower)
        {
            var iValue = Utilities.FixDP(s2.Sum) + suffix;
            PairedList.ItemPair itemPair;
            if ((Math.Abs(s1.Sum) < float.Epsilon) & !alwaysShow)
            {
                itemPair = new PairedList.ItemPair(string.Empty, string.Empty, false);
            }
            else if (Math.Abs(s1.Sum) < float.Epsilon)
            {
                itemPair = new PairedList.ItemPair($"{title}:", string.Empty, false);
            }
            else
            {
                var iAlternate = false;
                if (Math.Abs(s1.Sum - (double)s2.Sum) > float.Epsilon)
                {
                    if (!skipBase)
                    {
                        iValue += $" ({Utilities.FixDP(s1.Sum)})";
                    }

                    iAlternate = true;
                }

                var tip = GenerateTipFromEffect(basePower, tag).Trim();
                itemPair = new PairedList.ItemPair(title, iValue, iAlternate, isChance, isSpecial, tip.Trim());
            }

            return itemPair;
        }

        private static PairedList.ItemPair FastItem(string title, float s1, float s2, string suffix, bool skipBase, bool alwaysShow, bool isChance, bool isSpecial, string tip)
        {
            var iValue = Utilities.FixDP(s2) + suffix;
            PairedList.ItemPair itemPair;
            if ((Math.Abs(s1) < float.Epsilon) & !alwaysShow)
            {
                itemPair = new PairedList.ItemPair(string.Empty, string.Empty, false);
            }
            else if (Math.Abs(s1) < float.Epsilon)
            {
                itemPair = new PairedList.ItemPair(title, string.Empty, false);
            }
            else
            {
                var iAlternate = false;
                if (Math.Abs(s1 - (double)s2) > float.Epsilon)
                {
                    if (!skipBase)
                    {
                        iValue = $"{iValue} ({Utilities.FixDP(s1)}{(iValue.EndsWith("%") ? "%" : "")})";
                    }

                    iAlternate = true;
                }

                itemPair = new PairedList.ItemPair(title, iValue, iAlternate, isChance, isSpecial, tip.Trim());
            }

            return itemPair;
        }

        private static PairedList.ItemPair FastItem(string title, float s1, float s2, string suffix, bool skipBase = false, bool alwaysShow = false, bool isChance = false, bool isSpecial = false, int tagId = -1, int maxDecimal = -1)
        {
            var iValue = maxDecimal < 0 ? Utilities.FixDP(s2) + suffix : Utilities.FixDP(s2, maxDecimal) + suffix;
            PairedList.ItemPair itemPair;
            if ((Math.Abs(s1) < float.Epsilon) & !alwaysShow)
            {
                itemPair = new PairedList.ItemPair(string.Empty, string.Empty, false);
            }
            else
            {
                var iAlternate = false;
                if (Math.Abs(s1 - (double)s2) > float.Epsilon)
                {
                    if (!skipBase)
                    {
                        iValue = $"{iValue} ({Utilities.FixDP(s1)}{(iValue.EndsWith("%") ? "%" : "")})";
                    }

                    iAlternate = true;
                }

                itemPair = new PairedList.ItemPair(title, iValue, iAlternate, isChance, isSpecial, tagId);
            }

            return itemPair;
        }

        private static PairedList.ItemPair FastItem(string title, float s1, float s2, string suffix, string tip, bool skipBase = false, bool alwaysShow = false, bool isChance = false, bool isSpecial = false, int maxDecimal = -1)
        {
            var iValue = maxDecimal < 0 ? Utilities.FixDP(s2) + suffix : Utilities.FixDP(s2, maxDecimal) + suffix;
            PairedList.ItemPair itemPair;
            if ((Math.Abs(s1) < float.Epsilon) & !alwaysShow)
            {
                itemPair = new PairedList.ItemPair(string.Empty, string.Empty, false);
            }
            else
            {
                var iAlternate = false;
                if (Math.Abs(s1 - (double)s2) > float.Epsilon)
                {
                    if (!skipBase)
                    {
                        iValue += $" ({Utilities.FixDP(s1)}{(iValue.EndsWith("%") ? "%" : "")})";
                    }

                    iAlternate = true;
                }

                itemPair = new PairedList.ItemPair(title, iValue, iAlternate, isChance, isSpecial, tip.Trim());
            }

            return itemPair;
        }

        public void FlipStage(int Index, int Enh1, int Enh2, float State, int PowerID, Enums.eEnhGrade Grade1, Enums.eEnhGrade Grade2)
        {
            using var solidBrush1 = new SolidBrush(enhListing.BackColor);
            if (pBase == null)
                return;
            var solidBrush2 = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
            if (PowerID != pBase.PowerIndex)
                return;
            ImageAttributes recolorIa = clsDrawX.GetRecolorIa(MidsContext.Character.IsHero());
            var rectangle1 = new Rectangle();
            ref var local1 = ref rectangle1;
            var size = bxFlip.Size;
            var x = size.Width - 188 + 30 * Index;
            size = bxFlip.Size;
            var y1 = (int)Math.Round((size.Height / 2.0 - 30.0) / 2.0);
            local1 = new Rectangle(x, y1, 30, 30);
            var destRect = rectangle1;
            bxFlip.Graphics.FillRectangle(solidBrush1, rectangle1);
            var rectangle2 = new Rectangle((int)Math.Round(rectangle1.X + (30.0 - 30.0 * State) / 2.0), rectangle1.Y,
                (int)Math.Round(30.0 * State), 30);
            Graphics graphics;
            if (Enh1 > -1)
            {
                graphics = bxFlip.Graphics;
                I9Gfx.DrawFlippingEnhancement(ref graphics, rectangle1, State,
                    DatabaseAPI.Database.Enhancements[Enh1].ImageIdx,
                    I9Gfx.ToGfxGrade(DatabaseAPI.Database.Enhancements[Enh1].TypeID, Grade1));
            }
            else
            {
                bxFlip.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, rectangle2, 0, 0, 30, 30, GraphicsUnit.Pixel);
            }

            pnlEnhActive.CreateGraphics().DrawImage(bxFlip.Bitmap, destRect, rectangle1, GraphicsUnit.Pixel);
            ref var local2 = ref rectangle1;
            double y2 = rectangle1.Y;
            size = bxFlip.Size;
            var num1 = size.Height / 2.0;
            var num2 = (int)Math.Round(y2 + num1);
            local2.Y = num2;
            bxFlip.Graphics.FillRectangle(solidBrush1, rectangle1);
            rectangle2 = new Rectangle((int)Math.Round(rectangle1.X + (30.0 - 30.0 * State) / 2.0), rectangle1.Y,
                (int)Math.Round(30.0 * State), 30);
            if (Enh2 > -1)
            {
                graphics = bxFlip.Graphics;
                I9Gfx.DrawFlippingEnhancement(ref graphics, rectangle1, State,
                    DatabaseAPI.Database.Enhancements[Enh2].ImageIdx,
                    I9Gfx.ToGfxGrade(DatabaseAPI.Database.Enhancements[Enh2].TypeID, Grade2));
            }
            else
            {
                bxFlip.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, rectangle2, 0, 0, 30, 30, GraphicsUnit.Pixel);
            }

            rectangle2.Inflate(2, 2);
            bxFlip.Graphics.FillEllipse(solidBrush2, rectangle2);
            pnlEnhInactive.CreateGraphics().DrawImage(bxFlip.Bitmap, destRect, rectangle1, GraphicsUnit.Pixel);
        }

        private static string ConvertNewlinesToRTF(string str)
        {
            return str
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", RTF.Crlf());
        }

        private static string GetEnhancementStringLongRTF(I9Slot iEnh)
        {
            var str = iEnh.GetEnhancementStringLong();
            if (!string.IsNullOrEmpty(str))
                str = RTF.Color(RTF.ElementID.Enhancement) + RTF.Italic(ConvertNewlinesToRTF(str)) + RTF.Color(RTF.ElementID.Text);

            return str;
        }

        private static string GetEnhancementStringRTF(I9Slot iEnh)
        {
            var str = iEnh.GetEnhancementString();
            if (!string.IsNullOrEmpty(str))
                str = RTF.Color(RTF.ElementID.Enhancement) + ConvertNewlinesToRTF(str) + RTF.Color(RTF.ElementID.Text);

            return str;
        }

        private PairedList.ItemPair GetRankedEffect(int[] Index, int ID)
        {
            var title = string.Empty;
            var shortFxBase = new Enums.ShortFX();
            var shortFxEnh = new Enums.ShortFX();
            var tag2 = new Enums.ShortFX();
            var suffix = string.Empty;
            var enhancedPower = pEnh ?? pBase;
            var fx = pEnh != null && Index[ID] < pEnh.Effects.Length
                ? pEnh.Effects[Index[ID]]
                : Index[ID] < pBase.Effects.Length
                    ? pBase.Effects[Index[ID]]
                        : null;

            var fx2 = ID <= 0
                ? null
                : pEnh != null && Index[ID - 1] < pEnh.Effects.Length
                    ? pEnh.Effects[Index[ID - 1]]
                    : Index[ID - 1] < pBase.Effects.Length
                        ? pBase.Effects[Index[ID - 1]]
                        : null;

            if (fx == null)
            {
                return FastItem("", 0f, 0f, string.Empty);
            }

            if (Index[ID] > -1)
            {
                var flag = false;
                var onlySelf = fx.ToWho == Enums.eToWho.Self;
                var onlyTarget = fx.ToWho == Enums.eToWho.Target;
                if (ID > 0)
                {
                    flag = (fx.EffectType == fx2.EffectType) &
                           (fx.ToWho == Enums.eToWho.Self) &
                           (fx2.ToWho == Enums.eToWho.Self) &
                           (fx.ToWho == Enums.eToWho.Target);
                }

                if (fx.DelayedTime > 5)
                {
                    flag = true;
                }

                var names = Enum.GetNames(typeof(Enums.eEffectTypeShort));
                if (fx.EffectType == Enums.eEffectType.Enhancement)
                {
                    title = fx.ETModifies switch
                    {
                        Enums.eEffectType.EnduranceDiscount => "+EndRdx",
                        Enums.eEffectType.RechargeTime => "+Rechg",
                        Enums.eEffectType.Mez => fx.MezType == Enums.eMez.None
                            ? "+Effects"
                            : $"Enh({Enum.GetName(Enums.eMezShort.None.GetType(), fx.MezType)})",
                        Enums.eEffectType.Defense => "Enh(Def)",
                        Enums.eEffectType.Resistance => "Enh(Res)",
                        _ => CapString(Enum.GetName(fx.ETModifies.GetType(), fx.ETModifies), 7)
                    };

                    shortFxBase.Assign(pBase.GetEffectMagSum(fx.EffectType,
                        fx.ETModifies, fx.DamageType,
                        fx.MezType, false, onlySelf, onlyTarget));

                    shortFxEnh.Assign(enhancedPower.GetEffectMagSum(enhancedPower.Effects[Index[ID]].EffectType,
                        enhancedPower.Effects[Index[ID]].ETModifies, enhancedPower.Effects[Index[ID]].DamageType,
                        enhancedPower.Effects[Index[ID]].MezType, false, onlySelf, onlyTarget));
                }
                else
                {
                    title = fx.EffectType != Enums.eEffectType.Mez
                        ? names[(int)fx.EffectType]
                        : Enums.GetMezName((Enums.eMezShort)fx.MezType);
                }

                var temp = string.Empty;
                switch (fx.EffectType)
                {
                    case Enums.eEffectType.HitPoints:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.HitPoints, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.HitPoints, false, onlySelf, onlyTarget));
                        tag2.Assign(shortFxBase);
                        shortFxBase.Sum = (float)(shortFxBase.Sum / (double)MidsContext.Archetype.Hitpoints * 100);
                        shortFxEnh.Sum = (float)(shortFxEnh.Sum / (double)MidsContext.Archetype.Hitpoints * 100);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Heal:
                        if (fx.BuffedMag <= 1)
                        {
                            temp = $"{fx.BuffedMag:P2}";
                            shortFxBase.Add(Index[ID], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(Index[ID], Convert.ToSingle(temp.Replace("%", "")));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Heal, false, onlySelf, onlyTarget));
                            shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Heal, false, onlySelf, onlyTarget));
                            shortFxBase.Sum = (float)(shortFxBase.Sum / (double)MidsContext.Archetype.Hitpoints * 100);
                            shortFxEnh.Sum = (float)(shortFxEnh.Sum / (double)MidsContext.Archetype.Hitpoints * 100);
                            tag2.Assign(shortFxBase);
                        }
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Absorb:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Absorb, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Absorb, false, onlySelf, onlyTarget));
                        var absorbPercent = pBase.Effects
                            .Where(e => e.EffectType == Enums.eEffectType.Absorb)
                            .Any(e => e.DisplayPercentage);
                        tag2.Assign(shortFxBase);
                        suffix = absorbPercent ? "%" : "";
                        break;
                    case Enums.eEffectType.Endurance:
                        if (fx.BuffedMag < -0.01 && fx.BuffedMag > -1)
                        {
                            temp = $"{fx.BuffedMag:P2}";
                            shortFxBase.Add(Index[ID], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(Index[ID], Convert.ToSingle(temp.Replace("%", "")));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Endurance, false, onlySelf, onlyTarget));
                            shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Endurance, false, onlySelf, onlyTarget));
                            tag2.Assign(shortFxBase);
                        }
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Regeneration:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Regeneration, false, onlySelf, onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Regeneration, false, onlySelf, onlyTarget));
                        shortFxEnh.Sum *= 100;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Null:
                        if (fx.BuffedMag < 1)
                        {
                            temp = $"{fx.BuffedMag:P2}";
                            shortFxBase.Add(Index[ID], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(Index[ID], Convert.ToSingle(temp.Replace("%", "")));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Null, false, onlySelf, onlyTarget));
                            shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Null, false, onlySelf, onlyTarget));
                            tag2.Assign(shortFxBase);
                        }
                        suffix = "%";
                        break;
                    case Enums.eEffectType.ToHit:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.ToHit, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.ToHit, false, onlySelf, onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Fly:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Fly, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Fly, false, onlySelf, onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Recovery:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Recovery, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Recovery, false, onlySelf, onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Mez when fx.MezType is Enums.eMez.Taunt or Enums.eMez.Placate:
                        shortFxBase.Add(Index[ID], fx.Duration);
                        shortFxEnh.Add(Index[ID], enhancedPower.Effects[Index[ID]].Duration);
                        tag2.Assign(shortFxBase);
                        suffix = "s";
                        break;

                    // Set list of effects below that are treated as percentages
                    // Base and enhanced values will be multiplied by 100
                    case Enums.eEffectType.DamageBuff:
                    case Enums.eEffectType.Defense:
                    case Enums.eEffectType.Resistance:
                    case Enums.eEffectType.ResEffect:
                    case Enums.eEffectType.Enhancement:
                    case Enums.eEffectType.MezResist:
                    case Enums.eEffectType.RechargeTime:
                    case Enums.eEffectType.SpeedFlying:
                    case Enums.eEffectType.SpeedRunning:
                    case Enums.eEffectType.SpeedJumping:
                    case Enums.eEffectType.JumpHeight:
                    case Enums.eEffectType.PerceptionRadius:
                    case Enums.eEffectType.Meter:
                    case Enums.eEffectType.Range:
                    case Enums.eEffectType.MaxFlySpeed:
                    case Enums.eEffectType.MaxRunSpeed:
                    case Enums.eEffectType.MaxJumpSpeed:
                    case Enums.eEffectType.Jumppack:
                    case Enums.eEffectType.GlobalChanceMod:
                        if (fx.EffectType != Enums.eEffectType.Enhancement)
                        {
                            shortFxBase.Add(Index[ID], fx.BuffedMag);
                            shortFxEnh.Add(Index[ID], enhancedPower.Effects[Index[ID]].BuffedMag);
                        }

                        shortFxBase.Multiply();
                        shortFxEnh.Multiply();

                        tag2.Assign(enhancedPower.GetEffectMagSum(fx.EffectType, false, onlySelf, onlyTarget));
                        break;
                    case Enums.eEffectType.SilentKill:
                        shortFxBase.Add(Index[ID], fx.Absorbed_Duration);
                        shortFxEnh.Add(Index[ID], enhancedPower.Effects[Index[ID]].Absorbed_Duration);
                        tag2.Assign(shortFxBase);
                        break;
                    default:
                        shortFxBase.Add(Index[ID], fx.BuffedMag);
                        shortFxEnh.Add(Index[ID], enhancedPower.Effects[Index[ID]].BuffedMag);
                        tag2.Assign(shortFxBase);
                        break;
                }

                if (fx.DisplayPercentage)
                {
                    suffix = "%";
                }

                suffix += fx.ToWho switch
                {
                    Enums.eToWho.Target => " (Tgt)",
                    Enums.eToWho.Self => " (Self)",
                    _ => ""
                };

                if (flag)
                {
                    return FastItem("", 0f, 0f, string.Empty);
                }
            }

            for (var index = 0; index < shortFxEnh.Index.Length; index++)
            {
                var sFxIdx = shortFxEnh.Index[index];
                if (sFxIdx >= pBase.Effects.Length & sFxIdx >= pEnh.Effects.Length)
                {
                    continue;
                }

                var effect = sFxIdx < pBase.Effects.Length
                    ? pBase.Effects[sFxIdx]
                    : pEnh.Effects[sFxIdx];

                if (sFxIdx <= -1 || !effect.DisplayPercentage)
                {
                    continue;
                }

                if (shortFxEnh.Value[index] > 1)
                {
                    continue;
                }

                switch (effect.EffectType)
                {
                    case Enums.eEffectType.Absorb:
                        //Fixes the Absorb display to correctly show the percentage
                        shortFxEnh.Sum = float.Parse(shortFxEnh.Sum.ToString("P", CultureInfo.InvariantCulture).Replace("%", ""));
                        break;
                    case Enums.eEffectType.ToHit:
                        //Fixes the ToHit display to correctly show the percentage
                        if (effect.Stacking == Enums.eStacking.Yes)
                        {
                            var overage = fx.Ticks * 0.05f;
                            shortFxEnh.Sum -= overage;
                            shortFxEnh.Sum /= 2;
                        }

                        break;
                    default:
                        shortFxEnh.ReSum();
                        break;
                }

                break;
            }

            // shortFxEnh.Index.Length == 0 will occur if all effects of the same kind
            // have non validated conditionals.
            // E.g. -Recovery on Kick if Cross Punch has not been picked.
            var tip = shortFxEnh.Index.Length <= 0
                ? ""
                : pEnh.BuildTooltipStringAllVectorsEffects(pEnh.Effects[shortFxEnh.Index[0]].EffectType,
                pEnh.Effects[shortFxEnh.Index[0]].ETModifies, pEnh.Effects[shortFxEnh.Index[0]].DamageType,
                pEnh.Effects[shortFxEnh.Index[0]].MezType);

            if (fx.ActiveConditionals.Count > 0)
            {
                return FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, fx.Probability < 1, fx.ActiveConditionals.Count > 0, tip);
            }

            if (fx.SpecialCase != Enums.eSpecialCase.None)
            {
                return FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, fx.Probability < 1, fx.SpecialCase != Enums.eSpecialCase.None, tip);
            }

            return FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, fx.Probability < 1, false, tip);
        }

        public void Init()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
        }

        private static bool IsMezEffect(string iStr)

        {
            var names = Enum.GetNames(Enums.eMez.None.GetType());
            var num = names.Length - 1;
            for (var index = 0; index <= num; ++index)
                if (string.Equals(iStr, names[index], StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }

        private void lblFloat_Click(object sender, EventArgs e)

        {
            var floatChange = FloatChange;
            floatChange?.Invoke();
        }

        private void lblLock_Click(object sender, EventArgs e)

        {
            var unlockClick = Unlock_Click;
            unlockClick?.Invoke();
            lblLock.Visible = false;
            pnlTabs.Select();
        }

        private void lblShrink_Click(object sender, EventArgs e)

        {
            if (Compact)
                ResetSize();
            else
                CompactSize();
        }

        private void lblShrink_DoubleClick(object sender, EventArgs e)
        {
            lblShrink_Click(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private int miniGetEnhIndex(int iX, int iY)
        {
            var num1 = bxFlip.Size.Width - 188;
            if (pBase == null)
                return -1;
            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(pBase.PowerIndex);
            if (inToonHistory < 0)
                return -1;
            var num2 = MidsContext.Character.CurrentBuild.Powers[inToonHistory].SlotCount - 1;
            for (var index = 0; index <= num2; ++index)
            {
                var rectangle = new Rectangle(num1 + 30 * index,
                    (int)Math.Round((bxFlip.Size.Height / 2.0 - 30.0) / 2.0), 30, 30);
                if ((iX > rectangle.X) & (iX < rectangle.X + rectangle.Width) &&
                    (iY > rectangle.Y) & (iY < rectangle.Y + rectangle.Height))
                    return index;
            }

            return -1;
        }

        private void PairedList_Hover(object sender, Enums.ShortFX tag, string tooltip)
        {
            var empty1 = string.Empty;
            var str1 = string.Empty;
            if (tag.Present)
            {
                var empty2 = string.Empty;
                IPower power = new Power(pEnh);
                foreach (var t in tag.Index)
                {
                    if (t == -1 || power.Effects[t].EffectType == Enums.eEffectType.None)
                    {
                        continue;
                    }

                    var empty3 = string.Empty;
                    var returnMask = Array.Empty<int>();
                    power.GetEffectStringGrouped(t, ref empty3, ref returnMask, false, false);
                    if (returnMask.Length <= 0)
                    {
                        continue;
                    }

                    if (empty2 != string.Empty)
                    {
                        empty2 += "\r\n";
                    }

                    empty2 += empty3;
                    foreach (var m in returnMask)
                    {
                        power.Effects[m].EffectType = Enums.eEffectType.None;
                    }
                }

                foreach (var t in tag.Index)
                {
                    if (power.Effects[t].EffectType == Enums.eEffectType.None)
                    {
                        continue;
                    }

                    if (empty2 != string.Empty)
                    {
                        empty2 += "\r\n";
                    }

                    empty2 += power.Effects[t].BuildEffectString();
                }

                str1 = empty1 + empty2;
            }
            else if (string.IsNullOrWhiteSpace(tooltip))
            {
                str1 = string.Empty;
            }
            else
            {
                str1 = tooltip;
            }

            if (!string.IsNullOrWhiteSpace(str1))
            {
                dbTip.SetToolTip((Control) sender, str1);
            }
            else
            {
                dbTip.SetToolTip((Control) sender, string.Empty);
            }
        }

        private void PairedList_ItemOut(object sender)
        {
            dbTip.SetToolTip((Control)sender, string.Empty);
        }


        private void pnlEnhActive_MouseClick(object sender, MouseEventArgs e)
        {
            if (pBase == null || e.Button != MouseButtons.Left)
                return;
            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(pBase.PowerIndex);
            if (inToonHistory <= -1)
                return;
            var slotFlip = SlotFlip;
            slotFlip?.Invoke(inToonHistory);
        }

        private void pnlEnhActive_MouseMove(object sender, MouseEventArgs e)
        {
            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(pBase.PowerIndex);
            var enhIndex = miniGetEnhIndex(e.X, e.Y);
            if (enhIndex <= -1)
                return;
            SetEnhancement(MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[enhIndex].Enhancement,
                MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[enhIndex].Level);
        }

        private void pnlEnhActive_Paint(object sender, PaintEventArgs e)
        {
            RedrawFlip();
        }

        private void pnlEnhInactive_MouseClick(object sender, MouseEventArgs e)
        {
            if (pBase == null || e.Button != MouseButtons.Left)
                return;
            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(pBase.PowerIndex);
            if (inToonHistory <= -1)
                return;
            var slotFlip = SlotFlip;
            slotFlip?.Invoke(inToonHistory);
        }

        private void pnlEnhInactive_MouseMove(object sender, MouseEventArgs e)
        {
            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(pBase.PowerIndex);
            var enhIndex = miniGetEnhIndex(e.X, e.Y);
            if (enhIndex <= -1)
                return;
            SetEnhancement(MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[enhIndex].FlippedEnhancement,
                MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[enhIndex].Level);
        }

        private void pnlEnhInactive_Paint(object sender, PaintEventArgs e)
        {
            RedrawFlip();
        }

        private void pnlTabs_MouseDown(object sender, MouseEventArgs e)
        {
            var clipRect = new Rectangle(0, 0, 70, pnlTabs.Height);
            var index = 0;
            while (!(e.X >= clipRect.X & e.X <= clipRect.Width + clipRect.X))
            {
                clipRect.X += clipRect.Width;
                ++index;
                if (index > 3)
                {
                    return;
                }
            }

            if (TabsMask != null && TabsMask.Length > index)
            {
                if (!TabsMask[index])
                {
                    return;
                }
            }

            if (index != TabPage)
            {
                var tabChanged = TabChanged;
                tabChanged?.Invoke(index);
            }

            TabPage = index;
            pnlTabs_Paint(this, new PaintEventArgs(pnlTabs.CreateGraphics(), clipRect));
        }

        private void pnlTabs_Paint(object sender, PaintEventArgs e)
        {
            DoPaint();
        }

        private void PowerScaler_BarClick(float Value)
        {
            var num = (int)Math.Round(Value);
            if (num < pBase.VariableMin)
                num = pBase.VariableMin;
            if (num > pBase.VariableMax)
                num = pBase.VariableMax;
            MidsContext.Character.CurrentBuild.Powers[HistoryIDX].VariableValue = num;
            MidsContext.Character.CurrentBuild.Powers[HistoryIDX].Power.Stacks = num;
            /*foreach (var effect in MidsContext.Character.CurrentBuild.Powers[HistoryIDX].Power.Effects)
            {
                effect.UpdateAttrib();
                DisplayInfo();
            }*/
            if (num == pLastScaleVal)
                return;
            SetPowerScaler();
            pLastScaleVal = num;
            MainModule.MidsController.Toon.GenerateBuffedPowerArray();
            var slotUpdate = SlotUpdate;
            slotUpdate?.Invoke();
        }

        private void RedrawFlip()
        {
            if (bxFlip == null)
                DisplayFlippedEnhancements();
            var srcRect = new Rectangle(0, 0, pnlEnhActive.Width, pnlEnhActive.Height);
            var destRect = new Rectangle(0, 0, pnlEnhActive.Width, pnlEnhActive.Height);
            pnlEnhActive.CreateGraphics().DrawImage(bxFlip.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
            srcRect = new Rectangle(0, pnlEnhActive.Height, pnlEnhInactive.Width, pnlEnhInactive.Height);
            pnlEnhInactive.CreateGraphics().DrawImage(bxFlip.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
        }

        private void ResetSize()
        {
            var size = Size;
            info_txtSmall.Height = 32;
            Info_txtLarge.Top = info_txtSmall.Bottom + 4;
            if (PowerScaler.Visible)
            {
                Info_txtLarge.Height = 100 - PowerScaler.Height;
                PowerScaler.Top = Info_txtLarge.Bottom;
                info_DataList.Top = PowerScaler.Bottom + 4;
            }
            else
            {
                Info_txtLarge.Height = 100;
                PowerScaler.Top = Info_txtLarge.Bottom - PowerScaler.Height;
                info_DataList.Top = Info_txtLarge.Bottom + 4;
            }
            //Controls Dataview Sizing for Info panel.
            /*Info_Damage.ColorBackEnd = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            Info_Damage.ColorBackStart = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            */
            Info_Damage.ColorBackStart = Color.Black;
            Info_Damage.ColorBackEnd = Color.FromArgb(64, 0, 0);

            Info_Damage.ColorBaseEnd = MidsContext.Config.RtFont.ColorDamageBarBase;
            Info_Damage.ColorBaseStart = MidsContext.Config.RtFont.ColorDamageBarBase;
            Info_Damage.ColorEnhEnd = MidsContext.Config.RtFont.ColorDamageBarEnh;
            Info_Damage.ColorEnhStart = MidsContext.Config.RtFont.ColorDamageBarEnh;

            info_DataList.Height = 104;
            lblDmg.Visible = true;
            lblDmg.Top = info_DataList.Bottom + 4;
            Info_Damage.Top = lblDmg.Bottom + 4;
            Info_Damage.PaddingV = 1;
            Info_Damage.Height = 75;
            enhListing.Height = Info_Damage.Bottom - (enhListing.Top + (pnlEnhActive.Height + 4) * 2);
            pnlEnhActive.Top = enhListing.Bottom + 4;
            pnlEnhInactive.Top = pnlEnhActive.Bottom + 4;
            pnlInfo.Height = Info_Damage.Bottom + 20;
            pnlEnh.Height = pnlInfo.Height;
            Height = pnlInfo.Bottom;
            Compact = false;
            if (!(Size != size))
                return;
            var sizeChange = SizeChange;
            sizeChange?.Invoke(Size, Compact);
        }

        private void SetBackColor()
        {
            info_Title.BackColor = Color.Black;
            Info_txtLarge.BackColor = Color.Black;
            info_txtSmall.BackColor = Color.Black;
            info_DataList.BackColor = Color.Black;
            Info_Damage.BackColor = Color.Black;
            fx_List1.BackColor = Color.Black;
            fx_List2.BackColor = Color.Black;
            fx_List3.BackColor = Color.Black;
            fx_Title.BackColor = Color.Black;
            total_Misc.BackColor = Color.Black;
            total_Title.BackColor = Color.Black;
            enhListing.BackColor = Color.Black;
            Enh_Title.BackColor = Color.Black;
            enhNameDisp.BackColor = Color.Black;
            lblFloat.BackColor = Color.Black;
            lblShrink.BackColor = Color.Black;
            Info_Damage.ColorBackStart = Color.Black;
            Info_Damage.ColorBackEnd = Color.FromArgb(64, 0, 0);

            /*info_Title.BackColor = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            Info_txtLarge.BackColor = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            info_txtSmall.BackColor = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            info_DataList.BackColor = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            Info_Damage.BackColor = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            fx_List1.BackColor = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            fx_List2.BackColor = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            fx_List3.BackColor = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            fx_Title.BackColor = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            total_Misc.BackColor = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            total_Title.BackColor = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            enhListing.BackColor = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            Enh_Title.BackColor = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            enhNameDisp.BackColor = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            */
            DoPaint();
            /*lblFloat.BackColor = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            lblShrink.BackColor = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            Info_Damage.ColorBackEnd = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            Info_Damage.ColorBackStart = VillainColor
                ? MidsContext.Config.RtFont.ColorBackgroundVillain
                : MidsContext.Config.RtFont.ColorBackgroundHero;
            */
            info_DataList.Draw();
            Info_Damage.Draw(); //Drawing controls
            fx_List1.Draw();
            fx_List2.Draw();
            fx_List3.Draw();
            total_Misc.Draw();
            enhListing.Draw();
        }

        private void SetDamageTip()
        {
            var iTip = pEnh == null ? "" : pEnh.GetDamageTip();
            Info_Damage.SetTip(iTip);
        }

        private Power GetPowerRedirectParent(IPower pSrc)
        {
            var pSrcRedirectParent = new Power();
            pSrcRedirectParent.FullName = "";
            foreach (var p in DatabaseAPI.Database.Power)
            {
                if (p.GetPowerSet().SetType != Enums.ePowerSetType.Primary &
                    p.GetPowerSet().SetType != Enums.ePowerSetType.Secondary &
                    p.GetPowerSet().SetType != Enums.ePowerSetType.Pool &
                    p.GetPowerSet().SetType != Enums.ePowerSetType.Ancillary)
                {
                    continue;
                }

                foreach (var fx in p.Effects)
                {
                    if (fx.EffectType != Enums.eEffectType.PowerRedirect)
                        continue;

                    if (fx.Override != pSrc.FullName)
                        continue;

                    pSrcRedirectParent = new Power(DatabaseAPI.GetPowerByFullName(p.FullName));
                }
            }

            return pSrcRedirectParent.FullName == "" ? null : pSrcRedirectParent;
        }

        public void SetData(IPower? basePower, IPower enhancedPower, bool noLevel = false, bool locked = false, int iHistoryIdx = -1)
        {
            if (basePower == null)
            {
                return;
            }

            Lock = locked;

            var basePowerData = new Power(basePower);
            var enhancedPowerData = new Power(enhancedPower);

            if (enhancedPowerData.PowerIndex == -1 & basePowerData.PowerIndex == -1)
            {
                pBase = null;
            }
            else if (enhancedPowerData.PowerIndex == -1 & basePowerData.PowerIndex > -1)
            {
                pBase = basePowerData;
            }
            else
            {
                pBase = new Power(DatabaseAPI.Database.Power[enhancedPowerData.PowerIndex]);
            }

            pEnh = enhancedPowerData.PowerIndex == -1
                ? new Power(basePower) {PowerIndex = -1}
                : enhancedPowerData;

            // Data sent to the Dataview may differ from DB.
            // Not needed if ActivatePeriod absorb from summons is disabled in Power.AbsorbPetEffects()
            /*var dbPower = DatabaseAPI.GetPowerByFullName(pBase.FullName);
            if (dbPower != null)
            {
                pBase.ActivatePeriod = dbPower.ActivatePeriod;
                pEnh.ActivatePeriod = dbPower.ActivatePeriod;
            }*/

            pBase?.ProcessExecutes();
            pBase?.AbsorbPetEffects();
            pEnh?.ProcessExecutes();
            //pEnh?.AbsorbPetEffects(); // Done already during calculations (see clsToonX.GenerateBuffedPowerArray())

            HistoryIDX = iHistoryIdx;
            SetDamageTip();
            DisplayData(noLevel);
            SizeRefresh();
        }

        public void SetEnhancement(I9Slot iEnh, int iLevel = -1)
        {
            if (Lock & (TabPage != 3) || iLevel < 0)
            {
                return;
            }

            string str1;
            if (iEnh.Enh > -1)
            {
                str1 = DatabaseAPI.Database.Enhancements[iEnh.Enh].LongName;
                if ((str1.Length > 38) & (iLevel > -1))
                    str1 = DatabaseAPI.GetEnhancementNameShortWSet(iEnh.Enh);
            }
            else
            {
                str1 = pBase.DisplayName;
                info_txtSmall.Rtf = RTF.StartRTF() + pBase.DescShort + "\r\n" + RTF.Color(RTF.ElementID.Faded) +
                                    "Shift+Click to move slot. Right-Click to place enh." + RTF.EndRTF();
            }

            if ((iLevel > -1) & !MidsContext.Config.ShowSlotLevels)
            {
                str1 += $" (Slot Level {iLevel + 1})";
            }

            info_Title.Text = str1;
            fx_Title.Text = str1;
            enhNameDisp.Text = str1;
            if (TabPage > 1 || iEnh.Enh < 0)
                return;
            var iStr1 = string.Empty;
            var str2 = string.Empty;
            if ((DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.InventO) | (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.SetO))
            {
                iStr1 = RTF.Color(RTF.ElementID.Invention) + "Invention Level: " + Convert.ToString(iEnh.IOLevel + 1) + Enums.GetRelativeString(iEnh.RelativeLevel, false) + RTF.Color(RTF.ElementID.Text);
            }

            if ((DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.SetO) | (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.SpecialO))
            {
                if (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.SetO)
                {
                    if (DatabaseAPI.Database.Enhancements[iEnh.Enh].Unique)
                    {
                        iStr1 = iStr1 + RTF.Color(RTF.ElementID.Warning) + " (Unique) " + RTF.Color(RTF.ElementID.Text);
                    }

                    if ((DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance < 1.0) & (DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance > 0.0))
                    {

                        str2 = $"{str2}{RTF.Color(RTF.ElementID.Enhancement)}{Convert.ToDecimal(DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance * 100.0):#0.##)} % chance of ";
                    }
                }
                else
                {
                    iStr1 = iStr1 + RTF.Color(RTF.ElementID.Enhancement) + "Hamidon/Synthetic Hamidon Origin Enhancement";
                }
            }
            else
            {
                if (iStr1 != string.Empty)
                    iStr1 += " - ";
                iStr1 += GetEnhancementStringRTF(iEnh);
            }

            string iStr2;
            if (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.SetO)
            {
                iStr2 = str2 + GetEnhancementStringLongRTF(iEnh) + "\r\n" + EnhancementSetCollection.GetSetInfoLongRTF(DatabaseAPI.Database.Enhancements[iEnh.Enh].nIDSet);
            }
            else
            {
                var str3 = str2 + DatabaseAPI.Database.Enhancements[iEnh.Enh].Desc;
                if (str3 != string.Empty)
                    str3 += "\r\n";
                iStr2 = str3 + GetEnhancementStringLongRTF(iEnh);
            }

            info_txtSmall.Rtf = RTF.StartRTF() + RTF.ToRTF(iStr1) + RTF.Crlf() + RTF.Color(RTF.ElementID.Faded) + "Shift+Click to move slot. Right-Click to place enh." + RTF.EndRTF();
            Info_txtLarge.Rtf = RTF.StartRTF() + RTF.ToRTF(iStr2) + RTF.EndRTF();
        }

        public void SetEnhancementPicker(I9Slot iEnh)
        {
            if (iEnh.Enh < 0)
                info_Title.Text = "No Enhancement";
            info_Title.Text = DatabaseAPI.Database.Enhancements[iEnh.Enh].LongName;
            if (iEnh.Enh < 0)
                return;
            var str1 = string.Empty;
            var iStr1 = string.Empty;
            if ((DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.InventO) |
                (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.SetO))
                iStr1 = RTF.Color(RTF.ElementID.Invention) + "Invention Level: " + Convert.ToString(iEnh.IOLevel + 1) +
                        Enums.GetRelativeString(iEnh.RelativeLevel, false) + RTF.Color(RTF.ElementID.Text);
            if ((DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.SetO) |
                (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.SpecialO))
            {
                if (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.SetO)
                {
                    if (DatabaseAPI.Database.Enhancements[iEnh.Enh].Unique)
                        iStr1 = iStr1 + RTF.Color(RTF.ElementID.Warning) + " (Unique) " + RTF.Color(RTF.ElementID.Text);
                    if ((DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance > 1.0) & (DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance > 0.0))
                    {
                        str1 = $"{str1}{RTF.Color(RTF.ElementID.Enhancement)}{Convert.ToDecimal(DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance * 100.0):#0.##)} % chance of ";
                    }
                }
                else
                {
                    iStr1 += "Hamidon/Synthetic Hamidon Origin Enhancement";
                }
            }
            else
            {
                if (iStr1 != string.Empty)
                    iStr1 += " - ";
                iStr1 += GetEnhancementStringRTF(iEnh);
            }

            string iStr2;
            if (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.SetO)
            {
                // Fix strange white "-2" showing at the end of the enhancement long text
                /*iStr2 = str1 + GetEnhancementStringLongRTF(iEnh) + RTF.Size(RTF.SizeID.Tiny) + "\r\n" +
                        EnhancementSetCollection.GetSetInfoLongRTF(DatabaseAPI.Database.Enhancements[iEnh.Enh].nIDSet);*/

                iStr2 = str1 + GetEnhancementStringLongRTF(iEnh) + "\r\n" +
                        EnhancementSetCollection.GetSetInfoLongRTF(DatabaseAPI.Database.Enhancements[iEnh.Enh].nIDSet);
            }
            else
            {
                var str2 = str1 + DatabaseAPI.Database.Enhancements[iEnh.Enh].Desc;
                if (str2 != string.Empty)
                    str2 += "\r\n";
                iStr2 = str2 + GetEnhancementStringLongRTF(iEnh);
            }

            info_txtSmall.Rtf = RTF.StartRTF() + RTF.ToRTF(iStr1) + RTF.Crlf() + RTF.EndRTF();
            Info_txtLarge.Rtf = RTF.StartRTF() + RTF.ToRTF(iStr2) + RTF.EndRTF();
        }

        public void SetFontData()
        {
            var style = !MidsContext.Config.RtFont.PairedBold ? FontStyle.Regular : FontStyle.Bold;
            info_DataList.Font = new Font(info_DataList.Font.FontFamily, MidsContext.Config.RtFont.PairedBase);
            fx_List1.Font = new Font(fx_List1.Font.FontFamily, MidsContext.Config.RtFont.PairedBase);
            fx_List2.Font = new Font(fx_List2.Font.FontFamily, MidsContext.Config.RtFont.PairedBase);
            fx_List3.Font = new Font(fx_List3.Font.FontFamily, MidsContext.Config.RtFont.PairedBase);
            total_Misc.Font = new Font(total_Misc.Font.FontFamily, MidsContext.Config.RtFont.PairedBase);
            enhListing.Font = new Font(enhListing.Font.FontFamily, MidsContext.Config.RtFont.PairedBase);
            pnlEnhActive.Font = new Font(enhListing.Font.FontFamily, MidsContext.Config.RtFont.PairedBase);
            info_DataList.ForceBold = MidsContext.Config.RtFont.PairedBold;
            fx_List1.ForceBold = MidsContext.Config.RtFont.PairedBold;
            fx_List2.ForceBold = MidsContext.Config.RtFont.PairedBold;
            fx_List3.ForceBold = MidsContext.Config.RtFont.PairedBold;
            total_Misc.ForceBold = MidsContext.Config.RtFont.PairedBold;
            enhListing.ForceBold = MidsContext.Config.RtFont.PairedBold;
            gDef1.Font = new Font(gDef1.Font.FontFamily, 8.25f, style);
            //gDef1.Font = new Font("Arial", 10f, FontStyle.Bold, GraphicsUnit.Pixel);
            gDef2.Font = gDef1.Font;
            gRes1.Font = gDef1.Font;
            gRes2.Font = gDef1.Font;
            SetBackColor();
            info_DataList.NameColor = MidsContext.Config.RtFont.ColorPlName;
            fx_List1.NameColor = MidsContext.Config.RtFont.ColorPlName;
            fx_List2.NameColor = MidsContext.Config.RtFont.ColorPlName;
            fx_List3.NameColor = MidsContext.Config.RtFont.ColorPlName;
            total_Misc.NameColor = MidsContext.Config.RtFont.ColorPlName;
            enhListing.NameColor = MidsContext.Config.RtFont.ColorPlName;
            info_DataList.ItemColor = MidsContext.Config.RtFont.ColorText;
            fx_List1.ItemColor = MidsContext.Config.RtFont.ColorText;
            fx_List2.ItemColor = MidsContext.Config.RtFont.ColorText;
            fx_List3.ItemColor = MidsContext.Config.RtFont.ColorText;
            enhListing.ItemColor = MidsContext.Config.RtFont.ColorText;
            total_Misc.ItemColor = MidsContext.Config.RtFont.ColorText;
            info_DataList.ItemColorAlt = MidsContext.Config.RtFont.ColorEnhancement;
            fx_List1.ItemColorAlt = MidsContext.Config.RtFont.ColorEnhancement;
            fx_List2.ItemColorAlt = MidsContext.Config.RtFont.ColorEnhancement;
            fx_List3.ItemColorAlt = MidsContext.Config.RtFont.ColorEnhancement;
            enhListing.ItemColorAlt = Color.Yellow;
            total_Misc.ItemColorAlt = MidsContext.Config.RtFont.ColorEnhancement;
            info_DataList.ValueColorD = MidsContext.Config.RtFont.ColorInvention;
            fx_List1.ValueColorD = MidsContext.Config.RtFont.ColorInvention;
            fx_List2.ValueColorD = MidsContext.Config.RtFont.ColorInvention;
            fx_List3.ValueColorD = MidsContext.Config.RtFont.ColorInvention;
            enhListing.ValueColorD = Color.Red;
            total_Misc.ValueColorD = MidsContext.Config.RtFont.ColorInvention;
            info_DataList.ItemColorSpecial = MidsContext.Config.RtFont.ColorPlSpecial;
            fx_List1.ItemColorSpecial = MidsContext.Config.RtFont.ColorPlSpecial;
            fx_List2.ItemColorSpecial = MidsContext.Config.RtFont.ColorPlSpecial;
            fx_List3.ItemColorSpecial = MidsContext.Config.RtFont.ColorPlSpecial;
            enhListing.ItemColorSpecial = Color.FromArgb(0, byte.MaxValue, 0);
            total_Misc.ItemColorSpecial = MidsContext.Config.RtFont.ColorPlSpecial;
        }

        public void SetLocation(Point iLocation, bool Force)
        {
            var flag = Force | ((SnapLocation.X == Location.X) & (SnapLocation.Y == Location.Y));
            SnapLocation.X = iLocation.X;
            SnapLocation.Y = iLocation.Y;
            SnapLocation.Width = Width;
            if (SnapLocation.Height < Height)
                SnapLocation.Height = Height;
            if (!flag)
                return;
            Left = SnapLocation.X;
            Top = SnapLocation.Y;
        }

        private void SetPowerScaler()
        {
            if (pBase == null)
            {
                PowerScaler.Visible = false;
            }
            else if (pBase.VariableEnabled & (HistoryIDX > -1))
            {
                var str = pBase.VariableName;
                if (string.IsNullOrEmpty(str))
                    str = "Targets";
                PowerScaler.Visible = true;
                PowerScaler.BeginUpdate();
                PowerScaler.ForcedMax = pBase.VariableMax;
                PowerScaler.Clear();
                PowerScaler.AddItem(
                    str + ":|" + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[HistoryIDX].VariableValue),
                    MidsContext.Character.CurrentBuild.Powers[HistoryIDX].VariableValue, 0.0f,
                    "Use this slider to vary the power's effect.\r\nMin: " + Convert.ToString(pBase.VariableMin) +
                    "\r\nMax: " +
                    Convert.ToString(pBase.VariableMax));
                PowerScaler.EndUpdate();
            }
            else
            {
                PowerScaler.Visible = false;
            }
        }

        public void SetScreenBounds(Rectangle iBounds)
        {
            ScreenBounds = iBounds;
        }

        public void SetSetPicker(int iSet)
        {
            if (iSet < 0)
            {
                info_Title.Text = "No Enhancement";
                Info_txtLarge.Text = "";
                info_txtSmall.Text = "";
            }
            else
            {
                info_Title.Text = DatabaseAPI.Database.EnhancementSets[iSet].DisplayName;
                var str1 = DatabaseAPI.GetSetTypeByIndex(DatabaseAPI.Database.EnhancementSets[iSet].SetType).Name;
                if (!Compact)
                    str1 += RTF.Crlf();
                var str2 = DatabaseAPI.Database.EnhancementSets[iSet].LevelMin !=
                           DatabaseAPI.Database.EnhancementSets[iSet].LevelMax
                    ? $"{DatabaseAPI.Database.EnhancementSets[iSet].LevelMin + 1} to {DatabaseAPI.Database.EnhancementSets[iSet].LevelMax + 1}"
                    : $"{DatabaseAPI.Database.EnhancementSets[iSet].LevelMin + 1}";
                info_txtSmall.Rtf = $"{RTF.StartRTF()}{str1}{RTF.Color(RTF.ElementID.Invention)}Level: {str2}{RTF.Color(RTF.ElementID.Text)}{RTF.EndRTF()}";
                Info_txtLarge.Rtf = $"{RTF.StartRTF()}{EnhancementSetCollection.GetSetInfoLongRTF(iSet)}{RTF.EndRTF()}";
            }
        }

        private bool sFXCheck(Enums.ShortFX isFX)
        {
            if (isFX.Index == null)
                return false;
            var num = isFX.Index.Length - 1;
            for (var index = 0; index <= num; ++index)
                if ((pBase.Effects.Length > isFX.Index[index]) & (isFX.Index[index] > -1) &&
                    pBase.Effects[isFX.Index[index]].isEnhancementEffect)
                    return true;

            return false;
        }

        private string ShortStr(string full, string brief)
        {
            return info_DataList.Font.Size <= 100f / full.Length ? full : brief;
        }

        private void SizeRefresh()
        {
            if (Compact)
                CompactSize();
            else
                ResetSize();
        }

        public void SetGraphType(Enums.eDDGraph graphType, Enums.eDDStyle graphStyle)
        {
            Info_Damage.GraphType = graphType;
            Info_Damage.Style = graphStyle;
        }

        private bool SplitFX_AddToList(ref Enums.ShortFX BaseSFX, ref Enums.ShortFX EnhSFX, ref PairedList iList, string SpecialTitle = "")
        {
            bool flag;
            if (!BaseSFX.Present)
            {
                flag = false;
            }
            else
            {
                var shortFxArray1 = Power.SplitFX(ref BaseSFX, ref pBase);
                var shortFxArray2 = Power.SplitFX(ref EnhSFX, ref pEnh);
                if (shortFxArray2.Length < shortFxArray1.Length)
                {
                    var swappedFX = SwapExtraEffects(shortFxArray1, shortFxArray2);
                    shortFxArray1 = (Enums.ShortFX[])swappedFX[0].Clone();
                    shortFxArray2 = (Enums.ShortFX[])swappedFX[1].Clone();
                }
                
                for (var index = 0; index < shortFxArray1.Length; index++)
                {
                    if (!shortFxArray1[index].Present)
                        continue;
                    var Suffix = string.Empty;
                    var num2 = shortFxArray1[index].Value[0];
                    var num3 = index < shortFxArray2.Length
                        ? shortFxArray2[index].Value[0]
                        : shortFxArray2[index - 1].Value[0];
                    if (pEnh.Effects[shortFxArray1[index].Index[0]].DisplayPercentage)
                    {
                        Suffix = "%";
                        var effect = pEnh.Effects[shortFxArray1[index].Index[0]];
                        if ((effect.EffectType == Enums.eEffectType.Heal |
                             effect.EffectType == Enums.eEffectType.Endurance |
                             effect.EffectType == Enums.eEffectType.Damage) &
                            pEnh.Effects[shortFxArray1[index].Index[0]].Aspect == Enums.eAspect.Cur)
                        {
                            num2 *= 100f;
                            num3 *= 100f;
                        }
                    }
                    else
                    {
                        switch (pEnh.Effects[shortFxArray1[index].Index[0]].EffectType)
                        {
                            case Enums.eEffectType.Heal:
                                Suffix = " HP";
                                break;
                            case Enums.eEffectType.HitPoints:
                                Suffix = " HP";
                                break;
                        }
                    }

                    var title = Enums.GetEffectNameShort(pEnh.Effects[shortFxArray1[index].Index[0]].EffectType);
                    if (SpecialTitle != string.Empty)
                        title = SpecialTitle;
                    var s1 = num2;
                    var s2 = num3;
                    if ((pEnh.Effects[shortFxArray1[index].Index[0]].Suppression & MidsContext.Config.Suppression) !=
                        Enums.eSuppress.None)
                    {
                        s1 = 0.0f;
                        s2 = 0.0f;
                    }

                    iList.AddItem(FastItem(title, s1, s2, Suffix, false, false, pEnh.Effects[shortFxArray1[index].Index[0]].Probability < 1.0, pEnh.Effects[shortFxArray1[index].Index[0]].ActiveConditionals.Count > 0, Power.SplitFXGroupTip(ref shortFxArray1[index], ref pEnh, false)));
                    if (pEnh.Effects[shortFxArray1[index].Index[0]].isEnhancementEffect)
                        iList.SetUnique();
                }

                flag = true;
            }

            return flag;
        }

        private void Title_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_offset = new Point(-e.X, -e.Y);
        }

        private void Title_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || MoveDisable)
                return;
            var point1 = new Point(e.X, e.Y);
            point1.Offset(mouse_offset.X, mouse_offset.Y);
            var point2 = new Point();
            ref var local = ref point2;
            var location = Location;
            var x = location.X + point1.X;
            location = Location;
            var y = location.Y + point1.Y;
            local = new Point(x, y);
            if (point2.X - 10 < SnapLocation.X)
                point2.X = SnapLocation.X;
            if ((point2.X + Width + 10 > ScreenBounds.Right) & (point2.X + Width - 20 < ScreenBounds.Right))
                point2.X = ScreenBounds.Right;
            if ((point2.Y + 10 > SnapLocation.Y) & (point2.Y - 20 <= SnapLocation.Y))
                point2.Y = SnapLocation.Y;
            if (point2.Y < ScreenBounds.Top)
                point2.Y = ScreenBounds.Top;
            if (point2.Y + info_Title.Bottom + pnlInfo.Top > ScreenBounds.Bottom)
                point2.Y = ScreenBounds.Bottom - (pnlInfo.Top + info_Title.Bottom);
            if (point2.X + Width > ScreenBounds.Right)
                point2.X = ScreenBounds.Right - Width;
            if (Location == point2)
                return;
            Location = point2;
            var moved = Moved;
            moved?.Invoke();
        }

        private void Fx_ListItemClick(PairedList.ItemPair? item, MouseEventArgs e)
        {
            if (item == null)
            {
                return;
            }

            if (e.Button != MouseButtons.Left || e.Clicks != 1)
            {
                return;
            }

            if (item.EntTag == null)
            {
                return;
            }

            var sets = item.EntTag.PowersetFullName.ToList();
            var petPowers = new List<IPower>();
            foreach (var powersFound in sets.Select(powerSet => DatabaseAPI.GetPowersetByName(powerSet)?.Powers.ToList()).Where(powersFound => powersFound != null))
            {
                petPowers.AddRange(powersFound);
            }

            PetInfo = new PetInfo(HistoryIDX, pBase, petPowers);


            var powers = new HashSet<string>();
            foreach (var ps in sets)
            {
                var powerList = DatabaseAPI.GetPowersetByName(ps)?.Powers;
                var returnedPowers = powerList?.SelectMany(p => p.FullName, (power, c) => power.FullName).ToHashSet();
                if (returnedPowers == null)
                {
                    continue;
                }

                powers.UnionWith(returnedPowers);
            }

            EntityDetails?.Invoke(item.EntTag.UID, powers, HistoryIDX, PetInfo);
        }
    }
}
