using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mids_Reborn.Core
{
    public class Powerset : IPowerset
    {
        private string _fullName;

        private string _groupName;
        private PowersetGroup _powersetGroup;

        public Powerset()
        {
            nID = -1;
            nArchetype = -1;
            DisplayName = "New Powerset";
            SetType = Enums.ePowerSetType.None;
            ImageName = string.Empty;
            FullName = string.Empty;
            SetName = string.Empty;
            Description = string.Empty;
            SubName = string.Empty;
            ATClass = string.Empty;
            UIDTrunkSet = string.Empty;
            nIDTrunkSet = -1;
            nIDLinkSecondary = -1;
            UIDLinkSecondary = string.Empty;
            Powers = new IPower[0];
            Power = new int[0];
            nIDMutexSets = new int[0];
            UIDMutexSets = new string[0];
        }

        public Powerset(IPowerset? template)
        {
            nID = template.nID;
            nArchetype = template.nArchetype;
            DisplayName = template.DisplayName;
            SetType = template.SetType;
            ImageName = template.ImageName;
            FullName = template.FullName;
            SetName = template.SetName;
            Description = template.Description;
            SubName = template.SubName;
            ATClass = template.ATClass;
            _powersetGroup = template.GetGroup();
            GroupName = template.GroupName;
            UIDTrunkSet = template.UIDTrunkSet;
            nIDTrunkSet = template.nIDTrunkSet;
            nIDLinkSecondary = template.nIDLinkSecondary;
            UIDLinkSecondary = template.UIDLinkSecondary;
            Powers = new IPower[template.Powers.Length];
            Power = new int[template.Power.Length];
            Array.Copy(template.Power, Power, Power.Length);
            for (var index = 0; index < Powers.Length; ++index)
                Powers[index] = template.Powers[index];
            nIDMutexSets = new int[template.nIDMutexSets.Length];
            UIDMutexSets = new string[template.UIDMutexSets.Length];
            Array.Copy(template.nIDMutexSets, nIDMutexSets, nIDMutexSets.Length);
            Array.Copy(template.UIDMutexSets, UIDMutexSets, UIDMutexSets.Length);
        }

        public Powerset(BinaryReader reader)
        {
            nID = -1;
            Powers = Array.Empty<IPower>();
            DisplayName = reader.ReadString();
            nArchetype = reader.ReadInt32();
            SetType = (Enums.ePowerSetType) reader.ReadInt32();
            ImageName = reader.ReadString();
            FullName = reader.ReadString();
            if (string.IsNullOrEmpty(FullName))
            {
                FullName = "Orphan." + DisplayName.Replace(" ", "_");
            }

            SetName = reader.ReadString();
            Description = reader.ReadString();
            SubName = reader.ReadString();
            ATClass = reader.ReadString();
            UIDTrunkSet = reader.ReadString();
            UIDLinkSecondary = reader.ReadString();
            var num = reader.ReadInt32();
            UIDMutexSets = new string[num + 1];
            nIDMutexSets = new int[num + 1];
            for (var index = 0; index <= num; ++index)
            {
                UIDMutexSets[index] = reader.ReadString();
                nIDMutexSets[index] = reader.ReadInt32();
            }
        }


        public bool IsModified { get; set; }

        public bool IsNew { get; set; }

        public string Description { get; set; }

        public string SetName { get; set; }

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                _groupName = null;
            }
        }

        public string ImageName { get; set; }

        public int[] Power { get; set; }

        public Enums.ePowerSetType SetType { get; set; }

        public string DisplayName { get; set; }

        public int nArchetype { get; set; }

        public int nID { get; set; }

        public string SubName { get; set; }

        public string ATClass { get; set; }

        public string UIDTrunkSet { get; set; }

        public int nIDTrunkSet { get; set; }

        public string UIDLinkSecondary { get; set; }

        public int nIDLinkSecondary { get; set; }

        public string[] UIDMutexSets { get; set; }

        public int[] nIDMutexSets { get; set; }

        public PowersetGroup GetGroup()
        {
            return _powersetGroup;
        }

        public void SetGroup(PowersetGroup pg)
        {
            _powersetGroup = pg;
        }

        public string GroupName
        {
            get
            {
                string str;
                if ((str = _groupName) == null)
                {
                    str = _groupName = FullName.Contains(".")
                        ? FullName.Substring(0, FullName.IndexOf(".", StringComparison.Ordinal))
                        : string.Empty;
                }

                return str;
            }
            set => _groupName = value;
        }

        public IPower?[] Powers { get; set; }

        public bool ClassOk(int nIDClass)
        {
            return Powers.Length > 0 && Powers[0].Requires.ClassOk(nIDClass);
        }

        public List<string> GetArchetypes()
        {
            if (!string.IsNullOrEmpty(ATClass)) return new List<string> { ATClass };
            if (Powers.Length <= 0) return new List<string>();

            return Powers[0].Requires.ClassName.ToList();
        }

        public void StoreTo(ref BinaryWriter writer)
        {
            writer.Write(DisplayName);
            writer.Write(nArchetype);
            writer.Write((int) SetType);
            writer.Write(ImageName);
            writer.Write(FullName);
            writer.Write(SetName);
            writer.Write(Description);
            writer.Write(SubName);
            writer.Write(ATClass);
            writer.Write(UIDTrunkSet);
            writer.Write(UIDLinkSecondary);
            writer.Write(UIDMutexSets.Length - 1);
            for (var index = 0; index < UIDMutexSets.Length; ++index)
            {
                writer.Write(UIDMutexSets[index]);
                writer.Write(nIDMutexSets[index]);
            }
        }

        public int CompareTo(object obj)
        {
            if (!(obj is IPowerset powerset))
                throw new ArgumentException("Comparison failed - Passed object was not a Powerset Class!");
            var num = string.Compare(GroupName, powerset.GroupName, StringComparison.OrdinalIgnoreCase);
            if (num == 0)
                num = string.Compare(DisplayName, powerset.DisplayName, StringComparison.OrdinalIgnoreCase);
            return num;
        }
    }
}