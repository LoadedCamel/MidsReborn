using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.ShareSystem;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public partial class SharedBuilds : Form
    {
        private ExpiringCollection? _collection;
        private ExpiringCollectionItem? _selectedBuild;

        internal ClientTModel? FetchedData { get; set; }

        public SharedBuilds()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            Load += OnLoad;
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            WinApi.StylizeWindow(Handle, Color.Silver, Color.Black, Color.WhiteSmoke);
            _collection ??= new ExpiringCollection();
            _collection.DeserializeFromJson();
            alvShared.DataSource = _collection.GetItems();
            alvShared.AddColumnMapping(0, obj => ((ExpiringCollectionItem)obj).Id);
            alvShared.AddColumnMapping(1, obj => ((ExpiringCollectionItem)obj).SharedOn);
        }

        private void SharedBuilds_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (alvShared.SelectedItems.Count == 0) return;
            var selectedIndex = alvShared.SelectedIndices[0];
            if (selectedIndex < 0 || selectedIndex >= alvShared.Items.Count) return;
            if (alvShared.DataSource is not List<ExpiringCollectionItem> builds) return;
            _selectedBuild = builds[selectedIndex];

            // Display build info 
            SetControlText(txtBuildName, _selectedBuild.Name);
            SetControlText(txtArchetype, _selectedBuild.Archetype);
            SetControlText(txtPrimary, _selectedBuild.Primary);
            SetControlText(txtSecondary, _selectedBuild.Secondary);
            SetControlText(txtDescript, _selectedBuild.Description);
            SetControlText(txtDownloadUrl, _selectedBuild.DownloadUrl);
            SetControlText(txtImageUrl, _selectedBuild.ImageUrl);
            SetControlText(txtSchemaUrl, _selectedBuild.SchemaUrl);
            SetControlText(txtExpiresOn, _selectedBuild.ExpiresAt);
        }

        private static void SetControlText(Control control, string? text)
        {
            control.Text = text;
        }

        private async void EditSelectedBuild_Click(object sender, EventArgs e)
        {
            var message = new MessageBoxEx("Confirm Action", "You are about to open a previously shared build for editing.\r\nThis action will replace any build currently loaded and as such it is advised to save your current work first.\r\nDo you wish to continue?", MessageBoxEx.MessageBoxExButtons.YesNo, MessageBoxEx.MessageBoxExIcon.Warning);
            var result = message.ShowDialog(this);
            if (result != DialogResult.Yes) return;
            var fetchResult = await ShareClient.FetchData(_selectedBuild?.Id!);
            if (fetchResult is { IsSuccessful: true, Data: not null })
            {
                FetchedData = new ClientTModel(fetchResult.Data.Id!, fetchResult.Data.BuildData!);
                DialogResult = DialogResult.Continue;
                Close();
            }

            toolStripStatusLabel1.Text = fetchResult.Message;
        }

        private async void RefreshSelectedBuild_Click(object sender, EventArgs e)
        {
            if (_selectedBuild == null) return;
            var refreshResult = await ShareClient.RefreshShare(_selectedBuild.Id!);
            if (refreshResult.IsSuccessful)
            {
                toolStripStatusLabel1.Text = refreshResult.Message;
                if (refreshResult.Data is null) return;
                _selectedBuild.ExpiresAt = refreshResult.Data?.ExpiresAt;
                _collection?.Update(_selectedBuild);
            }
            else
            {
                toolStripStatusLabel1.Text = refreshResult.Message;
            }

            await Task.Delay(5000).ContinueWith(x =>
                toolStripStatusLabel1.Text = @"Select a build or choose an action item from the menu.");
        }
    }
}
