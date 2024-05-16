using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Extensions;
using Mids_Reborn.Core.Base.Master_Classes;
using MRBResourceLib;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmSetViewer : Form
    {
        #region Coord2D sub-class
        private class Coord2D
        {
            private int _X;
            private int _Y;

            public int X
            {
                get => _X;
                set => _X = value;
            }

            public int Y
            {
                get => _Y;
                set => _Y = value;
            }

            public int W
            {
                get => _X;
                set => _X = value;
            }

            public int H
            {
                get => _Y;
                set => _Y = value;
            }

            public Coord2D(int x, int y)
            {
                _X = x;
                _Y = y;
            }

            public Point ToPoint()
            {
                return new Point(_X, _Y);
            }

            public Size ToSize()
            {
                return new Size(_X, _Y);
            }
        }
        #endregion

        #region OverlayText sub-class
        private static class OverlayText
        {
            public static string Vector(Build.FXIdentifierKey idk, bool shortText=false)
            {
                var v = (idk.EffectType == Enums.eEffectType.Enhancement & idk.MezType == Enums.eMez.None & idk.TargetEffectType != Enums.eEffectType.SpeedRunning
                        ? idk.TargetEffectType
                        : idk.EffectType)
                    .ToString();
                if (!shortText) return $"{v}{(idk.EffectType == Enums.eEffectType.Enhancement ? ":" : "")}";

                v = v
                    .Replace("Resistance", "Res")
                    .Replace("Defense", "Def")
                    .Replace("EnduranceDiscount", "End. Discount")
                    .Replace("MezResist", "MezRes")
                    .Replace("Enhancement", "Enh.");
                return Regex.Replace(v, @"Endurance\b", "Max End");
            }

            public static string MezType(Build.FXIdentifierKey idk, bool shortText=false)
            {
                var m = "";
                if (shortText)
                {
                    m = idk.MezType == Enums.eMez.None
                        ? ""
                        : idk.MezType switch
                        {
                            Enums.eMez.Held => "Hold",
                            Enums.eMez.Stunned => "Stun",
                            Enums.eMez.Immobilized => "Immob.",
                            Enums.eMez.Knockback => "KB",
                            Enums.eMez.Terrorized => "Fear",
                            Enums.eMez.Teleport => "TP",
                            _ => idk.MezType.ToString()
                        };
                }
                else
                {
                    m = idk.MezType == Enums.eMez.None ? "" : idk.MezType.ToString();
                }

                if (idk.EffectType == Enums.eEffectType.Enhancement)
                {
                    if (shortText)
                    {
                        m = idk.MezType == Enums.eMez.None
                            ? ""
                            : (idk.EffectType == Enums.eEffectType.Enhancement ? " " : ": ") + idk.MezType switch
                            {
                                Enums.eMez.Held => "Hold",
                                Enums.eMez.Stunned => "Stun",
                                Enums.eMez.Immobilized => "Immob.",
                                Enums.eMez.Knockback => "KB",
                                Enums.eMez.Confused => "Confuse",
                                Enums.eMez.Terrorized => "Fear",
                                Enums.eMez.Teleport => "TP",
                                _ => idk.MezType.ToString()
                            };
                    }
                    else
                    {
                        m = idk.MezType == Enums.eMez.None
                            ? ""
                            : (idk.EffectType == Enums.eEffectType.Enhancement ? " " : ": ") + idk.MezType switch
                            {
                                Enums.eMez.Held => "Hold",
                                Enums.eMez.Stunned => "Stun",
                                Enums.eMez.Immobilized => "Immobilization",
                                Enums.eMez.Confused => "Confusion",
                                Enums.eMez.Terrorized => "Fear",
                                Enums.eMez.Teleport => "Teleportation",
                                _ => idk.MezType.ToString()
                            };
                    }
                }
                else if (m != "")
                {
                    m = $"({m})";
                }

                return m;
            }

            public static string TargetEffect(Build.FXIdentifierKey idk, bool shortText=false)
            {
                if (idk.EffectType == Enums.eEffectType.ResEffect |
                    (idk.EffectType == Enums.eEffectType.Enhancement & 
                     idk.TargetEffectType != Enums.eEffectType.None &
                     idk.TargetEffectType != Enums.eEffectType.EnduranceDiscount &
                     idk.TargetEffectType != Enums.eEffectType.Heal &
                     idk.TargetEffectType != Enums.eEffectType.Accuracy &
                     idk.TargetEffectType != Enums.eEffectType.RechargeTime))
                {
                    if (idk.EffectType == Enums.eEffectType.Enhancement & idk.TargetEffectType == Enums.eEffectType.SpeedRunning)
                    {
                        return "Slow";
                    }

                    return shortText
                        ? idk.TargetEffectType.ToString().Replace("SpeedRunning", "Run")
                        : idk.TargetEffectType.ToString();
                }

                return "";
            }

            public static string DamageType(Build.FXIdentifierKey idk, bool shortText=false)
            {
                if (!(idk.EffectType == Enums.eEffectType.Resistance | idk.EffectType == Enums.eEffectType.Defense) |
                    idk.DamageType == Enums.eDamage.None)
                {
                    return "";
                }

                return shortText
                    ? idk.DamageType switch
                    {
                        Enums.eDamage.Smashing => "S/L",
                        Enums.eDamage.Fire => "Fire/Cold",
                        Enums.eDamage.Energy => "Energy/Neg",
                        _ => idk.DamageType.ToString()
                    }
                    : idk.DamageType switch
                    {
                        Enums.eDamage.Smashing => "Smash/Lethal",
                        Enums.eDamage.Fire => "Fire/Cold",
                        Enums.eDamage.Energy => "Energy/Negative",
                        _ => idk.DamageType.ToString()
                    };
            }

            public static bool HasPercentage(Build.FXIdentifierKey idk)
            {
                return idk.EffectType != Enums.eEffectType.HitPoints & idk.EffectType != Enums.eEffectType.Endurance;
            }

            public static bool HasPlusSign(Build.FXIdentifierKey idk)
            {
                return idk.EffectType == Enums.eEffectType.SpeedRunning ||
                       idk.EffectType == Enums.eEffectType.SpeedJumping ||
                       idk.EffectType == Enums.eEffectType.SpeedFlying;
            }

            public static string ShortLabel(Build.FXIdentifierKey idk)
            {
                var overlayVector = Vector(idk, true);
                var overlayMezType = MezType(idk, true);
                var overlayTargetEffect = TargetEffect(idk, true);
                var overlayDmgType = DamageType(idk, true);
                var overlayValuePercent = HasPercentage(idk);

                //overlayDmgType = overlayDmgType != "" ? $"{overlayDmgType} " : "";
                overlayTargetEffect = overlayTargetEffect switch
                {
                    "" => "",
                    "Slow" => " Slow",
                    _ => $"({overlayTargetEffect})"
                };

                return $"{(overlayDmgType != "" ? overlayDmgType + " " : "")}{overlayVector}{overlayMezType}{overlayTargetEffect}:";
            }
        }
        #endregion

        private readonly frmMain myParent;
        private ImageButton btnClose;
        private ImageButton btnSmall;
        private ImageButton chkOnTop;
        private ColumnHeader ColumnHeader1;
        private ColumnHeader ColumnHeader2;
        private ColumnHeader ColumnHeader3;
        private ImageList ilSet;
        private Label Label1;
        private Label Label2;

        private ListView lstSets;
        private RichTextBox rtApplied;
        private RichTextBox rtxtFX;
        private RichTextBox rtxtInfo;

        private Dictionary<string, Build.FXIdentifierKey> BarsFX;
        private readonly Dictionary<string, Dictionary<string, Coord2D[]>> ShrinkExpandItemsPos;
        private readonly List<BarSettings> EffectListOrder;


        public frmSetViewer(frmMain iParent)
        {
            Move += frmSetViewer_Move;
            FormClosed += frmSetViewer_FormClosed;
            Load += frmSetViewer_Load;
            InitializeComponent();
            //var componentResourceManager = new ComponentResourceManager(typeof(frmSetViewer));
            Icon = Resources.MRB_Icon_Concept;
            Name = nameof(frmSetViewer);
            myParent = iParent;
            BarsFX = new Dictionary<string, Build.FXIdentifierKey>();

            #region Shrunk/expanded control positions and sizes
            ShrinkExpandItemsPos = new Dictionary<string, Dictionary<string, Coord2D[]>>();
            ShrinkExpandItemsPos.Add("this", new Dictionary<string, Coord2D[]>());
            ShrinkExpandItemsPos["this"].Add("Size", new []
            {
                new Coord2D(panelBars.Left + 20, Height),
                new Coord2D(panelBars.Right + 20, Height)
            });

            ShrinkExpandItemsPos.Add("rtxtInfo", new Dictionary<string, Coord2D[]>());
            ShrinkExpandItemsPos["rtxtInfo"].Add("Size", new []
            {
                new Coord2D(rtxtInfo.Width, 109),
                new Coord2D(rtxtInfo.Width, 132)
            });

            ShrinkExpandItemsPos.Add("btnDetailFx", new Dictionary<string, Coord2D[]>());
            ShrinkExpandItemsPos["btnDetailFx"].Add("Location", new []
            {
                new Coord2D(9, 3),
                new Coord2D(0, 3)
            });

            ShrinkExpandItemsPos.Add("btnSmall", new Dictionary<string, Coord2D[]>());
            ShrinkExpandItemsPos["btnSmall"].Add("Location", new[]
            {
                new Coord2D(117, 3),
                new Coord2D(0, 31)
            });

            ShrinkExpandItemsPos.Add("chkOnTop", new Dictionary<string, Coord2D[]>());
            ShrinkExpandItemsPos["chkOnTop"].Add("Location", new[]
            {
                new Coord2D(281, 3),
                new Coord2D(224, 3)
            });

            ShrinkExpandItemsPos.Add("btnClose", new Dictionary<string, Coord2D[]>());
            ShrinkExpandItemsPos["btnClose"].Add("Location", new[]
            {
                new Coord2D(389, 3),
                new Coord2D(224, 31)
            });

            ShrinkExpandItemsPos.Add("panelButtons", new Dictionary<string, Coord2D[]>());
            ShrinkExpandItemsPos["panelButtons"].Add("Size", new[]
            {
                new Coord2D(516, 30),
                new Coord2D(329, 53)
            });
            ShrinkExpandItemsPos["panelButtons"].Add("Location", new[]
            {
                new Coord2D(0, rtxtInfo.Bottom - 20),
                new Coord2D(panelBars.Left, panelBars.Bottom)
            });
            #endregion

            #region Bar settings
            EffectListOrder = new List<BarSettings>
            {
                new BarSettings (Enums.eEffectType.DamageBuff, Color.Red, Color.FromArgb(204, 0, 0)),
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eEffectType.Accuracy, Color.Yellow, Color.FromArgb(204, 204, 0)),
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eEffectType.RechargeTime, Color.FromArgb(255, 128, 0), Color.FromArgb(204, 102, 0)),
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eEffectType.Range, Color.FromArgb(170, 168, 179), Color.FromArgb(121, 120, 128)),
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eEffectType.Heal, Color.FromArgb(116, 255, 116), Color.FromArgb(92, 204, 92)),

                new BarSettings (Enums.eEffectType.Regeneration, Color.FromArgb(64, 255, 64), Color.FromArgb(51, 204, 51)),
                new BarSettings (Enums.eEffectType.HitPoints, Color.FromArgb(44, 180, 44), Color.FromArgb(31, 130, 31)),
                new BarSettings (Enums.eEffectType.Recovery, Color.DodgerBlue, Color.FromArgb(24, 114, 204)),
                new BarSettings (Enums.eEffectType.Endurance, Color.FromArgb(59, 158, 255), Color.FromArgb(47, 125, 204)),
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eEffectType.EnduranceDiscount, Color.RoyalBlue, Color.FromArgb(50, 81, 173)),

                new BarSettings (Enums.eEffectType.Resistance, Enums.eDamage.Smashing, Color.FromArgb(0, 192, 192), Color.FromArgb(0, 140, 140)),
                new BarSettings (Enums.eEffectType.Resistance, Enums.eDamage.Fire, Color.FromArgb(0, 192, 192), Color.FromArgb(0, 140, 140)),
                new BarSettings (Enums.eEffectType.Resistance, Enums.eDamage.Energy, Color.FromArgb(0, 192, 192), Color.FromArgb(0, 140, 140)),
                new BarSettings (Enums.eEffectType.Resistance, Enums.eDamage.Toxic, Color.FromArgb(0, 192, 192), Color.FromArgb(0, 140, 140)),
                new BarSettings (Enums.eEffectType.Resistance, Enums.eDamage.Psionic, Color.FromArgb(0, 192, 192), Color.FromArgb(0, 140, 140)),

                new BarSettings (Enums.eEffectType.Defense, Enums.eDamage.Smashing, Color.Magenta, Color.FromArgb(204, 0, 204)),
                new BarSettings (Enums.eEffectType.Defense, Enums.eDamage.Fire, Color.Magenta, Color.FromArgb(204, 0, 204)),
                new BarSettings (Enums.eEffectType.Defense, Enums.eDamage.Energy, Color.Magenta, Color.FromArgb(204, 0, 204)),
                new BarSettings (Enums.eEffectType.Defense, Enums.eDamage.Psionic, Color.Magenta, Color.FromArgb(204, 0, 204)),
                new BarSettings (Enums.eEffectType.Defense, Enums.eDamage.Melee, Color.Magenta, Color.FromArgb(204, 0, 204)),
                new BarSettings (Enums.eEffectType.Defense, Enums.eDamage.Ranged, Color.Magenta, Color.FromArgb(204, 0, 204)),
                new BarSettings (Enums.eEffectType.Defense, Enums.eDamage.AoE, Color.Magenta, Color.FromArgb(204, 0, 204)),
                new BarSettings (Enums.eEffectType.Elusivity, Color.FromArgb(163, 1, 231), Color.FromArgb(127, 1, 181)),

                new BarSettings (Enums.eEffectType.Enhancement, Enums.eMez.Held, Color.FromArgb(100, 70, 85), Color.FromArgb(48, 34, 41)),
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eMez.Stunned, Color.FromArgb(100, 70, 85), Color.FromArgb(48, 34, 41)),
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eMez.Sleep, Color.FromArgb(100, 70, 85), Color.FromArgb(48, 34, 41)),
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eMez.Immobilized, Color.FromArgb(100, 70, 85), Color.FromArgb(48, 34, 41)),
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eMez.Knockback, Color.FromArgb(100, 70, 85), Color.FromArgb(48, 34, 41)),
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eMez.Confused, Color.FromArgb(100, 70, 85), Color.FromArgb(48, 34, 41)),
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eMez.Terrorized, Color.FromArgb(100, 70, 85), Color.FromArgb(48, 34, 41)),

                new BarSettings (Enums.eEffectType.MezResist, Enums.eMez.Held, Color.FromArgb(90, 90, 120), Color.FromArgb(52, 52, 69)),
                new BarSettings (Enums.eEffectType.MezResist, Enums.eMez.Stunned, Color.FromArgb(90, 90, 120), Color.FromArgb(52, 52, 69)),
                new BarSettings (Enums.eEffectType.MezResist, Enums.eMez.Sleep, Color.FromArgb(90, 90, 120), Color.FromArgb(52, 52, 69)),
                new BarSettings (Enums.eEffectType.MezResist, Enums.eMez.Immobilized, Color.FromArgb(90, 90, 120), Color.FromArgb(52, 52, 69)),
                new BarSettings (Enums.eEffectType.MezResist, Enums.eMez.Knockback, Color.FromArgb(90, 90, 120), Color.FromArgb(52, 52, 69)),
                new BarSettings (Enums.eEffectType.MezResist, Enums.eMez.Confused, Color.FromArgb(90, 90, 120), Color.FromArgb(52, 52, 69)),
                new BarSettings (Enums.eEffectType.MezResist, Enums.eMez.Terrorized, Color.FromArgb(90, 90, 120), Color.FromArgb(52, 52, 69)),
                new BarSettings (Enums.eEffectType.MezResist, Enums.eMez.Teleport, Color.FromArgb(90, 90, 120), Color.FromArgb(52, 52, 69)),

                new BarSettings (Enums.eEffectType.SpeedRunning, Color.FromArgb(0, 192, 128), Color.FromArgb(0, 140, 94)),
                //new BarSettings (Enums.eEffectType.MaxRunSpeed, Color.FromArgb(0, 192, 128), Color.FromArgb(0, 140, 94)),
                new BarSettings (Enums.eEffectType.SpeedJumping, Color.FromArgb(0, 192, 128), Color.FromArgb(0, 140, 94)),
                //new BarSettings (Enums.eEffectType.JumpHeight, Color.FromArgb(0, 192, 128), Color.FromArgb(0, 140, 94)),
                //new BarSettings (Enums.eEffectType.MaxJumpSpeed, Color.FromArgb(0, 192, 128), Color.FromArgb(0, 140, 94)),
                new BarSettings (Enums.eEffectType.SpeedFlying, Color.FromArgb(0, 192, 128), Color.FromArgb(0, 140, 94)),
                //new BarSettings (Enums.eEffectType.MaxFlySpeed, Color.FromArgb(0, 192, 128), Color.FromArgb(0, 140, 94)),
                new BarSettings (Enums.eEffectType.ResEffect, Enums.eEffectType.SpeedRunning, Color.FromArgb(90, 120, 110), Color.FromArgb(52, 69, 63)),
                // Enhancement (slow)
                new BarSettings (Enums.eEffectType.Enhancement, Enums.eEffectType.SpeedRunning, Color.FromArgb(90, 120, 110), Color.FromArgb(52, 69, 63)),

                new BarSettings (Enums.eEffectType.StealthRadius, Color.FromArgb(106, 121, 136), Color.FromArgb(84, 95, 107)),
                new BarSettings (Enums.eEffectType.StealthRadiusPlayer, Color.FromArgb(106, 121, 136), Color.FromArgb(84, 95, 107)),
                new BarSettings (Enums.eEffectType.PerceptionRadius, Color.FromArgb(106, 121, 136), Color.FromArgb(84, 95, 107))
            };

            EffectListOrder = EffectListOrder
                .AsEnumerable()
                .OrderBy(e => (int)new Build.FXIdentifierKey
                {
                    EffectType = e.EffectType,
                    TargetEffectType = e.TargetEffectType,
                    DamageType = e.DamageType,
                    MezType = e.MezType
                }.L1Group)
                .ToList();
            #endregion
        }

        private void btnClose_Click()
        {
            Close();
        }

        private void btnSmall_Click()
        {
            const int ntk = 15;
            var idx = Width > 700 ? 1 : 0;
            var tk = 0;
            var timer = new Timer {Interval = 12};
            timer.Tick += (sender, e) =>
            {
                Width = ShrinkExpandItemsPos["this"]["Size"][idx].W + (ShrinkExpandItemsPos["this"]["Size"][1 - idx].W - ShrinkExpandItemsPos["this"]["Size"][idx].W) / ntk * tk;
                rtxtInfo.Height = ShrinkExpandItemsPos["rtxtInfo"]["Size"][idx].H + (ShrinkExpandItemsPos["rtxtInfo"]["Size"][1 - idx].H - ShrinkExpandItemsPos["rtxtInfo"]["Size"][idx].H) / ntk * tk;
                btnDetailFx.Location = new Point(
                    ShrinkExpandItemsPos["btnDetailFx"]["Location"][idx].X + (ShrinkExpandItemsPos["btnDetailFx"]["Location"][1 - idx].X - ShrinkExpandItemsPos["btnDetailFx"]["Location"][idx].X) / ntk * tk,
                    ShrinkExpandItemsPos["btnDetailFx"]["Location"][idx].Y + (ShrinkExpandItemsPos["btnDetailFx"]["Location"][1 - idx].Y - ShrinkExpandItemsPos["btnDetailFx"]["Location"][idx].Y) / ntk * tk);
                btnSmall.Location = new Point(
                    ShrinkExpandItemsPos["btnSmall"]["Location"][idx].X + (ShrinkExpandItemsPos["btnSmall"]["Location"][1 - idx].X - ShrinkExpandItemsPos["btnSmall"]["Location"][idx].X) / ntk * tk,
                    ShrinkExpandItemsPos["btnSmall"]["Location"][idx].Y + (ShrinkExpandItemsPos["btnSmall"]["Location"][1 - idx].Y - ShrinkExpandItemsPos["btnSmall"]["Location"][idx].Y) / ntk * tk);
                chkOnTop.Location = new Point(
                    ShrinkExpandItemsPos["chkOnTop"]["Location"][idx].X + (ShrinkExpandItemsPos["chkOnTop"]["Location"][1 - idx].X - ShrinkExpandItemsPos["chkOnTop"]["Location"][idx].X) / ntk * tk,
                    ShrinkExpandItemsPos["chkOnTop"]["Location"][idx].Y + (ShrinkExpandItemsPos["chkOnTop"]["Location"][1 - idx].Y - ShrinkExpandItemsPos["chkOnTop"]["Location"][idx].Y) / ntk * tk);
                btnClose.Location = new Point(
                    ShrinkExpandItemsPos["btnClose"]["Location"][idx].X + (ShrinkExpandItemsPos["btnClose"]["Location"][1 - idx].X - ShrinkExpandItemsPos["btnClose"]["Location"][idx].X) / ntk * tk,
                    ShrinkExpandItemsPos["btnClose"]["Location"][idx].Y + (ShrinkExpandItemsPos["btnClose"]["Location"][1 - idx].Y - ShrinkExpandItemsPos["btnClose"]["Location"][idx].Y) / ntk * tk);

                panelButtons.Location = new Point(
                    ShrinkExpandItemsPos["panelButtons"]["Location"][idx].X + (ShrinkExpandItemsPos["panelButtons"]["Location"][1 - idx].X - ShrinkExpandItemsPos["panelButtons"]["Location"][idx].X) / ntk * tk,
                    ShrinkExpandItemsPos["panelButtons"]["Location"][idx].Y + (ShrinkExpandItemsPos["panelButtons"]["Location"][1 - idx].Y - ShrinkExpandItemsPos["panelButtons"]["Location"][idx].Y) / ntk * tk);

                panelButtons.Size = new Size(
                    ShrinkExpandItemsPos["panelButtons"]["Size"][idx].W + (ShrinkExpandItemsPos["panelButtons"]["Size"][1 - idx].W - ShrinkExpandItemsPos["panelButtons"]["Size"][idx].W) / ntk * tk,
                    ShrinkExpandItemsPos["panelButtons"]["Size"][idx].H + (ShrinkExpandItemsPos["panelButtons"]["Size"][1 - idx].H - ShrinkExpandItemsPos["panelButtons"]["Size"][idx].H) / ntk * tk);

                var tint = Math.Max(0, Math.Min(255, idx == 1 ? 255 - (int) Math.Floor(tk * 255.0 / ntk) : (int) Math.Ceiling(tk * 255.0 / ntk)));
                Label1.ForeColor = Color.FromArgb(tint, tint, tint);

                if (tk++ < ntk) return;

                timer.Stop();
                Width = ShrinkExpandItemsPos["this"]["Size"][1 - idx].W;
                rtxtInfo.Height = ShrinkExpandItemsPos["rtxtInfo"]["Size"][1 - idx].H;
                btnDetailFx.Location = ShrinkExpandItemsPos["btnDetailFx"]["Location"][1 - idx].ToPoint();
                btnSmall.Location = ShrinkExpandItemsPos["btnSmall"]["Location"][1 - idx].ToPoint();
                chkOnTop.Location = ShrinkExpandItemsPos["chkOnTop"]["Location"][1 - idx].ToPoint();
                btnClose.Location = ShrinkExpandItemsPos["btnClose"]["Location"][1 - idx].ToPoint();
                panelButtons.Size = ShrinkExpandItemsPos["panelButtons"]["Size"][1 - idx].ToSize();
                panelButtons.Location = ShrinkExpandItemsPos["panelButtons"]["Location"][1 - idx].ToPoint();
                Label1.ForeColor = idx == 1 ? Color.Black : Color.White;
                btnSmall.TextOff = idx == 1 ? "Expand >>" : "<< Shrink";
                panelBars.Visible = idx == 0 & btnDetailFx.Checked;
                rtxtFX.Visible = idx == 0 & !btnDetailFx.Checked;
            };

            timer.Start();

            StoreLocation();
        }

        private void chkOnTop_CheckedChanged()
        {
            TopMost = chkOnTop.Checked;
        }

        private void DisplayList()
        {
            var items = new string[3];
            lstSets.BeginUpdate();
            lstSets.Items.Clear();
            var imageIndex = -1;
            FillImageList();
            foreach (var s in MidsContext.Character.CurrentBuild.SetBonuses)
            {
                for (var i = 0; i < s.SetInfo.Length; i++)
                {
                    var setInfo = s.SetInfo;
                    
                    items[0] = DatabaseAPI.Database.EnhancementSets[setInfo[i].SetIDX].DisplayName;
                    items[1] =
                        MidsContext.Character.CurrentBuild
                            .Powers[s.PowerIndex]
                            .NIDPowerset <= -1
                            ? ""
                            : DatabaseAPI.Database
                                .Powersets[
                                    MidsContext.Character.CurrentBuild
                                        .Powers[s.PowerIndex]
                                        .NIDPowerset].Powers[
                                    MainModule.MidsController.Toon.CurrentBuild
                                        .Powers[s.PowerIndex]
                                        .IDXPower].DisplayName;
                    items[2] = Convert.ToString(setInfo[i].SlottedCount);
                    imageIndex++;
                    lstSets.Items.Add(new ListViewItem(items, imageIndex));
                    lstSets.Items[^1].Tag = setInfo[i].SetIDX;
                }
            }

            lstSets.EndUpdate();
            if (lstSets.Items.Count > 0)
            {
                lstSets.Items[0].Selected = true;
            }

            FillEffectView();
        }

        private void FillEffectView(bool getDetails = false)
        {
            var str1 = "";
            var countDict = new Dictionary<int, int>();

            var hasOvercap = false;
            EnhancementSet enhancementSet = null;
            foreach (var s in MidsContext.Character.CurrentBuild.SetBonuses)
            {
                for (var index2 = 0; index2 < s.SetInfo.Length; index2++)
                {
                    if (s.SetInfo[index2].Powers.Length <= 0) continue;

                    var setInfo = s.SetInfo;
                    enhancementSet = DatabaseAPI.Database.EnhancementSets[setInfo[index2].SetIDX];
                    var str2 = str1 + RTF.Color(RTF.ElementID.Invention) + RTF.Underline(RTF.Bold(enhancementSet.DisplayName));
                    if (MidsContext.Character.CurrentBuild.Powers[s.PowerIndex].NIDPowerset > -1)
                    {
                        str2 += $"{RTF.Crlf()}{RTF.Color(RTF.ElementID.Faded)}({DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[s.PowerIndex].NIDPowerset].Powers[MidsContext.Character.CurrentBuild.Powers[s.PowerIndex].IDXPower].DisplayName})";
                    }

                    var str3 = $"{str2}{RTF.Crlf()}{RTF.Color(RTF.ElementID.Text)}";
                    var str4 = "";
                    for (var index4 = 0; index4 < enhancementSet.Bonus.Length; index4++)
                    {
                        if (!((setInfo[index2].SlottedCount >= enhancementSet.Bonus[index4].Slotted) &
                              ((enhancementSet.Bonus[index4].PvMode == Enums.ePvX.Any) |
                               ((enhancementSet.Bonus[index4].PvMode == Enums.ePvX.PvE) &
                                !MidsContext.Config.Inc.DisablePvE) |
                               ((enhancementSet.Bonus[index4].PvMode == Enums.ePvX.PvP) &
                                MidsContext.Config.Inc.DisablePvE))))
                        {
                            continue;
                        }

                        var localOverCap = false;
                        var str5 = enhancementSet.GetEffectString(index4, false, true, true);
                        if (string.IsNullOrWhiteSpace(str5)) continue;
                        str5 = $"  {str5}";
                        if (str4 != "")
                        {
                            str4 += RTF.Crlf();
                        }

                        foreach (var esb in enhancementSet.Bonus[index4].Index)
                        {
                            if (esb <= -1) continue;

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
                            str5 = $"{RTF.Italic(RTF.Color(RTF.ElementID.Warning))}{str5} >Cap{RTF.Color(RTF.ElementID.Text)}";
                            hasOvercap = true;
                        }

                        str4 += str5;
                    }

                    foreach (var si in s.SetInfo[index2].EnhIndexes)
                    {
                        var index5 = DatabaseAPI.IsSpecialEnh(si);
                        if (index5 <= -1) continue;
                        if (str4 != "")
                        {
                            str4 += RTF.Crlf();
                        }

                        var str5 = $"{str4}{RTF.Color(RTF.ElementID.Enhancement)}";
                        var localOverCap = false;
                        var str6 = "  " + DatabaseAPI.Database.EnhancementSets[s.SetInfo[index2].SetIDX].GetEffectString(index5, true, true, true);
                        foreach (var sb in DatabaseAPI.Database.EnhancementSets[s.SetInfo[index2].SetIDX].SpecialBonus[index5].Index)
                        {
                            if (sb <= -1) continue;
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
                            str6 = $"{RTF.Italic(RTF.Color(RTF.ElementID.Warning))}{str6} >Cap{RTF.Color(RTF.ElementID.Text)}";
                            hasOvercap = true;
                        }

                        str4 = str5 + str6;
                    }

                    str1 = str3 + str4 + RTF.Crlf() + RTF.Crlf();
                }
            }

            var str7 = hasOvercap
                ? RTF.Color(RTF.ElementID.Invention) + RTF.Underline(RTF.Bold("Information:")) + RTF.Crlf() +
                  RTF.Color(RTF.ElementID.Text) +
                  "One or more set bonuses have exceeded the 5 bonus cap, and will not affect your stats. Scroll down this list to find bonuses marked as '" +
                  RTF.Italic(RTF.Color(RTF.ElementID.Warning) + ">Cap") + RTF.Color(RTF.ElementID.Text) + "'" +
                  RTF.Crlf() + RTF.Crlf()
                : "";
            var str8 = RTF.StartRTF() + str7 + str1 + RTF.EndRTF();
            if (rtxtFX.Rtf != str8) rtxtFX.Rtf = str8;
            var iStr = "";
            if (!getDetails)
            {
                var cumulativeSetBonuses = MidsContext.Character.CurrentBuild.GetCumulativeSetBonuses().ToArray();
                Array.Sort(cumulativeSetBonuses);

                foreach (var csb in cumulativeSetBonuses)
                {
                    if (iStr != "") iStr += RTF.Crlf();
                    var str2 = csb.BuildEffectString(true, "", false, false, false, true);
                    if (!str2.StartsWith("+")) str2 = "+" + str2;
                    str2 = Regex.Replace(str2, @"Endurance\b|Max End", "Max Endurance");
                    iStr += str2;
                }
            }
            else
            {
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

                    if (iStr != "") iStr += RTF.Crlf();
                    var fxTypePercent = fxGroup.Key.EffectType == Enums.eEffectType.Endurance || effectSources[fxGroup.Key].Count > 0 && effectSources[fxGroup.Key].First().Fx.DisplayPercentage;
                    var fxSumMag = effectSources[fxGroup.Key].Count > 0
                        ? effectSources[fxGroup.Key].Sum(e => e.Mag)
                        : 0;

                    var fxBlockStr = "";
                    if (l2Group != Enums.eFXSubGroup.NoGroup)
                    {
                        fxBlockStr += fxGroup.Key.L2GroupText();
                    }
                    else
                    {
                        fxBlockStr += Regex.Replace(fxGroup.Key.EffectType.ToString(), @"Endurance\b|Max End", "Max Endurance");
                        fxBlockStr += fxGroup.Key.MezType != Enums.eMez.None ? $" ({fxGroup.Key.MezType})" : "";
                        fxBlockStr += fxGroup.Key.DamageType != Enums.eDamage.None ? $" ({fxGroup.Key.DamageType})" : "";
                        fxBlockStr += fxGroup.Key.TargetEffectType != Enums.eEffectType.None ? $" ({fxGroup.Key.TargetEffectType})" : "";
                    }
                    var petSum = 0;
                    var selfSum = 0;
                    fxBlockStr += $" ({(fxTypePercent ? fxSumMag * (fxGroup.Key.EffectType == Enums.eEffectType.Endurance ? 1 : 100) : fxSumMag):##0.##}{(fxTypePercent ? "%" : "")} Total)";

                    foreach (var e in effectSources[fxGroup.Key])
                    {
                        //if ((e.AffectedEntity & Enums.eEntity.Caster) == Enums.eEntity.None) continue;
                        //if (e.EntitiesAutoHit != Enums.eEntity.None & ((e.EntitiesAutoHit & Enums.eEntity.Caster) == Enums.eEntity.None)) continue;
                        fxBlockStr += RTF.Crlf();
                        string effectString;
                        effectString = l2Group != Enums.eFXSubGroup.NoGroup
                            ? e.Fx.BuildEffectString(true, "", false, false, false, true).Replace(effectName, fxGroup.Key.L2GroupText())
                            : e.Fx.BuildEffectString(true, "", false, false, false, true);

                        if (!effectString.StartsWith("+"))
                        {
                            effectString = $"+{effectString}";
                        }

                        effectString = Regex.Replace(effectString, @"Endurance\b|Max End", "Max Endurance");
                        fxBlockStr += $"    {(e.PvMode == Enums.ePvX.PvP ? "[PVP] " : "")}{effectString}";
                        switch (e.AffectedEntity)
                        {
                            case Enums.eEntity.MyPet:
                                fxBlockStr += @" to Pets";
                                break;
                            case Enums.eEntity.Caster:
                                fxBlockStr += @" to Self";
                                break;
                        }
                        if (e.EnhSet != "" & e.Power != "" & !e.IsFromEnh)
                        {
                            fxBlockStr += $"\r\n    (From {RTF.Color(RTF.ElementID.Invention)}{e.EnhSet}{RTF.Color(RTF.ElementID.Text)} in {e.Power})";
                        }
                        else if (e.IsFromEnh)
                        {
                            if (e.Enhancement != null)
                            {
                                fxBlockStr += $"\r\n    (From {RTF.Color(RTF.ElementID.Enhancement)}{e.Enhancement.LongName}{RTF.Color(RTF.ElementID.Text)} in {e.Power})";
                            }
                        }
                    }

                    iStr += fxBlockStr;
                }
            }

            var str9 = RTF.StartRTF() + RTF.ToRTF(iStr) + RTF.EndRTF();
            if (rtApplied.Rtf == str9)
            {
                return;
            }

            rtApplied.Rtf = str9;
        }

        private Bitmap DrawEnhancementSetBitmap(int w, int h, int imgIndex)
        {
            var xb = new ExtendedBitmap(w, h);
            xb.Graphics.Clear(Color.Transparent);
            var graphics = xb.Graphics;
            I9Gfx.DrawEnhancementSet(ref graphics, imgIndex);

            return xb.Bitmap ?? new Bitmap(w, h);
        }

        private void FillImageList()
        {
            /*
            I9Gfx.LoadSets():
            for (var index = 0; index < retList.Count; index++)
            {
                DatabaseAPI.Database.EnhancementSets[index].ImageIdx = index;
            }
            */

            var imgSets = MidsContext.Character.CurrentBuild.SetBonuses
                .SelectMany(e => e.SetInfo.Where(f => f.SetIDX >= 0))
                .Select(e => DatabaseAPI.Database.EnhancementSets[e.SetIDX].Image)
                .Select(e => DatabaseAPI.Database.EnhancementSets.TryFindIndex(f => f.Image == e))
                .Select(e => DrawEnhancementSetBitmap(ilSet.ImageSize.Width, ilSet.ImageSize.Height, e))
                .ToArray();

            ilSet.Images.Clear();
            foreach (var img in imgSets)
            {
                ilSet.Images.Add(img);
            }
        }

        private void Bar_Hover(object sender)
        {
            if (sender is not ctlLayeredBarPb bar)
            {
                return;
            }

            var barData = BarsFX[bar.Name];
            var barValues = bar.GetValues();
            var sEffects = MidsContext.Character.CurrentBuild.GetEffectSources();
            var overlayVector = OverlayText.Vector(barData);
            var overlayDmgType = OverlayText.DamageType(barData);
            var overlayMezType = OverlayText.MezType(barData);
            var overlayTargetEffect = OverlayText.TargetEffect(barData);
            var overlayValuePercent = OverlayText.HasPercentage(barData);
            var plusSignEnabled = OverlayText.HasPlusSign(barData);

            if (barData.EffectType == Enums.eEffectType.Recovery)
            {
                barValues["totalsvalue"] += 100;
            }

            var ttext = $"{(overlayDmgType != "" ? overlayDmgType + " " : "")}{overlayVector}{overlayMezType}{overlayTargetEffect}";
            if (barValues["setbuffs"] > 0)
            {
                ttext += $"\r\nFrom Sets: {(plusSignEnabled ? "+" : "")}{barValues["setbuffs"]:##0.##}{(overlayValuePercent ? "%" : "")}";
            }

            var fxPetMagSum = 0.0;
            var fxSelfMagSum = 0.0;
            var stext = string.Empty;
            var fxValue = sEffects.Where(x => OverlayText.Vector(x.Key) == overlayVector).Select(e => e.Value).FirstOrDefault();
            if (fxValue != null)
            {
                fxPetMagSum = fxValue.Where(v => v.AffectedEntity == Enums.eEntity.MyPet && v.IsFromEnh).Sum(x => x.Mag);
                
                if (fxPetMagSum > 0)
                {
                    stext += $" (Pets: {fxPetMagSum:P0})";
                }
            }

            if (!string.IsNullOrWhiteSpace(stext))
            {
                ttext += stext;
            }
            
            if (barValues["totalsvalue"] > 0) // & Math.Abs(barValues["setbuffs"] - barValues["totalsvalue"]) > float.Epsilon)
            {
                ttext += $"\r\nTotal: {(plusSignEnabled ? "+" : "")}{barValues["totalsvalue"]:##0.##}{(overlayValuePercent ? "%" : "")}";
            }

            bar.SetTip(ttext);
        }

        private void frmSetViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            myParent.FloatSets(false);
        }

        private void frmSetViewer_Load(object sender, EventArgs e)
        {
            lstSets.EnableDoubleBuffer();
            panelBars.EnableDoubleBuffer();
        }

        private void frmSetViewer_Move(object sender, EventArgs e)
        {
            StoreLocation();
        }

        private void lstSets_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstSets.SelectedItems.Count < 1) return;
            rtxtInfo.Rtf = RTF.StartRTF() + EnhancementSetCollection.GetSetInfoLongRTF(
                Convert.ToInt32(lstSets.SelectedItems[0].Tag),
                Convert.ToInt32(lstSets.SelectedItems[0].SubItems[2].Text)) + RTF.EndRTF();
        }

        public void SetLocation()
        {
            var rectangle = new Rectangle
            {
                X = MainModule.MidsController.SzFrmSets.X, Y = MainModule.MidsController.SzFrmSets.Y
            };
            if (rectangle.X < 1)
            {
                rectangle.X = myParent.Left + 8;
            }

            if (rectangle.Y < 32)
            {
                rectangle.Y = myParent.Top + (myParent.Height - myParent.ClientSize.Height) +
                              myParent.GetPrimaryBottom();
            }

            if (MidsContext.Config.ShrinkFrmSets & (Width > 700) |
                !MidsContext.Config.ShrinkFrmSets & (Width < 700))
            {
                btnSmall_Click();
            }

            Top = rectangle.Y;
            Left = rectangle.X;
        }

        private void StoreLocation()
        {
            if (!MainModule.MidsController.IsAppInitialized) return;
            MainModule.MidsController.SzFrmSets.X = Left;
            MainModule.MidsController.SzFrmSets.Y = Top;
            MidsContext.Config.ShrinkFrmSets = Width < 700;
        }

        private void SetupImageButton(ImageButton ib)
        {
            var imageOffIdx = MidsContext.Character.IsHero() ? 2 : 4;
            var imageOnIdx = imageOffIdx + 1;

            ib.IA = myParent.Drawing.PImageAttributes;
            ib.ImageOff = myParent.Drawing.BxPower[imageOffIdx].Bitmap;
            ib.ImageOn = myParent.Drawing.BxPower[imageOnIdx].Bitmap;
        }

        public void UpdateData()
        {
            if (myParent == null) return;

            BackColor = myParent.BackColor;
            if (rtApplied.BackColor != BackColor)
            {
                rtApplied.BackColor = BackColor;
            }

            if (rtxtFX.BackColor != BackColor)
            {
                rtxtFX.BackColor = BackColor;
            }

            if (rtxtInfo.BackColor != BackColor)
            {
                rtxtInfo.BackColor = BackColor;
            }

            var imageOffIdx = MidsContext.Character.IsHero() ? 2 : 4;
            var imageOnIdx = imageOffIdx + 1;

            SetupImageButton(btnClose);
            SetupImageButton(chkOnTop);
            SetupImageButton(btnSmall);
            SetupImageButton(btnDetailFx);

            DisplayList();
            DrawBars();
        }

        private class BarSettings
        {
            public Enums.eEffectType EffectType;
            public Enums.eEffectType TargetEffectType;
            public Enums.eDamage DamageType;
            public Enums.eMez MezType;
            public Color SetBuffsColor;
            public Color TotalsColor;

            public BarSettings(Enums.eEffectType effectType, Color setBuffsColor, Color totalsColor)
            {
                EffectType = effectType;
                TargetEffectType = Enums.eEffectType.None;
                DamageType = Enums.eDamage.None;
                MezType = Enums.eMez.None;
                SetBuffsColor = setBuffsColor;
                TotalsColor = totalsColor;
            }

            public BarSettings(Enums.eEffectType effectType, Enums.eEffectType targetEffectType, Color setBuffsColor, Color totalsColor)
            {
                EffectType = effectType;
                TargetEffectType = targetEffectType;
                DamageType = Enums.eDamage.None;
                MezType = Enums.eMez.None;
                SetBuffsColor = setBuffsColor;
                TotalsColor = totalsColor;
            }

            public BarSettings(Enums.eEffectType effectType, Enums.eDamage damageType, Color setBuffsColor, Color totalsColor)
            {
                EffectType = effectType;
                TargetEffectType = Enums.eEffectType.None;
                DamageType = damageType;
                MezType = Enums.eMez.None;
                SetBuffsColor = setBuffsColor;
                TotalsColor = totalsColor;
            }

            public BarSettings(Enums.eEffectType effectType, Enums.eMez mezType, Color setBuffsColor, Color totalsColor)
            {
                EffectType = effectType;
                TargetEffectType = Enums.eEffectType.None;
                DamageType = Enums.eDamage.None;
                MezType = mezType;
                SetBuffsColor = setBuffsColor;
                TotalsColor = totalsColor;
            }

            public Build.FXIdentifierKey GetIdentifierKey()
            {
                return new Build.FXIdentifierKey
                {
                    EffectType = EffectType,
                    DamageType = DamageType,
                    MezType = MezType,
                    TargetEffectType = TargetEffectType
                };
            }
        }

        private int GetControlYPosition(int numHeaders, int numBars, int barHeight)
        {
            return (barHeight + 3) * numBars + 20 * numHeaders;
        }

        private float GetMultiScaleMaxValue(float barValue, float[] caps)
        {
            for (var i = 0; i < caps.Length; i++)
            {
                if (barValue < 0.75 * caps[i]) return caps[i];
            }

            return caps[^1];
        }

        private void DrawBars()
        {
            var cumulativeSetBonuses = MidsContext.Character.CurrentBuild.GetCumulativeSetBonuses();
            var displayStats = MidsContext.Character.DisplayStats;
            var l1Group = "";
            var nh = 0; // Nb of headers
            var nb = 0; // Nb of bars
            var yPos = 0;
            var offset = 0;
            const int hBar = 12;
            const int barLabelWidth = 110;
            BarsFX.Clear();
            panelBars.SuspendLayout();
            panelBars.Controls.Clear();
            foreach (var st in EffectListOrder)
            {
                var fxId = new Build.FXIdentifierKey
                {
                    EffectType = st.EffectType,
                    TargetEffectType = st.TargetEffectType,
                    DamageType = st.DamageType,
                    MezType = st.MezType
                };

                yPos = GetControlYPosition(nh, nb, hBar);
                var fxL1Group = fxId.L1Group;
                var newHeader = false;
                var header = new Label();
                if (fxL1Group.ToString() != l1Group)
                {
                    if (nh > 0) offset += 2;
                    header.Location = new Point(0, yPos + offset);
                    header.ForeColor = Color.Cyan;
                    header.BackColor = Color.Black;
                    header.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold | FontStyle.Italic, GraphicsUnit.Point, 0);
                    header.Size = new Size(panelBars.Size.Width - 20, 16);
                    header.Name = $"HeaderLabel{nh + 1}";
                    header.Text = fxL1Group.ToString();

                    newHeader = true;
                }

                var lBuffs = cumulativeSetBonuses.Where(e => e.EffectType == st.EffectType &
                                                             (st.DamageType == Enums.eDamage.None | st.DamageType == e.DamageType) &
                                                             (st.TargetEffectType == Enums.eEffectType.None | st.TargetEffectType == e.ETModifies) &
                                                             (st.MezType == Enums.eMez.None | st.MezType == e.MezType)).ToList();

                if (lBuffs.Count <= 0)
                {
                    //if (newHeader) offset -= 0;
                    continue;
                }

                if (newHeader)
                {
                    nh++;
                    panelBars.Controls.Add(header);
                    l1Group = fxL1Group.ToString();
                }

                yPos = GetControlYPosition(nh, nb, hBar);
                var bar = new ctlLayeredBarPb
                {
                    Location = new Point(barLabelWidth + 8, yPos + offset),
                    Size = new Size(panelBars.Size.Width - barLabelWidth - 28, hBar),
                    Name = $"Bar{nb + 1}",
                    MaximumBarValue = 100,
                    OverlayTextFont = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Pixel, 0),
                    EnableOverlayText = true,
                    EnableOverlayOutline = true,
                };
                bar.BarHover += Bar_Hover;

                var barLabel = new Label
                {
                    Location = new Point(2, yPos + offset - 1),
                    Size = new Size(barLabelWidth, hBar + 2),
                    Name = $"BarLabel{nb + 1}",
                    TextAlign = ContentAlignment.TopLeft,
                    Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Pixel, 0),
                    ForeColor = Color.WhiteSmoke
                };

                bar.AssignNames(new List<(string name, Color color)>
                {
                    ("setbuffs", st.SetBuffsColor),
                    ("totalsvalue", st.TotalsColor)
                });
                
                var totalsValue = st.EffectType switch
                {
                    Enums.eEffectType.Resistance => displayStats.DamageResistance((int) st.DamageType, false),
                    Enums.eEffectType.Defense => displayStats.Defense((int) st.DamageType),
                    Enums.eEffectType.Regeneration => displayStats.HealthRegenPercent(false),
                    Enums.eEffectType.HitPoints => displayStats.HealthHitpointsNumeric(false),
                    Enums.eEffectType.Absorb => Math.Min(displayStats.Absorb, MidsContext.Character.Archetype.Hitpoints),
                    Enums.eEffectType.Recovery => displayStats.EnduranceRecoveryPercentage(false) - 100,
                    Enums.eEffectType.Endurance => displayStats.EnduranceMaxEnd - 100,
                    Enums.eEffectType.SpeedRunning => displayStats.MovementRunSpeed(Enums.eSpeedMeasure.FeetPerSecond, false),
                    Enums.eEffectType.SpeedJumping => displayStats.MovementJumpSpeed(Enums.eSpeedMeasure.FeetPerSecond, false),
                    Enums.eEffectType.JumpHeight => displayStats.MovementJumpHeight(Enums.eSpeedMeasure.FeetPerSecond),
                    Enums.eEffectType.SpeedFlying => displayStats.MovementFlySpeed(Enums.eSpeedMeasure.MilesPerHour, false),
                    
                    Enums.eEffectType.StealthRadius => MidsContext.Character.Totals.StealthPvE,
                    Enums.eEffectType.StealthRadiusPlayer => MidsContext.Character.Totals.StealthPvP,
                    Enums.eEffectType.PerceptionRadius => displayStats.Perception(false),
                    
                    Enums.eEffectType.RechargeTime => displayStats.BuffHaste(false) - 100,
                    Enums.eEffectType.ToHit => displayStats.BuffToHit,
                    Enums.eEffectType.DamageBuff => displayStats.BuffDamage(false),
                    Enums.eEffectType.Elusivity => MidsContext.Character.Totals.ElusivityMax,
                    Enums.eEffectType.Enhancement => st.TargetEffectType switch
                    {
                        Enums.eEffectType.RechargeTime => displayStats.BuffHaste(false) - 100,
                        Enums.eEffectType.Accuracy => displayStats.BuffAccuracy,
                        Enums.eEffectType.EnduranceDiscount => displayStats.BuffEndRdx,
                        _ => 0
                    },
                    _ => 0
                };

                var fx = lBuffs.First(); 
                var fxMagAdjusted = fx.Mag * st.EffectType switch
                {
                    Enums.eEffectType.DamageBuff => 100,
                    Enums.eEffectType.Enhancement => 100,
                    Enums.eEffectType.Regeneration => 100,
                    Enums.eEffectType.Resistance => 100,
                    Enums.eEffectType.Recovery => 100,
                    Enums.eEffectType.Defense => 100,
                    Enums.eEffectType.SpeedRunning => 100,
                    Enums.eEffectType.SpeedJumping => 100,
                    Enums.eEffectType.JumpHeight => 100,
                    Enums.eEffectType.SpeedFlying => 100,
                    Enums.eEffectType.ResEffect => 100,
                    Enums.eEffectType.MezResist => 100,
                    _ => 1
                };

                totalsValue = st.EffectType switch
                {
                    Enums.eEffectType.SpeedRunning => Math.Max(0, totalsValue / Statistics.BaseRunSpeed * 100 - 100),
                    Enums.eEffectType.SpeedJumping => Math.Max(0, totalsValue / Statistics.BaseJumpSpeed * 100 - 100),
                    Enums.eEffectType.SpeedFlying => Math.Max(0, totalsValue / Statistics.BaseFlySpeed * 100 - 100),
                    Enums.eEffectType.JumpHeight => Math.Max(0, totalsValue / Statistics.BaseJumpHeight * 100 - 100),
                    Enums.eEffectType.MezResist => MidsContext.Character.Totals.MezRes[(int)st.MezType],
                    Enums.eEffectType.Enhancement => st.TargetEffectType == Enums.eEffectType.SpeedRunning ? 0 : totalsValue, 
                    _ => totalsValue
                };

                if (Math.Abs(fxMagAdjusted - totalsValue) < float.Epsilon) totalsValue = 0;

                bar.MaximumBarValue = st.EffectType switch
                {
                    Enums.eEffectType.DamageBuff => MidsContext.Character.Archetype.DamageCap * 100 - 100,
                    Enums.eEffectType.Regeneration => MidsContext.Character.Archetype.RegenCap * 100,
                    Enums.eEffectType.HitPoints => MidsContext.Character.Archetype.HPCap,
                    Enums.eEffectType.Recovery => GetMultiScaleMaxValue(Math.Max(fxMagAdjusted, totalsValue), new float[] {50, 100, 260, 510, 760}),
                    Enums.eEffectType.Endurance => 50,
                    Enums.eEffectType.SpeedRunning => MidsContext.Character.Totals.MaxRunSpd / Statistics.BaseRunSpeed * 100 + 25,
                    Enums.eEffectType.SpeedJumping => MidsContext.Character.Totals.MaxJumpSpd / Statistics.BaseJumpSpeed * 100 + 25,
                    Enums.eEffectType.SpeedFlying => MidsContext.Character.Totals.MaxFlySpd / Statistics.BaseFlySpeed * 100 + 25,
                    Enums.eEffectType.StealthRadius => 1000,
                    Enums.eEffectType.StealthRadiusPlayer => 1000,
                    Enums.eEffectType.PerceptionRadius => 1000,
                    Enums.eEffectType.MezResist => 500,
                    Enums.eEffectType.Enhancement => st.TargetEffectType switch
                    {
                        Enums.eEffectType.Accuracy => 200,
                        Enums.eEffectType.RechargeTime => 400,
                        _ => 100
                    },
                    _ => 100
                };

                var idk = st.GetIdentifierKey();
                bar.AssignValues(new List<float> {fxMagAdjusted, totalsValue});
                
                bar.OverlayText = $"{fxMagAdjusted:##0.##}{(OverlayText.HasPercentage(idk) ? "%" : "")}";

                barLabel.Text = OverlayText.ShortLabel(idk);
                panelBars.Controls.Add(bar);
                panelBars.Controls.Add(barLabel);
                BarsFX.Add(bar.Name, new Build.FXIdentifierKey
                {
                    EffectType = st.EffectType,
                    TargetEffectType = st.TargetEffectType,
                    DamageType = st.DamageType,
                    MezType = st.MezType
                });

                nb++;
            }

            panelBars.ResumeLayout();
        }

        private void btnDetailFx_ButtonClicked()
        {
            panelBars.Visible = btnDetailFx.Checked;
            rtxtFX.Visible = !btnDetailFx.Checked;
            Label1.Text = btnDetailFx.Checked ? "Set Bonuses vs. Totals:" : "Effect Breakdown:";
            FillEffectView(btnDetailFx.Checked);
            if (btnDetailFx.Checked) DrawBars();
        }
    }
}