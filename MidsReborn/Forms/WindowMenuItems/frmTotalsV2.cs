﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms.Controls;
using MRBResourceLib;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmTotalsV2 : Form
    {
        private class TabColorScheme
        {
            public readonly Color HeroInactiveTabColor = Color.FromArgb(30, 85, 130);
            public readonly Color HeroInactiveHoveredTabColor = Color.FromArgb(43, 122, 187);
            public readonly Color HeroBorderColor = Color.Goldenrod;
            public readonly Color HeroActiveTabColor = Color.Goldenrod;

            public readonly Color VillainInactiveTabColor = Color.FromArgb(86, 12, 12);
            public readonly Color VillainInactiveHoveredTabColor = Color.FromArgb(143, 20, 20);
            public readonly Color VillainBorderColor = Color.FromArgb(184, 184, 187);
            public readonly Color VillainActiveTabColor = Color.FromArgb(184, 184, 187);
        }

        private readonly frmMain _myParent;
        private bool KeepOnTop { get; set; }
        private readonly TabColorScheme _tabColors = new();

        private readonly List<Enums.eMez> MezList = new()
        {
            Enums.eMez.Held, Enums.eMez.Stunned, Enums.eMez.Sleep, Enums.eMez.Immobilized,
            Enums.eMez.Knockback, Enums.eMez.Repel, Enums.eMez.Confused, Enums.eMez.Terrorized,
            Enums.eMez.Taunt, Enums.eMez.Placate, Enums.eMez.Teleport
        };

        private readonly List<Enums.eEffectType> DebuffEffectsList = new()
        {
            Enums.eEffectType.Defense, Enums.eEffectType.Endurance, Enums.eEffectType.Recovery,
            Enums.eEffectType.PerceptionRadius, Enums.eEffectType.ToHit, Enums.eEffectType.RechargeTime,
            Enums.eEffectType.SpeedRunning, Enums.eEffectType.Regeneration
        };

        public frmTotalsV2(ref frmMain parentForm)
        {
            try
            {
                _myParent = parentForm;
                FormClosed += frmTotalsV2_FormClosed;
                Load += frmTotalsV2_Load;

                KeepOnTop = true;
                InitializeComponent();
                Icon = Resources.MRB_Icon_Concept;
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private static string UcFirst(string s)
        {
            return char.ToUpper(s[0]) + s[1..].ToLower();
        }

        private string FormatVectorType(Type enumType, int vectorTypeIndex)
        {
            var name = UcFirst(Enum.GetName(enumType, vectorTypeIndex));
            var r = new Regex(@"([A-Z])");
            name = r.Replace(name, " " + "$1").TrimStart();

            return name switch
            {
                "Stealth Radius" => "Stealth Radius (PvE)",
                "Stealth Radius Player" => "Stealth Radius (PvP)",
                "Aoe" => "AoE",
                "Held" => "Hold",
                "Terrorized" => "Fear",
                _ => name
            };
        }

        private void Radio_CheckedChanged(object sender, EventArgs e)
        {
            var sendingControl = (RadioButton) sender;
            var radioControls = Controls.OfType<RadioButton>();
            if (!sendingControl.Checked) return;

            foreach (var radio in radioControls)
            {
                if (radio.Name != sendingControl.Name)
                {
                    radio.Checked = false;
                }
            }

            var previousCfgSpeedMeasure = MidsContext.Config.SpeedFormat;

            MidsContext.Config.SpeedFormat = sendingControl.Name switch
            {
                "radioButton1" => Enums.eSpeedMeasure.MilesPerHour,
                "radioButton2" => Enums.eSpeedMeasure.KilometersPerHour,
                "radioButton3" => Enums.eSpeedMeasure.FeetPerSecond,
                "radioButton4" => Enums.eSpeedMeasure.MetersPerSecond,
                _ => Enums.eSpeedMeasure.MilesPerHour
            };

            if (previousCfgSpeedMeasure != MidsContext.Config.SpeedFormat)
            {
                UpdateData();
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

        private void SetUnitRadioButtons()
        {
            radioButton1.Checked = MidsContext.Config.SpeedFormat == Enums.eSpeedMeasure.MilesPerHour;
            radioButton2.Checked = MidsContext.Config.SpeedFormat == Enums.eSpeedMeasure.KilometersPerHour;
            radioButton3.Checked = MidsContext.Config.SpeedFormat == Enums.eSpeedMeasure.FeetPerSecond;
            radioButton4.Checked = MidsContext.Config.SpeedFormat == Enums.eSpeedMeasure.MetersPerSecond;
        }

        public static void SetTitle(frmTotalsV2 frm)
        {
            if (frm == null) return;

            var titleTxt = "";
            var epicPowersetIndex = GetEpicPowersetIndex();
            var buildFileName = Path.GetFileName(frm._myParent.GetBuildFile());

            switch (MidsContext.Config.TotalsWindowTitleStyle)
            {
                case ConfigData.ETotalsWindowTitleStyle.CharNameAtPowersets:
                    titleTxt = $"{(!string.IsNullOrWhiteSpace(MidsContext.Character.Name) ? $"{MidsContext.Character.Name} - " : "")}{MidsContext.Character.Archetype.DisplayName}";
                    if (!MidsContext.Character.IsKheldian)
                    {
                        titleTxt += $" [ {MidsContext.Character.Powersets[0].DisplayName} / {MidsContext.Character.Powersets[1].DisplayName}{(epicPowersetIndex != -1 ? $" / {MidsContext.Character.Powersets[epicPowersetIndex].DisplayName}" : "")} ]";
                    }

                    frm.Text = $"Totals - {titleTxt}";
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

                    frm.Text = $"Totals - {titleTxt}";
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
                        titleTxt += !string.IsNullOrEmpty(buildFileName) ? $"[{buildFileName}]" : "";
                    }

                    frm.Text = titleTxt == "" ? "Totals for Self" : $"Totals - {titleTxt}";
                    break;

                case ConfigData.ETotalsWindowTitleStyle.Generic:
                default:
                    frm.Text = "Totals for Self";
                    break;
            }
        }

        private void frmTotalsV2_Load(object sender, EventArgs e)
        {
            MidsContext.Character.AlignmentChanged += CharacterOnAlignmentChanged;
            switch (MidsContext.Character.Alignment)
            {
                case Enums.Alignment.Hero:
                case Enums.Alignment.Vigilante:
                case Enums.Alignment.Resistance:

                    ctlTotalsTabStrip1.InactiveTabColor = _tabColors.HeroInactiveTabColor;
                    ctlTotalsTabStrip1.BackColor = _tabColors.HeroInactiveTabColor;
                    ctlTotalsTabStrip1.ActiveTabColor = _tabColors.HeroActiveTabColor;
                    ctlTotalsTabStrip1.StripLineColor = _tabColors.HeroBorderColor;
                    ctlTotalsTabStrip1.InactiveHoveredTabColor = _tabColors.HeroInactiveHoveredTabColor;
                    ibTopMost.UseAlt = false;
                    ibClose.UseAlt = false;

                    break;

                default:
                    ctlTotalsTabStrip1.InactiveTabColor = _tabColors.VillainInactiveTabColor;
                    ctlTotalsTabStrip1.BackColor = _tabColors.VillainInactiveTabColor;
                    ctlTotalsTabStrip1.ActiveTabColor = _tabColors.VillainActiveTabColor;
                    ctlTotalsTabStrip1.StripLineColor = _tabColors.VillainBorderColor;
                    ctlTotalsTabStrip1.InactiveHoveredTabColor = _tabColors.VillainInactiveHoveredTabColor;
                    ibTopMost.UseAlt = true;
                    ibClose.UseAlt = true;

                    break;
            }

            ibTopMost.ToggleState = TopMost ? ImageButtonEx.States.ToggledOn : ImageButtonEx.States.ToggledOff;
            MinimumSize = new Size(567, 771);
            Size = new Size(567, 771);
            CenterToParent();
            ctlTotalsTabStrip1.ClearItems();
            var headersText = new List<string>
            {
                "Core Stats",
                "Misc Buffs",
                "Status Protection",
                "Debuff Resistances"
            };

            foreach (var h in headersText)
            {
                ctlTotalsTabStrip1.AddItem(h);
            }

            ctlTotalsTabStrip1.Invalidate();

            //panel1.Size = new Size(561, 697);
            //panel2.Location = new Point(0, 684);
            //panel2.Size = Size with {Width = panel1.Width};
            // graph width: 526

            panelTab1.Location = new Point(0, ctlTotalsTabStrip1.Height);
            panelTab1.Visible = true;

            //Refresh();
            SetTitle(this);
            SetUnitRadioButtons();
            UpdateData();
        }

        private void CharacterOnAlignmentChanged(object? sender, Enums.Alignment e)
        {
            switch (e)
            {
                case Enums.Alignment.Hero:
                case Enums.Alignment.Vigilante:
                case Enums.Alignment.Resistance:

                    ctlTotalsTabStrip1.InactiveTabColor = _tabColors.HeroInactiveTabColor;
                    ctlTotalsTabStrip1.BackColor = _tabColors.HeroInactiveTabColor;
                    ctlTotalsTabStrip1.ActiveTabColor = _tabColors.HeroActiveTabColor;
                    ctlTotalsTabStrip1.StripLineColor = _tabColors.HeroBorderColor;
                    ctlTotalsTabStrip1.InactiveHoveredTabColor = _tabColors.HeroInactiveHoveredTabColor;
                    ibTopMost.UseAlt = false;
                    ibClose.UseAlt = false;
                    break;

                default:
                    ctlTotalsTabStrip1.InactiveTabColor = _tabColors.VillainInactiveTabColor;
                    ctlTotalsTabStrip1.BackColor = _tabColors.VillainInactiveTabColor;
                    ctlTotalsTabStrip1.ActiveTabColor = _tabColors.VillainActiveTabColor;
                    ctlTotalsTabStrip1.StripLineColor = _tabColors.VillainBorderColor;
                    ctlTotalsTabStrip1.InactiveHoveredTabColor = _tabColors.VillainInactiveHoveredTabColor;
                    ibTopMost.UseAlt = true;
                    ibClose.UseAlt = true;
                    break;
            }
        }

        private void frmTotalsV2_Resize(object sender, EventArgs e)
        {
            const int graphControlWidthPad = 41;
            const int pbCloseLocationXPad = 148;
            const int centerPanelYPad = 53;

            if (panelTab1.Visible)
            {
                Debug.WriteLine($"h1: {panelTab1.Size}");
                panelTab1.Size = new Size(Width, Height - panel1.Height - panel2.Height - centerPanelYPad);
                Debug.WriteLine($"h2: {panelTab1.Size}");
            }

            if (panelTab2.Visible)
            {
                panelTab2.Size = new Size(Width, Height - panel1.Height - panel2.Height - centerPanelYPad);
            }

            if (panelTab3.Visible)
            {
                panelTab3.Size = new Size(Width, Height - panel1.Height - panel2.Height - centerPanelYPad);
            }

            if (panelTab4.Visible)
            {
                panelTab4.Size = new Size(Width, Height - panel1.Height - panel2.Height - centerPanelYPad);
            }

            graphDef.Width = Width - graphControlWidthPad;
            graphRes.Width = Width - graphControlWidthPad;
            graphHP.Width = Width - graphControlWidthPad;
            graphEnd.Width = Width - graphControlWidthPad;

            graphMovement.Width = Width - graphControlWidthPad;
            graphPerception.Width = Width - graphControlWidthPad;
            graphHaste.Width = Width - graphControlWidthPad;
            graphToHit.Width = Width - graphControlWidthPad;
            graphAccuracy.Width = Width - graphControlWidthPad;
            graphDamage.Width = Width - graphControlWidthPad;
            graphEndRdx.Width = Width - graphControlWidthPad;
            graphThreat.Width = Width - graphControlWidthPad;

            graphStatusProt.Width = Width - graphControlWidthPad;
            graphStatusRes.Width = Width - graphControlWidthPad;

            graphDebuffRes.Width = Width - graphControlWidthPad;
            graphElusivity.Width = Width - graphControlWidthPad;

            ibClose.Location = ibClose.Location with { X = Width - pbCloseLocationXPad };

            // Prevent duplicate headers display (tiling) when stretching window horizontally
            ctlTotalsTabStrip1.Invalidate();
        }

        private void frmTotalsV2_FormClosed(object sender, FormClosedEventArgs e)
        {
            _myParent.FloatTotals(false, false);
        }

        private void frmTotalsV2_Move(object sender, EventArgs e)
        {
            StoreLocation();
        }

        private void ctlTotalsTabStrip1_TabClick(int index)
        {
            const int centerPanelYPad = 53;
            var panelYpos = ctlTotalsTabStrip1.Height;
            switch (index)
            {
                case 0 when !panelTab1.Visible:
                    panelTab2.Visible = false;
                    panelTab3.Visible = false;
                    panelTab4.Visible = false;

                    panelTab1.Location = new Point(0, panelYpos);
                    panelTab1.Size = new Size(Width, Height - panel1.Height - panel2.Height - centerPanelYPad);
                    panelTab1.Visible = true;

                    break;

                case 1 when !panelTab2.Visible:
                    panelTab1.Visible = false;
                    panelTab3.Visible = false;
                    panelTab4.Visible = false;

                    panelTab2.Location = new Point(0, panelYpos);
                    panelTab2.Size = new Size(Width, Height - panel1.Height - panel2.Height - centerPanelYPad);
                    panelTab2.Visible = true;

                    break;

                case 2 when !panelTab3.Visible:
                    panelTab1.Visible = false;
                    panelTab2.Visible = false;
                    panelTab4.Visible = false;

                    panelTab3.Location = new Point(0, panelYpos);
                    panelTab3.Size = new Size(Width, Height - panel1.Height - panel2.Height - centerPanelYPad);
                    panelTab3.Visible = true;

                    break;

                case 3 when !panelTab4.Visible:
                    panelTab1.Visible = false;
                    panelTab2.Visible = false;
                    panelTab3.Visible = false;

                    panelTab4.Location = new Point(0, panelYpos);
                    panelTab4.Size = new Size(Width, Height - panel1.Height - panel2.Height - centerPanelYPad);
                    panelTab4.Visible = true;

                    break;
            }
        }

        private void UpdateMovementData()
        {
            const int graphBottomMargin = 8;
            var displayStats = MidsContext.Character.DisplayStats;
            var movementUnitSpeed = clsConvertibleUnitValue.FormatSpeedUnit(MidsContext.Config.SpeedFormat);
            var movementUnitDistance = clsConvertibleUnitValue.FormatDistanceUnit(MidsContext.Config.SpeedFormat);

            graphMovement.Clear();

            var runSpdBase = displayStats.Speed(Statistics.BaseRunSpeed, MidsContext.Config.SpeedFormat);
            var runSpdValue = displayStats.MovementRunSpeed(MidsContext.Config.SpeedFormat, false);
            var runSpdUncapped = displayStats.MovementRunSpeed(MidsContext.Config.SpeedFormat, true);
            graphMovement.AddItemPair("Run Speed",
                $"{runSpdValue:##0.##} {movementUnitSpeed}",
                Math.Max(0, runSpdBase),
                Math.Max(0, runSpdValue),
                Math.Max(0, runSpdUncapped),
                (runSpdUncapped > runSpdValue & runSpdValue > 0
                    ? $"{runSpdUncapped:##0.##} {movementUnitSpeed} Run Speed, capped at {runSpdValue:##0.##} {movementUnitSpeed}"
                    : $"{runSpdValue:##0.##} {movementUnitSpeed} Run Speed"
                ) +
                (runSpdBase > 0
                    ? $"\r\nBase: {runSpdBase:##0.##} {movementUnitSpeed}"
                    : "")

                );

            var jumpSpdBase = displayStats.Speed(Statistics.BaseJumpSpeed, Enums.eSpeedMeasure.FeetPerSecond);
            var jumpSpdValue = displayStats.MovementJumpSpeed(MidsContext.Config.SpeedFormat, false);
            var jumpSpdUncapped = displayStats.MovementJumpSpeed(MidsContext.Config.SpeedFormat, true);
            graphMovement.AddItemPair("Jump Speed",
                $"{jumpSpdValue:##0.##} {movementUnitSpeed}",
                Math.Max(0, jumpSpdBase),
                Math.Max(0, jumpSpdValue),
                Math.Max(0, jumpSpdUncapped),
                (jumpSpdUncapped > jumpSpdValue & jumpSpdValue > 0
                    ? $"{jumpSpdUncapped:##0.##} {movementUnitSpeed} Jump Speed, capped at {jumpSpdValue:##0.##} {movementUnitSpeed}"
                    : $"{jumpSpdValue:##0.##} {movementUnitSpeed} Jump Speed"
                ) +
                (jumpSpdBase > 0
                    ? $"\r\nBase: {jumpSpdBase:##0.##} {movementUnitSpeed}"
                    : "")


                );

            var jumpHeightBase = displayStats.Distance(Statistics.BaseJumpHeight, MidsContext.Config.SpeedFormat);
            var jumpHeightValue = displayStats.MovementJumpHeight(MidsContext.Config.SpeedFormat);
            graphMovement.AddItemPair("Jump Height",
                $"{jumpHeightValue:##0.##} {movementUnitDistance}",
                Math.Max(0, jumpHeightBase),
                Math.Max(0, jumpHeightValue),
                $"{jumpHeightValue:##0.##} {movementUnitDistance} Jump Height" +
                (jumpHeightBase > 0
                    ? $"\r\nBase: {jumpHeightBase:##0.##} {movementUnitDistance}"
                    : "")

                );

            var flySpeedValue = displayStats.MovementFlySpeed(MidsContext.Config.SpeedFormat, false);
            var flySpeedBase = flySpeedValue == 0
                ? 0
                : displayStats.Speed(Statistics.BaseFlySpeed, MidsContext.Config.SpeedFormat);
            var flySpeedUncapped = displayStats.MovementFlySpeed(MidsContext.Config.SpeedFormat, true);
            graphMovement.AddItemPair("Fly Speed",
                $"{flySpeedValue:##0.##} {movementUnitSpeed}",
                Math.Max(0, flySpeedBase),
                Math.Max(0, flySpeedValue),
                Math.Max(0, flySpeedUncapped),
                (flySpeedUncapped > flySpeedValue & flySpeedValue > 0
                    ? $"{flySpeedUncapped:##0.##} {movementUnitSpeed} Fly Speed, capped at {flySpeedValue:##0.##} {movementUnitSpeed}"
                    : $"{flySpeedValue:##0.##} {movementUnitSpeed} Fly Speed"
                ) +
                (flySpeedBase > 0
                    ? $"\r\nBase: {flySpeedBase:##0.##} {movementUnitSpeed}"
                    : ""));

            graphMovement.Size = graphMovement.Size with { Height = Math.Max(graphMovement.Size.Height, graphMovement.ContentHeight + graphBottomMargin) };
            graphMovement.Draw();
        }

        private void UpdatePerceptionData()
        {
            const int graphBottomMargin = 8;
            var displayStats = MidsContext.Character.DisplayStats;
            var movementUnitDistance = clsConvertibleUnitValue.FormatDistanceUnit(MidsContext.Config.SpeedFormat);

            graphPerception.Clear();
            graphPerception.AddItemPair("PvE",
                $"{displayStats.Distance(MidsContext.Character.Totals.StealthPvE, MidsContext.Config.SpeedFormat):###0.##} {movementUnitDistance}",
                0,
                displayStats.Distance(MidsContext.Character.Totals.StealthPvE, MidsContext.Config.SpeedFormat),
                GenericDataTooltip3(displayStats.Distance(MidsContext.Character.Totals.StealthPvE, MidsContext.Config.SpeedFormat), 0, displayStats.Distance(MidsContext.Character.Totals.StealthPvE, MidsContext.Config.SpeedFormat), "Stealth (PvE)", "", movementUnitDistance));

            graphPerception.AddItemPair("PvP",
                $"{displayStats.Distance(MidsContext.Character.Totals.StealthPvP, MidsContext.Config.SpeedFormat):###0.##} {movementUnitDistance}",
                0,
                displayStats.Distance(MidsContext.Character.Totals.StealthPvP, MidsContext.Config.SpeedFormat),
                GenericDataTooltip3(displayStats.Distance(MidsContext.Character.Totals.StealthPvP, MidsContext.Config.SpeedFormat), 0, displayStats.Distance(MidsContext.Character.Totals.StealthPvP, MidsContext.Config.SpeedFormat), "Stealth (PvP)", "", movementUnitDistance));

            graphPerception.AddItemPair("Perception",
                $"{displayStats.Distance(displayStats.Perception(false), MidsContext.Config.SpeedFormat):###0.##} {movementUnitDistance}",
                displayStats.Distance(Statistics.BasePerception, MidsContext.Config.SpeedFormat),
                displayStats.Distance(displayStats.Perception(false), MidsContext.Config.SpeedFormat),
                displayStats.Distance(displayStats.Perception(true), MidsContext.Config.SpeedFormat),
                GenericDataTooltip3(displayStats.Distance(displayStats.Perception(false), MidsContext.Config.SpeedFormat), displayStats.Distance(Statistics.BasePerception, MidsContext.Config.SpeedFormat), displayStats.Distance(displayStats.Perception(true), MidsContext.Config.SpeedFormat), "Perception", "", movementUnitDistance)
            );

            graphPerception.Size = graphPerception.Size with { Height = Math.Max(graphPerception.Size.Height, graphPerception.ContentHeight + graphBottomMargin) };
            graphPerception.Draw();
        }

        private void rbUnits_CheckChanged(object sender, EventArgs e)
        {
            var target = (RadioButton) sender;
            if (!target.Checked)
            {
                return;
            }

            MidsContext.Config.SpeedFormat = target.Name switch
            {
                "radioButton2" => Enums.eSpeedMeasure.KilometersPerHour,
                "radioButton3" => Enums.eSpeedMeasure.FeetPerSecond,
                "radioButton4" => Enums.eSpeedMeasure.MetersPerSecond,
                _ => Enums.eSpeedMeasure.MilesPerHour
            };

            UpdateMovementData();
            UpdatePerceptionData();
        }

        #region ImageButtonEx buttons handlers

        private void IbCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void IbTopMostClick(object sender, EventArgs e)
        {
            KeepOnTop = !KeepOnTop;
            TopMost = KeepOnTop;
            ibTopMost.ToggleState = TopMost ? ImageButtonEx.States.ToggledOn : ImageButtonEx.States.ToggledOff;
        }

        #endregion
        

        #region frmTotals import

        private void StoreLocation()
        {
            if (!MainModule.MidsController.IsAppInitialized) return;

            MainModule.MidsController.SzFrmTotals.X = Left;
            MainModule.MidsController.SzFrmTotals.Y = Top;
            MainModule.MidsController.SzFrmTotals.Width = Width;
            MainModule.MidsController.SzFrmTotals.Height = Height;
        }

        public void SetLocation()
        {
            Top = MainModule.MidsController.SzFrmTotals.X;
            Left = MainModule.MidsController.SzFrmTotals.Y;
        }

        #endregion

        private string GenericDataTooltip3(float value, float valueBase, float valueUncapped, string statName, string percentageSign = "%", string movementUnit = "", bool plusSignEnabled = false)
        {
            return (valueUncapped > value
                       ? $"{(plusSignEnabled && valueUncapped > 0 ? "+" : "")}{valueUncapped:##0.##}{percentageSign}{movementUnit} {statName}, capped at {(plusSignEnabled && value > 0 ? "+" : "")}{value:##0.##}{percentageSign}"
                       : $"{(plusSignEnabled && value > 0 ? "+" : "")}{value:##0.##}{percentageSign}{movementUnit} {statName}"
                   ) +
                   (valueBase > 0
                       ? $"\r\nBase: {(plusSignEnabled && valueBase > 0 ? "+" : "")}{valueBase:##0.##}{percentageSign}{movementUnit}"
                       : "") +
                   (statName is "Damage" or "Haste" || valueBase > 0
                       ? $"\r\n(Enh: {value - valueBase:##0.##}{percentageSign})"
                       : "");
        }

        public void UpdateData()
        {
            //pbClose.Refresh();
            //pbTopMost.Refresh();
            //var uncappedStats = MidsContext.Character.Totals;
            //var cappedStats = MidsContext.Character.TotalsCapped;
            const int graphBottomMargin = 8;
            var displayStats = MidsContext.Character.DisplayStats;
            var atName = MidsContext.Character.Archetype.DisplayName;

            var movementUnitSpeed = clsConvertibleUnitValue.FormatSpeedUnit(MidsContext.Config.SpeedFormat);
            //var movementUnitDistance = clsConvertibleUnitValue.FormatDistanceUnit(MidsContext.Config.SpeedFormat);

            var damageVectors = Enum.GetValues(typeof(Enums.eDamage));
            var damageVectorsNames = Enum.GetNames(typeof(Enums.eDamage));
            var excludedDefVectors = new List<Enums.eDamage>
            {
                Enums.eDamage.None,
                DatabaseAPI.RealmUsesToxicDef() ? Enums.eDamage.None : Enums.eDamage.Toxic,
                Enums.eDamage.Special,
                Enums.eDamage.Unique1,
                Enums.eDamage.Unique2,
                Enums.eDamage.Unique3
            }.Cast<int>().ToList();

            var excludedResVectors = new List<Enums.eDamage>
            {
                Enums.eDamage.None,
                Enums.eDamage.Melee,
                Enums.eDamage.Ranged,
                Enums.eDamage.AoE,
                Enums.eDamage.Special,
                Enums.eDamage.Unique1,
                Enums.eDamage.Unique2,
                Enums.eDamage.Unique3
            }.Cast<int>().ToList();

            var excludedElusivityVectors = new List<Enums.eDamage>
            {
                Enums.eDamage.Special,
                Enums.eDamage.Unique1,
                Enums.eDamage.Unique2,
                Enums.eDamage.Unique3
            }.Cast<int>().ToList();

            graphDef.Clear();
            for (var i = 0; i < damageVectors.Length; i++)
            {
                if (excludedDefVectors.Contains(i))
                {
                    continue;
                }

                graphDef.AddItemPair(damageVectorsNames[i],
                    $"{displayStats.Defense(i):##0.##}%",
                    Math.Max(0, displayStats.Defense(i)),
                    Math.Max(0, displayStats.Defense(i)),
                    $"{displayStats.Defense(i):##0.###}% {FormatVectorType(typeof(Enums.eDamage), i)} defense");
            }

            graphDef.Size = graphDef.Size with {Height = Math.Max(graphDef.Size.Height, graphDef.ContentHeight + graphBottomMargin)};
            graphDef.Draw();

            graphRes.Clear();
            for (var i = 0; i < damageVectors.Length; i++)
            {
                if (excludedResVectors.Contains(i))
                {
                    continue;
                }

                var resValue = displayStats.DamageResistance(i, false);
                var resValueUncapped = displayStats.DamageResistance(i, true);
                graphRes.AddItemPair(damageVectorsNames[i], $"{resValue:##0.##}%",
                    Math.Max(0, resValue),
                    Math.Max(0, resValue),
                    Math.Max(0, resValueUncapped),
                    resValueUncapped > resValue & resValue > 0
                        ? $"{resValueUncapped:##0.##}% {FormatVectorType(typeof(Enums.eDamage), i)} resistance (capped at {MidsContext.Character.Archetype.ResCap * 100:##0.##}%)"
                        : $"{resValue:##0.##}% {FormatVectorType(typeof(Enums.eDamage), i)} resistance ({atName} resistance cap: {MidsContext.Character.Archetype.ResCap * 100:##0.##}%)");
            }

            graphRes.Size = graphRes.Size with {Height = Math.Max(graphRes.Size.Height, graphRes.ContentHeight + graphBottomMargin)};
            graphRes.Draw();

            graphHP.Clear();
            var regenValue = displayStats.HealthRegenPercent(false);
            var regenValueUncapped = displayStats.HealthRegenPercent(true);
            const float regenBase = 100;
            graphHP.AddItemPair("Regeneration",
                $"{regenValue:###0.##}%",
                Math.Max(0, regenBase),
                Math.Max(0, regenValue),
                Math.Max(0, regenValueUncapped),
                (regenValueUncapped > regenValue & regenValue > 0
                    ? $"{regenValueUncapped:##0.##}% Regeneration, capped at {regenValue:##0.##}%"
                    : $"{regenValue:##0.##}% Regeneration"
                ) +
                $" ({MidsContext.Character.DisplayStats.HealthRegenHPPerSec:##0.##} HP/s)" +
                (regenBase > 0 ? $"\r\nBase: {regenBase:##0.##}%" : ""));

            var hpValue = displayStats.HealthHitpointsNumeric(false);
            var hpValueUncapped = displayStats.HealthHitpointsNumeric(true);
            var hpBase = MidsContext.Character.Archetype.Hitpoints;
            var absorbValue = Math.Min(displayStats.Absorb, hpBase);
            graphHP.AddItemPair("Max HP", $"{hpValue:###0.##}",
                Math.Max(0, hpBase),
                Math.Max(0, hpValue),
                Math.Max(0, hpValueUncapped),
                Math.Max(0, absorbValue),
                (hpValueUncapped > hpValue & hpValue > 0
                    ? $"{hpValueUncapped:##0.##} HP, capped at {MidsContext.Character.Archetype.HPCap} HP"
                    : $"{hpValue:##0.##} HP ({atName} HP cap: {MidsContext.Character.Archetype.HPCap} HP)"

                ) +
                $"\r\nBase: {hpBase:##0.##} HP" +
                (absorbValue > 0
                    ? $"\r\nAbsorb: {absorbValue:##0.##} ({absorbValue / hpBase * 100:##0.##}% of base HP)"
                    : ""));

            graphHP.Size = graphHP.Size with {Height = Math.Max(graphHP.Size.Height, graphHP.ContentHeight + graphBottomMargin)};
            graphHP.Draw();
            
            graphEnd.Clear();
            var endRecValue = displayStats.EnduranceRecoveryNumeric;
            var endRecValueUncapped = displayStats.EnduranceRecoveryNumericUncapped;
            var endRecBase = MidsContext.Character.Archetype.BaseRecovery * displayStats.EnduranceMaxEnd / 60f;
            graphEnd.AddItemPair("End Rec", $"{endRecValue:##0.##}/s",
                Math.Max(0, endRecBase),
                Math.Max(0, endRecValue),
                Math.Max(0, endRecValueUncapped),
                (endRecValueUncapped > endRecValue & endRecValue > 0
                    ? $"{endRecValueUncapped:##0.##}/s End. ({displayStats.EnduranceRecoveryPercentage(true):##0.##}%), capped at {MidsContext.Character.Archetype.RecoveryCap * 100:##0.##}%"
                    : $"{endRecValue:##0.##}/s End. ({displayStats.EnduranceRecoveryPercentage(false):##0.##}%) ({atName} End. recovery cap: {MidsContext.Character.Archetype.RecoveryCap * 100:##0.##}%)"
                ) +
                $"\r\nBase: {endRecBase:##0.##}/s");
            
            graphEnd.AddItemPair("End Use",
                $"{displayStats.EnduranceUsage:##0.##}/s",
                0,
                displayStats.EnduranceUsage,
                $"{displayStats.EnduranceUsage:##0.##}/s End. (Net gain: {displayStats.EnduranceRecoveryNet:##0.##}/s)");

            const float maxEndBase = 100;
            graphEnd.AddItemPair("Max End",
                $"{displayStats.EnduranceMaxEnd:##0.##}",
                maxEndBase,
                displayStats.EnduranceMaxEnd,
                $"{displayStats.EnduranceMaxEnd:##0.##} Maximum Endurance (base: {maxEndBase:##0.##})");

            graphEnd.Size = graphEnd.Size with {Height = Math.Max(graphEnd.Size.Height, graphEnd.ContentHeight + graphBottomMargin)};
            graphEnd.Draw();

            ///////////////////////////////

            UpdateMovementData();

            ///////////////////////////////

            UpdatePerceptionData();

            ///////////////////////////////

            graphHaste.Clear();
            graphHaste.AddItemPair("Haste",
                $"{displayStats.BuffHaste(false):##0.##}%",
                100,
                Math.Max(0, displayStats.BuffHaste(false)),
                Math.Max(0, displayStats.BuffHaste(true)),
                GenericDataTooltip3(displayStats.BuffHaste(false), 100, displayStats.BuffHaste(true), "Haste"));

            graphToHit.Clear();
            graphToHit.AddItemPair("ToHit",
                $"{displayStats.BuffToHit:##0.##}%",
                0,
                Math.Max(0, displayStats.BuffToHit),
                GenericDataTooltip3(displayStats.BuffToHit, 0, displayStats.BuffToHit, "ToHit", "%", "", true));

            graphAccuracy.Clear();
            graphAccuracy.AddItemPair("Accuracy",
                $"{displayStats.BuffAccuracy:##0.##}%",
                0,
                Math.Max(0, displayStats.BuffAccuracy),
                GenericDataTooltip3(displayStats.BuffAccuracy, 0, displayStats.BuffAccuracy, "Accuracy", "%", "", true));

            graphDamage.Clear();
            graphDamage.AddItemPair("Damage",
                $"{displayStats.BuffDamage(false):##0.##}%",
                100,
                Math.Max(0, displayStats.BuffDamage(false)),
                Math.Max(0, displayStats.BuffDamage(true)),
                GenericDataTooltip3(displayStats.BuffDamage(false), 100, displayStats.BuffDamage(true), "Damage")
                );

            graphEndRdx.Clear();
            graphEndRdx.AddItemPair("EndRdx",
                $"{displayStats.BuffEndRdx:##0.##}%",
                0,
                displayStats.BuffEndRdx,
                GenericDataTooltip3(displayStats.BuffEndRdx, 0, displayStats.BuffEndRdx, "EndRdx"));

            graphThreat.Clear();
            graphThreat.AddItemPair("Threat",
                $"{displayStats.ThreatLevel:##0.##}",
                MidsContext.Character.Archetype.BaseThreat * 100,
                displayStats.ThreatLevel,
                GenericDataTooltip3(displayStats.ThreatLevel, MidsContext.Character.Archetype.BaseThreat * 100, displayStats.ThreatLevel, "Threat"));

            graphHaste.Draw();
            graphToHit.Draw();
            graphAccuracy.Draw();
            graphDamage.Draw();
            graphEndRdx.Draw();
            graphThreat.Draw();
 
            ///////////////////////////////

            graphStatusProt.Clear();
            graphStatusRes.Clear();
            var mezProtections = MidsContext.Character.Totals.Mez
                .Select(e => e > 0 ? 0 : Math.Abs(e))
                .ToArray();
            foreach (var m in MezList)
            {
                // Use Math.Abs() here instead of negative sign to prevent display of "-0"
                graphStatusProt.AddItemPair($"{m}",
                    $"{mezProtections[(int) m]:####0.##}",
                    0,
                    mezProtections[(int) m],
                    $"{mezProtections[(int) m]:####0.##} Status protection to {m}");

                graphStatusRes.AddItemPair($"{m}",
                    $"{MidsContext.Character.Totals.MezRes[(int) m]:####0.##}%",
                    0,
                    MidsContext.Character.Totals.MezRes[(int) m],
                    $"{MidsContext.Character.Totals.MezRes[(int) m]:####0.##}% Status resistance to {m}");
            }

            graphStatusProt.Size = graphStatusProt.Size with { Height = Math.Max(graphStatusProt.Size.Height, graphStatusProt.ContentHeight + graphBottomMargin)};
            graphStatusProt.Draw();

            graphStatusRes.Size = graphStatusRes.Size with { Height = Math.Max(graphStatusRes.Size.Height, graphStatusRes.ContentHeight + graphBottomMargin)};
            graphStatusRes.Draw();

            ///////////////////////////////

            graphDebuffRes.Clear();
            var cappedDebuffRes = DebuffEffectsList.Select(e => Math.Min(
                    e == Enums.eEffectType.Defense
                        ? Statistics.MaxDefenseDebuffRes
                        : Statistics.MaxGenericDebuffRes,
                    MidsContext.Character.Totals.DebuffRes[(int) e]))
                .ToList();

            var uncappedDebuffRes = DebuffEffectsList.Select(e => MidsContext.Character.Totals.DebuffRes[(int) e]).ToList();
            for (var i = 0; i < cappedDebuffRes.Count; i++)
            {
                graphDebuffRes.AddItemPair($"{DebuffEffectsList[i]}",
                    $"{cappedDebuffRes[i]:##0.##}%",
                    0,
                    Math.Max(0, cappedDebuffRes[i]),
                    Math.Max(0, uncappedDebuffRes[i]),
                    GenericDataTooltip3(cappedDebuffRes[i], 0, uncappedDebuffRes[i], $"Debuff resistance to {DebuffEffectsList[i]}")
                );
            }

            graphDebuffRes.Size = graphDebuffRes.Size with { Height = Math.Max(graphDebuffRes.Size.Height, graphDebuffRes.ContentHeight + graphBottomMargin)};
            graphDebuffRes.Draw();

            graphElusivity.Clear();
            for (var i = 0; i < damageVectors.Length; i++)
            {
                if (excludedElusivityVectors.Contains(i))
                {
                    continue;
                }

                var damageVector = damageVectorsNames[i] == "None" ? "Untyped" : damageVectorsNames[i];
                var elValue = (MidsContext.Character.Totals.Elusivity[i] + (MidsContext.Config.Inc.DisablePvE ? 0.4f : 0)) * 100;
                graphElusivity.AddItemPair(damageVector,
                    $"{elValue:##0.##}%",
                    0,
                    Math.Max(0, elValue),
                    $"{elValue:##0.##}% Elusivity ({damageVector})");
            }

            graphElusivity.Size = graphElusivity.Size with { Height = Math.Max(graphElusivity.Size.Height, graphElusivity.ContentHeight + graphBottomMargin) };
            graphElusivity.Draw();
        }
    }
}