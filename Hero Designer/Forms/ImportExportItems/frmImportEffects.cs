using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Hero_Designer.My;
using Import;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer.Forms.ImportExportItems
{
    public partial class frmImportEffects : Form
    {
        private readonly List<ListViewItem> _currentItems;
        private frmBusy _bFrm;

        private string _fullFileName;

        private List<EffectData> _importBuffer;

        private bool _showUnchanged;


        public frmImportEffects()
        {
            Load += frmImportEffects_Load;
            _fullFileName = "";
            _showUnchanged = true;
            InitializeComponent();
            Name = nameof(frmImportEffects);
            var componentResourceManager = new ComponentResourceManager(typeof(frmImportEffects));
            Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
            _importBuffer = new List<EffectData>();
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

        private void btnEraseAll_Click(object sender, EventArgs e)

        {
            var num1 = DatabaseAPI.Database.Power.Length - 1;
            for (var index = 0; index <= num1; ++index)
                DatabaseAPI.Database.Power[index].Effects = new IEffect[0];
            var num2 = _importBuffer.Count - 1;
            for (var index = 0; index <= num2; ++index)
                if (_importBuffer[index].IsValid)
                    _importBuffer[index].IsNew = true;
            MessageBox.Show("All power effects removed!");
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
            lblDate.Text = "Date: " +
                           Strings.Format(DatabaseAPI.Database.PowerEffectVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            udRevision.Value = new decimal(DatabaseAPI.Database.PowerEffectVersion.Revision);
            var num1 = 0;
            var num2 = DatabaseAPI.Database.Power.Length - 1;
            for (var index = 0; index <= num2; ++index)
                if (DatabaseAPI.Database.Power[index].NeverAutoUpdate)
                    ++num1;
            txtNoAU.Text = Convert.ToString(num1) + " powers locked.";
        }

        private void FillListView()

        {
            var items = new string[6];
            lstImport.BeginUpdate();
            lstImport.Items.Clear();
            var num1 = 0;
            var num2 = 0;
            var num3 = 0;
            var num4 = 0;
            var num5 = _importBuffer.Count - 1;
            for (var index = 0; index <= num5; ++index)
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
                items[0] = _importBuffer[index].Data.PowerFullName;
                items[1] = Enum.GetName(_importBuffer[index].Data.EffectType.GetType(),
                    _importBuffer[index].Data.EffectType);
                var flag = false;
                if (_importBuffer[index].IsNew)
                {
                    items[2] = "Yes";
                    if (_importBuffer[index].IsLocked)
                        items[2] = "Lock";
                    ++num2;
                }
                else
                {
                    items[2] = "No";
                    flag = _importBuffer[index]
                        .CheckDifference(
                            ref DatabaseAPI.Database.Power[_importBuffer[index].Index]
                                .Effects[_importBuffer[index].Nid], out items[5]);
                }

                if (flag)
                {
                    items[3] = "Yes";
                    if (_importBuffer[index].IsLocked)
                        items[3] = "Lock";
                    ++num3;
                }
                else
                {
                    items[3] = "No";
                }

                if (_importBuffer[index].IndexChanged)
                {
                    items[4] = "Yes (" + Convert.ToString(_importBuffer[index].Nid) + ")";
                    if (_importBuffer[index].IsLocked)
                        items[2] = "Lock (" + Convert.ToString(_importBuffer[index].Nid) + ")";
                    ++num4;
                }
                else
                {
                    items[4] = "No";
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
            MessageBox.Show($"New: {num2}\r\nModified: {num3}\r\nRe-Indexed: {num4}");
        }

        private void frmImportEffects_Load(object sender, EventArgs e)

        {
            _fullFileName = DatabaseAPI.Database.PowerEffectVersion.SourceFile;
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

        [DebuggerStepThrough]
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
            var num5 = 0;
            _importBuffer = new List<EffectData>();
            var num6 = 0;
            var num7 = -1;
            var num8 = 0;
            var index1 = -1;
            string iString;
            do
            {
                iString = FileIO.ReadLineUnlimited(iStream, char.MinValue);
                if (iString == null || iString.StartsWith("#"))
                    continue;
                ++num6;
                if (num6 >= 99)
                {
                    BusyMsg(Strings.Format(num3, "###,##0") + " records parsed.");
                    Application.DoEvents();
                    num6 = 0;
                }

                ++index1;
                _importBuffer.Add(new EffectData(iString));
                ++num3;
                if (!_importBuffer[index1].IsValid)
                {
                    ++num4;
                }
                else
                {
                    ++num1;
                    var effectData = _importBuffer[index1];
                    if (num7 != effectData.Index)
                    {
                        num7 = effectData.Index;
                        num8 = 0;
                        num5 = 0;
                    }
                    else
                    {
                        ++num8;
                    }

                    effectData.Data.nID = num8;
                    effectData.Nid = num8;
                    if (effectData.Data.nID > DatabaseAPI.Database.Power[effectData.Index].Effects.Length - 1)
                    {
                        effectData.IsNew = true;
                    }
                    else
                    {
                        var index2 = effectData.Nid - num5;
                        if (effectData.CheckSimilar(ref DatabaseAPI.Database.Power[effectData.Index].Effects[index2]))
                        {
                            effectData.Nid = index2;
                            effectData.Data.nID = index2;
                            effectData.IsNew = false;
                            if (num5 > 0)
                                effectData.IndexChanged = true;
                        }
                        else
                        {
                            effectData.IsNew = true;
                            var num2 = DatabaseAPI.Database.Power[effectData.Index].Effects.Length - 1;
                            for (var index3 = 0; index3 <= num2; ++index3)
                            {
                                var flag = index3 <= effectData.Nid &&
                                           _importBuffer[index1 - effectData.Nid + index3].Nid == index3;
                                if (!flag)
                                {
                                    var nid = effectData.Nid;
                                    for (var index4 = 0; index4 <= nid; ++index4)
                                    {
                                        if (_importBuffer[index1 - effectData.Nid + index4].Nid != index3)
                                            continue;
                                        flag = true;
                                        break;
                                    }
                                }

                                if (flag || !effectData.CheckSimilar(ref DatabaseAPI.Database.Power[effectData.Index]
                                    .Effects[index3]))
                                    continue;
                                effectData.Nid = index3;
                                effectData.Data.nID = index3;
                                effectData.IndexChanged = true;
                                effectData.IsNew = false;
                                break;
                            }
                        }
                    }

                    if (effectData.IsNew)
                        ++num5;
                }
            } while (iString != null);

            iStream.Close();
            MessageBox.Show($"Parse Completed!\r\nTotal Records: {num3}\r\nGood: {num1}\r\nRejected: {num4}",
                "File Parsed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        private bool ProcessImport()

        {
            var num1 = 0;
            var num2 = 0;
            BusyMsg("Applying...");
            Enabled = false;
            var num3 = 0;
            var num4 = lstImport.Items.Count - 1;
            for (var index = 0; index <= num4; ++index)
            {
                if (!lstImport.Items[index].Checked)
                    continue;
                if (!_importBuffer[Convert.ToInt32(lstImport.Items[index].Tag)].Apply())
                    ++num3;
                ++num1;
                ++num2;
                if (num2 < 9)
                    continue;
                BusyMsg("Applying: " + Convert.ToString(index) + " records done.");
                Application.DoEvents();
                num2 = 0;
            }

            Enabled = true;
            BusyMsg("Saving...");
            DatabaseAPI.Database.PowerEffectVersion.SourceFile = dlgBrowse.FileName;
            DatabaseAPI.Database.PowerEffectVersion.RevisionDate = DateTime.Now;
            DatabaseAPI.Database.PowerEffectVersion.Revision = Convert.ToInt32(udRevision.Value);
            DatabaseAPI.MatchAllIDs();
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveMainDatabase(serializer);
            BusyHide();
            MessageBox.Show($"Import of {num1} records completed!\r\nOf these, {num3} records were found read-only.",
                "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DisplayInfo();
            return false;
        }
    }
}