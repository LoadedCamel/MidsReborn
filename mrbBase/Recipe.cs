using System;
using System.IO;

namespace mrbBase
{
    public class Recipe
    {
        public enum RecipeRarity
        {
            Common,
            Uncommon,
            Rare,
            UltraRare
        }

        public string Enhancement = string.Empty;
        public int EnhIdx = -1;
        public string ExternalName = string.Empty;
        public string InternalName = string.Empty;
        public RecipeEntry[] Item = new RecipeEntry[0];
        public RecipeRarity Rarity;
        public bool IsVirtual;
        public bool IsGeneric;
        public bool IsHidden;

        public Recipe()
        {
        }

        public Recipe(BinaryReader reader, bool useOld = false)
        {
            if (!useOld)
            {
                Rarity = (RecipeRarity) reader.ReadInt32();
                InternalName = reader.ReadString();
                ExternalName = reader.ReadString();
                Enhancement = reader.ReadString();
                Item = new RecipeEntry[reader.ReadInt32() + 1];
                for (var index1 = 0; index1 < Item.Length; index1++)
                {
                    Item[index1] = new RecipeEntry
                    {
                        Level = reader.ReadInt32(),
                        BuyCost = reader.ReadInt32(),
                        CraftCost = reader.ReadInt32(),
                        BuyCostM = reader.ReadInt32(),
                        CraftCostM = reader.ReadInt32()
                    };
                    var num = reader.ReadInt32();
                    Item[index1].Salvage = new string[num + 1];
                    Item[index1].Count = new int[num + 1];
                    Item[index1].SalvageIdx = new int[num + 1];
                    for (var index2 = 0; index2 < Item[index1].Salvage.Length; ++index2)
                    {
                        Item[index1].Salvage[index2] = reader.ReadString();
                        Item[index1].Count[index2] = reader.ReadInt32();
                        Item[index1].SalvageIdx[index2] = reader.ReadInt32();
                        Item[index1].RecipeIdx[index2] = reader.ReadInt32();
                    }
                }

                //IsGeneric = reader.ReadBoolean();
                //IsVirtual = reader.ReadBoolean();
                //IsHidden = reader.ReadBoolean();
            }
            else
            {
                Rarity = (RecipeRarity)reader.ReadInt32();
                InternalName = reader.ReadString();
                ExternalName = reader.ReadString();
                Enhancement = reader.ReadString();
                Item = new RecipeEntry[reader.ReadInt32() + 1];
                for (int index1 = 0; index1 < Item.Length; ++index1)
                {
                    Item[index1] = new RecipeEntry
                    {
                        Level = reader.ReadInt32(),
                        BuyCost = reader.ReadInt32(),
                        CraftCost = reader.ReadInt32(),
                        BuyCostM = reader.ReadInt32(),
                        CraftCostM = reader.ReadInt32()
                    };
                    int num = reader.ReadInt32();
                    Item[index1].Salvage = new string[num + 1];
                    Item[index1].Count = new int[num + 1];
                    Item[index1].SalvageIdx = new int[num + 1];
                    for (int index2 = 0; index2 < Item[index1].Salvage.Length; ++index2)
                    {
                        Item[index1].Salvage[index2] = reader.ReadString();
                        Item[index1].Count[index2] = reader.ReadInt32();
                        Item[index1].SalvageIdx[index2] = reader.ReadInt32();
                    }
                }
            }
        }

        public void StoreTo(BinaryWriter writer)
        {
            writer.Write((int) Rarity);
            writer.Write(InternalName);
            writer.Write(ExternalName);
            writer.Write(Enhancement);
            writer.Write(Item.Length - 1);
            foreach (var r in Item)
            {
                writer.Write(r.Level);
                writer.Write(r.BuyCost);
                writer.Write(r.CraftCost);
                writer.Write(r.BuyCostM);
                writer.Write(r.CraftCostM);
                writer.Write(r.Salvage.Length - 1);
                for (var index2 = 0; index2 < r.Salvage.Length; index2++)
                {
                    writer.Write(r.Salvage[index2]);
                    writer.Write(r.Count[index2]);
                    writer.Write(r.SalvageIdx[index2]);
                    writer.Write(r.RecipeIdx[index2]);
                }
            }
            //writer.Write(IsGeneric);
            //writer.Write(IsVirtual);
            //writer.Write(IsHidden);
        }

        public Recipe(ref Recipe iRecipe)
        {
            Rarity = iRecipe.Rarity;
            InternalName = iRecipe.InternalName;
            ExternalName = iRecipe.ExternalName;
            Enhancement = iRecipe.Enhancement;
            EnhIdx = iRecipe.EnhIdx;
            Item = new RecipeEntry[iRecipe.Item.Length];
            IsVirtual = iRecipe.IsVirtual;
            IsGeneric = iRecipe.IsGeneric;
            IsHidden = iRecipe.IsHidden;
            for (var index1 = 0; index1 < iRecipe.Item.Length; index1++)
            {
                Item[index1] = new RecipeEntry
                {
                    Level = iRecipe.Item[index1].Level,
                    BuyCost = iRecipe.Item[index1].BuyCost,
                    CraftCost = iRecipe.Item[index1].CraftCost,
                    BuyCostM = iRecipe.Item[index1].BuyCostM,
                    CraftCostM = iRecipe.Item[index1].CraftCostM,
                    Salvage = new string[iRecipe.Item[index1].Salvage.Length],
                    SalvageIdx = new int[iRecipe.Item[index1].Salvage.Length],
                    Count = new int[iRecipe.Item[index1].Salvage.Length]
                };
                for (var index2 = 0; index2 <= Item[index1].Salvage.Length - 1; ++index2)
                {
                    Item[index1].Salvage[index2] = iRecipe.Item[index1].Salvage[index2];
                    Item[index1].SalvageIdx[index2] = iRecipe.Item[index1].SalvageIdx[index2];
                    Item[index1].Count[index2] = iRecipe.Item[index1].Count[index2];
                }
            }
        }

        public class RecipeEntry : ICloneable
        {
            public int BuyCost;
            public int BuyCostM;
            public int[] Count = new int[7];
            public int CraftCost;
            public int CraftCostM;
            public int Level;
            public string[] Salvage = new string[7];
            public int[] SalvageIdx = new int[7];
            public string[] Recipe = new string[7];
            public int[] RecipeIdx = new int[7];

            public RecipeEntry()
            {
                for (var index = 0; index <= 6; ++index)
                {
                    Salvage[index] = string.Empty;
                    SalvageIdx[index] = -1;
                    Count[index] = 0;
                    Recipe[index] = "";
                    RecipeIdx[index] = -1;
                }
            }

            public RecipeEntry(RecipeEntry iRe)
            {
                Level = iRe.Level;
                BuyCost = iRe.BuyCost;
                CraftCost = iRe.CraftCost;
                BuyCostM = iRe.BuyCostM;
                CraftCostM = iRe.CraftCostM;
                Salvage = new string[iRe.Salvage.Length];
                SalvageIdx = new int[iRe.Salvage.Length];
                Count = new int[iRe.Salvage.Length];
                for (var index = 0; index < iRe.Salvage.Length; ++index)
                {
                    Salvage[index] = iRe.Salvage[index];
                    SalvageIdx[index] = iRe.SalvageIdx[index];
                    Count[index] = iRe.Count[index];
                    Recipe[index] = "";
                    RecipeIdx[index] = -1;
                }
            }

            public object Clone()
            {
                return MemberwiseClone();
            }
        }
    }
}