using System.Linq;

namespace Mids_Reborn.Core;

public static class SpecialPowerBuildIdentityResolver
{
    public static bool Matches(IPower? left, IPower? right)
    {
        if (left == null || right == null)
        {
            return false;
        }

        if (left.PowerIndex == right.PowerIndex)
        {
            return true;
        }

        return left.StaticIndex >= 0 &&
               right.StaticIndex >= 0 &&
               left.StaticIndex == right.StaticIndex;
    }

    public static PowerEntry? FindMatchingEntry(Build? build, IPower? power)
    {
        if (build == null || power == null)
        {
            return null;
        }

        return build.Powers.FirstOrDefault(entry => entry?.Power is not null && Matches(entry.Power, power));
    }

    public static bool BuildUsesPower(Build? build, IPower? power)
    {
        return FindMatchingEntry(build, power) != null;
    }
}
