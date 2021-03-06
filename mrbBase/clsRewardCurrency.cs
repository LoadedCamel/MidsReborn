using System;
using System.Collections.Generic;
using System.Drawing;
using mrbBase.Base.Display;
using mrbBase.Base.Master_Classes;

namespace mrbBase
{
    public static class clsRewardCurrency
    {
        // Allowed -direct- conversions.
        // Game does not always allow bidirectional conversions
        // but we need this for calculations.
        private static readonly Dictionary<KeyValuePair<Enums.RewardCurrency, Enums.RewardCurrency>, KeyValuePair<int, int>> AllowedConversions =
            new Dictionary<KeyValuePair<Enums.RewardCurrency, Enums.RewardCurrency>, KeyValuePair<int, int>>
            {
                {
                    new KeyValuePair<Enums.RewardCurrency, Enums.RewardCurrency>(Enums.RewardCurrency.VanguardMerit, Enums.RewardCurrency.RewardMerit),
                    new KeyValuePair<int, int>(30, 1)
                },
                {
                    new KeyValuePair<Enums.RewardCurrency, Enums.RewardCurrency>(Enums.RewardCurrency.AstralMerit, Enums.RewardCurrency.RewardMerit),
                    new KeyValuePair<int, int>(1, 2)
                },
                {
                    new KeyValuePair<Enums.RewardCurrency, Enums.RewardCurrency>(Enums.RewardCurrency.EmpyreanMerit, Enums.RewardCurrency.RewardMerit),
                    new KeyValuePair<int, int>(10, 1)
                },
                {
                    new KeyValuePair<Enums.RewardCurrency, Enums.RewardCurrency>(Enums.RewardCurrency.EmpyreanMerit, Enums.RewardCurrency.AstralMerit),
                    new KeyValuePair<int, int>(1, 5)
                },
                {
                    new KeyValuePair<Enums.RewardCurrency, Enums.RewardCurrency>(Enums.RewardCurrency.AlignmentMerit, Enums.RewardCurrency.RewardMerit),
                    new KeyValuePair<int, int>(1, 50)
                },
                {
                    new KeyValuePair<Enums.RewardCurrency, Enums.RewardCurrency>(Enums.RewardCurrency.Influence, Enums.RewardCurrency.RewardMerit),
                    new KeyValuePair<int, int>(1000000, 1)
                },
            };

        public static int? CurrencyChange(Enums.RewardCurrency c1, Enums.RewardCurrency c2, int amount)
        {
            if (c1 == c2) return amount;
            if (amount == 0) return 0; // Approved by Captain Obvious.

            var path = new List<Enums.RewardCurrency>();

            var c = c1;
            while (c != c2)
            {
                var linkFound = false;
                foreach (var k in Enum.GetValues(typeof(Enums.RewardCurrency)))
                {
                    var kv = (Enums.RewardCurrency) k;
                    if (path.Contains(kv)) continue;
                    var cLink = new KeyValuePair<Enums.RewardCurrency, Enums.RewardCurrency>(c, kv);
                    var cLinkRev = new KeyValuePair<Enums.RewardCurrency, Enums.RewardCurrency>(kv, c);

                    if (AllowedConversions.ContainsKey(cLink))
                    {
                        path.Add(c);
                        var cnvRate = AllowedConversions[cLink];
                        amount = (int) Math.Floor((decimal) (amount / cnvRate.Key * cnvRate.Value));
                        c = kv;
                        linkFound = true;
                    }
                    else if (AllowedConversions.ContainsKey(cLinkRev))
                    {
                        path.Add(c);
                        var cnvRate = AllowedConversions[cLinkRev];
                        amount = (int) Math.Floor((decimal) (amount / cnvRate.Value * cnvRate.Key));
                        c = kv;
                        linkFound = true;
                    }
                }

                if (!linkFound) return null;
            }

            return amount;
        }

        public static int? GetSalvageCost(int salvageIdx, Enums.RewardCurrency c = Enums.RewardCurrency.RewardMerit)
        {
            if (salvageIdx < 0 || salvageIdx >= DatabaseAPI.Database.Salvage.Length) return null;

            return GetSalvageCost(DatabaseAPI.Database.Salvage[salvageIdx], c);
        }

        public static string GetCurrencyName(Enums.RewardCurrency c, int amount = 1)
        {
            var plural = amount > 1 ? "s" : "";
            return c switch
            {
                Enums.RewardCurrency.RewardMerit => $"Reward Merit{plural}",
                Enums.RewardCurrency.AstralMerit => $"Astral Merit{plural}",
                Enums.RewardCurrency.EmpyreanMerit => $"Empyrean Merit{plural}",
                Enums.RewardCurrency.AlignmentMerit => MidsContext.Character.Alignment switch
                {
                    Enums.Alignment.Villain => $"Villain Merit{plural}",
                    Enums.Alignment.Rogue => $"Villain Merit{plural}",
                    Enums.Alignment.Loyalist => $"Villain Merit{plural}",
                    _ => $"Hero Merit{plural}"
                },
                Enums.RewardCurrency.VanguardMerit => $"Vanguard Merit{plural}",
                Enums.RewardCurrency.AETicket => $"AE Ticket{plural}",
                Enums.RewardCurrency.Influence => "Influence",
                _ => ""
            };
        }

        public static Recipe.RecipeRarity GetCurrencyRarity(Enums.RewardCurrency c)
        {
            return c switch
            {
                Enums.RewardCurrency.RewardMerit => Recipe.RecipeRarity.Rare,
                Enums.RewardCurrency.AstralMerit => Recipe.RecipeRarity.UltraRare,
                Enums.RewardCurrency.EmpyreanMerit => Recipe.RecipeRarity.UltraRare,
                Enums.RewardCurrency.AlignmentMerit => Recipe.RecipeRarity.UltraRare,
                Enums.RewardCurrency.VanguardMerit => Recipe.RecipeRarity.Rare,
                _ => Recipe.RecipeRarity.Common,
            };
        }

        public static Color GetCurrencyRarityColor(Enums.RewardCurrency c)
        {
            return c switch
            {
                Enums.RewardCurrency.RewardMerit => PopUp.Colors.Rare,
                Enums.RewardCurrency.AstralMerit => PopUp.Colors.UltraRare,
                Enums.RewardCurrency.EmpyreanMerit => PopUp.Colors.UltraRare,
                Enums.RewardCurrency.AlignmentMerit => PopUp.Colors.UltraRare,
                Enums.RewardCurrency.VanguardMerit => PopUp.Colors.Rare,
                _ => PopUp.Colors.Common,
            };
        }

        public static int? GetSalvageCost(Salvage s, Enums.RewardCurrency c = Enums.RewardCurrency.RewardMerit, int amount = 1)
        {
            if (amount == 0) return 0;

            return s.ExternalName switch
            {
                "Reward Merit" => CurrencyChange(Enums.RewardCurrency.RewardMerit, c, amount),
                "Enhancement Catalyst" => CurrencyChange(Enums.RewardCurrency.RewardMerit, c, 20 * amount),
                "Enhancement Booster" => CurrencyChange(Enums.RewardCurrency.RewardMerit, c, 5 * amount),
                _ => c switch
                {
                    Enums.RewardCurrency.AETicket => s.Rarity switch
                    {
                        Recipe.RecipeRarity.Uncommon => 80 * amount,
                        Recipe.RecipeRarity.Rare => 540 * amount,
                        _ => null // Commons are within a reward roll for 8 tickets. Ignored.
                    },
                    Enums.RewardCurrency.Influence => s.Rarity switch
                    {
                        // Estimated AH prices here.
                        // Don't take it for granted.
                        Recipe.RecipeRarity.Common => 500 * amount,
                        Recipe.RecipeRarity.Uncommon => 1500 * amount,
                        Recipe.RecipeRarity.Rare => 500000 * amount,
                        _ => null
                    },
                    _ => null
                }
            };
        }
    }
}