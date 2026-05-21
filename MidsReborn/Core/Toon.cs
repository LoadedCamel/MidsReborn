using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using FastDeepCloner;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Display;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;
using Mids_Reborn.Core.PlannerRulesets;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.UI.Controls;
using Mids_Reborn.UI.Controls.Skia;

namespace Mids_Reborn.Core
{
    public class Toon : Character
    {
        private const double BuildFormatChange1 = 1.29999995231628;
        private const double BuildFormatChange2 = 1.39999997615814;
        private IPower?[] _basePowers = Array.Empty<IPower?>();
        private IPower?[] _assembledBasePowers = Array.Empty<IPower?>();
        private IPower?[] _buffedPowers = Array.Empty<IPower>();
        private IPower?[] _mathPowers = Array.Empty<IPower>();
        private IPower?[] _preBuffPowers = Array.Empty<IPower>();
        private Enums.BuffsX _selfBuffs;
        private Enums.BuffsX _selfEnhance;
        internal BuildCalculationSnapshot? LastCalculationSnapshot { get; private set; }

        private void ApplyPvpDr()
        {
            DatabaseAPI.GetPlannerRuleset().ApplyPvpDiminishingReturns(Totals);
        }

        private static PopUp.StringValue BuildEDItem(
            int index,
            float[] value,
            Enums.eSchedule[] schedule,
            string edName,
            float[] afterED,
            Enums.eEnhance enhanceType = Enums.eEnhance.None,
            int subType = -1,
            Enums.eBuffDebuff buffMode = Enums.eBuffDebuff.Any)
        {
            var stringValue1 = new PopUp.StringValue();
            var edInfo = Enhancement.GetDiversificationInfo(schedule[index], value[index], enhanceType, subType, buffMode);
            var flag1 = value[index] > edInfo.FirstThreshold;
            var flag2 = value[index] > edInfo.SecondThreshold;
            var flag3 = value[index] > edInfo.ThirdThreshold;
            PopUp.StringValue stringValue2;
            if (value[index] > 0)
            {
                var color = new Color();
                var str1 = edName + ":";
                var num1 = value[index] * 100f;
                var num2 = edInfo.AppliedValue * 100f;
                var str2 = $"{Convert.ToDecimal(num1 + afterED[index] * 100f):0.##}%";
                var str3 = $"{Convert.ToDecimal(num2 + afterED[index] * 100f):0.##}%";
                string str4;
                if (Math.Round(num1 - num2, 3) > 0)
                {
                    str4 = str3 + "  (Pre-ED: " + str2 + ")";
                    if (flag3)
                        color = Color.FromArgb(byte.MaxValue, 0, 0);
                    else if (flag2)
                        color = Color.FromArgb(byte.MaxValue, byte.MaxValue, 0);
                    else if (flag1)
                        color = Color.FromArgb(0, byte.MaxValue, 0);
                }
                else
                {
                    str4 = str3;
                    color = PopUp.Colors.Title;
                }

                stringValue1.Text = str1;
                stringValue1.TextColumn = str4;
                stringValue1.Color = PopUp.Colors.Title;
                stringValue1.ColorColumn = color;
                stringValue1.Size = 0.9f;
                stringValue1.Indent = 1;
                stringValue1.Format = FontStyle.Bold;
                stringValue2 = stringValue1;
            }
            else
            {
                stringValue1.Text = "";
                stringValue1.Color = Color.White;
                stringValue1.Format = FontStyle.Bold;
                stringValue1.Indent = 0;
                stringValue1.ColorColumn = Color.White;
                stringValue1.TextColumn = "";
                stringValue2 = stringValue1;
            }

            return stringValue2;
        }

        public IPowerset? PickDefaultSecondaryPowerset()
        {
            return ResolveSelectedSecondaryPowersetOrDefault();
        }

        public void BuildPower(int iSet, int powerID, bool noPoolShuffle = false)
        {
            BuildPower(iSet, powerID, -1, noPoolShuffle);
        }

        public void BuildPower(int iSet, int powerID, int preferredHistoryIndex, bool noPoolShuffle = false)
        {
            if (iSet < 0 || powerID < 0)
            {
                return;
            }

            var inToonHistory = CurrentBuild.FindInToonHistory(powerID);
            ResetLevel();
            var message = "";
            if (inToonHistory > -1)
            {
                if (CanRemovePower(inToonHistory, true, out message))
                {
                    if (inToonHistory < CurrentBuild.Powers.Count)
                    {
                        CurrentBuild.Powers[inToonHistory].Reset();
                    }

                    RequestedLevel = CurrentBuild.Powers[inToonHistory].Level;
                }
                else if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show(message, @"Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                ResetLevel();
                Lock();
            }
            else
            {
                var i = -1;
                switch (MidsContext.Config.BuildMode)
                {
                    case Enums.dmModes.LevelUp:
                        {
                            i = GetFirstAvailablePowerIndex(DatabaseAPI.Database.Power[powerID].Level - 1);
                            switch (i)
                            {
                                case < 0:
                                    message = "You cannot place any additional powers unless you first remove one.";
                                    break;
                                default:
                                    {
                                        if (CurrentBuild.Powers[i].Level <= Level)
                                        {
                                            if (!TestPower(powerID))
                                                i = -1;
                                        }
                                        else
                                        {
                                            i = -1;
                                        }

                                        break;
                                    }
                            }
                            break;
                        }
                    case Enums.dmModes.Normal:
                    case Enums.dmModes.Respec:
                        {
                            int minimumLevel = Math.Max(RequestedLevel, DatabaseAPI.Database.Power[powerID].Level - 1);
                            i = GetPreferredBuildPowerIndex(preferredHistoryIndex, minimumLevel);
                            if (i < 0)
                            {
                                i = GetFirstAvailablePowerIndex(minimumLevel);
                            }
                            break;
                        }
                }

                var flag2 = false;
                switch (i)
                {
                    case 0:
                        if (DatabaseAPI.Database.Powersets[iSet].SetType == Enums.ePowerSetType.Primary)
                        {
                            if (DatabaseAPI.Database.Power[powerID].Level == 1)
                            {
                                flag2 = true;
                                break;
                            }

                            message = "You must place a level 1 Primary power here.";
                            break;
                        }

                        if (DatabaseAPI.Database.Powersets[iSet].SetType == Enums.ePowerSetType.Secondary)
                        {
                            if (CurrentBuild.Powers[1].NIDPowerset < 0)
                            {
                                i = 1;
                                flag2 = true;
                            }
                            else
                            {
                                message = "You must place a level 1 Primary power here.";
                            }
                        }

                        break;
                    case 1:
                        if (DatabaseAPI.Database.Powersets[iSet].SetType == Enums.ePowerSetType.Secondary)
                        {
                            if (DatabaseAPI.Database.Power[powerID].Level == 1)
                            {
                                flag2 = true;
                                break;
                            }

                            message = "You must place a level 1 Secondary power here.";
                            break;
                        }

                        if (DatabaseAPI.Database.Powersets[iSet].SetType == Enums.ePowerSetType.Primary)
                        {
                            if (CurrentBuild.Powers[0].NIDPowerset < 0)
                            {
                                i = 0;
                                flag2 = true;
                            }
                            else
                            {
                                message = "You must place a level 1 Secondary power here.";
                            }
                        }

                        break;
                    default:
                        flag2 = i > 1;
                        break;
                }

                if (flag2)
                {
                    SetPower_NID(i, powerID);
                    Lock();

                    if (MidsContext.Character.CurrentBuild.Powers[i]?.Power is { VariableEnabled: true })
                    {
                        var initialVariableValue = Math.Max(MidsContext.Character.CurrentBuild.Powers[i].Power.VariableMin, MidsContext.Character.CurrentBuild.Powers[i].Power.VariableStart);
                        MidsContext.Character.CurrentBuild.Powers[i].VariableValue = initialVariableValue;
                    }
                }
                else if (!string.IsNullOrEmpty(message))
                {
                    MessageBox.Show(message, @"Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            Validate();
            if (!noPoolShuffle)
            {
                PoolShuffle();
            }

            ResetLevel();
        }

        private int GetPreferredBuildPowerIndex(int preferredHistoryIndex, int minimumLevel)
        {
            if (preferredHistoryIndex < 0 || preferredHistoryIndex >= CurrentBuild.Powers.Count)
            {
                return -1;
            }

            var targetPower = CurrentBuild.Powers[preferredHistoryIndex];
            if (targetPower == null || targetPower.Chosen || targetPower.NIDPowerset > -1)
            {
                return -1;
            }

            return targetPower.Level >= minimumLevel ? preferredHistoryIndex : -1;
        }

        public int BuildSlot(int powerIDX, int slotIDX = -1)
        {
            int num1;
            if ((powerIDX < 0) | (powerIDX >= CurrentBuild.Powers.Count))
            {
                num1 = -1;
            }
            else
            {
                var num2 = -1;
                if (slotIDX > -1)
                {
                    if (CurrentBuild.Powers[powerIDX].CanRemoveSlot(slotIDX, out var message))
                    {
                        CurrentBuild.RemoveSlotFromPower(powerIDX, slotIDX);
                        if (!CurrentBuild.Powers[powerIDX].Chosen & (CurrentBuild.Powers[powerIDX].Slots.Length == 0))
                        {
                            CurrentBuild.Powers[powerIDX].Level = -1;
                        }

                        ResetLevel();
                        Lock();
                    }
                    else if (!string.IsNullOrWhiteSpace(message) & !MidsContext.EnhCheckMode)
                    {
                        MessageBox.Show(message, @"FYI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var iLevel = SlotCheck(CurrentBuild.Powers[powerIDX]);
                    if (iLevel > -1)
                        num2 = CurrentBuild.Powers[powerIDX].AddSlot(iLevel);
                }

                ResetLevel();
                Validate();
                num1 = num2;
            }

            return num1;
        }

        public static string FixSpelling(string iString)
        {
            iString = iString?.Replace("Armour", "Armor");
            iString = iString?.Replace("Electric Mastery", "Electrical Mastery");
            return iString;
        }

        public void FlipAllSlots()
        {
            var num = CurrentBuild.Powers.Count - 1;
            for (var iPowerSlot = 0; iPowerSlot <= num; ++iPowerSlot)
                FlipSlots(iPowerSlot);
            GenerateBuffedPowerArray();
        }

        public void FlipSlots(int iPowerSlot)
        {
            if (iPowerSlot < 0)
                return;
            var num = CurrentBuild.Powers[iPowerSlot].SlotCount - 1;
            for (var index = 0; index <= num; ++index)
                CurrentBuild.Powers[iPowerSlot].Slots[index].Flip();
        }

        public void GenerateBuffedPowerArray(PlannerBuildRecipientContext? recipient = null)
        {
            CurrentBuild.GenerateSetBonusData();
            if (recipient == null)
            {
                LastCalculationSnapshot = null;
            }

            if (recipient == null)
            {
                ModifyEffects = new Dictionary<string, float>();
            }

            var pipeline = new PlannerPowerPipeline(CurrentBuild, Archetype, recipient);
            pipeline.ExecuteAssemblyPhase();
            ApplyPipelineResult(pipeline.Result);
            if (recipient == null)
            {
                ModifyEffects = pipeline.Result.ActorAssembly?.ChanceModifierCatalog
                                ?? new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
            }

            pipeline.ExecuteEnhancementBucketPhase();
            pipeline.ExecutePerPowerEnhancementMathPhase();
            pipeline.ExecuteSelfBuffBucketPhase();
            pipeline.ExecutePostBuffMultiplyPhase();
            if (recipient == null)
            {
                pipeline.ExecuteFinalizationPhase();
            }

            ApplyPipelineResult(pipeline.Result);

            if (recipient == null)
            {
                ModifyEffects = pipeline.Result.CalculationSnapshot?.PlayerActorSnapshot.ChanceModifierCatalog
                                ?? ModifyEffects
                                ?? new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
                LastCalculationSnapshot = pipeline.Result.CalculationSnapshot;
                ContributionTracker.UseSnapshot(LastCalculationSnapshot?.PlayerActorSnapshot.Contributions);
            }
        }

        private void ApplyPipelineResult(PlannerPowerPipelineResult result)
        {
            _basePowers = result.BasePowers;
            _assembledBasePowers = result.AssembledBasePowers;
            _mathPowers = result.MathPowers;
            _preBuffPowers = result.PreBuffPowers;
            _buffedPowers = result.BuffedPowers;
            _selfEnhance = result.SelfEnhanceBuckets;
            _selfBuffs = result.SelfBuffBuckets;
            if (MidsContext.Config != null && result.CombatContext != null)
            {
                MidsContext.Config.ScalingToHit = result.CombatContext.ToHitScale;
            }

            if (result.FinalTotalsSnapshot != null)
            {
                Totals.Assign(result.FinalTotalsSnapshot.Totals);
                TotalsCapped.Assign(result.FinalTotalsSnapshot.TotalsCapped);
            }
        }

        private static void RemoveExactDuplicateAbsorbedEffects(ref IPower? power)
        {
            if (power == null || power.Effects.Length <= 1)
            {
                return;
            }

            var filtered = new List<IEffect>(power.Effects.Length);
            var absorbedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var effect in power.Effects)
            {
                if (!effect.Absorbed_Effect)
                {
                    filtered.Add(effect);
                    continue;
                }

                var key = GetExactAbsorbedEffectKey(effect);
                if (!absorbedKeys.Add(key))
                {
                    continue;
                }

                filtered.Add(effect);
            }

            if (filtered.Count == power.Effects.Length)
            {
                return;
            }

            power.Effects = filtered.ToArray();
            foreach (var effect in power.Effects)
            {
                effect.SetPower(power);
            }
        }

        private static string GetExactAbsorbedEffectKey(IEffect effect)
        {
            static string NormalizeConditionSet(AdvancedConditionSet? conditions)
            {
                if (conditions == null || conditions.Rows.Count == 0)
                {
                    return string.Empty;
                }

                return string.Join(";",
                    conditions.Rows
                        .Select(row => $"{row.EvaluationMode}|{row.Kind}|{row.Link}|{row.Negated}|{AdvancedConditionCompiler.Compile(row)}")
                        .OrderBy(value => value, StringComparer.OrdinalIgnoreCase));
            }

            var tags = string.Join(",",
                effect.EffectTags
                    .Where(tag => !string.IsNullOrWhiteSpace(tag))
                    .OrderBy(tag => tag, StringComparer.OrdinalIgnoreCase));

            var recurrence = effect.PseudoPetRecurrence is { IsValid: true } info
                ? $"{info.EntityName}|{info.PetPowerName}|{info.SourceUsageTime:0.####}|{info.SourceActivatePeriod:0.####}|{info.EntCreateDuration:0.####}|{info.PetTickInterval:0.####}|{info.SpawnCount}|{info.TicksPerSpawn}|{info.TotalExpectedTicks}"
                : string.Empty;

            return string.Join("|",
                effect.EffectType,
                effect.ToWho,
                effect.PvMode,
                effect.AttribType,
                effect.Aspect,
                effect.DamageType,
                effect.MezType,
                effect.ModifierTable ?? string.Empty,
                effect.Scale.ToString("0.####", CultureInfo.InvariantCulture),
                effect.nMagnitude.ToString("0.####", CultureInfo.InvariantCulture),
                effect.Math_Mag.ToString("0.####", CultureInfo.InvariantCulture),
                effect.nDuration.ToString("0.####", CultureInfo.InvariantCulture),
                effect.Math_Duration.ToString("0.####", CultureInfo.InvariantCulture),
                effect.BaseProbability.ToString("0.####", CultureInfo.InvariantCulture),
                effect.ProcsPerMinute.ToString("0.####", CultureInfo.InvariantCulture),
                effect.DelayedTime.ToString("0.####", CultureInfo.InvariantCulture),
                effect.Ticks.ToString("0.####", CultureInfo.InvariantCulture),
                effect.Absorbed_Interval.ToString("0.####", CultureInfo.InvariantCulture),
                effect.EffectClass,
                effect.Stacking,
                effect.Absorbed_EffectID,
                effect.Absorbed_Power_nID,
                effect.Absorbed_Class_nID,
                effect.OmniSource ?? string.Empty,
                effect.EffectId ?? string.Empty,
                tags,
                recurrence,
                NormalizeConditionSet(effect.AdvancedConditions),
                effect.Summon ?? string.Empty,
                effect.Override ?? string.Empty,
                effect.Reward ?? string.Empty,
                effect.ModeName ?? string.Empty);
        }

        /// <summary>
        /// Generate MathPower, BuffedPower for the listed powers.
        /// These are going to be assimilated into build power # basePowerHistoryIdx in order to apply buff calculations
        /// This is typically used for pet powers that normally do not receive buffs from build.
        /// </summary>
        /// <param name="powers">List of powers to process</param>
        /// <param name="basePowerHistoryIdx">Index in build history of the power to attach to</param>
        /// <returns>KeyValuePair(keys=MathPower, values=BuffedPower</returns>
        public KeyValuePair<List<IPower>, List<IPower>>? GenerateBuffedPowers(List<IPower> powers, int basePowerHistoryIdx)
        {
            return GenerateBuffedPowersInternal(powers, basePowerHistoryIdx, null);
        }

        public KeyValuePair<List<IPower>, List<IPower>>? GenerateBuffedPowers(List<IPower> powers, int basePowerHistoryIdx, PlannerBuildRecipientContext recipient)
        {
            return GenerateBuffedPowersInternal(powers, basePowerHistoryIdx, recipient);
        }

        private KeyValuePair<List<IPower>, List<IPower>>? GenerateBuffedPowersInternal(List<IPower> powers, int basePowerHistoryIdx, PlannerBuildRecipientContext? recipient)
        {
            if (basePowerHistoryIdx < 0)
            {
                // If root power is not picked in build, get unbuffed powers data from the db directly.
                var powersList = powers
                    .Select(e => DatabaseAPI.Database.Power.FirstOrDefault(f => f?.StaticIndex == e.StaticIndex) ?? new Power { StaticIndex = -1 })
                    .Cast<IPower>()
                    .ToList();
                if (recipient != null)
                {
                    foreach (var power in powersList.OfType<Power>())
                    {
                        power.OmniDisplayClassName = recipient.ClassName;
                    }
                }

                var clonedPowersList = powersList.Clone();
                return new KeyValuePair<List<IPower>, List<IPower>>(clonedPowersList, clonedPowersList);
            }

            var mathPowers = new List<IPower>();
            var buffedPowers = new List<IPower>();
            var basePower = CurrentBuild.Powers[basePowerHistoryIdx].Power?.Clone();
            if (basePower == null)
            {
                return null;
            }

            var workingPowers = powers
                .Where(power => power != null)
                .Select(power => power.Clone())
                .ToList();
            workingPowers.Add(basePower);
            var originalNidPower = CurrentBuild.Powers[basePowerHistoryIdx].NIDPower;

            try
            {
                for (var i = 0; i < workingPowers.Count; i++)
                {
                    var workingPower = workingPowers[i];
                    if (workingPower == null)
                    {
                        continue;
                    }

                    CurrentBuild.Powers[basePowerHistoryIdx].NIDPower =
                        DatabaseAPI.Database.Power.TryFindIndex(e => e?.StaticIndex == workingPower.StaticIndex);
                    GenerateBuffedPowerArray(recipient);

                    if (i < workingPowers.Count - 1 &&
                        _mathPowers[basePowerHistoryIdx] != null &&
                        _buffedPowers[basePowerHistoryIdx] != null)
                    {
                        mathPowers.Add(_mathPowers[basePowerHistoryIdx].Clone());
                        buffedPowers.Add(_buffedPowers[basePowerHistoryIdx].Clone());
                    }
                }
            }
            finally
            {
                CurrentBuild.Powers[basePowerHistoryIdx].NIDPower = originalNidPower;
                GenerateBuffedPowerArray();
            }

            return new KeyValuePair<List<IPower>, List<IPower>>(mathPowers, buffedPowers);
        }

        public IReadOnlyList<RealPetActorRosterItem> GetRealPetActorRoster()
        {
            return CurrentBuild.GetRealPetActorRoster();
        }

        public KeyValuePair<List<IPower>, List<IPower>>? GenerateBuffedPetPowers(
            IReadOnlyList<ResolvedPetPower> powers,
            PlannerBuildRecipientContext recipient,
            PetActorPreviewState? previewState = null)
        {
            if (powers.Count == 0)
            {
                return new KeyValuePair<List<IPower>, List<IPower>>(new List<IPower>(), new List<IPower>());
            }

            var mathPowers = Enumerable.Repeat<IPower?>(null, powers.Count).ToArray();
            var buffedPowers = Enumerable.Repeat<IPower?>(null, powers.Count).ToArray();

            foreach (var group in powers
                         .Select((power, index) => new { Power = power, Index = index })
                         .GroupBy(entry => entry.Power.SourceHistoryIndex))
            {
                var generated = GenerateBuffedPowers(
                    group.Select(entry => entry.Power.Power).ToList(),
                    group.Key,
                    recipient);
                if (generated == null)
                {
                    return null;
                }

                var groupEntries = group.ToArray();
                for (var index = 0; index < groupEntries.Length; index++)
                {
                    mathPowers[groupEntries[index].Index] = generated.Value.Key[index];
                    buffedPowers[groupEntries[index].Index] = generated.Value.Value[index];
                }
            }

            var generatedMathPowers = mathPowers.Select(power => power ?? new Power()).ToList();
            var generatedBuffedPowers = buffedPowers.Select(power => power ?? new Power()).ToList();

            return new KeyValuePair<List<IPower>, List<IPower>>(
                generatedMathPowers,
                generatedBuffedPowers);
        }

        public PetActorSnapshot? GeneratePetActorSnapshot(RealPetActorRosterItem item, PetActorPreviewState? previewState = null)
        {
            return GeneratePetActorSnapshotCore(item, previewState, includeAppliedBonuses: true);
        }

        internal PetActorSnapshot? GeneratePetActorSnapshotForAnalysis(RealPetActorRosterItem item, PetActorPreviewState? previewState = null)
        {
            return GeneratePetActorSnapshotCore(item, previewState, includeAppliedBonuses: false);
        }

        private PetActorSnapshot? GeneratePetActorSnapshotCore(RealPetActorRosterItem item, PetActorPreviewState? previewState, bool includeAppliedBonuses)
        {
            var entity = DatabaseAPI.Database.Entities
                .FirstOrDefault(candidate => candidate?.UID.Equals(item.EntityUid, StringComparison.OrdinalIgnoreCase) == true);
            if (entity == null || !entity.IsRealPet)
            {
                return null;
            }

            var recipient = PlannerBuildRecipientContext.CreateOwnedPetRecipient(new SummonedEntity(entity), item.SourceHistoryIndex);
            var (resolvedPowers, availableUpgrades, availableSelfClickBuffs, effectivePreviewState) =
                PetActorPowerResolver.Resolve(CurrentBuild, new SummonedEntity(entity), item, previewState);

            var generatedPowers = GenerateBuffedPetPowers(resolvedPowers, recipient, effectivePreviewState);
            if (generatedPowers == null)
            {
                return null;
            }

            var basePowers = resolvedPowers
                .Select(resolvedPower => resolvedPower.Power)
                .Select(power =>
                {
                    var clone = power.Clone();
                    if (clone is Power concretePower)
                    {
                        concretePower.OmniDisplayClassName = recipient.ClassName;
                    }

                    return clone;
                })
                .ToList();

            var includedIndexes = resolvedPowers
                .Select((resolvedPower, index) => new { resolvedPower, index })
                .Where(entry => PetActorPowerResolver.ShouldIncludeInTotals(entry.resolvedPower, effectivePreviewState))
                .Select(entry => entry.index)
                .ToArray();

            var includedMathPowers = includedIndexes
                .Select(index => resolvedPowers[index].IsSelfClickBuff
                    ? generatedPowers.Value.Value[index]
                    : generatedPowers.Value.Key[index])
                .ToArray();
            var includedBuffedPowers = includedIndexes
                .Select(index => generatedPowers.Value.Value[index])
                .ToArray();
            var includedSelfClickBuffIndexes = includedIndexes
                .Where(index => resolvedPowers[index].IsSelfClickBuff)
                .ToArray();
            var supplementalEnhancementSourcePowers = includedSelfClickBuffIndexes
                .Select(index => generatedPowers.Value.Value[index])
                .ToArray();

            var setBonusPower = CurrentBuild.GetSetBonusVirtualPower(recipient);
            var aggregation = PetActorMath.Finalize(
                recipient.ClassName,
                generatedPowers.Value.Key,
                generatedPowers.Value.Value,
                setBonusPower == null ? Array.Empty<IPower>() : new[] { setBonusPower },
                includedMathPowers,
                includedBuffedPowers,
                supplementalEnhancementSourcePowers,
                includedSelfClickBuffIndexes);
            var totals = aggregation.Totals!;

            var appliedBonuses = includeAppliedBonuses
                ? PetActorBonusAnalyzer.Build(
                    this,
                    item,
                    recipient,
                    totals,
                    resolvedPowers,
                    availableUpgrades,
                    effectivePreviewState,
                    generatedPowers.Value.Key,
                    generatedPowers.Value.Value)
                : Array.Empty<PetAppliedBonusEntry>();
            var powerSnapshots = CalculationSnapshotFactory.CreatePowerSnapshots(
                basePowers.Cast<IPower?>().ToArray(),
                basePowers.Cast<IPower?>().ToArray(),
                generatedPowers.Value.Key.Cast<IPower?>().ToArray(),
                generatedPowers.Value.Key.Cast<IPower?>().ToArray(),
                generatedPowers.Value.Value.Cast<IPower?>().ToArray());

            return new PetActorSnapshot
            {
                RosterItem = item,
                Entity = new SummonedEntity(entity),
                Recipient = recipient,
                ActorTags = recipient.Tags.ToArray(),
                PreviewState = effectivePreviewState,
                AvailableUpgrades = availableUpgrades,
                AvailableSelfClickBuffs = availableSelfClickBuffs,
                ResolvedPowers = resolvedPowers,
                BasePowers = basePowers,
                MathPowers = generatedPowers.Value.Key,
                BuffedPowers = generatedPowers.Value.Value,
                Totals = totals,
                AppliedBonusEntries = appliedBonuses,
                CalculationSnapshot = CalculationSnapshotFactory.CreateActorSnapshot(
                    CalculationActorKind.Pet,
                    recipient.ClassName,
                    DatabaseAPI.GetArchetypeByClassName(recipient.ClassName),
                    generatedPowers.Value.Key,
                    generatedPowers.Value.Value,
                    aggregation.FinalSelfEnhanceBuckets,
                    aggregation.FinalSelfBuffBuckets,
                    totals,
                    null,
                    aggregation.Contributions,
                    powerSnapshots,
                    aggregation.Aggregation)
            };
        }

        public IPower? GetBasePower(int iPower, int nIDPower = -1)
        {
            if (iPower > -1)
            {
                if (CurrentBuild.Powers[iPower] != null)
                {
                    if (CurrentBuild.Powers.Count - 1 < iPower || CurrentBuild.Powers[iPower].NIDPower < 0)
                    {
                        return null;
                    }

                    nIDPower = CurrentBuild.Powers[iPower].NIDPower;
                    if (_basePowers.Length > iPower && _basePowers[iPower] != null && _basePowers[iPower].PowerIndex == nIDPower)
                    {
                        return _basePowers[iPower];
                    }

                    GenerateBuffedPowerArray();
                    if (_basePowers.Length > iPower && _basePowers[iPower] != null && _basePowers[iPower].PowerIndex == nIDPower)
                    {
                        return _basePowers[iPower];
                    }

                    return null;
                }
            }
            else if (nIDPower <= -1 || nIDPower > DatabaseAPI.Database.Power.Length - 1)
            {
                return null;
            }

            return new Power(DatabaseAPI.Database.Power[nIDPower]);
        }

        private static int GetClassByName(string iName)
        {
            foreach (var enhCls in DatabaseAPI.Database.EnhancementClasses)
            {
                if (string.Equals(enhCls.ShortName, iName, StringComparison.OrdinalIgnoreCase))
                    return enhCls.ID;
                if (!string.Equals(enhCls.Name, iName, StringComparison.OrdinalIgnoreCase))
                    continue;
                return enhCls.ID;
            }

            return -1;
        }

        public IPower? GetEnhancedPower(int iPower)
        {
            if ((iPower < 0) | (_buffedPowers.Length - 1 < iPower))
            {
                return null;
            }

            if (_buffedPowers[iPower] == null && CurrentBuild.Powers.Count > iPower && CurrentBuild.Powers[iPower]?.NIDPower > -1)
            {
                GenerateBuffedPowerArray();
            }

            return _buffedPowers[iPower];
        }

        public IPower? GetEnhancedPower(IPower? power)
        {
            if (power == null)
            {
                return null;
            }

            return _buffedPowers
                .DefaultIfEmpty(null)
                .FirstOrDefault(e => e != null && e.FullName == power.FullName);
        }

        internal IPower? TryGetAssembledBasePower(int historyIndex)
        {
            if (historyIndex < 0)
            {
                return null;
            }

            if (historyIndex < _assembledBasePowers.Length && _assembledBasePowers[historyIndex] != null)
            {
                return new Power(_assembledBasePowers[historyIndex]!);
            }

            if (LastCalculationSnapshot?.PowerSnapshots.Count > historyIndex &&
                LastCalculationSnapshot.PowerSnapshots[historyIndex].AssembledBasePower != null)
            {
                return new Power(LastCalculationSnapshot.PowerSnapshots[historyIndex].AssembledBasePower!);
            }

            return null;
        }

        public PowerDisplaySnapshot GetDisplayPowerSnapshot(int iPower, int nIDPower = -1)
        {
            IPower? basePower = null;
            IPower? enhancedPower = null;

            if (iPower > -1 &&
                iPower < CurrentBuild.Powers.Count &&
                CurrentBuild.Powers[iPower] != null &&
                CurrentBuild.Powers[iPower].NIDPower > -1)
            {
                basePower = GetBasePower(iPower);
                enhancedPower = GetEnhancedPower(iPower);
                if (basePower == null)
                {
                    basePower = GetBasePower(iPower, nIDPower);
                }
            }
            else if (nIDPower > -1)
            {
                basePower = GetBasePower(iPower, nIDPower);
            }

            return CreateDisplayPowerSnapshot(basePower, enhancedPower, iPower);
        }

        private PowerDisplaySnapshot CreateDisplayPowerSnapshot(IPower? basePower, IPower? enhancedPower, int historyIndex)
        {
            var displayBase = basePower == null ? null : new Power(basePower);
            var enhancedSource = enhancedPower;
            var baseWasResolvedForDisplay = false;
            var enhancedWasResolvedForDisplay = false;

            if (historyIndex >= 0)
            {
                if ((_assembledBasePowers.Length <= historyIndex || _assembledBasePowers[historyIndex] == null) &&
                    historyIndex < CurrentBuild.Powers.Count &&
                    CurrentBuild.Powers[historyIndex]?.NIDPower > -1)
                {
                    GenerateBuffedPowerArray();
                }

                if (historyIndex < _assembledBasePowers.Length && _assembledBasePowers[historyIndex] != null)
                {
                    displayBase = new Power(_assembledBasePowers[historyIndex]!);
                    baseWasResolvedForDisplay = true;
                }
            }
            else if (basePower != null)
            {
                var previewResolvedPower = AssembleDisplayPreviewPower(basePower.PowerIndex, historyIndex);
                if (previewResolvedPower != null)
                {
                    displayBase = new Power(previewResolvedPower);
                    enhancedSource = new Power(previewResolvedPower);
                    baseWasResolvedForDisplay = true;
                    enhancedWasResolvedForDisplay = true;
                }
            }

            if (enhancedSource == null && displayBase != null)
            {
                enhancedSource = new Power(displayBase)
                {
                    PowerIndex = -1
                };
            }

            var displayEnhanced = enhancedSource == null ? null : new Power(enhancedSource);
            var rootPowerName = displayBase == null
                ? string.Empty
                : Power.GetRootPowerName(historyIndex, displayBase, displayEnhanced);
            var rootPowerBase = string.IsNullOrEmpty(rootPowerName)
                ? null
                : DatabaseAPI.GetPowerByFullName(rootPowerName);
            var rootPowerEnh = historyIndex > -1 ? GetEnhancedPower(historyIndex) : null;

            return new PowerDisplaySnapshot(
                displayBase,
                displayEnhanced,
                rootPowerBase,
                rootPowerEnh,
                historyIndex,
                baseWasResolvedForDisplay,
                enhancedWasResolvedForDisplay,
                baseWasPaddedOrRepaired: false,
                enhancedWasPaddedOrRepaired: false)
            {
                ActorCalculationSnapshot = LastCalculationSnapshot?.PlayerActorSnapshot,
                PowerCalculationSnapshot = historyIndex >= 0 &&
                                           LastCalculationSnapshot?.PowerSnapshots.Count > historyIndex
                    ? LastCalculationSnapshot.PowerSnapshots[historyIndex]
                    : null,
                ContributionSnapshot = LastCalculationSnapshot?.PlayerActorSnapshot.Contributions
            };
        }

        private IPower? AssembleDisplayPreviewPower(int nIDPower, int historyIndex)
        {
            if (nIDPower < 0)
            {
                return null;
            }

            var pipeline = new PlannerPowerPipeline(CurrentBuild, Archetype);
            return pipeline.AssemblePowerEntry(nIDPower, historyIndex);
        }

        public int[] GetEnhancements(int iPowerSlot)
        {
            if (!(iPowerSlot < 0 || iPowerSlot >= CurrentBuild.Powers.Count) &&
                CurrentBuild.Powers[iPowerSlot].SlotCount > 0)
                return CurrentBuild.Powers[iPowerSlot].Slots.Select(slot => slot.Enhancement.Enh).ToArray();

            return Array.Empty<int>();
        }

        private bool ImportInternalDataUC(StreamReader iStream, float nVer)
        {
            float olderFile;
            if (nVer < 1.0)
            {
                olderFile = nVer;
                if (olderFile >= 0.899999976158142 && olderFile < 1.0)
                    olderFile = 0.0f;
                MessageBox.Show(@"The data being loaded was saved by either an older version of the application, or a different rendition of the application and may not load correctly.", @"FYI", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                olderFile = 0.0f;
            }

            // create method before any other variables are declared, inside other method to reduce class clutter
            // also to lower cognitive load, this method depends on local properties, not the huge amount of variables present in the containing/calling method
            // only 1 variable closed over: olderFile
            ReadOnlyCollection<int> readPowerEntries(string[] data)
            {
                var count = Convert.ToInt32(data[0]);
                var offset = 1;
                var indexLookup = new int[count + 1];
                for (var index2 = 0; index2 <= count; ++index2)
                {
                    var tPower = new PowerEntry
                    {
                        Level = Convert.ToInt32(data[offset])
                    };
                    offset += 1;
                    tPower.NIDPowerset = Math.Abs(olderFile - 0.100000001490116) > float.Epsilon
                        ? Convert.ToInt32(data[offset]) >= 0.0
                            ? Powersets[Convert.ToInt32(data[offset])].nID
                            : -1
                        : Convert.ToInt32(data[offset]);
                    offset += 1;
                    tPower.IDXPower = Convert.ToInt32(data[offset]);
                    tPower.NIDPower = !((tPower.NIDPowerset > -1) & (tPower.IDXPower > -1))
                        ? -1
                        : DatabaseAPI.Database.Powersets[tPower.NIDPowerset].Power[tPower.IDXPower];
                    if (Math.Abs(olderFile) < float.Epsilon || olderFile >= 1.0)
                    {
                        offset += +1;
                        tPower.StatInclude = Math.Abs(Convert.ToInt32(data[offset])) > float.Epsilon;
                        offset = offset + 1 + 1;
                    }

                    offset += 1;
                    indexLookup[index2] = tPower.NIDPower;
                    if (tPower.PowerSet.SetType == Enums.ePowerSetType.Inherent ||
                        IsRequiredSecondaryStarterPower(tPower.NIDPower))
                        continue;
                    RequestedLevel = tPower.Level;
                    BuildPower(tPower.NIDPowerset, tPower.NIDPower, true);
                }

                return new ReadOnlyCollection<int>(indexLookup.ToList());
            }

            void readSlotEntries(string[] data, ReadOnlyCollection<int> idxLookup)
            {
                var offset = 1; // previously line 4665
                for (var index2 = 0; index2 <= Convert.ToInt32(data[0]); ++index2)
                {
                    var slotEntry = new SlotEntry
                    {
                        Level = Convert.ToInt32(data[offset])
                    };
                    offset += 2;
                    var tPowerID = Convert.ToInt32(data[offset]);
                    offset += 1;
                    var tSlotIdx = Convert.ToInt32(data[offset]);
                    offset += 1;
                    if (olderFile > 0.0 && olderFile < 1.0)
                        offset += 2;
                    if (olderFile > 0.0 && olderFile < 1.10000002384186)
                    {
                        slotEntry.Enhancement = new I9Slot
                        {
                            Enh = DatabaseAPI.GetFirstValidEnhancement(GetClassByName(data[offset])),
                            Grade = MidsContext.Config.CalcEnhOrigin,
                            RelativeLevel = MidsContext.Config.CalcEnhLevel,
                            IOLevel = MidsContext.Config.I9.DefaultIOLevel
                        };
                        slotEntry.FlippedEnhancement = new I9Slot();
                    }
                    else
                    {
                        slotEntry.LoadFromString(data[offset], ":");
                    }

                    offset += 1;
                    if (tPowerID <= -1 || idxLookup[tPowerID] <= -1)
                        continue;
                    var nSlotID = 0;
                    var inToonHistory = CurrentBuild.FindInToonHistory(idxLookup[tPowerID]); // AKA nPowerID
                    if (tSlotIdx > 0)
                        nSlotID = BuildSlot(inToonHistory);
                    else if (tSlotIdx == 0)
                        nSlotID = tSlotIdx;
                    if (!((inToonHistory > -1) & (inToonHistory < CurrentBuild.Powers.Count)) ||
                        !((CurrentBuild.Powers[inToonHistory].Slots.Length > nSlotID) & (nSlotID > -1)))
                        continue;
                    var slot = CurrentBuild.Powers[inToonHistory].Slots[nSlotID];
                    slot.Enhancement.Enh = slotEntry.Enhancement.Enh;
                    slot.Enhancement.Grade = slotEntry.Enhancement.Grade;
                    slot.Enhancement.IOLevel = slotEntry.Enhancement.IOLevel;
                    slot.Enhancement.RelativeLevel = slotEntry.Enhancement.RelativeLevel;
                    // is this a bug from the original source? why wouldn't it be slot.Level = slotEntry.Level?
                    // commented out setter that does nothing
                    //slot.Level = slot.Level;
                    slot.FlippedEnhancement.Enh = slotEntry.FlippedEnhancement.Enh;
                    slot.FlippedEnhancement.Grade = slotEntry.FlippedEnhancement.Grade;
                    slot.FlippedEnhancement.IOLevel = slotEntry.FlippedEnhancement.IOLevel;
                    slot.FlippedEnhancement.RelativeLevel = slotEntry.FlippedEnhancement.RelativeLevel;
                }
            }

            // why are these two being saved then set back later, is something mutating them?
            var buildMode = MidsContext.Config.BuildMode;
            var buildOption = MidsContext.Config.BuildOption;
            MidsContext.Config.BuildMode = Enums.dmModes.Normal;
            MidsContext.Config.BuildOption = Enums.dmItem.Slot;
            var ret = IoGrab2(iStream);
            Name = ret[0];
            Archetype = DatabaseAPI.GetArchetypeByName(ret[2]);
            Origin = DatabaseAPI.GetOriginByName(Archetype, ret[1]);

            {
                // stage: powersets
                var setData = IoGrab2(iStream);
                for (var index = 0; index <= Powersets.Length - 1; ++index)
                {
                    Powersets[index] = DatabaseAPI.GetPowersetByName(setData[index], Archetype.DisplayName);
                    if (setData[index].IndexOf("Inherent", StringComparison.Ordinal) > -1)
                        Powersets[index] = DatabaseAPI.GetInherentPowerset();
                }
            }

            {
                // stage: poolLocks
                var poolData = IoGrab2(iStream);
                for (var index = 0; index <= PoolLocked.Length - 1; ++index)
                    PoolLocked[index] = Math.Abs(Convert.ToInt32(poolData[index + 1])) > float.Epsilon;
            }

            // fast forward stream
            IoGrab2(iStream);
            // fast forward stream
            IoGrab2(iStream);
            NewBuild(); // NewHistory()
            // stage: read powerEntries
            var lookup = readPowerEntries(IoGrab2(iStream));
            // stage: read slots
            readSlotEntries(IoGrab2(iStream), lookup);
            // stage: wrap up
            if (Archetype == null)
                Archetype = DatabaseAPI.Database.Classes[0];
            if (Origin > Archetype.Origin.Length - 1)
                Origin = Archetype.Origin.Length - 1;
            Lock();
            PoolShuffle();
            Validate();
            MidsContext.Config.BuildMode = buildMode;
            MidsContext.Config.BuildOption = buildOption;
            MidsContext.Archetype = Archetype;
            return true;
        }

        private static string[]? IoGrab2(StreamReader iStream, string delimiter = ";", char fakeLf = '\0')
        {
            var str = FileIO.ReadLineUnlimited(iStream, fakeLf);
            var strArray = str.Split(Convert.ToChar(delimiter));
            if (strArray.Length < 2)
                strArray = str.Split(';');
            for (var index = 0; index <= strArray.Length - 1; ++index)
                strArray[index] = FileIO.IOStrip(strArray[index]);
            return strArray;
        }

        public bool Load(string iFileName, ref Stream? mStream)
        {
            var buildString = "";

            // From file
            if (mStream == null || !string.IsNullOrEmpty(iFileName))
            {
                mStream = new FileStream(iFileName, FileMode.Open, FileAccess.Read);
                buildString = File.ReadAllText(iFileName);
            }
            // From clipboard (old datachunk)
            else if (Clipboard.ContainsText())
            {
                buildString = Clipboard.GetText();
                if (!Regex.IsMatch(buildString, @"\|MxD[uz];[0-9]+\;[0-9]+;[0-9]+;[A-Z]+;\|"))
                {
                    MessageBox.Show("Clipboard doesn't contain a valid datachunk.", "Whoops", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return false;
                }

                // This ignores previously set mStream
                mStream = new MemoryStream();
                var streamWriter = new StreamWriter(mStream);
                streamWriter.Write(buildString);
                streamWriter.Flush();
                mStream.Position = 0;
            }
            else
            {
                MessageBox.Show("Cannot load build: no data to load from.", "Whoops", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return false;
            }

            //Stream iStream1 = mStream != null ? mStream : (Stream) new FileStream(iFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            switch (MidsCharacterFileFormat.MxDExtractAndLoad(mStream))
            {
                case MidsCharacterFileFormat.eLoadReturnCode.Failure:
                    mStream.Close();

                    return false;
                case MidsCharacterFileFormat.eLoadReturnCode.Success:
                    mStream.Close();
                    ResetLevel();
                    PoolShuffle();
                    AssetManager.OriginIndex = Origin;
                    Validate();
                    ReadMetadata(buildString);

                    return true;
                case MidsCharacterFileFormat.eLoadReturnCode.IsOldFormat:
                    mStream.Close();
                    break;
            }

            StreamReader iStream2;
            try
            {
                iStream2 = new StreamReader(iFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            var flag = ReadInternalData(iStream2);
            iStream2.Close();
            ResetLevel();
            PoolShuffle();
            AssetManager.OriginIndex = Origin;
            Validate();
            ReadMetadata(buildString);

            return flag;
        }

        public PopUp.PopupData PopPowerInfo(int hIDX, int pIDX, bool includePowerKindLabel = false)
        {
            var popupData = new PopUp.PopupData();
            if (pIDX < 0)
            {
                if (hIDX < 0 || CurrentBuild.Powers[hIDX].NIDPower < 0)
                {
                    return popupData;
                }

                pIDX = CurrentBuild.Powers[hIDX].NIDPower;
            }

            var power = DatabaseAPI.Database.Power[pIDX];
            var index1 = popupData.Add();
            popupData.Sections[index1].Add(power.DisplayName, PopUp.Colors.Title, 1.25f);
            if (power.PowerSetID > -1)
            {
                popupData.Sections[index1].Add($"Powerset: {DatabaseAPI.Database.Powersets[power.PowerSetID].DisplayName}", PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
            }

            if (hIDX > -1)
            {
                if (CurrentBuild.Powers[hIDX].Chosen)
                {
                    popupData.Sections[index1].Add($"Available: Level {power.Level}", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
                    popupData.Sections[index1].Add($"Placed: Level {CurrentBuild.Powers[hIDX].Level + 1}", PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
                }
                else
                {
                    popupData.Sections[index1].Add($"Inherent: Level {CurrentBuild.Powers[hIDX].Level + 1}", PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
                }
            }
            else
            {
                popupData.Sections[index1].Add($"Available: Level {power.Level}", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            popupData.Sections[index1].Add(BuildPopupDescription(power, includePowerKindLabel), PopUp.Colors.Text);
            var flag1 = false;
            if (hIDX < 0 & pIDX > -1)
            {
                if (DatabaseAPI.Database.Power[pIDX].NIDSubPower.Length > 0)
                {
                    index1 = popupData.Add();
                    popupData.Sections[index1] = new PopUp.Section();
                    popupData.Sections[index1].Add("Powers:", PopUp.Colors.Title);
                    if (pIDX > -1)
                    {
                        foreach (var subPowerNid in DatabaseAPI.Database.Power[pIDX].NIDSubPower)
                        {
                            if (subPowerNid > -1)
                            {
                                popupData.Sections[index1].Add(DatabaseAPI.Database.Power[subPowerNid].DisplayName, PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
                            }
                        }
                    }
                }

                if (!DatabaseAPI.Database.Power[pIDX].Requires.ClassOk(Archetype.Idx))
                {
                    index1 = popupData.Add();
                    popupData.Sections[index1].Add($"You cannot take this power because you are a {Archetype.DisplayName}.", PopUp.Colors.Alert, 1f, FontStyle.Bold, 1);
                }
            }

            var hasEnhEffect = false;
            if (hIDX <= -1)
            {
                return popupData;
            }

            if (CurrentBuild.Powers[hIDX].NIDPower > -1)
            {
                if (DatabaseAPI.Database.Power[CurrentBuild.Powers[hIDX].NIDPower].Effects.Length > 0)
                {
                    if (DatabaseAPI.Database.Power[CurrentBuild.Powers[hIDX].NIDPower].NIDSubPower.Length > 0)
                    {
                        index1 = popupData.Add();
                        popupData.Sections[index1] = CurrentBuild.Powers[hIDX].PopSubPowerListing("Powers:", PopUp.Colors.Text, PopUp.Colors.Text);
                    }
                }
                else if (DatabaseAPI.Database.Power[CurrentBuild.Powers[hIDX].NIDPower].NIDSubPower.Length > 0)
                {
                    index1 = popupData.Add();
                    popupData.Sections[index1] = CurrentBuild.Powers[hIDX].PopSubPowerListing("Powers:", PopUp.Colors.Disabled, PopUp.Colors.Effect);
                }
            }

            if (CurrentBuild.Powers[hIDX].Slots.Length > 0)
            {
                for (var slotIdx = 0; slotIdx < CurrentBuild.Powers[hIDX].Slots.Length; slotIdx++)
                {
                    if (CurrentBuild.Powers[hIDX].Slots[slotIdx].Enhancement.Enh > -1 && DatabaseAPI.Database.Enhancements[CurrentBuild.Powers[hIDX].Slots[slotIdx].Enhancement.Enh].HasEnhEffect)
                    {
                        hasEnhEffect = true;
                    }
                }

                if (hasEnhEffect)
                {
                    index1 = popupData.Add();
                    popupData.Sections[index1] = PopSlottedEnhInfo(hIDX);
                }
            }

            foreach (var sb in CurrentBuild.SetBonuses)
            {
                if (sb.PowerIndex != hIDX)
                {
                    continue;
                }

                for (var senInfoIdx = 0; senInfoIdx < sb.SetInfo.Length; senInfoIdx++)
                {
                    if (!flag1)
                    {
                        flag1 = true;
                        index1 = popupData.Add();
                        popupData.Sections[index1].Add("Active Enhancement Sets:", PopUp.Colors.Text);
                    }

                    var setInfo = sb.SetInfo;
                    var enhancementSet = DatabaseAPI.Database.EnhancementSets[sb.SetInfo[senInfoIdx].SetIDX];
                    popupData.Sections[index1].Add($"{enhancementSet.DisplayName} ({setInfo[senInfoIdx].SlottedCount}/{DatabaseAPI.GetVisibleSetPieceCount(sb.SetInfo[senInfoIdx].SetIDX)})", PopUp.Colors.Title);
                    for (var bonusIdx = 0; bonusIdx < enhancementSet.Bonus.Length; bonusIdx++)
                    {
                        if (!(setInfo[senInfoIdx].SlottedCount >= enhancementSet.Bonus[bonusIdx].Slotted &
                              enhancementSet.BonusAppliesInContext(bonusIdx, false, MidsContext.Config.Inc.DisablePvE)))
                        {
                            continue;
                        }

                        var enhString = enhancementSet.GetEffectString(bonusIdx, false, true, true, true);
                        if (!string.IsNullOrWhiteSpace(enhString))
                        {
                            popupData.Sections[index1].Add(enhString.Replace(", ", "\n"), PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
                        }
                    }

                    foreach (var pieceIndex in (sb.SetInfo[senInfoIdx].PieceIndexes ?? Array.Empty<int>()).Where(index => index >= 0).Distinct())
                    {
                        var isSpecial = DatabaseAPI.GetSpecialRawMemberPositionForSetPiece(sb.SetInfo[senInfoIdx].SetIDX, pieceIndex);
                        if (isSpecial > -1)
                        {
                            popupData.Sections[index1].Add(enhancementSet.GetEffectString(isSpecial, true, true, true), PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
                        }
                    }
                }
            }

            if (DatabaseAPI.Database.Power[CurrentBuild.Powers[hIDX].NIDPower].UIDSubPower.Length > 0)
            {
                var index2 = popupData.Add();
                popupData.Sections[index2].Add("This virtual power contains additional powers which can be individually selected.\r\nTo change which powers are selected, either Control+Shift+Click or Double-Click on this power.\r\n\r\nRemember that the selected powers will only be active if this power's toggle button is switched on.", PopUp.Colors.Title, 0.9f, FontStyle.Bold, 1);
            }

            var empty = string.Empty;
            if (PowerState(CurrentBuild.Powers[hIDX].NIDPower, ref empty) != MidsItemState.Invalid || empty == "")
            {
                return popupData;
            }

            var index4 = popupData.Add();
            popupData.Sections[index4].Add(empty, PopUp.Colors.Alert);
            if (DatabaseAPI.Database.Power[CurrentBuild.Powers[hIDX].NIDPower].Requires.ClassOk(Archetype.Idx))
            {
                return popupData;
            }

            var index5 = popupData.Add();
            popupData.Sections[index5].Add($"You cannot take this power because you are a {Archetype.DisplayName}.", PopUp.Colors.Alert, 1f, FontStyle.Bold, 1);

            return popupData;
        }

        internal static string BuildPopupDescription(IPower power, bool includePowerKindLabel)
        {
            var description = power.DescShort?.Trim() ?? string.Empty;

            if (!includePowerKindLabel)
            {
                return description;
            }

            var typeLabel = GetPopupPowerKindLabel(power);
            if (TryNormalizeExistingPopupDescription(description, typeLabel, out var normalizedDescription))
            {
                return normalizedDescription;
            }

            if (string.IsNullOrWhiteSpace(typeLabel))
            {
                return description;
            }

            return string.IsNullOrWhiteSpace(description)
                ? typeLabel
                : $"{typeLabel}: {description}";
        }

        private static string GetPopupPowerKindLabel(IPower power)
        {
            return power.PowerType switch
            {
                Enums.ePowerType.Click when power.ClickBuff => "Click-Buff",
                Enums.ePowerType.Auto_ => "Auto",
                Enums.ePowerType.Toggle => "Toggle",
                Enums.ePowerType.Click => "Click",
                _ => string.Empty
            };
        }

        private static bool TryNormalizeExistingPopupDescription(
            string description,
            string computedTypeLabel,
            out string normalizedDescription)
        {
            normalizedDescription = description;
            if (string.IsNullOrWhiteSpace(description))
            {
                return false;
            }

            foreach (var knownLabel in new[] { "Click-Buff", "Toggle", "Auto", "Click" })
            {
                var canonicalPrefix = $"{knownLabel}:";
                if (description.StartsWith(canonicalPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    normalizedDescription = $"{knownLabel}: {description[canonicalPrefix.Length..].TrimStart()}";
                    return true;
                }
            }

            if (description.StartsWith("(Toggle)", StringComparison.OrdinalIgnoreCase))
            {
                normalizedDescription = NormalizeLegacyParentheticalPrefix("Toggle", description, "(Toggle)");
                return true;
            }

            if (description.StartsWith("(Auto)", StringComparison.OrdinalIgnoreCase))
            {
                normalizedDescription = NormalizeLegacyParentheticalPrefix("Auto", description, "(Auto)");
                return true;
            }

            if (description.StartsWith("(Click)", StringComparison.OrdinalIgnoreCase))
            {
                normalizedDescription = NormalizeLegacyParentheticalPrefix(
                    string.IsNullOrWhiteSpace(computedTypeLabel) ? "Click" : computedTypeLabel,
                    description,
                    "(Click)");
                return true;
            }

            return false;
        }

        private static string NormalizeLegacyParentheticalPrefix(string label, string description, string legacyPrefix)
        {
            var remainder = description[legacyPrefix.Length..].TrimStart();
            return string.IsNullOrWhiteSpace(remainder)
                ? label
                : $"{label}: {remainder}";
        }

        public PopUp.PopupData PopPowersetInfo(int nIDPowerset, string extraString = "")
        {
            var popupData = new PopUp.PopupData();
            var powerset = DatabaseAPI.Database.Powersets[nIDPowerset];
            var index1 = popupData.Add();
            popupData.Sections[index1].Add(powerset.DisplayName, PopUp.Colors.Title, 1.25f);
            popupData.Sections[index1].Add(
                    powerset.nArchetype > -1
                        ? $"Archetype: {DatabaseAPI.Database.Classes[powerset.nArchetype].DisplayName}"
                        : "Archetype: All", PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);

            popupData.Sections[index1].Add($"Set Type: {Enum.GetName(powerset.SetType.GetType(), powerset.SetType)}", PopUp.Colors.Effect, 0.9f, FontStyle.Bold, 1);
            popupData.Sections[index1].Add(powerset.Description.Replace("  ", " "), PopUp.Colors.Text);
            if (extraString != "")
            {
                var index2 = popupData.Add();
                popupData.Sections[index2].Add(extraString, PopUp.Colors.Invention, 1f, FontStyle.Bold, 1);
            }

            if (powerset.Powers.Length <= 0)
            {
                return popupData;
            }

            if (!powerset.Powers[0].Requires.ClassOk(Archetype.Idx))
            {
                var index2 = popupData.Add();
                popupData.Sections[index2].Add($"You cannot take powers from this pool because you are a {Archetype.DisplayName}.", PopUp.Colors.Alert, 1f, FontStyle.Bold, 1);
            }
            else if (PowersetMutexClash(Powersets[0].Power[0]))
            {
                var index2 = popupData.Add();
                popupData.Sections[index2].Add($"You cannot take the {Powersets[0].DisplayName} and {Powersets[1].DisplayName} sets together.", PopUp.Colors.Alert);
            }

            return popupData;
        }

        private PopUp.Section? PopSlottedEnhInfo(int hIDX)
        {
            var section = new PopUp.Section();
            section.Add("Buff/Debuff", PopUp.Colors.Text, "Value", PopUp.Colors.Text);
            if (hIDX < 0)
            {
                return section;
            }

            var eEnhance = Enums.eEnhance.None;
            var eMez = Enums.eMez.None;
            var nBuff = new float[Enum.GetValues<Enums.eEnhance>().Length];
            var nDebuff = new float[Enum.GetValues<Enums.eEnhance>().Length];
            var nAny = new float[Enum.GetValues<Enums.eEnhance>().Length];
            var schedBuff = new Enums.eSchedule[Enum.GetValues<Enums.eEnhance>().Length];
            var schedDebuff = new Enums.eSchedule[Enum.GetValues<Enums.eEnhance>().Length];
            var schedAny = new Enums.eSchedule[Enum.GetValues<Enums.eEnhance>().Length];
            var afterED1 = new float[Enum.GetValues<Enums.eEnhance>().Length];
            var afterED2 = new float[Enum.GetValues<Enums.eEnhance>().Length];
            var afterED3 = new float[Enum.GetValues<Enums.eEnhance>().Length];
            var afterED4 = new float[Enum.GetValues<Enums.eEnhance>().Length];
            var nMez = new float[Enum.GetValues<Enums.eMez>().Length];
            var schedMez = new Enums.eSchedule[Enum.GetValues<Enums.eEnhance>().Length];
            for (var index = 0; index < nBuff.Length; index++)
            {
                nBuff[index] = 0;
                nDebuff[index] = 0;
                nAny[index] = 0;
                schedBuff[index] = Enhancement.GetSchedule((Enums.eEnhance)index);
                schedDebuff[index] = schedBuff[index];
                schedAny[index] = schedBuff[index];
            }

            schedDebuff[3] = Enums.eSchedule.A; // Enums.eEnhance.Defense
            for (var tSub = 0; tSub < nMez.Length; tSub++)
            {
                nMez[tSub] = 0;
                schedMez[tSub] = Enhancement.GetSchedule(Enums.eEnhance.Mez, tSub);
            }

            for (var index1 = 0; index1 < CurrentBuild.Powers[hIDX].SlotCount; index1++)
            {
                if (CurrentBuild.Powers[hIDX].Slots[index1].Enhancement.Enh <= -1)
                {
                    continue;
                }

                for (var index2 = 0; index2 < DatabaseAPI.Database.Enhancements[CurrentBuild.Powers[hIDX].Slots[index1].Enhancement.Enh].Effect.Length; index2++)
                {
                    var effect = DatabaseAPI.Database.Enhancements[CurrentBuild.Powers[hIDX].Slots[index1].Enhancement.Enh].Effect;
                    if (effect[index2].Mode != Enums.eEffMode.Enhancement)
                    {
                        continue;
                    }

                    if (effect[index2].Enhance.ID == 12) // Enums.eEnhance.Mez
                    {
                        nMez[effect[index2].Enhance.SubID] += CurrentBuild.Powers[hIDX].Slots[index1].Enhancement.GetEnhancementEffect(Enums.eEnhance.Mez, effect[index2].Enhance.SubID, 1);
                    }
                    else
                    {
                        switch (DatabaseAPI.Database.Enhancements[CurrentBuild.Powers[hIDX].Slots[index1].Enhancement.Enh].Effect[index2].BuffMode)
                        {
                            case Enums.eBuffDebuff.BuffOnly:
                                nBuff[effect[index2].Enhance.ID] += CurrentBuild.Powers[hIDX].Slots[index1].Enhancement.GetEnhancementEffect((Enums.eEnhance)effect[index2].Enhance.ID, -1, 1);
                                break;

                            case Enums.eBuffDebuff.DeBuffOnly:
                                if (effect[index2].Enhance.ID is not (6 or 11 or 19)) // Enums.eEnhance.SpeedFlying, Enums.eEnhance.SpeedJumping, Enums.eEnhance.SpeedRunning
                                {
                                    nDebuff[effect[index2].Enhance.ID] += CurrentBuild.Powers[hIDX].Slots[index1].Enhancement.GetEnhancementEffect((Enums.eEnhance)effect[index2].Enhance.ID, -1, -1f);
                                }
                                break;

                            default:
                                nAny[effect[index2].Enhance.ID] += CurrentBuild.Powers[hIDX].Slots[index1].Enhancement.GetEnhancementEffect((Enums.eEnhance)effect[index2].Enhance.ID, -1, 1f);
                                break;
                        }
                    }
                }
            }

            if (!MidsContext.Config.DisableAlphaPopup)
            {
                for (var index1 = 0; index1 < CurrentBuild.Powers.Count; index1++)
                {
                    if (CurrentBuild.Powers[index1] == null || CurrentBuild.Powers[index1].Power == null || !CurrentBuild.Powers[index1].StatInclude)
                    {
                        continue;
                    }

                    IPower power1 = PlannerEffectResolver.ResolvePower(new Power(CurrentBuild.Powers[index1]?.Power), new PlannerEffectResolutionContext
                    {
                        AbsorbPetEffects = true
                    }).ResolvedPower;
                    foreach (var effect in power1.Effects)
                    {
                        if (power1.PowerType != Enums.ePowerType.GlobalBoost & (!effect.Absorbed_Effect | effect.Absorbed_PowerType != Enums.ePowerType.GlobalBoost))
                        {
                            continue;
                        }

                        var power2 = effect.Absorbed_Effect & effect.Absorbed_Power_nID > -1
                            ? DatabaseAPI.Database.Power[effect.Absorbed_Power_nID]
                            : power1;

                        var eBuffDebuff = Enums.eBuffDebuff.Any;
                        var flag = false;
                        foreach (var str1 in CurrentBuild.Powers[hIDX]?.Power.BoostsAllowed)
                        {
                            if (power2 != null && power2.BoostsAllowed.Any(str2 => str1 == str2))
                            {
                                if (str1.Contains("Buff"))
                                {
                                    eBuffDebuff = Enums.eBuffDebuff.BuffOnly;
                                }

                                if (str1.Contains("Debuff"))
                                {
                                    eBuffDebuff = Enums.eBuffDebuff.DeBuffOnly;
                                }

                                flag = true;
                            }

                            if (flag)
                            {
                                break;
                            }
                        }

                        if (!flag)
                        {
                            continue;
                        }

                        if (effect.EffectType == Enums.eEffectType.Enhancement)
                        {
                            switch (effect.ETModifies)
                            {
                                case Enums.eEffectType.Defense:
                                    if (effect.DamageType == Enums.eDamage.Smashing)
                                    {
                                        if (effect.IgnoreED)
                                        {
                                            switch (eBuffDebuff)
                                            {
                                                case Enums.eBuffDebuff.BuffOnly:
                                                    afterED1[3] += effect.Mag;
                                                    break;
                                                case Enums.eBuffDebuff.DeBuffOnly:
                                                    afterED2[3] += effect.Mag;
                                                    break;
                                                default:
                                                    afterED3[3] += effect.Mag;
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            switch (eBuffDebuff)
                                            {
                                                case Enums.eBuffDebuff.BuffOnly:
                                                    nBuff[3] += effect.Mag;
                                                    break;
                                                case Enums.eBuffDebuff.DeBuffOnly:
                                                    nDebuff[3] += effect.Mag;
                                                    break;
                                                default:
                                                    nAny[3] += effect.Mag;
                                                    break;
                                            }
                                        }
                                    }

                                    break;
                                case Enums.eEffectType.Mez:
                                    if (effect.IgnoreED)
                                    {
                                        afterED4[(int)effect.MezType] += effect.Mag;
                                        break;
                                    }

                                    nMez[(int)effect.MezType] += effect.Mag;
                                    break;
                                default:
                                    var index3 = effect.ETModifies != Enums.eEffectType.RechargeTime ? Convert.ToInt32(Enum.Parse(typeof(Enums.eEnhance), effect.ETModifies.ToString())) : 14;
                                    if (effect.IgnoreED)
                                    {
                                        afterED3[index3] += effect.Mag;
                                        break;
                                    }

                                    nAny[index3] += effect.Mag;
                                    break;
                            }
                        }
                        else if (effect.EffectType == Enums.eEffectType.DamageBuff & effect.DamageType == Enums.eDamage.Smashing)
                        {
                            if (power2 == null)
                            {
                                continue;
                            }

                            if (effect.IgnoreED)
                            {
                                foreach (var str in power2.BoostsAllowed)
                                {
                                    if (str.StartsWith("Res_Damage"))
                                    {
                                        afterED3[18] += effect.Mag;
                                        break;
                                    }

                                    if (!str.StartsWith("Damage"))
                                    {
                                        continue;
                                    }

                                    afterED3[2] += effect.Mag;
                                    break;
                                }
                            }
                            else
                            {
                                foreach (var str in power2.BoostsAllowed)
                                {
                                    if (str.StartsWith("Res_Damage"))
                                    {
                                        nAny[18] += effect.Mag;
                                        break;
                                    }

                                    if (!str.StartsWith("Damage"))
                                    {
                                        continue;
                                    }

                                    nAny[2] += effect.Mag;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            nBuff[8] = 0; // Enums.eEnhance.HitPoints
            nBuff[16] = 0; // Enums.eEnhance.Recovery
            nBuff[17] = 0; // Enums.eEnhance.Regeneration

            nDebuff[8] = 0;
            nDebuff[16] = 0;
            nDebuff[17] = 0;

            nAny[8] = 0;
            nAny[16] = 0;
            nAny[17] = 0;

            var sContent = new List<PopUp.StringValue>();
            for (var index = 0; index < nBuff.Length; index++)
            {
                if (nBuff[index] > 0)
                {
                    sContent.Add(BuildEDItem(
                        index,
                        nBuff,
                        schedBuff,
                        Enum.GetName(eEnhance.GetType(), index) ?? "",
                        afterED1,
                        (Enums.eEnhance)index,
                        -1,
                        Enums.eBuffDebuff.BuffOnly));
                }

                if (nDebuff[index] > 0)
                {
                    var valueText = Enum.GetName(eEnhance.GetType(), index) ?? "";
                    sContent.Add(BuildEDItem(
                        index,
                        nDebuff,
                        schedDebuff,
                        $"{(valueText == "" ? "" : $"{valueText} Debuff")}",
                        afterED2,
                        (Enums.eEnhance)index,
                        -1,
                        Enums.eBuffDebuff.DeBuffOnly));
                }

                if (nAny[index] <= 0)
                {
                    continue;
                }

                sContent.Add(BuildEDItem(
                    index,
                    nAny,
                    schedAny,
                    Enum.GetName(eEnhance.GetType(), index) ?? "",
                    afterED3,
                    (Enums.eEnhance)index));
            }

            for (var index = 0; index < nMez.Length; index++)
            {
                if (nMez[index] <= 0)
                {
                    continue;
                }

                if (sContent.Count > 0)
                {
                    sContent.Add(BuildEDItem(
                        index,
                        nMez,
                        schedMez,
                        Enum.GetName(eMez.GetType(), index) ?? "",
                        afterED4,
                        Enums.eEnhance.Mez,
                        index));
                }
            }

            section.Content = sContent.ToArray();

            if (MidsContext.Config.DisableAlphaPopup)
            {
                section.Add("Enhancement values exclude Alpha ability (see Data View for full info, or change this option in the Configuration panel)", PopUp.Colors.Text, 0.8f, FontStyle.Regular, 1);
            }

            return section;
        }

        public MidsItemState PowerState(int nIDPower, ref string message)
        {
            if (nIDPower < 0)
            {
                return MidsItemState.Disabled;
            }

            var power = DatabaseAPI.Database.Power[nIDPower];
            var inToonHistory = CurrentBuild.FindInToonHistory(nIDPower);
            var foundInBuild = inToonHistory > -1;
            var num1 = MidsContext.Config.BuildMode switch
            {
                Enums.dmModes.Normal when RequestedLevel > -1 => RequestedLevel,
                Enums.dmModes.Respec when RequestedLevel > -1 => RequestedLevel,
                _ => Level
            };

            var nLevel = foundInBuild
                ? CurrentBuild.Powers[inToonHistory].Level
                : num1;

            message = "";
            var flag2 = CurrentBuild.MeetsRequirement(power, nLevel);
            if (PowersetMutexClash(nIDPower))
            {
                message = $"You cannot take the {Powersets[0].DisplayName} and {Powersets[1].DisplayName} sets together.";
                return MidsItemState.Heading;
            }

            if (!foundInBuild)
                return flag2 && num1 >= power?.Level - 1
                    ? MidsItemState.Enabled
                    : MidsItemState.Disabled;

            var num2 = 0;
            Enums.PowersetType powersetType;
            int[] numArray;
            do
            {
                Enums.ePowerSetType ePowerSetType;
                int index1;
                if (num2 == 0)
                {
                    ePowerSetType = Enums.ePowerSetType.Primary;
                    powersetType = Enums.PowersetType.Primary;
                    index1 = 0;
                }
                else
                {
                    ePowerSetType = Enums.ePowerSetType.Secondary;
                    powersetType = Enums.PowersetType.Secondary;
                    index1 = 1;
                }

                if (power?.GetPowerSet().SetType == ePowerSetType & power.Level - 1 == 0)
                {
                    numArray = DatabaseAPI.NidPowersAtLevelBranch(0, Powersets[(int)powersetType].nID);
                    var flag3 = false;
                    var num3 = 0;
                    foreach (var k in numArray)
                    {
                        if (CurrentBuild.Powers[index1].NIDPower == k)
                        {
                            flag3 = true;
                        }
                        else if (CurrentBuild.FindInToonHistory(k) > -1)
                        {
                            num3++;
                        }
                    }

                    if (CurrentBuild.Powers[index1].NIDPowerset > 0 & !flag3 | num3 == numArray.Length)
                    {
                        message = $"This power has been placed in a way that is not possible in-game. One of the {numArray.Length} level 1 powers from your {Enum.GetName(powersetType.GetType(), powersetType)} set must be taken at level 1.";

                        return MidsItemState.Invalid;
                    }
                }

                ++num2;
            } while (num2 <= 1);

            if (flag2)
            {
                return num1 <= power.Level - 1
                    ? MidsItemState.SelectedDisabled
                    : MidsItemState.Selected;
            }

            if (power?.GetPowerSet()?.SetType == Enums.ePowerSetType.Ancillary | power?.GetPowerSet()?.SetType == Enums.ePowerSetType.Pool)
            {
                message = "This power has been placed in a way that is not possible in-game.";
                message += power?.PowerSetIndex switch
                {
                    2 => "\r\nYou must take one of the first two powers in a pool before taking the third.",
                    3 => "\r\nYou must take two of the first three powers in a pool before taking the fourth.",
                    4 => "\r\nYou must take two of the first three powers in a pool before taking the fifth.",
                    _ => ""
                };
            }
            else
            {
                if (power?.InherentType != Enums.eGridType.None)
                {
                    return MidsItemState.Enabled;
                }

                message = "This power has been placed in a way that is not possible in-game.\r\nCheck that any powers that it requires have been taken first, and that if this is a branching powerset, the power does not conflict with another.";

                return MidsItemState.Invalid;
            }

            return MidsItemState.Invalid;
        }

        public EItemState SkPowerState(int nIDPower, ref string message)
        {
            if (nIDPower < 0)
            {
                return EItemState.Disabled;
            }

            var power = DatabaseAPI.Database.Power[nIDPower];
            var inToonHistory = CurrentBuild.FindInToonHistory(nIDPower);
            var foundInBuild = inToonHistory > -1;
            var num1 = MidsContext.Config.BuildMode switch
            {
                Enums.dmModes.Normal when RequestedLevel > -1 => RequestedLevel,
                Enums.dmModes.Respec when RequestedLevel > -1 => RequestedLevel,
                _ => Level
            };

            var nLevel = foundInBuild
                ? CurrentBuild.Powers[inToonHistory].Level
                : num1;

            message = "";
            var flag2 = CurrentBuild.MeetsRequirement(power, nLevel);
            if (PowersetMutexClash(nIDPower))
            {
                message = $"You cannot take the {Powersets[0].DisplayName} and {Powersets[1].DisplayName} sets together.";
                return EItemState.Heading;
            }

            if (!foundInBuild)
                return flag2 && num1 >= power?.Level - 1
                    ? EItemState.Enabled
                    : EItemState.Disabled;

            var num2 = 0;
            Enums.PowersetType powersetType;
            int[] numArray;
            do
            {
                Enums.ePowerSetType ePowerSetType;
                int index1;
                if (num2 == 0)
                {
                    ePowerSetType = Enums.ePowerSetType.Primary;
                    powersetType = Enums.PowersetType.Primary;
                    index1 = 0;
                }
                else
                {
                    ePowerSetType = Enums.ePowerSetType.Secondary;
                    powersetType = Enums.PowersetType.Secondary;
                    index1 = 1;
                }

                if (power?.GetPowerSet().SetType == ePowerSetType & power.Level - 1 == 0)
                {
                    numArray = DatabaseAPI.NidPowersAtLevelBranch(0, Powersets[(int)powersetType].nID);
                    var flag3 = false;
                    var num3 = 0;
                    foreach (var k in numArray)
                    {
                        if (CurrentBuild.Powers[index1].NIDPower == k)
                        {
                            flag3 = true;
                        }
                        else if (CurrentBuild.FindInToonHistory(k) > -1)
                        {
                            num3++;
                        }
                    }

                    if (CurrentBuild.Powers[index1].NIDPowerset > 0 & !flag3 | num3 == numArray.Length)
                    {
                        message = $"This power has been placed in a way that is not possible in-game. One of the {numArray.Length} level 1 powers from your {Enum.GetName(powersetType.GetType(), powersetType)} set must be taken at level 1.";

                        return EItemState.Invalid;
                    }
                }

                ++num2;
            } while (num2 <= 1);

            if (flag2)
            {
                return num1 <= power.Level - 1
                    ? EItemState.SelectedDisabled
                    : EItemState.Selected;
            }

            if (power?.GetPowerSet()?.SetType == Enums.ePowerSetType.Ancillary | power?.GetPowerSet()?.SetType == Enums.ePowerSetType.Pool)
            {
                message = "This power has been placed in a way that is not possible in-game.";
                message += power?.PowerSetIndex switch
                {
                    2 => "\r\nYou must take one of the first two powers in a pool before taking the third.",
                    3 => "\r\nYou must take two of the first three powers in a pool before taking the fourth.",
                    4 => "\r\nYou must take two of the first three powers in a pool before taking the fifth.",
                    _ => ""
                };
            }
            else
            {
                if (power?.InherentType != Enums.eGridType.None)
                {
                    return EItemState.Enabled;
                }

                message = "This power has been placed in a way that is not possible in-game.\r\nCheck that any powers that it requires have been taken first, and that if this is a branching powerset, the power does not conflict with another.";

                return EItemState.Invalid;
            }

            return EItemState.Invalid;
        }

        private bool ReadInternalData(StreamReader iStream)
        {
            iStream.BaseStream.Seek(0L, SeekOrigin.Begin);
            string[]? strArray;
            string a;
            try
            {
                do
                {
                    strArray = IoGrab2(iStream, ";", '|');
                    a = strArray != null
                        ? strArray.Length <= 0 ? "" : strArray[0]
                        : throw new Exception("Reached end of data without finding header.");
                } while (!(string.Equals(a, AppDataPaths.Headers.Save.Uncompressed, StringComparison.OrdinalIgnoreCase) | string.Equals(a, AppDataPaths.Headers.Save.Compressed, StringComparison.OrdinalIgnoreCase) || string.Equals(a, AppDataPaths.Headers.Save.LegacyUncompressed, StringComparison.OrdinalIgnoreCase) | string.Equals(a, AppDataPaths.Headers.Save.LegacyCompressed, StringComparison.OrdinalIgnoreCase)));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                return false;
            }

            if (string.Equals(a, AppDataPaths.Headers.Save.Uncompressed, StringComparison.OrdinalIgnoreCase) || string.Equals(a, AppDataPaths.Headers.Save.LegacyUncompressed, StringComparison.OrdinalIgnoreCase))
            {
                iStream.BaseStream.Seek(0L, SeekOrigin.Begin);
                return ReadInternalDataUC(iStream);
            }

            if (!string.Equals(a, AppDataPaths.Headers.Save.Compressed, StringComparison.OrdinalIgnoreCase) || !string.Equals(a, AppDataPaths.Headers.Save.LegacyCompressed, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var asciiEncoding = new ASCIIEncoding();
            var outSize = Convert.ToInt32(strArray[1]);
            var num1 = Convert.ToInt32(strArray[2]);
            var num2 = Convert.ToInt32(strArray[3]);
            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);
            //var iBytes = (byte[]) Utils.CopyArray(Zlib.UUDecodeBytes((byte[]) Utils.CopyArray(asciiEncoding.GetBytes(Zlib.UnbreakString(iStream.ReadToEnd())), new byte[num2])), new byte[num1]);
            var encoding = asciiEncoding.GetBytes(ModernZlib.UnbreakString(iStream.ReadToEnd()));
            var zlibDecode = ModernZlib.UuDecodeBytes(encoding);
            var dest1 = new byte[num2];
            Array.Copy(zlibDecode, dest1, zlibDecode.Length);
            var iBytes = new byte[num1];
            Array.Copy(dest1, iBytes, dest1.Length);
            iBytes = ModernZlib.DecompressChunk(iBytes, outSize);
            binaryWriter.Write(iBytes);
            memoryStream.Seek(0L, SeekOrigin.Begin);
            using var reader = new StreamReader(memoryStream);
            if (ReadInternalDataUC(reader))
            {
                binaryWriter.Close();
                memoryStream.Close();

                return true;
            }

            binaryWriter.Close();
            memoryStream.Close();

            return false;
        }

        private bool ReadInternalDataUC(StreamReader iStream)
        {
            string[] strArray1;
            do
            {
                strArray1 = IoGrab2(iStream, "|");
            } while (strArray1[0] != AppDataPaths.Headers.Save.Uncompressed || strArray1[0] != AppDataPaths.Headers.Save.LegacyUncompressed);

            strArray1[1] = strArray1[1].Replace(",", ".");
            var nVer = Convert.ToSingle(strArray1[1]);
            bool flag;
            if (nVer < BuildFormatChange1)
            {
                flag = ImportInternalDataUC(iStream, nVer);
            }
            else
            {
                if ((nVer < BuildFormatChange2) & (Math.Abs(nVer - BuildFormatChange1) > float.Epsilon))
                {
                    MessageBox.Show(@"The data being loaded was saved by either an older version or different rendition of the application, attempting conversion.", @"FYI", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (nVer > BuildFormatChange2)
                {
                    MessageBox.Show($"The data being loaded was saved by a newer version of the application (File format v {nVer.ToString("##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0###")} expected {1.4f.ToString("##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0###")} ", "Just FYI", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    /*MessageBox.Show(("The data being loaded was saved by a newer version of the application (File format v" +
                                     Strings.Format(nVer, "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0###") + ", expected " +
                                     Strings.Format(1.4f, "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "0###") +
                                     "). It may not load correctly."), @"Just FYI", MessageBoxButtons.OK, MessageBoxIcon.Information);*/
                }

                var strArray2 = IoGrab2(iStream, "|");
                Name = strArray2[0];
                Archetype = DatabaseAPI.GetArchetypeByName(strArray2[2]);
                Origin = DatabaseAPI.GetOriginByName(Archetype, strArray2[1]);
                var strArray3 = IoGrab2(iStream, "|");
                for (var index = 0; index <= Powersets.Length - 1; ++index)
                {
                    Powersets[index] =
                        DatabaseAPI.GetPowersetByName(FixSpelling(strArray3[index]), Archetype.DisplayName);
                    if (strArray3[index].IndexOf("Inherent", StringComparison.Ordinal) > -1)
                        Powersets[index] = DatabaseAPI.GetInherentPowerset();
                }

                var strArray4 = IoGrab2(iStream, "|");
                NewBuild();
                var index1 = 0;
                var tIDX = CurrentBuild.Powers.Count - (nVer < BuildFormatChange2 ? 2 : 1);
                for (var index2 = 0; index2 <= tIDX; ++index2)
                {
                    var power = CurrentBuild.Powers[index2];
                    if (index1 + 6 <= strArray4.Length)
                    {
                        power.StatInclude = Math.Abs(Convert.ToDouble(strArray4[index1])) > 0.01;
                        var index3 = index1 + 1 + 1;
                        power.Level = Convert.ToInt32(strArray4[index3]);
                        var index4 = index3 + 1;
                        power.NIDPowerset = Convert.ToDouble(strArray4[index4]) >= 0.0 ? Powersets[Convert.ToInt32(strArray4[index4])].nID : -1;
                        var index5 = index4 + 1;
                        power.IDXPower = Convert.ToInt32(strArray4[index5]);
                        power.NIDPower = !((power.NIDPowerset > -1) & (power.IDXPower > -1)) ? -1 : DatabaseAPI.Database.Powersets[power.NIDPowerset].Power[power.IDXPower];
                        var index6 = index5 + 1;
                        power.Slots = new SlotEntry[Convert.ToInt32(strArray4[index6]) + 1];
                        var index7 = index6 + 1;
                        var num6 = power.Slots.Length - 1;
                        for (var index8 = 0; index8 <= num6; ++index8)
                        {
                            power.Slots[index8].LoadFromString(strArray4[index7], "~");
                            var index9 = index7 + 1;
                            power.Slots[index8].Level = Convert.ToInt32(strArray4[index9]);
                            index7 = index9 + 1;
                        }

                        power.VariableValue = Convert.ToInt32(strArray4[index7]);
                        index1 = index7 + 1;
                        if (!(nVer >= BuildFormatChange2)) continue;
                        {
                            power.SubPowers = new PowerSubEntry[Convert.ToInt32(strArray4[index1]) + 1];
                            ++index1;
                            var num7 = power.SubPowers.Length - 1;
                            for (var index8 = 0; index8 <= num7; ++index8)
                            {
                                power.SubPowers[index8] = new PowerSubEntry
                                {
                                    Power = Convert.ToInt32(strArray4[index1])
                                };
                                index1++;
                                power.SubPowers[index8].Powerset = Convert.ToInt32(strArray4[index1]);
                                index1++;
                                power.SubPowers[index8].StatInclude = Math.Abs(Convert.ToDouble(strArray4[index1])) > 0.01;
                                index1++;
                                power.SubPowers[index8].nIDPower = !((power.SubPowers[index8].Powerset > -1) & (power.SubPowers[index8].Power > -1)) ? -1 : DatabaseAPI.Database.Powersets[power.SubPowers[index8].Powerset].Power[power.SubPowers[index8].Power];
                            }

                            power.SubPowers = Array.Empty<PowerSubEntry>();
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                MidsContext.Archetype = Archetype;
                Validate();
                Lock();
                flag = true;
            }

            return flag;
        }

        public bool Save(string iFileName)
        {
            Archetype ??= DatabaseAPI.Database.Classes[0];
            if (Origin > Archetype.Origin.Length - 1)
                Origin = Archetype.Origin.Length - 1;
            var saveText = MidsCharacterFileFormat.MxDBuildSaveString(true, false);
            if (string.IsNullOrWhiteSpace(saveText))
            {
                MessageBox.Show(@"Save failed - save function returned empty data.", @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            var str2 = new ClsOutput
            {
                Plain = true,
                idFormat = 0
            }.Build("") + "\r\n\r\n";
            StreamWriter streamWriter;
            try
            {
                streamWriter = new StreamWriter(iFileName, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            streamWriter.Write(str2);
            streamWriter.Write(saveText);
            streamWriter.Close();
            return true;
        }

        private void SetPower_NID(int index, int nIDPower)
        {
            if (index < 0 | index >= CurrentBuild.Powers.Count)
                return;
            var power1 = DatabaseAPI.Database.Power[nIDPower];
            if (power1 != null)
            {
                var power2 = CurrentBuild.Powers[index];
                power2.NIDPowerset = power1.PowerSetID;
                power2.IDXPower = power1.PowerSetIndex;
                power2.NIDPower = power1.PowerIndex;
                if (power1.NIDSubPower.Length > 0)
                {
                    power2.SubPowers = new PowerSubEntry[power1.NIDSubPower.Length];
                    for (var i = 0; i < power2.SubPowers.Length; i++)
                    {
                        if (power2.SubPowers[i] != null)
                            power2.SubPowers[i] = new PowerSubEntry
                            {
                                nIDPower = power1.NIDSubPower[i],
                                Powerset = DatabaseAPI.Database.Power[power2.SubPowers[i].nIDPower].PowerSetID,
                                Power = DatabaseAPI.Database.Power[power2.SubPowers[i].nIDPower].PowerSetIndex
                            };
                    }
                }

                if (power1.Slottable & power2.Slots.Length == 0)
                {
                    power2.AddSlot(power2.Level);
                }

                CurrentBuild.Powers[index].StatInclude = power1.PowerType is Enums.ePowerType.Toggle or Enums.ePowerType.Auto_ & power1.AlwaysToggle;
            }

            CurrentBuild.Powers[index].ValidateSlots();
        }

        public bool StringToInternalData(string iString)
        {
            bool flag1;
            if ((iString?.IndexOf(AppDataPaths.Headers.Save.Compressed, StringComparison.Ordinal) == -1) & (iString?.IndexOf(AppDataPaths.Headers.Save.Uncompressed, StringComparison.Ordinal) == -1) || (iString?.IndexOf(AppDataPaths.Headers.Save.LegacyCompressed, StringComparison.Ordinal) == -1) & (iString?.IndexOf(AppDataPaths.Headers.Save.LegacyUncompressed, StringComparison.Ordinal) == -1))
            {
                if ((iString.IndexOf("Primary", StringComparison.Ordinal) > -1) &
                    (iString.IndexOf("Secondary", StringComparison.Ordinal) > -1))
                {
                    if (clsUniversalImport.InterpretForumPost(iString))
                    {
                        MessageBox.Show(@"Form post interpreted OK!", @"Forum Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        flag1 = true;
                    }
                    else
                    {
                        MessageBox.Show(@"Unable to interpret data. Please check that you copied the build data from the forum correctly and that it's a valid format.", @"Forum Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        flag1 = false;
                    }
                }
                else
                {
                    MessageBox.Show(@"Unable to recognize data. Please check that you copied the build data from the forum correctly and that it's a valid format.", @"Forum Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    flag1 = false;
                }
            }
            else
            {
                StreamWriter streamWriter;
                try
                {
                    streamWriter = new StreamWriter(FileIO.AddSlash(Application.StartupPath) + "import.tmp", false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, @"Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                try
                {
                    if (iString != null && (iString.IndexOf(AppDataPaths.Headers.Save.Compressed, StringComparison.Ordinal) < 0 || iString.IndexOf(AppDataPaths.Headers.Save.LegacyCompressed, StringComparison.Ordinal) < 0))
                    {
                        iString = iString.Replace("+\r\n+", "");
                        iString = iString.Replace("+ \r\n+", "");
                    }

                    iString = iString
                        .Replace("[Email]", "")
                        .Replace("[/Email]", "")
                        .Replace("[email]", "")
                        .Replace("[/email]", "")
                        .Replace("[EMAIL]", "")
                        .Replace("[/EMAIL]", "")
                        .Replace("[URL]", "")
                        .Replace("[/URL]", "")
                        .Replace("[url]", "")
                        .Replace("[/url]", "")
                        .Replace("[Url]", "")
                        .Replace("[/Url]", "");
                    streamWriter.Write(iString);
                    streamWriter.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    streamWriter.Close();
                    return false;
                }

                Stream? mStream = null;
                if (Load(FileIO.AddSlash(Application.StartupPath) + "import.tmp", ref mStream))
                {
                    MessageBox.Show(@"Build data imported!", @"Forum Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    flag1 = true;
                }
                else
                {
                    MessageBox.Show(@"Build data couldn't be imported.  Please check that you copied the build data from the forum correctly.", @"Forum Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    flag1 = false;
                }
            }

            return flag1;
        }

        private bool TestPower(int nIDPower)
        {
            if (CurrentBuild.FindInToonHistory(nIDPower) > -1)
                return false;
            var message = "";
            return PowerState(nIDPower, ref message) == MidsItemState.Enabled;
        }
    }
}
