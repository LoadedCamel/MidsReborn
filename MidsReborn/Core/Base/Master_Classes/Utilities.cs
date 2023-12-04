using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mids_Reborn.Core.Base.Master_Classes
{
    public static class Utilities
    {
        public static string FixDP(float iNum)
        {
            return iNum is < 100 and > -100 ? FixDP(iNum, 2) : FixDP(iNum, 1);
        }

        public static string FixDP(float iNum, int maxDecimal)
        {
            var format = "0.";
            if (iNum is >= 10 or <= -10)
            {
                format = "###0.";
            }

            for (var index = 0; index < maxDecimal; index++)
            {
                format += "#";
            }

            return iNum.ToString(format);
        }

        public static TV ProperEnum<T, TV>(dynamic value) where TV : struct
        {
            var converted = Enum.TryParse<TV>(Enum.GetName(typeof(T), (T)value), out var result);
            
            return converted ? result : new TV();
        }

        public static void ModifiedEffectString(ref string effectString, int collection)
        {
            var matchCollection1 = new Dictionary<string, string>
            {
                { @"(DamageBuff\(Smashing\), \d.*% DamageBuff\(Lethal\), \d.*% DamageBuff\(Fire\), \d.*% DamageBuff\(Cold\), \d.*% DamageBuff\(Energy\), \d.*% DamageBuff\(Negative\), \d.*% DamageBuff\(Toxic\), \d.*% DamageBuff\(Psionic\))", @"DamageBuff(All)" },
                { @"(Defense\(Melee\), \d.*% Defense\(AoE\), \d.*% Defense\(Ranged\), \d.*% Defense\(Smashing\), \d.*% Defense\(Lethal\), \d.*% Defense\(Fire\), \d.*% Defense\(Cold\), \d.*% Defense\(Energy\), \d.*% Defense\(Negative\), \d.*% Defense\(Psionic\))", @"Defense(All)" },
                { @"(Resistance\((Smashing|Lethal)\), \d.*% Resistance\((Smashing|Lethal)\), \d.*% Resistance\(Fire\), \d.*% Resistance\(Cold\), \d.*% Resistance\(Energy\), \d.*% Resistance\(Negative\), \d.*% Resistance\((Psionic|Toxic)\), \d.*% Resistance\((Toxic|Psionic)\)|Resistance\(Smashing,Fire,Cold,Energy,Negative,Toxic,Psionic\).*Resistance\(Lethal\))", @"Resistance(All)" },
                { @"(MezResist\(Immobilized\).*MezResist\(Held\).*MezResist\(Stunned\).*MezResist\(Sleep\).*MezResist\(Terrorized\).*MezResist\(Confused\))", @"MezResist(All)" },
                { @"(SpeedJumping.*JumpHeight.*SpeedFlying.*SpeedRunning)", @"Movement Speed" },
                { @"(Knockback.*\(Mag.-)", @"Knockback Protection (Mag " }, 
                { @"(Knockback.*Knockup.*protection)", @"Knockback Protection"},
                { @"Defense\(Smashing,Lethal,Fire,Cold,Energy,Negative,Toxic,Psionic\)", @"Defense(All)"},
                { @"Defense\(Smashing,Lethal,Fire,Cold,Energy,Negative,Psionic\)", @"Defense(All)"}

            };

            var matchCollection2 = new Dictionary<string, string>
            {
                { @"(DamageBuff\(Smashing\), \d.*% DamageBuff\(Lethal\), \d.*% DamageBuff\(Fire\), \d.*% DamageBuff\(Cold\), \d.*% DamageBuff\(Energy\), \d.*% DamageBuff\(Negative\), \d.*% DamageBuff\(Toxic\), \d.*% DamageBuff\(Psionic\))", @"DamageBuff(All)" },
                { @"(Defense\(Melee\), \d.*% Defense\(AoE\), \d.*% Defense\(Ranged\), \d.*% Defense\(Smashing\), \d.*% Defense\(Lethal\), \d.*% Defense\(Fire\), \d.*% Defense\(Cold\), \d.*% Defense\(Energy\), \d.*% Defense\(Negative\), \d.*% Defense\(Psionic\))", @"Defense(All)" },
                { @"(Resistance\((Smashing|Lethal)\), \d.*% Resistance\((Smashing|Lethal)\), \d.*% Resistance\(Fire\), \d.*% Resistance\(Cold\), \d.*% Resistance\(Energy\), \d.*% Resistance\(Negative\), \d.*% Resistance\((Psionic|Toxic)\), \d.*% Resistance\((Toxic|Psionic)\)|Resistance\(Smashing,Fire,Cold,Energy,Negative,Toxic,Psionic\).*Resistance\(Lethal\))", @" Resistance(All)" },
                { @"(MezResist\(Immobilized\).*MezResist\(Held\).*MezResist\(Stunned\).*MezResist\(Sleep\).*MezResist\(Terrorized\).*MezResist\(Confused\))", @"Status Resistance" },
                { @"(SpeedJumping.*JumpHeight.*SpeedFlying.*SpeedRunning)", @"Movement Speed" },
                { @"(Knockback.*\(Mag.-)", @"Knockback Protection (Mag " },
                { @"(Knockback.*Knockup.*protection)", @"Knockback Protection"},
                { @"Defense\(Smashing,Lethal,Fire,Cold,Energy,Negative,Toxic,Psionic\)", @"Defense(All)"},
                { @"Defense\(Smashing,Lethal,Fire,Cold,Energy,Negative,Psionic\)", @"Defense(All)"}
            };

            effectString = collection switch
            {
                1 => matchCollection1.Aggregate(effectString, (current, regMatch) => Regex.Replace(current, regMatch.Key, regMatch.Value)),
                2 => matchCollection2.Aggregate(effectString, (current, regMatch) => Regex.Replace(current, regMatch.Key, regMatch.Value)),
                _ => effectString
            };
        }
    }
}