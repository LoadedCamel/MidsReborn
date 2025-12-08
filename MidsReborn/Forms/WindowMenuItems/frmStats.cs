using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms.Controls;
using MRBResourceLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Extensions;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmStats : Form
    {
        public enum DisplayMode
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
            Regeneration,
            Defense,
            Resistance,
            HealthEndurance,
            MovementStealth,
            MiscBuffs,
            StatusProtection,
            StatusResistance,
            DebuffResistance,
            Elusivity
        }

        private enum CompareGraphStyle
        {
            Diff,
            RawValues
        }

        private struct GraphColors
        {
            public Color BarBaseColor;
            public Color BarEnhColor;
            public Color BarOvercapColor;
            public Color FadeEndColor;
            public Color? NegativeBaseColor;
            public Color? NegativeEnhColor;
            public Color? NegativeOvercapColor;
        }

        private const int MaxDisplayModeNoCompare = 13;

        private readonly frmMain myParent;

        private IPower?[] BaseArray;
        private bool BaseOverride;

        private ComboBox cbSet;

        private ComboBox cbStyle;

        private ComboBox cbValues;

        private IPower?[] EnhArray;
        private CtlMultiGraph Graph;
        private Label lblKey1;
        private Label lblKey2;
        private Label lblKeyColor1;
        private Label lblKeyColor2;
        private Label lblScale;
        private bool Loaded;
        private bool NoDraw;

        private TrackBar tbScaleX;
        private ToolTip tTip;
        private DisplayMode StatDisplayed;
        private CompareGraphStyle CompareGraphMode;

        private bool CompareMode;
        private StatsPowerData? CompareData;
        private StatsPowerData.TotalStats Totals;

        // FeetPerSecond, MetersPerSecond, MilesPerHour, KilometersPerHour
        // Row -> Col
        private static readonly float[][] SpeedCnvMatrix =
        [
            [1, 3.28084f, 1.466667f, 0.911344f],
            [0.3048f, 1, 0.44704f, 0.277778f],
            [0.681818f, 2.236936f, 1, 0.621371f],
            [1.09728f, 3.6f, 1.609344f, 1]
        ];

        public frmStats(ref frmMain iParent)
        {
            FormClosed += frmStats_FormClosed;
            Load += frmStats_Load;
            Move += frmStats_Move;
            Resize += frmStats_Resize;
            //VisibleChanged += frmStats_VisibleChanged;
            BaseArray = [];
            EnhArray = [];
            BaseOverride = false;
            Loaded = false;
            CompareMode = false;
            CompareData = null;
            NoDraw = false;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();
            btnClose.Click += btnClose_Click;
            chkOnTop.Click += chkOnTop_Click;
            Name = nameof(frmStats);
            //var componentResourceManager = new ComponentResourceManager(typeof(frmStats));
            Icon = Resources.MRB_Icon_Concept;
            myParent = iParent;
        }

        public static void SetTitle(frmStats? frm)
        {
            if (frm == null)
            {
                return;
            }

            var titleTxt = "";
            var epicPowersetIndex = GetEpicPowersetIndex();
            var buildFileName = Path.GetFileName(frm.myParent.GetBuildFile());

            switch (MidsContext.Config.TotalsWindowTitleStyle)
            {
                case ConfigData.ETotalsWindowTitleStyle.CharNameAtPowersets:
                    titleTxt = $"{(!string.IsNullOrWhiteSpace(MidsContext.Character.Name) ? $"{MidsContext.Character.Name} - " : "")}{MidsContext.Character.Archetype.DisplayName}";
                    if (!MidsContext.Character.IsKheldian)
                    {
                        titleTxt += $" [ {MidsContext.Character.Powersets[0].DisplayName} / {MidsContext.Character.Powersets[1].DisplayName}{(epicPowersetIndex != -1 ? $" / {MidsContext.Character.Powersets[epicPowersetIndex].DisplayName}" : "")} ]";
                    }

                    frm.Text = $"Power Stats - {titleTxt}";
                    break;

                case ConfigData.ETotalsWindowTitleStyle.BuildFileAtPowersets:
                    if (!MidsContext.Character.IsKheldian)
                    {
                        titleTxt += $"{MidsContext.Character.Powersets[0].DisplayName} / {MidsContext.Character.Powersets[1].DisplayName}{(epicPowersetIndex != -1 ? $" / {MidsContext.Character.Powersets[epicPowersetIndex].DisplayName}" : "")} ";
                    }

                    titleTxt += MidsContext.Character.Archetype.DisplayName +
                                (MainModule.MidsController.Toon != null && !string.IsNullOrEmpty(buildFileName)
                                    ? $" [{buildFileName}]"
                                    : "");

                    frm.Text = $"Power Stats - {titleTxt}";
                    break;

                case ConfigData.ETotalsWindowTitleStyle.CharNameBuildFile:
                    titleTxt = !string.IsNullOrWhiteSpace(MidsContext.Character.Name)
                        ? $"{MidsContext.Character.Name} "
                        : "";

                    if (titleTxt == "")
                    {
                        titleTxt = !string.IsNullOrEmpty(buildFileName) ? buildFileName : "";
                    }
                    else
                    {
                        titleTxt += !string.IsNullOrEmpty(buildFileName) ? $" [{buildFileName}]" : "";
                    }

                    frm.Text = titleTxt == "" ? "Power Stats" : $"Power Stats - {titleTxt}";
                    break;

                default:
                    frm.Text = "Power Stats";
                    break;
            }
        }

        private static int GetEpicPowersetIndex()
        {
            var idx = -1;
            int i;

            // Fetch ancillary/epic powerset index
            for (i = 0; i < MidsContext.Character.Powersets.Length; i++)
            {
                if (MidsContext.Character.Powersets[i] == null)
                {
                    continue;
                }

                if (MidsContext.Character.Powersets[i].GroupName != "Epic")
                {
                    continue;
                }

                idx = i;
                break;
            }

            if (idx == -1)
            {
                return -1;
            }

            // Check if power taken in pool
            for (i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
            {
                if (MidsContext.Character.CurrentBuild.Powers[i] == null)
                {
                    continue;
                }

                if (MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset == MidsContext.Character.Powersets[idx].nID)
                {
                    return idx;
                }
            }

            return -1;
        }

        private void SizeElements()
        {
            btnClose.Location = new Point(Math.Max(4, ClientSize.Width - btnClose.Width - 4), Math.Max(4, ClientSize.Height - btnClose.Height - 4));
            chkOnTop.Location = new Point(Math.Max(4, ClientSize.Width - chkOnTop.Width - 4), Math.Max(4, ClientSize.Height - chkOnTop.Height - 31));

            ibExport.Location = btnClose.Location with { X = btnClose.Location.X - ibExport.Size.Width - 4 };
            ibExport.Visible = CompareMode;

            if (Graph != null)
            {
                Graph.Height = chkOnTop.Top - Graph.Top - 4;

                if (!CompareMode)
                {
                    Graph.Width = ClientSize.Width - 8;
                    tbScaleX.Width = chkOnTop.Left - tbScaleX.Left - 4;
                }
                else
                {
                    Graph.Width = (int)Math.Floor(ClientSize.Width / 2f) - 12;
                    CompareGraph.Left = Graph.Width + 16;
                    CompareGraph.Width = Graph.Width;
                    CompareGraph.Height = Graph.Height;
                    tbScaleX.Width = (int)Math.Round((chkOnTop.Left - tbScaleX.Left) / 2f) - 16;
                }

                lblScale.Left = (int)Math.Round(tbScaleX.Left + (tbScaleX.Width - lblScale.Width) / 2f);
            }

            StoreLocation();
        }

        private void SetUiForCompare(bool fillComboValues = true)
        {
            MinimumSize = CompareMode
                ? new Size(1020, 640)
                : new Size(508, 554);

            if (MainModule.MidsController.SzFrmStats == null ||
                MainModule.MidsController.SzFrmStatsCompare == null ||
                (MainModule.MidsController.SzFrmStats?.Width <= 0) |
                (MainModule.MidsController.SzFrmStats?.Height <= 0) |
                (MainModule.MidsController.SzFrmStatsCompare?.Width <= 0) |
                (MainModule.MidsController.SzFrmStatsCompare?.Height <= 0))
            {
                MainModule.MidsController.SzFrmStats = new Rectangle(Left, Top, 492, 515);
                MainModule.MidsController.SzFrmStatsCompare = new Rectangle(Left, Top, 1004, 601);
            }

            var storedCompareSize = MainModule.MidsController.SzFrmStatsCompare!.Value with { X = 0, Y = 0 };
            var storedSize = MainModule.MidsController.SzFrmStats!.Value with { X = 0, Y = 0 };

            ClientSize = CompareMode
                ? new Size(Math.Max(1004, storedCompareSize.Width), Math.Max(601, storedCompareSize.Height))
                : new Size(Math.Max(492, storedSize.Width), Math.Max(515, storedSize.Height));

            var yOffset = CompareMode ? 64 : 0;
            Graph.Top = 57 + yOffset;
            CompareGraph.Top = 57 + yOffset;

            cbCompareGraphStyle.Visible = CompareMode & (StatDisplayed >= DisplayMode.Defense);
            ToolStripSeparator1.Visible = CompareMode;
            TsEndCompare.Visible = CompareMode;
            CompareGraph.Visible = CompareMode;

            if (fillComboValues)
            {
                var stat = cbValues.SelectedIndex;
                cbValues.BeginUpdate();
                cbValues.Items.Clear();
                if (!CompareMode)
                {
                    cbValues.Items.AddRange([
                        "Accuracy", "Damage", "Damage / Anim", "Damage / Sec", "Damage / End", "End Use", "End / Sec",
                        "Healing", "Heal / Sec", "Heal / End", "Effect Duration", "Range", "Recharge Time", "Regeneration"
                    ]);
                }
                else if (CompareData?.Metadata.PvMode == "PvP")
                {
                    cbValues.Items.AddRange([
                        "Accuracy", "Damage", "Damage / Anim", "Damage / Sec", "Damage / End", "End Use", "End / Sec",
                        "Healing", "Heal / Sec", "Heal / End", "Effect Duration", "Range", "Recharge Time", "Regeneration",
                        "Defense", "Resistance", "Health & Endurance", "Movement & Stealth", "Misc. Buffs", "Status Protection", "Status Resistance", "Debuff Resistance", "Elusivity"
                    ]);
                }
                else
                {
                    cbValues.Items.AddRange([
                        "Accuracy", "Damage", "Damage / Anim", "Damage / Sec", "Damage / End", "End Use", "End / Sec",
                        "Healing", "Heal / Sec", "Heal / End", "Effect Duration", "Range", "Recharge Time", "Regeneration",
                        "Defense", "Resistance", "Health & Endurance", "Movement & Stealth", "Misc. Buffs", "Status Protection", "Status Resistance", "Debuff Resistance"
                    ]);
                }
                cbValues.EndUpdate();

                if ((CompareData?.Metadata.PvMode == "PvP") & (StatDisplayed == DisplayMode.Elusivity))
                {
                    StatDisplayed = DisplayMode.Defense;
                    cbValues.SelectedIndex = (int)DisplayMode.Defense;
                }
                else
                {
                    cbValues.SelectedIndex = stat;
                }
            }

            if (CompareMode)
            {
                var txt1 = new StringBuilder();
                label1.Text = txt1.AppendJoin(' ',
                        "Current:",
                        MidsContext.Character.Name.Trim(),
                        string.IsNullOrWhiteSpace(MidsContext.Character.Name)
                            ? MidsContext.Config.Inc.DisablePvE
                                ? "PvP"
                                : "PvE"
                            : MidsContext.Config.Inc.DisablePvE
                                ? "(PvP)"
                                : "(PvE)",
                        "\r\n",
                        (MidsContext.Character.Powersets[0] != null) & (MidsContext.Character.Powersets[1] != null)
                            ? $"{MidsContext.Character.Powersets[0].DisplayName}/{MidsContext.Character.Powersets[1].DisplayName}"
                            : "",
                        MidsContext.Character.Archetype.DisplayName,
                        "\r\n",
                        Path.GetFileName(myParent.GetBuildFile() ?? ""))
                .Replace("  ", " ")
                .Replace(" \r\n ", "\r\n")
                .ToString()
                .Trim();

                // BUG: will crash with IndexOutOfBoundsException if imported data has empty powersets array or less than 2 items
                var txt2 = new StringBuilder();
                label2.Text = txt2.AppendJoin(' ',
                        "Reference:",
                        CompareData?.Metadata.Name?.Trim(),
                        string.IsNullOrWhiteSpace(CompareData?.Metadata.Name)
                                ? CompareData?.Metadata.PvMode == "PvP"
                                    ? "PvP"
                                    : "PvE"
                                : $"({(CompareData?.Metadata.PvMode == "PvP" ? "PvP" : "PvE")})",
                        "\r\n",
                        !string.IsNullOrWhiteSpace(CompareData?.Metadata.Powersets[0]) & !string.IsNullOrWhiteSpace(CompareData?.Metadata.Powersets[1])
                            ? $"{CompareData?.Metadata.Powersets[0]}/{CompareData?.Metadata.Powersets[1]}"
                            : "",
                        CompareData?.Metadata.Archetype,
                        "\r\n",
                        Path.GetFileName(CompareData?.Metadata.BuildFile ?? ""))
                .Replace("  ", " ")
                .Replace(" \r\n ", "\r\n")
                .ToString()
                .Trim();
            }
            else
            {
                label1.Text = "";
                label2.Text = "";
            }

            label1.Visible = CompareMode;
            label2.Visible = CompareMode;

            var setSelectedIndex = cbSet.SelectedIndex;
            var valuesSelectedIndex = cbValues.SelectedIndex;
            var styleSelectedIndex = cbStyle.SelectedIndex;

            FillComboBoxes(false);
            if (!CompareMode & ((int)StatDisplayed > MaxDisplayModeNoCompare))
            {
                StatDisplayed = DisplayMode.Damage; // 1
                cbValues.SelectedIndex = (int)StatDisplayed;
                // cbValues_SelectedIndexChanged(cbValues, EventArgs.Empty); // ??
            }
            else
            {
                cbSet.SelectedIndex = setSelectedIndex;
            }

            cbValues.SelectedIndex = valuesSelectedIndex;
            cbStyle.SelectedIndex = styleSelectedIndex;

            SizeElements();
        }

        private void SetGraphColors()
        {
            var graphColors = StatDisplayed switch
            {
                DisplayMode.Defense => new GraphColors
                {
                    BarBaseColor = Color.FromArgb(175, 0, 175),
                    BarEnhColor = Color.Magenta,
                    BarOvercapColor = Color.Magenta,
                    NegativeBaseColor = Color.FromArgb(124, 0, 32),
                    NegativeEnhColor = Color.FromArgb(175, 0, 45),
                    NegativeOvercapColor = Color.FromArgb(175, 0, 45),
                    FadeEndColor = Color.Purple,
                },

                DisplayMode.Resistance => new GraphColors
                {
                    BarBaseColor = Color.FromArgb(0, 160, 160),
                    BarEnhColor = Color.FromArgb(0, 192, 192),
                    BarOvercapColor = Color.FromArgb(255, 128, 128),
                    NegativeBaseColor = Color.FromArgb(160, 105, 0),
                    NegativeEnhColor = Color.FromArgb(191, 128, 0),
                    NegativeOvercapColor = Color.FromArgb(98, 65, 0),
                    FadeEndColor = Color.LightSeaGreen
                },

                DisplayMode.HealthEndurance => new GraphColors
                {
                    BarBaseColor = Color.FromArgb(37, 159, 128),
                    BarEnhColor = Color.FromArgb(42, 199, 160),
                    BarOvercapColor = Color.FromArgb(20, 87, 70),
                    NegativeBaseColor = Color.FromArgb(68, 97, 90),
                    NegativeEnhColor = Color.FromArgb(94, 138, 127),
                    NegativeOvercapColor = Color.FromArgb(25, 36, 33),
                    FadeEndColor = Color.FromArgb(150, 251, 150)
                },

                DisplayMode.MovementStealth => new GraphColors
                {
                    BarBaseColor = Color.FromArgb(42, 117, 101),
                    BarEnhColor = Color.FromArgb(53, 156, 132),
                    BarOvercapColor = Color.FromArgb(23, 50, 46),
                    NegativeBaseColor = Color.FromArgb(60, 68, 77),
                    NegativeEnhColor = Color.FromArgb(82, 93, 105),
                    NegativeOvercapColor = Color.FromArgb(36, 41, 46),
                    FadeEndColor = Color.FromArgb(0, 127, 95)
                },

                DisplayMode.MiscBuffs => new GraphColors
                {
                    BarBaseColor = Color.FromArgb(113, 86, 168),
                    BarEnhColor = Color.MediumPurple,
                    BarOvercapColor = Color.FromArgb(41, 31, 61),
                    NegativeBaseColor = Color.FromArgb(102, 76, 102),
                    NegativeEnhColor = Color.FromArgb(153, 115, 152),
                    NegativeOvercapColor = Color.FromArgb(43, 33, 43),
                    FadeEndColor = Color.FromArgb(72, 61, 137)
                },

                DisplayMode.StatusProtection => new GraphColors
                {
                    BarBaseColor = Color.FromArgb(175, 88, 0),
                    BarEnhColor = Color.FromArgb(255, 128, 0),
                    BarOvercapColor = Color.FromArgb(255, 128, 0),
                    NegativeBaseColor = Color.FromArgb(154, 0, 62),
                    NegativeEnhColor = Color.FromArgb(234, 0, 94),
                    NegativeOvercapColor = Color.FromArgb(117, 0, 47),
                    FadeEndColor = Color.FromArgb(127, 64, 0)
                },

                DisplayMode.StatusResistance => new GraphColors
                {
                    BarBaseColor = Color.FromArgb(175, 175, 0),
                    BarEnhColor = Color.Yellow,
                    BarOvercapColor = Color.FromArgb(60, 60, 0),
                    NegativeBaseColor = Color.FromArgb(99, 99, 40),
                    NegativeEnhColor = Color.FromArgb(178, 179, 71),
                    NegativeOvercapColor = Color.FromArgb(43, 43, 17),
                    FadeEndColor = Color.FromArgb(127, 127, 0)
                },

                DisplayMode.DebuffResistance => new GraphColors
                {
                    BarBaseColor = Color.FromArgb(0, 190, 190),
                    BarEnhColor = Color.Cyan,
                    BarOvercapColor = Color.FromArgb(0, 90, 127),
                    NegativeBaseColor = Color.FromArgb(73, 107, 140),
                    NegativeEnhColor = Color.FromArgb(99, 145, 191),
                    NegativeOvercapColor = Color.FromArgb(25, 37, 48),
                    FadeEndColor = Color.FromArgb(0, 127, 127)
                },

                DisplayMode.Elusivity => new GraphColors
                {
                    BarBaseColor = Color.FromArgb(163, 1, 231),
                    BarEnhColor = Color.FromArgb(163, 1, 231),
                    BarOvercapColor = Color.FromArgb(163, 1, 231),
                    NegativeBaseColor = Color.FromArgb(83, 62, 234),
                    NegativeEnhColor = Color.FromArgb(83, 62, 234),
                    NegativeOvercapColor = Color.FromArgb(83, 62, 234),
                    FadeEndColor = Color.FromArgb(141, 2, 200)
                },

                _ => new GraphColors
                {
                    BarBaseColor = Color.Blue,
                    BarEnhColor = Color.Yellow,
                    BarOvercapColor = Color.Cyan,
                    FadeEndColor = StatDisplayed switch
                    {
                        DisplayMode.Accuracy => Color.FromArgb(192, 192, 0),
                        DisplayMode.EndUse or DisplayMode.EndPerSec => Color.FromArgb(192, 192, 255),
                        DisplayMode.Healing or DisplayMode.HPS or DisplayMode.HPE => Color.FromArgb(96, 255, 96),
                        DisplayMode.EffectDuration => Color.FromArgb(128, 0, 255),
                        DisplayMode.Range => Color.FromArgb(64, 128, 96),
                        DisplayMode.RechargeTime => Color.FromArgb(255, 192, 128),
                        DisplayMode.Regeneration => Color.FromArgb(96, 192, 96),
                        _ => Color.DarkRed
                    }
                }
            };

            lblKeyColor1.BackColor = graphColors.BarBaseColor;
            lblKeyColor2.BackColor = graphColors.BarEnhColor;

            Graph.ColorFadeEnd = graphColors.FadeEndColor;
            Graph.ColorBase = graphColors.BarBaseColor;
            Graph.ColorEnh = graphColors.BarEnhColor;
            Graph.ColorOvercap = graphColors.BarOvercapColor;

            if (CompareGraph.Visible)
            {
                CompareGraph.ColorFadeEnd = graphColors.FadeEndColor;
                CompareGraph.ColorBase = graphColors.BarBaseColor;
                CompareGraph.ColorEnh = graphColors.BarEnhColor;
                CompareGraph.ColorOvercap = graphColors.BarOvercapColor;
            }

            if (graphColors.NegativeBaseColor != null)
            {
                Graph.NegativeBaseColor = graphColors.NegativeBaseColor.Value;
                if (CompareGraph.Visible)
                {
                    CompareGraph.NegativeBaseColor = graphColors.NegativeBaseColor.Value;
                }
            }

            if (graphColors.NegativeEnhColor != null)
            {
                Graph.NegativeEnhColor = graphColors.NegativeEnhColor.Value;
                if (CompareGraph.Visible)
                {
                    CompareGraph.NegativeEnhColor = graphColors.NegativeEnhColor.Value;
                }
            }

            if (graphColors.NegativeOvercapColor == null)
            {
                return;
            }

            Graph.NegativeOvercapColor = graphColors.NegativeOvercapColor.Value;
            if (CompareGraph.Visible)
            {
                CompareGraph.NegativeOvercapColor = graphColors.NegativeOvercapColor.Value;
            }
        }

        private void SetupGraph(bool setType = false)
        {
            if (setType)
            {
                SetGraphType();
            }

            Graph.BeginUpdate();
            if ((int)StatDisplayed > MaxDisplayModeNoCompare)
            {
                CompareGraph.BeginUpdate();
            }

            if (cbValues.SelectedIndex > -1)
            {
                SetGraphColors();
            }

            if ((int)StatDisplayed <= MaxDisplayModeNoCompare)
            {
                var pwStats = StatsPowerData.GetPowerStatsArray(cbSet.SelectedIndex, StatDisplayed);
                var graphValues = StatsPowerData.PreparePowersGraph(pwStats[0], pwStats[1], BaseOverride, Graph.Style, StatDisplayed);

                Graph.Clear();
                foreach (var val in graphValues)
                {
                    var displayName = (val.Power?.DisplayName != val.PowerName) & (val.Power?.FullName != val.PowerName)
                        ? val.PowerName
                        : val.Power?.DisplayName;
                    Graph.AddItem(displayName, val.BaseValue, val.EnhValue ?? val.BaseValue, val.Tip);
                }

                Graph.Max = graphValues.Max(e => Math.Max(e.BaseValue, e.UncappedValue ?? e.EnhValue ?? e.BaseValue)) * 1.025f;
            }
            else
            {
                Totals = StatsPowerData.GetTotals();

                Graph.Clear();
                var valuesGroup = StatDisplayed switch
                {
                    DisplayMode.Resistance => Totals.Resistance,
                    DisplayMode.HealthEndurance => Totals.Health.Concat(Totals.Endurance).ToArray(),
                    DisplayMode.MovementStealth => Totals.Movement.Concat(Totals.Stealth).ToArray(),
                    DisplayMode.MiscBuffs => Totals.MiscBuffs,
                    DisplayMode.StatusProtection => Totals.StatusProtection,
                    DisplayMode.StatusResistance => Totals.StatusResistance,
                    DisplayMode.DebuffResistance => Totals.DebuffResistance,
                    DisplayMode.Elusivity => Totals.Elusivity,
                    _ => Totals.Defense
                };

                foreach (var val in valuesGroup)
                {
                    Graph.AddItem(val.DisplayName, val.BaseValue, val.EnhValue ?? val.BaseValue, val.UncappedValue ?? val.EnhValue ?? val.BaseValue, val.Tip);
                }

                Graph.Max = valuesGroup.Max(e => Math.Max(e.BaseValue, e.UncappedValue ?? e.EnhValue ?? e.BaseValue)) * 1.025f;
            }

            tbScaleX.Value = Graph.ScaleIndex;
            SetGraphMetrics(Graph);
            Graph.EndUpdate();

            if (!CompareGraph.Visible)
            {
                Graph.Draw();
                return;
            }

            CompareGraph.BarsAlignment = CompareMode & (CompareGraphMode == CompareGraphStyle.RawValues)
                ? CtlMultiGraph.BarAlignment.Left
                : (int)StatDisplayed > MaxDisplayModeNoCompare
                    ? CtlMultiGraph.BarAlignment.Center
                    : CtlMultiGraph.BarAlignment.Left;

            SetCompareGraphValues(StatDisplayed, false);

            // Sync max range for power stats
            if (((int)StatDisplayed <= MaxDisplayModeNoCompare) | (CompareMode & (CompareGraphMode == CompareGraphStyle.RawValues)))
            {
                var m1 = Graph.Max;
                var m2 = CompareGraph.Max;
                var m = Math.Max(m1, m2);

                Graph.Max = m;
                CompareGraph.Max = m;
            }

            tbScaleX.Value = Graph.ScaleIndex;

            SetScaleLabel();

            Graph.Draw();
            CompareGraph.Draw();
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

            SetupGraph();
        }

        private void cbStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded)
            {
                return;
            }

            SetupGraph(true);
        }

        private void cbValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded)
            {
                return;
            }

            StatDisplayed = (DisplayMode)cbValues.SelectedIndex;
            cbCompareGraphStyle.Visible = CompareMode & (StatDisplayed >= DisplayMode.Defense);
            SetupGraph();
        }

        private void chkOnTop_Click(object? sender, EventArgs e)
        {
            TopMost = chkOnTop.ToggleState switch
            {
                ImageButtonEx.States.ToggledOn => true,
                _ => false
            };
        }

        private void FillComboBoxes(bool setSelection = true)
        {
            NoDraw = true;
            NewSets();
            cbValues.BeginUpdate();
            cbValues.Items.Clear();
            if (!CompareMode)
            {
                cbValues.Items.AddRange([
                    "Accuracy", "Damage", "Damage / Anim", "Damage / Sec", "Damage / End", "End Use", "End / Sec",
                    "Healing", "Heal / Sec", "Heal / End", "Effect Duration", "Range", "Recharge Time", "Regeneration"
                ]);
            }
            else if (CompareData?.Metadata.PvMode == "PvP")
            {
                cbValues.Items.AddRange([
                    "Accuracy", "Damage", "Damage / Anim", "Damage / Sec", "Damage / End", "End Use", "End / Sec",
                    "Healing", "Heal / Sec", "Heal / End", "Effect Duration", "Range", "Recharge Time", "Regeneration",
                    "Defense", "Resistance", "Health & Endurance", "Movement & Stealth", "Misc. Buffs", "Status Protection", "Status Resistance", "Debuff Resistance", "Elusivity"
                ]);
            }
            else
            {
                cbValues.Items.AddRange([
                    "Accuracy", "Damage", "Damage / Anim", "Damage / Sec", "Damage / End", "End Use", "End / Sec",
                    "Healing", "Heal / Sec", "Heal / End", "Effect Duration", "Range", "Recharge Time", "Regeneration",
                    "Defense", "Resistance", "Health & Endurance", "Movement & Stealth", "Misc. Buffs", "Status Protection", "Status Resistance", "Debuff Resistance"
                ]);
            }

            if (setSelection)
            {
                cbValues.SelectedIndex = 1;
            }

            cbValues.EndUpdate();

            cbStyle.BeginUpdate();
            cbStyle.Items.Clear();
            cbStyle.Items.AddRange([
                "Base & Enhanced", "Stacked Base + Enhanced", "Base Only", "Enhanced Only",
                "Active & Alternate", "Stacked Active + Alt"
            ]);

            if (MidsContext.Config.StatGraphStyle > (Enums.GraphStyle)(cbStyle.Items.Count - 1))
            {
                MidsContext.Config.StatGraphStyle = Enums.GraphStyle.Stacked;
            }

            if (setSelection)
            {
                cbStyle.SelectedIndex = (int)MidsContext.Config.StatGraphStyle;
            }

            cbStyle.EndUpdate();

            cbCompareGraphStyle.SelectedIndex = 0;
            CompareGraphMode = CompareGraphStyle.Diff;
            NoDraw = false;
        }

        private void frmStats_FormClosed(object sender, FormClosedEventArgs e)
        {
            myParent.FloatStatGraph(false);
        }

        private void frmStats_Load(object sender, EventArgs e)
        {
            SetUiForCompare(false);
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
            SetTitle(this);
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
            SizeElements();
        }

        private string DiffTip(StatsPowerData.PowerValueInfo item, StatsPowerData.PowerValueInfoExt[]? gValues, string refTip)
        {
            if (gValues == null || gValues.All(e => e.PowerName != item.PowerName))
            {
                return refTip;
            }

            var currentData = gValues.First(e => e.PowerName == item.PowerName);
            if (currentData.EnhValue != null && item.EnhValue != null)
            {
                if (Math.Abs((currentData.EnhValue ?? 0) - (item.EnhValue ?? 0)) < float.Epsilon)
                {
                    return refTip;
                }
            }

            return Graph.Style == Enums.GraphStyle.baseOnly
                ? $"Reference:\r\n{refTip}\r\n(Also in current, power is {(!currentData.PowerTaken ? "not " : "")} taken)"
                : (currentData.EnhValue == null) | (item.EnhValue == null)
                    ? refTip
                    : $"Current: {currentData.BaseValue:####0.##}{currentData.UnitSuffix} | {currentData.EnhValue:####0.##}{currentData.UnitSuffix} ({(!currentData.PowerTaken ? "not " : "")}taken)\r\n{currentData.Tip}\r\n\r\nReference: {item.BaseValue:####0.##}{item.UnitSuffix} | {item.EnhValue:####0.##}{item.UnitSuffix} ({(!item.PowerTaken ? "not " : "")}taken)\r\n{item.Tip}{(Math.Abs(item.EnhValue ?? 0) < float.Epsilon ? "" : $"\r\n\r\nDiff: {(currentData.EnhValue > item.EnhValue ? "+" : "")}{currentData.EnhValue / item.EnhValue * 100 - 100:####0.##}%")}";
        }

        private void SetCompareGraphValues(DisplayMode statDisplayed, bool draw = true)
        {
            CompareGraph.BeginUpdate();
            CompareGraph.Clear();
            CompareGraph.Style = Graph.Style;

            if ((int)statDisplayed > MaxDisplayModeNoCompare)
            {
                var statGroup = statDisplayed switch
                {
                    DisplayMode.Resistance => Totals.Resistance,
                    DisplayMode.HealthEndurance => Totals.Health.Concat(Totals.Endurance).ToArray(),
                    DisplayMode.MovementStealth => Totals.Movement.Concat(Totals.Stealth).ToArray(),
                    DisplayMode.MiscBuffs => Totals.MiscBuffs,
                    DisplayMode.StatusProtection => Totals.StatusProtection,
                    DisplayMode.StatusResistance => Totals.StatusResistance,
                    DisplayMode.DebuffResistance => Totals.DebuffResistance,
                    DisplayMode.Elusivity => Totals.Elusivity,
                    _ => Totals.Defense
                };

                var statGroupAux = statDisplayed switch
                {
                    DisplayMode.Resistance => CompareData?.Totals.Resistance ?? [],
                    DisplayMode.HealthEndurance => (CompareData?.Totals.Health ?? []).Concat(CompareData?.Totals.Endurance ?? []).ToArray(),
                    DisplayMode.MovementStealth => (CompareData?.Totals.Movement ?? []).Concat(CompareData?.Totals.Stealth ?? []).ToArray(),
                    DisplayMode.MiscBuffs => CompareData?.Totals.MiscBuffs ?? [],
                    DisplayMode.StatusProtection => CompareData?.Totals.StatusProtection ?? [],
                    DisplayMode.StatusResistance => CompareData?.Totals.StatusResistance ?? [],
                    DisplayMode.DebuffResistance => CompareData?.Totals.DebuffResistance ?? [],
                    DisplayMode.Elusivity => CompareData?.Totals.Elusivity ?? [],
                    _ => CompareData?.Totals.Defense ?? []
                };

                string[] statLabels = statDisplayed switch
                {
                    DisplayMode.Resistance => ["Smashing", "Lethal", "Fire", "Cold", "Energy", "Negative", "Toxic", "Psionic"],
                    DisplayMode.HealthEndurance => ["Regeneration", "Max HP", "Absorb", "End Rec", "End Use", "Max End"],
                    DisplayMode.MovementStealth => ["Run Speed", "Jump Speed", "Jump Height", "Fly Speed", "Stealth PvE", "Stealth PvP", "Perception"],
                    DisplayMode.MiscBuffs => ["Haste", "ToHit", "Accuracy", "Damage", "Range", "EndRdx", "Threat"],
                    DisplayMode.StatusProtection or DisplayMode.StatusResistance => ["Held", "Stunned", "Sleep", "Immobilized", "Knockback", "Repel", "Confused", "Terrorized", "Taunt", "Placate", "Teleport"],
                    DisplayMode.DebuffResistance => ["Defense", "Endurance", "Recovery", "PerceptionRadius", "ToHit", "RechargeTime", "SpeedRunning", "Regeneration"],
                    DisplayMode.Elusivity => ["Untyped", "Smashing", "Lethal", "Fire", "Cold", "Energy", "Negative", "Toxic", "Psionic", "Melee", "Ranged", "AoE"],
                    DisplayMode.Defense => ["Smashing", "Lethal", "Fire", "Cold", "Energy", "Negative", "Toxic", "Psionic", "Melee", "Ranged", "AoE"],
                };

                for (var index = 0; index < statGroup.Length; index++)
                {
                    var nBaseMain = statGroup[index].BaseValue;
                    var nEnhMain = statGroup[index].EnhValue ?? statGroup[index].BaseValue;
                    //var nUncappedMain = statGroup[index].UncappedValue ?? statGroup[index].EnhValue ?? statGroup[index].BaseValue;

                    var nBaseRef = statGroupAux[index].BaseValue;
                    var nEnhRef = statGroupAux[index].EnhValue ?? statGroupAux[index].BaseValue;
                    var nUncappedRef = statGroupAux[index].UncappedValue ?? statGroupAux[index].EnhValue ?? statGroupAux[index].BaseValue;

                    // Convert to currently selected speed/distance units
                    if (statDisplayed == DisplayMode.MovementStealth)
                    {
                        nBaseRef = ConvertSpeedValue(nBaseRef, CompareData?.Metadata.SpeedFormat ?? MidsContext.Config.SpeedFormat);
                        nEnhRef = ConvertSpeedValue(nEnhRef, CompareData?.Metadata.SpeedFormat ?? MidsContext.Config.SpeedFormat);
                        nUncappedRef = ConvertSpeedValue(nUncappedRef, CompareData?.Metadata.SpeedFormat ?? MidsContext.Config.SpeedFormat);
                    }

                    if (!MidsContext.Config.Inc.DisablePvE & (StatDisplayed == DisplayMode.Elusivity))
                    {
                        nBaseRef = 0;
                        nEnhRef = 0;
                    }

                    var nBaseDiff = nBaseMain - nBaseRef;
                    var nEnhDiff = nEnhMain - nEnhRef;
                    //var nUncappedDiff = nUncappedMain - nUncappedRef;

                    var displayName = statLabels[index];
                    var tip = GetCompareGraphTip(statDisplayed, index, displayName, nBaseMain, nEnhMain, nBaseRef, nEnhRef, nBaseDiff, nEnhDiff);

                    if (CompareGraphMode == CompareGraphStyle.Diff)
                    {
                        CompareGraph.AddItem(displayName, nBaseDiff, nEnhDiff, tip);
                    }
                    else
                    {
                        CompareGraph.AddItem(displayName, nBaseRef, nEnhRef, nUncappedRef, tip);
                    }
                }

                if (CompareGraphMode == CompareGraphStyle.Diff)
                {
                    var diff = 0f;
                    for (var i = 0; i < statGroup.Length; i++)
                    {
                        var diff1 = Math.Abs(statGroup[i].BaseValue - statGroupAux[i].BaseValue);
                        var diff2 = Math.Abs((statGroup[i].UncappedValue ?? statGroup[i].EnhValue ?? statGroup[i].BaseValue) - (statGroupAux[i].UncappedValue ?? statGroupAux[i].EnhValue ?? statGroupAux[i].BaseValue));

                        diff = Math.Max(diff, Math.Max(diff1, diff2));
                    }

                    CompareGraph.Max = diff * 1.025f;
                }
                else
                {
                    CompareGraph.Max = statGroupAux.Max(e => Math.Max(e.BaseValue, e.UncappedValue ?? e.EnhValue ?? e.BaseValue)) * 1.025f;
                }
            }
            else
            {
                var pwStats = StatsPowerData.GetPowerStatsArray(cbSet.SelectedIndex, StatDisplayed);
                var graphValues = StatsPowerData.PreparePowersGraph(pwStats[0], pwStats[1], BaseOverride, Graph.Style, StatDisplayed);

                var statGroupAux = CompareData?.GetGroupData(statDisplayed)
                    .Select(e => new StatsPowerData.PowerValueInfoExt
                    {
                        PowerName = e.PowerName,
                        Power = DatabaseAPI.GetPowerByFullName(e.PowerName), // Simplify to e.Power ?
                        PowerTaken = e.PowerTaken,
                        BaseValue = e.BaseValue,
                        EnhValue = e.EnhValue,
                        UncappedValue = e.UncappedValue,
                        Stacks = e.Stacks,
                        UnitSuffix = e.UnitSuffix,
                        Tip = DiffTip(e, graphValues, e.Tip)
                    })
                    .ToList();

                // Apply power type filter
                statGroupAux = cbSet.SelectedIndex switch
                {
                    // Primary/Secondary
                    1 => statGroupAux
                        .Where(e => (e.Power?.FullName.StartsWith("Redirects.") == true) | e.Power?.GetPowerSet()?.SetType is Enums.ePowerSetType.Primary or Enums.ePowerSetType.Secondary)
                        .ToList(),

                    // Primary
                    2 => statGroupAux
                        .Where(e => (e.Power?.FullName.StartsWith("Redirects.") == true) | (e.Power?.GetPowerSet()?.SetType == Enums.ePowerSetType.Primary))
                        .ToList(),

                    // Secondary
                    3 => statGroupAux
                        .Where(e => (e.Power?.FullName.StartsWith("Redirects.") == true) | (e.Power?.GetPowerSet()?.SetType == Enums.ePowerSetType.Secondary))
                        .ToList(),

                    // Epic/Ancillary
                    4 => statGroupAux
                        .Where(e => (e.Power?.FullName.StartsWith("Redirects.") == true) | (e.Power?.GetPowerSet()?.SetType == Enums.ePowerSetType.Ancillary))
                        .ToList(),

                    // Pools
                    5 => statGroupAux
                        .Where(e => (e.Power?.FullName.StartsWith("Redirects.") == true) | (e.Power?.GetPowerSet()?.SetType == Enums.ePowerSetType.Pool))
                        .ToList(),

                    // Powers taken
                    6 => statGroupAux
                        .Where(e => e.PowerTaken)
                        .ToList(),

                    // All toggles
                    7 => statGroupAux
                        .Where(e => e.Power is { PowerType: Enums.ePowerType.Toggle })
                        .ToList(),

                    // All clicks
                    8 => statGroupAux
                        .Where(e => e.Power is { PowerType: Enums.ePowerType.Click })
                        .ToList(),

                    _ => statGroupAux
                };

                for (var index = 0; index < statGroupAux?.Count; index++)
                {
                    var nBaseRef = statGroupAux[index].BaseValue;
                    var nEnhRef = statGroupAux[index].EnhValue ?? statGroupAux[index].BaseValue;
                    //var nUncappedRef = statGroupAux[index].UncappedValue ?? statGroupAux[index].EnhValue ?? statGroupAux[index].BaseValue;

                    var displayName = statGroupAux[index].Power?.DisplayName ?? "";
                    var tip = GetCompareGraphTip(statDisplayed, statGroupAux[index].Power?.DisplayName ?? "",
                        statGroupAux[index].BaseValue, statGroupAux[index].EnhValue ?? statGroupAux[index].BaseValue,
                        statGroupAux[index].Power, statGroupAux[index].Stacks, statGroupAux[index].Tip);

                    CompareGraph.AddItem(displayName, nBaseRef, nEnhRef, tip);
                }

                CompareGraph.Max = (statGroupAux?.Max(e => Math.Max(e.BaseValue, e.UncappedValue ?? e.EnhValue ?? e.BaseValue)) ?? 75) * 1.025f;
            }

            SetGraphMetrics(CompareGraph);
            CompareGraph.EndUpdate();

            if (!draw)
            {
                return;
            }

            CompareGraph.Draw();
        }

        private string GetCompareGraphTip(DisplayMode statDisplayed, int subIndex, string displayName, float nBaseMain, float nEnhMain, float nBaseRef, float nEnhRef, float nBaseDiff, float nEnhDiff)
        {
            var tip = "";
            var speedUnit = MidsContext.Config?.SpeedFormat switch
            {
                Enums.eSpeedMeasure.KilometersPerHour => "km/h",
                Enums.eSpeedMeasure.FeetPerSecond => "ft/s",
                Enums.eSpeedMeasure.MetersPerSecond => "m/s",
                _ => "mph"
            };

            var distanceUnit = MidsContext.Config?.SpeedFormat switch
            {
                Enums.eSpeedMeasure.KilometersPerHour or Enums.eSpeedMeasure.MetersPerSecond => "m",
                _ => "ft"
            };

            switch (statDisplayed)
            {
                case DisplayMode.Defense:
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName} Defense:\r\nCurrent: {nBaseMain:##0.#}%\r\nReference: {nBaseRef:##0.#}%\r\n\r\nDiff: {(nBaseDiff > 0 ? "+" : "")}{nBaseDiff:##0.#}%"
                        : $"{displayName} Defense:\r\nCurrent: {nEnhMain:##0.#}%\r\nReference: {nEnhRef:##0.#}%\r\n\r\nDiff: {(nEnhDiff > 0 ? "+" : "")}{nEnhDiff:##0.#}%";
                    break;

                case DisplayMode.Resistance:
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName} Resistance:\r\nCurrent: {nBaseMain:##0.#}%\r\nReference: {nBaseRef:##0.#}%\r\n\r\nDiff: {(nBaseDiff > 0 ? "+" : "")}{nBaseDiff:##0.#}%"
                        : $"{displayName} Resistance:\r\nCurrent: {nEnhMain:##0.#}%\r\nReference: {nEnhRef:##0.#}%\r\n\r\nDiff: {(nEnhDiff > 0 ? "+" : "")}{nEnhDiff:##0.#}%";
                    break;

                case DisplayMode.HealthEndurance when subIndex == 0: // Regeneration
                    var maxHpMain = Totals.Health[1].EnhValue ?? Totals.Health[1].BaseValue;
                    var baseRegenMain = maxHpMain / 12f * (0.05f + 0.05f * ((nBaseMain - 100) / 100f));
                    var baseRegenPercentMain = baseRegenMain / maxHpMain * 100f;
                    var enhRegenMain = maxHpMain / 12f * (0.05f + 0.05f * ((nEnhMain - 100) / 100f));
                    var enhRegenPercentMain = enhRegenMain / maxHpMain * 100;

                    var maxHpRef = CompareData?.Totals?.Health[1].EnhValue ?? CompareData?.Totals?.Health[1].BaseValue ?? CompareData?.Totals?.Health[1].BaseValue;
                    var baseRegenRef = maxHpRef / 12f * (0.05f + 0.05f * ((nBaseRef - 100) / 100f));
                    var baseRegenPercentRef = baseRegenRef / maxHpRef * 100f;
                    var enhRegenRef = maxHpRef / 12f * (0.05f + 0.05f * ((nEnhRef - 100) / 100f));
                    var enhRegenPercentRef = enhRegenRef / maxHpRef * 100;

                    var baseRegenDiff = baseRegenMain - baseRegenRef;
                    var baseRegenPercentDiff = baseRegenPercentMain - baseRegenPercentRef;
                    var enhRegenDiff = enhRegenMain - enhRegenRef;
                    var enhRegenPercentDiff = enhRegenPercentMain - enhRegenPercentRef;

                    if (BaseOverride)
                    {
                        (baseRegenMain, enhRegenMain) = (enhRegenMain, baseRegenMain);
                        (baseRegenPercentMain, enhRegenPercentMain) = (enhRegenPercentMain, baseRegenPercentMain);

                        (baseRegenRef, enhRegenRef) = (enhRegenRef, baseRegenRef);
                        (baseRegenPercentRef, enhRegenPercentRef) = (enhRegenPercentRef, baseRegenPercentRef);
                    }

                    if ((Graph.Style == Enums.GraphStyle.baseOnly) | (Math.Abs(nBaseMain - nEnhMain) < float.Epsilon))
                    {
                        tip = $"Regeneration:\r\nCurrent:\r\nHealth regenerated per second: {baseRegenPercentMain:##0.##}%\r\n Hit Points regenerated per second at level 50: {baseRegenMain:##0.#} HP\r\n\r\nReference:\r\nHealth regenerated per second: {baseRegenPercentRef:##0.##}%\r\n Hit Points regenerated per second at level 50: {baseRegenRef:##0.#} HP\r\n\r\nDiff: {(baseRegenPercentDiff > 0 ? "+" : "")}{baseRegenPercentDiff:##0.##}% ({(baseRegenDiff > 0 ? "+" : "")}{baseRegenDiff:##0.#} HP/s)";
                    }
                    /*else if (Math.Abs(nBaseMain - nEnhMain) < float.Epsilon)
                    {
                        tip = $"{displayName}: {nBaseMain:##0.#}%\r\n Health regenerated per second: {baseRegenPercentMain:##0.##}%\r\n Hit Points regenerated per second at level 50: {baseRegenMain:##0.#} HP";
                    }*/
                    else
                    {
                        tip = $"Regeneration:\r\nCurrent:\r\nHealth regenerated per second: {enhRegenPercentMain:##0.##}%\r\n Hit Points regenerated per second at level 50: {enhRegenMain:##0.#} HP\r\n\r\nReference:\r\nHealth regenerated per second: {enhRegenPercentRef:##0.##}%\r\n Hit Points regenerated per second at level 50: {enhRegenRef:##0.#} HP\r\n\r\nDiff: {(enhRegenPercentDiff > 0 ? "+" : "")}{enhRegenPercentDiff:##0.##}% ({(enhRegenDiff > 0 ? "+" : "")}{enhRegenDiff:##0.#} HP/s)";
                    }

                    break;

                case DisplayMode.HealthEndurance when subIndex == 1: // Max HP
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName}:\r\nCurrent: {nBaseMain:###0.#}\r\nReference: {nBaseRef:###0.#}\r\n\r\nDiff: {(nBaseDiff > 0 ? "+" : "")}{nBaseDiff:###0.#}"
                        : $"{displayName}:\r\nCurrent: {nEnhMain:###0.#}\r\nReference: {nEnhRef:###0.#}\r\n\r\nDiff: {(nEnhDiff > 0 ? "+" : "")}{nEnhDiff:###0.#}";
                    break;

                case DisplayMode.HealthEndurance when subIndex == 2: // Absorb
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName}:\r\nCurrent: {nBaseMain:###0.#}\r\nReference: {nBaseRef:###0.#}\r\n\r\nDiff: {(nBaseDiff > 0 ? "+" : "")}{nBaseDiff:###0.#}"
                        : $"{displayName}:\r\nCurrent: {nEnhMain:###0.#}\r\nReference: {nEnhRef:###0.#}\r\n\r\nDiff: {(nEnhDiff > 0 ? "+" : "")}{nEnhDiff:###0.#}";
                    break;

                case DisplayMode.HealthEndurance when subIndex == 3: // End Rec
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName}:\r\nCurrent: {nBaseMain:##0.##}/s\r\nReference: {nBaseRef:##0.##}/s\r\n\r\nDiff: {(nBaseDiff > 0 ? "+" : "")}{nBaseDiff:##0.#}/s"
                        : $"{displayName}:\r\nCurrent: {nEnhMain:##0.##}/s\r\nReference: {nEnhRef:##0.##}/s\r\n\r\nDiff: {(nEnhDiff > 0 ? "+" : "")}{nEnhDiff:##0.#}/s";
                    break;

                case DisplayMode.HealthEndurance when subIndex == 4: // End Use
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName}:\r\nCurrent: {nBaseMain:##0.##}/s\r\nReference: {nBaseRef:##0.##}/s\r\n\r\nDiff: {(nBaseDiff > 0 ? "+" : "")}{nBaseDiff:##0.##}/s"
                        : $"{displayName}:\r\nCurrent: {nEnhMain:##0.##}/s\r\nReference: {nEnhRef:##0.##}/s\r\n\r\nDiff: {(nEnhDiff > 0 ? "+" : "")}{nEnhDiff:##0.##}/s";
                    break;

                case DisplayMode.HealthEndurance when subIndex == 5: // Max End
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName}:\r\nCurrent: {nBaseMain:##0.##}\r\nReference: {nBaseRef:##0.##}\r\n\r\nDiff: {(nBaseDiff > 0 ? "+" : "")}{nBaseDiff:##0.##}"
                        : $"{displayName}:\r\nCurrent: {nEnhMain:##0.##}\r\nReference: {nEnhRef:##0.##}\r\n\r\nDiff: {(nEnhDiff > 0 ? "+" : "")}{nEnhDiff:##0.##}";
                    break;

                case DisplayMode.MovementStealth when subIndex is < 2 or 3: // Run Speed, Jump Speed, Fly Speed
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName}:\r\nCurrent: {nBaseMain:##0.##} mph\r\nReference: {nBaseRef:##0.##} mph\r\n\r\nDiff: {(nBaseDiff > 0 ? "+" : "")}{nBaseDiff:##0.##} {speedUnit}"
                        : $"{displayName}:\r\nCurrent: {nEnhMain:##0.##} mph\r\nReference: {nEnhRef:##0.##} mph\r\n\r\nDiff: {(nEnhDiff > 0 ? "+" : "")}{nEnhDiff:##0.##} {speedUnit}";
                    break;

                case DisplayMode.MovementStealth when subIndex is 2 or > 3: // Jump height, Stealth/Perception
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName}:\r\nCurrent: {nBaseMain:##0.##} ft\r\nReference: {nBaseRef:##0.##} ft\r\n\r\nDiff: {(nBaseDiff > 0 ? "+" : "")}{nBaseDiff:##0.##} {distanceUnit}"
                        : $"{displayName}:\r\nCurrent: {nEnhMain:##0.##} ft\r\nReference: {nEnhRef:##0.##} ft\r\n\r\nDiff: {(nEnhDiff > 0 ? "+" : "")}{nEnhDiff:##0.##} {distanceUnit}";
                    break;

                case DisplayMode.MiscBuffs when subIndex == 6: // Threat
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName}:\r\nCurrent: {nBaseMain:###0.#}\r\nReference: {nBaseRef:###0.#}\r\n\r\nDiff: {(nBaseDiff > 0 ? "+" : "")}{nBaseDiff:###0.#}"
                        : $"{displayName}:\r\nCurrent: {nEnhMain:###0.#}\r\nReference: {nEnhRef:###0.#}\r\n\r\nDiff: {(nEnhDiff > 0 ? "+" : "")}{nEnhDiff:###0.#}";
                    break;

                case DisplayMode.MiscBuffs:
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName}:\r\nCurrent: {nBaseMain:###0.##}%\r\nReference: {nBaseRef:###0.##}%\r\n\r\nDiff: {(nBaseDiff > 0 ? "+" : "")}{nBaseDiff:###0.##}%"
                        : $"{displayName}:\r\nCurrent: {nEnhMain:###0.##}%\r\nReference: {nEnhRef:###0.##}%\r\n\r\nDiff: {(nEnhDiff > 0 ? "+" : "")}{nEnhDiff:###0.##}%";
                    break;

                case DisplayMode.StatusProtection:
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName} Protection:\r\nCurrent: {nBaseMain:###0.##}\r\nReference: {nBaseRef:###0.##}\r\n\r\nDiff: {(nBaseDiff > 0 ? "+" : "")}{nBaseDiff:###0.##}"
                        : $"{displayName} Protection:\r\nCurrent: {nEnhMain:###0.##}\r\nReference: {nEnhRef:###0.##}\r\n\r\nDiff: {(nEnhDiff > 0 ? "+" : "")}{nEnhDiff:###0.##}";
                    break;

                case DisplayMode.StatusResistance:
                case DisplayMode.DebuffResistance:
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"Resistance to {displayName}:\r\nCurrent: {nBaseMain:###0.##}%\r\nReference: {nBaseRef:###0.##}%\r\n\r\nDiff: {(nBaseDiff > 0 ? "+" : "")}{nBaseDiff:###0.##}%"
                        : $"Resistance to {displayName}:\r\nCurrent: {nEnhMain:###0.##}%\r\nReference: {nEnhRef:###0.##}%\r\n\r\nDiff: {(nEnhDiff > 0 ? "+" : "")}{nEnhDiff:###0.##}%";
                    break;

                case DisplayMode.Elusivity:
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"Elusivity ({displayName}):\r\nCurrent: {nBaseMain:####0.##}%\r\nReference: {nBaseRef:####0.##}%\r\n\r\nDiff: {(nBaseDiff > 0 ? "+" : "")}{nBaseDiff:####0.##}%"
                        : $"Elusivity ({displayName}):\r\nCurrent: {nEnhMain:####0.##}%\r\nReference: {nEnhRef:####0.##}%\r\n\r\nDiff: {(nEnhDiff > 0 ? "+" : "")}{nEnhDiff:####0.##}%";
                    break;
            }

            return tip.Trim();
        }

        private string GetCompareGraphTip(DisplayMode statDisplayed, string displayName, float nBaseRef, float nEnhRef, IPower? power, int? stacks, string powerTip)
        {
            var tip = "";
            var baseHealValue = nBaseRef / CompareData?.Totals?.Health[1].BaseValue * 100;
            var enhHealValue = nEnhRef / CompareData?.Totals?.Health[1].EnhValue * 100;
            var stacksTip = $"{(stacks == null ? "" : $" (at {stacks} stack{(stacks == 1 ? "" : "s")})")}";

            switch (statDisplayed)
            {
                case DisplayMode.Accuracy:
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName} Accuracy:\r\nReference: {nBaseRef:##0.#}%{stacksTip}"
                        : $"{displayName} Accuracy:\r\nReference: {nEnhRef:##0.#}%{stacksTip}";

                    break;

                case DisplayMode.Damage:
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName} Damage:\r\nReference: {nBaseRef:###0.##}{stacksTip}"
                        : !BaseOverride
                            ? $"{displayName} Damage:\r\nReference: {nEnhRef:###0.##}{stacksTip}"
                            : $"{displayName} Damage:\r\nReference: {nBaseRef:###0.##}{stacksTip}";

                    if (Math.Abs(nBaseRef - nEnhRef) > float.Epsilon)
                    {
                        tip += $" ({nBaseRef:###0.##})";
                    }

                    if (power?.PowerType == Enums.ePowerType.Toggle)
                    {
                        tip += $"\r\n(Applied every {power?.ActivatePeriod}s)";
                    }

                    if (!string.IsNullOrWhiteSpace(powerTip))
                    {
                        tip += $"\r\n\r\n{powerTip}";
                    }

                    break;

                case DisplayMode.DPA:
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName} DPA:\r\nReference: {nBaseRef:###0.##}{stacksTip}"
                        : !BaseOverride
                            ? $"{displayName} DPA:\r\nReference: {nEnhRef:###0.##}{stacksTip}"
                            : $"{displayName} DPA:\r\nReference: {nBaseRef:###0.##}{stacksTip}";

                    tip += "/s";
                    if (Math.Abs(nBaseRef - nEnhRef) > float.Epsilon)
                    {
                        tip += $" ({nBaseRef:###0.##}/s)";
                    }

                    break;

                case DisplayMode.DPS:
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName} DPS:\r\nReference: {nBaseRef:###0.##}{stacksTip}"
                        : !BaseOverride
                            ? $"{displayName} DPS:\r\nReference: {nEnhRef:###0.##}{stacksTip}"
                            : $"{displayName} DPS:\r\nReference: {nBaseRef:###0.##}{stacksTip}";

                    tip += "/s";
                    if (Math.Abs(nBaseRef - nEnhRef) > float.Epsilon)
                    {
                        tip += $" ({nBaseRef:###0.##}/s)";
                    }

                    break;

                case DisplayMode.DPE:
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName} DPE:\r\nReference: {nBaseRef:###0.##}{stacksTip}"
                        : !BaseOverride
                            ? $"{displayName} DPE:\r\nReference: {nEnhRef:###0.##}{stacksTip}"
                            : $"{displayName} DPE:\r\nReference: {nBaseRef:###0.##}{stacksTip}";

                    if (Graph.Style == Enums.GraphStyle.baseOnly)
                    {
                        tip += $"\r\nDamage per unit of End: {nBaseRef:###0.##}";
                    }
                    else
                    {
                        tip += $"\r\nDamage per unit of End: {nEnhRef:###0.##}";
                        if (Math.Abs(nBaseRef - nEnhRef) > float.Epsilon)
                        {
                            tip += $" ({nBaseRef:###0.##})";
                        }
                    }

                    break;

                case DisplayMode.EffectDuration:
                    var durationEffectId = power?.GetDurationEffectID() ?? -1;
                    if (durationEffectId > -1)
                    {
                        var str = power.Effects[durationEffectId].EffectType != Enums.eEffectType.Mez
                            ? Enums.GetEffectName(power.Effects[durationEffectId].EffectType)
                            : Enums.GetMezName((Enums.eMezShort)power.Effects[durationEffectId].MezType);
                        if (power.Effects[durationEffectId].Mag < 0)
                        {
                            str = $"-{str}";
                        }

                        tip = Graph.Style == Enums.GraphStyle.baseOnly
                            ? $"{displayName} ({str}): {nBaseRef:##0.#}s{stacksTip}"
                            : $"{displayName} ({str}): {nEnhRef:##0.#}s{stacksTip}";

                        if (Math.Abs(nBaseRef - nEnhRef) > float.Epsilon)
                        {
                            tip += $" ({nBaseRef:##0.#}s)";
                        }
                    }

                    break;

                case DisplayMode.EndUse:
                    tip = Graph.Style != Enums.GraphStyle.baseOnly
                        ? $"{displayName}: {nEnhRef:##0.##}{stacksTip}"
                        : $"{displayName}: {nBaseRef:##0.##}{stacksTip}";

                    if (Math.Abs(nBaseRef - nEnhRef) > float.Epsilon)
                    {
                        tip += $" ({nBaseRef:##0.##})";
                    }

                    if (power?.PowerType == Enums.ePowerType.Toggle)
                    {
                        tip += "\r\n(Per Second)";
                    }

                    break;

                case DisplayMode.EndPerSec:
                    tip = Graph.Style != Enums.GraphStyle.baseOnly
                        ? $"{displayName}: {nEnhRef:##0.##}/s{stacksTip}"
                        : $"{displayName}: {nBaseRef:##0.##}/s{stacksTip}";

                    if (Math.Abs(nBaseRef - nEnhRef) > float.Epsilon)
                    {
                        tip += $" ({nBaseRef:##0.##})";
                    }

                    break;

                case DisplayMode.Healing:
                case DisplayMode.Regeneration:
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName}: {baseHealValue:###0.#}% ({nBaseRef:###0.#} HP){stacksTip}"
                        : $"{displayName}\r\n Enhanced: {enhHealValue:###0.#}% ({nEnhRef:###0.#} HP){stacksTip}";

                    if (Math.Abs(nBaseRef - nEnhRef) > float.Epsilon)
                    {
                        tip += $"\r\n Base: {baseHealValue:##0.#}% ({nBaseRef:##0.#} HP)";
                    }

                    break;

                case DisplayMode.HPE:
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName}: {nBaseRef:##0.##}%{stacksTip}"
                        : $"{displayName}\r\n Enhanced Heal per unit of End: {enhHealValue:##0.##}% ({nEnhRef:##0.##} HP){stacksTip}";

                    if (Math.Abs(nBaseRef - nEnhRef) > float.Epsilon)
                    {
                        tip += $"\r\n Base Heal per unit of End: {baseHealValue:##0.##}% ({nBaseRef:##0.##} HP)";
                    }

                    break;

                case DisplayMode.HPS:
                    tip = Graph.Style == Enums.GraphStyle.baseOnly
                        ? $"{displayName}: {baseHealValue:###0.##}%/s ({nBaseRef:###0.##} HP/s){stacksTip}"
                        : $"{displayName}\r\n Enhanced: {enhHealValue:###0.##}%/s ({nEnhRef:###0.##} HP/s){stacksTip}";

                    if (Math.Abs(nBaseRef - nEnhRef) > float.Epsilon)
                    {
                        tip += $"\r\n Base: {baseHealValue:###0.#}%/s ({nBaseRef:###0.##} HP/s)";
                    }

                    break;

                case DisplayMode.Range:
                    tip = Graph.Style != Enums.GraphStyle.baseOnly
                        ? $"{displayName} Range:\r\nReference: {nEnhRef:###0.#} ft{stacksTip}"
                        : $"{displayName} Range:\r\nReference: {nBaseRef:###0.#} ft{stacksTip}";

                    if (Math.Abs(nBaseRef - nEnhRef) > float.Epsilon)
                    {
                        tip += $" ({nBaseRef:###0.#} ft)";
                    }

                    break;

                case DisplayMode.RechargeTime:
                    tip = Graph.Style != Enums.GraphStyle.baseOnly
                        ? $"{displayName}: {nEnhRef:###0.##}s{stacksTip}"
                        : $"{displayName}: {nBaseRef:###0.##}s{stacksTip}";

                    if (Math.Abs(nBaseRef - nEnhRef) > float.Epsilon)
                    {
                        tip += $" ({nBaseRef:###0.##}s)";
                    }

                    break;
            }

            return tip.Trim();
        }

        [DebuggerStepThrough]
        private void NewSets()
        {
            cbSet.BeginUpdate();
            cbSet.Items.Clear();
            cbSet.Items.AddRange(
            [
                "All Sets",
                "Primary & Secondary",
                $"Primary ({MidsContext.Character.Powersets[0].DisplayName})",
                $"Secondary ({MidsContext.Character.Powersets[1].DisplayName})",
                "Ancillary",
                "Pools",
                "Powers Taken",
                "All Toggles",
                "All Clicks"
            ]);
            cbSet.SelectedIndex = 1;
            cbSet.EndUpdate();
        }

        private void SetGraphMetrics(CtlMultiGraph graph)
        {
            switch (graph.ItemCount)
            {
                case <= 13:
                    graph.ItemHeight = 18;
                    graph.PaddingY = 6f;
                    break;

                case < 18:
                    graph.ItemHeight = 15;
                    graph.PaddingY = 5f;
                    break;

                case > 32:
                    graph.ItemHeight = 10;
                    graph.PaddingY = 2f;
                    break;

                case > 30:
                    graph.ItemHeight = 11;
                    graph.PaddingY = 2f;
                    break;

                case > 27:
                    graph.ItemHeight = 11;
                    graph.PaddingY = 2.666667f;
                    break;

                default:
                    graph.ItemHeight = 12;
                    graph.PaddingY = 4f;
                    break;
            }
        }

        private void SetGraphType()
        {
            if ((cbStyle.SelectedIndex > -1) & (cbStyle.SelectedIndex < cbStyle.Items.Count - 2))
            {
                Graph.Style = (Enums.GraphStyle)cbStyle.SelectedIndex;
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
            var storedCompareSize = MainModule.MidsController.SzFrmStatsCompare == null
                ? new Rectangle(0, 0, 1004, 601)
                : MainModule.MidsController.SzFrmStatsCompare.Value;

            var storedSize = MainModule.MidsController.SzFrmStats == null
                ? new Rectangle(0, 0, 492, 515)
                : MainModule.MidsController.SzFrmStats.Value;

            var rectangle = CompareMode
                ? storedCompareSize
                : storedSize;

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
                rectangle.X = (int)Math.Round((Screen.PrimaryScreen.Bounds.Width - Width) / 2f);
            }

            if (rectangle.Y < 32)
            {
                rectangle.Y = (int)Math.Round((Screen.PrimaryScreen.Bounds.Height - Height) / 2f);
            }

            Left = rectangle.X;
            Top = rectangle.Y;
            //Width = rectangle.Width;
            //Height = rectangle.Height;
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

            if (!CompareMode)
            {
                MainModule.MidsController.SzFrmStats = new Rectangle(Left, Top, Width, Height);
            }
            else
            {
                MainModule.MidsController.SzFrmStatsCompare = new Rectangle(Left, Top, Width, Height);
            }

        }

        private void tbScaleX_Scroll(object sender, EventArgs e)
        {
            Graph.ScaleIndex = tbScaleX.Value;
            Graph.Draw();
            SetScaleLabel();
        }

        public void UpdateData(bool newData)
        {
            StatDisplayed = (DisplayMode)cbValues.SelectedIndex;
            BackColor = myParent.BackColor;
            Graph.BackColor = BackColor;
            if (newData)
            {
                NewSets();
            }

            SetupGraph(true);
        }

        private float ConvertSpeedValue(float val, Enums.eSpeedMeasure unit)
        {
            var targetUnit = MidsContext.Config.SpeedFormat;
            var xIndex = (int)unit;
            var yIndex = (int)targetUnit;

            return val * SpeedCnvMatrix[yIndex][xIndex];
        }

        private bool ExportToJson(string file)
        {
            var buildFile = myParent.GetBuildFile();
            buildFile = buildFile == null ? null : Path.GetFileName(buildFile);

            var s = new StatsPowerData();

            // Set up metadata
            // Character name, build file, archetype, powersets, speed format
            s.SetMetadata(MidsContext.Character.Name,
                buildFile,
                MidsContext.Character.Archetype.DisplayName,
                MidsContext.Character.Powersets.Where((e, i) => i != 2).Select(e => e == null ? "" : e.DisplayName).ToArray(), // Dummy powerset on index 2, to be skipped
                MidsContext.Config.SpeedFormat);

            foreach (var k in Enum.GetValues<DisplayMode>())
            {
                if ((int)k > MaxDisplayModeNoCompare)
                {
                    break;
                }

                var pwGroupData = StatsPowerData.GetPowerStatsArray(0, k);
                if (pwGroupData == null)
                {
                    return false;
                }

                var groupStats = new List<StatsPowerData.PowerValueInfo>();

                // Reverse lookup power.FullName => index
                var enhPowerIndex = pwGroupData[1]
                    .Select((e, i) => new KeyValuePair<int, IPower?>(i, e))
                    .Where(e => e.Value != null)
                    .ToDictionary(e => e.Value!.FullName, e => e.Key);

                // Get group values from display mode
                var pwValues = StatsPowerData.PreparePowersGraph(pwGroupData[0], pwGroupData[1], false, Enums.GraphStyle.Twin, k);
                if (pwValues == null)
                {
                    return false;
                }

                for (var i = 0; i < pwValues.Length; i++)
                {
                    var j = pwGroupData[0].TryFindIndex(e => e?.FullName == pwValues[i].Power?.FullName);
                    if (j < 0)
                    {
                        continue;
                    }

                    var pe = MidsContext.Character?.CurrentBuild?.Powers
                        .DefaultIfEmpty(null)
                        .FirstOrDefault(e => e is { Power: not null } && e.Power.FullName == pwGroupData[0][j]?.FullName);

                    groupStats.Add(new StatsPowerData.PowerValueInfo
                    {
                        PowerName = pwGroupData[0][j]?.FullName ?? "",
                        PowerTaken = pwGroupData[0][j]?.FullName != null && pe != null,
                        BaseValue = pwValues[i].BaseValue,
                        EnhValue = enhPowerIndex.ContainsKey(pwGroupData[0][j]?.FullName ?? "") ? pwValues[i].EnhValue : null,
                        UncappedValue = enhPowerIndex.ContainsKey(pwGroupData[0][j]?.FullName ?? "") ? pwValues[i].UncappedValue : null,
                        Stacks = pe != null && pwGroupData[0][j]?.VariableEnabled == true ? pe.Power?.Stacks : null,
                        UnitSuffix = pwValues[i].UnitSuffix,
                        Tip = pwValues[i].Tip
                    });
                }

                s.AddGroup(k, groupStats.ToArray());
            }

            // Append total stats
            s.SetTotals();

            // BUG: If this breaks, extra data are added to graph (need to be tested again)
            File.WriteAllText(file, s.ExportToJson());

            return true;
        }

        private bool ImportFromJson(string file)
        {
            if (!File.Exists(file))
            {
                return false;
            }

            var cnt = "";
            try
            {
                cnt = File.ReadAllText(file);
            }
            catch (Exception)
            {
                return false;
            }

            CompareData = StatsPowerData.ImportFromJson(cnt);
            if (CompareData == null)
            {
                return false;
            }

            /*if (CompareData.Metadata.Archetype != MidsContext.Character?.Archetype?.DisplayName)
            {
                var m = new MessageBoxEx("Cannot compare powers across different archetypes.", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error, true);
                m.ShowDialog(this);

                return false;
            }*/

            /*var buildPowersets = MidsContext.Character.Powersets
                .Where(e => e != null)
                .Select(e => e.DisplayName)
                .ToArray();
            var comparePowersets = CompareData.Metadata.Powersets;

            if (buildPowersets.Length < 2 ||
                comparePowersets.Length < 2 ||
                !string.Equals(buildPowersets[0], comparePowersets[0], StringComparison.InvariantCultureIgnoreCase) ||
                !string.Equals(buildPowersets[1], comparePowersets[1], StringComparison.InvariantCultureIgnoreCase))
            {
                var m = new MessageBoxEx("Cannot compare: primary/secondary powersets don't match.", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error, true);
                m.ShowDialog(this);

                return false;
            }*/

            // Very basic validation: Archetype has to be set, Powersets has to have 7+ items (even if powers from pools are not taken)
            if (string.IsNullOrWhiteSpace(CompareData.Metadata.Archetype))
            {
                var msg = new MessageBoxEx("Import Compare Data", "Cannot import data from file: missing archetype.",
                    MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error, true);
                msg.ShowDialog();

                return false;
            }

            if (CompareData.Metadata.Powersets.Length < 7)
            {
                var msg = new MessageBoxEx("Import Compare Data",
                    "Cannot import data from file: too few powersets (expecting at least 7 entries).",
                    MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error, true);
                msg.ShowDialog();

                return false;
            }

            CompareMode = true;

            return true;
        }

        private void TsCompareImport_Click(object sender, EventArgs e)
        {
            using var dlgOpen = new OpenFileDialog();
            dlgOpen.Filter = @"Compare Data|*.json";
            dlgOpen.InitialDirectory = MidsContext.Config.BuildsPath;
            dlgOpen.Multiselect = false;
            dlgOpen.CheckFileExists = true;

            var ret = dlgOpen.ShowDialog();
            if (ret != DialogResult.OK)
            {
                return;
            }

            var importStatus = ImportFromJson(dlgOpen.FileName);
            if (!importStatus)
            {
                return;
            }

            SetUiForCompare();
        }

        private void TsCompareExport_Click(object sender, EventArgs e)
        {
            using var dlgSave = new SaveFileDialog();
            dlgSave.Filter = @"Compare Data|*.json";
            dlgSave.InitialDirectory = MidsContext.Config.BuildsPath;

            var buildFile = myParent.GetBuildFile();
            buildFile = Path.GetFileName(buildFile ?? "");

            string saveFile;
            if (string.IsNullOrEmpty(buildFile))
            {
                saveFile = !string.IsNullOrWhiteSpace(MidsContext.Character.Name)
                    ? $"{MidsContext.Character.Name} - {MidsContext.Character.Archetype.DisplayName} ({MidsContext.Character.Powersets[0].DisplayName} - {MidsContext.Character.Powersets[1].DisplayName})"
                    : $"{MidsContext.Character.Archetype.DisplayName} ({MidsContext.Character.Powersets[0].DisplayName} - {MidsContext.Character.Powersets[1].DisplayName})";
            }
            else
            {
                var fileInfo = new FileInfo(buildFile);
                saveFile = fileInfo.Name.Replace(fileInfo.Extension, "");
            }

            dlgSave.FileName = $"[Compare] {saveFile}.json";

            var ret = dlgSave.ShowDialog();
            if (ret != DialogResult.OK)
            {
                return;
            }

            var exportStatus = ExportToJson(dlgSave.FileName);
            if (exportStatus)
            {
                return;
            }

            var msgBox = new MessageBoxEx("Export compare data", $"Failed to export to {dlgSave.FileName}.",
                MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error, true);
            msgBox.ShowDialog(this);
        }

        private void TsEndCompare_Click(object sender, EventArgs e)
        {
            if (!CompareMode)
            {
                return;
            }

            CompareMode = false;
            SetUiForCompare();
        }

        private void cbCompareGraphStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoDraw)
            {
                return;
            }

            CompareGraphMode = cbCompareGraphStyle.SelectedIndex == 0
                ? CompareGraphStyle.Diff
                : CompareGraphStyle.RawValues;
            SetupGraph();
        }

        private void ibExport_Click(object sender, EventArgs e)
        {
            if (CompareData == null)
            {
                return;
            }

            using var dlgSave = new SaveFileDialog();
            dlgSave.Filter = @"Raw Compare Data|*.csv";
            dlgSave.InitialDirectory = MidsContext.Config.BuildsPath;
            dlgSave.FileName = "compare_data.csv";

            var ret = dlgSave.ShowDialog(this);
            if (ret != DialogResult.OK)
            {
                return;
            }

            File.WriteAllText(dlgSave.FileName, ExportCsv());
        }

        private string ExportCsv()
        {
            dynamic[] headers =
            [
                "Notes",
                "Name",
                "Archetype",
                "PvMode",
                "Build File",
                "Primary Powerset",
                "Secondary Powerset",
                "Pool Powerset 1",
                "Pool Powerset 2",
                "Pool Powerset 3",
                "Pool Powerset 4",
                "Epic Powerset",
                "Speed Format",
                "",
                "Power Name",
                "Power Taken (current)",
                "Power Taken (ref)",
                "Stacks (current)",
                "Stacks (ref)",
                "Stat Name",
                "Base Value",
                "Enh. Value",
                "Uncapped Value",
                "Ref. Base Value",
                "Ref. Enh. Value",
                "Ref. Uncapped Value",
                "Diff. (base)",
                "Diff. (enh)",
                "Unit",
                "Details",
                "",
                "Stat Name",
                "Base Value",
                "Enh. Value",
                "Uncapped Value",
                "Ref. Base Value",
                "Ref. Enh. Value",
                "Ref. Uncapped Value",
                "Diff. (base)",
                "Diff. (enh.)",
                "Unit",
                "Details"
            ];

            Totals = StatsPowerData.GetTotals();

            var data = new List<dynamic[]>
            {
                headers,
                new dynamic[] {
                    $"Compare Data Record\r\n{MidsContext.AppName} v{MidsContext.AssemblyVersion} rev. {MidsContext.AppFileVersion.Revision}"
                }.Pad("", headers.Length),
                    
                new dynamic[] {
                    MidsContext.Character.Name.Trim(),
                    MidsContext.Character.Archetype.DisplayName,
                    MidsContext.Config.Inc.DisablePvE ? "PvP" : "PvE",
                    Path.GetFileName(myParent.GetBuildFile() ?? ""),
                    MidsContext.Character.Powersets[0]?.DisplayName ?? "",
                    MidsContext.Character.Powersets[1]?.DisplayName ?? "",
                    MidsContext.Character.Powersets[3]?.DisplayName ?? "",
                    MidsContext.Character.Powersets[4]?.DisplayName ?? "",
                    MidsContext.Character.Powersets[5]?.DisplayName ?? "",
                    MidsContext.Character.Powersets[6]?.DisplayName ?? "",
                    MidsContext.Character.Powersets[7]?.DisplayName ?? "",
                    MidsContext.Config.SpeedFormat
                }.Pad("", headers.Length, 1),
                    
                new dynamic[]
                {
                    CompareData?.Metadata.Name?.Trim() ?? "",
                    CompareData?.Metadata.Archetype ?? "",
                    CompareData?.Metadata.PvMode == "PvP" ? "PvP" : "PvE",
                    Path.GetFileName(CompareData?.Metadata.BuildFile ?? ""),
                    CompareData?.Metadata.Powersets[0] ?? "",
                    CompareData?.Metadata.Powersets[1] ?? "",
                    CompareData?.Metadata.Powersets[2] ?? "",
                    CompareData?.Metadata.Powersets[3] ?? "",
                    CompareData?.Metadata.Powersets[4] ?? "",
                    CompareData?.Metadata.Powersets[5] ?? "",
                    CompareData?.Metadata.Powersets[6] ?? "",
                    CompareData?.Metadata.SpeedFormat ?? MidsContext.Config.SpeedFormat
                }.Pad("", headers.Length, 1)
            };

            foreach (var s in Enum.GetValues<DisplayMode>())
            {
                if ((int)s <= MaxDisplayModeNoCompare)
                {
                    //Current Data
                    var pwStats = StatsPowerData.GetPowerStatsArray(cbSet.SelectedIndex, StatDisplayed);
                    var statGroup = StatsPowerData.PreparePowersGraph(pwStats[0], pwStats[1], BaseOverride, Graph.Style, StatDisplayed);

                    // Reference data
                    var statGroupAux = CompareData?.GetGroupData(s)
                        .Select(e => new StatsPowerData.PowerValueInfoExt
                        {
                            PowerName = e.PowerName,
                            Power = DatabaseAPI.GetPowerByFullName(e.PowerName),
                            PowerTaken = e.PowerTaken,
                            BaseValue = e.BaseValue,
                            EnhValue = e.EnhValue,
                            UncappedValue = e.UncappedValue,
                            Stacks = e.Stacks,
                            UnitSuffix = e.UnitSuffix,
                            Tip = DiffTip(e, statGroup, e.Tip)
                        })
                        .ToList();

                    // Apply power type filter
                    statGroupAux = cbSet.SelectedIndex switch
                    {
                        // Primary/Secondary
                        1 => statGroupAux
                            .Where(e => (e.Power?.FullName.StartsWith("Redirects.") == true) | e.Power?.GetPowerSet()?.SetType is Enums.ePowerSetType.Primary or Enums.ePowerSetType.Secondary)
                            .ToList(),

                        // Primary
                        2 => statGroupAux
                            .Where(e => (e.Power?.FullName.StartsWith("Redirects.") == true) | (e.Power?.GetPowerSet()?.SetType == Enums.ePowerSetType.Primary))
                            .ToList(),

                        // Secondary
                        3 => statGroupAux
                            .Where(e => (e.Power?.FullName.StartsWith("Redirects.") == true) | (e.Power?.GetPowerSet()?.SetType == Enums.ePowerSetType.Secondary))
                            .ToList(),

                        // Epic/Ancillary
                        4 => statGroupAux
                            .Where(e => (e.Power?.FullName.StartsWith("Redirects.") == true) | (e.Power?.GetPowerSet()?.SetType == Enums.ePowerSetType.Ancillary))
                            .ToList(),

                        // Pools
                        5 => statGroupAux
                            .Where(e => (e.Power?.FullName.StartsWith("Redirects.") == true) | (e.Power?.GetPowerSet()?.SetType == Enums.ePowerSetType.Pool))
                            .ToList(),

                        // Powers taken
                        6 => statGroupAux
                            .Where(e => e.PowerTaken)
                            .ToList(),

                        // All toggles
                        7 => statGroupAux
                            .Where(e => e.Power is { PowerType: Enums.ePowerType.Toggle })
                            .ToList(),

                        // All clicks
                        8 => statGroupAux
                            .Where(e => e.Power is { PowerType: Enums.ePowerType.Click })
                            .ToList(),

                        _ => statGroupAux
                    };

                    for (var index = 0; index < statGroupAux?.Count; index++)
                    {
                        var nBaseRef = statGroupAux[index].BaseValue;
                        var nEnhRef = statGroupAux[index].EnhValue ?? statGroupAux[index].BaseValue;
                        var nUncappedRef = statGroupAux[index].UncappedValue ?? statGroupAux[index].EnhValue ?? statGroupAux[index].BaseValue;

                        var tip = GetCompareGraphTip(s, statGroupAux[index].Power?.DisplayName ?? "",
                            statGroupAux[index].BaseValue, statGroupAux[index].EnhValue ?? statGroupAux[index].BaseValue,
                            statGroupAux[index].Power, statGroupAux[index].Stacks, statGroupAux[index].Tip);

                        statGroupAux[index] = new StatsPowerData.PowerValueInfoExt
                        {
                            BaseValue = nBaseRef,
                            EnhValue = nEnhRef,
                            UncappedValue = nUncappedRef,
                            Power = statGroupAux[index].Power,
                            PowerName = statGroupAux[index].PowerName,
                            PowerTaken = statGroupAux[index].PowerTaken,
                            Stacks = statGroupAux[index].Stacks,
                            UnitSuffix = statGroupAux[index].UnitSuffix,
                            Tip = tip
                        };
                    }

                    for (var index = 0; index < Math.Min(statGroup?.Length ?? 0, statGroupAux?.Count ?? 0); index++)
                    {
                        var displayName = (statGroup[index].Power?.DisplayName != statGroup[index].PowerName) & (statGroup[index].Power?.FullName != statGroup[index].PowerName)
                            ? statGroup[index].PowerName
                            : statGroup[index].Power?.DisplayName;

                        data.Add(new dynamic[]
                        {
                            displayName ?? "--",
                            statGroup[index].PowerTaken ? "Yes" : "No",
                            statGroupAux[index].PowerTaken ? "Yes" : "No",
                            statGroup[index].Stacks ?? 1,
                            statGroupAux[index].Stacks ?? 1,
                            s.ToString(),
                            statGroup[index].BaseValue,
                            statGroup[index].EnhValue ?? statGroup[index].BaseValue,
                            statGroup[index].UncappedValue ?? statGroup[index].EnhValue ?? statGroup[index].BaseValue,
                            statGroupAux[index].BaseValue,
                            statGroupAux[index].EnhValue ?? statGroupAux[index].BaseValue,
                            statGroupAux[index].UncappedValue ?? statGroupAux[index].EnhValue ?? statGroupAux[index].BaseValue,
                            statGroup[index].BaseValue - statGroupAux[index].BaseValue,
                            (statGroup[index].EnhValue ?? statGroup[index].BaseValue) - (statGroupAux[index].EnhValue ?? statGroupAux[index].BaseValue),
                            statGroup[index].UnitSuffix,
                            statGroupAux[index].Tip
                        }.Pad("", headers.Length, 14));
                    }
                }
                else
                {
                    string[] statLabels = s switch
                    {
                        DisplayMode.Resistance => ["Smashing", "Lethal", "Fire", "Cold", "Energy", "Negative", "Toxic", "Psionic"],
                        DisplayMode.HealthEndurance => ["Regeneration", "Max HP", "Absorb", "End Rec", "End Use", "Max End"],
                        DisplayMode.MovementStealth => ["Run Speed", "Jump Speed", "Jump Height", "Fly Speed", "Stealth PvE", "Stealth PvP", "Perception"],
                        DisplayMode.MiscBuffs => ["Haste", "ToHit", "Accuracy", "Damage", "Range", "EndRdx", "Threat"],
                        DisplayMode.StatusProtection or DisplayMode.StatusResistance => ["Held", "Stunned", "Sleep", "Immobilized", "Knockback", "Repel", "Confused", "Terrorized", "Taunt", "Placate", "Teleport"],
                        DisplayMode.DebuffResistance => ["Defense", "Endurance", "Recovery", "PerceptionRadius", "ToHit", "RechargeTime", "SpeedRunning", "Regeneration"],
                        DisplayMode.Elusivity => ["Untyped", "Smashing", "Lethal", "Fire", "Cold", "Energy", "Negative", "Toxic", "Psionic", "Melee", "Ranged", "AoE"],
                        DisplayMode.Defense => ["Smashing", "Lethal", "Fire", "Cold", "Energy", "Negative", "Toxic", "Psionic", "Melee", "Ranged", "AoE"]
                    };

                    string statGroupLabel = s switch
                    {
                        DisplayMode.Defense => " Defense",
                        DisplayMode.Resistance or DisplayMode.StatusResistance => " Resistance",
                        DisplayMode.StatusProtection => " Protection",
                        DisplayMode.DebuffResistance => " Debuff Resistance",
                        DisplayMode.Elusivity => " Elusivity",
                        _ => ""
                    };

                    // Current data
                    var statGroup = s switch
                    {
                        DisplayMode.Resistance => Totals.Resistance,
                        DisplayMode.HealthEndurance => Totals.Health.Concat(Totals.Endurance).ToArray(),
                        DisplayMode.MovementStealth => Totals.Movement.Concat(Totals.Stealth).ToArray(),
                        DisplayMode.MiscBuffs => Totals.MiscBuffs,
                        DisplayMode.StatusProtection => Totals.StatusProtection,
                        DisplayMode.StatusResistance => Totals.StatusResistance,
                        DisplayMode.DebuffResistance => Totals.DebuffResistance,
                        DisplayMode.Elusivity => Totals.Elusivity,
                        _ => Totals.Defense
                    };

                    // Reference data
                    var statGroupAux = s switch
                    {
                        DisplayMode.Resistance => CompareData?.Totals?.Resistance ?? [],
                        DisplayMode.HealthEndurance => (CompareData?.Totals?.Health ?? []).Concat(CompareData?.Totals?.Endurance ?? []).ToArray(),
                        DisplayMode.MovementStealth => (CompareData?.Totals?.Movement ?? []).Concat(CompareData?.Totals?.Stealth ?? []).ToArray(),
                        DisplayMode.MiscBuffs => CompareData?.Totals?.MiscBuffs ?? [],
                        DisplayMode.StatusProtection => CompareData?.Totals?.StatusProtection ?? [],
                        DisplayMode.StatusResistance => CompareData?.Totals?.StatusResistance ?? [],
                        DisplayMode.DebuffResistance => CompareData?.Totals?.DebuffResistance ?? [],
                        DisplayMode.Elusivity => CompareData?.Totals?.Elusivity ?? [],
                        _ => CompareData?.Totals?.Defense ?? []
                    };

                    for (var index = 0; index < Math.Min(statGroup.Length, statGroupAux.Length); index++)
                    {
                        var nBaseMain = statGroup[index].BaseValue;
                        var nEnhMain = statGroup[index].EnhValue ?? statGroup[index].BaseValue;
                        var nUncappedMain = statGroup[index].UncappedValue ?? statGroup[index].EnhValue ?? statGroup[index].BaseValue;

                        var nBaseRef = statGroupAux[index].BaseValue;
                        var nEnhRef = statGroupAux[index].EnhValue ?? statGroupAux[index].BaseValue;
                        var nUncappedRef = statGroupAux[index].UncappedValue ?? statGroupAux[index].EnhValue ?? statGroupAux[index].BaseValue;

                        // Convert to currently selected speed/distance units
                        if (s == DisplayMode.MovementStealth)
                        {
                            nBaseRef = ConvertSpeedValue(nBaseRef, CompareData?.Metadata.SpeedFormat ?? MidsContext.Config.SpeedFormat);
                            nEnhRef = ConvertSpeedValue(nEnhRef, CompareData?.Metadata.SpeedFormat ?? MidsContext.Config.SpeedFormat);
                            nUncappedRef = ConvertSpeedValue(nUncappedRef, CompareData?.Metadata.SpeedFormat ?? MidsContext.Config.SpeedFormat);
                        }

                        if (!MidsContext.Config.Inc.DisablePvE & (StatDisplayed == DisplayMode.Elusivity))
                        {
                            nBaseRef = 0;
                            nEnhRef = 0;
                        }

                        var nBaseDiff = nBaseMain - nBaseRef;
                        var nEnhDiff = nEnhMain - nEnhRef;
                        //var nUncappedDiff = nUncappedMain - nUncappedRef;

                        var displayName = $"{statLabels[index]}{statGroupLabel}";
                        var speedUnit = MidsContext.Config.SpeedFormat switch
                        {
                            Enums.eSpeedMeasure.KilometersPerHour => "km/h",
                            Enums.eSpeedMeasure.FeetPerSecond => "ft/s",
                            Enums.eSpeedMeasure.MetersPerSecond => "m/s",
                            _ => "mph"
                        };

                        var distanceUnit = MidsContext.Config.SpeedFormat switch
                        {
                            Enums.eSpeedMeasure.KilometersPerHour or Enums.eSpeedMeasure.MetersPerSecond => "m",
                            _ => "ft"
                        };

                        var unitSuffix = s switch
                        {
                            DisplayMode.HealthEndurance when index <= 2 => " HP",
                            DisplayMode.HealthEndurance when index <= 4 => "/s",
                            DisplayMode.HealthEndurance => "",
                            DisplayMode.MovementStealth when index is < 2 or 3 => $" {speedUnit}",
                            DisplayMode.MovementStealth when index is 2 or > 3 => $" {distanceUnit}",
                            DisplayMode.MiscBuffs when index == 6 => "",
                            DisplayMode.StatusProtection => "",
                            _ => "%"
                        };

                        var tip = GetCompareGraphTip(s, index, displayName, nBaseMain, nEnhMain, nBaseRef, nEnhRef, nBaseDiff, nEnhDiff);

                        data.Add(new dynamic[]
                        {
                            displayName,
                            nBaseMain,
                            nEnhMain,
                            nUncappedMain,
                            nBaseRef,
                            nEnhRef,
                            nUncappedRef,
                            nBaseDiff,
                            nEnhDiff,
                            unitSuffix,
                            tip
                        }.Pad("", headers.Length, 31));
                    }
                }
            }

            return CSV.ExportCsv(data);
        }
    }
}