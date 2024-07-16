using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core
{
    public class Requirement
    {
        public string[] ClassName = new string[0];
        public string[] ClassNameNot = new string[0];
        public int[] NClassName = new int[0];
        public int[] NClassNameNot = new int[0];
        public int[][] NPowerID = new int[0][];
        public int[][] NPowerIDNot = new int[0][];
        public string[][] PowerID = new string[0][];
        public string[][] PowerIDNot = new string[0][];

        public Requirement()
        {
        }

        public Requirement(Requirement iReq)
        {
            ClassName = new string[iReq.ClassName.Length];
            Array.Copy(iReq.ClassName, ClassName, iReq.ClassName.Length);
            ClassNameNot = new string[iReq.ClassNameNot.Length];
            Array.Copy(iReq.ClassNameNot, ClassNameNot, iReq.ClassNameNot.Length);
            NClassName = new int[iReq.NClassName.Length];
            Array.Copy(iReq.NClassName, NClassName, iReq.NClassName.Length);
            NClassNameNot = new int[iReq.NClassNameNot.Length];
            Array.Copy(iReq.NClassNameNot, NClassNameNot, iReq.NClassNameNot.Length);
            PowerID = new string[iReq.PowerID.Length][];
            for (var index = 0; index < PowerID.Length; index++)
            {
                PowerID[index] = new string[2];
                PowerID[index][0] = iReq.PowerID[index][0];
                PowerID[index][1] = iReq.PowerID[index][1];
            }

            PowerIDNot = new string[iReq.PowerIDNot.Length][];
            for (var index = 0; index < PowerIDNot.Length; index++)
            {
                PowerIDNot[index] = new string[2];
                PowerIDNot[index][0] = iReq.PowerIDNot[index][0];
                PowerIDNot[index][1] = iReq.PowerIDNot[index][1];
            }

            NPowerID = new int[iReq.NPowerID.Length][];
            for (var index = 0; index < NPowerID.Length; index++)
            {
                NPowerID[index] = new int[2];
                NPowerID[index][0] = iReq.NPowerID[index][0];
                NPowerID[index][1] = iReq.NPowerID[index][1];
            }

            NPowerIDNot = new int[iReq.NPowerIDNot.Length][];
            for (var index = 0; index < NPowerIDNot.Length; index++)
            {
                NPowerIDNot[index] = new int[2];
                NPowerIDNot[index][0] = iReq.NPowerIDNot[index][0];
                NPowerIDNot[index][1] = iReq.NPowerIDNot[index][1];
            }
        }

        public Requirement(BinaryReader reader)
        {
            ClassName = new string[reader.ReadInt32() + 1];
            for (var index = 0; index < ClassName.Length; index++)
                ClassName[index] = reader.ReadString();
            ClassNameNot = new string[reader.ReadInt32() + 1];
            for (var index = 0; index < ClassNameNot.Length; index++)
                ClassNameNot[index] = reader.ReadString();
            PowerID = new string[reader.ReadInt32() + 1][];
            for (var index = 0; index < PowerID.Length; ++index)
            {
                PowerID[index] = new string[2];
                PowerID[index][0] = reader.ReadString();
                PowerID[index][1] = reader.ReadString();
            }

            PowerIDNot = new string[reader.ReadInt32() + 1][];
            for (var index = 0; index < PowerIDNot.Length; index++)
            {
                PowerIDNot[index] = new string[2];
                PowerIDNot[index][0] = reader.ReadString();
                PowerIDNot[index][1] = reader.ReadString();
            }
        }

        public void StoreTo(BinaryWriter writer)
        {
            writer.Write(ClassName.Length - 1);
            foreach (var index in ClassName)
                writer.Write(index);

            writer.Write(ClassNameNot.Length - 1);
            foreach (var index in ClassNameNot)
                writer.Write(index);

            writer.Write(PowerID.Length - 1);
            foreach (var index in PowerID)
            {
                writer.Write(index[0]);
                writer.Write(index[1]);
            }

            writer.Write(PowerIDNot.Length - 1);
            foreach (var index in PowerIDNot)
            {
                writer.Write(index[0]);
                writer.Write(index[1]);
            }
        }

        public bool ClassOk(string uidClass)
        {
            if (string.IsNullOrEmpty(uidClass))
            {
                return true;
            }
            
            var flag2 = true;
            if (ClassName.Length > 0)
            {
                flag2 = ClassName.Any(t => string.Equals(t, uidClass, StringComparison.OrdinalIgnoreCase));
            }

            if (ClassNameNot.Length <= 0)
            {
                return flag2;
            }

            if (ClassNameNot.Any(t => string.Equals(t, uidClass, StringComparison.OrdinalIgnoreCase)))
            {
                flag2 = false;
            }

            return flag2;
        }

        public bool ClassOk(int nidClass)
        {
            if (nidClass < 0)
            {
                return true;
            }
            
            var flag2 = true;
            if (NClassName.Length > 0)
            {
                flag2 = NClassName.Any(t => t == nidClass);
            }

            if (NClassNameNot.Length <= 0)
            {
                return flag2;
            }

            if (NClassNameNot.Any(t => t == nidClass))
            {
                flag2 = false;
            }

            return flag2;
        }

        public bool ReferencesPower(string uidPower, string uidFix = "")
        {
            var flag = false;
            foreach (var index1 in PowerID)
            {
                for (var index2 = 0; index2 < index1.Length; index2++)
                {
                    if (!string.Equals(index1[index2], uidPower, StringComparison.OrdinalIgnoreCase))
                        continue;
                    flag = true;
                    index1[index2] = uidFix;
                }
            }

            foreach (var index1 in PowerIDNot)
            {
                for (var index2 = 0; index2 < index1.Length; index2++)
                {
                    if (!string.Equals(index1[index2], uidPower, StringComparison.OrdinalIgnoreCase))
                        continue;
                    flag = true;
                    index1[index2] = uidFix;
                }
            }

            return flag;
        }

        private bool RequiredPowersCheck()
        {
            if (PowerID.Length <= 0)
            {
                return true;
            }

            var rqPowerId = PowerID
                .Where(e => e.All(f => string.IsNullOrWhiteSpace(f) | DatabaseAPI.GetPowerByFullName(f) != null))
                .ToArray();

            if (rqPowerId.Length <= 0)
            {
                return true;
            }

            return rqPowerId.Any(e => e.Length == 2
                ? MidsContext.Character.CurrentBuild.FindInToonHistory(DatabaseAPI.NidFromUidPower(e[0])) >= 0 & MidsContext.Character.CurrentBuild.FindInToonHistory(DatabaseAPI.NidFromUidPower(e[1])) >= 0
                : MidsContext.Character.CurrentBuild.FindInToonHistory(DatabaseAPI.NidFromUidPower(e[0])) >= 0);
        }

        private bool ExcludedPowersCheck()
        {
            if (PowerIDNot.Length <= 0)
            {
                return true;
            }

            var rqPowerIdNot = PowerIDNot
                .Where(e => e.All(f => string.IsNullOrWhiteSpace(f) | DatabaseAPI.GetPowerByFullName(f) != null))
                .ToArray();

            if (rqPowerIdNot.Length <= 0)
            {
                return true;
            }

            return !rqPowerIdNot.Any(e => e.Length == 2
                ? MidsContext.Character.CurrentBuild.FindInToonHistory(DatabaseAPI.NidFromUidPower(e[0])) >= 0 & MidsContext.Character.CurrentBuild.FindInToonHistory(DatabaseAPI.NidFromUidPower(e[1])) >= 0
                : MidsContext.Character.CurrentBuild.FindInToonHistory(DatabaseAPI.NidFromUidPower(e[0])) >= 0);
        }

        public bool RequiredPowersOk()
        {
            return RequiredPowersCheck() & ExcludedPowersCheck();
        }

        public void AddPowers(string power1, string power2)
        {
            if (power1.StartsWith("!") & (power2.StartsWith("!") | string.IsNullOrEmpty(power2)))
            {
                Array.Resize(ref PowerIDNot, PowerIDNot.Length + 1);
                PowerIDNot[^1] = new string[2];
                PowerIDNot[^1][0] = power1;
                PowerIDNot[^1][1] = power2;
            }
            else if (!power1.StartsWith("!") & (!power2.StartsWith("!") | string.IsNullOrEmpty(power2)))
            {
                Array.Resize(ref PowerID, PowerID.Length + 1);
                PowerID[^1] = new string[2];
                PowerID[^1][0] = power1;
                PowerID[^1][1] = power2;
            }
            else
            {
                MessageBox.Show("An impossible power requirement has occurred: POWER AND NOT POWER.");
            }
        }
    }
}