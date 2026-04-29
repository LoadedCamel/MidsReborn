using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core.PlannerRulesets;

public static class PlannerRulesetResolver
{
    private static readonly IPlannerRuleset Legacy = new LegacyPlannerRuleset();
    private static readonly IPlannerRuleset Homecoming = new HomecomingPlannerRuleset();
    private static readonly IPlannerRuleset Ourodev = new OurodevPlannerRuleset();

    public static IPlannerRuleset Resolve(PlannerRulesetId rulesetId)
    {
        return rulesetId switch
        {
            PlannerRulesetId.Homecoming => Homecoming,
            PlannerRulesetId.Ourodev => Ourodev,
            _ => Legacy
        };
    }
}
