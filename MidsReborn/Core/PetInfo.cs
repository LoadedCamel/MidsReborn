using Mids_Reborn.Core.Base.Master_Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Mids_Reborn.Core.Import;

namespace Mids_Reborn.Core
{
    public class PetInfo
    {
        /// <summary>
        /// An event that when subscribed to signifies when an update has completed on the PowerData.
        /// </summary>
        public static event EventHandler? PowersUpdated;

        public event EventHandler? PowersDataUpdated;
        private static EventHandler? _powersDataModified;

        /// <summary>
        /// The parent power passed to the constructor that is the summoning power for the pet entity.
        /// </summary>
        public readonly int PowerEntryIndex;

        private readonly IPower? _basePower;

        private readonly SummonedEntity? _entity;

        /// <summary>
        /// List of powers passed to the constructor that represent the pet entities powers.
        /// </summary>
        private List<IPower>? _powers;

        /// <summary>
        /// Last Power index that was passed to GetPower.
        /// </summary>
        private int _lastPower = -1;

        /// <summary>
        /// Returned values from generating buffed power data from pet powers.
        /// </summary>
        private static PetPowersData? PowersData { get; set; }

        public PetInfo()
        {

        }

        
        public PetInfo(SummonedEntity entity, int idxPower, IPower basePower)
        {
            _entity = entity;
            PowerEntryIndex = idxPower;
            _basePower = basePower;
            CompilePetPowers(out _);
            GeneratePetPowerData();
            _powersDataModified += OnPowersDataModified;
        }

        private void OnPowersDataModified(object? sender, EventArgs e)
        {
            PowersDataUpdated?.Invoke(this, EventArgs.Empty);
        }

        public bool HasEmptyBasePower => _basePower == null;

        private void CompilePetPowers(out HashSet<string> powerNames)
        {
            powerNames = new HashSet<string>();
            if (_entity == null) return;
            var allPowers = _entity.GetPowers();
            var currentBuildPowers = MidsContext.Character.CurrentBuild.Powers
                .Where(pe => pe?.Power != null)
                .Select(pe => pe?.Power)
                .ToHashSet();

            var filteredPowers = allPowers
                .Where(powerPair => powerPair.Value == null || currentBuildPowers.Contains(powerPair.Value))
                .Select(powerPair => powerPair.Key)
                .ToList();

            powerNames = filteredPowers.Select(x => x.FullName).ToHashSet();
            _powers = filteredPowers;
        }

        public void ExecuteUpdate()
        {
            GeneratePetPowerData();
        }

        public void ExecuteUpdate(out HashSet<string> powers)
        {
            CompilePetPowers(out powers);
            GeneratePetPowerData();
        }

        public static List<IPower>? GetBuffedPowers()
        {
            return PowersData?.BuffedPowers;
        }

        /// <summary>
        /// Gets PowersData for the power passed.
        /// </summary>
        /// <param name="power"></param>
        /// <returns>PetPowers instance containing base and buffed versions of a power.</returns>
        public PetPower? GetPetPower(IPower power)
        {
            _lastPower = power.PowerIndex;
            if (PowersData != null && PowersData.BasePowers.Any())
            {
                return new PetPower(PowersData.BasePowers.First(p => p.PowerIndex == power.PowerIndex),
                    PowersData.BuffedPowers.First(p => p.PowerIndex == power.PowerIndex));
            }
            return null;
        }

        /// <summary>
        /// Gets new PowersData for the last power passed.
        /// </summary>
        /// <returns>PetPowers instance containing updated base and buffed versions of a power.</returns>
        public PetPower? GetPetPower()
        {
            if (PowersData != null && PowersData.BasePowers.Any())
                if (_lastPower < 0)
                    return new PetPower(PowersData.BasePowers.First(), PowersData.BuffedPowers.First());
                else
                    return new PetPower(PowersData.BasePowers.First(p => p.PowerIndex == _lastPower),
                        PowersData.BuffedPowers.First(p => p.PowerIndex == _lastPower));
            return null;
        }

        private void GeneratePetPowerData()
        {
            if (_powers == null || _basePower == null) return;
            var powerData = MainModule.MidsController.Toon?.GenerateBuffedPowers(_powers, PowerEntryIndex);
            if (powerData != null)
            {
                PowersData = new PetPowersData(powerData.Value.Key, powerData.Value.Value);
            }

            PowersDataUpdated?.Invoke(this, EventArgs.Empty);
        }

        private class PetPowersData
        {
            public readonly List<IPower> BasePowers;
            public readonly List<IPower> BuffedPowers;

            public PetPowersData(List<IPower> basePowers, List<IPower> buffedPowers)
            {
                BasePowers = basePowers.Where(p => p.IsPetPower).ToList();
                BuffedPowers = buffedPowers.Where(p => p.IsPetPower).ToList();
            }
        }

        public class PetPower
        {
            public readonly IPower BasePower;
            public readonly IPower BuffedPower;

            public PetPower(IPower basePower, IPower buffedPower)
            {
                BasePower = basePower;
                BuffedPower = buffedPower;
            }
        }
    }
}
