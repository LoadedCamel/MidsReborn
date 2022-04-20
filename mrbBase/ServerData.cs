using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrbBase
{
    public sealed class ServerData : IServerData
    {
        public static ServerData Instance { get; } = new();

        public string ManifestUri { get; set; } = "";
        public float BaseToHit { get; set; } = 0.75f;
        public float BaseFlySpeed { get; set; } = 31.5f;
        public float BaseJumpSpeed { get; set; } = 21f;
        public float BaseJumpHeight { get; set; } = 4f;
        public float BaseRunSpeed { get; set; } = 21f;
        public float BasePerception { get; set; } = 500f;
        public float MaxFlySpeed { get; set; } = 86f;
        public float MaxJumpSpeed { get; set; } = 114.40f;
        public float MaxRunSpeed { get; set; } = 135.67f;
        public int MaxSlots { get; set; } = 67;
        public bool EnableInherentSlotting { get; set; }
        public int HealthSlots { get; set; } = 2;
        public int HealthSlot1Level { get; set; } = 8;
        public int HealthSlot2Level { get; set; } = 16;
        public int StaminaSlots { get; set; } = 2;
        public int StaminaSlot1Level { get; set; } = 12;
        public int StaminaSlot2Level { get; set; } = 22;

    }
}
