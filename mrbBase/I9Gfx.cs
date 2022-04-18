using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Display;
using mrbBase.Base.Master_Classes;
using Newtonsoft.Json;

namespace mrbBase
{
    public static class I9Gfx
    {
        private const int IconLarge = 30;
        private const int IconSmall = 16;

        private const string ImageFilter = "*.png";
        private static List<ImageInfo> Images { get; set; }
        private static bool Initialized { get; set; }

        public static int OriginIndex;
        public static Bitmap[] Enhancements;
        public static ExtendedBitmap Borders;
        public static ExtendedBitmap Sets;
        public static ExtendedBitmap Classes;
        public static ExtendedBitmap SetTypes;
        public static ExtendedBitmap EnhTypes;
        public static ExtendedBitmap EnhGrades;
        public static ExtendedBitmap EnhSpecials;
        public static ExtendedBitmap Archetypes;
        public static ExtendedBitmap Origins;
        public static ExtendedBitmap Powersets;
        public static ExtendedBitmap UnknownPowerset;
        public static ExtendedBitmap UnknownArchetype;

        private struct ImageInfo
        {
            public string FileName { get; set; }
            public string Directory { get; set; }
            public string Path { get; set; }
            public bool IsBase { get; set; }
        }

        private static string BaseImagePath => Path.Combine(Application.StartupPath, "Images");

        public static string ImagePath(string type = "")
        {
            return !string.IsNullOrWhiteSpace(type) ? Path.Combine(BaseImagePath, type) : BaseImagePath;
        }

        private static IEnumerable<ImageInfo> GetBaseImages()
        {
            var retList = new List<ImageInfo>();
            var files = Directory.GetFiles(BaseImagePath, ImageFilter, SearchOption.AllDirectories).ToList();
            foreach (var file in files)
            {
                var fInfo = new FileInfo(file);
                retList.Add(new ImageInfo { FileName = fInfo.Name, Directory = fInfo.Directory?.Name, Path = file, IsBase = true });
            }

            return retList;
        }

        private static IEnumerable<ImageInfo> GetExtendedImages(string path)
        {
            var retList = new List<ImageInfo>();
            var files = Directory.GetFiles(path, ImageFilter, SearchOption.AllDirectories).ToList();
            foreach (var file in files)
            {
                var fInfo = new FileInfo(file);
                retList.Add(new ImageInfo { FileName = fInfo.Name, Directory = fInfo.Directory?.Name, Path = file, IsBase = false });
            }

            return retList;
        }

        private static ExtendedBitmap ExtendedBitmap(int x, int y)
        {
            var exBmp = new ExtendedBitmap(x, y);
            exBmp.Graphics.CompositingMode = CompositingMode.SourceOver;
            exBmp.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            exBmp.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            exBmp.Graphics.PageUnit = GraphicsUnit.Pixel;
            exBmp.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            exBmp.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            exBmp.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            return exBmp;
        }

        public static void SetOrigin(string iOrigin)
        {
            OriginIndex = DatabaseAPI.GetOriginIDByName(iOrigin);
        }

        public static async Task Initialize(string path)
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
            var archetypeImages = Images.Where(x => x.Directory == "Archetypes").ToList();
            var classImages = Images.Where(x => x.Directory == "Classes").ToList();
            var enhancementImages = Images.Where(x => x.Directory == "Enhancements").ToList();
            var originImages = Images.Where(x => x.Directory == "Origins").ToList();
            var powersetImages = Images.Where(x => x.Directory == "Powersets").ToList();
            var setImages = Images.Where(x => x.Directory == "Sets").ToList();

            await LoadOriginImages(originImages);
            await LoadArchetypeImages(archetypeImages, baseImages);
            await LoadPowersetImages(powersetImages, baseImages);
            await LoadEnhancementImages(enhancementImages, baseImages);
            await LoadEnhancementSetImages(enhancementImages, baseImages);
            await LoadBorderImages(baseImages);
            await LoadSetTypeImages(setImages, baseImages);
            await LoadEnhTypeImages(setImages, baseImages);
            await LoadEnhancementClassImages(classImages, baseImages);

            await Task.CompletedTask;
        }

        public static async Task<List<string>> LoadArchetypes()
        {
            var cSource = new TaskCompletionSource<List<string>>();
            var retList = new List<string>();
            var baseImages = Images.Where(x => x.IsBase).ToList();
            var archetypeImages = Images.Where(x => x.Directory == "Archetypes").ToList();
            var unknown = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            for (var index = 0; index <= DatabaseAPI.Database.Classes.Length - 1; ++index)
            {
                var path = archetypeImages.FirstOrDefault(i => i.FileName == $"{DatabaseAPI.Database.Classes[index].ClassName}.png").Path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = unknown;
                }
                retList.Add(path);
            }

            cSource.TrySetResult(retList);
            return await cSource.Task;
        }

        public static async Task LoadEnhancements()
        {
            var baseImages = Images.Where(x => x.IsBase).ToList();
            var enhancmentImages = Images.Where(x => x.Directory == "Enhancements").ToList();
            await LoadEnhancementImages(enhancmentImages, baseImages);
            await Task.CompletedTask;
        }

        public static async Task<List<string>> LoadSets()
        {
            var cSource = new TaskCompletionSource<List<string>>();
            var retList = new List<string>();
            var baseImages = Images.Where(x => x.IsBase).ToList();
            var enhancementImages = Images.Where(x => x.Directory == "Enhancements").ToList();
            var unknown = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            for (var index = 0; index <= DatabaseAPI.Database.EnhancementSets.Count - 1; ++index)
            {
                //Debug.WriteLine(DatabaseAPI.Database.EnhancementSets[index].Image);
                var path = enhancementImages.FirstOrDefault(i => i.FileName == DatabaseAPI.Database.EnhancementSets[index].Image).Path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = unknown;
                }
                retList.Add(path);
            }

            // foreach (var img in enhancementImages)
            // {
            //     Debug.WriteLine(img.Path);
            // }

            for (var index = 0; index < retList.Count; index++)
            {
                DatabaseAPI.Database.EnhancementSets[index].ImageIdx = index;
            }

            cSource.TrySetResult(retList);
            return await cSource.Task;
        }

        private static async Task LoadArchetypeImages(IReadOnlyCollection<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknown = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            Archetypes = ExtendedBitmap(DatabaseAPI.Database.Classes.Length * IconSmall, IconSmall);
            for (var index = 0; index <= DatabaseAPI.Database.Classes.Length - 1; ++index)
            {
                var x = index * IconSmall;
                var path = images.FirstOrDefault(i => i.FileName == $"{DatabaseAPI.Database.Classes[index].ClassName}.png").Path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = unknown;
                }

                using var extendedBitmap = new ExtendedBitmap(path);
                if ((extendedBitmap.Size.Height > IconSmall) | (extendedBitmap.Size.Width > IconSmall))
                {
                    Archetypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, IconSmall, IconSmall);
                }
                else
                {
                    Archetypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
                }
            }

            UnknownArchetype = new ExtendedBitmap(unknown);
            await Task.CompletedTask;
        }

        private static async Task LoadPowersetImages(IReadOnlyCollection<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknown = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            Powersets = ExtendedBitmap(DatabaseAPI.Database.Powersets.Length * IconSmall, IconSmall);
            for (var index = 0; index <= DatabaseAPI.Database.Powersets.Length - 1; ++index)
            {
                var x = index * IconSmall;
                var path = images.FirstOrDefault(i => i.FileName == DatabaseAPI.Database.Powersets[index].ImageName).Path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = unknown;
                }

                using var extendedBitmap = new ExtendedBitmap(path);
                if ((extendedBitmap.Size.Height > IconSmall) | (extendedBitmap.Size.Width > IconSmall))
                {
                    Powersets.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, IconSmall, IconSmall);
                }
                else
                {
                    Powersets.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
                }
            }

            UnknownPowerset = new ExtendedBitmap(unknown);
            await Task.CompletedTask;
        }

        private static async Task LoadOriginImages(IReadOnlyCollection<ImageInfo> images)
        {
            Origins = ExtendedBitmap(DatabaseAPI.Database.Origins.Count * IconSmall, IconSmall);
            for (var index = 0; index <= DatabaseAPI.Database.Origins.Count - 1; ++index)
            {
                var x = index * IconSmall;
                var path = images.FirstOrDefault(i => i.FileName.Contains(DatabaseAPI.Database.Origins[index].Name)).Path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                using var extendedBitmap = new ExtendedBitmap(path);
                if ((extendedBitmap.Size.Height > IconSmall) | (extendedBitmap.Size.Width > IconSmall))
                {
                    Origins.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, IconSmall, IconSmall);
                }
                else
                {
                    Origins.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
                }
            }
            await Task.CompletedTask;
        }

        private static async Task LoadEnhancementClassImages(IReadOnlyCollection<ImageInfo> images, IReadOnlyCollection<ImageInfo> baseImages)
        {
            var classImage = baseImages.FirstOrDefault(i => i.FileName == "Class.png").Path;
            var incImage = baseImages.FirstOrDefault(i => i.FileName == "Inc.png").Path;
            Classes = ExtendedBitmap(DatabaseAPI.Database.EnhancementClasses.Length * IconLarge, IconLarge);
            var overlayBitmap = new ExtendedBitmap(classImage);
            for (var index = 0; index <= DatabaseAPI.Database.EnhancementClasses.Length - 1; ++index)
            {
                if (index >= 27)
                {
                    overlayBitmap = new ExtendedBitmap(incImage);
                }

                var x = index * IconLarge;
                var path = images.FirstOrDefault(i => i.FileName == $"{DatabaseAPI.Database.EnhancementClasses[index].ID}.png").Path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                using var extendedBitmap = new ExtendedBitmap(path);
                Classes.Graphics.DrawImageUnscaled(overlayBitmap.Bitmap, x, 0);
                if ((extendedBitmap.Size.Height > IconLarge) | (extendedBitmap.Size.Width > IconLarge))
                {
                    Classes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, IconLarge, IconLarge);
                }
                else
                {
                    Classes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
                }
            }
            overlayBitmap.Dispose();
            GC.Collect();
            await Task.CompletedTask;
        }

        private static async Task LoadEnhancementImages(IReadOnlyCollection<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknown = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            Enhancements = new Bitmap[DatabaseAPI.Database.Enhancements.Length];
            for (var index = 0; index <= DatabaseAPI.Database.Enhancements.Length - 1; ++index)
            {
                if (!string.IsNullOrWhiteSpace(DatabaseAPI.Database.Enhancements[index].Image))
                {
                    try
                    {
                        var path = images.FirstOrDefault(i => i.FileName == DatabaseAPI.Database.Enhancements[index].Image).Path;
                        if (string.IsNullOrWhiteSpace(path))
                        {
                            path = unknown;
                        }

                        Enhancements[index] = new Bitmap(path);
                    }
                    catch (Exception)
                    {
                        Enhancements[index] = new Bitmap(IconLarge, IconLarge, PixelFormat.Format32bppArgb);
                    }

                    DatabaseAPI.Database.Enhancements[index].ImageIdx = index;
                }
                else
                {
                    Enhancements[index] = new Bitmap(IconLarge, IconLarge, PixelFormat.Format32bppArgb);
                    DatabaseAPI.Database.Enhancements[index].ImageIdx = -1;
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
            Sets = ExtendedBitmap(DatabaseAPI.Database.EnhancementSets.Count * IconLarge, IconLarge);
            for (var index = 0; index <= DatabaseAPI.Database.EnhancementSets.Count - 1; ++index)
            {
                var x = index * IconLarge;
                var path = images.FirstOrDefault(i => i.FileName == DatabaseAPI.Database.EnhancementSets[index].Image).Path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = unknown;
                }

                using var extendedBitmap = new ExtendedBitmap(path);
                var size = extendedBitmap.Size;
                int num;
                if (size.Height <= 30)
                {
                    size = extendedBitmap.Size;
                    num = size.Width <= 30 ? 1 : 0;
                }
                else
                {
                    num = 0;
                }

                if (num == 0)
                {
                    Sets.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 30, 30);
                }
                else
                {
                    Sets.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
                }
                
            }
            await Task.CompletedTask;
        }

        private static async Task LoadSetTypeImages(IReadOnlyCollection<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknown = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;

            var setTypes = DatabaseAPI.Database.SetTypes;
            SetTypes = ExtendedBitmap(setTypes.Count * IconLarge, IconLarge);
            for (var index = 0; index <= setTypes.Count - 1; ++index)
            {
                var x = index * IconLarge;
                var path = images.FirstOrDefault(i => i.FileName == $"{setTypes[index].ShortName}.png").Path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = unknown;
                }

                using var extendedBitmap = new ExtendedBitmap(path);
                var size = extendedBitmap.Size;
                var num1 = size.Height > IconLarge ? 1 : 0;
                size = extendedBitmap.Size;
                var num2 = size.Width > IconLarge ? 1 : 0;
                if ((num1 | num2) != 0)
                {
                    SetTypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, IconLarge, IconLarge);
                }
                else
                {
                    SetTypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
                }
            }
            await Task.CompletedTask;
        }

        private static async Task LoadEnhTypeImages(IReadOnlyCollection<ImageInfo> images, IEnumerable<ImageInfo> baseImages)
        {
            var unknown = baseImages.FirstOrDefault(i => i.FileName == "Unknown.png").Path;
            var values1 = Enum.GetValues(typeof(Enums.eType));
            var names1 = Enum.GetNames(typeof(Enums.eType));
            names1[3] = "HamiO";
            EnhTypes = ExtendedBitmap(values1.Length * IconLarge, IconLarge);
            for (var index = 0; index <= values1.Length - 1; ++index)
            {
                var x = index * IconLarge;
                var path = images.FirstOrDefault(i => i.FileName == $"{names1[index]}.png").Path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = unknown;
                }

                using var extendedBitmap = new ExtendedBitmap(path);
                var size = extendedBitmap.Size;
                var num1 = size.Height > IconLarge ? 1 : 0;
                size = extendedBitmap.Size;
                var num2 = size.Width > IconLarge ? 1 : 0;
                if ((num1 | num2) != 0)
                {
                    EnhTypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, IconLarge, IconLarge);
                }
                else
                {
                    EnhTypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
                }
            }

            var values2 = Enum.GetValues(typeof(Enums.eEnhGrade));
            var names2 = Enum.GetNames(typeof(Enums.eEnhGrade));
            EnhGrades = ExtendedBitmap(values2.Length * IconLarge, IconLarge);
            for (var index = 0; index <= values2.Length - 1; ++index)
            {
                var x = index * IconLarge;
                var path = images.FirstOrDefault(i => i.FileName == $"{names2[index]}.png").Path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = unknown;
                }

                using var extendedBitmap = new ExtendedBitmap(path);
                var size = extendedBitmap.Size;
                var num1 = size.Height > IconLarge ? 1 : 0;
                size = extendedBitmap.Size;
                var num2 = size.Width > IconLarge ? 1 : 0;
                if ((num1 | num2) != 0)
                {
                    EnhGrades.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, IconLarge, IconLarge);
                }
                else
                {
                    EnhGrades.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
                }
            }

            // var values3 = Enum.GetValues(typeof(Enums.eSubtype));
            //var names3 = Enum.GetNames(typeof(Enums.eSubtype));

            var specialEnhTypes = DatabaseAPI.Database.SpecialEnhancements;
            var specEnhNames = specialEnhTypes.Select(x => x.Name.Replace(" Origin", string.Empty)).ToArray();
            EnhSpecials = ExtendedBitmap(specialEnhTypes.Count * IconLarge, IconLarge);
            for (var index = 0; index <= specialEnhTypes.Count - 1; ++index)
            {
                var x = index * IconLarge;
                var path = images.FirstOrDefault(i => i.FileName == $"{specEnhNames[index]}.png").Path;
                if (string.IsNullOrWhiteSpace(path))
                {
                    path = unknown;
                }

                using var extendedBitmap = new ExtendedBitmap(path);
                var size = extendedBitmap.Size;
                var num1 = size.Height > IconLarge ? 1 : 0;
                size = extendedBitmap.Size;
                var num2 = size.Width > IconLarge ? 1 : 0;
                if ((num1 | num2) != 0)
                {
                    EnhSpecials.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, IconLarge, IconLarge);
                }
                else
                {
                    EnhSpecials.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
                }
            }
            await Task.CompletedTask;
        }

        private static async Task LoadBorderImages(IReadOnlyCollection<ImageInfo> images)
        {
            Borders = ExtendedBitmap(DatabaseAPI.Database.Origins.Count * IconLarge, 180);
            for (var index = 0; index <= DatabaseAPI.Database.Origins.Count - 1; ++index)
            {
                var x = index * IconLarge;
                for (var index2 = 0; index2 <= 5; ++index2)
                {
                    var path = images.FirstOrDefault(i => i.FileName == $"{DatabaseAPI.Database.Origins[index].Grades[index2]}.png").Path;
                    if (string.IsNullOrWhiteSpace(path))
                    {
                        continue;
                    }

                    using var extendedBitmap = new ExtendedBitmap(path);
                    if ((extendedBitmap.Size.Height > IconLarge) | (extendedBitmap.Size.Width > IconLarge))
                    {
                        Borders.Graphics.DrawImage(extendedBitmap.Bitmap, x, IconLarge * index2, IconLarge, IconLarge);
                    }
                    else
                    {
                        Borders.Graphics.DrawImage(extendedBitmap.Bitmap, x, IconLarge * index2);
                    }
                }
            }

            await Task.CompletedTask;
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

        public static string GetRecipeName()
        {
            return ImagePath("Overlay") + "\\Recipe.png";
        }

        public static string GetRecipeTransparentName()
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
            return Path.Combine(MidsContext.Config.DataPath, "Images\\Enhancements");
        }

        public static string GetOriginsPath()
        {
            return ImagePath() + "\\Origins\\";
        }

        public static void DrawFlippingEnhancement(ref Graphics iTarget, Rectangle iDest, float iSize, int iImageIndex, Origin.Grade iGrade)
        {
            var iDest1 = iDest;
            iDest1.Width = (int) (iDest1.Width * (double) iSize);
            iDest1.X += (iDest.Width - iDest1.Width) / 2;
            DrawEnhancementAt(ref iTarget, iDest1, iImageIndex, iGrade);
        }

        public static void DrawEnhancement(ref Graphics iTarget, int iImageIndex, Origin.Grade iGrade)
        {
            iTarget.PixelOffsetMode = PixelOffsetMode.HighQuality;
            iTarget.CompositingMode = CompositingMode.SourceOver;
            iTarget.CompositingQuality = CompositingQuality.HighQuality;
            iTarget.InterpolationMode = InterpolationMode.HighQualityBicubic;
            iTarget.SmoothingMode = SmoothingMode.HighQuality;
            iTarget.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            iTarget.PageUnit = GraphicsUnit.Pixel;
            iTarget.DrawImage(Borders.Bitmap, iTarget.ClipBounds, GetOverlayRectF(iGrade), GraphicsUnit.Pixel);
            iTarget.DrawImage(Enhancements[iImageIndex], iTarget.ClipBounds, new RectangleF(0.0f, 0.0f, 30f, 30f), GraphicsUnit.Pixel);
        }

        public static void DrawEnhancementAt(ref Graphics iTarget, Rectangle iDest, int iImageIndex, Origin.Grade iGrade, ImageAttributes imageAttributes)
        {
            if (iDest.Width > 30)
            {
                iDest.Width = 30;
            }

            if (iDest.Height > 30)
            {
                iDest.Height = 30;
            }

            if ((iImageIndex < 0) | (iImageIndex >= Enhancements.Length))
            {
                return;
            }

            var graphics = iTarget;
            var bitmap = Borders.Bitmap;
            var destRect = iDest;
            var overlayRect = GetOverlayRect(iGrade);
            var x = overlayRect.X;
            overlayRect = GetOverlayRect(iGrade);
            var y = overlayRect.Y;
            var imageAttr = imageAttributes;
            graphics.DrawImage(bitmap, destRect, x, y, 30, 30, GraphicsUnit.Pixel, imageAttr);
            iTarget.DrawImage(Enhancements[iImageIndex], iDest, 0, 0, 30, 30, GraphicsUnit.Pixel, imageAttributes);
        }

        public static void DrawEnhancementAt(ref Graphics iTarget, Rectangle iDest, int iImageIndex, Origin.Grade iGrade)
        {
            if (iDest.Width > 30)
            {
                iDest.Width = 30;
            }

            if (iDest.Height > 30)
            {
                iDest.Height = 30;
            }

            if (iImageIndex < 0 || iImageIndex >= Enhancements.Length)
            {
                return;
            }

            iTarget.PixelOffsetMode = PixelOffsetMode.HighQuality;
            iTarget.CompositingMode = CompositingMode.SourceOver;
            iTarget.CompositingQuality = CompositingQuality.HighQuality;
            iTarget.InterpolationMode = InterpolationMode.HighQualityBicubic;
            iTarget.SmoothingMode = SmoothingMode.HighQuality;
            iTarget.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            iTarget.PageUnit = GraphicsUnit.Pixel;
            iTarget.DrawImage(Borders.Bitmap, iDest, GetOverlayRect(iGrade), GraphicsUnit.Pixel);
            iTarget.DrawImage(Enhancements[iImageIndex], iDest, new Rectangle(0, 0, 30, 30), GraphicsUnit.Pixel);
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
            iTarget.DrawImage(Borders.Bitmap, iTarget.ClipBounds, GetOverlayRectF(Origin.Grade.SetO), GraphicsUnit.Pixel);
            iTarget.DrawImage(Sets.Bitmap, iTarget.ClipBounds, GetImageRectF(iImageIndex), GraphicsUnit.Pixel);
        }

        public static Rectangle GetOverlayRect(Origin.Grade iGrade)
        {
            if (iGrade == Origin.Grade.None)
            {
                iGrade = Origin.Grade.HO;
            }

            return new Rectangle(OriginIndex * 30, (int) iGrade * 30, 30, 30);
        }

        private static RectangleF GetOverlayRectF(Origin.Grade iGrade)
        {
            var overlayRect = GetOverlayRect(iGrade);
            return new RectangleF(overlayRect.X, overlayRect.Y, overlayRect.Width, overlayRect.Height);
        }

        public static Rectangle GetImageRect(int index)
        {
            return new Rectangle(index * 30, 0, 30, 30);
        }

        private static RectangleF GetImageRectF(int index)
        {
            var imageRect = GetImageRect(index);
            return new RectangleF(imageRect.X, imageRect.Y, imageRect.Width, imageRect.Height);
        }
    }
}