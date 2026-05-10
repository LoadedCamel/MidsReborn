using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core;

public sealed record SpecialPowerChipGroup(string Key, string Label, IReadOnlyList<IPower> Powers);
