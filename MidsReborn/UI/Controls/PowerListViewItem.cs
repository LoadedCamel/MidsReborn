using System;
using Mids_Reborn.Core;

namespace Mids_Reborn.UI.Controls;

public class PowerListViewItem : MidsListViewItem
{
    public int IdxPower { get; set; }
    public int NIdPower { get; set; }
    public int NIdSet { get; set; }

    public PowerListViewItem(
        string displayName,
        MidsItemState state,
        int nidSet = -1,
        int idxPower = -1,
        int nidPower = -1,
        object? tag = null,
        MidsItemFontStyles style = MidsItemFontStyles.Normal,
        MidsItemAlign alignment = MidsItemAlign.Left)
        : base(displayName ?? string.Empty, state, style, alignment)
    {
        IdxPower = idxPower;
        NIdPower = nidPower;
        NIdSet = nidSet;
        Tag = tag;
    }

    public PowerListViewItem(
        IPowerset powerSet,
        MidsItemState state = MidsItemState.Heading,
        int idxPower = -1,
        int nIdPower = -1,
        MidsItemFontStyles style = MidsItemFontStyles.Bold,
        MidsItemAlign alignment = MidsItemAlign.Center)
        : base(powerSet?.DisplayName ?? string.Empty, state, style, alignment)
    {
        if (powerSet is null) throw new ArgumentNullException(nameof(powerSet));
        IdxPower = idxPower;
        NIdPower = nIdPower;
        NIdSet = powerSet.nIDTrunkSet;
    }
}