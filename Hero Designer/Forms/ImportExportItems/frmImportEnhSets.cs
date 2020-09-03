using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Hero_Designer.My;
using Import;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer.Forms.ImportExportItems
{
    public partial class frmImportEnhSets : Form
    {
        private readonly List<ListViewItem> _currentItems;
        private frmBusy _bFrm;

        private string _fullFileName;

        private List<EnhSetData> _importBuffer;

        private bool _showUnchanged;


        public frmImportEnhSets()
        {
            Load += frmImportEnhSets_Load;
            _fullFileName = "";
            _showUnchanged = true;
            InitializeComponent();
            Name = nameof(frmImportEnhSets);
            var componentResourceManager = new ComponentResourceManager(typeof(frmImportEnhSets));
            Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
            _importBuffer = new List<EnhSetData>();
            _currentItems = new List<ListViewItem>();
        }

        private void btnCheckAll_Click(object sender, EventArgs e)

        {
            lstImport.BeginUpdate();
            var num = lstImport.Items.Count - 1;
            for (var index = 0; index <= num; ++index)
                lstImport.Items[index].Checked = true;
            lstImport.EndUpdate();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            dlgBrowse.FileName = _fullFileName;
            if (dlgBrowse.ShowDialog(this) == DialogResult.OK)
            {
                _fullFileName = dlgBrowse.FileName;
                Enabled = false;
                if (ParseClasses(_fullFileName))
                    FillListView();
                Enabled = true;
            }

            BusyHide();
            DisplayInfo();
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            ProcessImport();
        }

        private void btnUncheckAll_Click(object sender, EventArgs e)
        {
            lstImport.BeginUpdate();
            var num = lstImport.Items.Count - 1;
            for (var index = 0; index <= num; ++index)
                lstImport.Items[index].Checked = false;
            lstImport.EndUpdate();
        }

        private void BusyHide()
        {
            if (_bFrm == null)
                return;
            _bFrm.Close();
            _bFrm = null;
        }

        private void BusyMsg(string sMessage)
        {
            if (_bFrm == null)
            {
                _bFrm = new frmBusy();
                _bFrm.Show(this);
            }

            _bFrm.SetMessage(sMessage);
        }

        private void DisplayInfo()
        {
            lblFile.Text = FileIO.StripPath(_fullFileName);
        }

        private void FillListView()
        {
            var items = new string[6];
            lstImport.BeginUpdate();
            lstImport.Items.Clear();
            var num1 = 0;
            var num2 = 0;
            var num3 = 0;
            var num4 = _importBuffer.Count - 1;
            for (var index = 0; index <= num4; ++index)
            {
                ++num1;
                if (num1 >= 100)
                {
                    BusyMsg(Strings.Format(index, "###,##0") + " records checked.");
                    Application.DoEvents();
                    num1 = 0;
                }

                if (!_importBuffer[index].IsValid)
                    continue;
                items[0] = _importBuffer[index].Data.DisplayName;
                items[1] = Enum.GetName(_importBuffer[index].Data.SetType.GetType(), _importBuffer[index].Data.SetType);
                var flag = false;
                if (_importBuffer[index].IsNew)
                {
                    items[2] = "Yes";
                    ++num2;
                }
                else
                {
                    items[2] = "No";
                    flag = _importBuffer[index].CheckDifference(out items[4]);
                }

                if (flag)
                {
                    items[3] = "Yes";
                    ++num3;
                }
                else
                {
                    items[3] = "No";
                }

                var listViewItem = new ListViewItem(items)
                {
                    Checked = flag | _importBuffer[index].IsNew,
                    Tag = index
                };
                _currentItems.Add(listViewItem);
                lstImport.Items.Add(listViewItem);
            }

            if (lstImport.Items.Count > 0)
                lstImport.Items[0].EnsureVisible();
            lstImport.EndUpdate();
            HideUnchanged.Text = "Hide Unchanged";
            MessageBox.Show($"New: {num2}\r\nModified: {num3}");
        }

        private void frmImportEnhSets_Load(object sender, EventArgs e)
        {
            _fullFileName = "boostsets.csv";
            DisplayInfo();
        }

        private void HideUnchanged_Click(object sender, EventArgs e)
        {
            _showUnchanged = !_showUnchanged;
            lstImport.BeginUpdate();
            lstImport.Items.Clear();
            var num = _currentItems.Count - 1;
            for (var index = 0; index <= num; ++index)
                if (_showUnchanged | (_currentItems[index].SubItems[2].Text == "Yes") |
                    (_currentItems[index].SubItems[3].Text == "Yes"))
                    lstImport.Items.Add(_currentItems[index]);
            lstImport.EndUpdate();
        }

        private bool ParseClasses(string iFileName)
        {
            var num1 = 0;
            StreamReader iStream;
            try
            {
                iStream = new StreamReader(iFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Power CSV Not Opened", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var num3 = 0;
            var num4 = 0;
            _importBuffer = new List<EnhSetData>();
            var num5 = 0;
            string iString;
            do
            {
                iString = FileIO.ReadLineUnlimited(iStream, char.MinValue);
                if (iString == null || iString.StartsWith("#"))
                    continue;
                ++num5;
                if (num5 >= 100)
                {
                    BusyMsg(Strings.Format(num3, "###,##0") + " records parsed.");
                    num5 = 0;
                }

                _importBuffer.Add(new EnhSetData(iString));
                ++num3;
                if (_importBuffer[_importBuffer.Count - 1].IsValid)
                    ++num1;
                else
                    ++num4;
            } while (iString != null);

            iStream.Close();
            MessageBox.Show($"Parse Completed!\r\nTotal Records: {num3}\r\nGood: {num1}\r\nRejected: {num4}",
                "File Parsed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        private bool ProcessImport()
        {
            var num1 = 0;
            BusyMsg("Applying...");
            Enabled = false;
            var importCount = lstImport.Items.Count - 1;
            for (var index = 0; index <= importCount; ++index)
            {
                if (!lstImport.Items[index].Checked)
                    continue;
                _importBuffer[Convert.ToInt32(lstImport.Items[index].Tag)].Apply();
                ++num1;
                if (num1 <= 0 || num1 % 10 != 0)
                    continue;
                BusyMsg("Applying: " + Convert.ToString(index) + " records done.");
                Application.DoEvents();
            }

            Enabled = true;
            BusyMsg("Saving...");
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveMainDatabase(serializer);
            BusyHide();
            MessageBox.Show($"Import of {num1} records completed", "Done", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            DisplayInfo();
            return false;
        }
    }
}