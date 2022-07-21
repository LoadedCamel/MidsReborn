using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace mrbBase
{
    public static class Enums
    {
        public enum Alignment
        {
            Hero,
            Rogue,
            Vigilante,
            Villain,
            Loyalist,
            Resistance
        }

        public enum dmItem
        {
            None,
            Power,
            Slot
        }

        public enum dmModes
        {
            None,
            LevelUp,
            Normal,
            Respec
        }

        public enum eAspect
        {
            Res,
            Max,
            Abs,
            Str,
            Cur
        }

        public enum eAttribType
        {
            Magnitude,
            Duration,
            Expression
        }

        public enum eBuffDebuff
        {
            Any,
            BuffOnly,
            DeBuffOnly
        }

        public enum eBuffMode
        {
            Normal,
            Buff,
            Debuff
        }

        [Flags]
        public enum eCastFlags
        {
            None = 0,
            NearGround = 1,
            TargetNearGround = 2,
            CastableAfterDeath = 4
        }

        public enum eClassType
        {
            None,
            Hero,
            HeroEpic,
            Villain,
            VillainEpic,
            Henchman,
            Pet
        }

        public enum eInherentOrder
        {
            Class,
            Inherent,
            Powerset,
            Power,
            Prestige,
            Incarnate,
            Accolade,
            Pet,
            Temp

        }
        public enum eAlphaOrder
        {
            Boost,
            Core_Boost,
            Radial_Boost,
            Total_Core_Revamp,
            Partial_Core_Revamp,
            Total_Radial_Revamp,
            Partial_Radial_Revamp,
            Core_Paragon,
            Radial_Paragon
        }

        public enum eJudgementOrder
        {
            Judgement,
            Core_Judgement,
            Radial_Judgement,
            Total_Core_Judgement,
            Partial_Core_Judgement,
            Total_Radial_Judgement,
            Partial_Radial_Judgement,
            Core_Final_Judgement,
            Radial_Final_Judgement
        }

        public enum eInterfaceOrder
        {
            Interface,
            Core_Interface,
            Radial_Interface,
            Total_Core_Conversion,
            Partial_Core_Conversion,
            Total_Radial_Conversion,
            Partial_Radial_Conversion,
            Core_Flawless_Interface,
            Radial_Flawless_Interface
        }

        public enum eLoreOrder
        {
            Ally,
            Core_Ally,
            Radial_Ally,
            Total_Core_Improved_Ally,
            Partial_Core_Improved_Ally,
            Total_Radial_Improved_Ally,
            Partial_Radial_Improved_Ally,
            Core_Superior_Ally,
            Radial_Superior_Ally
        }

        public enum eDestinyOrder
        {
            Invocation,
            Core_Invocation,
            Radial_Invocation,
            Total_Core_Invocation,
            Partial_Core_Invocation,
            Total_Radial_Invocation,
            Partial_Radial_Invocation,
            Core_Epiphany,
            Radial_Epiphany
        }

        public enum eHybridOrder
        {
            Genome,
            Core_Genome,
            Radial_Genome,
            Total_Core_Graft,
            Partial_Core_Graft,
            Total_Radial_Graft,
            Partial_Radial_Graft,
            Core_Embodiment,
            Radial_Embodiment
        }

        public enum eCSVImport_Damage
        {
            None,
            Smashing,
            Lethal,
            Fire,
            Cold,
            Energy,
            Negative_Energy,
            Toxic,
            Psionic,
            Special,
            Melee,
            Ranged,
            AoE,
            Unique1,
            Unique2,
            Unique3
        }

        public enum eCSVImport_Damage_Def
        {
            None,
            Smashing_Attack,
            Lethal_Attack,
            Fire_Attack,
            Cold_Attack,
            Energy_Attack,
            Negative_Energy_Attack,
            Toxic_Attack,
            Psionic_Attack,
            Special,
            Melee_Attack,
            Ranged_Attack,
            AoE_Attack
        }

        public enum eCSVImport_Damage_Elusivity
        {
            None,
            Smashing_Elude,
            Lethal_Elude,
            Fire_Elude,
            Cold_Elude,
            Energy_Elude,
            Negative_Elude,
            Toxic_Elude,
            Psionic_Elude,
            Special_Elude,
            Melee_Elude,
            Ranged_Elude,
            AoE_Elude
        }

        public enum eDefense
        {
            None,
            All,
            Smashing,
            Lethal,
            Fire,
            Cold,
            Energy,
            Negative,
            Psionic,
            Melee,
            Ranged,
            AoE
        }

        public enum eResistance
        {
            None,
            All,
            Smashing,
            Lethal,
            Fire,
            Cold,
            Energy,
            Negative,
            Toxic,
            Psionic
        }

        public enum eMezResist
        { 
            None,
            All,
            Confused,
            Held,
            Immobilized,
            Knockback,
            Knockup,
            Placate,
            Repel,
            Sleep,
            Stunned,
            Taunt,
            Terrorized,
            Teleport,
        }

        public enum eDamage
        {
            None,
            Smashing,
            Lethal,
            Fire,
            Cold,
            Energy,
            Negative,
            Toxic,
            Psionic,
            Special,
            Melee,
            Ranged,
            AoE,
            Unique1,
            Unique2,
            Unique3
        }

        public enum eDamageShort
        {
            None,
            Smash,
            Lethal,
            Fire,
            Cold,
            Energy,
            Neg,
            Toxic,
            Psi,
            Spec,
            Melee,
            Rngd,
            AoE
        }

        public enum eDDAlign
        {
            Left,
            Center,
            Right
        }

        public enum eDDGraph
        {
            Simple,
            Enhanced,
            Both,
            Stacked
        }

        public enum eDDStyle
        {
            Text,
            Graph,
            TextOnGraph,
            TextUnderGraph
        }

        public enum eDDText
        {
            ActualValues,
            OnlyBase,
            OnlyEnhanced,
            pcOfBase,
            pcMaxBase,
            pcMaxEnh,
            DPS
        }

        public enum eEffectArea
        {
            None,
            Character,
            Sphere,
            Cone,
            Location,
            Volume,
            Map,
            Room,
            Touch
        }

        public enum eEffectClass
        {
            Primary,
            Secondary,
            Tertiary,
            Special,
            Ignored,
            DisplayOnly
        }

        public enum eEffectType
        {
            None,
            Accuracy,
            ViewAttrib,
            Damage,
            DamageBuff,
            Defense,
            DropToggles,
            Endurance,
            EnduranceDiscount,
            Enhancement,
            Fly,
            SpeedFlying,
            GrantPower,
            Heal,
            HitPoints,
            InterruptTime,
            JumpHeight,
            SpeedJumping,
            Meter,
            Mez,
            MezResist,
            MovementControl,
            MovementFriction,
            PerceptionRadius,
            Range,
            RechargeTime,
            Recovery,
            Regeneration,
            ResEffect,
            Resistance,
            RevokePower,
            Reward,
            SpeedRunning,
            SetCostume,
            SetMode,
            Slow,
            StealthRadius,
            StealthRadiusPlayer,
            EntCreate,
            ThreatLevel,
            ToHit,
            Translucency,
            XPDebtProtection,
            SilentKill,
            Elusivity,
            GlobalChanceMod,
            CombatModShift,
            UnsetMode,
            Rage,
            MaxRunSpeed,
            MaxJumpSpeed,
            MaxFlySpeed,
            DesignerStatus,
            PowerRedirect,
            TokenAdd,
            ExperienceGain,
            InfluenceGain,
            PrestigeGain,
            AddBehavior,
            RechargePower,
            RewardSourceTeam,
            VisionPhase,
            CombatPhase,
            ClearFog,
            SetSZEValue,
            ExclusiveVisionPhase,
            Absorb,
            XAfraid,
            XAvoid,
            BeastRun,
            ClearDamagers,
            EntCreate_x,
            Glide,
            Hoverboard,
            Jumppack,
            MagicCarpet,
            NinjaRun,
            Null,
            NullBool,
            Stealth,
            SteamJump,
            Walk,
            XPDebt,
            ForceMove,
            ModifyAttrib
        }

        public enum eEffectTypeShort
        {
            None,
            Acc,
            Anlyz,
            Dmg,
            DamBuff,
            Def,
            ToglDrop,
            Endrnce,
            EndRdx,
            Enhance,
            Fly,
            FlySpd,
            Grant,
            Heal,
            HP,
            ActRdx,
            Jump,
            JumpSpd,
            Meter,
            Mez,
            MezRes,
            MveCtrl,
            MveFrctn,
            Pceptn,
            Rng,
            Rechg,
            EndRec,
            Regen,
            ResEffect,
            Res,
            Revke,
            Reward,
            RunSpd,
            Costume,
            Smode,
            Slow,
            StealthR,
            StealthP,
            Summon,
            ThreatLvl,
            ToHit,
            Tnslncy,
            DebtProt,
            Expire,
            Elsvty,
            GlobalChance,
            LvlShift,
            ClrMode,
            Fury,
            MaxRunSpd,
            MaxJumpSpd,
            MaxFlySpd,
            DeStatus,
            Redirect,
            TokenAdd,
            RDebuff1,
            RDebuff2,
            RDebuff3,
            AddBehav,
            RechPower,
            LostCure,
            VisionPhase,
            CombatPhase,
            ClearFog,
            SetSZEValue,
            ExVisionPhase,
            Absorb,
            Afraid,
            Avoid,
            BeastRun,
            ClearDamagers,
            EntCreate,
            Glide,
            Hoverboard,
            Jumppack,
            MagicCarpet,
            NinjaRun,
            Null,
            NullBool,
            Stealth,
            SteamJump,
            Walk,
            XPDebt,
            ForceMove,
            ModifyAttrib
        }

        public enum ePowerAttribs
        {
            None,
            Accuracy,
            ActivateInterval,
            Arc,
            CastTime,
            EffectArea,
            EnduranceCost,
            InterruptTime,
            MaxTargets,
            Radius,
            Range,
            RechargeTime,
            SecondaryRange
        }

        public enum eEffMode
        {
            Enhancement,
            FX,
            PowerEnh,
            PowerProc
        }

        public enum eEnhance
        {
            None,
            Accuracy,
            Damage,
            Defense,
            EnduranceDiscount,
            Endurance,
            SpeedFlying,
            Heal,
            HitPoints,
            Interrupt,
            JumpHeight,
            SpeedJumping,
            Mez,
            Range,
            RechargeTime,
            X_RechargeTime,
            Recovery,
            Regeneration,
            Resistance,
            SpeedRunning,
            ToHit,
            Slow,
            Absorb
        }

        public enum eEnhanceShort
        {
            None,
            Acc,
            Dmg,
            Def,
            EndRdx,
            EndMod,
            Fly,
            Heal,
            HP,
            ActRdx,
            Jump,
            JumpSpd,
            Mez,
            Rng,
            Rchg,
            RchrgTm,
            EndRec,
            Regen,
            Res,
            RunSpd,
            ToHit,
            Slow
        }

        public enum eEnhGrade
        {
            None,
            TrainingO,
            DualO,
            SingleO
        }

        public enum eEnhMutex
        {
            None,
            Stealth,
            ArchetypeA,
            ArchetypeB,
            ArchetypeC,
            ArchetypeD,
            ArchetypeE,
            ArchetypeF
        }

        public enum eEnhRelative
        {
            None,
            MinusThree,
            MinusTwo,
            MinusOne,
            Even,
            PlusOne,
            PlusTwo,
            PlusThree,
            PlusFour,
            PlusFive
        }

        [Flags]
        public enum eEntity
        {
            None = 0,
            Caster = 1,
            Player = 2,
            DeadPlayer = 4,
            Teammate = 8,
            DeadTeammate = 16, // 0x00000010
            DeadOrAliveTeammate = 32, // 0x00000020
            Villain = 64, // 0x00000040
            DeadVillain = 128, // 0x00000080
            NPC = 256, // 0x00000100
            Friend = 512, // 0x00000200
            DeadFriend = 1024, // 0x00000400
            Foe = 2048, // 0x00000800
            Location = 8192, // 0x00002000
            Teleport = 16384, // 0x00004000
            Any = 32768, // 0x00008000
            MyPet = 65536, // 0x00010000
            DeadFoe = 131072, // 0x00020000
            FoeRezzingFoe = 262144, // 0x00040000
            Leaguemate = 524288, // 0x00080000
            DeadLeaguemate = 1048576, // 0x00100000
            AnyLeaguemate = 2097152, // 0x00200000
            DeadMyCreation = 4194304, // 0x00400000
            DeadMyPet = 8388608, // 0x00800000
            DeadOrAliveFoe = 16777216, // 0x01000000
            DeadOrAliveLeaguemate = 33554432, // 0x02000000
            DeadPlayerFriend = 67108864, // 0x04000000
            MyOwner = 134217728 // 0x08000000
        }

        public enum eBoosts
        {
            None,
            Accuracy_Boost,
            Interrupt_Boost,
            Confuse_Boost,
            Damage_Boost,
            Buff_Defense_Boost,
            Debuff_Defense_Boost,
            Recovery_Boost,
            EnduranceDiscount_Boost,
            Fear_Boost,
            SpeedFlying_Boost,
            Heal_Boost,
            Hold_Boost,
            Immobilized_Boost,
            Intangible_Boost,
            Jump_Boost,
            Knockback_Boost,
            Range_Boost,
            Recharge_Boost,
            Res_Damage_Boost,
            SpeedRunning_Boost,
            Sleep_Boost,
            Slow_Boost,
            Stunned_Boost,
            Taunt_Boost,
            Buff_ToHit_Boost,
            Debuff_ToHit_Boost,
            Magic_Boost,
            Mutation_Boost,
            Natural_Boost,
            Science_Boost,
            Technology_Boost,
            Hamidon_Boost,
            Incarnate_Lore_Boost,
            Incarnate_Destiny_Boost,
            Incarnate_Judgement_Boost,
            Incarnate_Interface_Boost
        }

        public enum eGridType
        {
            None,
            Accolade,
            Class,
            Incarnate,
            Inherent,
            Pet,
            Power,
            Powerset,
            Prestige,
            Temp
        }

        public enum eInterfaceMode
        {
            Normal,
            PowerToggle
        }

        public enum eMez
        {
            None,
            Confused,
            Held,
            Immobilized,
            Knockback,
            Knockup,
            OnlyAffectsSelf,
            Placate,
            Repel,
            Sleep,
            Stunned,
            Taunt,
            Terrorized,
            Untouchable,
            Teleport,
            ToggleDrop,
            Afraid,
            Avoid,
            CombatPhase,
            Intangible
        }

        public enum eMezShort
        {
            None,
            Conf,
            Held,
            Immob,
            KB,
            KUp,
            AffSelf,
            Placate,
            Repel,
            Sleep,
            Stun,
            Taunt,
            Fear,
            Untouch,
            TP,
            DeTogg,
            Afraid,
            Avoid,
            Phased,
            Intan
        }

        public enum eMMpets
        {
            Summon_Wolves,
            Summon_Lions,
            Summon_Dire_Wolf,
            Summon_Demonlings,
            Summon_Demons,
            Hell_on_Earth,
            Summon_Demon_Prince,
            Soldiers,
            Spec_Ops,
            Commando,
            Zombie_Horde,
            Grave_Knight,
            Soul_Extraction,
            Lich,
            Call_Genin,
            Call_Jounin,
            Oni,
            Battle_Drones,
            Protector_Bots,
            Assault_Bot,
            Call_Thugs,
            Call_Enforcer,
            Gang_War,
            Call_Bruiser
        }

        [Flags]
        public enum eModeFlags
        {
            None = 0,
            Arena = 1,
            Disable_All = 2,
            Disable_Enhancements = 4,
            Disable_Epic = 8,
            Disable_Inspirations = 16, // 0x00000010
            Disable_Market_TP = 32, // 0x00000020
            Disable_Pool = 64, // 0x00000040
            Disable_Rez_Insp = 128, // 0x00000080
            Disable_Teleport = 256, // 0x00000100
            Disable_Temp = 512, // 0x00000200
            Disable_Toggle = 1024, // 0x00000400
            Disable_Travel = 2048, // 0x00000800
            Domination = 4096, // 0x00001000
            Peacebringer_Blaster_Mode = 8192, // 0x00002000
            Peacebringer_Lightform_Mode = 16384, // 0x00004000
            Peacebringer_Tanker_Mode = 32768, // 0x00008000
            Raid_Attacker_Mode = 65536, // 0x00010000
            Shivan_Mode = 131072, // 0x00020000
            Unknown18 = 262144, // 0x00040000
            Warshade_Blaster_Mode = 524288, // 0x00080000
            Warshade_Tanker_Mode = 1048576 // 0x00100000
        }

        public enum eMutex
        {
            NoGroup,
            NoConflict,
            MutexFound,
            DetoggleMaster,
            DetoggleSlave
        }

        public enum eNotify
        {
            Always,
            Never,
            MissOnly,
            HitOnly
        }

        public enum eOverrideBoolean
        {
            NoOverride,
            TrueOverride,
            FalseOverride
        }

        public enum ePowerSetType
        {
            None,
            Primary,
            Secondary,
            Ancillary,
            Inherent,
            Pool,
            Accolade,
            Temp,
            Pet,
            SetBonus,
            Boost,
            Incarnate,
            Redirect
        }

        public enum ePowerState
        {
            Disabled,
            Empty,
            Used,
            Open
        }

        public enum ePowerType
        {
            Click,
            Auto_,
            Toggle,
            Boost,
            Inspiration,
            GlobalBoost
        }

        public enum ePvX
        {
            Any,
            PvE,
            PvP
        }

        public enum eSchedule
        {
            None = -1,
            A = 0,
            B = 1,
            C = 2,
            D = 3,
            Multiple = 4
        }

        public enum eSetType
        {
            Untyped,
            MeleeST,
            RangedST,
            RangedAoE,
            MeleeAoE,
            Snipe,
            Pets,
            Defense,
            Resistance,
            Heal,
            Hold,
            Stun,
            Immob,
            Slow,
            Sleep,
            Fear,
            Confuse,
            Flight,
            Jump,
            Run,
            Teleport,
            DefDebuff,
            EndMod,
            Knockback,
            Threat,
            ToHit,
            ToHitDeb,
            PetRech,
            Travel,
            AccHeal,
            AccDefDeb,
            AccToHitDeb,
            Arachnos,
            Blaster,
            Brute,
            Controller,
            Corruptor,
            Defender,
            Dominator,
            Kheldian,
            Mastermind,
            Scrapper,
            Stalker,
            Tanker,
            UniversalDamage,
            Sentinel,
            RunNoSprint,
            JumpNoSprint,
            FlightNoSprint,
            TeleportNoSprint
        }

        public enum eSpecialCase
        {
            None,
            Hidden,
            Domination,
            Scourge,
            Mezzed,
            CriticalHit,
            CriticalBoss,
            CriticalMinion,
            Robot,
            Assassination,
            Containment,
            Defiance,
            TargetDroneActive,
            Combo,
            VersusSpecial,
            NotDisintegrated,
            Disintegrated,
            NotAccelerated,
            Accelerated,
            NotDelayed,
            Delayed,
            ComboLevel0,
            ComboLevel1,
            ComboLevel2,
            ComboLevel3,
            FastMode,
            NotAssassination,
            PerfectionOfBody0,
            PerfectionOfBody1,
            PerfectionOfBody2,
            PerfectionOfBody3,
            PerfectionOfMind0,
            PerfectionOfMind1,
            PerfectionOfMind2,
            PerfectionOfMind3,
            PerfectionOfSoul0,
            PerfectionOfSoul1,
            PerfectionOfSoul2,
            PerfectionOfSoul3,
            TeamSize1,
            TeamSize2,
            TeamSize3,
            NotComboLevel3,
            ToHit97,
            DefensiveAdaptation,
            EfficientAdaptation,
            OffensiveAdaptation,
            NotDefensiveAdaptation,
            NotDefensiveNorOffensiveAdaptation,
            BoxingBuff,
            KickBuff,
            Supremacy,
            SupremacyAndBuffPwr,
            PetTier2,
            PetTier3,
            PackMentality,
            NotPackMentality,
            FastSnipe,
            NotFastSnipe,
            CrossPunchBuff,
            NotCrossPunchBuff,
            NotBoxingBuff,
            NotKickBuff
        }

        public enum eSpeedMeasure
        {
            FeetPerSecond,
            MetersPerSecond,
            MilesPerHour,
            KilometersPerHour
        }

        public enum eStacking
        {
            No,
            Yes
        }

        public enum eStats
        {
            None,
            Smashing_Defense,
            Lethal_Defense,
            Fire_Defense,
            Cold_Defense,
            Energy_Defense,
            Negative_Defense,
            Psionic_Defense,
            Melee_Defense,
            Ranged_Defense,
            AOE_Defense,
            Smashing_Resistance,
            Lethal_Resistance,
            Fire_Resistance,
            Cold_Resistance,
            Energy_Resistance,
            Negative_Resistance,
            Toxic_Resistance,
            Psionic_Resistance,
            Damage_Buff,
            Endurance_Usage,
            Global_Recharge,
            Recovery,
            Regen,
            ToHit
        }

        public enum eSubtype
        {
            None,
            Hamidon,
            Hydra,
            Titan,
            DSync
        }

        public enum eSummonEntity
        {
            Pet,
            Henchman
        }

        [Flags]
        public enum eSuppress
        {
            None = 0,
            Held = 1,
            Sleep = 2,
            Stunned = 4,
            Immobilized = 8,
            Terrorized = 16, // 0x00000010
            Knocked = 32, // 0x00000020
            Attacked = 64, // 0x00000040
            HitByFoe = 128, // 0x00000080
            MissionObjectClick = 256, // 0x00000100
            ActivateAttackClick = 512, // 0x00000200
            Damaged = 1024, // 0x00000400
            Phased1 = 2048, // 0x00000800
            Confused = 4096 // 0x00001000
        }

        public enum eToWho
        {
            Unspecified,
            Target,
            Self,
            All
        }

        public enum eType
        {
            None,
            Normal,
            InventO,
            SpecialO,
            SetO
        }

        public enum eValidationType
        {
            Power,
            Powerset
        }

        [Flags]
        public enum eVector
        {
            None = 0,
            Melee_Attack = 1,
            Ranged_Attack = 2,
            AOE_Attack = 4,
            Smashing_Attack = 8,
            Lethal_Attack = 16, // 0x00000010
            Cold_Attack = 32, // 0x00000020
            Fire_Attack = 64, // 0x00000040
            Energy_Attack = 128, // 0x00000080
            Negative_Energy_Attack = 256, // 0x00000100
            Psionic_Attack = 512 // 0x00000200
        }

        public enum eVisibleSize
        {
            Full,
            Small,
            VerySmall,
            Compact
        }

        public enum GraphStyle
        {
            Twin,
            Stacked,
            baseOnly,
            enhOnly
        }

        public enum PowersetType
        {
            None = -1,
            Primary = 0,
            Secondary = 1,
            Inherent = 2,
            Pool0 = 3,
            Pool1 = 4,
            Pool2 = 5,
            Pool3 = 6,
            Ancillary = 7
        }

        // clsToonX::GBD_Totals() stat types
        // for _selfBuff.Effect[n] and _selfEnhance.Effect[n]
        public enum eStatType
        {
            // _selfBuffs.Effect
            FlySpeed = 11,
            HPMax = 14,
            JumpHeight = 16,
            JumpSpeed = 17,
            Perception = 23,
            EndRec = 26,
            HPRegen = 27,
            RunSpeed = 32,
            StealthPvE = 36,
            StealthPvP = 37,
            ThreatLevel = 39,
            ToHit = 40,
            Elusivity = 44,
            MaxRunSpeed = 49,
            MaxJumpSpeed = 50,
            MaxFlySpeed = 51,
            Absorb = 66,

            // _selfEnhance.Effect //
            BuffEndRdx = 8,
            Haste = 25,

            // Any //
            BuffAcc = 1
        }

        public enum eBarGroup
        {
            Defense = 0,
            Resistance = 1,
            HP = 2,
            Endurance = 3,

            Movement = 4,
            Stealth = 5,
            MiscBuffs = 6,

            StatusProtection = 7,
            StatusResistance = 8,

            DebuffResistance = 9
        }

        public enum eBarType
        {
            DefenseSmashing = 0,
            DefenseLethal = 1,
            DefenseFire = 2,
            DefenseCold = 3,
            DefenseEnergy = 4,
            DefenseNegative = 5,
            DefensePsionic = 6,
            DefenseMelee = 7,
            DefenseRanged = 8,
            DefenseAoE = 9,
        
            ResistanceSmashing = 10,
            ResistanceLethal = 11,
            ResistanceFire = 12,
            ResistanceCold = 13,
            ResistanceEnergy = 14,
            ResistanceNegative = 15,
            ResistanceToxic = 16,
            ResistancePsionic = 17,

            Regeneration = 18,
            MaxHPAbsorb = 19,
            EndRec = 20,
            EndUse = 21,
            MaxEnd = 22,

            RunSpeed = 23,
            JumpSpeed = 24,
            JumpHeight = 25,
            FlySpeed = 26,

            StealthPvE = 27,
            StealthPvP = 28,
            Perception = 29,

            Haste = 30,
            ToHit = 31,
            Accuracy = 32,
            Damage = 33,
            EndRdx = 34,
            ThreatLevel = 35,
            Elusivity = 36,

            MezProtectionHold = 37,
            MezProtectionStunned = 38,
            MezProtectionSleep = 39,
            MezProtectionImmob = 40,
            MezProtectionKnockback = 41,
            MezProtectionRepel = 42,
            MezProtectionConfuse = 43,
            MezProtectionFear = 44,
            MezProtectionTaunt = 45,
            MezProtectionPlacate = 46,
            MezProtectionTeleport = 47,

            MezResistanceHold = 48,
            MezResistanceStunned = 49,
            MezResistanceSleep = 50,
            MezResistanceImmob = 51,
            MezResistanceKnockback = 52,
            MezResistanceRepel = 53,
            MezResistanceConfuse = 54,
            MezResistanceFear = 55,
            MezResistanceTaunt = 56,
            MezResistancePlacate = 57,
            MezResistanceTeleport = 58,

            DebuffResistanceDefense = 59,
            DebuffResistanceEndurance = 60,
            DebuffResistanceRecovery = 61,
            DebuffResistancePerception = 62,
            DebuffResistanceToHit = 63,
            DebuffResistanceRechargeTime = 64,
            DebuffResistanceSpeedRunning = 65,
            DebuffResistanceRegen = 66
        }

        public enum eFXGroup
        {
            Offense = 0,
            Defense = 1,
            Survival = 2,
            StatusEffects = 3,
            Movement = 4,
            Perception = 5,
            Misc = 6
        }

        public enum eFXSubGroup
        {
            NoGroup = 0,
            DamageAll = 1,
            MezResistAll = 2,
            SmashLethalDefense = 3,
            FireColdDefense = 4,
            EnergyNegativeDefense = 5,
            SmashLethalResistance = 6,
            FireColdResistance = 7,
            EnergyNegativeResistance = 8,
            SlowResistance = 9,
            SlowBuffs = 10,
            KnockProtection = 11,
            KnockResistance = 12,
            Jump = 13
        }

        public enum RewardCurrency
        {
            RewardMerit = 0,
            AstralMerit = 1,
            EmpyreanMerit = 2,
            AlignmentMerit = 3,
            VanguardMerit = 4,
            AETicket = 5,
            Influence = 6
        }

        public enum eColorSetting
        {
            ColorBackgroundHero = 0,
            ColorBackgroundVillain = 1,
            ColorText = 2,
            ColorInvention = 3,
            ColorInventionInv = 4,
            ColorFaded = 5,
            ColorEnhancement = 6,
            ColorWarning = 7,
            ColorPlName = 8,
            ColorPlSpecial = 9,
            ColorPowerAvailable = 10,
            ColorPowerDisabled = 11,
            ColorPowerTakenHero = 12,
            ColorPowerTakenDarkHero = 13,
            ColorPowerHighlightHero = 14,
            ColorPowerTakenVillain = 15,
            ColorPowerTakenDarkVillain = 16,
            ColorPowerHighlightVillain = 17,
            ColorDamageBarBase = 18,
            ColorDamageBarEnh = 19
        }

        public enum eFontSizeSetting
        {
            PairedBase = 0,
            PowersSelectBase = 1,
            PowersBase = 2
        }

        public static bool MezDurationEnhanceable(eMez mezEnum)
        {
            return mezEnum is eMez.Confused or eMez.Held or eMez.Immobilized or eMez.Placate or eMez.Sleep or eMez.Stunned or eMez.Taunt or eMez.Terrorized or eMez.Untouchable;
        }

        public static string GetEffectName(eEffectType iID)
        {
            return iID.ToString();
        }

        public static string GetEffectNameShort(eEffectType iID)
        {
            return iID != eEffectType.Endurance ? ((eEffectTypeShort) iID).ToString() : "End";
        }

        public static string GetMezName(eMezShort iID)
        {
            return ((eMez) iID).ToString();
        }

        public static string GetMezName(eMez iID)
        {
            return iID.ToString();
        }

        public static string GetMezNameShort(eMezShort iID)
        {
            return iID.ToString();
        }

        public static string GetDamageName(eDamage iID)
        {
            return iID.ToString();
        }

        public static string GetDamageNameShort(eDamage iID)
        {
            return ((eDamageShort) iID).ToString();
        }

        public static string GetRelativeString(eEnhRelative iRel, bool onlySign)
        {
            if (onlySign)
                return iRel switch
                {
                    eEnhRelative.MinusThree => "---",
                    eEnhRelative.MinusTwo => "--",
                    eEnhRelative.MinusOne => "-",
                    eEnhRelative.Even => string.Empty,
                    eEnhRelative.PlusOne => "+",
                    eEnhRelative.PlusTwo => "++",
                    eEnhRelative.PlusThree => "+++",
                    eEnhRelative.PlusFour => "+4",
                    eEnhRelative.PlusFive => "+5",
                    _ => string.Empty
                };

            return iRel switch
            {
                eEnhRelative.MinusThree => "-3",
                eEnhRelative.MinusTwo => "-2",
                eEnhRelative.MinusOne => "-1",
                eEnhRelative.Even => string.Empty,
                eEnhRelative.PlusOne => "+1",
                eEnhRelative.PlusTwo => "+2",
                eEnhRelative.PlusThree => "+3",
                eEnhRelative.PlusFour => "+4",
                eEnhRelative.PlusFive => "+5",
                _ => string.Empty
            };
        }

        public static int StringToFlaggedEnum(string iStr, object eEnum, bool noFlag = false)
        {
            var num1 = 0;
            iStr = iStr.ToUpper();
            var strArray1 = !iStr.Contains(",") ? iStr.Split(' ') : StringToArray(iStr);
            var strArray2 = strArray1;
            int num2;
            if (strArray2.Length < 1)
            {
                num2 = num1;
            }
            else
            {
                var names = Enum.GetNames(eEnum.GetType());
                var values = Enum.GetValues(eEnum.GetType());
                for (var index = 0; index < names.Length; ++index)
                    names[index] = names[index].ToUpper();
                foreach (var index1 in strArray2)
                {
                    if (index1.Length <= 0)
                        continue;
                    var index2 = Array.IndexOf(names, index1);
                    if (index2 <= -1)
                        continue;
                    if (noFlag)
                        return (int) values.GetValue(index2);
                    num1 += (int) values.GetValue(index2);
                }

                num2 = num1;
            }

            return num2;
        }

        public static T[] StringToEnumArray<T>(string iStr, Type eEnum)
        {
            var objList = new List<T>();
            iStr = iStr.ToUpper();
            var strArray1 = !iStr.Contains(",") ? iStr.Split(' ') : StringToArray(iStr);
            var strArray2 = strArray1;
            T[] array;
            if (strArray2.Length < 1)
            {
                array = objList.ToArray();
            }
            else
            {
                var names = Enum.GetNames(eEnum);
                var values = Enum.GetValues(eEnum);
                for (var index = 0; index < names.Length; ++index)
                    names[index] = names[index].ToUpper();
                objList.AddRange(from t in strArray2
                    where t.Length > 0
                    select Array.IndexOf(names, t)
                    into index2
                    where index2 > -1
                    select (T) values.GetValue(index2));
                array = objList.ToArray();
            }

            return array;
        }

        public static bool IsEnumValue(string iStr, object eEnum)
        {
            bool flag;
            if (iStr == null)
            {
                flag = false;
            }
            else
            {
                var names = Enum.GetNames(eEnum.GetType());
                iStr = iStr.ToUpper();
                for (var index = 0; index < names.Length; ++index)
                    names[index] = names[index].ToUpper();
                flag = Array.IndexOf(names, iStr) > -1;
            }

            return flag;
        }

        public static string[] StringToArray(string iStr)
        {
            var strArray1 = Array.Empty<string>();
            string[] strArray2;
            if (iStr == null)
            {
                strArray2 = strArray1;
            }
            else if (string.IsNullOrEmpty(iStr))
            {
                strArray2 = strArray1;
            }
            else
            {
                char[] chArray = {','};
                iStr = iStr.Replace(", ", ",");
                var array = iStr.Split(chArray);
                Array.Sort(array);
                strArray2 = array;
            }

            return strArray2;
        }

        public static string GetGroupedDamage(bool[] iDamage, bool shortForm)
        {
            string str1;
            if (iDamage.Length < Enum.GetValues(eDamage.None.GetType()).Length - 1)
            {
                str1 = "Error: Array Length Mismatch";
            }
            else
            {
                var str2 = "";
                if (iDamage[1] && iDamage[2] && iDamage[3] && iDamage[4] && iDamage[5] && iDamage[6] && iDamage[8] &&
                    iDamage[7])
                    str2 = "All";
                else
                    for (var index = 0; index < iDamage.Length; ++index)
                    {
                        if (!iDamage[index])
                            continue;
                        var str3 = shortForm ? GetDamageNameShort((eDamage) index) : GetDamageName((eDamage) index);
                        if (!string.IsNullOrEmpty(str2))
                            str2 += ",";
                        str2 += str3;
                    }

                str1 = str2;
            }

            return str1;
        }

        public static string GetGroupedDefense(bool[] iDamage, bool shortForm)
        {
            string str1;
            if (iDamage.Length < Enum.GetValues(eDamage.None.GetType()).Length - 1)
            {
                str1 = "Error: Array Length Mismatch";
            }
            else
            {
                var str2 = "";
                if (iDamage[1] && iDamage[2] && iDamage[3] && iDamage[4] && iDamage[5] && iDamage[6] && iDamage[8] && iDamage[10] && iDamage[11] && iDamage[12])
                {
                    str2 = "All";
                }
                else
                {
                    for (var index = 0; index < iDamage.Length; ++index)
                    {
                        if (!iDamage[index])
                        {
                            continue;
                        }

                        var str3 = shortForm ? GetDamageNameShort((eDamage)index) : GetDamageName((eDamage)index);
                        if (!string.IsNullOrEmpty(str2))
                        {
                            str2 += ",";
                        }

                        str2 += str3;
                    }
                }

                str1 = str2;
            }

            return str1;
        }

        public static string GetGroupedMez(bool[] iMez, bool shortForm)
        {
            string str1;
            if (iMez.Length < Enum.GetValues(eMez.None.GetType()).Length - 1)
            {
                str1 = "Error: Array Length Mismatch";
            }
            else
            {
                var str2 = "";
                if (iMez[1] && iMez[2] && iMez[10] && iMez[9])
                    str2 = "Mez";
                else if (iMez[4] && iMez[5] && iMez[1] && iMez[2] && iMez[10] && iMez[9])
                    str2 = "Knocked";
                else
                    for (var index = 0; index < iMez.Length; ++index)
                    {
                        if (!iMez[index])
                            continue;
                        var str3 = shortForm ? GetMezNameShort((eMezShort) index) : GetMezName((eMezShort) index);
                        if (!string.IsNullOrEmpty(str2))
                            str2 += ",";
                        str2 += str3;
                    }

                str1 = str2;
            }

            return str1;
        }

        public struct sEnhClass
        {
            public int ID;
            public string Name;
            public string ShortName;
            public string ClassID;
            public string Desc;
        }

        public struct sTwinID
        {
            public int ID;
            public int SubID;
        }

        public struct sEffect
        {
            public eEffMode Mode;
            public eBuffDebuff BuffMode;
            public sTwinID Enhance;
            public eSchedule Schedule;
            public float Multiplier;
            public IEffect FX;

            public void Assign(sEffect effect)
            {
                Mode = effect.Mode;
                BuffMode = effect.BuffMode;
                Enhance.ID = effect.Enhance.ID;
                Enhance.SubID = effect.Enhance.SubID;
                Schedule = effect.Schedule;
                Multiplier = effect.Multiplier;
                if (effect.FX == null)
                    return;
                FX = (IEffect) effect.FX.Clone();
            }
        }

        public struct ShortFX
        {
            public int[] Index;
            public float[] Value;
            public float Sum;

            public bool Present => Index != null && Index.Length >= 1 && Index[0] != -1;

            public bool Multiple => Index != null && Index.Length > 1;

            public int Max
            {
                get
                {
                    var num1 = 0.0f;
                    var num2 = 0;
                    int num3;
                    if (Present)
                    {
                        for (var index = 0; index < Value.Length; ++index)
                        {
                            if (!(Value[index] > (double) num1))
                                continue;
                            num1 = Value[index];
                            num2 = index;
                        }

                        num3 = num2;
                    }
                    else
                    {
                        num3 = -1;
                    }

                    return num3;
                }
            }

            public void Add(int iIndex, float iValue)
            {
                if (Value == null)
                {
                    Value = Array.Empty<float>();
                    Index = Array.Empty<int>();
                    Sum = 0.0f;
                }

                var values = Value.ToList();
                values.Add(iValue);
                Value = values.ToArray();

                var indexes = Index.ToList();
                indexes.Add(iIndex);
                Index = indexes.ToArray();
                
                Sum += iValue;
            }

            public void Remove(int iIndex)
            {
                var numArray1 = new float[Value.Length - 1];
                var numArray2 = new int[Index.Length - 1];
                var index1 = 0;
                for (var index2 = 0; index2 <= Index.Length - 1; ++index2)
                {
                    if (index2 == iIndex)
                        continue;
                    numArray1[index1] = Value[index2];
                    numArray2[index1] = Index[index2];
                    ++index1;
                }

                Value = numArray1;
                Index = numArray2;
            }

            public void Assign(ShortFX iFX)
            {
                if (iFX.Present)
                {
                    Index = iFX.Index;
                    Value = iFX.Value;
                    Sum = iFX.Sum;
                }
                else
                {
                    Index = Array.Empty<int>();
                    Value = Array.Empty<float>();
                    Sum = 0.0f;
                }
            }

            public void Multiply()
            {
                if (Value == null)
                    return;
                for (var index = 0; index <= Value.Length - 1; ++index)
                    Value[index] *= 100f;
                Sum *= 100f;
            }

            public void ReSum()
            {
                Sum = 0.0f;
                foreach (var index in Value)
                    Sum += index;
            }
        }

        public struct CompMap
        {
            public const int UBound = 21;
            public int[] IdxAT;
            public int[] IdxSet;
            public int[,] Map;

            public void Init()
            {
                IdxAT = new int[2];
                IdxSet = new int[2];
                Map = new int[21, 2];
                for (var index = 0; index < 21; ++index)
                {
                    Map[index, 0] = -1;
                    Map[index, 1] = -1;
                }
            }
        }

        public struct CompOverride
        {
            public string Powerset;
            public string Power;
            public string Override;
        }

        public struct BuffsX
        {
            public float[] Effect;
            public float[] EffectAux;
            public float[] Mez;
            public float[] MezRes;
            public float[] Damage;
            public float[] Defense;
            public float[] Resistance;
            public float[] Elusivity;
            public float[] StatusProtection;
            public float[] StatusResistance;
            public float[] DebuffResistance;
            public float MaxEnd;

            public void Reset()
            {
                MaxEnd = 0.0f;
                Effect = new float[Enum.GetValues(eEffectType.None.GetType()).Length];
                EffectAux = new float[Effect.Length - 1];
                Mez = new float[Enum.GetValues(eMez.None.GetType()).Length];
                MezRes = new float[Enum.GetValues(eMez.None.GetType()).Length];
                Damage = new float[Enum.GetValues(typeof(eDamage)).Length];
                Defense = new float[Enum.GetValues(eDamage.None.GetType()).Length];
                Resistance = new float[Enum.GetValues(eDamage.None.GetType()).Length];
                Elusivity = new float[Enum.GetValues(eDamage.None.GetType()).Length];
                StatusProtection = new float[Enum.GetValues(eMez.None.GetType()).Length];
                StatusResistance = new float[Enum.GetValues(eMez.None.GetType()).Length];
                DebuffResistance = new float[Enum.GetValues(eEffectType.None.GetType()).Length];
            }
        }

        public enum eHTextAlign
        {
            Left,
            Center,
            Right
        }

        public enum eVTextAlign
        {
            Top,
            Middle,
            Bottom,
            BaseLine
        }

        public class VersionData
        {
            public int Revision;
            public DateTime RevisionDate = new(0L, DateTimeKind.Local);
            public string SourceFile = "";

            public void Load(BinaryReader reader, bool fromChanged = false)
            {
                Revision = reader.ReadInt32();
                if (fromChanged)
                {
                    long usedTicks;
                    DateTime date;
                    var ticks = reader.ReadInt64();
                    if (ticks > 0)
                    {
                        usedTicks = -ticks & -0x3FFFFFFFFFFFFFFF;
                        date = DateTime.FromBinary(usedTicks);
                        Debug.WriteLine($"Ticks: {usedTicks}\r\nDate: {date}");
                    }
                    else
                    {
                        usedTicks = ticks;
                        date = DateTime.FromBinary(usedTicks);
                        Debug.WriteLine($"Ticks: {usedTicks}\r\nDate: {date}");
                    }

                    
                    //RevisionDate = DateTime.SpecifyKind(DateTime.FromBinary(ticks), DateTimeKind.Local);
                }
                else
                {
                    RevisionDate = DateTime.FromBinary(reader.ReadInt64());
                }

                SourceFile = reader.ReadString();
            }

            public void StoreTo(BinaryWriter writer)
            {
                writer.Write(Revision);
                writer.Write(RevisionDate.ToBinary());
                writer.Write(FileIO.StripPath(SourceFile));
            }
        }
    }
}