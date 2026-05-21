using Jace;
using System;
using System.Linq;

namespace Mids_Reborn.Core
{
    /// <summary>
    /// Legacy public entry points that now evaluate only the advanced condition model.
    /// Older string-based conditional formats are no longer consulted here.
    /// </summary>
    public static class BooleanExprPreprocessor
    {
        public static bool Parse(IEffect effect)
        {
            return AdvancedConditionEvaluator.Evaluate(effect);
        }

        public static bool Parse(IEffect effect, string cType, string cPowerName)
        {
            var filtered = new AdvancedConditionSet
            {
                Rows = effect.AdvancedConditions.Rows
                    .Where(row => MatchesLegacyFilter(row, cType, cPowerName))
                    .Select(row => row.Clone())
                    .ToList()
            };

            return filtered.Rows.Count > 0 && AdvancedConditionEvaluator.Evaluate(effect, filtered);
        }

        public static bool Parse(IEffect effect, string cPowerName)
        {
            var filtered = new AdvancedConditionSet
            {
                Rows = effect.AdvancedConditions.Rows
                    .Where(row => MatchesLegacyPowerFilter(row, cPowerName))
                    .Select(row => row.Clone())
                    .ToList()
            };

            return filtered.Rows.Count > 0 && AdvancedConditionEvaluator.Evaluate(effect, filtered);
        }

        private static bool MatchesLegacyFilter(AdvancedConditionRow row, string cType, string cPowerName)
        {
            var kindMatches = cType switch
            {
                "Active" => row.Kind == AdvancedConditionKind.PowerActive,
                "Taken" => row.Kind == AdvancedConditionKind.PowerTaken || row.Kind == AdvancedConditionKind.SourceOwnPower,
                "Stacks" => row.Kind == AdvancedConditionKind.PowerStacks,
                "Team" => row.Kind == AdvancedConditionKind.TeamMembers,
                "Config" => row.Kind == AdvancedConditionKind.CombatSetting,
                _ => false
            };

            return kindMatches && MatchesLegacyPowerFilter(row, cPowerName);
        }

        private static bool MatchesLegacyPowerFilter(AdvancedConditionRow row, string cPowerName)
        {
            if (row.Kind is AdvancedConditionKind.TeamMembers or AdvancedConditionKind.CombatSetting)
            {
                return true;
            }

            var power = DatabaseAPI.GetPowerByFullName(row.Subject);
            return power?.DisplayName?.Contains(cPowerName, StringComparison.CurrentCultureIgnoreCase) == true;
        }
    }
}
