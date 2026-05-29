using System.Reflection;

namespace Mids_Reborn.UI.Controls;

internal static class WinFormsBuffering
{
    private static readonly PropertyInfo? DoubleBufferedProperty =
        typeof(Control).GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo? SetStyleMethod =
        typeof(Control).GetMethod("SetStyle", BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo? UpdateStylesMethod =
        typeof(Control).GetMethod("UpdateStyles", BindingFlags.Instance | BindingFlags.NonPublic);

    private const ControlStyles BufferedStyles =
        ControlStyles.AllPaintingInWmPaint |
        ControlStyles.OptimizedDoubleBuffer |
        ControlStyles.ResizeRedraw;

    internal static void Enable(Control? control)
    {
        if (control is null || control.IsDisposed)
        {
            return;
        }

        DoubleBufferedProperty?.SetValue(control, true, null);
        SetStyleMethod?.Invoke(control, [BufferedStyles, true]);
        UpdateStylesMethod?.Invoke(control, null);
    }
}
