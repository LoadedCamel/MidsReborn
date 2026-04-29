using Mids_Reborn.UI.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Forms;
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

    /// <summary>
    /// Hardened, lazy-initialized, JSON-backed application config.
    /// - Auto-initializes on first access (thread-safe).
    /// - Loads from JSON when present; otherwise uses defaults and saves atomically.
    /// - Preserves existing public API & data model to avoid breaking callers.
    /// </summary>
    public sealed class ConfigData
    {
        #region Enums

        public enum EDamageMath { Minimum, Average, Max }
        public enum EDamageReturn { Numeric, DPS, DPA }
        public enum PrintOptionProfile { None, SinglePage, MultiPage }
        public enum ETotalsWindowTitleStyle { Generic, CharNameAtPowersets, BuildFileAtPowersets, CharNameBuildFile }
        public enum AutoUpdType { None, Disabled, Delay, Startup }
        public enum Modes { User, DbAdmin, AppAdmin }

        #endregion

        #region Constants & static

        private const string OverrideNames = "Mids Reborn Comparison Overrides";

        // The single, canonical instance (lazy, thread-safe).
        private static readonly Lazy<ConfigData> _lazy =
            new(() => CreateOrLoad(), LazyThreadSafetyMode.ExecutionAndPublication);

        // Backward-compatible lock for one-time Initialize() entry.
        private static readonly object _initSync = new();

        /// <summary>
        /// Public, non-nullable entry used throughout the app, e.g. via MidsContext.Config.
        /// Auto-initializes if not already done.
        /// </summary>
        internal static ConfigData Current => _lazy.Value;

        public static void Initialize()
        {
            lock (_initSync)
            {
                if (_lazy.IsValueCreated) return;
                _ = _lazy.Value; 
            }
        }

        // Centralized locations/utilities for config files
        private static string ConfigDirectory =>
            AppDataPaths.DefaultPath ?? Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private static string JsonConfigPath => AppDataPaths.JsonConfig;

        private static JsonSerializerOptions JsonOpts => new()
        {
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            WriteIndented = true
        };

        #endregion

        #region Constructor & factory

        public ConfigData()
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
            CombatContextSettings = new CombatContext();
        }

        private static ConfigData CreateOrLoad()
        {
            if (!File.Exists(JsonConfigPath))
            {
                var fresh = new ConfigData
                {
                    FirstRun = true
                };
                fresh.FinishSetupAndEnsurePaths();
                fresh.SaveConfig(); // atomic write of real JSON
                return fresh;
            }

            // Else: try to load JSON; on failure back up and recover with defaults.
            try
            {
                var json = File.ReadAllText(JsonConfigPath);
                var loaded = Serializer.GetSerializer().Deserialize<ConfigData>(json);
                if (loaded is null) throw new InvalidDataException("Deserialized ConfigData was null.");
                loaded.FinishSetupAndEnsurePaths();
                loaded.FirstRun = false;
                return loaded;
            }
            catch
            {
                try
                {
                    File.Copy(JsonConfigPath, JsonConfigPath + ".bak", overwrite: true);
                }
                catch { /* ignore backup issues */ }

                var fallback = new ConfigData
                {
                    FirstRun = true
                };
                fallback.FinishSetupAndEnsurePaths();
                fallback.SaveConfig(); // write clean defaults
                return fallback;
            }
        }

        #endregion

        #region Initialization helpers

        private void FinishSetupAndEnsurePaths()
        {
            // 1) Ensure DataPath
            if (string.IsNullOrWhiteSpace(DataPath))
                DataPath = AppDataPaths.DefaultPath;

            TryEnsureDir(DataPath);

            // 2) Ensure SavePath (defaults to DataPath)
            if (SavePath is null) SavePath = DataPath;
            if (!string.Equals(SavePath, DataPath, StringComparison.OrdinalIgnoreCase))
            {
                // User-provided SavePath different from DataPath ? ensure it exists
                TryEnsureDir(SavePath);
            }
            else
            {
                // If equal or creation failed, force to DataPath
                SavePath = DataPath;
            }

            // 3) Ensure BuildsPath (setter creates the directory)
            try
            {
                BuildsPath = string.IsNullOrWhiteSpace(BuildsPath)
                    ? AppDataPaths.DefaultBuildsPath
                    : BuildsPath;
            }
            catch
            {
                try { BuildsPath = AppDataPaths.DefaultBuildsPath; } catch { /* ignore */ }
            }

            // 4) Load overrides (best effort)
            try
            {
                LoadOverrides();
            }
            catch
            {
                CompOverride = Array.Empty<Enums.CompOverride>();
                try { SaveOverrides(Serializer.GetSerializer()); } catch { /* ignore */ }
            }

            EnemyRelativeLevel = NormalizeEnemyRelativeLevel(EnemyRelativeLevel, ScalingToHit);
            ScalingToHit = GetLegacyScalingToHitForRelativeLevel(EnemyRelativeLevel);

            IsInitialized = true;
        }

        private static void TryEnsureDir(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch { /* non-fatal */ }
        }

        public static (int Min, int Max) GetEnemyRelativeLevelBounds(string? databaseName = null)
        {
            if (!string.IsNullOrWhiteSpace(databaseName))
            {
                return DatabaseAPI.ServerData.ResolveEnemyRelativeLevelBounds(databaseName);
            }

            if (_lazy.IsValueCreated)
            {
                var current = _lazy.Value;
                if (!string.IsNullOrWhiteSpace(current.DataPath))
                {
                    return DatabaseAPI.ServerData.ResolveEnemyRelativeLevelBounds(new DirectoryInfo(current.DataPath).Name);
                }
            }

            return DatabaseAPI.ServerData.ResolveEnemyRelativeLevelBounds();
        }

        public static int ClampEnemyRelativeLevel(int relativeLevel)
        {
            var (min, max) = GetEnemyRelativeLevelBounds();
            return Math.Max(min, Math.Min(max, relativeLevel));
        }

        public static int FindNearestEnemyRelativeLevel(float scalingToHit)
        {
            var nearestRelativeLevel = 0;
            var nearestDistance = float.MaxValue;
            foreach (var (relativeLevel, scale) in EnumerateLegacyRelativeToHitScales())
            {
                var distance = Math.Abs(scale - scalingToHit);
                if (!(distance < nearestDistance))
                {
                    continue;
                }

                nearestRelativeLevel = relativeLevel;
                nearestDistance = distance;
            }

            return nearestRelativeLevel;
        }

        public static int NormalizeEnemyRelativeLevel(int storedRelativeLevel, float scalingToHit)
        {
            return storedRelativeLevel == int.MinValue
                ? FindNearestEnemyRelativeLevel(scalingToHit)
                : ClampEnemyRelativeLevel(storedRelativeLevel);
        }

        public static float GetLegacyScalingToHitForRelativeLevel(int relativeLevel)
        {
            return ClampEnemyRelativeLevel(relativeLevel) switch
            {
                -4 => 0.95f,
                -3 => 0.90f,
                -2 => 0.85f,
                -1 => 0.80f,
                0 => 0.75f,
                1 => 0.65f,
                2 => 0.56f,
                3 => 0.48f,
                4 => 0.39f,
                5 => 0.30f,
                6 => 0.20f,
                7 => 0.08f,
                _ => DatabaseAPI.ServerData.BaseToHit
            };
        }

        private static IEnumerable<KeyValuePair<int, float>> EnumerateLegacyRelativeToHitScales()
        {
            yield return new KeyValuePair<int, float>(-4, 0.95f);
            yield return new KeyValuePair<int, float>(-3, 0.90f);
            yield return new KeyValuePair<int, float>(-2, 0.85f);
            yield return new KeyValuePair<int, float>(-1, 0.80f);
            yield return new KeyValuePair<int, float>(0, 0.75f);
            yield return new KeyValuePair<int, float>(1, 0.65f);
            yield return new KeyValuePair<int, float>(2, 0.56f);
            yield return new KeyValuePair<int, float>(3, 0.48f);
            yield return new KeyValuePair<int, float>(4, 0.39f);
            yield return new KeyValuePair<int, float>(5, 0.30f);
            yield return new KeyValuePair<int, float>(6, 0.20f);
            yield return new KeyValuePair<int, float>(7, 0.08f);
        }

        #endregion

        #region Public data

        [JsonIgnore]
        public bool MasterMode => Mode is Modes.DbAdmin or Modes.AppAdmin;

        public bool FirstRun { get; set; }
        public AutoUpdate AutomaticUpdates { get; set; }

        public readonly short[] DragDropScenarioAction =
        [
            3, 0, 5, 0, 3, 5, 0, 0, 5, 0, 2, 3, 0, 2, 2, 0, 0, 0, 0, 0
        ];

        public Enums.eSpeedMeasure SpeedFormat = Enums.eSpeedMeasure.MilesPerHour;
        public bool CoDEffectFormat = false;

        [JsonIgnore]
        internal List<KeyValuePair<string, int>> RelativeLevels => GetRelativeLevels(
            string.IsNullOrWhiteSpace(DataPath) ? null : new DirectoryInfo(DataPath).Name);

        public static List<KeyValuePair<string, int>> GetRelativeLevels(string? databaseName = null)
        {
            var (min, max) = GetEnemyRelativeLevelBounds(databaseName);
            var levels = new List<KeyValuePair<string, int>>();
            for (var relativeLevel = min; relativeLevel <= max; relativeLevel++)
            {
                levels.Add(new KeyValuePair<string, int>(
                    relativeLevel == 0
                        ? "Enemy Relative Level: Default"
                        : $"Enemy Relative Level: {(relativeLevel > 0 ? "+" : string.Empty)}{relativeLevel}",
                    relativeLevel));
            }

            return levels;
        }

        public SDamageMath DamageMath { get; } = new();
        public IncludeExclude Inc { get; } = new();
        public Si9 I9 { get; } = new();
        public FontSettings RtFont { get; } = new();
        public Dictionary<string, int> TeamMembers { get; }

        public string SelectedTheme { get; set; } = "Hero";
        public string? WindowState { get; set; }
        public Rectangle Bounds { get; set; }
        public bool UseOldTotalsWindow { get; set; }
        public int EnemyRelativeLevel { get; set; } = int.MinValue;
        public float ScalingToHit { get; set; } = DatabaseAPI.ServerData.BaseToHit;
        public int ExempHigh { get; set; } = 50;
        public int TeamSize { get; set; } = 1;
        public int ExempLow { get; set; } = 30;
        public int ForceLevel { get; set; } = 50;
        public int ExportScheme { get; set; } = 1;
        public int ExportTarget { get; set; } = 1;
        public bool DisableDataDamageGraph { get; private set; }
        public bool DisableVillainColors { get; set; }
        public bool IsInitialized { get; set; }
        public int Columns { get; set; } = 3;
        public Enums.eColumnStacking ColumnStackingMode { get; set; } = Enums.eColumnStacking.None;
        public PrintOptionProfile PrintProfile { get; set; } = PrintOptionProfile.SinglePage;
        public bool DisablePrintProfileEnh { get; set; }
        public string LastPrinter { get; set; } = string.Empty;
        public bool DisableLoadLastFileOnStart { get; set; }
        public string? LastFileName { get; set; } = string.Empty;
        public Enums.eEnhGrade CalcEnhOrigin { get; set; } = Enums.eEnhGrade.SingleO;
        public Enums.eEnhRelative CalcEnhLevel { get; set; } = Enums.eEnhRelative.Even;
        public Enums.MDmgGraphType DataGraphType { get; set; } = Enums.MDmgGraphType.Layered;
        public Enums.GraphStyle StatGraphStyle { get; set; } = Enums.GraphStyle.Stacked;
        public Enums.CompOverride[] CompOverride { get; set; }
        public bool ShowSlotsLeft { get; set; }
        public bool DisableDesaturateInherent { get; set; }
        public Enums.dmModes BuildMode { get; set; } = Enums.dmModes.Normal;
        public Enums.dmItem BuildOption { get; set; } = Enums.dmItem.Slot;
        public bool DisableShowPopup { get; set; }
        public bool DisableAlphaPopup { get; set; }
        public bool DisableRepeatOnMiddleClick { get; set; }
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
        public CombatContext CombatContextSettings { get; set; }
        public Modes Mode { get; set; }
        public bool ShrinkFrmSets { get; set; }
        public bool WarnOnOldDbMbd { get; set; }
        public bool DimWindowStyleColors { get; set; }
        public bool CloseEnhSelectPopupByMove { get; set; }

        private string _buildsPath = AppDataPaths.DefaultBuildsPath;
        public string BuildsPath
        {
            get => _buildsPath;
            set
            {
                TryEnsureDir(value);
                _buildsPath = value;
            }
        }

        public string? DataPath { get; set; }

        private string? _savePath = AppDataPaths.DefaultPath;
        public string? SavePath
        {
            get => _savePath;
            set
            {
                if (!string.Equals(value, DataPath, StringComparison.OrdinalIgnoreCase))
                {
                    TryEnsureDir(value);
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
        public bool DisableTips { get; set; } = false;

        #endregion

        #region Public helpers

        public void ResetBuildsPath() => BuildsPath = AppDataPaths.DefaultBuildsPath;

        public static Dictionary<string, string> GetCombatSettings() => new()
        {
            { "cfg.player.hp", "Player HP %" },
            { "cfg.player.isAlive", "Player is Alive/Dead" },
            { "cfg.target.hp", "Target HP %" },
            { "cfg.target.end", "Target Endurance %" }
        };

        public static string? GetCombatSettingName(string param, Dictionary<string, string> table) =>
            (from k in table where string.Equals(param, k.Key, StringComparison.InvariantCultureIgnoreCase) select k.Value)
            .FirstOrDefault();

        public Color GetStreamColor(BinaryReader br, Enums.eColorSetting clSetting, bool autoFix = true)
        {
            var cl = br.ReadRGB();
            if (autoFix & cl.R == 0 & cl.G == 0 & cl.B == 0)
                return RtFont.GetDefaultColorSetting(clSetting);
            return cl;
        }

        public float GetStreamFontSize(BinaryReader br, Enums.eFontSizeSetting fntSetting, bool autoFix = true)
        {
            var fntSize = br.ReadSingle();
            if (autoFix & !RtFont.ValidFontSize(fntSize))
                return RtFont.GetDefaultFontSizeSetting(fntSetting);
            return fntSize;
        }

        #endregion

        #region Saving & loading

        /// <summary>
        /// Persist current config atomically. Keeps your existing serializer abstraction.
        /// </summary>
        public void SaveConfig()
        {
            // IMPORTANT: do NOT pre-create the file with File.Create (it leaves a zero-byte, locked file).
            var serializer = Serializer.GetSerializer();
            Save(serializer, JsonConfigPath);
            SaveOverrides(serializer);
        }

        private void SaveRaw(ISerialize serializer, string filename) =>
            SaveRawMhd(serializer, this, filename, null);

        private void Save(ISerialize serializer, string filename) =>
            SaveRaw(serializer, filename);

        public static RawSaveResult? SaveRawMhd(ISerialize serializer, object o, string fn, RawSaveResult lastSaveInfo)
        {
            // Real target path is "<basename>.<serializer.Extension>"
            var rootDir = Path.GetDirectoryName(fn);
            var targetFile = Path.Combine(rootDir ?? ".", $"{Path.GetFileNameWithoutExtension(fn)}.{serializer.Extension}");

            if (!Directory.Exists(rootDir)) Directory.CreateDirectory(rootDir!);
            if (!File.Exists(targetFile)) File.WriteAllText(targetFile, string.Empty); // harmless; will be replaced

            // Use a strong temp name, then Replace/Move for atomicity.
            var rng = RandomNumberGenerator.Create();
            var randomBytes = new byte[8];
            rng.GetNonZeroBytes(randomBytes);
            var randomIdCode = BitConverter.ToString(randomBytes).Replace("-", "").ToLowerInvariant();
            var tempFile = Path.Combine(rootDir ?? ".", $"{Path.GetFileNameWithoutExtension(fn)}_{randomIdCode}.tmp");

            var fileHash = File.ReadAllText(targetFile).GetHashCode();
            var newContent = "";
            var newContentHash = 0;

            try
            {
                using (var writer = new StreamWriter(File.Open(tempFile, FileMode.Create, FileAccess.Write, FileShare.None)))
                {
                    writer.Write(newContent = serializer.Serialize(o));
                }

                newContentHash = newContent.GetHashCode();
                if (newContentHash != fileHash)
                {
                    if (File.Exists(targetFile))
                        File.Replace(tempFile, targetFile, destinationBackupFileName: null);
                    else
                        File.Move(tempFile, targetFile);
                }
                else
                {
                    File.Delete(tempFile);
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(tempFile)) File.Delete(tempFile);

                MessageBox.Show(
                    $"Failed to save to {serializer.Extension.ToUpperInvariant()}: {ex.Message}\r\n\r\nFile: {targetFile}\r\nTemp file: {tempFile}",
                    "Whoops", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return null;
            }

            return new RawSaveResult(newContent.Length, newContentHash);
        }

        private void LoadOverrides()
        {
            var fn = AppDataPaths.SelectDataFileLoad(AppDataPaths.FileOverrides, DataPath);
            if (!File.Exists(fn))
            {
                MessageBox.Show($"Overrides file ({AppDataPaths.FileOverrides}) was not found.\r\nCreating a new one...", @"Database file missing", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CompOverride = Array.Empty<Enums.CompOverride>();
                SaveOverrides(Serializer.GetSerializer());
                return;
            }

            using var fileStream = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var binaryReader = new BinaryReader(fileStream);
            if (binaryReader.ReadString() != OverrideNames)
            {
                MessageBox.Show($"Overrides file ({AppDataPaths.FileOverrides}) was missing a header!\r\nNot loading powerset comparison overrides.", @"Database file failed to load", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void SaveRawOverrides(ISerialize serializer, string iFilename, string name)
        {
            var toSerialize = new { name, CompOverride };
            SaveRawMhd(serializer, toSerialize, iFilename, null);
        }

        private void SaveOverrides(ISerialize serializer)
        {
            var fn = AppDataPaths.SelectDataFileLoad("Compare.mhd");
            using var fileStream = new FileStream(fn, FileMode.Create, FileAccess.Write, FileShare.None);
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

        #endregion

        #region Nested types

        public class AutoUpdate
        {
            public bool Enabled => Type is AutoUpdType.Delay or AutoUpdType.Startup;
            public AutoUpdType Type { get; set; }
            public int Delay { get; set; }
            public DateTime? LastChecked { get; set; }

            public AutoUpdate(AutoUpdType type, int delay = 3)
            {
                Type = type; Delay = delay;
            }
        }

        public class SDamageMath
        {
            public SDamageMath() { }
            public SDamageMath(EDamageMath dmgMath, EDamageReturn dmgRet)
            { Calculate = dmgMath; ReturnValue = dmgRet; }

            public EDamageMath Calculate { get; set; }
            public EDamageReturn ReturnValue { get; set; }
        }

        public class IncludeExclude { public bool DisablePvE { get; set; } }

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

        public class CombatContext
        {
            public static string GetConfigChunkName(string condChunk) => condChunk.ToLowerInvariant() switch
            {
                "player" => "PlayerSettings",
                "target" => "TargetSettings",
                "hp" => "HpPercent",
                "end" => "EndPercent",
                "isalive" => "IsAlive",
                _ => ""
            };

            public static string ConfigChunkType(string condChunk) => condChunk.ToLowerInvariant() switch
            {
                "isalive" => "bool",
                _ => "int"
            };

            public static string FormatSettingName(string setting) =>
                CultureInfo.InvariantCulture.TextInfo
                    .ToTitleCase(setting.ToLowerInvariant()
                        .Replace("cfg.", "")
                        .Replace("settings", "")
                        .Replace("percent", "%")
                        .Replace('.', ' '))
                    .Replace("Isalive", "IsAlive");

            public static List<string> EnumerateFields(object obj, string prefix = "cfg")
            {
                var objType = obj.GetType();
                var properties = objType.GetProperties();
                var settings = new List<string>();

                foreach (var prop in properties)
                {
                    if (prop.PropertyType.Assembly == objType.Assembly)
                        settings.AddRange(EnumerateFields(prop.GetValue(obj, null), $"{prefix}.{prop.Name}"));
                    else
                        settings.Add($"{prefix}.{prop.Name.ToLowerInvariant()}");
                }

                return settings;
            }

            public class Player { public int HpPercent { get; set; } = 100; public int EndPercent { get; set; } = 100; public bool IsAlive { get; set; } = true; }
            public class Target { public int HpPercent { get; set; } = 100; public int EndPercent { get; set; } = 100; }

            public Player PlayerSettings { get; set; } = new();
            public Target TargetSettings { get; set; } = new();
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
                RTFBase = iFs.RTFBase; RTFBold = iFs.RTFBold;
                ColorBackgroundHero = iFs.ColorBackgroundHero; ColorBackgroundVillain = iFs.ColorBackgroundVillain;
                ColorText = iFs.ColorText; ColorInvention = iFs.ColorInvention; ColorInventionInv = iFs.ColorInventionInv;
                ColorFaded = iFs.ColorFaded; ColorEnhancement = iFs.ColorEnhancement; ColorWarning = iFs.ColorWarning;
                ColorPlName = iFs.ColorPlName; ColorPlSpecial = iFs.ColorPlSpecial;
                ColorPowerAvailable = iFs.ColorPowerAvailable; ColorPowerDisabled = iFs.ColorPowerDisabled;
                ColorPowerTakenHero = iFs.ColorPowerTakenHero; ColorPowerTakenDarkHero = iFs.ColorPowerTakenDarkHero;
                ColorPowerHighlightHero = iFs.ColorPowerHighlightHero;
                ColorPowerTakenVillain = iFs.ColorPowerTakenVillain; ColorPowerTakenDarkVillain = iFs.ColorPowerTakenDarkVillain;
                ColorPowerHighlightVillain = iFs.ColorPowerHighlightVillain;
                ColorDamageBarBase = iFs.ColorDamageBarBase; ColorDamageBarEnh = iFs.ColorDamageBarEnh;
                PairedBold = iFs.PairedBold; PairedBase = iFs.PairedBase;
                PowersSelectBase = iFs.PowersSelectBase; PowersSelectBold = iFs.PowersSelectBold;
                PowersBase = iFs.PowersBase; PowersBold = iFs.PowersBold;
            }

            public bool ValidFontSize(float fntSize) => fntSize >= 6 & fntSize <= 14;

            public Color GetDefaultColorSetting(Enums.eColorSetting clSetting) => clSetting switch
            {
                Enums.eColorSetting.ColorBackgroundHero => Color.FromArgb(0, 0, 32),
                Enums.eColorSetting.ColorBackgroundVillain => Color.FromArgb(32, 0, 0),
                Enums.eColorSetting.ColorText => Color.White,
                Enums.eColorSetting.ColorInvention => Color.Cyan,
                Enums.eColorSetting.ColorInventionInv => Color.Navy,
                Enums.eColorSetting.ColorFaded => Color.Silver,
                Enums.eColorSetting.ColorEnhancement => Color.Lime,
                Enums.eColorSetting.ColorWarning => Color.Red,
                Enums.eColorSetting.ColorPlName => Color.FromArgb(192, 192, 255),
                Enums.eColorSetting.ColorPlSpecial => Color.FromArgb(128, 128, 255),
                Enums.eColorSetting.ColorPowerAvailable => Color.Gold,
                Enums.eColorSetting.ColorPowerDisabled => Color.LightGray,
                Enums.eColorSetting.ColorPowerTakenHero => Color.FromArgb(116, 168, 234),
                Enums.eColorSetting.ColorPowerTakenDarkHero => Color.DodgerBlue,
                Enums.eColorSetting.ColorPowerHighlightHero => Color.FromArgb(64, 64, 96),
                Enums.eColorSetting.ColorPowerTakenVillain => Color.FromArgb(191, 74, 56),
                Enums.eColorSetting.ColorPowerTakenDarkVillain => Color.Maroon,
                Enums.eColorSetting.ColorPowerHighlightVillain => Color.FromArgb(96, 64, 64),
                Enums.eColorSetting.ColorDamageBarBase => Color.FromArgb(255, 194, 194),
                Enums.eColorSetting.ColorDamageBarEnh => Color.FromArgb(181, 0, 0),
                _ => Color.FromArgb(0, 0, 0)
            };

            public float GetDefaultFontSizeSetting(Enums.eFontSizeSetting fntSetting) => fntSetting switch
            {
                Enums.eFontSizeSetting.PairedBase => 8.25f,
                Enums.eFontSizeSetting.PowersSelectBase => 9.25f,
                Enums.eFontSizeSetting.PowersBase => 9.25f,
                _ => 8.50f
            };

            public void SetDefault()
            {
                RTFBase = 16; RTFBold = true;
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
                PowersSelectBase = GetDefaultFontSizeSetting(Enums.eFontSizeSetting.PowersSelectBase);
                PowersSelectBold = false;
                PowersBase = GetDefaultFontSizeSetting(Enums.eFontSizeSetting.PowersBase);
                PowersBold = true;
            }
        }

        #endregion
    }
}
