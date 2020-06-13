using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Base;
using HeroDesigner.Schema;

public interface ISerialize
{
    string Serialize(object o);
    T Deserialize<T>(string x);
    string Extension { get; }
}

public class ConfigData
{
    string _defaultSaveFolderOverride;
    Size _lastSize = new Size(1072, 760);

    // these properties require setters for deserialization
    public SDamageMath DamageMath { get; } = new SDamageMath();
    public IncludeExclude Inc { get; } = new IncludeExclude();
    public Si9 I9 { get; } = new Si9();
    public FontSettings RtFont { get; } = new FontSettings();
    public Size LastSize { get => _lastSize; set => _lastSize = value; }

    public float BaseAcc { get; set; } = 0.75f;
    public string UpdatePath = "https://midsreborn.com/mids_updates/update_manifest.xml";
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

    public string DNickName { get; set; }
    public List<string> DServers { get; } = new List<string>();
    public string DSelServer { get; set; }
    public string DChannel { get; set; }
    public List<string> CheckedStatBoxes { get; set; } = new List<string>();
    public List<string> CheckedStats { get; set; } = new List<string>();
    public bool DisableDesaturateInherent { get; set; }
    public Enums.dmModes BuildMode { get; set; } = Enums.dmModes.Dynamic;
    public Enums.dmItem BuildOption { get; set; } = Enums.dmItem.Slot;
    public bool DisableShowPopup { get; set; }
    public bool DisableAlphaPopup { get; set; }
    public bool DisableRepeatOnMiddleClick { get; set; }
    public bool DisableExportHex { get; set; }
    public readonly short[] DragDropScenarioAction = {
            3, 0, 5, 0, 3, 5, 0, 0, 5, 0, 2, 3, 0, 2, 2, 0, 0, 0, 0, 0
        };
    public Enums.eSpeedMeasure SpeedFormat = Enums.eSpeedMeasure.MilesPerHour;
    static ConfigData _current { get; set; }

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
    public bool EnhanceVisibility { get; set; }
    public Tips Tips { get; set; }
    public bool PopupRecipes { get; set; }
    public bool ShoppingListIncludesRecipes { get; set; }
    public bool ExportChunkOnly { get; set; }
    public bool LongExport { get; set; }
    public bool MasterMode { get; set; }
    public bool ShrinkFrmSets { get; set; }


    public string DefaultSaveFolderOverride
    {
        get => _defaultSaveFolderOverride;
        set
        {
            var osDefault = OS.GetDefaultSaveFolder();
            if (string.IsNullOrWhiteSpace(value)
                || Path.GetFullPath(value) == osDefault
                || value == osDefault
                || (osDefault != null && Path.GetFullPath(osDefault) == value))
            {
                _defaultSaveFolderOverride = null;
                return;
            }
            _defaultSaveFolderOverride = value;
        }
    }

    internal static ConfigData Current
    {
        get
        {
            ConfigData configData;
            if ((configData = _current) == null)
                throw new InvalidOperationException("Config was not initialized before access");
            //configData = ConfigData._current = ConfigData.Initialize();
            return configData;
        }
    }

    static void MigrateToSerializer(string mhdFn, ISerialize serializer)
    {
        var oldMethod = new ConfigData(deserializing: false, iFilename: mhdFn);
        oldMethod.IntializeComponent();
        var file = mhdFn + ".old";
        if (File.Exists(file))
            file += "2";
        File.Move(mhdFn, file);
        oldMethod.SaveConfig(serializer);
    }

    public static void Initialize(ISerialize serializer)
    {
        // migrate
        // force Mhd if it exists, then rename it
        var mhdFn = Files.GetConfigFilename(forceMhd: true);
        if (File.Exists(mhdFn))
            MigrateToSerializer(mhdFn, serializer);

        var fn = Files.GetConfigFilename(forceMhd: false);
        if (File.Exists(fn) && fn.EndsWith(".json"))
        {
            try
            {
                var value = serializer.Deserialize<ConfigData>(File.ReadAllText(fn));
                _current = value;
            }
            catch
            {
                MessageBox.Show("Failed to load json config, falling back to mhd");
            }
        }
        else
        {
            _current = new ConfigData(deserializing: false, iFilename: Files.GetConfigFilename(forceMhd: true));
        }
        _current.IntializeComponent();
    }

    ConfigData() : this(true, "") { }

    ConfigData(bool deserializing, string iFilename)
    {
        DamageMath.Calculate = EDamageMath.Average;
        DamageMath.ReturnValue = EDamageReturn.Numeric;
        I9.DefaultIOLevel = 49;
        RtFont.SetDefault();
        Tips = new Tips();
        Export = new ExportConfig();
        CompOverride = Array.Empty<Enums.CompOverride>();
        if (deserializing) return;
        if (!string.IsNullOrEmpty(iFilename) && File.Exists(iFilename))
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
        IntializeComponent();
    }

    void IntializeComponent()
    {
        if (string.IsNullOrEmpty(UpdatePath))
            UpdatePath = "https://midsreborn.com/mids_updates/update_manifest.xml";

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

    void LegacyForMigration(string iFilename)
    {
        //using (FileStream fileStream = new FileStream(iFilename, FileMode.Open, FileAccess.Read))
        {

            using (BinaryReader reader = new BinaryReader(File.Open(iFilename, FileMode.Open, FileAccess.Read)))
            {
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
                    case string x when x == header:
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
                {
                    reader.ReadBoolean();
                }
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
                }
                if (version >= 1.22000002861023)
                {
                    ShowSlotLevels = reader.ReadBoolean();
                    DisableLoadLastFileOnStart = !reader.ReadBoolean();
                    LastFileName = reader.ReadString();
                    RtFont.ColorPowerAvailable = reader.ReadRGB();
                    RtFont.ColorPowerTaken = reader.ReadRGB();
                    RtFont.ColorPowerTakenDark = reader.ReadRGB();
                    RtFont.ColorPowerDisabled = reader.ReadRGB();
                    RtFont.ColorPowerHighlight = reader.ReadRGB();
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
    }

    public string GetSaveFolder() => DefaultSaveFolderOverride ?? OS.GetDefaultSaveFolder();

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

    void SaveRaw(ISerialize serializer, string iFilename)
        => SaveRawMhd(serializer, this, iFilename, null);

    const string header = "Mids' Hero Designer Config V2";
    void Save(ISerialize serializer, string iFilename)
        => SaveRaw(serializer, iFilename);

    void RelocateSaveFolder(bool manual)
    {
        if (!string.IsNullOrWhiteSpace(DefaultSaveFolderOverride) && Directory.Exists(DefaultSaveFolderOverride) && OS.GetDefaultSaveFolder() != DefaultSaveFolderOverride & (!SaveFolderChecked | manual))
        {
            if (DefaultSaveFolderOverride.IndexOf(OS.GetMyDocumentsPath(), StringComparison.OrdinalIgnoreCase) > -1)
            {
                SaveFolderChecked = true;
                return;
            }
            if (Directory.Exists(DefaultSaveFolderOverride))
            {
                if (MessageBox.Show("In order for Mids' Reborn : Designer to operate better in more recent versions of Windows, the recommended Save folder has been changed to appear inside the My Documents folder.\nThe application can automatically move your save folder and its contents to 'My Documents\\Hero & Villain Builds\\'.\nThis message will not appear again.\n\nMove your Save folder now?", "Save Folder Location", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    LastFileName = string.Empty;
                    string defaultSaveFolder = DefaultSaveFolderOverride;
                    DefaultSaveFolderOverride = null;
                    if (FileIO.CopyFolder(defaultSaveFolder, GetSaveFolder()))
                    {
                        MessageBox.Show(@"Save folder was moved!", "All Done", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show(@"Save folder couldn't be moved! Using old save folder instead.", "Whoops", MessageBoxButtons.OK);
                        DefaultSaveFolderOverride = defaultSaveFolder;
                    }
                }
            }
            else
                CreateDefaultSaveFolder();
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

    void LoadOverrides()
    {
        using (FileStream fileStream = new FileStream(Files.SelectDataFileLoad("Compare.mhd"), FileMode.Open, FileAccess.Read))
        {
            using (BinaryReader binaryReader = new BinaryReader(fileStream))
            {
                if (binaryReader.ReadString() != "Mids' Hero Designer Comparison Overrides")
                {
                    MessageBox.Show("Overrides file was missing a header! Not loading powerset comparison overrides.");
                }
                else
                {
                    CompOverride = new Enums.CompOverride[binaryReader.ReadInt32() + 1];
                    for (int index = 0; index <= CompOverride.Length - 1; ++index)
                    {
                        CompOverride[index].Powerset = binaryReader.ReadString();
                        CompOverride[index].Power = binaryReader.ReadString();
                        CompOverride[index].Override = binaryReader.ReadString();
                    }
                }
            }
        }
    }

    public static (bool, T) LoadRawMhd<T>(ISerialize serializer, string fn)
    {
        return !File.Exists(fn) ? (false, default) : (true, serializer.Deserialize<T>(File.ReadAllText(fn)));
    }

    public static RawSaveResult SaveRawMhd(ISerialize serializer, object o, string fn, RawSaveResult lastSaveInfo)
    {
        try
        {
            var path = Path.Combine(Path.GetDirectoryName(fn), Path.GetFileNameWithoutExtension(fn) + "." + serializer.Extension);
            var text = serializer.Serialize(o);
            // don't save if the file will be the same
            if (lastSaveInfo != null && text != null
                && lastSaveInfo.Length > 0 && text.Length > 0
                && lastSaveInfo.Hash != 0 && lastSaveInfo.Hash == text.GetHashCode()
                && File.Exists(fn))
                return lastSaveInfo;
            if (lastSaveInfo != null)
                Console.WriteLine("Writing out updated file: " + fn);
            File.WriteAllText(path, contents: text);
            return new RawSaveResult(length: text?.Length ?? 0, hash: text?.GetHashCode() ?? 0);
        }
        catch (Exception ex)
        {
            MessageBox.Show("Failed to save raw config: " + ex.Message, ex.GetType().Name);
            return null;
        }
    }

    const string OverrideNames = "Mids' Hero Designer Comparison Overrides";
    void SaveRawOverrides(ISerialize serializer, string iFilename, string name)
    {
        var toSerialize = new
        {
            name,
            CompOverride
        };
        SaveRawMhd(serializer, toSerialize, iFilename, null);
    }

    void SaveOverrides(ISerialize serializer)
    {
        var fn = Files.SelectDataFileLoad("Compare.mhd");
        SaveRawOverrides(serializer, fn, OverrideNames);

        using (FileStream fileStream = new FileStream(fn, FileMode.Create))
        {
            using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
            {
                binaryWriter.Write(OverrideNames);
                binaryWriter.Write(CompOverride.Length - 1);
                for (int index = 0; index <= CompOverride.Length - 1; ++index)
                {
                    binaryWriter.Write(CompOverride[index].Powerset);
                    binaryWriter.Write(CompOverride[index].Power);
                    binaryWriter.Write(CompOverride[index].Override);
                }
            }
        }
    }

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

    public class SDamageMath
    {
        public EDamageMath Calculate { get; set; }
        public EDamageReturn ReturnValue { get; set; }
        public SDamageMath() { }
        public SDamageMath(EDamageMath dmgMath, EDamageReturn dmgRet)
        {
            Calculate = dmgMath;
            ReturnValue = dmgRet;
        }
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
        public Color ColorPowerTaken { get; set; }
        public Color ColorPowerTakenDark { get; set; }
        public Color ColorPowerHighlight { get; set; }
        public Color ColorPowerDisabled { get; set; }
        public Color ColorPowerTakenHero { get; set; }
        public Color ColorPowerTakenDarkHero { get; set; }
        public Color ColorPowerHighlightHero { get; set; }
        public Color ColorPowerTakenVillain { get; set; }
        public Color ColorPowerTakenDarkVillain { get; set; }
        public Color ColorPowerHighlightVillain { get; set; }
        public bool PairedBold { get; set; }
        public float PairedBase { get; set; }

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
            ColorPowerTaken = iFs.ColorPowerTaken;
            ColorPowerTakenDark = iFs.ColorPowerTakenDark;
            ColorPowerHighlight = iFs.ColorPowerHighlight;
            ColorPowerDisabled = iFs.ColorPowerDisabled;
            PairedBold = iFs.PairedBold;
            PairedBase = iFs.PairedBase;
        }

        public void SetDefault()
        {
            RTFBase = 16;
            RTFBold = false;
            ColorBackgroundHero = Color.FromArgb(0, 0, 32);
            ColorBackgroundVillain = Color.FromArgb(32, 0, 0);
            ColorEnhancement = Color.FromArgb(0, byte.MaxValue, 0);
            ColorFaded = Color.FromArgb(192, 192, 192);
            ColorInvention = Color.FromArgb(0, byte.MaxValue, byte.MaxValue);
            ColorInventionInv = Color.FromArgb(0, 0, 128);
            ColorText = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            ColorWarning = Color.FromArgb(byte.MaxValue, 0, 0);
            ColorPlName = Color.FromArgb(192, 192, byte.MaxValue);
            ColorPlSpecial = Color.FromArgb(128, 128, byte.MaxValue);
            ColorPowerAvailable = Color.Gold;
            ColorPowerTaken = Color.FromArgb(116, 168, 234);
            ColorPowerTakenDark = Color.DodgerBlue;
            ColorPowerHighlight = Color.FromArgb(64, 64, 96);
            ColorPowerDisabled = Color.DimGray;
            //ColorPowerTakenVillain = Color.FromArgb(191, 74, 56);
            //ColorPowerTakenDarkVillain = Color.Maroon;
            //ColorPowerHighlightVillain = Color.FromArgb(96,64,64);
            PairedBase = 8.25f;
            PairedBold = false;
        }
    }
}
