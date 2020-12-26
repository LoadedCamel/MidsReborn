using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Master_Classes;
using mrbControls;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmCompare : Form
    {
        private readonly string[] DisplayValueStrings;
        private readonly frmMain myParent;
        private readonly IPower[][] Powers;
        private readonly string[][] Tips;
        private readonly float[][] Values;
        private ImageButton btnClose;
        private Button btnTweakMatch;
        private ComboBox cbAT1;
        private ComboBox cbAT2;
        private ComboBox cbSet1;
        private ComboBox cbSet2;
        private ComboBox cbType1;
        private ComboBox cbType2;
        private CheckBox chkMatching;
        private ImageButton chkOnTop;
        private ctlMultiGraph Graph;
        private float GraphMax;
        private GroupBox GroupBox1;
        private GroupBox GroupBox2;
        private GroupBox GroupBox4;
        private Label lblKeyColor1;
        private Label lblKeyColor2;
        private Label lblScale;
        private bool Loaded;
        private ListBox lstDisplay;
        private Enums.CompMap Map;
        private bool Matching;
        private TrackBar tbScaleX;
        private ToolTip tTip;

        public frmCompare(ref frmMain iFrm)
        {
            Load += frmCompare_Load;
            KeyDown += frmCompare_KeyDown;
            VisibleChanged += frmCompare_VisibleChanged;
            Resize += frmCompare_Resize;
            Move += frmCompare_Move;
            FormClosed += frmCompare_FormClosed;
            Powers = new IPower[2][];
            Values = new float[2][];
            Tips = new string[2][];
            GraphMax = 1f;
            Matching = false;
            Loaded = false;
            DisplayValueStrings = new[]
            {
                "Base Accuracy",
                "Damage",
                "Damage / Anim",
                "Damage / Sec",
                "Damage / End",
                "Damage Buff",
                "Defense",
                "Defense Debuff",
                "Duration",
                "End Use",
                "End Use / Sec",
                "Healing",
                "Healing / Sec",
                "Healing / End",
                "+HP",
                "Max Targets",
                "Range",
                "Recharge Time",
                "Regeneration",
                "Resistance",
                "Resistance Debuff",
                "ToHit Buff",
                "ToHit Debuff"
            };
            InitializeComponent();
            Name = nameof(frmCompare);
            myParent = iFrm;
            var componentResourceManager = new ComponentResourceManager(typeof(frmCompare));
            Icon = Resources.reborn;
        }

        private void btnClose_ButtonClicked()
        {
            Close();
        }

        private void btnTweakMatch_Click(object sender, EventArgs e)
        {
            new frmTweakMatching().ShowDialog(this);
            DisplayGraph();
        }

        private void cbAT1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded)
                return;
            List_Sets(0);
        }

        private void cbAT2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded)
                return;
            List_Sets(1);
        }

        private void cbSet1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded)
                return;
            ResetScale();
            DisplayGraph();
        }

        private void cbSet2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded)
                return;
            ResetScale();
            DisplayGraph();
        }

        private void cbType1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded)
                return;
            List_Sets(0);
        }

        private void cbType2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded)
                return;
            List_Sets(1);
        }

        private void chkMatching_CheckedChanged(object sender, EventArgs e)
        {
            Matching = chkMatching.Checked;
            if (!Loaded)
                return;
            DisplayGraph();
        }

        private void chkOnTop_CheckedChanged()
        {
            TopMost = chkOnTop.Checked;
        }

        private void DisplayGraph()
        {
            if (lstDisplay.SelectedIndex < 0)
                return;
            Graph.BeginUpdate();
            Graph.Clear();
            GetPowers();
            if (Matching)
                map_Advanced();
            else
                map_Simple();
            switch (lstDisplay.SelectedIndex)
            {
                case 0:
                    Graph.ColorFadeEnd = Color.FromArgb(byte.MaxValue, byte.MaxValue, 0);
                    values_Accuracy();
                    break;
                case 1:
                    Graph.ColorFadeEnd = Color.Red;
                    values_Damage();
                    break;
                case 2:
                    Graph.ColorFadeEnd = Color.Red;
                    values_DPA();
                    break;
                case 3:
                    Graph.ColorFadeEnd = Color.Red;
                    values_DPS();
                    break;
                case 4:
                    Graph.ColorFadeEnd = Color.Red;
                    values_DPE();
                    break;
                case 5:
                    Graph.ColorFadeEnd = Color.FromArgb(192, 0, 0);
                    Values_Universal(Enums.eEffectType.DamageBuff, false, false);
                    break;
                case 6:
                    Graph.ColorFadeEnd = Color.FromArgb(192, 0, 192);
                    Values_Universal(Enums.eEffectType.Defense, false, false);
                    break;
                case 7:
                    Graph.ColorFadeEnd = Color.FromArgb(128, 0, 128);
                    Values_Universal(Enums.eEffectType.Defense, false, true);
                    break;
                case 8:
                    Graph.ColorFadeEnd = Color.FromArgb(128, 0, byte.MaxValue);
                    values_Duration();
                    break;
                case 9:
                    Graph.ColorFadeEnd = Color.FromArgb(192, 192, byte.MaxValue);
                    values_End();
                    break;
                case 10:
                    Graph.ColorFadeEnd = Color.FromArgb(192, 192, byte.MaxValue);
                    values_EPS();
                    break;
                case 11:
                    Graph.ColorFadeEnd = Color.FromArgb(96, byte.MaxValue, 96);
                    values_Heal();
                    break;
                case 12:
                    Graph.ColorFadeEnd = Color.FromArgb(96, byte.MaxValue, 96);
                    values_HPS();
                    break;
                case 13:
                    Graph.ColorFadeEnd = Color.FromArgb(96, byte.MaxValue, 96);
                    values_HPE();
                    break;
                case 14:
                    Graph.ColorFadeEnd = Color.FromArgb(96, byte.MaxValue, 96);
                    Values_Universal(Enums.eEffectType.HitPoints, true, false);
                    break;
                case 15:
                    Graph.ColorFadeEnd = Color.FromArgb(64, 128, 128);
                    values_MaxTargets();
                    break;
                case 16:
                    Graph.ColorFadeEnd = Color.FromArgb(96, 128, 96);
                    values_Range();
                    break;
                case 17:
                    Graph.ColorFadeEnd = Color.FromArgb(byte.MaxValue, 192, 128);
                    values_Recharge();
                    break;
                case 18:
                    Graph.ColorFadeEnd = Color.FromArgb(96, byte.MaxValue, 96);
                    Values_Universal(Enums.eEffectType.Regeneration, true, false);
                    break;
                case 19:
                    Graph.ColorFadeEnd = Color.FromArgb(0, 192, 192);
                    Values_Universal(Enums.eEffectType.Resistance, false, false);
                    break;
                case 20:
                    Graph.ColorFadeEnd = Color.FromArgb(0, 128, 128);
                    Values_Universal(Enums.eEffectType.Resistance, false, true);
                    break;
                case 21:
                    Graph.ColorFadeEnd = Color.FromArgb(byte.MaxValue, byte.MaxValue, 96);
                    Values_Universal(Enums.eEffectType.ToHit, true, false);
                    break;
                case 22:
                    Graph.ColorFadeEnd = Color.FromArgb(192, 192, 64);
                    Values_Universal(Enums.eEffectType.ToHit, true, true);
                    break;
            }

            var index1 = 0;
            do
            {
                string[] powerDisplays = {"", ""};
                var values = new float[2];
                var iTip = "";
                var mapIdx = 0;
                do
                {
                    if (Map.Map[index1, mapIdx] > -1)
                    {
                        powerDisplays[mapIdx] = Powers[mapIdx][Map.Map[index1, mapIdx]].DisplayName;
                        values[mapIdx] = Values[mapIdx][Map.Map[index1, mapIdx]];
                        if ((iTip != "") & (Tips[mapIdx][Map.Map[index1, mapIdx]] != ""))
                            iTip += "\r\n----------\r\n";
                        iTip += Tips[mapIdx][Map.Map[index1, mapIdx]];
                    }

                    ++mapIdx;
                } while (mapIdx <= 1);

                Graph.AddItemPair(powerDisplays[0], powerDisplays[1], values[0], values[1], iTip);
                ++index1;
            } while (index1 <= 20);

            Graph.Max = GraphMax;
            tbScaleX.Value = Graph.ScaleIndex;
            SetScaleLabel();
            Graph.EndUpdate();
        }

        private void FillDisplayList()
        {
            var lstDisplay = this.lstDisplay;
            lstDisplay.BeginUpdate();
            lstDisplay.Items.Clear();
            lstDisplay.Items.AddRange(DisplayValueStrings);
            this.lstDisplay.SelectedIndex = 0;
            lstDisplay.EndUpdate();
        }

        private void frmCompare_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.EventHandlerWithCatch(() =>
                myParent.FloatCompareGraph(false));
        }

        private void frmCompare_KeyDown(object sender, KeyEventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (!(e.Control & e.Shift & (e.KeyCode == Keys.T)))
                    return;
                btnTweakMatch.Visible = true;
            });
        }

        private void frmCompare_Load(object sender, EventArgs e)
        {
            FillDisplayList();
            UpdateData();
            list_AT();
            if (MidsContext.Character.Archetype.Idx > -1)
                cbAT1.SelectedIndex = MidsContext.Character.Archetype.Idx;
            if (MidsContext.Character.Archetype.Idx > -1)
                cbAT2.SelectedIndex = MidsContext.Character.Archetype.Idx;
            tbScaleX.Minimum = 0;
            tbScaleX.Maximum = Graph.ScaleCount - 1;
            list_Type();
            List_Sets(0);
            List_Sets(1);
            Map.Init();
            chkMatching.Checked = Matching;
            Loaded = true;
            DisplayGraph();
            tTip.SetToolTip(chkMatching,
                "Re-order powers so that similar powers are compared directly, regardless of their position in the set.\r\nFor example, moving snipe powers to directly compare.\r\n(This isn't known for its stunning accuracy, and gets confused by vastly different sets)");
        }

        private void frmCompare_Move(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(StoreLocation);
        }

        private void frmCompare_Resize(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(StoreLocation);
        }

        private void frmCompare_VisibleChanged(object sender, EventArgs e)
        {
            this.EventHandlerWithCatch(() =>
                Graph.BackColor = BackColor);
        }

        private int getAT(int idx)
        {
            return idx switch
            {
                0 => cbAT1.SelectedIndex,
                1 => cbAT2.SelectedIndex,
                _ => 0
            };
        }

        private int getMax(int iVal1, int ival2)
        {
            return iVal1 <= ival2 ? ival2 : iVal1;
        }

        private int GetNextFreeSlot()
        {
            var index = 0;
            while (Map.Map[index, 1] != -1)
            {
                ++index;
                if (index > 20)
                    return 20;
            }

            return index;
        }

        private void GetPowers()
        {
            var numArray = new int[2];
            var Index = 0;
            do
            {
                numArray[Index] = getSetIndex(Index);
                Powers[Index] = new IPower[DatabaseAPI.Database.Powersets[numArray[Index]].Powers.Length - 1 + 1];
                Values[Index] = new float[Powers[Index].Length + 1];
                Tips[Index] = new string[Powers[Index].Length + 1];
                var nIDClass = Index != 0 ? cbAT2.SelectedIndex : cbAT1.SelectedIndex;
                var num = Powers[Index].Length - 1;
                for (var index = 0; index <= num; ++index)
                {
                    Powers[Index][index] = new Power(DatabaseAPI.Database.Powersets[numArray[Index]].Powers[index]);
                    Powers[Index][index].AbsorbPetEffects();
                    Powers[Index][index].ApplyGrantPowerEffects();
                    if (nIDClass <= -1)
                        continue;
                    Powers[Index][index].ForcedClassID = nIDClass;
                    Powers[Index][index].ForcedClass = DatabaseAPI.UidFromNidClass(nIDClass);
                }

                ++Index;
            } while (Index <= 1);
        }

        private int getSetIndex(int Index)
        {
            ComboBox comboBox;
            switch (Index)
            {
                case 0:
                    comboBox = cbSet1;
                    break;
                case 1:
                    comboBox = cbSet2;
                    break;
                default:
                    return 0;
            }

            return DatabaseAPI.GetPowersetIndexes(getAT(Index), getSetType(Index))[comboBox.SelectedIndex].nID;
        }

        private Enums.ePowerSetType getSetType(int Index)
        {
            ComboBox comboBox;
            switch (Index)
            {
                case 0:
                    comboBox = cbType1;
                    break;
                case 1:
                    comboBox = cbType2;
                    break;
                default:
                    return Enums.ePowerSetType.Primary;
            }

            var ePowerSetType = comboBox.SelectedIndex switch
            {
                0 => Enums.ePowerSetType.Primary,
                1 => Enums.ePowerSetType.Secondary,
                2 => Enums.ePowerSetType.Ancillary,
                _ => Enums.ePowerSetType.Primary
            };
            return ePowerSetType;
        }

        private string GetUniversalTipString(Enums.ShortFX iSFX, ref IPower iPower)
        {
            var str1 = "";
            string str2;
            if (iSFX.Present)
            {
                var numArray = new int[0];
                var str3 = "";
                IPower power = new Power(iPower);
                var num1 = iSFX.Index.Length - 1;
                for (var index1 = 0; index1 <= num1; ++index1)
                {
                    if (iSFX.Index[index1] == -1 ||
                        power.Effects[iSFX.Index[index1]].EffectType == Enums.eEffectType.None)
                        continue;
                    var returnString = "";
                    var returnMask = new int[0];
                    power.GetEffectStringGrouped(iSFX.Index[index1], ref returnString, ref returnMask, false, false);
                    if (returnMask.Length <= 0)
                        continue;
                    if (str3 != "")
                        str3 += "\r\n  ";
                    str3 += returnString.Replace("\r\n", "\r\n  ");
                    var num2 = returnMask.Length - 1;
                    for (var index2 = 0; index2 <= num2; ++index2)
                        power.Effects[returnMask[index2]].EffectType = Enums.eEffectType.None;
                }

                var num3 = iSFX.Index.Length - 1;
                for (var index = 0; index <= num3; ++index)
                {
                    if (power.Effects[iSFX.Index[index]].EffectType == Enums.eEffectType.None)
                        continue;
                    if (str3 != "")
                        str3 += "\r\n  ";
                    str3 += power.Effects[iSFX.Index[index]].BuildEffectString().Replace("\r\n", "\r\n  ");
                }

                str2 = str1 + str3;
            }
            else
            {
                str2 = "";
            }

            return str2;
        }

        private void Graph_Load(object sender, EventArgs e)
        {
        }

        private void list_AT()
        {
            cbAT1.BeginUpdate();
            cbAT1.Items.Clear();
            cbAT2.BeginUpdate();
            cbAT2.Items.Clear();
            var num = DatabaseAPI.Database.Classes.Length - 1;
            for (var index = 0; index <= num; ++index)
            {
                if (!DatabaseAPI.Database.Classes[index].Playable)
                    continue;
                cbAT1.Items.Add(DatabaseAPI.Database.Classes[index].DisplayName);
                cbAT2.Items.Add(DatabaseAPI.Database.Classes[index].DisplayName);
            }

            cbAT1.SelectedIndex = MidsContext.Character.Archetype.Idx;
            cbAT2.SelectedIndex = MidsContext.Character.Archetype.Idx;
            cbAT1.EndUpdate();
            cbAT2.EndUpdate();
        }

        private void List_Sets(int Index)
        {
            var iSet = Enums.ePowerSetType.None;
            ComboBox comboBox1;
            ComboBox comboBox2;
            int selectedIndex;
            if (Index == 0)
            {
                comboBox1 = cbSet1;
                comboBox2 = cbType1;
                selectedIndex = cbAT1.SelectedIndex;
            }
            else
            {
                comboBox1 = cbSet2;
                comboBox2 = cbType2;
                selectedIndex = cbAT2.SelectedIndex;
            }

            iSet = comboBox2.SelectedIndex switch
            {
                0 => Enums.ePowerSetType.Primary,
                1 => Enums.ePowerSetType.Secondary,
                2 => Enums.ePowerSetType.Ancillary,
                _ => iSet
            };
            comboBox1.BeginUpdate();
            comboBox1.Items.Clear();
            var powersetIndexes = DatabaseAPI.GetPowersetIndexes(selectedIndex, iSet);
            var num = powersetIndexes.Length - 1;
            for (var index = 0; index <= num; ++index)
                comboBox1.Items.Add(powersetIndexes[index].DisplayName);
            if (comboBox1.Items.Count > 0)
                comboBox1.SelectedIndex = 0;
            comboBox1.EndUpdate();
        }

        private void list_Type()
        {
            cbType1.BeginUpdate();
            cbType1.Items.Clear();
            cbType2.BeginUpdate();
            cbType2.Items.Clear();
            cbType1.Items.Add("Primary");
            cbType1.Items.Add("Secondary");
            cbType1.Items.Add("Ancillary");
            cbType2.Items.Add("Primary");
            cbType2.Items.Add("Secondary");
            cbType2.Items.Add("Ancillary");
            cbType1.SelectedIndex = 0;
            cbType2.SelectedIndex = 0;
            cbType1.EndUpdate();
            cbType2.EndUpdate();
        }

        private void lstDisplay_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded)
                return;
            ResetScale();
            DisplayGraph();
        }

        private void map_Advanced()
        {
            var Placed = new bool[Powers[1].Length - 1 + 1];
            var num1 = Placed.Length - 1;
            for (var index = 0; index <= num1; ++index)
                Placed[index] = false;
            Map.Init();
            var num2 = getMax(Powers[0].Length, Powers[1].Length);
            if (num2 > 20)
                num2 = 20;
            var num3 = num2;
            for (var index = 0; index <= num3; ++index)
                if (Powers[0].Length > index)
                    Map.Map[index, 0] = index;
            var num4 = Powers[1].Length - 1;
            for (var index1 = 0; index1 <= num4; ++index1)
            {
                var displayName = Powers[1][index1].DisplayName;
                var num5 = Powers[0].Length - 1;
                for (var index2 = 0; index2 <= num5; ++index2)
                {
                    if (!(string.Equals(Powers[0][index2].DisplayName, displayName,
                        StringComparison.OrdinalIgnoreCase) & !Placed[index1]))
                        continue;
                    Map.Map[index2, 1] = index1;
                    Placed[index1] = true;
                    break;
                }
            }

            mapOverride();
            mapDescString(new[] {"summon"}, ref Placed);
            mapDescString(new[] {"toggle", "+def", "smash"}, ref Placed);
            mapDescString(new[] {"toggle", "+def", "energy"}, ref Placed);
            mapDescString(new[] {"toggle", "+def", "fire"}, ref Placed);
            mapDescString(new[] {"toggle", "+def", "ranged"}, ref Placed);
            mapDescString(new[] {"toggle", "+def", "melee"}, ref Placed);
            mapDescString(new[] {"toggle", "+def", "aoe"}, ref Placed);
            mapDescString(new[] {"auto", "+def", "smash"}, ref Placed);
            mapDescString(new[] {"auto", "+def", "energy"}, ref Placed);
            mapDescString(new[] {"auto", "+def", "fire"}, ref Placed);
            mapDescString(new[] {"auto", "+def", "ranged"}, ref Placed);
            mapDescString(new[] {"auto", "+def", "melee"}, ref Placed);
            mapDescString(new[] {"auto", "+def", "aoe"}, ref Placed);
            mapDescString(new[] {"toggle", "+res", "smash"}, ref Placed);
            mapDescString(new[] {"toggle", "+res", "energy"}, ref Placed);
            mapDescString(new[] {"toggle", "+res", "fire"}, ref Placed);
            mapDescString(new[] {"auto", "+res", "smash"}, ref Placed);
            mapDescString(new[] {"auto", "+res", "energy"}, ref Placed);
            mapDescString(new[] {"auto", "+res", "fire"}, ref Placed);
            mapDescString(new[] {"toggle", "+def"}, ref Placed);
            mapDescString(new[] {"toggle", "+res"}, ref Placed);
            mapDescString(new[] {"auto", "+def"}, ref Placed);
            mapDescString(new[] {"auto", "+res"}, ref Placed);
            mapDescString(new[] {"AoE", "disorient"}, ref Placed);
            mapDescString(new[] {"AoE", "stun"}, ref Placed);
            mapDescString(new[] {"AoE", "hold"}, ref Placed);
            mapDescString(new[] {"AoE", "sleep"}, ref Placed);
            mapDescString(new[]
            {
                "AoE",
                "immobilize"
            }, ref Placed);
            mapDescString(new[] {"AoE", "confuse"}, ref Placed);
            mapDescString(new[] {"AoE", "fear"}, ref Placed);
            mapDescString(new[]
            {
                "Cone",
                "disorient"
            }, ref Placed);
            mapDescString(new[] {"Cone", "stun"}, ref Placed);
            mapDescString(new[] {"Cone", "hold"}, ref Placed);
            mapDescString(new[] {"Cone", "sleep"}, ref Placed);
            mapDescString(new[]
            {
                "Cone",
                "immobilize"
            }, ref Placed);
            mapDescString(new[] {"Cone", "confuse"}, ref Placed);
            mapDescString(new[] {"Cone", "fear"}, ref Placed);
            mapDescString(new[] {"snipe"}, ref Placed);
            mapDescString(new[]
            {
                "AoE",
                "Extreme",
                "Self -Recovery"
            }, ref Placed);
            mapDescString(new[] {"close", "high"}, ref Placed);
            mapDescString(new[]
            {
                "ranged",
                "disorient",
                "minor"
            }, ref Placed);
            mapDescString(new[] {"ranged", "hold"}, ref Placed);
            mapDescString(new[] {"cone", "extreme"}, ref Placed);
            mapDescString(new[] {"cone", "superior"}, ref Placed);
            mapDescString(new[] {"cone", "high"}, ref Placed);
            mapDescString(new[] {"cone", "moderate"}, ref Placed);
            mapDescString(new[] {"cone", "minor"}, ref Placed);
            mapDescString(new[]
            {
                "ranged",
                "AoE",
                "extreme"
            }, ref Placed);
            mapDescString(new[]
            {
                "ranged",
                "AoE",
                "superior"
            }, ref Placed);
            mapDescString(new[]
            {
                "ranged",
                "AoE",
                "high"
            }, ref Placed);
            mapDescString(new[]
            {
                "ranged",
                "AoE",
                "moderate"
            }, ref Placed);
            mapDescString(new[]
            {
                "ranged",
                "AoE",
                "minor"
            }, ref Placed);
            mapDescString(new[] {"AoE", "extreme"}, ref Placed);
            mapDescString(new[] {"AoE", "superior"}, ref Placed);
            mapDescString(new[] {"AoE", "high"}, ref Placed);
            mapDescString(new[] {"AoE", "moderate"}, ref Placed);
            mapDescString(new[] {"AoE", "minor"}, ref Placed);
            mapDescString(new[] {"melee", "extreme"}, ref Placed);
            mapDescString(new[]
            {
                "melee",
                "superior"
            }, ref Placed);
            mapDescString(new[] {"melee", "high"}, ref Placed);
            mapDescString(new[]
            {
                "melee",
                "moderate"
            }, ref Placed);
            mapDescString(new[] {"melee", "minor"}, ref Placed);
            mapDescString(new[]
            {
                "melee",
                "disorient"
            }, ref Placed);
            mapDescString(new[] {"melee", "stun"}, ref Placed);
            mapDescString(new[] {"melee", "hold"}, ref Placed);
            mapDescString(new[] {"AoE", "knockback"}, ref Placed);
            mapDescString(new[] {"AoE", "knockup"}, ref Placed);
            mapDescString(new[]
            {
                "Cone",
                "knockback"
            }, ref Placed);
            mapDescString(new[] {"Cone", "knockup"}, ref Placed);
            mapDescString(new[] {"AoE", "stealth"}, ref Placed);
            mapDescString(new[] {"stealth"}, ref Placed);
            mapDescString(new[] {"toggle", "-def"}, ref Placed);
            mapDescString(new[] {"toggle", "-res"}, ref Placed);
            mapDescString(new[] {"toggle", "-acc"}, ref Placed);
            mapDescString(new[] {"toggle", "-dmg"}, ref Placed);
            mapDescString(new[] {"-def"}, ref Placed);
            mapDescString(new[] {"-res"}, ref Placed);
            mapDescString(new[] {"-acc"}, ref Placed);
            mapDescString(new[] {"-dmg"}, ref Placed);
            mapDescString(new[] {"+dmg"}, ref Placed);
            mapDescString(new[] {"+acc"}, ref Placed);
            mapDescString(new[] {"heal", "team"}, ref Placed);
            mapDescString(new[] {"heal", "ally"}, ref Placed);
            mapDescString(new[] {"heal"}, ref Placed);
            mapDescString(new[] {"+recovery"}, ref Placed);
            mapDescString(new[] {"-recovery"}, ref Placed);
            mapDescString(new[] {"-regen"}, ref Placed);
            mapDescString(new[] {"extreme"}, ref Placed);
            mapDescString(new[] {"superior"}, ref Placed);
            mapDescString(new[] {"high"}, ref Placed);
            mapDescString(new[] {"moderate"}, ref Placed);
            mapDescString(new[] {"minor"}, ref Placed);
            mapDescString(new[] {"disorient"}, ref Placed);
            mapDescString(new[] {"stun"}, ref Placed);
            mapDescString(new[] {"hold"}, ref Placed);
            mapDescString(new[] {"sleep"}, ref Placed);
            mapDescString(new[] {"immobilize"}, ref Placed);
            mapDescString(new[] {"confuse"}, ref Placed);
            mapDescString(new[] {"fear"}, ref Placed);
            mapDescString(new[] {"cone"}, ref Placed);
            mapDescString(new[] {"aoe"}, ref Placed);
            mapDescString(new[] {"melee"}, ref Placed);
            mapDescString(new[] {"ranged"}, ref Placed);
            var num6 = Powers[1].Length - 1;
            for (var index = 0; index <= num6; ++index)
            {
                if (Placed[index] || Map.Map[index, 1] != -1)
                    continue;
                Map.Map[index, 1] = index;
                Placed[index] = true;
            }

            var num7 = Powers[1].Length - 1;
            for (var index = 0; index <= num7; ++index)
            {
                if (Placed[index])
                    continue;
                Map.Map[GetNextFreeSlot(), 1] = index;
                Placed[index] = true;
            }
        }

        private void map_Simple()
        {
            Map.Init();
            var Index = 0;
            do
            {
                Map.IdxAT[Index] = getAT(Index);
                Map.IdxSet[Index] = getSetIndex(Index);
                ++Index;
            } while (Index <= 1);

            var num1 = getMax(Powers[0].Length, Powers[1].Length);
            if (num1 > 20)
                num1 = 20;
            var num2 = num1;
            for (var index = 0; index <= num2; ++index)
            {
                if (Powers[0].Length > index)
                    Map.Map[index, 0] = index;
                if (Powers[1].Length > index)
                    Map.Map[index, 1] = index;
            }
        }

        private int mapDescString(string[] iStrings, ref bool[] placed)
        {
            var num1 = 0;
            for (var powerIdx = 0; powerIdx <= Powers[1].Length - 1; ++powerIdx)
            {
                var flag1 = true;
                for (var index2 = 0; index2 <= iStrings.Length - 1; ++index2)
                    if (Powers[1][powerIdx].DescShort.IndexOf(iStrings[index2], StringComparison.OrdinalIgnoreCase) < 0)
                        flag1 = false;

                if (!(flag1 & !placed[powerIdx]))
                    continue;
                {
                    for (var index2 = 0; index2 <= Powers[0].Length - 1; ++index2)
                    {
                        var flag2 = Map.Map[index2, 1] < 0;
                        for (var index3 = 0; index3 <= iStrings.Length - 1; ++index3)
                            if (Powers[0][index2].DescShort
                                .IndexOf(iStrings[index3], StringComparison.OrdinalIgnoreCase) < 0)
                                flag2 = false;

                        if (!flag2)
                            continue;
                        Map.Map[index2, 1] = powerIdx;
                        placed[powerIdx] = true;
                        return powerIdx;
                    }
                }
            }

            return num1;
        }

        private void mapOverride()
        {
            for (var index1 = 0; index1 <= MidsContext.Config.CompOverride.Length - 1; ++index1)
            {
                var compOverride = MidsContext.Config.CompOverride;
                mapOverrideDo(compOverride[index1].Powerset, compOverride[index1].Power, compOverride[index1].Override);
            }
        }

        private void mapOverrideDo(string iSet, string iPower, string iNewStr)
        {
            var index1 = 0;
            do
            {
                var num = Powers[index1].Length - 1;
                for (var index2 = 0; index2 <= num; ++index2)
                    if (Powers[index1][index2].PowerSetID > -1 &&
                        string.Equals(Powers[index1][index2].DisplayName, iPower, StringComparison.OrdinalIgnoreCase) &
                        string.Equals(DatabaseAPI.Database.Powersets[Powers[index1][index2].PowerSetID].DisplayName,
                            iSet, StringComparison.OrdinalIgnoreCase))
                        Powers[index1][index2].DescShort = iNewStr.ToUpper();
                ++index1;
            } while (index1 <= 1);
        }

        private void ResetScale()
        {
            tbScaleX.Value = 10;
            Graph.Max = GraphMax;
            SetScaleLabel();
        }

        public void SetLocation()
        {
            var rectangle = new Rectangle
            {
                X = MainModule.MidsController.SzFrmCompare.X,
                Y = MainModule.MidsController.SzFrmCompare.Y
            };
            if (rectangle.X < 1)
                rectangle.X = (int) Math.Round((Screen.PrimaryScreen.Bounds.Width - Width) / 2.0);
            if (rectangle.Y < 32)
                rectangle.Y = (int) Math.Round((Screen.PrimaryScreen.Bounds.Height - Height) / 2.0);
            Top = rectangle.Y;
            Left = rectangle.X;
        }

        private void SetScaleLabel()
        {
            lblScale.Text = "Scale: 0 - " + Convert.ToString(Graph.ScaleValue, CultureInfo.InvariantCulture);
        }

        private void StoreLocation()
        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            MainModule.MidsController.SzFrmCompare.X = Left;
            MainModule.MidsController.SzFrmCompare.Y = Top;
        }

        private void tbScaleX_Scroll(object sender, EventArgs e)
        {
            Graph.ScaleIndex = tbScaleX.Value;
            SetScaleLabel();
        }

        public void UpdateData()
        {
            btnClose.IA = myParent.Drawing.pImageAttributes;
            btnClose.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            btnClose.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            chkOnTop.IA = myParent.Drawing.pImageAttributes;
            chkOnTop.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            chkOnTop.ImageOn = MidsContext.Character.IsHero() ? myParent.Drawing.bxPower[3].Bitmap : myParent.Drawing.bxPower[5].Bitmap;
            if (!Loaded)
                return;
            ResetScale();
            DisplayGraph();
        }

        private void values_Accuracy()
        {
            var num1 = 1f;
            var index1 = 0;
            do
            {
                var num2 = Powers[index1].Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    var flag = false;
                    var num3 = Powers[index1][index2].Effects.Length - 1;
                    for (var index3 = 0; index3 <= num3; ++index3)
                        if (Powers[index1][index2].Effects[index3].RequiresToHitCheck)
                            flag = true;
                    Values[index1][index2] = !((Powers[index1][index2].EntitiesAutoHit == Enums.eEntity.None) | flag)
                        ? 0.0f
                        : (float) (Powers[index1][index2].Accuracy * (double) MidsContext.Config.BaseAcc * 100.0);
                    if (Math.Abs(Values[index1][index2]) < float.Epsilon)
                        continue;
                    {
                        Tips[index1][index2] =
                            DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID].DisplayName + ":" +
                            Powers[index1][index2].DisplayName;
                        if (Matching)
                        {
                            var tips = Tips;
                            var index3 = index1;
                            var index4 = index2;
                            tips[index3][index4] = tips[index3][index4] + " [Level " +
                                                   Convert.ToString(Powers[index1][index2].Level) + "]";
                        }

                        var tips1 = Tips;
                        var index5 = index1;
                        var index6 = index2;
                        tips1[index5][index6] = tips1[index5][index6] + "\r\n  " +
                                                Strings.Format(Values[index1][index2], "##0.##") + "% base Accuracy";
                        var tips2 = Tips;
                        var index7 = index1;
                        var index8 = index2;
                        tips2[index7][index8] = tips2[index7][index8] + "\r\n  (Real Numbers style: " +
                                                Strings.Format(Powers[index1][index2].Accuracy,
                                                    "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator +
                                                    "00") + "x)";
                        if (num1 < (double) Values[index1][index2])
                            num1 = Values[index1][index2];
                    }
                }

                ++index1;
            } while (index1 <= 1);

            GraphMax = num1 * 1.025f;
        }

        private void values_Damage()
        {
            var num1 = 1f;
            var returnValue = MidsContext.Config.DamageMath.ReturnValue;
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.Numeric;
            var index1 = 0;
            do
            {
                var num2 = Powers[index1].Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    Values[index1][index2] = Powers[index1][index2].FXGetDamageValue();
                    if (Math.Abs(Values[index1][index2]) < float.Epsilon)
                        continue;
                    Tips[index1][index2] =
                        DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID].DisplayName + ":" +
                        Powers[index1][index2].DisplayName;
                    if (Matching)
                    {
                        var tips = Tips;
                        var index3 = index1;
                        var index4 = index2;
                        tips[index3][index4] = tips[index3][index4] + " [Level " +
                                               Convert.ToString(Powers[index1][index2].Level) + "]";
                    }

                    var tips1 = Tips;
                    var index5 = index1;
                    var index6 = index2;
                    tips1[index5][index6] =
                        tips1[index5][index6] + "\r\n  " + Powers[index1][index2].FXGetDamageString();
                    if (num1 < (double) Values[index1][index2])
                        num1 = Values[index1][index2];
                    if (Powers[index1][index2].PowerType != Enums.ePowerType.Toggle)
                        continue;
                    {
                        var tips2 = Tips;
                        var index3 = index1;
                        var index4 = index2;
                        tips2[index3][index4] = tips2[index3][index4] + "\r\n  (Applied every " +
                                                Convert.ToString(Powers[index1][index2].ActivatePeriod,
                                                    CultureInfo.InvariantCulture) + "s)";
                    }
                }

                ++index1;
            } while (index1 <= 1);

            GraphMax = num1 * 1.025f;
            MidsContext.Config.DamageMath.ReturnValue = returnValue;
        }

        private void values_DPA()
        {
            var num1 = 1f;
            var returnValue = MidsContext.Config.DamageMath.ReturnValue;
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.DPA;
            var index1 = 0;
            do
            {
                for (var index2 = 0; index2 <= Powers[index1].Length - 1; ++index2)
                {
                    Values[index1][index2] = Powers[index1][index2].FXGetDamageValue();
                    if (Math.Abs(Values[index1][index2]) < float.Epsilon)
                        continue;
                    Tips[index1][index2] =
                        DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID].DisplayName + ":" +
                        Powers[index1][index2].DisplayName;
                    if (Matching)
                    {
                        var tips = Tips;
                        tips[index1][index2] = tips[index1][index2] + " [Level " +
                                               Convert.ToString(Powers[index1][index2].Level) + "]";
                    }

                    var tips1 = Tips;
                    var index5 = index1;
                    var index6 = index2;
                    tips1[index5][index6] = tips1[index5][index6] + "\r\n  " +
                                            Powers[index1][index2].FXGetDamageString() + "/s";
                    if (num1 < (double) Values[index1][index2])
                        num1 = Values[index1][index2];
                }

                ++index1;
            } while (index1 <= 1);

            GraphMax = num1 * 1.025f;
            MidsContext.Config.DamageMath.ReturnValue = returnValue;
        }

        private void values_DPE()
        {
            var num1 = 1f;
            var returnValue = MidsContext.Config.DamageMath.ReturnValue;
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.Numeric;
            var powIdx = 0;
            do
            {
                for (var subPowIdx = 0; subPowIdx <= Powers[powIdx].Length - 1; ++subPowIdx)
                {
                    Values[powIdx][subPowIdx] = Powers[powIdx][subPowIdx].FXGetDamageValue();
                    if (Math.Abs(Values[powIdx][subPowIdx]) < float.Epsilon)
                        continue;
                    if (Powers[powIdx][subPowIdx].PowerType == Enums.ePowerType.Click &&
                        Powers[powIdx][subPowIdx].EndCost > 0.0)
                        Values[powIdx][subPowIdx] /= Powers[powIdx][subPowIdx].EndCost;
                    Tips[powIdx][subPowIdx] =
                        DatabaseAPI.Database.Classes[Powers[powIdx][subPowIdx].ForcedClassID].DisplayName + ":" +
                        Powers[powIdx][subPowIdx].DisplayName;
                    if (Matching)
                        Tips[powIdx][subPowIdx] = Tips[powIdx][subPowIdx] + " [Level " +
                                                  Convert.ToString(Powers[powIdx][subPowIdx].Level) + "]";
                    Tips[powIdx][subPowIdx] = Tips[powIdx][subPowIdx] + "\r\n  " +
                                              Powers[powIdx][subPowIdx].FXGetDamageString();
                    if (num1 < (double) Values[powIdx][subPowIdx])
                        num1 = Values[powIdx][subPowIdx];
                    Tips[powIdx][subPowIdx] = Tips[powIdx][subPowIdx] + " - DPE: " +
                                              Strings.Format(Values[powIdx][subPowIdx], "##0.##");
                }

                ++powIdx;
            } while (powIdx <= 1);

            GraphMax = num1 * 1.025f;
            MidsContext.Config.DamageMath.ReturnValue = returnValue;
        }

        private void values_DPS()
        {
            var num1 = 1f;
            var returnValue = MidsContext.Config.DamageMath.ReturnValue;
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.DPS;
            var index1 = 0;
            do
            {
                var num2 = Powers[index1].Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    Values[index1][index2] = Powers[index1][index2].FXGetDamageValue();
                    if (Math.Abs(Values[index1][index2]) < float.Epsilon)
                        continue;
                    Tips[index1][index2] =
                        DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID].DisplayName + ":" +
                        Powers[index1][index2].DisplayName;
                    if (Matching)
                    {
                        var tips = Tips;
                        var index3 = index1;
                        var index4 = index2;
                        tips[index3][index4] = tips[index3][index4] + " [Level " +
                                               Convert.ToString(Powers[index1][index2].Level) + "]";
                    }

                    var tips1 = Tips;
                    var index5 = index1;
                    var index6 = index2;
                    tips1[index5][index6] = tips1[index5][index6] + "\r\n  " +
                                            Powers[index1][index2].FXGetDamageString() + "/s";
                    if (num1 < (double) Values[index1][index2])
                        num1 = Values[index1][index2];
                }

                ++index1;
            } while (index1 <= 1);

            GraphMax = num1 * 1.025f;
            MidsContext.Config.DamageMath.ReturnValue = returnValue;
        }

        private void values_Duration()
        {
            var num1 = 1f;
            var index1 = 0;
            do
            {
                var powerCount = Powers[index1].Length - 1;
                for (var index2 = 0; index2 <= powerCount; ++index2)
                {
                    var durationEffectId = Powers[index1][index2].GetDurationEffectID();
                    if (durationEffectId <= -1)
                        continue;
                    Values[index1][index2] = Powers[index1][index2].Effects[durationEffectId].Duration;
                    if (Math.Abs(Values[index1][index2]) < float.Epsilon)
                        continue;
                    Tips[index1][index2] =
                        DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID].DisplayName + ":" +
                        Powers[index1][index2].DisplayName;
                    if (Matching)
                    {
                        var tips = Tips;
                        var index3 = index1;
                        var index4 = index2;
                        tips[index3][index4] = tips[index3][index4] + " [Level " +
                                               Convert.ToString(Powers[index1][index2].Level) + "]";
                    }

                    var tips1 = Tips;
                    var index5 = index1;
                    var index6 = index2;
                    tips1[index5][index6] = tips1[index5][index6] + "\r\n  " +
                                            Powers[index1][index2].Effects[durationEffectId].BuildEffectString();
                    if (num1 < (double) Values[index1][index2])
                        num1 = Values[index1][index2];
                }

                ++index1;
            } while (index1 <= 1);

            GraphMax = num1 * 1.025f;
        }

        private void values_End()
        {
            var num1 = 1f;
            var index1 = 0;
            do
            {
                var num2 = Powers[index1].Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    Values[index1][index2] = Powers[index1][index2].EndCost;
                    if (Math.Abs(Values[index1][index2]) < float.Epsilon)
                        continue;
                    Tips[index1][index2] =
                        DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID].DisplayName + ":" +
                        Powers[index1][index2].DisplayName;
                    if (Matching)
                    {
                        var tips = Tips;
                        var index3 = index1;
                        var index4 = index2;
                        tips[index3][index4] = tips[index3][index4] + " [Level " +
                                               Convert.ToString(Powers[index1][index2].Level) + "]";
                    }

                    var tips1 = Tips;
                    var index5 = index1;
                    var index6 = index2;
                    tips1[index5][index6] = tips1[index5][index6] + "\r\n  End: " +
                                            Strings.Format(Powers[index1][index2].EndCost, "##0.##");
                    if (Powers[index1][index2].PowerType == Enums.ePowerType.Toggle)
                    {
                        var tips2 = Tips;
                        var index3 = index1;
                        var index4 = index2;
                        tips2[index3][index4] = tips2[index3][index4] + " (Per Second)";
                    }

                    if ((double) num1 < Values[index1][index2])
                        num1 = Values[index1][index2];
                }

                ++index1;
            } while (index1 <= 1);

            GraphMax = num1 * 1.025f;
        }

        private void values_EPS()
        {
            var num1 = 1f;
            var index1 = 0;
            do
            {
                var num2 = Powers[index1].Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    Values[index1][index2] = Powers[index1][index2].EndCost;
                    if (Math.Abs(Values[index1][index2]) < float.Epsilon)
                        continue;
                    switch (Powers[index1][index2].PowerType)
                    {
                        case Enums.ePowerType.Click:
                        {
                            if (Powers[index1][index2].RechargeTime + (double) Powers[index1][index2].CastTime +
                                Powers[index1][index2].InterruptTime > 0.0)
                                Values[index1][index2] = Powers[index1][index2].EndCost /
                                                         (Powers[index1][index2].RechargeTime +
                                                          Powers[index1][index2].CastTime +
                                                          Powers[index1][index2].InterruptTime);
                            break;
                        }
                        case Enums.ePowerType.Toggle:
                            Values[index1][index2] =
                                Powers[index1][index2].EndCost / Powers[index1][index2].ActivatePeriod;
                            break;
                    }

                    Tips[index1][index2] =
                        DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID].DisplayName + ":" +
                        Powers[index1][index2].DisplayName;
                    if (Matching)
                    {
                        var tips = Tips;
                        var index3 = index1;
                        var index4 = index2;
                        tips[index3][index4] = tips[index3][index4] + " [Level " +
                                               Convert.ToString(Powers[index1][index2].Level) + "]";
                    }

                    var tips1 = Tips;
                    var index5 = index1;
                    var index6 = index2;
                    tips1[index5][index6] = tips1[index5][index6] + "\r\n  End: " +
                                            Strings.Format(Values[index1][index2], "##0.##");
                    var tips2 = Tips;
                    var index7 = index1;
                    var index8 = index2;
                    tips2[index7][index8] = tips2[index7][index8] + "/s";
                    if (num1 < (double) Values[index1][index2])
                        num1 = Values[index1][index2];
                }

                ++index1;
            } while (index1 <= 1);

            GraphMax = num1 * 1.025f;
        }

        private void values_Heal()
        {
            var archetype = MidsContext.Archetype;
            var num1 = 1f;
            var index1 = 0;
            do
            {
                var num2 = Powers[index1].Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    MidsContext.Archetype = DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID];
                    var effectMagSum = Powers[index1][index2].GetEffectMagSum(Enums.eEffectType.Heal);
                    Values[index1][index2] = effectMagSum.Sum;
                    if (Math.Abs(Values[index1][index2]) < float.Epsilon)
                        continue;
                    Tips[index1][index2] = MidsContext.Archetype.DisplayName + ":" + Powers[index1][index2].DisplayName;
                    if (Matching)
                    {
                        var tips = Tips;
                        var index3 = index1;
                        var index4 = index2;
                        tips[index3][index4] = tips[index3][index4] + " [Level " +
                                               Convert.ToString(Powers[index1][index2].Level) + "]";
                    }

                    var tips1 = Tips;
                    var index5 = index1;
                    var index6 = index2;
                    tips1[index5][index6] = tips1[index5][index6] + "\r\n  " +
                                            Powers[index1][index2].Effects[effectMagSum.Index[0]].BuildEffectString();
                    if (num1 < (double) Values[index1][index2])
                        num1 = Values[index1][index2];
                }

                ++index1;
            } while (index1 <= 1);

            GraphMax = num1 * 1.025f;
            MidsContext.Archetype = archetype;
        }

        private void values_HPE()
        {
            var archetype = MidsContext.Archetype;
            var num1 = 1f;
            var index1 = 0;
            do
            {
                var num2 = Powers[index1].Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    MidsContext.Archetype = DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID];
                    var effectMagSum = Powers[index1][index2].GetEffectMagSum(Enums.eEffectType.Heal);
                    Values[index1][index2] = effectMagSum.Sum;
                    if (Math.Abs(Values[index1][index2]) < float.Epsilon)
                        continue;
                    if (Powers[index1][index2].EndCost > 0.0)
                        Values[index1][index2] /= Powers[index1][index2].EndCost;
                    Tips[index1][index2] =
                        DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID].DisplayName + ":" +
                        Powers[index1][index2].DisplayName;
                    if (Matching)
                    {
                        var tips = Tips;
                        var index3 = index1;
                        var index4 = index2;
                        tips[index3][index4] = tips[index3][index4] + " [Level " +
                                               Convert.ToString(Powers[index1][index2].Level) + "]";
                    }

                    var tips1 = Tips;
                    var index5 = index1;
                    var index6 = index2;
                    tips1[index5][index6] = tips1[index5][index6] + "\r\n  Heal: " +
                                            Strings.Format(Values[index1][index2], "##0.##") + " HP per unit of end.";
                    if (num1 < (double) Values[index1][index2])
                        num1 = Values[index1][index2];
                }

                ++index1;
            } while (index1 <= 1);

            GraphMax = num1 * 1.025f;
            MidsContext.Archetype = archetype;
        }

        private void values_HPS()
        {
            var archetype = MidsContext.Archetype;
            var num1 = 1f;
            var index1 = 0;
            do
            {
                var num2 = Powers[index1].Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    MidsContext.Archetype = DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID];
                    var effectMagSum = Powers[index1][index2].GetEffectMagSum(Enums.eEffectType.Heal);
                    Values[index1][index2] = effectMagSum.Sum;
                    if (Math.Abs(Values[index1][index2]) < float.Epsilon)
                        continue;
                    if (Powers[index1][index2].PowerType == Enums.ePowerType.Click &&
                        Powers[index1][index2].RechargeTime + (double) Powers[index1][index2].CastTime +
                        Powers[index1][index2].InterruptTime > 0.0)
                        Values[index1][index2] /= Powers[index1][index2].RechargeTime +
                                                  Powers[index1][index2].CastTime +
                                                  Powers[index1][index2].InterruptTime;
                    Tips[index1][index2] =
                        DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID].DisplayName + ":" +
                        Powers[index1][index2].DisplayName;
                    if (Matching)
                    {
                        var tips = Tips;
                        var index3 = index1;
                        var index4 = index2;
                        tips[index3][index4] = tips[index3][index4] + " [Level " +
                                               Convert.ToString(Powers[index1][index2].Level) + "]";
                    }

                    var tips1 = Tips;
                    var index5 = index1;
                    var index6 = index2;
                    tips1[index5][index6] = tips1[index5][index6] + "\r\n  Heal: " +
                                            Strings.Format(Values[index1][index2], "##0.##") + " HP/s";
                    if (num1 < (double) Values[index1][index2])
                        num1 = Values[index1][index2];
                }

                ++index1;
            } while (index1 <= 1);

            GraphMax = num1 * 1.025f;
            MidsContext.Archetype = archetype;
        }

        private void values_MaxTargets()
        {
            var num1 = 1f;
            var index1 = 0;
            do
            {
                var num2 = Powers[index1].Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    Values[index1][index2] = Powers[index1][index2].MaxTargets;
                    if (Math.Abs(Values[index1][index2]) < float.Epsilon)
                        continue;
                    Tips[index1][index2] =
                        DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID].DisplayName + ":" +
                        Powers[index1][index2].DisplayName;
                    if (Matching)
                    {
                        var tips = Tips;
                        var index3 = index1;
                        var index4 = index2;
                        tips[index3][index4] = tips[index3][index4] + " [Level " +
                                               Convert.ToString(Powers[index1][index2].Level) + "]";
                    }

                    if (Values[index1][index2] > 1.0)
                    {
                        var tips = Tips;
                        var index3 = index1;
                        var index4 = index2;
                        tips[index3][index4] = tips[index3][index4] + "\r\n  " +
                                               Convert.ToString(Values[index1][index2], CultureInfo.InvariantCulture) +
                                               " Targets Max.";
                    }
                    else
                    {
                        var tips = Tips;
                        var index3 = index1;
                        var index4 = index2;
                        tips[index3][index4] = tips[index3][index4] + "\r\n  " +
                                               Convert.ToString(Values[index1][index2], CultureInfo.InvariantCulture) +
                                               " Target Max.";
                    }

                    if (num1 < (double) Values[index1][index2])
                        num1 = Values[index1][index2];
                }

                ++index1;
            } while (index1 <= 1);

            GraphMax = num1 * 1.025f;
        }

        private void values_Range()
        {
            var num1 = 1f;
            var index1 = 0;
            do
            {
                var num2 = Powers[index1].Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    var str = "";
                    switch (Powers[index1][index2].EffectArea)
                    {
                        case Enums.eEffectArea.Character:
                            str = Convert.ToString(Powers[index1][index2].Range, CultureInfo.InvariantCulture) +
                                  "ft range.";
                            Values[index1][index2] = Powers[index1][index2].Range;
                            break;
                        case Enums.eEffectArea.Sphere:
                            Values[index1][index2] = Powers[index1][index2].Radius;
                            if (Powers[index1][index2].Range > 0.0)
                            {
                                str = Convert.ToString(Powers[index1][index2].Range, CultureInfo.InvariantCulture) +
                                      "ft range, ";
                                Values[index1][index2] = Powers[index1][index2].Range;
                            }

                            str = str + Convert.ToString(Powers[index1][index2].Radius, CultureInfo.InvariantCulture) +
                                  "ft radius.";
                            break;
                        case Enums.eEffectArea.Cone:
                            Values[index1][index2] = Powers[index1][index2].Range;
                            str = Convert.ToString(Powers[index1][index2].Range, CultureInfo.InvariantCulture) +
                                  "ft range, " + Convert.ToString(Powers[index1][index2].Arc) + " degree cone.";
                            break;
                        case Enums.eEffectArea.Location:
                            Values[index1][index2] = Powers[index1][index2].Range;
                            str = Convert.ToString(Powers[index1][index2].Range, CultureInfo.InvariantCulture) +
                                  "ft range, " +
                                  Convert.ToString(Powers[index1][index2].Radius, CultureInfo.InvariantCulture) +
                                  "ft radius.";
                            break;
                        case Enums.eEffectArea.Volume:
                            Values[index1][index2] = Powers[index1][index2].Radius;
                            if (Powers[index1][index2].Range > 0.0)
                            {
                                str = Convert.ToString(Powers[index1][index2].Range, CultureInfo.InvariantCulture) +
                                      "ft range, ";
                                Values[index1][index2] = Powers[index1][index2].Range;
                            }

                            str = str + Convert.ToString(Powers[index1][index2].Radius, CultureInfo.InvariantCulture) +
                                  "ft radius.";
                            break;
                    }

                    if (Math.Abs(Values[index1][index2]) < float.Epsilon)
                        continue;
                    Tips[index1][index2] =
                        DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID].DisplayName + ":" +
                        Powers[index1][index2].DisplayName;
                    if (Matching)
                    {
                        var tips = Tips;
                        var index3 = index1;
                        var index4 = index2;
                        tips[index3][index4] = tips[index3][index4] + " [Level " +
                                               Convert.ToString(Powers[index1][index2].Level) + "]";
                    }

                    var tips1 = Tips;
                    var index5 = index1;
                    var index6 = index2;
                    tips1[index5][index6] = tips1[index5][index6] + "\r\n  " + str;
                    if (num1 < (double) Values[index1][index2])
                        num1 = Values[index1][index2];
                }

                ++index1;
            } while (index1 <= 1);

            GraphMax = num1 * 1.025f;
        }

        private void values_Recharge()
        {
            var num1 = 1f;
            var index1 = 0;
            do
            {
                var num2 = Powers[index1].Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    Values[index1][index2] = Powers[index1][index2].RechargeTime;
                    if (Math.Abs(Values[index1][index2]) < float.Epsilon)
                        continue;
                    Tips[index1][index2] =
                        DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID].DisplayName + ":" +
                        Powers[index1][index2].DisplayName;
                    if (Matching)
                    {
                        var tips = Tips;
                        var index3 = index1;
                        var index4 = index2;
                        tips[index3][index4] = tips[index3][index4] + " [Level " +
                                               Convert.ToString(Powers[index1][index2].Level) + "]";
                    }

                    var tips1 = Tips;
                    var index5 = index1;
                    var index6 = index2;
                    tips1[index5][index6] = tips1[index5][index6] + "\r\n  " +
                                            Strings.Format(Values[index1][index2], "##0.##") + "s";
                    if (num1 < (double) Values[index1][index2])
                        num1 = Values[index1][index2];
                }

                ++index1;
            } while (index1 <= 1);

            GraphMax = num1 * 1.025f;
        }

        private void Values_Universal(Enums.eEffectType iEffectType, bool Sum, bool Debuff)
        {
            var archetype = MidsContext.Archetype;
            var num1 = 1f;
            var str = "";
            var index1 = 0;
            do
            {
                var num2 = Powers[index1].Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    MidsContext.Archetype = DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID];
                    var effectMagSum = Powers[index1][index2].GetEffectMagSum(iEffectType);
                    var index3 = 0;
                    if (effectMagSum.Present)
                    {
                        if (Debuff)
                        {
                            var num3 = effectMagSum.Index.Length - 1;
                            for (var index4 = 0; index4 <= num3; ++index4)
                                if (effectMagSum.Value[index4] > 0.0)
                                    effectMagSum.Value[index4] = 0.0f;
                                else
                                    effectMagSum.Value[index4] *= -1f;
                            effectMagSum.ReSum();
                            index3 = effectMagSum.Max;
                        }
                        else
                        {
                            var num3 = effectMagSum.Index.Length - 1;
                            for (var index4 = 0; index4 <= num3; ++index4)
                                if (effectMagSum.Value[index4] < 0.0)
                                    effectMagSum.Value[index4] = 0.0f;
                            index3 = effectMagSum.Max;
                            effectMagSum.ReSum();
                        }
                    }

                    if (!Sum)
                    {
                        if (effectMagSum.Present)
                        {
                            str = GetUniversalTipString(effectMagSum, ref Powers[index1][index2]);
                            Values[index1][index2] = effectMagSum.Value[index3];
                            if (Powers[index1][index2].Effects[effectMagSum.Index[index3]].DisplayPercentage)
                                Values[index1][index2] *= 100f;
                        }
                    }
                    else
                    {
                        if (effectMagSum.Present &&
                            Powers[index1][index2].Effects[effectMagSum.Index[index3]].DisplayPercentage)
                            effectMagSum.Multiply();
                        Values[index1][index2] = effectMagSum.Sum;
                    }

                    if (Math.Abs(Values[index1][index2]) < float.Epsilon)
                        continue;
                    {
                        Tips[index1][index2] =
                            DatabaseAPI.Database.Classes[Powers[index1][index2].ForcedClassID].DisplayName + ":" +
                            Powers[index1][index2].DisplayName;
                        if (Matching)
                        {
                            var tips = Tips;
                            var index4 = index1;
                            var index5 = index2;
                            tips[index4][index5] = tips[index4][index5] + " [Level " +
                                                   Convert.ToString(Powers[index1][index2].Level) + "]";
                        }

                        if (Sum)
                        {
                            str = "";
                            var num3 = effectMagSum.Index.Length - 1;
                            for (var index4 = 0; index4 <= num3; ++index4)
                            {
                                if (str != "")
                                    str += "\r\n";
                                str = str + "  " + Powers[index1][index2].Effects[effectMagSum.Index[index4]]
                                    .BuildEffectString().Replace("\r\n", "\r\n  ");
                            }

                            var tips = Tips;
                            var index5 = index1;
                            var index6 = index2;
                            tips[index5][index6] = tips[index5][index6] + "\r\n" + str;
                        }
                        else
                        {
                            var tips = Tips;
                            var index4 = index1;
                            var index5 = index2;
                            tips[index4][index5] = tips[index4][index5] + "\r\n  " + str;
                        }

                        if (num1 < (double) Values[index1][index2])
                            num1 = Values[index1][index2];
                    }
                }

                ++index1;
            } while (index1 <= 1);

            GraphMax = num1 * 1.025f;
            MidsContext.Archetype = archetype;
        }

        protected enum eDisplayItems
        {
            Accuracy,
            Damage,
            DamagePA,
            DamagePS,
            DamagePE,
            DamageBuff,
            Defense,
            DefenseDebuff,
            Duration,
            EndUse,
            EndUsePS,
            Heal,
            HealPS,
            HealPE,
            HitPoints,
            TargetCount,
            Range,
            Recharge,
            Regen,
            Resistance,
            ResistanceDebuff,
            ToHitBuff,
            ToHitDeBuff
        }
    }
}