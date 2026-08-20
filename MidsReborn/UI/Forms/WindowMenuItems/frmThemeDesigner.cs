using FastDeepCloner;
using FontAwesome.Sharp;
using Mids_Reborn.UI.Forms.Controls;
using Mids_Reborn.UI.Theming;
using System.Diagnostics;
using System.Text.Json;

namespace Mids_Reborn.UI.Forms.WindowMenuItems;

public partial class frmThemeDesigner : Form
{
    private struct ThemeField
    {
        public string Field;
        public string DisplayName;
    }

    private struct ThemeFieldsGroup
    {
        public string Name;
        public string DisplayName;
    }

    private Dictionary<ThemeFieldsGroup, List<ThemeField>> ThemeItems;
    private ApplicationTheme OriginalTheme;
    private ApplicationTheme WorkingTheme;
    private Dictionary<string, BorderPanel> ColorBoxList;
    private Dictionary<string, TextBox> TextBoxList;
    private Dictionary<string, IconButton> ModButtonList;
    private Dictionary<string, IconButton> RevButtonList;
    private bool AutoApplyTheme = true;
    private frmColorSelector FrmColorSelector;
    private MainWindow2 ParentWindow;
    private readonly JsonSerializerOptions SerializerOptions;
    private const string ThemeVersion = "1.1";

    public frmThemeDesigner(MainWindow2 parent)
    {
        InitializeComponent();
        Icon = MRBResourceLib.Resources.MRB_Icon_Concept;
        InitThemeMappings();
        ColorBoxList = [];
        TextBoxList = [];
        ModButtonList = [];
        RevButtonList = [];
        FrmColorSelector = new frmColorSelector(Color.Black, "");
        FrmColorSelector.Visible = false;
        FrmColorSelector.VisibleChanged += FrmColorSelector_VisibleChanged;
        ParentWindow = parent;
        SerializerOptions = new JsonSerializerOptions { WriteIndented = true };
    }

    // Simulate form shown by .ShowDialog()
    // When form is going hidden, check for DialogResult and apply updates in the main UI
    private void FrmColorSelector_VisibleChanged(object? sender, EventArgs e)
    {
        if (FrmColorSelector.Visible)
        {
            return;
        }

        if (string.IsNullOrEmpty(FrmColorSelector.SourceControl))
        {
            return;
        }

        if (FrmColorSelector.DialogResult != DialogResult.OK)
        {
            return;
        }

        var ctl = ColorBoxList[FrmColorSelector.SourceControl];
        ctl.BackColor = FrmColorSelector.SelectedColor;

        var textBox = TextBoxList[FrmColorSelector.SourceControl.Replace("bp-", "tb-")];
        textBox.Text = $"{FrmColorSelector.SelectedColor.R}, {FrmColorSelector.SelectedColor.G}, {FrmColorSelector.SelectedColor.B}";
    }

    private void frmThemeDesigner_Load(object sender, EventArgs e)
    {
        if (ThemeManager.CurrentTheme == null)
        {
            MessageBox.Show("No theme is currently loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();

            return;
        }

        ThemeManager.SaveTheme();
        WorkingTheme = ThemeManager.CurrentTheme.Clone();
        OriginalTheme = ThemeManager.CurrentTheme.Clone();
        WorkingTheme.Version ??= ThemeVersion;

        // Lock theme change during edit
        // Bug: no muted color for menustrip disabled items
        ParentWindow.AllowThemeChange(false);

        InitUI();
    }

    // Set up theme fields mappings, for user-friendly names
    private void InitThemeMappings()
    {
        ThemeItems = new Dictionary<ThemeFieldsGroup, List<ThemeField>>
        {
            {
                new ThemeFieldsGroup { Name = "Button", DisplayName = "Button" },
                new List<ThemeField>
                {
                    new() { Field = "Border", DisplayName = "Border" },
                    new() { Field = "ForeColor", DisplayName = "Text Color" },
                    new() { Field = "TextOutlineColor", DisplayName = "Text Color (Outline)" },
                    new() { Field = "GradientTop", DisplayName = "Gradient Top" },
                    new() { Field = "GradientBottom", DisplayName = "Gradient Bottom" },
                    new() { Field = "HoverGradientTop", DisplayName = "Hover: Gradient Top" },
                    new() { Field = "HoverGradientBottom", DisplayName = "Hover: Gradient Bottom" },
                    new() { Field = "PressedGradientTop", DisplayName = "Pressed: Gradient Top" },
                    new() { Field = "PressedGradientBottom", DisplayName = "Pressed: Gradient Bottom" },
                    new() { Field = "ToggledGradientTop", DisplayName = "Toggled: Gradient Top" },
                    new() { Field = "ToggledGradientBottom", DisplayName = "Toggled: Gradient Bottom" },
                    new() { Field = "ToggledBorderColor", DisplayName = "Toggled: Border Color" },
                    new() { Field = "ToggledTextColor", DisplayName = "Toggled: Text Color" },
                    new() { Field = "ToggledTextOutlineColor", DisplayName = "Toggled: Text Outline Color" }
                }
            },
            {
                new ThemeFieldsGroup { Name = "DataView", DisplayName = "DataView" },
                new List<ThemeField>
                {
                    new() { Field = "Background", DisplayName = "Background" },
                    new() { Field = "Card", DisplayName = "Card" },
                    new() { Field = "Border", DisplayName = "Border" },
                    new() { Field = "Accent", DisplayName = "Accent" },
                    new() { Field = "Text", DisplayName = "Text" },
                    new() { Field = "Muted", DisplayName = "Muted Text" },

                    new() { Field = "HeaderTop", DisplayName = "Header: Top" },
                    new() { Field = "HeaderBottom", DisplayName = "Header: Bottom" },
                    new() { Field = "TabActiveTop", DisplayName = "Active Tab: Top" },
                    new() { Field = "TabActiveBottom", DisplayName = "Active Tab: Bottom" },
                    new() { Field = "TabBorder", DisplayName = "Tab: Border" },

                    new() { Field = "Chip", DisplayName = "Chip" },
                    new() { Field = "ChipActive", DisplayName = "Chip: Active" },
                    new() { Field = "Rail", DisplayName = "Rail" },
                    new() { Field = "RailFill", DisplayName = "Rail: Fill" },
                    new() { Field = "Thumb", DisplayName = "Thumb" },
                    new() { Field = "ThumbBorder", DisplayName = "Thumb Border" },

                    new() { Field = "GridHeaderTop", DisplayName = "Grid: Header Top" },
                    new() { Field = "GridHeaderBottom", DisplayName = "Grid: Header Bottom" },
                    new() { Field = "GridHeaderBorder", DisplayName = "Grid: Header Border" },

                    new() { Field = "GridRowEven", DisplayName = "Grid: Row (Even)" },
                    new() { Field = "GridRowOdd", DisplayName = "Grid: Row (Odd)" },
                    new() { Field = "GridRowLine", DisplayName = "Grid: Row Line" },
                    new() { Field = "GridGood", DisplayName = "Grid: Good" }, // In Arcane Matrix theme but not in ApplicationTheme classes
                    new() { Field = "GridBad", DisplayName = "Grid: Bad" }, //   In Arcane Matrix theme but not in ApplicationTheme classes
                    new() { Field = "GridNeutral", DisplayName = "Grid: Neutral" }
                }
            },
            {
                new ThemeFieldsGroup { Name = "DropDownList", DisplayName = "Dropdown List" },
                new List<ThemeField>
                {
                    new() { Field = "ForeColor", DisplayName = "Fore Color" },
                    new() { Field = "LockColor", DisplayName = "Lock Color" },
                    new() { Field = "Border", DisplayName = "Border" },
                    new() { Field = "GradientTop", DisplayName = "Gradient: Top" },
                    new() { Field = "GradientBottom", DisplayName = "Gradient: Bottom" },
                    new() { Field = "HoverGradientTop", DisplayName = "Hover: Gradient Top" },
                    new() { Field = "HoverGradientBottom", DisplayName = "Hover: Gradient Bottom" },
                    new() { Field = "HoverBorder", DisplayName = "Border: Hover" },
                    new() { Field = "DropDownBackColor", DisplayName = "Dropdown: Back Color" },
                    new() { Field = "DropDownSelectionBackColor", DisplayName = "Dropdown: Selection Back Color" },
                    new() { Field = "DropDownSelectionForeColor", DisplayName = "Dropdown: Selection Text Color" },
                    new() { Field = "Arrow", DisplayName = "Arrow" },
                    new() { Field = "HoverArrow", DisplayName = "Arrow: Hover" },
                    new() { Field = "FocusBorder", DisplayName = "Border: Focus" }
                }
            },
            {
                new ThemeFieldsGroup { Name = "Header", DisplayName = "Header" },
                new List<ThemeField>
                {
                    new() { Field = "HeaderLight", DisplayName = "Header: Light" },
                    new() { Field = "HeaderMid", DisplayName = "Header: Mid" },
                    new() { Field = "HeaderDark", DisplayName = "Header: Dark" },
                    new() { Field = "WindowIcon", DisplayName = "Window: Icon" },
                    new() { Field = "WindowIconHover", DisplayName = "Window: Icon (Hover)" },
                    new() { Field = "WindowIconPressed", DisplayName = "Window: Icon (Pressed)" },
                    new() { Field = "WindowIconCloseHover", DisplayName = "Window: Close Icon (Hover)" },
                    new() { Field = "WindowIconClosePressed", DisplayName = "Window: Close Icon (Pressed)" },
                    new() { Field = "LogoTargetColor", DisplayName = "Logo: Target Color" }
                }
            },
            {
                new ThemeFieldsGroup { Name = "ListView", DisplayName = "List View" },
                new List<ThemeField>
                {
                    new() { Field = "ScrollBar", DisplayName = "Scroll Bar" },
                    new() { Field = "ScrollButton", DisplayName = "Scroll Button" },

                    new() { Field = "Enabled", DisplayName = "State: Enabled" },
                    new() { Field = "Selected", DisplayName = "State: Selected" },
                    new() { Field = "Disabled", DisplayName = "State: Disabled" },
                    new() { Field = "SelectedDisabled", DisplayName = "State: Selected/Disabled" },
                    new() { Field = "Invalid", DisplayName = "State: Invalid" },
                    new() { Field = "Heading", DisplayName = "State: Headings" }
                }
            },
            {
                new ThemeFieldsGroup { Name = "MenuStrip", DisplayName = "Menu Strip" },
                new List<ThemeField>
                {
                    new() { Field = "ItemSelectedColor", DisplayName = "Item: Selected Color" },
                    new() { Field = "AccentColor", DisplayName = "Accent Color" },
                    new() { Field = "AccentLightColor", DisplayName = "Accent Light Color" }
                }
            },
            {
                new ThemeFieldsGroup { Name = "SegmentedToggle", DisplayName = "Segmented Toggle" },
                new List<ThemeField>
                {
                    new() { Field = "WellTop", DisplayName = "Well: Top" },
                    new() { Field = "WellBottom", DisplayName = "Well: Bottom" },
                    new() { Field = "Divider", DisplayName = "Divider" },
                    new() { Field = "SelectedTop", DisplayName = "Selected: Top" },
                    new() { Field = "SelectedBottom", DisplayName = "Selected: Bottom" },
                    new() { Field = "SelectedBorder", DisplayName = "Selected: Border" },
                    new() { Field = "SelectedText", DisplayName = "Selected: Text" },
                    new() { Field = "SelectedTextOutline", DisplayName = "Selected: Text Outline" },
                    new() { Field = "UnselectedText", DisplayName = "Unselected: Text" },
                    new() { Field = "UnselectedTextOutline", DisplayName = "Unselected: Text Outline" }
                }
            },
            {
                new ThemeFieldsGroup { Name = "PowerSlot", DisplayName = "Power Slot" },
                new List<ThemeField>
                {
                    new() { Field = "ForeColor", DisplayName = "Text Color" },
                    new() { Field = "Border", DisplayName = "Border" },
                    new() { Field = "GradientTop", DisplayName = "Gradient Top" },
                    new() { Field = "GradientBottom", DisplayName = "Gradient Bottom" },
                    new() { Field = "OpenBorder", DisplayName = "Border (Open)" },
                    new() { Field = "EmptyFill", DisplayName = "Fill (Empty)" },
                    new() { Field = "DisabledFill", DisplayName = "Fill (Disabled)" }
                }
            },
            {
                new ThemeFieldsGroup { Name = "ScrollPanel", DisplayName = "Scroll Panel" },
                new List<ThemeField>
                {
                    new() { Field = "Track", DisplayName = "Track" },
                    new() { Field = "Bar", DisplayName = "Bar" },
                    new() { Field = "Hover", DisplayName = "Hover" },
                }
            },
            {
                new ThemeFieldsGroup { Name = "Footer", DisplayName = "Footer" },
                new List<ThemeField>
                {
                    new() { Field = "Background", DisplayName = "Background" },
                    new() { Field = "SummaryText", DisplayName = "Summary Text" },
                    new() { Field = "TotalSlotsText", DisplayName = "Total Slots Text" },
                    new() { Field = "SlotsLeftText", DisplayName = "Slots Left Text" }
                }
            }
        };
    }

    // Enhancement: possible to get initial values directly from class?
    // Ref.: Mids_Reborn\UI\Theming\FooterTheme.cs
    private Color GetDefaultColor(string group, string field)
    {
        return group switch
        {
            // Got to look for default values because group
            // is nullable in theme and may be missing.
            // We want the default colors that match what is seen.            
            "Footer" => field switch
            {
                "Background" => Color.FromArgb(6, 17, 35),
                "SummaryText" => Color.WhiteSmoke,
                "TotalSlotsText" => Color.WhiteSmoke,
                "SlotsLeftText" => Color.FromArgb(115, 255, 110),
                _ => Color.Black
            },
            _ => Color.Black
        };
    }

    private void InitUI()
    {
        const int ScrollbarGutter = 24;
        var y = 60;
        var i = 1;
        panel1.SuspendLayout();

        tbThemeName.Text = WorkingTheme.Name;
        ColorBoxList = [];
        TextBoxList = [];
        ModButtonList = [];
        RevButtonList = [];

        foreach (var g in ThemeItems)
        {
            if (i > 1)
            {
                y += 10;
            }

            var groupLabel = new Label();
            groupLabel.AutoSize = true;
            groupLabel.ForeColor = Color.Goldenrod;
            groupLabel.Font = new Font("Segoe UI", 10, FontStyle.Bold, GraphicsUnit.Point);
            groupLabel.Location = new Point(12, y);
            groupLabel.Name = $"groupLabel{i}";
            groupLabel.Size = new Size(47, 15);
            groupLabel.TabIndex = 0;
            groupLabel.Text = $"--- {g.Key.DisplayName} ---";

            panel1.Controls.Add(groupLabel);

            y += 30;

            foreach (var item in g.Value)
            {
                var color = GetThemeValue(WorkingTheme, g.Key.Name, item.Field, GetDefaultColor(g.Key.Name, item.Field));
                var idKey = $"{g.Key.Name}-{item.Field}";

                var colorBox = new BorderPanel();
                colorBox.BackColor = color;
                colorBox.Border.Color = Color.FromArgb(160, 160, 160);
                colorBox.Border.Style = ButtonBorderStyle.Solid;
                colorBox.Border.Thickness = 1;
                colorBox.Border.Which = BorderPanel.PanelBorder.BorderToDraw.All;
                colorBox.Cursor = Cursors.Hand;
                colorBox.Location = new Point(20, y);
                colorBox.Name = $"borderPanel{i}";
                colorBox.Size = new Size(50, 26);
                colorBox.Tag = $"bp-{idKey}";
                colorBox.Click += ColorBox_Click;

                var textBox = new TextBox();
                textBox.BackColor = Color.FromArgb(18, 18, 18);
                textBox.ForeColor = Color.Gainsboro;
                textBox.Location = new Point(78, y + 1);
                textBox.Name = $"tb-{idKey}";
                textBox.Size = new Size(120, 23);
                textBox.Tag = $"tb-{idKey}";
                textBox.Text = $"{color.R}, {color.G}, {color.B}";
                textBox.TextChanged += ColorTextBox_TextChanged;

                var label = new Label();
                label.AutoSize = true;
                label.ForeColor = Color.Gainsboro;
                label.Location = new Point(206, y + 5);
                label.Name = $"label{i}";
                label.Size = new Size(47, 15);
                label.TabIndex = 0;
                label.Text = item.DisplayName;

                // Modified but not committed indicator
                var iconButton1 = new IconButton();
                iconButton1.BackColor = Color.FromArgb(0, 0, 0);
                iconButton1.FlatStyle = FlatStyle.Popup;
                iconButton1.IconChar = IconChar.Pencil;
                iconButton1.IconColor = Color.DeepSkyBlue;
                iconButton1.IconFont = IconFont.Auto;
                iconButton1.IconSize = 28;
                iconButton1.Location = new Point(panel1.Width - 76 - ScrollbarGutter, y);
                iconButton1.Name = $"btnMod-{idKey}";
                iconButton1.Size = new Size(30, 30);
                iconButton1.Tag = $"btnMod-{idKey}";
                iconButton1.UseVisualStyleBackColor = false;
                iconButton1.Visible = true;

                // Revert to original button
                var iconButton2 = new IconButton();
                iconButton2.BackColor = Color.FromArgb(0, 0, 0);
                iconButton2.Cursor = Cursors.Hand;
                iconButton2.FlatStyle = FlatStyle.Popup;
                iconButton2.IconChar = IconChar.RotateBack;
                iconButton2.IconColor = Color.MediumSeaGreen;
                iconButton2.IconFont = IconFont.Auto;
                iconButton2.IconSize = 28;
                iconButton2.Location = new Point(panel1.Width - 38 - ScrollbarGutter, y); // Bug: position of these is scrambled if initialized with visible = false
                iconButton2.Name = $"btnRev-{idKey}";
                iconButton2.Size = new Size(30, 30);
                iconButton2.Tag = $"btnRev-{idKey}";
                iconButton2.UseVisualStyleBackColor = false;
                iconButton2.Visible = true;
                iconButton2.Click += IbRevert_Click;

                panel1.Controls.Add(colorBox);
                panel1.Controls.Add(textBox);
                panel1.Controls.Add(label);
                panel1.Controls.Add(iconButton1);
                panel1.Controls.Add(iconButton2);

                // Populate indices for quick access
                ColorBoxList.Add($"bp-{idKey}", colorBox);
                TextBoxList.Add($"tb-{idKey}", textBox);
                ModButtonList.Add($"btnMod-{idKey}", iconButton1);
                RevButtonList.Add($"btnRev-{idKey}", iconButton2);

                y += 32;
                i++;
            }
        }

        panel1.ResumeLayout();

        // Hack: attempt to fix buttons' position bug when initialized with Visible = false.
        // Instead, create visible then hide everything
        panel1.SuspendLayout();
        foreach (var btn in ModButtonList)
        {
            btn.Value.Visible = false;
        }

        foreach (var btn in RevButtonList)
        {
            btn.Value.Visible = false;
        }

        panel1.ResumeLayout();
        panel1.Refresh();
    }

    private void ColorBox_Click(object? sender, EventArgs e)
    {
        if (sender == null)
        {
            return;
        }

        var ctl = sender as BorderPanel;
        var tagChunks = ctl.Tag.ToString().Split('-');
        if (tagChunks.Length < 3)
        {
            return;
        }

        var group = tagChunks[1];
        var field = tagChunks[2];
        var loc = ctl.Location;
        var ctlSize = ctl.Size;
        var selectorSize = FrmColorSelector.Size;

        // Default anchor: bottom-right
        // Change if this would make the color selector window go off-screen
        // Out of bounds check only for screen left and top edges.
        var x = loc.X - selectorSize.Width + Location.X;
        var y = loc.Y - 8 - selectorSize.Height + Location.Y;

        if (x < 0)
        {
            x = loc.X + ctlSize.Width + Location.X;
        }

        if (y < 0)
        {
            y = loc.Y + ctlSize.Height + Location.Y;
        }

        FrmColorSelector.Location = new Point(x, y);
        FrmColorSelector.InitAndShow(ctl.BackColor, ctl.Tag.ToString() ?? "");
    }

    private void ColorTextBox_TextChanged(object? sender, EventArgs e)
    {
        if (sender == null)
        {
            return;
        }

        var ctl = sender as TextBox;
        var txt = ctl.Text.Trim();
        var tagChunks = ctl.Tag.ToString().Split('-');
        if (tagChunks.Length < 3)
        {
            return;
        }

        var group = tagChunks[1];
        var field = tagChunks[2];

        // [Bug] Regex validation doesn't work.
        // Split chunks and use int.TryParse() to validate each component.
        var colorChunks = txt.Replace(" ", "").Split(',');
        if (colorChunks.Length != 3)
        {
            return;
        }

        if (!int.TryParse(colorChunks[0], out var r))
        {
            return;
        }

        if (!int.TryParse(colorChunks[1], out var g))
        {
            return;
        }

        if (!int.TryParse(colorChunks[2], out var b))
        {
            return;
        }

        if (r is < 0 or > 255)
        {
            return;
        }

        if (g is < 0 or > 255)
        {
            return;
        }

        if (b is < 0 or > 255)
        {
            return;
        }

        var color = Color.FromArgb(r, g, b);
        var idKey = $"{group}-{field}";

        var colorBox = ColorBoxList[$"bp-{idKey}"];
        colorBox.BackColor = color;

        // Color differs from original - allow revert
        if (color != GetThemeValue(OriginalTheme, group, field, Color.Black))
        {
            RevButtonList[$"btnRev-{idKey}"].Visible = true;
        }

        // Modified, but not committed indicator
        if (!AutoApplyTheme)
        {
            ModButtonList[$"btnMod-{idKey}"].Visible = true;
            return;
        }

        SetThemeValue(group, field, color);
        ThemeManager.ApplyThemeDirect(WorkingTheme);
    }

    private void IbRevert_Click(object? sender, EventArgs e)
    {
        if (sender == null)
        {
            return;
        }

        var ctl = sender as IconButton;
        var tagChunks = ctl.Tag.ToString().Split('-');
        if (tagChunks.Length < 3)
        {
            return;
        }

        var group = tagChunks[1];
        var field = tagChunks[2];

        var originalColor = GetThemeValue(OriginalTheme, group, field, Color.Black);
        Debug.WriteLine($"Original Color for ({group}, {field}): {originalColor}");
        var colorBox = ColorBoxList[$"bp-{group}-{field}"];
        colorBox.BackColor = originalColor;

        var textBox = TextBoxList[$"tb-{group}-{field}"];
        textBox.Text = $"{originalColor.R}, {originalColor.G}, {originalColor.B}";

        RevButtonList[$"btnRev-{group}-{field}"].Visible = false;
        ModButtonList[$"btnMod-{group}-{field}"].Visible = false;

        // Jump back to matching textbox.
        // If not set, will jump back to the next one.
        TextBoxList[$"tb-{group}-{field}"].Focus();
    }

    // Indirect object access from group + field name (getter)
    private T? GetThemeValue<T>(ApplicationTheme theme, string group, string field, T? defaultValue)
    {
        var objGroup = theme.GetType().GetProperty(group);
        if (objGroup == null)
        {
            Debug.WriteLine($"Warning: Group {group} is null in theme");

            return defaultValue;
        }

        var obj = objGroup?.GetValue(theme)?.GetType().GetProperty(field);
        if (obj == null)
        {
            Debug.WriteLine($"Warning: Field {field} in {group} is null in theme");

            return defaultValue;
        }

        return (T?)obj?.GetValue(objGroup?.GetValue(theme));
    }

    private bool ThemeGroupExists(ApplicationTheme theme, string group)
    {
        var objGroup = theme.GetType().GetProperty(group);

        return objGroup != null;
    }

    private bool ThemeFieldExists(ApplicationTheme theme, string group, string field)
    {
        var objGroup = theme.GetType().GetProperty(group);
        if (objGroup == null)
        {
            return false;
        }

        var obj = objGroup?.GetValue(theme)?.GetType().GetProperty(field);

        return obj != null;
    }

    // Indirect object access from group + field name (setter)
    private void SetThemeValue<T>(string group, string field, T value)
    {
        var objGroup = WorkingTheme.GetType().GetProperty(group);
        var obj = objGroup?.GetValue(WorkingTheme)?.GetType().GetProperty(field);

        // Actually needed? Testing on nulls seem unreliable
        var objGroupExists = ThemeGroupExists(WorkingTheme, group);
        var objExists = ThemeFieldExists(WorkingTheme, group, field);

        // Non-existent object group alone will never trigger (bug?)
        // Always non-null group + null field (despite being non-nullable)
        // Compensate for missing groups/fields in theme by creating defaults in group
        // before to input custom value
        if (!objGroupExists || !objExists)
        {
            switch (group)
            {
                case "SegmentedToggle":
                    WorkingTheme.SegmentedToggle = ThemeManager.CreateDerivedSegmentedToggleTheme(WorkingTheme);
                    break;

                case "Footer":
                    WorkingTheme.Footer = ThemeManager.CreateDefaultFooterTheme();
                    break;
            }

            objGroup = WorkingTheme.GetType().GetProperty(group);
            obj = objGroup?.GetValue(WorkingTheme)?.GetType().GetProperty(field);
            objGroupExists = ThemeGroupExists(WorkingTheme, group);
            objExists = ThemeFieldExists(WorkingTheme, group, field);
        }

        if (!objGroupExists || !objExists)
        {
            Debug.WriteLine($"Warning: Group {group} is null or {field} in {group} is null");
            return;
        }

        obj.SetValue(objGroup.GetValue(WorkingTheme), value);
    }

    // Discard all modifications
    private void frmThemeDesigner_FormClosed(object sender, FormClosedEventArgs e)
    {
        // Enh: saved theme should not be restored to its previous version
        ThemeManager.RestoreTheme();
        ParentWindow.AllowThemeChange(true);
        ParentWindow.SetTopMost(false);
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
        Close();
    }

    private void chkAutoApply_CheckedChanged(object sender, EventArgs e)
    {
        btnApply.Visible = !chkAutoApply.Checked;
        AutoApplyTheme = chkAutoApply.Checked;
    }

    private void btnApply_Click(object sender, EventArgs e)
    {
        ThemeManager.ApplyThemeDirect(WorkingTheme);
        foreach (var btn in ModButtonList)
        {
            btn.Value.Visible = false;
        }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        var themeName = tbThemeName.Text.Trim();
        if (string.IsNullOrWhiteSpace(themeName))
        {
            MessageBox.Show("Cannot save theme with a blank name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return;
        }

        if (ThemeManager.BuiltInThemeNames
            .Select(f => f.ToUpperInvariant())
            .Contains(themeName.ToUpperInvariant()))
        {
            MessageBox.Show("You cannot use this theme name because it's one of the built-in themes.\nChoose another name.", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);

            return;
        }

        using var saveDlg = new SaveFileDialog();
        saveDlg.Filter = "JSON (*.json)|*.json";
        saveDlg.InitialDirectory = Path.Combine(AppContext.BaseDirectory, "Themes");
        saveDlg.FileName = $"{themeName}.json";
        var ret = saveDlg.ShowDialog(this);
        if (ret != DialogResult.OK)
        {
            return;
        }

        var jsonContent = JsonSerializer.Serialize(WorkingTheme, SerializerOptions);
        File.WriteAllText(saveDlg.FileName, jsonContent);

        // Reload user themes
        ThemeManager.ReloadCustomThemes();
        ParentWindow.PopulateThemeMenuItems();

        // Copy working theme to original, reset all modified indicators, disable all reverts ?

        if (WorkingTheme.Name.ToUpperInvariant() == OriginalTheme.Name.ToUpperInvariant())
        {
            return;
        }

        var q = MessageBox.Show("Enable new theme now?", "New theme", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (q != DialogResult.Yes)
        {
            return;
        }

        ThemeManager.SetTheme(WorkingTheme.Name);
        ParentWindow.UpdateCheckedTheme();
    }

    private void tbThemeName_TextChanged(object sender, EventArgs e)
    {
        var themeName = tbThemeName.Text.Trim();

        if (string.IsNullOrWhiteSpace(themeName))
        {
            return;
        }

        WorkingTheme.Name = themeName;
    }

    private void chkTopMostSelf_CheckedChanged(object sender, EventArgs e)
    {
        TopMost = chkTopMostSelf.Checked;
    }

    private void chkTopMostParent_CheckedChanged(object sender, EventArgs e)
    {
        ParentWindow.SetTopMost(chkTopMostParent.Checked);
    }
}