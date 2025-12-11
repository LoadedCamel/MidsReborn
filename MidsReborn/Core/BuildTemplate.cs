using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.UI.Forms;

namespace Mids_Reborn.Core
{
    public class BuildTemplate
    {
        public struct PowerInfo
        {
            public int NidPower;
            public bool? Active;
            public int Level;
            public SlotEntry[]? Slots;
        }

        public int[] PoolSelections;
        public List<PowerInfo> PickedPowers;
        public string? Name = null;

        public static BuildTemplate SnapshotBuild(MainWindow2 f)
        {
            var poolSelections = f.GetCbPoolsIndices(false);
            var pickedPowers = MidsContext.Character?.CurrentBuild?.Powers
                .Where(e => e?.Power is not null) // Actually picked powers
                .Where(e => e?.Power?.GetPowerSet()?.SetType is not (Enums.ePowerSetType.Primary or Enums.ePowerSetType.Secondary or Enums.ePowerSetType.Ancillary)) // Only non archetype related powers
                .Where(e => e?.Power?.Requires.ClassName.Length <= 0) // No archetype requirements (filter outs Defiance, Supremacy, etc.)
                .Where(e => e?.Power?.FullName is not "Inherent.Inherent.Special_Set_Bonuses") // Filter out default inherents
                //.Where(e => e?.Power?.FullName is not ("Inherent.Inherent.Brawl" or "Inherent.Inherent.Sprint" or "Inherent.Inherent.Rest" or "Inherent.Inherent.Special_Set_Bonuses")) // Filter out default inherents
                //.Where(e => e?.Power?.FullName.StartsWith("Inherent.Fitness.") == false) // Filter out default inherent Fitness
                .Select(e => new PowerInfo
                {
                    NidPower = e!.NIDPower,
                    Active = (e.Power!.PowerType == Enums.ePowerType.Toggle) | e.Power!.ClickBuff ? e.StatInclude : null,
                    Level = e.Level,
                    Slots = e.Slots.Clone() as SlotEntry[]
                })
                .ToList();

            return new BuildTemplate(poolSelections, pickedPowers);
        }

        private BuildTemplate(int[] poolSelections, List<PowerInfo> pickedPowers)
        {
            PoolSelections = poolSelections;
            PickedPowers = pickedPowers;
        }

        public BuildTemplate()
        {
            PoolSelections = [0, 0, 0, 0];
            PickedPowers = [];
        }

        public void InjectToBuild(MainWindow2 f, bool draw = true)
        {
            // Combo boxes
            for (var i = 0; i < PoolSelections.Length; i++)
            {
                f.SetCbPoolIndex(i, Math.Max(0, PoolSelections[i]));
                var selectedPoolItem = f.GetCbPoolIndex(i, true) as string;
                if (selectedPoolItem == null)
                {
                    continue;
                }

                MidsContext.Character.Powersets[i + 3] = DatabaseAPI.GetPowersetByName(selectedPoolItem, Enums.ePowerSetType.Pool);
                Debug.WriteLine($"PoolSelection[{i}]={PoolSelections[i]} / {DatabaseAPI.GetPowersetByName(selectedPoolItem, Enums.ePowerSetType.Pool)?.FullName}");
            }

            var buildMode = MidsContext.Config.BuildMode;

            if (buildMode == Enums.dmModes.LevelUp)
            {
                MidsContext.Config.BuildMode = Enums.dmModes.Respec;
            }

            /*// Pools
            var powersetsFull = PickedPowers
                .Select(e => DatabaseAPI.Database.Power[e.NidPower]?.GetPowerSet()?.FullName)
                .Where(e => e != null)
                .Distinct()
                .Cast<string>()
                .ToList();

            var listPowersets = new UniqueList<string>();
            listPowersets.FromList(powersetsFull);

            ImportBase.FixUndetectedPowersets(ref listPowersets);
            ImportBase.PadPowerPools(ref listPowersets);
            ImportBase.SortPowersets(ref listPowersets);

            var toBlameSet = string.Empty;
            MidsContext.Character.LoadPowersetsByName2(listPowersets, ref toBlameSet);*/
            MidsContext.Character.CurrentBuild.LastPower = 24;

            // Powers
            try
            {
                for (var k = 0; k < PickedPowers.Count; k++)
                {
                    var power = DatabaseAPI.Database.Power[PickedPowers[k].NidPower];
                    
                    // Incarnate, Temps, Accolades
                    if ((power?.FullName.StartsWith("Incarnate") == true) |
                        (power?.FullName.StartsWith("Temporary_Powers") == true))
                    {
                        if (!MidsContext.Character.CurrentBuild.PowerUsed(power))
                        {
                            if (PickedPowers[k].Active != null)
                            {
                                MidsContext.Character.CurrentBuild.AddPower(power, 49).StatInclude = PickedPowers[k].Active!.Value;
                            }
                            else
                            {
                                MidsContext.Character.CurrentBuild.AddPower(power, 49);
                            }
                        }

                        continue;
                    }

                    // Regular powers
                    var ps = power?.GetPowerSet();
                    if (ps != null)
                    {
                        f.PowerPickedNoRedraw(ps.nID, PickedPowers[k].NidPower);
                    }
                }
            }
            catch (Exception ex)
            {
                MidsContext.Config.BuildMode = buildMode;
                MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}");
            }

            // Slots

            var sl = new SlotLevelQueue();
            try
            {
                for (var k = 0; k < PickedPowers.Count; k++)
                {
                    var power = DatabaseAPI.Database.Power[PickedPowers[k].NidPower];
                    if (power?.Slottable != true)
                    {
                        continue;
                    }

                    var pe = MidsContext.Character.CurrentBuild.Powers
                        .DefaultIfEmpty(null)
                        .FirstOrDefault(e => e is { Power: not null } && e.NIDPower == PickedPowers[k].NidPower);

                    if (pe == null)
                    {
                        continue;
                    }

                    while (pe.Slots.Length < PickedPowers[k].Slots?.Length)
                    {
                        pe.AddSlot(Character.MaxLevel);
                    }

                    for (var i = 0; i < pe.Slots.Length; i++)
                    {
                        pe.Slots[i].Level = i == 0 ? pe.Level : sl.PickSlot();
                        if (PickedPowers[k].Slots?[i].Enhancement == null || PickedPowers[k].Slots?[i].Enhancement.Enh < 0)
                        {
                            continue;
                        }

                        pe.Slots[i].Enhancement = (PickedPowers[k].Slots![i].Enhancement.Clone() as I9Slot)!;
                        pe.Slots[i].Enhancement.Obtained = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MidsContext.Config.BuildMode = buildMode;
                MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}");
            }

            f.FixStatIncludes();
            MidsContext.Character.PoolShuffle();
            f.SetEnhCheckModePosition();

            var idx = -1;
            if (MidsContext.Config.BuildMode is Enums.dmModes.Normal or Enums.dmModes.Respec)
            {
                idx = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex(MainModule.MidsController.Toon.RequestedLevel);
                if (idx < 0)
                {
                    idx = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex();
                }
            }
            else if (DatabaseAPI.Database.Levels[MidsContext.Character.Level].LevelType() == Enums.dmItem.Power)
            {
                idx = MainModule.MidsController.Toon.GetFirstAvailablePowerIndex();
                f.SetPowerEntryHighlight();
            }

            if (MainModule.MidsController.Toon.Complete)
            {
                f.SetPowerEntryHighlight();
            }

            if ((idx > -1) & (idx <= MidsContext.Character.CurrentBuild.Powers.Count))
            {
                MidsContext.Character.RequestedLevel = MidsContext.Character.CurrentBuild.Powers[idx].Level;
                MidsContext.Character.SetLevelTo(MidsContext.Character.CurrentBuild.Powers[idx].Level);
            }
            else
            {
                MidsContext.Character.RequestedLevel = Character.MaxLevel;
                MidsContext.Character.SetLevelTo(Character.MaxLevel);
            }

            MidsContext.Character.ResetLevel();
            MidsContext.Character.PoolShuffle();
            var powerEntryArray = MainWindow2.DeepCopyPowerList();
            f.RearrangeAllSlotsInBuild(powerEntryArray, true);
            MainWindow2.ShallowCopyPowerList(powerEntryArray);
            f.PowerModified(false);
            f.SetFileModified(false);
            MidsContext.Config.BuildMode = buildMode;

            if (!draw)
            {
                return;
            }

            f.DoRedraw();
        }
    }
}
