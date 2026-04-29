using System;
using System.Linq;

namespace Mids_Reborn.Core;

internal static class EnhancementScheduleMath
{
    internal static float GetRuntimeScheduleBaseScale(
        Enums.eType enhancementType,
        Enums.eEnhGrade grade,
        int ioLevel,
        Enums.eSchedule schedule)
    {
        if (schedule is Enums.eSchedule.None or Enums.eSchedule.Multiple)
        {
            return 0f;
        }

        var scheduleIndex = (int)schedule;
        return enhancementType switch
        {
            Enums.eType.Normal => grade switch
            {
                Enums.eEnhGrade.None => 0f,
                Enums.eEnhGrade.TrainingO => GetTableScale(DatabaseAPI.Database.MultTO, 0, scheduleIndex),
                Enums.eEnhGrade.DualO => GetTableScale(DatabaseAPI.Database.MultDO, 0, scheduleIndex),
                Enums.eEnhGrade.SingleO => GetTableScale(DatabaseAPI.Database.MultSO, 0, scheduleIndex),
                _ => 0f
            },
            Enums.eType.InventO or Enums.eType.SetO => GetTableScale(DatabaseAPI.Database.MultIO, ioLevel, scheduleIndex),
            Enums.eType.SpecialO => GetTableScale(DatabaseAPI.Database.MultHO, 0, scheduleIndex),
            _ => 0f
        };
    }

    internal static float NormalizeImportedScaleToMultiplier(
        Enums.eType enhancementType,
        Enums.eSchedule schedule,
        float sourceScale)
    {
        if (schedule is Enums.eSchedule.None or Enums.eSchedule.Multiple ||
            enhancementType is Enums.eType.InventO or Enums.eType.SetO)
        {
            return sourceScale;
        }

        var baseScale = GetClosestNormalizationBaseScale(enhancementType, schedule, sourceScale);
        if (baseScale <= float.Epsilon)
        {
            return sourceScale;
        }

        var multiplier = sourceScale / baseScale;
        return Math.Abs(multiplier - 1f) < 0.02f ? 1f : multiplier;
    }

    private static float GetClosestNormalizationBaseScale(
        Enums.eType enhancementType,
        Enums.eSchedule schedule,
        float sourceScale)
    {
        var scheduleIndex = (int)schedule;
        if (scheduleIndex < 0 || scheduleIndex > 3)
        {
            return 0f;
        }

        if (enhancementType == Enums.eType.SpecialO)
        {
            return GetTableScale(DatabaseAPI.Database.MultHO, 0, scheduleIndex);
        }

        var candidates = new[]
            {
                GetTableScale(DatabaseAPI.Database.MultTO, 0, scheduleIndex),
                GetTableScale(DatabaseAPI.Database.MultDO, 0, scheduleIndex),
                GetTableScale(DatabaseAPI.Database.MultSO, 0, scheduleIndex)
            }
            .Where(value => value > float.Epsilon)
            .OrderBy(value => Math.Abs(value - Math.Abs(sourceScale)))
            .ToArray();

        return candidates.FirstOrDefault();
    }

    private static float GetTableScale(float[][]? table, int rowIndex, int scheduleIndex)
    {
        if (table is not { Length: > 0 } ||
            rowIndex < 0 ||
            rowIndex >= table.Length ||
            table[rowIndex] == null ||
            scheduleIndex < 0 ||
            scheduleIndex >= table[rowIndex].Length)
        {
            return 0f;
        }

        return table[rowIndex][scheduleIndex];
    }
}
