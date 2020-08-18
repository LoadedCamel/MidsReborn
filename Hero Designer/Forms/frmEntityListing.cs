using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Hero_Designer.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmEntityListing : Form
    {
        private frmBusy bFrm;
        private Button btnAdd;
        private Button btnCancel;
        private Button btnClone;
        private Button btnDelete;
        private Button btnDown;
        private Button btnEdit;
        private Button btnOK;
        private Button btnUp;
        private ColumnHeader ColumnHeader1;
        private ColumnHeader ColumnHeader2;
        private ColumnHeader ColumnHeader3;
        private ListView lvEntity;

        public frmEntityListing()
        {
            Load += frmEntityListing_Load;
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmEntityListing));
            Icon = (Icon) componentResourceManager.GetObject("$this.Icon", CultureInfo.InvariantCulture);
            Name = nameof(frmEntityListing);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var iEntity = SummonedEntity.AddEntity();
            using var frmEntityEdit = new frmEntityEdit(iEntity);
            frmEntityEdit.ShowDialog();
            if (frmEntityEdit.DialogResult != DialogResult.OK)
                return;
            var database = DatabaseAPI.Database;
            var summonedEntityArray = (SummonedEntity[]) Utils.CopyArray(database.Entities,
                new SummonedEntity[DatabaseAPI.Database.Entities.Length + 1]);
            database.Entities = summonedEntityArray;
            DatabaseAPI.Database.Entities[DatabaseAPI.Database.Entities.Length - 1] =
                new SummonedEntity(frmEntityEdit.myEntity, DatabaseAPI.Database.Entities.Length - 1);
            ListAddItem(DatabaseAPI.Database.Entities.Length - 1);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            BusyMsg("Re-Indexing...");
            DatabaseAPI.LoadMainDatabase();
            DatabaseAPI.MatchAllIDs();
            BusyHide();
            Hide();
        }

        private void btnClone_Click(object sender, EventArgs e)
        {
            if (lvEntity.SelectedIndices.Count <= 0)
                return;
            using var frmEntityEdit = new frmEntityEdit(new SummonedEntity(
                DatabaseAPI.Database.Entities[lvEntity.SelectedIndices[0]], DatabaseAPI.Database.Entities.Length));
            if (frmEntityEdit.ShowDialog() != DialogResult.OK)
                return;
            var database = DatabaseAPI.Database;
            var summonedEntityArray = (SummonedEntity[]) Utils.CopyArray(database.Entities,
                new SummonedEntity[DatabaseAPI.Database.Entities.Length + 1]);
            database.Entities = summonedEntityArray;
            DatabaseAPI.Database.Entities[DatabaseAPI.Database.Entities.Length - 1] =
                new SummonedEntity(frmEntityEdit.myEntity);
            ListAddItem(DatabaseAPI.Database.Entities.Length - 1);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvEntity.SelectedIndices.Count <= 0 ||
                Interaction.MsgBox(
                    "Really delete entity: " + DatabaseAPI.Database.Entities[lvEntity.SelectedIndices[0]].DisplayName +
                    "?", MsgBoxStyle.YesNo | MsgBoxStyle.Question, "Are you sure?") != MsgBoxResult.Yes)
                return;
            var summonedEntityArray = new SummonedEntity[DatabaseAPI.Database.Entities.Length - 1 + 1];
            var selectedIndex = lvEntity.SelectedIndices[0];
            var index1 = 0;
            var num1 = DatabaseAPI.Database.Entities.Length - 1;
            for (var index2 = 0; index2 <= num1; ++index2)
            {
                if (index2 == selectedIndex)
                    continue;
                summonedEntityArray[index1] = new SummonedEntity(DatabaseAPI.Database.Entities[index2]);
                ++index1;
            }

            DatabaseAPI.Database.Entities = new SummonedEntity[DatabaseAPI.Database.Entities.Length - 2 + 1];
            var num2 = DatabaseAPI.Database.Entities.Length - 1;
            for (var index2 = 0; index2 <= num2; ++index2)
                DatabaseAPI.Database.Entities[index2] = new SummonedEntity(summonedEntityArray[index2]);
            DisplayList();
            if (lvEntity.Items.Count <= 0)
                return;
            if (lvEntity.Items.Count > selectedIndex)
                lvEntity.Items[selectedIndex].Selected = true;
            else if (lvEntity.Items.Count == selectedIndex)
                lvEntity.Items[selectedIndex - 1].Selected = true;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (lvEntity.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = lvEntity.SelectedIndices[0];
            if (selectedIndex >= lvEntity.Items.Count - 1)
                return;
            SummonedEntity[] summonedEntityArray =
            {
                new SummonedEntity(DatabaseAPI.Database.Entities[selectedIndex]),
                new SummonedEntity(DatabaseAPI.Database.Entities[selectedIndex + 1])
            };
            DatabaseAPI.Database.Entities[selectedIndex + 1] = new SummonedEntity(summonedEntityArray[0]);
            DatabaseAPI.Database.Entities[selectedIndex] = new SummonedEntity(summonedEntityArray[1]);
            DisplayList();
            lvEntity.Items[selectedIndex + 1].Selected = true;
            lvEntity.Items[selectedIndex + 1].EnsureVisible();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (lvEntity.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = lvEntity.SelectedIndices[0];
            using var frmEntityEdit = new frmEntityEdit(DatabaseAPI.Database.Entities[lvEntity.SelectedIndices[0]]);
            if (frmEntityEdit.ShowDialog() != DialogResult.OK)
                return;
            DatabaseAPI.Database.Entities[selectedIndex] = new SummonedEntity(frmEntityEdit.myEntity);
            ListUpdateItem(selectedIndex);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DatabaseAPI.MatchSummonIDs();
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveMainDatabase(serializer);
            Hide();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (lvEntity.SelectedIndices.Count <= 0)
                return;
            var selectedIndex = lvEntity.SelectedIndices[0];
            if (selectedIndex < 1)
                return;
            SummonedEntity[] summonedEntityArray =
            {
                new SummonedEntity(DatabaseAPI.Database.Entities[selectedIndex]),
                new SummonedEntity(DatabaseAPI.Database.Entities[selectedIndex - 1])
            };
            DatabaseAPI.Database.Entities[selectedIndex - 1] = new SummonedEntity(summonedEntityArray[0]);
            DatabaseAPI.Database.Entities[selectedIndex] = new SummonedEntity(summonedEntityArray[1]);
            DisplayList();
            lvEntity.Items[selectedIndex - 1].Selected = true;
            lvEntity.Items[selectedIndex - 1].EnsureVisible();
        }

        private void BusyHide()
        {
            if (bFrm == null)
                return;
            bFrm.Close();
            bFrm = null;
        }

        private void BusyMsg(string sMessage)
        {
            if (bFrm == null)
            {
                bFrm = new frmBusy();
                bFrm.Show(this);
            }

            bFrm.SetMessage(sMessage);
        }

        private void DisplayList()
        {
            lvEntity.BeginUpdate();
            lvEntity.Items.Clear();
            var num = DatabaseAPI.Database.Entities.Length - 1;
            for (var Index = 0; Index <= num; ++Index)
                ListAddItem(Index);
            if (lvEntity.Items.Count > 0)
            {
                lvEntity.Items[0].Selected = true;
                lvEntity.Items[0].EnsureVisible();
            }

            lvEntity.EndUpdate();
        }

        private void frmEntityListing_Load(object sender, EventArgs e)
        {
            DisplayList();
        }

        private void ListAddItem(int Index)
        {
            lvEntity.Items.Add(new ListViewItem(new[]
            {
                DatabaseAPI.Database.Entities[Index].UID,
                DatabaseAPI.Database.Entities[Index].DisplayName,
                Enum.GetName(DatabaseAPI.Database.Entities[Index].EntityType.GetType(),
                    DatabaseAPI.Database.Entities[Index].EntityType)
            }, Index));
            lvEntity.Items[lvEntity.Items.Count - 1].Selected = true;
            lvEntity.Items[lvEntity.Items.Count - 1].EnsureVisible();
        }

        private void ListUpdateItem(int Index)
        {
            string[] strArray =
            {
                DatabaseAPI.Database.Entities[Index].UID,
                DatabaseAPI.Database.Entities[Index].DisplayName,
                Enum.GetName(DatabaseAPI.Database.Entities[Index].EntityType.GetType(),
                    DatabaseAPI.Database.Entities[Index].EntityType)
            };
            var num = strArray.Length - 1;
            for (var index = 0; index <= num; ++index)
                lvEntity.Items[Index].SubItems[index].Text = strArray[index];
            lvEntity.Items[Index].EnsureVisible();
            lvEntity.Refresh();
        }

        private void lvEntity_DoubleClick(object sender, EventArgs e)

        {
            btnEdit_Click(RuntimeHelpers.GetObjectValue(sender), e);
        }
    }
}