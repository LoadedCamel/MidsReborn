
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

namespace Hero_Designer
{
    public partial class frmImportEffects : Form
    {
        frmBusy _bFrm;

        readonly List<ListViewItem> _currentItems;

        string _fullFileName;

        List<EffectData> _importBuffer;

        bool _showUnchanged;


        public frmImportEffects()
        {
            Load += frmImportEffects_Load;
            _fullFileName = "";
            _showUnchanged = true;
            InitializeComponent();
            Name = nameof(frmImportEffects);
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmImportEffects));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            _importBuffer = new List<EffectData>();
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

        void btnEraseAll_Click(object sender, EventArgs e)

        {
            int num1 = DatabaseAPI.Database.Power.Length - 1;
            for (int index = 0; index <= num1; ++index)
                DatabaseAPI.Database.Power[index].Effects = new IEffect[0];
            int num2 = _importBuffer.Count - 1;
            for (int index = 0; index <= num2; ++index)
            {
                if (_importBuffer[index].IsValid)
                    _importBuffer[index].IsNew = true;
            }
            int num3 = (int)Interaction.MsgBox("All power effects removed!");
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
            lblDate.Text = "Date: " + Strings.Format(DatabaseAPI.Database.PowerEffectVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            udRevision.Value = new Decimal(DatabaseAPI.Database.PowerEffectVersion.Revision);
            int num1 = 0;
            int num2 = DatabaseAPI.Database.Power.Length - 1;
            for (int index = 0; index <= num2; ++index)
            {
                if (DatabaseAPI.Database.Power[index].NeverAutoUpdate)
                    ++num1;
            }
            txtNoAU.Text = Convert.ToString(num1) + " powers locked.";
        }

        void FillListView()

        {
            string[] items = new string[6];
            lstImport.BeginUpdate();
            lstImport.Items.Clear();
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = _importBuffer.Count - 1;
            for (int index = 0; index <= num5; ++index)
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
                items[1] = Enum.GetName(_importBuffer[index].Data.EffectType.GetType(), _importBuffer[index].Data.EffectType);
                bool flag = false;
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
                    flag = _importBuffer[index].CheckDifference(ref DatabaseAPI.Database.Power[_importBuffer[index].Index].Effects[_importBuffer[index].Nid], out items[5]);
                }
                if (flag)
                {
                    items[3] = "Yes";
                    if (_importBuffer[index].IsLocked)
                        items[3] = "Lock";
                    ++num3;
                }
                else
                    items[3] = "No";
                if (_importBuffer[index].IndexChanged)
                {
                    items[4] = "Yes (" + Convert.ToString(_importBuffer[index].Nid) + ")";
                    if (_importBuffer[index].IsLocked)
                        items[2] = "Lock (" + Convert.ToString(_importBuffer[index].Nid) + ")";
                    ++num4;
                }
                else
                    items[4] = "No";
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
            int num6 = (int)Interaction.MsgBox(("New: " + Convert.ToString(num2) + "\r\nModified: " + Convert.ToString(num3) + "\r\nRe-Indexed: " + Convert.ToString(num4)));
        }

        void frmImportEffects_Load(object sender, EventArgs e)

        {
            _fullFileName = DatabaseAPI.Database.PowerEffectVersion.SourceFile;
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

        [DebuggerStepThrough]

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
            int num5 = 0;
            _importBuffer = new List<EffectData>();
            int num6 = 0;
            int num7 = -1;
            int num8 = 0;
            int index1 = -1;
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
                    EffectData effectData = _importBuffer[index1];
                    if (num7 != effectData.Index)
                    {
                        num7 = effectData.Index;
                        num8 = 0;
                        num5 = 0;
                    }
                    else
                        ++num8;
                    effectData.Data.nID = num8;
                    effectData.Nid = num8;
                    if (effectData.Data.nID > DatabaseAPI.Database.Power[effectData.Index].Effects.Length - 1)
                    {
                        effectData.IsNew = true;
                    }
                    else
                    {
                        int index2 = effectData.Nid - num5;
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
                            int num2 = DatabaseAPI.Database.Power[effectData.Index].Effects.Length - 1;
                            for (int index3 = 0; index3 <= num2; ++index3)
                            {
                                bool flag = index3 <= effectData.Nid && _importBuffer[index1 - effectData.Nid + index3].Nid == index3;
                                if (!flag)
                                {
                                    int nid = effectData.Nid;
                                    for (int index4 = 0; index4 <= nid; ++index4)
                                    {
                                        if (_importBuffer[index1 - effectData.Nid + index4].Nid != index3)
                                            continue;
                                        flag = true;
                                        break;
                                    }
                                }

                                if (flag || !effectData.CheckSimilar(ref DatabaseAPI.Database.Power[effectData.Index].Effects[index3]))
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
            }
            while (iString != null);
            iStream.Close();
            int num9 = (int)Interaction.MsgBox(("Parse Completed!\r\nTotal Records: " + Convert.ToString(num3) + "\r\nGood: " + Convert.ToString(num1) + "\r\nRejected: " + Convert.ToString(num4)), MsgBoxStyle.Information, "File Parsed");
            return true;
        }

        bool ProcessImport()

        {
            int num1 = 0;
            int num2 = 0;
            BusyMsg("Applying...");
            Enabled = false;
            int num3 = 0;
            int num4 = lstImport.Items.Count - 1;
            for (int index = 0; index <= num4; ++index)
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
            int num5 = (int)Interaction.MsgBox(("Import of " + Convert.ToString(num1) + " records completed!\r\nOf these, " + Convert.ToString(num3) + " records were found read-only."), MsgBoxStyle.Information, "Done");
            DisplayInfo();
            return false;
        }
    }
}