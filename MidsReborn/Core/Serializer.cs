using Mids_Reborn.Core.Base.Data_Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;

namespace Mids_Reborn.Core
{
    public class Serializer : ISerialize
    {
        private readonly Func<object, string> _serializeFunc;

        public Serializer(Func<object, string> serializeFunc, string extension)
        {
            Extension = extension;
            _serializeFunc = serializeFunc;
        }

        public string Extension { get; }

        public string Serialize(object o)
        {
            return _serializeFunc(o);
        }

        public T Deserialize<T>(string x)
        {
            return JsonConvert.DeserializeObject<T>(x);
        }

        public static ISerialize GetSerializer()
        {
            // Preserve your existing default writer settings for everything except ConfigData
            var defaultSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                Formatting = Formatting.Indented
            };

            // Identical to defaults, except we impose a grouped order for ConfigData
            var configDataSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Include,
                Formatting = Formatting.Indented,
                ContractResolver = new ConfigDataOrderingResolver()
            };

            return new Serializer(obj => JsonConvert.SerializeObject(obj, obj is ConfigData ? configDataSettings : defaultSettings), "json");
        }

        public static readonly JsonSerializerSettings? SerializerSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            Converters =
            {
                new AbstractConverter<Database, IDatabase>(),
                new AbstractConverter<Enhancement, IEnhancement>(),
                new AbstractConverter<Powerset, IPowerset>(),
                new AbstractConverter<Power, IPower>(),
                new AbstractConverter<Effect, IEffect>()
            },
            Formatting = Formatting.Indented
        };

        private class AbstractConverter<TReal, TAbstract> : JsonConverter where TReal : TAbstract
        {
            public override bool CanConvert(Type objectType) => objectType == typeof(TAbstract);

            public override object? ReadJson(JsonReader reader, Type type, object? value, JsonSerializer jser) => jser.Deserialize<TReal>(reader);

            public override void WriteJson(JsonWriter writer, object? value, JsonSerializer jser) => jser.Serialize(writer, value);
        }

        /// <summary>
        /// Applies a readable, stable, grouped property/field order to ConfigData serialization.
        /// No attributes or code changes required in ConfigData.cs.
        /// </summary>
        private sealed class ConfigDataOrderingResolver : DefaultContractResolver
        {
            // Group bases — lower numbers come first
            private const int GeneralBase = 0000;
            private const int MathBase = 1000;
            private const int BuildBase = 2000;
            private const int GraphsBase = 3000;
            private const int ExportBase = 4000;
            private const int I9Base = 5000;
            private const int PathsBase = 6000;
            private const int AutoUpdateBase = 7000;
            private const int OverridesBase = 7300;
            private const int FontsThemeBase = 7600;
            private const int MiscBase = 9000;

            // Everything below lists actual member names found in your ConfigData.cs

            // General / App
            private readonly string[] _general =
            {
                "FirstRun",
                "Mode",
                "MasterMode", // has only getter; will be skipped by Newtonsoft (no setter). Keeping here is harmless.
                "IsInitialized",
                "SelectedTheme",
                "WindowState",
                "Bounds",
                "TotalsWindowTitleStyle",
                "UseOldTotalsWindow",
                "PowerListsWordwrapMode",
                "NoToolTips",
                "DisableTips",
                "DisableVillainColors",
                "DisableDesaturateInherent",
                "RotationHelperLocation",
                "EntityDetailsLocation",
                "PetActorDetailsLocation",
                "PetActorDetailsSize",
                "DvState",
                "Suppression",
                "UseArcanaTime",
                "ShowSelfBuffsAny",
                "ShrinkFrmSets",
                "WarnOnOldDbMbd",
                "DimWindowStyleColors",
                "CloseEnhSelectPopupByMove",
                "Tips",
                "SaveFolderChecked"
            };

            // Math / Simulation
            private readonly string[] _math =
            {
                "DamageMath",
                "EnemyRelativeLevel",
                "ScalingToHit",
                "ExempHigh",
                "ExempLow",
                "ForceLevel",
                "TeamSize",
                "Inc",
                "TeamMembers",
                "TeamRoster",
                "CombatContextSettings",
                "RelativeLevels" // internal readonly; won’t serialize (non-public). Keeping here is harmless.
            };

            // Build / Editing UX
            private readonly string[] _buildUx =
            {
                "BuildMode",
                "BuildOption",
                "ShowSlotsLeft",
                "ShowSlotLevels",
                "ShowEnhRel",
                "ShowRelSymbols",
                "ShowSoLevels",
                "EnhanceVisibility",
                "DisableShowPopup",
                "DisableAlphaPopup",
                "DisableRepeatOnMiddleClick",
                "Columns",
                "ColumnStackingMode"
            };

            // Graphs / Printing
            private readonly string[] _graphsPrint =
            {
                "DisableDataDamageGraph",
                "DataDamageGraphPercentageOnly",
                "DataGraphType",
                "StatGraphStyle",
                "PrintProfile",
                "DisablePrintProfileEnh",
                "LastPrinter",
                "PrintInColor",
                "PrintHistory"
            };

            // Export / Share
            private readonly string[] _exportShare =
            {
                "Export",
                "ShareConfig",
                "LongExport",
                "ExportScheme",
                "ExportTarget",
                "ExportBonusTotals",
                "ExportBonusList",
                "PopupRecipes",
                "ShoppingListIncludesRecipes"
            };

            // I9 / Enhancement
            private readonly string[] _i9Enh =
            {
                "I9",
                "CalcEnhOrigin",
                "CalcEnhLevel",
                "CoDEffectFormat",    // field
                "SpeedFormat"         // field
            };

            // Paths / Files / Session
            private readonly string[] _paths =
            {
                "DataPath",
                "SavePath",
                "BuildsPath",
                "UpdatePath",
                "LastFileName",
                "DisableLoadLastFileOnStart",
                "PreferredCurrency"   // field
            };

            // Auto-Update
            private readonly string[] _autoUpdate =
            {
                "AutomaticUpdates"
            };

            // Overrides
            private readonly string[] _overrides =
            {
                "CompOverride"
            };

            // Fonts / Theme (RTF / colors / sizes)
            private readonly string[] _fontsTheme =
            {
                "RtFont",
                "DisableVillainColors" // already in _general; included there. (We keep unique mapping below.)
            };

            // Standalone public field (kept near Misc if not matched elsewhere)
            private readonly string[] _publicFields =
            {
                "DragDropScenarioAction"
            };

            private static readonly HashSet<string> ExcludedBuildScopedCombatContextProperties =
            [
                "EnemyRelativeLevel",
                "TeamSize",
                "TeamMembers",
                "TeamRoster",
                "CombatContextSettings"
            ];

            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var props = base.CreateProperties(type, memberSerialization);

                if (type.Name != nameof(ConfigData))
                    return props;

                props = props
                    .Where(prop => !ExcludedBuildScopedCombatContextProperties.Contains(prop.PropertyName))
                    .ToList();

                foreach (var prop in props)
                {
                    prop.Order = ComputeOrder(prop.PropertyName);
                }

                return props
                    .OrderBy(p => p.Order)
                    .ThenBy(p => p.PropertyName, StringComparer.Ordinal)
                    .ToList();
            }

            private int ComputeOrder(string name)
            {
                // De-duplicate across groups by first match wins
                if (In(_general, name, out var idx)) return GeneralBase + idx;
                if (In(_math, name, out idx)) return MathBase + idx;
                if (In(_buildUx, name, out idx)) return BuildBase + idx;
                if (In(_graphsPrint, name, out idx)) return GraphsBase + idx;
                if (In(_exportShare, name, out idx)) return ExportBase + idx;
                if (In(_i9Enh, name, out idx)) return I9Base + idx;
                if (In(_paths, name, out idx)) return PathsBase + idx;
                if (In(_autoUpdate, name, out idx)) return AutoUpdateBase + idx;
                if (In(_overrides, name, out idx)) return OverridesBase + idx;
                if (In(_fontsTheme, name, out idx)) return FontsThemeBase + idx;
                if (In(_publicFields, name, out idx)) return MiscBase + idx; // put fields like DragDropScenarioAction just after groups

                // Unmapped future members → stable tail bucket
                return MiscBase + StableSubIndex(name);
            }

            private static bool In(string[] list, string name, out int index)
            {
                for (var i = 0; i < list.Length; i++)
                {
                    if (string.Equals(list[i], name, StringComparison.Ordinal))
                    {
                        index = i;
                        return true;
                    }
                }
                index = -1;
                return false;
            }

            private static int StableSubIndex(string name)
            {
                unchecked
                {
                    int h = 17;
                    foreach (var ch in name) h = h * 31 + ch;
                    return Math.Abs(h % 1000);
                }
            }
        }
    }
}
