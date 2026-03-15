using System.Drawing.Imaging;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.UI.Controls.GfxModules;

public static class GfxColorFilters
{
    public static void UpdateImageAttributes(this ClsDrawX drawX, ref DrawVariables drawVars)
    {
        bool grey;
        ImageAttributes? imageAttr;
        var toggling = drawX.InterfaceMode == Enums.eInterfaceMode.PowerToggle;
        if (toggling)
        {
            drawVars.PowerState = drawVars.PowerState switch
            {
                Enums.ePowerState.Open => Enums.ePowerState.Empty,
                _ => drawVars.PowerState
            };
            switch (drawVars.PowerEntry.StatInclude & (drawVars.PowerState == Enums.ePowerState.Used))
            {
                case true:
                    drawVars.PowerState = Enums.ePowerState.Open;
                    grey = drawVars.PowerEntry.Level >= MidsContext.Config.ForceLevel;
                    imageAttr = drawX.GreySlot(grey, true);
                    break;
                default:
                {
                    if (drawVars.PowerEntry.CanIncludeForStats())
                    {
                        grey = drawVars.PowerEntry.Level >= MidsContext.Config.ForceLevel;
                        imageAttr = drawX.GreySlot(grey);
                    }
                    else
                    {
                        imageAttr = drawX.GreySlot(true);
                        grey = true;
                    }

                    break;
                }
            }
        }
        else
        {
            grey = drawVars.PowerEntry.Level >= MidsContext.Config.ForceLevel;
            imageAttr = drawX.GreySlot(grey);
        }

        drawVars.PowerRect = drawX.DrawPowerImage(drawVars.PowerEntry, drawVars.Location, drawVars.PowerState, toggling, imageAttr, grey);
    }

    public static void ColorSwitch(this ClsDrawX drawX)
    {
        /*bool useHeroColors = true;
        if (MidsContext.Character != null)
            useHeroColors = MidsContext.Character.IsHero();
        if (MidsContext.Config.DisableVillainColors)
            useHeroColors = true;
        VillainColor = !useHeroColors;*/
        drawX._pColorMatrix = new ColorMatrix(ClsDrawX.HeroMatrix);
        drawX.PImageAttributes ??= new ImageAttributes();
        drawX.PImageAttributes.SetColorMatrix(drawX._pColorMatrix);
    }

    public static ImageAttributes GetRecolorIa(this ClsDrawX drawX)
    {
        var colorMatrix = new ColorMatrix(ClsDrawX.HeroMatrix);
        var imageAttributes = new ImageAttributes();
        imageAttributes.SetColorMatrix(colorMatrix);
        
        return imageAttributes;
    }

    internal static ImageAttributes? GreySlot(this ClsDrawX drawX, bool grey, bool bypassIa = false)
    {
        if (!grey) return bypassIa ? new ImageAttributes() : drawX.PImageAttributes;

        checked
        {
            var colorMatrix = new ColorMatrix(ClsDrawX.HeroMatrix);
            var r = 0;
            do
            {
                var c = 0;
                do
                {
                    if (!bypassIa)
                    {
                        colorMatrix[r, c] = drawX._pColorMatrix[r, c];
                    }

                    colorMatrix[r, c] /= 1.5f;

                    c++;
                } while (c <= 2);

                r++;
            } while (r <= 2);

            var imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(colorMatrix);
            
            return imageAttributes;
        }
    }

    internal static ImageAttributes? Desaturate(this ClsDrawX drawX, bool grey, bool bypassIa = false)
    {
        var tMm = new ColorMatrix(ClsDrawX.DesaturateMatrix);
        var tCm = new ColorMatrix(ClsDrawX.HeroMatrix);
        var r = 0;
        checked
        {
            do
            {
                var c = 0;
                do
                {
                    //controls shading of inherents
                    if (!bypassIa)
                    {
                        tCm[r, c] = ((drawX._pColorMatrix == null ? 0 : drawX._pColorMatrix[r, c]) + tMm[r, c]) / 2f;
                    }

                    if (grey)
                    {
                        tCm[r, c] /= 1.5f;
                    }

                    c++;
                } while (c <= 2);

                r++;
            } while (r <= 2);

            var imageAttributes = new ImageAttributes();
            imageAttributes.SetColorMatrix(tCm);
            
            return imageAttributes;
        }
    }
}