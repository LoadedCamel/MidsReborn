
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Base.Data_Classes;
using Base.Display;
using Base.Master_Classes;
using HeroDesigner.Schema;

public class Build
{
    readonly Character _character;

    public readonly List<PowerEntry> Powers;
    public readonly List<I9SetData> SetBonus;
    public EnhancementSet MySet;
    private string setName { get; set; }
    public static HashSet<string> setSlotted = new HashSet<string>();

    IPower _setBonusVirtualPower;

    public int LastPower;

    public string compareStringSlottedEnh;

    public IPower SetBonusVirtualPower
    {
        get
        {
            IPower power;
            if ((power = _setBonusVirtualPower) == null)
                power = _setBonusVirtualPower = GetSetBonusVirtualPower();
            return power;
        }
    }

    public int PowersPlaced
    {
        get
        {
            int pplaced = 0;
            foreach (PowerEntry power in Powers)
            {
                if (power.Chosen && power.Power != null)
                    ++pplaced;
            }

            return pplaced;
        }
    }

    public int SlotsPlaced
    {
        get
        {
            int placed = 0;
            foreach (PowerEntry power in Powers)
            {
                if (power.Slots.Length > 1)
                    placed += power.Slots.Length - 1;
            }

            return placed;
        }
    }

    public int TotalSlotsAvailable
    {
        get { return DatabaseAPI.Database.Levels.Sum(level => level.Slots); }
    }

    public PowerEntry AddPower(IPower power, int specialLevel = -1)
    {
        PowerEntry powerEntry = GetPowerEntry(power);
        if (powerEntry == null)
        {
            powerEntry = new PowerEntry(specialLevel, power);
            Powers.Add(powerEntry);
        }

        powerEntry.ValidateSlots();
        return powerEntry;
    }

    public void RemovePower(IPower powerToRemove)
    {
        foreach (PowerEntry power in Powers)
        {
            if (power?.Power == null || power.Power.PowerIndex != powerToRemove.PowerIndex)
                continue;
            power.Reset();
            break;
        }
    }

    PowerEntry GetPowerEntry(IPower power)
    {
        foreach (PowerEntry power1 in Powers)
        {
            if (power1.Power != null && power1.Power.PowerIndex == power.PowerIndex)
                return power1;
        }

        return null;
    }

    public Build(Character owner, IList<LevelMap> iLevels)
    {
        _character = owner;
        Powers = new List<PowerEntry>
        {
            new PowerEntry(0, null, true)
        };
        SetBonus = new List<I9SetData>();
        LastPower = 0;
        for (int iLevel = 0; iLevel < iLevels.Count; ++iLevel)
        {
            for (int index = 0; index < iLevels[iLevel].Powers; ++index)
            {
                Powers.Add(new PowerEntry(iLevel, null, true));
                ++LastPower;
            }
        }
    }

    public void RemoveSlotFromPower(int powerIdx, int slotIdx)
    {
        if (powerIdx < 0 || powerIdx > Powers.Count - 1 || (slotIdx < 0 || slotIdx > Powers[powerIdx].Slots.Length - 1))
            return;
        int index1 = -1;
        SlotEntry[] slotEntryArray = new SlotEntry[Powers[powerIdx].Slots.Length - 1];
        for (int i = 0; i <= Powers[powerIdx].Slots.Length - 1; ++i)
        {
            if (i == slotIdx)
                continue;
            ++index1;
            slotEntryArray[index1].Assign(Powers[powerIdx].Slots[i]);
        }

        Powers[powerIdx].Slots = new SlotEntry[slotEntryArray.Length];
        for (int index2 = 0; index2 <= Powers[powerIdx].Slots.Length - 1; ++index2)
            Powers[powerIdx].Slots[index2].Assign(slotEntryArray[index2]);
    }

    void FillMissingSubPowers()
    {
        foreach (PowerEntry power in Powers)
        {
            if (power.Power == null || power.Power.NIDSubPower.Length <= 0)
                continue;
            if (power.SubPowers.Length != power.Power.NIDSubPower.Length)
                power.SubPowers = new PowerSubEntry[power.Power.NIDSubPower.Length];
            for (int index = 0; index <= power.Power.NIDSubPower.Length - 1; ++index)
            {
                if (power.SubPowers[index]?.nIDPower == power.Power.NIDSubPower[index])
                    continue;
                power.SubPowers[index] = new PowerSubEntry
                {
                    nIDPower = power.Power.NIDSubPower[index]
                };
                if (power.Power.NIDSubPower[index] > -1)
                {
                    power.SubPowers[index].Powerset = DatabaseAPI.Database.Power[power.Power.NIDSubPower[index]].PowerSetID;
                    power.SubPowers[index].Power = DatabaseAPI.Database.Power[power.Power.NIDSubPower[index]].PowerSetIndex;
                }
                else
                {
                    power.SubPowers[index].Powerset = -1;
                    power.SubPowers[index].Power = -1;
                }
            }
        }
    }

    void ValidateEnhancements()
    {
        foreach (PowerEntry power in Powers)
        {
            foreach (SlotEntry slot in power.Slots)
            {
                if (slot.Enhancement.Enh > -1)
                {
                    if (DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID == Enums.eType.Normal &&
                        slot.Enhancement.Grade <= Enums.eEnhGrade.None | slot.Enhancement.Grade > Enums.eEnhGrade.SingleO)
                        slot.Enhancement.Grade = Enums.eEnhGrade.SingleO;
                    if (DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID == Enums.eType.Normal |
                        DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID == Enums.eType.SpecialO &&
                        slot.Enhancement.RelativeLevel < Enums.eEnhRelative.None |
                        slot.Enhancement.RelativeLevel > Enums.eEnhRelative.PlusFive)
                        slot.Enhancement.RelativeLevel = MidsContext.Config.CalcEnhLevel;
                }

                if (slot.FlippedEnhancement.Enh <= -1)
                    continue;
                if (DatabaseAPI.Database.Enhancements[slot.FlippedEnhancement.Enh].TypeID == Enums.eType.Normal &&
                    slot.FlippedEnhancement.Grade <= Enums.eEnhGrade.None | slot.FlippedEnhancement.Grade > Enums.eEnhGrade.SingleO)
                    slot.FlippedEnhancement.Grade = Enums.eEnhGrade.SingleO;
                if (DatabaseAPI.Database.Enhancements[slot.FlippedEnhancement.Enh].TypeID == Enums.eType.Normal |
                    DatabaseAPI.Database.Enhancements[slot.FlippedEnhancement.Enh].TypeID == Enums.eType.SpecialO &&
                    slot.FlippedEnhancement.RelativeLevel < Enums.eEnhRelative.None |
                    slot.FlippedEnhancement.RelativeLevel > Enums.eEnhRelative.PlusFive)
                    slot.FlippedEnhancement.RelativeLevel = MidsContext.Config.CalcEnhLevel;
            }
        }
    }

    public bool SetEnhGrades(Enums.eEnhGrade newVal)
    {
        var str = newVal switch
        {
            Enums.eEnhGrade.None => "This value should never be passed to the function!",
            Enums.eEnhGrade.TrainingO => "Training",
            Enums.eEnhGrade.DualO => "Dual",
            Enums.eEnhGrade.SingleO => "Single",
            _ => string.Empty
        };

        if (MessageBox.Show(
                $@"Really set all placed Regular enhancements to {str} Origin?\n\nThis will not affect any Invention or Special enhancements.",
                @"Are you sure?", MessageBoxButtons.YesNo) != DialogResult.Yes)
            return false;
        foreach (PowerEntry power in Powers)
        {
            foreach (SlotEntry slot in power.Slots)
            {
                if (slot.Enhancement.Enh > -1 && DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID == Enums.eType.Normal)
                    slot.Enhancement.Grade = newVal;
            }
        }

        return true;
    }

    public bool SetIOLevels(int newVal, bool iSetMin, bool iSetMax)
    {
        string text;
        if (!iSetMin & !iSetMax)
            text = "Really set all placed Invention and Set enhancements to level " + newVal + 1 +
                   "?\n\nNote: Enhancements which are not available at the default level will be set to the closest value.";
        else if (iSetMin)
        {
            newVal = 0;
            text = "Really set all placed Invention and Set enhancements to their minimum possible level?";
        }
        else
        {
            newVal = 52;
            text = "Really set all placed Invention and Set enhancements to their maximum possible level?";
        }

        if (MessageBox.Show(text, "Are you sure?", MessageBoxButtons.YesNo) != DialogResult.Yes)
            return false;
        foreach (PowerEntry power in Powers)
        {
            foreach (SlotEntry slot in power.Slots)
            {
                if (slot.Enhancement.Enh <= -1)
                    continue;
                switch (DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].TypeID)
                {
                    case Enums.eType.InventO:
                        int levelMin1 = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].LevelMin;
                        int levelMax = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].LevelMax;
                        if (newVal >= levelMin1 & newVal <= levelMax)
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
                        {
                            slot.Enhancement.IOLevel = Enhancement.GranularLevelZb(levelMin1, levelMin1, levelMax);
                        }

                        break;
                    case Enums.eType.SetO:
                        int levelMin2 = DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].nIDSet].LevelMin;
                        int num = DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].nIDSet].LevelMax;
                        if (num > 49)
                            num = 49;
                        if (newVal >= levelMin2 & newVal <= num)
                        {
                            slot.Enhancement.IOLevel = newVal;
                            break;
                        }

                        if (newVal > num)
                        {
                            slot.Enhancement.IOLevel = num;
                            break;
                        }

                        if (newVal < levelMin2)
                        {
                            slot.Enhancement.IOLevel = levelMin2;
                        }

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
        }

        return true;
    }

    public HistoryMap[] BuildHistoryMap(bool enhNames, bool ioLevel = true)
    {
        var historyMapList = new List<HistoryMap>();
        for (int lvlIdx = 0; lvlIdx <= DatabaseAPI.Database.Levels.Length - 1; ++lvlIdx)
        {
            for (int powerIdx = 0; powerIdx <= Powers.Count - 1; ++powerIdx)
            {
                PowerEntry power = Powers[powerIdx];
                if ((!power.Chosen && power.SubPowers.Length != 0 && power.SlotCount != 0) || power.Level != lvlIdx || power.Power == null)
                    continue;
                var historyMap = new HistoryMap
                {
                    Level = lvlIdx, HID = powerIdx
                };
                string appendText = string.Empty;
                string choiceText = power.Chosen ? "Added" : "Received";
                if (power.Slots.Length > 0)
                {
                    historyMap.SID = 0;
                    if (power.Slots[0].Enhancement.Enh > -1)
                    {
                        if (enhNames)
                            appendText = " [" + DatabaseAPI.GetEnhancementNameShortWSet(power.Slots[0].Enhancement.Enh);
                        if (ioLevel && (DatabaseAPI.Database.Enhancements[power.Slots[0].Enhancement.Enh].TypeID == Enums.eType.InventO ||
                                        DatabaseAPI.Database.Enhancements[power.Slots[0].Enhancement.Enh].TypeID == Enums.eType.SetO))
                            appendText = appendText + "-" + power.Slots[0].Enhancement.IOLevel;
                        appendText += "]";
                    }
                    else if (enhNames)
                        appendText = " [Empty]";
                }

                historyMap.Text = "Level " + (lvlIdx + 1) + ": " + choiceText + " " + power.Power.DisplayName + " (" +
                                  Enum.GetName(DatabaseAPI.Database.Powersets[power.NIDPowerset].SetType.GetType(),
                                      DatabaseAPI.Database.Powersets[power.NIDPowerset].SetType) + ")" + appendText;
                historyMapList.Add(historyMap);
            }

            for (int powerIdx = 0; powerIdx <= Powers.Count - 1; ++powerIdx)
            {
                PowerEntry power = Powers[powerIdx];
                for (int slotIdx = 1; slotIdx <= power.Slots.Length - 1; ++slotIdx)
                {
                    if (power.Slots[slotIdx].Level != lvlIdx)
                        continue;
                    HistoryMap historyMap = new HistoryMap
                    {
                        Level = lvlIdx, HID = powerIdx, SID = slotIdx
                    };
                    string str = string.Empty;
                    if (power.Slots[slotIdx].Enhancement.Enh > -1)
                    {
                        if (enhNames)
                            str = " [" + DatabaseAPI.GetEnhancementNameShortWSet(power.Slots[slotIdx].Enhancement.Enh);
                        if (ioLevel &&
                            (DatabaseAPI.Database.Enhancements[power.Slots[slotIdx].Enhancement.Enh].TypeID == Enums.eType.InventO ||
                             DatabaseAPI.Database.Enhancements[power.Slots[slotIdx].Enhancement.Enh].TypeID == Enums.eType.SetO))
                            str = str + "-" + power.Slots[slotIdx].Enhancement.IOLevel;
                        str += "]";
                    }
                    else if (enhNames)
                        str = " [Empty]";

                    historyMap.Text = "Level " + (lvlIdx + 1) + ": Added Slot to " + power.Power.DisplayName + str;
                    historyMapList.Add(historyMap);
                }
            }
        }

        return historyMapList.ToArray();
    }

    int SlotsAtLevel(int powerEntryId, int iLevel)
    {
        return powerEntryId < 0 ? 0 : Powers[powerEntryId].Slots.Count(slot => slot.Level <= iLevel);
    }

    public int SlotsPlacedAtLevel(int level)
    {
        int slotsPlacedAtLevel = 0;
        foreach (var powerIdx in Powers)
        {
            for (int slotIdx = 0; slotIdx < powerIdx.Slots.Length; ++slotIdx)
            {
                if (powerIdx.Slots[slotIdx].Level == level)
                    ++slotsPlacedAtLevel;
            }
        }

        return slotsPlacedAtLevel;
    }

    public PopUp.PopupData GetRespecHelper(bool longFormat, int iLevel = 49)
    {
        PopUp.PopupData popupData = new PopUp.PopupData();
        HistoryMap[] historyMapArray = BuildHistoryMap(true);
        int index1 = popupData.Add();
        popupData.Sections[index1].Add("Respec to level: " + (iLevel + 1), PopUp.Colors.Effect, 1.25f);
        foreach (HistoryMap historyMap in historyMapArray)
        {
            if (historyMap.HID <= -1 || DatabaseAPI.Database.Levels[historyMap.Level].Powers <= 0 || historyMap.Level > iLevel)
                continue;
            PowerEntry power = Powers[historyMap.HID];
            if (power.Slots.Length <= 0)
                continue;
            int index2 = popupData.Add();
            string iText1;
            if (power.Power != null)
                iText1 = "Level " + (historyMap.Level + 1) + ": " + power.Power.DisplayName;
            else
                iText1 = "Level " + (historyMap.Level + 1) + ": [Empty]";
            popupData.Sections[index2].Add(iText1, PopUp.Colors.Text);
            int slots = SlotsAtLevel(historyMap.HID, iLevel);
            if (slots <= 0)
                continue;
            popupData.Sections[index2].Add("Slots: " + slots, PopUp.Colors.Text, 1f, FontStyle.Regular, 1);
            if (!longFormat)
                continue;
            for (int slotIdx = 0; slotIdx <= slots - 1; ++slotIdx)
            {
                string str = slotIdx != 0 ? "(" + (power.Slots[slotIdx].Level + 1) + ") " : "(A) ";
                string iText2;
                if (power.Slots[slotIdx].Enhancement.Enh > -1)
                {
                    iText2 = str + DatabaseAPI.GetEnhancementNameShortWSet(power.Slots[slotIdx].Enhancement.Enh);
                    if (DatabaseAPI.Database.Enhancements[power.Slots[slotIdx].Enhancement.Enh].TypeID == Enums.eType.InventO ||
                        DatabaseAPI.Database.Enhancements[power.Slots[slotIdx].Enhancement.Enh].TypeID == Enums.eType.SetO)
                        iText2 = iText2 + "-" + (power.Slots[slotIdx].Enhancement.IOLevel + 1);
                }
                else
                    iText2 = str + "[Empty]";

                popupData.Sections[index2].Add(iText2, PopUp.Colors.Invention, 1f, FontStyle.Regular, 2);
            }
        }

        return popupData;
    }

    public PopUp.PopupData GetRespecHelper2(bool longFormat, int iLevel = 49)
    {
        PopUp.PopupData popupData = new PopUp.PopupData();
        HistoryMap[] historyMapArray = BuildHistoryMap(true);
        int index = popupData.Add();
        popupData.Sections[index].Add("Respec to level: " + (iLevel + 1), PopUp.Colors.Effect, 1.25f);
        int histLvl = 0;
        foreach (HistoryMap historyMap in historyMapArray)
        {
            if (histLvl != historyMap.Level)
                index = popupData.Add();
            histLvl = historyMap.Level;
            if (historyMap.HID < 0)
                continue;
            PowerEntry power = Powers[historyMap.HID];
            if (DatabaseAPI.Database.Levels[historyMap.Level].Powers > 0 & historyMap.Level <= iLevel)
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
                string empty = string.Empty;
                string iText2;
                if (power.Slots[historyMap.SID].Enhancement.Enh > -1)
                {
                    iText2 = empty + DatabaseAPI.GetEnhancementNameShortWSet(power.Slots[historyMap.SID].Enhancement.Enh);
                    if (DatabaseAPI.Database.Enhancements[power.Slots[historyMap.SID].Enhancement.Enh].TypeID == Enums.eType.InventO ||
                        DatabaseAPI.Database.Enhancements[power.Slots[historyMap.SID].Enhancement.Enh].TypeID == Enums.eType.SetO)
                        iText2 = iText2 + "-" + (power.Slots[historyMap.SID].Enhancement.IOLevel + 1);
                }
                else
                    iText2 = empty + "[Empty]";

                popupData.Sections[index].Add(iText2, PopUp.Colors.Invention, 1f, FontStyle.Regular, 1);
            }
            else if (DatabaseAPI.Database.Levels[historyMap.Level].Slots > 0 & historyMap.Level <= iLevel && historyMap.SID > -1)
            {
                string str = historyMap.SID != 0
                    ? "Level " + (historyMap.Level + 1) + ": Added Slot To "
                    : "Level " + (historyMap.Level + 1) + ": Received Slot - ";
                string iText1 = power.Power == null ? str + "[Empty]" : str + power.Power.DisplayName;
                popupData.Sections[index].Add(iText1, PopUp.Colors.Effect);
                if (!longFormat)
                    continue;
                string iText2;
                if (power.Slots[historyMap.SID].Enhancement.Enh > -1)
                {
                    iText2 = string.Empty + DatabaseAPI.GetEnhancementNameShortWSet(power.Slots[historyMap.SID].Enhancement.Enh);
                    if (DatabaseAPI.Database.Enhancements[power.Slots[historyMap.SID].Enhancement.Enh].TypeID == Enums.eType.InventO ||
                        DatabaseAPI.Database.Enhancements[power.Slots[historyMap.SID].Enhancement.Enh].TypeID == Enums.eType.SetO)
                        iText2 = iText2 + "-" + (power.Slots[historyMap.SID].Enhancement.IOLevel + 1);
                }
                else
                    iText2 = "[Empty]";

                popupData.Sections[index].Add(iText2, PopUp.Colors.Invention, 1f, FontStyle.Regular, 1);
            }
        }

        return popupData;
    }

    public bool SetEnhRelativelevels(Enums.eEnhRelative newVal)
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

        if (MessageBox.Show(
                $@"Really set all placed enhancements to a relative level of {display}?

Note: Normal and Special enhancements cannot go above +3, and Inventions cannot go below +0.", @"Are you sure?", MessageBoxButtons.YesNo) !=
            DialogResult.Yes)
            return false;
        foreach (PowerEntry power in Powers)
        {
            foreach (SlotEntry slot in power.Slots)
            {
                if (slot.Enhancement.Enh <= -1)
                    continue;
                IEnhancement enhancement = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh];
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
            }
        }

        return true;
    }

    void CheckAndFixAllEnhancements()
    {
        foreach (PowerEntry power in Powers)
        {
            foreach (SlotEntry slot in power.Slots)
            {
                if (power.Power != null)
                {
                    if (slot.Enhancement.Enh > -1)
                    {
                        if (!power.Power.IsEnhancementValid(slot.Enhancement.Enh))
                            slot.Enhancement.Enh = -1;
                        else
                            slot.Enhancement.IOLevel = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh]
                                .CheckAndFixIOLevel(slot.Enhancement.IOLevel);
                    }

                    if (slot.FlippedEnhancement.Enh <= -1)
                        continue;
                    if (!power.Power.IsEnhancementValid(slot.FlippedEnhancement.Enh))
                        slot.FlippedEnhancement.Enh = -1;
                    else
                        slot.FlippedEnhancement.IOLevel = DatabaseAPI.Database.Enhancements[slot.FlippedEnhancement.Enh]
                            .CheckAndFixIOLevel(slot.FlippedEnhancement.IOLevel);
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

    void CheckAllVariableBounds()
    {
        for (int index = 0; index <= Powers.Count - 1; ++index)
            Powers[index].CheckVariableBounds();
    }

    internal void Validate()
    {
        ClearInvisibleSlots();
        ScanAndCleanAutomaticallyGrantedPowers();
        AddAutomaticGrantedPowers();
        FillMissingSubPowers();
        CheckAndFixAllEnhancements();
        CheckAllVariableBounds();
    }

    public int GetMaxLevel()
    {
        int maxLevel = 0;
        foreach (PowerEntry power in Powers)
        {
            if (power.Level > maxLevel)
                maxLevel = power.Level;
            maxLevel = power.Slots.Select(slot => slot.Level)
                .Concat(new[]
                {
                    maxLevel
                })
                .Max();
        }

        return maxLevel;
    }

    void ClearInvisibleSlots()
    {
        foreach (PowerEntry power in Powers)
        {
            power.ClearInvisibleSlots();
        }
    }

    void ScanAndCleanAutomaticallyGrantedPowers()
    {
        bool flag = false;
        int maxLevel = GetMaxLevel();
        foreach (PowerEntry power in Powers)
        {
            if (power.Power == null || power.PowerSet == null || power.Chosen ||
                (power.PowerSet.SetType != Enums.ePowerSetType.Inherent && power.PowerSet.SetType != Enums.ePowerSetType.Primary &&
                 power.PowerSet.SetType != Enums.ePowerSetType.Secondary))
                continue;
            if (power.Power.Level > maxLevel + 1 || !MeetsRequirement(power.Power, maxLevel) || !power.Power.IncludeFlag)
            {
                power.Tag = true;
                flag = true;
            }

            if (power.Power.Level > power.Level + 1)
                power.Level = power.Power.Level - 1;
        }

        if (!flag)
            return;
        int powerIdx = 0;
        while (powerIdx < Powers.Count)
        {
            if (Powers[powerIdx].Tag)
            {
                Powers[powerIdx].Reset();
                if (Powers[powerIdx].Chosen)
                    continue;
                Powers[powerIdx].Level = -1;
                Powers[powerIdx].Slots = Array.Empty<SlotEntry>();
            }
            else
                ++powerIdx;
        }
    }

    public bool MeetsRequirement(IPower power, int nLevel, int skipIdx = -1)
    {
        if (nLevel < 0)
            return false;

        int nIdSkip = -1;
        if (skipIdx > -1 & skipIdx < Powers.Count)
            nIdSkip = Powers[skipIdx].Power == null ? -1 : Powers[skipIdx].Power.PowerIndex;
        if (nLevel + 1 < power.Level)
            return false;
        if (power.Requires.NClassName.Length == 0 & power.Requires.NClassNameNot.Length == 0 & power.Requires.NPowerID.Length == 0 &
            power.Requires.NPowerIDNot.Length == 0)
            return true;

        bool valid = power.Requires.NClassName.Length == 0;

        foreach (int clsNameIdx in power.Requires.NClassName)
        {
            if (MidsContext.Character.Archetype.Idx == clsNameIdx)
                valid = true;
        }

        if (power.Requires.NClassNameNot.Any(nclsnamenot => MidsContext.Character.Archetype.Idx == nclsnamenot))
        {
            return false;
        }

        if (!valid)
            return false;

        if (power.Requires.NPowerID.Length > 0)
            valid = false;
        foreach (int[] numArray in power.Requires.NPowerID)
        {
            bool doubleValid = true;
            foreach (int nIDPower in numArray)
            {
                if (nIDPower <= -1)
                    continue;
                int index = -1;
                if (nIDPower != nIdSkip)
                    index = FindInToonHistory(nIDPower);
                if (index < 0)
                    doubleValid = false;
                else if (Powers[index].Level > nLevel)
                    doubleValid = false;
            }

            if (!doubleValid)
                continue;
            valid = true;
            break;
        }

        if (!valid)
            return false;
        foreach (int[] numArray in power.Requires.NPowerIDNot)
        {
            foreach (int nIDPower in numArray)
            {
                if (nIDPower <= -1) continue;
                int histIdx = -1;
                if (nIDPower != nIdSkip)
                    histIdx = FindInToonHistory(nIDPower);
                if (histIdx > -1)
                    return false;
            }
        }

        return true;
    }

    public int FindInToonHistory(int nIDPower)
    {
        for (int powerIdx = 0; powerIdx <= Powers.Count - 1; ++powerIdx)
        {
            if (Powers[powerIdx].Power != null && Powers[powerIdx].Power.PowerIndex == nIDPower)
                return powerIdx;
        }

        return -1;
    }

    public bool PowerUsed(IPower power)
        => FindInToonHistory(power.PowerIndex) > -1;

    void AddAutomaticGrantedPowers()
    {
        int maxLevel = GetMaxLevel();
        List<IPowerset> powersetList = new List<IPowerset>();
        powersetList.AddRange(_character.Powersets);
        foreach (IPowerset powerset in DatabaseAPI.Database.Powersets)
        {
            if (powerset.SetType == Enums.ePowerSetType.Inherent && !powersetList.Contains(powerset))
                powersetList.Add(powerset);
        }

        foreach (IPowerset powerset in powersetList)
        {
            if (powerset == null)
                continue;
            foreach (IPower power in powerset.Powers)
            {
                int val2 = 0;
                if (!power.IncludeFlag || power.Level > maxLevel + 1 || PowerUsed(power) || !MeetsRequirement(power, maxLevel + 1))
                    continue;
                if (power.Requires.NPowerID.Length > 0)
                {
                    int inToonHistory = FindInToonHistory(power.Requires.NPowerID[0][0]);
                    if (inToonHistory > -1)
                        val2 = Powers[inToonHistory].Level;
                }

                AddPower(power, Math.Max(power.Level - 1, val2));
            }
        }
    }

    private string GetEnhSetName(string longName)
    {
        var enhSet = string.Empty;
        var setCount = DatabaseAPI.Database.EnhancementSets.Count;
        for (var setIndex = 0; setIndex < setCount; setIndex++)
        {
            foreach (var enh in DatabaseAPI.Database.EnhancementSets[setIndex].Enhancements)
            {
                IEnhancement enhancement = DatabaseAPI.Database.Enhancements[enh];
                if (enhancement.LongName == longName)
                {
                    enhSet = DatabaseAPI.Database.EnhancementSets[setIndex].DisplayName;
                }
            }
        }

        return enhSet;
    }

    public bool EnhancementTest(int iSlotID, int hIdx, int iEnh, bool silent = false)
    {
        if (iEnh < 0 || iSlotID < 0)
        {
            return false;
        }

        IEnhancement enhancement = DatabaseAPI.Database.Enhancements[iEnh];
        bool foundMutex = false;
        bool foundInPower = false;
        if (enhancement.TypeID == Enums.eType.SetO && enhancement.nIDSet > -1 && hIdx > -1 && Powers[hIdx].Power != null)
        {
            bool allowedSet = false;
            Enums.eSetType setType = DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].SetType;
            for (int index = 0; index <= Powers[hIdx].Power.SetTypes.Length - 1; ++index)
            {
                if (Powers[hIdx].Power.SetTypes[index] != setType)
                    continue;
                allowedSet = true;
                break;
            }

            if (!allowedSet)
                return false;
        }
        for (int powerIdx = 0; powerIdx <= Powers.Count - 1; ++powerIdx)
        {
            setName = GetEnhSetName(enhancement.LongName);
            for (var i = 0; i < Powers[powerIdx].Slots.Length; i++)
            {
                if (Powers[powerIdx].Slots[i].Enhancement.Enh > -1)
                {
                    if (enhancement.Superior)
                    {
                        var containsNonSuperior = MidsContext.Character.powerEnhancements.FirstOrDefault(x => x.PowerName == Powers[powerIdx].Name && x.EnhancementSet == GetEnhSetName(enhancement.LongName).Remove(0, 9));
                        if (containsNonSuperior != null)
                        {
                            MessageBox.Show($"You cannot slot both superior and non-superior versions of {setName.Remove(0,9)} in your build.\r\n\nIf you wish to use this set then you must first remove the other.", @"Unable To Slot Enhancement");
                            return false;
                        }
                    }
                    else if (!enhancement.Superior)
                    {
                        var containsSuperior = MidsContext.Character.powerEnhancements.FirstOrDefault(x => x.PowerName == Powers[powerIdx].Name && x.EnhancementSet == $"Superior {GetEnhSetName(enhancement.LongName)}");
                        if (containsSuperior != null)
                        {
                            MessageBox.Show($"You cannot slot both superior and non-superior versions of {setName} in your build.\r\n\nIf you wish to use this set then you must first remove the other.", @"Unable To Slot Enhancement");
                            return false;
                        }
                    }
                }
            }

            for (int slotIndex = 0; slotIndex <= Powers[powerIdx].Slots.Length - 1; ++slotIndex)
            {
                if ((slotIndex == iSlotID && powerIdx == hIdx) || Powers[powerIdx].Slots[slotIndex].Enhancement.Enh <= -1)
                    continue;
                if (enhancement.Unique && Powers[powerIdx].Slots[slotIndex].Enhancement.Enh == iEnh)
                {
                    if (!silent)
                        MessageBox.Show($@"{enhancement.LongName} is a unique enhancement. You can only slot one of these across your entire build.",
                            @"Unable To Slot Enhancement");
                    return false;
                }

                if (enhancement.MutExID != Enums.eEnhMutex.None &&
                    DatabaseAPI.Database.Enhancements[Powers[powerIdx].Slots[slotIndex].Enhancement.Enh].MutExID == enhancement.MutExID)
                {
                    //6/29/19 Pine: Added checker for ATO Mutex
                    if (enhancement.MutExID == Enums.eEnhMutex.ArchetypeA ||
                        enhancement.MutExID == Enums.eEnhMutex.ArchetypeB ||
                        enhancement.MutExID == Enums.eEnhMutex.ArchetypeC ||
                        enhancement.MutExID == Enums.eEnhMutex.ArchetypeD ||
                        enhancement.MutExID == Enums.eEnhMutex.ArchetypeE ||
                        enhancement.MutExID == Enums.eEnhMutex.ArchetypeF)
                    {
                        /*string compareStringSlottedEnh = DatabaseAPI.Database.Enhancements[Powers[powerIdx].Slots[slotIndex].Enhancement.Enh].LongName;
                        if (compareStringSlottedEnh.Contains("Superior"))
                        {
                            compareStringSlottedEnh = compareStringSlottedEnh.Remove(0, 9);
                            for (var i = 0; i < Powers[powerIdx].Slots.Length; i++)
                            {
                                string currentEnh = DatabaseAPI.Database.Enhancements[Powers[powerIdx].Slots[i].Enhancement.Enh].LongName;
                                if (currentEnh == compareStringSlottedEnh)
                                {
                                    if (!silent)
                                        MessageBox.Show(@"You cannot slot both superior and non-superior versions of this enhancement in your build.", @"Unable To Slot Enhancement");
                                    return false;
                                }
                            }
                        }

                        string compareStringSlottingEnh = enhancement.LongName;
                        if (compareStringSlottingEnh.Contains("Superior"))
                        {
                            compareStringSlottingEnh = compareStringSlottingEnh.Remove(0, 9);
                        }

                        if (compareStringSlottedEnh != compareStringSlottingEnh)
                        {
                            break;
                        }*/
                    }

                    foundMutex = true;
                    break;
                }

                if (enhancement.nIDSet <= -1 || powerIdx != hIdx || Powers[powerIdx].Slots[slotIndex].Enhancement.Enh != iEnh)
                    continue;
                foundInPower = true;
                break;
            }
        }

        if (foundMutex)
        {
            if (!silent)
            {
                MessageBox.Show(
                    enhancement.LongName + " is mutually exclusive with enhancements in the " +
                    Enum.GetName(enhancement.MutExID.GetType(), enhancement.MutExID) +
                    " group. You can only slot one member of this group across your entire build.", "Can't Slot Enhancement");
            }

            return false;
        }

        if (!foundInPower)
            return true;
        if (!silent)
        {
            MessageBox.Show(
                enhancement.LongName +
                " is already slotted in this power. You can only slot one of each enhancement from the set in a given power.",
                "Can't Slot Enhancement");
        }

        return false;
    }

    public void GenerateSetBonusData()
    {
        SetBonus.Clear();
        for (int index1 = 0; index1 < Powers.Count; ++index1)
        {
            I9SetData i9SetData = new I9SetData
            {
                PowerIndex = index1
            };
            if (Powers[index1].Level <= MidsContext.Config.ForceLevel)
            {
                for (int index2 = 0; index2 < Powers[index1].SlotCount; ++index2)
                    i9SetData.Add(ref Powers[index1].Slots[index2].Enhancement);
            }

            i9SetData.BuildEffects(!MidsContext.Config.Inc.DisablePvE ? Enums.ePvX.PvE : Enums.ePvX.PvP);
            if (!i9SetData.Empty)
                SetBonus.Add(i9SetData);
        }

        _setBonusVirtualPower = null;
    }

    IPower GetSetBonusVirtualPower()
    {
        IPower power1 = new Power();
        if (MidsContext.Config.I9.IgnoreSetBonusFX)
            return power1;
        var nidPowers = DatabaseAPI.NidPowers("set_bonus");
        int[] setCount = new int[nidPowers.Length];
        for (int index = 0; index < setCount.Length; ++index)
            setCount[index] = 0;
        var effectList = new List<IEffect>();
        foreach (var setBonus in SetBonus)
        foreach (var setInfo in setBonus.SetInfo)
        foreach (var power in setInfo.Powers.Where(x => x > -1))
        {
            if (power > setCount.Length - 1)
                throw new IndexOutOfRangeException("power to setBonusArray");
            ++setCount[power];
            if (setCount[power] < 6)
            {
                effectList.AddRange(DatabaseAPI.Database.Power[power].Effects.Select(t => (IEffect) t.Clone()));
            }
        }

        power1.Effects = effectList.ToArray();
        return power1;
    }

    public IEffect[] GetCumulativeSetBonuses()
    {
        IPower bonusVirtualPower = SetBonusVirtualPower;
        IEffect[] array = Array.Empty<IEffect>();
        foreach (var effIdx in bonusVirtualPower.Effects)
        {
            if (effIdx.EffectType == Enums.eEffectType.None && string.IsNullOrEmpty(effIdx.Special))
                continue;
            int index2 = GcsbCheck(array, effIdx);
            if (index2 < 0)
            {
                Array.Resize(ref array, array.Length + 1);
                int index3 = array.Length - 1;
                array[index3] = (IEffect) effIdx.Clone();
                array[index3].Math_Mag = effIdx.Mag;
            }
            else
                array[index2].Math_Mag += effIdx.Mag;
        }

        return array;
    }

    static int GcsbCheck(IEffect[] fxList, IEffect testFX)
    {
        for (int index = 0; index < fxList.Length; ++index)
        {
            if (fxList[index].EffectType != testFX.EffectType || (!(fxList[index].Mag > 0.0 & testFX.Mag > 0.0) &&
                                                                  !(fxList[index].Mag < 0.0 & testFX.Mag < 0.0) &&
                                                                  !(Math.Abs(fxList[index].Mag - ((double) testFX.Mag > 0.0 ? 1f : 0.0f)) <
                                                                    0.001)))
                continue;
            if (
                (testFX.EffectType == Enums.eEffectType.Mez || testFX.EffectType == Enums.eEffectType.MezResist) &&
                fxList[index].MezType == testFX.MezType ||
                (testFX.EffectType == Enums.eEffectType.Damage && testFX.DamageType == fxList[index].DamageType ||
                 testFX.EffectType == Enums.eEffectType.Defense && testFX.DamageType == fxList[index].DamageType) ||
                (testFX.EffectType == Enums.eEffectType.Resistance && testFX.DamageType == fxList[index].DamageType ||
                 testFX.EffectType == Enums.eEffectType.DamageBuff && testFX.DamageType == fxList[index].DamageType ||
                 testFX.EffectType == Enums.eEffectType.Enhancement && testFX.ETModifies == fxList[index].ETModifies &&
                 (testFX.DamageType == fxList[index].DamageType && testFX.MezType == fxList[index].MezType)) ||
                testFX.EffectType == Enums.eEffectType.ResEffect && testFX.ETModifies == fxList[index].ETModifies ||
                testFX.EffectType == Enums.eEffectType.None && testFX.Special == fxList[index].Special &&
                testFX.Special.IndexOf("DEBT", StringComparison.OrdinalIgnoreCase) > -1)
                return index;
            if (testFX.EffectType != Enums.eEffectType.Mez & testFX.EffectType != Enums.eEffectType.MezResist &
                testFX.EffectType != Enums.eEffectType.Damage & testFX.EffectType != Enums.eEffectType.Defense &
                testFX.EffectType != Enums.eEffectType.Resistance & testFX.EffectType != Enums.eEffectType.DamageBuff &
                testFX.EffectType != Enums.eEffectType.Enhancement & testFX.EffectType != Enums.eEffectType.ResEffect &
                testFX.EffectType == fxList[index].EffectType)
                return index;
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
            IPower power1 = Powers[hIdx].Power;
            if (power1.MutexIgnore)
            {
                eMutex = Enums.eMutex.NoGroup;
            }
            else
            {
                List<PowerEntry> powerEntryList = new List<PowerEntry>();
                bool mutexAuto = false;
                int index1 = -1;
                for (int index2 = 0; index2 <= DatabaseAPI.Database.MutexList.Length - 1; ++index2)
                {
                    if (!string.Equals(DatabaseAPI.Database.MutexList[index2], "KHELDIAN_GROUP", StringComparison.OrdinalIgnoreCase))
                        continue;
                    index1 = index2;
                    break;
                }

                bool flag2 = power1.HasMutexID(index1);
                foreach (PowerEntry power2 in Powers)
                {
                    if (power2.Power == null || power2.Power.PowerIndex == power1.PowerIndex)
                        continue;
                    IPower power3 = power2.Power;
                    if (!power2.StatInclude || power3.MutexIgnore)
                        continue;
                    if (flag2 || (power3.PowerType != Enums.ePowerType.Click || power3.PowerName == "Light_Form") && power3.HasMutexID(index1))
                    {
                        powerEntryList.Add(power2);
                        if (power3.MutexAuto)
                            mutexAuto = true;
                    }
                    else
                    {
                        foreach (int num1 in power1.NGroupMembership)
                        {
                            foreach (int num2 in power3.NGroupMembership)
                            {
                                if (num1 != num2)
                                    continue;
                                powerEntryList.Add(power2);
                                if (power3.MutexAuto)
                                    mutexAuto = true;
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
                    foreach (PowerEntry powerEntry in powerEntryList)
                        powerEntry.StatInclude = false;
                    eMutex = Enums.eMutex.NoConflict;
                }
                else
                {
                    if (doDetoggle && mutexAuto && Powers[hIdx].StatInclude)
                        Powers[hIdx].StatInclude = false;
                    if (!silent && powerEntryList.Count > 0)
                    {
                        string empty = string.Empty;
                        string str1 = power1.DisplayName +
                                      " is mutually exclusive and can't be used at the same time as the following powers:\n";
                        foreach (PowerEntry powerEntry in powerEntryList)
                        {
                            if (!string.IsNullOrEmpty(empty))
                                empty += ", ";
                            empty += powerEntry.Power.DisplayName;
                        }

                        string str2 = str1 + empty;
                        MessageBox.Show(
                            !doDetoggle || !power1.MutexAuto || !Powers[hIdx].StatInclude
                                ? str2 + "\n\nYou should turn off the powers listed before turning this one on."
                                : str2 + "\n\nThe listed powers have been turned off.", "Power Conflict");
                    }

                    eMutex = powerEntryList.Count > 0
                        ? (!power1.MutexAuto
                            ? (!mutexAuto ? Enums.eMutex.MutexFound : Enums.eMutex.DetoggleSlave)
                            : Enums.eMutex.DetoggleMaster)
                        : Enums.eMutex.NoConflict;
                }
            }
        }

        return eMutex;
    }

    public void FullMutexCheck()
    {
        for (int hIdx = Powers.Count - 1; hIdx >= 0; hIdx += -1)
        {
            MutexV2(hIdx, true, true);
        }
    }
}
