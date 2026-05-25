using System.ComponentModel;

namespace Mids_Reborn.UI.Controls.Test;

[DesignerCategory("Code")]
internal sealed class MidsTotalsDualColumnHost : Panel
{
    private readonly Control _left;
    private readonly Control _right;
    private float _uiScale = 1f;

    public float UiScale
    {
        get => _uiScale;
        set
        {
            var clamped = Math.Clamp(value, 0.90f, 1.25f);
            if (Math.Abs(_uiScale - clamped) < 0.01f) return;
            _uiScale = clamped;
            if (_left is MidsTotalsBarList leftList) leftList.UiScale = clamped;
            if (_right is MidsTotalsBarList rightList) rightList.UiScale = clamped;
            PerformLayout();
            Invalidate();
        }
    }

    public MidsTotalsDualColumnHost(Control left, Control right)
    {
        _left = left;
        _right = right;

        BackColor = Color.Transparent;
        Margin = Padding.Empty;
        Padding = Padding.Empty;
        AutoSize = false;

        _left.Margin = Padding.Empty;
        _right.Margin = Padding.Empty;
        Controls.Add(_left);
        Controls.Add(_right);
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        var gap = ScalePx(8);
        var inset = ScalePx(1);
        var width = proposedSize.Width > 0 ? proposedSize.Width : Width;
        var columnWidth = Math.Max(1, (width - inset * 2 - gap) / 2);
        var leftSize = _left.GetPreferredSize(new Size(columnWidth, 0));
        var rightSize = _right.GetPreferredSize(new Size(columnWidth, 0));
        return new Size(width, inset * 2 + Math.Max(leftSize.Height, rightSize.Height));
    }

    protected override void OnLayout(LayoutEventArgs levent)
    {
        base.OnLayout(levent);
        var gap = ScalePx(8);
        var inset = ScalePx(1);
        var width = Math.Max(1, ClientSize.Width);
        var columnWidth = Math.Max(1, (width - inset * 2 - gap) / 2);
        var leftSize = _left.GetPreferredSize(new Size(columnWidth, 0));
        var rightSize = _right.GetPreferredSize(new Size(columnWidth, 0));
        var height = Math.Max(leftSize.Height, rightSize.Height);
        _left.Bounds = new Rectangle(inset, inset, columnWidth, leftSize.Height);
        _right.Bounds = new Rectangle(inset + columnWidth + gap, inset, columnWidth, rightSize.Height);
        if (Height != inset * 2 + height)
        {
            Height = inset * 2 + height;
        }
    }

    private int ScalePx(int value) => Math.Max(1, (int)Math.Round(value * _uiScale));
}
