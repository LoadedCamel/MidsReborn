using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn
{
    public abstract class clsConvertibleUnitValue
    {
        public static string FormatSpeedUnit()
        {
            return FormatSpeedUnit(MidsContext.Config.SpeedFormat);
        }
        
        public static string FormatSpeedUnit(Enums.eSpeedMeasure speedUnit)
        {
            return speedUnit switch
            {
                Enums.eSpeedMeasure.FeetPerSecond => "ft/s",
                Enums.eSpeedMeasure.MetersPerSecond => "m/s",
                Enums.eSpeedMeasure.MilesPerHour => "mph",
                Enums.eSpeedMeasure.KilometersPerHour => "km/h",
                _ => "ft/s"
            };
        }

        public static string FormatDistanceUnit()
        {
            return FormatDistanceUnit(MidsContext.Config.SpeedFormat);
        }

        public static string FormatDistanceUnit(Enums.eSpeedMeasure distanceUnit)
        {
            return distanceUnit switch
            {
                Enums.eSpeedMeasure.FeetPerSecond => "ft",
                Enums.eSpeedMeasure.MetersPerSecond => "m",
                Enums.eSpeedMeasure.MilesPerHour => "ft",
                Enums.eSpeedMeasure.KilometersPerHour => "m",
                _ => "ft"
            };
        }
    }
}