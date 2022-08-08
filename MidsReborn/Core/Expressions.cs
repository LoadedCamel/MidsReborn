using System;
using System.Collections.Generic;
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

        public enum ExpressionType
        {
            Duration,
            Magnitude,
            Probability
        }

        public static readonly List<string> CommandsList = new()
        {
            "power.base>activateperiod",
            "power.base>activatetime",
            "power.base>areafactor",
            "power.base>rechargetime",
            "power.base>endcost",
            "effect>scale",
            "@StdResult",
            "ifPvE",
            "ifPvP",
            "modifier>current",
            "maxEndurance",
            "rand()",
            "source.ownPower?(",
            "source>Base.kHitPoints",
            "source>Max.kHitPoints",
            ">variableVal",
            "modifier>",
            "powerGroupIn(",
            "powerGroupNotIn(",
            "powerIs(",
            "powerIsNot(",
            "eq(",
            "ne(",
            "minmax(",
        };


        private static Dictionary<string, string> CommandsDict(IEffect sourceFx)
        {
            return new Dictionary<string, string>
            {
                { "power.base>activateperiod", $"{sourceFx.GetPower().ActivatePeriod}"},
                { "power.base>activatetime", $"{sourceFx.GetPower().CastTime}" },
                { "power.base>areafactor", $"{sourceFx.GetPower().AoEModifier}" },
                { "power.base>rechargetime", $"{sourceFx.GetPower().BaseRechargeTime}" },
                { "power.base>endcost", $"{sourceFx.GetPower().EndCost}" },
                { "effect>scale", $"{sourceFx.Scale}" },
                { "@StdResult", $"{sourceFx.Scale}" },
                { "ifPvE", sourceFx.PvMode == Enums.ePvX.PvE ? "1" : "0" },
                { "ifPvP", sourceFx.PvMode == Enums.ePvX.PvP ? "1" : "0" },
                { "modifier>current", $"{DatabaseAPI.GetModifier(sourceFx)}" },
                { "maxEndurance", $"{MidsContext.Character.DisplayStats.EnduranceMaxEnd}" },
                { "rand()", $"{sourceFx.Rand}" },
                { "cur.kToHit", $"{MidsContext.Character.DisplayStats.BuffToHit}"},
                { "base.kToHit", $"{DatabaseAPI.ServerData.BaseToHit}"},
                { "source>Max.kHitPoints", $"{MidsContext.Character.Totals.HPMax}" },
                { "source>Base.kHitPoints", $"{MidsContext.Character.Archetype.Hitpoints}"}
            };
        }

        private static Dictionary<Regex, MatchEvaluator> FunctionsDict(IEffect sourceFx, ICollection<string> pickedPowerNames)
        {
            return new Dictionary<Regex, MatchEvaluator>
            {
                { new Regex(@"source\.ownPower\?\(([a-zA-Z0-9_\-\.]+)\)"), e => pickedPowerNames.Contains(e.Groups[1].Value) ? "1" : "0" },
                { new Regex(@"([a-zA-Z\-_\.]+)>variableVal"), e => GetVariableValue(e.Groups[1].Value) },
                { new Regex(@"modifier\>([a-zA-Z0-9_\-]+)"), e => GetModifier(e.Groups[1].Value) },
                { new Regex(@"powerGroupIn\(([a-zA-Z0-9_\-\.]+)\)"), e => sourceFx.GetPower().FullName.StartsWith(e.Groups[1].Value) ? "1" : "0" },
                { new Regex(@"powerGroupNotIn\(([a-zA-Z0-9_\-\.]+)\)"), e => sourceFx.GetPower().FullName.StartsWith(e.Groups[1].Value) ? "0" : "1" },
                { new Regex(@"powerIs\(([a-zA-Z0-9_\-\.]+)\)"), e => sourceFx.GetPower().FullName.Equals(e.Groups[1].Value, StringComparison.InvariantCultureIgnoreCase) ? "1" : "0" },
                { new Regex(@"powerIsNot\(([a-zA-Z0-9_\-\.]+)\)"), e => sourceFx.GetPower().FullName.Equals(e.Groups[1].Value, StringComparison.InvariantCultureIgnoreCase) ? "0" : "1" }
            };
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

        public static float Parse(IEffect sourceFx, ExpressionType expressionType, out ErrorData error)
        {
            error = new ErrorData();
            float retValue;
            switch (expressionType)
            {
                case ExpressionType.Duration:
                    retValue = InternalParsing(sourceFx, expressionType, out error);
                    break;

                case ExpressionType.Probability:
                    retValue = InternalParsing(sourceFx, expressionType, out error);
                    break;

                case ExpressionType.Magnitude:
                    if (sourceFx.Expressions.Magnitude.IndexOf(".8 rechargetime power.base> 1 30 minmax * 1.8 + 2 * @StdResult * 10 / areafactor power.base> /", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        retValue = (float)((Math.Max(Math.Min(sourceFx.GetPower().RechargeTime, 30f), 0.0f) * 0.800000011920929 + 1.79999995231628) / 5.0) / sourceFx.GetPower().AoEModifier * sourceFx.Scale;
                        if (sourceFx.Expressions.Magnitude.Length > ".8 rechargetime power.base> 1 30 minmax * 1.8 + 2 * @StdResult * 10 / areafactor power.base> /".Length + 2)
                        {
                            retValue *= float.Parse(sourceFx.Expressions.Magnitude.Substring(".8 rechargetime power.base> 1 30 minmax * 1.8 + 2 * @StdResult * 10 / areafactor power.base> /".Length + 1).Substring(0, 2));
                        }

                        return retValue;
                    }

                    if (string.IsNullOrWhiteSpace(sourceFx.Expressions.Magnitude)) return 0f;

                    var baseFx = sourceFx.Clone() as IEffect;
                    retValue = InternalParsing(baseFx, expressionType, out error);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(expressionType), expressionType, null);
            }

            return error.Found ? 0f : retValue;
        }

        private static float InternalParsing(IEffect sourceFx, ExpressionType expressionType, out ErrorData error)
        {
            var pickedPowerNames = MidsContext.Character.CurrentBuild == null
                ? new List<string>()
                : MidsContext.Character.CurrentBuild.Powers == null
                    ? new List<string>()
                    : MidsContext.Character.CurrentBuild.Powers
                        .Where(pe => pe.Power != null)
                        .Select(pe => pe.Power.FullName)
                        .ToList();

            error = new ErrorData();
            var mathEngine = CalculationEngine.New<double>();
            var expr = expressionType switch
            {
                ExpressionType.Duration => sourceFx.Expressions.Duration,
                ExpressionType.Magnitude => sourceFx.Expressions.Magnitude,
                ExpressionType.Probability => sourceFx.Expressions.Probability,
                _ => throw new ArgumentOutOfRangeException(nameof(expressionType), expressionType, null)
            };

            // Constants
            expr = CommandsDict(sourceFx).Aggregate(expr, (current, cmd) => current.Replace(cmd.Key, cmd.Value));

            // Non-numeric functions
            expr = FunctionsDict(sourceFx, pickedPowerNames).Aggregate(expr, (current, f1) => f1.Key.Replace(current, f1.Value));

            // Numeric functions
            mathEngine.AddFunction("eq", (a, b) => Math.Abs(a - b) < double.Epsilon ? 1 : 0);
            mathEngine.AddFunction("ne", (a, b) => Math.Abs(a - b) > double.Epsilon ? 1 : 0);
            mathEngine.AddFunction("minmax", (a, b, c) => Math.Min(b > c ? b : c, Math.Max(b > c ? c : b, a)));

            try
            {
                return (float)mathEngine.Calculate(expr);
            }
            catch (ParseException ex)
            {
                error.Type = expressionType;
                error.Found = true;
                error.Message = ex.Message;

                return 0;
            }
            catch (VariableNotDefinedException ex)
            {
                error.Type = expressionType;
                error.Found = true;
                error.Message = ex.Message;

                return 0;
            }
            catch (InvalidOperationException ex)
            {
                error.Type = expressionType;
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
