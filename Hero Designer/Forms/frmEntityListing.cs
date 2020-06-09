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
        Button btnAdd;
        Button btnCancel;
        Button btnClone;
        Button btnDelete;
        Button btnDown;
        Button btnEdit;
        Button btnOK;
        Button btnUp;
        ColumnHeader ColumnHeader1;
        ColumnHeader ColumnHeader2;
        ColumnHeader ColumnHeader3;
        ListView lvEntity;
        frmBusy bFrm;

        public frmEntityListing()
        {
            Load += frmEntityListing_Load;
            InitializeComponent();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmEntityListing));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon", CultureInfo.InvariantCulture);
            Name = nameof(frmEntityListing);
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            var iEntity = SummonedEntity.AddEntity();
            using frmEntityEdit frmEntityEdit = new frmEntityEdit(iEntity);
            frmEntityEdit.ShowDialog();
            if (frmEntityEdit.DialogResult != DialogResult.OK)
                return;
            IDatabase database = DatabaseAPI.Database;
            SummonedEntity[] summonedEntityArray = (SummonedEntity[])Utils.CopyArray(database.Entities, new SummonedEntity[DatabaseAPI.Database.Entities.Length + 1]);
            database.Entities = summonedEntityArray;
            DatabaseAPI.Database.Entities[DatabaseAPI.Database.Entities.Length - 1] = new SummonedEntity(frmEntityEdit.myEntity, DatabaseAPI.Database.Entities.Length - 1);
            ListAddItem(DatabaseAPI.Database.Entities.Length - 1);
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            BusyMsg("Re-Indexing...");
            DatabaseAPI.LoadMainDatabase();
            DatabaseAPI.MatchAllIDs();
            BusyHide();
            Hide();
        }

        void btnClone_Click(object sender, EventArgs e)
        {
            if (lvEntity.SelectedIndices.Count <= 0)
                return;
            using frmEntityEdit frmEntityEdit = new frmEntityEdit(new SummonedEntity(DatabaseAPI.Database.Entities[lvEntity.SelectedIndices[0]], DatabaseAPI.Database.Entities.Length));
            if (frmEntityEdit.ShowDialog() != DialogResult.OK)
                return;
            IDatabase database = DatabaseAPI.Database;
            SummonedEntity[] summonedEntityArray = (SummonedEntity[])Utils.CopyArray(database.Entities, new SummonedEntity[DatabaseAPI.Database.Entities.Length + 1]);
            database.Entities = summonedEntityArray;
            DatabaseAPI.Database.Entities[DatabaseAPI.Database.Entities.Length - 1] = new SummonedEntity(frmEntityEdit.myEntity);
            ListAddItem(DatabaseAPI.Database.Entities.Length - 1);
        }

        void btnDelete_Click(object sender, EventArgs e)
        {
            if (lvEntity.SelectedIndices.Count <= 0 || Interaction.MsgBox(("Really delete entity: " + DatabaseAPI.Database.Entities[lvEntity.SelectedIndices[0]].DisplayName + "?"), MsgBoxStyle.YesNo | MsgBoxStyle.Question, "Are you sure?") != MsgBoxResult.Yes)
                return;
            SummonedEntity[] summonedEntityArray = new SummonedEntity[DatabaseAPI.Database.Entities.Length - 1 + 1];
            int selectedIndex = lvEntity.SelectedIndices[0];
            int index1 = 0;
            int num1 = DatabaseAPI.Database.Entities.Length - 1;
            for (int index2 = 0; index2 <= num1; ++index2)
            {
                if (index2 == selectedIndex)
                    continue;
                summonedEntityArray[index1] = new SummonedEntity(DatabaseAPI.Database.Entities[index2]);
                ++index1;
            }
            DatabaseAPI.Database.Entities = new SummonedEntity[DatabaseAPI.Database.Entities.Length - 2 + 1];
            int num2 = DatabaseAPI.Database.Entities.Length - 1;
            for (int index2 = 0; index2 <= num2; ++index2)
                DatabaseAPI.Database.Entities[index2] = new SummonedEntity(summonedEntityArray[index2]);
            DisplayList();
            if (lvEntity.Items.Count <= 0)
                return;
            if (lvEntity.Items.Count > selectedIndex)
                lvEntity.Items[selectedIndex].Selected = true;
            else if (lvEntity.Items.Count == selectedIndex)
                lvEntity.Items[selectedIndex - 1].Selected = true;
        }

        void btnDown_Click(object sender, EventArgs e)
        {
            if (lvEntity.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lvEntity.SelectedIndices[0];
            if (selectedIndex >= lvEntity.Items.Count - 1)
                return;
            SummonedEntity[] summonedEntityArray = {
                new SummonedEntity(DatabaseAPI.Database.Entities[selectedIndex]),
                new SummonedEntity(DatabaseAPI.Database.Entities[selectedIndex + 1])
            };
            DatabaseAPI.Database.Entities[selectedIndex + 1] = new SummonedEntity(summonedEntityArray[0]);
            DatabaseAPI.Database.Entities[selectedIndex] = new SummonedEntity(summonedEntityArray[1]);
            DisplayList();
            lvEntity.Items[selectedIndex + 1].Selected = true;
            lvEntity.Items[selectedIndex + 1].EnsureVisible();
        }

        void btnEdit_Click(object sender, EventArgs e)
        {
            if (lvEntity.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lvEntity.SelectedIndices[0];
            using frmEntityEdit frmEntityEdit = new frmEntityEdit(DatabaseAPI.Database.Entities[lvEntity.SelectedIndices[0]]);
            if (frmEntityEdit.ShowDialog() != DialogResult.OK)
                return;
            DatabaseAPI.Database.Entities[selectedIndex] = new SummonedEntity(frmEntityEdit.myEntity);
            ListUpdateItem(selectedIndex);
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            DatabaseAPI.MatchSummonIDs();
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveMainDatabase(serializer);
            Hide();
        }

        void btnUp_Click(object sender, EventArgs e)
        {
            if (lvEntity.SelectedIndices.Count <= 0)
                return;
            int selectedIndex = lvEntity.SelectedIndices[0];
            if (selectedIndex < 1)
                return;
            SummonedEntity[] summonedEntityArray = {
                new SummonedEntity(DatabaseAPI.Database.Entities[selectedIndex]),
                new SummonedEntity(DatabaseAPI.Database.Entities[selectedIndex - 1])
            };
            DatabaseAPI.Database.Entities[selectedIndex - 1] = new SummonedEntity(summonedEntityArray[0]);
            DatabaseAPI.Database.Entities[selectedIndex] = new SummonedEntity(summonedEntityArray[1]);
            DisplayList();
            lvEntity.Items[selectedIndex - 1].Selected = true;
            lvEntity.Items[selectedIndex - 1].EnsureVisible();
        }

        void BusyHide()
        {
            if (bFrm == null)
                return;
            bFrm.Close();
            bFrm = null;
        }

        void BusyMsg(string sMessage)
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
            int num = DatabaseAPI.Database.Entities.Length - 1;
            for (int Index = 0; Index <= num; ++Index)
                ListAddItem(Index);
            if (lvEntity.Items.Count > 0)
            {
                lvEntity.Items[0].Selected = true;
                lvEntity.Items[0].EnsureVisible();
            }
            lvEntity.EndUpdate();
        }

        void frmEntityListing_Load(object sender, EventArgs e)
        {
            DisplayList();
        }

        private void ListAddItem(int Index)
        {
            lvEntity.Items.Add(new ListViewItem(new[]
            {
                DatabaseAPI.Database.Entities[Index].UID,
                DatabaseAPI.Database.Entities[Index].DisplayName,
                Enum.GetName(DatabaseAPI.Database.Entities[Index].EntityType.GetType(),  DatabaseAPI.Database.Entities[Index].EntityType)
            }, Index));
            lvEntity.Items[lvEntity.Items.Count - 1].Selected = true;
            lvEntity.Items[lvEntity.Items.Count - 1].EnsureVisible();
        }

        private void ListUpdateItem(int Index)
        {
            string[] strArray = {
                DatabaseAPI.Database.Entities[Index].UID,
                DatabaseAPI.Database.Entities[Index].DisplayName,
                Enum.GetName(DatabaseAPI.Database.Entities[Index].EntityType.GetType(),  DatabaseAPI.Database.Entities[Index].EntityType)
            };
            int num = strArray.Length - 1;
            for (int index = 0; index <= num; ++index)
                lvEntity.Items[Index].SubItems[index].Text = strArray[index];
            lvEntity.Items[Index].EnsureVisible();
            lvEntity.Refresh();
        }

        void lvEntity_DoubleClick(object sender, EventArgs e)

        {
            btnEdit_Click(RuntimeHelpers.GetObjectValue(sender), e);
        }
    }
}