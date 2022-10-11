using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Documents;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms.Controls;
using static Mids_Reborn.Core.CSV;

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
        /// The display name of the Pet entity
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// A description of the pet entity
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Returned values from generating buffed power data from pet powers.
        /// </summary>
        public PetPowersData? PowersData { get; set; }

        public PetInfo()
        {

        }

        public PetInfo(int idxPower, IPower basePower, List<IPower> powers)
        {
            _powerEntryIndex = idxPower;
            _basePower = basePower;
            _powers = powers;
            PopulateBasicInfo();
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
            return new PetPower(PowersData.BasePowers.First(p => p.PowerIndex == power.PowerIndex), PowersData.BuffedPowers.First(p => p.PowerIndex == power.PowerIndex));
        }

        /// <summary>
        /// Gets new PowersData for the last power passed.
        /// </summary>
        /// <returns>PetPowers instance containing updated base and buffed versions of a power.</returns>
        public PetPower? GetPetPower()
        {
            return _lastPower < 0 ? new PetPower(PowersData.BasePowers.First(), PowersData.BuffedPowers.First()) : new PetPower(PowersData.BasePowers.First(p => p.PowerIndex == _lastPower), PowersData.BuffedPowers.First(p => p.PowerIndex == _lastPower));
        }

        private void PopulateBasicInfo()
        {
            if (_basePower is not { HasEntity: true }) return;
            Name = _basePower.DisplayName;
            Description = _basePower.DescLong;
        }

        private void GeneratePetPowerData()
        {
            if (_powers == null || _basePower == null) return;
            var powerData = MainModule.MidsController.Toon?.GenerateBuffedPowers(_powers, _powerEntryIndex);
            if (powerData != null) PowersData = new PetPowersData(powerData.Value.Key, powerData.Value.Value);
            PowersUpdated?.Invoke(this, EventArgs.Empty);
        }

        public class PetPowersData
        {
            public readonly List<IPower> BasePowers;
            public readonly List<IPower> BuffedPowers;

            public PetPowersData(List<IPower> basePowers, List<IPower> buffedPowers)
            {
                BasePowers = basePowers;
                BuffedPowers = buffedPowers;
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
