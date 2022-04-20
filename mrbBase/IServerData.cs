namespace mrbBase
{
    public interface IServerData
    {
        string ManifestUri { get; set; }
        float BaseToHit { get; set; }
        float BaseFlySpeed { get; set; }
        float BaseJumpSpeed { get; set; }
        float BaseJumpHeight { get; set; }
        float BaseRunSpeed { get; set; }
        float BasePerception { get; set; }
        float MaxFlySpeed { get; set; }
        float MaxJumpSpeed { get; set; }
        float MaxRunSpeed { get; set; }
        int MaxSlots { get; set; }
        bool EnableInherentSlotting { get; set; }
        int HealthSlots { get; set; }
        int HealthSlot1Level { get; set; }
        int HealthSlot2Level { get; set; }
        int StaminaSlots { get; set; }
        int StaminaSlot1Level { get; set; }
        int StaminaSlot2Level { get; set; }
    }
}