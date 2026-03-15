using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Mids_Reborn.UI.Controls.GfxModules;

public static class GfxUtils
{
    internal static List<IPowerset?> GetDistinctPowersets(this ClsDrawX drawX)
    {
        return drawX.ColumnsPowersLayout
            .DistinctBy(e => e.Value.X)
            .Select(e => MidsContext.Character?.CurrentBuild?.Powers[e.Key]?.Power?.GetPowerSet())
            .ToList();
    }

    internal static int IndexFromLevel(this ClsDrawX drawX)
    {
        return MidsContext.Character.CurrentBuild.Powers.FindIndex(pow => pow?.Level == MidsContext.Character.RequestedLevel);
    }

    // ////////////////////////////////////////////

    public static PathGradientBrush MakePathBrush(Rectangle rect, PointF centerPoint, Color color1, Color color2)
    {
        /*
        var num = (float)(iRect.Left + iRect.Width * 0.5);
           var num2 = (float)(iRect.Top + iRect.Height * 0.5);
           var graphicsPath = new GraphicsPath();
           graphicsPath.AddEllipse(iRect);
           PathGradientBrush pathGradientBrush;
           PathGradientBrush pathGradientBrush2;
           checked
           {
               var array = new Color[graphicsPath.PathPoints.GetUpperBound(0) + 1];
               var lowerBound = graphicsPath.PathPoints.GetLowerBound(0);
               var upperBound = graphicsPath.PathPoints.GetUpperBound(0);
               for (var i = lowerBound; i <= upperBound; i++)
               {
                   array[i] = icolor2;
               }
           
               pathGradientBrush = new PathGradientBrush(graphicsPath)
               {
                   CenterColor = iColor1,
                   SurroundColors = array
               };
               pathGradientBrush2 = pathGradientBrush;
           }
           
           var centerPoint = new PointF((float)(num + (iCenter.X + iCenter.X * (iRect.Width * 0.5))), (float)(num2 + (iCenter.Y + iCenter.Y * (iRect.Height * 0.5))));
           pathGradientBrush2.CenterPoint = centerPoint;
           
           return pathGradientBrush;
         */

        // Prevent Out of Memory exceptions
        if ((rect.Width == 0) & (rect.Height == 0))
        {
            rect.Width = 8;
            rect.Height = 8;
        }
        else if (rect.Width == 0)
        {
            rect.Width = rect.Height;
        }
        else if (rect.Height == 0)
        {
            rect.Height = rect.Width;
        }

        var center2 = new PointF(rect.Left + rect.Width / 2f, rect.Top + rect.Height / 2f);
        var graphicsPath = new GraphicsPath();
        graphicsPath.AddEllipse(rect);
        var array = Enumerable.Repeat(color2, graphicsPath.PathPoints.GetUpperBound(0) + 1).ToArray();
        var pathGradientBrush = new PathGradientBrush(graphicsPath)
        {
            CenterColor = color1,
            SurroundColors = array
        };

        pathGradientBrush.CenterPoint = new PointF(center2.X + (centerPoint.X + centerPoint.X * (rect.Width * 0.5f)), center2.Y + (centerPoint.Y + centerPoint.Y * (rect.Height * 0.5f)));
        
        return pathGradientBrush;
    }
}