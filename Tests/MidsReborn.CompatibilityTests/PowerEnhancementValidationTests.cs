using System;
using System.Linq;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Xunit;

namespace MidsReborn.CompatibilityTests;

public sealed class PowerEnhancementValidationTests : IClassFixture<PopupFormattingFixture>
{
    private readonly PopupFormattingFixture _fixture;

    public PowerEnhancementValidationTests(PopupFormattingFixture fixture)
    {
        _fixture = fixture;
        _fixture.ResetContext();
    }

    [Fact]
    public void GetValidEnhancements_IgnoresInvalidEnhancementClassIndexes()
    {
        var power = DatabaseAPI.Database.Power
            .First(p => p is not null && p.GetValidEnhancements(Enums.eType.Normal).Count > 0);

        var enhancementId = power.GetValidEnhancements(Enums.eType.Normal)
            .First(id => DatabaseAPI.Database.Enhancements[id].ClassID.Length > 0);

        var enhancement = DatabaseAPI.Database.Enhancements[enhancementId];
        var originalClassIds = enhancement.ClassID.ToArray();

        try
        {
            enhancement.ClassID = [DatabaseAPI.Database.EnhancementClasses.Length + 5];

            var exception = Record.Exception(() => power.GetValidEnhancements(Enums.eType.Normal));

            Assert.Null(exception);
        }
        finally
        {
            enhancement.ClassID = originalClassIds;
        }
    }

    [Fact]
    public void CraftedDecimationAccuracyDamage_ImportsSingleLogicalDamageEnhancement()
    {
        var enhancementId = _fixture.GetEnhancementId("Crafted_Decimation_A");
        var enhancement = DatabaseAPI.Database.Enhancements[enhancementId];

        var damageEffects = enhancement.Effect
            .Where(effect => effect.Mode == Enums.eEffMode.Enhancement &&
                             effect.Enhance.ID == (int)Enums.eEnhance.Damage)
            .ToArray();
        var accuracyEffects = enhancement.Effect
            .Where(effect => effect.Mode == Enums.eEffMode.Enhancement &&
                             effect.Enhance.ID == (int)Enums.eEnhance.Accuracy)
            .ToArray();

        Assert.Single(damageEffects);
        Assert.Single(accuracyEffects);
    }

    [Fact]
    public void CraftedDecimationAccuracyDamage_HasMatchingRuntimeScheduleValues()
    {
        var slot = _fixture.CreateSlot("Crafted_Decimation_A", ioLevel: 39);

        var accuracy = slot.GetEnhancementEffect(Enums.eEnhance.Accuracy, -1, 1f);
        var damage = slot.GetEnhancementEffect(Enums.eEnhance.Damage, -1, 1f);

        Assert.True(accuracy > 0f);
        Assert.Equal(accuracy, damage, 4);
    }
}
