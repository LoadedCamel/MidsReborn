using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core.Base.Document_Classes
{
    public class Print : IDisposable
    {
        private bool _endOfPage;
        private int _pageNumber;
        private int _pIndex;
        private bool _printingHistory;
        private bool _printingProfile;
        private bool _printingStats;

        private Dictionary<PageType, bool> _completedPrintTasks;

        private PrintWhat _sectionCompleted;
        public PrintDocument Document;

        private enum PageType
        {
            ProfileShort,
            ProfileLong,
            History,
            Stats
        }

        public Print()
        {
            Document = new PrintDocument();
            Document.PrinterSettings.DefaultPageSettings.Margins.Bottom = 25;
            Document.PrinterSettings.DefaultPageSettings.Margins.Top = 25;
            Document.PrinterSettings.DefaultPageSettings.Margins.Left = 25;
            Document.PrinterSettings.DefaultPageSettings.Margins.Right = 25;
            Document.PrinterSettings.DefaultPageSettings.Landscape = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void InitiatePrint()
        {
            if (!Document.PrinterSettings.IsValid)
            {
                MessageBox.Show($"{Document.PrinterSettings.PrinterName} is not a valid printer!");
                Document = null;
            }
            else
            {
                Document.DocumentName = string.IsNullOrEmpty(MidsContext.Character.Name)
                    ? $"{MidsContext.Character.Alignment} Plan ({MidsContext.Character.Archetype.DisplayName})"
                    : $"{MidsContext.Character.Alignment} Plan ({MidsContext.Character.Name})";
                Document.PrinterSettings.DefaultPageSettings.Margins.Bottom = 25;
                Document.PrinterSettings.DefaultPageSettings.Margins.Top = 25;
                Document.PrinterSettings.DefaultPageSettings.Margins.Left = 25;
                Document.PrinterSettings.DefaultPageSettings.Margins.Right = 25;
                Document.BeginPrint += PrintBegin;
                Document.EndPrint += PrintEnd;
                Document.PrintPage += PrintPage;
                try
                {
                    Document.Print();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while attempting to print:\n\n{ex.Message}\n\nYou should save your work, exit and then re-launch the application.");
                    Document = new PrintDocument();
                }
            }
        }

        private void PrintBegin(object sender, PrintEventArgs e)
        {
            _pageNumber = 0;
            _pIndex = 0;
            _printingProfile = MidsContext.Config.PrintProfile != ConfigData.PrintOptionProfile.None;
            _printingHistory = MidsContext.Config.PrintHistory;
            _printingStats = MidsContext.Config.PrintProfile == ConfigData.PrintOptionProfile.MultiPage;
            _sectionCompleted = PrintWhat.None;
            _completedPrintTasks = new Dictionary<PageType, bool>();
            if (MidsContext.Config.PrintProfile == ConfigData.PrintOptionProfile.SinglePage & _printingProfile)
            {
                _completedPrintTasks.Add(PageType.ProfileShort, false);
            }

            if (MidsContext.Config.PrintProfile == ConfigData.PrintOptionProfile.MultiPage & _printingProfile)
            {
                _completedPrintTasks.Add(PageType.ProfileLong, false);
            }

            if (_printingHistory)
            {
                _completedPrintTasks.Add(PageType.History, false);
            }

            if (_printingStats)
            {
                _completedPrintTasks.Add(PageType.Stats, false);
            }
        }

        private void PrintEnd(object sender, PrintEventArgs e)
        {
            Document = new PrintDocument();
        }

        private void PrintPage(object sender, PrintPageEventArgs args)
        {
            var visibleClipBounds = args.Graphics.VisibleClipBounds;
            ++_pageNumber;
            var num = PageBorder(RectConvert(visibleClipBounds), args);
            visibleClipBounds.Y += num;
            visibleClipBounds.Height -= num;
            switch (MidsContext.Config.PrintProfile)
            {
                case ConfigData.PrintOptionProfile.SinglePage when _printingProfile:
                    PrintProfileShort(RectConvert(visibleClipBounds), args);
                    break;

                case ConfigData.PrintOptionProfile.MultiPage when _printingProfile:
                    PrintProfileLong(RectConvert(visibleClipBounds), args);
                    break;

                case ConfigData.PrintOptionProfile.MultiPage when _printingStats:
                    PrintStats(RectConvert(visibleClipBounds), args);
                    break;

                default:
                    if (MidsContext.Config.PrintHistory & _printingHistory)
                    {
                        PrintHistory(RectConvert(visibleClipBounds), args);
                    }

                    break;
            }

            args.HasMorePages = !_completedPrintTasks.Values.All(e => e);
        }

        private int PageBorder(Rectangle bounds, PrintPageEventArgs args)
        {
            var solidBrush = new SolidBrush(Color.Black);
            var pen = new Pen(Color.Black, 3f);
            var top = bounds.Top;
            var format = new StringFormat(StringFormatFlags.NoClip);
            args.Graphics.DrawRectangle(pen, bounds.Left, bounds.Top, bounds.Width, bounds.Height);
            var num1 = top + 8;
            var num2 = 28;
            var font1 = new Font("Segoe UI", num2, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Near;
            var layoutRectangle = new RectangleF(bounds.Left, num1, bounds.Width, Convert.ToInt32(num2 * 1.25));
            var lvl49PowerTaken = MidsContext.Character.CurrentBuild.Powers != null &&
                MidsContext.Character.CurrentBuild.Powers
                    .Where(pe => pe.Power != null)
                    .Where(pe => pe.Level == 48)
                    .Count() > 0;
            var cLevel = (MidsContext.Character.CurrentBuild.Powers != null & lvl49PowerTaken)
                ? 50
                : Math.Min(50, MidsContext.Character.Level + 1);
            var s = string.IsNullOrEmpty(MidsContext.Character.Name)
                ? $"Level {cLevel} {MidsContext.Character.Archetype.DisplayName}"
                : $"{MidsContext.Character.Name}: Level {cLevel} {MidsContext.Character.Archetype.DisplayName}";
            args.Graphics.DrawString(s, font1, solidBrush, layoutRectangle, format);
            var num4 = num1 + 8 + num2;
            args.Graphics.DrawLine(pen, bounds.Left, num4, bounds.Left + bounds.Width, num4);
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Center;
            //var int32 = Convert.ToInt32(12.8); // 13
            layoutRectangle = new RectangleF(bounds.Left + 5.28f, bounds.Top, bounds.Width, num4 - bounds.Top);
            var font2 = new Font("Segoe UI", 13, FontStyle.Bold, GraphicsUnit.Pixel, 0);
            args.Graphics.DrawString("Page " + _pageNumber, font2, solidBrush, layoutRectangle, format);
            format.Alignment = StringAlignment.Far;
            layoutRectangle = new RectangleF(bounds.Left, bounds.Top, bounds.Width - 5.28f, num4 - bounds.Top);
            args.Graphics.DrawString($"{DateTime.Now.ToShortDateString()}\n{DateTime.Now.ToShortTimeString()}", font2,
                solidBrush, layoutRectangle, format);
            return num4 + 8;
        }

        private void PrintHistory(Rectangle bounds, PrintPageEventArgs args)
        {
            var top = bounds.Top;
            var solidBrush = new SolidBrush(Color.Black);
            var format = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip)
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near
            };
            
            var historyMapArray = MidsContext.Character.CurrentBuild.BuildHistoryMap(true, !MidsContext.Config.I9.DisablePrintIOLevels);
            var lvl = 0;
            var s = $"{MidsContext.Character.Alignment} Build History";
            var layoutRectangle = new RectangleF(bounds.Left + 15, top, bounds.Width, 12.5f);
            var font1 = new Font("Segoe UI", 10f, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Pixel);
            args.Graphics.DrawString(s, font1, solidBrush, layoutRectangle, format);
            var y1 = top + Convert.ToInt32(12.5f);
            var y2 = y1;
            var font2 = new Font("Segoe UI", 10f, FontStyle.Bold, GraphicsUnit.Pixel);
            foreach (var hItem in historyMapArray)
            {
                if (hItem.Level < 25)
                {
                    if (hItem.Level != lvl)
                    {
                        y1 += Convert.ToInt32(10f);
                        lvl = hItem.Level;
                    }

                    layoutRectangle = new RectangleF(bounds.Left + 15, y1, bounds.Width / 2 - 15, 12.5f);
                    y1 += 12;
                }
                else
                {
                    if (hItem.Level != lvl)
                    {
                        if (hItem.Level > 25)
                        {
                            y2 += 10;
                        }

                        lvl = hItem.Level;
                    }

                    layoutRectangle = new RectangleF(bounds.Left + bounds.Width / 2, y2, bounds.Width / 2f, 12.5f);
                    y2 += 12;
                }

                args.Graphics.DrawString(hItem.Text, font2, solidBrush, layoutRectangle, format);
            }

            _printingHistory = false;
            if (_completedPrintTasks.ContainsKey(PageType.History))
            {
                _completedPrintTasks[PageType.History] = true;
            }
        }

        private void PrintStats(Rectangle bounds, PrintPageEventArgs args)
        {
            var leftColumnItems = new List<string>();
            var rightColumnItems = new List<string>();

            var displayStats = MidsContext.Character.DisplayStats;

            var damageVectors = Enum.GetValues(typeof(Enums.eDamage));
            var damageVectorsNames = Enum.GetNames(typeof(Enums.eDamage));
            var excludedDefVectors = new List<Enums.eDamage>
            {
                Enums.eDamage.None,
                DatabaseAPI.RealmUsesToxicDef() ? Enums.eDamage.None : Enums.eDamage.Toxic,
                Enums.eDamage.Special,
                Enums.eDamage.Unique1,
                Enums.eDamage.Unique2,
                Enums.eDamage.Unique3
            }.Cast<int>().ToList();

            var excludedResVectors = new List<Enums.eDamage>
            {
                Enums.eDamage.None,
                Enums.eDamage.Melee,
                Enums.eDamage.Ranged,
                Enums.eDamage.AoE,
                Enums.eDamage.Special,
                Enums.eDamage.Unique1,
                Enums.eDamage.Unique2,
                Enums.eDamage.Unique3
            }.Cast<int>().ToList();

            var excludedElusivityVectors = new List<Enums.eDamage>
            {
                Enums.eDamage.Special,
                Enums.eDamage.Unique1,
                Enums.eDamage.Unique2,
                Enums.eDamage.Unique3
            }.Cast<int>().ToList();

            var mezList = new List<Enums.eMez>
            {
                Enums.eMez.Held, Enums.eMez.Stunned, Enums.eMez.Sleep, Enums.eMez.Immobilized,
                Enums.eMez.Knockback, Enums.eMez.Repel, Enums.eMez.Confused, Enums.eMez.Terrorized,
                Enums.eMez.Taunt, Enums.eMez.Placate, Enums.eMez.Teleport
            };

            var debuffEffectsList = new List<Enums.eEffectType>
            {
                Enums.eEffectType.Defense, Enums.eEffectType.Endurance, Enums.eEffectType.Recovery,
                Enums.eEffectType.PerceptionRadius, Enums.eEffectType.ToHit, Enums.eEffectType.RechargeTime,
                Enums.eEffectType.SpeedRunning, Enums.eEffectType.Regeneration
            };

            leftColumnItems.Add("- Defense -");

            for (var i = 0; i < damageVectors.Length; i++)
            {
                if (excludedDefVectors.Contains(i))
                {
                    continue;
                }

                leftColumnItems.Add($"{damageVectorsNames[i]}: {displayStats.Defense(i):##0.##}%");
            }

            rightColumnItems.Add("- Resistance -");

            for (var i = 0; i < damageVectors.Length; i++)
            {
                if (excludedResVectors.Contains(i))
                {
                    continue;
                }

                var resValue = displayStats.DamageResistance(i, false);
                rightColumnItems.Add($"{damageVectorsNames[i]}: {resValue:##0.##}%");
            }

            leftColumnItems.Add("");
            leftColumnItems.Add("- HP & Endurance -");

            var regenValue = displayStats.HealthRegenPercent(false);
            var hpValue = displayStats.HealthHitpointsNumeric(false);
            var hpBase = MidsContext.Character.Archetype.Hitpoints;
            var absorbValue = Math.Min(displayStats.Absorb, hpBase);
            var endRecValue = displayStats.EnduranceRecoveryNumeric;
            leftColumnItems.Add($"Regeneration: {regenValue:###0.##}%");
            leftColumnItems.Add($"Max HP: {hpValue:###0.##} {(absorbValue > 0 ? $" (Absorb: {absorbValue:##0.##} -- {absorbValue / hpBase * 100:##0.##}% of base HP)" : "")}");
            leftColumnItems.Add($"End Recovery: {endRecValue:##0.##}/s");
            leftColumnItems.Add($"End Use: {displayStats.EnduranceUsage:##0.##}/s End. (Net gain: {displayStats.EnduranceRecoveryNet:##0.##}/s)");
            leftColumnItems.Add($"Max End: {displayStats.EnduranceMaxEnd:##0.##}");

            rightColumnItems.Add("");
            rightColumnItems.Add("- Movement -");

            var movementUnitSpeed = clsConvertibleUnitValue.FormatSpeedUnit(MidsContext.Config.SpeedFormat);
            var movementUnitDistance = clsConvertibleUnitValue.FormatDistanceUnit(MidsContext.Config.SpeedFormat);
            var runSpdValue = displayStats.MovementRunSpeed(Enums.eSpeedMeasure.FeetPerSecond, false);
            var jumpSpdValue = displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.FeetPerSecond, false);
            var jumpHeightValue = displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond);
            var flySpeedValue = displayStats.MovementFlySpeed(Enums.eSpeedMeasure.MilesPerHour, false);
            rightColumnItems.Add($"Run Speed: {runSpdValue:##0.##} {movementUnitSpeed}");
            rightColumnItems.Add($"Jump Speed: {jumpSpdValue:##0.##} {movementUnitSpeed}");
            rightColumnItems.Add($"Jump Height: {jumpHeightValue:##0.##} {movementUnitDistance}");
            rightColumnItems.Add($"Fly Speed: {flySpeedValue:##0.##} {movementUnitSpeed}");

            leftColumnItems.Add("");
            leftColumnItems.Add("- Stealth & Perception -");
            
            leftColumnItems.Add($"Stealth (PvE): {displayStats.Distance(MidsContext.Character.Totals.StealthPvE, MidsContext.Config.SpeedFormat)} {movementUnitDistance}");
            leftColumnItems.Add($"Stealth (PvP): {displayStats.Distance(MidsContext.Character.Totals.StealthPvP, MidsContext.Config.SpeedFormat)} {movementUnitDistance}");
            leftColumnItems.Add($"Perception: {displayStats.Distance(displayStats.Perception(false), MidsContext.Config.SpeedFormat)} {movementUnitDistance}");

            rightColumnItems.Add("");
            rightColumnItems.Add("- Misc -");

            rightColumnItems.Add($"Haste: {displayStats.BuffHaste(false):##0.##}%");
            rightColumnItems.Add($"ToHit: {displayStats.BuffToHit:##0.##}%");
            rightColumnItems.Add($"Accuracy: {displayStats.BuffAccuracy:##0.##}%");
            rightColumnItems.Add($"Damage: {displayStats.BuffDamage(false):##0.##}%");
            rightColumnItems.Add($"End Rdx: {displayStats.BuffEndRdx:##0.##}%");
            rightColumnItems.Add($"Threat: {displayStats.ThreatLevel:##0.##}");

            leftColumnItems.Add("");
            leftColumnItems.Add("- Status Protection -");

            rightColumnItems.Add("");
            rightColumnItems.Add("- Status Resistance -");

            foreach (var m in mezList)
            {
                // Use Math.Abs() here instead of negative sign to prevent display of "-0"
                leftColumnItems.Add($"{m}: {Math.Abs(MidsContext.Character.Totals.Mez[(int) m]):####0.##}");
                rightColumnItems.Add($"{m}: {MidsContext.Character.Totals.MezRes[(int)m]:####0.##}%");
            }

            leftColumnItems.Add("");
            leftColumnItems.Add("- Debuff Resistance -");

            var cappedDebuffRes = debuffEffectsList.Select(e => Math.Min(
                    e == Enums.eEffectType.Defense
                        ? Statistics.MaxDefenseDebuffRes
                        : Statistics.MaxGenericDebuffRes,
                    MidsContext.Character.Totals.DebuffRes[(int)e]))
                .ToList();

            leftColumnItems.AddRange(cappedDebuffRes.Select((v, i) => $"{debuffEffectsList[i]}: {v:##0.##}%"));

            rightColumnItems.Add("");
            rightColumnItems.Add("- Elusivity (PvP) -");

            for (var i = 0; i < damageVectors.Length; i++)
            {
                if (excludedElusivityVectors.Contains(i))
                {
                    continue;
                }

                var elValue = (MidsContext.Character.Totals.Elusivity[i] + (MidsContext.Config.Inc.DisablePvE ? 0.4f : 0)) * 100;
                rightColumnItems.Add($"{damageVectorsNames[i]}: {elValue:##0.##}%");
            }

            var top = bounds.Top;
            var solidBrush = new SolidBrush(Color.Black);
            var format = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip)
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near
            };

            var layoutRectangle = new RectangleF(bounds.Left + 15, top, bounds.Width, 12.5f);
            var font1 = new Font("Segoe UI", 10f, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Pixel);
            args.Graphics.DrawString("Build Statistics", font1, solidBrush, layoutRectangle, format);
            var y1 = top + Convert.ToInt32(12.5f);
            var y2 = y1;
            var font2 = new Font("Segoe UI", 10f, FontStyle.Bold, GraphicsUnit.Pixel);

            foreach (var item in leftColumnItems)
            {
                y1 += 18;
                layoutRectangle = new RectangleF(bounds.Left + 15, y1, bounds.Width / 2 - 15, 12.5f);

                args.Graphics.DrawString(item, font2, solidBrush, layoutRectangle, format);
            }

            foreach (var item in rightColumnItems)
            {
                y2 += 18;
                layoutRectangle = new RectangleF(bounds.Left + bounds.Width / 2, y2, bounds.Width / 2f, 12.5f);

                args.Graphics.DrawString(item, font2, solidBrush, layoutRectangle, format);
            }

            _printingStats = false;
            if (_completedPrintTasks.ContainsKey(PageType.Stats))
            {
                _completedPrintTasks[PageType.Stats] = true;
            }
        }

        private static int PpInfo(Rectangle bounds, PrintPageEventArgs args)
        {
            var solidBrush = new SolidBrush(Color.Black);
            var top = bounds.Top;
            var font = new Font("Segoe UI", 12f, FontStyle.Bold, GraphicsUnit.Pixel);
            var format = new StringFormat(StringFormatFlags.NoClip)
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near
            };
            args.Graphics.DrawString($"Primary Power Set: {MidsContext.Character.Powersets[0].DisplayName}\n",
                font, solidBrush, new RectangleF(bounds.Left + 25, top, bounds.Width, 15f), format);
            var num1 = top + 15;
            args.Graphics.DrawString($"Secondary Power Set: {MidsContext.Character.Powersets[1].DisplayName}\n",
                font, solidBrush, new RectangleF(bounds.Left + 25, num1, bounds.Width, 15f), format);
            var y = num1 + 15;
            
            for (var i = 3; i < 8; i++)
            {
                if (!MidsContext.Character.PoolTaken(i)) continue;
                
                args.Graphics.DrawString($"{(i < 7 ? "Power" : "Ancillary")} Pool: {MidsContext.Character.Powersets[i].DisplayName}\n", font,
                    solidBrush, new RectangleF(bounds.Left + 25, y, bounds.Width, 15f), format);
                y += 15;
            }
            
            return y;
        }

        private void PrintProfileLong(Rectangle bounds, PrintPageEventArgs args)
        {
            var solidBrush = new SolidBrush(Color.Black);
            var vPos = bounds.Top;
            var format = new StringFormat(StringFormatFlags.NoClip)
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near
            };
            if (_pageNumber == 1)
            {
                var num = PpInfo(bounds, args) + 6;
                var font = new Font("Segoe UI", 12f, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Pixel);
                args.Graphics.DrawString($"Extended {MidsContext.Character.Alignment} Profile", font, solidBrush,
                    new RectangleF(bounds.Left + 15, num, bounds.Width, 15f), format);
                vPos = num + 15;
            }

            var font1 = new Font("Segoe UI", 12f, FontStyle.Bold, GraphicsUnit.Pixel);
            if (_sectionCompleted == PrintWhat.None)
            {
                _endOfPage = false;
                // mutates vPos
                var num = BuildPowerListLong(ref vPos, bounds, 12, PrintWhat.Powers, args);
                if (_endOfPage)
                {
                    return;
                }

                var s = "------------";
                args.Graphics.DrawString(s, font1, solidBrush, new RectangleF(bounds.Left + 15, num, bounds.Width, 15f), format);
                vPos = num + 15;
                _sectionCompleted = PrintWhat.Powers;
            }

            if (_sectionCompleted == PrintWhat.Powers)
            {
                _endOfPage = false;
                vPos = BuildPowerListLong(ref vPos, bounds, 12, PrintWhat.Inherent, args);
                if (_endOfPage)
                {
                    return;
                }

                _sectionCompleted = PrintWhat.Inherent;
                if (MidsContext.Character.Archetype.Epic)
                {
                    var s = "------------";
                    args.Graphics.DrawString(s, font1, solidBrush,
                        new RectangleF(bounds.Left + 15, vPos, bounds.Width, 15f), format);
                    vPos += 15;
                }
            }

            if (_sectionCompleted == PrintWhat.Inherent && MidsContext.Character.Archetype.Epic)
            {
                _endOfPage = false;
                BuildPowerListLong(ref vPos, bounds, 12, PrintWhat.EpicInherent, args);
                if (_endOfPage)
                {
                    return;
                }
            }

            _printingProfile = false;
            if (_completedPrintTasks.ContainsKey(PageType.ProfileLong))
            {
                _completedPrintTasks[PageType.ProfileLong] = true;
            }
        }

        private int BuildPowerListLong(
            ref int vPos,
            RectangleF bounds,
            int fontSize,
            PrintWhat selection,
            PrintPageEventArgs args)
        {
            if (_pIndex < 0)
            {
                _endOfPage = true;
                _printingProfile = false;
                return vPos;
            }

            var format = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.NoClip);
            var solidBrush = new SolidBrush(Color.Black);
            var pgIdx = -1;
            var font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            var isEnd = false;
            for (var pIndex = _pIndex; pIndex <= MidsContext.Character.CurrentBuild.Powers.Count - 1; ++pIndex)
            {
                var include = false;
                switch (selection)
                {
                    case PrintWhat.Powers:
                        if (MidsContext.Character.CurrentBuild.Powers[pIndex].Chosen)
                            include = true;
                        break;
                    case PrintWhat.Inherent:
                        if (!MidsContext.Character.CurrentBuild.Powers[pIndex].Chosen &&
                            MidsContext.Character.CurrentBuild.Powers[pIndex].Power != null)
                        {
                            include = !MidsContext.Character.CurrentBuild.Powers[pIndex].Power.IsEpic;
                            if (include && MidsContext.Character.CurrentBuild.Powers[pIndex].Slots.Length < 1)
                                include = false;
                        }

                        break;
                    case PrintWhat.EpicInherent:
                        if (!MidsContext.Character.CurrentBuild.Powers[pIndex].Chosen &&
                            MidsContext.Character.CurrentBuild.Powers[pIndex].Power != null)
                            include = MidsContext.Character.CurrentBuild.Powers[pIndex].Power.IsEpic;
                        break;
                }

                if (!MidsContext.Character.CurrentBuild.Powers[pIndex].Chosen &
                    (MidsContext.Character.CurrentBuild.Powers[pIndex].SubPowers.Length > 0))
                    include = false;
                if (!include)
                    continue;
                var levelVar = MidsContext.Character.CurrentBuild.Powers[pIndex].Level + 1;
                var s1 = "Level " + levelVar + ":";
                var s2 = MidsContext.Character.CurrentBuild.Powers[pIndex].Power != null
                    ? MidsContext.Character.CurrentBuild.Powers[pIndex].Power.DisplayName
                    : "[No Power]";
                var s3 = "";
                if (vPos - (double) bounds.Top + (MidsContext.Character.CurrentBuild.Powers[pIndex].Slots.Length + 1) *
                    fontSize * 1.25 > bounds.Height)
                {
                    pgIdx = pIndex;
                    isEnd = true;
                    break;
                }

                for (var index = 0;
                    index <= MidsContext.Character.CurrentBuild.Powers[pIndex].Slots.Length - 1;
                    ++index)
                {
                    if (!string.IsNullOrEmpty(s3))
                        s3 += '\n';
                    string str1;
                    if (index == 0)
                        str1 = s3 + "(A) ";
                    else
                        levelVar = MidsContext.Character.CurrentBuild.Powers[pIndex].Slots[index].Level + 1;
                    str1 = s3 + "(" + levelVar + ") ";
                    if (MidsContext.Character.CurrentBuild.Powers[pIndex].Slots[index].Enhancement.Enh > -1)
                    {
                        var enhancement = DatabaseAPI.Database.Enhancements[
                            MidsContext.Character.CurrentBuild.Powers[pIndex].Slots[index].Enhancement.Enh];
                        switch (enhancement.TypeID)
                        {
                            case Enums.eType.Normal:
                                var relativeString1 = Enums.GetRelativeString(
                                    MidsContext.Character.CurrentBuild.Powers[pIndex].Slots[index].Enhancement
                                        .RelativeLevel, false);
                                string str2;
                                if (!string.IsNullOrEmpty(relativeString1) & (relativeString1 != "X"))
                                    str2 = str1 + relativeString1 + " " +
                                           DatabaseAPI.Database.EnhGradeStringLong[
                                               (int) MidsContext.Character.CurrentBuild.Powers[pIndex].Slots[index]
                                                   .Enhancement.Grade] + " - ";
                                else
                                    str2 = relativeString1 != "X"
                                        ? str1 + DatabaseAPI.Database.EnhGradeStringLong[
                                            (int) MidsContext.Character.CurrentBuild.Powers[pIndex].Slots[index]
                                                .Enhancement.Grade] + " - "
                                        : str1 + "Disabled " +
                                          DatabaseAPI.Database.EnhGradeStringLong[
                                              (int) MidsContext.Character.CurrentBuild.Powers[pIndex].Slots[index]
                                                  .Enhancement.Grade] + " - ";
                                str1 = str2 + " ";
                                break;
                            case Enums.eType.SpecialO:
                                var relativeString2 = Enums.GetRelativeString(
                                    MidsContext.Character.CurrentBuild.Powers[pIndex].Slots[index].Enhancement
                                        .RelativeLevel, false);
                                string str3;
                                if (!string.IsNullOrEmpty(relativeString2) & (relativeString2 != "X"))
                                    str3 = str1 + relativeString2 + " " + enhancement.GetSpecialName() + " - ";
                                else
                                    str3 = relativeString2 != "X"
                                        ? str1 + enhancement.GetSpecialName() + " - "
                                        : str1 + "Disabled " + enhancement.GetSpecialName() + " - ";
                                str1 = str3 + " ";
                                break;
                            case Enums.eType.None:
                                break;
                            case Enums.eType.InventO:
                                break;
                            case Enums.eType.SetO:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        s3 = str1 + enhancement.LongName;
                        switch (enhancement.TypeID)
                        {
                            case Enums.eType.InventO:
                                levelVar = MidsContext.Character.CurrentBuild.Powers[pIndex].Slots[index].Enhancement
                                    .IOLevel + 1;
                                s3 = s3 + " - IO:" + levelVar;
                                continue;
                            case Enums.eType.SetO:
                                levelVar = MidsContext.Character.CurrentBuild.Powers[pIndex].Slots[index].Enhancement
                                    .IOLevel + 1;
                                s3 = s3 + " - IO:" + levelVar;
                                continue;
                            case Enums.eType.None:
                                break;
                            case Enums.eType.Normal:
                                break;
                            case Enums.eType.SpecialO:
                                //s3 = s3.Remove(0,11);
                                break;
                            default:
                                continue;
                        }
                    }
                    else
                    {
                        s3 = str1 + "[Empty]";
                    }
                }

                if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2) && string.IsNullOrEmpty(s3))
                    continue;
                var layoutRectangle = new RectangleF(bounds.Left + 15f, vPos, bounds.Width, fontSize * 1.25f);
                args.Graphics.DrawString(s1, font, solidBrush, layoutRectangle, format);
                layoutRectangle = new RectangleF(bounds.Left + 80f, vPos, bounds.Width, fontSize * 1.25f);
                args.Graphics.DrawString(s2, font, solidBrush, layoutRectangle, format);
                vPos += (int) (fontSize * 1.25);
                layoutRectangle = new RectangleF(bounds.Left + 90f, vPos, bounds.Width,
                    MidsContext.Character.CurrentBuild.Powers[pIndex].Slots.Length * fontSize * 1.25f);
                args.Graphics.DrawString(s3, font, solidBrush, layoutRectangle, format);
                vPos += (int) (fontSize * 1.25 * 1.1 +
                               fontSize * 1.25 * (MidsContext.Character.CurrentBuild.Powers[pIndex].Slots.Length - 1));
            }

            _pIndex = pgIdx;
            if (isEnd)
                _endOfPage = true;
            else
                _pIndex = 0;
            return vPos;
        }

        private void PrintProfileShort(Rectangle bounds, PrintPageEventArgs args)
        {
            _printingProfile = false;
            var solidBrush = new SolidBrush(Color.Black);
            var format = new StringFormat(StringFormatFlags.NoClip)
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near
            };

            var vPos = PpInfo(bounds, args) + 6;
            var font1 = new Font("Segoe UI", 12f, FontStyle.Bold | FontStyle.Underline, GraphicsUnit.Pixel);
            args.Graphics.DrawString(MidsContext.Character.Alignment + " Profile", font1, solidBrush,
                new RectangleF(bounds.Left + 15, vPos, bounds.Width, 15f), format);
            vPos += 15;
            var font2 = new Font("Segoe UI", 12f, FontStyle.Bold, GraphicsUnit.Pixel);
            BuildPowerListShort(ref vPos, bounds, 12, true, false, false, args);
            var s2 = "------------";
            args.Graphics.DrawString(s2, font2, solidBrush, new RectangleF(bounds.Left + 15, vPos, bounds.Width, 15f), format);
            vPos += 15;
            BuildPowerListShort(ref vPos, bounds, 12, false, true, false, args);

            if (_completedPrintTasks.ContainsKey(PageType.ProfileShort))
            {
                _completedPrintTasks[PageType.ProfileShort] = true;
            }

            if (!MidsContext.Character.Archetype.Epic)
            {
                return;
            }

            var s3 = "------------";
            args.Graphics.DrawString(s3, font2, solidBrush, new RectangleF(bounds.Left + 15, vPos, bounds.Width, 15f), format);
            vPos += 15;
            BuildPowerListShort(ref vPos, bounds, 12, false, true, true, args);
        }

        private static void BuildPowerListShort(
            ref int vPos,
            RectangleF bounds,
            int fontSize,
            bool skipInherent,
            bool skipNormal,
            bool kheldian,
            PrintPageEventArgs args)
        {
            var printIoLevels = !MidsContext.Config.I9.DisablePrintIOLevels;
            var format = new StringFormat(StringFormatFlags.NoClip);
            var solidBrush = new SolidBrush(Color.Black);
            for (var index1 = 0; index1 <= MidsContext.Character.CurrentBuild.Powers.Count - 1; ++index1)
            {
                var font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
                var isChosen = !MidsContext.Character.CurrentBuild.Powers[index1].Chosen;
                var include = false;
                if (!skipInherent && isChosen && MidsContext.Character.CurrentBuild.Powers[index1].Power != null)
                {
                    if (kheldian)
                        include = MidsContext.Character.CurrentBuild.Powers[index1].Power.IsEpic;
                    else if (MidsContext.Character.CurrentBuild.Powers[index1].Power.Requires.NPowerID.Length == 0 ||
                             !MidsContext.Character.CurrentBuild.Powers[index1].Power.Slottable)
                        include = true;
                }

                if (!skipNormal && !isChosen)
                    include = true;
                if (isChosen && MidsContext.Character.CurrentBuild.Powers[index1].PowerSet == null)
                    include = false;
                if (isChosen & (MidsContext.Character.CurrentBuild.Powers[index1].SubPowers.Length > 0))
                    include = false;
                if (!include)
                    continue;
                var s1 = $"Level {MidsContext.Character.CurrentBuild.Powers[index1].Level + 1}:";
                var layoutRectangle = new RectangleF(bounds.Left + 15f, vPos, bounds.Width, fontSize * 1.25f);
                args.Graphics.DrawString(s1, font, solidBrush, layoutRectangle, format);
                var s2 = MidsContext.Character.CurrentBuild.Powers[index1].Power != null
                    ? MidsContext.Character.CurrentBuild.Powers[index1].Power.DisplayName
                    : "[Empty]";
                layoutRectangle = new RectangleF(bounds.Left + 80f, vPos, bounds.Width, fontSize * 1.25f);
                args.Graphics.DrawString(s2, font, solidBrush, layoutRectangle, format);
                if (MidsContext.Character.CurrentBuild.Powers[index1].Slots.Length > 0)
                {
                    var str1 = string.Empty;
                    for (var index2 = 0;
                        index2 <= MidsContext.Character.CurrentBuild.Powers[index1].Slots.Length - 1;
                        ++index2)
                    {
                        if (index2 > 0)
                            str1 += ", ";
                        if (!MidsContext.Config.DisablePrintProfileEnh)
                        {
                            if (MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh > -1)
                            {
                                var enhancement = DatabaseAPI.Database.Enhancements[
                                    MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Enhancement.Enh];
                                switch (enhancement.TypeID)
                                {
                                    case Enums.eType.Normal:
                                        str1 += enhancement.ShortName;
                                        break;
                                    case Enums.eType.InventO:
                                        str1 = str1 + enhancement.ShortName + "-I";
                                        if (printIoLevels)
                                            str1 = str1 + ":" + Convert.ToString(MidsContext.Character.CurrentBuild
                                                .Powers[index1].Slots[index2].Enhancement.IOLevel + 1);
                                        break;
                                    case Enums.eType.SpecialO:
                                        string str2;

                                        var specEnh = DatabaseAPI.GetSpecialEnhByIndex(enhancement.SubTypeID);
                                        str2 = specEnh.ShortName;
                                        // Default should be X:

                                        str1 = str1 + str2 + enhancement.ShortName;
                                        break;
                                    case Enums.eType.SetO:
                                        str1 = str1 +
                                               DatabaseAPI.Database.EnhancementSets[enhancement.nIDSet].ShortName +
                                               "-" + enhancement.ShortName;
                                        if (printIoLevels)
                                            str1 = str1 + ":" + Convert.ToString(MidsContext.Character.CurrentBuild
                                                .Powers[index1].Slots[index2].Enhancement.IOLevel + 1);
                                        break;
                                    case Enums.eType.None:
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                            else
                            {
                                str1 += "Empty";
                            }
                        }

                        var str3 = str1 + "(";
                        str1 = (index2 != 0
                            ? str3 + (MidsContext.Character.CurrentBuild.Powers[index1].Slots[index2].Level + 1)
                            : str3 + "A") + ")";
                    }

                    layoutRectangle = new RectangleF(bounds.Left + 250f, vPos, bounds.Width, fontSize * 1.25f);
                    layoutRectangle.Width -= layoutRectangle.Left;
                    var sizeF = args.Graphics.MeasureString(str1, font, (int) layoutRectangle.Width * 5, format);
                    if (sizeF.Width > (double) layoutRectangle.Width)
                    {
                        var num = MidsContext.Character.CurrentBuild.Powers[index1].Slots.Length / 2;
                        var length = -1;
                        for (var index2 = 0; index2 <= num - 1; ++index2)
                            length = str1.IndexOf(", ", length + 1, StringComparison.Ordinal);
                        if (length > -1)
                        {
                            str1 = str1.Substring(0, length) + "..." + '\n' + "..." + str1.Substring(length + 2);
                            layoutRectangle.Height *= 2f;
                            vPos += (int) (fontSize * 1.25);
                        }

                        sizeF = args.Graphics.MeasureString(str1, font, (int) (layoutRectangle.Width * 5.0), format);
                        var width = sizeF.Width;
                        if (width > (double) layoutRectangle.Width)
                            font = new Font("Segoe UI", Convert.ToSingle(fontSize) * (layoutRectangle.Width / width),
                                FontStyle.Bold, GraphicsUnit.Pixel);
                    }

                    args.Graphics.DrawString(str1, font, solidBrush, layoutRectangle, format);
                }

                vPos += (int) (fontSize * 1.25 * 1.1);
            }
        }

        private static Rectangle RectConvert(RectangleF iRect)
        {
            return new Rectangle((int) iRect.X, (int) iRect.Y, (int) iRect.Width, (int) iRect.Height);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || Document == null)
                return;
            Document.Dispose();
            Document = null;
        }

        private enum PrintWhat
        {
            None = -1,
            Powers = 0,
            Inherent = 1,
            EpicInherent = 2
        }
    }
}