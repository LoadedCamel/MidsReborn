using Mids_Reborn.Core.Base.Master_Classes;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using HtmlAgilityPack;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;
using WebMarkupMin.Core;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;
using Mids_Reborn.Core;

namespace Mids_Reborn.Forms.ImportExportItems
{
    internal class PageBuilder
    {
        private static string HtmlBoilerPlate => MRBResourceLib.Resources.Boilerplate;
        private static string DummyBoilerPlate => MRBResourceLib.Resources.DummyBoilerPlate;
        private Dictionary<string, List<Helpers.Stat>> Stats { get; set; }

        internal struct DataLink
        {
            public string BuildLink { get; set; }
            public string ImageLink { get; set; }

            public DataLink(string buildLink, string imageLink)
            {
                BuildLink = buildLink;
                ImageLink = imageLink;
            }
        }

        public PageBuilder()
        {
            Stats = new Dictionary<string, List<Helpers.Stat>>();
            Initialize();
        }

        private void Initialize()
        {
            if (MidsContext.Character == null || MidsContext.Character.CurrentBuild == null || !MidsContext.Character.CurrentBuild.Powers.Any())
            {
                var messageBox = new MessageBoxEx("Unable to initialize builder, either the character data is null or you have not selected any powers.", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return;
            }
            Stats = Helpers.GeneratedStatData();
        }

        public string GeneratedPageData(DataLink dataLink, bool inclIncarnate, bool inclAccolade, bool inclSetBonus, bool inclBreakdown)
        {
            GenerateHtml(HtmlBoilerPlate, dataLink, inclIncarnate, inclAccolade, inclSetBonus, inclBreakdown, out var generatedHtml);
            var bytes = Encoding.UTF8.GetBytes(generatedHtml);
            var compressedBase = Compression.CompressToBase64(bytes);
            return compressedBase.OutString;
        }

        private void GenerateHtml(string boilerPlate, DataLink dataLink, bool inclIncarnate, bool inclAccolade, bool inclSetBonus, bool inclBreakdown, out string generatedHtml)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(boilerPlate);
            HtmlNode newNode;

            #region Title

            var htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//title");
            var title = !string.IsNullOrWhiteSpace(MidsContext.Character?.Name)
                ? $"{MidsContext.Character.Name} - {MidsContext.Character.Powersets[0]?.DisplayName}/{MidsContext.Character.Powersets[1]?.DisplayName} {MidsContext.Character.Archetype?.DisplayName}"
                : $"{MidsContext.Character?.Powersets[0]?.DisplayName}/{MidsContext.Character?.Powersets[1]?.DisplayName} {MidsContext.Character?.Archetype?.DisplayName}";
            htmlNode.InnerHtml = title;

            #endregion

            #region Meta

            htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='twitter:title']");
            htmlNode.Attributes["content"].Value = title;

            var description = $"Created with {MidsContext.AppName} v{MidsContext.AssemblyVersion} Rev. {MidsContext.AppFileVersion.Revision}, using the {DatabaseAPI.DatabaseName} database.";
            htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='twitter:description']");
            htmlNode.Attributes["content"].Value = description;

            htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='twitter:image']");
            htmlNode.Attributes["content"].Value = dataLink.ImageLink;

            #endregion

            #region Heading

            htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{heading}')]");
            var heading = $"<h3 class=\"generated\">This build was created with <font color=\"#B1C9F5\">{MidsContext.AppName} v{MidsContext.AssemblyVersion} Rev. {MidsContext.AppFileVersion.Revision}</font></h3>";
            heading += $"<h3 class=\"generated\">Using the <font color=\"#B1C9F5\">{DatabaseAPI.DatabaseName}</font> database <font color=\"#B1C9F5\">v{DatabaseAPI.Database.Version}</font></h3>";
            heading += "<h3 class=\"generated\">You can grab the latest MRB release from <a href=\"https://midsreborn.com\">Our Site</a> or <a href=\"https://github.com/LoadedCamel/MidsReborn/releases/latest\">GitHub</a></h3>";
            heading += "<br />";
            heading += $"<a class=\"build\" href=\"{dataLink.BuildLink}\" target=\"_blank\">Open In MidsReborn</a> or <a class=\"build\" href=\"{dataLink.BuildLink}\" target=\"_blank\">Download Build</a>";
            htmlNode.InnerHtml = heading;

            #endregion

            #region CharacterInfo

            var character = "<h2 class=\"header\">Character Info</h2>";
            character += string.IsNullOrWhiteSpace(MidsContext.Character.Name) ? $"<h3 class=\"info\">Character<font color=\"whitesmoke\">: Level {MidsContext.Character.Level + 1} {MidsContext.Character.Archetype?.Origin[MidsContext.Character.Origin]} {MidsContext.Character.Archetype?.DisplayName} ({MidsContext.Character.Alignment})</font></h3>" : $"<h3 class=\"info\">Character<font color=\"whitesmoke\">: \"{MidsContext.Character.Name}\" A Level {MidsContext.Character.Level + 1} {MidsContext.Character.Archetype?.Origin[MidsContext.Character.Origin]} {MidsContext.Character.Archetype?.DisplayName} ({MidsContext.Character.Alignment})</font></h3>";
            foreach (var set in MidsContext.Character.Powersets)
            {
                if (set != null)
                {
                    switch (set.SetType)
                    {
                        case Enums.ePowerSetType.Primary:
                            character += $"<h3 class=\"info\">Primary Power Set<font color=\"whitesmoke\">: {set.DisplayName}</font></h3>";
                            break;
                        case Enums.ePowerSetType.Secondary:
                            character += $"<h3 class=\"info\">Secondary Power Set<font color=\"whitesmoke\">: {set.DisplayName}</font></h3>";
                            break;
                        case Enums.ePowerSetType.Ancillary:
                            character += $"<h3 class=\"info\">Epic Power Pool<font color=\"whitesmoke\">: {set.DisplayName}</font></h3>";
                            break;
                        case Enums.ePowerSetType.Pool:
                            character += $"<h3 class=\"info\">Power Pool<font color=\"whitesmoke\">: {set.DisplayName}</font></h3>";
                            break;
                        case Enums.ePowerSetType.Incarnate:
                            character += $"<h3 class=\"info\">Incarnate Pool<font color=\"whitesmoke\">: {set.DisplayName}</font></h3>";
                            break;
                    }
                }
            }

            htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{character-details}')]");
            htmlNode.InnerHtml = character;
            #endregion

            #region StatTable

            htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//table/tr");
            foreach (var stat in Stats)
            {
                newNode = HtmlNode.CreateNode($"<th>{stat.Key}</th>");
                htmlNode.AppendChild(newNode);
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//table/tr/th[last()]");
            }
            for (var index = 0; index < Stats["Defense"].Count; index++)
            {
                htmlNode = htmlNode.LastChild;
                newNode = HtmlNode.CreateNode("<tr></tr>");
                htmlNode.AppendChild(newNode);

                // Process Defense
                var statValue = Stats["Defense"][index];
                //tableNode = htmlDoc.DocumentNode.SelectSingleNode("//tr");
                htmlNode = htmlNode.LastChild;
                newNode = HtmlNode.CreateNode($"<td><font color=\"whitesmoke\">{statValue.Type}:</font><font color=\"{statValue.Hex}\">{statValue.Percentage}</font></td>");
                htmlNode.AppendChild(newNode);

                // Process Resistance
                htmlNode = htmlNode.LastChild;
                newNode = HtmlNode.CreateNode(index < Stats["Resistance"].Count
                    ? $"<td><font color=\"whitesmoke\">{Stats["Resistance"][index].Type}:</font><font color=\"{Stats["Resistance"][index].Hex}\">{Stats["Resistance"][index].Percentage}</font></td>"
                    : "<td></td>");
                htmlNode.AppendChild(newNode);

                // Process Sustain
                htmlNode = htmlNode.LastChild;
                newNode = HtmlNode.CreateNode(index < Stats["Sustain"].Count
                    ? $"<td><font color=\"whitesmoke\">{Stats["Sustain"][index].Type}:</font><font color=\"{Stats["Sustain"][index].Hex}\">{Stats["Sustain"][index].Percentage}{Stats["Sustain"][index].ExtraData}</font></td>"
                    : "<td></td>");
                htmlNode.AppendChild(newNode);

                // Process Offense
                htmlNode = htmlNode.LastChild;
                newNode = HtmlNode.CreateNode(index < Stats["Offense"].Count
                    ? $"<td><font color=\"whitesmoke\">{Stats["Offense"][index].Type}:</font><font color=\"{Stats["Offense"][index].Hex}\">{Stats["Offense"][index].Percentage}</font></td>"
                    : "<td></td>");
                htmlNode.AppendChild(newNode);

                // Process Debuff Resistance
                htmlNode = htmlNode.LastChild;
                newNode = HtmlNode.CreateNode(index < Stats["Debuff Resistance"].Count
                    ? $"<td><font color=\"whitesmoke\">{Stats["Debuff Resistance"][index].Type}:</font><font color=\"{Stats["Debuff Resistance"][index].Hex}\">{Stats["Debuff Resistance"][index].Percentage}</font></td>"
                    : "<td></td>");
                htmlNode.AppendChild(newNode);
            }

            #endregion

            #region Powers

            htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{power-details}')]");
            var powers = "<h2 class=\"header\">Selected Powers</h2>";
            foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
            {
                if (powerEntry?.Power != null & powerEntry.Power.Slottable)
                {
                    if (powerEntry.Power.GetPowerSet().SetType is not Enums.ePowerSetType.Inherent)
                    {
                        if (powerEntry.Slots[0].Enhancement.Enh <= -1) continue;
                        powers += $"<h3 class=\"info\">Level {powerEntry.Level + 1}<font color=\"whitesmoke\">: {powerEntry.Power.DisplayName}<font></h3>";
                        powers += "<ul>";
                        for (var slotIndex = 0; slotIndex < powerEntry.Slots.Length; slotIndex++)
                        {
                            var slot = powerEntry.Slots[slotIndex];
                            if (slot.Enhancement.Enh > -1)
                            {
                                powers += $"<li class=\"power\">Slot Level {slot.Level + 1}<font color=\"whitesmoke\">: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].LongName}</font></li>";
                            }
                            else
                            {
                                powers += $"<li class=\"power\">Slot Level {slot.Level + 1}<font color=\"whitesmoke\">: Empty</font></li>";
                            }
                        }
                        powers += "</ul>";
                    }
                }
            }

            htmlNode.InnerHtml = powers;

            #endregion

            #region InherentPowers

            htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{inherent-details}')]");
            var inherents = "<h2 class=\"header\">Inherent Powers</h2>";
            foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
            {
                if (powerEntry?.Power != null & powerEntry.Power.Slottable)
                {
                    if (powerEntry.Power.GetPowerSet().SetType is Enums.ePowerSetType.Inherent)
                    {
                        if (powerEntry.Slots[0].Enhancement.Enh <= -1) continue;
                        inherents += $"<h3 class=\"inherent\">{powerEntry.Power.DisplayName}<font color=\"whitesmoke\">:</font></h3>";
                        inherents += "<ul>";
                        for (var slotIndex = 0; slotIndex < powerEntry.Slots.Length; slotIndex++)
                        {
                            var slot = powerEntry.Slots[slotIndex];
                            if (slot.Enhancement.Enh > -1)
                            {
                                inherents += $"<li class=\"power\">Slot Level {slot.Level + 1}<font color=\"whitesmoke\">: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].LongName}</font></li>";
                            }
                            else
                            {
                                inherents += $"<li class=\"power\">Slot Level {slot.Level + 1}<font color=\"whitesmoke\">: Empty</font></li>";
                            }
                        }
                        inherents += "</ul>";
                    }
                }
            }

            htmlNode.InnerHtml = inherents;

            #endregion

            #region Incarnates

            if (inclIncarnate)
            {
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{incarnate-details}')]");
                var incarnates = "<h2 class=\"header\">Incarnate Abilities</h2>";
                foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
                {
                    if (powerEntry?.Power != null & !powerEntry.Chosen)
                    {
                        switch (powerEntry.PowerSet.SetType)
                        {
                            case Enums.ePowerSetType.Incarnate when inclIncarnate:
                                incarnates += $"<h3 class=\"info2\">{powerEntry.Power.DisplayName} (<font color=\"ff4a47\">{powerEntry.Power.GetPowerSet().DisplayName}</font>)</h3>";
                                break;

                        }
                    }
                }
                htmlNode.InnerHtml = incarnates;
            }
            else
            {
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{incarnate-details}')]");
                RemoveBlock(htmlNode);
            }

            #endregion

            #region Accolades

            if (inclAccolade)
            {
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{accolade-details}')]");
                var accolades = "<h2 class=\"header\">Accolades</h2>";
                foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
                {
                    if (powerEntry?.Power != null & !powerEntry.Chosen)
                    {
                        switch (powerEntry.PowerSet.SetType)
                        {
                            case Enums.ePowerSetType.Accolade when inclAccolade:
                                accolades += $"<h3 class=\"info2\">{powerEntry.Power.DisplayName}</h3>";
                                break;

                        }
                    }
                }

                htmlNode.InnerHtml = accolades;
            }
            else
            {
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{accolade-details}')]");
                RemoveBlock(htmlNode);
            }

            #endregion

            #region SetBonus

            if (inclSetBonus)
            {
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{setBonus-details}')]");
                var setBonusList = MidsContext.Character.CurrentBuild.GetCumulativeSetBonuses().ToList();
                if (setBonusList.Any())
                {
                    var setBonuses = "<h2 class=\"header\">Set Bonus Totals</h2>";
                    setBonuses += "<ul>";
                    foreach (var bonus in setBonusList)
                    {
                        var setBonus = bonus.BuildEffectString(true);
                        if (setBonus.IndexOf("Endurance", StringComparison.Ordinal) > -1)
                        {
                            setBonus = setBonus.Replace("Endurance", "Endurance ");
                        }

                        setBonuses += $"<li>{setBonus}</li>";
                    }
                    htmlNode.InnerHtml = setBonuses;
                }
                else
                {
                    RemoveBlock(htmlNode);
                }
            }
            else
            {
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{setBonus-details}')]");
                RemoveBlock(htmlNode);
            }

            #endregion

            #region Breakdown

            if (inclBreakdown)
            {
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{breakdown-details}')]");
                var bonusCheck = new int[DatabaseAPI.NidPowers("set_bonus").Length];
                var setBonuses = MidsContext.Character.CurrentBuild.SetBonuses;
                if (setBonuses.Any())
                {
                    var breakdown = "<h2 class=\"header\">Set Bonus Breakdown</h2>";
                    foreach (var bonus in setBonuses)
                    {
                        var setInfo = bonus.SetInfo.ToList();
                        foreach (var info in setInfo)
                        {
                            if (info.Powers.Length <= 0) continue;
                            breakdown += $"<h3 class=\"info\">{DatabaseAPI.Database.EnhancementSets[info.SetIDX].DisplayName}<font color=\"whitesmoke\">: {MidsContext.Character.CurrentBuild.Powers[bonus.PowerIndex]?.Power?.DisplayName}</font></h3>";
                            breakdown += "<ul>";
                            var slotted = info.SlottedCount - 1;
                            for (var index = 0; index < slotted; index++)
                            {
                                var effect = DatabaseAPI.Database.EnhancementSets[info.SetIDX].GetEffectString(index, false, true);
                                var bonusItems = DatabaseAPI.Database.EnhancementSets[info.SetIDX].Bonus[index].Index.ToList();
                                foreach (var itemIndex in bonusItems)
                                {
                                    if (itemIndex <= -1) continue;
                                    ++bonusCheck[itemIndex];
                                    if (bonusCheck[itemIndex] > 5)
                                    {
                                        breakdown += $"<li>{effect} (<font color=\"ff4a47\">Exceeded the 5 Bonus Cap</font>)</li>";
                                    }
                                    else
                                    {
                                        breakdown += $"<li>{effect}</li>";
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
                                breakdown += $"<li>{effect}</li>";
                            }
                            breakdown += "</ul>";
                        }
                    }

                    htmlNode.InnerHtml = breakdown;
                }
                else
                {
                    RemoveBlock(htmlNode);
                }
            }
            else
            {
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{breakdown-details}')]");
                RemoveBlock(htmlNode);
            }

            #endregion


            using var writer = new StringWriter();
            htmlDoc.Save(writer);
            var html = writer.ToString();
            var minifier = new HtmlMinifier();
            var minifyResult = minifier.Minify(html);
            generatedHtml = minifyResult.MinifiedContent;
        }

        internal string GeneratedPreviewHtml(bool inclAccolade, bool inclIncarnate, bool inclSetBonus, bool inclBreakdown)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(DummyBoilerPlate);
            HtmlNode newNode;

            #region Title

            var htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//title");
            var title = !string.IsNullOrWhiteSpace(MidsContext.Character?.Name)
                ? $"{MidsContext.Character.Name} - {MidsContext.Character.Powersets[0]?.DisplayName}/{MidsContext.Character.Powersets[1]?.DisplayName} {MidsContext.Character.Archetype?.DisplayName}"
                : $"{MidsContext.Character?.Powersets[0]?.DisplayName}/{MidsContext.Character?.Powersets[1]?.DisplayName} {MidsContext.Character?.Archetype?.DisplayName}";
            htmlNode.InnerHtml = title;

            #endregion

            #region Heading

            htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{heading}')]");
            var heading = $"<h3 class=\"generated\">This build was created with <font color=\"#B1C9F5\">{MidsContext.AppName} v{MidsContext.AssemblyVersion} Rev. {MidsContext.AppFileVersion.Revision}</font></h3>";
            heading += $"<h3 class=\"generated\">Using the <font color=\"#B1C9F5\">{DatabaseAPI.DatabaseName}</font> database <font color=\"#B1C9F5\">v{DatabaseAPI.Database.Version}</font></h3>";
            heading += "<h3 class=\"generated\">You can grab the latest MRB release from <a href=\"https://midsreborn.com\">Our Site</a> or <a href=\"https://github.com/LoadedCamel/MidsReborn/releases/latest\">GitHub</a></h3>";
            heading += "<br />";
            heading += $"<a class=\"build\" href=\"\" target=\"_blank\">Open In MidsReborn</a>\r\nor\r\n<a class=\"build\" href=\"\" target=\"_blank\">Download Build</a>";
            htmlNode.InnerHtml = heading;

            #endregion

            #region CharacterInfo

            var character = "<h2 class=\"header\">Character Info</h2>";
            character += string.IsNullOrWhiteSpace(MidsContext.Character.Name) ? $"<h3 class=\"info\">Character<font color=\"whitesmoke\">: Level {MidsContext.Character.Level + 1} {MidsContext.Character.Archetype?.Origin[MidsContext.Character.Origin]} {MidsContext.Character.Archetype?.DisplayName} ({MidsContext.Character.Alignment})</font></h3>" : $"<h3 class=\"info\">Character<font color=\"whitesmoke\">: \"{MidsContext.Character.Name}\" A Level {MidsContext.Character.Level + 1} {MidsContext.Character.Archetype?.Origin[MidsContext.Character.Origin]} {MidsContext.Character.Archetype?.DisplayName} ({MidsContext.Character.Alignment})</font></h3>";
            foreach (var set in MidsContext.Character.Powersets)
            {
                if (set != null)
                {
                    switch (set.SetType)
                    {
                        case Enums.ePowerSetType.Primary:
                            character += $"<h3 class=\"info\">Primary Power Set<font color=\"whitesmoke\">: {set.DisplayName}</font></h3>";
                            break;
                        case Enums.ePowerSetType.Secondary:
                            character += $"<h3 class=\"info\">Secondary Power Set<font color=\"whitesmoke\">: {set.DisplayName}</font></h3>";
                            break;
                        case Enums.ePowerSetType.Ancillary:
                            character += $"<h3 class=\"info\">Epic Power Pool<font color=\"whitesmoke\">: {set.DisplayName}</font></h3>";
                            break;
                        case Enums.ePowerSetType.Pool:
                            character += $"<h3 class=\"info\">Power Pool<font color=\"whitesmoke\">: {set.DisplayName}</font></h3>";
                            break;
                        case Enums.ePowerSetType.Incarnate:
                            character += $"<h3 class=\"info\">Incarnate Pool<font color=\"whitesmoke\">: {set.DisplayName}</font></h3>";
                            break;
                    }
                }
            }

            htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{character-details}')]");
            htmlNode.InnerHtml = character;
            #endregion

            #region StatTable

            htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//table/tr");
            foreach (var stat in Stats)
            {
                newNode = HtmlNode.CreateNode($"<th>{stat.Key}</th>");
                htmlNode.AppendChild(newNode);
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//table/tr/th[last()]");
            }
            for (var index = 0; index < Stats["Defense"].Count; index++)
            {
                htmlNode = htmlNode.LastChild;
                newNode = HtmlNode.CreateNode("<tr></tr>");
                htmlNode.AppendChild(newNode);

                // Process Defense
                var statValue = Stats["Defense"][index];
                //tableNode = htmlDoc.DocumentNode.SelectSingleNode("//tr");
                htmlNode = htmlNode.LastChild;
                newNode = HtmlNode.CreateNode($"<td><font color=\"whitesmoke\">{statValue.Type}:</font> <font color=\"{statValue.Hex}\">{statValue.Percentage}</font></td>");
                htmlNode.AppendChild(newNode);

                // Process Resistance
                htmlNode = htmlNode.LastChild;
                newNode = HtmlNode.CreateNode(index < Stats["Resistance"].Count
                    ? $"<td><font color=\"whitesmoke\">{Stats["Resistance"][index].Type}:</font> <font color=\"{Stats["Resistance"][index].Hex}\">{Stats["Resistance"][index].Percentage}</font></td>"
                    : "<td></td>");
                htmlNode.AppendChild(newNode);

                // Process Sustain
                htmlNode = htmlNode.LastChild;
                newNode = HtmlNode.CreateNode(index < Stats["Sustain"].Count
                    ? $"<td><font color=\"whitesmoke\">{Stats["Sustain"][index].Type}:</font> <font color=\"{Stats["Sustain"][index].Hex}\">{Stats["Sustain"][index].Percentage}{Stats["Sustain"][index].ExtraData}</font></td>"
                    : "<td></td>");
                htmlNode.AppendChild(newNode);

                // Process Offense
                htmlNode = htmlNode.LastChild;
                newNode = HtmlNode.CreateNode(index < Stats["Offense"].Count
                    ? $"<td><font color=\"whitesmoke\">{Stats["Offense"][index].Type}:</font> <font color=\"{Stats["Offense"][index].Hex}\">{Stats["Offense"][index].Percentage}</font></td>"
                    : "<td></td>");
                htmlNode.AppendChild(newNode);

                // Process Debuff Resistance
                htmlNode = htmlNode.LastChild;
                newNode = HtmlNode.CreateNode(index < Stats["Debuff Resistance"].Count
                    ? $"<td><font color=\"whitesmoke\">{Stats["Debuff Resistance"][index].Type}:</font>  <font color=\"{Stats["Debuff Resistance"][index].Hex}\">{Stats["Debuff Resistance"][index].Percentage}</font></td>"
                    : "<td></td>");
                htmlNode.AppendChild(newNode);
            }

            #endregion

            #region Powers

            htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{power-details}')]");
            var powers = "<h2 class=\"header\">Selected Powers</h2>";
            foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
            {
                if (powerEntry?.Power != null & powerEntry.Power.Slottable)
                {
                    if (powerEntry.Power.GetPowerSet().SetType is not Enums.ePowerSetType.Inherent)
                    {
                        if (powerEntry.Slots[0].Enhancement.Enh <= -1) continue;
                        powers += $"<h3 class=\"info\">Level {powerEntry.Level + 1}<font color=\"whitesmoke\">: {powerEntry.Power.DisplayName}<font></h3>";
                        powers += "<ul>";
                        for (var slotIndex = 0; slotIndex < powerEntry.Slots.Length; slotIndex++)
                        {
                            var slot = powerEntry.Slots[slotIndex];
                            if (slot.Enhancement.Enh > -1)
                            {
                                powers += $"<li class=\"power\">Slot Level {slot.Level + 1}<font color=\"whitesmoke\">: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].LongName}</font></li>";
                            }
                            else
                            {
                                powers += $"<li class=\"power\">Slot Level {slot.Level + 1}<font color=\"whitesmoke\">: Empty</font></li>";
                            }
                        }
                        powers += "</ul>";
                    }
                }
            }

            htmlNode.InnerHtml = powers;

            #endregion

            #region InherentPowers

            htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{inherent-details}')]");
            var inherents = "<h2 class=\"header\">Inherent Powers</h2>";
            foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
            {
                if (powerEntry?.Power != null & powerEntry.Power.Slottable)
                {
                    if (powerEntry.Power.GetPowerSet().SetType is Enums.ePowerSetType.Inherent)
                    {
                        if (powerEntry.Slots[0].Enhancement.Enh <= -1) continue;
                        inherents += $"<h3 class=\"inherent\">{powerEntry.Power.DisplayName}<font color=\"whitesmoke\">:</font></h3>";
                        inherents += "<ul>";
                        for (var slotIndex = 0; slotIndex < powerEntry.Slots.Length; slotIndex++)
                        {
                            var slot = powerEntry.Slots[slotIndex];
                            if (slot.Enhancement.Enh > -1)
                            {
                                inherents += $"<li class=\"power\">Slot Level {slot.Level + 1}<font color=\"whitesmoke\">: {DatabaseAPI.Database.Enhancements[slot.Enhancement.Enh].LongName}</font></li>";
                            }
                            else
                            {
                                inherents += $"<li class=\"power\">Slot Level {slot.Level + 1}<font color=\"whitesmoke\">: Empty</font></li>";
                            }
                        }
                        inherents += "</ul>";
                    }
                }
            }

            htmlNode.InnerHtml = inherents;

            #endregion

            #region Incarnates

            if (inclIncarnate)
            {
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{incarnate-details}')]");
                var incarnates = "<h2 class=\"header\">Incarnate Abilities</h2>";
                foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
                {
                    if (powerEntry?.Power != null & !powerEntry.Chosen)
                    {
                        switch (powerEntry.PowerSet.SetType)
                        {
                            case Enums.ePowerSetType.Incarnate when inclIncarnate:
                                incarnates += $"<h3 class=\"info2\">{powerEntry.Power.DisplayName} (<font color=\"ff4a47\">{powerEntry.Power.GetPowerSet().DisplayName}</font>)</h3>";
                                break;

                        }
                    }
                }
                htmlNode.InnerHtml = incarnates;
            }
            else
            {
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{incarnate-details}')]");
                RemoveBlock(htmlNode);
            }

            #endregion

            #region Accolades

            if (inclAccolade)
            {
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{accolade-details}')]");
                var accolades = "<h2 class=\"header\">Accolades</h2>";
                foreach (var powerEntry in MidsContext.Character.CurrentBuild.Powers)
                {
                    if (powerEntry?.Power != null & !powerEntry.Chosen)
                    {
                        switch (powerEntry.PowerSet.SetType)
                        {
                            case Enums.ePowerSetType.Accolade when inclAccolade:
                                accolades += $"<h3 class=\"info2\">{powerEntry.Power.DisplayName}</h3>";
                                break;

                        }
                    }
                }

                htmlNode.InnerHtml = accolades;
            }
            else
            {
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{accolade-details}')]");
                RemoveBlock(htmlNode);
            }

            #endregion

            #region SetBonus

            if (inclSetBonus)
            {
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{setBonus-details}')]");
                var setBonusList = MidsContext.Character.CurrentBuild.GetCumulativeSetBonuses().ToList();
                if (setBonusList.Any())
                {
                    var setBonuses = "<h2 class=\"header\">Set Bonus Totals</h2>";
                    setBonuses += "<ul>";
                    foreach (var bonus in setBonusList)
                    {
                        var setBonus = bonus.BuildEffectString(true);
                        if (setBonus.IndexOf("Endurance", StringComparison.Ordinal) > -1)
                        {
                            setBonus = setBonus.Replace("Endurance", "Endurance ");
                        }

                        setBonuses += $"<li>{setBonus}</li>";
                    }
                    htmlNode.InnerHtml = setBonuses;
                }
                else
                {
                    RemoveBlock(htmlNode);
                }
            }
            else
            {
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{setBonus-details}')]");
                RemoveBlock(htmlNode);
            }

            #endregion

            #region Breakdown

            if (inclBreakdown)
            {
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{breakdown-details}')]");
                var bonusCheck = new int[DatabaseAPI.NidPowers("set_bonus").Length];
                var setBonuses = MidsContext.Character.CurrentBuild.SetBonuses;
                if (setBonuses.Any())
                {
                    var breakdown = "<h2 class=\"header\">Set Bonus Breakdown</h2>";
                    foreach (var bonus in setBonuses)
                    {
                        var setInfo = bonus.SetInfo.ToList();
                        foreach (var info in setInfo)
                        {
                            if (info.Powers.Length <= 0) continue;
                            breakdown += $"<h3 class=\"info\">{DatabaseAPI.Database.EnhancementSets[info.SetIDX].DisplayName}<font color=\"whitesmoke\">: {MidsContext.Character.CurrentBuild.Powers[bonus.PowerIndex]?.Power?.DisplayName}</font></h3>";
                            breakdown += "<ul>";
                            var slotted = info.SlottedCount - 1;
                            for (var index = 0; index < slotted; index++)
                            {
                                var effect = DatabaseAPI.Database.EnhancementSets[info.SetIDX].GetEffectString(index, false, true);
                                var bonusItems = DatabaseAPI.Database.EnhancementSets[info.SetIDX].Bonus[index].Index.ToList();
                                foreach (var itemIndex in bonusItems)
                                {
                                    if (itemIndex <= -1) continue;
                                    ++bonusCheck[itemIndex];
                                    if (bonusCheck[itemIndex] > 5)
                                    {
                                        breakdown += $"<li>{effect} (<font color=\"ff4a47\">Exceeded the 5 Bonus Cap</font>)</li>";
                                    }
                                    else
                                    {
                                        breakdown += $"<li>{effect}</li>";
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
                                breakdown += $"<li>{effect}</li>";
                            }
                            breakdown += "</ul>";
                        }
                    }

                    htmlNode.InnerHtml = breakdown;
                }
                else
                {
                    RemoveBlock(htmlNode);
                }
            }
            else
            {
                htmlNode = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(.,'{breakdown-details}')]");
                RemoveBlock(htmlNode);
            }

            #endregion


            using var writer = new StringWriter();
            htmlDoc.Save(writer);
            var html = writer.ToString();
            var minifier = new HtmlMinifier();
            var minifyResult = minifier.Minify(html);
            return minifyResult.MinifiedContent;
        }

        private static void RemoveBlock(HtmlNode node)
        {
            var previousNode = node.PreviousSibling.PreviousSibling;
            previousNode.Remove();
            node.Remove();
        }
    }
}
