namespace Mids_Reborn.Core
{
    public class Stats
    {
        public class Display
        {
            private static Display _instance;
            private static readonly object Mutex = new();

            public Absorb Absorb { get; set; }
            public Accuracy Accuracy { get; set; }
            public DamageBuff DamageBuff { get; set; }
            public DebuffResistance DebuffResistance { get; set; }
            public Defense Defense { get; set; }
            public DefenseDebuff DefenseDebuff { get; set; }
            public Elusivity Elusivity { get; set; }
            public Endurance Endurance { get; set; }
            public float EnduranceReduction { get; set; }
            public float EnduranceUsage { get; set; }
            public HitPoints HitPoints { get; set; }
            public Movement Movement { get; set; }
            public Perception Perception { get; set; }
            public Recharge Recharge { get; set; }
            public Recovery Recovery { get; set; }
            public Regeneration Regeneration { get; set; }
            public Resistance Resistance { get; set; }
            public ResistanceDebuff ResistanceDebuff { get; set; }
            public StatusProtection StatusProtection { get; set; }
            public StatusResistance StatusResistance { get; set; }
            public StealthRadius StealthRadius { get; set; }
            public Threat Threat { get; set; }
            public ToHit ToHit { get; set; }

            // public static Display GetInstance(bool noReset = true)
            // {
            //     if (_instance != null && !noReset) return _instance;
            //     lock (Mutex)
            //     {
            //         _instance = new Display();
            //         _instance.Initialize();
            //     }
            //     return _instance;
            // }

            internal Display()
            {
                Initialize();
            }

            public void Initialize(bool noReset = true)
            {
                
                DebuffResistance = new DebuffResistance
                {
                    Defense = new Defense
                    {
                        Smashing = new Smashing { Base = 0f, Current = 0f, Maximum = 0f },
                        Lethal = new Lethal { Base = 0f, Current = 0f, Maximum = 0f },
                        Fire = new Fire { Base = 0f, Current = 0f, Maximum = 0f },
                        Cold = new Cold { Base = 0f, Current = 0f, Maximum = 0f },
                        Energy = new Energy { Base = 0f, Current = 0f, Maximum = 0f },
                        Negative = new Negative { Base = 0f, Current = 0f, Maximum = 0f },
                        Psionic = new Psionic { Base = 0f, Current = 0f, Maximum = 0f },
                        Melee = new Melee { Base = 0f, Current = 0f, Maximum = 0f },
                        Ranged = new Ranged { Base = 0f, Current = 0f, Maximum = 0f },
                        Aoe = new Aoe { Base = 0f, Current = 0f, Maximum = 0f }
                    },
                    Endurance = new Endurance { Base = 0f, Current = 0f, Maximum = 0f },
                    Perception = new Perception { Base = 0f, Current = 0f, Maximum = 0f },
                    Recharge = new Recharge { Base = 0f, Current = 0f, Maximum = 0f },
                    Recovery = new Recovery { Base = 0f, Current = 0f, Maximum = 0f },
                    Regeneration = new Regeneration { Base = 0f, Current = 0f, Maximum = 0f },
                    RunSpeed = new RunSpeed { Base = 0f, Current = 0f, Maximum = 0f },
                    ToHit = new ToHit { Base = 0f, Current = 0f, Maximum = 0f }
                };
                Defense = new Defense
                {
                    Smashing = new Smashing { Base = 0f, Current = 0f, Maximum = 0f },
                    Lethal = new Lethal { Base = 0f, Current = 0f, Maximum = 0f },
                    Fire = new Fire { Base = 0f, Current = 0f, Maximum = 0f },
                    Cold = new Cold { Base = 0f, Current = 0f, Maximum = 0f },
                    Energy = new Energy { Base = 0f, Current = 0f, Maximum = 0f },
                    Negative = new Negative { Base = 0f, Current = 0f, Maximum = 0f },
                    Psionic = new Psionic { Base = 0f, Current = 0f, Maximum = 0f },
                    Melee = new Melee { Base = 0f, Current = 0f, Maximum = 0f },
                    Ranged = new Ranged { Base = 0f, Current = 0f, Maximum = 0f },
                    Aoe = new Aoe { Base = 0f, Current = 0f, Maximum = 0f }
                };
                DefenseDebuff = new DefenseDebuff
                {
                    Smashing = new Smashing { Base = 0f, Current = 0f, Maximum = 0f },
                    Lethal = new Lethal { Base = 0f, Current = 0f, Maximum = 0f },
                    Fire = new Fire { Base = 0f, Current = 0f, Maximum = 0f },
                    Cold = new Cold { Base = 0f, Current = 0f, Maximum = 0f },
                    Energy = new Energy { Base = 0f, Current = 0f, Maximum = 0f },
                    Negative = new Negative { Base = 0f, Current = 0f, Maximum = 0f },
                    Psionic = new Psionic { Base = 0f, Current = 0f, Maximum = 0f },
                    Melee = new Melee { Base = 0f, Current = 0f, Maximum = 0f },
                    Ranged = new Ranged { Base = 0f, Current = 0f, Maximum = 0f },
                    Aoe = new Aoe { Base = 0f, Current = 0f, Maximum = 0f }
                };
                Elusivity = new Elusivity
                {
                    Smashing = new Smashing { Base = 0f, Current = 0f, Maximum = 0f },
                    Lethal = new Lethal { Base = 0f, Current = 0f, Maximum = 0f },
                    Fire = new Fire { Base = 0f, Current = 0f, Maximum = 0f },
                    Cold = new Cold { Base = 0f, Current = 0f, Maximum = 0f },
                    Energy = new Energy { Base = 0f, Current = 0f, Maximum = 0f },
                    Negative = new Negative { Base = 0f, Current = 0f, Maximum = 0f },
                    Psionic = new Psionic { Base = 0f, Current = 0f, Maximum = 0f },
                    Melee = new Melee { Base = 0f, Current = 0f, Maximum = 0f },
                    Ranged = new Ranged { Base = 0f, Current = 0f, Maximum = 0f },
                    Aoe = new Aoe { Base = 0f, Current = 0f, Maximum = 0f }
                };
                Resistance = new Resistance
                {
                    Smashing = new Smashing { Base = 0f, Current = 0f, Maximum = 0f },
                    Lethal = new Lethal { Base = 0f, Current = 0f, Maximum = 0f },
                    Fire = new Fire { Base = 0f, Current = 0f, Maximum = 0f },
                    Cold = new Cold { Base = 0f, Current = 0f, Maximum = 0f },
                    Energy = new Energy { Base = 0f, Current = 0f, Maximum = 0f },
                    Negative = new Negative { Base = 0f, Current = 0f, Maximum = 0f },
                    Psionic = new Psionic { Base = 0f, Current = 0f, Maximum = 0f },
                    Toxic = new Toxic { Base = 0f, Current = 0f, Maximum = 0f }
                };
                ResistanceDebuff = new ResistanceDebuff
                {
                    Smashing = new Smashing { Base = 0f, Current = 0f, Maximum = 0f },
                    Lethal = new Lethal { Base = 0f, Current = 0f, Maximum = 0f },
                    Fire = new Fire { Base = 0f, Current = 0f, Maximum = 0f },
                    Cold = new Cold { Base = 0f, Current = 0f, Maximum = 0f },
                    Energy = new Energy { Base = 0f, Current = 0f, Maximum = 0f },
                    Negative = new Negative { Base = 0f, Current = 0f, Maximum = 0f },
                    Psionic = new Psionic { Base = 0f, Current = 0f, Maximum = 0f },
                    Toxic = new Toxic { Base = 0f, Current = 0f, Maximum = 0f }
                };
                StatusProtection = new StatusProtection
                {
                    Confuse = new Confused { Base = 0f, Current = 0f, Maximum = 0f },
                    Fear = new Terrorized { Base = 0f, Current = 0f, Maximum = 0f },
                    Hold = new Held { Base = 0f, Current = 0f, Maximum = 0f },
                    Immobilize = new Immobilized { Base = 0f, Current = 0f, Maximum = 0f },
                    KnockBack = new KnockBack { Base = 0f, Current = 0f, Maximum = 0f },
                    KnockUp = new KnockUp { Base = 0f, Current = 0f, Maximum = 0f },
                    Placate = new Placate { Base = 0f, Current = 0f, Maximum = 0f },
                    Repel = new Repel { Base = 0f, Current = 0f, Maximum = 0f },
                    Sleep = new Sleep { Base = 0f, Current = 0f, Maximum = 0f },
                    Stun = new Stunned { Base = 0f, Current = 0f, Maximum = 0f },
                    Taunt = new Taunt { Base = 0f, Current = 0f, Maximum = 0f },
                    Teleport = new Teleport { Base = 0f, Current = 0f, Maximum = 0f }
                };
                StatusResistance = new StatusResistance
                {
                    Confuse = new Confused { Base = 0f, Current = 0f, Maximum = 0f },
                    Fear = new Terrorized { Base = 0f, Current = 0f, Maximum = 0f },
                    Hold = new Held { Base = 0f, Current = 0f, Maximum = 0f },
                    Immobilize = new Immobilized { Base = 0f, Current = 0f, Maximum = 0f },
                    KnockBack = new KnockBack { Base = 0f, Current = 0f, Maximum = 0f },
                    KnockUp = new KnockUp { Base = 0f, Current = 0f, Maximum = 0f },
                    Placate = new Placate { Base = 0f, Current = 0f, Maximum = 0f },
                    Repel = new Repel { Base = 0f, Current = 0f, Maximum = 0f },
                    Sleep = new Sleep { Base = 0f, Current = 0f, Maximum = 0f },
                    Stun = new Stunned { Base = 0f, Current = 0f, Maximum = 0f },
                    Taunt = new Taunt { Base = 0f, Current = 0f, Maximum = 0f },
                    Teleport = new Teleport { Base = 0f, Current = 0f, Maximum = 0f }
                };

                if (!noReset) return;
                Absorb = new Absorb { Base = 0f, Current = 0f, Maximum = 0f };
                Accuracy = new Accuracy { Base = 0f, Current = 0f, Maximum = 0f };
                DamageBuff = new DamageBuff { Base = 0f, Current = 0f, Maximum = 0f };

                Endurance = new Endurance { Base = 0f, Current = 0f, Maximum = 0f };
                EnduranceReduction = 0f;
                EnduranceUsage = 0f;
                HitPoints = new HitPoints { Base = 0f, Current = 0f, Maximum = 0f };
                Movement = new Movement
                {
                    FlySpeed = new FlySpeed { Base = 0f, Current = 0f, Maximum = 0f },
                    JumpHeight = new JumpHeight { Base = 0f, Current = 0f, Maximum = 0f },
                    JumpSpeed = new JumpSpeed { Base = 0f, Current = 0f, Maximum = 0f },
                    RunSpeed = new RunSpeed { Base = 0f, Current = 0f, Maximum = 0f }
                };
                Perception = new Perception { Base = 0f, Current = 0f, Maximum = 0f };
                Recharge = new Recharge { Base = 0f, Current = 0f, Maximum = 0f };
                Recovery = new Recovery { Base = 0f, Current = 0f, Maximum = 0f };
                Regeneration = new Regeneration { Base = 0f, Current = 0f, Maximum = 0f };
                StealthRadius = new StealthRadius
                {
                    PvE = new StealthRadiusPve { Base = 0f, Current = 0f, Maximum = 0f },
                    PvP = new StealthRadiusPvp { Base = 0f, Current = 0f, Maximum = 0f }
                };
                Threat = new Threat { Base = 0f, Current = 0f, Maximum = 0f };
                ToHit = new ToHit { Base = 0f, Current = 0f, Maximum = 0f };
            }

        }

        public class Absorb
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Accuracy
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class DamageBuff
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class DebuffResistance
        {
            public Defense Defense { get; set; }
            public Endurance Endurance { get; set; }
            public Perception Perception { get; set; }
            public Recharge Recharge { get; set; }
            public Recovery Recovery { get; set; }
            public Regeneration Regeneration { get; set; }
            public RunSpeed RunSpeed { get; set; }
            public ToHit ToHit { get; set; }
        }

        public class Defense
        {
            public Smashing Smashing { get; set; }
            public Lethal Lethal { get; set; }
            public Fire Fire { get; set; }
            public Cold Cold { get; set; }
            public Energy Energy { get; set; }
            public Negative Negative { get; set; }
            public Psionic Psionic { get; set; }
            public Toxic Toxic { get; set; }
            public Melee Melee { get; set; }
            public Ranged Ranged { get; set; }
            public Aoe Aoe { get; set; }
        }

        public class DefenseDebuff
        {
            public Smashing Smashing { get; set; }
            public Lethal Lethal { get; set; }
            public Fire Fire { get; set; }
            public Cold Cold { get; set; }
            public Energy Energy { get; set; }
            public Negative Negative { get; set; }
            public Psionic Psionic { get; set; }
            public Toxic Toxic { get; set; }
            public Melee Melee { get; set; }
            public Ranged Ranged { get; set; }
            public Aoe Aoe { get; set; }
        }

        public class Elusivity
        {
            public Smashing Smashing { get; set; }
            public Lethal Lethal { get; set; }
            public Fire Fire { get; set; }
            public Cold Cold { get; set; }
            public Energy Energy { get; set; }
            public Negative Negative { get; set; }
            public Psionic Psionic { get; set; }
            public Toxic Toxic { get; set; }
            public Melee Melee { get; set; }
            public Ranged Ranged { get; set; }
            public Aoe Aoe { get; set; }
        }

        public class Endurance
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class HitPoints
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Movement
        {
            public FlySpeed FlySpeed { get; set; }
            public JumpHeight JumpHeight { get; set; }
            public JumpSpeed JumpSpeed { get; set; }
            public RunSpeed RunSpeed { get; set; }
        }

        public class Perception
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Recharge
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Recovery
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Regeneration
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Resistance
        {
            public Smashing Smashing { get; set; }
            public Lethal Lethal { get; set; }
            public Fire Fire { get; set; }
            public Cold Cold { get; set; }
            public Energy Energy { get; set; }
            public Negative Negative { get; set; }
            public Psionic Psionic { get; set; }
            public Toxic Toxic { get; set; }
        }

        public class ResistanceDebuff
        {
            public Smashing Smashing { get; set; }
            public Lethal Lethal { get; set; }
            public Fire Fire { get; set; }
            public Cold Cold { get; set; }
            public Energy Energy { get; set; }
            public Negative Negative { get; set; }
            public Psionic Psionic { get; set; }
            public Toxic Toxic { get; set; }
        }

        public class StatusProtection
        {
            public Confused Confuse { get; set; }
            public Terrorized Fear { get; set; }
            public Held Hold { get; set; }
            public Immobilized Immobilize { get; set; }
            public KnockBack KnockBack { get; set; }
            public KnockUp KnockUp { get; set; }
            public Placate Placate { get; set; }
            public Repel Repel { get; set; }
            public Sleep Sleep { get; set; }
            public Stunned Stun { get; set; }
            public Taunt Taunt { get; set; }
            public Teleport Teleport { get; set; }
        }

        public class StatusResistance
        {
            public Confused Confuse { get; set; }
            public Terrorized Fear { get; set; }
            public Held Hold { get; set; }
            public Immobilized Immobilize { get; set; }
            public KnockBack KnockBack { get; set; }
            public KnockUp KnockUp { get; set; }
            public Placate Placate { get; set; }
            public Repel Repel { get; set; }
            public Sleep Sleep { get; set; }
            public Stunned Stun { get; set; }
            public Taunt Taunt { get; set; }
            public Teleport Teleport { get; set; }
        }

        public class StealthRadius
        {
            public StealthRadiusPve PvE { get; set; }
            public StealthRadiusPvp PvP { get; set; }
        }

        public class Threat
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class ToHit
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        #region Substats

        #region DamageTypes

        public class Smashing
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Lethal
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Fire
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Cold
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Energy
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Negative
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Psionic
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Toxic
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Melee
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Ranged
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Aoe
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        #endregion

        #region MovementTypes

        public class FlySpeed
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class JumpHeight
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class JumpSpeed
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class RunSpeed
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        #endregion

        #region StealthTypes

        public class StealthRadiusPve
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class StealthRadiusPvp
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        #endregion

        #region MezTypes

        public class Confused
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Held
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Immobilized
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class KnockBack
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class KnockUp
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Placate
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Repel
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Sleep
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Stunned
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Taunt
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Teleport
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        public class Terrorized
        {
            public float Base { get; set; }
            public float Current { get; set; }
            public float Maximum { get; set; }
        }

        #endregion

        #endregion
    }
}
