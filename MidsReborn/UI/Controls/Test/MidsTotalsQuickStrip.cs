using System.ComponentModel;

namespace Mids_Reborn.UI.Controls.Test;

[DesignerCategory("Code")]
internal sealed class MidsTotalsQuickStrip : Panel
{
    private readonly List<MidsTotalsQuickChip> _chips = [];
    private float _uiScale = 1f;

    public float UiScale
    {
        get => _uiScale;
        set
        {
            var clamped = Math.Clamp(value, 0.90f, 1.25f);
            if (Math.Abs(_uiScale - clamped) < 0.01f) return;
            _uiScale = clamped;
            foreach (var chip in _chips)
            {
                chip.UiScale = _uiScale;
            }
            PerformLayout();
            Invalidate();
        }
    }

    public MidsTotalsQuickStrip()
    {
        BackColor = Color.Transparent;
        Margin = Padding.Empty;
        TabStop = false;
    }

    public void SetMetrics(IReadOnlyList<TotalsQuickMetric> metrics)
    {
        while (_chips.Count > metrics.Count)
        {
            var index = _chips.Count - 1;
            var chip = _chips[index];
            Controls.Remove(chip);
            chip.Dispose();
            _chips.RemoveAt(index);
        }

        for (var i = 0; i < metrics.Count; i++)
        {
            if (i >= _chips.Count)
            {
                var chip = new MidsTotalsQuickChip
                {
                    Margin = Padding.Empty,
                    UiScale = _uiScale
                };
                _chips.Add(chip);
                Controls.Add(chip);
            }

            _chips[i].Metric = metrics[i];
        }

        PerformLayout();
    }

    public override Size GetPreferredSize(Size proposedSize)
    {
        var width = proposedSize.Width > 0 ? proposedSize.Width : Math.Max(ScalePx(320), Width);
        return new Size(width, MeasureHeight(width));
    }

    protected override void OnLayout(LayoutEventArgs levent)
    {
        base.OnLayout(levent);

        var width = Math.Max(ScalePx(240), ClientSize.Width);
        var height = MeasureAndArrange(width, applyBounds: true);
        if (Height != height)
        {
            Height = height;
        }
    }

    private int MeasureHeight(int width) => MeasureAndArrange(width, applyBounds: false);

    private int MeasureAndArrange(int width, bool applyBounds)
    {
        if (_chips.Count == 0)
        {
            return 0;
        }

        var gap = ScalePx(5);
        var inset = ScalePx(1);
        var chipHeight = _chips[0].GetPreferredSize(Size.Empty).Height;
        var minChipWidth = ScalePx(76);
        var columns = _chips.Count >= 4 && width >= (minChipWidth * 4) + (gap * 3)
            ? 4
            : Math.Min(2, _chips.Count);
        columns = Math.Max(1, columns);
        var rows = (int)Math.Ceiling(_chips.Count / (double)columns);
        var available = Math.Max(1, width - inset * 2 - gap * (columns - 1));
        var chipWidth = Math.Max(ScalePx(72), available / columns);

        if (applyBounds)
        {
            for (var i = 0; i < _chips.Count; i++)
            {
                var row = i / columns;
                var col = i % columns;
                var x = inset + col * (chipWidth + gap);
                var y = inset + row * (chipHeight + gap);
                _chips[i].Bounds = new Rectangle(x, y, chipWidth, chipHeight);
            }
        }

        return inset * 2 + rows * chipHeight + (rows - 1) * gap;
    }

    private int ScalePx(int value) => Math.Max(1, (int)Math.Round(value * _uiScale));
}
