using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
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
using FontStyle = System.Drawing.FontStyle;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace Mids_Reborn.Forms.Controls
{
    public partial class DataView : UserControl
    {
        private struct ItemPairGroup
        {
            public string Label;
            public Func<GroupedFx.FxId, bool> Filter;
            public List<PairedListEx.Item> ItemPairs;
        }

        private struct ItemPairGroupEx
        {
            public string Label;
            public Func<GroupedFx.FxId, bool> Filter;
            public List<KeyValuePair<GroupedFx, PairedListEx.Item>> ItemPairsEx;
        }


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
        public delegate void SlotFlipEventHandler(int powerIndex);
        public delegate void SlotUpdateEventHandler();
        public delegate void TabChangedEventHandler(int index);
        public delegate void UnlockClickEventHandler();
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
        private IPower? rootPowerBase;
        private IPower? rootPowerEnh;
        private int pLastScaleVal;
        private Rectangle ScreenBounds;
        public Rectangle SnapLocation;
        public int TabPage;
        private bool VillainColor;
        public bool[]? TabsMask;
        private List<GroupedFx> GroupedRankedEffects;
        private List<KeyValuePair<GroupedFx, PairedListEx.Item>> EffectsItemPairs;

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

        public event FloatChangeEventHandler? FloatChange;
        public event MovedEventHandler? Moved;
        public event SizeChangeEventHandler? SizeChange;
        public event SlotFlipEventHandler? SlotFlip;
        public event SlotUpdateEventHandler? SlotUpdate;
        public event TabChangedEventHandler? TabChanged;
        public event UnlockClickEventHandler? UnlockClick;
        public event EntityDetailsEventHandler EntityDetails;

        private static PairedListEx.Item BuildEDItem(int index, float[] value, Enums.eSchedule[] schedule, string Name, float[] afterED)
        {
            var flag1 = value[index] > (double)DatabaseAPI.Database.MultED[(int)schedule[index]][0];
            var flag2 = value[index] > (double)DatabaseAPI.Database.MultED[(int)schedule[index]][1];
            var iSpecialCase = value[index] > (double)DatabaseAPI.Database.MultED[(int)schedule[index]][2];
            PairedListEx.Item itemPair;
            if (value[index] <= 0.0)
            {
                itemPair = new PairedListEx.Item(string.Empty, string.Empty, false, false, false, string.Empty);
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

                itemPair = new PairedListEx.Item(iName, iValue, flag2 & !iSpecialCase, flag1 & !flag2,
                    iSpecialCase, iTip);
            }

            return itemPair;
        }

        public void Clear()
        {
            info_DataList.Clear(true);
            info_Title.Text = Pages[0];
            Info_txtLarge.Text = string.Empty;
            info_txtSmall.Text = @"Hold the mouse over a power to see its description.";
            PowerScaler.Visible = false;
            fx_lblHead1.Text = string.Empty;
            fx_lblHead2.Text = string.Empty;
            fx_LblHead3.Text = string.Empty;
            fx_List1.Clear(true);
            fx_List2.Clear(true);
            fx_List3.Clear(true);
            fx_Title.Text = Pages[1];
            enhListing.Clear(true);
            Enh_Title.Text = @"Enhancements";
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

        private void DataView_BackColorChanged(object? sender, EventArgs e)
        {
            SetBackColor();
        }

        private void DataView_Load(object? sender, EventArgs e)
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

        private void DisplayEDFigures()
        {
            if (pBase == null)
            {
                return;
            }

            Enh_Title.Text = pBase.DisplayName;
            enhListing.Clear();
            if (MidsContext.Character == null)
            {
                enhListing.Redraw();

                return;
            }

            var powerBase = rootPowerBase ?? pBase;
            var buildHistoryIdx = MidsContext.Character.CurrentBuild.FindInToonHistory(powerBase.PowerIndex);
            if (buildHistoryIdx < 0)
            {
                enhListing.Redraw();

                return;
            }

            var eEnhs = Enum.GetValues(typeof(Enums.eEnhance)).Length;
            var buffs = new float[eEnhs];
            var debuffs = new float[eEnhs];
            var buffDebuffs = new float[eEnhs];
            var buffsSchedule = new Enums.eSchedule[eEnhs];
            var debuffsSchedule = new Enums.eSchedule[eEnhs];
            var buffsDebuffsSchedule = new Enums.eSchedule[eEnhs];
            var buffsAfterED = new float[eEnhs];
            var debuffsAfterED = new float[eEnhs];
            var buffsDebuffsAfterED = new float[eEnhs];
            var mezBuffs = new float[Enum.GetValues(typeof(Enums.eMez)).Length];
            var mezSchedule = new Enums.eSchedule[eEnhs];
            var mezAfterED = new float[eEnhs];

            Array.Fill(buffs, 0);
            Array.Fill(debuffs, 0);
            Array.Fill(buffDebuffs, 0);
            Array.Fill(mezBuffs, 0);

            for (var i = 0; i < buffs.Length; i++)
            {
                buffsSchedule[i] = Enhancement.GetSchedule((Enums.eEnhance)i);
                debuffsSchedule[i] = buffsSchedule[i];
                buffsDebuffsSchedule[i] = buffsSchedule[i];
            }

            debuffsSchedule[(int)Enums.eEnhance.Defense] = Enums.eSchedule.A; // 3
            for (var tSub = 0; tSub < mezBuffs.Length; tSub++)
            {
                mezSchedule[tSub] = Enhancement.GetSchedule(Enums.eEnhance.Mez, tSub);
            }

            var buildPower = MidsContext.Character.CurrentBuild.Powers[buildHistoryIdx];
            for (var i = 0; i < buildPower?.SlotCount; i++)
            {
                var slot = buildPower.Slots[i];
                if (slot.Enhancement.Enh <= -1)
                {
                    continue;
                }

                var slotEnh = slot.Enhancement.Enh;
                for (var se = 0; se < DatabaseAPI.Database.Enhancements[slotEnh].Effect.Length; se++)
                {
                    var effect = DatabaseAPI.Database.Enhancements[slotEnh].Effect;
                    if (effect[se].Mode != Enums.eEffMode.Enhancement)
                    {
                        continue;
                    }

                    if (effect[se].Enhance.ID == 12)
                    {
                        mezBuffs[effect[se].Enhance.SubID] += slot.Enhancement.GetEnhancementEffect(Enums.eEnhance.Mez, effect[se].Enhance.SubID, 1);
                    }
                    else
                    {
                        switch (DatabaseAPI.Database.Enhancements[slotEnh].Effect[se].BuffMode)
                        {
                            case Enums.eBuffDebuff.BuffOnly:
                                buffs[effect[se].Enhance.ID] += slot.Enhancement.GetEnhancementEffect((Enums.eEnhance)effect[se].Enhance.ID, -1, 1);
                                break;
                            
                            case Enums.eBuffDebuff.DeBuffOnly:
                                if (effect[se].Enhance.ID is not 6 and not 11 and not 19)
                                {
                                    debuffs[effect[se].Enhance.ID] += slot.Enhancement.GetEnhancementEffect((Enums.eEnhance)effect[se].Enhance.ID, -1, -1);
                                }

                                break;
                            default:
                                buffDebuffs[effect[se].Enhance.ID] += slot.Enhancement.GetEnhancementEffect((Enums.eEnhance)effect[se].Enhance.ID, -1, 1);
                                break;
                        }
                    }
                }
            }

            foreach (var p in MidsContext.Character.CurrentBuild.Powers)
            {
                if (p == null)
                {
                    continue;
                }

                if (!p.StatInclude)
                {
                    continue;
                }

                IPower power1 = new Power(p.Power);
                power1.AbsorbPetEffects();
                power1.ApplyGrantPowerEffects();
                foreach (var effect in power1.Effects)
                {
                    if (power1.PowerType != Enums.ePowerType.GlobalBoost & (!effect.Absorbed_Effect | effect.Absorbed_PowerType != Enums.ePowerType.GlobalBoost))
                    {
                        continue;
                    }

                    if (effect.Absorbed_Effect & effect.Absorbed_Power_nID > -1)
                    {
                        power1 = DatabaseAPI.Database.Power[effect.Absorbed_Power_nID];
                    }

                    var eBuffDebuff = Enums.eBuffDebuff.Any;
                    var flag = false;
                    if (MidsContext.Character.CurrentBuild.Powers[buildHistoryIdx] == null)
                    {
                        continue;
                    }

                    foreach (var b in buildPower.Power.BoostsAllowed)
                    {
                        if (power1 != null && power1.BoostsAllowed.Any(e => b == e))
                        {
                            if (b.Contains("Buff"))
                            {
                                eBuffDebuff = Enums.eBuffDebuff.BuffOnly;
                            }

                            if (b.Contains("Debuff"))
                            {
                                eBuffDebuff = Enums.eBuffDebuff.DeBuffOnly;
                            }

                            flag = true;
                        }

                        if (flag)
                        {
                            break;
                        }
                    }

                    if (!flag)
                    {
                        continue;
                    }

                    switch (effect.EffectType)
                    {
                        case Enums.eEffectType.Enhancement:
                            switch (effect.ETModifies)
                            {
                                case Enums.eEffectType.Defense:
                                    if (effect.DamageType == Enums.eDamage.Smashing)
                                    {
                                        if (effect.IgnoreED)
                                        {
                                            switch (eBuffDebuff)
                                            {
                                                case Enums.eBuffDebuff.BuffOnly:
                                                    buffsAfterED[(int)Enums.eEnhance.Defense] += effect.BuffedMag; // 3
                                                    break;

                                                case Enums.eBuffDebuff.DeBuffOnly:
                                                    debuffsAfterED[(int)Enums.eEnhance.Defense] += effect.BuffedMag;
                                                    break;

                                                default:
                                                    buffsDebuffsAfterED[(int)Enums.eEnhance.Defense] += effect.BuffedMag;
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            switch (eBuffDebuff)
                                            {
                                                case Enums.eBuffDebuff.BuffOnly:
                                                    buffs[(int)Enums.eEnhance.Defense] += effect.BuffedMag; // 3
                                                    break;

                                                case Enums.eBuffDebuff.DeBuffOnly:
                                                    debuffs[(int)Enums.eEnhance.Defense] += effect.BuffedMag;
                                                    break;

                                                default:
                                                    buffDebuffs[(int)Enums.eEnhance.Defense] += effect.BuffedMag;
                                                    break;
                                            }
                                        }
                                    }

                                    break;
                                case Enums.eEffectType.Mez:
                                    if (effect.IgnoreED)
                                    {
                                        mezAfterED[(int)effect.MezType] += effect.BuffedMag;
                                        break;
                                    }

                                    mezBuffs[(int)effect.MezType] += effect.BuffedMag;
                                    break;

                                default:
                                    var rechargeBuffIndex = effect.ETModifies != Enums.eEffectType.RechargeTime
                                        ? Convert.ToInt32(Enum.Parse(typeof(Enums.eEnhance), effect.ETModifies.ToString()))
                                        : (int)Enums.eEnhance.RechargeTime; // 14
                                    if (effect.IgnoreED)
                                    {
                                        buffsDebuffsAfterED[rechargeBuffIndex] += effect.BuffedMag;
                                        break;
                                    }

                                    buffDebuffs[rechargeBuffIndex] += effect.BuffedMag;
                                    break;
                            }

                            break;
                        default:
                        {
                            if (effect.EffectType == Enums.eEffectType.DamageBuff & effect.DamageType == Enums.eDamage.Smashing)
                            {
                                switch (effect.IgnoreED)
                                {
                                    case true:
                                    {
                                        foreach (var b in power1?.BoostsAllowed)
                                        {
                                            if (b.StartsWith("Res_Damage"))
                                            {
                                                buffsDebuffsAfterED[(int)Enums.eEnhance.Resistance] += effect.BuffedMag; // 18
                                                break;
                                            }

                                            if (!b.StartsWith("Damage"))
                                            {
                                                continue;
                                            }

                                            buffsDebuffsAfterED[(int)Enums.eEnhance.Damage] += effect.BuffedMag; // 2
                                            break;
                                        }

                                        break;
                                    }
                                    default:
                                    {
                                        foreach (var b in power1?.BoostsAllowed)
                                        {
                                            if (b.StartsWith("Res_Damage"))
                                            {
                                                buffDebuffs[(int)Enums.eEnhance.Resistance] += effect.BuffedMag;
                                                break;
                                            }

                                            if (!b.StartsWith("Damage"))
                                            {
                                                continue;
                                            }

                                            buffDebuffs[(int)Enums.eEnhance.Damage] += effect.BuffedMag;
                                            break;
                                        }

                                        break;
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }

            var zeroedEnhanceBuffs = new[]
            {
                Enums.eEnhance.HitPoints, Enums.eEnhance.Regeneration, Enums.eEnhance.Recovery // 8, 16, 17
            }.Cast<int>();

            foreach (var buff in zeroedEnhanceBuffs)
            {
                buffs[buff] = 0;
                debuffs[buff] = 0;
                buffDebuffs[buff] = 0;
            }

            for (var i = 0; i < buffs.Length; i++)
            {
                if (buffs[i] > 0)
                {
                    enhListing.AddItem(BuildEDItem(i, buffs, buffsSchedule, Enum.GetName(typeof(Enums.eEnhance), i), buffsAfterED));
                    if (enhListing.IsSpecialColor())
                    {
                        enhListing.SetUnique();
                    }
                }

                if (debuffs[i] > 0)
                {
                    enhListing.AddItem(BuildEDItem(i, debuffs, debuffsSchedule, $"{Enum.GetName(typeof(Enums.eEnhance), i)} Debuff", debuffsAfterED));
                    if (enhListing.IsSpecialColor())
                    {
                        enhListing.SetUnique();
                    }
                }

                if (buffDebuffs[i] <= 0)
                {
                    continue;
                }

                enhListing.AddItem(BuildEDItem(i, buffDebuffs, buffsDebuffsSchedule, Enum.GetName(typeof(Enums.eEnhance), i), buffsDebuffsAfterED));
                if (enhListing.IsSpecialColor())
                {
                    enhListing.SetUnique();
                }
            }

            for (var i = 0; i < mezBuffs.Length; i++)
            {
                if (mezBuffs[i] <= 0)
                {
                    continue;
                }

                enhListing.AddItem(BuildEDItem(i, mezBuffs, mezSchedule, Enum.GetName(typeof(Enums.eMez), i), mezAfterED));
                if (enhListing.IsSpecialColor())
                {
                    enhListing.SetUnique();
                }
            }

            enhListing.Redraw();
            DisplayFlippedEnhancements();
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
            info_DataList.AddItem(FastItemBuilder.Fi.FastItem(ShortStr("End Cost", "End"), pBase.ToggleCost, enhancedPower.ToggleCost, suffix1, tip1));
            var absorbedEffectsFlag = pBase.HasAbsorbedEffects && pBase.PowerIndex > -1 && DatabaseAPI.Database.Power[pBase.PowerIndex].EntitiesAutoHit == Enums.eEntity.None;
            var requiresToHitCheckFlag = pBase.Effects.Any(t => t.RequiresToHitCheck);
            var entitiesAutoHitFlag = pBase.EntitiesAutoHit == Enums.eEntity.None |
                                      pBase.Effects
                                          .Where(e => e.EffectType == Enums.eEffectType.EntCreate)
                                          .SelectMany(e => DatabaseAPI.Database.Entities.ElementAtOrDefault(e.nSummon) == null
                                              ? Array.Empty<IPower?>()
                                              : DatabaseAPI.Database.Powersets.ElementAtOrDefault(DatabaseAPI.Database.Entities[e.nSummon].GetNPowerset()[0]) == null
                                                ? Array.Empty<IPower?>()
                                                : DatabaseAPI.Database.Powersets[DatabaseAPI.Database.Entities[e.nSummon].GetNPowerset()[0]]?.Powers)
                                          .Any(e => e?.EntitiesAutoHit == Enums.eEntity.None);
            
            if (entitiesAutoHitFlag | requiresToHitCheckFlag | absorbedEffectsFlag | pBase.Range > 20 & pBase.I9FXPresentP(Enums.eEffectType.Mez, Enums.eMez.Taunt))
            {
                var accuracy1 = pBase.Accuracy;
                var accuracy2 = enhancedPower.Accuracy;
                var num2 = MidsContext.Config.ScalingToHit * pBase.Accuracy;
                var str = string.Empty;
                var suffix2 = "%";
                if (pBase.EntitiesAutoHit != Enums.eEntity.None & requiresToHitCheckFlag)
                {
                    str = "\r\n* This power is autohit, but has an effect that requires a ToHit roll.";
                    suffix2 += "*";
                }

                if (Math.Abs(accuracy1 - accuracy2) > float.Epsilon & Math.Abs(num2 - accuracy2) > float.Epsilon)
                {
                    var tip2 = $"Accuracy multiplier without other buffs (Real Numbers style): {pBase.Accuracy + (enhancedPower.Accuracy - MidsContext.Config.ScalingToHit):##0.00000}x{str}";
                    info_DataList.AddItem(FastItemBuilder.Fi.FastItem(ShortStr("Accuracy", "Acc"),
                        MidsContext.Config.ScalingToHit * pBase.Accuracy * 100, enhancedPower.Accuracy * 100, suffix2, tip2));
                }
                else
                {
                    var tip2 = $"Accuracy multiplier without other buffs (Real Numbers style): {pBase.AccuracyMult:##0.00}x{str}";
                    info_DataList.AddItem(FastItemBuilder.Fi.FastItem(ShortStr("Accuracy", "Acc"),
                        MidsContext.Config.ScalingToHit * pBase.Accuracy * 100,
                        MidsContext.Config.ScalingToHit * pBase.Accuracy * 100, suffix2, tip2));
                }
            }
            else
            {
                info_DataList.AddItem(new PairedListEx.Item(string.Empty, string.Empty, false, false, false, string.Empty));
            }

            info_DataList.AddItem(FastItemBuilder.Fi.FastItem(ShortStr("Recharge", "Rchg"), pBase.RechargeTime, enhancedPower.RechargeTime, "s"));
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

            info_DataList.AddItem(FastItemBuilder.Fi.FastItem(ShortStr("Duration", "Durtn"), s1, s2, "s"));
            info_DataList.AddItem(FastItemBuilder.Fi.FastItem(ShortStr("Range", "Range"), pBase.Range, enhancedPower.Range, "ft"));
            info_DataList.AddItem(pBase.Arc > 0
                ? FastItemBuilder.Fi.FastItem("Arc", pBase.Arc, enhancedPower.Arc, "°")
                : FastItemBuilder.Fi.FastItem("Radius", pBase.Radius, enhancedPower.Radius, "ft"));
            info_DataList.AddItem(FastItemBuilder.Fi.FastItem(ShortStr("Cast Time", "Cast"), enhancedPower.CastTime, pBase.CastTime, "s", $"CastTime: {pBase.CastTime}s\r\nArcana CastTime: {(Math.Ceiling(enhancedPower.CastTime / 0.132f) + 1) * 0.132:####0.###}s", false, true, false, false, 3));
            info_DataList.AddItem(pBase.PowerType == Enums.ePowerType.Toggle
                ? FastItemBuilder.Fi.FastItem(ShortStr("Activate", "Act"), pBase.ActivatePeriod, enhancedPower.ActivatePeriod, "s", "The effects of this toggle power are applied at this interval.")
                : FastItemBuilder.Fi.FastItem(ShortStr("Interrupt", "Intrpt"), enhancedPower.InterruptTime, pBase.InterruptTime, "s", "After activating this power, it can be interrupted for this amount of time."));
            if (durationEffectId > -1 &&
                pBase.Effects[durationEffectId].EffectType == Enums.eEffectType.Mez &
                pBase.Effects[durationEffectId].MezType != Enums.eMez.Taunt &
                !((pBase.Effects[durationEffectId].MezType == Enums.eMez.Knockback |
                   pBase.Effects[durationEffectId].MezType == Enums.eMez.Knockup) &
                  pBase.Effects[durationEffectId].Mag < 0))
            {
                info_DataList.AddItem(new PairedListEx.Item("Effect:",
                    Enum.GetName(Enums.eMez.None.GetType(), pBase.Effects[durationEffectId].MezType), false,
                    pBase.Effects[durationEffectId].Probability < 1,
                    pBase.Effects[durationEffectId].CanInclude(),
                    durationEffectId));

                info_DataList.AddItem(new PairedListEx.Item("Mag:",
                    $"{enhancedPower.Effects[durationEffectId].BuffedMag:####0.##}",
                    Math.Abs(pBase.Effects[durationEffectId].BuffedMag - enhancedPower.Effects[durationEffectId].BuffedMag) > float.Epsilon,
                    pBase.Effects[durationEffectId].Probability < 1));
            }

            var rankedEffectsExt = GroupedFx.FilterListItemsExt(EffectsItemPairs,
                e => e.EffectType is not Enums.eEffectType.GrantPower or Enums.eEffectType.MaxRunSpeed
                         or Enums.eEffectType.MaxFlySpeed or Enums.eEffectType.MaxJumpSpeed or Enums.eEffectType.Mez ||
                     e is {EffectType: Enums.eEffectType.Mez, ToWho: Enums.eToWho.Self} or
                         {EffectType: Enums.eEffectType.Mez, MezType: Enums.eMez.Taunt or Enums.eMez.Teleport});
            foreach (var rex in rankedEffectsExt)
            {
                info_DataList.AddItem(rex.Value);
                if (rex.Key.EnhancementEffect)
                {
                    info_DataList.SetUnique();
                }
            }

            info_DataList.Redraw();
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
            DisplayEDFigures();
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

            PairedListEx[] pairedListArray =
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

            var itemPairGroups = new List<ItemPairGroupEx>
            {
                new()
                {
                    Label = "Defense/Resistance",
                    Filter = e => e.EffectType is Enums.eEffectType.Defense or Enums.eEffectType.Resistance,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                },

                new()
                {
                    Label = "Heal/Endurance",
                    Filter = e => e.EffectType is Enums.eEffectType.Heal or Enums.eEffectType.HitPoints
                        or Enums.eEffectType.Regeneration or Enums.eEffectType.Endurance or Enums.eEffectType.Recovery,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                },

                new()
                {
                    Label = "Status",
                    Filter = e => e.EffectType is Enums.eEffectType.Mez or Enums.eEffectType.MezResist
                        or Enums.eEffectType.Translucency,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                },

                new()
                {
                    Label = "Buff/Debuff",
                    Filter = e => e.EffectType is Enums.eEffectType.ToHit or Enums.eEffectType.DamageBuff
                        or Enums.eEffectType.PerceptionRadius or Enums.eEffectType.StealthRadius
                        or Enums.eEffectType.StealthRadiusPlayer or Enums.eEffectType.ResEffect
                        or Enums.eEffectType.ThreatLevel or Enums.eEffectType.DropToggles
                        or Enums.eEffectType.RechargeTime or Enums.eEffectType.Enhancement,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                },

                new()
                {
                    Label = "Movement",
                    Filter = e => e.EffectType is Enums.eEffectType.SpeedRunning or Enums.eEffectType.SpeedJumping
                        or Enums.eEffectType.SpeedFlying or Enums.eEffectType.JumpHeight or Enums.eEffectType.Jumppack
                        or Enums.eEffectType.Fly or Enums.eEffectType.MaxRunSpeed or Enums.eEffectType.MaxJumpSpeed
                        or Enums.eEffectType.MaxFlySpeed,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                },

                new()
                {
                    Label = "Summon",
                    Filter = e => e.EffectType is Enums.eEffectType.EntCreate,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                },

                new()
                {
                    Label = "Granted Powers",
                    Filter = e => e.EffectType == Enums.eEffectType.GrantPower,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                },

                new()
                {
                    Label = "Modify Effect",
                    Filter = e => e.EffectType == Enums.eEffectType.ModifyAttrib,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                },

                new()
                {
                    Label = "Elusivity",
                    Filter = e =>
                        (MidsContext.Config != null && MidsContext.Config.Inc.DisablePvE) &
                        e.EffectType == Enums.eEffectType.Elusivity,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                }
            };

            for (var i = 0; i < itemPairGroups.Count; i++)
            {
                itemPairGroups[i] = new ItemPairGroupEx
                {
                    Label = itemPairGroups[i].Label,
                    Filter = itemPairGroups[i].Filter,
                    ItemPairsEx = GroupedFx.FilterListItemsExt(EffectsItemPairs, itemPairGroups[i].Filter)
                };
            }

            var activeItemPairGroups = itemPairGroups
                .Where(e => e.ItemPairsEx.Count > 0)
                .ToList();

            // Fill the 3 blocks once
            // If more categories left, combine titles then display along with the other first 3 groups
            for (var i = 0; i < activeItemPairGroups.Count; i++)
            {
                labelArray[i % 3].Text = labelArray[i % 3].Text.EndsWith(":")
                    ? labelArray[i % 3].Text.Replace(":", $" | {activeItemPairGroups[i].Label}:")
                    : $"{activeItemPairGroups[i].Label}:";
                
                foreach (var ip in activeItemPairGroups[i].ItemPairsEx)
                {
                    pairedListArray[i % 3].AddItem(ip.Value);
                    if (ip.Key.EnhancementEffect)
                    {
                        pairedListArray[i % 3].SetUnique();
                    }
                }
            }

            fx_List1.Redraw();
            fx_List2.Redraw();
            fx_List3.Redraw();
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
            {
                return;
            }

            var powerBase = rootPowerBase ?? pBase;

            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(powerBase.PowerIndex);
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
                using var solidBrush1 = new SolidBrush(enhListing.ItemColor);
                format.Alignment = StringAlignment.Far;
                format.LineAlignment = StringAlignment.Center;
                bxFlip.Graphics.DrawString("Active Slotting:", pnlEnhActive.Font, solidBrush1, rectangle1, format);
                rectangle1.Y += rectangle1.Height;
                bxFlip.Graphics.DrawString("Alternate:", pnlEnhActive.Font, solidBrush1, rectangle1, format);
                //ImageAttributes recolorIa = clsDrawX.GetRecolorIa(MidsContext.Character.IsHero());
                using var solidBrush2 = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                var power = MidsContext.Character.CurrentBuild.Powers[inToonHistory];
                for (var index = 0; index < power.SlotCount; index++)
                {
                    var iDest = new Rectangle();
                    ref var local2 = ref iDest;
                    var x1 = num1 + 30 * index;
                    size = bxFlip.Size;
                    var y1 = (int)Math.Round((size.Height / 2.0 - 30) / 2.0);
                    local2 = new Rectangle(x1, y1, 30, 30);
                    var rectangle2 = new Rectangle();
                    ref var local3 = ref rectangle2;
                    var x2 = num1 + 30 * index;
                    size = bxFlip.Size;
                    var num3 = size.Height / 2.0;
                    size = bxFlip.Size;
                    var num4 = (size.Height / 2.0 - 30) / 2.0;
                    var y2 = (int)Math.Round(num3 + num4);
                    local3 = new Rectangle(x2, y2, 30, 30);
                    RectangleF bounds;
                    Rectangle destRect;
                    if (power.Slots[index].Enhancement.Enh > -1)
                    {
                        var graphics1 = bxFlip.Graphics;
                        I9Gfx.DrawEnhancementAt(ref graphics1, iDest,
                            DatabaseAPI.Database.Enhancements[power.Slots[index].Enhancement.Enh].ImageIdx,
                            I9Gfx.ToGfxGrade(
                                DatabaseAPI.Database.Enhancements[power.Slots[index].Enhancement.Enh].TypeID,
                                power.Slots[index].Enhancement.Grade));
                        if (power.Slots[index].Enhancement.Enh > -1)
                        {
                            if (!MidsContext.Config.I9.HideIOLevels & DatabaseAPI.Database.Enhancements[power.Slots[index].Enhancement.Enh].TypeID is Enums.eType.SetO or Enums.eType.InventO)
                            {
                                bounds = iDest;
                                bounds.Y -= 3f;
                                bounds.Height = DefaultFont.GetHeight(bxFlip.Graphics);
                                var graphics2 = bxFlip.Graphics;
                                clsDrawX.DrawOutlineText($"{power.Slots[index].Enhancement.IOLevel + 1}", bounds,
                                    Color.Cyan, Color.FromArgb(128, 0, 0, 0), pnlEnhActive.Font, 1f, graphics2);
                            }
                            else if (MidsContext.Config.ShowEnhRel & DatabaseAPI.Database.Enhancements[power.Slots[index].Enhancement.Enh].TypeID is Enums.eType.Normal or Enums.eType.SpecialO)
                            {
                                bounds = iDest;
                                bounds.Y -= 3f;
                                bounds.Height = DefaultFont.GetHeight(bxFlip.Graphics);
                                var text = power.Slots[index].Enhancement.RelativeLevel != Enums.eEnhRelative.None
                                        ? power.Slots[index].Enhancement.RelativeLevel >= Enums.eEnhRelative.Even
                                            ? power.Slots[index].Enhancement.RelativeLevel <= Enums.eEnhRelative.Even
                                                ? Color.White
                                                : Color.FromArgb(0, byte.MaxValue, byte.MaxValue)
                                            : Color.Yellow
                                        : Color.Red;
                                var graphics2 = bxFlip.Graphics;
                                clsDrawX.DrawOutlineText(
                                    Enums.GetRelativeString(power.Slots[index].Enhancement.RelativeLevel,
                                        MidsContext.Config.ShowRelSymbols), bounds, text, Color.FromArgb(128, 0, 0, 0),
                                    pnlEnhActive.Font, 1f, graphics2);
                            }
                        }
                    }
                    else
                    {
                        destRect = iDest with {Width = 30, Height = 30};
                        bxFlip.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, destRect, 0, 0, 30, 30, GraphicsUnit.Pixel);
                    }

                    if (power.Slots[index].FlippedEnhancement.Enh > -1)
                    {
                        var graphics1 = bxFlip.Graphics;
                        I9Gfx.DrawEnhancementAt(ref graphics1, rectangle2, DatabaseAPI.Database.Enhancements[power.Slots[index].FlippedEnhancement.Enh].ImageIdx,
                            I9Gfx.ToGfxGrade(
                                DatabaseAPI.Database.Enhancements[power.Slots[index].FlippedEnhancement.Enh].TypeID,
                                power.Slots[index].FlippedEnhancement.Grade));
                        
                        if (power.Slots[index].FlippedEnhancement.Enh > -1)
                        {
                            if (!MidsContext.Config.I9.HideIOLevels & DatabaseAPI.Database.Enhancements[power.Slots[index].FlippedEnhancement.Enh].TypeID is Enums.eType.SetO or Enums.eType.InventO)
                            {
                                bounds = rectangle2;
                                bounds.Y -= 3f;
                                bounds.Height = DefaultFont.GetHeight(bxFlip.Graphics);
                                var graphics2 = bxFlip.Graphics;
                                clsDrawX.DrawOutlineText(
                                    $"{power.Slots[index].FlippedEnhancement.IOLevel + 1}", bounds, Color.Cyan,
                                    Color.FromArgb(128, 0, 0, 0), pnlEnhActive.Font, 1f, graphics2);
                            }
                            else if (MidsContext.Config.ShowEnhRel & DatabaseAPI.Database.Enhancements[power.Slots[index].FlippedEnhancement.Enh].TypeID is Enums.eType.Normal or Enums.eType.SpecialO)
                            {
                                bounds = rectangle2;
                                bounds.Y -= 3f;
                                bounds.Height = DefaultFont.GetHeight(bxFlip.Graphics);
                                var text = power.Slots[index].FlippedEnhancement.RelativeLevel != Enums.eEnhRelative.None
                                        ? power.Slots[index].FlippedEnhancement.RelativeLevel >= Enums.eEnhRelative.Even
                                            ? power.Slots[index].FlippedEnhancement.RelativeLevel <= Enums.eEnhRelative.Even
                                                ? Color.White
                                                : Color.FromArgb(0, byte.MaxValue, byte.MaxValue)
                                            : Color.Yellow
                                        : Color.Red;
                                var graphics2 = bxFlip.Graphics;
                                clsDrawX.DrawOutlineText(
                                    Enums.GetRelativeString(power.Slots[index].FlippedEnhancement.RelativeLevel,
                                        MidsContext.Config.ShowRelSymbols), bounds, text, Color.FromArgb(128, 0, 0, 0),
                                    pnlEnhActive.Font, 1f, graphics2);
                            }
                        }
                    }
                    else
                    {
                        destRect = rectangle2 with {Width = 30, Height = 30};
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
            total_Misc.Clear(true);
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
                targetGraph.AddItem($"{dmgNames[dType]}:|{displayStats.Defense(dType):0.#}%", Math.Max(0, displayStats.Defense(dType)), 0, iTip);
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
                targetGraph.AddItem($"{dmgNames[dType]}:|{displayStats.DamageResistance(dType, false):0.#}%", Math.Max(0, displayStats.DamageResistance(dType, false)), Math.Max(0, displayStats.DamageResistance(dType, true)), iTip);
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
            total_Misc.AddItem(new PairedListEx.Item("Recovery:", $"{displayStats.EnduranceRecoveryPercentage(false):0.#}% ({displayStats.EnduranceRecoveryNumeric:0.#}/s)", false, false, false, iTip2));
            total_Misc.AddItem(new PairedListEx.Item("Regen:", $"{displayStats.HealthRegenPercent(false):0.#}%", false, false, false, iTip3));
            total_Misc.AddItem(new PairedListEx.Item("EndDrain:", $"{displayStats.EnduranceUsage:0.#}/s", false, false, false, iTip1));
            total_Misc.AddItem(new PairedListEx.Item("+ToHit:", $"{displayStats.BuffToHit:0.#}%", false, false, false, "This effect is increasing the accuracy of all your powers."));
            total_Misc.AddItem(new PairedListEx.Item("+EndRdx:", $"{displayStats.BuffEndRdx:0.#}%", false, false, false, "The end cost of all your powers is being reduced by this effect.\r\nThis is applied like an end-reduction enhancement."));
            total_Misc.AddItem(new PairedListEx.Item("+Recharge:", $"{displayStats.BuffHaste(false) - 100.0:0.#}%", false, false, false, "The recharge time of your powers is being altered by this effect.\r\nThe higher the value, the faster the recharge."));
            total_Misc.Rows = 3;
            total_Misc.Redraw();
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
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            var pen = new Pen(Color.Black);
            var font1 = new Font("Segoe UI", 9.25f, FontStyle.Regular);
            var font2 = new Font("Segoe UI", 9.25f, FontStyle.Bold);
            var format = new StringFormat(StringFormatFlags.NoWrap);
            var solidBrush1 = new SolidBrush(Color.White);
            var solidBrush2 = new SolidBrush(BackColor);
            var solidBrush3 = new SolidBrush(Color.Black);
            var extendedBitmap = new ExtendedBitmap(pnlTabs.Size);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            var rect = new Rectangle(0, 0, 75, pnlTabs.Height);
            extendedBitmap.Graphics?.FillRectangle(solidBrush2, extendedBitmap.ClipRect);
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

            for (var index = 0; index < Pages.Length; index++)
            {
                rect = new Rectangle(rect.Width * index, 2, 70, pnlTabs.Height - 2);
                if (TabsMask != null && !TabsMask[index])
                {
                    continue;
                }

                extendedBitmap.Graphics?.DrawRectangle(pen, rect);
                TextRenderer.DrawText(extendedBitmap.Graphics!, Pages[index], font1, rect, Color.WhiteSmoke, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }

            rect = new Rectangle(70 * TabPage, 0, 70, pnlTabs.Height);
            extendedBitmap.Graphics?.FillRectangle(solidBrush3, rect);
            extendedBitmap.Graphics?.DrawRectangle(pen, rect);
            TextRenderer.DrawText(extendedBitmap.Graphics!, Pages[TabPage], font2, rect, Color.WhiteSmoke, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            graphics.DrawImageUnscaled(extendedBitmap.Bitmap!, 0, 0);
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

        private PairedListEx.Item GetRankedEffect(int[] Index, int ID)
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
                return FastItemBuilder.Fi.FastItem("", 0f, 0f, string.Empty);
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
                        _ => FastItemBuilder.Str.CapString(Enum.GetName(fx.ETModifies.GetType(), fx.ETModifies), 7)
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
                    return FastItemBuilder.Fi.FastItem("", 0f, 0f, string.Empty);
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
                return FastItemBuilder.Fi.FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, fx.Probability < 1, fx.ActiveConditionals.Count > 0, tip);
            }

            if (fx.SpecialCase != Enums.eSpecialCase.None)
            {
                return FastItemBuilder.Fi.FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, fx.Probability < 1, fx.SpecialCase != Enums.eSpecialCase.None, tip);
            }

            return FastItemBuilder.Fi.FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, fx.Probability < 1, false, tip);
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
            var unlockClick = UnlockClick;
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

        private void PairedList_Hover(object? sender, int index, Enums.ShortFX tag, string tooltip)
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
            var powerBase = rootPowerBase ?? pBase;
            
            if (powerBase == null || e.Button != MouseButtons.Left)
            {
                return;
            }

            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(powerBase.PowerIndex);
            if (inToonHistory <= -1)
            {
                return;
            }

            var slotFlip = SlotFlip;
            slotFlip?.Invoke(inToonHistory);
        }

        private void pnlEnhActive_MouseMove(object sender, MouseEventArgs e)
        {
            var powerBase = rootPowerBase ?? pBase;
            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(powerBase.PowerIndex);
            var enhIndex = miniGetEnhIndex(e.X, e.Y);
            if (enhIndex <= -1)
            {
                return;
            }

            SetEnhancement(MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[enhIndex].Enhancement,
                MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[enhIndex].Level);
        }

        private void pnlEnhActive_Paint(object sender, PaintEventArgs e)
        {
            RedrawFlip();
        }

        private void pnlEnhInactive_MouseClick(object sender, MouseEventArgs e)
        {
            var powerBase = rootPowerBase ?? pBase;

            if (powerBase == null || e.Button != MouseButtons.Left)
            {
                return;
            }

            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(powerBase.PowerIndex);
            if (inToonHistory <= -1)
            {
                return;
            }

            var slotFlip = SlotFlip;
            slotFlip?.Invoke(inToonHistory);
        }

        private void pnlEnhInactive_MouseMove(object sender, MouseEventArgs e)
        {
            var powerBase = rootPowerBase ?? pBase;

            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(powerBase.PowerIndex);
            var enhIndex = miniGetEnhIndex(e.X, e.Y);
            if (enhIndex <= -1)
            {
                return;
            }

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
            {
                num = pBase.VariableMin;
            }

            if (num > pBase.VariableMax)
            {
                num = pBase.VariableMax;
            }

            MidsContext.Character.CurrentBuild.Powers[HistoryIDX].VariableValue = num;
            MidsContext.Character.CurrentBuild.Powers[HistoryIDX].Power.Stacks = num;
            /*foreach (var effect in MidsContext.Character.CurrentBuild.Powers[HistoryIDX].Power.Effects)
            {
                effect.UpdateAttrib();
                DisplayInfo();
            }*/
            if (num == pLastScaleVal)
            {
                return;
            }

            SetPowerScaler();
            pLastScaleVal = num;
            MainModule.MidsController.Toon.GenerateBuffedPowerArray();
            var slotUpdate = SlotUpdate;
            slotUpdate?.Invoke();
        }

        private void RedrawFlip()
        {
            if (bxFlip == null)
            {
                DisplayFlippedEnhancements();
            }

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
            info_DataList.Redraw();
            Info_Damage.Draw(); //Drawing controls
            fx_List1.Redraw();
            fx_List2.Redraw();
            fx_List3.Redraw();
            total_Misc.Redraw();
            enhListing.Redraw();
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

        public void SetData(IPower? basePower, IPower? enhancedPower, bool noLevel = false, bool locked = false, int iHistoryIdx = -1)
        {
            if (basePower == null)
            {
                return;
            }

            Lock = locked;

            var basePowerData = new Power(basePower);
            var enhancedPowerData = new Power(enhancedPower);

            var rootPowerName = iHistoryIdx >= 0 && iHistoryIdx < MidsContext.Character.CurrentBuild.Powers.Count
                ? MidsContext.Character.CurrentBuild.Powers[iHistoryIdx]?.Power?.FullName
                : MidsContext.Character.CurrentBuild.Powers
                    .Where(e => e.Power != null)
                    .Select(e => new KeyValuePair<string, IEffect[]>(e.Power.FullName, e.Power.Effects))
                    .DefaultIfEmpty(new KeyValuePair<string, IEffect[]>("", Array.Empty<IEffect>()))
                    .FirstOrDefault(e => e.Value.Any(fx =>
                        fx.EffectType == Enums.eEffectType.PowerRedirect &&
                        ((basePower != null && fx.Override == basePower.FullName) |
                         (enhancedPower != null && fx.Override == enhancedPower.FullName))))
                    .Key;

            rootPowerBase = string.IsNullOrEmpty(rootPowerName)
                ? null
                : DatabaseAPI.GetPowerByFullName(rootPowerName);

            rootPowerEnh = string.IsNullOrEmpty(rootPowerName)
                ? null
                : MainModule.MidsController.Toon?.GetEnhancedPower(iHistoryIdx);

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
            pEnh?.ProcessExecutes();

            GroupedRankedEffects = GroupedFx.AssembleGroupedEffects(pEnh);
            EffectsItemPairs = GroupedFx.GenerateListItems(GroupedRankedEffects, pBase, pEnh, pEnh.GetRankedEffects(true).ToList(), info_DataList.Font.Size);

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
            info_DataList.SetItemsBold = MidsContext.Config.RtFont.PairedBold;
            fx_List1.SetItemsBold = MidsContext.Config.RtFont.PairedBold;
            fx_List2.SetItemsBold = MidsContext.Config.RtFont.PairedBold;
            fx_List3.SetItemsBold = MidsContext.Config.RtFont.PairedBold;
            total_Misc.SetItemsBold = MidsContext.Config.RtFont.PairedBold;
            enhListing.SetItemsBold = MidsContext.Config.RtFont.PairedBold;
            gDef1.Font = new Font(gDef1.Font.FontFamily, 8.25f, style);
            //gDef1.Font = new Font("Segoe UI", 10f, FontStyle.Bold, GraphicsUnit.Pixel);
            gDef2.Font = gDef1.Font;
            gRes1.Font = gDef1.Font;
            gRes2.Font = gDef1.Font;
            SetBackColor();
            info_DataList.ItemColor = MidsContext.Config.RtFont.ColorPlName;
            fx_List1.ItemColor = MidsContext.Config.RtFont.ColorPlName;
            fx_List2.ItemColor = MidsContext.Config.RtFont.ColorPlName;
            fx_List3.ItemColor = MidsContext.Config.RtFont.ColorPlName;
            total_Misc.ItemColor = MidsContext.Config.RtFont.ColorPlName;
            enhListing.ItemColor = MidsContext.Config.RtFont.ColorPlName;
            info_DataList.ValueColor = MidsContext.Config.RtFont.ColorText;
            fx_List1.ValueColor = MidsContext.Config.RtFont.ColorText;
            fx_List2.ValueColor = MidsContext.Config.RtFont.ColorText;
            fx_List3.ValueColor = MidsContext.Config.RtFont.ColorText;
            enhListing.ValueColor = MidsContext.Config.RtFont.ColorText;
            total_Misc.ValueColor = MidsContext.Config.RtFont.ColorText;
            info_DataList.ValueAlternateColor = MidsContext.Config.RtFont.ColorEnhancement;
            fx_List1.ValueAlternateColor = MidsContext.Config.RtFont.ColorEnhancement;
            fx_List2.ValueAlternateColor = MidsContext.Config.RtFont.ColorEnhancement;
            fx_List3.ValueAlternateColor = MidsContext.Config.RtFont.ColorEnhancement;
            enhListing.ValueAlternateColor = Color.Yellow;
            total_Misc.ValueAlternateColor = MidsContext.Config.RtFont.ColorEnhancement;
            info_DataList.ValueConditionColor = MidsContext.Config.RtFont.ColorInvention;
            fx_List1.ValueConditionColor = MidsContext.Config.RtFont.ColorInvention;
            fx_List2.ValueConditionColor = MidsContext.Config.RtFont.ColorInvention;
            fx_List3.ValueConditionColor = MidsContext.Config.RtFont.ColorInvention;
            enhListing.ValueConditionColor = Color.Red;
            total_Misc.ValueConditionColor = MidsContext.Config.RtFont.ColorInvention;
            info_DataList.ValueSpecialColor = MidsContext.Config.RtFont.ColorPlSpecial;
            fx_List1.ValueSpecialColor = MidsContext.Config.RtFont.ColorPlSpecial;
            fx_List2.ValueSpecialColor = MidsContext.Config.RtFont.ColorPlSpecial;
            fx_List3.ValueSpecialColor = MidsContext.Config.RtFont.ColorPlSpecial;
            enhListing.ValueSpecialColor = Color.FromArgb(0, byte.MaxValue, 0);
            total_Misc.ValueSpecialColor = MidsContext.Config.RtFont.ColorPlSpecial;
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

        private bool SplitFX_AddToList(ref Enums.ShortFX BaseSFX, ref Enums.ShortFX EnhSFX, ref PairedListEx iList, string SpecialTitle = "")
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

                    iList.AddItem(FastItemBuilder.Fi.FastItem(title, s1, s2, Suffix, false, false, pEnh.Effects[shortFxArray1[index].Index[0]].Probability < 1.0, pEnh.Effects[shortFxArray1[index].Index[0]].ActiveConditionals.Count > 0, Power.SplitFXGroupTip(ref shortFxArray1[index], ref pEnh, false)));
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

        private void Fx_ListItemClick(object? sender, PairedListEx.Item? item, MouseEventArgs e)
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
