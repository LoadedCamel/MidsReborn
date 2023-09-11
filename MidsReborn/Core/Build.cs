using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastDeepCloner;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms.Controls;

namespace Mids_Reborn.Core
{
    public class Build
    {
        private readonly Character _character;

        public readonly List<PowerEntry?> Powers;
        public readonly List<I9SetData> SetBonus;

        private IPower? _setBonusVirtualPower;

        public int LastPower;
        public EnhancementSet? MySet;

        public Build(Character owner, IList<LevelMap> iLevels)
        {
            _character = owner;
            Powers = new List<PowerEntry?>
            {
                new PowerEntry(0, null, true)
            };
            SetBonus = new List<I9SetData>();
            LastPower = 0;
            for (var iLevel = 0; iLevel < iLevels.Count; ++iLevel)
            {
                for (var index = 0; index < iLevels[iLevel].Powers; ++index)
                {
                    Powers.Add(new PowerEntry(iLevel, null, true));
                    ++LastPower;
                }
            }
        }

        public IPower SetBonusVirtualPower
        {
            get
            {
                IPower? power;
                if ((power = _setBonusVirtualPower) == null)
                    power = _setBonusVirtualPower = GetSetBonusVirtualPower();
                return power;
            }
        }

        public int PowersPlaced
        {
            get
            {
                var pplaced = 0;
                foreach (var power in Powers.Where(p => p != null))
                {
                    if (power.Chosen && power.Power != null)
                    {
                        ++pplaced;
                    }
                }

                return pplaced;
            }
        }

        public int SlotsPlaced
        {
            get
            {
                var placed = 0;
                foreach (var power in Powers.Where(p => p != null))
                {
                    if (power.Slots.Length > 1)
                    {
                        placed += power.Slots.Length - 1;
                    }
                }

                return placed;
            }
        }

        /*public int TotalSlotsAvailable
    {
        get { return DatabaseAPI.Database.Levels.Sum(level => level.Slots); }
    }*/

        public static int TotalSlotsAvailable
        {
            get
            {
                return DatabaseAPI.ServerData.EnableInherentSlotting switch
                {
                    false => DatabaseAPI.ServerData.MaxSlots,
                    true => DatabaseAPI.ServerData.MaxSlots + DatabaseAPI.ServerData.HealthSlots + DatabaseAPI.ServerData.StaminaSlots
                };
            }
        }

        public PowerEntry AddPower(IPower? power, int specialLevel = -1)
        {
            var powerEntry = GetPowerEntry(power);
            if (powerEntry == null)
            {
                powerEntry = new PowerEntry(specialLevel, power);
                Powers.Add(powerEntry);
            }

            powerEntry.ValidateSlots();
            return powerEntry;
        }

        public void RemovePower(IPower? powerToRemove)
        {
            foreach (var powerEntry in Powers.Where(powerEntry => powerEntry is { Power: { } } && powerEntry.Power.PowerIndex == powerToRemove.PowerIndex))
            {
                powerEntry?.Reset();
                break;
            }
        }

        private PowerEntry? GetPowerEntry(IPower? power)
        {
            return Powers.FirstOrDefault(powerEntry => powerEntry is { Power: { } } && powerEntry.Power.PowerIndex.Equals(power.PowerIndex));
        }

        public async Task RemoveSlotFromPowerEntry(int powerIdx, int slotIdx)
        {
            if (powerIdx < 0 || slotIdx <= 0)
            {
                return;
            }

            var power = Powers.FirstOrDefault(x => x != null && x.IDXPower == powerIdx);
            if (power == null)
            {
                return;
            }

            if (slotIdx >= power.Slots.Length)
            {
                return;
            }

            var slots = power.Slots.ToList();
            slots.RemoveAt(slotIdx);
            power.Slots = slots.ToArray();

            await Task.CompletedTask;
        }

        public void RemoveSlotFromPower(int powerIdx, int slotIdx)
        {
            if (powerIdx < 0 || powerIdx > Powers.Count - 1 || slotIdx < 0)
            {
                return;
            }
            var powerEntry = Powers[powerIdx];
            if (powerEntry == null || slotIdx > powerEntry.Slots.Length - 1) return;


            var index1 = -1;
            var slotEntryArray = new SlotEntry[powerEntry.Slots.Length - 1];
            for (var i = 0; i <= powerEntry.Slots.Length - 1; ++i)
            {
                if (i == slotIdx)
                {
                    continue;
                }

                ++index1;
                slotEntryArray[index1].Assign(powerEntry.Slots[i]);
            }

            powerEntry.Slots = new SlotEntry[slotEntryArray.Length];
            for (var index2 = 0; index2 <= powerEntry.Slots.Length - 1; ++index2)
            {
                powerEntry.Slots[index2].Assign(slotEntryArray[index2]);
            }
        }

        private void FillMissingSubPowers()
        {
            foreach (var power in Powers.Where(power => power != null))
            {
                if (power.Power == null || power.Power.NIDSubPower.Length <= 0)
                    continue;
                if (power.SubPowers.Length != power.Power.NIDSubPower.Length)
                    power.SubPowers = new PowerSubEntry[power.Power.NIDSubPower.Length];
                for (var index = 0; index <= power.Power.NIDSubPower.Length - 1; ++index)
                {
                    if (power.SubPowers[index]?.nIDPower == power.Power.NIDSubPower[index])
                        continue;
                    power.SubPowers[index] = new PowerSubEntry
                    {
                        nIDPower = power.Power.NIDSubPower[index]
                    };
                    if (power.Power.NIDSubPower[index] > -1)
                    {
                        power.SubPowers[index].Powerset =
                            DatabaseAPI.Database.Power[power.Power.NIDSubPower[index]].PowerSetID;
                        power.SubPowers[index].Power =
                            DatabaseAPI.Database.Power[power.Power.NIDSubPower[index]].PowerSetIndex;
                    }
                    else
                    {
                        power.SubPowers[index].Powerset = -1;
                        power.SubPowers[index].Power = -1;
                    }
                }
            }
        }

        private void ValidateEnhancements()
        {
            foreach (var power in Powers.Where(p => p != null))
                foreach (var slot in power.Slots)
                {
                    if (slot.Enhancement.Enh > -1)
                    {
                        if (DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID == Enums.eType.Normal &&
                            (slot.Enhancement.Grade <= Enums.eEnhGrade.None) |
                            (slot.Enhancement.Grade > Enums.eEnhGrade.SingleO))
                            slot.Enhancement.Grade = Enums.eEnhGrade.SingleO;
                        if ((DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID == Enums.eType.Normal) |
                            (DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID == Enums.eType.SpecialO) &&
                            (slot.Enhancement.RelativeLevel < Enums.eEnhRelative.None) |
                            (slot.Enhancement.RelativeLevel > Enums.eEnhRelative.PlusFive))
                            slot.Enhancement.RelativeLevel = MidsContext.Config.CalcEnhLevel;
                    }

                    if (slot.FlippedEnhancement.Enh <= -1)
                        continue;
                    if (DatabaseAPI.Database.Enhancements[slot.FlippedEnhancement.Enh].TypeID == Enums.eType.Normal &&
                        (slot.FlippedEnhancement.Grade <= Enums.eEnhGrade.None) |
                        (slot.FlippedEnhancement.Grade > Enums.eEnhGrade.SingleO))
                        slot.FlippedEnhancement.Grade = Enums.eEnhGrade.SingleO;
                    if ((DatabaseAPI.Database.Enhancements[slot.FlippedEnhancement.Enh].TypeID == Enums.eType.Normal) |
                        (DatabaseAPI.Database.Enhancements[slot.FlippedEnhancement.Enh].TypeID == Enums.eType.SpecialO) &&
                        (slot.FlippedEnhancement.RelativeLevel < Enums.eEnhRelative.None) |
                        (slot.FlippedEnhancement.RelativeLevel > Enums.eEnhRelative.PlusFive))
                        slot.FlippedEnhancement.RelativeLevel = MidsContext.Config.CalcEnhLevel;
                }
        }

        public bool SetEnhGrades(Enums.eEnhGrade newVal)
        {
            var str = newVal switch
            {
                //Enums.eEnhGrade.None => "None", // This value should never be passed to the function!
                Enums.eEnhGrade.TrainingO => "Training",
                Enums.eEnhGrade.DualO => "Dual",
                Enums.eEnhGrade.SingleO => "Single",
                _ => string.Empty
            };

            if (TopMostMessageBox(
                $"Really set all placed Regular enhancements to {str} Origin?\r\n\r\nThis will not affect any Invention or Special enhancements.",
                "Are you sure?", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return false;
            foreach (var power in Powers)
                foreach (var slot in power.Slots)
                    if (slot.Enhancement.Enh > -1 &&
                        DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID == Enums.eType.Normal)
                        slot.Enhancement.Grade = newVal;

            return true;
        }

        public bool SetIOLevels(int newVal, bool iSetMin, bool iSetMax)
        {
            var text = "Really set all placed Invention and Set enhancements to ";
            if (!iSetMin & !iSetMax)
            {
                text += $"level {newVal + 1}?\r\n\r\nNote: Enhancements which are not available at the default level will be set to the closest one.";
            }
            else if (iSetMin)
            {
                newVal = 0;
                text += "their minimum possible level?";
            }
            else
            {
                newVal = 52;
                text += "their maximum possible level?";
            }

            if (TopMostMessageBox(text, "Are you sure?", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return false;
            foreach (var power in Powers)
                foreach (var slot in power.Slots)
                {
                    if (slot.Enhancement.Enh <= -1)
                        continue;
                    switch (DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID)
                    {
                        case Enums.eType.InventO:
                            var levelMin1 = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].LevelMin;
                            var levelMax = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].LevelMax;
                            if ((newVal >= levelMin1) & (newVal <= levelMax))
                            {
                                slot.Enhancement.IOLevel = Enhancement.GranularLevelZb(newVal, levelMin1, levelMax);
                                break;
                            }

                            if (newVal > levelMax)
                            {
                                slot.Enhancement.IOLevel = Enhancement.GranularLevelZb(levelMax, levelMin1, levelMax);
                                break;
                            }

                            if (newVal < levelMin1)
                                slot.Enhancement.IOLevel = Enhancement.GranularLevelZb(levelMin1, levelMin1, levelMax);

                            break;
                        case Enums.eType.SetO:
                            var levelMin2 = DatabaseAPI.Database
                                .EnhancementSets[DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].nIDSet].LevelMin;
                            var num = DatabaseAPI.Database
                                .EnhancementSets[DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].nIDSet].LevelMax;
                            if (num > 49)
                                num = 49;
                            if ((newVal >= levelMin2) & (newVal <= num))
                            {
                                slot.Enhancement.IOLevel = newVal;
                                break;
                            }

                            if (newVal > num)
                            {
                                slot.Enhancement.IOLevel = num;
                                break;
                            }

                            if (newVal < levelMin2) slot.Enhancement.IOLevel = levelMin2;

                            break;
                        case Enums.eType.None:
                            break;
                        case Enums.eType.Normal:
                            break;
                        case Enums.eType.SpecialO:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

            return true;
        }

        public HistoryMap[] BuildHistoryMap(bool enhNames, bool ioLevel = true)
        {
            var historyMapList = new List<HistoryMap>();
            for (var lvlIdx = 0; lvlIdx < DatabaseAPI.Database.Levels.Length; lvlIdx++)
            {
                for (var powerIdx = 0; powerIdx < Powers.Count; powerIdx++)
                {
                    var power = Powers[powerIdx];
                    if (!power.Chosen && power.SubPowers.Length != 0 && power.SlotCount != 0 || power.Level != lvlIdx ||
                        power.Power == null)
                    {
                        continue;
                    }

                    var historyMap = new HistoryMap
                    {
                        Level = lvlIdx,
                        HID = powerIdx
                    };

                    var appendText = string.Empty;
                    var choiceText = power.Chosen ? "Added" : "Received";
                    if (power.Slots.Length > 0)
                    {
                        historyMap.SID = 0;
                        if (power.Slots[0].Enhancement.Enh > -1)
                        {
                            if (enhNames)
                            {
                                appendText = $" [{DatabaseAPI.GetEnhancementNameShortWSet(power.Slots[0].Enhancement.Enh)}";
                            }

                            if (ioLevel && DatabaseAPI.Database.Enhancements[power.Slots[0].Enhancement.Enh].TypeID is Enums.eType.InventO or Enums.eType.SetO)
                            {
                                appendText = $"{appendText}-{power.Slots[0].Enhancement.IOLevel + 1}";
                            }

                            appendText += "]";
                        }
                        else if (enhNames)
                        {
                            appendText = " [Empty]";
                        }
                    }

                    historyMap.Text = $"Level {lvlIdx + 1}: {choiceText} {power.Power.DisplayName} ({Enum.GetName(DatabaseAPI.Database.Powersets[power.NIDPowerset].SetType.GetType(), DatabaseAPI.Database.Powersets[power.NIDPowerset].SetType)}){appendText}";
                    historyMapList.Add(historyMap);
                }

                for (var powerIdx = 0; powerIdx < Powers.Count; powerIdx++)
                {
                    var power = Powers[powerIdx];
                    for (var slotIdx = 1; slotIdx < power.Slots.Length; slotIdx++)
                    {
                        if (power.Slots[slotIdx].Level != lvlIdx)
                        {
                            continue;
                        }

                        var historyMap = new HistoryMap
                        {
                            Level = lvlIdx,
                            HID = powerIdx,
                            SID = slotIdx
                        };

                        var str = string.Empty;
                        if (power.Slots[slotIdx].Enhancement.Enh > -1)
                        {
                            if (enhNames)
                            {
                                str = $" [{DatabaseAPI.GetEnhancementNameShortWSet(power.Slots[slotIdx].Enhancement.Enh)}";
                            }

                            if (ioLevel &&
                                DatabaseAPI.Database.Enhancements[power.Slots[slotIdx].Enhancement.Enh].TypeID is Enums.eType.InventO or Enums.eType.SetO)
                            {
                                str = $"{str}-{power.Slots[slotIdx].Enhancement.IOLevel + 1}";
                            }

                            str += "]";
                        }
                        else if (enhNames)
                        {
                            str = " [Empty]";
                        }

                        historyMap.Text = $"Level {lvlIdx + 1}: Added Slot to {power.Power.DisplayName}{str}";
                        historyMapList.Add(historyMap);
                    }
                }
            }

            return historyMapList.ToArray();
        }

        private int SlotsAtLevel(int powerEntryId, int iLevel)
        {
            if (powerEntryId < 0) return 0;
            var powerEntry = Powers[powerEntryId];
            return powerEntry == null ? 0 : powerEntry.Slots.Count(slot => slot.Level <= iLevel);
        }

        public int SlotsPlacedAtLevel(int level)
        {
            var slotsPlacedAtLevel = 0;
            foreach (var powerIdx in Powers.Where(p => p != null))
            {
                if (powerIdx == null) continue;
                for (var slotIdx = 0; slotIdx < powerIdx.Slots.Length; ++slotIdx)
                {
                    if (powerIdx.Slots[slotIdx].Level == level)
                    {
                        ++slotsPlacedAtLevel;
                    }
                }
            }

            return slotsPlacedAtLevel;
        }

        public PopUp.PopupData GetRespecHelper(bool longFormat, int iLevel = 49)
        {
            var popupData = new PopUp.PopupData();
            var historyMapArray = BuildHistoryMap(true);
            var index1 = popupData.Add();
            popupData.Sections[index1].Add("Respec to level: " + (iLevel + 1), PopUp.Colors.Effect, 1.25f);
            foreach (var historyMap in historyMapArray)
            {
                if (historyMap.HID <= -1 || DatabaseAPI.Database.Levels[historyMap.Level].Powers <= 0 ||
                    historyMap.Level > iLevel)
                    continue;
                var power = Powers[historyMap.HID];
                if (power.Slots.Length <= 0)
                    continue;
                var index2 = popupData.Add();
                string iText1;
                if (power.Power != null)
                    iText1 = "Level " + (historyMap.Level + 1) + ": " + power.Power.DisplayName;
                else
                    iText1 = "Level " + (historyMap.Level + 1) + ": [Empty]";
                popupData.Sections[index2].Add(iText1, PopUp.Colors.Text);
                var slots = SlotsAtLevel(historyMap.HID, iLevel);
                if (slots <= 0)
                    continue;
                popupData.Sections[index2].Add("Slots: " + slots, PopUp.Colors.Text, 1f, FontStyle.Regular, 1);
                if (!longFormat)
                    continue;
                for (var slotIdx = 0; slotIdx <= slots - 1; ++slotIdx)
                {
                    var str = slotIdx != 0 ? "(" + (power.Slots[slotIdx].Level + 1) + ") " : "(A) ";
                    string iText2;
                    if (power.Slots[slotIdx].Enhancement.Enh > -1)
                    {
                        iText2 = str + DatabaseAPI.GetEnhancementNameShortWSet(power.Slots[slotIdx].Enhancement.Enh);
                        if (DatabaseAPI.Database.Enhancements[power.Slots[slotIdx].Enhancement.Enh].TypeID ==
                            Enums.eType.InventO ||
                            DatabaseAPI.Database.Enhancements[power.Slots[slotIdx].Enhancement.Enh].TypeID ==
                            Enums.eType.SetO)
                            iText2 = iText2 + "-" + (power.Slots[slotIdx].Enhancement.IOLevel + 1);
                    }
                    else
                    {
                        iText2 = str + "[Empty]";
                    }

                    popupData.Sections[index2].Add(iText2, PopUp.Colors.Invention, 1f, FontStyle.Regular, 2);
                }
            }

            return popupData;
        }

        public PopUp.PopupData GetRespecHelper2(bool longFormat, int iLevel = 49)
        {
            var popupData = new PopUp.PopupData();
            var historyMapArray = BuildHistoryMap(true);
            var index = popupData.Add();
            popupData.Sections[index].Add("Respec to level: " + (iLevel + 1), PopUp.Colors.Effect, 1.25f);
            var histLvl = 0;
            foreach (var historyMap in historyMapArray)
            {
                if (histLvl != historyMap.Level)
                    index = popupData.Add();
                histLvl = historyMap.Level;
                if (historyMap.HID < 0)
                    continue;
                var power = Powers[historyMap.HID];
                if ((DatabaseAPI.Database.Levels[historyMap.Level].Powers > 0) & (historyMap.Level <= iLevel))
                {
                    if (power.Slots.Length <= 0)
                        continue;
                    string iText1;
                    if (power.Power != null)
                        iText1 = "Level " + (historyMap.Level + 1) + ": " + power.Power.DisplayName;
                    else
                        iText1 = "Level " + (historyMap.Level + 1) + ": [Empty]";
                    popupData.Sections[index].Add(iText1, PopUp.Colors.Text);
                    if (!longFormat)
                        continue;
                    var empty = string.Empty;
                    string iText2;
                    if (power.Slots[historyMap.SID].Enhancement.Enh > -1)
                    {
                        iText2 = empty +
                                 DatabaseAPI.GetEnhancementNameShortWSet(power.Slots[historyMap.SID].Enhancement.Enh);
                        if (DatabaseAPI.Database.Enhancements[power.Slots[historyMap.SID].Enhancement.Enh].TypeID ==
                            Enums.eType.InventO ||
                            DatabaseAPI.Database.Enhancements[power.Slots[historyMap.SID].Enhancement.Enh].TypeID ==
                            Enums.eType.SetO)
                            iText2 = iText2 + "-" + (power.Slots[historyMap.SID].Enhancement.IOLevel + 1);
                    }
                    else
                    {
                        iText2 = empty + "[Empty]";
                    }

                    popupData.Sections[index].Add(iText2, PopUp.Colors.Invention, 1f, FontStyle.Regular, 1);
                }
                else if ((DatabaseAPI.Database.Levels[historyMap.Level].Slots > 0) & (historyMap.Level <= iLevel) &&
                         historyMap.SID > -1)
                {
                    var str = historyMap.SID != 0
                        ? "Level " + (historyMap.Level + 1) + ": Added Slot To "
                        : "Level " + (historyMap.Level + 1) + ": Received Slot - ";
                    var iText1 = power.Power == null ? str + "[Empty]" : str + power.Power.DisplayName;
                    popupData.Sections[index].Add(iText1, PopUp.Colors.Effect);
                    if (!longFormat)
                        continue;
                    string iText2;
                    if (power.Slots[historyMap.SID].Enhancement.Enh > -1)
                    {
                        iText2 = string.Empty +
                                 DatabaseAPI.GetEnhancementNameShortWSet(power.Slots[historyMap.SID].Enhancement.Enh);
                        if (DatabaseAPI.Database.Enhancements[power.Slots[historyMap.SID].Enhancement.Enh].TypeID ==
                            Enums.eType.InventO ||
                            DatabaseAPI.Database.Enhancements[power.Slots[historyMap.SID].Enhancement.Enh].TypeID ==
                            Enums.eType.SetO)
                            iText2 = iText2 + "-" + (power.Slots[historyMap.SID].Enhancement.IOLevel + 1);
                    }
                    else
                    {
                        iText2 = "[Empty]";
                    }

                    popupData.Sections[index].Add(iText2, PopUp.Colors.Invention, 1f, FontStyle.Regular, 1);
                }
            }

            return popupData;
        }

        // https://stackoverflow.com/a/65888392
        // frmMain.FloatTop(false) should be used here but it is unreachable.
        private DialogResult TopMostMessageBox(string msg, string title, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            using var form = new Form { TopMost = true };
            var ret = MessageBox.Show(form, msg, title, buttons);
            form.Dispose();

            return ret;
        }

        public bool SetEnhRelativeLevels(Enums.eEnhRelative newVal)
        {
            var display = newVal switch
            {
                Enums.eEnhRelative.None => "None (Enhancements will have no effect)",
                Enums.eEnhRelative.MinusThree => "-3",
                Enums.eEnhRelative.MinusTwo => "-2",
                Enums.eEnhRelative.MinusOne => "-1",
                Enums.eEnhRelative.Even => "Even (+/- 0)",
                Enums.eEnhRelative.PlusOne => "+1",
                Enums.eEnhRelative.PlusTwo => "+2",
                Enums.eEnhRelative.PlusThree => "+3",
                Enums.eEnhRelative.PlusFour => "+4",
                Enums.eEnhRelative.PlusFive => "+5",
                _ => throw new ArgumentOutOfRangeException(nameof(newVal), newVal, null)
            };

            if (TopMostMessageBox($"Really set all placed enhancements to a relative level of {display}?\r\n\r\nNote: Normal and special enhancements cannot go above +3,\r\nInventions cannot go below +0.",
                    "Are you sure?", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return false;
            foreach (var power in Powers)
                foreach (var slot in power.Slots)
                {
                    if (slot.Enhancement.Enh <= -1) continue;

                    var enhancement = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh];
                    if (enhancement.TypeID == Enums.eType.SpecialO || enhancement.TypeID == Enums.eType.Normal)
                    {
                        if (newVal > Enums.eEnhRelative.PlusThree) newVal = Enums.eEnhRelative.PlusThree;
                    }
                    else if (enhancement.TypeID == Enums.eType.InventO || enhancement.TypeID == Enums.eType.SetO)
                    {
                        if (newVal < Enums.eEnhRelative.Even && newVal != Enums.eEnhRelative.None)
                            newVal = Enums.eEnhRelative.Even;
                    }

                    /*
                    if (newVal > Enums.eEnhRelative.PlusThree)
                    {
                        int num = enhancement.TypeID == Enums.eType.Normal ? 0 : (enhancement.TypeID != Enums.eType.SpecialO ? 1 : 0);
                        slot.Enhancement.RelativeLevel = num != 0 ? newVal : Enums.eEnhRelative.PlusThree;
                    }
                    else if (newVal < Enums.eEnhRelative.Even)
                    {
                        int num = enhancement.TypeID == Enums.eType.Normal ? 0 : (enhancement.TypeID != Enums.eType.SpecialO ? 1 : 0);
                        slot.Enhancement.RelativeLevel = num != 0 ? Enums.eEnhRelative.Even : newVal;
                    }
                    else
                        slot.Enhancement.RelativeLevel = newVal;
                    */
                    slot.Enhancement.RelativeLevel = newVal;
                }

            return true;
        }

        private void CheckAndFixAllEnhancements()
        {
            foreach (var power in Powers.Where(p => p != null))
            {
                foreach (var slot in power.Slots)
                {
                    if (power.Power != null)
                    {
                        if (slot.Enhancement.Enh > -1)
                        {
                            if (!power.Power.IsEnhancementValid(slot.Enhancement.Enh))
                            {
                                slot.Enhancement.Enh = -1;
                            }
                            else
                            {
                                slot.Enhancement.IOLevel = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].CheckAndFixIOLevel(slot.Enhancement.IOLevel);
                            }
                        }

                        if (slot.FlippedEnhancement.Enh <= -1)
                        {
                            continue;
                        }

                        if (!power.Power.IsEnhancementValid(slot.FlippedEnhancement.Enh))
                        {
                            slot.FlippedEnhancement.Enh = -1;
                        }
                        else
                        {
                            slot.FlippedEnhancement.IOLevel = DatabaseAPI.Database.Enhancements[slot.FlippedEnhancement.Enh]
                                .CheckAndFixIOLevel(slot.FlippedEnhancement.IOLevel);
                        }
                    }
                    else
                    {
                        slot.Enhancement.Enh = -1;
                        slot.Enhancement.IOLevel = 0;
                        slot.FlippedEnhancement.Enh = -1;
                        slot.FlippedEnhancement.IOLevel = 0;
                    }
                }
            }

            ValidateEnhancements();
        }

        private void CheckAllVariableBounds()
        {
            for (var index = 0; index <= Powers.Count - 1; ++index)
                Powers[index]?.CheckVariableBounds();
        }

        private void CheckInherentSlotting()
        {
            foreach (var power in Powers.Where(power => power?.Power != null))
            {
                switch (power?.Power?.FullName)
                {
                    case "Inherent.Fitness.Health":
                       switch (MidsContext.Config.BuildMode)
                            {
                                case Enums.dmModes.LevelUp:
                                    if (MidsContext.Character != null && MidsContext.Character.Level == DatabaseAPI.ServerData.HealthSlot1Level)
                                    {
                                        if (power.InherentSlotsUsed < 1)
                                        {
                                            power.AddSlot(DatabaseAPI.ServerData.HealthSlot1Level, true);
                                            power.InherentSlotsUsed += 1;
                                        }
                                    }

                                    if (MidsContext.Character != null && MidsContext.Character.Level == DatabaseAPI.ServerData.HealthSlot2Level)
                                    {
                                        if (power.InherentSlotsUsed is > 0 and < 2)
                                        {
                                            power.AddSlot(DatabaseAPI.ServerData.HealthSlot2Level, true);
                                            power.InherentSlotsUsed += 1;
                                        }
                                    }

                                    break;
                                case Enums.dmModes.Normal:
                                    var chosenCount = Powers.Where(x => x is { Power: { }, Chosen: true }).ToList()
                                        .Count;
                                    if (chosenCount > 0)
                                    {
                                        if (power is { SlotCount: < 2, InherentSlotsUsed: < 2 })
                                        {
                                            power.AddSlot(DatabaseAPI.ServerData.HealthSlot1Level, true);
                                            power.AddSlot(DatabaseAPI.ServerData.HealthSlot2Level, true);
                                            power.InherentSlotsUsed = 2;
                                        }
                                    }

                                    break;
                            }

                        break;
                    case "Inherent.Fitness.Stamina":
                        switch (MidsContext.Config.BuildMode)
                        {
                            case Enums.dmModes.LevelUp:
                                if (MidsContext.Character != null && MidsContext.Character.Level ==
                                    DatabaseAPI.ServerData.StaminaSlot1Level)
                                {
                                    if (power.InherentSlotsUsed < 1)
                                    {
                                        power.AddSlot(DatabaseAPI.ServerData.StaminaSlot1Level, true);
                                        power.InherentSlotsUsed += 1;
                                    }
                                }

                                if (MidsContext.Character != null && MidsContext.Character.Level ==
                                    DatabaseAPI.ServerData.StaminaSlot2Level)
                                {
                                    if (power.InherentSlotsUsed is > 0 and < 2)
                                    {
                                        power.AddSlot(DatabaseAPI.ServerData.StaminaSlot2Level, true);
                                        power.InherentSlotsUsed += 1;
                                    }
                                }

                                break;
                            case Enums.dmModes.Normal
                                : // Need to check if a build is started if not then do not add slots
                                var chosenCount = Powers.Where(x => x is { Power: { }, Chosen: true }).ToList().Count;
                                if (chosenCount > 0)
                                {
                                    if (power is { SlotCount: < 2, InherentSlotsUsed: < 2 })
                                    {
                                        power.AddSlot(DatabaseAPI.ServerData.StaminaSlot1Level, true);
                                        power.AddSlot(DatabaseAPI.ServerData.StaminaSlot2Level, true);
                                        power.InherentSlotsUsed = 2;
                                    }
                                }

                                break;
                        }

                        break;

                }
            }
        }

        internal void Validate()
        {
            ClearInvisibleSlots();
            ScanAndCleanAutomaticallyGrantedPowers();
            AddAutomaticGrantedPowers();
            FillMissingSubPowers();
            CheckAndFixAllEnhancements();
            CheckAllVariableBounds();
            if (DatabaseAPI.ServerData.EnableInherentSlotting)
            {
                CheckInherentSlotting();
            }
        }

        public int GetMaxLevel()
        {
            var maxLevel = 0;
            foreach (var power in Powers)
            {
                if (power != null && power.Level > maxLevel)
                    maxLevel = power.Level;
                if (power != null)
                    maxLevel = power.Slots.Select(slot => slot.Level)
                        .Concat(new[]
                        {
                            maxLevel
                        })
                        .Max();
            }

            return maxLevel;
        }

        private void ClearInvisibleSlots()
        {
            foreach (var power in Powers) power?.ClearInvisibleSlots();
        }

        private void ScanAndCleanAutomaticallyGrantedPowers()
        {
            var flag = false;
            var maxLevel = GetMaxLevel();
            foreach (var power in Powers)
            {
                if (power?.Power == null)
                {
                    continue;
                }

                if (power.Chosen || power.PowerSet.SetType != Enums.ePowerSetType.Inherent && power.PowerSet.SetType != Enums.ePowerSetType.Primary && power.PowerSet.SetType != Enums.ePowerSetType.Secondary && power.PowerSet.SetType != Enums.ePowerSetType.Pool)
                {
                    continue;
                }

                if (power.Power.Level > maxLevel + 1 || !MeetsRequirement(power.Power, maxLevel) ||
                    !power.Power.IncludeFlag)
                {
                    power.Tag = true;
                    flag = true;
                }

                if (power.Power.Level > power.Level + 1)
                {
                    power.Level = power.Power.Level - 1;
                }
            }

            if (!flag)
            {
                return;
            }

            for (var powerIdx = 0; powerIdx < Powers.Count; powerIdx++)
            {
                if (Powers[powerIdx] == null)
                {
                    continue;
                }

                if (Powers[powerIdx].Tag)
                {
                    Powers[powerIdx].Reset();
                    if (Powers[powerIdx].Chosen)
                    {
                        continue;
                    }

                    Powers[powerIdx].Level = -1;
                    Powers[powerIdx].Slots = Array.Empty<SlotEntry>();
                }
            }
        }

        public bool MeetsRequirement(IPower? power, int nLevel, int skipIdx = -1)
        {
            if (nLevel < 0)
            {
                return false;
            }

            var nIdSkip = -1;
            if (skipIdx > -1 & skipIdx < Powers.Count)
            {
                nIdSkip = Powers[skipIdx].Power == null ? -1 : Powers[skipIdx].Power.PowerIndex;
            }

            if (nLevel + 1 < power.Level)
            {
                return false;
            }

            if (power.Requires.NClassName.Length == 0 & power.Requires.NClassNameNot.Length == 0 & power.Requires.NPowerID.Length == 0 & power.Requires.NPowerIDNot.Length == 0)
            {
                return true;
            }

            var valid = power.Requires.NClassName.Length == 0;

            foreach (var clsNameIdx in power.Requires.NClassName)
            {
                if (MidsContext.Character.Archetype.Idx == clsNameIdx)
                {
                    valid = true;
                }
            }

            if (power.Requires.NClassNameNot.Any(nClsNameNot => MidsContext.Character.Archetype.Idx == nClsNameNot))
            {
                return false;
            }

            if (!valid)
            {
                return false;
            }

            if (power.Requires.NPowerID.Length > 0)
            {
                valid = false;
            }

            foreach (var numArray in power.Requires.NPowerID)
            {
                var doubleValid = true;
                var powerIndex = -1;
                foreach (var nIdPower in numArray)
                {
                    if (nIdPower <= -1) continue;
                    if (nIdPower != nIdSkip)
                    {
                        powerIndex = FindInToonHistory(nIdPower);
                    }

                    if (powerIndex < 0 || Powers[powerIndex]?.Level > nLevel)
                    {
                        doubleValid = false;
                    }
                }

                if (!doubleValid)
                {
                    continue;
                }

                valid = true;
                break;
            }

            if (!valid)
            {
                return false;
            }

            foreach (var numArray in power.Requires.NPowerIDNot)
            {
                foreach (var nIdPower in numArray)
                {
                    if (nIdPower <= -1)
                    {
                        continue;
                    }

                    var histIdx = -1;
                    if (nIdPower != nIdSkip)
                    {
                        histIdx = FindInToonHistory(nIdPower);
                    }

                    if (histIdx > -1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public int FindInToonHistory(int nIDPower)
        {
            for (var powerIdx = 0; powerIdx <= Powers.Count - 1; ++powerIdx)
            {
                if (Powers[powerIdx]?.Power != null && Powers[powerIdx]?.Power.PowerIndex == nIDPower)
                {
                    return powerIdx;
                }
            }

            return -1;
        }

        public bool PowerUsed(IPower? power)
        {
            return FindInToonHistory(power.PowerIndex) > -1;
        }

        public bool PowerActive(IPower power)
        {
            for (var powerIdx = 0; powerIdx <= Powers.Count - 1; ++powerIdx)
            {
                if (Powers[powerIdx] != null && Powers[powerIdx].Power != null && Powers[powerIdx].Power.PowerIndex == power.PowerIndex)
                {
                    return Powers[powerIdx].Power.Active;
                }
            }
            return false;
        }

        private void AddAutomaticGrantedPowers()
        {
            var maxLevel = GetMaxLevel();
            var powersetList = new List<IPowerset?>();
            powersetList.AddRange(_character.Powersets);
            foreach (var powerset in DatabaseAPI.Database.Powersets)
            {
                if (powerset.SetType == Enums.ePowerSetType.Inherent && !powersetList.Contains(powerset))
                {
                    powersetList.Add(powerset);
                }
            }

            foreach (var powerset in powersetList)
            {
                if (powerset == null)
                {
                    continue;
                }

                foreach (var power in powerset.Powers)
                {
                    var val2 = 0;
                    if (!power.IncludeFlag || power.Level > maxLevel + 1 || PowerUsed(power) || !MeetsRequirement(power, maxLevel + 1) || power.InherentType == Enums.eGridType.Prestige)
                    {
                        continue;
                    }

                    if (power.Requires.NPowerID.Length > 0)
                    {
                        var inToonHistory = FindInToonHistory(power.Requires.NPowerID[0][0]);
                        if (inToonHistory > -1)
                        {
                            val2 = Powers[inToonHistory].Level;
                        }
                    }

                    AddPower(power, Math.Max(power.Level - 1, val2));
                }
            }
        }

        public bool EnhancementTest(int iSlotID, int hIdx, int iEnh, bool silent = false)
        {
            if (iEnh < 0 || iSlotID < 0) return false;

            var enhancement = DatabaseAPI.Database.Enhancements[iEnh];
            var foundMutex = false;
            var foundInPower = false;
            var foundEnh = string.Empty;
            var mutexType = -1;
            if (enhancement.TypeID == Enums.eType.SetO && enhancement.nIDSet > -1 && hIdx > -1 && Powers[hIdx].Power != null)
            {
                var allowedSet = false;
                var setType = DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].SetType;
                for (var index = 0; index <= Powers[hIdx].Power.SetTypes.Count - 1; ++index)
                {
                    if (Powers[hIdx].Power.SetTypes[index] != setType)
                    {
                        continue;
                    }

                    allowedSet = true;
                    break;
                }

                if (!allowedSet)
                {
                    return false;
                }
            }

            for (var powerIdx = 0; powerIdx < Powers.Count; powerIdx++)
            {
                if (Powers[powerIdx] == null)
                {
                    continue;
                }

                var power = Powers[powerIdx];
                for (var slotIndex = 0; slotIndex < power.Slots.Length; slotIndex++)
                {
                    if (slotIndex == iSlotID && powerIdx == hIdx || Powers[powerIdx].Slots[slotIndex].Enhancement.Enh <= -1)
                    {
                        continue;
                    }

                    if (enhancement.Unique && Powers[powerIdx].Slots[slotIndex].Enhancement.Enh == iEnh)
                    {
                        if (!silent)
                        {
                            // Standard MessageBox may spawn behind TopMost controls like the totals or sets windows.
                            //MessageBox.Show($@"{enhancement.LongName} is a unique enhancement. You can only slot one of these across your entire build.", @"Unable To Slot Enhancement");
                            using var msgBox = new MessageBoxEx($@"{enhancement.LongName} is a unique enhancement. You can only slot one of these across your entire build.", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Warning);
                            msgBox.ShowDialog();
                        }

                        return false;
                    }

                    if (enhancement.Superior && enhancement.MutExID != Enums.eEnhMutex.None)
                    {
                        //Debug.WriteLine(enhancement.UID);
                        var nVersion = Regex.Replace(enhancement.UID, @"(Attuned_|Superior_)", "");
                        foreach (var item in MidsContext.Character.PEnhancementsList)
                        {
                            if (item.Contains(nVersion))
                            {
                                foundEnh = DatabaseAPI.Database.Enhancements[DatabaseAPI.GetEnhancementByUIDName(item)].LongName;
                                mutexType = 0;
                                foundMutex = true;
                            }
                        }
                    }
                    else if (!enhancement.Superior && enhancement.MutExID != Enums.eEnhMutex.None && enhancement.MutExID != Enums.eEnhMutex.Stealth)
                    {
                        var nVersion = Regex.Replace(enhancement.UID, @"(Attuned_|Superior_)", "");
                        foreach (var item in MidsContext.Character.PEnhancementsList)
                        {
                            if (item.Contains($"Superior_Attuned_{nVersion}") || item.Contains($"Superior_Attuned_Superior_{nVersion}"))
                            {
                                foundEnh = DatabaseAPI.Database.Enhancements[DatabaseAPI.GetEnhancementByUIDName(item)].LongName;
                                mutexType = 0;
                                foundMutex = true;
                            }
                        }
                    }
                    else if (enhancement.MutExID == Enums.eEnhMutex.Stealth)
                    {
                        foreach (var item in MidsContext.Character.PEnhancementsList)
                        {
                            if (DatabaseAPI.Database.Enhancements[DatabaseAPI.GetEnhancementByUIDName(item)].MutExID == Enums.eEnhMutex.Stealth)
                            {
                                foundEnh = DatabaseAPI.Database.Enhancements[DatabaseAPI.GetEnhancementByUIDName(item)].LongName;
                                mutexType = 1;
                                foundMutex = true;
                            }
                        }
                    }

                    if (enhancement.nIDSet <= -1 || powerIdx != hIdx || Powers[powerIdx].Slots[slotIndex].Enhancement.Enh != iEnh) continue;
                    foundInPower = true;
                    break;
                }
            }

            if (foundMutex)
            {
                if (!silent)
                {
                    switch (mutexType)
                    {
                        case 0:
                            MessageBox.Show(@$"{enhancement.LongName} is mutually exclusive with {foundEnh}. You can only slot one type of this enhancement across your entire build.", @"Unable To Slot Enhancement");
                            break;
                        case 1:
                            MessageBox.Show(@$"{enhancement.LongName} is mutually exclusive with {foundEnh}. You can only slot one stealth proc across your entire build.", @"Unable To Slot Enhancement");
                            break;
                    }

                    return false;
                }
            }
            if (!foundInPower)
            {
                return true;
            }

            if (!silent)
            {
                MessageBox.Show(@$"{enhancement.LongName} is already slotted in this power. You can only slot one of each enhancement from the set in a given power.", @"Unable To Slot Enhancement");
            }
            return false;
        }

        /// <summary>
        /// Builds a power containing active special enhancement effects from the source power
        /// </summary>
        /// <param name="pName">Source power (full name) from current build</param>
        /// <returns>Power with active set bonuses effects attached. Flattens effects list returned from <see cref="PowerActiveSpecialSetBonuses"/></returns>
        public Power BuildVirtualSetBonusesPower(string pName)
        {
            var power = new Power
            {
                StaticIndex = -1,
                Effects = PowerActiveSpecialSetBonuses(pName).SelectMany(e => e.Effects).ToArray()
            };

            return power;
        }

        /// <summary>
        /// Builds a list of powers from active special effects on the source power
        /// </summary>
        /// <param name="pName">Source power (full name) from current build</param>
        /// <returns>List of powers matching the active special enhancement effects</returns>
        public List<IPower> PowerActiveSpecialSetBonuses(string pName)
        {
            var pe = Powers
                .DefaultIfEmpty(null)
                .FirstOrDefault(e => e.Power != null && e.Power.FullName == pName);

            // PowerEntry -> PowerEntry.Slots -> Dictionary<int:Enhancement index, KeyValuePair<int:Enhancement set index, int:index of enhancement in set>>
            var slottedInSets = pe == null
                ? new Dictionary<int, KeyValuePair<int, int>>()
                : pe.Slots.ToDictionary(
                    e => e.Enhancement.Enh,
                    e => new KeyValuePair<int, int>(
                        e.Enhancement.Enh < 0
                            ? -1
                            : DatabaseAPI.Database.Enhancements[e.Enhancement.Enh].nIDSet,
                        e.Enhancement.Enh < 0
                            ? -1
                            : DatabaseAPI.Database.Enhancements[e.Enhancement.Enh].nIDSet < 0
                                ? -1
                                : Array.FindIndex(
                                    DatabaseAPI.Database
                                        .EnhancementSets[DatabaseAPI.Database.Enhancements[e.Enhancement.Enh].nIDSet]
                                        .Enhancements, enh => enh == e.Enhancement.Enh)
                    ));

            var activeSetBonuses = slottedInSets
                .Where(e => e.Key >= 0 && e.Value.Key >= 0 && e.Value.Value >= 0)
                .SelectMany(e => DatabaseAPI.Database.EnhancementSets[e.Value.Key].SpecialBonus[e.Value.Value].Index.Select(k => DatabaseAPI.Database.Power[k].Clone()))
                .ToList();

            // Mark all effects as from enhancement
            foreach (var s in activeSetBonuses)
            {
                foreach (var fx in s.Effects)
                {
                    fx.isEnhancementEffect = true;
                }
            }

            return activeSetBonuses;
        }

        public void GenerateSetBonusData()
        {
            SetBonus.Clear();
            for (var index1 = 0; index1 < Powers.Count; ++index1)
            {
                var i9SetData = new I9SetData
                {
                    PowerIndex = index1
                };
                if (Powers[index1] != null && Powers[index1].Level <= MidsContext.Config.ForceLevel)
                {
                    for (var index2 = 0; index2 < Powers[index1].SlotCount; ++index2)
                    {
                        i9SetData.Add(ref Powers[index1].Slots[index2].Enhancement);
                    }
                }

                i9SetData.BuildEffects(!MidsContext.Config.Inc.DisablePvE ? Enums.ePvX.PvE : Enums.ePvX.PvP);
                if (!i9SetData.Empty)
                {
                    SetBonus.Add(i9SetData);
                }
            }

            _setBonusVirtualPower = null;
        }

        private IPower GetSetBonusVirtualPower()
        {
            IPower power1 = new Power();
            if (MidsContext.Config.I9.IgnoreSetBonusFX)
            {
                return power1;
            }

            var nidPowers = DatabaseAPI.NidPowers("set_bonus");
            var setCount = new int[nidPowers.Length];
            for (var index = 0; index < setCount.Length; ++index)
            {
                setCount[index] = 0;
            }

            var skipEffects = false;
            var effectList = new List<IEffect>();
            foreach (var setBonus in SetBonus)
            {
                foreach (var setInfo in setBonus.SetInfo)
                {
                    foreach (var power in setInfo.Powers.Where(x => x > -1))
                    {
                        if (power > setCount.Length - 1)
                        {
                            throw new IndexOutOfRangeException("power to setBonusArray");
                        }

                        ++setCount[power];
                        
                        if ((DatabaseAPI.Database.Power[power].Target & Enums.eEntity.MyPet) != 0 && (DatabaseAPI.Database.Power[power].EntitiesAffected & Enums.eEntity.MyPet) != 0)
                        {
                            skipEffects = true;
                        }
                        //Console.WriteLine($"{DatabaseAPI.Database.Power[power].DisplayName} skip effects? {skipEffects}");
                        if (setCount[power] < 6)
                        {
                            effectList.AddRange(DatabaseAPI.Database.Power[power].Effects.Select(t => (IEffect)t.Clone()));
                        }
                    }
                }
            }

            power1.Effects = effectList.ToArray();
            return power1;
        }

        public List<IEffect> GetCumulativeSetBonuses()
        {
            var bonusVirtualPower = SetBonusVirtualPower;
            var fxList = new List<IEffect>();
            foreach (var effIdx in bonusVirtualPower.Effects)
            {
                if (effIdx.EffectType == Enums.eEffectType.None && string.IsNullOrEmpty(effIdx.Special)) continue;
                var index2 = GcsbCheck(fxList.ToArray(), effIdx);
                if (index2 < 0)
                {
                    var fx = (IEffect) effIdx.Clone();
                    fx.Math_Mag = effIdx.Mag;
                    fxList = fxList.Append(fx).ToList();
                }
                else
                {
                    fxList[index2].Math_Mag += effIdx.Mag;
                }
            }

            return fxList;
        }

        private static int GcsbCheck(IReadOnlyList<IEffect> fxList, IEffect testFx)
        {
            for (var index = 0; index < fxList.Count; ++index)
            {
                if (fxList[index].EffectType != testFx.EffectType || !((fxList[index].Mag > 0.0) & (testFx.Mag > 0.0)) && !((fxList[index].Mag < 0.0) & (testFx.Mag < 0.0)) && !(Math.Abs(fxList[index].Mag - ((double)testFx.Mag > 0.0 ? 1f : 0.0f)) < 0.001))
                {
                    continue;
                }

                switch (testFx.EffectType)
                {
                    case Enums.eEffectType.Mez or Enums.eEffectType.MezResist when fxList[index].MezType == testFx.MezType:
                    case Enums.eEffectType.Damage when testFx.DamageType == fxList[index].DamageType:
                    case Enums.eEffectType.Defense when testFx.DamageType == fxList[index].DamageType:
                    case Enums.eEffectType.Resistance when testFx.DamageType == fxList[index].DamageType:
                    case Enums.eEffectType.DamageBuff when testFx.DamageType == fxList[index].DamageType:
                    case Enums.eEffectType.Enhancement when testFx.ETModifies == fxList[index].ETModifies && testFx.DamageType == fxList[index].DamageType && testFx.MezType == fxList[index].MezType:
                    case Enums.eEffectType.ResEffect when testFx.ETModifies == fxList[index].ETModifies:
                    case Enums.eEffectType.None when testFx.Special == fxList[index].Special && testFx.Special.IndexOf("DEBT", StringComparison.OrdinalIgnoreCase) > -1:
                        return index;
                }
                switch ((testFx.EffectType != Enums.eEffectType.Mez) & (testFx.EffectType != Enums.eEffectType.MezResist) &
                        (testFx.EffectType != Enums.eEffectType.Damage) & (testFx.EffectType != Enums.eEffectType.Defense) &
                        (testFx.EffectType != Enums.eEffectType.Resistance) &
                        (testFx.EffectType != Enums.eEffectType.DamageBuff) &
                        (testFx.EffectType != Enums.eEffectType.Enhancement) &
                        (testFx.EffectType != Enums.eEffectType.ResEffect) &
                        (testFx.EffectType == fxList[index].EffectType))
                {
                    case true:
                        return index;
                }
            }

            return -1;
        }

        public Enums.eMutex MutexV2(int hIdx, bool silent = false, bool doDetoggle = false)
        {
            Enums.eMutex eMutex;
            if (hIdx < 0 || hIdx > Powers.Count || Powers[hIdx].Power == null)
            {
                eMutex = Enums.eMutex.NoGroup;
            }
            else
            {
                var power1 = Powers[hIdx].Power;
                if (power1.MutexIgnore)
                {
                    eMutex = Enums.eMutex.NoGroup;
                }
                else
                {
                    var powerEntryList = new List<PowerEntry?>();
                    var mutexAuto = false;
                    var index1 = -1;
                    for (var index2 = 0; index2 < DatabaseAPI.Database.MutexList.Length; index2++)
                    {
                        if (!string.Equals(DatabaseAPI.Database.MutexList[index2], "KHELDIAN_GROUP",
                            StringComparison.OrdinalIgnoreCase))
                            continue;
                        index1 = index2;
                        break;
                    }

                    var flag2 = power1.HasMutexID(index1);
                    var isKheldianShapeshift = new List<string>
                    {
                        "Peacebringer_Offensive.Luminous_Blast.Bright_Nova",
                        "Peacebringer_Defensive.Luminous_Aura.White_Dwarf",
                        "Warshade_Offensive.Umbral_Blast.Dark_Nova",
                        "Warshade_Defensive.Umbral_Aura.Black_Dwarf"
                    }.Contains(power1.FullName);
                    foreach (var power2 in Powers)
                    {
                        if (power2.Power == null || power2.Power.PowerIndex == power1.PowerIndex) continue;
                        var power3 = power2.Power;
                        if (!power2.StatInclude || power3.MutexIgnore) continue;

                        if (isKheldianShapeshift & (power2.Power.FullName.StartsWith("Temporary_Powers.Accolades.") | power2.Power.FullName.StartsWith("Incarnate.")))
                        {
                            continue;
                        }

                        if (flag2 || (power3.PowerType != Enums.ePowerType.Click || power3.PowerName == "Light_Form") && power3.HasMutexID(index1))
                        {
                            powerEntryList.Add(power2);
                            if (power3.MutexAuto)
                            {
                                mutexAuto = true;
                            }
                        }
                        else
                        {
                            foreach (var num1 in power1.NGroupMembership)
                            {
                                foreach (var num2 in power3.NGroupMembership)
                                {
                                    if (num1 != num2) continue;
                                    powerEntryList.Add(power2);
                                    if (power3.MutexAuto) mutexAuto = true;
                                }
                            }
                        }
                    }

                    if (power1.MutexAuto)
                    {
                        doDetoggle = true;
                        silent = true;
                    }

                    if (doDetoggle && power1.MutexAuto)
                    {
                        foreach (var powerEntry in powerEntryList)
                        {
                            powerEntry.StatInclude = false;
                        }

                        eMutex = Enums.eMutex.NoConflict;
                    }
                    else
                    {
                        if (doDetoggle && mutexAuto && Powers[hIdx].StatInclude)
                        {
                            Powers[hIdx].StatInclude = false;
                        }

                        if (!silent && powerEntryList.Count > 0)
                        {
                            var str1 = $"{power1.DisplayName} is mutually exclusive and can't be used at the same time as the following powers:\n{string.Join(", ", powerEntryList.Select(e => e.Power.DisplayName).ToArray())}";
                            MessageBox.Show(
                                !doDetoggle || !power1.MutexAuto || !Powers[hIdx].StatInclude
                                    ? str1 + "\n\nYou should turn off the powers listed before turning this one on."
                                    : str1 + "\n\nThe listed powers have been turned off.", @"Power Conflict");
                        }

                        eMutex = powerEntryList.Count > 0
                            ? !power1.MutexAuto
                                ? !mutexAuto ? Enums.eMutex.MutexFound : Enums.eMutex.DetoggleSlave
                                : Enums.eMutex.DetoggleMaster
                            : Enums.eMutex.NoConflict;
                    }
                }
            }

            return eMutex;
        }

        public void FullMutexCheck()
        {
            for (var hIdx = Powers.Count - 1; hIdx >= 0; hIdx += -1) MutexV2(hIdx, true, true);
        }

        public Dictionary<FXIdentifierKey, List<FXSourceData>> GetEffectSources()
        {
            var ret = new Dictionary<FXIdentifierKey, List<FXSourceData>>();

            foreach (var s in SetBonus)
            {
                for (var i = 0; i < s.SetInfo.Length; i++)
                {
                    if (s.SetInfo[i].Powers.Length <= 0) continue;

                    var enhancementSet = DatabaseAPI.Database.EnhancementSets[s.SetInfo[i].SetIDX];
                    var sourcePower = DatabaseAPI.Database
                        .Powersets[Powers[s.PowerIndex].NIDPowerset]
                        .Powers[Powers[s.PowerIndex].IDXPower];
                    var powerName = sourcePower.DisplayName;

                    for (var j = 0; j < enhancementSet.Bonus.Length; j++)
                    {
                        var pvMode = enhancementSet.Bonus[j].PvMode;
                        if (!((s.SetInfo[i].SlottedCount >= enhancementSet.Bonus[j].Slotted) &
                              ((pvMode == Enums.ePvX.Any) |
                               ((pvMode == Enums.ePvX.PvE) & !MidsContext.Config.Inc.DisablePvE) |
                               ((pvMode == Enums.ePvX.PvP) & MidsContext.Config.Inc.DisablePvE))))
                            continue;

                        var setEffectsData = enhancementSet.GetEffectDetailedData2(j, false);
                        var setLinkedPowers = enhancementSet.GetEnhancementSetLinkedPowers(j, false);

                        foreach (var e in setEffectsData)
                        {
                            var p = setLinkedPowers.First(pw => pw.FullName == e.Key);
                            foreach (var fx in e.Value)
                            {
                                var identKey = new FXIdentifierKey
                                {
                                    EffectType = fx.EffectType,
                                    MezType = fx.MezType,
                                    DamageType = fx.DamageType,
                                    TargetEffectType = fx.ETModifies,
                                };

                                if (!ret.ContainsKey(identKey))
                                {
                                    ret.Add(identKey, new List<FXSourceData>());
                                }

                                ret[identKey].Add(new FXSourceData
                                {
                                    Fx = fx,
                                    Mag = fx.Mag,
                                    EnhSet = enhancementSet.DisplayName,
                                    Power = powerName,
                                    PvMode = pvMode,
                                    IsFromEnh = false,
                                    AffectedEntity = p.EntitiesAffected,
                                    EntitiesAutoHit = p.EntitiesAutoHit
                                });
                            }
                        }
                    }

                    foreach (var si in s.SetInfo[i].EnhIndexes)
                    {
                        var specialEnhIdx = DatabaseAPI.IsSpecialEnh(si);
                        if (specialEnhIdx <= -1) continue;

                        var enhEffectsData = enhancementSet.GetEffectDetailedData2(specialEnhIdx, true);
                        var setLinkedPowers = enhancementSet.GetEnhancementSetLinkedPowers(specialEnhIdx, true);
                        foreach (var e in enhEffectsData)
                        {
                            var p = setLinkedPowers.First(pw => pw.FullName == e.Key);
                            foreach (var fx in e.Value)
                            {
                                var identKey = new FXIdentifierKey
                                {
                                    EffectType = fx.EffectType,
                                    MezType = fx.MezType,
                                    DamageType = fx.DamageType,
                                    TargetEffectType = fx.ETModifies
                                };

                                if (!ret.ContainsKey(identKey)) ret.Add(identKey, new List<FXSourceData>());
                                ret[identKey].Add(new FXSourceData
                                {
                                    Fx = fx,
                                    Mag = fx.Mag,
                                    EnhSet = enhancementSet.DisplayName,
                                    Power = powerName,
                                    PvMode = Enums.ePvX.Any,
                                    IsFromEnh = true,
                                    AffectedEntity = p.EntitiesAffected,
                                    EntitiesAutoHit = p.EntitiesAutoHit,
                                    Enhancement = DatabaseAPI.Database.Enhancements[si]
                                });
                            }
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

        #region EDFigures sub-class

        public static class EDFigures
        {
            // Imperted and improved from DataView.cs
            public enum EDStrength
            {
                None,
                Light,
                Medium,
                Strong
            }

            public struct EDWeightedItem
            {
                public string StatName;
                public float Value;
                public Enums.eSchedule Schedule;
                public EDStrength EDStrength;
                public float PostEDValue;
                public EDValueSettings EDSettings;
            }

            public struct EDFiguresGroups
            {
                public List<EDWeightedItem> Buffs;
                public List<EDWeightedItem> Debuffs;
                public List<EDWeightedItem> BuffDebuffs;
                public List<EDWeightedItem> Mez;
            }

            public struct EDValueSettings
            {
                public EDStrength EDStrength;
                public string PreEDValue;
                public string PostEDValue;
                public string ShortText;
                public string TooltipText;
                public bool Valid;
            }

            private static EDValueSettings BuildEDItem(float value, Enums.eSchedule schedule, string name, float afterED, bool useRtf = false)
            {
                var flag1 = value > (double)DatabaseAPI.Database.MultED[(int)schedule][0];
                var flag2 = value > (double)DatabaseAPI.Database.MultED[(int)schedule][1];
                var specialCase = value > (double)DatabaseAPI.Database.MultED[(int)schedule][2];

                var ret = new EDValueSettings
                {
                    EDStrength = EDStrength.None,
                    PreEDValue = "",
                    PostEDValue = "",
                    ShortText = "",
                    TooltipText = "",
                    Valid = true
                };

                if (value <= 0)
                {
                    ret.Valid = false;

                    return ret;
                }

                var valuePercent = value * 100;
                var valuePercentAfterED = Enhancement.ApplyED(schedule, value) * 100;
                var postEDValue = valuePercentAfterED + afterED * 100;
                var valueDiff = (float)Math.Round(valuePercent - valuePercentAfterED, 3);
                var preEDValue = valuePercent + afterED * 100;

                var totalEffectStr = $"Total Effect: {preEDValue:P2}\r\nWith ED Applied: {postEDValue:P2}\r\n\r\n";
                var infoText = "";
                var tooltipText = "";
                var edStrength = EDStrength.None;
                if (valueDiff > 0)
                {
                    infoText = $"{postEDValue:P2} (Pre-ED: {preEDValue:P2})";
                    if (afterED > 0)
                    {
                        totalEffectStr += $"Amount from pre-ED sources: {valuePercent:P2}\r\n";
                    }

                    tooltipText = $"{totalEffectStr} ED reduction: {valueDiff:P2} ({valueDiff / valuePercent * 100:P2} of total)\r\n";


                    if (specialCase)
                    {
                        tooltipText += $" The highest level of ED reduction is being applied.\r\nThreshold: {DatabaseAPI.Database.MultED[(int)schedule][2] * 100:P2}\r\n";
                        edStrength = EDStrength.Strong;
                    }
                    else if (flag2)
                    {
                        tooltipText += $" The middle level of ED reduction is being applied.\r\nThreshold: {(DatabaseAPI.Database.MultED[(int)schedule][1] * 100):P2}\r\n";
                        edStrength = EDStrength.Medium;
                    }
                    else if (flag1)
                    {
                        tooltipText += $" The lowest level of ED reduction is being applied.\r\nThreshold: {(DatabaseAPI.Database.MultED[(int)schedule][0] * 100):P2}\r\n";
                        edStrength = EDStrength.Light;
                    }

                    if (afterED > 0)
                    {
                        tooltipText += $" Amount from post-ED sources: {Convert.ToDecimal(afterED * 100):P2}\r\n";
                    }
                }
                else
                {
                    infoText = $"{postEDValue:P2}";
                    if (afterED > 0)
                    {
                        totalEffectStr += $" Amount from post-ED sources: {Convert.ToDecimal(afterED * 100):P2}\r\n";
                    }

                    tooltipText = $"{totalEffectStr}This effect has not been affected by ED.\r\n";
                }

                return useRtf
                    ? new EDValueSettings
                    {
                        PreEDValue = $"{valuePercent + afterED * 100:P2}",
                        PostEDValue = $"{postEDValue:P2}",
                        EDStrength = edStrength,
                        ShortText = edStrength switch
                        {
                            EDStrength.Light => RTF.Color(RTF.ElementID.Enhancement), // Green
                            EDStrength.Medium => RTF.Color(RTF.ElementID.Alert), // Yellow
                            EDStrength.Strong => RTF.Color(RTF.ElementID.Warning), // Red
                            _ => RTF.Color(RTF.ElementID.Text)
                        } + $"{RTF.Bold(name)}: {infoText}{RTF.Color(RTF.ElementID.Text)}",
                        TooltipText = tooltipText,
                        Valid = true
                    }
                    : new EDValueSettings
                    {
                        PreEDValue = $"{valuePercent + afterED * 100:P2}",
                        PostEDValue = $"{postEDValue:P2}",
                        EDStrength = edStrength,
                        ShortText = $"{name}: {infoText}",
                        TooltipText = tooltipText,
                        Valid = true
                    };
            }

            public static EDFiguresGroups GetBuffsForBuildPower(int historyIdx = -1)
            {
                var ret = new EDFiguresGroups
                {
                    Buffs = new List<EDWeightedItem>(),
                    Debuffs = new List<EDWeightedItem>(),
                    BuffDebuffs = new List<EDWeightedItem>(),
                    Mez = new List<EDWeightedItem>()
                };

                if (MidsContext.Character == null)
                {
                    return ret;
                }

                if (MidsContext.Character.CurrentBuild == null)
                {
                    return ret;
                }

                if (MidsContext.Character.CurrentBuild.Powers == null)
                {
                    return ret;
                }

                if (historyIdx < 0 | historyIdx > MidsContext.Character.CurrentBuild.Powers.Count)
                {
                    return ret;
                }

                if (MidsContext.Character.CurrentBuild.Powers[historyIdx] == null)
                {
                    return ret;
                }

                var eEnhs = Enum.GetValues(typeof(Enums.eEnhance)).Length;
                var eMezzes = Enum.GetValues(typeof(Enums.eMez)).Length;

                var buffValues = new List<float>(eEnhs);
                var buffSchedules = new List<Enums.eSchedule>(eEnhs);
                var buffValuesAfterED = new List<float>(eEnhs);
                
                var debuffValues = new List<float>(eEnhs);
                var debuffSchedules = new List<Enums.eSchedule>(eEnhs);
                var debuffValuesAfterED = new List<float>(eEnhs);

                var buffDebuffValues = new List<float>(eEnhs);
                var buffDebuffSchedules = new List<Enums.eSchedule>(eEnhs);
                var buffDebuffValuesAfterED = new List<float>(eEnhs);

                var mezValues = new List<float>(eMezzes);
                var mezSchedules = new List<Enums.eSchedule>(eEnhs);
                var mezValuesAfterED = new List<float>(eEnhs);

                for (var i = 0; i < eEnhs; i++)
                {
                    buffValues[i] = 0;
                    debuffValues[i] = 0;
                    buffDebuffValues[i] = 0;
                    buffSchedules[i] = Enhancement.GetSchedule((Enums.eEnhance)i);
                    debuffSchedules[i] = buffSchedules[i];
                    buffDebuffSchedules[i] = buffSchedules[i];
                }

                debuffSchedules[(int)Enums.eEnhance.Defense] = Enums.eSchedule.A; // 3
                for (var tSub = 0; tSub < eMezzes; tSub++)
                {
                    mezValues[tSub] = 0;
                    mezSchedules[tSub] = Enhancement.GetSchedule(Enums.eEnhance.Mez, tSub);
                }

                var refPower = MidsContext.Character.CurrentBuild.Powers[historyIdx];
                for (var index1 = 0; index1 < refPower.SlotCount; index1++)
                {
                    var slot = refPower.Slots[index1];
                    if (slot.Enhancement.Enh <= -1) continue;

                    var enh = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh];
                    for (var index2 = 0; index2 < enh.Effect.Length; index2++)
                    {
                        var effects = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].Effect;
                        if (effects[index2].Mode != Enums.eEffMode.Enhancement)
                        {
                            continue;
                        }

                        if (effects[index2].Enhance.ID == (int)Enums.eEnhance.Mez) // 12
                        {
                            mezValues[effects[index2].Enhance.SubID] += slot.Enhancement.GetEnhancementEffect(Enums.eEnhance.Mez, effects[index2].Enhance.SubID, 1);
                        }
                        else
                        {
                            switch (enh.Effect[index2].BuffMode)
                            {
                                case Enums.eBuffDebuff.BuffOnly:
                                    buffValues[effects[index2].Enhance.ID] += slot.Enhancement.GetEnhancementEffect((Enums.eEnhance)effects[index2].Enhance.ID, -1, 1);
                                    break;
                                case Enums.eBuffDebuff.DeBuffOnly:
                                    if ((effects[index2].Enhance.ID != (int)Enums.eEnhance.SpeedFlying) &
                                        (effects[index2].Enhance.ID != (int)Enums.eEnhance.SpeedRunning) &
                                        (effects[index2].Enhance.ID != (int)Enums.eEnhance.SpeedJumping)) // 6, 19, 11
                                    {
                                        debuffValues[effects[index2].Enhance.ID] += slot.Enhancement.GetEnhancementEffect((Enums.eEnhance)effects[index2].Enhance.ID, -1, -1);
                                    }

                                    break;
                                default:
                                    buffDebuffValues[effects[index2].Enhance.ID] += slot.Enhancement.GetEnhancementEffect((Enums.eEnhance)effects[index2].Enhance.ID, -1, 1f);
                                    break;
                            }
                        }
                    }
                }

                foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
                {
                    if (powerEntry.Power == null || !powerEntry.StatInclude)
                    {
                        continue;
                    }

                    IPower power1 = new Power(powerEntry.Power);
                    power1.AbsorbPetEffects();
                    power1.ApplyGrantPowerEffects();
                    foreach (var effect in power1.Effects)
                    {
                        if ((power1.PowerType != Enums.ePowerType.GlobalBoost) & (!effect.Absorbed_Effect | (effect.Absorbed_PowerType != Enums.ePowerType.GlobalBoost)))
                        {
                            continue;
                        }

                        // var power2 = power1;
                        var power2 = effect.Absorbed_Effect & (effect.Absorbed_Power_nID > -1)
                            ? DatabaseAPI.Database.Power[effect.Absorbed_Power_nID]
                            : new Power(powerEntry.Power);

                        var eBuffDebuff = Enums.eBuffDebuff.Any;
                        var buffDebuffFound = false;
                        foreach (var str1 in MidsContext.Character.CurrentBuild.Powers[historyIdx].Power.BoostsAllowed)
                        {
                            if (power2.BoostsAllowed.All(str2 => str1 != str2)) continue;

                            if (str1.Contains("Buff"))
                            {
                                eBuffDebuff = Enums.eBuffDebuff.BuffOnly;
                            }

                            if (str1.Contains("Debuff"))
                            {
                                eBuffDebuff = Enums.eBuffDebuff.DeBuffOnly;
                            }

                            buffDebuffFound = true;
                            break;

                            //if (buffDebuffFound)
                            //    break;
                        }

                        if (!buffDebuffFound)
                        {
                            continue;
                        }

                        if (effect.EffectType == Enums.eEffectType.Enhancement)
                        {
                            switch (effect.ETModifies)
                            {
                                case Enums.eEffectType.Defense:
                                    if (effect.DamageType == Enums.eDamage.Smashing)
                                    {
                                        if (effect.IgnoreED)
                                        {
                                            switch (eBuffDebuff)
                                            {
                                                case Enums.eBuffDebuff.BuffOnly:
                                                    buffValuesAfterED[(int)Enums.eEnhance.Defense] += effect.BuffedMag; // 3
                                                    break;
                                                case Enums.eBuffDebuff.DeBuffOnly:
                                                    debuffValuesAfterED[(int)Enums.eEnhance.Defense] += effect.BuffedMag; // 3
                                                    break;
                                                default:
                                                    buffDebuffValuesAfterED[(int)Enums.eEnhance.Defense] += effect.BuffedMag; // 3
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            switch (eBuffDebuff)
                                            {
                                                case Enums.eBuffDebuff.BuffOnly:
                                                    buffValues[(int)Enums.eEnhance.Defense] += effect.BuffedMag; // 3
                                                    break;
                                                case Enums.eBuffDebuff.DeBuffOnly:
                                                    debuffValues[(int)Enums.eEnhance.Defense] += effect.BuffedMag; // 3
                                                    break;
                                                default:
                                                    buffDebuffValues[(int)Enums.eEnhance.Defense] += effect.BuffedMag; // 3
                                                    break;
                                            }
                                        }
                                    }

                                    break;
                                case Enums.eEffectType.Mez:
                                    if (effect.IgnoreED)
                                    {
                                        mezValuesAfterED[(int)effect.MezType] += effect.BuffedMag;
                                        break;
                                    }

                                    mezValues[(int)effect.MezType] += effect.BuffedMag;
                                    break;
                                default:
                                    var index2 = effect.ETModifies != Enums.eEffectType.RechargeTime
                                        ? Convert.ToInt32(Enum.Parse(typeof(Enums.eEnhance), effect.ETModifies.ToString()))
                                        : (int)Enums.eEnhance.RechargeTime; // 14
                                    if (effect.IgnoreED)
                                    {
                                        buffDebuffValuesAfterED[index2] += effect.BuffedMag;
                                        break;
                                    }

                                    buffDebuffValues[index2] += effect.BuffedMag;
                                    break;
                            }
                        }
                        else if ((effect.EffectType == Enums.eEffectType.DamageBuff) & (effect.DamageType == Enums.eDamage.Smashing))
                        {
                            if (effect.IgnoreED)
                            {
                                foreach (var str in power2.BoostsAllowed)
                                {
                                    if (str.StartsWith("Res_Damage"))
                                    {
                                        buffDebuffValuesAfterED[(int)Enums.eEnhance.Resistance] += effect.BuffedMag; // 18
                                        break;
                                    }

                                    if (!str.StartsWith("Damage"))
                                    {
                                        continue;
                                    }

                                    buffDebuffValuesAfterED[(int)Enums.eEnhance.Damage] += effect.BuffedMag; // 2
                                    break;
                                }
                            }
                            else
                            {
                                foreach (var str in power2.BoostsAllowed)
                                {
                                    if (str.StartsWith("Res_Damage"))
                                    {
                                        buffDebuffValues[(int)Enums.eEnhance.Resistance] += effect.BuffedMag; // 18
                                        break;
                                    }

                                    if (!str.StartsWith("Damage"))
                                    {
                                        continue;
                                    }

                                    buffDebuffValues[(int)Enums.eEnhance.Damage] += effect.BuffedMag; // 2
                                    break;
                                }
                            }
                        }
                    }
                }

                buffValues[(int)Enums.eEnhance.HitPoints] = 0; // 8
                debuffValues[(int)Enums.eEnhance.HitPoints] = 0; // 8
                buffDebuffValues[(int)Enums.eEnhance.HitPoints] = 0; // 8

                buffValues[(int)Enums.eEnhance.Regeneration] = 0; // 17
                debuffValues[(int)Enums.eEnhance.Regeneration] = 0; // 17
                buffDebuffValues[(int)Enums.eEnhance.Regeneration] = 0; // 17

                buffValues[(int)Enums.eEnhance.Recovery] = 0; // 16
                debuffValues[(int)Enums.eEnhance.Recovery] = 0; // 16
                buffDebuffValues[(int)Enums.eEnhance.Recovery] = 0; // 16

                var statNames = Enum.GetNames(typeof(Enums.eEnhance));
                for (var i = 0; i < eEnhs; i++)
                {
                    if (Math.Abs(buffValues[i]) > float.Epsilon)
                    {
                        var edSettings = BuildEDItem(buffValues[i], buffSchedules[i], statNames[i], buffValuesAfterED[i]);
                        ret.Buffs.Add(new EDWeightedItem
                        {
                            StatName = statNames[i],
                            Value = buffValues[i],
                            Schedule = buffSchedules[i],
                            PostEDValue = buffValuesAfterED[i],
                            EDStrength = edSettings.EDStrength,
                            EDSettings = edSettings
                        });
                    }

                    if (Math.Abs(debuffValues[i]) > float.Epsilon)
                    {
                        var edSettings = BuildEDItem(debuffValues[i], debuffSchedules[i], statNames[i], debuffValuesAfterED[i]);
                        ret.Debuffs.Add(new EDWeightedItem
                        {
                            StatName = statNames[i],
                            Value = debuffValues[i],
                            Schedule = debuffSchedules[i],
                            PostEDValue = debuffValuesAfterED[i],
                            EDStrength = edSettings.EDStrength,
                            EDSettings = edSettings
                        });
                    }

                    if (Math.Abs(buffDebuffValues[i]) > float.Epsilon)
                    {
                        var edSettings = BuildEDItem(buffDebuffValues[i], buffDebuffSchedules[i], statNames[i], buffDebuffValuesAfterED[i]);
                        ret.BuffDebuffs.Add(new EDWeightedItem
                        {
                            StatName = statNames[i],
                            Value = buffDebuffValues[i],
                            Schedule = buffDebuffSchedules[i],
                            PostEDValue = buffDebuffValuesAfterED[i],
                            EDStrength = edSettings.EDStrength,
                            EDSettings = edSettings
                        });
                    }
                }

                statNames = Enum.GetNames(typeof(Enums.eMez));
                for (var i = 0; i < eMezzes; i++)
                {
                    if (Math.Abs(mezValues[i]) > float.Epsilon)
                    {
                        var edSettings = BuildEDItem(mezValues[i], mezSchedules[i], statNames[i], mezValuesAfterED[i]);
                        ret.Mez.Add(new EDWeightedItem
                        {
                            StatName = statNames[i],
                            Value = mezValues[i],
                            Schedule = mezSchedules[i],
                            PostEDValue = mezValuesAfterED[i],
                            EDStrength = edSettings.EDStrength,
                            EDSettings = edSettings
                        });
                    }
                }

                return ret;
            }
        }

        #endregion

        #region FXIdentifierKey sub-class
        public class FXIdentifierKey
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
                        EffectType == Enums.eEffectType.ResEffect |
                        (EffectType == Enums.eEffectType.Enhancement & TargetEffectType == Enums.eEffectType.None & MezType != Enums.eMez.None))
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

        public class FXSourceData
        {
            private IEffect _fx;
            public IEffect Fx
            {
                get => _fx;
                set => _fx = (IEffect)value.Clone();
            }
            public float Mag { get; set; }
            public string EnhSet { get; set; }
            public string Power { get; set; }
            public Enums.ePvX PvMode { get; set; }
            public bool IsFromEnh { get; set; }
            public Enums.eEntity AffectedEntity { get; set; }
            public Enums.eEntity EntitiesAutoHit { get; set; }
            public IEnhancement Enhancement { get; set; }
        }
        #endregion
    }
}