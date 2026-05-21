#nullable enable
using System.Text.RegularExpressions;
using FastDeepCloner;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;
using Mids_Reborn.UI.Controls;
using MRBResourceLib;

namespace Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor
{
    public sealed partial class frmEffectConditionals : Form
    {
        public readonly List<KeyValue<string, string>> Conditionals;
        public AdvancedConditionSet AdvancedConditions { get; set; }

        private readonly List<string> _conditionalTypes;
        private readonly List<string> _conditionalOps;
        private readonly TextBox _advancedExpressionText = new();
        private readonly Label _builderHint = new();
        private readonly Button _updateRow = new();
        private bool _loadingModernRow;
        private Dictionary<string, string> _CSFieldsRev;

        public frmEffectConditionals(AdvancedConditionSet? advancedConditions)
        {
            InitializeComponent();
            _conditionalTypes =
            [
                "Power Active",
                "Power Taken",
                "Stacks",
                "Team Members",
                "Combat Setting",
                "Source Mode",
                "Target Entity Type",
                "Target Mode",
                "Character Archetype",
                "Character Level",
                "Advanced Expression"
            ];
            _conditionalOps = ["Equal To", "Greater Than", "Less Than"];
            _CSFieldsRev = ConfigData.CombatContext.EnumerateFields(MidsContext.Config.CombatContextSettings)
                .ToDictionary(ConfigData.CombatContext.FormatSettingName, e => e);

            Conditionals = [];
            AdvancedConditions = advancedConditions?.Clone() ?? new AdvancedConditionSet();

            Text = @"Effect Conditions";
            Icon = Resources.MRB_Icon_Concept;
            Load += OnLoad;
            ApplyConditionBuilderLayout();
            BuildModernConditionEditor();
        }

        private async void OnLoad(object? sender, EventArgs e)
        {
            CenterToParent();
            await UpdateConditionTypes();
            await UpdateConditionals();
            PopulateModernTypes();
            RefreshModernConditionRows();
        }

        private async Task UpdateConditionTypes()
        {
            lvConditionalType.BeginUpdate();
            lvConditionalType.Items.Clear();
            //var indexVal = _conditionalTypes.Count - 1;
            foreach (var c in _conditionalTypes)
            {
                lvConditionalType.Items.Add(c);
            }

            /*if (indexVal > -1)
            {
                lvConditionalType.Items[indexVal].Selected = true;
                lvConditionalType.Items[indexVal].EnsureVisible();
            }*/

            lvConditionalType.View = View.Details;
            lvConditionalType.EndUpdate();

            lvConditionalOp.Visible = false;
            lvConditionalBool.Visible = false;
            _advancedExpressionText.Visible = false;

            /*if (lvConditionalOp.Items.Count != 0) return;

            foreach (var op in _conditionalOps)
            {
                lvConditionalOp.Items.Add(op);
            }*/

            await Task.CompletedTask;
        }

        private static IEnumerable<float> FloatRange(float min, float max, float step)
        {
            for (var i = 0; i < int.MaxValue; i++)
            {
                var value = min + step * i;
                if (value > max)
                {
                    break;
                }

                yield return value;
            }
        }

        private async Task UpdateConditionals()
        {
            RefreshConditionRows();

            panelLinkType.Visible = AdvancedConditions.Rows.Count > 0;
            rbLinkTypeAnd.Checked = true;

            await Task.CompletedTask;
        }

        private void RefreshConditionRows()
        {
            lvActiveConditionals.BeginUpdate();
            lvActiveConditionals.Items.Clear();
            for (var i = 0; i < AdvancedConditions.Rows.Count; i++)
            {
                lvActiveConditionals.Items.Add(BuildAdvancedConditionItem(AdvancedConditions.Rows[i], i));
            }

            lvActiveConditionals.EndUpdate();
        }

        private static bool IsLegacyCondition(AdvancedConditionRow row)
        {
            return row.Kind is AdvancedConditionKind.PowerActive
                or AdvancedConditionKind.PowerTaken
                or AdvancedConditionKind.PowerStacks
                or AdvancedConditionKind.TeamMembers
                or AdvancedConditionKind.CombatSetting;
        }

        private static ListViewItem BuildAdvancedConditionItem(AdvancedConditionRow row, int index)
        {
            var linkType = index > 0
                ? row.Link == AdvancedConditionLink.Or ? "OR" : "AND"
                : "";
            var item = new ListViewItem { Text = linkType, Name = row.Subject };
            item.SubItems.Add(GetAdvancedConditionDisplay(row));
            item.SubItems.Add(AdvancedConditionSet.FormatOperator(row.Operator));
            item.SubItems.Add(GetAdvancedConditionValue(row));
            return item;
        }

        private static string GetAdvancedConditionDisplay(AdvancedConditionRow row)
        {
            var suffix = GetEvaluationModeSuffix(row);
            return row.Kind switch
            {
                AdvancedConditionKind.SourceMode => $"Source Mode:{FormatModeSubject(row.Subject)}{suffix}",
                AdvancedConditionKind.PowerActive => $"Power Active:{GetPowerDisplayName(row.Subject)}",
                AdvancedConditionKind.PowerTaken => $"Power Taken:{GetPowerDisplayName(row.Subject)}",
                AdvancedConditionKind.PowerStacks => $"Stacks:{GetPowerDisplayName(row.Subject)}",
                AdvancedConditionKind.TeamMembers => $"Team Members:{row.Subject}",
                AdvancedConditionKind.CombatSetting => $"Combat Setting:{ConfigData.CombatContext.FormatSettingName(row.Subject)}",
                AdvancedConditionKind.TargetEntityType when row.TargetScope != AdvancedConditionTargetScope.Unknown => $"Target:{FormatTargetScope(row.TargetScope)}{suffix}",
                AdvancedConditionKind.TargetEntityType => $"Target Entity:{row.Value}{suffix}",
                AdvancedConditionKind.TargetMode => $"Target Mode:{row.Subject}{suffix}",
                AdvancedConditionKind.CharacterArchetype => $"Character Archetype:{row.Value}",
                AdvancedConditionKind.CharacterLevel => "Character Level",
                AdvancedConditionKind.PowerCount => $"Power Count:{row.Subject}",
                AdvancedConditionKind.AdvancedExpression => $"Advanced:{row.RawExpression}{suffix}",
                _ => $"{row.Kind}:{row.Subject}{suffix}"
            };
        }

        private static string FormatModeSubject(string modeName)
        {
            return PlannerModeMapper.TryGetPlannerMode(modeName, out var plannerMode)
                ? PlannerModeMapper.ToDisplayName(plannerMode)
                : modeName;
        }

        private static string GetEvaluationModeSuffix(AdvancedConditionRow row)
        {
            return row.EvaluationMode switch
            {
                AdvancedConditionEvaluationMode.RuntimeTargetOnly => " (runtime target condition, preserved but not evaluated by Mids)",
                AdvancedConditionEvaluationMode.ReportOnly => " (report only, preserved but not evaluated by Mids)",
                _ => row.Unsupported ? " (not simulated)" : ""
            };
        }

        private static string GetAdvancedConditionValue(AdvancedConditionRow row)
        {
            return row.Kind switch
            {
                AdvancedConditionKind.SourceMode or AdvancedConditionKind.TargetMode => (!row.Negated).ToString(),
                AdvancedConditionKind.PowerActive or AdvancedConditionKind.PowerTaken => row.Value,
                AdvancedConditionKind.TargetEntityType when row.TargetScope != AdvancedConditionTargetScope.Unknown => FormatTargetScope(row.TargetScope),
                AdvancedConditionKind.TargetEntityType or AdvancedConditionKind.CharacterArchetype => row.Value,
                AdvancedConditionKind.CharacterLevel => row.Value,
                AdvancedConditionKind.PowerCount => $"{AdvancedConditionSet.FormatOperator(row.Operator)} {row.Value}",
                _ => row.Value
            };
        }

        private static string FormatTargetScope(AdvancedConditionTargetScope scope)
        {
            return scope switch
            {
                AdvancedConditionTargetScope.Self => "Self",
                AdvancedConditionTargetScope.Pet => "Pet",
                AdvancedConditionTargetScope.Player => "Player",
                AdvancedConditionTargetScope.Ally => "Ally",
                AdvancedConditionTargetScope.Foe => "Foe",
                _ => "Unknown"
            };
        }

        private static AdvancedConditionTargetScope ParseTargetScope(string value)
        {
            return value.Trim().ToLowerInvariant() switch
            {
                "self" => AdvancedConditionTargetScope.Self,
                "pet" => AdvancedConditionTargetScope.Pet,
                "player" => AdvancedConditionTargetScope.Player,
                "ally" => AdvancedConditionTargetScope.Ally,
                "foe" => AdvancedConditionTargetScope.Foe,
                "critter" => AdvancedConditionTargetScope.Foe,
                _ => AdvancedConditionTargetScope.Unknown
            };
        }

        private static void AddTargetScopeChoices(ListView listView)
        {
            listView.Items.Add("Self").Name = "Self";
            listView.Items.Add("Pet").Name = "Pet";
            listView.Items.Add("Player").Name = "Player";
            listView.Items.Add("Ally").Name = "Ally";
            listView.Items.Add("Foe").Name = "Foe";
        }

        private static string GetPowerDisplayName(string powerName)
        {
            return DatabaseAPI.GetPowerByFullName(powerName)?.DisplayName ?? powerName;
        }

        private void ApplyConditionBuilderLayout()
        {
            Text = @"Effect Conditions";
            groupBox2.Text = @"Build Conditions";
            columnHeader3.Text = @"Condition";
            columnHeader2.Text = @"Pick";
            columnHeader7.Text = @"Compare";
            columnHeader4.Text = @"Value";
            columnHeader1.Text = @"";
            columnHeader5.Text = @"Current Conditions";
            columnHeader8.Text = @"";
            columnHeader6.Text = @"Value";

            lvConditionalType.SetBounds(12, 38, 172, 388);
            lvSubConditional.SetBounds(194, 38, 420, 388);
            lvConditionalOp.SetBounds(624, 38, 118, 118);
            lvConditionalBool.SetBounds(752, 38, 112, 388);
            lvActiveConditionals.SetBounds(874, 38, 396, 388);
            panelLinkType.SetBounds(624, 164, 240, 62);
            addConditional.SetBounds(624, 236, 240, 32);
            removeConditional.SetBounds(624, 276, 240, 32);
            tbFilter.SetBounds(207, 494, 300, 23);
            btnClearFilter.SetBounds(515, 494, 112, 23);

            addConditional.Text = @"Add Condition";
            removeConditional.Text = @"Remove Selected";
            label1.Text = @"Combine with previous row:";
            rbLinkTypeAnd.Location = new Point(17, 34);
            rbLinkTypeOr.Location = new Point(92, 34);

            _builderHint.AutoSize = false;
            _builderHint.Text = @"Choose a condition, pick a value, then add it to the list.";
            _builderHint.ForeColor = Color.Azure;
            _builderHint.Location = new Point(12, 18);
            _builderHint.Size = new Size(520, 18);
            groupBox2.Controls.Add(_builderHint);

            _advancedExpressionText.Multiline = true;
            _advancedExpressionText.ScrollBars = ScrollBars.Vertical;
            _advancedExpressionText.Visible = false;
            _advancedExpressionText.Location = lvSubConditional.Location;
            _advancedExpressionText.Size = new Size(420, 388);
            groupBox2.Controls.Add(_advancedExpressionText);

            columnHeader3.Width = 148;
            columnHeader2.Width = 390;
            columnHeader7.Width = 92;
            columnHeader4.Width = 86;
            columnHeader1.Width = 48;
            columnHeader5.Width = 254;
            columnHeader8.Width = 42;
            columnHeader6.Width = 70;
        }

        private void BuildModernConditionEditor()
        {
            groupBox2.Visible = false;
            tbFilter.Visible = false;
            btnClearFilter.Visible = false;
            ConfigureModernUpdateButton();
            _conditionRows.SelectedIndexChanged -= ModernConditionRowsSelectedIndexChanged;
            _conditionRows.SelectedIndexChanged += ModernConditionRowsSelectedIndexChanged;
            _modernPanel.BringToFront();
            btnOkay.BringToFront();
            btnCancel.BringToFront();
        }

        private void ConfigureModernUpdateButton()
        {
            if (_updateRow.Parent == null)
            {
                _updateRow.BackColor = Color.FromArgb(54, 96, 72);
                _updateRow.FlatStyle = FlatStyle.Popup;
                _updateRow.ForeColor = Color.White;
                _updateRow.Name = "_updateRow";
                _updateRow.Text = @"Update Selected";
                _updateRow.UseVisualStyleBackColor = false;
                _updateRow.Click += UpdateModernCondition_Click;
                _modernPanel.Controls.Add(_updateRow);
            }

            _updateRow.Location = new Point(462, 230);
            _updateRow.Size = new Size(274, 32);
            _updateRow.TabIndex = 15;
            _updateRow.Enabled = _conditionRows.SelectedIndices.Count > 0;
            _removeRow.Location = new Point(462, 270);
            _removeRow.TabIndex = 16;
        }

        private void ModernSearchTextChanged(object? sender, EventArgs e) => PopulateModernChoices();

        private void ModernChoiceSelectedIndexChanged(object? sender, EventArgs e) => PopulateModernValues();

        private void AddModernCondition_Click(object? sender, EventArgs e) => AddModernCondition();

        private void UpdateModernCondition_Click(object? sender, EventArgs e) => UpdateModernCondition();

        private void RemoveModernCondition_Click(object? sender, EventArgs e) => RemoveModernCondition();

        private void ModernConditionRowsSelectedIndexChanged(object? sender, EventArgs e)
        {
            ConfigureModernUpdateButton();
            if (_conditionRows.SelectedIndices.Count <= 0)
            {
                _addRow.Text = @"Add";
                return;
            }

            var index = _conditionRows.SelectedIndices[0];
            if (index < 0 || index >= AdvancedConditions.Rows.Count)
            {
                return;
            }

            PopulateModernEditorFromRow(AdvancedConditions.Rows[index], index);
        }

        private void PopulateModernTypes()
        {
            _conditionType.Items.Clear();
            foreach (var type in _conditionalTypes)
            {
                _conditionType.Items.Add(type);
            }

            if (_conditionType.Items.Count > 0)
            {
                _conditionType.SelectedIndex = 0;
            }
        }

        private void ModernConditionTypeChanged(object? sender, EventArgs e)
        {
            if (_loadingModernRow)
            {
                return;
            }

            _search.Clear();
            PopulateModernOperators();
            PopulateModernChoices();
            PopulateModernValues();
        }

        private string ModernType => _conditionType.SelectedItem?.ToString() ?? string.Empty;

        private void PopulateModernOperators()
        {
            _operator.Items.Clear();
            var ops = ModernType is "Power Active" or "Power Taken" or "Source Mode" or "Target Entity Type" or "Target Mode" or "Character Archetype" or "Advanced Expression"
                ? new[] { "Is" }
                : _conditionalOps.ToArray();

            _operator.Items.AddRange(ops);
            if (_operator.Items.Count > 0)
            {
                _operator.SelectedIndex = 0;
            }
        }

        private void PopulateModernChoices()
        {
            _choices.BeginUpdate();
            _choices.Items.Clear();
            _choiceLabel.Text = @"Pick";
            _choices.Visible = ModernType != "Advanced Expression";
            _search.Visible = ModernType != "Advanced Expression";
            _expression.Visible = ModernType == "Advanced Expression";
            _operator.Enabled = ModernType != "Advanced Expression";
            _value.Enabled = ModernType != "Advanced Expression";

            var filter = _search.Text.Trim();
            switch (ModernType)
            {
                case "Power Active":
                    AddPowerChoices([6, 7, 8, 9, 10, 11], filter, p => p.PowerType is Enums.ePowerType.Auto_ or Enums.ePowerType.Toggle || p.PowerType == Enums.ePowerType.Click && p.ClickBuff);
                    break;
                case "Power Taken":
                    AddPowerChoices([6, 7, 8, 9, 10, 11], filter, p => p.PowerType is Enums.ePowerType.Auto_ or Enums.ePowerType.Toggle || p.PowerType == Enums.ePowerType.Click && p.ClickBuff);
                    break;
                case "Stacks":
                    AddPowerChoices([6, 8, 9, 10, 11], filter, p => p.VariableEnabled);
                    break;
                case "Team Members":
                    _choiceLabel.Text = @"Archetype";
                    _choices.Items.Add("Any").Name = "Any";
                    foreach (var at in DatabaseAPI.Database.Classes.Where(x => x is { Playable: true }))
                    {
                        _choices.Items.Add(at.DisplayName).Name = at.DisplayName;
                    }
                    break;
                case "Combat Setting":
                    _choiceLabel.Text = @"Setting";
                    foreach (var setting in ConfigData.CombatContext.EnumerateFields(MidsContext.Config.CombatContextSettings))
                    {
                        var display = ConfigData.CombatContext.FormatSettingName(setting);
                        if (string.IsNullOrWhiteSpace(filter) || display.Contains(filter, StringComparison.OrdinalIgnoreCase))
                        {
                            _choices.Items.Add(display).Name = setting;
                        }
                    }
                    break;
                case "Source Mode":
                    AddNamedChoices(
                        Enum.GetValues(typeof(PlannerMode))
                            .Cast<PlannerMode>()
                            .Where(mode => mode != PlannerMode.None)
                            .Select(PlannerModeMapper.ToCanonicalName)
                            .Where(name => !string.IsNullOrWhiteSpace(name))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase));
                    break;
                case "Target Entity Type":
                    AddTargetScopeChoices(_choices);
                    break;
                case "Target Mode":
                    _note.Text = @"Target mode is stored for review, but Mids cannot simulate target mode state.";
                    AddNamedChoices(["kWet", "kLevitated", "kOpportunityLock", "kOpportunitySustain", "kChain_Induction", "kMastermind_Upgrade_1", "kMastermind_Upgrade_2", "kFocusFire_Burst", "kFocusFire_Slug", "kFocusFire_M30"]);
                    break;
                case "Character Archetype":
                    foreach (var at in DatabaseAPI.Database.Classes.Where(x => x is { Playable: true }))
                    {
                        _choices.Items.Add(at.DisplayName).Name = at.ClassName;
                    }
                    break;
                case "Character Level":
                    _choices.Items.Add("Character Level").Name = "char>level";
                    break;
                case "Advanced Expression":
                    _note.Text = @"For Omni expressions that cannot be represented by friendly rows yet.";
                    break;
            }

            if (ModernType != "Target Mode" && ModernType != "Advanced Expression")
            {
                _note.Text = @"Pick a condition type, choose its value, then add it to the list. Advanced rows are saved with the database.";
            }

            _choices.EndUpdate();
            if (_choices.Items.Count > 0)
            {
                _choices.Items[0].Selected = true;
            }
        }

        private void AddNamedChoices(IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                _choices.Items.Add(value).Name = value;
            }
        }

        private void AddPowerChoices(int[] excludedSetTypes, string filter, Func<IPower, bool> predicate)
        {
            foreach (var power in DatabaseAPI.Database.Power.Where(p => p != null))
            {
                if (!TryGetPowerConditionListParts(power, excludedSetTypes, out var parts, out var archetype) || !predicate(power))
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(filter) && !FilterMatch(filter, parts[2], archetype, parts[1]))
                {
                    continue;
                }

                _choices.Items.Add($"{parts[2]} [{archetype} / {parts[1]}]").Name = power.FullName;
            }
        }

        private void PopulateModernValues()
        {
            _value.Items.Clear();
            switch (ModernType)
            {
                case "Power Active":
                case "Power Taken":
                case "Source Mode":
                case "Target Entity Type":
                case "Target Mode":
                case "Character Archetype":
                    _value.Items.AddRange(["True", "False"]);
                    break;
                case "Stacks":
                    var power = _choices.SelectedItems.Count > 0 ? DatabaseAPI.GetPowerByFullName(_choices.SelectedItems[0].Name) : null;
                    if (power != null)
                    {
                        foreach (var stackNum in FloatRange(power.VariableMin, power.VariableMax + 1, 1).Where(x => x >= power.VariableMin && x <= power.VariableMax))
                        {
                            _value.Items.Add($"{stackNum}");
                        }
                    }
                    break;
                case "Team Members":
                    foreach (var num in Enumerable.Range(1, 7))
                    {
                        _value.Items.Add($"{num}");
                    }
                    break;
                case "Combat Setting":
                    foreach (var num in Enumerable.Range(0, 101))
                    {
                        _value.Items.Add($"{num}");
                    }
                    break;
                case "Character Level":
                    foreach (var num in Enumerable.Range(1, 50))
                    {
                        _value.Items.Add($"{num}");
                    }
                    break;
            }

            if (_value.Items.Count > 0)
            {
                _value.SelectedIndex = 0;
            }
        }

        private void AddModernCondition()
        {
            var conditionCount = AdvancedConditions.Rows.Count;
            var link = conditionCount > 0 && string.Equals(_linkType.Text, "OR", StringComparison.OrdinalIgnoreCase)
                ? AdvancedConditionLink.Or
                : AdvancedConditionLink.And;

            var row = BuildModernConditionRow(link);
            if (row == null)
            {
                return;
            }

            AdvancedConditions.Rows.Add(row);
            _conditionRows.SelectedIndices.Clear();
            CommitModernRows();
        }

        private void UpdateModernCondition()
        {
            if (_conditionRows.SelectedIndices.Count <= 0)
            {
                return;
            }

            var index = _conditionRows.SelectedIndices[0];
            if (index < 0 || index >= AdvancedConditions.Rows.Count)
            {
                return;
            }

            var link = index == 0
                ? AdvancedConditionLink.And
                : string.Equals(_linkType.Text, "OR", StringComparison.OrdinalIgnoreCase)
                    ? AdvancedConditionLink.Or
                    : AdvancedConditionLink.And;
            var row = BuildModernConditionRow(link);
            if (row == null)
            {
                return;
            }

            AdvancedConditions.Rows[index] = row;
            CommitModernRows(index);
        }

        private AdvancedConditionRow? BuildModernConditionRow(AdvancedConditionLink link)
        {
            if (ModernType == "Advanced Expression")
            {
                var expression = _expression.Text.Trim();
                if (string.IsNullOrWhiteSpace(expression))
                {
                    return null;
                }

                return AdvancedConditionRow.AdvancedExpression(link, expression, unsupported: true);
            }

            if (_choices.SelectedItems.Count <= 0 || _value.SelectedItem == null)
            {
                return null;
            }

            var choice = _choices.SelectedItems[0];
            var value = _value.Text;
            var op = ParseModernOperator(_operator.Text);

            return ModernType switch
            {
                "Power Active" => new AdvancedConditionRow { Link = link, Kind = AdvancedConditionKind.PowerActive, Subject = choice.Name, Value = value, Operator = AdvancedConditionOperator.Equals },
                "Power Taken" => new AdvancedConditionRow { Link = link, Kind = AdvancedConditionKind.PowerTaken, Subject = choice.Name, Value = value, Operator = AdvancedConditionOperator.Equals },
                "Stacks" => new AdvancedConditionRow { Link = link, Kind = AdvancedConditionKind.PowerStacks, Subject = choice.Name, Value = value, Operator = op },
                "Team Members" => new AdvancedConditionRow { Link = link, Kind = AdvancedConditionKind.TeamMembers, Subject = choice.Name, Value = value, Operator = op },
                "Combat Setting" => new AdvancedConditionRow { Link = link, Kind = AdvancedConditionKind.CombatSetting, Subject = choice.Name, Value = value, Operator = op },
                "Source Mode" when OmniModeMapper.IsKnownBuildSourceMode(choice.Name) => new AdvancedConditionRow { Link = link, Kind = AdvancedConditionKind.SourceMode, Subject = choice.Name, Negated = value == "False" },
                "Source Mode" => AdvancedConditionRow.AdvancedExpression(
                    link,
                    $"source.Mode?({choice.Name})",
                    unsupported: true,
                    evaluationMode: AdvancedConditionEvaluationMode.RuntimeTargetOnly),
                "Target Entity Type" => new AdvancedConditionRow
                {
                    Link = link,
                    Kind = AdvancedConditionKind.TargetEntityType,
                    Value = choice.Name,
                    TargetScope = ParseTargetScope(choice.Name),
                    Operator = value == "False" ? AdvancedConditionOperator.NotEquals : AdvancedConditionOperator.Equals,
                    EvaluationMode = AdvancedConditionEvaluationMode.RuntimeTargetOnly,
                    Unsupported = true
                },
                "Target Mode" => new AdvancedConditionRow
                {
                    Link = link,
                    Kind = AdvancedConditionKind.TargetMode,
                    Subject = choice.Name,
                    Negated = value == "False",
                    Unsupported = true,
                    EvaluationMode = AdvancedConditionEvaluationMode.RuntimeTargetOnly
                },
                "Character Archetype" => new AdvancedConditionRow { Link = link, Kind = AdvancedConditionKind.CharacterArchetype, Value = choice.Name, Operator = value == "False" ? AdvancedConditionOperator.NotEquals : AdvancedConditionOperator.Equals },
                "Character Level" => new AdvancedConditionRow { Link = link, Kind = AdvancedConditionKind.CharacterLevel, Value = value, Operator = op },
                _ => null
            };
        }

        private void RemoveModernCondition()
        {
            if (_conditionRows.SelectedIndices.Count <= 0)
            {
                return;
            }

            var index = _conditionRows.SelectedIndices[0];
            if (index < 0 || index >= AdvancedConditions.Rows.Count)
            {
                return;
            }

            AdvancedConditions.Rows.RemoveAt(index);
            if (AdvancedConditions.Rows.Count > 0)
            {
                AdvancedConditions.Rows[0].Link = AdvancedConditionLink.And;
            }

            CommitModernRows();
        }

        private void CommitModernRows(int selectedIndex = -1)
        {
            SyncLegacyConditionalsFromAdvanced();
            RefreshModernConditionRows();
            if (selectedIndex >= 0 && selectedIndex < _conditionRows.Items.Count)
            {
                _conditionRows.Items[selectedIndex].Selected = true;
                _conditionRows.Items[selectedIndex].EnsureVisible();
            }

            panelLinkType.Visible = AdvancedConditions.Rows.Count > 0;
        }

        private void PopulateModernEditorFromRow(AdvancedConditionRow row, int index)
        {
            _loadingModernRow = true;
            try
            {
                var type = GetModernTypeForRow(row);
                SelectComboText(_conditionType, type);
                _search.Clear();
                PopulateModernOperators();
                PopulateModernChoices();
                PopulateModernValues();

                if (index > 0)
                {
                    SelectComboText(_linkType, row.Link == AdvancedConditionLink.Or ? "OR" : "AND");
                }
                else
                {
                    SelectComboText(_linkType, "AND");
                }

                SelectComboText(_operator, GetModernOperatorText(row.Operator));

                if (type == "Advanced Expression")
                {
                    _expression.Text = string.IsNullOrWhiteSpace(row.RawExpression) ? row.Value : row.RawExpression;
                }
                else
                {
                    SelectModernChoiceForRow(row);
                    PopulateModernValues();
                    SelectComboText(_value, GetModernEditorValue(row));
                }

                _addRow.Text = @"Add New";
                _builderHint.Text = @"Edit the selected condition, then click Update Selected. Add New keeps the existing row and adds another.";
            }
            finally
            {
                _loadingModernRow = false;
            }
        }

        private static string GetModernTypeForRow(AdvancedConditionRow row)
        {
            return row.Kind switch
            {
                AdvancedConditionKind.PowerActive => "Power Active",
                AdvancedConditionKind.PowerTaken => "Power Taken",
                AdvancedConditionKind.PowerStacks => "Stacks",
                AdvancedConditionKind.TeamMembers => "Team Members",
                AdvancedConditionKind.CombatSetting => "Combat Setting",
                AdvancedConditionKind.SourceMode => "Source Mode",
                AdvancedConditionKind.TargetEntityType => "Target Entity Type",
                AdvancedConditionKind.TargetMode => "Target Mode",
                AdvancedConditionKind.CharacterArchetype => "Character Archetype",
                AdvancedConditionKind.CharacterLevel => "Character Level",
                _ => "Advanced Expression"
            };
        }

        private void SelectModernChoiceForRow(AdvancedConditionRow row)
        {
            var choiceName = row.Kind switch
            {
                AdvancedConditionKind.TargetEntityType when row.TargetScope != AdvancedConditionTargetScope.Unknown => FormatTargetScope(row.TargetScope),
                AdvancedConditionKind.TargetEntityType => row.Value,
                AdvancedConditionKind.CharacterArchetype => row.Value,
                AdvancedConditionKind.CharacterLevel => "char>level",
                _ => row.Subject
            };

            if (string.IsNullOrWhiteSpace(choiceName))
            {
                return;
            }

            SelectListViewItemByName(_choices, choiceName);
        }

        private static string GetModernEditorValue(AdvancedConditionRow row)
        {
            return row.Kind switch
            {
                AdvancedConditionKind.SourceMode or AdvancedConditionKind.TargetMode => row.Negated ? "False" : "True",
                AdvancedConditionKind.TargetEntityType => row.Operator == AdvancedConditionOperator.NotEquals ? "False" : "True",
                AdvancedConditionKind.CharacterArchetype => row.Operator == AdvancedConditionOperator.NotEquals ? "False" : "True",
                _ => row.Value
            };
        }

        private static string GetModernOperatorText(AdvancedConditionOperator op)
        {
            return op switch
            {
                AdvancedConditionOperator.GreaterThan => "Greater Than",
                AdvancedConditionOperator.LessThan => "Less Than",
                _ => "Equal To"
            };
        }

        private static void SelectComboText(ComboBox combo, string value)
        {
            for (var i = 0; i < combo.Items.Count; i++)
            {
                if (string.Equals(combo.Items[i]?.ToString(), value, StringComparison.OrdinalIgnoreCase))
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        private static void SelectListViewItemByName(ListView listView, string name)
        {
            foreach (ListViewItem item in listView.Items)
            {
                if (!string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                item.Selected = true;
                item.EnsureVisible();
                return;
            }

            var added = listView.Items.Add(name);
            added.Name = name;
            added.Selected = true;
            added.EnsureVisible();
        }

        private static AdvancedConditionOperator ParseModernOperator(string op)
        {
            return op switch
            {
                "Greater Than" => AdvancedConditionOperator.GreaterThan,
                "Less Than" => AdvancedConditionOperator.LessThan,
                _ => AdvancedConditionOperator.Equals
            };
        }

        private void RefreshModernConditionRows()
        {
            _conditionRows.BeginUpdate();
            _conditionRows.Items.Clear();
            for (var i = 0; i < AdvancedConditions.Rows.Count; i++)
            {
                var row = AdvancedConditions.Rows[i];
                var item = new ListViewItem(i == 0 ? "" : row.Link == AdvancedConditionLink.Or ? "OR" : "AND");
                item.SubItems.Add(GetAdvancedConditionDisplay(row));
                item.SubItems.Add(AdvancedConditionSet.FormatOperator(row.Operator));
                item.SubItems.Add(GetAdvancedConditionValue(row));
                _conditionRows.Items.Add(item);
            }

            _conditionRows.EndUpdate();
            RefreshConditionRows();
        }

        private void lvConditionalType_SelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (lvConditionalType.SelectedItems.Count <= 0)
            {
                return;
            }

            _advancedExpressionText.Visible = false;
            lvSubConditional.Visible = true;
            _builderHint.Text = @"Choose a condition, pick a value, then add it to the list.";

            var lvBoolSizeStandAlone = new Size(112, 259);
            var lvBoolLocStandAlone = new Point(537, 16);

            var lvBoolSizeSecondary = new Size(112, 170);
            var lvBoolLocSecondary = new Point(537, 117);

            if (lvConditionalType.SelectedItems.Count == 0)
            {
                lvSubConditional.BeginUpdate();
                lvSubConditional.Items.Clear();
                lvSubConditional.EndUpdate();

                lvConditionalBool.BeginUpdate();
                lvConditionalBool.Items.Clear();
                lvConditionalBool.EndUpdate();

                return;
            }

            switch (e.Item?.Text)
            {
                case "Power Active":
                    tbFilter.Visible = true;
                    btnClearFilter.Visible = true;
                    lvConditionalBool.Enabled = true;
                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    var pArray = DatabaseAPI.Database.Power.Where(p => p != null).ToArray();
                    var eArray = new[] { 6, 7, 8, 9, 10, 11 };
                    foreach (var power in pArray)
                    {
                        if (!TryGetPowerConditionListParts(power, eArray, out var pStrings, out var pArchetype))
                        {
                            continue;
                        }

                        var pType = power?.PowerType;
                        var isType = pType is Enums.ePowerType.Auto_ or Enums.ePowerType.Toggle ||
                                     pType == Enums.ePowerType.Click && power?.ClickBuff == true;
                        if (!isType)
                        {
                            continue;
                        }

                        var textFilter = tbFilter.Text.Trim();
                        if (!string.IsNullOrEmpty(textFilter))
                        {
                            if (!pStrings[2].ToLowerInvariant().Contains(textFilter.ToLowerInvariant()))
                            {
                                continue;
                            }
                        }

                        lvSubConditional.Items.Add($"{pStrings[2]} [{pArchetype} / {pStrings[1]}]").Name = power.FullName;
                    }

                    lvConditionalBool.Size = lvBoolSizeStandAlone;
                    lvConditionalBool.Location = lvBoolLocStandAlone;
                    lvConditionalOp.Visible = false;
                    lvSubConditional.Columns[0].Text = @"Power Name [Class / Powerset]";
                    lvSubConditional.EndUpdate();
                    break;

                case "Power Taken":
                    tbFilter.Visible = true;
                    btnClearFilter.Visible = true;
                    lvConditionalBool.Enabled = true;
                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    pArray = DatabaseAPI.Database.Power.Where(p => p != null).ToArray();
                    eArray = [6, 7, 8, 9, 10, 11];
                    foreach (var power in pArray)
                    {
                        if (!TryGetPowerConditionListParts(power, eArray, out var pStrings, out var pArchetype))
                        {
                            continue;
                        }

                        var pType = power?.PowerType;
                        var isType = pType == Enums.ePowerType.Auto_ || pType == Enums.ePowerType.Toggle ||
                                     (pType == Enums.ePowerType.Click && power?.ClickBuff == true);
                        if (!isType)
                        {
                            continue;
                        }

                        var textFilter = tbFilter.Text.Trim();
                        if (!string.IsNullOrEmpty(textFilter))
                        {
                            if (!pStrings[2].ToLowerInvariant().Contains(textFilter.ToLowerInvariant()))
                            {
                                continue;
                            }
                        }

                        lvSubConditional.Items.Add($"{pStrings[2]} [{pArchetype} / {pStrings[1]}]").Name = power.FullName;
                    }

                    lvConditionalBool.Size = lvBoolSizeStandAlone;
                    lvConditionalBool.Location = lvBoolLocStandAlone;
                    lvConditionalOp.Visible = false;
                    lvSubConditional.Columns[0].Text = @"Power Name [Class / Powerset]";
                    lvSubConditional.EndUpdate();
                    break;

                case "Stacks":
                    tbFilter.Visible = true;
                    btnClearFilter.Visible = true;
                    lvConditionalBool.Enabled = true;
                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    pArray = DatabaseAPI.Database.Power.Where(p => p != null).ToArray();
                    eArray = [6, 8, 9, 10, 11];
                    foreach (var power in pArray)
                    {
                        if (!TryGetPowerConditionListParts(power, eArray, out var pStrings, out var pArchetype))
                        {
                            continue;
                        }

                        var isType = power.VariableEnabled;
                        if (!isType) continue;

                        lvConditionalBool.Size = lvBoolSizeSecondary;
                        lvConditionalBool.Location = lvBoolLocSecondary;
                        lvConditionalOp.Visible = true;
                        lvSubConditional.Items.Add($"{pStrings[2]} [{pArchetype} / {pStrings[1]}]").Name = power.FullName;
                    }

                    lvConditionalOp.Columns[0].Text = @"Stacks are?";
                    lvConditionalBool.Columns[0].Text = @"# of Stacks";
                    lvSubConditional.Columns[0].Text = @"Power Name [Class / Powerset]";
                    lvSubConditional.EndUpdate();
                    break;

                case "Team Members":
                    tbFilter.Visible = false;
                    btnClearFilter.Visible = false;
                    lvConditionalBool.Size = lvBoolSizeSecondary;
                    lvConditionalBool.Location = lvBoolLocSecondary;
                    lvConditionalOp.Visible = true;
                    lvConditionalBool.Visible = true;
                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    var teamATs = new List<string>
                    {
                        "Any",
                    };
                    var playableClasses = DatabaseAPI.Database.Classes
                        .Where(x => x is { Playable: true })
                        .Select(x => x.DisplayName)
                        .ToList();
                    teamATs = teamATs
                        .Concat(playableClasses)
                        .ToList();

                    foreach (var member in teamATs)
                    {
                        lvSubConditional.Items.Add(member);
                    }

                    lvConditionalOp.Columns[0].Text = @"Members are?";
                    lvSubConditional.Columns[0].Text = @"Team Members";
                    lvSubConditional.EndUpdate();
                    break;

                case "Combat Setting":
                    var cfgSettings = ConfigData.CombatContext.EnumerateFields(MidsContext.Config.CombatContextSettings);

                    lvSubConditional.Columns[0].Text = @"Combat Context Variable";

                    lvConditionalOp.Columns[0].Text = @"Value is?";
                    lvConditionalOp.Visible = true;

                    lvConditionalBool.Columns[0].Text = @"Value";
                    lvConditionalBool.Visible = true;
                    
                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    foreach (var setting in cfgSettings)
                    {
                        lvSubConditional.Items.Add(ConfigData.CombatContext.FormatSettingName(setting)).Name = setting;
                    }
                    lvSubConditional.EndUpdate();

                    break;

                case "Source Mode":
                    tbFilter.Visible = false;
                    btnClearFilter.Visible = false;
                    lvConditionalOp.Visible = false;
                    lvConditionalBool.Visible = true;
                    lvConditionalBool.Enabled = true;
                    lvSubConditional.Columns[0].Text = @"Source Mode";
                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    foreach (var mode in new[]
                             {
                                 "kDefensiveAdaptation",
                                 "kEfficientAdaptation",
                                 "kOffensiveAdaptation",
                                 "kDomination",
                                 "kScourge",
                                 "kContainment",
                                 "kCriticalHit",
                                 "kAssassination"
                             })
                    {
                        lvSubConditional.Items.Add(mode).Name = mode;
                    }
                    lvSubConditional.EndUpdate();
                    break;

                case "Target Entity Type":
                    tbFilter.Visible = false;
                    btnClearFilter.Visible = false;
                    lvConditionalOp.Visible = false;
                    lvConditionalBool.Visible = true;
                    lvConditionalBool.Enabled = true;
                    lvSubConditional.Columns[0].Text = @"Target";
                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    AddTargetScopeChoices(lvSubConditional);
                    lvSubConditional.EndUpdate();
                    break;

                case "Target Mode":
                    tbFilter.Visible = false;
                    btnClearFilter.Visible = false;
                    lvConditionalOp.Visible = false;
                    lvConditionalBool.Visible = true;
                    lvConditionalBool.Enabled = true;
                    lvSubConditional.Columns[0].Text = @"Target Mode";
                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    foreach (var mode in new[]
                             {
                                 "kWet",
                                 "kLevitated",
                                 "kOpportunityLock",
                                 "kOpportunitySustain",
                                 "kChain_Induction",
                                 "kMastermind_Upgrade_1",
                                 "kMastermind_Upgrade_2",
                                 "kFocusFire_Burst",
                                 "kFocusFire_Slug",
                                 "kFocusFire_M30"
                             })
                    {
                        lvSubConditional.Items.Add(mode).Name = mode;
                    }
                    lvSubConditional.EndUpdate();
                    break;

                case "Character Archetype":
                    tbFilter.Visible = false;
                    btnClearFilter.Visible = false;
                    lvConditionalOp.Visible = false;
                    lvConditionalBool.Visible = true;
                    lvConditionalBool.Enabled = true;
                    lvSubConditional.Columns[0].Text = @"Archetype";
                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    foreach (var archetype in DatabaseAPI.Database.Classes.Where(x => x is { Playable: true }))
                    {
                        lvSubConditional.Items.Add(archetype.DisplayName).Name = archetype.ClassName;
                    }
                    lvSubConditional.EndUpdate();
                    break;

                case "Character Level":
                    tbFilter.Visible = false;
                    btnClearFilter.Visible = false;
                    lvConditionalOp.Visible = true;
                    lvConditionalBool.Visible = true;
                    lvConditionalBool.Enabled = true;
                    lvConditionalOp.Columns[0].Text = @"Level is?";
                    lvSubConditional.Columns[0].Text = @"Character";
                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    lvSubConditional.Items.Add("Character Level").Name = "char>level";
                    lvSubConditional.EndUpdate();
                    break;
                
                case "Advanced Expression":
                    tbFilter.Visible = false;
                    btnClearFilter.Visible = false;
                    lvConditionalOp.Visible = false;
                    lvConditionalBool.Visible = false;
                    lvSubConditional.Visible = false;
                    _advancedExpressionText.Visible = true;
                    _advancedExpressionText.Focus();
                    _builderHint.Text = @"For expressions that cannot be represented yet. Use sparingly.";
                    break;
            }
        }

        private void lvSubConditional_SelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var powName = lvSubConditional.SelectedItems.Count > 0
                ? lvSubConditional.SelectedItems[0].Name
                : string.Empty;

            var selected = DatabaseAPI.GetPowerByFullName(powName);

            if (lvConditionalType.SelectedItems.Count <= 0)
            {
                return;
            }

            lvConditionalBool.Items.Clear();
            switch (lvConditionalType.SelectedItems[0].Text)
            {
                case "Power Active":
                    lvConditionalBool.BeginUpdate();
                    lvConditionalBool.Items.Add("True");
                    lvConditionalBool.Items.Add("False");
                    lvConditionalBool.Columns[0].Text = @"Power Active?";
                    lvConditionalBool.EndUpdate();

                    lvConditionalBool.Visible = selected != null;

                    break;
                
                case "Power Taken":
                    lvConditionalBool.BeginUpdate();
                    lvConditionalBool.Items.Add("True");
                    lvConditionalBool.Items.Add("False");
                    lvConditionalBool.Columns[0].Text = @"Power Taken?";
                    lvConditionalBool.EndUpdate();

                    break;
                
                case "Stacks":
                    lvConditionalBool.BeginUpdate();
                    if (selected != null)
                    {
                        var stackRange = FloatRange(selected.VariableMin, selected.VariableMax + 1, 1);
                        foreach (var stackNum in stackRange)
                        {
                            if (stackNum < selected.VariableMin)
                            {
                                continue;
                            }

                            if (stackNum > selected.VariableMax)
                            {
                                break;
                            }

                            lvConditionalBool.Items.Add($"{stackNum}");
                        }
                    }

                    lvConditionalBool.Columns[0].Text = @"# of Stacks?";
                    lvConditionalBool.EndUpdate();

                    lvConditionalBool.Visible = selected != null;

                    break;
                
                case "Team Members":
                    var tRange = Enumerable.Range(1, 7);
                    lvConditionalBool.BeginUpdate();
                    lvConditionalBool.Items.Clear();
                    foreach (var num in tRange)
                    {
                        lvConditionalBool.Items.Add($"{num}");
                    }

                    lvConditionalBool.Columns[0].Text = @"# of Members";
                    lvConditionalBool.EndUpdate();

                    lvConditionalBool.Visible = true;

                    break;

                case "Combat Setting":
                    if (lvConditionalType.SelectedItems.Count > 0 && lvSubConditional.SelectedItems.Count > 0)
                    {
                        var selectedItem = lvSubConditional.SelectedItems[0].Text.ToLowerInvariant();
                        lvConditionalBool.BeginUpdate();
                        lvConditionalBool.Items.Clear();
                        if (selectedItem.Contains("isalive"))
                        {
                            // Type bool
                            lvConditionalBool.Items.Add("True");
                            lvConditionalBool.Items.Add("False");

                            lvConditionalOp.BeginUpdate();
                            lvConditionalOp.Items.Clear();
                            lvConditionalOp.Items.Add("Equal To");
                            lvConditionalOp.EndUpdate();
                        }
                        else
                        {
                            // Type int in [0; 100]
                            var range = Enumerable.Range(0, 101);
                            foreach (var num in range)
                            {
                                lvConditionalBool.Items.Add($"{num}");
                            }

                            lvConditionalBool.Columns[0].Text = @"Value";

                            lvConditionalOp.BeginUpdate();
                            lvConditionalOp.Items.Clear();
                            foreach (var op in _conditionalOps)
                            {
                                lvConditionalOp.Items.Add(op);
                            }
                            lvConditionalOp.EndUpdate();
                        }

                        lvConditionalBool.EndUpdate();

                        lvConditionalBool.Visible = true;
                    }

                    break;

                case "Source Mode":
                case "Target Entity Type":
                case "Target Mode":
                case "Character Archetype":
                    lvConditionalBool.BeginUpdate();
                    lvConditionalBool.Items.Clear();
                    lvConditionalBool.Items.Add("True");
                    lvConditionalBool.Items.Add("False");
                    lvConditionalBool.Columns[0].Text = @"Condition is?";
                    lvConditionalBool.EndUpdate();
                    lvConditionalBool.Visible = true;
                    break;

                case "Character Level":
                    lvConditionalOp.BeginUpdate();
                    lvConditionalOp.Items.Clear();
                    foreach (var op in _conditionalOps)
                    {
                        lvConditionalOp.Items.Add(op);
                    }
                    lvConditionalOp.EndUpdate();

                    lvConditionalBool.BeginUpdate();
                    lvConditionalBool.Items.Clear();
                    foreach (var num in Enumerable.Range(1, 50))
                    {
                        lvConditionalBool.Items.Add($"{num}");
                    }
                    lvConditionalBool.Columns[0].Text = @"Level";
                    lvConditionalBool.EndUpdate();
                    lvConditionalBool.Visible = true;
                    break;
            }
        }

        private void lvSubConditional_MouseClick(object sender, MouseEventArgs e)
        {
            if (lvSubConditional.Items.Count <= 0) return;
            if (e.Button != MouseButtons.Right) return;

            var conditionalType = lvConditionalType.SelectedItems.Count <= 0
                ? ""
                : lvConditionalType.Items[lvConditionalType.SelectedItems[0].Index].Text;

            if (conditionalType != "Power Taken" & conditionalType != "Power Active") return;

            using var sf = new frmConditionalAttributeSearch();
            var ret = sf.ShowDialog();
            if (ret == DialogResult.Cancel) return;
            if (sf.SearchTerms.PowerName == "") return;

            var searchAtGroup = sf.SearchTerms.AtGroup switch
            {
                "Any" => "",
                "None" => "",
                _ => sf.SearchTerms.AtGroup.ToLowerInvariant()
            };

            var searchPowerName = sf.SearchTerms.PowerName.ToLowerInvariant();
            var n = lvSubConditional.Items.Count;

            for (var i = 0; i < n; i++)
            {
                var lvItem = lvSubConditional.Items[i].Text.ToLowerInvariant();
                if (!lvItem.StartsWith(searchPowerName))
                {
                    continue;
                }

                if (!(searchAtGroup == "" | lvItem.Contains($"[{searchAtGroup}")))
                {
                    continue;
                }

                lvSubConditional.Items[i].Selected = true;
                lvSubConditional.Items[i].EnsureVisible();

                return;
            }

            MessageBox.Show(
                $@"No match found for '{sf.SearchTerms.PowerName}'{(searchAtGroup == "" ? "" : $" in AT/group {sf.SearchTerms.AtGroup}")}",
                @"Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void addConditional_Click(object sender, EventArgs e)
        {
            string powerName;
            var cOp = string.Empty;
            IPower power;
            string value;
            ListViewItem item;
            var conditionCount = AdvancedConditions.Rows.Count;
            var linkPrefix = conditionCount > 0 && rbLinkTypeOr.Checked ? "OR " : "";
            var linkPrefixLv = conditionCount > 0
                ? rbLinkTypeOr.Checked
                    ? "OR "
                    : "AND "
                : "";
            var advancedLink = conditionCount > 0 && rbLinkTypeOr.Checked ? AdvancedConditionLink.Or : AdvancedConditionLink.And;

            if (lvConditionalType.SelectedItems.Count <= 0)
            {
                return;
            }

            switch (lvConditionalType.SelectedItems[0].Text)
            {
                case "Power Active":
                    if (lvSubConditional.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    if (lvConditionalBool.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    powerName = lvSubConditional.SelectedItems[0].Name;
                    power = DatabaseAPI.GetPowerByFullName(powerName);
                    value = lvConditionalBool.SelectedItems[0].Text;
                    item = new ListViewItem { Text = linkPrefixLv, Name = power?.FullName };
                    item.SubItems.Add($@"Active:{power?.DisplayName}");
                    item.SubItems.Add("");
                    item.SubItems.Add(value);
                    lvActiveConditionals.Items.Add(item);
                    break;
                
                case "Power Taken":
                    if (lvSubConditional.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    if (lvConditionalBool.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    powerName = lvSubConditional.SelectedItems[0].Name;
                    power = DatabaseAPI.GetPowerByFullName(powerName);
                    value = lvConditionalBool.SelectedItems[0].Text;
                    item = new ListViewItem { Text = linkPrefixLv, Name = power?.FullName };
                    item.SubItems.Add($@"Taken:{power?.DisplayName}");
                    item.SubItems.Add("");
                    item.SubItems.Add(value);
                    lvActiveConditionals.Items.Add(item);
                    break;
                
                case "Stacks":
                    if (lvSubConditional.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    if (lvConditionalOp.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    if (lvConditionalBool.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    powerName = lvSubConditional.SelectedItems[0].Name;
                    power = DatabaseAPI.GetPowerByFullName(powerName);
                    cOp = lvConditionalOp.SelectedItems[0].Text switch
                    {
                        "Equal To" => "=",
                        "Greater Than" => ">",
                        "Less Than" => "<",
                        _ => cOp
                    };
                    value = lvConditionalBool.SelectedItems[0].Text;
                    item = new ListViewItem { Text = linkPrefixLv, Name = power?.FullName };
                    item.SubItems.Add($@"Stacks:{power?.DisplayName}");
                    item.SubItems.Add(cOp);
                    item.SubItems.Add(value);
                    lvActiveConditionals.Items.Add(item);
                    break;
                
                case "Team Members":
                    if (lvSubConditional.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    if (lvConditionalBool.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    var archetype = lvSubConditional.SelectedItems[0].Text;
                    cOp = lvConditionalOp.SelectedItems[0].Text switch
                    {
                        "Equal To" => "=",
                        "Greater Than" => ">",
                        "Less Than" => "<",
                        _ => cOp
                    };

                    value = lvConditionalBool.SelectedItems[0].Text;
                    item = new ListViewItem { Text = linkPrefixLv, Name = archetype };
                    item.SubItems.Add($@"Team:{archetype}");
                    item.SubItems.Add(cOp);
                    item.SubItems.Add(value);
                    lvActiveConditionals.Items.Add(item);
                    break;

                case "Combat Setting":
                    if (lvSubConditional.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    if (lvConditionalOp.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    if (lvConditionalBool.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    cOp = lvConditionalOp.SelectedItems[0].Text switch
                    {
                        "Equal To" => "=",
                        "Greater Than" => ">",
                        "Less Than" => "<",
                        _ => cOp
                    };

                    var field = lvSubConditional.SelectedItems[0].Text;
                    value = lvConditionalBool.SelectedItems[0].Text;
                    item = new ListViewItem { Text = linkPrefixLv, Name = field };
                    item.SubItems.Add(@$"Config:{field}");
                    item.SubItems.Add(cOp);
                    item.SubItems.Add(value);
                    lvActiveConditionals.Items.Add(item);

                    break;

                case "Source Mode":
                    if (lvSubConditional.SelectedItems.Count <= 0 || lvConditionalBool.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    value = lvConditionalBool.SelectedItems[0].Text;
                    var sourceModeRow = new AdvancedConditionRow
                    {
                        Link = advancedLink,
                        Kind = AdvancedConditionKind.SourceMode,
                        Subject = lvSubConditional.SelectedItems[0].Name,
                        Negated = value == "False"
                    };
                    AdvancedConditions.Rows.Add(sourceModeRow);
                    item = BuildAdvancedConditionItem(sourceModeRow, conditionCount);
                    lvActiveConditionals.Items.Add(item);
                    break;

                case "Target Entity Type":
                    if (lvSubConditional.SelectedItems.Count <= 0 || lvConditionalBool.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    value = lvConditionalBool.SelectedItems[0].Text;
                    var targetEntityRow = new AdvancedConditionRow
                    {
                        Link = advancedLink,
                        Kind = AdvancedConditionKind.TargetEntityType,
                        Value = lvSubConditional.SelectedItems[0].Name,
                        TargetScope = ParseTargetScope(lvSubConditional.SelectedItems[0].Name),
                        Operator = value == "False" ? AdvancedConditionOperator.NotEquals : AdvancedConditionOperator.Equals,
                        EvaluationMode = AdvancedConditionEvaluationMode.RuntimeTargetOnly,
                        Unsupported = true
                    };
                    AdvancedConditions.Rows.Add(targetEntityRow);
                    item = BuildAdvancedConditionItem(targetEntityRow, conditionCount);
                    lvActiveConditionals.Items.Add(item);
                    break;

                case "Target Mode":
                    if (lvSubConditional.SelectedItems.Count <= 0 || lvConditionalBool.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    value = lvConditionalBool.SelectedItems[0].Text;
                    var targetModeRow = new AdvancedConditionRow
                    {
                        Link = advancedLink,
                        Kind = AdvancedConditionKind.TargetMode,
                        Subject = lvSubConditional.SelectedItems[0].Name,
                        Negated = value == "False",
                        Unsupported = true
                    };
                    AdvancedConditions.Rows.Add(targetModeRow);
                    item = BuildAdvancedConditionItem(targetModeRow, conditionCount);
                    lvActiveConditionals.Items.Add(item);
                    break;

                case "Character Archetype":
                    if (lvSubConditional.SelectedItems.Count <= 0 || lvConditionalBool.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    value = lvConditionalBool.SelectedItems[0].Text;
                    var archetypeRow = new AdvancedConditionRow
                    {
                        Link = advancedLink,
                        Kind = AdvancedConditionKind.CharacterArchetype,
                        Value = lvSubConditional.SelectedItems[0].Name,
                        Operator = value == "False" ? AdvancedConditionOperator.NotEquals : AdvancedConditionOperator.Equals
                    };
                    AdvancedConditions.Rows.Add(archetypeRow);
                    item = BuildAdvancedConditionItem(archetypeRow, conditionCount);
                    lvActiveConditionals.Items.Add(item);
                    break;

                case "Character Level":
                    if (lvConditionalOp.SelectedItems.Count <= 0 || lvConditionalBool.SelectedItems.Count <= 0)
                    {
                        return;
                    }

                    cOp = lvConditionalOp.SelectedItems[0].Text switch
                    {
                        "Equal To" => "=",
                        "Greater Than" => ">",
                        "Less Than" => "<",
                        _ => cOp
                    };

                    var levelRow = new AdvancedConditionRow
                    {
                        Link = advancedLink,
                        Kind = AdvancedConditionKind.CharacterLevel,
                        Operator = cOp switch
                        {
                            ">" => AdvancedConditionOperator.GreaterThan,
                            "<" => AdvancedConditionOperator.LessThan,
                            _ => AdvancedConditionOperator.Equals
                        },
                        Value = lvConditionalBool.SelectedItems[0].Text
                    };
                    AdvancedConditions.Rows.Add(levelRow);
                    item = BuildAdvancedConditionItem(levelRow, conditionCount);
                    lvActiveConditionals.Items.Add(item);
                    break;

                case "Advanced Expression":
                    var expression = _advancedExpressionText.Text.Trim();
                    if (string.IsNullOrWhiteSpace(expression))
                    {
                        return;
                    }

                    var advancedExpressionRow = AdvancedConditionRow.AdvancedExpression(advancedLink, expression, unsupported: true);
                    AdvancedConditions.Rows.Add(advancedExpressionRow);
                    _advancedExpressionText.Clear();
                    break;
            }

            SyncLegacyConditionalsFromAdvanced();
            RefreshConditionRows();
            panelLinkType.Visible = AdvancedConditions.Rows.Count > 0;
            rbLinkTypeAnd.Checked = true;
        }

        private void removeConditional_Click(object sender, EventArgs e)
        {
            if (lvActiveConditionals.SelectedItems.Count <= 0)
            {
                return;
            }

            var selectedName = lvActiveConditionals.SelectedItems[0].Name;
            var selectedCondition = lvActiveConditionals.SelectedItems[0].SubItems.Count > 1
                ? lvActiveConditionals.SelectedItems[0].SubItems[1].Text
                : "";
            AdvancedConditions.Rows.RemoveAll(row =>
                (!string.IsNullOrWhiteSpace(selectedName) &&
                 string.Equals(row.Subject, selectedName, StringComparison.OrdinalIgnoreCase)) ||
                string.Equals(GetAdvancedConditionDisplay(row), selectedCondition, StringComparison.OrdinalIgnoreCase));

            lvActiveConditionals.SelectedItems[0].Remove();

            SyncLegacyConditionalsFromAdvanced();
            RefreshConditionRows();
            panelLinkType.Visible = AdvancedConditions.Rows.Count > 0;
            rbLinkTypeAnd.Checked = true;
        }

        private void ListView_Leave(object? sender, EventArgs e)
        {
            if (sender is ctlListViewColored { FocusedItem: not null } lvControl)
            {
                lvControl.LostFocusItem = lvControl.FocusedItem.Index;
            }
        }

        private void ListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            e.DrawBackground();
        }

        private void ListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            var lvControl = (ctlListViewColored)sender;
            if (lvControl.Enabled)
            {
                if (e.Item.Selected)
                {
                    if (lvControl.LostFocusItem == e.Item.Index)
                    {
                        e.Item.BackColor = Color.Goldenrod;
                        e.Item.ForeColor = Color.Black;
                        lvControl.LostFocusItem = -1;
                    }
                    else if (lvControl.Focused)
                    {
                        e.Item.ForeColor = SystemColors.HighlightText;
                        e.Item.BackColor = SystemColors.Highlight;
                    }
                }
                else
                {
                    e.Item.BackColor = lvControl.BackColor;
                    e.Item.ForeColor = lvControl.ForeColor;
                }
            }
            else
            {
                e.Item.ForeColor = SystemColors.GrayText;
            }

            e.DrawBackground();
            e.DrawText();
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            SyncLegacyConditionalsFromAdvanced();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void SyncLegacyConditionalsFromAdvanced()
        {
            Conditionals.Clear();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x84:
                    base.WndProc(ref m);
                    if ((int)m.Result == 0x1)
                    {
                        m.Result = 0x2;
                    }

                    return;
            }

            base.WndProc(ref m);
        }

        private void rbLinkTypeAnd_CheckedChanged(object sender, EventArgs e)
        {
            rbLinkTypeOr.Checked = !rbLinkTypeAnd.Checked;
        }

        private void rbLinkTypeOr_CheckedChanged(object sender, EventArgs e)
        {
            rbLinkTypeAnd.Checked = !rbLinkTypeOr.Checked;
        }

        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            tbFilter.Text = string.Empty;
        }

        private bool FilterMatch(string textFilter, string shortName, string archetype, string powerset)
        {
            if (!textFilter.Contains(','))
            {
                return shortName.ToLowerInvariant().Contains(textFilter.ToLowerInvariant());
            }

            var chunks = textFilter.Split(',');
            for (var i = 0; i < chunks.Length; i++)
            {
                chunks[i] = chunks[i].Trim();
            }

            var validName = shortName.ToLowerInvariant().Contains(chunks[0].ToLowerInvariant());
            var validArchetype = chunks.Length <= 1 || string.IsNullOrEmpty(chunks[1]) || archetype.ToLowerInvariant().Contains(chunks[1].ToLowerInvariant());
            var validPowerset = chunks.Length <= 2 || string.IsNullOrEmpty(chunks[2]) || powerset.ToLowerInvariant().Contains(chunks[2].ToLowerInvariant());

            return validName & validArchetype & validPowerset;
        }

        private static bool TryGetPowerConditionListParts(IPower? power, int[] excludedSetTypes, out string[] powerNameParts, out string archetype)
        {
            powerNameParts = [];
            archetype = string.Empty;

            if (power == null || string.IsNullOrWhiteSpace(power.FullName))
            {
                return false;
            }

            var powerset = power.GetPowerSet();
            if (powerset == null || excludedSetTypes.Contains((int)powerset.SetType))
            {
                return false;
            }

            powerNameParts = new Regex("[_]").Replace(power.FullName, " ").Split('.');
            if (powerNameParts.Length < 3)
            {
                return false;
            }

            archetype = new Regex("[ ].*").Replace(powerNameParts[0], "");
            return true;
        }

        private void tbFilter_TextChanged(object sender, EventArgs e)
        {
            var conditionalType = lvConditionalType.SelectedItems.Count <= 0
                ? ""
                : lvConditionalType.SelectedItems[0].Text;

            var pArray = DatabaseAPI.Database.Power.Where(p => p != null).ToArray();
            var eArray = Array.Empty<int>();
            switch (conditionalType)
            {
                case "Power Active":
                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    eArray = [6, 7, 8, 9, 10, 11];

                    foreach (var power in pArray)
                    {
                        if (!TryGetPowerConditionListParts(power, eArray, out var pStrings, out var pArchetype))
                        {
                            continue;
                        }

                        var pType = power?.PowerType;
                        var isType = pType is Enums.ePowerType.Auto_ or Enums.ePowerType.Toggle ||
                                     pType == Enums.ePowerType.Click && power?.ClickBuff == true;
                        if (!isType)
                        {
                            continue;
                        }

                        var textFilter = tbFilter.Text.Trim();
                        if (!string.IsNullOrEmpty(textFilter))
                        {
                            if (!FilterMatch(textFilter, pStrings[2], pArchetype, pStrings[1]))
                            {
                                continue;
                            }
                        }

                        lvSubConditional.Items.Add($"{pStrings[2]} [{pArchetype} / {pStrings[1]}]").Name = power.FullName;
                    }

                    lvSubConditional.Columns[0].Text = @"Power Name [Class / Powerset]";
                    lvSubConditional.EndUpdate();

                    break;

                case "Power Taken":
                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    
                    eArray = [6, 7, 8, 9, 10, 11];
                    foreach (var power in pArray)
                    {
                        if (!TryGetPowerConditionListParts(power, eArray, out var pStrings, out var pArchetype))
                        {
                            continue;
                        }

                        var pType = power?.PowerType;
                        var isType = pType is Enums.ePowerType.Auto_ or Enums.ePowerType.Toggle ||
                                     pType == Enums.ePowerType.Click && power?.ClickBuff == true;
                        if (!isType)
                        {
                            continue;
                        }

                        var textFilter = tbFilter.Text.Trim();
                        if (!string.IsNullOrEmpty(textFilter))
                        {
                            if (!FilterMatch(textFilter, pStrings[2], pArchetype, pStrings[1]))
                            {
                                continue;
                            }
                        }

                        lvSubConditional.Items.Add($"{pStrings[2]} [{pArchetype} / {pStrings[1]}]").Name = power.FullName;
                    }

                    lvSubConditional.Columns[0].Text = @"Power Name [Class / Powerset]";
                    lvSubConditional.EndUpdate();

                    break;

                case "Stacks":
                    /*
                     tbFilter.Visible = true;
                       btnClearFilter.Visible = true;
                       lvConditionalBool.Enabled = true;
                       lvSubConditional.BeginUpdate();
                       lvSubConditional.Items.Clear();
                       pArray = DatabaseAPI.Database.Power;
                       eArray = new[] { 6, 8, 9, 10, 11 };
                       foreach (var power in pArray)
                       {
                           var pSetType = power.GetPowerSet().SetType;
                           var isType = power.VariableEnabled;
                           var isUsable = !eArray.Contains((int)pSetType);
                           if (!isUsable || !isType) continue;
                       
                           var pItem = new Regex("[_]");
                           var pStrings = pItem.Replace(power.FullName, " ").Split('.');
                           var pMatch = new Regex("[ ].*");
                           var pArchetype = pMatch.Replace(pStrings[0], "");
                           lvConditionalBool.Size = lvBoolSizeSecondary;
                           lvConditionalBool.Location = lvBoolLocSecondary;
                           lvConditionalOp.Visible = true;
                           lvSubConditional.Items.Add($"{pStrings[2]} [{pArchetype} / {pStrings[1]}]").Name =
                               power.FullName;
                       }
                    */

                    lvSubConditional.BeginUpdate();
                    lvSubConditional.Items.Clear();
                    
                    eArray = [6, 8, 9, 10, 11];
                    foreach (var power in pArray)
                    {
                        if (!TryGetPowerConditionListParts(power, eArray, out var pStrings, out var pArchetype))
                        {
                            continue;
                        }

                        var isType = power.VariableEnabled;
                        if (!isType)
                        {
                            continue;
                        }

                        var textFilter = tbFilter.Text.Trim();
                        if (!string.IsNullOrEmpty(textFilter))
                        {
                            if (!FilterMatch(textFilter, pStrings[2], pArchetype, pStrings[1]))
                            {
                                continue;
                            }
                        }

                        lvSubConditional.Items.Add($"{pStrings[2]} [{pArchetype} / {pStrings[1]}]").Name = power.FullName;
                    }

                    lvSubConditional.Columns[0].Text = @"Power Name [Class / Powerset]";
                    lvSubConditional.EndUpdate();

                    break;
            }
        }
    }
}
