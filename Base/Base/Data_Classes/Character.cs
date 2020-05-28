
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Base.Display;
using Base.Master_Classes;

namespace Base.Data_Classes
{
    public class PowerEnhancements
    {
        public string PowerName { get; set; }
        public string EnhancementSet { get; set; }
        public int EnhancementSlot { get; set; }
    }
    public class Character
    {
        Archetype _archetype;
        bool? _completeCache;

        public List<PowerEnhancements> powerEnhancements { get; set; }

        public string setName { get; set; }
        public HashSet<string> setSlotted = new HashSet<string>();
        public Dictionary<string, int> SetSlotLoc = new Dictionary<string, int>();

        public string Name { get; set; }

        public int Level
        {
            get
            {
                if (LevelCache > -1)
                {
                    return LevelCache;
                }

                int num2;
                if (MidsContext.Config.BuildMode == Enums.dmModes.Dynamic)
                {
                    num2 = CurrentBuild.GetMaxLevel();
                }
                else
                {
                    int val1 = GetFirstAvailablePowerLevel(CurrentBuild);
                    if (val1 < 0)
                        val1 = 49;
                    int val2 = GetFirstAvailableSlotLevel();
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

        public int RequestedLevel { get; set; }

        Build[] Builds { get; }


        public Build CurrentBuild => Builds.Length > 0 ? Builds[0] : null;

        public Archetype Archetype
        {
            get => _archetype;
            set
            {
                _archetype = value;
                Alignment = _archetype.Hero ? Enums.Alignment.Hero : Enums.Alignment.Villain;
            }
        }

        public Enums.Alignment Alignment { get; set; }

        public int Origin { get; set; }

        public IPowerset[] Powersets { get; private set; }

        public bool[] PoolLocked { get; set; }

        protected int LevelCache { get; set; }

        public bool Locked { get; set; }

        public bool Complete
        {
            get
            {
                if (_completeCache.HasValue) return _completeCache.GetValueOrDefault();
                int num1 = CurrentBuild.TotalSlotsAvailable - CurrentBuild.SlotsPlaced;
                int num2 = CurrentBuild.LastPower + 1 - CurrentBuild.PowersPlaced;
                _completeCache = num1 < 1 && num2 < 1;
                return _completeCache.GetValueOrDefault();
            }
            set => _completeCache = value ? _completeCache : new bool?();
        }

        public int ActiveComboLevel { get; private set; }

        public int PerfectionOfBodyLevel => IsStalker || PerfectionType == "body" ? ActiveComboLevel : 0;

        public int PerfectionOfMindLevel => !IsStalker && PerfectionType == "mind" ? ActiveComboLevel : 0;

        public int PerfectionOfSoulLevel => !IsStalker && PerfectionType == "soul" ? ActiveComboLevel : 0;

        string PerfectionType { get; set; }

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

        public Dictionary<string, float> ModifyEffects { get; protected set; }

        public TotalStatistics Totals { get; }

        public TotalStatistics TotalsCapped { get; }

        public Statistics DisplayStats { get; }

        public int SlotsRemaining
        {
            get
            {
                int num1;
                if (Level < DatabaseAPI.Database.Levels.Length && DatabaseAPI.Database.Levels[Level].Slots > 0)
                {
                    int num2 = DatabaseAPI.Database.Levels[Level].Slots - CurrentBuild.SlotsPlacedAtLevel(Level);
                    if (num2 < 0)
                        num2 = 0;
                    num1 = num2;
                }
                else
                    num1 = 0;
                return num1;
            }
        }

        public bool CanPlaceSlot
        {
            get
            {
                if (MidsContext.Config.BuildMode == Enums.dmModes.Dynamic)
                {
                    if (CurrentBuild.TotalSlotsAvailable - CurrentBuild.SlotsPlaced > 0 & MidsContext.Config.BuildOption != Enums.dmItem.Power)
                        return true;
                }
                else if (Level > -1 & Level < DatabaseAPI.Database.Levels.Length && DatabaseAPI.Database.Levels[Level].LevelType() == Enums.dmItem.Slot && SlotsRemaining > 0)
                    return true;
                return false;
            }
        }

        public void ResetLevel()
            => LevelCache = -1;

        public void Lock()
        {
            int powersPlaced = CurrentBuild.PowersPlaced;
            if (powersPlaced == 1 & CurrentBuild.PowerUsed(Powersets[1].Powers[0]))
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

        public bool IsHero() => Alignment == Enums.Alignment.Hero || Alignment == Enums.Alignment.Vigilante;

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

        public bool PoolTaken(int poolID) => Powersets[poolID] != null && poolID >= 3 && poolID <= 7 && PoolLocked[poolID - 3];

        protected Character()
        {
            Name = string.Empty;
            Powersets = new IPowerset[8];
            PoolLocked = new bool[5];
            Totals = new TotalStatistics();
            TotalsCapped = new TotalStatistics();
            DisplayStats = new Statistics(this);
            Builds = new[] { new Build(this, DatabaseAPI.Database.Levels) };
            Reset();
        }

        // there are 2 versions of this method distributed, going to try to combine the logic bit by bit to see if there are substantial differences
        // returns the last thing it tried to read from, for inclusion in the error message
        public void LoadPowersetsByName2(IList<string> names, ref string blameName)
        {
            var powersets = names.Select(n => string.IsNullOrEmpty(n) ? null : DatabaseAPI.GetPowersetByName(n)).ToArray();
            var toBlame = powersets.Select((ps, i) => new { Index = i, Ps = ps }).FirstOrDefault(x => x.Ps == null);
            if (toBlame != null)
                blameName = names[toBlame.Index];

            if (names.Count > Powersets.Length + 1)
                Powersets = new IPowerset[names.Count];
            IPowerset powersetByName1 = DatabaseAPI.GetPowersetByName(names[0]);
            if (powersetByName1 == null)
                blameName = names[0];
            Powersets[0] = powersetByName1;
            IPowerset powersetByName2 = DatabaseAPI.GetPowersetByName(names[1]);
            if (powersetByName2 == null)
                blameName = names[1];
            Powersets[1] = powersetByName2;

            for (int index = 2; index < names.Count; ++index)
            {
                IPowerset powersetByName3 = DatabaseAPI.GetPowersetByName(names[index]);
                if (powersetByName3 == null)
                    blameName = names[index];
                Powersets[index + 1] = powersetByName3;
            }
        }
        public IEnumerable<(int, string)> LoadPowersetsByName(IList<string> names)
        {
            Powersets = names.Select(n => string.IsNullOrEmpty(n) ? null : DatabaseAPI.GetPowersetByName(n)).ToArray();
            return Powersets.Select((ps, i) => new { I = i, Ps = ps?.FullName, N = names[i] }).Where(x => !string.IsNullOrWhiteSpace(x.N) && x.Ps == null).Select(x => (x.I, x.N));
        }

        public void Reset(Archetype iArchetype = null, int iOrigin = 0)
        {
            Name = string.Empty;
            bool flag1 = Archetype != null && iArchetype != null && Archetype.Idx == iArchetype.Idx;
            Archetype = iArchetype ?? DatabaseAPI.Database.Classes[0];
            MidsContext.Archetype = Archetype;
            Origin = iOrigin > Archetype.Origin.Length - 1 ? Archetype.Origin.Length - 1 : iOrigin;
            if (flag1)
            {
                bool flag2 = Powersets[0] != null && Powersets[0].nArchetype == Archetype.Idx;
                if (!flag2)
                    Powersets[0] = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Primary)[0];
                bool flag3 = Powersets[1] != null && Powersets[1].nArchetype == Archetype.Idx;
                if (!flag3)
                    Powersets[1] = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Secondary)[0];
            }
            else
            {
                Powersets[0] = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Primary)[0];
                Powersets[1] = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Secondary)[0];
            }
            IPowerset[] powersetIndexes1 = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Pool);
            int index = 0;
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
            IPowerset[] powersetIndexes2 = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Ancillary);
            Powersets[7] = powersetIndexes2.Length <= 0 ? null : powersetIndexes2[0];
            ModifyEffects = new Dictionary<string, float>();
            PoolLocked = new bool[5];
            NewBuild();
            Locked = false;
            LevelCache = -1;
        }

        protected void NewBuild()
        {
            Builds[0] = new Build(this, DatabaseAPI.Database.Levels);
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
        }

        void RefreshActiveSpecial()
        {
            powerEnhancements = new List<PowerEnhancements>();
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

            foreach (var power in CurrentBuild.Powers)
            {
                if (power?.Power == null) continue;
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

                for (var slotIndex = 0; slotIndex < power.SlotCount; slotIndex++)
                {
                    var pSlotEnh = power.Slots[slotIndex].Enhancement.Enh;
                    if (pSlotEnh > -1)
                    {
                        IEnhancement enhancement = DatabaseAPI.Database.Enhancements[pSlotEnh];
                        powerEnhancements.Add(new PowerEnhancements{PowerName = power.Name, EnhancementSet = GetEnhSetName(enhancement.LongName), EnhancementSlot = slotIndex});

                    }
                    else
                    {
                        var index = slotIndex;
                        var toRem = powerEnhancements.FindIndex(x => x.PowerName.Equals(power.Name) && x.EnhancementSlot == index);
                        if (toRem != -1)
                        {
                            powerEnhancements.RemoveAt(toRem);
                        }
                    }
                }
            }

            //MessageBox.Show(EquipRobot.ToString());
        }

        public string GetEnhSetName(string longName)
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

        public void Validate()
        {
            CheckAncillaryPowerSet();
            CurrentBuild.Validate();
            RefreshActiveSpecial();
        }

        protected bool PowersetMutexClash(int nIDPower)
        {
            int powerSetId = DatabaseAPI.Database.Power[nIDPower].PowerSetID;
            Enums.PowersetType powersetType;
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

            if (powersetType == Enums.PowersetType.None)
                return false;
            for (int index = 0; index <= DatabaseAPI.Database.Powersets[powerSetId].nIDMutexSets.Length - 1; ++index)
            {
                if (DatabaseAPI.Database.Powersets[powerSetId].nIDMutexSets[index] == Powersets[(int)powersetType].nID)
                    return true;
            }
            for (int index = 0; index <= Powersets[(int)powersetType].nIDMutexSets.Length - 1; ++index)
            {
                if (Powersets[(int)powersetType].nIDMutexSets[index] == powerSetId)
                    return true;
            }
            return false;
        }

        void CheckAncillaryPowerSet()
        {
            IPowerset[] powersetIndexes = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Ancillary);
            if (powersetIndexes.Length == 0)
                Powersets[7] = null;
            else if (Powersets[7] == null)
            {
                Powersets[7] = powersetIndexes[0];
            }
            else
            {
                bool flag = false;
                for (int index = 0; index <= powersetIndexes.Length - 1; ++index)
                {
                    if (Powersets[7].nID == powersetIndexes[index].nID)
                        flag = true;
                }
                if (!flag && powersetIndexes.Length > 0)
                    Powersets[7] = powersetIndexes[0];
            }
        }

        IEnumerable<int> PoolGetAvailable(int iPool)
        {
            IPowerset[] powersetIndexes = DatabaseAPI.GetPowersetIndexes(Archetype, Enums.ePowerSetType.Pool);
            List<int> intList = new List<int>();
            foreach (var index1 in powersetIndexes)
            {
                bool available = false;
                for (int index2 = 3; index2 <= 6; ++index2)
                {
                    if (index2 == iPool || !(PoolLocked[index2 - 3] && index1.nID == Powersets[index2].nID))
                        available = true;
                }
                if (available)
                    intList.Add(index1.nID);
            }
            return intList;
        }

        public int PoolToComboID(int iPool, int index)
        {
            IEnumerable<int> available = PoolGetAvailable(iPool);
            int num1 = -1;
            foreach (int num2 in available)
            {
                ++num1;
                if (num2 == index)
                    return num1;
            }
            return 0;
        }

        public static PopUp.PopupData PopEnhInfo(I9Slot iSlot, int iLevel = -1, PowerEntry powerEntry = null)
        {
            PopUp.PopupData popupData1 = new PopUp.PopupData();
            int index1 = popupData1.Add();
            PopUp.PopupData popupData2;
            if (iSlot.Enh < 0)
            {
                popupData1.Sections[index1].Add("Empty Slot", PopUp.Colors.Disabled, 1.25f, FontStyle.Bold, 0);
                if (iLevel > -1)
                    popupData1.Sections[index1].Add("Slot placed at level: " + (iLevel + 1), PopUp.Colors.Text, 1f, FontStyle.Bold, 0);
                int index2 = popupData1.Add();
                popupData1.Sections[index2].Add("Right-Click to place an enhancement.", PopUp.Colors.Disabled, 1f, FontStyle.Bold | FontStyle.Italic);
                popupData1.Sections[index2].Add("Shift-Click to move this slot.", PopUp.Colors.Disabled, 1f, FontStyle.Bold | FontStyle.Italic);
                popupData2 = popupData1;
            }
            else
            {
                IEnhancement enhancement = DatabaseAPI.Database.Enhancements[iSlot.Enh];
                switch (enhancement.TypeID)
                {
                    case Enums.eType.Normal:
                        popupData1.Sections[index1].Add(enhancement.Name, PopUp.Colors.Title, 1.25f, FontStyle.Bold, 0);
                        break;
                    case Enums.eType.InventO:
                        popupData1.Sections[index1].Add("Invention: " + enhancement.Name, PopUp.Colors.Title, 1.25f, FontStyle.Bold, 0);
                        break;
                    case Enums.eType.SpecialO:
                        popupData1.Sections[index1].Add(enhancement.Name, PopUp.Colors.Title, 1.25f, FontStyle.Bold, 0);
                        break;
                    case Enums.eType.SetO:
                        Color iColor = PopUp.Colors.Title;
                        if (enhancement.RecipeIDX > -1)
                        {
                            switch (DatabaseAPI.Database.Recipes[enhancement.RecipeIDX].Rarity)
                            {
                                case Recipe.RecipeRarity.Uncommon:
                                    iColor = PopUp.Colors.Uncommon;
                                    break;
                                case Recipe.RecipeRarity.Rare:
                                    iColor = PopUp.Colors.Rare;
                                    break;
                                case Recipe.RecipeRarity.UltraRare:
                                    iColor = PopUp.Colors.UltraRare;
                                    break;
                            }
                        }
                        popupData1.Sections[index1].Add(DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].DisplayName + ": " + enhancement.Name, iColor, 1.25f, FontStyle.Bold, 0);
                        break;
                }
                switch (enhancement.TypeID)
                {
                    case Enums.eType.Normal:
                        popupData1.Sections[index1].Add(iSlot.GetEnhancementString(), Color.FromArgb(0, byte.MaxValue, 0), 1f, FontStyle.Bold, 0);
                        break;
                    case Enums.eType.InventO:
                        popupData1.Sections[index1].Add("Invention Level: " + (iSlot.IOLevel + 1) + iSlot.GetRelativeString(false) + " - " + iSlot.GetEnhancementString(), PopUp.Colors.Invention, 1f, FontStyle.Bold, 0);
                        break;
                    case Enums.eType.SpecialO:
                        popupData1.Sections[index1].Add(iSlot.GetEnhancementString(), Color.FromArgb(byte.MaxValue, byte.MaxValue, 0), 1f, FontStyle.Bold, 0);
                        break;
                    case Enums.eType.SetO:
                        popupData1.Sections[index1].Add("Invention Level: " + (iSlot.IOLevel + 1) + iSlot.GetRelativeString(false), PopUp.Colors.Invention, 1f, FontStyle.Bold, 0);
                        break;
                }
                if (iLevel > -1)
                    popupData1.Sections[index1].Add("Slot placed at level: " + (iLevel + 1), PopUp.Colors.Text, 1f, FontStyle.Bold, 0);
                if (enhancement.Unique)
                {
                    index1 = popupData1.Add();
                    popupData1.Sections[index1].Add("This enhancement is Unique. No more than one enhancement of this type can be slotted by a character.", PopUp.Colors.Text, 0.9f, FontStyle.Bold, 0);
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
                        int index2 = popupData1.Add();
                        string[] strArray1 = BreakByNewLine(iSlot.GetEnhancementStringLong());
                        for (int index3 = 0; index3 <= strArray1.Length - 1; ++index3)
                        {
                            string[] strArray2 = BreakByBracket(strArray1[index3]);
                            popupData1.Sections[index2].Add(strArray2[0], Color.FromArgb(0, byte.MaxValue, 0), strArray2[1], Color.FromArgb(0, byte.MaxValue, 0), 0.9f);
                        }
                        break;
                    case Enums.eType.SpecialO:
                    case Enums.eType.SetO:
                        if (!string.IsNullOrEmpty(enhancement.Desc))
                            popupData1.Sections[index1].Add(enhancement.Desc, PopUp.Colors.Title, 1f, FontStyle.Bold, 0);
                        int index4 = popupData1.Add();
                        string[] strArray3 = BreakByNewLine(iSlot.GetEnhancementStringLong());
                        for (int index3 = 0; index3 <= strArray3.Length - 1; ++index3)
                        {
                            var strArray2 = !enhancement.HasPowerEffect ? BreakByBracket(strArray3[index3]) : new[] { strArray3[index3], string.Empty };
                            string[] strArray4 = strArray2;
                            popupData1.Sections[index4].Add(strArray4[0], Color.FromArgb(0, byte.MaxValue, 0), strArray4[1], Color.FromArgb(0, byte.MaxValue, 0), 0.9f, FontStyle.Bold, 0);
                        }
                        break;
                }
                if (!MidsContext.Config.PopupRecipes)
                {
                    if (enhancement.TypeID == Enums.eType.SetO)
                    {
                        int index3 = popupData1.Add();
                        popupData1.Sections[index3].Add("Set Type: " + DatabaseAPI.Database.SetTypeStringLong[(int)DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].SetType], PopUp.Colors.Invention, 1f, FontStyle.Bold, 0);
                        popupData1.Sections[index3].Add("Set Level Range: " + (DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].LevelMin + 1) + " to " + (DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].LevelMax + 1), PopUp.Colors.Text, 1f, FontStyle.Bold, 0);
                        popupData1.Add(PopSetEnhList(enhancement.nIDSet, powerEntry));
                        popupData1.Add(PopSetBonusListing(enhancement.nIDSet, powerEntry));
                    }
                }
                else if (enhancement.TypeID == Enums.eType.SetO || enhancement.TypeID == Enums.eType.InventO)
                    popupData1.Add(PopRecipeInfo(enhancement.RecipeIDX, iSlot.IOLevel));
                popupData2 = popupData1;
            }
            return popupData2;
        }

        public int GetFirstAvailablePowerIndex(int iLevel = 0)
        {
            for (int index = 0; index <= CurrentBuild.LastPower; ++index)
            {
                if (CurrentBuild.Powers[index].NIDPowerset < 0 && CurrentBuild.Powers[index].Level >= iLevel)
                    return index;
            }
            return -1;
        }

        static int GetFirstAvailablePowerLevel(Build currentbuild, int iLevel = 0)
        {
            for (int index = 0; index <= currentbuild.LastPower; ++index)
            {
                if (currentbuild.Powers[index].NIDPowerset < 0 & currentbuild.Powers[index].Level >= iLevel)
                    return currentbuild.Powers[index].Level;
            }
            return -1;
        }

        int GetFirstAvailableSlotLevel(int iLevel = 0)
        {
            if (iLevel < 0)
                iLevel = 0;
            for (int level = iLevel; level < DatabaseAPI.Database.Levels.Length; ++level)
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

            int iLevel = power.Level;
            if (DatabaseAPI.Database.Power[power.NIDPower].AllowFrontLoading)
                iLevel = 0;
            int firstAvailable = GetFirstAvailableSlotLevel(iLevel);
            if (MidsContext.Config.BuildMode == Enums.dmModes.LevelUp && firstAvailable > CurrentBuild.GetMaxLevel() + 1)
                firstAvailable = -1;
            return firstAvailable;
        }

        public int[] GetSlotCounts()
        {
            int[] numArray = new int[2];
            for (int level = 0; level < DatabaseAPI.Database.Levels.Length; ++level)
            {
                if (DatabaseAPI.Database.Levels[level].Slots <= 0)
                    continue;
                int num = CurrentBuild.SlotsPlacedAtLevel(level);
                numArray[0] += DatabaseAPI.Database.Levels[level].Slots - num;
                numArray[1] += num;
            }
            return numArray;
        }

        static string[] BreakByNewLine(string iString)
        {
            iString = iString.Replace('\n', '^');
            return iString.Split('^');
        }

        static string[] BreakByBracket(string iString)
        {
            string[] strArray1 = { iString, string.Empty };
            int length = iString.IndexOf(" (", StringComparison.Ordinal);
            string[] strArray2;
            if (length < 0)
            {
                strArray2 = strArray1;
            }
            else
            {
                strArray1[0] = iString.Substring(0, length) + ":";
                if (iString.Length - (length + 1) > 0)
                    strArray1[1] = iString.Substring(length + 1).Replace("(", "").Replace(")", "");
                strArray2 = strArray1;
            }
            return strArray2;
        }

        static PopUp.Section PopSetBonusListing(int sIdx, PowerEntry power)
        {
            var section1 = new PopUp.Section();
            section1.Add("Set Bonus:", PopUp.Colors.Title, 1f, FontStyle.Bold, 0);
            int num = 0;
            if (power != null)
            {
                for (int index = 0; index <= power.Slots.Length - 1; ++index)
                {
                    if (power.Slots[index].Enhancement.Enh > -1 && DatabaseAPI.Database.Enhancements[power.Slots[index].Enhancement.Enh].nIDSet == sIdx)
                        ++num;
                }
            }
            if (sIdx < 0 || sIdx > DatabaseAPI.Database.EnhancementSets.Count - 1)
            {
                return section1;
            }

            EnhancementSet enhancementSet = DatabaseAPI.Database.EnhancementSets[sIdx];
            for (int index = 0; index <= enhancementSet.Bonus.Length - 1; ++index)
            {
                string effectString = enhancementSet.GetEffectString(index, false, true);
                if (string.IsNullOrEmpty(effectString))
                    continue;
                if (enhancementSet.Bonus[index].PvMode == Enums.ePvX.PvP)
                    effectString += " (PvP)";
                if (num >= enhancementSet.Bonus[index].Slotted & (enhancementSet.Bonus[index].PvMode == Enums.ePvX.PvE & !MidsContext.Config.Inc.DisablePvE | enhancementSet.Bonus[index].PvMode == Enums.ePvX.PvP & MidsContext.Config.Inc.DisablePvE | enhancementSet.Bonus[index].PvMode == Enums.ePvX.Any))
                    section1.Add("(" + enhancementSet.Bonus[index].Slotted + ") " + effectString, PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 0);
                else if (power == null)
                    section1.Add("(" + enhancementSet.Bonus[index].Slotted + ") " + effectString, PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 0);
                else
                    section1.Add("(" + enhancementSet.Bonus[index].Slotted + ") " + effectString, PopUp.Colors.Disabled, 0.9f, FontStyle.Bold, 0);
            }
            for (int index = 0; index <= enhancementSet.SpecialBonus.Length - 1; ++index)
            {
                string effectString = enhancementSet.GetEffectString(index, true, true);
                if (string.IsNullOrEmpty(effectString))
                    continue;
                bool flag = false;
                if (power != null)
                {
                    foreach (SlotEntry slot in power.Slots)
                    {
                        if (slot.Enhancement.Enh > -1 && enhancementSet.SpecialBonus[index].Special > -1 && slot.Enhancement.Enh == enhancementSet.Enhancements[enhancementSet.SpecialBonus[index].Special])
                            flag = true;
                    }
                }
                if (flag)
                    section1.Add("(Enh) " + effectString, PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 0);
                else if (power == null)
                    section1.Add("(Enh) " + effectString, PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 0);
                else
                    section1.Add("(Enh) " + effectString, PopUp.Colors.Disabled, 0.9f, FontStyle.Bold, 0);
            }
            return section1;
        }

        public static PopUp.Section PopRecipeInfo(int rIdx, int iLevel)
        {
            PopUp.Section section1 = new PopUp.Section();
            if (rIdx < 0)
            {
                return section1;
            }

            Recipe recipe = DatabaseAPI.Database.Recipes[rIdx];
            int index1 = -1;
            int num1 = 52;
            int num2 = 0;
            for (int index2 = 0; index2 <= recipe.Item.Length - 1; ++index2)
            {
                if (recipe.Item[index2].Level > num2)
                    num2 = recipe.Item[index2].Level;
                if (recipe.Item[index2].Level < num1)
                    num1 = recipe.Item[index2].Level;
                if (recipe.Item[index2].Level != iLevel)
                    continue;
                index1 = index2;
                break;
            }
            if (index1 < 0)
            {
                iLevel = Enhancement.GranularLevelZb(iLevel, 0, 49, 5);
                for (int index2 = 0; index2 <= recipe.Item.Length - 1; ++index2)
                {
                    if (recipe.Item[index2].Level != iLevel)
                        continue;
                    index1 = index2;
                    break;
                }
            }
            if (index1 < 0)
            {
                return section1;
            }

            Recipe.RecipeEntry recipeEntry = recipe.Item[index1];
            string str = string.Empty;
            if (recipe.EnhIdx > -1)
                str = " - " + DatabaseAPI.Database.Enhancements[recipe.EnhIdx].LongName;
            section1.Add("Recipe" + str, PopUp.Colors.Title, 1f, FontStyle.Bold, 0);
            if (recipeEntry.BuyCost > 0)
                section1.Add("Buy Cost:", PopUp.Colors.Invention, $"{recipeEntry.BuyCost:###,###,##0}", PopUp.Colors.Invention, 0.9f, FontStyle.Bold, 1);
            if (recipeEntry.CraftCost > 0)
                section1.Add("Craft Cost:", PopUp.Colors.Invention, $"{recipeEntry.CraftCost:###,###,##0}", PopUp.Colors.Invention, 0.9f, FontStyle.Bold, 1);
            if (recipeEntry.CraftCostM > 0)
                section1.Add("Craft Cost (Memorized):", PopUp.Colors.Effect, $"{recipeEntry.CraftCostM:###,###,##0}", PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
            for (int index2 = 0; index2 <= recipeEntry.Salvage.Length - 1 && (index2 == 0 || recipeEntry.SalvageIdx[index2] != recipeEntry.SalvageIdx[0]); ++index2)
            {
                if (recipeEntry.SalvageIdx[index2] < 0)
                    continue;
                Color iColor;
                string empty = string.Empty;
                switch (DatabaseAPI.Database.Salvage[recipeEntry.SalvageIdx[index2]].Rarity)
                {
                    case Recipe.RecipeRarity.Common:
                        iColor = PopUp.Colors.Common;
                        break;
                    case Recipe.RecipeRarity.Uncommon:
                        iColor = PopUp.Colors.Uncommon;
                        break;
                    case Recipe.RecipeRarity.Rare:
                        iColor = PopUp.Colors.Rare;
                        break;
                    case Recipe.RecipeRarity.UltraRare:
                        iColor = PopUp.Colors.UltraRare;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (recipeEntry.Count[index2] > 0)
                    section1.Add(DatabaseAPI.Database.Salvage[recipeEntry.SalvageIdx[index2]].ExternalName + empty, iColor, recipeEntry.Count[index2].ToString(CultureInfo.InvariantCulture), PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }
            return section1;
        }

        public static PopUp.PopupData PopSetInfo(int sIdx, PowerEntry powerEntry = null)
        {
            if (sIdx < 0)
            {
                PopUp.PopupData popupData1 = new PopUp.PopupData();
                return popupData1;
            }
            else
            {
                EnhancementSet enhancementSet = DatabaseAPI.Database.EnhancementSets[sIdx];
                Color iColor = PopUp.Colors.Uncommon;
                for (int index = 0; index <= enhancementSet.Enhancements.Length - 1; ++index)
                {
                    IEnhancement enhancement = DatabaseAPI.Database.Enhancements[enhancementSet.Enhancements[index]];
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
                PopUp.PopupData popupData1 = new PopUp.PopupData();
                int index1 = popupData1.Add();
                popupData1.Sections[index1].Add(enhancementSet.DisplayName, iColor, 1.25f, FontStyle.Bold, 0);
                popupData1.Sections[index1].Add("Set Type: " + DatabaseAPI.Database.SetTypeStringLong[(int)enhancementSet.SetType], PopUp.Colors.Invention, 1f, FontStyle.Bold, 0);
                string str = enhancementSet.LevelMin != enhancementSet.LevelMax ? (enhancementSet.LevelMin + 1) + " to " + (enhancementSet.LevelMax + 1) : (enhancementSet.LevelMin + 1).ToString(CultureInfo.InvariantCulture);
                popupData1.Sections[index1].Add("Level Range: " + str, PopUp.Colors.Text, 1f, FontStyle.Bold, 0);
                popupData1.Add(PopSetEnhList(sIdx, powerEntry));
                popupData1.Add(PopSetBonusListing(sIdx, powerEntry));
                return popupData1;
            }
        }

        static PopUp.Section PopSetEnhList(int sIdx, PowerEntry power)
        {
            if (sIdx < 0 || sIdx > DatabaseAPI.Database.EnhancementSets.Count - 1)
            {
                return new PopUp.Section();
            }

            int num = 0;
            EnhancementSet enhancementSet = DatabaseAPI.Database.EnhancementSets[sIdx];
            bool[] flagArray = new bool[enhancementSet.Enhancements.Length];
            for (int index = 0; index <= flagArray.Length - 1; ++index)
                flagArray[index] = false;
            if (power != null)
            {
                foreach (SlotEntry slot in power.Slots)
                {
                    if (slot.Enhancement.Enh < 0 || DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].nIDSet != sIdx)
                        continue;
                    ++num;
                    for (int index = 0; index <= enhancementSet.Enhancements.Length - 1; ++index)
                    {
                        if (slot.Enhancement.Enh == enhancementSet.Enhancements[index])
                            flagArray[index] = true;
                    }
                }
            }
            var section1 = new PopUp.Section();
            if (power != null)
                section1.Add("Set: " + enhancementSet.DisplayName + " (" + num + "/" + enhancementSet.Enhancements.Length + ")", PopUp.Colors.Title, 1f, FontStyle.Bold, 0);
            for (int index = 0; index <= enhancementSet.Enhancements.Length - 1; ++index)
            {
                string iText = enhancementSet.DisplayName + ": " + DatabaseAPI.Database.Enhancements[enhancementSet.Enhancements[index]].Name;
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
            int[] poolOrder = new int[4];
            int[] poolIndex = new int[4];
            for (int i = 3; i <= 6; ++i)
            {
                var ps = Powersets[i];
                poolIndex[i - 3] = ps?.nID ?? -1;
                poolOrder[i - 3] = ps != null ? GetEarliestPowerIndex(Powersets[i].nID) : -1;
            }
            for (int i = 0; i <= 3; ++i)
            {
                int minO = byte.MaxValue;
                int minI = -1;
                for (int x = 0; x <= poolOrder.Length - 1; ++x)
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
            // HACK: this assumes at least 8 powersets exist, but the database is fully editable.
            PoolLocked[0] = PowersetUsed(Powersets[3]) & PoolUnique(Enums.PowersetType.Pool0);
            PoolLocked[1] = PowersetUsed(Powersets[4]) & PoolUnique(Enums.PowersetType.Pool1);
            PoolLocked[2] = PowersetUsed(Powersets[5]) & PoolUnique(Enums.PowersetType.Pool2);
            PoolLocked[3] = PowersetUsed(Powersets[6]) & PoolUnique(Enums.PowersetType.Pool3);
            PoolLocked[4] = PowersetUsed(Powersets[7]);
        }

        int GetEarliestPowerIndex(int iSet)
        {
            for (int index = 0; index <= CurrentBuild.LastPower; ++index)
            {
                if (CurrentBuild.Powers[index].NIDPowerset == iSet)
                    return index;
            }
            return CurrentBuild.LastPower + 1;
        }

        bool PoolUnique(Enums.PowersetType pool)
        {
            var ps = Powersets[(int)pool];
            if (ps == null) return false;
            for (int index = 3; (Enums.PowersetType)index < pool; ++index)
            {
                if (Powersets[index] != null && Powersets[index].nID == Powersets[(int)pool].nID)
                    return false;
            }
            return true;
        }

        bool PowersetUsed(IPowerset powerset)
        {
            return powerset != null && CurrentBuild.Powers.Any(t => t.NIDPowerset == powerset.nID & t.IDXPower > -1);
        }

        protected bool CanRemovePower(int index, bool allowSecondary, out string message)
        {
            message = string.Empty;
            PowerEntry power = CurrentBuild.Powers[index];
            if (!power.Chosen)
            {
                message = "You can't remove inherent powers.\nIf the power is a Kheldian form power, you can remove it by removing the shapeshift power which grants it.";
                return false;
            }

            if (!(power.NIDPowerset == Powersets[1].nID & power.IDXPower == 0 & !allowSecondary))
                return power.NIDPowerset >= 0;
            if (CurrentBuild.PowersPlaced <= 1)
                return true;
            message = "The first power from your secondary set is non-optional and can't be removed.";
            return false;

        }

        public void SwitchSets(IPowerset newPowerset, IPowerset oldPowerset)
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
            for (int index4 = 0; index4 < Powersets.Length; ++index4)
            {
                if (Powersets[index4] != null && Powersets[index4].nID == oldPowerset.nID)
                    Powersets[index4] = newPowerset;
            }
            foreach (PowerEntry power in CurrentBuild.Powers)
            {
                if (power.NIDPowerset < 0)
                    continue;
                int idxPower = power.IDXPower;
                if (power.NIDPowerset == oldTrunk)
                {
                    for (int index4 = 0; index4 < DatabaseAPI.Database.Powersets[oldTrunk].Power.Length && DatabaseAPI.Database.Powersets[oldTrunk].Powers[index4].Level == 0; ++index4)
                        --idxPower;
                    for (int index4 = 0; index4 < DatabaseAPI.Database.Powersets[newTrunk].Power.Length && DatabaseAPI.Database.Powersets[newTrunk].Powers[index4].Level == 0; ++index4)
                        ++idxPower;
                    if (newTrunk < 0)
                        power.Reset();
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
                    for (int index4 = 0; index4 < DatabaseAPI.Database.Powersets[oldTrunk].Power.Length && DatabaseAPI.Database.Powersets[oldTrunk].Powers[index4].Level == 0; ++index4)
                        --idxPower;
                    for (int index4 = 0; index4 < DatabaseAPI.Database.Powersets[newTrunk].Power.Length && DatabaseAPI.Database.Powersets[newTrunk].Powers[index4].Level == 0; ++index4)
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
                {
                    power.Slots = new[]{
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
                    for (int index4 = 0; index4 < power.SlotCount; ++index4)
                    {
                        if (!power.PowerSet.Powers[idxPower].IsEnhancementValid(power.Slots[index4].Enhancement.Enh))
                            power.Slots[index4].Enhancement = new I9Slot();
                    }
                }
            }
            CurrentBuild.FullMutexCheck();
        }

        public class TotalStatistics
        {
            public float[] Def { get; set; }
            public float[] Res { get; set; }
            public float[] Mez { get; set; }
            public float[] MezRes { get; set; }
            public float[] DebuffRes { get; set; }
            public float Elusivity { get; set; }
            public float HPRegen { get; set; }
            public float HPMax { get; set; }
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

            internal TotalStatistics()
            {
                // do not set values to the value they default to in a constructor
                Init(fullReset: false);
            }

            public void Init(bool fullReset = true)
            {
                Def = new float[Enum.GetValues(Enums.eDamage.None.GetType()).Length];
                Res = new float[Enum.GetValues(Enums.eDamage.None.GetType()).Length];
                Mez = new float[Enum.GetValues(Enums.eMez.None.GetType()).Length];
                MezRes = new float[Enum.GetValues(Enums.eMez.None.GetType()).Length];
                DebuffRes = new float[Enum.GetValues(Enums.eEffectType.None.GetType()).Length];
                if (!fullReset) return;
                Elusivity = 0.0f;
                HPRegen = 0.0f;
                HPMax = 0.0f;
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
