using System;
using System.Collections.Generic;
using System.Linq;

namespace Mids_Reborn.Core
{
    public class PetInfo
    {
        /// <summary>
        /// An event that when subscribed to signifies when an update has completed on the PowerData.
        /// </summary>
        public event EventHandler? PowersUpdated;

        /// <summary>
        /// The parent power passed to the constructor that is the summoning power for the pet entity.
        /// </summary>
        private readonly int _powerEntryIndex;

        private readonly IPower? _basePower;

        /// <summary>
        /// List of powers passed to the constructor that represent the pet entities powers.
        /// </summary>
        private readonly List<IPower>? _powers;

        /// <summary>
        /// Last Power index that was passed to GetPower.
        /// </summary>
        private int _lastPower = -1;

        /// <summary>
        /// Returned values from generating buffed power data from pet powers.
        /// </summary>
        private PetPowersData? PowersData { get; set; }

        public PetInfo()
        {

        }

        public PetInfo(int idxPower, IPower basePower, List<IPower> powers)
        {
            _powerEntryIndex = idxPower;
            _basePower = basePower;
            _powers = powers;
            GeneratePetPowerData();
        }

        public bool HasEmptyBasePower => _basePower == null;

        public void ExecuteUpdate()
        {
            GeneratePetPowerData();
        }

        /// <summary>
        /// Gets PowersData for the power passed.
        /// </summary>
        /// <param name="power"></param>
        /// <returns>PetPowers instance containing base and buffed versions of a power.</returns>
        public PetPower? GetPetPower(IPower power)
        {
            _lastPower = power.PowerIndex;
            if (PowersData != null)
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
            if (PowersData != null)
                return _lastPower < 0
                    ? new PetPower(PowersData.BasePowers.First(), PowersData.BuffedPowers.First())
                    : new PetPower(PowersData.BasePowers.First(p => p.PowerIndex == _lastPower),
                        PowersData.BuffedPowers.First(p => p.PowerIndex == _lastPower));
            return null;
        }

        private void GeneratePetPowerData()
        {
            if (_powers == null || _basePower == null) return;
            var powerData = MainModule.MidsController.Toon?.GenerateBuffedPowers(_powers, _powerEntryIndex);
            if (powerData != null)
            {
                PowersData = new PetPowersData(powerData.Value.Key, powerData.Value.Value);
            }

            PowersUpdated?.Invoke(this, EventArgs.Empty);
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
