using System.Diagnostics;
using System.Drawing.Text;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.BuildFile;
using Mids_Reborn.Core.ShareSystem.RestModels;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Design.Extensions;
using Mids_Reborn.UI.Forms.Controls;
using Mids_Reborn.UI.Forms.ImportExportItems;
using Mids_Reborn.UI.Forms.OptionsMenuItems;
using Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor;
using Mids_Reborn.UI.Forms.UpdateSystem;
using Mids_Reborn.UI.Forms.WindowMenuItems;
using Mids_Reborn.UI.Renderer;
using MRBLogging;
using MRBResourceLib;
using RestSharp;

namespace Mids_Reborn.UI.Forms
{
    public partial class MainWindow : Form
    {
        // Define the Windows message ID for a setting change.
        private const int WmSettingChange = 0x001A;

        private const string UriScheme = "mrb";
        private frmBusy? _frmBusy;
        private FrmTeam? _frmTeam;
        private bool _loading;
        private bool _gfxDrawing;
        private long _popupLastOpenTime;
        private int _originalIndex = -1;
        public event EventHandler? TitleUpdated;
        public static MainWindow? MainInstance;
        private SetInspector? _setInspector;
        private DataView? _dvAnchored;
        private I9Picker? _i9Picker;
        private EnhCheckMode? _enhCheckMode;
        private Rectangle _formOrigin;
        private readonly BuildManager _buildManager;
        private PopUpDisplay? _popup;
        internal BuildRenderer? Drawing => drawing;

        public bool DbChangeRequested { get; set; }
        private string[]? CommandArgs { get; }
        private string? ProcessedCommand { get; set; }
        private bool ProcessedFromCommand { get; set; }
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

        private Lazy<ComboBoxT<Archetype>> CbtAT => new(() => new ComboBoxT<Archetype>(cbAT));
        private Lazy<ComboBoxT<string>> CbtPrimary => new(() => new ComboBoxT<string>(cbPrimary));
        private Lazy<ComboBoxT<string>> CbtSecondary => new(() => new ComboBoxT<string>(cbSecondary));
        private Lazy<ComboBoxT<string>> CbtAncillary => new(() => new ComboBoxT<string>(cbAncillary));
        private Lazy<ComboBoxT<string>> CbtPool0 => new(() => new ComboBoxT<string>(cbPool0));
        private Lazy<ComboBoxT<string>> CbtPool1 => new(() => new ComboBoxT<string>(cbPool1));
        private Lazy<ComboBoxT<string>> CbtPool2 => new(() => new ComboBoxT<string>(cbPool2));
        private Lazy<ComboBoxT<string>> CbtPool3 => new(() => new ComboBoxT<string>(cbPool3));

        public MainWindow(string[]? args = null)
        {
            CommandArgs = args;
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            Load += MainWindow_Load;
            Closed += MainWindow_Closed;
            FormClosing += MainWindow_Closing;
            Resize += MainWindow_Resize;
            ResizeEnd += MainWindow_ResizeEnd;
            KeyDown += MainWindow_KeyDown;
            MouseWheel += MainWindow_MouseWheel;
            TitleUpdated += OnTitleUpdate;
            Move += MainWindow_Move;
            Shown += MainWindow_Shown;
            NoUpdate = false;
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
            ActivePopupBounds = new Rectangle(0, 0, 1, 1);
            LastState = FormWindowState.Normal;
            FlipSteps = 5;
            FlipInterval = 10;
            FlipStepDelay = 3;
            FlipPowerID = -1;
            FlipSlotState = Array.Empty<int>();
            dragStartPower = -1;
            dragStartSlot = -1;
            dragdropScenarioAction = new short[20];
            DoneDblClick = false;
            DbChangeRequested = false;
            InitializeComponent();
            KeyPreview = true;

            InitializePopup(); // Initializes the popup (used to be i9Popup)
            InitializePicker();

            AddNonStandardControls();
            MainInstance = this;
            _buildManager = BuildManager.Instance;
            //disable menus that are no longer hooked up, but probably should be hooked back up
            tsHelp.Visible = false;
            tsHelp.Enabled = false;
            tmrGfx.Tick += tmrGfx_Tick;
            PetView.SliderUpdated += OnPetViewSliderUpdated;
            Icon = Resources.MRB_Icon_Concept;
            LogManager.Configure("Logs\\mids.log", "MidsReborn");
        }

        #region Override Methods

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style &= ~0x0002;
                cp.ExStyle &= ~0x02000000;
                cp.ExStyle &= ~0x00000020;
                return cp;
            }
        }

        protected override void WndProc(ref Message m)
        {
            // First, let the base form process the message normally.
            base.WndProc(ref m);

            // After that, check if the message was for a setting change.
            if (m.Msg == WmSettingChange)
            {
                // If so, tell our drawing class to update its font
                // and then trigger a full UI layout and redraw.
                if (drawing != null)
                {
                    UpdateUILayout();
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            MainWindow_Resize(this, EventArgs.Empty); // ensure correct size on launch
        }

        #endregion

        #region Event Methods

        private void MainWindow_Load(object? sender, EventArgs e)
        {
            _loading = true;
            try
            {
                if (MidsContext.Config.I9.DefaultIOLevel == 27)
                {
                    MidsContext.Config.I9.DefaultIOLevel = 49;
                }

                myDataView = _dvAnchored;
                pnlGFX.BackColor = BackColor;
                NoUpdate = true;

                _dvAnchored.VisibleSize = MidsContext.Config.DvState;
                SetTitleBar();
                NewToon();
                MidsContext.Character!.AlignmentChanged += CharacterOnAlignmentChanged;
                PowerModified(true);


                var comboData = MidsContext.Config.RelativeScales;
                EnemyRelativeToolStripComboBox.ComboBox.DataSource = null;
                EnemyRelativeToolStripComboBox.ComboBox.DisplayMember = "Key";
                EnemyRelativeToolStripComboBox.ComboBox.ValueMember = "Value";
                EnemyRelativeToolStripComboBox.ComboBox.DataSource = comboData;

                var scalingToHitItem = comboData.FirstOrDefault(x => x.Value == MidsContext.Config.ScalingToHit);
                var selectedIndex = comboData.IndexOf(scalingToHitItem);
                //EnemyRelativeToolStripComboBox.SelectedIndex = selectedIndex;

                _dvAnchored.Init();
                cbAT.SelectedItem = MidsContext.Character.Archetype;

                SetLockVisibility(false);

                if (MidsContext.Config.Bounds.Location.IsEmpty)
                {
                    Location = new Point((Screen.PrimaryScreen.Bounds.Width - Width) / 2, (Screen.PrimaryScreen.Bounds.Height - Height) / 2);
                    Size = new Size(1280, 720);
                }
                else
                {
                    switch (MidsContext.Config.WindowState)
                    {
                        case "Maximized":
                            WindowState = FormWindowState.Maximized;
                            DesktopBounds = MidsContext.Config.Bounds;
                            break;
                        case "Normal":
                            WindowState = FormWindowState.Normal;
                            DesktopBounds = MidsContext.Config.Bounds;
                            break;
                        case "Minimized":
                            WindowState = FormWindowState.Normal;
                            Location = new Point((Screen.PrimaryScreen.Bounds.Width - Width) / 2, (Screen.PrimaryScreen.Bounds.Height - Height) / 2);
                            Size = new Size(1280, 720);
                            break;
                    }
                }

                tsViewIOLevels.Checked = !MidsContext.Config.I9.HideIOLevels;
                tsViewRelative.Checked = MidsContext.Config.ShowEnhRel;
                tsViewSOLevels.Checked = MidsContext.Config.ShowSoLevels;
                tsViewSlotLevels.Checked = MidsContext.Config.ShowSlotLevels;
                tsViewRelativeAsSigns.Checked = MidsContext.Config.ShowRelSymbols;
                TsViewSelected();
                tsIODefault.Text = $"Default ({MidsContext.Config.I9.DefaultIOLevel + 1})";
                SetDamageMenuCheckMarks();
                GetBestDamageValues();
                _dvAnchored.SetFontData();
                DlgSave!.InitialDirectory = MidsContext.Config.BuildsPath;
                DlgOpen!.InitialDirectory = MidsContext.Config.BuildsPath;
                NoUpdate = false;
                tsViewSlotLevels.Checked = MidsContext.Config.ShowSlotLevels;
                ibSlotLevelsEx.ToggleState = MidsContext.Config.ShowSlotLevels switch
                {
                    true => MidsVectorButton.States.ToggledOn,
                    false => MidsVectorButton.States.ToggledOff
                };

                UpdateModeInfo();
                tsViewRelative.Checked = MidsContext.Config.ShowEnhRel;
                ibPopupEx.ToggleState = MidsContext.Config.DisableShowPopup switch
                {
                    true => MidsVectorButton.States.ToggledOff,
                    false => MidsVectorButton.States.ToggledOn
                };

                ibRecipeEx.ToggleState = MidsContext.Config.PopupRecipes switch
                {
                    true => MidsVectorButton.States.ToggledOn,
                    false => MidsVectorButton.States.ToggledOff
                };

                ibPvXEx.ToggleState = MidsContext.Config.Inc.DisablePvE switch
                {
                    true => MidsVectorButton.States.ToggledOn,
                    false => MidsVectorButton.States.ToggledOff
                };

                // _dvAnchored.SetScreenBounds(dataPanel.ClientRectangle);
                // var iLocation = new Point();
                // ref var local = ref iLocation;
                // var left = llPrimary.Left;
                // var top = llPrimary.Top;
                // var size1 = llPrimary.SizeNormal;
                // var height5 = size1.Height;
                // var y = top + height5 + 5;
                // local = new Point(left, y);
                // _dvAnchored.SetLocation(iLocation, true);
                PriSec_ExpandChanged(true);
                _loading = false;
                UpdateControls(true);
                SetColumns(MidsContext.Config.Columns < 1 ? 3 : MidsContext.Config.Columns, MidsContext.Config.Columns == 3 ? MidsContext.Config.ColumnStackingMode : Enums.eColumnStacking.None);
                //UpdatePoolsPanelSize();
                //InitializeDataView(); // This is the data view
                SetEnhCheckModePosition();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error has occurred when loading the main form. Error: {ex.Message}\r\n{ex.StackTrace}",
                    "OMIGODHAX");
                throw;
            }

            _loading = false;
            //MidsContext.Config.SaveConfig();
        }

        private void MainWindow_Move(object? sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            MidsContext.Config.Bounds = DesktopBounds;
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            switch (WindowState)
            {
                case FormWindowState.Minimized:
                    MidsContext.Config.WindowState = WindowState.ToString();
                    MessageBox.Show(@"Warning: Mids is currently minimized!
The default position/state will be used upon next launch.", @"Window State Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                case FormWindowState.Maximized:
                    MidsContext.Config.Bounds = DesktopBounds;
                    MidsContext.Config.WindowState = WindowState.ToString();
                    break;
                case FormWindowState.Normal:
                    MidsContext.Config.Bounds = DesktopBounds;
                    MidsContext.Config.WindowState = WindowState.ToString();
                    break;
            }

            MidsContext.Config.SaveConfig();

            FormClosing -= MainWindow_Closing;

            Application.Exit();
        }

        private void MainWindow_Closing(object? sender, FormClosingEventArgs e)
        {
            e.Cancel = CloseCommand();
            if (!e.Cancel)
            {
                AssetManager.Shutdown();
            }
        }

        private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            if (ModifierKeys == (Keys.Control | Keys.Alt | Keys.Shift))
            {
                MidsContext.Config.Mode = e.KeyCode switch
                {
                    Keys.A => MidsContext.Config.Mode switch
                    {
                        ConfigData.Modes.User => ConfigData.Modes.DbAdmin,
                        ConfigData.Modes.AppAdmin => ConfigData.Modes.DbAdmin,
                        ConfigData.Modes.DbAdmin => ConfigData.Modes.User,
                        _ => throw new ArgumentOutOfRangeException(nameof(ConfigData.Modes))
                    },
                    Keys.S => MidsContext.Config.Mode switch
                    {
                        ConfigData.Modes.User => ConfigData.Modes.AppAdmin,
                        ConfigData.Modes.DbAdmin => ConfigData.Modes.AppAdmin,
                        ConfigData.Modes.AppAdmin => ConfigData.Modes.User,
                        _ => throw new ArgumentOutOfRangeException(nameof(ConfigData.Modes))
                    },
                    _ => MidsContext.Config.Mode
                };
            }

            ToolStripSeparator5.Visible = MidsContext.Config.MasterMode;
            AdvancedToolStripMenuItem1.Visible = MidsContext.Config.MasterMode;
            SetTitleBar(MainModule.MidsController.Toon!.IsHero());
        }

        private void MainWindow_MouseWheel(object? sender, MouseEventArgs e)
        {
            _dvAnchored.Info_txtLarge.Focus();
        }

        private void MainWindow_Resize(object? sender, EventArgs e)
        {
            float scale = Math.Clamp(ClientSize.Width / 1600f, 0.8f, 1.5f);

            int baseWidth = 120;
            int baseHeight = 40;
            int scaledWidth = (int)(baseWidth * scale);
            int scaledHeight = (int)(baseHeight * scale);
            float scaledFont = 12f * scale;

            foreach (Control ctrl in buttonsLayoutPanel.Controls)
            {
                if (ctrl is not MidsVectorButton btn) continue;
                btn.Size = new Size(scaledWidth, scaledHeight);
                btn.Font = new Font(Fonts.Family("Noto Sans"), scaledFont, FontStyle.Bold, GraphicsUnit.Pixel);
            }

            buttonsLayoutPanel.PerformLayout(); // ensure it reflows properly
        }

        private void MainWindow_ResizeEnd(object? sender, EventArgs e)
        {
            if (_loading)
            {
                return;
            }

            //UpdatePoolsPanelSize();
            if (_dvAnchored != null)
            {
                _dvAnchored.SetScreenBounds(ClientRectangle);
                if (WindowState == FormWindowState.Minimized)
                {
                    if (_dvAnchored.Visible || FloatingDataForm == null)
                    {
                        return;
                    }

                    FloatingDataForm.Visible = false;
                    return;
                }

                if (!_dvAnchored.Visible && FloatingDataForm != null)
                {
                    FloatingDataForm.Visible = true;
                }
            }

            if (!NoResizeEvent & MainModule.MidsController.IsAppInitialized & Visible)
            {
                if (WindowState != FormWindowState.Minimized)
                {
                    MidsContext.Config.Bounds = DesktopBounds;
                    MidsContext.Config.WindowState = WindowState.ToString();
                }
            }

            UpdateControls();
            DoRedraw();
        }

        private async void MainWindow_Shown(object? sender, EventArgs e)
        {
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
                                DoOpen(file);
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
                            toonLoaded = await RunSchemaCommands(CommandArgs[0]);
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

            if (!MidsContext.Config.DisableLoadLastFileOnStart && !ProcessedFromCommand)
            {
                switch (MidsContext.Config.LastFileName)
                {
                    case var legacyBuild when MidsContext.Config.LastFileName.EndsWith(".mxd"):
                        toonLoaded = DoOpen(legacyBuild);
                        break;
                    case var build when MidsContext.Config.LastFileName.EndsWith(".mbd"):
                        toonLoaded = LoadCharacterFile(build);
                        break;
                }
            }

            if (!toonLoaded)
            {
                NewToon();
                PowerModified(true);
            }

            switch (ProcessedFromCommand)
            {
                case false when !MidsContext.Config.DisableLoadLastFileOnStart && !toonLoaded:
                    PowerModified(true);
                    break;
                // Build loaded from a file set as argument from the command line: done
                // Restore config variables to their original values.
                case true:
                    MidsContext.Config.LastFileName = prevLastFileNameCfg;
                    break;
            }

            MidsContext.Config.DisableLoadLastFileOnStart = prevLoadLastCfg;

            if (comLoad)
            {
                command_Load(ProcessedCommand);
            }

            if (!MidsContext.Config.AutomaticUpdates.Enabled)
            {
                return;
            }

            var updateLogger = LogManager.GetLogger("UpdateCheck");
            switch (MidsContext.Config.AutomaticUpdates.Type)
            {
                case ConfigData.AutoUpdType.Startup:
                    await UpdateCoordinator.CheckAndHandleUpdatesAsync(this, false, false, updateLogger);
                    break;
                case ConfigData.AutoUpdType.Delay:
                    await UpdateCoordinator.CheckAndHandleUpdatesAsync(this, false, true);
                    break;
            }

            if (drawing != null)
            {
                drawing.Columns = MidsContext.Config.Columns;
                drawing.ColumnStackingMode = MidsContext.Config.ColumnStackingMode;
                drawing.GetPowersLayout();

                // Calculate the minimum width for the entire form.
                // This includes the drawing panel, the left-side panel, and the window borders.
                int drawingMinWidth = drawing.GetMinimumRequiredWidth();
                int nonDrawingWidth = leftControlPanel.Width + (Width - ClientSize.Width);
                int formMinimumWidth = drawingMinWidth + nonDrawingWidth;

                // Set the form's minimum size.
                MinimumSize = MinimumSize with { Width = formMinimumWidth };

                // If the form is currently smaller than the new minimum, resize it.
                if (Width < formMinimumWidth)
                {
                    Width = formMinimumWidth;
                }

                UpdateUILayout();
            }

            Shown -= MainWindow_Shown;
        }

        private void OnPetViewSliderUpdated()
        {
            FrmEntityDetails?.UpdateData();
        }

        private void ibModeEx_OnClick(object sender, EventArgs eventArgs)
        {
            if (MainModule.MidsController.Toon == null)
            {
                return;
            }

            switch (ibModeEx.ToggleState)
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

        private void cbAncillary_DrawItem(object sender, DrawItemEventArgs e)
        {
            CbDrawItem(CbtAncillary.Value, Enums.ePowerSetType.Ancillary, e);
        }

        private void cbAncillery_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
            {
                return;
            }

            ChangeSets();
            UpdatePowerLists();
            if (!MidsContext.Config.UseOldTotalsWindow)
            {
                frmTotalsV2.SetTitle(fTotals2);
            }
        }

        private void cbPools_MouseLeave(object sender, EventArgs e)
        {
            HidePopup();
        }

        private void cbPools_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null)
            {
                return;
            }

            var combo = (ComboBox)sender;
            var rBounds = new Rectangle(
                poolsPanel.Location.X + combo.Bounds.X + 1,
                poolsPanel.Location.Y + combo.Location.Y - (combo.Name == "cbAncillary" ? 0 : combo.Height) + lblPool1.Height + 5,
                combo.Width,
                combo.Height
            );
            const string extraString = "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            var nId = combo.Name switch
            {
                "cbPool0" => MidsContext.Character.Powersets[3].nID,
                "cbPool1" => MidsContext.Character.Powersets[4].nID,
                "cbPool2" => MidsContext.Character.Powersets[5].nID,
                "cbPool3" => MidsContext.Character.Powersets[6].nID,
                "cbAncillary" => MidsContext.Character.IsKheldian ? -1 : MidsContext.Character.Powersets[7].nID,
                _ => -1
            };

            var vAlign = combo.Name == "cbAncillary"
                ? VerticalAlignment.Bottom
                : VerticalAlignment.Top;
            ShowPopup(nId, MidsContext.Character.Archetype.Idx, rBounds, extraString, vAlign);
        }

        private void cbAT_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized)
            {
                return;
            }

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.DrawBackground();
            using var solidBrush = new SolidBrush(Color.Black);
            if (e.Index > -1)
            {
                var cbAT = new ComboBoxT<Archetype>(this.cbAT);
                var index = ArchetypeIndirectToIndex(e.Index);
                var destRect = new RectangleF(e.Bounds.X + 1, e.Bounds.Y, 16f, 16f);

                // Look up the individual archetype icon from the dictionary
                if (AssetManager.Archetypes.TryGetValue(index, out var atIcon) && atIcon?.Bitmap != null)
                {
                    e.Graphics.DrawImage(atIcon.Bitmap, destRect);
                }

                using var format = new StringFormat(StringFormatFlags.NoWrap)
                {
                    LineAlignment = StringAlignment.Center
                };
                var layoutRectangle = new RectangleF(e.Bounds.X + destRect.X + destRect.Width, e.Bounds.Y,
                    e.Bounds.Width - (destRect.X + destRect.Width), e.Bounds.Height);
                e.Graphics.DrawString(cbAT[e.Index].DisplayName, e.Font, solidBrush, layoutRectangle, format);
            }

            e.DrawFocusRectangle();
        }

        private void cbAT_MouseLeave(object sender, EventArgs e)
        {
            HidePopup();
        }

        private void cbAT_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || cbAT.SelectedIndex < 0)
            {
                return;
            }

            ShowPopup(-1, CbtAT.Value.SelectedItem.Idx, cbAT.Bounds);
        }

        private void cbAT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
            {
                return;
            }

            NewToon(false);
            SetEnhCheckModePosition();
            SetAncilPoolHeight();
            GetBestDamageValues();
            if (!MidsContext.Config.UseOldTotalsWindow)
            {
                frmTotalsV2.SetTitle(fTotals2);
            }
        }

        private static void CbDrawItem(ComboBoxT<string> target, Enums.ePowerSetType setType, DrawItemEventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized)
            {
                return;
            }

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.DrawBackground();

            using var solidBrush = new SolidBrush(Color.Black);
            var powersetIndexes = DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, setType);

            if (e.Index > -1 && e.Index < powersetIndexes.Length)
            {
                var nId = powersetIndexes[e.Index].nID;
                var powerset = DatabaseAPI.Database.Powersets[nId];
                var destRect = new RectangleF(e.Bounds.X + 1, e.Bounds.Y, 16f, 16f);

                // Get the pre-cached image using the helper method
                var powersetImage = AssetManager.GetPowersetImage(powerset);

                // This if/else block is now correctly restored
                if ((e.State & DrawItemState.ComboBoxEdit) > DrawItemState.None)
                {
                    // Only draw the icon in the edit box if the text fits
                    if (e.Graphics.MeasureString(target[e.Index], e.Font).Width <= e.Bounds.Width - 18)
                    {
                        if (powersetImage?.Bitmap != null)
                        {
                            e.Graphics.DrawImage(powersetImage.Bitmap, destRect);
                        }
                    }
                    else
                    {
                        // Otherwise, "hide" the icon to make room for text
                        destRect.Width = 0.0f;
                    }
                }
                else
                {
                    // Always draw the icon in the dropdown list portion
                    if (powersetImage?.Bitmap != null)
                    {
                        e.Graphics.DrawImage(powersetImage.Bitmap, destRect);
                    }
                }

                using var format = new StringFormat(StringFormatFlags.NoWrap);
                format.LineAlignment = StringAlignment.Center;

                // The text layout correctly starts after the (potentially hidden) icon
                var layout = new RectangleF(e.Bounds.X + destRect.X + destRect.Width, e.Bounds.Y, e.Bounds.Width - (destRect.X + destRect.Width), e.Bounds.Height);
                e.Graphics.DrawString(target[e.Index], e.Font, solidBrush, layout, format);
            }

            e.DrawFocusRectangle();
        }

        private void cbOrigin_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized)
            {
                return;
            }

            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.DrawBackground();
            using var solidBrush = new SolidBrush(Color.Black);
            if (e.Index > -1)
            {
                var cmbOrigin = GetCbOrigin();
                var destRect = new RectangleF(e.Bounds.X + 1, e.Bounds.Y, 16f, 16f);

                // Look up the individual origin icon from the dictionary
                var originId = DatabaseAPI.GetOriginIDByName(cmbOrigin[e.Index]);
                if (AssetManager.Origins.TryGetValue(originId, out var originIcon) && originIcon?.Bitmap != null)
                {
                    e.Graphics.DrawImage(originIcon.Bitmap, Rectangle.Truncate(destRect));
                }

                using var format = new StringFormat(StringFormatFlags.NoWrap);
                format.LineAlignment = StringAlignment.Center;

                var layoutRectangle = new RectangleF(e.Bounds.X + destRect.X + destRect.Width, e.Bounds.Y,
                    e.Bounds.Width - (destRect.X + destRect.Width), e.Bounds.Height);
                e.Graphics.DrawString(cmbOrigin[e.Index], e.Font, solidBrush, layoutRectangle, format);
            }

            e.DrawFocusRectangle();
        }

        private void cbOrigin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
            {
                return;
            }

            MidsContext.Character.Origin = cbOrigin.SelectedIndex;
            AssetManager.SetOrigin(cbOrigin.SelectedItem.ToStringOrNull());
            DisplayName();
        }

        private void cbPool0_DrawItem(object sender, DrawItemEventArgs e)
        {
            CbDrawItem(CbtPool0.Value, Enums.ePowerSetType.Pool, e);
        }

        private void cbPool0_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
            {
                return;
            }

            ChangeSets();
            UpdatePowerLists();
        }

        private void cbPool1_DrawItem(object sender, DrawItemEventArgs e)
        {
            CbDrawItem(CbtPool1.Value, Enums.ePowerSetType.Pool, e);
        }

        private void cbPool1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
            {
                return;
            }

            ChangeSets();
            UpdatePowerLists();
        }

        private void cbPool2_DrawItem(object sender, DrawItemEventArgs e)
        {
            CbDrawItem(CbtPool2.Value, Enums.ePowerSetType.Pool, e);
        }

        private void cbPool2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
            {
                return;
            }

            ChangeSets();
            UpdatePowerLists();
        }

        private void cbPool3_DrawItem(object sender, DrawItemEventArgs e)
        {
            CbDrawItem(CbtPool3.Value, Enums.ePowerSetType.Pool, e);
        }

        private void cbPool3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
            {
                return;
            }

            ChangeSets();
            UpdatePowerLists();
        }

        private void cbPrimary_DrawItem(object sender, DrawItemEventArgs e)
        {
            CbDrawItem(CbtPrimary.Value, Enums.ePowerSetType.Primary, e);
        }

        private void cbPrimary_MouseLeave(object sender, EventArgs e)
        {
            HidePopup();
        }

        private void cbPrimary_MouseMove(object sender, MouseEventArgs e)
        {
            if (MidsContext.Character == null || MidsContext.Character.Archetype == null || cbPrimary.SelectedIndex < 0)
            {
                return;
            }

            var extraString =
                "This is your primary powerset. This powerset can be changed after a build has been started, and any placed powers will be swapped out for those in the new set.";
            ShowPopup(
                DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, Enums.ePowerSetType.Primary)[
                    cbPrimary.SelectedIndex].nID,
                MidsContext.Character.Archetype.Idx, cbPrimary.Bounds, extraString);
        }

        private void cbPrimary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
            {
                return;
            }

            ChangeSets();
            UpdatePowerLists();
            if (!MidsContext.Config.UseOldTotalsWindow)
            {
                frmTotalsV2.SetTitle(fTotals2);
            }
        }

        private void cbSecondary_DrawItem(object sender, DrawItemEventArgs e)
        {
            CbDrawItem(CbtSecondary.Value, Enums.ePowerSetType.Secondary, e);
        }

        private void cbSecondary_MouseLeave(object sender, EventArgs e)
        {
            HidePopup();
        }

        private void cbSecondary_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Archetype.Idx < 0 ||
                cbSecondary.SelectedIndex < 0)
            {
                return;
            }

            var extraString = MidsContext.Character.Powersets[0].nIDLinkSecondary <= -1
                ? "This is your secondary powerset. This powerset can be changed after a build has been started, and any placed powers will be swapped out for those in the new set."
                : "This is your secondary powerset. This powerset is linked to your primary set and cannot be changed independantly. However, it can be changed by selecting a different primary powerset.";
            ShowPopup(
                DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, Enums.ePowerSetType.Secondary)[
                    cbSecondary.SelectedIndex].nID,
                MidsContext.Character.Archetype.Idx, cbSecondary.Bounds, extraString);
        }

        private void cbSecondary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
            {
                return;
            }

            ChangeSets();
            UpdatePowerLists();
            if (!MidsContext.Config.UseOldTotalsWindow)
            {
                frmTotalsV2.SetTitle(fTotals2);
            }
        }

        internal void DataView_SlotFlip(int powerIndex)
        {
            StartFlip(powerIndex);
        }

        internal void DataView_SlotUpdate(IPower? power, int val)
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

        private void dvAnchored_Float()
        {
            FloatingDataForm = new frmFloatingStats(this)
            {
                Left = Left + _dvAnchored.Left,
                Top = Top + _dvAnchored.Top,
                dvFloat =
                {
                    VisibleSize = _dvAnchored.VisibleSize
                }
            };
            myDataView = FloatingDataForm.dvFloat;
            myDataView.TabPage = _dvAnchored.TabPage;
            FloatingDataForm.dvFloat.Init();
            FloatingDataForm.dvFloat.SetFontData();
            myDataView.BackColor = BackColor;
            myDataView.DrawVillain = !MainModule.MidsController.Toon.IsHero();
            _dvAnchored.Visible = false;
            pnlGFX.Select();
            FloatingDataForm.Show();
            RefreshInfo();
            if (dvLastPower <= -1)
            {
                return;
            }

            Info_Power(dvLastPower, dvLastEnh, dvLastNoLev, DataViewLocked);
        }

        private void dvAnchored_Move()
        {
            PriSec_ExpandChanged(true);
            ReArrange(false);
        }

        private void dvAnchored_SizeChange(Size newSize, bool compact)
        {
            ReArrange(false);
            if (!(MainModule.MidsController.IsAppInitialized & Visible))
            {
                return;
            }

            MidsContext.Config.DvState = _dvAnchored.VisibleSize;
        }

        private void dvAnchored_TabChanged(int index)
        {
            SetDataViewTab(index);
        }

        private void dvAnchored_Unlock()
        {
            DataViewLocked = false;
            if (dvLastPower <= -1)
            {
                return;
            }

            Info_Power(dvLastPower, dvLastEnh, dvLastNoLev, DataViewLocked);
        }

        private void dvAnchored_EntityDetails(string entityUid, HashSet<string> powers, int basePowerHistoryIdx, PetInfo petInfo)
        {
            if (FrmEntityDetails is not { Visible: true })
            {
                FrmEntityDetails = new FrmEntityDetails(entityUid, powers, petInfo);
                FrmEntityDetails.Show(this);
            }
            else if (FrmEntityDetails.Visible)
            {
                FrmEntityDetails.UpdateData(entityUid, powers);
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

                if (!_dvAnchored.PetInfo.HasEmptyBasePower)
                {
                    _dvAnchored.PetInfo.ExecuteUpdate();
                }

                MidsContext.Config.Tips.Show(Tips.TipType.FirstEnhancement);
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
            myDataView.SetEnhancementPicker(i9Slot);
            ShowPopup(PickerHID, -1, -1, new Point(), I9Picker.Bounds, i9Slot, -1, VerticalAlignment.Top, enhUniqueStatus);
        }

        private void I9Picker_HoverSet(int e)
        {
            myDataView.SetSetPicker(e);
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

        private void I9Picker_Moved(Rectangle newBounds, Rectangle oldBounds)
        {
            MovePopup(I9Picker.Bounds);
            RedrawUnderPopup(oldBounds);
        }

        private void Popup_MouseMove(object? sender, MouseEventArgs e)
        {
            if (MidsContext.Config.CloseEnhSelectPopupByMove)
            {
                HidePopup();
            }
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

        private void ibTeamEx_OnClick(object? sender, EventArgs e)
        {
            if (_frmTeam == null || _frmTeam.IsDisposed)
            {
                _frmTeam = new FrmTeam(this);
            }

            _frmTeam.Show();
        }

        private void ibAlignmentEx_OnClick(object? sender, EventArgs e)
        {
            var nbUpdated = 0;
            if (MidsContext.Character != null)
            {
                MidsContext.Character.Alignment = ibAlignmentEx.ToggleState switch
                {
                    MidsVectorButton.States.ToggledOff => Enums.Alignment.Hero,
                    MidsVectorButton.States.ToggledOn => Enums.Alignment.Villain,
                    _ => MidsContext.Character.Alignment
                };

                MidsContext.Character.Alignment = ibAlignmentEx.ToggleState switch
                {
                    MidsVectorButton.States.ToggledOff => Enums.Alignment.Hero,
                    MidsVectorButton.States.ToggledOn => Enums.Alignment.Villain,
                    _ => MidsContext.Character.Alignment
                };

                if (fAccolade != null)
                {
                    if (!fAccolade.IsDisposed)
                    {
                        fAccolade.Dispose();
                    }

                    var factionSpecificAccolades = frmAccolade.FactionSpecificAccolades();
                    var pSource = new List<string>();
                    var pTarget = new List<string>();

                    if (MainModule.MidsController.Toon == null || MainModule.MidsController.Toon.IsHero())
                    {
                        pSource = factionSpecificAccolades.Select(x => x.Villain).ToList();
                        pTarget = factionSpecificAccolades.Select(x => x.Hero).ToList();
                    }
                    else
                    {
                        pSource = factionSpecificAccolades.Select(x => x.Hero).ToList();
                        pTarget = factionSpecificAccolades.Select(x => x.Villain).ToList();
                    }

                    var pDict = pSource
                        .Zip(pTarget, KeyValuePair.Create)
                        .ToDictionary(x => x.Key, x => x.Value);

                    var selectedAccolades = MidsContext.Character?.CurrentBuild?.Powers
                        .Where(x => x is { Power.InherentType: Enums.eGridType.Accolade })
                        .Select(x => x?.Power)
                        .ToList();

                    if (selectedAccolades is not { Count: > 0 })
                    {
                        return;
                    }

                    foreach (var p in selectedAccolades)
                    {
                        if (p == null)
                        {
                            continue;
                        }

                        if (!pDict.TryGetValue(p.DisplayName, out var pName))
                        {
                            continue;
                        }

                        var targetPower = DatabaseAPI.Database.Power
                            .DefaultIfEmpty(null)
                            .FirstOrDefault(x => x is not null && x.FullName.StartsWith("Temporary_Powers.Accolades.") & x.DisplayName == pName);

                        if (targetPower == null)
                        {
                            continue;
                        }

                        MidsContext.Character.CurrentBuild.RemovePower(p);
                        MidsContext.Character.CurrentBuild.AddPower(targetPower, 49).StatInclude = true;
                        nbUpdated++;
                    }
                }
            }

            if (nbUpdated > 0)
            {
                PowerModified(true, false);
            }

            drawing?.ColorSwitch();
            fTotals?.Refresh();
            fTotals2?.Refresh();
            SetTitleBar();
            UpdateColors();
            DoRedraw();
        }

        private void ibPopupEx_OnClick(object? sender, EventArgs e)
        {
            MidsContext.Config.DisableShowPopup = ibPopupEx.ToggleState switch
            {
                MidsVectorButton.States.ToggledOff => true,
                MidsVectorButton.States.ToggledOn => false,
                _ => MidsContext.Config.DisableShowPopup
            };
        }

        private void ibPvXEx_OnClick(object? sender, EventArgs e)
        {
            MidsContext.Config.Inc.DisablePvE = ibPvXEx.ToggleState switch
            {
                MidsVectorButton.States.ToggledOff => false,
                MidsVectorButton.States.ToggledOn => true,
                _ => MidsContext.Config.Inc.DisablePvE
            };

            RefreshInfo();
        }

        private void ibRecipeEx_OnClick(object? sender, EventArgs e)
        {
            MidsContext.Config.PopupRecipes = ibRecipeEx.ToggleState switch
            {
                MidsVectorButton.States.ToggledOff => false,
                MidsVectorButton.States.ToggledOn => true,
                _ => MidsContext.Config.PopupRecipes
            };
        }

        private void ibSetsEx_OnClick(object? sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon == null)
            {
                return;
            }

            FloatSets(true);
        }

        private void ibSlotLevelsEx_OnClick(object? sender, EventArgs e)
        {
            tsViewSlotLevels_Click(this, EventArgs.Empty);
        }

        private void ibSlotInfoEx_Onclick(object? sender, EventArgs e)
        {
            MidsContext.Config.ShowSlotsLeft = ibSlotInfoEx.ToggleState == MidsVectorButton.States.ToggledOff;
        }

        private void ibTotalsEx_OnClick(object? sender, EventArgs e)
        {
            FloatTotals(true, MidsContext.Config is { UseOldTotalsWindow: true });
        }

        private void ibIncarnatesEx_OnClick(object? sender, EventArgs e)
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
                ibIncarnatePowersEx.ToggleState = MidsVectorButton.States.ToggledOn;
                fIncarnate.Show(this);
            }
            else
            {
                ibIncarnatePowersEx.ToggleState = MidsVectorButton.States.ToggledOff;
                fIncarnate?.Close();
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

        private void lblATLocked_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || cbAT.SelectedIndex < 0)
            {
                return;
            }

            ShowPopup(-1, CbtAT.Value.SelectedItem.Idx, cbAT.Bounds, string.Empty);
        }

        private void lblATLocked_Paint(object sender, PaintEventArgs e)
        {
            if (MainModule.MidsController.Toon == null)
            {
                return;
            }

            var destRect = new Rectangle(1, (lblATLocked.Height - 17) / 2, 16, 16);
            destRect.Y += 1;
            destRect.X += 2;

            // Look up the individual archetype icon
            if (AssetManager.Archetypes.TryGetValue(MidsContext.Character.Archetype.Idx, out var atIcon) && atIcon?.Bitmap != null)
            {
                var graphics = e.Graphics;
                graphics.DrawImage(atIcon.Bitmap, destRect);
                destRect.X = lblATLocked.Width - 19;
                graphics.DrawImage(atIcon.Bitmap, destRect);
            }
        }

        private void lblLocked_MouseLeave(object sender, EventArgs e)
        {
            HidePopup();
        }

        private void lblLockedPool0_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[3] == null)
            {
                return;
            }

            var extraString =
                "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[3].nID, MidsContext.Character.Archetype.Idx, cbPool0.Bounds,
                extraString);
        }

        private void lblLockedPool0_Paint(object sender, PaintEventArgs e)
        {
            MiniPaint(ref e, Enums.PowersetType.Pool0);
        }

        private void lblLockedPool1_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[4] == null)
            {
                return;
            }

            var extraString =
                "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[4].nID, MidsContext.Character.Archetype.Idx, cbPool1.Bounds,
                extraString);
        }

        private void lblLockedPool1_Paint(object sender, PaintEventArgs e)
        {
            MiniPaint(ref e, Enums.PowersetType.Pool1);
        }

        private void lblLockedPool2_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[5] == null)
            {
                return;
            }

            var ExtraString =
                "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[5].nID, MidsContext.Character.Archetype.Idx, cbPool2.Bounds,
                ExtraString);
        }

        private void lblLockedPool2_Paint(object sender, PaintEventArgs e)
        {
            MiniPaint(ref e, Enums.PowersetType.Pool2);
        }

        private void lblLockedPool3_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[6] == null)
            {
                return;
            }

            var ExtraString =
                "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            // Bug: popup will turn into total garbage if the mouse pointer is within the popup drawing rectangle.
            // Add a vertical offset to ensure the mouse pointer stays out of the popup. 
            ShowPopup(MidsContext.Character.Powersets[6].nID, MidsContext.Character.Archetype.Idx,
                new Rectangle(lblLockedPool3.Location.X, lblLockedPool3.Location.Y - 3 * lblLockedPool3.Height, cbPool3.Bounds.Width, cbPool3.Bounds.Height),
                //cbPool3.Bounds,
                ExtraString);
        }

        private void lblLockedPool3_Paint(object sender, PaintEventArgs e)
        {
            MiniPaint(ref e, Enums.PowersetType.Pool3);
        }

        private void lblLockedAncillary_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[7] == null)
            {
                return;
            }

            var ExtraString =
                "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[7].nID, MidsContext.Character.Archetype.Idx,
                CbtAncillary.Value.Bounds, ExtraString);
        }

        private void lblLockedAncillary_Paint(object sender, PaintEventArgs e)
        {
            MiniPaint(ref e, Enums.PowersetType.Ancillary);
        }

        private void lblLockedSecondary_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Archetype.Idx < 0 ||
                cbSecondary.SelectedIndex < 0)
            {
                return;
            }

            var ExtraString = MidsContext.Character.Powersets[0].nIDLinkSecondary <= -1
                ? "This is your secondary powerset. This powerset can be changed after a build has been started, and any placed powers will be swapped out for those in the new set."
                : "This is your secondary powerset. This powerset is linked to your primary set and cannot be changed independantly. However, it can be changed by selecting a different primary powerset.";
            ShowPopup(
                DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, Enums.ePowerSetType.Secondary)[
                    cbSecondary.SelectedIndex].nID,
                MidsContext.Character.Archetype.Idx, cbSecondary.Bounds, ExtraString);
        }

        private void llAll_EmptyHover()
        {
            HidePopup();
        }

        private void llALL_MouseLeave(object sender, EventArgs e)
        {
            HidePopup();
        }

        private void llALL_ItemClick_RefreshGfx()
        {
            DoRefresh();
        }

        private void llAncillary_ItemClick(ListLabel.ListLabelItem item, MouseButtons button)
        {
            if (MidsContext.EnhCheckMode)
            {
                return;
            }

            if (item.ItemState == ListLabel.LlItemState.Heading)
            {
                return;
            }

            switch (button)
            {
                case MouseButtons.Left:
                    PowerPicked(item.NIdSet, item.NIdPower);
                    frmTotalsV2.SetTitle(fTotals2);
                    break;
                case MouseButtons.Right:
                    Info_Power(item.NIdPower, -1, false, true);
                    break;
            }

            llALL_ItemClick_RefreshGfx();
        }

        private void llAncillary_ItemHover(ListLabel.ListLabelItem item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            var llBounds = new Rectangle(
                llAncillary.Bounds.X + poolsPanel.Bounds.X,
                llAncillary.Bounds.Y + poolsPanel.Bounds.Y,
                llAncillary.Bounds.Width,
                llAncillary.Bounds.Height);
            if (item.ItemState == ListLabel.LlItemState.Heading)
            {
                ShowPopup(item.NIdSet, -1, llBounds);
            }
            else
            {
                Info_Power(item.NIdPower);
                ShowPopup(-1, item.NIdPower, -1, new Point(), llBounds, null, -1, VerticalAlignment.Bottom);
            }
        }

        private void llPool0_ItemClick(ListLabel.ListLabelItem item, MouseButtons button)
        {
            if (MidsContext.EnhCheckMode)
            {
                return;
            }

            if (button == MouseButtons.Left)
            {
                PowerPicked(Enums.PowersetType.Pool0, item.NIdPower);
            }
            else
            {
                if (button != MouseButtons.Right)
                {
                    return;
                }

                Info_Power(item.NIdPower, -1, false, true);
            }

            llALL_ItemClick_RefreshGfx();
        }

        private void llPool0_ItemHover(ListLabel.ListLabelItem item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            Info_Power(item.NIdPower);
            var llBounds = llPool0.Bounds with { X = llPool0.Bounds.X + poolsPanel.Bounds.X, Y = llPool0.Bounds.Y + poolsPanel.Bounds.Y };
            ShowPopup(-1, item.NIdPower, -1, new Point(), llBounds);
        }

        private void llPool1_ItemClick(ListLabel.ListLabelItem item, MouseButtons button)
        {
            if (MidsContext.EnhCheckMode)
            {
                return;
            }

            if (button == MouseButtons.Left)
            {
                PowerPicked(Enums.PowersetType.Pool1, item.NIdPower);
            }
            else
            {
                if (button != MouseButtons.Right)
                {
                    return;
                }

                Info_Power(item.NIdPower, -1, false, true);
            }

            llALL_ItemClick_RefreshGfx();
        }

        private void llPool1_ItemHover(ListLabel.ListLabelItem item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            Info_Power(item.NIdPower);
            var llBounds = llPool1.Bounds with { X = llPool1.Bounds.X + poolsPanel.Bounds.X, Y = llPool1.Bounds.Y + poolsPanel.Bounds.Y };
            ShowPopup(-1, item.NIdPower, -1, new Point(), llBounds);
        }

        private void llPool2_ItemClick(ListLabel.ListLabelItem item, MouseButtons button)
        {
            if (MidsContext.EnhCheckMode)
            {
                return;
            }

            if (button == MouseButtons.Left)
            {
                PowerPicked(Enums.PowersetType.Pool2, item.NIdPower);
            }
            else
            {
                if (button != MouseButtons.Right)
                {
                    return;
                }

                Info_Power(item.NIdPower, -1, false, true);
            }

            llALL_ItemClick_RefreshGfx();
        }

        private void llPool2_ItemHover(ListLabel.ListLabelItem item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            Info_Power(item.NIdPower);
            var llBounds = llPool2.Bounds with { X = llPool2.Bounds.X + poolsPanel.Bounds.X, Y = llPool2.Bounds.Y + poolsPanel.Bounds.Y };
            ShowPopup(-1, item.NIdPower, -1, new Point(), llBounds);
        }

        private void llPool3_ItemClick(ListLabel.ListLabelItem item, MouseButtons button)
        {
            if (MidsContext.EnhCheckMode)
            {
                return;
            }

            if (button == MouseButtons.Left)
            {
                PowerPicked(Enums.PowersetType.Pool3, item.NIdPower);
            }
            else
            {
                if (button != MouseButtons.Right)
                {
                    return;
                }

                Info_Power(item.NIdPower, -1, false, true);
            }

            llALL_ItemClick_RefreshGfx();
        }

        private void llPool3_ItemHover(ListLabel.ListLabelItem item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            Info_Power(item.NIdPower);
            var llBounds = llPool3.Bounds with { X = llPool3.Bounds.X + poolsPanel.Bounds.X, Y = llPool3.Bounds.Y + poolsPanel.Bounds.Y };
            ShowPopup(-1, item.NIdPower, -1, new Point(), llBounds);
        }

        private void llPrimary_ItemClick(ListLabel.ListLabelItem item, MouseButtons button)
        {
            if (item.ItemState == ListLabel.LlItemState.Heading)
            {
                return;
            }

            if (MidsContext.EnhCheckMode)
            {
                return;
            }

            switch (button)
            {
                case MouseButtons.Left:
                    PowerPicked(item.NIdSet, item.NIdPower);
                    break;
                case MouseButtons.Right:
                    Info_Power(item.NIdPower, -1, false, true);
                    break;
            }

            llALL_ItemClick_RefreshGfx();
        }

        private void llPrimary_ItemHover(ListLabel.ListLabelItem item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            if (item.ItemState == ListLabel.LlItemState.Heading)
            {
                ShowPopup(item.NIdSet, -1, llPrimary.Bounds, string.Empty);
            }
            else
            {
                Info_Power(item.NIdPower);
                ShowPopup(-1, item.NIdPower, -1, new Point(), llPrimary.Bounds);
            }
        }

        private void llSecondary_ItemClick(ListLabel.ListLabelItem item, MouseButtons button)
        {
            if (item.ItemState == ListLabel.LlItemState.Heading)
            {
                return;
            }

            if (MidsContext.EnhCheckMode)
            {
                return;
            }

            switch (button)
            {
                case MouseButtons.Left:
                    PowerPicked(item.NIdSet, item.NIdPower);
                    break;
                case MouseButtons.Right:
                    Info_Power(item.NIdPower, -1, false, true);
                    break;
            }

            llALL_ItemClick_RefreshGfx();
        }

        private void llSecondary_ItemHover(ListLabel.ListLabelItem item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            if (item.ItemState == ListLabel.LlItemState.Heading)
            {
                ShowPopup(item.NIdSet, -1, llSecondary.Bounds, string.Empty);
            }
            else
            {
                Info_Power(item.NIdPower);
                ShowPopup(-1, item.NIdPower, -1, new Point(), llSecondary.Bounds);
            }
        }

        private void EnemyRelativeLevel_DropDown(object? sender, EventArgs e)
        {
            if (EnemyRelativeToolStripComboBox.ComboBox != null)
            {
                _originalIndex = EnemyRelativeToolStripComboBox.ComboBox.SelectedIndex;
            }
        }

        private void EnemyRelativeLevel_DropDownClosed(object? sender, EventArgs e)
        {
            if (EnemyRelativeToolStripComboBox.ComboBox != null && EnemyRelativeToolStripComboBox.ComboBox.SelectedIndex != _originalIndex)
            {
                // If selection changed, update the value and refresh
                MidsContext.Config.ScalingToHit = (float)EnemyRelativeToolStripComboBox.ComboBox.SelectedValue;
                RefreshInfo();
            }
            // Always return focus to the menu bar regardless of whether the selection changed
            MenuBar.Focus();
        }

        private void EnemyRelativeLevel_MouseLeave(object? sender, EventArgs e)
        {
            if (EnemyRelativeToolStripComboBox.ComboBox != null)
            {
                EnemyRelativeToolStripComboBox.ComboBox.DroppedDown = false;
            }
        }

        private void EnemyRelativeLevel_SelectionChangeCommitted(object? sender, EventArgs e)
        {
            if (EnemyRelativeToolStripComboBox.ComboBox == null)
            {
                return;
            }

            MidsContext.Config.ScalingToHit = (float)EnemyRelativeToolStripComboBox.ComboBox.SelectedValue;
            RefreshInfo();
        }

        private void ibDynMode_Click(object? sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon == null)
            {
                return;
            }

            if (MidsContext.Config.BuildMode == Enums.dmModes.LevelUp && !ibDynMode.Lock)
            {
                ibDynMode.Lock = true;
            }

            MidsContext.Config.BuildOption = MidsContext.Config.BuildOption switch
            {
                Enums.dmItem.Power => Enums.dmItem.Slot,
                _ => Enums.dmItem.Power
            };

            UpdateDmBuffer();
        }

        private void pnlGFX_DragDrop(object sender, DragEventArgs e)
        {
            if (MidsContext.Config.ColumnStackingMode != Enums.eColumnStacking.None)
            {
                return;
            }

            if (!sender.Equals(pnlGFX))
            {
                return;
            }

            pnlGFX.AllowDrop = false;
            ControlPaint.DrawReversibleFrame(dragRect, Color.White, FrameStyle.Thick);
            oldDragRect = Rectangle.Empty;
            dragRect = Rectangle.Empty;
            var iValue1 = e.X + xCursorOffset;
            var iValue2 = e.Y + yCursorOffset;
            dragFinishPower = drawing.WhichSlot(iValue1, iValue2);
            if (dragStartSlot != -1)
            {
                dragFinishSlot = drawing.WhichEnh(iValue1, iValue2);
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

        private void pnlGFX_DragEnter(object sender, DragEventArgs e)
        {
            if (MidsContext.Config.ColumnStackingMode != Enums.eColumnStacking.None)
            {
                return;
            }

            e.Effect = sender.Equals(pnlGFX) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void pnlGFX_DragOver(object sender, DragEventArgs e)
        {
            if (MidsContext.Config.ColumnStackingMode != Enums.eColumnStacking.None)
            {
                return;
            }

            Point position;
            int num1;
            if (sender.Equals(pnlGFX))
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

        private void pnlGFX_MouseDoubleClick(object sender, MouseEventArgs e)
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

        private void pnlGFX_MouseDown(object sender, MouseEventArgs e)
        {
            if (MidsContext.EnhCheckMode)
            {
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            pnlGFX.AllowDrop = true;
            dragStartX = e.X;
            dragStartY = e.Y;
            dragStartPower = drawing.WhichSlot(e.X, e.Y);
            dragStartSlot = drawing.WhichEnh(e.X, e.Y);
        }

        private void pnlGFX_MouseLeave(object sender, EventArgs e)
        {
            HidePopup();
            drawing?.HighlightSlot(-1);
        }

        private void pnlGFX_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left & pnlGFX.AllowDrop && Math.Abs(e.X - dragStartX) + Math.Abs(e.Y - dragStartY) > 7)
            {
                if (dragStartSlot == 0)
                {
                    MessageBox.Show(this, "You cannot change the level of any power's automatic slot.", null, MessageBoxButtons.OK);
                    pnlGFX.AllowDrop = false;
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
                    pnlGFX.Cursor = Cursors.Default;
                    drawing?.HighlightSlot(-1);
                    Application.DoEvents();
                    ibPopupEx.DoDragDrop(dataObject, DragDropEffects.Move);
                }
            }
            else
            {
                if (drawing == null)
                {
                    return;
                }

                var index = drawing.WhichSlot(e.X, e.Y);
                var sIDX = drawing.WhichEnh(e.X, e.Y);
                if (index < 0 | index >= MidsContext.Character.CurrentBuild.Powers.Count)
                {
                    HidePopup();
                }
                else
                {
                    var e1 = new Point(e.X + 10, e.Y + 10);
                    ShowPopup(index, -1, sIDX, e1, new Rectangle());
                    if (MidsContext.Character.CanPlaceSlot & MainModule.MidsController.Toon.SlotCheck(MidsContext.Character.CurrentBuild.Powers[index]) > -1)
                    {
                        drawing.HighlightSlot(index);
                        pnlGFX.Cursor = index > -1 & drawing.InterfaceMode != Enums.eInterfaceMode.PowerToggle
                            ? Cursors.Hand
                            : Cursors.Default;
                    }
                    else
                    {
                        pnlGFX.Cursor = Cursors.Default;
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

        private void pnlGFX_MouseUp(object sender, MouseEventArgs e)
        {
            pnlGFX.AllowDrop = false;

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
                            break;

                        case Enums.eToggleType.Proc:
                            powerEntry.ProcInclude = !powerEntry.ProcInclude;
                            RedrawSinglePower(ref powerEntry, true, true);
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
                    _dvAnchored.PetInfo.ExecuteUpdate();
                    return;
                }

                // Standard Left Click (Add Slot or Select Power)
                if (EnhPickerActive) return;

                if (!isPowerChosen && powerEntry.Level > -1)
                {
                    MainModule.MidsController.Toon.RequestedLevel = powerEntry.Level;
                    UpdatePowerLists();
                    DoRedraw();
                }
                else if (MainModule.MidsController.Toon.BuildSlot(hIDPower) > -1)
                {
                    PowerModified(false); // Adding a slot doesn't modify the build file until an enh is placed
                    LastClickPlacedSlot = true;
                    MidsContext.Config.Tips.Show(Tips.TipType.FirstSlot);
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
                            (int)Math.Round(pnlGFXFlow.Left - pnlGFXFlow.HorizontalScroll.Value + e.X - I9Picker.Width / 2f),
                            (int)Math.Round(pnlGFXFlow.Top - pnlGFXFlow.VerticalScroll.Value + e.Y - I9Picker.Height / 2f));

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
                pnlGFX.Invalidate();
            }
        }

        private void pnlGFX_Paint(object sender, PaintEventArgs e)
        {
            if (drawing == null || drawing.BxBuffer?.Bitmap == null)
            {
                return;
            }

            // This is the correct place to draw the buffer to the screen.
            // e.Graphics is the safe, persistent graphics object provided by the system.
            e.Graphics.DrawImageUnscaled(drawing.BxBuffer.Bitmap, Point.Empty);
        }

        private void pnlGFX_Resize(object sender, EventArgs e)
        {
            UpdateUILayout();
        }

        private void pnlGFXFlow_Scroll(object sender, ScrollEventArgs e)
        {
            var delta = e.NewValue - e.OldValue;
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll & delta > 0)
            {
                pnlGFXFlow.Refresh();
            }
        }

        private void pnlGFXFlow_MouseEnter(object sender, EventArgs e)
        {
            pnlGFXFlow.Focus();
        }

        private void pnlGFXFlow_Resize(object sender, EventArgs e)
        {
            pnlGFX.Width = pnlGFXFlow.ClientSize.Width - pnlGFXFlow.Padding.Horizontal - 12;
        }

        private void OnTitleUpdate(object? sender, EventArgs eventArgs)
        {
            SetTitleBar(MidsContext.Character.IsHero());
        }

        private void tsEnhToDO_Click(object sender, EventArgs e)
        {
            OnGradePick(Enums.eEnhGrade.DualO);
        }

        private void tsEnhToSO_Click(object sender, EventArgs e)
        {
            OnGradePick(Enums.eEnhGrade.SingleO);
        }

        private void tsEnhToTO_Click(object sender, EventArgs e)
        {
            OnGradePick(Enums.eEnhGrade.TrainingO);
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

        private void tsShareLegacy_Click(object sender, EventArgs e)
        {
            MessageBoxEx message;
            if (Path.GetExtension(LastFileName)?.ToUpperInvariant() != ".MXD")
            {
                if (MidsContext.Character.CurrentBuild.PowersPlaced > 0)
                {
                    message = new MessageBoxEx("Share Protection Activated", "You must save the build as an mxd prior to using this function.", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error);
                    message.ShowDialog(this);
                }
                else
                {
                    message = new MessageBoxEx("Share Protection Activated", "Your cannot share an otherwise empty build. Please create a build then save it as an mxd prior to using this function.", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error);
                    message.ShowDialog(this);
                }
            }
            else
            {
                var data = MidsCharacterFileFormat.MxDBuildSaveHyperlink(true, false, false);
                Clipboard.SetDataObject(data, true);
                message = new MessageBoxEx("Success", "The data-link has been successfully generated and added to your clipboard.", MessageBoxEx.MessageBoxExButtons.Ok);
                message.ShowDialog(this);
            }
        }

        private void tsImportChunk_Click(object? sender, EventArgs e)
        {
            var loaded = false;
            using var importBuild = new ImportCode();
            var result = importBuild.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                DataViewLocked = false;
                NewToon(true, true);
                loaded = _buildManager.ValidateAndLoadImportData(importBuild.ImportClassificationResult);
            }

            if (!loaded)
            {
                return;
            }

            FileModified = false;
            LastFileName = "";
            SetTitleBar();
            SetLockedPoolsState();
            if (drawing != null)
            {
                drawing.Highlight = -1;
            }

            myDataView?.Clear();
            PowerModified(false);
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

            myDataView?.Clear();
            PowerModified(false);
        }

        private void tsFileNew_Click(object sender, EventArgs e)
        {
            command_New();
        }

        private void tsBuildRcv_Click(object sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon.Locked & FileModified)
            {
                FloatTop(false);
                var msgBoxResult = MessageBox.Show("Current hero/villain data will be discarded, are you sure?",
                    "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

            //DoOpen(DlgOpen.FileName);
            BuildRecover(DlgOpen.FileName);
            FloatTop(true);
        }

        private void tsFileOpen_Click(object sender, EventArgs e)
        {
            /*if (DlgOpen == null)
            {
                MessageBox.Show("MainWindow.tsFileOpen_Click(): DlgOpen is null");

                return;
            }*/

            if (MainModule.MidsController.Toon?.Locked == true & FileModified)
            {
                FloatTop(false);
                var msgBoxResult = MessageBox.Show("Current hero/villain data will be discarded, are you sure?",
                    "Question",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                    DoOpen(legacyBuild);
                    break;

                case var newBuild when DlgOpen.FileName.EndsWith(".mbd"):
                    LoadCharacterFile(newBuild);
                    break;

                default:
                    if (DlgOpen.FileName.EndsWith(".txt"))
                    {
                        DoOpen(DlgOpen.FileName);
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

            pnlGFX.Update();
            pnlGFX.Refresh();
        }

        private void tsFilePrint_Click(object sender, EventArgs e)
        {
            new frmPrint().ShowDialog(this);
        }

        private void tsFileQuit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tsFileSave_Click(object sender, EventArgs e)
        {
            DoSave();
        }

        private void tsFileSaveAs_Click(object sender, EventArgs e)
        {
            DoSaveAs();
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

        private void tsFlipAllEnh_Click(object sender, EventArgs e)
        {
            MainModule.MidsController.Toon.FlipAllSlots();
            DoRedraw();
            RefreshInfo();
            FloatUpdate();
        }

        private void tsHelperLong_Click(object sender, EventArgs e)
        {
            new FrmInputLevel(this, true).ShowDialog(this);
        }

        private void tsHelperShort_Click(object sender, EventArgs e)
        {
            new FrmInputLevel(this, false).ShowDialog(this);
        }

        private void tsImport_Click(object sender, EventArgs e)
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
                        myDataView.Clear();
                        PowerModified(true);
                        UpdateControls(true, true);
                    }
                    else
                    {
                        NewToon();
                        myDataView.Clear();
                        PowerModified(true);
                    }

                    GetBestDamageValues();
                    if (drawing != null)
                    {
                        DoRedraw();
                    }

                    UpdateColors();
                    FloatTop(true);
                    SetTitleBar(MidsContext.Character.IsHero(), true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Message: {ex.Message}\r\nTrace: {ex.StackTrace}");
                FloatTop(true);
            }
        }

        private void tsIODefault_Click(object sender, EventArgs e)
        {
            if (MidsContext.Character.CurrentBuild.SetIOLevels(MidsContext.Config.I9.DefaultIOLevel, false, false))
            {
                //I9Picker.LastLevel = MidsContext.Config.I9.DefaultIOLevel + 1;
            }

            DoRedraw();
        }

        private void tsIOMax_Click(object sender, EventArgs e)
        {
            if (MidsContext.Character.CurrentBuild.SetIOLevels(MidsContext.Config.I9.DefaultIOLevel, false, true))
            {
                //I9Picker.LastLevel = 50;
            }

            DoRedraw();
        }

        private void tsIOMin_Click(object sender, EventArgs e)
        {
            if (MidsContext.Character.CurrentBuild.SetIOLevels(MidsContext.Config.I9.DefaultIOLevel, true, false))
            {
                //I9Picker.LastLevel = 10;
            }

            DoRedraw();
        }

        private void tsRecipeViewer_Click(object sender, EventArgs e)
        {
            FloatRecipe(true);
        }

        private void tsRotationHelper_Click(object sender, EventArgs e)
        {
            FloatRotationHelper(true);
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

        private void tsSetFind_Click(object sender, EventArgs e)
        {
            if (_setInspector == null || _setInspector.IsDisposed)
            {
                _setInspector = new SetInspector(this);
                _setInspector.FormClosing += (_, _) => _setInspector.Dispose(); // Dispose when closing
            }

            _setInspector.Show();
        }

        private void Github_Link(object? sender, EventArgs e)
        {
            SupportSites.GoToGitHub();
        }

        private async void tsUpdateCheck_Click(object sender, EventArgs e)
        {
            await UpdateCoordinator.CheckAndHandleUpdatesAsync(this, true);
        }

        private void EditBuildComment_Click(object sender, EventArgs e)
        {
            if (MidsContext.Character?.CurrentBuild?.PowersPlaced <= 0)
            {
                var errorMsg = new MessageBoxEx("Error", "You cannot add a comment/description to an otherwise empty build.", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Protected, true);
                errorMsg.ShowDialog(this);

                return;
            }

            using var editor = new frmEditComment();
            editor.ShowDialog(this);
        }

        private void EditBuildComment_MouseEnter(object sender, EventArgs e)
        {
            EditBuildCommentMenuItem.ForeColor = Color.FromArgb(5, 177, 255);
            EditBuildCommentMenuItem.IconColor = Color.Gold;
        }

        private void EditBuildComment_MouseLeave(object sender, EventArgs e)
        {
            EditBuildCommentMenuItem.ForeColor = Color.WhiteSmoke;
            EditBuildCommentMenuItem.IconColor = Color.WhiteSmoke;
        }

        private void tsView2Col_Click(object sender, EventArgs e)
        {
            tsView3Col.Checked = false;
            tsView4Col.Checked = false;
            tsView5Col.Checked = false;
            tsView6Col.Checked = false;
            tsView2Col.Checked = true;
            tsView3ColV.Checked = false;
            tsView3ColH.Checked = false;
            SetColumns(2);
        }

        private void tsView3Col_Click(object sender, EventArgs e)
        {
            tsView2Col.Checked = false;
            tsView4Col.Checked = false;
            tsView5Col.Checked = false;
            tsView6Col.Checked = false;
            tsView3Col.Checked = true;
            tsView3ColV.Checked = false;
            tsView3ColH.Checked = false;
            SetColumns(3);
        }

        private void tsView4Col_Click(object sender, EventArgs e)
        {
            tsView2Col.Checked = false;
            tsView3Col.Checked = false;
            tsView5Col.Checked = false;
            tsView6Col.Checked = false;
            tsView4Col.Checked = true;
            tsView3ColV.Checked = false;
            tsView3ColH.Checked = false;
            SetColumns(4);
        }

        private void tsView5Col_Click(object sender, EventArgs e)
        {
            tsView2Col.Checked = false;
            tsView3Col.Checked = false;
            tsView4Col.Checked = false;
            tsView6Col.Checked = false;
            tsView5Col.Checked = true;
            tsView3ColV.Checked = false;
            tsView3ColH.Checked = false;
            SetColumns(5);
        }

        private void tsView6Col_Click(object sender, EventArgs e)
        {
            tsView2Col.Checked = false;
            tsView3Col.Checked = false;
            tsView4Col.Checked = false;
            tsView5Col.Checked = false;
            tsView6Col.Checked = true;
            tsView3ColV.Checked = false;
            tsView3ColH.Checked = false;
            SetColumns(6);
        }

        private void tsView3ColV_Click(object sender, EventArgs e)
        {
            tsView2Col.Checked = false;
            tsView3Col.Checked = false;
            tsView4Col.Checked = false;
            tsView5Col.Checked = false;
            tsView6Col.Checked = false;
            tsView3ColV.Checked = true;
            tsView3ColH.Checked = false;
            SetColumns(3, Enums.eColumnStacking.Vertical);
        }

        private void tsView3ColH_Click(object sender, EventArgs e)
        {
            tsView2Col.Checked = false;
            tsView3Col.Checked = false;
            tsView4Col.Checked = false;
            tsView5Col.Checked = false;
            tsView6Col.Checked = false;
            tsView3ColV.Checked = false;
            tsView3ColH.Checked = true;
            SetColumns(3, Enums.eColumnStacking.Horizontal);
        }

        private void tsViewActualDamage_New_Click(object sender, EventArgs e)
        {
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.Numeric;
            SetDamageMenuCheckMarks();
            DisplayFormatChanged();
        }

        private void tsViewData_Click(object sender, EventArgs e)
        {
            FloatData(true);
        }

        private void tsViewDPS_New_Click(object sender, EventArgs e)
        {
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.DPS;
            SetDamageMenuCheckMarks();
            DisplayFormatChanged();
        }

        private void tsViewGraphs_Click(object sender, EventArgs e)
        {
            FloatStatGraph(true);
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

        private void tsViewSetCompare_Click(object sender, EventArgs e)
        {
            FloatCompareGraph(true);
        }

        private void tsViewSets_Click(object sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon == null)
            {
                return;
            }

            FloatSets(true);
        }

        private void tsViewSlotLevels_Click(object sender, EventArgs e)
        {
            MidsContext.Config.ShowSlotLevels = !MidsContext.Config.ShowSlotLevels;
            tsViewSlotLevels.Checked = MidsContext.Config.ShowSlotLevels;
            ibSlotLevelsEx.ToggleState = MidsContext.Config.ShowSlotLevels switch
            {
                true => MidsVectorButton.States.ToggledOn,
                false => MidsVectorButton.States.ToggledOff
            };

            DoRedraw();
        }

        private void tsViewTotals_Click(object sender, EventArgs e)
        {
            FloatTotals(true, MidsContext.Config is { UseOldTotalsWindow: true });
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
            {
                return;
            }

            if (MidsContext.Character != null)
            {
                MidsContext.Character.Name = txtName.Text;
            }

            if (fTotals2 != null)
            {
                frmTotalsV2.SetTitle(fTotals2);
            }

            DisplayName();
        }

        private void TemporaryPowersWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ibTempPowersEx_OnClick(sender, e);
        }

        private void ibTempPowersEx_OnClick(object? sender, EventArgs e)
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

        private void tlsDPA_Click(object sender, EventArgs e)
        {
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.DPA;
            SetDamageMenuCheckMarks();
            DisplayFormatChanged();
        }

        private void tmrGfx_Tick(object? sender, EventArgs e)
        {
            if (FlipActive)
            {
                DoFlipStep();
            }
        }

        private bool ToggleClicked(int hID, int iX, int iY)
        {
            var rectangle1 = new Rectangle();
            if (hID < 0)
            {
                return false;
            }

            if (MidsContext.Character.CurrentBuild.Powers[hID].IDXPower < 0)
            {
                return false;
            }

            var rectangle2 = new Rectangle
            {
                Location = drawing.PowerPosition(MidsContext.Character.CurrentBuild.Powers[hID]),
                Size = drawing.BxPower[0].Size
            };
            rectangle1.Height = 15;
            rectangle1.Width = rectangle1.Height;
            rectangle1.Y = (int)Math.Round(rectangle2.Top + (rectangle2.Height - rectangle1.Height) / 2.0);
            rectangle1.X =
                (int)Math.Round(rectangle2.Right - (rectangle1.Width + (rectangle2.Height - rectangle1.Height) / 2.0));
            return (iX > rectangle1.X) & (iX < rectangle1.Right) & (iY > rectangle1.Top) & (iY < rectangle1.Bottom);
        }

        private bool ProcToggleClicked(int hID, int iX, int iY)
        {
            var rectangle1 = new Rectangle();
            if (hID < 0)
            {
                return false;
            }

            if (MidsContext.Character.CurrentBuild.Powers[hID].IDXPower < 0)
            {
                return false;
            }

            var rectangle2 = new Rectangle
            {
                Location = drawing.PowerPosition(MidsContext.Character.CurrentBuild.Powers[hID]),
                Size = drawing.BxPower[0].Size
            };
            rectangle1.Height = 15;
            rectangle1.Width = rectangle1.Height;
            rectangle1.Y = (int)Math.Round(rectangle2.Top + (rectangle2.Height - rectangle1.Height) / 2.0);
            rectangle1.X = (int)Math.Round(rectangle2.Right - (rectangle1.Width + (rectangle2.Height - rectangle1.Height) / 1.0));
            return (iX > rectangle1.X) & (iX < rectangle1.Right) & (iY > rectangle1.Top) & (iY < rectangle1.Bottom);
        }

        private void tsAdvDBEdit_Click(object sender, EventArgs e)
        {
            FloatTop(false);
            using var frmDbEdit = new frmDBEdit();
            frmDbEdit.ShowDialog(this);
            FloatTop(true);
        }

        private void tsAdvFreshInstall_Click(object sender, EventArgs e)
        {
            FloatTop(false);
            if (!MidsContext.Config.IsInitialized)
            {
                MidsContext.Config.IsInitialized = true;
                MidsContext.Config.SaveFolderChecked = true;
                MessageBox.Show(@"Fresh Install flag has been unset!", null, MessageBoxButtons.OK);
            }
            else
            {
                MidsContext.Config.IsInitialized = false;
                MidsContext.Config.SaveFolderChecked = false;
                MessageBox.Show(@"Fresh Install flag has been set!", null, MessageBoxButtons.OK);
            }

            tsAdvFreshInstall.Checked = !MidsContext.Config.IsInitialized;
            FloatTop(true);
        }

        private void tsAdvResetTips_Click(object sender, EventArgs e)
        {
            MidsContext.Config.Tips = new Tips();
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

        private async void tsChangeDb_Click(object sender, EventArgs e)
        {
            using var dbSelector = new DatabaseSelector();
            var result = dbSelector.ShowDialog();
            if (result != DialogResult.OK)
            {
                return;
            }

            var dbSelected = dbSelector.SelectedDatabase;
            MidsContext.Config.DataPath = dbSelected;
            MidsContext.Config.SavePath = dbSelected;
            MidsContext.Config.SaveConfig();
            using var iFrm = new frmBusy();
            _frmBusy = iFrm;
            _frmBusy.SetTitle(@"Changing Database");
            _frmBusy.Show();
            await MainModule.MidsController.ChangeDatabase(_frmBusy);
        }

        private void tsConfig_Click(object sender, EventArgs e)
        {
            FloatTop(false);
            var iParent = this;
            var frmCalcOpt = new frmCalcOpt(ref iParent);
            if (frmCalcOpt.ShowDialog(this) == DialogResult.OK)
            {
                MidsContext.Config.SaveConfig();
                UpdateControls();
                UpdateOtherFormsFonts();
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

        private void tsKoFi_Click(object? sender, EventArgs e)
        {
            SupportSites.KoFi();
        }

        private void tsPatreon_Click(object? sender, EventArgs e)
        {
            SupportSites.Patreon();
        }

        private void tsSupport_Click(object sender, EventArgs e)
        {
            SupportSites.SupportServer();
        }

        private void tsAbout_Click(object sender, EventArgs e)
        {
            using var frmAbout = new frmAbout();
            frmAbout.ShowDialog();
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
            }

            info_Totals();
            DoRedraw();
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

        private void CharacterOnAlignmentChanged(object? sender, Enums.Alignment e)
        {
            // var buttonControls = Helpers.GetControlsOfType<MidsVectorButton>(this);
            // foreach (var button in buttonControls)
            // {
            //     button.Alignment = e switch
            //     {
            //         Enums.Alignment.Hero => Enums.Alignment.Hero,
            //         Enums.Alignment.Villain => Enums.Alignment.Villain,
            //         _ => button.Alignment
            //     };
            // }

            if (FrmEntityDetails is { Visible: true })
            {
                FrmEntityDetails.UpdateColorTheme(e);
            }

            if (fGraphStats is { Visible: true })
            {
                fGraphStats.UpdateColorTheme(e);
            }

            if (fRotationHelper is { Visible: true })
            {
                fRotationHelper.UpdateColorTheme(e);
            }
        }

        private void IncarnateWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ibIncarnatesEx_OnClick(sender, EventArgs.Empty);
        }

        private void AccoladesWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ibAccoladesEx_OnClick(sender, EventArgs.Empty);
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

        private void tsToggleCheckModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MidsContext.EnhCheckMode = !MidsContext.EnhCheckMode;
            if (fRecipe is { Visible: true })
            {
                fRecipe.UpdateData();
            }

            _enhCheckMode.Visible = MidsContext.EnhCheckMode;

            //FloatBuildSalvageHud(MidsContext.EnhCheckMode);
            DoRedraw();
        }

        #endregion

        private void InitializePopup()
        {
            _popup = new PopUpDisplay
            {
                BackColor = Color.Black,
                BxHeight = 675,
                ColumnPosition = 0.5f,
                ColumnRight = false,
                Font = new Font("Segoe UI", 12f, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = Color.FromArgb(96, 48, 255),
                InternalPadding = 3,
                Location = new Point(513, 490),
                Name = "Popup",
                ScrollY = 0f,
                SectionPadding = 8,
                Size = new Size(450, 203),
                TabIndex = 102,
                Visible = false
            };

            _popup.MouseMove += Popup_MouseMove;

            Controls.Add(_popup);
            _popup.BringToFront();
        }

        private void InitializePicker()
        {
            _i9Picker = new I9Picker
            {
                BackColor = Color.Black,
                ForeColor = Color.Blue,
                //Highlight = Color.MediumSlateBlue,
                //ImageSize = 30,
                Location = new Point(20, 20),
                Name = "i9Picker",
                //Selected = Color.SlateBlue,
                //Size = new Size(250, 400),
                TabIndex = 83,
                Visible = false
            };

            _i9Picker.EnhancementPicked += I9Picker_EnhancementPicked;
            _i9Picker.EnhancementSelectionCancelled += I9Picker_EnhancementSelectionCancelled;
            _i9Picker.HoverEnhancement += I9Picker_HoverEnhancement;
            _i9Picker.HoverSet += I9Picker_HoverSet;
            _i9Picker.Moved += I9Picker_Moved;
            _i9Picker.MouseDown += I9Picker_MouseDown;
            _i9Picker.MouseEnter += I9Picker_MouseEnter;
            _i9Picker.MouseLeave += I9Picker_MouseLeave;
            _i9Picker.KeyDown += I9Picker_KeyDown;

            Controls.Add(_i9Picker);
            _i9Picker.BringToFront();
        }

        private void InitializeDataView()
        {
            Info_Power(llPrimary.Items[0].NIdPower);
        }

        private void AddNonStandardControls()
        {
            _enhCheckMode = new EnhCheckMode(this)
            {
                Location = new Point(5, 825),
                Name = "enhCheckMode",
                Size = new Size(445, 35),
                Visible = false
            };

            _dvAnchored = new DataView
            {
                BackColor = Color.Black,
                DrawVillain = false,
                Floating = false,
                Font = new Font(Fonts.Family("Noto Sans"), 10.25f, FontStyle.Regular, GraphicsUnit.Pixel, 0),
                Location = new Point(3, 3),
                Name = "dvAnchored",
                Size = new Size(300, 400),
                TabIndex = 69,
                VisibleSize = Enums.eVisibleSize.Full
            };
            _dvAnchored.MouseWheel += MainWindow_MouseWheel;
            _dvAnchored.SizeChange += dvAnchored_SizeChange;
            _dvAnchored.FloatChange += dvAnchored_Float;
            _dvAnchored.UnlockClick += dvAnchored_Unlock;
            _dvAnchored.SlotUpdate += DataView_SlotUpdate;
            _dvAnchored.SlotFlip += DataView_SlotFlip;
            _dvAnchored.Moved += dvAnchored_Move;
            _dvAnchored.TabChanged += dvAnchored_TabChanged;
            _dvAnchored.EntityDetails += dvAnchored_EntityDetails;

            //dataPanel.Controls.Add(_dvAnchored);
            //dataPanel.Controls.Add(_enhCheckMode);
        }

        private void UpdateUILayout()
        {
            // Guard against running before everything is initialized.
            if (drawing == null || IsDisposed || ClientSize.Width == 0)
            {
                return;
            }

            const float designReferenceDrawWidth = 847f;

            // This is the intensity of the scaling. 1.0f = 100% (aggressive), 0.5f = 50% (subtle).
            // You can tune this value to get the exact feel you want.
            const float scalingIntensity = 0.5f;

            // Calculate the raw scale based on the panel's width
            float rawScale = pnlGFXFlow.ClientSize.Width / designReferenceDrawWidth;

            // Dampen the scaling effect using the intensity factor
            drawing.MasterScale = 1.0f + (rawScale - 1.0f) * scalingIntensity;

            drawing.UpdateFontScale(drawing.MasterScale);

            // Step 1: Manually set the drawing panel's width to match its container.

            pnlGFX.Width = pnlGFXFlow.ClientSize.Width - pnlGFXFlow.Padding.Horizontal - 12;

            // Step 2: Tell the drawing engine to calculate its layout based on this correct width.
            drawing.UpdateLayout(pnlGFX.Width);

            // Step 3: Set the inner panel's HEIGHT to be the full required height of the content.
            pnlGFX.Height = drawing.GetRequiredDrawingArea().Height;

            // Step 4: Re-initialize the drawing engine's buffer with the correct dimensions.
            drawing.ReInit(pnlGFX);

            // Step 5: Force the scroll panel to reset its view to the top. THIS IS THE KEY FIX.
            pnlGFXFlow.ScrollToTop();

            // Step 6: Invalidate the container to trigger the final Paint event.
            pnlGFXFlow.Invalidate();
        }

        public bool PetWindowFlag { get; set; }

        private List<string> MmPets { get; set; } = new();

        // store the instance for reuse, as these things are called per draw/redraw

        public int GetPrimaryBottom()
        {
            return cbPrimary.Top + cbPrimary.Height;
        }

        public string? GetBuildFile(bool stripExt = false)
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

        private ComboBoxT<string> GetCbOrigin()
        {
            return new ComboBoxT<string>(cbOrigin);
        }

        private void SetLockVisibility(bool visible, string? nameFilter = null)
        {
            var lblLocks = Helpers.GetControlsOfType<Label>(this).Where(l => l.Name.Contains("lblLocked"));

            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                lblLocks = lblLocks.Where(l => l.Name.Equals(nameFilter, StringComparison.OrdinalIgnoreCase));
            }

            foreach (var lbl in lblLocks) lbl.Visible = visible;
        }

        private async Task<bool> RunSchemaCommands(string url)
        {
            var returnData = false;
            var code = url.Replace("mrb://", "");
            var options = new RestClientOptions("https://api.midsreborn.com")
            {
                Timeout = TimeSpan.FromSeconds(30),
            };
            var client = new RestClient(options);
            var response = await client.GetJsonAsync<SchemaData>($"build/redirect-to-schema/{code}");
            if (response != null)
            {
                returnData = DoLoadFromSchema(response);
            }
            return returnData;
        }

        internal void ChildRequestedRedraw()
        {
            DoRedraw();
        }

        public void UpdateWindowStyle()
        {
            CharacterOnAlignmentChanged(null, MidsContext.Character == null ? Enums.Alignment.Hero : MidsContext.Character.Alignment);
        }

        private void UpdateModeInfo()
        {
            switch (MidsContext.Config.BuildMode)
            {
                case Enums.dmModes.LevelUp:
                    ibModeEx.ToggleText.ToggledOff = MainModule.MidsController.Toon is { Complete: false }
                        ? $"Level-Up: {MidsContext.Character?.Level + 1}"
                        : @"Level-Up";
                    if (ibModeEx.Text != ibModeEx.ToggleText.ToggledOff)
                    {
                        ibModeEx.Text = MainModule.MidsController.Toon is { Complete: false }
                            ? $"Level-Up: {MidsContext.Character?.Level + 1}"
                            : @"Level-Up";
                    }

                    ibModeEx.ToggleState = MidsVectorButton.States.ToggledOff;
                    break;
                case Enums.dmModes.Normal:
                    ibModeEx.ToggleText.ToggledOn = @"Normal";
                    ibModeEx.ToggleState = MidsVectorButton.States.ToggledOn;
                    break;
                case Enums.dmModes.Respec:
                    ibModeEx.ToggleText.Indeterminate = @"Respec";
                    ibModeEx.ToggleState = MidsVectorButton.States.Indeterminate;
                    break;
                case Enums.dmModes.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static int ArchetypeIndirectToIndex(int iIndirect)
        {
            var num1 = -1;
            for (var index = 0; index < DatabaseAPI.Database.Classes.Length; index++)
            {
                if (!DatabaseAPI.Database.Classes[index].Playable)
                {
                    continue;
                }

                ++num1;
                if (num1 == iIndirect)
                {
                    return index;
                }
            }

            return 0;
        }

        private void AssemblePowerList(ListLabel llPower, IPowerset? iPowerset)
        {
            if (iPowerset == null || iPowerset.Powers?.Length < 1)
            {
                llPower.SuspendRedraw = true;
                llPower.ClearItems();
                llPower.SuspendRedraw = false;
            }
            else
            {
                llPower.SuspendRedraw = true;
                llPower.ClearItems();
                string message;
                if (iPowerset.nIDTrunkSet > -1)
                {
                    var powerset = DatabaseAPI.Database.Powersets[iPowerset.nIDTrunkSet];
                    var iItem1 = new ListLabel.ListLabelItem(powerset.DisplayName, ListLabel.LlItemState.Heading, iPowerset.nIDTrunkSet, -1, -1, "", ListLabel.LlFontFlags.Bold, ListLabel.LlTextAlign.Center);
                    llPower.AddItem(iItem1);
                    for (var iIDXPower = 0; iIDXPower < powerset.Powers.Length; iIDXPower++)
                    {
                        if (powerset.Powers[iIDXPower].Level <= 0)
                        {
                            continue;
                        }

                        message = "";
                        // var iItem2 = new ListLabel.ListLabelItem(iText: powerset.Powers[iIDXPower].DisplayName, iState: MainModule.MidsController.Toon.PowerState(powerset.Powers[iIDXPower].PowerIndex, ref message), inIdSet: iPowerset.nIDTrunkSet, iIdxPower: iIDXPower, inIdPower: powerset.Powers[iIDXPower].PowerIndex, iStringTag: "", iFont: ListLabel.LlFontFlags.Bold)
                        // {
                        //     Bold = MidsContext.Config.RtFont.PairedBold
                        // };
                        // if (iItem2.ItemState == ListLabel.LlItemState.Invalid)
                        // {
                        //     iItem2.Italic = true;
                        // }
                        //
                        // llPower.AddItem(iItem2);
                    }

                    var iItem = new ListLabel.ListLabelItem(iPowerset.DisplayName, ListLabel.LlItemState.Heading, iPowerset.nID, -1, -1, "", ListLabel.LlFontFlags.Bold, ListLabel.LlTextAlign.Center);
                    llPower.AddItem(iItem);
                }

                if (iPowerset.Powers != null)
                {
                    for (var iIDXPower = 0; iIDXPower < iPowerset.Powers.Length; iIDXPower++)
                    {
                        if (iPowerset.Powers[iIDXPower].Level <= 0 || !iPowerset.Powers[iIDXPower].AllowedForClass(MidsContext.Character.Archetype.Idx))
                        {
                            continue;
                        }

                        message = "";
                        /*var targetPs = MainModule.MidsController.Toon.PowerState(iPowerset.Powers[iIDXPower].PowerIndex, ref message);
                        var power = iPowerset.Powers[iIDXPower];
                        var iItem = new ListLabel.ListLabelItem(iText: iPowerset.Powers[iIDXPower].DisplayName, iState: targetPs, inIdSet: iPowerset.nID, iIdxPower: iIDXPower, inIdPower: power.PowerIndex, iStringTag: "", iFont: ListLabel.LlFontFlags.Bold)
                        {
                            Bold = MidsContext.Config.RtFont.PairedBold
                        };
                        if (iItem.ItemState == ListLabel.LlItemState.Invalid)
                        {
                            iItem.Italic = true;
                        }

                        llPower.AddItem(iItem);*/
                    }
                }

                llPower.SuspendRedraw = false;
            }
        }

        private void SetEnhCheckModePosition()
        {
            _enhCheckMode.Location = _enhCheckMode.Location with { Y = Math.Max(llPrimary.Top + llPrimary.SizeNormal.Height + 431, poolsPanel.Top + llAncillary.Top + llAncillary.SizeNormal.Height + 35) };
        }

        private void ChangeSets()
        {
            MainUiLogic.ChangeSets(MainModule.MidsController.Toon, MidsContext.Character,
                cbPrimary.SelectedIndex,
                cbSecondary.SelectedIndex,
                cbPool0.SelectedIndex,
                cbPool1.SelectedIndex,
                cbPool2.SelectedIndex,
                cbPool3.SelectedIndex,
                cbAncillary.SelectedIndex,
                DatabaseAPI.GetPowersetIndexes,
                () => cbSecondary.Enabled = true
            );
            DataViewLocked = false;
            ActiveControl = llPrimary;
            PowerModified(true);
            FloatUpdate(true);
            GetBestDamageValues();
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

        private bool CloseCommand()
        {
            if (MainModule.MidsController.Toon == null)
            {
                return false;
            }

            if (!(MainModule.MidsController.Toon.Locked & FileModified))
            {
                return false;
            }

            FloatTop(false);
            var msgBoxResult = MessageBox.Show(@"Do you wish to save your build before closing?", @"Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            FloatTop(true);

            return msgBoxResult switch
            {
                DialogResult.Cancel => true,
                DialogResult.Yes => !DoSave(),
                _ => false
            };
        }

        private bool ComboCheckAT(Archetype?[] playableClasses)
        {
            var cbtAT = CbtAT.Value;
            if (cbtAT.Count != playableClasses.Length)
            {
                return true;
            }

            return playableClasses
                .Where((t, index) => cbtAT[index].Idx != t.Idx)
                .Any();
        }

        private bool ComboCheckOrigin()
        {
            var cbtOrigin = GetCbOrigin();
            if (cbtOrigin.Count != MidsContext.Character.Archetype.Origin.Length)
            {
                return true;
            }

            if (cbtOrigin.Count > 1)
            {
                return false;
            }

            return MidsContext.Character.Archetype.Origin
                .Where((t, index) => cbtOrigin[index] != t)
                .Any();
        }

        private static void ComboCheckPool(ComboBoxT<string> iCb, Enums.ePowerSetType iSetType)
        {
            var powersetNames = DatabaseAPI.GetPowersetNames(MidsContext.Character.Archetype.Idx, iSetType);
            var needsComboUpdate = iCb.Items.Count != powersetNames.Length || !iCb.Items.SequenceEqual(powersetNames);
            if (!needsComboUpdate)
            {
                return;
            }

            iCb.BeginUpdate();
            iCb.Clear();
            iCb.AddRange(powersetNames);
            iCb.EndUpdate();
        }

        private static void ComboCheckPS(ComboBoxT<string> iCb, Enums.PowersetType iSetId, Enums.ePowerSetType iSetType)
        {
            var powersetNames = DatabaseAPI.GetPowersetNames(MidsContext.Character.Archetype.Idx, iSetType);
            var needsComboUpdate = iCb.Items.Count != powersetNames.Length || !iCb.Items.SequenceEqual(powersetNames);
            if (needsComboUpdate)
            {
                iCb.BeginUpdate();
                iCb.Clear();
                iCb.AddRange(powersetNames);
                iCb.EndUpdate();
            }

            var powersetIndexes = DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, iSetType);
            iCb.SelectedIndex =
                DatabaseAPI.ToDisplayIndex(MidsContext.Character.Powersets[(int)iSetId], powersetIndexes);
        }

        private void command_New()
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

            DataViewLocked = false;
            NewToon(false);
            MidsContext.EnhCheckMode = false;
            if (fRecipe is { Visible: true })
            {
                fRecipe.UpdateData();
            }

            if (fSalvageHud is { Visible: true })
            {
                FloatBuildSalvageHud(false);
            }

            MidsContext.Config.LastFileName = "";
            LastFileName = "";
            PowerModified(false);
            FileModified = false;
            SetTitleBar();
            DoRedraw();
            myDataView.Clear();
        }

        private static PowerEntry?[] DeepCopyPowerList()
        {
            return MidsContext.Character.CurrentBuild.Powers.Select(x => (PowerEntry)x?.Clone()).ToArray();
        }

        private Rectangle Dilate(Rectangle iRect, int iAdd)
        {
            iRect.X -= iAdd;
            iRect.Y -= iAdd;
            iRect.Height += iAdd * 2;
            iRect.Width += iAdd * 2;

            return iRect;
        }

        private void DisplayFormatChanged()
        {
            GetBestDamageValues();
            RefreshInfo();
        }

        private void DisplayName()
        {
            var str1 = "";
            var str2 = "";
            var ch = MidsContext.Character;
            var level = ch.Level;
            if (!((Build.TotalSlotsAvailable - ch.CurrentBuild.SlotsPlaced < 1) & (ch.CurrentBuild.LastPower + 1 - ch.CurrentBuild.PowersPlaced < 1)) && ch.Level > 0)
            {
                str1 = $" (Placing {ch.Level + 1})";
            }

            SetTitleBar(MainModule.MidsController.Toon.IsHero());
            var str3 = $"{ch.Name}: ";
            if ((MidsContext.Config.BuildMode == Enums.dmModes.LevelUp) & (str1 != ""))
            {
                str3 += $"Level {level}{str1} ";
            }

            var str4 = $"{str3}{ch.Archetype.Origin[ch.Origin]} {ch.Archetype.DisplayName}";
            if (MainModule.MidsController.Toon.Locked)
            {
                var ch0name = ch.Powersets[0] == null ? "--" : ch.Powersets[0].DisplayName;
                var ch1name = ch.Powersets[1] == null ? "--" : ch.Powersets[1].DisplayName;
                str4 += $" ({ch0name} / {ch1name}){str2}";
            }

            if (MidsContext.Config.ExempLow < MidsContext.Config.ExempHigh)
            {
                str4 += $" - Exemped from {MidsContext.Config.ExempHigh} to {MidsContext.Config.ExempLow}";
            }

            lblCharacter.Text = str4;
            if (txtName.Text == ch.Name)
            {
                return;
            }

            txtName.Text = ch.Name;
        }

        private void DoFlipStep()
        {
            if (!FlipActive)
            {
                return;
            }

            var point1 = new Point();
            var currentBuild = MidsContext.Character.CurrentBuild;
            var power = currentBuild.Powers[FlipPowerID];
            var point2 = drawing.DrawPowerSlot(ref power);
            var index = -1;
            var Enh1 = -1;
            var Enh2 = -1;
            I9Slot? i9Slot1 = null;
            I9Slot? i9Slot2 = null;
            var recolorIa = BuildRenderer.GetRecolorIa(MainModule.MidsController.Toon.IsHero());
            using var solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
            var num1 = FlipSlotState.Length - 1;
            Rectangle rectangle1;
            var slotId = -1;
            for (var i = 0; i <= num1; ++i)
            {
                point1.X = (int)Math.Round(point2.X - 30 + (drawing.SzPower.Width - drawing.SzSlot.Width * 6) / 2.0);
                point1.Y = point2.Y + drawing.OffsetY;
                ++FlipSlotState[i];
                var num2 = 1f;
                var powerEntry = MidsContext.Character.CurrentBuild.Powers[FlipPowerID];
                var slot = powerEntry.Slots[i];
                slotId = i;
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

                rectangle1 = new Rectangle(point1.X + 30 * i, point1.Y, 30, 30);
                if (!(num2 > 0.0))
                {
                    continue;
                }

                var rectangle2 = new Rectangle((int)Math.Round(rectangle1.X + (30.0 - 30.0 * num2) / 2.0),
                    rectangle1.Y,
                    (int)Math.Round(30.0 * num2), 30);
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

                if (!((myDataView == null) | (i9Slot1 == null) | (i9Slot2 == null)))
                {
                    myDataView?.FlipStage(i, Enh1, Enh2, num2, powerEntry.NIDPower, i9Slot1.Grade, i9Slot2.Grade);
                }
            }

            rectangle1 = new Rectangle(point1.X - 1, point1.Y - 1, drawing.SzPower.Width + 1,
                drawing.SzSlot.Height + 1);
            pnlGFX.Invalidate(rectangle1);
            if (FlipSlotState[^1] >= FlipSteps)
            {
                EndFlip();
            }
        }

        private bool DoLoadFromSchema(SchemaData response)
        {
            DataViewLocked = false;
            NewToon(true, true);
            var ret = response.Data != null && _buildManager.ValidateAndLoadSchemaData(response.Data);
            FileModified = false;
            if (drawing != null)
            {
                drawing.Highlight = -1;
            }

            myDataView?.Clear();
            PowerModified(false);
            return ret;
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

        private bool LoadCharacterFile(string? fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            DataViewLocked = false;
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

            myDataView?.Clear();
            MidsContext.Character?.ResetLevel();
            PowerModified(false);
            SetLockedPoolsState();
            UpdateControls(true);
            SetTitleBar();
            Application.DoEvents();
            GetBestDamageValues();
            UpdateColors();
            DoRedraw();
            FloatUpdate(true);

            return true;
        }

        private bool DoOpen(string? fName)
        {
            if (!File.Exists(fName))
            {
                return false;
            }

            DataViewLocked = false;
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

            myDataView?.Clear();
            MidsContext.Character?.ResetLevel();
            PowerModified(false);
            UpdateControls(true);
            SetTitleBar();
            Application.DoEvents();
            GetBestDamageValues();
            SetEnhCheckModePosition();
            UpdateColors();
            FloatUpdate(true);

            return true;
        }

        private bool DoLoad(Stream? data)
        {
            DataViewLocked = false;
            NewToon(true, true);
            if (data == null)
            {
                return true;
            }

            var loaded = MainModule.MidsController.Toon != null && MainModule.MidsController.Toon.Load("", ref data);

            if (!loaded)
            {
                return true;
            }

            FileModified = false;
            if (drawing != null)
            {
                drawing.Highlight = -1;
            }

            NewDraw();
            myDataView.Clear();
            MidsContext.Character?.ResetLevel();
            PowerModified(true);
            UpdateControls(true);
            SetTitleBar();
            Application.DoEvents();
            GetBestDamageValues();
            UpdateColors();
            FloatUpdate(true);

            return true;
        }

        private bool DoLoad(string? data)
        {
            DataViewLocked = false;
            NewToon(true, true);
            if (data == null || (!data.Contains("MxDz") && !data.Contains("MxDu")))
            {
                return true;
            }

            Stream? mStream = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var loaded = MainModule.MidsController.Toon != null && MainModule.MidsController.Toon.Load("", ref mStream);

            if (!loaded)
            {
                return true;
            }

            FileModified = false;
            if (drawing != null)
            {
                drawing.Highlight = -1;
            }

            NewDraw();
            myDataView.Clear();
            MidsContext.Character?.ResetLevel();
            PowerModified(true);
            UpdateControls(true);
            SetTitleBar();
            Application.DoEvents();
            GetBestDamageValues();
            UpdateColors();
            FloatUpdate(true);

            return true;
        }

        private void command_Load(string? data)
        {
            FloatTop(false);
            FileModified = false;
            var loaded = false;
            NewToon();

            if (data != null && (data.Contains("MxDz") || data.Contains("MxDu")))
            {
                Stream? mStream = new MemoryStream(new ASCIIEncoding().GetBytes(data));
                loaded = MainModule.MidsController.Toon.Load("", ref mStream);
            }

            if (!loaded)
            {
                loaded = MainModule.MidsController.Toon.StringToInternalData(data);
            }

            if (loaded)
            {
                drawing.Highlight = -1;
                NewDraw();
                myDataView.Clear();
                PowerModified(true);
                UpdateControls(true);
                SetFormHeight();
            }
            else
            {
                NewToon();
                myDataView.Clear();
                PowerModified(true);
            }

            GetBestDamageValues();
            if (drawing != null)
            {
                DoRedraw();
            }

            UpdateColors();
            FloatTop(true);
            SetTitleBar();
        }

        public void DoRedraw()
        {
            if (drawing == null)
            {
                return;
            }

            // Tell the drawing class to update its internal buffer.
            drawing.FullRedraw();

            // Tell the panel to repaint itself from the updated buffer.
            pnlGFX.Invalidate();
        }

        private void DoResize(bool forceResize = false)
        {
            if (drawing == null)
            {
                return;
            }

            // The pnlGFX.Resize event already handles all the core logic.
            // This method can now just ensure a full redraw is triggered
            // when called from other places.
            drawing.FullRedraw();
        }

        public void DoRefresh()
        {
            pnlGFX.Refresh();
        }

        private bool DoSave()
        {
            if (string.IsNullOrEmpty(LastFileName))
            {
                return DoSaveAs();
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
            SetTitleBar();
            return true;
        }
        
        private bool DoSaveAs()
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
                SetTitleBar(MidsContext.Character.IsHero());

                return true;
            }

            FloatTop(true);

            return false;
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

        private void EndFlip()
        {
            FlipActive = false;
            tmrGfx.Enabled = false;
            FlipPowerID = -1;
            FlipSlotState = Array.Empty<int>();
            DoRedraw();
        }

        private void EnhancementModified()
        {
            DoRedraw();
            RefreshInfo();
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

        private void FixPrimarySecondaryHeight()
        {
            if (_dvAnchored.Visible & _dvAnchored.Bounds.IntersectsWith(_dvAnchored.SnapLocation))
            {
                var size = ClientSize;
                var height = size.Height - _dvAnchored.Height - cbPrimary.Top - cbPrimary.Height - 10;
                if (llPrimary.DesiredHeight < height)
                {
                    size = llPrimary.SizeNormal;
                    llPrimary.SizeNormal = size with { Height = llPrimary.DesiredHeight };
                }
                else
                {
                    if (height < 70)
                    {
                        height = 70;
                    }

                    size = llPrimary.SizeNormal with { Height = height };
                    llPrimary.SizeNormal = size;
                }

                if (llSecondary.DesiredHeight < height)
                {
                    size = llSecondary.SizeNormal with { Height = llSecondary.DesiredHeight };
                    llSecondary.SizeNormal = size;
                }
                else
                {
                    if (height < 70)
                    {
                        height = 70;
                    }

                    size = llSecondary.SizeNormal with { Height = height };
                    llSecondary.SizeNormal = size;
                }
            }
            else
            {
                var size = llPrimary.SizeNormal with { Height = llPrimary.DesiredHeight };
                llPrimary.SizeNormal = size;
                size = llSecondary.SizeNormal with { Height = llSecondary.DesiredHeight };
                llSecondary.SizeNormal = size;
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
                    pe.StatInclude = ibTempPowersEx.ToggleState switch
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

        internal void FloatSetFinder(bool show)
        {
            if (show)
            {
                fSetFinder ??= new frmSetFind(this);
                fSetFinder.Show();
                fSetFinder.Activate();
            }
            else
            {
                if (fSetFinder == null)
                {
                    return;
                }

                fSetFinder.Hide();
                fSetFinder.Dispose();
                fSetFinder = null;
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

        internal void DoCalcOptUpdates()
        {
            GetBestDamageValues();
            RefreshInfo();
            DisplayName();
            //I9Picker.LastLevel = MidsContext.Config.I9.DefaultIOLevel + 1;
            myDataView?.SetFontData();
            if (dvLastPower > -1)
            {
                Info_Power(dvLastPower, dvLastEnh, dvLastNoLev, DataViewLocked);
            }

            if (drawing != null)
            {
                DoRedraw();
            }

            UpdateColors();
            SetTitleBar();
            frmTotalsV2.SetTitle(fTotals2);
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

            myDataView.Info_Damage.nHighBase = highBase;
            myDataView.Info_Damage.nHighEnh = highEnh;
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

        private bool GetPlayableClasses(Archetype? a)
        {
            return a.Playable;
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

        private void HidePopup()
        {
            if (!PopUpVisible)
            {
                return;
            }

            PopUpVisible = false;
            var bounds = _popup.Bounds;
            bounds.X -= pnlGFXFlow.Left;
            bounds.Y -= pnlGFXFlow.Top;
            _popup.Visible = false;
            _popup.EIdx = -1;
            _popup.PIdx = -1;
            _popup.HIdx = -1;
            _popup.PsIdx = -1;
            ActivePopupBounds = new Rectangle(0, 0, 0, 0);
            pnlGFX.Invalidate(bounds);
        }

        private void Info_Enhancement(I9Slot? iEnh, int iLevel = -1)
        {
            myDataView.SetEnhancement(iEnh, iLevel);
        }

        internal void UnlockFloatingStats()
        {
            DataViewLocked = false;
            if (dvLastPower <= -1)
            {
                return;
            }

            Info_Power(dvLastPower, dvLastEnh, dvLastNoLev, DataViewLocked);
        }

        private void Info_Power(int powerIdx, int iEnhLvl = -1, bool noLevel = false, bool @lock = false)
        {
            if (!@lock & DataViewLocked)
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
            fData?.UpdateData(dvLastPower);
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

            DataViewLocked = @lock;
            if (powIndex > -1)
            {
                var basePower = MainModule.MidsController.Toon.GetBasePower(powIndex);
                var enhancedPower = MainModule.MidsController.Toon.GetEnhancedPower(powIndex);
                if (basePower != null && enhancedPower != null)
                {
                    myDataView.SetData(basePower, enhancedPower, noLevel, DataViewLocked, powIndex);
                }
                else
                {
                    myDataView.SetData(MainModule.MidsController.Toon.GetBasePower(powIndex, powerIdx), null, noLevel, DataViewLocked, powIndex);
                }
            }
            else
            {
                myDataView.SetData(MainModule.MidsController.Toon.GetBasePower(powIndex, powerIdx), null, noLevel, DataViewLocked, powIndex);
            }

            if (!@lock || _dvAnchored.Visible)
            {
                return;
            }

            FloatingDataForm.Activate();
        }

        private void info_Totals()
        {
            if ((MainModule.MidsController.Toon == null) | !MainModule.MidsController.IsAppInitialized)
            {
                return;
            }

            MainModule.MidsController.Toon?.GenerateBuffedPowerArray();
            myDataView.DisplayTotals();
            FloatUpdate();
        }

        private void MiniPaint(ref PaintEventArgs e, Enums.PowersetType iId)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[(int)iId] == null)
            {
                return;
            }

            var destRect = new Rectangle(1, (lblLockedPool0.Height - 16) / 2, 16, 16);
            destRect.Y--;

            // Use the helper method to get the individual powerset icon
            var powerset = MidsContext.Character.Powersets[(int)iId];
            var powersetImage = AssetManager.GetPowersetImage(powerset);

            if (powersetImage?.Bitmap != null)
            {
                var graphics = e.Graphics;
                graphics.DrawImage(powersetImage.Bitmap, destRect);
                destRect.X = lblLockedPool0.Width - 19;
                graphics.DrawImage(powersetImage.Bitmap, destRect);
            }
        }

        private void MovePopup(Rectangle rBounds)
        {
            if (!PopUpVisible)
            {
                return;
            }

            var bounds = _popup.Bounds;
            if (rBounds == bounds)
            {
                return;
            }

            SetPopupLocation(rBounds, false, true);
            RedrawUnderPopup(bounds);
        }

        private void NewDraw(bool skipDraw = false)
        {
            if (drawing == null)
            {
                drawing = new BuildRenderer(pnlGFX);
            }
            else
            {
                drawing.ReInit(pnlGFX);
            }

            pnlGFX.Image = drawing.BxBuffer.Bitmap;
            if (drawing != null)
            {
                drawing.Highlight = -1;
            }

            if (skipDraw)
            {
                return;
            }

            DoRedraw();
        }

        private void NewToon(bool init = true, bool skipDraw = false)
        {
            try
            {
                if (MainModule.MidsController.Toon == null)
                {
                    MainModule.MidsController.Toon = new clsToonX();
                }
                else if (init)
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
                    MidsContext.Character.Reset((Archetype)cbAT.SelectedItem, cbOrigin.SelectedIndex);
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
                UpdateControls(true);
                SetTitleBar(MidsContext.Character != null && MidsContext.Character.IsHero());
                UpdateColors();
                MidsContext.EnhCheckMode = false;
                UpdateEnhCheckModeToolStrip();
                _enhCheckMode.Hide();
                info_Totals();
                FileModified = false;
                myDataView?.SetData(null, null, true);
                DoRedraw();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"{e.Message}\r\n\r\n{e.StackTrace}");
            }
        }

        private void RedrawSinglePower(ref PowerEntry? powerEntry, bool singleDraw = false, bool refreshInfo = false)
        {
            drawing.DrawPowerSlot(ref powerEntry, singleDraw);
            pnlGFX.Refresh();
            if (refreshInfo)
            {
                RefreshInfo();
            }
        }

        // if we are loading a file, the file isn't modified when this method is called
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
                        ibSlotInfoEx.ToggleText.ToggledOff = @"No slots left";
                        break;
                    case < 0:
                        ibSlotInfoEx.ToggleText.ToggledOff = slotCounts[0] switch
                        {
                            < 2 => $"{Math.Abs(slotCounts[0])} slot over",
                            > 1 => $"{Math.Abs(slotCounts[0])} slots over"
                        };
                        MessageBox.Show($"This build exceeds the slot limit.\r\nPlease remove {Math.Abs(slotCounts[0])} slots from the build.", @"Invalid Slotting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    default:
                        ibSlotInfoEx.ToggleText.ToggledOff = slotCounts[0] switch
                        {
                            < 2 => $"{slotCounts[0]} slot to go",
                            > 1 => $"{slotCounts[0]} slots to go"
                        };
                        break;
                }

                ibSlotInfoEx.ToggleText.ToggledOn = slotCounts[1] switch
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
                DoRedraw();
                Application.DoEvents();
                UpdateControls();
            }

            RefreshInfo();
            UpdateModeInfo();
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

        private void PowerPicked(Enums.PowersetType setId, int nIdPower)
        {
            MainModule.MidsController.Toon.BuildPower(MidsContext.Character.Powersets[(int)setId].nID, nIdPower);
            PowerModified(true);
            MidsContext.Config.Tips.Show(Tips.TipType.FirstPower);
        }

        private void PowerPicked(int nIdPowerset, int nIdPower)
        {
            MainModule.MidsController.Toon.BuildPower(nIdPowerset, nIdPower);
            PowerModified(true);
            MidsContext.Config.Tips.Show(Tips.TipType.FirstPower);
            DoRedraw();
        }

        private void PowerPickedNoRedraw(int nIdPowerset, int nIdPower)
        {
            MainModule.MidsController.Toon.BuildPower(nIdPowerset, nIdPower, true);
            // Zed: Important: if using PowerModified() the rendering will be super slow!
            //PowerModified(markModified: true);
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

        private void PriSec_ExpandChanged(bool expanded)
        {
            if (llPrimary.IsExpanded | (llSecondary.IsExpanded & _dvAnchored.IsDocked & !HasSentForwards))
            {
                llPrimary.BringToFront();
                llSecondary.BringToFront();
                HasSentBack = false;
                HasSentForwards = true;
            }
            else
            {
                if (!(llPrimary.Bounds.IntersectsWith(_dvAnchored.Bounds) & !HasSentBack))
                {
                    return;
                }

                llPrimary.SendToBack();
                llSecondary.SendToBack();
                HasSentBack = true;
                HasSentForwards = false;
            }
        }

        private Rectangle RaGetPoolRect(int index)
        {
            Label label;
            ListLabel ll;
            switch (index)
            {
                case 0:
                    label = lblPool1;
                    ll = llPool0;
                    break;
                case 1:
                    label = lblPool2;
                    ll = llPool1;
                    break;
                case 2:
                    label = lblPool3;
                    ll = llPool2;
                    break;
                case 3:
                    label = lblPool4;
                    ll = llPool3;
                    break;
                case 4:
                    label = lblEpic;
                    ll = llAncillary;
                    break;
                default:
                    return new Rectangle(0, 0, 10, 10);
            }

            var height = ll.Top - label.Top + ll.Height;
            return new Rectangle(label.Left, label.Top, ll.Width, height);
        }

        private int RaGetTop()
        {
            return MainModule.MidsController.Toon != null
                ? 4 + llPrimary.Top + RaGreater(llPrimary.Height, llSecondary.Height)
                : llPrimary.Top + llPrimary.Height;
        }

        private int RaGreater(int iVal1, int iVal2)
        {
            return iVal1 <= iVal2 ? iVal2 : iVal1;
        }

        private void RaMovePool(int index, int x, int y)
        {
            Label label1;
            ComboBox comboBox;
            Label label2;
            ListLabel ll;
            switch (index)
            {
                case 0:
                    label1 = lblPool1;
                    comboBox = cbPool0;
                    label2 = lblLockedPool0;
                    ll = llPool0;
                    break;
                case 1:
                    label1 = lblPool2;
                    comboBox = cbPool1;
                    label2 = lblLockedPool1;
                    ll = llPool1;
                    break;
                case 2:
                    label1 = lblPool3;
                    comboBox = cbPool2;
                    label2 = lblLockedPool2;
                    ll = llPool2;
                    break;
                case 3:
                    label1 = lblPool4;
                    comboBox = cbPool3;
                    label2 = lblLockedPool3;
                    ll = llPool3;
                    break;
                case 4:
                    label1 = lblEpic;
                    comboBox = cbAncillary;
                    label2 = lblLockedAncillary;
                    ll = llAncillary;
                    break;
                default:
                    return;
            }

            label1.Location = new Point(x, y);

            var point = new Point(label1.Location.X, label1.Location.Y);
            point.Y += label1.Height;
            comboBox.Location = point;
            label2.Location = point;
            point.Y += comboBox.Height;
            ll.Location = point;
        }

        private void RaToFloat()
        {
            llPool0.Height = llPool0.DesiredHeight;
            llPool1.Height = llPool1.DesiredHeight;
            llPool2.Height = llPool2.DesiredHeight;
            llPool3.Height = llPool3.DesiredHeight;
            llAncillary.Height = llAncillary.DesiredHeight;
            var poolRect1 = RaGetPoolRect(0);
            RaMovePool(1, poolRect1.Left, poolRect1.Bottom);
            var poolRect2 = RaGetPoolRect(1);
            RaMovePool(2, poolRect2.Left, poolRect2.Bottom);
            FixPrimarySecondaryHeight();
            var num = RaGreater(RaGetTop(), lblPool3.Top);
            if (num + llAncillary.DesiredHeight > ClientSize.Height)
            {
                num = ClientSize.Height - llAncillary.DesiredHeight - cbAncillary.Height - lblEpic.Height;
                var size = llPrimary.SizeNormal;
                llPrimary.SizeNormal = new Size(size.Width, num - 4 - llPrimary.Top);

                llSecondary.SizeNormal = new Size(llSecondary.SizeNormal.Width, num - 4 - llPrimary.Top);
            }

            var poolRect3 = RaGetPoolRect(2);
            poolRect3.X = llPrimary.Left;
            poolRect3.Y = num;
            RaMovePool(4, poolRect3.Left, poolRect3.Top);
            poolRect3.X = llSecondary.Left;
            RaMovePool(3, poolRect3.Left, poolRect3.Top);
        }

        private bool RaToNormal()
        {
            llPool0.SuspendRedraw = true;
            llPool1.SuspendRedraw = true;
            llPool2.SuspendRedraw = true;
            llPool3.SuspendRedraw = true;
            llAncillary.SuspendRedraw = true;
            llPool0.Height = llPool0.DesiredHeight;
            llPool1.Height = llPool1.DesiredHeight;
            llPool2.Height = llPool2.DesiredHeight;
            llPool3.Height = llPool3.DesiredHeight;
            llAncillary.Height = llAncillary.DesiredHeight;
            FixPrimarySecondaryHeight();
            var llList = new List<ListLabel> { llAncillary, llPool3, llPool2, llPool1, llPool0 };

            foreach (var ll in llList)
            {
                //ll.Height = Math.Max(ll.DesiredHeight, 3 * llAncillary.ActualLineHeight);
                ll.Size = ll.Size with { Height = Math.Max(ll.DesiredHeight + 5, 3 * llAncillary.ActualLineHeight) };

                //Debug.WriteLine($"raToNormal(): {ll.Name}.Height = {ll.Height}/{ll.Size.Height} | {ll.Name}.DesiredHeight = {ll.DesiredHeight}, min = {3 * llAncillary.ActualLineHeight}");
            }

            var poolRect = RaGetPoolRect(0);
            RaMovePool(1, poolRect.Left, poolRect.Bottom);
            poolRect = RaGetPoolRect(1);
            RaMovePool(2, poolRect.Left, poolRect.Bottom);
            poolRect = RaGetPoolRect(2);
            RaMovePool(3, poolRect.Left, poolRect.Bottom);
            poolRect = RaGetPoolRect(3);
            RaMovePool(4, poolRect.Left, poolRect.Bottom);
            llPool0.SuspendRedraw = false;
            llPool1.SuspendRedraw = false;
            llPool2.SuspendRedraw = false;
            llPool3.SuspendRedraw = false;
            llAncillary.SuspendRedraw = false;

            return false;
        }

        private void ReArrange(bool init)
        {
            bool flag2;
            if (drawing == null)
            {
                flag2 = false;
            }
            else
            {
                var flag3 = !_dvAnchored.Visible;
                if (init)
                {
                    flag2 = RaToNormal();
                }
                else
                {
                    if (!flag3 & _dvAnchored.Bounds.IntersectsWith(_dvAnchored.SnapLocation))
                    {
                        RaToNormal();
                    }
                    else
                    {
                        RaToFloat();
                    }

                    SetAncilPoolHeight();
                    flag2 = false;
                }
            }
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

        private void RedrawUnderPopup(Rectangle rectRedraw)
        {
            var clip = rectRedraw;
            ref var local = ref clip;
            var location = pnlGFXFlow.Location;
            var x = -location.X;
            location = pnlGFXFlow.Location;
            var y = -location.Y;
            local.Offset(x, y);
            pnlGFX.Invalidate(clip);
            if (llPrimary.Bounds.IntersectsWith(rectRedraw))
            {
                llPrimary.Refresh();
            }

            if (llSecondary.Bounds.IntersectsWith(rectRedraw))
            {
                llSecondary.Refresh();
            }

            if (RaGetPoolRect(0).IntersectsWith(rectRedraw))
            {
                llPool0.Refresh();
                cbPool0.Refresh();
                lblPool1.Refresh();
                lblLockedPool0.Refresh();
            }

            if (RaGetPoolRect(1).IntersectsWith(rectRedraw))
            {
                llPool1.Refresh();
                cbPool1.Refresh();
                lblPool2.Refresh();
                lblLockedPool1.Refresh();
            }

            if (RaGetPoolRect(2).IntersectsWith(rectRedraw))
            {
                llPool2.Refresh();
                cbPool2.Refresh();
                lblPool3.Refresh();
                lblLockedPool2.Refresh();
            }

            if (RaGetPoolRect(3).IntersectsWith(rectRedraw))
            {
                llPool3.Refresh();
                cbPool3.Refresh();
                lblPool4.Refresh();
                lblLockedPool3.Refresh();
            }

            if (!RaGetPoolRect(4).IntersectsWith(rectRedraw))
            {
                return;
            }

            llAncillary.Refresh();
            cbAncillary.Refresh();
            lblEpic.Refresh();
            lblLockedAncillary.Refresh();
        }

        public void RefreshInfo()
        {
            info_Totals();
            if (dvLastPower <= -1)
            {
                return;
            }

            Info_Power(dvLastPower, dvLastEnh, dvLastNoLev, DataViewLocked);
            if (FrmEntityDetails is not { Visible: true })
            {
                return;
            }

            FrmEntityDetails.UpdateData(true);
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

        private void RemoveSlotFromTempList(PowerEntry? tp, int slotIdx)
        {
            if (tp == null)
            {
                return;
            }

            tp.Slots = tp.Slots.RemoveIndex(slotIdx);
        }

        private void SetAncilPoolHeight()
        {
            var num1 = llAncillary.ActualLineHeight * 2;
            var num2 = 1;
            do
            {
                if (llAncillary.Top + num1 + llAncillary.ActualLineHeight <= ClientRectangle.Size.Height)
                {
                    num1 += llAncillary.ActualLineHeight;
                }

                ++num2;
            } while (num2 <= 4);

            if (num1 < llAncillary.ActualLineHeight * 2)
            {
                num1 = llAncillary.ActualLineHeight * 2;
            }

            llAncillary.Height = num1;
        }

        private void SetColumns(int columns, Enums.eColumnStacking stackingMode = Enums.eColumnStacking.None)
        {
            if (columns == MidsContext.Config.Columns & stackingMode == MidsContext.Config.ColumnStackingMode)
            {
                return;
            }

            MidsContext.Config.Columns = columns;
            MidsContext.Config.ColumnStackingMode = stackingMode;

            drawing.Columns = columns;
            drawing.ColumnStackingMode = stackingMode;
            drawing.GetPowersLayout();

            // Calculate the minimum width for the entire form.
            // This includes the drawing panel, the left-side panel, and the window borders.
            int drawingMinWidth = drawing.GetMinimumRequiredWidth();
            int nonDrawingWidth = leftControlPanel.Width + (Width - ClientSize.Width);
            int formMinimumWidth = drawingMinWidth + nonDrawingWidth;

            // Set the form's minimum size.
            MinimumSize = MinimumSize with { Width = formMinimumWidth };

            // If the form is currently smaller than the new minimum, resize it.
            if (Width < formMinimumWidth)
            {
                Width = formMinimumWidth;
            }

            // 1. Recalculate the visual layout based on the panel's current width.
            drawing.UpdateLayout(pnlGFX.ClientSize.Width);

            // 2. Adjust the panel's height to fit the new number of rows.
            pnlGFX.Height = drawing.GetRequiredDrawingArea().Height;

            // 3. Force the panel to repaint itself with the new layout.
            pnlGFX.Refresh();
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

        internal void SetDataViewTab(int index)
        {
            RefreshInfo();
            if (index == 2)
            {
                if (drawing != null)
                {
                    drawing.InterfaceMode = Enums.eInterfaceMode.PowerToggle;
                }

                DoRedraw();
                //Fix so tips only show once
                MidsContext.Config.Tips.Show(Tips.TipType.TotalsTab);
            }
            else
            {
                if (drawing is { InterfaceMode: Enums.eInterfaceMode.Normal })
                {
                    return;
                }

                if (drawing != null)
                {
                    drawing.InterfaceMode = Enums.eInterfaceMode.Normal;
                }

                DoRedraw();
            }
        }

        private void SetFormHeight(bool force = false)
        {
            int iVal2;
            var num = Height - ClientSize.Height;
            if (!_dvAnchored.Visible)
            {
                iVal2 = llPool3.Top + llPool3.Height * 2 + 4 + num;
            }
            else
            {
                switch (_dvAnchored.VisibleSize)
                {
                    case Enums.eVisibleSize.Full:
                        var dvAnchoredSnapLocation = _dvAnchored.SnapLocation;
                        iVal2 = RaGreater(dvAnchoredSnapLocation.Bottom,
                            llAncillary.Top + llAncillary.ActualLineHeight * llAncillary.Items.Length) + 4 + num;
                        break;
                    case Enums.eVisibleSize.Small:
                        return;
                    case Enums.eVisibleSize.VerySmall:
                        return;
                    case Enums.eVisibleSize.Compact:
                        switch (BuildRenderer.EpicColumns)
                        {
                            case false:
                                break;
                            case true:
                                break;
                        }

                        return;
                    default:
                        return;
                }
            }

            if ((iVal2 > Height) | force | _dvAnchored.IsDocked)
            {
                if (Screen.PrimaryScreen.WorkingArea.Height > iVal2)
                {
                    Height = iVal2;
                }
                else if (Screen.PrimaryScreen.WorkingArea.Height < iVal2)
                {
                    Height = Screen.PrimaryScreen.WorkingArea.Height;
                }
            }

            NoResizeEvent = false;
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

        private void SetPopupLocation(Rectangle objectBounds, bool powerListing = false, bool picker = false)
        {
            int y;
            var top = objectBounds.Top;
            var num1 = ClientSize.Height - objectBounds.Bottom;
            var left = objectBounds.Left;
            var num2 = ClientSize.Width - objectBounds.Right;
            var rectangle = new Rectangle(0, 0, 1, 1);
            if (_dvAnchored.Visible)
            {
                rectangle.X = _dvAnchored.Left;
                rectangle.Y = _dvAnchored.Top;
                rectangle.Width = _dvAnchored.Width;
                rectangle.Height = _dvAnchored.Height;
            }

            var x = -1;
            Size clientSize;
            if (!powerListing & !picker)
            {
                if (num1 >= _popup.Height)
                {
                    y = objectBounds.Bottom;
                }
                else if (top >= _popup.Height)
                {
                    y = objectBounds.Top - _popup.Height;
                }
                else if (num2 >= _popup.Width)
                {
                    x = objectBounds.Right;
                    y = (int)Math.Round(objectBounds.Top + objectBounds.Height / 2.0 - _popup.Height / 2.0);
                }
                else if (left >= _popup.Width)
                {
                    x = objectBounds.Left - _popup.Width;
                    y = (int)Math.Round(objectBounds.Top + objectBounds.Height / 2.0 - _popup.Height / 2.0);
                }
                else
                {
                    y = objectBounds.Bottom;
                }
            }
            else if (picker)
            {
                if (num2 >= _popup.Width)
                {
                    x = objectBounds.Right;
                    y = objectBounds.Top;
                }
                else if (left >= _popup.Width)
                {
                    x = objectBounds.Left - _popup.Width;
                    y = objectBounds.Top;
                }
                else
                {
                    y = num1 < _popup.Height
                        ? top < _popup.Height ? objectBounds.Bottom : objectBounds.Top - _popup.Height
                        : objectBounds.Bottom;
                }
            }
            else
            {
                y = (int)Math.Round(objectBounds.Top + objectBounds.Height / 2.0 - _popup.Height / 2.0);
                if (y < 0)
                {
                    y = 0;
                }

                var num3 = y + _popup.Height;
                clientSize = ClientSize;
                var height = clientSize.Height;
                if (num3 > height)
                {
                    clientSize = ClientSize;
                    y = clientSize.Height - _popup.Height;
                }

                x = objectBounds.Right;
            }

            if (x < 0)
            {
                x = (int)Math.Round(objectBounds.Left + objectBounds.Width / 2.0 - _popup.Width / 2.0);
                if (left < (_popup.Width - objectBounds.Width) / 2.0)
                {
                    x = left;
                }
                else if (num2 < (_popup.Width - objectBounds.Width) / 2.0)
                {
                    clientSize = ClientSize;
                    x = clientSize.Width - _popup.Width;
                }
            }

            if (y + _popup.Height > ClientSize.Height)
            {
                y -= y + _popup.Height - ClientSize.Height;
            }

            _popup.BringToFront();
            _popup.Location = new Point(x, y);
        }

        private void SetTitleBar(bool hero = true, bool ignoreBuildSource = false)
        {
            if (MainModule.MidsController.Toon != null)
            {
                hero = MainModule.MidsController.Toon.IsHero();
            }

            var str1 = string.Empty;
            if (MainModule.MidsController.Toon != null & !ignoreBuildSource)
            {
                if (!string.IsNullOrWhiteSpace(LastFileName))
                {
                    var fileInfo = new FileInfo(LastFileName);
                    var fileName = fileInfo.Name.Length > 255 ? "Build" : fileInfo.Name;
                    str1 = $"{fileName}{(FileModified ? " [Modified]" : "")} - ";
                    tsFileSave.Text = $"&Save '{(string.IsNullOrEmpty(fileInfo.Extension) ? fileName : fileName.Replace(fileInfo.Extension, ""))}'";
                }
                else
                {
                    tsFileSave.Text = @"&Save";
                }
            }
            else
            {
                tsFileSave.Text = @"&Save";
            }

            var str2 = $"{str1}{MidsContext.Title}";
            if (!hero)
            {
                str2 = str2.Replace(nameof(hero), "Villain");
            }

            var userMode = MidsContext.Config.Mode switch
            {
                ConfigData.Modes.User => "",
                ConfigData.Modes.DbAdmin => "(DB Admin) ",
                ConfigData.Modes.AppAdmin => "(App Admin) ",
                _ => throw new ArgumentOutOfRangeException(nameof(MidsContext.Config.Mode))
            };

            Text = $@"{str2} {userMode}v{MidsContext.AssemblyVersion} {MidsContext.AppVersionStatus} ({DatabaseAPI.DatabaseName} Issue: {DatabaseAPI.Database.Issue}, {DatabaseAPI.Database.PageVolText}: {DatabaseAPI.Database.PageVol} - DBVersion: {DatabaseAPI.Database.Version})";
        }

        public void UpdateTitle()
        {
            TitleUpdated?.Invoke(this, null);
        }

        private static void ShallowCopyPowerList(PowerEntry?[] source)
        {
            for (var index = 0; index < MidsContext.Character.CurrentBuild.Powers.Count; index++)
            {
                MidsContext.Character.CurrentBuild.Powers[index] = source[index];
            }
        }

        internal void ShowAnchoredDataView()
        {
            if (FloatingDataForm != null)
            {
                _dvAnchored.VisibleSize = FloatingDataForm.dvFloat.VisibleSize;
                _dvAnchored.TabPage = FloatingDataForm.dvFloat.TabPage;
            }

            myDataView = _dvAnchored;
            myDataView.Init();
            myDataView.BackColor = BackColor;
            myDataView.DrawVillain = !MainModule.MidsController.Toon.IsHero();
            _dvAnchored.Visible = true;
            NoResizeEvent = true;
            OnResizeEnd(EventArgs.Empty);
            NoResizeEvent = false;
            RefreshInfo();
            ReArrange(false);
            FloatingDataForm = null;
        }

        private void ShowPopup(int nIdPowerset, int nIdClass, Rectangle rBounds, string extraString = "", VerticalAlignment vAlign = VerticalAlignment.Top)
        {
            if (MidsContext.Config.DisableShowPopup)
            {
                HidePopup();
            }
            else
            {
                if (vAlign == VerticalAlignment.Center)
                {
                    vAlign = VerticalAlignment.Bottom;
                }

                var bounds = _popup.Bounds;
                RedrawUnderPopup(bounds);
                if (!((nIdPowerset > -1) | (nIdClass > -1)))
                {
                    return;
                }

                if (_popup.PsIdx != (nIdPowerset <= -1 ? nIdClass : nIdPowerset))
                {
                    PopUp.PopupData iPopup;
                    if (nIdPowerset <= -1)
                    {
                        iPopup = MidsContext.Character.Archetype.PopInfo();
                    }
                    else
                    {
                        iPopup = MainModule.MidsController.Toon.PopPowersetInfo(nIdPowerset, extraString);
                    }

                    if (iPopup.Sections != null)
                    {
                        _popup.SetPopup(iPopup);
                        if (vAlign == VerticalAlignment.Bottom)
                        {
                            _popup.Location = new Point(_popup.Location.X, _popup.Location.Y - _popup.Height);
                            rBounds.Y -= _popup.Height;
                        }

                        PopUpVisible = true;
                        SetPopupLocation(rBounds, false, true);
                    }
                    else
                    {
                        HidePopup();
                    }

                    _popup.Visible = true;
                    if (ActivePopupBounds != _popup.Bounds)
                    {
                        RedrawUnderPopup(bounds);
                        ActivePopupBounds = _popup.Bounds;
                    }
                }

                _popup.HIdx = -1;
                _popup.EIdx = -1;
                _popup.PIdx = -1;
                _popup.PsIdx = nIdPowerset;
            }
        }

        private void ShowPopup(int hIdx, int pIdx, int sIdx, Point e, Rectangle rBounds, I9Slot? eSlot = null, int setIdx = -1, VerticalAlignment vAlign = VerticalAlignment.Top, I9Picker.EnhUniqueStatus? enhUniqueStatus = null)
        {
            if (MidsContext.Config.DisableShowPopup)
            {
                HidePopup();
            }
            else
            {
                var flag = false;
                var iPopup = new PopUp.PopupData();
                var picker = false;
                var powerListing = false;
                var bounds = _popup.Bounds;
                if (hIdx < 0 & pIdx > -1)
                {
                    hIdx = MidsContext.Character.CurrentBuild.FindInToonHistory(pIdx);
                }

                PowerEntry? powerEntry = null;
                if (hIdx > -1)
                {
                    powerEntry = MidsContext.Character.CurrentBuild.Powers[hIdx];
                }

                if (!(_popup.HIdx != hIdx | _popup.EIdx != sIdx | _popup.PIdx != pIdx | _popup.HIdx == -1 | _popup.EIdx == -1 | _popup.PIdx == -1))
                {
                    return;
                }

                var rectangle = new Rectangle();
                if (hIdx > -1 & sIdx < 0 & pIdx < 0 & eSlot == null & setIdx < 0)
                {
                    rectangle = drawing.PowerBoundsUnscaled(hIdx);
                    var e1 = new Point(e.X, e.Y);
                    if (drawing.WithinPowerBar(rectangle, e1))
                    {
                        if (powerEntry is { NIDPower: > -1 })
                        {
                            iPopup = MainModule.MidsController.Toon.PopPowerInfo(hIdx, powerEntry.NIDPower);
                        }

                        flag = true;
                    }
                }
                else if (sIdx > -1)
                {
                    rectangle = drawing.PowerBoundsUnscaled(hIdx);
                    if (powerEntry != null)
                    {
                        iPopup = Character.PopEnhInfo(powerEntry.Slots[sIdx].Enhancement, powerEntry.Slots[sIdx].Level, powerEntry);
                    }

                    flag = true;
                }
                else if (pIdx > -1)
                {
                    rectangle = rBounds;
                    iPopup = MainModule.MidsController.Toon.PopPowerInfo(hIdx, pIdx);
                    flag = true;
                    powerListing = true;
                }
                else if (eSlot != null & setIdx < 0)
                {
                    rectangle = rBounds;
                    iPopup = Character.PopEnhInfo(eSlot, -1, powerEntry);
                    flag = true;
                    picker = true;
                }
                else if (setIdx > -1)
                {
                    rectangle = rBounds;
                    iPopup = Character.PopSetInfo(setIdx, powerEntry);
                    flag = true;
                    picker = true;
                }

                if (flag & iPopup.Sections != null)
                {
                    if (_popup.HIdx != hIdx | _popup.EIdx != sIdx | _popup.PIdx != pIdx | _popup.HIdx == -1 | _popup.EIdx == -1 | _popup.PIdx == -1)
                    {
                        if (!picker & !powerListing)
                        {
                            rectangle = Dilate(rectangle, 2);
                            rectangle.X += pnlGFXFlow.Left - pnlGFXFlow.HorizontalScroll.Value;
                            rectangle.Y += pnlGFXFlow.Top - pnlGFXFlow.VerticalScroll.Value;
                        }

                        _popup.SetPopup(iPopup, enhUniqueStatus);
                        if (vAlign == VerticalAlignment.Bottom)
                        {
                            rectangle.Y -= rectangle.Height;
                        }
                        //else if (rectangle.Bottom > ClientSize.Height - MenuBar.Height)
                        //{
                        //    rectangle.Y -= rectangle.Bottom - (ClientSize.Height - MenuBar.Height); // _popup.Height
                        //}

                        PopUpVisible = true;
                        SetPopupLocation(rectangle, powerListing, picker);
                    }

                    _popup.Visible = true;
                    if (ActivePopupBounds != _popup.Bounds)
                    {
                        RedrawUnderPopup(bounds);
                        ActivePopupBounds = _popup.Bounds;
                    }
                }
                else
                {
                    HidePopup();
                }

                _popup.HIdx = hIdx;
                _popup.EIdx = sIdx;
                _popup.PIdx = pIdx;
                _popup.PsIdx = -1;
            }
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
            info_Totals();
            DoRedraw();
        }

        private void TsViewSelected()
        {
            switch (MidsContext.Config.ColumnStackingMode)
            {
                case Enums.eColumnStacking.Horizontal:
                    tsView3ColH.Checked = true;
                    break;

                case Enums.eColumnStacking.Vertical:
                    tsView3ColV.Checked = true;
                    break;

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
                        case 5:
                            tsView5Col.Checked = true;
                            break;
                        case 6:
                            tsView6Col.Checked = true;
                            break;
                    }

                    break;
            }
        }

        internal void UnSetMiniList()
        {
            fMini?.Dispose();
            fMini = null;
            GC.Collect();
        }

        private void UpdateColors(bool skipDraw = false)
        {
            myDataView.DrawVillain = !MidsContext.Character.IsHero();
            bool draw;
            draw = I9Picker.ForeColor.R != 96;
            BackColor = Color.FromArgb(0, 0, 0);
            lblATLocked.BackColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenHero
                : MidsContext.Config.RtFont.ColorPowerTakenVillain;
            I9Picker.ForeColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain;
            // I9Picker.Selected = MidsContext.Character.IsHero()
            //     ? MidsContext.Config.RtFont.ColorPowerHighlightHero
            //     : MidsContext.Config.RtFont.ColorPowerHighlightVillain;
            I9Picker.BackColor = BackColor;
            _popup.BackColor = Color.Black;
            _popup.ForeColor = I9Picker.ForeColor;
            myDataView.BackColor = BackColor;
            /*var style = !MidsContext.Config.RtFont.PowersSelectBold ? FontStyle.Regular : FontStyle.Bold;
            using var font = new Font(llPrimary.Font.FontFamily, MidsContext.Config.RtFont.PowersSelectBase, style, GraphicsUnit.Point);*/
            //using var font = new Font("Segoe UI", 12f, FontStyle.Bold, GraphicsUnit.Pixel);
            var toColor = new Control[]
            {
                llPrimary, llSecondary, llPool0, llPool1, llPool2, llPool3, llAncillary, lblName, lblAT, lblOrigin,
                lblCharacter, pnlGFX
            };
            foreach (var colorItem in toColor)
            {
                colorItem.BackColor = BackColor;
                if (!(colorItem is ListLabel ll))
                {
                    continue;
                }

                UpdateLLColors(ll);
                //ll.Font = font;
            }

            var toOtherColor = new Control[]
            {
                lblLockedPool0, lblLockedPool1, lblLockedPool2, lblLockedPool3, lblLockedAncillary, lblLockedSecondary, lblATLocked
            };
            foreach (var colorItem in toOtherColor) colorItem.BackColor = lblATLocked.BackColor;

            var llControls = Helpers.GetControlsOfType<ListLabel>(this);
            foreach (var llControl in llControls)
            {
                llControl.ScrollBarColor = MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenHero
                    : MidsContext.Config.RtFont.ColorPowerTakenVillain;
                llControl.ScrollButtonColor = MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                    : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain;
                llControl.UpdateTextColors(ListLabel.LlItemState.Selected,
                    MidsContext.Character.IsHero()
                        ? MidsContext.Config.RtFont.ColorPowerTakenHero
                        : MidsContext.Config.RtFont.ColorPowerTakenVillain);
                llControl.UpdateTextColors(ListLabel.LlItemState.SelectedDisabled,
                    MidsContext.Character.IsHero()
                        ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                        : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain);
                llControl.HoverColor = MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerHighlightHero
                    : MidsContext.Config.RtFont.ColorPowerHighlightVillain;
            }

            if (fRecipe is { Visible: true })
            {
                fRecipe.UpdateColorTheme();
            }

            if (fSalvageHud != null && fSalvageHud.Visible)
            {
                fSalvageHud.UpdateColorTheme();
            }

            if (!draw)
            {
                return;
            }

            if (!skipDraw)
            {
                DoRedraw();
            }

            UpdateDmBuffer();
        }

        private void UpdateControls(bool forceComplete = false, bool skipResize = false)
        {
            if (_loading)
            {
                return;
            }

            NoUpdate = true;
            ToolStripSeparator5.Visible = MidsContext.Config.MasterMode;
            AdvancedToolStripMenuItem1.Visible = MidsContext.Config.MasterMode;


            var all = Array.FindAll(DatabaseAPI.Database.Classes, GetPlayableClasses);
            var cbAT = new ComboBoxT<Archetype?>(this.cbAT);
            if (ComboCheckAT(all))
            {
                cbAT.BeginUpdate();
                cbAT.Clear();
                cbAT.AddRange(all);
                cbAT.EndUpdate();
            }

            if (cbAT.SelectedItem == null)
            {
                cbAT.SelectedItem = MidsContext.Character.Archetype;
            }
            else if (cbAT.SelectedItem.Idx != MidsContext.Character.Archetype.Idx)
            {
                cbAT.SelectedItem = MidsContext.Character.Archetype;
            }

            ibPvXEx.ToggleState = MidsContext.Config.Inc.DisablePvE switch
            {
                true => MidsVectorButton.States.ToggledOn,
                false => MidsVectorButton.States.ToggledOff
            };

            ibSlotInfoEx.ToggleState = MidsContext.Config.ShowSlotsLeft switch
            {
                true => MidsVectorButton.States.ToggledOff,
                false => MidsVectorButton.States.ToggledOn
            };

            var cbOrigin = new ComboBoxT<string>(this.cbOrigin);
            if (ComboCheckOrigin())
            {
                cbOrigin.BeginUpdate();
                cbOrigin.Clear();
                cbOrigin.AddRange(cbAT.SelectedItem.Origin);
                cbOrigin.EndUpdate();
            }

            if (cbOrigin.SelectedIndex != MidsContext.Character.Origin)
            {
                cbOrigin.SelectedIndex = MidsContext.Character.Origin < cbOrigin.Items.Count
                    ? MidsContext.Character.Origin
                    : 0;
                AssetManager.SetOrigin(cbOrigin.SelectedItem);
            }

            ComboCheckPS(CbtPrimary.Value, Enums.PowersetType.Primary, Enums.ePowerSetType.Primary);
            ComboCheckPS(CbtSecondary.Value, Enums.PowersetType.Secondary, Enums.ePowerSetType.Secondary);

            cbSecondary.Enabled = MidsContext.Character.Powersets[0].nIDLinkSecondary <= -1;

            ComboCheckPool(CbtPool0.Value, Enums.ePowerSetType.Pool);
            ComboCheckPool(CbtPool1.Value, Enums.ePowerSetType.Pool);
            ComboCheckPool(CbtPool2.Value, Enums.ePowerSetType.Pool);
            ComboCheckPool(CbtPool3.Value, Enums.ePowerSetType.Pool);
            ComboCheckPool(CbtAncillary.Value, Enums.ePowerSetType.Ancillary);

            cbPool0.SelectedIndex = MainModule.MidsController.Toon.PoolToComboID(0, MidsContext.Character.Powersets[3]?.nID ?? -1);
            cbPool1.SelectedIndex = MainModule.MidsController.Toon.PoolToComboID(1, MidsContext.Character.Powersets[4]?.nID ?? -1);
            cbPool2.SelectedIndex = MainModule.MidsController.Toon.PoolToComboID(2, MidsContext.Character.Powersets[5]?.nID ?? -1);
            cbPool3.SelectedIndex = MainModule.MidsController.Toon.PoolToComboID(3, MidsContext.Character.Powersets[6]?.nID ?? -1);

            var powersetIndexes = DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, Enums.ePowerSetType.Ancillary);
            if (MidsContext.Character.Powersets[7] != null)
            {
                cbAncillary.SelectedIndex = DatabaseAPI.ToDisplayIndex(MidsContext.Character.Powersets[7], powersetIndexes);
            }
            else
            {
                cbAncillary.SelectedIndex = 0;
            }

            cbAncillary.Enabled = MidsContext.Character.Powersets[7] != null;

            UpdatePowerLists();
            DisplayName();
            cbAT.Enabled = !MainModule.MidsController.Toon.Locked;
            cbPool0.Enabled = !MainModule.MidsController.Toon.PoolLocked[0];
            cbPool1.Enabled = !MainModule.MidsController.Toon.PoolLocked[1];
            cbPool2.Enabled = !MainModule.MidsController.Toon.PoolLocked[2];
            cbPool3.Enabled = !MainModule.MidsController.Toon.PoolLocked[3];
            cbAncillary.Enabled = !MainModule.MidsController.Toon.PoolLocked[4];
            lblATLocked.Text = cbAT.SelectedItem.DisplayName;
            lblATLocked.Visible = MainModule.MidsController.Toon.Locked;
            lblLockedPool0.Location = cbPool0.Location;
            lblLockedPool0.Size = cbPool0.Size;
            lblLockedPool0.Text = cbPool0.Text;
            lblLockedPool0.Visible = MainModule.MidsController.Toon.PoolLocked[0];
            lblLockedPool1.Location = cbPool1.Location;
            lblLockedPool1.Size = cbPool1.Size;
            lblLockedPool1.Text = cbPool1.Text;
            lblLockedPool1.Visible = MainModule.MidsController.Toon.PoolLocked[1];
            lblLockedPool2.Location = cbPool2.Location;
            lblLockedPool2.Size = cbPool2.Size;
            lblLockedPool2.Text = cbPool2.Text;
            lblLockedPool2.Visible = MainModule.MidsController.Toon.PoolLocked[2];
            lblLockedPool3.Location = cbPool3.Location;
            lblLockedPool3.Size = cbPool3.Size;
            lblLockedPool3.Text = cbPool3.Text;
            lblLockedPool3.Visible = MainModule.MidsController.Toon.PoolLocked[3];
            lblLockedAncillary.Location = cbAncillary.Location;
            lblLockedAncillary.Size = cbAncillary.Size;
            lblLockedAncillary.Text = cbAncillary.Text;
            lblLockedAncillary.Visible = !cbAncillary.Enabled;
            lblLockedSecondary.Location = cbSecondary.Location;
            lblLockedSecondary.Size = cbSecondary.Size;
            lblLockedSecondary.Text = cbSecondary.Text;
            lblLockedSecondary.Visible = !cbSecondary.Enabled;
            llPrimary.SuspendRedraw = true;
            llSecondary.SuspendRedraw = true;
            llPrimary.PaddingY = 2;
            llSecondary.PaddingY = 2;
            FixPrimarySecondaryHeight();

            /*foreach (var llControl in Helpers.GetControlsOfType<ListLabel>(this))
            {
                var loc = llControl.Location;
                var style = !MidsContext.Config.RtFont.PowersSelectBold ? FontStyle.Regular : FontStyle.Bold;
                llControl.Font = new Font(llControl.Font.FontFamily, MidsContext.Config.RtFont.PowersSelectBase, style, GraphicsUnit.Point);
                foreach (var e in llControl.Items)
                {
                    e.Bold = MidsContext.Config.RtFont.PowersSelectBold;
                }

                // For some reason llControl will be moved when changing font style/size if not using default font size.
                llControl.Location = new Point(loc.X, loc.Y);

                switch (llControl.Name)
                {
                    // Readjust primary/secondary lists so they don't overlap
                    case "llPrimary":
                        llControl.Width = cbPrimary.Width;
                        break;

                    case "llSecondary":
                        llControl.Width = cbSecondary.Width;
                        break;

                    // Prevent horizontal scrollbar in pools panel
                    case "llPool0":
                    case "llPool1":
                    case "llPool2":
                    case "llPool3":
                    case "llAncillary":
                        llControl.Width = cbPool0.Width;
                        break;
                }
            }*/

            ibAlignmentEx.ToggleState = MidsContext.Character.IsHero() switch
            {
                true => MidsVectorButton.States.ToggledOff,
                false => MidsVectorButton.States.ToggledOn
            };

            //_dvAnchored.SetLocation(new Point(llPrimary.Left, llPrimary.Top + RaGreater(llPrimary.SizeNormal.Height, llSecondary.SizeNormal.Height) + 5), forceComplete);
            llPrimary.SuspendRedraw = false;
            llSecondary.SuspendRedraw = false;
            if (myDataView != null && (drawing.InterfaceMode == Enums.eInterfaceMode.Normal) & (myDataView.TabPageIndex == 2))
            {
                dvAnchored_TabChanged(myDataView.TabPageIndex);
            }

            if (MidsContext.Config.BuildMode == Enums.dmModes.LevelUp)
            {
                UpdateDmBuffer();
            }

            if (!skipResize)
            {
                DoResize();
            }

            NoUpdate = false;
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

            if (ibDynMode.Lock && MidsContext.Config.BuildMode == Enums.dmModes.LevelUp)
            {
                ibDynMode.Lock = false;
            }
            else if (MidsContext.Config.BuildMode != Enums.dmModes.LevelUp)
            {
                ibDynMode.Lock = false;
            }

            switch (powerState)
            {
                case Enums.ePowerState.Used:
                    ibDynMode.ToggleText.ToggledOff = text;
                    ibDynMode.ToggleState = MidsVectorButton.States.ToggledOff;
                    break;
                case Enums.ePowerState.Open:
                    ibDynMode.ToggleText.ToggledOn = text;
                    ibDynMode.ToggleState = MidsVectorButton.States.ToggledOn;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!ibDynMode.Lock && MidsContext.Config.BuildMode == Enums.dmModes.LevelUp)
            {
                ibDynMode.Lock = true;
            }
        }

        private void UpdateLLColors(ListLabel iList)
        {
            iList.UpdateTextColors(ListLabel.LlItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            iList.UpdateTextColors(ListLabel.LlItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            iList.UpdateTextColors(ListLabel.LlItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
            iList.ScrollBarColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenHero
                : MidsContext.Config.RtFont.ColorPowerTakenVillain;
            iList.ScrollButtonColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain;
            iList.UpdateTextColors(ListLabel.LlItemState.Selected,
                MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenHero
                    : MidsContext.Config.RtFont.ColorPowerTakenVillain);
            iList.UpdateTextColors(ListLabel.LlItemState.SelectedDisabled,
                MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                    : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain);
            iList.HoverColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerHighlightHero
                : MidsContext.Config.RtFont.ColorPowerHighlightVillain;
        }

        private void UpdateOtherFormsFonts()
        {
            if (fIncarnate is { Visible: true })
            {
                foreach (var llControl in fIncarnate.Controls.OfType<ListLabel>())
                {
                    llControl.SuspendRedraw = true;
                    llControl.Font = llPrimary.Font;
                    llControl.UpdateTextColors(ListLabel.LlItemState.Enabled,
                        MidsContext.Config.RtFont.ColorPowerAvailable);
                    llControl.UpdateTextColors(ListLabel.LlItemState.Disabled,
                        MidsContext.Config.RtFont.ColorPowerDisabled);
                    llControl.UpdateTextColors(ListLabel.LlItemState.Invalid,
                        Color.FromArgb(byte.MaxValue, 0, 0));
                    llControl.ScrollBarColor = MidsContext.Character.IsHero()
                        ? MidsContext.Config.RtFont.ColorPowerTakenHero
                        : MidsContext.Config.RtFont.ColorPowerTakenVillain;
                    llControl.ScrollButtonColor = MidsContext.Character.IsHero()
                        ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                        : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain;
                    llControl.UpdateTextColors(ListLabel.LlItemState.Selected,
                        MidsContext.Character.IsHero()
                            ? MidsContext.Config.RtFont.ColorPowerTakenHero
                            : MidsContext.Config.RtFont.ColorPowerTakenVillain);
                    llControl.UpdateTextColors(ListLabel.LlItemState.SelectedDisabled,
                        MidsContext.Character.IsHero()
                            ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                            : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain);
                    llControl.HoverColor = MidsContext.Character.IsHero()
                        ? MidsContext.Config.RtFont.ColorPowerHighlightHero
                        : MidsContext.Config.RtFont.ColorPowerHighlightVillain;
                }

                foreach (var t in fIncarnate.LlLeft.Items)
                {
                    t.Bold = MidsContext.Config.RtFont.PairedBold;
                }

                foreach (var t in fIncarnate.LlRight.Items)
                {
                    t.Bold = MidsContext.Config.RtFont.PairedBold;
                }

                fIncarnate.LlLeft.SuspendRedraw = false;
                fIncarnate.LlRight.SuspendRedraw = false;
                fIncarnate.LlLeft.Refresh();
                fIncarnate.LlRight.Refresh();
            }

            if (fTemp is { Visible: true })
            {
                fTemp.UpdateFonts(llPrimary.Font);
            }

            if (fAccolade is { Visible: true })
            {
                fAccolade.UpdateFonts(llPrimary.Font);
            }

            if (fPrestige is { Visible: true })
            {
                fPrestige.UpdateFonts(llPrimary.Font);
            }
        }

        private void UpdatePowerList(ListLabel llPower)
        {
            llPower.SuspendRedraw = true;
            if (llPower.Items.Length == 0)
            {
                llPower.AddItem(new ListLabel.ListLabelItem("Nothing", ListLabel.LlItemState.Disabled));
            }

            foreach (var listLabelItemV3 in llPower.Items)
            {
                if (listLabelItemV3.NIdSet <= -1 | listLabelItemV3.IdxPower <= -1)
                {
                    continue;
                }

                var message = "";
                // listLabelItemV3.ItemState = MainModule.MidsController.Toon.PowerState(listLabelItemV3.NIdPower, ref message);
                // listLabelItemV3.Italic = listLabelItemV3.ItemState == ListLabel.LlItemState.Invalid;
                // listLabelItemV3.Bold = MidsContext.Config.RtFont.PairedBold;
            }

            llPower.SuspendRedraw = false;
        }

        private void UpdatePowerLists()
        {
            var noPrimary = false;
            if (llPrimary.Items.Length == 0)
            {
                noPrimary = true;
            }
            else if (llPrimary.Items[^1].NIdSet != (MidsContext.Character.Powersets[0] == null ? -1 : MidsContext.Character.Powersets[0].nID))
            {
                noPrimary = true;
            }

            if (llSecondary.Items.Length == 0)
            {
                noPrimary = true;
            }
            else if (llSecondary.Items[^1].NIdSet != (MidsContext.Character.Powersets[1] == null ? -1 : MidsContext.Character.Powersets[1].nID))
            {
                noPrimary = true;
            }

            var noAncillary = false;
            if (llAncillary.Items.Length == 0 || MidsContext.Character.Powersets[7] == null)
            {
                noAncillary = true;
            }
            else if (llAncillary.Items[^1].NIdSet != MidsContext.Character.Powersets[7].nID)
            {
                noAncillary = true;
            }

            if (noPrimary)
            {
                var llPrimary = this.llPrimary;
                AssemblePowerList(llPrimary, MidsContext.Character.Powersets[0]);
                this.llPrimary = llPrimary;
                AssemblePowerList(llSecondary, MidsContext.Character.Powersets[1]);
            }
            else
            {
                UpdatePowerList(llPrimary);
                UpdatePowerList(llSecondary);
            }

            if (noAncillary | noPrimary)
            {
                AssemblePowerList(llAncillary, MidsContext.Character.Powersets[7]);
                UpdatePowerList(llAncillary);
            }
            else
            {
                UpdatePowerList(llAncillary);
            }

            AssemblePowerList(llPool0, MidsContext.Character.Powersets[3]);


            AssemblePowerList(llPool1, MidsContext.Character.Powersets[4]);
            AssemblePowerList(llPool2, MidsContext.Character.Powersets[5]);
            AssemblePowerList(llPool3, MidsContext.Character.Powersets[6]);
            UpdatePowerList(llPool0);
            UpdatePowerList(llPool1);
            UpdatePowerList(llPool2);
            UpdatePowerList(llPool3);
        }

        public string GetSelectedArchetype()
        {
            return cbAT.SelectedIndex < 0 ? "" : cbAT.Items[cbAT.SelectedIndex].ToString();
        }

        public string GetSelectedPrimaryPowerset()
        {
            return cbPrimary.SelectedIndex < 0 ? "" : cbPrimary.Items[cbPrimary.SelectedIndex].ToString();
        }

        public string GetSelectedSecondaryPowerset()
        {
            return cbSecondary.SelectedIndex < 0 ? "" : cbSecondary.Items[cbSecondary.SelectedIndex].ToString();
        }

        public void UpdateEnhCheckModeToolStrip()
        {
            ToggleCheckModeToolStripMenuItem.Checked = MidsContext.EnhCheckMode;
        }

        public bool IsSalvageHudVisible()
        {
            return fSalvageHud is { Visible: true };
        }

        public void SetSalvageHudOnCloseExecution(bool s)
        {
            if (IsSalvageHudVisible())
            {
                fSalvageHud?.SetOnCloseUpdatesExecution(s);
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

        private void BuildRecover(string? buildString)
        {
            try
            {
                var importHandle = new PlainTextParser(buildString);
                var listPowers = importHandle.Parse();

                if (listPowers == null)
                {
                    return;
                }

                InjectBuild(buildString, listPowers, importHandle.GetPowersets(), importHandle.GetCharacterInfo());
                LastFileName = "";
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\r\n\r\n{e.StackTrace}");
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

            SetEnhCheckModePosition();
            SetTitleBar();

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

            // Update slots counter... maybe.
            // Turns out all this block is not needed. (I think)
            /*
            int index = -1;
            MainModule.MidsController.Toon.Complete = !sl.IsValidNext();
            fixStatIncludes();
            FileModified = false;
            if (MidsContext.Config.BuildMode == Enums.dmModes.Normal)
            {
                index = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex(MainModule.MidsController.Toon.RequestedLevel);
                if (index < 0)
                {
                    index = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex();
                }
            }
            else if (DatabaseAPI.Database.Levels[MidsContext.Character.Level].LevelType() == Enums.dmItem.Power)
            {
                index = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex();
                drawing.HighlightSlot(-1);
            }

            if (MainModule.MidsController.Toon.Complete)
            {
                drawing.HighlightSlot(-1);
            }

            int[] slotCounts = MainModule.MidsController.Toon.GetSlotCounts(characterInfo.Level - 1);
            ibAccolade.TextOff = slotCounts[0] <= 0 ? "No slot left" : slotCounts[0] + " slot" + (slotCounts[0] == 1 ? String.Empty : "s") + " to go";
            ibAccolade.TextOn = slotCounts[1] <= 0 ? "No slot placed" : slotCounts[1] + " slot" + (slotCounts[1] == 1 ? String.Empty : "s") + " placed";
            if (index > -1 & index <= MidsContext.Character.CurrentBuild.Powers.Count)
            {
                MidsContext.Character.RequestedLevel = MidsContext.Character.CurrentBuild.Powers[index].Level;
                MidsContext.Character.SetLevelTo(MidsContext.Character.CurrentBuild.Powers[index].Level);
            }

            MidsContext.Character.Validate();
            MidsContext.Character.Lock();
            MidsContext.Character.PoolShuffle();
            AssetManager.OriginIndex = MidsContext.Character.Origin;

            MidsContext.Character.Validate();
            MidsContext.Config.LastFileName = buildFile;
            */
        }

        #region "fields"

        private Rectangle ActivePopupBounds;

        private bool DataViewLocked;
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
        private BuildRenderer? drawing;
        private int dvLastEnh;
        private bool dvLastNoLev;
        private int dvLastPower;
        private int EnhancingPower;
        private int EnhancingSlot;
        private readonly bool EnhPickerActive;
        private frmAccolade? fAccolade;
        private frmData? fData;
        private frmCompare? fGraphCompare;
        private frmStats? fGraphStats;
        private bool FileModified { get; set; }
        private FrmIncarnate? fIncarnate;
        private frmPrestige? fPrestige;
        private bool FlipActive;
        private PowerEntry? FlipGP;
        private readonly int FlipInterval;
        private int FlipPowerID;
        private int[]? FlipSlotState;
        private readonly int FlipStepDelay;
        private readonly int FlipSteps;
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
        private bool HasSentBack;
        private bool HasSentForwards;
        private bool LastClickPlacedSlot;
        private int LastEnhIndex;
        private I9Slot? LastEnhPlaced;
        private string? LastFileName;
        private int LastIndex;
        private FormWindowState LastState;
        private DataView? myDataView;
        private bool NoResizeEvent;
        private bool NoUpdate;
        private Rectangle oldDragRect;
        private int PickerHID;
        private bool PopUpVisible;
        private bool top_fData;
        private bool top_fGraphCompare;
        private bool top_fGraphStats;
        private bool top_fRecipe;
        private bool top_fSetFinder;
        private bool top_fSets;
        private bool top_fTotals;
        private int xCursorOffset;
        private int yCursorOffset;


        //RawEnhData RawEnhData = new RawEnhData();
        //RawPowerData RawPowerData = new RawPowerData();

        #endregion
    }
}
