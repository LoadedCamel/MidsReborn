using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Base;
using Base.Data_Classes;
using Base.Display;
using Base.Master_Classes;
using Hero_Designer.Forms;
using Hero_Designer.My;
using midsControls;
using Timer = System.Windows.Forms.Timer;
using HeroViewer;
using HeroViewer.Base;


namespace Hero_Designer
{
    public partial class frmMain : Form
    {
        #region "fields"

        Rectangle ActivePopupBounds;

        bool DataViewLocked;

        // dragdrop scenario action
        readonly short[] dragdropScenarioAction;
        ExtendedBitmap dmBuffer;
        bool DoneDblClick;
        int dragFinishPower;
        int dragFinishSlot;
        Rectangle dragRect;
        int dragStartPower;
        int dragStartSlot;
        int dragStartX;
        int dragStartY;
        int dragXOffset;
        int dragYOffset;
        clsDrawX drawing;
        int dvLastEnh;
        bool dvLastNoLev;
        int dvLastPower;
        int EnhancingPower;
        int EnhancingSlot;
        readonly bool EnhPickerActive;
        frmAccolade fAccolade;
        frmData fData;
        frmCompare fGraphCompare;
        frmStats fGraphStats;
        bool FileModified { get; set; }
        frmIncarnate fIncarnate;
        frmMMPowers fMMPets;
        bool FlipActive;
        PowerEntry FlipGP;
        readonly int FlipInterval;
        int FlipPowerID;
        int[] FlipSlotState;
        readonly int FlipStepDelay;
        readonly int FlipSteps;
        frmFloatingStats FloatingDataForm;
        frmMiniList fMini;
        frmRecipeViewer fRecipe;
        frmDPSCalc fDPSCalc;
        frmSetFind fSetFinder;
        frmSetViewer fSets;
        frmTemp fTemp;
        frmTotals fTotals;
        bool HasSentBack;
        bool HasSentForwards;
        bool LastClickPlacedSlot;
        int LastEnhIndex;
        I9Slot LastEnhPlaced;
        string LastFileName;
        int LastIndex;
        FormWindowState LastState;
        DataView myDataView;
        bool NoResizeEvent;
        bool NoUpdate;
        Rectangle oldDragRect;
        int PickerHID;
        bool PopUpVisible;
        bool top_fData;
        bool top_fGraphCompare;
        bool top_fGraphStats;
        bool top_fRecipe;
        bool top_fSetFinder;
        bool top_fSets;
        bool top_fTotals;
        int xCursorOffset;
        int yCursorOffset;
        //RawEnhData RawEnhData = new RawEnhData();
        //RawPowerData RawPowerData = new RawPowerData();

        #endregion

        public int GetPrimaryBottom() => cbPrimary.Top + cbPrimary.Height;

        internal OpenFileDialog DlgOpen;

        internal SaveFileDialog DlgSave;

        public bool petWindowFlag { get; set; }

        public List<string> MMPets { get; set; } = new List<string>();

        ComboBoxT<string> GetCbOrigin() => new ComboBoxT<string>(cbOrigin);

        // store the instance for reuse, as these things are called per draw/redraw
        Lazy<ComboBoxT<Archetype>> CbtAT => new Lazy<ComboBoxT<Archetype>>(() => new ComboBoxT<Archetype>(cbAT));
        Lazy<ComboBoxT<string>> CbtPrimary => new Lazy<ComboBoxT<string>>(() => new ComboBoxT<string>(cbPrimary));
        Lazy<ComboBoxT<string>> CbtSecondary => new Lazy<ComboBoxT<string>>(() => new ComboBoxT<string>(cbSecondary));
        Lazy<ComboBoxT<string>> CbtAncillary => new Lazy<ComboBoxT<string>>(() => new ComboBoxT<string>(cbAncillary));

        Lazy<ComboBoxT<string>> CbtPool0 => new Lazy<ComboBoxT<string>>(() => new ComboBoxT<string>(cbPool0));
        Lazy<ComboBoxT<string>> CbtPool1 => new Lazy<ComboBoxT<string>>(() => new ComboBoxT<string>(cbPool1));
        Lazy<ComboBoxT<string>> CbtPool2 => new Lazy<ComboBoxT<string>>(() => new ComboBoxT<string>(cbPool2));
        Lazy<ComboBoxT<string>> CbtPool3 => new Lazy<ComboBoxT<string>>(() => new ComboBoxT<string>(cbPool3));

        internal clsDrawX Drawing => drawing;

        internal bool loading;

        public frmMain()
        {
            if (!Debugger.IsAttached || !this.IsInDesignMode() ||
                !Process.GetCurrentProcess().ProcessName.ToLowerInvariant().Contains("devenv"))
            {
                ConfigData.Initialize(MyApplication.GetSerializer());
                Load += frmMain_Load;
                Closed += frmMain_Closed;
                FormClosing += frmMain_Closing;
                ResizeEnd += frmMain_Resize;
                KeyDown += frmMain_KeyDown;
                Resize += frmMain_Maximize;
                MouseWheel += frmMain_MouseWheel;
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
            }

            InitializeComponent();
            Application.EnableVisualStyles();
            foreach (var backup in Directory.GetFiles(Directory.GetCurrentDirectory(), "*.bak"))
            {
                File.Delete(backup);
            }
            if (MidsContext.Config.CheckForUpdates)
            {
                clsXMLUpdate.CheckUpdate();
            }

            //disable menus that are no longer hooked up, but probably should be hooked back up
            tsHelp.Visible = false;
            tsHelp.Enabled = false;
            //enable menus
            tsKoFi.Visible = true;
            tsKoFi.Enabled = true;
            tsPatreon.Visible = true;
            tsPatreon.Enabled = true;


            tmrGfx.Tick += tmrGfx_Tick;
            //adding events
            if (Debugger.IsAttached && this.IsInDesignMode() && Process.GetCurrentProcess().ProcessName.ToLowerInvariant().Contains("devenv"))
                return;
            dvAnchored = new DataView();
            Controls.Add(dvAnchored);
            dvAnchored.BackColor = Color.Black;
            //dvAnchored.DrawVillain = false;
            dvAnchored.Floating = false;
            dvAnchored.Font = new Font("Arial", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);

            dvAnchored.Location = new Point(16, 391);
            dvAnchored.Name = "dvAnchored";

            dvAnchored.Size = new Size(300, 347);
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
            var componentResourceManager = new ComponentResourceManager(typeof(frmMain));
            var icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            Icon = icon;
            Name = nameof(frmMain);
        }

        // [Zed 06/01/20]
        // Input: argv: string[] Command line parameters, value: argument value to look for, caseSensitive: bool, perform case (in)sensitive lookup
        // Output: bool, target value has (not) been found
        bool findCommandLineParameter(string[] argv, string value, bool caseSensitive = true)
        {
            // Only inspect first 10 arguments,
            // skip first argument (aka %0), since it is the executable path.
            for (int i = 1; i < Math.Min(10, argv.Length); i++)
            {
                if (caseSensitive)
                {
                    if (argv[i] == value) return true;
                }
                else
                {
                    if (argv[i].ToLower() == value.ToLower()) return true;
                }
            }
            return false;
        }

        // [Zed 06/01/20]
        // Input: clFilename: string, filename passed as command line argument
        // Output: Usable filename (trimmed out of quotes)
        // Note: not checked here, but a valid file name should be either a local path name (X:\...) or a UNC resource name (\\server\share\...)
        // Integrate Environment.GetCommandLineArgs().Skip(1); directly here?
        string formatFilenameFromParameter(string clFilename)
        {
            return clFilename.Replace("\"", string.Empty);
        }

        void frmMain_Load(object sender, EventArgs e)
        {
            loading = true;
            try
            {
                if (MidsContext.Config.I9.DefaultIOLevel == 27)
                    MidsContext.Config.I9.DefaultIOLevel = 49;
                int height1 = 0;
                int width1 = 0;
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
                frmLoading iFrm = new frmLoading();
                iFrm.Show();
                myDataView = dvAnchored;
                pnlGFX.BackColor = BackColor;
                NoUpdate = true;

                //AutoUpdater.ApplicationExitEvent += AutoUpdater_ApplicationExitEvent;
                //AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
                if (!this.IsInDesignMode() && !MidsContext.Config.IsInitialized)
                {
                    MidsContext.Config.CheckForUpdates = false;
                    //MessageBox.Show(("Welcome to Mids' Reborn : Hero Designer "
                    //+ MidsContext.AppVersion 
                    //+ "! Please check the Readme/Help for quick instructions.\r\n\r\nMids' Hero Designer is able to check for and download updates automatically when it starts.\r\nIt's recommended that you turn on automatic updating. Do you want to?\r\n\r\n(If you don't, you can manually check from the 'Updates' tab in the options.)"), MessageBoxButtons.YesNo | MessageBoxIcon.Question, "Welcome!") == DialogResult.Yes;
                    MidsContext.Config.DefaultSaveFolderOverride = null;
                    MidsContext.Config.CreateDefaultSaveFolder();
                    MidsContext.Config.IsInitialized = true;
                }

                string[] args = Environment.GetCommandLineArgs();
                if (findCommandLineParameter(args, "RECOVERY"))
                {
                    MessageBox.Show(
                        "As recovery mode has been invoked, you will be redirected to the download site for the most recent full install package.",
                        "Recovery Mode", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    clsXMLUpdate.DownloadFromDomain();
                    Application.Exit();
                    return;
                }

                if (findCommandLineParameter(args, "MASTERMODE=YES"))
                    MidsContext.Config.MasterMode = true;
                MainModule.MidsController.LoadData(ref iFrm);
                iFrm?.SetMessage("Setting up UI...");
                dvAnchored.VisibleSize = MidsContext.Config.DvState;
                SetTitleBar();
                var loadedFromArgs = false;
                if (args.Length > 1)
                {
                    string clFilename = formatFilenameFromParameter(args[1]).Trim();
                    if (File.Exists(clFilename) && !DoOpen(clFilename))
                    {
                        loadedFromArgs = true;
                        PowerModified(markModified: false);
                    }
                }
                bool toonLoaded = false;
                if (!MidsContext.Config.DisableLoadLastFileOnStart)
                    toonLoaded = DoOpen(MidsContext.Config.LastFileName);

                if (!toonLoaded)
                {
                    NewToon(true, false);
                    PowerModified(markModified: true);
                }

                if (!loadedFromArgs && !MidsContext.Config.DisableLoadLastFileOnStart && !toonLoaded)
                    PowerModified(markModified: true);

                dvAnchored.Init();
                cbAT.SelectedItem = MidsContext.Character.Archetype;
                lblATLocked.Location = cbAT.Location;
                lblATLocked.Size = cbAT.Size;
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
                if (Screen.PrimaryScreen.WorkingArea.Width > MidsContext.Config.LastSize.Width &
                    MidsContext.Config.LastSize.Width >= MinimumSize.Width)
                {
                    int hasMaxSize = MaximumSize.Width > 0 ? 1 : 0;
                    int hasValidLastSize = MaximumSize.Width - MidsContext.Config.LastSize.Width < 32 ? 1 : 0;
                    int hasValidBoth = hasMaxSize & hasValidLastSize;
                    int needsWidthReduction = Screen.PrimaryScreen.WorkingArea.Width > MaximumSize.Width ? 1 : 0;
                    width1 = (hasValidBoth & needsWidthReduction) != 0 ? MaximumSize.Width : MidsContext.Config.LastSize.Width;
                }
                else if (Screen.PrimaryScreen.WorkingArea.Width <= MidsContext.Config.LastSize.Width)
                {
                    width1 = Screen.PrimaryScreen.WorkingArea.Width - (Size.Width - ClientSize.Width);
                }

                if (Screen.PrimaryScreen.WorkingArea.Height > MidsContext.Config.LastSize.Height &&
                    MidsContext.Config.LastSize.Height >= MinimumSize.Height)
                    height1 = MidsContext.Config.LastSize.Height;
                else if (Screen.PrimaryScreen.WorkingArea.Height <= MidsContext.Config.LastSize.Height)
                {
                    height1 = Screen.PrimaryScreen.WorkingArea.Height - (Size.Height - ClientSize.Height);
                }
                Size = new Size(width1, height1);
                tsViewIOLevels.Checked = !MidsContext.Config.I9.HideIOLevels;
                tsViewSlotLevels.Checked = MidsContext.Config.ShowSlotLevels;
                tsViewSelected();
                tsIODefault.Text = "Default (" + (MidsContext.Config.I9.DefaultIOLevel + 1) + ")";
                SetDamageMenuCheckMarks();
                //Procat: Removed during performance optimization.
                //ReArrange(true);
                GetBestDamageValues();
                dvAnchored.SetFontData();
                DlgSave.InitialDirectory = MidsContext.Config.GetSaveFolder();
                DlgOpen.InitialDirectory = MidsContext.Config.GetSaveFolder();
                NoUpdate = false;
                tsViewSlotLevels.Checked = MidsContext.Config.ShowSlotLevels;
                ibSlotLevels.Checked = MidsContext.Config.ShowSlotLevels;
                tsViewRelative.Checked = MidsContext.Config.ShowEnhRel;
                ibPopup.Checked = !MidsContext.Config.DisableShowPopup;
                ibRecipe.Checked = MidsContext.Config.PopupRecipes;
                if (MidsContext.Config.MasterMode)
                {
                    tsAdvFreshInstall.Visible = true;
                    tsAdvResetTips.Visible = true;
                }
                Show();
                iFrm.Hide();
                iFrm.Close();
                Refresh();
                dvAnchored.SetScreenBounds(ClientRectangle);
                Point iLocation = new Point();
                ref Point local = ref iLocation;
                int left = llPrimary.Left;
                int top = llPrimary.Top;
                var size1 = llPrimary.SizeNormal;
                int height5 = size1.Height;
                int y = top + height5 + 5;
                local = new Point(left, y);
                dvAnchored.SetLocation(iLocation, true);
                PriSec_ExpandChanged(true);
                this.loading = false;
                UpdateControls(true);
                if (this.IsInDesignMode())
                    return;
                /*if (MidsContext.Config.CheckForUpdates)
                    clsXMLUpdate.CheckUpdate();*/

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occurred when loading the main form. Error: " + ex.Message + "\r\n" + ex.StackTrace, "OMIGODHAX");
                throw;
            }
            this.loading = false;
        }

        I9Picker I9Picker
        {
            get
            {
                if (i9Picker.Height <= 235)
                    i9Picker.Height = 315;
                return i9Picker;
            }
            set => i9Picker = value;
        }

        internal void ChildRequestedRedraw()
            => DoRedraw();

        void accoladeButton_ButtonClicked() => this.PowerModified(markModified: false);

        void accoladeButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks != 2)
                return;
            this.accoladeButton.Checked = false;
            if (this.fAccolade == null || this.fAccolade.IsDisposed)
            {
                IPower power = !MainModule.MidsController.Toon.IsHero() ? DatabaseAPI.Database.Power[DatabaseAPI.NidFromStaticIndexPower(3258)] : DatabaseAPI.Database.Power[DatabaseAPI.NidFromStaticIndexPower(3257)];
                List<IPower> iPowers = new List<IPower>();
                int num = power.NIDSubPower.Length - 1;
                for (int index = 0; index <= num; ++index)
                    iPowers.Add(DatabaseAPI.Database.Power[power.NIDSubPower[index]]);
                this.fAccolade = new frmAccolade(this, iPowers) { Text = "Accolades" };
            }
            if (!this.fAccolade.Visible)
                this.fAccolade.Show(this);
        }

        void AccoladesWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.accoladeButton_MouseDown(RuntimeHelpers.GetObjectValue(sender), new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0));
            this.accoladeButton.Checked = true;
        }

        static int ArchetypeIndirectToIndex(int iIndirect)
        {
            int num1 = -1;
            for (int index = 0; index <= DatabaseAPI.Database.Classes.Length - 1; ++index)
            {
                if (!DatabaseAPI.Database.Classes[index].Playable)
                    continue;
                ++num1;
                if (num1 == iIndirect)
                    return index;
            }

            return 0;
        }

        void AssemblePowerList(ListLabelV2 llPower, IPowerset Powerset)
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
                    IPowerset powerset = DatabaseAPI.Database.Powersets[Powerset.nIDTrunkSet];
                    var iItem1 = new ListLabelV2.ListLabelItemV2(powerset.DisplayName, ListLabelV2.LLItemState.Heading, Powerset.nIDTrunkSet,
                        -1, -1, "", ListLabelV2.LLFontFlags.Bold, ListLabelV2.LLTextAlign.Center);
                    llPower.AddItem(iItem1);
                    for (int iIDXPower = 0; iIDXPower <= powerset.Powers.Length - 1; ++iIDXPower)
                    {
                        if (powerset.Powers[iIDXPower].Level <= 0)
                            continue;
                        message = "";
                        var iItem2 = new ListLabelV2.ListLabelItemV2(powerset.Powers[iIDXPower].DisplayName,
                            MainModule.MidsController.Toon.PowerState(powerset.Powers[iIDXPower].PowerIndex,
                                ref message), Powerset.nIDTrunkSet, iIDXPower, powerset.Powers[iIDXPower].PowerIndex, "",
                            ListLabelV2.LLFontFlags.Bold)
                        {
                            Bold = MidsContext.Config.RtFont.PairedBold
                        };
                        if (iItem2.ItemState == ListLabelV2.LLItemState.Invalid)
                            iItem2.Italic = true;
                        llPower.AddItem(iItem2);
                    }

                    var iItem = new ListLabelV2.ListLabelItemV2(Powerset.DisplayName, ListLabelV2.LLItemState.Heading, Powerset.nID, -1, -1,
                        "", ListLabelV2.LLFontFlags.Bold, ListLabelV2.LLTextAlign.Center);
                    llPower.AddItem(iItem);
                }

                if (Powerset.Powers != null)
                    for (int iIDXPower = 0; iIDXPower <= Powerset.Powers.Length - 1; ++iIDXPower)
                    {
                        if ((Powerset.Powers[iIDXPower].Level <= 0) || (!Powerset.Powers[iIDXPower].AllowedForClass(MidsContext.Character.Archetype.Idx)))
                            continue;
                        message = "";
                        var targetPs =
                            MainModule.MidsController.Toon.PowerState(Powerset.Powers[iIDXPower].PowerIndex,
                                ref message);
                        var power = Powerset.Powers[iIDXPower];
                        var iItem = new ListLabelV2.ListLabelItemV2(
                            Powerset.Powers[iIDXPower].DisplayName,
                            targetPs,
                            Powerset.nID,
                            iIDXPower,
                            power.PowerIndex, "", ListLabelV2.LLFontFlags.Bold)
                        {
                            Bold = MidsContext.Config.RtFont.PairedBold
                        };
                        if (iItem.ItemState == ListLabelV2.LLItemState.Invalid)
                            iItem.Italic = true;
                        llPower.AddItem(iItem);
                    }

                llPower.SuspendRedraw = false;
            }
        }

        void AutoArrangeAllSlotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PowerEntry[] powerEntryArray = DeepCopyPowerList();
            RearrangeAllSlotsInBuild(powerEntryArray, true);
            ShallowCopyPowerList(powerEntryArray);
            // unless they set more than just slotting order, don't force the save flag
            PowerModified(markModified: false);
            DoRedraw();
        }

        void cbAncillary_DrawItem(object sender, DrawItemEventArgs e)
            => cbDrawItem(CbtAncillary.Value, Enums.ePowerSetType.Ancillary, e);

        void cbAncillary_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[7] == null)
                return;
            string ExtraString = "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[7].nID, MidsContext.Character.Archetype.Idx, cbAncillary.Bounds, ExtraString);
        }

        void cbAncillery_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            ChangeSets();
            UpdatePowerLists();
        }

        void cbAT_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            e.Graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
            e.DrawBackground();
            SolidBrush solidBrush = new SolidBrush(SystemColors.ControlText);
            if (e.Index > -1)
            {
                var cbAT = new ComboBoxT<Archetype>(this.cbAT);
                int index = ArchetypeIndirectToIndex(e.Index);
                RectangleF destRect = new RectangleF(e.Bounds.X + 1, e.Bounds.Y, 16f, 16f);
                RectangleF srcRect = new RectangleF(index * 16, 0.0f, 16f, 16f);
                e.Graphics.DrawImage(I9Gfx.Archetypes.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
                StringFormat format = new StringFormat(StringFormatFlags.NoWrap)
                {
                    LineAlignment = StringAlignment.Center
                };
                RectangleF layoutRectangle = new RectangleF(e.Bounds.X + destRect.X + destRect.Width, e.Bounds.Y,
                    e.Bounds.Width - (destRect.X + destRect.Width), e.Bounds.Height);
                e.Graphics.DrawString(cbAT[e.Index].DisplayName, e.Font, solidBrush, layoutRectangle, format);
            }

            e.DrawFocusRectangle();
        }


        void cbAT_MouseLeave(object sender, EventArgs e) => HidePopup();

        void cbAT_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || cbAT.SelectedIndex < 0)
                return;
            ShowPopup(-1, CbtAT.Value.SelectedItem.Idx, cbAT.Bounds);
        }

        void cbAT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            NewToon(false);
            SetFormHeight();
            SetAncilPoolHeight();
            GetBestDamageValues();
        }

        static void cbDrawItem(
            ComboBoxT<string> target,
            Enums.ePowerSetType SetType,
            DrawItemEventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            e.Graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
            e.DrawBackground();
            SolidBrush solidBrush = new SolidBrush(Color.Black);
            IPowerset[] powersetIndexes = DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, SetType);
            if (e.Index > -1 & e.Index < powersetIndexes.Length)
            {
                int nId = powersetIndexes[e.Index].nID;
                RectangleF destRect = new RectangleF();
                ref RectangleF local1 = ref destRect;
                local1 = new RectangleF(e.Bounds.X + 1, e.Bounds.Y, 16f, 16f);
                RectangleF srcRect = new RectangleF(nId * 16, 0.0f, 16f, 16f);
                if ((e.State & DrawItemState.ComboBoxEdit) > DrawItemState.None)
                {
                    if (e.Graphics.MeasureString(target[e.Index], e.Font).Width <= e.Bounds.Width - 10)
                        e.Graphics.DrawImage(I9Gfx.Powersets.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
                    else
                        destRect.Width = 0.0f;
                }
                else
                    e.Graphics.DrawImage(I9Gfx.Powersets.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);

                StringFormat format = new StringFormat(StringFormatFlags.NoWrap)
                {
                    LineAlignment = StringAlignment.Center
                };
                var layout = new RectangleF(e.Bounds.X + destRect.X + destRect.Width, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
                e.Graphics.DrawString(target[e.Index], e.Font, solidBrush, layout, format);
            }

            e.DrawFocusRectangle();
        }

        void cbOrigin_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            e.Graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
            e.DrawBackground();
            //SolidBrush solidBrush = new SolidBrush(SystemColors.ControlText);
            SolidBrush solidBrush = new SolidBrush(Color.Black);
            if (e.Index > -1)
            {
                var cmbOrigin = GetCbOrigin();
                RectangleF destRect = new RectangleF(e.Bounds.X + 1, e.Bounds.Y, 16f, 16f);
                RectangleF srcRect = new RectangleF(DatabaseAPI.GetOriginIDByName(cmbOrigin[e.Index]) * 16, 0.0f, 16f, 16f);
                e.Graphics.DrawImage(I9Gfx.Origins.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
                StringFormat format = new StringFormat(StringFormatFlags.NoWrap)
                {
                    LineAlignment = StringAlignment.Center
                };
                RectangleF layoutRectangle = new RectangleF(e.Bounds.X + destRect.X + destRect.Width, e.Bounds.Y,
                    e.Bounds.Width - (destRect.X + destRect.Width), e.Bounds.Height);
                e.Graphics.DrawString(cmbOrigin[e.Index], e.Font, solidBrush, layoutRectangle, format);
            }

            e.DrawFocusRectangle();
        }

        void cbOrigin_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            MidsContext.Character.Origin = cbOrigin.SelectedIndex;
            I9Gfx.SetOrigin(cbOrigin.SelectedItem.ToStringOrNull());
            if (drawing != null)
                DoRedraw();
            DisplayName();
        }

        void cbPool0_DrawItem(object sender, DrawItemEventArgs e)
            => cbDrawItem(CbtPool0.Value, Enums.ePowerSetType.Pool, e);

        void cbPool0_MouseLeave(object sender, EventArgs e) => HidePopup();

        void cbPool0_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[3] == null)
                return;
            string ExtraString = "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[3].nID, MidsContext.Character.Archetype.Idx, cbPool0.Bounds, ExtraString);
        }

        void cbPool0_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            ChangeSets();
            UpdatePowerLists();
        }

        void cbPool1_DrawItem(object sender, DrawItemEventArgs e)
            => cbDrawItem(CbtPool1.Value, Enums.ePowerSetType.Pool, e);

        void cbPool1_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[4] == null)
                return;
            ShowPopup(MidsContext.Character.Powersets[4].nID, MidsContext.Character.Archetype.Idx, cbPool1.Bounds,
                "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.");
        }

        void cbPool1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            ChangeSets();
            UpdatePowerLists();
        }

        void cbPool2_DrawItem(object sender, DrawItemEventArgs e)
            => cbDrawItem(CbtPool2.Value, Enums.ePowerSetType.Pool, e);

        void cbPool2_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[5] == null)
                return;
            string ExtraString = "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[5].nID, MidsContext.Character.Archetype.Idx, cbPool2.Bounds, ExtraString);
        }

        void cbPool2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            ChangeSets();
            UpdatePowerLists();
        }

        void cbPool3_DrawItem(object sender, DrawItemEventArgs e)
            => cbDrawItem(CbtPool3.Value, Enums.ePowerSetType.Pool, e);

        void cbPool3_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[6] == null)
                return;
            string ExtraString = "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[6].nID, MidsContext.Character.Archetype.Idx, cbPool3.Bounds, ExtraString);
        }

        void cbPool3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            ChangeSets();
            UpdatePowerLists();
        }

        void cbPrimary_DrawItem(object sender, DrawItemEventArgs e)
            => cbDrawItem(CbtPrimary.Value, Enums.ePowerSetType.Primary, e);

        void cbPrimary_MouseLeave(object sender, EventArgs e) => HidePopup();

        void cbPrimary_MouseMove(object sender, MouseEventArgs e)
        {
            if (MidsContext.Character == null || MidsContext.Character.Archetype == null || cbPrimary.SelectedIndex < 0)
                return;
            string ExtraString =
                "This is your primary powerset. This powerset can be changed after a build has been started, and any placed powers will be swapped out for those in the new set.";
            ShowPopup(
                DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, Enums.ePowerSetType.Primary)[cbPrimary.SelectedIndex].nID,
                MidsContext.Character.Archetype.Idx, cbPrimary.Bounds, ExtraString);
        }

        void cbPrimary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            ChangeSets();
            UpdatePowerLists();
        }

        void cbSecondary_DrawItem(object sender, DrawItemEventArgs e)
            => cbDrawItem(CbtSecondary.Value, Enums.ePowerSetType.Secondary, e);

        void cbSecondary_MouseLeave(object sender, EventArgs e) => HidePopup();

        void cbSecondary_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Archetype.Idx < 0 || cbSecondary.SelectedIndex < 0)
                return;
            string ExtraString = MidsContext.Character.Powersets[0].nIDLinkSecondary <= -1
                ? "This is your secondary powerset. This powerset can be changed after a build has been started, and any placed powers will be swapped out for those in the new set."
                : "This is your secondary powerset. This powerset is linked to your primary set and cannot be changed independantly. However, it can be changed by selecting a different primary powerset.";
            ShowPopup(
                DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, Enums.ePowerSetType.Secondary)[cbSecondary.SelectedIndex].nID,
                MidsContext.Character.Archetype.Idx, cbSecondary.Bounds, ExtraString);
        }

        void cbSecondary_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            ChangeSets();
            UpdatePowerLists();
        }

        void ChangeSets()
        {
            MainUILogic.ChangeSets(MainModule.MidsController.Toon, MidsContext.Character,
                primaryIndex: cbPrimary.SelectedIndex,
                secondaryIndex: cbSecondary.SelectedIndex,
                pool0Index: cbPool0.SelectedIndex,
                pool1Index: cbPool1.SelectedIndex,
                pool2Index: cbPool2.SelectedIndex,
                pool3Index: cbPool3.SelectedIndex,
                ancillaryIndex: cbAncillary.SelectedIndex,
                getPowerSets: DatabaseAPI.GetPowersetIndexes,
                enableSecondary: () => cbSecondary.Enabled = true
            );
            DataViewLocked = false;
            ActiveControl = llPrimary;
            PowerModified(markModified: true);
            FloatUpdate(true);
            GetBestDamageValues();
        }

        void clearPower(PowerEntry[] tp, int pwrIdx)
        {
            tp[pwrIdx].Slots = Array.Empty<SlotEntry>();
            tp[pwrIdx].SubPowers = Array.Empty<PowerSubEntry>();
            tp[pwrIdx].IDXPower = -1;
            tp[pwrIdx].NIDPower = -1;
            tp[pwrIdx].NIDPowerset = -1;
            tp[pwrIdx].Tag = false;
            tp[pwrIdx].StatInclude = false;
        }

        internal bool CloseCommand()
        {
            if (MainModule.MidsController.Toon == null)
            {
                return false;
            }

            if (!(MainModule.MidsController.Toon.Locked & FileModified))
                return false;
            FloatTop(false);
            var msgBoxResult = MessageBox.Show("Do you wish to save your hero/villain data before quitting?", "Question",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
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

        bool ComboCheckAT(Archetype[] playableClasses)
        {
            var cbtAT = CbtAT.Value;
            if (cbtAT.Count != playableClasses.Length)
            {
                return true;
            }

            int num = playableClasses.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (cbtAT[index].Idx != playableClasses[index].Idx)
                    return true;
            }

            return false;
        }

        bool ComboCheckOrigin()
        {
            var cbtOrigin = GetCbOrigin();
            if (cbtOrigin.Count != MidsContext.Character.Archetype.Origin.Length)
            {
                return true;
            }

            if (cbtOrigin.Count > 1)
                return false;
            int num = MidsContext.Character.Archetype.Origin.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (cbtOrigin[index] != MidsContext.Character.Archetype.Origin[index])
                    return true;
            }

            return false;
        }

        static void ComboCheckPool(ComboBoxT<string> iCB, Enums.ePowerSetType iSetType)
        {
            string[] powersetNames = DatabaseAPI.GetPowersetNames(MidsContext.Character.Archetype.Idx, iSetType);
            bool needsComboUpdate = iCB.Items.Count != powersetNames.Length || !iCB.Items.SequenceEqual(powersetNames);
            if (!needsComboUpdate)
                return;
            iCB.BeginUpdate();
            iCB.Clear();
            iCB.AddRange(powersetNames);
            iCB.EndUpdate();
        }

        static void ComboCheckPS(
            ComboBoxT<string> iCB,
            Enums.PowersetType iSetID,
            Enums.ePowerSetType iSetType)
        {
            string[] powersetNames = DatabaseAPI.GetPowersetNames(MidsContext.Character.Archetype.Idx, iSetType);
            var needsComboUpdate = iCB.Items.Count != powersetNames.Length || !iCB.Items.SequenceEqual(powersetNames);
            if (needsComboUpdate)
            {
                iCB.BeginUpdate();
                iCB.Clear();
                iCB.AddRange(powersetNames);
                iCB.EndUpdate();
            }

            IPowerset[] powersetIndexes = DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, iSetType);
            iCB.SelectedIndex = DatabaseAPI.ToDisplayIndex(MidsContext.Character.Powersets[(int)iSetID], powersetIndexes);
        }

        void command_ForumImport()
        {
            if (MainModule.MidsController.Toon.Locked & FileModified)
            {
                FloatTop(false);
                var msgBoxResult = MessageBox.Show("Current hero/villain data will be discarded, are you sure?", "Question",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                FloatTop(true);
                if (msgBoxResult == DialogResult.No)
                    return;
            }

            FloatTop(false);
            FileModified = false;
            bool loaded = false;
            if (MessageBox.Show("Copy the build data on the forum to the clipboard. When that's done, click on OK.", "Standing By",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK)
                return;
            string str = Clipboard.GetDataObject()?.GetData("System.String", true).ToString();
            NewToon();
            try
            {
                if (str != null && str.Length < 1)
                {
                    int num = (int)MessageBox.Show(
                        "No data. Please check that you copied the build data from the forum correctly and that it's a valid format.",
                        "Forum Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (str != null && (str.Contains("MxDz") || str.Contains("MxDu")))
                    {
                        Stream mStream = new MemoryStream(new ASCIIEncoding().GetBytes(str));
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
                        PowerModified(markModified: true);
                        UpdateControls(true);
                        SetFormHeight();
                    }
                    else
                    {
                        NewToon();
                        myDataView.Clear();
                        PowerModified(markModified: true);
                    }

                    GetBestDamageValues();
                    if (drawing != null)
                        DoRedraw();
                    UpdateColours();
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

        void command_New()
        {
            if (MainModule.MidsController.Toon.Locked & FileModified)
            {
                FloatTop(false);
                DialogResult msgBoxResult = MessageBox.Show("Current hero/villain data will be discarded, are you sure?", "Question",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                FloatTop(true);
                if (msgBoxResult == DialogResult.No)
                    return;
            }

            DataViewLocked = false;
            NewToon(false);
            MidsContext.Config.LastFileName = "";
            LastFileName = "";
            PowerModified(markModified: false);
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

        static PowerEntry[] DeepCopyPowerList()
            => MidsContext.Character.CurrentBuild.Powers.Select(x => (PowerEntry)x.Clone()).ToArray();

        Rectangle Dilate(Rectangle irect, int iAdd)
        {
            irect.X -= iAdd;
            irect.Y -= iAdd;
            irect.Height += iAdd * 2;
            irect.Width += iAdd * 2;
            return irect;
        }

        void DisplayFormatChanged()
        {
            GetBestDamageValues();
            RefreshInfo();
        }

        void DisplayName()
        {
            string str1 = "";
            string str2 = "";
            var ch = MidsContext.Character;
            int level = ch.Level;
            if (!(ch.CurrentBuild.TotalSlotsAvailable - ch.CurrentBuild.SlotsPlaced < 1 &
                  ch.CurrentBuild.LastPower + 1 - ch.CurrentBuild.PowersPlaced < 1) && ch.Level > 0)
                str1 = " (Placing " + (ch.Level + 1) + ")";
            SetTitleBar(MainModule.MidsController.Toon.IsHero());
            string str3 = ch.Name + ": ";
            if (MidsContext.Config.BuildMode == Enums.dmModes.LevelUp & str1 != "")
                str3 = str3 + "Level " + level + str1 + " ";
            string str4 = str3 + ch.Archetype.Origin[ch.Origin] + " " + ch.Archetype.DisplayName;
            if (MainModule.MidsController.Toon.Locked)
                str4 = str4 + " (" + ch.Powersets[0].DisplayName + " / " + ch.Powersets[1].DisplayName + ")" + str2;
            if (MidsContext.Config.ExempLow < MidsContext.Config.ExempHigh)
                str4 = str4 + " - Exemped from " + MidsContext.Config.ExempHigh + " to " + MidsContext.Config.ExempLow;
            lblHero.Text = str4;
            if (txtName.Text == ch.Name)
                return;
            txtName.Text = ch.Name;
        }

        void doFlipStep()
        {
            if (!FlipActive)
                return;
            Point point1 = new Point();
            Build currentBuild = MidsContext.Character.CurrentBuild;
            PowerEntry power = currentBuild.Powers[FlipPowerID];
            Point point2 = drawing.DrawPowerSlot(ref power);
            int index = -1;
            int Enh1 = -1;
            int Enh2 = -1;
            I9Slot i9Slot1 = null;
            I9Slot i9Slot2 = null;
            ImageAttributes recolourIa = clsDrawX.GetRecolourIa(MainModule.MidsController.Toon.IsHero());
            SolidBrush solidBrush = new SolidBrush(Color.FromArgb(160, 0, 0, 0));
            int num1 = FlipSlotState.Length - 1;
            Rectangle rectangle1;
            for (int i = 0; i <= num1; ++i)
            {
                point1.X = (int)Math.Round(point2.X + (drawing.SzPower.Width - drawing.szSlot.Width * 6) / 2.0);
                point1.Y = point2.Y + 18;
                ++FlipSlotState[i];
                float num2 = 1f;
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
                    float num3 = FlipSlotState[i] / (FlipSteps / 2f);
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
                Rectangle rectangle2 = new Rectangle((int)Math.Round(rectangle1.X + (30.0 - 30.0 * num2) / 2.0), rectangle1.Y,
                    (int)Math.Round(30.0 * num2), 30);
                rectangle2 = drawing.ScaleDown(rectangle2);
                rectangle1 = drawing.ScaleDown(rectangle1);
                if (index > -1)
                {
                    Graphics graphics = drawing.bxBuffer.Graphics;
                    if (i9Slot1 != null)
                        I9Gfx.DrawFlippingEnhancement(ref graphics, rectangle1, num2,
                            DatabaseAPI.Database.Enhancements[index].ImageIdx,
                            I9Gfx.ToGfxGrade(DatabaseAPI.Database.Enhancements[index].TypeID, i9Slot1.Grade));
                }
                else
                    drawing.bxBuffer.Graphics.DrawImage(I9Gfx.EnhTypes.Bitmap, rectangle2, 0, 0, 30, 30, GraphicsUnit.Pixel, recolourIa);

                if (MidsContext.Config.CalcEnhLevel == Enums.eEnhRelative.None | slot.Level >= MidsContext.Config.ForceLevel |
                    drawing.InterfaceMode == Enums.eInterfaceMode.PowerToggle & !powerEntry.StatInclude)
                {
                    rectangle2.Inflate(1, 1);
                    drawing.bxBuffer.Graphics.FillEllipse(solidBrush, rectangle2);
                }

                if (!(myDataView == null | i9Slot1 == null | i9Slot2 == null))
                    myDataView.FlipStage(i, Enh1, Enh2, num2, powerEntry.NIDPower, i9Slot1.Grade, i9Slot2.Grade);
            }

            rectangle1 = new Rectangle(point1.X - 1, point1.Y - 1, drawing.SzPower.Width + 1, drawing.szSlot.Height + 1);
            drawing.Refresh(drawing.ScaleDown(rectangle1));
            if (FlipSlotState[FlipSlotState.Length - 1] >= FlipSteps)
                EndFlip();
        }

        bool DoOpen(string fName)
        {
            if (!File.Exists(fName))
                return false;
            DataViewLocked = false;
            NewToon(true, true);
            Stream mStream = null;
            if (fName.Contains(".txt"))
                GameImport(fName);
            else if (!MainModule.MidsController.Toon.Load(fName, ref mStream))
            {
                NewToon();
                LastFileName = "";
                MidsContext.Config.LastFileName = "";
            }
            else
            {
                LastFileName = fName;
                if (!fName.EndsWith("mids_build.mxd"))
                    MidsContext.Config.LastFileName = fName;
            }

            FileModified = false;
            drawing.Highlight = -1;
            if (MidsContext.Character.Archetype.DisplayName != "Mastermind")
            {
                petsButton.Visible = false;
                petsButton.Enabled = false;
            }
            else
            {
                petsButton.Visible = true;
                petsButton.Enabled = true;
            }
            NewDraw();
            UpdateControls();
            SetFormHeight();
            myDataView.Clear();
            MidsContext.Character.ResetLevel();
            PowerModified(markModified: false);
            UpdateControls(true);
            SetTitleBar();
            Application.DoEvents();
            GetBestDamageValues();
            //UpdateColours();
            FloatUpdate(true);
            return true;
        }

        void DoRedraw()
        {
            if (drawing == null) return;
            NoResizeEvent = true;
            int width = pnlGFXFlow.Width;
            double scale = 1.0;
            Size drawingArea = drawing.GetDrawingArea();
            int num3 = width - 30;
            if (num3 < drawingArea.Width)
                scale = num3 / (double)drawingArea.Width;
            pnlGFX.Width = num3;
            pnlGFX.Height = (int)Math.Round(drawingArea.Height * scale);
            pnlGFX.Update();
            pnlGFXFlow.Update();
            NoResizeEvent = false;
            drawing.FullRedraw();
        }

        void DoResize()
        {
            lblHero.Width = ibRecipe.Left - 4;
            if (NoResizeEvent || drawing == null)
                return;
            int num1 = ClientSize.Width - pnlGFXFlow.Left;
            int num2 = ClientSize.Height - pnlGFXFlow.Top;
            pnlGFXFlow.Width = num1;
            pnlGFXFlow.Height = num2;
            double scale = 1.0;
            Size drawingArea = drawing.GetDrawingArea();
            int num5 = num1 - 30;
            if (num5 < drawingArea.Width)
                scale = num5 / (double)drawingArea.Width;
            int num6 = (int)Math.Round(drawingArea.Height * scale);
            pnlGFX.Width = num5;
            pnlGFX.Height = num6;
            if (drawing.Scaling && Math.Abs(scale - 1.0) < float.Epsilon || Math.Abs(scale - 1.0) > float.Epsilon)
            {
                drawing.bxBuffer.Size = pnlGFX.Size;
                Control pnlGfx = pnlGFX;
                drawing.ReInit(pnlGfx);
                pnlGFX = (PictureBox)pnlGfx;
                pnlGFX.Image = drawing.bxBuffer.Bitmap;
            }

            NoResizeEvent = true;
            drawing.SetScaling(scale < 1.0 ? pnlGFX.Size : drawing.bxBuffer.Size);
            NoResizeEvent = false;
            ReArrange(false);
        }

        bool doSave()
        {
            if (LastFileName == string.Empty)
                return doSaveAs();
            if (LastFileName.Length > 3 && LastFileName.ToUpper().EndsWith(".TXT"))
            {
                return doSaveAs();
            }

            if (!MainModule.MidsController.Toon.Save(LastFileName))
                return false;
            FileModified = false;
            return true;
        }

        bool doSaveAs()
        {
            FloatTop(false);
            if (LastFileName != string.Empty)
            {
                DlgSave.FileName = FileIO.StripPath(LastFileName);
                if (DlgSave.FileName.Length > 3 && DlgSave.FileName.ToUpper().EndsWith(".TXT"))
                    DlgSave.FileName = DlgSave.FileName.Substring(0, DlgSave.FileName.Length - 3) + DlgSave.DefaultExt;
                DlgSave.InitialDirectory = LastFileName.Substring(0, LastFileName.LastIndexOf("\\", StringComparison.Ordinal));
            }
            else if (!string.IsNullOrWhiteSpace(MidsContext.Character.Name))
            {
                if (MidsContext.Character.Archetype.ClassType == Enums.eClassType.VillainEpic)
                    DlgSave.FileName = MidsContext.Character.Name + " - Arachnos " + MidsContext.Character.Powersets[0].DisplayName
                                           .Replace(" Training", string.Empty).Replace("Arachnos ", string.Empty);
                else
                    DlgSave.FileName = MidsContext.Character.Name + " - " + MidsContext.Character.Archetype.DisplayName + " (" +
                                       MidsContext.Character.Powersets[0].DisplayName + ")";
            }
            else if (MidsContext.Character.Archetype.ClassType == Enums.eClassType.VillainEpic)
            {
                DlgSave.FileName = "Arachnos " + MidsContext.Character.Powersets[0].DisplayName.Replace(" Training", string.Empty)
                                       .Replace("Arachnos ", string.Empty);
            }
            else
            {
                DlgSave.FileName = MidsContext.Character.Archetype.DisplayName;
                SaveFileDialog dlgSave = DlgSave;
                dlgSave.FileName = dlgSave.FileName + " - " + MidsContext.Character.Powersets[0].DisplayName + " - " +
                                   MidsContext.Character.Powersets[1].DisplayName;
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

        void dvAnchored_Float()
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
            //myDataView.DrawVillain = !MainModule.MidsController.Toon.IsHero();
            dvAnchored.Visible = false;
            pnlGFX.Select();
            FloatingDataForm.Show();
            RefreshInfo();
            ReArrange(false);
            if (dvLastPower <= -1)
                return;
            Info_Power(dvLastPower, dvLastEnh, dvLastNoLev, DataViewLocked);
        }

        void dvAnchored_Move()
        {
            PriSec_ExpandChanged(true);
            ReArrange(false);
        }

        void dvAnchored_SizeChange(Size newSize, bool Compact)
        {
            ReArrange(false);
            if (!(MainModule.MidsController.IsAppInitialized & Visible))
                return;
            MidsContext.Config.DvState = dvAnchored.VisibleSize;
        }

        void dvAnchored_TabChanged(int Index)
            => SetDataViewTab(Index);

        void dvAnchored_Unlock()
        {
            DataViewLocked = false;
            if (dvLastPower <= -1)
                return;
            Info_Power(dvLastPower, dvLastEnh, dvLastNoLev, DataViewLocked);
        }

        bool EditAccoladesOrTemps(int hIDPower)
        {
            if (hIDPower <= -1 || MidsContext.Character.CurrentBuild.Powers[hIDPower].SubPowers.Length <= 0)
                return false;

            List<IPower> iPowers = new List<IPower>();
            int num1 = MidsContext.Character.CurrentBuild.Powers[hIDPower].SubPowers.Length - 1;
            for (int index = 0; index <= num1; ++index)
                iPowers.Add(DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[hIDPower].SubPowers[index].nIDPower]);
            frmAccolade frmAccolade = new frmAccolade(this, iPowers)
            {
                Text = DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[hIDPower].NIDPower]
                    .DisplayName
            };
            frmAccolade.ShowDialog(this);
            EnhancementModified();
            LastClickPlacedSlot = false;
            return true;
        }

        void EndFlip()
        {
            FlipActive = false;
            tmrGfx.Enabled = false;
            FlipPowerID = -1;
            FlipSlotState = new int[0];
            DoRedraw();
        }

        void EnhancementModified()
        {
            DoRedraw();
            RefreshInfo();
        }

        int[] fakeInitialize(params int[] nums) => nums;

        void FixPrimarySecondaryHeight()
        {
            if (dvAnchored.Visible & dvAnchored.Bounds.IntersectsWith(dvAnchored.SnapLocation))
            {
                Size size = ClientSize;
                int height = size.Height - dvAnchored.Height - cbPrimary.Top - cbPrimary.Height - 10;
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
                Size size = new Size(llPrimary.SizeNormal.Width, llPrimary.DesiredHeight);
                llPrimary.SizeNormal = size;
                size = new Size(llSecondary.SizeNormal.Width, llSecondary.DesiredHeight);
                llSecondary.SizeNormal = size;
            }
        }

        void fixStatIncludes()
        {
            if (MainModule.MidsController.Toon == null)
                return;
            for (int index = 0; index <= MidsContext.Character.CurrentBuild.Powers.Count - 1; ++index)
            {
                if (MidsContext.Character.CurrentBuild.Powers[index].PowerSet == null)
                    continue;
                switch (MidsContext.Character.CurrentBuild.Powers[index].PowerSet.DisplayName)
                {
                    case "Temporary Powers":
                        MidsContext.Character.CurrentBuild.Powers[index].StatInclude = tempPowersButton.Checked;
                        break;
                    case "Accolades":
                        MidsContext.Character.CurrentBuild.Powers[index].StatInclude = accoladeButton.Checked;
                        break;
                }
            }
        }

        internal void FloatCompareGraph(bool show)
        {
            if (show)
            {
                if (fGraphCompare == null)
                {
                    frmMain iFrm = this;
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

        void FloatData(bool show)
        {
            if (show)
            {
                if (fData == null)
                {
                    frmMain iParent = this;
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
                if (fRecipe == null)
                    fRecipe = new frmRecipeViewer(this);
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
                    frmMain iParent = this;
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
                    frmMain iParent = this;
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

        void FloatTop(bool onTop)
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

                if (fTotals != null)
                {
                    top_fTotals = fTotals.TopMost;
                    if (fTotals.TopMost)
                        fTotals.TopMost = false;
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

                if (fTotals != null && fTotals.TopMost != top_fTotals)
                {
                    fTotals.TopMost = top_fTotals;
                    if (fTotals.TopMost)
                        fTotals.BringToFront();
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

        internal void FloatTotals(bool show)
        {
            if (show)
            {
                if (fTotals == null)
                {
                    frmMain iParent = this;
                    fTotals = new frmTotals(ref iParent);
                }

                DoRedraw();
                fTotals.SetLocation();
                fTotals.Show();
                fTotals.BringToFront();
                fTotals.UpdateData();
                fTotals.Activate();
            }
            else
            {
                if (fTotals == null)
                    return;
                fTotals.Hide();
                fTotals.Dispose();
                fTotals = null;
            }
        }

        void FloatUpdate(bool newData = false)
        {
            fSets?.UpdateData();
            fGraphStats?.UpdateData(newData);
            fTotals?.UpdateData();
            fGraphCompare?.UpdateData();
            fRecipe?.UpdateData();
            fDPSCalc?.UpdateData();
            fData?.UpdateData(dvLastPower);
        }

        void frmMain_Closed(object sender, EventArgs e)
        {
            MidsContext.Config.LastSize = Size;
            MidsContext.Config.SaveConfig(MyApplication.GetSerializer());
        }

        void frmMain_Closing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = CloseCommand();
        }

        void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (!(e.Alt & e.Control & e.Shift & e.KeyCode == Keys.A))
                return;
            tsAdvFreshInstall.Visible = true;
            MidsContext.Config.MasterMode = true;
            SetTitleBar(MainModule.MidsController.Toon.IsHero());
        }


        void frmMain_Maximize(object sender, EventArgs e)
        {
            if (WindowState != LastState)
                frmMain_Resize(RuntimeHelpers.GetObjectValue(sender), e);
            LastState = WindowState;
        }

        void frmMain_MouseWheel(object sender, MouseEventArgs e) => dvAnchored.Info_txtLarge.Focus();

        void frmMain_Resize(object sender, EventArgs e)
        {
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
                MidsContext.Config.LastSize = Size;
            UpdateControls();
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
            UpdateColours();
        }

        void GetBestDamageValues()
        {
            if (MainModule.MidsController.Toon == null)
                return;
            float highBase = 0.0f;
            for (int index = 0; index <= MidsContext.Character.Powersets[0].Powers.Length - 1; ++index)
            {
                IPower power = MidsContext.Character.Powersets[0].Powers[index];
                if (power.SkipMax)
                    continue;
                float damageValue = power.FXGetDamageValue();
                if (damageValue > (double)highBase)
                    highBase = damageValue;
            }

            for (int index = 0; index <= MidsContext.Character.Powersets[1].Powers.Length - 1; ++index)
            {
                IPower power = MidsContext.Character.Powersets[1].Powers[index];
                if (power.SkipMax)
                    continue;
                float damageValue = power.FXGetDamageValue();
                if (damageValue > (double)highBase)
                    highBase = damageValue;
            }

            MainModule.MidsController.Toon.GenerateBuffedPowerArray();
            float highEnh = highBase * (1f + MidsContext.Character.TotalsCapped.BuffDam + Enhancement.ApplyED(Enums.eSchedule.A, 2.277f));
            if (MidsContext.Config.DamageMath.ReturnValue == ConfigData.EDamageReturn.DPS |
                MidsContext.Config.DamageMath.ReturnValue == ConfigData.EDamageReturn.DPA)
                highEnh *= 1.5f;
            myDataView.Info_Damage.nHighBase = highBase;
            myDataView.Info_Damage.nHighEnh = highEnh;
        }

        int GetFirstValidSetEnh(int slotIndex, int hID)
        {
            if (LastEnhPlaced == null || LastEnhPlaced.Enh < 0 ||
                DatabaseAPI.Database.Enhancements[LastEnhPlaced.Enh].TypeID != Enums.eType.SetO)
                return -1;
            int nIdSet = DatabaseAPI.Database.Enhancements[LastEnhPlaced.Enh].nIDSet;
            if (nIdSet < 0)
                return -1;
            if (MidsContext.Character.CurrentBuild.EnhancementTest(slotIndex, hID, LastEnhPlaced.Enh, true))
                return LastEnhPlaced.Enh;
            int num = DatabaseAPI.Database.EnhancementSets[nIdSet].Enhancements.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (MidsContext.Character.CurrentBuild.EnhancementTest(slotIndex, hID,
                    DatabaseAPI.Database.EnhancementSets[nIdSet].Enhancements[index], true))
                    return DatabaseAPI.Database.EnhancementSets[nIdSet].Enhancements[index];
            }

            return -1;
        }

        bool GetPlayableClasses(Archetype a) => a.Playable;

        I9Slot GetRepeatEnhancement(int powerIndex, int iSlotIndex)
        {
            if (LastEnhPlaced == null)
                return new I9Slot();
            if (MidsContext.Character.CurrentBuild.Powers[powerIndex].NIDPower < 0)
                return new I9Slot();
            if (LastEnhPlaced.Enh <= -1)
                return new I9Slot();
            if (DatabaseAPI.Database.Enhancements[LastEnhPlaced.Enh].TypeID != Enums.eType.SetO)
            {
                return DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[powerIndex].NIDPower]
                    .IsEnhancementValid(LastEnhPlaced.Enh)
                    ? LastEnhPlaced
                    : new I9Slot();
            }

            int firstValidSetEnh = GetFirstValidSetEnh(iSlotIndex, powerIndex);
            if (firstValidSetEnh <= -1)
                return new I9Slot();
            LastEnhPlaced.Enh = firstValidSetEnh;
            LastEnhPlaced.IOLevel = DatabaseAPI.Database.Enhancements[firstValidSetEnh].CheckAndFixIOLevel(LastEnhPlaced.IOLevel);
            return LastEnhPlaced;
        }

        void heroVillain_ButtonClicked()
        {
            MidsContext.Character.Alignment = !heroVillain.Checked ? Enums.Alignment.Hero : Enums.Alignment.Villain;
            if (fAccolade != null)
            {
                if (!fAccolade.IsDisposed)
                    fAccolade.Dispose();
                IPower power = MidsContext.Character.IsHero()
                    ? DatabaseAPI.Database.Power[DatabaseAPI.NidFromStaticIndexPower(3258)]
                    : DatabaseAPI.Database.Power[DatabaseAPI.NidFromStaticIndexPower(3257)];
                for (int index = 0; index <= power.NIDSubPower.Length - 1; ++index)
                {
                    if (MidsContext.Character.CurrentBuild.PowerUsed(DatabaseAPI.Database.Power[power.NIDSubPower[index]]))
                        MidsContext.Character.CurrentBuild.RemovePower(DatabaseAPI.Database.Power[power.NIDSubPower[index]]);
                }
            }

            drawing.ColourSwitch();
            SetTitleBar();
            UpdateColours();
            DoRedraw();
        }

        void HidePopup()
        {
            if (!PopUpVisible)
                return;
            PopUpVisible = false;
            Rectangle bounds = I9Popup.Bounds;
            bounds.X -= pnlGFXFlow.Left;
            bounds.Y -= pnlGFXFlow.Top;
            I9Popup.Visible = false;
            I9Popup.eIDX = -1;
            I9Popup.pIDX = -1;
            I9Popup.hIDX = -1;
            I9Popup.psIDX = -1;
            ActivePopupBounds = new Rectangle(0, 0, 1, 1);
            drawing.Refresh(bounds);
        }

        void I9Picker_EnhancementPicked(I9Slot e)
        {
            e.RelativeLevel = I9Picker.UI.View.RelLevel;
            if (EnhancingSlot <= -1)
                return;
            bool enhChanged = false;
            if (MidsContext.Character.CurrentBuild.EnhancementTest(EnhancingSlot, EnhancingPower, e.Enh) | e.Enh < 0)
            {
                var power = MidsContext.Character.CurrentBuild.Powers[EnhancingPower];
                if (e.Enh != power.Slots[EnhancingSlot].Enhancement.Enh)
                    enhChanged = true;
                bool hasProc = power.HasProc();
                power.Slots[EnhancingSlot].Enhancement = (I9Slot)e.Clone();
                if (e.Enh > -1)
                    LastEnhPlaced = (I9Slot)e.Clone();
                if (enhChanged)
                {
                    if (e.Enh > -1)
                    {
                        if (!hasProc && power.HasProc() && (Math.Abs(DatabaseAPI.Database.Enhancements[e.Enh].Probability) < float.Epsilon ||
                                                            Math.Abs(DatabaseAPI.Database.Enhancements[e.Enh].Probability - 1.0) <
                                                            float.Epsilon))
                            power.StatInclude = true;
                        else if (!power.CanIncludeForStats())
                            power.StatInclude = false;
                    }
                    else if (!power.CanIncludeForStats())
                        power.StatInclude = false;
                }

                I9Picker.Visible = false;
                PowerModified(markModified: true);
                if (EnhancingPower > -1)
                    RefreshTabs(MidsContext.Character.CurrentBuild.Powers[EnhancingPower].NIDPower, e);
            }

            I9Picker.Visible = false;
            EnhancingSlot = -1;
            EnhancingPower = -1;
        }

        void I9Picker_Hiding(object sender, EventArgs e)
        {
            if (!I9Picker.Visible)
                return;
            I9Picker.Visible = false;
            HidePopup();
            EnhancingSlot = -1;
            RefreshInfo();
        }

        void I9Picker_HoverEnhancement(int e)
        {
            I9Slot i9Slot = new I9Slot
            {
                Enh = e,
                IOLevel = I9Picker.CheckAndReturnIOLevel() - 1,
                Grade = I9Picker.UI.View.GradeID,
                RelativeLevel = I9Picker.UI.View.RelLevel
            };
            myDataView.SetEnhancementPicker(i9Slot);
            ShowPopup(PickerHID, -1, -1, new Point(), I9Picker.Bounds, i9Slot);
        }

        void I9Picker_HoverSet(int e)
        {
            myDataView.SetSetPicker(e);
            ShowPopup(PickerHID, -1, -1, new Point(), I9Picker.Bounds, null, e);
        }

        void I9Picker_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || EnhancingSlot <= -1)
                return;
            I9Picker.Visible = false;
            EnhancingSlot = -1;
            RefreshInfo();
        }

        void I9Picker_Moved(Rectangle NewBounds, Rectangle OldBounds)
        {
            MovePopup(I9Picker.Bounds);
            RedrawUnderPopup(OldBounds);
        }

        void I9Popup_MouseMove(object sender, MouseEventArgs e) => HidePopup();

        void ibMode_ButtonClicked()
        {
            if (MainModule.MidsController.Toon == null)
                return;
            MidsContext.Config.BuildMode =
                MidsContext.Config.BuildMode != Enums.dmModes.Dynamic ? Enums.dmModes.Dynamic : Enums.dmModes.LevelUp;
            MidsContext.Character.ResetLevel();
            // changing from dynamic view to level up or reverse is not a file modification
            PowerModified(markModified: false);
            UpdateDMBuffer();
            pbDynMode.Refresh();
        }

        void ibPopup_ButtonClicked() => MidsContext.Config.DisableShowPopup = !ibPopup.Checked;

        void ibPvX_ButtonClicked()
        {
            MidsContext.Config.Inc.DisablePvE = ibPvX.Checked;
            RefreshInfo();
        }

        void ibRecipe_ButtonClicked() => MidsContext.Config.PopupRecipes = ibRecipe.Checked;

        void ibSets_ButtonClicked()
        {
            if (MainModule.MidsController.Toon == null)
                return;
            FloatSets(true);
        }

        void ibSlotLevels_ButtonClicked() => tsViewSlotLevels_Click(this, new EventArgs());

        void ibTotals_ButtonClicked() => FloatTotals(true);

        void incarnateButton_ButtonClicked()
        {
            bool flag = false;
            if (fIncarnate == null)
                flag = true;
            else if (fIncarnate.IsDisposed)
                flag = true;
            if (flag)
            {
                frmMain iParent = this;
                fIncarnate = new frmIncarnate(ref iParent);
            }

            if (!fIncarnate.Visible)
            {
                incarnateButton.Checked = true;
                fIncarnate.Show(this);
            }
            else
            {
                incarnateButton.Checked = false;
                fIncarnate.Close();
            }
        }

        void IncarnateWindowToolStripMenuItem_Click(object sender, EventArgs e) => incarnateButton_ButtonClicked();

        void Info_Enhancement(I9Slot iEnh, int iLevel = -1) => myDataView.SetEnhancement(iEnh, iLevel);

        internal void UnlockFloatingStats()
        {
            DataViewLocked = false;
            if (dvLastPower <= -1)
                return;
            Info_Power(dvLastPower, dvLastEnh, dvLastNoLev, DataViewLocked);
        }

        void Info_Power(int powerIdx, int iEnhLvl = -1, bool NoLevel = false, bool Lock = false)
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
            int num1 = -1;
            if (MainModule.MidsController.Toon.Locked)
            {
                int num2 = MidsContext.Character.CurrentBuild.Powers.Count - 1;
                for (int index = 0; index <= num2; ++index)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[index].NIDPower != powerIdx)
                        continue;
                    num1 = index;
                    break;
                }
            }

            DataViewLocked = Lock;
            if (num1 > -1)
                myDataView.SetData(MainModule.MidsController.Toon.GetBasePower(num1), MainModule.MidsController.Toon.GetEnhancedPower(num1),
                    NoLevel, DataViewLocked, num1);
            else
                myDataView.SetData(MainModule.MidsController.Toon.GetBasePower(num1, powerIdx), null, NoLevel, DataViewLocked, num1);
            if (!Lock || dvAnchored.Visible)
                return;
            FloatingDataForm.Activate();
        }

        void info_Totals()
        {
            if (MainModule.MidsController.Toon == null | !MainModule.MidsController.IsAppInitialized)
                return;
            MainModule.MidsController.Toon.GenerateBuffedPowerArray();
            myDataView.DisplayTotals();
            FloatUpdate();
        }

        void lblATLocked_MouseLeave(object sender, EventArgs e) => HidePopup();

        void lblATLocked_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || cbAT.SelectedIndex < 0)
                return;
            ShowPopup(-1, CbtAT.Value.SelectedItem.Idx, cbAT.Bounds, string.Empty);
        }

        void lblATLocked_Paint(object sender, PaintEventArgs e)
        {
            if (MainModule.MidsController.Toon == null)
                return;
            RectangleF destRect = new RectangleF(1f, (lblATLocked.Height - 16) / 2f, 16f, 16f);
            --destRect.Y;
            RectangleF srcRect = new RectangleF((MidsContext.Character.Archetype.Idx * 16), 0.0f, 16f, 16f);
            Graphics graphics = e.Graphics;
            graphics.DrawImage(I9Gfx.Archetypes.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
            destRect.X = lblATLocked.Width - 19;
            graphics.DrawImage(I9Gfx.Archetypes.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
        }

        void lblLocked0_MouseLeave(object sender, EventArgs e) => HidePopup();

        void lblLocked0_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[3] == null)
                return;
            string ExtraString = "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[3].nID, MidsContext.Character.Archetype.Idx, cbPool0.Bounds, ExtraString);
        }

        void lblLocked0_Paint(object sender, PaintEventArgs e) => MiniPaint(ref e, Enums.PowersetType.Pool0);

        void lblLocked1_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[4] == null)
                return;
            string ExtraString = "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[4].nID, MidsContext.Character.Archetype.Idx, cbPool1.Bounds, ExtraString);
        }

        void lblLocked1_Paint(object sender, PaintEventArgs e) => MiniPaint(ref e, Enums.PowersetType.Pool1);

        void lblLocked2_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[5] == null)
                return;
            string ExtraString = "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[5].nID, MidsContext.Character.Archetype.Idx, cbPool2.Bounds, ExtraString);
        }

        void lblLocked2_Paint(object sender, PaintEventArgs e) => MiniPaint(ref e, Enums.PowersetType.Pool2);

        void lblLocked3_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[6] == null)
                return;
            string ExtraString = "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[6].nID, MidsContext.Character.Archetype.Idx, cbPool3.Bounds, ExtraString);
        }

        void lblLocked3_Paint(object sender, PaintEventArgs e) => MiniPaint(ref e, Enums.PowersetType.Pool3);

        void lblLockedAncillary_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[7] == null)
                return;
            string ExtraString = "This is a pool powerset. This powerset can be changed by removing all of the powers selected from it.";
            ShowPopup(MidsContext.Character.Powersets[7].nID, MidsContext.Character.Archetype.Idx, CbtAncillary.Value.Bounds, ExtraString);
        }

        void lblLockedAncillary_Paint(object sender, PaintEventArgs e) => MiniPaint(ref e, Enums.PowersetType.Ancillary);

        void lblLockedSecondary_MouseLeave(object sender, EventArgs e) => HidePopup();

        void lblLockedSecondary_MouseMove(object sender, MouseEventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Archetype.Idx < 0 || cbSecondary.SelectedIndex < 0)
                return;
            string ExtraString = MidsContext.Character.Powersets[0].nIDLinkSecondary <= -1
                ? "This is your secondary powerset. This powerset can be changed after a build has been started, and any placed powers will be swapped out for those in the new set."
                : "This is your secondary powerset. This powerset is linked to your primary set and cannot be changed independantly. However, it can be changed by selecting a different primary powerset.";
            ShowPopup(
                DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, Enums.ePowerSetType.Secondary)[cbSecondary.SelectedIndex].nID,
                MidsContext.Character.Archetype.Idx, cbSecondary.Bounds, ExtraString);
        }

        void llAll_EmptyHover() => HidePopup();

        void llALL_MouseLeave(object sender, EventArgs e) => HidePopup();

        void llAncillary_ItemClick(ListLabelV2.ListLabelItemV2 Item, MouseButtons Button)
        {
            if (Item.ItemState == ListLabelV2.LLItemState.Heading)
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
        }

        void llAncillary_ItemHover(ListLabelV2.ListLabelItemV2 Item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            if (Item.ItemState == ListLabelV2.LLItemState.Heading)
            {
                ShowPopup(Item.nIDSet, -1, llAncillary.Bounds, string.Empty);
            }
            else
            {
                Info_Power(Item.nIDPower);
                ShowPopup(-1, Item.nIDPower, -1, new Point(), llAncillary.Bounds);
            }
        }

        void llPool0_ItemClick(ListLabelV2.ListLabelItemV2 Item, MouseButtons Button)
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
        }

        void llPool0_ItemHover(ListLabelV2.ListLabelItemV2 Item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            Info_Power(Item.nIDPower);
            ShowPopup(-1, Item.nIDPower, -1, new Point(), llPool0.Bounds);
        }

        void llPool1_ItemClick(ListLabelV2.ListLabelItemV2 Item, MouseButtons Button)
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
        }

        void llPool1_ItemHover(ListLabelV2.ListLabelItemV2 Item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            Info_Power(Item.nIDPower);
            ShowPopup(-1, Item.nIDPower, -1, new Point(), llPool1.Bounds);
        }

        void llPool2_ItemClick(ListLabelV2.ListLabelItemV2 Item, MouseButtons Button)
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
        }

        void llPool2_ItemHover(ListLabelV2.ListLabelItemV2 Item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            Info_Power(Item.nIDPower);
            ShowPopup(-1, Item.nIDPower, -1, new Point(), llPool2.Bounds);
        }

        void llPool3_ItemClick(ListLabelV2.ListLabelItemV2 Item, MouseButtons Button)
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
        }

        void llPool3_ItemHover(ListLabelV2.ListLabelItemV2 Item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            Info_Power(Item.nIDPower);
            ShowPopup(-1, Item.nIDPower, -1, new Point(), llPool3.Bounds);
        }

        void llPrimary_ItemClick(ListLabelV2.ListLabelItemV2 Item, MouseButtons Button)
        {
            if (Item.ItemState == ListLabelV2.LLItemState.Heading)
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
        }

        void llPrimary_ItemHover(ListLabelV2.ListLabelItemV2 Item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            if (Item.ItemState == ListLabelV2.LLItemState.Heading)
            {
                ShowPopup(Item.nIDSet, -1, llPrimary.Bounds, string.Empty);
            }
            else
            {
                Info_Power(Item.nIDPower);
                ShowPopup(-1, Item.nIDPower, -1, new Point(), llPrimary.Bounds);
            }
        }

        void llSecondary_ItemClick(ListLabelV2.ListLabelItemV2 Item, MouseButtons Button)
        {
            if (Item.ItemState == ListLabelV2.LLItemState.Heading)
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
        }

        void llSecondary_ItemHover(ListLabelV2.ListLabelItemV2 Item)
        {
            LastIndex = -1;
            LastEnhIndex = -1;
            if (Item.ItemState == ListLabelV2.LLItemState.Heading)
            {
                ShowPopup(Item.nIDSet, -1, llSecondary.Bounds, string.Empty);
            }
            else
            {
                Info_Power(Item.nIDPower);
                ShowPopup(-1, Item.nIDPower, -1, new Point(), llSecondary.Bounds);
            }
        }

        void MiniPaint(ref PaintEventArgs e, Enums.PowersetType iId)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Character.Powersets[(int)iId] == null)
                return;
            RectangleF destRect = new RectangleF(1f, (lblLocked0.Height - 16) / 2f, 16f, 16f);
            --destRect.Y;
            RectangleF srcRect = new RectangleF(MidsContext.Character.Powersets[(int)iId].nID * 16, 0.0f, 16f, 16f);
            Graphics graphics = e.Graphics;
            graphics.DrawImage(I9Gfx.Powersets.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
            destRect.X = lblLocked0.Width - 19;
            graphics.DrawImage(I9Gfx.Powersets.Bitmap, destRect, srcRect, GraphicsUnit.Pixel);
        }

        void MovePopup(Rectangle rBounds)
        {
            if (!PopUpVisible)
                return;
            Rectangle bounds = I9Popup.Bounds;
            if (rBounds == bounds)
                return;
            SetPopupLocation(rBounds, false, true);
            RedrawUnderPopup(bounds);
        }

        void NewDraw(bool skipDraw = false)
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

        void NewToon(bool init = true, bool skipDraw = false)
        {
            if (MainModule.MidsController.Toon == null)
                MainModule.MidsController.Toon = new clsToonX();
            else if (init)
            {
                MidsContext.Character.Reset();
            }
            else
            {
                string str = !MainModule.MidsController.Toon.Locked ? MidsContext.Character.Name : string.Empty;
                MidsContext.Character.Reset((Archetype)cbAT.SelectedItem, cbOrigin.SelectedIndex);
                if (MidsContext.Character.Powersets[0].nIDLinkSecondary > -1)
                    MidsContext.Character.Powersets[1] = DatabaseAPI.Database.Powersets[MidsContext.Character.Powersets[0].nIDLinkSecondary];
                MidsContext.Character.Name = str;
            }

            if (fAccolade != null && !fAccolade.IsDisposed)
                fAccolade.Dispose();
            if (fTemp != null && !fTemp.IsDisposed)
                fTemp.Dispose();
            if (fIncarnate != null && !fIncarnate.IsDisposed)
                fIncarnate.Dispose();
            if (MidsContext.Character.Archetype.DisplayName != "Mastermind")
            {
                petsButton.Visible = false;
                petsButton.Enabled = false;
            }
            else
            {
                petsButton.Visible = true;
                petsButton.Enabled = true;
            }
            NewDraw(skipDraw);
            UpdateControls(true);
            SetTitleBar(MidsContext.Character.IsHero());
            UpdateColours();
            info_Totals();
            FileModified = false;
            DoRedraw();
        }

        void pbDynMode_Click(object sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon == null || MidsContext.Config.BuildMode != Enums.dmModes.Dynamic)
                return;
            MidsContext.Config.BuildOption = MidsContext.Config.BuildOption == Enums.dmItem.Power ? Enums.dmItem.Slot : Enums.dmItem.Power;
            UpdateDMBuffer();
            pbDynMode.Refresh();
        }

        void pbDynMode_Paint(object sender, PaintEventArgs e)
        {
            if (dmBuffer == null)
                UpdateDMBuffer();
            if (dmBuffer == null)
                return;
            e.Graphics.DrawImage(dmBuffer.Bitmap, e.ClipRectangle, e.ClipRectangle, GraphicsUnit.Pixel);
        }

        void pnlGFX_DragDrop(object sender, DragEventArgs e)
        {
            if (!sender.Equals(pnlGFX))
                return;
            pnlGFX.AllowDrop = false;
            ControlPaint.DrawReversibleFrame(dragRect, Color.White, FrameStyle.Thick);
            oldDragRect = Rectangle.Empty;
            dragRect = Rectangle.Empty;
            int iValue1 = e.X + xCursorOffset;
            int iValue2 = e.Y + yCursorOffset;
            dragFinishPower = drawing.WhichSlot(drawing.ScaleUp(iValue1), drawing.ScaleUp(iValue2));
            if (dragStartSlot != -1)
            {
                dragFinishSlot = drawing.WhichEnh(drawing.ScaleUp(iValue1), drawing.ScaleUp(iValue2));
                if (dragFinishSlot == 0)
                {
                    MessageBox.Show(this, "You cannot change the level of any power's automatic slot.", null, MessageBoxButtons.OK);
                }
                else
                    SlotLevelSwap(dragStartPower, dragStartSlot, dragFinishPower, dragFinishSlot);
            }
            else if ((e.KeyState & 4) > 0)
                PowerMoveByUser(dragStartPower, dragFinishPower);
            else
                PowerSwapByUser(dragStartPower, dragFinishPower);
        }

        void pnlGFX_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = sender.Equals(pnlGFX) ? DragDropEffects.Move : DragDropEffects.None;
        }

        void pnlGFX_DragOver(object sender, DragEventArgs e)
        {
            Point position;
            int num1;
            if (sender.Equals(pnlGFX))
            {
                if (!dragRect.IsEmpty)
                {
                    int top = dragRect.Top;
                    position = Cursor.Position;
                    int num2 = position.Y - dragYOffset;
                    int num3 = top != num2 ? 1 : 0;
                    int left = dragRect.Left;
                    position = Cursor.Position;
                    int num4 = position.X - dragXOffset;
                    int num5 = left != num4 ? 1 : 0;
                    num1 = (num3 | num5) == 0 ? 1 : 0;
                }
                else
                    num1 = 0;
            }
            else
                num1 = 1;

            if (num1 != 0)
                return;
            if (dragStartSlot != -1)
            {
                position = Cursor.Position;
                int x = position.X - dragXOffset;
                position = Cursor.Position;
                int y = position.Y - dragYOffset;
                int width = drawing.ScaleDown(drawing.szSlot.Width);
                int height = drawing.ScaleDown(drawing.szSlot.Height);
                dragRect = new Rectangle(x, y, width, height);
            }
            else
            {
                position = Cursor.Position;
                int x = position.X - dragXOffset;
                position = Cursor.Position;
                int y = position.Y - dragYOffset;
                int width = drawing.ScaleDown(drawing.SzPower.Width);
                int height = drawing.ScaleDown(drawing.SzPower.Height);
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

        void pnlGFX_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!(!LastClickPlacedSlot && dragStartSlot >= 0))
                return;
            MainModule.MidsController.Toon.BuildSlot(dragStartPower, dragStartSlot);
            // no idea what pnlGFX_MouseDoubleClick represents, marking modified as it would have before the added arg
            PowerModified(markModified: true);
            FileModified = true;
            DoneDblClick = true;
            LastClickPlacedSlot = false;
        }

        void pnlGFX_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            pnlGFX.AllowDrop = true;
            dragStartX = e.X;
            dragStartY = e.Y;
            dragStartPower = drawing.WhichSlot(drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y));
            dragStartSlot = drawing.WhichEnh(drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y));
        }

        void pnlGFX_MouseEnter(object sender, EventArgs e) => pnlGFXFlow.Focus();

        void pnlGFX_MouseLeave(object sender, EventArgs e)
        {
            HidePopup();
            drawing.HighlightSlot(-1);
        }

        void pnlGFX_MouseMove(object sender, MouseEventArgs e)
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
                        dragXOffset = drawing.ScaleDown(drawing.szSlot.Width / 2);
                        dragYOffset = drawing.ScaleDown(drawing.szSlot.Height / 2);
                    }
                    else
                    {
                        dragXOffset = drawing.ScaleDown(drawing.SzPower.Width / 2);
                        dragYOffset = drawing.ScaleDown(drawing.SzPower.Height / 2);
                    }

                    DataObject dataObject = new DataObject();
                    dataObject.SetText("This is some filler power text right here");
                    HidePopup();
                    pnlGFX.Cursor = Cursors.Default;
                    drawing.HighlightSlot(-1);
                    Application.DoEvents();
                    ibPopup.DoDragDrop(dataObject, DragDropEffects.Move);
                }
            }
            else
            {
                int index = drawing.WhichSlot(drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y));
                int sIDX = drawing.WhichEnh(drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y));
                if (index < 0 | index >= MidsContext.Character.CurrentBuild.Powers.Count)
                {
                    HidePopup();
                }
                else
                {
                    Point e1 = new Point(e.X, e.Y);
                    ShowPopup(index, -1, sIDX, e1, new Rectangle());
                    if (MidsContext.Character.CanPlaceSlot &
                        MainModule.MidsController.Toon.SlotCheck(MidsContext.Character.CurrentBuild.Powers[index]) > -1)
                    {
                        drawing.HighlightSlot(index);
                        if (index > -1 & drawing.InterfaceMode != Enums.eInterfaceMode.PowerToggle)
                            pnlGFX.Cursor = Cursors.Hand;
                        else
                            pnlGFX.Cursor = Cursors.Default;
                    }
                    else
                    {
                        pnlGFX.Cursor = Cursors.Default;
                        drawing.HighlightSlot(-1);
                    }

                    if (index <= -1 || !(index != LastIndex | LastEnhIndex != sIDX))
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

        void pnlGFX_MouseUp(object sender, MouseEventArgs e)
        {
            pnlGFX.AllowDrop = false;
            if (DoneDblClick)
            {
                DoneDblClick = false;
            }
            else
            {
                int hIDPower = drawing.WhichSlot(drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y));
                int slotID = drawing.WhichEnh(drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y));
                if (hIDPower < 0 | hIDPower >= MidsContext.Character.CurrentBuild.Powers.Count)
                    return;
                bool flag = MidsContext.Character.CurrentBuild.Powers[hIDPower].NIDPower < 0;
                /*if (e.Button == MouseButtons.Left & ModifierKeys == (Keys.Shift | Keys.Control) && EditAccoladesOrTemps(hIDPower))
                    return;*/
                if (e.Button == MouseButtons.Left & ModifierKeys == (Keys.Shift | Keys.Control) && EditAccoladesOrTemps(hIDPower))
                    return;
                if (drawing.InterfaceMode == Enums.eInterfaceMode.PowerToggle & e.Button == MouseButtons.Left)
                {
                    if (!flag && MidsContext.Character.CurrentBuild.Powers[hIDPower].CanIncludeForStats())
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[hIDPower].StatInclude)
                        {
                            MidsContext.Character.CurrentBuild.Powers[hIDPower].StatInclude = false;
                        }
                        else
                        {
                            Enums.eMutex eMutex = MainModule.MidsController.Toon.CurrentBuild.MutexV2(hIDPower);
                            if (eMutex == Enums.eMutex.NoConflict | eMutex == Enums.eMutex.NoGroup)
                                MidsContext.Character.CurrentBuild.Powers[hIDPower].StatInclude = true;
                        }
                    }

                    EnhancementModified();
                    LastClickPlacedSlot = false;
                }
                else if (ToggleClicked(hIDPower, drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y)) & e.Button == MouseButtons.Left)
                {
                    if (!flag && MidsContext.Character.CurrentBuild.Powers[hIDPower].CanIncludeForStats())
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[hIDPower].StatInclude)
                        {
                            MidsContext.Character.CurrentBuild.Powers[hIDPower].StatInclude = false;
                        }
                        else
                        {
                            Enums.eMutex eMutex = MainModule.MidsController.Toon.CurrentBuild.MutexV2(hIDPower);
                            if (eMutex == Enums.eMutex.NoConflict | eMutex == Enums.eMutex.NoGroup)
                                MidsContext.Character.CurrentBuild.Powers[hIDPower].StatInclude = true;
                        }

                        MidsContext.Character.Validate();
                    }

                    EnhancementModified();
                    LastClickPlacedSlot = false;
                }
                else if (e.Button == MouseButtons.Left & ModifierKeys == Keys.Alt)
                {
                    MainModule.MidsController.Toon.BuildPower(MidsContext.Character.CurrentBuild.Powers[hIDPower].NIDPowerset,
                        MidsContext.Character.CurrentBuild.Powers[hIDPower].NIDPower);
                    PowerModified(markModified: true);
                    LastClickPlacedSlot = false;
                }
                else if (e.Button == MouseButtons.Left & ModifierKeys == Keys.Shift & slotID > -1)
                {
                    if (MidsContext.Config.BuildMode == Enums.dmModes.LevelUp)
                    {
                        MainModule.MidsController.Toon.RequestedLevel =
                            MidsContext.Character.CurrentBuild.Powers[hIDPower].Slots[slotID].Level;
                        MidsContext.Character.ResetLevel();
                    }

                    MainModule.MidsController.Toon.BuildSlot(hIDPower, slotID);
                    PowerModified(markModified: true);
                    LastClickPlacedSlot = false;
                }
                else
                {
                    if (e.Button == MouseButtons.Left & !EnhPickerActive)
                    {
                        if (MidsContext.Config.BuildMode == Enums.dmModes.Dynamic & flag)
                        {
                            if (true & MidsContext.Character.CurrentBuild.Powers[hIDPower].Level > -1)
                            {
                                MainModule.MidsController.Toon.RequestedLevel = MidsContext.Character.CurrentBuild.Powers[hIDPower].Level;
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
                                PowerModified(markModified: false);
                                LastClickPlacedSlot = true;
                                /* Disabled until can find why it is not saving
                                        MidsContext.Config.Tips.Show(Tips.TipType.FirstEnh);*/
                                return;
                            }

                            LastClickPlacedSlot = false;
                        }
                    }

                    if (e.Button == MouseButtons.Middle & slotID > -1 & !MidsContext.Config.DisableRepeatOnMiddleClick)
                    {
                        EnhancingSlot = slotID;
                        EnhancingPower = hIDPower;
                        I9Picker_EnhancementPicked(GetRepeatEnhancement(hIDPower, slotID));
                        EnhancementModified();
                    }
                    else if (e.Button == MouseButtons.Right & slotID > -1 && ModifierKeys != Keys.Shift)
                    {
                        EnhancingSlot = slotID;
                        EnhancingPower = hIDPower;
                        int[] enhancements = MainModule.MidsController.Toon.GetEnhancements(hIDPower);
                        PickerHID = hIDPower;
                        if (!flag)
                            I9Picker.SetData(MidsContext.Character.CurrentBuild.Powers[hIDPower].NIDPower,
                                ref MidsContext.Character.CurrentBuild.Powers[hIDPower].Slots[slotID].Enhancement, ref drawing, enhancements);
                        else
                            I9Picker.SetData(-1, ref MidsContext.Character.CurrentBuild.Powers[hIDPower].Slots[slotID].Enhancement,
                                ref drawing, enhancements);


                        var point = new Point(
                            (int)Math.Round(pnlGFXFlow.Left - pnlGFXFlow.HorizontalScroll.Value + e.X - I9Picker.Width / 2.0),
                            (int)Math.Round(pnlGFXFlow.Top - pnlGFXFlow.VerticalScroll.Value + e.Y - I9Picker.Height / 2.0));
                        if (point.Y < MenuBar.Height)
                            point.Y = MenuBar.Height;
                        Size clientSize;
                        if (point.Y + I9Picker.Height > ClientSize.Height)
                        {
                            ref Point local = ref point;
                            clientSize = ClientSize;
                            local.Y = clientSize.Height - I9Picker.Height;
                        }

                        clientSize = ClientSize;
                        if (point.X + I9Picker.Width > clientSize.Width)
                        {
                            ref Point local = ref point;
                            clientSize = ClientSize;
                            int num2 = clientSize.Width - I9Picker.Width;
                            local.X = num2;
                        }

                        I9Picker.Location = point;
                        I9Picker.BringToFront();
                        I9Picker.Visible = true;
                        I9Picker.Select();
                        LastClickPlacedSlot = false;
                    }
                    else if (e.Button == MouseButtons.Right & ModifierKeys == Keys.Shift)
                        StartFlip(hIDPower);
                    else if (e.Button == MouseButtons.Right)
                    {
                        Info_Power(MidsContext.Character.CurrentBuild.Powers[hIDPower].NIDPower, -1, true, true);
                        LastClickPlacedSlot = false;
                    }
                }
            }
        }

        void pnlGFXFlow_MouseEnter(object sender, EventArgs e) => pnlGFXFlow.Focus();

        // if we are loading a file, the file isn't modified when this method is called
        internal void PowerModified(bool markModified)
        {
            int index = -1;
            MainModule.MidsController.Toon.Complete = false;
            fixStatIncludes();
            if (markModified)
                FileModified = true;
            if (MidsContext.Config.BuildMode == Enums.dmModes.Dynamic)
            {
                index = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex(MainModule.MidsController.Toon.RequestedLevel);
                if (index < 0)
                    index = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex();
            }
            else if (DatabaseAPI.Database.Levels[MidsContext.Character.Level].LevelType() == Enums.dmItem.Power)
            {
                index = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex();
                drawing.HighlightSlot(-1);
            }

            if (MainModule.MidsController.Toon.Complete)
                drawing.HighlightSlot(-1);
            int[] slotCounts = MainModule.MidsController.Toon.GetSlotCounts();
            ibAccolade.TextOff = slotCounts[0] <= 0 ? "No slot left" : slotCounts[0] + " slot" + (slotCounts[0] == 1 ? String.Empty : "s") + " to go";
            ibAccolade.TextOn = slotCounts[1] <= 0 ? "No slot placed" : slotCounts[1] + " slot" + (slotCounts[1] == 1 ? String.Empty : "s") + " placed";
            if (index > -1 & index <= MidsContext.Character.CurrentBuild.Powers.Count)
                MidsContext.Character.RequestedLevel = MidsContext.Character.CurrentBuild.Powers[index].Level;
            MidsContext.Character.Validate();
            DoRedraw();
            Application.DoEvents();
            UpdateControls();
            RefreshInfo();
            UpdateDynamicModeInfo();
            pbDynMode.Refresh();
        }

        bool? CheckInitDdsaValue(int index, int? defaultOpt, string descript, params string[] options)
        {
            if (dragdropScenarioAction[index] != 0) return null;
            var (result, remember) = frmOptionListDlg.ShowWithOptions(AllowRemember: true, DefaultOption: defaultOpt ?? 1, descript, options);
            dragdropScenarioAction[index] = (short)result;
            if (remember == true)
                MidsContext.Config.DragDropScenarioAction[index] = dragdropScenarioAction[index];
            return remember;
        }

        int PowerMove(PowerEntry[] tp, int start, int finish)
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
                            "Move/swap power to its lowest possible level", "Allow power to be moved anyway (mark as invalid)");
                    dragdropScenarioAction[0] = (short)result;
                    if (canOverride)
                    {
                        if (dragdropScenarioAction[0] == 2)
                            dragdropScenarioAction[0] = 3;
                    }

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

                    int lvl = DatabaseAPI.Database.Power[tp[start].NIDPower].Level - 1;
                    int index = 0;
                    while (tp[index].Level != lvl)
                    {
                        ++index;
                        if (index > 23)
                            return PowerMove(tp, start, lvl);
                    }
                }
            }

            bool flag1 = start < finish;
            bool[] flagArray = new bool[tp.Length - 1 + 1];
            if (flag1)
            {
                flagArray[start] = true;
                int level = tp[start].Level;
                int num = finish;
                for (int index = start + 1; index <= num; ++index)
                {
                    if (tp[index].NIDPower < 0)
                    {
                        flagArray[index] = true;
                        level = tp[index].Level;
                    }
                    else if (DatabaseAPI.Database.Power[tp[index].NIDPower].Level - 1 == tp[index].Level)
                        flagArray[index] = false;
                    else if (level >= DatabaseAPI.Database.Power[tp[index].NIDPower].Level - 1)
                    {
                        flagArray[index] = true;
                        level = tp[index].Level;
                    }
                    else
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
                    int num1 = start + 1;
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
                        MessageBox.Show(@"None of the powers can be shifted, so the power was not moved.", null, MessageBoxButtons.OK);
                        return 0;
                    }
                }
            }

            PowerEntry powerEntry =
                tp[start].NIDPower != -1 ? new PowerEntry(DatabaseAPI.Database.Power[tp[start].NIDPower]) : new PowerEntry();
            powerEntry.Slots = (SlotEntry[])tp[start].Slots.Clone();
            powerEntry.Level = tp[start].Level;
            clearPower(tp, start);
            bool flag2 = false;
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

            int num6 = num4;
            int num7 = num5;
            for (int index = num3; (num7 >> 31 ^ index) <= (num7 >> 31 ^ num6); index += num7)
            {
                if (tp[index].NIDPower != -1 && flag1 && !flagArray[index])
                {
                    CheckInitDdsaValue(7, null, "Power being shifted down cannot shift to the necessary level", "Shift other powers around it",
                        "Overwrite it; leave previous power slot empty", "Allow anyway (mark as invalid)");
                    if (dragdropScenarioAction[7] == 1)
                        return 0;
                    if (dragdropScenarioAction[7] == 3)
                    {
                        if (!flag2)
                        {
                            start = index;
                        }

                        break;
                    }
                }

                if (!(!flag2 & tp[index].NIDPower < 0))
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

            int index1 = start;
            int num8 = !flag1 ? index1 - 1 : index1 + 1;
            while (num8 != finish)
            {
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

        void PowerMoveByUser(int dragStart, int dragFinish)
        {
            if (dragStart < 0 || dragStart > 23 || dragFinish < 0 || dragFinish > 23 || dragStart == dragFinish)
                return;
            int index = 0;
            do
            {
                dragdropScenarioAction[index] = MidsContext.Config.DragDropScenarioAction[index];
                ++index;
            } while (index <= 19);

            PowerEntry[] powerEntryArray = DeepCopyPowerList();
            if (PowerMove(powerEntryArray, dragStart, dragFinish) == 0)
                return;
            ShallowCopyPowerList(powerEntryArray);
            PowerModified(markModified: true);
            DoRedraw();
        }

        void PowerPicked(Enums.PowersetType SetID, int nIDPower)
        {
            MainModule.MidsController.Toon.BuildPower(MidsContext.Character.Powersets[(int)SetID].nID, nIDPower);
            PowerModified(markModified: true);
            /* Disabled
             * MidsContext.Config.Tips.Show(Tips.TipType.FirstPower);
             */
        }

        void PowerPicked(int nIDPowerset, int nIDPower)
        {
            MainModule.MidsController.Toon.BuildPower(nIDPowerset, nIDPower);
            PowerModified(markModified: true);
            /* Disabled
             * MidsContext.Config.Tips.Show(Tips.TipType.FirstPower);
             */
            DoRedraw();
        }

        void PowerPickedNoRedraw(int nIDPowerset, int nIDPower)
        {
            MainModule.MidsController.Toon.BuildPower(nIDPowerset, nIDPower, true);
            // Zed: Important: if using PowerModified() the rendering will be super slow!
            //PowerModified(markModified: true);
        }

        int PowerSwap(int mode, ref PowerEntry[] tp, int start, int finish)
        {
            int num1;
            if (start < 0 || start > 23 || finish < 0 || finish > 23 || start == finish)
                return 0;

            if (tp[start].NIDPower == -1 || DatabaseAPI.Database.Power[tp[start].NIDPower].Level - 1 <= tp[finish].Level)
            {
                if (tp[finish].NIDPower != -1 && DatabaseAPI.Database.Power[tp[finish].NIDPower].Level - 1 > tp[start].Level)
                {
                    switch (mode)
                    {
                        case 0:
                            CheckInitDdsaValue(4, null, "Power being replaced is swapped too low", "Overwrite rather than swap",
                                "Allow power to be swapped anyway (mark as invalid)");
                            if (dragdropScenarioAction[4] == 1) return 0;
                            if (dragdropScenarioAction[4] == 2)
                            {
                                tp[finish].NIDPower = -1;
                                tp[finish].NIDPowerset = -1;
                                tp[finish].IDXPower = -1;
                                tp[finish].StatInclude = false;
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
                        CheckInitDdsaValue(0, 0, "Power is moved or swapped too low", "Move/swap power to its lowest possible level",
                            "Allow power to be moved anyway (mark as invalid)");
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

                    int lvl = DatabaseAPI.Database.Power[tp[start].NIDPower].Level - 1;
                    int index = 0;
                    while (tp[index].Level != lvl)
                    {
                        ++index;
                        if (index > 23)
                            return PowerSwap(mode, ref tp, start, lvl);
                    }

                    int num4 = index;
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
                            CheckInitDdsaValue(12, null, "The power in the destination slot is prevented from being shifted up",
                                "Unlock and shift all level-locked powers", "Shift destination power to the first valid and empty slot",
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
            }

            int num5 = tp[22].SlotCount + tp[23].SlotCount;
            int num6 = -1;
            if (start == 22 && finish < 22 && num5 <= 8 && tp[finish].SlotCount + tp[23].SlotCount > 8 ||
                start == 23 && finish < 22 && tp[start].SlotCount <= 4 && tp[finish].SlotCount > 4 ||
                start == 23 && finish < 22 && num5 <= 8 && tp[22].SlotCount + tp[finish].SlotCount > 8 ||
                start == 23 && finish == 22 && tp[finish].SlotCount > 4)
            {
                if (mode < 2)
                    CheckInitDdsaValue(6, null, "Power being replaced is swapped too high to have # slots", "Remove impossible slots",
                        "Allow anyway (Mark slots as invalid)");
                num6 = 6;
            }
            else if (start < 22 & finish == 22 & num5 <= 8 & tp[start].SlotCount + tp[23].SlotCount > 8 ||
                     start < 22 & finish == 23 & tp[finish].SlotCount <= 4 & tp[start].SlotCount > 4 ||
                     start < 22 & finish == 23 & num5 <= 8 & tp[22].SlotCount + tp[start].SlotCount > 8 ||
                     start == 22 & finish == 23 & tp[start].SlotCount > 4)
            {
                if (mode < 2)
                    CheckInitDdsaValue(3, null, "Power is moved or swapped too high to have # slots", "Remove impossible slots",
                        "Allow anyway (Mark slots as invalid)");
                num6 = 3;
            }

            if (num6 != -1 && mode == 2)
            {
                CheckInitDdsaValue(9, null, "Power being shifted up has impossible # of slots", "Remove impossible slots",
                    "Allow anyway (Mark slots as invalid)");
                num6 = 9;
            }

            if (((num6 != 6 ? 0 : (mode < 2 ? 1 : 0)) & (dragdropScenarioAction[6] == 1 ? 1 : 0)) != 0 ||
                ((num6 != 3 ? 0 : (mode < 2 ? 1 : 0)) & (dragdropScenarioAction[3] == 1 ? 1 : 0)) != 0 ||
                num6 == 9 && dragdropScenarioAction[9] == 1)
            {
                num1 = 0;
            }
            else
            {
                if (((num6 != 6 ? 0 : (mode < 2 ? 1 : 0)) & (dragdropScenarioAction[6] == 2 ? 1 : 0)) != 0 ||
                    ((num6 != 3 ? 0 : (mode < 2 ? 1 : 0)) & (dragdropScenarioAction[3] == 2 ? 1 : 0)) != 0 ||
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
                    int integer1 = num2 == 22 ? index :
                        index == 22 ? num2 : 22;
                    int integer2 = num2 == 23 ? index : 23;
                    while (tp[integer1].SlotCount + tp[integer2].SlotCount > 8 || tp[index].SlotCount > 4 && integer2 != 23)
                        tp[index].Slots =
                            tp[index].Slots
                                .RemoveLast(); // (SlotEntry[])Utils.CopyArray(tp[index].Slots, (Array)new SlotEntry[tp[index].SlotCount - 2 + 1]);
                }
                else if (((num6 != 6 ? 0 : (mode < 2 ? 1 : 0)) & (dragdropScenarioAction[6] == (short)3 ? 1 : 0)) != 0 ||
                         ((num6 != 3 ? 0 : (mode < 2 ? 1 : 0)) & (dragdropScenarioAction[3] == (short)3 ? 1 : 0)) != 0 ||
                         num6 == 9 && dragdropScenarioAction[9] == 3)
                {
                    int index1 = start <= finish ? start : finish;
                    if (start == 23 | finish == 23)
                    {
                        for (int index2 = tp[index1].SlotCount - 1; index2 >= 1; index2 += -1)
                        {
                            if (index2 + tp[22].SlotCount > 7 | index2 > 3)
                                tp[index1].Slots[index2].Level = 50;
                        }
                    }
                    else
                    {
                        for (int index2 = tp[index1].SlotCount - 1; index2 >= 1; index2 += -1)
                        {
                            if (index2 + tp[22].SlotCount > 7)
                                tp[index1].Slots[index2].Level = 50;
                        }
                    }
                }

                PowerEntry powerEntry = tp[start];
                tp[start] = tp[finish];
                tp[finish] = powerEntry;
                int level1 = tp[start].Level;
                tp[start].Level = tp[finish].Level;
                tp[finish].Level = level1;
                // swapping start and finish values
                int tmpSwap = start;
                start = finish;
                finish = tmpSwap;
                int index3 = 0;
                do
                {
                    if (tp[index3 == 0 ? start : finish].SlotCount > 0)
                    {
                        tp[index3 == 0 ? start : finish].Slots[0].Level = tp[index3 == 0 ? start : finish].Level;
                        int num2 = tp[index3 == 0 ? start : finish].SlotCount - 1;
                        int slotIDX = 1;
                        while (true)
                        {
                            if (slotIDX <= num2 && slotIDX <= tp[index3 == 0 ? start : finish].SlotCount - 1)
                            {
                                if (tp[index3 == 0 ? start : finish].Slots[slotIDX].Level < tp[index3 == 0 ? start : finish].Level)
                                {
                                    if (mode < 2 & index3 == 0 & dragdropScenarioAction[2] == 0)
                                    {
                                        CheckInitDdsaValue(2, 3, "Power is moved or swapped higher than slots' levels", "Remove slots",
                                            "Mark invalid slots", "Swap slot levels if valid; remove invalid ones",
                                            "Swap slot levels if valid; mark invalid ones", "Rearrange all slots in build");
                                    }
                                    else if (mode == 0 & index3 == 1 & dragdropScenarioAction[5] == 0)
                                    {
                                        CheckInitDdsaValue(5, 3, "Power being replaced is swapped higher than slots' levels", "Remove slots",
                                            "Mark invalid slots", "Swap slot levels if valid; remove invalid ones",
                                            "Swap slot levels if valid; mark invalid ones", "Rearrange all slots in build");
                                    }
                                    else if (mode == 2 & dragdropScenarioAction[8] == 0)
                                    {
                                        CheckInitDdsaValue(8, 3, "Power being shifted up has slots from lower levels", "Remove slots",
                                            "Mark invalid slots", "Swap slot levels if valid; remove invalid ones",
                                            "Swap slot levels if valid; mark invalid ones", "Rearrange all slots in build");
                                    }

                                    if (!(mode < 2 & index3 == 0 & dragdropScenarioAction[2] == 1 ||
                                          mode == 0 & index3 == 1 & dragdropScenarioAction[5] == 1 ||
                                          mode == 2 & dragdropScenarioAction[8] == 1))
                                    {
                                        var value = 1 - index3 == 0 ? start : finish;
                                        if (mode < 2 & index3 == 0 & dragdropScenarioAction[2] == 2 ||
                                            mode == 0 & index3 == 1 & dragdropScenarioAction[5] == 2 ||
                                            mode == 2 & dragdropScenarioAction[8] == 2)
                                        {
                                            RemoveSlotFromTempList(tp[index3 == 0 ? start : finish], slotIDX);
                                            --slotIDX;
                                        }
                                        else if (mode < 2 & index3 == 0 & dragdropScenarioAction[2] == 4 ||
                                                 mode == 0 & index3 == 1 & dragdropScenarioAction[5] == 4 ||
                                                 mode == 2 & dragdropScenarioAction[8] == 4)
                                        {
                                            if (tp[value].SlotCount > slotIDX)
                                            {
                                                int level2 = tp[value].Slots[slotIDX].Level;
                                                tp[value].Slots[slotIDX].Level = tp[index3 == 0 ? start : finish].Slots[slotIDX].Level;
                                                tp[index3 == 0 ? start : finish].Slots[slotIDX].Level = level2;
                                            }
                                            else
                                            {
                                                RemoveSlotFromTempList(tp[index3 == 0 ? start : finish], slotIDX);
                                                --slotIDX;
                                            }
                                        }
                                        else if (mode < 2 & index3 == 0 & dragdropScenarioAction[2] == 5 ||
                                                 mode == 0 & index3 == 1 & dragdropScenarioAction[5] == 5 ||
                                                 mode == 2 & dragdropScenarioAction[8] == 5)
                                        {
                                            if (tp[value].SlotCount > slotIDX)
                                            {
                                                int level2 = tp[value].Slots[slotIDX].Level;
                                                tp[value].Slots[slotIDX].Level = tp[index3 == 0 ? start : finish].Slots[slotIDX].Level;
                                                tp[index3 == 0 ? start : finish].Slots[slotIDX].Level = level2;
                                            }
                                        }
                                        else if (mode < 2 & index3 == 0 & dragdropScenarioAction[2] == 6 ||
                                                 mode == 0 & index3 == 1 & dragdropScenarioAction[5] == 6 ||
                                                 mode == 2 & dragdropScenarioAction[8] == 6)
                                            RearrangeAllSlotsInBuild(tp, true);
                                    }
                                    else
                                        return 0;
                                }

                                ++slotIDX;
                            }
                            else
                                break;
                        }
                    }

                    ++index3;
                } while (index3 <= 1);

                num1 = -1;
            }

            return num1;
        }

        void PowerSwapByUser(int start, int finish)
        {
            int index = 0;
            do
            {
                dragdropScenarioAction[index] = MidsContext.Config.DragDropScenarioAction[index];
                ++index;
            } while (index <= 19);

            PowerEntry[] tp = DeepCopyPowerList();
            if (PowerSwap(0, ref tp, start, finish) != -1)
                return;
            ShallowCopyPowerList(tp);
            PowerModified(markModified: true);
            DoRedraw();
        }

        void PriSec_ExpandChanged(bool Expanded)
        {
            if (llPrimary.isExpanded | llSecondary.isExpanded & dvAnchored.IsDocked & !HasSentForwards)
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

        Rectangle raGetPoolRect(int Index)
        {
            Label label;
            ListLabelV2 ll;
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

        int raGetTop() => MainModule.MidsController.Toon != null
            ? 4 + llPrimary.Top + raGreater(llPrimary.Height, llSecondary.Height)
            : llPrimary.Top + llPrimary.Height;

        int raGreater(int iVal1, int iVal2) => iVal1 <= iVal2 ? iVal2 : iVal1;

        void raMovePool(int Index, int X, int Y)
        {
            Label label1;
            ComboBox comboBox;
            Label label2;
            ListLabelV2 ll;
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

        bool raToFloat()
        {
            llPool0.Height = llPool0.DesiredHeight;
            llPool1.Height = llPool1.DesiredHeight;
            llPool2.Height = llPool2.DesiredHeight;
            llPool3.Height = llPool3.DesiredHeight;
            llAncillary.Height = llAncillary.DesiredHeight;
            Rectangle poolRect1 = raGetPoolRect(0);
            raMovePool(1, poolRect1.Left, poolRect1.Bottom);
            Rectangle poolRect2 = raGetPoolRect(1);
            raMovePool(2, poolRect2.Left, poolRect2.Bottom);
            FixPrimarySecondaryHeight();
            int num = raGreater(raGetTop(), lblPool3.Top);
            if (num + llAncillary.DesiredHeight > ClientSize.Height)
            {
                num = ClientSize.Height - llAncillary.DesiredHeight - cbAncillary.Height - lblEpic.Height;
                Size size = llPrimary.SizeNormal;
                llPrimary.SizeNormal = new Size(size.Width, num - 4 - llPrimary.Top);

                llSecondary.SizeNormal = new Size(llSecondary.SizeNormal.Width, num - 4 - llPrimary.Top);
            }

            Rectangle poolRect3 = raGetPoolRect(2);
            poolRect3.X = llPrimary.Left;
            poolRect3.Y = num;
            raMovePool(4, poolRect3.Left, poolRect3.Top);
            poolRect3.X = llSecondary.Left;
            raMovePool(3, poolRect3.Left, poolRect3.Top);
            return false;
        }

        bool raToNormal()
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
            int num1 = llPool0.Top + cbPool0.Height * 4 + lblPool1.Height * 4;
            int num2 = 3 * llAncillary.ActualLineHeight;
            if (num1 + llPool0.Height + llPool1.Height + llPool2.Height + llPool3.Height + llAncillary.Height > ClientSize.Height)
            {
                int num3 = ClientSize.Height - num1 - llPool0.Height - llPool1.Height - llPool2.Height - llPool3.Height;
                if (num3 < num2)
                    num3 = num2;
                if (llAncillary.Height > num3)
                    llAncillary.Height = num3;
                if (num1 + llPool0.Height + llPool1.Height + llPool2.Height + llPool3.Height + llAncillary.Height > ClientSize.Height)
                {
                    int num4 = ClientSize.Height - num1 - llPool0.Height - llPool1.Height - llPool2.Height - llAncillary.Height;
                    if (num4 < num2)
                        num4 = num2;
                    llPool3.Height = num4;
                    if (num1 + llPool0.Height + llPool1.Height + llPool2.Height + llPool3.Height + llAncillary.Height > ClientSize.Height)
                    {
                        int num5 = ClientSize.Height - num1 - llPool0.Height - llPool1.Height - llPool3.Height - llAncillary.Height;
                        if (num5 < num2)
                            num5 = num2;
                        llPool2.Height = num5;
                        if (num1 + llPool0.Height + llPool1.Height + llPool2.Height + llPool3.Height + llAncillary.Height > ClientSize.Height)
                        {
                            int num6 = ClientSize.Height - num1 - llPool0.Height - llPool2.Height - llPool3.Height - llAncillary.Height;
                            if (num6 < num2)
                                num6 = num2;
                            llPool1.Height = num6;
                            if (num1 + llPool0.Height + llPool1.Height + llPool2.Height + llPool3.Height + llAncillary.Height >
                                ClientSize.Height)
                            {
                                int num7 = ClientSize.Height - num1 - llPool1.Height - llPool2.Height - llPool3.Height - llAncillary.Height;
                                if (num7 < num2)
                                    num7 = num2;
                                llPool0.Height = num7;
                            }
                        }
                    }
                }
            }

            Rectangle poolRect = raGetPoolRect(0);
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

        bool ReArrange(bool Init)
        {
            bool flag2;
            if (drawing == null)
            {
                flag2 = false;
            }
            else
            {
                bool flag3 = !dvAnchored.Visible;
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

        void RearrangeAllSlotsInBuild(PowerEntry[] tp, bool notifyUser = false)
        {
            int index1 = 0;
            int[] numArray1 = new int[tp.Length - 1 + 1];
            int num1 = tp.Length - 1;
            for (int index2 = 0; index2 <= num1; ++index2)
            {
                if (tp[index2].NIDPower == -1 || !DatabaseAPI.Database.Power[tp[index2].NIDPower].AllowFrontLoading)
                    continue;
                numArray1[index1] = index2;
                ++index1;
            }

            int index3 = index1;
            int num2 = tp.Length - 1;
            for (int index2 = 0; index2 <= num2; ++index2)
            {
                if (((tp[index2].NIDPower == -1 ? 0 : (!DatabaseAPI.Database.Power[tp[index2].NIDPower].AllowFrontLoading ? 1 : 0)) |
                     (tp[index2].NIDPower == -1 ? 1 : 0)) == 0)
                    continue;
                bool flag = true;
                int num3 = index2 - 1;
                for (int index4 = index1; index4 <= num3; ++index4)
                {
                    if (tp[index2].Level >= tp[numArray1[index4]].Level)
                        continue;
                    int num4 = index4;
                    for (int index5 = index3 - 1; index5 >= num4; index5 += -1)
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

            int[] numArray2 = fakeInitialize(3, 3, 5, 5, 7, 7, 9, 9, 11, 11, 13, 13, 15, 15, 17, 17, 19, 19, 21, 21, 23, 23, 25, 25, 27, 27,
                29, 29, 31, 31, 31, 33, 33, 33, 34, 34, 34, 36, 36, 36, 37, 37, 37, 39, 39, 39, 40, 40, 40, 42, 42, 42, 43, 43, 43, 45, 45, 45,
                46, 46, 46, 48, 48, 48, 50, 50, 50);
            bool flag1 = false;
            int index6 = 0;
            int num5 = tp.Length - 1;
            for (int index2 = 0; index2 <= num5; ++index2)
            {
                int num3 = tp[numArray1[index2]].SlotCount - 1;
                for (int index4 = 1; index4 <= num3; ++index4)
                {
                    if (index6 == numArray2.Length)
                        flag1 = true;
                    tp[numArray1[index2]].Slots[index4].Level = 50;
                    if (flag1)
                        continue;
                    if (tp[numArray1[index2]].NIDPower == -1 || !DatabaseAPI.Database.Power[tp[numArray1[index2]].NIDPower].AllowFrontLoading)
                    {
                        while (numArray2[index6] <= tp[numArray1[index2]].Level)
                        {
                            ++index6;
                            if (index6 != numArray2.Length)
                                continue;
                            flag1 = true;
                            break;
                        }
                    }

                    tp[numArray1[index2]].Slots[index4].Level = numArray2[index6] - 1;
                    ++index6;
                }
            }

            if (!(flag1 & notifyUser))
                return;
            MessageBox.Show(
                @"The current arrangement of powers and their slots is impossible in-game. Invalid slots have been darkened and marked as level 51.",
                null, MessageBoxButtons.OK);
        }

        void RedrawUnderPopup(Rectangle RectRedraw)
        {
            Rectangle Clip = RectRedraw;
            ref Rectangle local = ref Clip;
            Point location = pnlGFXFlow.Location;
            int x = -location.X;
            location = pnlGFXFlow.Location;
            int y = -location.Y;
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

        internal void RefreshInfo()
        {
            info_Totals();
            if (dvLastPower <= -1)
                return;
            Info_Power(dvLastPower, dvLastEnh, dvLastNoLev, DataViewLocked);
        }

        void RefreshTabs(int iPower, I9Slot iEnh, int iLevel = -1)
        {
            if (iEnh.Enh > -1)
            {
                Info_Power(iPower, iLevel);
                Info_Enhancement(iEnh, iLevel);
            }
            else
                Info_Power(iPower, iLevel, true);
        }

        void RemoveSlotFromTempList(PowerEntry tp, int slotIDX)
            => tp.Slots = tp.Slots.RemoveIndex(slotIDX);

        void SetAncilPoolHeight()
        {
            int num1 = llAncillary.ActualLineHeight * 2;
            int num2 = 1;
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

        void setColumns(int columns)
        {
            MidsContext.Config.Columns = columns;
            drawing.Columns = columns;
            DoResize();
            DoRedraw();
            SetFormWidth();
        }

        void SetDamageMenuCheckMarks()
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
                /* Disabled
                 * MidsContext.Config.Tips.Show(Tips.TipType.TotalsTab);
                 */
            }
            else
            {
                if (drawing.InterfaceMode == Enums.eInterfaceMode.Normal)
                    return;
                drawing.InterfaceMode = Enums.eInterfaceMode.Normal;
                DoRedraw();
            }
        }

        void SetFormHeight(bool Force = false)
        {
            int iVal2;
            int num = Height - ClientSize.Height;
            if (!dvAnchored.Visible)
            {
                iVal2 = llPool3.Top + llPool3.Height * 2 + 4 + num;
            }
            else
            {
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
            }

            if (iVal2 > Height | Force | dvAnchored.IsDocked)
            {
                if (Screen.PrimaryScreen.WorkingArea.Height > iVal2)
                    Height = iVal2;
                else if (Screen.PrimaryScreen.WorkingArea.Height < iVal2)
                    Height = Screen.PrimaryScreen.WorkingArea.Height;
            }

            NoResizeEvent = false;
        }

        void SetFormWidth(bool ToFull = false)
        {
            NoResizeEvent = true;
            int num1 = Width - ClientSize.Width;
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            int num2 = (MidsContext.Config.Columns != 2
                           ? num1 + drawing.GetRequiredDrawingArea().Width + pnlGFXFlow.Left
                           : (!ToFull
                               ? num1 + pnlGFXFlow.Left + drawing.ScaleDown(drawing.GetRequiredDrawingArea().Width)
                               : num1 + drawing.GetRequiredDrawingArea().Width + pnlGFXFlow.Left)) + 8;
            if (Screen.PrimaryScreen.WorkingArea.Width > num2)
            {
                Width = num2;
            }
            else
            {
                Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
                if (workingArea.Width <= num2)
                {
                    workingArea = Screen.PrimaryScreen.WorkingArea;
                    Width = workingArea.Width - num1;
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

        void SetPopupLocation(Rectangle ObjectBounds, bool PowerListing = false, bool Picker = false)
        {
            int y;
            int top = ObjectBounds.Top;
            int num1 = ClientSize.Height - ObjectBounds.Bottom;
            int left = ObjectBounds.Left;
            int num2 = ClientSize.Width - ObjectBounds.Right;
            Rectangle rectangle = new Rectangle(0, 0, 1, 1);
            if (dvAnchored.Visible)
            {
                rectangle.X = dvAnchored.Left;
                rectangle.Y = dvAnchored.Top;
                rectangle.Width = dvAnchored.Width;
                rectangle.Height = dvAnchored.Height;
            }

            int x = -1;
            Size clientSize;
            if (!PowerListing & !Picker)
            {
                if (num1 >= I9Popup.Height)
                    y = ObjectBounds.Bottom;
                else if (top >= I9Popup.Height)
                    y = ObjectBounds.Top - I9Popup.Height;
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
                    y = ObjectBounds.Bottom;
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
                    y = num1 < I9Popup.Height
                        ? (top < I9Popup.Height ? ObjectBounds.Bottom : ObjectBounds.Top - I9Popup.Height)
                        : ObjectBounds.Bottom;
            }
            else if (true)
            {
                y = (int)Math.Round(ObjectBounds.Top + ObjectBounds.Height / 2.0 - I9Popup.Height / 2.0);
                if (y < 0)
                    y = 0;
                int num3 = y + I9Popup.Height;
                clientSize = ClientSize;
                int height = clientSize.Height;
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
                    x = left;
                else if (num2 < (I9Popup.Width - ObjectBounds.Width) / 2.0)
                {
                    clientSize = ClientSize;
                    x = clientSize.Width - I9Popup.Width;
                }
            }

            I9Popup.BringToFront();
            I9Popup.Location = new Point(x, y);
        }

        void SetTitleBar(bool Hero = true)
        {
            if (MainModule.MidsController.Toon != null)
                Hero = MainModule.MidsController.Toon.IsHero();
            string str1 = string.Empty;
            if (MainModule.MidsController.Toon != null)
            {
                if (LastFileName != string.Empty)
                {
                    str1 = FileIO.StripPath(LastFileName) + " - ";
                    tsFileSave.Text = "&Save '" + FileIO.StripPath(LastFileName) + "'";
                }
                else
                    tsFileSave.Text = "&Save";
            }
            else
                tsFileSave.Text = "&Save";

            string str2 = str1 + MidsContext.Title;
            if (!Hero)
                str2 = str2.Replace(nameof(Hero), "Villain");
            if (MidsContext.Config.MasterMode)
            {
                Text = $@"{str2} (Master Mode) v{MidsContext.AppAssemblyVersion} (Database Issue: {DatabaseAPI.Database.Issue}, Version: {DatabaseAPI.Database.Version})";
            }
            else
            {
                //this.Text = str2 + " v" + MidsContext.AppAssemblyVersion + " (Database Issue: " + DatabaseAPI.Database.Issue + " - Updated: " + DatabaseAPI.Database.Date.ToString("dd/MM/yy") + ")";
                Text =
                    $@"{str2} v{MidsContext.AppAssemblyVersion} (Database Issue: {DatabaseAPI.Database.Issue}, Version: {DatabaseAPI.Database.Version})";
            }
        }

        void ShallowCopyPowerList(PowerEntry[] source)
        {
            int num = MidsContext.Character.CurrentBuild.Powers.Count - 1;
            for (int index = 0; index <= num; ++index)
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
            //myDataView.DrawVillain = !MainModule.MidsController.Toon.IsHero();
            dvAnchored.Visible = true;
            NoResizeEvent = true;
            OnResizeEnd(new EventArgs());
            NoResizeEvent = false;
            RefreshInfo();
            ReArrange(false);
            FloatingDataForm = null;
        }

        void ShowPopup(int nIDPowerset, int nIDClass, Rectangle rBounds, string ExtraString = "")
        {
            if (MidsContext.Config.DisableShowPopup)
            {
                HidePopup();
            }
            else
            {
                Rectangle bounds = I9Popup.Bounds;
                RedrawUnderPopup(bounds);
                if (!(nIDPowerset > -1 | nIDClass > -1))
                    return;
                if (I9Popup.psIDX != (nIDPowerset <= -1 ? nIDClass : nIDPowerset))
                {
                    PopUp.PopupData iPopup = nIDPowerset <= -1
                        ? MidsContext.Character.Archetype.PopInfo()
                        : MainModule.MidsController.Toon.PopPowersetInfo(nIDPowerset, ExtraString);
                    if (true & iPopup.Sections != null)
                    {
                        I9Popup.SetPopup(iPopup);
                        PopUpVisible = true;
                        SetPopupLocation(rBounds, false, true);
                    }
                    else
                        HidePopup();

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
                I9Popup.psIDX = nIDPowerset;
            }
        }

        void ShowPopup(
            int hIDX,
            int pIDX,
            int sIDX,
            Point e,
            Rectangle rBounds,
            I9Slot eSlot = null,
            int setIDX = -1)
        {
            if (MidsContext.Config.DisableShowPopup)
            {
                HidePopup();
            }
            else
            {
                bool flag = false;
                PopUp.PopupData iPopup = new PopUp.PopupData();
                bool Picker = false;
                bool PowerListing = false;
                Rectangle bounds = I9Popup.Bounds;
                if (hIDX < 0 & pIDX > -1)
                    hIDX = MidsContext.Character.CurrentBuild.FindInToonHistory(pIDX);
                PowerEntry powerEntry = null;
                if (hIDX > -1)
                    powerEntry = MidsContext.Character.CurrentBuild.Powers[hIDX];
                if (!(I9Popup.hIDX != hIDX | I9Popup.eIDX != sIDX | I9Popup.pIDX != pIDX |
                      (I9Popup.hIDX == -1 | I9Popup.eIDX == -1 | I9Popup.pIDX == -1)))
                    return;
                Rectangle rectangle = new Rectangle();
                if (hIDX > -1 & sIDX < 0 & pIDX < 0 & eSlot == null & setIDX < 0)
                {
                    rectangle = drawing.PowerBoundsUnScaled(hIDX);
                    Point e1 = new Point(drawing.ScaleUp(e.X), drawing.ScaleUp(e.Y));
                    if (drawing.WithinPowerBar(rectangle, e1))
                    {
                        if (powerEntry != null && powerEntry.NIDPower > 0)
                            iPopup = MainModule.MidsController.Toon.PopPowerInfo(hIDX, powerEntry.NIDPower);
                        flag = true;
                    }
                }
                else if (sIDX > -1)
                {
                    rectangle = drawing.PowerBoundsUnScaled(hIDX);
                    if (powerEntry != null)
                        iPopup = Character.PopEnhInfo(powerEntry.Slots[sIDX].Enhancement,
                            powerEntry.Slots[sIDX].Level, powerEntry);
                    flag = true;
                }
                else if (pIDX > -1)
                {
                    rectangle = rBounds;
                    iPopup = MainModule.MidsController.Toon.PopPowerInfo(hIDX, pIDX);
                    flag = true;
                    PowerListing = true;
                }
                else if (eSlot != null & setIDX < 0)
                {
                    rectangle = rBounds;
                    iPopup = Character.PopEnhInfo(eSlot, -1, powerEntry);
                    flag = true;
                    Picker = true;
                }
                else if (setIDX > -1)
                {
                    rectangle = rBounds;
                    iPopup = Character.PopSetInfo(setIDX, powerEntry);
                    flag = true;
                    Picker = true;
                }

                if (flag & iPopup.Sections != null)
                {
                    if (I9Popup.hIDX != hIDX | I9Popup.eIDX != sIDX | I9Popup.pIDX != pIDX |
                        (I9Popup.hIDX == -1 | I9Popup.eIDX == -1 | I9Popup.pIDX == -1))
                    {
                        if (!Picker & !PowerListing)
                        {
                            rectangle = Dilate(drawing.ScaleDown(rectangle), 2);
                            rectangle.X += pnlGFXFlow.Left - pnlGFXFlow.HorizontalScroll.Value;
                            rectangle.Y += pnlGFXFlow.Top - pnlGFXFlow.VerticalScroll.Value;
                        }

                        I9Popup.SetPopup(iPopup);
                        PopUpVisible = true;
                        SetPopupLocation(rectangle, PowerListing, Picker);
                    }

                    I9Popup.Visible = true;
                    if (ActivePopupBounds != I9Popup.Bounds)
                    {
                        RedrawUnderPopup(bounds);
                        ActivePopupBounds = I9Popup.Bounds;
                    }
                }
                else
                    HidePopup();

                I9Popup.hIDX = hIDX;
                I9Popup.eIDX = sIDX;
                I9Popup.pIDX = pIDX;
                I9Popup.psIDX = -1;
            }
        }

        void SlotLevelSwap(int sourcePower, int sourceSlot, int destPower, int destSlot)
        {
            int index = 0;
            do
            {
                dragdropScenarioAction[index] = MidsContext.Config.DragDropScenarioAction[index];
                ++index;
            } while (index <= 19);

            if (MidsContext.Character.CurrentBuild.Powers[sourcePower].Slots[sourceSlot].Level <
                MidsContext.Character.CurrentBuild.Powers[destPower].Level & !DatabaseAPI.Database
                    .Power[MidsContext.Character.CurrentBuild.Powers[destPower].NIDPower].AllowFrontLoading)
            {
                CheckInitDdsaValue(13, 0, "Slot being level-swapped is too low for the destination power",
                    "Allow swap anyway (mark as invalid)");
                if (dragdropScenarioAction[13] == 1)
                    return;
            }

            if (MidsContext.Character.CurrentBuild.Powers[destPower].Slots[destSlot].Level <
                MidsContext.Character.CurrentBuild.Powers[sourcePower].Level & !DatabaseAPI.Database
                    .Power[MidsContext.Character.CurrentBuild.Powers[sourcePower].NIDPower].AllowFrontLoading)
            {
                CheckInitDdsaValue(14, 0, "Slot being level-swapped is too low for the source power", "Allow swap anyway (mark as invalid)");
                if (dragdropScenarioAction[14] == 1)
                    return;
            }

            int level = MidsContext.Character.CurrentBuild.Powers[sourcePower].Slots[sourceSlot].Level;
            MidsContext.Character.CurrentBuild.Powers[sourcePower].Slots[sourceSlot].Level =
                MidsContext.Character.CurrentBuild.Powers[destPower].Slots[destSlot].Level;
            MidsContext.Character.CurrentBuild.Powers[destPower].Slots[destSlot].Level = level;
            PowerModified(markModified: true);
            DoRedraw();
        }

        internal void smlRespecLong(int iLevel, bool mode2)
        {
            SetMiniList(MidsContext.Character.CurrentBuild.GetRespecHelper2(true, iLevel), "Respec Helper", mode2 ? 5096 : 4072);
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

        void StartFlip(int iPowerIndex)
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
            int num = FlipSlotState.Length - 1;
            for (int index = 0; index <= num; ++index)
                FlipSlotState[index] = -(FlipStepDelay * index);
            FlipGP = new PowerEntry();
            FlipGP.Assign(MidsContext.Character.CurrentBuild.Powers[iPowerIndex]);
            FlipGP.Slots = Array.Empty<SlotEntry>();
            if (tmrGfx == null)
                tmrGfx = new Timer(Container);
            tmrGfx.Interval = FlipInterval;
            FlipActive = true;
            tmrGfx.Enabled = true;
            tmrGfx.Start();
        }

        void TemporaryPowersWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tempPowersButton_MouseDown(sender, new MouseEventArgs(MouseButtons.Left, 2, 0, 0, 0));
            tempPowersButton.Checked = true;
        }

        void tempPowersButton_ButtonClicked()
            => PowerModified(markModified: false);

        void petsButton_ButtonClicked()
        {
            if (petWindowFlag)
            {
                petsButton.Checked = false;
                fMMPets.Close();
                petWindowFlag = false;
            }
            else
            {
                MMPets.Clear();
                var CurrentPowers = MidsContext.Character.CurrentBuild.Powers;
                foreach (var item in CurrentPowers)
                {
                    bool PetExists = Enum.GetNames(typeof(Enums.eMMpets)).Contains(item.Name.Replace(" ", "_"));
                    if (PetExists)
                    {
                        MMPets.Add(item.Name);
                    }
                }

                bool isEmpty = !MMPets.Any();
                if (!isEmpty)
                {
                    fMMPets = new frmMMPowers(this, MMPets)
                    {
                        Text = "Mastermind Pets"
                    };

                    if (!fMMPets.Visible)
                    {
                        petsButton.Checked = true;
                        petWindowFlag = true;
                        fMMPets.Show(this);
                    }
                    else
                    {
                        petsButton.Checked = false;
                        fMMPets.Close();
                    }
                }
                else
                {
                    MessageBox.Show("You haven't selected an pet abilities for your mastermind.", "Unavailable", MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    petsButton.Checked = false;
                }
            }
        }

        void tempPowersButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks != 2)
                return;
            tempPowersButton.Checked = false;
            if (fTemp == null || fTemp.IsDisposed)
            {
                IPower power = DatabaseAPI.Database.Power[DatabaseAPI.NidFromStaticIndexPower(3259)];
                List<IPower> iPowers = new List<IPower>();
                int num = power.NIDSubPower.Length - 1;
                for (int index = 0; index <= num; ++index)
                    iPowers.Add(DatabaseAPI.Database.Power[power.NIDSubPower[index]]);
                fTemp = new frmTemp(this, iPowers)
                {
                    Text = "Temporary Powers"
                };
            }

            if (!fTemp.Visible)
                fTemp.Show(this);
        }

        void tlsDPA_Click(object sender, EventArgs e)
        {
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.DPA;
            SetDamageMenuCheckMarks();
            DisplayFormatChanged();
        }

        void tmrGfx_Tick(object sender, EventArgs e)
        {
            if (FlipActive)
                doFlipStep();
        }

        bool ToggleClicked(int hID, int iX, int iY)
        {
            Rectangle rectangle1 = new Rectangle();
            if (hID < 0)
                return false;
            if (MidsContext.Character.CurrentBuild.Powers[hID].IDXPower < 0)
                return false;
            Rectangle rectangle2 = new Rectangle
            {
                Location = drawing.PowerPosition(MidsContext.Character.CurrentBuild.Powers[hID]),
                Size = drawing.bxPower[0].Size
            };
            rectangle1.Height = 15;
            rectangle1.Width = rectangle1.Height;
            rectangle1.Y = (int)Math.Round(rectangle2.Top + ((rectangle2.Height - rectangle1.Height) / 2.0));
            rectangle1.X = (int)Math.Round(rectangle2.Right - (rectangle1.Width + (rectangle2.Height - rectangle1.Height) / 2.0));
            return iX > rectangle1.X & iX < rectangle1.Right & iY > rectangle1.Top & iY < rectangle1.Bottom;
        }

        void tsAdvDBEdit_Click(object sender, EventArgs e)
        {
            FloatTop(false);
            new frmDBEdit().ShowDialog(this);
            FloatTop(true);
        }

        void tsAdvFreshInstall_Click(object sender, EventArgs e)
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

        void tsAdvResetTips_Click(object sender, EventArgs e) => MidsContext.Config.Tips = new Tips();

        /*void tsBug_Click(object sender, EventArgs e)
        {
            string at = "ATFailed";
            string pri = "PriFailed";
            string sec = "SecFailed";
            try
            {
                at = MidsContext.Character.Archetype.DisplayName;
                pri = MidsContext.Character.Powersets[0].DisplayName;
                sec = MidsContext.Character.Powersets[1].DisplayName;
            }
            catch (Exception)
            {
                // ignored
            }

            clsXMLUpdate.BugReport(at, pri, sec, string.Empty);
        }*/

        void tsClearAllEnh_Click(object sender, EventArgs e)
        {
            FloatTop(false);
            if (MessageBox.Show(
                    "Really clear all slotted enhancements?\r\nThis will not clear the alternate slotting, only the currently active slots.",
                    "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                for (int index1 = 0; index1 <= MidsContext.Character.CurrentBuild.Powers.Count - 1; ++index1)
                {
                    for (int index2 = 0; index2 <= MidsContext.Character.CurrentBuild.Powers[index1].Slots.Length - 1; ++index2)
                        MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh = -1;
                }

                DoRedraw();
                RefreshInfo();
            }

            FloatTop(true);
        }

        void tsConfig_Click(object sender, EventArgs e)
        {
            FloatTop(false);
            frmMain iParent = this;
            frmCalcOpt frmCalcOpt = new frmCalcOpt(ref iParent);
            if (frmCalcOpt.ShowDialog(this) == DialogResult.OK)
            {
                UpdateControls();
                UpdateOtherFormsFonts();
            }

            frmCalcOpt.Dispose();
            tsIODefault.Text = "Default (" + (MidsContext.Config.I9.DefaultIOLevel + 1) + ")";
            FloatTop(true);
        }

        void tsKoFi_Click(object sender, EventArgs e) => clsXMLUpdate.KoFi();
        void tsPatreon_Click(object sender, EventArgs e) => clsXMLUpdate.Patreon();
        void tsBugReportCrytilis_Click(object sender, EventArgs e) => clsXMLUpdate.BugReportCrytilis();

        void tsDynamic_Click(object sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon == null)
                return;
            MidsContext.Config.BuildMode = Enums.dmModes.Dynamic;
            MidsContext.Character.ResetLevel();
            PowerModified(markModified: false);
        }

        void OnRelativeClick(Enums.eEnhRelative newVal)
        {
            if (MainModule.MidsController.Toon == null)
                return;
            if (MidsContext.Character.CurrentBuild.SetEnhRelativelevels(newVal))
                I9Picker.UI.Initial.RelLevel = newVal;
            info_Totals();
            DoRedraw();
        }

        void tsEnhToEven_Click(object sender, EventArgs e)
            => OnRelativeClick(Enums.eEnhRelative.Even);

        void tsEnhToMinus1_Click(object sender, EventArgs e)
            => OnRelativeClick(Enums.eEnhRelative.MinusOne);

        void tsEnhToMinus2_Click(object sender, EventArgs e)
            => OnRelativeClick(Enums.eEnhRelative.MinusTwo);

        void tsEnhToMinus3_Click(object sender, EventArgs e)
            => OnRelativeClick(Enums.eEnhRelative.MinusThree);

        void tsEnhToNone_Click(object sender, EventArgs e)
            => OnRelativeClick(Enums.eEnhRelative.None);

        void tsEnhToPlus1_Click(object sender, EventArgs e)
            => OnRelativeClick(Enums.eEnhRelative.PlusOne);

        void tsEnhToPlus2_Click(object sender, EventArgs e)
            => OnRelativeClick(Enums.eEnhRelative.PlusTwo);

        void tsEnhToPlus3_Click(object sender, EventArgs e)
            => OnRelativeClick(Enums.eEnhRelative.PlusThree);

        void tsEnhToPlus4_Click(object sender, EventArgs e)
            => OnRelativeClick(Enums.eEnhRelative.PlusFour);

        void tsEnhToPlus5_Click(object sender, EventArgs e)
            => OnRelativeClick(Enums.eEnhRelative.PlusFive);

        void OnGradePick(Enums.eEnhGrade grade)
        {
            if (MidsContext.Character == null)
                return;
            if (MidsContext.Character.CurrentBuild.SetEnhGrades(grade))
                I9Picker.UI.Initial.GradeID = grade;
            info_Totals();
            DoRedraw();
        }

        void tsEnhToDO_Click(object sender, EventArgs e)
            => OnGradePick(Enums.eEnhGrade.DualO);


        void tsEnhToSO_Click(object sender, EventArgs e)
            => OnGradePick(Enums.eEnhGrade.SingleO);

        void tsEnhToTO_Click(object sender, EventArgs e)
            => OnGradePick(Enums.eEnhGrade.TrainingO);

        void tsExport_Click(object sender, EventArgs e)
        {
            FloatTop(false);
            MidsContext.Config.LongExport = false;
            var frmForum1 = new frmForum
            {
                BackColor = BackColor,
                IBCancel =
                {
                    IA = drawing.pImageAttributes, ImageOff = drawing.bxPower[2].Bitmap, ImageOn = drawing.bxPower[3].Bitmap
                },
                IBExport =
                {
                    IA = drawing.pImageAttributes, ImageOff = drawing.bxPower[2].Bitmap, ImageOn = drawing.bxPower[3].Bitmap
                }
            };
            frmForum1.ShowDialog(this);
            FloatTop(true);
        }

        void tsExportDataLink_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(MidsCharacterFileFormat.MxDBuildSaveHyperlink(false, true), true);
            MessageBox.Show("The data link has been placed on the clipboard and is ready to paste.", "Export Done", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        bool exportDiscordInProgress;

        public async void tsExportDiscord_Click(object sender, EventArgs e)
        {
            void ShowConfigError(string field)
                => MessageBox.Show($"{field} must be filled out in configuration before discord exports will function",
                    "Discord is not configured");

            if (exportDiscordInProgress)
            {
                MessageBox.Show("Another discord export is in progress, might be stuck");
                return;
            }

            if (string.IsNullOrWhiteSpace(MidsContext.Config.DSelServer))
            {
                ShowConfigError("Server");
                return;
            }

            if (string.IsNullOrWhiteSpace(MidsContext.Config.DChannel))
            {
                // tired of typing this
                if (Debugger.IsAttached) MidsContext.Config.DChannel = "feature-testing";
                ShowConfigError("Channel");
                return;
            }

            if (string.IsNullOrWhiteSpace(MidsContext.Config.DNickName))
            {
                ShowConfigError("NickName");
                return;
            }

            if (!MidsContext.Config.CheckedStats.Any())
            {
                ShowConfigError("Stat Boxes");
                return;
            }

            exportDiscordInProgress = true;
            try
            {
                await Clshook.DiscordExport();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                exportDiscordInProgress = false;
            }
        }

        void tsExportLong_Click(object sender, EventArgs e)
        {
            FloatTop(false);
            MidsContext.Config.LongExport = true;
            frmForum frmForum1 = new frmForum
            {
                BackColor = BackColor,
                IBCancel =
                {
                    IA = drawing.pImageAttributes, ImageOff = drawing.bxPower[2].Bitmap, ImageOn = drawing.bxPower[3].Bitmap
                },
                IBExport =
                {
                    IA = drawing.pImageAttributes, ImageOff = drawing.bxPower[2].Bitmap, ImageOn = drawing.bxPower[3].Bitmap
                }
            };
            frmForum1.ShowDialog(this);
            FloatTop(true);
            MidsContext.Config.LongExport = false;
        }

        void tsFileNew_Click(object sender, EventArgs e) => command_New();

        void tsBuildRcv_Click(object sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon.Locked & FileModified)
            {
                FloatTop(false);
                DialogResult msgBoxResult = MessageBox.Show("Current hero/villain data will be discarded, are you sure?", "Question",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                FloatTop(true);
                if (msgBoxResult == DialogResult.No)
                    return;
            }

            FloatTop(false);
            if (DlgOpen.ShowDialog() == DialogResult.OK)
            {
                //DoOpen(DlgOpen.FileName);
                BuildRecover(DlgOpen.FileName);
            }
            FloatTop(true);
        }
        void tsFileOpen_Click(object sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon.Locked & FileModified)
            {
                FloatTop(false);
                DialogResult msgBoxResult = MessageBox.Show("Current hero/villain data will be discarded, are you sure?", "Question",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                FloatTop(true);
                if (msgBoxResult == DialogResult.No)
                    return;
            }

            FloatTop(false);
            if (DlgOpen.ShowDialog() == DialogResult.OK)
                DoOpen(DlgOpen.FileName);
            FloatTop(true);
        }

        void tsFilePrint_Click(object sender, EventArgs e)
            => new frmPrint().ShowDialog(this);

        void tsFileQuit_Click(object sender, EventArgs e)
            => Close();

        void tsFileSave_Click(object sender, EventArgs e)
            => doSave();

        void tsFileSaveAs_Click(object sender, EventArgs e)
            => doSaveAs();

        void tsFlipAllEnh_Click(object sender, EventArgs e)
        {
            MainModule.MidsController.Toon.FlipAllSlots();
            DoRedraw();
            RefreshInfo();
            FloatUpdate();
        }

        void tsHelp_Click(object sender, EventArgs e)
        {
            frmReadme frmReadme = new frmReadme(OS.GetApplicationPath() + "readme.rtf")
            {
                BtnClose =
                {
                    IA = drawing.pImageAttributes, ImageOff = drawing.bxPower[2].Bitmap, ImageOn = drawing.bxPower[3].Bitmap
                }
            };
            FloatTop(false);
            frmReadme.ShowDialog(this);
            FloatTop(true);
        }

        void tsHelperLong_Click(object sender, EventArgs e) => new FrmInputLevel(this, true, false).ShowDialog(this);

        void tsHelperLong2_Click(object sender, EventArgs e) => new FrmInputLevel(this, true, true).ShowDialog(this);

        void tsHelperShort_Click(object sender, EventArgs e) => new FrmInputLevel(this, false, false).ShowDialog(this);

        void tsHelperShort2_Click(object sender, EventArgs e) => new FrmInputLevel(this, false, true).ShowDialog(this);

        void tsImport_Click(object sender, EventArgs e) => command_ForumImport();

        void tsIODefault_Click(object sender, EventArgs e)
        {
            if (MidsContext.Character.CurrentBuild.SetIOLevels(MidsContext.Config.I9.DefaultIOLevel, false, false))
                I9Picker.LastLevel = MidsContext.Config.I9.DefaultIOLevel + 1;
            DoRedraw();
        }

        void tsIOMax_Click(object sender, EventArgs e)
        {
            if (MidsContext.Character.CurrentBuild.SetIOLevels(MidsContext.Config.I9.DefaultIOLevel, false, true))
                I9Picker.LastLevel = 50;
            DoRedraw();
        }

        void tsIOMin_Click(object sender, EventArgs e)
        {
            if (MidsContext.Character.CurrentBuild.SetIOLevels(MidsContext.Config.I9.DefaultIOLevel, true, false))
                I9Picker.LastLevel = 10;
            DoRedraw();
        }

        void tsLevelUp_Click(object sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon == null)
                return;
            MidsContext.Config.BuildMode = Enums.dmModes.LevelUp;
            MidsContext.Character.ResetLevel();
            PowerModified(markModified: true);
        }

        void tsRecipeViewer_Click(object sender, EventArgs e) => FloatRecipe(true);

        void tsDPSCalc_Click(object sender, EventArgs e) => FloatDPSCalc(true);

        void tsRemoveAllSlots_Click(object sender, EventArgs e)
        {
            FloatTop(false);
            if (MessageBox.Show(
                    "Really remove all slots?\r\nThis will not remove the slots granted automatically with powers, but will remove all the slots you placed manually.",
                    "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                for (int index = 0; index <= MidsContext.Character.CurrentBuild.Powers.Count - 1; ++index)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[index].SlotCount > 1)
                        MidsContext.Character.CurrentBuild.Powers[index].Slots =
                            MidsContext.Character.CurrentBuild.Powers[index].Slots.Take(1).ToArray();
                }

                DoRedraw();
                MidsContext.Character.ResetLevel();
                // if all slots are removed, changes are they don't want to be prompted to save, unless something else is changed/added
                PowerModified(markModified: false);
                RefreshInfo();
            }

            FloatTop(true);
        }

        void tsSetFind_Click(object sender, EventArgs e)
            => FloatSetFinder(true);

        void tsForumLink(object sender, EventArgs e)
            => clsXMLUpdate.GoToForums();

        void tsCrytilisLink(object sender, EventArgs e)
            => clsXMLUpdate.GoToGitHub();

        private void AutoUpdater_ApplicationExitEvent()
        {
            Text = @"Closing application...";
            Thread.Sleep(5000);
            Application.Exit();
        }

        void tsUpdateCheck_Click(object sender, EventArgs e) => clsXMLUpdate.CheckUpdate();

        /*private void ForceReinstall()
        {
            try
            {
                var path = MidsContext.Config.UpdatePath;
                if (string.IsNullOrWhiteSpace(path))
                {
                    MessageBox.Show("Unable to check for updates, no update path found");
                    return;
                }

                // prove it is also a valid URI
                if (Uri.TryCreate(path, UriKind.Absolute, out var _)) {
                    AutoUpdater.Mandatory = true;
                    AutoUpdater.UpdateMode = Mode.ForcedDownload;
                    AutoUpdater.Start(path);
                } else
                    MessageBox.Show("Unable to check for updates, bad update path found : " + path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }*/

        //void tsForceReinstall_Click(object sender, EventArgs e) => ForceReinstall();

        void tsViewSelected()
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
            }
        }

        void tsView2Col_Click(object sender, EventArgs e)
        {
            tsView3Col.Checked = false;
            tsView4Col.Checked = false;
            tsView2Col.Checked = true;
            setColumns(2);
        }

        void tsView3Col_Click(object sender, EventArgs e)
        {
            tsView2Col.Checked = false;
            tsView4Col.Checked = false;
            tsView3Col.Checked = true;
            setColumns(3);
        }

        void tsView4Col_Click(object sender, EventArgs e)
        {
            tsView2Col.Checked = false;
            tsView3Col.Checked = false;
            tsView4Col.Checked = true;
            setColumns(4);
        }

        void tsViewActualDamage_New_Click(object sender, EventArgs e)
        {
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.Numeric;
            SetDamageMenuCheckMarks();
            DisplayFormatChanged();
        }

        void tsViewData_Click(object sender, EventArgs e) => FloatData(true);

        void tsViewDPS_New_Click(object sender, EventArgs e)
        {
            MidsContext.Config.DamageMath.ReturnValue = ConfigData.EDamageReturn.DPS;
            SetDamageMenuCheckMarks();
            DisplayFormatChanged();
        }

        void tsViewGraphs_Click(object sender, EventArgs e) => FloatStatGraph(true);

        void tsViewIOLevels_Click(object sender, EventArgs e)
        {
            MidsContext.Config.I9.HideIOLevels = !MidsContext.Config.I9.HideIOLevels;
            tsViewIOLevels.Checked = !MidsContext.Config.I9.HideIOLevels;
            DoRedraw();
        }

        void tsViewRelative_Click(object sender, EventArgs e)
        {
            MidsContext.Config.ShowEnhRel = !MidsContext.Config.ShowEnhRel;
            tsViewRelative.Checked = MidsContext.Config.ShowEnhRel;
            DoRedraw();
        }

        void tsViewSetCompare_Click(object sender, EventArgs e)
            => FloatCompareGraph(true);

        void tsViewSets_Click(object sender, EventArgs e)
        {
            if (MainModule.MidsController.Toon == null)
                return;
            FloatSets(true);
        }

        void tsViewSlotLevels_Click(object sender, EventArgs e)
        {
            MidsContext.Config.ShowSlotLevels = !MidsContext.Config.ShowSlotLevels;
            tsViewSlotLevels.Checked = MidsContext.Config.ShowSlotLevels;
            ibSlotLevels.Checked = MidsContext.Config.ShowSlotLevels;
            DoRedraw();
        }

        void tsViewTotals_Click(object sender, EventArgs e) => FloatTotals(true);

        void txtName_TextChanged(object sender, EventArgs e)
        {
            if (NoUpdate)
                return;
            MidsContext.Character.Name = txtName.Text;
            DisplayName();
        }

        internal void UnSetMiniList()
        {
            fMini?.Dispose();
            fMini = null;
            GC.Collect();
        }

        void UpdateColours(bool skipDraw = false)
        {
            //myDataView.DrawVillain = !MidsContext.Character.IsHero();
            bool draw;
            draw = I9Picker.ForeColor.R != 96;
            BackColor = Color.FromArgb(0, 0, 0);
            lblATLocked.BackColor = Color.FromArgb(128, 128, byte.MaxValue);
            I9Picker.ForeColor = Color.FromArgb(96, 48, byte.MaxValue);

            I9Picker.BackColor = BackColor;
            I9Popup.BackColor = Color.Black;
            I9Popup.ForeColor = I9Picker.ForeColor;
            myDataView.BackColor = BackColor;
            //var font = new Font(llPrimary.Font.FontFamily, MidsContext.Config.RtFont.PairedBase, FontStyle.Bold, GraphicsUnit.Point);
            var font = new Font("Arial", 12f, FontStyle.Bold, GraphicsUnit.Pixel);
            var toColor = new Control[]
            {
                llPrimary, llSecondary, llPool0, llPool1, llPool2, llPool3, llAncillary, lblName, lblAT, lblOrigin, lblHero, pnlGFX
            };
            foreach (var colorItem in toColor)
            {
                colorItem.BackColor = BackColor;
                if (!(colorItem is ListLabelV2 ll))
                    continue;
                UpdateLLColours(ll);
                ll.Font = font;
            }

            var toOtherColor = new Control[]
            {
                lblLocked0, lblLocked1, lblLocked2, lblLocked3, lblLockedAncillary, lblLockedSecondary //, this.lblATLocked
            };
            foreach (var colorItem in toOtherColor)
            {
                colorItem.BackColor = lblATLocked.BackColor;
            }

            var ibs = new[]
            {
                ibSets, ibPvX, incarnateButton, tempPowersButton, petsButton, accoladeButton, heroVillain, ibVetPools, ibTotals, ibMode, ibSlotLevels,
                ibPopup, ibRecipe, ibAccolade
            };
            foreach (var ib in ibs)
                ib.SetImages(drawing.pImageAttributes, drawing.bxPower[2].Bitmap, drawing.bxPower[3].Bitmap);


            if (!draw)
                return;
            if (!skipDraw)
                DoRedraw();
            UpdateDMBuffer();
            pbDynMode.Refresh();
        }

        void UpdateControls(bool ForceComplete = false)
        {
            if (this.loading)
                return;

            NoUpdate = true;
            Archetype[] all = Array.FindAll(DatabaseAPI.Database.Classes, GetPlayableClasses);
            var cbAT = new ComboBoxT<Archetype>(this.cbAT);
            if (ComboCheckAT(all))
            {
                cbAT.BeginUpdate();
                cbAT.Clear();
                cbAT.AddRange(all);
                cbAT.EndUpdate();
            }

            if (cbAT.SelectedItem == null)
                cbAT.SelectedItem = MidsContext.Character.Archetype;
            //else if (Operators.ConditionalCompareObjectNotEqual(NewLateBinding.LateGet(cbAT.SelectedItem, null, "Idx", new object[0], null, (System.Type[])null, null), MidsContext.Character.Archetype.Idx, false))
            else if (cbAT.SelectedItem.Idx != MidsContext.Character.Archetype.Idx)
                cbAT.SelectedItem = MidsContext.Character.Archetype;
            ibPvX.Checked = MidsContext.Config.Inc.DisablePvE;
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
                cbOrigin.SelectedIndex = MidsContext.Character.Origin < cbOrigin.Items.Count ? MidsContext.Character.Origin : 0;
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
            cbPool0.SelectedIndex = MainModule.MidsController.Toon.PoolToComboID(0, MidsContext.Character.Powersets[3]?.nID ?? -1);
            cbPool1.SelectedIndex = MainModule.MidsController.Toon.PoolToComboID(1, MidsContext.Character.Powersets[4]?.nID ?? -1);
            cbPool2.SelectedIndex = MainModule.MidsController.Toon.PoolToComboID(2, MidsContext.Character.Powersets[5]?.nID ?? -1);
            cbPool3.SelectedIndex = MainModule.MidsController.Toon.PoolToComboID(3, MidsContext.Character.Powersets[6]?.nID ?? -1);
            IPowerset[] powersetIndexes = DatabaseAPI.GetPowersetIndexes(MidsContext.Character.Archetype, Enums.ePowerSetType.Ancillary);
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
            //llPrimary.Font = new Font(llPrimary.Font.FontFamily, MidsContext.Config.RtFont.PairedBase, FontStyle.Bold, GraphicsUnit.Point);
            llPrimary.Font = new Font("Arial", 12f, FontStyle.Bold, GraphicsUnit.Pixel);
            llSecondary.Font = llPrimary.Font;
            int num1 = llPrimary.Items.Length - 1;
            for (int index = 0; index <= num1; ++index)
                llPrimary.Items[index].Bold = MidsContext.Config.RtFont.PairedBold;
            int num2 = llSecondary.Items.Length - 1;
            for (int index = 0; index <= num2; ++index)
                llSecondary.Items[index].Bold = MidsContext.Config.RtFont.PairedBold;
            heroVillain.Checked = !MidsContext.Character.IsHero();
            dvAnchored.SetLocation(
                new Point(llPrimary.Left, llPrimary.Top + raGreater(llPrimary.SizeNormal.Height, llSecondary.SizeNormal.Height) + 5),
                ForceComplete);
            llPrimary.SuspendRedraw = false;
            llSecondary.SuspendRedraw = false;
            if (myDataView != null && drawing.InterfaceMode == Enums.eInterfaceMode.Normal & myDataView.TabPageIndex == 2)
                dvAnchored_TabChanged(myDataView.TabPageIndex);
            if (MidsContext.Config.BuildMode == Enums.dmModes.LevelUp)
            {
                UpdateDMBuffer();
                pbDynMode.Refresh();
            }

            DoResize();
            NoUpdate = false;
        }

        void UpdateDMBuffer()
        {
            if (drawing == null || MainModule.MidsController.Toon == null)
                return;
            if (dmBuffer == null)
                dmBuffer = new ExtendedBitmap(pbDynMode.Width, pbDynMode.Height);
            Enums.ePowerState ePowerState;
            string iStr;
            if (MidsContext.Config.BuildMode == Enums.dmModes.Dynamic)
            {
                if (MidsContext.Config.BuildOption == Enums.dmItem.Slot)
                {
                    ePowerState = Enums.ePowerState.Open;
                    iStr = "Power / Slot";
                }
                else
                {
                    ePowerState = Enums.ePowerState.Used;
                    iStr = "Power Only";
                }
            }
            else if (DatabaseAPI.Database.Levels[MidsContext.Character.Level].LevelType() == Enums.dmItem.Power)
            {
                ePowerState = Enums.ePowerState.Used;
                iStr = "Power";
            }
            else
            {
                int slotsRemaining = MainModule.MidsController.Toon.SlotsRemaining;
                ePowerState = Enums.ePowerState.Open;
                iStr = slotsRemaining + " Slot";
                if (slotsRemaining > 1)
                    iStr += "s";
            }

            if (MainModule.MidsController.Toon.Complete)
            {
                if (MidsContext.Config.BuildMode == Enums.dmModes.LevelUp)
                    ePowerState = Enums.ePowerState.Used;
                iStr = "Complete";
            }

            Rectangle rectangle = new Rectangle();
            ref Rectangle local = ref rectangle;
            Size size = drawing.bxPower[(int)ePowerState].Size;
            int width = size.Width;
            size = drawing.bxPower[(int)ePowerState].Size;
            int height1 = size.Height;
            local = new Rectangle(0, 0, width, height1);
            Rectangle destRect = new Rectangle(0, 0, pbDynMode.Width, pbDynMode.Height);
            StringFormat stringFormat = new StringFormat();
            Font bFont = new Font(Font.FontFamily, Font.Size, FontStyle.Bold, GraphicsUnit.Pixel);
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            if (ePowerState == Enums.ePowerState.Open)
                dmBuffer.Graphics.DrawImage(drawing.bxPower[(int)ePowerState].Bitmap, destRect, 0, 0, rectangle.Width, rectangle.Height,
                    GraphicsUnit.Pixel);
            else
                dmBuffer.Graphics.DrawImage(drawing.bxPower[(int)ePowerState].Bitmap, destRect, 0, 0, rectangle.Width, rectangle.Height,
                    GraphicsUnit.Pixel, drawing.pImageAttributes);
            float height2 = bFont.GetHeight(dmBuffer.Graphics) + 2f;
            RectangleF Bounds = new RectangleF(0.0f, (float)((pbDynMode.Height - (double)height2) / 2.0), pbDynMode.Width, height2);
            Graphics graphics = dmBuffer.Graphics;
            clsDrawX.DrawOutlineText(iStr, Bounds, Color.WhiteSmoke, Color.FromArgb(192, 0, 0, 0), bFont, 1f, graphics);
        }

        void UpdateDynamicModeInfo()
        {
            if (MidsContext.Config.BuildMode == Enums.dmModes.Dynamic)
            {
                tsDynamic.Checked = true;
                tsLevelUp.Checked = false;
            }
            else
            {
                tsDynamic.Checked = false;
                tsLevelUp.Checked = true;
            }

            ibMode.TextOff = MidsContext.Config.BuildMode != Enums.dmModes.Dynamic
                ? (!MainModule.MidsController.Toon.Complete ? "Level-Up: " + (MidsContext.Character.Level + 1) : "Level-Up")
                : "Dynamic";
        }

        void UpdateLLColours(ListLabelV2 iList)
        {
            iList.UpdateTextColors(ListLabelV2.LLItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
            iList.UpdateTextColors(ListLabelV2.LLItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
            iList.UpdateTextColors(ListLabelV2.LLItemState.Selected, MidsContext.Config.RtFont.ColorPowerTaken);
            iList.UpdateTextColors(ListLabelV2.LLItemState.SelectedDisabled, MidsContext.Config.RtFont.ColorPowerTakenDark);
            iList.UpdateTextColors(ListLabelV2.LLItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
            iList.HoverColor = MidsContext.Config.RtFont.ColorPowerHighlight;

            iList.ScrollBarColor = Color.FromArgb(64, 64, byte.MaxValue);
            iList.ScrollButtonColor = Color.FromArgb(32, 32, byte.MaxValue);

        }

        void UpdateOtherFormsFonts()
        {
            if (this.fIncarnate != null)
            {
                frmIncarnate fIncarnate = this.fIncarnate;
                if (fIncarnate.Visible)
                {
                    fIncarnate.LLLeft.SuspendRedraw = true;
                    fIncarnate.LLRight.SuspendRedraw = true;
                    fIncarnate.LLLeft.Font = llPrimary.Font;
                    fIncarnate.LLRight.Font = llPrimary.Font;
                    fIncarnate.LLLeft.UpdateTextColors(ListLabelV2.LLItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
                    fIncarnate.LLLeft.UpdateTextColors(ListLabelV2.LLItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
                    fIncarnate.LLLeft.UpdateTextColors(ListLabelV2.LLItemState.Selected, MidsContext.Config.RtFont.ColorPowerTaken);
                    fIncarnate.LLLeft.UpdateTextColors(ListLabelV2.LLItemState.SelectedDisabled,
                        MidsContext.Config.RtFont.ColorPowerTakenDark);
                    fIncarnate.LLLeft.UpdateTextColors(ListLabelV2.LLItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
                    fIncarnate.LLLeft.HoverColor = MidsContext.Config.RtFont.ColorPowerHighlight;
                    fIncarnate.LLRight.UpdateTextColors(ListLabelV2.LLItemState.Enabled, MidsContext.Config.RtFont.ColorPowerAvailable);
                    fIncarnate.LLRight.UpdateTextColors(ListLabelV2.LLItemState.Disabled, MidsContext.Config.RtFont.ColorPowerDisabled);
                    fIncarnate.LLRight.UpdateTextColors(ListLabelV2.LLItemState.Selected, MidsContext.Config.RtFont.ColorPowerTaken);
                    fIncarnate.LLRight.UpdateTextColors(ListLabelV2.LLItemState.SelectedDisabled,
                        MidsContext.Config.RtFont.ColorPowerTakenDark);
                    fIncarnate.LLRight.UpdateTextColors(ListLabelV2.LLItemState.Invalid, Color.FromArgb(byte.MaxValue, 0, 0));
                    fIncarnate.LLRight.HoverColor = MidsContext.Config.RtFont.ColorPowerHighlight;
                    int num1 = fIncarnate.LLLeft.Items.Length - 1;
                    for (int index = 0; index <= num1; ++index)
                        fIncarnate.LLLeft.Items[index].Bold = MidsContext.Config.RtFont.PairedBold;
                    int num2 = fIncarnate.LLRight.Items.Length - 1;
                    for (int index = 0; index <= num2; ++index)
                        fIncarnate.LLRight.Items[index].Bold = MidsContext.Config.RtFont.PairedBold;
                    fIncarnate.LLLeft.SuspendRedraw = false;
                    fIncarnate.LLRight.SuspendRedraw = false;
                    fIncarnate.LLLeft.Refresh();
                    fIncarnate.LLRight.Refresh();
                }
            }

            if (this.fTemp != null)
            {
                frmTemp fTemp = this.fTemp;
                if (fTemp.Visible)
                    fTemp.UpdateFonts(llPrimary.Font);
            }

            if (this.fAccolade == null)
                return;
            frmAccolade fAccolade = this.fAccolade;
            if (fAccolade.Visible)
                fAccolade.UpdateFonts(llPrimary.Font);
        }

        void UpdatePowerList(ListLabelV2 llPower)
        {
            llPower.SuspendRedraw = true;
            if (llPower.Items.Length == 0)
                llPower.AddItem(new ListLabelV2.ListLabelItemV2("Nothing", ListLabelV2.LLItemState.Disabled, -1, -1, -1, string.Empty));
            int num = llPower.Items.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                ListLabelV2.ListLabelItemV2 listLabelItemV2 = llPower.Items[index];
                if (!(listLabelItemV2.nIDSet > -1 & listLabelItemV2.IDXPower > -1))
                    continue;
                string message = string.Empty;
                listLabelItemV2.ItemState = MainModule.MidsController.Toon.PowerState(listLabelItemV2.nIDPower, ref message);
                listLabelItemV2.Italic = listLabelItemV2.ItemState == ListLabelV2.LLItemState.Invalid;
                listLabelItemV2.Bold = MidsContext.Config.RtFont.PairedBold;
            }

            llPower.SuspendRedraw = false;
        }

        void UpdatePowerLists()
        {
            bool noPrimary = false;
            if (this.llPrimary.Items.Length == 0)
                noPrimary = true;
            else if (this.llPrimary.Items[this.llPrimary.Items.Length - 1].nIDSet != MidsContext.Character.Powersets[0].nID)
                noPrimary = true;
            if (llSecondary.Items.Length == 0)
                noPrimary = true;
            else if (llSecondary.Items[llSecondary.Items.Length - 1].nIDSet != MidsContext.Character.Powersets[1].nID)
                noPrimary = true;
            bool noAncillary = false;
            if (llAncillary.Items.Length == 0 || MidsContext.Character.Powersets[7] == null)
                noAncillary = true;
            else if (llAncillary.Items[llAncillary.Items.Length - 1].nIDSet != MidsContext.Character.Powersets[7].nID)
                noAncillary = true;
            if (noPrimary)
            {
                ListLabelV2 llPrimary = this.llPrimary;
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

        void GameImport(string buildString)
        {
            try
            {
                ImportFromBuildsave importHandle = new ImportFromBuildsave(buildString);
                List<PowerEntry>? listPowers = importHandle.Parse();

                if (listPowers == null) return;

                InjectBuild(buildString, listPowers, importHandle.GetPowersets(), importHandle.GetCharacterInfo());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\r\n\r\n" + e.StackTrace);
            }
        }

        void BuildRecover(string buildString)
        {
            try
            {
                PlainTextParser importHandle = new PlainTextParser(buildString);
                List<PowerEntry>? listPowers = importHandle.Parse();

                if (listPowers == null) return;

                InjectBuild(buildString, listPowers, importHandle.GetPowersets(), importHandle.GetCharacterInfo());
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\r\n\r\n" + e.StackTrace);
            }
        }

        private int CountPools(UniqueList<string> listPowersets)
        {
            return listPowersets.Where(ps => ps.IndexOf("Pool.", StringComparison.OrdinalIgnoreCase) == 0).ToArray().Length;
        }

        private void PadPowerPools(ref UniqueList<string> listPowersets)
        {
            int nbPools = CountPools(listPowersets);

            if (nbPools == 4) return;

            string[] pickedPowerPools = listPowersets.Where(ps => ps.IndexOf("Pool.", StringComparison.OrdinalIgnoreCase) == 0).ToArray();
            string[] dbPowerPools = Database.Instance.Powersets
                .Where(ps => ps.FullName.IndexOf("Pool.", StringComparison.OrdinalIgnoreCase) == 0 && !pickedPowerPools.Contains(ps.FullName))
                .OrderBy(e => e.DisplayName)
                .Select(e => e.FullName)
                .ToArray();

            for (int i = 0; i < 4 - nbPools; i++)
            {
                listPowersets.Add(dbPowerPools[i]);
            }
        }

        private void FilterVEATPools(ref UniqueList<string> listPowersets)
        {
            if (listPowersets.Contains("Training_Gadgets.Bane_Spider_Training") || listPowersets.Contains("Training_Gadgets.Crab_Spider_Training"))
            {
                listPowersets.FromList(listPowersets.Where(e => e != "Training_Gadgets.Training_and_Gadgets").ToList());
                return;
            }

            if (listPowersets.Contains("Widow_Training.Fortunata_Training") || listPowersets.Contains("Widow_Training.Night_Widow_Training"))
            {
                listPowersets.FromList(listPowersets.Where(e => e != "Widow_Training.Widow_Training" && e != "Teamwork.Teamwork").ToList());
                return;
            }
        }

        private void InjectBuild(string buildFile, List<PowerEntry> listPowers, UniqueList<string> listPowersets, RawCharacterInfo characterInfo)
        {
            // Need to pad pools powerlist so there are 4
            // So epic pools doesn't end up shown as a regular pool...
            PadPowerPools(ref listPowersets);
            FilterVEATPools(ref listPowersets);

            string toBlameSet = String.Empty;
            MessageBox.Show(String.Join(", ", listPowersets.ToArray()));
            MidsContext.Character.LoadPowersetsByName2(listPowersets, ref toBlameSet);
            MidsContext.Character.CurrentBuild.LastPower = MidsContext.Character.GetPowersByLevel(characterInfo.Level - 1);

            List<PowerEntry> powerEntryList = listPowers.OrderBy(x => x.Level).ToList();
            int k = 0;
            int pickedSlots = 0;
            try
            {
                for (k = 0; k < listPowers.Count; k++)
                {
                    if (!powerEntryList[k].PowerSet.FullName.Contains("Inherent"))
                    {
                        PowerPickedNoRedraw(powerEntryList[k].NIDPowerset, powerEntryList[k].NIDPower);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
            }

            PowerEntry p;
            int i;
            SlotLevelQueue sl = new SlotLevelQueue();
            try
            {
                foreach (PowerEntry pe in MidsContext.Character.CurrentBuild.Powers)
                {
                    if (pe.Power == null) continue; // Not picked power will be in the list, but not instanciated!
                    var pList = powerEntryList.Where(e => pe.Power.FullName == e.Power.FullName).ToArray();
                    if (pList.Length == 0) continue;

                    p = pList.First();
                    while (pe.Slots.Length < p.Slots.Length)
                    {
                        pe.AddSlot(MidsContext.Character.MaxLevel);
                    }

                    p.Slots.CopyTo(pe.Slots, 0);
                    for (i = 0; i < pe.Slots.Length; i++)
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

            fixStatIncludes();
            FileModified = false;
            MidsContext.Character.Lock();
            MidsContext.Character.PoolShuffle();
            I9Gfx.OriginIndex = MidsContext.Character.Origin;
            MidsContext.Config.LastFileName = buildFile;
            LastFileName = buildFile;
            SetTitleBar();

            int idx = -1;
            if (MidsContext.Config.BuildMode == Enums.dmModes.Dynamic)
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

            if (idx > -1 & idx <= MidsContext.Character.CurrentBuild.Powers.Count)
            {
                MidsContext.Character.RequestedLevel = MidsContext.Character.CurrentBuild.Powers[idx].Level;
                MidsContext.Character.SetLevelTo(MidsContext.Character.CurrentBuild.Powers[idx].Level);
            }

            return;

            // Update slots counter... maybe.
            // Turns out all this block is not needed. (I think)
            /*
            int index = -1;
            MainModule.MidsController.Toon.Complete = !sl.IsValidNext();
            fixStatIncludes();
            FileModified = false;
            if (MidsContext.Config.BuildMode == Enums.dmModes.Dynamic)
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
    }
}