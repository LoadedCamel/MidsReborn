using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using Mids_Reborn.Core.Utils;
using Path = System.IO.Path;
using Rectangle = System.Drawing.Rectangle;

namespace Mids_Reborn.Core
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

        public enum AutoUpdType
        {
            None,
            Disabled,
            Delay,
            Startup
        }

        public enum Modes
        {
            User,
            DbAdmin,
            AppAdmin
        }

        private const string OverrideNames = "Mids Reborn Comparison Overrides";

        public bool FirstRun { get; set; }
        public AutoUpdate AutomaticUpdates { get; set; }

        public readonly short[] DragDropScenarioAction =
        {
            3, 0, 5, 0, 3, 5, 0, 0, 5, 0, 2, 3, 0, 2, 2, 0, 0, 0, 0, 0
        };

        public Enums.eSpeedMeasure SpeedFormat = Enums.eSpeedMeasure.MilesPerHour;
        public bool CoDEffectFormat = false;

        private ConfigData()
        {
            AutomaticUpdates = new AutoUpdate(AutoUpdType.Delay);
            DamageMath.Calculate = EDamageMath.Average;
            DamageMath.ReturnValue = EDamageReturn.Numeric;
            I9.DefaultIOLevel = 49;
            TotalsWindowTitleStyle = ETotalsWindowTitleStyle.Generic;
            RtFont.SetDefault();
            Tips = new Tips();
            Export = new ExportConfig();
            CompOverride = Array.Empty<Enums.CompOverride>();
            TeamMembers = new Dictionary<string, int>();
            ShowSelfBuffsAny = false;
            WarnOnOldDbMbd = true;
            DimWindowStyleColors = true;
            CloseEnhSelectPopupByMove = true;
            PowerListsWordwrapMode = Enums.WordwrapMode.Legacy;
            Mode = Modes.User;
            InitializeComponent();
        }

        // these properties require setters for deserialization
        public SDamageMath DamageMath { get; } = new();
        public IncludeExclude Inc { get; } = new();
        public Si9 I9 { get; } = new();
        public FontSettings RtFont { get; } = new();
        public Dictionary<string, int> TeamMembers { get; }

        public string? WindowState { get; set; }
        public Rectangle Bounds { get; set; }

        public bool UseOldTotalsWindow { get; set; }

        public float ScalingToHit { get; set; } = DatabaseAPI.ServerData.BaseToHit;

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
        public string? LastFileName { get; set; } = string.Empty;
        public Enums.eEnhGrade CalcEnhOrigin { get; set; } = Enums.eEnhGrade.SingleO;
        public Enums.eEnhRelative CalcEnhLevel { get; set; } = Enums.eEnhRelative.Even;
        public Enums.eDDGraph DataGraphType { get; set; } = Enums.eDDGraph.Both;
        public Enums.GraphStyle StatGraphStyle { get; set; } = Enums.GraphStyle.Stacked;
        public Enums.CompOverride[] CompOverride { get; set; }
        public bool ShowSlotsLeft { get; set; }

        public bool DisableDesaturateInherent { get; set; }
        public Enums.dmModes BuildMode { get; set; } = Enums.dmModes.Normal;
        public Enums.dmItem BuildOption { get; set; } = Enums.dmItem.Slot;
        public bool DisableShowPopup { get; set; }
        public bool DisableAlphaPopup { get; set; }
        public bool DisableRepeatOnMiddleClick { get; set; }
        private static ConfigData? Instance { get; set; } = null;
        public bool ExportBonusTotals { get; set; }
        public bool ExportBonusList { get; set; }
        public bool NoToolTips { get; set; }
        public bool DataDamageGraphPercentageOnly { get; private set; }
        public Enums.eVisibleSize DvState { get; set; }
        public Enums.eSuppress Suppression { get; set; }
        public bool UseArcanaTime { get; set; }
        public ExportConfig Export { get; }
        public ShareConfig ShareConfig { get; set; } = new();
        public bool PrintInColor { get; set; }
        public bool PrintHistory { get; set; }
        public bool SaveFolderChecked { get; set; }
        public bool ShowSlotLevels { get; set; }
        public bool ShowEnhRel { get; set; }
        public bool ShowRelSymbols { get; set; }
        public bool ShowSoLevels { get; set; }
        public bool EnhanceVisibility { get; set; }
        public Tips Tips { get; set; }
        public bool PopupRecipes { get; set; }
        public bool ShoppingListIncludesRecipes { get; set; }
        public bool LongExport { get; set; }

        public Point? RotationHelperLocation { get; set; }

        public Enums.WordwrapMode PowerListsWordwrapMode { get; set; }

        internal bool MasterMode
        {
            get
            {
                var mode = Mode switch
                {
                    Modes.User => false,
                    Modes.DbAdmin => true,
                    Modes.AppAdmin => true,
                    _ => throw new ArgumentOutOfRangeException()
                };
                return mode;
            }
        }
        public Modes Mode { get; set; }
        public bool ShrinkFrmSets { get; set; }
        public bool WarnOnOldDbMbd { get; set; }
        public bool DimWindowStyleColors { get; set; }
        public bool CloseEnhSelectPopupByMove { get; set; }

        private string _buildsPath = Files.FDefaultBuildsPath;

        public string BuildsPath
        {
            get => _buildsPath;
            set
            {
                if (!Directory.Exists(value))
                {
                    Directory.CreateDirectory(value);
                }

                _buildsPath = value;
            }
        }

        public string? DataPath { get; set; }

        private string? _savePath = Files.FDefaultPath;

        public string? SavePath
        {
            get => _savePath;
            set
            {
                if (value != DataPath)
                {
                    if (!Directory.Exists(value))
                    {
                        if (value != null) Directory.CreateDirectory(value);
                    }

                    _savePath = value;
                }
                else
                {
                    _savePath = DataPath;
                }
            }
        }

        public string? UpdatePath { get; private set; }

        public Enums.RewardCurrency PreferredCurrency = Enums.RewardCurrency.RewardMerit;

        public bool ShowSelfBuffsAny { get; set; }

        public ETotalsWindowTitleStyle TotalsWindowTitleStyle { get; set; }

        public Point? EntityDetailsLocation { get; set; }

        internal static ConfigData? Current
        {
            get
            {
                var configData = Instance;
                return configData!;
            }
        }

        internal readonly List<KeyValuePair<string, float>> RelativeScales = new()
        {
            new KeyValuePair<string, float>("Enemy Relative Level: -4", 0.95f),
            new KeyValuePair<string, float>("Enemy Relative Level: -3", 0.9f),
            new KeyValuePair<string, float>("Enemy Relative Level: -2", 0.85f),
            new KeyValuePair<string, float>("Enemy Relative Level: -1", 0.8f),
            new KeyValuePair<string, float>("Enemy Relative Level: Default", 0.75f),
            new KeyValuePair<string, float>("Enemy Relative Level: +1", 0.65f),
            new KeyValuePair<string, float>("Enemy Relative Level: +2", 0.56f),
            new KeyValuePair<string, float>("Enemy Relative Level: +3", 0.48f),
            new KeyValuePair<string, float>("Enemy Relative Level: +4", 0.39f),
            new KeyValuePair<string, float>("Enemy Relative Level: +5", 0.3f),
            new KeyValuePair<string, float>("Enemy Relative Level: +6", 0.2f),
            new KeyValuePair<string, float>("Enemy Relative Level: +7", 0.08f)
        };

        public void ResetBuildsPath()
        {
            BuildsPath = Files.FDefaultBuildsPath;
        }

        public static void Initialize(bool firstRun = false)
        {
            var serializer = Serializer.GetSerializer();
            if (firstRun)
            {
                Instance = new ConfigData();
                Instance.SaveConfig();
                Instance.InitializeComponent();
                return;
            }

            Instance = serializer.Deserialize<ConfigData>(File.ReadAllText(Files.FNameJsonConfig));
            Instance.InitializeComponent();
        }

        private void InitializeComponent()
        {
            UpdatePath = Debugger.IsAttached
                ? "https://midsreborn.com/mids_updates/app-test/update_manifest.xml"
                : "https://midsreborn.com/mids_updates/app/update_manifest.xml";

            if (string.IsNullOrWhiteSpace(DataPath))
            {
                DataPath = Files.FDefaultPath;
            }

            // RelocateSaveFolder(false);
            try
            {
                LoadOverrides();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Message: {ex.Message}\r\nTrace: {ex.StackTrace}");
            }
        }

        public Color GetStreamColor(BinaryReader br, Enums.eColorSetting clSetting, bool autoFix = true)
        {
            var cl = br.ReadRGB();
            if (autoFix & cl.R == 0 & cl.G == 0 & cl.B == 0)
            {
                return RtFont.GetDefaultColorSetting(clSetting);
            }

            return cl;
        }

        public float GetStreamFontSize(BinaryReader br, Enums.eFontSizeSetting fntSetting, bool autoFix = true)
        {
            var fntSize = br.ReadSingle();
            if (autoFix & !RtFont.ValidFontSize(fntSize))
            {
                return RtFont.GetDefaultFontSizeSetting(fntSetting);
            }

            return fntSize;
        }

        private void SaveRaw(ISerialize serializer, string iFilename)
        {
            SaveRawMhd(serializer, this, iFilename, null);
        }

        private void Save(ISerialize serializer, string iFilename)
        {
            SaveRaw(serializer, iFilename);
        }

        public void SaveConfig()
        {
            if (!File.Exists(Files.FNameJsonConfig))
            {
                File.Create(Files.FNameJsonConfig);
            }

            var serializer = Serializer.GetSerializer();
            Save(serializer, Files.FNameJsonConfig);
            SaveOverrides(serializer);
        }

        private void LoadOverrides()
        {
            if (!File.Exists(Files.SelectDataFileLoad(Files.MxdbFileOverrides, DataPath)))
            {
                MessageBox.Show($@"Overrides file ({Files.MxdbFileOverrides}) was not found.\r\nCreating a new one...", @"Database file missing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CompOverride = Array.Empty<Enums.CompOverride>();
                SaveOverrides(Serializer.GetSerializer());

                return;
            }

            using var fileStream = new FileStream(Files.SelectDataFileLoad(Files.MxdbFileOverrides, DataPath), FileMode.Open, FileAccess.Read);
            using var binaryReader = new BinaryReader(fileStream);
            if (binaryReader.ReadString() != OverrideNames)
            {
                MessageBox.Show($@"Overrides file ({Files.MxdbFileOverrides}) was missing a header!\r\nNot loading powerset comparison overrides.", @"Database file failed to load", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            CompOverride = new Enums.CompOverride[binaryReader.ReadInt32() + 1];
            for (var index = 0; index < CompOverride.Length; index++)
            {
                CompOverride[index].Powerset = binaryReader.ReadString();
                CompOverride[index].Power = binaryReader.ReadString();
                CompOverride[index].Override = binaryReader.ReadString();
            }
        }

        public static RawSaveResult? SaveRawMhd(ISerialize serializer, object o, string fn, RawSaveResult lastSaveInfo)
        {
            var rootDir = Path.GetDirectoryName(fn);
            var targetFile = Path.Combine(rootDir ?? ".", $"{Path.GetFileNameWithoutExtension(fn)}.{serializer.Extension}");
            if (!File.Exists(targetFile)) File.WriteAllText(targetFile, string.Empty);

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
            //SaveRawOverrides(serializer, fn, OverrideNames);

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

        public class AutoUpdate
        {
            public bool Enabled => Type is AutoUpdType.Delay or AutoUpdType.Startup;
            public AutoUpdType Type { get; set; }
            public int Delay { get; set; }
            public DateTime? LastChecked { get; set; }

            public AutoUpdate(AutoUpdType type, int delay = 3)
            {
                Type = type;
                Delay = delay;
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
                PairedBold = iFs.PairedBold;
                PairedBase = iFs.PairedBase;
                PowersSelectBase = iFs.PowersSelectBase;
                PowersSelectBold = iFs.PowersSelectBold;
                PowersBase = iFs.PowersBase;
                PowersBold = iFs.PowersBold;
            }

            public bool ValidFontSize(float fntSize)
            {
                return fntSize >= 6 & fntSize <= 14;
            }

            public Color GetDefaultColorSetting(Enums.eColorSetting clSetting)
            {
                return clSetting switch
                {
                    Enums.eColorSetting.ColorBackgroundHero => Color.FromArgb(0, 0, 32), //Color.Black;
                    Enums.eColorSetting.ColorBackgroundVillain => Color.FromArgb(32, 0, 0), //Color.Black;
                    Enums.eColorSetting.ColorText => Color.White,
                    Enums.eColorSetting.ColorInvention => Color.Cyan,
                    Enums.eColorSetting.ColorInventionInv => Color.Navy,
                    Enums.eColorSetting.ColorFaded => Color.Silver,
                    Enums.eColorSetting.ColorEnhancement => Color.Lime,
                    Enums.eColorSetting.ColorWarning => Color.Red,
                    Enums.eColorSetting.ColorPlName => Color.FromArgb(192, 192, 255),
                    Enums.eColorSetting.ColorPlSpecial => Color.FromArgb(128, 128, 255),
                    Enums.eColorSetting.ColorPowerAvailable => Color.Gold,
                    Enums.eColorSetting.ColorPowerDisabled => Color.DimGray,
                    Enums.eColorSetting.ColorPowerTakenHero => Color.FromArgb(116, 168, 234),
                    Enums.eColorSetting.ColorPowerTakenDarkHero => Color.DodgerBlue,
                    Enums.eColorSetting.ColorPowerHighlightHero => Color.FromArgb(64, 64, 96),
                    Enums.eColorSetting.ColorPowerTakenVillain => Color.FromArgb(191, 74, 56),
                    Enums.eColorSetting.ColorPowerTakenDarkVillain => Color.Maroon,
                    Enums.eColorSetting.ColorPowerHighlightVillain => Color.FromArgb(96, 64, 64),
                    Enums.eColorSetting.ColorDamageBarBase => Color.FromArgb(255,194,194),
                    Enums.eColorSetting.ColorDamageBarEnh => Color.FromArgb(181, 0, 0),
                    _ => Color.FromArgb(0, 0, 0)
                };
            }

            public float GetDefaultFontSizeSetting(Enums.eFontSizeSetting fntSetting)
            {
                return fntSetting switch
                {
                    Enums.eFontSizeSetting.PairedBase => 8.25f,
                    Enums.eFontSizeSetting.PowersSelectBase => 9.25f,
                    Enums.eFontSizeSetting.PowersBase => 9.25f,
                    _ => 8.50f
                };
            }

            public void SetDefault()
            {
                RTFBase = 16;
                RTFBold = true;
                ColorBackgroundHero = GetDefaultColorSetting(Enums.eColorSetting.ColorBackgroundHero);
                ColorBackgroundVillain = GetDefaultColorSetting(Enums.eColorSetting.ColorBackgroundVillain);
                ColorText = GetDefaultColorSetting(Enums.eColorSetting.ColorText);
                ColorInvention = GetDefaultColorSetting(Enums.eColorSetting.ColorInvention);
                ColorInventionInv = GetDefaultColorSetting(Enums.eColorSetting.ColorInventionInv);
                ColorFaded = GetDefaultColorSetting(Enums.eColorSetting.ColorFaded);
                ColorEnhancement = GetDefaultColorSetting(Enums.eColorSetting.ColorEnhancement);
                ColorWarning = GetDefaultColorSetting(Enums.eColorSetting.ColorWarning);
                ColorPlName = GetDefaultColorSetting(Enums.eColorSetting.ColorPlName);
                ColorPlSpecial = GetDefaultColorSetting(Enums.eColorSetting.ColorPlSpecial);
                ColorPowerAvailable = GetDefaultColorSetting(Enums.eColorSetting.ColorPowerAvailable);
                ColorPowerDisabled = GetDefaultColorSetting(Enums.eColorSetting.ColorPowerDisabled);
                ColorPowerTakenHero = GetDefaultColorSetting(Enums.eColorSetting.ColorPowerTakenHero);
                ColorPowerTakenDarkHero = GetDefaultColorSetting(Enums.eColorSetting.ColorPowerTakenDarkHero);
                ColorPowerHighlightHero = GetDefaultColorSetting(Enums.eColorSetting.ColorPowerHighlightHero);
                ColorPowerTakenVillain = GetDefaultColorSetting(Enums.eColorSetting.ColorPowerTakenVillain);
                ColorPowerTakenDarkVillain = GetDefaultColorSetting(Enums.eColorSetting.ColorPowerTakenDarkVillain);
                ColorPowerHighlightVillain = GetDefaultColorSetting(Enums.eColorSetting.ColorPowerHighlightVillain);
                ColorDamageBarBase = GetDefaultColorSetting(Enums.eColorSetting.ColorDamageBarBase);
                ColorDamageBarEnh = GetDefaultColorSetting(Enums.eColorSetting.ColorDamageBarEnh);
                PairedBase = GetDefaultFontSizeSetting(Enums.eFontSizeSetting.PairedBase);
                PairedBold = false;
                // Zed: With Tahoma, spaces tend to be munched if PowersSelectBase is at 8.25
                // Looks good with 8.50 with no other noticeable difference.
                PowersSelectBase = GetDefaultFontSizeSetting(Enums.eFontSizeSetting.PowersSelectBase);
                PowersSelectBold = false;
                PowersBase = GetDefaultFontSizeSetting(Enums.eFontSizeSetting.PowersBase);
                PowersBold = true;
            }
        }
    }
}