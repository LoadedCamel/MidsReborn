using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Mids_Reborn.Core
{
    /// <summary>
    /// A static manager for loading and retrieving game image assets.
    /// It uses ExtendedBitmap to ensure proper resource management.
    /// IMPORTANT: Call the Shutdown() method when the application is closing to prevent memory leaks.
    /// </summary>
    public static class AssetManager
    {
        private const int IconLarge = 128;
        private const int IconSmall = 16;
        private const string ImageFilter = "*.png";

        private static List<ImageInfo> Images { get; set; } = [];
        private static bool Initialized { get; set; }

        #region Image Collections (Using ExtendedBitmap)
        public static List<ExtendedBitmap> Buttons { get; private set; } = [];
        public static List<ExtendedBitmap> Enhancements { get; private set; } = [];
        public static Dictionary<Point, ExtendedBitmap> Borders { get; private set; } = [];
        public static Dictionary<int, ExtendedBitmap> Sets { get; private set; } = [];
        public static Dictionary<int, ExtendedBitmap> Classes { get; private set; } = [];
        public static Dictionary<int, ExtendedBitmap> SetTypes { get; private set; } = [];
        public static Dictionary<int, ExtendedBitmap> EnhTypes { get; private set; } = [];
        public static Dictionary<int, ExtendedBitmap> EnhGrades { get; private set; } = [];
        public static Dictionary<int, ExtendedBitmap> EnhSpecials { get; private set; } = [];
        public static Dictionary<int, ExtendedBitmap> Archetypes { get; private set; } = [];
        public static Dictionary<int, ExtendedBitmap> Origins { get; private set; } = [];
        public static Dictionary<int, ExtendedBitmap> Powersets { get; private set; } = [];
        public static ExtendedBitmap UnknownIcon { get; private set; }
        public static ExtendedBitmap EmptySlot { get; private set; }
        public static ExtendedBitmap NewSlot { get; private set; }
        public static ExtendedBitmap RecipeIcon { get; private set; }
        public static ExtendedBitmap RecipeIconTransparent { get; private set; }
        #endregion

        public static int OriginIndex;

        /// <summary>
        /// Gathers all image paths but does not load them into memory.
        /// </summary>
        public static void Initialize(string path)
        {
            var baseImages = GetBaseImages();
            var extendedImages = GetExtendedImages(path);
            Images = baseImages.Concat(extendedImages).ToList();
            Initialized = true;
        }

        /// <summary>
        /// Loads all images into memory. Should be called after Initialize().
        /// To run asynchronously and not block the UI, call this method with Task.Run(() => AssetManager.LoadImages());
        /// </summary>
        public static void LoadImages()
        {
            if (!Initialized)
            {
                // Throw an exception to signal a critical error to the calling code.
                throw new InvalidOperationException("Attempted to access image assets before initialization.");
            }

            // Group images by directory for faster lookups
            var imageGroups = Images.ToLookup(img => img.Directory);
            var baseImages = Images.Where(x => x.IsBase).ToList();

            // Load common base images
            EmptySlot = new ExtendedBitmap(Images.FirstOrDefault(x => x.FileName.Equals("none.png", StringComparison.OrdinalIgnoreCase)).Path);
            NewSlot = new ExtendedBitmap(Images.FirstOrDefault(x => x.FileName.Equals("newslot.png", StringComparison.OrdinalIgnoreCase)).Path);

            // Load recipe images
            RecipeIcon = new ExtendedBitmap(Images.FirstOrDefault(x => x.FileName.Equals("Recipe.png", StringComparison.OrdinalIgnoreCase)).Path);
            RecipeIconTransparent = new ExtendedBitmap(Images.FirstOrDefault(x => x.FileName.Equals("Recipe2.png", StringComparison.OrdinalIgnoreCase)).Path);

            var unknownPath = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path ?? string.Empty;
            UnknownIcon = new ExtendedBitmap(unknownPath);

            // Load button images
            var buttonPaths = Images.Where(x => x.IsBase && x.FileName.Contains("pSlot")).Select(x => x.Path).ToList();
            foreach (var path in buttonPaths)
            {
                Buttons.Add(new ExtendedBitmap(path));
            }

            // Load all other image categories
            LoadOriginImages(imageGroups["Origins"]);
            LoadArchetypeImages(imageGroups["Archetypes"], baseImages);
            LoadPowersetImages(imageGroups["Powersets"], baseImages);
            LoadEnhancementImages(imageGroups["Enhancements"], baseImages);
            LoadEnhancementSetImages(imageGroups["Enhancements"], baseImages);
            LoadBorderImages(baseImages);
            LoadSetTypeImages(imageGroups["Sets"], baseImages);
            LoadEnhTypeImages(imageGroups["Sets"], baseImages);
            LoadEnhancementClassImages(imageGroups["Classes"], baseImages);
        }

        /// <summary>
        /// Disposes all loaded image assets to free up memory. Call this on application exit.
        /// </summary>
        public static void Shutdown()
        {
            var allImages = new List<ExtendedBitmap>();
            allImages.AddRange(Buttons);
            allImages.AddRange(Enhancements);
            allImages.AddRange(Borders.Values);
            allImages.AddRange(Sets.Values);
            allImages.AddRange(Classes.Values);
            allImages.AddRange(SetTypes.Values);
            allImages.AddRange(EnhTypes.Values);
            allImages.AddRange(EnhGrades.Values);
            allImages.AddRange(EnhSpecials.Values);
            allImages.AddRange(Archetypes.Values);
            allImages.AddRange(Origins.Values);
            allImages.AddRange(Powersets.Values);
            allImages.Add(UnknownIcon);
            allImages.Add(RecipeIcon);
            allImages.Add(RecipeIconTransparent);
            allImages.Add(EmptySlot);
            allImages.Add(NewSlot);

            foreach (var image in allImages.Where(img => img != null))
            {
                image.Dispose();
            }

            // Clear collections to release references
            Buttons.Clear();
            Enhancements.Clear();
            Borders.Clear();
            Sets.Clear();
            Classes.Clear();
            SetTypes.Clear();
            EnhTypes.Clear();
            EnhGrades.Clear();
            EnhSpecials.Clear();
            Archetypes.Clear();
            Origins.Clear();
            Powersets.Clear();
        }

        #region Loading Methods
        private static void LoadArchetypeImages(IEnumerable<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknownPath = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            Archetypes.Clear();
            for (var index = 0; index < DatabaseAPI.Database.Classes.Length; index++)
            {
                var className = DatabaseAPI.Database.Classes[index].ClassName;
                var path = images.FirstOrDefault(i => i.FileName == $"{className}.png").Path ?? unknownPath;
                using var original = new ExtendedBitmap(path);
                Archetypes[index] = ResizeTo(original, IconSmall);
            }
        }

        private static void LoadPowersetImages(IEnumerable<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknownPath = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            Powersets.Clear();
            for (var index = 0; index < DatabaseAPI.Database.Powersets.Length; index++)
            {
                var ps = DatabaseAPI.Database.Powersets[index];
                var path = images.FirstOrDefault(i => i.FileName == ps.ImageName).Path ?? unknownPath;
                using var original = new ExtendedBitmap(path);
                Powersets[index] = ResizeTo(original, IconSmall);
            }
        }

        private static void LoadOriginImages(IEnumerable<ImageInfo> images)
        {
            Origins.Clear();
            for (int index = 0; index < DatabaseAPI.Database.Origins.Count; index++)
            {
                var origin = DatabaseAPI.Database.Origins[index];
                var path = images.FirstOrDefault(i => i.FileName.Contains(origin.Name)).Path;
                if (string.IsNullOrWhiteSpace(path)) continue;
                using var original = new ExtendedBitmap(path);
                Origins[index] = ResizeTo(original, IconSmall);
            }
        }

        private static void LoadEnhancementClassImages(IEnumerable<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var classImagePath = baseImages.FirstOrDefault(i => i.FileName == "Class.png").Path;
            var incImagePath = baseImages.FirstOrDefault(i => i.FileName == "Inc.png").Path;
            Classes.Clear();

            for (int index = 0; index < DatabaseAPI.Database.EnhancementClasses.Length; index++)
            {
                var classId = DatabaseAPI.Database.EnhancementClasses[index].ID;
                var iconPath = images.FirstOrDefault(i => i.FileName == $"{classId}.png").Path;
                if (string.IsNullOrWhiteSpace(iconPath)) continue;

                string overlayPath = (index >= 27 ? incImagePath : classImagePath);
                if (string.IsNullOrWhiteSpace(overlayPath)) continue;

                var finalImage = new ExtendedBitmap(IconLarge, IconLarge);
                using (var g = finalImage.Graphics)
                {
                    if (g == null)
                    {
                        finalImage.Dispose();
                        continue;
                    }

                    ConfigureGraphics(g);
                    using var overlayBitmap = new ExtendedBitmap(overlayPath);
                    using var resizedOverlay = ResizeTo(overlayBitmap, IconLarge);
                    if (resizedOverlay.Bitmap != null) g.DrawImage(resizedOverlay.Bitmap, 0, 0);

                    using var originalBitmap = new ExtendedBitmap(iconPath);
                    using var resizedIcon = ResizeTo(originalBitmap, IconLarge);
                    if (resizedIcon.Bitmap != null) g.DrawImage(resizedIcon.Bitmap, 0, 0);
                }
                Classes[index] = finalImage;
            }
        }

        private static void LoadEnhancementImages(IEnumerable<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknownPath = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            Enhancements.Clear();

            for (int index = 0; index < DatabaseAPI.Database.Enhancements.Length; index++)
            {
                var enh = DatabaseAPI.Database.Enhancements[index];
                if (!string.IsNullOrWhiteSpace(enh.Image))
                {
                    var path = images.FirstOrDefault(i => i.FileName == enh.Image).Path ?? unknownPath;
                    using var original = new ExtendedBitmap(path);
                    Enhancements.Add(ResizeTo(original, IconLarge));
                    enh.ImageIdx = Enhancements.Count - 1;
                }
                else
                {
                    Enhancements.Add(new ExtendedBitmap(IconLarge, IconLarge));
                    enh.ImageIdx = -1;
                }
            }
        }

        private static void LoadEnhancementSetImages(IEnumerable<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknownPath = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            Sets.Clear();
            for (int index = 0; index < DatabaseAPI.Database.EnhancementSets.Count; index++)
            {
                var enhSet = DatabaseAPI.Database.EnhancementSets[index];
                var path = images.FirstOrDefault(i => i.FileName == enhSet.Image).Path ?? unknownPath;
                using var original = new ExtendedBitmap(path);
                Sets[index] = ResizeTo(original, IconLarge);
            }
        }

        private static void LoadSetTypeImages(IEnumerable<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknownPath = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            SetTypes.Clear();
            var setTypes = DatabaseAPI.Database.SetTypes;
            for (int index = 0; index < setTypes.Count; index++)
            {
                var shortName = setTypes[index].ShortName;
                var path = images.FirstOrDefault(i => i.FileName == $"{shortName}.png").Path ?? unknownPath;
                using var original = new ExtendedBitmap(path);
                SetTypes[index] = ResizeTo(original, IconLarge);
            }
        }

        private static void LoadEnhTypeImages(IEnumerable<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknownPath = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            EnhTypes.Clear();
            EnhGrades.Clear();
            EnhSpecials.Clear();

            var typeNames = Enum.GetNames(typeof(Enums.eType));
            for (int index = 0; index < typeNames.Length; index++)
            {
                var path = images.FirstOrDefault(i => i.FileName == $"{typeNames[index]}.png").Path ?? unknownPath;
                using var original = new ExtendedBitmap(path);
                EnhTypes[index] = ResizeTo(original, IconLarge);
            }

            var gradeNames = Enum.GetNames(typeof(Enums.eEnhGrade));
            for (int index = 0; index < gradeNames.Length; index++)
            {
                var path = images.FirstOrDefault(i => i.FileName == $"{gradeNames[index]}.png").Path ?? unknownPath;
                using var original = new ExtendedBitmap(path);
                EnhGrades[index] = ResizeTo(original, IconLarge);
            }

            var specNames = DatabaseAPI.Database.SpecialEnhancements.Select(x => x.Name.Replace(" Origin", string.Empty)).ToArray();
            for (int index = 0; index < specNames.Length; index++)
            {
                var path = images.FirstOrDefault(i => i.FileName == $"{specNames[index]}.png").Path ?? unknownPath;
                using var original = new ExtendedBitmap(path);
                EnhSpecials[index] = ResizeTo(original, IconLarge);
            }
        }

        private static void LoadBorderImages(IEnumerable<ImageInfo> images)
        {
            Borders.Clear();
            var origins = DatabaseAPI.Database.Origins;
            for (int originIndex = 0; originIndex < origins.Count; originIndex++)
            {
                for (int gradeIndex = 0; gradeIndex <= 5; gradeIndex++)
                {
                    string fileName = origins[originIndex].Grades[gradeIndex];
                    var path = images.FirstOrDefault(i => i.FileName == $"{fileName}.png").Path;
                    if (string.IsNullOrWhiteSpace(path)) continue;
                    using var original = new ExtendedBitmap(path);
                    Borders[new Point(originIndex, gradeIndex)] = ResizeTo(original, IconLarge);
                }
            }
        }
        #endregion

        #region Drawing Methods
        public static void DrawFlippingEnhancement(Graphics iTarget, Rectangle iDest, float iSize, int iImageIndex, Origin.Grade iGrade)
        {
            var iDest1 = iDest;
            iDest1.Width = (int)(iDest1.Width * iSize);
            iDest1.X += (iDest.Width - iDest1.Width) / 2;
            DrawEnhancementAt(iTarget, iDest1, iImageIndex, iGrade);
        }

        public static void DrawEnhancementAt(Graphics iTarget, Rectangle iDest, int iImageIndex, Origin.Grade iGrade)
        {
            if (iImageIndex < 0 || iImageIndex >= Enhancements.Count || Enhancements[iImageIndex]?.Bitmap is null) return;

            var borderKey = new Point(OriginIndex, (int)iGrade);
            if (!Borders.TryGetValue(borderKey, out var borderImage) || borderImage?.Bitmap is null) return;

            ConfigureGraphics(iTarget);
            iTarget.DrawImage(borderImage.Bitmap, iDest);
            iTarget.DrawImage(Enhancements[iImageIndex].Bitmap, iDest);
        }

        public static void DrawEnhancementAt(Graphics iTarget, Rectangle iDest, int iImageIndex, Origin.Grade iGrade, ImageAttributes imageAttributes)
        {
            if (iImageIndex < 0 || iImageIndex >= Enhancements.Count || Enhancements[iImageIndex]?.Bitmap is null) return;

            var borderKey = new Point(OriginIndex, (int)iGrade);
            if (!Borders.TryGetValue(borderKey, out var borderImage) || borderImage?.Bitmap is null) return;

            ConfigureGraphics(iTarget);
            var srcRect = new Rectangle(0, 0, borderImage.Size.Width, borderImage.Size.Height);
            iTarget.DrawImage(borderImage.Bitmap, iDest, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, imageAttributes);
            iTarget.DrawImage(Enhancements[iImageIndex].Bitmap, iDest, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, imageAttributes);
        }

        public static void DrawEnhancementSet(Graphics iTarget, int iImageIndex)
        {
            DrawEnhancementSet(iTarget, Rectangle.Truncate(iTarget.ClipBounds), iImageIndex);
        }

        public static void DrawEnhancementSet(Graphics iTarget, Rectangle iDest, int iImageIndex)
        {
            var borderKey = new Point(OriginIndex, (int)Origin.Grade.SetO);
            if (!Borders.TryGetValue(borderKey, out var borderImage) || borderImage?.Bitmap is null) return;
            if (!Sets.TryGetValue(iImageIndex, out var setImage) || setImage?.Bitmap is null) return;

            ConfigureGraphics(iTarget);
            iTarget.DrawImage(borderImage.Bitmap, iDest);
            iTarget.DrawImage(setImage.Bitmap, iDest);
        }
        #endregion

        #region Retrieval Helpers

        public static ExtendedBitmap GetPowersetImage(IPower? power)
        {
            var powerSet = power?.GetPowerSet();
            if (powerSet is null) return UnknownIcon;

            var index = DatabaseAPI.Database.Powersets.TryFindIndex(ps => ps != null && ps.ImageName.Equals(powerSet.ImageName));
            if (index.Equals(-1)) return UnknownIcon;

            Powersets.TryGetValue(index, out var extendedBitmap);
            return extendedBitmap ?? UnknownIcon;
        }

        public static ExtendedBitmap GetPowersetImage(IPowerset? powerset)
        {
            var index = DatabaseAPI.Database.Powersets.TryFindIndex(ps => ps != null && powerset != null && ps.ImageName.Equals(powerset.ImageName));
            if (index.Equals(-1)) return UnknownIcon;
            Powersets.TryGetValue(index, out var extendedBitmap);
            return extendedBitmap ?? UnknownIcon;
        }

        public static string GetEnhancementsPath()
        {
            return Path.Combine(AppDataPaths.BaseAssetsPath, "Enhancements");
        }

        public static string GetDbEnhancementsPath()
        {
            return Path.Combine(MidsContext.Config.DataPath, "Images", "Enhancements");
        }

        public static string GetPowersetsPath()
        {
            return Path.Combine(AppDataPaths.BaseAssetsPath, "Powersets");
        }

        public static string GetDbPowerSetsPath()
        {
            return Path.Combine(MidsContext.Config.DataPath, "Images", "Powersets");
        }

        #endregion

        #region Logic Helpers

        public static void SetOrigin(string? iOrigin)
        {
            if (iOrigin != null) OriginIndex = DatabaseAPI.GetOriginIDByName(iOrigin);
        }

        public static Origin.Grade ToGfxGrade(Enums.eType iType)
        {
            return iType switch
            {
                Enums.eType.None => Origin.Grade.None,
                Enums.eType.Normal => Origin.Grade.TrainingO,
                Enums.eType.InventO => Origin.Grade.IO,
                Enums.eType.SpecialO => Origin.Grade.HO,
                Enums.eType.SetO => Origin.Grade.SetO,
                _ => Origin.Grade.None
            };
        }

        public static Origin.Grade ToGfxGrade(Enums.eType iType, Enums.eEnhGrade iGrade)
        {
            switch (iType)
            {
                case Enums.eType.Normal:
                    return iGrade switch
                    {
                        Enums.eEnhGrade.TrainingO => Origin.Grade.TrainingO,
                        Enums.eEnhGrade.DualO => Origin.Grade.DualO,
                        Enums.eEnhGrade.SingleO => Origin.Grade.SingleO,
                        _ => Origin.Grade.None
                    };
                case Enums.eType.InventO: return Origin.Grade.IO;
                case Enums.eType.SpecialO: return Origin.Grade.HO;
                case Enums.eType.SetO: return Origin.Grade.SetO;
                default: return Origin.Grade.None;
            }
        }
        #endregion

        #region File and Image Helpers
        private struct ImageInfo
        {
            public string FileName { get; set; }
            public string Directory { get; set; }
            public string Path { get; set; }
            public bool IsBase { get; set; }
        }

        private static IEnumerable<ImageInfo> GetBaseImages()
        {
            var retList = new List<ImageInfo>();
            if (!Directory.Exists(AppDataPaths.BaseAssetsPath)) return retList;

            var files = Directory.GetFiles(AppDataPaths.BaseAssetsPath, ImageFilter, SearchOption.AllDirectories);
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

        private static IEnumerable<ImageInfo> GetExtendedImages(string path)
        {
            var retList = new List<ImageInfo>();
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path)) return retList;

            var files = Directory.GetFiles(path, ImageFilter, SearchOption.AllDirectories);
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

        private static ExtendedBitmap ResizeTo(ExtendedBitmap original, int targetSizeLogical)
        {
            float scale = 96f; // Default DPI scale
            try
            {
                using var gdi = Graphics.FromHwnd(IntPtr.Zero);
                scale = gdi.DpiX;
            }
            catch { /* Fails in some non-interactive environments, use default */ }

            int targetSize = (int)(targetSizeLogical * (scale / 96f));

            if (original.Size.Width == targetSize && original.Size.Height == targetSize)
            {
                return new ExtendedBitmap(original); // Return a clone
            }

            var result = new ExtendedBitmap(targetSize, targetSize);
            using (var g = result.Graphics)
            {
                if (g != null && original.Bitmap != null)
                {
                    ConfigureGraphics(g);
                    g.DrawImage(original.Bitmap, 0, 0, targetSize, targetSize);
                }
            }
            return result;
        }

        private static void ConfigureGraphics(Graphics g)
        {
            g.CompositingMode = CompositingMode.SourceOver;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        }
        #endregion
    }
}