using Mids_Reborn.Core;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Forms.Controls
{
    public partial class GenericSelector : Form
    {
        public dynamic? SelectedItem { get; private set; }
        public bool NoItem { get; private set; }

        protected ItemType Type;
        protected bool MultiSelection; // Not implemented
        
        private bool _playableArchetypesOnly;

        public enum ItemType
        {
            Power,
            Archetype,
            PowerGroup,
            PowerGroupPrefix,
            Modifier,
            AttackVector
        }

        public GenericSelector(ItemType type = ItemType.Power, bool multiSelection = false)
        {
            Type = type;
            MultiSelection = multiSelection;

            InitializeComponent();
        }

        protected async void OnLoad(object? sender, EventArgs e)
        {
            formPages1.SelectedIndex = Type switch
            {
                ItemType.Archetype => 1,
                ItemType.PowerGroup or ItemType.PowerGroupPrefix => 2,
                ItemType.AttackVector => 3,
                ItemType.Modifier => 4,
                _ => 0
            };

            _playableArchetypesOnly = true;

            switch (Type)
            {
                case ItemType.Archetype:
                    checkPlayableAtOnly.Checked = _playableArchetypesOnly;
                    FillArchetypes();

                    break;

                case ItemType.Power:
                    await FillPowerGroups();

                    break;

                case ItemType.Modifier:
                    FillModifiers();

                    break;

                case ItemType.PowerGroup or ItemType.PowerGroupPrefix:
                    FillPowerGroups2(Type == ItemType.PowerGroupPrefix);

                    break;

                case ItemType.AttackVector:
                    FillAttackVectors();

                    break;
            }
        }

        private async Task FillPowerGroups()
        {
            var groups = (from dbGroup in DatabaseAPI.Database.PowersetGroups where dbGroup.Key != "Boosts" && dbGroup.Key != "Incarnate" && dbGroup.Key != "Set_Bonus" select new KeyValue<string, PowersetGroup>(dbGroup.Key, dbGroup.Value)).ToList();
            cbGroup.BeginUpdate();
            cbGroup.DataSource = new BindingSource(groups, null);
            cbGroup.DisplayMember = "Key";
            cbGroup.ValueMember = "Value";
            cbGroup.SelectedIndex = 0;
            cbGroup.EndUpdate();
            CbGroupOnSelectedIndexChanged(this, EventArgs.Empty);
            await Task.CompletedTask;
        }

        private void FillArchetypes() // make async ?
        {
            var archetypes = DatabaseAPI.Database.Classes
                .Where(e => e != null && _playableArchetypesOnly | e.Playable)
                .Select(e => new KeyValue<string, Archetype>(e.DisplayName, e))
                .ToList();

            cbArchetypes.BeginUpdate();
            cbArchetypes.DataSource = new BindingSource(archetypes, null);
            cbArchetypes.DisplayMember = "Key";
            cbArchetypes.ValueMember = "Value";
            cbArchetypes.SelectedIndex = 0;
            cbArchetypes.EndUpdate();
        }

        private void FillPowerGroups2(bool prefixMode)
        {
            cbP2Powerset.Visible = prefixMode;
            label9.Visible = prefixMode;

            var groups = (from dbGroup in DatabaseAPI.Database.PowersetGroups where dbGroup.Key != "Boosts" && dbGroup.Key != "Set_Bonus" select new KeyValue<string, PowersetGroup>(dbGroup.Key, dbGroup.Value)).ToList();
            cbP2Group.BeginUpdate();
            cbP2Group.DataSource = new BindingSource(groups, null);
            cbP2Group.DisplayMember = "Key";
            cbP2Group.ValueMember = "Value";
            cbP2Group.SelectedIndex = 0;
            cbP2Group.EndUpdate();
            cbP2Powerset_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void FillModifiers()
        {
            var modifiers = DatabaseAPI.Database.AttribMods.Modifier
                .Select((e, i) => new KeyValue<int, string>(i, e.ID))
                .ToList();

            cbModifier.BeginUpdate();
            cbModifier.DataSource = new BindingSource(modifiers, null);
            cbModifier.DisplayMember = "Value";
            cbModifier.ValueMember = "Key";
            cbModifier.SelectedIndex = 0;
            cbModifier.EndUpdate();
        }

        private void FillAttackVectors()
        {
            var vectors = Enum.GetNames(typeof(Enums.eDamage))
                .Select((e, i) => new KeyValue<int, string>(i, e))
                .ToList();

            cbVectors.BeginUpdate();
            cbVectors.DataSource = new BindingSource(vectors, null);
            cbVectors.DisplayMember = "Value";
            cbVectors.ValueMember = "Key";
            cbVectors.SelectedIndex = 0;
            cbVectors.EndUpdate();
        }

        private void CbGroupOnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbGroup.SelectedIndex < 0)
            {
                return;
            }

            if (cbGroup.SelectedValue is not PowersetGroup selectedGroup)
            {
                return;
            }

            var powerSets = selectedGroup.Powersets.Values.ToList();
            cbPowerset.BeginUpdate();
            cbPowerset.DataSource = new BindingSource(powerSets, null);
            cbPowerset.DisplayMember = "DisplayName";
            cbPowerset.ValueMember = null;
            cbPowerset.SelectedIndex = 0;
            cbPowerset.EndUpdate();
        }

        private void CbPowersetOnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPowerset.SelectedIndex < 0)
            {
                return;
            }

            if (cbPowerset.SelectedValue is not IPowerset selectedPowerSet)
            {
                return;
            }

            var powers = selectedPowerSet.Powers.ToList();
            cbPower.BeginUpdate();
            cbPower.DataSource = new BindingSource(powers, null);
            cbPower.DisplayMember = "DisplayName";
            cbPower.ValueMember = null;
            cbPower.SelectedIndex = 0;
            cbPower.EndUpdate();
        }

        private void CbPowerOnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbPower.SelectedIndex < 0)
            {
                return;
            }

            if (cbPower.SelectedValue is not IPower selectedPower)
            {
                return;
            }

            SelectedItem = selectedPower;
        }

        private void cbModifier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbModifier.SelectedIndex < 0)
            {
                return;
            }

            if (cbModifier.SelectedValue is not int selectedModifier) // ???
            {
                return;
            }

            SelectedItem = DatabaseAPI.Database.AttribMods.Modifier[selectedModifier];
        }

        private void cbVectors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbVectors.SelectedIndex < 0)
            {
                return;
            }

            if (cbVectors.SelectedValue is not int selectedVector) // ???
            {
                return;
            }

            SelectedItem = (Enums.eDamage)selectedVector;
        }

        private void cbP2Group_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbP2Group.SelectedIndex < 0)
            {
                return;
            }

            if (cbP2Group.SelectedValue is not PowersetGroup selectedGroup)
            {
                return;
            }

            if (Type == ItemType.PowerGroup)
            {
                SelectedItem = selectedGroup;

                return;
            }

            var powerSets = selectedGroup.Powersets.Values.ToList();
            cbPowerset.BeginUpdate();
            cbPowerset.DataSource = new BindingSource(powerSets, null);
            cbPowerset.DisplayMember = "DisplayName";
            cbPowerset.ValueMember = null;
            cbPowerset.SelectedIndex = 0;
            cbPowerset.EndUpdate();
        }

        private void cbP2Powerset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Type == ItemType.PowerGroup)
            {
                return;
            }

            if (cbP2Powerset.SelectedIndex < 0)
            {
                return;
            }

            if (cbP2Powerset.SelectedValue is not Powerset selectedPowerset)
            {
                return;
            }

            SelectedItem = selectedPowerset;
        }

        private void checkPlayableAtOnly_CheckedChanged(object sender, EventArgs e)
        {
            _playableArchetypesOnly = checkPlayableAtOnly.Checked;
            FillArchetypes();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            NoItem = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            NoItem = true;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnChooseNone_Click(object sender, EventArgs e)
        {
            NoItem = true;
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    public class ArchetypeSelector : GenericSelector
    {
        public ArchetypeSelector(bool multiSelection = false)
        {
            Type = ItemType.Archetype;
            MultiSelection = multiSelection;

            InitializeComponent();
        }
    }

    public class PowerGroupSelector : GenericSelector
    {
        public PowerGroupSelector(bool prefixMode, bool multiSelection = false)
        {
            Type = prefixMode ? ItemType.PowerGroupPrefix : ItemType.PowerGroup;
            MultiSelection = multiSelection;

            InitializeComponent();
        }
    }

    public class AttackVectorSelector : GenericSelector
    {
        public AttackVectorSelector()
        {
            Type = ItemType.AttackVector;
            MultiSelection = false;

            InitializeComponent();
        }
    }

    public class ModifierSelector : GenericSelector
    {
        public ModifierSelector()
        {
            Type = ItemType.Modifier;
            MultiSelection = false;

            InitializeComponent();
        }
    }

    // Get rid of PowerSelector, then rename
    // When done with GenericSelector
    public class PowerSelector2 : GenericSelector
    {
        public PowerSelector2(bool multiSelection = false)
        {
            Type = ItemType.Power;
            MultiSelection = multiSelection;

            InitializeComponent();
        }
    }
}
