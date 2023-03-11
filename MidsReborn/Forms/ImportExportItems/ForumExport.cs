using System;
using System.Diagnostics;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Forms.ImportExportItems
{
    internal class ForumExport
    {
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

        public static void BuildExport(bool inclAccolade, bool inclIncarnate, bool shortExport = true)
        {
            var tempString = $"[size=\"5][b][color=\"#eba91c\"]This build was created with {MidsContext.AppName} {MidsContext.AppFileVersion}";
            using var output = new ExportBuilder(tempString);
            output.AppendLine($"Using {DatabaseAPI.DatabaseName} database {DatabaseAPI.Database.Version}[/color]");
            output.AppendLine("");
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

            output.AppendLine("");
            output.AppendLine(@"[color=""#eba91c""][u].::Powers::.[/u][/color]");
            foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
            {
                if (powerEntry?.Power != null & powerEntry.Power.Slottable)
                {
                    if (powerEntry.Power.GetPowerSet().SetType is not Enums.ePowerSetType.Inherent)
                    {
                        if (powerEntry.Slots[0].Enhancement.Enh <= -1) continue;
                        output.AppendLine($"[color=\"#eb2a1c\"]Level {powerEntry.Level + 1}[/color]: {powerEntry.Power.DisplayName}");
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
                                                    $"{DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].ShortName}: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#eba91c\"]{slot.Level + 1}[/color])[color=\"#757171\"],[/color]");
                                                break;
                                            case Enums.eType.SpecialO:
                                            {
                                                var specEnh = DatabaseAPI.GetSpecialEnhByIndex(enhancement.SubTypeID);
                                                output.Append(
                                                    $"{specEnh.ShortName}: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#eba91c\"]{slot.Level + 1}[/color])[color=\"#757171\"],[/color]");
                                                break;
                                            }
                                            default:
                                                output.Append(
                                                    $"{DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#eba91c\"]{slot.Level + 1}[/color])[color=\"#757171\"],[/color]");
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (enhancement.TypeID)
                                        {
                                            case Enums.eType.SetO:
                                                output.Append(
                                                    $"{DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].ShortName}: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#eba91c\"]{slot.Level + 1}[/color])");
                                                break;
                                            case Enums.eType.SpecialO:
                                            {
                                                var specEnh = DatabaseAPI.GetSpecialEnhByIndex(enhancement.SubTypeID);
                                                output.Append(
                                                    $"{specEnh.ShortName}: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#eba91c\"]{slot.Level + 1}[/color])");
                                                break;
                                            }
                                            default:
                                                output.Append(
                                                    $"{DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#eba91c\"]{slot.Level + 1}[/color])");
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            output.AppendLine("");
            output.AppendLine(@"[color=""#eba91c""][u].::Inherents::.[/u][/color]");
            foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
            {
                if (powerEntry?.Power != null & powerEntry.Power.Slottable)
                {
                    if (powerEntry.Power.GetPowerSet().SetType is Enums.ePowerSetType.Inherent)
                    {
                        if (powerEntry.Slots[0].Enhancement.Enh <= -1) continue;
                        output.AppendLine($"[color=\"#eb2a1c\"]{powerEntry.Power.DisplayName}[/color]:");
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
                                                    $"{DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].ShortName}: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#eba91c\"]{slot.Level + 1}[/color])[color=\"#757171\"],[/color]");
                                                break;
                                            case Enums.eType.SpecialO:
                                            {
                                                var specEnh = DatabaseAPI.GetSpecialEnhByIndex(enhancement.SubTypeID);
                                                output.Append(
                                                    $"{specEnh.ShortName}: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#eba91c\"]{slot.Level + 1}[/color])[color=\"#757171\"],[/color]");
                                                break;
                                            }
                                            default:
                                                output.Append(
                                                    $"{DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#eba91c\"]{slot.Level + 1}[/color])[color=\"#757171\"],[/color]");
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        switch (enhancement.TypeID)
                                        {
                                            case Enums.eType.SetO:
                                                output.Append(
                                                    $"{DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].ShortName}: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#eba91c\"] {slot.Level + 1} [/color])");
                                                break;
                                            case Enums.eType.SpecialO:
                                            {
                                                var specEnh = DatabaseAPI.GetSpecialEnhByIndex(enhancement.SubTypeID);
                                                output.Append(
                                                    $"{specEnh.ShortName}: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#eba91c\"] {slot.Level + 1} [/color])");
                                                break;
                                            }
                                            default:
                                                output.Append(
                                                    $"{DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].ShortName} ([color=\"#eba91c\"]{slot.Level + 1}[/color])");
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            output.AppendLine("");
            if (inclAccolade & !inclIncarnate)
            {
                output.AppendLine(@"[color=""#eba91c""][u].::Accolades::.[/u][/color]");
            }
            else if (!inclAccolade & inclIncarnate)
            {
                output.AppendLine(@"[color=""#eba91c""][u].::Incarnates::.[/u][/color]");
            }
            else if (inclAccolade & inclIncarnate)
            {
                output.AppendLine(@"[color=""#eba91c""][u].::Accolades & Incarnates::.[/u][/color]");
            }

            foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
            {
                if (powerEntry?.Power != null & !powerEntry.Chosen)
                {
                    switch (powerEntry.PowerSet.SetType)
                    {
                        case Enums.ePowerSetType.Accolade when inclAccolade:
                            output.AppendLine($"[color=\"#eb2a1c\"]Accolade[/color]: {powerEntry.Power.DisplayName}");
                            break;
                        case Enums.ePowerSetType.Incarnate when inclIncarnate:
                            output.AppendLine($"[color=\"#eb2a1c\"]{powerEntry.Power.GetPowerSet().DisplayName} Slot[/color]: {powerEntry.Power.DisplayName}");
                            break;

                    }
                }
            }
            output.AppendLine("[/b][/size]");
            var ret = output.ToString();
            Debug.WriteLine(ret);
        }
    }
}
