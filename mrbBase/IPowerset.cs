using System;
using System.Collections.Generic;
using System.IO;

namespace mrbBase
{
    public interface IPowerset : IComparable
    {
        bool IsModified { get; set; }

        bool IsNew { get; set; }

        int nID { get; set; }

        int nArchetype { get; set; }

        string DisplayName { get; set; }

        Enums.ePowerSetType SetType { get; set; }

        int[] Power { get; set; }

        IPower?[] Powers { get; set; }

        string ImageName { get; set; }

        string FullName { get; set; }

        string SetName { get; set; }

        string Description { get; set; }

        string SubName { get; set; }

        string ATClass { get; set; }

        string UIDTrunkSet { get; set; }

        int nIDTrunkSet { get; set; }

        string UIDLinkSecondary { get; set; }

        int nIDLinkSecondary { get; set; }

        string[] UIDMutexSets { get; set; }

        int[] nIDMutexSets { get; set; }

        string GroupName { get; set; }

        // not a property, is a reference loop/cycle.
        PowersetGroup GetGroup();
        void SetGroup(PowersetGroup group);

        bool ClassOk(int nIDClass);

        public List<string> GetArchetypes();

        void StoreTo(ref BinaryWriter writer);

        bool ImportFromCSV(string csv);
    }
}