using System;
using System.Drawing;
using System.Linq;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core
{
    public class PowerEntry : ICloneable
    {
        public PowerEntry(IPower? power)
        {
            StatInclude = false;
            Level = -1;
            if (power == null)
            {
                IDXPower = -1;
                NIDPowerset = -1;
                NIDPower = -1;
            }
            else
            {
                IDXPower = power.PowerSetIndex;
                NIDPower = power.PowerIndex;
                NIDPowerset = power.PowerSetID;
            }

            Tag = false;
            Slots = Array.Empty<SlotEntry>();
            SubPowers = Array.Empty<PowerSubEntry>();
            VariableValue = 0;
            ProcInclude = false;
            InherentSlotsUsed = 0;
        }

        public PowerEntry(int iLevel = -1, IPower? power = null, bool chosen = false)
        {
            StatInclude = false;
            Level = iLevel;
            Chosen = chosen;
            InherentSlotsUsed = 0;
            if (power != null)
            {
                NIDPowerset = power.PowerSetID;
                IDXPower = power.PowerSetIndex;
                NIDPower = power.PowerIndex;
                if (power.NIDSubPower.Length > 0)
                {
                    SubPowers = new PowerSubEntry[power.NIDSubPower.Length];
                    for (var index = 0; index <= SubPowers.Length - 1; ++index)
                    {
                        SubPowers[index] = new PowerSubEntry
                        {
                            nIDPower = power.NIDSubPower[index]
                        };
                        SubPowers[index].Powerset = DatabaseAPI.Database.Power[SubPowers[index].nIDPower].PowerSetID;
                        SubPowers[index].Power = DatabaseAPI.Database.Power[SubPowers[index].nIDPower].PowerSetIndex;
                    }
                }
                else
                {
                    SubPowers = Array.Empty<PowerSubEntry>();
                }

                if (power.Slottable & (power.GetPowerSet()?.GroupName != "Incarnate"))
                {
                    Slots = new SlotEntry[1];
                    Slots[0].Enhancement = new I9Slot();
                    Slots[0].FlippedEnhancement = new I9Slot();
                    Slots[0].Level = iLevel;
                    Slots[0].IsInherent = false;
                }
                else
                {
                    Slots = Array.Empty<SlotEntry>();
                }

                if (power.AlwaysToggle | (power.PowerType == Enums.ePowerType.Auto_))
                    StatInclude = true;
            }
            else
            {
                IDXPower = -1;
                NIDPowerset = -1;
                NIDPower = -1;
                Slots = Array.Empty<SlotEntry>();
                SubPowers = Array.Empty<PowerSubEntry>();
            }

            Tag = false;
            if (Power != null && Power.VariableStart > Power.VariableMin && Power.VariableStart <= Power.VariableMax)
            {
                VariableValue = Power.VariableStart;
                Power.Stacks = VariableValue;
            }
            else
            {
                VariableValue = 0;
            }
            ProcInclude = false;
        }

        public int Level { get; set; }
        public int NIDPowerset { get; set; }
        public int IDXPower { get; set; }
        public int NIDPower { get; set; }
        public bool Tag { get; set; }
        public bool StatInclude { get; set; }
        public bool ProcInclude { get; set; }
        public int InherentSlotsUsed { get; set; }

        private int _VirtualVariableValue;
        private int _VariableValue;
        public int VirtualVariableValue
        {
            get => _VirtualVariableValue;
            set => _VirtualVariableValue = value;
        }

        public int VariableValue
        {
            get => _VirtualVariableValue;
            set
            {
                _VirtualVariableValue = value;
                _VariableValue = value;
            }
        }

        public int InternalVariableValue => _VariableValue;
        public SlotEntry[] Slots { get; set; }
        public PowerSubEntry[] SubPowers { get; set; }

        public bool Chosen { get; }

        public Enums.ePowerState State => Power == null
            ? Chosen ? Enums.ePowerState.Empty : Enums.ePowerState.Disabled
            : Enums.ePowerState.Used;

        public IPower? Power => NIDPower >= 0 && NIDPower <= DatabaseAPI.Database.Power.Length - 1
            ? DatabaseAPI.Database.Power[NIDPower]
            : null;

        public IPowerset? PowerSet => Power != null ? DatabaseAPI.Database.Powersets[Power.PowerSetID] : null;

        public bool AllowFrontLoading => Power != null && Power.AllowFrontLoading;

        public string Name => Power != null ? Power.DisplayName : "";

        public bool Virtual => !Chosen && SubPowers.Length > 0;

        public int SlotCount => Slots?.Length ?? 0;

        public object Clone()
        {
            var powerEntry = new PowerEntry(Level, Power, Chosen)
            {
                StatInclude = StatInclude,
                ProcInclude = ProcInclude,
                Tag = Tag,
                VariableValue = VariableValue,
                InherentSlotsUsed = InherentSlotsUsed,
                SubPowers = (PowerSubEntry[]) SubPowers.Clone(),
                Slots = new SlotEntry[Slots.Length]
            };
            for (var index = 0; index < SlotCount; ++index)
            {
                powerEntry.Slots[index].Level = Slots[index].Level;
                powerEntry.Slots[index].IsInherent = Slots[index].IsInherent;
                powerEntry.Slots[index].Enhancement = Slots[index].Enhancement.Clone() as I9Slot;
                powerEntry.Slots[index].FlippedEnhancement = Slots[index].FlippedEnhancement.Clone() as I9Slot;
            }

            return powerEntry;
        }

        public void ClearInvisibleSlots()
        {
            if (SlotCount > 0 && (Power == null && !Chosen || Power is { Slottable: false }))
            {
                Slots = Array.Empty<SlotEntry>();
            }
            else if (SlotCount > 6)
            {
                Slots = Slots.Take(6).ToArray();
            }
        }

        public void Assign(PowerEntry? iPe)
        {
            Level = iPe.Level;
            NIDPowerset = iPe.NIDPowerset;
            IDXPower = iPe.IDXPower;
            NIDPower = iPe.NIDPower;
            Tag = iPe.Tag;
            StatInclude = iPe.StatInclude;
            VariableValue = iPe.VariableValue;
            ProcInclude = iPe.ProcInclude;
            InherentSlotsUsed = iPe.InherentSlotsUsed;
            if (iPe.Slots != null)
            {
                Slots = new SlotEntry[iPe.Slots.Length];
                for (var index = 0; index <= Slots.Length - 1; ++index)
                {
                    Slots[index].Assign(iPe.Slots[index]);
                }
            }
            else
            {
                Slots = Array.Empty<SlotEntry>();
            }

            if (iPe.SubPowers != null)
            {
                SubPowers = new PowerSubEntry[iPe.SubPowers.Length];
                for (var index = 0; index <= SubPowers.Length - 1; ++index)
                {
                    SubPowers[index].Assign(iPe.SubPowers[index]);
                }
            }
            else
            {
                SubPowers = Array.Empty<PowerSubEntry>();
            }
        }

        public bool HasProc()
        {
            for (var index = 0; index < SlotCount; ++index)
            {
                if (Slots[index].Enhancement.Enh < 0) continue;
                var enh = DatabaseAPI.Database.Enhancements[Slots[index].Enhancement.Enh];
                var power = enh.GetPower();
                if (DatabaseAPI.Database.Enhancements[Slots[index].Enhancement.Enh].IsProc && power != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanIncludeForStats()
        {
            if (NIDPowerset <= -1)
            {
                return false;
            }

            if (IDXPower <= -1)
            {
                return false;
            }

            var power = DatabaseAPI.Database.Powersets[NIDPowerset]?.Powers[IDXPower];
            return power?.PowerType switch
            {
                Enums.ePowerType.Auto_ => true,
                Enums.ePowerType.Click when power.ClickBuff => true,
                Enums.ePowerType.Toggle => true,
                _ => false
            };
        }

        public void CheckVariableBounds()
        {
            if (Power is not { VariableEnabled: true })
            {
                VariableValue = 0;
            }
            else if (Power.VariableMin > VariableValue)
            {
                VariableValue = Power.VariableMin;
            }
            else
            {
                if (Power.VariableMax >= VariableValue)
                    return;
                VariableValue = Power.VariableMax;
            }
        }

        public void ValidateSlots()
        {
            for (var index = 0; index <= Slots.Length - 1; ++index)
            {
                if (!Power.IsEnhancementValid(Slots[index].Enhancement.Enh))
                {
                    Slots[index].Enhancement = new I9Slot();
                }

                if (!Power.IsEnhancementValid(Slots[index].FlippedEnhancement.Enh))
                {
                    Slots[index].FlippedEnhancement = new I9Slot();
                }
            }
        }

        public void Reset()
        {
            NIDPowerset = -1;
            IDXPower = -1;
            NIDPower = -1;
            Tag = false;
            StatInclude = false;
            ProcInclude = false;
            InherentSlotsUsed = 0;
            SubPowers = Array.Empty<PowerSubEntry>();
            if (Slots.Length != 1 || Slots[0].Enhancement.Enh != -1)
                return;
            Slots = Array.Empty<SlotEntry>();
        }

        public PopUp.Section? PopSubPowerListing(string sTitle, Color disabledColor, Color enabledColor)
        {
            var section = new PopUp.Section();
            section.Add(sTitle, PopUp.Colors.Title);
            foreach (var subPower in SubPowers)
            {
                if (subPower.nIDPower > -1)
                {
                    section.Add(DatabaseAPI.Database.Power[subPower.nIDPower].DisplayName, subPower.StatInclude ? enabledColor : disabledColor, 0.9f, FontStyle.Bold, 1);
                }
            }

            return section;
        }

        public int AddSlot(int iLevel, bool isInherent = false)
        {
            int slotIdx;
            if ((Slots.Length > 5) | !DatabaseAPI.Database.Power[NIDPower].Slottable)
            {
                slotIdx = -1;
            }
            else
            {
                int index1;
                if (Slots.Length == 0)
                {
                    Slots = new SlotEntry[1];
                    index1 = 0;
                }
                else
                {
                    var num2 = 0;
                    for (var index2 = 1; index2 < Slots.Length; ++index2)
                    {
                        if (Slots[index2].Level <= iLevel)
                        {
                            num2 = index2;
                        }
                    }

                    index1 = num2 + 1;

                    var slotEntryArray = new SlotEntry[Slots.Length + 1];

                    var index3 = -1;

                    for (var index2 = 0; index2 < slotEntryArray.Length; ++index2)
                    {
                        if (index2 == index1)
                        {
                            continue;
                        }
                        ++index3;
                        if (index3 is > 0 and < 2 && isInherent)
                        {
                            Slots[index3].IsInherent = true;
                        }
                        slotEntryArray[index2].Assign(Slots[index3]);
                    }

                    Slots = new SlotEntry[slotEntryArray.Length];
                    for (var index2 = 0; index2 < Slots.Length; ++index2)
                    {
                        if (index2 != index1)
                        {
                            if (index2 is > 0 and < 2 && isInherent)
                            {
                                slotEntryArray[index2].IsInherent = true;
                            }
                            Slots[index2].Assign(slotEntryArray[index2]);
                        }
                    }
                }

                Slots[index1].Enhancement = new I9Slot();
                Slots[index1].FlippedEnhancement = new I9Slot();
                Slots[index1].Level = iLevel;
                if (isInherent)
                {
                    Slots[index1].IsInherent = true;
                    Slots[index1].Level = iLevel - 1;
                }
                slotIdx = index1;
            }
            return slotIdx;
        }

        public int AddSlot2(int lvl)
        {
            if ((Slots.Length > 5) | !DatabaseAPI.Database.Power[NIDPower].Slottable)
            {
                return -1;
            }
            else
            {
                if (Slots.Length == 0)
                {
                    Slots = new SlotEntry[1];
                    Slots[0].Enhancement = new I9Slot();
                    Slots[0].FlippedEnhancement = new I9Slot();
                    Slots[0].Level = lvl;

                    return 0;
                }

                var slotsList = Slots.ToList();
                slotsList.Add(new SlotEntry
                {
                    Enhancement = new I9Slot(),
                    FlippedEnhancement = new I9Slot(),
                    Level = lvl
                });

                Slots = slotsList.ToArray();

                return Slots.Length - 1;
            }
        }

        public bool CanRemoveSlot(int slotIdx, out string message)
        {
            message = string.Empty;
            if (slotIdx < 0 || slotIdx > Slots.Length - 1)
            {
                return false;
            }

            if ((slotIdx == 0) & (NIDPowerset > -1))
            {
                message = "This slot was added automatically and can't be removed without also removing the power.";
                return false;
            }

            if ((slotIdx > 0) & Slots[slotIdx].IsInherent && MidsContext.Config.BuildMode is Enums.dmModes.Normal && DatabaseAPI.ServerData.EnableInherentSlotting)
            {
                message = "This slot is an inherent slot and can only be removed/re-assigned in Respec mode which assumes you have 6 slotted the power in game prior to respec.";
                return false;
            }

            if ((slotIdx > 0) & Slots[slotIdx].IsInherent && MidsContext.Config.BuildMode is Enums.dmModes.LevelUp && DatabaseAPI.ServerData.EnableInherentSlotting)
            {
                switch (Power.FullName)
                {
                    case "Inherent.Fitness.Health":
                        if (Level < DatabaseAPI.ServerData.HealthSlot1Level)
                        {
                            return true;
                        }
                        else if (Level < DatabaseAPI.ServerData.HealthSlot2Level)
                        {
                            return true;
                        }

                        break;
                    case "Inherent.Fitness.Stamina":
                        if (Level < DatabaseAPI.ServerData.StaminaSlot1Level)
                        {
                            return true;
                        }
                        else if (Level < DatabaseAPI.ServerData.StaminaSlot2Level)
                        {
                            return true;
                        }

                        break;
                }
                message = "This slot is an inherent slot and can only be removed/re-assigned in Respec mode which assumes you have 6 slotted the power in game prior to respec.";
                return false;
            }

            if (slotIdx != 0 || Slots.Length <= 1)
            {
                return true;
            }

            message = "This slot was added automatically with a power, and can't be removed until you've removed all other slots from this power.";
            return false;
        }
    }
}