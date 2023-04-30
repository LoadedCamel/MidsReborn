using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using FastDeepCloner;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core.Base.Data_Classes
{
    public class Character
    {
        public static List<string> gridEntries = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h" };
        private Archetype? _archetype;
        private bool? _completeCache;
        public Dictionary<string, int> SetSlotLoc = new Dictionary<string, int>();
        public HashSet<string> setSlotted = new HashSet<string>();
        public event EventHandler<Enums.Alignment>? AlignmentChanged;

        internal Character()
        {
            Name = string.Empty;
            Comment = string.Empty;
            Powersets = new IPowerset?[8];
            PoolLocked = new bool[5];
            Totals = new TotalStatistics();
            TotalsCapped = new TotalStatistics();
            DisplayStats = new Statistics(this);
            Build = new Build(this, DatabaseAPI.Database.Levels);
            PEnhancementsList = new List<string>();
            Reset();
        }

        public List<string> PEnhancementsList { get; set; }

        public string? setName { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }

        public int Level
        {
            get
            {
                if (LevelCache > -1) return LevelCache;

                int num2;
                if (MidsContext.Config.BuildMode is Enums.dmModes.Normal or Enums.dmModes.Respec)
                {
                    num2 = CurrentBuild.GetMaxLevel();
                }
                else
                {
                    var val1 = GetFirstAvailablePowerLevel(CurrentBuild);
                    if (val1 < 0)
                        val1 = 49;
                    var val2 = GetFirstAvailableSlotLevel();
                    if (val2 < 0)
                        val2 = 49;
                    num2 = Math.Min(val1, val2);
                }

                if (num2 < 0)
                    num2 = 49;
                LevelCache = num2;
                return num2;
            }
        }

        public static int MaxLevel => 49;

        public int RequestedLevel { get; set; }

        private Build Build { get; set; }

        public Build CurrentBuild => Build;

        public Archetype? Archetype
        {
            get => _archetype;
            set
            {
                _archetype = value;
                Alignment = _archetype is { Hero: true } ? Enums.Alignment.Hero : Enums.Alignment.Villain;
            }
        }

        private Enums.Alignment _alignment;
        public Enums.Alignment Alignment
        {
            get => _alignment;
            set
            {
                _alignment = value;
                AlignmentChanged?.Invoke(this, value);
            }
        }

        public int Origin { get; set; }

        public IPowerset?[] Powersets { get; set; }

        public bool[] PoolLocked { get; private set; }

        private int LevelCache { get; set; }

        public bool Locked { get; set; }

        public bool Complete
        {
            get
            {
                if (_completeCache.HasValue) return _completeCache.GetValueOrDefault();
                var num1 = Build.TotalSlotsAvailable - CurrentBuild.SlotsPlaced;
                var num2 = CurrentBuild.LastPower + 1 - CurrentBuild.PowersPlaced;
                _completeCache = num1 < 1 && num2 < 1;
                return _completeCache.GetValueOrDefault();
            }
            set => _completeCache = value ? _completeCache : new bool?();
        }

        public int ActiveComboLevel { get; private set; }

        public int PerfectionOfBodyLevel => IsStalker || PerfectionType == "body" ? ActiveComboLevel : 0;

        public int PerfectionOfMindLevel => !IsStalker && PerfectionType == "mind" ? ActiveComboLevel : 0;

        public int PerfectionOfSoulLevel => !IsStalker && PerfectionType == "soul" ? ActiveComboLevel : 0;

        private string? PerfectionType { get; set; }

        public bool AcceleratedActive { get; private set; }

        public bool DelayedActive { get; private set; }

        public bool DisintegrateActive { get; private set; }

        public bool TargetDroneActive { get; private set; }

        public bool Assassination { get; private set; }

        public bool Domination { get; private set; }

        public bool Containment { get; private set; }

        public bool Scourge { get; private set; }

        public bool CriticalHits { get; private set; }

        public bool FastModeActive { get; private set; }

        public bool Defiance { get; private set; }

        public bool DefensiveAdaptation { get; private set; }

        public bool EfficientAdaptation { get; private set; }

        public bool OffensiveAdaptation { get; private set; }

        public bool NotDefensiveAdaptation { get; private set; }

        public bool NotDefensiveNorOffensiveAdaptation { get; private set; }

        //Fighting Pool Synergy

        public bool BoxingBuff { get; private set; }
        public bool NotBoxingBuff { get; private set; }
        public bool KickBuff { get; private set; }
        public bool NotKickBuff { get; private set; }
        public bool CrossPunchBuff { get; private set; }
        public bool NotCrossPunchBuff { get; private set; }

        //Mastermind 
        public bool Supremacy { get; private set; }
        public bool SupremacyAndBuffPwr { get; private set; }
        public bool PetTier2 { get; private set; }
        public bool PetTier3 { get; private set; }
        public bool PackMentality { get; private set; }
        public bool NotPackMentality { get; private set; }

        //Sniper Attacks
        public bool FastSnipe { get; private set; }
        public bool NotFastSnipe { get; private set; }

        public Dictionary<string, float>? ModifyEffects { get; protected set; }

        public TotalStatistics Totals { get; }

        public TotalStatistics TotalsCapped { get; }

        public Statistics DisplayStats { get; }

        public int displayIndex { get; set; }
        public List<InherentDisplayItem>? InherentDisplayList { get; set; }
        public int SlotsRemaining
        {
            get
            {
                var num = Build.TotalSlotsAvailable - CurrentBuild.SlotsPlaced;
                return num;
            }
        }

        public static void ParseCase()
        {
            Console.WriteLine(nameof(BoxingBuff));
        }

        public bool CanPlaceSlot
        {
            get
            {
                if (MidsContext.Config.BuildMode is Enums.dmModes.Normal or Enums.dmModes.Respec)
                {
                    if (Build.TotalSlotsAvailable - CurrentBuild.SlotsPlaced > 0 && MidsContext.Config.BuildOption != Enums.dmItem.Power)
                    {
                        return true;
                    }
                }
                else if ((Level > -1) & (Level < DatabaseAPI.Database.Levels.Length) && DatabaseAPI.Database.Levels[Level].LevelType() == Enums.dmItem.Slot && SlotsRemaining > 0)
                {
                    return true;
                }
                else if ((Level > -1) & (Level < DatabaseAPI.Database.Levels.Length) && DatabaseAPI.Database.Levels[Level].LevelType() == Enums.dmItem.Power && SlotsRemaining > 0 && DatabaseAPI.ServerData.EnableInherentSlotting)
                {
                    if (Level == DatabaseAPI.ServerData.HealthSlot1Level)
                    {
                        return true;
                    }
                    if (Level == DatabaseAPI.ServerData.HealthSlot2Level)
                    {
                        return true;
                    }
                    if (Level == DatabaseAPI.ServerData.StaminaSlot1Level)
                    {
                        return true;
                    }
                    if (Level == DatabaseAPI.ServerData.StaminaSlot2Level)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool IsVillain => Alignment == Enums.Alignment.Rogue || Alignment == Enums.Alignment.Villain;

        public bool IsPraetorian => Alignment == Enums.Alignment.Loyalist || Alignment == Enums.Alignment.Resistance;
        public bool IsBlaster => Archetype.DisplayName.ToLower() == "blaster";

        public bool IsController => Archetype.DisplayName.ToLower() == "controller";

        public bool IsDefender => Archetype.DisplayName.ToLower() == "defender";

        public bool IsScrapper => Archetype.DisplayName.ToLower() == "scrapper";

        public bool IsTanker => Archetype.DisplayName.ToLower() == "tank";

        public bool IsKheldian => Archetype.ClassType == Enums.eClassType.HeroEpic;

        public bool IsBrute => Archetype.DisplayName.ToLower() == "brute";

        public bool IsCorruptor => Archetype.DisplayName.ToLower() == "corruptor";

        public bool IsDominator => Archetype.DisplayName.ToLower() == "dominator";

        public bool IsMastermind => Archetype.DisplayName.ToLower() == "mastermind";

        public bool IsStalker => Archetype.DisplayName.ToLower() == "stalker";

        public bool IsArachnos => Archetype.ClassType == Enums.eClassType.VillainEpic;

        public void ResetLevel()
        {
            LevelCache = -1;
        }

        public void SetLevelTo(int Level)
        {
            LevelCache = Level;
        }

        public void Lock()
        {
            var powersPlaced = CurrentBuild.PowersPlaced;
            var ps1 = Powersets[1] == null || Powersets[1].nID < 0
                ? DatabaseAPI.Database.Powersets
                    .First(ps =>
                        ps.ATClass == MidsContext.Character.Archetype.ClassName &
                        ps.SetType == Enums.ePowerSetType.Secondary)
                : Powersets[1];
            if ((powersPlaced == 1) & CurrentBuild.PowerUsed(ps1.Powers[0]))
            {
                Locked = false;
                ResetLevel();
            }
            else if (powersPlaced > 0)
            {
                Locked = true;
            }
            else
            {
                if (powersPlaced != 0)
                    return;
                Locked = false;
                ResetLevel();
            }
        }

        public int GetPowersByLevel(int Level)
        {
            int[] powerPickedLevels =
                {0, 1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 29, 31, 34, 37, 40, 43, 46, 48};

            return powerPickedLevels.Where(e => e <= Level).ToArray().Length;
        }

        public bool IsHero()
        {
            return Alignment is Enums.Alignment.Hero or Enums.Alignment.Vigilante;
        }

        public bool PoolTaken(int poolID)
        {
            return Powersets[poolID] != null && poolID >= 3 && poolID <= 7 && PoolLocked[poolID - 3];
        }

        // There are 2 versions of this method distributed.
        // Combining the logic bit by bit to see if there are substantial differences
        // returns the last thing it tried to read from, for inclusion in the error message
        public void LoadPowersetsByName2(IList<string> names, ref string blameName)
        {
            Powersets = new IPowerset?[8];
            var m = 0;
            var k = 3;
            foreach (var e in names)
            {
                if (string.IsNullOrWhiteSpace(e)) continue;
                if (e.IndexOf("Epic.", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Powersets[7] = DatabaseAPI.GetPowersetByName(e);
                    if (Powersets[7] == null) blameName = e;
                }
                else if (e.IndexOf("Pool.", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Powersets[k] = DatabaseAPI.GetPowersetByName(e);
                    if (Powersets[k] == null) blameName = e;
                    k++;
                }
                else
                {
                    Powersets[m] = DatabaseAPI.GetPowersetByName(e);
                    if (Powersets[m] == null) blameName = e;
                    m++;
                }
            }
        }

        public IEnumerable<(int, string)> LoadPowersetsByName(IList<string> names)
        {
            Powersets = names.Select(n =>
            {
                if (string.IsNullOrEmpty(n))
                {
                    return null;
                }
                else
                {
                    return DatabaseAPI.GetPowersetByName(n);
                }
            }).ToArray();
            return Powersets.Select((ps, i) => new { I = i, Ps = ps?.FullName, N = names[i] }).Where(x => !string.IsNullOrWhiteSpace(x.N) && x.Ps == null).Select(x => (x.I, x.N));
        }

        public void LoadPowerSetsByName(IEnumerable<string> sets)
        {
            Powersets = sets.Select(set => DatabaseAPI.Database.Powersets.FirstOrDefault(ps => ps?.FullName == set)).Select(powerSet => powerSet ?? new Powerset()).ToArray();
        }

        public void Reset(Archetype? iArchetype = null, int iOrigin = 0)
        {
            Name = string.Empty;
            var flag1 = Archetype != null && iArchetype != null && Archetype.Idx == iArchetype.Idx;
            Archetype = iArchetype ?? DatabaseAPI.Database.Classes[0];
            MidsContext.Archetype = Archetype;
            Origin = iOrigin > Archetype.Origin.Length - 1 ? Archetype.Origin.Length - 1 : iOrigin;
            if (flag1)
            {
                var flag2 = Powersets[0] != null && Powersets[0].nArchetype == Archetype.Idx;
                if (!flag2)
                    Powersets[0] = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Primary)[0];
                var flag3 = Powersets[1] != null && Powersets[1].nArchetype == Archetype.Idx;
                if (!flag3)
                    Powersets[1] = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Secondary)[0];
            }
            else
            {
                Powersets[0] = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Primary)[0];
                Powersets[1] = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Secondary)[0];
            }

            var powersetIndexes1 = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Pool);
            var index = 0;
            Powersets[3] = powersetIndexes1[index];
            if (powersetIndexes1.Length - 1 > index)
                ++index;
            Powersets[4] = powersetIndexes1[index];
            if (powersetIndexes1.Length - 1 > index)
                ++index;
            Powersets[5] = powersetIndexes1[index];
            if (powersetIndexes1.Length - 1 > index)
                ++index;
            Powersets[6] = powersetIndexes1[index];
            var powersetIndexes2 = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Ancillary);
            Powersets[7] = powersetIndexes2.Length <= 0 ? null : powersetIndexes2[0];
            ModifyEffects = new Dictionary<string, float>();
            PoolLocked = new bool[5];
            NewBuild();
            Locked = false;
            LevelCache = -1;
        }

        protected void NewBuild()
        {
            Build = new Build(this, DatabaseAPI.Database.Levels);
            AcceleratedActive = false;
            ActiveComboLevel = 0;
            DelayedActive = false;
            DisintegrateActive = false;
            TargetDroneActive = false;
            FastModeActive = false;
            Assassination = false;
            CriticalHits = false;
            Containment = false;
            Domination = false;
            Scourge = false;
            DefensiveAdaptation = false;
            EfficientAdaptation = false;
            OffensiveAdaptation = false;
            NotDefensiveAdaptation = true;
            NotDefensiveNorOffensiveAdaptation = true;
            PerfectionType = string.Empty;
            BoxingBuff = false;
            NotBoxingBuff = true;
            KickBuff = false;
            NotKickBuff = true;
            CrossPunchBuff = false;
            NotCrossPunchBuff = true;
            Supremacy = false;
            SupremacyAndBuffPwr = false;
            PetTier2 = false;
            PetTier3 = false;
            PackMentality = false;
            NotPackMentality = true;
            FastSnipe = false;
            NotFastSnipe = true;
            Totals.Init();
            TotalsCapped.Init();
            RequestedLevel = -1;
            PEnhancementsList = new List<string>();
        }

        public async void ClearInvalidInherentSlots()
        {
            ResetLevel();
            foreach (var power in CurrentBuild.Powers.Where(power => power?.Power != null))
            {
                switch (power.Power.FullName)
                {
                    case "Inherent.Fitness.Health":
                    {
                        if (Level < DatabaseAPI.ServerData.HealthSlot1Level)
                        {
                            if (power.InherentSlotsUsed > 0)
                            {
                                for (var i = 0; i < power.Slots.Length; i++)
                                {
                                    var slot = power.Slots[i];
                                    if (!slot.IsInherent) continue;
                                    await CurrentBuild.RemoveSlotFromPowerEntry(power.IDXPower, i);
                                    power.InherentSlotsUsed -= 1;
                                    break;
                                }
                            }
                        }

                        if (MidsContext.Character.Level < DatabaseAPI.ServerData.HealthSlot2Level)
                        {
                            if (power.InherentSlotsUsed > 0)
                            {
                                for (var i = 0; i < power.Slots.Length; i++)
                                {
                                    var slot = power.Slots[i];
                                    if (!slot.IsInherent) continue;
                                    await CurrentBuild.RemoveSlotFromPowerEntry(power.IDXPower, i);
                                    power.InherentSlotsUsed -= 1;
                                    break;
                                }
                            }
                        }

                        break;
                    }
                    case "Inherent.Fitness.Stamina":
                    {
                        if (MidsContext.Character.Level < DatabaseAPI.ServerData.StaminaSlot1Level)
                        {
                            if (power.InherentSlotsUsed > 0)
                            {
                                for (var i = 0; i < power.Slots.Length; i++)
                                {
                                    var slot = power.Slots[i];
                                    if (!slot.IsInherent) continue;
                                    await CurrentBuild.RemoveSlotFromPowerEntry(power.IDXPower, i);
                                    power.InherentSlotsUsed -= 1;
                                    break;
                                }
                            }
                        }

                        if (MidsContext.Character.Level < DatabaseAPI.ServerData.StaminaSlot2Level)
                        {
                            if (power.InherentSlotsUsed > 0)
                            {
                                for (var i = 0; i < power.Slots.Length; i++)
                                {
                                    var slot = power.Slots[i];
                                    if (!slot.IsInherent) continue;
                                    await CurrentBuild.RemoveSlotFromPowerEntry(power.IDXPower, i);
                                    power.InherentSlotsUsed -= 1;
                                    break;
                                }
                            }
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Call this function when a power is enabled/disabled, added, or removed, including when the archetype is changed.
        /// </summary>
        private void RefreshActiveSpecial()
        {
            ActiveComboLevel = 0;
            AcceleratedActive = false;
            DelayedActive = false;
            DisintegrateActive = false;
            TargetDroneActive = false;
            FastModeActive = false;
            Assassination = false;
            Domination = false;
            Containment = false;
            Scourge = false;
            CriticalHits = false;
            Defiance = false;
            DefensiveAdaptation = false;
            EfficientAdaptation = false;
            OffensiveAdaptation = false;
            NotDefensiveAdaptation = true;
            NotDefensiveNorOffensiveAdaptation = true;
            PerfectionType = string.Empty;
            BoxingBuff = false;
            NotBoxingBuff = true;
            KickBuff = false;
            NotKickBuff = true;
            CrossPunchBuff = false;
            NotCrossPunchBuff = true;
            Supremacy = false;
            SupremacyAndBuffPwr = false;
            PetTier2 = false;
            PetTier3 = false;
            PackMentality = false;
            NotPackMentality = true;
            FastSnipe = false;
            NotFastSnipe = true;
            InherentDisplayList = new List<InherentDisplayItem>();
            PEnhancementsList = new List<string>();

            //CheckInherentSlots();
            if (CurrentBuild?.Powers == null) return;
            foreach (var power in CurrentBuild.Powers)
            {
                if (power?.Power == null) continue;
                var powName = power.Power.PowerName;
                if (power.HasProc())
                {
                    power.Power.HasProcSlotted = true;
                }
                else if (!power.HasProc())
                {
                    power.Power.HasProcSlotted = false;
                }

                switch (powName)
                {
                    default:
                        if (power.Chosen || !power.Chosen && CurrentBuild.PowerUsed(power.Power))
                        {
                            power.Power.Taken = true;
                        }

                        power.Power.Active = power.StatInclude switch
                        {
                            false => false,
                            true => true
                        };

                        break;
                }

                for (var slotIndex = 0; slotIndex < power.SlotCount; slotIndex++)
                {
                    var pSlotEnh = power.Slots[slotIndex].Enhancement.Enh;
                    if (pSlotEnh == -1) continue;
                    var enhancement = DatabaseAPI.Database.Enhancements[pSlotEnh];
                    if (!PEnhancementsList.Contains(enhancement.UID))
                    {
                        PEnhancementsList.Add(enhancement.UID);
                    }
                }

                if (!power.Power.HasAttribModEffects()) continue;
                foreach (var effect in power.Power.Effects)
                {
                    effect.UpdateAttrib();
                }
            }

            foreach (var power in CurrentBuild.Powers)
            {
                if (power?.Power == null || !power.StatInclude) continue;
                switch (power.Power.PowerName.ToUpper())
                {
                    case "TIME_CRAWL":
                        DelayedActive = true;
                        break;
                    case "TARGETING_DRONE":
                        TargetDroneActive = true;
                        break;
                    case "TEMPORAL_SELECTION":
                        AcceleratedActive = true;
                        break;
                    case "DISINTEGRATE":
                        DisintegrateActive = true;
                        break;
                    case "COMBO_LEVEL_1":
                        ActiveComboLevel = 1;
                        break;
                    case "COMBO_LEVEL_2":
                        ActiveComboLevel = 2;
                        break;
                    case "COMBO_LEVEL_3":
                        ActiveComboLevel = 3;
                        break;
                    case "FAST_MODE":
                        FastModeActive = true;
                        break;
                    case "DEFIANCE":
                        Defiance = true;
                        break;
                    case "ASSASSINATION":
                        Assassination = true;
                        break;
                    case "DOMINATION":
                        Domination = true;
                        break;
                    case "CRITICAL_HIT":
                        CriticalHits = true;
                        break;
                    case "CONTAINMENT":
                        Containment = true;
                        break;
                    case "SCOURGE":
                        Scourge = true;
                        break;
                    case "FORM_OF_THE_BODY":
                        PerfectionType = "body";
                        break;
                    case "FORM_OF_THE_MIND":
                        PerfectionType = "mind";
                        break;
                    case "FORM_OF_THE_SOUL":
                        PerfectionType = "soul";
                        break;
                    case "DEFENSIVE_ADAPTATION":
                        DefensiveAdaptation = true;
                        NotDefensiveAdaptation = false;
                        NotDefensiveNorOffensiveAdaptation = false;
                        break;
                    case "EFFICIENT_ADAPTATION":
                        EfficientAdaptation = true;
                        break;
                    case "OFFENSIVE_ADAPTATION":
                        OffensiveAdaptation = true;
                        NotDefensiveNorOffensiveAdaptation = false;
                        break;
                    case "SUPREMACY":
                        Supremacy = true;
                        break;
                    case "TRAIN_BEASTS":
                        PetTier2 = true;
                        break;
                    case "TAME_BEASTS":
                        PetTier3 = true;
                        break;
                    case "ENCHANT_DEMON":
                        PetTier2 = true;
                        break;
                    case "ABYSSAL_EMPOWERMENT":
                        PetTier3 = true;
                        break;
                    case "EQUIP_MERCENARY":
                        PetTier2 = true;
                        break;
                    case "TACTICAL_UPGRADE":
                        PetTier3 = true;
                        break;
                    case "ENCHANT_UNDEAD":
                        PetTier2 = true;
                        break;
                    case "DARK_EMPOWERMENT":
                        PetTier3 = true;
                        break;
                    case "TRAIN_NINJAS":
                        PetTier2 = true;
                        break;
                    case "KUJI_IN_ZEN":
                        PetTier3 = true;
                        break;
                    case "EQUIP_ROBOT":
                        PetTier2 = true;
                        break;
                    case "UPGRADE_ROBOT":
                        PetTier3 = true;
                        break;
                    case "EQUIP_THUGS":
                        PetTier2 = true;
                        break;
                    case "UPGRADE_EQUIPMENT":
                        PetTier3 = true;
                        break;
                    case "PACK_MENTALITY":
                        PackMentality = true;
                        NotPackMentality = false;
                        break;
                    case "FAST_SNIPE":
                        FastSnipe = true;
                        NotFastSnipe = false;
                        break;
                }
            }

            var inherentPowersList = CurrentBuild?.Powers
                .Where(p => p is { Chosen: false, Power: { }} && CurrentBuild.PowerUsed(p.Power)).Select(p => p.Power)
                .ToList();
            if (inherentPowersList != null)
            {
                foreach (var inherent in inherentPowersList)
                {
                    if (inherent == null)
                    {
                        continue;
                    }

                    if (inherent.InherentType == Enums.eGridType.None)
                    {
                        continue;
                    }

                    var priority = (int)Enum.Parse(typeof(Enums.eInherentOrder), inherent.InherentType.ToString());
                    InherentDisplayList.Add(new InherentDisplayItem(priority, inherent));
                }

                InherentDisplayList = new List<InherentDisplayItem>(InherentDisplayList.OrderBy(x => x.Priority));
            }

            if (CurrentBuild == null) return;
            {
                foreach (var power in CurrentBuild.Powers.Where(power => power?.Power != null))
                {
                    switch (power.Power.PowerName.ToUpper())
                    {
                        case "BOXING":
                            BoxingBuff = true;
                            NotBoxingBuff = false;
                            break;
                        case "KICK":
                            KickBuff = true;
                            NotKickBuff = false;
                            break;
                        case "CROSS_PUNCH":
                            CrossPunchBuff = true;
                            NotCrossPunchBuff = false;
                            break;
                    }

                    if (CurrentBuild != null && (power.Chosen || !CurrentBuild.PowerUsed(power.Power))) continue;
                    var displayItem = InherentDisplayList.FirstOrDefault(x => x.Power.FullName == power.Power.FullName);
                    power.Power.DisplayLocation = InherentDisplayList.IndexOf(displayItem);
                }
            }
        }

        protected void ReadMetadata(string buildText)
        {
            var tags = new List<string> {"comment", "enhobtained"};

            var metadata = MidsCharacterFileFormat.ReadMetadata(tags, buildText);

            foreach (var tag in tags)
            {
                if (!metadata.ContainsKey(tag))
                {
                    continue;
                }

                switch (tag)
                {
                    case "comment":
                        Comment = metadata[tag];
                        break;

                    case "enhobtained":
                        var obtainedSlots = metadata[tag];
                        var n = obtainedSlots.Length;
                        var k = 0;

                        for (var i = 0; i < CurrentBuild.Powers.Count; i++)
                        {
                            if (CurrentBuild.Powers[i].Power == null)
                            {
                                continue;
                            }

                            for (var j = 0; j < CurrentBuild.Powers[i].Slots.Length; j++)
                            {
                                if (k < n)
                                {
                                    var obtained = obtainedSlots[k] == '1';
                                    CurrentBuild.Powers[i].Slots[j].Enhancement.Obtained = obtained;
                                    CurrentBuild.Powers[i].Slots[j].FlippedEnhancement.Obtained = obtained;

                                    k++;
                                }
                                else
                                {
                                    CurrentBuild.Powers[i].Slots[j].Enhancement.Obtained = false;
                                    CurrentBuild.Powers[i].Slots[j].FlippedEnhancement.Obtained = false;
                                }
                            }
                        }

                        break;
                }
            }
        }

        public void Validate()
        {
            CheckAncillaryPowerSet();
            CurrentBuild?.Validate();
            RefreshActiveSpecial();
        }

        /// <summary>
        /// Returns true if there is a clash between two chosen powersets.
        /// </summary>
        /// <param name="nIDPower"></param>
        /// <returns></returns>
        protected bool PowersetMutexClash(int nIDPower)
        {
            //Returns true if there's a clash.
            var powerSetId = DatabaseAPI.Database.Power[nIDPower].PowerSetID;

            Enums.PowersetType powersetType;

            //Only check the one set (ie, if power is in primary, we check secondary)
            switch (DatabaseAPI.Database.Powersets[powerSetId].SetType)
            {
                case Enums.ePowerSetType.Primary:
                    powersetType = Enums.PowersetType.Secondary;
                    break;
                case Enums.ePowerSetType.Secondary:
                    powersetType = Enums.PowersetType.Primary;
                    break;
                case Enums.ePowerSetType.Ancillary:
                    powersetType = Enums.PowersetType.Ancillary;
                    break;
                default:
                    return false;
            }

            var psID = Powersets[(int) powersetType] == null ? -1 : Powersets[(int) powersetType].nID;

            if (powersetType == Enums.PowersetType.None)
                return false;
            if (DatabaseAPI.Database.Powersets[powerSetId].nIDMutexSets.Any(t => t == psID))
            {
                // Powerset combination is denied
                return true;
            }

            if (Powersets[(int) powersetType] == null)
                return false;

            return Powersets[(int) powersetType].nIDMutexSets
                .Where(t => Powersets[(int) powersetType] != null).Any(t => t == powerSetId);
        }

        private void CheckAncillaryPowerSet()
        {
            var powersetIndexes = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Ancillary);
            if (powersetIndexes.Length == 0)
            {
                Powersets[7] = null;
            }
            else if (Powersets[7] == null)
            {
                Powersets[7] = powersetIndexes[0];
            }
            else
            {
                var flag = false;
                foreach (var p in powersetIndexes)
                {
                    if (Powersets[7].nID == p.nID)
                    {
                        flag = true;
                    }
                }

                if (!flag && powersetIndexes.Length > 0)
                {
                    Powersets[7] = powersetIndexes[0];
                }
            }
        }

        private IEnumerable<int> PoolGetAvailable(int iPool)
        {
            var powersetIndexes = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Pool);
            var intList = new List<int>();
            foreach (var index1 in powersetIndexes)
            {
                var available = false;
                for (var index2 = 3; index2 <= 6; ++index2)
                    if (index2 == iPool || !(PoolLocked[index2 - 3] && index1.nID == Powersets[index2].nID))
                        available = true;
                if (available)
                    intList.Add(index1.nID);
            }

            return intList;
        }

        public int PoolToComboID(int iPool, int index)
        {
            var available = PoolGetAvailable(iPool);
            var num1 = -1;
            foreach (var num2 in available)
            {
                ++num1;
                if (num2 == index)
                    return num1;
            }

            return 0;
        }

        public static PopUp.PopupData PopEnhInfo(I9Slot iSlot, int iLevel = -1, PowerEntry? powerEntry = null)
        {
            var popupData1 = new PopUp.PopupData();
            var index1 = popupData1.Add();
            PopUp.PopupData popupData2;
            if (iSlot.Enh < 0)
            {
                popupData1.Sections[index1].Add("Empty Slot", PopUp.Colors.Disabled, 1.25f);
                if (iLevel > -1)
                {
                    popupData1.Sections[index1].Add($"Slot placed at level: {iLevel + 1}", PopUp.Colors.Text);
                    if (powerEntry != null)
                    {
                        var slot = powerEntry.Slots.FirstOrDefault(x => x.Enhancement == iSlot);
                        if (slot.IsInherent)
                        {
                            popupData1.Sections[index1].Add($"This slot is an Inherent slot.", PopUp.Colors.Text);
                        }
                    }
                }

                var index2 = popupData1.Add();
                popupData1.Sections[index2].Add("Right-Click to place an enhancement.", PopUp.Colors.Disabled, 1f, FontStyle.Bold | FontStyle.Italic);
                popupData1.Sections[index2].Add("Shift-Click to move this slot.", PopUp.Colors.Disabled, 1f, FontStyle.Bold | FontStyle.Italic);
                popupData2 = popupData1;
            }
            else
            {
                var enhancement = DatabaseAPI.Database.Enhancements[iSlot.Enh];
                switch (enhancement.TypeID)
                {
                    case Enums.eType.Normal:
                        popupData1.Sections[index1].Add(enhancement.Name, PopUp.Colors.Title, 1.25f);
                        break;
                    case Enums.eType.InventO:
                        popupData1.Sections[index1].Add($"Invention: {enhancement.Name}", PopUp.Colors.Title, 1.25f);
                        break;
                    case Enums.eType.SpecialO:
                        popupData1.Sections[index1].Add(enhancement.Name, PopUp.Colors.Title, 1.25f);
                        break;
                    case Enums.eType.SetO:
                        var iColor = PopUp.Colors.Title;
                        if (enhancement.RecipeIDX > -1)
                        {
                            iColor = DatabaseAPI.Database.Recipes[enhancement.RecipeIDX].Rarity switch
                            {
                                Recipe.RecipeRarity.Uncommon => PopUp.Colors.Uncommon,
                                Recipe.RecipeRarity.Rare => PopUp.Colors.Rare,
                                Recipe.RecipeRarity.UltraRare => PopUp.Colors.UltraRare,
                                _ => iColor
                            };
                        }

                        popupData1.Sections[index1].Add($"{DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].DisplayName}: {enhancement.Name}", iColor, 1.25f);
                        break;
                }

                switch (enhancement.TypeID)
                {
                    case Enums.eType.Normal:
                        popupData1.Sections[index1].Add(iSlot.GetEnhancementString(), Color.FromArgb(0, 255, 0));
                        break;
                    case Enums.eType.InventO:
                        popupData1.Sections[index1].Add($"Invention Level: {iSlot.IOLevel + 1}{iSlot.GetRelativeString(false)} - {iSlot.GetEnhancementString()}", PopUp.Colors.Invention);
                        break;
                    case Enums.eType.SpecialO:
                        popupData1.Sections[index1].Add(iSlot.GetEnhancementString(), Color.Yellow);
                        break;
                    case Enums.eType.SetO:
                        if (!DatabaseAPI.EnhIsNaturallyAttuned(iSlot.Enh))
                        {
                            popupData1.Sections[index1].Add($"Invention Level: {iSlot.IOLevel + 1}{iSlot.GetRelativeString(false)}", PopUp.Colors.Invention);
                        }
                        break;
                }

                if (iLevel > -1)
                {
                    popupData1.Sections[index1].Add($"Slot placed at level: {iLevel + 1}", PopUp.Colors.Text);
                }

                if (enhancement.Unique)
                {
                    index1 = popupData1.Add();
                    popupData1.Sections[index1].Add("This enhancement is Unique. No more than one enhancement of this type can be slotted by a character.", PopUp.Colors.Text, 0.9f);
                }

                switch (enhancement.TypeID)
                {
                    case Enums.eType.Normal:
                    case Enums.eType.InventO:
                        if (!string.IsNullOrEmpty(enhancement.Desc))
                        {
                            popupData1.Sections[index1].Add(enhancement.Desc, PopUp.Colors.Title);
                            break;
                        }

                        var index2 = popupData1.Add();
                        var strArray1 = BreakByNewLine(iSlot.GetEnhancementStringLong());
                        foreach (var s in strArray1)
                        {
                            var strArray2 = BreakByBracket(s);
                            popupData1.Sections[index2].Add(strArray2[0], Color.FromArgb(0, 255, 0),
                                strArray2[1], Color.FromArgb(0, 255, 0), 0.9f);
                        }

                        break;
                    case Enums.eType.SpecialO:
                    case Enums.eType.SetO:
                        if (!string.IsNullOrEmpty(enhancement.Desc))
                        {
                            popupData1.Sections[index1].Add(enhancement.Desc, PopUp.Colors.Title);
                        }

                        var index4 = popupData1.Add();
                        var strArray3 = BreakByNewLine(iSlot.GetEnhancementStringLong());
                        for (var index3 = 0; index3 < strArray3.Length; index3++)
                        {
                            var strArray2 = !enhancement.HasPowerEffect
                                ? BreakByBracket(strArray3[index3])
                                : new[] { strArray3[index3], string.Empty };


                            popupData1.Sections[index4].Add(strArray2[0], Color.FromArgb(0, 255, 0), strArray2[1], Color.FromArgb(0, 255, 0), 0.9f);
                        }

                        break;
                }

                if (!MidsContext.Config.PopupRecipes)
                {
                    if (enhancement.TypeID == Enums.eType.SetO)
                    {
                        var index3 = popupData1.Add();
                        popupData1.Sections[index3].Add($"Set Type: {DatabaseAPI.GetSetTypeByIndex(DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].SetType).Name}", PopUp.Colors.Invention);
                        //popupData1.Sections[index3].Add("Set Type: " + DatabaseAPI.Database.SetTypeStringLong[DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].SetType], PopUp.Colors.Invention);
                        var levelMin = DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].LevelMin + 1;
                        var levelMax = DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].LevelMax + 1;
                        popupData1.Sections[index3]
                            .Add(
                                levelMin == levelMax
                                    ? $"Set Level: {levelMin}"
                                    : $"Set Level Range: {levelMin} to {levelMax}",
                                PopUp.Colors.Text);
                        popupData1.Add(PopSetEnhList(enhancement.nIDSet, powerEntry));
                        popupData1.Add(PopSetBonusListing(enhancement.nIDSet, powerEntry));
                    }
                }
                else if (enhancement.TypeID == Enums.eType.SetO || enhancement.TypeID == Enums.eType.InventO)
                {
                    popupData1.Add(PopRecipeInfo(enhancement.RecipeIDX, iSlot.IOLevel, iSlot.RelativeLevel));
                }

                popupData2 = popupData1;
            }

            return popupData2;
        }

        public int GetFirstAvailablePowerIndex(int iLevel = 0)
        {
            for (var index = 0; index < Math.Min(CurrentBuild.Powers.Count, CurrentBuild.LastPower + 1); index++)
            {
                if (CurrentBuild.Powers[index].NIDPowerset < 0 && CurrentBuild.Powers[index].Level >= iLevel)
                {
                    return index;
                }
            }

            return -1;
        }

        private static int GetFirstAvailablePowerLevel(Build? currentbuild, int iLevel = 0)
        {
            for (var index = 0; index < Math.Min(currentbuild.Powers.Count, currentbuild.LastPower + 1); index++)
            {
                if ((currentbuild.Powers[index].NIDPowerset < 0) & (currentbuild.Powers[index].Level >= iLevel))
                {
                    return currentbuild.Powers[index].Level;
                }
            }

            return -1;
        }

        private int GetFirstAvailableSlotLevel(int iLevel = 0)
        {
            if (iLevel < 0)
            {
                iLevel = 0;
            }

            for (var level = iLevel; level < DatabaseAPI.Database.Levels.Length; level++)
            {
                if (DatabaseAPI.Database.Levels[level].Slots > 0 && DatabaseAPI.Database.Levels[level].Slots - CurrentBuild.SlotsPlacedAtLevel(level) > 0)
                    return level;
            }

            return -1;
        }

        public int SlotCheck(PowerEntry power)
        {
            if (power.Power == null || !CanPlaceSlot || power.SlotCount > 5)
                return -1;
            if (!DatabaseAPI.Database.Power[power.NIDPower].Slottable)
            {
                return -1;
            }

            var iLevel = power.Level;
            if (DatabaseAPI.Database.Power[power.NIDPower].AllowFrontLoading)
            {
                iLevel = 0;
            }

            var firstAvailable = GetFirstAvailableSlotLevel(iLevel);

            return firstAvailable;
        }

        public int[] GetSlotCounts()
        {
            var numArray = new int[2];
            for (var level = 0; level < DatabaseAPI.Database.Levels.Length; ++level)
            {
                if (DatabaseAPI.Database.Levels[level].Slots <= 0)
                    continue;
                var num = CurrentBuild.SlotsPlacedAtLevel(level);
                numArray[0] += DatabaseAPI.Database.Levels[level].Slots - num;
                numArray[1] += num;
            }

            return numArray;
        }

        public int[] GetSlotCounts(int level)
        {
            var numArray = new int[2];

            var numTaken = SlotLevelQueue.GetNumSlotsBeforeLevel(level) + CurrentBuild.SlotsPlacedAtLevel(level);
            var numTotal = DatabaseAPI.Database.Levels.Sum(e => e.Slots);

            numArray[0] = numTotal - numTaken;
            numArray[1] = numTaken;

            return numArray;
        }

        private static string[] BreakByNewLine(string iString)
        {
            iString = iString.Replace('\n', '^');
            return iString.Split('^');
        }

        private static string[] BreakByBracket(string iString)
        {
            string[] strArray1 = { iString, string.Empty };
            var length = iString.IndexOf(" (", StringComparison.Ordinal);
            string[] strArray2;
            if (length < 0)
            {
                strArray2 = strArray1;
            }
            else
            {
                strArray1[0] = iString.Substring(0, length) + ":";
                if (iString.Length - (length + 1) > 0)
                {
                    strArray1[1] = iString.Substring(length + 1).Replace("(", "").Replace(")", "");
                }

                strArray2 = strArray1;
            }

            return strArray2;
        }

        private static PopUp.Section? PopSetBonusListing(int sIdx, PowerEntry power)
        {
            var section1 = new PopUp.Section();
            section1.Add("Set Bonus:", PopUp.Colors.Title);
            var num = 0;
            if (power != null)
            {
                for (var index = 0; index <= power.Slots.Length - 1; ++index)
                {
                    if (power.Slots[index].Enhancement.Enh > -1 &&
                        DatabaseAPI.Database.Enhancements[power.Slots[index].Enhancement.Enh].nIDSet == sIdx)
                    {
                        ++num;
                    }
                }
            }

            if (sIdx < 0 || sIdx > DatabaseAPI.Database.EnhancementSets.Count - 1)
            {
                return section1;
            }

            var enhancementSet = DatabaseAPI.Database.EnhancementSets[sIdx];
            for (var index = 0; index <= enhancementSet.Bonus.Length - 1; ++index)
            {
                var effectString = enhancementSet.GetEffectString(index, false, true, true, true);
                if (string.IsNullOrEmpty(effectString))
                {
                    continue;
                }

                if (enhancementSet.Bonus[index].PvMode == Enums.ePvX.PvP)
                {
                    effectString += " [PvP]";
                }

                if ((num >= enhancementSet.Bonus[index].Slotted) & (((enhancementSet.Bonus[index].PvMode == Enums.ePvX.PvE) & !MidsContext.Config.Inc.DisablePvE) | ((enhancementSet.Bonus[index].PvMode == Enums.ePvX.PvP) & MidsContext.Config.Inc.DisablePvE) | (enhancementSet.Bonus[index].PvMode == Enums.ePvX.Any)))
                {
                    section1.Add("(" + enhancementSet.Bonus[index].Slotted + ") " + effectString, PopUp.Colors.Effect, 0.9f);
                }
                else if (power == null)
                {
                    section1.Add("(" + enhancementSet.Bonus[index].Slotted + ") " + effectString, PopUp.Colors.Effect, 0.9f);
                }
                else
                {
                    section1.Add("(" + enhancementSet.Bonus[index].Slotted + ") " + effectString, PopUp.Colors.Disabled, 0.9f);
                }
            }

            for (var index = 0; index <= enhancementSet.SpecialBonus.Length - 1; ++index)
            {
                var checkStatus = false;
                List<Power> specialPowers = null;
                if (enhancementSet.SpecialBonus.Length > 0)
                {
                    specialPowers = enhancementSet.SpecialBonus[enhancementSet.SpecialBonus.Length - 1].Index.Length switch
                    {
                        0 => enhancementSet.GetEnhancementSetLinkedPowers(enhancementSet.SpecialBonus.Length - 2, true),
                        _ => enhancementSet.GetEnhancementSetLinkedPowers(enhancementSet.SpecialBonus.Length - 1, true)
                    };
                }

                if (specialPowers is { Count: 1 })
                {
                    if (specialPowers[0].FullName.Contains("Skin") || specialPowers[0].FullName.Contains("Aegis")) checkStatus = true;
                }
                var effectString = enhancementSet.GetEffectString(index, true, true, true, true, checkStatus);
                if (string.IsNullOrEmpty(effectString))
                {
                    continue;
                }

                var flag = false;
                if (power != null)
                {
                    foreach (var slot in power.Slots)
                    {
                        if (slot.Enhancement.Enh > -1 && enhancementSet.SpecialBonus[index].Special > -1 && slot.Enhancement.Enh == enhancementSet.Enhancements[enhancementSet.SpecialBonus[index].Special])
                        {
                            flag = true;
                        }
                    }
                }

                if (flag)
                {
                    section1.Add("(Enh) " + effectString, PopUp.Colors.Effect, 0.9f);
                }
                else if (power == null)
                {
                    section1.Add("(Enh) " + effectString, PopUp.Colors.Effect, 0.9f);
                }
                else
                {
                    section1.Add("(Enh) " + effectString, PopUp.Colors.Disabled, 0.9f);
                }
            }

            return section1;
        }

        private static void GetSalvageCostOuter(ref Dictionary<Enums.RewardCurrency, int> costList, Salvage s, int amount)
        {
            var sCost = clsRewardCurrency.GetSalvageCost(s, MidsContext.Config.PreferredCurrency, amount);
            if (sCost != null)
            {
                costList[MidsContext.Config.PreferredCurrency] += (int) sCost;
            }
            else
            {
                var sCost2 = clsRewardCurrency.GetSalvageCost(s, Enums.RewardCurrency.RewardMerit, amount);
                if (sCost2 != null)
                {
                    costList[Enums.RewardCurrency.RewardMerit] += (int) sCost2;
                }
            }
        }

        public static PopUp.Section? PopRecipeInfo(int rIdx, int iLevel, Enums.eEnhRelative relLevel = Enums.eEnhRelative.Even)
        {
            var section1 = new PopUp.Section();
            if (rIdx < 0) return section1;

            var recipe = DatabaseAPI.Database.Recipes[rIdx];
            var index1 = -1;
            var lvlUbound = 52;
            var lvlLbound = 0;
            for (var index2 = 0; index2 < recipe.Item.Length; index2++)
            {
                if (recipe.Item[index2].Level > lvlLbound)
                    lvlLbound = recipe.Item[index2].Level;
                if (recipe.Item[index2].Level < lvlUbound)
                    lvlUbound = recipe.Item[index2].Level;
                if (recipe.Item[index2].Level != iLevel)
                    continue;
                index1 = index2;
                break;
            }

            if (index1 < 0)
            {
                iLevel = Enhancement.GranularLevelZb(iLevel, 0, 49);
                for (var index2 = 0; index2 < recipe.Item.Length; index2++)
                {
                    if (recipe.Item[index2].Level != iLevel)
                        continue;
                    index1 = index2;
                    break;
                }
            }

            if (index1 < 0) return section1;

            var recipeEntry = recipe.Item[index1];
            if (recipe.EnhIdx > -1 & !recipe.IsGeneric & !recipe.InternalName.StartsWith("G_"))
            {
                section1.Add($"Recipe - {DatabaseAPI.Database.Enhancements[recipe.EnhIdx].LongName}", PopUp.Colors.Title);
            }
            else
            {
                section1.Add($"Materials:", PopUp.Colors.Title);
            }

            if (recipeEntry.BuyCost > 0)
                section1.Add("Buy Cost:", PopUp.Colors.Invention, $"{recipeEntry.BuyCost:###,###,##0}",
                    PopUp.Colors.Invention, 0.9f, FontStyle.Bold, 1);
            if (recipeEntry.CraftCost > 0)
                section1.Add("Craft Cost:", PopUp.Colors.Invention, $"{recipeEntry.CraftCost:###,###,##0}",
                    PopUp.Colors.Invention, 0.9f, FontStyle.Bold, 1);
            if (recipeEntry.CraftCostM > 0)
                section1.Add("Craft Cost (Memorized):", PopUp.Colors.Effect, $"{recipeEntry.CraftCostM:###,###,##0}",
                    PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);

            var subRecipesCost = Enum.GetValues(typeof(Enums.RewardCurrency))
                .Cast<Enums.RewardCurrency>()
                .ToDictionary(c => c, _ => 0);

            for (var index2 = 0;
                index2 < recipeEntry.Salvage.Length &&
                (index2 == 0 || recipeEntry.SalvageIdx[index2] != recipeEntry.SalvageIdx[0]);
                index2++)
            {
                if (recipeEntry.SalvageIdx[index2] < 0) continue;

                var iColor = DatabaseAPI.Database.Salvage[recipeEntry.SalvageIdx[index2]].Rarity switch
                {
                    Recipe.RecipeRarity.Uncommon => PopUp.Colors.Uncommon,
                    Recipe.RecipeRarity.Rare => PopUp.Colors.Rare,
                    Recipe.RecipeRarity.UltraRare => PopUp.Colors.UltraRare,
                    _ => PopUp.Colors.Common
                };

                if (recipeEntry.Count[index2] <= 0) continue;

                section1.Add(DatabaseAPI.Database.Salvage[recipeEntry.SalvageIdx[index2]].ExternalName,
                    iColor, recipeEntry.Count[index2].ToString(CultureInfo.InvariantCulture), PopUp.Colors.Title,
                    0.9f, FontStyle.Bold, 1);
                GetSalvageCostOuter(ref subRecipesCost, DatabaseAPI.Database.Salvage[recipeEntry.SalvageIdx[index2]],
                    recipeEntry.Count[index2]);
            }

            var numBoosters = relLevel switch
            {
                Enums.eEnhRelative.PlusOne => 1,
                Enums.eEnhRelative.PlusTwo => 2,
                Enums.eEnhRelative.PlusThree => 3,
                Enums.eEnhRelative.PlusFour => 4,
                Enums.eEnhRelative.PlusFive => 5,
                _ => 0
            };

            if (numBoosters > 0)
            {
                //section1.Add("", PopUp.Colors.Title);
                section1.Add("Enhancement Booster",
                    PopUp.Colors.Rare, numBoosters.ToString(CultureInfo.InvariantCulture), PopUp.Colors.Title,
                    0.9f, FontStyle.Bold, 1);
                var boosterSalvage = DatabaseAPI.Database.Salvage.First(s => s.ExternalName == "Enhancement Booster");
                GetSalvageCostOuter(ref subRecipesCost, boosterSalvage, numBoosters);
            }

            var subCostTotal = subRecipesCost.Count <= 0 ? 0 : subRecipesCost.Sum(e => e.Value);
            if (subRecipesCost.Count > 0 & subCostTotal > 0)
            {
                section1.Add("", PopUp.Colors.Title);
                section1.Add($"Salvage detailed cost:", PopUp.Colors.Title);
                foreach (var c in subRecipesCost)
                {
                    if (c.Value <= 0) continue;
                    var cAmt = c.Key == Enums.RewardCurrency.Influence
                        ? $"{c.Value:###,###,##0}"
                        : c.Value.ToString(CultureInfo.InvariantCulture);
                    section1.Add(clsRewardCurrency.GetCurrencyName(c.Key),
                        clsRewardCurrency.GetCurrencyRarityColor(c.Key), cAmt, PopUp.Colors.Title,
                        0.9f, FontStyle.Bold, 1);
                }
            }

            return section1;
        }

        public static PopUp.PopupData PopSetInfo(int sIdx, PowerEntry powerEntry = null)
        {
            if (sIdx < 0)
            {
                var popupData1 = new PopUp.PopupData();
                return popupData1;
            }
            else
            {
                var enhancementSet = DatabaseAPI.Database.EnhancementSets[sIdx];
                var iColor = PopUp.Colors.Uncommon;
                for (var index = 0; index <= enhancementSet.Enhancements.Length - 1; ++index)
                {
                    var enhancement = DatabaseAPI.Database.Enhancements[enhancementSet.Enhancements[index]];
                    if (enhancement.RecipeIDX <= -1)
                        continue;
                    if (DatabaseAPI.Database.Recipes[enhancement.RecipeIDX].Rarity == Recipe.RecipeRarity.Rare)
                    {
                        iColor = PopUp.Colors.Rare;
                        break;
                    }

                    if (DatabaseAPI.Database.Recipes[enhancement.RecipeIDX].Rarity == Recipe.RecipeRarity.UltraRare)
                    {
                        iColor = PopUp.Colors.UltraRare;
                        break;
                    }

                    if (index > 2)
                        break;
                }

                var popupData1 = new PopUp.PopupData();
                var index1 = popupData1.Add();
                popupData1.Sections[index1].Add(enhancementSet.DisplayName, iColor, 1.25f);
                popupData1.Sections[index1].Add($"Set Type: {DatabaseAPI.GetSetTypeByIndex(enhancementSet.SetType).Name}", PopUp.Colors.Invention);
                //popupData1.Sections[index1].Add("Set Type: " + DatabaseAPI.Database.SetTypeStringLong[(int)enhancementSet.SetType], PopUp.Colors.Invention);
                string str;
                if (enhancementSet.LevelMin != enhancementSet.LevelMax)
                {
                    str = enhancementSet.LevelMin + 1 + " to " + (enhancementSet.LevelMax + 1);
                }
                else
                {
                    str = (enhancementSet.LevelMin + 1).ToString(CultureInfo.InvariantCulture);
                }

                popupData1.Sections[index1].Add("Level Range: " + str, PopUp.Colors.Text);
                popupData1.Add(PopSetEnhList(sIdx, powerEntry));
                popupData1.Add(PopSetBonusListing(sIdx, powerEntry));
                return popupData1;
            }
        }

        private static PopUp.Section? PopSetEnhList(int sIdx, PowerEntry power)
        {
            if (sIdx < 0 || sIdx > DatabaseAPI.Database.EnhancementSets.Count - 1) return new PopUp.Section();

            var num = 0;
            var enhancementSet = DatabaseAPI.Database.EnhancementSets[sIdx];
            var flagArray = new bool[enhancementSet.Enhancements.Length];
            for (var index = 0; index <= flagArray.Length - 1; ++index)
                flagArray[index] = false;
            if (power != null)
                foreach (var slot in power.Slots)
                {
                    if (slot.Enhancement.Enh < 0 ||
                        DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].nIDSet != sIdx)
                        continue;
                    ++num;
                    for (var index = 0; index <= enhancementSet.Enhancements.Length - 1; ++index)
                        if (slot.Enhancement.Enh == enhancementSet.Enhancements[index])
                            flagArray[index] = true;
                }

            var section1 = new PopUp.Section();
            if (power != null)
                section1.Add(
                    "Set: " + enhancementSet.DisplayName + " (" + num + "/" + enhancementSet.Enhancements.Length + ")",
                    PopUp.Colors.Title);
            for (var index = 0; index <= enhancementSet.Enhancements.Length - 1; ++index)
            {
                var iText = enhancementSet.DisplayName + ": " +
                            DatabaseAPI.Database.Enhancements[enhancementSet.Enhancements[index]].Name;
                if (flagArray[index] || power == null)
                    section1.Add(iText, PopUp.Colors.Invention, 0.9f, FontStyle.Bold, 1);
                else
                    section1.Add(iText, PopUp.Colors.Disabled, 0.9f, FontStyle.Bold, 1);
            }

            return section1;
        }

        public void PoolShuffle()
        {
            //var poolPowers = this.Powersets.Skip(2).Take(4).ToList();
            var poolOrder = new int[4];
            var poolIndex = new int[4];
            for (var i = 3; i <= 6; ++i)
            {
                // This can actually happen.
                // See: https://forums.homecomingservers.com/topic/19963-mids-reborn-hero-designer/?do=findComment&comment=382180
                if (Powersets[i] == null)
                {
                    if (i == 3)
                    {
                        Powersets[i] = DatabaseAPI.GetPowersetByIndex(DatabaseAPI.GetPowersetIndexesByGroupName("Pool")[0]);
                    }
                    else
                    {
                        Powersets[i] = Powersets[i - 1].Clone();
                    }
                }
                var ps = Powersets[i];
                poolIndex[i - 3] = ps?.nID ?? -1;
                poolOrder[i - 3] = ps != null ? GetEarliestPowerIndex(Powersets[i].nID) : -1;
            }

            for (var i = 0; i <= 3; ++i)
            {
                int minO = byte.MaxValue;
                var minI = -1;
                for (var x = 0; x <= poolOrder.Length - 1; ++x)
                {
                    if (minO <= poolOrder[x])
                        continue;
                    minO = poolOrder[x];
                    minI = x;
                }

                if (minI <= -1 || poolIndex[minI] <= -1)
                    continue;
                Powersets[i + 3] = DatabaseAPI.Database.Powersets[poolIndex[minI]];
                poolOrder[minI] = 512;
            }

            for (var i = 3; i < 7; i++)
            {
                if (Powersets[i].SetName == "Leadership_beta")
                {
                    Powersets[i] = DatabaseAPI.GetPowersetByName("Leadership");
                }
            }

            // HACK: this assumes at least 8 powersets exist, but the database is fully editable.
            PoolLocked[0] = PowersetUsed(Powersets[3]) & PoolUnique(Enums.PowersetType.Pool0);
            PoolLocked[1] = PowersetUsed(Powersets[4]) & PoolUnique(Enums.PowersetType.Pool1);
            PoolLocked[2] = PowersetUsed(Powersets[5]) & PoolUnique(Enums.PowersetType.Pool2);
            PoolLocked[3] = PowersetUsed(Powersets[6]) & PoolUnique(Enums.PowersetType.Pool3);
            PoolLocked[4] = PowersetUsed(Powersets[7]);
        }

        private int GetEarliestPowerIndex(int iSet)
        {
            for (var index = 0; index < Math.Min(CurrentBuild.Powers.Count, CurrentBuild.LastPower); index++)
            {
                if (CurrentBuild.Powers[index].NIDPowerset == iSet)
                {
                    return index;
                }
            }

            return CurrentBuild.LastPower + 1;
        }

        private bool PoolUnique(Enums.PowersetType pool)
        {
            var ps = Powersets[(int)pool];
            if (ps == null) return false;
            for (var index = 3; (Enums.PowersetType)index < pool; ++index)
                if (Powersets[index] != null && Powersets[index].nID == Powersets[(int)pool].nID)
                    return false;
            return true;
        }

        private bool PowersetUsed(IPowerset? powerset)
        {
            return powerset != null &&
                   CurrentBuild.Powers.Any(t => (t?.NIDPowerset == powerset.nID) & (t?.IDXPower > -1));
        }

        protected bool CanRemovePower(int index, bool allowSecondary, out string message)
        {
            message = string.Empty;
            var power = CurrentBuild.Powers[index];
            if (!power.Chosen)
            {
                message =
                    "You can't remove inherent powers.\nIf the power is a Kheldian form power, you can remove it by removing the shapeshift power which grants it.";
                return false;
            }

            if (!((power.NIDPowerset == Powersets[1].nID) & (power.IDXPower == 0) & !allowSecondary))
                return power.NIDPowerset >= 0;
            if (CurrentBuild.PowersPlaced <= 1)
                return true;
            message = "The first power from your secondary set is non-optional and can't be removed.";
            return false;
        }

        public void SwitchSets(IPowerset? newPowerset, IPowerset? oldPowerset)
        {
            int oldTrunk;
            int oldBranch;
            int newTrunk;
            int newBranch;
            if (oldPowerset.nIDTrunkSet > -1)
            {
                oldTrunk = oldPowerset.nIDTrunkSet;
                oldBranch = oldPowerset.nID;
                if (newPowerset.nIDTrunkSet > -1)
                {
                    newTrunk = newPowerset.nIDTrunkSet;
                    newBranch = newPowerset.nID;
                }
                else
                {
                    newTrunk = newPowerset.nID;
                    newBranch = -1;
                }
            }
            else
            {
                oldTrunk = oldPowerset.nID;
                oldBranch = -1;
                if (newPowerset.nIDTrunkSet > -1)
                {
                    newTrunk = newPowerset.nIDTrunkSet;
                    newBranch = newPowerset.nID;
                }
                else
                {
                    newTrunk = newPowerset.nID;
                    newBranch = -1;
                }
            }

            for (var index4 = 0; index4 < Powersets.Length; ++index4)
                if (Powersets[index4] != null && Powersets[index4].nID == oldPowerset.nID)
                    Powersets[index4] = newPowerset;
            foreach (var power in CurrentBuild.Powers)
            {
                if (power.NIDPowerset < 0)
                    continue;
                var idxPower = power.IDXPower;
                if (power.NIDPowerset == oldTrunk)
                {
                    for (var index4 = 0;
                        index4 < DatabaseAPI.Database.Powersets[oldTrunk].Power.Length &&
                        DatabaseAPI.Database.Powersets[oldTrunk].Powers[index4].Level == 0;
                        ++index4)
                        --idxPower;
                    for (var index4 = 0;
                        index4 < DatabaseAPI.Database.Powersets[newTrunk].Power.Length &&
                        DatabaseAPI.Database.Powersets[newTrunk].Powers[index4].Level == 0;
                        ++index4)
                        ++idxPower;
                    if (newTrunk < 0)
                    {
                        power.Reset();
                    }
                    else if (idxPower > DatabaseAPI.Database.Powersets[newTrunk].Power.Length - 1 || idxPower < 0)
                    {
                        power.Reset();
                    }
                    else
                    {
                        power.NIDPowerset = newTrunk;
                        power.NIDPower = DatabaseAPI.Database.Powersets[newTrunk].Power[idxPower];
                        power.IDXPower = idxPower;
                    }
                }
                else if (power.NIDPowerset == oldBranch)
                {
                    for (var index4 = 0;
                        index4 < DatabaseAPI.Database.Powersets[oldTrunk].Power.Length &&
                        DatabaseAPI.Database.Powersets[oldTrunk].Powers[index4].Level == 0;
                        ++index4)
                        --idxPower;
                    for (var index4 = 0;
                        index4 < DatabaseAPI.Database.Powersets[newTrunk].Power.Length &&
                        DatabaseAPI.Database.Powersets[newTrunk].Powers[index4].Level == 0;
                        ++index4)
                        ++idxPower;
                    if (newBranch < 0 || idxPower > DatabaseAPI.Database.Powersets[newBranch].Power.Length - 1)
                    {
                        power.Reset();
                    }
                    else
                    {
                        power.NIDPowerset = newBranch;
                        power.NIDPower = DatabaseAPI.Database.Powersets[newBranch].Power[idxPower];
                        power.IDXPower = idxPower;
                    }
                }

                if (power.Power == null || !power.Power.Slottable)
                    power.Slots = Array.Empty<SlotEntry>();
                else if (power.Slots.Length == 0)
                    power.Slots = new[]
                    {
                        new SlotEntry
                        {
                            Enhancement = new I9Slot(),
                            FlippedEnhancement = new I9Slot(),
                            Level = power.Level
                        }
                    };
                else if (idxPower > -1)
                    for (var index4 = 0; index4 < power.SlotCount; ++index4)
                        if (!power.PowerSet.Powers[idxPower].IsEnhancementValid(power.Slots[index4].Enhancement.Enh))
                            power.Slots[index4].Enhancement = new I9Slot();
            }

            CurrentBuild.FullMutexCheck();
        }

        public class TotalStatistics
        {
            internal TotalStatistics()
            {
                // do not set values to the value they default to in a constructor
                Init(false);
            }

            public float[] Def { get; private set; }
            public float[] Res { get; private set; }
            public float[] Mez { get; private set; }
            public float[] MezRes { get; private set; }
            public float[] DebuffRes { get; private set; }
            public float[] Elusivity { get; set; }
            public float ElusivityMax => Elusivity.Max();
            public float HPRegen { get; set; }
            public float HPMax { get; set; }
            public float Absorb { get; set; }
            public float EndRec { get; set; }
            public float EndUse { get; set; }
            public float EndMax { get; set; }
            public float RunSpd { get; set; }
            public float MaxRunSpd { get; set; }
            public float JumpSpd { get; set; }
            public float MaxJumpSpd { get; set; }
            public float FlySpd { get; set; }
            public float MaxFlySpd { get; set; }
            public float JumpHeight { get; set; }
            public float StealthPvE { get; set; }
            public float StealthPvP { get; set; }
            public float ThreatLevel { get; set; }
            public float Perception { get; set; }
            public float BuffHaste { get; set; }
            public float BuffAcc { get; set; }
            public float BuffToHit { get; set; }
            public float BuffDam { get; set; }
            public float BuffEndRdx { get; set; }

            public void Init(bool fullReset = true)
            {
                Def = new float[Enum.GetValues(Enums.eDamage.None.GetType()).Length];
                Res = new float[Enum.GetValues(Enums.eDamage.None.GetType()).Length];
                Mez = new float[Enum.GetValues(Enums.eMez.None.GetType()).Length];
                MezRes = new float[Enum.GetValues(Enums.eMez.None.GetType()).Length];
                DebuffRes = new float[Enum.GetValues(Enums.eEffectType.None.GetType()).Length];
                Elusivity = new float[Enum.GetValues(Enums.eDamage.None.GetType()).Length];
                if (!fullReset) return;
                HPRegen = 0.0f;
                HPMax = 0.0f;
                Absorb = 0.0f;
                EndRec = 0.0f;
                EndUse = 0.0f;
                EndMax = 0.0f;
                RunSpd = 0.0f;
                JumpSpd = 0.0f;
                FlySpd = 0.0f;
                JumpHeight = 0.0f;
                StealthPvE = 0.0f;
                StealthPvP = 0.0f;
                ThreatLevel = 0.0f;
                Perception = 0.0f;
                BuffHaste = 0.0f;
                BuffAcc = 0.0f;
                BuffToHit = 0.0f;
                BuffDam = 0.0f;
                BuffEndRdx = 0.0f;
            }

            public void Assign(TotalStatistics iSt)
            {
                Def = (float[])iSt.Def.Clone();
                Res = (float[])iSt.Res.Clone();
                Mez = (float[])iSt.Mez.Clone();
                MezRes = (float[])iSt.MezRes.Clone();
                DebuffRes = (float[])iSt.DebuffRes.Clone();
                Elusivity = iSt.Elusivity;
                HPRegen = iSt.HPRegen;
                HPMax = iSt.HPMax;
                Absorb = iSt.Absorb;
                EndRec = iSt.EndRec;
                EndUse = iSt.EndUse;
                EndMax = iSt.EndMax;
                RunSpd = iSt.RunSpd;
                JumpSpd = iSt.JumpSpd;
                FlySpd = iSt.FlySpd;
                JumpHeight = iSt.JumpHeight;
                StealthPvE = iSt.StealthPvE;
                StealthPvP = iSt.StealthPvP;
                ThreatLevel = iSt.ThreatLevel;
                Perception = iSt.Perception;
                BuffHaste = iSt.BuffHaste;
                BuffAcc = iSt.BuffAcc;
                BuffToHit = iSt.BuffToHit;
                BuffDam = iSt.BuffDam;
                BuffEndRdx = iSt.BuffEndRdx;
            }
        }
    }
}