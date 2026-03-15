using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.UI.Controls.GfxModules;
using Mids_Reborn.UI.Controls.GfxModules.GfxBackend;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Controls;

public class ClsDrawX : BufferedGraphicsBackend
{
    public enum GfxBackendType
    {
        BxBuffer,
        BufferedGraphics
    }

    // Horizontal space between power slots
    public const int PaddingX = 15;

    // Vertical space between power slots
    internal const int PaddingY = 25;

    // Vertical offset for enhancement slots
    public const int OffsetY = 23;

    // Horizontal offset for enhancement slots
    internal const int OffsetX = 30;

    internal const int OffsetInherent = 10;

    // Same size as target drawing area
    private Size _szBuffer;

    // Size of a power slot
    public Size SzPower { get; set; }

    // Size of an enhancement slot
    public Size SzSlot;

    // List of disabled, empty, filled, waiting
    public readonly List<ExtendedBitmap> BxPower;

    // The unplaced enhancement slot image
    public ExtendedBitmap? BxNewSlot;

    // Column variables
    internal const int VcPowers = 24;
    internal int _vcCols;
    internal int _vcRowsPowers;
    internal Enums.eColumnStacking _ColumnStackingMode = Enums.eColumnStacking.None;
    internal Dictionary<int, Point> ColumnsPowersLayout;
    internal int LayoutColumns;
    internal bool HasNullColumn;

    // Recoloring variables
    internal ColorMatrix? _pColorMatrix;
    public ImageAttributes? PImageAttributes;

    // Scaling variables
    internal bool Scaling { get; set; } = true;

    // Identity matrix (no color filtering)
    public static readonly float[][] HeroMatrix =
    [
        [
            1, 0, 0, 0, 0
        ],
        [
            0, 1, 0, 0, 0
        ],
        [
            0, 0, 1, 0, 0
        ],
        [
            0, 0, 0, 1, 0
        ],
        [
            0, 0, 0, 0, 1
        ]
    ];

    public static readonly float[][] DesaturateMatrix =
    [
        [
            0.299f, 0.299f, 0.299f, 0f, 0f
        ],
        [
            0.587f, 0.587f, 0.587f, 0f, 0f
        ],
        [
            0.114f, 0.114f, 0.114f, 0f, 0f
        ],
        [
            0, 0, 0, 1f, 0
        ],
        [
            0, 0, 0, 0, 1f
        ]
    ];

    internal const int IcoOffset = 32;
    internal const int ToggleButtonSize = 15;

    private Control _cTarget;
    public int Highlight;
    public Enums.eInterfaceMode InterfaceMode;
    internal readonly bool _inDesigner = AppDomain.CurrentDomain.FriendlyName.Contains("devenv"); // ???

    public ClsDrawX(Control ctl)
    {
        InterfaceMode = 0;
        _vcCols = 6;
        _vcRowsPowers = 24;
        BxPower = [];
        ColumnsPowersLayout = new Dictionary<int, Point>();

        this.ColorSwitch();
        InitColumns = MidsContext.Config.Columns;
        _cTarget = ctl;
        InitializeAsync();
        PrepareTarget(ctl);
        
        _defaultFont = new Font(Fonts.Family("Noto Sans"), 12.25f, FontStyle.Bold, GraphicsUnit.Pixel, 0);
        _backColor = ctl.BackColor;
        
        if (_szBuffer.Height < _cTarget.Height)
        {
            _gTarget?.FillRectangle(new SolidBrush(_backColor), 0, _szBuffer.Height, _cTarget.Width, _cTarget.Height - _szBuffer.Height);
        }
    }

    private async void InitializeAsync()
    {
        var gfxImages = await I9Gfx.LoadButtons();
        var buttonPaths = gfxImages.Where(gfxImage => !string.IsNullOrWhiteSpace(gfxImage)).ToList();
        var firstPath = buttonPaths.First();
        if (string.IsNullOrWhiteSpace(firstPath)) throw new ArgumentException("Image path cannot be null or empty");
        foreach (var buttonPath in buttonPaths.OfType<string>())
        {
            BxPower.Add(new ExtendedBitmap(Image.FromFile(buttonPath)));
        }

        SzPower = BxPower[0].Size;


        var slotImagePath = await I9Gfx.LoadNewSlot();
        if (slotImagePath != null)
        {
            var slotImage = Image.FromFile(slotImagePath);
            BxNewSlot = new ExtendedBitmap(slotImage);
        }

        if (BxNewSlot != null) SzSlot = BxNewSlot.Size;

        _szBuffer = this.GetMaxDrawingArea();

        if (BackendType == GfxBackendType.BxBuffer)
        {
            /*BxBuffer = new ExtendedBitmap(_szBuffer.Width, _szBuffer.Height);
            if (BxBuffer.Graphics != null)
            {
                BxBuffer.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                BxBuffer.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                BxBuffer.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                BxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                BxBuffer.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                BxBuffer.Graphics.CompositingMode = CompositingMode.SourceOver;
                BxBuffer.Graphics.PageUnit = GraphicsUnit.Pixel;
            }*/
        }

        _szBuffer = this.GetRequiredDrawingArea();
    }

    public static bool EpicColumns => MidsContext.Character is { Archetype.ClassType: Enums.eClassType.HeroEpic };

    public int Columns
    {
        set
        {
            this.MiniSetCol(value);
            Blank();
            _szBuffer = this.GetRequiredDrawingArea();
            this.SetScaling(_cTarget.Size);
        }
    }

    // Useless, need cleanup
    public Enums.eColumnStacking ColumnStackingMode
    {
        set => _ColumnStackingMode = value;
        get => _ColumnStackingMode;
    }

    private int InitColumns
    {
        init
        {
            if (value == _vcCols)
            {
                return;
            }

            if (value is < 2 or > 6)
            {
                return;
            }

            _vcCols = value;
            _vcRowsPowers = VcPowers / _vcCols;
        }
    }

    public void FullRedraw()
    {
        var s = Stopwatch.StartNew();

        // Bug: off-sync with ColumnStackingMode on load (vertical stacking only ?)
        if (_ColumnStackingMode != MidsContext.Config.ColumnStackingMode)
        {
            _ColumnStackingMode = MidsContext.Config.ColumnStackingMode;
        }

        this.ColorSwitch();
        _backColor = _cTarget.BackColor;
        BxBuffer?.Graphics?.Clear(_backColor);

        this.InitHeadersVariables();
        if (_ColumnStackingMode != Enums.eColumnStacking.None)
        {
            this.GetPowersLayout();
        }

        this.DrawPowers();
        if (_ColumnStackingMode != Enums.eColumnStacking.None)
        {
            // Do not draw headers before powers or scaling will fail.
            this.DrawHeaders();
        }

        try
        {
            OutputUnscaled();
        }
        catch (Exception)
        {
            // Call will fail if loading a build made with a different database
            // and auto switch
        }

        //GC.Collect(); // ??

        s.Stop();

        Debug.WriteLine($"FullRedraw(): {s.ElapsedMilliseconds} ms");
    }
}