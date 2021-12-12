using System.Collections.Generic;
using mrbBase.Base.Master_Classes;

namespace mrbBase
{
    public class EnhancementSetCollection : List<EnhancementSet>
    {
        public int GetSetBonusEnhCount(int sidx, int bonus)
        {
            return sidx >= 0 && sidx <= Count
                ? string.IsNullOrEmpty(this[sidx].GetEffectString(bonus, false, true))
                    ? string.IsNullOrEmpty(this[sidx].GetEffectString(bonus, true, true)) ? 0 : 1
                    : this[sidx].Bonus[bonus].Slotted
                : 0;
        }

        public static string GetSetInfoLongRTF(int iSet, int enhCount = -1)
        {
            if ((iSet < 0) | (iSet > DatabaseAPI.Database.EnhancementSets.Count - 1)) return "";

            var str3 = ((DatabaseAPI.Database.EnhancementSets[iSet].LevelMin == DatabaseAPI.Database.EnhancementSets[iSet].LevelMax)
                ? RTF.Bold(RTF.Underline(
                    $"{DatabaseAPI.Database.EnhancementSets[iSet].DisplayName} ({DatabaseAPI.Database.EnhancementSets[iSet].LevelMin + 1}): "))
                : RTF.Bold(RTF.Underline(
                    $"{DatabaseAPI.Database.EnhancementSets[iSet].DisplayName} ({DatabaseAPI.Database.EnhancementSets[iSet].LevelMin + 1}-{DatabaseAPI.Database.EnhancementSets[iSet].LevelMax + 1}): ")));
            str3 = $"{RTF.Color(RTF.ElementID.Invention)}{str3}{RTF.Color(RTF.ElementID.Text)}";
            for (var index = 0; index <= DatabaseAPI.Database.EnhancementSets[iSet].Bonus.Length - 1; ++index)
            {
                var effectString = DatabaseAPI.Database.EnhancementSets[iSet].GetEffectString(index, false);
                if (string.IsNullOrEmpty(effectString))
                    continue;
                if (DatabaseAPI.Database.EnhancementSets[iSet].Bonus[index].PvMode == Enums.ePvX.PvP)
                    effectString += " (PvP)";

                var fxColor = ((enhCount >= DatabaseAPI.Database.EnhancementSets[iSet].Bonus[index].Slotted) &
                    ((!MidsContext.Config.Inc.DisablePvE &
                      (DatabaseAPI.Database.EnhancementSets[iSet].Bonus[index].PvMode == Enums.ePvX.PvE)) |
                     (MidsContext.Config.Inc.DisablePvE &
                      (DatabaseAPI.Database.EnhancementSets[iSet].Bonus[index].PvMode == Enums.ePvX.PvP)) |
                     (DatabaseAPI.Database.EnhancementSets[iSet].Bonus[index].PvMode == Enums.ePvX.Any)))
                     ? RTF.ElementID.Invention
                     : RTF.ElementID.Faded;

                str3 += $"{RTF.Crlf()}{RTF.Bold(RTF.Color(RTF.ElementID.Text))}  {DatabaseAPI.Database.EnhancementSets[iSet].Bonus[index].Slotted} Slotted: {RTF.Color(fxColor)}{effectString}{RTF.Color(RTF.ElementID.Text)}";
            }

            for (var index = 0; index <= DatabaseAPI.Database.EnhancementSets[iSet].SpecialBonus.Length - 1; ++index)
            {
                var effectString = DatabaseAPI.Database.EnhancementSets[iSet].GetEffectString(index, true);
                if (!string.IsNullOrEmpty(effectString))
                    str3 += $"{RTF.Crlf()}{RTF.Color(RTF.ElementID.Enhancement)}{RTF.Bold($"  {DatabaseAPI.Database.Enhancements[DatabaseAPI.Database.EnhancementSets[iSet].Enhancements[index]].Name}: ")}{RTF.Color(RTF.ElementID.Faded)}{effectString}{RTF.Color(RTF.ElementID.Text)}";
            }

            return str3;
        }
    }
}