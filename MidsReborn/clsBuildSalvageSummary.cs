using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;
using static Mids_Reborn.Core.Files.Headers;

namespace Mids_Reborn
{
    public static class BuildSalvageSummary
    {
        private static readonly Color ColorNormalText = Color.WhiteSmoke;
        private static readonly Color ColorFullBuild = Color.FromArgb(0, 255, 128);

        public static int EnhObtained { get; private set; }
        public static int EnhCatalysts { get; private set; }
        public static int EnhBoosters { get; private set; }
        public static int TotalEnhancements { get; private set; }

        private static void CalcAll()
        {
            TotalEnhancements = 0;
            EnhObtained = 0;
            EnhCatalysts = 0;
            EnhBoosters = 0;

            if (MidsContext.Character.CurrentBuild?.Powers == null) return;
            for (var index = 0; index < MidsContext.Character.CurrentBuild.Powers.Count; index++)
            {
                var powerEntry = MidsContext.Character.CurrentBuild.Powers[index];
                if (powerEntry == null) continue;
                var slots = powerEntry.Slots;
                if (slots.Length < 1) continue;
                for (var i = 0; i < slots.Length; i++)
                {
                    var slot = slots[i];
                    if (slot.Enhancement.Enh > -1)
                    {
                        Debug.WriteLine($"{Database.Instance.Enhancements[slot.Enhancement.Enh].LongName} - HasBeenObtained: {slot.Enhancement.Obtained}");
                    }
                }
            }


            for (var powerEntryIndex = 0; powerEntryIndex < MidsContext.Character.CurrentBuild.Powers.Count; powerEntryIndex++)
            {
                var p = MidsContext.Character.CurrentBuild.Powers[powerEntryIndex];
                if (p == null) continue;
                for (var j = 0; j < p.Slots.Length; j++)
                {
                    var enhIdx = p.Slots[j].Enhancement.Enh;

                    if (enhIdx > -1)
                    {
                        TotalEnhancements++;
                    }
                    if (p.Slots[j].Enhancement.Obtained & enhIdx > -1)
                    {
                        EnhObtained++;
                    }

                    if (enhIdx == -1) continue;

                    var enhName = Database.Instance.Enhancements[enhIdx].UID;
                    if (DatabaseAPI.EnhHasCatalyst(enhName) && DatabaseAPI.EnhIsSuperior(enhIdx)) EnhCatalysts++;

                    var relativeLevel = p.Slots[j].Enhancement.RelativeLevel;
                    if (DatabaseAPI.EnhIsIO(enhIdx))
                    {
                        EnhBoosters += relativeLevel switch
                        {
                            Enums.eEnhRelative.PlusOne => 1,
                            Enums.eEnhRelative.PlusTwo => 2,
                            Enums.eEnhRelative.PlusThree => 3,
                            Enums.eEnhRelative.PlusFour => 4,
                            Enums.eEnhRelative.PlusFive => 5,
                            _ => 0
                        };
                    }
                }
            }
        }

        public static void CalcTotalEnhancements()
        {
            TotalEnhancements = 0;
            foreach (var p in MidsContext.Character.CurrentBuild.Powers)
            {
                for (var j = 0; j < p.Slots.Length; j++)
                {
                    if (p.Slots[j].Enhancement.Enh > -1)
                    {
                        TotalEnhancements++;
                    }
                }
            }
        }

        private static void CalcEnhObtained()
        {
            EnhObtained = 0;
            if (MidsContext.Character.CurrentBuild == null) return;
            foreach (var p in MidsContext.Character.CurrentBuild.Powers)
            {
                for (var j = 0; j < p?.Slots.Length; j++)
                {
                    if (p.Slots[j].Enhancement.Obtained & p.Slots[j].Enhancement.Enh > -1)
                    {
                        EnhObtained++;
                    }
                }
            }
        }

        public static void CalcEnhCatalysts()
        {
            EnhCatalysts = 0;
            foreach (var p in MidsContext.Character.CurrentBuild.Powers)
            {
                for (var j = 0; j < p?.Slots.Length; j++)
                {
                    var enhIdx = p.Slots[j].Enhancement.Enh;
                    if (enhIdx == -1) continue;
                    var enhName = Database.Instance.Enhancements[enhIdx].UID;

                    if (DatabaseAPI.EnhHasCatalyst(enhName) && DatabaseAPI.EnhIsSuperior(enhIdx)) EnhCatalysts++;
                }
            }
        }

        public static void CalcEnhBoosters()
        {
            EnhBoosters = 0;
            foreach (var p in MidsContext.Character.CurrentBuild.Powers)
            {
                for (var j = 0; j < p.Slots.Length; j++)
                {
                    var enhIdx = p.Slots[j].Enhancement.Enh;
                    if (enhIdx == -1) continue;
                    if (!DatabaseAPI.EnhIsIO(enhIdx)) continue;

                    var relativeLevel = p.Slots[j].Enhancement.RelativeLevel;
                    EnhBoosters += relativeLevel switch
                    {
                        Enums.eEnhRelative.PlusOne => 1,
                        Enums.eEnhRelative.PlusTwo => 2,
                        Enums.eEnhRelative.PlusThree => 3,
                        Enums.eEnhRelative.PlusFour => 4,
                        Enums.eEnhRelative.PlusFive => 5,
                        _ => 0
                    };
                }
            }
        }

        public static void UpdateAllSalvage(Label lblEnhObtained, Label lblCatalysts, Label lblBoosters)
        {
            CalcAll();
            lblEnhObtained.ForeColor = EnhObtained == TotalEnhancements & TotalEnhancements > 0
                ? ColorFullBuild
                : ColorNormalText;
            lblEnhObtained.Text = $@"Obtained: {EnhObtained}/{TotalEnhancements}";
            lblCatalysts.Text = EnhCatalysts == 0
                ? "--"
                : $"x{EnhCatalysts}";
            lblBoosters.Text = EnhBoosters == 0
                ? "--"
                : $"x{EnhBoosters}";
        }

        public static void UpdateAllSalvage(Control? control, bool useNegativeCount = false)
        {
            CalcAll();
            switch (control)
            {
                case EnhCheckMode mode:
                    mode.EnhObtainedColor = EnhObtained == TotalEnhancements & TotalEnhancements > 0
                        ? ColorFullBuild
                        : ColorNormalText;
                    mode.EnhObtained = useNegativeCount
                        ? EnhObtained == TotalEnhancements & TotalEnhancements > 0
                            ? "Complete"
                            : $"{TotalEnhancements - EnhObtained} to go"
                        : $@"Obtained: {EnhObtained}/{TotalEnhancements}";
                    mode.Catalyst = EnhCatalysts == 0
                        ? "--"
                        : $"x{EnhCatalysts}";
                    mode.Boosters = EnhBoosters == 0
                        ? "--"
                        : $"x{EnhBoosters}";
                    break;
            }
        }

        public static void UpdateEnhObtained(Label lblEnhObtained, bool useNegativeCount = false)
        {
            CalcEnhObtained();
            lblEnhObtained.ForeColor = EnhObtained == TotalEnhancements & TotalEnhancements > 0
                ? ColorFullBuild
                : ColorNormalText;
            lblEnhObtained.Text = useNegativeCount
                ? EnhObtained == TotalEnhancements & TotalEnhancements > 0
                    ? "Complete"
                    : $"{TotalEnhancements - EnhObtained} to go"
                : $@"Obtained: {EnhObtained}/{TotalEnhancements}";
        }

        public static void UpdateEnhObtained(Control? control, bool useNegativeCount = false)
        {
            CalcEnhObtained();
            switch (control)
            {
                case EnhCheckMode mode:
                    mode.EnhObtainedColor = EnhObtained == TotalEnhancements & TotalEnhancements > 0
                        ? ColorFullBuild
                        : ColorNormalText;
                    mode.EnhObtained = useNegativeCount
                        ? EnhObtained == TotalEnhancements & TotalEnhancements > 0
                            ? "Complete"
                            : $"{TotalEnhancements - EnhObtained} to go"
                        : $@"Obtained: {EnhObtained}/{TotalEnhancements}";
                    break;
            }
        }
    }
}