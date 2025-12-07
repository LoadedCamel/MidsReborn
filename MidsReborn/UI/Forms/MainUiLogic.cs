using System;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.UI.Forms
{
    // non-mutating OR low var count methods OR anything easily liftable
    // avoid mutation and the service location pattern
    public static class MainUiLogic
    {
        public static void ChangeSets(Toon toon, Character? ch, int primaryIndex, int secondaryIndex, int pool0Index, int pool1Index, int pool2Index, int pool3Index, int ancillaryIndex, Func<Archetype, Enums.ePowerSetType, IPowerset[]> getPowerSets, Action lockSecondary)
        {
            var at = ch.Archetype;
            var newPrimaryPowerset = getPowerSets(at, Enums.ePowerSetType.Primary)[primaryIndex];
            IPowerset?[] ancPowersets = getPowerSets(at, Enums.ePowerSetType.Ancillary);
            if (toon != null)
            {
                var powerset1 = ch.Powersets[0];
                if (powerset1.nID != newPrimaryPowerset.nID)
                {
                    toon.SwitchSets(newPrimaryPowerset, powerset1);
                }

                if (ch.Powersets[0].nIDLinkSecondary > -1)
                {
                    var powerset2 = ch.Powersets[1];
                    var powerset3 = DatabaseAPI.Database.Powersets[ch.Powersets[0].nIDLinkSecondary];
                    if (powerset2.nID != powerset3.nID)
                    {
                        toon.SwitchSets(powerset3, powerset2);
                    }
                }
                else
                {
                    lockSecondary();
                    var powerset2 = ch.Powersets[1];
                    IPowerset?[] secondaryPowersets = getPowerSets(at, Enums.ePowerSetType.Secondary);
                    var newPowerset2 = secondaryPowersets[secondaryIndex];
                    if (powerset2.nID != newPowerset2.nID)
                    {
                        toon.SwitchSets(newPowerset2, powerset2);
                    }
                }
            }
            else
            {
                IPowerset?[] secondaryPowersets = getPowerSets(at, Enums.ePowerSetType.Secondary);
                ch.Powersets[0] = newPrimaryPowerset;
                ch.Powersets[1] = secondaryPowersets[secondaryIndex];
            }

            IPowerset?[] poolPowersets = getPowerSets(at, Enums.ePowerSetType.Pool);
            ch.Powersets[3] = poolPowersets[pool0Index];
            ch.Powersets[4] = poolPowersets[pool1Index];
            ch.Powersets[5] = poolPowersets[pool2Index];
            ch.Powersets[6] = poolPowersets[pool3Index];
            if (ancPowersets.Length > 0)
            {
                ch.Powersets[7] = ancPowersets[ancillaryIndex];
            }

            ch.Validate();
        }

        public static void ChangeSets(Toon? toon, Character? ch, int primaryIndex, int secondaryIndex, int pool0Index, int pool1Index, int pool2Index, int pool3Index, int ancillaryIndex, Func<Archetype, Enums.ePowerSetType, IPowerset[]> getPowerSets, Action<bool> setSecondaryLocked /* true = lock (no clear), false = unlock */)
        {
            var at = ch?.Archetype;

            var primarySets = getPowerSets(at, Enums.ePowerSetType.Primary);
            var secondarySets = getPowerSets(at, Enums.ePowerSetType.Secondary);
            var poolSets = getPowerSets(at, Enums.ePowerSetType.Pool);
            var ancillarySets = getPowerSets(at, Enums.ePowerSetType.Ancillary);

            static int Clamp(int idx, int len) => len == 0 ? -1 : Math.Max(0, Math.Min(idx, len - 1));

            primaryIndex = Clamp(primaryIndex, primarySets.Length);
            secondaryIndex = Clamp(secondaryIndex, secondarySets.Length);
            pool0Index = Clamp(pool0Index, poolSets.Length);
            pool1Index = Clamp(pool1Index, poolSets.Length);
            pool2Index = Clamp(pool2Index, poolSets.Length);
            pool3Index = Clamp(pool3Index, poolSets.Length);
            ancillaryIndex = Clamp(ancillaryIndex, ancillarySets.Length);

            var targetPrimary = primaryIndex >= 0 ? primarySets[primaryIndex] : null;
            var targetSecondary = secondaryIndex >= 0 ? secondarySets[secondaryIndex] : null;
            var targetPool0 = pool0Index >= 0 ? poolSets[pool0Index] : null;
            var targetPool1 = pool1Index >= 0 ? poolSets[pool1Index] : null;
            var targetPool2 = pool2Index >= 0 ? poolSets[pool2Index] : null;
            var targetPool3 = pool3Index >= 0 ? poolSets[pool3Index] : null;
            var targetAncillary = ancillaryIndex >= 0 ? ancillarySets[ancillaryIndex] : null;

            // Primary
            if (targetPrimary is not null)
            {
                var currentPrimary = ch.Powersets[0];
                if (toon is not null)
                {
                    if (currentPrimary?.nID != targetPrimary.nID)
                        toon.SwitchSets(targetPrimary, currentPrimary);
                }
                else
                {
                    ch.Powersets[0] = targetPrimary;
                }
            }

            // Secondary: forced vs free
            var linkedSecondaryId = ch.Powersets[0]?.nIDLinkSecondary ?? -1;
            if (linkedSecondaryId > -1)
            {
                // Forced secondary → lock (no clear) and set the forced set
                setSecondaryLocked(true);
                var forced = DatabaseAPI.Database.Powersets[linkedSecondaryId];

                if (toon is not null)
                {
                    var currentSecondary = ch.Powersets[1];
                    if (currentSecondary?.nID != forced?.nID)
                        toon.SwitchSets(forced, currentSecondary);
                }
                else
                {
                    ch.Powersets[1] = forced;
                }
            }
            else
            {
                // Free secondary → unlock and use the user’s selection
                setSecondaryLocked(false);

                if (targetSecondary is not null)
                {
                    if (toon is not null)
                    {
                        var currentSecondary = ch.Powersets[1];
                        if (currentSecondary?.nID != targetSecondary.nID)
                            toon.SwitchSets(targetSecondary, currentSecondary);
                    }
                    else
                    {
                        ch.Powersets[1] = targetSecondary;
                    }
                }
            }

            // Pools
            if (targetPool0 is not null) ch.Powersets[3] = targetPool0;
            if (targetPool1 is not null) ch.Powersets[4] = targetPool1;
            if (targetPool2 is not null) ch.Powersets[5] = targetPool2;
            if (targetPool3 is not null) ch.Powersets[6] = targetPool3;

            // Ancillary
            if (targetAncillary is not null) ch.Powersets[7] = targetAncillary;

            ch.Validate();
        }
    }
}