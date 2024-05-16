using Mids_Reborn.Core.Base.Master_Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Mids_Reborn.Core.Utils
{
    internal static class Helpers
    {
        public static IEnumerable<Control> GetControlHierarchy(Control root)
        {
            var queue = new Queue<Control>();
            queue.Enqueue(root);
            do
            {
                var control = queue.Dequeue();
                yield return control;
                foreach (var child in control.Controls.OfType<Control>())
                {
                    queue.Enqueue(child);
                }
            } while (queue.Count > 0);
        }

        public static IEnumerable<T> GetControlOfType<T>(Control.ControlCollection root) where T : Control
        {
            var controls = new List<T>();
            foreach (Control control in root)
            {
                if (control is T baseControl) controls.Add(baseControl);
                if (!control.HasChildren) continue;
                foreach (Control child in control.Controls)
                {
                    if (child is T childControl) controls.Add(childControl);
                }
            }

            return controls;
        }

        public static bool CompareVersions(Version versionA, Version versionB)
        {
            var comparisonResult = versionA.CompareTo(versionB);
            return comparisonResult > 0;
        }

        public static Bitmap ResizeImage(string path, Size size)
        {
            using var image = Image.FromFile(path);
            var destRect = new Rectangle(0, 0, size.Width, size.Height);
            var destImage = new Bitmap(size.Width, size.Height);
            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
            using var gfx = Graphics.FromImage(destImage);
            gfx.CompositingMode = CompositingMode.SourceCopy;
            gfx.CompositingQuality = CompositingQuality.HighQuality;
            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
            gfx.SmoothingMode = SmoothingMode.HighQuality;
            gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
            using var imgAtt = new ImageAttributes();
            imgAtt.SetWrapMode(WrapMode.TileFlipXY);
            gfx.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imgAtt);
            return destImage;
        }

        internal struct Stat
        {
            public string Type { get; set; }
            public string Percentage { get; set; }
            public string? Hex { get; set; }
            public string? ExtraData { get; set; }

            public Stat(string type, string percentage, string? extraData = null, string? hexColor = null)
            {
                Type = type;
                Percentage = percentage;
                ExtraData = extraData;
                Hex = hexColor;
            }
        }

        private static readonly Enums.eEffectType[] DebuffEffectTypes =
        {
            Enums.eEffectType.Defense,
            Enums.eEffectType.Endurance,
            Enums.eEffectType.Recovery,
            Enums.eEffectType.PerceptionRadius,
            Enums.eEffectType.ToHit,
            Enums.eEffectType.RechargeTime,
            Enums.eEffectType.SpeedRunning,
            Enums.eEffectType.Regeneration
        };

        private static void ValidDamageTypes(out Dictionary<string, int> validDamageTypes, int statType = 0)
        {
            var allTypes = Enum.GetValues(typeof(Enums.eDamage)).Cast<Enums.eDamage>().ToList();
            var excludedTypes = statType switch
            {
                1 => new List<Enums.eDamage>
                {
                    Enums.eDamage.None,
                    Enums.eDamage.Special,
                    Enums.eDamage.Melee,
                    Enums.eDamage.Ranged,
                    Enums.eDamage.AoE,
                    Enums.eDamage.Unique1,
                    Enums.eDamage.Unique2,
                    Enums.eDamage.Unique3
                },
                _ => new List<Enums.eDamage>
                {
                    Enums.eDamage.None,
                    DatabaseAPI.RealmUsesToxicDef() ? Enums.eDamage.None : Enums.eDamage.Toxic,
                    Enums.eDamage.Special,
                    Enums.eDamage.Unique1,
                    Enums.eDamage.Unique2,
                    Enums.eDamage.Unique3
                }
            };

            var damageTypes = allTypes.Except(excludedTypes).ToList();
            validDamageTypes = damageTypes.ToDictionary(damageType => damageType.ToString(),
                damageType => (int)Enum.Parse<Enums.eDamage>(damageType.ToString()));
        }

        public static Dictionary<string, List<Stat>> GeneratedStatData(bool infoGraphic = false)
        {
            var stats = new Dictionary<string, List<Stat>>();
            var totalStat = MidsContext.Character?.Totals;
            var displayStat = MidsContext.Character?.DisplayStats;
            ValidDamageTypes(out var defTypes);
            ValidDamageTypes(out var resTypes, 1);
            List<string> debuffTypes;
            if (!infoGraphic)
                debuffTypes = new List<string>
                {
                    "Defense", "Endurance", "Recovery", "Perception", "ToHit", "Recharge", "Movement", "Regeneration"
                };
            else
                debuffTypes = new List<string>
                {
                    "Defense", "Endurance", "Recovery", "Perception", "ToHit", "Recharge", "Movement", "Regen"
                };

            var statList = (from defType in defTypes
                let multiplied = totalStat.Def[defType.Value] * 100f
                let percentage = $"{Convert.ToDecimal(multiplied):0.##}%"
                select new Stat(defType.Key, percentage, null, "#a954d1")).ToList();
            stats.Add("Defense", statList);
            statList = (from resType in resTypes
                let multiplied = totalStat.Res[resType.Value] * 100f
                let percentage = $"{Convert.ToDecimal(multiplied):0.##}%"
                select new Stat(resType.Key, percentage, null, "#54b0d1")).ToList();
            stats.Add("Resistance", statList);
            if (!infoGraphic)
            {
                statList = new List<Stat>
                {
                    new("Max HP", $"{Convert.ToDecimal(displayStat?.HealthHitpointsPercentage):0.##}%",
                        $" ({Convert.ToDecimal(displayStat?.HealthHitpointsNumeric(false)):0.##} HP)", "#79d154"),
                    new("Regeneration", $"{Convert.ToDecimal(displayStat?.HealthRegenPercent(false)):0.##}%",
                        $" ({Convert.ToDecimal(displayStat?.HealthRegenHPPerSec):0.##}/s)", "#79d154"),
                    new("Max End", $"{Convert.ToDecimal(totalStat?.EndMax + 100f):0.##}%", null, "#549dd1"),
                    new("Recovery", $"{displayStat?.EnduranceRecoveryPercentage(false):###0}%",
                        $" ({Convert.ToDecimal(displayStat?.EnduranceRecoveryNumeric):0.##}/s)", "#549dd1"),
                    new("End Usage", $"{Convert.ToDecimal(displayStat?.EnduranceUsage):0.##}/s", null, "#549dd1")
                };
            }
            else
            {
                statList = new List<Stat>
                {
                    new("Max HP", $"{Convert.ToDecimal(displayStat?.HealthHitpointsNumeric(false)):0.##}"),
                    new("Regen", $"{Convert.ToDecimal(displayStat?.HealthRegenPercent(false)):0.##}%"),
                    new("↑ HP/s", $"{Convert.ToDecimal(displayStat?.HealthRegenHPPerSec):0.##}/s"),
                    new("Max End", $"{Convert.ToDecimal(totalStat?.EndMax + 100f):0.##}%"),
                    new("Recovery", $"{displayStat?.EnduranceRecoveryPercentage(false):###0}%"),
                    new("↑ End/s", $"{Convert.ToDecimal(displayStat?.EnduranceRecoveryNumeric):0.##}/s"),
                    new("End Usage", $"{Convert.ToDecimal(displayStat?.EnduranceUsage):0.##}/s")
                };
            }

            stats.Add("Sustain", statList);
            if (!infoGraphic)
            {
                statList = new List<Stat>
                {
                    new("Haste", $"{Convert.ToDecimal(totalStat?.BuffHaste * 100f):0.##}%", null, "#d18254"),
                    new("Damage", $"{Convert.ToDecimal(totalStat?.BuffDam * 100f):0.##}%", null, "#d16054"),
                    new("To Hit", $"{Convert.ToDecimal(totalStat?.BuffToHit * 100f):0.##}%", null, "#d1be54"),
                    new("Accuracy", $"{Convert.ToDecimal(totalStat?.BuffAcc * 100f):0.##}%", null, "#d1be54"),
                    new("End Reduction", $"{Convert.ToDecimal(totalStat?.BuffEndRdx * 100f):0.##}%", null, "#549dd1")

                };
            }
            else
            {
                statList = new List<Stat>
                {
                    new("Haste", $"{Convert.ToDecimal(totalStat?.BuffHaste * 100f):0.##}%"),
                    new("Damage", $"{Convert.ToDecimal(totalStat?.BuffDam * 100f):0.##}%"),
                    new("To Hit", $"{Convert.ToDecimal(totalStat?.BuffToHit * 100f):0.##}%"),
                    new("Accuracy", $"{Convert.ToDecimal(totalStat?.BuffAcc * 100f):0.##}%"),
                    new("End Redux", $"{Convert.ToDecimal(totalStat?.BuffEndRdx * 100f):0.##}%")

                };
            }

            stats.Add("Offense", statList);
            statList = DebuffEffectTypes.Select(t => totalStat?.DebuffRes[(int)t])
                .Select(notMultiplied => $"{Convert.ToDecimal(notMultiplied):0.##}%").Select((percentage, index) =>
                    new Stat(debuffTypes[index], percentage, null, "#8e54d1")).ToList();
            stats.Add(!infoGraphic ? "Debuff Resistance" : "Debuff Resist", statList);

            return stats;
        }

        /// <summary>
        /// Gets the project path
        /// Note: When calling do not pass anything to method
        /// </summary>
        internal static string GetPathInDebug([CallerFilePath] string? callPath = null)
        {
            var path = callPath ?? string.Empty;
            var fileInfo = new FileInfo(path);
            var toRem = Path.Combine(fileInfo.Directory!.Name, fileInfo.Name);
            return path.Replace(toRem, string.Empty);
        }
    }
}