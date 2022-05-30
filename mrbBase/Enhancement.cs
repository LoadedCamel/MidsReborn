using System;
using System.IO;
using mrbBase.Base.Data_Classes;

namespace mrbBase
{
    public class Enhancement : IEnhancement
    {
        private IPower? _power;

        public Enhancement()
        {
            UID = string.Empty;
            Name = "New Enhancement";
            ShortName = "NewEnh";
            Desc = string.Empty;
            TypeID = Enums.eType.Normal;
            //SubTypeID = Enums.eSubtype.None;
            SubTypeID = 0;
            Image = string.Empty;
            nIDSet = -1;
            EffectChance = 1f;
            LevelMax = 52;
            UIDSet = string.Empty;
            _power = new Power
            {
                PowerType = Enums.ePowerType.Boost,
                DisplayName = Name,
                FullName = UID
            };
            BuffMode = Enums.eBuffDebuff.Any;
            ClassID = Array.Empty<int>();
            Effect = Array.Empty<Enums.sEffect>();
            RecipeName = string.Empty;
            RecipeIDX = -1;
            UID = string.Empty;
            IsProc = false;
            IsScalable = false;
        }

        public Enhancement(IEnhancement iEnh)
        {
            StaticIndex = iEnh.StaticIndex;
            Name = iEnh.Name;
            ShortName = iEnh.ShortName;
            Desc = iEnh.Desc;
            TypeID = iEnh.TypeID;
            SubTypeID = iEnh.SubTypeID;
            Image = iEnh.Image;
            ImageIdx = iEnh.ImageIdx;
            nIDSet = iEnh.nIDSet;
            UIDSet = iEnh.UIDSet;
            EffectChance = iEnh.EffectChance;
            LevelMin = iEnh.LevelMin;
            LevelMax = iEnh.LevelMax;
            Unique = iEnh.Unique;
            MutExID = iEnh.MutExID;
            BuffMode = iEnh.BuffMode;
            _power = new Power(iEnh.GetPower());
            ClassID = new int[iEnh.ClassID.Length];
            for (var index = 0; index <= ClassID.Length - 1; ++index)
                ClassID[index] = iEnh.ClassID[index];
            Effect = new Enums.sEffect[iEnh.Effect.Length];
            for (var index = 0; index <= Effect.Length - 1; ++index)
            {
                Effect[index].Mode = iEnh.Effect[index].Mode;
                Effect[index].BuffMode = iEnh.Effect[index].BuffMode;
                Effect[index].Enhance.ID = iEnh.Effect[index].Enhance.ID;
                Effect[index].Enhance.SubID = iEnh.Effect[index].Enhance.SubID;
                Effect[index].Schedule = iEnh.Effect[index].Schedule;
                Effect[index].Multiplier = iEnh.Effect[index].Multiplier;
                if (iEnh.Effect[index].FX != null)
                    Effect[index].FX = iEnh.Effect[index].FX.Clone() as IEffect;
            }

            UID = iEnh.UID;
            IsProc = iEnh.IsProc;
            IsScalable = iEnh.IsScalable;
            RecipeName = iEnh.RecipeName;
            RecipeIDX = iEnh.RecipeIDX;
            Superior = iEnh.Superior;
        }

        public Enhancement(BinaryReader reader)
        {
            RecipeIDX = -1;
            IsModified = false;
            IsNew = false;
            StaticIndex = reader.ReadInt32();
            Name = reader.ReadString();
            ShortName = reader.ReadString();
            Desc = reader.ReadString();
            TypeID = (Enums.eType)reader.ReadInt32();
            //SubTypeID = (Enums.eSubtype)reader.ReadInt32();
            SubTypeID = reader.ReadInt32();
            ClassID = new int[reader.ReadInt32() + 1];
            for (var index = 0; index < ClassID.Length; ++index)
                ClassID[index] = reader.ReadInt32();
            Image = reader.ReadString();
            nIDSet = reader.ReadInt32();
            UIDSet = reader.ReadString();
            EffectChance = reader.ReadSingle();
            LevelMin = reader.ReadInt32();
            LevelMax = reader.ReadInt32();
            Unique = reader.ReadBoolean();
            MutExID = (Enums.eEnhMutex)reader.ReadInt32();
            BuffMode = (Enums.eBuffDebuff)reader.ReadInt32();
            if (MutExID < Enums.eEnhMutex.None)
                MutExID = Enums.eEnhMutex.None;
            Effect = new Enums.sEffect[reader.ReadInt32() + 1];
            for (var index = 0; index <= Effect.Length - 1; ++index)
            {
                Effect[index].Mode = (Enums.eEffMode)reader.ReadInt32();
                Effect[index].BuffMode = (Enums.eBuffDebuff)reader.ReadInt32();
                Effect[index].Enhance.ID = reader.ReadInt32();
                Effect[index].Enhance.SubID = reader.ReadInt32();
                Effect[index].Schedule = (Enums.eSchedule)reader.ReadInt32();
                Effect[index].Multiplier = reader.ReadSingle();
                ref var local = ref Effect[index];
                Effect effect;
                if (!reader.ReadBoolean())
                {
                    effect = null;
                }
                else
                {
                    effect = new Effect(reader)
                    {
                        isEnhancementEffect = true,
                    };
                }
                local.FX = effect;
            }

            UID = reader.ReadString();
            RecipeName = reader.ReadString();
            Superior = reader.ReadBoolean();
            IsProc = reader.ReadBoolean();
            IsScalable = reader.ReadBoolean();
        }

        public bool IsModified { get; set; }

        public bool IsNew { get; set; }

        public int StaticIndex { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string Desc { get; set; }

        public Enums.eType TypeID { get; set; }

        //public Enums.eSubtype SubTypeID { get; set; }

        public int SubTypeID { get; set; }

        public int[] ClassID { get; set; }

        public string Image { get; set; }

        public int ImageIdx { get; set; }

        public int nIDSet { get; set; }

        public string UIDSet { get; set; }

        public bool IsProc { get; set; }

        public bool IsScalable { get; set; }

        public IPower? GetPower()
        {
            return _power ??= DatabaseAPI.GetPowerByFullName("Boosts." + UID + "." + UID);
        }

        void IEnhancement.SetPower(IPower? power)
        {
            _power = power;
        }

        //public IPower Power
        //{
        //    get
        //    {
        //    }
        //    set
        //    {
        //        this._power = value;
        //    }
        //}

        public Enums.sEffect[] Effect { get; set; }

        public float EffectChance { get; set; }

        public int LevelMin { get; set; }

        public int LevelMax { get; set; }

        public bool Unique { get; set; }

        public Enums.eEnhMutex MutExID { get; set; }

        public Enums.eBuffDebuff BuffMode { get; set; }

        public string RecipeName { get; set; }

        public int RecipeIDX { get; set; }

        public string UID { get; set; }

        public bool Superior { get; set; }

        public float Probability
        {
            get
            {
                for (var index = 0; index <= Effect.Length - 1; ++index)
                    if (Effect[index].Mode == Enums.eEffMode.FX)
                        return Effect[index].FX.Probability;
                return 0.0f;
            }
        }

        public bool HasEnhEffect
        {
            get
            {
                for (var index = 0; index <= Effect.Length - 1; ++index)
                    if (Effect[index].Mode == Enums.eEffMode.Enhancement)
                        return true;
                return false;
            }
        }

        public bool HasPowerEffect
        {
            get
            {
                for (var index = 0; index <= Effect.Length - 1; ++index)
                    if (Effect[index].Mode == Enums.eEffMode.FX)
                        return true;
                return false;
            }
        }

        public string LongName
        {
            get
            {
                var str = TypeID switch
                {
                    Enums.eType.Normal => Name,
                    Enums.eType.SpecialO => Name,
                    Enums.eType.InventO => "Invention: " + Name,
                    Enums.eType.SetO => DatabaseAPI.Database.EnhancementSets[nIDSet].DisplayName + ": " + Name,
                    _ => string.Empty
                };
                return str;
            }
        }

        public Enums.eSchedule Schedule
        {
            get
            {
                Enums.eSchedule eSchedule;
                switch (Effect.Length)
                {
                    case 1:
                        eSchedule = Effect[0].Schedule;
                        break;
                    case 0:
                        eSchedule = Enums.eSchedule.None;
                        break;
                    default:
                    {
                        var schedule = Effect[0].Schedule;
                        var flag = false;
                        for (var index = 0; index <= Effect.Length - 1; ++index)
                            if (Effect[index].Schedule != schedule)
                                flag = true;
                        eSchedule = !flag ? schedule : Enums.eSchedule.Multiple;
                        break;
                    }
                }

                return eSchedule;
            }
        }

        public void StoreTo(BinaryWriter writer)
        {
            writer.Write(StaticIndex);
            writer.Write(Name);
            writer.Write(ShortName);
            writer.Write(Desc);
            writer.Write((int) TypeID);
            //writer.Write((int) SubTypeID);
            writer.Write(SubTypeID);
            writer.Write(ClassID.Length - 1);
            for (var index = 0; index <= ClassID.Length - 1; ++index)
                writer.Write(ClassID[index]);
            writer.Write(Image);
            writer.Write(nIDSet);
            writer.Write(UIDSet);
            writer.Write(EffectChance);
            writer.Write(LevelMin);
            writer.Write(LevelMax);
            writer.Write(Unique);
            writer.Write((int) MutExID);
            writer.Write((int) BuffMode);
            writer.Write(Effect.Length - 1);
            for (var index = 0; index <= Effect.Length - 1; ++index)
            {
                writer.Write((int) Effect[index].Mode);
                writer.Write((int) Effect[index].BuffMode);
                writer.Write(Effect[index].Enhance.ID);
                writer.Write(Effect[index].Enhance.SubID);
                writer.Write((int) Effect[index].Schedule);
                writer.Write(Effect[index].Multiplier);
                if (Effect[index].FX == null)
                {
                    writer.Write(false);
                }
                else
                {
                    writer.Write(true);
                    Effect[index].FX.StoreTo(ref writer);
                }
            }

            writer.Write(UID);
            writer.Write(RecipeName);
            writer.Write(Superior);
            writer.Write(IsProc);
            writer.Write(IsScalable);
        }

        public int CheckAndFixIOLevel(int level)
        {
            if (TypeID != Enums.eType.InventO && TypeID != Enums.eType.SetO)
                return level - 1;

            var iMax = 52;
            var iMin = 9;
            switch (TypeID)
            {
                case Enums.eType.InventO:
                    iMax = LevelMax;
                    iMin = LevelMin;
                    break;
                case Enums.eType.SetO:
                    if (nIDSet > -1)
                    {
                        iMax = DatabaseAPI.Database.EnhancementSets[nIDSet].LevelMax;
                        iMin = DatabaseAPI.Database.EnhancementSets[nIDSet].LevelMin;
                    }

                    break;
            }

            if (level > iMax)
                level = iMax;
            if (level < iMin)
                level = iMin;
            if (TypeID != Enums.eType.InventO)
                return level;
            if (iMax > 49)
                iMax = 49;
            level = GranularLevelZb(level, iMin, iMax);
            return level;
        }

        public string GetSpecialName()
        {
            return DatabaseAPI.GetSpecialEnhByIndex(SubTypeID).ShortName;
            //return $"{Enum.GetName(typeof(Enums.eSubtype), (int) SubTypeID)} Origin";
        }

        public static int GranularLevelZb(int iLevel, int iMin, int iMax, int iStep = 5)
        {
            ++iMin;
            ++iMax;
            ++iLevel;
            var nStep = Convert.ToSingle(iStep);
            var nLevel = Convert.ToSingle(iLevel);
            var midway = nStep / 2f;
            var extra = iLevel % nStep;
            if (extra > 0.0)
                iLevel = extra >= (double) midway
                    ? (int) (Math.Floor(nLevel / (double) nStep) * nStep + nStep)
                    : (int) (Math.Floor(nLevel / (double) nStep) * nStep);
            if (iLevel > iMax)
                iLevel = iMax;
            if (iLevel < iMin)
                iLevel = iMin;
            return iLevel - 1;
        }

        public static Enums.eSchedule GetSchedule(Enums.eEnhance iEnh, int tSub = -1)
        {
            Enums.eSchedule eSchedule;
            switch (iEnh)
            {
                case Enums.eEnhance.Defense:
                    eSchedule = Enums.eSchedule.B;
                    break;
                case Enums.eEnhance.Interrupt:
                    return Enums.eSchedule.C;
                case Enums.eEnhance.Mez:
                    return tSub <= -1 || !((tSub == 4) | (tSub == 5)) ? Enums.eSchedule.A : Enums.eSchedule.D;
                case Enums.eEnhance.Range:
                    return Enums.eSchedule.B;
                case Enums.eEnhance.Resistance:
                    return Enums.eSchedule.B;
                case Enums.eEnhance.ToHit:
                    return Enums.eSchedule.B;
                default:
                    eSchedule = Enums.eSchedule.A;
                    break;
            }

            return eSchedule;
        }

        public static float ApplyED(Enums.eSchedule iSched, float iVal)
        {
            switch (iSched)
            {
                case Enums.eSchedule.None:
                    return 0.0f;
                case Enums.eSchedule.Multiple:
                    return 0.0f;
                default:
                    var ed = new float[3];
                    for (var index = 0; index <= 2; ++index)
                        ed[index] = DatabaseAPI.Database.MultED[(int) iSched][index];
                    if (iVal <= (double) ed[0])
                        return iVal;
                    float[] edm =
                    {
                        ed[0],
                        ed[0] + (float) ((ed[1] - (double) ed[0]) * 0.899999976158142),
                        (float) (ed[0] + (ed[1] - (double) ed[0]) * 0.899999976158142 +
                                 (ed[2] - (double) ed[1]) * 0.699999988079071)
                    };
                    return iVal > (double) ed[1]
                        ? iVal > (double) ed[2] ? edm[2] + (float) ((iVal - (double) ed[2]) * 0.150000005960464) :
                        edm[1] + (float) ((iVal - (double) ed[1]) * 0.699999988079071)
                        : edm[0] + (float) ((iVal - (double) ed[0]) * 0.899999976158142);
            }
        }
    }
}