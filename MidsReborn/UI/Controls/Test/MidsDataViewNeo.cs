using FastDeepCloner;
using FontAwesome.Sharp;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Controls.Test;
using Mids_Reborn.UI.Renderer;
using Mids_Reborn.UI.Theming;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Mids_Reborn.UI.Controls
{
    public partial class MidsDataViewNeo : UserControl
    {
        #region Constants

        private readonly string[] _tabs = ["INFO", "EFFECTS", "TOTALS", "ENHANCE"];
        private const int TabPaddingX = 16;     // reserved for future text padding if needed
        private const int TabHeight = 24;
        private const int TabSpacing = 4;
        private const int CornerRadius = 2;

        #endregion

        #region Structs

        private struct ItemPairGroup
        {
            public string Label;
            public Func<GroupedFx.FxId, bool> Filter;
            public List<PairedListEx.Item> ItemPairs;
        }

        private struct ItemPairGroupEx
        {
            public string Label;
            public Func<GroupedFx.FxId, bool> Filter;
            public List<KeyValuePair<GroupedFx, PairedListEx.Item>> ItemPairsEx;
        }

        #endregion

        #region Fields

        private int _hoveredTabIndex = -1;
        private int _selectedTabIndex;

        private Form? _floatingHostForm;
        private bool _isDocked = true;

        // Docking/parent data for reliable redocking
        private Control? _originalParent;
        private DockStyle _originalDock;
        private int _originalIndex;
        private Rectangle _originalBounds;

        private readonly Color[] _tabSelectedColors =
        [
            Color.FromArgb(55, 115, 220), // INFO (Blue)
            Color.FromArgb(70, 175, 115), // EFFECTS (Green)
            Color.FromArgb(220, 180, 60), // TOTALS (Gold)
            Color.FromArgb(180, 80, 200)  // ENHANCE (Purple)
        ];


        private bool bFloating;
        private ExtendedBitmap? bxFlip;
        private int HistoryIDX;
        private bool _isLocked;
        private IPower? pBase;
        private IPower? pEnh;
        private IPower? rootPowerBase;
        private IPower? rootPowerEnh;
        private int pLastScaleVal;
        private List<GroupedFx> GroupedRankedEffects;
        private List<KeyValuePair<GroupedFx, PairedListEx.Item>> EffectsItemPairs;

        public PetInfo PetInfo;

        #endregion

        #region Events

        /// <summary>Fires when the selected tab changes.</summary>
        public event EventHandler<int>? TabChanged;

        public delegate void FloatChangeEventHandler();
        public delegate void SlotFlipEventHandler(int powerIndex);
        public delegate void SlotUpdateEventHandler(IPower? power, int val);
        public delegate void LockStateChangedEventHandler(object? sender, bool locked);
        public delegate void EntityDetailsEventHandler(string entityUid, HashSet<string> powers, int basePowerHistoryIdx, PetInfo petInfo);

        public event FloatChangeEventHandler? FloatChange;
        public event SlotFlipEventHandler? SlotFlip;
        public event SlotUpdateEventHandler? SlotUpdate;
        public event LockStateChangedEventHandler? LockStateChanged;
        public event EntityDetailsEventHandler EntityDetails;

        #endregion

        #region Properties

        public bool IsLocked
        {
            get => _isLocked;
            set => SetLock(value, true);
        }

        private DataViewTheme CurrentTheme
        {
            get
            {
                if (DesignMode)
                {
                    return ThemeManager.DesignTime.DataView;
                }
                return ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;
            }
        }

        #endregion

        #region Constructor

        public MidsDataViewNeo()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);

            // Ensure the header panel itself is double-buffered (prevents flicker)
            EnableDoubleBuffer(headerPanel);

            
            LockButton.Click += LockButton_Click;
            ApplyLockVisuals();

            _selectedTabIndex = 0;
            SelectTab(_selectedTabIndex);
            //dvPages.SelectedIndexChanged += DvPages_SelectedIndexChanged;

            PetInfo = new PetInfo();
            if (!DesignMode) ThemeManager.ThemeChanged += Invalidate;
        }

        #endregion

        #region Paint (Header)

        private void HeaderPanel_Paint(object? sender, PaintEventArgs e)
        {
            // DO NOT call base.OnPaint(e) here: this is an event handler, not an override.
            var g = e.Graphics;

            // Quality settings (cheap here, header is small)
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Background
            // using (var backgroundBrush = new SolidBrush(Color.FromArgb(32, 32, 32)))
            // {
            //     g.FillRectangle(backgroundBrush, headerPanel.ClientRectangle);
            // }

            var hr = headerPanel.ClientRectangle;
            using (var backgroundBrush = new LinearGradientBrush(hr, CurrentTheme.HeaderTop, CurrentTheme.HeaderBottom, LinearGradientMode.Vertical ))
            {
                g.FillRectangle(backgroundBrush, hr);
            }

            // Effective tab area width (exclude right-side buttons and spacing)
            var rightButtonsWidth = DockButton.Width + LockButton.Width;
            var availableWidth = Math.Max(0, headerPanel.ClientSize.Width - rightButtonsWidth - (TabSpacing * (_tabs.Length + 1)));

            // Precompute tab rectangles (distributes remainder pixels evenly)
            var tabRects = ComputeTabRects(availableWidth, _tabs.Length, new Point(TabSpacing, 2), TabHeight, TabSpacing);

            // Draw tabs
            using var hoverBrush = new SolidBrush(Color.FromArgb(40, 40, 60));
            using var outlineColor = new SolidBrush(Color.Black); // for outline method
            using var font = new Font(Font.FontFamily, Font.Size, FontStyle.Bold);

            for (int i = 0; i < _tabs.Length; i++)
            {
                var rect = tabRects[i];

                using var path = RoundedRect(rect, CornerRadius);

                if (i == _selectedTabIndex)
                {
                    //using var selectedBrush = new SolidBrush(_tabSelectedColors[i]);
                    using var selectedBrush = new LinearGradientBrush(rect, CurrentTheme.TabActiveTop, CurrentTheme.TabActiveBottom, LinearGradientMode.Vertical);
                    g.FillPath(selectedBrush, path);
                }
                else if (i == _hoveredTabIndex)
                {
                    g.FillPath(hoverBrush, path);
                }
                else
                {
                    using var inactiveBrush = new LinearGradientBrush(rect, CurrentTheme.TabInactiveTop, CurrentTheme.TabInactiveBottom, LinearGradientMode.Vertical);
                    g.FillPath(inactiveBrush, path);
                }

                DrawTextWithOutline(g, _tabs[i], font, rect, Color.White, Color.Black);
            }
        }

        private static Rectangle[] ComputeTabRects(int totalWidth, int count, Point origin, int height, int spacing)
        {
            // Divide width evenly and distribute any remainder to the left-most tabs
            int baseWidth = count > 0 ? totalWidth / count : 0;
            int remainder = count > 0 ? totalWidth % count : 0;

            var rects = new Rectangle[count];
            int x = origin.X;

            for (int i = 0; i < count; i++)
            {
                int w = baseWidth + (i < remainder ? 1 : 0);
                // include spacing between tabs
                rects[i] = new Rectangle(x, origin.Y, w, height);
                x += w + spacing;
            }

            return rects;
        }

        private static void DrawTextWithOutline(Graphics g, string text, Font font, Rectangle bounds, Color foreColor, Color outlineColor)
        {
            var flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

            // simple 1px outline
            const int off = 1;
            for (int dx = -off; dx <= off; dx++)
            {
                for (int dy = -off; dy <= off; dy++)
                {
                    if (dx == 0 && dy == 0) continue;
                    var shadowRect = new Rectangle(bounds.X + dx, bounds.Y + dy, bounds.Width, bounds.Height);
                    TextRenderer.DrawText(g, text, font, shadowRect, outlineColor, flags);
                }
            }

            TextRenderer.DrawText(g, text, font, bounds, foreColor, flags);
        }

        #endregion

        #region Mouse (Header)

        private void HeaderPanel_MouseMove(object? sender, MouseEventArgs e)
        {
            // Ignore hover when over either right-side button
            if (DockButton.Bounds.Contains(e.Location) || LockButton.Bounds.Contains(e.Location))
            {
                if (_hoveredTabIndex != -1)
                {
                    _hoveredTabIndex = -1;
                    headerPanel.Invalidate();
                }
                return;
            }

            // Hit-testing uses the same rect math as Paint
            var rightButtonsWidth = DockButton.Width + LockButton.Width;
            var availableWidth = Math.Max(0, headerPanel.ClientSize.Width - rightButtonsWidth - (TabSpacing * (_tabs.Length + 1)));
            var tabRects = ComputeTabRects(availableWidth, _tabs.Length, new Point(TabSpacing, 2), TabHeight, TabSpacing);

            int newHovered = -1;
            for (int i = 0; i < tabRects.Length; i++)
            {
                if (tabRects[i].Contains(e.Location))
                {
                    newHovered = i;
                    break;
                }
            }

            if (newHovered != _hoveredTabIndex)
            {
                _hoveredTabIndex = newHovered;
                headerPanel.Invalidate();
            }
        }

        private void HeaderPanel_MouseLeave(object? sender, EventArgs e)
        {
            if (_hoveredTabIndex != -1)
            {
                _hoveredTabIndex = -1;
                headerPanel.Invalidate();
            }
        }

        private void HeaderPanel_MouseDown(object? sender, MouseEventArgs e)
        {
            if (_hoveredTabIndex >= 0 && _hoveredTabIndex != _selectedTabIndex)
            {
                SelectTab(_hoveredTabIndex);
            }
        }

        #endregion

        #region Public API

        public void SelectTab(int index)
        {
            if (index < 0 || index > _tabs.Length - 1 || index == _selectedTabIndex)
                return;

            // If it was already the selected index, ensure header state is consistent
            if (_selectedTabIndex != index)
            {
                _selectedTabIndex = index;
                dvPages.SelectedIndex = _selectedTabIndex;
                headerPanel.Invalidate();
                TabChanged?.Invoke(this, index);
            }
        }

        public void SetData(IPower? basePower, IPower? enhancedPower, bool noLevel = false, bool locked = false, int iHistoryIdx = -1)
        {
            if (basePower == null)
            {
                return;
            }

            IsLocked = locked;

            var basePowerData = new Power(basePower);
            var enhancedPowerData = new Power(enhancedPower);

            // (Optional) show the redirect parent in the header if present
            var rootPowerName = Power.GetRootPowerName(iHistoryIdx, basePower, enhancedPower);
            rootPowerBase = string.IsNullOrEmpty(rootPowerName) ? null : DatabaseAPI.GetPowerByFullName(rootPowerName);
            rootPowerEnh = string.IsNullOrEmpty(rootPowerName) ? null : MainModule.MidsController.Toon?.GetEnhancedPower(iHistoryIdx);

            if ((enhancedPowerData?.PowerIndex ?? -1) == -1 && basePowerData.PowerIndex == -1)
            {
                pBase = null;
            }
            else
            {
                pBase = basePowerData;
            }

            // Build pEnh consistently: if none provided, synthesize from base
            pEnh = (enhancedPowerData?.PowerIndex ?? -1) == -1
                ? new Power(basePower) { PowerIndex = -1 }
                : enhancedPowerData;

            // Data sent to the Dataview may differ from DB.
            // Not needed if ActivatePeriod absorb from summons is disabled in Power.AbsorbPetEffects()
            /*var dbPower = DatabaseAPI.GetPowerByFullName(pBase.FullName);
            if (dbPower != null)
            {
                pBase.ActivatePeriod = dbPower.ActivatePeriod;
                pEnh.ActivatePeriod = dbPower.ActivatePeriod;
            }*/

            pBase?.ProcessExecutes();
            pBase?.ApplyModifyEffects();

            // Do not run ApplyModifyEffects() on pEnh, this is done within totals calculations
            pEnh?.ProcessExecutes();

            GroupedRankedEffects = GroupedFx.AssembleGroupedEffects(pEnh);
            //EffectsItemPairs = GroupedFx.GenerateListItems(GroupedRankedEffects, pBase, pEnh, pEnh?.GetRankedEffects(true).ToList(), effectDataList1.Font.Size);

            HistoryIDX = iHistoryIdx;
            SetDamageTip();
            DisplayData(noLevel);
        }

        public void SetData()
        {
            pBase?.ProcessExecutes();
            pBase?.ApplyModifyEffects();

            pEnh?.ProcessExecutes();

            GroupedRankedEffects = GroupedFx.AssembleGroupedEffects(pEnh);
            //EffectsItemPairs = GroupedFx.GenerateListItems(GroupedRankedEffects, pBase, pEnh, pEnh?.GetRankedEffects(true).ToList(), effectDataList1.Font.Size);

            SetDamageTip();
            DisplayData();
        }

        public void SetEnhancement(I9Slot iEnh, int iLevel = -1)
        {
            if ((_isLocked & _selectedTabIndex != 3) || iLevel < 0)
            {
                return;
            }

            string str1;
            if (iEnh.Enh > -1)
            {
                str1 = DatabaseAPI.Database.Enhancements[iEnh.Enh].LongName;
                if (str1.Length > 38 & iLevel > -1)
                {
                    str1 = DatabaseAPI.GetEnhancementNameShortWSet(iEnh.Enh);
                }
            }
            else
            {
                str1 = pBase.DisplayName;
            }

            if (iLevel > -1 & !MidsContext.Config.ShowSlotLevels)
            {
                str1 += $" (Slot Level {iLevel + 1})";
            }

            title.Text = str1;
            if (_selectedTabIndex > 1 || iEnh.Enh < 0)
            {
                return;
            }

            var iStr1 = string.Empty;
            var str2 = string.Empty;
            if (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.InventO | DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.SetO)
            {
                iStr1 = $"{RTF.Color(RTF.ElementID.Invention)}Invention Level: {iEnh.IOLevel + 1}{Enums.GetRelativeString(iEnh.RelativeLevel, false)}{RTF.Color(RTF.ElementID.Text)}";
            }

            switch (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID)
            {
                case Enums.eType.SetO:
                    if (DatabaseAPI.Database.Enhancements[iEnh.Enh].Unique)
                    {
                        iStr1 += $"{RTF.Color(RTF.ElementID.Warning)} (Unique) {RTF.Color(RTF.ElementID.Text)}";
                    }

                    if (DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance is < 1 and > 0)
                    {

                        str2 += $"{RTF.Color(RTF.ElementID.Enhancement)}{DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance * 100:#0.##)} % chance of ";
                    }

                    break;

                case Enums.eType.SpecialO:
                    iStr1 += RTF.Color(RTF.ElementID.Enhancement) + "Hamidon/Synthetic Hamidon Origin Enhancement";
                    break;

                default:
                    if (iStr1 != string.Empty)
                    {
                        iStr1 += " - ";
                    }

                    iStr1 += GetEnhancementStringRtf(iEnh);
                    break;
            }

            string iStr2;
            if (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.SetO)
            {
                iStr2 = str2 + GetEnhancementStringLongRtf(iEnh) + "\r\n" + EnhancementSetCollection.GetSetInfoLongRTF(DatabaseAPI.Database.Enhancements[iEnh.Enh].nIDSet);
            }
            else
            {
                var str3 = str2 + DatabaseAPI.Database.Enhancements[iEnh.Enh].Desc;
                if (str3 != string.Empty)
                {
                    str3 += "\r\n";
                }

                iStr2 = str3 + GetEnhancementStringLongRtf(iEnh);
            }

            //infoLDesc.Rtf = RTF.StartRTF() + RTF.ToRTF(iStr2) + RTF.EndRTF();
        }

        public void SetEnhancementPicker(I9Slot iEnh)
        {
            if (iEnh.Enh < 0)
            {
                title.Text = "No Enhancement";
            }

            title.Text = DatabaseAPI.Database.Enhancements[iEnh.Enh].LongName;
            if (iEnh.Enh < 0)
            {
                return;
            }

            var str1 = string.Empty;
            var iStr1 = string.Empty;
            if (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID is Enums.eType.InventO or Enums.eType.SetO)
            {
                iStr1 = $"{RTF.Color(RTF.ElementID.Invention)}Invention Level: {iEnh.IOLevel + 1}{Enums.GetRelativeString(iEnh.RelativeLevel, false)}{RTF.Color(RTF.ElementID.Text)}";
            }


            switch (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID)
            {
                case Enums.eType.SetO:
                    if (DatabaseAPI.Database.Enhancements[iEnh.Enh].Unique)
                    {
                        iStr1 += $"{RTF.Color(RTF.ElementID.Warning)} (Unique) {RTF.Color(RTF.ElementID.Text)}";
                    }

                    if (DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance is < 1 and > 0)
                    {
                        str1 +=
                            $"{RTF.Color(RTF.ElementID.Enhancement)}{DatabaseAPI.Database.Enhancements[iEnh.Enh].EffectChance * 100:#0.##)} % chance of ";
                    }

                    break;

                case Enums.eType.SpecialO:
                    iStr1 += "Hamidon/Synthetic Hamidon Origin Enhancement";
                    break;

                default:
                    if (iStr1 != string.Empty)
                    {
                        iStr1 += " - ";
                    }

                    iStr1 += GetEnhancementStringRtf(iEnh);
                    break;
            }

            string iStr2;
            if (DatabaseAPI.Database.Enhancements[iEnh.Enh].TypeID == Enums.eType.SetO)
            {
                // Fix strange white "-2" showing at the end of the enhancement long text
                /*iStr2 = str1 + GetEnhancementStringLongRTF(iEnh) + RTF.Size(RTF.SizeID.Tiny) + "\r\n" +
                        EnhancementSetCollection.GetSetInfoLongRTF(DatabaseAPI.Database.Enhancements[iEnh.Enh].nIDSet);*/

                iStr2 = str1 + GetEnhancementStringLongRtf(iEnh) + "\r\n" +
                        EnhancementSetCollection.GetSetInfoLongRTF(DatabaseAPI.Database.Enhancements[iEnh.Enh].nIDSet);
            }
            else
            {
                var str2 = str1 + DatabaseAPI.Database.Enhancements[iEnh.Enh].Desc;
                if (str2 != string.Empty)
                {
                    str2 += "\r\n";
                }

                iStr2 = str2 + GetEnhancementStringLongRtf(iEnh);
            }

            //infoLDesc.Rtf = RTF.StartRTF() + RTF.ToRTF(iStr2) + RTF.EndRTF();
        }

        public void DisplayTotals()
        {
            if (MidsContext.Character == null)
            {
                return;
            }

            var dmgNames = Enum.GetNames(typeof(Enums.eDamage));
            var displayStats = MidsContext.Character.DisplayStats;
            coreDataList.Clear(true);
            defenseGraph1.Clear();
            defenseGraph2.Clear();
            var numArray1 = new[]
            {
                0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0
            };

            var unusedVectors = new List<Enums.eDamage>
            {
                Enums.eDamage.Special,
                Enums.eDamage.Unique1,
                Enums.eDamage.Unique2,
                Enums.eDamage.Unique3
            }.Cast<int>();
            const int toxicVector = (int)Enums.eDamage.Toxic;

            for (var dType = 1; dType < dmgNames.Length; dType++)
            {
                var iTip = $"{displayStats.Defense(dType):0.##}% {dmgNames[dType]} defense";
                if (dType == toxicVector && !DatabaseAPI.RealmUsesToxicDef())
                {
                    continue;
                }

                if (unusedVectors.Contains(dType))
                {
                    continue;
                }

                var targetGraph = numArray1[dType] == 0 ? defenseGraph1 : defenseGraph2;
                //var targetGraph = dType % 2 == 1 ? gDef1 : gDef2;
                targetGraph.AddItem($"{dmgNames[dType]}:|{displayStats.Defense(dType):0.#}%", Math.Max(0, displayStats.Defense(dType)), 0, iTip);
            }

            var maxValue1 = Math.Max(defenseGraph1.GetMaxValue(), defenseGraph2.GetMaxValue());
            defenseGraph1.Max = maxValue1;
            defenseGraph2.Max = maxValue1;
            defenseGraph1.Draw();
            defenseGraph2.Draw();

            var atResCap = $"{MidsContext.Character.Archetype.DisplayName} resistance cap: {MidsContext.Character.Archetype.ResCap * 100:0.##}%";
            resistGraph1.Clear();
            resistGraph2.Clear();
            var numArray2 = new[]
            {
                0, 0, 0, 1, 1, 0, 0, 1, 1, 0, 1, 1, 1
            };

            unusedVectors = new List<Enums.eDamage>
            {
                Enums.eDamage.Melee,
                Enums.eDamage.Ranged,
                Enums.eDamage.AoE,
                Enums.eDamage.Special,
                Enums.eDamage.Unique1,
                Enums.eDamage.Unique2,
                Enums.eDamage.Unique3
            }.Cast<int>();

            for (var dType = 1; dType < dmgNames.Length; dType++)
            {
                if (unusedVectors.Contains(dType))
                {
                    continue;
                }

                var iTip = MidsContext.Character.TotalsCapped.Res[dType] < MidsContext.Character.Totals.Res[dType]
                    ? $"{displayStats.DamageResistance(dType, true):0.##}% {dmgNames[dType]} resistance capped at {displayStats.DamageResistance(dType, false):0.##}%"
                    : $"{displayStats.DamageResistance(dType, true):0.##}% {dmgNames[dType]} resistance. ({atResCap})";

                var targetGraph = numArray2[dType] == 0 ? resistGraph1 : resistGraph2;
                targetGraph.AddItem($"{dmgNames[dType]}:|{displayStats.DamageResistance(dType, false):0.#}%", Math.Max(0, displayStats.DamageResistance(dType, false)), Math.Max(0, displayStats.DamageResistance(dType, true)), iTip);
            }

            var maxValue2 = Math.Max(resistGraph1.GetMaxValue(), resistGraph2.GetMaxValue());
            resistGraph1.Max = maxValue2;
            resistGraph2.Max = maxValue2;
            resistGraph1.Draw();
            resistGraph2.Draw();

            var iTip1 = string.Empty;
            var iTip2 = $"Time to go from 0-100% end: {Utilities.FixDP(displayStats.EnduranceTimeToFull)}s.\r\nHover the mouse over the End Drain stats for more info.";
            switch (displayStats.EnduranceRecoveryNet)
            {
                case > 0:
                    {
                        iTip1 = $"Net Endurance Gain (Recovery - Drain): {Utilities.FixDP(displayStats.EnduranceRecoveryNet)}/s.";
                        if (Math.Abs(displayStats.EnduranceRecoveryNet - displayStats.EnduranceRecoveryNumeric) > float.Epsilon)
                        {
                            iTip1 += $"\r\nTime to go from 0-100% end (using net gain): {Utilities.FixDP(displayStats.EnduranceTimeToFullNet)}s.";
                        }

                        break;
                    }
                case < 0:
                    iTip1 = $"With current end drain, you will lose end at a rate of: {Utilities.FixDP(displayStats.EnduranceRecoveryLossNet)}/s.\r\nFrom 100% you would run out of end in: {Utilities.FixDP(displayStats.EnduranceTimeToZero)}s.";
                    break;
            }

            var iTip3 = $"Time to go from 0-100% health: {Utilities.FixDP(displayStats.HealthRegenTimeToFull)}s.\r\nHealth regenerated per second: {Utilities.FixDP(displayStats.HealthRegenHealthPerSec)}%\r\nHitPoints regenerated per second at level 50: {Utilities.FixDP(displayStats.HealthRegenHPPerSec)} HP";
            coreDataList.AddItem(new PairedListEx.Item("Recovery:", $"{displayStats.EnduranceRecoveryPercentage(false):0.##}% ({displayStats.EnduranceRecoveryNumeric:0.#}/s)", false, false, false, iTip2));
            coreDataList.AddItem(new PairedListEx.Item("Regen:", $"{displayStats.HealthRegenPercent(false):0.##}%", false, false, false, iTip3));
            coreDataList.AddItem(new PairedListEx.Item("EndDrain:", $"{displayStats.EnduranceUsage:0.##}/s", false, false, false, iTip1));
            coreDataList.AddItem(new PairedListEx.Item("+ToHit:", $"{displayStats.BuffToHit:0.##}%", false, false, false, "This effect is increasing the accuracy of all your powers."));
            coreDataList.AddItem(new PairedListEx.Item("+EndRdx:", $"{displayStats.BuffEndRdx:0.##}%", false, false, false, "The end cost of all your powers is being reduced by this effect.\r\nThis is applied like an end-reduction enhancement."));
            coreDataList.AddItem(new PairedListEx.Item("+Recharge:", $"{displayStats.BuffHaste(false) - 100:0.#}%", false, false, false, "The recharge time of your powers is being altered by this effect.\r\nThe higher the value, the faster the recharge."));
            //total_Misc.Rows = 3;
            coreDataList.Redraw();
        }

        public void FlipStage(int Index, int Enh1, int Enh2, float State, int PowerID, Enums.eEnhGrade Grade1, Enums.eEnhGrade Grade2)
        {
            using var solidBrush1 = new SolidBrush(enhDataList.BackColor);
            if (pBase == null)
            {
                return;
            }

            var solidBrush2 = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
            if (PowerID != pBase.PowerIndex)
            {
                return;
            }

            ImageAttributes recolorIa = BuildRenderer.GetRecolorIa(MidsContext.Character.IsHero());
            var rectangle1 = new Rectangle();
            ref var local1 = ref rectangle1;
            var size = bxFlip.Size;
            var x = size.Width - 188 + 30 * Index;
            size = bxFlip.Size;
            var y1 = (int)Math.Round((size.Height / 2.0 - 30.0) / 2.0);
            local1 = new Rectangle(x, y1, 30, 30);
            var destRect = rectangle1;
            bxFlip.Graphics.FillRectangle(solidBrush1, rectangle1);
            var rectangle2 = new Rectangle((int)Math.Round(rectangle1.X + (30.0 - 30.0 * State) / 2.0), rectangle1.Y,
                (int)Math.Round(30.0 * State), 30);
            Graphics graphics;
            if (Enh1 > -1)
            {
                graphics = bxFlip.Graphics;
                AssetManager.DrawFlippingEnhancement(graphics, rectangle1, State,
                    DatabaseAPI.Database.Enhancements[Enh1].ImageIdx,
                    AssetManager.ToGfxGrade(DatabaseAPI.Database.Enhancements[Enh1].TypeID, Grade1));
            }
            else
            {
                bxFlip.Graphics.DrawImage(AssetManager.EmptySlot.Bitmap, rectangle2);
            }

            pnlEnhActive.CreateGraphics().DrawImage(bxFlip.Bitmap, destRect, rectangle1, GraphicsUnit.Pixel);
            ref var local2 = ref rectangle1;
            double y2 = rectangle1.Y;
            size = bxFlip.Size;
            var num1 = size.Height / 2.0;
            var num2 = (int)Math.Round(y2 + num1);
            local2.Y = num2;
            bxFlip.Graphics.FillRectangle(solidBrush1, rectangle1);
            rectangle2 = new Rectangle((int)Math.Round(rectangle1.X + (30.0 - 30.0 * State) / 2.0), rectangle1.Y,
                (int)Math.Round(30.0 * State), 30);
            if (Enh2 > -1)
            {
                graphics = bxFlip.Graphics;
                AssetManager.DrawFlippingEnhancement(graphics, rectangle1, State,
                    DatabaseAPI.Database.Enhancements[Enh2].ImageIdx,
                    AssetManager.ToGfxGrade(DatabaseAPI.Database.Enhancements[Enh2].TypeID, Grade2));
            }
            else
            {
                bxFlip.Graphics.DrawImage(AssetManager.EmptySlot.Bitmap, rectangle2);
            }

            rectangle2.Inflate(2, 2);
            bxFlip.Graphics.FillEllipse(solidBrush2, rectangle2);
            pnlEnhInactive.CreateGraphics().DrawImage(bxFlip.Bitmap, destRect, rectangle1, GraphicsUnit.Pixel);
        }

        public void SetSetPicker(int iSet)
        {
            if (iSet < 0)
            {
                title.Text = "No Enhancement";
                //infoLDesc.Text = "";
            }
            else
            {
                title.Text = DatabaseAPI.Database.EnhancementSets[iSet].DisplayName;
                var str1 = DatabaseAPI.GetSetTypeByIndex(DatabaseAPI.Database.EnhancementSets[iSet].SetType).Name;

                var str2 = DatabaseAPI.Database.EnhancementSets[iSet].LevelMin !=
                           DatabaseAPI.Database.EnhancementSets[iSet].LevelMax
                    ? $"{DatabaseAPI.Database.EnhancementSets[iSet].LevelMin + 1} to {DatabaseAPI.Database.EnhancementSets[iSet].LevelMax + 1}"
                    : $"{DatabaseAPI.Database.EnhancementSets[iSet].LevelMin + 1}";
                //infoLDesc.Rtf = $"{RTF.StartRTF()}{EnhancementSetCollection.GetSetInfoLongRTF(iSet)}{RTF.EndRTF()}";
            }
        }

        public void SetGraphType(Enums.MDmgGraphType graphType, Enums.MDmgDisplayStyle graphStyle)
        {
            infoDamageDisplay.GraphType = graphType;
            infoDamageDisplay.Style = graphStyle;
        }

        #endregion

        #region Private Methods

        private void DvPages_SelectedIndexChanged(object? sender, int pageIndex)
        {
            // Reflect FormPages selection into header state and raise external event
            if (pageIndex < 0 || pageIndex >= _tabs.Length)
                return;

            if (_selectedTabIndex != pageIndex)
            {
                _selectedTabIndex = pageIndex;
                headerPanel.Invalidate();
                TabChanged?.Invoke(this, pageIndex);
            }
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();

            if (bounds.Width <= 0 || bounds.Height <= 0)
                return path; // empty, safe

            // Clamp radius to fit within the rectangle
            int r = Math.Max(0, Math.Min(radius, Math.Min(bounds.Width, bounds.Height) / 2));
            if (r == 0)
            {
                path.AddRectangle(bounds);
                path.CloseFigure();
                return path;
            }

            int d = r * 2;

            // careful with Right/Bottom – subtract diameter to avoid negative sizes
            path.AddArc(bounds.Left, bounds.Top, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Top, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void EnableDoubleBuffer(Control c)
        {
            // HeaderPanel is a Panel; DoubleBuffered is protected, so enable via reflection.
            typeof(Control)
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)!
                .SetValue(c, true, null);
        }

        private static PairedListEx.Item BuildEdItem(int index, float[] value, Enums.eSchedule[] schedule, string name, float[] afterEd)
        {
            var flag1 = value[index] > (double)DatabaseAPI.Database.MultED[(int)schedule[index]][0];
            var flag2 = value[index] > (double)DatabaseAPI.Database.MultED[(int)schedule[index]][1];
            var iSpecialCase = value[index] > (double)DatabaseAPI.Database.MultED[(int)schedule[index]][2];
            PairedListEx.Item itemPair;
            if (value[index] < 0)
            {
                itemPair = new PairedListEx.Item(string.Empty, string.Empty, false, false, false, string.Empty);
            }
            else
            {
                var iName = $"{name}:";
                var num1 = value[index] * 100f;
                var num2 = Enhancement.ApplyED(schedule[index], value[index]) * 100f;
                var num3 = num2 + afterEd[index] * 100f;
                var num4 = (float)Math.Round(num1 - (double)num2, 3);

                var str1 = $"{num1:##0.00} %";
                var str2 = $"{num4:##0.00} %";
                var str3 = $"{num3:##0.00} %";
                var str4 = $"Total Effect: {num1 + afterEd[index] * 100:0.##}%\r\nWith ED Applied: {str3}\r\n\r\n";
                string iValue;
                string iTip;
                if (num4 > 0)
                {
                    iValue = $"{str3} (Pre-ED: {num1 + afterEd[index] * 100:0.##}%)";
                    if (afterEd[index] > 0)
                    {
                        str4 += $"Amount from pre-ED sources: {str1}\r\n";
                    }

                    iTip = $"{str4} ED reduction: {str2} ({num4 / (double)num1 * 100:0.##}% of total)\r\n";
                    if (iSpecialCase)
                    {
                        iTip = $"{iTip} The highest level of ED reduction is being applied.\r\nThreshold: {DatabaseAPI.Database.MultED[(int)schedule[index]][2] * 100:0.##} %\r\n";
                    }
                    else if (flag2)
                    {
                        iTip = $"{iTip} The middle level of ED reduction is being applied.\r\nThreshold: {DatabaseAPI.Database.MultED[(int)schedule[index]][1] * 100:0.##} %\r\n";
                    }
                    else if (flag1)
                    {
                        iTip = $"{iTip} The lowest level of ED reduction is being applied.\r\nThreshold: {DatabaseAPI.Database.MultED[(int)schedule[index]][0] * 100:0.##} %\r\n";
                    }

                    if (afterEd[index] > 0)
                    {
                        iTip = $"{iTip} Amount from post-ED sources: {afterEd[index] * 100:0.##} %\r\n";
                    }
                }
                else
                {
                    iValue = str3;
                    if (afterEd[index] > 0)
                    {
                        str4 = $"{str4} Amount from post-ED sources: {afterEd[index] * 100:0.##} %\r\n";
                    }

                    iTip = $"{str4}This effect has not been affected by ED.\r\n";
                }

                itemPair = new PairedListEx.Item(iName, iValue, flag2 & !iSpecialCase, flag1 & !flag2, iSpecialCase, iTip);
            }

            return itemPair;
        }

        private void DisplayEdFigures()
        {
            if (pBase == null)
            {
                return;
            }

            title.Text = pBase.DisplayName;
            enhDataList.Clear();
            if (MidsContext.Character == null)
            {
                enhDataList.Redraw();

                return;
            }

            var powerBase = rootPowerBase ?? pBase;
            var buildHistoryIdx = MidsContext.Character.CurrentBuild.FindInToonHistory(powerBase.PowerIndex);
            if (buildHistoryIdx < 0)
            {
                enhDataList.Redraw();

                return;
            }

            var eEnhs = Enum.GetValues(typeof(Enums.eEnhance)).Length;
            var buffs = new float[eEnhs];
            var debuffs = new float[eEnhs];
            var buffDebuffs = new float[eEnhs];
            var buffsSchedule = new Enums.eSchedule[eEnhs];
            var debuffsSchedule = new Enums.eSchedule[eEnhs];
            var buffsDebuffsSchedule = new Enums.eSchedule[eEnhs];
            var buffsAfterEd = new float[eEnhs];
            var debuffsAfterEd = new float[eEnhs];
            var buffsDebuffsAfterEd = new float[eEnhs];
            var mezBuffs = new float[Enum.GetValues(typeof(Enums.eMez)).Length];
            var mezSchedule = new Enums.eSchedule[eEnhs];
            var mezAfterED = new float[eEnhs];

            Array.Fill(buffs, 0);
            Array.Fill(debuffs, 0);
            Array.Fill(buffDebuffs, 0);
            Array.Fill(mezBuffs, 0);

            for (var i = 0; i < buffs.Length; i++)
            {
                buffsSchedule[i] = Enhancement.GetSchedule((Enums.eEnhance)i);
                debuffsSchedule[i] = buffsSchedule[i];
                buffsDebuffsSchedule[i] = buffsSchedule[i];
            }

            debuffsSchedule[(int)Enums.eEnhance.Defense] = Enums.eSchedule.A; // 3
            for (var tSub = 0; tSub < mezBuffs.Length; tSub++)
            {
                mezSchedule[tSub] = Enhancement.GetSchedule(Enums.eEnhance.Mez, tSub);
            }

            var buildPower = MidsContext.Character.CurrentBuild.Powers[buildHistoryIdx];
            for (var i = 0; i < buildPower?.SlotCount; i++)
            {
                var slot = buildPower.Slots[i];
                if (slot.Enhancement.Enh <= -1)
                {
                    continue;
                }

                var slotEnh = slot.Enhancement.Enh;
                for (var se = 0; se < DatabaseAPI.Database.Enhancements[slotEnh].Effect.Length; se++)
                {
                    var effect = DatabaseAPI.Database.Enhancements[slotEnh].Effect;
                    if (effect[se].Mode != Enums.eEffMode.Enhancement)
                    {
                        continue;
                    }

                    if (effect[se].Enhance.ID == 12)
                    {
                        mezBuffs[effect[se].Enhance.SubID] += slot.Enhancement.GetEnhancementEffect(Enums.eEnhance.Mez, effect[se].Enhance.SubID, 1);
                    }
                    else
                    {
                        switch (DatabaseAPI.Database.Enhancements[slotEnh].Effect[se].BuffMode)
                        {
                            case Enums.eBuffDebuff.BuffOnly:
                                buffs[effect[se].Enhance.ID] += slot.Enhancement.GetEnhancementEffect((Enums.eEnhance)effect[se].Enhance.ID, -1, 1);
                                break;

                            case Enums.eBuffDebuff.DeBuffOnly:
                                if (effect[se].Enhance.ID is not 6 and not 11 and not 19)
                                {
                                    debuffs[effect[se].Enhance.ID] += slot.Enhancement.GetEnhancementEffect((Enums.eEnhance)effect[se].Enhance.ID, -1, -1);
                                }

                                break;
                            default:
                                buffDebuffs[effect[se].Enhance.ID] += slot.Enhancement.GetEnhancementEffect((Enums.eEnhance)effect[se].Enhance.ID, -1, 1);
                                break;
                        }
                    }
                }
            }

            foreach (var p in MidsContext.Character.CurrentBuild.Powers)
            {
                if (p == null)
                {
                    continue;
                }

                if (!p.StatInclude)
                {
                    continue;
                }

                IPower power1 = new Power(p.Power);
                power1.AbsorbPetEffects();
                power1.ApplyGrantPowerEffects();
                foreach (var effect in power1.Effects)
                {
                    if (power1.PowerType != Enums.ePowerType.GlobalBoost & (!effect.Absorbed_Effect | effect.Absorbed_PowerType != Enums.ePowerType.GlobalBoost))
                    {
                        continue;
                    }

                    if (effect.Absorbed_Effect & effect.Absorbed_Power_nID > -1)
                    {
                        power1 = DatabaseAPI.Database.Power[effect.Absorbed_Power_nID];
                    }

                    var eBuffDebuff = Enums.eBuffDebuff.Any;
                    var flag = false;
                    if (MidsContext.Character.CurrentBuild.Powers[buildHistoryIdx] == null)
                    {
                        continue;
                    }

                    foreach (var b in buildPower.Power.BoostsAllowed)
                    {
                        if (power1 != null && power1.BoostsAllowed.Any(e => b == e))
                        {
                            if (b.Contains("Buff"))
                            {
                                eBuffDebuff = Enums.eBuffDebuff.BuffOnly;
                            }

                            if (b.Contains("Debuff"))
                            {
                                eBuffDebuff = Enums.eBuffDebuff.DeBuffOnly;
                            }

                            flag = true;
                        }

                        if (flag)
                        {
                            break;
                        }
                    }

                    if (!flag)
                    {
                        continue;
                    }

                    switch (effect.EffectType)
                    {
                        case Enums.eEffectType.Enhancement:
                            switch (effect.ETModifies)
                            {
                                case Enums.eEffectType.Defense:
                                    if (effect.DamageType == Enums.eDamage.Smashing)
                                    {
                                        if (effect.IgnoreED)
                                        {
                                            switch (eBuffDebuff)
                                            {
                                                case Enums.eBuffDebuff.BuffOnly:
                                                    buffsAfterEd[(int)Enums.eEnhance.Defense] += effect.BuffedMag; // 3
                                                    break;

                                                case Enums.eBuffDebuff.DeBuffOnly:
                                                    debuffsAfterEd[(int)Enums.eEnhance.Defense] += effect.BuffedMag;
                                                    break;

                                                default:
                                                    buffsDebuffsAfterEd[(int)Enums.eEnhance.Defense] += effect.BuffedMag;
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            switch (eBuffDebuff)
                                            {
                                                case Enums.eBuffDebuff.BuffOnly:
                                                    buffs[(int)Enums.eEnhance.Defense] += effect.BuffedMag; // 3
                                                    break;

                                                case Enums.eBuffDebuff.DeBuffOnly:
                                                    debuffs[(int)Enums.eEnhance.Defense] += effect.BuffedMag;
                                                    break;

                                                default:
                                                    buffDebuffs[(int)Enums.eEnhance.Defense] += effect.BuffedMag;
                                                    break;
                                            }
                                        }
                                    }

                                    break;
                                case Enums.eEffectType.Mez:
                                    if (effect.IgnoreED)
                                    {
                                        mezAfterED[(int)effect.MezType] += effect.BuffedMag;
                                        break;
                                    }

                                    mezBuffs[(int)effect.MezType] += effect.BuffedMag;
                                    break;

                                default:
                                    var rechargeBuffIndex = effect.ETModifies != Enums.eEffectType.RechargeTime
                                        ? Convert.ToInt32(Enum.Parse(typeof(Enums.eEnhance), effect.ETModifies.ToString()))
                                        : (int)Enums.eEnhance.RechargeTime; // 14
                                    if (effect.IgnoreED)
                                    {
                                        buffsDebuffsAfterEd[rechargeBuffIndex] += effect.BuffedMag;
                                        break;
                                    }

                                    buffDebuffs[rechargeBuffIndex] += effect.BuffedMag;
                                    break;
                            }

                            break;
                        default:
                            {
                                if (effect.EffectType == Enums.eEffectType.DamageBuff & effect.DamageType == Enums.eDamage.Smashing)
                                {
                                    switch (effect.IgnoreED)
                                    {
                                        case true:
                                            {
                                                foreach (var b in power1?.BoostsAllowed)
                                                {
                                                    if (b.StartsWith("Res_Damage"))
                                                    {
                                                        buffsDebuffsAfterEd[(int)Enums.eEnhance.Resistance] += effect.BuffedMag; // 18
                                                        break;
                                                    }

                                                    if (!b.StartsWith("Damage"))
                                                    {
                                                        continue;
                                                    }

                                                    buffsDebuffsAfterEd[(int)Enums.eEnhance.Damage] += effect.BuffedMag; // 2
                                                    break;
                                                }

                                                break;
                                            }
                                        default:
                                            {
                                                foreach (var b in power1?.BoostsAllowed)
                                                {
                                                    if (b.StartsWith("Res_Damage"))
                                                    {
                                                        buffDebuffs[(int)Enums.eEnhance.Resistance] += effect.BuffedMag;
                                                        break;
                                                    }

                                                    if (!b.StartsWith("Damage"))
                                                    {
                                                        continue;
                                                    }

                                                    buffDebuffs[(int)Enums.eEnhance.Damage] += effect.BuffedMag;
                                                    break;
                                                }

                                                break;
                                            }
                                    }
                                }

                                break;
                            }
                    }
                }
            }

            var zeroedEnhanceBuffs = new[]
            {
                Enums.eEnhance.HitPoints, Enums.eEnhance.Regeneration, Enums.eEnhance.Recovery // 8, 16, 17
            }.Cast<int>();

            foreach (var buff in zeroedEnhanceBuffs)
            {
                buffs[buff] = 0;
                debuffs[buff] = 0;
                buffDebuffs[buff] = 0;
            }

            var liBuffsDebuffs = new List<List<PairedListEx.Item>>
            {
                new(),
                new(),
                new()
            };

            for (var i = 0; i < buffs.Length; i++)
            {
                if (buffs[i] > 0)
                {
                    liBuffsDebuffs[0].Add(BuildEdItem(i, buffs, buffsSchedule, Enum.GetName(typeof(Enums.eEnhance), i), buffsAfterEd));
                }

                if (debuffs[i] > 0)
                {
                    liBuffsDebuffs[1].Add(BuildEdItem(i, debuffs, debuffsSchedule, $"{Enum.GetName(typeof(Enums.eEnhance), i)} Debuff", debuffsAfterEd));
                }

                if (buffDebuffs[i] > 0)
                {
                    liBuffsDebuffs[2].Add(BuildEdItem(i, buffDebuffs, buffsDebuffsSchedule, Enum.GetName(typeof(Enums.eEnhance), i), buffsDebuffsAfterEd));
                }
            }

            for (var i = 0; i < liBuffsDebuffs.Count; i++)
            {
                if (liBuffsDebuffs[i].Count <= 0)
                {
                    continue;
                }

                var slowIdx = liBuffsDebuffs[i].TryFindIndex(e => e.Name is "Slow:");
                if (slowIdx >= 0 & slowIdx <= liBuffsDebuffs.Count)
                {
                    var slowValue = liBuffsDebuffs[i][slowIdx].Value;
                    liBuffsDebuffs[i] = liBuffsDebuffs[i]
                        .Where(e => !(e.Name is "SpeedFlying:" or "SpeedJumping:" or "SpeedRunning:" & (e.Value != null && e.Value == slowValue)))
                        .ToList();
                }

                for (var j = 0; j < liBuffsDebuffs[i].Count; j++)
                {
                    enhDataList.AddItem(liBuffsDebuffs[i][j]);
                    if (enhDataList.IsSpecialColor())
                    {
                        enhDataList.SetUnique();
                    }
                }
            }

            enhDataList.Redraw();
            DisplayFlippedEnhancements();
        }

        private static bool HasEnhancementCarrier(IPower p, Enums.eEffectType modifies)
        {
            if (p?.Effects == null || p.Effects.Length == 0) return false;
            for (int i = 0; i < p.Effects.Length; i++)
                if (p.Effects[i].EffectType == Enums.eEffectType.Enhancement &&
                    p.Effects[i].ETModifies == modifies)
                    return true;
            return false;
        }

        private List<PowerStatsGrid.Row> BuildCanonicalStatRows(IPower pBase, IPower pEnh)
        {
            const double eps = 1e-6;
            var rows = new List<PowerStatsGrid.Row>();
            bool isToggle = pBase.PowerType == Enums.ePowerType.Toggle;
            bool isAuto = pBase.PowerType == Enums.ePowerType.Auto_;

            // --- Shape: Arc/Radius + Range ---
            if (pBase.Arc > eps)
            {
                rows.Add(new PowerStatsGrid.Row("Arc", pBase.Arc, pEnh.Arc, "°", higherIsBetter: false));
                if (pBase.Range > eps || pEnh.Range > eps)
                    rows.Add(new PowerStatsGrid.Row("Range", pBase.Range, pEnh.Range, "ft", higherIsBetter: true));
            }
            else if (pBase.Radius > eps)
            {
                rows.Add(new PowerStatsGrid.Row("Radius", pBase.Radius, pEnh.Radius, "ft", higherIsBetter: true));
                if (pBase.Range > eps || pEnh.Range > eps)
                    rows.Add(new PowerStatsGrid.Row("Range", pBase.Range, pEnh.Range, "ft", higherIsBetter: true));
            }
            else if (pBase.Range > eps || pEnh.Range > eps)
            {
                rows.Add(new PowerStatsGrid.Row("Range", pBase.Range, pEnh.Range, "ft", higherIsBetter: true));
            }

            // --- Timing ---
            if (!isAuto)
            {
                if (!isToggle && (pBase.CastTime > eps || pEnh.CastTime > eps))
                    rows.Add(new PowerStatsGrid.Row("Cast Time", pBase.CastTime, pEnh.CastTime, "s", higherIsBetter: false,
                        tooltip: $"CastTime: {pEnh.CastTimeBase:0.###}s\nArcana CastTime: {pEnh.ArcanaCastTime:0.###}s"));

                if (!isToggle && (pBase.InterruptTime > eps || pEnh.InterruptTime > eps))
                    rows.Add(new PowerStatsGrid.Row("Interrupt", pBase.InterruptTime, pEnh.InterruptTime, "s", higherIsBetter: false));

                if (isToggle && (pBase.ActivatePeriod > eps || pEnh.ActivatePeriod > eps))
                    rows.Add(new PowerStatsGrid.Row("Activate", pBase.ActivatePeriod, pEnh.ActivatePeriod, "s", higherIsBetter: false,
                        tooltip: "The effects of this toggle are applied at this interval."));
            }

            // Recharge (lower is better)
            if (pBase.RechargeTime > eps || pEnh.RechargeTime > eps)
                rows.Add(new PowerStatsGrid.Row("Recharge", pBase.RechargeTime, pEnh.RechargeTime, "s", higherIsBetter: false,
                    affectedByEd: HasEnhancementCarrier(pEnh, Enums.eEffectType.RechargeTime)));

            // Endurance cost
            if (isToggle)
            {
                if (pBase.ToggleCost > eps || pEnh.ToggleCost > eps)
                    rows.Add(new PowerStatsGrid.Row("End Cost", pBase.ToggleCost, pEnh.ToggleCost, "/s", higherIsBetter: false,
                        affectedByEd: HasEnhancementCarrier(pEnh, Enums.eEffectType.EnduranceDiscount),
                        tooltip: "Per-second endurance upkeep for a running toggle."));
            }
            else
            {
                if (pBase.EndCost > eps || pEnh.EndCost > eps)
                    rows.Add(new PowerStatsGrid.Row("End Cost", pBase.EndCost, pEnh.EndCost, "End", higherIsBetter: false,
                        affectedByEd: HasEnhancementCarrier(pEnh, Enums.eEffectType.EnduranceDiscount)));
            }

            // Accuracy (Real Numbers style multiplier, higher is better)
            double baseAcc = MidsContext.Config.ScalingToHit * pBase.Accuracy;
            double enhAcc = MidsContext.Config.ScalingToHit * pEnh.Accuracy;

            // Show Accuracy only if autohit caveats or a to-hit check is present (matches prior UI logic)
            bool requiresToHit = pBase.Effects.Any(t => t.RequiresToHitCheck);
            bool entitiesAutoHit = pBase.EntitiesAutoHit == Enums.eEntity.None ||
                pBase.Effects.Where(e => e.EffectType == Enums.eEffectType.EntCreate)
                             .SelectMany(e =>
                             {
                                 var ent = DatabaseAPI.Database.Entities.ElementAtOrDefault(e.nSummon);
                                 if (ent == null) return [];
                                 var nps = ent.GetNPowerset();
                                 if (nps.Count == 0) return [];
                                 var psidx = nps[0];
                                 var ps = DatabaseAPI.Database.Powersets.ElementAtOrDefault(psidx);
                                 return ps?.Powers ?? [];
                             })
                             .Any(p => p?.EntitiesAutoHit == Enums.eEntity.None);

            bool showAcc = entitiesAutoHit || requiresToHit || pBase.Range > 20 ||
                           pBase.I9FXPresentP(Enums.eEffectType.Mez, Enums.eMez.Taunt);

            if (showAcc && (baseAcc > eps || enhAcc > eps))
            {
                var star = (pBase.EntitiesAutoHit != Enums.eEntity.None && requiresToHit) ? "*" : "";
                var tip = $"Accuracy multiplier without other buffs (Real Numbers style): {pBase.AccuracyMult:0.00}x" +
                          (star.Length > 0 ? "\n* Autohit power with at least one effect that requires a ToHit roll." : "");
                rows.Add(new PowerStatsGrid.Row($"Accuracy (per activation){star}",
                    baseAcc, enhAcc, "", higherIsBetter: true,
                    affectedByEd: HasEnhancementCarrier(pEnh, Enums.eEffectType.Accuracy), tooltip: tip));
            }

            // Optional: Duration when discoverable (e.g., mez duration as a canonical timing stat)
            var durId = pBase.GetDurationEffectID();
            if (durId > -1 && pBase.Effects[durId].Duration <= 9999)
            {
                var d1 = pBase.Effects[durId].Duration;
                var d2 = pEnh.Effects[durId].Duration;
                rows.Add(new PowerStatsGrid.Row("Duration", d1, d2, "s", higherIsBetter: true));
            }

            return rows;
        }

        private void DisplayInfo(bool noLevel = false, int iEnhLvl = -1)
        {
            if (pBase == null)
            {
                powerStatsGrid.Clear();
                title.Text = string.Empty;
                subTitle.Text = string.Empty;
                //infoLDesc.Rtf = RTF.StartRTF() + RTF.EndRTF();
                infoDamageDisplay.BaseValue = 0;
                infoDamageDisplay.EnhancedValue = 0;
                infoDamageDisplay.MaxEnhancedValue = 0;
                infoDamageDisplay.HighestEnhancedValue = 0;
                infoDamageDisplay.Text = string.Empty;
                return;
            }

            var enhancedPower = (pEnh == null || pEnh.PowerIndex == -1) ? pBase : pEnh;

            // Title + descriptions (unchanged)
            title.Text = !noLevel && pBase.Level > 0
                ? $"[{(rootPowerBase?.Level ?? pBase.Level)}] {pBase.DisplayName}"
                : pBase.DisplayName;
            if (iEnhLvl > -1) title.Text += $" (Slot Level {iEnhLvl + 1})";
            subTitle.Text = "Enhancement Values";

            //var longInfo = Regex.Replace(pBase.DescLongFormatted.Trim().Replace("\0", "").Replace("<br>", RTF.Crlf()), @"\s{2,}", " ");
            //infoLDesc.Rtf = RTF.StartRTF() + RTF.ToRTF(longInfo) + RTF.EndRTF();

            // --- NEW: Canonical stats via PowerCanonicalStats → powerStatsGrid ---
            var statRows = PowerCanonicalStats.BuildRows(pBase, enhancedPower);
            powerStatsGrid.SetRows(statRows);

            // --- Damage visualization (unchanged) ---
            var str1 = "Damage" + (MidsContext.Config.DamageMath.ReturnValue switch
            {
                ConfigData.EDamageReturn.DPS => " Per Second",
                ConfigData.EDamageReturn.DPA => " Per Animation Second",
                _ => ""
            });
            if (MidsContext.Config.DataDamageGraphPercentageOnly) str1 += " (% only)";

            var baseDamage = Math.Abs(pBase.FXGetDamageValue(pBase.PowerIndex > -1 & pEnh?.PowerIndex > -1));
            var enhancedDamage = Math.Abs(enhancedPower.FXGetDamageValue());

            if (pBase.NIDSubPower.Length > 0 & baseDamage == 0 && enhancedDamage == 0)
            {
                infoDamageDisplay.BaseValue = 0;
                infoDamageDisplay.EnhancedValue = 0;
                infoDamageDisplay.MaxEnhancedValue = 0;
                infoDamageDisplay.HighestEnhancedValue = 0;
                infoDamageDisplay.Text = string.Empty;
            }
            else
            {
                var hasPercentDamage = pEnh?.Effects.Any(e =>
                    e.EffectType == Enums.eEffectType.Damage && (e.DisplayPercentage || e.Aspect == Enums.eAspect.Str));
                var dmgMultiplier = hasPercentDamage == true ? MidsContext.Character.Totals.HPMax : 1;

                infoDamageDisplay.BaseValue = Math.Max(0, baseDamage * dmgMultiplier);
                infoDamageDisplay.EnhancedValue = Math.Max(0, enhancedDamage * dmgMultiplier);
                infoDamageDisplay.MaxEnhancedValue = Math.Max(baseDamage * dmgMultiplier * (1 + Enhancement.ApplyED(Enums.eSchedule.A, 2.277f)), enhancedDamage * dmgMultiplier);
                infoDamageDisplay.HighestEnhancedValue = Math.Max(414, enhancedDamage * dmgMultiplier);
                infoDamageDisplay.Text = Math.Abs(enhancedDamage - baseDamage) > float.Epsilon
                    ? $"{enhancedPower.FXGetDamageString(pEnh?.PowerIndex == -1)} ({(hasPercentDamage == true ? $"{Utilities.FixDP(baseDamage * 100)}%" : Utilities.FixDP(baseDamage))})"
                    : pBase.FXGetDamageString(pBase.PowerIndex > -1 & pEnh?.PowerIndex > -1);
            }

            SetPowerScaler();
        }

        private void DisplayData(bool noLevel = false, int iEnhLevel = -1)
        {
            if (IsDisposed)
            {
                // Occurs when trying to load a build made for a different database
                // and auto switch
                return;
            }

            if (!MidsContext.Config.DisableDataDamageGraph)
            {
                infoDamageDisplay.GraphType = MidsContext.Config.DataGraphType;
                infoDamageDisplay.TextAlign = HorizontalAlignment.Center;
                infoDamageDisplay.Style = Enums.MDmgDisplayStyle.TextUnderGraph;
            }
            else
            {
                infoDamageDisplay.TextAlign = HorizontalAlignment.Center;
                infoDamageDisplay.Style = Enums.MDmgDisplayStyle.TextOnly;
            }

            if (pBase != null && pEnh != null)
            {
                // Ensure pEnh has at least as many effects as pBase
                if (pBase.Effects.Length > pEnh.Effects.Length)
                {
                    var swappedFx = SwapExtraEffects(pBase.Effects, pEnh.Effects);
                    pBase.Effects = (IEffect[])swappedFx[0].Clone();
                    pEnh.Effects = (IEffect[])swappedFx[1].Clone();
                }
            }

            //lblLock.Visible = Lock & (_selectedTabIndex != 2);
            DisplayInfo(noLevel, iEnhLevel);
            DisplayEffects(noLevel, iEnhLevel);
            DisplayEdFigures();
        }

        private void DisplayEffects(bool noLevel = false, int iEnhLvl = -1)
        {
            if (pBase == null) return;

            // --- Build UI-agnostic effect items (uses the same ranked effects pipeline) ---
            // GroupedRankedEffects is already set earlier in the flow when powers change 
            var enh = pEnh ?? pBase;
            var rankedSafe = GetRankedEffectsSafe(enh);

            // --- Build Effect groups for the PowerEffectsGrid ---
            // PowerEffects maps items into (Defense/Resistance, Heal/Endurance, Status, Buff/Debuff, Movement, Special, Descriptors)
            var groups = PowerEffects.Build(pBase, enh, GroupedRankedEffects, rankedSafe);

            // --- Push into the grid ---
            effectsGrid.SetGroups(groups);
        }

        private static List<int> GetRankedEffectsSafe(IPower power)
        {
            var ranked = power?.GetRankedEffects(true)?.ToList();
            if (ranked == null || ranked.Count == 0)
            {
                var n = power?.Effects?.Length ?? 0;
                ranked = Enumerable.Range(0, n).ToList();
            }
            return ranked;
        }

        private void DisplayFlippedEnhancements()
        {
            Pen pen;
            if (enhDataList.BackColor.B <= 10)
            {
                pen = new Pen(Color.FromArgb(byte.MaxValue, 0, 0));
            }
            else
            {
                pen = new Pen(Color.FromArgb(0, 0, byte.MaxValue));
            }

            bxFlip ??= new ExtendedBitmap(pnlEnhActive.Width, pnlEnhInactive.Height * 2);
            bxFlip.Graphics.Clear(enhDataList.BackColor);
            bxFlip.Graphics.DrawRectangle(pen, 0, 0, pnlEnhActive.Width - 1, pnlEnhInactive.Height - 1);
            bxFlip.Graphics.DrawRectangle(pen, 0, pnlEnhInactive.Height, pnlEnhActive.Width - 1, pnlEnhInactive.Height - 1);
            if (pBase == null)
            {
                return;
            }

            var powerBase = rootPowerBase ?? pBase;

            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(powerBase.PowerIndex);
            if (inToonHistory < 0)
            {
                RedrawFlip();
            }
            else
            {
                using var format = new StringFormat();
                var num1 = bxFlip.Size.Width - 188;
                var rectangle1 = new Rectangle();
                ref var local1 = ref rectangle1;
                var width = num1;
                var size = bxFlip.Size;
                var height = (int)Math.Round(size.Height / 2.0);
                local1 = new Rectangle(-4, 0, width, height);
                using var solidBrush1 = new SolidBrush(enhDataList.ItemColor);
                format.Alignment = StringAlignment.Far;
                format.LineAlignment = StringAlignment.Center;
                bxFlip.Graphics.DrawString("Active Slotting:", pnlEnhActive.Font, solidBrush1, rectangle1, format);
                rectangle1.Y += rectangle1.Height;
                bxFlip.Graphics.DrawString("Alternate:", pnlEnhActive.Font, solidBrush1, rectangle1, format);
                //ImageAttributes recolorIa = clsDrawX.GetRecolorIa(MidsContext.Character.IsHero());
                using var solidBrush2 = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
                var power = MidsContext.Character.CurrentBuild.Powers[inToonHistory];
                for (var index = 0; index < power.SlotCount; index++)
                {
                    var iDest = new Rectangle();
                    ref var local2 = ref iDest;
                    var x1 = num1 + 30 * index;
                    size = bxFlip.Size;
                    var y1 = (int)Math.Round((size.Height / 2.0 - 30) / 2.0);
                    local2 = new Rectangle(x1, y1, 30, 30);
                    var rectangle2 = new Rectangle();
                    ref var local3 = ref rectangle2;
                    var x2 = num1 + 30 * index;
                    size = bxFlip.Size;
                    var num3 = size.Height / 2.0;
                    size = bxFlip.Size;
                    var num4 = (size.Height / 2.0 - 30) / 2.0;
                    var y2 = (int)Math.Round(num3 + num4);
                    local3 = new Rectangle(x2, y2, 30, 30);
                    RectangleF bounds;
                    Rectangle destRect;
                    if (power.Slots[index].Enhancement.Enh > -1)
                    {
                        var graphics1 = bxFlip.Graphics;
                        AssetManager.DrawEnhancementAt(graphics1, iDest, DatabaseAPI.Database.Enhancements[power.Slots[index].Enhancement.Enh].ImageIdx, AssetManager.ToGfxGrade(DatabaseAPI.Database.Enhancements[power.Slots[index].Enhancement.Enh].TypeID, power.Slots[index].Enhancement.Grade));
                        if (power.Slots[index].Enhancement.Enh > -1)
                        {
                            if (!MidsContext.Config.I9.HideIOLevels & DatabaseAPI.Database.Enhancements[power.Slots[index].Enhancement.Enh].TypeID is Enums.eType.SetO or Enums.eType.InventO)
                            {
                                bounds = iDest;
                                bounds.Y -= 3f;
                                bounds.Height = DefaultFont.GetHeight(bxFlip.Graphics);
                                var graphics2 = bxFlip.Graphics;
                                BuildRenderer.DrawOutlineText($"{power.Slots[index].Enhancement.IOLevel + 1}", bounds, Color.Cyan, Color.FromArgb(128, 0, 0, 0), pnlEnhActive.Font, 1f, graphics2);
                            }
                            else if (MidsContext.Config.ShowEnhRel & DatabaseAPI.Database.Enhancements[power.Slots[index].Enhancement.Enh].TypeID is Enums.eType.Normal or Enums.eType.SpecialO)
                            {
                                bounds = iDest;
                                bounds.Y -= 3f;
                                bounds.Height = DefaultFont.GetHeight(bxFlip.Graphics);
                                Color text;
                                if (power.Slots[index].Enhancement.RelativeLevel != Enums.eEnhRelative.None)
                                {
                                    if (power.Slots[index].Enhancement.RelativeLevel >= Enums.eEnhRelative.Even)
                                    {
                                        if (power.Slots[index].Enhancement.RelativeLevel <= Enums.eEnhRelative.Even)
                                        {
                                            text = Color.White;
                                        }
                                        else
                                        {
                                            text = Color.FromArgb(0, byte.MaxValue, byte.MaxValue);
                                        }
                                    }
                                    else
                                    {
                                        text = Color.Yellow;
                                    }
                                }
                                else
                                {
                                    text = Color.Red;
                                }

                                var graphics2 = bxFlip.Graphics;
                                BuildRenderer.DrawOutlineText(Enums.GetRelativeString(power.Slots[index].Enhancement.RelativeLevel, MidsContext.Config.ShowRelSymbols), bounds, text, Color.FromArgb(128, 0, 0, 0), pnlEnhActive.Font, 1f, graphics2);
                            }
                        }
                    }
                    else
                    {
                        destRect = iDest with { Width = 30, Height = 30 };
                        bxFlip.Graphics.DrawImage(AssetManager.EmptySlot.Bitmap, destRect);
                    }

                    if (power.Slots[index].FlippedEnhancement.Enh > -1)
                    {
                        var graphics1 = bxFlip.Graphics;
                        AssetManager.DrawEnhancementAt(graphics1, rectangle2, DatabaseAPI.Database.Enhancements[power.Slots[index].FlippedEnhancement.Enh].ImageIdx, AssetManager.ToGfxGrade(DatabaseAPI.Database.Enhancements[power.Slots[index].FlippedEnhancement.Enh].TypeID, power.Slots[index].FlippedEnhancement.Grade));

                        if (power.Slots[index].FlippedEnhancement.Enh > -1)
                        {
                            if (!MidsContext.Config.I9.HideIOLevels & DatabaseAPI.Database.Enhancements[power.Slots[index].FlippedEnhancement.Enh].TypeID is Enums.eType.SetO or Enums.eType.InventO)
                            {
                                bounds = rectangle2;
                                bounds.Y -= 3f;
                                bounds.Height = DefaultFont.GetHeight(bxFlip.Graphics);
                                var graphics2 = bxFlip.Graphics;
                                BuildRenderer.DrawOutlineText($"{power.Slots[index].FlippedEnhancement.IOLevel + 1}", bounds, Color.Cyan, Color.FromArgb(128, 0, 0, 0), pnlEnhActive.Font, 1f, graphics2);
                            }
                            else if (MidsContext.Config.ShowEnhRel & DatabaseAPI.Database.Enhancements[power.Slots[index].FlippedEnhancement.Enh].TypeID is Enums.eType.Normal or Enums.eType.SpecialO)
                            {
                                bounds = rectangle2;
                                bounds.Y -= 3f;
                                bounds.Height = DefaultFont.GetHeight(bxFlip.Graphics);
                                Color text;
                                if (power.Slots[index].FlippedEnhancement.RelativeLevel != Enums.eEnhRelative.None)
                                {
                                    if (power.Slots[index].FlippedEnhancement.RelativeLevel >= Enums.eEnhRelative.Even)
                                    {
                                        if (power.Slots[index].FlippedEnhancement.RelativeLevel <= Enums.eEnhRelative.Even)
                                        {
                                            text = Color.White;
                                        }
                                        else
                                        {
                                            text = Color.FromArgb(0, byte.MaxValue, byte.MaxValue);
                                        }
                                    }
                                    else
                                    {
                                        text = Color.Yellow;
                                    }
                                }
                                else
                                {
                                    text = Color.Red;
                                }

                                var graphics2 = bxFlip.Graphics;
                                BuildRenderer.DrawOutlineText(Enums.GetRelativeString(power.Slots[index].FlippedEnhancement.RelativeLevel, MidsContext.Config.ShowRelSymbols), bounds, text, Color.FromArgb(128, 0, 0, 0), pnlEnhActive.Font, 1f, graphics2);
                            }
                        }
                    }
                    else
                    {
                        destRect = rectangle2 with { Width = 30, Height = 30 };
                        bxFlip.Graphics.DrawImage(AssetManager.EmptySlot.Bitmap, destRect);
                    }

                    rectangle2.Inflate(2, 2);
                    bxFlip.Graphics.FillEllipse(solidBrush2, rectangle2);
                }

                RedrawFlip();
            }
        }

        private string GetToWhoShort(IEffect fx)
        {
            return fx.ToWho switch
            {
                Enums.eToWho.Target => " (Tgt)",
                Enums.eToWho.Self => " (Self)",
                _ => ""
            };
        }

        private List<IEffect[]> SwapExtraEffects(IEffect[] baseEffects, IEffect[] enhEffects)
        {
            var enhFxList = enhEffects.ToList();
            for (var i = enhEffects.Length; i < baseEffects.Length; i++)
            {
                enhFxList.Add((IEffect)baseEffects[i].Clone());
            }

            var baseFxList = new List<IEffect>();
            for (var i = 0; i < enhEffects.Length; i++)
            {
                baseFxList.Add((IEffect)baseEffects[i].Clone());
            }

            baseEffects = baseFxList.ToArray();
            enhEffects = enhFxList.ToArray();

            return [baseEffects, enhEffects];
        }

        private List<Enums.ShortFX[]> SwapExtraEffects(Enums.ShortFX[] baseEffects, Enums.ShortFX[] enhEffects)
        {
            var enhFxList = enhEffects.ToList();
            for (var i = enhEffects.Length; i < baseEffects.Length; i++)
            {
                enhFxList.Add((Enums.ShortFX)baseEffects[i].Clone());
            }

            var baseFxList = new List<Enums.ShortFX>();
            for (var i = 0; i < enhEffects.Length; i++)
            {
                baseFxList.Add((Enums.ShortFX)baseEffects[i].Clone());
            }

            baseEffects = baseFxList.ToArray();
            enhEffects = enhFxList.ToArray();

            return [baseEffects, enhEffects];
        }

        private static string ConvertNewlinesToRtf(string str)
        {
            return str
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", RTF.Crlf());
        }

        private static string GetEnhancementStringLongRtf(I9Slot iEnh)
        {
            var str = iEnh.GetEnhancementStringLong();
            if (!string.IsNullOrEmpty(str))
            {
                str = RTF.Color(RTF.ElementID.Enhancement) + RTF.Italic(ConvertNewlinesToRtf(str)) + RTF.Color(RTF.ElementID.Text);
            }

            return str;
        }

        private static string GetEnhancementStringRtf(I9Slot iEnh)
        {
            var str = iEnh.GetEnhancementString();
            if (!string.IsNullOrEmpty(str))
            {
                str = RTF.Color(RTF.ElementID.Enhancement) + ConvertNewlinesToRtf(str) + RTF.Color(RTF.ElementID.Text);
            }

            return str;
        }

        private PairedListEx.Item GetRankedEffect(int[] index, int id)
        {
            var title = string.Empty;
            var shortFxBase = new Enums.ShortFX();
            var shortFxEnh = new Enums.ShortFX();
            var tag2 = new Enums.ShortFX();
            var suffix = string.Empty;
            var enhancedPower = pEnh ?? pBase;
            var fx = pEnh != null && index[id] < pEnh.Effects.Length
                ? pEnh.Effects[index[id]]
                : index[id] < pBase.Effects.Length
                    ? pBase.Effects[index[id]]
                        : null;

            var fx2 = id <= 0
                ? null
                : pEnh != null && index[id - 1] < pEnh.Effects.Length
                    ? pEnh.Effects[index[id - 1]]
                    : index[id - 1] < pBase.Effects.Length
                        ? pBase.Effects[index[id - 1]]
                        : null;

            if (fx == null)
            {
                return FastItemBuilder.Fi.FastItem("", 0f, 0f, string.Empty);
            }

            if (index[id] > -1)
            {
                var flag = false;
                var onlySelf = fx.ToWho == Enums.eToWho.Self;
                var onlyTarget = fx.ToWho == Enums.eToWho.Target;
                if (id > 0)
                {
                    flag = (fx.EffectType == fx2.EffectType) &
                           (fx.ToWho == Enums.eToWho.Self) &
                           (fx2.ToWho == Enums.eToWho.Self) &
                           (fx.ToWho == Enums.eToWho.Target);
                }

                if (fx.DelayedTime > 5)
                {
                    flag = true;
                }

                var names = Enum.GetNames(typeof(Enums.eEffectTypeShort));
                if (fx.EffectType == Enums.eEffectType.Enhancement)
                {
                    title = fx.ETModifies switch
                    {
                        Enums.eEffectType.EnduranceDiscount => "+EndRdx",
                        Enums.eEffectType.RechargeTime => "+Rechg",
                        Enums.eEffectType.Mez => fx.MezType == Enums.eMez.None
                            ? "+Effects"
                            : $"Enh({Enum.GetName(Enums.eMezShort.None.GetType(), fx.MezType)})",
                        Enums.eEffectType.Defense => "Enh(Def)",
                        Enums.eEffectType.Resistance => "Enh(Res)",
                        _ => FastItemBuilder.Str.CapString(Enum.GetName(fx.ETModifies.GetType(), fx.ETModifies), 7)
                    };

                    shortFxBase.Assign(pBase.GetEffectMagSum(fx.EffectType,
                        fx.ETModifies, fx.DamageType,
                        fx.MezType, false, onlySelf, onlyTarget));

                    shortFxEnh.Assign(enhancedPower.GetEffectMagSum(enhancedPower.Effects[index[id]].EffectType,
                        enhancedPower.Effects[index[id]].ETModifies, enhancedPower.Effects[index[id]].DamageType,
                        enhancedPower.Effects[index[id]].MezType, false, onlySelf, onlyTarget));
                }
                else
                {
                    title = fx.EffectType != Enums.eEffectType.Mez
                        ? names[(int)fx.EffectType]
                        : Enums.GetMezName((Enums.eMezShort)fx.MezType);
                }

                var temp = string.Empty;
                switch (fx.EffectType)
                {
                    case Enums.eEffectType.HitPoints:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.HitPoints, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.HitPoints, false, onlySelf, onlyTarget));
                        tag2.Assign(shortFxBase);
                        shortFxBase.Sum = (float)(shortFxBase.Sum / (double)MidsContext.Archetype.Hitpoints * 100);
                        shortFxEnh.Sum = (float)(shortFxEnh.Sum / (double)MidsContext.Archetype.Hitpoints * 100);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Heal:
                        if (fx.BuffedMag <= 1)
                        {
                            temp = $"{fx.BuffedMag:P2}";
                            shortFxBase.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Heal, false, onlySelf, onlyTarget));
                            shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Heal, false, onlySelf, onlyTarget));
                            shortFxBase.Sum = (float)(shortFxBase.Sum / (double)MidsContext.Archetype.Hitpoints * 100);
                            shortFxEnh.Sum = (float)(shortFxEnh.Sum / (double)MidsContext.Archetype.Hitpoints * 100);
                            tag2.Assign(shortFxBase);
                        }
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Absorb:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Absorb, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Absorb, false, onlySelf, onlyTarget));
                        var absorbPercent = pBase.Effects
                            .Where(e => e.EffectType == Enums.eEffectType.Absorb)
                            .Any(e => e.DisplayPercentage);
                        tag2.Assign(shortFxBase);
                        suffix = absorbPercent ? "%" : "";
                        break;
                    case Enums.eEffectType.Endurance:
                        if (fx.BuffedMag < -0.01 && fx.BuffedMag > -1)
                        {
                            temp = $"{fx.BuffedMag:P2}";
                            shortFxBase.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Endurance, false, onlySelf, onlyTarget));
                            shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Endurance, false, onlySelf, onlyTarget));
                            tag2.Assign(shortFxBase);
                        }
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Regeneration:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Regeneration, false, onlySelf, onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Regeneration, false, onlySelf, onlyTarget));
                        shortFxEnh.Sum *= 100;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Null:
                        if (fx.BuffedMag < 1)
                        {
                            temp = $"{fx.BuffedMag:P2}";
                            shortFxBase.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Null, false, onlySelf, onlyTarget));
                            shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Null, false, onlySelf, onlyTarget));
                            tag2.Assign(shortFxBase);
                        }
                        suffix = "%";
                        break;
                    case Enums.eEffectType.ToHit:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.ToHit, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.ToHit, false, onlySelf, onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Fly:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Fly, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Fly, false, onlySelf, onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Recovery:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Recovery, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Recovery, false, onlySelf, onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Mez when fx.MezType is Enums.eMez.Taunt or Enums.eMez.Placate:
                        shortFxBase.Add(index[id], fx.Duration);
                        shortFxEnh.Add(index[id], enhancedPower.Effects[index[id]].Duration);
                        tag2.Assign(shortFxBase);
                        suffix = "s";
                        break;

                    // Set list of effects below that are treated as percentages
                    // Base and enhanced values will be multiplied by 100
                    case Enums.eEffectType.DamageBuff:
                    case Enums.eEffectType.Defense:
                    case Enums.eEffectType.Resistance:
                    case Enums.eEffectType.ResEffect:
                    case Enums.eEffectType.Enhancement:
                    case Enums.eEffectType.MezResist:
                    case Enums.eEffectType.RechargeTime:
                    case Enums.eEffectType.SpeedFlying:
                    case Enums.eEffectType.SpeedRunning:
                    case Enums.eEffectType.SpeedJumping:
                    case Enums.eEffectType.JumpHeight:
                    case Enums.eEffectType.PerceptionRadius:
                    case Enums.eEffectType.Meter:
                    case Enums.eEffectType.Range:
                    case Enums.eEffectType.MaxFlySpeed:
                    case Enums.eEffectType.MaxRunSpeed:
                    case Enums.eEffectType.MaxJumpSpeed:
                    case Enums.eEffectType.Jumppack:
                    case Enums.eEffectType.GlobalChanceMod:
                        if (fx.EffectType != Enums.eEffectType.Enhancement)
                        {
                            shortFxBase.Add(index[id], fx.BuffedMag);
                            shortFxEnh.Add(index[id], enhancedPower.Effects[index[id]].BuffedMag);
                        }

                        shortFxBase.Multiply();
                        shortFxEnh.Multiply();

                        tag2.Assign(enhancedPower.GetEffectMagSum(fx.EffectType, false, onlySelf, onlyTarget));
                        break;
                    case Enums.eEffectType.SilentKill:
                        shortFxBase.Add(index[id], fx.Absorbed_Duration);
                        shortFxEnh.Add(index[id], enhancedPower.Effects[index[id]].Absorbed_Duration);
                        tag2.Assign(shortFxBase);
                        break;
                    default:
                        shortFxBase.Add(index[id], fx.BuffedMag);
                        shortFxEnh.Add(index[id], enhancedPower.Effects[index[id]].BuffedMag);
                        tag2.Assign(shortFxBase);
                        break;
                }

                if (fx.DisplayPercentage)
                {
                    suffix = "%";
                }

                suffix += fx.ToWho switch
                {
                    Enums.eToWho.Target => " (Tgt)",
                    Enums.eToWho.Self => " (Self)",
                    _ => ""
                };

                if (flag)
                {
                    return FastItemBuilder.Fi.FastItem("", 0f, 0f, string.Empty);
                }
            }

            for (var fxIndex = 0; fxIndex < shortFxEnh.Index.Length; fxIndex++)
            {
                var sFxIdx = shortFxEnh.Index[fxIndex];
                if (sFxIdx >= pBase.Effects.Length & sFxIdx >= pEnh.Effects.Length)
                {
                    continue;
                }

                var effect = sFxIdx < pBase.Effects.Length
                    ? pBase.Effects[sFxIdx]
                    : pEnh.Effects[sFxIdx];

                if (sFxIdx <= -1 || !effect.DisplayPercentage)
                {
                    continue;
                }

                if (shortFxEnh.Value[fxIndex] > 1)
                {
                    continue;
                }

                switch (effect.EffectType)
                {
                    case Enums.eEffectType.Absorb:
                        //Fixes the Absorb display to correctly show the percentage
                        shortFxEnh.Sum = float.Parse(shortFxEnh.Sum.ToString("P", CultureInfo.InvariantCulture).Replace("%", ""));
                        break;
                    case Enums.eEffectType.ToHit:
                        //Fixes the ToHit display to correctly show the percentage
                        if (effect.Stacking == Enums.eStacking.Yes)
                        {
                            var overage = fx.Ticks * 0.05f;
                            shortFxEnh.Sum -= overage;
                            shortFxEnh.Sum /= 2;
                        }

                        break;
                    default:
                        shortFxEnh.ReSum();
                        break;
                }

                break;
            }

            // shortFxEnh.Index.Length == 0 will occur if all effects of the same kind
            // have non validated conditionals.
            // E.g. -Recovery on Kick if Cross Punch has not been picked.
            var tip = shortFxEnh.Index.Length <= 0
                ? ""
                : pEnh.BuildTooltipStringAllVectorsEffects(pEnh.Effects[shortFxEnh.Index[0]].EffectType,
                pEnh.Effects[shortFxEnh.Index[0]].ETModifies, pEnh.Effects[shortFxEnh.Index[0]].DamageType,
                pEnh.Effects[shortFxEnh.Index[0]].MezType);

            if (fx.ActiveConditionals.Count > 0)
            {
                return FastItemBuilder.Fi.FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, fx.Probability < 1, fx.ActiveConditionals.Count > 0, tip);
            }

            if (fx.SpecialCase != Enums.eSpecialCase.None)
            {
                return FastItemBuilder.Fi.FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, fx.Probability < 1, fx.SpecialCase != Enums.eSpecialCase.None, tip);
            }

            return FastItemBuilder.Fi.FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, fx.Probability < 1, false, tip);
        }

        private static bool IsMezEffect(string iStr)
        {
            var names = Enum.GetNames(Enums.eMez.None.GetType());
            var num = names.Length - 1;
            for (var index = 0; index <= num; ++index)
                if (string.Equals(iStr, names[index], StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

            return false;
        }

        private void RedrawFlip()
        {
            if (bxFlip == null)
            {
                DisplayFlippedEnhancements();
            }

            var srcRect = new Rectangle(0, 0, pnlEnhActive.Width, pnlEnhActive.Height);
            var destRect = new Rectangle(0, 0, pnlEnhActive.Width, pnlEnhActive.Height);
            pnlEnhActive.CreateGraphics().DrawImage(bxFlip.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
            srcRect = new Rectangle(0, pnlEnhActive.Height, pnlEnhInactive.Width, pnlEnhInactive.Height);
            pnlEnhInactive.CreateGraphics().DrawImage(bxFlip.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
        }

        private void SetDamageTip()
        {
            var iTip = pEnh == null ? "" : pEnh.GetDamageTip();
            infoDamageDisplay.SetTip(iTip);
        }

        private Power? GetPowerRedirectParent(IPower pSrc)
        {
            var pSrcRedirectParent = new Power
            {
                FullName = ""
            };
            foreach (var power in DatabaseAPI.Database.Power)
            {
                if (power is null) continue;
                var powerset = power.GetPowerSet();
                if (powerset is null) continue;
                if (powerset.SetType != Enums.ePowerSetType.Primary & powerset.SetType != Enums.ePowerSetType.Secondary & powerset.SetType != Enums.ePowerSetType.Pool & powerset.SetType != Enums.ePowerSetType.Ancillary)
                {
                    continue;
                }

                foreach (var fx in power.Effects)
                {
                    if (fx.EffectType != Enums.eEffectType.PowerRedirect)
                    {
                        continue;
                    }

                    if (fx.Override != pSrc.FullName)
                    {
                        continue;
                    }

                    pSrcRedirectParent = new Power(DatabaseAPI.GetPowerByFullName(power.FullName));
                }
            }

            return pSrcRedirectParent.FullName == "" ? null : pSrcRedirectParent;
        }

        private void SetPowerScaler()
        {
            /*if (pBase == null)
            {
                powerScaler.Visible = false;
            }
            else if (pBase.VariableEnabled & HistoryIDX > -1)
            {
                var str = string.IsNullOrEmpty(pBase.VariableName) ? "Targets" : pBase.VariableName;
                powerScaler.Visible = true;
                powerScaler.BeginUpdate();
                powerScaler.ForcedMax = pBase.VariableMax;
                powerScaler.Clear();
                powerScaler.AddItem(
                    $"{str}:|{MidsContext.Character.CurrentBuild.Powers[HistoryIDX].VariableValue}",
                    MidsContext.Character.CurrentBuild.Powers[HistoryIDX].VariableValue, 0,
                    $"Use this slider to vary the power's effect.\r\nMin: {pBase.VariableMin}\r\nMax: {pBase.VariableMax}");
                powerScaler.EndUpdate();
            }
            else
            {
                powerScaler.Visible = false;
            }*/
        }

        private bool SFxCheck(Enums.ShortFX isFx)
        {
            return isFx.Index != null && isFx.Index.Any(t => pBase?.Effects.Length > t & t > -1 && pBase?.Effects[t].isEnhancementEffect == true);
        }

        private string ShortStr(string full, string brief)
        {
            return powerStatsGrid.Font.Size <= 100f / full.Length ? full : brief;
        }

        private int MiniGetEnhIndex(int iX, int iY)
        {
            if (bxFlip == null) return -1;
            var num1 = bxFlip.Size.Width - 188;
            if (pBase == null)
            {
                return -1;
            }

            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(pBase.PowerIndex);
            if (inToonHistory < 0)
            {
                return -1;
            }

            for (var index = 0; index < MidsContext.Character.CurrentBuild.Powers[inToonHistory].SlotCount; index++)
            {
                var rectangle = new Rectangle(num1 + 30 * index, (int)Math.Round((bxFlip.Size.Height / 2f - 30) / 2f), 30, 30);
                if ((iX > rectangle.X) & (iX < rectangle.X + rectangle.Width) &&
                    (iY > rectangle.Y) & (iY < rectangle.Y + rectangle.Height))
                {
                    return index;
                }
            }

            return -1;
        }

        private bool SplitFX_AddToList(ref Enums.ShortFX baseSfx, ref Enums.ShortFX enhSfx, ref PairedListEx iList, string specialTitle = "")
        {
            if (!baseSfx.Present)
            {
                return false;
            }

            var shortFxArray1 = Power.SplitFX(ref baseSfx, ref pBase);
            var shortFxArray2 = Power.SplitFX(ref enhSfx, ref pEnh);
            if (shortFxArray2.Length < shortFxArray1.Length)
            {
                var swappedFx = SwapExtraEffects(shortFxArray1, shortFxArray2);
                shortFxArray1 = (Enums.ShortFX[])swappedFx[0].Clone();
                shortFxArray2 = (Enums.ShortFX[])swappedFx[1].Clone();
            }

            for (var index = 0; index < shortFxArray1.Length; index++)
            {
                if (!shortFxArray1[index].Present)
                {
                    continue;
                }

                var Suffix = string.Empty;
                var num2 = shortFxArray1[index].Value[0];
                var num3 = index < shortFxArray2.Length
                    ? shortFxArray2[index].Value[0]
                    : shortFxArray2[index - 1].Value[0];
                if (pEnh.Effects[shortFxArray1[index].Index[0]].DisplayPercentage)
                {
                    Suffix = "%";
                    var effect = pEnh.Effects[shortFxArray1[index].Index[0]];
                    if ((effect.EffectType == Enums.eEffectType.Heal |
                         effect.EffectType == Enums.eEffectType.Endurance |
                         effect.EffectType == Enums.eEffectType.Damage) &
                        pEnh.Effects[shortFxArray1[index].Index[0]].Aspect == Enums.eAspect.Cur)
                    {
                        num2 *= 100;
                        num3 *= 100;
                    }
                }
                else
                {
                    switch (pEnh.Effects[shortFxArray1[index].Index[0]].EffectType)
                    {
                        case Enums.eEffectType.Heal:
                        case Enums.eEffectType.HitPoints:
                            Suffix = " HP";
                            break;
                    }
                }

                var title = Enums.GetEffectNameShort(pEnh.Effects[shortFxArray1[index].Index[0]].EffectType);
                if (specialTitle != string.Empty)
                {
                    title = specialTitle;
                }

                var s1 = num2;
                var s2 = num3;
                if ((pEnh.Effects[shortFxArray1[index].Index[0]].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                {
                    s1 = 0;
                    s2 = 0;
                }

                iList.AddItem(FastItemBuilder.Fi.FastItem(title, s1, s2, Suffix, false, false, pEnh.Effects[shortFxArray1[index].Index[0]].Probability < 1.0, pEnh.Effects[shortFxArray1[index].Index[0]].ActiveConditionals.Count > 0, Power.SplitFXGroupTip(ref shortFxArray1[index], ref pEnh, false)));
                if (pEnh.Effects[shortFxArray1[index].Index[0]].isEnhancementEffect)
                {
                    iList.SetUnique();
                }
            }

            return true;
        }

        #endregion

        #region Dock/Undock

        private void DockButton_Click(object? sender, EventArgs e)
        {
            if (_isDocked)
                Undock();
            else
                Redock();
        }

        private void Undock()
        {
            if (!_isDocked) return;

            // Remember original placement info for a reliable round-trip
            _originalParent = Parent;
            _originalDock = Dock;
            _originalBounds = Bounds;
            _originalIndex = _originalParent?.Controls.IndexOf(this) ?? -1;

            _floatingHostForm = new Form
            {
                Text = @"Data View",
                Size = new Size(Math.Max(400, Width), Math.Max(300, Height)),
                StartPosition = FormStartPosition.Manual,
                Location = PointToScreen(Point.Empty),
                FormBorderStyle = FormBorderStyle.SizableToolWindow,
                BackColor = BackColor
            };

            // Move into floating form
            _originalParent?.Controls.Remove(this);
            _floatingHostForm.Controls.Add(this);
            Dock = DockStyle.Fill;

            // Toggle icon to indicate current state
            DockButton.IconChar = IconChar.Anchor;
            DockButton.IconColor = Color.Silver;

            // When the toolwindow closes, re-dock automatically
            _floatingHostForm.FormClosing += FloatingHostForm_FormClosing;

            _floatingHostForm.Show();
            _isDocked = false;
        }

        private void FloatingHostForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // Return to original parent on close (unless app is exiting)
            Redock();
        }

        private void Redock()
        {
            if (_isDocked)
                return;

            // Detach from floating form
            if (_floatingHostForm != null)
            {
                _floatingHostForm.FormClosing -= FloatingHostForm_FormClosing;
                _floatingHostForm.Controls.Remove(this);
                _floatingHostForm.Close();
                _floatingHostForm.Dispose();
                _floatingHostForm = null;
            }

            // Restore to original parent/position if still available
            if (_originalParent != null && !_originalParent.IsDisposed)
            {
                if (_originalIndex >= 0 && _originalIndex <= _originalParent.Controls.Count)
                {
                    _originalParent.Controls.Add(this);
                    _originalParent.Controls.SetChildIndex(this, _originalIndex);
                }
                else
                {
                    _originalParent.Controls.Add(this);
                }

                Dock = _originalDock;
                if (_originalDock == DockStyle.None)
                    Bounds = _originalBounds;
            }

            // Toggle icon back to locked anchor
            DockButton.IconChar = IconChar.UpRightFromSquare;
            DockButton.IconColor = Color.Silver;

            _isDocked = true;
        }

        #endregion

        #region Lock/Unlock

        private void ApplyLockVisuals()
        {
            LockButton.IconChar = _isLocked ? IconChar.Lock : IconChar.Unlock;
            LockButton.IconColor = _isLocked ? Color.Red : Color.LimeGreen;
            LockButton.Invalidate();
        }

        private void SetLock(bool locked, bool raiseEvent)
        {
            if (_isLocked == locked) return;
            _isLocked = locked;
            ApplyLockVisuals();
            if (raiseEvent)
                LockStateChanged?.Invoke(this, _isLocked);
        }

        public void ToggleLock() => SetLock(!_isLocked, true);

        private void LockButton_Click(object? sender, EventArgs e)
        {
            ToggleLock();
        }

        #endregion

        #region Lifecycle

        private void MidsDataView_Resize(object? sender, EventArgs e)
        {
            // Simple: just repaint; no buffer to rebuild
            headerPanel.Invalidate();
        }

        private void Fx_ListItemClick(object? sender, PairedListEx.Item? item, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (item?.EntTag == null)
            {
                return;
            }

            var allPowers = item.EntTag.GetPowers();
            var currentBuildPowers = MidsContext.Character.CurrentBuild.Powers
                .Where(pe => pe?.Power != null)
                .Select(pe => pe?.Power)
                .ToHashSet();

            var filteredPowers = allPowers
                .Where(powerPair => powerPair.Value == null || currentBuildPowers.Contains(powerPair.Value))
                .Select(powerPair => powerPair.Key)
                .ToList();

            if (pBase != null)
            {
                PetInfo = new PetInfo(item.EntTag, HistoryIDX, pBase);
            }

            var hashsetPowers = filteredPowers.Select(x => x.FullName).ToHashSet();

            EntityDetails?.Invoke(item.EntTag.UID, hashsetPowers, HistoryIDX, PetInfo);
        }

        private void PairedList_Hover(object? sender, int index, Enums.ShortFX tag, string tooltip)
        {
            var empty1 = string.Empty;
            var str1 = string.Empty;
            if (tag.Present)
            {
                var empty2 = string.Empty;
                IPower power = new Power(pEnh);
                foreach (var t in tag.Index)
                {
                    if (t == -1 || power.Effects[t].EffectType == Enums.eEffectType.None)
                    {
                        continue;
                    }

                    var empty3 = string.Empty;
                    var returnMask = Array.Empty<int>();
                    power.GetEffectStringGrouped(t, ref empty3, ref returnMask, false, false);
                    if (returnMask.Length <= 0)
                    {
                        continue;
                    }

                    if (empty2 != string.Empty)
                    {
                        empty2 += "\r\n";
                    }

                    empty2 += empty3;
                    foreach (var m in returnMask)
                    {
                        power.Effects[m].EffectType = Enums.eEffectType.None;
                    }
                }

                foreach (var t in tag.Index)
                {
                    if (power.Effects[t].EffectType == Enums.eEffectType.None)
                    {
                        continue;
                    }

                    if (empty2 != string.Empty)
                    {
                        empty2 += "\r\n";
                    }

                    empty2 += power.Effects[t].BuildEffectString();
                }

                str1 = empty1 + empty2;
            }
            else if (string.IsNullOrWhiteSpace(tooltip))
            {
                str1 = string.Empty;
            }
            else
            {
                str1 = tooltip;
            }

            if (!string.IsNullOrWhiteSpace(str1))
            {
                //dvToolTip.SetToolTip((Control)sender, str1);
            }
            else
            {
                //dvToolTip.SetToolTip((Control)sender, string.Empty);
            }
        }

        private void PairedList_ItemOut(object sender)
        {
            //dvToolTip.SetToolTip((Control)sender, string.Empty);
        }

        private void pnlEnhActive_MouseClick(object sender, MouseEventArgs e)
        {
            var powerBase = rootPowerBase ?? pBase;

            if (powerBase == null || e.Button != MouseButtons.Left)
            {
                return;
            }

            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(powerBase.PowerIndex);
            if (inToonHistory <= -1)
            {
                return;
            }

            var slotFlip = SlotFlip;
            slotFlip?.Invoke(inToonHistory);
        }

        private void pnlEnhActive_MouseMove(object sender, MouseEventArgs e)
        {
            var powerBase = rootPowerBase ?? pBase;
            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(powerBase.PowerIndex);
            var enhIndex = MiniGetEnhIndex(e.X, e.Y);
            if (enhIndex <= -1)
            {
                return;
            }

            SetEnhancement(MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[enhIndex].Enhancement,
                MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[enhIndex].Level);
        }

        private void pnlEnhActive_Paint(object sender, PaintEventArgs e)
        {
            RedrawFlip();
        }

        private void pnlEnhInactive_MouseClick(object sender, MouseEventArgs e)
        {
            var powerBase = rootPowerBase ?? pBase;

            if (powerBase == null || e.Button != MouseButtons.Left)
            {
                return;
            }

            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(powerBase.PowerIndex);
            if (inToonHistory <= -1)
            {
                return;
            }

            var slotFlip = SlotFlip;
            slotFlip?.Invoke(inToonHistory);
        }

        private void pnlEnhInactive_MouseMove(object sender, MouseEventArgs e)
        {
            var powerBase = rootPowerBase ?? pBase;

            var inToonHistory = MidsContext.Character.CurrentBuild.FindInToonHistory(powerBase.PowerIndex);
            var enhIndex = MiniGetEnhIndex(e.X, e.Y);
            if (enhIndex <= -1)
            {
                return;
            }

            SetEnhancement(MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[enhIndex].FlippedEnhancement,
                MidsContext.Character.CurrentBuild.Powers[inToonHistory].Slots[enhIndex].Level);
        }

        private void pnlEnhInactive_Paint(object sender, PaintEventArgs e)
        {
            RedrawFlip();
        }

        private void PowerScaler_BarClick(float val)
        {
            var num = (int)Math.Round(val);
            if (num < pBase.VariableMin)
            {
                num = pBase.VariableMin;
            }

            if (num > pBase.VariableMax)
            {
                num = pBase.VariableMax;
            }

            MidsContext.Character.CurrentBuild.Powers[HistoryIDX].VariableValue = num;
            MidsContext.Character.CurrentBuild.Powers[HistoryIDX].Power.Stacks = num;
            /*foreach (var effect in MidsContext.Character.CurrentBuild.Powers[HistoryIDX].Power.Effects)
            {
                effect.UpdateAttrib();
                DisplayInfo();
            }*/
            if (num == pLastScaleVal)
            {
                return;
            }

            SetPowerScaler();
            pLastScaleVal = num;
            MainModule.MidsController.Toon?.GenerateBuffedPowerArray();
            SlotUpdate?.Invoke(pBase, num);
        }

        #endregion
    }
}
