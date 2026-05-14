using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core
{
    public static class I9Gfx
    {
        private const int IconLarge = 64;
        private const int IconSmall = 32;

        private const string ImageFilter = "*.png";
        private static List<ImageInfo> Images { get; set; } = [];
        private static bool Initialized { get; set; }

        public static int OriginIndex;
        public static Bitmap[]? Enhancements = [];
        public static ExtendedBitmap Borders = new();
        public static ExtendedBitmap Sets = new();
        public static ExtendedBitmap Classes = new();
        public static ExtendedBitmap SetTypes = new();
        public static ExtendedBitmap EnhTypes = new();
        public static ExtendedBitmap EnhGrades = new();
        public static ExtendedBitmap EnhSpecials = new();
        public static ExtendedBitmap Archetypes = new();
        public static ExtendedBitmap Origins = new();
        public static ExtendedBitmap Powersets = new();
        public static ExtendedBitmap UnknownPowerset = new();
        public static ExtendedBitmap UnknownArchetype = new();

        public static Bitmap EmptySlot { get; private set; }
        public static Bitmap NewSlot { get; private set; }

        private struct ImageInfo
        {
            public string FileName { get; set; }
            public string Directory { get; set; }
            public string? Path { get; set; }
            public bool IsBase { get; set; }
        }

        private static string BaseImagePath => AppDataPaths.BaseAssetsPath;

        public static string ImagePath(string type = "")
        {
            return !string.IsNullOrWhiteSpace(type) ? Path.Combine(BaseImagePath, type) : BaseImagePath;
        }

        private static int GetDpiScaledSize(int logicalSize)
        {
            using var g = Graphics.FromHwnd(IntPtr.Zero);
            float scale = g.DpiX / 96f;
            return (int)(logicalSize * scale);
        }

        private static Bitmap ResizeTo(Bitmap original, int targetSizeLogical)
        {
            float scale = Graphics.FromHwnd(IntPtr.Zero).DpiX / 96f;
            int targetSize = (int)(targetSizeLogical * scale);

            if (original.Width == targetSize && original.Height == targetSize)
            {
                return new Bitmap(original); // shallow copy if needed
            }

            var result = new Bitmap(targetSize, targetSize);
            result.SetResolution(original.HorizontalResolution, original.VerticalResolution);

            using var g = Graphics.FromImage(result);
            ConfigureGraphics(g);
            g.DrawImage(original, 0, 0, targetSize, targetSize);
            return result;
        }

        private static IEnumerable<ImageInfo> GetBaseImages()
        {
            var retList = new List<ImageInfo>();
            var files = Directory.GetFiles(BaseImagePath, ImageFilter, SearchOption.AllDirectories).ToList();
            foreach (var file in files)
            {
                var fInfo = new FileInfo(file);
                if (fInfo.Directory != null)
                {
                    retList.Add(new ImageInfo { FileName = fInfo.Name, Directory = fInfo.Directory.Name, Path = file, IsBase = true });
                }
            }

            return retList;
        }

        private static IEnumerable<ImageInfo> GetExtendedImages(string? path)
        {
            var retList = new List<ImageInfo>();
            if (path == null) return retList;
            var files = Directory.GetFiles(path, ImageFilter, SearchOption.AllDirectories).ToList();
            foreach (var file in files)
            {
                var fInfo = new FileInfo(file);
                if (fInfo.Directory != null)
                {
                    retList.Add(new ImageInfo { FileName = fInfo.Name, Directory = fInfo.Directory.Name, Path = file, IsBase = false });
                }
            }

            return retList;
        }

        private static string NormalizeImageBucket(string directory)
        {
            var key = NormalizeImageNameKey(directory);
            return key switch
            {
                "archetypes" => "archetypes",
                "origins" => "origins",
                "powersets" => "powersets",
                "enhancements" => "enhancements",
                "classes" => "classes",
                "sets" => "sets",
                "overlay" => "enhancement_borders",
                "border" => "enhancement_borders",
                "borders" => "enhancement_borders",
                "enhancementborder" => "enhancement_borders",
                "enhancementborders" => "enhancement_borders",
                "enhancementcategory" => "enhancement_categories",
                "enhancementcategories" => "enhancement_categories",
                _ => key
            };
        }

        private static bool TryFindImagePath(IEnumerable<ImageInfo> images, IEnumerable<string> candidates, out string path)
        {
            path = string.Empty;
            var orderedImages = images.OrderBy(i => i.IsBase ? 1 : 0).ToList();
            foreach (var candidate in candidates.Where(c => !string.IsNullOrWhiteSpace(c)).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var exact = orderedImages.FirstOrDefault(i => i.FileName.Equals(candidate, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(exact.Path))
                {
                    path = exact.Path!;
                    return true;
                }

                var normalizedCandidate = NormalizeImageNameKey(candidate);
                if (string.IsNullOrWhiteSpace(normalizedCandidate))
                {
                    continue;
                }

                var normalized = orderedImages.FirstOrDefault(i =>
                    NormalizeImageNameKey(i.FileName).Equals(normalizedCandidate, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(normalized.Path))
                {
                    path = normalized.Path!;
                    return true;
                }
            }

            return false;
        }

        private static string NormalizeImageNameKey(string? value)
        {
            var raw = Path.GetFileNameWithoutExtension(value ?? string.Empty);
            if (string.IsNullOrWhiteSpace(raw))
            {
                return string.Empty;
            }

            return new string(raw.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
        }

        private static IEnumerable<string> BuildArchetypeImageCandidates(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                yield break;
            }

            yield return $"{className}.png";

            var trimmed = className.StartsWith("Class_", StringComparison.OrdinalIgnoreCase)
                ? className["Class_".Length..]
                : className;
            yield return $"{trimmed}.png";
            yield return $"archetypeicon_{trimmed.ToLowerInvariant()}.png";
            yield return $"v_archetypeicon_{trimmed.ToLowerInvariant()}.png";
        }

        private static IEnumerable<string> BuildOriginImageCandidates(string originName)
        {
            if (string.IsNullOrWhiteSpace(originName))
            {
                yield break;
            }

            yield return $"{originName}.png";
            yield return $"{originName.ToLowerInvariant()}.png";
        }

        private static IEnumerable<string> BuildPowersetImageCandidates(string imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
            {
                yield break;
            }

            yield return imageName;
            var fileName = Path.GetFileName(imageName);
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                yield return fileName;
                yield return Path.ChangeExtension(fileName, ".png") ?? fileName;
            }
        }

        private static IEnumerable<string> BuildEnhancementTypeImageCandidates(Enums.eType type)
        {
            yield return $"{type}.png";
            switch (type)
            {
                case Enums.eType.Normal:
                    yield return "normal.png";
                    break;
                case Enums.eType.InventO:
                    yield return "io.png";
                    break;
                case Enums.eType.SetO:
                    yield return "io_sets.png";
                    break;
                case Enums.eType.SpecialO:
                    yield return "special.png";
                    break;
            }
        }

        private static IEnumerable<string> BuildEnhancementGradeImageCandidates(Enums.eEnhGrade grade)
        {
            yield return $"{grade}.png";
            switch (grade)
            {
                case Enums.eEnhGrade.TrainingO:
                    yield return "to.png";
                    yield return "normal.png";
                    break;
                case Enums.eEnhGrade.DualO:
                    yield return "do.png";
                    break;
                case Enums.eEnhGrade.SingleO:
                    yield return "so.png";
                    break;
            }
        }

        private static IEnumerable<string> BuildBorderImageCandidates(Origin origin, int gradeIndex)
        {
            if (origin == null || gradeIndex < 0 || gradeIndex >= origin.Grades.Length)
            {
                yield break;
            }

            yield return $"{origin.Grades[gradeIndex]}.png";

            switch (gradeIndex)
            {
                case 0:
                    yield return "generic.png";
                    break;
                case 2:
                    var originStem = origin.Name switch
                    {
                        "Technology" => "tech",
                        "Mutation" => "mutant",
                        _ => origin.Name.ToLowerInvariant()
                    };
                    yield return $"{originStem}.png";
                    break;
                case 3:
                    yield return "uber.png";
                    break;
                case 4:
                case 5:
                    yield return "invention.png";
                    break;
                case 6:
                    yield return "attuned.png";
                    break;
            }
        }

        public static void SetOrigin(string iOrigin)
        {
            OriginIndex = DatabaseAPI.GetOriginIDByName(iOrigin);
        }

        public static async Task Initialize(string? path)
        {
            var baseImages = GetBaseImages().ToList();
            var extendedImages = GetExtendedImages(path).ToList();
            Images = baseImages.Concat(extendedImages).ToList();
            Initialized = true;
            await Task.CompletedTask;
        }

        public static async Task LoadImages()
        {
            if (!Initialized)
            {
                MessageBox.Show(@"Reason: Attempted to access I9GFX before initialization.", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var baseImages = Images.Where(x => x.IsBase).ToList();
            var archetypeImages = Images.Where(x => NormalizeImageBucket(x.Directory) == "archetypes").ToList();
            var classImages = Images.Where(x => NormalizeImageBucket(x.Directory) == "classes").ToList();
            var enhancementImages = Images.Where(x => NormalizeImageBucket(x.Directory) == "enhancements").ToList();
            var originImages = Images.Where(x => NormalizeImageBucket(x.Directory) == "origins").ToList();
            var powersetImages = Images.Where(x => NormalizeImageBucket(x.Directory) == "powersets").ToList();
            var setImages = Images.Where(x => NormalizeImageBucket(x.Directory) == "sets").ToList();
            var categoryImages = Images.Where(x => NormalizeImageBucket(x.Directory) == "enhancement_categories").ToList();
            var borderImages = Images.Where(x => NormalizeImageBucket(x.Directory) == "enhancement_borders").Concat(baseImages).ToList();

            var emptySlotPath = Images.FirstOrDefault(x => x.FileName.Equals("none.png", StringComparison.OrdinalIgnoreCase)).Path;
            if (emptySlotPath != null) EmptySlot = new Bitmap(emptySlotPath);

            var newSlotPath = Images.FirstOrDefault(x => x.FileName.Equals("newslot.png", StringComparison.OrdinalIgnoreCase)).Path;
            if (newSlotPath != null) NewSlot = new Bitmap(newSlotPath);

            await LoadOriginImages(originImages);
            await LoadArchetypeImages(archetypeImages, baseImages);
            await LoadPowersetImages(powersetImages, baseImages);
            await LoadEnhancementImages(enhancementImages, baseImages);
            await LoadEnhancementSetImages(enhancementImages, baseImages);
            await LoadBorderImages(borderImages);
            await LoadSetTypeImages(setImages, baseImages);
            await LoadEnhTypeImages(categoryImages, setImages, baseImages);
            await LoadEnhancementClassImages(classImages, baseImages);

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: false);
            await Task.CompletedTask;
        }

        public static List<string?> LoadButtons()
        {
            return Images
                .Where(x => x.IsBase && x.FileName.Contains("pSlot"))
                .Select(x => x.Path)
                .ToList();
        }

        public static string? LoadNewSlot()
        {
            return Images.FirstOrDefault(x => x.FileName.Contains("New")).Path;
        }

        public static Task<List<string>> LoadArchetypes()
        {
            var baseImage = Images.FirstOrDefault(x => x is { IsBase: true, FileName: "Unknown.png" }).Path ?? string.Empty;
            var archTypePaths = new HashSet<string>();

            foreach (var c in DatabaseAPI.Database.Classes)
            {
                var path = Images.FirstOrDefault(i => c != null && i.Directory == "Archetypes" && i.FileName == $"{c.ClassName}.png").Path ?? baseImage;
                archTypePaths.Add(path);
            }

            return Task.FromResult(archTypePaths.ToList());
        }

        public static List<string> ArchetypeImages
        {
            get
            {
                var retList = new List<string>();
                var baseImages = Images.Where(x => x.IsBase).ToList();
                var archetypeImages = Images.Where(x => x.Directory == "Archetypes").ToList();
                var unknown = baseImages.First(i => i.FileName == "Unknown.png").Path;
                foreach (var c in DatabaseAPI.Database.Classes)
                {
                    var path = archetypeImages.FirstOrDefault(i => i.FileName == $"{c?.ClassName}.png").Path;
                    if (string.IsNullOrWhiteSpace(path))
                    {
                        path = unknown;
                    }

                    if (retList.Any(p => p == path)) continue;
                    if (path != null) retList.Add(path);
                }

                return retList;
            }
        }

        public static List<string> OriginImages
        {
            get
            {
                var retList = new List<string>();
                var baseImages = Images.Where(x => x.IsBase).ToList();
                var images = Images.Where(x => x.Directory == "Origins").ToList();
                var unknown = baseImages.First(i => i.FileName == "Unknown.png").Path;
                foreach (var o in DatabaseAPI.Database.Origins)
                {
                    var path = images.First(i => i.FileName == $"{o.Name}.png").Path;
                    if (string.IsNullOrWhiteSpace(path))
                    {
                        path = unknown;
                    }

                    if (retList.Any(p => p == path)) continue;
                    if (path != null) retList.Add(path);
                }

                return retList;
            }
        }

        public static Task<List<string>> LoadOrigins()
        {
            var baseImage = Images.FirstOrDefault(x => x is { IsBase: true, FileName: "Unknown.png" }).Path ?? string.Empty;
            var originPaths = new HashSet<string>();

            foreach (var origin in DatabaseAPI.Database.Origins)
            {
                var path = Images.FirstOrDefault(i => i.Directory == "Origins" && i.FileName == $"{origin.Name}.png").Path ?? baseImage;
                originPaths.Add(path);
            }

            return Task.FromResult(originPaths.ToList());
        }

        public static async Task LoadEnhancements()
        {
            var baseImages = Images.Where(x => x.IsBase).ToList();
            var enhancmentImages = Images.Where(x => NormalizeImageBucket(x.Directory) == "enhancements").ToList();
            await LoadEnhancementImages(enhancmentImages, baseImages);
            await Task.CompletedTask;
        }

        public static async Task<List<string?>> LoadPowerSets()
        {
            var cSource = new TaskCompletionSource<List<string?>>();
            var retList = new List<string?>();
            var baseImages = Images.Where(x => x.IsBase).ToList();
            var powersetImages = Images.Where(x => NormalizeImageBucket(x.Directory) == "powersets").OrderBy(i => i.IsBase ? 1 : 0).ToList();
            var unknown = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            if (retList.Any(p => p != unknown))
            {
                if (unknown != null) retList.Add(unknown);
            }
            retList.AddRange(DatabaseAPI.Database.Powersets.Select(ps =>
                ps == null || !TryFindImagePath(powersetImages, BuildPowersetImageCandidates(ps.ImageName), out var resolvedPath)
                    ? null
                    : resolvedPath).Where(path => !string.IsNullOrWhiteSpace(path)));

            cSource.TrySetResult(retList);

            return await cSource.Task;
        }

        public static async Task<List<string?>> LoadSets()
        {
            var cSource = new TaskCompletionSource<List<string?>>();
            var retList = new List<string?>();
            var baseImages = Images.Where(x => x.IsBase).ToList();
            var enhancementImages = Images.Where(x => NormalizeImageBucket(x.Directory) == "enhancements").OrderBy(i => i.IsBase ? 1 : 0).ToList();
            var unknown = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            foreach (var es in DatabaseAPI.Database.EnhancementSets)
            {
                //Debug.WriteLine(DatabaseAPI.Database.EnhancementSets[index].Image);
                var path = enhancementImages.FirstOrDefault(i => i.FileName.Equals(es.Image, StringComparison.OrdinalIgnoreCase)).Path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = unknown;
                }
                retList.Add(path);
            }

            for (var index = 0; index < retList.Count; index++)
            {
                DatabaseAPI.Database.EnhancementSets[index].ImageIdx = index;
            }

            cSource.TrySetResult(retList);
            return await cSource.Task;
        }

        private static async Task LoadArchetypeImages(IReadOnlyCollection<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknown = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path ?? string.Empty;
            int count = DatabaseAPI.Database.Classes.Length;

            var scaledSize = GetDpiScaledSize(IconSmall);

            Archetypes = new ExtendedBitmap(count * scaledSize, scaledSize);
            Archetypes.Graphics.CompositingMode = CompositingMode.SourceOver;
            Archetypes.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Archetypes.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Archetypes.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Archetypes.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Archetypes.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            for (var index = 0; index < count; index++)
            {
                var className = DatabaseAPI.Database.Classes[index].ClassName;
                var path = TryFindImagePath(images, BuildArchetypeImageCandidates(className), out var resolvedPath)
                    ? resolvedPath
                    : unknown;

                using var originalBitmap = new Bitmap(path);
                using var resized = ResizeTo(originalBitmap, IconSmall);

                int x = index * scaledSize;
                Archetypes.Graphics.DrawImage(resized, x, 0, scaledSize, scaledSize);
            }

            UnknownArchetype = new ExtendedBitmap(unknown);
            await Task.CompletedTask;
        }

        private static async Task LoadPowersetImages(IReadOnlyCollection<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknown = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path ?? string.Empty;
            int count = DatabaseAPI.Database.Powersets.Length;

            var scaledSize = GetDpiScaledSize(IconSmall);
            Powersets = new ExtendedBitmap(count * scaledSize, scaledSize);
            Powersets.Graphics.CompositingMode = CompositingMode.SourceOver;
            Powersets.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Powersets.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Powersets.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Powersets.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Powersets.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            for (var index = 0; index < count; index++)
            {
                var ps = DatabaseAPI.Database.Powersets[index];
                var path = TryFindImagePath(images, BuildPowersetImageCandidates(ps.ImageName), out var resolvedPath)
                    ? resolvedPath
                    : unknown;

                using var originalBitmap = new Bitmap(path);
                using var resized = ResizeTo(originalBitmap, IconSmall);

                int x = index * scaledSize;
                Powersets.Graphics.DrawImage(resized, x, 0, scaledSize, scaledSize);
            }

            UnknownPowerset = new ExtendedBitmap(unknown);
            await Task.CompletedTask;
        }

        private static async Task LoadOriginImages(IReadOnlyCollection<ImageInfo> images)
        {
            int count = DatabaseAPI.Database.Origins.Count;

            var scaledSize = GetDpiScaledSize(IconSmall);
            Origins = new ExtendedBitmap(count * scaledSize, scaledSize);
            Origins.Graphics.CompositingMode = CompositingMode.SourceOver;
            Origins.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Origins.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Origins.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Origins.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Origins.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            for (int index = 0; index < count; index++)
            {
                var origin = DatabaseAPI.Database.Origins[index];
                var path = TryFindImagePath(images, BuildOriginImageCandidates(origin.Name), out var resolvedPath)
                    ? resolvedPath
                    : string.Empty;

                if (string.IsNullOrWhiteSpace(path))
                    continue;

                using var originalBitmap = new Bitmap(path);
                using var resized = ResizeTo(originalBitmap, IconSmall);

                int x = index * scaledSize;
                Origins.Graphics.DrawImage(resized, x, 0, scaledSize, scaledSize);
            }

            await Task.CompletedTask;
        }

        private static async Task LoadEnhancementClassImages(IReadOnlyCollection<ImageInfo> images, IReadOnlyCollection<ImageInfo> baseImages)
        {
            var classImagePath = baseImages.FirstOrDefault(i => i.FileName == "Class.png").Path;
            var incImagePath = baseImages.FirstOrDefault(i => i.FileName == "Inc.png").Path;
            int count = DatabaseAPI.Database.EnhancementClasses.Length;
            var scaledSize = GetDpiScaledSize(IconLarge);

            Classes = new ExtendedBitmap(count * scaledSize, scaledSize);
            Classes.Graphics.CompositingMode = CompositingMode.SourceOver;
            Classes.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Classes.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Classes.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Classes.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Classes.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            for (int index = 0; index < count; index++)
            {
                string overlayPath = (index >= 27 ? incImagePath : classImagePath) ?? string.Empty;
                using var overlayBitmap = new Bitmap(overlayPath);
                using var resizedOverlay = ResizeTo(overlayBitmap, IconLarge);

                var classId = DatabaseAPI.Database.EnhancementClasses[index].ID;
                var iconPath = images.FirstOrDefault(i => i.FileName == $"{classId}.png").Path;
                if (string.IsNullOrWhiteSpace(iconPath))
                    continue;

                using var originalBitmap = new Bitmap(iconPath);
                using var resized = ResizeTo(originalBitmap, IconLarge);

                int x = index * scaledSize;
                Classes.Graphics.DrawImage(resizedOverlay, x, 0, scaledSize, scaledSize);
                Classes.Graphics.DrawImage(resized, x, 0, scaledSize, scaledSize);
            }

            await Task.CompletedTask;
        }

        private static async Task LoadEnhancementImages(IReadOnlyCollection<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknown = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            Enhancements = new Bitmap[DatabaseAPI.Database.Enhancements.Length];
            var preferredImages = images.OrderBy(i => i.IsBase ? 1 : 0).ToList();

            for (int index = 0; index < Enhancements.Length; index++)
            {
                var enh = DatabaseAPI.Database.Enhancements[index];

                if (!string.IsNullOrWhiteSpace(enh.Image))
                {
                    try
                    {
                        var path = preferredImages.FirstOrDefault(i => i.FileName.Equals(enh.Image, StringComparison.OrdinalIgnoreCase)).Path ?? unknown;

                        using var original = new Bitmap(path);
                        Enhancements[index] = ResizeTo(original, IconLarge);
                    }
                    catch
                    {
                        Enhancements[index] = new Bitmap(IconLarge, IconLarge, PixelFormat.Format32bppArgb);
                    }

                    enh.ImageIdx = index;
                }
                else
                {
                    Enhancements[index] = new Bitmap(IconLarge, IconLarge, PixelFormat.Format32bppArgb);
                    enh.ImageIdx = -1;
                }

                if (index % 5 == 0)
                {
                    Application.DoEvents();
                }
            }

            await Task.CompletedTask;
        }

        private static async Task LoadEnhancementSetImages(IReadOnlyCollection<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknown = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            int count = DatabaseAPI.Database.EnhancementSets.Count;
            var preferredImages = images.OrderBy(i => i.IsBase ? 1 : 0).ToList();

            var scaledSize = GetDpiScaledSize(IconLarge);
            Sets = new ExtendedBitmap(count * scaledSize, scaledSize);
            ConfigureGraphics(Sets.Graphics);

            for (int index = 0; index < count; index++)
            {
                var enhSet = DatabaseAPI.Database.EnhancementSets[index];
                var path = preferredImages.FirstOrDefault(i => i.FileName.Equals(enhSet.Image, StringComparison.OrdinalIgnoreCase)).Path ?? unknown;

                using var original = new Bitmap(path);
                using var resized = ResizeTo(original, IconLarge);

                int x = index * scaledSize;
                Sets.Graphics.DrawImage(resized, x, 0, scaledSize, scaledSize);
            }

            await Task.CompletedTask;
        }

        private static async Task LoadSetTypeImages(IReadOnlyCollection<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknown = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            var setTypes = DatabaseAPI.Database.SetTypes;
            int count = setTypes.Count;
            int scaledSize = GetDpiScaledSize(IconLarge);

            SetTypes = new ExtendedBitmap(count * scaledSize, scaledSize);
            ConfigureGraphics(SetTypes.Graphics);

            for (int index = 0; index < count; index++)
            {
                var shortName = setTypes[index].ShortName;
                var path = TryFindImagePath(images, AssetManager.BuildSetTypeImageCandidates(shortName), out var resolvedPath)
                    ? resolvedPath
                    : unknown;

                using var original = new Bitmap(path);
                using var resized = ResizeTo(original, IconLarge);

                int x = index * scaledSize;
                SetTypes.Graphics.DrawImage(resized, x, 0, scaledSize, scaledSize);
            }

            await Task.CompletedTask;
        }

        private static async Task LoadEnhTypeImages(IReadOnlyCollection<ImageInfo> categoryImages, IReadOnlyCollection<ImageInfo> setImages, IEnumerable<ImageInfo> baseImages)
        {
            var unknown = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;

            // --- Enhancement Types (Enums.eType) ---
            var typeNames = Enum.GetNames(typeof(Enums.eType));
            //typeNames[3] = "HamiO"; // fix name override
            int scaledSize = GetDpiScaledSize(IconLarge);

            EnhTypes = new ExtendedBitmap(typeNames.Length * scaledSize, scaledSize);
            ConfigureGraphics(EnhTypes.Graphics);

            for (int index = 0; index < typeNames.Length; index++)
            {
                var type = (Enums.eType)index;
                var path = TryFindImagePath(categoryImages, BuildEnhancementTypeImageCandidates(type), out var resolvedPath)
                    ? resolvedPath
                    : unknown;

                using var original = new Bitmap(path);
                using var resized = ResizeTo(original, IconLarge);

                int x = index * scaledSize;
                EnhTypes.Graphics.DrawImage(resized, x, 0, scaledSize, scaledSize);
            }

            // --- Enhancement Grades (Enums.eEnhGrade) ---
            var gradeNames = Enum.GetNames(typeof(Enums.eEnhGrade));
            EnhGrades = new ExtendedBitmap(gradeNames.Length * scaledSize, scaledSize);
            ConfigureGraphics(EnhGrades.Graphics);

            for (int index = 0; index < gradeNames.Length; index++)
            {
                var grade = (Enums.eEnhGrade)index;
                var path = TryFindImagePath(categoryImages, BuildEnhancementGradeImageCandidates(grade), out var resolvedPath)
                    ? resolvedPath
                    : unknown;

                using var original = new Bitmap(path);
                using var resized = ResizeTo(original, IconLarge);

                int x = index * scaledSize;
                EnhGrades.Graphics.DrawImage(resized, x, 0, scaledSize, scaledSize);
            }

            // --- Special Enhancements (e.g., "Magic", "Mutant", etc.) ---
            var specialEnhancements = DatabaseAPI.Database.SpecialEnhancements;
            var specNames = specialEnhancements.Select(x => x.Name.Replace(" Origin", string.Empty)).ToArray();

            EnhSpecials = new ExtendedBitmap(specNames.Length * scaledSize, scaledSize);
            ConfigureGraphics(EnhSpecials.Graphics);

            for (int index = 0; index < specNames.Length; index++)
            {
                var path = TryFindImagePath(setImages, [$"{specNames[index]}.png"], out var resolvedPath)
                    ? resolvedPath
                    : unknown;

                using var original = new Bitmap(path);
                using var resized = ResizeTo(original, IconLarge);

                int x = index * scaledSize;
                EnhSpecials.Graphics.DrawImage(resized, x, 0, scaledSize, scaledSize);
            }

            await Task.CompletedTask;
        }

        private static async Task LoadBorderImages(IReadOnlyCollection<ImageInfo> images)
        {
            var origins = DatabaseAPI.Database.Origins;
            int originCount = origins.Count;
            int gradeCount = DatabaseAPI.Database.Origins.FirstOrDefault()?.Grades.Length ?? 7;

            var scaledSize = GetDpiScaledSize(IconLarge);
            Borders = new ExtendedBitmap(originCount * scaledSize, gradeCount * scaledSize);
            ConfigureGraphics(Borders.Graphics);

            for (int originIndex = 0; originIndex < originCount; originIndex++)
            {
                int x = originIndex * scaledSize;

                for (int gradeIndex = 0; gradeIndex < origins[originIndex].Grades.Length; gradeIndex++)
                {
                    var path = TryFindImagePath(images, BuildBorderImageCandidates(origins[originIndex], gradeIndex), out var resolvedPath)
                        ? resolvedPath
                        : string.Empty;

                    if (string.IsNullOrWhiteSpace(path))
                        continue;

                    using var original = new Bitmap(path);
                    using var resized = ResizeTo(original, IconLarge);

                    int y = gradeIndex * scaledSize;
                    Borders.Graphics.DrawImage(resized, x, y, scaledSize, scaledSize);
                }
            }

            await Task.CompletedTask;
        }


        private static void ConfigureGraphics(Graphics g)
        {
            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }

        public static Image GetArchetypeImage(IPower power)
        {
            var imgString = "";
            var imgFile = "";
            var atString = power.GetPowerSet().ATClass;
            if (string.IsNullOrWhiteSpace(atString))
            {
                atString = power.Requires.ClassName[0];
            }

            if (string.IsNullOrWhiteSpace(atString))
            {
                imgFile = $"{ImagePath()}\\Unknown.png";
            }
            else
            {
                imgFile = $"{ImagePath("OriginAT")}\\{atString}.png";
                if (!File.Exists(imgFile))
                {
                    imgFile = $"{ImagePath()}\\Unknown.png";
                }
            }

            return Image.FromFile(imgFile);
        }

        public static Image GetArchetypeImage(Archetype atClass)
        {
            var imgFile = $"{ImagePath("OriginAT")}\\{atClass.ClassName}.png";
            if (!File.Exists(imgFile))
            {
                imgFile = $"{ImagePath()}\\Unknown.png";
            }

            return Image.FromFile(imgFile);
        }

        public static Image GetPowersetImage(IPower power)
        {
            var imgString = power.GetPowerSet().ImageName;
            var imgFile = $"{ImagePath("Powersets")}\\{imgString}";
            if (!File.Exists(imgFile))
            {
                imgFile = $"{ImagePath()}Unknown.png";
            }

            return Image.FromFile(imgFile);
        }

        public static Image GetPowersetImage(IPowerset powerset)
        {
            var imgString = powerset.ImageName;
            var imgFile = $"{ImagePath("Powersets")}\\{imgString}";
            if (!File.Exists(imgFile))
            {
                imgFile = $"{ImagePath()}\\Unknown.png";
            }

            return Image.FromFile(imgFile);
        }

        public static Origin.Grade ToGfxGrade(Enums.eType iType)
        {
            var grade = iType switch
            {
                Enums.eType.None => Origin.Grade.None,
                Enums.eType.Normal => Origin.Grade.TrainingO,
                Enums.eType.InventO => Origin.Grade.IO,
                Enums.eType.SpecialO => Origin.Grade.HO,
                Enums.eType.SetO => Origin.Grade.SetO,
                _ => Origin.Grade.None
            };
            return grade;
        }

        public static Origin.Grade ToGfxGrade(Enums.eType iType, Enums.eEnhGrade iGrade)
        {
            switch (iType)
            {
                case Enums.eType.None:
                    return Origin.Grade.None;
                case Enums.eType.Normal:
                    switch (iGrade)
                    {
                        case Enums.eEnhGrade.None:
                            return Origin.Grade.None;
                        case Enums.eEnhGrade.TrainingO:
                            return Origin.Grade.TrainingO;
                        case Enums.eEnhGrade.DualO:
                            return Origin.Grade.DualO;
                        case Enums.eEnhGrade.SingleO:
                            return Origin.Grade.SingleO;
                    }

                    break;
                case Enums.eType.InventO:
                    return Origin.Grade.IO;
                case Enums.eType.SpecialO:
                    return Origin.Grade.HO;
                case Enums.eType.SetO:
                    return Origin.Grade.SetO;
            }

            return Origin.Grade.None;
        }

        public static string? GetRecipeName()
        {
            return ImagePath("Overlay") + "\\Recipe.png";
        }

        public static string? GetRecipeTransparentName()
        {
            return ImagePath("Overlay") + "\\Recipe2.png";
        }

        public static string GetPowersetsPath()
        {
            return ImagePath() + "\\Powersets\\";
        }

        public static string GetEnhancementsPath()
        {
            return $"{ImagePath()}\\Enhancements\\";
        }

        public static string GetDbEnhancementsPath()
        {
            return Path.Combine(MidsContext.Config.DataPath, "Assets\\Enhancements");
        }

        public static string GetDbPowerSetsPath()
        {
            return Path.Combine(MidsContext.Config.DataPath, "Assets\\Powersets");
        }

        public static string GetOriginsPath()
        {
            return ImagePath() + "\\Origins\\";
        }

        public static void DrawFlippingEnhancement(ref Graphics iTarget, Rectangle iDest, float iSize, int iImageIndex, Origin.Grade iGrade)
        {
            var iDest1 = iDest;
            iDest1.Width = (int)(iDest1.Width * (double)iSize);
            iDest1.X += (iDest.Width - iDest1.Width) / 2;
            DrawEnhancementAt(ref iTarget, iDest1, iImageIndex, iGrade);
        }

        public static void DrawEnhancement(ref Graphics iTarget, int iImageIndex, Origin.Grade iGrade)
        {
            int size = GetDpiScaledSize(IconLarge);
            var srcRect = new RectangleF(0f, 0f, size, size);

            iTarget.PixelOffsetMode = PixelOffsetMode.HighQuality;
            iTarget.CompositingMode = CompositingMode.SourceOver;
            iTarget.CompositingQuality = CompositingQuality.HighQuality;
            iTarget.InterpolationMode = InterpolationMode.HighQualityBicubic;
            iTarget.SmoothingMode = SmoothingMode.HighQuality;
            iTarget.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            iTarget.PageUnit = GraphicsUnit.Pixel;

            iTarget.DrawImage(Borders.Bitmap, iTarget.ClipBounds, GetOverlayRectF(iGrade), GraphicsUnit.Pixel);
            iTarget.DrawImage(Enhancements[iImageIndex], iTarget.ClipBounds, srcRect, GraphicsUnit.Pixel);
        }

        public static void DrawEnhancementAt(ref Graphics iTarget, Rectangle iDest, int iImageIndex, Origin.Grade iGrade, ImageAttributes imageAttributes)
        {
            int scaledSize = GetDpiScaledSize(IconLarge);

            if (iDest.Width > scaledSize)
                iDest.Width = scaledSize;

            if (iDest.Height > scaledSize)
                iDest.Height = scaledSize;

            if (iImageIndex < 0 || iImageIndex >= Enhancements.Length)
                return;

            Rectangle overlayRect = GetOverlayRect(iGrade);
            iTarget.DrawImage(Borders.Bitmap, iDest, overlayRect.X, overlayRect.Y, scaledSize, scaledSize, GraphicsUnit.Pixel, imageAttributes);
            iTarget.DrawImage(Enhancements[iImageIndex], iDest, 0, 0, scaledSize, scaledSize, GraphicsUnit.Pixel, imageAttributes);
        }

        public static void DrawEnhancementAt(ref Graphics iTarget, Rectangle iDest, int iImageIndex, Origin.Grade iGrade)
        {
            int scaledSize = GetDpiScaledSize(IconLarge);

            if (iDest.Width > scaledSize)
                iDest.Width = scaledSize;

            if (iDest.Height > scaledSize)
                iDest.Height = scaledSize;

            if (iImageIndex < 0 || iImageIndex >= Enhancements.Length)
                return;

            iTarget.PixelOffsetMode = PixelOffsetMode.HighQuality;
            iTarget.CompositingMode = CompositingMode.SourceOver;
            iTarget.CompositingQuality = CompositingQuality.HighQuality;
            iTarget.InterpolationMode = InterpolationMode.HighQualityBicubic;
            iTarget.SmoothingMode = SmoothingMode.HighQuality;
            iTarget.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            iTarget.PageUnit = GraphicsUnit.Pixel;

            iTarget.DrawImage(Borders.Bitmap, iDest, GetOverlayRect(iGrade), GraphicsUnit.Pixel);
            iTarget.DrawImage(Enhancements[iImageIndex], iDest, new Rectangle(0, 0, scaledSize, scaledSize), GraphicsUnit.Pixel);
        }

        public static void DrawEnhancementSet(ref Graphics iTarget, int iImageIndex)
        {
            iTarget.PixelOffsetMode = PixelOffsetMode.HighQuality;
            iTarget.CompositingMode = CompositingMode.SourceOver;
            iTarget.CompositingQuality = CompositingQuality.HighQuality;
            iTarget.InterpolationMode = InterpolationMode.HighQualityBicubic;
            iTarget.SmoothingMode = SmoothingMode.HighQuality;
            iTarget.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            iTarget.PageUnit = GraphicsUnit.Pixel;
            if (Borders.Bitmap == null) return;
            iTarget.DrawImage(Borders.Bitmap, iTarget.ClipBounds, GetOverlayRectF(Origin.Grade.SetO), GraphicsUnit.Pixel);
            if (Sets.Bitmap != null)
            {
                iTarget.DrawImage(Sets.Bitmap, iTarget.ClipBounds, GetImageRectF(iImageIndex), GraphicsUnit.Pixel);
            }
        }

        public static Rectangle GetOverlayRect(Origin.Grade iGrade)
        {
            if (iGrade == Origin.Grade.None)
            {
                iGrade = Origin.Grade.HO;
            }

            int size = GetDpiScaledSize(IconLarge);
            return new Rectangle(OriginIndex * size, (int)iGrade * size, size, size);
        }

        private static RectangleF GetOverlayRectF(Origin.Grade iGrade)
        {
            var overlayRect = GetOverlayRect(iGrade);
            return new RectangleF(overlayRect.X, overlayRect.Y, overlayRect.Width, overlayRect.Height);
        }

        public static Rectangle GetImageRect(int index)
        {
            int size = GetDpiScaledSize(IconLarge);
            return new Rectangle(index * size, 0, size, size);
        }

        private static RectangleF GetImageRectF(int index)
        {
            var imageRect = GetImageRect(index);
            return new RectangleF(imageRect.X, imageRect.Y, imageRect.Width, imageRect.Height);
        }

        public static string? UnknownImgPath()
        {
            return Images
                .Where(x => x.IsBase)
                .FirstOrDefault(i => i.FileName == "Unknown.png").Path;
        }
    }
}
