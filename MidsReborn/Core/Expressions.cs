using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Jace;
using Mids_Reborn.Core.Base.Master_Classes;
using static Mids_Reborn.Core.Base.Data_Classes.Character;

namespace Mids_Reborn.Core
{
    public class Expressions
    {
        public string Duration { get; set; } = "";
        public string Magnitude { get; set; } = "";
        public string Probability { get; set; } = "";

        public struct ErrorData
        {
            public ExpressionType Type { get; set; }
            public bool Found { get; set; }
            public string Message { get; set; }
        }

        public struct ExprCommand
        {
            public string Keyword { get; set; }
            public ExprKeywordType KeywordType { get; set; }
            public ExprKeywordInfix InfixMode { get; set; }
            public ExprCommandToken CommandTokenType { get; set; }
            public bool SingleToken { get; set; }
        }

        public enum ExprKeywordType
        {
            Keyword,
            Function
        }

        public enum ExprCommandToken
        {
            None,
            Numeric,
            ExpressionNumeric,
            Modifier,
            AttackVector,
            PowerName,
            PowerGroup, // Groups
            PowerGroupPrefix, // Groups + Powersets
            ArchetypeName
        }

        public enum ExprKeywordInfix
        {
            Atomic,
            Prefix,
            Suffix
        }

        public enum ExpressionType
        {
            Duration,
            Magnitude,
            Probability
        }

        public static readonly List<ExprCommand> CommandsList = new()
        {
            new ExprCommand
            {
                Keyword = "power.base>activateperiod",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "power.base>activatetime",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "power.base>areafactor",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "power.base>rechargetime",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "power.base>endcost",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "power.base>range",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "effect>scale",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "@StdResult",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "@Strength",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "ifPvE",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "ifPvP",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "modifier>current",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "caster>modifier(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.Modifier,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "caster>modifier>current",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "maxEndurance",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "rand()",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "source.ownPower?(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Prefix,
                CommandTokenType = ExprCommandToken.PowerName,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "source.ownPowerNum?(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Prefix,
                CommandTokenType = ExprCommandToken.ExpressionNumeric,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "source>Base.kHitPoints",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "source>Max.kHitPoints",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = ">variableVal",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Suffix,
                CommandTokenType = ExprCommandToken.PowerName,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "modifier>",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Prefix,
                CommandTokenType = ExprCommandToken.Modifier,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "powerGroupIn(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Prefix,
                CommandTokenType = ExprCommandToken.PowerGroupPrefix,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "powerGroupNotIn(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Prefix,
                CommandTokenType = ExprCommandToken.PowerGroupPrefix,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "powerIs(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Prefix,
                CommandTokenType = ExprCommandToken.PowerName,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "powerIsNot(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Prefix,
                CommandTokenType = ExprCommandToken.PowerName,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "powerVectorsContains(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Prefix,
                CommandTokenType = ExprCommandToken.AttackVector,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "source.owner>arch(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Prefix,
                CommandTokenType = ExprCommandToken.ArchetypeName,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "source.owner>archIn(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Prefix,
                CommandTokenType = ExprCommandToken.ArchetypeName,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "eq(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Prefix,
                CommandTokenType = ExprCommandToken.ExpressionNumeric,
                SingleToken = false
            },
            new ExprCommand
            {
                Keyword = "ne(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Prefix,
                CommandTokenType = ExprCommandToken.ExpressionNumeric,
                SingleToken = false
            },
            new ExprCommand
            {
                Keyword = "gt(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Prefix,
                CommandTokenType = ExprCommandToken.ExpressionNumeric,
                SingleToken = false
            },
            new ExprCommand
            {
                Keyword = "gte(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Prefix,
                CommandTokenType = ExprCommandToken.ExpressionNumeric,
                SingleToken = false
            },
            new ExprCommand
            {
                Keyword = "lt(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = false
            },
            new ExprCommand
            {
                Keyword = "lte(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = false
            },
            new ExprCommand
            {
                Keyword = "minmax(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.ExpressionNumeric,
                SingleToken = false
            },
            new ExprCommand
            {
                Keyword = "and(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = false
            },
            new ExprCommand
            {
                Keyword = "or(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = false
            },
            new ExprCommand
            {
                Keyword = "GCMActive(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "GCMScale(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "source>kMeter",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "source>kMeterAbs",
                KeywordType = ExprKeywordType.Keyword,
                InfixMode = ExprKeywordInfix.Atomic,
                CommandTokenType = ExprCommandToken.None,
                SingleToken = true
            },
            new ExprCommand
            {
                Keyword = "powerActive(",
                KeywordType = ExprKeywordType.Function,
                InfixMode = ExprKeywordInfix.Prefix,
                CommandTokenType = ExprCommandToken.PowerName,
                SingleToken = false
            }
        };

        private static Dictionary<string, string> CommandsDict(IEffect sourceFx)
        {
            var fxPower = sourceFx.GetPower();

            return new Dictionary<string, string>
            {
                { "power.base>activateperiod", $"{(fxPower == null ? "0" : fxPower.ActivatePeriod)}"},
                { "power.base>activatetime", $"{(fxPower == null ? "0" : fxPower.CastTime)}" },
                { "power.base>areafactor", $"{(fxPower == null ? "0" : fxPower.AoEModifier)}" },
                { "power.base>rechargetime", $"{(fxPower == null ? "0" : fxPower.BaseRechargeTime)}" },
                { "power.base>endcost", $"{(fxPower == null ? "0" : fxPower.EndCost)}" },
                { "power.base>range", $"{(fxPower == null ? "0" : fxPower.Range)}" },
                { "effect>scale", $"{sourceFx.Scale}" },
                { "@StdResult", $"{sourceFx.Scale}" },
                { "@Strength", $"{GetStrength(sourceFx)}"},
                { "ifPvE", sourceFx.PvMode == Enums.ePvX.PvE ? "1" : "0" },
                { "ifPvP", sourceFx.PvMode == Enums.ePvX.PvP ? "1" : "0" },
                { "caster>modifier>current", ModifierCaster(sourceFx) },
                { "modifier>current", $"{DatabaseAPI.GetModifier(sourceFx)}" },
                { "maxEndurance", $"{MidsContext.Character.DisplayStats.EnduranceMaxEnd}" },
                { "rand()", $"{sourceFx.Rand}" },
                { "cur.kToHit", $"{MidsContext.Character.DisplayStats.BuffToHit}"},
                { "base.kToHit", $"{(MidsContext.Config.ScalingToHit)}"},
                { "source>Max.kHitPoints", $"{MidsContext.Character.Totals.HPMax}" },
                { "source>Base.kHitPoints", $"{(MidsContext.Character.Archetype == null ? 1000 : MidsContext.Character.Archetype.Hitpoints)}"},
                { "source>cur.kMeter", $"{(fxPower == null ? "0" : GetVariableValue(fxPower.FullName, false))}"},
                { "source>cur.kMeterAbs", $"{(fxPower == null ? "0" : GetVariableValue(fxPower.FullName))}"}
            };
        }

        private static Dictionary<Regex, MatchEvaluator> FunctionsDict(IEffect sourceFx, List<string?> pickedPowerNames)
        {
            var fxPower = sourceFx.GetPower();

            return new Dictionary<Regex, MatchEvaluator>
            {
                { new Regex(@"source\.ownPower\?\(([a-zA-Z0-9_\-\.]+)\)"), e => pickedPowerNames.Contains(e.Groups[1].Value) ? "1" : "0" },
                { new Regex(@"source\.ownPowerNum\?\(([a-zA-Z0-9_\-\.]+)\)"), e => OwnPowerNumCheck(e.Groups[1].Value) },
                { new Regex(@"([a-zA-Z\-_\.]+)>variableVal"), e => GetVariableValue(e.Groups[1].Value) },
                { new Regex(@"modifier\>([a-zA-Z0-9_\-]+)"), e => GetModifier(e.Groups[1].Value) },
                { new Regex(@"powerGroupIn\(([a-zA-Z0-9_\-\.]+)\)"), e => fxPower == null ? "0" : fxPower.FullName.StartsWith(e.Groups[1].Value) ? "1" : "0" },
                { new Regex(@"powerGroupNotIn\(([a-zA-Z0-9_\-\.]+)\)"), e => fxPower == null ? "0" : fxPower.FullName.StartsWith(e.Groups[1].Value) ? "0" : "1" },
                { new Regex(@"powerIs\(([a-zA-Z0-9_\-\.]+)\)"), e => fxPower == null ? "0" : fxPower.FullName.Equals(e.Groups[1].Value, StringComparison.InvariantCultureIgnoreCase) ? "1" : "0" },
                { new Regex(@"powerIsNot\(([a-zA-Z0-9_\-\.]+)\)"), e => fxPower == null ? "0" : fxPower.FullName.Equals(e.Groups[1].Value, StringComparison.InvariantCultureIgnoreCase) ? "0" : "1" },
                { new Regex(@"powerVectorsContains\(([a-zA-Z0-9_\-\.]+)\)"), e => PowerVectorsContains(sourceFx.GetPower(), e.Groups[1].Value) },
                { new Regex(@"source\.owner\>arch\(([a-zA-Z\s]+)\)"), e => MidsContext.Character.Archetype == null ? "0" : MidsContext.Character.Archetype.DisplayName.Equals(e.Groups[1].Value, StringComparison.InvariantCultureIgnoreCase) ? "1" : "0" },
                { new Regex(@"source\.owner\>archIn\(([a-zA-Z\s\,]+)\)"), e => MidsContext.Character.Archetype == null ? "0" : Regex.Split(e.Groups[1].Value, @"(\s*)\,").Select(f => f.ToLowerInvariant().Trim()).Contains(MidsContext.Character.Archetype.DisplayName.ToLowerInvariant()) ? "1" : "0" },
                { new Regex(@"caster\>modifier\(([a-zA-Z0-9_\-]+)\)"), e => ModifierCaster(e.Groups[1].Value) },
                { new Regex(@"GCMActive\(([a-zA-Z0-9_\-]+)\)"), e => CheckGCM(e.Groups[1].Value) },
                { new Regex(@"GCMScale\(([a-zA-Z0-9_\-]+)\)"), e => GCMScale(e.Groups[1].Value) },
                { new Regex(@"powerActive\(([a-zA-Z0-9_\-\.]+)\)"), e => IsPowerActive(e.Groups[1].Value) ? "1" : "0" }
            };
        }

        private static string OwnPowerNumCheck(string powerName)
        {
            var power = MidsContext.Character.CurrentBuild?.Powers.FirstOrDefault(p => p is { Power: not null } && p.Power.FullName.Equals(powerName, StringComparison.InvariantCultureIgnoreCase));
            return power != null ? "1" : "0";
        }

        private static string CheckGCM(string gcm)
        {
            return MidsContext.Character.ModifyEffects.ContainsKey(gcm) ? "1" : "0";
        }

        private static string GCMScale(string gcm)
        {
            return MidsContext.Character.ModifyEffects.TryGetValue(gcm, out var gcmScale)
                ? $"{gcmScale}"
                : "0";
        }

        private static string ModifierCaster(IEffect effect)
        {
            return ModifierCaster(effect.ModifierTable);
        }

        private static string ModifierCaster(string modifier)
        {
            const int maxPlayerLevel = 50;

            var modifierData = DatabaseAPI.Database.AttribMods.Modifier
                .DefaultIfEmpty(new Modifiers.ModifierTable())
                .FirstOrDefault(e => string.Equals(e.ID, modifier, StringComparison.InvariantCultureIgnoreCase));

            if (modifierData == null)
            {
                return "0";
            }

            if (modifierData.Table.Count <= 0)
            {
                return "0";
            }

            var archetypeNid = DatabaseAPI.Database.Classes
                .Where(e => e is {Playable: true})
                .Select((e, i) => new KeyValuePair<int, string>(i, e.DisplayName))
                .DefaultIfEmpty(new KeyValuePair<int, string>(-1, ""))
                .FirstOrDefault(e => e.Value == MidsContext.Character?.Archetype?.DisplayName)
                .Key;
            
            return archetypeNid < 0
                ? "0"
                : Convert.ToString(modifierData.Table[maxPlayerLevel - 1][archetypeNid], CultureInfo.InvariantCulture);
        }

        private static string PowerVectorsContains(IPower? sourcePower, string vector)
        {
            var ret = Enum.TryParse(typeof(Enums.eVector), vector, out var eValue);

            return !ret
                ? "0"
                : (sourcePower.AttackTypes & (Enums.eVector) eValue) == Enums.eVector.None
                    ? "0"
                    : "1";
        }

        private static bool IsPowerActive(string powerName)
        {
            var power = MidsContext.Character.CurrentBuild?.Powers.FirstOrDefault(p => p?.Power != null && p.Power.FullName.Equals(powerName, StringComparison.InvariantCultureIgnoreCase));
            
            return power?.Power is { Active: true };
        }

        private static float GetStrength(IEffect? sourceEffect)
        {
            if (sourceEffect is null)
            {
                return 0;
            }

            return sourceEffect.Scale;
        }

        private static string GetModifier(string modifierName)
        {
            var modTable = DatabaseAPI.Database.AttribMods.Modifier.Where(e => e.ID == modifierName).ToList();

            return modTable.Count <= 0 ?
                "0" :
                $"{modTable[0].Table[MidsContext.Character.Level][MidsContext.Character.Archetype.Column]}";
        }

        private static string GetVariableValue(string powerName, bool absoluteValue = true)
        {
            var target = MidsContext.Character.CurrentBuild.Powers.FirstOrDefault(x => x is {Power: not null} && x.Power.FullName == powerName);

            return target == null
                ? "0"
                : absoluteValue
                    ? $"{target.VariableValue}"
                    : $"{target.VariableValue * target.Power.VariableMax / 100.0}";
        }

        public static float Parse(IEffect sourceFx, ExpressionType exprType, out ErrorData error)
        {
            error = new ErrorData();
            float retValue;
            switch (exprType)
            {
                case ExpressionType.Duration:
                    retValue = InternalParsing(sourceFx, exprType, out error);
                    break;

                case ExpressionType.Probability:
                    retValue = InternalParsing(sourceFx, exprType, out error);
                    break;

                case ExpressionType.Magnitude:
                    if (sourceFx.Expressions.Magnitude.IndexOf(".8 rechargetime power.base> 1 30 minmax * 1.8 + 2 * @StdResult * 10 / areafactor power.base> /", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        retValue = (float)((Math.Max(Math.Min(sourceFx.GetPower().RechargeTime, 30f), 0) * 0.800000011920929 + 1.79999995231628) / 5.0) / sourceFx.GetPower().AoEModifier * sourceFx.Scale;
                        if (sourceFx.Expressions.Magnitude.Length > ".8 rechargetime power.base> 1 30 minmax * 1.8 + 2 * @StdResult * 10 / areafactor power.base> /".Length + 2)
                        {
                            retValue *= float.Parse(sourceFx.Expressions.Magnitude[(".8 rechargetime power.base> 1 30 minmax * 1.8 + 2 * @StdResult * 10 / areafactor power.base> /".Length + 1)..][..2]);
                        }

                        return retValue;
                    }

                    if (string.IsNullOrWhiteSpace(sourceFx.Expressions.Magnitude))
                    {
                        return 0;
                    }

                    var baseFx = sourceFx.Clone() as IEffect;
                    retValue = InternalParsing(baseFx, exprType, out error);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(exprType), exprType, null);
            }

            return error.Found ? 0 : retValue;
        }

        private static float InternalParsing(IEffect sourceFx, ExpressionType exprType, out ErrorData error)
        {
            var pickedPowerNames = MidsContext.Character.CurrentBuild == null ? new List<string?>() : MidsContext.Character.CurrentBuild.Powers.Select(pe => pe?.Power?.FullName).ToList();

            error = new ErrorData();
            var mathEngine = CalculationEngine.New<double>();
            var expr = exprType switch
            {
                ExpressionType.Duration => sourceFx.Expressions.Duration,
                ExpressionType.Magnitude => sourceFx.Expressions.Magnitude,
                ExpressionType.Probability => sourceFx.Expressions.Probability,
                _ => throw new ArgumentOutOfRangeException(nameof(exprType), exprType, null)
            };

            // Numeric functions
            mathEngine.AddFunction("eq", (a, b) => Math.Abs(a - b) < double.Epsilon ? 1 : 0);
            mathEngine.AddFunction("ne", (a, b) => Math.Abs(a - b) > double.Epsilon ? 1 : 0);
            mathEngine.AddFunction("gt", (a, b) => a > b ? 1 : 0);
            mathEngine.AddFunction("gte", (a, b) => a >= b ? 1 : 0);
            mathEngine.AddFunction("lt", (a, b) => a < b ? 1 : 0);
            mathEngine.AddFunction("lte", (a, b) => a <= b ? 1 : 0);
            mathEngine.AddFunction("minmax", (a, b, c) => Math.Min(b > c ? b : c, Math.Max(b > c ? c : b, a)));

            // Logical functions
            mathEngine.AddFunction("and", (a, b) => (a != 0 && b != 0) ? 1 : 0);
            mathEngine.AddFunction("or", (a, b) => (a != 0 || b != 0) ? 1 : 0);

            // Constants
            expr = CommandsDict(sourceFx).Aggregate(expr, (current, cmd) => current.Replace(cmd.Key, cmd.Value));

            // Non-numeric functions
            expr = FunctionsDict(sourceFx, pickedPowerNames).Aggregate(expr, (current, f1) => f1.Key.Replace(current, f1.Value));

            try
            {
                return (float)mathEngine.Calculate(expr);
            }
            catch (ParseException ex)
            {
                Debug.WriteLine($"Expression failed in {expr}\n  Power: {sourceFx.GetPower()?.FullName}");

                error.Type = exprType;
                error.Found = true;
                error.Message = ex.Message;

                return 0;
            }
            catch (VariableNotDefinedException ex)
            {
                Debug.WriteLine($"Expression failed (variable not defined) in {expr}\nPower: {sourceFx.GetPower()?.FullName}");

                error.Type = exprType;
                error.Found = true;
                error.Message = ex.Message;

                return 0;
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine($"Expression failed (invalid operation) in {expr}\nPower: {sourceFx.GetPower()?.FullName}");

                error.Type = exprType;
                error.Found = true;
                error.Message = ex.Message;

                return 0;
            }
        }

        public struct Validation
        {
            public ExpressionType Type { get; set; }
            public bool Validated { get; set; }
            public string Message { get; set; }
        }

        public static void Validate(IEffect fx, out List<Validation> validationItems)
        {
            ErrorData error;
            ExpressionType type;
            validationItems = new List<Validation>();

            if (!string.IsNullOrWhiteSpace(fx.Expressions.Duration))
            {
                type = ExpressionType.Duration;
                Parse(fx, type, out error);
                if (error.Found)
                {
                    validationItems.Add(new Validation
                    {
                        Type = error.Type,
                        Validated = false,
                        Message = $"{type} Expression Error: {error.Message}"
                    });
                }
                else
                {
                    validationItems.Add(new Validation
                    {
                        Type = type,
                        Validated = true
                    });
                }
            }

            if (!string.IsNullOrWhiteSpace(fx.Expressions.Magnitude))
            {
                type = ExpressionType.Magnitude;
                Parse(fx, type, out error);
                if (error.Found)
                {
                    validationItems.Add(new Validation
                    {
                        Type = error.Type,
                        Validated = false,
                        Message = $"{type} Expression Error: {error.Message}"
                    });
                }
                else
                {
                    validationItems.Add(new Validation
                    {
                        Type = type,
                        Validated = true
                    });
                }
            }

            if (string.IsNullOrWhiteSpace(fx.Expressions.Probability))
            {
                return;
            }

            type = ExpressionType.Probability;
            Parse(fx, type, out error);
            if (error.Found)
            {
                validationItems.Add(new Validation
                {
                    Type = error.Type,
                    Validated = false,
                    Message = $"{type} Expression Error: {error.Message}"
                });
            }
            else
            {
                validationItems.Add(new Validation
                {
                    Type = type,
                    Validated = true
                });
            }
        }

        private static readonly Dictionary<Enums.eEffectType, string> EffectTotalMap = new()
        {
            { Enums.eEffectType.Accuracy, nameof(TotalStatistics.BuffAcc) },
            { Enums.eEffectType.DamageBuff, nameof(TotalStatistics.BuffDam) },
            { Enums.eEffectType.Defense, nameof(TotalStatistics.Def) },
            { Enums.eEffectType.EnduranceDiscount, nameof(TotalStatistics.BuffEndRdx) },
            { Enums.eEffectType.SpeedFlying, nameof(TotalStatistics.FlySpd) },
            { Enums.eEffectType.HitPoints, nameof(TotalStatistics.HPMax) },
            { Enums.eEffectType.JumpHeight, nameof(TotalStatistics.JumpHeight) },
            { Enums.eEffectType.SpeedJumping, nameof(TotalStatistics.JumpSpd) },
            { Enums.eEffectType.Mez, nameof(TotalStatistics.Mez) },
            { Enums.eEffectType.MezResist, nameof(TotalStatistics.MezRes) },
            { Enums.eEffectType.PerceptionRadius, nameof(TotalStatistics.Perception) },
            { Enums.eEffectType.RechargeTime, nameof(TotalStatistics.BuffHaste) },
            { Enums.eEffectType.Recovery, nameof(TotalStatistics.EndRec) },
            { Enums.eEffectType.Regeneration, nameof(TotalStatistics.HPRegen) },
            { Enums.eEffectType.ResEffect, nameof(TotalStatistics.DebuffRes) },
            { Enums.eEffectType.Resistance, nameof(TotalStatistics.Res) },
            { Enums.eEffectType.SpeedRunning, nameof(TotalStatistics.RunSpd) },
            { Enums.eEffectType.StealthRadius, nameof(TotalStatistics.StealthPvE) },
            { Enums.eEffectType.StealthRadiusPlayer, nameof(TotalStatistics.StealthPvP) },
            { Enums.eEffectType.ThreatLevel, nameof(TotalStatistics.ThreatLevel) },
            { Enums.eEffectType.ToHit, nameof(TotalStatistics.BuffToHit) },
            { Enums.eEffectType.Elusivity, nameof(TotalStatistics.Elusivity) },
            { Enums.eEffectType.MaxRunSpeed, nameof(TotalStatistics.MaxRunSpd) },
            { Enums.eEffectType.MaxJumpSpeed, nameof(TotalStatistics.MaxJumpSpd) },
            { Enums.eEffectType.MaxFlySpeed, nameof(TotalStatistics.MaxFlySpd) },
            { Enums.eEffectType.Absorb, nameof(TotalStatistics.Absorb) },
            { Enums.eEffectType.Stealth, nameof(TotalStatistics.StealthPvE) }
        };
    }
}
