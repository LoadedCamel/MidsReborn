using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Extensions;
using MRBResourceLib;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmRestoreHidden : Form
    {
        private List<IPower?> _hiddenPowers;
        private bool _ignoreTemps = true;

        public frmRestoreHidden()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            Icon = Resources.MRB_Icon_Concept;
        }

        private void frmRestoreHidden_Load(object sender, EventArgs e)
        {
            lvPowers.EnableDoubleBuffer();
            ckIgnoreTemps.Checked = _ignoreTemps;
            PopulatePowersList();
            CenterToParent();
        }

        private void PopulatePowersList()
        {
            _hiddenPowers = _ignoreTemps
                ? DatabaseAPI.Database.Power.Where(p => p.HiddenPower & !p.FullName.StartsWith("Temporary_Powers.Temporary_Powers.")).ToList()
                : DatabaseAPI.Database.Power.Where(p => p.HiddenPower).ToList();
            lvPowers.BeginUpdate();
            lvPowers.Items.Clear();
            foreach (var p in _hiddenPowers)
            {
                var powerNameChunks = p.FullName.Split('.');
                var lvItem = new ListViewItem();
                lvItem.SubItems.Add(powerNameChunks[0]);
                lvItem.SubItems.Add(powerNameChunks.Length < 2 ? "-" : powerNameChunks[1]);
                lvItem.SubItems.Add(powerNameChunks.Length < 3 ? "-" : powerNameChunks[2]);
                lvPowers.Items.Add(lvItem);
            }
            lvPowers.EndUpdate();
            label1.Text = _hiddenPowers.Count switch
            {
                0 => "No hidden power found.",
                1 => "1 hidden power found:",
                _ => $"{_hiddenPowers.Count} hidden powers found:"
            };
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (_hiddenPowers.Count > 0)
            {
                var n = lvPowers.Items.Count;
                for (var i = 0; i < n; i++)
                {
                    if (lvPowers.Items[i].Checked)
                    {
                        _hiddenPowers[i].HiddenPower = false;
                    }
                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            if (lvPowers.Items.Count <= 0) return;

            lvPowers.BeginUpdate();
            for (var i = 0; i < _hiddenPowers.Count; i++)
            {
                lvPowers.Items[i].Checked = true;
            }

            lvPowers.EndUpdate();
        }

        private void btnSelectNone_Click(object sender, EventArgs e)
        {
            if (lvPowers.Items.Count <= 0) return;

            lvPowers.BeginUpdate();
            for (var i = 0; i < _hiddenPowers.Count; i++)
            {
                lvPowers.Items[i].Checked = false;
            }

            lvPowers.EndUpdate();
        }

        private void ckIgnoreTemps_CheckedChanged(object sender, EventArgs e)
        {
            _ignoreTemps = ckIgnoreTemps.Checked;
            PopulatePowersList();
        }
    }
}