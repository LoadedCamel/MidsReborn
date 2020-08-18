using System;
using System.Drawing;

namespace Base.Display
{
    public static class PopUp
    {
        public static class Colors
        {
            public static Color Title = Color.FromArgb(216, 216, byte.MaxValue);
            public static Color Text = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue);
            public static Color Disabled = Color.FromArgb(192, 192, 192);
            public static Color Invention = Color.FromArgb(0, byte.MaxValue, byte.MaxValue);
            public static Color Effect = Color.FromArgb(0, byte.MaxValue, 128);
            public static Color Alert = Color.FromArgb(byte.MaxValue, 0, 0);
            public static Color UltraRare = Color.FromArgb(192, 96, 192);
            public static Color Rare = Color.FromArgb(byte.MaxValue, 128, 0);
            public static Color Uncommon = Color.FromArgb(byte.MaxValue, byte.MaxValue, 0);
            public static Color Common = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue);
        }

        public struct StringValue
        {
            public string Text;
            public Color tColor;
            public float tSize;
            public FontStyle tFormat;
            public int tIndent;
            public string TextColumn;
            public Color tColorColumn;

            public bool HasColumn => !string.IsNullOrEmpty(TextColumn);
        }

        public class Section
        {
            public StringValue[] Content;

            public void Add(string iText, Color iColor, float iSize = 1f, FontStyle iFormat = FontStyle.Bold,
                int iIndent = 0)
            {
                if (Content == null)
                    Content = new StringValue[0];
                Array.Resize(ref Content, Content.Length + 1);
                Content[Content.Length - 1].Text = iText;
                Content[Content.Length - 1].tColor = iColor;
                Content[Content.Length - 1].tSize = iSize;
                Content[Content.Length - 1].tFormat = iFormat;
                Content[Content.Length - 1].tIndent = iIndent;
                Content[Content.Length - 1].TextColumn = "";
                Content[Content.Length - 1].tColorColumn = iColor;
            }

            public void Add(
                string iText,
                Color iColor,
                string iColumnText,
                Color iColumnColor,
                float iSize = 1f,
                FontStyle iFormat = FontStyle.Bold,
                int iIndent = 0)
            {
                if (Content == null)
                    Content = new StringValue[0];
                Array.Resize(ref Content, Content.Length + 1);
                Content[Content.Length - 1].Text = iText;
                Content[Content.Length - 1].tColor = iColor;
                Content[Content.Length - 1].tSize = iSize;
                Content[Content.Length - 1].tFormat = iFormat;
                Content[Content.Length - 1].tIndent = iIndent;
                Content[Content.Length - 1].TextColumn = iColumnText;
                Content[Content.Length - 1].tColorColumn = iColumnColor;
            }
        }

        public struct PopupData
        {
            public Section[] Sections;
            private float _columnPosition;

            private bool _rightAlignColumn;


            public bool CustomSet { get; private set; }

            public float ColPos
            {
                get => _columnPosition;
                set
                {
                    _columnPosition = value;
                    CustomSet = true;
                }
            }

            public bool ColRight
            {
                get => _rightAlignColumn;
                set
                {
                    _rightAlignColumn = value;
                    CustomSet = true;
                }
            }

            public int Add(Section section = null)
            {
                if (Sections == null)
                    Sections = new Section[0];
                if (section == null)
                    section = new Section
                    {
                        Content = new StringValue[0]
                    };
                Array.Resize(ref Sections, Sections.Length + 1);
                Sections[Sections.Length - 1] = section;
                return Sections.Length - 1;
            }

            public void Init()
            {
                var index1 = Add();
                Sections[index1].Add("Popup Information", Colors.Title, 1.25f);
                Sections[index1]
                    .Add(
                        "This is just an example string. It should wrap around if it gets too long, and not cause too many issues.",
                        Colors.Text);
                Sections[index1]
                    .Add("This is a second string added as an additional content structure within the section.",
                        Colors.Disabled);
                var index2 = Add();
                Sections[index2].Add("Second Section", Colors.Title);
                Sections[index2].Add("Columns follow this item:", Colors.Text);
                Sections[index2].Add("Column 1", Colors.Text, "Column 2", Colors.Invention, 0.9f, FontStyle.Bold, 1);
                Sections[index2].Add("Column 1a", Colors.Text, "Column 2a", Colors.Invention, 0.9f, FontStyle.Bold, 1);
                Sections[index2].Add("Column 1b", Colors.Text, "Column 2b", Colors.Invention, 0.9f, FontStyle.Bold, 1);
                Sections[index2].Add("Page from the Malleus mundi", Colors.Text, "1", Colors.Invention, 0.9f,
                    FontStyle.Bold, 1);
                Sections[index2].Add("Extra long column list item 1234567890", Colors.Text, "1", Colors.Invention, 0.9f,
                    FontStyle.Bold, 1);
            }
        }
    }
}