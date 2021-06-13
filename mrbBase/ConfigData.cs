using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace mrbBase
{
    public interface ISerialize
    {
        string Extension { get; }
        string Serialize(object o);
        T Deserialize<T>(string x);
    }

    public class ConfigData
    {
        public enum EDamageMath
        {
            Minimum,
            Average,
            Max
        }

        public enum EDamageReturn
        {
            Numeric,
            DPS,
            DPA
        }

        public enum PrintOptionProfile
        {
            None,
            SinglePage,
            MultiPage
        }

        public enum ETotalsWindowTitleStyle
        {
            Generic,
            CharNameAtPowersets,
            BuildFileAtPowersets,
            CharNameBuildFile
        }

        private const string header = "Mids' Hero Designer Config V2";

        private const string OverrideNames = "Mids' Hero Designer Comparison Overrides";

        public readonly short[] DragDropScenarioAction =
        {
            3, 0, 5, 0, 3, 5, 0, 0, 5, 0, 2, 3, 0, 2, 2, 0, 0, 0, 0, 0
        };

        private string _defaultSaveFolderOverride;
        private Size _lastSize = new Size(1072, 760);
        public Enums.eSpeedMeasure SpeedFormat = Enums.eSpeedMeasure.MilesPerHour;
        public string UpdatePath = "https://midsreborn.com/mids_updates/app/update_manifest.xml";
        public string DbUpdatePath = "https://midsreborn.com/mids_updates/db/update_manifest.xml";
        public string AppChangeLog { get; set; }
        public string DbChangeLog { get; set; }
        public bool CoDEffectFormat = false;
        private ConfigData() : this(true, "")
        {
        }

        private ConfigData(bool deserializing, string iFilename)
        {
            DamageMath.Calculate = EDamageMath.Average;
            DamageMath.ReturnValue = EDamageReturn.Numeric;
            I9.DefaultIOLevel = 49;
            TotalsWindowTitleStyle = ETotalsWindowTitleStyle.Generic;
            RtFont.SetDefault();
            Tips = new Tips();
            Export = new ExportConfig();
            CompOverride = Array.Empty<Enums.CompOverride>();
            if (deserializing) return;
            if (iFilename != Files.FNameJsonConfig && File.Exists(iFilename))
            {
                try
                {
                    LegacyForMigration(iFilename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Message: {ex.Message}\r\nTrace: {ex.StackTrace}");
                }
            }

            TeamMembers = new Dictionary<string, int>();
            Registered = 0;
            DiscordAuthorized = false;
            InitializeComponent();
        }

        // these properties require setters for deserialization
        public SDamageMath DamageMath { get; } = new SDamageMath();
        public IncludeExclude Inc { get; } = new IncludeExclude();
        public Si9 I9 { get; } = new Si9();
        public FontSettings RtFont { get; } = new FontSettings();
        public Dictionary<string, int> TeamMembers { get; } = new Dictionary<string, int>();

        public Size LastSize
        {
            get => _lastSize;
            set => _lastSize = value;
        }

        public bool UseOldTotalsWindow { get; set; }
        public float BaseAcc { get; set; } = 0.75f;
        public bool DoNotUpdateFileAssociation { get; set; }
        public int ExempHigh { get; set; } = 50;
        public int TeamSize { get; set; } = 1;
        public int ExempLow { get; set; } = 50;
        public int ForceLevel { get; set; } = 50;
        public int ExportScheme { get; set; } = 1;
        public int ExportTarget { get; set; } = 1;
        public bool DisableDataDamageGraph { get; private set; }
        public bool DisableVillainColors { get; set; }
        public bool IsInitialized { get; set; }
        public int Columns { get; set; } = 3;
        public PrintOptionProfile PrintProfile { get; set; } = PrintOptionProfile.SinglePage;
        public bool DisablePrintProfileEnh { get; set; }
        public string LastPrinter { get; set; } = string.Empty;
        public bool DisableLoadLastFileOnStart { get; set; }
        public string LastFileName { get; set; } = string.Empty;
        public Enums.eEnhGrade CalcEnhOrigin { get; set; } = Enums.eEnhGrade.SingleO;
        public Enums.eEnhRelative CalcEnhLevel { get; set; } = Enums.eEnhRelative.Even;
        public Enums.eDDGraph DataGraphType { get; set; } = Enums.eDDGraph.Both;
        public Enums.GraphStyle StatGraphStyle { get; set; } = Enums.GraphStyle.Stacked;
        public Enums.CompOverride[] CompOverride { get; set; }

        public bool DiscordAuthorized { get; set; }
        public int Registered { get; set; } = 0;
        public bool DisableDesaturateInherent { get; set; }
        public Enums.dmModes BuildMode { get; set; } = Enums.dmModes.Normal;
        public Enums.dmItem BuildOption { get; set; } = Enums.dmItem.Slot;
        public bool DisableShowPopup { get; set; }
        public bool DisableAlphaPopup { get; set; }
        public bool DisableRepeatOnMiddleClick { get; set; }
        public bool DisableExportHex { get; set; }
        private static ConfigData _current { get; set; }

        public bool ExportBonusTotals { get; set; }
        public bool ExportBonusList { get; set; }
        public bool NoToolTips { get; set; }
        public bool DataDamageGraphPercentageOnly { get; private set; }
        public bool CheckForUpdates { get; set; }
        public Enums.eVisibleSize DvState { get; set; }
        public Enums.eSuppress Suppression { get; set; }
        public bool UseArcanaTime { get; set; }
        public ExportConfig Export { get; }
        public bool PrintInColor { get; set; }
        public bool PrintHistory { get; set; }
        public bool SaveFolderChecked { get; set; }
        public bool ShowSlotLevels { get; set; }
        public bool ShowEnhRel { get; set; }
        public bool ShowRelSymbols { get; set; }
        public bool ShowSOLevels { get; set; }
        public bool EnhanceVisibility { get; set; }
        public Tips Tips { get; set; }
        public bool PopupRecipes { get; set; }
        public bool ShoppingListIncludesRecipes { get; set; }
        public bool ExportChunkOnly { get; set; }
        public bool LongExport { get; set; }
        public bool MasterMode { get; set; }
        public bool ShrinkFrmSets { get; set; }
        public bool ConvertOldDb { get; set; }
        public bool FirstRun { get; set; } = true;
        public string SourceDataPath { get; set; }
        public string ConversionDataPath { get; set; }
        public string DataPath { get; set; } = Path.Combine(Files.GetAssemblyLoc(), "Data\\Homecoming\\");

        public Enums.RewardCurrency PreferredCurrency = Enums.RewardCurrency.RewardMerit;

        public string DefaultSaveFolderOverride
        {
            get => _defaultSaveFolderOverride;
            set
            {
                var osDefault = OS.GetDefaultSaveFolder();
                if (string.IsNullOrWhiteSpace(value)
                    || Path.GetFullPath(value) == osDefault
                    || value == osDefault
                    || osDefault != null && Path.GetFullPath(osDefault) == value)
                {
                    _defaultSaveFolderOverride = null;
                    return;
                }

                _defaultSaveFolderOverride = value;
            }
        }

        public ETotalsWindowTitleStyle TotalsWindowTitleStyle { get; set; }

        internal static ConfigData Current
        {
            get
            {
                var configData = _current;
                return configData;
            }
        }

        private static void MigrateToSerializer(string mhdFn, ISerialize serializer)
        {
            var oldMethod = new ConfigData(false, mhdFn);
            oldMethod.InitializeComponent();
            var file = mhdFn + ".old";
            if (File.Exists(file))
                file += "2";
            File.Move(mhdFn, file);
            oldMethod.SaveConfig(serializer);
        }

        private static void GenerateDefaultConfig(ISerialize serializer, string fileName)
        {
            File.WriteAllText(fileName, string.Empty);
            _current = new ConfigData(false, fileName);
            SaveRawMhd(serializer, _current, fileName, new RawSaveResult(0, 0));
        }

        public static void Initialize(ISerialize serializer)
        {
            // migrate
            // force Mhd if it exists, then rename it
            var mhdFn = Files.GetConfigFilename(true);
            var fn = Files.GetConfigFilename(false);
            if (File.Exists(mhdFn))
            {
                MigrateToSerializer(mhdFn, serializer);
            }

            
            if (File.Exists(fn))
            {
                try
                {
                    var value = serializer.Deserialize<ConfigData>(File.ReadAllText(fn));
                    _current = value;
                }
                catch
                {
                    GenerateDefaultConfig(serializer, fn);
                    Initialize(serializer);
                }
            }
            else
            {
                GenerateDefaultConfig(serializer, fn);
                Initialize(serializer);
            }

            _current.InitializeComponent();
        }

        private void InitializeComponent()
        {
            if (string.IsNullOrWhiteSpace(UpdatePath))
                UpdatePath = "https://midsreborn.com/mids_updates/app/update_manifest.xml";

            if (string.IsNullOrWhiteSpace(DbUpdatePath))
                DbUpdatePath = "https://midsreborn.com/mids_updates/db/update_manifest.xml";
            if (string.IsNullOrWhiteSpace(DataPath))
                DataPath = Files.FDefaultPath;
            RelocateSaveFolder(false);
            try
            {
                LoadOverrides();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Message: {ex.Message}\r\nTrace: {ex.StackTrace}");
            }
        }

        private void LegacyForMigration(string iFilename)
        {
            //using (FileStream fileStream = new FileStream(iFilename, FileMode.Open, FileAccess.Read))
            {
                using var reader = new BinaryReader(File.Open(iFilename, FileMode.Open, FileAccess.Read));
                float version;
                switch (reader.ReadString())
                {
                    // legacy string, refers to something specific in files, do not change
                    case "Mids' Hero Designer Config":
                        version = 0.9f;
                        break;
                    // legacy string, refers to something specific in files, do not change
                    // here's something F# doesn't do easily(fallthrough where one branch has a when variable declared)
                    case "Mids' Hero Designer Config V2":
                    case { } x when x == header:
                        version = reader.ReadSingle();
                        break;
                    default:
                        MessageBox.Show("Config file was missing a header! Using defaults.");
                        reader.Close();
                        //fileStream.Close();
                        return;
                }

                /* Commenting out for now - will remove later
                this.DNickName = reader.ReadString();
                this.DSelServer = reader.ReadString();
                this.DChannel = reader.ReadString();*/
                NoToolTips = reader.ReadBoolean();
                BaseAcc = reader.ReadSingle();
                double num3 = reader.ReadSingle();
                double num4 = reader.ReadSingle();
                double num5 = reader.ReadSingle();
                double num6 = reader.ReadSingle();
                double num7 = reader.ReadSingle();
                CalcEnhLevel = (Enums.eEnhRelative)reader.ReadInt32();
                CalcEnhOrigin = (Enums.eEnhGrade)reader.ReadInt32();
                ExempHigh = reader.ReadInt32();
                ExempLow = reader.ReadInt32();
                Inc.DisablePvE = !reader.ReadBoolean();
                reader.ReadBoolean();
                DamageMath.Calculate = (EDamageMath)reader.ReadInt32();
                reader.ReadSingle();
                if (version < 1.24000000953674)
                    reader.ReadBoolean();
                else
                    reader.ReadInt32();
                DamageMath.ReturnValue = (EDamageReturn)reader.ReadInt32();
                DisableDataDamageGraph = !reader.ReadBoolean();
                DataDamageGraphPercentageOnly = reader.ReadBoolean();
                DataGraphType = (Enums.eDDGraph)reader.ReadInt32();
                ExportScheme = reader.ReadInt32();
                ExportTarget = reader.ReadInt32();
                if (version >= 1.24000000953674)
                {
                    ExportBonusTotals = reader.ReadBoolean();
                    ExportBonusList = reader.ReadBoolean();
                }

                //this._hideOriginEnhancements =
                reader.ReadBoolean();
                DisableVillainColors = !reader.ReadBoolean();
                CheckForUpdates = reader.ReadBoolean();
                Columns = reader.ReadInt32();
                _lastSize.Width = reader.ReadInt32();
                _lastSize.Height = reader.ReadInt32();
                DvState = (Enums.eVisibleSize)reader.ReadInt32();
                StatGraphStyle = (Enums.GraphStyle)reader.ReadInt32();
                if (version >= 1.0)
                    IsInitialized = !reader.ReadBoolean();
                if (version >= 1.10000002384186)
                    ForceLevel = reader.ReadInt32();
                if (version >= 1.20000004768372)
                {
                    I9.DefaultIOLevel = reader.ReadInt32();
                    if (I9.DefaultIOLevel > 49)
                        I9.DefaultIOLevel = 49;
                    I9.HideIOLevels = !reader.ReadBoolean();
                    I9.IgnoreEnhFX = !reader.ReadBoolean();
                    I9.IgnoreSetBonusFX = !reader.ReadBoolean();
                    I9.ExportIOLevels = reader.ReadBoolean();
                    I9.DisablePrintIOLevels = !reader.ReadBoolean();
                    I9.DisableExportCompress = !reader.ReadBoolean();
                    I9.DisableExportDataChunk = !reader.ReadBoolean();
                    I9.ExportStripEnh = reader.ReadBoolean();
                    I9.ExportStripSetNames = reader.ReadBoolean();
                    I9.ExportExtraSep = reader.ReadBoolean();
                    PrintInColor = reader.ReadBoolean();
                    //this._printScheme = 
                    reader.ReadInt32();
                }

                if (version >= 1.21000003814697)
                {
                    RtFont.PairedBase = reader.ReadSingle();
                    RtFont.PairedBold = reader.ReadBoolean();
                    RtFont.RTFBase = reader.ReadInt32();
                    RtFont.RTFBold = reader.ReadBoolean();
                    RtFont.ColorBackgroundHero = reader.ReadRGB();
                    RtFont.ColorBackgroundVillain = reader.ReadRGB();
                    RtFont.ColorEnhancement = reader.ReadRGB();
                    RtFont.ColorFaded = reader.ReadRGB();
                    RtFont.ColorInvention = reader.ReadRGB();
                    RtFont.ColorInventionInv = reader.ReadRGB();
                    RtFont.ColorText = reader.ReadRGB();
                    RtFont.ColorWarning = reader.ReadRGB();
                    RtFont.ColorPlName = reader.ReadRGB();
                    RtFont.ColorPlSpecial = reader.ReadRGB();
                    RtFont.PowersSelectBase = reader.ReadSingle();
                    RtFont.PowersSelectBold = reader.ReadBoolean();
                    RtFont.PowersBase = reader.ReadSingle();
                    RtFont.PowersBold = reader.ReadBoolean();
                }

                if (version >= 1.22000002861023)
                {
                    ShowSlotLevels = reader.ReadBoolean();
                    DisableLoadLastFileOnStart = !reader.ReadBoolean();
                    LastFileName = reader.ReadString();
                    RtFont.ColorPowerAvailable = reader.ReadRGB();
                    RtFont.ColorPowerDisabled = reader.ReadRGB();
                    RtFont.ColorPowerTakenHero = reader.ReadRGB();
                    RtFont.ColorPowerTakenDarkHero = reader.ReadRGB();
                    RtFont.ColorPowerHighlightHero = reader.ReadRGB();
                    RtFont.ColorPowerTakenVillain = reader.ReadRGB();
                    RtFont.ColorPowerTakenDarkVillain = reader.ReadRGB();
                    RtFont.ColorPowerHighlightVillain = reader.ReadRGB();
                }

                if (version >= 1.23000001907349)
                {
                    Tips = new Tips(reader);
                    DefaultSaveFolderOverride = reader.ReadString();
                }

                if (version >= 1.24000000953674)
                {
                    EnhanceVisibility = reader.ReadBoolean();
                    reader.ReadBoolean();
                    BuildMode = (Enums.dmModes)reader.ReadInt32();
                    BuildOption = (Enums.dmItem)reader.ReadInt32();
                    //this.UpdatePath =
                    reader.ReadString();
                    //if (string.IsNullOrEmpty(this.UpdatePath))
                    //    this.UpdatePath = "https://midsreborn.com/mids_updates/";
                }

                if (version >= 1.25)
                {
                    ShowEnhRel = reader.ReadBoolean();
                    ShowRelSymbols = reader.ReadBoolean();
                    DisableShowPopup = !reader.ReadBoolean();
                    if (version >= 1.32000005245209)
                        DisableAlphaPopup = !reader.ReadBoolean();
                    PopupRecipes = reader.ReadBoolean();
                    ShoppingListIncludesRecipes = reader.ReadBoolean();
                    PrintProfile = (PrintOptionProfile)reader.ReadInt32();
                    PrintHistory = reader.ReadBoolean();
                    LastPrinter = reader.ReadString();
                    DisablePrintProfileEnh = !reader.ReadBoolean();
                    DisableDesaturateInherent = !reader.ReadBoolean();
                    DisableRepeatOnMiddleClick = !reader.ReadBoolean();
                }

                if (version >= 1.25999999046326)
                    DisableExportHex = !reader.ReadBoolean();
                if (version >= 1.26999998092651)
                    SpeedFormat = (Enums.eSpeedMeasure)reader.ReadInt32();
                if (version >= 1.27999997138977)
                    SaveFolderChecked = reader.ReadBoolean();
                if (version >= 1.28999996185303)
                    UseArcanaTime = reader.ReadBoolean(); //this is correct
                CreateDefaultSaveFolder();
            }
        }

        public string GetSaveFolder()
        {
            return DefaultSaveFolderOverride ?? OS.GetDefaultSaveFolder();
        }

        public void CreateDefaultSaveFolder()
        {
            // if there is a save folder override, but it does not exist, wipe it out
            if (!string.IsNullOrWhiteSpace(DefaultSaveFolderOverride) && !Directory.Exists(DefaultSaveFolderOverride))
                DefaultSaveFolderOverride = null;
            var saveFolder = GetSaveFolder();
            if (Directory.Exists(saveFolder))
                return;
            Directory.CreateDirectory(saveFolder);
        }

        private void SaveRaw(ISerialize serializer, string iFilename)
        {
            SaveRawMhd(serializer, this, iFilename, null);
        }

        private void Save(ISerialize serializer, string iFilename)
        {
            SaveRaw(serializer, iFilename);
        }

        private void RelocateSaveFolder(bool manual)
        {
            if (!string.IsNullOrWhiteSpace(DefaultSaveFolderOverride) && Directory.Exists(DefaultSaveFolderOverride) &&
                (OS.GetDefaultSaveFolder() != DefaultSaveFolderOverride) & (!SaveFolderChecked | manual))
            {
                if (DefaultSaveFolderOverride.IndexOf(OS.GetMyDocumentsPath(), StringComparison.OrdinalIgnoreCase) > -1)
                {
                    SaveFolderChecked = true;
                    return;
                }

                if (Directory.Exists(DefaultSaveFolderOverride))
                {
                    if (MessageBox.Show(
                        "In order for Mids' Reborn : Designer to operate better in more recent versions of Windows, the recommended Save folder has been changed to appear inside the My Documents folder.\nThe application can automatically move your save folder and its contents to 'My Documents\\Hero & Villain Builds\\'.\nThis message will not appear again.\n\nMove your Save folder now?",
                        "Save Folder Location", MessageBoxButtons.YesNo, MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                    {
                        LastFileName = string.Empty;
                        var defaultSaveFolder = DefaultSaveFolderOverride;
                        DefaultSaveFolderOverride = null;
                        if (FileIO.CopyFolder(defaultSaveFolder, GetSaveFolder()))
                        {
                            MessageBox.Show(@"Save folder was moved!", "All Done", MessageBoxButtons.OK);
                        }
                        else
                        {
                            MessageBox.Show(@"Save folder couldn't be moved! Using old save folder instead.", "Whoops",
                                MessageBoxButtons.OK);
                            DefaultSaveFolderOverride = defaultSaveFolder;
                        }
                    }
                }
                else
                {
                    CreateDefaultSaveFolder();
                }
            }

            SaveFolderChecked = true;
        }

        // poorly named
        // saves both config.mhd, and compare.mhd
        public void SaveConfig(ISerialize serializer)
        {
            try
            {
                Save(serializer, Files.GetConfigFilename(false));
                SaveOverrides(serializer);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Message: {ex.Message}\r\nTrace: {ex.StackTrace}");
            }
        }

        private void LoadOverrides()
        {
            using var fileStream = new FileStream(Files.SelectDataFileLoad(Files.MxdbFileOverrides, DataPath), FileMode.Open, FileAccess.Read);
            using var binaryReader = new BinaryReader(fileStream);
            if (binaryReader.ReadString() != "Mids' Hero Designer Comparison Overrides")
            {
                MessageBox.Show("Overrides file was missing a header! Not loading powerset comparison overrides.");
            }
            else
            {
                CompOverride = new Enums.CompOverride[binaryReader.ReadInt32() + 1];
                for (var index = 0; index <= CompOverride.Length - 1; ++index)
                {
                    CompOverride[index].Powerset = binaryReader.ReadString();
                    CompOverride[index].Power = binaryReader.ReadString();
                    CompOverride[index].Override = binaryReader.ReadString();
                }
            }
        }

        public static (bool, T) LoadRawMhd<T>(ISerialize serializer, string fn)
        {
            return !File.Exists(fn) ? (false, default) : (true, serializer.Deserialize<T>(File.ReadAllText(fn)));
        }

        public static RawSaveResult SaveRawMhd(ISerialize serializer, object o, string fn, RawSaveResult lastSaveInfo)
        {
            var rootDir = Path.GetDirectoryName(fn);
            var targetFile = Path.Combine(rootDir ?? ".", $"{Path.GetFileNameWithoutExtension(fn)}.{serializer.Extension}");
            var rng = RandomNumberGenerator.Create();
            var randomBytes = new byte[8];
            rng.GetNonZeroBytes(randomBytes);
            var randomIdCode = BitConverter.ToString(randomBytes)
                .Replace("-", "")
                .ToLowerInvariant();
            var tempFile = Path.Combine(rootDir ?? ".", $"{Path.GetFileNameWithoutExtension(fn)}_{randomIdCode}.tmp");
            //Debug.WriteLine($"Target: {targetFile}, Temp: {tempFile}");

            var fileHash = File.ReadAllText(targetFile).GetHashCode();
            var newContent = "";
            var newContentHash = 0;
            try
            {
                using (var fileStreamW = File.CreateText(tempFile))
                {
                    fileStreamW.Write(newContent = serializer.Serialize(o));
                }

                newContentHash = newContent.GetHashCode();
                if (newContentHash != fileHash)
                {
                    File.Delete(targetFile);
                    File.Move(tempFile, targetFile);
                }
                else
                {
                    File.Delete(tempFile);
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }

                MessageBox.Show(
                    $"Failed to save to {serializer.Extension.ToUpperInvariant()}: {ex.Message}\r\n\r\nFile: {targetFile}\r\nTemp file: {tempFile}",
                    "Whoops", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                return null;
            }

            return new RawSaveResult(newContent.Length, newContentHash);
        }

        private void SaveRawOverrides(ISerialize serializer, string iFilename, string name)
        {
            var toSerialize = new
            {
                name,
                CompOverride
            };
            SaveRawMhd(serializer, toSerialize, iFilename, null);
        }

        private void SaveOverrides(ISerialize serializer)
        {
            var fn = Files.SelectDataFileLoad("Compare.mhd");
            SaveRawOverrides(serializer, fn, OverrideNames);

            using var fileStream = new FileStream(fn, FileMode.Create);
            using var binaryWriter = new BinaryWriter(fileStream);
            binaryWriter.Write(OverrideNames);
            binaryWriter.Write(CompOverride.Length - 1);
            for (var index = 0; index <= CompOverride.Length - 1; ++index)
            {
                binaryWriter.Write(CompOverride[index].Powerset);
                binaryWriter.Write(CompOverride[index].Power);
                binaryWriter.Write(CompOverride[index].Override);
            }
        }

        public class SDamageMath
        {
            public SDamageMath()
            {
            }

            public SDamageMath(EDamageMath dmgMath, EDamageReturn dmgRet)
            {
                Calculate = dmgMath;
                ReturnValue = dmgRet;
            }

            public EDamageMath Calculate { get; set; }
            public EDamageReturn ReturnValue { get; set; }
        }

        public class IncludeExclude
        {
            public bool DisablePvE { get; set; }
        }

        public class Si9
        {
            public int DefaultIOLevel { get; set; }
            public bool HideIOLevels { get; set; }
            public bool IgnoreEnhFX { get; set; }
            public bool IgnoreSetBonusFX { get; set; }
            public bool DisablePrintIOLevels { get; set; }
            public bool ExportIOLevels { get; set; }
            public bool ExportStripSetNames { get; set; }
            public bool ExportStripEnh { get; set; }
            public bool DisableExportDataChunk { get; set; }
            public bool DisableExportCompress { get; set; }
            public bool ExportExtraSep { get; set; }
        }

        public class FontSettings
        {
            public int RTFBase { get; set; }
            public bool RTFBold { get; set; }
            public Color ColorBackgroundHero { get; set; }
            public Color ColorBackgroundVillain { get; set; }
            public Color ColorText { get; set; }
            public Color ColorInvention { get; set; }
            public Color ColorInventionInv { get; set; }
            public Color ColorFaded { get; set; }
            public Color ColorEnhancement { get; set; }
            public Color ColorWarning { get; set; }
            public Color ColorPlName { get; set; }
            public Color ColorPlSpecial { get; set; }
            public Color ColorPowerAvailable { get; set; }
            public Color ColorPowerDisabled { get; set; }
            public Color ColorPowerTakenHero { get; set; }
            public Color ColorPowerTakenDarkHero { get; set; }
            public Color ColorPowerHighlightHero { get; set; }
            public Color ColorPowerTakenVillain { get; set; }
            public Color ColorPowerTakenDarkVillain { get; set; }
            public Color ColorPowerHighlightVillain { get; set; }
            public Color ColorDamageBarBase { get; set; }
            public Color ColorDamageBarEnh { get; set; }
            public List<Color> ColorList { get; set; }
            public bool PairedBold { get; set; }
            public float PairedBase { get; set; }
            public bool PowersSelectBold { get; set; }
            public float PowersSelectBase { get; set; }
            public bool PowersBold { get; set; }
            public float PowersBase { get; set; }

            public void Assign(FontSettings iFs)
            {
                RTFBase = iFs.RTFBase;
                RTFBold = iFs.RTFBold;
                ColorBackgroundHero = iFs.ColorBackgroundHero;
                ColorBackgroundVillain = iFs.ColorBackgroundVillain;
                ColorText = iFs.ColorText;
                ColorInvention = iFs.ColorInvention;
                ColorInventionInv = iFs.ColorInventionInv;
                ColorFaded = iFs.ColorFaded;
                ColorEnhancement = iFs.ColorEnhancement;
                ColorWarning = iFs.ColorWarning;
                ColorPlName = iFs.ColorPlName;
                ColorPlSpecial = iFs.ColorPlSpecial;
                ColorPowerAvailable = iFs.ColorPowerAvailable;
                ColorPowerDisabled = iFs.ColorPowerDisabled;
                ColorPowerTakenHero = iFs.ColorPowerTakenHero;
                ColorPowerTakenDarkHero = iFs.ColorPowerTakenDarkHero;
                ColorPowerHighlightHero = iFs.ColorPowerHighlightHero;
                ColorPowerTakenVillain = iFs.ColorPowerTakenVillain;
                ColorPowerTakenDarkVillain = iFs.ColorPowerTakenDarkVillain;
                ColorPowerHighlightVillain = iFs.ColorPowerHighlightVillain;
                ColorDamageBarBase = iFs.ColorDamageBarBase;
                ColorDamageBarEnh = iFs.ColorDamageBarEnh;
                ColorList = iFs.ColorList;
                PairedBold = iFs.PairedBold;
                PairedBase = iFs.PairedBase;
                PowersSelectBase = iFs.PowersSelectBase;
                PowersSelectBold = iFs.PowersSelectBold;
                PowersBase = iFs.PowersBase;
                PowersBold = iFs.PowersBold;
            }

            public void SetDefault()
            {
                RTFBase = 16;
                RTFBold = true;
                ColorBackgroundHero = Color.Black;
                ColorBackgroundVillain = Color.Black;
                ColorEnhancement = Color.FromArgb(0, byte.MaxValue, 0);
                ColorFaded = Color.FromArgb(192, 192, 192);
                ColorInvention = Color.FromArgb(0, byte.MaxValue, byte.MaxValue);
                ColorInventionInv = Color.FromArgb(0, 0, 128);
                ColorText = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue);
                ColorWarning = Color.FromArgb(byte.MaxValue, 0, 0);
                ColorPlName = Color.FromArgb(192, 192, byte.MaxValue);
                ColorPlSpecial = Color.FromArgb(128, 128, byte.MaxValue);
                ColorPowerAvailable = Color.Gold;
                ColorPowerDisabled = Color.DimGray;
                ColorPowerTakenHero = Color.FromArgb(116, 168, 234);
                ColorPowerTakenDarkHero = Color.DodgerBlue;
                ColorPowerHighlightHero = Color.FromArgb(64, 64, 96);
                ColorPowerTakenVillain = Color.FromArgb(191, 74, 56);
                ColorPowerTakenDarkVillain = Color.Maroon;
                ColorPowerHighlightVillain = Color.FromArgb(96, 64, 64);
                ColorDamageBarBase = Color.LimeGreen;
                ColorDamageBarEnh = Color.DarkRed;
                ColorList = new List<Color>
                {
                    ColorPowerTakenHero, ColorPowerTakenDarkHero, ColorPowerHighlightHero, ColorPowerTakenVillain,
                    ColorPowerTakenDarkVillain, ColorPowerHighlightVillain
                };
                PairedBase = 10.25f;
                PairedBold = false;
                // Zed: With Tahoma, spaces tend to be munched if PowersSelectBase is at 8.25
                // Looks good with 8.50 with no other noticeable difference.
                PowersSelectBase = 8.50f;
                PowersSelectBold = false;
                PowersBase = 9.25f;
                PowersBold = true;
            }
        }
    }
}