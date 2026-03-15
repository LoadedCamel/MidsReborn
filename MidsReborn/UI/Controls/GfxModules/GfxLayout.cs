using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Mids_Reborn.UI.Controls.GfxModules;

public static class GfxLayout
{
    internal static void DrawSplit(this ClsDrawX drawX)
    {
        var pen = new Pen(Color.Goldenrod, 2f);
        checked
        {
            int y;
            switch (drawX._ColumnStackingMode)
            {
                case Enums.eColumnStacking.Horizontal or Enums.eColumnStacking.Vertical:
                    y = ClsDrawX.OffsetInherent + drawX._vcRowsPowers * (drawX.SzPower.Height + 27) + (drawX._ColumnStackingMode != Enums.eColumnStacking.None ? 18 : 0);
                    drawX.BxBuffer?.Graphics?.DrawLine(pen, 2, drawX.ScaleDown(y), drawX.ScaleDown(drawX.GetDrawingArea().Width), drawX.ScaleDown(y));
                    drawX.BxBuffer?.Graphics?.DrawString("Inherent Powers",
                        new Font(Fonts.Family("Noto Sans"), 13f, FontStyle.Regular, GraphicsUnit.Pixel),
                        MidsContext.Character.IsHero()
                            ? new SolidBrush(Color.DodgerBlue)
                            : new SolidBrush(Color.Red), drawX.ScaleDown(drawX.GetDrawingArea().Width / 2 - 50), drawX.ScaleDown(y));

                    break;

                default:
                    switch (drawX._vcCols)
                    {
                        case 2:
                            y = ClsDrawX.OffsetInherent + drawX._vcRowsPowers * (drawX.SzPower.Height + 27);
                            drawX.BxBuffer?.Graphics?.DrawLine(pen, 2, drawX.ScaleDown(y), drawX.ScaleDown(drawX.GetDrawingArea().Width),
                                drawX.ScaleDown(y));
                            drawX.BxBuffer.Graphics?.DrawString("Inherent Powers",
                                new Font(Fonts.Family("Noto Sans"), 13f, FontStyle.Regular, GraphicsUnit.Pixel),
                                MidsContext.Character.IsHero()
                                    ? new SolidBrush(Color.DodgerBlue)
                                    : new SolidBrush(Color.Red), drawX.ScaleDown(drawX.GetDrawingArea().Width / 2 - 50),
                                drawX.ScaleDown(y));
                            break;
                        
                        case 3:
                        case 4:
                            y = ClsDrawX.OffsetInherent + drawX._vcRowsPowers * (drawX.SzPower.Height + 27);
                            drawX.BxBuffer?.Graphics?.DrawLine(pen, 2, drawX.ScaleDown(y), drawX.ScaleDown(drawX.GetDrawingArea().Width),
                                drawX.ScaleDown(y));
                            drawX.BxBuffer?.Graphics?.DrawString("Inherent Powers",
                                new Font(Fonts.Family("Noto Sans"), 13f, FontStyle.Regular, GraphicsUnit.Pixel),
                                MidsContext.Character.IsHero()
                                    ? new SolidBrush(Color.DodgerBlue)
                                    : new SolidBrush(Color.Red), drawX.ScaleDown(drawX.GetDrawingArea().Width / 2 - 50),
                                drawX.ScaleDown(y));

                            break;

                        case 5:
                            y = ClsDrawX.OffsetInherent + drawX._vcRowsPowers * (drawX.SzPower.Height + 48);
                            drawX.BxBuffer?.Graphics?.DrawLine(pen, 2, drawX.ScaleDown(y), drawX.ScaleDown(drawX.GetDrawingArea().Width),
                                drawX.ScaleDown(y));
                            drawX.BxBuffer?.Graphics?.DrawString("Inherent Powers",
                                new Font(Fonts.Family("Noto Sans"), 13f, FontStyle.Regular, GraphicsUnit.Pixel),
                                MidsContext.Character.IsHero()
                                    ? new SolidBrush(Color.DodgerBlue)
                                    : new SolidBrush(Color.Red), drawX.ScaleDown(drawX.GetDrawingArea().Width / 2 - 50),
                                drawX.ScaleDown(y));
                            break;

                        case 6:
                            y = ClsDrawX.OffsetInherent + drawX._vcRowsPowers * (drawX.SzPower.Height + 27);
                            drawX.BxBuffer?.Graphics?.DrawLine(pen, 2, drawX.ScaleDown(y), drawX.ScaleDown(drawX.GetDrawingArea().Width),
                                drawX.ScaleDown(y));
                            drawX.BxBuffer?.Graphics?.DrawString("Inherent Powers",
                                new Font(Fonts.Family("Noto Sans"), 13f, FontStyle.Regular, GraphicsUnit.Pixel),
                                MidsContext.Character.IsHero()
                                    ? new SolidBrush(Color.DodgerBlue)
                                    : new SolidBrush(Color.Red), drawX.ScaleDown(drawX.GetDrawingArea().Width / 2 - 50),
                                drawX.ScaleDown(y));
                            break;
                    }

                    break;
            }
        }
    }

    public static void GetPowersLayout(this ClsDrawX drawX)
    {
        var powersLayout = new List<List<int>>();
        var nullColumn = new List<int>();

        if (MidsContext.Config.Columns < 2)
        {
            MidsContext.Config.Columns = 3;
        }

        switch (drawX._ColumnStackingMode)
        {
            case Enums.eColumnStacking.Horizontal:
                powersLayout.Add([]);
                powersLayout.Add([]);
                var epicColumn = new List<int>();
                var pools = new Dictionary<string, int>();

                for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[i] is { Power: null })
                    {
                        nullColumn.Add(i);
                        continue;
                    }

                    switch (MidsContext.Character.CurrentBuild.Powers[i]?.Power?.GetPowerSet()?.SetType)
                    {
                        case Enums.ePowerSetType.Primary:
                            powersLayout[0].Add(i);
                            break;

                        case Enums.ePowerSetType.Secondary:
                            powersLayout[1].Add(i);
                            break;

                        case Enums.ePowerSetType.Pool:
                            var setName = MidsContext.Character.CurrentBuild.Powers[i]?.Power?.GetPowerSet()?.FullName;
                            if (!string.IsNullOrEmpty(setName))
                            {
                                if (!pools.ContainsKey(setName))
                                {
                                    powersLayout.Add([]);
                                    powersLayout[^1].Add(i);
                                    pools.Add(setName, powersLayout.Count - 1);
                                }
                                else
                                {
                                    powersLayout[pools[setName]].Add(i);
                                }
                            }

                            break;

                        case Enums.ePowerSetType.Ancillary:
                            epicColumn.Add(i);

                            break;

                    }
                }

                if (epicColumn.Count > 0)
                {
                    powersLayout.Add(epicColumn);
                }

                if (nullColumn.Count > 0)
                {
                    powersLayout.Add(nullColumn);
                }

                break;

            case Enums.eColumnStacking.Vertical:
                powersLayout.Add([]);
                powersLayout.Add([]);
                powersLayout.Add([]);

                for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
                {
                    if (MidsContext.Character.CurrentBuild.Powers[i] is { Power: null })
                    {
                        nullColumn.Add(i);
                        continue;
                    }

                    var col = MidsContext.Character.CurrentBuild.Powers[i]?.Power?.GetPowerSet()?.SetType switch
                    {
                        Enums.ePowerSetType.Primary => 0,
                        Enums.ePowerSetType.Secondary => 1,
                        Enums.ePowerSetType.Pool or Enums.ePowerSetType.Ancillary => 2,
                        _ => -1
                    };

                    if (col < 0)
                    {
                        continue;
                    }

                    powersLayout[col].Add(i);
                }

                if (nullColumn.Count > 0)
                {
                    powersLayout.Add(nullColumn);
                }

                break;
        }

        drawX.ColumnsPowersLayout = drawX.LayoutToGridPos(powersLayout);
        drawX.Columns = drawX.LayoutColumns;
        drawX.HasNullColumn = nullColumn.Count > 0;

        if (drawX._ColumnStackingMode == Enums.eColumnStacking.None)
        {
            drawX._vcCols = MidsContext.Config.Columns;
            drawX._vcRowsPowers = ClsDrawX.VcPowers / drawX._vcCols;
        }
        else
        {
            drawX._vcCols = drawX.LayoutColumns;
            drawX._vcRowsPowers = powersLayout.Select(e => e.Count).Max();
        }
    }

    public static int GetVisualIdx(this ClsDrawX drawX, int powerIndex)
    {
        var nidPowerset = MidsContext.Character.CurrentBuild.Powers[powerIndex] != null
            ? MidsContext.Character.CurrentBuild.Powers[powerIndex].NIDPowerset
            : -1;
        var idxPower = MidsContext.Character.CurrentBuild.Powers[powerIndex] != null
            ? MidsContext.Character.CurrentBuild.Powers[powerIndex].IDXPower
            : -1;

        var isInherent = powerIndex > 23;

        if (nidPowerset > -1)
        {
            if ((DatabaseAPI.Database.Powersets[nidPowerset].SetType == Enums.ePowerSetType.Inherent) & isInherent)
            {
                return DatabaseAPI.Database.Powersets[nidPowerset].Powers[idxPower].LocationIndex;
            }

            var vIdx = -1;
            for (var i = 0; i <= powerIndex; i++)
            {
                if (MidsContext.Character.CurrentBuild.Powers[i]?.NIDPowerset > -1)
                {
                    if ((DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset].SetType != Enums.ePowerSetType.Inherent) | !isInherent)
                    {
                        vIdx++;
                    }
                }
                else
                {
                    vIdx++;
                }
            }

            return vIdx;
        }
        else
        {
            var vIdx = -1;
            for (var i = 0; i <= powerIndex; i++)
            {
                if (MidsContext.Character.CurrentBuild.Powers[i]?.NIDPowerset > -1)
                {
                    if (DatabaseAPI.Database.Powersets[MidsContext.Character.CurrentBuild.Powers[i].NIDPowerset].SetType != Enums.ePowerSetType.Inherent)
                    {
                        vIdx++;
                    }
                }
                else
                {
                    vIdx++;
                }
            }

            return vIdx;
        }
    }

    public static int[][] GetInherentGrid(this ClsDrawX drawX)
    {
        switch (drawX._vcCols)
        {
            case 2:
                if (MidsContext.Character.Archetype.ClassType == Enums.eClassType.HeroEpic)
                    return
                    [
                        [
                            0, 1
                        ],
                        [
                            2, 3
                        ],
                        [
                            4, 5
                        ],
                        [
                            6, 7
                        ],
                        [
                            8, 9
                        ],
                        [
                            10, 11
                        ],
                        [
                            12, 13
                        ],
                        [
                            14, 15
                        ],
                        [
                            16, 17
                        ],
                        [
                            18, 19
                        ],
                        [
                            20, 21
                        ],
                        [
                            22, 23
                        ],
                        [
                            24, 25
                        ],
                        [
                            26, 27
                        ],
                        [
                            28, 29
                        ],
                        [
                            30, 31
                        ],
                        [
                            32, 33
                        ],
                        [
                            34, 35
                        ],
                        [
                            36, 37
                        ],
                        [
                            38, 39
                        ],
                        [
                            40, 41
                        ],
                        [
                            42, 43
                        ],
                        [
                            44, 45
                        ],
                        [
                            46, 47
                        ],
                        [
                            48, 49
                        ],
                        [
                            50, 51
                        ],
                        [
                            52, 53
                        ],
                        [
                            54, 55
                        ],
                        [
                            56, 57
                        ],
                        [
                            58, 59
                        ]
                    ];

                return
                [
                    [
                        0, 1
                    ],
                    [
                        2, 3
                    ],
                    [
                        4, 5
                    ],
                    [
                        6, 7
                    ],
                    [
                        8, 9
                    ],
                    [
                        10, 11
                    ],
                    [
                        12, 13
                    ],
                    [
                        14, 15
                    ],
                    [
                        16, 17
                    ],
                    [
                        18, 19
                    ],
                    [
                        20, 21
                    ],
                    [
                        22, 23
                    ],
                    [
                        24, 25
                    ],
                    [
                        26, 27
                    ],
                    [
                        28, 29
                    ],
                    [
                        30, 31
                    ],
                    [
                        32, 33
                    ],
                    [
                        34, 35
                    ],
                    [
                        36, 37
                    ],
                    [
                        38, 39
                    ],
                    [
                        40, 41
                    ],
                    [
                        42, 43
                    ],
                    [
                        44, 45
                    ],
                    [
                        46, 47
                    ],
                    [
                        48, 49
                    ],
                    [
                        50, 51
                    ],
                    [
                        52, 53
                    ],
                    [
                        54, 55
                    ],
                    [
                        56, 57
                    ],
                    [
                        58, 59
                    ]
                ];
            case 4:
                if (MidsContext.Character.Archetype.ClassType == Enums.eClassType.HeroEpic)
                    return
                    [
                        [
                            0, 1, 2, 3
                        ],
                        [
                            4, 5, 6, 7
                        ],
                        [
                            8, 9, 10, 11
                        ],
                        [
                            12, 13, 14, 15
                        ],
                        [
                            16, 17, 18, 19
                        ],
                        [
                            20, 21, 22, 23
                        ],
                        [
                            24, 25, 26, 27
                        ],
                        [
                            28, 29, 30, 31
                        ],
                        [
                            32, 33, 34, 35
                        ],
                        [
                            36, 37, 38, 39
                        ],
                        [
                            40, 41, 42, 43
                        ],
                        [
                            44, 45, 46, 47
                        ],
                        [
                            48, 49, 50, 51
                        ],
                        [
                            52, 53, 54, 55
                        ],
                        [
                            56, 57, 58, 59
                        ]
                    ];

                return
                [
                    [
                        0, 1, 2, 3
                    ],
                    [
                        4, 5, 6, 7
                    ],
                    [
                        8, 9, 10, 11
                    ],
                    [
                        12, 13, 14, 15
                    ],
                    [
                        16, 17, 18, 19
                    ],
                    [
                        20, 21, 22, 23
                    ],
                    [
                        24, 25, 26, 27
                    ],
                    [
                        28, 29, 30, 31
                    ],
                    [
                        32, 33, 34, 35
                    ],
                    [
                        36, 37, 38, 39
                    ],
                    [
                        40, 41, 42, 43
                    ],
                    [
                        44, 45, 46, 47
                    ],
                    [
                        48, 49, 50, 51
                    ],
                    [
                        52, 53, 54, 55
                    ],
                    [
                        56, 57, 58, 59
                    ]
                ];
            case 5:
                if (MidsContext.Character.Archetype.ClassType == Enums.eClassType.HeroEpic)
                    return
                    [
                        [
                            0, 1, 2, 3, 4
                        ],
                        [
                            5, 6, 7, 8, 9
                        ],
                        [
                            10, 11, 12, 13, 14
                        ],
                        [
                            15, 16, 17, 18, 19
                        ],
                        [
                            20, 21, 22, 23, 24
                        ],
                        [
                            25, 26, 27, 28, 29
                        ],
                        [
                            30, 31, 32, 33, 34
                        ],
                        [
                            35, 36, 37, 38, 39
                        ],
                        [
                            40, 41, 42, 43, 44
                        ],
                        [
                            45, 46, 47, 48, 49
                        ],
                        [
                            50, 51, 52, 53, 54
                        ],
                        [
                            55, 56, 57, 58, 59
                        ]
                    ];

                return
                [
                    [
                        0, 1, 2, 3, 4
                    ],
                    [
                        5, 6, 7, 8, 9
                    ],
                    [
                        10, 11, 12, 13, 14
                    ],
                    [
                        15, 16, 17, 18, 19
                    ],
                    [
                        20, 21, 22, 23, 24
                    ],
                    [
                        25, 26, 27, 28, 29
                    ],
                    [
                        30, 31, 32, 33, 34
                    ],
                    [
                        35, 36, 37, 38, 39
                    ],
                    [
                        40, 41, 42, 43, 44
                    ],
                    [
                        45, 46, 47, 48, 49
                    ],
                    [
                        50, 51, 52, 53, 54
                    ],
                    [
                        55, 56, 57, 58, 59
                    ]
                ];
            case 6:
                if (MidsContext.Character.Archetype.ClassType == Enums.eClassType.HeroEpic)
                    return
                    [
                        [
                            0, 1, 2, 3, 4, 5
                        ],
                        [
                            6, 7, 8, 9, 10, 11
                        ],
                        [
                            12, 13, 14, 15, 16, 17
                        ],
                        [
                            18, 19, 20, 21, 22, 23
                        ],
                        [
                            24, 25, 26, 27, 28, 29
                        ],
                        [
                            30, 31, 32, 33, 34, 35
                        ],
                        [
                            36, 37, 38, 39, 40, 41
                        ],
                        [
                            42, 43, 44, 45, 46, 47
                        ],
                        [
                            48, 49, 50, 51, 52, 53
                        ],
                        [
                            54, 55, 56, 57, 58, 59
                        ]
                    ];

                return
                [
                    [
                        0, 1, 2, 3, 4, 5
                    ],
                    [
                        6, 7, 8, 9, 10, 11
                    ],
                    [
                        12, 13, 14, 15, 16, 17
                    ],
                    [
                        18, 19, 20, 21, 22, 23
                    ],
                    [
                        24, 25, 26, 27, 28, 29
                    ],
                    [
                        30, 31, 32, 33, 34, 35
                    ],
                    [
                        36, 37, 38, 39, 40, 41
                    ],
                    [
                        42, 43, 44, 45, 46, 47
                    ],
                    [
                        48, 49, 50, 51, 52, 53
                    ],
                    [
                        54, 55, 56, 57, 58, 59
                    ]
                ];
        }

        if (MidsContext.Character.Archetype.ClassType == Enums.eClassType.HeroEpic)
            return
            [
                [
                    0, 1, 2
                ],
                [
                    3, 4, 5
                ],
                [
                    6, 7, 8
                ],
                [
                    9, 10, 11
                ],
                [
                    12, 13, 14
                ],
                [
                    15, 16, 17
                ],
                [
                    18, 19, 20
                ],
                [
                    21, 22, 23
                ],
                [
                    24, 25, 26
                ],
                [
                    27, 28, 29
                ],
                [
                    30, 31, 32
                ],
                [
                    33, 34, 35
                ],
                [
                    36, 37, 38
                ],
                [
                    39, 40, 41
                ],
                [
                    42, 43, 44
                ],
                [
                    45, 46, 47
                ],
                [
                    48, 49, 50
                ],
                [
                    51, 52, 53
                ],
                [
                    54, 55, 56
                ],
                [
                    57, 58, 59
                ]
            ];

        return
        [
            [
                0, 1, 2
            ],
            [
                3, 4, 5
            ],
            [
                6, 7, 8
            ],
            [
                9, 10, 11
            ],
            [
                12, 13, 14
            ],
            [
                15, 16, 17
            ],
            [
                18, 19, 20
            ],
            [
                21, 22, 23
            ],
            [
                24, 25, 26
            ],
            [
                27, 28, 29
            ],
            [
                30, 31, 32
            ],
            [
                33, 34, 35
            ],
            [
                36, 37, 38
            ],
            [
                39, 40, 41
            ],
            [
                42, 43, 44
            ],
            [
                45, 46, 47
            ],
            [
                48, 49, 50
            ],
            [
                51, 52, 53
            ],
            [
                54, 55, 56
            ],
            [
                57, 58, 59
            ]
        ];
    }

    public static Size GetDrawingArea(this ClsDrawX drawX)
    {
        var result = (Size)drawX.PowerPosition(ClsDrawX.VcPowers - 1);
        checked
        {
            result.Width += drawX.SzPower.Width;
            result.Height = result.Height + drawX.SzPower.Height + ClsDrawX.PaddingY;
            for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
            {
                if (MidsContext.Character.CurrentBuild.Powers[i] != null && (MidsContext.Character.CurrentBuild.Powers[i].Power == null || (MidsContext.Character.CurrentBuild.Powers[i].Chosen && i > MidsContext.Character.CurrentBuild.LastPower)))
                {
                    continue;
                }

                var size = result with { Height = drawX.PowerPosition(i).Y + drawX.SzPower.Height + ClsDrawX.PaddingY };
                if (size.Height > result.Height)
                {
                    result.Height = size.Height;
                }

                if (size.Width > result.Width)
                {
                    result.Width = size.Width;
                }
            }

            return result;
        }
    }

    public static Size GetMaxDrawingArea(this ClsDrawX drawX)
    {
        var cols = drawX._vcCols;
        drawX.MiniSetCol(6);
        var result = (Size)drawX.PowerPosition(ClsDrawX.VcPowers - 1);
        drawX.MiniSetCol(2);
        var inherentGrid = drawX.GetInherentGrid();
        checked
        {
            var size = (Size)drawX.CRtoXy(inherentGrid[^1].Length - 1, inherentGrid.Length - 1);
            if (size.Height > result.Height)
            {
                result.Height = size.Height;
            }

            if (size.Width > result.Width)
            {
                result.Width = size.Width;
            }

            drawX.MiniSetCol(cols);
            result.Width += drawX.SzPower.Width;
            result.Height = result.Height + drawX.SzPower.Height + ClsDrawX.PaddingY;

            return result;
        }
    }

    public static void MiniSetCol(this ClsDrawX drawX, int cols)
    {
        if (cols == drawX._vcCols)
        {
            return;
        }

        if (cols is < 2 or > 6)
        {
            return;
        }

        drawX._vcCols = cols;
        drawX._vcRowsPowers = ClsDrawX.VcPowers / drawX._vcCols;
    }

    public static Size GetRequiredDrawingArea(this ClsDrawX drawX)
    {
        var maxY = -1;
        var maxX = -1;
        checked
        {
            for (var i = 0; i < MidsContext.Character.CurrentBuild.Powers.Count; i++)
            {
                if (!((MidsContext.Character.CurrentBuild.Powers[i].IDXPower > -1) | MidsContext.Character.CurrentBuild.Powers[i].Chosen))
                {
                    continue;
                }

                var point = drawX.PowerPosition(i);
                if (point.X > maxX)
                {
                    maxX = point.X;
                }

                if (point.Y > maxY)
                {
                    maxY = point.Y;
                }
            }

            if ((maxX > -1) & (maxY > -1))
            {
                return new Size(maxX + drawX.SzPower.Width, maxY + drawX.SzPower.Height + ClsDrawX.PaddingY);
            }


            var point2 = drawX.PowerPosition(MidsContext.Character.CurrentBuild.LastPower);
            return drawX._vcCols != 5
                ? new Size(point2.X + drawX.SzPower.Width,
                    point2.Y + drawX.SzPower.Height + ClsDrawX.PaddingY + ClsDrawX.OffsetInherent)
                : new Size(point2.X + drawX.SzPower.Width,
                    point2.Y + drawX.SzPower.Height + ClsDrawX.PaddingY + ClsDrawX.OffsetInherent);
        }
    }
}