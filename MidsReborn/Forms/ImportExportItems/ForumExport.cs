using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.BuildFile.RestModels;
using Mids_Reborn.Forms.Controls;
using RestSharp;

namespace Mids_Reborn.Forms.ImportExportItems
{
    internal class ForumExport
    {
        public string TitleColor { get; set; }
        public string CategoryColor { get; set; }
        public string PowerLevelColor { get; set; }
        public string SectionColor { get; set; }
        public string SlotLevelColor { get; set; }

        public int CategorySize { get; set; }
        public int EnhancementSize { get; set; }
        public int PowerLevelSize { get; set; }
        public int SectionSize { get; set; }
        public int SlotLevelSize { get; set; }

        public string? Code { get; set; }
        public string? ShareUrl { get; set; }

        private sealed class ExportBuilder : IDisposable
        {
            private string _export;
            private bool _disposed;

            public ExportBuilder(string export)
            {
                _export = export;
            }

            public void Append(string text)
            {
                _export += $" {text}";
            }

            public void AppendLine(string text)
            {
                _export += $"\r\n{text}";
            }

            public override string ToString()
            {
                return _export;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (_disposed) return;
                if (disposing) 
                {
                    _export = string.Empty;
                }

                _disposed = true;
            }
        }

        public static void BuildExport(bool inclAccolade, bool inclIncarnate, bool inclSetBonus = false, bool inclBreakdown = false, bool shortExport = true)
        {
            var tempString = $"[size=\"5\"][b][color=\"#eba91c\"]This build was created with {MidsContext.AppName} {MidsContext.AppFileVersion}";
            using var output = new ExportBuilder(tempString);
            output.AppendLine($"Using {DatabaseAPI.DatabaseName} database {DatabaseAPI.Database.Version}[/color]");
            output.AppendLine(" ");
            output.AppendLine($"[color=\"DodgerBlue\"][url=\"mrb://\"]Click Here To Open This Build In MRB");
            var character = string.IsNullOrWhiteSpace(MidsContext.Character.Name) ? $"[color=\"#eb2a1c\"]Character[/color]: Level {MidsContext.Character.Level + 1} {MidsContext.Character.Archetype?.Origin[MidsContext.Character.Origin]} {MidsContext.Character.Archetype?.DisplayName} ({MidsContext.Character.Alignment})" : $"[color=\"#eb2a1c\"]Character[/color]: \"{MidsContext.Character.Name}\" A Level {MidsContext.Character.Level + 1} {MidsContext.Character.Archetype?.Origin[MidsContext.Character.Origin]} {MidsContext.Character.Archetype?.DisplayName} ({MidsContext.Character.Alignment})";
            output.AppendLine(character);
            foreach (var set in MidsContext.Character.Powersets)
            {
                if (set != null)
                {
                    switch (set.SetType)
                    {
                        case Enums.ePowerSetType.Primary:
                            output.AppendLine($"[color=\"#eb2a1c\"]Primary Power Set[/color]: {set.DisplayName}");
                            break;
                        case Enums.ePowerSetType.Secondary:
                            output.AppendLine($"[color=\"#eb2a1c\"]Secondary Power Set[/color]: {set.DisplayName}");
                            break;
                        case Enums.ePowerSetType.Ancillary:
                            output.AppendLine($"[color=\"#eb2a1c\"]Epic Power Pool[/color]: {set.DisplayName}");
                            break;
                        case Enums.ePowerSetType.Pool:
                            output.AppendLine($"[color=\"#eb2a1c\"]Power Pool[/color]: {set.DisplayName}");
                            break;
                        case Enums.ePowerSetType.Incarnate:
                            output.AppendLine($"[color=\"#eb2a1c\"]Incarnate Pool[/color]: {set.DisplayName}");
                            break;
                    }
                }
            }

            output.AppendLine(" ");
            output.AppendLine(@"[color=""#eba91c""][u]Powers[/u][/color]");
            foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
            {
                if (powerEntry?.Power != null & powerEntry.Power.Slottable)
                {
                    if (powerEntry.Power.GetPowerSet().SetType is not Enums.ePowerSetType.Inherent)
                    {
                        if (powerEntry.Slots[0].Enhancement.Enh <= -1) continue;
                        if (!shortExport)
                        {
                            output.AppendLine($"[size=\"5\"][color=\"#eb2a1c\"]Level {powerEntry.Level + 1}[/color]: {powerEntry.Power.DisplayName}[size=\"4\"]");
                        }
                        else
                        {
                            output.AppendLine($"[size=\"5\"][color=\"#eb2a1c\"]Level {powerEntry.Level + 1}[/color]: {powerEntry.Power.DisplayName}[size=\"4\"] [color=\"#eba91c\"]-[/color] ");
                        }

                        for (var slotIndex = 0; slotIndex < powerEntry.Slots.Length; slotIndex++)
                        {
                            var slot = powerEntry.Slots[slotIndex];
                            if (slot.Enhancement.Enh > -1)
                            {
                                var enhancement = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh];
                                if (!shortExport)
                                {
                                    output.AppendLine(
                                        $"\t[color=\"#eba91c\"]Slot Level {slot.Level + 1}[/color]: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].LongName}");
                                }
                                else
                                {
                                    if (slotIndex < powerEntry.Slots.Length - 1)
                                    {
                                        switch (enhancement.TypeID)
                                        {
                                            case Enums.eType.SetO:
                                                output.Append(
                                                    $"{DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].ShortName}: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#74db1a\"]{slot.Level + 1}[/color])[color=\"#eba91c\"],[/color]");
                                                break;
                                            case Enums.eType.SpecialO:
                                            {
                                                var specEnh = DatabaseAPI.GetSpecialEnhByIndex(enhancement.SubTypeID);
                                                output.Append(
                                                    $"{specEnh.ShortName}: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#74db1a\"]{slot.Level + 1}[/color])[color=\"#eba91c\"],[/color]");
                                                break;
                                            }
                                            default:
                                                output.Append(
                                                    $"{DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#74db1a\"]{slot.Level + 1}[/color])[color=\"#eba91c\"],[/color]");
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (enhancement.TypeID)
                                        {
                                            case Enums.eType.SetO:
                                                output.Append(
                                                    $"{DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].ShortName}: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#74db1a\"]{slot.Level + 1}[/color])");
                                                break;
                                            case Enums.eType.SpecialO:
                                            {
                                                var specEnh = DatabaseAPI.GetSpecialEnhByIndex(enhancement.SubTypeID);
                                                output.Append(
                                                    $"{specEnh.ShortName}: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#74db1a\"]{slot.Level + 1}[/color])");
                                                break;
                                            }
                                            default:
                                                output.Append(
                                                    $"{DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#74db1a\"]{slot.Level + 1}[/color])");
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var inhInc = 0;
            foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
            {
                if (inhInc < 1)
                {
                    output.AppendLine(" ");
                    output.AppendLine(@"[size=""5""][color=""#eba91c""][u]Inherents[/u][/color]");
                    inhInc++;
                }
                if (powerEntry?.Power != null & powerEntry.Power.Slottable)
                {
                    if (powerEntry.Power.GetPowerSet().SetType is Enums.ePowerSetType.Inherent)
                    {
                        if (powerEntry.Slots[0].Enhancement.Enh <= -1) continue;
                        output.AppendLine($"[size=\"5\"][color=\"#eb2a1c\"]{powerEntry.Power.DisplayName}[/color]:[size=\"4\"]");
                        for (var slotIndex = 0; slotIndex < powerEntry.Slots.Length; slotIndex++)
                        {
                            var slot = powerEntry.Slots[slotIndex];
                            var enhancement = DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh];
                            if (slot.Enhancement.Enh > -1)
                            {
                                if (!shortExport)
                                {
                                    output.AppendLine(
                                        $"\t[color=\"#eba91c\"]Slot Level {slot.Level + 1}[/color]: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].LongName}");
                                }
                                else
                                {
                                    if (slotIndex < powerEntry.Slots.Length - 1)
                                    {
                                        switch (enhancement.TypeID)
                                        {
                                            case Enums.eType.SetO:
                                                output.Append(
                                                    $"{DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].ShortName}: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#74db1a\"]{slot.Level + 1}[/color])[color=\"#eba91c\"],[/color]");
                                                break;
                                            case Enums.eType.SpecialO:
                                            {
                                                var specEnh = DatabaseAPI.GetSpecialEnhByIndex(enhancement.SubTypeID);
                                                output.Append(
                                                    $"{specEnh.ShortName}: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#74db1a\"]{slot.Level + 1}[/color])[color=\"#eba91c\"],[/color]");
                                                break;
                                            }
                                            default:
                                                output.Append(
                                                    $"{DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#74db1a\"]{slot.Level + 1}[/color])[color=\"#eba91c\"],[/color]");
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (enhancement.TypeID)
                                        {
                                            case Enums.eType.SetO:
                                                output.Append(
                                                    $"{DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].ShortName}: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#74db1a\"] {slot.Level + 1} [/color])");
                                                break;
                                            case Enums.eType.SpecialO:
                                            {
                                                var specEnh = DatabaseAPI.GetSpecialEnhByIndex(enhancement.SubTypeID);
                                                output.Append(
                                                    $"{specEnh.ShortName}: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#74db1a\"] {slot.Level + 1} [/color])");
                                                break;
                                            }
                                            default:
                                                output.Append(
                                                    $"{DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#74db1a\"]{slot.Level + 1}[/color])");
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (inclAccolade & !inclIncarnate)
            {
                output.AppendLine(" ");
                output.AppendLine(@"[size=""5""][color=""#eba91c""][u]Accolades[/u][/color]");
            }
            else if (!inclAccolade & inclIncarnate)
            {
                output.AppendLine(" ");
                output.AppendLine(@"[size=""5""][color=""#eba91c""][u]Incarnates[/u][/color]");
            }
            else if (inclAccolade & inclIncarnate)
            {
                output.AppendLine(" ");
                output.AppendLine(@"[size=""5""][color=""#eba91c""][u]Accolades & Incarnates[/u][/color]");
            }

            if (inclAccolade || inclIncarnate)
            {
                foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
                {
                    if (powerEntry?.Power != null & !powerEntry.Chosen)
                    {
                        switch (powerEntry.PowerSet.SetType)
                        {
                            case Enums.ePowerSetType.Accolade when inclAccolade:
                                output.AppendLine(
                                    $"[size=\"5\"][color=\"#eb2a1c\"]Accolade[/color]:[size=\"4\"] {powerEntry.Power.DisplayName}");
                                break;
                            case Enums.ePowerSetType.Incarnate when inclIncarnate:
                                output.AppendLine(
                                    $"[size=\"5\"][color=\"#eb2a1c\"]{powerEntry.Power.GetPowerSet().DisplayName} Slot[/color]:[size=\"4\"] {powerEntry.Power.DisplayName}");
                                break;

                        }
                    }
                }
            }

            if (inclSetBonus)
            {
                var setBonuses = MidsContext.Character.CurrentBuild.GetCumulativeSetBonuses().ToList();
                if (setBonuses.Any())
                {
                    output.AppendLine(" ");
                    output.AppendLine(@"[size=""5""][color=""#eba91c""][u]Set Bonus Totals[/u][/color][size=""4""]");
                    foreach (var bonus in setBonuses)
                    {
                        var setBonus = bonus.BuildEffectString(true);
                        if (setBonus.IndexOf("Endurance", StringComparison.Ordinal) > -1)
                        {
                            setBonus = setBonus.Replace("Endurance", "Max Endurance");
                        }
                        output.AppendLine(setBonus);
                    }
                }

            }

            if (inclBreakdown)
            {
                var bonusCheck = new int[DatabaseAPI.NidPowers("set_bonus").Length];
                var setBonuses = MidsContext.Character.CurrentBuild.SetBonus;
                if (setBonuses.Any())
                {
                    output.AppendLine(" ");
                    output.AppendLine(@"[size=""5""][color=""#eba91c""][u]Set Bonus Breakdown[/u][/color][size=""4""]");
                    foreach (var bonus in setBonuses)
                    {
                        var setInfo = bonus.SetInfo.ToList();
                        foreach (var info in setInfo)
                        {
                            if (info.Powers.Length <= 0) continue;
                            output.AppendLine($"[color=\"#eb2a1c\"]{DatabaseAPI.Database.EnhancementSets[info.SetIDX].DisplayName}[/color]: {MidsContext.Character.CurrentBuild.Powers[bonus.PowerIndex]?.Power?.DisplayName}");
                            var slotted = info.SlottedCount - 1;
                            for (var index = 0; index < slotted; index++)
                            {
                                var effect = DatabaseAPI.Database.EnhancementSets[info.SetIDX].GetEffectString(index, false, true);
                                output.AppendLine($"\t[color=\"#74db1a\"]{effect}[/color]");
                                var bonusItems = DatabaseAPI.Database.EnhancementSets[info.SetIDX].Bonus[index].Index.ToList();
                                foreach (var itemIndex in bonusItems)
                                {
                                    if (itemIndex <= -1) continue;
                                    ++bonusCheck[itemIndex];
                                    if (bonusCheck[itemIndex] > 5)
                                    {
                                        output.Append("\t([color=\"#eb2a1c\"]Exceeded the 5 Bonus Cap[/color])");
                                    }
                                }
                            }

                            var enhIndexes = info.EnhIndexes.ToList();
                            for (var enhIndex = 0; enhIndex < enhIndexes.Count; enhIndex++)
                            {
                                var enh = enhIndexes[enhIndex];
                                var specialIndex = DatabaseAPI.IsSpecialEnh(enh);
                                if (specialIndex <= -1) continue;
                                var effect = DatabaseAPI.Database.EnhancementSets[info.SetIDX].GetEffectString(specialIndex, true, true);
                                output.AppendLine($"\t[color=\"#74db1a\"]{effect}[/color]");
                            }
                        }
                    }
                }
            }

            output.AppendLine("[/b][/size]");
            var ret = output.ToString();
            Debug.WriteLine(ret);
        }

        private async void RequestShortUrl(string data)
        {
            var options = new RestClientOptions("https://mids.app")
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var idResponse = await client.GetJsonAsync<ResponseModel>("build/requestId");
            if (idResponse == null || idResponse.Status == "Failed")
            {
                var messageBox = new MessageBoxEx("Failed to obtain a share id.\r\nYou are either not connected to the internet or there may be an issue with the server.", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return;
            };
            var submission = new SubmissionModel(idResponse.Id, data);
            var subResponse = await client.PostJsonAsync<SubmissionModel, ResponseModel>("build/submit", submission);
            if (subResponse == null)
            {
                var messageBox = new MessageBoxEx($"Failed to submit build data to the server.\r\nYou are either not connected to the internet or there may be an issue with the server.", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return;
            }

            if (subResponse.Status == "Failed")
            {
                var messageBox = new MessageBoxEx($"Failed to submit build data to the server.\r\nReason: {subResponse.ErrorMessage}", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return;
            }

            ShareUrl = subResponse.Url;
            Code = subResponse.Code;
        }
    }
}
