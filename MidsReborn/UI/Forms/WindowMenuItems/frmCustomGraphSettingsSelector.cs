using System;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;

namespace Mids_Reborn.UI.Forms.WindowMenuItems
{
    public partial class frmCustomGraphSettingsSelector : Form
    {
        private enum StatType
        {
            Damage,
            Mez,
            Effect
        }

        private CustomGraphStat.eCustomGraphStat Stat;
        public ConfigData.CustomGraphSettings Settings { get; }

        public frmCustomGraphSettingsSelector(CustomGraphStat.eCustomGraphStat stat, ConfigData.CustomGraphSettings initialSettings)
        {
            InitializeComponent();
            Stat = stat;
            Settings = initialSettings;
        }

        private void frmCustomGraphSettingsSelector_Load(object sender, EventArgs e)
        {
            Text = $"Options for {Stat} graph";
            switch (GetStatType())
            {
                case StatType.Damage:
                    switch (Settings.DamageMode)
                    {
                        case CustomGraphStat.eCustomGraphMode.Single:
                            radioButton1.Checked = true;
                            break;

                        case CustomGraphStat.eCustomGraphMode.Min:
                            radioButton2.Checked = true;
                            break;

                        case CustomGraphStat.eCustomGraphMode.Max:
                            radioButton3.Checked = true;
                            break;

                        case CustomGraphStat.eCustomGraphMode.Average:
                            radioButton4.Checked = true;
                            break;
                    }

                    break;

                case StatType.Effect:
                    switch (Settings.EffectMode)
                    {
                        case CustomGraphStat.eCustomGraphMode.Single:
                            radioButton1.Checked = true;
                            break;

                        case CustomGraphStat.eCustomGraphMode.Min:
                            radioButton2.Checked = true;
                            break;

                        case CustomGraphStat.eCustomGraphMode.Max:
                            radioButton3.Checked = true;
                            break;

                        case CustomGraphStat.eCustomGraphMode.Average:
                            radioButton4.Checked = true;
                            break;
                    }

                    break;

                case StatType.Mez:
                    switch (Settings.MezMode)
                    {
                        case CustomGraphStat.eCustomGraphMode.Single:
                            radioButton1.Checked = true;
                            break;

                        case CustomGraphStat.eCustomGraphMode.Min:
                            radioButton2.Checked = true;
                            break;

                        case CustomGraphStat.eCustomGraphMode.Max:
                            radioButton3.Checked = true;
                            break;

                        case CustomGraphStat.eCustomGraphMode.Average:
                            radioButton4.Checked = true;
                            break;
                    }

                    break;
            }

            var vectors = AvailableVectors();
            cbVector.SuspendLayout();
            cbVector.Items.Clear();
            foreach (var v in vectors)
            {
                cbVector.Items.Add(v);
            }

            cbVector.SelectedIndex = 0;

            cbVector.ResumeLayout(true);
        }

        private StatType GetStatType()
        {
            return Stat switch
            {
                CustomGraphStat.eCustomGraphStat.DebuffResistance => StatType.Effect,
                CustomGraphStat.eCustomGraphStat.Defense or CustomGraphStat.eCustomGraphStat.Resistance or CustomGraphStat.eCustomGraphStat.Elusivity => StatType.Damage,
                _ => StatType.Mez
            };
        }

        private string[] AvailableVectors()
        {
            return GetStatType() switch
            {
                StatType.Damage when Stat == CustomGraphStat.eCustomGraphStat.Elusivity => Enum.GetNames<Enums.eDamage>()[..8] // None + types
                    .Concat(Enum.GetNames<Enums.eDamage>()[10..12]) // Positions
                    .Select(e => e == "None" ? "Untyped" : e)
                    .ToArray(),
                StatType.Damage when Stat == CustomGraphStat.eCustomGraphStat.Defense => Enum.GetNames<Enums.eDamage>()[1..8] // Types
                    .Concat(Enum.GetNames<Enums.eDamage>()[10..12]) // Positions
                    .ToArray(),
                StatType.Damage => Enum.GetNames<Enums.eDamage>()[1..8].ToArray(), // Resistance
                StatType.Effect => new[]
                {
                    Enums.eEffectType.Defense,
                    Enums.eEffectType.Endurance,
                    Enums.eEffectType.Recovery,
                    Enums.eEffectType.PerceptionRadius,
                    Enums.eEffectType.ToHit,
                    Enums.eEffectType.RechargeTime,
                    Enums.eEffectType.SpeedRunning,
                    Enums.eEffectType.Regeneration
                }.Cast<string>().ToArray(),
                _ => new[]
                {
                    Enums.eMez.Held,
                    Enums.eMez.Stunned,
                    Enums.eMez.Sleep,
                    Enums.eMez.Immobilized,
                    Enums.eMez.Knockback,
                    Enums.eMez.Repel,
                    Enums.eMez.Confused,
                    Enums.eMez.Terrorized,
                    Enums.eMez.Taunt,
                    Enums.eMez.Placate,
                    Enums.eMez.Teleport
                }.Cast<string>().ToArray()
            };
        }

        private void SetSelectedVector(int index)
        {
            switch (GetStatType())
            {
                case StatType.Damage when Stat == CustomGraphStat.eCustomGraphStat.Elusivity:
                    Settings.DamageType = index switch
                    {
                        0 => Enums.eDamage.None,
                        1 => Enums.eDamage.Smashing,
                        2 => Enums.eDamage.Lethal,
                        3 => Enums.eDamage.Fire,
                        4 => Enums.eDamage.Cold,
                        5 => Enums.eDamage.Energy,
                        6 => Enums.eDamage.Negative,
                        7 => Enums.eDamage.Toxic,
                        8 => Enums.eDamage.Psionic,
                        9 => Enums.eDamage.Melee,
                        10 => Enums.eDamage.Ranged,
                        11 => Enums.eDamage.AoE,
                        _ => null
                    };

                    break;

                case StatType.Damage when Stat == CustomGraphStat.eCustomGraphStat.Defense:
                    Settings.DamageType = index switch
                    {
                        0 => Enums.eDamage.Smashing,
                        1 => Enums.eDamage.Lethal,
                        2 => Enums.eDamage.Fire,
                        3 => Enums.eDamage.Cold,
                        4 => Enums.eDamage.Energy,
                        5 => Enums.eDamage.Negative,
                        6 => Enums.eDamage.Toxic,
                        7 => Enums.eDamage.Psionic,
                        8 => Enums.eDamage.Melee,
                        9 => Enums.eDamage.Ranged,
                        10 => Enums.eDamage.AoE,
                        _ => null
                    };

                    break;

                case StatType.Damage:
                    if (index is < 0 or > 7)
                    {
                        Settings.DamageType = null;
                        return;
                    }

                    Settings.DamageType = (Enums.eDamage)(index + 1);
                    
                    break;

                case StatType.Effect:
                    Settings.EffectTypeAux = index switch
                    {
                        0 => Enums.eEffectType.Defense,
                        1 => Enums.eEffectType.Endurance,
                        2 => Enums.eEffectType.Recovery,
                        3 => Enums.eEffectType.PerceptionRadius,
                        4 => Enums.eEffectType.ToHit,
                        5 => Enums.eEffectType.RechargeTime,
                        6 => Enums.eEffectType.SpeedRunning,
                        7 => Enums.eEffectType.Regeneration,
                        _ => null
                    };
                    
                    break;

                default:
                    Settings.MezType = index switch
                    {
                        0 => Enums.eMez.Held,
                        1 => Enums.eMez.Stunned,
                        2 => Enums.eMez.Sleep,
                        3 => Enums.eMez.Immobilized,
                        4 => Enums.eMez.Knockback,
                        5 => Enums.eMez.Repel,
                        6 => Enums.eMez.Confused,
                        7 => Enums.eMez.Terrorized,
                        8 => Enums.eMez.Taunt,
                        9 => Enums.eMez.Placate,
                        10 => Enums.eMez.Teleport,
                        _ => null
                    };
                    
                    break;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            label2.Enabled = radioButton1.Checked;
            cbVector.Enabled = radioButton1.Checked;

            if (!radioButton1.Checked)
            {
                return;
            }

            radioButton2.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;

            switch (GetStatType())
            {
                case StatType.Damage:
                    Settings.DamageMode = CustomGraphStat.eCustomGraphMode.Single;
                    break;

                case StatType.Effect:
                    Settings.EffectMode = CustomGraphStat.eCustomGraphMode.Single;
                    break;

                case StatType.Mez:
                    Settings.MezMode = CustomGraphStat.eCustomGraphMode.Single;
                    break;
            }

            SetSelectedVector(cbVector.SelectedIndex);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton2.Checked)
            {
                return;
            }

            radioButton1.Checked = false;
            radioButton3.Checked = false;
            radioButton4.Checked = false;

            label2.Enabled = radioButton1.Checked;
            cbVector.Enabled = radioButton1.Checked;

            switch (GetStatType())
            {
                case StatType.Damage:
                    Settings.DamageMode = CustomGraphStat.eCustomGraphMode.Min;
                    break;

                case StatType.Effect:
                    Settings.EffectMode = CustomGraphStat.eCustomGraphMode.Min;
                    break;

                case StatType.Mez:
                    Settings.MezMode = CustomGraphStat.eCustomGraphMode.Min;
                    break;
            }

            SetSelectedVector(-1);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton3.Checked)
            {
                return;
            }

            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton4.Checked = false;

            label2.Enabled = radioButton1.Checked;
            cbVector.Enabled = radioButton1.Checked;

            switch (GetStatType())
            {
                case StatType.Damage:
                    Settings.DamageMode = CustomGraphStat.eCustomGraphMode.Max;
                    break;

                case StatType.Effect:
                    Settings.EffectMode = CustomGraphStat.eCustomGraphMode.Max;
                    break;

                case StatType.Mez:
                    Settings.MezMode = CustomGraphStat.eCustomGraphMode.Max;
                    break;
            }

            SetSelectedVector(-1);
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (!radioButton4.Checked)
            {
                return;
            }

            radioButton1.Checked = false;
            radioButton2.Checked = false;
            radioButton3.Checked = false;

            label2.Enabled = radioButton1.Checked;
            cbVector.Enabled = radioButton1.Checked;

            switch (GetStatType())
            {
                case StatType.Damage:
                    Settings.DamageMode = CustomGraphStat.eCustomGraphMode.Average;
                    break;

                case StatType.Effect:
                    Settings.EffectMode = CustomGraphStat.eCustomGraphMode.Average;
                    break;

                case StatType.Mez:
                    Settings.MezMode = CustomGraphStat.eCustomGraphMode.Average;
                    break;
            }

            SetSelectedVector(-1);
        }

        private void cbVector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbVector.SelectedIndex < 0)
            {
                return;
            }

            SetSelectedVector(cbVector.SelectedIndex);
        }
    }
}
