using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Core;

public sealed class PowerDisplaySnapshot
{
    public PowerDisplaySnapshot(
        IPower? basePower,
        IPower? enhancedPower,
        IPower? rootPowerBase,
        IPower? rootPowerEnh,
        int historyIndex,
        bool baseWasResolvedForDisplay,
        bool enhancedWasResolvedForDisplay,
        bool baseWasPaddedOrRepaired,
        bool enhancedWasPaddedOrRepaired)
    {
        BasePower = basePower;
        EnhancedPower = enhancedPower;
        RootPowerBase = rootPowerBase;
        RootPowerEnh = rootPowerEnh;
        HistoryIndex = historyIndex;
        BaseWasResolvedForDisplay = baseWasResolvedForDisplay;
        EnhancedWasResolvedForDisplay = enhancedWasResolvedForDisplay;
        BaseWasPaddedOrRepaired = baseWasPaddedOrRepaired;
        EnhancedWasPaddedOrRepaired = enhancedWasPaddedOrRepaired;
    }

    public IPower? BasePower { get; }
    public IPower? EnhancedPower { get; }
    public IPower? RootPowerBase { get; }
    public IPower? RootPowerEnh { get; }
    public int HistoryIndex { get; }
    public bool BaseWasResolvedForDisplay { get; }
    public bool EnhancedWasResolvedForDisplay { get; }
    public bool BaseWasPaddedOrRepaired { get; }
    public bool EnhancedWasPaddedOrRepaired { get; }
}
