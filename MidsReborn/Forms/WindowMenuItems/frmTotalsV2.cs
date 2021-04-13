using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Data_Classes;
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
        private readonly TabColorScheme _tabColors = new TabColorScheme();
        private IEnumerable<ctlLayeredBarPb> _barsList;
        private IEnumerable<BarLabel> _lvList;

        #region Enums sub-lists (groups)

        private readonly List<Enums.eDamage> DefenseDamageList = new List<Enums.eDamage>()
        {
            Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
            Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Psionic, Enums.eDamage.Melee,
            Enums.eDamage.Ranged, Enums.eDamage.AoE
        };

        private readonly List<Enums.eDamage> ResistanceDamageList = new List<Enums.eDamage>()
        {
            Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
            Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Toxic, Enums.eDamage.Psionic
        };

        private readonly List<Enums.eEffectType> MovementTypesList = new List<Enums.eEffectType>()
        {
            Enums.eEffectType.SpeedRunning, Enums.eEffectType.SpeedJumping, Enums.eEffectType.JumpHeight,
            Enums.eEffectType.SpeedFlying
        };

        private readonly List<Enums.eEffectType> PerceptionEffectsList = new List<Enums.eEffectType>()
        {
            Enums.eEffectType.StealthRadius, Enums.eEffectType.StealthRadiusPlayer, Enums.eEffectType.PerceptionRadius
        };

        private readonly List<Enums.eMez> MezList = new List<Enums.eMez>()
        {
            Enums.eMez.Held, Enums.eMez.Stunned, Enums.eMez.Sleep, Enums.eMez.Immobilized,
            Enums.eMez.Knockback, Enums.eMez.Repel, Enums.eMez.Confused, Enums.eMez.Terrorized,
            Enums.eMez.Taunt, Enums.eMez.Placate, Enums.eMez.Teleport
        };

        private readonly List<Enums.eEffectType> DebuffEffectsList = new List<Enums.eEffectType>()
        {
            Enums.eEffectType.Defense, Enums.eEffectType.Endurance, Enums.eEffectType.Recovery,
            Enums.eEffectType.PerceptionRadius, Enums.eEffectType.ToHit, Enums.eEffectType.RechargeTime,
            Enums.eEffectType.SpeedRunning, Enums.eEffectType.Regeneration
        };

        #endregion

        #region Controls querying

        private IEnumerable<Control> GetControlHierarchy(Control root)
        {
            Queue<Control> queue = new Queue<Control>();
            queue.Enqueue(root);
            do
            {
                var control = queue.Dequeue();
                yield return control;
                foreach (var child in control.Controls.OfType<Control>())
                {
                    queue.Enqueue(child);
                }
            } while (queue.Count > 0);
        }

        private IEnumerable<Control> GetControlHierarchy<T>(Control root)
        {
            Queue<Control> queue = new Queue<Control>();
            queue.Enqueue(root);
            do
            {
                var control = queue.Dequeue();
                if (control.GetType() == typeof(T)) yield return control;
                foreach (var child in control.Controls.OfType<Control>())
                {
                    queue.Enqueue(child);
                }
            } while (queue.Count > 0);
        }

        #endregion

        public frmTotalsV2(ref frmMain iParent)
        {
            try
            {
                FormClosed += frmTotalsV2_FormClosed;
                Load += OnLoad;

                KeepOnTop = true;
                InitializeComponent();
                SetLvTypes();
                BarsSetup();
                _myParent = iParent;
                foreach (var control in GetControlHierarchy(tabControlAdv2))
                {
                    if (!control.Name.Contains("bar")) continue;
                    //ctlLayeredBarPb bar = Controls.Find(control.Name, true).Cast<ctlLayeredBarPb>().FirstOrDefault();
                    var bar = (ctlLayeredBarPb) control;
                    bar.BarHover += Bar_Hover;
                }

                // Tab Foreground Colors don't stick in the designer.
                // Note: default colors will be used anyway.
                // This may only cause issues if a custom
                // Windows theme is in use.
                tabControlAdv2.SelectedTab.ForeColor = _tabColors.TabTextColor;
                tabControlAdv2.ActiveTabOutlineColor = _tabColors.TabOutlineColor;
                tabControlAdv2.InactiveTabOutlineColor = _tabColors.TabOutlineColor;
                for (var i = 0; i < tabControlAdv2.TabPages.Count; i++)
                {
                    tabControlAdv2.TabPages[i].ForeColor = _tabColors.TabTextColor;
                }

                //Set Tab Text Colors for Inactive Tabs
                /*for (var i = 0; i < tabControlAdv2.TabPages.Count; i++)
                {
                    if (i != tabControlAdv2.SelectedTab.TabIndex)
                    {
                        tabControlAdv2.TabPages[i].ForeColor = _tabColors.TabTextColor;
                    }
                }*/
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        #region Label values specific fields setup

        private void SetLvTypes()
        {
            lv1.Group = "Defense";
            lv1.FormatType = 0;
            lv2.Group = "Defense";
            lv2.FormatType = 0;
            lv3.Group = "Defense";
            lv3.FormatType = 0;
            lv4.Group = "Defense";
            lv4.FormatType = 0;
            lv5.Group = "Defense";
            lv5.FormatType = 0;
            lv6.Group = "Defense";
            lv6.FormatType = 0;
            lv7.Group = "Defense";
            lv7.FormatType = 0;
            lv8.Group = "Defense";
            lv8.FormatType = 0;
            lv9.Group = "Defense";
            lv9.FormatType = 0;
            lv10.Group = "Defense";
            lv10.FormatType = 0;

            lv11.Group = "Resistance";
            lv11.FormatType = 0;
            lv12.Group = "Resistance";
            lv12.FormatType = 0;
            lv13.Group = "Resistance";
            lv13.FormatType = 0;
            lv14.Group = "Resistance";
            lv14.FormatType = 0;
            lv15.Group = "Resistance";
            lv15.FormatType = 0;
            lv16.Group = "Resistance";
            lv16.FormatType = 0;
            lv17.Group = "Resistance";
            lv17.FormatType = 0;
            lv18.Group = "Resistance";
            lv18.FormatType = 0;

            lv19.Group = "HP";
            lv19.FormatType = 0;
            lv20.Group = "HP";
            lv20.FormatType = 1;

            lv21.Group = "Endurance";
            lv21.FormatType = 4;
            lv22.Group = "Endurance";
            lv22.FormatType = 4;
            lv23.Group = "Endurance";
            lv23.FormatType = 0;

            lv24.Group = "Movement";
            lv24.FormatType = 5;
            lv25.Group = "Movement";
            lv25.FormatType = 5;
            lv26.Group = "Movement";
            lv26.FormatType = 6;
            lv27.Group = "Movement";
            lv27.FormatType = 5;

            lv28.Group = "Perception";
            lv28.FormatType = 6;
            lv29.Group = "Perception";
            lv29.FormatType = 6;
            lv30.Group = "Perception";
            lv30.FormatType = 6;

            lv31.Group = "";
            lv31.FormatType = 0;
            lv32.Group = "";
            lv32.FormatType = 7;
            lv33.Group = "";
            lv33.FormatType = 7;
            lv34.Group = "";
            lv34.FormatType = 0;
            lv35.Group = "";
            lv35.FormatType = 7;
            lv36.Group = "";
            lv36.FormatType = 0;
            lv37.Group = "";
            lv37.FormatType = 0;

            lv38.Group = "Status Protection";
            lv38.FormatType = 3;
            lv39.Group = "Status Protection";
            lv39.FormatType = 3;
            lv40.Group = "Status Protection";
            lv40.FormatType = 3;
            lv41.Group = "Status Protection";
            lv41.FormatType = 3;
            lv42.Group = "Status Protection";
            lv42.FormatType = 3;
            lv43.Group = "Status Protection";
            lv43.FormatType = 3;
            lv44.Group = "Status Protection";
            lv44.FormatType = 3;
            lv45.Group = "Status Protection";
            lv45.FormatType = 3;
            lv46.Group = "Status Protection";
            lv46.FormatType = 3;
            lv47.Group = "Status Protection";
            lv47.FormatType = 3;
            lv48.Group = "Status Protection";
            lv48.FormatType = 3;

            lv49.Group = "Status Resistance";
            lv49.FormatType = 0;
            lv50.Group = "Status Resistance";
            lv50.FormatType = 0;
            lv51.Group = "Status Resistance";
            lv51.FormatType = 0;
            lv52.Group = "Status Resistance";
            lv52.FormatType = 0;
            lv53.Group = "Status Resistance";
            lv53.FormatType = 0;
            lv54.Group = "Status Resistance";
            lv54.FormatType = 0;
            lv55.Group = "Status Resistance";
            lv55.FormatType = 0;
            lv56.Group = "Status Resistance";
            lv56.FormatType = 0;
            lv57.Group = "Status Resistance";
            lv57.FormatType = 0;
            lv58.Group = "Status Resistance";
            lv58.FormatType = 0;
            lv59.Group = "Status Resistance";
            lv59.FormatType = 0;

            lv60.Group = "Debuff Resistance";
            lv60.FormatType = 0;
            lv61.Group = "Debuff Resistance";
            lv61.FormatType = 0;
            lv62.Group = "Debuff Resistance";
            lv62.FormatType = 0;
            lv63.Group = "Debuff Resistance";
            lv63.FormatType = 0;
            lv64.Group = "Debuff Resistance";
            lv64.FormatType = 0;
            lv65.Group = "Debuff Resistance";
            lv65.FormatType = 0;
            lv66.Group = "Debuff Resistance";
            lv66.FormatType = 0;
            lv67.Group = "Debuff Resistance";
            lv67.FormatType = 0;
        }

        #endregion

        private void BarsSetup()
        {
            var defenseGroupSettings = new List<(string name, Color color)> // 1-10
            {
                (name: "value", color: Color.Magenta)
            };

            var resistanceGroupSettings = new List<(string name, Color color)> // 11-18
            {
                (name: "value", color: Color.FromArgb(0, 192, 192)),
                (name: "uncapped", color: Color.FromArgb(255, 128, 128))
            };

            var regenSettings = new List<(string name, Color color)> // 19
            {
                (name: "value", color: Color.FromArgb(64, 255, 64)),
                (name: "base", color: Color.FromArgb(51, 204, 51)),
                (name: "uncapped", color: Color.FromArgb(28, 111, 28))
            };

            var hpSettings = new List<(string name, Color color)> // 20
            {
                (name: "value", color: Color.FromArgb(44, 180, 44)),
                (name: "base", color: Color.FromArgb(31, 130, 31)),
                (name: "uncapped", color: Color.FromArgb(10, 38, 10)),
                (name: "absorb", color: Color.Gainsboro)
            };

            var endRecSettings = new List<(string name, Color color)> // 21
            {
                (name: "value", color: Color.DodgerBlue),
                (name: "base", color: Color.FromArgb(24, 114, 204)),
                (name: "uncapped", color: Color.FromArgb(13, 63, 112))
            };

            var endUseSettings = new List<(string name, Color color)> // 22
            {
                (name: "value", color: Color.FromArgb(149, 203, 255))
            };

            var maxEndSettings = new List<(string name, Color color)> // 23
            {
                (name: "value", color: Color.FromArgb(59, 158, 255)),
                (name: "base", color: Color.FromArgb(47, 125, 204))
            };

            ////////////////////////////

            var movementSettings = new List<(string name, Color color)> // 24-27
            {
                (name: "value", color: Color.FromArgb(0, 192, 128)),
                (name: "base", color: Color.FromArgb(0, 140, 94)),
                (name: "uncapped", color: Color.FromArgb(0, 48, 32))
            };

            var perceptionSettings = new List<(string name, Color color)> // 28-30
            {
                (name: "value", color: Color.FromArgb(106, 121, 136)),
                (name: "base", color: Color.FromArgb(84, 95, 107)),
                (name: "uncapped", color: Color.FromArgb(46, 52, 59))
            };

            var hasteSettings = new List<(string name, Color color)> // 31
            {
                (name: "value", color: Color.FromArgb(255, 128, 0)),
                (name: "base", color: Color.FromArgb(204, 102, 0)),
                (name: "uncapped", color: Color.FromArgb(112, 56, 0))
            };

            var toHitSettings = new List<(string name, Color color)> // 32
            {
                (name: "value", color: Color.FromArgb(255, 255, 128))
            };

            var accuracySettings = new List<(string name, Color color)> // 33
            {
                (name: "value", color: Color.Yellow)
            };

            var damageSettings = new List<(string name, Color color)> // 34
            {
                (name: "value", color: Color.Red),
                (name: "base", color: Color.FromArgb(204, 0, 0)),
                (name: "uncapped", color: Color.FromArgb(112, 0, 0))
            };

            var endRdxSettings = new List<(string name, Color color)> // 35
            {
                (name: "value", color: Color.RoyalBlue)
            };

            var threatSettings = new List<(string name, Color color)> // 36
            {
                (name: "value", color: Color.MediumPurple),
                (name: "base", color: Color.FromArgb(113, 86, 168))
            };

            var elusivitySettings = new List<(string name, Color color)> // 37
            {
                (name: "value", color: Color.FromArgb(163, 1, 231))
            };

            ////////////////////////////

            var statusProtectionSettings = new List<(string name, Color color)> // 38-48
            {
                (name: "value", color: Color.FromArgb(255, 128, 0))
            };

            var statusResistanceSettings = new List<(string name, Color color)> // 49-59
            {
                (name: "value", color: Color.Yellow)
            };

            ////////////////////////////

            var debuffResistanceSettings = new List<(string name, Color color)> // 60-67
            {
                (name: "value", color: Color.Cyan),
                (name: "uncapped", color: Color.FromArgb(0, 90, 127))
            };

            bar1.AssignNames(defenseGroupSettings);
            bar2.AssignNames(defenseGroupSettings);
            bar3.AssignNames(defenseGroupSettings);
            bar4.AssignNames(defenseGroupSettings);
            bar5.AssignNames(defenseGroupSettings);
            bar6.AssignNames(defenseGroupSettings);
            bar7.AssignNames(defenseGroupSettings);
            bar8.AssignNames(defenseGroupSettings);
            bar9.AssignNames(defenseGroupSettings);
            bar10.AssignNames(defenseGroupSettings);

            bar11.AssignNames(resistanceGroupSettings);
            bar12.AssignNames(resistanceGroupSettings);
            bar13.AssignNames(resistanceGroupSettings);
            bar14.AssignNames(resistanceGroupSettings);
            bar15.AssignNames(resistanceGroupSettings);
            bar16.AssignNames(resistanceGroupSettings);
            bar17.AssignNames(resistanceGroupSettings);
            bar18.AssignNames(resistanceGroupSettings);

            bar19.AssignNames(regenSettings);
            bar20.AssignNames(hpSettings);

            bar21.AssignNames(endRecSettings);
            bar22.AssignNames(endUseSettings);
            bar23.AssignNames(maxEndSettings);


            bar24.AssignNames(movementSettings);
            bar25.AssignNames(movementSettings);
            bar26.AssignNames(movementSettings);
            bar27.AssignNames(movementSettings);

            bar28.AssignNames(perceptionSettings);
            bar29.AssignNames(perceptionSettings);
            bar30.AssignNames(perceptionSettings);

            bar31.AssignNames(hasteSettings);
            bar32.AssignNames(toHitSettings);
            bar33.AssignNames(accuracySettings);
            bar34.AssignNames(damageSettings);
            bar35.AssignNames(endRdxSettings);
            bar36.AssignNames(threatSettings);
            bar37.AssignNames(elusivitySettings);


            bar38.AssignNames(statusProtectionSettings);
            bar39.AssignNames(statusProtectionSettings);
            bar40.AssignNames(statusProtectionSettings);
            bar41.AssignNames(statusProtectionSettings);
            bar42.AssignNames(statusProtectionSettings);
            bar43.AssignNames(statusProtectionSettings);
            bar44.AssignNames(statusProtectionSettings);
            bar45.AssignNames(statusProtectionSettings);
            bar46.AssignNames(statusProtectionSettings);
            bar47.AssignNames(statusProtectionSettings);
            bar48.AssignNames(statusProtectionSettings);

            bar49.AssignNames(statusResistanceSettings);
            bar50.AssignNames(statusResistanceSettings);
            bar51.AssignNames(statusResistanceSettings);
            bar52.AssignNames(statusResistanceSettings);
            bar53.AssignNames(statusResistanceSettings);
            bar54.AssignNames(statusResistanceSettings);
            bar55.AssignNames(statusResistanceSettings);
            bar56.AssignNames(statusResistanceSettings);
            bar57.AssignNames(statusResistanceSettings);
            bar58.AssignNames(statusResistanceSettings);
            bar59.AssignNames(statusResistanceSettings);


            bar60.AssignNames(debuffResistanceSettings);
            bar61.AssignNames(debuffResistanceSettings);
            bar62.AssignNames(debuffResistanceSettings);
            bar63.AssignNames(debuffResistanceSettings);
            bar64.AssignNames(debuffResistanceSettings);
            bar65.AssignNames(debuffResistanceSettings);
            bar66.AssignNames(debuffResistanceSettings);
            bar67.AssignNames(debuffResistanceSettings);
        }

        private string UcFirst(string s)
        {
            return char.ToUpper(s[0]) + s.Substring(1).ToLower();
        }

        private void Bar_Hover(object sender)
        {
            var isPanel = false;
            ctlLayeredBarPb trigger;
            isPanel = false;
            trigger = (ctlLayeredBarPb) sender;

            var displayStats = MidsContext.Character.DisplayStats;
            var barGroup = trigger.Group;
            var barIndex = GetBarIndex(trigger);
            var vectorTypeIndex = string.IsNullOrEmpty(trigger.Group) ? -1 : GetBarVectorTypeIndex(barIndex, barGroup);
            var atName = MidsContext.Character.Archetype.DisplayName;
            string tooltipText;
            var barTypesNames = Enum.GetNames(typeof(Enums.eBarType));
            var r = new Regex(@"/([A-Z])/");
            barTypesNames = barTypesNames.Select(e => r.Replace(e, " $1").TrimStart()).ToArray();
            BarLabel lv;
            string percentageSign;
            bool plusSignEnabled;
            string movementUnit;
            var values = trigger.GetValues();
            var sEnableBase = values.ContainsKey("base");
            var sEnableOverCap = values.ContainsKey("uncapped");
            var sEnableOverlay1 = values.ContainsKey("absorb");
            var sValueMainBar = values.ContainsKey("value") ? values["value"] : 0;
            var sValueBase = sEnableBase ? values["base"] : 0;
            var sValueOverCap = sEnableOverCap ? values["uncapped"] : 0;
            var sOverlay1 = sEnableOverlay1 ? values["absorb"] : 0;
            
            float valueMainBar = 0;
            float valueBase = 0;
            float valueOverCap = 0;

            switch (barGroup)
            {
                case "Defense":
                    tooltipText =
                        $"{sValueMainBar:##0.###}% {FormatVectorType(typeof(Enums.eDamage), vectorTypeIndex)} defense";
                    break;

                case "Resistance":
                    tooltipText = sValueOverCap > sValueMainBar
                        ? $"{sValueOverCap:##0.##}% {FormatVectorType(typeof(Enums.eDamage), vectorTypeIndex)} resistance (capped at {MidsContext.Character.Archetype.ResCap * 100:##0.##}%)"
                        : $"{sValueMainBar:##0.##}% {FormatVectorType(typeof(Enums.eDamage), vectorTypeIndex)} resistance ({atName} resistance cap: {MidsContext.Character.Archetype.ResCap * 100:##0.##}%)";
                    break;

                case "HP" when barIndex == (int) Enums.eBarType.MaxHPAbsorb:
                    tooltipText = (sValueOverCap > sValueMainBar
                                      ? $"{sValueOverCap:##0.##} HP, capped at {MidsContext.Character.Archetype.HPCap} HP"
                                      : $"{sValueMainBar:##0.##} HP ({atName} HP cap: {MidsContext.Character.Archetype.HPCap} HP)"
                                      
                                  ) +
                                  $"\r\nBase: {sValueBase:##0.##} HP" +
                                  (sOverlay1 != 0
                                      ? $"\r\nAbsorb: {sOverlay1:##0.##} ({(sOverlay1 / sValueBase * 100):##0.##}% of base HP)"
                                      : "");
                    break;

                case "HP" when barIndex == (int)Enums.eBarType.Regeneration:
                    tooltipText = (sValueOverCap > sValueMainBar
                                      ? $"{sValueOverCap:##0.##}% {barTypesNames[barIndex]}, capped at {sValueMainBar:##0.##}%"
                                      : $"{sValueMainBar:##0.##}% {barTypesNames[barIndex]}"
                                  ) +
                                  $" ({MidsContext.Character.DisplayStats.HealthRegenHPPerSec:##0.##} HP/s)" +
                                  (sValueBase > 0 ? $"\r\nBase: {sValueBase:##0.##}%" : "");
                    break;

                case "Endurance" when barIndex == (int) Enums.eBarType.EndRec:
                    tooltipText = (sValueOverCap > sValueMainBar
                                      ? $"{sValueOverCap:##0.##}/s End., capped at {MidsContext.Character.Archetype.RecoveryCap * 100:##0.##}%"
                                      : $"{sValueMainBar:##0.##}/s End. ({atName} End. recovery cap: {MidsContext.Character.Archetype.RecoveryCap * 100:##0.##}%)"
                                  ) +
                                  $"\r\nBase: {sValueBase:##0.##}/s";
                    break;

                case "Endurance" when barIndex == (int) Enums.eBarType.EndUse:
                    tooltipText = $"{sValueMainBar:##0.##}/s End. (Net gain: {displayStats.EnduranceRecoveryNet:##0.##}/s)";
                    break;

                case "Endurance" when barIndex == (int) Enums.eBarType.MaxEnd:
                    tooltipText = $"{sValueMainBar:##0.##} Maximum Endurance (base: {sValueBase:##0.##})";
                    break;

                case "Movement":
                    lv = FetchLv(barIndex);
                    movementUnit = lv.FormatType == 5
                        ? clsConvertibleUnitValue.FormatSpeedUnit(MidsContext.Config.SpeedFormat)
                        : clsConvertibleUnitValue.FormatDistanceUnit(MidsContext.Config.SpeedFormat);
                    valueMainBar = 0;
                    valueBase = 0;
                    valueOverCap = 0;
                    switch (barIndex)
                    {
                        case (int)Enums.eBarType.RunSpeed:
                            valueMainBar = displayStats.MovementRunSpeed(MidsContext.Config.SpeedFormat, false);
                            valueBase = displayStats.Speed(Statistics.BaseRunSpeed, MidsContext.Config.SpeedFormat);
                            valueOverCap = displayStats.MovementRunSpeed(MidsContext.Config.SpeedFormat, true);
                            break;

                        case (int)Enums.eBarType.JumpSpeed:
                            valueMainBar = displayStats.MovementJumpSpeed(MidsContext.Config.SpeedFormat, false);
                            valueBase = displayStats.Speed(Statistics.BaseJumpSpeed, MidsContext.Config.SpeedFormat);
                            valueOverCap = displayStats.MovementJumpSpeed(MidsContext.Config.SpeedFormat, true);
                            break;

                        case (int)Enums.eBarType.JumpHeight:
                            valueMainBar = displayStats.MovementJumpHeight(MidsContext.Config.SpeedFormat);
                            valueBase = displayStats.Speed(Statistics.BaseJumpHeight, MidsContext.Config.SpeedFormat);
                            valueOverCap = displayStats.MovementJumpHeight(MidsContext.Config.SpeedFormat);
                            break;

                        case (int)Enums.eBarType.FlySpeed:
                            valueMainBar = displayStats.MovementFlySpeed(MidsContext.Config.SpeedFormat, false);
                            valueBase = displayStats.MovementFlySpeed(MidsContext.Config.SpeedFormat, false) == 0
                                ? 0
                                : displayStats.Speed(Statistics.BaseFlySpeed, MidsContext.Config.SpeedFormat);
                            valueOverCap = displayStats.MovementFlySpeed(MidsContext.Config.SpeedFormat, true);
                            break;
                    }
                        
                    tooltipText = (valueOverCap > valueMainBar
                                      ? $"{valueOverCap:##0.##} {movementUnit} {barTypesNames[barIndex]}, capped at {valueMainBar:##0.##} {movementUnit}"
                                      : $"{valueMainBar:##0.##} {movementUnit} {barTypesNames[barIndex]}"
                                  ) +
                                  (valueBase > 0
                                      ? $"\r\nBase: {valueBase:##0.##} {movementUnit}"
                                      : "");
                    break;

                case "Status Protection":
                    tooltipText =
                        $"{Math.Abs(sValueMainBar):##0.##} {UcFirst(trigger.Group)} to {FormatVectorType(typeof(Enums.eMez), vectorTypeIndex)}";
                    break;

                case "Status Resistance":
                    tooltipText =
                        $"{sValueMainBar:##0.##}% {UcFirst(trigger.Group)} to {FormatVectorType(typeof(Enums.eMez), vectorTypeIndex)}";
                    break;

                case "Debuff Resistance":
                    tooltipText = sValueOverCap > sValueMainBar
                        ? $"{sValueOverCap:##0.##}% {UcFirst(trigger.Group)} to {FormatVectorType(typeof(Enums.eEffectType), vectorTypeIndex)}, capped at {sValueMainBar:##0.##}%"
                        : $"{sValueMainBar:##0.##}% {UcFirst(trigger.Group)} to {FormatVectorType(typeof(Enums.eEffectType), vectorTypeIndex)}";
                    break;

                // Triple bar main + base + overcap
                case "Perception" when sEnableBase && sEnableOverCap:
                case "" when sEnableBase && sEnableOverCap:
                    lv = FetchLv(barIndex);
                    percentageSign = lv.FormatType == 0 || lv.FormatType == 7 ? "%" : "";
                    plusSignEnabled = lv.FormatType == 2 || lv.FormatType == 7;
                    movementUnit = lv.FormatType switch
                    {
                        5 => $" {clsConvertibleUnitValue.FormatSpeedUnit(MidsContext.Config.SpeedFormat)}",
                        6 => $" {clsConvertibleUnitValue.FormatDistanceUnit(MidsContext.Config.SpeedFormat)}",
                        _ => ""
                    };
                    switch (barIndex)
                    {
                        case (int)Enums.eBarType.StealthPvE:
                            valueMainBar = displayStats.Distance(MidsContext.Character.Totals.StealthPvE, MidsContext.Config.SpeedFormat);
                            valueBase = 0;
                            valueOverCap = valueMainBar;
                            break;

                        case (int)Enums.eBarType.StealthPvP:
                            valueMainBar = displayStats.Distance(MidsContext.Character.Totals.StealthPvP, MidsContext.Config.SpeedFormat);
                            valueBase = 0;
                            valueOverCap = valueMainBar;
                            break;

                        case (int)Enums.eBarType.Perception:
                            valueMainBar = displayStats.Distance(displayStats.Perception(false), MidsContext.Config.SpeedFormat);
                            valueBase = displayStats.Distance(Statistics.BasePerception, MidsContext.Config.SpeedFormat);
                            valueOverCap = displayStats.Distance(displayStats.Perception(true), MidsContext.Config.SpeedFormat);
                            break;
                    }

                    tooltipText = (valueOverCap > valueMainBar
                                      ? $"{(plusSignEnabled && valueOverCap > 0 ? "+" : "")}{valueOverCap:##0.##}{percentageSign}{movementUnit} {barTypesNames[barIndex]}, capped at {(plusSignEnabled && valueMainBar > 0 ? "+" : "")}{valueMainBar:##0.##}{percentageSign}"
                                      : $"{(plusSignEnabled && valueMainBar > 0 ? "+" : "")}{valueMainBar:##0.##}{percentageSign}{movementUnit} {barTypesNames[barIndex]}"
                                  ) +
                                  (valueBase > 0
                                      ? $"\r\nBase: {(plusSignEnabled && valueBase > 0 ? "+" : "")}{valueBase:##0.##}{percentageSign}{movementUnit}"
                                      : "");
                    break;

                // Dual bar main + overcap
                case "Perception" when !sEnableBase && sEnableOverCap:
                case "" when !sEnableBase && sEnableOverCap:
                    lv = FetchLv(barIndex);
                    percentageSign = lv.FormatType == 0 || lv.FormatType == 7 ? "%" : "";
                    plusSignEnabled = lv.FormatType == 2 || lv.FormatType == 7;
                    tooltipText = sValueOverCap > sValueMainBar
                        ? $"{(plusSignEnabled && sValueOverCap > 0 ? "+" : "")}{sValueOverCap:##0.##}{percentageSign} {barTypesNames[barIndex]}, capped at {(plusSignEnabled && sValueMainBar > 0 ? "+" : "")}{sValueMainBar:##0.##}{percentageSign}"
                        : $"{(plusSignEnabled && sValueMainBar > 0 ? "+" : "")}{sValueMainBar:##0.##}{percentageSign} {barTypesNames[barIndex]}";
                    break;

                // Dual bar main + base
                case "" when sEnableBase && !sEnableOverCap:
                    lv = FetchLv(barIndex);
                    percentageSign = lv.FormatType == 0 || lv.FormatType == 7 ? "%" : "";
                    plusSignEnabled = lv.FormatType == 2 || lv.FormatType == 7;
                    tooltipText =
                        $"{(plusSignEnabled && sValueMainBar > 0 ? "+" : "")}{sValueMainBar:##0.##}{percentageSign} {barTypesNames[barIndex]}" +
                        (sValueBase > 0
                            ? $"\r\nBase: {(plusSignEnabled && sValueBase > 0 ? "+" : "")}{sValueBase:##0.##}{percentageSign}"
                            : "");
                    break;

                // Single bar
                case "" when !sEnableBase && !sEnableOverCap:
                    lv = FetchLv(barIndex);
                    percentageSign = lv.FormatType == 0 || lv.FormatType == 7 ? "%" : "";
                    plusSignEnabled = lv.FormatType == 2 || lv.FormatType == 7;
                    tooltipText =
                        $"{(plusSignEnabled && sValueMainBar > 0 ? "+" : "")}{sValueMainBar:##0.##}{percentageSign} {barTypesNames[barIndex]}";
                    break;

                default:
                    tooltipText = "";
                    break;
            }
            
            trigger.SetTip(tooltipText);
        }

        private int GetBarVectorTypeIndex(int barIndex, string barGroup)
        {
            return barGroup switch
            {
                "Defense" => (int) DefenseDamageList[barIndex],
                "Resistance" => (int) ResistanceDamageList[barIndex - 10],
                "Movement" => (int) MovementTypesList[barIndex - 23],
                "Perception" => (int) PerceptionEffectsList[barIndex - 27],
                "Status Protection" => (int) MezList[barIndex - 37],
                "Status Resistance" => (int) MezList[barIndex - 48],
                "Debuff Resistance" => (int) DebuffEffectsList[barIndex - 59],
                _ => -1
            };
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

        private int GetBarIndex(ctlLayeredBarPb bar)
        {
            return Convert.ToInt32(bar.Name.Substring(3)) - 1;
        }

        private void SetBarsBulk(IEnumerable<Control> controlsList, string group, List<float> values)
        {
            List<ctlLayeredBarPb> barsGroup = controlsList
                .Cast<ctlLayeredBarPb>()
                .Where(e => e.Group == group)
                .ToList();

            barsGroup.Sort((a, b) => GetBarIndex(a).CompareTo(GetBarIndex(b)));

            for (var i = 0; i < barsGroup.Count; i++)
            {
                SetBarSingle(barsGroup[i], new List<float> {values[i]});
            }
        }

        private void SetBarsBulk(IEnumerable<Control> controlsList, string group, List<List<float>> values)
        {
            List<ctlLayeredBarPb> barsGroup = controlsList
                .Cast<ctlLayeredBarPb>()
                .Where(e => e.Group == group)
                .ToList();

            barsGroup.Sort((a, b) => GetBarIndex(a).CompareTo(GetBarIndex(b)));

            for (var i = 0; i < barsGroup.Count; i++)
            {
                SetBarSingle(barsGroup[i], values[i]);
            }
        }

        private void SetBarSingle(ctlLayeredBarPb bar, List<float> values)
        {
            bar.AssignValues(values);
        }

        private void SetBarSingle(Enums.eBarType barType, List<float> values)
        {
            SetBarSingle(FetchBar((int) barType), values);
        }

        private int GetLvIndex(BarLabel lv)
        {
            return Convert.ToInt32(lv.Name.Substring(2)) - 1;
        }

        private void SetLvsBulk(IEnumerable<Control> controlsList, string group, float[] values)
        {
            List<BarLabel> lvGroup = controlsList
                .Cast<BarLabel>()
                .Where(e => e.Group == group)
                .ToList();

            lvGroup.Sort((a, b) => GetLvIndex(a).CompareTo(GetLvIndex(b)));

            for (var i = 0; i < lvGroup.Count; i++)
            {
                SetLvSingle(lvGroup[i], values[i]);
            }
        }

        private void SetLvSingle(BarLabel lv, float value)
        {
            lv.Text = Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private void SetLvSingle(BarLabel lv, string valueText)
        {
            lv.Text = valueText;
        }

        private void SetLvSingle(Enums.eBarType barType, float value)
        {
            FetchLv(barType).SetText(value);
        }

        private void SetLvSingle(Enums.eBarType barType, string valueText)
        {
            FetchLv(barType).SetText(valueText);
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

            string titleTxt;
            var epicPowersetIndex = GetEpicPowersetIndex();
            var buildFileName = Path.GetFileName(frm._myParent.GetBuildFile());

            switch (MidsContext.Config.TotalsWindowTitleStyle)
            {
                case ConfigData.ETotalsWindowTitleStyle.Generic:
                    frm.Text = "Totals for Self";
                    break;

                case ConfigData.ETotalsWindowTitleStyle.CharNameAtPowersets:
                    titleTxt = (!string.IsNullOrWhiteSpace(MidsContext.Character.Name)
                                   ? MidsContext.Character.Name + " - "
                                   : "")
                               + MidsContext.Character.Archetype.DisplayName;
                    if (!MidsContext.Character.IsKheldian)
                    {
                        titleTxt += " [ " +
                                    MidsContext.Character.Powersets[0].DisplayName + " / " +
                                    MidsContext.Character.Powersets[1].DisplayName +
                                    (epicPowersetIndex != -1
                                        ? " / " + MidsContext.Character.Powersets[epicPowersetIndex].DisplayName
                                        : "") +
                                    " ]";
                    }

                    frm.Text = "Totals - " + titleTxt;
                    break;

                case ConfigData.ETotalsWindowTitleStyle.BuildFileAtPowersets:
                    titleTxt = "";
                    if (!MidsContext.Character.IsKheldian)
                    {
                        titleTxt += MidsContext.Character.Powersets[0].DisplayName + " / " +
                                    MidsContext.Character.Powersets[1].DisplayName +
                                    (epicPowersetIndex != -1
                                        ? " / " + MidsContext.Character.Powersets[epicPowersetIndex].DisplayName
                                        : "") + " ";
                    }

                    titleTxt += MidsContext.Character.Archetype.DisplayName +
                                (MainModule.MidsController.Toon != null && !string.IsNullOrEmpty(buildFileName)
                                    ? " [" + buildFileName + "]"
                                    : "");

                    frm.Text = "Totals - " + titleTxt;
                    break;

                case ConfigData.ETotalsWindowTitleStyle.CharNameBuildFile:
                    titleTxt = !string.IsNullOrWhiteSpace(MidsContext.Character.Name)
                        ? MidsContext.Character.Name + " "
                        : "";
                    if (titleTxt == "")
                    {
                        titleTxt = !string.IsNullOrEmpty(buildFileName) ? buildFileName : "";
                    }
                    else
                    {
                        titleTxt += !string.IsNullOrEmpty(buildFileName) ? "[" + buildFileName + "]" : "";
                    }

                    frm.Text = titleTxt == "" ? "Totals for Self" : "Totals - " + titleTxt;
                    break;

                default:
                    frm.Text = "Totals for Self";
                    break;
            }
        }

        public override void Refresh()
        {
            if (MidsContext.Character.IsHero())
            {
                tabControlAdv2.InactiveTabColor = _tabColors.HeroInactiveTabColor;
                tabControlAdv2.TabPanelBackColor = _tabColors.HeroInactiveTabColor;
                tabControlAdv2.FixedSingleBorderColor = _tabColors.HeroBorderColor;
                tabControlAdv2.ActiveTabColor = _tabColors.HeroActiveTabColor;
            }
            else
            {
                tabControlAdv2.InactiveTabColor = _tabColors.VillainInactiveTabColor;
                tabControlAdv2.TabPanelBackColor = _tabColors.VillainInactiveTabColor;
                tabControlAdv2.FixedSingleBorderColor = _tabColors.VillainBorderColor;
                tabControlAdv2.ActiveTabColor = _tabColors.VillainActiveTabColor;
            }

            base.Refresh();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            CenterToParent();
            Refresh();
            SetTitle(this);
            SetUnitRadioButtons();
            _barsList = GetControlHierarchy<ctlLayeredBarPb>(tabControlAdv2).Cast<ctlLayeredBarPb>().ToList();
            _lvList = GetControlHierarchy<BarLabel>(tabControlAdv2).Cast<BarLabel>().ToList();
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
                extendedBitmap.Graphics.DrawImage(_myParent.Drawing.bxPower[index].Bitmap, destRect, 0, 0,
                    rectangle.Width, rectangle.Height, GraphicsUnit.Pixel);
            else
                extendedBitmap.Graphics.DrawImage(_myParent.Drawing.bxPower[index].Bitmap, destRect, 0, 0,
                    rectangle.Width, rectangle.Height, GraphicsUnit.Pixel, _myParent.Drawing.pImageAttributes);
            var height = bFont.GetHeight(e.Graphics) + 2;
            var Bounds = new RectangleF(0, (22 - height) / 2, 105, height);
            var graphics = extendedBitmap.Graphics;
            clsDrawX.DrawOutlineText(iStr, Bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1, graphics);
            e.Graphics.DrawImage(extendedBitmap.Bitmap, 0, 0);
        }

        #endregion

        #region Bars/Labels indexes

        private ctlLayeredBarPb FetchBar(int n)
        {
            return n switch
            {
                0 => bar1,
                1 => bar2,
                2 => bar3,
                3 => bar4,
                4 => bar5,
                5 => bar6,
                6 => bar7,
                7 => bar8,
                8 => bar9,
                9 => bar10,
                10 => bar11,
                11 => bar12,
                12 => bar13,
                13 => bar14,
                14 => bar15,
                15 => bar16,
                16 => bar17,
                17 => bar18,
                18 => bar19,
                19 => bar20,
                20 => bar21,
                21 => bar22,
                22 => bar23,
                23 => bar24,
                24 => bar25,
                25 => bar26,
                26 => bar27,
                27 => bar28,
                28 => bar29,
                29 => bar30,
                30 => bar31,
                31 => bar32,
                32 => bar33,
                33 => bar34,
                34 => bar35,
                35 => bar36,
                36 => bar37,
                37 => bar38,
                38 => bar39,
                39 => bar40,
                40 => bar41,
                41 => bar42,
                42 => bar43,
                43 => bar44,
                44 => bar45,
                45 => bar46,
                46 => bar47,
                47 => bar48,
                48 => bar49,
                49 => bar50,
                50 => bar51,
                51 => bar52,
                52 => bar53,
                53 => bar54,
                54 => bar55,
                55 => bar56,
                56 => bar57,
                57 => bar58,
                58 => bar59,
                59 => bar60,
                60 => bar61,
                61 => bar62,
                62 => bar63,
                63 => bar64,
                64 => bar65,
                65 => bar66,
                66 => bar67,
                _ => bar1
            };
        }

        private ctlLayeredBarPb FetchBar(Enums.eBarType barType)
        {
            return FetchBar((int) barType);
        }

        private BarLabel FetchLv(int n)
        {
            return n switch
            {
                0 => lv1,
                1 => lv2,
                2 => lv3,
                3 => lv4,
                4 => lv5,
                5 => lv6,
                6 => lv7,
                7 => lv8,
                8 => lv9,
                9 => lv10,
                10 => lv11,
                11 => lv12,
                12 => lv13,
                13 => lv14,
                14 => lv15,
                15 => lv16,
                16 => lv17,
                17 => lv18,
                18 => lv19,
                19 => lv20,
                20 => lv21,
                21 => lv22,
                22 => lv23,
                23 => lv24,
                24 => lv25,
                25 => lv26,
                26 => lv27,
                27 => lv28,
                28 => lv29,
                29 => lv30,
                30 => lv31,
                31 => lv32,
                32 => lv33,
                33 => lv34,
                34 => lv35,
                35 => lv36,
                36 => lv37,
                37 => lv38,
                38 => lv39,
                39 => lv40,
                40 => lv41,
                41 => lv42,
                42 => lv43,
                43 => lv44,
                44 => lv45,
                45 => lv46,
                46 => lv47,
                47 => lv48,
                48 => lv49,
                49 => lv50,
                50 => lv51,
                51 => lv52,
                52 => lv53,
                53 => lv54,
                54 => lv55,
                55 => lv56,
                56 => lv57,
                57 => lv58,
                58 => lv59,
                59 => lv60,
                60 => lv61,
                61 => lv62,
                62 => lv63,
                63 => lv64,
                64 => lv65,
                65 => lv66,
                66 => lv67,
                _ => lv1
            };
        }

        private BarLabel FetchLv(Enums.eBarType barType)
        {
            return FetchLv((int) barType);
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

        private List<List<float>> ToCombinedList(List<float> l1, List<float> l2)
        {
            var ret = new List<List<float>>();
            for (var i = 0; i < l1.Count; i++)
            {
                ret = ret.Append(new List<float> {l1[i], l2[i]}).ToList();
            }

            return ret;
        }

        private List<List<float>> ToCombinedList(List<float> l1, List<float> l2, List<float> l3)
        {
            var ret = new List<List<float>>();
            for (var i = 0; i < l1.Count; i++)
            {
                ret = ret.Append(new List<float> {l1[i], l2[i], l3[i]}).ToList();
            }

            return ret;
        }

        public void UpdateData()
        {
            //pbClose.Refresh();
            //pbTopMost.Refresh();
            var uncappedStats = MidsContext.Character.Totals;
            //var cappedStats = MidsContext.Character.TotalsCapped;
            var displayStats = MidsContext.Character.DisplayStats;
            //var watch = Stopwatch.StartNew();
            tabControlAdv2.SuspendLayout();

            Debug.WriteLine($"MaxFlySpeed: {uncappedStats.MaxFlySpd}, MaxRunSpeed: {uncappedStats.MaxRunSpd}, MaxJumpSpeed: {uncappedStats.MaxJumpSpd}");
            Debug.WriteLine($"Base fly speed: {Statistics.BaseFlySpeed}, Base run speed: {Statistics.BaseRunSpeed}, Base jump speed: {Statistics.BaseJumpSpeed}");

            #region Bars setup

            SetBarsBulk(
                _barsList,
                "Defense",
                DefenseDamageList.Cast<int>().Select(t => displayStats.Defense(t)).ToList()
            );

            SetBarsBulk(
                _barsList,
                "Resistance",
                ToCombinedList(
                    ResistanceDamageList.Cast<int>().Select(t => displayStats.DamageResistance(t, false)).ToList(),
                    ResistanceDamageList.Cast<int>().Select(t => displayStats.DamageResistance(t, true)).ToList())
            );

            SetBarSingle(Enums.eBarType.Regeneration,
                new List<float> {
                    displayStats.HealthRegenPercent(false),
                    100,
                    displayStats.HealthRegenPercent(true)
            });
            
            SetBarSingle(Enums.eBarType.MaxHPAbsorb,
                new List<float> {
                    displayStats.HealthHitpointsNumeric(false),
                    MidsContext.Character.Archetype.Hitpoints,
                    displayStats.HealthHitpointsNumeric(true),
                    Math.Min(displayStats.Absorb, MidsContext.Character.Archetype.Hitpoints)
            });

            SetBarSingle(Enums.eBarType.EndRec,
                new List<float> {
                    displayStats.EnduranceRecoveryNumeric,
                    MidsContext.Character.Archetype.BaseRecovery * displayStats.EnduranceMaxEnd / 60,
                    displayStats.EnduranceRecoveryNumericUncapped
            });

            SetBarSingle(Enums.eBarType.EndUse, new List<float> { displayStats.EnduranceUsage });
            SetBarSingle(Enums.eBarType.MaxEnd, new List<float> { displayStats.EnduranceMaxEnd, 100 });

            ///////////////////////////////

            SetBarsBulk(
                _barsList,
                "Movement",
                ToCombinedList(
                    new List<float>
                    {
                        displayStats.MovementRunSpeed(Enums.eSpeedMeasure.FeetPerSecond, false),
                        displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.FeetPerSecond, false),
                        displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond),
                        displayStats.MovementFlySpeed(Enums.eSpeedMeasure.MilesPerHour, false)
                    },
                    new List<float> {
                        displayStats.Speed(Statistics.BaseRunSpeed, Enums.eSpeedMeasure.FeetPerSecond),
                        displayStats.Speed(Statistics.BaseJumpSpeed, Enums.eSpeedMeasure.FeetPerSecond),
                        displayStats.Speed(Statistics.BaseJumpHeight, Enums.eSpeedMeasure.FeetPerSecond),
                        displayStats.MovementFlySpeed(Enums.eSpeedMeasure.MilesPerHour, false) == 0
                            ? 0
                            : displayStats.Speed(Statistics.BaseFlySpeed, Enums.eSpeedMeasure.MilesPerHour)},
                    new List<float>
                    {
                        displayStats.MovementRunSpeed(Enums.eSpeedMeasure.FeetPerSecond, true),
                        displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.FeetPerSecond, true),
                        displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond),
                        displayStats.MovementFlySpeed(Enums.eSpeedMeasure.MilesPerHour, true)
                    }
                )
            );

            ///////////////////////////////

            SetBarsBulk(
                _barsList,
                "Perception",
                ToCombinedList(
                    new List<float>
                    {
                        MidsContext.Character.Totals.StealthPvE,
                        MidsContext.Character.Totals.StealthPvP,
                        displayStats.Perception(false)
                    },
                    new List<float>
                    {
                        0,
                        0,
                        Statistics.BasePerception
                    },
                    new List<float>
                    {
                        0, //MidsContext.Character.Totals.StealthPvE,
                        0, //MidsContext.Character.Totals.StealthPvP,
                        displayStats.Perception(true)
                    }
                )
            );

            ///////////////////////////////

            SetBarSingle(Enums.eBarType.Haste, new List<float>
            {
                displayStats.BuffHaste(false),
                100,
                displayStats.BuffHaste(true)
            });

            SetBarSingle(Enums.eBarType.ToHit, new List<float>
            {
                displayStats.BuffToHit
            });

            SetBarSingle(Enums.eBarType.Accuracy, new List<float>
            {
                displayStats.BuffAccuracy
            });

            SetBarSingle(Enums.eBarType.Damage, new List<float>
            {
                displayStats.BuffDamage(false),
                100,
                displayStats.BuffDamage(true)
            });

            SetBarSingle(Enums.eBarType.EndRdx, new List<float>
            {
                displayStats.BuffEndRdx
            });

            SetBarSingle(Enums.eBarType.ThreatLevel, new List<float>
            {
                displayStats.ThreatLevel,
                MidsContext.Character.Archetype.BaseThreat * 100
            });

            SetBarSingle(Enums.eBarType.Elusivity, new List<float>
            {
                MidsContext.Character.Totals.Elusivity * 100
            });

            ///////////////////////////////

            SetBarsBulk(
                _barsList,
                "Status Protection",
                MezList.Select(m => -MidsContext.Character.Totals.Mez[(int) m]).ToList()
            );

            SetBarsBulk(
                _barsList,
                "Status Resistance",
                MezList.Select(m => MidsContext.Character.Totals.MezRes[(int) m]).ToList()
            );

            ///////////////////////////////

            var cappedDebuffRes = DebuffEffectsList.Select(e => Math.Min(
                    e == Enums.eEffectType.Defense
                        ? Statistics.MaxDefenseDebuffRes
                        : Statistics.MaxGenericDebuffRes,
                    MidsContext.Character.Totals.DebuffRes[(int) e]))
                .ToList();

            var uncappedDebuffRes = DebuffEffectsList.Select(e => MidsContext.Character.Totals.DebuffRes[(int) e]).ToList();

            SetBarsBulk(
                _barsList,
                "Debuff Resistance",
                ToCombinedList(cappedDebuffRes, uncappedDebuffRes)
            );

            #endregion

            #region Labels setup

            SetLvsBulk(
                _lvList,
                "Defense",
                DefenseDamageList.Cast<int>().Select(t => displayStats.Defense(t)).ToArray()
            );

            SetLvsBulk(
                _lvList,
                "Resistance",
                ResistanceDamageList.Cast<int>().Select(t => displayStats.DamageResistance(t, false)).ToArray()
            );

            SetLvSingle(Enums.eBarType.Regeneration, displayStats.HealthRegenPercent(false));
            SetLvSingle(Enums.eBarType.MaxHPAbsorb, displayStats.HealthHitpointsNumeric(false));
            SetLvSingle(Enums.eBarType.EndRec, displayStats.EnduranceRecoveryNumeric);
            SetLvSingle(Enums.eBarType.EndUse, displayStats.EnduranceUsage);
            SetLvSingle(Enums.eBarType.MaxEnd, displayStats.EnduranceMaxEnd);

            ///////////////////////////////

            SetLvsBulk(
                _lvList,
                "Movement",
                new[]
                {
                    displayStats.MovementRunSpeed(MidsContext.Config.SpeedFormat, false),
                    displayStats.MovementJumpSpeed(MidsContext.Config.SpeedFormat, false),
                    displayStats.MovementJumpHeight(MidsContext.Config.SpeedFormat),
                    displayStats.MovementFlySpeed(MidsContext.Config.SpeedFormat, false)
                }
            );

            ///////////////////////////////

            SetLvsBulk(
                _lvList,
                "Perception",
                new[]
                {
                    displayStats.Distance(MidsContext.Character.Totals.StealthPvE, MidsContext.Config.SpeedFormat),
                    displayStats.Distance(MidsContext.Character.Totals.StealthPvP, MidsContext.Config.SpeedFormat),
                    displayStats.Distance(displayStats.Perception(false), MidsContext.Config.SpeedFormat)
                }
            );

            ///////////////////////////////

            SetLvSingle(Enums.eBarType.Haste, displayStats.BuffHaste(false));
            SetLvSingle(Enums.eBarType.ToHit, displayStats.BuffToHit);
            SetLvSingle(Enums.eBarType.Accuracy, displayStats.BuffAccuracy);
            SetLvSingle(Enums.eBarType.Damage, displayStats.BuffDamage(false));
            SetLvSingle(Enums.eBarType.EndRdx, displayStats.BuffEndRdx);
            SetLvSingle(Enums.eBarType.ThreatLevel, displayStats.ThreatLevel);
            SetLvSingle(Enums.eBarType.Elusivity, MidsContext.Character.Totals.Elusivity * 100);

            ///////////////////////////////

            SetLvsBulk(
                _lvList,
                "Status Protection",
                MezList.Select(m => -MidsContext.Character.Totals.Mez[(int) m]).ToArray()
            );

            SetLvsBulk(
                _lvList,
                "Status Resistance",
                MezList.Select(m => MidsContext.Character.Totals.MezRes[(int) m]).ToArray()
            );

            ///////////////////////////////

            SetLvsBulk(
                _lvList,
                "Debuff Resistance",
                DebuffEffectsList.Select(e => Math.Min(
                        e == Enums.eEffectType.Defense
                            ? Statistics.MaxDefenseDebuffRes
                            : Statistics.MaxGenericDebuffRes,
                        MidsContext.Character.Totals.DebuffRes[(int)e]))
                    .ToArray()
            );

            #endregion

            tabControlAdv2.ResumeLayout();

            //watch.Stop();
            //Debug.WriteLine($"frmTotalsV2.UpdateData(): {watch.ElapsedMilliseconds}ms");
        }
    }

    public class BarLabel : Label
    {
        [Description("Label group"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Group = "";

        [Description(
             "Format type\r\n0: Percentage\r\n1: Numeric, 2 decimals\r\n2: Numeric, 2 decimals, with sign\r\n3: Numeric, 2 decimals (for mez protection)\r\n4: Numeric, 2 decimals, per second\r\n5: Numeric, 2 decimals, speed\r\n6: Numeric, 2 decimals, distance\r\n7: Percentage, 2 decimals, with sign"),
         Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int FormatType;

        [Description("Bar label text"), Category("Appearance"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new string Text
        {
            get => base.Text;
            set => base.Text = clsConvertibleUnitValue.FormatValue(FormatType, value);
        }

        public void SetText(float value)
        {
            Text = Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        public void SetText(string valueText)
        {
            Text = valueText;
        }

        public BarLabel()
        {
            SetStyle(
                ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Name = "BarLabel";
        }
    }
}