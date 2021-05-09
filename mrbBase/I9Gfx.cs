using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Display;

namespace mrbBase
{
    public static class I9Gfx
    {
        public const int IconLarge = 30;
        private const int IconSmall = 16;

        public const string ImageExtension = ".png";
        private const string FileOverlayClass = "Class.png";

        private const string GfxPath = "Images\\";

        private const string PathClass = "Classes\\";

        private const string PathOverlay = "Overlay\\";

        private const string PathEnh = "Enhancements\\";

        private const string PathSetType = "Sets\\";

        private const string PathOriginAT = "OriginAT\\";

        private const string PathPowersets = "Powersets\\";

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

        public static void SetOrigin(string iOrigin)
        {
            OriginIndex = DatabaseAPI.GetOriginIDByName(iOrigin);
        }

        public static void LoadPowersetImages()
        {
            Powersets = new ExtendedBitmap(DatabaseAPI.Database.Powersets.Length * 16, 16);
            Powersets.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Powersets.Graphics.CompositingMode = CompositingMode.SourceOver;
            Powersets.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Powersets.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Powersets.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Powersets.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Powersets.Graphics.PageUnit = GraphicsUnit.Pixel;
            for (var index = 0; index <= DatabaseAPI.Database.Powersets.Length - 1; ++index)
            {
                var x = index * 16;
                var str = GetPowersetsPath() + DatabaseAPI.Database.Powersets[index].ImageName;
                if (!File.Exists(str))
                    str = ImagePath() + "Unknown.png";
                using var extendedBitmap = new ExtendedBitmap(str);
                if ((extendedBitmap.Size.Height > 16) | (extendedBitmap.Size.Width > 16))
                    Powersets.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 16, 16);
                else
                    Powersets.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
            }

            UnknownPowerset = new ExtendedBitmap($"{ImagePath()}Unknown.png");
        }

        public static void LoadOriginImages()
        {
            Origins = new ExtendedBitmap(DatabaseAPI.Database.Origins.Count * 16, 16);
            Origins.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Origins.Graphics.CompositingMode = CompositingMode.SourceOver;
            Origins.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Origins.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Origins.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Origins.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Origins.Graphics.PageUnit = GraphicsUnit.Pixel;
            for (var index = 0; index <= DatabaseAPI.Database.Origins.Count - 1; ++index)
            {
                var x = index * 16;
                using var extendedBitmap = new ExtendedBitmap(GetOriginsPath() + DatabaseAPI.Database.Origins[index].Name + ".png");
                if ((extendedBitmap.Size.Height > 16) | (extendedBitmap.Size.Width > 16))
                    Origins.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 16, 16);
                else
                    Origins.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
            }
        }

        public static void LoadArchetypeImages()
        {
            Archetypes = new ExtendedBitmap(DatabaseAPI.Database.Classes.Length * 16, 16);
            Archetypes.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Archetypes.Graphics.CompositingMode = CompositingMode.SourceOver;
            Archetypes.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Archetypes.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Archetypes.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Archetypes.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Archetypes.Graphics.PageUnit = GraphicsUnit.Pixel;
            for (var index = 0; index <= DatabaseAPI.Database.Classes.Length - 1; ++index)
            {
                var x = index * 16;
                var str = GetOriginsPath() + DatabaseAPI.Database.Classes[index].ClassName + ".png";
                if (!File.Exists(str))
                    str = ImagePath() + "Unknown.png";
                using var extendedBitmap = new ExtendedBitmap(str);
                if ((extendedBitmap.Size.Height > 16) | (extendedBitmap.Size.Width > 16))
                    Archetypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 16, 16);
                else
                    Archetypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
            }

            UnknownArchetype = new ExtendedBitmap($"{ImagePath()}Unknown.png");
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
                imgFile = $"{ImagePath()}Unknown.png";
            }
            else
            {
                imgFile = $"{ImagePath()}OriginAT\\{atString}.png";
                if (!File.Exists(imgFile))
                {
                    imgFile = $"{ImagePath()}Unknown.png";
                }
            }

            return Image.FromFile(imgFile);
        }

        public static Image GetArchetypeImage(Archetype atClass)
        {
            var imgFile = $"{ImagePath()}OriginAT\\{atClass.ClassName}.png";
            if (!File.Exists(imgFile))
            {
                imgFile = $"{ImagePath()}Unknown.png";
            }

            return Image.FromFile(imgFile);
        }

        public static Image GetPowersetImage(IPower power)
        {
            var imgString = power.GetPowerSet().ImageName;
            var imgFile = $"{ImagePath()}Powersets\\{imgString}";
            if (!File.Exists(imgFile))
            {
                imgFile = $"{ImagePath()}Unknown.png";
            }

            return Image.FromFile(imgFile);
        }

        public static Image GetPowersetImage(IPowerset powerset)
        {
            var imgString = powerset.ImageName;
            var imgFile = $"{ImagePath()}Powersets\\{imgString}";
            if (!File.Exists(imgFile))
            {
                imgFile = $"{ImagePath()}Unknown.png";
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

        public static string ImagePath()
        {
            //Debug.WriteLine($"{Directory.GetCurrentDirectory()}\\Images\\");
            var asmLOC = Assembly.GetExecutingAssembly().Location;
            var dirLOC = Directory.GetParent(asmLOC);
            return $"{dirLOC}\\Images\\";
        }

        public static void LoadClasses()
        {
            Classes = new ExtendedBitmap(DatabaseAPI.Database.EnhancementClasses.Length * 30, 30);
            Classes.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Classes.Graphics.CompositingMode = CompositingMode.SourceOver;
            Classes.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Classes.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Classes.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Classes.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Classes.Graphics.PageUnit = GraphicsUnit.Pixel;
            using (var extendedBitmap1 = new ExtendedBitmap(ImagePath() + "Overlay\\Class.png"))
            {
                for (var index = 0; index <= DatabaseAPI.Database.EnhancementClasses.Length - 1; ++index)
                {
                    var x = index * 30;
                    using var extendedBitmap2 = new ExtendedBitmap(ImagePath() + "Classes\\" +
                                                                   DatabaseAPI.Database.EnhancementClasses[index].ID +
                                                                   ".png");
                    Classes.Graphics.DrawImageUnscaled(extendedBitmap1.Bitmap, x, 0);
                    if ((extendedBitmap2.Size.Height > 30) | (extendedBitmap2.Size.Width > 30))
                        Classes.Graphics.DrawImage(extendedBitmap2.Bitmap, x, 0, 30, 30);
                    else
                        Classes.Graphics.DrawImage(extendedBitmap2.Bitmap, x, 0);
                }
            }

            GC.Collect();
        }

        public static void LoadEnhancements()
        {
            Enhancements = new Bitmap[DatabaseAPI.Database.Enhancements.Length];
            for (var index = 0; index <= DatabaseAPI.Database.Enhancements.Length - 1; ++index)
            {
                if (!string.IsNullOrWhiteSpace(DatabaseAPI.Database.Enhancements[index].Image))
                {
                    try
                    {
                        //Debug.WriteLine($"{GetEnhancementsPath()}{DatabaseAPI.Database.Enhancements[index].Image}");
                        Enhancements[index] = new Bitmap($"{GetEnhancementsPath()}{DatabaseAPI.Database.Enhancements[index].Image}");
                    }
                    catch (Exception)
                    {
                        //MessageBox.Show($"Message: {ex.Message} \r\n\r\n Trace: {ex.StackTrace}");
                        Enhancements[index] = new Bitmap(30, 30, PixelFormat.Format32bppArgb);
                    }

                    DatabaseAPI.Database.Enhancements[index].ImageIdx = index;
                }
                else
                {
                    Enhancements[index] = new Bitmap(30, 30, PixelFormat.Format32bppArgb);
                    DatabaseAPI.Database.Enhancements[index].ImageIdx = -1;
                }

                if (index % 5 == 0)
                    Application.DoEvents();
            }
        }

        public static void LoadSets()
        {
            Sets = new ExtendedBitmap(DatabaseAPI.Database.EnhancementSets.Count * 30, 30);
            Sets.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Sets.Graphics.CompositingMode = CompositingMode.SourceOver;
            Sets.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Sets.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Sets.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Sets.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Sets.Graphics.PageUnit = GraphicsUnit.Pixel;
            for (var index = 0; index <= DatabaseAPI.Database.EnhancementSets.Count - 1; ++index)
            {
                if (!string.IsNullOrEmpty(DatabaseAPI.Database.EnhancementSets[index].Image))
                {
                    var x = index * 30;
                    using var extendedBitmap = new ExtendedBitmap(GetEnhancementsPath() + DatabaseAPI.Database.EnhancementSets[index].Image);
                    DatabaseAPI.Database.EnhancementSets[index].ImageIdx = index;
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
                        Sets.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 30, 30);
                    else
                        Sets.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
                }
                else
                {
                    goto label_16;
                }

                label_13:
                if (index % 5 == 0) Application.DoEvents();
                continue;
                label_16:
                DatabaseAPI.Database.EnhancementSets[index].ImageIdx = -1;
                goto label_13;
            }
        }

        public static void LoadSetTypes()
        {
            var values = Enum.GetValues(typeof(Enums.eSetType));
            var names = Enum.GetNames(typeof(Enums.eSetType));
            var length = values.Length;
            SetTypes = new ExtendedBitmap(length * 30, 30);
            SetTypes.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            SetTypes.Graphics.CompositingMode = CompositingMode.SourceOver;
            SetTypes.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            SetTypes.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            SetTypes.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            SetTypes.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            SetTypes.Graphics.PageUnit = GraphicsUnit.Pixel;
            for (var index = 0; index <= length - 1; ++index)
            {
                var x = index * 30;
                using var extendedBitmap = new ExtendedBitmap(ImagePath() + "Sets\\" + names[index] + ".png");
                var size = extendedBitmap.Size;
                var num1 = size.Height > 30 ? 1 : 0;
                size = extendedBitmap.Size;
                var num2 = size.Width > 30 ? 1 : 0;
                if ((num1 | num2) != 0)
                    SetTypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 30, 30);
                else
                    SetTypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
            }
        }

        public static void LoadEnhTypes()
        {
            var values1 = Enum.GetValues(typeof(Enums.eType));
            var names1 = Enum.GetNames(typeof(Enums.eType));
            names1[3] = "HamiO";
            EnhTypes = new ExtendedBitmap(values1.Length * 30, 30);
            EnhTypes.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            EnhTypes.Graphics.CompositingMode = CompositingMode.SourceOver;
            EnhTypes.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            EnhTypes.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            EnhTypes.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            EnhTypes.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            EnhTypes.Graphics.PageUnit = GraphicsUnit.Pixel;
            for (var index = 0; index < values1.Length; ++index)
            {
                var x = index * 30;
                using var extendedBitmap = new ExtendedBitmap(ImagePath() + "Sets\\" + names1[index] + ".png");
                var size = extendedBitmap.Size;
                var num1 = size.Height > 30 ? 1 : 0;
                size = extendedBitmap.Size;
                var num2 = size.Width > 30 ? 1 : 0;
                if ((num1 | num2) != 0)
                    EnhTypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 30, 30);
                else
                    EnhTypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
            }

            var values2 = Enum.GetValues(typeof(Enums.eEnhGrade));
            var names2 = Enum.GetNames(typeof(Enums.eEnhGrade));
            EnhGrades = new ExtendedBitmap(values2.Length * 30, 30);
            EnhGrades.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            EnhGrades.Graphics.CompositingMode = CompositingMode.SourceOver;
            EnhGrades.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            EnhGrades.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            EnhGrades.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            EnhGrades.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            EnhGrades.Graphics.PageUnit = GraphicsUnit.Pixel;
            for (var index = 0; index < values2.Length; ++index)
            {
                var x = index * 30;
                using var extendedBitmap = new ExtendedBitmap(ImagePath() + "Sets\\" + names2[index] + ".png");
                var size = extendedBitmap.Size;
                var num1 = size.Height > 30 ? 1 : 0;
                size = extendedBitmap.Size;
                var num2 = size.Width > 30 ? 1 : 0;
                if ((num1 | num2) != 0)
                    EnhGrades.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 30, 30);
                else
                    EnhGrades.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
            }

            var values3 = Enum.GetValues(typeof(Enums.eSubtype));
            var names3 = Enum.GetNames(typeof(Enums.eSubtype));
            EnhSpecials = new ExtendedBitmap(values3.Length * 30, 30);
            EnhSpecials.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            EnhSpecials.Graphics.CompositingMode = CompositingMode.SourceOver;
            EnhSpecials.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            EnhSpecials.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            EnhSpecials.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            EnhSpecials.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            EnhSpecials.Graphics.PageUnit = GraphicsUnit.Pixel;
            for (var index = 0; index < values3.Length; ++index)
            {
                var x = index * 30;
                using var extendedBitmap = new ExtendedBitmap(ImagePath() + "Sets\\" + names3[index] + ".png");
                var size = extendedBitmap.Size;
                var num1 = size.Height > 30 ? 1 : 0;
                size = extendedBitmap.Size;
                var num2 = size.Width > 30 ? 1 : 0;
                if ((num1 | num2) != 0)
                    EnhSpecials.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 30, 30);
                else
                    EnhSpecials.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
            }
        }

        public static void LoadBorders()
        {
            Borders = new ExtendedBitmap(DatabaseAPI.Database.Origins.Count * 30, 180);
            Borders.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            Borders.Graphics.CompositingMode = CompositingMode.SourceOver;
            Borders.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Borders.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            Borders.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            Borders.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            Borders.Graphics.PageUnit = GraphicsUnit.Pixel;
            for (var index1 = 0; index1 <= DatabaseAPI.Database.Origins.Count - 1; ++index1)
            {
                var x = index1 * 30;
                for (var index2 = 0; index2 <= 5; ++index2)
                {
                    using var extendedBitmap = new ExtendedBitmap(ImagePath() + "Overlay\\" + DatabaseAPI.Database.Origins[index1].Grades[index2] + ".png");
                    if ((extendedBitmap.Size.Height > 30) | (extendedBitmap.Size.Width > 30))
                        Borders.Graphics.DrawImage(extendedBitmap.Bitmap, x, 30 * index2, 30, 30);
                    else
                        Borders.Graphics.DrawImage(extendedBitmap.Bitmap, x, 30 * index2);
                }
            }
        }

        public static string GetRecipeName()
        {
            return ImagePath() + "Overlay\\Recipe.png";
        }

        public static string GetRecipeTransparentName()
        {
            return ImagePath() + "Overlay\\Recipe2.png";
        }

        public static string GetPowersetsPath()
        {
            return ImagePath() + "Powersets\\";
        }

        public static string GetEnhancementsPath()
        {
            return $"{ImagePath()}Enhancements\\";
        }

        public static string GetOriginsPath()
        {
            return ImagePath() + "OriginAT\\";
        }

        public static void DrawFlippingEnhancement(
            ref Graphics iTarget,
            Rectangle iDest,
            float iSize,
            int iImageIndex,
            Origin.Grade iGrade)
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

        public static void DrawEnhancementAt(
            ref Graphics iTarget,
            Rectangle iDest,
            int iImageIndex,
            Origin.Grade iGrade,
            ImageAttributes imageAttributes)
        {
            if (iDest.Width > 30)
                iDest.Width = 30;
            if (iDest.Height > 30)
                iDest.Height = 30;
            if ((iImageIndex < 0) | (iImageIndex >= Enhancements.Length))
                return;
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

        public static void DrawEnhancementAt(
            ref Graphics iTarget,
            Rectangle iDest,
            int iImageIndex,
            Origin.Grade iGrade)
        {
            if (iDest.Width > 30)
                iDest.Width = 30;
            if (iDest.Height > 30)
                iDest.Height = 30;
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
                iGrade = Origin.Grade.HO;
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