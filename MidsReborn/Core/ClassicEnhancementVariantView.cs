using System;
using System.Collections.Generic;
using System.Linq;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core
{
    public sealed class ClassicEnhancementVariantView
    {
        public string CanonicalUid { get; init; } = string.Empty;
        public int EnhancementIndex { get; init; } = -1;
        public string SourceKey { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Tier { get; init; } = string.Empty;
        public string DisplayName { get; init; } = string.Empty;
        public string Icon { get; init; } = string.Empty;
        public string Flavor { get; init; } = string.Empty;
        public string PrimaryOrigin { get; init; } = string.Empty;
        public string SecondaryOrigin { get; init; } = string.Empty;
        public bool HasExplicitPrimaryOrigin { get; init; }
        public string[] CompatibleOrigins { get; init; } = Array.Empty<string>();
        public Enums.eEnhGrade DisplayGrade { get; init; }

        public string DisplayTypeLabel => Tier;

        public string DisplayNameWithFlavor =>
            string.IsNullOrWhiteSpace(Flavor) || string.Equals(Flavor, "Training", StringComparison.OrdinalIgnoreCase)
                ? DisplayName
                : $"{DisplayName} [{Flavor}]";

        public bool MatchesOrigin(string? originName)
        {
            if (string.IsNullOrWhiteSpace(originName) || CompatibleOrigins.Length == 0)
            {
                return false;
            }

            return CompatibleOrigins.Any(origin => string.Equals(origin, originName, StringComparison.OrdinalIgnoreCase));
        }

        internal ClassicEnhancementVariantView WithDisplayOrigins(string primaryOrigin, string secondaryOrigin, bool hasExplicitPrimaryOrigin)
        {
            var compatibleOrigins = new[] { primaryOrigin, secondaryOrigin }
                .Where(origin => !string.IsNullOrWhiteSpace(origin))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            return new ClassicEnhancementVariantView
            {
                CanonicalUid = CanonicalUid,
                EnhancementIndex = EnhancementIndex,
                SourceKey = SourceKey,
                Name = Name,
                Tier = Tier,
                DisplayName = DisplayName,
                Icon = Icon,
                Flavor = ClassicEnhancementVariantResolver.BuildFlavor(
                    new ClassicEnhancementOriginDescriptor
                    {
                        PrimaryOrigin = primaryOrigin,
                        SecondaryOrigin = secondaryOrigin,
                        HasExplicitPrimaryOrigin = hasExplicitPrimaryOrigin,
                        CompatibleOrigins = compatibleOrigins
                    },
                    Tier),
                PrimaryOrigin = primaryOrigin,
                SecondaryOrigin = secondaryOrigin,
                HasExplicitPrimaryOrigin = hasExplicitPrimaryOrigin,
                CompatibleOrigins = compatibleOrigins,
                DisplayGrade = DisplayGrade
            };
        }
    }

    internal sealed class ClassicEnhancementOriginDescriptor
    {
        public string PrimaryOrigin { get; init; } = string.Empty;
        public string SecondaryOrigin { get; init; } = string.Empty;
        public bool HasExplicitPrimaryOrigin { get; init; }
        public string[] CompatibleOrigins { get; init; } = Array.Empty<string>();
    }

    internal static class ClassicEnhancementVariantResolver
    {
        private static readonly Dictionary<string, string> OriginNames = new(StringComparer.OrdinalIgnoreCase)
        {
            ["magic"] = "Magic",
            ["mutation"] = "Mutation",
            ["natural"] = "Natural",
            ["science"] = "Science",
            ["technology"] = "Technology"
        };

        internal static string NormalizeTier(string? tier)
        {
            return tier?.Trim().ToUpperInvariant() switch
            {
                "TR" => "TO",
                "TO" => "TO",
                "DO" => "DO",
                "SO" => "SO",
                _ => string.Empty
            };
        }

        internal static Enums.eEnhGrade ToEnhGrade(string? tier)
        {
            return NormalizeTier(tier) switch
            {
                "TO" => Enums.eEnhGrade.TrainingO,
                "DO" => Enums.eEnhGrade.DualO,
                "SO" => Enums.eEnhGrade.SingleO,
                _ => Enums.eEnhGrade.None
            };
        }

        internal static string BuildFlavor(ClassicEnhancementOriginDescriptor descriptor, string normalizedTier)
        {
            if (normalizedTier == "TO")
            {
                return "Training";
            }

            return descriptor.CompatibleOrigins.Length switch
            {
                1 => descriptor.CompatibleOrigins[0],
                > 1 => string.Join("/", descriptor.CompatibleOrigins),
                _ => string.Empty
            };
        }

        internal static ClassicEnhancementVariantView ReorderForDisplayOrigin(
            ClassicEnhancementVariantView variant,
            string? originName)
        {
            if (variant == null ||
                !string.Equals(variant.Tier, "DO", StringComparison.OrdinalIgnoreCase) ||
                variant.HasExplicitPrimaryOrigin ||
                string.IsNullOrWhiteSpace(originName) ||
                !variant.MatchesOrigin(originName))
            {
                return variant;
            }

            if (string.Equals(variant.PrimaryOrigin, originName, StringComparison.OrdinalIgnoreCase))
            {
                return variant;
            }

            if (string.Equals(variant.SecondaryOrigin, originName, StringComparison.OrdinalIgnoreCase))
            {
                return variant.WithDisplayOrigins(variant.SecondaryOrigin, variant.PrimaryOrigin, false);
            }

            if (variant.CompatibleOrigins.Length < 2)
            {
                return variant;
            }

            var primaryOrigin = variant.CompatibleOrigins.FirstOrDefault(origin =>
                string.Equals(origin, originName, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrWhiteSpace(primaryOrigin))
            {
                return variant;
            }

            var secondaryOrigin = variant.CompatibleOrigins.FirstOrDefault(origin =>
                !string.Equals(origin, primaryOrigin, StringComparison.OrdinalIgnoreCase)) ?? string.Empty;
            return variant.WithDisplayOrigins(primaryOrigin, secondaryOrigin, false);
        }

        internal static ClassicEnhancementVariantView CreateView(
            string canonicalUid,
            int enhancementIndex,
            ClassicEnhancementSourceVariantMetadata metadata)
        {
            var tier = NormalizeTier(string.IsNullOrWhiteSpace(metadata.NormalizedTier) ? metadata.Tier : metadata.NormalizedTier);
            var descriptor = BuildOriginDescriptor(metadata);
            var flavor = string.IsNullOrWhiteSpace(metadata.Flavor)
                ? BuildFlavor(descriptor, tier)
                : metadata.Flavor;

            return new ClassicEnhancementVariantView
            {
                CanonicalUid = canonicalUid,
                EnhancementIndex = enhancementIndex,
                SourceKey = metadata.SourceKey,
                Name = metadata.Name,
                Tier = tier,
                DisplayName = string.IsNullOrWhiteSpace(metadata.DisplayName) ? metadata.Name : metadata.DisplayName,
                Icon = metadata.Icon,
                Flavor = flavor,
                PrimaryOrigin = descriptor.PrimaryOrigin,
                SecondaryOrigin = descriptor.SecondaryOrigin,
                HasExplicitPrimaryOrigin = descriptor.HasExplicitPrimaryOrigin,
                CompatibleOrigins = descriptor.CompatibleOrigins,
                DisplayGrade = ToEnhGrade(tier)
            };
        }

        internal static ClassicEnhancementOriginDescriptor BuildOriginDescriptor(
            ClassicEnhancementSourceVariantMetadata metadata)
        {
            var tier = NormalizeTier(string.IsNullOrWhiteSpace(metadata.NormalizedTier) ? metadata.Tier : metadata.NormalizedTier);

            if (tier == "TO")
            {
                return new ClassicEnhancementOriginDescriptor
                {
                    CompatibleOrigins = Array.Empty<string>()
                };
            }

            if (!string.IsNullOrWhiteSpace(metadata.PrimaryOrigin) ||
                !string.IsNullOrWhiteSpace(metadata.SecondaryOrigin) ||
                (metadata.CompatibleOrigins?.Count ?? 0) > 0)
            {
                return new ClassicEnhancementOriginDescriptor
                {
                    PrimaryOrigin = metadata.PrimaryOrigin,
                    SecondaryOrigin = metadata.SecondaryOrigin,
                    HasExplicitPrimaryOrigin = metadata.HasExplicitPrimaryOrigin,
                    CompatibleOrigins = (metadata.CompatibleOrigins ?? Enumerable.Empty<string>())
                        .Where(origin => !string.IsNullOrWhiteSpace(origin))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToArray()
                };
            }

            return BuildOriginDescriptor(metadata.Name, metadata.SourceKey, metadata.Flavor, tier);
        }

        internal static ClassicEnhancementOriginDescriptor BuildOriginDescriptor(
            string? sourceName,
            string? sourceKey,
            string? flavor,
            string? tier)
        {
            var normalizedTier = NormalizeTier(tier);
            if (normalizedTier == "TO")
            {
                return new ClassicEnhancementOriginDescriptor
                {
                    CompatibleOrigins = Array.Empty<string>()
                };
            }

            if (TryParseOrderedOrigins(sourceName, normalizedTier, out var primaryOrigin, out var secondaryOrigin) ||
                TryParseOrderedOrigins(sourceKey, normalizedTier, out primaryOrigin, out secondaryOrigin))
            {
                var compatibleOrigins = string.IsNullOrWhiteSpace(secondaryOrigin)
                    ? new[] { primaryOrigin }
                    : new[] { primaryOrigin, secondaryOrigin };
                return new ClassicEnhancementOriginDescriptor
                {
                    PrimaryOrigin = primaryOrigin,
                    SecondaryOrigin = secondaryOrigin,
                    HasExplicitPrimaryOrigin = !string.IsNullOrWhiteSpace(primaryOrigin),
                    CompatibleOrigins = compatibleOrigins
                };
            }

            var flavorOrigins = ParseOriginsFromFlavor(flavor);
            if (flavorOrigins.Length > 0)
            {
                return new ClassicEnhancementOriginDescriptor
                {
                    PrimaryOrigin = flavorOrigins[0],
                    SecondaryOrigin = flavorOrigins.Length > 1 ? flavorOrigins[1] : string.Empty,
                    HasExplicitPrimaryOrigin = normalizedTier == "SO" && flavorOrigins.Length == 1,
                    CompatibleOrigins = flavorOrigins
                };
            }

            return new ClassicEnhancementOriginDescriptor
            {
                CompatibleOrigins = Array.Empty<string>()
            };
        }

        internal static ClassicEnhancementVariantView? ResolvePreferredVariant(
            IReadOnlyList<ClassicEnhancementVariantView> variants,
            Enums.eEnhGrade grade,
            string? originName,
            string? preferredVariantName = null)
        {
            if (variants.Count == 0)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(preferredVariantName))
            {
                var exactMatch = variants.FirstOrDefault(variant =>
                    string.Equals(variant.Name, preferredVariantName, StringComparison.OrdinalIgnoreCase));
                if (exactMatch != null)
                {
                    return exactMatch;
                }
            }

            var desiredTier = grade switch
            {
                Enums.eEnhGrade.TrainingO => "TO",
                Enums.eEnhGrade.DualO => "DO",
                Enums.eEnhGrade.SingleO => "SO",
                _ => string.Empty
            };

            var tierVariants = variants
                .Where(variant => string.Equals(variant.Tier, desiredTier, StringComparison.OrdinalIgnoreCase))
                .ToArray();
            if (tierVariants.Length == 0)
            {
                return null;
            }

            if (desiredTier == "TO")
            {
                return tierVariants[0];
            }

            if (desiredTier == "SO")
            {
                return tierVariants.FirstOrDefault(variant => variant.MatchesOrigin(originName)) ?? tierVariants[0];
            }

            var explicitPrimaryMatch = tierVariants.FirstOrDefault(variant =>
                variant.HasExplicitPrimaryOrigin &&
                string.Equals(variant.PrimaryOrigin, originName, StringComparison.OrdinalIgnoreCase));
            if (explicitPrimaryMatch != null)
            {
                return explicitPrimaryMatch;
            }

            var compatibleMatch = tierVariants.FirstOrDefault(variant => variant.MatchesOrigin(originName));
            if (compatibleMatch != null)
            {
                return ReorderForDisplayOrigin(compatibleMatch, originName);
            }

            return ReorderForDisplayOrigin(tierVariants[0], originName);
        }

        internal static bool TryNormalizeOriginToken(string? token, out string normalizedOrigin)
        {
            normalizedOrigin = string.Empty;
            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            return OriginNames.TryGetValue(token.Trim(), out normalizedOrigin);
        }

        private static bool TryParseOrderedOrigins(
            string? value,
            string normalizedTier,
            out string primaryOrigin,
            out string secondaryOrigin)
        {
            primaryOrigin = string.Empty;
            secondaryOrigin = string.Empty;
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var segments = value.Split('_', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (segments.Length == 0)
            {
                return false;
            }

            if (normalizedTier == "SO")
            {
                return TryNormalizeOriginToken(segments[0], out primaryOrigin);
            }

            if (normalizedTier != "DO" || segments.Length < 2)
            {
                return false;
            }

            return TryNormalizeOriginToken(segments[0], out primaryOrigin) &&
                   TryNormalizeOriginToken(segments[1], out secondaryOrigin) &&
                   !string.Equals(primaryOrigin, secondaryOrigin, StringComparison.OrdinalIgnoreCase);
        }

        private static string[] ParseOriginsFromFlavor(string? flavor)
        {
            if (string.IsNullOrWhiteSpace(flavor) ||
                string.Equals(flavor, "Training", StringComparison.OrdinalIgnoreCase))
            {
                return Array.Empty<string>();
            }

            return flavor
                .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(origin => TryNormalizeOriginToken(origin, out var normalizedOrigin) ? normalizedOrigin : string.Empty)
                .Where(origin => !string.IsNullOrWhiteSpace(origin))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
    }
}
