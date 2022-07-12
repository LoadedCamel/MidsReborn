using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Display;
using mrbBase.Base.Master_Classes;
using mrbControls;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmTotalsV2 : Form
    {
        private class TabColorScheme
        {
            public readonly Color TabTextColor = Color.WhiteSmoke;
            public readonly Color TabOutlineColor = Color.Black;

            public readonly Color HeroInactiveTabColor = Color.FromArgb(30, 85, 130);
            public readonly Color HeroBorderColor = Color.Goldenrod;
            public readonly Color HeroActiveTabColor = Color.Goldenrod;

            public readonly Color VillainInactiveTabColor = Color.FromArgb(86, 12, 12);
            public readonly Color VillainBorderColor = Color.FromArgb(184, 184, 187);
            public readonly Color VillainActiveTabColor = Color.FromArgb(184, 184, 187);
        }

        private readonly frmMain _myParent;
        private bool KeepOnTop { get; set; }
        private readonly TabColorScheme _tabColors = new();

        #region Enums sub-lists (groups)

        private readonly List<Enums.eDamage> DefenseDamageList = new()
        {
            Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
            Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Psionic, Enums.eDamage.Melee,
            Enums.eDamage.Ranged, Enums.eDamage.AoE
        };

        private readonly List<Enums.eDamage> ResistanceDamageList = new()
        {
            Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
            Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Toxic, Enums.eDamage.Psionic
        };

        private readonly List<Enums.eEffectType> MovementTypesList = new()
        {
            Enums.eEffectType.SpeedRunning, Enums.eEffectType.SpeedJumping, Enums.eEffectType.JumpHeight,
            Enums.eEffectType.SpeedFlying
        };

        private readonly List<Enums.eEffectType> PerceptionEffectsList = new()
        {
            Enums.eEffectType.StealthRadius, Enums.eEffectType.StealthRadiusPlayer, Enums.eEffectType.PerceptionRadius
        };

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

        #endregion

        public frmTotalsV2(ref frmMain parentForm)
        {
            try
            {
                _myParent = parentForm;
                FormClosed += frmTotalsV2_FormClosed;
                Load += frmTotalsV2_Load;

                KeepOnTop = true;
                InitializeComponent();
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
            IEnumerable<RadioButton> radioControls = Controls.OfType<RadioButton>();
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

            if (previousCfgSpeedMeasure != MidsContext.Config.SpeedFormat) UpdateData();
        }

        private static int GetEpicPowersetIndex()
        {
            var idx = -1;
            int i;

            // Fetch ancillary/epic powerset index
            for (i = 0; i < MidsContext.Character.Powersets.Length; i++)
            {
                if (MidsContext.Character.Powersets[i] == null) continue;
                if (MidsContext.Character.Powersets[i].GroupName == "Epic")
                {
                    idx = i;
                    break;
                }
            }

            if (idx == -1) return -1;

            // Check if power taken in pool
            for (i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
            {
                if (MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset ==
                    MidsContext.Character.Powersets[idx].nID) return idx;
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

        public override void Refresh()
        {
            if (MidsContext.Character.IsHero())
            {
                ctlTotalsTabStrip1.ColorInactiveTab = _tabColors.HeroInactiveTabColor;
                ctlTotalsTabStrip1.BackColor = _tabColors.HeroInactiveTabColor;
                ctlTotalsTabStrip1.ColorActiveTab = _tabColors.HeroActiveTabColor;
                ctlTotalsTabStrip1.ColorStripLine = _tabColors.HeroBorderColor;
            }
            else
            {
                ctlTotalsTabStrip1.ColorInactiveTab = _tabColors.VillainInactiveTabColor;
                ctlTotalsTabStrip1.BackColor = _tabColors.VillainInactiveTabColor;
                ctlTotalsTabStrip1.ColorActiveTab = _tabColors.VillainActiveTabColor;
                ctlTotalsTabStrip1.ColorStripLine = _tabColors.VillainBorderColor;
            }

            base.Refresh();
        }

        private void frmTotalsV2_Load(object sender, EventArgs e)
        {
            CenterToParent();
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl1.ItemSize = new Size(0, 1);
            
            // Tab control border is 3 px thick, move it a little off to hide it
            // Change the size fixed values so they are the same as in frmTotalsV2_Resize()
            tabControl1.Location = new Point(-3, 19);
            tabControl1.Size = new Size(ClientSize.Width + 8, ClientSize.Height - 58);
            
            ctlTotalsTabStrip1.ClearItems();
            for (var i = 0; i < tabControl1.TabPages.Count; i++)
            {
                ctlTotalsTabStrip1.AddItem(tabControl1.TabPages[i].Text);
            }

            ctlTotalsTabStrip1.Redraw();

            Refresh();
            SetTitle(this);
            SetUnitRadioButtons();
            UpdateData();
        }

        private void frmTotalsV2_FormClosed(object sender, FormClosedEventArgs e)
        {
            _myParent.FloatTotals(false, false);
        }

        private void frmTotalsV2_Move(object sender, EventArgs e)
        {
            StoreLocation();
        }

        #region PictureBox buttons handlers

        private void PbCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        private void PbClosePaint(object sender, PaintEventArgs e)
        {
            if (_myParent?.Drawing == null) return;

            var iStr = "Close";
            var rectangle = new Rectangle();
            ref var local = ref rectangle;
            var size = MidsContext.Character.IsHero()
                ? _myParent.Drawing.bxPower[2].Size
                : _myParent.Drawing.bxPower[4].Size;
            var width = size.Width;
            size = MidsContext.Character.IsHero()
                ? _myParent.Drawing.bxPower[2].Size
                : _myParent.Drawing.bxPower[4].Size;
            var height1 = size.Height;
            local = new Rectangle(0, 0, width, height1);
            var destRect = new Rectangle(0, 0, 105, 22);
            using var stringFormat = new StringFormat();
            using var bFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold, GraphicsUnit.Point);
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            using var extendedBitmap = new ExtendedBitmap(destRect.Width, destRect.Height);
            extendedBitmap.Graphics.Clear(BackColor);
            extendedBitmap.Graphics.DrawImage(
                MidsContext.Character.IsHero()
                    ? _myParent.Drawing.bxPower[2].Bitmap
                    : _myParent.Drawing.bxPower[4].Bitmap, destRect, 0, 0, rectangle.Width, rectangle.Height,
                GraphicsUnit.Pixel, _myParent.Drawing.pImageAttributes);
            var height2 = bFont.GetHeight(e.Graphics) + 2;
            var Bounds = new RectangleF(0, (22 - height2) / 2, 105, height2);
            var graphics = extendedBitmap.Graphics;
            clsDrawX.DrawOutlineText(iStr, Bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1, graphics);
            e.Graphics.DrawImage(extendedBitmap.Bitmap, 0, 0);
        }

        private void PbTopMostClick(object sender, EventArgs e)
        {
            KeepOnTop = !KeepOnTop;
            TopMost = KeepOnTop;
            pbTopMost.Refresh();
        }

        private void PbTopMostPaint(object sender, PaintEventArgs e)
        {
            if (_myParent?.Drawing == null) return;

            var index = 2;
            if (KeepOnTop) index = 3;
            var iStr = "Keep On top";
            var rectangle = new Rectangle(0, 0, _myParent.Drawing.bxPower[index].Size.Width,
                _myParent.Drawing.bxPower[index].Size.Height);
            var destRect = new Rectangle(0, 0, 105, 22);
            using var stringFormat = new StringFormat();
            using var bFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold, GraphicsUnit.Point);
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            using var extendedBitmap = new ExtendedBitmap(destRect.Width, destRect.Height);
            extendedBitmap.Graphics.Clear(BackColor);
            if (index == 3)
            {
                extendedBitmap.Graphics.DrawImage(_myParent.Drawing.bxPower[index].Bitmap, destRect, 0, 0,
                    rectangle.Width, rectangle.Height, GraphicsUnit.Pixel);
            }
            else
            {
                extendedBitmap.Graphics.DrawImage(_myParent.Drawing.bxPower[index].Bitmap, destRect, 0, 0,
                    rectangle.Width, rectangle.Height, GraphicsUnit.Pixel, _myParent.Drawing.pImageAttributes);
            }

            var height = bFont.GetHeight(e.Graphics) + 2;
            var Bounds = new RectangleF(0, (22 - height) / 2, 105, height);
            var graphics = extendedBitmap.Graphics;
            clsDrawX.DrawOutlineText(iStr, Bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1, graphics);
            e.Graphics.DrawImage(extendedBitmap.Bitmap, 0, 0);
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

        private bool RealmUsesToxicDef()
        {
            var dbPath = (MidsContext.Config != null ? MidsContext.Config.DataPath : Files.FDefaultPath).ToLowerInvariant();

            return dbPath.EndsWith("homecoming");
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
                   (statName == "Damage"
                       ? $"\r\nEnh: {value - valueBase}{percentageSign}"
                       : "");
        }

        public void UpdateData()
        {
            //pbClose.Refresh();
            //pbTopMost.Refresh();
            //var uncappedStats = MidsContext.Character.Totals;
            //var cappedStats = MidsContext.Character.TotalsCapped;
            var displayStats = MidsContext.Character.DisplayStats;
            var atName = MidsContext.Character.Archetype.DisplayName;
            //var watch = Stopwatch.StartNew();

            var damageVectors = Enum.GetValues(typeof(Enums.eDamage));
            var damageVectorsNames = Enum.GetNames(typeof(Enums.eDamage));
            var excludedDefVectors = new List<Enums.eDamage>
            {
                Enums.eDamage.None,
                RealmUsesToxicDef() ? Enums.eDamage.None : Enums.eDamage.Toxic,
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

            graphDef.Clear();
            for (var i = 0; i < damageVectors.Length; i++)
            {
                if (excludedDefVectors.Contains(i))
                {
                    continue;
                }

                graphDef.AddItemPair(damageVectorsNames[i],
                    $"{displayStats.Defense(i):##0.##}%",
                    displayStats.Defense(i),
                    displayStats.Defense(i),
                    $"{displayStats.Defense(i):##0.###}% {FormatVectorType(typeof(Enums.eDamage), i)} defense");
            }

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
                    resValue,
                    resValue,
                    resValueUncapped,
                    resValueUncapped > resValue
                        ? $"{resValueUncapped:##0.##}% {FormatVectorType(typeof(Enums.eDamage), i)} resistance (capped at {MidsContext.Character.Archetype.ResCap * 100:##0.##}%)"
                        : $"{resValue:##0.##}% {FormatVectorType(typeof(Enums.eDamage), i)} resistance ({atName} resistance cap: {MidsContext.Character.Archetype.ResCap * 100:##0.##}%)");
            }

            graphRes.Draw();

            graphHP.Clear();
            var regenValue = displayStats.HealthRegenPercent(false);
            var regenValueUncapped = displayStats.HealthRegenPercent(true);
            const float regenBase = 100;
            graphHP.AddItemPair("Regeneration",
                $"{regenValue:###0.##}%",
                regenBase,
                regenValue,
                regenValueUncapped,
                (regenValueUncapped > regenValue
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
                hpBase,
                hpValue,
                hpValueUncapped,
                absorbValue,
                (hpValueUncapped > hpValue
                    ? $"{hpValueUncapped:##0.##} HP, capped at {MidsContext.Character.Archetype.HPCap} HP"
                    : $"{hpValue:##0.##} HP ({atName} HP cap: {MidsContext.Character.Archetype.HPCap} HP)"

                ) +
                $"\r\nBase: {hpBase:##0.##} HP" +
                (absorbValue > 0
                    ? $"\r\nAbsorb: {absorbValue:##0.##} ({absorbValue / hpBase * 100:##0.##}% of base HP)"
                    : ""));

            graphHP.Draw();
            
            graphEnd.Clear();
            var endRecValue = displayStats.EnduranceRecoveryNumeric;
            var endRecValueUncapped = displayStats.EnduranceRecoveryNumericUncapped;
            var endRecBase = MidsContext.Character.Archetype.BaseRecovery * displayStats.EnduranceMaxEnd / 60f;
            graphEnd.AddItemPair("End Rec", $"{endRecValue:##0.##}/s",
                endRecBase,
                endRecValue,
                endRecValueUncapped,
                (endRecValueUncapped > endRecValue
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

            graphEnd.Draw();

            ///////////////////////////////

            /*SetLvsBulk(
                _lvList,
                "Movement",
                new[]
                {
                    displayStats.MovementRunSpeed(MidsContext.Config.SpeedFormat, false),
                    displayStats.MovementJumpSpeed(MidsContext.Config.SpeedFormat, false),
                    displayStats.MovementJumpHeight(MidsContext.Config.SpeedFormat),
                    displayStats.MovementFlySpeed(MidsContext.Config.SpeedFormat, false)
                }
            );*/

            graphMovement.Clear();
            var movementUnitSpeed = clsConvertibleUnitValue.FormatSpeedUnit(MidsContext.Config.SpeedFormat);
            var movementUnitDistance = clsConvertibleUnitValue.FormatDistanceUnit(MidsContext.Config.SpeedFormat);

            var runSpdBase = displayStats.Speed(Statistics.BaseRunSpeed, Enums.eSpeedMeasure.FeetPerSecond);
            var runSpdValue = displayStats.MovementRunSpeed(Enums.eSpeedMeasure.FeetPerSecond, false);
            var runSpdUncapped = displayStats.MovementRunSpeed(Enums.eSpeedMeasure.FeetPerSecond, true);
            graphMovement.AddItemPair("Run Speed",
                $"{runSpdValue:##0.##} mph",
                runSpdBase,
                runSpdValue,
                runSpdUncapped,
                (runSpdUncapped > runSpdValue
                    ? $"{runSpdUncapped:##0.##} {movementUnitSpeed} Run Speed, capped at {runSpdValue:##0.##} {movementUnitSpeed}"
                    : $"{runSpdValue:##0.##} {movementUnitSpeed} Run Speed"
                ) +
                (runSpdBase > 0
                    ? $"\r\nBase: {runSpdBase:##0.##} {movementUnitSpeed}"
                    : "")
                
                );

            var jumpSpdBase = displayStats.Speed(Statistics.BaseJumpSpeed, Enums.eSpeedMeasure.FeetPerSecond);
            var jumpSpdValue = displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.FeetPerSecond, false);
            var jumpSpdUncapped = displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.FeetPerSecond, true);
            graphMovement.AddItemPair("Jump Speed",
                $"{jumpSpdValue:##0.##} mph",
                jumpSpdBase,
                jumpSpdValue,
                jumpSpdUncapped,
                (jumpSpdUncapped > jumpSpdValue
                    ? $"{jumpSpdUncapped:##0.##} {movementUnitSpeed} Jump Speed, capped at {jumpSpdValue:##0.##} {movementUnitSpeed}"
                    : $"{jumpSpdValue:##0.##} {movementUnitSpeed} Jump Speed"
                ) +
                (jumpSpdBase > 0
                    ? $"\r\nBase: {jumpSpdBase:##0.##} {movementUnitSpeed}"
                    : "")
                
                
                );

            var jumpHeightBase = displayStats.Distance(Statistics.BaseJumpHeight, Enums.eSpeedMeasure.FeetPerSecond);
            var jumpHeightValue = displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond);
            graphMovement.AddItemPair("Jump Height",
                $"{jumpHeightValue:##0.##} ft",
                jumpHeightBase,
                jumpHeightValue,
                $"{jumpHeightValue:##0.##} {movementUnitDistance} Jump Height" +
                (jumpHeightBase > 0
                    ? $"\r\nBase: {jumpHeightBase:##0.##} {movementUnitDistance}"
                    : "")
                
                );

            var flySpeedValue = displayStats.MovementFlySpeed(Enums.eSpeedMeasure.MilesPerHour, false);
            var flySpeedBase = flySpeedValue == 0
                ? 0
                : displayStats.Speed(Statistics.BaseFlySpeed, Enums.eSpeedMeasure.MilesPerHour);
            var flySpeedUncapped = displayStats.MovementFlySpeed(Enums.eSpeedMeasure.MilesPerHour, true);
            graphMovement.AddItemPair("Fly Speed",
                $"{flySpeedValue:##0.##} mph",
                flySpeedBase,
                flySpeedValue,
                flySpeedUncapped,
                (flySpeedUncapped > flySpeedValue
                    ? $"{flySpeedUncapped:##0.##} {movementUnitSpeed} Fly Speed, capped at {flySpeedValue:##0.##} {movementUnitSpeed}"
                    : $"{flySpeedValue:##0.##} {movementUnitSpeed} Fly Speed"
                ) +
                (flySpeedBase > 0
                    ? $"\r\nBase: {flySpeedBase:##0.##} {movementUnitSpeed}"
                    : ""));

            graphMovement.Draw();

            ///////////////////////////////

            graphPerception.Clear();
            graphPerception.AddItemPair("PvE",
                $"{MidsContext.Character.Totals.StealthPvE:###0.##} ft",
                0,
                MidsContext.Character.Totals.StealthPvE,
                GenericDataTooltip3(displayStats.Distance(MidsContext.Character.Totals.StealthPvE, MidsContext.Config.SpeedFormat), 0, displayStats.Distance(MidsContext.Character.Totals.StealthPvE, MidsContext.Config.SpeedFormat), "Stealth (PvE)", "", movementUnitDistance));

            graphPerception.AddItemPair("PvP",
                $"{MidsContext.Character.Totals.StealthPvP:###0.##} ft",
                0,
                MidsContext.Character.Totals.StealthPvP,
                GenericDataTooltip3(displayStats.Distance(MidsContext.Character.Totals.StealthPvP, MidsContext.Config.SpeedFormat), 0, displayStats.Distance(MidsContext.Character.Totals.StealthPvP, MidsContext.Config.SpeedFormat), "Stealth (PvP)", "", movementUnitDistance));

            graphPerception.AddItemPair("Perception",
                $"{displayStats.Perception(false):###0.##} ft",
                Statistics.BasePerception,
                displayStats.Perception(false),
                displayStats.Perception(true),
                GenericDataTooltip3(displayStats.Distance(displayStats.Perception(false), MidsContext.Config.SpeedFormat), displayStats.Distance(Statistics.BasePerception, MidsContext.Config.SpeedFormat), displayStats.Distance(displayStats.Perception(true), MidsContext.Config.SpeedFormat), "Perception", "", movementUnitDistance)
                );

            graphPerception.Draw();

            ///////////////////////////////

            graphHaste.Clear();
            graphHaste.AddItemPair("Haste",
                $"{displayStats.BuffHaste(false):##0.##}%",
                100,
                displayStats.BuffHaste(false),
                displayStats.BuffHaste(true),
                GenericDataTooltip3(displayStats.BuffHaste(false), 100, displayStats.BuffHaste(true), "Haste"));

            graphToHit.Clear();
            graphToHit.AddItemPair("ToHit",
                $"{displayStats.BuffToHit:##0.##}%",
                0,
                displayStats.BuffToHit,
                GenericDataTooltip3(displayStats.BuffToHit, 0, displayStats.BuffToHit, "ToHit", "%", "", true));

            graphAccuracy.Clear();
            graphAccuracy.AddItemPair("Accuracy",
                $"{displayStats.BuffAccuracy:##0.##}%",
                0,
                displayStats.BuffAccuracy,
                GenericDataTooltip3(displayStats.BuffAccuracy, 0, displayStats.BuffAccuracy, "Accuracy", "%", "", true));

            graphDamage.Clear();
            graphDamage.AddItemPair("Damage",
                $"{displayStats.BuffDamage(false):##0.##}%",
                100,
                displayStats.BuffDamage(false),
                displayStats.BuffDamage(true),
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
            foreach (var m in MezList)
            {
                graphStatusProt.AddItemPair($"{m}",
                    $"{-MidsContext.Character.Totals.Mez[(int) m]:##0.##}",
                    0,
                    -MidsContext.Character.Totals.Mez[(int) m],
                    $"{-MidsContext.Character.Totals.Mez[(int) m]} Status protection to {m}");

                graphStatusRes.AddItemPair($"{m}",
                    $"{-MidsContext.Character.Totals.MezRes[(int) m]:##0.##}",
                    0,
                    -MidsContext.Character.Totals.MezRes[(int) m],
                    $"{-MidsContext.Character.Totals.MezRes[(int) m]} Status resistance to {m}");
            }

            graphStatusProt.Draw();
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
                    $"{cappedDebuffRes[i]:##0.##}",
                    0,
                    cappedDebuffRes[i],
                    uncappedDebuffRes[i],
                    GenericDataTooltip3(cappedDebuffRes[i], 0, uncappedDebuffRes[i], $"Debuff resistance to {DebuffEffectsList[i]}")
                );
            }

            graphDebuffRes.Draw();

            graphElusivity.Clear();
            for (var i = 0; i < damageVectors.Length; i++)
            {
                if (excludedResVectors.Contains(i))
                {
                    continue;
                }

                var elValue = (MidsContext.Character.Totals.Elusivity[i] + (MidsContext.Config.Inc.DisablePvE ? 0.4f : 0)) * 100;
                graphElusivity.AddItemPair(damageVectorsNames[i],
                    $"{elValue:##0.##}%",
                    0,
                    elValue,
                    $"{elValue:##0.##} Elusivity ({damageVectorsNames[i]})");
            }

            graphElusivity.Draw();

            //watch.Stop();
            //Debug.WriteLine($"frmTotalsV2.UpdateData(): {watch.ElapsedMilliseconds}ms");
        }

        private void ctlTotalsTabStrip1_TabClick(int index)
        {
            tabControl1.SelectedIndex = index;
        }

        private void frmTotalsV2_Resize(object sender, EventArgs e)
        {
            // ClientSize min: 620x846

            const int tabControlWidthPad = -8;
            const int tabControlHeightPad = 58;
            const int graphControlWidthPad = 20;
            const int pbCloseLocationXPad = 132;

            panel1.Size = ClientSize;
            tabControl1.Size = new Size(ClientSize.Width - tabControlWidthPad, ClientSize.Height - tabControlHeightPad);
            
            graphDef.Width = ClientSize.Width - graphControlWidthPad;
            graphRes.Width = ClientSize.Width - graphControlWidthPad;
            graphHP.Width = ClientSize.Width - graphControlWidthPad;
            graphEnd.Width = ClientSize.Width - graphControlWidthPad;

            graphMovement.Width = ClientSize.Width - graphControlWidthPad;
            graphPerception.Width = ClientSize.Width - graphControlWidthPad;
            graphHaste.Width = ClientSize.Width - graphControlWidthPad;
            graphToHit.Width = ClientSize.Width - graphControlWidthPad;
            graphAccuracy.Width = ClientSize.Width - graphControlWidthPad;
            graphDamage.Width = ClientSize.Width - graphControlWidthPad;
            graphEndRdx.Width = ClientSize.Width - graphControlWidthPad;
            graphThreat.Width = ClientSize.Width - graphControlWidthPad;

            graphStatusProt.Width = ClientSize.Width - graphControlWidthPad;
            graphStatusRes.Width = ClientSize.Width - graphControlWidthPad;

            graphDebuffRes.Width = ClientSize.Width - graphControlWidthPad;
            graphElusivity.Width = ClientSize.Width - graphControlWidthPad;

            pbClose.Location = pbClose.Location with {X = ClientSize.Width - pbCloseLocationXPad};

            // Prevent duplicate headers display (tiling) when stretching window horizontally
            ctlTotalsTabStrip1.Redraw();
        }
    }
}