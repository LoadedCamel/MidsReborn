using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using static Mids_Reborn.Core.EnhancementSet;
using static Mids_Reborn.Core.Enums;

namespace Mids_Reborn.Forms
{
    public partial class SetInspector : Form
    {
        private List<Archetype?>? _archetypeList;
        private static List<EnhancementSet> EnhancementSetList => DatabaseAPI.Database.EnhancementSets;
        private List<EnhancementSet>? _filteredEnhancementSets;

        private readonly List<IEffect> _effects;

        private string? _selectedBonus;
        private List<string>? _selectedBonuses;
        private EnhancementSet? _selectedSet;

        private List<IPower>? _baseFilteredPowers;
        private List<IPower>? _filteredPowers;
        private List<IPower>? _selectedPrimaries;
        private List<IPower>? _selectedSecondaries;

        public SetInspector()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            Load += OnLoad;
            _effects = new List<IEffect>();
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            CenterToParent();
            EnableAtFilters(false);
            setImageList ??= new ImageList
            {
                ImageSize = new Size(16, 16),
                ColorDepth = ColorDepth.Depth32Bit
            };
            FillImageList();
            AssignSets();
            AssignEffects();
        }

        /*private void AssignArchetypes()
        {
            var genericAt = new Archetype
            {
                DisplayName = "Select Archetype",
                Playable = false
            };
            var nonPlayable = new List<Archetype?> { genericAt };
            var playable = DatabaseAPI.Database.Classes.Where(x => x is { Playable: true }).ToList();
            _archetypeList = nonPlayable.Concat(playable).ToList();
            cbArchetype.DisplayMember = "DisplayName";
            cbArchetype.ValueMember = null;
            cbArchetype.DataSource = _archetypeList;
            cbArchetype.SelectedIndex = 0;
            EnableAtFilters(false);
        }*/

        private void EnableAtFilters(bool aEnable)
        {
            cbArchetype.Enabled = aEnable;
        }

        private void ApplyAtFilter()
        {
            var filteredPSets = DatabaseAPI.Database.Powersets.Where(ps => ps != null && ps.Powers.Any(power => _selectedSet != null && power != null && power.SetTypes.Contains(_selectedSet.SetType))).ToList();
            var genericAt = new Archetype
            {
                DisplayName = "Select Archetype",
                Playable = false
            };
            var filteredArchetypes = new List<Archetype?> { genericAt };
            foreach (var archetype in from ps in filteredPSets where ps != null select DatabaseAPI.GetArchetypeByClassName(ps.ATClass) into archetype where archetype != null && !filteredArchetypes.Contains(archetype) && archetype.Playable select archetype)
            {
                filteredArchetypes.Add(archetype);
            }
            _archetypeList = filteredArchetypes;
            var selectIdx = 0;
            if (cbArchetype.SelectedIndex > 0)
            {
                var currentAt = cbArchetype.SelectedItem as Archetype;
                var atIdx = _archetypeList.IndexOf(currentAt);
                if (atIdx > 0) selectIdx = atIdx;
            }
            cbArchetype.DisplayMember = "DisplayName";
            cbArchetype.ValueMember = null;
            cbArchetype.DataSource = _archetypeList;
            cbArchetype.SelectedIndex = selectIdx;
            EnableAtFilters(true);
        }

        private void AssignSets()
        {
            if (!EnhancementSetList.Any()) return;
            alvSets.DataSource = EnhancementSetList;
    
            // Add column mappings with custom data retriever and transformers
            alvSets.AddColumnMapping(0, obj => ((EnhancementSet)obj).DisplayName);
            alvSets.AddColumnMapping(1, obj => ((EnhancementSet)obj).LevelMin, 
                value => (int)value! + 1, 
                value2 => (int)value2! + 1);
            alvSets.AddColumnMapping(2, obj => ((EnhancementSet)obj).SetType, 
                value => DatabaseAPI.GetSetTypeByIndex((int)value!).Name);
            alvSets.AddColumnMapping(3, obj => ((EnhancementSet)obj).Bonus, 
                _ => "?");
        }

        private async void FillImageList(bool filtered = false)
        {
            await I9Gfx.LoadSets();
            using var extendedBitmap = new ExtendedBitmap(setImageList.ImageSize);
            setImageList.Images.Clear();
            if (!filtered)
                foreach (var set in EnhancementSetList)
                {
                    if (set.ImageIdx > -1)
                    {
                        extendedBitmap.Graphics!.Clear(Color.Transparent);
                        var graphics = extendedBitmap.Graphics;
                        I9Gfx.DrawEnhancementSet(ref graphics, set.ImageIdx);
                        setImageList.Images.Add(extendedBitmap.Bitmap!);
                    }
                    else
                    {
                        var images = setImageList.Images;
                        var imageSize = setImageList.ImageSize;
                        var bitmap = new Bitmap(imageSize.Width, imageSize.Height);
                        images.Add(bitmap);
                    }
                }
            else
            {
                if (_filteredEnhancementSets == null) return;
                foreach (var set in _filteredEnhancementSets)
                {
                    if (set.ImageIdx > -1)
                    {
                        extendedBitmap.Graphics!.Clear(Color.Transparent);
                        var graphics = extendedBitmap.Graphics;
                        I9Gfx.DrawEnhancementSet(ref graphics, set.ImageIdx);
                        setImageList.Images.Add(extendedBitmap.Bitmap!);
                    }
                    else
                    {
                        var images = setImageList.Images;
                        var imageSize = setImageList.ImageSize;
                        var bitmap = new Bitmap(imageSize.Width, imageSize.Height);
                        images.Add(bitmap);
                    }
                }
            }
        }

        private async void FillPowerImageList()
        {
            if (_baseFilteredPowers == null) return;
            var powerSetImages = await I9Gfx.LoadPowerSets();
            powerImageList.Images.Clear();

            foreach (var power in _baseFilteredPowers)
            {
                var setImage = power.GetPowerSet()?.ImageName;
                if (string.IsNullOrWhiteSpace(setImage)) continue;
                var psImage = powerSetImages.FirstOrDefault(path => path != null && path.Contains(setImage));
                if (psImage != null)
                {
                    powerImageList.Images.Add(Image.FromFile(psImage));
                }
            }
        }

        private void AssignEffects()
        {
            var powerList = new List<IPower?>();

            foreach (var set in EnhancementSetList)
            {
                foreach (var bonus in set.Bonus)
                {
                    powerList.AddRange(bonus.Name.Select(DatabaseAPI.GetPowerByFullName).Where(power => power != null));
                }

                foreach (var specialBonus in set.SpecialBonus)
                {
                    powerList.AddRange(specialBonus.Name.Select(DatabaseAPI.GetPowerByFullName)
                        .Where(power => power != null));
                }
            }


            foreach (var power in powerList.Where(power => power != null))
            {
                if (power != null) _effects.AddRange(power.Effects);
            }

            cbEffect.DataSource = GenerateComboBoxItems(_effects.Where(effect => effect.EffectType is not (eEffectType.Damage or eEffectType.GlobalChanceMod or eEffectType.GrantPower or eEffectType.Null)).Select(e => e.EffectType is eEffectType.Endurance ? "MaxEnd" : e.EffectType.ToString()).Distinct().OrderBy(effectType => effectType).ToList(), "Select Effect");
        }

        private void CbEffect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle cbEffect selection change
            var selectedEffectType = cbEffect.GetItemText(cbEffect.SelectedItem);
            const string placeholderText = "Select Type";

            var effectTypeItems = selectedEffectType switch
            {
                "Defense" or "Resistance" or "DamageBuff" => _effects
                    .Where(effect =>
                        effect.EffectType == (eEffectType)Enum.Parse(typeof(eEffectType), selectedEffectType))
                    .Select(effect => effect.DamageType.ToString())
                    .Distinct()
                    .OrderBy(damageType => damageType)
                    .ToList(),
                "Enhancement" or "ResEffect" => _effects
                    .Where(effect =>
                        effect.EffectType == (eEffectType)Enum.Parse(typeof(eEffectType), selectedEffectType))
                    .Select(effect => effect.ETModifies is eEffectType.Mez or eEffectType.MezResist
                        ?
                        // Include the MezType along with "Mez" for display
                        $"{effect.ETModifies} ({effect.MezType})"
                        : effect.ETModifies.ToString())
                    .Distinct()
                    .OrderBy(effectType => effectType)
                    .ToList(),
                "Mez" or "MezResist" => _effects
                    .Where(effect =>
                        effect.EffectType == (eEffectType)Enum.Parse(typeof(eEffectType), selectedEffectType))
                    .Select(effect => effect.MezType.ToString())
                    .Distinct()
                    .OrderBy(mezType => mezType)
                    .ToList(),
                _ => new List<string>()
            };

            cbEffectType.DataSource = GenerateComboBoxItems(effectTypeItems, placeholderText);

        }

        private void CbEffectType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Handle cbEffectType selection change
            var selectedEffectType = cbEffect.GetItemText(cbEffect.SelectedItem);
            var selectedValue = cbEffectType.GetItemText(cbEffectType.SelectedItem);
            var placeholderText = "Select Strength";

            // Determine the unit based on the selected items
            var unit = "%"; // Default to percentage
            if (selectedEffectType == "HitPoints" || selectedValue == "HitPoints")
            {
                unit = " HP"; // Change to HP if either is HitPoints
            }

            if (selectedEffectType == "MaxEnd") selectedEffectType = "Endurance";

            // Filter effects based on selectedEffectType and selectedValue, then format and bind cbEffectStr with distinct Magnitudes
            var filteredEffects = _effects
                .Where(effect => effect.EffectType == (eEffectType)Enum.Parse(typeof(eEffectType), selectedEffectType) &&
                                 selectedEffectType switch
                                 {
                                     "Defense" or "Resistance" or "DamageBuff" => effect.DamageType.ToString() == selectedValue,
                                     "Enhancement" or "ResEffect" => selectedValue.Contains(effect.ETModifies.ToString()),
                                     _ => selectedEffectType is not ("Mez" or "MezResist") || effect.MezType.ToString() == selectedValue
                                 })
                .ToList();

            // Convert and format Magnitudes as numbers
            var formattedMagnitudes = filteredEffects
                .Select(effect =>
                {
                    var magnitude = Math.Round(Convert.ToDecimal(effect.MagPercent), 2).ToString(CultureInfo.InvariantCulture);
                    string returnVal;
                    if (effect.MezType is eMez.Knockback or eMez.Knockback)
                    {
                        unit = "Mag ";
                        magnitude = Math.Abs(Math.Round(Convert.ToDecimal(effect.MagPercent), 2)).ToString(CultureInfo.InvariantCulture);
                        returnVal = $"{unit}{magnitude}";
                    }
                    else
                    {
                        returnVal = $"{magnitude}{unit}";
                    }
                    return returnVal;
                })
                .Distinct()
                .OrderBy(mag =>
                {
                    if (mag.Contains("Mag"))
                    {
                        // If it's a "Mag" value, parse it as a negative float for correct ordering
                        return -float.Parse(mag.Replace("Mag ", ""));
                    }

                    // Otherwise, parse it as a float
                    return float.Parse(mag.Contains("HP") ? mag.Replace(" HP", "") : mag.TrimEnd('%', ' '));
                })
                .ToList();

            if (formattedMagnitudes.Count > 1) formattedMagnitudes.Insert(0, "Any");

            cbEffectStr.DataSource = GenerateComboBoxItems(formattedMagnitudes, placeholderText);
        }

        private void CbEffectStr_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbEffect.SelectedIndex <= 0 || cbEffectStr.SelectedIndex <= 0) return;

            var selectedEffect = cbEffect.GetItemText(cbEffect.SelectedItem);
            if (selectedEffect is "MaxEnd") selectedEffect = "Endurance";
            var effectType = (eEffectType)Enum.Parse(typeof(eEffectType), selectedEffect);
            var magList = new List<decimal>();
            var chosenEffect = new SelectedEffect();

            if (cbEffectStr.SelectedIndex == 1)
            {
                var magItems = cbEffectStr.Items.Cast<StandardListItem>().ToList();
                magItems.RemoveAt(0);
                magItems.RemoveAt(0);

                foreach (var item in magItems)
                {
                    if (!item.Text.Contains("Mag"))
                    {
                        var mag = decimal.Parse(item.Text.Replace(" HP", "").Replace("%", "").Trim());
                        magList.Add(mag);
                    }
                    else
                    {
                        var mag = decimal.Parse(item.Text.Replace("Mag ", "-").Trim());
                        magList.Add(mag);
                    }
                }

                switch (effectType)
                {
                    case eEffectType.Defense or eEffectType.Resistance or eEffectType.DamageBuff:
                        {
                            if (cbEffectType.SelectedIndex > 0)
                            {
                                var selectedEffectType = cbEffectType.GetItemText(cbEffectType.SelectedItem);
                                var damageType = (eDamage)Enum.Parse(typeof(eDamage), selectedEffectType);
                                chosenEffect = new SelectedEffect(effectType, damageType, magList);
                            }

                            break;
                        }
                    case eEffectType.Mez or eEffectType.MezResist:
                        {
                            if (cbEffectType.SelectedIndex > 0)
                            {
                                var selectedEffectType = cbEffectType.GetItemText(cbEffectType.SelectedItem);
                                var mezType = (eMez)Enum.Parse(typeof(eMez), selectedEffectType);
                                chosenEffect = new SelectedEffect(effectType, mezType, magList);
                            }

                            break;
                        }
                    case eEffectType.Enhancement:
                        {
                            if (cbEffectType.SelectedIndex > 0)
                            {
                                var selectedEffectType = cbEffectType.GetItemText(cbEffectType.SelectedItem);
                                if (selectedEffectType.Contains("Mez"))
                                {
                                    var selectedSplit = selectedEffectType.Split(' ');
                                    var etModifies = (eEffectType)Enum.Parse(typeof(eEffectType), selectedSplit[0]);
                                    const string pattern = @"\((\w+)\)";
                                    var match = Regex.Match(selectedSplit[1], pattern);
                                    var mezType = (eMez)Enum.Parse(typeof(eMez), match.Groups[1].Value);
                                    chosenEffect = new SelectedEffect(effectType, etModifies, mezType, magList);
                                }
                                else
                                {
                                    var etModifies = (eEffectType)Enum.Parse(typeof(eEffectType), selectedEffectType);
                                    chosenEffect = new SelectedEffect(effectType, etModifies, magList);
                                }
                            }

                            break;
                        }
                    case eEffectType.ResEffect:
                        {
                            if (cbEffectType.SelectedIndex > 0)
                            {
                                var selectedEffectType = cbEffectType.GetItemText(cbEffectType.SelectedItem);
                                var etModifies = (eEffectType)Enum.Parse(typeof(eEffectType), selectedEffectType);
                                chosenEffect = new SelectedEffect(effectType, etModifies, magList);
                            }

                            break;
                        }
                    default:
                        {
                            chosenEffect = new SelectedEffect(effectType, magList);
                            break;
                        }
                }

                if (chosenEffect.EffectType is eEffectType.None) return;
                var selectedEffects = _effects.Where(effect =>
                    chosenEffect.MagPercentList != null &&
                    effect.EffectType == chosenEffect.EffectType &&
                    effect.ETModifies == chosenEffect.EtModifies &&
                    effect.DamageType == chosenEffect.DamageType && effect.MezType == chosenEffect.MezType &&
                    chosenEffect.MagPercentList.Contains(Math.Round(Convert.ToDecimal(effect.MagPercent), 2)));

                var enumerable = selectedEffects.ToList();
                if (!enumerable.Any()) return;
                _selectedBonuses = enumerable.Select(x => x.PowerFullName).ToList();
                FilterSetsByMultipleBonuses(_selectedBonuses);
            }
            else
            {
                switch (effectType)
                {
                    case eEffectType.Defense or eEffectType.Resistance or eEffectType.DamageBuff:
                        {
                            if (cbEffectType.SelectedIndex > 0)
                            {
                                var selectedEffectType = cbEffectType.GetItemText(cbEffectType.SelectedItem);
                                var selectEffectStr = cbEffectStr.GetItemText(cbEffectStr.SelectedItem).Replace("%", "")
                                    .Replace(" HP", "").Trim();
                                var damageType = (eDamage)Enum.Parse(typeof(eDamage), selectedEffectType);
                                var magPercent = decimal.Parse(selectEffectStr);
                                chosenEffect = new SelectedEffect(effectType, damageType, magPercent);
                            }

                            break;
                        }
                    case eEffectType.Mez or eEffectType.MezResist:
                        {
                            if (cbEffectType.SelectedIndex > 0)
                            {
                                var selectedEffectType = cbEffectType.GetItemText(cbEffectType.SelectedItem);
                                var selectEffectStr = cbEffectStr.GetItemText(cbEffectStr.SelectedItem).Replace("%", "")
                                    .Replace(" HP", "").Trim();
                                if (selectEffectStr.Contains("Mag"))
                                {
                                    selectEffectStr = selectEffectStr.Replace("Mag ", "-");
                                }

                                var mezType = (eMez)Enum.Parse(typeof(eMez), selectedEffectType);
                                var magPercent = decimal.Parse(selectEffectStr);
                                Debug.WriteLine(magPercent);
                                chosenEffect = new SelectedEffect(effectType, mezType, magPercent);
                            }

                            break;
                        }
                    case eEffectType.Enhancement:
                        {
                            if (cbEffectType.SelectedIndex > 0)
                            {
                                var selectedEffectType = cbEffectType.GetItemText(cbEffectType.SelectedItem);
                                var selectEffectStr = cbEffectStr.GetItemText(cbEffectStr.SelectedItem).Replace("%", "").Replace(" HP", "").Replace("Mag", "").Trim();
                                if (selectedEffectType.Contains("Mez"))
                                {
                                    var selectedSplit = selectedEffectType.Split(' ');
                                    var etModifies = (eEffectType)Enum.Parse(typeof(eEffectType), selectedSplit[0]);
                                    var pattern = @"\((\w+)\)";
                                    var match = Regex.Match(selectedSplit[1], pattern);
                                    var mezType = (eMez)Enum.Parse(typeof(eMez), match.Groups[1].Value);
                                    var magPercent = decimal.Parse(selectEffectStr);
                                    chosenEffect = new SelectedEffect(effectType, etModifies, mezType, magPercent);
                                }
                                else
                                {
                                    var etModifies = (eEffectType)Enum.Parse(typeof(eEffectType), selectedEffectType);
                                    var magPercent = decimal.Parse(selectEffectStr);
                                    chosenEffect = new SelectedEffect(effectType, etModifies, magPercent);
                                }
                            }

                            break;
                        }
                    case eEffectType.ResEffect:
                        {
                            if (cbEffectType.SelectedIndex > 0)
                            {
                                var selectedEffectType = cbEffectType.GetItemText(cbEffectType.SelectedItem);
                                var selectEffectStr = cbEffectStr.GetItemText(cbEffectStr.SelectedItem).Replace("%", "")
                                    .Replace(" HP", "").Trim();
                                var etModifies = (eEffectType)Enum.Parse(typeof(eEffectType), selectedEffectType);
                                var magPercent = decimal.Parse(selectEffectStr);
                                chosenEffect = new SelectedEffect(effectType, etModifies, magPercent);
                            }

                            break;
                        }
                    default:
                        {
                            var selectEffectStr = cbEffectStr.GetItemText(cbEffectStr.SelectedItem).Replace("%", "")
                                .Replace(" HP", "").Trim();
                            var magPercent = decimal.Parse(selectEffectStr);
                            chosenEffect = new SelectedEffect(effectType, magPercent);
                            break;
                        }
                }

                if (chosenEffect.EffectType is eEffectType.None) return;

                var selected = _effects.FirstOrDefault(effect =>
                    effect.EffectType == chosenEffect.EffectType &&
                    effect.ETModifies == chosenEffect.EtModifies &&
                    effect.DamageType == chosenEffect.DamageType && effect.MezType == chosenEffect.MezType &&
                    Math.Round(Convert.ToDecimal(effect.MagPercent), 2) == chosenEffect.MagPercent);

                if (selected == null) return;
                _selectedBonus = selected.PowerFullName;
                FilterSetsByBonus(_selectedBonus);
            }
        }

        private void FilterSetsByMultipleBonuses(IReadOnlyList<string> selectedBonuses)
        {
            _filteredEnhancementSets = EnhancementSetList
                .Where(set =>
                    set.Bonus.Any(bonus => selectedBonuses.Any(selectedBonus => bonus.Name.Contains(selectedBonus))) ||
                    set.SpecialBonus.Any(specialBonus =>
                        selectedBonuses.Any(selectedBonus => specialBonus.Name.Contains(selectedBonus))))
                .ToList();

            FillImageList(true);
            alvSets.DataSource = _filteredEnhancementSets;

            // Clear existing column mappings
            alvSets.ColumnMappings?.Clear();

            // Add column mappings with custom data retriever and transformers
            alvSets.AddColumnMapping(0, obj => ((EnhancementSet)obj).DisplayName);
            alvSets.AddColumnMapping(1, obj => ((EnhancementSet)obj).LevelMin,
                value => (int)value! + 1,
                value2 => (int)value2! + 1);
            alvSets.AddColumnMapping(2, obj => ((EnhancementSet)obj).SetType,
                value => DatabaseAPI.GetSetTypeByIndex((int)value!).Name);
            alvSets.AddColumnMapping(3, obj => ((EnhancementSet)obj).Bonus,
                value =>
                {
                    var req = "S";
                    if (value is not BonusItem[] bonusItems)
                    {
                        req = "-1";
                    }
                    else
                    {
                        for (var i = 0; i < bonusItems.Length; i++)
                        {
                            var i1 = i;
                            if (selectedBonuses.Any(selectedBonus => bonusItems[i1].Name.Contains(selectedBonus)))
                            {
                                req = bonusItems[i].Slotted.ToString();
                            }
                        }
                    }

                    return req;
                });
        }

        private void FilterSetsByBonus(string selectedBonus)
        {
            _filteredEnhancementSets = EnhancementSetList
                .Where(set => set.Bonus.Any(bonus => bonus.Name.Contains(selectedBonus)) ||
                              set.SpecialBonus.Any(specialBonus => specialBonus.Name.Contains(selectedBonus)))
                .ToList();

            FillImageList(true);
            alvSets.DataSource = _filteredEnhancementSets;

            // Clear existing column mappings
            alvSets.ColumnMappings?.Clear();

            // Add column mappings with custom data retriever and transformers
            alvSets.AddColumnMapping(0, obj => ((EnhancementSet)obj).DisplayName);
            alvSets.AddColumnMapping(1, obj => ((EnhancementSet)obj).LevelMin,
                value => (int)value! + 1,
                value2 => (int)value2! + 1);
            alvSets.AddColumnMapping(2, obj => ((EnhancementSet)obj).SetType,
                value => DatabaseAPI.GetSetTypeByIndex((int)value!).Name);
            alvSets.AddColumnMapping(3, obj => ((EnhancementSet)obj).Bonus,
                value =>
                {
                    var req = "S";
                    if (value is not BonusItem[] bonusItems)
                    {
                        req = "-1";
                    }
                    else
                    {
                        for (var i = 0; i < bonusItems.Length; i++)
                        {
                            if (bonusItems[i].Name.Contains(selectedBonus))
                            {
                                req = bonusItems[i].Slotted.ToString();
                            }
                        }
                    }

                    return req;
                }, tagFunction: value =>
                {
                    if (value is EnhancementSet set)
                    {
                        for (var index = 0; index < set.Bonus.Length; index++)
                        {
                            if (set.Bonus[index].Name.Contains(selectedBonus))
                            {
                                return set.GetEffectString(index, false, true, true, true);
                            }
                        }

                        for (var index = 0; index < set.SpecialBonus.Length; index++)
                        {
                            if (set.SpecialBonus[index].Name.Contains(selectedBonus))
                            {
                                return set.GetEffectString(index, true, true, true, true);
                            }
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"Unexpected type: {value?.GetType().Name ?? "null"}");
                    }

                    return null;
                });
        }
        
        private void AdvListView1OnSelectedIndexChanged(object? sender, EventArgs e)
        {
            if (alvSets.SelectedItems.Count == 0) return;

            // Get the selected item's index
            var selectedIndex = alvSets.SelectedIndices[0];

            // Get the selected EnhancementSet from the data source
            if (selectedIndex < 0 || selectedIndex >= alvSets.Items.Count) return;
            if (alvSets.DataSource is not List<EnhancementSet> sets) return;
            _selectedSet = sets[selectedIndex];
            var setData = new EnhSetInfo.SetData
            {
                Set = _selectedSet.DisplayName,
                SetRarity = _selectedSet.GetEnhancementSetRarity(),
                SetType = DatabaseAPI.GetSetTypeByIndex(_selectedSet.SetType).Name,
                LevelRange = $"{_selectedSet.LevelMin + 1} to {_selectedSet.LevelMax + 1}",
                EnhCount = _selectedSet.Enhancements.Length.ToString(),
            };

            foreach (var enh in _selectedSet.Enhancements)
            {
                setData.Enhancements.Add($"{DatabaseAPI.Database.Enhancements[enh].Name}");
            }

            for (var index = 0; index < _selectedSet.Bonus.Length; index++)
            {
                var bonus = _selectedSet.Bonus[index];
                var effectString = _selectedSet.GetEffectString(index, false, true, true, true);
                if (string.IsNullOrWhiteSpace(effectString)) continue;
                if (bonus.PvMode is ePvX.PvP) effectString += " [PvP]";
                setData.Bonuses.Add($"({bonus.Slotted}) {effectString}");
            }
            for (var index = 0; index < _selectedSet.SpecialBonus.Length; index++)
            {
                var specialBonus = _selectedSet.SpecialBonus[index];
                var effectString = _selectedSet.GetEffectString(index, true, true, true, true);
                if (string.IsNullOrWhiteSpace(effectString)) continue;
                if (specialBonus.PvMode is ePvX.PvP) effectString += " [PvP]";
                setData.Bonuses.Add($"(Enh) {effectString}");
            }

            // Check if selected is a special
            var selected = alvSets.SelectedItems[0].Tag;
            if (!string.IsNullOrWhiteSpace(selected as string))
            {
                setData.Selected = (string)selected;
            }

            enhSetInfo1.SetInfo(setData);
            ApplyAtFilter();
        }

        private static List<StandardListItem> GenerateComboBoxItems(IEnumerable<string> items, string placeholderText)
        {
            var comboBoxItems = new List<StandardListItem> { new(placeholderText) };
            comboBoxItems.AddRange(items.Select(item => new StandardListItem(item)));
            return comboBoxItems;
        }

        private void CbArchetypeOnSelectedIndexChanged(object? sender, EventArgs e)
        {
            switch (cbArchetype.SelectedIndex)
            {
                case < 0:
                    return;
                case 0:
                    alvPowers.DataSource = null;
                    alvPowers.Refresh();
                    return;
            }

            if (cbArchetype.SelectedItem is not Archetype archetype) return;

            // Create generics
            var gpList = new List<IPowerset?>
            {
                new Powerset
                {
                    DisplayName = "Select Primary",
                    ATClass = archetype.ClassName
                }
            };
            var gsList = new List<IPowerset?>
            {
                new Powerset
                {
                    DisplayName = "Select Secondary",
                    ATClass = archetype.ClassName
                }
            };

            // Filter primaries only and concat with generic
            var atpList = (from powerSet in DatabaseAPI.Database.Powersets where powerSet is not null where powerSet.ATClass == archetype.ClassName && powerSet.GroupName == archetype.PrimaryGroup from power in powerSet.Powers where power != null && _selectedSet != null && power.SetTypes.Contains(_selectedSet.SetType) select powerSet).Distinct().ToList();
            var primaryList = gpList.Concat(atpList).ToList();

            // Filter secondaries only and contact with generic
            var atsList = (from powerSet in DatabaseAPI.Database.Powersets where powerSet is not null where powerSet.ATClass == archetype.ClassName && powerSet.GroupName == archetype.SecondaryGroup from power in powerSet.Powers where power != null && _selectedSet != null && power.SetTypes.Contains(_selectedSet.SetType) select powerSet).Distinct().ToList();
            var secondaryList = gsList.Concat(atsList).ToList();

            _baseFilteredPowers = new List<IPower>();
            var primaryEmpty = primaryList.Count < 2;
            if (!primaryEmpty)
            {
                foreach (var power in from set in atpList select set.Powers into atpPowers from power in atpPowers where power is not null && _selectedSet is not null &&
                             power.SetTypes.Contains(_selectedSet.SetType) && !_baseFilteredPowers.Contains(power) select power)
                {
                    _baseFilteredPowers.Add(power);
                }
            }

            var secondaryEmpty = secondaryList.Count < 2;
            if (!secondaryEmpty)
            {
                foreach (var power in from set in atsList select set.Powers into atsPowers from power in atsPowers where power is not null && _selectedSet is not null &&
                             power.SetTypes.Contains(_selectedSet.SetType) && !_baseFilteredPowers.Contains(power) select power)
                {
                    _baseFilteredPowers.Add(power);
                }
            }

            EnableAtFilters(true);
            FillAlvPowers();
        }

        private void FillAlvPowers()
        {
            FillPowerImageList();
            alvPowers.DataSource = _baseFilteredPowers;
            alvPowers.AddColumnMapping(0, obj => ((IPower)obj).GetPowerSet()?.DisplayName);
            alvPowers.AddColumnMapping(1, obj => ((IPower)obj).GetPowerSet()?.SetType);
            alvPowers.AddColumnMapping(2, obj => ((IPower)obj).DisplayName);
            UpdateAlvPowerColumns();
        }

        private void UpdateAlvPowerColumns(bool usePower = false)
        {
            if (usePower)
            {
                alvPowers.Columns[0].Text = @"Power";
                alvPowers.Columns[1].Text = @"Type";
                alvPowers.Columns[2].Text = @"Available At";
            }
            else
            {
                alvPowers.Columns[0].Text = @"Powerset";
                alvPowers.Columns[1].Text = @"Type";
                alvPowers.Columns[2].Text = @"Power";
            }
            alvPowers.Refresh();
        }

        internal class SelectedEffect
        {
            internal eEffectType EffectType { get; set; }
            internal eEffectType? EtModifies { get; set; }
            internal eDamage? DamageType { get; set; }
            internal eMez? MezType { get; set; }
            internal decimal MagPercent { get; set; }
            internal List<decimal>? MagPercentList { get; set; }

            public SelectedEffect()
            {
                EffectType = eEffectType.None;
                EtModifies = eEffectType.None;
                DamageType = eDamage.None;
                MezType = eMez.None;
                MagPercent = 0;
            }

            public SelectedEffect(eEffectType effectType, decimal magPercent)
            {
                EffectType = effectType;
                EtModifies = eEffectType.None;
                DamageType = eDamage.None;
                MezType = eMez.None;
                MagPercent = magPercent;
            }

            public SelectedEffect(eEffectType effectType, eDamage damageType, decimal magPercent)
            {
                EffectType = effectType;
                EtModifies = eEffectType.None;
                DamageType = damageType;
                MezType = eMez.None;
                MagPercent = magPercent;
            }

            public SelectedEffect(eEffectType effectType, eMez mezType, decimal magPercent)
            {
                EffectType = effectType;
                EtModifies = eEffectType.None;
                DamageType = eDamage.None;
                MezType = mezType;
                MagPercent = magPercent;
            }

            public SelectedEffect(eEffectType effectType, eEffectType etModifies, decimal magPercent)
            {
                EffectType = effectType;
                EtModifies = etModifies;
                DamageType = eDamage.None;
                MezType = eMez.None;
                MagPercent = magPercent;
            }

            public SelectedEffect(eEffectType effectType, eEffectType etModifies, eMez mezType, decimal magPercent)
            {
                EffectType = effectType;
                EtModifies = etModifies;
                DamageType = eDamage.None;
                MezType = mezType;
                MagPercent = magPercent;
            }

            public SelectedEffect(eEffectType effectType, List<decimal> magPercentList)
            {
                EffectType = effectType;
                EtModifies = eEffectType.None;
                DamageType = eDamage.None;
                MezType = eMez.None;
                MagPercent = 0;
                MagPercentList = magPercentList;
            }

            public SelectedEffect(eEffectType effectType, eDamage damageType, List<decimal> magPercentList)
            {
                EffectType = effectType;
                EtModifies = eEffectType.None;
                DamageType = damageType;
                MezType = eMez.None;
                MagPercent = 0;
                MagPercentList = magPercentList;
            }

            public SelectedEffect(eEffectType effectType, eMez mezType, List<decimal> magPercentList)
            {
                EffectType = effectType;
                EtModifies = eEffectType.None;
                DamageType = eDamage.None;
                MezType = mezType;
                MagPercent = 0;
                MagPercentList = magPercentList;
            }

            public SelectedEffect(eEffectType effectType, eEffectType etModifies, List<decimal> magPercentList)
            {
                EffectType = effectType;
                EtModifies = etModifies;
                DamageType = eDamage.None;
                MezType = eMez.None;
                MagPercent = 0;
                MagPercentList = magPercentList;
            }

            public SelectedEffect(eEffectType effectType, eEffectType etModifies, eMez mezType, List<decimal> magPercentList)
            {
                EffectType = effectType;
                EtModifies = etModifies;
                DamageType = eDamage.None;
                MezType = mezType;
                MagPercent = 0;
                MagPercentList = magPercentList;
            }
        }

        internal class StandardListItem
        {
            public string Text { get; set; }

            public StandardListItem(string text)
            {
                Text = text;
            }

            public override string ToString()
            {
                return Text;
            }
        }
    }
}