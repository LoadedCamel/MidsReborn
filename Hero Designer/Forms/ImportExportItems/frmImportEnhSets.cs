
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

namespace Hero_Designer
{
    public partial class frmImportEnhSets : Form
    {
        frmBusy _bFrm;

        readonly List<ListViewItem> _currentItems;

        string _fullFileName;

        List<EnhSetData> _importBuffer;

        bool _showUnchanged;


        public frmImportEnhSets()
        {
            Load += frmImportEnhSets_Load;
            _fullFileName = "";
            _showUnchanged = true;
            InitializeComponent();
            Name = nameof(frmImportEnhSets);
            var componentResourceManager = new ComponentResourceManager(typeof(frmImportEnhSets));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            _importBuffer = new List<EnhSetData>();
            _currentItems = new List<ListViewItem>();
        }

        void btnCheckAll_Click(object sender, EventArgs e)

        {
            lstImport.BeginUpdate();
            int num = lstImport.Items.Count - 1;
            for (int index = 0; index <= num; ++index)
                lstImport.Items[index].Checked = true;
            lstImport.EndUpdate();
        }

        void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        void btnFile_Click(object sender, EventArgs e)
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

        void btnImport_Click(object sender, EventArgs e)
        {
            ProcessImport();
        }

        void btnUncheckAll_Click(object sender, EventArgs e)
        {
            lstImport.BeginUpdate();
            int num = lstImport.Items.Count - 1;
            for (int index = 0; index <= num; ++index)
                lstImport.Items[index].Checked = false;
            lstImport.EndUpdate();
        }

        void BusyHide()
        {
            if (_bFrm == null)
                return;
            _bFrm.Close();
            _bFrm = null;
        }

        void BusyMsg(string sMessage)
        {
            if (_bFrm == null)
            {
                _bFrm = new frmBusy();
                _bFrm.Show(this);
            }
            _bFrm.SetMessage(sMessage);
        }

        void DisplayInfo()
        {
            lblFile.Text = FileIO.StripPath(_fullFileName);
        }

        void FillListView()
        {
            string[] items = new string[6];
            lstImport.BeginUpdate();
            lstImport.Items.Clear();
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = _importBuffer.Count - 1;
            for (int index = 0; index <= num4; ++index)
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
                bool flag = false;
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
                    items[3] = "No";
                ListViewItem listViewItem = new ListViewItem(items)
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
            int num5 = (int)Interaction.MsgBox(("New: " + Convert.ToString(num2) + "\r\nModified: " + Convert.ToString(num3)));
        }

        void frmImportEnhSets_Load(object sender, EventArgs e)
        {
            _fullFileName = "boostsets.csv";
            DisplayInfo();
        }

        void HideUnchanged_Click(object sender, EventArgs e)
        {
            _showUnchanged = !_showUnchanged;
            lstImport.BeginUpdate();
            lstImport.Items.Clear();
            int num = _currentItems.Count - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (_showUnchanged | _currentItems[index].SubItems[2].Text == "Yes" | _currentItems[index].SubItems[3].Text == "Yes")
                    lstImport.Items.Add(_currentItems[index]);
            }
            lstImport.EndUpdate();
        }

        bool ParseClasses(string iFileName)
        {
            int num1 = 0;
            StreamReader iStream;
            try
            {
                iStream = new StreamReader(iFileName);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                int num2 = (int)Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical, "Power CSV Not Opened");
                ProjectData.ClearProjectError();
                return false;
            }
            int num3 = 0;
            int num4 = 0;
            _importBuffer = new List<EnhSetData>();
            int num5 = 0;
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
            }
            while (iString != null);
            iStream.Close();
            int num6 = (int)Interaction.MsgBox(("Parse Completed!\r\nTotal Records: " + Convert.ToString(num3) + "\r\nGood: " + Convert.ToString(num1) + "\r\nRejected: " + Convert.ToString(num4)), MsgBoxStyle.Information, "File Parsed");
            return true;
        }

        bool ProcessImport()
        {
            int num1 = 0;
            BusyMsg("Applying...");
            Enabled = false;
            int importCount = lstImport.Items.Count - 1;
            for (int index = 0; index <= importCount; ++index)
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
            Interaction.MsgBox(("Import of " + Convert.ToString(num1) + " records completed!"), MsgBoxStyle.Information, "Done");
            DisplayInfo();
            return false;
        }
    }
}