using Jace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using FastDeepCloner;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core
{
    public static class BooleanExprPreprocessor
    {
        /// <summary>
        /// Get conditionals from effect, validate each of them
        /// Return value for single conditionals is either "1" or "0"
        /// </summary>
        /// <param name="effect">Source effect</param>
        /// <returns>List of pre-processed conditionals from source effect</returns>
        private static IEnumerable<string> GetConditions(IEffect effect)
        {
            var ret = new List<string>();
            if (effect.ActiveConditionals is not {Count: > 0})
            {
                return ret;
            }

            ret.AddRange(effect.ActiveConditionals
                .Select((t, i) => i > 0 && t.Key.StartsWith("OR ") ? "OR " : "")
                .Select((prefix, i) => $"{prefix}{(effect.ValidateConditional(i) ? "1" : "0")}"));

            return ret;
        }

        /// <summary>
        /// Build global infix expression from conditionals
        /// Use the result of <see cref="GetConditions"/>
        /// </summary>
        /// <param name="expressions">Conditional expressions</param>
        /// <returns>Single expression string with infix operators</returns>
        private static string BuildGlobalExpression(IEnumerable<string> expressions)
        {
            return expressions.Select(expr => expr.Trim())
                .Aggregate("", (current, s) => current + (current == ""
                    ? s.StartsWith("OR ")
                        ? s.Replace("OR ", "")
                        : s
                    : s.StartsWith("OR ")
                        ? $" {s}"
                        : $" AND {s}"));
        }

        /// <summary>
        /// Build global infix expression from conditionals, restrict to conditional type and power name
        /// </summary>
        /// <param name="effect">Source effect</param>
        /// <param name="cType">Conditional type</param>
        /// <param name="cPowerName">Conditional power name</param>
        /// <returns>Single expression string with infix operators</returns>
        private static string BuildGlobalExpression(IEffect effect, string cType, string cPowerName)
        {
            if (effect.ActiveConditionals is not {Count: > 0})
            {
                return "0";
            }

            var getCondition = new Regex("(:.*)");
            var getConditionItem = new Regex("(.*:)");
            var conditionResults = new List<bool>();
            var conditionTypes = new List<string>();

            foreach (var cVp in effect.ActiveConditionals)
            {
                conditionTypes.Add(cVp.Key.Contains("OR ") ? "OR" : "AND");

                var k = cVp.Key.Replace("AND ", "").Replace("OR ", "");
                var condition = getCondition.Replace(k, "");
                var conditionItemName = getConditionItem.Replace(k, "").Replace(":", "");
                var conditionPower = DatabaseAPI.GetPowerByFullName(conditionItemName);
                var cVal = cVp.Value.Split(' ');
                var powerDisplayName = conditionPower?.DisplayName;

                if (powerDisplayName == null || !powerDisplayName.Contains(cPowerName))
                {
                    return "0";
                }

                if (string.Equals(cType, condition, StringComparison.CurrentCultureIgnoreCase) && condition == "Active")
                {
                    bool? boolVal = Convert.ToBoolean(cVp.Value);
                    conditionResults.Add(MidsContext.Character.CurrentBuild.PowerActive(conditionPower) == boolVal);
                }
                else if (string.Equals(cType, condition, StringComparison.CurrentCultureIgnoreCase) && condition == "Taken")
                {
                    conditionResults.Add(MidsContext.Character.CurrentBuild.PowerUsed(conditionPower).Equals(Convert.ToBoolean(cVp.Value)));
                }
                else if (string.Equals(cType, condition, StringComparison.CurrentCultureIgnoreCase) && condition == "Stacks")
                {
                    conditionResults.Add(cVal[0] switch
                    {
                        "=" => conditionPower.Stacks.Equals(Convert.ToInt32(cVal[1])),
                        ">" => conditionPower.Stacks > Convert.ToInt32(cVal[1]),
                        "<" => conditionPower.Stacks < Convert.ToInt32(cVal[1]),
                        _ => true
                    });
                }
                else if (string.Equals(cType, condition, StringComparison.CurrentCultureIgnoreCase) && condition == "Team")
                {
                    conditionResults.Add(cVal[0] switch
                    {
                        "=" => MidsContext.Config.TeamMembers.ContainsKey(conditionItemName) && MidsContext.Config
                            .TeamMembers[conditionItemName]
                            .Equals(Convert.ToInt32(cVal[1])),
                        ">" => MidsContext.Config.TeamMembers.ContainsKey(conditionItemName) &&
                               MidsContext.Config.TeamMembers[conditionItemName] > Convert.ToInt32(cVal[1]),
                        "<" => MidsContext.Config.TeamMembers.ContainsKey(conditionItemName) &&
                               MidsContext.Config.TeamMembers[conditionItemName] < Convert.ToInt32(cVal[1]),
                        _ => true
                    });
                }
                else
                {
                    conditionResults.Add(true);
                }
            }

            var expr = "";

            for (var i = 0; i < conditionResults.Count; i++)
            {
                expr += i == 0
                    ? conditionResults[i] ? "1" : "0"
                    : $" {conditionTypes[i]} {(conditionResults[i] ? "1" : "0")}";
            }

            return expr;
        }

        /// <summary>
        /// Build global infix expression from conditionals, restrict to power name
        /// </summary>
        /// <param name="effect">Source effect</param>
        /// <param name="cPowerName">Conditional power name</param>
        /// <returns>Single expression string with infix operators</returns>
        private static string BuildGlobalExpression(IEffect effect, string cPowerName)
        {
            if (effect.ActiveConditionals is not { Count: > 0 })
            {
                return "0";
            }

            var getCondition = new Regex("(:.*)");
            var getConditionItem = new Regex("(.*:)");
            var conditionResults = new List<bool>();
            var conditionTypes = new List<string>();
            foreach (var cVp in effect.ActiveConditionals)
            {
                conditionTypes.Add(cVp.Key.Contains("OR ") ? "OR" : "AND");

                var k = cVp.Key.Replace("AND ", "").Replace("OR ", "");
                var condition = getCondition.Replace(k, "");
                var conditionItemName = getConditionItem.Replace(k, "").Replace(":", "");
                var conditionPower = DatabaseAPI.GetPowerByFullName(conditionItemName);
                var buildPowers = MidsContext.Character.CurrentBuild.Powers;
                var cVal = cVp.Value.Split(' ');
                var powerDisplayName = conditionPower?.DisplayName;
                if (powerDisplayName == null || !powerDisplayName.Contains(cPowerName))
                {
                    return "0";
                }

                switch (condition)
                {
                    case "Active":
                        bool? boolVal = Convert.ToBoolean(cVp.Value);
                        conditionResults.Add(MidsContext.Character.CurrentBuild.PowerActive(conditionPower) == boolVal);

                        break;
                    case "Taken":
                        conditionResults.Add(MidsContext.Character.CurrentBuild.PowerUsed(conditionPower)
                            .Equals(Convert.ToBoolean(cVp.Value)));

                        break;
                    case "Stacks":
                        var stacks = buildPowers
                            .Where(x => x.Power == conditionPower)
                            .Select(x => x.Power.Stacks)
                            .ToList();
                        conditionResults.Add(cVal[0] switch
                        {
                            "=" => stacks[0].Equals(Convert.ToInt32(cVal[1])),
                            ">" => stacks[0] > Convert.ToInt32(cVal[1]),
                            "<" => stacks[0] < Convert.ToInt32(cVal[1]),
                            _ => true
                        });

                        break;
                    case "Team":
                        conditionResults.Add(cVal[0] switch
                        {
                            "=" => MidsContext.Config.TeamMembers.ContainsKey(conditionItemName) && MidsContext.Config
                                .TeamMembers[conditionItemName]
                                .Equals(Convert.ToInt32(cVal[1])),
                            ">" => MidsContext.Config.TeamMembers.ContainsKey(conditionItemName) &&
                                   MidsContext.Config.TeamMembers[conditionItemName] > Convert.ToInt32(cVal[1]),
                            "<" => MidsContext.Config.TeamMembers.ContainsKey(conditionItemName) &&
                                   MidsContext.Config.TeamMembers[conditionItemName] < Convert.ToInt32(cVal[1]),
                            _ => true
                        });

                        break;
                    default:
                        conditionResults.Add(true);
                        break;
                }
            }

            var expr = "";

            for (var i = 0; i < conditionResults.Count; i++)
            {
                expr += i == 0
                    ? conditionResults[i] ? "1" : "0"
                    : $" {conditionTypes[i]} {(conditionResults[i] ? "1" : "0")}";
            }

            return expr;
        }

        /// <summary>
        /// Split expression into tokens, by operator (either AND or OR)
        /// </summary>
        /// <param name="expr">Global infix expression</param>
        /// <returns>List of expression tokens (values/operators)</returns>
        private static List<string> Tokenize(string expr)
        {
            return Regex.Split(expr, @" (AND|OR) ").ToList();
        }

        /// <summary>
        /// Transforms and merge infix operators of the selected kind into prefix functions style.
        /// Due to limitations of the math engine, operators are limited to two members (func(a, b))
        /// </summary>
        /// <param name="exprTokens">Expression tokens - <see cref="Tokenize"/></param>
        /// <param name="op">Operator to look for (either AND or OR)</param>
        /// <returns>List of prefix functions tokens</returns>
        private static List<string> Merge(List<string> exprTokens, string op)
        {
            // Work on a local copy...
            // Obsolete ?
            var mergedTokens = exprTokens.Clone();
            var m = true;
            var n = 0;
            var l = mergedTokens.Count;
            while (m & n++ < 30)
            {
                var lastPos = -1;
                var tokensPos = new Dictionary<int, List<int>>(); // Position of tokens, positions of members
                for (var i = 0; i < mergedTokens.Count; i++)
                {
                    if (mergedTokens[i] != op)
                    {
                        continue;
                    }

                    if (lastPos == -1 || i - lastPos > 1)
                    {
                        lastPos = i;
                        tokensPos.Add(i, new List<int> {i - 1, i + 1});
                    }
                }

                // No more matching operators
                if (tokensPos.Count <= 0)
                {
                    return mergedTokens;
                }

                // Process only first operator found
                var t = tokensPos.First();
                if (t.Value[0] < mergedTokens.Count && t.Value[1] < mergedTokens.Count)
                {
                    mergedTokens[t.Key] = $"{op}({string.Join(", ", t.Value.Select(e => mergedTokens[e]))})";
                    
                    // Remove values from t.Value in one pass
                    // More robust than two List.RemoveAt()
                    mergedTokens = mergedTokens
                        .Select((e, i) => new KeyValuePair<int, string>(i, e))
                        .Where(e => !t.Value.Contains(e.Key))
                        .Select(e => e.Value)
                        .ToList();

                    m = l != mergedTokens.Count;
                    l = mergedTokens.Count;
                }
                else
                {
                    m = false;
                }
            }

            return mergedTokens;
        }

        /// <summary>
        /// Build Jace-compatible prefix style formula from effect conditionals
        /// </summary>
        /// <param name="effect">Source effect</param>
        /// <see cref="GetConditions"/>
        /// <see cref="BuildGlobalExpression"/>
        /// <see cref="Tokenize"/>
        /// <see cref="Merge"/>
        /// <seealso cref="Parse(IEffect)"/>
        /// <returns>Expression from conditionals, prefix style</returns>
        private static string Build(IEffect effect)
        {
            var conditionals = GetConditions(effect);
            var expr = BuildGlobalExpression(conditionals);
            var tokens = Tokenize(expr);

            //Debug.WriteLine($"------ Begin testing conditionals validation ------");
            //Debug.WriteLine($"  Initial expression: {string.Join(", ", tokens)}\r\n");
            //Debug.WriteLine($"    Merging AND operators");
            tokens = Merge(tokens, "AND");
            //Debug.WriteLine($"    Result after AND merge: {string.Join(" ", tokens)}\r\n");

            //Debug.WriteLine($"    Merging OR operators");
            tokens = Merge(tokens, "OR");
            //Debug.WriteLine($"    Result after OR merge: {string.Join(" ", tokens)}\r\n");

            //Debug.WriteLine($"  Final expression: {string.Join(" ", tokens)}\r\n");

            return string.Join(" ", tokens);
        }

        private static string Build(string expr)
        {
            var tokens = Tokenize(expr);
            tokens = Merge(tokens, "AND");
            tokens = Merge(tokens, "OR");
            
            return string.Join(" ", tokens);
        }

        /// <summary>
        /// Compute prefix style formula
        /// </summary>
        /// <param name="effect">Source effect</param>
        /// <seealso cref="Build(IEffect)"/>
        /// <returns>Result of expression, as boolean</returns>
        public static bool Parse(IEffect effect)
        {
            var prefixExpr = Build(effect);
            var mathEngine = CalculationEngine.New<double>();

            mathEngine.AddFunction("AND", (a, b) => a > 0 & b > 0 ? 1 : 0);
            mathEngine.AddFunction("OR", (a, b) => a > 0 | b > 0 ? 1 : 0);

            var ret = false;
            try
            {
                ret = mathEngine.Calculate(prefixExpr) > 0;
            }
            catch (ParseException ex)
            {
                Debug.WriteLine($"Conditional check failed in {prefixExpr}\nPower: {effect.GetPower()?.FullName}\r\n{ex.Message}");

                return false;
            }
            catch (VariableNotDefinedException ex)
            {
                Debug.WriteLine($"Conditional check failed (variable not defined) in {prefixExpr}\nPower: {effect.GetPower()?.FullName}\r\n{ex.Message}");

                return false;
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine($"Conditional check failed (invalid operation) in {prefixExpr}\nPower: {effect.GetPower()?.FullName}\r\n{ex.Message}");

                return false;
            }

            return ret;
        }

        /// <summary>
        /// Compute prefix style formula
        /// </summary>
        /// <param name="effect">Source effect</param>
        /// <param name="cType">Conditional type</param>
        /// <param name="cPowerName">Conditional power name</param>
        /// <seealso cref="BuildGlobalExpression(IEffect, string, string)"/>
        /// <seealso cref="Build(string)"/>
        /// <returns>Result of expression, as boolean</returns>
        public static bool Parse(IEffect effect, string cType, string cPowerName)
        {
            if (effect.ActiveConditionals is {Count: <= 0})
            {
                return false;
            }

            var expr = BuildGlobalExpression(effect, cType, cPowerName);
            var prefixExpr = Build(expr);
            var mathEngine = CalculationEngine.New<double>();

            mathEngine.AddFunction("AND", (a, b) => a > 0 & b > 0 ? 1 : 0);
            mathEngine.AddFunction("OR", (a, b) => a > 0 | b > 0 ? 1 : 0);

            var ret = false;
            try
            {
                ret = mathEngine.Calculate(prefixExpr) > 0;
            }
            catch (ParseException ex)
            {
                Debug.WriteLine($"Conditional check failed in {prefixExpr}\nPower: {effect.GetPower()?.FullName}\r\n{ex.Message}");

                return false;
            }
            catch (VariableNotDefinedException ex)
            {
                Debug.WriteLine($"Conditional check failed (variable not defined) in {prefixExpr}\nPower: {effect.GetPower()?.FullName}\r\n{ex.Message}");

                return false;
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine($"Conditional check failed (invalid operation) in {prefixExpr}\nPower: {effect.GetPower()?.FullName}\r\n{ex.Message}");

                return false;
            }

            return ret;
        }

        /// <summary>
        /// Compute prefix style formula
        /// </summary>
        /// <param name="effect">Source effect</param>
        /// <param name="cPowerName">Conditional power name</param>
        /// <seealso cref="BuildGlobalExpression(IEffect, string)"/>
        /// <seealso cref="Build(string)"/>
        /// <returns>Result of expression, as boolean</returns>
        public static bool Parse(IEffect effect, string cPowerName)
        {
            if (effect.ActiveConditionals is { Count: <= 0 })
            {
                return false;
            }

            var expr = BuildGlobalExpression(effect, cPowerName);
            var prefixExpr = Build(expr);
            var mathEngine = CalculationEngine.New<double>();

            mathEngine.AddFunction("AND", (a, b) => a > 0 & b > 0 ? 1 : 0);
            mathEngine.AddFunction("OR", (a, b) => a > 0 | b > 0 ? 1 : 0);

            var ret = false;
            try
            {
                ret = mathEngine.Calculate(prefixExpr) > 0;
            }
            catch (ParseException ex)
            {
                Debug.WriteLine($"Conditional check failed in {prefixExpr}\nPower: {effect.GetPower()?.FullName}\r\n{ex.Message}");

                return false;
            }
            catch (VariableNotDefinedException ex)
            {
                Debug.WriteLine($"Conditional check failed (variable not defined) in {prefixExpr}\nPower: {effect.GetPower()?.FullName}\r\n{ex.Message}");

                return false;
            }
            catch (InvalidOperationException ex)
            {
                Debug.WriteLine($"Conditional check failed (invalid operation) in {prefixExpr}\nPower: {effect.GetPower()?.FullName}\r\n{ex.Message}");

                return false;
            }

            return ret;
        }
    }
}
