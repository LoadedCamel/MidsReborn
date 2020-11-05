using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Base.Data_Classes;
using Base.Display;
using Base.Master_Classes;
using midsControls;
using Syncfusion.Windows.Forms.Tools;

namespace Hero_Designer.Forms.WindowMenuItems
{
    public partial class frmTotalsV2 : Form
    {
        private readonly frmMain _myParent;
        private bool KeepOnTop { get; set; }

        private readonly List<Enums.eDamage> DefenseDamageList = new List<Enums.eDamage>
        {
            Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
            Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Psionic, Enums.eDamage.Melee,
            Enums.eDamage.Ranged, Enums.eDamage.AoE
        };

        private readonly List<Enums.eDamage> ResistanceDamageList = new List<Enums.eDamage>
        {
            Enums.eDamage.Smashing, Enums.eDamage.Lethal, Enums.eDamage.Fire, Enums.eDamage.Cold,
            Enums.eDamage.Energy, Enums.eDamage.Negative, Enums.eDamage.Toxic, Enums.eDamage.Psionic
        };

        private readonly List<Enums.eEffectType> MovementTypesList = new List<Enums.eEffectType>
        {
            Enums.eEffectType.SpeedRunning, Enums.eEffectType.SpeedJumping, Enums.eEffectType.JumpHeight, Enums.eEffectType.SpeedFlying
        };

        private readonly List<Enums.eEffectType> PerceptionEffectsList = new List<Enums.eEffectType>
        {
            Enums.eEffectType.StealthRadius, Enums.eEffectType.StealthRadiusPlayer, Enums.eEffectType.PerceptionRadius
        };

        private readonly List<Enums.eMez> MezList = new List<Enums.eMez>
        {
            Enums.eMez.Held, Enums.eMez.Stunned, Enums.eMez.Sleep, Enums.eMez.Immobilized,
            Enums.eMez.Knockback, Enums.eMez.Repel, Enums.eMez.Confused, Enums.eMez.Terrorized,
            Enums.eMez.Taunt, Enums.eMez.Placate, Enums.eMez.Teleport
        };

        private readonly List<Enums.eEffectType> DebuffEffectsList = new List<Enums.eEffectType>
        {
            Enums.eEffectType.Defense, Enums.eEffectType.Endurance, Enums.eEffectType.Recovery,
            Enums.eEffectType.PerceptionRadius, Enums.eEffectType.ToHit, Enums.eEffectType.RechargeTime,
            Enums.eEffectType.SpeedRunning, Enums.eEffectType.Regeneration
        };

        public Control StatControl(string tab, int panel, string type, int control)
        {
            Regex regEx = new Regex(@"^\d+");
            TabPageAdv page = tabControlAdv2.Controls.OfType<TabPageAdv>().First(t => t.Text.Contains(tab));
            List<GradientPanel> gradientList = page.Controls.OfType<GradientPanel>().ToList();
            List<GradientPanel> gradientPanels = gradientList.OrderBy(x => x.Name).ToList();
            List<TableLayoutPanel> tablePanels = gradientPanels[panel - 1].Controls.OfType<TableLayoutPanel>().ToList();

            switch (type)
            {
                case "Bar":
                    List<Control> controls = new List<Control>();
                    for (int rowIndex = 0; rowIndex < tablePanels[0].RowCount; rowIndex++)
                    {
                        Control tControl = tablePanels[0].GetControlFromPosition(2, rowIndex);
                        controls.Add(tControl);
                    }

                    List<ctlLayeredBar> barList = controls.OfType<ctlLayeredBar>().ToList();
                    List<ctlLayeredBar> bars = barList.OrderBy(x => regEx.Match(x.Name).Value).ToList();

                    return bars[control - 1];
                case "Label":
                    controls = new List<Control>();
                    for (int rowIndex = 0; rowIndex < tablePanels[0].RowCount; rowIndex++)
                    {
                        Control tControl = tablePanels[0].GetControlFromPosition(1, rowIndex);
                        controls.Add(tControl);
                    }

                    List<Label> labelList = controls.OfType<Label>().ToList();
                    List<Label> labels = labelList.OrderBy(x => regEx.Match(x.Name).Value).ToList();

                    return labels[control - 1];
            }

            return null;
        }

        private IEnumerable<Control> GetControlHierarchy(Control root)
        {
            Queue<Control> queue = new Queue<Control>();
            queue.Enqueue(root);
            do
            {
                Control control = queue.Dequeue();
                yield return control;
                foreach (Control child in control.Controls.OfType<Control>())
                {
                    queue.Enqueue(child);
                }
            } while (queue.Count > 0);
        }

        public frmTotalsV2(ref frmMain iParent)
        {
            FormClosed += frmTotalsV2_FormClosed;
            Load += OnLoad;
            KeepOnTop = true;
            InitializeComponent();
            _myParent = iParent;
            foreach (Control control in GetControlHierarchy(tabControlAdv2))
            {
                if (!control.Name.Contains("bar")) continue;

                control.MouseEnter += Bar_Enter;
                control.MouseLeave += Bar_Leave;
            }

            // Tab Foreground Colors don't stick in the designer.
            // Note: default colors will be used anyway.
            // This may only cause issues if a custom
            // Windows theme is in use.
            tabControlAdv2.ActiveTabForeColor = Color.White;
            tabControlAdv2.InActiveTabForeColor = Color.Black;
        }

        private List<string> GetVectorTypesList(string[] enumNames, IEnumerable<int> targetValues)
        {
            List<string> ret = new List<string>();
            ret.AddRange(targetValues.Select(k => enumNames[k]).ToList());

            return ret;
        }

        private string UCFirst(string s)
        {
            return char.ToUpper(s[0]) + s.Substring(1).ToLower();
        }

        private void Bar_Enter(object sender, EventArgs e)
        {
            ctlLayeredBar trigger = (ctlLayeredBar) sender;
            Character.TotalStatistics totals = MidsContext.Character.Totals;
            Character.TotalStatistics cappedTotals = MidsContext.Character.TotalsCapped;
            string barGroup = trigger.Group;
            int barIndex = GetBarIndex(trigger);
            int vectorTypeIndex = string.IsNullOrEmpty(trigger.Group) ? -1 : GetBarVectorTypeIndex(barIndex, barGroup);
            string atName = MidsContext.Character.Archetype.DisplayName;
            string tooltipText = "";
            List<string> vectorTypes;

            switch (barGroup)
            {
                case "Defense":
                    vectorTypes = GetVectorTypesList(Enum.GetNames(typeof(Enums.eDamage)), DefenseDamageList.Cast<int>());
                    tooltipText = $"{trigger.ValueMainBar:##0.##}% {vectorTypes[vectorTypeIndex]} defense";
                    break;

                case "Resistance":
                    vectorTypes = GetVectorTypesList(Enum.GetNames(typeof(Enums.eDamage)), ResistanceDamageList.Cast<int>());
                    tooltipText = trigger.ValueMainBar <= trigger.ValueOverCap
                        ? $"{trigger.ValueMainBar:##0.##}% {vectorTypes[vectorTypeIndex]} resistance ({atName} resistance cap: {MidsContext.Character.Archetype.ResCap * 100:##0.##}%)"
                        : $"{trigger.ValueOverCap:##0.##}% {vectorTypes[vectorTypeIndex]} resistance (capped at {MidsContext.Character.Archetype.ResCap * 100:##0.##}%)";
                    break;

                case "HP" when barIndex == (int)Enums.eBarType.MaxHPAbsorb:
                    tooltipText = (trigger.ValueMainBar <= trigger.ValueOverCap
                               ? $"{trigger.ValueMainBar:##0.##} HP ({atName} HP cap: {MidsContext.Character.Archetype.HPCap})"
                               : $"{trigger.ValueOverCap:##0.##} HP, capped at {MidsContext.Character.Archetype.HPCap}"
                           ) +
                           $"\r\nBase: {trigger.ValueBase}" +
                           (trigger.ValueOverlay1 != 0 ? $"\r\nAbsorb: {trigger.ValueOverlay1:##0.##} ({(trigger.ValueOverlay1 / trigger.ValueBase * 100):##0.##}% of base HP)" : "");
                    break;

                // Triple bars main + base + overcap
                // Todo
                case "HP" when barIndex == (int)Enums.eBarType.Regeneration:
                case "Endurance" when trigger.EnableBaseValue && trigger.EnableOverCap:
                case "Movement":
                case "" when trigger.EnableBaseValue && trigger.EnableOverCap:
                    break;

                // Dual bar main + overcap
                case "Endurance" when !trigger.EnableBaseValue && trigger.EnableOverCap:
                case "" when !trigger.EnableBaseValue && trigger.EnableOverCap:
                    break;

                // Dual bar main + base
                case "Endurance" when trigger.EnableBaseValue && !trigger.EnableOverCap:
                case "" when trigger.EnableBaseValue && !trigger.EnableOverCap:
                    break;

                case "Status Protection":
                    vectorTypes = GetVectorTypesList(Enum.GetNames(typeof(Enums.eMez)), MezList.Cast<int>());
                    tooltipText = $"{Math.Abs(trigger.ValueMainBar):##0.##} {UCFirst(trigger.Group)} to {UCFirst(vectorTypes[vectorTypeIndex])}";
                    break;
                case "Status Resistance":
                case "Debuff Resistance":
                    vectorTypes = GetVectorTypesList(Enum.GetNames(typeof(Enums.eMez)), MezList.Cast<int>());
                    tooltipText = $"{trigger.ValueMainBar:##0.##}% {UCFirst(trigger.Group)} to {UCFirst(vectorTypes[vectorTypeIndex])}";
                    break;

                default:
                    tooltipText = "";
                    break;
            }
        }

        private int GetBarVectorTypeIndex(int barIndex, string barGroup)
        {
            return barGroup switch
            {
                "Defense" => (int) DefenseDamageList[barIndex - 1],
                "Resistance" => (int) ResistanceDamageList[barIndex - 11],
                "Movement" => (int) MovementTypesList[barIndex - 24],
                "Perception" => (int) PerceptionEffectsList[barIndex - 28],
                "Status Protection" => (int) MezList[barIndex - 38],
                "Status Resistance" => (int) MezList[barIndex - 49],
                "Debuff Protection" => (int) MezList[barIndex - 60],
                _ => -1
            };
        }

        private void Bar_Leave(object sender, EventArgs e)
        {
            //TTip.SetToolTip(this, "");
        }

        private Enums.eBarType GetValueType(ctlLayeredBar bar)
        {
            return (Enums.eBarType) GetBarIndex(bar);
        }

        private int GetBarIndex(ctlLayeredBar bar)
        {
            return Convert.ToInt32(bar.Name.Substring(3)) - 1;
        }

        private string GetValueGroup(ctlLayeredBar bar)
        {
            return bar.Group;
        }

        private void SetBarsBulk(IEnumerable<Control> controlsList, string group, float[] mainValues)
        {
            List<ctlLayeredBar> barsGroup = controlsList
                .Cast<ctlLayeredBar>()
                .Where(e => e.Group == group)
                .ToList();

            barsGroup.Sort((a, b) => GetBarIndex(a).CompareTo(GetBarIndex(b)));

            for (int i = 0; i < barsGroup.Count; i++)
            {
                SetBarSingle(barsGroup[i], mainValues[i]);
            }
        }

        private void SetBarsBulk(IEnumerable<Control> controlsList, string group, float[] mainValues, float[] auxValues, bool auxIsBase)
        {
            List<ctlLayeredBar> barsGroup = controlsList
                .Cast<ctlLayeredBar>()
                .Where(e => e.Group == group)
                .ToList();

            barsGroup.Sort((a, b) => GetBarIndex(a).CompareTo(GetBarIndex(b)));

            for (int i = 0; i < barsGroup.Count; i++)
            {
                if (auxIsBase)
                {
                    SetBarSingle(barsGroup[i], mainValues[i], auxValues[i]);
                }
                else
                {
                    SetBarSingle(barsGroup[i], mainValues[i], 0, auxValues[i]);
                }
            }
        }

        private void SetBarsBulk(IEnumerable<Control> controlsList, string group, float[] mainValues, float[] baseValues, float[] overcapValues)
        {
            List<ctlLayeredBar> barsGroup = controlsList
                .Cast<ctlLayeredBar>()
                .Where(e => e.Group == group)
                .ToList();

            barsGroup.Sort((a, b) => GetBarIndex(a).CompareTo(GetBarIndex(b)));

            for (int i = 0; i < barsGroup.Count; i++)
            {
                SetBarSingle(barsGroup[i], mainValues[i], baseValues[i], overcapValues[i]);
            }
        }

        private void SetBarSingle(Enums.eBarType barType, float value, float baseValue=0, float overCapValue=0, float overlay1Value=0, float overlay2Value=0)
        {
            SetBarSingle(FetchBar((int) barType), value, baseValue, overCapValue, overlay1Value, overlay2Value);
        }

        private void SetBarSingle(ctlLayeredBar bar, float value, float baseValue = 0, float overCapValue = 0, float overlay1Value = 0, float overlay2Value = 0)
        {
            bar.SuspendUpdate();
            bar.ValueMainBar = value;
            bar.ValueBase = baseValue;
            bar.ValueOverCap = overCapValue;
            bar.ValueOverlay1 = overlay1Value;
            bar.ValueOverlay2 = overlay2Value;
            bar.ResumeUpdate();
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

            for (int i = 0; i < lvGroup.Count; i++)
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
            FetchLv(barType).Text = Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        private void SetLvSingle(Enums.eBarType barType, string valueText)
        {
            FetchLv(barType).Text = valueText;
        }

        private void Radio_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton sendingControl = (RadioButton) sender;
            IEnumerable<RadioButton> radioControls = Controls.OfType<RadioButton>();
            if (!sendingControl.Checked) return;

            foreach (RadioButton radio in radioControls)
            {
                if (radio.Name != sendingControl.Name)
                {
                    radio.Checked = false;
                }
            }
        }

        public override void Refresh()
        {
            if (MidsContext.Character.IsHero())
            {
                tabControlAdv2.InactiveTabColor = Color.FromArgb(61, 111, 161);
                tabControlAdv2.TabPanelBackColor = Color.FromArgb(61, 111, 161);
                tabControlAdv2.FixedSingleBorderColor = Color.Goldenrod;
                tabControlAdv2.ActiveTabColor = Color.Goldenrod;
            }
            else
            {
                /*tabControlAdv2.InactiveTabColor = Color.FromArgb(193, 23, 23);
                tabControlAdv2.TabPanelBackColor = Color.FromArgb(193, 23, 23);
                tabControlAdv2.FixedSingleBorderColor = Color.FromArgb(198, 128, 29);
                tabControlAdv2.ActiveTabColor = Color.FromArgb(198, 128, 29);*/
                tabControlAdv2.InactiveTabColor = Color.FromArgb(138, 62, 57);
                tabControlAdv2.TabPanelBackColor = Color.FromArgb(138, 62, 57);
                tabControlAdv2.FixedSingleBorderColor = Color.Goldenrod;
                tabControlAdv2.ActiveTabColor = Color.Goldenrod;
            }

            base.Refresh();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            CenterToParent();
            Refresh();
        }

        private void frmTotalsV2_FormClosed(object sender, FormClosedEventArgs e)
        {
            _myParent.FloatTotals(false);
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

        private ctlLayeredBar FetchBar(int n)
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

        private ctlLayeredBar FetchBar(Enums.eBarType barType)
        {
            return FetchBar((int) barType);
        }

        private Label FetchLv(int n)
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

        private Label FetchLv(Enums.eBarType barType)
        {
            return FetchLv((int) barType);
        }
        #endregion

        #region frmTotals import

        /*
        private void RbSpeedCheckedChanged(object sender, EventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            if (rbMPH.Checked)
                MidsContext.Config.SpeedFormat = Enums.eSpeedMeasure.MilesPerHour;
            else if (rbKPH.Checked)
                MidsContext.Config.SpeedFormat = Enums.eSpeedMeasure.KilometersPerHour;
            else if (rbFPS.Checked)
                MidsContext.Config.SpeedFormat = Enums.eSpeedMeasure.FeetPerSecond;
            else if (rbMSec.Checked)
                MidsContext.Config.SpeedFormat = Enums.eSpeedMeasure.MetersPerSecond;
            UpdateData();
        }
        */

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

        public void UpdateData()
        {
            //pbClose.Refresh();
            //pbTopMost.Refresh();
            Statistics displayStats = MidsContext.Character.DisplayStats;
            Character.TotalStatistics uncappedStats = MidsContext.Character.Totals;
            Character.TotalStatistics cappedStats = MidsContext.Character.TotalsCapped;
            Stopwatch watch = Stopwatch.StartNew();
            tabControlAdv2.SuspendLayout();
            IEnumerable<ctlLayeredBar> barsList = GetControlHierarchy(tabControlAdv2).Cast<ctlLayeredBar>().ToList();
            IEnumerable<BarLabel> lvList = GetControlHierarchy(tabControlAdv2).Cast<BarLabel>().ToList();

            #region Bars setup

            SetBarsBulk(
                barsList,
                "Defense",
                DefenseDamageList.Cast<int>().Select(t => displayStats.Defense(t)).ToArray()
            );

            SetBarsBulk(
                barsList,
                "Resistance",
                ResistanceDamageList.Cast<int>().Select(t => displayStats.DamageResistance(t, false)).ToArray(),
                ResistanceDamageList.Cast<int>().Select(t => displayStats.DamageResistance(t, true)).ToArray(),
                false
            );

            SetBarSingle(Enums.eBarType.Regeneration,
                displayStats.HealthRegenPercent(false),
                MidsContext.Character.Archetype.BaseRegen * 100,
                displayStats.HealthRegenPercent(true));
            
            //Debug.WriteLine($"Regen: {MidsContext.Character.Archetype.BaseRegen} / {displayStats.HealthRegenPercent(false)} / {displayStats.HealthRegenPercent(true)}");

            SetBarSingle(Enums.eBarType.MaxHPAbsorb,
                displayStats.HealthHitpointsNumeric(false),
                MidsContext.Character.Archetype.Hitpoints,
                displayStats.HealthHitpointsNumeric(true),
                Math.Min(displayStats.Absorb, MidsContext.Character.Archetype.Hitpoints));
            
            SetBarSingle(Enums.eBarType.EndRec,
                displayStats.EnduranceRecoveryNumeric,
                MidsContext.Character.Archetype.BaseRecovery,
                displayStats.EnduranceRecoveryNumericUncapped);
            SetBarSingle(Enums.eBarType.EndUse, displayStats.EnduranceUsage);
            SetBarSingle(Enums.eBarType.MaxEnd, displayStats.EnduranceMaxEnd, 100);

            ///////////////////////////////

            SetBarsBulk(
                barsList,
                "Movement",
                new[]
                {
                    displayStats.MovementRunSpeed(Enums.eSpeedMeasure.FeetPerSecond, false),
                    displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.FeetPerSecond, false),
                    displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond),
                    displayStats.MovementFlySpeed(Enums.eSpeedMeasure.MilesPerHour, false)
                },
                new[]
                {
                    Statistics.BaseRunSpeed, // ft/s
                    Statistics.BaseJumpSpeed,
                    Statistics.BaseJumpHeight,
                    displayStats.MovementFlySpeed(Enums.eSpeedMeasure.MilesPerHour, false) == 0 ? 0 : Statistics.BaseFlySpeed
                },
                new[]
                {
                    displayStats.MovementRunSpeed(Enums.eSpeedMeasure.FeetPerSecond, true),
                    displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.FeetPerSecond, true),
                    displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond),
                    displayStats.MovementFlySpeed(Enums.eSpeedMeasure.MilesPerHour, true)
                });

            //Debug.WriteLine($"Run speed:\r\nBase: {Statistics.BaseRunSpeed}, Main: {displayStats.MovementRunSpeed(Enums.eSpeedMeasure.FeetPerSecond, false)}, Overcap: {displayStats.MovementRunSpeed(Enums.eSpeedMeasure.FeetPerSecond, true)}");
            //Debug.WriteLine($"Jump speed:\r\nBase: {Statistics.BaseJumpSpeed}, Main: {displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.FeetPerSecond, false)}, Overcap: {displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.FeetPerSecond, true)}");
            //Debug.WriteLine($"Jump height:\r\nBase: {Statistics.BaseJumpHeight}, Main: {displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond)}");

            ///////////////////////////////
            
            SetBarsBulk(
                barsList,
                "Perception",
                new []
                {
                    MidsContext.Character.Totals.StealthPvE,
                    MidsContext.Character.Totals.StealthPvP,
                    displayStats.Perception(false)
                },
                new []
                {
                    0,
                    0,
                    Statistics.BasePerception
                },
                new []
                {
                    MidsContext.Character.Totals.StealthPvE,
                    MidsContext.Character.Totals.StealthPvP,
                    displayStats.Perception(true)
                });

            ///////////////////////////////

            SetBarSingle(Enums.eBarType.Haste, displayStats.BuffHaste(false), 100, displayStats.BuffHaste(true));
            SetBarSingle(Enums.eBarType.ToHit, displayStats.BuffToHit);
            SetBarSingle(Enums.eBarType.ToHit, displayStats.BuffAccuracy);
            SetBarSingle(Enums.eBarType.Damage, displayStats.BuffDamage(false), 100, displayStats.BuffDamage(true));
            SetBarSingle(Enums.eBarType.EndRdx, displayStats.BuffEndRdx);
            SetBarSingle(Enums.eBarType.ThreatLevel, displayStats.ThreatLevel, MidsContext.Character.Archetype.BaseThreat);
            SetBarSingle(Enums.eBarType.Elusivity, MidsContext.Character.Totals.Elusivity);

            ///////////////////////////////

            SetBarsBulk(
                barsList,
                "Status Protection",
                MezList.Select(m => Math.Abs(MidsContext.Character.Totals.Mez[(int) m])).ToArray());

            SetBarsBulk(
                barsList,
                "Status Resistance",
                MezList.Select(m => MidsContext.Character.Totals.MezRes[(int) m]).ToArray());

            ///////////////////////////////

            SetBarsBulk(
                barsList,
                "Debuff Resistance",
                DebuffEffectsList.Select(e => MidsContext.Character.Totals.DebuffRes[(int) e]).ToArray());
            #endregion

            #region Labels setup

            SetLvsBulk(
                lvList,
                "Defense",
                DefenseDamageList.Cast<int>().Select(t => displayStats.Defense(t)).ToArray()
            );

            SetLvsBulk(
                lvList,
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
                lvList,
                "Movement",
                new[]
                {
                    displayStats.MovementRunSpeed(Enums.eSpeedMeasure.FeetPerSecond, false),
                    displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.FeetPerSecond, false),
                    displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond),
                    displayStats.MovementFlySpeed(Enums.eSpeedMeasure.MilesPerHour, false)
                });

            ///////////////////////////////

            SetLvsBulk(
                lvList,
                "Perception",
                new[]
                {
                    MidsContext.Character.Totals.StealthPvE,
                    MidsContext.Character.Totals.StealthPvP,
                    displayStats.Perception(false)
                });

            ///////////////////////////////

            SetLvSingle(Enums.eBarType.Haste, displayStats.BuffHaste(false));
            SetLvSingle(Enums.eBarType.ToHit, displayStats.BuffToHit);
            SetLvSingle(Enums.eBarType.Accuracy, displayStats.BuffAccuracy);
            SetLvSingle(Enums.eBarType.Damage, displayStats.BuffDamage(false)); // Need to add +100 here ?
            SetLvSingle(Enums.eBarType.EndRdx, displayStats.BuffEndRdx);
            SetLvSingle(Enums.eBarType.ThreatLevel, displayStats.ThreatLevel);
            SetLvSingle(Enums.eBarType.Elusivity, MidsContext.Character.Totals.Elusivity);

            ///////////////////////////////

            SetLvsBulk(
                lvList,
                "Status Protection",
                MezList.Select(m => Math.Abs(MidsContext.Character.Totals.Mez[(int)m])).ToArray());

            SetLvsBulk(
                lvList,
                "Status Resistance",
                MezList.Select(m => MidsContext.Character.Totals.MezRes[(int)m]).ToArray());

            ///////////////////////////////

            SetLvsBulk(
                lvList,
                "Debuff Resistance",
                DebuffEffectsList.Select(e => MidsContext.Character.Totals.DebuffRes[(int)e]).ToArray());

            #endregion

            tabControlAdv2.ResumeLayout();

            watch.Stop();
            Debug.WriteLine($"frmTotalsV2.UpdateData(): {watch.ElapsedMilliseconds}ms");
        }
    }

    public class BarLabel : Label
    {
        [Description("Label group"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Group = "";

        [Description("Format type\r\n0: Percentage\r\n1: Numeric, 2 decimals\r\n2: Numeric, 2 decimals, with sign\r\n3: Numeric, 2 decimals (for mez protection)\r\n4: Numeric, 2 decimals, per second\r\n5: Numeric, 2 decimals, speed\r\n6: Numeric, 2 decimals, distance"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int FormatType = 0;

        [Description("Bar label text"), Category("Appearance"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new string Text
        {
            get => base.Text;
            set => base.Text = clsConvertibleUnitValue.FormatValue(FormatType, value);
        }

        public BarLabel()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            Name = "BarLabel1";
        }
    }
}