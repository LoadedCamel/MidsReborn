using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using mrbBase.Base.Display;

namespace mrbBase.Base.Data_Classes
{
    public class Archetype : IComparable
    {
        public bool IsModified;
        public bool IsNew;
        public Image Image(List<string> atImages)
        {
            var img = atImages.FirstOrDefault(i => i.Contains(ClassName));
            return System.Drawing.Image.FromFile(img ?? throw new InvalidOperationException());
        }

        public Archetype()
        {
            BaseThreat = 1f;
            BaseRegen = 1f;
            BaseRecovery = 1.67f;
            Playable = true;
            PoolGroup = "POOL";
            EpicGroup = "EPIC";
            SecondaryGroup = string.Empty;
            PrimaryGroup = string.Empty;
            ClassName = string.Empty;
            PerceptionCap = 1153f;
            Ancillary = Array.Empty<int>();
            Secondary = Array.Empty<int>();
            Primary = Array.Empty<int>();
            Origin = Array.Empty<string>();
            RecoveryCap = 5f;
            RegenCap = 20f;
            DamageCap = 4f;
            RechargeCap = 5f;
            ResCap = 90f;
            DescShort = string.Empty;
            DescLong = string.Empty;
            HPCap = 5000f;
            Hitpoints = 5000;
            ClassType = Enums.eClassType.None;
            DisplayName = string.Empty;
            Origin = new[]
            {
                "Magic",
                "Mutation",
                "Natural",
                "Science",
                "Technology"
            };
            DisplayName = "New Archetype";
            ClassName = "NewClass";
        }

        public Archetype(Archetype template) : this()
        {
            Idx = template.Idx;
            DisplayName = template.DisplayName;
            HPCap = template.HPCap;
            Hitpoints = template.Hitpoints;
            DescLong = template.DescLong;
            DescShort = template.DescShort;
            ResCap = template.ResCap;
            Origin = (string[]) template.Origin.Clone();
            Primary = (int[]) template.Primary.Clone();
            Secondary = (int[]) template.Secondary.Clone();
            Ancillary = (int[]) template.Ancillary.Clone();
            ClassName = template.ClassName;
            ClassType = template.ClassType;
            Column = template.Column;
            PrimaryGroup = template.PrimaryGroup;
            SecondaryGroup = template.SecondaryGroup;
            Playable = template.Playable;
            RechargeCap = template.RechargeCap;
            DamageCap = template.DamageCap;
            RecoveryCap = template.RecoveryCap;
            RegenCap = template.RegenCap;
            BaseRecovery = template.BaseRecovery;
            BaseRegen = template.BaseRegen;
            BaseThreat = template.BaseThreat;
            PerceptionCap = template.PerceptionCap;
        }

        public Archetype(BinaryReader reader) : this()
        {
            DisplayName = reader.ReadString();
            Hitpoints = reader.ReadInt32();
            HPCap = reader.ReadSingle();
            DescLong = reader.ReadString();
            ResCap = reader.ReadSingle();
            var num = reader.ReadInt32();
            Origin = new string[num + 1];
            for (var index = 0; index <= num; ++index)
                Origin[index] = reader.ReadString();
            ClassName = reader.ReadString();
            ClassType = (Enums.eClassType) reader.ReadInt32();
            Column = reader.ReadInt32();
            DescShort = reader.ReadString();
            PrimaryGroup = reader.ReadString();
            SecondaryGroup = reader.ReadString();
            Playable = reader.ReadBoolean();
            RechargeCap = reader.ReadSingle();
            DamageCap = reader.ReadSingle();
            RecoveryCap = reader.ReadSingle();
            RegenCap = reader.ReadSingle();
            BaseRecovery = reader.ReadSingle();
            BaseRegen = reader.ReadSingle();
            BaseThreat = reader.ReadSingle();
            PerceptionCap = reader.ReadSingle();
        }

        public int Idx { get; set; }

        public string DisplayName { get; set; }

        public Enums.eClassType ClassType { get; set; }

        public int Hitpoints { get; set; }

        public float HPCap { get; set; }

        public string DescLong { get; set; }

        public string DescShort { get; set; }

        public float ResCap { get; set; }

        public float RechargeCap { get; set; }

        public float DamageCap { get; set; }

        public float RegenCap { get; set; }

        public float RecoveryCap { get; set; }

        public string[] Origin { get; set; }

        public int[] Primary { get; set; }

        public int[] Secondary { get; set; }

        public int[] Ancillary { get; set; }

        public float PerceptionCap { get; set; }

        public string ClassName { get; set; }

        public int Column { get; set; }

        public string PrimaryGroup { get; set; }

        public string SecondaryGroup { get; set; }

        public string EpicGroup { get; }

        private string PoolGroup { get; set; }

        public bool Playable { get; set; }

        public bool Pet { get; set; }

        public float BaseRecovery { get; set; }

        public float BaseRegen { get; set; }

        public float BaseThreat { get; set; }

        public bool Hero => ClassType == Enums.eClassType.Hero || ClassType == Enums.eClassType.HeroEpic;

        public bool Epic => ClassType == Enums.eClassType.HeroEpic || ClassType == Enums.eClassType.VillainEpic;

        public int CompareTo(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            if (!(obj is Archetype archetype))
                throw new ArgumentException("Comparison failed - Passed object was not an Archetype Class!");
            if (Playable & !archetype.Playable)
                return -1;
            if (!Playable & archetype.Playable)
                return 1;
            if ((ClassType == Enums.eClassType.None) & (archetype.ClassType != Enums.eClassType.None))
                return 1;
            if ((ClassType != Enums.eClassType.None) & (archetype.ClassType == Enums.eClassType.None))
                return -1;
            if (ClassType > archetype.ClassType)
                return 1;
            if (ClassType < archetype.ClassType)
                return -1;
            var classNameMatch = string.Compare(ClassName, archetype.ClassName, StringComparison.OrdinalIgnoreCase);
            if (classNameMatch != 0) return classNameMatch;
            if (Column > archetype.Column) return 1;
            return Column < archetype.Column ? 1 : 0;
        }

        public void StoreTo(ref BinaryWriter writer)
        {
            writer.Write(DisplayName);
            writer.Write(Hitpoints);
            writer.Write(HPCap);
            writer.Write(DescLong);
            writer.Write(ResCap);
            writer.Write(Origin.Length - 1);
            foreach (var index in Origin)
                writer.Write(index);

            writer.Write(ClassName);
            writer.Write((int) ClassType);
            writer.Write(Column);
            writer.Write(DescShort);
            writer.Write(PrimaryGroup);
            writer.Write(SecondaryGroup);
            writer.Write(Playable);
            writer.Write(RechargeCap);
            writer.Write(DamageCap);
            writer.Write(RecoveryCap);
            writer.Write(RegenCap);
            writer.Write(BaseRecovery);
            writer.Write(BaseRegen);
            writer.Write(BaseThreat);
            writer.Write(PerceptionCap);
        }

        public PopUp.PopupData PopInfo()
        {
            var popupData = new PopUp.PopupData();
            var index1 = popupData.Add();
            popupData.Sections[index1].Add(DisplayName, PopUp.Colors.Title, 1.25f);
            popupData.Sections[index1].Add(DescShort, PopUp.Colors.Effect);
            popupData.Sections[index1].Add(DescLong, PopUp.Colors.Text, 0.9f, FontStyle.Bold, 1);
            var index2 = popupData.Add();
            popupData.Sections[index2]
                .Add(
                    "You can't change archetype once a build has been started.\nIf you want to pick a different archetype, you need to clear the current build and start a new one.",
                    PopUp.Colors.Effect, 0.9f);
            return popupData;
        }

        public bool UpdateFromCSV(string csv)
        {
            if (string.IsNullOrEmpty(csv))
                return false;

            var array = CSV.ToArray(csv);
            if (array.Length < 11)
                return false;

            ClassName = array[0];
            Column = int.Parse(array[1]) - 2;
            DisplayName = array[2];
            DescLong = array[3];
            Origin = array[4].Split(Convert.ToChar(" "));
            var str = array[5];
            if (str.IndexOf("KHELDIAN HERO", StringComparison.OrdinalIgnoreCase) > -1)
                ClassType = Enums.eClassType.HeroEpic;
            else if (str.IndexOf("ARACHNOSSOLDIER VILLAIN", StringComparison.OrdinalIgnoreCase) > -1 ||
                     str.IndexOf("ARACHNOSWIDOW VILLAIN", StringComparison.OrdinalIgnoreCase) > -1)
                ClassType = Enums.eClassType.VillainEpic;
            else if (str.IndexOf("HERO", StringComparison.OrdinalIgnoreCase) > -1)
                ClassType = Enums.eClassType.Hero;
            else if (str.IndexOf("VILLAIN", StringComparison.OrdinalIgnoreCase) > -1)
                ClassType = Enums.eClassType.Villain;
            Playable = !string.IsNullOrWhiteSpace(str);
            DescShort = array[6];
            PrimaryGroup = array[8];
            SecondaryGroup = array[9];
            PoolGroup = array[10];
            return true;
        }
    }
}