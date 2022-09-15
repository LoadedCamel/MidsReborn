using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;
using Mids_Reborn.Forms.DiscordSharing;
using Mids_Reborn.Forms.ImportExportItems;
using Mids_Reborn.Forms.OptionsMenuItems;
using Mids_Reborn.Forms.OptionsMenuItems.DbEditor;
using Mids_Reborn.Forms.UpdateSystem;
using Mids_Reborn.Forms.WindowMenuItems;
using MRBResourceLib;
using Cursor = System.Windows.Forms.Cursor;
using Cursors = System.Windows.Forms.Cursors;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;
using Timer = System.Windows.Forms.Timer;

namespace Mids_Reborn.Forms
{
    public sealed partial class frmMain : Form
    {
        private const string UriScheme = "mrb";

        private frmInitializing _frmInitializing;

        private frmBusy _frmBusy;

        internal OpenFileDialog DlgOpen;

        internal SaveFileDialog DlgSave;

        private bool exportDiscordInProgress;

        private bool loading;

        private bool gfxDrawing;

        private long popupLastOpenTime;

        public bool DbChangeRequested { get; set; }

        public event EventHandler TitleUpdated;

        public static frmMain MainInstance;

        private string[] CommandArgs { get; }
        private string ProcessedCommand { get; set; }
        private bool ProcessedFromCommand { get; set; }

        public frmMain(string[] args)
        {
            CommandArgs = args;
            if (!Debugger.IsAttached || !this.IsInDesignMode() || !Process.GetCurrentProcess().ProcessName.ToLowerInvariant().Contains("devenv"))
            {
                if (File.Exists(Files.GetConfigFilename()))
                {
                    var fInfo = new FileInfo(Files.GetConfigFilename());
                    if (fInfo.Length > 0)
                    {
                        ConfigData.Initialize(Serializer.GetSerializer());
                    }
                    else
                    {
                        var tempConfig = new ConfigData();
                        tempConfig.Save(Serializer.GetSerializer(), Files.GetConfigFilename());
                        tempConfig.SaveConfig(Serializer.GetSerializer());
                        ConfigData.Initialize(Serializer.GetSerializer());
                    }
                }
                if (MidsContext.Config is { ApplicationRegistered: false })
                {
                    RegisterUriScheme();
                }
                
                Load += frmMain_Load;
                Closed += frmMain_Closed;
                FormClosing += frmMain_Closing;
                ResizeEnd += frmMain_Resize;
                KeyDown += frmMain_KeyDown;
                MouseWheel += frmMain_MouseWheel;
                TitleUpdated += OnTitleUpdate;
                Move += frmMain_Move;
                Shown += OnShown;
                NoUpdate = false;
                EnhancingSlot = -1;
                EnhancingPower = -1;
                EnhPickerActive = false;
                PickerHID = -1;
                LastFileName = string.Empty;
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
                DisplayApi.FrmMain = this;
            }
            InitializeComponent();
            MainInstance = this;
            if (MidsContext.Config is { CheckForUpdates: true }) clsXMLUpdate.CheckUpdate(this);
            //disable menus that are no longer hooked up, but probably should be hooked back up
            tsHelp.Visible = false;
            tsHelp.Enabled = false;
            tmrGfx.Tick += tmrGfx_Tick;
            //adding events
            if (Debugger.IsAttached && this.IsInDesignMode() && Process.GetCurrentProcess().ProcessName.ToLowerInvariant().Contains("devenv"))
                return;
            dvAnchored = new DataView();
            Controls.Add(dvAnchored);
            dvAnchored.BackColor = Color.Black;
            dvAnchored.DrawVillain = false;
            dvAnchored.Floating = false;
            dvAnchored.Font = new Font("Arial", 10.25f, FontStyle.Regular, GraphicsUnit.Pixel, 0);

            dvAnchored.Location = new Point(16, 425);
            dvAnchored.Name = "dvAnchored";

            dvAnchored.Size = new Size(300, 400);
            dvAnchored.TabIndex = 69;
            dvAnchored.VisibleSize = Enums.eVisibleSize.Full;
            dvAnchored.MouseWheel += frmMain_MouseWheel;
            dvAnchored.SizeChange += dvAnchored_SizeChange;
            dvAnchored.FloatChange += dvAnchored_Float;
            dvAnchored.Unlock_Click += dvAnchored_Unlock;
            dvAnchored.SlotUpdate += DataView_SlotUpdate;
            dvAnchored.SlotFlip += DataView_SlotFlip;
            dvAnchored.Moved += dvAnchored_Move;
            dvAnchored.TabChanged += dvAnchored_TabChanged;

            Icon = Resources.MRB_Icon_Concept;
        }

        private void OnShown(object? sender, EventArgs e)
        {
            var comLoad = false;
            if (MidsContext.Config != null)
            {
                var prevLastFileNameCfg = MidsContext.Config.LastFileName;
                var prevLoadLastCfg = MidsContext.Config.DisableLoadLastFileOnStart;
                var toonLoaded = false;
                if (CommandArgs.Length > 0)
                {
                    switch (CommandArgs[0])
                    {
                        case "-master":
                            MidsContext.Config.MasterMode = true;
                            ToolStripSeparator5.Visible = MidsContext.Config.MasterMode;
                            AdvancedToolStripMenuItem1.Visible = MidsContext.Config.MasterMode;
                            SetTitleBar();
                            ProcessedCommand = CommandArgs[0];
                            ProcessedFromCommand = true;

                            break;
                        case "-load":
                            var nArgs = CommandArgs.Skip(1);
                            var file = string.Join(" ", nArgs);
                            MidsContext.Config.DisableLoadLastFileOnStart = false;
                            DoOpen(file);
                            ProcessedFromCommand = true;

                            break;
                        case var fileLoad when CommandArgs[0].Contains(".mxd") && !CommandArgs[0].Contains("mrb://"):
                            ProcessedFromCommand = false;
                            MidsContext.Config.LastFileName = fileLoad;
                            break;
                        default:
                            if (Uri.TryCreate(CommandArgs[0], UriKind.Absolute, out var uri) && string.Equals(uri.Scheme, UriScheme, StringComparison.OrdinalIgnoreCase))
                            {
                                MidsContext.Config.DisableLoadLastFileOnStart = false;
                                Task.Run(() => ProcessUriCommands(uri));
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
                    toonLoaded = DoOpen(MidsContext.Config.LastFileName);
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
                        MidsContext.Config.DisableLoadLastFileOnStart = prevLoadLastCfg;
                        break;
                }
            }

            if (comLoad)
            {
                command_Load(ProcessedCommand);
            }
        }

        public bool petWindowFlag { get; set; }

        private List<string> MMPets { get; set; } = new();

        // store the instance for reuse, as these things are called per draw/redraw
        private Lazy<ComboBoxT<Archetype>> CbtAT => new(() => new ComboBoxT<Archetype>(cbAT));

        private Lazy<ComboBoxT<string>> CbtPrimary => new(() => new ComboBoxT<string>(cbPrimary));

        private Lazy<ComboBoxT<string>> CbtSecondary => new(() => new ComboBoxT<string>(cbSecondary));

        private Lazy<ComboBoxT<string>> CbtAncillary => new(() => new ComboBoxT<string>(cbAncillary));

        private Lazy<ComboBoxT<string>> CbtPool0 => new(() => new ComboBoxT<string>(cbPool0));
        private Lazy<ComboBoxT<string>> CbtPool1 => new(() => new ComboBoxT<string>(cbPool1));
        private Lazy<ComboBoxT<string>> CbtPool2 => new(() => new ComboBoxT<string>(cbPool2));
        private Lazy<ComboBoxT<string>> CbtPool3 => new(() => new ComboBoxT<string>(cbPool3));

        internal clsDrawX Drawing => drawing;

        private void InitializeDv()
        {
            Info_Power(llPrimary.Items[0].nIDPower);
        }

        private I9Picker I9Picker
        {
            get
            {
                if (i9Picker.Height <= 235)
                {
                    i9Picker.Height = 315;
                }

                return i9Picker;
            }
            set => i9Picker = value;
        }

        public int GetPrimaryBottom()
        {
            return cbPrimary.Top + cbPrimary.Height;
        }

        public string GetBuildFile(bool stripExt = false)
        {
            if (MainModule.MidsController.Toon == null) return "";
            if (!stripExt) return LastFileName;

            var r = new Regex(@"\.(([tT][xX][tT])|([mM][hHxX][dD]))$");
            return r.Replace(LastFileName, "");
        }

        private ComboBoxT<string> GetCbOrigin()
        {
            return new ComboBoxT<string>(cbOrigin);
        }

        private static void RegisterUriScheme()
        {
            if (MidsContext.Config == null) return;
            // Modify to only perform on 1st run - need to account for removal as well. Perhaps move to the installer?
            const string friendlyName = "Mids Reborn Protocol";
            var appLoc = Application.ExecutablePath;
            using var key = Registry.CurrentUser.CreateSubKey($@"SOFTWARE\Classes\{UriScheme}");
            key?.SetValue("", $"URL:{friendlyName}");
            key?.SetValue("URL Protocol", "");
            using var defaultIcon = key?.CreateSubKey("DefaultIcon");
            defaultIcon?.SetValue("", appLoc + ",1");
            using var commandKey = key?.CreateSubKey(@"shell\open\command");
            commandKey?.SetValue("", $"\"{appLoc}\" \"%1\"");
            MidsContext.Config.ApplicationRegistered = true;
        }

        private void frmMain_Load(object? sender, EventArgs e)
        {
            if (MidsContext.Config == null) return;
            loading = true;
            try
            {
                if (MidsContext.Config.I9.DefaultIOLevel == 27)
                {
                    MidsContext.Config.I9.DefaultIOLevel = 49;
                }

                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
                using var iFrm = new frmInitializing();
                _frmInitializing = iFrm;
                _frmInitializing.Show();
                myDataView = dvAnchored;
                pnlGFX.BackColor = BackColor;
                NoUpdate = true;

                if (!this.IsInDesignMode() && !MidsContext.Config.IsInitialized)
                {
                    MidsContext.Config.CheckForUpdates = false;
                    //MessageBox.Show(("Welcome to Mids' Reborn : Hero Designer "
                    //+ MidsContext.AppVersion 
                    //+ "! Please check the Readme/Help for quick instructions.\r\n\r\nMids' Hero Designer is able to check for and download updates automatically when it starts.\r\nIt's recommended that you turn on automatic updating. Do you want to?\r\n\r\n(If you don't, you can manually check from the 'Updates' tab in the options.)"), MessageBoxButtons.YesNo | MessageBoxIcon.Question, "Welcome!") == DialogResult.Yes;
                    MidsContext.Config.IsInitialized = true;
                    MidsContext.Config.FirstRun = true;
                }

                if (MidsContext.Config.FirstRun)
                {
                    MainModule.MidsController.SelectDatabase(_frmInitializing);
                }
                else
                {
                    MainModule.MidsController.LoadData(ref _frmInitializing, MidsContext.Config.DataPath);
                }

                _frmInitializing?.SetMessage("Setting up UI...");
                dvAnchored.VisibleSize = MidsContext.Config.DvState;
                SetTitleBar();
                NewToon();
                PowerModified(true);

                var comboData = MidsContext.Config.RelativeScales;
                if (EnemyRelativeToolStripComboBox.ComboBox != null)
                {
                    EnemyRelativeToolStripComboBox.ComboBox.DataSource = null;
                    EnemyRelativeToolStripComboBox.ComboBox.DisplayMember = "Key";
                    EnemyRelativeToolStripComboBox.ComboBox.ValueMember = "Value";
                    EnemyRelativeToolStripComboBox.ComboBox.DataSource = comboData;

                    var scalingToHitItem = comboData.FirstOrDefault(x => x.Value == MidsContext.Config.ScalingToHit);
                    var selectedIndex = comboData.IndexOf(scalingToHitItem);
                    EnemyRelativeToolStripComboBox.SelectedIndex = selectedIndex;
                }


                dvAnchored.Init();
                cbAT.SelectedItem = MidsContext.Character.Archetype;
                lblATLocked.Location = new Point(cbAT.Location.X, cbAT.Location.Y);
                lblATLocked.Size = new Size(cbAT.Width, cbAT.Height);
                lblATLocked.BorderStyle = BorderStyle.None;
                lblATLocked.FlatStyle = FlatStyle.Flat;
                lblATLocked.Visible = false;
                lblLocked0.Location = cbPool0.Location;
                lblLocked0.Size = cbPool0.Size;
                lblLocked0.Visible = false;
                lblLocked1.Location = cbPool1.Location;
                lblLocked1.Size = cbPool1.Size;
                lblLocked1.Visible = false;
                lblLocked2.Location = cbPool2.Location;
                lblLocked2.Size = cbPool2.Size;
                lblLocked2.Visible = false;
                lblLocked3.Location = cbPool3.Location;
                lblLocked3.Size = cbPool3.Size;
                lblLocked3.Visible = false;
                lblLockedAncillary.Location = cbAncillary.Location;
                lblLockedAncillary.Size = cbAncillary.Size;
                lblLockedAncillary.Visible = false;

                if (MidsContext.Config.Bounds.Location.IsEmpty)
                {
                    Location = new Point((Screen.PrimaryScreen.Bounds.Width - Width) / 2, (Screen.PrimaryScreen.Bounds.Height - Height) / 2);
                    Size = new Size(1342, 1001);
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
                            DesktopBounds = MidsContext.Config.Bounds;
                            break;
                        case "Minimized":
                            WindowState = FormWindowState.Normal;
                            Location = new Point((Screen.PrimaryScreen.Bounds.Width - Width) / 2, (Screen.PrimaryScreen.Bounds.Height - Height) / 2);
                            Size = new Size(1342, 1001);
                            break;
                    }
                }

                tsViewIOLevels.Checked = !MidsContext.Config.I9.HideIOLevels;
                tsViewRelative.Checked = MidsContext.Config.ShowEnhRel;
                tsViewSOLevels.Checked = MidsContext.Config.ShowSoLevels;
                tsViewSlotLevels.Checked = MidsContext.Config.ShowSlotLevels;
                tsViewRelativeAsSigns.Checked = MidsContext.Config.ShowRelSymbols;
                tsViewSelected();
                tsIODefault.Text = $"Default ({MidsContext.Config.I9.DefaultIOLevel + 1})";
                SetDamageMenuCheckMarks();
                GetBestDamageValues();
                dvAnchored.SetFontData();
                DlgSave.InitialDirectory = MidsContext.Config.BuildsPath;
                DlgOpen.InitialDirectory = MidsContext.Config.BuildsPath;
                NoUpdate = false;
                tsViewSlotLevels.Checked = MidsContext.Config.ShowSlotLevels;
                ibSlotLevelsEx.ToggleState = MidsContext.Config.ShowSlotLevels switch
                {
                    true => ImageButtonEx.States.ToggledOn,
                    false => ImageButtonEx.States.ToggledOff
                };
                
                UpdateModeInfo();
                tsViewRelative.Checked = MidsContext.Config.ShowEnhRel;
                ibPopupEx.ToggleState = MidsContext.Config.DisableShowPopup switch
                {
                    true => ImageButtonEx.States.ToggledOff,
                    false => ImageButtonEx.States.ToggledOn
                };

                ibRecipeEx.ToggleState = MidsContext.Config.PopupRecipes switch
                {
                    true => ImageButtonEx.States.ToggledOn,
                    false => ImageButtonEx.States.ToggledOff
                };

                ibPvXEx.ToggleState = MidsContext.Config.Inc.DisablePvE switch
                {
                    true => ImageButtonEx.States.ToggledOn,
                    false => ImageButtonEx.States.ToggledOff
                };

                Show();
                _frmInitializing?.Hide();
                _frmInitializing?.Close();
                Refresh();
                dvAnchored.SetScreenBounds(ClientRectangle);
                var iLocation = new Point();
                ref var local = ref iLocation;
                var left = llPrimary.Left;
                var top = llPrimary.Top;
                var size1 = llPrimary.SizeNormal;
                var height5 = size1.Height;
                var y = top + height5 + 5;
                local = new Point(left, y);
                dvAnchored.SetLocation(iLocation, true);
                PriSec_ExpandChanged(true);
                loading = false;
                UpdateControls(true);
                setColumns(MidsContext.Config.Columns < 1 ? 3 : MidsContext.Config.Columns);
                UpdatePoolsPanelSize();
                InitializeDv();
                MidsContext.Character.AlignmentChanged += CharacterOnAlignmentChanged;
                if (this.IsInDesignMode())
                    return;
                /*if (MidsContext.Config.CheckForUpdates)
                    clsXMLUpdate.CheckUpdate();*/
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error has occurred when loading the main form. Error: " + ex.Message + "\r\n" + ex.StackTrace,
                    "OMIGODHAX");
                throw;
            }

            loading = false;
            MidsContext.Config.FirstRun = false;
            MidsContext.Config.SaveConfig(Serializer.GetSerializer());
        }

        private async Task ProcessUriCommands(Uri uri)
        {
            using var nStream = new MemoryStream();
            using var httpClient = new HttpClient();
            var newUri = uri.ToString().Replace("mrb://", "https://");
            var buildTask = await httpClient.GetStreamAsync(newUri);
            await buildTask.CopyToAsync(nStream);
            DoLoad(nStream);
        }

        internal void ChildRequestedRedraw()
        {
            DoRedraw();
        }

        //void accoladeButton_ButtonClicked() => PowerModified(markModified: false);

        private void CharacterOnAlignmentChanged(object? sender, Enums.Alignment e)
        {
            var imageButtonExControls = Helpers.GetControlOfType<ImageButtonEx>(Controls);
            foreach (var ibEx in imageButtonExControls)
            {
                ibEx.UseAlt = e switch
                {
                    Enums.Alignment.Hero => false,
                    Enums.Alignment.Villain => true,
                    _ => ibEx.UseAlt
                };
            }
        }

        private void ibModeEx_OnClick(object sender, EventArgs eventArgs)
        {
            if (MainModule.MidsController.Toon == null)
                return;

            if (MidsContext.Config == null) return;
            switch (ibModeEx.ToggleState)
            {
                case ImageButtonEx.States.ToggledOff:
                    MidsContext.Config.BuildMode = Enums.dmModes.LevelUp;
                    if (DatabaseAPI.ServerData.EnableInherentSlotting)
                    {
                        MainModule.MidsController.Toon.ClearInvalidInherentSlots();
                    }

                    break;
                case ImageButtonEx.States.ToggledOn:
                    MidsContext.Config.BuildMode = Enums.dmModes.Normal;
                    break;
                case ImageButtonEx.States.Indeterminate:
                    MidsContext.Config.BuildMode = Enums.dmModes.Respec;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!DatabaseAPI.LoadLevelsDatabase(MidsContext.Config.DataPath)) return;
            MidsContext.Character?.ResetLevel();
            PowerModified(markModified: false);
            UpdateDmBuffer();
        }

        private void UpdateModeInfo()
        {
            if (MidsContext.Config == null) return;
            switch (MidsContext.Config.BuildMode)
            {
                case Enums.dmModes.LevelUp:
                    ibModeEx.ToggleText.ToggledOff = MainModule.MidsController.Toon is { Complete: false }
                        ? $"Level-Up: {MidsContext.Character?.Level + 1}"
                        : @"Level-Up";
                    if (ibModeEx.CurrentText != ibModeEx.ToggleText.ToggledOff)
                    {
                        ibModeEx.CurrentText = MainModule.MidsController.Toon is { Complete: false }
                            ? $"Level-Up: {MidsContext.Character?.Level + 1}"
                            : @"Level-Up";
                    }

                    ibModeEx.ToggleState = ImageButtonEx.States.ToggledOff;
                    break;
                case Enums.dmModes.Normal:
                    ibModeEx.ToggleText.ToggledOn = @"Normal";
                    ibModeEx.ToggleState = ImageButtonEx.States.ToggledOn;
                    break;
                case Enums.dmModes.Respec:
                    ibModeEx.ToggleText.Indeterminate = @"Respec";
                    ibModeEx.ToggleState = ImageButtonEx.States.Indeterminate;
                    break;
                case Enums.dmModes.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ibAccoladesEx_OnClick(object? sender, EventArgs e)
        {
            var flag = false;
            if (fAccolade == null)
                flag = true;
            else if (fAccolade.IsDisposed)
                flag = true;
            if (flag)
            {
                var iParent = this;
                var power = MainModule.MidsController.Toon != null && !MainModule.MidsController.Toon.IsHero()
                    ? DatabaseAPI.Database.Power[DatabaseAPI.NidFromStaticIndexPower(3258)]
                    : DatabaseAPI.Database.Power[DatabaseAPI.NidFromStaticIndexPower(3257)];
                var iPowers = new List<IPower?>();
                if (power != null)
                {
                    var num = power.NIDSubPower.Length - 1;
                    for (var index = 0; index <= num; ++index)
                    {
                        var thisPower = DatabaseAPI.Database.Power[power.NIDSubPower[index]];
                        if (thisPower != null && (thisPower.ClickBuff || thisPower.PowerType == Enums.ePowerType.Auto_ | thisPower.PowerType == Enums.ePowerType.Toggle))
                            iPowers.Add(thisPower);
                    }
                }

                fAccolade = new frmAccolade(iParent, iPowers);
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

        private void AccoladesWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ibAccoladesEx_OnClick(sender, EventArgs.Empty);
        }

        private static int ArchetypeIndirectToIndex(int iIndirect)
        {
            var num1 = -1;
            for (var index = 0; index <= DatabaseAPI.Database.Classes.Length - 1; ++index)
            {
                if (!DatabaseAPI.Database.Classes[index].Playable)
                    continue;
                ++num1;
                if (num1 == iIndirect)
                    return index;
            }

            return 0;
        }

        private void AssemblePowerList(ListLabelV3 llPower, IPowerset? Powerset)
        {
            if (Powerset == null || Powerset.Powers?.Length < 1)
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
                if (Powerset.nIDTrunkSet > -1)
                {
                    var powerset = DatabaseAPI.Database.Powersets[Powerset.nIDTrunkSet];
                    var iItem1 = new ListLabelV3.ListLabelItemV3(powerset.DisplayName, ListLabelV3.LLItemState.Heading,
                        Powerset.nIDTrunkSet,
                        -1, -1, "", ListLabelV3.LLFontFlags.Bold, ListLabelV3.LLTextAlign.Center);
                    llPower.AddItem(iItem1);
                    for (var iIDXPower = 0; iIDXPower < powerset.Powers.Length; iIDXPower++)
                    {
                        if (powerset.Powers[iIDXPower].Level <= 0) continue;
                        message = "";
                        var iItem2 = new ListLabelV3.ListLabelItemV3(powerset.Powers[iIDXPower].DisplayName,
                            MainModule.MidsController.Toon.PowerState(powerset.Powers[iIDXPower].PowerIndex,
                                ref message), Powerset.nIDTrunkSet, iIDXPower, powerset.Powers[iIDXPower].PowerIndex,
                            "",
                            ListLabelV3.LLFontFlags.Bold)
                        {
                            Bold = MidsContext.Config.RtFont.PairedBold
                        };
                        if (iItem2.ItemState == ListLabelV3.LLItemState.Invalid)
                            iItem2.Italic = true;
                        llPower.AddItem(iItem2);
                    }

                    var iItem = new ListLabelV3.ListLabelItemV3(Powerset.DisplayName, ListLabelV3.LLItemState.Heading,
                        Powerset.nID, -1, -1,
                        "", ListLabelV3.LLFontFlags.Bold, ListLabelV3.LLTextAlign.Center);
                    llPower.AddItem(iItem);
                }

                if (Powerset.Powers != null)
                    for (var iIDXPower = 0; iIDXPower < Powerset.Powers.Length; iIDXPower++)
                    {
                        if (Powerset.Powers[iIDXPower].Level <= 0 || !Powerset.Powers[iIDXPower]
                            .AllowedForClass(MidsContext.Character.Archetype.Idx))
                            continue;
                        message = "";
                        var targetPs = MainModule.MidsController.Toon.PowerState(Powerset.Powers[iIDXPower].PowerIndex, ref message);
                        var power = Powerset.Powers[iIDXPower];
                        var iItem = new ListLabelV3.ListLabelItemV3(
                            Powerset.Powers[iIDXPower].DisplayName,
                            targetPs,
                            Powerset.nID,
                            iIDXPower,
                            power.PowerIndex, "", ListLabelV3.LLFontFlags.Bold)
                        {
                            Bold = MidsContext.Config.RtFont.PairedBold
                        };
                        if (iItem.ItemState == ListLabelV3.LLItemState.Invalid) iItem.Italic = true;
                        llPower.AddItem(iItem);
                    }

                llPower.SuspendRedraw = false;
            }
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

        private void cbAncillary_DrawItem(object sender, DrawItemEventArgs e)
        {
            cbDrawItem(CbtAncillary.Value, Enums.ePowerSetType.Ancillary, e);
        }

        private void cbPools_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null) return;
            
            var combo = (ComboBox) sender;
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

        private void cbAncillery_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            ChangeSets();
            UpdatePowerLists();
            if (!MidsContext.Config.UseOldTotalsWindow)
                frmTotalsV2.SetTitle(fTotals2);
        }

        private void cbAT_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.DrawBackground();
            using var solidBrush = new SolidBrush(Color.Black);
            if (e.Index > -1)
            {
                var cbAT = new ComboBoxT<Archetype>(this.cbAT);
                var index = ArchetypeIndirectToIndex(e.Index);
                var destRect = new RectangleF(e.Bounds.X + 1, e.Bounds.Y, 16f, 16f);
                var srcRect = new RectangleF(index * 16, 0.0f, 16f, 16f);
                e.Graphics.DrawImage(I9Gfx.Archetypes.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
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
                return;
            ShowPopup(-1, CbtAT.Value.SelectedItem.Idx, cbAT.Bounds);
        }

        private void cbAT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate) return;
            NewToon(false);
            //SetFormHeight();
            //PerformAutoScale();
            SetAncilPoolHeight();
            GetBestDamageValues();
            if (!MidsContext.Config.UseOldTotalsWindow)
                frmTotalsV2.SetTitle(fTotals2);
        }

        private static void cbDrawItem(
            ComboBoxT<string> target,
            Enums.ePowerSetType SetType,
            DrawItemEventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.DrawBackground();
            using var solidBrush = new SolidBrush(Color.Black);
            var powersetIndexes = DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, SetType);
            if ((e.Index > -1) & (e.Index < powersetIndexes.Length))
            {
                var nId = powersetIndexes[e.Index].nID;
                var destRect = new RectangleF();
                ref var local1 = ref destRect;
                local1 = new RectangleF(e.Bounds.X + 1, e.Bounds.Y, 16f, 16f);
                var srcRect = new RectangleF(nId * 16, 0.0f, 16f, 16f);
                if ((e.State & DrawItemState.ComboBoxEdit) > DrawItemState.None)
                {
                    if (e.Graphics.MeasureString(target[e.Index], e.Font).Width <= e.Bounds.Width - 10)
                        e.Graphics.DrawImage(I9Gfx.Powersets.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
                    else
                        destRect.Width = 0.0f;
                }
                else
                {
                    e.Graphics.DrawImage(I9Gfx.Powersets.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
                }

                using var format = new StringFormat(StringFormatFlags.NoWrap)
                {
                    LineAlignment = StringAlignment.Center
                };
                var layout = new RectangleF(e.Bounds.X + destRect.X + destRect.Width, e.Bounds.Y, e.Bounds.Width,
                    e.Bounds.Height);
                e.Graphics.DrawString(target[e.Index], e.Font, solidBrush, layout, format);
            }

            e.DrawFocusRectangle();
        }

        private void cbOrigin_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.DrawBackground();
            using var solidBrush = new SolidBrush(Color.Black);
            if (e.Index > -1)
            {
                var cmbOrigin = GetCbOrigin();
                var destRect = new RectangleF(e.Bounds.X + 1, e.Bounds.Y, 16f, 16f);
                var srcRect = new RectangleF(DatabaseAPI.GetOriginIDByName(cmbOrigin[e.Index]) * 16, 0.0f, 16f, 16f);
                e.Graphics.DrawImage(I9Gfx.Origins.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
                using var format = new StringFormat(StringFormatFlags.NoWrap)
                {
                    LineAlignment = StringAlignment.Center
                };
                var layoutRectangle = new RectangleF(e.Bounds.X + destRect.X + destRect.Width, e.Bounds.Y,
                    e.Bounds.Width - (destRect.X + destRect.Width), e.Bounds.Height);
                e.Graphics.DrawString(cmbOrigin[e.Index], e.Font, solidBrush, layoutRectangle, format);
            }

            e.DrawFocusRectangle();
        }

        private void cbOrigin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            MidsContext.Character.Origin = cbOrigin.SelectedIndex;
            I9Gfx.SetOrigin(cbOrigin.SelectedItem.ToStringOrNull());
            DisplayName();
        }

        private void cbPool0_DrawItem(object sender, DrawItemEventArgs e)
        {
            cbDrawItem(CbtPool0.Value, Enums.ePowerSetType.Pool, e);
        }

        private void cbPool0_MouseLeave(object sender, EventArgs e)
        {
            HidePopup();
        }

        private void cbPool0_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            ChangeSets();
            UpdatePowerLists();
        }

        private void cbPool1_DrawItem(object sender, DrawItemEventArgs e)
        {
            cbDrawItem(CbtPool1.Value, Enums.ePowerSetType.Pool, e);
        }

        private void cbPool1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            ChangeSets();
            UpdatePowerLists();
        }

        private void cbPool2_DrawItem(object sender, DrawItemEventArgs e)
        {
            cbDrawItem(CbtPool2.Value, Enums.ePowerSetType.Pool, e);
        }

        private void cbPool2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            ChangeSets();
            UpdatePowerLists();
        }

        private void cbPool3_DrawItem(object sender, DrawItemEventArgs e)
        {
            cbDrawItem(CbtPool3.Value, Enums.ePowerSetType.Pool, e);
        }

        private void cbPool3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            ChangeSets();
            UpdatePowerLists();
        }

        private void cbPrimary_DrawItem(object sender, DrawItemEventArgs e)
        {
            cbDrawItem(CbtPrimary.Value, Enums.ePowerSetType.Primary, e);
        }

        private void cbPrimary_MouseLeave(object sender, EventArgs e)
        {
            HidePopup();
        }

        private void cbPrimary_MouseMove(object sender, MouseEventArgs e)
        {
            if (MidsContext.Character == null || MidsContext.Character.Archetype == null || cbPrimary.SelectedIndex < 0)
                return;
            var ExtraString =
                "This is your primary powerset. This powerset can be changed after a build has been started, and any placed powers will be swapped out for those in the new set.";
            ShowPopup(
                DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, Enums.ePowerSetType.Primary)[
                    cbPrimary.SelectedIndex].nID,
                MidsContext.Character.Archetype.Idx, cbPrimary.Bounds, ExtraString);
        }

        private void cbPrimary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            ChangeSets();
            UpdatePowerLists();
            if (!MidsContext.Config.UseOldTotalsWindow)
                frmTotalsV2.SetTitle(fTotals2);
        }

        private void cbSecondary_DrawItem(object sender, DrawItemEventArgs e)
        {
            cbDrawItem(CbtSecondary.Value, Enums.ePowerSetType.Secondary, e);
        }

        private void cbSecondary_MouseLeave(object sender, EventArgs e)
        {
            HidePopup();
        }

        private void cbSecondary_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Archetype.Idx < 0 ||
                cbSecondary.SelectedIndex < 0)
                return;
            var ExtraString = MidsContext.Character.Powersets[0].nIDLinkSecondary <= -1
                ? "This is your secondary powerset. This powerset can be changed after a build has been started, and any placed powers will be swapped out for those in the new set."
                : "This is your secondary powerset. This powerset is linked to your primary set and cannot be changed independantly. However, it can be changed by selecting a different primary powerset.";
            ShowPopup(
                DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, Enums.ePowerSetType.Secondary)[
                    cbSecondary.SelectedIndex].nID,
                MidsContext.Character.Archetype.Idx, cbSecondary.Bounds, ExtraString);
        }

        private void cbSecondary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            ChangeSets();
            UpdatePowerLists();
            if (!MidsContext.Config.UseOldTotalsWindow)
                frmTotalsV2.SetTitle(fTotals2);
        }

        private void ChangeSets()
        {
            MainUILogic.ChangeSets(MainModule.MidsController.Toon, MidsContext.Character,
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

        private void clearPower(PowerEntry?[] tp, int pwrIdx)
        {
            tp[pwrIdx].Slots = Array.Empty<SlotEntry>();
            tp[pwrIdx].SubPowers = Array.Empty<PowerSubEntry>();
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
            int num;
            switch (msgBoxResult)
            {
                case DialogResult.Cancel:
                    return true;
                case DialogResult.Yes:
                    num = doSave() ? 1 : 0;
                    break;
                default:
                    num = 1;
                    break;
            }

            return num == 0;
        }

        private bool ComboCheckAT(Archetype[] playableClasses)
        {
            var cbtAT = CbtAT.Value;
            if (cbtAT.Count != playableClasses.Length) return true;

            var num = playableClasses.Length - 1;
            for (var index = 0; index <= num; ++index)
                if (cbtAT[index].Idx != playableClasses[index].Idx)
                    return true;

            return false;
        }

        private bool ComboCheckOrigin()
        {
            var cbtOrigin = GetCbOrigin();
            if (cbtOrigin.Count != MidsContext.Character.Archetype.Origin.Length) return true;

            if (cbtOrigin.Count > 1)
                return false;
            var num = MidsContext.Character.Archetype.Origin.Length - 1;
            for (var index = 0; index <= num; ++index)
                if (cbtOrigin[index] != MidsContext.Character.Archetype.Origin[index])
                    return true;

            return false;
        }

        private static void ComboCheckPool(ComboBoxT<string> iCB, Enums.ePowerSetType iSetType)
        {
            var powersetNames = DatabaseAPI.GetPowersetNames(MidsContext.Character.Archetype.Idx, iSetType);
            var needsComboUpdate = iCB.Items.Count != powersetNames.Length || !iCB.Items.SequenceEqual(powersetNames);
            if (!needsComboUpdate)
                return;
            iCB.BeginUpdate();
            iCB.Clear();
            iCB.AddRange(powersetNames);
            iCB.EndUpdate();
        }

        private static void ComboCheckPS(
            ComboBoxT<string> iCB,
            Enums.PowersetType iSetID,
            Enums.ePowerSetType iSetType)
        {
            var powersetNames = DatabaseAPI.GetPowersetNames(MidsContext.Character.Archetype.Idx, iSetType);
            var needsComboUpdate = iCB.Items.Count != powersetNames.Length || !iCB.Items.SequenceEqual(powersetNames);
            if (needsComboUpdate)
            {
                iCB.BeginUpdate();
                iCB.Clear();
                iCB.AddRange(powersetNames);
                iCB.EndUpdate();
            }

            var powersetIndexes = DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, iSetType);
            iCB.SelectedIndex =
                DatabaseAPI.ToDisplayIndex(MidsContext.Character.Powersets[(int)iSetID], powersetIndexes);
        }

        private void command_ForumImport()
        {
            if (MainModule.MidsController.Toon.Locked & FileModified)
            {
                FloatTop(false);
                var msgBoxResult = MessageBox.Show(@"Current character data will be discarded, are you sure?", @"Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                FloatTop(true);
                if (msgBoxResult == DialogResult.No)
                    return;
            }

            FloatTop(false);
            FileModified = false;
            var loaded = false;
            if (MessageBox.Show(@"Copy the build data on the forum to the clipboard. When that's done, click on OK.", @"Standing By", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK)
                return;
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
                    if (str != null && (str.Contains("MxDz") || str.Contains("MxDu")))
                    {
                        Stream? mStream = new MemoryStream(new ASCIIEncoding().GetBytes(str));
                        loaded = MainModule.MidsController.Toon.Load("", ref mStream);
                    }
                    else if (str != null && (str.Contains("Character Profile:") || str.Contains("build.txt")))
                    {
                        GameImport(str);
                        loaded = true;
                    }

                    if (!loaded)
                        loaded = MainModule.MidsController.Toon.StringToInternalData(str);
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
                        DoRedraw();
                    UpdateColors();
                    FloatTop(true);
                    SetTitleBar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Message: {ex.Message}\r\nTrace: {ex.StackTrace}");
                FloatTop(true);
            }
        }


        private void command_New()
        {
            if (MainModule.MidsController.Toon.Locked & FileModified)
            {
                FloatTop(false);
                var msgBoxResult = MessageBox.Show(@"Current character data will be discarded, are you sure?", @"Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                FloatTop(true);
                if (msgBoxResult == DialogResult.No)
                    return;
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

        internal void DataView_SlotFlip(int PowerIndex)
        {
            StartFlip(PowerIndex);
        }

        internal void DataView_SlotUpdate()
        {
            DoRedraw();
            RefreshInfo();
        }


        private static PowerEntry?[] DeepCopyPowerList()
        {
            return MidsContext.Character.CurrentBuild.Powers.Select(x => (PowerEntry)x.Clone()).ToArray();
        }

        private Rectangle Dilate(Rectangle irect, int iAdd)
        {
            irect.X -= iAdd;
            irect.Y -= iAdd;
            irect.Height += iAdd * 2;
            irect.Width += iAdd * 2;
            return irect;
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
                return;
            txtName.Text = ch.Name;
        }

        private void doFlipStep()
        {
            if (!FlipActive)
                return;
            var point1 = new Point();
            var currentBuild = MidsContext.Character.CurrentBuild;
            var power = currentBuild.Powers[FlipPowerID];
            var point2 = drawing.DrawPowerSlot(ref power);
            var index = -1;
            var Enh1 = -1;
            var Enh2 = -1;
            I9Slot i9Slot1 = null;
            I9Slot i9Slot2 = null;
            var recolorIa = clsDrawX.GetRecolorIa(MainModule.MidsController.Toon.IsHero());
            using var solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
            var num1 = FlipSlotState.Length - 1;
            Rectangle rectangle1;
            var slotId = -1;
            for (var i = 0; i <= num1; ++i)
            {
                point1.X = (int)Math.Round(point2.X - 30 + (drawing.SzPower.Width - drawing.szSlot.Width * 6) / 2.0);
                point1.Y = point2.Y + clsDrawX.OffsetY;
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
                    continue;
                var rectangle2 = new Rectangle((int)Math.Round(rectangle1.X + (30.0 - 30.0 * num2) / 2.0),
                    rectangle1.Y,
                    (int)Math.Round(30.0 * num2), 30);
                rectangle2 = drawing.ScaleDown(rectangle2);
                rectangle1 = drawing.ScaleDown(rectangle1);
                if (index > -1)
                {
                    var graphics = drawing.bxBuffer.Graphics;
                    if (i9Slot1 != null)
                        I9Gfx.DrawFlippingEnhancement(ref graphics, rectangle1, num2,
                            DatabaseAPI.Database.Enhancements[index].ImageIdx,
                            I9Gfx.ToGfxGrade(DatabaseAPI.Database.Enhancements[index].TypeID, i9Slot1.Grade));
                }
                else
                {
                    drawing.bxBuffer.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, rectangle2, 0, 0, 30, 30,
                        GraphicsUnit.Pixel, recolorIa);
                }

                if ((MidsContext.Config.CalcEnhLevel == Enums.eEnhRelative.None) | (slot.Level >= MidsContext.Config.ForceLevel) | ((drawing.InterfaceMode == Enums.eInterfaceMode.PowerToggle) & !powerEntry.StatInclude))
                {
                    rectangle2.Inflate(1, 1);
                    drawing.bxBuffer.Graphics.FillEllipse(solidBrush, rectangle2);
                }

                if (!((myDataView == null) | (i9Slot1 == null) | (i9Slot2 == null)))
                {
                    myDataView.FlipStage(i, Enh1, Enh2, num2, powerEntry.NIDPower, i9Slot1.Grade, i9Slot2.Grade);
                }
            }

            rectangle1 = new Rectangle(point1.X - 1, point1.Y - 1, drawing.SzPower.Width + 1,
                drawing.szSlot.Height + 1);
            drawing.Refresh(drawing.ScaleDown(rectangle1));
            if (FlipSlotState[FlipSlotState.Length - 1] >= FlipSteps)
                EndFlip();
        }

        private bool DoOpen(string fName)
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
                if (MidsContext.Config != null) MidsContext.Config.LastFileName = "";
            }
            else
            {
                LastFileName = fName;
                if (!fName.EndsWith("mids_build.mxd"))
                {
                    if (MidsContext.Config != null) MidsContext.Config.LastFileName = fName;
                }
            }

            FileModified = false;
            if (drawing != null) drawing.Highlight = -1;
            switch (MidsContext.Character?.Archetype?.DisplayName)
            {
                case "Mastermind":
                    ibPetsEx.Visible = true;
                    ibPetsEx.Enabled = true;
                    break;
                default:
                    ibPetsEx.Visible = false;
                    ibPetsEx.Enabled = false;
                    break;
            }

            myDataView.Clear();
            MidsContext.Character?.ResetLevel();
            PowerModified(false);
            UpdateControls(true);
            SetTitleBar();
            Application.DoEvents();
            GetBestDamageValues();
            UpdateColors();
            FloatUpdate(true);

            return true;
        }

        private bool DoLoad(Stream? data)
        {
            DataViewLocked = false;
            NewToon(true, true);
            if (data == null) return true;
            var loaded = MainModule.MidsController.Toon != null && MainModule.MidsController.Toon.Load("", ref data);

            if (!loaded) return true;
            Debug.WriteLine("Loaded");
            FileModified = false;
            if (drawing != null) drawing.Highlight = -1;
            switch (MidsContext.Character?.Archetype?.DisplayName)
            {
                case "Mastermind":
                    ibPetsEx.Visible = true;
                    ibPetsEx.Enabled = true;
                    break;
                default:
                    ibPetsEx.Visible = false;
                    ibPetsEx.Enabled = false;
                    break;
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
            Debug.WriteLine(data);
            DataViewLocked = false;
            NewToon(true, true);
            if (data == null || (!data.Contains("MxDz") && !data.Contains("MxDu"))) return true;
            Stream? mStream = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var loaded = MainModule.MidsController.Toon != null && MainModule.MidsController.Toon.Load("", ref mStream);

            if (!loaded) return true;
            Debug.WriteLine("Loaded");
            FileModified = false;
            if (drawing != null) drawing.Highlight = -1;
            switch (MidsContext.Character?.Archetype?.DisplayName)
            {
                case "Mastermind":
                    ibPetsEx.Visible = true;
                    ibPetsEx.Enabled = true;
                    break;
                default:
                    ibPetsEx.Visible = false;
                    ibPetsEx.Enabled = false;
                    break;
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

        private void command_Load(string data)
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
                loaded = MainModule.MidsController.Toon.StringToInternalData(data);
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
                DoRedraw();
            UpdateColors();
            FloatTop(true);
            SetTitleBar();
        }

        public void DoRedraw()
        {
            //var t = new Stopwatch();
            //t.Start();
            if (drawing == null) return;
            NoResizeEvent = true;
            pnlGFX.Width = pnlGFXFlow.Width - 26;
            NoResizeEvent = false;
            drawing.FullRedraw();
            //t.Stop();
            //Debug.WriteLine($"DoRedraw: {t.ElapsedMilliseconds} ms");
        }

        private void DoResize(bool forceResize = false)
        {
            if (drawing == null) return;
            var prevDrawingWidth = pnlGFX.Width;
            var clientWidth = ClientSize.Width - pnlGFXFlow.Left;
            var clientHeight = ClientSize.Height - pnlGFXFlow.Top;
            pnlGFXFlow.Width = clientWidth;
            pnlGFXFlow.Height = clientHeight;
            var drawingArea = drawing.GetDrawingArea();
            var drawingWidth = pnlGFXFlow.Width - 30;
            var prevScale = prevDrawingWidth / (double)drawingArea.Width;
            var scale = drawingWidth / (double)drawingArea.Width;
            NoResizeEvent = prevScale >= 2 & scale >= 2;
            if (NoResizeEvent & !forceResize) return;
            scale = Math.Max(scale, 2);
            int drawingHeight;
            pnlGFX.Width = drawingWidth;
            if (MidsContext.Character.CurrentBuild.Powers.Count < 41)
            {
                drawingHeight = drawingArea.Height;
                pnlGFX.Height = drawingHeight;
            }
            else
            {
                drawingHeight = (int)Math.Round(drawingArea.Height * scale);
                pnlGFX.Height = drawingHeight;
            }

            drawing.bxBuffer.Size = pnlGFX.Size;
            Control pnlGfx = pnlGFX;
            drawing.ReInit(pnlGfx);
            pnlGFX = (pnlGFX)pnlGfx;
            pnlGFX.Image = drawing.bxBuffer.Bitmap;
            drawing.SetScaling(scale < 1 ? pnlGFX.Size : drawing.bxBuffer.Size);
            ReArrange(false);
            NoResizeEvent = true;
            DoRedraw();
        }

        public void DoRefresh()
        {
            drawing.Refresh(new Rectangle(0, 0, pnlGFX.Width, pnlGFX.Height));
            pnlGFX.Refresh();
        }

        private bool doSave()
        {
            if (string.IsNullOrEmpty(LastFileName))
                return doSaveAs();
            if (LastFileName.Length > 3 && LastFileName.ToUpper().EndsWith(".TXT")) return doSaveAs();

            if (!MainModule.MidsController.Toon.Save(LastFileName))
                return false;
            FileModified = false;
            return true;
        }

        private bool doSaveAs()
        {
            FloatTop(false);
            if (LastFileName != string.Empty)
            {
                DlgSave.FileName = FileIO.StripPath(LastFileName);
                if (DlgSave.FileName.Length > 3 && DlgSave.FileName.ToUpper().EndsWith(".TXT"))
                {
                    DlgSave.FileName = DlgSave.FileName.Substring(0, DlgSave.FileName.Length - 3) + DlgSave.DefaultExt;
                }

                DlgSave.InitialDirectory = LastFileName.Substring(0, LastFileName.LastIndexOf("\\", StringComparison.Ordinal));
            }
            else if (!string.IsNullOrWhiteSpace(MidsContext.Character.Name))
            {
                if (MidsContext.Character.Archetype.ClassType == Enums.eClassType.VillainEpic)
                {
                    DlgSave.FileName = MidsContext.Character.Name + " - Arachnos " + MidsContext.Character.Powersets[0]
                        .DisplayName
                        .Replace(" Training", string.Empty).Replace("Arachnos ", string.Empty);
                }
                else
                {
                    DlgSave.FileName = MidsContext.Character.Name + " - " +
                                       MidsContext.Character.Archetype.DisplayName + " (" +
                                       MidsContext.Character.Powersets[0].DisplayName + ")";
                }
            }
            else if (MidsContext.Character.Archetype.ClassType == Enums.eClassType.VillainEpic)
            {
                DlgSave.FileName = "Arachnos " + MidsContext.Character.Powersets[0].DisplayName
                    .Replace(" Training", string.Empty)
                    .Replace("Arachnos ", string.Empty);
            }
            else
            {
                DlgSave.FileName = MidsContext.Character.Archetype.DisplayName;
                var dlgSave = DlgSave;
                dlgSave.FileName = dlgSave.FileName + " - " + MidsContext.Character.Powersets[0].DisplayName + " - " + MidsContext.Character.Powersets[1].DisplayName;
            }

            if (DlgSave.ShowDialog() == DialogResult.OK)
            {
                if (!MainModule.MidsController.Toon.Save(DlgSave.FileName))
                    return false;
                FileModified = false;
                LastFileName = DlgSave.FileName;
                MidsContext.Config.LastFileName = DlgSave.FileName;
                SetTitleBar();
                FloatTop(true);
                return true;
            }

            FloatTop(true);
            return false;
        }

        private void dvAnchored_Float()
        {
            FloatingDataForm = new frmFloatingStats(this)
            {
                Left = Left + dvAnchored.Left,
                Top = Top + dvAnchored.Top,
                dvFloat =
                {
                    VisibleSize = dvAnchored.VisibleSize
                }
            };
            myDataView = FloatingDataForm.dvFloat;
            myDataView.TabPage = dvAnchored.TabPage;
            FloatingDataForm.dvFloat.Init();
            FloatingDataForm.dvFloat.SetFontData();
            myDataView.BackColor = BackColor;
            myDataView.DrawVillain = !MainModule.MidsController.Toon.IsHero();
            dvAnchored.Visible = false;
            pnlGFX.Select();
            FloatingDataForm.Show();
            RefreshInfo();
            //ReArrange(false);
            if (dvLastPower <= -1)
                return;
            Info_Power(dvLastPower, dvLastEnh, dvLastNoLev, DataViewLocked);
        }

        private void dvAnchored_Move()
        {
            PriSec_ExpandChanged(true);
            ReArrange(false);
        }

        private void dvAnchored_SizeChange(Size newSize, bool Compact)
        {
            ReArrange(false);
            if (!(MainModule.MidsController.IsAppInitialized & Visible))
                return;
            MidsContext.Config.DvState = dvAnchored.VisibleSize;
        }

        private void dvAnchored_TabChanged(int Index)
        {
            SetDataViewTab(Index);
        }

        private void dvAnchored_Unlock()
        {
            DataViewLocked = false;
            if (dvLastPower <= -1)
                return;
            Info_Power(dvLastPower, dvLastEnh, dvLastNoLev, DataViewLocked);
        }

        private bool EditAccoladesOrTemps(int hIDPower)
        {
            if (hIDPower <= -1 || MidsContext.Character.CurrentBuild.Powers[hIDPower].SubPowers.Length <= 0)
                return false;

            var iPowers = new List<IPower?>();
            var num1 = MidsContext.Character.CurrentBuild.Powers[hIDPower].SubPowers.Length - 1;
            for (var index = 0; index <= num1; ++index)
                iPowers.Add(DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[hIDPower].SubPowers[index].nIDPower]);
            using var frmAccolade = new frmAccolade(this, iPowers)
            {
                Text = DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[hIDPower].NIDPower].DisplayName
            };
            frmAccolade.ShowDialog(this);
            EnhancementModified();
            LastClickPlacedSlot = false;
            return true;
        }

        private void EndFlip()
        {
            FlipActive = false;
            tmrGfx.Enabled = false;
            FlipPowerID = -1;
            FlipSlotState = new int[0];
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
                if (DatabaseAPI.Database.Levels[i].Slots <= 0) continue;

                for (var j = 0; j < DatabaseAPI.Database.Levels[i].Slots; j++)
                {
                    slotLevels.Add(i + 1);
                }
            }

            return slotLevels.ToArray();
        }

        private void FixPrimarySecondaryHeight()
        {
            if (dvAnchored.Visible & dvAnchored.Bounds.IntersectsWith(dvAnchored.SnapLocation))
            {
                var size = ClientSize;
                var height = size.Height - dvAnchored.Height - cbPrimary.Top - cbPrimary.Height - 10;
                if (llPrimary.DesiredHeight < height)
                {
                    size = llPrimary.SizeNormal;
                    llPrimary.SizeNormal = new Size(size.Width, llPrimary.DesiredHeight);
                }
                else
                {
                    if (height < 70)
                        height = 70;
                    size = new Size(llPrimary.SizeNormal.Width, height);
                    llPrimary.SizeNormal = size;
                }

                if (llSecondary.DesiredHeight < height)
                {
                    size = new Size(llSecondary.SizeNormal.Width, llSecondary.DesiredHeight);
                    llSecondary.SizeNormal = size;
                }
                else
                {
                    if (height < 70)
                        height = 70;
                    size = new Size(llSecondary.SizeNormal.Width, height);
                    llSecondary.SizeNormal = size;
                }
            }
            else
            {
                var size = new Size(llPrimary.SizeNormal.Width, llPrimary.DesiredHeight);
                llPrimary.SizeNormal = size;
                size = new Size(llSecondary.SizeNormal.Width, llSecondary.DesiredHeight);
                llSecondary.SizeNormal = size;
            }
        }

        private void FixStatIncludes()
        {
            if (MainModule.MidsController.Toon == null)
                return;
            for (var index = 0; index <= MidsContext.Character.CurrentBuild.Powers.Count - 1; ++index)
            {
                if (MidsContext.Character.CurrentBuild.Powers[index] != null)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[index].PowerSet == null)
                        continue;
                    switch (MidsContext.Character.CurrentBuild.Powers[index].PowerSet.DisplayName)
                    {
                        case "Temporary Powers":
                            if (ibTempPowersEx.ToggleState == ImageButtonEx.States.ToggledOff)
                            {
                                MidsContext.Character.CurrentBuild.Powers[index].StatInclude = false;
                            }
                            else if (ibTempPowersEx.ToggleState == ImageButtonEx.States.ToggledOn)
                            {
                                MidsContext.Character.CurrentBuild.Powers[index].StatInclude = true;
                            }
                            break;
                    }
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
                    return;
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
                    return;
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
                    return;
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
                    return;
                fSalvageHud.Hide();
                fSalvageHud.Dispose();
                fSalvageHud = null;
            }
        }

        internal void FloatDPSCalc(bool showow)
        {
            if (showow)
            {
                if (fDPSCalc == null)
                    fDPSCalc = new frmDPSCalc(this);
                fDPSCalc.SetLocation();
                fDPSCalc.Show();
                FloatUpdate();
                fDPSCalc.Activate();
            }
            else
            {
                if (fDPSCalc == null)
                    return;
                fDPSCalc.Hide();
                fDPSCalc = null;
            }
        }

        internal void FloatSetFinder(bool show)
        {
            if (show)
            {
                if (fSetFinder == null)
                    fSetFinder = new frmSetFind(this);
                fSetFinder.Show();
                fSetFinder.Activate();
            }
            else
            {
                if (fSetFinder == null)
                    return;
                fSetFinder.Hide();
                fSetFinder.Dispose();
                fSetFinder = null;
            }
        }

        internal void FloatSets(bool show)
        {
            if (show)
            {
                if (fSets == null)
                {
                    var iParent = this;
                    fSets = new frmSetViewer(iParent);
                }

                fSets.SetLocation();
                fSets.Show();
                FloatUpdate();
                fSets.Activate();
            }
            else
            {
                if (fSets == null)
                    return;
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
                    return;
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
                        fSets.TopMost = false;
                }

                if (fGraphStats != null)
                {
                    top_fGraphStats = fGraphStats.TopMost;
                    if (fGraphStats.TopMost)
                        fGraphStats.TopMost = false;
                }

                if (fGraphCompare != null)
                {
                    top_fGraphCompare = fGraphCompare.TopMost;
                    if (fGraphCompare.TopMost)
                        fGraphCompare.TopMost = false;
                }

                if (MidsContext.Config.UseOldTotalsWindow)
                {
                    if (fTotals != null)
                    {
                        top_fTotals = fTotals.TopMost;
                        if (fTotals.TopMost)
                            fTotals.TopMost = false;
                    }
                }
                else
                {
                    if (fTotals2 != null)
                    {
                        top_fTotals = fTotals2.TopMost;
                        if (fTotals2.TopMost)
                            fTotals2.TopMost = false;
                    }
                }

                if (fRecipe != null)
                {
                    top_fRecipe = fRecipe.TopMost;
                    if (fRecipe.TopMost)
                        fRecipe.TopMost = false;
                }

                if (fData != null)
                {
                    top_fData = fData.TopMost;
                    if (fData.TopMost)
                        fData.TopMost = false;
                }

                if (fSetFinder == null)
                    return;
                top_fSetFinder = fSetFinder.TopMost;
                if (fSetFinder.TopMost)
                    fSetFinder.TopMost = false;
            }
            else
            {
                BringToFront();
                if (fSets != null && fSets.TopMost != top_fSets)
                {
                    fSets.TopMost = top_fSets;
                    if (fSets.TopMost)
                        fSets.BringToFront();
                }

                if (fGraphStats != null && fGraphStats.TopMost != top_fGraphStats)
                {
                    fGraphStats.TopMost = top_fGraphStats;
                    if (fGraphStats.TopMost)
                        fGraphStats.BringToFront();
                }

                if (fGraphCompare != null && fGraphCompare.TopMost != top_fGraphCompare)
                {
                    fGraphCompare.TopMost = top_fGraphCompare;
                    if (fGraphCompare.TopMost)
                        fGraphCompare.BringToFront();
                }

                if (MidsContext.Config.UseOldTotalsWindow)
                {
                    if (fTotals != null && fTotals.TopMost != top_fTotals)
                    {
                        fTotals.TopMost = top_fTotals;
                        if (fTotals.TopMost)
                            fTotals.BringToFront();
                    }
                }
                else
                {
                    if (fTotals2 != null && fTotals2.TopMost != top_fTotals)
                    {
                        fTotals2.TopMost = top_fTotals;
                        if (fTotals2.TopMost)
                            fTotals2.BringToFront();
                    }
                }

                if (fRecipe != null && fRecipe.TopMost != top_fRecipe)
                {
                    fRecipe.TopMost = top_fRecipe;
                    if (fRecipe.TopMost)
                        fRecipe.BringToFront();
                }

                if (fData != null && fData.TopMost != top_fData)
                {
                    fData.TopMost = top_fData;
                    if (fData.TopMost)
                        fData.BringToFront();
                }

                if (fSetFinder == null || fSetFinder.TopMost == top_fSetFinder)
                    return;
                fSetFinder.TopMost = top_fSetFinder;
                if (fSetFinder.TopMost)
                    fSetFinder.BringToFront();
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
            fDPSCalc?.UpdateData();
            fData?.UpdateData(dvLastPower);
        }

        private void frmMain_Move(object? sender, EventArgs e)
        {
            MidsContext.Config.Bounds = DesktopBounds;
        }

        private void frmMain_Closed(object? sender, EventArgs e)
        {
            if (MidsContext.Config.FirstRun)
            {
                MidsContext.Config.FirstRun = false;
            }

            if (MidsContext.Config.MasterMode)
            {
                if (ProcessedFromCommand && ProcessedCommand.Contains("master"))
                {
                    MidsContext.Config.MasterMode = false;
                }
            }

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
            MidsContext.Config.SaveConfig(Serializer.GetSerializer());
        }

        private void frmMain_Closing(object? sender, FormClosingEventArgs e)
        {
            e.Cancel = CloseCommand();
        }

        private void frmMain_KeyDown(object? sender, KeyEventArgs e)
        {
            if (!(e.Alt & e.Control & e.Shift & (e.KeyCode == Keys.A)) && !(e.Control & e.Alt & e.Shift & (e.KeyCode == Keys.L)))
            {
                return;
            }

            if (e.Control & e.Alt & e.Shift & (e.KeyCode == Keys.A) && !MidsContext.Config.IsLcAdmin)
            {
                MidsContext.Config.MasterMode = MidsContext.Config.MasterMode switch
                {
                    false => true,
                    true => false
                };
            }

            if (e.Control & e.Alt & e.Shift & (e.KeyCode == Keys.L))
            {
                MidsContext.Config.MasterMode = MidsContext.Config.MasterMode switch
                {
                    false => true,
                    true => false
                };
                MidsContext.Config.IsLcAdmin = MidsContext.Config.MasterMode;
            }
            
            ToolStripSeparator5.Visible = MidsContext.Config.MasterMode;
            AdvancedToolStripMenuItem1.Visible = MidsContext.Config.MasterMode;
            SetTitleBar(MainModule.MidsController.Toon.IsHero());
        }

        private void frmMain_MouseWheel(object? sender, MouseEventArgs e)
        {
            dvAnchored.Info_txtLarge.Focus();
        }

        private void frmMain_Resize(object? sender, EventArgs e)
        {
            if (loading) return;

            UpdatePoolsPanelSize();
            if (dvAnchored != null)
            {
                dvAnchored.SetScreenBounds(ClientRectangle);
                if (WindowState == FormWindowState.Minimized)
                {
                    if (dvAnchored.Visible || FloatingDataForm == null)
                        return;
                    FloatingDataForm.Visible = false;
                    return;
                }

                if (!dvAnchored.Visible && FloatingDataForm != null)
                    FloatingDataForm.Visible = true;
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

        internal void DoCalcOptUpdates()
        {
            GetBestDamageValues();
            RefreshInfo();
            DisplayName();
            I9Picker.LastLevel = MidsContext.Config.I9.DefaultIOLevel + 1;
            myDataView?.SetFontData();
            if (dvLastPower > -1)
                Info_Power(dvLastPower, dvLastEnh, dvLastNoLev, DataViewLocked);
            if (drawing != null)
                DoRedraw();
            UpdateColors();
            SetTitleBar();
            frmTotalsV2.SetTitle(fTotals2);
        }

        private void GetBestDamageValues()
        {
            if (MainModule.MidsController.Toon == null)
                return;
            var highBase = 0.0f;
            for (var index = 0; index <= MidsContext.Character.Powersets[0].Powers.Length - 1; ++index)
            {
                var power = MidsContext.Character.Powersets[0].Powers[index];
                if (power.SkipMax)
                    continue;
                var damageValue = power.FXGetDamageValue();
                if (damageValue > (double)highBase)
                    highBase = damageValue;
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
            if ((MidsContext.Config.DamageMath.ReturnValue == ConfigData.EDamageReturn.DPS) | (MidsContext.Config.DamageMath.ReturnValue == ConfigData.EDamageReturn.DPA))
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

            var num = DatabaseAPI.Database.EnhancementSets[nIdSet].Enhancements.Length - 1;
            for (var index = 0; index <= num; ++index)
            {
                if (MidsContext.Character.CurrentBuild.EnhancementTest(slotIndex, hID, DatabaseAPI.Database.EnhancementSets[nIdSet].Enhancements[index], true))
                {
                    return DatabaseAPI.Database.EnhancementSets[nIdSet].Enhancements[index];
                }
            }

            return -1;
        }

        private bool GetPlayableClasses(Archetype a)
        {
            return a.Playable;
        }

        private I9Slot GetRepeatEnhancement(int powerIndex, int iSlotIndex)
        {
            if (LastEnhPlaced == null)
                return new I9Slot();
            if (MidsContext.Character.CurrentBuild.Powers[powerIndex].NIDPower < 0)
                return new I9Slot();
            if (LastEnhPlaced.Enh <= -1)
                return new I9Slot();
            if (DatabaseAPI.Database.Enhancements[LastEnhPlaced.Enh].TypeID != Enums.eType.SetO)
                return DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[powerIndex].NIDPower]
                    .IsEnhancementValid(LastEnhPlaced.Enh)
                    ? LastEnhPlaced
                    : new I9Slot();

            var firstValidSetEnh = GetFirstValidSetEnh(iSlotIndex, powerIndex);
            if (firstValidSetEnh <= -1)
                return new I9Slot();
            LastEnhPlaced.Enh = firstValidSetEnh;
            LastEnhPlaced.IOLevel = DatabaseAPI.Database.Enhancements[firstValidSetEnh]
                .CheckAndFixIOLevel(LastEnhPlaced.IOLevel);
            return LastEnhPlaced;
        }

        private void ibAlignmentEx_OnClick(object? sender, EventArgs e)
        {
            if (MidsContext.Character != null)
            {
                MidsContext.Character.Alignment = ibAlignmentEx.ToggleState switch
                {
                    ImageButtonEx.States.ToggledOff => Enums.Alignment.Hero,
                    ImageButtonEx.States.ToggledOn => Enums.Alignment.Villain,
                    _ => MidsContext.Character.Alignment
                };
                if (fAccolade != null)
                {
                    if (!fAccolade.IsDisposed)
                    {
                        fAccolade.Dispose();
                    }

                    var power = MidsContext.Character.IsHero()
                        ? DatabaseAPI.Database.Power[DatabaseAPI.NidFromStaticIndexPower(3258)]
                        : DatabaseAPI.Database.Power[DatabaseAPI.NidFromStaticIndexPower(3257)];
                    if (power != null)
                        foreach (var p in power.NIDSubPower)
                        {
                            if (MidsContext.Character.CurrentBuild != null && MidsContext.Character.CurrentBuild.PowerUsed(DatabaseAPI.Database.Power[p]))
                            {
                                MidsContext.Character.CurrentBuild.RemovePower(DatabaseAPI.Database.Power[p]);
                            }
                        }
                }
            }

            drawing?.ColorSwitch();
            fTotals?.Refresh();
            fTotals2?.Refresh();
            SetTitleBar();
            UpdateColors();
            DoRedraw();
        }

        private void HidePopup()
        {
            if (!PopUpVisible)
                return;
            PopUpVisible = false;
            var bounds = I9Popup.Bounds;
            bounds.X -= pnlGFXFlow.Left;
            bounds.Y -= pnlGFXFlow.Top;
            I9Popup.Visible = false;
            I9Popup.eIDX = -1;
            I9Popup.pIDX = -1;
            I9Popup.hIDX = -1;
            I9Popup.psIDX = -1;
            ActivePopupBounds = new Rectangle(0, 0, 0, 0);
            drawing?.Refresh(bounds);
        }

        private void I9Picker_EnhancementPicked(I9Slot e)
        {
            e.RelativeLevel = I9Picker.Ui.View.RelLevel;
            if (EnhancingSlot <= -1)
                return;
            // Let popup visible when repeating enhancement
            if (!gfxDrawing)
                HidePopup();

            var enhChanged = false;
            if (MidsContext.Character != null && MidsContext.Character.CurrentBuild.EnhancementTest(EnhancingSlot, EnhancingPower, e.Enh) | (e.Enh < 0))
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
                        if (e.Enh > -1)
                        {
                            // if (!hasProc && power.HasProc && ...) ??
                            if (!hasProc && (DatabaseAPI.Database.Enhancements[e.Enh].Probability) == 0 ||
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

                        fRecipe?.RecalcSalvage();
                    }
                }

                I9Picker.Visible = false;
                if (!gfxDrawing)
                    PowerModified(true);
                if (EnhancingPower > -1)
                    RefreshTabs(MidsContext.Character.CurrentBuild.Powers[EnhancingPower].NIDPower, e);
            }
            else
            {
                I9Picker.Visible = false;
                EnhancingSlot = -1;
                EnhancingPower = -1;
            }
        }

        private void I9Picker_Hiding(object sender, EventArgs e)
        {
            // 10 000 ticks in a millisecond / 10 000 000 ticks in a second (1.10^7)
            // Ensure the picker doesn't close instantly.
            if (!I9Picker.Visible | DateTime.Now.Ticks - popupLastOpenTime < 1e6)
                return;

            I9Picker.Visible = false;
            HidePopup();
            EnhancingSlot = -1;
            RefreshInfo();
        }

        private void I9Picker_HoverEnhancement(int e)
        {
            var i9Slot = new I9Slot
            {
                Enh = e,
                IOLevel = I9Picker.CheckAndReturnIoLevel() - 1,
                Grade = I9Picker.Ui.View.GradeId,
                RelativeLevel = I9Picker.Ui.View.RelLevel
            };
            myDataView.SetEnhancementPicker(i9Slot);
            ShowPopup(PickerHID, -1, -1, new Point(), I9Picker.Bounds, i9Slot);
        }

        private void I9Picker_HoverSet(int e)
        {
            myDataView.SetSetPicker(e);
            ShowPopup(PickerHID, -1, -1, new Point(), I9Picker.Bounds, null, e);
        }

        private void I9Picker_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || EnhancingSlot <= -1)
                return;
            I9Picker.Visible = false;
            EnhancingSlot = -1;
            RefreshInfo();
        }

        private void I9Picker_Moved(Rectangle NewBounds, Rectangle OldBounds)
        {
            MovePopup(I9Picker.Bounds);
            RedrawUnderPopup(OldBounds);
        }

        private void I9Popup_MouseMove(object sender, MouseEventArgs e)
        {
            HidePopup();
        }

        private void ibTeamEx_OnClick(object? sender, EventArgs e)
        {
            var iParent = this;
            var frmTeam = new FrmTeam(ref iParent);
            frmTeam.ShowDialog();
        }

        private void ibPopupEx_OnClick(object? sender, EventArgs e)
        {
            if (MidsContext.Config != null)
            {
                MidsContext.Config.DisableShowPopup = ibPopupEx.ToggleState switch
                {
                    ImageButtonEx.States.ToggledOff => true,
                    ImageButtonEx.States.ToggledOn => false,
                    _ => MidsContext.Config.DisableShowPopup
                };
            }
        }

        private void ibPvXEx_OnClick(object? sender, EventArgs e)
        {
            if (MidsContext.Config != null)
            {
                MidsContext.Config.Inc.DisablePvE = ibPvXEx.ToggleState switch
                {
                    ImageButtonEx.States.ToggledOff => false,
                    ImageButtonEx.States.ToggledOn => true,
                    _ => MidsContext.Config.Inc.DisablePvE
                };
            }
            RefreshInfo();
        }

        private void ibRecipeEx_OnClick(object? sender, EventArgs e)
        {
            if (MidsContext.Config != null)
            {
                MidsContext.Config.PopupRecipes = ibRecipeEx.ToggleState switch
                {
                    ImageButtonEx.States.ToggledOff => false,
                    ImageButtonEx.States.ToggledOn => true,
                    _ => MidsContext.Config.PopupRecipes
                };
            }
        }

        private void ibSetsEx_OnClick(object? sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon == null)
                return;
            FloatSets(true);
        }

        private void ibSlotLevelsEx_OnClick(object? sender, EventArgs e)
        {
            tsViewSlotLevels_Click(this, EventArgs.Empty);
        }

        private void ibTotalsEx_OnClick(object? sender, EventArgs e)
        {
            FloatTotals(true, MidsContext.Config is { UseOldTotalsWindow: true });
        }

        private void ibIncarnatesEx_OnClick(object? sender, EventArgs  e)
        {
            var flag = false;
            if (fIncarnate == null)
                flag = true;
            else if (fIncarnate.IsDisposed)
                flag = true;
            if (flag)
            {
                var iParent = this;
                fIncarnate = new frmIncarnate(ref iParent);
            }

            if (fIncarnate is { Visible: false })
            {
                ibIncarnatePowersEx.ToggleState = ImageButtonEx.States.ToggledOn;
                fIncarnate.Show(this);
            }
            else
            {
                ibIncarnatePowersEx.ToggleState = ImageButtonEx.States.ToggledOff;
                fIncarnate?.Close();
            }
        }

        private void IncarnateWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ibIncarnatesEx_OnClick(sender, EventArgs.Empty);
        }

        private void ibPrestigePowersEx_OnClick(object? sender, EventArgs e)
        {
            var flag = false;
            if (fPrestige == null)
                flag = true;
            else if (fPrestige.IsDisposed)
                flag = true;
            if (flag)
            {
                var iParent = this;
                var iPowers = DatabaseAPI.Database.Power.Where(power => power is { InherentType: Enums.eGridType.Prestige, PowerType: Enums.ePowerType.Toggle }).ToList();
                fPrestige = new frmPrestige(iParent, iPowers);
            }

            if (fPrestige is { Visible: false })
            {
                ibPrestigePowersEx.ToggleState = ImageButtonEx.States.ToggledOn;
                fPrestige.Show(this);
            }
            else
            {
                ibPrestigePowersEx.ToggleState = ImageButtonEx.States.ToggledOff;
                fPrestige?.Close();
            }
        }

        private void Info_Enhancement(I9Slot iEnh, int iLevel = -1)
        {
            myDataView.SetEnhancement(iEnh, iLevel);
        }

        internal void UnlockFloatingStats()
        {
            DataViewLocked = false;
            if (dvLastPower <= -1)
                return;
            Info_Power(dvLastPower, dvLastEnh, dvLastNoLev, DataViewLocked);
        }

        private void Info_Power(int powerIdx, int iEnhLvl = -1, bool NoLevel = false, bool Lock = false)
        {
            if (!Lock & DataViewLocked)
            {
                if (dvLastPower != powerIdx)
                    return;
                Lock = true;
            }

            dvLastEnh = iEnhLvl;
            dvLastPower = powerIdx;
            dvLastNoLev = NoLevel;
            fData?.UpdateData(dvLastPower);
            var powIndex = -1;
            if (MainModule.MidsController.Toon.Locked)
            {
                var num2 = MidsContext.Character.CurrentBuild.Powers.Count - 1;
                for (var index = 0; index <= num2; ++index)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[index] == null) continue;
                    if (MidsContext.Character.CurrentBuild.Powers[index].NIDPower != powerIdx)
                        continue;
                    powIndex = index;
                    break;
                }
            }

            DataViewLocked = Lock;
            if (powIndex > -1)
            {
                var basePower = MainModule.MidsController.Toon.GetBasePower(powIndex);
                var enhancedPower = MainModule.MidsController.Toon.GetEnhancedPower(powIndex);
                if (basePower != null && enhancedPower != null)
                {
                    myDataView.SetData(basePower, enhancedPower, NoLevel, DataViewLocked, powIndex);
                }
                else
                {
                    myDataView.SetData(MainModule.MidsController.Toon.GetBasePower(powIndex, powerIdx), null, NoLevel, DataViewLocked, powIndex);
                }
            }
            else
            {
                myDataView.SetData(MainModule.MidsController.Toon.GetBasePower(powIndex, powerIdx), null, NoLevel, DataViewLocked, powIndex);
            }
            
            if (!Lock || dvAnchored.Visible)
            {
                return;
            }

            FloatingDataForm.Activate();
        }

        private void info_Totals()
        {
            if ((MainModule.MidsController.Toon == null) | !MainModule.MidsController.IsAppInitialized)
                return;
            MainModule.MidsController.Toon.GenerateBuffedPowerArray();
            myDataView.DisplayTotals();
            FloatUpdate();
        }

        private void lblATLocked_MouseLeave(object sender, EventArgs e)
        {
            HidePopup();
        }

        private void lblATLocked_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || cbAT.SelectedIndex < 0)
                return;
            ShowPopup(-1, CbtAT.Value.SelectedItem.Idx, cbAT.Bounds, string.Empty);
        }

        private void lblATLocked_Paint(object sender, PaintEventArgs e)
        {
            if (MainModule.MidsController.Toon == null)
                return;
            var destRect = new RectangleF(1f, (lblATLocked.Height - 17) / 2f, 16f, 16f);
            destRect.Y += 1;
            destRect.X += 2;
            var srcRect = new RectangleF(MidsContext.Character.Archetype.Idx * 16, 0.0f, 16f, 16f);
            var graphics = e.Graphics;
            graphics.DrawImage(I9Gfx.Archetypes.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
            destRect.X = lblATLocked.Width - 19;
            graphics.DrawImage(I9Gfx.Archetypes.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
        }

        private void lblLocked0_MouseLeave(object sender, EventArgs e)
        {
            HidePopup();
        }

        private void lblLocked0_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[3] == null)
                return;
            var extraString =
                "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[3].nID, MidsContext.Character.Archetype.Idx, cbPool0.Bounds,
                extraString);
        }

        private void lblLocked0_Paint(object sender, PaintEventArgs e)
        {
            MiniPaint(ref e, Enums.PowersetType.Pool0);
        }

        private void lblLocked1_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[4] == null)
                return;
            var ExtraString =
                "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[4].nID, MidsContext.Character.Archetype.Idx, cbPool1.Bounds,
                ExtraString);
        }

        private void lblLocked1_Paint(object sender, PaintEventArgs e)
        {
            MiniPaint(ref e, Enums.PowersetType.Pool1);
        }

        private void lblLocked2_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[5] == null)
                return;
            var ExtraString =
                "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[5].nID, MidsContext.Character.Archetype.Idx, cbPool2.Bounds,
                ExtraString);
        }

        private void lblLocked2_Paint(object sender, PaintEventArgs e)
        {
            MiniPaint(ref e, Enums.PowersetType.Pool2);
        }

        private void lblLocked3_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[6] == null)
                return;
            var ExtraString =
                "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            // Bug: popup will turn into total garbage if the mouse pointer is within the popup drawing rectangle.
            // Add a vertical offset to ensure the mouse pointer stays out of the popup. 
            ShowPopup(MidsContext.Character.Powersets[6].nID, MidsContext.Character.Archetype.Idx, 
                new Rectangle(lblLocked3.Location.X, lblLocked3.Location.Y - 3 * lblLocked3.Height, cbPool3.Bounds.Width, cbPool3.Bounds.Height),
                //cbPool3.Bounds,
                ExtraString);
        }

        private void lblLocked3_Paint(object sender, PaintEventArgs e)
        {
            MiniPaint(ref e, Enums.PowersetType.Pool3);
        }

        private void lblLockedAncillary_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[7] == null)
                return;
            var ExtraString =
                "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[7].nID, MidsContext.Character.Archetype.Idx,
                CbtAncillary.Value.Bounds, ExtraString);
        }

        private void lblLockedAncillary_Paint(object sender, PaintEventArgs e)
        {
            MiniPaint(ref e, Enums.PowersetType.Ancillary);
        }

        private void lblLockedSecondary_MouseLeave(object sender, EventArgs e)
        {
            HidePopup();
        }

        private void lblLockedSecondary_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Archetype.Idx < 0 ||
                cbSecondary.SelectedIndex < 0)
                return;
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

        private void llAncillary_ItemClick(ListLabelV3.ListLabelItemV3 Item, MouseButtons Button)
        {
            if (Item.ItemState == ListLabelV3.LLItemState.Heading)
            {
                return;
            }

            switch (Button)
            {
                case MouseButtons.Left:
                    PowerPicked(Item.nIDSet, Item.nIDPower);
                    frmTotalsV2.SetTitle(fTotals2);
                    break;
                case MouseButtons.Right:
                    Info_Power(Item.nIDPower, -1, false, true);
                    break;
            }

            llALL_ItemClick_RefreshGfx();
        }

        private void llAncillary_ItemHover(ListLabelV3.ListLabelItemV3 Item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            var llBounds = new Rectangle(
                llAncillary.Bounds.X + poolsPanel.Bounds.X,
                llAncillary.Bounds.Y + poolsPanel.Bounds.Y,
                llAncillary.Bounds.Width,
                llAncillary.Bounds.Height);
            if (Item.ItemState == ListLabelV3.LLItemState.Heading)
            {
                ShowPopup(Item.nIDSet, -1, llBounds);
            }
            else
            {
                Info_Power(Item.nIDPower);
                ShowPopup(-1, Item.nIDPower, -1, new Point(), llBounds, null, -1, VerticalAlignment.Bottom);
            }
        }

        private void llPool0_ItemClick(ListLabelV3.ListLabelItemV3 Item, MouseButtons Button)
        {
            if (Button == MouseButtons.Left)
            {
                PowerPicked(Enums.PowersetType.Pool0, Item.nIDPower);
            }
            else
            {
                if (Button != MouseButtons.Right)
                    return;
                Info_Power(Item.nIDPower, -1, false, true);
            }

            llALL_ItemClick_RefreshGfx();
        }

        private void llPool0_ItemHover(ListLabelV3.ListLabelItemV3 Item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            Info_Power(Item.nIDPower);
            var llBounds = new Rectangle(
                llPool0.Bounds.X + poolsPanel.Bounds.X,
                llPool0.Bounds.Y + poolsPanel.Bounds.Y,
                llPool0.Bounds.Width,
                llPool0.Bounds.Height);
            ShowPopup(-1, Item.nIDPower, -1, new Point(), llBounds);
        }

        private void llPool1_ItemClick(ListLabelV3.ListLabelItemV3 Item, MouseButtons Button)
        {
            if (Button == MouseButtons.Left)
            {
                PowerPicked(Enums.PowersetType.Pool1, Item.nIDPower);
            }
            else
            {
                if (Button != MouseButtons.Right)
                    return;
                Info_Power(Item.nIDPower, -1, false, true);
            }

            llALL_ItemClick_RefreshGfx();
        }

        private void llPool1_ItemHover(ListLabelV3.ListLabelItemV3 Item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            Info_Power(Item.nIDPower);
            var llBounds = new Rectangle(
                llPool1.Bounds.X + poolsPanel.Bounds.X,
                llPool1.Bounds.Y + poolsPanel.Bounds.Y,
                llPool1.Bounds.Width,
                llPool1.Bounds.Height);
            ShowPopup(-1, Item.nIDPower, -1, new Point(), llBounds);
        }

        private void llPool2_ItemClick(ListLabelV3.ListLabelItemV3 Item, MouseButtons Button)
        {
            if (Button == MouseButtons.Left)
            {
                PowerPicked(Enums.PowersetType.Pool2, Item.nIDPower);
            }
            else
            {
                if (Button != MouseButtons.Right)
                    return;
                Info_Power(Item.nIDPower, -1, false, true);
            }

            llALL_ItemClick_RefreshGfx();
        }

        private void llPool2_ItemHover(ListLabelV3.ListLabelItemV3 Item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            Info_Power(Item.nIDPower);
            var llBounds = new Rectangle(
                llPool2.Bounds.X + poolsPanel.Bounds.X,
                llPool2.Bounds.Y + poolsPanel.Bounds.Y,
                llPool2.Bounds.Width,
                llPool2.Bounds.Height);
            ShowPopup(-1, Item.nIDPower, -1, new Point(), llBounds);
        }

        private void llPool3_ItemClick(ListLabelV3.ListLabelItemV3 Item, MouseButtons Button)
        {
            if (Button == MouseButtons.Left)
            {
                PowerPicked(Enums.PowersetType.Pool3, Item.nIDPower);
            }
            else
            {
                if (Button != MouseButtons.Right)
                    return;
                Info_Power(Item.nIDPower, -1, false, true);
            }

            llALL_ItemClick_RefreshGfx();
        }

        private void llPool3_ItemHover(ListLabelV3.ListLabelItemV3 Item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            Info_Power(Item.nIDPower);
            var llBounds = new Rectangle(
                llPool3.Bounds.X + poolsPanel.Bounds.X,
                llPool3.Bounds.Y + poolsPanel.Bounds.Y,
                llPool3.Bounds.Width,
                llPool3.Bounds.Height);
            ShowPopup(-1, Item.nIDPower, -1, new Point(), llBounds);
        }

        private void llPrimary_ItemClick(ListLabelV3.ListLabelItemV3 Item, MouseButtons Button)
        {
            if (Item.ItemState == ListLabelV3.LLItemState.Heading)
                return;
            switch (Button)
            {
                case MouseButtons.Left:
                    PowerPicked(Item.nIDSet, Item.nIDPower);
                    break;
                case MouseButtons.Right:
                    Info_Power(Item.nIDPower, -1, false, true);
                    break;
            }

            llALL_ItemClick_RefreshGfx();
        }

        private void llPrimary_ItemHover(ListLabelV3.ListLabelItemV3 Item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            if (Item.ItemState == ListLabelV3.LLItemState.Heading)
            {
                ShowPopup(Item.nIDSet, -1, llPrimary.Bounds, string.Empty);
            }
            else
            {
                Info_Power(Item.nIDPower);
                ShowPopup(-1, Item.nIDPower, -1, new Point(), llPrimary.Bounds);
            }
        }

        private void llSecondary_ItemClick(ListLabelV3.ListLabelItemV3 Item, MouseButtons Button)
        {
            if (Item.ItemState == ListLabelV3.LLItemState.Heading)
                return;
            switch (Button)
            {
                case MouseButtons.Left:
                    PowerPicked(Item.nIDSet, Item.nIDPower);
                    break;
                case MouseButtons.Right:
                    Info_Power(Item.nIDPower, -1, false, true);
                    break;
            }

            llALL_ItemClick_RefreshGfx();
        }

        private void llSecondary_ItemHover(ListLabelV3.ListLabelItemV3 Item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            if (Item.ItemState == ListLabelV3.LLItemState.Heading)
            {
                ShowPopup(Item.nIDSet, -1, llSecondary.Bounds, string.Empty);
            }
            else
            {
                Info_Power(Item.nIDPower);
                ShowPopup(-1, Item.nIDPower, -1, new Point(), llSecondary.Bounds);
            }
        }

        private void MiniPaint(ref PaintEventArgs e, Enums.PowersetType iId)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[(int)iId] == null)
                return;
            var destRect = new RectangleF(1f, (lblLocked0.Height - 16) / 2f, 16f, 16f);
            --destRect.Y;
            var srcRect = new RectangleF(MidsContext.Character.Powersets[(int)iId].nID * 16, 0.0f, 16f, 16f);
            var graphics = e.Graphics;
            graphics.DrawImage(I9Gfx.Powersets.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
            destRect.X = lblLocked0.Width - 19;
            graphics.DrawImage(I9Gfx.Powersets.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
        }

        private void MovePopup(Rectangle rBounds)
        {
            if (!PopUpVisible)
                return;
            var bounds = I9Popup.Bounds;
            if (rBounds == bounds)
                return;
            SetPopupLocation(rBounds, false, true);
            RedrawUnderPopup(bounds);
        }

        private void NewDraw(bool skipDraw = false)
        {
            if (drawing == null)
            {
                drawing = new clsDrawX(pnlGFX);
            }
            else
            {
                drawing.ReInit(pnlGFX);
            }

            pnlGFX.Image = drawing.bxBuffer.Bitmap;
            if (drawing != null)
                drawing.Highlight = -1;
            if (skipDraw)
                return;
            DoRedraw();
        }

        private void NewToon(bool init = true, bool skipDraw = false)
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
                var str = !MainModule.MidsController.Toon.Locked ? MidsContext.Character.Name : string.Empty;
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

            switch (MidsContext.Character?.Archetype?.DisplayName)
            {
                case "Mastermind":
                    ibPetsEx.Visible = true;
                    ibPetsEx.Enabled = true;
                    break;
                default:
                    ibPetsEx.Visible = false;
                    ibPetsEx.Enabled = false;
                    break;
            }

            NewDraw(skipDraw);
            UpdateControls(true);
            SetTitleBar(MidsContext.Character != null && MidsContext.Character.IsHero());
            UpdateColors();
            MidsContext.EnhCheckMode = false;
            UpdateEnhCheckModeToolStrip();
            enhCheckMode.Hide();
            info_Totals();
            FileModified = false;
            DoRedraw();
        }

        private void EnemyRelativeLevel_Changed(object? sender, EventArgs e)
        {
            if (MidsContext.Config == null) return;
            if (EnemyRelativeToolStripComboBox.ComboBox == null) return;
            MidsContext.Config.ScalingToHit = (float)EnemyRelativeToolStripComboBox.ComboBox.SelectedValue;
            RefreshInfo();
        }

        private void ibDynMode_Click(object? sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Config == null) return;
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

        // private void pbDynMode_Paint(object sender, PaintEventArgs e)
        // {
        //     if (dmBuffer == null)
        //         UpdateDmBuffer();
        //     if (dmBuffer == null)
        //         return;
        //     e.Graphics.DrawImage(dmBuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        // }

        private void pnlGFX_DragDrop(object sender, DragEventArgs e)
        {
            if (!sender.Equals(pnlGFX))
                return;
            pnlGFX.AllowDrop = false;
            ControlPaint.DrawReversibleFrame(dragRect, Color.White, FrameStyle.Thick);
            oldDragRect = Rectangle.Empty;
            dragRect = Rectangle.Empty;
            var iValue1 = e.X + xCursorOffset;
            var iValue2 = e.Y + yCursorOffset;
            dragFinishPower = drawing.WhichSlot(drawing.ScaleUp(iValue1), drawing.ScaleUp(iValue2));
            if (dragStartSlot != -1)
            {
                dragFinishSlot = drawing.WhichEnh(drawing.ScaleUp(iValue1), drawing.ScaleUp(iValue2));
                if (dragFinishSlot == 0)
                    MessageBox.Show(this, "You cannot change the level of any power's automatic slot.", null,
                        MessageBoxButtons.OK);
                else
                    SlotLevelSwap(dragStartPower, dragStartSlot, dragFinishPower, dragFinishSlot);
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
            e.Effect = sender.Equals(pnlGFX) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void pnlGFX_DragOver(object sender, DragEventArgs e)
        {
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
                return;
            if (dragStartSlot != -1)
            {
                position = Cursor.Position;
                var x = position.X - dragXOffset;
                position = Cursor.Position;
                var y = position.Y - dragYOffset;
                var width = drawing.ScaleDown(drawing.szSlot.Width);
                var height = drawing.ScaleDown(drawing.szSlot.Height);
                dragRect = new Rectangle(x, y, width, height);
            }
            else
            {
                position = Cursor.Position;
                var x = position.X - dragXOffset;
                position = Cursor.Position;
                var y = position.Y - dragYOffset;
                var width = drawing.ScaleDown(drawing.SzPower.Width);
                var height = drawing.ScaleDown(drawing.SzPower.Height);
                dragRect = new Rectangle(x, y, width, height);
            }

            if (!oldDragRect.IsEmpty)
                ControlPaint.DrawReversibleFrame(oldDragRect, Color.White, FrameStyle.Thick);
            if (ClientRectangle.Contains(RectangleToClient(dragRect)))
                oldDragRect = dragRect;
            else
                dragRect = oldDragRect;
            ControlPaint.DrawReversibleFrame(dragRect, Color.White, FrameStyle.Thick);
        }

        private void pnlGFX_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Lock double click usage when enhancement check mode is active
            // Disable the ability to double click an enhancement slot to remove it
            if (MidsContext.EnhCheckMode) return;
            if (!(!LastClickPlacedSlot && dragStartSlot >= 0)) return;
            MainModule.MidsController.Toon.BuildSlot(dragStartPower, dragStartSlot);
            var powerEntryArray = DeepCopyPowerList();
            RearrangeAllSlotsInBuild(powerEntryArray, true);
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
            if (MidsContext.EnhCheckMode) return;
            if (e.Button != MouseButtons.Left)
                return;
            pnlGFX.AllowDrop = true;
            dragStartX = e.X;
            dragStartY = e.Y;
            dragStartPower = drawing.WhichSlot(drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y));
            dragStartSlot = drawing.WhichEnh(drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y));
        }

        private void pnlGFX_MouseEnter(object sender, EventArgs e)
        {
            pnlGFXFlow.Focus();
        }

        private void pnlGFX_MouseLeave(object sender, EventArgs e)
        {
            HidePopup();
            drawing?.HighlightSlot(-1);
        }

        private void pnlGFX_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Left) & pnlGFX.AllowDrop && Math.Abs(e.X - dragStartX) + Math.Abs(e.Y - dragStartY) > 7)
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
                            dragXOffset = drawing.ScaleDown(drawing.szSlot.Width / 2);
                            dragYOffset = drawing.ScaleDown(drawing.szSlot.Height / 2);
                        }
                    }
                    else
                    {
                        if (drawing != null)
                        {
                            dragXOffset = drawing.ScaleDown(drawing.SzPower.Width / 2);
                            dragYOffset = drawing.ScaleDown(drawing.SzPower.Height / 2);
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
                if (drawing == null) return;
                var index = drawing.WhichSlot(drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y));
                var sIDX = drawing.WhichEnh(drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y));
                if ((index < 0) | (index >= MidsContext.Character.CurrentBuild.Powers.Count))
                {
                    HidePopup();
                }
                else
                {
                    var e1 = new Point(e.X + 10, e.Y + 10);
                    ShowPopup(index, -1, sIDX, e1, new Rectangle());
                    if (MidsContext.Character.CanPlaceSlot & (MainModule.MidsController.Toon.SlotCheck(MidsContext.Character.CurrentBuild.Powers[index]) > -1))
                    {
                        drawing.HighlightSlot(index);
                        if ((index > -1) & (drawing.InterfaceMode != Enums.eInterfaceMode.PowerToggle))
                            pnlGFX.Cursor = Cursors.Hand;
                        else
                            pnlGFX.Cursor = Cursors.Default;
                    }
                    else
                    {
                        pnlGFX.Cursor = Cursors.Default;
                        drawing.HighlightSlot(-1);
                    }

                    if (index <= -1 || !((index != LastIndex) | (LastEnhIndex != sIDX)))
                        return;
                    LastIndex = index;
                    LastEnhIndex = sIDX;
                    if (sIDX > -1)
                        RefreshTabs(MidsContext.Character.CurrentBuild.Powers[index].NIDPower,
                            MidsContext.Character.CurrentBuild.Powers[index].Slots[sIDX].Enhancement,
                            MidsContext.Character.CurrentBuild.Powers[index].Slots[sIDX].Level);
                    else
                        RefreshTabs(MidsContext.Character.CurrentBuild.Powers[index].NIDPower, new I9Slot());
                }
            }
        }

        private void RedrawSinglePower(ref PowerEntry? powerEntry, bool singleDraw = false, bool refreshInfo = false)
        {
            drawing.DrawPowerSlot(ref powerEntry, singleDraw);
            pnlGFX.Refresh();
            if (refreshInfo) RefreshInfo();
        }

        private void pnlGFX_MouseUp(object sender, MouseEventArgs e)
        {
            pnlGFX.AllowDrop = false;
            if (DoneDblClick)
            {
                DoneDblClick = false;
            }
            else
            {
                var hIDPower = drawing.WhichSlot(drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y));
                var slotID = drawing.WhichEnh(drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y));
                if ((hIDPower < 0) | (hIDPower >= MidsContext.Character.CurrentBuild.Powers.Count))
                    return;
                var flag = MidsContext.Character.CurrentBuild.Powers[hIDPower].NIDPower < 0;
                if ((e.Button == MouseButtons.Left) & (ModifierKeys == (Keys.Shift | Keys.Control)) && EditAccoladesOrTemps(hIDPower))
                    return;

                if (MidsContext.EnhCheckMode)
                {
                    if (!((e.Button == MouseButtons.Left) & (slotID > -1))) return;

                    MidsContext.Character.CurrentBuild.Powers[hIDPower].Slots[slotID].Enhancement.Obtained = !MidsContext.Character.CurrentBuild.Powers[hIDPower].Slots[slotID].Enhancement.Obtained;
                    if (fRecipe != null && fRecipe.Visible)
                    {
                        //fRecipe.RecalcSalvage();
                        fRecipe.UpdateEnhObtained();
                    }

                    if (enhCheckMode.Visible)
                    {
                        enhCheckMode.UpdateEnhObtained();
                    }

                    var powerEntry = MidsContext.Character.CurrentBuild.Powers[hIDPower];
                    RedrawSinglePower(ref powerEntry, true);
                }
                else
                {
                    if ((drawing.InterfaceMode == Enums.eInterfaceMode.PowerToggle) & (e.Button == MouseButtons.Left))
                    {
                        if (!flag && MidsContext.Character.CurrentBuild.Powers[hIDPower].CanIncludeForStats())
                        {
                            if (MidsContext.Character.CurrentBuild.Powers[hIDPower].StatInclude)
                            {
                                MidsContext.Character.CurrentBuild.Powers[hIDPower].StatInclude = false;
                            }
                            else
                            {
                                var eMutex = MainModule.MidsController.Toon.CurrentBuild.MutexV2(hIDPower);
                                if ((eMutex == Enums.eMutex.NoConflict) | (eMutex == Enums.eMutex.NoGroup))
                                    MidsContext.Character.CurrentBuild.Powers[hIDPower].StatInclude = true;
                            }
                        }

                        else if (!flag && MidsContext.Character.CurrentBuild.Powers[hIDPower].HasProc())
                        {
                            if (MidsContext.Character.CurrentBuild.Powers[hIDPower].ProcInclude)
                            {
                                MidsContext.Character.CurrentBuild.Powers[hIDPower].ProcInclude = false;
                            }
                            else
                            {
                                MidsContext.Character.CurrentBuild.Powers[hIDPower].ProcInclude = true;
                            }
                        }

                        EnhancementModified();
                        LastClickPlacedSlot = false;
                        pnlGFX.Update();
                        pnlGFX.Refresh();
                    }
                    else if (ToggleClicked(hIDPower, drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y)) & (e.Button == MouseButtons.Left))
                    {
                        if (!flag && MidsContext.Character.CurrentBuild.Powers[hIDPower].CanIncludeForStats() &&
                            !MidsContext.Character.CurrentBuild.Powers[hIDPower].HasProc())
                        {
                            if (MidsContext.Character.CurrentBuild.Powers[hIDPower].StatInclude)
                            {
                                MidsContext.Character.CurrentBuild.Powers[hIDPower].StatInclude = false;
                                MidsContext.Character.CurrentBuild.Powers[hIDPower].Power.Active = false;
                            }
                            else
                            {
                                var eMutex = MainModule.MidsController.Toon.CurrentBuild.MutexV2(hIDPower);
                                if ((eMutex == Enums.eMutex.NoConflict) | (eMutex == Enums.eMutex.NoGroup))
                                {
                                    MidsContext.Character.CurrentBuild.Powers[hIDPower].StatInclude = true;
                                    MidsContext.Character.CurrentBuild.Powers[hIDPower].Power.Active = true;
                                }
                            }

                            MidsContext.Character.Validate();
                        }
                        else if (!flag && MidsContext.Character.CurrentBuild.Powers[hIDPower].HasProc() && !MidsContext.Character.CurrentBuild.Powers[hIDPower].CanIncludeForStats())
                        {
                            if (MidsContext.Character.CurrentBuild.Powers[hIDPower].ProcInclude)
                            {
                                MidsContext.Character.CurrentBuild.Powers[hIDPower].ProcInclude = false;
                            }
                            else
                            {
                                MidsContext.Character.CurrentBuild.Powers[hIDPower].ProcInclude = true;
                            }
                        }
                        else if (!flag && MidsContext.Character.CurrentBuild.Powers[hIDPower].CanIncludeForStats() && MidsContext.Character.CurrentBuild.Powers[hIDPower].HasProc())
                        {
                            if (MidsContext.Character.CurrentBuild.Powers[hIDPower].StatInclude)
                            {
                                MidsContext.Character.CurrentBuild.Powers[hIDPower].StatInclude = false;
                                MidsContext.Character.CurrentBuild.Powers[hIDPower].Power.Active = false;
                            }
                            else
                            {
                                var eMutex = MainModule.MidsController.Toon.CurrentBuild.MutexV2(hIDPower);
                                if ((eMutex == Enums.eMutex.NoConflict) | (eMutex == Enums.eMutex.NoGroup))
                                {
                                    MidsContext.Character.CurrentBuild.Powers[hIDPower].StatInclude = true;
                                    MidsContext.Character.CurrentBuild.Powers[hIDPower].Power.Active = true;
                                }
                            }

                            MidsContext.Character.Validate();
                        }

                        EnhancementModified();
                        LastClickPlacedSlot = false;
                    }
                    else if (ProcToggleClicked(hIDPower, drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y)) & (e.Button == MouseButtons.Left))
                    {
                        var powerEntry = MidsContext.Character.CurrentBuild.Powers[hIDPower];
                        if (!flag && powerEntry.CanIncludeForStats() && powerEntry.HasProc())
                        {
                            powerEntry.ProcInclude = !powerEntry.ProcInclude;
                        }

                        //EnhancementModified();
                        RedrawSinglePower(ref powerEntry, true, true);
                        LastClickPlacedSlot = false;
                    }
                    else if ((e.Button == MouseButtons.Left) & (ModifierKeys == Keys.Alt))
                    {
                        MainModule.MidsController.Toon.BuildPower(
                            MidsContext.Character.CurrentBuild.Powers[hIDPower].NIDPowerset,
                            MidsContext.Character.CurrentBuild.Powers[hIDPower].NIDPower);
                        PowerModified(true);
                        LastClickPlacedSlot = false;
                    }
                    else if ((e.Button == MouseButtons.Left) & (ModifierKeys == Keys.Shift) & (slotID > -1))
                    {
                        if (MidsContext.Config.BuildMode == Enums.dmModes.LevelUp)
                        {
                            MainModule.MidsController.Toon.RequestedLevel = MidsContext.Character.CurrentBuild.Powers[hIDPower].Slots[slotID].Level;
                            MidsContext.Character.ResetLevel();
                        }
                        MainModule.MidsController.Toon.BuildSlot(hIDPower, slotID);
                        PowerModified(true);
                        LastClickPlacedSlot = false;
                    }
                    else
                    {
                        if ((e.Button == MouseButtons.Left) & !EnhPickerActive)
                        {
                            if ((MidsContext.Config.BuildMode == Enums.dmModes.Normal) & flag)
                            {
                                if (MidsContext.Character.CurrentBuild.Powers[hIDPower].Level > -1)
                                {
                                    MainModule.MidsController.Toon.RequestedLevel =
                                        MidsContext.Character.CurrentBuild.Powers[hIDPower].Level;
                                    UpdatePowerLists();
                                    DoRedraw();
                                    return;
                                }
                            }
                            else if ((MidsContext.Config.BuildMode == Enums.dmModes.Respec) & flag)
                            {
                                if (MidsContext.Character.CurrentBuild.Powers[hIDPower].Level > -1)
                                {
                                    MainModule.MidsController.Toon.RequestedLevel =
                                        MidsContext.Character.CurrentBuild.Powers[hIDPower].Level;
                                    UpdatePowerLists();
                                    DoRedraw();
                                    return;
                                }
                            }
                            else
                            {
                                if (MainModule.MidsController.Toon.BuildSlot(hIDPower) > -1)
                                {
                                    // adding a slot by itself doesn't really change the build substantially without an enh going into it
                                    // var powerEntryArray = DeepCopyPowerList();
                                    // RearrangeAllSlotsInBuild(powerEntryArray, true);
                                    // ShallowCopyPowerList(powerEntryArray);
                                    // PowerModified(false);
                                    // DoRedraw();
                                    PowerModified(false);
                                    LastClickPlacedSlot = true;
                                    //MidsContext.Config.Tips.Show(Tips.TipType.FirstEnh);
                                    return;
                                }

                                LastClickPlacedSlot = false;
                            }
                        }

                        if ((e.Button == MouseButtons.Middle) & (slotID > -1) &
                            !MidsContext.Config.DisableRepeatOnMiddleClick)
                        {
                            EnhancingSlot = slotID;
                            EnhancingPower = hIDPower;
                            gfxDrawing = true;
                            I9Picker_EnhancementPicked(GetRepeatEnhancement(hIDPower, slotID));
                            gfxDrawing = false;
                            EnhancementModified();
                            
                            drawing.Refresh(new Rectangle(0, 0, pnlGFX.Width, pnlGFX.Height));
                        }
                        else if ((e.Button == MouseButtons.Right) & (slotID > -1) && ModifierKeys != Keys.Shift)
                        {
                            EnhancingSlot = slotID;
                            EnhancingPower = hIDPower;
                            var enhancements = MainModule.MidsController.Toon.GetEnhancements(hIDPower);
                            PickerHID = hIDPower;
                            if (!flag)
                                I9Picker.SetData(MidsContext.Character.CurrentBuild.Powers[hIDPower].NIDPower,
                                    ref MidsContext.Character.CurrentBuild.Powers[hIDPower].Slots[slotID].Enhancement,
                                    ref drawing, enhancements);
                            else
                                I9Picker.SetData(-1,
                                    ref MidsContext.Character.CurrentBuild.Powers[hIDPower].Slots[slotID].Enhancement,
                                    ref drawing, enhancements);


                            var point = new Point(
                                (int) Math.Round(pnlGFXFlow.Left - pnlGFXFlow.HorizontalScroll.Value + e.X -
                                                 I9Picker.Width / 2.0),
                                (int) Math.Round(pnlGFXFlow.Top - pnlGFXFlow.VerticalScroll.Value + e.Y -
                                                 I9Picker.Height / 2.0));
                            if (point.Y < MenuBar.Height)
                                point.Y = MenuBar.Height;
                            Size clientSize;
                            if (point.Y + I9Picker.Height > ClientSize.Height)
                            {
                                ref var local = ref point;
                                clientSize = ClientSize;
                                local.Y = clientSize.Height - I9Picker.Height;
                            }

                            clientSize = ClientSize;
                            if (point.X + I9Picker.Width > clientSize.Width)
                            {
                                ref var local = ref point;
                                clientSize = ClientSize;
                                var num2 = clientSize.Width - I9Picker.Width;
                                local.X = num2;
                            }

                            I9Picker.Location = point;
                            I9Picker.BringToFront();
                            popupLastOpenTime = DateTime.Now.Ticks;
                            I9Picker.Visible = true;
                            I9Picker.Select();
                            LastClickPlacedSlot = false;
                        }
                        else if ((e.Button == MouseButtons.Right) & (ModifierKeys == Keys.Shift))
                        {
                            //MidsContext.Character.PEnhancementsList.Clear();
                            StartFlip(hIDPower);
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            Info_Power(MidsContext.Character.CurrentBuild.Powers[hIDPower].NIDPower, -1, true, true);
                            LastClickPlacedSlot = false;
                        }
                    }
                }
            }
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
                        ibSlotInfoEx.ToggleState = ImageButtonEx.States.ToggledOff;
                        break;
                    case < 0:
                        ibSlotInfoEx.ToggleText.ToggledOff = slotCounts[0] switch
                        {
                            < 2 => $"{Math.Abs(slotCounts[0])} slot over",
                            > 1 => $"{Math.Abs(slotCounts[0])} slots over"
                        };
                        ibSlotInfoEx.ToggleState = ImageButtonEx.States.ToggledOff;
                        MessageBox.Show($"This build exceeds the slot limit.\r\nPlease remove {Math.Abs(slotCounts[0])} slots from the build.", @"Invalid Slotting", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    default:
                        ibSlotInfoEx.ToggleText.ToggledOff = slotCounts[0] switch
                        {
                            < 2 => $"{slotCounts[0]} slot to go",
                            > 1 => $"{slotCounts[0]} slots to go"
                        };
                        ibSlotInfoEx.ToggleState = ImageButtonEx.States.ToggledOff;
                        break;
                }

                ibSlotInfoEx.ToggleText.ToggledOn = slotCounts[1] switch
                {
                    <= 0 => "No slots placed",
                    < 2 => $"{slotCounts[1]} slot placed",
                    > 1 => $"{slotCounts[1]} slots placed"
                };
                ibSlotInfoEx.ToggleState = ImageButtonEx.States.ToggledOn;
            }

            if (MidsContext.Character != null && (index > -1) & (index <= MidsContext.Character.CurrentBuild.Powers.Count))
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

        private bool? CheckInitDdsaValue(int index, int? defaultOpt, string descript, params string[] options)
        {
            if (dragdropScenarioAction[index] != 0) return null;
            var (result, remember) = frmOptionListDlg.ShowWithOptions(true, defaultOpt ?? 1, descript, options);
            dragdropScenarioAction[index] = (short)result;
            if (remember != true) return remember;
            if (MidsContext.Config != null)
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
                        if (dragdropScenarioAction[0] == 2)
                            dragdropScenarioAction[0] = 3;

                    if (remember == true)
                        MidsContext.Config.DragDropScenarioAction[0] = dragdropScenarioAction[0];
                }

                if (dragdropScenarioAction[0] == 1)
                    return 0;
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
                            return PowerMove(tp, start, lvl);
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
                    return 0;
                if (dragdropScenarioAction[1] == 2)
                {
                    var num1 = start + 1;
                    int index;
                    for (index = finish; index >= num1; index += -1)
                    {
                        if (!flagArray[index])
                            continue;
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
            clearPower(tp, start);
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
                        return 0;
                    if (dragdropScenarioAction[7] == 3)
                    {
                        if (!flag2) start = index;

                        break;
                    }
                }

                if (!(!flag2 & (tp[index].NIDPower < 0)))
                    continue;
                CheckInitDdsaValue(10, null, "There is a gap in a group of powers that are being shifted",
                    "Fill empty slot; don't move powers unnecessarily", "Shift empty slot as if it were a power");
                if (dragdropScenarioAction[10] == 1)
                    return 0;
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
                return;
            var index = 0;
            do
            {
                dragdropScenarioAction[index] = MidsContext.Config.DragDropScenarioAction[index];
                ++index;
            } while (index <= 19);

            var powerEntryArray = DeepCopyPowerList();
            if (PowerMove(powerEntryArray, dragStart, dragFinish) == 0)
                return;
            ShallowCopyPowerList(powerEntryArray);
            PowerModified(true);
            DoRedraw();
        }

        private void PowerPicked(Enums.PowersetType SetID, int nIDPower)
        {
            MainModule.MidsController.Toon.BuildPower(MidsContext.Character.Powersets[(int)SetID].nID, nIDPower);
            PowerModified(true);
            //MidsContext.Config.Tips.Show(Tips.TipType.FirstPower);
        }

        private void PowerPicked(int nIDPowerset, int nIDPower)
        {
            MainModule.MidsController.Toon.BuildPower(nIDPowerset, nIDPower);
            PowerModified(true);
            //MidsContext.Config.Tips.Show(Tips.TipType.FirstPower);
            DoRedraw();
        }

        private void PowerPickedNoRedraw(int nIDPowerset, int nIDPower)
        {
            MainModule.MidsController.Toon.BuildPower(nIDPowerset, nIDPower, true);
            // Zed: Important: if using PowerModified() the rendering will be super slow!
            //PowerModified(markModified: true);
        }

        private int PowerSwap(int mode, ref PowerEntry?[] tp, int start, int finish)
        {
            int num1;
            if (start < 0 || start > 23 || finish < 0 || finish > 23 || start == finish)
                return 0;

            if (tp[start].NIDPower == -1 ||
                DatabaseAPI.Database.Power[tp[start].NIDPower].Level - 1 <= tp[finish].Level)
            {
                if (tp[finish].NIDPower != -1 &&
                    DatabaseAPI.Database.Power[tp[finish].NIDPower].Level - 1 > tp[start].Level)
                    switch (mode)
                    {
                        case 0:
                            CheckInitDdsaValue(4, null, "Power being replaced is swapped too low",
                                "Overwrite rather than swap",
                                "Allow power to be swapped anyway (mark as invalid)");
                            if (dragdropScenarioAction[4] == 1) return 0;
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
                                return 1;
                            break;
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
                                MidsContext.Config.DragDropScenarioAction[0] = dragdropScenarioAction[0];
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
                    return 0;

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
                            return PowerSwap(mode, ref tp, start, lvl);
                    }

                    var num4 = index;
                    return PowerSwap(mode, ref tp, start, num4);
                }
            }

            if (mode == 1 || mode == 2 && tp[finish].NIDPower != -1 &&
                DatabaseAPI.Database.Power[tp[finish].NIDPower].Level - 1 == tp[finish].Level)
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
                                return 0;
                            if (dragdropScenarioAction[12] == 2)
                            {
                                dragdropScenarioAction[11] = 2;
                                return 2;
                            }

                            if (dragdropScenarioAction[12] != 3 && dragdropScenarioAction[12] == 4)
                                return 3;
                            break;
                        }
                    case 2:
                        {
                            CheckInitDdsaValue(11, null, "A power placed at its minimum level is being shifted up",
                                "Shift it along with the other powers", "Shift other powers around it");
                            if (dragdropScenarioAction[11] == 1)
                                return 0;
                            if (dragdropScenarioAction[11] != 2 && dragdropScenarioAction[11] == 3)
                                return 1;
                            break;
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
                    CheckInitDdsaValue(6, null, "Power being replaced is swapped too high to have # slots",
                        "Remove impossible slots",
                        "Allow anyway (Mark slots as invalid)");
                num6 = 6;
            }
            else if ((start < 22) & (finish == 22) & (num5 <= 8) & (tp[start].SlotCount + tp[23].SlotCount > 8) ||
                     (start < 22) & (finish == 23) & (tp[finish].SlotCount <= 4) & (tp[start].SlotCount > 4) ||
                     (start < 22) & (finish == 23) & (num5 <= 8) & (tp[22].SlotCount + tp[start].SlotCount > 8) ||
                     (start == 22) & (finish == 23) & (tp[start].SlotCount > 4))
            {
                if (mode < 2)
                    CheckInitDdsaValue(3, null, "Power is moved or swapped too high to have # slots",
                        "Remove impossible slots",
                        "Allow anyway (Mark slots as invalid)");
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
                                tp[index1].Slots[index2].Level = 50;
                    }
                    else
                    {
                        for (var index2 = tp[index1].SlotCount - 1; index2 >= 1; index2 += -1)
                            if (index2 + tp[22].SlotCount > 7)
                                tp[index1].Slots[index2].Level = 50;
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
                                        CheckInitDdsaValue(2, 3, "Power is moved or swapped higher than slots' levels",
                                            "Remove slots",
                                            "Mark invalid slots", "Swap slot levels if valid; remove invalid ones",
                                            "Swap slot levels if valid; mark invalid ones",
                                            "Rearrange all slots in build");
                                    else if ((mode == 0) & (index3 == 1) & (dragdropScenarioAction[5] == 0))
                                        CheckInitDdsaValue(5, 3,
                                            "Power being replaced is swapped higher than slots' levels", "Remove slots",
                                            "Mark invalid slots", "Swap slot levels if valid; remove invalid ones",
                                            "Swap slot levels if valid; mark invalid ones",
                                            "Rearrange all slots in build");
                                    else if ((mode == 2) & (dragdropScenarioAction[8] == 0))
                                        CheckInitDdsaValue(8, 3, "Power being shifted up has slots from lower levels",
                                            "Remove slots",
                                            "Mark invalid slots", "Swap slot levels if valid; remove invalid ones",
                                            "Swap slot levels if valid; mark invalid ones",
                                            "Rearrange all slots in build");

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
                return;
            ShallowCopyPowerList(tp);
            PowerModified(true);
            DoRedraw();
        }

        private void PriSec_ExpandChanged(bool Expanded)
        {
            if (llPrimary.isExpanded | (llSecondary.isExpanded & dvAnchored.IsDocked & !HasSentForwards))
            {
                llPrimary.BringToFront();
                llSecondary.BringToFront();
                HasSentBack = false;
                HasSentForwards = true;
            }
            else
            {
                if (!(llPrimary.Bounds.IntersectsWith(dvAnchored.Bounds) & !HasSentBack))
                    return;
                llPrimary.SendToBack();
                llSecondary.SendToBack();
                HasSentBack = true;
                HasSentForwards = false;
            }
        }

        private Rectangle raGetPoolRect(int Index)
        {
            Label label;
            ListLabelV3 ll;
            switch (Index)
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

        private int raGetTop()
        {
            return MainModule.MidsController.Toon != null
                ? 4 + llPrimary.Top + raGreater(llPrimary.Height, llSecondary.Height)
                : llPrimary.Top + llPrimary.Height;
        }

        private int raGreater(int iVal1, int iVal2)
        {
            return iVal1 <= iVal2 ? iVal2 : iVal1;
        }

        private void raMovePool(int Index, int X, int Y)
        {
            Label label1;
            ComboBox comboBox;
            Label label2;
            ListLabelV3 ll;
            switch (Index)
            {
                case 0:
                    label1 = lblPool1;
                    comboBox = cbPool0;
                    label2 = lblLocked0;
                    ll = llPool0;
                    break;
                case 1:
                    label1 = lblPool2;
                    comboBox = cbPool1;
                    label2 = lblLocked1;
                    ll = llPool1;
                    break;
                case 2:
                    label1 = lblPool3;
                    comboBox = cbPool2;
                    label2 = lblLocked2;
                    ll = llPool2;
                    break;
                case 3:
                    label1 = lblPool4;
                    comboBox = cbPool3;
                    label2 = lblLocked3;
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

            label1.Location = new Point(X, Y);

            var point = new Point(label1.Location.X, label1.Location.Y);
            point.Y += label1.Height;
            comboBox.Location = point;
            label2.Location = point;
            point.Y += comboBox.Height;
            ll.Location = point;
        }

        private bool raToFloat()
        {
            llPool0.Height = llPool0.DesiredHeight;
            llPool1.Height = llPool1.DesiredHeight;
            llPool2.Height = llPool2.DesiredHeight;
            llPool3.Height = llPool3.DesiredHeight;
            llAncillary.Height = llAncillary.DesiredHeight;
            var poolRect1 = raGetPoolRect(0);
            raMovePool(1, poolRect1.Left, poolRect1.Bottom);
            var poolRect2 = raGetPoolRect(1);
            raMovePool(2, poolRect2.Left, poolRect2.Bottom);
            FixPrimarySecondaryHeight();
            var num = raGreater(raGetTop(), lblPool3.Top);
            if (num + llAncillary.DesiredHeight > ClientSize.Height)
            {
                num = ClientSize.Height - llAncillary.DesiredHeight - cbAncillary.Height - lblEpic.Height;
                var size = llPrimary.SizeNormal;
                llPrimary.SizeNormal = new Size(size.Width, num - 4 - llPrimary.Top);

                llSecondary.SizeNormal = new Size(llSecondary.SizeNormal.Width, num - 4 - llPrimary.Top);
            }

            var poolRect3 = raGetPoolRect(2);
            poolRect3.X = llPrimary.Left;
            poolRect3.Y = num;
            raMovePool(4, poolRect3.Left, poolRect3.Top);
            poolRect3.X = llSecondary.Left;
            raMovePool(3, poolRect3.Left, poolRect3.Top);
            return false;
        }

        private bool raToNormal()
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
            var num1 = llPool0.Top + cbPool0.Height * 4 + lblPool1.Height * 4;
            var num2 = 3 * llAncillary.ActualLineHeight;
            if (num1 + llPool0.Height + llPool1.Height + llPool2.Height + llPool3.Height + llAncillary.Height >
                ClientSize.Height)
            {
                var num3 = ClientSize.Height - num1 - llPool0.Height - llPool1.Height - llPool2.Height - llPool3.Height;
                if (num3 < num2)
                    num3 = num2;
                if (llAncillary.Height > num3)
                    llAncillary.Height = num3;
                if (num1 + llPool0.Height + llPool1.Height + llPool2.Height + llPool3.Height + llAncillary.Height >
                    ClientSize.Height)
                {
                    var num4 = ClientSize.Height - num1 - llPool0.Height - llPool1.Height - llPool2.Height -
                               llAncillary.Height;
                    if (num4 < num2)
                        num4 = num2;
                    llPool3.Height = num4;
                    if (num1 + llPool0.Height + llPool1.Height + llPool2.Height + llPool3.Height + llAncillary.Height >
                        ClientSize.Height)
                    {
                        var num5 = ClientSize.Height - num1 - llPool0.Height - llPool1.Height - llPool3.Height -
                                   llAncillary.Height;
                        if (num5 < num2)
                            num5 = num2;
                        llPool2.Height = num5;
                        if (num1 + llPool0.Height + llPool1.Height + llPool2.Height + llPool3.Height +
                            llAncillary.Height > ClientSize.Height)
                        {
                            var num6 = ClientSize.Height - num1 - llPool0.Height - llPool2.Height - llPool3.Height -
                                       llAncillary.Height;
                            if (num6 < num2)
                                num6 = num2;
                            llPool1.Height = num6;
                            if (num1 + llPool0.Height + llPool1.Height + llPool2.Height + llPool3.Height +
                                llAncillary.Height >
                                ClientSize.Height)
                            {
                                var num7 = ClientSize.Height - num1 - llPool1.Height - llPool2.Height - llPool3.Height -
                                           llAncillary.Height;
                                if (num7 < num2)
                                    num7 = num2;
                                llPool0.Height = num7;
                            }
                        }
                    }
                }
            }

            var poolRect = raGetPoolRect(0);
            raMovePool(1, poolRect.Left, poolRect.Bottom);
            poolRect = raGetPoolRect(1);
            raMovePool(2, poolRect.Left, poolRect.Bottom);
            poolRect = raGetPoolRect(2);
            raMovePool(3, poolRect.Left, poolRect.Bottom);
            poolRect = raGetPoolRect(3);
            raMovePool(4, poolRect.Left, poolRect.Bottom);
            llPool0.SuspendRedraw = false;
            llPool1.SuspendRedraw = false;
            llPool2.SuspendRedraw = false;
            llPool3.SuspendRedraw = false;
            llAncillary.SuspendRedraw = false;
            return false;
        }

        private bool ReArrange(bool Init)
        {
            bool flag2;
            if (drawing == null)
            {
                flag2 = false;
            }
            else
            {
                var flag3 = !dvAnchored.Visible;
                if (Init)
                {
                    flag2 = raToNormal();
                }
                else
                {
                    if (!flag3 & dvAnchored.Bounds.IntersectsWith(dvAnchored.SnapLocation))
                        raToNormal();
                    else
                        raToFloat();
                    SetAncilPoolHeight();
                    flag2 = false;
                }
            }

            return flag2;
        }

        private void RearrangeAllSlotsInBuild(PowerEntry?[] tp, bool notifyUser = false)
        {
            var index1 = 0;
            var numArray1 = new int[tp.Length];
            var num1 = tp.Length - 1;
            for (var index2 = 0; index2 <= num1; ++index2)
            {
                if (tp[index2].NIDPower == -1 || !DatabaseAPI.Database.Power[tp[index2].NIDPower].AllowFrontLoading)
                    continue;
                numArray1[index1] = index2;
                ++index1;
            }

            var index3 = index1;
            var num2 = tp.Length - 1;
            for (var index2 = 0; index2 <= num2; ++index2)
            {
                if (((tp[index2].NIDPower == -1 ? 0 :
                         !DatabaseAPI.Database.Power[tp[index2].NIDPower].AllowFrontLoading ? 1 : 0) |
                     (tp[index2].NIDPower == -1 ? 1 : 0)) == 0)
                    continue;
                var flag = true;
                var num3 = index2 - 1;
                for (var index4 = index1; index4 <= num3; ++index4)
                {
                    if (tp[index2].Level >= tp[numArray1[index4]].Level)
                        continue;
                    var num4 = index4;
                    for (var index5 = index3 - 1; index5 >= num4; index5 += -1)
                        numArray1[index5 + 1] = numArray1[index5];
                    numArray1[index4] = index2;
                    ++index3;
                    flag = false;
                    break;
                }

                if (!flag)
                    continue;
                numArray1[index3] = index2;
                ++index3;
            }

            var slotLevels = GetSlotLevels();
            var flag1 = false;
            var index6 = 0;
            var num5 = tp.Length - 1;
            for (var index2 = 0; index2 <= num5; ++index2)
            {
                var num3 = tp[numArray1[index2]].SlotCount - 1;
                for (var index4 = 1; index4 <= num3; ++index4)
                {
                    if (index6 == slotLevels.Length)
                        flag1 = true;
                    tp[numArray1[index2]].Slots[index4].Level = 50;
                    if (flag1)
                        continue;
                    if (tp[numArray1[index2]].NIDPower == -1 ||
                        !DatabaseAPI.Database.Power[tp[numArray1[index2]].NIDPower].AllowFrontLoading)
                        while (slotLevels[index6] <= tp[numArray1[index2]].Level)
                        {
                            ++index6;
                            if (index6 != slotLevels.Length)
                                continue;
                            flag1 = true;
                            break;
                        }

                    tp[numArray1[index2]].Slots[index4].Level = slotLevels[index6] - 1;
                    ++index6;
                }
            }

            if (!(flag1 & notifyUser))
                return;
            MessageBox.Show(
                @"The current arrangement of powers and their slots is impossible in-game. Invalid slots have been darkened and marked as level 51.",
                null, MessageBoxButtons.OK);
        }

        private void RedrawUnderPopup(Rectangle RectRedraw)
        {
            var Clip = RectRedraw;
            ref var local = ref Clip;
            var location = pnlGFXFlow.Location;
            var x = -location.X;
            location = pnlGFXFlow.Location;
            var y = -location.Y;
            local.Offset(x, y);
            drawing.Refresh(Clip);
            if (llPrimary.Bounds.IntersectsWith(RectRedraw))
                llPrimary.Refresh();
            if (llSecondary.Bounds.IntersectsWith(RectRedraw))
                llSecondary.Refresh();
            if (raGetPoolRect(0).IntersectsWith(RectRedraw))
            {
                llPool0.Refresh();
                cbPool0.Refresh();
                lblPool1.Refresh();
                lblLocked0.Refresh();
            }

            if (raGetPoolRect(1).IntersectsWith(RectRedraw))
            {
                llPool1.Refresh();
                cbPool1.Refresh();
                lblPool2.Refresh();
                lblLocked1.Refresh();
            }

            if (raGetPoolRect(2).IntersectsWith(RectRedraw))
            {
                llPool2.Refresh();
                cbPool2.Refresh();
                lblPool3.Refresh();
                lblLocked2.Refresh();
            }

            if (raGetPoolRect(3).IntersectsWith(RectRedraw))
            {
                llPool3.Refresh();
                cbPool3.Refresh();
                lblPool4.Refresh();
                lblLocked3.Refresh();
            }

            if (!raGetPoolRect(4).IntersectsWith(RectRedraw))
                return;
            llAncillary.Refresh();
            cbAncillary.Refresh();
            lblEpic.Refresh();
            lblLockedAncillary.Refresh();
        }

        private void RefreshInfo()
        {
            info_Totals();
            if (dvLastPower <= -1)
                return;
            Info_Power(dvLastPower, dvLastEnh, dvLastNoLev, DataViewLocked);
        }

        private void RefreshTabs(int iPower, I9Slot iEnh, int iLevel = -1)
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

        private void RemoveSlotFromTempList(PowerEntry? tp, int slotIDX)
        {
            tp.Slots = tp.Slots.RemoveIndex(slotIDX);
        }

        private void SetAncilPoolHeight()
        {
            var num1 = llAncillary.ActualLineHeight * 2;
            var num2 = 1;
            do
            {
                if (llAncillary.Top + num1 + llAncillary.ActualLineHeight <= ClientRectangle.Size.Height)
                    num1 += llAncillary.ActualLineHeight;
                ++num2;
            } while (num2 <= 4);

            if (num1 < llAncillary.ActualLineHeight * 2)
                num1 = llAncillary.ActualLineHeight * 2;
            llAncillary.Height = num1;
        }

        private void setColumns(int columns)
        {
            MidsContext.Config.Columns = columns;
            drawing.Columns = columns;
            DoResize();
            SetFormWidth();
            DoRedraw();
            pnlGFXFlow.AutoScroll = false;
            pnlGFXFlow.HorizontalScroll.Enabled = false;
            pnlGFXFlow.HorizontalScroll.Visible = false;
            pnlGFXFlow.HorizontalScroll.Maximum = 0;
            pnlGFXFlow.AutoScroll = true;
            //PerformAutoScale();
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
            if (index == 2)
            {
                drawing.InterfaceMode = Enums.eInterfaceMode.PowerToggle;
                DoRedraw();
                //Fix so tips only show once
                //MidsContext.Config.Tips.Show(Tips.TipType.TotalsTab);
            }
            else
            {
                if (drawing.InterfaceMode == Enums.eInterfaceMode.Normal)
                    return;
                drawing.InterfaceMode = Enums.eInterfaceMode.Normal;
                DoRedraw();
            }
        }

        private void SetFormHeight(bool Force = false)
        {
            int iVal2;
            var num = Height - ClientSize.Height;
            if (!dvAnchored.Visible)
                iVal2 = llPool3.Top + llPool3.Height * 2 + 4 + num;
            else
                switch (dvAnchored.VisibleSize)
                {
                    case Enums.eVisibleSize.Full:
                        var dvAnchoredSnapLocation = dvAnchored.SnapLocation;
                        iVal2 = raGreater(dvAnchoredSnapLocation.Bottom,
                            llAncillary.Top + llAncillary.ActualLineHeight * llAncillary.Items.Length) + 4 + num;
                        break;
                    case Enums.eVisibleSize.Small:
                        return;
                    case Enums.eVisibleSize.VerySmall:
                        return;
                    case Enums.eVisibleSize.Compact:
                        switch (drawing.EpicColumns)
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

            if ((iVal2 > Height) | Force | dvAnchored.IsDocked)
            {
                if (Screen.PrimaryScreen.WorkingArea.Height > iVal2)
                    Height = iVal2;
                else if (Screen.PrimaryScreen.WorkingArea.Height < iVal2)
                    Height = Screen.PrimaryScreen.WorkingArea.Height;
            }

            NoResizeEvent = false;
        }

        private void SetFormWidth(bool ToFull = false)
        {
            NoResizeEvent = true;
            var initialWidth = Width - ClientSize.Width;
            //int modifiedWidth;
            if (!MainModule.MidsController.IsAppInitialized)
            {
                return;
            }

            var num2 = (MidsContext.Config.Columns != 2 ? initialWidth + drawing.GetRequiredDrawingArea().Width + pnlGFXFlow.Left
                : !ToFull
                    ? initialWidth + pnlGFXFlow.Left + drawing.ScaleDown(drawing.GetDrawingArea().Width)
                    : initialWidth + drawing.GetDrawingArea().Width + pnlGFXFlow.Left) + 8;

            if (Screen.FromControl(this).WorkingArea.Width > num2)
            {
                Width = num2 - 30;
            }
            else
            {
                var workingArea = Screen.FromControl(this).WorkingArea;
                if (workingArea.Width <= num2)
                {
                    workingArea = Screen.FromControl(this).WorkingArea;
                    Width = workingArea.Width - initialWidth;
                }
            }
            NoResizeEvent = false;
            DoResize();
        }

        internal void SetMiniList(PopUp.PopupData iData, string iTitle, int bxHeight = 2048)
        {
            if (fMini == null)
                fMini = new frmMiniList(this);
            fMini.PInfo.BXHeight = bxHeight;
            fMini.SizeMe();
            fMini.Text = iTitle;
            fMini.PInfo.SetPopup(iData);
            fMini.Show();
            fMini.BringToFront();
        }

        private void SetPopupLocation(Rectangle ObjectBounds, bool PowerListing = false, bool Picker = false)
        {
            int y;
            var top = ObjectBounds.Top;
            var num1 = ClientSize.Height - ObjectBounds.Bottom;
            var left = ObjectBounds.Left;
            var num2 = ClientSize.Width - ObjectBounds.Right;
            var rectangle = new Rectangle(0, 0, 1, 1);
            if (dvAnchored.Visible)
            {
                rectangle.X = dvAnchored.Left;
                rectangle.Y = dvAnchored.Top;
                rectangle.Width = dvAnchored.Width;
                rectangle.Height = dvAnchored.Height;
            }

            var x = -1;
            Size clientSize;
            if (!PowerListing & !Picker)
            {
                if (num1 >= I9Popup.Height)
                {
                    y = ObjectBounds.Bottom;
                }
                else if (top >= I9Popup.Height)
                {
                    y = ObjectBounds.Top - I9Popup.Height;
                }
                else if (num2 >= I9Popup.Width)
                {
                    x = ObjectBounds.Right;
                    y = (int)Math.Round(ObjectBounds.Top + ObjectBounds.Height / 2.0 - I9Popup.Height / 2.0);
                }
                else if (left >= I9Popup.Width)
                {
                    x = ObjectBounds.Left - I9Popup.Width;
                    y = (int)Math.Round(ObjectBounds.Top + ObjectBounds.Height / 2.0 - I9Popup.Height / 2.0);
                }
                else
                {
                    y = ObjectBounds.Bottom;
                }
            }
            else if (Picker)
            {
                if (num2 >= I9Popup.Width)
                {
                    x = ObjectBounds.Right;
                    y = ObjectBounds.Top;
                }
                else if (left >= I9Popup.Width)
                {
                    x = ObjectBounds.Left - I9Popup.Width;
                    y = ObjectBounds.Top;
                }
                else
                {
                    y = num1 < I9Popup.Height
                        ? top < I9Popup.Height ? ObjectBounds.Bottom : ObjectBounds.Top - I9Popup.Height
                        : ObjectBounds.Bottom;
                }
            }
            else
            {
                y = (int)Math.Round(ObjectBounds.Top + ObjectBounds.Height / 2.0 - I9Popup.Height / 2.0);
                if (y < 0)
                    y = 0;
                var num3 = y + I9Popup.Height;
                clientSize = ClientSize;
                var height = clientSize.Height;
                if (num3 > height)
                {
                    clientSize = ClientSize;
                    y = clientSize.Height - I9Popup.Height;
                }

                x = ObjectBounds.Right;
            }

            if (x < 0)
            {
                x = (int)Math.Round(ObjectBounds.Left + ObjectBounds.Width / 2.0 - I9Popup.Width / 2.0);
                if (left < (I9Popup.Width - ObjectBounds.Width) / 2.0)
                {
                    x = left;
                }
                else if (num2 < (I9Popup.Width - ObjectBounds.Width) / 2.0)
                {
                    clientSize = ClientSize;
                    x = clientSize.Width - I9Popup.Width;
                }
            }

            if (y + I9Popup.Height > ClientSize.Height)
            {
                y -= y + I9Popup.Height - ClientSize.Height;
            }

            I9Popup.BringToFront();
            I9Popup.Location = new Point(x, y);
        }

        private void SetTitleBar(bool hero = true)
        {
            if (MainModule.MidsController.Toon != null)
            {
                hero = MainModule.MidsController.Toon.IsHero();
            }

            var str1 = string.Empty;
            if (MainModule.MidsController.Toon != null)
            {
                if (LastFileName != string.Empty)
                {
                    str1 = FileIO.StripPath(LastFileName) + " - ";
                    tsFileSave.Text = @"&Save '" + FileIO.StripPath(LastFileName) + @"'";
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

            var str2 = str1 + MidsContext.Title;
            if (!hero)
            {
                str2 = str2.Replace(nameof(hero), "Villain");
            }

            if (MidsContext.Config.MasterMode && !MidsContext.Config.IsLcAdmin)
            {
                Text = $@"{str2} (DB Admin) v{MidsContext.AssemblyVersion} {MidsContext.AppVersionStatus} ({DatabaseAPI.DatabaseName} Issue: {DatabaseAPI.Database.Issue}, {DatabaseAPI.Database.PageVolText}: {DatabaseAPI.Database.PageVol} - DBVersion: {DatabaseAPI.Database.Version})";
            }
            else if (MidsContext.Config.MasterMode && MidsContext.Config.IsLcAdmin)
            {
                Text = $@"{str2} (LC Admin) v{MidsContext.AssemblyVersion} {MidsContext.AppVersionStatus} ({DatabaseAPI.DatabaseName} Issue: {DatabaseAPI.Database.Issue}, {DatabaseAPI.Database.PageVolText}: {DatabaseAPI.Database.PageVol} - DBVersion: {DatabaseAPI.Database.Version})";
            }
            else
            {
                Text = $@"{str2} v{MidsContext.AssemblyVersion} {MidsContext.AppVersionStatus} ({DatabaseAPI.DatabaseName} Issue: {DatabaseAPI.Database.Issue}, {DatabaseAPI.Database.PageVolText}: {DatabaseAPI.Database.PageVol} - DBVersion: {DatabaseAPI.Database.Version})";
            }
        }

        public void UpdateTitle()
        {
            TitleUpdated?.Invoke(this, null);
        }

        private void OnTitleUpdate(object? sender, EventArgs eventArgs)
        {
            SetTitleBar(MidsContext.Character.IsHero());
        }

        private static void ShallowCopyPowerList(PowerEntry?[] source)
        {
            var num = MidsContext.Character.CurrentBuild.Powers.Count - 1;
            for (var index = 0; index <= num; ++index)
                MidsContext.Character.CurrentBuild.Powers[index] = source[index];
        }

        internal void ShowAnchoredDataView()
        {
            if (FloatingDataForm != null)
            {
                dvAnchored.VisibleSize = FloatingDataForm.dvFloat.VisibleSize;
                dvAnchored.TabPage = FloatingDataForm.dvFloat.TabPage;
            }

            myDataView = dvAnchored;
            myDataView.Init();
            myDataView.BackColor = BackColor;
            myDataView.DrawVillain = !MainModule.MidsController.Toon.IsHero();
            dvAnchored.Visible = true;
            NoResizeEvent = true;
            OnResizeEnd(new EventArgs());
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

                var bounds = I9Popup.Bounds;
                RedrawUnderPopup(bounds);
                if (!((nIdPowerset > -1) | (nIdClass > -1)))
                {
                    return;
                }

                if (I9Popup.psIDX != (nIdPowerset <= -1 ? nIdClass : nIdPowerset))
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
                        I9Popup.SetPopup(iPopup);
                        if (vAlign == VerticalAlignment.Bottom)
                        {
                            I9Popup.Location = new Point(I9Popup.Location.X, I9Popup.Location.Y - I9Popup.Height);
                            rBounds.Y -= I9Popup.Height;
                        }

                        PopUpVisible = true;
                        SetPopupLocation(rBounds, false, true);
                    }
                    else
                    {
                        HidePopup();
                    }

                    I9Popup.Visible = true;
                    if (ActivePopupBounds != I9Popup.Bounds)
                    {
                        RedrawUnderPopup(bounds);
                        ActivePopupBounds = I9Popup.Bounds;
                    }
                }

                I9Popup.hIDX = -1;
                I9Popup.eIDX = -1;
                I9Popup.pIDX = -1;
                I9Popup.psIDX = nIdPowerset;
            }
        }

        private void ShowPopup(int hIdx, int pIdx, int sIdx, Point e, Rectangle rBounds, I9Slot eSlot = null, int setIdx = -1, VerticalAlignment vAlign = VerticalAlignment.Top)
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
                var bounds = I9Popup.Bounds;
                if ((hIdx < 0) & (pIdx > -1))
                {
                    hIdx = MidsContext.Character.CurrentBuild.FindInToonHistory(pIdx);
                }

                PowerEntry? powerEntry = null;
                if (hIdx > -1)
                {
                    powerEntry = MidsContext.Character.CurrentBuild.Powers[hIdx];
                }

                if (!((I9Popup.hIDX != hIdx) | (I9Popup.eIDX != sIdx) | (I9Popup.pIDX != pIdx) | (I9Popup.hIDX == -1) | (I9Popup.eIDX == -1) | (I9Popup.pIDX == -1)))
                {
                    return;
                }

                var rectangle = new Rectangle();
                if ((hIdx > -1) & (sIdx < 0) & (pIdx < 0) & (eSlot == null) & (setIdx < 0))
                {
                    rectangle = drawing.PowerBoundsUnScaled(hIdx);
                    var e1 = new Point(drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y));
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
                    rectangle = drawing.PowerBoundsUnScaled(hIdx);
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
                else if ((eSlot != null) & (setIdx < 0))
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

                if (flag & (iPopup.Sections != null))
                {
                    if ((I9Popup.hIDX != hIdx) | (I9Popup.eIDX != sIdx) | (I9Popup.pIDX != pIdx) | (I9Popup.hIDX == -1) | (I9Popup.eIDX == -1) | (I9Popup.pIDX == -1))
                    {
                        if (!picker & !powerListing)
                        {
                            rectangle = Dilate(drawing.ScaleDown(rectangle), 2);
                            rectangle.X += pnlGFXFlow.Left - pnlGFXFlow.HorizontalScroll.Value;
                            rectangle.Y += pnlGFXFlow.Top - pnlGFXFlow.VerticalScroll.Value;
                        }

                        I9Popup.SetPopup(iPopup);
                        if (vAlign == VerticalAlignment.Bottom)
                        {
                            rectangle.Y -= rectangle.Height;
                        }
                        //else if (rectangle.Bottom > ClientSize.Height - MenuBar.Height)
                        //{
                        //    rectangle.Y -= rectangle.Bottom - (ClientSize.Height - MenuBar.Height); // I9Popup.Height
                        //}

                        PopUpVisible = true;
                        SetPopupLocation(rectangle, powerListing, picker);
                    }

                    I9Popup.Visible = true;
                    if (ActivePopupBounds != I9Popup.Bounds)
                    {
                        RedrawUnderPopup(bounds);
                        ActivePopupBounds = I9Popup.Bounds;
                    }
                }
                else
                {
                    HidePopup();
                }

                I9Popup.hIDX = hIdx;
                I9Popup.eIDX = sIdx;
                I9Popup.pIDX = pIdx;
                I9Popup.psIDX = -1;
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
                    return;
            }

            if ((MidsContext.Character.CurrentBuild.Powers[destPower].Slots[destSlot].Level <
                 MidsContext.Character.CurrentBuild.Powers[sourcePower].Level) & !DatabaseAPI.Database
                .Power[MidsContext.Character.CurrentBuild.Powers[sourcePower].NIDPower].AllowFrontLoading)
            {
                CheckInitDdsaValue(14, 0, "Slot being level-swapped is too low for the source power",
                    "Allow swap anyway (mark as invalid)");
                if (dragdropScenarioAction[14] == 1)
                    return;
            }

            var level = MidsContext.Character.CurrentBuild.Powers[sourcePower].Slots[sourceSlot].Level;
            MidsContext.Character.CurrentBuild.Powers[sourcePower].Slots[sourceSlot].Level =
                MidsContext.Character.CurrentBuild.Powers[destPower].Slots[destSlot].Level;
            MidsContext.Character.CurrentBuild.Powers[destPower].Slots[destSlot].Level = level;
            PowerModified(true);
            DoRedraw();
        }

        internal void smlRespecLong(int iLevel, bool mode2)
        {
            SetMiniList(MidsContext.Character.CurrentBuild.GetRespecHelper2(true, iLevel), "Respec Helper",
                mode2 ? 5096 : 4072);
            fMini.Width = 350;
            fMini.SizeMe();
        }

        internal void smlRespecShort(int iLevel, bool mode2)
        {
            var helper = MidsContext.Character.CurrentBuild.GetRespecHelper2(false, iLevel);
            if (mode2)
                SetMiniList(helper, "Respec Helper", 4072);
            else
                SetMiniList(helper, "Respec Helper");
            fMini.Width = mode2 ? 300 : 250;
            fMini.SizeMe();
        }

        private void StartFlip(int iPowerIndex)
        {
            if (FlipActive)
                EndFlip();
            if (iPowerIndex <= -1 || MidsContext.Character.CurrentBuild.Powers[iPowerIndex].Slots.Length == 0)
                return;
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
            tmrGfx ??= new Timer(Container!);
            tmrGfx.Interval = FlipInterval;
            FlipActive = true;
            tmrGfx.Enabled = true;
            tmrGfx.Start();
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
                    var num = power.NIDSubPower.Length - 1;
                    for (var index = 0; index <= num; ++index)
                    {
                        var thisPower = DatabaseAPI.Database.Power[power.NIDSubPower[index]];
                        if (thisPower != null && (thisPower.ClickBuff ||
                                                  thisPower.PowerType == Enums.ePowerType.Auto_ |
                                                  thisPower.PowerType == Enums.ePowerType.Toggle))
                        {
                            iPowers.Add(thisPower);
                        }
                    }
                }

                fTemp = new frmTemp(this, iPowers)
                {
                    Text = @"Temporary Powers"
                };
            }

            if (!fTemp.Visible)
                fTemp.Show(this);
        }

        private void ibPetsEx_OnClick(object? sender, EventArgs e)
        {
            if (petWindowFlag)
            {
                ibPetsEx.ToggleState = ImageButtonEx.States.ToggledOff;
                fMMPets?.Close();
                petWindowFlag = false;
            }
            else
            {
                // Zed: added check on level so hidden Henchmen powers (*_H) don't get in,
                // leading to duplicates.
                MMPets = MidsContext.Character?.CurrentBuild.Powers == null
                    ? new List<string>()
                    : MidsContext.Character.CurrentBuild.Powers
                    .Where(e =>
                        e is { Power: { } } &&
                        Enum.GetNames(typeof(Enums.eMMpets)).Contains(e.Name.Replace(" ", "_")) &
                        e.Power.Level > 0)
                    .Select(e => e.Name)
                    .ToList();

                var isEmpty = MMPets.Count <= 0;
                if (!isEmpty)
                {
                    fMMPets = new frmMMPowers(this, MMPets)
                    {
                        Text = @"Mastermind Pets"
                    };

                    if (!fMMPets.Visible)
                    {
                        ibPetsEx.ToggleState = ImageButtonEx.States.ToggledOn;
                        petWindowFlag = true;
                        fMMPets.Show(this);
                    }
                    else
                    {
                        ibPetsEx.ToggleState = ImageButtonEx.States.ToggledOff;
                        fMMPets.Close();
                    }
                }
                else
                {
                    MessageBox.Show(@"You haven't selected an pet abilities for your mastermind.", @"Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    ibPetsEx.ToggleState = ImageButtonEx.States.ToggledOff;
                }
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
                doFlipStep();
        }
        //
        private bool ToggleClicked(int hID, int iX, int iY)
        {
            var rectangle1 = new Rectangle();
            if (hID < 0)
                return false;
            if (MidsContext.Character.CurrentBuild.Powers[hID].IDXPower < 0)
                return false;
            var rectangle2 = new Rectangle
            {
                Location = drawing.PowerPosition(MidsContext.Character.CurrentBuild.Powers[hID]),
                Size = drawing.bxPower[0].Size
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
                return false;
            if (MidsContext.Character.CurrentBuild.Powers[hID].IDXPower < 0)
                return false;
            var rectangle2 = new Rectangle
            {
                Location = drawing.PowerPosition(MidsContext.Character.CurrentBuild.Powers[hID]),
                Size = drawing.bxPower[0].Size
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
            if (result != DialogResult.OK) return;
            var dbSelected = dbSelector.SelectedDatabase;
            MidsContext.Config.DataPath = dbSelected;
            MidsContext.Config.SavePath = dbSelected;
            MidsContext.Config.SaveConfig(Serializer.GetSerializer());
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
                var serializer = Serializer.GetSerializer();
                MidsContext.Config.SaveConfig(serializer);
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
            clsXMLUpdate.KoFi();
        }

        private void tsPatreon_Click(object? sender, EventArgs e)
        {
            clsXMLUpdate.Patreon();
        }

        private void tsCoinbase_Click(object? sender, EventArgs e)
        {
            clsXMLUpdate.CoinBase();
        }

        private void tsSupport_Click(object sender, EventArgs e)
        {
            clsXMLUpdate.SupportServer();
        }

        private void OnRelativeClick(Enums.eEnhRelative newVal)
        {
            if (MainModule.MidsController.Toon == null)
                return;
            if (MidsContext.Character.CurrentBuild.SetEnhRelativeLevels(newVal))
                I9Picker.Ui.Initial.RelLevel = newVal;
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

        private void OnGradePick(Enums.eEnhGrade grade)
        {
            if (MidsContext.Character == null)
                return;
            if (MidsContext.Character.CurrentBuild.SetEnhGrades(grade))
                I9Picker.Ui.Initial.GradeId = grade;
            info_Totals();
            DoRedraw();
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

        private void ForumExport_Click(object? sender, EventArgs e)
        {
            if (MidsContext.Config == null || MidsContext.Character == null || drawing == null) return;
            FloatTop(false);
            var exportResult = MessageBox.Show(@"Do you wish to export the full build?", @"Export Type (Short/Long)", MessageBoxButtons.YesNoCancel);
            switch (exportResult)
            {
                case DialogResult.Yes:
                    MidsContext.Config.LongExport = true;
                    break;
                case DialogResult.No:
                    MidsContext.Config.LongExport = false;
                    break;
                case DialogResult.Cancel:
                    return;
            }

            using var forumExport = new frmForum
            {
                BackColor = BackColor,
                IBCancel =
                {
                    IA = drawing.pImageAttributes,
                    ImageOff = MidsContext.Character.IsHero() ? drawing.bxPower[2].Bitmap : drawing.bxPower[4].Bitmap,
                    ImageOn = MidsContext.Character.IsHero() ? drawing.bxPower[3].Bitmap : drawing.bxPower[5].Bitmap
                },
                IBExport =
                {
                    IA = drawing.pImageAttributes,
                    ImageOff = MidsContext.Character.IsHero() ? drawing.bxPower[2].Bitmap : drawing.bxPower[4].Bitmap,
                    ImageOn = MidsContext.Character.IsHero() ? drawing.bxPower[3].Bitmap : drawing.bxPower[5].Bitmap
                }
            };
            forumExport.ShowDialog(this);
            FloatTop(true);
            if (MidsContext.Config.LongExport) MidsContext.Config.LongExport = false;
        }

        private void tsExportDataLink_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(MidsCharacterFileFormat.MxDBuildSaveHyperlink(false, true), true);
            MessageBox.Show("The data link has been placed on the clipboard and is ready to paste.", "Export Done",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void tsShareDiscord_Click(object sender, EventArgs e)
        {
            try
            {
                new DiscordShare().ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\r\n\r\n{ex.StackTrace}", @"Debug Error", MessageBoxButtons.OK);
            }
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
            if (MainModule.MidsController.Toon.Locked & FileModified)
            {
                FloatTop(false);
                var msgBoxResult = MessageBox.Show("Current hero/villain data will be discarded, are you sure?",
                    "Question",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                FloatTop(true);
                if (msgBoxResult == DialogResult.No)
                    return;
            }

            if (DlgOpen.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            FloatTop(false);
            MidsContext.EnhCheckMode = false;
            if (fRecipe != null && fRecipe.Visible)
            {
                fRecipe.UpdateData();
            }

            if (fSalvageHud != null && fSalvageHud.Visible)
            {
                FloatBuildSalvageHud(false);
            }

            DoOpen(DlgOpen.FileName);
            FloatTop(true);
            var containsPower = MidsContext.Character.CurrentBuild.Powers
                .Where(pe => pe?.Power != null)
                .ToList()
                .Exists(x => Enum.IsDefined(typeof(Enums.eGridType), x.Power.InherentType));
            if (containsPower && ActiveForm == this)
            {
                pnlGFX.Update();
                pnlGFX.Refresh();
            }
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
            doSave();
        }

        private void tsFileSaveAs_Click(object sender, EventArgs e)
        {
            doSaveAs();
        }

        private static void inputBox_Validating(object sender, InputBoxValidatingArgs e)
        {
            if (e.Text.Trim().Length != 0) return;
            e.Cancel = true;
            e.Message = "Required";
        }

        void tsGenFreebies_Click(object sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon == null) return;
            FloatTop(false);
            // Zed:
            // Rev. 2: use a folder picker instead of a file picker so the data folder
            // and sub directories can be automatically created.
            var dirSelector = new FolderBrowserDialog
            {
                Description = @"Select your base CoH directory:",
                ShowNewFolderButton = false
            };
            var dsr = dirSelector.ShowDialog();
            if (dsr == DialogResult.Cancel) return;
            var iResult = InputBox.Show("Enter a name for the popmenu", "Name your menu", false, "Enter the menu name here", InputBox.InputBoxIcon.Info, inputBox_Validating);
            if (!iResult.OK) return;
            clsGenFreebies.MenuName = iResult.Text;
            Directory.CreateDirectory(dirSelector.SelectedPath + @"\data\texts\English\Menus");
            var mnuFileName = dirSelector.SelectedPath + @"\data\texts\English\Menus\" + clsGenFreebies.MenuName + "." + clsGenFreebies.MenuExt;
            var saveOp = clsGenFreebies.SaveTo(mnuFileName);
            if (!saveOp)
            {
                MessageBox.Show("Couldn't save popmenu to file: " + mnuFileName, "Welp", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Popmenu created.\r\nIf necessary, restart your client for it to become available for use.\r\nUse /popmenu " + clsGenFreebies.MenuName + " to open it.",
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
            new FrmInputLevel(this, true, false).ShowDialog(this);
        }

        private void tsHelperLong2_Click(object sender, EventArgs e)
        {
            new FrmInputLevel(this, true, true).ShowDialog(this);
        }

        private void tsHelperShort_Click(object sender, EventArgs e)
        {
            new FrmInputLevel(this, false, false).ShowDialog(this);
        }

        private void tsHelperShort2_Click(object sender, EventArgs e)
        {
            new FrmInputLevel(this, false, true).ShowDialog(this);
        }

        private void tsImport_Click(object sender, EventArgs e)
        {
            command_ForumImport();
        }

        private void tsIODefault_Click(object sender, EventArgs e)
        {
            if (MidsContext.Character.CurrentBuild.SetIOLevels(MidsContext.Config.I9.DefaultIOLevel, false, false))
                I9Picker.LastLevel = MidsContext.Config.I9.DefaultIOLevel + 1;
            DoRedraw();
        }

        private void tsIOMax_Click(object sender, EventArgs e)
        {
            if (MidsContext.Character.CurrentBuild.SetIOLevels(MidsContext.Config.I9.DefaultIOLevel, false, true))
                I9Picker.LastLevel = 50;
            DoRedraw();
        }

        private void tsIOMin_Click(object sender, EventArgs e)
        {
            if (MidsContext.Character.CurrentBuild.SetIOLevels(MidsContext.Config.I9.DefaultIOLevel, true, false))
                I9Picker.LastLevel = 10;
            DoRedraw();
        }

        private void tsRecipeViewer_Click(object sender, EventArgs e)
        {
            FloatRecipe(true);
        }

        private void tsDPSCalc_Click(object sender, EventArgs e)
        {
            FloatDPSCalc(true);
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
                        MidsContext.Character.CurrentBuild.Powers[index].Slots =
                            MidsContext.Character.CurrentBuild.Powers[index].Slots.Take(1).ToArray();

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
            FloatSetFinder(true);
        }

        private void tsForumLink(object sender, EventArgs e)
        {
            clsXMLUpdate.GoToForums();
        }

        private void Github_Link(object? sender, EventArgs e)
        {
            clsXMLUpdate.GoToGitHub();
        }

        private void tsPatchNotes_Click(object sender, EventArgs e)
        {
            PatchNotes patchNotes;
            var patchResult = new PatchQuery(this);
            patchResult.ShowDialog();
            switch (patchResult.DialogResult)
            {
                case DialogResult.Yes:
                {
                    patchNotes = new PatchNotes(this, false)
                    {
                        Type = clsXMLUpdate.UpdateType.App.ToString(),
                        Version = MidsContext.AppFileVersion.ToString()
                    };
                    patchNotes.ShowDialog();
                    break;
                }
                case DialogResult.No:
                    patchNotes = new PatchNotes(this, false)
                    {
                        Type = clsXMLUpdate.UpdateType.Database.ToString(),
                        Version = DatabaseAPI.Database.Version.ToString()
                    };
                    patchNotes.ShowDialog();
                    break;
                case DialogResult.Cancel:
                    patchResult.Close();
                    break;
            }
        }

        private void tsUpdateCheck_Click(object sender, EventArgs e)
        {
            clsXMLUpdate.CheckUpdate(this);
        }

        private void tsViewSelected()
        {
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
        }

        private void tsView2Col_Click(object sender, EventArgs e)
        {
            tsView3Col.Checked = false;
            tsView4Col.Checked = false;
            tsView5Col.Checked = false;
            tsView6Col.Checked = false;
            tsView2Col.Checked = true;
            setColumns(2);
        }

        private void tsView3Col_Click(object sender, EventArgs e)
        {
            tsView2Col.Checked = false;
            tsView4Col.Checked = false;
            tsView5Col.Checked = false;
            tsView6Col.Checked = false;
            tsView3Col.Checked = true;
            setColumns(3);
        }

        private void tsView4Col_Click(object sender, EventArgs e)
        {
            tsView2Col.Checked = false;
            tsView3Col.Checked = false;
            tsView5Col.Checked = false;
            tsView6Col.Checked = false;
            tsView4Col.Checked = true;
            setColumns(4);
        }

        private void tsView5Col_Click(object sender, EventArgs e)
        {
            tsView2Col.Checked = false;
            tsView3Col.Checked = false;
            tsView4Col.Checked = false;
            tsView6Col.Checked = false;
            tsView5Col.Checked = true;
            setColumns(5);
        }

        private void tsView6Col_Click(object sender, EventArgs e)
        {
            tsView2Col.Checked = false;
            tsView3Col.Checked = false;
            tsView4Col.Checked = false;
            tsView5Col.Checked = false;
            tsView6Col.Checked = true;
            setColumns(6);
        }

        private void tsViewActualDamage_New_Click(object sender, EventArgs e)
        {
            if (MidsContext.Config != null)
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
            if (MidsContext.Config != null) MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.DPS;
            SetDamageMenuCheckMarks();
            DisplayFormatChanged();
        }

        private void tsViewGraphs_Click(object sender, EventArgs e)
        {
            FloatStatGraph(true);
        }

        private void tsViewIOLevels_Click(object sender, EventArgs e)
        {
            if (MidsContext.Config != null)
            {
                MidsContext.Config.I9.HideIOLevels = !MidsContext.Config.I9.HideIOLevels;
                tsViewIOLevels.Checked = !MidsContext.Config.I9.HideIOLevels;
            }

            DoRedraw();
        }

        private void tsViewSOLevels_Click(object sender, EventArgs e)
        {
            if (MidsContext.Config != null)
            {
                MidsContext.Config.ShowSoLevels = !MidsContext.Config.ShowSoLevels;
                tsViewSOLevels.Checked = MidsContext.Config.ShowSoLevels;
            }

            DoRedraw();
        }

        private void tsViewRelative_Click(object sender, EventArgs e)
        {
            if (MidsContext.Config != null)
            {
                MidsContext.Config.ShowEnhRel = !MidsContext.Config.ShowEnhRel;
                tsViewRelative.Checked = MidsContext.Config.ShowEnhRel;
            }

            DoRedraw();
        }

        private void tsViewRelativeAsSigns_Click(object sender, EventArgs e)
        {
            if (MidsContext.Config != null)
            {
                MidsContext.Config.ShowRelSymbols = !MidsContext.Config.ShowRelSymbols;
                tsViewRelativeAsSigns.Checked = MidsContext.Config.ShowRelSymbols;
            }

            DoRedraw();
        }

        private void tsViewSetCompare_Click(object sender, EventArgs e)
        {
            FloatCompareGraph(true);
        }

        private void tsViewSets_Click(object sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon == null)
                return;
            FloatSets(true);
        }

        private void tsViewSlotLevels_Click(object sender, EventArgs e)
        {
            if (MidsContext.Config != null)
            {
                MidsContext.Config.ShowSlotLevels = !MidsContext.Config.ShowSlotLevels;
                tsViewSlotLevels.Checked = MidsContext.Config.ShowSlotLevels;
                ibSlotLevelsEx.ToggleState = MidsContext.Config.ShowSlotLevels switch
                {
                    true => ImageButtonEx.States.ToggledOn,
                    false => ImageButtonEx.States.ToggledOff
                };
            }

            DoRedraw();
        }

        private void tsViewTotals_Click(object sender, EventArgs e)
        {
            FloatTotals(true, MidsContext.Config is { UseOldTotalsWindow: true });
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            if (MidsContext.Character != null) MidsContext.Character.Name = txtName.Text;
            if (fTotals2 != null) frmTotalsV2.SetTitle(fTotals2);
            DisplayName();
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
            I9Picker.Selected = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerHighlightHero
                : MidsContext.Config.RtFont.ColorPowerHighlightVillain;
            I9Picker.BackColor = BackColor;
            I9Popup.BackColor = Color.Black;
            I9Popup.ForeColor = I9Picker.ForeColor;
            myDataView.BackColor = BackColor;
            /*var style = !MidsContext.Config.RtFont.PowersSelectBold ? FontStyle.Regular : FontStyle.Bold;
            using var font = new Font(llPrimary.Font.FontFamily, MidsContext.Config.RtFont.PowersSelectBase, style, GraphicsUnit.Point);*/
            //using var font = new Font("Arial", 12f, FontStyle.Bold, GraphicsUnit.Pixel);
            var toColor = new Control[]
            {
                llPrimary, llSecondary, llPool0, llPool1, llPool2, llPool3, llAncillary, lblName, lblAT, lblOrigin,
                lblCharacter, pnlGFX
            };
            foreach (var colorItem in toColor)
            {
                colorItem.BackColor = BackColor;
                if (!(colorItem is ListLabelV3 ll))
                    continue;
                UpdateLLColors(ll);
                //ll.Font = font;
            }

            var toOtherColor = new Control[]
            {
                lblLocked0, lblLocked1, lblLocked2, lblLocked3, lblLockedAncillary, lblLockedSecondary, lblATLocked
            };
            foreach (var colorItem in toOtherColor) colorItem.BackColor = lblATLocked.BackColor;

            foreach (var llControl in Controls.OfType<ListLabelV3>())
            {
                llControl.ScrollBarColor = MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenHero
                    : MidsContext.Config.RtFont.ColorPowerTakenVillain;
                llControl.ScrollButtonColor = MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                    : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain;
                llControl.UpdateTextColors(ListLabelV3.LLItemState.Selected,
                    MidsContext.Character.IsHero()
                        ? MidsContext.Config.RtFont.ColorPowerTakenHero
                        : MidsContext.Config.RtFont.ColorPowerTakenVillain);
                llControl.UpdateTextColors(ListLabelV3.LLItemState.SelectedDisabled,
                    MidsContext.Character.IsHero()
                        ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                        : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain);
                llControl.HoverColor = MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerHighlightHero
                    : MidsContext.Config.RtFont.ColorPowerHighlightVillain;
            }

            if (fRecipe != null && fRecipe.Visible)
            {
                fRecipe.UpdateColorTheme();
            }

            if (fSalvageHud != null && fSalvageHud.Visible)
            {
                fSalvageHud.UpdateColorTheme();
            }

            if (!draw)
                return;
            if (!skipDraw)
                DoRedraw();
            UpdateDmBuffer();
        }

        private void UpdatePoolsPanelSize()
        {
            poolsPanel.Height = Math.Max(32, Size.Height - poolsPanel.Location.Y - 39); // 16 + 22 + 1
        }

        private void UpdateControls(bool ForceComplete = false)
        {
            if (loading) return;
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
                true => ImageButtonEx.States.ToggledOn,
                false => ImageButtonEx.States.ToggledOff
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
                I9Gfx.SetOrigin(cbOrigin.SelectedItem);
            }

            ComboCheckPS(CbtPrimary.Value, Enums.PowersetType.Primary, Enums.ePowerSetType.Primary);
            ComboCheckPS(CbtSecondary.Value, Enums.PowersetType.Secondary, Enums.ePowerSetType.Secondary);
            cbSecondary.Enabled = MidsContext.Character.Powersets[0].nIDLinkSecondary <= -1;
            ComboCheckPool(CbtPool0.Value, Enums.ePowerSetType.Pool);
            ComboCheckPool(CbtPool1.Value, Enums.ePowerSetType.Pool);
            ComboCheckPool(CbtPool2.Value, Enums.ePowerSetType.Pool);
            ComboCheckPool(CbtPool3.Value, Enums.ePowerSetType.Pool);
            ComboCheckPool(CbtAncillary.Value, Enums.ePowerSetType.Ancillary);
            cbPool0.SelectedIndex =
                MainModule.MidsController.Toon.PoolToComboID(0, MidsContext.Character.Powersets[3]?.nID ?? -1);
            cbPool1.SelectedIndex =
                MainModule.MidsController.Toon.PoolToComboID(1, MidsContext.Character.Powersets[4]?.nID ?? -1);
            cbPool2.SelectedIndex =
                MainModule.MidsController.Toon.PoolToComboID(2, MidsContext.Character.Powersets[5]?.nID ?? -1);
            cbPool3.SelectedIndex =
                MainModule.MidsController.Toon.PoolToComboID(3, MidsContext.Character.Powersets[6]?.nID ?? -1);
            var powersetIndexes =
                DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, Enums.ePowerSetType.Ancillary);
            cbAncillary.SelectedIndex = MidsContext.Character.Powersets[7] != null
                ? DatabaseAPI.ToDisplayIndex(MidsContext.Character.Powersets[7], powersetIndexes)
                : 0;
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
            lblLocked0.Location = cbPool0.Location;
            lblLocked0.Size = cbPool0.Size;
            lblLocked0.Text = cbPool0.Text;
            lblLocked0.Visible = MainModule.MidsController.Toon.PoolLocked[0];
            lblLocked1.Location = cbPool1.Location;
            lblLocked1.Size = cbPool1.Size;
            lblLocked1.Text = cbPool1.Text;
            lblLocked1.Visible = MainModule.MidsController.Toon.PoolLocked[1];
            lblLocked2.Location = cbPool2.Location;
            lblLocked2.Size = cbPool2.Size;
            lblLocked2.Text = cbPool2.Text;
            lblLocked2.Visible = MainModule.MidsController.Toon.PoolLocked[2];
            lblLocked3.Location = cbPool3.Location;
            lblLocked3.Size = cbPool3.Size;
            lblLocked3.Text = cbPool3.Text;
            lblLocked3.Visible = MainModule.MidsController.Toon.PoolLocked[3];
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
            foreach (var llControl in Controls.OfType<ListLabelV3>().Concat(poolsPanel.Controls.OfType<ListLabelV3>()))
            {
                var style = !MidsContext.Config.RtFont.PowersSelectBold ? FontStyle.Regular : FontStyle.Bold;
                llControl.Font = new Font(llControl.Font.FontFamily, MidsContext.Config.RtFont.PowersSelectBase, style, GraphicsUnit.Point);
                foreach (var e in llControl.Items)
                {
                    e.Bold = MidsContext.Config.RtFont.PowersSelectBold;
                }
            }

            ibAlignmentEx.ToggleState = MidsContext.Character.IsHero() switch
            {
                true => ImageButtonEx.States.ToggledOff,
                false => ImageButtonEx.States.ToggledOn
            };

            dvAnchored.SetLocation(new Point(llPrimary.Left, llPrimary.Top + raGreater(llPrimary.SizeNormal.Height, llSecondary.SizeNormal.Height) + 5), ForceComplete);
            llPrimary.SuspendRedraw = false;
            llSecondary.SuspendRedraw = false;
            if (myDataView != null && (drawing.InterfaceMode == Enums.eInterfaceMode.Normal) & (myDataView.TabPageIndex == 2))
                dvAnchored_TabChanged(myDataView.TabPageIndex);
            if (MidsContext.Config.BuildMode == Enums.dmModes.LevelUp)
            {
                UpdateDmBuffer();
                //pbDynMode.Refresh();
            }

            DoResize();
            NoUpdate = false;
        }

        private void UpdateDmBuffer()
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Config == null || MidsContext.Character == null) return;

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

            if (ibDynMode.Lock && MidsContext.Config.BuildMode == Enums.dmModes.LevelUp) ibDynMode.Lock = false;
            else if (MidsContext.Config.BuildMode != Enums.dmModes.LevelUp) ibDynMode.Lock = false;
            switch (powerState)
            {
                case Enums.ePowerState.Used:
                    ibDynMode.ToggleText.ToggledOff = text;
                    ibDynMode.ToggleState = ImageButtonEx.States.ToggledOff;
                    break;
                case Enums.ePowerState.Open:
                    ibDynMode.ToggleText.ToggledOn = text;
                    ibDynMode.ToggleState = ImageButtonEx.States.ToggledOn;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (!ibDynMode.Lock && MidsContext.Config.BuildMode == Enums.dmModes.LevelUp) ibDynMode.Lock = true;
        }

        private void UpdateLLColors(ListLabelV3 iList)
        {
            iList.UpdateTextColors(ListLabelV3.LLItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            iList.UpdateTextColors(ListLabelV3.LLItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            iList.UpdateTextColors(ListLabelV3.LLItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
            iList.ScrollBarColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenHero
                : MidsContext.Config.RtFont.ColorPowerTakenVillain;
            iList.ScrollButtonColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain;
            iList.UpdateTextColors(ListLabelV3.LLItemState.Selected,
                MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenHero
                    : MidsContext.Config.RtFont.ColorPowerTakenVillain);
            iList.UpdateTextColors(ListLabelV3.LLItemState.SelectedDisabled,
                MidsContext.Character.IsHero()
                    ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                    : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain);
            iList.HoverColor = MidsContext.Character.IsHero()
                ? MidsContext.Config.RtFont.ColorPowerHighlightHero
                : MidsContext.Config.RtFont.ColorPowerHighlightVillain;
        }

        private void UpdateOtherFormsFonts()
        {
            if (fIncarnate != null)
            {
                var fIncarnate = this.fIncarnate;
                if (fIncarnate.Visible)
                {
                    foreach (var llControl in fIncarnate.Controls.OfType<ListLabelV3>())
                    {
                        llControl.SuspendRedraw = true;
                        llControl.Font = llPrimary.Font;
                        llControl.UpdateTextColors(ListLabelV3.LLItemState.Enabled,
                            MidsContext.Config.RtFont.ColorPowerAvailable);
                        llControl.UpdateTextColors(ListLabelV3.LLItemState.Disabled,
                            MidsContext.Config.RtFont.ColorPowerDisabled);
                        llControl.UpdateTextColors(ListLabelV3.LLItemState.Invalid,
                            Color.FromArgb(byte.MaxValue, 0, 0));
                        llControl.ScrollBarColor = MidsContext.Character.IsHero()
                            ? MidsContext.Config.RtFont.ColorPowerTakenHero
                            : MidsContext.Config.RtFont.ColorPowerTakenVillain;
                        llControl.ScrollButtonColor = MidsContext.Character.IsHero()
                            ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                            : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain;
                        llControl.UpdateTextColors(ListLabelV3.LLItemState.Selected,
                            MidsContext.Character.IsHero()
                                ? MidsContext.Config.RtFont.ColorPowerTakenHero
                                : MidsContext.Config.RtFont.ColorPowerTakenVillain);
                        llControl.UpdateTextColors(ListLabelV3.LLItemState.SelectedDisabled,
                            MidsContext.Character.IsHero()
                                ? MidsContext.Config.RtFont.ColorPowerTakenDarkHero
                                : MidsContext.Config.RtFont.ColorPowerTakenDarkVillain);
                        llControl.HoverColor = MidsContext.Character.IsHero()
                            ? MidsContext.Config.RtFont.ColorPowerHighlightHero
                            : MidsContext.Config.RtFont.ColorPowerHighlightVillain;
                    }

                    var num1 = fIncarnate.LLLeft.Items.Length - 1;
                    for (var index = 0; index <= num1; ++index)
                        fIncarnate.LLLeft.Items[index].Bold = MidsContext.Config.RtFont.PairedBold;
                    var num2 = fIncarnate.LLRight.Items.Length - 1;
                    for (var index = 0; index <= num2; ++index)
                        fIncarnate.LLRight.Items[index].Bold = MidsContext.Config.RtFont.PairedBold;
                    fIncarnate.LLLeft.SuspendRedraw = false;
                    fIncarnate.LLRight.SuspendRedraw = false;
                    fIncarnate.LLLeft.Refresh();
                    fIncarnate.LLRight.Refresh();
                }
            }

            if (fTemp != null)
            {
                var tempPowers = fTemp;
                if (tempPowers.Visible)
                    tempPowers.UpdateFonts(llPrimary.Font);
            }

            if (fAccolade == null)
                return;
            var accoladePowers = fAccolade;
            if (accoladePowers.Visible)
                accoladePowers.UpdateFonts(llPrimary.Font);

            if (fPrestige == null)
                return;
            var prestigePowers = fPrestige;
            if (prestigePowers.Visible)
                prestigePowers.UpdateFonts(llPrimary.Font);
        }

        private void UpdatePowerList(ListLabelV3 llPower)
        {
            llPower.SuspendRedraw = true;
            if (llPower.Items.Length == 0)
            {
                llPower.AddItem(new ListLabelV3.ListLabelItemV3("Nothing", ListLabelV3.LLItemState.Disabled, -1, -1, -1, ""));
            }
            
            foreach (var listLabelItemV3 in llPower.Items)
            {
                if (!((listLabelItemV3.nIDSet > -1) & (listLabelItemV3.IDXPower > -1))) continue;
                var message = "";
                listLabelItemV3.ItemState = MainModule.MidsController.Toon.PowerState(listLabelItemV3.nIDPower, ref message);
                listLabelItemV3.Italic = listLabelItemV3.ItemState == ListLabelV3.LLItemState.Invalid;
                listLabelItemV3.Bold = MidsContext.Config.RtFont.PairedBold;
            }

            llPower.SuspendRedraw = false;
        }

        private void UpdatePowerLists()
        {
            var noPrimary = false;
            if (llPrimary.Items.Length == 0)
                noPrimary = true;
            else if (llPrimary.Items[llPrimary.Items.Length - 1].nIDSet != (MidsContext.Character.Powersets[0] == null ? -1 : MidsContext.Character.Powersets[0].nID))
                noPrimary = true;
            if (llSecondary.Items.Length == 0)
                noPrimary = true;
            else if (llSecondary.Items[llSecondary.Items.Length - 1].nIDSet != (MidsContext.Character.Powersets[1] == null ? -1 : MidsContext.Character.Powersets[1].nID))
                noPrimary = true;
            var noAncillary = false;
            if (llAncillary.Items.Length == 0 || MidsContext.Character.Powersets[7] == null)
                noAncillary = true;
            else if (llAncillary.Items[llAncillary.Items.Length - 1].nIDSet != MidsContext.Character.Powersets[7].nID)
                noAncillary = true;
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

        private void tsToggleCheckModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MidsContext.EnhCheckMode = !MidsContext.EnhCheckMode;
            if (fRecipe != null && fRecipe.Visible)
            {
                fRecipe.UpdateData();
            }

            enhCheckMode.Visible = MidsContext.EnhCheckMode;

            //FloatBuildSalvageHud(MidsContext.EnhCheckMode);
            DoRedraw();
        }

        public void UpdateEnhCheckModeToolStrip()
        {
            ToggleCheckModeToolStripMenuItem.Checked = MidsContext.EnhCheckMode;
        }

        public bool IsSalvageHudVisible()
        {
            return fSalvageHud != null && fSalvageHud.Visible;
        }

        public void SetSalvageHudOnCloseExecution(bool s)
        {
            if (IsSalvageHudVisible())
            {
                fSalvageHud.SetOnCloseUpdatesExecution(s);
            }
        }
    
        private void GameImport(string buildString)
        {
            try
            {
                var importHandle = new ImportFromBuildsave(buildString);
                var listPowers = importHandle.Parse();

                if (listPowers == null) return;

                InjectBuild(buildString, listPowers, importHandle.GetPowersets(), importHandle.GetCharacterInfo());
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\r\n\r\n{e.StackTrace}");
            }
        }

        private void BuildRecover(string buildString)
        {
            try
            {
                var importHandle = new PlainTextParser(buildString);
                var listPowers = importHandle.Parse();

                if (listPowers == null) return;

                InjectBuild(buildString, listPowers, importHandle.GetPowersets(), importHandle.GetCharacterInfo());
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\r\n\r\n{e.StackTrace}");
            }
        }

        private void InjectBuild(string buildFile, List<PowerEntry> listPowers, UniqueList<string> listPowersets, RawCharacterInfo characterInfo)
        {
            // Need to pad pools powers list so there are 4
            // So epic pools doesn't end up shown as a regular pool...
            ImportBase.PadPowerPools(ref listPowersets);
            ImportBase.FilterVEATPools(ref listPowersets);
            ImportBase.FixUndetectedPowersets(ref listPowersets);
            ImportBase.FinalizePowersetsList(ref listPowersets, listPowers);

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
                    if (powerEntryList[k].PowerSet.FullName.Contains("Inherent"))
                    {
                        continue;
                    }

                    PowerPickedNoRedraw(powerEntryList[k].NIDPowerset, powerEntryList[k].NIDPower);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}");
            }

            var sl = new SlotLevelQueue();
            try
            {
                foreach (var pe in MidsContext.Character.CurrentBuild.Powers)
                {
                    if (pe.Power == null) continue; // Not picked power will be in the list, but not instanciated!
                    var pList = powerEntryList.Where(e => pe.Power.FullName == e.Power.FullName).ToArray();
                    if (pList.Length == 0) continue;
                    if (!DatabaseAPI.Database.Power[pe.NIDPower].Slottable) continue;

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
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }

            FixStatIncludes();
            FileModified = false;
            MidsContext.Character.Lock();
            MidsContext.Character.PoolShuffle();
            I9Gfx.OriginIndex = MidsContext.Character.Origin;
            MidsContext.Config.LastFileName = buildFile;
            LastFileName = buildFile;
            SetTitleBar();

            var idx = -1;
            if (MidsContext.Config.BuildMode is Enums.dmModes.Normal or Enums.dmModes.Respec)
            {
                idx = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex(MainModule.MidsController.Toon
                    .RequestedLevel);
                if (idx < 0) idx = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex();
            }
            else if (DatabaseAPI.Database.Levels[MidsContext.Character.Level].LevelType() == Enums.dmItem.Power)
            {
                idx = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex();
                drawing.HighlightSlot(-1);
            }

            if (MainModule.MidsController.Toon.Complete) drawing.HighlightSlot(-1);

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
            I9Gfx.OriginIndex = MidsContext.Character.Origin;
            MidsContext.Character.Validate();
            var powerEntryArray = DeepCopyPowerList();
            RearrangeAllSlotsInBuild(powerEntryArray, true);
            ShallowCopyPowerList(powerEntryArray);
            PowerModified(false);
            DoRedraw();

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
            I9Gfx.OriginIndex = MidsContext.Character.Origin;

            MidsContext.Character.Validate();
            MidsContext.Config.LastFileName = buildFile;
            */
        }

        #region "fields"

        private Rectangle ActivePopupBounds;

        private bool DataViewLocked;

        // dragdrop scenario action
        private readonly short[] dragdropScenarioAction;
        //private ExtendedBitmap? dmBuffer;
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
        private clsDrawX? drawing;
        public clsDrawX? DrawX => drawing;
        private int dvLastEnh;
        private bool dvLastNoLev;
        private int dvLastPower;
        private int EnhancingPower;
        private int EnhancingSlot;
        private readonly bool EnhPickerActive;
        private frmAccolade? fAccolade;
        private frmData fData;
        private frmCompare fGraphCompare;
        private frmStats fGraphStats;
        private bool FileModified { get; set; }
        private frmIncarnate? fIncarnate;
        private frmMMPowers? fMMPets;
        private frmPrestige? fPrestige;
        private bool FlipActive;
        private PowerEntry FlipGP;
        private readonly int FlipInterval;
        private int FlipPowerID;
        private int[] FlipSlotState;
        private readonly int FlipStepDelay;
        private readonly int FlipSteps;
        private frmFloatingStats FloatingDataForm;
        private frmMiniList fMini;
        private frmRecipeViewer fRecipe;
        private frmDPSCalc fDPSCalc;
        private frmSetFind fSetFinder;
        private frmSetViewer fSets;
        private frmTemp? fTemp;
        private frmTotalsV2? fTotals2;
        private frmTotals? fTotals;
        private frmBuildSalvageHud fSalvageHud;
        private bool HasSentBack;
        private bool HasSentForwards;
        private bool LastClickPlacedSlot;
        private int LastEnhIndex;
        private I9Slot LastEnhPlaced;
        private string LastFileName;
        private int LastIndex;
        private FormWindowState LastState;
        private DataView myDataView;
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