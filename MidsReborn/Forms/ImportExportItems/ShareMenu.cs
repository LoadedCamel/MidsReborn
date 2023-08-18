using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Extensions;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;
using static Mids_Reborn.Core.Utils.WinApi;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public partial class ShareMenu : Form
    {
        private readonly ShareConfig _share;
        private bool _isLoading = true;

        private delegate void PreviewUpdateHandler(bool inclAcc, bool inclInc, bool inclSetBonus, bool inclSetBreakdown);
        private event PreviewUpdateHandler? PreviewUpdate;

        private readonly Timer _previewTimer;
        private bool _isLabelVisible = true;

        public ShareMenu()
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            InitializeComponent();
            Icon = MRBResourceLib.Resources.MRB_Icon_Concept;
            _share = MidsContext.Config.ShareConfig;
            _previewTimer = new Timer
            {
                Interval = 500
            };
            _previewTimer.Tick += Timer_Tick;
            Load += OnLoad;
            PreviewUpdate += OnPreviewUpdate;
            formPages1.SelectedIndexChanged += FormPages1OnSelectedIndexChanged;
        }

        private void FormPages1OnSelectedIndexChanged(object? sender, int pageIndex)
        {
            if (pageIndex >= 0 && pageIndex < formPages1.Pages.Count)
            {
                switch (pageIndex)
                {
                    case 0: // BuildData Page
                        {
                            bdChunkBox.Text = ShareGenerator.GeneratedBuildDataChunk();
                            break;
                        }
                    case 1: // ForumFormats Page
                        {
                            break;
                        }
                    case 2: // InfoGraphic Page
                        {
                            igPictureBox.Image = ShareGenerator.SharedBuildImage(_share.InfoGraphic.UseAltImage);
                            break;
                        }
                    case { } when pageIndex != 3: // When not the MobileFriendly Page
                        {
                            _previewTimer.Stop();
                            break;
                        }
                    case 3 when !_previewTimer.Enabled && previewLabel.Visible: // MobileFriendly Page with conditions
                        {
                            _previewTimer.Start();
                            break;
                        }
                }
            }

            _share.LastPageIndex = pageIndex;
        }

        private async void OnLoad(object? sender, EventArgs e)
        {
            // Restyle utility windows - experimental
            StylizeWindow(Handle, Color.Silver, Color.Black, Color.WhiteSmoke);
            if (_share.LastPageIndex > -1)
            {
                formPages1.SelectedIndex = _share.LastPageIndex;
            }

            #region Page3

            await webViewPreview.EnsureCoreWebView2Async();
            previewLabel.BackColor = Color.FromArgb(75, Color.Firebrick);
            previewLabel.ForeColor = Color.FromArgb(100, Color.WhiteSmoke);

            cbInclAccolade.Checked = _share.MobileFriendly.InclAccolades;
            cbInclIncarnate.Checked = _share.MobileFriendly.InclIncarnates;
            cbInclSetBonus.Checked = _share.MobileFriendly.InclSetBonus;
            cbInclSetBreakdown.Checked = _share.MobileFriendly.InclSetBreakdown;
            PreviewUpdate?.Invoke(cbInclAccolade.Checked, cbInclIncarnate.Checked, cbInclSetBonus.Checked, cbInclSetBreakdown.Checked);
            #endregion

            #region Page2

            chkUseAltIg.Checked = _share.InfoGraphic.UseAltImage;

            #endregion

            #region Page1
            if (!_share.ForumFormat.ColorThemes.Any())
            {
                _share.ForumFormat.ResetThemes();
            }

            if (!_share.ForumFormat.FormatCodes.Any())
            {
                _share.ForumFormat.ResetCodes();
            }

            var formatsList = string.Join(", ", _share.ForumFormat.FormatCodes.Select(code => code.Name));
            if (_share.ForumFormat.FormatCodes.Count > 1)
            {
                var lastCommaIndex = formatsList.LastIndexOf(", ", StringComparison.Ordinal);
                formatsList = formatsList[..lastCommaIndex] + ", and" + formatsList[(lastCommaIndex + 1)..];
            }
            label10.Text = label10.Text.Replace("{formats}", formatsList);

            lbColorTheme.SetDataSource(_share.ForumFormat.ColorThemes);
            lbColorTheme.DisplayMember = "Name";

            lbFormatCodeType.DataSource = _share.ForumFormat.FormatCodes;
            lbFormatCodeType.DisplayMember = "Name";

            var lastSelectedThemeIndex = -1;
            var lastSelectedCodeIndex = -1;

            for (var i = 0; i < lbColorTheme.Items.Count; i++)
            {
                if (lbColorTheme.Items[i] is not ColorTheme item || item.Name != _share.ForumFormat.SelectedTheme?.Name) continue;
                lastSelectedThemeIndex = i;
                break;
            }
            lbColorTheme.SelectedIndex = lastSelectedThemeIndex;
            if (_share.ForumFormat.SelectedTheme != null)
            {
                panelColorHeadings.BackColor = _share.ForumFormat.SelectedTheme.Headings;
                panelColorLevels.BackColor = _share.ForumFormat.SelectedTheme.Levels;
                panelColorSlots.BackColor = _share.ForumFormat.SelectedTheme.Slots;
                panelColorTitle.BackColor = _share.ForumFormat.SelectedTheme.Title;
                chkCustomThemeDark.Checked = _share.ForumFormat.SelectedTheme.DarkTheme;
            }

            for (var i = 0; i < lbFormatCodeType.Items.Count; i++)
            {
                if (lbFormatCodeType.Items[i] is not FormatCode item || item.Name != _share.ForumFormat.SelectedFormatCode?.Name) continue;
                lastSelectedCodeIndex = i;
                break;
            }
            lbFormatCodeType.SelectedIndex = lastSelectedCodeIndex;

            switch (_share.ForumFormat.Filter)
            {
                case ThemeFilter.Any:
                    rbAllThemes.Checked = true;
                    break;
                case ThemeFilter.Light:
                    rbLightThemes.Checked = true;
                    break;
                case ThemeFilter.Dark:
                    rbDarkThemes.Checked = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            chkOptAccolades.Checked = _share.ForumFormat.InclAccolades;
            chkOptIncarnates.Checked = _share.ForumFormat.InclIncarnates;
            chkOptLongFormat.Checked = _share.ForumFormat.InclBonusBreakdown;
            #endregion

            _isLoading = false;
        }

        #region MobileFriendly

        private void cbInclAccolade_CheckedChanged(object sender, EventArgs e)
        {
            _share.MobileFriendly.InclAccolades = cbInclAccolade.Checked;
            PreviewUpdate?.Invoke(_share.MobileFriendly.InclAccolades, _share.MobileFriendly.InclIncarnates, _share.MobileFriendly.InclSetBonus, _share.MobileFriendly.InclSetBreakdown);
        }

        private void cbInclIncarnate_CheckedChanged(object sender, EventArgs e)
        {
            _share.MobileFriendly.InclIncarnates = cbInclIncarnate.Checked;
            PreviewUpdate?.Invoke(_share.MobileFriendly.InclAccolades, _share.MobileFriendly.InclIncarnates, _share.MobileFriendly.InclSetBonus, _share.MobileFriendly.InclSetBreakdown);
        }

        private void cbInclSetBonus_CheckedChanged(object sender, EventArgs e)
        {
            _share.MobileFriendly.InclSetBonus = cbInclSetBonus.Checked;
            PreviewUpdate?.Invoke(_share.MobileFriendly.InclAccolades, _share.MobileFriendly.InclIncarnates, _share.MobileFriendly.InclSetBonus, _share.MobileFriendly.InclSetBreakdown);
        }

        private void cbInclSetBreakdown_CheckedChanged(object sender, EventArgs e)
        {
            _share.MobileFriendly.InclSetBreakdown = cbInclSetBreakdown.Checked;
            PreviewUpdate?.Invoke(_share.MobileFriendly.InclAccolades, _share.MobileFriendly.InclIncarnates, _share.MobileFriendly.InclSetBonus, _share.MobileFriendly.InclSetBreakdown);
        }

        private void OnPreviewUpdate(bool inclAcc, bool inclInc, bool inclSetBonus, bool inclSetBreakdown)
        {
            var pageBuilder = new PageBuilder();
            var generatedHtml = pageBuilder.GeneratedPreviewHtml(inclAcc, inclInc, inclSetBonus, inclSetBreakdown);
            webViewPreview.CoreWebView2.NavigateToString(generatedHtml);
            InitializeOverlay();
        }

        private void InitializeOverlay()
        {
            _previewTimer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _isLabelVisible = !_isLabelVisible;
            previewLabel.Visible = _isLabelVisible;
        }

        #endregion

        #region ForumFormats
        // Previous HTML generator code
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (sender is ImageButtonEx button)
            {
                DataObject? data;
                switch (button.Name)
                {
                    case "bdExport":
                        data = new DataObject(bdChunkBox.Text);
                        Clipboard.SetDataObject(data, true);
                        var messageBox = new MessageBoxEx("Export Result", "The data chunk has been placed in your clipboard.", MessageBoxEx.MessageBoxButtons.Okay);
                        messageBox.ShowDialog(this);
                        break;
                    case "ffExport":
                        var exportData = ExportData(_share.ForumFormat.InclAccolades, _share.ForumFormat.InclIncarnates, _share.ForumFormat.InclBonusBreakdown);
                        data = new DataObject(exportData);
                        Clipboard.SetDataObject(data, true);
                        messageBox = new MessageBoxEx("Export Result", "The Forum data has been placed in your clipboard.", MessageBoxEx.MessageBoxButtons.Okay);
                        messageBox.ShowDialog(this);
                        break;
                    case "igExport":
                        var image = ShareGenerator.SharedBuildImage(_share.InfoGraphic.UseAltImage);
                        data = new DataObject(image);
                        Clipboard.SetDataObject(data, true);
                        messageBox = new MessageBoxEx("Export Result", "The InfoGraphic has been placed in your clipboard.", MessageBoxEx.MessageBoxButtons.Okay);
                        messageBox.ShowDialog(this);
                        break;
                    case "mbfExport":
                        ShareGenerator.ShareMobileFriendlyBuild(this, _share.MobileFriendly.InclIncarnates, _share.MobileFriendly.InclAccolades, _share.MobileFriendly.InclSetBonus, _share.MobileFriendly.InclSetBreakdown);
                        break;
                }
            }
        }

        private string ExportData(bool inclAccolade, bool inclIncarnate, bool inclBonusBreakdown)
        {
            var formatType = _share.ForumFormat.SelectedFormatCode!.Type;
            var activeTheme = _share.ForumFormat.SelectedTheme!;
            var tg = new TagGenerator(formatType, activeTheme);

            var txt = tg.Header($"{(string.IsNullOrWhiteSpace(MidsContext.Character.Name) ? "" : $"{MidsContext.Character.Name} - ")}{MidsContext.Character.Powersets[0]?.DisplayName} / {MidsContext.Character.Powersets[1]?.DisplayName} {MidsContext.Character.Alignment} {MidsContext.Character.Archetype.DisplayName}");
            var appVersionChunks = MidsContext.AssemblyFileVersion.Split('.');
            var appVersion = appVersionChunks.Length < 4
                ? MidsContext.AssemblyVersion
                : $"{appVersionChunks[0]}.{appVersionChunks[1]}.{appVersionChunks[2]} rev. {appVersionChunks[3]}";

            txt += tg.Size(6,
                tg.Bold(tg.Color(activeTheme.Headings,
                    $"{(string.IsNullOrWhiteSpace(MidsContext.Character.Name) ? "" : $"{MidsContext.Character.Name} - ")}{MidsContext.Character.Alignment} {MidsContext.Character.Archetype.DisplayName}")));
            txt += tg.BlankLine();
            txt += tg.Size(4, tg.Color(activeTheme.Levels, tg.Italic($"Build plan made with {MidsContext.AppName} v{appVersion}")));

            txt += tg.SeparatorLine();

            txt += tg.List();
            txt += tg.ListItem(tg.Bold(tg.Color(activeTheme.Headings, "Primary powerset: ") + tg.Color(activeTheme.Title, MidsContext.Character.Powersets[0]?.DisplayName)));
            txt += tg.ListItem(tg.Bold(tg.Color(activeTheme.Headings, "Secondary powerset: ") + tg.Color(activeTheme.Title, MidsContext.Character.Powersets[1]?.DisplayName)));

            var k = 1;
            for (var i = 3; i < 7; i++)
            {
                if (MidsContext.Character.Powersets[i] == null)
                {
                    continue;
                }

                txt += tg.ListItem(tg.Bold(tg.Color(activeTheme.Headings, $"Pool powerset (#{k++}): ") + tg.Color(activeTheme.Title, MidsContext.Character.Powersets[i]?.DisplayName)));
            }

            if (MidsContext.Character.Powersets[7] != null)
            {
                txt += tg.ListItem(tg.Bold(tg.Color(activeTheme.Headings, $"{(MidsContext.Character.Alignment is Enums.Alignment.Hero or Enums.Alignment.Vigilante or Enums.Alignment.Resistance ? "Epic" : "Ancillary")} powerset: ") + tg.Color(activeTheme.Title, MidsContext.Character.Powersets[7]?.DisplayName)));
            }

            txt += tg.List(true);
            txt += tg.BlankLine() + tg.SeparatorLine() + tg.BlankLine();

            txt += tg.Size(6, tg.Color(activeTheme.Headings, tg.Bold("Powers taken:")));
            txt += tg.BlankLine() + tg.BlankLine();
            foreach (var pe in MidsContext.Character.CurrentBuild.Powers)
            {
                if (pe?.Power == null)
                {
                    continue;
                }

                if (pe.Power.InherentType != Enums.eGridType.None)
                {
                    continue;
                }

                if (pe.Power.GetPowerSet()?.FullName.StartsWith("Incarnate.") == true)
                {
                    continue;
                }

                txt += tg.Bold(tg.Color(activeTheme.Levels, $"Level {pe.Level + 1}: ") + tg.Color(activeTheme.Title, pe.Power.DisplayName));
                txt += tg.BlankLine();
                //if (longFormat && pe.Slots.Length > 0)
                if (pe.Slots.Length > 0)
                {
                    txt += tg.List();
                    for (var i = 0; i < pe.Slots.Length; i++)
                    {
                        txt += tg.ListItem(tg.Color(activeTheme.Slots, pe.Slots[i].Enhancement.Enh < 0
                            ? "(Empty)"
                            : $"{tg.Color(activeTheme.Levels, pe.Slots[i].Level <= pe.Level ? "A" : $"{pe.Slots[i].Level + 1}")}: {tg.Color(activeTheme.Slots, DatabaseAPI.Database.Enhancements[pe.Slots[i].Enhancement.Enh].LongName)}"));
                    }

                    txt += tg.List(true);
                }

                txt += tg.BlankLine() + tg.BlankLine();
            }

            txt += tg.SeparatorLine() + tg.BlankLine();

            txt += tg.Size(6, tg.Color(activeTheme.Headings, tg.Bold("Inherents:")));
            txt += tg.BlankLine() + tg.BlankLine();
            foreach (var pe in MidsContext.Character.CurrentBuild.Powers)
            {
                if (pe?.Power == null)
                {
                    continue;
                }

                if (pe.Power.InherentType == Enums.eGridType.None)
                {
                    continue;
                }

                if (pe.Power.GetPowerSet()?.FullName.StartsWith("Incarnate.") == true)
                {
                    continue;
                }

                if (pe.Power.GetPowerSet()?.FullName.StartsWith("Temporary_Powers.Accolades") == true)
                {
                    continue;
                }

                txt += tg.Bold(tg.Color(activeTheme.Levels, $"Level {pe.Level + 1}: ") +
                               tg.Color(activeTheme.Title, pe.Power.DisplayName));
                txt += tg.BlankLine();
                //if (longFormat && pe.Slots.Length > 0)
                if (pe.Slots.Length > 0)
                {
                    txt += tg.List();
                    for (var i = 0; i < pe.Slots.Length; i++)
                    {
                        txt += tg.ListItem(tg.Color(activeTheme.Slots, pe.Slots[i].Enhancement.Enh < 0
                            ? "(Empty)"
                            : $"{tg.Color(activeTheme.Levels, pe.Slots[i].Level <= pe.Level ? "A" : $"{pe.Slots[i].Level + 1}")}: {tg.Color(activeTheme.Slots, DatabaseAPI.Database.Enhancements[pe.Slots[i].Enhancement.Enh].LongName)}"));
                    }

                    txt += tg.List(true);
                }

                txt += tg.BlankLine() + tg.BlankLine();
            }

            if (inclAccolade)
            {
                k = 0;
                foreach (var pe in MidsContext.Character.CurrentBuild.Powers)
                {
                    if (pe?.Power == null)
                    {
                        continue;
                    }

                    if (pe.Power.GetPowerSet()?.FullName.StartsWith("Temporary_Powers.Accolades") != true)
                    {
                        continue;
                    }

                    if (k++ == 0)
                    {
                        txt += tg.SeparatorLine();
                        txt += tg.Size(6, tg.Color(activeTheme.Headings, tg.Bold("Accolades:")));
                        txt += tg.BlankLine() + tg.BlankLine();
                    }

                    txt += tg.Bold(tg.Color(activeTheme.Title, pe.Power.DisplayName));
                    txt += tg.BlankLine();

                }

                if (k > 0)
                {
                    txt += tg.BlankLine() + tg.BlankLine();
                }
            }

            if (inclIncarnate)
            {
                k = 0;
                foreach (var pe in MidsContext.Character.CurrentBuild.Powers)
                {
                    if (pe?.Power == null)
                    {
                        continue;
                    }

                    if (pe.Power.GetPowerSet()?.FullName.StartsWith("Incarnate.") != true)
                    {
                        continue;
                    }

                    if (k++ == 0)
                    {
                        txt += tg.SeparatorLine();
                        txt += tg.Size(6, tg.Color(activeTheme.Headings, tg.Bold("Incarnates:")));
                        txt += tg.BlankLine() + tg.BlankLine();
                    }

                    txt += tg.Bold(tg.Color(activeTheme.Title, pe.Power.DisplayName));
                }
            }

            if (inclBonusBreakdown)
            {
                txt += tg.SeparatorLine();
                txt += tg.Size(6, tg.Color(activeTheme.Headings, tg.Bold("Stats Breakdown:")));
                txt += tg.BlankLine() + tg.BlankLine();

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

                txt += tg.Size(4, tg.Color(activeTheme.Headings, tg.Italic(tg.Bold("- Defense -"))));
                txt += tg.BlankLine() + tg.BlankLine();

                for (var i = 0; i < damageVectors.Length; i++)
                {
                    if (excludedDefVectors.Contains(i))
                    {
                        continue;
                    }

                    txt +=
                        $"{tg.Bold($"{damageVectorsNames[i]}:")} {tg.Color(activeTheme.Slots, $"{displayStats.Defense(i):##0.##}%")}";
                    txt += tg.BlankLine();
                }

                txt += tg.Size(4, tg.Color(activeTheme.Headings, tg.Italic(tg.Bold("- Resistance -"))));
                txt += tg.BlankLine() + tg.BlankLine();

                for (var i = 0; i < damageVectors.Length; i++)
                {
                    if (excludedResVectors.Contains(i))
                    {
                        continue;
                    }

                    var resValue = displayStats.DamageResistance(i, false);
                    txt +=
                        $"{tg.Bold($"{damageVectorsNames[i]}:")} {tg.Color(activeTheme.Slots, $"{resValue:##0.##}%")}";
                    txt += tg.BlankLine();
                }

                txt += tg.BlankLine();
                txt += tg.Size(4, tg.Color(activeTheme.Headings, tg.Italic(tg.Bold("- HP & Endurance -"))));
                txt += tg.BlankLine() + tg.BlankLine();

                var regenValue = displayStats.HealthRegenPercent(false);
                var hpValue = displayStats.HealthHitpointsNumeric(false);
                var hpBase = MidsContext.Character.Archetype.Hitpoints;
                var absorbValue = Math.Min(displayStats.Absorb, hpBase);
                var endRecValue = displayStats.EnduranceRecoveryNumeric;

                txt += $"{tg.Bold("Regeneration:")} {tg.Color(activeTheme.Slots, $"{regenValue:##0.##}%")}";
                txt += tg.BlankLine();
                txt += $"{tg.Bold("Max HP:")} {tg.Color(activeTheme.Slots, $"{hpValue:###0.##}")} | {(absorbValue > 0 ? $" ({tg.Bold("Absorb:")} {tg.Color(activeTheme.Slots, $"{absorbValue:###0.##}")} -- {tg.Color(activeTheme.Slots, $"{absorbValue / hpBase * 100:##0.##}%")} of base HP)" : "")}";
                txt += tg.BlankLine();
                txt += $"{tg.Bold("End Recovery:")} {tg.Color(activeTheme.Slots, $"{endRecValue:##0.##}/s")}";
                txt += tg.BlankLine();
                txt += $"{tg.Bold("End Use:")} {tg.Color(activeTheme.Slots, $"{displayStats.EnduranceUsage:##0.##}/s End.")} (Net gain: {tg.Color(activeTheme.Slots, $"{displayStats.EnduranceRecoveryNet:##0.##}/s")})";
                txt += tg.BlankLine();
                txt += $"{tg.Bold("Max End:")} {tg.Color(activeTheme.Slots, $"{displayStats.EnduranceMaxEnd:##0.##}")}";
                txt += tg.BlankLine();

                txt += tg.BlankLine();
                txt += tg.Size(4, tg.Color(activeTheme.Headings, tg.Italic(tg.Bold("- Movement -"))));
                txt += tg.BlankLine() + tg.BlankLine();

                var movementUnitSpeed = clsConvertibleUnitValue.FormatSpeedUnit(MidsContext.Config.SpeedFormat);
                var movementUnitDistance = clsConvertibleUnitValue.FormatDistanceUnit(MidsContext.Config.SpeedFormat);
                var runSpdValue = displayStats.MovementRunSpeed(MidsContext.Config.SpeedFormat, false);
                var jumpSpdValue = displayStats.MovementJumpSpeed(MidsContext.Config.SpeedFormat, false);
                var jumpHeightValue = displayStats.MovementJumpHeight(MidsContext.Config.SpeedFormat);
                var flySpeedValue = displayStats.MovementFlySpeed(MidsContext.Config.SpeedFormat, false);
                txt += $"{tg.Bold("Run Speed:")} {tg.Color(activeTheme.Slots, $"{runSpdValue:##0.##} {movementUnitSpeed}")}";
                txt += tg.BlankLine();
                txt += $"{tg.Bold("Jump Speed:")} {tg.Color(activeTheme.Slots, $"{jumpSpdValue:##0.##} {movementUnitSpeed}")}";
                txt += tg.BlankLine();
                txt += $"{tg.Bold("Jump Height:")} {tg.Color(activeTheme.Slots, $"{jumpHeightValue:##0.##} {movementUnitDistance}")}";
                txt += tg.BlankLine();
                txt += $"{tg.Bold("Fly Speed:")} {tg.Color(activeTheme.Slots, $"{flySpeedValue:##0.##} {movementUnitSpeed}")}";
                txt += tg.BlankLine();

                txt += tg.BlankLine();
                txt += tg.Size(4, tg.Color(activeTheme.Headings, tg.Italic(tg.Bold("- Stealth & Perception -"))));
                txt += tg.BlankLine() + tg.BlankLine();

                txt += $"{tg.Bold("Stealth (PvE):")} {tg.Color(activeTheme.Slots, $"{displayStats.Distance(MidsContext.Character.Totals.StealthPvE, MidsContext.Config.SpeedFormat):####0.##} {movementUnitDistance}")}";
                txt += tg.BlankLine();
                txt += $"{tg.Bold("Stealth (PvP):")} {tg.Color(activeTheme.Slots, $"{displayStats.Distance(MidsContext.Character.Totals.StealthPvP, MidsContext.Config.SpeedFormat):####0.##} {movementUnitDistance}")}";
                txt += tg.BlankLine();
                txt += $"{tg.Bold("Perception:")} {tg.Color(activeTheme.Slots, $"{displayStats.Distance(displayStats.Perception(false), MidsContext.Config.SpeedFormat):####0.##} {movementUnitDistance}")}";
                txt += tg.BlankLine();

                txt += tg.BlankLine();
                txt += tg.Size(4, tg.Color(activeTheme.Headings, tg.Italic(tg.Bold("- Misc -"))));
                txt += tg.BlankLine() + tg.BlankLine();

                txt += $"{tg.Bold("Haste:")} {tg.Color(activeTheme.Slots, $"{displayStats.BuffHaste(false):##0.##}%")}";
                txt += tg.BlankLine();
                txt += $"{tg.Bold("ToHit:")} {tg.Color(activeTheme.Slots, $"{displayStats.BuffToHit:##0.##}%")}";
                txt += tg.BlankLine();
                txt += $"{tg.Bold("Accuracy:")} {tg.Color(activeTheme.Slots, $"{displayStats.BuffAccuracy:##0.##}%")}";
                txt += tg.BlankLine();
                txt += $"{tg.Bold("Damage:")} {tg.Color(activeTheme.Slots, $"{displayStats.BuffDamage(false):##0.##}%")}";
                txt += tg.BlankLine();
                txt += $"{tg.Bold("End Rdx:")} {tg.Color(activeTheme.Slots, $"{displayStats.BuffEndRdx:##0.##}%")}";
                txt += tg.BlankLine();
                txt += $"{tg.Bold("Threat:")} {tg.Color(activeTheme.Slots, $"{displayStats.ThreatLevel:##0.##}")}";
                txt += tg.BlankLine();

                txt += tg.BlankLine();
                txt += tg.Size(4, tg.Color(activeTheme.Headings, tg.Italic(tg.Bold("- Status Protection -"))));
                txt += tg.BlankLine() + tg.BlankLine();

                foreach (var m in mezList)
                {
                    // Use Math.Abs() here instead of negative sign to prevent display of "-0"
                    txt += $"{tg.Bold($"{m}:")} {tg.Color(activeTheme.Slots, $"{Math.Abs(MidsContext.Character.Totals.Mez[(int) m]):####0.##}")}";
                    txt += tg.BlankLine();
                }

                txt += tg.BlankLine();
                txt += tg.Size(4, tg.Color(activeTheme.Headings, tg.Italic(tg.Bold("- Status Resistance -"))));
                txt += tg.BlankLine() + tg.BlankLine();

                foreach (var m in mezList)
                {
                    txt += $"{tg.Bold($"{m}:")} {tg.Color(activeTheme.Slots, $"{MidsContext.Character.Totals.MezRes[(int) m]:####0.##}%")}";
                    txt += tg.BlankLine();
                }

                txt += tg.BlankLine();
                txt += tg.Size(4, tg.Color(activeTheme.Headings, tg.Italic(tg.Bold("- Debuff Resistance -"))));
                txt += tg.BlankLine() + tg.BlankLine();

                var cappedDebuffRes = debuffEffectsList.Select(e => Math.Min(
                        e == Enums.eEffectType.Defense
                            ? Statistics.MaxDefenseDebuffRes
                            : Statistics.MaxGenericDebuffRes,
                        MidsContext.Character.Totals.DebuffRes[(int)e]))
                    .ToList();

                txt += cappedDebuffRes.Select((v, i) => $"{tg.Bold($"{debuffEffectsList[i]}:")} {tg.Color(activeTheme.Slots, $"{v:##0.##}%")}{tg.BlankLine()}");

                if (MidsContext.Config.Inc.DisablePvE)
                {
                    txt += tg.BlankLine();
                    txt += tg.Size(4, tg.Color(activeTheme.Headings, tg.Italic(tg.Bold("- Elusivity (PvP) -"))));
                    txt += tg.BlankLine() + tg.BlankLine();

                    for (var i = 0; i < damageVectors.Length; i++)
                    {
                        if (excludedElusivityVectors.Contains(i))
                        {
                            continue;
                        }

                        var elValue = (MidsContext.Character.Totals.Elusivity[i] + 0.4f) * 100;
                        txt += $"{tg.Bold($"{damageVectorsNames[i]}:")} {tg.Color(activeTheme.Slots, $"{elValue:##0.##}%")}";
                        txt += tg.BlankLine();
                    }
                }


                var setsEffects = "";
                var countDict = new Dictionary<int, int>();

                foreach (var s in MidsContext.Character.CurrentBuild.SetBonus)
                {
                    for (var i = 0; i < s.SetInfo.Length; i++)
                    {
                        if (s.SetInfo[i].Powers.Length <= 0)
                        {
                            continue;
                        }

                        var setInfo = s.SetInfo;
                        var enhancementSet = DatabaseAPI.Database.EnhancementSets[setInfo[i].SetIDX];
                        var str2 = setsEffects + tg.Color(activeTheme.Levels,
                            tg.Underline(tg.Bold(enhancementSet.DisplayName)));
                        if (MidsContext.Character.CurrentBuild.Powers[s.PowerIndex].NIDPowerset > -1)
                        {
                            str2 += $"{tg.BlankLine()}{tg.Color(activeTheme.Slots, $"({DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[s.PowerIndex].NIDPowerset].Powers[MidsContext.Character.CurrentBuild.Powers[s.PowerIndex].IDXPower].DisplayName})")}";
                        }

                        str2 += tg.BlankLine();
                        var str4 = tg.List(false, 2);
                        for (var j = 0; j < enhancementSet.Bonus.Length; j++)
                        {
                            if (!((setInfo[i].SlottedCount >= enhancementSet.Bonus[j].Slotted) &
                                  ((enhancementSet.Bonus[j].PvMode == Enums.ePvX.Any) |
                                   ((enhancementSet.Bonus[j].PvMode == Enums.ePvX.PvE) &
                                    !MidsContext.Config.Inc.DisablePvE) |
                                   ((enhancementSet.Bonus[j].PvMode == Enums.ePvX.PvP) &
                                    MidsContext.Config.Inc.DisablePvE))))
                            {
                                continue;
                            }

                            var localOverCap = false;
                            var str5 = enhancementSet.GetEffectString(j, false, true, true);
                            if (string.IsNullOrWhiteSpace(str5))
                            {
                                continue;
                            }

                            foreach (var esb in enhancementSet.Bonus[j].Index)
                            {
                                if (esb <= -1)
                                {
                                    continue;
                                }

                                if (!countDict.ContainsKey(esb))
                                {
                                    countDict.Add(esb, 1);
                                }
                                else
                                {
                                    countDict[esb]++;
                                }

                                if (countDict[esb] > 5)
                                {
                                    localOverCap = true;
                                }
                            }

                            if (localOverCap)
                            {
                                str5 = tg.Italic($"{str5} {tg.Color(activeTheme.Title, ">Cap")}");
                            }

                            str4 += tg.ListItem(str5);
                        }

                        foreach (var si in s.SetInfo[i].EnhIndexes)
                        {
                            var index5 = DatabaseAPI.IsSpecialEnh(si);
                            if (index5 <= -1)
                            {
                                continue;
                            }

                            var localOverCap = false;
                            var str6 = DatabaseAPI.Database.EnhancementSets[s.SetInfo[i].SetIDX].GetEffectString(index5, true, true, true);
                            foreach (var sb in DatabaseAPI.Database.EnhancementSets[s.SetInfo[i].SetIDX].SpecialBonus[index5].Index)
                            {
                                if (sb <= -1)
                                {
                                    continue;
                                }

                                if (!countDict.ContainsKey(sb))
                                {
                                    countDict.Add(sb, 1);
                                }
                                else
                                {
                                    countDict[sb]++;
                                }

                                if (countDict[sb] > 5)
                                {
                                    localOverCap = true;
                                }
                            }

                            if (localOverCap)
                            {
                                str6 = tg.Italic($"{str6} {tg.Color(activeTheme.Title, ">Cap")}");
                            }

                            str4 += tg.ListItem(str6);
                        }

                        str4 += tg.List(true);

                        setsEffects = str2 + str4 + tg.BlankLine();
                    }
                }

                var setsFxSources = "";
                var effectSources = MidsContext.Character.CurrentBuild.GetEffectSources();
                var pickedGroups = new Dictionary<Enums.eFXSubGroup, bool>();
                foreach (var fxGroup in effectSources)
                {
                    if (fxGroup.Key.EffectType == Enums.eEffectType.GrantPower |
                        fxGroup.Key.EffectType == Enums.eEffectType.EntCreate |
                        fxGroup.Key.EffectType == Enums.eEffectType.EntCreate_x |
                        fxGroup.Key.EffectType == Enums.eEffectType.Null |
                        fxGroup.Key.EffectType == Enums.eEffectType.NullBool |
                        fxGroup.Key.EffectType == Enums.eEffectType.ExecutePower)
                    {
                        continue;
                    }

                    var l2Group = fxGroup.Key.L2Group;
                    if (l2Group != Enums.eFXSubGroup.NoGroup && pickedGroups.ContainsKey(l2Group))
                    {
                        continue;
                    }

                    if (l2Group != Enums.eFXSubGroup.NoGroup)
                    {
                        pickedGroups.Add(l2Group, true);
                    }

                    var effectName = Enums.GetEffectName(fxGroup.Key.EffectType);
                    if (fxGroup.Key.MezType != Enums.eMez.None)
                    {
                        effectName += $"({Enums.GetMezName(fxGroup.Key.MezType)})";
                    }

                    if (fxGroup.Key.DamageType != Enums.eDamage.None)
                    {
                        effectName += $"({Enums.GetDamageName(fxGroup.Key.DamageType)})";
                    }

                    if (fxGroup.Key.TargetEffectType != Enums.eEffectType.None)
                    {
                        effectName += $"({Enums.GetEffectName(fxGroup.Key.TargetEffectType)})";
                    }

                    if (setsFxSources != "")
                    {
                        setsFxSources += tg.BlankLine() + tg.BlankLine();
                    }

                    var fxTypePercent = fxGroup.Key.EffectType == Enums.eEffectType.Endurance ||
                                        effectSources[fxGroup.Key].Count > 0 &&
                                        effectSources[fxGroup.Key].First().Fx.DisplayPercentage;
                    var fxSumMag = effectSources[fxGroup.Key].Count > 0
                        ? effectSources[fxGroup.Key].Sum(e => e.Mag)
                        : 0;

                    var fxBlockStr = "";
                    var fxBlockHeader = "";
                    if (l2Group != Enums.eFXSubGroup.NoGroup)
                    {
                        fxBlockHeader += fxGroup.Key.L2GroupText();
                    }
                    else
                    {
                        fxBlockHeader += Regex.Replace(fxGroup.Key.EffectType.ToString(), @"Endurance\b|Max End", "Max Endurance");
                        fxBlockHeader += fxGroup.Key.MezType != Enums.eMez.None ? $" ({fxGroup.Key.MezType})" : "";
                        fxBlockHeader += fxGroup.Key.DamageType != Enums.eDamage.None
                            ? $" ({fxGroup.Key.DamageType})"
                            : "";
                        fxBlockHeader += fxGroup.Key.TargetEffectType != Enums.eEffectType.None
                            ? $" ({fxGroup.Key.TargetEffectType})"
                            : "";
                    }

                    var petSum = 0;
                    var selfSum = 0;
                    var fxBlockHeaderTotal = fxGroup.Key.EffectType switch
                    {
                        Enums.eEffectType.GlobalChanceMod or Enums.eEffectType.EntCreate => "",
                        Enums.eEffectType.Absorb => $" ({(fxTypePercent ? fxSumMag * (fxGroup.Key.EffectType == Enums.eEffectType.Endurance ? 1 : 100) : fxSumMag):##0.##}{(fxTypePercent ? "%" : "")} Total) / {(fxTypePercent ? fxSumMag : fxSumMag / MidsContext.Character.Archetype.Hitpoints) * 100:##0.##}% of Base HP)",
                        Enums.eEffectType.HitPoints => $" ({fxSumMag:##0.##} HP Total / {fxSumMag / MidsContext.Character.Archetype.Hitpoints * 100:##0.##}% of Base HP)",
                        Enums.eEffectType.Regeneration => $" ({fxSumMag * 100:####0.##}% Total / {MidsContext.Character.DisplayStats.HealthRegenHPPerSec:####0.##} HP/s)",
                        _ => $" ({(fxTypePercent ? fxSumMag * (fxGroup.Key.EffectType == Enums.eEffectType.Endurance ? 1 : 100) : fxSumMag):##0.##}{(fxTypePercent ? "%" : "")} Total)"
                    };

                    fxBlockStr += tg.Color(activeTheme.Headings, tg.Underline(tg.Bold(fxBlockHeader))) +
                                  tg.Color(activeTheme.Levels, tg.Underline(tg.Bold(fxBlockHeaderTotal)));
                    fxBlockStr += tg.List(false, 2);

                    foreach (var e in effectSources[fxGroup.Key])
                    {
                        //if ((e.AffectedEntity & Enums.eEntity.Caster) == Enums.eEntity.None) continue;
                        //if (e.EntitiesAutoHit != Enums.eEntity.None & ((e.EntitiesAutoHit & Enums.eEntity.Caster) == Enums.eEntity.None)) continue;
                        var effectString = l2Group != Enums.eFXSubGroup.NoGroup
                            ? e.Fx.BuildEffectString(true, "", false, false, false, true)
                                .Replace(effectName, fxGroup.Key.L2GroupText())
                            : e.Fx.BuildEffectString(true, "", false, false, false, true);

                        if (!effectString.StartsWith("+"))
                        {
                            effectString = $"+{effectString}";
                        }

                        effectString = Regex.Replace(effectString, @"Endurance\b|Max End", "Max Endurance");

                        var fxSource = "";
                        if (e.EnhSet != "" & e.Power != "" & !e.IsFromEnh)
                        {
                            fxSource = tg.List(false, 4) +
                                       tg.ListItem($"(From {tg.Color(activeTheme.Levels, e.EnhSet)} in {tg.Color(activeTheme.Slots, e.Power)})") +
                                       tg.List(true);

                        }
                        else if (e is {IsFromEnh: true, Enhancement: not null})
                        {
                            fxSource = tg.List(false, 4) +
                                       tg.ListItem($"(From {tg.Color(activeTheme.Levels, e.Enhancement.LongName)} in {tg.Color(activeTheme.Slots, e.Power)})") +
                                       tg.List(true);
                        }

                        fxBlockStr += tg.ListItem($"{(e.PvMode == Enums.ePvX.PvP ? "[PVP] " : "")}{effectString}{(e.AffectedEntity == Enums.eEntity.MyPet ? " to Pets" : "")}{fxSource}");
                    }

                    fxBlockStr += tg.List(true);

                    setsFxSources += fxBlockStr;
                }

                txt += tg.BlankLine() + tg.SeparatorLine() + tg.BlankLine() +
                       tg.Size(6, tg.Color(activeTheme.Headings, tg.Bold("Set Effects Breakdown"))) +
                       tg.BlankLine() + tg.BlankLine() +
                       setsEffects +
                       tg.BlankLine() + tg.SeparatorLine() + tg.BlankLine() +
                       tg.Size(6, tg.Color(activeTheme.Headings, tg.Bold("Set Buffs Totals"))) +
                       tg.BlankLine() + tg.BlankLine() +
                       setsFxSources;
            }

            txt += tg.BlankLine() + tg.BlankLine() +
                   tg.Footer();

            return txt;
        }

        private void lbColorTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;
            _share.ForumFormat.SelectedTheme = (ColorTheme)lbColorTheme.SelectedItem;
            panelColorHeadings.BackColor = _share.ForumFormat.SelectedTheme.Headings;
            panelColorLevels.BackColor = _share.ForumFormat.SelectedTheme.Levels;
            panelColorSlots.BackColor = _share.ForumFormat.SelectedTheme.Slots;
            panelColorTitle.BackColor = _share.ForumFormat.SelectedTheme.Title;
            chkCustomThemeDark.Checked = _share.ForumFormat.SelectedTheme.DarkTheme;
        }

        private void lbFormatCodeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;
            _share.ForumFormat.SelectedFormatCode = (FormatCode)lbFormatCodeType.SelectedItem;
        }

        private void rbLightThemes_CheckedChanged(object sender, EventArgs e)
        {
            lbColorTheme.ApplyFilter(item => ((ColorTheme)item).DarkTheme is false);
            _share.ForumFormat.Filter = ThemeFilter.Light;
        }

        private void rbDarkThemes_CheckedChanged(object sender, EventArgs e)
        {
            lbColorTheme.ApplyFilter(item => ((ColorTheme)item).DarkTheme);
            _share.ForumFormat.Filter = ThemeFilter.Dark;
        }

        private void rbAllThemes_CheckedChanged(object sender, EventArgs e)
        {
            lbColorTheme.ClearFilter();
            _share.ForumFormat.Filter = ThemeFilter.Any;
        }

        private void panelColorTitle_Click(object sender, EventArgs e)
        {
            using var colorDialog = new ColorDialogExt(this, "Title Color");
            if (_share.ForumFormat.SelectedTheme != null)
            {
                colorDialog.Color = _share.ForumFormat.SelectedTheme.Title;
                var result = colorDialog.ShowDialog(this);
                if (result != DialogResult.OK) return;
                _share.ForumFormat.SelectedTheme.Title = colorDialog.Color;
            }

            panelColorTitle.BackColor = colorDialog.Color;
        }

        private void panelColorHeadings_Click(object sender, EventArgs e)
        {
            using var colorDialog = new ColorDialogExt(this, "Heading Color");
            if (_share.ForumFormat.SelectedTheme != null)
            {
                colorDialog.Color = _share.ForumFormat.SelectedTheme.Headings;
                var result = colorDialog.ShowDialog(this);
                if (result != DialogResult.OK) return;
                _share.ForumFormat.SelectedTheme.Headings = colorDialog.Color;
            }

            panelColorHeadings.BackColor = colorDialog.Color;
        }

        private void panelColorLevels_Click(object sender, EventArgs e)
        {
            using var colorDialog = new ColorDialogExt(this, "Level Color");
            if (_share.ForumFormat.SelectedTheme != null)
            {
                colorDialog.Color = _share.ForumFormat.SelectedTheme.Levels;
                var result = colorDialog.ShowDialog(this);
                if (result != DialogResult.OK) return;
                _share.ForumFormat.SelectedTheme.Levels = colorDialog.Color;
            }

            panelColorLevels.BackColor = colorDialog.Color;
        }

        private void panelColorSlots_Click(object sender, EventArgs e)
        {
            using var colorDialog = new ColorDialogExt(this, "Slot Color");
            if (_share.ForumFormat.SelectedTheme != null)
            {
                colorDialog.Color = _share.ForumFormat.SelectedTheme.Slots;
                var result = colorDialog.ShowDialog(this);
                if (result != DialogResult.OK) return;
                _share.ForumFormat.SelectedTheme.Slots = colorDialog.Color;
            }

            panelColorSlots.BackColor = colorDialog.Color;
        }

        private void chkCustomThemeDark_CheckedChanged(object sender, EventArgs e)
        {
            if (_share.ForumFormat.SelectedTheme != null) _share.ForumFormat.SelectedTheme.DarkTheme = chkCustomThemeDark.Checked;
        }

        private void chkOptLongFormat_CheckChanged(object sender, EventArgs e)
        {
            _share.ForumFormat.InclBonusBreakdown = chkOptLongFormat.Checked;
        }

        private void chkOptIncarnates_CheckChanged(object sender, EventArgs e)
        {
            _share.ForumFormat.InclIncarnates = chkOptIncarnates.Checked;
        }

        private void chkOptAccolades_CheckChanged(object sender, EventArgs e)
        {
            _share.ForumFormat.InclAccolades = chkOptAccolades.Checked;
        }

        private void addThemeBtn_Click(object sender, EventArgs e)
        {
            using var themeCreator = new ThemeCreator();
            var result = themeCreator.ShowDialog(this);
            if (result != DialogResult.Continue || themeCreator.CreatedTheme is null) return;
            _share.ForumFormat.AddTheme(themeCreator.CreatedTheme);
            lbColorTheme.Update();
            lbColorTheme.SelectedIndex = lbColorTheme.Items.Count - 1;
        }

        private void remThemBtn_Click(object sender, EventArgs e)
        {
            _share.ForumFormat.RemoveTheme(lbColorTheme.SelectedIndex);
        }

        private void resetThemeBtn_Click(object sender, EventArgs e)
        {
            _share.ForumFormat.ResetThemes();
            lbColorTheme.Update();
            lbColorTheme.SelectedIndex = 0;
        }
        #endregion

        #region SharedPage

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion

        private void chkUseAltIg_CheckChanged(object? sender, EventArgs e)
        {
            _share.InfoGraphic.UseAltImage = chkUseAltIg.Checked;
            igPictureBox.Image = ShareGenerator.SharedBuildImage(_share.InfoGraphic.UseAltImage);
        }
    }
}
