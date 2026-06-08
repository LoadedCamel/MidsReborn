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
        private enum BorderState
        {
            Training = 0,
            DualOrigin = 1,
            SingleOrigin = 2,
            Special = 3,
            Invention = 4,
            SetCrafted = 5,
            SetAttuned = 6,
            SetSuperiorAttuned = 7
        }

        private const int IconLarge = 64;
        private const int IconSmall = 32;
        private const string ImageFilter = "*.png";
        private const string BucketArchetypes = "archetypes";
        private const string BucketClasses = "classes";
        private const string BucketEnhancements = "enhancements";
        private const string BucketOrigins = "origins";
        private const string BucketOverlay = "overlay";
        private const string BucketPowersets = "powersets";
        private const string BucketSets = "sets";
        private const string BucketTypesRoot = "types";
        private const string BucketTypeGrades = "types/grades";
        private const string BucketTypeSets = "types/sets";

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
        private static Dictionary<string, ExtendedBitmap> NamedPowerImages { get; } = new(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, ExtendedBitmap> NamedEnhancementImages { get; } = new(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, ExtendedBitmap> NamedOverlayImages { get; } = new(StringComparer.OrdinalIgnoreCase);
        private static Dictionary<string, ExtendedBitmap> NamedAlignmentImages { get; } = new(StringComparer.OrdinalIgnoreCase);
        private static List<ExtendedBitmap> RetiredPowersets { get; } = [];
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

        public static bool TryResolveImageFileName(IEnumerable<string> candidates, out string fileName)
        {
            fileName = string.Empty;
            if (!Initialized)
            {
                return false;
            }

            if (!TryFindImagePath(Images, candidates, out var path))
            {
                return false;
            }

            fileName = Path.GetFileName(path);
            return !string.IsNullOrWhiteSpace(fileName);
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
            var imageGroups = BuildImageGroups();
            var baseImages = Images.Where(x => x.IsBase).ToList();

            // Load common base images
            EmptySlot = new ExtendedBitmap(
                TryFindImagePath(imageGroups[BucketTypesRoot], ["None.png"], out var emptySlotPath)
                    ? emptySlotPath
                    : string.Empty);
            NewSlot = new ExtendedBitmap(
                TryFindImagePath(Images, ["Newslot.png"], out var newSlotPath)
                    ? newSlotPath
                    : string.Empty);

            // Load recipe images
            RecipeIcon = new ExtendedBitmap(
                TryFindImagePath(Images, ["Recipe.png"], out var recipePath)
                    ? recipePath
                    : string.Empty);
            RecipeIconTransparent = new ExtendedBitmap(
                TryFindImagePath(Images, ["Recipe2.png"], out var recipeTransparentPath)
                    ? recipeTransparentPath
                    : string.Empty);

            var unknownPath = TryFindImagePath(Images, ["Unknown.png"], out var resolvedUnknownPath)
                ? resolvedUnknownPath
                : string.Empty;
            UnknownIcon = new ExtendedBitmap(unknownPath);

            // Load button images
            var buttonPaths = Images.Where(x => x.IsBase && x.FileName.Contains("pSlot")).Select(x => x.Path).ToList();
            foreach (var path in buttonPaths)
            {
                Buttons.Add(new ExtendedBitmap(path));
            }

            // Load all other image categories
            LoadOriginImages(imageGroups[BucketOrigins]);
            LoadArchetypeImages(imageGroups[BucketArchetypes], baseImages);
            LoadPowersetImages(imageGroups[BucketPowersets], baseImages);
            LoadEnhancementImages(imageGroups[BucketEnhancements], baseImages);
            LoadEnhancementSetImages(imageGroups[BucketSets], imageGroups[BucketEnhancements], baseImages);
            LoadBorderImages(imageGroups[BucketOverlay]);
            LoadSetTypeImages(imageGroups[BucketTypeSets], baseImages);
            LoadEnhTypeImages(imageGroups[BucketTypeGrades], imageGroups[BucketTypesRoot], baseImages);
            LoadSpecialRailImages(imageGroups[BucketTypeSets], baseImages);
            LoadEnhancementClassImages(imageGroups[BucketClasses], imageGroups[BucketOverlay]);
        }

        public static void ReloadImages()
        {
            if (!Initialized)
            {
                return;
            }

            Shutdown();
            LoadImages();
        }

        public static void RefreshPowersetImages()
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Attempted to access image assets before initialization.");
            }

            RetiredPowersets.AddRange(Powersets.Values.Where(img => img != null));
            Powersets.Clear();

            var imageGroups = BuildImageGroups();
            var baseImages = Images.Where(x => x.IsBase).ToList();
            LoadPowersetImages(imageGroups[BucketPowersets], baseImages);
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
            allImages.AddRange(NamedPowerImages.Values);
            allImages.AddRange(NamedEnhancementImages.Values);
            allImages.AddRange(NamedOverlayImages.Values);
            allImages.AddRange(NamedAlignmentImages.Values);
            allImages.AddRange(RetiredPowersets);
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
            NamedPowerImages.Clear();
            NamedEnhancementImages.Clear();
            NamedOverlayImages.Clear();
            NamedAlignmentImages.Clear();
            RetiredPowersets.Clear();
        }

        #region Loading Methods
        private static void LoadArchetypeImages(IEnumerable<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknownPath = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            Archetypes.Clear();
            for (var index = 0; index < DatabaseAPI.Database.Classes.Length; index++)
            {
                var className = DatabaseAPI.Database.Classes[index].ClassName;
                var path = TryFindImagePath(images, BuildArchetypeImageCandidates(className), out var resolvedPath)
                    ? resolvedPath
                    : unknownPath;
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
                var path = TryFindImagePath(images, BuildPowersetImageCandidates(ps.ImageName), out var resolvedPath)
                    ? resolvedPath
                    : unknownPath;
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
                var path = TryFindImagePath(images, BuildOriginImageCandidates(origin.Name), out var resolvedPath)
                    ? resolvedPath
                    : string.Empty;
                if (string.IsNullOrWhiteSpace(path)) continue;
                using var original = new ExtendedBitmap(path);
                Origins[index] = ResizeTo(original, IconSmall);
            }
        }

        private static void LoadEnhancementClassImages(IEnumerable<ImageInfo> images, IEnumerable<ImageInfo> overlayImages)
        {
            var classImagePath = TryFindImagePath(overlayImages, ["uber.png"], out var resolvedClassOverlayPath)
                ? resolvedClassOverlayPath
                : string.Empty;
            var incImagePath = TryFindImagePath(overlayImages, ["Inc.png"], out var resolvedIncOverlayPath)
                ? resolvedIncOverlayPath
                : string.Empty;
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
            var preferredImages = images.OrderBy(i => i.IsBase ? 1 : 0).ToList();

            for (int index = 0; index < DatabaseAPI.Database.Enhancements.Length; index++)
            {
                var enh = DatabaseAPI.Database.Enhancements[index];
                if (!string.IsNullOrWhiteSpace(enh.Image))
                {
                    var path = preferredImages.FirstOrDefault(i => i.FileName.Equals(enh.Image, StringComparison.OrdinalIgnoreCase)).Path ?? unknownPath;
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

        private static void LoadEnhancementSetImages(IEnumerable<ImageInfo> setImages, IEnumerable<ImageInfo> enhancementImages, IEnumerable<ImageInfo> baseImages)
        {
            var unknownPath = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            Sets.Clear();
            var preferredImages = setImages
                .Concat(enhancementImages)
                .OrderBy(i => i.IsBase ? 1 : 0)
                .ToList();
            for (int index = 0; index < DatabaseAPI.Database.EnhancementSets.Count; index++)
            {
                var enhSet = DatabaseAPI.Database.EnhancementSets[index];
                var path = preferredImages.FirstOrDefault(i => i.FileName.Equals(enhSet.Image, StringComparison.OrdinalIgnoreCase)).Path ?? unknownPath;
                using var original = new ExtendedBitmap(path);
                Sets[index] = ResizeTo(original, IconLarge);
                enhSet.ImageIdx = index;
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
                var path = TryFindImagePath(images, BuildSetTypeImageCandidates(shortName), out var resolvedPath)
                    ? resolvedPath
                    : unknownPath;
                using var original = new ExtendedBitmap(path);
                SetTypes[index] = ResizeTo(original, IconLarge);
            }
        }

        private static void LoadEnhTypeImages(IEnumerable<ImageInfo> gradeImages, IEnumerable<ImageInfo> typeRootImages, IEnumerable<ImageInfo> baseImages)
        {
            var unknownPath = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            EnhTypes.Clear();
            EnhGrades.Clear();

            var typeNames = Enum.GetNames(typeof(Enums.eType));
            for (int index = 0; index < typeNames.Length; index++)
            {
                var type = (Enums.eType)index;
                var candidateImages = type == Enums.eType.None ? typeRootImages : gradeImages;
                var path = TryFindImagePath(candidateImages, BuildEnhancementTypeImageCandidates(type), out var resolvedPath)
                    ? resolvedPath
                    : unknownPath;
                using var original = new ExtendedBitmap(path);
                EnhTypes[index] = ResizeTo(original, IconLarge);
            }

            var gradeNames = Enum.GetNames(typeof(Enums.eEnhGrade));
            for (int index = 0; index < gradeNames.Length; index++)
            {
                var grade = (Enums.eEnhGrade)index;
                var candidateImages = grade == Enums.eEnhGrade.None ? typeRootImages : gradeImages;
                var path = TryFindImagePath(candidateImages, BuildEnhancementGradeImageCandidates(grade), out var resolvedPath)
                    ? resolvedPath
                    : unknownPath;
                using var original = new ExtendedBitmap(path);
                EnhGrades[index] = ResizeTo(original, IconLarge);
            }
        }

        private static void LoadSpecialRailImages(IEnumerable<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknownPath = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            EnhSpecials.Clear();

            var specialEnhancements = DatabaseAPI.Database.SpecialEnhancements;
            for (int index = 0; index < specialEnhancements.Count; index++)
            {
                var specialEnhancement = specialEnhancements[index];
                var path = TryFindImagePath(images, BuildSpecialRailImageCandidates(specialEnhancement), out var resolvedPath)
                    ? resolvedPath
                    : unknownPath;
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
                foreach (BorderState borderState in Enum.GetValues(typeof(BorderState)))
                {
                    var path = TryFindImagePath(images, BuildBorderImageCandidates(origins[originIndex], borderState), out var resolvedPath)
                        ? resolvedPath
                        : string.Empty;
                    if (string.IsNullOrWhiteSpace(path)) continue;
                    using var original = new ExtendedBitmap(path);
                    Borders[new Point(originIndex, (int)borderState)] = ResizeTo(original, IconLarge);
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

            if (!TryGetBorderBitmap(OriginIndex, GetBorderState(iGrade), out var borderImage) || borderImage?.Bitmap is null) return;

            ConfigureGraphics(iTarget);
            iTarget.DrawImage(borderImage.Bitmap, iDest);
            iTarget.DrawImage(Enhancements[iImageIndex].Bitmap, iDest);
        }

        public static void DrawEnhancementAt(Graphics iTarget, Rectangle iDest, int iImageIndex, Origin.Grade iGrade, ImageAttributes imageAttributes)
        {
            if (iImageIndex < 0 || iImageIndex >= Enhancements.Count || Enhancements[iImageIndex]?.Bitmap is null) return;

            if (!TryGetBorderBitmap(OriginIndex, GetBorderState(iGrade), out var borderImage) || borderImage?.Bitmap is null) return;

            ConfigureGraphics(iTarget);
            var srcRect = new Rectangle(0, 0, borderImage.Size.Width, borderImage.Size.Height);
            iTarget.DrawImage(borderImage.Bitmap, iDest, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, imageAttributes);
            iTarget.DrawImage(Enhancements[iImageIndex].Bitmap, iDest, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, imageAttributes);
        }

        public static void DrawEnhancementAt(Graphics iTarget, Rectangle iDest, int iImageIndex, int enhancementId, Enums.eType typeId, Enums.eEnhGrade grade)
        {
            if (iImageIndex < 0 || iImageIndex >= Enhancements.Count || Enhancements[iImageIndex]?.Bitmap is null) return;
            if (!TryGetBorderBitmap(OriginIndex, ResolveBorderState(enhancementId, typeId, grade), out var borderImage) || borderImage?.Bitmap is null) return;

            ConfigureGraphics(iTarget);
            iTarget.DrawImage(borderImage.Bitmap, iDest);
            iTarget.DrawImage(Enhancements[iImageIndex].Bitmap, iDest);
        }

        public static void DrawEnhancementAt(Graphics iTarget, Rectangle iDest, int iImageIndex, int enhancementId, Enums.eType typeId, Enums.eEnhGrade grade, ImageAttributes imageAttributes)
        {
            if (iImageIndex < 0 || iImageIndex >= Enhancements.Count || Enhancements[iImageIndex]?.Bitmap is null) return;
            if (!TryGetBorderBitmap(OriginIndex, ResolveBorderState(enhancementId, typeId, grade), out var borderImage) || borderImage?.Bitmap is null) return;

            ConfigureGraphics(iTarget);
            var srcRect = new Rectangle(0, 0, borderImage.Size.Width, borderImage.Size.Height);
            iTarget.DrawImage(borderImage.Bitmap, iDest, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, imageAttributes);
            iTarget.DrawImage(Enhancements[iImageIndex].Bitmap, iDest, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, imageAttributes);
        }

        public static void DrawEnhancementSet(Graphics iTarget, int iImageIndex)
        {
            DrawEnhancementSet(iTarget, Rectangle.Truncate(iTarget.ClipBounds), iImageIndex);
        }

        public static void DrawEnhancementSet(Graphics iTarget, Rectangle iDest, int setId)
        {
            if (!TryGetSetBorderBitmap(setId, out var borderImage) || borderImage?.Bitmap is null) return;
            if (!Sets.TryGetValue(setId, out var setImage) || setImage?.Bitmap is null) return;

            ConfigureGraphics(iTarget);
            iTarget.DrawImage(borderImage.Bitmap, iDest);
            iTarget.DrawImage(setImage.Bitmap, iDest);
        }

        public static void DrawEnhancementSetVariant(Graphics iTarget, Rectangle iDest, int setId, SetVariantKind variantKind, string? badgeText = null)
        {
            if (!TryGetSetVariantBorderBitmap(variantKind, out var borderImage) || borderImage?.Bitmap is null) return;
            if (!Sets.TryGetValue(setId, out var setImage) || setImage?.Bitmap is null) return;

            ConfigureGraphics(iTarget);
            iTarget.DrawImage(borderImage.Bitmap, iDest);
            iTarget.DrawImage(setImage.Bitmap, iDest);
            DrawSetVariantBadge(iTarget, iDest, badgeText);
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

        public static ExtendedBitmap GetPowerImage(IPower? power)
        {
            return TryGetPowerBitmap(power, out var powerImage)
                ? powerImage ?? UnknownIcon
                : UnknownIcon;
        }

        public static bool TryGetPowerBitmap(IPower? power, out ExtendedBitmap powerImage)
        {
            powerImage = null;
            return power != null && TryGetNamedPowerBitmap(power.IconName, out powerImage);
        }

        public static string GetEnhancementsPath()
        {
            return Path.Combine(AppDataPaths.BaseAssetsPath, "Enhancements");
        }

        public static string GetAlignmentsPath()
        {
            return Path.Combine(AppDataPaths.BaseAssetsPath, "Alignments");
        }

        public static string GetDbAlignmentsPath()
        {
            return Path.Combine(MidsContext.Config.DataPath, "Assets", "Alignments");
        }

        public static string GetDbEnhancementsPath()
        {
            return Path.Combine(MidsContext.Config.DataPath, "Assets", "Enhancements");
        }

        public static string? ResolveNamedAlignmentImagePath(string? imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
            {
                return null;
            }

            var basePath = Path.Combine(GetAlignmentsPath(), imageName);
            if (File.Exists(basePath))
            {
                return basePath;
            }

            var dbPath = Path.Combine(GetDbAlignmentsPath(), imageName);
            return File.Exists(dbPath) ? dbPath : null;
        }

        public static bool TryGetNamedAlignmentBitmap(string? imageName, out ExtendedBitmap alignmentImage)
        {
            alignmentImage = null;
            if (string.IsNullOrWhiteSpace(imageName))
            {
                return false;
            }

            var imagePath = ResolveNamedAlignmentImagePath(imageName);
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return false;
            }

            if (NamedAlignmentImages.TryGetValue(imagePath, out alignmentImage) && alignmentImage?.Bitmap != null)
            {
                return true;
            }

            if (!File.Exists(imagePath))
            {
                return false;
            }

            alignmentImage = new ExtendedBitmap(imagePath);
            NamedAlignmentImages[imagePath] = alignmentImage;
            return alignmentImage.Bitmap != null;
        }

        public static bool TryGetNamedPowerBitmap(string? imageName, out ExtendedBitmap powerImage)
        {
            powerImage = null;
            if (string.IsNullOrWhiteSpace(imageName))
            {
                return false;
            }

            var imagePath = ResolveNamedPowerImagePath(imageName);
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return false;
            }

            if (NamedPowerImages.TryGetValue(imagePath, out powerImage) && powerImage?.Bitmap != null)
            {
                return true;
            }

            if (!File.Exists(imagePath))
            {
                return false;
            }

            powerImage = new ExtendedBitmap(imagePath);
            NamedPowerImages[imagePath] = powerImage;
            return powerImage.Bitmap != null;
        }

        public static string? ResolveNamedEnhancementImagePath(string? imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
            {
                return null;
            }

            var basePath = Path.Combine(GetEnhancementsPath(), imageName);
            if (File.Exists(basePath))
            {
                return basePath;
            }

            var dbPath = Path.Combine(GetDbEnhancementsPath(), imageName);
            return File.Exists(dbPath) ? dbPath : null;
        }

        private static string? ResolveNamedPowerImagePath(string? imageName)
        {
            foreach (var candidate in BuildPowerImageCandidates(imageName))
            {
                var dbPath = Path.Combine(GetDbPowersPath(), candidate);
                if (File.Exists(dbPath))
                {
                    return dbPath;
                }

                var basePath = Path.Combine(GetPowersPath(), candidate);
                if (File.Exists(basePath))
                {
                    return basePath;
                }
            }

            return null;
        }

        public static bool TryGetNamedEnhancementBitmap(string? imageName, out ExtendedBitmap enhancementImage)
        {
            enhancementImage = null;
            if (string.IsNullOrWhiteSpace(imageName))
            {
                return false;
            }

            var imagePath = ResolveNamedEnhancementImagePath(imageName);
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return false;
            }

            if (NamedEnhancementImages.TryGetValue(imagePath, out enhancementImage) && enhancementImage?.Bitmap != null)
            {
                return true;
            }

            if (!File.Exists(imagePath))
            {
                return false;
            }

            enhancementImage = new ExtendedBitmap(imagePath);
            NamedEnhancementImages[imagePath] = enhancementImage;
            return enhancementImage.Bitmap != null;
        }

        public static int ResolveBuildOriginIndex(int? preferredOriginIndex = null)
        {
            if (preferredOriginIndex.HasValue &&
                DatabaseAPI.Database?.Origins != null &&
                preferredOriginIndex.Value >= 0 &&
                preferredOriginIndex.Value < DatabaseAPI.Database.Origins.Count)
            {
                return preferredOriginIndex.Value;
            }

            var characterOrigin = MidsContext.Character?.Origin ?? -1;
            if (DatabaseAPI.Database?.Origins != null &&
                characterOrigin >= 0 &&
                characterOrigin < DatabaseAPI.Database.Origins.Count)
            {
                return characterOrigin;
            }

            if (DatabaseAPI.Database?.Origins != null &&
                OriginIndex >= 0 &&
                OriginIndex < DatabaseAPI.Database.Origins.Count)
            {
                return OriginIndex;
            }

            return 0;
        }

        public static string? ResolveNamedOverlayImagePath(string? imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
            {
                return null;
            }

            var overlayImages = Images.Where(image => string.Equals(image.Bucket, BucketOverlay, StringComparison.OrdinalIgnoreCase));
            return TryFindImagePath(overlayImages, [imageName], out var resolvedPath) ? resolvedPath : null;
        }

        public static bool TryGetNamedOverlayBitmap(string? imageName, out ExtendedBitmap overlayImage)
        {
            overlayImage = null;
            if (string.IsNullOrWhiteSpace(imageName))
            {
                return false;
            }

            var imagePath = ResolveNamedOverlayImagePath(imageName);
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return false;
            }

            if (NamedOverlayImages.TryGetValue(imagePath, out overlayImage) && overlayImage?.Bitmap != null)
            {
                return true;
            }

            if (!File.Exists(imagePath))
            {
                return false;
            }

            overlayImage = new ExtendedBitmap(imagePath);
            NamedOverlayImages[imagePath] = overlayImage;
            return overlayImage.Bitmap != null;
        }

        public static string GetPowersetsPath()
        {
            return Path.Combine(AppDataPaths.BaseAssetsPath, "Powersets");
        }

        public static string GetPowersPath()
        {
            return Path.Combine(AppDataPaths.BaseAssetsPath, "Powers");
        }

        public static string GetDbPowersPath()
        {
            return Path.Combine(MidsContext.Config.DataPath, "Assets", "Powers");
        }

        public static string GetDbPowerSetsPath()
        {
            return Path.Combine(MidsContext.Config.DataPath, "Assets", "Powersets");
        }

        public static bool TryGetBorderBitmap(IEnhancement enhancement, Enums.eEnhGrade grade, out ExtendedBitmap borderImage)
        {
            borderImage = null;
            if (enhancement == null)
            {
                return false;
            }

            return TryGetBorderBitmap(OriginIndex, ResolveBorderState(enhancement, grade), out borderImage);
        }

        public static bool TryGetBorderBitmap(Origin.Grade grade, out ExtendedBitmap borderImage)
        {
            borderImage = null;
            return TryGetBorderBitmap(OriginIndex, GetBorderState(grade), out borderImage);
        }

        public static bool TryGetSetBorderBitmap(int setId, out ExtendedBitmap borderImage)
        {
            borderImage = null;
            if (setId < 0 || setId >= DatabaseAPI.Database.EnhancementSets.Count)
            {
                return TryGetBorderBitmap(OriginIndex, BorderState.SetCrafted, out borderImage);
            }

            return TryGetSetBorderBitmap(DatabaseAPI.Database.EnhancementSets[setId], out borderImage);
        }

        public static bool TryGetSetBorderBitmap(EnhancementSet enhancementSet, out ExtendedBitmap borderImage)
        {
            borderImage = null;
            if (enhancementSet == null)
            {
                return false;
            }

            return TryGetBorderBitmap(OriginIndex, ResolveSetBorderState(enhancementSet), out borderImage);
        }

        public static bool TryGetSetVariantBorderBitmap(SetVariantKind variantKind, out ExtendedBitmap borderImage)
        {
            borderImage = null;
            return TryGetBorderBitmap(OriginIndex, ResolveSetBorderState(variantKind), out borderImage);
        }

        public static bool TryGetClassicVariantBorderBitmap(
            ClassicEnhancementVariantView? variant,
            int buildOriginIndex,
            out ExtendedBitmap borderImage)
        {
            borderImage = null;
            if (variant == null)
            {
                return false;
            }

            var resolvedOriginIndex = ResolveBuildOriginIndex(buildOriginIndex);
            foreach (var candidate in BuildClassicVariantBorderImageCandidates(variant, resolvedOriginIndex)
                         .Where(candidate => !string.IsNullOrWhiteSpace(candidate))
                         .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (TryGetNamedOverlayBitmap(candidate, out borderImage) && borderImage?.Bitmap != null)
                {
                    return true;
                }
            }

            // Final fallback for incomplete data packs: use the generic grade border.
            var fallbackGrade = ToGfxGrade(Enums.eType.Normal, variant.DisplayGrade);
            return TryGetBorderBitmap(resolvedOriginIndex, GetBorderState(fallbackGrade), out borderImage);
        }

        public static bool TryDrawClassicVariantAt(
            Graphics iTarget,
            Rectangle iDest,
            ClassicEnhancementVariantView? variant,
            int buildOriginIndex,
            ImageAttributes? imageAttributes = null)
        {
            if (variant == null ||
                !TryGetNamedEnhancementBitmap(variant.Icon, out var enhancementImage) ||
                enhancementImage?.Bitmap == null)
            {
                return false;
            }

            ConfigureGraphics(iTarget);
            var hasBorder = TryGetClassicVariantBorderBitmap(variant, buildOriginIndex, out var borderImage) &&
                borderImage?.Bitmap != null;
            if (imageAttributes == null)
            {
                if (hasBorder)
                {
                    iTarget.DrawImage(borderImage.Bitmap, iDest);
                }

                iTarget.DrawImage(enhancementImage.Bitmap, iDest);
                return true;
            }

            var srcRect = new Rectangle(0, 0, enhancementImage.Size.Width, enhancementImage.Size.Height);
            if (hasBorder)
            {
                var borderRect = new Rectangle(0, 0, borderImage.Size.Width, borderImage.Size.Height);
                iTarget.DrawImage(borderImage.Bitmap, iDest, borderRect.X, borderRect.Y, borderRect.Width, borderRect.Height, GraphicsUnit.Pixel, imageAttributes);
            }

            iTarget.DrawImage(enhancementImage.Bitmap, iDest, srcRect.X, srcRect.Y, srcRect.Width, srcRect.Height, GraphicsUnit.Pixel, imageAttributes);
            return true;
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

        private static bool TryGetBorderBitmap(int originIndex, BorderState borderState, out ExtendedBitmap borderImage)
        {
            borderImage = null;
            return Borders.TryGetValue(new Point(originIndex, (int)borderState), out borderImage) && borderImage?.Bitmap is not null;
        }

        private static BorderState GetBorderState(Origin.Grade grade)
        {
            return grade switch
            {
                Origin.Grade.TrainingO => BorderState.Training,
                Origin.Grade.DualO => BorderState.DualOrigin,
                Origin.Grade.SingleO => BorderState.SingleOrigin,
                Origin.Grade.HO => BorderState.Special,
                Origin.Grade.IO => BorderState.Invention,
                Origin.Grade.SetO => BorderState.SetCrafted,
                Origin.Grade.Attuned => BorderState.SetAttuned,
                _ => BorderState.Special
            };
        }

        private static BorderState ResolveBorderState(int enhancementId, Enums.eType typeId, Enums.eEnhGrade grade)
        {
            if (typeId == Enums.eType.SetO)
            {
                return ResolveSetBorderState(enhancementId);
            }

            return typeId switch
            {
                Enums.eType.Normal => grade switch
                {
                    Enums.eEnhGrade.DualO => BorderState.DualOrigin,
                    Enums.eEnhGrade.SingleO => BorderState.SingleOrigin,
                    _ => BorderState.Training
                },
                Enums.eType.InventO => BorderState.Invention,
                Enums.eType.SpecialO => BorderState.Special,
                _ => BorderState.Special
            };
        }

        private static BorderState ResolveBorderState(IEnhancement enhancement, Enums.eEnhGrade grade)
        {
            if (enhancement.TypeID == Enums.eType.SetO)
            {
                return ResolveSetBorderState(enhancement);
            }

            return enhancement.TypeID switch
            {
                Enums.eType.Normal => grade switch
                {
                    Enums.eEnhGrade.DualO => BorderState.DualOrigin,
                    Enums.eEnhGrade.SingleO => BorderState.SingleOrigin,
                    _ => BorderState.Training
                },
                Enums.eType.InventO => BorderState.Invention,
                Enums.eType.SpecialO => BorderState.Special,
                _ => BorderState.Special
            };
        }

        private static BorderState ResolveSetBorderState(int enhancementId)
        {
            if (enhancementId < 0 || enhancementId >= DatabaseAPI.Database.Enhancements.Length)
            {
                return BorderState.SetCrafted;
            }

            return ResolveSetBorderState(DatabaseAPI.GetSetVariantKind(enhancementId));
        }

        private static BorderState ResolveSetBorderState(IEnhancement enhancement)
        {
            return ResolveSetBorderState(enhancement.StaticIndex);
        }

        private static BorderState ResolveSetBorderState(EnhancementSet enhancementSet)
        {
            var setId = DatabaseAPI.Database.EnhancementSets.TryFindIndex(set =>
                ReferenceEquals(set, enhancementSet) ||
                string.Equals(set?.Uid, enhancementSet.Uid, StringComparison.OrdinalIgnoreCase));
            var variants = DatabaseAPI.GetAvailableSetVariants(setId);
            if (variants.Count == 1)
            {
                return ResolveSetBorderState(variants[0]);
            }

            return BorderState.SetCrafted;
        }

        private static BorderState ResolveSetBorderState(SetVariantKind variantKind)
        {
            return variantKind switch
            {
                SetVariantKind.Attuned => BorderState.SetAttuned,
                SetVariantKind.Superior => BorderState.SetSuperiorAttuned,
                SetVariantKind.SuperiorAttuned => BorderState.SetSuperiorAttuned,
                _ => BorderState.SetCrafted
            };
        }

        #endregion

        #region File and Image Helpers
        private struct ImageInfo
        {
            public string FileName { get; set; }
            public string RelativeDirectory { get; set; }
            public string Bucket { get; set; }
            public string Path { get; set; }
            public bool IsBase { get; set; }
        }

        private static IEnumerable<ImageInfo> GetBaseImages()
        {
            return GetImagesFromRoot(AppDataPaths.BaseAssetsPath, isBase: true);
        }

        private static IEnumerable<ImageInfo> GetExtendedImages(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return [];
            }

            return GetImagesFromRoot(Path.Combine(path, "Assets"), isBase: false);
        }

        private static ILookup<string, ImageInfo> BuildImageGroups()
        {
            return Images.ToLookup(img => img.Bucket, StringComparer.OrdinalIgnoreCase);
        }

        private static IEnumerable<ImageInfo> GetImagesFromRoot(string rootPath, bool isBase)
        {
            var retList = new List<ImageInfo>();
            if (!Directory.Exists(rootPath))
            {
                return retList;
            }

            var files = Directory.GetFiles(rootPath, ImageFilter, SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var relativePath = Path.GetRelativePath(rootPath, file);
                var relativeDirectory = Path.GetDirectoryName(relativePath) ?? string.Empty;
                var fInfo = new FileInfo(file);
                retList.Add(new ImageInfo
                {
                    FileName = fInfo.Name,
                    RelativeDirectory = relativeDirectory,
                    Bucket = ClassifyImageBucket(relativeDirectory),
                    Path = file,
                    IsBase = isBase
                });
            }

            return retList;
        }

        private static string ClassifyImageBucket(string relativeDirectory)
        {
            var key = NormalizeRelativePathKey(relativeDirectory);
            return key switch
            {
                BucketArchetypes => BucketArchetypes,
                BucketClasses => BucketClasses,
                BucketEnhancements => BucketEnhancements,
                BucketOrigins => BucketOrigins,
                BucketOverlay => BucketOverlay,
                BucketPowersets => BucketPowersets,
                BucketSets => BucketSets,
                BucketTypesRoot => BucketTypesRoot,
                BucketTypeGrades => BucketTypeGrades,
                BucketTypeSets => BucketTypeSets,
                _ => string.Empty
            };
        }

        private static string NormalizeRelativePathKey(string? relativeDirectory)
        {
            if (string.IsNullOrWhiteSpace(relativeDirectory))
            {
                return string.Empty;
            }

            var segments = relativeDirectory
                .Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries)
                .Select(NormalizeImageNameKey)
                .Where(segment => !string.IsNullOrWhiteSpace(segment))
                .ToArray();

            return segments.Length == 0 ? string.Empty : string.Join("/", segments);
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
                    path = exact.Path;
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
                    path = normalized.Path;
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

        private static IEnumerable<string> BuildPowerImageCandidates(string imageName)
        {
            if (string.IsNullOrWhiteSpace(imageName))
            {
                yield break;
            }

            foreach (var candidate in new[]
                     {
                         imageName.Trim(),
                         Path.GetFileName(imageName.Trim())
                     }.Where(candidate => !string.IsNullOrWhiteSpace(candidate))
                     .Distinct(StringComparer.OrdinalIgnoreCase))
            {
                yield return candidate;
                var pngCandidate = Path.ChangeExtension(candidate, ".png");
                if (!string.IsNullOrWhiteSpace(pngCandidate))
                {
                    yield return pngCandidate;
                }

                var baseName = Path.GetFileNameWithoutExtension(candidate);
                if (!string.IsNullOrWhiteSpace(baseName))
                {
                    yield return baseName;
                    yield return $"{baseName}.png";
                }

                if (!candidate.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    yield return $"{candidate}.png";
                }
            }
        }

        private static IEnumerable<string> BuildEnhancementTypeImageCandidates(Enums.eType type)
        {
            switch (type)
            {
                case Enums.eType.None:
                    yield return "None.png";
                    break;
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
            switch (grade)
            {
                case Enums.eEnhGrade.None:
                    yield return "None.png";
                    break;
                case Enums.eEnhGrade.TrainingO:
                    yield return "to.png";
                    break;
                case Enums.eEnhGrade.DualO:
                    yield return "do.png";
                    break;
                case Enums.eEnhGrade.SingleO:
                    yield return "so.png";
                    break;
            }
        }

        internal static IEnumerable<string> BuildSetTypeImageCandidates(string? shortName)
        {
            var canonicalShortName = DatabaseAPI.CanonicalizeSetTypeShortName(shortName);
            if (string.IsNullOrWhiteSpace(canonicalShortName))
            {
                yield break;
            }

            switch (canonicalShortName)
            {
                case "Untyped":
                    yield return "untyped.png";
                    break;
                case "MeleeAoEDamage":
                    yield return "melee_aoe.png";
                    break;
                case "MeleeDamage":
                    yield return "melee_single_target.png";
                    break;
                case "RangedAoEDamage":
                    yield return "ranged_aoe.png";
                    break;
                case "RangedDamage":
                    yield return "ranged_single_target.png";
                    break;
                case "RechargeIntensivePets":
                    yield return "pet_recharge.png";
                    break;
                case "UniversalDamage":
                    yield return "universal_damage.png";
                    break;
                case "DefenseDebuff":
                    yield return "defense_debuff.png";
                    break;
                case "ThreatDuration":
                    yield return "taunt.png";
                    break;
                case "ToHitDebuff":
                    yield return "tohit_debuff.png";
                    break;
                case "AccurateHealing":
                    yield return "accurate_heal.png";
                    break;
                case "AccurateDefenseDebuff":
                    yield return "accurate_defense_debuff.png";
                    break;
                case "AccurateToHitDebuff":
                    yield return "accurate_tohit_debuff.png";
                    break;
                case "EnduranceModification":
                    yield return "end_mod.png";
                    break;
                case "UniversalTravel":
                    yield return "travel.png";
                    break;
            }

            foreach (var candidateShortName in DatabaseAPI.GetSetTypeShortNameVariants(canonicalShortName).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                yield return $"{candidateShortName}.png";
            }
        }

        private static IEnumerable<string> BuildSpecialRailImageCandidates(Mids_Reborn.Core.Utils.TypeGrade specialEnhancement)
        {
            if (!string.IsNullOrWhiteSpace(specialEnhancement.ShortName))
            {
                switch (specialEnhancement.ShortName)
                {
                    case "HO":
                        yield return "hamidon.png";
                        break;
                    case "SynHO":
                        yield return "synthetic_hamidon.png";
                        break;
                    case "HyO":
                        yield return "hydra.png";
                        break;
                    case "TnO":
                        yield return "titan.png";
                        break;
                    case "DSyncO":
                        yield return "d-sync.png";
                        yield return "dsync.png";
                        break;
                    case "Yin":
                        yield return "yin.png";
                        break;
                }

                yield return $"{specialEnhancement.ShortName}.png";
            }

            if (!string.IsNullOrWhiteSpace(specialEnhancement.Name))
            {
                switch (specialEnhancement.Name)
                {
                    case "Hamidon Origin":
                    case "Hamidon":
                        yield return "hamidon.png";
                        break;
                    case "Synthetic Hamidon":
                        yield return "synthetic_hamidon.png";
                        break;
                    case "Hydra Origin":
                    case "Hydra":
                        yield return "hydra.png";
                        break;
                    case "Titan Origin":
                    case "Titan":
                        yield return "titan.png";
                        break;
                    case "D-Sync Origin":
                    case "D-Sync":
                        yield return "d-sync.png";
                        yield return "dsync.png";
                        break;
                    case "Yin's Talisman":
                        yield return "yin.png";
                        break;
                }

                yield return $"{specialEnhancement.Name}.png";
            }
        }

        private static IEnumerable<string> BuildBorderImageCandidates(Origin origin, BorderState borderState)
        {
            if (origin == null)
            {
                yield break;
            }

            switch (borderState)
            {
                case BorderState.Training:
                    yield return "generic.png";
                    break;
                case BorderState.DualOrigin:
                    yield return $"{origin.Grades[(int)Origin.Grade.DualO]}.png";
                    break;
                case BorderState.SingleOrigin:
                    yield return $"{origin.Grades[(int)Origin.Grade.SingleO]}.png";
                    break;
                case BorderState.Special:
                    yield return "uber.png";
                    break;
                case BorderState.Invention:
                case BorderState.SetCrafted:
                    yield return "invention.png";
                    break;
                case BorderState.SetAttuned:
                    yield return "attuned.png";
                    break;
                case BorderState.SetSuperiorAttuned:
                    yield return "superior_attuned.png";
                    break;
            }
        }

        private static void DrawSetVariantBadge(Graphics target, Rectangle bounds, string? badgeText)
        {
            if (string.IsNullOrWhiteSpace(badgeText))
            {
                return;
            }

            var badgeBounds = new Rectangle(bounds.Right - 18, bounds.Bottom - 14, 16, 12);
            using var backgroundBrush = new SolidBrush(Color.FromArgb(220, 16, 20, 28));
            using var textBrush = new SolidBrush(Color.White);
            using var badgeFont = new Font(SystemFonts.MessageBoxFont.FontFamily, 6.75f, FontStyle.Bold, GraphicsUnit.Point);
            using var stringFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            target.FillRectangle(backgroundBrush, badgeBounds);
            target.DrawString(badgeText, badgeFont, textBrush, badgeBounds, stringFormat);
        }
        private static IEnumerable<string> BuildClassicVariantBorderImageCandidates(
            ClassicEnhancementVariantView variant,
            int buildOriginIndex)
        {
            if (variant == null)
            {
                yield break;
            }

            switch (variant.DisplayGrade)
            {
                case Enums.eEnhGrade.TrainingO:
                    yield return "generic.png";
                    yield break;
                case Enums.eEnhGrade.SingleO:
                    foreach (var alias in BuildOriginOverlayAliases(variant.PrimaryOrigin))
                    {
                        yield return $"{alias}.png";
                    }
                    break;
                case Enums.eEnhGrade.DualO:
                    var primaryAliases = BuildOriginOverlayAliases(variant.PrimaryOrigin).ToArray();
                    var secondaryAliases = BuildOriginOverlayAliases(variant.SecondaryOrigin).ToArray();
                    if (primaryAliases.Length > 0 && secondaryAliases.Length > 0)
                    {
                        foreach (var primaryAlias in primaryAliases)
                        {
                            foreach (var secondaryAlias in secondaryAliases)
                            {
                                yield return $"{primaryAlias}_{secondaryAlias}.png";
                            }
                        }

                        foreach (var secondaryAlias in secondaryAliases)
                        {
                            foreach (var primaryAlias in primaryAliases)
                            {
                                yield return $"{secondaryAlias}_{primaryAlias}.png";
                            }
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(variant.PrimaryOrigin))
                    {
                        foreach (var alias in BuildOriginOverlayAliases(variant.PrimaryOrigin))
                        {
                            yield return $"{alias}.png";
                        }
                    }

                    if (DatabaseAPI.Database?.Origins != null &&
                        buildOriginIndex >= 0 &&
                        buildOriginIndex < DatabaseAPI.Database.Origins.Count)
                    {
                        var origin = DatabaseAPI.Database.Origins[buildOriginIndex];
                        if (!string.IsNullOrWhiteSpace(origin?.Grades[(int)Origin.Grade.DualO]))
                        {
                            yield return $"{origin.Grades[(int)Origin.Grade.DualO]}.png";
                        }
                    }
                    break;
            }
        }

        private static IEnumerable<string> BuildOriginOverlayAliases(string? originName)
        {
            if (string.IsNullOrWhiteSpace(originName))
            {
                yield break;
            }

            foreach (var alias in BuildOverlayAliasesFromToken(originName))
            {
                yield return alias;
            }

            var originIndex = DatabaseAPI.GetOriginIDByName(originName);
            if (DatabaseAPI.Database?.Origins == null ||
                originIndex < 0 ||
                originIndex >= DatabaseAPI.Database.Origins.Count)
            {
                yield break;
            }

            var origin = DatabaseAPI.Database.Origins[originIndex];
            foreach (var token in new[]
                     {
                         origin?.Grades[(int)Origin.Grade.SingleO],
                         origin?.Grades[(int)Origin.Grade.DualO]
                     })
            {
                foreach (var alias in BuildOverlayAliasesFromToken(token))
                {
                    yield return alias;
                }
            }
        }

        private static IEnumerable<string> BuildOverlayAliasesFromToken(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                yield break;
            }

            var normalized = NormalizeImageNameKey(token);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                yield break;
            }

            switch (normalized)
            {
                case "magso":
                case "magdo":
                case "mag":
                case "magic":
                    yield return "magic";
                    yield break;
                case "mutso":
                case "mutdo":
                case "mut":
                case "mutation":
                case "mutant":
                    yield return "mutant";
                    yield return "mutation";
                    yield break;
                case "natso":
                case "natdo":
                case "nat":
                case "natural":
                    yield return "natural";
                    yield break;
                case "sciso":
                case "scido":
                case "sci":
                case "science":
                    yield return "science";
                    yield break;
                case "techso":
                case "techdo":
                case "tech":
                case "technology":
                    yield return "tech";
                    yield return "technology";
                    yield break;
                default:
                    yield return normalized;
                    yield break;
            }
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
