using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms.Controls;
using MRBResourceLib;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmCompare : Form
    {
        private readonly string[] DisplayValueStrings;
        private readonly frmMain myParent;
        private readonly IPower?[][] Powers;
        private readonly string[][] Tips;
        private readonly float[][] Values;
        private Button btnTweakMatch;
        private ComboBox cbAT1;
        private ComboBox cbAT2;
        private ComboBox cbSet1;
        private ComboBox cbSet2;
        private ComboBox cbType1;
        private ComboBox cbType2;
        private CheckBox chkMatching;
        private CtlMultiGraph Graph;
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
            Icon = Resources.MRB_Icon_Concept;
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
            ListSets(0);
        }

        private void cbAT2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded)
                return;
            ListSets(1);
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
            ListSets(0);
        }

        private void cbType2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Loaded)
                return;
            ListSets(1);
        }

        private void chkMatching_CheckedChanged(object sender, EventArgs e)
        {
            Matching = chkMatching.Checked;
            if (!Loaded)
                return;
            DisplayGraph();
        }

        private void DisplayGraph()
        {
            if (lstDisplay.SelectedIndex < 0)
            {
                return;
            }

            Graph.BeginUpdate();
            Graph.Clear();
            GetPowers();
            if (Matching)
            {
                MapAdvanced();
            }
            else
            {
                MapSimple();
            }

            switch (lstDisplay.SelectedIndex)
            {
                case 0:
                    Graph.ColorFadeEnd = Color.FromArgb(255, 255, 0);
                    ValuesAccuracy();
                    break;
                case 1:
                    Graph.ColorFadeEnd = Color.Red;
                    ValuesDamage();
                    break;
                case 2:
                    Graph.ColorFadeEnd = Color.Red;
                    ValuesDpa();
                    break;
                case 3:
                    Graph.ColorFadeEnd = Color.Red;
                    ValuesDps();
                    break;
                case 4:
                    Graph.ColorFadeEnd = Color.Red;
                    ValuesDpe();
                    break;
                case 5:
                    Graph.ColorFadeEnd = Color.FromArgb(192, 0, 0);
                    ValuesUniversal(Enums.eEffectType.DamageBuff, false, false);
                    break;
                case 6:
                    Graph.ColorFadeEnd = Color.FromArgb(192, 0, 192);
                    ValuesUniversal(Enums.eEffectType.Defense, false, false);
                    break;
                case 7:
                    Graph.ColorFadeEnd = Color.FromArgb(128, 0, 128);
                    ValuesUniversal(Enums.eEffectType.Defense, false, true);
                    break;
                case 8:
                    Graph.ColorFadeEnd = Color.FromArgb(128, 0, 255);
                    ValuesDuration();
                    break;
                case 9:
                    Graph.ColorFadeEnd = Color.FromArgb(192, 192, 255);
                    ValuesEnd();
                    break;
                case 10:
                    Graph.ColorFadeEnd = Color.FromArgb(192, 192, 255);
                    ValuesEps();
                    break;
                case 11:
                    Graph.ColorFadeEnd = Color.FromArgb(96, 255, 96);
                    ValuesHeal();
                    break;
                case 12:
                    Graph.ColorFadeEnd = Color.FromArgb(96, 255, 96);
                    ValuesHps();
                    break;
                case 13:
                    Graph.ColorFadeEnd = Color.FromArgb(96, 255, 96);
                    ValuesHpe();
                    break;
                case 14:
                    Graph.ColorFadeEnd = Color.FromArgb(96, 255, 96);
                    ValuesUniversal(Enums.eEffectType.HitPoints, true, false);
                    break;
                case 15:
                    Graph.ColorFadeEnd = Color.FromArgb(64, 128, 128);
                    ValuesMaxTargets();
                    break;
                case 16:
                    Graph.ColorFadeEnd = Color.FromArgb(96, 128, 96);
                    ValuesRange();
                    break;
                case 17:
                    Graph.ColorFadeEnd = Color.FromArgb(255, 192, 128);
                    ValuesRecharge();
                    break;
                case 18:
                    Graph.ColorFadeEnd = Color.FromArgb(96, 255, 96);
                    ValuesUniversal(Enums.eEffectType.Regeneration, true, false);
                    break;
                case 19:
                    Graph.ColorFadeEnd = Color.FromArgb(0, 192, 192);
                    ValuesUniversal(Enums.eEffectType.Resistance, false, false);
                    break;
                case 20:
                    Graph.ColorFadeEnd = Color.FromArgb(0, 128, 128);
                    ValuesUniversal(Enums.eEffectType.Resistance, false, true);
                    break;
                case 21:
                    Graph.ColorFadeEnd = Color.FromArgb(255, 255, 96);
                    ValuesUniversal(Enums.eEffectType.ToHit, true, false);
                    break;
                case 22:
                    Graph.ColorFadeEnd = Color.FromArgb(192, 192, 64);
                    ValuesUniversal(Enums.eEffectType.ToHit, true, true);
                    break;
            }

            for (var index1 = 0; index1 < 20; index1++)
            {
                string[] powerDisplays = {"", ""};
                var values = new float[2];
                var iTip = "";
                for (var mapIdx = 0; mapIdx < 2; mapIdx++)
                {
                    if (Map.Map[index1, mapIdx] <= -1)
                    {
                        continue;
                    }

                    powerDisplays[mapIdx] = Powers[mapIdx][Map.Map[index1, mapIdx]].DisplayName;
                    values[mapIdx] = Values[mapIdx][Map.Map[index1, mapIdx]];
                    if (iTip != "" & Tips[mapIdx][Map.Map[index1, mapIdx]] != "")
                    {
                        iTip += "\r\n----------\r\n";
                    }

                    iTip += Tips[mapIdx][Map.Map[index1, mapIdx]];
                }

                Graph.AddItemPair(powerDisplays[0], powerDisplays[1], values[0], values[1], iTip);
            }

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
            this.EventHandlerWithCatch(() => myParent.FloatCompareGraph(false));
        }

        private void frmCompare_KeyDown(object sender, KeyEventArgs e)
        {
            this.EventHandlerWithCatch(() =>
            {
                if (!(e.Control & e.Shift & e.KeyCode == Keys.T))
                {
                    return;
                }

                btnTweakMatch.Visible = true;
            });
        }

        private void frmCompare_Load(object sender, EventArgs e)
        {
            FillDisplayList();
            UpdateData();
            ListAt();
            if (MidsContext.Character.Archetype.Idx > -1)
            {
                cbAT1.SelectedIndex = MidsContext.Character.Archetype.Idx;
            }

            if (MidsContext.Character.Archetype.Idx > -1)
            {
                cbAT2.SelectedIndex = MidsContext.Character.Archetype.Idx;
            }

            tbScaleX.Minimum = 0;
            tbScaleX.Maximum = Graph.ScaleCount - 1;
            ListType();
            ListSets(0);
            ListSets(1);
            Map.Init();
            chkMatching.Checked = Matching;
            ibExKeepOnTop.ToggleState = ImageButtonEx.States.ToggledOn;
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
            this.EventHandlerWithCatch(() => Graph.BackColor = BackColor);
        }

        private void ibExKeepOnTop_Click(object sender, EventArgs e)
        {
            TopMost = ibExKeepOnTop.ToggleState == ImageButtonEx.States.ToggledOn;
        }

        private void ibExClose_Click(object sender, EventArgs e)
        {
            Close();
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

        private int GetMax(int a, int b)
        {
            return a <= b ? b : a;
        }

        private int GetNextFreeSlot()
        {
            var index = 0;
            while (Map.Map[index, 1] != -1)
            {
                ++index;
                if (index > 20)
                {
                    return 20;
                }
            }

            return index;
        }

        // clsToonX.GBPA_ApplyPowerOverride()
        private void ApplyPowerOverride(ref IPower ret)
        {
            if (!ret.HasPowerOverrideEffect)
            {
                return;
            }

            foreach (var fx in ret.Effects)
            {
                if (fx.EffectType != Enums.eEffectType.PowerRedirect || !(fx.nOverride > -1 & Math.Abs(fx.Probability - 1) < 0.01 & fx.CanInclude()))
                {
                    continue;
                }

                ret = new Power(DatabaseAPI.Database.Power[fx.nOverride])
                {
                    Level = ret.Level
                };
            }
        }

        private void GetPowers()
        {
            var numArray = new int[2];
            for (var index = 0; index < 2; index++)
            {
                numArray[index] = GetSetIndex(index);
                Powers[index] = new IPower[DatabaseAPI.Database.Powersets[numArray[index]].Powers.Length];
                Values[index] = new float[Powers[index].Length + 1];
                Tips[index] = new string[Powers[index].Length + 1];
                var nIDClass = index != 0 ? cbAT2.SelectedIndex : cbAT1.SelectedIndex;
                for (var i = 0; i < Powers[index].Length; i++)
                {
                    Powers[index][i] = new Power(DatabaseAPI.Database.Powersets[numArray[index]].Powers[i]);
                    ApplyPowerOverride(ref Powers[index][i]);
                    Powers[index][i].AbsorbPetEffects();
                    Powers[index][i].ApplyGrantPowerEffects();
                    Powers[index][i].ProcessExecutes();

                    if (nIDClass <= -1)
                    {
                        continue;
                    }

                    Powers[index][i].ForcedClassID = nIDClass;
                    Powers[index][i].ForcedClass = DatabaseAPI.UidFromNidClass(nIDClass);
                }
            }
        }

        private int GetSetIndex(int index)
        {
            ComboBox comboBox;
            switch (index)
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

            return DatabaseAPI.GetPowersetIndexes(getAT(index), GetSetType(index))[comboBox.SelectedIndex].nID;
        }

        private Enums.ePowerSetType GetSetType(int Index)
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

        private string GetUniversalTipString(Enums.ShortFX iSFX, ref IPower? iPower)
        {
            var str1 = "";
            var str2 = "";
            if (!iSFX.Present)
            {
                return str2;
            }

            var str3 = "";
            IPower power = new Power(iPower);
            foreach (var sfx in iSFX.Index)
            {
                if (sfx == -1 || power.Effects[sfx].EffectType == Enums.eEffectType.None)
                {
                    continue;
                }

                var returnString = "";
                var returnMask = Array.Empty<int>();
                power.GetEffectStringGrouped(sfx, ref returnString, ref returnMask, false, false);
                if (returnMask.Length <= 0)
                {
                    continue;
                }

                if (str3 != "")
                {
                    str3 += "\r\n  ";
                }

                str3 += returnString.Replace("\r\n", "\r\n  ");
                foreach (var m in returnMask)
                {
                    power.Effects[m].EffectType = Enums.eEffectType.None;
                }
            }

            foreach (var sfx in iSFX.Index)
            {
                if (power.Effects[sfx].EffectType == Enums.eEffectType.None)
                {
                    continue;
                }

                if (str3 != "")
                {
                    str3 += "\r\n  ";
                }

                str3 += power.Effects[sfx].BuildEffectString().Replace("\r\n", "\r\n  ");
            }

            return str1 + str3;
        }

        private void Graph_Load(object sender, EventArgs e)
        {
        }

        private void ListAt()
        {
            cbAT1.BeginUpdate();
            cbAT1.Items.Clear();
            cbAT2.BeginUpdate();
            cbAT2.Items.Clear();
            foreach (var c in DatabaseAPI.Database.Classes)
            {
                if (!c.Playable)
                {
                    continue;
                }

                cbAT1.Items.Add(c.DisplayName);
                cbAT2.Items.Add(c.DisplayName);
            }

            cbAT1.SelectedIndex = MidsContext.Character.Archetype.Idx;
            cbAT2.SelectedIndex = MidsContext.Character.Archetype.Idx;
            cbAT1.EndUpdate();
            cbAT2.EndUpdate();
        }

        private void ListSets(int index)
        {
            var comboBox1 = index == 0 ? cbSet1 : cbSet2;
            var comboBox2 = index == 0 ? cbType1 : cbType2;
            var selectedIndex = index == 0 ? cbAT1.SelectedIndex : cbAT2.SelectedIndex;
            
            if (index == 0)
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

            var iSet = comboBox2.SelectedIndex switch
            {
                0 => Enums.ePowerSetType.Primary,
                1 => Enums.ePowerSetType.Secondary,
                2 => Enums.ePowerSetType.Ancillary,
                _ => Enums.ePowerSetType.None
            };

            comboBox1.BeginUpdate();
            comboBox1.Items.Clear();
            var powersetIndexes = DatabaseAPI.GetPowersetIndexes(selectedIndex, iSet);
            foreach (var pi in powersetIndexes)
            {
                comboBox1.Items.Add(pi.DisplayName);
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }

            comboBox1.EndUpdate();
        }

        private void ListType()
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
            {
                return;
            }

            ResetScale();
            DisplayGraph();
        }

        private void MapAdvanced()
        {
            var placed = new bool[Powers[1].Length];
            for (var index = 0; index < placed.Length; index++)
            {
                placed[index] = false;
            }

            Map.Init();
            for (var index = 0; index < Math.Min(20, GetMax(Powers[0].Length, Powers[1].Length)); index++)
            {
                if (Powers[0].Length > index)
                {
                    Map.Map[index, 0] = index;
                }
            }

            for (var index1 = 0; index1 < Powers[1].Length; index1++)
            {
                var displayName = Powers[1][index1].DisplayName;
                for (var index2 = 0; index2 < Powers[0].Length; index2++)
                {
                    if (!(string.Equals(Powers[0][index2].DisplayName, displayName, StringComparison.OrdinalIgnoreCase) & !placed[index1]))
                    {
                        continue;
                    }

                    Map.Map[index2, 1] = index1;
                    placed[index1] = true;
                    break;
                }
            }

            MapOverride();
            MapDescString(new[] { "summon" }, ref placed);
            MapDescString(new[] { "toggle", "+def", "smash" }, ref placed);
            MapDescString(new[] { "toggle", "+def", "energy" }, ref placed);
            MapDescString(new[] { "toggle", "+def", "fire" }, ref placed);
            MapDescString(new[] { "toggle", "+def", "ranged" }, ref placed);
            MapDescString(new[] { "toggle", "+def", "melee" }, ref placed);
            MapDescString(new[] { "toggle", "+def", "aoe" }, ref placed);
            MapDescString(new[] { "auto", "+def", "smash" }, ref placed);
            MapDescString(new[] { "auto", "+def", "energy" }, ref placed);
            MapDescString(new[] { "auto", "+def", "fire" }, ref placed);
            MapDescString(new[] { "auto", "+def", "ranged" }, ref placed);
            MapDescString(new[] { "auto", "+def", "melee" }, ref placed);
            MapDescString(new[] { "auto", "+def", "aoe" }, ref placed);
            MapDescString(new[] { "toggle", "+res", "smash" }, ref placed);
            MapDescString(new[] { "toggle", "+res", "energy" }, ref placed);
            MapDescString(new[] { "toggle", "+res", "fire" }, ref placed);
            MapDescString(new[] { "auto", "+res", "smash" }, ref placed);
            MapDescString(new[] { "auto", "+res", "energy" }, ref placed);
            MapDescString(new[] { "auto", "+res", "fire" }, ref placed);
            MapDescString(new[] { "toggle", "+def" }, ref placed);
            MapDescString(new[] { "toggle", "+res" }, ref placed);
            MapDescString(new[] { "auto", "+def" }, ref placed);
            MapDescString(new[] { "auto", "+res" }, ref placed);
            MapDescString(new[] { "AoE", "disorient" }, ref placed);
            MapDescString(new[] { "AoE", "stun" }, ref placed);
            MapDescString(new[] { "AoE", "hold" }, ref placed);
            MapDescString(new[] { "AoE", "sleep" }, ref placed);
            MapDescString(new[]
            {
                "AoE",
                "immobilize"
            }, ref placed);
            MapDescString(new[] { "AoE", "confuse" }, ref placed);
            MapDescString(new[] { "AoE", "fear" }, ref placed);
            MapDescString(new[]
            {
                "Cone",
                "disorient"
            }, ref placed);
            MapDescString(new[] { "Cone", "stun" }, ref placed);
            MapDescString(new[] { "Cone", "hold" }, ref placed);
            MapDescString(new[] { "Cone", "sleep" }, ref placed);
            MapDescString(new[]
            {
                "Cone",
                "immobilize"
            }, ref placed);
            MapDescString(new[] { "Cone", "confuse" }, ref placed);
            MapDescString(new[] { "Cone", "fear" }, ref placed);
            MapDescString(new[] { "snipe" }, ref placed);
            MapDescString(new[]
            {
                "AoE",
                "Extreme",
                "Self -Recovery"
            }, ref placed);
            MapDescString(new[] { "close", "high" }, ref placed);
            MapDescString(new[]
            {
                "ranged",
                "disorient",
                "minor"
            }, ref placed);
            MapDescString(new[] { "ranged", "hold" }, ref placed);
            MapDescString(new[] { "cone", "extreme" }, ref placed);
            MapDescString(new[] { "cone", "superior" }, ref placed);
            MapDescString(new[] { "cone", "high" }, ref placed);
            MapDescString(new[] { "cone", "moderate" }, ref placed);
            MapDescString(new[] { "cone", "minor" }, ref placed);
            MapDescString(new[]
            {
                "ranged",
                "AoE",
                "extreme"
            }, ref placed);
            MapDescString(new[]
            {
                "ranged",
                "AoE",
                "superior"
            }, ref placed);
            MapDescString(new[]
            {
                "ranged",
                "AoE",
                "high"
            }, ref placed);
            MapDescString(new[]
            {
                "ranged",
                "AoE",
                "moderate"
            }, ref placed);
            MapDescString(new[]
            {
                "ranged",
                "AoE",
                "minor"
            }, ref placed);
            MapDescString(new[] { "AoE", "extreme" }, ref placed);
            MapDescString(new[] { "AoE", "superior" }, ref placed);
            MapDescString(new[] { "AoE", "high" }, ref placed);
            MapDescString(new[] { "AoE", "moderate" }, ref placed);
            MapDescString(new[] { "AoE", "minor" }, ref placed);
            MapDescString(new[] { "melee", "extreme" }, ref placed);
            MapDescString(new[]
            {
                "melee",
                "superior"
            }, ref placed);
            MapDescString(new[] { "melee", "high" }, ref placed);
            MapDescString(new[]
            {
                "melee",
                "moderate"
            }, ref placed);
            MapDescString(new[] { "melee", "minor" }, ref placed);
            MapDescString(new[]
            {
                "melee",
                "disorient"
            }, ref placed);
            MapDescString(new[] { "melee", "stun" }, ref placed);
            MapDescString(new[] { "melee", "hold" }, ref placed);
            MapDescString(new[] { "AoE", "knockback" }, ref placed);
            MapDescString(new[] { "AoE", "knockup" }, ref placed);
            MapDescString(new[]
            {
                "Cone",
                "knockback"
            }, ref placed);
            MapDescString(new[] { "Cone", "knockup" }, ref placed);
            MapDescString(new[] { "AoE", "stealth" }, ref placed);
            MapDescString(new[] { "stealth" }, ref placed);
            MapDescString(new[] { "toggle", "-def" }, ref placed);
            MapDescString(new[] { "toggle", "-res" }, ref placed);
            MapDescString(new[] { "toggle", "-acc" }, ref placed);
            MapDescString(new[] { "toggle", "-dmg" }, ref placed);
            MapDescString(new[] { "-def" }, ref placed);
            MapDescString(new[] { "-res" }, ref placed);
            MapDescString(new[] { "-acc" }, ref placed);
            MapDescString(new[] { "-dmg" }, ref placed);
            MapDescString(new[] { "+dmg" }, ref placed);
            MapDescString(new[] { "+acc" }, ref placed);
            MapDescString(new[] { "heal", "team" }, ref placed);
            MapDescString(new[] { "heal", "ally" }, ref placed);
            MapDescString(new[] { "heal" }, ref placed);
            MapDescString(new[] { "+recovery" }, ref placed);
            MapDescString(new[] { "-recovery" }, ref placed);
            MapDescString(new[] { "-regen" }, ref placed);
            MapDescString(new[] { "extreme" }, ref placed);
            MapDescString(new[] { "superior" }, ref placed);
            MapDescString(new[] { "high" }, ref placed);
            MapDescString(new[] { "moderate" }, ref placed);
            MapDescString(new[] { "minor" }, ref placed);
            MapDescString(new[] { "disorient" }, ref placed);
            MapDescString(new[] { "stun" }, ref placed);
            MapDescString(new[] { "hold" }, ref placed);
            MapDescString(new[] { "sleep" }, ref placed);
            MapDescString(new[] { "immobilize" }, ref placed);
            MapDescString(new[] { "confuse" }, ref placed);
            MapDescString(new[] { "fear" }, ref placed);
            MapDescString(new[] { "cone" }, ref placed);
            MapDescString(new[] { "aoe" }, ref placed);
            MapDescString(new[] { "melee" }, ref placed);
            MapDescString(new[] { "ranged" }, ref placed);
            for (var index = 0; index < Powers[1].Length; index++)
            {
                if (placed[index] || Map.Map[index, 1] != -1)
                {
                    continue;
                }

                Map.Map[index, 1] = index;
                placed[index] = true;
            }

            for (var index = 0; index < Powers[1].Length; index++)
            {
                if (placed[index])
                {
                    continue;
                }

                Map.Map[GetNextFreeSlot(), 1] = index;
                placed[index] = true;
            }
        }

        private void MapSimple()
        {
            Map.Init();

            for (var index = 0; index < 1; index++)
            {
                Map.IdxAT[index] = getAT(index);
                Map.IdxSet[index] = GetSetIndex(index);
            }

            for (var index = 0; index < Math.Min(20, GetMax(Powers[0].Length, Powers[1].Length)); index++)
            {
                if (Powers[0].Length > index)
                {
                    Map.Map[index, 0] = index;
                }

                if (Powers[1].Length > index)
                {
                    Map.Map[index, 1] = index;
                }
            }
        }

        private int MapDescString(string[] iStrings, ref bool[] placed)
        {
            for (var powerIdx = 0; powerIdx < Powers[1].Length; powerIdx++)
            {
                var flag1 = iStrings.All(s => Powers[1][powerIdx]?.DescShort.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0);

                if (!(flag1 & !placed[powerIdx]))
                {
                    continue;
                }

                for (var index2 = 0; index2 < Powers[0].Length; index2++)
                {
                    var flag2 = Map.Map[index2, 1] < 0;
                    if (iStrings.Any(s => Powers[0][index2]?.DescShort.IndexOf(s, StringComparison.OrdinalIgnoreCase) < 0))
                    {
                        flag2 = false;
                    }

                    if (!flag2)
                    {
                        continue;
                    }

                    Map.Map[index2, 1] = powerIdx;
                    placed[powerIdx] = true;

                    return powerIdx;
                }
            }

            return 0;
        }

        private void MapOverride()
        {
            for (var index1 = 0; index1 < MidsContext.Config.CompOverride.Length; index1++)
            {
                var compOverride = MidsContext.Config.CompOverride;
                MapOverrideDo(compOverride[index1].Powerset, compOverride[index1].Power, compOverride[index1].Override);
            }
        }

        private void MapOverrideDo(string iSet, string iPower, string iNewStr)
        {
            var index1 = 0;
            for (var i = 0; i < 2; i++)
            {
                for (var index2 = 0; index2 < Powers[index1].Length; index2++)
                {
                    if (Powers[index1][index2].PowerSetID > -1 &&
                        string.Equals(Powers[index1][index2].DisplayName, iPower, StringComparison.OrdinalIgnoreCase) &
                        string.Equals(DatabaseAPI.Database.Powersets[Powers[index1][index2].PowerSetID].DisplayName,
                            iSet, StringComparison.OrdinalIgnoreCase))
                    {
                        Powers[index1][index2].DescShort = iNewStr.ToUpper();
                    }
                }
            }
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
            {
                rectangle.X = (int)Math.Round((Screen.PrimaryScreen.Bounds.Width - Width) / 2f);
            }

            if (rectangle.Y < 32)
            {
                rectangle.Y = (int)Math.Round((Screen.PrimaryScreen.Bounds.Height - Height) / 2f);
            }

            Top = rectangle.Y;
            Left = rectangle.X;
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
            if (!Loaded)
            {
                return;
            }

            ResetScale();
            DisplayGraph();
        }

        private void ValuesAccuracy()
        {
            var scaleFactor = 1f;
            for (var i = 0 ; i < 2 ; i++)
            {
                for (var j = 0; j < Powers[i].Length; j++)
                {
                    if (Powers[i][j] == null)
                    {
                        Tips[i][j] = "";
                        Values[i][j] = 0;

                        continue;
                    }

                    //Values[i][j] = !((Powers[i][j].EntitiesAutoHit == Enums.eEntity.None) | Powers[i][j].Effects.Any(e => e.RequiresToHitCheck))
                    //    ? 0.01f
                    //    : Powers[i][j].Accuracy * MidsContext.Config.ScalingToHit * 100;
                    Values[i][j] = Powers[i][j].Accuracy * MidsContext.Config.ScalingToHit * 100;
                    if (Math.Abs(Values[i][j]) < float.Epsilon)
                    {
                        continue;
                    }

                    Tips[i][j] = $"{DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID].DisplayName}:{Powers[i][j].DisplayName}";
                    if (Matching)
                    {
                        Tips[i][j] += $" [Level {Powers[i][j].Level}]";
                    }

                    Tips[i][j] += $"\r\n {Values[i][j]:##0.##}% base Accuracy";
                    Tips[i][j] += $"\r\n (Real Numbers style: {Powers[i][j].Accuracy:##0.##}x)";
                    if (Values[i][j] > scaleFactor)
                    {
                        scaleFactor = Values[i][j];
                    }
                }
            }

            GraphMax = scaleFactor * 1.025f;
        }

        private void ValuesDamage()
        {
            var scaleFactor = 1f;
            var returnValue = MidsContext.Config.DamageMath.ReturnValue;
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.Numeric;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < Powers[i].Length; j++)
                {
                    if (Powers[i][j] == null)
                    {
                        Tips[i][j] = "";
                        Values[i][j] = 0;

                        continue;
                    }

                    Values[i][j] = Powers[i][j].FXGetDamageValue();
                    if (Math.Abs(Values[i][j]) < float.Epsilon)
                    {
                        continue;
                    }

                    Tips[i][j] = $"{DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID].DisplayName}:{Powers[i][j].DisplayName}";
                    if (Matching)
                    {
                        Tips[i][j] += $" [Level {Convert.ToString(Powers[i][j].Level)}]";
                    }

                    Tips[i][j] += $"\r\n  {Powers[i][j].FXGetDamageString()}";
                    if (Values[i][j] > scaleFactor)
                    {
                        scaleFactor = Values[i][j];
                    }

                    if (Powers[i][j].PowerType != Enums.ePowerType.Toggle)
                    {
                        continue;
                    }

                    Tips[i][j] += $"\r\n  (Applied every {Powers[i][j].ActivatePeriod}s)";
                }

            }

            GraphMax = scaleFactor * 1.025f;
            MidsContext.Config.DamageMath.ReturnValue = returnValue;
        }

        private void ValuesDpa()
        {
            var scaleFactor = 1f;
            var returnValue = MidsContext.Config.DamageMath.ReturnValue;
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.DPA;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < Powers[i].Length; j++)
                {
                    if (Powers[i][j] == null)
                    {
                        Tips[i][j] = "";
                        Values[i][j] = 0;

                        continue;
                    }

                    Values[i][j] = Powers[i][j].FXGetDamageValue();
                    if (Math.Abs(Values[i][j]) < float.Epsilon)
                    {
                        continue;
                    }

                    Tips[i][j] = $"{DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID].DisplayName}:{Powers[i][j].DisplayName}";
                    if (Matching)
                    {
                        Tips[i][j] += $" [Level {Convert.ToString(Powers[i][j].Level)}]";
                    }

                    Tips[i][j] += $"\r\n  {Powers[i][j].FXGetDamageString()}/s";
                    if (Values[i][j] > scaleFactor)
                    {
                        scaleFactor = Values[i][j];
                    }
                }
            }

            GraphMax = scaleFactor * 1.025f;
            MidsContext.Config.DamageMath.ReturnValue = returnValue;
        }

        private void ValuesDpe()
        {
            var scaleFactor = 1f;
            var returnValue = MidsContext.Config.DamageMath.ReturnValue;
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.Numeric;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < Powers[i].Length; j++)
                {
                    if (Powers[i][j] == null)
                    {
                        Tips[i][j] = "";
                        Values[i][j] = 0;

                        continue;
                    }

                    Values[i][j] = Powers[i][j].FXGetDamageValue();
                    if (Math.Abs(Values[i][j]) < float.Epsilon)
                    {
                        continue;
                    }

                    if (Powers[i][j].PowerType == Enums.ePowerType.Click &&
                        Powers[i][j].EndCost > 0)
                    {
                        Values[i][j] /= Powers[i][j].EndCost;
                    }

                    Tips[i][j] = $"{DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID].DisplayName}:{Powers[i][j].DisplayName}";
                    if (Matching)
                    {
                        Tips[i][j] += $" [Level {Convert.ToString(Powers[i][j].Level)}]";
                    }

                    Tips[i][j] += $"\r\n  {Powers[i][j].FXGetDamageString()}";
                    if (Values[i][j] > scaleFactor)
                    {
                        scaleFactor = Values[i][j];
                    }

                    Tips[i][j] += $" - DPE: {Values[i][j]:##0.##}";
                }
            }

            GraphMax = scaleFactor * 1.025f;
            MidsContext.Config.DamageMath.ReturnValue = returnValue;
        }

        private void ValuesDps()
        {
            var scaleFactor = 1f;
            var returnValue = MidsContext.Config.DamageMath.ReturnValue;
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.DPS;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < Powers[i].Length; j++)
                {
                    if (Powers[i][j] == null)
                    {
                        Tips[i][j] = "";
                        Values[i][j] = 0;

                        continue;
                    }

                    Values[i][j] = Powers[i][j].FXGetDamageValue();
                    if (Math.Abs(Values[i][j]) < float.Epsilon)
                    {
                        continue;
                    }

                    Tips[i][j] = $"{DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID].DisplayName}:{Powers[i][j].DisplayName}";
                    if (Matching)
                    {
                        Tips[i][j] += $" [Level {Powers[i][j].Level}]";
                    }

                    Tips[i][j] += $"\r\n  {Powers[i][j].FXGetDamageString()}/s";
                    if (Values[i][j] > scaleFactor)
                    {
                        scaleFactor = Values[i][j];
                    }
                }
            }

            GraphMax = scaleFactor * 1.025f;
            MidsContext.Config.DamageMath.ReturnValue = returnValue;
        }

        private void ValuesDuration()
        {
            var scaleFactor = 1f;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < Powers[i].Length; j++)
                {
                    if (Powers[i][j] == null)
                    {
                        Tips[i][j] = "";
                        Values[i][j] = 0;

                        continue;
                    }

                    var durationEffectId = Powers[i][j].GetDurationEffectID();
                    if (durationEffectId <= -1)
                    {
                        continue;
                    }

                    Values[i][j] = Powers[i][j].Effects[durationEffectId].Duration;
                    if (Math.Abs(Values[i][j]) < float.Epsilon)
                    {
                        continue;
                    }

                    Tips[i][j] = $"{DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID].DisplayName}:{Powers[i][j].DisplayName}";
                    if (Matching)
                    {
                        Tips[i][j] += $" [Level {Convert.ToString(Powers[i][j].Level)}]";
                    }

                    Tips[i][j] += $"\r\n  {Powers[i][j].Effects[durationEffectId].BuildEffectString()}";
                    if (Values[i][j] > scaleFactor)
                    {
                        scaleFactor = Values[i][j];
                    }
                }
            }

            GraphMax = scaleFactor * 1.025f;
        }

        private void ValuesEnd()
        {
            var scaleFactor = 1f;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < Powers[i].Length; j++)
                {
                    if (Powers[i][j] == null)
                    {
                        Tips[i][j] = "";
                        Values[i][j] = 0;

                        continue;
                    }

                    Values[i][j] = Powers[i][j].EndCost;
                    if (Math.Abs(Values[i][j]) < float.Epsilon)
                    {
                        continue;
                    }

                    Tips[i][j] = $"{DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID].DisplayName}:{Powers[i][j].DisplayName}";
                    if (Matching)
                    {
                        Tips[i][j] += $" [Level {Powers[i][j].Level}]";
                    }

                    Tips[i][j] += $"\r\n End: {Powers[i][j].EndCost:##0.##}";
                    if (Powers[i][j].PowerType == Enums.ePowerType.Toggle)
                    {
                        Tips[i][j] += " (Per Second)";
                    }

                    if (Values[i][j] > scaleFactor)
                    {
                        scaleFactor = Values[i][j];
                    }
                }
            }

            GraphMax = scaleFactor * 1.025f;
        }

        private void ValuesEps()
        {
            var scaleFactor = 1f;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < Powers[i].Length; j++)
                {
                    if (Powers[i][j] == null)
                    {
                        Tips[i][j] = "";
                        Values[i][j] = 0;

                        continue;
                    }

                    Values[i][j] = Powers[i][j].EndCost;
                    if (Math.Abs(Values[i][j]) < float.Epsilon)
                    {
                        continue;
                    }

                    switch (Powers[i][j].PowerType)
                    {
                        case Enums.ePowerType.Click:
                        {
                            if (Powers[i][j].RechargeTime + Powers[i][j].CastTime +
                                Powers[i][j].InterruptTime > 0)
                            {
                                Values[i][j] = Powers[i][j].EndCost /
                                                         (Powers[i][j].RechargeTime +
                                                          Powers[i][j].CastTime +
                                                          Powers[i][j].InterruptTime);
                            }

                            break;
                        }
                        case Enums.ePowerType.Toggle:
                            Values[i][j] = Powers[i][j].EndCost / Powers[i][j].ActivatePeriod;
                            break;
                    }

                    Tips[i][j] = $"{DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID].DisplayName}:{Powers[i][j].DisplayName}";
                    if (Matching)
                    {
                        Tips[i][j] += $" [Level {Powers[i][j].Level}]";
                    }

                    Tips[i][j] += $"\r\n  End: {Values[i][j]:##0.##}/s";
                    if (Values[i][j] > scaleFactor)
                    {
                        scaleFactor = Values[i][j];
                    }
                }
            }

            GraphMax = scaleFactor * 1.025f;
        }

        private void ValuesHeal()
        {
            var archetype = MidsContext.Archetype;
            var scaleFactor = 1f;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < Powers[i].Length; j++)
                {
                    if (Powers[i][j] == null)
                    {
                        Tips[i][j] = "";
                        Values[i][j] = 0;

                        continue;
                    }

                    MidsContext.Archetype = DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID];
                    var effectMagSum = Powers[i][j].GetEffectMagSum(Enums.eEffectType.Heal);
                    Values[i][j] = effectMagSum.Sum;
                    if (Math.Abs(Values[i][j]) < float.Epsilon)
                    {
                        continue;
                    }

                    Tips[i][j] = $"{MidsContext.Archetype.DisplayName}:{Powers[i][j].DisplayName}";
                    if (Matching)
                    {
                        Tips[i][j] += $" [Level {Powers[i][j].Level}]";
                    }

                    Tips[i][j] += $"\r\n  {Powers[i][j].Effects[effectMagSum.Index[0]].BuildEffectString()}";
                    if (Values[i][j] > scaleFactor)
                    {
                        scaleFactor = Values[i][j];
                    }
                }

                ++i;
            }

            GraphMax = scaleFactor * 1.025f;
            MidsContext.Archetype = archetype;
        }

        private void ValuesHpe()
        {
            var archetype = MidsContext.Archetype;
            var scaleFactor = 1f;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < Powers[i].Length; j++)
                {
                    if (Powers[i][j] == null)
                    {
                        Tips[i][j] = "";
                        Values[i][j] = 0;

                        continue;
                    }

                    MidsContext.Archetype = DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID];
                    var effectMagSum = Powers[i][j].GetEffectMagSum(Enums.eEffectType.Heal);
                    Values[i][j] = effectMagSum.Sum;
                    if (Math.Abs(Values[i][j]) < float.Epsilon)
                    {
                        continue;
                    }

                    if (Powers[i][j].EndCost > 0)
                    {
                        Values[i][j] /= Powers[i][j].EndCost;
                    }

                    Tips[i][j] = $"{DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID].DisplayName}:{Powers[i][j].DisplayName}";
                    if (Matching)
                    {
                        Tips[i][j] += $" [Level {Powers[i][j].Level}]";
                    }

                    Tips[i][j] += $"\r\n Heal: {Values[i][j]:##0.##} HP per unit of end.";
                    if (Values[i][j] < scaleFactor)
                    {
                        scaleFactor = Values[i][j];
                    }
                }
            }

            GraphMax = scaleFactor * 1.025f;
            MidsContext.Archetype = archetype;
        }

        private void ValuesHps()
        {
            var archetype = MidsContext.Archetype;
            var scaleFactor = 1f;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < Powers[i].Length; j++)
                {
                    if (Powers[i][j] == null)
                    {
                        Tips[i][j] = "";
                        Values[i][j] = 0;

                        continue;
                    }

                    MidsContext.Archetype = DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID];
                    var effectMagSum = Powers[i][j].GetEffectMagSum(Enums.eEffectType.Heal);
                    Values[i][j] = effectMagSum.Sum;
                    if (Math.Abs(Values[i][j]) < float.Epsilon)
                    {
                        continue;
                    }

                    if (Powers[i][j].PowerType == Enums.ePowerType.Click &&
                        Powers[i][j].RechargeTime + Powers[i][j].CastTime +
                        Powers[i][j].InterruptTime > 0)
                    {
                        Values[i][j] /= Powers[i][j].RechargeTime +
                                                  Powers[i][j].CastTime +
                                                  Powers[i][j].InterruptTime;
                    }

                    Tips[i][j] = $"{DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID].DisplayName}:{Powers[i][j].DisplayName}";
                    if (Matching)
                    {
                        Tips[i][j] += $" [Level {Powers[i][j].Level}]";
                    }

                    Tips[i][j] += $"\r\n Heal: {Values[i][j]:##0.##} HP/s";
                    if (Values[i][j] > scaleFactor)
                    {
                        scaleFactor = Values[i][j];
                    }
                }
            }

            GraphMax = scaleFactor * 1.025f;
            MidsContext.Archetype = archetype;
        }

        private void ValuesMaxTargets()
        {
            var scaleFactor = 1f;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < Powers[i].Length; j++)
                {
                    if (Powers[i][j] == null)
                    {
                        Tips[i][j] = "";
                        Values[i][j] = 0;

                        continue;
                    }

                    Values[i][j] = Powers[i][j].MaxTargets;
                    if (Math.Abs(Values[i][j]) < float.Epsilon)
                    {
                        continue;
                    }

                    Tips[i][j] = $"{DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID].DisplayName}:{Powers[i][j].DisplayName}";
                    if (Matching)
                    {
                        Tips[i][j] += $" [Level {Powers[i][j].Level}]";
                    }

                    if (Values[i][j] > 1)
                    {
                        Tips[i][j] += $"\r\n  {Values[i][j]} Targets Max.";
                    }
                    else
                    {
                        Tips[i][j] += $"\r\n  {Values[i][j]} Target Max.";
                    }

                    if (Values[i][j] > scaleFactor)
                    {
                        scaleFactor = Values[i][j];
                    }
                }
            }

            GraphMax = scaleFactor * 1.025f;
        }

        private void ValuesRange()
        {
            var scaleFactor = 1f;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < Powers[i].Length; j++)
                {
                    if (Powers[i][j] == null)
                    {
                        Tips[i][j] = "";
                        Values[i][j] = 0;

                        continue;
                    }

                    var str = "";
                    switch (Powers[i][j].EffectArea)
                    {
                        case Enums.eEffectArea.Character:
                            str = $"{Powers[i][j].Range}ft range.";
                            Values[i][j] = Powers[i][j].Range;
                            break;

                        case Enums.eEffectArea.Sphere:
                            Values[i][j] = Powers[i][j].Radius;
                            if (Powers[i][j].Range > 0)
                            {
                                str = $"{Powers[i][j].Range}ft range, ";
                                Values[i][j] = Powers[i][j].Range;
                            }

                            str += $"{Powers[i][j].Radius}ft radius.";
                            break;

                        case Enums.eEffectArea.Cone:
                            Values[i][j] = Powers[i][j].Range;
                            str = $"{Powers[i][j].Range}ft range, {Powers[i][j].Arc} degrees cone.";
                            break;

                        case Enums.eEffectArea.Location:
                            Values[i][j] = Powers[i][j].Range;
                            str = $"{Powers[i][j].Range}ft range, {Powers[i][j].Radius}ft radius.";
                            break;

                        case Enums.eEffectArea.Volume:
                            Values[i][j] = Powers[i][j].Radius;
                            if (Powers[i][j].Range > 0)
                            {
                                str = $"{Powers[i][j].Range}ft range, ";
                                Values[i][j] = Powers[i][j].Range;
                            }

                            str += $"{Powers[i][j].Radius}ft radius.";
                            break;
                    }

                    if (Math.Abs(Values[i][j]) < float.Epsilon)
                    {
                        continue;
                    }

                    Tips[i][j] = $"{DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID].DisplayName}:{Powers[i][j].DisplayName}";
                    if (Matching)
                    {
                        Tips[i][j] += $" [Level {Powers[i][j].Level}]";
                    }

                    Tips[i][j] += $"\r\n  {str}";
                    if (Values[i][j] > scaleFactor)
                    {
                        scaleFactor = Values[i][j];
                    }
                }
            }

            GraphMax = scaleFactor * 1.025f;
        }

        private void ValuesRecharge()
        {
            var scaleFactor = 1f;
            for (var i = 0; i < 2; i++)
            {
                for (var j = 0; j < Powers[i].Length; j++)
                {
                    if (Powers[i][j] == null)
                    {
                        Tips[i][j] = "";
                        Values[i][j] = 0;

                        continue;
                    }

                    Values[i][j] = Powers[i][j].RechargeTime;
                    if (Math.Abs(Values[i][j]) < float.Epsilon)
                    {
                        continue;
                    }

                    Tips[i][j] = $"{DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID].DisplayName}:{Powers[i][j].DisplayName}";
                    if (Matching)
                    {
                        Tips[i][j] += $" [Level {Powers[i][j].Level}]";
                    }

                    Tips[i][j] += $"\r\n {Values[i][j]:##0.##}s";
                    if (Values[i][j] > scaleFactor)
                    {
                        scaleFactor = Values[i][j];
                    }
                }

                ++i;
            }

            GraphMax = scaleFactor * 1.025f;
        }

        private void ValuesUniversal(Enums.eEffectType iEffectType, bool sum, bool Debuff)
        {
            var archetype = MidsContext.Archetype;
            var scaleFactor = 1f;
            var str = "";
            for (var i = 0; i < 1; i++)
            {
                for (var j = 0; j < Powers[i].Length; j++)
                {
                    if (Powers[i][j] == null)
                    {
                        Tips[i][j] = "";
                        Values[i][j] = 0;

                        continue;
                    }

                    MidsContext.Archetype = DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID];
                    var effectMagSum = Powers[i][j].GetEffectMagSum(iEffectType);
                    var index3 = 0;
                    if (effectMagSum.Present)
                    {
                        if (Debuff)
                        {
                            for (var index4 = 0; index4 < effectMagSum.Index.Length; index4++)
                            {
                                if (effectMagSum.Value[index4] > 0)
                                {
                                    effectMagSum.Value[index4] = 0;
                                }
                                else
                                {
                                    effectMagSum.Value[index4] *= -1;
                                }
                            }

                            effectMagSum.ReSum();
                            index3 = effectMagSum.Max;
                        }
                        else
                        {
                            for (var index4 = 0; index4 < effectMagSum.Index.Length; index4++)
                            {
                                if (effectMagSum.Value[index4] < 0)
                                {
                                    effectMagSum.Value[index4] = 0;
                                }
                            }

                            index3 = effectMagSum.Max;
                            effectMagSum.ReSum();
                        }
                    }

                    if (!sum)
                    {
                        if (effectMagSum.Present)
                        {
                            str = GetUniversalTipString(effectMagSum, ref Powers[i][j]);
                            Values[i][j] = effectMagSum.Value[index3];
                            if (Powers[i][j].Effects[effectMagSum.Index[index3]].DisplayPercentage)
                            {
                                Values[i][j] *= 100;
                            }
                        }
                    }
                    else
                    {
                        if (effectMagSum.Present && Powers[i][j].Effects[effectMagSum.Index[index3]].DisplayPercentage)
                        {
                            effectMagSum.Multiply();
                        }

                        Values[i][j] = effectMagSum.Sum;
                    }

                    if (Math.Abs(Values[i][j]) < float.Epsilon)
                    {
                        continue;
                    }

                    Tips[i][j] = $"{DatabaseAPI.Database.Classes[Powers[i][j].ForcedClassID].DisplayName}:{Powers[i][j].DisplayName}";
                    if (Matching)
                    {
                        Tips[i][j] += $" [Level {Powers[i][j].Level}]";
                    }

                    if (sum)
                    {
                        str = "";
                        foreach (var s in effectMagSum.Index)
                        {
                            if (str != "")
                            {
                                str += "\r\n";
                            }

                            str += $"  {Powers[i][j].Effects[s].BuildEffectString().Replace("\r\n", "\r\n  ")}";
                        }

                        Tips[i][j] += $"\r\n{str}";
                    }
                    else
                    {
                        Tips[i][j] += $"\r\n  {str}";
                    }

                    if (Values[i][j] > scaleFactor)
                    {
                        scaleFactor = Values[i][j];
                    }
                }
            }

            GraphMax = scaleFactor * 1.025f;
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