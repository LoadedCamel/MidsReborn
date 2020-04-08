
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Base.Data_Classes;
using Base.Display;

public static class I9Gfx
{
    public const int IconLarge = 30;
    const int IconSmall = 16;

    public const string ImageExtension = ".png";
    const string FileOverlayClass = "Class.png";

    const string GfxPath = "Images\\";

    const string PathClass = "Classes\\";

    const string PathOverlay = "Overlay\\";

    const string PathEnh = "Enhancements\\";

    const string PathSetType = "Sets\\";

    const string PathOriginAT = "OriginAT\\";

    const string PathPowersets = "Powersets\\";

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

    public static void SetOrigin(string iOrigin)
    {
        OriginIndex = DatabaseAPI.GetOriginIDByName(iOrigin);
    }

    public static void LoadPowersetImages()
    {
        Powersets = new ExtendedBitmap(DatabaseAPI.Database.Powersets.Length * 16, 16);
        Powersets.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        Powersets.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
        Powersets.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        Powersets.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        Parallel.For(0, DatabaseAPI.Database.Powersets.Length-1, index =>
        {
            int x = index * 16;
            string str = GetPowersetsPath() + DatabaseAPI.Database.Powersets[index].ImageName;
            if (!File.Exists(str))
                str = ImagePath() + "Unknown.png";
            using (ExtendedBitmap extendedBitmap = new ExtendedBitmap(str))
            {
                lock (Powersets.Graphics)
                {
                    if (extendedBitmap.Size.Height > 16 | extendedBitmap.Size.Width > 16)
                        Powersets.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 16, 16);
                    else
                        Powersets.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
                }
            }
        });
    }

    public static void LoadOriginImages()
    {
        Origins = new ExtendedBitmap(DatabaseAPI.Database.Origins.Count * 16, 16);
        Origins.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        Origins.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
        Origins.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        Origins.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        Parallel.For(0, DatabaseAPI.Database.Origins.Count, index =>
        {
            int x = index * 16;
            using (ExtendedBitmap extendedBitmap = new ExtendedBitmap(GetOriginsPath() + DatabaseAPI.Database.Origins[index].Name + ".png"))
            {
                lock (Origins.Graphics)
                {
                    if (extendedBitmap.Size.Height > 16 | extendedBitmap.Size.Width > 16)
                        Origins.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 16, 16);
                    else
                        Origins.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
                }
            }
        });
    }

    public static void LoadArchetypeImages()
    {
        Archetypes = new ExtendedBitmap(DatabaseAPI.Database.Classes.Length * 16, 16);
        Archetypes.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        Archetypes.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
        Archetypes.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        Archetypes.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        Parallel.For(0, DatabaseAPI.Database.Classes.Length-1, index =>
        {
            int x = index * 16;
            string str = GetOriginsPath() + DatabaseAPI.Database.Classes[index].ClassName + ".png";
            if (!File.Exists(str))
                str = ImagePath() + "Unknown.png";
            using (ExtendedBitmap extendedBitmap = new ExtendedBitmap(str))
            {
                lock (Archetypes.Graphics)
                {
                    if (extendedBitmap.Size.Height > 16 | extendedBitmap.Size.Width > 16)
                        Archetypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 16, 16);
                    else
                        Archetypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
                }
            }
        });
    }

    public static Origin.Grade ToGfxGrade(Enums.eType iType)
    {
        Origin.Grade grade;
        switch (iType)
        {
            case Enums.eType.None:
                grade = Origin.Grade.None;
                break;
            case Enums.eType.Normal:
                grade = Origin.Grade.TrainingO;
                break;
            case Enums.eType.InventO:
                grade = Origin.Grade.IO;
                break;
            case Enums.eType.SpecialO:
                grade = Origin.Grade.HO;
                break;
            case Enums.eType.SetO:
                grade = Origin.Grade.SetO;
                break;
            default:
                grade = Origin.Grade.None;
                break;
        }
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
        return FileIO.AddSlash(Application.StartupPath) + "Images\\";
    }

    public static void LoadClasses()
    {
        Classes = new ExtendedBitmap(DatabaseAPI.Database.EnhancementClasses.Length * 30, 30);
        Classes.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        Classes.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
        Classes.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        Classes.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        using (ExtendedBitmap extendedBitmap1 = new ExtendedBitmap(ImagePath() + "Overlay\\Class.png"))
        {
            Parallel.For(0, DatabaseAPI.Database.EnhancementClasses.Length-1, index =>
            {
                int x = index * 30;
                using (ExtendedBitmap extendedBitmap2 = new ExtendedBitmap(ImagePath() + "Classes\\" + DatabaseAPI.Database.EnhancementClasses[index].ID + ".png"))
                {
                    lock (Classes.Graphics)
                    {
                        Classes.Graphics.DrawImageUnscaled(extendedBitmap1.Bitmap, x, 0);
                        if (extendedBitmap2.Size.Height > 30 | extendedBitmap2.Size.Width > 30)
                            Classes.Graphics.DrawImage(extendedBitmap2.Bitmap, x, 0, 30, 30);
                        else
                            Classes.Graphics.DrawImage(extendedBitmap2.Bitmap, x, 0);
                    }
                }
            });
        }
        GC.Collect();
    }

    public static void LoadEnhancements()
    {
        Enhancements = new Bitmap[DatabaseAPI.Database.Enhancements.Length];
        for (int index = 0; index <= DatabaseAPI.Database.Enhancements.Length - 1; ++index)
        {
            if (DatabaseAPI.Database.Enhancements[index].Image != string.Empty)
            {
                try
                {
                    Enhancements[index] = new Bitmap(GetEnhancementsPath() + DatabaseAPI.Database.Enhancements[index].Image);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Message: {ex.Message}\r\nTrace: {ex.StackTrace}");
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
        Sets.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        Sets.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
        Sets.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        Sets.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        for (int index = 0; index <= DatabaseAPI.Database.EnhancementSets.Count - 1; ++index)
        {
            if (!string.IsNullOrEmpty(DatabaseAPI.Database.EnhancementSets[index].Image))
            {
                int x = index * 30;
                using (ExtendedBitmap extendedBitmap = new ExtendedBitmap(GetEnhancementsPath() + DatabaseAPI.Database.EnhancementSets[index].Image))
                {
                    DatabaseAPI.Database.EnhancementSets[index].ImageIdx = index;
                    Size size = extendedBitmap.Size;
                    int num;
                    if (size.Height <= 30)
                    {
                        size = extendedBitmap.Size;
                        num = size.Width <= 30 ? 1 : 0;
                    }
                    else
                        num = 0;
                    if (num == 0)
                        Sets.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 30, 30);
                    else
                        Sets.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
                }
            }
            else
                goto label_16;
            label_13:
            if (index % 5 == 0)
            {
                Application.DoEvents();
            }
            continue;
        label_16:
            DatabaseAPI.Database.EnhancementSets[index].ImageIdx = -1;
            goto label_13;
        }
    }

    public static void LoadSetTypes()
    {
        Array values = Enum.GetValues(typeof(Enums.eSetType));
        string[] names = Enum.GetNames(typeof(Enums.eSetType));
        int length = values.Length;
        SetTypes = new ExtendedBitmap(length * 30, 30);
        SetTypes.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        SetTypes.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
        SetTypes.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        SetTypes.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        for (int index = 0; index <= length - 1; ++index)
        {
            int x = index * 30;
            using (ExtendedBitmap extendedBitmap = new ExtendedBitmap(ImagePath() + "Sets\\" + names[index] + ".png"))
            {
                Size size = extendedBitmap.Size;
                int num1 = size.Height > 30 ? 1 : 0;
                size = extendedBitmap.Size;
                int num2 = size.Width > 30 ? 1 : 0;
                if ((num1 | num2) != 0)
                    SetTypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 30, 30);
                else
                    SetTypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
            }
        }
    }

    public static void LoadEnhTypes()
    {
        Array values1 = Enum.GetValues(typeof(Enums.eType));
        string[] names1 = Enum.GetNames(typeof(Enums.eType));
        names1[3] = "HamiO";
        EnhTypes = new ExtendedBitmap(values1.Length * 30, 30);
        EnhTypes.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        EnhTypes.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
        EnhTypes.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        EnhTypes.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        for (int index = 0; index < values1.Length; ++index)
        {
            int x = index * 30;
            using (ExtendedBitmap extendedBitmap = new ExtendedBitmap(ImagePath() + "Sets\\" + names1[index] + ".png"))
            {
                Size size = extendedBitmap.Size;
                int num1 = size.Height > 30 ? 1 : 0;
                size = extendedBitmap.Size;
                int num2 = size.Width > 30 ? 1 : 0;
                if ((num1 | num2) != 0)
                    EnhTypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 30, 30);
                else
                    EnhTypes.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
            }
        }
        Array values2 = Enum.GetValues(typeof(Enums.eEnhGrade));
        string[] names2 = Enum.GetNames(typeof(Enums.eEnhGrade));
        EnhGrades = new ExtendedBitmap(values2.Length * 30, 30);
        EnhGrades.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        EnhGrades.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
        EnhGrades.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        EnhGrades.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        for (int index = 0; index < values2.Length; ++index)
        {
            int x = index * 30;
            using (ExtendedBitmap extendedBitmap = new ExtendedBitmap(ImagePath() + "Sets\\" + names2[index] + ".png"))
            {
                Size size = extendedBitmap.Size;
                int num1 = size.Height > 30 ? 1 : 0;
                size = extendedBitmap.Size;
                int num2 = size.Width > 30 ? 1 : 0;
                if ((num1 | num2) != 0)
                    EnhGrades.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 30, 30);
                else
                    EnhGrades.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
            }
        }
        Array values3 = Enum.GetValues(typeof(Enums.eSubtype));
        string[] names3 = Enum.GetNames(typeof(Enums.eSubtype));
        EnhSpecials = new ExtendedBitmap(values3.Length * 30, 30);
        EnhSpecials.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        EnhSpecials.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
        EnhSpecials.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        EnhSpecials.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        for (int index = 0; index < values3.Length; ++index)
        {
            int x = index * 30;
            using (ExtendedBitmap extendedBitmap = new ExtendedBitmap(ImagePath() + "Sets\\" + names3[index] + ".png"))
            {
                Size size = extendedBitmap.Size;
                int num1 = size.Height > 30 ? 1 : 0;
                size = extendedBitmap.Size;
                int num2 = size.Width > 30 ? 1 : 0;
                if ((num1 | num2) != 0)
                    EnhSpecials.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0, 30, 30);
                else
                    EnhSpecials.Graphics.DrawImage(extendedBitmap.Bitmap, x, 0);
            }
        }
    }

    public static void LoadBorders()
    {
        Borders = new ExtendedBitmap(DatabaseAPI.Database.Origins.Count * 30, 180);
        Borders.Graphics.CompositingQuality = CompositingQuality.HighQuality;
        Borders.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
        Borders.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        Borders.Graphics.SmoothingMode = SmoothingMode.HighQuality;
        for (int index1 = 0; index1 <= DatabaseAPI.Database.Origins.Count - 1; ++index1)
        {
            int x = index1 * 30;
            for (int index2 = 0; index2 <= 5; ++index2)
            {
                using (ExtendedBitmap extendedBitmap = new ExtendedBitmap(ImagePath() + "Overlay\\" + DatabaseAPI.Database.Origins[index1].Grades[index2] + ".png"))
                {
                    if (extendedBitmap.Size.Height > 30 | extendedBitmap.Size.Width > 30)
                        Borders.Graphics.DrawImage(extendedBitmap.Bitmap, x, 30 * index2, 30, 30);
                    else
                        Borders.Graphics.DrawImage(extendedBitmap.Bitmap, x, 30 * index2);
                }
            }
        }
    }

    public static string GetRecipeName()
    {
        return ImagePath() + "Overlay\\Recipe.png";
    }

    public static string GetPowersetsPath()
    {
        return ImagePath() + "Powersets\\";
    }

    public static string GetEnhancementsPath()
    {
        return ImagePath() + "Enhancements\\";
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
        Rectangle iDest1 = iDest;
        iDest1.Width = (int)(iDest1.Width * (double)iSize);
        iDest1.X += (iDest.Width - iDest1.Width) / 2;
        DrawEnhancementAt(ref iTarget, iDest1, iImageIndex, iGrade);
    }

    public static void DrawEnhancement(ref Graphics iTarget, int iImageIndex, Origin.Grade iGrade)
    {
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
        if (iImageIndex < 0 | iImageIndex >= Enhancements.Length)
            return;
        Graphics graphics = iTarget;
        Bitmap bitmap = Borders.Bitmap;
        Rectangle destRect = iDest;
        Rectangle overlayRect = GetOverlayRect(iGrade);
        int x = overlayRect.X;
        overlayRect = GetOverlayRect(iGrade);
        int y = overlayRect.Y;
        ImageAttributes imageAttr = imageAttributes;
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
        iTarget.DrawImage(Borders.Bitmap, iDest, GetOverlayRect(iGrade), GraphicsUnit.Pixel);
        iTarget.DrawImage(Enhancements[iImageIndex], iDest, new Rectangle(0, 0, 30, 30), GraphicsUnit.Pixel);
    }

    public static void DrawEnhancementSet(ref Graphics iTarget, int iImageIndex)
    {
        iTarget.DrawImage(Borders.Bitmap, iTarget.ClipBounds, GetOverlayRectF(Origin.Grade.SetO), GraphicsUnit.Pixel);
        iTarget.DrawImage(Sets.Bitmap, iTarget.ClipBounds, GetImageRectF(iImageIndex), GraphicsUnit.Pixel);
    }

    public static Rectangle GetOverlayRect(Origin.Grade iGrade)
    {
        if (iGrade == Origin.Grade.None)
            iGrade = Origin.Grade.HO;
        return new Rectangle(OriginIndex * 30, (int)iGrade * 30, 30, 30);
    }

    static RectangleF GetOverlayRectF(Origin.Grade iGrade)

    {
        Rectangle overlayRect = GetOverlayRect(iGrade);
        return new RectangleF(overlayRect.X, overlayRect.Y, overlayRect.Width, overlayRect.Height);
    }

    public static Rectangle GetImageRect(int index)
    {
        return new Rectangle(index * 30, 0, 30, 30);
    }

    static RectangleF GetImageRectF(int index)

    {
        Rectangle imageRect = GetImageRect(index);
        return new RectangleF(imageRect.X, imageRect.Y, imageRect.Width, imageRect.Height);
    }
}
