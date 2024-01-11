using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FastDeepCloner;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms.Controls;
using MRBResourceLib;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmStats : Form
    {
        private enum DisplayMode
        {
            Accuracy,
            Damage,
            DPA,
            DPS,
            DPE,
            EndUse,
            EndPerSec,
            Healing,
            HPS,
            HPE,
            EffectDuration,
            Range,
            RechargeTime,
            Regeneration
        }
        
        private readonly frmMain myParent;

        private IPower[] BaseArray;
        private bool BaseOverride;

        private ComboBox cbSet;

        private ComboBox cbStyle;

        private ComboBox cbValues;

        private IPower[] EnhArray;
        private CtlMultiGraph Graph;
        private float GraphMax;
        private Label lblKey1;
        private Label lblKey2;
        private Label lblKeyColor1;
        private Label lblKeyColor2;
        private Label lblScale;
        private bool Loaded;

        private TrackBar tbScaleX;
        private ToolTip tTip;
        private DisplayMode StatDisplayed;

        public frmStats(ref frmMain iParent)
        {
            FormClosed += frmStats_FormClosed;
            Load += frmStats_Load;
            Move += frmStats_Move;
            Resize += frmStats_Resize;
            //VisibleChanged += frmStats_VisibleChanged;
            BaseArray = Array.Empty<IPower?>();
            EnhArray = Array.Empty<IPower?>();
            GraphMax = 1;
            BaseOverride = false;
            Loaded = false;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();
            btnClose.Click += btnClose_Click;
            chkOnTop.Click += chkOnTop_Click;
            Name = nameof(frmStats);
            //var componentResourceManager = new ComponentResourceManager(typeof(frmStats));
            Icon = Resources.MRB_Icon_Concept;
            myParent = iParent;
        }

        private void btnClose_Click(object? sender, EventArgs e)
        {
            Close();
        }

        private void cbSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded)
            {
                return;
            }

            GetPowerArray();
            DisplayGraph();
        }

        private void cbStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded)
            {
                return;
            }

            SetGraphType();
            DisplayGraph();
        }

        private void cbValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded)
            {
                return;
            }

            DisplayGraph();
        }

        private void chkOnTop_Click(object? sender, EventArgs e)
        {
            TopMost = chkOnTop.ToggleState switch
            {
                ImageButtonEx.States.ToggledOn => true,
                _ => false
            };
        }

        private void DisplayGraph()
        {
            if ((MainModule.MidsController.Toon == null) | !MainModule.MidsController.IsAppInitialized)
            {
                return;
            }

            Graph.BeginUpdate();
            Graph.Clear();
            if (cbValues.SelectedIndex > -1)
            {
                StatDisplayed = (DisplayMode) cbValues.SelectedIndex;
                ConfigData.EDamageReturn returnValue;
                switch (StatDisplayed)
                {
                    case DisplayMode.Accuracy:
                        Graph.ColorFadeEnd = Color.FromArgb(192, 192, 0);
                        SetGraphValues((b, e) =>
                            {
                                var baseAccuracy = b.Accuracy;
                                if (BaseOverride)
                                {
                                    return baseAccuracy * 100;
                                }

                                return MidsContext.Config.ScalingToHit * baseAccuracy * 100;
                            },

                            (b, e) =>
                            {
                                var baseAccuracy = b.Accuracy;
                                var enhAccuracy = e.Accuracy;
                                if (BaseOverride)
                                {
                                    return enhAccuracy * 100;
                                }

                                if (Math.Abs(e.Accuracy - baseAccuracy) < float.Epsilon)
                                {
                                    enhAccuracy *= MidsContext.Config.ScalingToHit;
                                }

                                return enhAccuracy * 100;
                            },
                            (b, e) =>
                            {
                                if (Math.Abs(b.Accuracy) < float.Epsilon)
                                {
                                    return false;
                                }

                                return b.EntitiesAutoHit == Enums.eEntity.None | b.Range > 20 &
                                    b.I9FXPresentP(Enums.eEffectType.Mez, Enums.eMez.Taunt);
                            },
                            DisplayMode.Accuracy);
                        break;
                    case DisplayMode.Damage:
                        Graph.ColorFadeEnd = Color.DarkRed;
                        SetGraphValues((b, e) => b.FXGetDamageValue(),
                            (b, e) => e.FXGetDamageValue(),
                            (b, e) => Math.Abs(b.FXGetDamageValue()) >= float.Epsilon,
                            DisplayMode.Damage, "");
                        break;
                    case DisplayMode.DPA:
                        Graph.ColorFadeEnd = Color.DarkRed;
                        returnValue = MidsContext.Config.DamageMath.ReturnValue;
                        MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.DPA;
                        SetGraphValues((b, e) => b.FXGetDamageValue(),
                            (b, e) => e.FXGetDamageValue(),
                            (b, e) => Math.Abs(b.FXGetDamageValue()) >= float.Epsilon,
                            DisplayMode.DPA, "");
                        MidsContext.Config.DamageMath.ReturnValue = returnValue;
                        break;
                    case DisplayMode.DPS:
                        Graph.ColorFadeEnd = Color.DarkRed;
                        returnValue = MidsContext.Config.DamageMath.ReturnValue;
                        MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.DPS;
                        SetGraphValues((b, e) => b.FXGetDamageValue(),
                            (b, e) => e.FXGetDamageValue(),
                            (b, e) => Math.Abs(b.FXGetDamageValue()) >= float.Epsilon,
                            DisplayMode.DPS, "");
                        MidsContext.Config.DamageMath.ReturnValue = returnValue;
                        break;
                    case DisplayMode.DPE:
                        Graph.ColorFadeEnd = Color.DarkRed;
                        returnValue = MidsContext.Config.DamageMath.ReturnValue;
                        MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.Numeric;
                        SetGraphValues((b, e) => b.FXGetDamageValue(),
                            (b, e) => e.FXGetDamageValue(),
                            (b, e) => Math.Abs(b.FXGetDamageValue()) >= float.Epsilon,
                            DisplayMode.DPE, "");
                        MidsContext.Config.DamageMath.ReturnValue = returnValue;
                        break;
                    case DisplayMode.EndUse:
                        Graph.ColorFadeEnd = Color.FromArgb(192, 192, 255);
                        SetGraphValues((b, e) => b.EndCost,
                            (b, e) => e.EndCost,
                            (b, e) => b.EndCost >= float.Epsilon,
                            DisplayMode.EndUse, "/s");
                        break;
                    case DisplayMode.EndPerSec:
                        Graph.ColorFadeEnd = Color.FromArgb(192, 192, 255);
                        SetGraphValues((b, e) =>
                            {
                                var nBase = b.EndCost;
                                switch (b.PowerType)
                                {
                                    case Enums.ePowerType.Click when e.RechargeTime + e.CastTime + e.InterruptTime > 0:
                                        nBase = b.EndCost / (b.RechargeTime + b.CastTime + b.InterruptTime);
                                        break;
                                    case Enums.ePowerType.Toggle:
                                        nBase = b.EndCost / b.ActivatePeriod;
                                        break;
                                }

                                return nBase;
                            },
                            (b, e) =>
                            {
                                var nEnh = e.EndCost;
                                switch (b.PowerType)
                                {
                                    case Enums.ePowerType.Click when e.RechargeTime + e.CastTime + e.InterruptTime > 0:
                                        nEnh = e.EndCost / (e.RechargeTime + e.CastTime + e.InterruptTime);
                                        break;
                                    case Enums.ePowerType.Toggle:
                                        nEnh = e.EndCost / e.ActivatePeriod;
                                        break;
                                }

                                return nEnh;
                            },
                            (b, e) => Math.Abs(b.EndCost) >= float.Epsilon,
                            DisplayMode.EndPerSec, "/s");
                        break;
                    case DisplayMode.Healing:
                        Graph.ColorFadeEnd = Color.FromArgb(96, 255, 96);
                        SetGraphValues((b, e) => b.GetEffectMagSum(Enums.eEffectType.Heal).Sum,
                            (b, e) => e.GetEffectMagSum(Enums.eEffectType.Heal).Sum,
                            (b, e) => Math.Abs(b.GetEffectMagSum(Enums.eEffectType.Heal).Sum) >= float.Epsilon,
                            DisplayMode.Healing); // ???
                        break;
                    case DisplayMode.HPS:
                        Graph.ColorFadeEnd = Color.FromArgb(96, 255, 96);
                        SetGraphValues((b, e) =>
                            {
                                var nBase = b.GetEffectMagSum(Enums.eEffectType.Heal).Sum;
                                switch (b.PowerType)
                                {
                                    case Enums.ePowerType.Click when e.RechargeTime + e.CastTime + e.InterruptTime > 0:
                                        nBase /= b.RechargeTime + b.CastTime + b.InterruptTime;
                                        break;
                                    case Enums.ePowerType.Toggle:
                                        nBase /= b.ActivatePeriod;
                                        break;
                                }

                                return nBase;
                            },
                            (b, e) =>
                            {
                                var nEnh = e.GetEffectMagSum(Enums.eEffectType.Heal).Sum;
                                switch (b.PowerType)
                                {
                                    case Enums.ePowerType.Click when e.RechargeTime + e.CastTime + e.InterruptTime > 0:
                                        nEnh /= e.RechargeTime + e.CastTime + e.InterruptTime;
                                        break;
                                    case Enums.ePowerType.Toggle:
                                        nEnh /= e.ActivatePeriod;
                                        break;
                                }

                                return nEnh;
                            },
                            (b, e) => Math.Abs(b.GetEffectMagSum(Enums.eEffectType.Heal).Sum) >= float.Epsilon,
                            DisplayMode.HPS); // ???
                        break;
                    case DisplayMode.HPE:
                        Graph.ColorFadeEnd = Color.FromArgb(96, 255, 96);
                        SetGraphValues((b, e) => b.GetEffectMagSum(Enums.eEffectType.Heal).Sum / (e.EndCost > 0 ? b.EndCost : 1),
                            (b, e) => e.GetEffectMagSum(Enums.eEffectType.Heal).Sum / (e.EndCost > 0 ? e.EndCost : 1),
                            (b, e) => Math.Abs(b.GetEffectMagSum(Enums.eEffectType.Heal).Sum) >= float.Epsilon,
                            DisplayMode.HPE); // ???
                        break;
                    case DisplayMode.EffectDuration:
                        Graph.ColorFadeEnd = Color.FromArgb(128, 0, 255);
                        SetGraphValues((b, e) =>
                            {
                                var durationEffectId = b.GetDurationEffectID();
                                
                                return b.Effects[durationEffectId].Duration;
                            },
                            (b, e) =>
                            {
                                var durationEffectId = b.GetDurationEffectID();

                                return e.Effects[durationEffectId].Duration;
                            },
                            (b, e) =>
                            {
                                var durationEffectId = b.GetDurationEffectID();
                                
                                return durationEffectId >= 0;
                            },
                            DisplayMode.EffectDuration, "s");
                        break;
                    case DisplayMode.Range:
                        Graph.ColorFadeEnd = Color.FromArgb(64, 128, 96);
                        SetGraphValues((b, e) =>
                            {
                                var nBase = 0f;
                                if (b.Range > 0)
                                {
                                    nBase = b.Range;
                                }
                                else if (b.Radius > 0)
                                {
                                    nBase = b.Radius;
                                }

                                return nBase;
                            },
                            (b, e) =>
                            {
                                var nEnh = 0f;
                                if (b.Range > 0)
                                {
                                    nEnh = e.Range;
                                }
                                else if (b.Radius > 0)
                                {
                                    nEnh = e.Radius;
                                }

                                return nEnh;
                            },
                            (b, e) =>
                            {
                                var nBase = 0f;
                                if (b.Range > 0)
                                {
                                    nBase = b.Range;
                                }
                                else if (b.Radius > 0)
                                {
                                    nBase = b.Radius;
                                }

                                return Math.Abs(nBase) >= float.Epsilon;
                            },
                            DisplayMode.Range, "ft");
                        break;
                    case DisplayMode.RechargeTime:
                        Graph.ColorFadeEnd = Color.FromArgb(255, 192, 128);
                        SetGraphValues((b, e) => b.RechargeTime,
                            (b, e) => e.RechargeTime,
                            (b, e) => Math.Abs(b.RechargeTime) >= float.Epsilon,
                            DisplayMode.RechargeTime, "s");
                        break;
                    case DisplayMode.Regeneration:
                        Graph.ColorFadeEnd = Color.FromArgb(96, 192, 96);
                        SetGraphValues((b, e) => b.GetEffectMagSum(Enums.eEffectType.Regeneration).Sum,
                            (b, e) => e.GetEffectMagSum(Enums.eEffectType.Regeneration).Sum,
                            (b, e) => Math.Abs(b.GetEffectMagSum(Enums.eEffectType.Regeneration).Sum) >= float.Epsilon,
                            DisplayMode.Regeneration, "%/s"); // ???
                        break;
                }
            }

            Graph.Max = GraphMax;
            tbScaleX.Value = Graph.ScaleIndex;
            SetScaleLabel();
            SetGraphMetrics();
            Graph.EndUpdate();
            Graph.Draw();
        }

        private void FillComboBoxes()
        {
            NewSets();
            cbValues.BeginUpdate();
            var items1 = cbValues.Items;
            items1.Clear();
            items1.Add("Accuracy");
            items1.Add("Damage");
            items1.Add("Damage / Anim");
            items1.Add("Damage / Sec");
            items1.Add("Damage / End");
            items1.Add("End Use");
            items1.Add("End / Sec");
            items1.Add("Healing");
            items1.Add("Heal / Sec");
            items1.Add("Heal / End");
            items1.Add("Effect Duration");
            items1.Add("Range");
            items1.Add("Recharge Time");
            items1.Add("Regeneration");
            cbValues.SelectedIndex = 1;
            cbValues.EndUpdate();
            cbStyle.BeginUpdate();
            var items2 = cbStyle.Items;
            items2.Clear();
            items2.Add("Base & Enhanced");
            items2.Add("Stacked Base + Enhanced");
            items2.Add("Base Only");
            items2.Add("Enhanced Only");
            items2.Add("Active & Alternate");
            items2.Add("Stacked Active + Alt");
            if (MidsContext.Config.StatGraphStyle > (Enums.GraphStyle) (cbStyle.Items.Count - 1))
            {
                MidsContext.Config.StatGraphStyle = Enums.GraphStyle.Stacked;
            }

            cbStyle.SelectedIndex = (int) MidsContext.Config.StatGraphStyle;
            cbStyle.EndUpdate();
        }

        private void frmStats_FormClosed(object sender, FormClosedEventArgs e)
        {
            myParent.FloatStatGraph(false);
        }

        private void frmStats_Load(object sender, EventArgs e)
        {
            FillComboBoxes();
            Loaded = true;
            tbScaleX.Minimum = 0;
            tbScaleX.Maximum = Graph.ScaleCount - 1;
            chkOnTop.ToggleState = TopMost switch
            {
                true => ImageButtonEx.States.ToggledOn,
                _ => ImageButtonEx.States.ToggledOff
            };

            UpdateColorTheme();
            UpdateData(false);
        }

        public void UpdateColorTheme()
        {
            chkOnTop.UseAlt = MidsContext.Character.Alignment is Enums.Alignment.Villain or Enums.Alignment.Rogue or Enums.Alignment.Loyalist;
            btnClose.UseAlt = MidsContext.Character.Alignment is Enums.Alignment.Villain or Enums.Alignment.Rogue or Enums.Alignment.Loyalist;
        }

        public void UpdateColorTheme(Enums.Alignment alignment)
        {
            chkOnTop.UseAlt = alignment is Enums.Alignment.Villain or Enums.Alignment.Rogue or Enums.Alignment.Loyalist;
            btnClose.UseAlt = alignment is Enums.Alignment.Villain or Enums.Alignment.Rogue or Enums.Alignment.Loyalist;
        }

        private void frmStats_Move(object sender, EventArgs e)
        {
            StoreLocation();
        }

        private void frmStats_Resize(object sender, EventArgs e)
        {
            if (Graph != null)
            {
                Graph.Width = ClientSize.Width - Graph.Left - 4;
                Graph.Height = ClientSize.Height - Graph.Top - ClientSize.Height + tbScaleX.Top - 4;
                tbScaleX.Width = ClientSize.Width - tbScaleX.Left - ClientSize.Width + chkOnTop.Left - 4;
                lblScale.Left = (int) Math.Round(tbScaleX.Left + (tbScaleX.Width - lblScale.Width) / 2f);
                cbStyle.Width = cbStyle.Left + 157 > ClientSize.Width
                    ? ClientSize.Width - cbStyle.Left - 4
                    : 186;
            }

            btnClose.Location = new Point(Math.Max(4, ClientSize.Width - btnClose.Width - 4), Math.Max(4, ClientSize.Height - btnClose.Height - 4));
            chkOnTop.Location = new Point(Math.Max(4, ClientSize.Width - chkOnTop.Width - 4), Math.Max(4, ClientSize.Height - chkOnTop.Height - 31));

            StoreLocation();
        }

        private void GetPowerArray()
        {
            if (MainModule.MidsController.Toon == null | !MainModule.MidsController.IsAppInitialized)
            {
                return;
            }

            // Powers in build
            var powers = new Dictionary<int, KeyValuePair<IPower, IPower>>();
            for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
            {
                var pBase = MainModule.MidsController.Toon.GetBasePower(i);
                var pEnh = MainModule.MidsController.Toon.GetEnhancedPower(i);

                if (pBase == null)
                {
                    continue;
                }

                var baseDmg = pBase.FXGetDamageValue();
                var enhDmg = pEnh?.FXGetDamageValue(pEnh == null) ?? 0;

                powers.Add(i,
                    baseDmg <= enhDmg
                        ? new KeyValuePair<IPower, IPower>(pBase, pEnh)
                        : new KeyValuePair<IPower, IPower>(pEnh, pBase));
            }

            // Filter out zero damage powers
            powers = powers
                .Where(e => (e.Value.Value != null && e.Value.Value.FXGetDamageValue(e.Value.Value == null) > 0) || (e.Value.Key != null && e.Value.Key.FXGetDamageValue() > 0))
                .ToDictionary(e => e.Key, e => e.Value);

            // basePower FullName -> index
            var powersDict = powers
                .Select(e => new KeyValuePair<IPower, int>(e.Value.Key, e.Key))
                .ToDictionary(e => e.Key.FullName, e => e.Value);

            // Selected powersets in build
            var powersets = MainModule.MidsController.Toon.Powersets
                .Where(e => e != null)
                .ToList();

            // Root Powers
            var rootPowers = new Dictionary<string, string>();
            foreach (var p in powers)
            {
                var basePower = p.Value.Key;
                var enhancedPower = p.Value.Value;
                var rootPowerName = Power.GetRootPowerName(basePower, enhancedPower);

                if (!string.IsNullOrEmpty(rootPowerName))
                {
                    rootPowers.Add(rootPowerName, basePower.FullName);
                }
            }

            var rootPowersNames = rootPowers
                .Select(e => e.Key)
                .ToList();

            // Powers from powersets (use build base + enhanced if available)
            var powersetsPowers = new Dictionary<int, KeyValuePair<IPower, IPower>>();
            var k = 0;
            foreach (var ps in powersets)
            {
                foreach (var p in ps.Powers)
                {
                    if (powersDict.TryGetValue(p.FullName, out var idx1))
                    {
                        powersetsPowers.Add(k++, new KeyValuePair<IPower, IPower>(powers[idx1].Key.Clone(), powers[idx1].Value.Clone()));
                    }
                    else if (rootPowersNames.Contains(p.FullName))
                    {
                        var idx2 = powersDict[rootPowers[p.FullName]];
                        powersetsPowers.Add(k++, new KeyValuePair<IPower, IPower>(powers[idx2].Key.Clone(), powers[idx2].Value.Clone()));
                    }
                    else
                    {
                        var pBase = p.Clone();
                        pBase?.ProcessExecutes();
                        pBase?.AbsorbPetEffects();
                        powersetsPowers.Add(k++, new KeyValuePair<IPower, IPower>(pBase, pBase));
                    }
                }
            }

            // Apply selection filter
            powers = cbSet.SelectedIndex switch
            {
                // Primary/Secondary
                1 => powersetsPowers
                    .Where(e => e.Value.Key.FullName.StartsWith("Redirects.") | e.Value.Key.GetPowerSet().SetType is Enums.ePowerSetType.Primary or Enums.ePowerSetType.Secondary)
                    .ToDictionary(e => e.Key, e => e.Value),
                
                // Primary
                2 => powersetsPowers
                    .Where(e => e.Value.Key.FullName.StartsWith("Redirects.") | e.Value.Key.GetPowerSet().SetType == Enums.ePowerSetType.Primary)
                    .ToDictionary(e => e.Key, e => e.Value),
                
                // Secondary
                3 => powersetsPowers
                    .Where(e => e.Value.Key.FullName.StartsWith("Redirects.") | e.Value.Key.GetPowerSet().SetType == Enums.ePowerSetType.Secondary)
                    .ToDictionary(e => e.Key, e => e.Value),
                
                // Epic/Ancillary
                4 => powersetsPowers
                    .Where(e => e.Value.Key.FullName.StartsWith("Redirects.") | e.Value.Key.GetPowerSet().SetType == Enums.ePowerSetType.Ancillary)
                    .ToDictionary(e => e.Key, e => e.Value),
                
                // Pools
                5 => powersetsPowers
                    .Where(e => e.Value.Key.FullName.StartsWith("Redirects.") | e.Value.Key.GetPowerSet().SetType == Enums.ePowerSetType.Pool)
                    .ToDictionary(e => e.Key, e => e.Value),
                
                // Powers taken
                6 => powers,
                
                // All toggles
                7 => powers
                    .Where(e => e.Value.Key.PowerType == Enums.ePowerType.Toggle)
                    .ToDictionary(e => e.Key, e => e.Value),

                // All clicks
                8 => powersetsPowers
                    .Where(e => e.Value.Key.PowerType == Enums.ePowerType.Click)
                    .ToDictionary(e => e.Key, e => e.Value),

                _ => powersetsPowers
            };

            BaseArray = powers.Select(e => e.Value.Key).ToArray();
            EnhArray = powers.Select(e => e.Value.Value).ToArray();
        }

        /// <summary>
        /// Fill graph with values according to viewed stat.
        /// </summary>
        /// <param name="getBaseValue">Lambda function of type f(basePower:IPower?, enhancedPower:IPower?) : float to get base value of each element</param>
        /// <param name="getEnhValue">Lambda function of type f(basePower:IPower?, enhancedPower:IPower?) : float to get enhanced value of each element</param>
        /// <param name="filterPower">Lambda function of type f(basePower:IPower?, enhancedPower:IPower?) : bool to filter each element. If it returns false, element will be ignored.</param>
        /// <param name="statDisplayed">Stat displayed, one of display stat type (<see cref="DisplayMode"/>)</param>
        /// <param name="valueSuffix">Value suffix or unit to use in tooltips.</param>
        private void SetGraphValues(Func<IPower?, IPower?, float> getBaseValue, Func<IPower?, IPower?, float> getEnhValue,
            Func<IPower?, IPower?, bool> filterPower, DisplayMode statDisplayed, string valueSuffix = "%")
        {
            var num1 = 1f;
            for (var index = 0; index < BaseArray.Length; index++)
            {
                if (!filterPower(BaseArray[index], EnhArray[index]))
                {
                    continue;
                }

                var nBase = getBaseValue(BaseArray[index], EnhArray[index]);
                var nEnh = getEnhValue(BaseArray[index], EnhArray[index]);
                var displayName = BaseArray[index].DisplayName;

                if (num1 < nEnh)
                {
                    num1 = nEnh;
                }

                if (num1 < nBase)
                {
                    num1 = nBase;
                }

                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                }

                var tip = string.Empty;
                var str = string.Empty;
                var baseHealValue = nBase / MidsContext.Archetype.Hitpoints * 100;
                var enhHealValue = nEnh / MidsContext.Archetype.Hitpoints * 100;

                switch (statDisplayed)
                {
                    case DisplayMode.Accuracy:
                        tip = Graph.Style == Enums.GraphStyle.baseOnly
                            ? $"{displayName}: {nBase:##0.#}{valueSuffix}"
                            : $"{displayName}: {nEnh:##0.#}{valueSuffix}";

                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $" ({nBase:##0.#}{valueSuffix})";
                        }

                        break;

                    case DisplayMode.Damage:
                        if (Graph.Style == Enums.GraphStyle.baseOnly)
                        {
                            tip += $"\r\n{BaseArray[index].FXGetDamageString()}";
                        }
                        else
                        {
                            tip += !BaseOverride
                                ? $"\r\n{EnhArray[index].FXGetDamageString()}"
                                : $"\r\n{BaseArray[index].FXGetDamageString()}";
                        }

                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $" ({nBase:##0.##})";
                        }

                        if (BaseArray[index].PowerType == Enums.ePowerType.Toggle)
                        {
                            tip += $"\r\n(Applied every {BaseArray[index].ActivatePeriod}s)";
                        }

                        break;

                    case DisplayMode.DPA:
                        str = BaseArray[index].DisplayName;

                        if (Graph.Style == Enums.GraphStyle.baseOnly)
                        {
                            str += $"\r\n{BaseArray[index].FXGetDamageString()}";
                        }
                        else
                        {
                            if (!BaseOverride)
                            {
                                str += $"\r\n{EnhArray[index].FXGetDamageString()}";
                            }
                            else
                            {
                                str += $"\r\n{BaseArray[index].FXGetDamageString()}";
                            }
                        }

                        tip = $"{str}/s";
                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $" ({nBase:##0.##})";
                        }

                        break;

                    case DisplayMode.DPS:
                        str = BaseArray[index].DisplayName;

                        if (Graph.Style == Enums.GraphStyle.baseOnly)
                        {
                            str += $"\r\n{BaseArray[index].FXGetDamageString()}";
                        }
                        else
                        {
                            if (!BaseOverride)
                            {
                                str += $"\r\n{EnhArray[index].FXGetDamageString()}";
                            }
                            else
                            {
                                str += $"\r\n{BaseArray[index].FXGetDamageString()}";
                            }
                        }

                        tip = $"{str}/s";
                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $" ({nBase:##0.##})";
                        }

                        break;

                    case DisplayMode.DPE:
                        str = BaseArray[index].DisplayName;
                        
                        if (Graph.Style == Enums.GraphStyle.baseOnly)
                        {
                            str += $"\r\n{BaseArray[index].FXGetDamageString()}";
                        }
                        else
                        {
                            if (!BaseOverride)
                            {
                                str += $"\r\n{EnhArray[index].FXGetDamageString()}";
                            }
                            else
                            {
                                str += $"\r\n{BaseArray[index].FXGetDamageString()}";
                            }
                        }

                        tip = string.Empty;
                        if (Graph.Style == Enums.GraphStyle.baseOnly)
                        {
                            tip = $"{str}\r\nDamage per unit of End: {nBase:##0.##}";
                        }
                        else
                        {
                            tip = $"{str}\r\nDamage per unit of End: {nEnh:##0.##}";
                            if (Math.Abs(nBase - nEnh) > float.Epsilon)
                            {
                                tip += $" ({nBase:##0.##})";
                            }
                        }

                        break;

                    case DisplayMode.EffectDuration:
                        var durationEffectId = BaseArray[index].GetDurationEffectID();
                        if (durationEffectId <= -1)
                        {
                            tip = "";
                        }
                        else
                        {
                            str = EnhArray[index].Effects[durationEffectId].EffectType != Enums.eEffectType.Mez
                                ? Enums.GetEffectName(EnhArray[index].Effects[durationEffectId].EffectType)
                                : Enums.GetMezName((Enums.eMezShort) EnhArray[index].Effects[durationEffectId].MezType);
                            if (EnhArray[index].Effects[durationEffectId].Mag < 0)
                            {
                                str = $"-{str}";
                            }

                            tip = Graph.Style == Enums.GraphStyle.baseOnly
                                ? $"{displayName} ({str}): {nBase:##0.#}"
                                : $"{displayName} ({str}): {nEnh:##0.#}";

                            if (Math.Abs(nBase - nEnh) > float.Epsilon)
                            {
                                tip += $" ({nBase:##0.#})";
                            }

                            tip += "s";
                        }

                        break;

                    case DisplayMode.EndUse:
                        tip = Graph.Style != Enums.GraphStyle.baseOnly
                            ? $"{displayName}: {nEnh:##0.##}"
                            : $"{displayName}: {nBase:##0.##}";

                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $" ({nBase:##0.##})";
                        }

                        if (BaseArray[index].PowerType == Enums.ePowerType.Toggle)
                        {
                            tip += "\r\n(Per Second)";
                        }

                        break;

                    case DisplayMode.EndPerSec:
                        tip = Graph.Style != Enums.GraphStyle.baseOnly
                            ? $"{displayName}: {nEnh:##0.##}"
                            : $"{displayName}: {nBase:##0.##}/s";

                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $" ({nBase:##0.##})";
                        }

                        break;

                    case DisplayMode.Healing:
                        tip = Graph.Style == Enums.GraphStyle.baseOnly
                            ? $"{displayName}: {baseHealValue:##0.#}%"
                            : $"{displayName}\r\n Enhanced: {enhHealValue:##0.#}% ({nEnh:##0.#} HP)";

                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $"\r\n Base: {baseHealValue:##0.#}% ({nBase:##0.#} HP)";
                        }

                        break;

                    case DisplayMode.HPE:
                        tip = Graph.Style == Enums.GraphStyle.baseOnly
                            ? $"{displayName}: {nBase:##0.##}%"
                            : $"{displayName}\r\n Enhanced Heal per unit of End: {enhHealValue:##0.##}% ({nEnh:##0.##} HP)";

                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $"\r\n Base Heal per unit of End: {baseHealValue:##0.##}% ({nBase:##0.##} HP)";
                        }

                        break;

                    case DisplayMode.HPS:
                        tip = Graph.Style == Enums.GraphStyle.baseOnly 
                            ? $"{displayName}: {baseHealValue:##0.##}%" 
                            : $"{displayName}\r\n Enhanced: {enhHealValue:##0.##}%/s ({nEnh:##0.##} HP)";

                        if (Math.Abs(nBase - (double)nEnh) > float.Epsilon)
                        {
                            tip += $"\r\n Base: {baseHealValue:##0.#}%/s ({nBase:##0.##} HP)";
                        }

                        break;

                    case DisplayMode.Range:
                        tip = Graph.Style != Enums.GraphStyle.baseOnly
                            ? $"{displayName}: {nEnh:##0.#}{valueSuffix}"
                            : $"{displayName}: {nBase:##0.#}{valueSuffix}";

                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $" ({nBase:##0.#}{valueSuffix})";
                        }

                        tip += "ft";

                        break;

                    case DisplayMode.RechargeTime:
                        tip = Graph.Style != Enums.GraphStyle.baseOnly
                            ? $"{displayName}: {nEnh:##0.##}{valueSuffix}"
                            : $"{displayName}: {nBase:##0.##}{valueSuffix}";

                        if (Math.Abs(nBase - nEnh) > float.Epsilon)
                        {
                            tip += $" ({nBase:##0.##}{valueSuffix})";
                        }

                        break;

                    case DisplayMode.Regeneration:
                        var maxHp = MidsContext.Character.DisplayStats.HealthHitpointsNumeric(false);
                        var baseRegen = (float) (maxHp / 12f * (0.05 + 0.05 * ((nBase - 100) / 100f)));
                        var baseRegenPercent = baseRegen / maxHp * 100f;
                        var enhRegen = (float) (maxHp / 12f * (0.05 + 0.05 * ((nEnh - 100) / 100f)));
                        var enhRegenPercent = enhRegen / maxHp * 100;
                        if (BaseOverride)
                        {
                            (baseRegen, enhRegen) = (enhRegen, baseRegen);
                            (baseRegenPercent, enhRegenPercent) = (enhRegenPercent, baseRegenPercent);
                        }

                        if (Graph.Style == Enums.GraphStyle.baseOnly)
                        {
                            tip = $"Health regenerated per second: {baseRegenPercent:##0.##}%\r\n Hit Points regenerated per second at level 50: {baseRegen:##0.#} HP";
                        }
                        else if (Math.Abs(nBase - nEnh) < float.Epsilon)
                        {
                            tip = $"{displayName}: {nBase:##0.#}%\r\n Health regenerated per second: {baseRegenPercent:##0.##}%\r\n Hit Points regenerated per second at level 50: {baseRegen:##0.#} HP";
                        }
                        else
                        {
                            tip = $"{displayName}: {nEnh:##0.#}% ({nBase:##0.#}%)\r\n Health regenerated per second: {enhRegenPercent:##0.##}% ({baseRegenPercent:##0.##})\r\n Hit Points regenerated per second at level 50: {enhRegen:##0.#} HP ({baseRegen:##0.##})";
                        }

                        break;
                }

                tip = tip.Trim();

                if (BaseOverride)
                {
                    (nBase, nEnh) = (nEnh, nBase);
                    if (statDisplayed is DisplayMode.Healing or DisplayMode.HPE or DisplayMode.HPS)
                    {
                        tip = tip.Replace("Enhanced", "Active").Replace("Base", "Alternate");
                    }
                }

                Graph.AddItem(displayName, nBase, nEnh, tip);
            }

            GraphMax = num1 * 1.025f;
        }

        [DebuggerStepThrough]
        private void NewSets()
        {
            cbSet.BeginUpdate();
            var items = cbSet.Items;
            items.Clear();
            items.Add("All Sets");
            items.Add("Primary & Secondary");
            items.Add($"Primary ({MidsContext.Character.Powersets[0].DisplayName})");
            items.Add($"Secondary ({MidsContext.Character.Powersets[1].DisplayName})");
            items.Add("Ancillary");
            items.Add("Pools");
            items.Add("Powers Taken");
            items.Add("All Toggles");
            items.Add("All Clicks");
            cbSet.SelectedIndex = 1;
            cbSet.EndUpdate();
        }

        private void SetGraphMetrics()
        {
            if (Graph.ItemCount < 13.5)
            {
                Graph.ItemHeight = 18;
                Graph.PaddingY = 6f;
            }
            else if (Graph.ItemCount < 18.0)
            {
                Graph.ItemHeight = 15;
                Graph.PaddingY = 5f;
            }
            else if (Graph.ItemCount > 32)
            {
                Graph.PaddingY = 2f;
                Graph.ItemHeight = 10;
            }
            else if (Graph.ItemCount > 30)
            {
                Graph.PaddingY = 2f;
                Graph.ItemHeight = 11;
            }
            else if (Graph.ItemCount > 27)
            {
                Graph.PaddingY = 2.666667f;
                Graph.ItemHeight = 11;
            }
            else
            {
                Graph.ItemHeight = 12;
                Graph.PaddingY = 4f;
            }
        }

        private void SetGraphType()
        {
            if ((cbStyle.SelectedIndex > -1) & (cbStyle.SelectedIndex < cbStyle.Items.Count - 2))
            {
                Graph.Style = (Enums.GraphStyle) cbStyle.SelectedIndex;
                MidsContext.Config.StatGraphStyle = Graph.Style;
                BaseOverride = false;
            }
            else if (cbStyle.SelectedIndex == cbStyle.Items.Count - 2)
            {
                Graph.Style = Enums.GraphStyle.Twin;
                BaseOverride = true;
            }
            else if (cbStyle.SelectedIndex == cbStyle.Items.Count - 1)
            {
                Graph.Style = Enums.GraphStyle.Stacked;
                BaseOverride = true;
            }

            GetPowerArray();
            if (BaseOverride)
            {
                lblKey1.Text = "Active";
                lblKey2.Text = "Alternate";
            }
            else
            {
                lblKey1.Text = "Base";
                lblKey2.Text = "Enhanced";
            }

            MidsContext.Config.StatGraphStyle = Graph.Style;
        }

        public void SetLocation()
        {
            var rectangle = new Rectangle
            {
                X = MainModule.MidsController.SzFrmStats.X,
                Y = MainModule.MidsController.SzFrmStats.Y,
                Width = MainModule.MidsController.SzFrmStats.Width,
                Height = MainModule.MidsController.SzFrmStats.Height
            };
            
            if (rectangle.Width < 1)
            {
                rectangle.Width = Width;
            }

            if (rectangle.Height < 1)
            {
                rectangle.Height = Height;
            }

            if (rectangle.Width < MinimumSize.Width)
            {
                rectangle.Width = MinimumSize.Width;
            }

            if (rectangle.Height < MinimumSize.Height)
            {
                rectangle.Height = MinimumSize.Height;
            }

            if (rectangle.X < 1)
            {
                rectangle.X = (int) Math.Round((Screen.PrimaryScreen.Bounds.Width - Width) / 2.0);
            }

            if (rectangle.Y < 32)
            {
                rectangle.Y = (int) Math.Round((Screen.PrimaryScreen.Bounds.Height - Height) / 2.0);
            }

            Top = rectangle.Y;
            Left = rectangle.X;
            Height = rectangle.Height;
            Width = rectangle.Width;
        }

        private void SetScaleLabel()
        {
            lblScale.Text = $"Scale: 0 - {Graph.ScaleValue}";
        }

        private void StoreLocation()
        {
            if (!MainModule.MidsController.IsAppInitialized)
            {
                return;
            }

            MainModule.MidsController.SzFrmStats.X = Left;
            MainModule.MidsController.SzFrmStats.Y = Top;
            MainModule.MidsController.SzFrmStats.Width = Width;
            MainModule.MidsController.SzFrmStats.Height = Height;
        }

        private void tbScaleX_Scroll(object sender, EventArgs e)
        {
            Graph.ScaleIndex = tbScaleX.Value;
            Graph.Draw();
            SetScaleLabel();
        }

        public void UpdateData(bool NewData)
        {
            StatDisplayed = (DisplayMode) cbValues.SelectedIndex;
            BackColor = myParent.BackColor;
            Graph.BackColor = BackColor;
            if (NewData)
            {
                NewSets();
            }

            SetGraphType();
            GetPowerArray();
            DisplayGraph();
        }
    }
}