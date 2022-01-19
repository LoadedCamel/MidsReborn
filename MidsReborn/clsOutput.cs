using System;
using System.Drawing;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn
{
    public class clsOutput
    {
        private readonly string[] BBWhite = {" ", "\t"};

        public readonly bool HTML;
        private readonly int idScheme = MidsContext.Config.ExportScheme;

        private readonly bool LongExport;

        private readonly bool NoHTMLBr;

        private readonly bool UNB;
        public int idFormat = MidsContext.Config.ExportTarget;

        public bool Plain;

        public clsOutput()
        {
            LongExport = MidsContext.Config.LongExport;
            if ((MidsContext.Config.ExportTarget < 0) | (MidsContext.Config.Export.FormatCode.Length == 0))
            {
                MidsContext.Config.Export.ResetCodesToDefaults();
                MessageBox.Show(@"No formatting codes found, resetting to defaults.", @"Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if ((MidsContext.Config.ExportScheme < 0) | (MidsContext.Config.Export.ColorSchemes.Length == 0))
            {
                MidsContext.Config.Export.ResetColorsToDefaults();
                MessageBox.Show(@"No color schemes found, resetting to defaults.", @"Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (MidsContext.Config.ExportTarget >= MidsContext.Config.Export.FormatCode.Length)
                MidsContext.Config.ExportTarget = 0;
            if (MidsContext.Config.ExportScheme >= MidsContext.Config.Export.ColorSchemes.Length)
                MidsContext.Config.ExportScheme = 0;
            if (MidsContext.Config.Export.FormatCode[idFormat].Notes.Contains(nameof(UNB)))
                UNB = true;
            if (MidsContext.Config.Export.FormatCode[idFormat].Name.Contains(nameof(UNB)))
                UNB = true;
            if (MidsContext.Config.Export.FormatCode[idFormat].Notes.IndexOf(nameof(HTML), StringComparison.Ordinal) >
                -1)
                HTML = true;
            if (MidsContext.Config.Export.FormatCode[idFormat].Notes
                .IndexOf("no <br /> tags", StringComparison.OrdinalIgnoreCase) <= -1)
                return;
            NoHTMLBr = true;
        }

        public string Build(string iDataLink)
        {
            var str1 = "";
            var formatCode = MidsContext.Config.Export.FormatCode;
            var idFormatA = idFormat;
            Plain = formatCode[idFormatA].BoldOn
                + formatCode[idFormatA].ColorOn
                + formatCode[idFormatA].ItalicOn
                + formatCode[idFormatA].SizeOn
                + formatCode[idFormatA].UnderlineOn == "";
            var str2 = MidsContext.Character.Alignment.ToString();
            var str3 = str1 +
                       formatColor(formatBold($"This {str2} build was built using Mids Reborn {MidsContext.AppAssemblyVersion}"), ExportConfig.Element.Heading) + LineBreak() + formatColor(formatBold(@"https://github.com/LoadedCamel/MidsReborn"),
                                       ExportConfig.Element.Heading) + LineBreak();
            if (iDataLink != "" && !Plain)
                str3 = str3 + LineBreak() +
                       formatColor(formatUnderline(formatBold(iDataLink)), ExportConfig.Element.Title) + LineBreak();
            var str4 = str3 + LineBreak();
            var num = MidsContext.Character.Level + 1;
            if (num > 50)
                num = 50;
            string str5;
            if (MidsContext.Character.Name != "")
                str5 = str4 + formatBold(formatColor(MidsContext.Character.Name + ":", ExportConfig.Element.Heading) +
                                         formatColor(
                                             " Level " + Convert.ToString(num) + " " +
                                             MidsContext.Character.Archetype.Origin[MidsContext.Character.Origin] +
                                             " " + MidsContext.Character.Archetype.DisplayName,
                                             ExportConfig.Element.Power)) + LineBreak();
            else
                str5 = str4 +
                       formatBold(formatColor(
                           "Level " + Convert.ToString(num) + " " +
                           MidsContext.Character.Archetype.Origin[MidsContext.Character.Origin] + " " +
                           MidsContext.Character.Archetype.DisplayName, ExportConfig.Element.Power)) + LineBreak();
            var str6 = str5 +
                       formatBold(formatColor("Primary Power Set: ", ExportConfig.Element.Heading) +
                                  formatColor(MidsContext.Character.Powersets[0].DisplayName,
                                      ExportConfig.Element.Power)) + LineBreak() + formatBold(
                           formatColor("Secondary Power Set: ", ExportConfig.Element.Heading) +
                           formatColor(MidsContext.Character.Powersets[1].DisplayName, ExportConfig.Element.Power)) +
                       LineBreak();
            if (MidsContext.Character.PoolTaken(3))
                str6 = str6 + formatBold(formatColor("Power Pool: ", ExportConfig.Element.Heading) +
                                         formatColor(MidsContext.Character.Powersets[3].DisplayName,
                                             ExportConfig.Element.Power)) + LineBreak();
            if (MidsContext.Character.PoolTaken(4))
                str6 = str6 + formatBold(formatColor("Power Pool: ", ExportConfig.Element.Heading) +
                                         formatColor(MidsContext.Character.Powersets[4].DisplayName,
                                             ExportConfig.Element.Power)) + LineBreak();
            if (MidsContext.Character.PoolTaken(5))
                str6 = str6 + formatBold(formatColor("Power Pool: ", ExportConfig.Element.Heading) +
                                         formatColor(MidsContext.Character.Powersets[5].DisplayName,
                                             ExportConfig.Element.Power)) + LineBreak();
            if (MidsContext.Character.PoolTaken(6))
                str6 = str6 + formatBold(formatColor("Power Pool: ", ExportConfig.Element.Heading) +
                                         formatColor(MidsContext.Character.Powersets[6].DisplayName,
                                             ExportConfig.Element.Power)) + LineBreak();
            if (MidsContext.Character.PoolTaken(7))
                str6 = str6 + formatBold(formatColor("Ancillary Pool: ", ExportConfig.Element.Heading) +
                                         formatColor(MidsContext.Character.Powersets[7].DisplayName,
                                             ExportConfig.Element.Power)) + LineBreak();
            var str7 = str6 + LineBreak() + formatColor(formatBold(str2 + " Profile:"), ExportConfig.Element.Heading) +
                       LineBreak();
            if (Plain)
                str7 = str7 + "------------" + LineBreak();
            var str8 = str7 + BuildPowerList(true, false, false) +
                       formatColor("------------", ExportConfig.Element.Heading) + LineBreak() +
                       BuildPowerList(false, true, false);
            if (MidsContext.Character.Archetype.Epic)
                str8 = str8 + formatColor("------------", ExportConfig.Element.Heading) + LineBreak() +
                       BuildPowerList(false, true, true);
            if (MidsContext.Config.ExportBonusTotals)
                str8 = str8 + formatColor("------------", ExportConfig.Element.Heading) + LineBreak() +
                       BuildSetBonusListShort() + LineBreak();
            if (MidsContext.Config.ExportBonusList)
                str8 = str8 + formatColor("------------", ExportConfig.Element.Heading) + LineBreak() +
                       buildSetBonusListLong() + LineBreak();
            return str8 + LineBreak();
        }

        private string BuildPowerList(bool SkipInherent, bool SkipNormal, bool Kheldian)

        {
            var str1 = "";
            var str2 = WhiteSpace();
            var exportIoLevels = MidsContext.Config.I9.ExportIOLevels;
            var flag1 = !MidsContext.Config.I9.ExportStripSetNames;
            var flag2 = !MidsContext.Config.I9.ExportStripEnh;
            var num1 = MidsContext.Character.CurrentBuild.Powers.Count - 1;
            for (var index1 = 0; index1 <= num1; ++index1)
            {
                var flag3 = !MidsContext.Character.CurrentBuild.Powers[index1].Chosen &
                            (MidsContext.Character.CurrentBuild.Powers[index1].NIDPower == -1);
                var flag4 = false;
                if (!SkipInherent && flag3)
                {
                    if (Kheldian)
                    {
                        if (MidsContext.Character.CurrentBuild.Powers[index1].NIDPower > -1 &&
                            DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[index1].NIDPower]
                                .Requires.NPowerID.Length > 0 && DatabaseAPI.Database
                                .Power[MidsContext.Character.CurrentBuild.Powers[index1].NIDPower].Requires
                                .NPowerID[0][0] != -1)
                            flag4 = true;
                    }
                    else if (MidsContext.Character.CurrentBuild.Powers[index1].NIDPower > -1)
                    {
                        if ((DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[index1].NIDPower]
                            .Requires.NPowerID.Length == 0) | !DatabaseAPI.Database
                            .Power[MidsContext.Character.CurrentBuild.Powers[index1].NIDPower].Slottable)
                            flag4 = true;
                    }
                    else
                    {
                        flag4 = false;
                    }
                }

                if (!SkipNormal & !flag3)
                    flag4 = true;
                if (flag3 & (MidsContext.Character.CurrentBuild.Powers[index1].NIDPowerset < 0))
                    flag4 = false;
                if (flag3 & (MidsContext.Character.CurrentBuild.Powers[index1].SubPowers.Length > 0))
                    flag4 = false;
                if (!flag4)
                    continue;
                if ((MidsContext.Character.CurrentBuild.Powers[index1].NIDPowerset > -1) &
                    (MidsContext.Character.CurrentBuild.Powers[index1].IDXPower > -1))
                {
                    str1 = str1 + formatBold(
                        formatColor(
                            "Level " + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Level + 1) +
                            ":", ExportConfig.Element.Level) + str2 +
                        formatColor(
                            DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[index1].NIDPower]
                                .DisplayName, ExportConfig.Element.Power)) + str2;
                    var flag5 = !MidsContext.Character.CurrentBuild.Powers[index1].Chosen;
                }
                else
                {
                    str1 = str1 +
                           formatBold(
                               formatColor(
                                   "Level " + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Level +
                                                               1) + ":", ExportConfig.Element.Level) + str2 +
                               formatItalic(formatColor("[Empty]", ExportConfig.Element.Power))) + str2;
                }

                if (MidsContext.Character.CurrentBuild.Powers[index1].Slots.Length > 0)
                {
                    if (!LongExport)
                    {
                        if (!Plain)
                            str1 += formatColor("--", ExportConfig.Element.Heading);
                        var num2 =
                            !((MidsContext.Character.CurrentBuild.Powers[index1].NIDPowerset > -1) &
                              (MidsContext.Character.CurrentBuild.Powers[index1].IDXPower > -1))
                                ? 7
                                : DatabaseAPI.Database.Power[MidsContext.Character.CurrentBuild.Powers[index1].NIDPower]
                                    .DisplayName.Length;
                        if (Plain & (num2 < 16))
                        {
                            str1 += str2;
                            if (num2 < 8)
                                str1 += str2;
                        }

                        str1 += str2;
                    }

                    var iText = "";
                    var str3 = "";
                    var num3 = MidsContext.Character.CurrentBuild.Powers[index1].Slots.Length - 1;
                    for (var index2 = 0; index2 <= num3; ++index2)
                    {
                        if (flag2)
                        {
                            if (LongExport)
                            {
                                var str4 = iText + ListItemOn();
                                var str5 = " (";
                                str3 = (index2 != 0
                                    ? str5 + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1]
                                        .Slots[index2].Level + 1)
                                    : str5 + "A") + ") ";
                                iText = str4 + str3;
                            }
                            else if (index2 > 0)
                            {
                                iText += formatColor(", ", ExportConfig.Element.Title);
                            }

                            if (MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh <= -1)
                                iText += formatColor("Empty", ExportConfig.Element.Slots);
                            else
                                switch (DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].TypeID)
                                {
                                    case Enums.eType.Normal:
                                        if (LongExport)
                                            iText += formatColor(DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].Name, ExportConfig.Element.Slots);
                                        else
                                            iText += formatColor(DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].ShortName, ExportConfig.Element.Slots);
                                        break;
                                    case Enums.eType.InventO:
                                        if (LongExport)
                                            iText = iText + formatColor(DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].Name, ExportConfig.Element.IO) + formatColor(" IO", ExportConfig.Element.IO);
                                        else
                                            iText = iText + formatColor(DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].ShortName, ExportConfig.Element.IO) + formatColor("-I", ExportConfig.Element.IO);
                                        if (exportIoLevels)
                                            if (!LongExport)
                                                iText += formatColor(":" + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.IOLevel + 1), ExportConfig.Element.IO);
                                            else
                                                iText += formatColor(": Level " + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.IOLevel + 1), ExportConfig.Element.IO);
                                        break;
                                    case Enums.eType.SpecialO:
                                        if (!LongExport)
                                        {
                                            var str4 = DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].SubTypeID switch
                                            {
                                                Enums.eSubtype.DSync => "DS:",
                                                Enums.eSubtype.Hamidon => "HO:",
                                                Enums.eSubtype.Hydra => "HY:",
                                                Enums.eSubtype.Titan => "TN:",
                                                _ => "X:",
                                            };
                                            iText += formatColor(str4 + DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].ShortName, ExportConfig.Element.HO);
                                            break;
                                        }

                                        var str6 = DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].SubTypeID switch
                                        {
                                            Enums.eSubtype.DSync => "DSync:",
                                            Enums.eSubtype.Hamidon => "HamiO:",
                                            Enums.eSubtype.Hydra => "Hydra:",
                                            Enums.eSubtype.Titan => "Titan:",
                                            _ => "Special:",
                                        };
                                        iText += formatColor(str6 + DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].Name, ExportConfig.Element.HO);
                                        break;
                                    case Enums.eType.SetO:
                                        if (flag1)
                                            if (LongExport)
                                                iText += formatColor(DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].nIDSet].DisplayName + " - ", ExportConfig.Element.SetO);
                                            else
                                                iText += formatColor(DatabaseAPI.Database.EnhancementSets[DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].nIDSet].ShortName + "-", ExportConfig.Element.SetO);
                                        if (!LongExport)
                                            iText += formatColor(DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].ShortName, ExportConfig.Element.SetO);
                                        else
                                            iText += formatColor(DatabaseAPI.Database.Enhancements[MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh].Name, ExportConfig.Element.SetO);
                                        if (LongExport)
                                        {
                                            if (exportIoLevels & flag1)
                                                iText += formatColor(": Level " + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.IOLevel + 1), ExportConfig.Element.SetO);
                                            if (exportIoLevels & !flag1)
                                                iText += formatColor(": Level " + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.IOLevel + 1), ExportConfig.Element.SetO);
                                            break;
                                        }

                                        if (exportIoLevels & flag1)
                                            iText += formatColor(":" + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.IOLevel + 1), ExportConfig.Element.SetO);
                                        if (exportIoLevels & !flag1)
                                            iText += formatColor("-S:" + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.IOLevel + 1), ExportConfig.Element.SetO);
                                        break;
                                }
                        }

                        if (!LongExport)
                        {
                            var str4 = "(";
                            str3 = (index2 != 0
                                ? str4 + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1]
                                    .Slots[index2].Level + 1)
                                : str4 + "A") + ")";
                        }
                        else if (!flag2)
                        {
                            if (index2 == 0)
                                iText += ListItemOn();
                            var str4 = " (";
                            str3 = (index2 != 0
                                ? str4 + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[index1]
                                    .Slots[index2].Level + 1)
                                : str4 + "A") + ")";
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
                        {
                            iText += str3;
                        }
                    }

                    if (LongExport)
                        iText = List(iText);
                    str1 += iText;
                }

                if (!LongExport | (MidsContext.Character.CurrentBuild.Powers[index1].SlotCount == 0))
                    str1 += LineBreak();
            }

            return str1;
        }

        private string buildSetBonusListLong()
        {
            var str1 = formatColor(formatUnderline(formatBold("Set Bonuses:")), ExportConfig.Element.Heading) +
                       LineBreak();
            var numArray = new int[DatabaseAPI.NidPowers("set_bonus").Length - 1 + 1];
            var num1 = MidsContext.Character.CurrentBuild.SetBonus.Count - 1;
            for (var index1 = 0; index1 <= num1; ++index1)
            {
                var num2 = MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo.Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                {
                    if (MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].Powers.Length <= 0)
                    {
                        continue;
                    }

                    var setInfo = MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo;
                    var index3 = index2;
                    var str2 = str1 +
                               formatColor(
                                   formatUnderline(formatBold(DatabaseAPI.Database
                                       .EnhancementSets[setInfo[index3].SetIDX].DisplayName)), ExportConfig.Element.IO);
                    if (MidsContext.Character.CurrentBuild
                            .Powers[MidsContext.Character.CurrentBuild.SetBonus[index1].PowerIndex].NIDPowerset > -1)
                    {
                        str2 = str2 + LineBreak() + formatColor(
                            "(" + DatabaseAPI.Database
                                .Powersets[
                                    MidsContext.Character.CurrentBuild
                                        .Powers[MidsContext.Character.CurrentBuild.SetBonus[index1].PowerIndex]
                                        .NIDPowerset]
                                .Powers[
                                    MidsContext.Character.CurrentBuild
                                        .Powers[MidsContext.Character.CurrentBuild.SetBonus[index1].PowerIndex]
                                        .IDXPower].DisplayName + ")", ExportConfig.Element.Power);
                    }

                    var iText = "";
                    var num3 = MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SlottedCount - 2;
                    for (var index4 = 0; index4 <= num3; ++index4)
                    {
                        var str3 = iText + ListItemOn();
                        var flag = false;
                        var str4 = "  " + DatabaseAPI.Database
                            .EnhancementSets[MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX]
                            .GetEffectString(index4, false, true);
                        var num4 = DatabaseAPI.Database
                            .EnhancementSets[MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX]
                            .Bonus[index4].Index.Length - 1;
                        for (var index5 = 0; index5 <= num4; ++index5)
                        {
                            if (DatabaseAPI.Database
                                    .EnhancementSets[
                                        MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX]
                                    .Bonus[index4]
                                    .Index[index5] <= -1)
                            {
                                continue;
                            }

                            ++numArray[
                                DatabaseAPI.Database
                                    .EnhancementSets[
                                        MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX]
                                    .Bonus[index4].Index[index5]];
                            if (numArray[
                                    DatabaseAPI.Database
                                        .EnhancementSets[
                                            MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX]
                                        .Bonus[index4].Index[index5]] > 5)
                            {
                                flag = true;
                            }
                        }

                        if (flag)
                        {
                            str4 = formatItalic(formatColor(str4 + " (Exceeded 5 Bonus Cap)",
                                ExportConfig.Element.Slots));
                        }

                        iText = str3 + str4 + ListItemOff();
                    }

                    var num5 = MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].EnhIndexes.Length -
                               1;
                    for (var index4 = 0; index4 <= num5; ++index4)
                    {
                        var index5 = DatabaseAPI.IsSpecialEnh(MidsContext.Character.CurrentBuild.SetBonus[index1]
                            .SetInfo[index2].EnhIndexes[index4]);
                        if (index5 > -1)
                        {
                            iText = iText + ListItemOn() + formatColor("  " + DatabaseAPI.Database.EnhancementSets[MidsContext.Character.CurrentBuild.SetBonus[index1].SetInfo[index2].SetIDX].GetEffectString(index5, true, true), ExportConfig.Element.Power) + ListItemOff();
                        }
                    }

                    str1 = str2 + List(iText);
                }
            }

            return str1;
        }

        private string BuildSetBonusListShort()

        {
            var cumulativeSetBonuses = MidsContext.Character.CurrentBuild.GetCumulativeSetBonuses().ToArray();
            Array.Sort(cumulativeSetBonuses);
            var iText = "";
            var num = cumulativeSetBonuses.Length - 1;
            for (var index = 0; index <= num; ++index)
            {
                var str = cumulativeSetBonuses[index].BuildEffectString(true);
                if (str.IndexOf("Endurance", StringComparison.Ordinal) > -1)
                    str = str.Replace("Endurance", "Max Endurance");
                iText = iText + ListItemOn() + str + ListItemOff();
            }

            return formatColor(formatUnderline(formatBold("Set Bonus Totals:")), ExportConfig.Element.Heading) +
                   List(iText);
        }

        private string formatBold(string iText)

        {
            return MidsContext.Config.Export.FormatCode[idFormat].BoldOn + iText +
                   MidsContext.Config.Export.FormatCode[idFormat].BoldOff;
        }

        private string formatColor(string iText, ExportConfig.Element iElement)

        {
            string str1;
            if (Plain)
            {
                str1 = iText;
            }
            else
            {
                var color = iElement switch
                {
                    ExportConfig.Element.Title => MidsContext.Config.Export.ColorSchemes[idScheme].Title,
                    ExportConfig.Element.Heading => MidsContext.Config.Export.ColorSchemes[idScheme].Heading,
                    ExportConfig.Element.Level => MidsContext.Config.Export.ColorSchemes[idScheme].Level,
                    ExportConfig.Element.Power => MidsContext.Config.Export.ColorSchemes[idScheme].Power,
                    ExportConfig.Element.Slots => MidsContext.Config.Export.ColorSchemes[idScheme].Slots,
                    ExportConfig.Element.IO => MidsContext.Config.Export.ColorSchemes[idScheme].IOColor,
                    ExportConfig.Element.SetO => MidsContext.Config.Export.ColorSchemes[idScheme].SetColor,
                    ExportConfig.Element.HO => MidsContext.Config.Export.ColorSchemes[idScheme].HOColor,
                    _ => Color.Black
                };
                var hexString = $"{color.R:X2}{color.G:X2}{color.B:X2}";
                str1 = ExportConfig.FormatCodes.FillCode(MidsContext.Config.Export.FormatCode[idFormat].ColorOn, "#" + hexString) + iText + MidsContext.Config.Export.FormatCode[idFormat].ColorOff;
            }

            return str1;
        }

        private string formatItalic(string iText)

        {
            return MidsContext.Config.Export.FormatCode[idFormat].ItalicOn + iText +
                   MidsContext.Config.Export.FormatCode[idFormat].ItalicOff;
        }

        private string formatUnderline(string iText)

        {
            return MidsContext.Config.Export.FormatCode[idFormat].UnderlineOn + iText +
                   MidsContext.Config.Export.FormatCode[idFormat].UnderlineOff;
        }

        private string LineBreak()

        {
            return !(HTML & !NoHTMLBr) ? "\r\n" : "<br />\r\n";
        }

        private string List(string iText)

        {
            return !HTML
                ? !Plain ? !UNB ? "[list]" + iText + "[/list]" : "\r\n" + iText + "\r\n\r\n" :
                "\r\n" + iText + "\r\n\r\n"
                : "<ul>" + iText + "</ul>";
        }

        private string ListItemOff()

        {
            return !HTML ? "\r\n" : "</li>\r\n";
        }

        private string ListItemOn()

        {
            return !HTML ? !Plain ? !UNB ? "[*]" : "* " : "" : "<li>";
        }

        private string WhiteSpace()

        {
            return !HTML ? BBWhite[(int) MidsContext.Config.Export.FormatCode[idFormat].Space] : "&nbsp;&nbsp;";
        }
    }
}