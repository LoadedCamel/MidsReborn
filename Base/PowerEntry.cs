using System;
using System.Drawing;
using System.Linq;
using mrbBase.Base.Display;

namespace mrbBase
{
    public class PowerEntry : ICloneable
    {
        public PowerEntry(IPower power)
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
            Slots = new SlotEntry[0];
            SubPowers = new PowerSubEntry[0];
            VariableValue = 0;
        }

        public PowerEntry(int iLevel = -1, IPower power = null, bool chosen = false)
        {
            StatInclude = false;
            Level = iLevel;
            Chosen = chosen;
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
                    SubPowers = new PowerSubEntry[0];
                }

                if (power.Slottable & (power.GetPowerSet().GroupName != "Incarnate"))
                {
                    Slots = new SlotEntry[1];
                    Slots[0].Enhancement = new I9Slot();
                    Slots[0].FlippedEnhancement = new I9Slot();
                    Slots[0].Level = iLevel;
                }
                else
                {
                    Slots = new SlotEntry[0];
                }

                if (power.AlwaysToggle | (power.PowerType == Enums.ePowerType.Auto_))
                    StatInclude = true;
            }
            else
            {
                IDXPower = -1;
                NIDPowerset = -1;
                NIDPower = -1;
                Slots = new SlotEntry[0];
                SubPowers = new PowerSubEntry[0];
            }

            Tag = false;
            VariableValue = 0;
        }

        // public fields are bad, mkay? O_o
        public int Level { get; set; }
        public int NIDPowerset { get; set; }
        public int IDXPower { get; set; }
        public int NIDPower { get; set; }
        public bool Tag { get; set; }
        public bool StatInclude { get; set; }
        public int VariableValue { get; set; }
        public SlotEntry[] Slots { get; set; }
        public PowerSubEntry[] SubPowers { get; set; }

        public bool Chosen { get; }

        public Enums.ePowerState State => Power == null
            ? Chosen ? Enums.ePowerState.Empty : Enums.ePowerState.Disabled
            : Enums.ePowerState.Used;

        public IPower Power => NIDPower >= 0 && NIDPower <= DatabaseAPI.Database.Power.Length - 1
            ? DatabaseAPI.Database.Power[NIDPower]
            : null;

        public IPowerset PowerSet => Power != null ? DatabaseAPI.Database.Powersets[Power.PowerSetID] : null;

        public bool AllowFrontLoading => Power != null && Power.AllowFrontLoading;

        public string Name => Power != null ? Power.DisplayName : "";

        public bool Virtual => !Chosen && SubPowers.Length > 0;

        public int SlotCount => Slots?.Length ?? 0;

        public object Clone()
        {
            var powerEntry = new PowerEntry(Level, Power, Chosen)
            {
                StatInclude = StatInclude,
                Tag = Tag,
                VariableValue = VariableValue,
                SubPowers = (PowerSubEntry[]) SubPowers.Clone(),
                Slots = new SlotEntry[Slots.Length]
            };
            for (var index = 0; index < SlotCount; ++index)
            {
                powerEntry.Slots[index].Level = Slots[index].Level;
                powerEntry.Slots[index].Enhancement = Slots[index].Enhancement.Clone() as I9Slot;
                powerEntry.Slots[index].FlippedEnhancement = Slots[index].FlippedEnhancement.Clone() as I9Slot;
            }

            return powerEntry;
        }

        public void ClearInvisibleSlots()
        {
            if (SlotCount > 0 && (Power == null && !Chosen || Power != null && !Power.Slottable))
                Slots = Array.Empty<SlotEntry>();
            else if (SlotCount > 6)
                Slots = Slots.Take(6).ToArray();
        }

        public void Assign(PowerEntry iPe)
        {
            Level = iPe.Level;
            NIDPowerset = iPe.NIDPowerset;
            IDXPower = iPe.IDXPower;
            NIDPower = iPe.NIDPower;
            Tag = iPe.Tag;
            StatInclude = iPe.StatInclude;
            VariableValue = iPe.VariableValue;
            if (iPe.Slots != null)
            {
                Slots = new SlotEntry[iPe.Slots.Length];
                for (var index = 0; index <= Slots.Length - 1; ++index)
                    Slots[index].Assign(iPe.Slots[index]);
            }
            else
            {
                Slots = new SlotEntry[0];
            }

            if (iPe.SubPowers != null)
            {
                SubPowers = new PowerSubEntry[iPe.SubPowers.Length];
                for (var index = 0; index <= SubPowers.Length - 1; ++index)
                    SubPowers[index].Assign(iPe.SubPowers[index]);
            }
            else
            {
                SubPowers = new PowerSubEntry[0];
            }
        }

        public bool HasProc()
        {
            for (var index1 = 0; index1 <= SlotCount - 1; ++index1)
            {
                if (Slots[index1].Enhancement.Enh < 0) continue;
                var enh = DatabaseAPI.Database.Enhancements[Slots[index1].Enhancement.Enh];
                var power = enh.GetPower();
                if (DatabaseAPI.Database.Enhancements[Slots[index1].Enhancement.Enh].Effect.Length <= 0 || power == null)
                    continue;
                foreach (var index2 in power.Effects)
                {
                    var num = 0;
                    switch (index2.EffectType)
                    {
                        case Enums.eEffectType.None:
                        case Enums.eEffectType.Damage:
                        case Enums.eEffectType.DamageBuff:
                        case Enums.eEffectType.Enhancement:
                        case Enums.eEffectType.Heal:
                            num = 1;
                            break;
                        case Enums.eEffectType.Mez:
                            if (index2.Mag > 0.0)
                                goto case Enums.eEffectType.None;
                            else
                                goto default;
                        case Enums.eEffectType.Accuracy:
                            break;
                        case Enums.eEffectType.ViewAttrib:
                            break;
                        case Enums.eEffectType.Defense:
                            break;
                        case Enums.eEffectType.DropToggles:
                            break;
                        case Enums.eEffectType.Endurance:
                            break;
                        case Enums.eEffectType.EnduranceDiscount:
                            break;
                        case Enums.eEffectType.Fly:
                            break;
                        case Enums.eEffectType.SpeedFlying:
                            break;
                        case Enums.eEffectType.GrantPower:
                            break;
                        case Enums.eEffectType.HitPoints:
                            break;
                        case Enums.eEffectType.InterruptTime:
                            break;
                        case Enums.eEffectType.JumpHeight:
                            break;
                        case Enums.eEffectType.SpeedJumping:
                            break;
                        case Enums.eEffectType.Meter:
                            break;
                        case Enums.eEffectType.MezResist:
                            break;
                        case Enums.eEffectType.MovementControl:
                            break;
                        case Enums.eEffectType.MovementFriction:
                            break;
                        case Enums.eEffectType.PerceptionRadius:
                            break;
                        case Enums.eEffectType.Range:
                            break;
                        case Enums.eEffectType.RechargeTime:
                            break;
                        case Enums.eEffectType.Recovery:
                            break;
                        case Enums.eEffectType.Regeneration:
                            break;
                        case Enums.eEffectType.ResEffect:
                            break;
                        case Enums.eEffectType.Resistance:
                            break;
                        case Enums.eEffectType.RevokePower:
                            break;
                        case Enums.eEffectType.Reward:
                            break;
                        case Enums.eEffectType.SpeedRunning:
                            break;
                        case Enums.eEffectType.SetCostume:
                            break;
                        case Enums.eEffectType.SetMode:
                            break;
                        case Enums.eEffectType.Slow:
                            break;
                        case Enums.eEffectType.StealthRadius:
                            break;
                        case Enums.eEffectType.StealthRadiusPlayer:
                            break;
                        case Enums.eEffectType.EntCreate:
                            break;
                        case Enums.eEffectType.ThreatLevel:
                            break;
                        case Enums.eEffectType.ToHit:
                            break;
                        case Enums.eEffectType.Translucency:
                            break;
                        case Enums.eEffectType.XPDebtProtection:
                            break;
                        case Enums.eEffectType.SilentKill:
                            break;
                        case Enums.eEffectType.Elusivity:
                            break;
                        case Enums.eEffectType.GlobalChanceMod:
                            break;
                        case Enums.eEffectType.CombatModShift:
                            break;
                        case Enums.eEffectType.UnsetMode:
                            break;
                        case Enums.eEffectType.Rage:
                            break;
                        case Enums.eEffectType.MaxRunSpeed:
                            break;
                        case Enums.eEffectType.MaxJumpSpeed:
                            break;
                        case Enums.eEffectType.MaxFlySpeed:
                            break;
                        case Enums.eEffectType.DesignerStatus:
                            break;
                        case Enums.eEffectType.PowerRedirect:
                            break;
                        case Enums.eEffectType.TokenAdd:
                            break;
                        case Enums.eEffectType.ExperienceGain:
                            break;
                        case Enums.eEffectType.InfluenceGain:
                            break;
                        case Enums.eEffectType.PrestigeGain:
                            break;
                        case Enums.eEffectType.AddBehavior:
                            break;
                        case Enums.eEffectType.RechargePower:
                            break;
                        case Enums.eEffectType.RewardSourceTeam:
                            break;
                        case Enums.eEffectType.VisionPhase:
                            break;
                        case Enums.eEffectType.CombatPhase:
                            break;
                        case Enums.eEffectType.ClearFog:
                            break;
                        case Enums.eEffectType.SetSZEValue:
                            break;
                        case Enums.eEffectType.ExclusiveVisionPhase:
                            break;
                        case Enums.eEffectType.Absorb:
                            break;
                        case Enums.eEffectType.XAfraid:
                            break;
                        case Enums.eEffectType.XAvoid:
                            break;
                        case Enums.eEffectType.BeastRun:
                            break;
                        case Enums.eEffectType.ClearDamagers:
                            break;
                        case Enums.eEffectType.EntCreate_x:
                            break;
                        case Enums.eEffectType.Glide:
                            break;
                        case Enums.eEffectType.Hoverboard:
                            break;
                        case Enums.eEffectType.Jumppack:
                            break;
                        case Enums.eEffectType.MagicCarpet:
                            break;
                        case Enums.eEffectType.NinjaRun:
                            break;
                        case Enums.eEffectType.Null:
                            break;
                        case Enums.eEffectType.NullBool:
                            break;
                        case Enums.eEffectType.Stealth:
                            break;
                        case Enums.eEffectType.SteamJump:
                            break;
                        case Enums.eEffectType.Walk:
                            break;
                        case Enums.eEffectType.XPDebt:
                            break;
                        case Enums.eEffectType.ForceMove:
                            break;
                        case Enums.eEffectType.ModifyAttrib:
                            break;
                        default:
                            num = index2.ToWho == Enums.eToWho.Target ? 1 : 0;
                            break;
                    }

                    if (num == 0)
                        return true;
                }
            }

            return false;
        }

        public bool CanIncludeForStats()
        {
            return (NIDPowerset > -1) & (IDXPower > -1) && (HasProc() ||
                                                            DatabaseAPI.Database.Powersets[NIDPowerset].Powers[IDXPower]
                                                                .PowerType == Enums.ePowerType.Toggle ||
                                                            DatabaseAPI.Database.Powersets[NIDPowerset].Powers[IDXPower]
                                                                .PowerType == Enums.ePowerType.Click &&
                                                            DatabaseAPI.Database.Powersets[NIDPowerset].Powers[IDXPower]
                                                                .ClickBuff ||
                                                            DatabaseAPI.Database.Powersets[NIDPowerset].Powers[IDXPower]
                                                                .PowerType == Enums.ePowerType.Auto_);
        }

        public void CheckVariableBounds()
        {
            if (Power == null || !Power.VariableEnabled)
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
                    Slots[index].Enhancement = new I9Slot();
                if (!Power.IsEnhancementValid(Slots[index].FlippedEnhancement.Enh))
                    Slots[index].FlippedEnhancement = new I9Slot();
            }
        }

        public void Reset()
        {
            NIDPowerset = -1;
            IDXPower = -1;
            NIDPower = -1;
            Tag = false;
            StatInclude = false;
            SubPowers = new PowerSubEntry[0];
            if (Slots.Length != 1 || Slots[0].Enhancement.Enh != -1)
                return;
            Slots = new SlotEntry[0];
        }

        public PopUp.Section PopSubPowerListing(
            string sTitle,
            Color disabledColor,
            Color enabledColor)
        {
            var section = new PopUp.Section();
            section.Add(sTitle, PopUp.Colors.Title);
            foreach (var subPower in SubPowers)
                if (subPower.nIDPower > -1)
                    section.Add(DatabaseAPI.Database.Power[subPower.nIDPower].DisplayName,
                        subPower.StatInclude ? enabledColor : disabledColor, 0.9f, FontStyle.Bold, 1);
            return section;
        }

        public int AddSlot(int iLevel)
        {
            int num1;
            if ((Slots.Length > 5) | !DatabaseAPI.Database.Power[NIDPower].Slottable)
            {
                num1 = -1;
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
                        if (Slots[index2].Level <= iLevel)
                            num2 = index2;
                    index1 = num2 + 1;
                    var slotEntryArray = new SlotEntry[Slots.Length + 1];
                    var index3 = -1;
                    for (var index2 = 0; index2 < slotEntryArray.Length; ++index2)
                    {
                        if (index2 == index1)
                            continue;
                        ++index3;
                        slotEntryArray[index2].Assign(Slots[index3]);
                    }

                    Slots = new SlotEntry[slotEntryArray.Length];
                    for (var index2 = 0; index2 < Slots.Length; ++index2)
                        if (index2 != index1)
                            Slots[index2].Assign(slotEntryArray[index2]);
                }

                Slots[index1].Enhancement = new I9Slot();
                Slots[index1].FlippedEnhancement = new I9Slot();
                Slots[index1].Level = iLevel;
                num1 = index1;
            }

            return num1;
        }

        public bool CanRemoveSlot(int slotIdx, out string message)
        {
            message = string.Empty;
            if (slotIdx < 0 || slotIdx > Slots.Length - 1) return false;

            if ((slotIdx == 0) & (NIDPowerset > -1))
            {
                message = "This slot was added automatically and can't be removed without also removing the power.";
                return false;
            }

            if (slotIdx != 0 || Slots.Length <= 1)
                return true;
            message =
                "This slot was added automatically with a power, and can't be removed until you've removed all other slots from this power.";
            return false;
        }
    }
}