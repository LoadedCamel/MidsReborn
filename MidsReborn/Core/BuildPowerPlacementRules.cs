namespace Mids_Reborn.Core
{
    public static class BuildPowerPlacementRules
    {
        public static bool TryValidateStarterPowerSlots(
            IReadOnlyList<PowerEntry?> powers,
            IReadOnlyList<IPowerset?>? selectedPowersets,
            out string message)
        {
            message = string.Empty;

            if (powers.Count < 2 || selectedPowersets is not { Count: > 1 })
            {
                return true;
            }

            return TryValidateStarterPowerSlot(
                       powers,
                       selectedPowersets[0],
                       slotIndex: 0,
                       slotName: "first",
                       setName: "Primary",
                       out message) &&
                   TryValidateStarterPowerSlot(
                       powers,
                       selectedPowersets[1],
                       slotIndex: 1,
                       slotName: "second",
                       setName: "Secondary",
                       out message);
        }

        private static bool TryValidateStarterPowerSlot(
            IReadOnlyList<PowerEntry?> powers,
            IPowerset? selectedPowerset,
            int slotIndex,
            string slotName,
            string setName,
            out string message)
        {
            message = string.Empty;

            if (selectedPowerset is not { nID: >= 0 })
            {
                return true;
            }

            var starterPowerIds = DatabaseAPI.NidPowersAtLevelBranch(0, selectedPowerset.nID).ToHashSet();
            if (starterPowerIds.Count == 0)
            {
                return true;
            }

            var entry = powers[slotIndex];
            if (entry is not { NIDPower: >= 0 })
            {
                if (!ContainsStarterPowerOutsideSlot(powers, starterPowerIds, slotIndex))
                {
                    return true;
                }

                message = $"The {slotName} power pick must be one of the level 1 powers from your {setName} set.";
                return false;
            }

            if (starterPowerIds.Contains(entry.NIDPower))
            {
                return true;
            }

            message = $"The {slotName} power pick must be one of the level 1 powers from your {setName} set.";
            return false;
        }

        private static bool ContainsStarterPowerOutsideSlot(
            IReadOnlyList<PowerEntry?> powers,
            HashSet<int> starterPowerIds,
            int slotIndex)
        {
            for (var index = 0; index < powers.Count; index++)
            {
                if (index == slotIndex)
                {
                    continue;
                }

                if (powers[index] is { NIDPower: >= 0 } entry && starterPowerIds.Contains(entry.NIDPower))
                {
                    return true;
                }
            }

            return false;
        }
    }
}