using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mids_Reborn.Core.Base.Master_Classes
{
    public static class Utilities
    {
        public static class RelLevel
        {
            public static int RelativeLevelToValue(Enums.eEnhRelative relLevel)
            {
                return relLevel switch
                {
                    Enums.eEnhRelative.MinusThree => -3,
                    Enums.eEnhRelative.MinusTwo => -2,
                    Enums.eEnhRelative.MinusOne => -1,
                    Enums.eEnhRelative.PlusOne => 1,
                    Enums.eEnhRelative.PlusTwo => 2,
                    Enums.eEnhRelative.PlusThree => 3,
                    Enums.eEnhRelative.PlusFour => 4,
                    Enums.eEnhRelative.PlusFive => 5,
                    _ => 0
                };
            }

            public static Enums.eEnhRelative ValueToRelativeLevel(int relLevel)
            {
                return relLevel switch
                {
                    <= -3 => Enums.eEnhRelative.MinusThree,
                    -2 => Enums.eEnhRelative.MinusTwo,
                    -1 => Enums.eEnhRelative.MinusOne,
                    0 => Enums.eEnhRelative.Even,
                    1 => Enums.eEnhRelative.PlusOne,
                    2 => Enums.eEnhRelative.PlusTwo,
                    3 => Enums.eEnhRelative.PlusThree,
                    4 => Enums.eEnhRelative.PlusFour,
                    >= 5 => Enums.eEnhRelative.PlusFive
                };
            }

            public static Enums.eEnhRelative UpOne(Enums.eEnhRelative relLevel)
            {
                return relLevel switch
                {
                    Enums.eEnhRelative.MinusThree => Enums.eEnhRelative.MinusTwo,
                    Enums.eEnhRelative.MinusTwo => Enums.eEnhRelative.MinusOne,
                    Enums.eEnhRelative.MinusOne => Enums.eEnhRelative.Even,
                    Enums.eEnhRelative.Even => Enums.eEnhRelative.PlusOne,
                    Enums.eEnhRelative.PlusOne => Enums.eEnhRelative.PlusTwo,
                    Enums.eEnhRelative.PlusTwo => Enums.eEnhRelative.PlusThree,
                    Enums.eEnhRelative.PlusThree => Enums.eEnhRelative.PlusFour,
                    Enums.eEnhRelative.PlusFour => Enums.eEnhRelative.PlusFive,
                    Enums.eEnhRelative.PlusFive => Enums.eEnhRelative.PlusFive,
                    _ => Enums.eEnhRelative.Even
                };
            }

            public static Enums.eEnhRelative DownOne(Enums.eEnhRelative relLevel)
            {
                return relLevel switch
                {
                    Enums.eEnhRelative.MinusThree => Enums.eEnhRelative.MinusThree,
                    Enums.eEnhRelative.MinusTwo => Enums.eEnhRelative.MinusThree,
                    Enums.eEnhRelative.MinusOne => Enums.eEnhRelative.MinusTwo,
                    Enums.eEnhRelative.Even => Enums.eEnhRelative.MinusOne,
                    Enums.eEnhRelative.PlusOne => Enums.eEnhRelative.Even,
                    Enums.eEnhRelative.PlusTwo => Enums.eEnhRelative.PlusOne,
                    Enums.eEnhRelative.PlusThree => Enums.eEnhRelative.PlusTwo,
                    Enums.eEnhRelative.PlusFour => Enums.eEnhRelative.PlusThree,
                    Enums.eEnhRelative.PlusFive => Enums.eEnhRelative.PlusFour,
                    _ => Enums.eEnhRelative.Even
                };
            }

            public static Enums.eEnhRelative Maximum => Enums.eEnhRelative.PlusFive;
            public static Enums.eEnhRelative Minimum => Enums.eEnhRelative.MinusThree;
            public static Enums.eEnhRelative MaximumIo => Enums.eEnhRelative.PlusFive;
            public static Enums.eEnhRelative MaximumSoHo => Enums.eEnhRelative.PlusThree;
            public static Enums.eEnhRelative Neutral => Enums.eEnhRelative.Even;
        }

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

        public static int GetEnhClassById(int id)
        {
            return Math.Max(0, DatabaseAPI.Database.EnhancementClasses.TryFindIndex(e => e.ID == id));
        }

        public static string TimeConverter(int time)
        {
            var words = new[] { "Year", "Month", "Week", "Day", "Hour", "Minute", "Second" };
            var values = new List<string>();

            if (time >= 31536000) // 365 * 86400
            {
                var years = (int)Math.Floor(time / 31536000f);
                time -= years * 31536000;
                values.Add($"{years} {words[0]}{(years == 1 ? "" : "s")}");
            }

            if (time >= 2592000) // 30 * 86400
            {
                var months = (int)Math.Floor(time / 2592000f);
                time -= months * 2592000;
                values.Add($"{months} {words[1]}{(months == 1 ? "" : "s")}");
            }

            if (time >= 604800) // 7 * 86400
            {
                var weeks = (int)Math.Floor(time / 604800f);
                time -= weeks * 604800;
                values.Add($"{weeks} {words[2]}{(weeks == 1 ? "" : "s")}");
            }

            if (time >= 86400) // 24 * 3600
            {
                var days = (int)Math.Floor(time / 86400f);
                time -= days * 86400;
                values.Add($"{days} {words[3]}{(days == 1 ? "" : "s")}");
            }

            if (time >= 3600) // 60 * 60
            {
                var hours = (int)Math.Floor(time / 3600f);
                time -= hours * 3600;
                values.Add($"{hours} {words[4]}{(hours == 1 ? "" : "s")}");
            }

            if (time >= 60)
            {
                var minutes = (int)Math.Floor(time / 60f);
                time -= minutes * 60;
                values.Add($"{minutes} {words[5]}{(minutes == 1 ? "" : "s")}");
            }

            if (time > 0)
            {
                values.Add($"{time} {words[6]}{(time == 1 ? "" : "s")}");
            }

            return string.Join(", ", values);
        }
    }
}