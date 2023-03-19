using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public partial class frmForumExport : Form
    {
        private List<ForumColorTheme> ForumThemes;
        private List<string> FormatTypes;
        private ForumColorsHex ThemeColorsHex;

        public frmForumExport()
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            InitializeComponent();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            var includeAccolades = chkOptAccolades.Checked;
            var includeIncarnates = chkOptIncarnates.Checked;
            var longFormat = chkOptLongFormat.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void lbColorTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbColorTheme.SelectedIndex < 0)
            {
                return;
            }

            var theme = ForumThemes[lbColorTheme.SelectedIndex];
            ThemeColorsHex = new ForumColorsHex
            {
                Text = ForumColorThemes.ColorToHex(theme.Text),
                Headings = ForumColorThemes.ColorToHex(theme.Headings),
                Levels = ForumColorThemes.ColorToHex(theme.Levels),
                Slots = ForumColorThemes.ColorToHex(theme.Slots)
            };

            panelColorTitle.BackColor = theme.Text;
            panelColorHeadings.BackColor = theme.Headings;
            panelColorLevels.BackColor = theme.Levels;
            panelColorSlots.BackColor = theme.Slots;
        }

        private void rbLightThemes_CheckedChanged(object sender, EventArgs e)
        {
            lbColorTheme.BeginUpdate();
            lbColorTheme.Items.Clear();
            foreach (var theme in ForumThemes)
            {
                if (theme.DarkTheme)
                {
                    continue;
                }

                lbColorTheme.Items.Add(theme.Name);
            }
            lbColorTheme.EndUpdate();
        }

        private void rbDarkThemes_CheckedChanged(object sender, EventArgs e)
        {
            lbColorTheme.BeginUpdate();
            lbColorTheme.Items.Clear();
            foreach (var theme in ForumThemes)
            {
                if (!theme.DarkTheme)
                {
                    continue;
                }

                lbColorTheme.Items.Add(theme.Name);
            }
            lbColorTheme.EndUpdate();
        }

        private void rbAllThemes_CheckedChanged(object sender, EventArgs e)
        {
            lbColorTheme.BeginUpdate();
            lbColorTheme.Items.Clear();
            foreach (var theme in ForumThemes)
            {
                lbColorTheme.Items.Add(theme.Name);
            }
            lbColorTheme.EndUpdate();
        }

        private void frmForumExport_Load(object sender, EventArgs e)
        {
            FormatTypes = new List<string>
            {
                "BBCode",
                "HTML",
                "Markdown",
                "No Codes"
            };

            ForumThemes = ForumColorThemes.GetThemes();
            var themeNames = ForumThemes.Select(e => e.Name).ToList();

            lbColorTheme.BeginUpdate();
            lbColorTheme.Items.Clear();
            foreach (var th in themeNames)
            {
                lbColorTheme.Items.Add(th);
            }
            lbColorTheme.EndUpdate();

            lbFormatCodeType.BeginUpdate();
            lbFormatCodeType.Items.Clear();
            foreach (var type in FormatTypes)
            {
                lbFormatCodeType.Items.Add(type);
            }
            lbFormatCodeType.EndUpdate();
        }
    }
}
