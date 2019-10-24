
using System;
using System.Drawing;
using Base.Master_Classes;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public class clsOutput
    {
        readonly string[] BBWhite = { " ", "\t" };

        public bool HTML;
        public int idFormat = MidsContext.Config.ExportTarget;
        readonly int idScheme = MidsContext.Config.ExportScheme;

        readonly bool LongExport;

        readonly bool NoHTMLBr;

        readonly bool UNB;

        public bool Plain;

        public clsOutput()
        {
            LongExport = MidsContext.Config.LongExport;
            if (MidsContext.Config.ExportTarget < 0 | MidsContext.Config.Export.FormatCode.Length == 0)
            {
                MidsContext.Config.Export.ResetCodesToDefaults();
                Interaction.MsgBox("No formatting codes found, resetting to defaults.", MsgBoxStyle.Information, "Huh");
            }
            if (MidsContext.Config.ExportScheme < 0 | MidsContext.Config.Export.ColorSchemes.Length == 0)
            {
                MidsContext.Config.Export.ResetColorsToDefaults();
                Interaction.MsgBox("No colour schemes found, resetting to defaults.", MsgBoxStyle.Information, "Huh");
            }
            if (MidsContext.Config.ExportTarget >= MidsContext.Config.Export.FormatCode.Length)
                MidsContext.Config.ExportTarget = 0;
            if (MidsContext.Config.ExportScheme >= MidsContext.Config.Export.ColorSchemes.Length)
                MidsContext.Config.ExportScheme = 0;
            if (MidsContext.Config.Export.FormatCode[idFormat].Notes.Contains(nameof(UNB)))
                UNB = true;
            if (MidsContext.Config.Export.FormatCode[idFormat].Name.Contains(nameof(UNB)))
                UNB = true;
            if (MidsContext.Config.Export.FormatCode[idFormat].Notes.IndexOf(nameof(HTML), StringComparison.Ordinal) > -1)
                HTML = true;
            if (MidsContext.Config.Export.FormatCode[idFormat].Notes.IndexOf("no <br /> tags", StringComparison.OrdinalIgnoreCase) <= -1)
                return;
            NoHTMLBr = true;
        }

        public string Build(string iDataLink)
        {
            string str1 = "";
            ExportConfig.FormatCodes[] formatCode = MidsContext.Config.Export.FormatCode;
            int idFormatA = idFormat;
            Plain = formatCode[idFormatA].BoldOn
                    + formatCode[idFormatA].ColourOn
                    + formatCode[idFormatA].ItalicOn
                    + formatCode[idFormatA].SizeOn
                    + formatCode[idFormatA].UnderlineOn == "";
            string str2 = MidsContext.Character.Alignment.ToString();
            string str3 = str1 + formatColor(formatBold(str2 + " Plan by Mids' Reborn : Hero Designer " + MidsContext.AppAssemblyVersion), ExportConfig.Element.Heading) + LineBreak() + formatColor(formatBold(@"https://github.com/Crytilis/mids-reborn-hero-designer"), ExportConfig.Element.Heading) + LineBreak();
            if (iDataLink != "" && !Plain)
                str3 = str3 + LineBreak() + formatColor(formatUnderline(formatBold(iDataLink)), ExportConfig.Element.Title) + LineBreak();
            string str4 = str3 + LineBreak();
            int num = MidsContext.Character.Level + 1;
            if (num > 50)
                num = 50;
            string str5;
            if (MidsContext.Character.Name != "")
                str5 = str4 + formatBold(formatColor(MidsContext.Character.Name + ":", ExportConfig.Element.Heading) + formatColor(" Level " + Convert.ToString(num) + " " + MidsContext.Character.Archetype.Origin[MidsContext.Character.Origin] + " " + MidsContext.Character.Archetype.DisplayName, ExportConfig.Element.Power)) + LineBreak();
            else
                str5 = str4 + formatBold(formatColor("Level " + Convert.ToString(num) + " " + MidsContext.Character.Archetype.Origin[MidsContext.Character.Origin] + " " + MidsContext.Character.Archetype.DisplayName, ExportConfig.Element.Power)) + LineBreak();
            string str6 = str5 + formatBold(formatColor("Primary Power Set: ", ExportConfig.Element.Heading) + formatColor(MidsContext.Character.Powersets[0].DisplayName, ExportConfig.Element.Power)) + LineBreak() + formatBold(formatColor("Secondary Power Set: ", ExportConfig.Element.Heading) + formatColor(MidsContext.Character.Powersets[1].DisplayName, ExportConfig.Element.Power)) + LineBreak();
            if (MidsContext.Character.PoolTaken(3))
                str6 = str6 + formatBold(formatColor("Power Pool: ", ExportConfig.Element.Heading) + formatColor(MidsContext.Character.Powersets[3].DisplayName, ExportConfig.Element.Power)) + LineBreak();
            if (MidsContext.Character.PoolTaken(4))
                str6 = str6 + formatBold(formatColor("Power Pool: ", ExportConfig.Element.Heading) + formatColor(MidsContext.Character.Powersets[4].DisplayName, ExportConfig.Element.Power)) + LineBreak();
            if (MidsContext.Character.PoolTaken(5))
                str6 = str6 + formatBold(formatColor("Power Pool: ", ExportConfig.Element.Heading) + formatColor(MidsContext.Character.Powersets[5].DisplayName, ExportConfig.Element.Power)) + LineBreak();
            if (MidsContext.Character.PoolTaken(6))
                str6 = str6 + formatBold(formatColor("Power Pool: ", ExportConfig.Element.Heading) + formatColor(MidsContext.Character.Powersets[6].DisplayName, ExportConfig.Element.Power)) + LineBreak();
            if (MidsContext.Character.PoolTaken(7))
                str6 = str6 + formatBold(formatColor("Ancillary Pool: ", ExportConfig.Element.Heading) + formatColor(MidsContext.Character.Powersets[7].DisplayName, ExportConfig.Element.Power)) + LineBreak();
            string str7 = str6 + LineBreak() + formatColor(formatBold(str2 + " Profile:"), ExportConfig.Element.Heading) + LineBreak();
            if (Plain)
                str7 = str7 + "------------" + LineBreak();
            string str8 = str7 + BuildPowerList(true, false, false) + formatColor("------------", ExportConfig.Element.Heading) + LineBreak() + BuildPowerList(false, true, false);
            if (MidsContext.Character.Archetype.Epic)
                str8 = str8 + formatColor("------------", ExportConfig.Element.Heading) + LineBreak() + BuildPowerList(false, true, true);
            if (MidsContext.Config.ExportBonusTotals)
                str8 = str8 + formatColor("------------", ExportConfig.Element.Heading) + LineBreak() + BuildSetBonusListShort() + LineBreak();
            if (MidsContext.Config.ExportBonusList)
                str8 = str8 + formatColor("------------", ExportConfig.Element.Heading) + LineBreak() + buildSetBonusListLong() + LineBreak();
            return str8 + LineBreak();
        }

        string BuildPowerList(bool SkipInherent, bool SkipNormal, bool Kheldian)

        {
            string str1 = "";
            string str2 = WhiteSpace();
            bool exportIoLevels = MidsContext.Config.I9.ExportIOLevels;
            bool flag1 = !MidsContext.Config.I9.ExportStripSetNames;
            bool flag2 = !MidsContext.Config.I9.ExportStripEnh;
            int num1 = MidsContext.Character.CurrentBuild.Powers.Count - 1;
            for (int index1 = 0; index1 <= num1; ++index1)
            {
                bool flag3 = !MidsContext.Character.CurrentBuild.Powers[index1].Chosen & MidsContext.Character.CurrentBuild.Powers[index1].NIDPower == -1;
                bool flag4 = false;
                if (!SkipInherent && flag3)
                {
                    if (Kheldian)
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[index1].NIDPower > -1 && DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[index1].NIDPower].Requires.NPowerID.Length > 0 && DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[index1].NIDPower].Requires.NPowerID[0][0] != -1)
                            flag4 = true;
                    }
                    else if (MidsContext.Character.CurrentBuild.Powers[index1].NIDPower > -1)
                    {
                        if (DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[index1].NIDPower].Requires.NPowerID.Length == 0 | !DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[index1].NIDPower].Slottable)
                            flag4 = true;
                    }
                    else
                        flag4 = false;
                }
                if (!SkipNormal & !flag3)
                    flag4 = true;
                if (flag3 & MidsContext.Character.CurrentBuild.Powers[index1].NIDPowerset < 0)
                    flag4 = false;
                if (flag3 & MidsContext.Character.CurrentBuild.Powers[index1].SubPowers.Length > 0)
                    flag4 = false;
                if (!flag4)
                    continue;
                if (MidsContext.Character.CurrentBuild.Powers[index1].NIDPowerset > -1 & MidsContext.Character.CurrentBuild.Powers[index1].IDXPower > -1)
                {
                    str1 = str1 + formatBold(formatColor("Level " + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Level + 1) + ":", ExportConfig.Element.Level) + str2 + formatColor(DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[index1].NIDPower].DisplayName, ExportConfig.Element.Power)) + str2;
                    bool flag5 = !MidsContext.Character.CurrentBuild.Powers[index1].Chosen;
                }
                else
                    str1 = str1 + formatBold(formatColor("Level " + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Level + 1) + ":", ExportConfig.Element.Level) + str2 + formatItalic(formatColor("[Empty]", ExportConfig.Element.Power))) + str2;
                if (MidsContext.Character.CurrentBuild.Powers[index1].Slots.Length > 0)
                {
                    if (!LongExport)
                    {
                        if (!Plain)
                            str1 += formatColor("--", ExportConfig.Element.Heading);
                        int num2 = !(MidsContext.Character.CurrentBuild.Powers[index1].NIDPowerset > -1 & MidsContext.Character.CurrentBuild.Powers[index1].IDXPower > -1) ? 7 : DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[index1].NIDPower].DisplayName.Length;
                        if (Plain & num2 < 16)
                        {
                            str1 += str2;
                            if (num2 < 8)
                                str1 += str2;
                        }
                        str1 += str2;
                    }
                    string iText = "";
                    string str3 = "";
                    int num3 = MidsContext.Character.CurrentBuild.Powers[index1].Slots.Length - 1;
                    for (int index2 = 0; index2 <= num3; ++index2)
                    {
                        if (flag2)
                        {
                            if (LongExport)
                            {
                                string str4 = iText + ListItemOn();
                                string str5 = " (";
                                str3 = (index2 != 0 ? str5 + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Level + 1) : str5 + "A") + ") ";
                                iText = str4 + str3;
                            }
                            else if (index2 > 0)
                                iText += formatColor(", ", ExportConfig.Element.Title);
                            if (MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh <= -1)
                            {
                                iText += formatColor("Empty", ExportConfig.Element.Slots);
                            }
                            else
                            {
                                switch (DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].TypeID)
                                {
                                    case Enums.eType.Normal:
                                        iText = LongExport ? iText + formatColor(DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].Name, ExportConfig.Element.Slots) : iText + formatColor(DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].ShortName, ExportConfig.Element.Slots);
                                        break;
                                    case Enums.eType.InventO:
                                        iText = LongExport ? iText + formatColor(DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].Name, ExportConfig.Element.IO) + formatColor(" IO", ExportConfig.Element.IO) : iText + formatColor(DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].ShortName, ExportConfig.Element.IO) + formatColor("-I", ExportConfig.Element.IO);
                                        if (exportIoLevels)
                                        {
                                            iText = !LongExport ? iText + formatColor(":" + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.IOLevel + 1), ExportConfig.Element.IO) : iText + formatColor(": Level " + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.IOLevel + 1), ExportConfig.Element.IO);
                                        }
                                        break;
                                    case Enums.eType.SpecialO:
                                        if (!LongExport)
                                        {
                                            string str4 = DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].SubTypeID != Enums.eSubtype.Hamidon ? (DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].SubTypeID != Enums.eSubtype.Hydra ? (DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].SubTypeID != Enums.eSubtype.Titan ? "X:" : "TN:") : "HY:") : "HO:";
                                            iText += formatColor(str4 + DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].ShortName, ExportConfig.Element.HO);
                                            break;
                                        }
                                        string str6 = DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].SubTypeID == Enums.eSubtype.Hamidon ? "HamiO:" : (DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].SubTypeID != Enums.eSubtype.Hydra ? (DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].SubTypeID != Enums.eSubtype.Titan ? "Special:" : "Titan:") : "Hydra:");
                                        iText += formatColor(str6 + DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].Name, ExportConfig.Element.HO);
                                        break;
                                    case Enums.eType.SetO:
                                        if (flag1)
                                            iText = LongExport ? iText + formatColor(DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].nIDSet].DisplayName + " - ", ExportConfig.Element.SetO) : iText + formatColor(DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].nIDSet].ShortName + "-", ExportConfig.Element.SetO);
                                        iText = !LongExport ? iText + formatColor(DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].ShortName, ExportConfig.Element.SetO) : iText + formatColor(DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].Name, ExportConfig.Element.SetO);
                                        if (LongExport)
                                        {
                                            if (exportIoLevels & flag1)
                                                iText += formatColor(": Level " + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.IOLevel + 1), ExportConfig.Element.SetO);
                                            if (exportIoLevels & !flag1)
                                            {
                                                iText += formatColor(": Level " + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.IOLevel + 1), ExportConfig.Element.SetO);
                                            }
                                            break;
                                        }
                                        if (exportIoLevels & flag1)
                                            iText += formatColor(":" + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.IOLevel + 1), ExportConfig.Element.SetO);
                                        if (exportIoLevels & !flag1)
                                            iText += formatColor("-S:" + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.IOLevel + 1), ExportConfig.Element.SetO);
                                        break;
                                }
                            }
                        }
                        if (!LongExport)
                        {
                            string str4 = "(";
                            str3 = (index2 != 0 ? str4 + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Level + 1) : str4 + "A") + ")";
                        }
                        else if (!flag2)
                        {
                            if (index2 == 0)
                                iText += ListItemOn();
                            string str4 = " (";
                            str3 = (index2 != 0 ? str4 + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Level + 1) : str4 + "A") + ")";
                            iText += str3;
                        }
                        if (LongExport)
                        {
                            if (flag2)
                                iText += ListItemOff();
                            else if (index2 == MidsContext.Character.CurrentBuild.Powers[index1].Slots.Length - 1)
                                iText += ListItemOff();
                        }
                        else
                            iText += str3;
                    }
                    if (LongExport)
                        iText = List(iText);
                    str1 += iText;
                }
                if (!LongExport | MidsContext.Character.CurrentBuild.Powers[index1].SlotCount == 0)
                    str1 += LineBreak();
            }
            return str1;
        }

        string buildSetBonusListLong()

        {
            string str1 = formatColor(formatUnderline(formatBold("Set Bonuses:")), ExportConfig.Element.Heading) + LineBreak();
            int[] numArray = new int[DatabaseAPI.NidPowers("set_bonus", "").Length - 1 + 1];
            int num1 = MidsContext.Character.CurrentBuild.SetBonus.Count - 1;
            for (int index1 = 0; index1 <= num1; ++index1)
            {
                int num2 = MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo.Length - 1;
                for (int index2 = 0; index2 <= num2; ++index2)
                {
                    if (MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].Powers.Length <= 0)
                        continue;
                    I9SetData.sSetInfo[] setInfo = MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo;
                    int index3 = index2;
                    string str2 = str1 + formatColor(formatUnderline(formatBold(DatabaseAPI.Database.EnhancementSets[setInfo[index3].SetIDX].DisplayName)), ExportConfig.Element.IO);
                    if (MidsContext.Character.CurrentBuild.Powers[MidsContext.Character.CurrentBuild.SetBonus[index1].PowerIndex].NIDPowerset > -1)
                        str2 = str2 + LineBreak() + formatColor("(" + DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[MidsContext.Character.CurrentBuild.SetBonus[index1].PowerIndex].NIDPowerset].Powers[MidsContext.Character.CurrentBuild.Powers[MidsContext.Character.CurrentBuild.SetBonus[index1].PowerIndex].IDXPower].DisplayName + ")", ExportConfig.Element.Power);
                    string iText = "";
                    int num3 = MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SlottedCount - 2;
                    for (int index4 = 0; index4 <= num3; ++index4)
                    {
                        string str3 = iText + ListItemOn();
                        bool flag = false;
                        string str4 = "  " + DatabaseAPI.Database.EnhancementSets[MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX].GetEffectString(index4, false, true);
                        int num4 = DatabaseAPI.Database.EnhancementSets[MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX].Bonus[index4].Index.Length - 1;
                        for (int index5 = 0; index5 <= num4; ++index5)
                        {
                            if (DatabaseAPI.Database
                                    .EnhancementSets[MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX].Bonus[index4]
                                    .Index[index5] <= -1)
                                continue;
                            ++numArray[DatabaseAPI.Database.EnhancementSets[MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX].Bonus[index4].Index[index5]];
                            if (numArray[DatabaseAPI.Database.EnhancementSets[MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX].Bonus[index4].Index[index5]] > 5)
                                flag = true;
                        }
                        if (flag)
                            str4 = formatItalic(formatColor(str4 + " (Exceeded 5 Bonus Cap)", ExportConfig.Element.Slots));
                        iText = str3 + str4 + ListItemOff();
                    }
                    int num5 = MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].EnhIndexes.Length - 1;
                    for (int index4 = 0; index4 <= num5; ++index4)
                    {
                        int index5 = DatabaseAPI.IsSpecialEnh(MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].EnhIndexes[index4]);
                        if (index5 > -1)
                            iText = iText + ListItemOn() + formatColor("  " + DatabaseAPI.Database.EnhancementSets[MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX].GetEffectString(index5, true, true), ExportConfig.Element.Power) + ListItemOff();
                    }
                    str1 = str2 + List(iText);
                }
            }
            return str1;
        }

        string BuildSetBonusListShort()

        {
            IEffect[] cumulativeSetBonuses = MidsContext.Character.CurrentBuild.GetCumulativeSetBonuses();
            Array.Sort(cumulativeSetBonuses);
            string iText = "";
            int num = cumulativeSetBonuses.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                string str = cumulativeSetBonuses[index].BuildEffectString(true);
                if (str.IndexOf("Endurance", StringComparison.Ordinal) > -1)
                    str = str.Replace("Endurance", "Max Endurance");
                iText = iText + ListItemOn() + str + ListItemOff();
            }
            return formatColor(formatUnderline(formatBold("Set Bonus Totals:")), ExportConfig.Element.Heading) + List(iText);
        }

        string formatBold(string iText)

        {
            return MidsContext.Config.Export.FormatCode[idFormat].BoldOn + iText + MidsContext.Config.Export.FormatCode[idFormat].BoldOff;
        }

        string formatColor(string iText, ExportConfig.Element iElement)

        {
            string str1;
            if (Plain)
            {
                str1 = iText;
            }
            else
            {
                Color color;
                switch (iElement)
                {
                    case ExportConfig.Element.Title:
                        color = MidsContext.Config.Export.ColorSchemes[idScheme].Title;
                        break;
                    case ExportConfig.Element.Heading:
                        color = MidsContext.Config.Export.ColorSchemes[idScheme].Heading;
                        break;
                    case ExportConfig.Element.Level:
                        color = MidsContext.Config.Export.ColorSchemes[idScheme].Level;
                        break;
                    case ExportConfig.Element.Power:
                        color = MidsContext.Config.Export.ColorSchemes[idScheme].Power;
                        break;
                    case ExportConfig.Element.Slots:
                        color = MidsContext.Config.Export.ColorSchemes[idScheme].Slots;
                        break;
                    case ExportConfig.Element.IO:
                        color = MidsContext.Config.Export.ColorSchemes[idScheme].IOColor;
                        break;
                    case ExportConfig.Element.SetO:
                        color = MidsContext.Config.Export.ColorSchemes[idScheme].SetColor;
                        break;
                    case ExportConfig.Element.HO:
                        color = MidsContext.Config.Export.ColorSchemes[idScheme].HOColor;
                        break;
                    default:
                        color = Color.Black;
                        break;
                }
                string str2 = Conversion.Hex(color.R);
                string str3 = Conversion.Hex(color.G);
                string str4 = Conversion.Hex(color.B);
                while (str2.Length < 2)
                    str2 = "0" + str2;
                while (str3.Length < 2)
                    str3 = "0" + str3;
                while (str4.Length < 2)
                    str4 = "0" + str4;
                str1 = ExportConfig.FormatCodes.FillCode(MidsContext.Config.Export.FormatCode[idFormat].ColourOn, "#" + str2 + str3 + str4) + iText + MidsContext.Config.Export.FormatCode[idFormat].ColourOff;
            }
            return str1;
        }

        string formatItalic(string iText)

        {
            return MidsContext.Config.Export.FormatCode[idFormat].ItalicOn + iText + MidsContext.Config.Export.FormatCode[idFormat].ItalicOff;
        }

        string formatUnderline(string iText)

        {
            return MidsContext.Config.Export.FormatCode[idFormat].UnderlineOn + iText + MidsContext.Config.Export.FormatCode[idFormat].UnderlineOff;
        }

        string LineBreak()

        {
            return !(HTML & !NoHTMLBr) ? "\r\n" : "<br />\r\n";
        }

        string List(string iText)

        {
            return !HTML ? (!Plain ? (!UNB ? "[list]" + iText + "[/list]" : "\r\n" + iText + "\r\n\r\n") : "\r\n" + iText + "\r\n\r\n") : "<ul>" + iText + "</ul>";
        }

        string ListItemOff()

        {
            return !HTML ? "\r\n" : "</li>\r\n";
        }

        string ListItemOn()

        {
            return !HTML ? (!Plain ? (!UNB ? "[*]" : "* ") : "") : "<li>";
        }

        string WhiteSpace()

        {
            return !HTML ? BBWhite[(int)MidsContext.Config.Export.FormatCode[idFormat].Space] : "&nbsp;&nbsp;";
        }
    }
}
