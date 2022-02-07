using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Display;
using mrbBase.Base.Extensions;
using mrbBase.Base.Master_Classes;
using mrbControls;

namespace Mids_Reborn.Forms
{
    public partial class frmSetFind : Form
    {
        private struct fxIdentifier
        {
            public Enums.eEffectType EffectType;
            public Enums.eDamage DamageType;
            public Enums.eMez MezType;
            public Enums.eEffectType ETModifies;
            public Enums.eMez SubMezType;

            public override string ToString()
            {
                return $"<fxIdentifier>({EffectType}, {DamageType}, {MezType}, {ETModifies}, {SubMezType})";
            }
        }

        private readonly frmMain myParent;

        private ImageButton ibClose;
        private ImageButton ibTopmost;
        private ctlPopUp SetInfo;

        private List<Enums.eEffectType> DisallowedEffectTypes;
        private List<Enums.eEffectType> EnhancementOnlyEffectTypes;
        private List<Enums.eEffectType> HasSubsEffectTypes;

        // Dictionary<fxIdentifier, Dictionary<effectMagString, List<KeyValuePair<setNID, BonusID>>>>
        private Dictionary<fxIdentifier, Dictionary<string, List<KeyValuePair<int, int>>>> EffectsMap;

        public frmSetFind(frmMain iParent)
        {
            FormClosed += frmSetFind_FormClosed;
            Load += frmSetFind_Load;
            InitializeComponent();
            //var componentResourceManager = new ComponentResourceManager(typeof(frmSetFind));
            Icon = Resources.reborn;
            Name = nameof(frmSetFind);
            ibClose.ButtonClicked += ibClose_ButtonClicked;
            ibTopmost.ButtonClicked += ibTopmost_ButtonClicked;
            myParent = iParent;
        }

        private void frmSetFind_Load(object sender, EventArgs e)
        {
            //SetBonusList = DatabaseAPI.NidPowers("Set_Bonus.Set_Bonus").ToList();
            BuildEffectsMap();

            #region Effect filters lists
            
            // Effects that are not part of set bonuses (or no one may care about)
            DisallowedEffectTypes = new List<Enums.eEffectType>()
            {
                Enums.eEffectType.None,
                Enums.eEffectType.AddBehavior,
                Enums.eEffectType.BeastRun,
                Enums.eEffectType.ClearDamagers,
                Enums.eEffectType.ClearFog,
                Enums.eEffectType.CombatModShift,
                Enums.eEffectType.CombatPhase,
                Enums.eEffectType.Damage,
                Enums.eEffectType.DesignerStatus,
                Enums.eEffectType.DropToggles,
                Enums.eEffectType.EntCreate,
                Enums.eEffectType.EntCreate_x,
                Enums.eEffectType.ExclusiveVisionPhase,
                Enums.eEffectType.ForceMove,
                Enums.eEffectType.Glide,
                Enums.eEffectType.GlobalChanceMod,
                Enums.eEffectType.GrantPower,
                Enums.eEffectType.Hoverboard,
                Enums.eEffectType.Jumppack,
                Enums.eEffectType.MagicCarpet,
                Enums.eEffectType.Meter,
                Enums.eEffectType.ModifyAttrib,
                Enums.eEffectType.NinjaRun,
                Enums.eEffectType.Null,
                Enums.eEffectType.NullBool,
                Enums.eEffectType.PowerRedirect,
                Enums.eEffectType.RechargePower,
                Enums.eEffectType.RevokePower,
                Enums.eEffectType.Reward,
                Enums.eEffectType.RewardSourceTeam,
                Enums.eEffectType.SetCostume,
                Enums.eEffectType.SetMode,
                Enums.eEffectType.SetSZEValue,
                Enums.eEffectType.SilentKill,
                Enums.eEffectType.SteamJump,
                Enums.eEffectType.TokenAdd,
                Enums.eEffectType.UnsetMode,
                Enums.eEffectType.ViewAttrib,
                Enums.eEffectType.VisionPhase,
                Enums.eEffectType.Walk,
                Enums.eEffectType.XAfraid,
                Enums.eEffectType.XAvoid,
                Enums.eEffectType.XPDebt
            };

            // Effect types with a sub-attribute
            HasSubsEffectTypes = new List<Enums.eEffectType>
            {
                Enums.eEffectType.Damage,
                Enums.eEffectType.DamageBuff,
                Enums.eEffectType.Defense,
                Enums.eEffectType.Elusivity,
                Enums.eEffectType.Enhancement,
                Enums.eEffectType.Mez,
                Enums.eEffectType.MezResist,
                Enums.eEffectType.ResEffect,
                Enums.eEffectType.Resistance
            };

            // Effects that are listed under Enhancement() only
            // Exhaustive list?
            EnhancementOnlyEffectTypes = new List<Enums.eEffectType>
            {
                Enums.eEffectType.Absorb,
                Enums.eEffectType.Accuracy,
                Enums.eEffectType.Heal,
                Enums.eEffectType.RechargeTime
            };
            #endregion

            BackColor = myParent.BackColor;
            SetImageButtonStyle(ibClose);
            SetImageButtonStyle(ibTopmost);
            SetInfo.SetPopup(new PopUp.PopupData());
            FillImageList();
            FillEffectList();
            FillArchetypesList();

            cbArchetype.SelectedIndex = 0;
        }

        private void AddEffect(ref List<string> list, ref List<int> nIDList, string effect, int nID)
        {
            if (list.Contains(effect)) return;

            list.Add(effect);
            nIDList.Add(nID);
        }

        private void AddEffect(ref Dictionary<string, int> list, string effect, int nID)
        {
            if (list.ContainsKey(effect)) return;

            list.Add(effect, nID);
        }

        private void BuildEffectsMap()
        {
            //var t = new Stopwatch();
            //t.Start();

            EffectsMap = new Dictionary<fxIdentifier, Dictionary<string, List<KeyValuePair<int, int>>>>();

            for (var s = 0; s < DatabaseAPI.Database.EnhancementSets.Count; s++)
            {
                var set = DatabaseAPI.Database.EnhancementSets[s];
                for (var b = 0; b < set.Bonus.Length; b++)
                {
                    var bonus = set.Bonus[b];
                    foreach (var i in bonus.Index)
                    {
                        foreach (var fx in DatabaseAPI.Database.Power[i].Effects)
                        {
                            var fxKey = fx.EffectType == Enums.eEffectType.Enhancement &
                                        fx.ETModifies == Enums.eEffectType.Mez
                                ? new fxIdentifier
                                {
                                    EffectType = fx.EffectType,
                                    DamageType = fx.DamageType,
                                    ETModifies = fx.ETModifies,
                                    MezType = Enums.eMez.None,
                                    SubMezType = fx.MezType
                                }
                                : new fxIdentifier
                                {
                                    EffectType = fx.EffectType,
                                    DamageType = fx.DamageType,
                                    ETModifies = fx.ETModifies,
                                    MezType = fx.MezType,
                                    SubMezType = Enums.eMez.None
                                };

                            string effect;
                            float effectMag;
                            if (fx.EffectType == Enums.eEffectType.HitPoints)
                            {
                                effectMag = fx.Mag / MidsContext.Archetype.Hitpoints * 100;
                                effect = $"{decimal.Round((decimal)effectMag, 3):##0.###}%";
                            }
                            else if (fx.EffectType == Enums.eEffectType.Endurance)
                            {
                                effectMag = fx.Mag;
                                effect = $"{decimal.Round((decimal)effectMag, 3):##0.###}%";
                            }
                            else
                            {
                                effectMag = fx.MagPercent;
                                effect = $"{decimal.Round((decimal)effectMag, 3):##0.###}%";
                            }

                            if (!EffectsMap.ContainsKey(fxKey))
                            {
                                EffectsMap.Add(fxKey, new Dictionary<string, List<KeyValuePair<int, int>>>());
                                EffectsMap[fxKey].Add(effect, new List<KeyValuePair<int, int>>());
                                EffectsMap[fxKey][effect].Add(new KeyValuePair<int, int>(s, b));
                            }
                            else
                            {
                                if (!EffectsMap[fxKey].ContainsKey(effect))
                                {
                                    EffectsMap[fxKey].Add(effect, new List<KeyValuePair<int, int>>());
                                    EffectsMap[fxKey][effect].Add(new KeyValuePair<int, int>(s, b));
                                }
                                else
                                {
                                    EffectsMap[fxKey][effect].Add(new KeyValuePair<int, int>(s, b));
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool EffectsMapContainsPartial(fxIdentifier fxKey)
        {
            if (EffectsMap.ContainsKey(fxKey)) return true;

            var keysList = EffectsMap.Keys;
            switch (fxKey.EffectType)
            {
                case Enums.eEffectType.Enhancement when fxKey.ETModifies == Enums.eEffectType.Mez:
                    return keysList.Any(e => e.EffectType == fxKey.EffectType & e.ETModifies == fxKey.ETModifies);

                case Enums.eEffectType.Enhancement:
                case Enums.eEffectType.None:
                    return false;

                case Enums.eEffectType.Damage when fxKey.DamageType == Enums.eDamage.None:
                case Enums.eEffectType.DamageBuff when fxKey.DamageType == Enums.eDamage.None:
                case Enums.eEffectType.Defense when fxKey.DamageType == Enums.eDamage.None:
                case Enums.eEffectType.Resistance when fxKey.DamageType == Enums.eDamage.None:
                case Enums.eEffectType.Elusivity when fxKey.DamageType == Enums.eDamage.None:
                case Enums.eEffectType.Mez when fxKey.MezType == Enums.eMez.None:
                case Enums.eEffectType.MezResist when fxKey.MezType == Enums.eMez.None:
                case Enums.eEffectType.ResEffect when fxKey.ETModifies == Enums.eEffectType.None:
                    return keysList.Any(e => e.EffectType == fxKey.EffectType);

                default:
                    return keysList.Any(e => e.EffectType == fxKey.EffectType);
            }
        }

        private void AddEnhancementSet(int nIDSet, int BonusID)
        {
            lvSet.Items.Add(new ListViewItem(new[]
            {
                DatabaseAPI.Database.EnhancementSets[nIDSet].DisplayName,
                $"{DatabaseAPI.Database.EnhancementSets[nIDSet].LevelMin + 1} - {DatabaseAPI.Database.EnhancementSets[nIDSet].LevelMax + 1}",
                DatabaseAPI.Database.SetTypeStringLong[(int) DatabaseAPI.Database.EnhancementSets[nIDSet].SetType],
                BonusID >= 0
                    ? $"{DatabaseAPI.Database.EnhancementSets.GetSetBonusEnhCount(nIDSet, BonusID)}"
                    : "Special"
            }, nIDSet));
            lvSet.Items[lvSet.Items.Count - 1].Tag = nIDSet;
        }

        private void FillEffectList()
        {
            lvBonus.BeginUpdate();
            lvBonus.Items.Clear();

            var fxTypeNames = Enum.GetValues(typeof(Enums.eEffectType)).Cast<Enums.eEffectType>().ToList();
            var fxTypes = fxTypeNames
                .Select((t, i) => new KeyValuePair<Enums.eEffectType, int>(t, i))
                .Where(tk => !DisallowedEffectTypes.Contains(tk.Key) & !EnhancementOnlyEffectTypes.Contains(tk.Key))
                .OrderBy(tk => $"{tk.Key}")
                .ToList();

            lvBonus.Items.Add(new ListViewItem(""));
            lvBonus.Items[0].Tag = -1;
            for (var i = 0; i < fxTypes.Count; i++)
            {
                lvBonus.Items.Add(fxTypes[i].Key.ToString() == "Endurance"
                    ? new ListViewItem("EndMax")
                    : new ListViewItem($"{fxTypes[i].Key}"));

                lvBonus.Items[i + 1].Tag = i;
            }

            lvBonus.Sorting = SortOrder.Ascending;
            lvBonus.Sort();
            if (lvBonus.Items.Count > 0)
                lvBonus.Items[0].Selected = true;
            lvBonus.EndUpdate();
        }

        private void FillImageList()
        {
            var imageSize1 = ilSets.ImageSize;
            var width1 = imageSize1.Width;
            imageSize1 = ilSets.ImageSize;
            var height1 = imageSize1.Height;
            using var extendedBitmap = new ExtendedBitmap(width1, height1);
            ilSets.Images.Clear();
            foreach (var set in DatabaseAPI.Database.EnhancementSets)
            {
                if (set.ImageIdx > -1)
                {
                    extendedBitmap.Graphics.Clear(Color.Transparent);
                    var graphics = extendedBitmap.Graphics;
                    I9Gfx.DrawEnhancementSet(ref graphics, set.ImageIdx);
                    ilSets.Images.Add(extendedBitmap.Bitmap);
                }
                else
                {
                    var images = ilSets.Images;
                    var imageSize2 = ilSets.ImageSize;
                    var width2 = imageSize2.Width;
                    imageSize2 = ilSets.ImageSize;
                    var height2 = imageSize2.Height;
                    var bitmap = new Bitmap(width2, height2);
                    images.Add(bitmap);
                }
            }
        }

        private fxIdentifier BuildFilterFxIdentifier()
        {
            var lvText = lvBonus.SelectedItems[0].Text;
            if (lvText == "EndMax")
            {
                lvText = "Endurance";
            }

            var ret = Enum.TryParse<Enums.eEffectType>(lvText, out var effectType);
            if (!ret) effectType = Enums.eEffectType.None;
            var damageType = Enums.eDamage.None;
            var mezType = Enums.eMez.None;
            var etModifies = Enums.eEffectType.None;
            var mezTypeSub = Enums.eMez.None;
            if (lvVector.SelectedItems.Count > 0)
            {
                var selectedVector = lvVector.SelectedItems[0].Text;
                var chunks = new Regex(@"[\(\)]").Split(selectedVector, 2);
                if (chunks.Length > 1)
                {
                    ret = Enum.TryParse(chunks[0], out etModifies);
                    if (ret && etModifies == Enums.eEffectType.Mez)
                    {
                        ret = Enum.TryParse(chunks[1], out mezTypeSub);
                        if (!ret) mezTypeSub = Enums.eMez.None;
                    }
                }
                else
                {
                    switch (effectType)
                    {
                        case Enums.eEffectType.Damage:
                        case Enums.eEffectType.DamageBuff:
                        case Enums.eEffectType.Defense:
                        case Enums.eEffectType.Resistance:
                        case Enums.eEffectType.Elusivity:
                            ret = Enum.TryParse(selectedVector, out damageType);
                            if (!ret) damageType = Enums.eDamage.None;

                            break;

                        case Enums.eEffectType.Mez:
                        case Enums.eEffectType.MezResist:
                            ret = Enum.TryParse(selectedVector, out mezType);
                            if (!ret) mezType = Enums.eMez.None;

                            break;

                        case Enums.eEffectType.ResEffect:
                        case Enums.eEffectType.Enhancement:
                            ret = Enum.TryParse(selectedVector, out etModifies);
                            if (!ret) etModifies = Enums.eEffectType.None;

                            break;
                    }
                }
            }

            return new fxIdentifier
            {
                EffectType = effectType,
                DamageType = damageType,
                MezType = mezType,
                ETModifies = etModifies,
                SubMezType = mezTypeSub,
            };
        }


        private void FillMagList()
        {
            if (lvBonus.SelectedItems.Count < 1)
            {
                lvMag.Items.Clear();
            }
            else
            {
                var filterFxIdentifier = BuildFilterFxIdentifier();
                lvMag.BeginUpdate();
                lvMag.Items.Clear();
                if (EffectsMapContainsPartial(filterFxIdentifier))
                {
                    if (EffectsMap.ContainsKey(filterFxIdentifier))
                    {
                        lvMag.Items.Add("All");
                        var mags = EffectsMap[filterFxIdentifier].Keys
                            .OrderBy(e => Convert.ToSingle(e.TrimEnd('%')))
                            .ToList();
                        foreach (var mag in mags)
                        {
                            lvMag.Items.Add(new ListViewItem(mag));
                        }
                    }
                    else
                    {
                        lvMag.Items.Add("All");
                        var mags = filterFxIdentifier.EffectType == Enums.eEffectType.Enhancement & filterFxIdentifier.ETModifies == Enums.eEffectType.Mez
                            ? EffectsMap
                                .Where(e => e.Key.EffectType == filterFxIdentifier.EffectType & e.Key.ETModifies == filterFxIdentifier.ETModifies)
                                .SelectMany(e => e.Value.Keys)
                                .Distinct()
                                .OrderBy(e => Convert.ToSingle(e.TrimEnd('%')))
                                .ToList()
                            : EffectsMap
                                .Where(e => e.Key.EffectType == filterFxIdentifier.EffectType)
                                .SelectMany(e => e.Value.Keys)
                                .Distinct()
                                .OrderBy(e => Convert.ToSingle(e.TrimEnd('%')))
                                .ToList();

                        foreach (var mag in mags)
                        {
                            lvMag.Items.Add(new ListViewItem(mag));
                        }
                    }
                }
                else
                {
                    lvMag.Items.Add("None");
                }

                if (lvMag.Items.Count > 0)
                    lvMag.Items[0].Selected = true;
                lvMag.EndUpdate();
            }
        }

        private void FillSetList()
        {
            if ((lvBonus.SelectedItems.Count < 1) | (lvMag.SelectedItems.Count < 1))
            {
                lvSet.Items.Clear();
            }
            else
            {
                lvSet.BeginUpdate();
                lvSet.Items.Clear();

                var mag = lvMag.SelectedItems[0].Text == "All" | lvMag.SelectedItems[0].Text == "None"
                    ? ""
                    : lvMag.SelectedItems[0].Text;

                var filterFxIdentifier = BuildFilterFxIdentifier();
                if (EffectsMapContainsPartial(filterFxIdentifier))
                {
                    var sets = new List<KeyValuePair<int, int>>();
                    if (mag == "")
                    {
                        sets = EffectsMap
                            .Where(e => (e.Key.EffectType == filterFxIdentifier.EffectType | filterFxIdentifier.EffectType == Enums.eEffectType.None) &
                                        (e.Key.DamageType == filterFxIdentifier.DamageType | filterFxIdentifier.DamageType == Enums.eDamage.None) &
                                        (e.Key.ETModifies == filterFxIdentifier.ETModifies | (filterFxIdentifier.ETModifies == Enums.eEffectType.None &
                                          filterFxIdentifier.EffectType != Enums.eEffectType.Enhancement)) &
                                        (e.Key.MezType == filterFxIdentifier.MezType | filterFxIdentifier.MezType == Enums.eMez.None) &
                                        (e.Key.SubMezType == filterFxIdentifier.SubMezType | filterFxIdentifier.SubMezType == Enums.eMez.None))
                            .Select(e => e.Value
                                .SelectMany(k => k.Value))
                            .SelectMany(e => e)
                            .ToList();
                    }
                    else
                    {
                        sets = EffectsMap
                            .Where(e => (e.Key.EffectType == filterFxIdentifier.EffectType | filterFxIdentifier.EffectType == Enums.eEffectType.None) &
                                        (e.Key.DamageType == filterFxIdentifier.DamageType | filterFxIdentifier.DamageType == Enums.eDamage.None) &
                                        (e.Key.ETModifies == filterFxIdentifier.ETModifies | (filterFxIdentifier.ETModifies == Enums.eEffectType.None &
                                          filterFxIdentifier.EffectType != Enums.eEffectType.Enhancement)) &
                                        (e.Key.MezType == filterFxIdentifier.MezType | filterFxIdentifier.MezType == Enums.eMez.None) &
                                        (e.Key.SubMezType == filterFxIdentifier.SubMezType | filterFxIdentifier.SubMezType == Enums.eMez.None) &
                                        e.Value.Keys.Contains(mag))
                            .Select(e => e.Value
                                .Where(k => k.Key == mag)
                                .SelectMany(k => k.Value))
                            .SelectMany(e => e)
                            .ToList();
                    }

                    foreach (var set in sets)
                    {
                        AddEnhancementSet(set.Key, set.Value);
                    }
                }

                if (lvSet.Items.Count > 0)
                        lvSet.Items[0].Selected = true;
                lvSet.EndUpdate();
            }
        }

        private void frmSetFind_FormClosed(object sender, FormClosedEventArgs e)
        {
            myParent.FloatSetFinder(false);
        }

        private void SetImageButtonStyle(ImageButton ib)
        {
            ib.IA = myParent.Drawing.pImageAttributes;
            ib.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ib.ImageOn = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[3].Bitmap
                : myParent.Drawing.bxPower[5].Bitmap;
        }

        private void FillArchetypesList()
        {
            //var ignoredClasses = Archetype.GetNpcClasses();
            //var classesList = DatabaseAPI.Database.Classes.Select(at => at.DisplayName);
            //var playerClasses = classesList.Except(ignoredClasses);
            var playerClasses = DatabaseAPI.Database.Classes
                .Where(at => at.Playable)
                .Select(at => at.DisplayName);

            cbArchetype.BeginUpdate();
            cbArchetype.Items.Clear();
            cbArchetype.Items.Add("--Archetype--");
            foreach (var c in playerClasses)
            {
                cbArchetype.Items.Add(c);
            }

            cbArchetype.EndUpdate();
        }

        private string GetPowerString(int nIDPower)
        {
            var str1 = "";
            var returnString = "";
            var returnMask = Array.Empty<int>();
            DatabaseAPI.Database.Power[nIDPower]
                .GetEffectStringGrouped(0, ref returnString, ref returnMask, true, true, true);
            if (returnString != "")
            {
                return returnString;
            }

            for (var index1 = 0; index1 < DatabaseAPI.Database.Power[nIDPower].Effects.Length; index1++)
            {
                var flag = false;
                foreach (var m in returnMask)
                {
                    if (index1 == m)
                        flag = true;
                }

                if (flag)
                    continue;
                if (str1 != "")
                    str1 += ", ";
                var str3 = DatabaseAPI.Database.Power[nIDPower].Effects[index1].BuildEffectString(true, "", true).Trim();
                if (str3.Contains("Res("))
                    str3 = str3.Replace("Res(", "Resistance(");
                if (str3.Contains("Def("))
                    str3 = str3.Replace("Def(", "Defense(");
                if (str3.Contains("EndRec"))
                    str3 = str3.Replace("EndRec", "Recovery");
                if (str3.Contains("Endurance"))
                    str3 = str3.Replace("Endurance", "Max End");
                else if (str3.Contains("End") & !str3.Contains("Max End"))
                    str3 = str3.Replace("End", "Max End");
                str1 += str3;
            }

            return str1;
        }

        private void ibClose_ButtonClicked()
        {
            Close();
        }

        private void ibTopmost_ButtonClicked()
        {
            TopMost = ibTopmost.Checked;
            if (!TopMost)
                return;
            BringToFront();
        }

        private void UpdateEffectSubAttribList(out bool hasSubs)
        {
            lvVector.BeginUpdate();
            lvVector.Items.Clear();
            var vectorsList = new List<string>();
            var disallowedFxStrings = DisallowedEffectTypes
                .Select(t => t.ToString())
                .ToList();
            var hasSubEffectsFxStrings = HasSubsEffectTypes
                .Select(t => t.ToString())
                .ToList();
            var effectType = Enums.eEffectType.None;

            if (lvBonus.SelectedItems.Count > 0)
            {
                Enum.TryParse(lvBonus.SelectedItems[0].Text, out effectType);
            }

            switch (effectType)
            {
                case Enums.eEffectType.Damage:
                case Enums.eEffectType.DamageBuff:
                case Enums.eEffectType.Defense:
                case Enums.eEffectType.Resistance:
                case Enums.eEffectType.Elusivity:
                    vectorsList = Enum.GetNames(typeof(Enums.eDamage)).ToList();
                    lvVector.Columns[0].Text = "Damage Type / Vector";

                    break;

                case Enums.eEffectType.Mez:
                case Enums.eEffectType.MezResist:
                    vectorsList = Enum.GetNames(typeof(Enums.eMez)).ToList();
                    lvVector.Columns[0].Text = "Mez Type";

                    break;

                case Enums.eEffectType.ResEffect:
                    
                    vectorsList = Enum.GetNames(typeof(Enums.eEffectType))
                        .Where(v => !disallowedFxStrings.Contains(v) & !hasSubEffectsFxStrings.Contains(v))
                        .ToList();
                    lvVector.Columns[0].Text = "Effect Type";

                    break;

                case Enums.eEffectType.Enhancement:
                    vectorsList = Enum.GetNames(typeof(Enums.eEffectType))
                        .Where(v => !disallowedFxStrings.Contains(v) & !hasSubEffectsFxStrings.Contains(v))
                        .ToList();
                    lvVector.Columns[0].Text = "Effect Type";

                    break;

                default:
                    lvVector.Columns[0].Text = "";
                    
                    break;
            }

            if (vectorsList.Count > 0)
            {
                foreach (var v in vectorsList)
                {
                    // Coalesce Mezzes so there is no need for a subsub attribute
                    if (v == "Mez" & (effectType == Enums.eEffectType.ResEffect |
                                      effectType == Enums.eEffectType.Enhancement))
                    {
                        var subVectors = Enum.GetNames(typeof(Enums.eMez)).ToList();
                        foreach (var sv in subVectors)
                        {
                            lvVector.Items.Add($"Mez({sv})");
                        }

                    }
                    else
                    {
                        lvVector.Items.Add(v.Replace("None", "None / All"));
                    }
                }

                lvVector.Enabled = true;
                lvVector.Items[0].Selected = true;
                lvVector.Items[0].EnsureVisible();
                hasSubs = true;
            }
            else
            {
                lvVector.Enabled = false;
                hasSubs = false;
            }

            lvVector.EndUpdate();
        }

        [DebuggerStepThrough]
        private void lvBonus_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateEffectSubAttribList(out var hasSubs);
            if (!hasSubs)
            {
                FillMagList();
            }
        }

        private void lvVector_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillMagList();
        }

        private void lvMag_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillSetList();
        }

        private void lvSet_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSet.SelectedItems.Count <= 0) return;

            var sIdx = Convert.ToInt32(lvSet.SelectedItems[0].Tag);
            SetInfo.SetPopup(Character.PopSetInfo(sIdx));
            FillMatchingPowers(sIdx);
        }

        private bool IsPeacebringerInherent(IPower power)
        {
            return power.GetPowerSet().SetName == "Inherent" &
                   (power.PowerName == "Energy_Flight" |
                    power.PowerName == "Combat_Flight" |
                    power.PowerName == "Quantum_Acceleration" |
                    power.PowerName.StartsWith("Bright_Nova") |
                    power.PowerName.StartsWith("White_Dwarf"));
        }

        private bool IsWarshadeInherent(IPower power)
        {
            return power.GetPowerSet().SetName == "Inherent" &
                   (power.PowerName == "Shadow_Recall" |
                    power.PowerName == "Shadow_Step" |
                    power.PowerName.StartsWith("Dark_Nova") |
                    power.PowerName.StartsWith("Black_Dwarf"));
        }


        private void FillMatchingPowers(int sIdx)
        {
            var enhSet = DatabaseAPI.Database.EnhancementSets[sIdx];
            var powerSetsIconsDict = new Dictionary<string, int>();
            var atIconsDict = new Dictionary<string, int>();
            var imgIdx = 0;

            var setGroup = enhSet.SetType;
            var atClass = cbArchetype.SelectedIndex < 1
                ? null
                : DatabaseAPI.GetArchetypeByName(cbArchetype.Items[cbArchetype.SelectedIndex].ToString());

            var matchingPowers = DatabaseAPI.Database.Power
                .Where(p => p.SetTypes.Contains(setGroup) &
                            !p.HiddenPower &
                            !p.FullName.StartsWith("Pets.") &
                            !p.FullName.StartsWith("Villain_Pets."))
                .ToList();

            if (matchingPowers.Count <= 0)
            {
                lvPowers.BeginUpdate();
                lvPowers.Items.Clear();
                lvPowers.EndUpdate();

                return;
            }

            var matchingPowersets = matchingPowers
                .Select(p => p.GetPowerSet())
                .Distinct()
                .ToList();
            
            var matchingPowersetsIdx = matchingPowersets
                .Select(ps => Array.IndexOf(DatabaseAPI.Database.Powersets, ps))
                .ToList();
            
            /*var matchingArchetypes = matchingPowers
                .Select(p => DatabaseAPI.Database.Classes
                    .FirstOrDefault(at => DatabaseAPI.Database.Powersets[p.PowerSetID].ATClass == at.ClassName))
                .Distinct()
                .ToList();
            matchingArchetypes =
                matchingArchetypes
                    .Union(new[] {"Peacebringer", "Warshade"}.Select(DatabaseAPI.GetArchetypeByName))
                    .Distinct()
                    .ToList();*/

            var matchingArchetypes = DatabaseAPI.Database.Classes
                .Where(at => at.Playable)
                .ToList();
            
            var matchingArchetypesIdx = matchingArchetypes
                .Select(at => Array.IndexOf(DatabaseAPI.Database.Classes, at))
                .ToList();

            var imgList = new ImageList {ColorDepth = ColorDepth.Depth32Bit, ImageSize = new Size(18, 16)};
            for (var i = 0 ; i < matchingPowersets.Count ; i++)
            {
                var idx = matchingPowersetsIdx[i];
                var icon = new Bitmap(18, 16);
                using (var g = Graphics.FromImage(icon))
                {
                    if (idx < 0)
                    {
                        g.DrawImage(I9Gfx.UnknownPowerset.Bitmap,
                            new Rectangle(0, 0, 16, 16),
                            new Rectangle(0, 0, 16, 16),
                            GraphicsUnit.Pixel);
                    }
                    else
                    {
                        // Weird offset... Icons appear cropped if pasted at (0, 0)
                        g.DrawImage(I9Gfx.Powersets.Bitmap,
                            new Rectangle(2, 0, 18, 16),
                            new Rectangle(idx * 16, 0, 18, 16),
                            GraphicsUnit.Pixel);
                    }

                    imgList.Images.Add(icon);
                }

                powerSetsIconsDict.Add(matchingPowersets[i].FullName, imgIdx);

                imgIdx++;
            }

            var n = matchingArchetypes.Count;
            for (var i = 0; i < n + 1; i++)
            {
                var idx = i < n ? matchingArchetypesIdx[i] : -1;
                var icon = new Bitmap(18, 16);
                using (var g = Graphics.FromImage(icon))
                {
                    if (idx < 0 | i == n)
                    {
                        g.DrawImage(I9Gfx.UnknownArchetype.Bitmap,
                            new Rectangle(0, 0, 16, 16),
                            new Rectangle(0, 0, 16, 16),
                            GraphicsUnit.Pixel);
                    }
                    else
                    {
                        g.DrawImage(I9Gfx.Archetypes.Bitmap,
                            new Rectangle(0, 0, 16, 16),
                            new Rectangle(idx * 16, 0, 16, 16),
                            GraphicsUnit.Pixel);
                    }
                }

                imgList.Images.Add(icon);
                atIconsDict.Add(i == n
                    ? "unknown_generic"
                    : matchingArchetypes[i] == null
                        ? "unknown_generic"
                        : matchingArchetypes[i].ClassName, imgIdx);

                imgIdx++;
            }

            lvPowers.BeginUpdate();
            lvPowers.Items.Clear();
            lvPowers.SmallImageList = imgList;
            var selectedClassIndex = atClass == null ? -1 : Array.IndexOf(DatabaseAPI.Database.Classes, atClass);
            var epicPowersets = DatabaseAPI.GetEpicPowersets(atClass == null ? "" : atClass.ClassName);
            var lvRow = 0;
            foreach (var p in matchingPowers)
            {
                var powerSetData = p.GetPowerSet();
                var powerSetGroup = DatabaseAPI.Database.PowersetGroups[powerSetData.GroupName].Name;
                var allowedClasses = powerSetGroup == "Inherent" | powerSetGroup == "Epic" | powerSetGroup == "Pool"
                    ? new List<string>()
                    : powerSetData.GetArchetypes();
                
                var atClassFull = atClass == null
                    ? allowedClasses.Count > 0
                        ? DatabaseAPI.GetArchetypeByClassName(allowedClasses[0])
                        : null
                    : allowedClasses.Count > 0
                        ? allowedClasses.Contains(atClass.ClassName)
                            ? atClass
                            : DatabaseAPI.GetArchetypeByClassName(allowedClasses[0])
                        : null;

                var allowedForClass = atClass == null || p.AllowedForClass(selectedClassIndex);
                if (!allowedForClass) continue;

                if (atClass != null & powerSetGroup == "Epic" & epicPowersets.Count > 0)
                {
                    if (!epicPowersets.Contains(powerSetData)) continue;
                }

                if (atClassFull != null & atClass != null)
                {
                    if (atClassFull.ClassName != atClass.ClassName) continue;
                    if (atClass.ClassName != "Class_Peacebringer" & IsPeacebringerInherent(p)) continue;
                    if (atClass.ClassName != "Class_Warshade" & IsWarshadeInherent(p)) continue;
                }

                // Show Peacebringer/Warshade AT icon for Dwarves/Novas sub-powers,
                // and other PB/WS inherents
                var overrideAtClass = "";
                if (powerSetGroup == "Inherent")
                {
                    if (IsPeacebringerInherent(p))
                    {
                        overrideAtClass = "Class_Peacebringer";
                    }
                    else if (IsWarshadeInherent(p))
                    {
                        overrideAtClass = "Class_Warshade";
                    }
                }

                var atIconKey = overrideAtClass != ""
                    ? overrideAtClass
                    : atClassFull == null | powerSetGroup == "Epic" | powerSetGroup == "Inherent" | powerSetGroup == "Pool"
                        ? "unknown_generic"
                        : atClassFull.ClassName;

                // Column 0 item text goes into the constructor.
                // Column 1-2 items text go into lvItem.SubItems .
                var lvItem = new ListViewItem(powerSetGroup, atIconsDict[atIconKey]);
                lvItem.SubItems.AddRange(new[] {powerSetData.SetName, p.DisplayName});
                lvPowers.Items.Add(lvItem);

                //lvPowers.AddIconToSubItem(lvRow, 0, atIconsDict[atIconKey]);
                lvPowers.AddIconToSubItem(lvRow, 1, powerSetsIconsDict[powerSetData.FullName]);

                lvRow++;
            }

            lvPowers.ShowSubItemIcons();
            lvPowers.EndUpdate();
        }

        private void ibSelAt_ButtonClicked()
        {
            var selectedArchetype = myParent.GetSelectedArchetype();
            if (selectedArchetype == "")
            {
                cbArchetype.SelectedIndex = 0;

                return;
            }

            var n = cbArchetype.Items.Count;
            for (var i = 1; i < n; i++)
            {
                if (selectedArchetype != cbArchetype.Items[i].ToString()) continue;

                cbArchetype.SelectedIndex = i;

                return;
            }
        }

        private void cbArchetype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSet.SelectedItems.Count <= 0) return;

            var sIdx = Convert.ToInt32(lvSet.SelectedItems[0].Tag);
            FillMatchingPowers(sIdx);
        }
    }
}