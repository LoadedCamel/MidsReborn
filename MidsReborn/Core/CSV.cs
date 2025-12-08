using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Mids_Reborn.Core
{
    public static class CSV
    {
        private static readonly Regex Reg = new(",(?=(?:[^\"]|\"[^\"]*\")*$)", RegexOptions.CultureInvariant);


        public static string[] ToArray(string iLine)
        {
            var strArray = Reg.Split(iLine);
            char[] chArray = ['"'];
            for (var index = 0; index < strArray.Length; index++)
            {
                strArray[index] = strArray[index].Trim(chArray);
            }

            return strArray;
        }


        internal enum HEffect
        {
            PowerID,
            Table,
            Aspect,
            Attrib,
            Target,
            Scale,
            Type,
            AllowStrength,
            AllowResistance,
            Suppress,
            CancelEvents,
            AllowCombatMobs,
            CostumeName,
            EntityDef,
            PriorityList,
            Delay,
            Duration,
            Magnitude,
            StackType,
            Period,
            Chance,
            NearGround,
            CancelOnMiss,
            VanishOnTimeout,
            RadiusInner,
            RadiusOuter,
            Requires,
            MagnitudeExpr,
            DurationExpr,
            Reward,
            IgnoreSuppressErrors,
            DisplayFloat,
            DisplayAttackerHit,
            DisplayVictimHit,
            Order,
            ShowFloaters,
            ModeName,
            EffectId,
            BoostIgnoreDiminishing,
            BoostTemplate,
            PrimaryStringList,
            SecondaryStringList,
            ApplicationType,
            UseMagnitudeResistance,
            UseDurationResistance,
            UseMagnitudeCombatMods,
            UseDurationCombatMods,
            CasterStackType,
            StackLimit,
            ContinuingFX,
            ConditionalFX,
            Params,
            DisplayOnlyIfNotZero,
            MatchExactPower,
            DoNotTint,
            KeepThroughDeath,
            DelayEval,
            BoostModAllowed,
            EvalFlags,
            ProcsPerMinute
        }

        internal enum BoostSet
        {
            Id,
            DisplayName,
            GroupName,
            MinLevel,
            MaxLevel
        }

        public static string ExportCsv(List<dynamic[]> arr)
        {
            var ret = "";
            var k = 0;
            foreach (var row in arr)
            {
                var line = new List<string>();
                foreach (var item in row)
                {
                    if (item is string)
                    {
                        line.Add($"{item}".Contains(' ') | string.IsNullOrWhiteSpace(item) ? $"\"{item}\"" : $"{item}");
                    }
                    else
                    {
                        line.Add($"{item}");
                    }
                }

                if (k++ > 0)
                {
                    ret += "\r\n";
                }
                
                ret += string.Join(", ", line);
            }

            return ret;
        }
    }
}