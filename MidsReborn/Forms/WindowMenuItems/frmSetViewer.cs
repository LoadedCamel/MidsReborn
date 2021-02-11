using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Display;
using mrbBase.Base.Master_Classes;
using mrbControls;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmSetViewer : Form
    {
        #region FXIdentifier sub-class
        private class FXIdentifierKey
        {
            public Enums.eEffectType EffectType { get; set; }
            public Enums.eMez MezType { get; set; }
            public Enums.eDamage DamageType { get; set; }
            public Enums.eEffectType TargetEffectType { get; set; }

            public Enums.eFXGroup L1Group
            {
                get
                {
                    if (EffectType == Enums.eEffectType.Damage |
                        EffectType == Enums.eEffectType.DamageBuff |
                        EffectType == Enums.eEffectType.Accuracy |
                        EffectType == Enums.eEffectType.ToHit |
                        EffectType == Enums.eEffectType.RechargeTime |
                        EffectType == Enums.eEffectType.Range |
                        EffectType == Enums.eEffectType.Enhancement &
                        (TargetEffectType == Enums.eEffectType.RechargeTime |
                         TargetEffectType == Enums.eEffectType.Accuracy))
                    {
                        return Enums.eFXGroup.Offense;
                    }

                    if (EffectType == Enums.eEffectType.Regeneration |
                        EffectType == Enums.eEffectType.HitPoints |
                        EffectType == Enums.eEffectType.Absorb |
                        EffectType == Enums.eEffectType.Recovery |
                        EffectType == Enums.eEffectType.Endurance |
                        EffectType == Enums.eEffectType.EnduranceDiscount)
                    {
                        return Enums.eFXGroup.Survival;
                    }

                    if (EffectType == Enums.eEffectType.Mez |
                        EffectType == Enums.eEffectType.MezResist |
                        EffectType == Enums.eEffectType.Slow |
                        EffectType == Enums.eEffectType.ResEffect)
                    {
                        return Enums.eFXGroup.StatusEffects;
                    }

                    if (EffectType == Enums.eEffectType.SpeedRunning |
                        EffectType == Enums.eEffectType.MaxRunSpeed |
                        EffectType == Enums.eEffectType.SpeedJumping |
                        EffectType == Enums.eEffectType.JumpHeight |
                        EffectType == Enums.eEffectType.MaxJumpSpeed |
                        EffectType == Enums.eEffectType.SpeedFlying |
                        EffectType == Enums.eEffectType.MaxFlySpeed)
                    {
                        return Enums.eFXGroup.Movement;
                    }

                    if (EffectType == Enums.eEffectType.StealthRadius |
                        EffectType == Enums.eEffectType.StealthRadiusPlayer |
                        EffectType == Enums.eEffectType.PerceptionRadius)
                    {
                        return Enums.eFXGroup.Perception;
                    }

                    if (EffectType == Enums.eEffectType.Resistance |
                        EffectType == Enums.eEffectType.Defense |
                        EffectType == Enums.eEffectType.Elusivity)
                    {
                        return Enums.eFXGroup.Defense;
                    }

                    return Enums.eFXGroup.Misc;
                }
            }

            public Enums.eFXSubGroup L2Group
            {
                get
                {
                    if (EffectType == Enums.eEffectType.DamageBuff)
                    {
                        return Enums.eFXSubGroup.DamageAll;
                    }

                    if (EffectType == Enums.eEffectType.MezResist &
                        (MezType == Enums.eMez.Sleep |
                         MezType == Enums.eMez.Stunned |
                         MezType == Enums.eMez.Held |
                         MezType == Enums.eMez.Immobilized |
                         MezType == Enums.eMez.Confused |
                         MezType == Enums.eMez.Terrorized))
                    {
                        return Enums.eFXSubGroup.MezResistAll;
                    }

                    if (EffectType == Enums.eEffectType.Defense &
                        (DamageType == Enums.eDamage.Smashing | DamageType == Enums.eDamage.Lethal))
                    {
                        return Enums.eFXSubGroup.SmashLethalDefense;
                    }

                    if (EffectType == Enums.eEffectType.Defense &
                        (DamageType == Enums.eDamage.Fire | DamageType == Enums.eDamage.Cold))
                    {
                        return Enums.eFXSubGroup.FireColdDefense;
                    }

                    if (EffectType == Enums.eEffectType.Defense &
                        (DamageType == Enums.eDamage.Energy | DamageType == Enums.eDamage.Negative))
                    {
                        return Enums.eFXSubGroup.EnergyNegativeDefense;
                    }

                    if (EffectType == Enums.eEffectType.Resistance &
                        (DamageType == Enums.eDamage.Smashing | DamageType == Enums.eDamage.Lethal))
                    {
                        return Enums.eFXSubGroup.SmashLethalResistance;
                    }

                    if (EffectType == Enums.eEffectType.Resistance &
                        (DamageType == Enums.eDamage.Fire | DamageType == Enums.eDamage.Cold))
                    {
                        return Enums.eFXSubGroup.FireColdResistance;
                    }

                    if (EffectType == Enums.eEffectType.Resistance &
                        (DamageType == Enums.eDamage.Energy | DamageType == Enums.eDamage.Negative))
                    {
                        return Enums.eFXSubGroup.EnergyNegativeResistance;
                    }

                    if (EffectType == Enums.eEffectType.MezResist &
                        (TargetEffectType == Enums.eEffectType.SpeedRunning |
                         TargetEffectType == Enums.eEffectType.SpeedJumping |
                         TargetEffectType == Enums.eEffectType.SpeedFlying |
                         TargetEffectType == Enums.eEffectType.RechargeTime))
                    {
                        return Enums.eFXSubGroup.SlowResistance;
                    }

                    if (EffectType == Enums.eEffectType.Enhancement & TargetEffectType == Enums.eEffectType.Slow) // ???
                    {
                        return Enums.eFXSubGroup.SlowBuffs;
                    }

                    if (EffectType == Enums.eEffectType.MezResist &
                        (MezType == Enums.eMez.Knockback | MezType == Enums.eMez.Knockup))
                    {
                        return Enums.eFXSubGroup.KnockResistance;
                    }

                    return Enums.eFXSubGroup.NoGroup;
                }
            }

            public string L2GroupText()
            {
                return L2Group switch
                {
                    Enums.eFXSubGroup.DamageAll => "Damage(All)",
                    Enums.eFXSubGroup.MezResistAll => "MezResist(All)",
                    Enums.eFXSubGroup.SmashLethalDefense => "S/L Defense",
                    Enums.eFXSubGroup.FireColdDefense => "Fire/Cold Defense",
                    Enums.eFXSubGroup.EnergyNegativeDefense => "Energy/Negative Defense",
                    Enums.eFXSubGroup.SmashLethalResistance => "S/L Resistance",
                    Enums.eFXSubGroup.FireColdResistance => "Fire/Cold Resistance",
                    Enums.eFXSubGroup.EnergyNegativeResistance => "Energy/Negative Resistance",
                    Enums.eFXSubGroup.SlowResistance => "Slow Resistance",
                    Enums.eFXSubGroup.SlowBuffs => "Slows",
                    Enums.eFXSubGroup.KnockProtection => "Knock Protection",
                    Enums.eFXSubGroup.KnockResistance => "Knock Resistance",
                    Enums.eFXSubGroup.Jump => "Jump",
                    _ => "No group"
                };
            }

            public override bool Equals(object obj)
            {
                if (obj is FXIdentifierKey o)
                {
                    return (EffectType == o.EffectType &
                            MezType == o.MezType &
                            DamageType == o.DamageType &
                            TargetEffectType == o.TargetEffectType);
                }

                return false;
            }

            public override int GetHashCode()
            {
                return EffectType.GetHashCode() ^ MezType.GetHashCode() ^ DamageType.GetHashCode() ^ TargetEffectType.GetHashCode();
            }
        }
        #endregion

        #region FXSourceData sub-class
        private class FXSourceData
        {
            private IEffect _fx;
            public IEffect Fx
            {
                get => _fx;
                set => _fx = (IEffect) value.Clone();
            }
            public float Mag { get; set; }
            public string EnhSet { get; set; }
            public string Power { get; set; }
            public Enums.ePvX PvMode { get; set; }
            public bool IsFromEnh { get; set; }
        }
        #endregion

        private readonly frmMain myParent;
        private ImageButton btnClose;
        private ImageButton btnSmall;
        private ImageButton chkOnTop;
        private ColumnHeader ColumnHeader1;
        private ColumnHeader ColumnHeader2;
        private ColumnHeader ColumnHeader3;
        private ImageList ilSet;
        private Label Label1;
        private Label Label2;

        private ListView lstSets;
        private RichTextBox rtApplied;
        private RichTextBox rtxtFX;
        private RichTextBox rtxtInfo;

        private Dictionary<string, FXIdentifierKey> BarsFX;

        public frmSetViewer(frmMain iParent)
        {
            Move += frmSetViewer_Move;
            FormClosed += frmSetViewer_FormClosed;
            Load += frmSetViewer_Load;
            InitializeComponent();
            //var componentResourceManager = new ComponentResourceManager(typeof(frmSetViewer));
            Icon = Resources.reborn;
            Name = nameof(frmSetViewer);
            myParent = iParent;
            BarsFX = new Dictionary<string, FXIdentifierKey>();
        }

        private void btnClose_Click()
        {
            Close();
        }

        private void btnSmall_Click()
        {
            if (Width > 600)
            {
                Width = 387;
                rtxtInfo.Height = btnClose.Top - (rtxtInfo.Top + 8);
                btnSmall.Left = rtxtInfo.Width + rtxtInfo.Left - btnSmall.Width;
                btnClose.Left = btnSmall.Left - (btnClose.Width + 8);
                chkOnTop.Left = btnClose.Left - (chkOnTop.Width + 4);
                chkOnTop.Top = (int) Math.Round(btnClose.Top + (btnClose.Height - chkOnTop.Height) / 2.0);
                btnSmall.TextOff = "Expand >>";
            }
            else
            {
                Width = 681;
                rtxtInfo.Height = 132;
                btnClose.Left = 558;
                btnClose.Top = 418;
                btnSmall.Left = 384;
                btnSmall.Top = 418;
                chkOnTop.Left = 558;
                chkOnTop.Top = 392;
                btnSmall.TextOff = "<< Shrink";
            }

            StoreLocation();
        }

        private void chkOnTop_CheckedChanged()
        {
            TopMost = chkOnTop.Checked;
        }

        private void DisplayList()
        {
            var items = new string[3];
            lstSets.BeginUpdate();
            lstSets.Items.Clear();
            var imageIndex = -1;
            FillImageList();
            foreach (var s in MidsContext.Character.CurrentBuild.SetBonus)
            {
                for (var index2 = 0; index2 < s.SetInfo.Length; index2++)
                {
                    var setInfo = s.SetInfo;
                    var index3 = index2;
                    items[0] = DatabaseAPI.Database.EnhancementSets[setInfo[index3].SetIDX].DisplayName;
                    items[1] =
                        MidsContext.Character.CurrentBuild
                            .Powers[s.PowerIndex]
                            .NIDPowerset <= -1
                            ? ""
                            : DatabaseAPI.Database
                                .Powersets[
                                    MidsContext.Character.CurrentBuild
                                        .Powers[s.PowerIndex]
                                        .NIDPowerset].Powers[
                                    MainModule.MidsController.Toon.CurrentBuild
                                        .Powers[s.PowerIndex]
                                        .IDXPower].DisplayName;
                    items[2] = Convert.ToString(setInfo[index3].SlottedCount);
                    imageIndex++;
                    lstSets.Items.Add(new ListViewItem(items, imageIndex));
                    lstSets.Items[lstSets.Items.Count - 1].Tag = setInfo[index3].SetIDX;
                }
            }

            lstSets.EndUpdate();
            if (lstSets.Items.Count > 0)
                lstSets.Items[0].Selected = true;
            FillEffectView();
        }

        private Dictionary<FXIdentifierKey, List<FXSourceData>> GetEffectSources()
        {
            var ret = new Dictionary<FXIdentifierKey, List<FXSourceData>>();
            
            foreach (var s in MidsContext.Character.CurrentBuild.SetBonus)
            {
                for (var i = 0; i < s.SetInfo.Length; i++)
                {
                    if (s.SetInfo[i].Powers.Length <= 0) continue;

                    var enhancementSet = DatabaseAPI.Database.EnhancementSets[s.SetInfo[i].SetIDX];
                    var powerName = DatabaseAPI.Database
                        .Powersets[MidsContext.Character.CurrentBuild.Powers[s.PowerIndex].NIDPowerset]
                        .Powers[MidsContext.Character.CurrentBuild.Powers[s.PowerIndex].IDXPower]
                        .DisplayName;

                    for (var j = 0; j < enhancementSet.Bonus.Length; j++)
                    {
                        var pvMode = enhancementSet.Bonus[j].PvMode;
                        if (!((s.SetInfo[i].SlottedCount >= enhancementSet.Bonus[j].Slotted) &
                              ((pvMode == Enums.ePvX.Any) |
                               ((pvMode == Enums.ePvX.PvE) & !MidsContext.Config.Inc.DisablePvE) |
                               ((pvMode == Enums.ePvX.PvP) & MidsContext.Config.Inc.DisablePvE))))
                            continue;

                        var setEffectsData = enhancementSet.GetEffectDetailedData(j, false);
                        foreach (var e in setEffectsData)
                        {
                            var identKey = new FXIdentifierKey
                            {
                                EffectType = e.EffectType,
                                MezType = e.MezType,
                                DamageType = e.DamageType,
                                TargetEffectType = e.ETModifies
                            };
                            
                            if (!ret.ContainsKey(identKey)) ret.Add(identKey, new List<FXSourceData>());
                            ret[identKey].Add(new FXSourceData
                            {
                                Fx = e,
                                Mag = e.Mag,
                                EnhSet = enhancementSet.DisplayName,
                                Power = powerName,
                                PvMode = pvMode,
                                IsFromEnh = false
                            });
                        }
                    }

                    foreach (var si in s.SetInfo[i].EnhIndexes)
                    {
                        var specialEnhIdx = DatabaseAPI.IsSpecialEnh(si);
                        if (specialEnhIdx <= -1) continue;
                        
                        var enhEffectsData = enhancementSet.GetEffectDetailedData(specialEnhIdx, true);
                        foreach (var e in enhEffectsData)
                        {
                            var identKey = new FXIdentifierKey
                            {
                                EffectType = e.EffectType,
                                MezType = e.MezType,
                                DamageType = e.DamageType,
                                TargetEffectType = e.ETModifies
                            };
                            
                            if (!ret.ContainsKey(identKey)) ret.Add(identKey, new List<FXSourceData>());
                            ret[identKey].Add(new FXSourceData
                            {
                                Fx = e,
                                Mag = e.Mag,
                                EnhSet = enhancementSet.DisplayName,
                                Power = powerName,
                                PvMode = Enums.ePvX.Any,
                                IsFromEnh = true
                            });
                        }
                    }
                }
            }

            foreach (var fxListSub in ret)
            {
                // Sort each type of buff, biggest one first
                fxListSub.Value.Sort((a, b) => -a.Mag.CompareTo(b.Mag));
            }

            // Sort groups by effect type, mez type, damage type and target effect type (e.g. for enhancement(something) )
            ret = ret
                .OrderBy(pair =>
                    $"{pair.Key.EffectType}{(int)pair.Key.MezType:0:000}{(int)pair.Key.DamageType:0:000}{(int)pair.Key.TargetEffectType:0:000}")
                .ToDictionary(x => x.Key, x => x.Value);

            return ret;
        }

        private void FillEffectView(bool getDetails = false)
        {
            var str1 = "";
            var numArray = new int[DatabaseAPI.NidPowers("set_bonus").Length];
            var hasOvercap = false;
            foreach (var s in MidsContext.Character.CurrentBuild.SetBonus)
            {
                for (var index2 = 0; index2 < s.SetInfo.Length; index2++)
                {
                    if (s.SetInfo[index2].Powers.Length <= 0) continue;

                    var setInfo = s.SetInfo;
                    var index3 = index2;
                    var enhancementSet = DatabaseAPI.Database.EnhancementSets[setInfo[index3].SetIDX];
                    var str2 = str1 + RTF.Color(RTF.ElementID.Invention) + RTF.Underline(RTF.Bold(enhancementSet.DisplayName));
                    if (MidsContext.Character.CurrentBuild.Powers[s.PowerIndex].NIDPowerset > -1)
                        str2 += RTF.Crlf() + RTF.Color(RTF.ElementID.Faded) + "(" + DatabaseAPI.Database
                            .Powersets[
                                MidsContext.Character.CurrentBuild
                                    .Powers[s.PowerIndex].NIDPowerset]
                            .Powers[
                                MidsContext.Character.CurrentBuild
                                    .Powers[s.PowerIndex].IDXPower]
                            .DisplayName + ")";
                    var str3 = str2 + RTF.Crlf() + RTF.Color(RTF.ElementID.Text);
                    var str4 = "";
                    for (var index4 = 0; index4 < enhancementSet.Bonus.Length; index4++)
                    {
                        if (!((setInfo[index3].SlottedCount >= enhancementSet.Bonus[index4].Slotted) &
                              ((enhancementSet.Bonus[index4].PvMode == Enums.ePvX.Any) |
                               ((enhancementSet.Bonus[index4].PvMode == Enums.ePvX.PvE) &
                                !MidsContext.Config.Inc.DisablePvE) |
                               ((enhancementSet.Bonus[index4].PvMode == Enums.ePvX.PvP) &
                                MidsContext.Config.Inc.DisablePvE))))
                            continue;
                        if (str4 != "") str4 += RTF.Crlf();
                        var localOverCap = false;
                        var str5 = "  " + enhancementSet.GetEffectString(index4, false, true);
                        foreach (var esb in enhancementSet.Bonus[index4].Index)
                        {
                            if (esb <= -1) continue;

                            numArray[esb]++;
                            if (numArray[esb] > 5)
                            {
                                localOverCap = true;
                            }
                        }

                        if (localOverCap)
                        {
                            str5 = RTF.Italic(RTF.Color(RTF.ElementID.Warning) + str5 + " >Cap" +
                                              RTF.Color(RTF.ElementID.Text));
                            hasOvercap = true;
                        }

                        str4 += str5;
                    }

                    foreach (var si in s.SetInfo[index2].EnhIndexes)
                    {
                        var index5 = DatabaseAPI.IsSpecialEnh(si);
                        if (index5 <= -1) continue;
                        if (str4 != "")
                            str4 += RTF.Crlf();
                        var str5 = str4 + RTF.Color(RTF.ElementID.Enhancement);
                        var localOverCap = false;
                        var str6 = "  " + DatabaseAPI.Database
                            .EnhancementSets[s.SetInfo[index2].SetIDX]
                            .GetEffectString(index5, true, true);
                        foreach (var sb in DatabaseAPI.Database.EnhancementSets[s.SetInfo[index2].SetIDX].SpecialBonus[index5].Index)
                        {
                            if (sb <= -1) continue;
                            numArray[sb]++;
                            if (numArray[sb] > 5)
                            {
                                localOverCap = true;
                            }
                        }

                        if (localOverCap)
                        {
                            str6 = RTF.Italic(RTF.Color(RTF.ElementID.Warning) + str6 + " >Cap" + RTF.Color(RTF.ElementID.Text));
                            hasOvercap = true;
                        }

                        str4 = str5 + str6;
                    }

                    str1 = str3 + str4 + RTF.Crlf() + RTF.Crlf();
                }
            }

            var str7 = hasOvercap
                ? RTF.Color(RTF.ElementID.Invention) + RTF.Underline(RTF.Bold("Information:")) + RTF.Crlf() +
                  RTF.Color(RTF.ElementID.Text) +
                  "One or more set bonuses have exceeded the 5 bonus cap, and will not affect your stats. Scroll down this list to find bonuses marked as '" +
                  RTF.Italic(RTF.Color(RTF.ElementID.Warning) + ">Cap") + RTF.Color(RTF.ElementID.Text) + "'" +
                  RTF.Crlf() + RTF.Crlf()
                : "";
            var str8 = RTF.StartRTF() + str7 + str1 + RTF.EndRTF();
            if (rtxtFX.Rtf != str8) rtxtFX.Rtf = str8;
            var iStr = "";
            if (!getDetails)
            {
                var cumulativeSetBonuses = MidsContext.Character.CurrentBuild.GetCumulativeSetBonuses().ToArray();
                Array.Sort(cumulativeSetBonuses);

                foreach (var csb in cumulativeSetBonuses)
                {
                    if (iStr != "") iStr += RTF.Crlf();
                    var str2 = csb.BuildEffectString(true);
                    if (!str2.StartsWith("+")) str2 = "+" + str2;
                    str2 = Regex.Replace(str2, @"Endurance\b|Max End", "Max Endurance");
                    iStr += str2;
                }
            }
            else
            {
                var effectSources = GetEffectSources();
                var pickedGroups = new Dictionary<Enums.eFXSubGroup, bool>();
                foreach (var fxGroup in effectSources)
                {
                    if (fxGroup.Key.EffectType == Enums.eEffectType.GrantPower |
                        fxGroup.Key.EffectType == Enums.eEffectType.EntCreate |
                        fxGroup.Key.EffectType == Enums.eEffectType.EntCreate_x |
                        fxGroup.Key.EffectType == Enums.eEffectType.Null |
                        fxGroup.Key.EffectType == Enums.eEffectType.NullBool) continue;
                    
                    var L2Group = fxGroup.Key.L2Group;
                    if (L2Group != Enums.eFXSubGroup.NoGroup && pickedGroups.ContainsKey(L2Group)) continue;
                    if (L2Group != Enums.eFXSubGroup.NoGroup) pickedGroups.Add(L2Group, true);
                    var effectName = Enums.GetEffectName(fxGroup.Key.EffectType);

                    if (fxGroup.Key.MezType != Enums.eMez.None)
                    {
                        effectName += $"({Enums.GetMezName(fxGroup.Key.MezType)})";
                    }

                    if (fxGroup.Key.DamageType != Enums.eDamage.None)
                    {
                        effectName += $"({Enums.GetDamageName(fxGroup.Key.DamageType)})";
                    }

                    if (fxGroup.Key.TargetEffectType != Enums.eEffectType.None)
                    {
                        effectName += $"({Enums.GetEffectName(fxGroup.Key.TargetEffectType)})";
                    }

                    if (iStr != "") iStr += RTF.Crlf();
                    var fxTypePercent = fxGroup.Key.EffectType == Enums.eEffectType.Endurance ||
                                        effectSources[fxGroup.Key].Count > 0 && effectSources[fxGroup.Key].First().Fx.DisplayPercentage;
                    var fxSumMag = effectSources[fxGroup.Key].Count > 0
                        ? effectSources[fxGroup.Key].Sum(e => e.Mag)
                        : 0;

                    var fxBlockStr = "";
                    if (L2Group != Enums.eFXSubGroup.NoGroup)
                    {
                        fxBlockStr += fxGroup.Key.L2GroupText();
                    }
                    else
                    {
                        fxBlockStr += Regex.Replace(fxGroup.Key.EffectType.ToString(), @"Endurance\b|Max End", "Max Endurance");
                        fxBlockStr += fxGroup.Key.MezType != Enums.eMez.None ? $" ({fxGroup.Key.MezType})" : "";
                        fxBlockStr += fxGroup.Key.DamageType != Enums.eDamage.None ? $" ({fxGroup.Key.DamageType})" : "";
                        fxBlockStr += fxGroup.Key.TargetEffectType != Enums.eEffectType.None ? $" ({fxGroup.Key.TargetEffectType})" : "";
                    }

                    fxBlockStr += $" ({(fxTypePercent ? fxSumMag * (fxGroup.Key.EffectType == Enums.eEffectType.Endurance ? 1 : 100) : fxSumMag):##0.##}{(fxTypePercent ? "%" : "")})";

                    foreach (var e in effectSources[fxGroup.Key])
                    {
                        fxBlockStr += RTF.Crlf();
                        var effectString = L2Group != Enums.eFXSubGroup.NoGroup
                            ? e.Fx.BuildEffectString(true).Replace(effectName, fxGroup.Key.L2GroupText())
                            : e.Fx.BuildEffectString(true);
                        if (!effectString.StartsWith("+"))
                        {
                            effectString = "+" + effectString;
                        }

                        effectString = Regex.Replace(effectString, @"Endurance\b|Max End", "Max Endurance");

                        fxBlockStr += $"    {(e.PvMode == Enums.ePvX.PvP ? "[PVP] " : "")}{effectString}";
                        if (e.EnhSet != "" & e.Power != "" & !e.IsFromEnh)
                        {
                            fxBlockStr += $" ({e.EnhSet}, on {e.Power})";
                        }
                        else if (e.IsFromEnh)
                        {
                            fxBlockStr += $" (Enh. special, on {e.Power})";
                        }
                    }

                    iStr += fxBlockStr;
                }
            }

            var str9 = RTF.StartRTF() + RTF.ToRTF(iStr) + RTF.EndRTF();
            if (rtApplied.Rtf == str9)
                return;
            rtApplied.Rtf = str9;
        }

        private void FillImageList()
        {
            var imageSize1 = ilSet.ImageSize;
            var width1 = imageSize1.Width;
            imageSize1 = ilSet.ImageSize;
            var height1 = imageSize1.Height;
            var extendedBitmap = new ExtendedBitmap(width1, height1);
            ilSet.Images.Clear();
            foreach (var sb in MidsContext.Character.CurrentBuild.SetBonus)
            {
                for (var index2 = 0; index2 < sb.SetInfo.Length; index2++)
                {
                    if (sb.SetInfo[index2].SetIDX <= -1) continue;
                    var enhancementSet = DatabaseAPI.Database.EnhancementSets[sb.SetInfo[index2].SetIDX];
                    if (enhancementSet.ImageIdx > -1)
                    {
                        extendedBitmap.Graphics.Clear(Color.White);
                        var graphics = extendedBitmap.Graphics;
                        I9Gfx.DrawEnhancementSet(ref graphics, enhancementSet.ImageIdx);
                        ilSet.Images.Add(extendedBitmap.Bitmap);
                    }
                    else
                    {
                        var images = ilSet.Images;
                        var imageSize2 = ilSet.ImageSize;
                        var width2 = imageSize2.Width;
                        imageSize2 = ilSet.ImageSize;
                        var height2 = imageSize2.Height;
                        var bitmap = new Bitmap(width2, height2);
                        images.Add(bitmap);
                    }
                }
            }
        }

        private void Bar_Hover(object sender)
        {
            if (!(sender is ctlLayeredBarPb bar)) return;

            var barData = BarsFX[bar.Name];
            var barValues = bar.GetValues();

            var overlayVector = (barData.EffectType == Enums.eEffectType.Enhancement ? barData.TargetEffectType : barData.EffectType).ToString();
            overlayVector = overlayVector
                .Replace("Resistance", "Res")
                .Replace("Defense", "Def")
                .Replace("EnduranceDiscount", "End. Discount");
            overlayVector = Regex.Replace(overlayVector, @"Endurance\b", "Max End");
            var overlayDmgType = !(barData.EffectType == Enums.eEffectType.Resistance | barData.EffectType == Enums.eEffectType.Defense)
                ? ""
                : barData.DamageType switch
                {
                    Enums.eDamage.Smashing => "S/L",
                    Enums.eDamage.Fire => "Fire/Cold",
                    Enums.eDamage.Energy => "Energy/Neg",
                    _ => barData.DamageType.ToString()
                };
            var overlayValuePercent = barData.EffectType != Enums.eEffectType.HitPoints & barData.EffectType != Enums.eEffectType.Endurance;
            var plusSignEnabled = barData.EffectType == Enums.eEffectType.SpeedRunning ||
                                  barData.EffectType == Enums.eEffectType.SpeedJumping ||
                                  barData.EffectType == Enums.eEffectType.SpeedFlying;

            var ttext = $"{(overlayDmgType != "" ? overlayDmgType + " " : "")}{overlayVector}\r\nFrom sets: {(plusSignEnabled & barValues["setbuffs"] > 0 ? "+" : "")}{barValues["setbuffs"]:##0.##}{(overlayValuePercent ? "%" : "")}";
            if (barValues["totalsvalue"] > 0) // & Math.Abs(barValues["setbuffs"] - barValues["totalsvalue"]) > float.Epsilon)
            {
                ttext += $"\r\nTotal: {(plusSignEnabled ? "+" : "")}{barValues["totalsvalue"]:##0.##}{(overlayValuePercent ? "%" : "")}";
            }

            bar.SetTip(ttext);
        }

        private void frmSetViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            myParent.FloatSets(false);
        }

        private void frmSetViewer_Load(object sender, EventArgs e)
        {
            lstSets
                .GetType()
                .GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.SetValue(lstSets, true, null);
        }

        private void frmSetViewer_Move(object sender, EventArgs e)
        {
            StoreLocation();
        }

        private void lstSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSets.SelectedItems.Count < 1) return;
            rtxtInfo.Rtf = RTF.StartRTF() + EnhancementSetCollection.GetSetInfoLongRTF(
                Convert.ToInt32(lstSets.SelectedItems[0].Tag),
                Convert.ToInt32(lstSets.SelectedItems[0].SubItems[2].Text)) + RTF.EndRTF();
        }

        public void SetLocation()
        {
            var rectangle = new Rectangle
            {
                X = MainModule.MidsController.SzFrmSets.X, Y = MainModule.MidsController.SzFrmSets.Y
            };
            if (rectangle.X < 1)
            {
                rectangle.X = myParent.Left + 8;
            }

            if (rectangle.Y < 32)
            {
                rectangle.Y = myParent.Top + (myParent.Height - myParent.ClientSize.Height) +
                              myParent.GetPrimaryBottom();
            }

            if (MidsContext.Config.ShrinkFrmSets & (Width > 600) |
                !MidsContext.Config.ShrinkFrmSets & (Width < 600))
            {
                btnSmall_Click();
            }

            Top = rectangle.Y;
            Left = rectangle.X;
        }

        private void StoreLocation()
        {
            if (!MainModule.MidsController.IsAppInitialized) return;
            MainModule.MidsController.SzFrmSets.X = Left;
            MainModule.MidsController.SzFrmSets.Y = Top;
            MidsContext.Config.ShrinkFrmSets = Width < 600;
        }

        public void UpdateData()
        {
            if (myParent == null) return;

            BackColor = myParent.BackColor;
            if (rtApplied.BackColor != BackColor)
            {
                rtApplied.BackColor = BackColor;
            }

            if (rtxtFX.BackColor != BackColor)
            {
                rtxtFX.BackColor = BackColor;
            }

            if (rtxtInfo.BackColor != BackColor)
            {
                rtxtInfo.BackColor = BackColor;
            }

            var imageOffIdx = MidsContext.Character.IsHero() ? 2 : 4;
            var imageOnIdx = imageOffIdx + 1;

            btnClose.IA = myParent.Drawing.pImageAttributes;
            btnClose.ImageOff = myParent.Drawing.bxPower[imageOffIdx].Bitmap;
            btnClose.ImageOn = myParent.Drawing.bxPower[imageOnIdx].Bitmap;

            chkOnTop.IA = myParent.Drawing.pImageAttributes;
            chkOnTop.ImageOff = myParent.Drawing.bxPower[imageOffIdx].Bitmap;
            chkOnTop.ImageOn = myParent.Drawing.bxPower[imageOnIdx].Bitmap;

            btnSmall.IA = myParent.Drawing.pImageAttributes;
            btnSmall.ImageOff = myParent.Drawing.bxPower[imageOffIdx].Bitmap;
            btnSmall.ImageOn = myParent.Drawing.bxPower[imageOnIdx].Bitmap;

            btnDetailFx.IA = myParent.Drawing.pImageAttributes;
            btnDetailFx.ImageOff = myParent.Drawing.bxPower[imageOffIdx].Bitmap;
            btnDetailFx.ImageOn = myParent.Drawing.bxPower[imageOnIdx].Bitmap;

            DisplayList();
            DrawBars();
        }

        private class BarSettings
        {
            public Enums.eEffectType EffectType;
            public Enums.eEffectType TargetEffectType = Enums.eEffectType.None;
            public Enums.eDamage DamageType = Enums.eDamage.None;
            public Color SetBuffsColor;
            public Color TotalsColor;

            public BarSettings(Enums.eEffectType effectType, Color setBuffsColor, Color totalsColor)
            {
                EffectType = effectType;
                TargetEffectType = Enums.eEffectType.None;
                DamageType = Enums.eDamage.None;
                SetBuffsColor = setBuffsColor;
                TotalsColor = totalsColor;
            }

            public BarSettings(Enums.eEffectType effectType, Enums.eEffectType targetEffectType, Color setBuffsColor, Color totalsColor)
            {
                EffectType = effectType;
                TargetEffectType = targetEffectType;
                DamageType = Enums.eDamage.None;
                SetBuffsColor = setBuffsColor;
                TotalsColor = totalsColor;
            }

            public BarSettings(Enums.eEffectType effectType, Enums.eDamage damageType, Color setBuffsColor, Color totalsColor)
            {
                EffectType = effectType;
                TargetEffectType = Enums.eEffectType.None;
                DamageType = damageType;
                SetBuffsColor = setBuffsColor;
                TotalsColor = totalsColor;
            }
        }

        private int GetControlHeight(int numHeaders, int numBars, int barHeight)
        {
            return (barHeight + 3) * numBars + 20 * numHeaders;
        }

        private void DrawBars()
        {
            var cumulativeSetBonuses = MidsContext.Character.CurrentBuild.GetCumulativeSetBonuses();
            var displayStats = MidsContext.Character.DisplayStats;
            var effectListOrder = new List<BarSettings>
            {
                new BarSettings (Enums.eEffectType.DamageBuff, Color.Red, Color.FromArgb(204, 0, 0)),
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eEffectType.Accuracy, Color.Yellow, Color.FromArgb(204, 204, 0)),
                //new BarSettings (Enums.eEffectType.ToHit, Color.FromArgb(255, 255, 128), Color.FromArgb(204, 204, 102)),
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eEffectType.RechargeTime, Color.FromArgb(255, 128, 0), Color.FromArgb(204, 102, 0)),
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eEffectType.Range, Color.FromArgb(170, 168, 179), Color.FromArgb(121, 120, 128)),
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eEffectType.Heal, Color.FromArgb(116, 255, 116), Color.FromArgb(92, 204, 92)),

                new BarSettings (Enums.eEffectType.Regeneration, Color.FromArgb(64, 255, 64), Color.FromArgb(51, 204, 51)),
                new BarSettings (Enums.eEffectType.HitPoints, Color.FromArgb(44, 180, 44), Color.FromArgb(31, 130, 31)),
                //new BarSettings (Enums.eEffectType.Absorb, Color.Gainsboro, Color.FromArgb(168, 168, 168)),
                new BarSettings (Enums.eEffectType.Recovery, Color.DodgerBlue, Color.FromArgb(24, 114, 204)),
                new BarSettings (Enums.eEffectType.Endurance, Color.FromArgb(59, 158, 255), Color.FromArgb(47, 125, 204)),
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eEffectType.EnduranceDiscount, Color.RoyalBlue, Color.FromArgb(50, 81, 173)),

                new BarSettings (Enums.eEffectType.Resistance, Enums.eDamage.Smashing, Color.FromArgb(0, 192, 192), Color.FromArgb(0, 140, 140)),
                new BarSettings (Enums.eEffectType.Resistance, Enums.eDamage.Fire, Color.FromArgb(0, 192, 192), Color.FromArgb(0, 140, 140)),
                new BarSettings (Enums.eEffectType.Resistance, Enums.eDamage.Energy, Color.FromArgb(0, 192, 192), Color.FromArgb(0, 140, 140)),
                new BarSettings (Enums.eEffectType.Resistance, Enums.eDamage.Toxic, Color.FromArgb(0, 192, 192), Color.FromArgb(0, 140, 140)),
                new BarSettings (Enums.eEffectType.Resistance, Enums.eDamage.Psionic, Color.FromArgb(0, 192, 192), Color.FromArgb(0, 140, 140)),

                new BarSettings (Enums.eEffectType.Defense, Enums.eDamage.Smashing, Color.Magenta, Color.FromArgb(204, 0, 204)),
                new BarSettings (Enums.eEffectType.Defense, Enums.eDamage.Fire, Color.Magenta, Color.FromArgb(204, 0, 204)),
                new BarSettings (Enums.eEffectType.Defense, Enums.eDamage.Energy, Color.Magenta, Color.FromArgb(204, 0, 204)),
                new BarSettings (Enums.eEffectType.Defense, Enums.eDamage.Psionic, Color.Magenta, Color.FromArgb(204, 0, 204)),
                new BarSettings (Enums.eEffectType.Defense, Enums.eDamage.Melee, Color.Magenta, Color.FromArgb(204, 0, 204)),
                new BarSettings (Enums.eEffectType.Defense, Enums.eDamage.Ranged, Color.Magenta, Color.FromArgb(204, 0, 204)),
                new BarSettings (Enums.eEffectType.Defense, Enums.eDamage.AoE, Color.Magenta, Color.FromArgb(204, 0, 204)),
                new BarSettings (Enums.eEffectType.Elusivity, Color.FromArgb(163, 1, 231), Color.FromArgb(127, 1, 181)),

                //Enums.eEffectType.Mez,
                //Enums.eEffectType.MezResist,
                //Enums.eEffectType.Slow,
                //Enums.eEffectType.ResEffect,

                new BarSettings (Enums.eEffectType.SpeedRunning, Color.FromArgb(0, 192, 128), Color.FromArgb(0, 140, 94)),
                //new BarSettings (Enums.eEffectType.MaxRunSpeed, Color.FromArgb(0, 192, 128), Color.FromArgb(0, 140, 94)),
                new BarSettings (Enums.eEffectType.SpeedJumping, Color.FromArgb(0, 192, 128), Color.FromArgb(0, 140, 94)),
                //new BarSettings (Enums.eEffectType.JumpHeight, Color.FromArgb(0, 192, 128), Color.FromArgb(0, 140, 94)),
                //new BarSettings (Enums.eEffectType.MaxJumpSpeed, Color.FromArgb(0, 192, 128), Color.FromArgb(0, 140, 94)),
                new BarSettings (Enums.eEffectType.SpeedFlying, Color.FromArgb(0, 192, 128), Color.FromArgb(0, 140, 94)),
                //new BarSettings (Enums.eEffectType.MaxFlySpeed, Color.FromArgb(0, 192, 128), Color.FromArgb(0, 140, 94)),

                new BarSettings (Enums.eEffectType.StealthRadius, Color.FromArgb(106, 121, 136), Color.FromArgb(84, 95, 107)),
                new BarSettings (Enums.eEffectType.StealthRadiusPlayer, Color.FromArgb(106, 121, 136), Color.FromArgb(84, 95, 107)),
                new BarSettings (Enums.eEffectType.PerceptionRadius, Color.FromArgb(106, 121, 136), Color.FromArgb(84, 95, 107))
            };

            effectListOrder = effectListOrder
                .AsEnumerable()
                .OrderBy(e => (int) new FXIdentifierKey
                {
                    EffectType = e.EffectType,
                    TargetEffectType = e.TargetEffectType,
                    DamageType = e.DamageType,
                    MezType = Enums.eMez.None
                }.L1Group)
                .ToList();

            var l1Group = "";
            var nh = 0; // Nb of headers
            var nb = 0; // Nb of bars
            var yPos = 0;
            var offset = 0;
            const int hBar = 12;
            const int barLabelWidth = 100;
            BarsFX.Clear();
            panelBars.SuspendLayout();
            panelBars.Controls.Clear();
            foreach (var st in effectListOrder)
            {
                var fxId = new FXIdentifierKey
                {
                    EffectType = st.EffectType,
                    TargetEffectType = st.TargetEffectType,
                    DamageType = st.DamageType,
                    MezType = Enums.eMez.None
                };

                yPos = GetControlHeight(nh, nb, hBar);
                var fxL1Group = fxId.L1Group;
                var newHeader = false;
                var header = new Label();
                if (fxL1Group.ToString() != l1Group)
                {
                    if (nh > 0) offset += 6;
                    header.Location = new Point(0, yPos + offset);
                    header.ForeColor = Color.White;
                    header.BackColor = Color.Black; // Transparent
                    header.Font = new Font("Arial", 8.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
                    header.Size = new Size(panelBars.Size.Width - 20, 16);
                    header.Name = $"HeaderLabel{nh + 1}";
                    header.Text = fxL1Group.ToString();

                    newHeader = true;
                }

                var lBuffs = cumulativeSetBonuses.Where(e => e.EffectType == st.EffectType &
                                                             (st.DamageType == Enums.eDamage.None | st.DamageType == e.DamageType) &
                                                             (st.TargetEffectType == Enums.eEffectType.None | st.TargetEffectType == e.ETModifies)).ToList();

                if (lBuffs.Count <= 0) continue;

                if (newHeader)
                {
                    nh++;
                    panelBars.Controls.Add(header);
                    l1Group = fxL1Group.ToString();
                }

                yPos = GetControlHeight(nh, nb, hBar);
                var bar = new ctlLayeredBarPb
                {
                    Location = new Point(barLabelWidth + 8, yPos + offset),
                    Size = new Size(panelBars.Size.Width - barLabelWidth - 28, hBar),
                    Name = $"Bar{nb + 1}",
                    MaximumBarValue = 100,
                    OverlayTextFont = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Pixel, 0),
                    EnableOverlayText = true,
                    EnableOverlayOutline = true,
                };
                bar.BarHover += Bar_Hover;

                var barLabel = new Label
                {
                    Location = new Point(2, yPos + offset - 1),
                    Size = new Size(barLabelWidth, hBar + 2),
                    Name = $"BarLabel{nb + 1}",
                    TextAlign = ContentAlignment.TopLeft,
                    Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Pixel, 0),
                    ForeColor = Color.WhiteSmoke
                };

                bar.AssignNames(new List<(string name, Color color)>
                {
                    ("setbuffs", st.SetBuffsColor),
                    ("totalsvalue", st.TotalsColor)
                });
                
                var totalsValue = st.EffectType switch
                {
                    Enums.eEffectType.Resistance => displayStats.DamageResistance((int) st.DamageType, false),
                    Enums.eEffectType.Defense => displayStats.Defense((int) st.DamageType),
                    Enums.eEffectType.Regeneration => displayStats.HealthRegenPercent(false),
                    Enums.eEffectType.HitPoints => displayStats.HealthHitpointsNumeric(false),
                    Enums.eEffectType.Absorb => Math.Min(displayStats.Absorb, MidsContext.Character.Archetype.Hitpoints),
                    Enums.eEffectType.Recovery => displayStats.EnduranceRecoveryNumeric - MidsContext.Character.Archetype.BaseRecovery,
                    Enums.eEffectType.Endurance => displayStats.EnduranceMaxEnd - 100,
                    Enums.eEffectType.SpeedRunning => displayStats.MovementRunSpeed(Enums.eSpeedMeasure.FeetPerSecond, false),
                    Enums.eEffectType.SpeedJumping => displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.FeetPerSecond, false),
                    Enums.eEffectType.JumpHeight => displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond),
                    Enums.eEffectType.SpeedFlying => displayStats.MovementFlySpeed(Enums.eSpeedMeasure.MilesPerHour, false),
                    // MaxRunSpeed
                    // MaxJumpSpeed
                    // MaxFlySpeed
                    
                    Enums.eEffectType.StealthRadius => MidsContext.Character.Totals.StealthPvE,
                    Enums.eEffectType.StealthRadiusPlayer => MidsContext.Character.Totals.StealthPvP,
                    Enums.eEffectType.PerceptionRadius => displayStats.Perception(false),
                    
                    Enums.eEffectType.RechargeTime => displayStats.BuffHaste(false) - 100,
                    Enums.eEffectType.ToHit => displayStats.BuffToHit,
                    Enums.eEffectType.DamageBuff => displayStats.BuffDamage(false),
                    Enums.eEffectType.Elusivity => MidsContext.Character.Totals.Elusivity,
                    Enums.eEffectType.Enhancement => st.TargetEffectType switch
                    {
                        Enums.eEffectType.RechargeTime => displayStats.BuffHaste(false) - 100,
                        Enums.eEffectType.Accuracy => displayStats.BuffAccuracy,
                        Enums.eEffectType.EnduranceDiscount => displayStats.BuffEndRdx,
                        _ => 0
                    },
                    _ => 0
                };

                var fx = lBuffs.First(); 
                var fxMagAdjusted = fx.Mag * st.EffectType switch
                {
                    Enums.eEffectType.DamageBuff => 100,
                    Enums.eEffectType.Enhancement => 100,
                    Enums.eEffectType.Regeneration => 100,
                    Enums.eEffectType.Resistance => 100,
                    Enums.eEffectType.Defense => 100,
                    Enums.eEffectType.SpeedRunning => 100,
                    Enums.eEffectType.SpeedJumping => 100,
                    Enums.eEffectType.JumpHeight => 100,
                    Enums.eEffectType.SpeedFlying => 100,
                    _ => 1
                };

                totalsValue = st.EffectType switch
                {
                    Enums.eEffectType.SpeedRunning => Math.Max(0, totalsValue / Statistics.BaseRunSpeed * 100 - 100),
                    Enums.eEffectType.SpeedJumping => Math.Max(0, totalsValue / Statistics.BaseJumpSpeed * 100 - 100),
                    Enums.eEffectType.SpeedFlying => Math.Max(0, totalsValue / Statistics.BaseFlySpeed * 100 - 100),
                    Enums.eEffectType.JumpHeight => Math.Max(0, totalsValue / Statistics.BaseJumpHeight * 100 - 100),
                    _ => totalsValue
                };

                if (Math.Abs(fxMagAdjusted - totalsValue) < float.Epsilon) totalsValue = 0;

                bar.MaximumBarValue = st.EffectType switch
                {
                    Enums.eEffectType.DamageBuff => MidsContext.Character.Archetype.DamageCap * 100 - 100,
                    Enums.eEffectType.Regeneration => MidsContext.Character.Archetype.RegenCap * 100,
                    Enums.eEffectType.HitPoints => MidsContext.Character.Archetype.HPCap,
                    Enums.eEffectType.Recovery => 10,
                    Enums.eEffectType.Endurance => 50,
                    Enums.eEffectType.SpeedRunning => MidsContext.Character.Totals.MaxRunSpd / Statistics.BaseRunSpeed * 100 + 25,
                    Enums.eEffectType.SpeedJumping => MidsContext.Character.Totals.MaxJumpSpd / Statistics.BaseJumpSpeed * 100 + 25,
                    Enums.eEffectType.SpeedFlying => MidsContext.Character.Totals.MaxFlySpd / Statistics.BaseFlySpeed * 100 + 25,
                    Enums.eEffectType.StealthRadius => 1000,
                    Enums.eEffectType.StealthRadiusPlayer => 1000,
                    Enums.eEffectType.PerceptionRadius => 1000,
                    Enums.eEffectType.Enhancement => st.TargetEffectType switch
                    {
                        Enums.eEffectType.Accuracy => 200,
                        Enums.eEffectType.RechargeTime => 400,
                        _ => 100
                    },
                    _ => 100
                };

                var overlayVector = (st.EffectType == Enums.eEffectType.Enhancement ? st.TargetEffectType : st.EffectType).ToString();
                overlayVector = overlayVector
                    .Replace("Resistance", "Res")
                    .Replace("Defense", "Def")
                    .Replace("EnduranceDiscount", "End. Discount");
                overlayVector = Regex.Replace(overlayVector, @"Endurance\b", "Max End");
                var overlayDmgType = !(st.EffectType == Enums.eEffectType.Resistance | st.EffectType == Enums.eEffectType.Defense)
                    ? ""
                    : st.DamageType switch
                    {
                        Enums.eDamage.Smashing => "S/L",
                        Enums.eDamage.Fire => "Fire/Cold",
                        Enums.eDamage.Energy => "Energy/Neg",
                        _ => st.DamageType.ToString()
                    };
                var overlayValuePercent = st.EffectType != Enums.eEffectType.HitPoints & st.EffectType != Enums.eEffectType.Endurance;
                bar.AssignValues(new List<float> {fxMagAdjusted, totalsValue});
                bar.OverlayText = $"{fxMagAdjusted:##0.##}{(overlayValuePercent ? "%" : "")}";

                barLabel.Text = $"{(overlayDmgType != "" ? overlayDmgType + " " : "")}{overlayVector}:";
                panelBars.Controls.Add(bar);
                panelBars.Controls.Add(barLabel);
                BarsFX.Add(bar.Name, new FXIdentifierKey
                {
                    EffectType = st.EffectType,
                    TargetEffectType = st.TargetEffectType,
                    DamageType = st.DamageType,
                    MezType = Enums.eMez.None
                });

                nb++;
            }

            panelBars.ResumeLayout();
        }

        private void btnDetailFx_ButtonClicked()
        {
            panelBars.Visible = btnDetailFx.Checked;
            rtxtFX.Visible = !btnDetailFx.Checked;
            Label1.Text = btnDetailFx.Checked ? "Set Bonuses vs. Totals:" : "Effect Breakdown:";
            FillEffectView(btnDetailFx.Checked);
            if (btnDetailFx.Checked) DrawBars();
        }
    }
}