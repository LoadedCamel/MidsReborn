using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Controls;

public sealed class MidsSmartLayoutPanel : TableLayoutPanel
{
    #region Fields

    private int _preferredGrowRow = 1; // innerLeftPanel row
    private int _flexibleRow = 2;      // MidsDataView row

    private bool _layoutScheduled;

    // Baseline captured after first real layout pass
    private int? _initialGrowPx;
    private int? _initialFlexPx;

    #endregion

    #region Properties

    [Category("Layout")]
    [Description("Row that grows first until its children are fully visible.")]
    [DefaultValue(1)]
    public int PreferredGrowRow
    {
        get => _preferredGrowRow;
        set { if (_preferredGrowRow != value) { _preferredGrowRow = value; ResetBaselines(); InvalidateSmartLayout(); } }
    }

    [Category("Layout")]
    [Description("Row that absorbs remaining space after the preferred row is satisfied.")]
    [DefaultValue(2)]
    public int FlexibleRow
    {
        get => _flexibleRow;
        set { if (_flexibleRow != value) { _flexibleRow = value; ResetBaselines(); InvalidateSmartLayout(); } }
    }

    #endregion

    #region Overrides

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        ScheduleSmartLayout();
    }

    protected override void OnLayout(LayoutEventArgs levent)
    {
        base.OnLayout(levent);
        ScheduleSmartLayout();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
        base.OnSizeChanged(e);
        ScheduleSmartLayout();
    }

    protected override void OnControlAdded(ControlEventArgs e)
    {
        base.OnControlAdded(e);
        HookChild(e.Control, true);
        ScheduleSmartLayout();
    }

    protected override void OnControlRemoved(ControlEventArgs e)
    {
        base.OnControlRemoved(e);
        HookChild(e.Control, false);
        ScheduleSmartLayout();
    }

    #endregion

    #region Core

    private void ScheduleSmartLayout()
    {
        if (_layoutScheduled || !IsHandleCreated || DesignMode) return;
        _layoutScheduled = true;
        BeginInvoke(new MethodInvoker(ApplySmartLayout));
    }

    private void InvalidateSmartLayout()
    {
        _layoutScheduled = false;
        ScheduleSmartLayout();
    }

    private void ResetBaselines()
    {
        _initialGrowPx = null;
        _initialFlexPx = null;
    }

    private void ApplySmartLayout()
    {
        _layoutScheduled = false;
        if (!IsHandleCreated) return;

        int maxRow = Math.Max(PreferredGrowRow, FlexibleRow);
        if (RowCount <= maxRow) return;

        var growContainer = GetControlFromPosition(0, PreferredGrowRow);
        var flexContainer = GetControlFromPosition(0, FlexibleRow);
        if (growContainer is null || !growContainer.Visible) return;

        // Snapshot actual row heights after normal layout
        int[] rowHeights = GetRowHeights();
        if (rowHeights.Length < RowCount) return;

        // Capture the designer baselines once
        _initialGrowPx ??= Math.Max(0, rowHeights[PreferredGrowRow]);
        _initialFlexPx ??= Math.Max(0, rowHeights[FlexibleRow]);

        int initialGrow = _initialGrowPx.Value;
        int initialFlex = _initialFlexPx.Value;

        // Height needed for innerLeftPanel so both MidsListView columns have no scrollbars
        int capGrow = ComputeCapHeightFor(growContainer);
        if (capGrow <= 0)
        {
            // fall back to container preference
            int w = Math.Max(0, growContainer.Width);
            capGrow = growContainer.GetPreferredSize(new Size(w, int.MaxValue)).Height;
        }
        // Ensure cap is at least the designer baseline
        capGrow = Math.Max(capGrow, initialGrow);

        // Freeze all non-target rows to their measured height to keep Y positions stable
        int otherFixed = 0;
        for (int r = 0; r < RowStyles.Count; r++)
        {
            if (r == PreferredGrowRow || r == FlexibleRow) continue;
            otherFixed += rowHeights[r];
        }

        int total = ClientSize.Height;
        int availableForGroup = Math.Max(0, total - otherFixed); // the space shared by (grow + flex)

        // --- Allocation rules (acts like "normal fill" with a clamp) ---
        // 1) While available <= initialGrow + initialFlex (at/below designer): keep grow at its baseline,
        //    flex takes the hit. If even that isn't enough, both shrink (flex to 0 first).
        // 2) While initialGrow + initialFlex < available <= capGrow + initialFlex:
        //    ONLY grow increases up to cap; flex stays at its baseline (appears anchored under panel).
        // 3) Once available > capGrow + initialFlex:
        //    grow is frozen at cap; all extra space goes to flex.

        int targetGrow, targetFlex;

        if (availableForGroup <= initialGrow)
        {
            // Tiny: flex = 0; grow shrinks below baseline if absolutely necessary
            targetGrow = availableForGroup;
            targetFlex = 0;
        }
        else if (availableForGroup <= initialGrow + initialFlex)
        {
            // Shrinking back toward baseline: keep grow at baseline; flex gives up space
            targetGrow = initialGrow;
            targetFlex = availableForGroup - initialGrow;
        }
        else if (availableForGroup <= capGrow + initialFlex)
        {
            // Natural expansion: only grow increases up to cap; flex stays at baseline
            targetGrow = Math.Min(capGrow, availableForGroup - initialFlex);
            targetFlex = initialFlex;
        }
        else
        {
            // Beyond cap: grow frozen at cap; flex absorbs remaining space
            targetGrow = capGrow;
            targetFlex = availableForGroup - capGrow;
        }

        // Safety
        targetGrow = Math.Max(0, targetGrow);
        targetFlex = Math.Max(0, targetFlex);

        SuspendLayout();

        // Lock other rows to keep their Y positions
        for (int r = 0; r < RowStyles.Count; r++)
        {
            if (r == PreferredGrowRow || r == FlexibleRow) continue;
            SetRowAbsolute(r, rowHeights[r]);
        }

        // Apply targets: both rows Absolute to match exact allocation
        SetRowAbsolute(PreferredGrowRow, targetGrow);
        SetRowAbsolute(FlexibleRow, targetFlex);

        ResumeLayout(performLayout: false);
    }

    #endregion

    #region Measurement

    // Cap is the height of the growContainer needed so that *each* MidsListView inside
    // can display all items (no scrollbar). We take the max bottom extent, not a sum.
    private static int ComputeCapHeightFor(Control container)
    {
        int topPad = container.Padding.Top;
        int bottomPad = container.Padding.Bottom;

        var listViews = EnumerateDescendantsInclusive(container)
            .OfType<MidsListView>()
            .Where(lv => lv.Visible)
            .ToList();

        if (listViews.Count == 0) return 0;

        int maxBottom = 0;
        foreach (var lv in listViews)
        {
            int w = Math.Max(0, lv.Width);
            int preferred = lv.GetPreferredSize(new Size(w, int.MaxValue)).Height;
            int top = lv.Top - lv.Margin.Top;
            int bottom = top + preferred + lv.Margin.Vertical + 20;
            if (bottom > maxBottom) maxBottom = bottom;
        }

        return topPad + maxBottom + bottomPad;
    }

    #endregion

    #region Row helpers

    private void SetRowAbsolute(int row, int height)
    {
        if (row < 0 || row >= RowStyles.Count) return;
        var rs = RowStyles[row];
        rs.SizeType = SizeType.Absolute;
        rs.Height = Math.Max(0, height);
    }

    #endregion

    #region Child hooks

    private void HookChild(Control? c, bool hook)
    {
        if (c is null) return;

        if (hook)
        {
            c.SizeChanged += ChildChanged;
            c.VisibleChanged += ChildChanged;
        }
        else
        {
            c.SizeChanged -= ChildChanged;
            c.VisibleChanged -= ChildChanged;
        }

        foreach (Control child in c.Controls)
            HookChild(child, hook);
    }

    private void ChildChanged(object? s, EventArgs e) => ScheduleSmartLayout();

    private static IEnumerable<Control> EnumerateDescendantsInclusive(Control root)
    {
        yield return root;
        foreach (Control child in root.Controls)
            foreach (var d in EnumerateDescendantsInclusive(child))
                yield return d;
    }

    #endregion
}