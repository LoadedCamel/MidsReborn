using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;

namespace Mids_Reborn.Core.Utils
{
    internal static class Fonts
    {
        /// <summary>
        /// Available Fonts are
        /// DejaVu Sans Mono
        /// Noto Sans
        /// Noto Sans Black
        /// Noto Sans ExtraBold
        /// Noto Sans ExtraLight
        /// Noto Sans Light
        /// Noto Sans Medium
        /// Noto Sans SemiBold
        /// Noto Sans Thin
        /// </summary>

        private static PrivateFontCollection? _fontCollection;
        private static List<FontFamily>? _fontFamilies;
        
        internal static void BuildFontCollection()
        {
            if (_fontCollection != null) return;
            _fontCollection = new PrivateFontCollection();
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "DejaVuSansMono.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "DejaVuSansMono-Bold.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "DejaVuSansMono-BoldOblique.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "DejaVuSansMono-Oblique.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-Black.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-BlackItalic.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-Bold.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-BoldItalic.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-ExtraBold.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-ExtraBoldItalic.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-ExtraLight.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-ExtraLightItalic.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-Italic.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-Light.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-LightItalic.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-Medium.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-MediumItalic.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-Regular.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-SemiBold.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-SemiBoldItalic.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-Thin.ttf"));
            _fontCollection.AddFontFile(Path.Combine(AppContext.BaseDirectory, "Fonts", "NotoSans-ThinItalic.ttf"));
            _fontFamilies = _fontCollection.Families.ToList();
        }

        public static FontFamily Family(string familyName)
        {
            if (_fontCollection == null) throw new NullReferenceException("Font collection has not been initialized");
            if (_fontFamilies == null) throw new NullReferenceException("Font collection has not been initialized");
            return _fontFamilies.First(x => x.Name.Equals(familyName));
        }
    }
}
