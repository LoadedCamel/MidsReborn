using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn.Forms.Controls
{
    public partial class DataView2 : UserControl
    {
        private IPower _basePower;
        private IPower _enhancedPower;
        private int HistoryIdx = -1;
        private bool NoLevel;
        private PowerEntry BuildPowerEntry;
        private bool FreezeScalerCB;

        private enum BoostType
        {
            Reduction,
            Equal,
            Enhancement,
            Extra
        }

        private struct ColorRange
        {
            public Color LowerBoundColor;
            public Color UpperBoundColor;
        }

        private struct TrackGradientsScheme
        {
            public ColorRange ElapsedInnerColor;
            public ColorRange ElapsedPenColorBottom;
            public ColorRange ElapsedPenColorTop;
        }

        private readonly TrackGradientsScheme TrackColors = new()
        {
            ElapsedInnerColor = new ColorRange { LowerBoundColor = Color.FromArgb(0, 51, 0), UpperBoundColor = Color.FromArgb(0, 128, 0) },
            ElapsedPenColorBottom = new ColorRange { LowerBoundColor = Color.FromArgb(58, 94, 58), UpperBoundColor = Color.FromArgb(144, 238, 44) },
            ElapsedPenColorTop = new ColorRange { LowerBoundColor = Color.FromArgb(0, 102, 51), UpperBoundColor = Color.FromArgb(0, 255, 127) }
        };

        public bool Locked;

        public DataView2()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();
        }

        // Move the extra effects from the longest array (pBase) to the shortest (pEnh),
        // Resulting in pEnh always being the longest one.
        private static List<IEffect[]> SwapExtraEffects(IEffect[] baseEffects, IEffect[] enhEffects)
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

        public void SetData(IPower basePower = null, IPower enhancedPower = null, bool noLevel = false, bool locked = false, int historyIdx = -1)
        {
            _basePower = basePower;
            _enhancedPower = enhancedPower;
            Locked = locked;
            NoLevel = noLevel;
            HistoryIdx = historyIdx;
            BuildPowerEntry = HistoryIdx > -1
                ? MidsContext.Character.CurrentBuild.Powers[HistoryIdx]
                : null;

            // Data may differ from DB.
            if (_basePower == null) return;

            var dbPower = DatabaseAPI.GetPowerByFullName(_basePower.FullName);
            if (dbPower == null) return;

            if (_basePower != null)
            {
                _basePower.ActivatePeriod = dbPower.ActivatePeriod;
            }

            if (_enhancedPower == null) return;
            _enhancedPower.ActivatePeriod = dbPower.ActivatePeriod;

            if (_basePower.Effects.Length <= _enhancedPower.Effects.Length) return;

            var swappedFx = SwapExtraEffects(_basePower.Effects, _enhancedPower.Effects);
            _basePower.Effects = (IEffect[])swappedFx[0].Clone();
            _enhancedPower.Effects = (IEffect[])swappedFx[1].Clone();
        }

        private void InitScaler()
        {
            if (_basePower is { VariableEnabled: true } && HistoryIdx > -1)
            {
                FreezeScalerCB = true;
                labelPowerScaler.Text = string.IsNullOrWhiteSpace(_basePower.VariableName)
                    ? "Targets"
                    : _basePower.VariableName;
                powerScaler.Minimum = _basePower.VariableMin;
                powerScaler.Maximum = _basePower.VariableMax;
                powerScaler.Value = MidsContext.Character.CurrentBuild.Powers[HistoryIdx].VariableValue;
                // TODO: show range tooltip when mouseover.
                // TODO: show current value when moving, mousedown.
                FreezeScalerCB = false;
                panelPowerScaler.Visible = true;

            }
            else
            {
                panelPowerScaler.Visible = false;
            }
        }

        #region Text to RTF methods

        private string Text2RTF(string s)
        {
            return $"{RTF.StartRTF()}{s.Replace("\r\n", "\n").Replace("\n", RTF.Crlf())}{RTF.EndRTF()}";
        }

        private string List2RTF(List<string> ls)
        {
            var ret = RTF.StartRTF();
            for (var i = 0 ; i < ls.Count ; i++)
            {
                if (i == 0)
                {
                    ret += RTF.Crlf();
                }

                ret += ls[i];
            }

            ret += RTF.EndRTF();

            return ret;
        }
        #endregion

        private BoostType GetBoostType(float valueBase, float valueEnhanced)
        {
            var diff = valueEnhanced - valueBase;
            
            return diff switch
            {
                < 0 => BoostType.Reduction,
                > 0 => BoostType.Enhancement,
                _ => BoostType.Equal
            };
        }

        private static ListViewItem CreateStatLVItem(string statName, string value, BoostType boostType, string tip = "")
        {
            var valueColor = boostType switch
            {
                BoostType.Reduction => Color.FromArgb(255, 20, 20),
                BoostType.Enhancement => Color.FromArgb(0, 240, 80),
                BoostType.Extra => Color.FromArgb(0, 220, 220),
                _ => Color.WhiteSmoke,
            };

            var lvItem = new ListViewItem
            {
                ForeColor = Color.FromArgb(160, 160, 160),
                Text = statName,
                ToolTipText = tip
            };

            lvItem.SubItems.Add(
                new ListViewItem.ListViewSubItem
                {
                    ForeColor = valueColor,
                    Text = value
                }
            );

            return lvItem;
        }

        private static ListViewItem CreateStatLVItem()
        {
            var lvItem = new ListViewItem
            {
                Text = ""
            };

            lvItem.SubItems.Add(
                new ListViewItem.ListViewSubItem
                {
                    Text = ""
                }
            );

            return lvItem;
        }

        #region Info Tab

        private void DisplayInfo()
        {
            infoTabTitle.Text = $"{(BuildPowerEntry != null ? $"[{BuildPowerEntry.Level}] " : "")}{_basePower?.DisplayName ?? "Info"}";
            richInfoSmall.Rtf = Text2RTF(_basePower?.DescShort ?? "");
            richInfoLarge.Rtf = Text2RTF(_basePower?.DescLong ?? "");

            if (_basePower == null) return;

            // Add basic power info
            listInfosL.BeginUpdate();
            listInfosR.BeginUpdate();
            listInfosL.Items.Add(CreateStatLVItem("End Cost", $"{_enhancedPower.EndCost:##.##}", GetBoostType(_basePower.EndCost, _enhancedPower?.EndCost ?? _basePower.EndCost)));
            listInfosL.Items.Add(CreateStatLVItem("Recharge", $"{_enhancedPower.RechargeTime:#####.##}s", GetBoostType(_basePower.RechargeTime, _enhancedPower?.RechargeTime ?? _basePower.RechargeTime)));
            listInfosL.Items.Add(CreateStatLVItem("Range", $"{_enhancedPower.Range:####.##}ft", GetBoostType(_basePower.Range, _enhancedPower?.Range ?? _basePower.Range)));
            listInfosL.Items.Add(CreateStatLVItem("Case Time", $"{_enhancedPower.CastTime:##.##}s", GetBoostType(_basePower.CastTime, _enhancedPower?.CastTime ?? _basePower.CastTime)));

            listInfosR.Items.Add(CreateStatLVItem("Accuracy", $"{_enhancedPower.Accuracy:P2}", GetBoostType(_basePower.Accuracy, _enhancedPower?.Accuracy ?? _basePower.Accuracy)));

            // Check if there is a mez effect, display duration in the right column.
            var hasMez = _basePower.Effects.Any(e => e.EffectType == Enums.eEffectType.Mez);
            if (hasMez)
            {
                var baseDuration = _basePower.Effects
                    .Where(e => e.EffectType == Enums.eEffectType.Mez)
                    .Select(e => e.Duration)
                    .Max();

                var enhancedDuration = _enhancedPower.Effects
                    .Where(e => e.EffectType == Enums.eEffectType.Mez)
                    .Select(e => e.Duration)
                    .Max();

                listInfosR.Items.Add(CreateStatLVItem("Duration", $"{enhancedDuration:###.##}s", GetBoostType(baseDuration, enhancedDuration)));

                listInfosR.Items.Add(CreateStatLVItem());
                listInfosR.Items.Add(CreateStatLVItem());
            }
            else
            {
                listInfosR.Items.Add(CreateStatLVItem());
                listInfosR.Items.Add(CreateStatLVItem());
                listInfosR.Items.Add(CreateStatLVItem());
            }

            // Misc & special effects (4 max)
            var effectsHidden = new[]
            {
                Enums.eEffectType.GrantPower,
                Enums.eEffectType.RevokePower,
                Enums.eEffectType.PowerRedirect,
                Enums.eEffectType.Null,
                Enums.eEffectType.SetMode,
                Enums.eEffectType.EntCreate,
                Enums.eEffectType.Damage
            };

            var miscEffectsIndexes = _enhancedPower.Effects.FindIndexes(e => !effectsHidden.Contains(e.EffectType)).ToList();
            for (var i = 0 ; i < Math.Min(4, miscEffectsIndexes.Count) ; i++)
            {
                if (miscEffectsIndexes[i] >= _basePower.Effects.Length || _basePower.Effects[miscEffectsIndexes[i]].EffectType != _enhancedPower.Effects[miscEffectsIndexes[i]].EffectType)
                {
                    var fx = _enhancedPower.Effects[miscEffectsIndexes[i]];
                    var fxType = fx.EffectType switch
                    {
                        Enums.eEffectType.Enhancement => $"{fx.EffectType}({fx.ETModifies})",
                        Enums.eEffectType.MezResist => $"{fx.EffectType}({fx.MezType})",
                        Enums.eEffectType.Mez => $"{fx.EffectType}({fx.MezType})",
                        Enums.eEffectType.Resistance => $"{fx.EffectType}({fx.DamageType})",
                        Enums.eEffectType.Defense => $"{fx.EffectType}({fx.DamageType})",
                        _ => $"{fx.EffectType}"
                    };

                    var enhValue = fx.EffectType switch
                    {
                        Enums.eEffectType.Mez when fx.MezType == Enums.eMez.Knockback | fx.MezType == Enums.eMez.Knockup => fx.BuffedMag,
                        Enums.eEffectType.Mez => fx.Duration,
                        _ => fx.BuffedMag
                    };

                    if (i % 2 == 0)
                    {
                        listInfosL.Items.Add(CreateStatLVItem(fxType, fx.DisplayPercentage ? $"{fx.BuffedMag:P2}" : $"{fx.BuffedMag:###.##}", BoostType.Extra));
                    }
                    else
                    {
                        listInfosR.Items.Add(CreateStatLVItem(fxType, fx.DisplayPercentage ? $"{fx.BuffedMag:P2}" : $"{fx.BuffedMag:###.##}", BoostType.Extra));
                    }
                }
                else
                {
                    var fxEnh = _enhancedPower.Effects[miscEffectsIndexes[i]];
                    var fxBase = _basePower.Effects[miscEffectsIndexes[i]];
                    var fxType = fxEnh.EffectType switch
                    {
                        Enums.eEffectType.Enhancement => $"{fxEnh.EffectType}({fxEnh.ETModifies})",
                        Enums.eEffectType.MezResist => $"{fxEnh.EffectType}({fxEnh.MezType})",
                        Enums.eEffectType.Mez => $"{fxEnh.EffectType}({fxEnh.MezType})",
                        Enums.eEffectType.Resistance => $"{fxEnh.EffectType}({fxEnh.DamageType})",
                        Enums.eEffectType.Defense => $"{fxEnh.EffectType}({fxEnh.DamageType})",
                        _ => $"{fxEnh.EffectType}"
                    };

                    var enhValue = fxEnh.EffectType switch
                    {
                        Enums.eEffectType.Mez when fxEnh.MezType == Enums.eMez.Knockback | fxEnh.MezType == Enums.eMez.Knockup => fxEnh.BuffedMag,
                        Enums.eEffectType.Mez => fxEnh.Duration,
                        _ => fxEnh.BuffedMag
                    };

                    var baseValue = fxBase.EffectType switch
                    {
                        Enums.eEffectType.Mez when fxBase.MezType == Enums.eMez.Knockback | fxBase.MezType == Enums.eMez.Knockup => fxBase.BuffedMag,
                        Enums.eEffectType.Mez => fxBase.Duration,
                        _ => fxBase.BuffedMag
                    };

                    if (i % 2 == 0)
                    {
                        listInfosL.Items.Add(CreateStatLVItem(fxType, fxEnh.DisplayPercentage ? $"{enhValue:P2}" : $"{enhValue:###.##}", GetBoostType(baseValue, enhValue)));
                    }
                    else
                    {
                        listInfosR.Items.Add(CreateStatLVItem(fxType, fxEnh.DisplayPercentage ? $"{enhValue:P2}" : $"{enhValue:###.##}", GetBoostType(baseValue, enhValue)));
                    }
                }
            }

            listInfosL.EndUpdate();
            listInfosR.EndUpdate();

            var baseDamage = _basePower.FXGetDamageValue();
            var enhancedDamage = _enhancedPower.FXGetDamageValue();
            ctlDamageDisplay1.nBaseVal = baseDamage;
            ctlDamageDisplay1.nMaxEnhVal = baseDamage * (1 + Enhancement.ApplyED(Enums.eSchedule.A, 2.277f));
            ctlDamageDisplay1.nEnhVal = enhancedDamage;
            ctlDamageDisplay1.Text = Math.Abs(enhancedDamage - baseDamage) > float.Epsilon
                ? $"{_enhancedPower.FXGetDamageString()} ({Utilities.FixDP(baseDamage)})"
                : _basePower.FXGetDamageString();
        }

        #endregion

        #region Effects Tab
        
        private void DisplayEffects()
        {

        }

        #endregion

        #region Totals Tab
        private void DisplayTotals()
        {

        }

        #endregion

        #region Enhance Tab

        private void DisplayEnhance()
        {

        }

        #endregion

        #region Event callbacks

        private void tabBox_TabIndexChanged(object sender, EventArgs e)
        {
            switch (tabBox.TabIndex)
            {
                case 0:
                    // L=39 / L=23
                    tabBox.ActiveTabColor = Color.FromArgb(12, 56, 100);
                    tabBox.InactiveTabColor = Color.FromArgb(7, 33, 59);
                    break;

                case 1:
                    // L=51 / L=30
                    tabBox.ActiveTabColor = Color.Indigo;
                    tabBox.InactiveTabColor = Color.FromArgb(45, 0, 77);
                    break;

                case 2:
                    // L=50 / L=30
                    tabBox.ActiveTabColor = Color.Green;
                    tabBox.InactiveTabColor = Color.FromArgb(0, 77, 0);
                    break;

                case 3:
                    // L=50 / L=30
                    tabBox.ActiveTabColor = Color.Teal;
                    tabBox.InactiveTabColor = Color.FromArgb(0, 77, 77);
                    break;
            }
        }

        #endregion

        private static Color InterpolateColor(decimal value, decimal valueMin, decimal valueMax, ColorRange colorRange)
        {
            return Color.FromArgb(
                (int)Math.Round((value - valueMin) / (valueMax - valueMin) * (colorRange.UpperBoundColor.R - colorRange.LowerBoundColor.R) + colorRange.LowerBoundColor.R),
                (int)Math.Round((value - valueMin) / (valueMax - valueMin) * (colorRange.UpperBoundColor.G - colorRange.LowerBoundColor.G) + colorRange.LowerBoundColor.G),
                (int)Math.Round((value - valueMin) / (valueMax - valueMin) * (colorRange.UpperBoundColor.B - colorRange.LowerBoundColor.B) + colorRange.LowerBoundColor.B)
            );
        }

        private void powerScaler_ValueChanged(object sender, EventArgs e)
        {
            var target = (ColorSlider)sender;

            target.ElapsedInnerColor = InterpolateColor(target.Value, target.Minimum, target.Maximum, TrackColors.ElapsedInnerColor);
            target.ElapsedPenColorBottom = InterpolateColor(target.Value, target.Minimum, target.Maximum, TrackColors.ElapsedPenColorBottom);
            target.ElapsedPenColorTop = InterpolateColor(target.Value, target.Minimum, target.Maximum, TrackColors.ElapsedPenColorTop);

            if (FreezeScalerCB) return;

            MainModule.MidsController.Toon.GenerateBuffedPowerArray();

            // TODO: display updated infos (???)
        }
    }
}