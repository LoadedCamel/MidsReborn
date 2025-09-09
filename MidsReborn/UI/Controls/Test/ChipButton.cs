using Mids_Reborn.UI.Theming;
using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace Mids_Reborn.UI.Controls.Test;

public sealed class ChipButton : Control
{
    public bool Selected
    {
        get => _selected;
        set { if (_selected == value) return; _selected = value; Invalidate(); SelectedChanged?.Invoke(this, value); }
    }
    private bool _selected;

    public event EventHandler<bool>? SelectedChanged;

    [Browsable(false)] public DataViewTheme Theme { get; set; } = new();

    private float DpiScale => DeviceDpi / 96f;
    private int ScalePx(int v) => (int)Math.Round(v * DpiScale);

    public ChipButton()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        Font = new Font("Segoe UI", 9f, FontStyle.Bold);
        Padding = new Padding(ScalePx(12), ScalePx(4), ScalePx(12), ScalePx(4));
        AutoSize = true;
        Cursor = Cursors.Hand;
    }
    protected override void OnMouseDown(MouseEventArgs e) { Selected = !Selected; base.OnMouseDown(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var r = new Rectangle(0, 0, Width - 1, Height - 1);
        var bgc = Selected ? Theme.ChipActive : Theme.Chip;
        using (var b = new SolidBrush(bgc)) g.FillPath(b, UiUtil.Capsule(r, ScalePx(12)));
        using (var p = new Pen(Color.FromArgb(20, Theme.Border))) g.DrawPath(p, UiUtil.Capsule(r, ScalePx(12)));

        var flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis;
        TextRenderer.DrawText(g, Text, Font, r, Theme.Text, flags);
    }
}