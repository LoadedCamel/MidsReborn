using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using FastDeepCloner;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Display;
using mrbBase.Base.Master_Classes;
using mrbControls;

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
        private IPower pBase;
        private IPower pEnh;
        private int pLastScaleVal;
        private Rectangle ScreenBounds;
        public Rectangle SnapLocation;
        public int TabPage;
        private bool VillainColor;

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
            //InitializeComponent();
            BackColorChanged += DataView_BackColorChanged;
            Load += DataView_Load;
            Name = nameof(DataView);
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

        private static PairedList.ItemPair BuildEDItem(
            int index,
            float[] value,
            Enums.eSchedule[] schedule,
            string Name,
            float[] afterED)
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
            return iString.Length >= capLength ? iString.Substring(0, capLength) : iString;
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
            Info_Damage.nBaseVal = 0.0f;
            Info_Damage.nEnhVal = 0.0f;
            Info_Damage.nHighBase = 0.0f;
            Info_Damage.nHighEnh = 0.0f;
            Info_Damage.nMaxEnhVal = 0.0f;
            Info_txtLarge.Height = 100;
            Floating = bFloating;
            Clear();
        }

        private void Display_EDFigures()
        {
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
                        if (MidsContext.Character.CurrentBuild.Powers[index1].Power == null || !MidsContext.Character.CurrentBuild.Powers[index1].StatInclude)
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
                            foreach (var str1 in MidsContext.Character.CurrentBuild.Powers[inToonHistory].Power.BoostsAllowed)
                            {
                                if (power2.BoostsAllowed.Any(str2 => str1 == str2))
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
                                        foreach (var str in power2.BoostsAllowed)
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
                                        foreach (var str in power2.BoostsAllowed)
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
                            enhListing.AddItem(BuildEDItem(index, numArray1, schedule1,
                                Enum.GetName(eEnhance.GetType(), index), afterED1));
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

        private void display_Info(bool NoLevel = false, int iEnhLvl = -1)
        {
            if (!NoLevel & (pBase.Level > 0))
                info_Title.Text = $"[{pBase.Level}] {pBase.DisplayName}";
            else
                info_Title.Text = pBase.DisplayName;
            if (iEnhLvl > -1)
            {
                var infoTitle = info_Title;
                infoTitle.Text = $"{infoTitle.Text} (Slot Level {iEnhLvl + 1})";
            }

            enhNameDisp.Text = "Enhancement Values";
            info_txtSmall.Rtf = RTF.StartRTF() + RTF.ToRTF(pBase.DescShort) + RTF.EndRTF();
            Info_txtLarge.Rtf = RTF.StartRTF() + RTF.ToRTF(pBase.DescLong) + RTF.EndRTF();
            var Suffix1 = pBase.PowerType != Enums.ePowerType.Toggle ? "" : "/s";
            info_DataList.Clear();
            var Tip1 = string.Empty;
            if (pBase.PowerType == Enums.ePowerType.Click)
            {
                if ((pEnh.ToggleCost > 0.0) & (pEnh.RechargeTime + (double)pEnh.CastTime + pEnh.InterruptTime > 0.0))
                    Tip1 = $"Effective end drain per second: {Utilities.FixDP(pEnh.ToggleCost / (pEnh.RechargeTime + pEnh.CastTime + pEnh.InterruptTime))}/s";

                if ((pEnh.ToggleCost > 0.0) &
                    (MidsContext.Config.DamageMath.ReturnValue == ConfigData.EDamageReturn.Numeric))
                {
                    var damageValue = pEnh.FXGetDamageValue();
                    if (damageValue > 0.0)
                    {
                        if (!string.IsNullOrEmpty(Tip1))
                            Tip1 += "\r\n";

                        Tip1 = $"{Tip1}Effective damage per unit of end: {Utilities.FixDP(damageValue / pEnh.ToggleCost)}";
                    }
                }
            }

            /*foreach (var effect in pBase.Effects)
            {
                effect.UpdateAttrib();
                SetDamageTip();
            }*/

            info_DataList.AddItem(FastItem(ShortStr("End Cost", "End"), pBase.ToggleCost, pEnh.ToggleCost, Suffix1, Tip1));
            var flag1 = pBase.HasAbsorbedEffects && pBase.PowerIndex > -1 &&
                        DatabaseAPI.Database.Power[pBase.PowerIndex].EntitiesAutoHit == Enums.eEntity.None;
            var flag2 = false;
            var num1 = pBase.Effects.Length - 1;
            for (var index = 0; (index <= num1) & !flag2; ++index)
                if (pBase.Effects[index].RequiresToHitCheck)
                    flag2 = true;

            if ((pBase.EntitiesAutoHit == Enums.eEntity.None) | flag2 | flag1 | ((pBase.Range > 20.0) & pBase.I9FXPresentP(Enums.eEffectType.Mez, Enums.eMez.Taunt)))
            {
                var accuracy1 = pBase.Accuracy;
                var accuracy2 = pEnh.Accuracy;
                var num2 = MidsContext.Config.BaseAcc * pBase.Accuracy;
                var str = string.Empty;
                var Suffix2 = "%";
                if ((pBase.EntitiesAutoHit != Enums.eEntity.None) & flag2)
                {
                    str = "\r\n* This power is autohit, but has an effect that requires a ToHit roll.";
                    Suffix2 += "*";
                }

                if ((Math.Abs(accuracy1 - (double)accuracy2) > float.Epsilon) &
                    (Math.Abs(num2 - (double)accuracy2) > float.Epsilon))
                {
                    var Tip2 = $"Accuracy multiplier without other buffs (Real Numbers style): {pBase.Accuracy + (pEnh.Accuracy - (double)MidsContext.Config.BaseAcc):##0.00000}x{str}";
                    //var Tip2 = "Accuracy multiplier without other buffs (Real Numbers style): " + Strings.Format((float) (pBase.Accuracy + (pEnh.Accuracy - (double) MidsContext.Config.BaseAcc)), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00000") + "x" + str;
                    info_DataList.AddItem(FastItem(ShortStr("Accuracy", "Acc"),
                        (float)(MidsContext.Config.BaseAcc * (double)pBase.Accuracy * 100.0), pEnh.Accuracy * 100f,
                        Suffix2, Tip2));
                }
                else
                {
                    var Tip2 = $"Accuracy multiplier without other buffs (Real Numbers style): {pBase.AccuracyMult:##0.00}x{str}";
                    //var Tip2 = "Accuracy multiplier without other buffs (Real Numbers style): " + Strings.Format(pBase.AccuracyMult, "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00") + "x" + str;
                    info_DataList.AddItem(FastItem(ShortStr("Accuracy", "Acc"),
                        (float)(MidsContext.Config.BaseAcc * (double)pBase.Accuracy * 100.0),
                        (float)(MidsContext.Config.BaseAcc * (double)pBase.Accuracy * 100.0), Suffix2, Tip2));
                }
            }
            else
            {
                info_DataList.AddItem(new PairedList.ItemPair(string.Empty, string.Empty, false, false, false,
                    string.Empty));
            }

            info_DataList.AddItem(FastItem(ShortStr("Recharge", "Rchg"), pBase.RechargeTime, pEnh.RechargeTime, "s"));
            var s1 = 0.0f;
            var s2 = 0.0f;
            var durationEffectId = pBase.GetDurationEffectID();
            if (durationEffectId > -1)
            {
                if ((pBase.Effects[durationEffectId].EffectType == Enums.eEffectType.EntCreate) &
                    (pBase.Effects[durationEffectId].Duration >= 9999.0))
                {
                    s1 = 0.0f;
                    s2 = 0.0f;
                }
                else
                {
                    s1 = pBase.Effects[durationEffectId].Duration;
                    s2 = pEnh.Effects[durationEffectId].Duration;
                }
            }

            info_DataList.AddItem(FastItem(ShortStr("Duration", "Durtn"), s1, s2, "s"));

            info_DataList.AddItem(FastItem(ShortStr("Range", "Range"), pBase.Range, pEnh.Range, string.Empty));
            info_DataList.AddItem(pBase.Arc > 0
                ? FastItem("Arc", pBase.Arc, pEnh.Arc, "°")
                : FastItem("Radius", pBase.Radius, pEnh.Radius, string.Empty));
            info_DataList.AddItem(FastItem(ShortStr("Cast Time", "Cast"), pEnh.CastTime, pBase.CastTime, "s", $"CastTime: {pBase.CastTime}\r\nArcana CastTime: {(float)(Math.Ceiling(pEnh.CastTime / 0.132f) + 1.0) * 0.132f}", false, true, false, false, 3));
            info_DataList.AddItem(pBase.PowerType == Enums.ePowerType.Toggle
                ? FastItem(ShortStr("Activate", "Act"), pBase.ActivatePeriod, pEnh.ActivatePeriod, "s", "The effects of this toggle power are applied at this interval.")
                : FastItem(ShortStr("Interrupt", "Intrpt"), pEnh.InterruptTime, pBase.InterruptTime, "s", "After activating this power, it can be interrupted for this amount of time."));
            if (durationEffectId > -1 &&
                pBase.Effects[durationEffectId].EffectType == Enums.eEffectType.Mez &
                pBase.Effects[durationEffectId].MezType != Enums.eMez.Taunt &
                !((pBase.Effects[durationEffectId].MezType == Enums.eMez.Knockback |
                   pBase.Effects[durationEffectId].MezType == Enums.eMez.Knockup) &
                  pBase.Effects[durationEffectId].Mag < 0))
            {
                info_DataList.AddItem(new PairedList.ItemPair("Effect:",
                    Enum.GetName(Enums.eMez.None.GetType(), pBase.Effects[durationEffectId].MezType), false,
                    pBase.Effects[durationEffectId].Probability < 1.0,
                    pBase.Effects[durationEffectId].CanInclude(),
                    durationEffectId));

                var iAlternate =
                    Math.Abs(pBase.Effects[durationEffectId].BuffedMag -
                             (double)pEnh.Effects[durationEffectId].BuffedMag) > float.Epsilon;

                info_DataList.AddItem(new PairedList.ItemPair("Mag:",
                    Convert.ToString(pEnh.Effects[durationEffectId].BuffedMag, CultureInfo.InvariantCulture),
                    iAlternate,
                    pBase.Effects[durationEffectId].Probability < 1.0));
            }

            var rankedEffects = pBase.GetRankedEffects();
            for (var ID = 0; ID < rankedEffects.Length; ID++)
            {
                try
                {
                    if (rankedEffects[ID] <= -1)
                        continue;
                    if (pBase.Effects[rankedEffects[ID]].EffectType == Enums.eEffectType.Mez)
                        continue;
                    var rankedEffect = GetRankedEffect(rankedEffects, ID);
                    if (pBase.Effects[rankedEffects[ID]].EffectType == Enums.eEffectType.PowerRedirect)
                        continue;
                    if (!((pBase.Effects[rankedEffects[ID]].Probability > 0.0) &
                          ((MidsContext.Config.Suppression & pBase.Effects[rankedEffects[ID]].Suppression) ==
                           Enums.eSuppress.None) &
                          pBase.Effects[rankedEffects[ID]].CanInclude()))
                        continue;
                    if (pBase.Effects[rankedEffects[ID]].EffectType != Enums.eEffectType.Enhancement)
                    {
                        if (pBase.Effects[rankedEffects[ID]].EffectType != Enums.eEffectType.Mez)
                            switch (pBase.Effects[rankedEffects[ID]].EffectType)
                            {
                                case Enums.eEffectType.Recovery:
                                    rankedEffect.Name = "Recovery";
                                    rankedEffect.Value = $"{(pBase.Effects[rankedEffects[ID]].DisplayPercentage ? $"{pEnh.Effects[rankedEffects[ID]].BuffedMag * 100}%" : $"{pEnh.Effects[rankedEffects[ID]].BuffedMag}"):####.##}";
                                    break;
                                
                                case Enums.eEffectType.EntCreate:
                                {
                                    rankedEffect.Name = "Summon";
                                    if (pBase.Effects[rankedEffects[ID]].nSummon > -1)
                                    {
                                        rankedEffect.Value = DatabaseAPI.Database
                                            .Entities[pBase.Effects[rankedEffects[ID]].nSummon].DisplayName;
                                    }
                                    else
                                    {
                                        rankedEffect.Value = pBase.Effects[rankedEffects[ID]].Summon;
                                        if (rankedEffect.Value.StartsWith("MastermindPets_"))
                                            rankedEffect.Value =
                                                rankedEffect.Value.Replace("MastermindPets_", string.Empty);
                                        if (rankedEffect.Value.StartsWith("Pets_"))
                                            rankedEffect.Value = rankedEffect.Value.Replace("Pets_", string.Empty);
                                        if (rankedEffect.Value.StartsWith("Villain_Pets_"))
                                            rankedEffect.Value =
                                                rankedEffect.Value.Replace("Villain_Pets_", string.Empty);
                                    }

                                    break;
                                }
                                case Enums.eEffectType.RevokePower:
                                {
                                    rankedEffect.Name = "Revoke";
                                    if (pBase.Effects[rankedEffects[ID]].nSummon > -1)
                                    {
                                        rankedEffect.Value = DatabaseAPI.Database
                                            .Entities[pBase.Effects[rankedEffects[ID]].nSummon].DisplayName;
                                    }
                                    else
                                    {
                                        rankedEffect.Value = pBase.Effects[rankedEffects[ID]].Summon;
                                        if (rankedEffect.Value.StartsWith("MastermindPets_"))
                                            rankedEffect.Value =
                                                rankedEffect.Value.Replace("MastermindPets_", string.Empty);
                                        if (rankedEffect.Value.StartsWith("Pets_"))
                                            rankedEffect.Value = rankedEffect.Value.Replace("Pets_", string.Empty);
                                        if (rankedEffect.Value.StartsWith("Villain_Pets_"))
                                            rankedEffect.Value =
                                                rankedEffect.Value.Replace("Villain_Pets_", string.Empty);
                                    }

                                    break;
                                }
                                default:
                                    rankedEffect.Name = ShortStr(Enums.GetEffectName(pBase.Effects[rankedEffects[ID]].EffectType),
                                            Enums.GetEffectNameShort(pBase.Effects[rankedEffects[ID]].EffectType));
                                    break;
                            }
                        else
                            rankedEffect.Name = ShortStr(
                                Enums.GetMezName((Enums.eMezShort)pBase.Effects[rankedEffects[ID]].MezType),
                                Enums.GetMezNameShort((Enums.eMezShort)pBase.Effects[rankedEffects[ID]].MezType));
                    }

                    info_DataList.AddItem(rankedEffect);
                    if (pBase.Effects[rankedEffects[ID]].isEnhancementEffect)
                        info_DataList.SetUnique();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}\n{ex.StackTrace}");
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
                str1 += " (% only)";
            var damageValue1 = pBase.FXGetDamageValue();
            if (pBase.NIDSubPower.Length > 0 & damageValue1 == 0.0)
            {
                lblDmg.Text = string.Empty;
                Info_Damage.nBaseVal = 0.0f;
                Info_Damage.nMaxEnhVal = 0.0f;
                Info_Damage.nEnhVal = 0.0f;
                Info_Damage.Text = string.Empty;
            }
            else
            {
                lblDmg.Text = $"{str1}:";
                var damageValue2 = pEnh.FXGetDamageValue();
                var num2 = damageValue1 * (1f + Enhancement.ApplyED(Enums.eSchedule.A, 2.277f));
                Info_Damage.nBaseVal = damageValue1;
                Info_Damage.nMaxEnhVal = num2;
                Info_Damage.nEnhVal = damageValue2;
                
                if (Math.Abs(damageValue2 - damageValue1) > float.Epsilon)
                {
                    Info_Damage.Text = pEnh.FXGetDamageString() + " (" + Utilities.FixDP(damageValue1) + ")";
                }
                else
                {
                    Info_Damage.Text = pBase.FXGetDamageString();
                }
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
                /*switch (MidsContext.Config.DataGraphType)
                {
                    case Enums.eDDGraph.Simple:
                        Info_Damage.TextAlign = Enums.eDDAlign.Center;
                        Info_Damage.Style = Enums.eDDStyle.TextUnderGraph;
                        break;
                    case Enums.eDDGraph.Enhanced:
                        Info_Damage.TextAlign = Enums.eDDAlign.Center;
                        Info_Damage.Style = Enums.eDDStyle.TextUnderGraph;
                        break;
                    case Enums.eDDGraph.Both:
                        Info_Damage.TextAlign = Enums.eDDAlign.Center;
                        Info_Damage.Style = Enums.eDDStyle.TextUnderGraph;
                        break;
                    case Enums.eDDGraph.Stacked:
                        Info_Damage.TextAlign = Enums.eDDAlign.Center;
                        Info_Damage.Style = Enums.eDDStyle.TextUnderGraph;
                        break;
                }*/
            }
            else
            {
                Info_Damage.TextAlign = Enums.eDDAlign.Center;
                Info_Damage.Style = Enums.eDDStyle.Text;
            }

            lblLock.Visible = Lock & (TabPage != 2);
            display_Info(noLevel, iEnhLevel);
            DisplayEffects(noLevel, iEnhLevel);
            Display_EDFigures();
        }

        private void DisplayEffects(bool noLevel = false, int iEnhLvl = -1)
        {
            if (!noLevel & (pBase.Level > 0))
                fx_Title.Text = "[" + Convert.ToString(pBase.Level) + "] " + pBase.DisplayName;
            else
                fx_Title.Text = pBase.DisplayName;
            if (iEnhLvl > -1)
            {
                var fxTitle = fx_Title;
                fxTitle.Text = fxTitle.Text + " (Slot Level " + Convert.ToString(iEnhLvl + 1) + ")";
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
            var flag1 = false;
            var flag2 = false;
            if (index < PairedListArray.Length)
            {
                PairedListArray[index].Clear();
                num1 = effects_Heal(labelArray[index], PairedListArray[index]);
                if (num1 > 0)
                {
                    flag2 = true;
                    ++index;
                    if (index < PairedListArray.Length)
                        PairedListArray[index].Clear();
                }
            }

            if (index < PairedListArray.Length)
            {
                num1 = effects_Status(labelArray[index], PairedListArray[index]);
                if (num1 > 0)
                    flag1 = true;
                if ((num1 > 2) | ((num1 > 0) & (index == 0)))
                {
                    ++index;
                    if (index < PairedListArray.Length)
                        PairedListArray[index].Clear();
                }
            }

            if (!flag1 & flag2 & (num1 < 3))
                --index;
            if (index < PairedListArray.Length &&
                (effects_BuffDebuff(labelArray[index], PairedListArray[index]) > 0) &
                (PairedListArray[index].ItemCount > 2) & (index + 1 < PairedListArray.Length))
            {
                ++index;
                if (index < PairedListArray.Length)
                    PairedListArray[index].Clear();
            }

            if (index < PairedListArray.Length)
                index += effects_Movement(labelArray[index], PairedListArray[index]);
            if (index < PairedListArray.Length)
                index += effects_Summon(labelArray[index], PairedListArray[index]);
            if (index < PairedListArray.Length)
                index += effects_GrantPower(labelArray[index], PairedListArray[index]);
            if (index < PairedListArray.Length)
                index += effects_ModifyEffect(labelArray[index], PairedListArray[index]);
            if (index < PairedListArray.Length)
            {
                var num2 = index + effects_Elusivity(labelArray[index], PairedListArray[index]);
            }

            if (fx_lblHead1.Text != string.Empty)
                fx_lblHead1.Text += ":";
            if (fx_lblHead2.Text != string.Empty)
                fx_lblHead2.Text += ":";
            if (fx_LblHead3.Text != string.Empty)
                fx_LblHead3.Text += ":";
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
                return;
            var names = Enum.GetNames(Enums.eDamage.None.GetType());
            total_Misc.Clear();
            var displayStats = MidsContext.Character.DisplayStats;
            gDef1.Clear();
            gDef2.Clear();
            int[] numArray1 =
            {
                0, 0, 0, 1, 1, 0, 0, 1, 0, 0, 1, 1, 1, 0, 0, 0
            };
            var num1 = names.Length - 1;
            for (var dType = 1; dType <= num1; ++dType)
            {
                var iTip = $"{Convert.ToDecimal(displayStats.Defense(dType)):0.##}% {names[dType]} defense";
                //var iTip = Strings.Format(displayStats.Defense(dType), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "##") + "% " + names[dType] + " defense";
                if (!((dType != 9) & (dType != 7)))
                    continue;
                if (numArray1[dType] == 0)
                    gDef1.AddItem(names[dType] + ":|" + $"{Convert.ToDecimal(displayStats.Defense(dType)):0.##}%", displayStats.Defense(dType), 0.0f, iTip);
                else
                    gDef2.AddItem(names[dType] + ":|" + $"{Convert.ToDecimal(displayStats.Defense(dType)):0.##}%", displayStats.Defense(dType), 0.0f, iTip);
            }

            var num2 = gDef1.GetMaxValue();
            var maxValue1 = gDef2.GetMaxValue();
            if (maxValue1 > (double)num2)
                num2 = maxValue1;
            gDef1.Max = num2;
            gDef2.Max = num2;
            gDef1.Draw();
            gDef2.Draw();
            var str = $"{MidsContext.Character.Archetype.DisplayName} resistance cap: {Convert.ToDecimal(MidsContext.Character.Archetype.ResCap * 100.0):0.##}%";
            //var str = MidsContext.Character.Archetype.DisplayName + " resistance cap: " + Strings.Format((float) (MidsContext.Character.Archetype.ResCap * 100.0), "###0") + "%";
            gRes1.Clear();
            gRes2.Clear();
            int[] numArray2 =
            {
                0, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 1
            };
            var dType1 = 1;
            do
            {
                if (dType1 != 9)
                {
                    string iTip;
                    if (MidsContext.Character.TotalsCapped.Res[dType1] < (double)MidsContext.Character.Totals.Res[dType1])
                        iTip = $"{Convert.ToDecimal(displayStats.DamageResistance(dType1, true)):0.##}% {names[dType1]} resistance capped at {Convert.ToDecimal(displayStats.DamageResistance(dType1, false)):0.##}%";
                    //iTip = Strings.Format(displayStats.DamageResistance(dType1, true), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "##") + "% " + names[dType1] + " resistance capped at " + Strings.Format(displayStats.DamageResistance(dType1, false), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "##") + "%";
                    else
                        iTip = $"{Convert.ToDecimal(displayStats.DamageResistance(dType1, true)):0.##}% {names[dType1]} resistance. ({str})";
                    //iTip = Strings.Format(displayStats.DamageResistance(dType1, true), "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "##") + "% " + names[dType1] + " resistance. (" + str + ")";
                    if (numArray2[dType1] == 0)
                        gRes1.AddItem(names[dType1] + ":|" + $"{Convert.ToDecimal(displayStats.DamageResistance(dType1, false)):0.##}%", displayStats.DamageResistance(dType1, false), displayStats.DamageResistance(dType1, true), iTip);
                    else
                        gRes2.AddItem(names[dType1] + ":|" + $"{Convert.ToDecimal(displayStats.DamageResistance(dType1, false)):0.##}%", displayStats.DamageResistance(dType1, false), displayStats.DamageResistance(dType1, true), iTip);
                }

                ++dType1;
            } while (dType1 <= 9);

            var num3 = gRes1.GetMaxValue();
            var maxValue2 = gRes2.GetMaxValue();
            if (maxValue2 > (double)num3)
                num3 = maxValue2;
            gRes1.Max = num3;
            gRes2.Max = num3;
            gRes1.Draw();
            gRes2.Draw();
            var iTip1 = string.Empty;
            var iTip2 = "Time to go from 0-100% end: " + Utilities.FixDP(displayStats.EnduranceTimeToFull) +
                        "s.\r\nHover the mouse over the End Drain stats for more info.";
            if (displayStats.EnduranceRecoveryNet > 0.0)
            {
                iTip1 = "Net Endurance Gain (Recovery - Drain): " + Utilities.FixDP(displayStats.EnduranceRecoveryNet) +
                        "/s.";
                if (Math.Abs(displayStats.EnduranceRecoveryNet - (double)displayStats.EnduranceRecoveryNumeric) >
                    float.Epsilon)
                    iTip1 = iTip1 + "\r\nTime to go from 0-100% end (using net gain): " +
                            Utilities.FixDP(displayStats.EnduranceTimeToFullNet) + "s.";
            }
            else if (displayStats.EnduranceRecoveryNet < 0.0)
            {
                iTip1 = "With current end drain, you will lose end at a rate of: " +
                        Utilities.FixDP(displayStats.EnduranceRecoveryLossNet) +
                        "/s.\r\nFrom 100% you would run out of end in: " +
                        Utilities.FixDP(displayStats.EnduranceTimeToZero) + "s.";
            }

            var iTip3 = "Time to go from 0-100% health: " + Utilities.FixDP(displayStats.HealthRegenTimeToFull) +
                        "s.\r\nHealth regenerated per second: " +
                        Utilities.FixDP(displayStats.HealthRegenHealthPerSec) +
                        "%\r\nHitPoints regenerated per second at level 50: " +
                        Utilities.FixDP(displayStats.HealthRegenHPPerSec) + " HP";
            total_Misc.AddItem(new PairedList.ItemPair("Recovery:", $"{displayStats.EnduranceRecoveryPercentage(false):0.##}% ({displayStats.EnduranceRecoveryNumeric:0.##}/s)", false, false, false, iTip2));
            total_Misc.AddItem(new PairedList.ItemPair("Regen:", $"{displayStats.HealthRegenPercent(false):0.##}%", false, false, false, iTip3));
            total_Misc.AddItem(new PairedList.ItemPair("EndDrain:", $"{displayStats.EnduranceUsage:0.##}/s", false, false, false, iTip1));
            total_Misc.AddItem(new PairedList.ItemPair("+ToHit:", $"{displayStats.BuffToHit:0.##}%", false, false, false, "This effect is increasing the accuracy of all your powers."));
            total_Misc.AddItem(new PairedList.ItemPair("+EndRdx:", $"{displayStats.BuffEndRdx:0.##}%", false, false, false, "The end cost of all your powers is being reduced by this effect.\r\nThis is applied like an end-reduction enhancement."));
            total_Misc.AddItem(new PairedList.ItemPair("+Recharge:", $"{displayStats.BuffHaste(false) - 100.0:0.##}%", false, false, false, "The recharge time of your powers is being altered by this effect.\r\nThe higher the value, the faster the recharge."));
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
            var font1 = new Font("Arial", 8.55f, FontStyle.Regular);//
            var font2 = new Font("Arial", 9.55f, FontStyle.Bold);
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

            var num = Pages.Length - 1;
            RectangleF layoutRectangle;
            for (var index = 0; index <= num; ++index)
            {
                rect = new Rectangle(rect.Width * index, 2, 70, pnlTabs.Height - 2);
                layoutRectangle = new RectangleF(rect.X, rect.Y + (float)((rect.Height - (double)font1.GetHeight(graphics)) / 2.0), rect.Width, font1.GetHeight(graphics));
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

        private string GetToolTip(Enums.ShortFX fx)
        {
            string iTip = string.Empty;
            List<string> iTemp = new List<string>();
            var shortFxArray = Power.SplitFX(ref fx, ref pEnh);
            for (var index1 = 0; index1 < shortFxArray.Length; index1++)
            {
                if (!shortFxArray[index1].Present)
                    continue;
                iTemp.Add(Power.SplitFXGroupTip(ref shortFxArray[index1], ref pEnh, false));
            }
            foreach (var str in iTemp)
            {
                if (string.IsNullOrWhiteSpace(iTip))
                    iTip = str;
                else
                    iTip += $"{Environment.NewLine}{str}";
            }

            return iTip;
        }

        private int effects_BuffDebuff(Control iLabel, PairedList iList)
        {
            var effectMagSum1 = pBase.GetEffectMagSum(Enums.eEffectType.ToHit);
            var effectMagSum2 = pEnh.GetEffectMagSum(Enums.eEffectType.ToHit);
            var effectMagSum3 = pEnh.GetEffectMagSum(Enums.eEffectType.DamageBuff);
            var effectMagSum4 = pBase.GetEffectMagSum(Enums.eEffectType.PerceptionRadius);
            var effectMagSum5 = pEnh.GetEffectMagSum(Enums.eEffectType.PerceptionRadius);
            var effectMagSum6 = pBase.GetEffectMagSum(Enums.eEffectType.StealthRadius);
            var effectMagSum7 = pEnh.GetEffectMagSum(Enums.eEffectType.StealthRadius);
            var effectMagSum8 = pBase.GetEffectMagSum(Enums.eEffectType.ResEffect);
            var effectMagSum9 = pEnh.GetEffectMagSum(Enums.eEffectType.ResEffect);
            var effectMagSum10 = pBase.GetEffectMagSum(Enums.eEffectType.Enhancement);
            var effectMagSum11 = pEnh.GetEffectMagSum(Enums.eEffectType.Enhancement);
            var effectMagSum12 = pBase.GetEffectMagSum(Enums.eEffectType.DamageBuff);
            
            var effectMag1 = pBase.GetEffectMag(Enums.eEffectType.ThreatLevel);
            var effectMag2 = pEnh.GetEffectMag(Enums.eEffectType.ThreatLevel);
            var effectMag3 = pBase.GetEffectMag(Enums.eEffectType.DropToggles);
            var effectMag4 = pEnh.GetEffectMag(Enums.eEffectType.DropToggles);
            var effectMag5 = pBase.GetEffectMag(Enums.eEffectType.RechargeTime, Enums.eToWho.Self);
            var effectMag6 = pEnh.GetEffectMag(Enums.eEffectType.RechargeTime, Enums.eToWho.Self);
            var effectMag7 = pBase.GetEffectMag(Enums.eEffectType.RechargeTime, Enums.eToWho.Target);
            var effectMag8 = pEnh.GetEffectMag(Enums.eEffectType.RechargeTime, Enums.eToWho.Target);
            var effectMag9 = pBase.GetEffectMag(Enums.eEffectType.RechargeTime, Enums.eToWho.Ally);
            var effectMag10 = pEnh.GetEffectMag(Enums.eEffectType.RechargeTime, Enums.eToWho.Ally);
            

            effectMagSum1.Multiply();
            effectMagSum2.Multiply();
            effectMagSum3.Multiply();
            effectMagSum4.Multiply();
            effectMagSum5.Multiply();

            effectMag1.Multiply();
            effectMag2.Multiply();
            effectMag5.Multiply();
            effectMag6.Multiply();
            effectMag7.Multiply();
            effectMag8.Multiply();
            effectMag9.Multiply();
            effectMag10.Multiply();

            effectMagSum8.Multiply();
            effectMagSum9.Multiply();
            effectMagSum10.Multiply();
            effectMagSum11.Multiply();
            effectMagSum12.Multiply();

            var str1 = string.Empty;
            if (effectMagSum10.Present)
                str1 = pBase.Effects[effectMagSum10.Index[0]].ETModifies == Enums.eEffectType.Mez
                    ? Enums.GetMezNameShort((Enums.eMezShort)pBase.Effects[effectMagSum10.Index[0]].MezType)
                    : Enums.GetEffectNameShort(pBase.Effects[effectMagSum10.Index[0]].ETModifies);
            iList.ValueWidth = 55;
            int num1;
            if (!(effectMagSum1.Present |
                  effectMagSum3.Present |
                  effectMagSum1.Present |
                  effectMagSum4.Present |
                  effectMagSum6.Present |
                  effectMag1.Present |
                  effectMag3.Present |
                  effectMag5.Present |
                  effectMag7.Present |
                  effectMagSum10.Present |
                  effectMagSum8.Present |
                  effectMag9.Present))
            {
                num1 = 0;
            }
            else
            {
                string iTip;
                if (iLabel.Text != string.Empty)
                    iLabel.Text += @" / Misc ";
                iLabel.Text += @"Effects";
                if (effectMagSum1.Present)
                {
                    if (pBase.Effects[effectMagSum1.Index[0]].SpecialCase != Enums.eSpecialCase.None)
                    {
                        iList.AddItem(FastItem("ToHit", effectMagSum1, effectMagSum2, "%", false, false,
                            pBase.Effects[effectMagSum1.Index[0]].SpecialCase == Enums.eSpecialCase.Combo, false,
                            effectMagSum1));
                    }
                    else
                    {
                        iList.AddItem(FastItem("ToHit", effectMagSum1, effectMagSum2, "%", false, false,
                            pBase.Effects[effectMagSum1.Index[0]].ValidateConditional("active", "Combo"), false,
                            effectMagSum1));
                    }
                }
                
                if (sFXCheck(effectMagSum1))
                    iList.SetUnique();

                //original damage buff code (Effects Tab)
                Enums.ShortFX[] shortFxArray = Power.SplitFX(ref effectMagSum3, ref pEnh);
                int num2 = shortFxArray.Length - 1;
                for (int index1 = 0; index1 <= num2; ++index1)
                {
                    if (shortFxArray[index1].Present)
                    {
                        bool isDefiance = true;
                        int num3 = shortFxArray[index1].Index.Length - 1;
                        for (int index2 = 0; index2 <= num3; ++index2)
                        {
                            if (pEnh.Effects[shortFxArray[index1].Index[index2]].SpecialCase != Enums.eSpecialCase.Defiance)
                            {
                                isDefiance = false;
                            }
                            else if (!pEnh.Effects[shortFxArray[index1].Index[index2]].ValidateConditional("active", "defiance"))
                            {
                                isDefiance = false;
                            }
                        }
                        if (isDefiance)
                        {
                            iList.AddItem(new PairedList.ItemPair("Defiance:", Utilities.FixDP(shortFxArray[index1].Value[0]) + "%", false, false, false, pEnh.Effects[shortFxArray[index1].Index[0]].BuildEffectString(false, "Damage Buff (Defiance)", false, false, false)));
                        }
                        else
                        {
                            if (pEnh.Effects[shortFxArray[index1].Index[0]].SpecialCase != Enums.eSpecialCase.None && pEnh.Effects[shortFxArray[index1].Index[0]].SpecialCase != Enums.eSpecialCase.Defiance)
                            {
                                iList.AddItem(new PairedList.ItemPair("DamageBuff:", Utilities.FixDP(shortFxArray[index1].Value[0]) + "%", false, pEnh.Effects[shortFxArray[index1].Index[0]].SpecialCase == Enums.eSpecialCase.Combo, false, Power.SplitFXGroupTip(ref shortFxArray[index1], ref pEnh, false)));
                            }
                            else if (pEnh.Effects[shortFxArray[index1].Index[0]].ActiveConditionals.Count > 0)
                            {
                                iList.AddItem(new PairedList.ItemPair("DamageBuff:", Utilities.FixDP(shortFxArray[index1].Value[0]) + "%", false, false, pEnh.Effects[shortFxArray[index1].Index[0]].ValidateConditional("active", "combo"), Power.SplitFXGroupTip(ref shortFxArray[index1], ref pEnh, false)));
                            }
                            else
                            {
                                iList.AddItem(new PairedList.ItemPair("DamageBuff:", $"{Utilities.FixDP(shortFxArray[index1].Value[0])}%", false, false, false, Power.SplitFXGroupTip(ref shortFxArray[index1], ref pEnh, false)));
                            }
                        }

                        if (pEnh.Effects[shortFxArray[index1].Index[0]].isEnhancementEffect)
                        {
                            iList.SetUnique();
                        }
                    }
                }

                /*if (effectMagSum3.Present)
                {
                    iTip = GetToolTip(effectMagSum3);
                    if (pBase.Effects[effectMagSum12.Index[0]].SpecialCase == Enums.eSpecialCase.Defiance)
                    {
                        iList.AddItem(FastItem("Defiance", effectMagSum12, effectMagSum3, "%", false, false, false, pBase.Effects[effectMagSum12.Index[0]].SpecialCase == Enums.eSpecialCase.Combo, iTip));
                    }
                    else if (pBase.Effects[effectMagSum12.Index[0]].ValidateConditional("Active", "Defiance"))
                    {
                        iList.AddItem(FastItem("Defiance", effectMagSum12, effectMagSum3, "%", false, false, false, pBase.Effects[effectMagSum12.Index[0]].SpecialCase == Enums.eSpecialCase.Combo, iTip));
                    }
                    else
                    {
                        iList.AddItem(FastItem("Damage Buff", effectMagSum12, effectMagSum3, "%",false, false, false,pBase.Effects[effectMagSum12.Index[0]].ValidateConditional("active", "Combo"), iTip));
                    }
                }*/

                if (sFXCheck(effectMagSum12))
                    iList.SetUnique();

                if (effectMagSum4.Present)
                    iList.AddItem(FastItem("Percept", effectMagSum4, effectMagSum5, "%", effectMagSum4));
                if (sFXCheck(effectMagSum4))
                    iList.SetUnique();
                if (effectMagSum6.Present)
                    iList.AddItem(new PairedList.ItemPair("Stealth",
                        Convert.ToString(effectMagSum6.Sum, CultureInfo.InvariantCulture) + "ft", false, false, false,
                        pEnh.Effects[effectMagSum7.Index[0]].BuildEffectString(false, "Stealth Radius")));
                if (sFXCheck(effectMagSum6))
                    iList.SetUnique();
                if (effectMag1.Present)
                    iList.AddItem(FastItem("ThretLvl", effectMag1, effectMag2, "%", effectMag1));
                if (sFXCheck(effectMag1))
                    iList.SetUnique();
                if (effectMag3.Present)
                    iList.AddItem(FastItem("TogDrop", effectMag3, effectMag4, "%", false, false,
                        pBase.Effects[effectMag3.Index[0]].Probability < 1.0, false, effectMag3));
                if (sFXCheck(effectMag3))
                    iList.SetUnique();
                if (effectMag7.Present)
                    iList.AddItem(FastItem("Rchrg (Tgt)", effectMag7, effectMag8, "%", effectMag7));
                if (sFXCheck(effectMag7))
                    iList.SetUnique();
                if (effectMag5.Present)
                    iList.AddItem(FastItem("Rchrg (Self)", effectMag5, effectMag6, "%", effectMag5));
                if (sFXCheck(effectMag5))
                    iList.SetUnique();
                if (effectMag9.Present)
                    iList.AddItem(FastItem("Rchrg (Ally)", effectMag9, effectMag10, "%", effectMag9));
                if (sFXCheck(effectMag9))
                    iList.SetUnique();
                if (effectMagSum8.Present)
                {
                    if (!effectMagSum8.Multiple)
                    {
                        if (effectMagSum8.Present)
                            iList.AddItem(FastItem(
                                "Res(" + Enums.GetEffectNameShort(pBase.Effects[effectMagSum8.Index[0]].ETModifies) +
                                ")",
                                effectMagSum8, effectMagSum9, "%", effectMagSum8));
                        if (sFXCheck(effectMagSum8))
                            iList.SetUnique();
                    }
                    else
                    {
                        iList.AddItem(new PairedList.ItemPair("Res(Multi):",
                            Convert.ToString(effectMagSum8.Value[0], CultureInfo.InvariantCulture) + "%", false,
                            false, false, effectMagSum8));
                        if (sFXCheck(effectMagSum8))
                            iList.SetUnique();
                    }
                }

                if (effectMagSum10.Present & str1 != string.Empty)
                {
                    var str2 = !string.Equals(str1, "EnduranceDiscount", StringComparison.OrdinalIgnoreCase)
                        ? !string.Equals(str1, "RechargeTime", StringComparison.OrdinalIgnoreCase)
                            ? !IsMezEffect(str1) ? CapString(str1, 7) : "Effects"
                            : "RechRdx"
                        : "EndRdx";
                    if (effectMagSum10.Multiple)
                    {
                        if (effectMagSum10.Index.Length < 5)
                        {
                            var num3 = effectMagSum10.Index.Length - 1;
                            for (var index = 0; index <= num3; ++index)
                            {
                                var str3 = pBase.Effects[effectMagSum10.Index[index]].ETModifies ==
                                           Enums.eEffectType.Mez
                                    ? Enums.GetMezNameShort(
                                        (Enums.eMezShort)pBase.Effects[effectMagSum10.Index[index]].MezType)
                                    : Enums.GetEffectNameShort(pBase.Effects[effectMagSum10.Index[index]].ETModifies);
                                var str4 = !string.Equals(str3, "EnduranceDiscount", StringComparison.OrdinalIgnoreCase)
                                    ? !string.Equals(str3, "RechargeTime", StringComparison.OrdinalIgnoreCase)
                                        ? CapString(str3, 7)
                                        : "RechRdx"
                                    : "EndRdx";
                                var str5 = pEnh.Effects[effectMagSum10.Index[index]].ToWho switch
                                {
                                    Enums.eToWho.Target => " (Tgt)",
                                    Enums.eToWho.Self => " (Self)",
                                    Enums.eToWho.Ally => " (Ally)",
                                    _ => ""
                                };

                                if (str4.IndexOf("Jump", StringComparison.OrdinalIgnoreCase) > -1)
                                    continue;
                                iList.AddItem(new PairedList.ItemPair($"+{str4}:",
                                    $"{effectMagSum11.Value[index]}%{str5}", false, false, false,
                                    pEnh.Effects[effectMagSum11.Index[index]].BuildEffectString()));
                                if (pEnh.Effects[effectMagSum10.Index[index]].isEnhancementEffect)
                                    iList.SetUnique();
                            }
                        }
                        else
                        {
                            var str3 = "Multiple";
                            var iIndex = 0;
                            var num3 = -1;
                            while (iIndex < effectMagSum10.Index.Length)
                                if (string.Equals(pEnh.Effects[effectMagSum10.Index[iIndex]].Special, "RechargeTime",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    var str4 = pEnh.Effects[effectMagSum10.Index[iIndex]].ToWho switch
                                    {
                                        Enums.eToWho.Target => " (Tgt)",
                                        Enums.eToWho.Self => " (Self)",
                                        Enums.eToWho.Ally => " (Ally)",
                                        _ => ""
                                    };
                                    
                                    iList.AddItem(new PairedList.ItemPair("+RechRdx:",
                                        $"{effectMagSum11.Value[iIndex]}%{str4}", false, false, false,
                                        pEnh.Effects[effectMagSum11.Index[iIndex]].BuildEffectString()));
                                    if (pEnh.Effects[effectMagSum11.Index[iIndex]].isEnhancementEffect)
                                        iList.SetUnique();
                                    effectMagSum10.Remove(iIndex);
                                }
                                else
                                {
                                    if (num3 == -1)
                                        num3 = (int)Math.Round(effectMagSum11.Value[0]);
                                    else if ((num3 > 0) & (Math.Abs(num3 - (double)effectMagSum11.Value[iIndex]) > float.Epsilon))
                                        num3 = -2;
                                    ++iIndex;
                                }

                            if (effectMagSum10.Present)
                            {
                                var iValue = "Varies";
                                if (num3 > 0)
                                    iValue = $"{num3}%";
                                if (effectMagSum11.Multiple)
                                {
                                    iList.AddItem(new PairedList.ItemPair($"+{str3}:", iValue, false, false, false, effectMagSum11));
                                }
                                else
                                {
                                    var str4 = CapString(pBase.Effects[effectMagSum10.Index[0]].Special, 7);
                                    if (str4.IndexOf("Jump", StringComparison.OrdinalIgnoreCase) < 0)
                                    {
                                        iList.AddItem(
                                            new PairedList.ItemPair($"+{str4}:", iValue, false, false, false, effectMagSum11));
                                        if (sFXCheck(effectMagSum10))
                                            iList.SetUnique();
                                    }
                                }
                            }
                        }
                    }
                    else if (str2.IndexOf("Jump", StringComparison.OrdinalIgnoreCase) < 0 && effectMagSum11.Value != null)
                    {
                        iList.AddItem(new PairedList.ItemPair($"+{str2}:",
                            $"{effectMagSum11.Value[0]}%", false,
                            false, false, effectMagSum11));
                        if (sFXCheck(effectMagSum10))
                            iList.SetUnique();
                    }
                }

                num1 = 1;
            }

            return num1;
        }

        private int effects_Elusivity(Label iLabel, PairedList iList)
        {
            var flag = iList.ItemCount == 0;
            var num1 = 0;
            var num2 = pBase.Effects.Length - 1;
            for (var idEffect = 0; idEffect <= num2; ++idEffect)
            {
                if (!((pBase.Effects[idEffect].EffectType == Enums.eEffectType.Elusivity) &
                      (pBase.Effects[idEffect].Probability > 0.0)))
                    continue;
                var empty = string.Empty;
                var returnMask = new int[Enum.GetValues(pBase.Effects[idEffect].DamageType.GetType()).Length + 1];
                pBase.GetEffectStringGrouped(idEffect, ref empty, ref returnMask, false, true, true);
                var iTip = empty;
                var num3 = pBase.Effects[idEffect].MagPercent;
                if ((pBase.Effects[idEffect].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                    num3 = 0.0f;
                var iItem = new PairedList.ItemPair("Elusivity:", $"{Convert.ToDecimal(num3):0.##}%", false, pBase.Effects[idEffect].Probability < 1.0, false, iTip);
                iList.AddItem(iItem);
                var num4 = num1 + 1;
                if (flag)
                    iLabel.Text = "Elusivity (PvP)";
                return num4;
            }

            if (num1 > 0 && flag)
                iLabel.Text = "Elusivity (PvP)";
            return num1;
        }

        private int effects_GrantPower(Label iLabel, PairedList iList)
        {
            var flag = iList.ItemCount == 0;
            var num1 = 0;
            var num2 = pBase.Effects.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                if (!((pBase.Effects[index].EffectType == Enums.eEffectType.GrantPower) &
                      (pBase.Effects[index].Probability > 0.0) &
                      (pBase.Effects[index].EffectClass != Enums.eEffectClass.Ignored)))
                    continue;
                var iValue = "[Power]";
                if (pEnh.Effects[index].nSummon > -1)
                {
                    iValue = DatabaseAPI.Database.Power[pEnh.Effects[index].nSummon].DisplayName;
                }
                else
                {
                    var startIndex = pEnh.Effects[index].Summon.LastIndexOf(".", StringComparison.Ordinal) + 1;
                    if (startIndex < pEnh.Effects[index].Summon.Length)
                        iValue = pEnh.Effects[index].Summon.Substring(startIndex);
                }

                var iTip = pEnh.Effects[index].BuildEffectString();
                if ((pBase.Effects[index].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                    iValue = "(suppressed)";
                var iItem =
                    new PairedList.ItemPair("GrantPwr:", iValue, false, pBase.Effects[index].Probability < 1.0,
                        false, iTip);
                iList.AddItem(iItem);
                if (pBase.Effects[index].isEnhancementEffect)
                    iList.SetUnique();
                ++num1;
            }

            if (num1 > 0 && flag)
                iLabel.Text = "GrantPower Effects";
            return num1;
        }

        private int effects_Heal(Label iLabel, PairedList iList)
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
            var num1 = pBase.Effects.Length - 1;
            for (var iIndex = 0; iIndex <= num1; ++iIndex)
            {
                if ((pBase.Effects[iIndex].EffectType == Enums.eEffectType.Damage) &
                    (pBase.Effects[iIndex].DamageType == Enums.eDamage.Special) &
                    (pBase.Effects[iIndex].ToWho == Enums.eToWho.Self) &
                    (pBase.Effects[iIndex].Probability > 0.0) &
                    ((pBase.Effects[iIndex].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None))
                {
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
                var flag3 = pBase.AffectsAlly(Enums.eEffectType.Heal) | pBase.AffectsAlly(Enums.eEffectType.HitPoints) |
                            pBase.AffectsAlly(Enums.eEffectType.Regeneration) |
                            pBase.AffectsAlly(Enums.eEffectType.Recovery) |
                            pBase.AffectsAlly(Enums.eEffectType.Endurance) |
                            pBase.AffectsAlly(Enums.eEffectType.Absorb);
                if (flag1 & !flag2 && !flag3)
                    iLabel.Text = @"Health / Endurance (Target)";
                else if (flag2 & !flag3 && !flag1)
                    iLabel.Text = @"Health / Endurance (Self)";
                else if (flag3 & !flag2 && !flag1)
                    iLabel.Text = @"Health / Endurance (Ally)";
                else
                    iLabel.Text = @"Health / Endurance";
                if (shortFx.Present)
                {
                    var num3 = shortFx.Index.Length - 1;
                    for (var index = 0; index <= num3; ++index)
                    {
                        if (pBase.Effects[shortFx.Index[index]].Aspect != Enums.eAspect.Cur)
                            continue;
                        if (Math.Abs(shortFx.Value[index] - -1.0) < float.Epsilon)
                            shortFx.Value[index] = 0.0f;
                        if (Math.Abs(shortFx.Value[index] - 100.0) > float.Epsilon)
                            shortFx.Value[index] *= 100f;
                    }

                    shortFx.ReSum();
                    iList.AddItem(FastItem("Damage", shortFx, shortFx, " (Self)", shortFx));
                }

                SplitFX_AddToList(ref BaseSFX1, ref EnhSFX1, ref iList);
                SplitFX_AddToList(ref BaseSFX2, ref EnhSFX2, ref iList);
                SplitFX_AddToList(ref BaseSFX3, ref EnhSFX3, ref iList);
                SplitFX_AddToList(ref BaseSFX5, ref EnhSFX5, ref iList, "Regen(S)");
                SplitFX_AddToList(ref BaseSFX4, ref EnhSFX4, ref iList, "Regen(T)");
                SplitFX_AddToList(ref BaseSFX10, ref EnhSFX10, ref iList, "Regen(A)");
                SplitFX_AddToList(ref BaseSFX6, ref EnhSFX6, ref iList, "Recvry(S)");
                SplitFX_AddToList(ref BaseSFX7, ref EnhSFX7, ref iList, "Recvry(T)");
                SplitFX_AddToList(ref BaseSFX11, ref EnhSFX11, ref iList, "Recvry(A)");
                SplitFX_AddToList(ref BaseSFX9, ref EnhSFX9, ref iList, "End (Tgt)");
                SplitFX_AddToList(ref BaseSFX8, ref EnhSFX8, ref iList, "End (Self)");
                SplitFX_AddToList(ref BaseSFX12, ref EnhSFX12, ref iList, "End (Ally)");
                num2 = iList.ItemCount;
            }

            return num2;
        }

        private int effects_ModifyEffect(Label iLabel, PairedList iList)
        {
            var flag = iList.ItemCount == 0;
            var num1 = 0;
            var num2 = pBase.Effects.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                if (!((pBase.Effects[index].EffectType == Enums.eEffectType.GlobalChanceMod) & (pBase.Effects[index].Probability > 0.0)))
                {
                    continue;
                }

                var iTip = pEnh.Effects[index].BuildEffectString();
                var iValue = Convert.ToString(pBase.Effects[index].MagPercent, CultureInfo.InvariantCulture) + "%";
                if ((pBase.Effects[index].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                {
                    iValue = "(suppressed)";
                }

                var iItem = new PairedList.ItemPair(pBase.Effects[index].Reward + ":", iValue, false, pBase.Effects[index].Probability < 1.0, false, iTip);
                iList.AddItem(iItem);
                ++num1;
            }

            if (num1 > 0 && flag)
            {
                iLabel.Text = "Modify Effect";
            }

            return num1;
        }

        private int effects_Movement(Label iLabel, PairedList iList)
        {
            var shortFx1 = new Enums.ShortFX();
            var shortFx2 = new Enums.ShortFX();
            var s2_1 = new Enums.ShortFX();
            var shortFx3 = new Enums.ShortFX();
            var s2_2 = new Enums.ShortFX();
            var shortFx4 = new Enums.ShortFX();
            var s2_3 = new Enums.ShortFX();
            var shortFx5 = new Enums.ShortFX();
            var s2_4 = new Enums.ShortFX();
            var shortFx6 = new Enums.ShortFX();
            var shortFx7 = new Enums.ShortFX();
            var shortFx8 = new Enums.ShortFX();
            var shortFx9 = new Enums.ShortFX();
            var shortFx10 = new Enums.ShortFX();
            shortFx2.Assign(pBase.GetEffectMagSum(Enums.eEffectType.SpeedFlying));
            s2_1.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.SpeedFlying));
            shortFx1.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Fly));
            shortFx3.Assign(pBase.GetEffectMagSum(Enums.eEffectType.JumpHeight));
            s2_2.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.JumpHeight));
            shortFx4.Assign(pBase.GetEffectMagSum(Enums.eEffectType.SpeedJumping));
            s2_3.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.SpeedJumping));
            shortFx5.Assign(pBase.GetEffectMagSum(Enums.eEffectType.SpeedRunning));
            s2_4.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.SpeedRunning));
            shortFx6.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Enhancement));
            shortFx8.Assign(pBase.GetEffectMagSum(Enums.eEffectType.SpeedFlying, false, false, false, true));
            shortFx9.Assign(pBase.GetEffectMagSum(Enums.eEffectType.SpeedJumping, false, false, false, true));
            shortFx10.Assign(pBase.GetEffectMagSum(Enums.eEffectType.SpeedRunning, false, false, false, true));
            shortFx2.Multiply();
            s2_1.Multiply();
            shortFx3.Multiply();
            s2_2.Multiply();
            shortFx4.Multiply();
            s2_3.Multiply();
            shortFx5.Multiply();
            s2_4.Multiply();
            if (shortFx6.Present)
            {
                var num = shortFx6.Index.Length - 1;
                for (var index = 0; index <= num; ++index)
                    if (pBase.Effects[shortFx6.Index[index]].Special
                        .IndexOf("Jump", StringComparison.OrdinalIgnoreCase) > -1)
                        shortFx7.Add(shortFx6.Index[index], pBase.Effects[shortFx6.Index[index]].BuffedMag);
            }

            iList.ValueWidth = 55;
            int num1;
            if (!(shortFx7.Present | shortFx1.Present | shortFx2.Present | shortFx3.Present | shortFx4.Present |
                  shortFx5.Present))
            {
                num1 = 0;
            }
            else
            {
                var flag = (shortFx2.Present & pBase.AffectsTarget(Enums.eEffectType.SpeedFlying)) |
                           (shortFx3.Present & pBase.AffectsTarget(Enums.eEffectType.JumpHeight)) |
                           (shortFx4.Present & pBase.AffectsTarget(Enums.eEffectType.SpeedJumping)) |
                           (shortFx5.Present & pBase.AffectsTarget(Enums.eEffectType.SpeedRunning));
                if (iList.ItemCount == 0) iLabel.Text = flag ? "Movement (Target)" : "Movement (Self)";

                if (shortFx1.Present)
                    iList.AddItem(FastItem("Fly", shortFx1, shortFx1, string.Empty, shortFx1));
                if (sFXCheck(shortFx1))
                    iList.SetUnique();
                if (shortFx2.Present)
                    iList.AddItem(FastItem("FlySpd", shortFx2, s2_1, "%", shortFx2));
                if (sFXCheck(shortFx2))
                    iList.SetUnique();
                if (shortFx3.Present)
                    iList.AddItem(FastItem("JmpHeight", shortFx3, s2_2, "%", shortFx3));
                if (sFXCheck(shortFx3))
                    iList.SetUnique();
                if (shortFx4.Present)
                    iList.AddItem(FastItem("JmpSpd", shortFx4, s2_3, "%", shortFx4));
                if (sFXCheck(shortFx4))
                    iList.SetUnique();
                if (shortFx7.Present)
                    iList.AddItem(FastItem("+JmpHeight", shortFx7, shortFx7, "%", shortFx7));
                if (sFXCheck(shortFx7))
                    iList.SetUnique();
                if (shortFx5.Present)
                    iList.AddItem(FastItem("RunSpd", shortFx5, s2_4, "%", shortFx5));
                if (sFXCheck(shortFx5))
                    iList.SetUnique();
                if (shortFx10.Present)
                    iList.AddItem(FastItem("MaxRun", shortFx10, shortFx10, string.Empty, shortFx10));
                if (shortFx9.Present)
                    iList.AddItem(FastItem("MaxJmp", shortFx9, shortFx9, string.Empty, shortFx9));
                if (shortFx8.Present)
                    iList.AddItem(FastItem("MaxFly", shortFx8, shortFx8, string.Empty, shortFx8));
                num1 = 1;
            }

            return num1;
        }

        private int effects_Status(Label iLabel, PairedList iList)
        {
            var eMezShort = Enums.eMezShort.None;
            var shortFx1 = new Enums.ShortFX();
            var shortFx2 = new Enums.ShortFX();
            var shortFx3 = new Enums.ShortFX();
            var shortFx4 = new Enums.ShortFX();
            var num1 = 0;
            shortFx1.Assign(pBase.GetEffectMag(Enums.eEffectType.MezResist));
            shortFx2.Assign(pEnh.GetEffectMag(Enums.eEffectType.MezResist));
            shortFx3.Assign(pBase.GetEffectMag(Enums.eEffectType.Mez, Enums.eToWho.Unspecified, true));
            shortFx4.Assign(pEnh.GetEffectMag(Enums.eEffectType.Mez, Enums.eToWho.Unspecified, true));
            shortFx1.Multiply();
            shortFx2.Multiply();
            iList.ValueWidth = 60;
            if (shortFx1.Present | shortFx3.Present)
            {
                if (pBase.AffectsTarget(Enums.eEffectType.MezResist) | pBase.AffectsTarget(Enums.eEffectType.Mez))
                {
                    iLabel.Text = @"Status Effects (Target)";
                }
                else if (pBase.AffectsAlly(Enums.eEffectType.MezResist) | pBase.AffectsAlly(Enums.eEffectType.Mez))
                {
                    iLabel.Text = @"Status Effects (Ally)";
                }
                else
                {
                    iLabel.Text = @"Status Effects (Self)";
                }
            }

            var names = Enum.GetNames(eMezShort.GetType());
            if (shortFx3.Present)
            {
                var num2 = pBase.Effects.Length - 1;
                for (var iTagID = 0; iTagID <= num2; ++iTagID)
                {
                    if (!((pBase.Effects[iTagID].EffectType == Enums.eEffectType.Mez) &
                          (pBase.Effects[iTagID].Probability > 0.0) &
                          pBase.Effects[iTagID].CanInclude()) || !pEnh.Effects[iTagID].PvXInclude())
                        continue;
                    var str = !((pEnh.Effects[iTagID].Duration < 2.0) | (pBase.PowerType == Enums.ePowerType.Auto_)) ? " - " + pEnh.Effects[iTagID].Duration.ToString("#0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "#") + "s" : string.Empty;
                    if (pBase.Effects[iTagID].BuffedMag > 0.0)
                    {
                        var iAlternate2 =
                            (Math.Abs(pBase.Effects[iTagID].Duration - (double)pEnh.Effects[iTagID].Duration) >
                             float.Epsilon) |
                            (!Enums.MezDurationEnhancable(pBase.Effects[iTagID].MezType) &
                             (Math.Abs(pEnh.Effects[iTagID].BuffedMag - (double)pBase.Effects[iTagID].BuffedMag) > float.Epsilon));
                        var iValue = "Mag " + Utilities.FixDP(pEnh.Effects[iTagID].BuffedMag) + str;
                        if ((pBase.Effects[iTagID].Suppression & MidsContext.Config.Suppression) !=
                            Enums.eSuppress.None)
                            iValue = "0";
                        var iItem = new PairedList.ItemPair(CapString(names[(int)pBase.Effects[iTagID].MezType], 7) + ":", iValue, iAlternate2, (pBase.Effects[iTagID].Probability < 1.0) | (pBase.Effects[iTagID].ValidateConditional("active", "Combo")), pBase.Effects[iTagID].ActiveConditionals.Count > 0, iTagID);
                        iList.AddItem(iItem);
                        if (pBase.Effects[iTagID].isEnhancementEffect)
                            iList.SetUnique();
                    }
                    else if ((pBase.Effects[iTagID].MezType == Enums.eMez.ToggleDrop) &
                             (pBase.Effects[iTagID].Probability > 0.0))
                    {
                        var iValue = Convert.ToString(pBase.Effects[iTagID].Probability * 100f,
                            CultureInfo.InvariantCulture) + "%";
                        if ((pBase.Effects[iTagID].Suppression & MidsContext.Config.Suppression) !=
                            Enums.eSuppress.None)
                            iValue = "0%";
                        var iItem = new PairedList.ItemPair(CapString(names[(int)pBase.Effects[iTagID].MezType], 7) + ":", iValue, false, (pBase.Effects[iTagID].Probability < 1.0) | (pBase.Effects[iTagID].ValidateConditional("active", "Combo")), pBase.Effects[iTagID].ActiveConditionals.Count > 0, iTagID);
                        iList.AddItem(iItem);
                        if (pBase.Effects[iTagID].isEnhancementEffect)
                            iList.SetUnique();
                    }
                    else
                    {
                        var iAlternate2 =
                            (Math.Abs(pBase.Effects[iTagID].Duration - (double)pEnh.Effects[iTagID].Duration) >
                             float.Epsilon) |
                            (!Enums.MezDurationEnhancable(pBase.Effects[iTagID].MezType) &
                             (Math.Abs(pEnh.Effects[iTagID].BuffedMag - (double)pBase.Effects[iTagID].BuffedMag) > float.Epsilon));
                        var iValue = "Mag " + Utilities.FixDP(pEnh.Effects[iTagID].BuffedMag) + str;
                        if ((pBase.Effects[iTagID].Suppression & MidsContext.Config.Suppression) !=
                            Enums.eSuppress.None)
                            iValue = "0";
                        var iItem = new PairedList.ItemPair(
                            CapString(names[(int)pBase.Effects[iTagID].MezType], 7) + ":", iValue, iAlternate2,
                            pBase.Effects[iTagID].Probability < 1.0,
                            pBase.Effects[iTagID].ActiveConditionals.Count > 0, iTagID);
                        iList.AddItem(iItem);
                        if (pBase.Effects[iTagID].isEnhancementEffect)
                            iList.SetUnique();
                    }

                    ++num1;
                }
            }

            if (!shortFx1.Present)
                return num1;
            {
                var num2 = pBase.Effects.Length - 1;
                for (var iTagID = 0; iTagID <= num2; ++iTagID)
                {
                    if (!(((pBase.Effects[iTagID].PvMode != Enums.ePvX.PvP) & !MidsContext.Config.Inc.DisablePvE) |
                          ((pBase.Effects[iTagID].PvMode != Enums.ePvX.PvE) & MidsContext.Config.Inc.DisablePvE)) ||
                        !((pBase.Effects[iTagID].EffectType == Enums.eEffectType.MezResist) &
                          (pBase.Effects[iTagID].Probability > 0.0)))
                        continue;
                    var str = (double)pEnh.Effects[iTagID].Duration >= 15.0
                        ? " - " + Utilities.FixDP(pEnh.Effects[iTagID].Duration) + "s"
                        : string.Empty;
                    var iValue = Convert.ToString(pBase.Effects[iTagID].MagPercent, CultureInfo.InvariantCulture) +
                                 "%" + str;
                    if ((pBase.Effects[iTagID].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                        iValue = "0%";
                    var iItem = new PairedList.ItemPair(
                        CapString("-" + names[(int)pBase.Effects[iTagID].MezType], 7) + ":", iValue, false, false,
                        false, iTagID);
                    iList.AddItem(iItem);
                    if (pBase.Effects[iTagID].isEnhancementEffect)
                        iList.SetUnique();
                    ++num1;
                }
            }
            return num1;
        }

        private int effects_Summon(Label iLabel, PairedList iList)
        {
            var num1 = 0;
            var flag = iList.ItemCount == 0;
            var num2 = pBase.Effects.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                if (!((pBase.Effects[index].EffectType == Enums.eEffectType.EntCreate) &
                      (pBase.Effects[index].Probability > 0.0)))
                    continue;
                var iValue = pEnh.Effects[index].SummonedEntityName;
                if (iValue.StartsWith("MastermindPets_"))
                    iValue = iValue.Replace("MastermindPets_", string.Empty);
                if (iValue.StartsWith("Pets_"))
                    iValue = iValue.Replace("Pets_", string.Empty);
                if (iValue.StartsWith("Villain_Pets_"))
                    iValue = iValue.Replace("Villain_Pets_", string.Empty);
                var iTip = pEnh.Effects[index].BuildEffectString();
                if ((pBase.Effects[index].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                    iValue = "(suppressed)";
                var iItem =
                    new PairedList.ItemPair("Summon:", iValue, false, pBase.Effects[index].Probability < 1.0, false,
                        iTip);
                iList.AddItem(iItem);
                if (pBase.Effects[index].isEnhancementEffect)
                    iList.SetUnique();
                ++num1;
            }

            if (num1 > 0 && flag)
                iLabel.Text = "Summoned Entities";
            return num1;
        }

        private void EffectsDef()
        {
            var effectMagSum = pEnh.GetEffectMagSum(Enums.eEffectType.Defense, true);
            if (effectMagSum.Value == null)
                return;
            var eDamage = Enums.eDamage.None;
            effectMagSum.Multiply();
            var flag1 = false;
            var flag2 = false;
            var num1 = effectMagSum.Value.Length - 1;
            for (var index = 0; index <= num1; ++index)
            {
                if (effectMagSum.Value[index] > 0.0)
                    flag1 = true;
                if (effectMagSum.Value[index] < 0.0)
                    flag2 = true;
            }

            var buffDebuff = !(flag1 & flag2) ? !flag1 ? !flag2 ? 0 : -1 : 1 : 1;
            var def1 = pBase.GetDef(buffDebuff);
            var def2 = pEnh.GetDef(buffDebuff);
            var names = Enum.GetNames(eDamage.GetType());
            if (pBase.AffectsTarget(Enums.eEffectType.Defense))
            {
                fx_lblHead1.Text = @"Defense (Target)";
            }
            else if (pBase.AffectsAlly(Enums.eEffectType.Defense))
            {
                fx_lblHead1.Text = @"Defense (Ally)";
            }
            else if (pBase.AffectsSelf(Enums.eEffectType.Defense))
            {
                fx_lblHead1.Text = @"Defense (Self)";
            }
            //fx_lblHead1.Text = pBase.AffectsTarget(Enums.eEffectType.Defense) ? "Defense (Target)" : "Defense (Self)";
            fx_List1.ValueWidth = 55;
            var num2 = def1.Length - 1;
            for (var index = 0; index <= num2; ++index)
                def1[index] *= 100f;
            var num3 = def2.Length - 1;
            for (var index = 0; index <= num3; ++index)
                def2[index] *= 100f;
            var multiple = effectMagSum.Multiple;
            var iSub1 = Enums.eDamage.Smashing;
            if (multiple)
                effectMagSum.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Defense, iSub1, true));
            fx_List1.AddItem(FastItem(names[(int)iSub1], def1[(int)iSub1], def2[(int)iSub1], "%", false, true, false,
                false, effectMagSum));
            if (sFXCheck(effectMagSum))
                fx_List1.SetUnique();
            var iSub2 = Enums.eDamage.Fire;
            if (multiple)
                effectMagSum.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Defense, iSub2, true));
            fx_List1.AddItem(FastItem(names[(int)iSub2], def1[(int)iSub2], def2[(int)iSub2], "%", false, true, false,
                false, effectMagSum));
            if (sFXCheck(effectMagSum))
                fx_List1.SetUnique();
            var iSub3 = Enums.eDamage.Lethal;
            if (multiple)
                effectMagSum.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Defense, iSub3, true));
            fx_List1.AddItem(FastItem(names[(int)iSub3], def1[(int)iSub3], def2[(int)iSub3], "%", false, true, false,
                false, effectMagSum));
            if (sFXCheck(effectMagSum))
                fx_List1.SetUnique();
            var iSub4 = Enums.eDamage.Cold;
            if (multiple)
                effectMagSum.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Defense, iSub4, true));
            fx_List1.AddItem(FastItem(names[(int)iSub4], def1[(int)iSub4], def2[(int)iSub4], "%", false, true, false,
                false, effectMagSum));
            if (sFXCheck(effectMagSum))
                fx_List1.SetUnique();
            var iSub5 = Enums.eDamage.Energy;
            if (multiple)
                effectMagSum.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Defense, iSub5, true));
            fx_List1.AddItem(FastItem(names[(int)iSub5], def1[(int)iSub5], def2[(int)iSub5], "%", false, true, false,
                false, effectMagSum));
            if (sFXCheck(effectMagSum))
                fx_List1.SetUnique();
            var iSub6 = Enums.eDamage.Melee;
            if (multiple)
                effectMagSum.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Defense, iSub6, true));
            fx_List1.AddItem(FastItem(names[(int)iSub6], def1[(int)iSub6], def2[(int)iSub6], "%", false, true, false,
                false, effectMagSum));
            if (sFXCheck(effectMagSum))
                fx_List1.SetUnique();
            var iSub7 = Enums.eDamage.Negative;
            if (multiple)
                effectMagSum.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Defense, iSub7, true));
            fx_List1.AddItem(FastItem(names[(int)iSub7], def1[(int)iSub7], def2[(int)iSub7], "%", false, true, false,
                false, effectMagSum));
            if (sFXCheck(effectMagSum))
                fx_List1.SetUnique();
            var iSub8 = Enums.eDamage.Ranged;
            if (multiple)
                effectMagSum.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Defense, iSub8, true));
            fx_List1.AddItem(FastItem(names[(int)iSub8], def1[(int)iSub8], def2[(int)iSub8], "%", false, true, false,
                false, effectMagSum));
            if (sFXCheck(effectMagSum))
                fx_List1.SetUnique();
            var iSub9 = Enums.eDamage.Psionic;
            if (multiple)
                effectMagSum.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Defense, iSub9, true));
            fx_List1.AddItem(FastItem(names[(int)iSub9], def1[(int)iSub9], def2[(int)iSub9], "%", false, true, false,
                false, effectMagSum));
            if (sFXCheck(effectMagSum))
                fx_List1.SetUnique();
            var iSub10 = Enums.eDamage.AoE;
            if (multiple)
                effectMagSum.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Defense, iSub10, true));
            fx_List1.AddItem(FastItem(names[(int)iSub10], def1[(int)iSub10], def2[(int)iSub10], "%", false, true,
                false, false,
                effectMagSum));
            if (sFXCheck(effectMagSum))
                fx_List1.SetUnique();
        }

        private int EffectsDrh()
        {
            var index = 0;
            if (pBase.HasDefEffects())
            {
                EffectsDef();
                ++index;
            }

            if (!pBase.HasResEffects())
                return index;
            EffectsRes(index);
            ++index;
            return index;
        }

        private void EffectsRes(int index)
        {
            var eDamage = Enums.eDamage.None;
            var res1 = pBase.GetRes(!MidsContext.Config.Inc.DisablePvE);
            var res2 = pEnh.GetRes(!MidsContext.Config.Inc.DisablePvE);
            var names = Enum.GetNames(eDamage.GetType());
            Label label;
            PairedList PairedList;
            if (index == 0)
            {
                label = fx_lblHead1;
                PairedList = fx_List1;
            }
            else
            {
                label = fx_lblHead2;
                PairedList = fx_List2;
            }

            if (pBase.AffectsTarget(Enums.eEffectType.Resistance))
            {
                label.Text = @"Resistance (Target)";
            }
            else if (pBase.AffectsAlly(Enums.eEffectType.Resistance))
            {
                label.Text = @"Resistance (Ally)";
            }
            else if (pBase.AffectsSelf(Enums.eEffectType.Resistance))
            {
                label.Text = @"Resistance (Self)";
            }
            //label.Text = pBase.AffectsTarget(Enums.eEffectType.Resistance) ? "Resistance (Target)" : "Resistance (Self)";
            fx_List2.ValueWidth = 55;
            var shortFx = new Enums.ShortFX();
            shortFx.Multiply();
            var num1 = res1.Length - 1;
            for (var index1 = 0; index1 <= num1; ++index1)
                res1[index1] *= 100f;
            var num2 = res2.Length - 1;
            for (var index1 = 0; index1 <= num2; ++index1)
                res2[index1] *= 100f;
            var iSub1 = Enums.eDamage.Smashing;
            shortFx.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Resistance, iSub1, true));
            PairedList.AddItem(FastItem(names[(int)iSub1], res1[(int)iSub1], res2[(int)iSub1], "%", false, true,
                false, false, shortFx));
            if (sFXCheck(shortFx))
                PairedList.SetUnique();
            var iSub2 = Enums.eDamage.Fire;
            shortFx.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Resistance, iSub2, true));
            PairedList.AddItem(FastItem(names[(int)iSub2], res1[(int)iSub2], res2[(int)iSub2], "%", false, true,
                false, false, shortFx));
            if (sFXCheck(shortFx))
                PairedList.SetUnique();
            var iSub3 = Enums.eDamage.Lethal;
            shortFx.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Resistance, iSub3, true));
            PairedList.AddItem(FastItem(names[(int)iSub3], res1[(int)iSub3], res2[(int)iSub3], "%", false, true,
                false, false, shortFx));
            if (sFXCheck(shortFx))
                PairedList.SetUnique();
            var iSub4 = Enums.eDamage.Cold;
            shortFx.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Resistance, iSub4, true));
            PairedList.AddItem(FastItem(names[(int)iSub4], res1[(int)iSub4], res2[(int)iSub4], "%", false, true,
                false, false, shortFx));
            if (sFXCheck(shortFx))
                PairedList.SetUnique();
            var iSub5 = Enums.eDamage.Energy;
            shortFx.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Resistance, iSub5, true));
            PairedList.AddItem(FastItem(names[(int)iSub5], res1[(int)iSub5], res2[(int)iSub5], "%", false, true,
                false, false, shortFx));
            if (sFXCheck(shortFx))
                PairedList.SetUnique();
            var iSub6 = Enums.eDamage.Psionic;
            shortFx.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Resistance, iSub6, true));
            PairedList.AddItem(FastItem(names[(int)iSub6], res1[(int)iSub6], res2[(int)iSub6], "%", false, true,
                false, false, shortFx));
            if (sFXCheck(shortFx))
                PairedList.SetUnique();
            var iSub7 = Enums.eDamage.Negative;
            shortFx.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Resistance, iSub7, true));
            PairedList.AddItem(FastItem(names[(int)iSub7], res1[(int)iSub7], res2[(int)iSub7], "%", false, true,
                false, false, shortFx));
            if (sFXCheck(shortFx))
                PairedList.SetUnique();
            var iSub8 = Enums.eDamage.Toxic;
            shortFx.Assign(pEnh.GetDamageMagSum(Enums.eEffectType.Resistance, iSub8, true));
            PairedList.AddItem(FastItem(names[(int)iSub8], res1[(int)iSub8], res2[(int)iSub8], "%", false, true,
                false, false, shortFx));
        }

        private static PairedList.ItemPair FastItem(string Title, Enums.ShortFX s1, Enums.ShortFX s2, string Suffix, Enums.ShortFX Tag)
        {
            return FastItem(Title, s1, s2, Suffix, false, false, false, false, Tag);
        }

        private static PairedList.ItemPair FastItem(string Title, float s1, float s2, string Suffix, string Tip) {
            return FastItem(Title, s1, s2, Suffix, false, false, false, false, Tip);
        }
        
        private static PairedList.ItemPair FastItem(string Title, Enums.ShortFX s1, Enums.ShortFX s2, string Suffix, bool SkipBase, bool AlwaysShow, bool isChance, bool isSpecial, string Tip)
        {
            var iValue = Utilities.FixDP(s2.Sum) + Suffix;
            PairedList.ItemPair iItem;
            if ((Math.Abs(s1.Sum) < float.Epsilon) & !AlwaysShow)
            {
                iItem = new PairedList.ItemPair(string.Empty, string.Empty, false);
            }
            else if (Math.Abs(s1.Sum) < float.Epsilon)
            {
                iItem = new PairedList.ItemPair(Title + ":", string.Empty, false);
            }
            else
            {
                bool iAlternate;
                if (Math.Abs(s1.Sum - (double)s2.Sum) > float.Epsilon)
                {
                    if (!SkipBase)
                    {
                        var iValue2 = $"({Utilities.FixDP(s2.Sum)}{Suffix})";
                        iValue += iValue2.Replace("%", "");
                    }

                    iAlternate = true;
                }
                else
                {
                    iAlternate = false;
                }

                iItem = new PairedList.ItemPair($"{Title}:", iValue, iAlternate, isChance, isSpecial, Tip);
            }

            return iItem;
        }

        private static PairedList.ItemPair FastItem(string Title, Enums.ShortFX s1, Enums.ShortFX s2, string Suffix, bool SkipBase, bool AlwaysShow, bool isChance, bool isSpecial, Enums.ShortFX Tag)
        {
            var iValue = Utilities.FixDP(s2.Sum) + Suffix;
            PairedList.ItemPair itemPair;
            if ((Math.Abs(s1.Sum) < float.Epsilon) & !AlwaysShow)
            {
                itemPair = new PairedList.ItemPair(string.Empty, string.Empty, false);
            }
            else if (Math.Abs(s1.Sum) < float.Epsilon)
            {
                itemPair = new PairedList.ItemPair(Title + ":", string.Empty, false);
            }
            else
            {
                bool iAlternate;
                if (Math.Abs(s1.Sum - (double)s2.Sum) > float.Epsilon)
                {
                    if (!SkipBase)
                        iValue = iValue + " (" + Utilities.FixDP(s1.Sum) + ")";
                    iAlternate = true;
                }
                else
                {
                    iAlternate = false;
                }

                itemPair = new PairedList.ItemPair($"Title:", iValue, iAlternate, isChance, isSpecial, Tag);
            }

            return itemPair;
        }

        private static PairedList.ItemPair FastItem(string Title, float s1, float s2, string Suffix, bool SkipBase, bool AlwaysShow, bool isChance, bool isSpecial, Enums.ShortFX Tag)
        {
            var iValue = Utilities.FixDP(s2) + Suffix;
            PairedList.ItemPair itemPair;
            if ((Math.Abs(s1) < float.Epsilon) & !AlwaysShow)
            {
                itemPair = new PairedList.ItemPair(string.Empty, string.Empty, false);
            }
            else if (Math.Abs(s1) < float.Epsilon)
            {
                itemPair = new PairedList.ItemPair($"Title:", string.Empty, false);
            }
            else
            {
                bool iAlternate;
                if (Math.Abs(s1 - (double)s2) > float.Epsilon)
                {
                    if (!SkipBase)
                        iValue = $"{iValue} ({Utilities.FixDP(s1)})";
                    iAlternate = true;
                }
                else
                {
                    iAlternate = false;
                }

                itemPair = new PairedList.ItemPair($"{Title}:", iValue, iAlternate, isChance, isSpecial, Tag);
            }

            return itemPair;
        }

        
        private static PairedList.ItemPair FastItem(string Title, float s1, float s2, string Suffix, bool SkipBase, bool AlwaysShow, bool isChance, bool isSpecial, string Tip)
        {
            var iValue = Utilities.FixDP(s2) + Suffix;
            PairedList.ItemPair itemPair;
            if ((Math.Abs(s1) < float.Epsilon) & !AlwaysShow)
            {
                itemPair = new PairedList.ItemPair(string.Empty, string.Empty, false);
            }
            else if (Math.Abs(s1) < float.Epsilon)
            {
                itemPair = new PairedList.ItemPair($"{Title}:", string.Empty, false);
            }
            else
            {
                bool iAlternate;
                if (Math.Abs(s1 - (double)s2) > float.Epsilon)
                {
                    if (!SkipBase)
                        iValue = $"{iValue} ({Utilities.FixDP(s1)})";
                    iAlternate = true;
                }
                else
                {
                    iAlternate = false;
                }

                itemPair = new PairedList.ItemPair($"{Title}:", iValue, iAlternate, isChance, isSpecial, Tip);
            }

            return itemPair;
        }

        private static PairedList.ItemPair FastItem(string Title, float s1, float s2, string Suffix, bool SkipBase = false, bool AlwaysShow = false, bool isChance = false, bool isSpecial = false, int TagID = -1, int maxDecimal = -1)
        {
            var iValue = maxDecimal < 0 ? Utilities.FixDP(s2) + Suffix : Utilities.FixDP(s2, maxDecimal) + Suffix;
            PairedList.ItemPair itemPair;
            if ((Math.Abs(s1) < float.Epsilon) & !AlwaysShow)
            {
                itemPair = new PairedList.ItemPair(string.Empty, string.Empty, false);
            }
            else
            {
                bool iAlternate;
                if (Math.Abs(s1 - (double)s2) > float.Epsilon)
                {
                    if (!SkipBase)
                        iValue = $"{iValue} ({Utilities.FixDP(s1)})";
                    iAlternate = true;
                }
                else
                {
                    iAlternate = false;
                }

                itemPair = new PairedList.ItemPair($"{Title}:", iValue, iAlternate, isChance, isSpecial, TagID);
            }

            return itemPair;
        }

        private static PairedList.ItemPair FastItem(string Title, float s1, float s2, string Suffix, string tip, bool SkipBase = false, bool AlwaysShow = false, bool isChance = false, bool isSpecial = false, int maxDecimal = -1)
        {
            var iValue = maxDecimal < 0 ? Utilities.FixDP(s2) + Suffix : Utilities.FixDP(s2, maxDecimal) + Suffix;
            PairedList.ItemPair itemPair;
            if ((Math.Abs(s1) < float.Epsilon) & !AlwaysShow)
            {
                itemPair = new PairedList.ItemPair(string.Empty, string.Empty, false);
            }
            else
            {
                bool iAlternate;
                if (Math.Abs(s1 - (double)s2) > float.Epsilon)
                {
                    if (!SkipBase)
                        iValue = iValue + " (" + Utilities.FixDP(s1) + ")";
                    iAlternate = true;
                }
                else
                {
                    iAlternate = false;
                }

                itemPair = new PairedList.ItemPair($"{Title}:", iValue, iAlternate, isChance, isSpecial, tip);
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
            var Title = string.Empty;
            var shortFxBase = new Enums.ShortFX();
            var shortFxEnh = new Enums.ShortFX();
            var tag2 = new Enums.ShortFX();
            var Suffix = string.Empty;

            if (Index[ID] > -1)
            {
                var eEffectTypeShort = Enums.eEffectTypeShort.None;
                var flag = false;
                var onlySelf = pBase.Effects[Index[ID]].ToWho == Enums.eToWho.Self;
                var onlyTarget = pBase.Effects[Index[ID]].ToWho == Enums.eToWho.Target;
                var onlyAlly = pBase.Effects[Index[ID]].ToWho == Enums.eToWho.Ally;
                if (ID > 0)
                {
                    flag = (pBase.Effects[Index[ID]].EffectType == pBase.Effects[Index[ID - 1]].EffectType) &
                           (pBase.Effects[Index[ID]].ToWho == Enums.eToWho.Self) &
                           (pBase.Effects[Index[ID - 1]].ToWho == Enums.eToWho.Self) &
                           (pBase.Effects[Index[ID]].ToWho == Enums.eToWho.Target) &
                           (pBase.Effects[Index[ID]].ToWho == Enums.eToWho.Ally);
                }

                if (pBase.Effects[Index[ID]].DelayedTime > 5.0)
                    flag = true;
                var names = Enum.GetNames(eEffectTypeShort.GetType());
                if (pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.Enhancement)
                {
                    Title = pBase.Effects[Index[ID]].ETModifies switch
                    {
                        Enums.eEffectType.EnduranceDiscount => "+EndRdx",
                        Enums.eEffectType.RechargeTime => "+Rechg",
                        Enums.eEffectType.Mez => pBase.Effects[Index[ID]].MezType == Enums.eMez.None
                            ? "+Effects"
                            : $"Enh({Enum.GetName(Enums.eMezShort.None.GetType(), pBase.Effects[Index[ID]].MezType)})",
                        Enums.eEffectType.Defense => "Enh(Def)",
                        Enums.eEffectType.Resistance => "Enh(Res)",
                        _ => CapString(Enum.GetName(pBase.Effects[Index[ID]].ETModifies.GetType(), pBase.Effects[Index[ID]].ETModifies), 7)
                    };
                }
                else
                {
                    Title = pBase.Effects[Index[ID]].EffectType != Enums.eEffectType.Mez ? names[(int)pBase.Effects[Index[ID]].EffectType] : Enums.GetMezName((Enums.eMezShort)pBase.Effects[Index[ID]].MezType);
                }
                
                string temp = string.Empty;
                switch (pBase.Effects[Index[ID]].EffectType)
                {
                    case Enums.eEffectType.HitPoints:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.HitPoints, false, onlySelf, onlyTarget, onlyAlly));
                        shortFxEnh.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.HitPoints, false, onlySelf, onlyTarget, onlyAlly));
                        tag2.Assign(shortFxBase);
                        shortFxBase.Sum = (float)(shortFxBase.Sum / (double)MidsContext.Archetype.Hitpoints * 100.0);
                        shortFxEnh.Sum = (float)(shortFxEnh.Sum / (double)MidsContext.Archetype.Hitpoints * 100.0);
                        Suffix = "%";
                        break;
                    case Enums.eEffectType.Heal:
                        if (pBase.Effects[Index[ID]].BuffedMag <= 1)
                        {
                            temp = $"{pBase.Effects[Index[ID]].BuffedMag:P2}";
                            shortFxBase.Add(Index[ID], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(Index[ID], Convert.ToSingle(temp.Replace("%", "")));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Heal, false, onlySelf, onlyTarget, onlyAlly));
                            shortFxEnh.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Heal, false, onlySelf, onlyTarget, onlyAlly));
                            shortFxBase.Sum = (float)(shortFxBase.Sum / (double)MidsContext.Archetype.Hitpoints * 100.0);
                            shortFxEnh.Sum = (float)(shortFxEnh.Sum / (double)MidsContext.Archetype.Hitpoints * 100.0);
                            tag2.Assign(shortFxBase);
                        }
                        Suffix = "%";
                        break;
                    case Enums.eEffectType.Absorb:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Absorb, false, onlySelf, onlyTarget, onlyAlly));
                        shortFxEnh.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Absorb, false, onlySelf, onlyTarget, onlyAlly));
                        tag2.Assign(shortFxBase);
                        Suffix = "%";
                        break;
                    case Enums.eEffectType.Endurance:
                        if (pBase.Effects[Index[ID]].BuffedMag < -0.01 && pBase.Effects[Index[ID]].BuffedMag > -1)
                        {
                            temp = $"{pBase.Effects[Index[ID]].BuffedMag:P2}";
                            shortFxBase.Add(Index[ID], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(Index[ID], Convert.ToSingle(temp.Replace("%", "")));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Endurance, false, onlySelf, onlyTarget, onlyAlly));
                            shortFxEnh.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Endurance, false, onlySelf, onlyTarget, onlyAlly));
                            tag2.Assign(shortFxBase);
                        }
                        Suffix = "%";
                        break;
                    case Enums.eEffectType.Regeneration:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Regeneration, false, onlySelf, onlyTarget, onlyAlly));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Regeneration, false, onlySelf, onlyTarget, onlyAlly));
                        shortFxEnh.Sum *= 100;
                        tag2.Assign(shortFxBase);
                        Suffix = "%";
                        break;
                    case Enums.eEffectType.Null:
                        if (pBase.Effects[Index[ID]].BuffedMag < 1)
                        {
                            temp = $"{pBase.Effects[Index[ID]].BuffedMag:P2}";
                            shortFxBase.Add(Index[ID], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(Index[ID], Convert.ToSingle(temp.Replace("%", "")));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Null, false, onlySelf, onlyTarget, onlyAlly));
                            shortFxEnh.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Null, false, onlySelf, onlyTarget, onlyAlly));
                            tag2.Assign(shortFxBase);
                        }
                        Suffix = "%";
                        break;
                    case Enums.eEffectType.ToHit:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.ToHit, false, onlySelf, onlyTarget, onlyAlly));
                        shortFxEnh.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.ToHit, false, onlySelf, onlyTarget, onlyAlly));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        Suffix = "%";
                        break;
                    case Enums.eEffectType.Fly:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Fly, false, onlySelf, onlyTarget, onlyAlly));
                        shortFxEnh.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Fly, false, onlySelf, onlyTarget, onlyAlly));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        Suffix = "%";
                        break;
                    case Enums.eEffectType.Recovery:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Recovery, false, onlySelf, onlyTarget, onlyAlly));
                        shortFxEnh.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.Recovery, false, onlySelf, onlyTarget, onlyAlly));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        Suffix = "%";
                        break;
                    default:
                        if ((pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.Mez) &
                            ((pBase.Effects[Index[ID]].MezType == Enums.eMez.Taunt) |
                             (pBase.Effects[Index[ID]].MezType == Enums.eMez.Placate)))
                        {
                            shortFxBase.Add(Index[ID], pBase.Effects[Index[ID]].Duration);
                            shortFxEnh.Add(Index[ID], pEnh.Effects[Index[ID]].Duration);
                            tag2.Assign(shortFxBase);
                            Suffix = "s";
                        }
                        /*else if (pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.SpeedFlying)
                        {
                            shortFx.Assign(pBase.GetEffectMagSum(Enums.eEffectType.SpeedFlying, false, onlySelf, onlyTarget));
                            s2.Assign(pEnh.GetEffectMagSum(Enums.eEffectType.SpeedFlying, false, onlySelf, onlyTarget));
                            Tag2.Assign(shortFx);
                        }*/
                        // Set list of effects below that are treated as percentages
                        // Base and enhanced values will be multiplied by 100
                        else if (pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.DamageBuff |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.Defense |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.Resistance |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.ResEffect |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.Enhancement |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.MezResist |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.RechargeTime |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.SpeedFlying |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.SpeedRunning |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.SpeedJumping |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.JumpHeight |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.PerceptionRadius |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.Meter |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.Range |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.MaxFlySpeed |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.MaxRunSpeed |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.MaxJumpSpeed |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.Jumppack |
                                 pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.GlobalChanceMod)
                        {
                            /*if (!pBase.Effects[Index[ID]].Absorbed_Effect)
                            {
                                shortFx.Add(Index[ID], pBase.Effects[Index[ID]].BuffedMag);
                                s2.Add(Index[ID], pEnh.Effects[Index[ID]].BuffedMag);
                                shortFx.Multiply();
                                s2.Multiply();
                                Tag2.Assign(pEnh.GetEffectMagSum(pBase.Effects[Index[ID]].EffectType, false, onlySelf, onlyTarget, onlyAlly, false));
                            }
                            else
                            {
                                shortFx.Add(Index[ID], pBase.Effects[Index[ID]].BuffedMag);
                                s2.Add(Index[ID], pEnh.Effects[Index[ID]].BuffedMag);
                                s2.Multiply();
                                Tag2.Assign(pEnh.GetEffectMagSum(pBase.Effects[Index[ID]].EffectType, false, onlySelf, onlyTarget, onlyAlly, false));
                            }*/

                            shortFxBase.Add(Index[ID], pBase.Effects[Index[ID]].BuffedMag);
                            shortFxEnh.Add(Index[ID], pEnh.Effects[Index[ID]].BuffedMag);
                            shortFxBase.Multiply();
                            shortFxEnh.Multiply();
                            tag2.Assign(pEnh.GetEffectMagSum(pBase.Effects[Index[ID]].EffectType, false, onlySelf, onlyTarget, onlyAlly));
                        }
                        else if (pBase.Effects[Index[ID]].EffectType == Enums.eEffectType.SilentKill)
                        {
                            shortFxBase.Add(Index[ID], pBase.Effects[Index[ID]].Absorbed_Duration);
                            shortFxEnh.Add(Index[ID], pEnh.Effects[Index[ID]].Absorbed_Duration);
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Add(Index[ID], pBase.Effects[Index[ID]].BuffedMag);
                            shortFxEnh.Add(Index[ID], pEnh.Effects[Index[ID]].BuffedMag);
                            tag2.Assign(shortFxBase);
                        }

                        break;
                }
                
                if (pBase.Effects[Index[ID]].DisplayPercentage)
                {
                    Suffix = "%";
                }

                Suffix += pBase.Effects[Index[ID]].ToWho switch
                {
                    Enums.eToWho.Target => " (Tgt)",
                    Enums.eToWho.Self => " (Self)",
                    Enums.eToWho.Ally => " (Ally)",
                    _ => ""
                };

                if (flag)
                    return FastItem("", 0.0f, 0.0f, string.Empty);
            }

            for (var index = 0; index < shortFxEnh.Index.Length; index++)
            {
                if (shortFxEnh.Index[index] <= -1 || !pBase.Effects[shortFxEnh.Index[index]].DisplayPercentage)
                    continue;
                if (shortFxEnh.Value[index] > 1)
                    continue;

                if (pBase.Effects[shortFxEnh.Index[index]].EffectType == Enums.eEffectType.Absorb)
                {
                    //Fixes the Absorb display to correctly show the percentage
                    shortFxEnh.Sum = float.Parse(shortFxEnh.Sum.ToString("P", CultureInfo.InvariantCulture).Replace("%", ""));
                }
                else if (pBase.Effects[shortFxEnh.Index[index]].EffectType == Enums.eEffectType.ToHit)
                {
                    //Fixes the ToHit display to correctly show the percentage
                    if (pBase.Effects[shortFxEnh.Index[index]].Stacking == Enums.eStacking.Yes)
                    {
                        var overage = pBase.Effects[Index[ID]].Ticks * 0.05f;
                        shortFxEnh.Sum -= overage;
                        shortFxEnh.Sum /= 2;
                    }
                }
                else
                {
                    shortFxEnh.ReSum();
                }

                break;
            }

            var iTip = GetToolTip(shortFxEnh);
            if (pBase.Effects[Index[ID]].ActiveConditionals.Count > 0)
            {
                return FastItem(Title, shortFxBase, shortFxEnh, Suffix, true, false, pBase.Effects[Index[ID]].Probability < 1.0, pBase.Effects[Index[ID]].ActiveConditionals.Count > 0, iTip); //
            }

            if (pBase.Effects[Index[ID]].SpecialCase != Enums.eSpecialCase.None)
            {
                return FastItem(Title, shortFxBase, shortFxEnh, Suffix, true, false, pBase.Effects[Index[ID]].Probability < 1.0, pBase.Effects[Index[ID]].SpecialCase != Enums.eSpecialCase.None, iTip);
            }

            return FastItem(Title, shortFxBase, shortFxEnh, Suffix, true, false, pBase.Effects[Index[ID]].Probability < 1.0, false, iTip);
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

        private void PairedList_Hover(object Sender, int Index, Enums.ShortFX Tag2, string tooltip)
        {
            var empty1 = string.Empty;
            string str1;
            if (Tag2.Present)
            {
                var numArray = new int[0];
                var empty2 = string.Empty;
                IPower power = new Power(pEnh);
                var num1 = Tag2.Index.Length - 1;
                for (var index1 = 0; index1 <= num1; ++index1)
                {
                    if (Tag2.Index[index1] == -1 ||
                        power.Effects[Tag2.Index[index1]].EffectType == Enums.eEffectType.None)
                        continue;
                    var empty3 = string.Empty;
                    var returnMask = new int[0];
                    power.GetEffectStringGrouped(Tag2.Index[index1], ref empty3, ref returnMask, false, false);
                    if (returnMask.Length <= 0)
                        continue;
                    if (empty2 != string.Empty)
                        empty2 += "\r\n";
                    empty2 += empty3;
                    var num2 = returnMask.Length - 1;
                    for (var index2 = 0; index2 <= num2; ++index2)
                        power.Effects[returnMask[index2]].EffectType = Enums.eEffectType.None;
                }

                var num3 = Tag2.Index.Length - 1;
                for (var index = 0; index <= num3; ++index)
                {
                    if (power.Effects[Tag2.Index[index]].EffectType == Enums.eEffectType.None)
                        continue;
                    if (empty2 != string.Empty)
                        empty2 += "\r\n";
                    empty2 += power.Effects[Tag2.Index[index]].BuildEffectString();
                }

                str1 = empty1 + empty2;
            }
            else if (string.IsNullOrWhiteSpace(tooltip))
            {
                str1 = "No Valid Tip";
            }
            else
            {
                str1 = tooltip;
            }

            object[] Arguments = { str1 };
            bool[] CopyBack = { true };


            NewLateBinding.LateCall(Sender, null, "SetTip", Arguments, null, null, CopyBack, true);

            if (!CopyBack[0])
            {
            }
            else
            {
                var str2 = (string)Convert.ChangeType(RuntimeHelpers.GetObjectValue(Arguments[0]), typeof(string));
            }
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
            var Index = 0;
            while (!((e.X >= clipRect.X) & (e.X <= clipRect.Width + clipRect.X)))
            {
                clipRect.X += clipRect.Width;
                ++Index;
                if (Index > 3)
                    return;
            }

            if (Index != TabPage)
            {
                var tabChanged = TabChanged;
                tabChanged?.Invoke(Index);
            }

            TabPage = Index;
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
                display_Info();
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
            var iTip = string.Empty;
            var num1 = -1;
            var num2 = -1;
            var num3 = 0;
            var num4 = pEnh.Effects.Length - 1;
            for (var index = 0; index <= num4; ++index)
            {
                var effect = pEnh.Effects[index];
                if (effect.EffectType != Enums.eEffectType.Damage)
                    continue;
                if (effect.CanInclude() & pEnh.Effects[index].PvXInclude())
                {
                    if (iTip != string.Empty)
                        iTip += "\r\n";
                    var str = pEnh.Effects[index].BuildEffectString();
                    if (pEnh.Effects[index].isEnhancementEffect & (pEnh.PowerType == Enums.ePowerType.Toggle))
                    {
                        ++num1;
                        str += " (Special only every 10s)";
                    }
                    else if (pEnh.PowerType == Enums.ePowerType.Toggle)
                    {
                        ++num2;
                    }

                    iTip += str;
                }
                else
                {
                    ++num3;
                }
            }

            if (num3 > 0)
            {
                if (iTip != string.Empty)
                    iTip += "\r\n";
                iTip += "\r\nThis power deals different damage in PvP and PvE modes.";
            }

            if (!((pBase.PowerType == Enums.ePowerType.Toggle) & (num1 == -1) & (num2 == -1)) && (pBase.PowerType == Enums.ePowerType.Toggle) & (num2 > -1) && !string.IsNullOrEmpty(iTip))
            {
                iTip = "Applied every " + Convert.ToString(pBase.ActivatePeriod, CultureInfo.InvariantCulture) + "s:\r\n" + iTip;
            }

            Info_Damage.SetTip(iTip);
        }

        private Power? GetPowerRedirectParent(IPower pSrc)
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

        /*private void MergeRedirectEffects(ref IPower pSrc, IPower pSub = null, int rLevel = 0)
        {
            // Merge pSub effects into pSrc and follow redirects
            // (up to 5 levels recursion)
            // Look for redirects in pSrc and follow any if pSub == null

            if (pSub == null)
            {
                foreach (var fx in pSrc.Effects)
                {
                    if (fx.EffectType == Enums.eEffectType.PowerRedirect) continue;

                    // Check conditionals/GCM
                    pSub = DatabaseAPI.GetPowerByFullName(fx.Override);
                    if (pSub == null) continue;
                        
                    if (rLevel < 5) MergeRedirectEffects(ref pSrc, pSub, ++rLevel);
                }
            }
            else
            {
                pSrc.Accuracy = pSub.Accuracy;
                pSrc.ActivatePeriod = pSub.ActivatePeriod;
                pSrc.Arc = pSub.Arc;
                pSrc.CastTime = pSub.CastTime;
                pSrc.EffectArea = pSub.EffectArea;
                pSrc.EndCost = pSub.EndCost;
                pSrc.InterruptTime = pSub.InterruptTime;
                pSrc.MaxTargets = pSub.MaxTargets;
                pSrc.Radius = pSub.Radius;
                pSrc.Range = pSub.Range;
                pSrc.RechargeTime = pSub.RechargeTime;
                pSrc.RangeSecondary = pSub.RangeSecondary;
                pSrc.IgnoreEnh = (Enums.eEnhance[])pSub.IgnoreEnh.Clone();
                pSrc.Ignore_Buff = (Enums.eEnhance[])pSub.Ignore_Buff.Clone();

                foreach (var fx in pSub.Effects)
                {
                    if (fx.EffectType != Enums.eEffectType.PowerRedirect)
                    {
                        // Check conditionals/GCM
                        if (rLevel < 5) MergeRedirectEffects(ref pSrc, pSub, ++rLevel);
                    }

                    var srcEffects = pSrc.Effects.ToList();
                    srcEffects.Add(fx);
                    pSrc.Effects = srcEffects.ToArray();
                }
            }
        }*/

        public void SetData(IPower iBase, IPower iEnhanced, bool noLevel = false, bool Locked = false,
            int iHistoryIDX = -1)
        {
            if (iBase == null)
                return;
            Lock = Locked;
            pBase = new Power(iBase);

            var pBaseRedirectParent = GetPowerRedirectParent(pBase);
            if (pBaseRedirectParent != null && pBase.FullName != pBaseRedirectParent.FullName)
            {
                pBase = pBaseRedirectParent.Clone();
            }

            pEnh = iEnhanced == null
                ? new Power(iBase)
                : new Power(iEnhanced);

            var pEnhRedirectParent = GetPowerRedirectParent(pEnh);
            if (pEnhRedirectParent != null && pEnh.FullName != pEnhRedirectParent.FullName)
            {
                pEnh = pBaseRedirectParent.Clone();
            }

            var basePowerDb = DatabaseAPI.GetPowerByFullName(pBase.FullName);
            pBase.ActivatePeriod = basePowerDb.ActivatePeriod;
            pEnh.ActivatePeriod = basePowerDb.ActivatePeriod;

            HistoryIDX = iHistoryIDX;
            SetDamageTip();
            DisplayData(noLevel);
            SizeRefresh();
        }

        public void SetEnhancement(I9Slot iEnh, int iLevel = -1)
        {
            if (Lock & (TabPage != 3) || iLevel < 0)
                return;
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
                str1 = str1 + " (Slot Level " + Convert.ToString(iLevel + 1) + ")";
            info_Title.Text = str1;
            fx_Title.Text = str1;
            enhNameDisp.Text = str1;
            if (TabPage > 1 || iEnh.Enh < 0)
                return;
            var iStr1 = string.Empty;
            var str2 = string.Empty;
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
                    if ((DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance < 1.0) &
                        (DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance > 0.0))
                        str2 = str2 + RTF.Color(RTF.ElementID.Enhancement) +
                               Strings.Format(
                                   (float)(DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance * 100.0), "##0") +
                               "% chance of ";
                }
                else
                {
                    iStr1 = iStr1 + RTF.Color(RTF.ElementID.Enhancement) +
                            "Hamidon/Synthetic Hamidon Origin Enhancement";
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
                iStr2 = str2 + GetEnhancementStringLongRTF(iEnh) + "\r\n" +
                        EnhancementSetCollection.GetSetInfoLongRTF(DatabaseAPI.Database.Enhancements[iEnh.Enh].nIDSet);
            }
            else
            {
                var str3 = str2 + DatabaseAPI.Database.Enhancements[iEnh.Enh].Desc;
                if (str3 != string.Empty)
                    str3 += "\r\n";
                iStr2 = str3 + GetEnhancementStringLongRTF(iEnh);
            }

            info_txtSmall.Rtf = RTF.StartRTF() + RTF.ToRTF(iStr1) + RTF.Crlf() + RTF.Color(RTF.ElementID.Faded) +
                                "Shift+Click to move slot. Right-Click to place enh." + RTF.EndRTF();
            Info_txtLarge.Rtf = RTF.StartRTF() + RTF.ToRTF("aaa " + iStr2) + RTF.EndRTF();
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
                    if ((DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance > 1.0) &
                        (DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance > 0.0))
                        str1 = str1 + RTF.Color(RTF.ElementID.Enhancement) +
                               Strings.Format(
                                   (float)(DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance * 100.0), "##0") +
                               "% chance of ";
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
            Info_txtLarge.Rtf = RTF.StartRTF() + RTF.ToRTF("bbb " + iStr2) + RTF.EndRTF();
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
                Info_txtLarge.Text = string.Empty;
                info_txtSmall.Text = string.Empty;
            }
            else
            {
                info_Title.Text = DatabaseAPI.Database.EnhancementSets[iSet].DisplayName;
                var str1 = DatabaseAPI.Database.SetTypeStringLong[
                    (int)DatabaseAPI.Database.EnhancementSets[iSet].SetType];
                if (!Compact)
                    str1 += RTF.Crlf();
                var str2 = DatabaseAPI.Database.EnhancementSets[iSet].LevelMin !=
                           DatabaseAPI.Database.EnhancementSets[iSet].LevelMax
                    ? Convert.ToString(DatabaseAPI.Database.EnhancementSets[iSet].LevelMin + 1) + " to " +
                      Convert.ToString(DatabaseAPI.Database.EnhancementSets[iSet].LevelMax + 1)
                    : Convert.ToString(DatabaseAPI.Database.EnhancementSets[iSet].LevelMin + 1);
                info_txtSmall.Rtf = RTF.StartRTF() +
                                    str1 + RTF.Color(RTF.ElementID.Invention) + "Level: " + str2 +
                                    RTF.Color(RTF.ElementID.Text) +
                                    RTF.EndRTF();
                Info_txtLarge.Rtf = RTF.StartRTF() + EnhancementSetCollection.GetSetInfoLongRTF(iSet) + RTF.EndRTF();
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

        private bool SplitFX_AddToList(
            ref Enums.ShortFX BaseSFX,
            ref Enums.ShortFX EnhSFX,
            ref PairedList iList,
            string SpecialTitle = "")
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
                var num1 = shortFxArray1.Length - 1;
                for (var index = 0; index <= num1; ++index)
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

                    var Title = Enums.GetEffectNameShort(pEnh.Effects[shortFxArray1[index].Index[0]].EffectType);
                    if (SpecialTitle != string.Empty)
                        Title = SpecialTitle;
                    var s1 = num2;
                    var s2 = num3;
                    if ((pEnh.Effects[shortFxArray1[index].Index[0]].Suppression & MidsContext.Config.Suppression) !=
                        Enums.eSuppress.None)
                    {
                        s1 = 0.0f;
                        s2 = 0.0f;
                    }

                    iList.AddItem(FastItem(Title, s1, s2, Suffix, false, false, pEnh.Effects[shortFxArray1[index].Index[0]].Probability < 1.0, pEnh.Effects[shortFxArray1[index].Index[0]].ActiveConditionals.Count > 0, Power.SplitFXGroupTip(ref shortFxArray1[index], ref pEnh, false)));
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

    }
}
