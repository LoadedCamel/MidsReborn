using System.Globalization;
using System.Linq;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;
using MRBResourceLib;

namespace Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor;

public sealed class frmClassAttributes : Form
{
    private readonly string? _initialClassName;
    private readonly string? _initialGroup;
    private readonly TextBox _search = new();
    private readonly ListBox _classes = new();
    private readonly ComboBox _group = new();
    private readonly DataGridView _grid = new();
    private readonly Label _summary = new();

    private static readonly string[] AttributeGroups =
    [
        "Base",
        "Min",
        "Max",
        "MaxMax",
        "StrengthMin",
        "StrengthMax",
        "ResistanceMin",
        "ResistanceMax",
        "DiminishingStrength",
        "DiminishingCurrent",
        "NamedTables"
    ];

    public frmClassAttributes(string? initialClassName = null, string? initialGroup = null)
    {
        _initialClassName = initialClassName;
        _initialGroup = initialGroup;
        Text = @"Class Attributes";
        Icon = Resources.MRB_Icon_Concept;
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(980, 640);
        Size = new Size(1120, 720);
        InitializeUi();
        Load += (_, _) => LoadClassList();
    }

    private void InitializeUi()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            Padding = new Padding(10)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 96));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var left = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1
        };
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        left.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        left.Controls.Add(new Label
        {
            Text = @"Search classes",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        _search.Dock = DockStyle.Fill;
        _search.TextChanged += (_, _) => LoadClassList();
        left.Controls.Add(_search, 0, 1);

        _classes.Dock = DockStyle.Fill;
        _classes.IntegralHeight = false;
        _classes.SelectedIndexChanged += (_, _) => UpdateSelectedClass();
        left.Controls.Add(_classes, 0, 2);

        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        header.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        header.Controls.Add(new Label
        {
            Text = @"Attribute group",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        _group.Dock = DockStyle.Fill;
        _group.DropDownStyle = ComboBoxStyle.DropDownList;
        _group.Items.AddRange(AttributeGroups.Cast<object>().ToArray());
        _group.SelectedIndexChanged += (_, _) => UpdateGrid();
        header.Controls.Add(_group, 1, 0);

        _summary.Dock = DockStyle.Fill;
        _summary.BorderStyle = BorderStyle.FixedSingle;
        _summary.Padding = new Padding(8);
        _summary.TextAlign = ContentAlignment.MiddleLeft;
        header.SetColumnSpan(_summary, 2);
        header.Controls.Add(_summary, 0, 1);

        _grid.Dock = DockStyle.Fill;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AllowUserToResizeRows = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
        _grid.BackgroundColor = Color.White;
        _grid.ReadOnly = true;
        _grid.RowHeadersVisible = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

        root.Controls.Add(left, 0, 0);
        root.SetRowSpan(left, 2);
        root.Controls.Add(header, 1, 0);
        root.Controls.Add(_grid, 1, 1);
        Controls.Add(root);
    }

    private void LoadClassList()
    {
        var selectedClass = (_classes.SelectedItem as ClassListItem)?.ClassName ?? _initialClassName;
        var filter = _search.Text.Trim();
        IEnumerable<OmniClassAttributeTable> classes = DatabaseAPI.Database.ClassAttributes?.Values ?? Enumerable.Empty<OmniClassAttributeTable>();

        _classes.BeginUpdate();
        _classes.Items.Clear();
        foreach (var classAttributes in classes
                     .Where(c => MatchesFilter(c, filter))
                     .OrderByDescending(c => c.Playable)
                     .ThenBy(c => c.DisplayName.Length == 0 ? c.ClassName : c.DisplayName)
                     .ThenBy(c => c.ClassName))
        {
            _classes.Items.Add(new ClassListItem(classAttributes));
        }

        _classes.EndUpdate();

        if (_classes.Items.Count == 0)
        {
            _summary.Text = @"No imported class attributes are available.";
            _grid.Columns.Clear();
            return;
        }

        var selectedIndex = 0;
        for (var i = 0; i < _classes.Items.Count; i++)
        {
            if ((_classes.Items[i] as ClassListItem)?.ClassName.Equals(selectedClass, StringComparison.OrdinalIgnoreCase) == true)
            {
                selectedIndex = i;
                break;
            }
        }

        _classes.SelectedIndex = selectedIndex;
        if (_group.SelectedIndex < 0)
        {
            _group.SelectedItem = ResolveInitialGroup();
        }
    }

    private string ResolveInitialGroup()
    {
        return AttributeGroups.Contains(_initialGroup, StringComparer.OrdinalIgnoreCase)
            ? AttributeGroups.First(group => string.Equals(group, _initialGroup, StringComparison.OrdinalIgnoreCase))
            : "NamedTables";
    }

    private static bool MatchesFilter(OmniClassAttributeTable classAttributes, string filter)
    {
        return string.IsNullOrWhiteSpace(filter) ||
               classAttributes.ClassName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
               classAttributes.DisplayName.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }

    private void UpdateSelectedClass()
    {
        UpdateSummary();
        UpdateGrid();
    }

    private void UpdateSummary()
    {
        if ((_classes.SelectedItem as ClassListItem)?.Table is not { } table)
        {
            _summary.Text = @"Select a class.";
            return;
        }

        var level = Math.Max(1, MidsContext.MathLevelBase + 1);
        var hpBase = Leveled(table.Max, "hit_points");
        var hpCap = Leveled(table.MaxMax, "hit_points");
        var damageCap = Leveled(table.Max, "damage");
        var resCap = Leveled(table.Max, "damage_resistance");
        var rechargeCap = Leveled(table.Max, "recharge_time");
        var recoveryCap = Leveled(table.Max, "recovery");
        var regenCap = Leveled(table.Max, "regeneration");
        var perceptionCap = Leveled(table.MaxMax, "perception_radius");
        var baseRecovery = Scalar(table.Base, "recovery");
        var baseRegen = Scalar(table.Base, "regeneration");
        var baseThreat = Scalar(table.Base, "threat_level");

        _summary.Text =
            $"{table.ClassName} ({(table.Playable ? "Playable" : "Non-playable")}) - Level {level}\r\n" +
            $"HP base {hpBase}, HP cap {hpCap}, Damage cap {Percent(damageCap)}, Resistance cap {Percent(resCap)}, Recharge cap {Percent(rechargeCap)}\r\n" +
            $"Recovery cap {Percent(recoveryCap)}, Regen cap {Percent(regenCap)}, Perception cap {perceptionCap}, Base recovery {baseRecovery}, Base regen {baseRegen}, Base threat {baseThreat}";

        string Scalar(IReadOnlyDictionary<string, float> source, string key)
        {
            return source.TryGetValue(key, out var value) ? Format(value) : "legacy/fallback";
        }

        string Leveled(IReadOnlyDictionary<string, float[]> source, string key)
        {
            if (!source.TryGetValue(key, out var values) || values.Length == 0)
            {
                return "legacy/fallback";
            }

            return Format(values[Math.Clamp(MidsContext.MathLevelBase, 0, values.Length - 1)]);
        }

        static string Percent(string value)
        {
            return value == "legacy/fallback"
                ? value
                : $"{Format(float.Parse(value, CultureInfo.InvariantCulture) * 100f)}%";
        }
    }

    private void UpdateGrid()
    {
        _grid.Columns.Clear();
        _grid.Rows.Clear();

        if ((_classes.SelectedItem as ClassListItem)?.Table is not { } table ||
            _group.SelectedItem is not string group)
        {
            return;
        }

        switch (group)
        {
            case "Base":
                FillScalar(table.Base);
                break;
            case "Min":
                FillScalar(table.Min);
                break;
            case "StrengthMin":
                FillScalar(table.StrengthMin);
                break;
            case "ResistanceMin":
                FillScalar(table.ResistanceMin);
                break;
            case "DiminishingStrength":
                FillScalar(table.DiminishingStrength);
                break;
            case "DiminishingCurrent":
                FillScalar(table.DiminishingCurrent);
                break;
            case "Max":
                FillLeveled(table.Max);
                break;
            case "MaxMax":
                FillLeveled(table.MaxMax);
                break;
            case "StrengthMax":
                FillLeveled(table.StrengthMax);
                break;
            case "ResistanceMax":
                FillLeveled(table.ResistanceMax);
                break;
            case "NamedTables":
                FillLeveled(table.NamedTables);
                break;
        }
    }

    private void FillScalar(IReadOnlyDictionary<string, float> values)
    {
        _grid.Columns.Add("name", "Name");
        _grid.Columns.Add("value", "Value");
        foreach (var pair in values.OrderBy(v => v.Key))
        {
            _grid.Rows.Add(pair.Key, Format(pair.Value));
        }
    }

    private void FillLeveled(IReadOnlyDictionary<string, float[]> tables)
    {
        _grid.Columns.Add("name", "Name");
        var maxLevel = tables.Values.Any() ? tables.Values.Max(v => v.Length) : 0;
        for (var i = 0; i < maxLevel; i++)
        {
            _grid.Columns.Add($"level{i + 1}", $"Level {i + 1}");
        }

        foreach (var pair in tables.OrderBy(v => v.Key))
        {
            var row = new object[maxLevel + 1];
            row[0] = pair.Key;
            for (var i = 0; i < maxLevel; i++)
            {
                row[i + 1] = i < pair.Value.Length ? Format(pair.Value[i]) : string.Empty;
            }

            _grid.Rows.Add(row);
        }
    }

    private static string Format(float value)
    {
        return value.ToString("0.#######", CultureInfo.InvariantCulture);
    }

    private sealed class ClassListItem
    {
        public ClassListItem(OmniClassAttributeTable table)
        {
            Table = table;
        }

        public OmniClassAttributeTable Table { get; }
        public string ClassName => Table.ClassName;

        public override string ToString()
        {
            var display = string.IsNullOrWhiteSpace(Table.DisplayName) ? Table.ClassName : Table.DisplayName;
            return $"{display} - {Table.ClassName}";
        }
    }
}
