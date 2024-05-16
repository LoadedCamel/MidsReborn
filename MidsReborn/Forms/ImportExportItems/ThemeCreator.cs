using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Extensions;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public partial class ThemeCreator : Form
    {
        private bool _isValidationSuccessful = true;
        public ColorTheme? CreatedTheme;

        public ThemeCreator()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            Load += OnLoad;
            tbName.PlaceholderText = @"Enter theme name here";
            tbName.Validating += TextBox_Validating;
            panelTitle.Click += Panel_Click;
            panelTitle.Validating += Panel_Validating;
            panelHeadings.Click += Panel_Click;
            panelHeadings.Validating += Panel_Validating;
            panelLevels.Click += Panel_Click;
            panelLevels.Validating += Panel_Validating;
            panelSlots.Click += Panel_Click;
            panelSlots.Validating += Panel_Validating;
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            CenterToParent();
        }

        private void TextBox_Validating(object? sender, CancelEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                errorProvider1.SetError(tbName, "Text cannot be blank.");
                e.Cancel = true;
                _isValidationSuccessful = false;
            }
            else
            {
                errorProvider1.SetError(tbName, null);
            }
        }

        private void Panel_Click(object? sender, EventArgs e)
        {
            if (sender is not Panel panel) return;
            using var colorDialog = new ColorDialogExt(this, $"{panel.Name.Replace("panel", "")} Color");
            colorDialog.Color = panel.BackColor;
            var result = colorDialog.ShowDialog(this);
            if (result != DialogResult.OK) return;
            panel.BackColor = colorDialog.Color;
        }

        private void Panel_Validating(object? sender, CancelEventArgs e)
        {
            if (sender is not Panel panel) return;
            if (panel.BackColor == Color.Empty)
            {
                errorProvider1.SetError(panel, "Color cannot be empty.");
                e.Cancel = true;
                _isValidationSuccessful = false;
            }
            else
            {
                errorProvider1.SetError(panel, null);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (_isValidationSuccessful)
            {
                CreatedTheme = new ColorTheme
                {
                    Name = tbName.Text,
                    Title = panelTitle.BackColor,
                    Headings = panelHeadings.BackColor,
                    Levels = panelLevels.BackColor,
                    Slots = panelSlots.BackColor,
                    DarkTheme = chkIsDarkTheme.Checked
                };
                DialogResult = DialogResult.Continue;
                Close();
            }
            else
            {
                using var errorBox = new MessageBoxEx("Validation Error", "Please correct validation errors or cancel by clicking the x in the top right hand corner.", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error, true);
                errorBox.ShowDialog(this);
            }
        }
    }
}
