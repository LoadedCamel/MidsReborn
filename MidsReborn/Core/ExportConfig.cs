using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Mids_Reborn.Core
{
    public class ExportConfig
    {
        public enum Element
        {
            Title,
            Heading,
            Level,
            Power,
            Slots,
            IO,
            SetO,
            HO
        }

        public enum WhiteSpace
        {
            Space,
            Tab
        }

        public ColorScheme[] ColorSchemes = new ColorScheme[0];
        public FormatCodes[] FormatCode = new FormatCodes[0];

        public ExportConfig()
        {
            ResetColorsToDefaults();
        }

        public void AddScheme()
        {
            Array.Resize(ref ColorSchemes, ColorSchemes.Length + 1);
            ColorSchemes[ColorSchemes.Length - 1].SetDefault();
            ColorSchemes[ColorSchemes.Length - 1].SchemeName = "New Colors";
        }

        public void AddCodes()
        {
            Array.Resize(ref FormatCode, FormatCode.Length + 1);
            FormatCode[FormatCode.Length - 1].SetDefault();
            FormatCode[FormatCode.Length - 1].Name = "New Format";
            FormatCode[FormatCode.Length - 1].Notes = string.Empty;
        }

        public void RemoveScheme(int index)
        {
            if (!((index > -1) & (index < ColorSchemes.Length)))
                return;
            var colorSchemeArray = new ColorScheme[ColorSchemes.Length - 1];
            var index1 = 0;
            for (var index2 = 0; index2 < ColorSchemes.Length; ++index2)
            {
                if (index2 == index)
                    continue;
                colorSchemeArray[index1].Assign(ColorSchemes[index2]);
                ++index1;
            }

            ColorSchemes = new ColorScheme[colorSchemeArray.Length];
            for (var index2 = 0; index2 < colorSchemeArray.Length; ++index2)
                ColorSchemes[index2].Assign(colorSchemeArray[index2]);
        }

        public void RemoveCodes(int index)
        {
            if (!((index > -1) & (index < FormatCode.Length)))
                return;
            var formatCodesArray = new FormatCodes[FormatCode.Length - 1];
            var index1 = 0;
            for (var index2 = 0; index2 < FormatCode.Length; ++index2)
            {
                if (index2 == index)
                    continue;
                formatCodesArray[index1].Assign(FormatCode[index2]);
                ++index1;
            }

            FormatCode = new FormatCodes[formatCodesArray.Length];
            for (var index2 = 0; index2 < formatCodesArray.Length; ++index2)
                FormatCode[index2].Assign(formatCodesArray[index2]);
        }

        private static bool GrabString(out string dest, ref StreamReader reader)

        {
            dest = reader.ReadLine();
            return (dest == "#CODE#") | (dest == "#END#");
        }

        public void ResetColorsToDefaults()
        {
            ColorSchemes = new ColorScheme[10];
            ColorSchemes[0].SchemeName = "Navy";
            ColorSchemes[0].Title = Color.FromArgb(0, 0, 205);
            ColorSchemes[0].Heading = Color.FromArgb(0, 0, 128);
            ColorSchemes[0].Level = Color.FromArgb(72, 61, 139);
            ColorSchemes[0].Power = Color.FromArgb(0, 0, 0);
            ColorSchemes[0].Slots = Color.FromArgb(72, 61, 139);
            ColorSchemes[0].IOColor = Color.FromArgb(0, 0, 205);
            ColorSchemes[0].SetColor = Color.FromArgb(0, 0, 205);
            ColorSchemes[0].HOColor = Color.FromArgb(72, 61, 139);
            ColorSchemes[1].SchemeName = "Light Blue (US)";
            ColorSchemes[1].Title = Color.FromArgb(177, 201, 245);
            ColorSchemes[1].Heading = Color.FromArgb(72, 154, byte.MaxValue);
            ColorSchemes[1].Level = Color.FromArgb(79, 167, byte.MaxValue);
            ColorSchemes[1].Power = Color.FromArgb(179, 202, 247);
            ColorSchemes[1].Slots = Color.FromArgb(94, 174, byte.MaxValue);
            ColorSchemes[1].IOColor = Color.FromArgb(139, 175, 241);
            ColorSchemes[1].SetColor = Color.FromArgb(122, 164, 239);
            ColorSchemes[1].HOColor = Color.FromArgb(74, 165, byte.MaxValue);
            ColorSchemes[2].SchemeName = "Purple";
            ColorSchemes[2].Title = Color.FromArgb(128, 0, 128);
            ColorSchemes[2].Heading = Color.FromArgb(148, 0, 211);
            ColorSchemes[2].Level = Color.FromArgb(186, 85, 211);
            ColorSchemes[2].Power = Color.FromArgb(0, 0, 0);
            ColorSchemes[2].Slots = Color.FromArgb(148, 0, 211);
            ColorSchemes[2].IOColor = Color.FromArgb(128, 0, 128);
            ColorSchemes[2].SetColor = Color.FromArgb(128, 0, 128);
            ColorSchemes[2].HOColor = Color.FromArgb(148, 0, 211);
            ColorSchemes[3].SchemeName = "Purple (US)";
            ColorSchemes[3].Title = Color.FromArgb(207, 179, byte.MaxValue);
            ColorSchemes[3].Heading = Color.FromArgb(188, 155, byte.MaxValue);
            ColorSchemes[3].Level = Color.FromArgb(175, 138, 253);
            ColorSchemes[3].Power = Color.FromArgb(227, 218, 254);
            ColorSchemes[3].Slots = Color.FromArgb(194, 180, 252);
            ColorSchemes[3].IOColor = Color.FromArgb(184, 160, 252);
            ColorSchemes[3].SetColor = Color.FromArgb(179, 154, 252);
            ColorSchemes[3].HOColor = Color.FromArgb(205, 193, 253);
            ColorSchemes[4].SchemeName = "Orange";
            ColorSchemes[4].Title = Color.FromArgb(byte.MaxValue, 140, 0);
            ColorSchemes[4].Heading = Color.FromArgb(byte.MaxValue, 165, 0);
            ColorSchemes[4].Level = Color.FromArgb(byte.MaxValue, 69, 0);
            ColorSchemes[4].Power = Color.FromArgb(0, 0, 0);
            ColorSchemes[4].Slots = Color.FromArgb(184, 134, 11);
            ColorSchemes[4].IOColor = Color.FromArgb(byte.MaxValue, 140, 0);
            ColorSchemes[4].SetColor = Color.FromArgb(byte.MaxValue, 140, 0);
            ColorSchemes[4].HOColor = Color.FromArgb(184, 134, 11);
            ColorSchemes[5].SchemeName = "Olive Drab";
            ColorSchemes[5].Title = Color.FromArgb(85, 107, 47);
            ColorSchemes[5].Heading = Color.FromArgb(0, 128, 0);
            ColorSchemes[5].Level = Color.FromArgb(107, 142, 35);
            ColorSchemes[5].Power = Color.FromArgb(0, 0, 0);
            ColorSchemes[5].Slots = Color.FromArgb(107, 142, 35);
            ColorSchemes[5].IOColor = Color.FromArgb(85, 107, 47);
            ColorSchemes[5].SetColor = Color.FromArgb(85, 107, 47);
            ColorSchemes[5].HOColor = Color.FromArgb(107, 142, 35);
            ColorSchemes[6].SchemeName = "Reds";
            ColorSchemes[6].Title = Color.FromArgb(128, 0, 0);
            ColorSchemes[6].Heading = Color.FromArgb(168, 0, 0);
            ColorSchemes[6].Level = Color.FromArgb(132, 63, 60);
            ColorSchemes[6].Power = Color.FromArgb(0, 0, 0);
            ColorSchemes[6].Slots = Color.FromArgb(111, 0, 0);
            ColorSchemes[6].IOColor = Color.FromArgb(155, 0, 0);
            ColorSchemes[6].SetColor = Color.FromArgb(130, 0, 0);
            ColorSchemes[6].HOColor = Color.FromArgb(147, 22, 0);
            ColorSchemes[7].SchemeName = "Reds (US)";
            ColorSchemes[7].Title = Color.FromArgb(byte.MaxValue, 106, 106);
            ColorSchemes[7].Heading = Color.FromArgb(byte.MaxValue, 0, 0);
            ColorSchemes[7].Level = Color.FromArgb(byte.MaxValue, 108, 108);
            ColorSchemes[7].Power = Color.FromArgb(byte.MaxValue, 183, 183);
            ColorSchemes[7].Slots = Color.FromArgb(byte.MaxValue, 128, 128);
            ColorSchemes[7].IOColor = Color.FromArgb(byte.MaxValue, 102, 102);
            ColorSchemes[7].SetColor = Color.FromArgb(byte.MaxValue, 74, 74);
            ColorSchemes[7].HOColor = Color.FromArgb(byte.MaxValue, 151, 151);
            ColorSchemes[8].SchemeName = "Fruit Salad (US)";
            ColorSchemes[8].Title = Color.FromArgb(byte.MaxValue, 165, 0);
            ColorSchemes[8].Heading = Color.FromArgb(30, 144, byte.MaxValue);
            ColorSchemes[8].Level = Color.FromArgb(50, 205, 50);
            ColorSchemes[8].Power = Color.FromArgb(30, 144, byte.MaxValue);
            ColorSchemes[8].Slots = Color.FromArgb(byte.MaxValue, 215, 0);
            ColorSchemes[8].IOColor = Color.FromArgb(byte.MaxValue, 192, 0);
            ColorSchemes[8].SetColor = Color.FromArgb(byte.MaxValue, 230, 0);
            ColorSchemes[8].HOColor = Color.FromArgb(byte.MaxValue, 195, 75);
            ColorSchemes[9].SchemeName = "Pink (US)";
            ColorSchemes[9].Title = Color.FromArgb(byte.MaxValue, 128, 192);
            ColorSchemes[9].Heading = Color.FromArgb(byte.MaxValue, 128, byte.MaxValue);
            ColorSchemes[9].Level = Color.FromArgb(byte.MaxValue, 128, byte.MaxValue);
            ColorSchemes[9].Power = Color.FromArgb(byte.MaxValue, 204, 230);
            ColorSchemes[9].Slots = Color.FromArgb(byte.MaxValue, 174, byte.MaxValue);
            ColorSchemes[9].IOColor = Color.FromArgb(233, 174, byte.MaxValue);
            ColorSchemes[9].SetColor = Color.FromArgb(byte.MaxValue, 174, 213);
            ColorSchemes[9].HOColor = Color.FromArgb(223, 174, byte.MaxValue);
        }

        public void ResetCodesToDefaults()
        {
            FormatCode = new FormatCodes[0];
            AddCodes();
            FormatCode[FormatCode.Length - 1].Name = "No Codes";
            FormatCode[FormatCode.Length - 1].Notes = "Unformatted plain text";
            FormatCode[FormatCode.Length - 1].ColorOn = string.Empty;
            FormatCode[FormatCode.Length - 1].ColorOff = string.Empty;
            FormatCode[FormatCode.Length - 1].SizeOn = string.Empty;
            FormatCode[FormatCode.Length - 1].SizeOff = string.Empty;
            FormatCode[FormatCode.Length - 1].BoldOn = string.Empty;
            FormatCode[FormatCode.Length - 1].BoldOff = string.Empty;
            FormatCode[FormatCode.Length - 1].ItalicOn = string.Empty;
            FormatCode[FormatCode.Length - 1].ItalicOff = string.Empty;
            FormatCode[FormatCode.Length - 1].UnderlineOn = string.Empty;
            FormatCode[FormatCode.Length - 1].UnderlineOff = string.Empty;
            FormatCode[FormatCode.Length - 1].Space = WhiteSpace.Tab;
            AddCodes();
            FormatCode[FormatCode.Length - 1].Name = "Universal Codes";
            FormatCode[FormatCode.Length - 1].Notes = "No font size or color attributes";
            FormatCode[FormatCode.Length - 1].ColorOn = string.Empty;
            FormatCode[FormatCode.Length - 1].ColorOff = string.Empty;
            FormatCode[FormatCode.Length - 1].SizeOn = string.Empty;
            FormatCode[FormatCode.Length - 1].SizeOff = string.Empty;
            FormatCode[FormatCode.Length - 1].BoldOn = "[b]";
            FormatCode[FormatCode.Length - 1].BoldOff = "[/b]";
            FormatCode[FormatCode.Length - 1].ItalicOn = "[i]";
            FormatCode[FormatCode.Length - 1].ItalicOff = "[/i]";
            FormatCode[FormatCode.Length - 1].UnderlineOn = "[u]";
            FormatCode[FormatCode.Length - 1].UnderlineOff = "[/u]";
            FormatCode[FormatCode.Length - 1].Space = WhiteSpace.Space;
            AddCodes();
            FormatCode[FormatCode.Length - 1].Name = "phpBB";
            FormatCode[FormatCode.Length - 1].Notes = "As used by HU and The Echelon";
            FormatCode[FormatCode.Length - 1].ColorOn = "[color=%VAL%]";
            FormatCode[FormatCode.Length - 1].ColorOff = "[/color]";
            FormatCode[FormatCode.Length - 1].SizeOn = "[size=%VAL%]";
            FormatCode[FormatCode.Length - 1].SizeOff = "[/size]";
            FormatCode[FormatCode.Length - 1].BoldOn = "[b]";
            FormatCode[FormatCode.Length - 1].BoldOff = "[/b]";
            FormatCode[FormatCode.Length - 1].ItalicOn = "[i]";
            FormatCode[FormatCode.Length - 1].ItalicOff = "[/i]";
            FormatCode[FormatCode.Length - 1].UnderlineOn = "[u]";
            FormatCode[FormatCode.Length - 1].UnderlineOff = "[/u]";
            FormatCode[FormatCode.Length - 1].Space = WhiteSpace.Space;
            AddCodes();
            FormatCode[FormatCode.Length - 1].Name = "UBB.threads";
            FormatCode[FormatCode.Length - 1].Notes =
                "Used by the official CoX foums (which don't support small-fonts for the data chunk)";
            FormatCode[FormatCode.Length - 1].ColorOn = "[color:%VAL%]";
            FormatCode[FormatCode.Length - 1].ColorOff = "[/color]";
            FormatCode[FormatCode.Length - 1].SizeOn = string.Empty;
            FormatCode[FormatCode.Length - 1].SizeOff = string.Empty;
            FormatCode[FormatCode.Length - 1].BoldOn = "[b]";
            FormatCode[FormatCode.Length - 1].BoldOff = "[/b]";
            FormatCode[FormatCode.Length - 1].ItalicOn = "[i]";
            FormatCode[FormatCode.Length - 1].ItalicOff = "[/i]";
            FormatCode[FormatCode.Length - 1].UnderlineOn = "[u]";
            FormatCode[FormatCode.Length - 1].UnderlineOff = "[/u]";
            FormatCode[FormatCode.Length - 1].Space = WhiteSpace.Space;
            AddCodes();
            FormatCode[FormatCode.Length - 1].Name = "AkBBS";
            FormatCode[FormatCode.Length - 1].Notes = "These codes work with Runboard";
            FormatCode[FormatCode.Length - 1].ColorOn = "[col=%VAL%]";
            FormatCode[FormatCode.Length - 1].ColorOff = "[/col]";
            FormatCode[FormatCode.Length - 1].SizeOn = "[small]";
            FormatCode[FormatCode.Length - 1].SizeOff = "[/small]";
            FormatCode[FormatCode.Length - 1].BoldOn = "[b]";
            FormatCode[FormatCode.Length - 1].BoldOff = "[/b]";
            FormatCode[FormatCode.Length - 1].ItalicOn = "[i]";
            FormatCode[FormatCode.Length - 1].ItalicOff = "[/i]";
            FormatCode[FormatCode.Length - 1].UnderlineOn = "[u]";
            FormatCode[FormatCode.Length - 1].UnderlineOff = "[/u]";
            FormatCode[FormatCode.Length - 1].Space = WhiteSpace.Space;
            AddCodes();
            FormatCode[FormatCode.Length - 1].Name = "EZBoard";
            FormatCode[FormatCode.Length - 1].Notes = string.Empty;
            FormatCode[FormatCode.Length - 1].ColorOn = "[font color=%VAL%]";
            FormatCode[FormatCode.Length - 1].ColorOff = "[/font]";
            FormatCode[FormatCode.Length - 1].SizeOn = "[font size=1]";
            FormatCode[FormatCode.Length - 1].SizeOff = "[/font]";
            FormatCode[FormatCode.Length - 1].BoldOn = "[b]";
            FormatCode[FormatCode.Length - 1].BoldOff = "[/b]";
            FormatCode[FormatCode.Length - 1].ItalicOn = "[i]";
            FormatCode[FormatCode.Length - 1].ItalicOff = "[/i]";
            FormatCode[FormatCode.Length - 1].UnderlineOn = "[u]";
            FormatCode[FormatCode.Length - 1].UnderlineOff = "[/u]";
            FormatCode[FormatCode.Length - 1].Space = WhiteSpace.Space;
        }

        public void LoadCodes(string fName)
        {
            if (!File.Exists(fName))
                return;
            var flag = false;
            StreamReader reader;
            try
            {
                reader = new StreamReader(fName);
            }
            catch (Exception ex)
            {
                var num = (int) MessageBox.Show(ex.Message);
                return;
            }

            var num1 = 0;
            try
            {
                var str = reader.ReadLine();
                do
                {
                    ++num1;
                    if (str == "#END#" || str != "#CODE#")
                        continue;
                    var index1 = -1;
                    var iFc = new FormatCodes();
                    flag = GrabString(out iFc.Name, ref reader) | GrabString(out iFc.Notes, ref reader) |
                           GrabString(out iFc.ColorOn, ref reader) | GrabString(out iFc.ColorOff, ref reader) |
                           GrabString(out iFc.SizeOn, ref reader) | GrabString(out iFc.SizeOff, ref reader) |
                           GrabString(out iFc.BoldOn, ref reader) | GrabString(out iFc.BoldOff, ref reader) |
                           GrabString(out iFc.ItalicOn, ref reader) | GrabString(out iFc.ItalicOff, ref reader) |
                           GrabString(out iFc.UnderlineOn, ref reader) | GrabString(out iFc.UnderlineOff, ref reader) |
                           GrabString(out var dest, ref reader);
                    iFc.Space = dest.IndexOf(" ", StringComparison.Ordinal) > -1 ? WhiteSpace.Space : WhiteSpace.Tab;
                    if (!flag)
                    {
                        for (var index2 = 0; index2 < FormatCode.Length; ++index2)
                            if (FormatCode[index2].Name == iFc.Name)
                                index1 = index2;
                        if (index1 == -1)
                        {
                            Array.Resize(ref FormatCode, FormatCode.Length + 1);
                            index1 = FormatCode.Length - 1;
                        }

                        FormatCode[index1].Assign(iFc);
                        str = reader.ReadLine();
                    }
                    else
                    {
                        break;
                    }
                } while (!((str == "#END#") | (num1 > 1024)));

                if ((num1 > 1024) & !flag)
                {
                    var num2 = (int) MessageBox.Show(
                        "Nonfatal error reading Forum Code Update. Couldn't find end of file!");
                }
                else if (flag)
                {
                    var num3 = (int) MessageBox.Show("Nonfatal error reading Forum Code Update.");
                }
            }
            catch (Exception ex)
            {
                var num2 = (int) MessageBox.Show(ex.Message);
            }
            finally
            {
                reader.Close();
            }
        }

        public struct ColorScheme
        {
            public string SchemeName;
            public Color Title;
            public Color Heading;
            public Color Level;
            public Color Slots;
            public Color Power;
            public Color IOColor;
            public Color SetColor;
            public Color HOColor;

            public void Assign(ColorScheme iCs)
            {
                SchemeName = iCs.SchemeName;
                Title = iCs.Title;
                Heading = iCs.Heading;
                Level = iCs.Level;
                Power = iCs.Power;
                Slots = iCs.Slots;
                IOColor = iCs.IOColor;
                SetColor = iCs.SetColor;
                HOColor = iCs.HOColor;
            }

            public void SetDefault()
            {
                SchemeName = string.Empty;
                Title = Color.MediumBlue;
                Heading = Color.Navy;
                Level = Color.DarkSlateBlue;
                Slots = Color.DarkSlateBlue;
                Power = Color.Black;
                IOColor = Color.DarkBlue;
                SetColor = Color.Navy;
                HOColor = Color.DarkCyan;
            }
        }

        public struct FormatCodes
        {
            private const string Placeholder = "%VAL%";

            public string Name;
            public string Notes;
            public string ColorOn;
            public string ColorOff;
            public string SizeOn;
            public string SizeOff;
            public string BoldOn;
            public string BoldOff;
            public string ItalicOn;
            public string ItalicOff;
            public string UnderlineOn;
            public string UnderlineOff;
            public WhiteSpace Space;

            public static string FillCode(string iCode, string iVal)
            {
                return iCode.Replace("%VAL%", iVal);
            }

            public void Assign(FormatCodes iFc)
            {
                Name = iFc.Name;
                Notes = iFc.Notes;
                ColorOn = iFc.ColorOn;
                ColorOff = iFc.ColorOff;
                SizeOn = iFc.SizeOn;
                SizeOff = iFc.SizeOff;
                BoldOn = iFc.BoldOn;
                BoldOff = iFc.BoldOff;
                ItalicOn = iFc.ItalicOn;
                ItalicOff = iFc.ItalicOff;
                UnderlineOn = iFc.UnderlineOn;
                UnderlineOff = iFc.UnderlineOff;
                Space = iFc.Space;
            }

            public void SetDefault()
            {
                Name = string.Empty;
                Notes = string.Empty;
                ColorOn = string.Empty;
                ColorOff = string.Empty;
                SizeOn = string.Empty;
                SizeOff = string.Empty;
                BoldOn = string.Empty;
                BoldOff = string.Empty;
                ItalicOn = string.Empty;
                ItalicOff = string.Empty;
                UnderlineOn = string.Empty;
                UnderlineOff = string.Empty;
                Space = WhiteSpace.Space;
            }
        }
    }
}