#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastDeepCloner;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Controls;
using MRBResourceLib;

namespace Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor
{
    public sealed partial class frmEffectConditionals : Form
    {
        public readonly List<KeyValue<string, string>> Conditionals;

        private readonly List<string> _conditionalTypes;
        private readonly List<string> _conditionalOps;
        private Dictionary<string, string> _CSFieldsRev;

        public frmEffectConditionals(List<KeyValue<string, string>>? conditions)
        {
            InitializeComponent();
            _conditionalTypes = ["Power Active", "Power Taken", "Stacks", "Team Members", "Combat Setting"];
            _conditionalOps = ["Equal To", "Greater Than", "Less Than"];
            _CSFieldsRev = ConfigData.CombatContext.EnumerateFields(MidsContext.Config.CombatContextSettings)
                .ToDictionary(ConfigData.CombatContext.FormatSettingName, e => e);

            if (conditions != null)
            {
                Conditionals = conditions.Clone();
            }

            Text = @"Effect Conditions";
            Icon = Resources.MRB_Icon_Concept;
            Load += OnLoad;
        }

        private async void OnLoad(object? sender, EventArgs e)
        {
            CenterToParent();
            await UpdateConditionTypes();
            await UpdateConditionals();
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
            lvActiveConditionals.BeginUpdate();
            var getCondition = new Regex("(:.*)");
            var getConditionPower = new Regex("(.*:)");

            var k = 0;
            foreach (var cVp in Conditionals)
            {
                var condition = getCondition.Replace(cVp.Key, "");
                var linkTypeLv = k > 0
                    ? condition.StartsWith("OR ")
                        ? "OR"
                        : "AND"
                    : "";

                condition = condition.Replace("OR ", "");
                var conditionPower = getConditionPower.Replace(cVp.Key, "").Replace(":", "");
                var power = DatabaseAPI.GetPowerByFullName(conditionPower);
                switch (condition)
                {
                    case "Active":
                        var item = new ListViewItem { Text = linkTypeLv, Name = power?.FullName };
                        item.SubItems.Add($@"{condition}:{power?.DisplayName}");
                        item.SubItems.Add("");
                        item.SubItems.Add(cVp.Value);
                        lvActiveConditionals.Items.Add(item);
                        break;
                    
                    case "Taken":
                        item = new ListViewItem { Text = linkTypeLv, Name = power?.FullName };
                        item.SubItems.Add($@"{condition}:{power?.DisplayName}");
                        item.SubItems.Add("");
                        item.SubItems.Add(cVp.Value);
                        lvActiveConditionals.Items.Add(item);
                        break;
                    
                    case "Stacks":
                        item = new ListViewItem { Text = linkTypeLv, Name = power?.FullName };
                        item.SubItems.Add($@"{condition}:{power?.DisplayName}");
                        var cVSplit = cVp.Value.Split(' ');
                        item.SubItems.Add(cVSplit[0]);
                        item.SubItems.Add(cVSplit[1]);
                        lvActiveConditionals.Items.Add(item);
                        break;
                    
                    case "Team":
                        item = new ListViewItem { Text = linkTypeLv, Name = conditionPower };
                        item.SubItems.Add($@"{condition}:{conditionPower}");
                        cVSplit = cVp.Value.Split(' ');
                        item.SubItems.Add(cVSplit[0]);
                        item.SubItems.Add(cVSplit[1]);
                        lvActiveConditionals.Items.Add(item);
                        break;

                    case "Config":
                        var cfgSetting = getConditionPower.Replace(":", "");
                        item = new ListViewItem { Text = linkTypeLv, Name = cfgSetting };
                        item.SubItems.Add($@"{condition}:{ConfigData.CombatContext.FormatSettingName(cfgSetting)}");
                        item.SubItems.Add("");
                        item.SubItems.Add(cVp.Value);
                        lvActiveConditionals.Items.Add(item);
                        break;
                }

                k++;
            }

            lvActiveConditionals.EndUpdate();

            panelLinkType.Visible = Conditionals.Count > 0;
            rbLinkTypeAnd.Checked = true;

            await Task.CompletedTask;
        }

        private void lvConditionalType_SelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
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
                        var pSetType = power?.GetPowerSet()?.SetType;
                        var pType = power?.PowerType;
                        var isType = pType is Enums.ePowerType.Auto_ or Enums.ePowerType.Toggle ||
                                     pType == Enums.ePowerType.Click && power?.ClickBuff == true;
                        var isUsable = !eArray.Contains((int)pSetType);
                        if (!isUsable || !isType)
                        {
                            continue;
                        }

                        var pItem = new Regex("[_]");
                        var pStrings = pItem.Replace(power.FullName, " ").Split('.');
                        var pMatch = new Regex("[ ].*");
                        var pArchetype = pMatch.Replace(pStrings[0], "");
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
                        var pSetType = power?.GetPowerSet()?.SetType;
                        var pType = power?.PowerType;
                        var isType = pType == Enums.ePowerType.Auto_ || pType == Enums.ePowerType.Toggle ||
                                     (pType == Enums.ePowerType.Click && power?.ClickBuff == true);
                        var isUsable = !eArray.Contains((int)pSetType);
                        if (!isUsable && !isType)
                        {
                            continue;
                        }

                        var pItem = new Regex("[_]");
                        var pStrings = pItem.Replace(power.FullName, " ").Split('.');
                        var pMatch = new Regex("[ ].*");
                        var pArchetype = pMatch.Replace(pStrings[0], "");
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
                        var pSetType = power?.GetPowerSet()?.SetType;
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
            }
        }

        private void lvSubConditional_SelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var powName = lvSubConditional.SelectedItems.Count > 0
                ? lvSubConditional.SelectedItems[0].Name
                : string.Empty;

            var selected = DatabaseAPI.GetPowerByFullName(powName);

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
            var linkPrefix = Conditionals.Count > 0 && rbLinkTypeOr.Checked ? "OR " : "";
            var linkPrefixLv = Conditionals.Count > 0
                ? rbLinkTypeOr.Checked
                    ? "OR "
                    : "AND "
                : "";

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
                    Conditionals.Add(new KeyValue<string, string>($"{linkPrefix}Active:{powerName}", value));
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
                    Conditionals.Add(new KeyValue<string, string>($"{linkPrefix}Taken:{powerName}", value));
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
                    Conditionals.Add(new KeyValue<string, string>($"{linkPrefix}Stacks:{powerName}", $"{cOp} {value}"));
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
                    Conditionals.Add(new KeyValue<string, string>($"{linkPrefix}Team:{archetype}", $"{cOp} {value}"));
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
                    Conditionals.Add(new KeyValue<string, string>($"{linkPrefix}Config:{_CSFieldsRev[field]}", $"{cOp} {value}"));

                    break;
            }

            panelLinkType.Visible = Conditionals.Count > 0;
            rbLinkTypeAnd.Checked = true;
        }

        private void removeConditional_Click(object sender, EventArgs e)
        {
            if (lvActiveConditionals.SelectedItems.Count <= 0)
            {
                return;
            }

            foreach (var cVp in Conditionals.Where(kv => kv.Key.Contains(lvActiveConditionals.SelectedItems[0].Name))
                         .ToList())
            {
                Conditionals.Remove(cVp);
            }

            lvActiveConditionals.SelectedItems[0].Remove();
            if (Conditionals.Count == 1)
            {
                Conditionals[0] = new KeyValue<string, string>(Conditionals[0].Key.Replace("OR ", ""), Conditionals[0].Value);
                lvActiveConditionals.Items[0].SubItems[0] = new ListViewItem.ListViewSubItem(lvActiveConditionals.Items[0], "");
            }

            panelLinkType.Visible = Conditionals.Count > 0;
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
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
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

        private void tbFilter_TextChanged(object sender, EventArgs e)
        {
            var conditionalType = lvConditionalType.SelectedIndices.Count < 0
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
                        var pSetType = power?.GetPowerSet()?.SetType;
                        var pType = power?.PowerType;
                        var isType = pType is Enums.ePowerType.Auto_ or Enums.ePowerType.Toggle ||
                                     pType == Enums.ePowerType.Click && power?.ClickBuff == true;
                        var isUsable = !eArray.Contains((int)pSetType);
                        if (!isUsable || !isType)
                        {
                            continue;
                        }

                        var pItem = new Regex("[_]");
                        var pStrings = pItem.Replace(power.FullName, " ").Split('.');
                        var pMatch = new Regex("[ ].*");
                        var pArchetype = pMatch.Replace(pStrings[0], "");
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
                        var pSetType = power?.GetPowerSet()?.SetType;
                        var pType = power?.PowerType;
                        var isType = pType is Enums.ePowerType.Auto_ or Enums.ePowerType.Toggle ||
                                     pType == Enums.ePowerType.Click && power?.ClickBuff == true;
                        var isUsable = !eArray.Contains((int)pSetType);
                        if (!isUsable && !isType)
                        {
                            continue;
                        }

                        var pItem = new Regex("[_]");
                        var pStrings = pItem.Replace(power.FullName, " ").Split('.');
                        var pMatch = new Regex("[ ].*");
                        var pArchetype = pMatch.Replace(pStrings[0], "");
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
                        var pSetType = power?.GetPowerSet()?.SetType;
                        var isType = power.VariableEnabled;
                        var isUsable = !eArray.Contains((int)pSetType);
                        if (!isUsable || !isType)
                        {
                            continue;
                        }

                        var pItem = new Regex("[_]");
                        var pStrings = pItem.Replace(power.FullName, " ").Split('.');
                        var pMatch = new Regex("[ ].*");
                        var pArchetype = pMatch.Replace(pStrings[0], "");
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
