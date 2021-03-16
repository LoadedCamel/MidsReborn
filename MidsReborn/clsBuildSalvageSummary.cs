using System.Drawing;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbBase.Base.Master_Classes;
using System.Windows.Forms;

namespace Mids_Reborn
{
    public static class BuildSalvageSummary
    {
        private static readonly Color ColorNormalText = Color.White;
        private static readonly Color ColorFullBuild = Color.FromArgb(0, 255, 128);

        public static int EnhObtained { get; private set; }
        public static int EnhCatalysts { get; private set; }
        public static int EnhBoosters { get; private set; }
        public static int TotalEnhancements { get; private set; }

        public static void CalcAll()
        {
            TotalEnhancements = 0;
            EnhObtained = 0;
            EnhCatalysts = 0;
            EnhBoosters = 0;

            foreach (var p in MidsContext.Character.CurrentBuild.Powers)
            {
                for (var j = 0; j < p.Slots.Length; j++)
                {
                    var enhIdx = p.Slots[j].Enhancement.Enh;

                    if (enhIdx > -1) TotalEnhancements++;
                    if (p.Slots[j].Enhancement.Obtained & enhIdx > -1) EnhObtained++;
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

        public static void CalcEnhObtained()
        {
            EnhObtained = 0;
            foreach (var p in MidsContext.Character.CurrentBuild.Powers)
            {
                for (var j = 0; j < p.Slots.Length; j++)
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
                for (var j = 0; j < p.Slots.Length; j++)
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
            lblEnhObtained.Text = $"Obtained: {EnhObtained}/{TotalEnhancements}";
            lblCatalysts.Text = EnhCatalysts == 0
                ? "--"
                : $"x{EnhCatalysts}";
            lblBoosters.Text = EnhBoosters == 0
                ? "--"
                : $"x{EnhBoosters}";
        }

        public static void UpdateEnhObtained(Label lblEnhObtained)
        {
            CalcEnhObtained();
            lblEnhObtained.ForeColor = EnhObtained == TotalEnhancements & TotalEnhancements > 0
                ? ColorFullBuild
                : ColorNormalText;
            lblEnhObtained.Text = $"Obtained: {EnhObtained}/{TotalEnhancements}";
        }
    }
}