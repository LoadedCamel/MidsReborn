using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using FastDeepCloner;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core.Base.Data_Classes
{
    public class Character
    {
        private const string FastSnipePlannerPowerFullName = "Inherent.Inherent.Fast_Snipe";
        private const string ExperiencedMarksmanBonusPowerFullName = "Set_Bonus.Global_Bonus.Experienced_Marksman";
        private Archetype? _archetype;
        private bool? _completeCache;
        private bool _experiencedMarksmanForcedFastSnipe;
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
            Builds = [new Build(this, DatabaseAPI.Database.Levels)];
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

        private Build?[] Builds { get; }

        public Build? CurrentBuild => Builds.Length > 0 ? Builds[0] : null;

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
                if (_completeCache.HasValue) return _completeCache.Value;

                int slotsLeft = 0;
                int powersLeft = 0;

                if (CurrentBuild is not null)
                {
                    slotsLeft = Build.TotalSlotsAvailable - CurrentBuild.SlotsPlaced;
                    powersLeft = CurrentBuild.LastPower + 1 - CurrentBuild.PowersPlaced;
                }

                _completeCache = slotsLeft < 1 && powersLeft < 1;
                return _completeCache.Value;
            }
            set => _completeCache = value ? true : null;
        }

        public int ActiveComboLevel { get; private set; }

        public int ActivePerfectionLevel { get; private set; }

        private readonly HashSet<PlannerMode> _activePlannerModes = [];
        private readonly Dictionary<string, int> _plannerStateStacks = new(StringComparer.OrdinalIgnoreCase);
        public IReadOnlyCollection<PlannerMode> ActivePlannerModes => _activePlannerModes;
        public IReadOnlyDictionary<string, int> PlannerStateStacks => _plannerStateStacks;

        public int PerfectionOfBodyLevel => IsStalker || PerfectionType == "body" ? ActivePerfectionLevel : 0;

        public int PerfectionOfMindLevel => !IsStalker && PerfectionType == "mind" ? ActivePerfectionLevel : 0;

        public int PerfectionOfSoulLevel => !IsStalker && PerfectionType == "soul" ? ActivePerfectionLevel : 0;

        public string? PerfectionType { get; private set; }

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

        public bool Insight { get; private set; }

        public bool Exhausted { get; private set; }

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
                // Normal/Respec: rely on totals (not the level schedule).
                if (MidsContext.Config.BuildMode is Enums.dmModes.Normal or Enums.dmModes.Respec)
                {
                    if (Build.TotalSlotsAvailable - CurrentBuild.SlotsPlaced > 0 &&
                        MidsContext.Config.BuildOption != Enums.dmItem.Power)
                        return true;
                }
                else
                {
                    // Level-gated slotting: respect per-level schedule.
                    if (Level > -1 && Level < DatabaseAPI.Database.Levels.Length)
                    {
                        // A) normal slot grants at this level
                        if (DatabaseAPI.Database.Levels[Level].LevelType() == Enums.dmItem.Slot &&
                            SlotsRemaining > 0)
                            return true;

                        // B) inherent slotting (Health/Stamina), if enabled
                        if (SlotsRemaining > 0 && DatabaseAPI.ServerData.EnableInherentSlotting)
                        {
                            if (Level == DatabaseAPI.ServerData.HealthSlot1Level) return true;
                            if (Level == DatabaseAPI.ServerData.HealthSlot2Level) return true;
                            if (Level == DatabaseAPI.ServerData.StaminaSlot1Level) return true;
                            if (Level == DatabaseAPI.ServerData.StaminaSlot2Level) return true;
                        }
                    }
                }

                return false;
            }
        }

        private static bool AtNameEquals(Archetype? at, string name) =>
            at is not null &&
            !string.IsNullOrEmpty(at.DisplayName) &&
            string.Equals(at.DisplayName, name, StringComparison.OrdinalIgnoreCase);

        //public bool IsHero => Alignment is Enums.Alignment.Hero or Enums.Alignment.Vigilante;
        public bool IsVillain => Alignment is Enums.Alignment.Rogue or Enums.Alignment.Villain;
        public bool IsPraetorian => Alignment is Enums.Alignment.Loyalist or Enums.Alignment.Resistance;

        public bool IsBlaster => AtNameEquals(Archetype, "Blaster");
        public bool IsController => AtNameEquals(Archetype, "Controller");
        public bool IsDefender => AtNameEquals(Archetype, "Defender");
        public bool IsScrapper => AtNameEquals(Archetype, "Scrapper");
        public bool IsTanker => AtNameEquals(Archetype, "Tank") || AtNameEquals(Archetype, "Tanker");
        public bool IsBrute => AtNameEquals(Archetype, "Brute");
        public bool IsCorruptor => AtNameEquals(Archetype, "Corruptor");
        public bool IsDominator => AtNameEquals(Archetype, "Dominator");
        public bool IsMastermind => AtNameEquals(Archetype, "Mastermind");
        public bool IsStalker => AtNameEquals(Archetype, "Stalker");

        public bool IsKheldian => Archetype?.ClassType == Enums.eClassType.HeroEpic;
        public bool IsArachnos => Archetype?.ClassType == Enums.eClassType.VillainEpic;

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
            var requiredSecondaryStarterPower = GetRequiredSecondaryStarterPower();
            if ((powersPlaced == 1) && (requiredSecondaryStarterPower != null) &&
                CurrentBuild.PowerUsed(requiredSecondaryStarterPower))
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

        public bool PoolTaken(int poolId)
        {
            return Powersets[poolId] != null && poolId >= 3 && poolId <= 7 && PoolLocked[poolId - 3];
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
            Powersets = sets
                .Select(DatabaseAPI.GetPowersetByFullname)
                .Select(powerSet => powerSet ?? new Powerset())
                .ToArray();
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
                    Powersets[0] = GetSafePowerset(Archetype, Enums.ePowerSetType.Primary, Powersets[0]);
                var flag3 = Powersets[1] != null && Powersets[1].nArchetype == Archetype.Idx;
                if (!flag3)
                    Powersets[1] = GetSafePowerset(Archetype, Enums.ePowerSetType.Secondary, Powersets[1]);
            }
            else
            {
                Powersets[0] = GetSafePowerset(Archetype, Enums.ePowerSetType.Primary, Powersets[0]);
                Powersets[1] = GetSafePowerset(Archetype, Enums.ePowerSetType.Secondary, Powersets[1]);
            }

            var powersetIndexes1 = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Pool);
            var index = 0;
            Powersets[3] = GetSafePowerset(powersetIndexes1, index, Powersets[3]);
            if (powersetIndexes1.Length - 1 > index)
                ++index;
            Powersets[4] = GetSafePowerset(powersetIndexes1, index, Powersets[4]);
            if (powersetIndexes1.Length - 1 > index)
                ++index;
            Powersets[5] = GetSafePowerset(powersetIndexes1, index, Powersets[5]);
            if (powersetIndexes1.Length - 1 > index)
                ++index;
            Powersets[6] = GetSafePowerset(powersetIndexes1, index, Powersets[6]);
            var powersetIndexes2 = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Ancillary);
            Powersets[7] = powersetIndexes2.Length <= 0 ? null : powersetIndexes2[0];
            ModifyEffects = new Dictionary<string, float>();
            PoolLocked = new bool[5];
            NewBuild();
            Locked = false;
            LevelCache = -1;
        }

        private static IPowerset? GetSafePowerset(Archetype archetype, Enums.ePowerSetType setType, IPowerset? fallback)
        {
            return GetSafePowerset(DatabaseAPI.GetPowersetIndexes(archetype, setType), 0, fallback);
        }

        private static IPowerset? GetSafePowerset(IPowerset?[] powersets, int index, IPowerset? fallback)
        {
            if (powersets.Length == 0)
            {
                return fallback;
            }

            if (index >= 0 && index < powersets.Length)
            {
                return powersets[index];
            }

            if (fallback != null)
            {
                var match = powersets.FirstOrDefault(p => p?.nID == fallback.nID);
                if (match != null)
                {
                    return match;
                }
            }

            return powersets[0];
        }

        protected void NewBuild()
        {
            Builds[0] = new Build(this, DatabaseAPI.Database.Levels);
            AcceleratedActive = false;
            ActiveComboLevel = 0;
            ActivePerfectionLevel = 0;
            DelayedActive = false;
            DisintegrateActive = false;
            TargetDroneActive = false;
            FastModeActive = false;
            Insight = false;
            Exhausted = false;
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
            _activePlannerModes.Clear();
            _plannerStateStacks.Clear();
            _experiencedMarksmanForcedFastSnipe = false;
        }

        public void ClearInvalidInherentSlots()
        {
            ResetLevel();
            if (CurrentBuild?.Powers == null) return;

            // Walk by index so we can call the synchronous RemoveSlotFromPower(...)
            for (int p = 0; p < CurrentBuild.Powers.Count; p++)
            {
                var power = CurrentBuild.Powers[p];
                if (power?.Power == null) continue;

                int allowed = 0;
                switch (power.Power.FullName)
                {
                    case "Inherent.Fitness.Health":
                        if (Level >= DatabaseAPI.ServerData.HealthSlot1Level) allowed = 1;
                        if (Level >= DatabaseAPI.ServerData.HealthSlot2Level) allowed = 2;
                        break;

                    case "Inherent.Fitness.Stamina":
                        if (Level >= DatabaseAPI.ServerData.StaminaSlot1Level) allowed = 1;
                        if (Level >= DatabaseAPI.ServerData.StaminaSlot2Level) allowed = 2;
                        break;

                    default:
                        continue;
                }

                // Trim down to the allowed count by removing highest-index inherent slots (>= 1)
                while (power.InherentSlotsUsed > allowed)
                {
                    int idxToRemove = -1;

                    // Never remove slot 0; scan from the end so subsequent indices remain valid
                    for (int i = power.Slots.Length - 1; i >= 1; i--)
                    {
                        if (power.Slots[i].IsInherent)
                        {
                            idxToRemove = i;
                            break;
                        }
                    }

                    if (idxToRemove < 0)
                    {
                        // No inherent slot found to remove; keep counters consistent and break.
                        power.InherentSlotsUsed = Math.Max(allowed, power.InherentSlotsUsed);
                        break;
                    }

                    // Remove synchronously and update the counter
                    CurrentBuild.RemoveSlotFromPower(p, idxToRemove);
                    power.InherentSlotsUsed = Math.Max(0, power.InherentSlotsUsed - 1);
                }
            }
        }

        internal bool IsPlannerToggleLocked(IPower? power)
        {
            return power != null &&
                   power.FullName.Equals(FastSnipePlannerPowerFullName, StringComparison.OrdinalIgnoreCase) &&
                   HasForcedFastSnipe();
        }

        private bool HasForcedFastSnipe()
        {
            return CurrentBuild?.OwnsPowerByFullName(ExperiencedMarksmanBonusPowerFullName) == true;
        }

        private void ApplyForcedPlannerStateToggles()
        {
            if (CurrentBuild?.Powers == null)
            {
                return;
            }

            var fastSnipeEntry = CurrentBuild.Powers.FirstOrDefault(powerEntry =>
                powerEntry?.Power != null &&
                powerEntry.Power.FullName.Equals(FastSnipePlannerPowerFullName, StringComparison.OrdinalIgnoreCase));
            if (fastSnipeEntry?.Power == null)
            {
                _experiencedMarksmanForcedFastSnipe = false;
                return;
            }

            var hasForcedFastSnipe = HasForcedFastSnipe();
            if (hasForcedFastSnipe)
            {
                fastSnipeEntry.StatInclude = true;
                fastSnipeEntry.Power.Active = true;
            }
            else if (_experiencedMarksmanForcedFastSnipe)
            {
                fastSnipeEntry.StatInclude = false;
                fastSnipeEntry.Power.Active = false;
            }

            _experiencedMarksmanForcedFastSnipe = hasForcedFastSnipe;
        }


        /// <summary>
        /// Call this function when a power is enabled/disabled, added, or removed, including when the archetype is changed.
        /// </summary>
        private void RefreshActiveSpecial()
        {
            ActiveComboLevel = 0;
            ActivePerfectionLevel = 0;
            AcceleratedActive = false;
            DelayedActive = false;
            DisintegrateActive = false;
            TargetDroneActive = false;
            FastModeActive = false;
            Insight = false;
            Exhausted = false;
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
            _activePlannerModes.Clear();
            _plannerStateStacks.Clear();
            InherentDisplayList = new List<InherentDisplayItem>();
            PEnhancementsList = new List<string>();
            if (CurrentBuild?.Powers == null) return;

            ApplyForcedPlannerStateToggles();
            AssassinationPlanner.Synchronize(CurrentBuild, MidsContext.Config?.CombatContextSettings.Assassination);
            OpportunityPlanner.Synchronize(CurrentBuild, MidsContext.Config?.CombatContextSettings.Opportunity);

            foreach (var power in CurrentBuild.Powers)
            {
                if (power?.Power == null)
                {
                    continue;
                }

                if (PowerEntry.ShouldForceAutoIncluded(power.Power))
                {
                    power.StatInclude = true;
                }

                power.Power.HasProcSlotted = power.HasProc();
                if (power.Chosen || !power.Chosen && CurrentBuild.PowerUsed(power.Power))
                {
                    power.Power.Taken = true;
                }

                power.Power.Active = power.StatInclude &&
                                    CurrentBuild.MeetsRequirement(power.Power, CurrentBuild.GetMaxLevel());

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
            }

            foreach (var power in CurrentBuild.Powers)
            {
                if (power?.Power == null || !power.Power.Active) continue;

                if (power.Power.VariableEnabled)
                {
                    SyncPlannerVariableTargets(power);
                    if (power.VariableValue > 0)
                    {
                        _plannerStateStacks[power.Power.FullName] = power.VariableValue;
                    }
                    if (power.Power.FullName.Equals(PlannerStateCatalog.PackMentalityMarker, StringComparison.OrdinalIgnoreCase))
                    {
                        PackMentality = true;
                        NotPackMentality = false;
                        ApplyPlannerMode(PlannerMode.PackMentality, true);
                    }
                }

                foreach (var effect in power.Power.Effects ?? [])
                {
                    ApplyPlannerModeEffect(effect);
                }

                if (power.Power.ShowStatToggle &&
                    (PlannerModeMapper.TryGetPlannerMode(power.Power.PowerName, out var plannerMode) ||
                     PlannerModeMapper.TryGetPlannerMode(power.Power.FullName?.Split('.').LastOrDefault(), out plannerMode)))
                {
                    ApplyPlannerMode(plannerMode, true);
                }
            }

            var impliedPlannerModes = CurrentBuild.Powers
                .Where(power => power?.Power is not null && power.Power.Active)
                .SelectMany(power =>
                    PlannerStateCatalog.TryGetImpliedPlannerModes(power!.Power.FullName, out var modes)
                        ? modes
                        : Array.Empty<PlannerMode>())
                .Distinct()
                .ToArray();

            foreach (var impliedMode in impliedPlannerModes)
            {
                ApplyPlannerMode(impliedMode, true);
            }

            SyncForcedPlannerStateValues();

            var inherentPowersList = CurrentBuild?.Powers
                .Where(p => p is { Chosen: false, Power: not null } && CurrentBuild.PowerUsed(p.Power)).Select(p => p.Power)
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

                InherentDisplayList = new List<InherentDisplayItem>(InherentDisplayList
                    .OrderBy(x => x.Priority)
                    .ThenBy(x => x.Power.Level)
                    .ThenBy(x => x.Power.DisplayName, StringComparer.OrdinalIgnoreCase));
            }

            if (CurrentBuild == null)
            {
                return;
            }

            foreach (var power in CurrentBuild.Powers.Where(power => power?.Power != null))
            {
                if (power.Chosen || !CurrentBuild.PowerUsed(power.Power))
                {
                    continue;
                }

                var displayItem = InherentDisplayList.FirstOrDefault(x => x.Power.FullName == power.Power.FullName);
                power.Power.DisplayLocation = InherentDisplayList.IndexOf(displayItem);
            }
        }

        protected void ReadMetadata(string buildText)
        {
            var tags = new List<string> { "comment", "enhobtained" };

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
            CurrentBuild?.GenerateSetBonusData();
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

            var psID = Powersets[(int)powersetType] == null ? -1 : Powersets[(int)powersetType].nID;

            if (powersetType == Enums.PowersetType.None)
                return false;
            if (DatabaseAPI.Database.Powersets[powerSetId].nIDMutexSets.Any(t => t == psID))
            {
                // Powerset combination is denied
                return true;
            }

            if (Powersets[(int)powersetType] == null)
                return false;

            return Powersets[(int)powersetType].nIDMutexSets
                .Where(t => Powersets[(int)powersetType] != null).Any(t => t == powerSetId);
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
                {
                    if (index2 == iPool || !(PoolLocked[index2 - 3] && index1.nID == Powersets[index2].nID))
                    {
                        available = true;
                    }
                }

                if (available)
                {
                    intList.Add(index1.nID);
                }
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
                {
                    return num1;
                }
            }

            return 0;
        }

        private IEnumerable<int> GetAvailablePools(int poolSlot)
        {
            // Collect IDs of pools that are locked in *other* slots.
            var lockedIds = new HashSet<int>(
                from slot in Enumerable.Range(3, 4)           // 3,4,5,6
                where slot != poolSlot && PoolLocked[slot - 3]
                select Powersets[slot].nID
            );

            // Any candidate whose ID is not locked elsewhere is available.
            return DatabaseAPI
                .GetPowersetIndexes(Archetype, Enums.ePowerSetType.Pool)
                .Where(candidate => !lockedIds.Contains(candidate.nID))
                .Select(candidate => candidate.nID);
        }

        public int PoolToDropDownIndex(int poolSlot, int powersetId)
        {
            int index = 0;
            foreach (int id in GetAvailablePools(poolSlot))
            {
                if (id == powersetId)
                    return index;
                index++;
            }
            return 0;
        }

        public static PopUp.PopupData PopEnhInfo(I9Slot iSlot, int iLevel = -1, PowerEntry? powerEntry = null)
        {
            var popupData1 = new PopUp.PopupData();
            var index1 = popupData1.Add();
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

                return popupData1;
            }

            var enhancement = DatabaseAPI.Database.Enhancements[iSlot.Enh];
            switch (enhancement.TypeID)
            {
                case Enums.eType.Normal:
                    popupData1.Sections[index1].Add(DatabaseAPI.GetEnhancementDisplayName(iSlot, includeFlavor: true), PopUp.Colors.Title, 1.25f);
                    break;
                case Enums.eType.SpecialO:
                    popupData1.Sections[index1].Add(enhancement.Name, PopUp.Colors.Title, 1.25f);
                    break;
                case Enums.eType.InventO:
                    popupData1.Sections[index1].Add($"Invention: {enhancement.Name}", PopUp.Colors.Title, 1.25f);
                    break;
                case Enums.eType.SetO:
                    var iColor = PopUp.Colors.Title;
                    if (DatabaseAPI.TryGetEnhancementResolvedRarity(iSlot.Enh, out var enhancementRarity))
                    {
                        iColor = GetPopupRarityColor(enhancementRarity);
                    }

                    popupData1.Sections[index1].Add(BuildSetEnhancementPopupTitle(enhancement), iColor, 1.25f);
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
                case Enums.eType.SetO when !DatabaseAPI.EnhIsNaturallyAttuned(iSlot.Enh):
                    popupData1.Sections[index1].Add($"Invention Level: {iSlot.IOLevel + 1}{iSlot.GetRelativeString(false)}", PopUp.Colors.Invention);
                    break;
            }

            if (iLevel > -1)
            {
                popupData1.Sections[index1].Add($"Slot placed at level: {iLevel + 1}", PopUp.Colors.Text);
            }

            var resolvedDescription = iSlot.GetResolvedEnhancementDescription();
            if (enhancement.Unique && !ContainsUniqueRestrictionText(resolvedDescription))
            {
                index1 = popupData1.Add();
                popupData1.Sections[index1].Add("This enhancement is Unique. No more than one enhancement of this type can be slotted by a character.", PopUp.Colors.Text, 0.9f);
            }

            if (!string.IsNullOrWhiteSpace(resolvedDescription))
            {
                popupData1.Sections[index1].Add(resolvedDescription, PopUp.Colors.Title);
            }

            var enhStringLong = iSlot.GetEnhancementStringLong();
            if (enhancement.UID.Contains("Assassins_Mark"))
            {
                enhStringLong = Regex.Replace(enhStringLong, @"(([\s]*)([0-9\.\%]+) RechargePower([0-9a-zA-Z\%\.\(\) ]+)[\r\n]*)+", "\r\n$2RechargePower(Stalker's Build Ups)\r\n");
            }

            if (!string.IsNullOrWhiteSpace(enhStringLong))
            {
                var index4 = popupData1.Add();
                var strArray3 = enhStringLong.Replace("\r\n", "\n").Split('\n');
                foreach (var s in strArray3.Where(line => !string.IsNullOrWhiteSpace(line)))
                {
                    var strArray2 = !enhancement.HasPowerEffect
                        ? BreakByBracket(s)
                        : [s, string.Empty];

                    popupData1.Sections[index4].Add(strArray2[0], Color.FromArgb(0, 255, 0), strArray2[1], Color.FromArgb(0, 255, 0), 0.9f);
                }
            }

            if (!MidsContext.Config.PopupRecipes)
            {
                if (enhancement.TypeID != Enums.eType.SetO)
                {
                    return popupData1;
                }

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
            else if (enhancement.TypeID is Enums.eType.SetO or Enums.eType.InventO)
            {
                popupData1.Add(PopRecipeInfo(enhancement.RecipeIDX, iSlot.IOLevel, iSlot.RelativeLevel));
            }

            return popupData1;
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

        private static int GetFirstAvailablePowerLevel(Build? currentBuild, int iLevel = 0)
        {
            if (currentBuild is null) return -1;
            if (iLevel < 0) iLevel = 0;

            int ceiling = Math.Min(currentBuild.Powers.Count, currentBuild.LastPower + 1);
            for (int i = 0; i < ceiling; i++)
            {
                var p = currentBuild.Powers[i];
                if (p is { NIDPowerset: < 0 } && p.Level >= iLevel)
                    return p.Level;
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
            if (ShouldKeepEffectVectorInline(iString) || !ShouldSplitByBracket(iString))
            {
                return strArray1;
            }

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

        private static string BuildSetEnhancementPopupTitle(IEnhancement enhancement)
        {
            var setName = DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].DisplayName;
            if (enhancement.Name.StartsWith($"{setName}:", StringComparison.OrdinalIgnoreCase))
            {
                return enhancement.Name;
            }

            return $"{setName}: {enhancement.Name}";
        }

        private static Color GetPopupRarityColor(Recipe.RecipeRarity rarity)
        {
            return rarity switch
            {
                Recipe.RecipeRarity.Common => PopUp.Colors.Common,
                Recipe.RecipeRarity.Uncommon => PopUp.Colors.Uncommon,
                Recipe.RecipeRarity.Rare => PopUp.Colors.Rare,
                Recipe.RecipeRarity.UltraRare => PopUp.Colors.UltraRare,
                _ => PopUp.Colors.Title
            };
        }

        private static bool ContainsUniqueRestrictionText(string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return false;
            }

            return description.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) ||
                   description.Contains("No more than 1 enhancement of this type", StringComparison.OrdinalIgnoreCase) ||
                   description.Contains("No more than one enhancement of this type", StringComparison.OrdinalIgnoreCase);
        }

        private void ApplyPlannerModeEffect(IEffect effect)
        {
            if (effect.EffectType is not (Enums.eEffectType.SetMode or Enums.eEffectType.UnsetMode))
            {
                return;
            }

            if (!PlannerModeMapper.TryGetPlannerMode(effect.ModeName, out var mode) &&
                !PlannerModeMapper.TryGetPlannerMode(effect.ModeFlag.ToString(), out mode))
            {
                return;
            }

            ApplyPlannerMode(mode, effect.EffectType == Enums.eEffectType.SetMode);
        }

        private void ApplyPlannerMode(PlannerMode mode, bool enabled)
        {
            if (enabled)
            {
                ClearExclusivePlannerModeFamily(mode);
            }

            if (enabled)
            {
                _activePlannerModes.Add(mode);
            }
            else
            {
                _activePlannerModes.Remove(mode);
            }

            switch (mode)
            {
                case PlannerMode.FastSnipe:
                    FastSnipe = enabled;
                    NotFastSnipe = !enabled;
                    break;
                case PlannerMode.Containment:
                    Containment = enabled;
                    break;
                case PlannerMode.Domination:
                    if (enabled)
                    {
                        _activePlannerModes.Add(PlannerMode.DominationActive);
                    }
                    else
                    {
                        _activePlannerModes.Remove(PlannerMode.DominationActive);
                    }
                    Domination = enabled;
                    break;
                case PlannerMode.DominationActive:
                    if (enabled)
                    {
                        _activePlannerModes.Add(PlannerMode.Domination);
                    }
                    else
                    {
                        _activePlannerModes.Remove(PlannerMode.Domination);
                    }
                    Domination = enabled;
                    break;
                case PlannerMode.Scourge:
                    Scourge = enabled;
                    break;
                case PlannerMode.CriticalHit:
                    CriticalHits = enabled;
                    break;
                case PlannerMode.Assassination:
                    Assassination = enabled;
                    break;
                case PlannerMode.StalkerHidden:
                    break;
                case PlannerMode.Defiance:
                    Defiance = enabled;
                    break;
                case PlannerMode.DefensiveAdaptation:
                    DefensiveAdaptation = enabled;
                    NotDefensiveAdaptation = !enabled;
                    NotDefensiveNorOffensiveAdaptation = !enabled && !OffensiveAdaptation;
                    break;
                case PlannerMode.EfficientAdaptation:
                    EfficientAdaptation = enabled;
                    break;
                case PlannerMode.OffensiveAdaptation:
                    OffensiveAdaptation = enabled;
                    NotDefensiveNorOffensiveAdaptation = !enabled && !DefensiveAdaptation;
                    break;
                case PlannerMode.ComboLevel1:
                    ActiveComboLevel = enabled ? 1 : ActiveComboLevel == 1 ? 0 : ActiveComboLevel;
                    break;
                case PlannerMode.ComboLevel2:
                    ActiveComboLevel = enabled ? 2 : ActiveComboLevel == 2 ? 0 : ActiveComboLevel;
                    break;
                case PlannerMode.ComboLevel3:
                    ActiveComboLevel = enabled ? 3 : ActiveComboLevel == 3 ? 0 : ActiveComboLevel;
                    break;
                case PlannerMode.FastMode:
                    FastModeActive = enabled;
                    break;
                case PlannerMode.Insight:
                    Insight = enabled;
                    break;
                case PlannerMode.Exhausted:
                    Exhausted = enabled;
                    break;
                case PlannerMode.PerfectionLevel1:
                    ActivePerfectionLevel = enabled ? 1 : ActivePerfectionLevel == 1 ? 0 : ActivePerfectionLevel;
                    break;
                case PlannerMode.PerfectionLevel2:
                    ActivePerfectionLevel = enabled ? 2 : ActivePerfectionLevel == 2 ? 0 : ActivePerfectionLevel;
                    break;
                case PlannerMode.PerfectionLevel3:
                    ActivePerfectionLevel = enabled ? 3 : ActivePerfectionLevel == 3 ? 0 : ActivePerfectionLevel;
                    break;
                case PlannerMode.PerfectionOfBody:
                    PerfectionType = enabled ? "body" : PerfectionType == "body" ? string.Empty : PerfectionType;
                    break;
                case PlannerMode.PerfectionOfBody1:
                    PerfectionType = enabled ? "body" : PerfectionType == "body" && ActivePerfectionLevel == 1 ? string.Empty : PerfectionType;
                    ActivePerfectionLevel = enabled ? 1 : ActivePerfectionLevel == 1 ? 0 : ActivePerfectionLevel;
                    break;
                case PlannerMode.PerfectionOfBody2:
                    PerfectionType = enabled ? "body" : PerfectionType == "body" && ActivePerfectionLevel == 2 ? string.Empty : PerfectionType;
                    ActivePerfectionLevel = enabled ? 2 : ActivePerfectionLevel == 2 ? 0 : ActivePerfectionLevel;
                    break;
                case PlannerMode.PerfectionOfBody3:
                    PerfectionType = enabled ? "body" : PerfectionType == "body" && ActivePerfectionLevel == 3 ? string.Empty : PerfectionType;
                    ActivePerfectionLevel = enabled ? 3 : ActivePerfectionLevel == 3 ? 0 : ActivePerfectionLevel;
                    break;
                case PlannerMode.PerfectionOfMind:
                    PerfectionType = enabled ? "mind" : PerfectionType == "mind" ? string.Empty : PerfectionType;
                    break;
                case PlannerMode.PerfectionOfMind1:
                    PerfectionType = enabled ? "mind" : PerfectionType == "mind" && ActivePerfectionLevel == 1 ? string.Empty : PerfectionType;
                    ActivePerfectionLevel = enabled ? 1 : ActivePerfectionLevel == 1 ? 0 : ActivePerfectionLevel;
                    break;
                case PlannerMode.PerfectionOfMind2:
                    PerfectionType = enabled ? "mind" : PerfectionType == "mind" && ActivePerfectionLevel == 2 ? string.Empty : PerfectionType;
                    ActivePerfectionLevel = enabled ? 2 : ActivePerfectionLevel == 2 ? 0 : ActivePerfectionLevel;
                    break;
                case PlannerMode.PerfectionOfMind3:
                    PerfectionType = enabled ? "mind" : PerfectionType == "mind" && ActivePerfectionLevel == 3 ? string.Empty : PerfectionType;
                    ActivePerfectionLevel = enabled ? 3 : ActivePerfectionLevel == 3 ? 0 : ActivePerfectionLevel;
                    break;
                case PlannerMode.PerfectionOfSoul:
                    PerfectionType = enabled ? "soul" : PerfectionType == "soul" ? string.Empty : PerfectionType;
                    break;
                case PlannerMode.PerfectionOfSoul1:
                    PerfectionType = enabled ? "soul" : PerfectionType == "soul" && ActivePerfectionLevel == 1 ? string.Empty : PerfectionType;
                    ActivePerfectionLevel = enabled ? 1 : ActivePerfectionLevel == 1 ? 0 : ActivePerfectionLevel;
                    break;
                case PlannerMode.PerfectionOfSoul2:
                    PerfectionType = enabled ? "soul" : PerfectionType == "soul" && ActivePerfectionLevel == 2 ? string.Empty : PerfectionType;
                    ActivePerfectionLevel = enabled ? 2 : ActivePerfectionLevel == 2 ? 0 : ActivePerfectionLevel;
                    break;
                case PlannerMode.PerfectionOfSoul3:
                    PerfectionType = enabled ? "soul" : PerfectionType == "soul" && ActivePerfectionLevel == 3 ? string.Empty : PerfectionType;
                    ActivePerfectionLevel = enabled ? 3 : ActivePerfectionLevel == 3 ? 0 : ActivePerfectionLevel;
                    break;
                case PlannerMode.PackMentality:
                    PackMentality = enabled;
                    NotPackMentality = !enabled;
                    break;
            }
        }

        private void SyncForcedPlannerStateValues()
        {
            if (CurrentBuild?.Powers == null)
            {
                return;
            }

            var meterEntry = CurrentBuild.Powers.FirstOrDefault(entry =>
                entry?.Power?.FullName.Equals(PlannerStateCatalog.DominationMeterPowerFullName, StringComparison.OrdinalIgnoreCase) == true);
            if (meterEntry?.Power == null)
            {
                return;
            }

            if (!_activePlannerModes.Contains(PlannerMode.DominationActive))
            {
                meterEntry.VariableValue = meterEntry.Power.VariableStart;
                if (meterEntry.VariableValue > 0)
                {
                    _plannerStateStacks[meterEntry.Power.FullName] = meterEntry.VariableValue;
                }
                else
                {
                    _plannerStateStacks.Remove(meterEntry.Power.FullName);
                }

                return;
            }

            var forcedMeterValue = meterEntry.Power.VariableMax > 0
                ? meterEntry.Power.VariableMax
                : 100;
            meterEntry.StatInclude = true;
            meterEntry.VariableValue = Math.Max(meterEntry.VariableValue, forcedMeterValue);
            meterEntry.Power.Active = meterEntry.StatInclude &&
                                      CurrentBuild.MeetsRequirement(meterEntry.Power, CurrentBuild.GetMaxLevel());
            _plannerStateStacks[meterEntry.Power.FullName] = meterEntry.VariableValue;
        }

        private void SyncPlannerVariableTargets(PowerEntry powerEntry)
        {
            if (CurrentBuild?.Powers == null ||
                powerEntry?.Power == null ||
                !PlannerStateCatalog.TryGetVariableSyncTargets(powerEntry.Power.FullName, out var syncTargets))
            {
                return;
            }

            foreach (var targetFullName in syncTargets)
            {
                var targetEntry = CurrentBuild.Powers.FirstOrDefault(entry =>
                    entry?.Power?.FullName.Equals(targetFullName, StringComparison.OrdinalIgnoreCase) == true);

                if (targetEntry?.Power == null)
                {
                    continue;
                }

                targetEntry.VariableValue = powerEntry.VariableValue;
                targetEntry.StatInclude = powerEntry.VariableValue > 0 || targetEntry.Power.AlwaysToggle;
                _plannerStateStacks[targetEntry.Power.FullName] = powerEntry.VariableValue;
            }

            if (powerEntry.Power.FullName.Equals(PlannerStateCatalog.AssassinationPowerFullName, StringComparison.OrdinalIgnoreCase) ||
                powerEntry.Power.FullName.Equals(PlannerStateCatalog.AssassinsFocusMarker, StringComparison.OrdinalIgnoreCase))
            {
                var hasFocusStacks = powerEntry.VariableValue > 0;
                if (hasFocusStacks)
                {
                    _activePlannerModes.Add(PlannerMode.Assassination);
                    _plannerStateStacks[PlannerStateCatalog.AssassinationPowerFullName] = powerEntry.VariableValue;
                    _plannerStateStacks[PlannerStateCatalog.AssassinsFocusMarker] = powerEntry.VariableValue;
                }
                else
                {
                    _activePlannerModes.Remove(PlannerMode.Assassination);
                    _plannerStateStacks.Remove(PlannerStateCatalog.AssassinationPowerFullName);
                    _plannerStateStacks.Remove(PlannerStateCatalog.AssassinsFocusMarker);
                }

                Assassination = hasFocusStacks;
            }
        }

        private void ClearExclusivePlannerModeFamily(PlannerMode activeMode)
        {
            if (!PlannerStateCatalog.TryGetExclusiveModeFamily(activeMode, out var familyModes))
            {
                return;
            }

            foreach (var siblingMode in familyModes)
            {
                if (siblingMode != activeMode)
                {
                    _activePlannerModes.Remove(siblingMode);
                }
            }
        }

        private static bool ShouldKeepEffectVectorInline(string value)
        {
            return value.Contains("DamageBuff (", StringComparison.Ordinal) ||
                   value.Contains("Damage Buff (", StringComparison.Ordinal) ||
                   value.Contains("Damage (", StringComparison.Ordinal);
        }

        private static bool ShouldSplitByBracket(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            if (!Regex.IsMatch(value, @"^[^(]+\([^()]+\)$"))
            {
                return false;
            }

            return value.Contains(" enhancement (Sched.", StringComparison.Ordinal) ||
                   Regex.IsMatch(value, @"^[^:]+ \((?:Mag|PPM|Chance|[\d\.]+%|Sched\.)", RegexOptions.IgnoreCase);
        }

        private static PopUp.Section? PopSetBonusListing(int sIdx, PowerEntry power)
        {
            var section1 = new PopUp.Section();
            section1.Add("Set Bonus:", PopUp.Colors.Title);
            var usedEnhancements = new List<int>();
            if (power != null)
            {
                for (var index = 0; index < power.Slots.Length; index++)
                {
                    if (power.Slots[index].Enhancement.Enh > -1 &&
                        DatabaseAPI.Database.Enhancements[power.Slots[index].Enhancement.Enh].nIDSet == sIdx)
                    {
                        usedEnhancements.Add(power.Slots[index].Enhancement.Enh);
                    }
                }
            }

            if (sIdx < 0 || sIdx > DatabaseAPI.Database.EnhancementSets.Count - 1)
            {
                return section1;
            }

            var enhancementSet = DatabaseAPI.Database.EnhancementSets[sIdx];
            var usedPieceCount = DatabaseAPI.CountDistinctVisibleSetPieces(sIdx, usedEnhancements);
            for (var index = 0; index < enhancementSet.Bonus.Length; index++)
            {
                var pvMode = enhancementSet.GetEffectiveBonusPvMode(index, false);
                var effectStrings = enhancementSet.GetPopupEffectStrings(index, false, true);
                if (effectStrings.Count == 0)
                {
                    continue;
                }

                var popupColor = usedPieceCount >= enhancementSet.Bonus[index].Slotted & enhancementSet.BonusAppliesInContext(index, false, MidsContext.Config.Inc.DisablePvE)
                    ? PopUp.Colors.Effect
                    : power == null
                        ? PopUp.Colors.Effect
                        : PopUp.Colors.Disabled;

                foreach (var effectStringRaw in effectStrings)
                {
                    var effectString = effectStringRaw;
                    if (pvMode == Enums.ePvX.PvP)
                    {
                        effectString += " (PVP)";
                    }

                    section1.Add($"({enhancementSet.Bonus[index].Slotted}) {effectString}", popupColor, 0.9f);
                }
            }

            var projection = DatabaseAPI.GetEnhancementSetProjection(sIdx);
            var usedPieceIndexes = new HashSet<int>();
            foreach (var enhancementId in usedEnhancements)
            {
                if (DatabaseAPI.TryGetSetPieceIndexForEnhancement(enhancementId, out _, out var pieceIndex))
                {
                    usedPieceIndexes.Add(pieceIndex);
                }
            }

            for (var pieceIndex = 0; pieceIndex < projection.VisiblePieces.Count; pieceIndex++)
            {
                var rawMemberPosition = DatabaseAPI.GetSpecialRawMemberPositionForSetPiece(sIdx, pieceIndex);
                if (rawMemberPosition < 0)
                {
                    continue;
                }

                var checkStatus = false;
                List<Power> specialPowers = null;
                if (enhancementSet.SpecialBonus.Length > rawMemberPosition)
                {
                    specialPowers = enhancementSet.GetEnhancementSetLinkedPowers(rawMemberPosition, true);
                }

                if (specialPowers is { Count: 1 })
                {
                    if (specialPowers[0].FullName.Contains("Skin") || specialPowers[0].FullName.Contains("Aegis")) checkStatus = true;
                }

                var effectStrings = enhancementSet.GetPopupEffectStrings(rawMemberPosition, true, true, checkStatus);
                if (effectStrings.Count == 0)
                {
                    continue;
                }

                var flag = power != null && usedPieceIndexes.Contains(pieceIndex);

                var popupColor = flag || power == null ? PopUp.Colors.Effect : PopUp.Colors.Disabled;
                foreach (var effectString in effectStrings)
                {
                    section1.Add($"(Enh) {effectString}", popupColor, 0.9f);
                }
            }

            return section1;
        }

        private static void GetSalvageCostOuter(ref Dictionary<Enums.RewardCurrency, int> costList, Salvage s, int amount)
        {
            var sCost = clsRewardCurrency.GetSalvageCost(s, MidsContext.Config.PreferredCurrency, amount);
            if (sCost != null)
            {
                costList[MidsContext.Config.PreferredCurrency] += (int)sCost;
            }
            else
            {
                var sCost2 = clsRewardCurrency.GetSalvageCost(s, Enums.RewardCurrency.RewardMerit, amount);
                if (sCost2 != null)
                {
                    costList[Enums.RewardCurrency.RewardMerit] += (int)sCost2;
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

            for (var i = 0; i < recipe.Item.Length; i++)
            {
                if (recipe.Item[i].Level > lvlLbound) lvlLbound = recipe.Item[i].Level;
                if (recipe.Item[i].Level < lvlUbound) lvlUbound = recipe.Item[i].Level;
                if (recipe.Item[i].Level == iLevel) index1 = i;
            }

            // If the requested level was not found, fall back to the closest entry
            if (index1 < 0)
            {
                for (var i = 0; i < recipe.Item.Length; i++)
                {
                    if (recipe.Item[i].Level == iLevel) { index1 = i; break; }
                }
            }

            if (index1 < 0) return section1;

            var recipeEntry = recipe.Item[index1];

            // FIX: bitwise '&' -> logical '&&'
            if (recipe.EnhIdx > -1 && !recipe.IsGeneric && !recipe.InternalName.StartsWith("G_", StringComparison.Ordinal))
                section1.Add($"Recipe - {DatabaseAPI.Database.Enhancements[recipe.EnhIdx].LongName}", PopUp.Colors.Title);
            else
                section1.Add("Materials:", PopUp.Colors.Title);

            if (recipeEntry.BuyCost > 0)
                section1.Add("Buy Cost:", PopUp.Colors.Invention, $"{recipeEntry.BuyCost:###,###,##0}", PopUp.Colors.Invention, 0.9f, FontStyle.Bold, 1);

            if (recipeEntry.CraftCost > 0)
                section1.Add("Craft Cost:", PopUp.Colors.Invention, $"{recipeEntry.CraftCost:###,###,##0}", PopUp.Colors.Invention, 0.9f, FontStyle.Bold, 1);

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
            if (!(subRecipesCost.Count > 0 & subCostTotal > 0))
            {
                return section1;
            }

            section1.Add("", PopUp.Colors.Title);
            section1.Add("Salvage detailed cost:", PopUp.Colors.Title);
            foreach (var c in subRecipesCost)
            {
                if (c.Value <= 0) continue;
                var cAmt = c.Key == Enums.RewardCurrency.Influence
                    ? $"{c.Value:###,###,##0}"
                    : $"{c.Value}";
                section1.Add(clsRewardCurrency.GetCurrencyName(c.Key),
                    clsRewardCurrency.GetCurrencyRarityColor(c.Key), cAmt, PopUp.Colors.Title,
                    0.9f, FontStyle.Bold, 1);
            }

            return section1;
        }

        public static PopUp.PopupData PopSetInfo(int sIdx, PowerEntry powerEntry = null)
        {
            if (sIdx < 0)
            {
                return new PopUp.PopupData();
            }

            var enhancementSet = DatabaseAPI.Database.EnhancementSets[sIdx];
            var iColor = DatabaseAPI.TryGetEnhancementSetResolvedRarity(sIdx, out var setRarity)
                ? GetPopupRarityColor(setRarity)
                : PopUp.Colors.Title;

            var popupData1 = new PopUp.PopupData();
            var index1 = popupData1.Add();
            popupData1.Sections[index1].Add(enhancementSet.DisplayName, iColor, 1.25f);
            popupData1.Sections[index1].Add($"Set Type: {DatabaseAPI.GetSetTypeByIndex(enhancementSet.SetType).Name}", PopUp.Colors.Invention);
            var lvlRange = enhancementSet.LevelMin != enhancementSet.LevelMax ? $"{enhancementSet.LevelMin + 1} to {enhancementSet.LevelMax + 1}" : $"{enhancementSet.LevelMin + 1}";

            popupData1.Sections[index1].Add($"Level Range: {lvlRange}", PopUp.Colors.Text);
            popupData1.Add(PopSetEnhList(sIdx, powerEntry));
            popupData1.Add(PopSetBonusListing(sIdx, powerEntry));

            return popupData1;
        }

        private static PopUp.Section? PopSetEnhList(int sIdx, PowerEntry? powerEntry)
        {
            if (sIdx < 0 || sIdx >= DatabaseAPI.Database.EnhancementSets.Count)
                return new PopUp.Section();

            var enhancementSet = DatabaseAPI.Database.EnhancementSets[sIdx];
            var setEnhUsed = new HashSet<int>();
            var setEnhObtained = new HashSet<int>();

            if (powerEntry != null)
            {
                foreach (var slot in powerEntry.Slots)
                {
                    if (slot.Enhancement.Enh < 0) continue;
                    if (DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].nIDSet != sIdx) continue;

                    if (DatabaseAPI.TryGetSetPieceIndexForEnhancement(slot.Enhancement.Enh, out _, out var pieceIndex))
                    {
                        setEnhUsed.Add(pieceIndex);
                        if (slot.Enhancement.Obtained)
                        {
                            setEnhObtained.Add(pieceIndex);
                        }
                    }
                }
            }

            var section1 = new PopUp.Section();
            var projection = DatabaseAPI.GetEnhancementSetProjection(sIdx);
            if (powerEntry != null)
                section1.Add($"Set: {enhancementSet.DisplayName} ({setEnhUsed.Count}/{projection.VisiblePieces.Count})", PopUp.Colors.Title);

            // FIX: explicit ToDictionary key/value selectors (was .ToDictionary() → runtime exception)
            var setEnhancements = projection.VisiblePieces;

            foreach (var piece in setEnhancements)
            {
                var enhUsed = setEnhUsed.Contains(piece.PieceIndex) || powerEntry == null;

                // FIX: bitwise '&' → logical '&&'
                var color = true switch
                {
                    _ when !MidsContext.EnhCheckMode && enhUsed => PopUp.Colors.Invention,
                    _ when MidsContext.EnhCheckMode && enhUsed && setEnhObtained.Contains(piece.PieceIndex) => PopUp.Colors.Invention,
                    _ when MidsContext.EnhCheckMode && enhUsed => PopUp.Colors.UltraRare,
                    _ => PopUp.Colors.Disabled
                };

                section1.Add(piece.DisplayLabel, color);
            }

            return section1;
        }

        public void PoolShuffle()
        {
            //var poolPowers = this.Powersets.Skip(2).Take(4).ToList();
            var poolOrder = new int[4];
            var poolIndex = new int[4];
            for (var i = 3; i < 7; i++)
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

            for (var i = 0; i < 4; i++)
            {
                int minO = byte.MaxValue;
                var minI = -1;
                for (var x = 0; x < poolOrder.Length; x++)
                {
                    if (minO <= poolOrder[x])
                    {
                        continue;
                    }

                    minO = poolOrder[x];
                    minI = x;
                }

                if (minI <= -1 || poolIndex[minI] <= -1)
                {
                    continue;
                }

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
            if (ps == null)
            {
                return false;
            }

            for (var index = 3; (Enums.PowersetType)index < pool; index++)
            {
                if (Powersets[index] != null && Powersets[index].nID == Powersets[(int)pool].nID)
                {
                    return false;
                }
            }

            return true;
        }

        private bool PowersetUsed(IPowerset? powerset)
        {
            if (powerset is null || CurrentBuild is null) return false;
            return CurrentBuild.Powers.Any(t => t is not null &&
                                                t.NIDPowerset == powerset.nID &&
                                                t.IDXPower > -1);
        }

        protected bool CanRemovePower(int index, bool allowSecondary, out string message)
        {
            message = string.Empty;
            var power = CurrentBuild.Powers[index];
            if (!power.Chosen)
            {
                message = "You can't remove inherent powers.\nIf the power is a Kheldian form power, you can remove it by removing the shapeshift power which grants it.";

                return false;
            }

            if (!(IsRequiredSecondaryStarterPower(power.NIDPower) & !allowSecondary))
            {
                return power.NIDPowerset >= 0;
            }

            if (CurrentBuild.PowersPlaced <= 1)
            {
                return true;
            }

            message = "The first power from your secondary set is non-optional and can't be removed.";

            return false;
        }

        protected IPowerset? ResolveSelectedSecondaryPowersetOrDefault()
        {
            if (Powersets[1] != null && Powersets[1].nID >= 0)
            {
                return Powersets[1];
            }

            var availableSecondaries = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Secondary);
            return availableSecondaries.FirstOrDefault(powerset => powerset != null);
        }

        protected IPower? GetRequiredSecondaryStarterPower(IPowerset? secondaryPowerset = null)
        {
            secondaryPowerset ??= ResolveSelectedSecondaryPowersetOrDefault();
            if (secondaryPowerset == null || secondaryPowerset.nID < 0)
            {
                return null;
            }

            var starterPowerIds = DatabaseAPI.NidPowersAtLevelBranch(0, secondaryPowerset.nID);
            return starterPowerIds.Length == 0
                ? null
                : DatabaseAPI.Database.Power[starterPowerIds[0]];
        }

        protected bool IsRequiredSecondaryStarterPower(int nIdPower, IPowerset? secondaryPowerset = null)
        {
            return nIdPower >= 0 &&
                   GetRequiredSecondaryStarterPower(secondaryPowerset)?.PowerIndex == nIdPower;
        }

        public void SwitchSets(IPowerset? newPowerset, IPowerset? oldPowerset)
        {
            if (newPowerset is null || oldPowerset is null) return;

            int oldTrunk = oldPowerset.nIDTrunkSet > -1 ? oldPowerset.nIDTrunkSet : oldPowerset.nID;
            int oldBranch = oldPowerset.nIDTrunkSet > -1 ? oldPowerset.nID : -1;

            int newTrunk = newPowerset.nIDTrunkSet > -1 ? newPowerset.nIDTrunkSet : newPowerset.nID;
            int newBranch = newPowerset.nIDTrunkSet > -1 ? newPowerset.nID : -1;

            for (var index4 = 0; index4 < Powersets.Length; index4++)
            {
                if (Powersets[index4] != null && Powersets[index4].nID == oldPowerset.nID)
                {
                    Powersets[index4] = newPowerset;
                }
            }

            foreach (var power in CurrentBuild.Powers)
            {
                if (power.NIDPowerset < 0)
                {
                    continue;
                }

                var powerIndex = CurrentBuild.Powers.IndexOf(power);
                var idxPower = power.IDXPower;
                if (power.NIDPowerset == oldTrunk)
                {
                    for (var index4 = 0;
                        index4 < DatabaseAPI.Database.Powersets[oldTrunk].Power.Length &&
                        DatabaseAPI.Database.Powersets[oldTrunk].Powers[index4].Level == 0;
                        ++index4)
                    {
                        --idxPower;
                    }

                    for (var index4 = 0;
                         index4 < DatabaseAPI.Database.Powersets[newTrunk].Power.Length &&
                         DatabaseAPI.Database.Powersets[newTrunk].Powers[index4].Level == 0;
                         ++index4)
                    {
                        ++idxPower;
                    }

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
                    {
                        --idxPower;
                    }

                    for (var index4 = 0;
                         index4 < DatabaseAPI.Database.Powersets[newTrunk].Power.Length &&
                         DatabaseAPI.Database.Powersets[newTrunk].Powers[index4].Level == 0;
                         ++index4)
                    {
                        ++idxPower;
                    }

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

                if (power.Power is not { Slottable: true })
                {
                    power.Slots = Array.Empty<SlotEntry>();
                }
                else if (power.Slots.Length == 0)
                {
                    power.Slots = new[]
                    {
                        new SlotEntry
                        {
                            Enhancement = new I9Slot(),
                            FlippedEnhancement = new I9Slot(),
                            Level = power.Level
                        }
                    };
                }
                else if (idxPower > -1)
                {
                    for (var index4 = 0; index4 < power.SlotCount; index4++)
                    {
                        if (!DatabaseAPI.ValidateEnhancementSlot(CurrentBuild, powerIndex, index4, power.Slots[index4].Enhancement.Enh).IsValid)
                        {
                            power.Slots[index4].Enhancement = new I9Slot();
                        }
                    }
                }
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
            public float BuffRange { get; set; }

            public void Init(bool fullReset = true)
            {
                Def = new float[Enum.GetValues<Enums.eDamage>().Length];
                Res = new float[Enum.GetValues<Enums.eDamage>().Length];
                Mez = new float[Enum.GetValues<Enums.eMez>().Length];
                MezRes = new float[Enum.GetValues<Enums.eMez>().Length];
                DebuffRes = new float[Enum.GetValues<Enums.eEffectType>().Length];
                Elusivity = new float[Enum.GetValues<Enums.eDamage>().Length];
                if (!fullReset) return;
                HPRegen = 0;
                HPMax = 0;
                Absorb = 0;
                EndRec = 0;
                EndUse = 0;
                EndMax = 0;
                RunSpd = 0;
                JumpSpd = 0;
                FlySpd = 0;
                JumpHeight = 0;
                StealthPvE = 0;
                StealthPvP = 0;
                ThreatLevel = 0;
                Perception = 0;
                BuffHaste = 0;
                BuffAcc = 0;
                BuffToHit = 0;
                BuffDam = 0;
                BuffEndRdx = 0;
                BuffRange = 0;
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
                BuffRange = iSt.BuffRange;
            }
        }
    }
}
