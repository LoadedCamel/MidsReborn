using System.ComponentModel;
using FontAwesome.Sharp;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.BuildFile;
using Mids_Reborn.Core.ShareSystem.RestModels;
using Mids_Reborn.Core.Theming;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Core.Utils.Scopes;
using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Controls.Test;
using Mids_Reborn.UI.Forms.Controls;
using Mids_Reborn.UI.Forms.ImportExportItems;
using Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor;
using Mids_Reborn.UI.Forms.UpdateSystem;
using Mids_Reborn.UI.Forms.WindowMenuItems;
using Mids_Reborn.UI.Renderer;
using Mids_Reborn.UI.Theming;
using MRBResourceLib;
using RestSharp;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using Mids_Reborn.UI.Forms.OptionsMenuItems;

namespace Mids_Reborn.UI.Forms
{
    public partial class MainWindow2 : Form
    {

        #region Constants

        private const int LButtonDown = 0xA1;
        private const int Caption = 0x2;
        private const string UriScheme = "mrb";

        #endregion

        #region Fields

        private readonly EventSuppressionController _events = new();

        private bool _gfxDrawing;
        private long _popupLastOpenTime;
        private int _originalIndex = -1;

        private const int BaselineCanvasWidth = 610;
        private float _lastMasterScale = 1f;
        private int _lastCanvasWidth = -1;

        // Drag & drop / mouse tracking
        private readonly short[]? dragdropScenarioAction;
        private bool DoneDblClick;
        private int dragFinishPower;
        private int dragFinishSlot;
        private Rectangle dragRect;
        private int dragStartPower;
        private int dragStartSlot;
        private int dragStartX;
        private int dragStartY;
        private int dragXOffset;
        private int dragYOffset;

        // Drawing / DV caching
        private BuildRenderer? drawing;
        internal BuildRenderer? Drawing => drawing;
        private int dvLastEnh;
        private bool dvLastNoLev;
        private int dvLastPower;

        // Enhancement picker state
        private int EnhancingPower;
        private int EnhancingSlot;
        private readonly bool EnhPickerActive;
        private int PickerHID;

        // Top-level windows / forms
        private SetInspector? _setInspector;
        private I9Picker? _i9Picker;
        private EnhCheckMode? _enhCheckMode;
        private MidsPopupDisplay? _popupHost;
        private frmBusy? _frmBusy;
        private FrmTeam? _frmTeam;
        private frmAccolade? fAccolade;
        private frmData? fData;
        private frmCompare? fGraphCompare;
        private frmStats? fGraphStats;
        private FrmIncarnate? fIncarnate;
        private frmPrestige? fPrestige;
        private frmFloatingStats? FloatingDataForm;
        private frmMiniList? fMini;
        private frmRecipeViewer? fRecipe;
        private frmRotationHelper? fRotationHelper;
        private frmSetFind? fSetFinder;
        private frmSetViewer? fSets;
        private frmTemp? fTemp;
        private frmTotalsV2? fTotals2;
        private frmTotals? fTotals;
        private frmBuildSalvageHud? fSalvageHud;

        // Theme fields
        private ToolStripMenuItem? _themeMenu;
        private FileSystemWatcher? _themeWatcher;
        private string? _lastAppliedTheme;

        // Flip animation state
        private bool FlipActive;
        private PowerEntry? FlipGP;
        private readonly int FlipInterval;
        private int FlipPowerID;
        private int[]? FlipSlotState;
        private readonly int FlipStepDelay;
        private readonly int FlipSteps;

        // Misc state
        private bool HasSentBack;
        private bool HasSentForwards;
        private bool LastClickPlacedSlot;
        private int LastEnhIndex;
        private I9Slot? LastEnhPlaced;
        private string? LastFileName;
        private int LastIndex;
        private FormWindowState LastState;
        private bool NoResizeEvent;
        private bool NoUpdate;
        private Rectangle oldDragRect;
        private bool PopUpVisible;

        // Z-order hints for tool windows
        private bool top_fData;
        private bool top_fGraphCompare;
        private bool top_fGraphStats;
        private bool top_fRecipe;
        private bool top_fSetFinder;
        private bool top_fSets;
        private bool top_fTotals;

        // Cursor offsets
        private int xCursorOffset;
        private int yCursorOffset;

        private readonly BuildManager _buildManager;

        #endregion

        #region Properties

        public bool DbChangeRequested { get; set; }
        private string[]? CommandArgs { get; }
        private string? ProcessedCommand { get; set; }
        private bool ProcessedFromCommand { get; set; }
        private bool FileModified { get; set; }

        private FrmEntityDetails? FrmEntityDetails { get; set; }

        private I9Picker I9Picker
        {
            get
            {
                if (_i9Picker.Height <= 235)
                {
                    _i9Picker.Height = 315;
                }
                return _i9Picker;
            }
            set => _i9Picker = value;
        }

        private HeaderTheme CurrentTheme => DesignMode ? ThemeManager.DesignTime.Header : ThemeManager.CurrentTheme?.Header ?? ThemeManager.DesignTime.Header;

        #endregion

        #region Events

        public event EventHandler? TitleUpdated;

        #endregion

        #region Constructor

        public MainWindow2(string[]? args)
        {
            FormBorderStyle = FormBorderStyle.None;
            InitializeComponent();
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            UpdateStyles();

            Load += MainWindow2_Load;
            Shown += MainWindow2_Shown;
            Closing += MainWindow2_Closing;
            ResizeEnd += OnResizeEnd;

            EnhancingSlot = -1;
            EnhancingPower = -1;
            EnhPickerActive = false;
            PickerHID = -1;
            FileModified = false;
            LastIndex = -1;
            LastEnhIndex = -1;
            dvLastPower = -1;
            dvLastEnh = -1;
            dvLastNoLev = true;
            LastState = FormWindowState.Normal;
            FlipSteps = 5;
            FlipInterval = 10;
            FlipStepDelay = 3;
            FlipPowerID = -1;
            FlipSlotState = [];
            dragStartPower = -1;
            dragStartSlot = -1;
            dragdropScenarioAction = new short[20];
            DoneDblClick = false;
            DbChangeRequested = false;
            KeyPreview = true;

            _buildManager = BuildManager.Instance;

            Icon = Resources.MRB_Icon_Concept;

            InitializeThemeMenu();

            if (!DesignMode)
            {
                ThemeManager.ThemeChanged += OnThemeChanged;
            }

            ApplyTheme();
            InitializePopup();
            InitializePicker();
            tmrGfx.Tick += tmrGfx_Tick;
            dataView.SlotUpdate += DataView_SlotUpdate;
            dataView.SlotFlip += DataView_SlotFlip;
        }

        private void OnResizeEnd(object? sender, EventArgs e)
        {
            Debug.WriteLine(ClientSize.ToString());
        }

        #endregion

        #region Overrides

        private void ApplyWindowEffects()
        {
            WinApi.DisableSystemCaptionAndBorder(Handle);
            WinApi.SetWindowCornerPreference(Handle, WinApi.CornerPreference.Round);
            WinApi.SetWindowBackdropType(Handle, WinApi.BackdropTypes.MainWindow);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyWindowEffects();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Invalidate(true);            // full form background
            UpdateUiLayout();
        }

        #endregion

        #region Form Event Methods

        private void MainWindow2_Load(object? sender, EventArgs e)
        {
            if (MidsContext.Config is null) return;

            ThemeManager.SetTheme(MidsContext.Config.SelectedTheme);
            RestoreWindowPlacement(this, MidsContext.Config);

            MidsContext.Config.I9.DefaultIOLevel = 49;

            NewToon(skipDraw: true);
            PowerModified(true);

            tsViewIOLevels.Checked = !MidsContext.Config.I9.HideIOLevels;
            tsViewRelative.Checked = MidsContext.Config.ShowEnhRel;
            tsViewSOLevels.Checked = MidsContext.Config.ShowSoLevels;
            tsViewSlotLevels.Checked = MidsContext.Config.ShowSlotLevels;
            tsViewRelativeAsSigns.Checked = MidsContext.Config.ShowRelSymbols;
            TsViewSelected();
            tsIODefault.Text = $"Default ({MidsContext.Config.I9.DefaultIOLevel + 1})";

            GetBestDamageValues();

            DlgSave!.InitialDirectory = MidsContext.Config.BuildsPath;
            DlgOpen!.InitialDirectory = MidsContext.Config.BuildsPath;
            tsViewSlotLevels.Checked = MidsContext.Config.ShowSlotLevels;
            slotLevelsEx.ToggleState = MidsContext.Config.ShowSlotLevels switch
            {
                true => MidsVectorButton.States.ToggledOn,
                false => MidsVectorButton.States.ToggledOff
            };

            UpdateModeInfo();
            tsViewRelative.Checked = MidsContext.Config.ShowEnhRel;
            popupEx.ToggleState = MidsContext.Config.DisableShowPopup switch
            {
                true => MidsVectorButton.States.ToggledOff,
                false => MidsVectorButton.States.ToggledOn
            };

            recipeEx.ToggleState = MidsContext.Config.PopupRecipes switch
            {
                true => MidsVectorButton.States.ToggledOn,
                false => MidsVectorButton.States.ToggledOff
            };
            SetColumns(MidsContext.Config.Columns < 1 ? 3 : MidsContext.Config.Columns, MidsContext.Config.Columns == 3 ? MidsContext.Config.ColumnStackingMode : Enums.eColumnStacking.None);

        }

        private void MainWindow2_Shown(object? sender, EventArgs e)
        {
            NewDraw();

            var comLoad = false;
            var prevLastFileNameCfg = MidsContext.Config.LastFileName;
            var prevLoadLastCfg = MidsContext.Config.DisableLoadLastFileOnStart;
            var toonLoaded = false;
            if (CommandArgs is { Length: > 0 })
            {
                switch (CommandArgs[0])
                {
                    case "-load":
                        var nArgs = CommandArgs.Skip(1);
                        var file = string.Join(" ", nArgs);
                        MidsContext.Config.DisableLoadLastFileOnStart = false;
                        switch (file)
                        {
                            case var _ when DlgOpen.FileName.EndsWith(".mxd"):
                                LoadLegacyOrGameFile(file);
                                break;

                            case var _ when DlgOpen.FileName.EndsWith(".mbd"):
                                LoadCharacterFile(file);
                                break;
                        }

                        ProcessedFromCommand = true;

                        break;
                    case var fileLoad when (CommandArgs[0].Contains(".mxd") || CommandArgs[0].Contains(".mbd")) &&
                                           !CommandArgs[0].Contains("mrb://"):
                        ProcessedFromCommand = false;
                        MidsContext.Config.LastFileName = fileLoad;
                        MidsContext.Config.DisableLoadLastFileOnStart = false;
                        break;
                    default:
                        if (Uri.TryCreate(CommandArgs[0], UriKind.Absolute, out var uri) &&
                            string.Equals(uri.Scheme, UriScheme, StringComparison.OrdinalIgnoreCase))
                        {
                            MidsContext.Config.DisableLoadLastFileOnStart = false;
                            toonLoaded = RunSchemaCommands(CommandArgs[0]);
                            comLoad = false;

                            ProcessedFromCommand = true;
                        }
                        else
                        {
                            MidsContext.Config.DisableLoadLastFileOnStart = false;
                            comLoad = true;
                            ProcessedCommand = CommandArgs[0];
                            ProcessedFromCommand = false;
                        }

                        break;
                }
            }
        }

        private void MainWindow2_Closing(object? sender, CancelEventArgs e)
        {
            e.Cancel = ShouldCancelClose();
        }

        private void OnThemeChanged()
        {
            ApplyTheme();
            // Force the form to redraw its background and non-client areas
            Invalidate(true);
        }

        private void Title_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WinApi.ReleaseCapture();
                WinApi.SendMessage(Handle, LButtonDown, Caption, 0);
            }
        }

        private void BtnMaximize_Click(object? sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;
                btnMaximize.IconChar = IconChar.WindowMaximize;
                tTip.SetToolTip(btnMaximize, "Maximize");
            }
            else
            {
                MaximumSize = Screen.FromControl(this).WorkingArea.Size;
                WindowState = FormWindowState.Maximized;
                btnMaximize.IconChar = IconChar.WindowRestore;
                tTip.SetToolTip(btnMaximize, "Restore");
            }
        }

        private void BtnMinimize_Click(object? sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Initializers

        private void InitializeThemeMenu()
        {
            if (ViewToolStripMenuItem is null) return;

            _themeMenu = ViewToolStripMenuItem.DropDownItems
                .OfType<ToolStripMenuItem>()
                .FirstOrDefault(i => i == themeMenuItem);
            if (_themeMenu == null) return;

            PopulateThemeMenuItems();
        }

        private void InitializePopup()
        {
            _popupHost = new MidsPopupDisplay
            {
                BackColor = Color.Black,
                ColumnRight = false,
                Font = new Font("Segoe UI", 9.25f, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(96, 48, 255),
                Location = new Point(513, 490),
                Name = "Popup",
                Size = new Size(450, 203),
                TabIndex = 102,
                Visible = false
            };

            //_popup.MouseMove += Popup_MouseMove;

            Controls.Add(_popupHost);
            _popupHost.BringToFront();
            _popupHost.Invalidate();
        }

        private void InitializePicker()
        {
            _i9Picker = new I9Picker
            {
                BackColor = Color.Black,
                ForeColor = Color.Blue,
                Location = new Point(20, 20),
                Name = "i9Picker",
                TabIndex = 83,
                Visible = false
            };

            _i9Picker.EnhancementPicked += I9Picker_EnhancementPicked;
            _i9Picker.EnhancementSelectionCancelled += I9Picker_EnhancementSelectionCancelled;
            _i9Picker.HoverEnhancement += I9Picker_HoverEnhancement;
            _i9Picker.HoverSet += I9Picker_HoverSet;
            _i9Picker.MouseDown += I9Picker_MouseDown;
            _i9Picker.MouseEnter += I9Picker_MouseEnter;
            _i9Picker.MouseLeave += I9Picker_MouseLeave;
            _i9Picker.KeyDown += I9Picker_KeyDown;

            Controls.Add(_i9Picker);
            _i9Picker.BringToFront();
        }

        #endregion

        #region Control Event Methods

        private void AtDropDown_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_events.IsSuppressed) return;
            NewToon(false);
            GetBestDamageValues();
        }

        private void OriginDropDown_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_events.IsSuppressed) return;
            MidsContext.Character.Origin = originDropDown.SelectedIndex;
            AssetManager.SetOrigin(originDropDown.SelectedItem);
        }

        private void PrimaryDropDown_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_events.IsSuppressed) return;
            ChangeSets();
            UpdatePowerLists();
        }

        private void SecondaryDropDown_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_events.IsSuppressed) return;
            ChangeSets();
            UpdatePowerLists();
        }

        private void PoolsDropDown_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_events.IsSuppressed) return;
            ChangeSets();
            UpdatePowerLists();
        }

        private void AncillaryDropDown_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_events.IsSuppressed) return;
            ChangeSets();
            UpdatePowerLists();
        }

        private void MListView_ItemHovered(object? sender, MidsListViewItemHoverEventArgs e)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            if (sender is MidsListView listView)
            {
                if (e.Item is PowerListViewItem item)
                {
                    var bounds = MapRectToThis(listView, e.ItemBounds);
                    if (item.State == MidsItemState.Heading)
                    {
                        ShowPopup(item.NIdSet, -1, bounds, string.Empty);
                    }
                    else
                    {
                        var point = MapPointToThis(listView, e.Location);
                        Info_Power(item.NIdPower);
                        ShowPopup(-1, item.NIdPower, -1, point, bounds);
                    }
                }
                else
                {
                    HidePopup();
                }
            }
        }

        private void PrimaryList_ItemClicked(object? sender, MidsListViewItemClickEventArgs e)
        {
            if (sender is not MidsListView listView) return;
            if (e.Item is not PowerListViewItem item) return;
            if (item.State is MidsItemState.Heading) return;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    PowerPicked(item.NIdSet, item.NIdPower);
                    RefreshItemStates(listView);
                    break;
                case MouseButtons.Right:
                    Info_Power(item.NIdPower, -1, false, true);
                    break;
            }

        }

        private void SecondaryList_ItemClicked(object? sender, MidsListViewItemClickEventArgs e)
        {
            if (sender is not MidsListView listView) return;
            if (e.Item is not PowerListViewItem item) return;
            if (item.State is MidsItemState.Heading) return;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    PowerPicked(item.NIdSet, item.NIdPower);
                    RefreshItemStates(listView);
                    break;
                case MouseButtons.Right:
                    Info_Power(item.NIdPower, -1, false, true);
                    break;
            }
        }

        private void Pool0List_ItemClicked(object? sender, MidsListViewItemClickEventArgs e)
        {
            if (sender is not MidsListView listView) return;
            if (e.Item is not PowerListViewItem item) return;
            if (item.State is MidsItemState.Heading) return;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    PowerPicked(Enums.PowersetType.Pool0, item.NIdPower);
                    RefreshItemStates(listView);
                    break;
                case MouseButtons.Right:
                    Info_Power(item.NIdPower, -1, false, true);
                    break;
            }
        }

        private void Pool1List_ItemClicked(object? sender, MidsListViewItemClickEventArgs e)
        {
            if (sender is not MidsListView listView) return;
            if (e.Item is not PowerListViewItem item) return;
            if (item.State is MidsItemState.Heading) return;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    PowerPicked(Enums.PowersetType.Pool1, item.NIdPower);
                    RefreshItemStates(listView);
                    break;
                case MouseButtons.Right:
                    Info_Power(item.NIdPower, -1, false, true);
                    break;
            }
        }

        private void Pool2List_ItemClicked(object? sender, MidsListViewItemClickEventArgs e)
        {
            if (sender is not MidsListView listView) return;
            if (e.Item is not PowerListViewItem item) return;
            if (item.State is MidsItemState.Heading) return;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    PowerPicked(Enums.PowersetType.Pool2, item.NIdPower);
                    RefreshItemStates(listView);
                    break;
                case MouseButtons.Right:
                    Info_Power(item.NIdPower, -1, false, true);
                    break;
            }
        }

        private void Pool3List_ItemClicked(object? sender, MidsListViewItemClickEventArgs e)
        {
            if (sender is not MidsListView listView) return;
            if (e.Item is not PowerListViewItem item) return;
            if (item.State is MidsItemState.Heading) return;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    PowerPicked(Enums.PowersetType.Pool3, item.NIdPower);
                    RefreshItemStates(listView);
                    break;
                case MouseButtons.Right:
                    Info_Power(item.NIdPower, -1, false, true);
                    break;
            }
        }

        private void AncillaryList_ItemClicked(object? sender, MidsListViewItemClickEventArgs e)
        {
            if (sender is not MidsListView listView) return;
            if (e.Item is not PowerListViewItem item) return;
            if (item.State is MidsItemState.Heading) return;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    PowerPicked(item.NIdSet, item.NIdPower);
                    RefreshItemStates(listView);
                    frmTotalsV2.SetTitle(fTotals2);
                    break;
                case MouseButtons.Right:
                    Info_Power(item.NIdPower, -1, false, true);
                    break;
            }
        }

        private void I9Picker_EnhancementSelectionCancelled()
        {
            I9Picker.Visible = false;
            HidePopup();
            EnhancingSlot = -1;
            EnhancingPower = -1;
        }

        private void I9Picker_EnhancementPicked(I9Slot? e)
        {
            if (e == null)
            {
                I9Picker.Visible = false;
                HidePopup();
                EnhancingSlot = -1;
                EnhancingPower = -1;

                return;
            }

            e.RelativeLevel = I9Picker.View.RelLevel;
            if (EnhancingSlot <= -1)
            {
                return;
            }

            // Let popup visible when repeating enhancement
            if (!_gfxDrawing)
            {
                HidePopup();
            }

            var enhChanged = false;
            if (MidsContext.Character != null && MidsContext.Character.CurrentBuild.EnhancementTest(EnhancingSlot, EnhancingPower, e.Enh) | e.Enh < 0)
            {
                //Code below triggers after an enhancement is added
                var power = MidsContext.Character.CurrentBuild.Powers[EnhancingPower];
                if (power != null && e.Enh != power.Slots[EnhancingSlot].Enhancement.Enh)
                {
                    enhChanged = true;
                }

                var hasProc = power != null && power.HasProc();
                if (power != null)
                {
                    power.Slots[EnhancingSlot].Enhancement = (I9Slot)e.Clone();
                    if (e.Enh > -1)
                    {
                        LastEnhPlaced = (I9Slot)e.Clone();
                    }
                    else
                    {
                        MidsContext.Character.PEnhancementsList.Clear();
                    }

                    if (enhChanged)
                    {
                        // Do not turn off StatInclude for clicks that don't have a green tick button
                        if (power.Power is { PowerType: Enums.ePowerType.Click, ClickBuff: false })
                        {
                            power.StatInclude = true;
                        }
                        else
                        {
                            if (e.Enh > -1)
                            {
                                // if (!hasProc && power.HasProc && ...) ??
                                if (!hasProc && DatabaseAPI.Database.Enhancements[e.Enh].Probability == 0 ||
                                    DatabaseAPI.Database.Enhancements[e.Enh].Probability > 0)
                                {
                                    power.StatInclude = true;
                                }
                                else if (!power.CanIncludeForStats())
                                {
                                    power.StatInclude = false;
                                }
                            }
                            else if (!power.CanIncludeForStats())
                            {
                                power.StatInclude = false;
                            }
                        }

                        fRecipe?.RecalcSalvage();
                    }
                }

                I9Picker.Visible = false;
                if (!_gfxDrawing)
                {
                    PowerModified(true);
                }

                if (EnhancingPower > -1)
                {
                    RefreshTabs(MidsContext.Character.CurrentBuild.Powers[EnhancingPower].NIDPower, e);
                }

                // if (!_dvAnchored.PetInfo.HasEmptyBasePower)
                // {
                //     _dvAnchored.PetInfo.ExecuteUpdate();
                // }

                //MidsContext.Config.Tips.Show(Tips.TipType.FirstEnhancement);
            }
            else
            {
                I9Picker.Visible = false;
                EnhancingSlot = -1;
                EnhancingPower = -1;
            }
        }

        private void I9Picker_MouseEnter(object? sender, EventArgs e)
        {
            if (I9Picker.Visible)
            {
                // Give focus to the control so when hitting ESC
                // one doesn't have to click on the picker first.
                I9Picker.Focus();
            }
        }

        private void I9Picker_MouseLeave(object? sender, EventArgs e)
        {
            if (!MidsContext.Config.CloseEnhSelectPopupByMove)
            {
                return;
            }

            // 10 000 ticks in a millisecond / 10 000 000 ticks in a second (1.10^7)
            // Ensure the picker doesn't close instantly.
            if (!I9Picker.Visible | DateTime.Now.Ticks - _popupLastOpenTime < 1e6)
            {
                return;
            }

            I9Picker.Visible = false;
            HidePopup();
            EnhancingSlot = -1;
            RefreshInfo();
        }

        private void I9Picker_HoverEnhancement(int e, I9Picker.EnhUniqueStatus? enhUniqueStatus)
        {
            var i9Slot = new I9Slot
            {
                Enh = e,
                IOLevel = I9Picker.CheckAndReturnIoLevel() - 1,
                Grade = I9Picker.View.GradeId,
                RelativeLevel = I9Picker.View.RelLevel
            };

            ShowPopup(PickerHID, -1, -1, new Point(), I9Picker.Bounds, i9Slot, -1, VerticalAlignment.Top, enhUniqueStatus);
        }

        private void I9Picker_HoverSet(int e)
        {
            ShowPopup(PickerHID, -1, -1, new Point(), I9Picker.Bounds, null, e);
        }

        private void I9Picker_MouseDown(object? sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || EnhancingSlot <= -1)
            {
                return;
            }

            I9Picker.Visible = false;
            EnhancingSlot = -1;
            RefreshInfo();
        }

        private void I9Picker_KeyDown(object? sender, KeyEventArgs e)
        {
            if (MidsContext.Config.CloseEnhSelectPopupByMove || e.KeyCode != Keys.Escape)
            {
                return;
            }

            I9Picker.Visible = false;
            HidePopup();
            EnhancingSlot = -1;
        }

        private void Canvas_DragDrop(object sender, DragEventArgs e)
        {
            if (MidsContext.Config.ColumnStackingMode != Enums.eColumnStacking.None)
            {
                return;
            }

            if (!sender.Equals(canvas))
            {
                return;
            }

            canvas.AllowDrop = false;
            ControlPaint.DrawReversibleFrame(dragRect, Color.White, FrameStyle.Thick);
            oldDragRect = Rectangle.Empty;
            dragRect = Rectangle.Empty;
            var iValue1 = e.X + xCursorOffset;
            var iValue2 = e.Y + yCursorOffset;
            dragFinishPower = canvas.WhichSlot(iValue1, iValue2);
            if (dragStartSlot != -1)
            {
                dragFinishSlot = canvas.WhichEnh(iValue1, iValue2);
                if (dragFinishSlot == 0)
                {
                    MessageBox.Show(this, "You cannot change the level of any power's automatic slot.", null,
                        MessageBoxButtons.OK);
                }
                else
                {
                    SlotLevelSwap(dragStartPower, dragStartSlot, dragFinishPower, dragFinishSlot);
                }
            }
            else if ((e.KeyState & 4) > 0)
            {
                PowerMoveByUser(dragStartPower, dragFinishPower);
            }
            else
            {
                PowerSwapByUser(dragStartPower, dragFinishPower);
            }
        }

        private void Canvas_DragEnter(object sender, DragEventArgs e)
        {
            if (MidsContext.Config.ColumnStackingMode != Enums.eColumnStacking.None)
            {
                return;
            }

            e.Effect = sender.Equals(canvas) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void Canvas_DragOver(object sender, DragEventArgs e)
        {
            if (MidsContext.Config.ColumnStackingMode != Enums.eColumnStacking.None)
            {
                return;
            }

            Point position;
            int num1;
            if (sender.Equals(canvas))
            {
                if (!dragRect.IsEmpty)
                {
                    var top = dragRect.Top;
                    position = Cursor.Position;
                    var num2 = position.Y - dragYOffset;
                    var num3 = top != num2 ? 1 : 0;
                    var left = dragRect.Left;
                    position = Cursor.Position;
                    var num4 = position.X - dragXOffset;
                    var num5 = left != num4 ? 1 : 0;
                    num1 = (num3 | num5) == 0 ? 1 : 0;
                }
                else
                {
                    num1 = 0;
                }
            }
            else
            {
                num1 = 1;
            }

            if (num1 != 0)
            {
                return;
            }

            if (dragStartSlot != -1)
            {
                position = Cursor.Position;
                var x = position.X - dragXOffset;
                position = Cursor.Position;
                var y = position.Y - dragYOffset;
                var width = drawing.SzSlot.Width;
                var height = drawing.SzSlot.Height;
                dragRect = new Rectangle(x, y, width, height);
            }
            else
            {
                position = Cursor.Position;
                var x = position.X - dragXOffset;
                position = Cursor.Position;
                var y = position.Y - dragYOffset;
                var width = drawing.SzPower.Width;
                var height = drawing.SzPower.Height;
                dragRect = new Rectangle(x, y, width, height);
            }

            if (!oldDragRect.IsEmpty)
            {
                ControlPaint.DrawReversibleFrame(oldDragRect, Color.White, FrameStyle.Thick);
            }

            if (ClientRectangle.Contains(RectangleToClient(dragRect)))
            {
                oldDragRect = dragRect;
            }
            else
            {
                dragRect = oldDragRect;
            }

            ControlPaint.DrawReversibleFrame(dragRect, Color.White, FrameStyle.Thick);
        }

        private void Canvas_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Lock double click usage when enhancement check mode is active
            // Disable the ability to double click an enhancement slot to remove it
            if (MidsContext.EnhCheckMode)
            {
                return;
            }

            if (e.Button != MouseButtons.Left && e.Clicks != 2)
            {
                return;
            }

            if (!(!LastClickPlacedSlot && dragStartSlot >= 0))
            {
                return;
            }

            MainModule.MidsController.Toon.BuildSlot(dragStartPower, dragStartSlot);
            var powerEntryArray = DeepCopyPowerList();
            ShallowCopyPowerList(powerEntryArray);
            PowerModified(false);
            DoRedraw();
            PowerModified(true);
            FileModified = true;
            DoneDblClick = true;
            LastClickPlacedSlot = false;
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (MidsContext.EnhCheckMode)
            {
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            canvas.AllowDrop = true;
            dragStartX = e.X;
            dragStartY = e.Y;
            dragStartPower = canvas.WhichSlot(e.X, e.Y);
            dragStartSlot = canvas.WhichEnh(e.X, e.Y);
        }

        private void Canvas_MouseLeave(object sender, EventArgs e)
        {
            if (IsHoveringPopup()) return;
            HidePopup();
            drawing?.HighlightSlot(-1);
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsHoveringPopup())
                return;

            if (e.Button == MouseButtons.Left & canvas.AllowDrop && Math.Abs(e.X - dragStartX) + Math.Abs(e.Y - dragStartY) > 7)
            {
                if (dragStartSlot == 0)
                {
                    MessageBox.Show(this, "You cannot change the level of any power's automatic slot.", null, MessageBoxButtons.OK);
                    canvas.AllowDrop = false;
                }
                else
                {
                    xCursorOffset = e.X - Cursor.Position.X;
                    yCursorOffset = e.Y - Cursor.Position.Y;
                    if (dragStartSlot != -1)
                    {
                        if (drawing != null)
                        {
                            dragXOffset = drawing.SzSlot.Width / 2;
                            dragYOffset = drawing.SzSlot.Height / 2;
                        }
                    }
                    else
                    {
                        if (drawing != null)
                        {
                            dragXOffset = drawing.SzPower.Width / 2;
                            dragYOffset = drawing.SzPower.Height / 2;
                        }
                    }

                    var dataObject = new DataObject();
                    dataObject.SetText("This is some filler power text right here");
                    HidePopup();
                    canvas.Cursor = Cursors.Default;
                    drawing?.HighlightSlot(-1);
                    Application.DoEvents();
                    popupEx.DoDragDrop(dataObject, DragDropEffects.Move);
                }
            }
            else
            {
                if (drawing == null)
                    return;

                var index = canvas.WhichSlot(e.Location);
                var sIDX = canvas.WhichEnh(e.Location);

                if (index < 0 || index >= MidsContext.Character.CurrentBuild.Powers.Count)
                {
                    HidePopup();
                }
                else
                {
                    var powerRect = canvas.GetPowerButtonRect(index);
                    var enhRect = canvas.GetEnhancementSlotRect(index, sIDX);
                    var anchorRect = sIDX > -1 ? enhRect : powerRect;
                    ShowPopup(index, -1, sIDX, e.Location, anchorRect);
                    
                    if (MidsContext.Character.CanPlaceSlot & MainModule.MidsController.Toon.SlotCheck(MidsContext.Character.CurrentBuild.Powers[index]) > -1)
                    {
                        drawing.HighlightSlot(index);
                        canvas.Cursor = index > -1 & drawing.InterfaceMode != Enums.eInterfaceMode.PowerToggle
                            ? Cursors.Hand
                            : Cursors.Default;
                    }
                    else
                    {
                        canvas.Cursor = Cursors.Default;
                        drawing.HighlightSlot(-1);
                    }

                    if (index <= -1 || !(index != LastIndex | LastEnhIndex != sIDX))
                    {
                        return;
                    }

                    LastIndex = index;
                    LastEnhIndex = sIDX;
                    if (sIDX > -1)
                    {
                        RefreshTabs(MidsContext.Character.CurrentBuild.Powers[index].NIDPower,
                            MidsContext.Character.CurrentBuild.Powers[index].Slots[sIDX].Enhancement,
                            MidsContext.Character.CurrentBuild.Powers[index].Slots[sIDX].Level);
                    }
                    else
                    {
                        RefreshTabs(MidsContext.Character.CurrentBuild.Powers[index].NIDPower, new I9Slot());
                    }
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            canvas.AllowDrop = false;

            if (DoneDblClick)
            {
                DoneDblClick = false;
                return;
            }

            int hIDPower = drawing.WhichSlot(e.X, e.Y);
            if (hIDPower < 0 || hIDPower >= MidsContext.Character.CurrentBuild.Powers.Count)
            {
                return;
            }

            var powerEntry = MidsContext.Character.CurrentBuild.Powers[hIDPower];
            int slotID = drawing.WhichEnh(e.X, e.Y);
            bool isPowerChosen = powerEntry.NIDPower > -1;

            // --- Left Mouse Button Logic ---
            if (e.Button == MouseButtons.Left)
            {
                // Handle Enhancement Check Mode
                if (MidsContext.EnhCheckMode)
                {
                    if (slotID > -1)
                    {
                        powerEntry.Slots[slotID].Enhancement.Obtained = !powerEntry.Slots[slotID].Enhancement.Obtained;
                        fRecipe?.UpdateEnhObtained();
                        _enhCheckMode?.UpdateEnhObtained();
                        RedrawSinglePower(ref powerEntry, true);
                        canvas.Invalidate(canvas.GetPowerAreaRect(hIDPower));
                    }
                    return;
                }

                // Handle Power Toggle Mode
                if (drawing.InterfaceMode == Enums.eInterfaceMode.PowerToggle)
                {
                    if (isPowerChosen)
                    {
                        if (powerEntry.CanIncludeForStats())
                        {
                            powerEntry.StatInclude = !powerEntry.StatInclude;
                        }
                        else if (powerEntry.HasProc())
                        {
                            powerEntry.ProcInclude = !powerEntry.ProcInclude;
                        }
                        EnhancementModified();
                    }
                    return;
                }

                // Handle Toggle Clicks (Stat/Proc)
                var clickedToggle = drawing.WhichToggle(hIDPower, e.X, e.Y);
                if (clickedToggle != Enums.eToggleType.None)
                {
                    switch (clickedToggle)
                    {
                        case Enums.eToggleType.Stat:
                            if (powerEntry.StatInclude)
                            {
                                powerEntry.StatInclude = false;
                                powerEntry.Power.Active = false;
                            }
                            else
                            {
                                var eMutex = MainModule.MidsController.Toon.CurrentBuild.MutexV2(hIDPower);
                                if (eMutex == Enums.eMutex.NoConflict || eMutex == Enums.eMutex.NoGroup)
                                {
                                    powerEntry.StatInclude = true;
                                    powerEntry.Power.Active = true;
                                }
                            }
                            MidsContext.Character.Validate();
                            EnhancementModified();
                            canvas.Invalidate(canvas.GetPowerAreaRect(hIDPower));
                            break;

                        case Enums.eToggleType.Proc:
                            powerEntry.ProcInclude = !powerEntry.ProcInclude;
                            RedrawSinglePower(ref powerEntry, true, true);
                            canvas.Invalidate(canvas.GetPowerAreaRect(hIDPower));
                            break;
                    }
                    LastClickPlacedSlot = false;
                    return;
                }

                // Handle Modifier Key Shortcuts
                if (ModifierKeys == (Keys.Shift | Keys.Control))
                {
                    EditAccoladesOrTemps(hIDPower);
                    return;
                }
                if (ModifierKeys == Keys.Alt)
                {
                    MainModule.MidsController.Toon?.BuildPower(powerEntry.NIDPowerset, powerEntry.NIDPower);
                    PowerModified(true);
                    LastClickPlacedSlot = false;
                    canvas.RequestFullRedraw();
                    return;
                }
                if (ModifierKeys == Keys.Shift && slotID > -1)
                {
                    if (MidsContext.Config.BuildMode == Enums.dmModes.LevelUp)
                    {
                        MainModule.MidsController.Toon.RequestedLevel = powerEntry.Slots[slotID].Level;
                        MidsContext.Character.ResetLevel();
                    }
                    MainModule.MidsController.Toon?.BuildSlot(hIDPower, slotID);
                    PowerModified(true);
                    LastClickPlacedSlot = false;
                    canvas.RequestFullRedraw();
                    //_dvAnchored.PetInfo.ExecuteUpdate();
                    return;
                }

                // Standard Left Click (Add Slot or Select Power)
                if (EnhPickerActive) return;

                if (!isPowerChosen && powerEntry.Level > -1)
                {
                    MainModule.MidsController.Toon.RequestedLevel = powerEntry.Level;
                    UpdatePowerLists();
                    canvas.RequestFullRedraw();
                }
                else if (MainModule.MidsController.Toon.BuildSlot(hIDPower) > -1)
                {
                    PowerModified(false); // Adding a slot doesn't modify the build file until an enh is placed
                    LastClickPlacedSlot = true;
                    canvas.RequestFullRedraw();
                    //MidsContext.Config.Tips.Show(Tips.TipType.FirstSlot);
                }
                else
                {
                    LastClickPlacedSlot = false;
                }
                return;
            }

            // --- Right Mouse Button Logic ---
            if (e.Button == MouseButtons.Right)
            {
                if (ModifierKeys == Keys.Shift)
                {
                    StartFlip(hIDPower);
                }
                else if (slotID > -1)
                {
                    // Open Enhancement Picker
                    EnhancingSlot = slotID;
                    EnhancingPower = hIDPower;
                    PickerHID = hIDPower;
                    var enhancements = MainModule.MidsController.Toon?.GetEnhancements(hIDPower);
                    if (enhancements != null)
                    {
                        I9Picker.SetData(powerEntry.NIDPower, powerEntry.Slots[slotID].Enhancement, enhancements);
                        var point = new Point(
                            (int)Math.Round(canvasScrollPanel.Left - canvasScrollPanel.HorizontalScroll.Value + e.X - I9Picker.Width / 2f),
                            (int)Math.Round(canvasScrollPanel.Top - canvasScrollPanel.VerticalScroll.Value + e.Y - I9Picker.Height / 2f));

                        // Clamp picker to screen bounds
                        point.Y = Math.Max(MenuBar.Height, Math.Min(point.Y, ClientSize.Height - I9Picker.Height));
                        point.X = Math.Max(0, Math.Min(point.X, ClientSize.Width - I9Picker.Width));

                        I9Picker.Location = point;
                        I9Picker.BringToFront();
                        _popupLastOpenTime = DateTime.Now.Ticks;
                        I9Picker.Visible = true;
                        I9Picker.Select();
                    }
                }
                else if (isPowerChosen)
                {
                    // Lock Data View on the clicked power
                    Info_Power(powerEntry.NIDPower, -1, true, true);
                }
                LastClickPlacedSlot = false;
                return;
            }

            // --- Middle Mouse Button Logic ---
            if (e.Button == MouseButtons.Middle && slotID > -1 && !MidsContext.Config.DisableRepeatOnMiddleClick)
            {
                EnhancingSlot = slotID;
                EnhancingPower = hIDPower;
                _gfxDrawing = true;
                I9Picker_EnhancementPicked(GetRepeatEnhancement(hIDPower, slotID));
                _gfxDrawing = false;
                EnhancementModified();
                canvas.Invalidate();
            }
        }

        private void IncarnatesEx_OnClick(object? sender, EventArgs e)
        {
            var flag = false;
            if (fIncarnate == null)
            {
                flag = true;
            }
            else if (fIncarnate.IsDisposed)
            {
                flag = true;
            }

            if (flag)
            {
                var iParent = this;
                fIncarnate = new FrmIncarnate(ref iParent);
            }

            if (fIncarnate is { Visible: false })
            {
                incarnatesEx.ToggleState = MidsVectorButton.States.ToggledOn;
                fIncarnate.Show(this);
            }
            else
            {
                incarnatesEx.ToggleState = MidsVectorButton.States.ToggledOff;
                fIncarnate?.Close();
            }
        }

        private void PvXEx_OnClick(object? sender, EventArgs e)
        {
            MidsContext.Config.Inc.DisablePvE = pvXEx.ToggleState switch
            {
                MidsVectorButton.States.ToggledOff => false,
                MidsVectorButton.States.ToggledOn => true,
                _ => MidsContext.Config.Inc.DisablePvE
            };

            RefreshInfo();
        }

        private void TotalsEx_OnClick(object? sender, EventArgs e)
        {
            FloatTotals(true, MidsContext.Config is { UseOldTotalsWindow: true });
        }

        private void DynMode_Click(object? sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon == null)
            {
                return;
            }

            if (MidsContext.Config.BuildMode == Enums.dmModes.LevelUp && !dynMode.Lock)
            {
                dynMode.Lock = true;
            }

            MidsContext.Config.BuildOption = MidsContext.Config.BuildOption switch
            {
                Enums.dmItem.Power => Enums.dmItem.Slot,
                _ => Enums.dmItem.Power
            };

            UpdateDmBuffer();
        }

        private void TsFileOpen_Click(object? sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon?.Locked == true & FileModified)
            {
                FloatTop(false);
                var msgBoxResult = MessageBox.Show(@"Current hero/villain data will be discarded, are you sure?", @"Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                FloatTop(true);
                if (msgBoxResult == DialogResult.No)
                {
                    return;
                }
            }

            if (DlgOpen.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            FloatTop(false);
            MidsContext.EnhCheckMode = false;
            if (fRecipe is { Visible: true })
            {
                fRecipe.UpdateData();
            }

            if (fSalvageHud is { Visible: true })
            {
                FloatBuildSalvageHud(false);
            }

            switch (DlgOpen.FileName)
            {
                case var legacyBuild when DlgOpen.FileName.EndsWith(".mxd"):
                    LoadLegacyOrGameFile(legacyBuild);
                    break;

                case var newBuild when DlgOpen.FileName.EndsWith(".mbd"):
                    LoadCharacterFile(newBuild);
                    break;

                default:
                    if (DlgOpen.FileName.EndsWith(".txt"))
                    {
                        LoadLegacyOrGameFile(DlgOpen.FileName);
                    }

                    break;
            }
            FloatTop(true);
            var containsPower = MidsContext.Character?.CurrentBuild?.Powers
                .Where(pe => pe?.Power != null)
                .ToList()
                .Exists(x => Enum.IsDefined(typeof(Enums.eGridType), x?.Power?.InherentType ?? Enums.eGridType.None));

            if (containsPower != true || ActiveForm != this)
            {
                return;
            }
        }

        private void TsFileSave_Click(object sender, EventArgs e)
        {
            SaveBuild();
        }

        private void TsFileSaveAs_Click(object sender, EventArgs e)
        {
            SaveBuildAs();
        }

        private void tsFilePrint_Click(object sender, EventArgs e)
        {
            new frmPrint().ShowDialog(this);
        }

        private void tsFileQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tsGenFreebies_Click(object sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon == null)
            {
                return;
            }

            FloatTop(false);
            // Zed:
            // Rev. 2: use a folder picker instead of a file picker so the data folder
            // and subdirectories can be automatically created.
            var dirSelector = new FolderBrowserDialog
            {
                Description = @"Select your base CoH directory:",
                ShowNewFolderButton = false
            };
            var dsr = dirSelector.ShowDialog();
            if (dsr == DialogResult.Cancel)
            {
                return;
            }

            var iResult = InputBox.Show("Enter a name for the popmenu", "Name your menu", false, clsGenFreebies.DefaultMenuName, InputBox.InputBoxIcon.Info, inputBox_Validating);
            if (!iResult.OK)
            {
                return;
            }

            clsGenFreebies.MenuName = iResult.Text;
            Directory.CreateDirectory($@"{dirSelector.SelectedPath}\data\texts\English\Menus");
            var mnuFileName = $@"{dirSelector.SelectedPath}\data\texts\English\Menus\{clsGenFreebies.MenuName}.{clsGenFreebies.MenuExt}";
            var saveOp = clsGenFreebies.MenuExport.SaveTo(mnuFileName);
            if (!saveOp)
            {
                MessageBox.Show($"Couldn't save popmenu to file: {mnuFileName}", "Welp", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show($"Popmenu created.\r\nIf necessary, restart your client for it to become available for use.\r\nUse /popmenu {clsGenFreebies.MenuName} to open it.",
                    "Woop",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            dirSelector.Dispose();
            FloatTop(true);
        }

        private static void inputBox_Validating(object sender, InputBoxValidatingArgs e)
        {
            if (e.Text.Trim().Length != 0)
            {
                return;
            }

            e.Cancel = true;
            e.Message = "Required";
        }

        private void tsIODefault_Click(object sender, EventArgs e)
        {
            if (MidsContext.Character.CurrentBuild.SetIOLevels(MidsContext.Config.I9.DefaultIOLevel, false, false))
            {
                I9Picker.LastLevel = MidsContext.Config.I9.DefaultIOLevel + 1;
            }

            DoRedraw();
        }

        private void tsIOMin_Click(object sender, EventArgs e)
        {
            if (MidsContext.Character.CurrentBuild.SetIOLevels(MidsContext.Config.I9.DefaultIOLevel, true, false))
            {
                I9Picker.LastLevel = 10;
            }

            DoRedraw();
        }

        private void tsIOMax_Click(object sender, EventArgs e)
        {
            if (MidsContext.Character.CurrentBuild.SetIOLevels(MidsContext.Config.I9.DefaultIOLevel, false, true))
            {
                I9Picker.LastLevel = 50;
            }

            DoRedraw();
        }

        private void tsEnhToSO_Click(object sender, EventArgs e)
        {
            OnGradePick(Enums.eEnhGrade.SingleO);
        }

        private void tsEnhToDO_Click(object sender, EventArgs e)
        {
            OnGradePick(Enums.eEnhGrade.DualO);
        }

        private void tsEnhToTO_Click(object sender, EventArgs e)
        {
            OnGradePick(Enums.eEnhGrade.TrainingO);
        }

        private void tsEnhToEven_Click(object sender, EventArgs e)
        {
            OnRelativeClick(Enums.eEnhRelative.Even);
        }

        private void tsEnhToMinus1_Click(object sender, EventArgs e)
        {
            OnRelativeClick(Enums.eEnhRelative.MinusOne);
        }

        private void tsEnhToMinus2_Click(object sender, EventArgs e)
        {
            OnRelativeClick(Enums.eEnhRelative.MinusTwo);
        }

        private void tsEnhToMinus3_Click(object sender, EventArgs e)
        {
            OnRelativeClick(Enums.eEnhRelative.MinusThree);
        }

        private void tsEnhToNone_Click(object sender, EventArgs e)
        {
            OnRelativeClick(Enums.eEnhRelative.None);
        }

        private void tsEnhToPlus1_Click(object sender, EventArgs e)
        {
            OnRelativeClick(Enums.eEnhRelative.PlusOne);
        }

        private void tsEnhToPlus2_Click(object sender, EventArgs e)
        {
            OnRelativeClick(Enums.eEnhRelative.PlusTwo);
        }

        private void tsEnhToPlus3_Click(object sender, EventArgs e)
        {
            OnRelativeClick(Enums.eEnhRelative.PlusThree);
        }

        private void tsEnhToPlus4_Click(object sender, EventArgs e)
        {
            OnRelativeClick(Enums.eEnhRelative.PlusFour);
        }

        private void tsEnhToPlus5_Click(object sender, EventArgs e)
        {
            OnRelativeClick(Enums.eEnhRelative.PlusFive);
        }

        private void OnRelativeClick(Enums.eEnhRelative newVal)
        {
            if (MainModule.MidsController.Toon == null)
            {
                return;
            }

            if (MidsContext.Character?.CurrentBuild == null)
            {
                return;
            }

            if (MidsContext.Character.CurrentBuild.SetEnhRelativeLevels(newVal))
            {
                //I9Picker.Ui.Initial.RelLevel = newVal;
                I9Picker.View.RelLevel = newVal;
            }

            Info_Totals();
            DoRedraw();
        }

        private void tsFlipAllEnh_Click(object sender, EventArgs e)
        {
            MainModule.MidsController.Toon.FlipAllSlots();
            DoRedraw();
            RefreshInfo();
            FloatUpdate();
        }

        private void tsClearAllEnh_Click(object sender, EventArgs e)
        {
            FloatTop(false);
            if (MessageBox.Show(
                    "Really clear all slotted enhancements?\r\nThis will not clear the alternate slotting, only the currently active slots.",
                    "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                for (var index1 = 0; index1 <= MidsContext.Character.CurrentBuild.Powers.Count - 1; ++index1)
                for (var index2 = 0;
                     index2 <= MidsContext.Character.CurrentBuild.Powers[index1].Slots.Length - 1;
                     ++index2)
                    MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh = -1;

                DoRedraw();
                RefreshInfo();
            }

            FloatTop(true);
        }

        private void tsRemoveAllSlots_Click(object sender, EventArgs e)
        {
            FloatTop(false);
            if (MessageBox.Show(
                    "Really remove all slots?\r\nThis will not remove the slots granted automatically with powers, but will remove all the slots you placed manually.",
                    "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                for (var index = 0; index <= MidsContext.Character.CurrentBuild.Powers.Count - 1; ++index)
                    if (MidsContext.Character.CurrentBuild.Powers[index].SlotCount > 1)
                    {
                        MidsContext.Character.CurrentBuild.Powers[index].Slots =
                            MidsContext.Character.CurrentBuild.Powers[index].Slots.Take(1).ToArray();
                    }

                DoRedraw();
                MidsContext.Character.ResetLevel();
                // if all slots are removed, changes are they don't want to be prompted to save, unless something else is changed/added
                PowerModified(false);
                RefreshInfo();
            }

            FloatTop(true);
        }

        private void AutoArrangeAllSlotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var powerEntryArray = DeepCopyPowerList();
            RearrangeAllSlotsInBuild(powerEntryArray, true);
            ShallowCopyPowerList(powerEntryArray);
            // unless they set more than just slotting order, don't force the save flag
            PowerModified(false);
            DoRedraw();
        }

        private void tsView2Col_Click(object sender, EventArgs e)
        {
            tsView3Col.Checked = false;
            tsView4Col.Checked = false;
            tsView2Col.Checked = true;
            SetColumns(2);
        }

        private void tsView3Col_Click(object sender, EventArgs e)
        {
            tsView2Col.Checked = false;
            tsView4Col.Checked = false;
            tsView3Col.Checked = true;
            SetColumns(3);
        }

        private void tsView4Col_Click(object sender, EventArgs e)
        {
            tsView2Col.Checked = false;
            tsView3Col.Checked = false;
            tsView4Col.Checked = true;
            SetColumns(4);
        }

        private void tsViewIOLevels_Click(object sender, EventArgs e)
        {
            MidsContext.Config.I9.HideIOLevels = !MidsContext.Config.I9.HideIOLevels;
            tsViewIOLevels.Checked = !MidsContext.Config.I9.HideIOLevels;

            DoRedraw();
        }

        private void tsViewSOLevels_Click(object sender, EventArgs e)
        {
            MidsContext.Config.ShowSoLevels = !MidsContext.Config.ShowSoLevels;
            tsViewSOLevels.Checked = MidsContext.Config.ShowSoLevels;

            DoRedraw();
        }

        private void tsViewRelative_Click(object sender, EventArgs e)
        {
            MidsContext.Config.ShowEnhRel = !MidsContext.Config.ShowEnhRel;
            tsViewRelative.Checked = MidsContext.Config.ShowEnhRel;

            DoRedraw();
        }

        private void tsViewRelativeAsSigns_Click(object sender, EventArgs e)
        {
            MidsContext.Config.ShowRelSymbols = !MidsContext.Config.ShowRelSymbols;
            tsViewRelativeAsSigns.Checked = MidsContext.Config.ShowRelSymbols;

            DoRedraw();
        }

        private void tsViewSlotLevels_Click(object sender, EventArgs e)
        {
            MidsContext.Config.ShowSlotLevels = !MidsContext.Config.ShowSlotLevels;
            tsViewSlotLevels.Checked = MidsContext.Config.ShowSlotLevels;
            slotLevelsEx.ToggleState = MidsContext.Config.ShowSlotLevels switch
            {
                true => MidsVectorButton.States.ToggledOn,
                false => MidsVectorButton.States.ToggledOff
            };

            DoRedraw();
        }

        private void tsViewActualDamage_New_Click(object sender, EventArgs e)
        {
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.Numeric;
            SetDamageMenuCheckMarks();
            DisplayFormatChanged();
        }

        private void tsViewDPS_New_Click(object sender, EventArgs e)
        {
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.DPS;
            SetDamageMenuCheckMarks();
            DisplayFormatChanged();
        }

        private void tlsDPA_Click(object sender, EventArgs e)
        {
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.DPA;
            SetDamageMenuCheckMarks();
            DisplayFormatChanged();
        }

        private void tsViewSets_Click(object? sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon == null)
            {
                return;
            }

            FloatSets(true);
        }

        private void tsViewGraphs_Click(object? sender, EventArgs e)
        {
            FloatStatGraph(true);
        }

        private void tsViewData_Click(object? sender, EventArgs e)
        {
            FloatData(true);
        }

        private void tsViewSetCompare_Click(object? sender, EventArgs e)
        {
            FloatCompareGraph(true);
        }

        private void tsSetFind_Click(object? sender, EventArgs e)
        {
            if (_setInspector == null || _setInspector.IsDisposed)
            {
                _setInspector = new SetInspector(this);
                _setInspector.FormClosing += (_, _) => _setInspector.Dispose(); // Dispose when closing
            }

            _setInspector.Show();
        }

        private void tsRecipeViewer_Click(object? sender, EventArgs e)
        {
            FloatRecipe(true);
        }

        private void tsRotationHelper_Click(object? sender, EventArgs e)
        {
            FloatRotationHelper(true);
        }

        private void tsHelperLong_Click(object sender, EventArgs e)
        {
            new FrmInputLevel(this, true).ShowDialog(this);
        }

        private void tsHelperShort_Click(object? sender, EventArgs e)
        {
            new FrmInputLevel(this, false).ShowDialog(this);
        }

        private void tsAdvDBEdit_Click(object? sender, EventArgs e)
        {
            FloatTop(false);
            using var frmDbEdit = new frmDBEdit();
            frmDbEdit.ShowDialog(this);
            FloatTop(true);
        }

        private async void tsUpdateCheck_Click(object? sender, EventArgs e)
        {
            await UpdateCoordinator.CheckAndHandleUpdatesAsync(this, true);
        }

        private void Github_Link(object? sender, EventArgs e)
        {
            SupportSites.GoToGitHub();
        }

        private void tsSupport_Click(object? sender, EventArgs e)
        {
            SupportSites.DiscordServer();
        }

        private void tsWebsite_Click(object? sender, EventArgs e)
        {
            SupportSites.Website();
        }

        private void tsAbout_Click(object? sender, EventArgs e)
        {
            using var frmAbout = new frmAbout();
            frmAbout.ShowDialog();
        }

        private void tsViewSharedBuilds_Click(object? sender, EventArgs e)
        {
            using var vsb = new SharedBuilds();
            var result = vsb.ShowDialog(this);
            if (result != DialogResult.Continue || vsb.FetchedData == null)
            {
                return;
            }

            _buildManager.ValidateAndLoadSchemaData(vsb.FetchedData.Data, vsb.FetchedData.Id);
            FileModified = false;
            if (drawing != null)
            {
                drawing.Highlight = -1;
            }

            //dataView?.Clear();
            PowerModified(false);
        }

        private void tsImportChunk_Click(object? sender, EventArgs e)
        {
            var loaded = false;
            using var importBuild = new ImportCode();
            var result = importBuild.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                dataView.IsLocked = false;
                NewToon(true, true);
                loaded = _buildManager.ValidateAndLoadImportData(importBuild.ImportClassificationResult);
            }

            if (!loaded)
            {
                return;
            }

            FileModified = false;
            LastFileName = "";
            //SetTitleBar();
            SetLockedPoolsState();
            if (drawing != null)
            {
                drawing.Highlight = -1;
            }

            //dataView?.Clear();
            PowerModified(false);
        }

        private void tsImportLegacyForumPost_Click(object? sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon.Locked & FileModified)
            {
                FloatTop(false);
                var msgBoxResult = MessageBox.Show(@"Current character data will be discarded, are you sure?", @"Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                FloatTop(true);
                if (msgBoxResult == DialogResult.No)
                {
                    return;
                }
            }

            FloatTop(false);
            FileModified = false;
            var loaded = false;
            if (MessageBox.Show(@"Copy the build data on the forum to the clipboard. When that's done, click on OK.", @"Standing By", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK)
            {
                return;
            }

            var str = Clipboard.GetDataObject()?.GetData("System.String", true).ToString();
            NewToon();
            try
            {
                if (str is { Length: < 1 })
                {
                    MessageBox.Show(@"No data. Please check that you copied the build data from the forum correctly and that it's a valid format.", @"Forum Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (str == null || string.IsNullOrWhiteSpace(str))
                    {
                        loaded = false;
                    }
                    else if (str.Contains("MxDz") || str.Contains("MxDu"))
                    {
                        Debug.WriteLine("Loading because contains");
                        Stream? mStream = new MemoryStream(new ASCIIEncoding().GetBytes(str));
                        loaded = MainModule.MidsController.Toon.Load("", ref mStream);
                    }
                    else if (str.Contains("Character Profile:") || str.Contains("build.txt"))
                    {
                        GameImport(str);
                        loaded = true;
                    }
                    else if (str.Contains("Hero Profile:") || str.Contains("Villain Profile:"))
                    {
                        ForumImport(str);
                        loaded = true;
                    }

                    if (!loaded)
                    {
                        loaded = MainModule.MidsController.Toon.StringToInternalData(str);
                    }

                    if (loaded)
                    {
                        drawing.Highlight = -1;
                        NewDraw();
                        //dataView.Clear();
                        PowerModified(true);
                        //UpdateControls(true, true);
                    }
                    else
                    {
                        NewToon();
                        //dataView.Clear();
                        PowerModified(true);
                    }

                    GetBestDamageValues();
                    if (drawing != null)
                    {
                        DoRedraw();
                    }

                    //UpdateColors();
                    FloatTop(true);
                    //SetTitleBar(MidsContext.Character.IsHero(), true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Message: {ex.Message}\r\nTrace: {ex.StackTrace}");
                FloatTop(true);
            }
        }

        private void ShareMenu_Click(object? sender, EventArgs e)
        {
            if (MidsContext.Character.CurrentBuild.PowersPlaced <= 0)
            {
                var errorMsg = new MessageBoxEx("Share Protection Activated", "You cannot access the Share Menu at this time as there is not any build data to share.\r\nPlease either start creating a build or load one prior to accessing the menu.", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Protected, true);
                errorMsg.ShowDialog(this);
            }
            else
            {
                using var fe = new ShareMenu(_buildManager);
                fe.ShowDialog(this);
            }
        }

        private void TempPowersEx_OnClick(object? sender, EventArgs e)
        {
            if (fTemp == null || fTemp.IsDisposed)
            {
                // Inherent.Inherent.MxD_Temps
                var power = DatabaseAPI.Database.Power[DatabaseAPI.NidFromStaticIndexPower(3259)];
                var iPowers = new List<IPower?>();
                if (power != null)
                {
                    iPowers.AddRange(power.NIDSubPower.Select(t => DatabaseAPI.Database.Power[t]).OfType<IPower>().Where(p => p.ClickBuff || p.PowerType == Enums.ePowerType.Auto_ | p.PowerType == Enums.ePowerType.Toggle));
                }

                fTemp = new frmTemp(this, iPowers)
                {
                    Text = @"Temporary Powers"
                };
            }

            if (!fTemp.Visible)
            {
                fTemp.Show(this);
            }
        }

        private void ibPrestigePowersEx_OnClick(object? sender, EventArgs e)
        {
            var flag = false;
            if (fPrestige == null)
            {
                flag = true;
            }
            else if (fPrestige.IsDisposed)
            {
                flag = true;
            }

            if (flag)
            {
                var iParent = this;
                var iPowers = DatabaseAPI.Database.Power.Where(power => power is { InherentType: Enums.eGridType.Prestige, PowerType: Enums.ePowerType.Toggle }).ToList();
                fPrestige = new frmPrestige(iParent, iPowers);
            }

            if (fPrestige is { Visible: false })
            {
                ibPrestigePowersEx.ToggleState = MidsVectorButton.States.ToggledOn;
                fPrestige.Show(this);
            }
            else
            {
                ibPrestigePowersEx.ToggleState = MidsVectorButton.States.ToggledOff;
                fPrestige?.Close();
            }
        }

        private void ibAccoladesEx_OnClick(object? sender, EventArgs e)
        {
            if (fAccolade == null || fAccolade.IsDisposed)
            {
                fAccolade = new frmAccolade(this);
            }

            if (fAccolade is { Visible: false })
            {
                fAccolade.Show(this);
            }
            else
            {
                fAccolade?.Close();
            }
        }

        private void ibModeEx_OnClick(object sender, EventArgs eventArgs)
        {
            if (MainModule.MidsController.Toon == null)
            {
                return;
            }

            switch (modeEx.ToggleState)
            {
                case MidsVectorButton.States.ToggledOff:
                    MidsContext.Config.BuildMode = Enums.dmModes.LevelUp;
                    if (DatabaseAPI.ServerData.EnableInherentSlotting)
                    {
                        MainModule.MidsController.Toon.ClearInvalidInherentSlots();
                    }

                    break;
                case MidsVectorButton.States.ToggledOn:
                    MidsContext.Config.BuildMode = Enums.dmModes.Normal;
                    break;
                case MidsVectorButton.States.Indeterminate:
                    MidsContext.Config.BuildMode = Enums.dmModes.Respec;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!DatabaseAPI.LoadLevelsDatabase(MidsContext.Config.DataPath))
            {
                return;
            }

            MidsContext.Character?.ResetLevel();
            PowerModified(markModified: false);
            UpdateDmBuffer();
        }

        private void tsConfig_Click(object sender, EventArgs e)
        {
            FloatTop(false);
            var iParent = this;
            var frmCalcOpt = new frmCalcOpt(ref iParent);
            if (frmCalcOpt.ShowDialog(this) == DialogResult.OK)
            {
                MidsContext.Config.SaveConfig();
                //UpdateControls();
                //UpdateOtherFormsFonts();
            }

            frmCalcOpt.Dispose();
            tsIODefault.Text = "Default (" + (MidsContext.Config.I9.DefaultIOLevel + 1) + ")";
            FloatTop(true);

            // if (DbChangeRequested)
            // {
            //     using var iFrm = new frmBusy();
            //     _frmBusy = iFrm;
            //     _frmBusy.SetTitle(@"Database Change Requested");
            //     _frmBusy.Show();
            //     await MainModule.MidsController.ChangeDatabase(_frmBusy);
            // }
        }

        #endregion

        #region Internal Methods

        internal void PowerModified(bool markModified, bool redraw = true)
        {
            var index = -1;
            if (MainModule.MidsController.Toon != null)
            {
                MainModule.MidsController.Toon.Complete = false;
                FixStatIncludes();
                if (markModified)
                {
                    FileModified = true;
                }

                if (MidsContext.Config is { BuildMode: Enums.dmModes.Normal or Enums.dmModes.Respec })
                {
                    index = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex(MainModule.MidsController.Toon.RequestedLevel);
                    if (index < 0)
                    {
                        index = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex();
                    }
                }
                else if (MidsContext.Character != null && DatabaseAPI.Database.Levels[MidsContext.Character.Level].LevelType() == Enums.dmItem.Power)
                {
                    index = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex();
                    drawing?.HighlightSlot(-1);
                }

                if (MainModule.MidsController.Toon.Complete)
                {
                    drawing?.HighlightSlot(-1);
                }

                var slotCounts = MainModule.MidsController.Toon.GetSlotCounts();
                if (MidsContext.Character != null)
                {
                    slotCounts[0] = Build.TotalSlotsAvailable - MidsContext.Character.CurrentBuild.SlotsPlaced;
                    slotCounts[1] = MidsContext.Character.CurrentBuild.SlotsPlaced;
                }

                switch (slotCounts[0])
                {
                    case 0:
                        slotInfoEx.ToggleText.ToggledOff = @"No slots left";
                        break;
                    case < 0:
                        slotInfoEx.ToggleText.ToggledOff = slotCounts[0] switch
                        {
                            < 2 => $"{Math.Abs(slotCounts[0])} slot over",
                            > 1 => $"{Math.Abs(slotCounts[0])} slots over"
                        };
                        MessageBox.Show($"This build exceeds the slot limit.\r\nPlease remove {Math.Abs(slotCounts[0])} slots from the build.", @"Invalid Slotting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    default:
                        slotInfoEx.ToggleText.ToggledOff = slotCounts[0] switch
                        {
                            < 2 => $"{slotCounts[0]} slot to go",
                            > 1 => $"{slotCounts[0]} slots to go"
                        };
                        break;
                }

                slotInfoEx.ToggleText.ToggledOn = slotCounts[1] switch
                {
                    <= 0 => "No slots placed",
                    < 2 => $"{slotCounts[1]} slot placed",
                    > 1 => $"{slotCounts[1]} slots placed"
                };
            }

            if (MidsContext.Character != null && index > -1 & index <= MidsContext.Character.CurrentBuild.Powers.Count)
            {
                MidsContext.Character.RequestedLevel = MidsContext.Character.CurrentBuild.Powers[index].Level;
            }

            MidsContext.Character?.Validate();
            if (redraw)
            {
                canvas.RequestFullRedraw();
                canvas.ResizeToContent();
            }

            // if (redraw)
            // {
            //     UpdateUiControls();
            // }
            // if (redraw)
            // {
            //     DoRedraw();
            //     Application.DoEvents();
            //     UpdateControls();
            // }

            // RefreshInfo();
            UpdateModeInfo();
            RefreshInfo();
        }

        internal void FloatCompareGraph(bool show)
        {
            if (show)
            {
                if (fGraphCompare == null)
                {
                    var iFrm = this;
                    fGraphCompare = new frmCompare(ref iFrm);
                }

                fGraphCompare.SetLocation();
                fGraphCompare.Show();
                fGraphCompare.Activate();
            }
            else
            {
                if (fGraphCompare == null)
                {
                    return;
                }

                fGraphCompare.Hide();
                fGraphCompare.Dispose();
                fGraphCompare = null;
            }
        }

        internal void FloatSets(bool show)
        {
            if (show)
            {
                fSets ??= new frmSetViewer(this);
                fSets.SetLocation();
                fSets.Show();
                FloatUpdate();
                fSets.Activate();
            }
            else
            {
                if (fSets == null)
                {
                    return;
                }

                fSets.Hide();
                fSets.Dispose();
                fSets = null;
            }
        }

        internal void FloatStatGraph(bool show)
        {
            if (show)
            {
                if (fGraphStats == null)
                {
                    var iParent = this;
                    fGraphStats = new frmStats(ref iParent);
                }

                fGraphStats.SetLocation();
                fGraphStats.Show();
                fGraphStats.Activate();
            }
            else
            {
                if (fGraphStats == null)
                {
                    return;
                }

                fGraphStats.Hide();
                fGraphStats.Dispose();
                fGraphStats = null;
            }
        }

        internal void FloatBuildSalvageHud(bool show)
        {
            if (show)
            {
                fSalvageHud ??= new frmBuildSalvageHud(this);
                fSalvageHud.Show();
                FloatUpdate();
                //fSalvageHud.Activate();
            }
            else
            {
                if (fSalvageHud == null)
                {
                    return;
                }

                fSalvageHud.Hide();
                fSalvageHud.Dispose();
                fSalvageHud = null;
            }
        }

        internal void FloatTotals(bool show, bool useOld)
        {
            if (!useOld)
            {
                if (show)
                {
                    if (fTotals2 != null)
                    {
                        return;
                    }

                    var iParent = this;
                    fTotals2 = new frmTotalsV2(ref iParent);

                    fTotals2.SetLocation();
                    fTotals2.Show();
                    fTotals2.BringToFront();
                    fTotals2.UpdateData();
                    frmTotalsV2.SetTitle(fTotals2);
                    fTotals2.Activate();
                }
                else
                {
                    if (fTotals2 == null)
                    {
                        return;
                    }

                    fTotals2.Hide();
                    fTotals2.Dispose();
                    fTotals2 = null;
                }
            }
            else
            {
                if (show)
                {
                    if (fTotals != null)
                    {
                        return;
                    }

                    var iParent = this;
                    fTotals = new frmTotals(ref iParent);

                    fTotals.SetLocation();
                    fTotals.Show();
                    fTotals.BringToFront();
                    fTotals.UpdateData();
                    fTotals.Activate();
                }
                else
                {
                    if (fTotals == null)
                    {
                        return;
                    }

                    fTotals.Hide();
                    fTotals.Dispose();
                    fTotals = null;
                }
            }
        }

        internal string? GetBuildFile(bool stripExt = false)
        {
            if (MainModule.MidsController.Toon == null)
            {
                return "";
            }

            if (!stripExt)
            {
                return LastFileName;
            }

            var r = new Regex(@"\.(([tT][xX][tT])|([mM][hHxX][dD]))$");
            return r.Replace(LastFileName, "");
        }

        internal void SmlRespecShort(int iLevel)
        {
            SetMiniList(MidsContext.Character.CurrentBuild.GetRespecHelper2(false, iLevel), "Respec Helper (Brief)");
            fMini.Width = 350;
        }

        internal void SmlRespecLong(int iLevel)
        {
            SetMiniList(MidsContext.Character.CurrentBuild.GetRespecHelper2(true, iLevel), "Respec Helper (Detailed)");
            fMini.Width = 350;
        }

        internal void SetMiniList(PopUp.PopupData iData, string iTitle)
        {
            var newMini = fMini == null;
            fMini ??= new frmMiniList(this);
            fMini.Text = iTitle;
            fMini.SetData(iData, !newMini);
            if (newMini)
            {
                fMini.Show();
            }
            fMini.BringToFront();
        }

        internal void UnSetMiniList()
        {
            fMini?.Dispose();
        }

        internal void FloatRecipe(bool show)
        {
            if (show)
            {
                fRecipe ??= new frmRecipeViewer(this);
                fRecipe.SetLocation();
                fRecipe.Show();
                FloatUpdate();
                fRecipe.Activate();
            }
            else
            {
                if (fRecipe == null)
                {
                    return;
                }

                fRecipe.Hide();
                fRecipe.Dispose();
                fRecipe = null;
            }
        }

        #endregion

        #region Private Methods

        private static void RestoreWindowPlacement(Form form, ConfigData cfg)
        {
            // 1) Bounds
            var target = RestoreBoundsSafe(cfg.Bounds);

            // If cfg.Bounds was empty/unset, center the form reasonably:
            if (target.Width <= 0 || target.Height <= 0)
            {
                var wa = Screen.PrimaryScreen!.WorkingArea;
                target = new Rectangle(
                    wa.Left + (wa.Width - form.Width) / 2,
                    wa.Top + (wa.Height - form.Height) / 2,
                    Math.Max(form.Width, 1280), // your defaults
                    Math.Max(form.Height, 720));
            }

            // Ensure the rectangle is visible on any screen (handles multi-monitor changes)
            target = CoerceToVisible(target);

            form.Bounds = target;

            // 2) WindowState (apply after Bounds so it maximizes on the correct screen)
            if (Enum.TryParse(cfg.WindowState, out FormWindowState state))
            {
                // Never start minimized on fresh launch; fall back to Normal
                if (state == FormWindowState.Minimized) state = FormWindowState.Normal;
                form.WindowState = state;
            }
            else
            {
                form.WindowState = FormWindowState.Normal;
            }
        }

        private static Rectangle RestoreBoundsSafe(Rectangle r)
        {
            // Defend against default(Rectangle) or negative sizes from old configs
            if (r.Width <= 0 || r.Height <= 0)
                return Rectangle.Empty;
            return r;
        }

        private static Rectangle CoerceToVisible(Rectangle desired)
        {
            // If any portion of the window intersects an attached screen, accept it; otherwise, move to primary
            var any = Screen.AllScreens.Any(s => s.WorkingArea.IntersectsWith(desired));
            if (any) return desired;

            var wa = Screen.PrimaryScreen!.WorkingArea;
            var width = Math.Min(desired.Width, wa.Width);
            var height = Math.Min(desired.Height, wa.Height);

            return new Rectangle(
                wa.Left + Math.Max(0, Math.Min(desired.Left - wa.Left, wa.Width - width)),
                wa.Top + Math.Max(0, Math.Min(desired.Top - wa.Top, wa.Height - height)),
                width,
                height);
        }

        private bool ShouldCancelClose()
        {
            var toon = MainModule.MidsController.Toon;
            if (toon is null) return false;

            if (!(toon.Locked && FileModified)) return false;

            FloatTop(false);
            var choice = MessageBoxEx.ShowDialog(this, @"Do you wish to save your build before closing?", @"Question",
                MessageBoxEx.MessageBoxExButtons.YesNo, MessageBoxEx.MessageBoxExIcon.Question);

            FloatTop(true);

            return choice switch
            {
                DialogResult.Yes => !SaveBuild(),
                _ => false
            };
        }

        private void PopulateThemeMenuItems()
        {
            if (_themeMenu is null) return;

            _themeMenu.DropDownItems.Clear();


            var themes = ThemeManager.AvailableThemes.OrderBy(t => t.IsUser)
                .ThenBy(t => t.Name)
                .ToList();

            var active = ThemeManager.CurrentTheme?.Name;

            foreach (var theme in themes)
            {
                var display = theme.IsUser ? $"{theme.Name} (User)" : theme.Name;
                var item = new ToolStripMenuItem(display)
                {
                    Tag = theme.Name,
                    Checked = string.Equals(theme.Name, active, StringComparison.OrdinalIgnoreCase)
                };
                item.Click += (_, __) =>
                {
                    ThemeManager.SetTheme((string)item.Tag!);
                    UpdateCheckedTheme();
                    // (optional) persist:
                    // Properties.Settings.Default.LastThemeName = (string)item.Tag!;
                    // Properties.Settings.Default.Save();
                };
                _themeMenu.DropDownItems.Add(item);
            }

            _themeMenu.DropDownItems.Add(new ToolStripSeparator());
            var reload = new ToolStripMenuItem("Reload Custom Themes…");
            reload.Click += (_, __) =>
            {
                // If you added ThemeManager.ReloadCustomThemes(); prefer it.
                ThemeManager.ReloadCustomThemes();
                PopulateThemeMenuItems();
                UpdateCheckedTheme();
            };
            _themeMenu.DropDownItems.Add(reload);
        }

        private void UpdateCheckedTheme()
        {
            if (_themeMenu is null) return;
            var active = ThemeManager.CurrentTheme?.Name;
            foreach (var mi in _themeMenu.DropDownItems.OfType<ToolStripMenuItem>())
            {
                if (mi.Tag is string name)
                    mi.Checked = string.Equals(name, active, StringComparison.OrdinalIgnoreCase);
            }
        }

        private void FloatRotationHelper(bool show)
        {
            if (show)
            {
                // ???
                if (fRotationHelper?.IsDisposed == true)
                {
                    fRotationHelper = null;
                }

                fRotationHelper ??= new frmRotationHelper(this);
                //fRotationHelper.SetLocation();
                fRotationHelper.Show();
                FloatUpdate();
                fRotationHelper.Activate();
            }
            else
            {
                if (fRotationHelper == null)
                {
                    return;
                }

                fRotationHelper.Hide();
                fRotationHelper = null;
            }
        }

        private void LoadLegacyOrGameFile(string? fName)
        {
            if (!File.Exists(fName)) return;

            dataView.IsLocked = false;
            NewToon(true, true);
            Stream? mStream = null;
            if (fName.Trim(' ', '"').EndsWith(".txt"))
            {
                GameImport(fName);
            }
            else if (MainModule.MidsController.Toon != null && !MainModule.MidsController.Toon.Load(fName, ref mStream))
            {
                NewToon();
                LastFileName = "";
                MidsContext.Config.LastFileName = "";
            }
            else
            {
                LastFileName = fName;
                if (!fName.EndsWith("mids_build.mxd"))
                {
                    MidsContext.Config.LastFileName = fName;
                }
            }

            FileModified = false;
            if (drawing != null)
            {
                drawing.Highlight = -1;
            }

            //dataView?.Clear();
            MidsContext.Character?.ResetLevel();
            PowerModified(false);
            //UpdateControls(true);
            //SetTitleBar();
            Application.DoEvents();
            GetBestDamageValues();
            //SetEnhCheckModePosition();
            //UpdateColors();
            FloatUpdate(true);
        }

        private bool LoadCharacterFile(string? fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            dataView.IsLocked = false;
            NewToon(true, true);
            if (_buildManager.LoadFromFile(fileName))
            {
                MidsContext.Config.LastFileName = fileName;
                LastFileName = fileName;
            }
            else
            {
                MidsContext.Config.LastFileName = string.Empty;
                LastFileName = string.Empty;
                return false;
            }

            FileModified = false;
            if (drawing != null)
            {
                drawing.Highlight = -1;
            }

            //dataView?.Clear();
            MidsContext.Character?.ResetLevel();
            PowerModified(false);
            SetLockedPoolsState();
            //UpdateControls(true);
            //SetTitleBar();
            Application.DoEvents();
            GetBestDamageValues();
            //UpdateColors();
            DoRedraw();
            FloatUpdate(true);

            return true;
        }

        private bool SaveBuild()
        {
            if (string.IsNullOrEmpty(LastFileName))
            {
                return SaveBuildAs();
            }

            var fileInfo = new FileInfo(LastFileName);
            switch (fileInfo.Extension)
            {
                case ".mxd" or ".txt":
                    var fileName = LastFileName.Replace(fileInfo.Extension, ".mbd");
                    LastFileName = fileName;
                    break;
            }
            if (!_buildManager.SaveToFile(LastFileName))
            {
                return false;
            }

            MidsContext.Config.LastFileName = LastFileName;
            FileModified = false;
            //SetTitleBar();
            return true;
        }

        private bool SaveBuildAs()
        {
            FloatTop(false);
            string saveFile;

            if (!string.IsNullOrWhiteSpace(LastFileName))
            {
                var fileInfo = new FileInfo(LastFileName);
                saveFile = fileInfo.Name.Replace(fileInfo.Extension, "");
                DlgSave.InitialDirectory = fileInfo.Directory.FullName;
            }
            else if (!string.IsNullOrWhiteSpace(MidsContext.Character.Name))
            {
                saveFile = $"{MidsContext.Character.Name} - {MidsContext.Character.Archetype.DisplayName} ({MidsContext.Character.Powersets[0].DisplayName} - {MidsContext.Character.Powersets[1].DisplayName})";
            }
            else
            {
                saveFile = $"{MidsContext.Character.Archetype.DisplayName} ({MidsContext.Character.Powersets[0].DisplayName} - {MidsContext.Character.Powersets[1].DisplayName})";
            }

            DlgSave.FileName = saveFile;

            if (DlgSave.ShowDialog() == DialogResult.OK)
            {
                var buildFile = DlgSave.FileName;
                var extension = Path.GetExtension(buildFile).ToUpperInvariant();
                switch (extension)
                {
                    case ".MBD":
                        if (!_buildManager.SaveToFile(buildFile))
                        {
                            return false;
                        }
                        break;
                    case ".MXD":
                        if (!MainModule.MidsController.Toon.Save(DlgSave.FileName))
                        {
                            return false;
                        }
                        break;
                }

                LastFileName = buildFile;
                FileModified = false;
                //SetTitleBar(MidsContext.Character.IsHero());

                return true;
            }

            FloatTop(true);

            return false;
        }

        private void SetLockedPoolsState()
        {
            // Pool powerset must be selected and have at least one power picked
            for (var i = 0; i < 5; i++)
            {
                MainModule.MidsController.Toon.PoolLocked[i] = (MidsContext.Character.Powersets[i + 3]?.nID ?? -1) > -1 &&
                                                               MidsContext.Character.CurrentBuild.Powers.Any(e => e?.Power != null && e.Power.FullName.StartsWith(MidsContext.Character.Powersets[i + 3] == null ? "   " : MidsContext.Character.Powersets[i + 3].FullName));
            }
        }

        private void GameImport(string? buildString)
        {
            try
            {
                var importHandle = new ImportFromBuildsave(buildString);
                var listPowers = importHandle.Parse();

                if (listPowers == null)
                {
                    return;
                }

                InjectBuild(buildString, listPowers, importHandle.GetPowersets(), importHandle.GetCharacterInfo());
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\r\n\r\n{e.StackTrace}");
            }
        }

        private void ForumImport(string? buildString)
        {
            try
            {
                var importHandle = new ImportFromBuildsave(buildString);
                var listPowers = importHandle.ParseForumPost();

                if (listPowers == null)
                {
                    return;
                }

                InjectBuild(buildString, listPowers, importHandle.GetPowersets(), importHandle.GetCharacterInfo());
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.StackTrace}\r\n\r\n{e.Message}");
            }
        }

        private void InjectBuild(string? buildFile, List<PowerEntry> listPowers, UniqueList<string> listPowersetsFull, RawCharacterInfo characterInfo, bool addToAutoOpen = false)
        {
            var buildMode = MidsContext.Config.BuildMode;

            if (buildMode == Enums.dmModes.LevelUp)
            {
                MidsContext.Config.BuildMode = Enums.dmModes.Respec;
            }

            var psFullNames = listPowersetsFull
                .Where(e => !e.StartsWith("Incarnate.Lore_Pet_"))
                .Select(e => e.Contains('.')
                    ? e
                    : DatabaseAPI.GetPowersetByName(e, characterInfo.Archetype)?.FullName)
                .Distinct()
                .ToList();

            listPowersetsFull = new UniqueList<string>();
            foreach (var ps in psFullNames)
            {
                listPowersetsFull.Add(ps);
            }

            var listPowersets = new UniqueList<string>();
            var trunkPowersets = listPowersetsFull
                .Select(e => DatabaseAPI.GetPowersetByFullname(e) ?? null)
                .Where(e => e is { SetType: Enums.ePowerSetType.Primary or Enums.ePowerSetType.Secondary, nIDTrunkSet: > -1 })
                .Select(e => DatabaseAPI.Database.Powersets[e.nIDTrunkSet].FullName)
                .ToList();

            foreach (var ps in listPowersetsFull)
            {
                if (!trunkPowersets.Contains(ps))
                {
                    listPowersets.Add(ps);
                }
            }

            // Need to pad pools powers list so there are 4
            // So epic pools doesn't end up shown as a regular pool...
            ImportBase.FilterVEATPools(ref listPowersets);
            ImportBase.FixUndetectedPowersets(ref listPowersets);
            ImportBase.FinalizePowersetsList(ref listPowersets, listPowers, trunkPowersets);
            ImportBase.PadPowerPools(ref listPowersets);
            ImportBase.FilterTempPowersets(ref listPowersets);
            ImportBase.SortPowersets(ref listPowersets);

            var toBlameSet = string.Empty;
            MidsContext.Character.LoadPowersetsByName2(listPowersets, ref toBlameSet);
            MidsContext.Character.CurrentBuild.LastPower = 24;
            //MidsContext.Character.GetPowersByLevel(characterInfo.Level - 1);

            var powerEntryList = listPowers.OrderBy(x => x.Level).ToList();
            var pickedSlots = 0;
            try
            {
                for (var k = 0; k < listPowers.Count; k++)
                {
                    if (powerEntryList[k].PowerSet?.FullName.Contains("Inherent") == true)
                    {
                        continue;
                    }

                    // Incarnate, Temps, Accolades
                    if (powerEntryList[k].PowerSet?.FullName.StartsWith("Incarnate") == true |
                        powerEntryList[k].PowerSet?.FullName.StartsWith("Temporary_Powers") == true)
                    {
                        if (!MidsContext.Character.CurrentBuild.PowerUsed(powerEntryList[k].Power))
                        {
                            MidsContext.Character.CurrentBuild.AddPower(powerEntryList[k].Power, 49).StatInclude = true;
                        }

                        continue;
                    }

                    // Regular powers
                    PowerPickedNoRedraw(powerEntryList[k].NIDPowerset, powerEntryList[k].NIDPower);
                }
            }
            catch (Exception ex)
            {
                MidsContext.Config.BuildMode = buildMode;
                MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}");
            }

            var sl = new SlotLevelQueue();
            try
            {
                foreach (var pe in MidsContext.Character.CurrentBuild.Powers)
                {
                    if (pe?.Power == null)
                    {
                        continue; // Not picked power will be in the list, but not instantiated!
                    }

                    var pList = powerEntryList.Where(e => pe.Power.FullName == e.Power?.FullName).ToArray();
                    if (pList.Length == 0)
                    {
                        continue;
                    }

                    if (!DatabaseAPI.Database.Power[pe.NIDPower].Slottable)
                    {
                        continue;
                    }

                    if (DatabaseAPI.Database.Power[pe.NIDPower].VariableEnabled)
                    {
                        var initialStacks = Math.Max(DatabaseAPI.Database.Power[pe.NIDPower].VariableMin,
                            Math.Min(DatabaseAPI.Database.Power[pe.NIDPower].VariableMax,
                                DatabaseAPI.Database.Power[pe.NIDPower].VariableStart));
                        pe.VariableValue = initialStacks;
                        pe.Power.Stacks = initialStacks;
                    }

                    var p = pList.First();
                    while (pe.Slots.Length < p.Slots.Length)
                    {
                        pe.AddSlot(Character.MaxLevel);
                    }

                    p.Slots.CopyTo(pe.Slots, 0);
                    for (var i = 0; i < pe.Slots.Length; i++)
                    {
                        if (i == 0)
                        {
                            pe.Slots[i].Level = pe.Level;
                        }
                        else
                        {
                            pe.Slots[i].Level = sl.PickSlot();
                            pickedSlots++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MidsContext.Config.BuildMode = buildMode;
                MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}");
            }

            FixStatIncludes();
            FileModified = false;
            MidsContext.Character.Lock();
            MidsContext.Character.PoolShuffle();
            AssetManager.OriginIndex = MidsContext.Character.Origin;
            if (addToAutoOpen)
            {
                MidsContext.Config.LastFileName = buildFile;
                LastFileName = buildFile;
            }
            else
            {
                MidsContext.Config.LastFileName = "";
                LastFileName = "";
            }

            //SetEnhCheckModePosition();
            //SetTitleBar();

            var idx = -1;
            if (MidsContext.Config.BuildMode is Enums.dmModes.Normal or Enums.dmModes.Respec)
            {
                idx = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex(MainModule.MidsController.Toon.RequestedLevel);
                if (idx < 0)
                {
                    idx = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex();
                }
            }
            else if (DatabaseAPI.Database.Levels[MidsContext.Character.Level].LevelType() == Enums.dmItem.Power)
            {
                idx = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex();
                drawing.HighlightSlot(-1);
            }

            if (MainModule.MidsController.Toon.Complete)
            {
                drawing.HighlightSlot(-1);
            }

            if ((idx > -1) & (idx <= MidsContext.Character.CurrentBuild.Powers.Count))
            {
                MidsContext.Character.RequestedLevel = MidsContext.Character.CurrentBuild.Powers[idx].Level;
                MidsContext.Character.SetLevelTo(MidsContext.Character.CurrentBuild.Powers[idx].Level);
            }
            else
            {
                MidsContext.Character.RequestedLevel = Character.MaxLevel;
                MidsContext.Character.SetLevelTo(Character.MaxLevel);
            }

            MidsContext.Archetype = MidsContext.Character.Archetype;
            MidsContext.Character.Validate();
            MidsContext.Character.Lock();
            MidsContext.Character.ResetLevel();
            MidsContext.Character.PoolShuffle();
            AssetManager.OriginIndex = MidsContext.Character.Origin;
            MidsContext.Character.Validate();
            var powerEntryArray = DeepCopyPowerList();
            RearrangeAllSlotsInBuild(powerEntryArray, true);
            ShallowCopyPowerList(powerEntryArray);
            PowerModified(false);
            DoRedraw();

            MidsContext.Config.BuildMode = buildMode;
        }

        private bool RunSchemaCommands(string url)
        {
            var returnData = false;
            var code = url.Replace("mrb://", "");
            var options = new RestClientOptions("https://api.midsreborn.com")
            {
                Timeout = TimeSpan.FromSeconds(30),
            };
            using var client = new RestClient(options);
            var response = client.Get<SchemaData>($"build/redirect-to-schema/{code}");
            if (response is not null)
            {
                returnData = DoLoadFromSchema(response);
            }
            return returnData;
        }

        private bool DoLoadFromSchema(SchemaData response)
        {
            dataView.IsLocked = false;
            NewToon(true, true);
            var ret = response.Data != null && _buildManager.ValidateAndLoadSchemaData(response.Data);
            FileModified = false;
            if (drawing != null)
            {
                drawing.Highlight = -1;
            }

            //dataView?.Clear();
            PowerModified(false);
            return ret;
        }

        private void PowerPickedNoRedraw(int nIdPowerset, int nIdPower)
        {
            MainModule.MidsController.Toon.BuildPower(nIdPowerset, nIdPower, true);
            PowerModified(markModified: true);
        }

        private void FloatTop(bool onTop)
        {
            if (!onTop)
            {
                if (fSets != null)
                {
                    top_fSets = fSets.TopMost;
                    if (fSets.TopMost)
                    {
                        fSets.TopMost = false;
                    }
                }

                if (fGraphStats != null)
                {
                    top_fGraphStats = fGraphStats.TopMost;
                    if (fGraphStats.TopMost)
                    {
                        fGraphStats.TopMost = false;
                    }
                }

                if (fGraphCompare != null)
                {
                    top_fGraphCompare = fGraphCompare.TopMost;
                    if (fGraphCompare.TopMost)
                    {
                        fGraphCompare.TopMost = false;
                    }
                }

                if (MidsContext.Config.UseOldTotalsWindow)
                {
                    if (fTotals != null)
                    {
                        top_fTotals = fTotals.TopMost;
                        if (fTotals.TopMost)
                        {
                            fTotals.TopMost = false;
                        }
                    }
                }
                else
                {
                    if (fTotals2 != null)
                    {
                        top_fTotals = fTotals2.TopMost;
                        if (fTotals2.TopMost)
                        {
                            fTotals2.TopMost = false;
                        }
                    }
                }

                if (fRecipe != null)
                {
                    top_fRecipe = fRecipe.TopMost;
                    if (fRecipe.TopMost)
                    {
                        fRecipe.TopMost = false;
                    }
                }

                if (fData != null)
                {
                    top_fData = fData.TopMost;
                    if (fData.TopMost)
                    {
                        fData.TopMost = false;
                    }
                }

                if (fSetFinder == null)
                {
                    return;
                }

                top_fSetFinder = fSetFinder.TopMost;
                if (fSetFinder.TopMost)
                {
                    fSetFinder.TopMost = false;
                }
            }
            else
            {
                BringToFront();
                if (fSets != null && fSets.TopMost != top_fSets)
                {
                    fSets.TopMost = top_fSets;
                    if (fSets.TopMost)
                    {
                        fSets.BringToFront();
                    }
                }

                if (fGraphStats != null && fGraphStats.TopMost != top_fGraphStats)
                {
                    fGraphStats.TopMost = top_fGraphStats;
                    if (fGraphStats.TopMost)
                    {
                        fGraphStats.BringToFront();
                    }
                }

                if (fGraphCompare != null && fGraphCompare.TopMost != top_fGraphCompare)
                {
                    fGraphCompare.TopMost = top_fGraphCompare;
                    if (fGraphCompare.TopMost)
                    {
                        fGraphCompare.BringToFront();
                    }
                }

                if (MidsContext.Config.UseOldTotalsWindow)
                {
                    if (fTotals != null && fTotals.TopMost != top_fTotals)
                    {
                        fTotals.TopMost = top_fTotals;
                        if (fTotals.TopMost)
                        {
                            fTotals.BringToFront();
                        }
                    }
                }
                else
                {
                    if (fTotals2 != null && fTotals2.TopMost != top_fTotals)
                    {
                        fTotals2.TopMost = top_fTotals;
                        if (fTotals2.TopMost)
                        {
                            fTotals2.BringToFront();
                        }
                    }
                }

                if (fRecipe != null && fRecipe.TopMost != top_fRecipe)
                {
                    fRecipe.TopMost = top_fRecipe;
                    if (fRecipe.TopMost)
                    {
                        fRecipe.BringToFront();
                    }
                }

                if (fData != null && fData.TopMost != top_fData)
                {
                    fData.TopMost = top_fData;
                    if (fData.TopMost)
                    {
                        fData.BringToFront();
                    }
                }

                if (fSetFinder == null || fSetFinder.TopMost == top_fSetFinder)
                {
                    return;
                }

                fSetFinder.TopMost = top_fSetFinder;
                if (fSetFinder.TopMost)
                {
                    fSetFinder.BringToFront();
                }
            }
        }

        private void EditAccoladesOrTemps(int hIdPower)
        {
            if (hIdPower <= -1 || MidsContext.Character.CurrentBuild.Powers[hIdPower].SubPowers.Length <= 0)
            {
                return;
            }

            var iPowers = MidsContext.Character.CurrentBuild.Powers[hIdPower].SubPowers
                .Select(t => DatabaseAPI.Database.Power[t.nIDPower])
                .ToList();
            using var frmAccolade = new frmAccolade(this, iPowers);
            frmAccolade.Text = DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[hIdPower].NIDPower].DisplayName;
            frmAccolade.ShowDialog(this);
            EnhancementModified();
            LastClickPlacedSlot = false;
        }

        private void OnGradePick(Enums.eEnhGrade grade)
        {
            if (MidsContext.Character == null)
            {
                return;
            }

            if (MidsContext.Character.CurrentBuild.SetEnhGrades(grade))
            {
                //I9Picker.Ui.Initial.GradeId = grade;
            }
            Info_Totals();
            DoRedraw();
        }

        private void DisplayFormatChanged()
        {
            GetBestDamageValues();
            RefreshInfo();
        }

        private void FloatData(bool show)
        {
            if (show)
            {
                if (fData == null)
                {
                    var iParent = this;
                    fData = new frmData(() => FloatData(false));
                }

                fData.SetLocation();
                fData.Show();
                FloatUpdate();
                fData.Activate();
            }
            else
            {
                if (fData == null)
                {
                    return;
                }

                fData.Hide();
                fData.Dispose();
                fData = null;
            }
        }

        private void SetDamageMenuCheckMarks()
        {
            switch (MidsContext.Config.DamageMath.ReturnValue)
            {
                case ConfigData.EDamageReturn.Numeric:
                    tsViewDPS_New.Checked = false;
                    tsViewActualDamage_New.Checked = true;
                    tlsDPA.Checked = false;
                    break;
                case ConfigData.EDamageReturn.DPS:
                    tsViewDPS_New.Checked = true;
                    tsViewActualDamage_New.Checked = false;
                    tlsDPA.Checked = false;
                    break;
                case ConfigData.EDamageReturn.DPA:
                    tsViewDPS_New.Checked = false;
                    tsViewActualDamage_New.Checked = false;
                    tlsDPA.Checked = true;
                    break;
            }
        }

        private I9Slot? GetRepeatEnhancement(int powerIndex, int iSlotIndex)
        {
            if (LastEnhPlaced == null)
            {
                return new I9Slot();
            }

            if (MidsContext.Character.CurrentBuild.Powers[powerIndex].NIDPower < 0)
            {
                return new I9Slot();
            }

            if (LastEnhPlaced.Enh <= -1)
            {
                return new I9Slot();
            }

            if (DatabaseAPI.Database.Enhancements[LastEnhPlaced.Enh].TypeID != Enums.eType.SetO)
            {
                return DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[powerIndex].NIDPower]
                    .IsEnhancementValid(LastEnhPlaced.Enh)
                    ? LastEnhPlaced
                    : new I9Slot();
            }

            var firstValidSetEnh = GetFirstValidSetEnh(iSlotIndex, powerIndex);
            if (firstValidSetEnh <= -1)
            {
                return new I9Slot();
            }

            LastEnhPlaced.Enh = firstValidSetEnh;
            LastEnhPlaced.IOLevel = DatabaseAPI.Database.Enhancements[firstValidSetEnh]
                .CheckAndFixIOLevel(LastEnhPlaced.IOLevel);
            return LastEnhPlaced;
        }

        private int GetFirstValidSetEnh(int slotIndex, int hID)
        {
            if (LastEnhPlaced == null || LastEnhPlaced.Enh < 0 || DatabaseAPI.Database.Enhancements[LastEnhPlaced.Enh].TypeID != Enums.eType.SetO)
            {
                return -1;
            }

            var nIdSet = DatabaseAPI.Database.Enhancements[LastEnhPlaced.Enh].nIDSet;
            if (nIdSet < 0)
            {
                return -1;
            }

            if (MidsContext.Character.CurrentBuild.EnhancementTest(slotIndex, hID, LastEnhPlaced.Enh, true))
            {
                return LastEnhPlaced.Enh;
            }

            var setEnhancements = DatabaseAPI.Database.EnhancementSets[nIdSet].Enhancements
                .OrderBy(e => e < 0 ? "" : DatabaseAPI.Database.Enhancements[e].UID)
                .ToArray();
            for (var index = 0; index < DatabaseAPI.Database.EnhancementSets[nIdSet].Enhancements.Length; index++)
            {
                if (MidsContext.Character.CurrentBuild.EnhancementTest(slotIndex, hID, setEnhancements[index], true))
                {
                    return setEnhancements[index];
                }
            }

            return -1;
        }

        private void StartFlip(int iPowerIndex)
        {
            if (FlipActive)
            {
                EndFlip();
            }

            if (iPowerIndex <= -1 || MidsContext.Character.CurrentBuild.Powers[iPowerIndex].Slots.Length == 0)
            {
                return;
            }

            FileModified = true;
            MainModule.MidsController.Toon.FlipSlots(iPowerIndex);
            RefreshInfo();
            FlipPowerID = iPowerIndex;
            FlipSlotState = new int[MidsContext.Character.CurrentBuild.Powers[iPowerIndex].Slots.Length];
            var num = FlipSlotState.Length - 1;
            for (var index = 0; index <= num; ++index)
                FlipSlotState[index] = -(FlipStepDelay * index);
            FlipGP = new PowerEntry();
            FlipGP.Assign(MidsContext.Character.CurrentBuild.Powers[iPowerIndex]);
            FlipGP.Slots = Array.Empty<SlotEntry>();
            tmrGfx ??= new System.Windows.Forms.Timer(Container!);
            tmrGfx.Interval = FlipInterval;
            FlipActive = true;
            tmrGfx.Enabled = true;
            tmrGfx.Start();
        }

        private void DoFlipStep()
        {
            if (!FlipActive)
            {
                return;
            }

            var currentBuild = MidsContext.Character.CurrentBuild;
            var power = currentBuild.Powers[FlipPowerID];
            drawing.DrawPowerSlot(ref power);
            var index = -1;
            var Enh1 = -1;
            var Enh2 = -1;
            I9Slot? i9Slot1 = null;
            I9Slot? i9Slot2 = null;
            var recolorIa = BuildRenderer.GetRecolorIa(MainModule.MidsController.Toon.IsHero());
            using var solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
            var num1 = FlipSlotState.Length - 1;
            Rectangle rectangle1;
            for (var i = 0; i <= num1; ++i)
            {
                ++FlipSlotState[i];
                var num2 = 1f;
                var powerEntry = MidsContext.Character.CurrentBuild.Powers[FlipPowerID];
                var slot = powerEntry.Slots[i];
                if (FlipSlotState[i] < 0)
                {
                    index = slot.FlippedEnhancement.Enh;
                    Enh1 = index;
                    Enh2 = slot.Enhancement.Enh;
                    i9Slot1 = slot.FlippedEnhancement;
                    i9Slot2 = slot.Enhancement;
                }
                else if (FlipSlotState[i] > FlipSteps)
                {
                    index = slot.Enhancement.Enh;
                    Enh1 = index;
                    Enh2 = slot.FlippedEnhancement.Enh;
                    i9Slot1 = slot.Enhancement;
                    i9Slot2 = slot.FlippedEnhancement;
                }

                if (FlipSlotState[i] >= 0 && FlipSlotState[i] <= FlipSteps)
                {
                    var num3 = FlipSlotState[i] / (FlipSteps / 2f);
                    if (num3 > 1.0)
                    {
                        num2 = (float)(-1.0 * (1.0 - num3));
                        index = slot.Enhancement.Enh;
                        Enh1 = index;
                        Enh2 = slot.FlippedEnhancement.Enh;
                        i9Slot1 = slot.Enhancement;
                        i9Slot2 = slot.FlippedEnhancement;
                    }
                    else
                    {
                        num2 = 1f - num3;
                        index = slot.FlippedEnhancement.Enh;
                        Enh1 = index;
                        Enh2 = slot.Enhancement.Enh;
                        i9Slot1 = slot.FlippedEnhancement;
                        i9Slot2 = slot.Enhancement;
                    }
                }

                rectangle1 = drawing.GetEnhancementSlotRect(FlipPowerID, i);
                if (!(num2 > 0.0))
                {
                    continue;
                }

                if (rectangle1.IsEmpty)
                {
                    continue;
                }

                var rectangle2 = new Rectangle((int)Math.Round(rectangle1.X + (rectangle1.Width - rectangle1.Width * num2) / 2.0),
                    rectangle1.Y,
                    (int)Math.Round(rectangle1.Width * num2),
                    rectangle1.Height);
                if (index > -1)
                {
                    var graphics = drawing.BxBuffer.Graphics;
                    if (i9Slot1 != null)
                    {
                        AssetManager.DrawFlippingEnhancement(graphics, rectangle1, num2,
                            DatabaseAPI.Database.Enhancements[index].ImageIdx,
                            AssetManager.ToGfxGrade(DatabaseAPI.Database.Enhancements[index].TypeID, i9Slot1.Grade));
                    }
                }
                else
                {
                    drawing.BxBuffer.Graphics?.DrawImage(AssetManager.EmptySlot.Bitmap, rectangle2, 0, 0, 128, 128,
                        GraphicsUnit.Pixel, recolorIa);
                }

                if ((MidsContext.Config.CalcEnhLevel == Enums.eEnhRelative.None) | (slot.Level >= MidsContext.Config.ForceLevel) | ((drawing.InterfaceMode == Enums.eInterfaceMode.PowerToggle) & !powerEntry.StatInclude))
                {
                    rectangle2.Inflate(1, 1);
                    drawing.BxBuffer.Graphics?.FillEllipse(solidBrush, rectangle2);
                }

                if (!((dataView == null) | (i9Slot1 == null) | (i9Slot2 == null)))
                {
                    dataView?.FlipStage(i, Enh1, Enh2, num2, powerEntry.NIDPower, i9Slot1.Grade, i9Slot2.Grade);
                }
            }

            rectangle1 = drawing.GetPowerAreaRect(FlipPowerID);
            rectangle1.Inflate(2, 2);
            canvas.Invalidate(rectangle1);
            if (FlipSlotState[^1] >= FlipSteps)
            {
                EndFlip();
            }
        }

        private void tmrGfx_Tick(object? sender, EventArgs e)
        {
            if (FlipActive)
            {
                DoFlipStep();
            }
        }

        private void EndFlip()
        {
            FlipActive = false;
            tmrGfx.Enabled = false;
            FlipPowerID = -1;
            FlipSlotState = [];
            DoRedraw();
        }

        private void EnhancementModified()
        {
            DoRedraw();
            RefreshInfo();
        }

        private void DataView_SlotFlip(int powerIndex)
        {
            StartFlip(powerIndex);
        }

        private void DataView_SlotUpdate(IPower? power, int val)
        {
            DoRedraw();
            RefreshInfo();
            if (_frmTeam?.Visible != true || power == null)
            {
                return;
            }

            var pKey = power.CSPrimaryKey;
            if (pKey == null)
            {
                return;
            }

            _frmTeam.FeedbackUpdate(pKey, val);
        }

        internal void DoRedraw()
        {
            canvas.RequestFullRedraw();
        }

        private void UpdateModeInfo()
        {
            switch (MidsContext.Config?.BuildMode)
            {
                case Enums.dmModes.LevelUp:
                    modeEx.ToggleText.ToggledOff = MainModule.MidsController.Toon is { Complete: false }
                        ? $"Level-Up: {MidsContext.Character?.Level + 1}"
                        : @"Level-Up";
                    if (modeEx.Text != modeEx.ToggleText.ToggledOff)
                    {
                        modeEx.Text = MainModule.MidsController.Toon is { Complete: false }
                            ? $"Level-Up: {MidsContext.Character?.Level + 1}"
                            : @"Level-Up";
                    }

                    modeEx.ToggleState = MidsVectorButton.States.ToggledOff;
                    break;
                case Enums.dmModes.Normal:
                    modeEx.ToggleText.ToggledOn = @"Normal";
                    modeEx.ToggleState = MidsVectorButton.States.ToggledOn;
                    break;
                case Enums.dmModes.Respec:
                    modeEx.ToggleText.Indeterminate = @"Respec";
                    modeEx.ToggleState = MidsVectorButton.States.Indeterminate;
                    break;
                case Enums.dmModes.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void FixStatIncludes()
        {
            if (MainModule.MidsController.Toon == null)
            {
                return;
            }

            foreach (var pe in MidsContext.Character.CurrentBuild.Powers)
            {
                if (pe?.Power == null)
                {
                    continue;
                }

                if (pe.Power.FullName.StartsWith("Temporary_Powers.Temporary_Powers."))
                {
                    pe.StatInclude = tempPowersEx.ToggleState switch
                    {
                        MidsVectorButton.States.ToggledOff => false,
                        MidsVectorButton.States.ToggledOn => true,
                        _ => pe.Power.AlwaysToggle
                    };
                }
                else if (pe.Power is not ({ PowerType: Enums.ePowerType.Toggle } or { PowerType: Enums.ePowerType.GlobalBoost } or { PowerType: Enums.ePowerType.Auto_ }) & // Not a toggle, global boost, auto
                         pe.Power is { ClickBuff: false } & // Not a click-buff
                         pe.Slots.Select(e => e.Enhancement.Enh).Any(e => e > -1)) // Has at least one enhancement slotted
                {
                    pe.StatInclude = true;
                }
            }
        }

        private void TsViewSelected()
        {
            switch (MidsContext.Config.ColumnStackingMode)
            {
                // case Enums.eColumnStacking.Horizontal:
                //     tsView3ColH.Checked = true;
                //     break;
                //
                // case Enums.eColumnStacking.Vertical:
                //     tsView3ColV.Checked = true;
                //     break;

                default:
                    switch (MidsContext.Config.Columns)
                    {
                        case 2:
                            tsView2Col.Checked = true;
                            break;
                        case 3:
                            tsView3Col.Checked = true;
                            break;
                        case 4:
                            tsView4Col.Checked = true;
                            break;
                    }

                    break;
            }
        }

        private void UpdateDmBuffer()
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character == null)
            {
                return;
            }

            Enums.ePowerState powerState;
            string? text;
            switch (MidsContext.Config.BuildMode)
            {
                case Enums.dmModes.Normal or Enums.dmModes.Respec when MidsContext.Config.BuildOption == Enums.dmItem.Slot:
                    powerState = Enums.ePowerState.Open;
                    text = @"Power / Slot";
                    break;
                case Enums.dmModes.Normal or Enums.dmModes.Respec:
                    powerState = Enums.ePowerState.Used;
                    text = @"Power Only";
                    break;
                case Enums.dmModes.LevelUp when DatabaseAPI.Database.Levels[MidsContext.Character.Level].LevelType() == Enums.dmItem.Power:
                    powerState = Enums.ePowerState.Used;
                    text = @"Power";
                    break;
                default:
                    {
                        var slotsLeft = MainModule.MidsController.Toon.SlotsRemaining;
                        var slotText = slotsLeft > 9 ? @"Slots" : @"Slot";
                        powerState = Enums.ePowerState.Open;
                        text = $"{slotsLeft} {slotText}";
                        break;
                    }
            }

            if (MainModule.MidsController.Toon.Complete)
            {
                if (MidsContext.Config is { BuildMode: Enums.dmModes.LevelUp })
                {
                    powerState = Enums.ePowerState.Used;
                }

                text = @"Complete";
            }

            if (dynMode.Lock && MidsContext.Config.BuildMode == Enums.dmModes.LevelUp)
            {
                dynMode.Lock = false;
            }
            else if (MidsContext.Config.BuildMode != Enums.dmModes.LevelUp)
            {
                dynMode.Lock = false;
            }

            switch (powerState)
            {
                case Enums.ePowerState.Used:
                    dynMode.ToggleText.ToggledOff = text;
                    dynMode.ToggleState = MidsVectorButton.States.ToggledOff;
                    break;
                case Enums.ePowerState.Open:
                    dynMode.ToggleText.ToggledOn = text;
                    dynMode.ToggleState = MidsVectorButton.States.ToggledOn;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!dynMode.Lock && MidsContext.Config.BuildMode == Enums.dmModes.LevelUp)
            {
                dynMode.Lock = true;
            }
        }

        private void SetColumns(int columns, Enums.eColumnStacking stackingMode = Enums.eColumnStacking.None)
        {
            if (columns == MidsContext.Config.Columns & stackingMode == MidsContext.Config.ColumnStackingMode)
            {
                return;
            }

            MidsContext.Config.Columns = columns;
            MidsContext.Config.ColumnStackingMode = stackingMode;

            if (drawing != null)
            {
                drawing.Columns = columns;
                drawing.ColumnStackingMode = stackingMode;
                drawing.GetPowersLayout();
                UpdateUiLayout(true);
                canvas.RequestFullRedraw();
                canvas.ResizeToContent();

                /*// Calculate the minimum width for the entire form.
                // This includes the drawing panel, the left-side panel, and the window borders.
                int drawingMinWidth = drawing.GetMinimumRequiredWidth();
                int nonDrawingWidth = leftLayoutPanel.Width + (Width - ClientSize.Width);
                int formMinimumWidth = drawingMinWidth + nonDrawingWidth;

                // Set the form's minimum size.
                MinimumSize = MinimumSize with { Width = formMinimumWidth };

                // If the form is currently smaller than the new minimum, resize it.
                if (Width < formMinimumWidth)
                {
                    Width = formMinimumWidth;
                }

                // 1. Recalculate the visual layout based on the panel's current width.
                drawing.UpdateLayout(canvas.ClientSize.Width);*/

            }
            // // 3. Force the panel to repaint itself with the new layout.
            // canvas.RequestFullRedraw();
            // canvas.ResizeToContent();
            // canvas.Invalidate();
        }

        private void UpdateUiControls(bool suppressDuringSetup = false, bool skipResize = false)
        {
            pvXEx.ToggleState = MidsContext.Config.Inc.DisablePvE switch
            {
                true => MidsVectorButton.States.ToggledOn,
                false => MidsVectorButton.States.ToggledOff
            };

            slotInfoEx.ToggleState = MidsContext.Config.ShowSlotsLeft switch
            {
                true => MidsVectorButton.States.ToggledOff,
                false => MidsVectorButton.States.ToggledOn
            };

            using (_events.SuppressIf(suppressDuringSetup))
            {
                Load_AtDropDown();
                atDropDown.SelectedItem = MidsContext.Character?.Archetype;

                LoadOrigins(atDropDown.SelectedItem);
                if (originDropDown.SelectedIndex != MidsContext.Character.Origin)
                {
                    if (MidsContext.Character.Origin < originDropDown.Items.Count)
                        originDropDown.SelectedIndex = MidsContext.Character.Origin;
                    else
                        originDropDown.SelectedIndex = 0;
                    AssetManager.SetOrigin(originDropDown.SelectedItem);
                }

                LoadPrimary(atDropDown.SelectedItem);
                primaryDropDown.SelectedIndex = AssignSetIndex(Enums.PowersetType.Primary, Enums.ePowerSetType.Primary);

                LoadSecondary(atDropDown.SelectedItem);
                secondaryDropDown.SelectedIndex = AssignSetIndex(Enums.PowersetType.Secondary, Enums.ePowerSetType.Secondary);

                LoadPools();
                pool0DropDown.SelectedIndex = MainModule.MidsController.Toon.PoolToDropDownIndex(0, MidsContext.Character.Powersets[3]?.nID ?? -1);
                pool1DropDown.SelectedIndex = MainModule.MidsController.Toon.PoolToDropDownIndex(1, MidsContext.Character.Powersets[4]?.nID ?? -1);
                pool2DropDown.SelectedIndex = MainModule.MidsController.Toon.PoolToDropDownIndex(2, MidsContext.Character.Powersets[5]?.nID ?? -1);
                pool3DropDown.SelectedIndex = MainModule.MidsController.Toon.PoolToDropDownIndex(3, MidsContext.Character.Powersets[6]?.nID ?? -1);

                LoadAncillary();
                var powersetIndexes = DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, Enums.ePowerSetType.Ancillary);
                if (MidsContext.Character.Powersets[7] != null)
                    ancillaryDropDown.SelectedIndex = DatabaseAPI.ToDisplayIndex(MidsContext.Character.Powersets[7], powersetIndexes);
                else if (powersetIndexes.Length > 0)
                    ancillaryDropDown.SelectedIndex = 0;
                else
                    ancillaryDropDown.SelectedIndex = -1;

                UpdatePowerLists();
                ProcessLocks();

                if (MidsContext.Config.BuildMode == Enums.dmModes.LevelUp)
                {
                    UpdateDmBuffer();
                }
            }
        }

        private void ProcessLocks()
        {
            primaryDropDown.IsLocked = MainModule.MidsController.Toon.Locked;
            secondaryDropDown.IsLocked = MidsContext.Character.Powersets[0].nIDLinkSecondary > -1;
            pool0DropDown.IsLocked = MainModule.MidsController.Toon.PoolLocked[0];
            pool1DropDown.IsLocked = MainModule.MidsController.Toon.PoolLocked[1];
            pool2DropDown.IsLocked = MainModule.MidsController.Toon.PoolLocked[2];
            pool3DropDown.IsLocked = MainModule.MidsController.Toon.PoolLocked[3];

            if (MainModule.MidsController.Toon.PoolLocked[4])
            {
                ancillaryDropDown.Lock();
            }
            else if (ancillaryDropDown.Items.Count is 0)
            {
                ancillaryDropDown.Lock("Not Available", true);
            }
            else
            {
                ancillaryDropDown.Unlock();
            }
        }

        private void UpdatePowerLists()
        {
            var ch = MidsContext.Character;
            bool needPrimaryPairRebuild = NeedRebuild(primaryList, ch?.Powersets[0]) ||
                                      NeedRebuild(secondaryList, ch?.Powersets[1]);

            if (needPrimaryPairRebuild)
            {
                UpdateOrAssemble(primaryList, ch?.Powersets[0], true);
                UpdateOrAssemble(secondaryList, ch?.Powersets[1], true);
            }
            else
            {
                RefreshItemStates(primaryList);
                RefreshItemStates(secondaryList);
            }

            bool needAncillaryRebuild = ch?.Powersets[7] != null || NeedRebuild(ancillaryList, ch?.Powersets[7]) ||
                                        needPrimaryPairRebuild;

            UpdateOrAssemble(ancillaryList, ch?.Powersets[7], needAncillaryRebuild);

            UpdateOrAssemble(pool0List, ch?.Powersets[3], forceRebuild: true);
            UpdateOrAssemble(pool1List, ch?.Powersets[4], forceRebuild: true);
            UpdateOrAssemble(pool2List, ch?.Powersets[5], forceRebuild: true);
            UpdateOrAssemble(pool3List, ch?.Powersets[6], forceRebuild: true);
        }

        private void UpdateOrAssemble(MidsListView list, IPowerset? powerset, bool forceRebuild)
        {
            // Null/empty powerset: show “Nothing” and bail
            if (powerset?.Powers is null || powerset.Powers.Length == 0)
            {
                list.Items.Clear();
                list.Invalidate();
                return;
            }

            // Rebuild if needed or forced
            if (forceRebuild || NeedRebuild(list, powerset))
            {
                var built = BuildPowerListItems(powerset); // List<PowerListViewItem>

                // Convert to List<MidsListViewItem> (List<T> is invariant)
                var items = (built.Count > 0)
                    ? built.Cast<MidsListViewItem>().ToList()
                    : [new PowerListViewItem("Nothing", MidsItemState.Disabled)];

                list.Items = items;

                // Mirror legacy behavior: refresh states after a rebuild too
                RefreshItemStates(list);
                return;
            }

            // Otherwise, just refresh the existing list in-place
            RefreshItemStates(list);
        }

        private bool NeedRebuild(MidsListView list, IPowerset? powerset)
        {
            if (powerset is null) return true;
            if (list.Items.Count == 0) return true;

            // Must check the actual last item, not “last with a set”
            if (list.Items[^1] is not PowerListViewItem last) return true;

            return last.NIdSet != powerset.nID;
        }

        private List<PowerListViewItem> BuildPowerListItems(IPowerset? powerset)
        {
            var result = new List<PowerListViewItem>();
            if (powerset is null) return result;

            bool pairedBold = MidsContext.Config?.RtFont.PairedBold ?? false;
            string message = string.Empty;

            var toon = MainModule.MidsController.Toon;

            if (powerset.nIDTrunkSet > -1)
            {
                var db = DatabaseAPI.Database;

                var trunkSet = db.Powersets[powerset.nIDTrunkSet];

                if (trunkSet != null)
                {
                    result.Add(new PowerListViewItem(trunkSet) { Bold = pairedBold });

                    var trunkPowers = trunkSet.Powers ?? [];
                    // If the trunk set has powers, add them first
                    for (var i = 0; i < trunkPowers.Length; i++)
                    {
                        var power = trunkPowers[i];
                        if (power is null) continue;
                        if (power.Level <= 0) continue;

                        var state = toon?.PowerState(power.PowerIndex, ref message) ?? MidsItemState.Enabled;

                        var item = new PowerListViewItem(power.DisplayName, state, nidSet: trunkSet.nIDTrunkSet, idxPower: i, nidPower: power.PowerIndex, tag: power, style: MidsItemFontStyles.Bold)
                        {
                            Bold = pairedBold,
                            Italic = state == MidsItemState.Invalid
                        };

                        result.Add(item);
                    }

                    result.Add(new PowerListViewItem(displayName: powerset.DisplayName, state: MidsItemState.Heading, nidSet: powerset.nID, idxPower: -1, nidPower: -1, tag: powerset, style: MidsItemFontStyles.Bold, alignment: MidsItemAlign.Center) { Bold = pairedBold });
                }
            }

            var powers = powerset.Powers ?? [];
            for (var i = 0; i < powers.Length; i++)
            {
                var power = powers[i];
                if (power is null) continue;
                if (power.Level <= 0 || !power.AllowedForClass(MidsContext.Character.Archetype.Idx)) continue;

                toon = MainModule.MidsController.Toon;
                var state = toon?.PowerState(power.PowerIndex, ref message) ?? MidsItemState.Enabled;

                var item = new PowerListViewItem(power.DisplayName, state, nidSet: powerset.nID, idxPower: i, nidPower: power.PowerIndex, tag: power, style: MidsItemFontStyles.Bold)
                {
                    Bold = pairedBold,
                    Italic = state == MidsItemState.Invalid
                };

                result.Add(item);
            }

            return result;
        }

        private void RefreshItemStates(MidsListView list)
        {
            bool pairedBold = MidsContext.Config?.RtFont.PairedBold ?? false;
            string message = string.Empty;
            var toon = MainModule.MidsController.Toon;

            if (list.Items.Count is 0)
            {
                list.Items = [new PowerListViewItem("Nothing", MidsItemState.Disabled)];
            }

            foreach (var baseItem in list.Items)
            {
                if (baseItem is not PowerListViewItem item) continue;

                if (item.NIdSet <= -1 || item.NIdPower <= -1) continue;

                var state = toon?.PowerState(item.NIdPower, ref message) ?? MidsItemState.Enabled;
                item.State = state;
                item.Bold = pairedBold;
                item.Italic = state == MidsItemState.Invalid;
            }

            list.Invalidate();
        }

        private int AssignSetIndex(Enums.PowersetType setId, Enums.ePowerSetType setType)
        {
            var powersetIndexes = DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, setType);
            return DatabaseAPI.ToDisplayIndex(MidsContext.Character.Powersets[(int)setId], powersetIndexes);
        }

        private void Load_AtDropDown()
        {
            var ats = DatabaseAPI.Database.Classes.Where(at => at is not null && at.Playable).ToList();

            atDropDown.DisplayMember = "DisplayName";
            atDropDown.DataSource = ats;

            atDropDown.IconProvider = item =>
            {
                int index = Array.IndexOf(DatabaseAPI.Database.Classes, item);
                if (index < 0) return null;

                AssetManager.Archetypes.TryGetValue(index, out var icon);
                return icon?.Bitmap;
            };
            atDropDown.RefreshIcons();
        }

        private void LoadOrigins(Archetype? selectedItem)
        {
            originDropDown.DataSource = selectedItem?.Origin;
            originDropDown.IconProvider = item =>
            {
                int index = Array.IndexOf(selectedItem.Origin, item);
                if (index < 0) return null;

                AssetManager.Origins.TryGetValue(index, out var icon);
                return icon?.Bitmap;
            };
            originDropDown.RefreshIcons();
        }

        private void LoadPrimary(Archetype? selectedItem)
        {
            var powerSets = selectedItem?.Primary
                .Select(index => DatabaseAPI.Database.Powersets[index]).ToList();

            primaryDropDown.DisplayMember = "DisplayName";
            primaryDropDown.DataSource = powerSets;
            primaryDropDown.IconProvider = item =>
            {
                int index = Array.IndexOf(DatabaseAPI.Database.Powersets, item);
                if (index < 0) return null;

                AssetManager.Powersets.TryGetValue(index, out var icon);
                return icon?.Bitmap;
            };
            primaryDropDown.RefreshIcons();
        }

        private void LoadSecondary(Archetype? selectedItem)
        {
            var powerSets = selectedItem?.Secondary
                .Select(index => DatabaseAPI.Database.Powersets[index]).ToList();

            secondaryDropDown.DisplayMember = "DisplayName";
            secondaryDropDown.DataSource = powerSets;
            secondaryDropDown.IconProvider = item =>
            {
                int index = Array.IndexOf(DatabaseAPI.Database.Powersets, item);
                if (index < 0) return null;

                AssetManager.Powersets.TryGetValue(index, out var icon);
                return icon?.Bitmap;
            };
            secondaryDropDown.RefreshIcons();
        }

        private void LoadPools()
        {
            var pools = this.GetAllControlsOfType<PowersetDropDownList>().Where(p => p.Name.Contains("pool", StringComparison.OrdinalIgnoreCase)).ToList();
            for (var poolIndex = 0; poolIndex < pools.Count; poolIndex++)
            {
                var poolDropDown = pools[poolIndex];
                var poolSets = DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, Enums.ePowerSetType.Pool).ToList();

                poolDropDown.DisplayMember = "DisplayName";
                poolDropDown.DataSource = poolSets;

                poolDropDown.IconProvider = item =>
                {
                    int index = Array.IndexOf(DatabaseAPI.Database.Powersets, item);
                    if (index < 0) return null;
                    AssetManager.Powersets.TryGetValue(index, out var icon);
                    return icon?.Bitmap;
                };
                poolDropDown.RefreshIcons();
            }
        }

        private void LoadAncillary()
        {
            var ancillaryPool = this.GetAllControlsOfType<PowersetDropDownList>().FirstOrDefault(lv => lv.Name.Contains("ancillary", StringComparison.OrdinalIgnoreCase));
            if (ancillaryPool == null) return;

            var ancillarySets = DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, Enums.ePowerSetType.Ancillary).ToList();
            ancillaryPool.DisplayMember = "DisplayName";
            ancillaryPool.DataSource = ancillarySets;

            ancillaryPool.IconProvider = item =>
            {
                int index = Array.IndexOf(DatabaseAPI.Database.Powersets, item);
                if (index < 0) return null;
                AssetManager.Powersets.TryGetValue(index, out var icon);
                return icon?.Bitmap;
            };
            ancillaryPool.RefreshIcons();
        }

        private void ChangeSets()
        {
            UpdateToon(
                MainModule.MidsController.Toon, 
                MidsContext.Character, 
                primaryDropDown.SelectedIndex, 
                secondaryDropDown.SelectedIndex, 
                pool0DropDown.SelectedIndex, 
                pool1DropDown.SelectedIndex, 
                pool2DropDown.SelectedIndex, 
                pool3DropDown.SelectedIndex, 
                ancillaryDropDown.SelectedIndex, 
                DatabaseAPI.GetPowersetIndexes,
                () => secondaryDropDown.IsLocked = false);
        }

        private void NewToon(bool reinit = true, bool skipDraw = false)
        {
            MainModule.MidsController.Toon ??= new Toon();

            if (reinit)
            {
                MidsContext.Character.Reset();
            }
            else
            {
                string str;
                if (!MainModule.MidsController.Toon.Locked)
                    str = MidsContext.Character.Name;
                else
                    str = string.Empty;
                MidsContext.Character.Reset(atDropDown.SelectedItem, originDropDown.SelectedIndex);
                if (MidsContext.Character.Powersets[0].nIDLinkSecondary > -1)
                {
                    MidsContext.Character.Powersets[1] =
                        DatabaseAPI.Database.Powersets[MidsContext.Character.Powersets[0].nIDLinkSecondary];
                }

                MidsContext.Character.Name = str;
            }

            if (fAccolade is { IsDisposed: false })
            {
                fAccolade.Dispose();
            }

            if (fTemp is { IsDisposed: false })
            {
                fTemp.Dispose();
            }

            if (fIncarnate is { IsDisposed: false })
            {
                fIncarnate.Dispose();
            }

            if (fPrestige is { IsDisposed: false })
            {
                fPrestige.Dispose();
            }

            if (FrmEntityDetails is { IsDisposed: false })
            {
                FrmEntityDetails.Dispose();
            }

            NewDraw(skipDraw);
            UpdateUiControls(true);

            MidsContext.EnhCheckMode = false;
            Info_Totals();
            FileModified = false;
            dataView?.SetData(null, null, true);
        }

        private void NewDraw(bool skipDraw = false)
        {
            if (drawing == null)
            {
                drawing = new BuildRenderer(canvas);
                canvas.Renderer = drawing;           // <-- make the panel own the renderer immediately
                canvas.ManageRendererOnSize = false;
                UpdateUiLayout(true); 
            }
            else
            {
                UpdateUiLayout(true);
            }

            if (skipDraw) return;

            drawing.Highlight = -1;
            canvas.RequestFullRedraw();
            canvas.ResizeToContent();
        }

        private void UpdateUiLayout(bool force = false)
        {
            if (IsDisposed || drawing is null) return;

            // Use ONE width for scale + layout to avoid jitter when scrollbars appear
            int widthForLayout = canvas.ClientSize.Width;
            if (widthForLayout <= 0) return;

            // Compute raw scale vs baseline
            float rawScale = (float)widthForLayout / BaselineCanvasWidth;

            // Tune how “eager” scaling feels: 0 = frozen, 1 = full raw scaling
            const float scalingIntensity = 0.5f;    // 50% dampening is a good default

            // Apply dampening and clamp to keep visuals readable
            float master = 1f + (rawScale - 1f) * scalingIntensity;
            master = Math.Clamp(master, 0.90f, 1.30f); // adjust if you want tighter/looser scaling

            // Avoid thrashing on tiny size changes
            if (!force &&
                Math.Abs(master - _lastMasterScale) < 0.01f &&
                widthForLayout == _lastCanvasWidth)
                return;

            _lastMasterScale = master;
            _lastCanvasWidth = widthForLayout;

            // --- First pass with current width ---
            drawing.MasterScale = master;
            drawing.UpdateFontScale(master);
            drawing.UpdateLayout(widthForLayout);
            drawing.ReInit(canvas);       // also FullRedraw inside ReInit
            canvas.ResizeToContent();     // may toggle vertical scrollbar -> width can change
            canvas.Invalidate();

            // --- If scrollbar visibility changed width, settle once more ---
            int widthAfter = canvas.ClientSize.Width;
            if (widthAfter != widthForLayout)
            {
                _lastCanvasWidth = widthAfter;

                rawScale = (float)widthAfter / BaselineCanvasWidth;
                master = 1f + (rawScale - 1f) * scalingIntensity;
                master = Math.Clamp(master, 0.90f, 1.30f);
                _lastMasterScale = master;

                drawing.MasterScale = master;
                drawing.UpdateFontScale(master);
                drawing.UpdateLayout(widthAfter);
                drawing.ReInit(canvas);
                canvas.ResizeToContent();
            }
        }

        private void GetBestDamageValues()
        {
            if (MainModule.MidsController.Toon == null)
            {
                return;
            }

            var highBase = 0.0f;
            for (var index = 0; index <= MidsContext.Character.Powersets[0].Powers.Length - 1; ++index)
            {
                var power = MidsContext.Character.Powersets[0].Powers[index];
                if (power.SkipMax)
                {
                    continue;
                }

                var damageValue = power.FXGetDamageValue();
                if (damageValue > (double)highBase)
                {
                    highBase = damageValue;
                }
            }


            var ps1 = MainModule.MidsController.Toon.PickDefaultSecondaryPowerset();
            foreach (var power in ps1.Powers)
            {
                if (power.SkipMax)
                {
                    continue;
                }

                var damageValue = power.FXGetDamageValue();
                if (damageValue > (double)highBase)
                {
                    highBase = damageValue;
                }
            }

            MainModule.MidsController.Toon.GenerateBuffedPowerArray();
            var highEnh = highBase * (1f + MidsContext.Character.TotalsCapped.BuffDam + Enhancement.ApplyED(Enums.eSchedule.A, 2.277f));
            if (MidsContext.Config.DamageMath.ReturnValue == ConfigData.EDamageReturn.DPS | MidsContext.Config.DamageMath.ReturnValue == ConfigData.EDamageReturn.DPA)
            {
                highEnh *= 1.5f;
            }

            dataView.infoDamageDisplay.HighestBaseValue = highBase;
            dataView.infoDamageDisplay.HighestEnhancedValue = highEnh;
        }

        private void PowerPicked(Enums.PowersetType setId, int nIdPower)
        {
            MainModule.MidsController.Toon.BuildPower(MidsContext.Character.Powersets[(int)setId].nID, nIdPower);
            PowerModified(true);
            //MidsContext.Config.Tips.Show(Tips.TipType.FirstPower);
            canvas.RequestFullRedraw();
            //canvas.ResizeToContent();
            //canvas.Invalidate();
        }

        private void PowerPicked(int nIdPowerset, int nIdPower)
        {
            MainModule.MidsController.Toon.BuildPower(nIdPowerset, nIdPower);
            PowerModified(true);
            //MidsContext.Config.Tips.Show(Tips.TipType.FirstPower);
            canvas.RequestFullRedraw();
            //canvas.ResizeToContent();
            //canvas.Invalidate();
        }

        private void Info_Enhancement(I9Slot? iEnh, int iLevel = -1)
        {
            dataView.SetEnhancement(iEnh, iLevel);
        }

        private void Info_Power(int powerIdx, int iEnhLvl = -1, bool noLevel = false, bool @lock = false)
        {
            if (dataView is null) return;

            if (!@lock & dataView.IsLocked)
            {
                if (dvLastPower != powerIdx)
                {
                    return;
                }

                @lock = true;
            }

            dvLastEnh = iEnhLvl;
            dvLastPower = powerIdx;
            dvLastNoLev = noLevel;
            //fData?.UpdateData(dvLastPower);
            var powIndex = -1;
            if (MainModule.MidsController.Toon.Locked)
            {
                for (var index = 0; index < MidsContext.Character.CurrentBuild.Powers.Count; index++)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[index] == null)
                    {
                        continue;
                    }

                    if (MidsContext.Character.CurrentBuild.Powers[index].NIDPower != powerIdx)
                    {
                        continue;
                    }

                    powIndex = index;
                    break;
                }
            }

            dataView.IsLocked = @lock;
            if (powIndex > -1)
            {
                var basePower = MainModule.MidsController.Toon.GetBasePower(powIndex);
                var enhancedPower = MainModule.MidsController.Toon.GetEnhancedPower(powIndex);
                if (basePower != null && enhancedPower != null)
                {
                    dataView.SetData(basePower, enhancedPower, noLevel, dataView.IsLocked, powIndex);
                }
                else
                {
                    dataView.SetData(MainModule.MidsController.Toon.GetBasePower(powIndex, powerIdx), null, noLevel, dataView.IsLocked, powIndex);
                }
            }
            else
            {
                dataView.SetData(MainModule.MidsController.Toon.GetBasePower(powIndex, powerIdx), null, noLevel, dataView.IsLocked, powIndex);
            }

            //FloatingDataForm.Activate();
        }

        private void Info_Totals()
        {
            if ((MainModule.MidsController.Toon == null) | !MainModule.MidsController.IsAppInitialized)
            {
                return;
            }

            MainModule.MidsController.Toon?.GenerateBuffedPowerArray();
            dataView.DisplayTotals();
            FloatUpdate();
        }

        private void RefreshTabs(int iPower, I9Slot? iEnh, int iLevel = -1)
        {
            if (iEnh.Enh > -1)
            {
                Info_Power(iPower, iLevel);
                Info_Enhancement(iEnh, iLevel);
            }
            else
            {
                Info_Power(iPower, iLevel, true);
            }
        }

        private void ShowPopup(int nIdPowerset, int nIdClass, Rectangle rBounds, string extraString = "", VerticalAlignment vAlign = VerticalAlignment.Top)
        {
            if (MidsContext.Config.DisableShowPopup) { HidePopup(); return; }

            if (vAlign == VerticalAlignment.Center) vAlign = VerticalAlignment.Bottom;
            if (nIdPowerset < 0 && nIdClass < 0) return;

            // Prepare content
            PopUp.PopupData data =
                nIdPowerset < 0
                    ? MidsContext.Character.Archetype.PopInfo()
                    : MainModule.MidsController.Toon.PopPowersetInfo(nIdPowerset, extraString);

            if (data.Sections == null) { HidePopup(); return; }

            // Update popup content
            _popupHost.SetPopup(data);

            // Bias: if caller asked "Bottom", we prefer below; otherwise prefer above (legacy parity).
            var bias = vAlign == VerticalAlignment.Bottom
                ? MidsPopupDisplay.PlacementBias.PreferBelow
                : MidsPopupDisplay.PlacementBias.PreferAbove;

            // Show: rBounds is in CANVAS client coords – constrain to the FORM (or change to 'canvas' to constrain inside canvas)
            _popupHost?.TryShowAt(rBounds, bias, margin: 8);

            // Legacy indices
            _popupHost.HIdx = -1;
            _popupHost.EIdx = -1;
            _popupHost.PIdx = -1;
            _popupHost.PsIdx = (nIdPowerset < 0 ? nIdClass : nIdPowerset);
        }

        private void ShowPopup(int hIdx, int pIdx, int sIdx, Point e, Rectangle rBounds, I9Slot? eSlot = null, int setIdx = -1, VerticalAlignment vAlign = VerticalAlignment.Bottom, I9Picker.EnhUniqueStatus? enhUniqueStatus = null)
        {
            if (MidsContext.Config.DisableShowPopup) { HidePopup(); return; }

            bool flag = false;
            bool picker = false;
            bool powerListing = false;

            // Normalize indices (legacy)
            if (hIdx < 0 && pIdx > -1)
                hIdx = MidsContext.Character.CurrentBuild.FindInToonHistory(pIdx);

            PowerEntry? powerEntry = (hIdx > -1) ? MidsContext.Character.CurrentBuild.Powers[hIdx] : null;

            // Short-circuit if content scope unchanged
            if (!(_popupHost.HIdx != hIdx || _popupHost.EIdx != sIdx || _popupHost.PIdx != pIdx || _popupHost.HIdx == -1 || _popupHost.EIdx == -1 || _popupHost.PIdx == -1))
            {
                return;
            }

            // Choose data + anchor rectangle (in CANVAS client coords)
            Rectangle anchorRect = rBounds;
            PopUp.PopupData data = default;

            if (hIdx > -1 && sIdx < 0 && pIdx < 0 && eSlot == null && setIdx < 0)
            {
                // hovering a power button area (rBounds from caller)
                if (canvas.IsWithinBounds(e, rBounds) && powerEntry?.NIDPower > -1)
                {
                    data = MainModule.MidsController.Toon.PopPowerInfo(hIdx, powerEntry.NIDPower);
                    flag = true;
                }
            }
            else if (sIdx > -1)
            {
                // enhancement slot inside a power – you may have your own method for this rect
                anchorRect = rBounds;

                if (powerEntry != null)
                    data = Character.PopEnhInfo(powerEntry.Slots[sIdx].Enhancement, powerEntry.Slots[sIdx].Level, powerEntry);
                flag = true;
            }
            else if (pIdx > -1)
            {
                // power listing item
                data = MainModule.MidsController.Toon.PopPowerInfo(hIdx, pIdx);
                flag = true; powerListing = true;
            }
            else if (eSlot != null && setIdx < 0)
            {
                // picker: enhancement slot from picker UI
                data = Character.PopEnhInfo(eSlot, -1, powerEntry);
                flag = true; picker = true;
            }
            else if (setIdx > -1)
            {
                // picker: set info
                data = Character.PopSetInfo(setIdx, powerEntry);
                flag = true; picker = true;
            }

            if (!(flag && data.Sections != null)) { HidePopup(); return; }

            
            // Update content + unique status
            _popupHost.SetPopup(data, enhUniqueStatus);

            // Bias: Bottom → prefer below; otherwise prefer above (legacy feel).
            var bias = vAlign == VerticalAlignment.Bottom
                ? MidsPopupDisplay.PlacementBias.PreferBelow
                : MidsPopupDisplay.PlacementBias.PreferAbove;

            if (sIdx > -1 || picker || powerListing) bias = MidsPopupDisplay.PlacementBias.PreferRight;

            // Show at anchor (client coords). Constrain to FORM so it auto-flips above when near bottom.
            Rectangle anchorFormRect =
                (!picker && !powerListing)
                    ? ToFormClientRect(canvas, anchorRect) // from control → form
                    : anchorRect;                           // already in form coords

            _popupHost.TryShowAt(anchorFormRect, bias, margin: 8);

            // Legacy indices
            _popupHost.HIdx = hIdx;
            _popupHost.EIdx = sIdx;
            _popupHost.PIdx = pIdx;
            _popupHost.PsIdx = -1;
        }

        private void HidePopup()
        {
            if (_popupHost is { IsOpen: false }) return;
            _popupHost?.HidePopup();  // hides + resets HIdx/EIdx/PIdx/PsIdx
        }

        private Rectangle ToFormClientRect(Control from, Rectangle rectInFrom)
        {
            var screenPt = from.PointToScreen(rectInFrom.Location);
            var formPt = PointToClient(screenPt);
            return new Rectangle(formPt, rectInFrom.Size);
        }

        private void FloatUpdate(bool newData = false)
        {
            fSets?.UpdateData();
            fGraphStats?.UpdateData(newData);
            fTotals?.UpdateData();
            fTotals2?.UpdateData();
            fGraphCompare?.UpdateData();
            fRecipe?.UpdateData();
            fRotationHelper?.UpdateData();
            fData?.UpdateData(dvLastPower);
            fRotationHelper?.UpdateData();
        }

        private void SlotLevelSwap(int sourcePower, int sourceSlot, int destPower, int destSlot)
        {
            var index = 0;
            do
            {
                dragdropScenarioAction[index] = MidsContext.Config.DragDropScenarioAction[index];
                ++index;
            } while (index <= 19);

            if ((MidsContext.Character.CurrentBuild.Powers[sourcePower].Slots[sourceSlot].Level <
                 MidsContext.Character.CurrentBuild.Powers[destPower].Level) & !DatabaseAPI.Database
                    .Power[MidsContext.Character.CurrentBuild.Powers[destPower].NIDPower].AllowFrontLoading)
            {
                CheckInitDdsaValue(13, 0, "Slot being level-swapped is too low for the destination power",
                    "Allow swap anyway (mark as invalid)");
                if (dragdropScenarioAction[13] == 1)
                {
                    return;
                }
            }

            if ((MidsContext.Character.CurrentBuild.Powers[destPower].Slots[destSlot].Level <
                 MidsContext.Character.CurrentBuild.Powers[sourcePower].Level) & !DatabaseAPI.Database
                    .Power[MidsContext.Character.CurrentBuild.Powers[sourcePower].NIDPower].AllowFrontLoading)
            {
                CheckInitDdsaValue(14, 0, "Slot being level-swapped is too low for the source power",
                    "Allow swap anyway (mark as invalid)");
                if (dragdropScenarioAction[14] == 1)
                {
                    return;
                }
            }

            var level = MidsContext.Character.CurrentBuild.Powers[sourcePower].Slots[sourceSlot].Level;
            MidsContext.Character.CurrentBuild.Powers[sourcePower].Slots[sourceSlot].Level =
                MidsContext.Character.CurrentBuild.Powers[destPower].Slots[destSlot].Level;
            MidsContext.Character.CurrentBuild.Powers[destPower].Slots[destSlot].Level = level;
            PowerModified(true);
            DoRedraw();
        }

        private bool? CheckInitDdsaValue(int index, int? defaultOpt, string description, params string[] options)
        {
            if (dragdropScenarioAction[index] != 0)
            {
                return null;
            }

            var (result, remember) = frmOptionListDlg.ShowWithOptions(true, defaultOpt ?? 1, description, options);
            dragdropScenarioAction[index] = (short)result;
            if (remember != true)
            {
                return remember;
            }

            MidsContext.Config.DragDropScenarioAction[index] = dragdropScenarioAction[index];

            return remember;
        }

        private int PowerMove(PowerEntry?[] tp, int start, int finish)
        {
            if (tp[start].NIDPower != -1 && DatabaseAPI.Database.Power[tp[start].NIDPower].Level - 1 > tp[finish].Level)
            {
                if (dragdropScenarioAction[0] == 0)
                {
                    var canOverride = DatabaseAPI.Database.Power[tp[start].NIDPower].Level - 1 == tp[start].Level;
                    var (result, remember) = canOverride
                        ? frmOptionListDlg.ShowWithOptions(true, 0, "Power is moved or swapped too low",
                            "Allow power to be moved anyway (mark as invalid)")
                        : frmOptionListDlg.ShowWithOptions(true, 1, "Power is moved or swapped too low",
                            "Move/swap power to its lowest possible level",
                            "Allow power to be moved anyway (mark as invalid)");
                    dragdropScenarioAction[0] = (short)result;
                    if (canOverride)
                    {
                        if (dragdropScenarioAction[0] == 2)
                        {
                            dragdropScenarioAction[0] = 3;
                        }
                    }

                    if (remember == true)
                    {
                        MidsContext.Config.DragDropScenarioAction[0] = dragdropScenarioAction[0];
                    }
                }

                if (dragdropScenarioAction[0] == 1)
                {
                    return 0;
                }

                if (dragdropScenarioAction[0] == 2)
                {
                    if (DatabaseAPI.Database.Power[tp[start].NIDPower].Level - 1 == tp[start].Level)
                    {
                        MessageBox.Show(
                            @"You have chosen to always swap a power with its minimum level when attempting to move it too low, but the power you are trying to swap is already at its minimum level. Visit the Drag & Drop tab of the configuration window to change this setting.",
                            null, MessageBoxButtons.OK);
                        return 0;
                    }

                    var lvl = DatabaseAPI.Database.Power[tp[start].NIDPower].Level - 1;
                    var index = 0;
                    while (tp[index].Level != lvl)
                    {
                        ++index;
                        if (index > 23)
                        {
                            return PowerMove(tp, start, lvl);
                        }
                    }
                }
            }

            var flag1 = start < finish;
            var flagArray = new bool[tp.Length - 1 + 1];
            if (flag1)
            {
                flagArray[start] = true;
                var level = tp[start].Level;
                var num = finish;
                for (var index = start + 1; index <= num; ++index)
                    if (tp[index].NIDPower < 0)
                    {
                        flagArray[index] = true;
                        level = tp[index].Level;
                    }
                    else if (DatabaseAPI.Database.Power[tp[index].NIDPower].Level - 1 == tp[index].Level)
                    {
                        flagArray[index] = false;
                    }
                    else if (level >= DatabaseAPI.Database.Power[tp[index].NIDPower].Level - 1)
                    {
                        flagArray[index] = true;
                        level = tp[index].Level;
                    }
                    else
                    {
                        flagArray[index] = false;
                    }
            }

            if (flag1 & !flagArray[finish])
            {
                CheckInitDdsaValue(1, null, "Power is moved too high (some powers will no longer fit)",
                    "Move to the last power slot that can be shifted");
                if (dragdropScenarioAction[1] == 1)
                {
                    return 0;
                }

                if (dragdropScenarioAction[1] == 2)
                {
                    var num1 = start + 1;
                    int index;
                    for (index = finish; index >= num1; index += -1)
                    {
                        if (!flagArray[index])
                        {
                            continue;
                        }

                        finish = index;
                        break;
                    }

                    if (finish != index)
                    {
                        MessageBox.Show(@"None of the powers can be shifted, so the power was not moved.", null,
                            MessageBoxButtons.OK);
                        return 0;
                    }
                }
            }

            var powerEntry =
                tp[start].NIDPower != -1
                    ? new PowerEntry(DatabaseAPI.Database.Power[tp[start].NIDPower])
                    : new PowerEntry();
            powerEntry.Slots = (SlotEntry[])tp[start].Slots.Clone();
            powerEntry.Level = tp[start].Level;
            ClearPower(tp, start);
            var flag2 = false;
            int num3;
            int num4;
            int num5;
            if (flag1)
            {
                num3 = finish;
                num4 = start + 1;
                num5 = -1;
            }
            else
            {
                num3 = start + 1;
                num4 = finish;
                num5 = 1;
            }

            var num6 = num4;
            var num7 = num5;
            for (var index = num3; ((num7 >> 31) ^ index) <= ((num7 >> 31) ^ num6); index += num7)
            {
                if (tp[index].NIDPower != -1 && flag1 && !flagArray[index])
                {
                    CheckInitDdsaValue(7, null, "Power being shifted down cannot shift to the necessary level",
                        "Shift other powers around it",
                        "Overwrite it; leave previous power slot empty", "Allow anyway (mark as invalid)");
                    if (dragdropScenarioAction[7] == 1)
                    {
                        return 0;
                    }

                    if (dragdropScenarioAction[7] == 3)
                    {
                        if (!flag2)
                        {
                            start = index;
                        }

                        break;
                    }
                }

                if (!(!flag2 & (tp[index].NIDPower < 0)))
                {
                    continue;
                }

                CheckInitDdsaValue(10, null, "There is a gap in a group of powers that are being shifted",
                    "Fill empty slot; don't move powers unnecessarily", "Shift empty slot as if it were a power");
                if (dragdropScenarioAction[10] == 1)
                {
                    return 0;
                }

                if (dragdropScenarioAction[10] == 2)
                {
                    if (tp[finish].NIDPower < 0)
                    {
                        powerEntry.Level = tp[start].Level;
                        tp[start] = powerEntry;
                        return PowerSwap(1, ref tp, start, finish) == 0 ? 0 : -1;
                    }

                    start = index;
                }

                flag2 = true;
            }

            var index1 = start;
            var num8 = !flag1 ? index1 - 1 : index1 + 1;
            while (num8 != finish)
                switch (PowerSwap(2, ref tp, index1, num8))
                {
                    case -1:
                        index1 = num8;
                        if (flag1)
                        {
                            ++num8;
                            break;
                        }

                        --num8;
                        break;
                    case 0:
                        MessageBox.Show(
                            @"Move canceled by user. If you didn't click Cancel, check that none of your Shift options are set to Cancel by default.",
                            null, MessageBoxButtons.OK);
                        return 0;
                    case 1:
                        if (flag1)
                        {
                            ++num8;
                            break;
                        }

                        --num8;
                        break;
                    case 2:
                        PowerMoveByUser(dragStartPower, dragFinishPower);
                        return 0;
                }

            powerEntry.Level = tp[index1].Level;
            tp[index1] = powerEntry;
            switch (PowerSwap(1, ref tp, index1, num8))
            {
                case 0:
                    return 0;
                case 3:
                    PowerSwapByUser(dragStartPower, dragFinishPower);
                    return 0;
                default:
                    return -1;
            }
        }

        private void PowerMoveByUser(int dragStart, int dragFinish)
        {
            if (dragStart < 0 || dragStart > 23 || dragFinish < 0 || dragFinish > 23 || dragStart == dragFinish)
            {
                return;
            }

            var index = 0;
            do
            {
                dragdropScenarioAction[index] = MidsContext.Config.DragDropScenarioAction[index];
                ++index;
            } while (index <= 19);

            var powerEntryArray = DeepCopyPowerList();
            if (PowerMove(powerEntryArray, dragStart, dragFinish) == 0)
            {
                return;
            }

            ShallowCopyPowerList(powerEntryArray);
            PowerModified(true);
            DoRedraw();
        }

        private int PowerSwap(int mode, ref PowerEntry?[] tp, int start, int finish)
        {
            int num1;
            if (start < 0 || start > 23 || finish < 0 || finish > 23 || start == finish)
            {
                return 0;
            }

            if (tp[start].NIDPower == -1 ||
                DatabaseAPI.Database.Power[tp[start].NIDPower].Level - 1 <= tp[finish].Level)
            {
                if (tp[finish].NIDPower != -1 &&
                    DatabaseAPI.Database.Power[tp[finish].NIDPower].Level - 1 > tp[start].Level)
                {
                    switch (mode)
                    {
                        case 0:
                            CheckInitDdsaValue(4, null, "Power being replaced is swapped too low",
                                "Overwrite rather than swap",
                                "Allow power to be swapped anyway (mark as invalid)");
                            if (dragdropScenarioAction[4] == 1)
                            {
                                return 0;
                            }

                            if (dragdropScenarioAction[4] == 2)
                            {
                                tp[finish].NIDPower = -1;
                                tp[finish].NIDPowerset = -1;
                                tp[finish].IDXPower = -1;
                                tp[finish].StatInclude = false;
                                tp[finish].ProcInclude = false;
                                tp[finish].VariableValue = 0;
                                tp[finish].Slots = new SlotEntry[0];
                            }

                            break;
                        case 2:
                            if (dragdropScenarioAction[7] == 2)
                            {
                                return 1;
                            }

                            break;
                    }
                }
            }
            else if (mode == 0)
            {
                if (dragdropScenarioAction[0] == 0)
                {
                    if (DatabaseAPI.Database.Power[tp[start].NIDPower].Level - 1 == tp[start].Level)
                    {
                        var remember = CheckInitDdsaValue(0, null, "Power is moved or swapped too low",
                            "Allow power to be moved anyway (mark as invalid)");
                        if (dragdropScenarioAction[0] == 2)
                        {
                            dragdropScenarioAction[0] = 3;
                            if (remember == true)
                            {
                                MidsContext.Config.DragDropScenarioAction[0] = dragdropScenarioAction[0];
                            }
                        }
                    }
                    else
                    {
                        CheckInitDdsaValue(0, 0, "Power is moved or swapped too low",
                            "Move/swap power to its lowest possible level",
                            "Allow power to be moved anyway (mark as invalid)");
                    }
                }

                if (dragdropScenarioAction[0] == 1)
                {
                    return 0;
                }

                if (dragdropScenarioAction[0] == 2)
                {
                    if (DatabaseAPI.Database.Power[tp[start].NIDPower].Level - 1 == tp[start].Level)
                    {
                        MessageBox.Show(
                            @"You have chosen to always swap a power with its minimum level when attempting to swap it too low, but the power you are trying to swap is already at its minimum level. Visit the Drag & Drop tab of the configuration window to change this setting.",
                            null, MessageBoxButtons.OK);
                        return 0;
                    }

                    var lvl = DatabaseAPI.Database.Power[tp[start].NIDPower].Level - 1;
                    var index = 0;
                    while (tp[index].Level != lvl)
                    {
                        ++index;
                        if (index > 23)
                        {
                            return PowerSwap(mode, ref tp, start, lvl);
                        }
                    }

                    var num4 = index;
                    return PowerSwap(mode, ref tp, start, num4);
                }
            }

            if (mode == 1 || mode == 2 && tp[finish].NIDPower != -1 &&
                DatabaseAPI.Database.Power[tp[finish].NIDPower].Level - 1 == tp[finish].Level)
            {
                switch (mode)
                {
                    case 1:
                        {
                            CheckInitDdsaValue(12, null,
                                "The power in the destination slot is prevented from being shifted up",
                                "Unlock and shift all level-locked powers",
                                "Shift destination power to the first valid and empty slot",
                                "Swap instead of move");
                            if (dragdropScenarioAction[12] == 1)
                            {
                                return 0;
                            }

                            if (dragdropScenarioAction[12] == 2)
                            {
                                dragdropScenarioAction[11] = 2;
                                return 2;
                            }

                            if (dragdropScenarioAction[12] != 3 && dragdropScenarioAction[12] == 4)
                            {
                                return 3;
                            }

                            break;
                        }
                    case 2:
                        {
                            CheckInitDdsaValue(11, null, "A power placed at its minimum level is being shifted up",
                                "Shift it along with the other powers", "Shift other powers around it");
                            if (dragdropScenarioAction[11] == 1)
                            {
                                return 0;
                            }

                            if (dragdropScenarioAction[11] != 2 && dragdropScenarioAction[11] == 3)
                            {
                                return 1;
                            }

                            break;
                        }
                }
            }

            var num5 = tp[22].SlotCount + tp[23].SlotCount;
            var num6 = -1;
            if (start == 22 && finish < 22 && num5 <= 8 && tp[finish].SlotCount + tp[23].SlotCount > 8 ||
                start == 23 && finish < 22 && tp[start].SlotCount <= 4 && tp[finish].SlotCount > 4 ||
                start == 23 && finish < 22 && num5 <= 8 && tp[22].SlotCount + tp[finish].SlotCount > 8 ||
                start == 23 && finish == 22 && tp[finish].SlotCount > 4)
            {
                if (mode < 2)
                {
                    CheckInitDdsaValue(6, null, "Power being replaced is swapped too high to have # slots",
                        "Remove impossible slots",
                        "Allow anyway (Mark slots as invalid)");
                }

                num6 = 6;
            }
            else if ((start < 22) & (finish == 22) & (num5 <= 8) & (tp[start].SlotCount + tp[23].SlotCount > 8) ||
                     (start < 22) & (finish == 23) & (tp[finish].SlotCount <= 4) & (tp[start].SlotCount > 4) ||
                     (start < 22) & (finish == 23) & (num5 <= 8) & (tp[22].SlotCount + tp[start].SlotCount > 8) ||
                     (start == 22) & (finish == 23) & (tp[start].SlotCount > 4))
            {
                if (mode < 2)
                {
                    CheckInitDdsaValue(3, null, "Power is moved or swapped too high to have # slots",
                        "Remove impossible slots",
                        "Allow anyway (Mark slots as invalid)");
                }

                num6 = 3;
            }

            if (num6 != -1 && mode == 2)
            {
                CheckInitDdsaValue(9, null, "Power being shifted up has impossible # of slots",
                    "Remove impossible slots",
                    "Allow anyway (Mark slots as invalid)");
                num6 = 9;
            }

            if (((num6 != 6 ? 0 : mode < 2 ? 1 : 0) & (dragdropScenarioAction[6] == 1 ? 1 : 0)) != 0 ||
                ((num6 != 3 ? 0 : mode < 2 ? 1 : 0) & (dragdropScenarioAction[3] == 1 ? 1 : 0)) != 0 ||
                num6 == 9 && dragdropScenarioAction[9] == 1)
            {
                num1 = 0;
            }
            else
            {
                if (((num6 != 6 ? 0 : mode < 2 ? 1 : 0) & (dragdropScenarioAction[6] == 2 ? 1 : 0)) != 0 ||
                    ((num6 != 3 ? 0 : mode < 2 ? 1 : 0) & (dragdropScenarioAction[3] == 2 ? 1 : 0)) != 0 ||
                    num6 == 9 && dragdropScenarioAction[9] == 2)
                {
                    int index;
                    int num2;
                    if (start > finish)
                    {
                        index = finish;
                        num2 = start;
                    }
                    else
                    {
                        index = start;
                        num2 = finish;
                    }

                    //int integer1 = Convert.ToInt32(Interaction.IIf(num2 == 22, index, RuntimeHelpers.GetObjectValue(Interaction.IIf(index == 22, num2, 22))));
                    var integer1 = num2 == 22 ? index :
                        index == 22 ? num2 : 22;
                    var integer2 = num2 == 23 ? index : 23;
                    while (tp[integer1].SlotCount + tp[integer2].SlotCount > 8 ||
                           tp[index].SlotCount > 4 && integer2 != 23)
                        tp[index].Slots =
                            tp[index].Slots
                                .RemoveLast(); // (SlotEntry[])Utils.CopyArray(tp[index].Slots, (Array)new SlotEntry[tp[index].SlotCount - 2 + 1]);
                }
                else if (((num6 != 6 ? 0 : mode < 2 ? 1 : 0) & (dragdropScenarioAction[6] == (short)3 ? 1 : 0)) != 0 ||
                         ((num6 != 3 ? 0 : mode < 2 ? 1 : 0) & (dragdropScenarioAction[3] == (short)3 ? 1 : 0)) != 0 ||
                         num6 == 9 && dragdropScenarioAction[9] == 3)
                {
                    var index1 = start <= finish ? start : finish;
                    if ((start == 23) | (finish == 23))
                    {
                        for (var index2 = tp[index1].SlotCount - 1; index2 >= 1; index2 += -1)
                            if ((index2 + tp[22].SlotCount > 7) | (index2 > 3))
                            {
                                tp[index1].Slots[index2].Level = 50;
                            }
                    }
                    else
                    {
                        for (var index2 = tp[index1].SlotCount - 1; index2 >= 1; index2 += -1)
                            if (index2 + tp[22].SlotCount > 7)
                            {
                                tp[index1].Slots[index2].Level = 50;
                            }
                    }
                }

                var powerEntry = tp[start];
                tp[start] = tp[finish];
                tp[finish] = powerEntry;
                var level1 = tp[start].Level;
                tp[start].Level = tp[finish].Level;
                tp[finish].Level = level1;
                // swapping start and finish values
                var tmpSwap = start;
                start = finish;
                finish = tmpSwap;
                var index3 = 0;
                do
                {
                    if (tp[index3 == 0 ? start : finish].SlotCount > 0)
                    {
                        tp[index3 == 0 ? start : finish].Slots[0].Level = tp[index3 == 0 ? start : finish].Level;
                        var num2 = tp[index3 == 0 ? start : finish].SlotCount - 1;
                        var slotIDX = 1;
                        while (true)
                            if (slotIDX <= num2 && slotIDX <= tp[index3 == 0 ? start : finish].SlotCount - 1)
                            {
                                if (tp[index3 == 0 ? start : finish].Slots[slotIDX].Level <
                                    tp[index3 == 0 ? start : finish].Level)
                                {
                                    if ((mode < 2) & (index3 == 0) & (dragdropScenarioAction[2] == 0))
                                    {
                                        CheckInitDdsaValue(2, 3, "Power is moved or swapped higher than slots' levels",
                                            "Remove slots",
                                            "Mark invalid slots", "Swap slot levels if valid; remove invalid ones",
                                            "Swap slot levels if valid; mark invalid ones",
                                            "Rearrange all slots in build");
                                    }
                                    else if ((mode == 0) & (index3 == 1) & (dragdropScenarioAction[5] == 0))
                                    {
                                        CheckInitDdsaValue(5, 3,
                                            "Power being replaced is swapped higher than slots' levels", "Remove slots",
                                            "Mark invalid slots", "Swap slot levels if valid; remove invalid ones",
                                            "Swap slot levels if valid; mark invalid ones",
                                            "Rearrange all slots in build");
                                    }
                                    else if ((mode == 2) & (dragdropScenarioAction[8] == 0))
                                    {
                                        CheckInitDdsaValue(8, 3, "Power being shifted up has slots from lower levels",
                                            "Remove slots",
                                            "Mark invalid slots", "Swap slot levels if valid; remove invalid ones",
                                            "Swap slot levels if valid; mark invalid ones",
                                            "Rearrange all slots in build");
                                    }

                                    if (!((mode < 2) & (index3 == 0) & (dragdropScenarioAction[2] == 1) ||
                                          (mode == 0) & (index3 == 1) & (dragdropScenarioAction[5] == 1) ||
                                          (mode == 2) & (dragdropScenarioAction[8] == 1)))
                                    {
                                        var value = 1 - index3 == 0 ? start : finish;
                                        if ((mode < 2) & (index3 == 0) & (dragdropScenarioAction[2] == 2) ||
                                            (mode == 0) & (index3 == 1) & (dragdropScenarioAction[5] == 2) ||
                                            (mode == 2) & (dragdropScenarioAction[8] == 2))
                                        {
                                            RemoveSlotFromTempList(tp[index3 == 0 ? start : finish], slotIDX);
                                            --slotIDX;
                                        }
                                        else if ((mode < 2) & (index3 == 0) & (dragdropScenarioAction[2] == 4) ||
                                                 (mode == 0) & (index3 == 1) & (dragdropScenarioAction[5] == 4) ||
                                                 (mode == 2) & (dragdropScenarioAction[8] == 4))
                                        {
                                            if (tp[value].SlotCount > slotIDX)
                                            {
                                                var level2 = tp[value].Slots[slotIDX].Level;
                                                tp[value].Slots[slotIDX].Level = tp[index3 == 0 ? start : finish]
                                                    .Slots[slotIDX].Level;
                                                tp[index3 == 0 ? start : finish].Slots[slotIDX].Level = level2;
                                            }
                                            else
                                            {
                                                RemoveSlotFromTempList(tp[index3 == 0 ? start : finish], slotIDX);
                                                --slotIDX;
                                            }
                                        }
                                        else if ((mode < 2) & (index3 == 0) & (dragdropScenarioAction[2] == 5) ||
                                                 (mode == 0) & (index3 == 1) & (dragdropScenarioAction[5] == 5) ||
                                                 (mode == 2) & (dragdropScenarioAction[8] == 5))
                                        {
                                            if (tp[value].SlotCount > slotIDX)
                                            {
                                                var level2 = tp[value].Slots[slotIDX].Level;
                                                tp[value].Slots[slotIDX].Level = tp[index3 == 0 ? start : finish]
                                                    .Slots[slotIDX].Level;
                                                tp[index3 == 0 ? start : finish].Slots[slotIDX].Level = level2;
                                            }
                                        }
                                        else if ((mode < 2) & (index3 == 0) & (dragdropScenarioAction[2] == 6) ||
                                                 (mode == 0) & (index3 == 1) & (dragdropScenarioAction[5] == 6) ||
                                                 (mode == 2) & (dragdropScenarioAction[8] == 6))
                                        {
                                            RearrangeAllSlotsInBuild(tp, true);
                                        }
                                    }
                                    else
                                    {
                                        return 0;
                                    }
                                }

                                ++slotIDX;
                            }
                            else
                            {
                                break;
                            }
                    }

                    ++index3;
                } while (index3 <= 1);

                num1 = -1;
            }

            return num1;
        }

        private void RearrangeAllSlotsInBuild(PowerEntry?[] tp, bool notifyUser = false)
        {
            var index1 = 0;
            var numArray1 = new int[tp.Length];
            for (var index2 = 0; index2 < tp.Length; index2++)
            {
                if (tp[index2] == null || tp[index2].NIDPower == -1 || !DatabaseAPI.Database.Power[tp[index2].NIDPower].AllowFrontLoading)
                {
                    continue;
                }

                numArray1[index1] = index2;
                index1++;
            }

            var index3 = index1;
            for (var index2 = 0; index2 < tp.Length; index2++)
            {
                if (tp[index2] == null)
                {
                    continue;
                }

                if (((tp[index2].NIDPower == -1 ? 0 :
                         !DatabaseAPI.Database.Power[tp[index2].NIDPower].AllowFrontLoading ? 1 : 0) |
                     (tp[index2].NIDPower == -1 ? 1 : 0)) == 0)
                {
                    continue;
                }

                var flag = true;
                for (var index4 = index1; index4 < index2; index4++)
                {
                    if (tp[index2].Level >= tp[numArray1[index4]].Level)
                    {
                        continue;
                    }

                    for (var index5 = index3 - 1; index5 >= index4; index5 += -1)
                    {
                        numArray1[index5 + 1] = numArray1[index5];
                    }

                    numArray1[index4] = index2;
                    index3++;
                    flag = false;
                    break;
                }

                if (!flag)
                {
                    continue;
                }

                numArray1[index3] = index2;
                index3++;
            }

            var slotLevels = GetSlotLevels();
            var flag1 = false;
            var index6 = 0;
            for (var index2 = 0; index2 < tp.Length; index2++)
            {
                for (var index4 = 1; index4 < tp[numArray1[index2]].SlotCount; index4++)
                {
                    if (index6 == slotLevels.Length)
                    {
                        flag1 = true;
                    }

                    tp[numArray1[index2]].Slots[index4].Level = 50;
                    if (flag1)
                    {
                        continue;
                    }

                    if (tp[numArray1[index2]].NIDPower == -1 ||
                        !DatabaseAPI.Database.Power[tp[numArray1[index2]].NIDPower].AllowFrontLoading)
                    {
                        while (slotLevels[index6] <= tp[numArray1[index2]].Level)
                        {
                            index6++;
                            if (index6 != slotLevels.Length)
                            {
                                continue;
                            }

                            flag1 = true;
                            break;
                        }
                    }

                    tp[numArray1[index2]].Slots[index4].Level = slotLevels[index6] - 1;
                    index6++;
                }
            }

            if (!(flag1 & notifyUser))
            {
                return;
            }

            MessageBox.Show(
                @"The current arrangement of powers and their slots is impossible in-game. Invalid slots have been darkened and marked as level 51.",
                null, MessageBoxButtons.OK);
        }

        private int[] GetSlotLevels()
        {
            var slotLevels = new List<int>();
            for (var i = 0; i < DatabaseAPI.Database.Levels.Length; i++)
            {
                if (DatabaseAPI.Database.Levels[i].Slots <= 0)
                {
                    continue;
                }

                for (var j = 0; j < DatabaseAPI.Database.Levels[i].Slots; j++)
                {
                    slotLevels.Add(i + 1);
                }
            }

            return slotLevels.ToArray();
        }

        private void RemoveSlotFromTempList(PowerEntry? tp, int slotIdx)
        {
            if (tp == null)
            {
                return;
            }

            tp.Slots = tp.Slots.RemoveIndex(slotIdx);
        }

        private void PowerSwapByUser(int start, int finish)
        {
            var index = 0;
            do
            {
                dragdropScenarioAction[index] = MidsContext.Config.DragDropScenarioAction[index];
                ++index;
            } while (index <= 19);

            var tp = DeepCopyPowerList();
            if (PowerSwap(0, ref tp, start, finish) != -1)
            {
                return;
            }

            ShallowCopyPowerList(tp);
            PowerModified(true);
            DoRedraw();
        }

        private void RedrawSinglePower(ref PowerEntry? powerEntry, bool singleDraw = false, bool refreshInfo = false)
        {
            drawing.DrawPowerSlot(ref powerEntry, singleDraw);
            canvas.Invalidate();
            if (refreshInfo)
            {
                RefreshInfo();
            }
        }

        private void ClearPower(PowerEntry?[] tp, int pwrIdx)
        {
            tp[pwrIdx].Slots = [];
            tp[pwrIdx].SubPowers = [];
            tp[pwrIdx].IDXPower = -1;
            tp[pwrIdx].NIDPower = -1;
            tp[pwrIdx].NIDPowerset = -1;
            tp[pwrIdx].Tag = false;
            tp[pwrIdx].StatInclude = false;
        }

        private static PowerEntry?[] DeepCopyPowerList()
        {
            return MidsContext.Character.CurrentBuild.Powers.Select(x => (PowerEntry)x?.Clone()).ToArray();
        }

        private static void ShallowCopyPowerList(PowerEntry?[] source)
        {
            for (var index = 0; index < MidsContext.Character.CurrentBuild.Powers.Count; index++)
            {
                MidsContext.Character.CurrentBuild.Powers[index] = source[index];
            }
        }

        private Rectangle Dilate(Rectangle iRect, int iAdd)
        {
            iRect.X -= iAdd;
            iRect.Y -= iAdd;
            iRect.Height += iAdd * 2;
            iRect.Width += iAdd * 2;

            return iRect;
        }

        private int GetIconSize()
        {
            using var g = CreateGraphics();
            float dpiScale = g.DpiX / 96f;
            return (int)(24 * dpiScale);
        }

        private void ApplyTheme()
        {
            var theme = CurrentTheme;
            foreach (var button in Helpers.GetControlsOfType<IconButton>(this))
            {
                if (Equals(button.Tag, "KeepColors"))
                    continue;

                button.IconColor = theme.WindowIcon;
                button.IconSize = GetIconSize();

                if (button.Name != "btnClose")
                {
                    button.FlatAppearance.MouseOverBackColor = theme.WindowIconHover;
                    button.FlatAppearance.MouseDownBackColor = theme.WindowIconPressed;
                }
                else
                {
                    button.FlatAppearance.MouseOverBackColor = theme.WindowIconCloseHover;
                    button.FlatAppearance.MouseDownBackColor = theme.WindowIconClosePressed;
                }
            }
            canvas?.RequestFullRedraw();
        }

        private Rectangle MapRectToThis(Control origin, Rectangle localRect)
        {
            var screen = origin.RectangleToScreen(localRect);
            return RectangleToClient(screen); // "this" = MainWindow hosting the popup
        }

        private Point MapPointToThis(Control origin, Point localPoint)
        {
            var screen = origin.PointToScreen(localPoint);
            return PointToClient(screen);
        }

        private bool IsHoveringPopup()
        {
            return _popupHost != null
                   && _popupHost.Visible
                   && _popupHost.RectangleToScreen(_popupHost.ClientRectangle).Contains(Cursor.Position);
        }

        private void UpdateToon(Toon toon, Character? ch, int primaryIndex, int secondaryIndex, int pool0Index, int pool1Index, int pool2Index, int pool3Index, int ancillaryIndex, Func<Archetype, Enums.ePowerSetType, IPowerset[]> getPowerSets, Action lockSecondary)
        {
            var at = ch.Archetype;
            var newPrimaryPowerset = getPowerSets(at, Enums.ePowerSetType.Primary)[primaryIndex];
            IPowerset?[] ancPowersets = getPowerSets(at, Enums.ePowerSetType.Ancillary);
            if (toon != null)
            {
                var powerset1 = ch.Powersets[0];
                if (powerset1.nID != newPrimaryPowerset.nID)
                {
                    toon.SwitchSets(newPrimaryPowerset, powerset1);
                }

                if (ch.Powersets[0].nIDLinkSecondary > -1)
                {
                    var powerset2 = ch.Powersets[1];
                    var powerset3 = DatabaseAPI.Database.Powersets[ch.Powersets[0].nIDLinkSecondary];
                    if (powerset2.nID != powerset3.nID)
                    {
                        toon.SwitchSets(powerset3, powerset2);
                    }
                }
                else
                {
                    lockSecondary();
                    var powerset2 = ch.Powersets[1];
                    IPowerset?[] secondaryPowersets = getPowerSets(at, Enums.ePowerSetType.Secondary);
                    var newPowerset2 = secondaryPowersets[secondaryIndex];
                    if (powerset2.nID != newPowerset2.nID)
                    {
                        toon.SwitchSets(newPowerset2, powerset2);
                    }
                }
            }
            else
            {
                IPowerset?[] secondaryPowersets = getPowerSets(at, Enums.ePowerSetType.Secondary);
                ch.Powersets[0] = newPrimaryPowerset;
                ch.Powersets[1] = secondaryPowersets[secondaryIndex];
            }

            IPowerset?[] poolPowersets = getPowerSets(at, Enums.ePowerSetType.Pool);
            ch.Powersets[3] = poolPowersets[pool0Index];
            ch.Powersets[4] = poolPowersets[pool1Index];
            ch.Powersets[5] = poolPowersets[pool2Index];
            ch.Powersets[6] = poolPowersets[pool3Index];
            if (ancPowersets.Length > 0)
            {
                ch.Powersets[7] = ancPowersets[ancillaryIndex];
            }

            ch.Validate();
        }

        #endregion

        #region Public Methods

        public void RefreshInfo()
        {
            Info_Totals();
            if (dvLastPower <= -1)
            {
                return;
            }

            Info_Power(dvLastPower, dvLastEnh, dvLastNoLev, dataView.IsLocked);
            if (FrmEntityDetails is not { Visible: true })
            {
                return;
            }

            FrmEntityDetails.UpdateData(true);
        }

        #endregion

        #region Drawing

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Color menuBgColor = Color.FromArgb(200, 0, 0, 0); // 78% opaque black from MidsMenuStripRenderer

            int paddingLeft = Padding.Left;
            int paddingRight = Padding.Right;
            int menuBarHeight = MenuBar.Height;

            Rectangle leftRect = new(0, titlePanel.Bottom, paddingLeft, menuBarHeight);
            Rectangle rightRect = new(Width - paddingRight, titlePanel.Bottom, paddingRight, menuBarHeight);

            using SolidBrush menuBarBrush = new SolidBrush(menuBgColor);
            g.FillRectangle(menuBarBrush, leftRect);

            g.FillRectangle(menuBarBrush, rightRect);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = ClientRectangle;
            var theme = CurrentTheme;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            // Define regions
            int blueAreaHeight = titlePanel.Height + MenuBar.Height;
            Rectangle blueRect = new Rectangle(0, 0, rect.Width, blueAreaHeight);

            // Define palette
            Color lightBlue = theme.HeaderLight;
            Color midBlue = theme.HeaderMid;
            Color darkBlue = theme.HeaderDark;
            Color baseBackground = Color.Black;

            // ---- 1. Fill entire background with gray
            using (var baseBrush = new SolidBrush(baseBackground))
                g.FillRectangle(baseBrush, rect);

            // ---- 2. Fill top 90px with vertical blue gradient
            using var blueGradient = new LinearGradientBrush(blueRect, lightBlue, darkBlue, LinearGradientMode.Vertical);
            var blend = new ColorBlend
            {
                Colors = [lightBlue, midBlue, darkBlue],
                Positions = [0f, 0.5f, 1f]
            };
            blueGradient.InterpolationColors = blend;
            g.FillRectangle(blueGradient, blueRect);
        }

        #endregion

        #region Window Logic

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_MINIMIZEBOX = 0x00020000;
                const int WS_MAXIMIZEBOX = 0x00010000;
                const int WS_THICKFRAME = 0x00040000; // resizable border (required for proper resize/snap/shadow)
                const int WS_SYSMENU = 0x00080000; // enables system behaviors (Alt+Space, etc.)
                const int WS_CAPTION = 0x00C00000; // keep for snap/shadow heuristics; we suppress drawing anyway

                var cp = base.CreateParams;
                cp.Style |= WS_MINIMIZEBOX | WS_MAXIMIZEBOX | WS_THICKFRAME | WS_SYSMENU | WS_CAPTION;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_STYLECHANGED = 0x007D;
            const int WM_THEMECHANGED = 0x031A;
            const int WM_DPICHANGED = 0x02E0;
            const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
            const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;
            const int WM_ERASEBKGND = 0x0014;
            const int WM_NCCALCSIZE = 0x0083;
            const int WM_NCHITTEST = 0x0084;

            // prevent flicker
            if (m.Msg == WM_ERASEBKGND)
            {
                m.Result = IntPtr.Zero;
                return;
            }

            // remove non-client sizing border
            if (m.Msg == WM_NCCALCSIZE && m.WParam != IntPtr.Zero)
            {
                m.Result = IntPtr.Zero;
                return;
            }

            if (m.Msg == WM_NCHITTEST)
            {
                base.WndProc(ref m); // let WinForms do its thing first

                if (WindowState != FormWindowState.Maximized && m.Result == (IntPtr)HitTest.Client)
                {
                    var pt = PointToClient(Cursor.Position);
                    int bw = Math.Max(6, WinApi.GetResizeBorderThicknessPx(Handle)); // DPI-aware

                    int x = pt.X, y = pt.Y, w = ClientSize.Width, h = ClientSize.Height;

                    if (x < bw && y < bw) m.Result = (IntPtr)HitTest.TopLeft;
                    else if (x >= w - bw && y < bw) m.Result = (IntPtr)HitTest.TopRight;
                    else if (x < bw && y >= h - bw) m.Result = (IntPtr)HitTest.BottomLeft;
                    else if (x >= w - bw && y >= h - bw) m.Result = (IntPtr)HitTest.BottomRight;
                    else if (y < bw) m.Result = (IntPtr)HitTest.Top;
                    else if (y >= h - bw) m.Result = (IntPtr)HitTest.Bottom;
                    else if (x < bw) m.Result = (IntPtr)HitTest.Left;
                    else if (x >= w - bw) m.Result = (IntPtr)HitTest.Right;
                }
                return;
            }

            base.WndProc(ref m);

            if (m.Msg is WM_STYLECHANGED or WM_THEMECHANGED or WM_DPICHANGED
                or WM_DWMCOMPOSITIONCHANGED or WM_DWMCOLORIZATIONCOLORCHANGED)
            {
                ApplyWindowEffects(); // defensive re-apply
            }
        }

        private enum HitTest
        {
            Client = 1,
            Left = 10,
            Right = 11,
            Top = 12,
            TopLeft = 13,
            TopRight = 14,
            Bottom = 15,
            BottomLeft = 16,
            BottomRight = 17
        }

        #endregion
    }
}
