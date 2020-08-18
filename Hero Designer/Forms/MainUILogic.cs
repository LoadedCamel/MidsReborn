using System;
using Base.Data_Classes;

namespace Hero_Designer.Forms
{
    // non-mutating OR low var count methods OR anything easily liftable
    // avoid mutation and the service location pattern
    public static class MainUILogic
    {
        public static void ChangeSets(clsToonX toon, Character ch, int primaryIndex, int secondaryIndex, int pool0Index,
            int pool1Index, int pool2Index, int pool3Index, int ancillaryIndex,
            Func<Archetype, Enums.ePowerSetType, IPowerset[]> getPowerSets, Action enableSecondary)
        {
            var at = ch.Archetype;
            var newPrimaryPowerset = getPowerSets(at, Enums.ePowerSetType.Primary)[primaryIndex];
            var ancPowersets = getPowerSets(at, Enums.ePowerSetType.Ancillary);
            if (toon != null)
            {
                var powerset1 = ch.Powersets[0];
                if (powerset1.nID != newPrimaryPowerset.nID)
                    toon.SwitchSets(newPrimaryPowerset, powerset1);
                if (ch.Powersets[0].nIDLinkSecondary > -1)
                {
                    var powerset2 = ch.Powersets[1];
                    var powerset3 = DatabaseAPI.Database.Powersets[ch.Powersets[0].nIDLinkSecondary];
                    if (powerset2.nID != powerset3.nID)
                        toon.SwitchSets(powerset3, powerset2);
                }
                else
                {
                    enableSecondary();
                    var powerset2 = ch.Powersets[1];
                    var secondaryPowersets = getPowerSets(at, Enums.ePowerSetType.Secondary);
                    var newPowerset2 = secondaryPowersets[secondaryIndex];
                    if (powerset2.nID != newPowerset2.nID)
                        toon.SwitchSets(newPowerset2, powerset2);
                }
            }
            else
            {
                var secondaryPowersets = getPowerSets(at, Enums.ePowerSetType.Secondary);
                ch.Powersets[0] = newPrimaryPowerset;
                ch.Powersets[1] = secondaryPowersets[secondaryIndex];
            }

            var poolPowersets = getPowerSets(at, Enums.ePowerSetType.Pool);
            ch.Powersets[3] = poolPowersets[pool0Index];
            ch.Powersets[4] = poolPowersets[pool1Index];
            ch.Powersets[5] = poolPowersets[pool2Index];
            ch.Powersets[6] = poolPowersets[pool3Index];
            if (ancPowersets.Length > 0)
                ch.Powersets[7] = ancPowersets[ancillaryIndex];
            ch.Validate();
        }
    }
}