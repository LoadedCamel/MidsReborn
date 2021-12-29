using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using mrbBase.Base.Master_Classes;
using Jace;

namespace mrbBase
{
    public static class ExpressionParser
    {
        public class ParsedData
        {
            public bool ErrorFound { get; set; }
            public string ErrorString { get; set; } = string.Empty;
        }

        public static readonly string ExprSeparator = "///";

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
                { "rand()", $"{new Random().NextDouble()}" }
            };
        }

        private static Dictionary<Regex, MatchEvaluator> FunctionsDict1(IEffect sourceFx, ICollection<string> pickedPowerNames)
        {
            return new Dictionary<Regex, MatchEvaluator>
            {
                { new Regex(@"source\.ownPower\?\(([a-zA-Z0-9_\-\.]+)\)"), e => pickedPowerNames.Contains(e.Groups[1].Value) ? "1" : "0" },
                { new Regex(@"([a-zA-Z\-_\.]+)>stacks"), e => GetStacks(e.Groups[1].Value, pickedPowerNames) },
                { new Regex(@"modifier\>([a-zA-Z0-9_\-]+)"), e => GetModifier(e.Groups[1].Value) },
                { new Regex(@"powerGroupIn\(([a-zA-Z0-9_\-\.]+)\)"), e => sourceFx.GetPower().FullName.StartsWith(e.Groups[1].Value) ? "1" : "0" },
                { new Regex(@"powerGroupNotIn\(([a-zA-Z0-9_\-\.]+)\)"), e => sourceFx.GetPower().FullName.StartsWith(e.Groups[1].Value) ? "0" : "1" }
            };
        }

        private static string GetStacks(string powerName, ICollection<string> pickedPowerNames)
        {
            if (!pickedPowerNames.Contains(powerName)) return "0";
            foreach (var pe in MidsContext.Character.CurrentBuild.Powers)
            {
                if (pe.Power == null) continue;
                if (pe.Power.FullName != powerName) continue;

                if (pe.Power.Active)
                {
                    return $"{pe.VariableValue}";
                }
            }

            return "0";
        }

        private static string GetModifier(string modifierName)
        {
            // DatabaseAPI.NidFromUidAttribMod(ModifierTable);
            var modTable = DatabaseAPI.Database.AttribMods.Modifier.Where(e => e.ID == modifierName).ToList();

            return modTable.Count <= 0
                ? "0"
                : $"{Math.Abs(modTable[0].Table[MidsContext.Character.Level][MidsContext.Character.Archetype.Column])}";
        }

        public static List<string> SplitExpression(IEffect sourceFx, out bool forcedMagDefault, bool useDefaultMag = true)
        {
            var chunks = sourceFx.MagnitudeExpression.Split(new[] { ExprSeparator }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (chunks.Count == 1 & sourceFx.MagnitudeExpression.TrimStart().StartsWith(ExprSeparator))
            {
                forcedMagDefault = true;
                return new List<string> { $"{(useDefaultMag ? sourceFx.Scale * sourceFx.nMagnitude : "")}", chunks[0] };
            }

            forcedMagDefault = false;

            return chunks;
        }

        public static string AssembleExpression(string magExpr, string probExpr = "")
        {
            magExpr = magExpr.Trim('/', ' ', '\t', '\r', '\n', '\0');
            probExpr = probExpr.Trim('/', ' ', '\t', '\r', '\n', '\0');

            return $"{magExpr}{(string.IsNullOrEmpty(probExpr) ? "" : $"{ExprSeparator}{probExpr}")}";
        }

        public static IEffect SubExpressionToFx(string magExpr, IEffect baseFx)
        {
            var subFx = (IEffect)baseFx.Clone();
            subFx.MagnitudeExpression = magExpr;

            return subFx;
        }

        public static bool HasSeparator(IEffect sourceFx)
        {
            return sourceFx.MagnitudeExpression.Contains(ExprSeparator);
        }

        public static float ParseExpression(IEffect sourceFx, out ParsedData data, int chunk = 0)
        {
            data = new ParsedData();
            if (sourceFx.MagnitudeExpression.IndexOf(".8 rechargetime power.base> 1 30 minmax * 1.8 + 2 * @StdResult * 10 / areafactor power.base> /", StringComparison.OrdinalIgnoreCase) > -1)
            {
                var num2 = (float)((Math.Max(Math.Min(sourceFx.GetPower().RechargeTime, 30f), 0.0f) * 0.800000011920929 + 1.79999995231628) / 5.0) / sourceFx.GetPower().AoEModifier * sourceFx.Scale;
                if (sourceFx.MagnitudeExpression.Length > ".8 rechargetime power.base> 1 30 minmax * 1.8 + 2 * @StdResult * 10 / areafactor power.base> /".Length + 2)
                {
                    num2 *= float.Parse(sourceFx.MagnitudeExpression.Substring(".8 rechargetime power.base> 1 30 minmax * 1.8 + 2 * @StdResult * 10 / areafactor power.base> /".Length + 1).Substring(0, 2));
                }

                return num2;
            }

            if (string.IsNullOrWhiteSpace(sourceFx.MagnitudeExpression)) return 0f;
            float ret;
            if (sourceFx.MagnitudeExpression.Contains(ExprSeparator))
            {
                var chunks = SplitExpression(sourceFx, out _);

                ret = ParseExpression2(SubExpressionToFx(chunks[chunk], sourceFx), out data);

                return data.ErrorFound ? 0f : ret;
            }

            ret = ParseExpression2(SubExpressionToFx(sourceFx.MagnitudeExpression, sourceFx), out data);

            return data.ErrorFound ? 0f : ret;
        }

        public static float ParseExpression2(IEffect sourceFx, out ParsedData parsedData)
        {
            var pickedPowerNames = MidsContext.Character.CurrentBuild == null
                ? new List<string>()
                : MidsContext.Character.CurrentBuild.Powers == null
                    ? new List<string>()
                    : MidsContext.Character.CurrentBuild.Powers
                        .Where(pe => pe.Power != null)
                        .Select(pe => pe.Power.FullName)
                        .ToList();

            parsedData = new ParsedData();
            var mathEngine = new CalculationEngine();
            var magExpr = sourceFx.MagnitudeExpression.TrimEnd('/');

            // Constants
            magExpr = CommandsDict(sourceFx).Aggregate(magExpr, (current, cmd) => current.Replace(cmd.Key, cmd.Value));

            // Non-numeric functions
            magExpr = FunctionsDict1(sourceFx, pickedPowerNames).Aggregate(magExpr, (current, f1) => f1.Key.Replace(current, f1.Value));

            // Numeric functions
            mathEngine.AddFunction("eq", (a, b) => Math.Abs(a - b) < double.Epsilon ? 1 : 0);
            mathEngine.AddFunction("ne", (a, b) => Math.Abs(a - b) > double.Epsilon ? 1 : 0);
            mathEngine.AddFunction("minmax", (a, b, c) => Math.Max(b, Math.Min(c, a)));

            try
            {
                var ret = (float)mathEngine.Calculate(magExpr);

                return ret;
            }
            catch (ParseException ex)
            {
                parsedData.ErrorFound = true;
                parsedData.ErrorString = ex.Message;

                return 0;
            }
            catch (VariableNotDefinedException ex)
            {
                parsedData.ErrorFound = true;
                parsedData.ErrorString = ex.Message;

                return 0;
            }
        }
    }
}