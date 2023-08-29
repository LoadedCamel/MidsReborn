using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Jace;
using Mids_Reborn.Core.Base.Master_Classes;

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
                { "ifPvE", sourceFx.PvMode == Enums.ePvX.PvE ? "1" : "0" },
                { "ifPvP", sourceFx.PvMode == Enums.ePvX.PvP ? "1" : "0" },
                { "caster>modifier>current", ModifierCaster(sourceFx) },
                { "modifier>current", $"{DatabaseAPI.GetModifier(sourceFx)}" },
                { "maxEndurance", $"{MidsContext.Character.DisplayStats.EnduranceMaxEnd}" },
                { "rand()", $"{sourceFx.Rand}" },
                { "cur.kToHit", $"{MidsContext.Character.DisplayStats.BuffToHit}"},
                { "base.kToHit", $"{(MidsContext.Config.ScalingToHit)}"},
                { "source>Max.kHitPoints", $"{MidsContext.Character.Totals.HPMax}" },
                { "source>Base.kHitPoints", $"{(MidsContext.Character.Archetype == null ? 1000 : MidsContext.Character.Archetype.Hitpoints)}"}
            };
        }

        private static Dictionary<Regex, MatchEvaluator> FunctionsDict(IEffect sourceFx, List<string?> pickedPowerNames)
        {
            var fxPower = sourceFx.GetPower();

            return new Dictionary<Regex, MatchEvaluator>
            {
                { new Regex(@"source\.ownPower\?\(([a-zA-Z0-9_\-\.]+)\)"), e => pickedPowerNames.Contains(e.Groups[1].Value) ? "1" : "0" },
                { new Regex(@"([a-zA-Z\-_\.]+)>variableVal"), e => GetVariableValue(e.Groups[1].Value) },
                { new Regex(@"modifier\>([a-zA-Z0-9_\-]+)"), e => GetModifier(e.Groups[1].Value) },
                { new Regex(@"powerGroupIn\(([a-zA-Z0-9_\-\.]+)\)"), e => fxPower == null ? "0" : fxPower.FullName.StartsWith(e.Groups[1].Value) ? "1" : "0" },
                { new Regex(@"powerGroupNotIn\(([a-zA-Z0-9_\-\.]+)\)"), e => fxPower == null ? "0" : fxPower.FullName.StartsWith(e.Groups[1].Value) ? "0" : "1" },
                { new Regex(@"powerIs\(([a-zA-Z0-9_\-\.]+)\)"), e => fxPower == null ? "0" : fxPower.FullName.Equals(e.Groups[1].Value, StringComparison.InvariantCultureIgnoreCase) ? "1" : "0" },
                { new Regex(@"powerIsNot\(([a-zA-Z0-9_\-\.]+)\)"), e => fxPower == null ? "0" : fxPower.FullName.Equals(e.Groups[1].Value, StringComparison.InvariantCultureIgnoreCase) ? "0" : "1" },
                { new Regex(@"powerVectorsContains\(([a-zA-Z0-9_\-\.]+)\)"), e => PowerVectorsContains(sourceFx.GetPower(), e.Groups[1].Value) },
                { new Regex(@"source\.owner\>arch\(([a-zA-Z\s]+)\)"), e => MidsContext.Character.Archetype == null ? "0" : MidsContext.Character.Archetype.DisplayName.Equals(e.Groups[1].Value, StringComparison.InvariantCultureIgnoreCase) ? "1" : "0" },
                { new Regex(@"source\.owner\>archIn\(([a-zA-Z\s\,]+)\)"), e => MidsContext.Character.Archetype == null ? "0" : Regex.Split(e.Groups[1].Value, @"(\s*)\,").Select(f => f.ToLowerInvariant().Trim()).Contains(MidsContext.Character.Archetype.DisplayName.ToLowerInvariant()) ? "1" : "0" },
                { new Regex(@"caster\>modifier\(([a-zA-Z0-9_\-]+)\)"), e => ModifierCaster(e.Groups[1].Value) }
            };
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
            
            if (archetypeNid < 0)
            {
                return "0";
            }

            return Convert.ToString(modifierData.Table[maxPlayerLevel - 1][archetypeNid], CultureInfo.InvariantCulture);
        }

        private static string PowerVectorsContains(IPower? sourcePower, string vector)
        {
            var ret = Enum.TryParse(typeof(Enums.eVector), vector, out var eValue);

            if (!ret)
            {
                return "0";
            }

            return (sourcePower.AttackTypes & (Enums.eVector) eValue) == Enums.eVector.None ? "0" : "1";
        }

        private static string GetModifier(string modifierName)
        {
            var modTable = DatabaseAPI.Database.AttribMods.Modifier.Where(e => e.ID == modifierName).ToList();

            return modTable.Count <= 0
                ? "0"
                : $"{Math.Abs(modTable[0].Table[MidsContext.Character.Level][MidsContext.Character.Archetype.Column])}";
        }

        private static string GetVariableValue(string powerName)
        {
            var target = MidsContext.Character.CurrentBuild.Powers.FirstOrDefault(x => x.Power != null && x.Power.FullName == powerName);
            return target == null ? "0" : $"{target.VariableValue}";
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
                            retValue *= float.Parse(sourceFx.Expressions.Magnitude.Substring(".8 rechargetime power.base> 1 30 minmax * 1.8 + 2 * @StdResult * 10 / areafactor power.base> /".Length + 1)[..2]);
                        }

                        return retValue;
                    }

                    if (string.IsNullOrWhiteSpace(sourceFx.Expressions.Magnitude)) return 0f;

                    var baseFx = sourceFx.Clone() as IEffect;
                    retValue = InternalParsing(baseFx, exprType, out error);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(exprType), exprType, null);
            }

            return error.Found ? 0f : retValue;
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

            // Constants
            expr = CommandsDict(sourceFx).Aggregate(expr, (current, cmd) => current.Replace(cmd.Key, cmd.Value));

            // Non-numeric functions
            expr = FunctionsDict(sourceFx, pickedPowerNames).Aggregate(expr, (current, f1) => f1.Key.Replace(current, f1.Value));

            // Numeric functions
            mathEngine.AddFunction("eq", (a, b) => Math.Abs(a - b) < double.Epsilon ? 1 : 0);
            mathEngine.AddFunction("ne", (a, b) => Math.Abs(a - b) > double.Epsilon ? 1 : 0);
            mathEngine.AddFunction("gt", (a, b) => a > b ? 1 : 0);
            mathEngine.AddFunction("gte", (a, b) => a >= b ? 1 : 0);
            mathEngine.AddFunction("lt", (a, b) => a < b ? 1 : 0);
            mathEngine.AddFunction("lte", (a, b) => a <= b ? 1 : 0);
            mathEngine.AddFunction("minmax", (a, b, c) => Math.Min(b > c ? b : c, Math.Max(b > c ? c : b, a)));

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

            if (!string.IsNullOrWhiteSpace(fx.Expressions.Probability))
            {
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
        }
    }
}
