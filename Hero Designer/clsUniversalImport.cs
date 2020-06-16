
using System;
using Base.Data_Classes;
using Base.Master_Classes;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public class clsUniversalImport
    {
        private static int IndexOf;
        public const string MarkerA = "Primary";
        public const string MarkerB = "Secondary";

        static sPowerLine BreakLine(string iLine, int nAT)
        {
            sPowerLine sPowerLine = new sPowerLine();
            string[] strArray1 = SmartBreak(iLine, nAT);
            sPowerLine.Level = (int) Math.Round(Conversion.Val(strArray1[0]));
            sPowerLine.Power = strArray1[1];
            sPowerLine.Slots = Array.Empty<sSlot>();
            string[] strArray2 = strArray1[2].Replace(" ", "|").Replace(")", "|").Split('|');
            int num1 = strArray2.Length - 1;
            for (int index = 0; index <= num1; ++index)
            {
                string iStr = EnhNameFix(strArray2[index]);
                bool flag1 = false;
                bool flag2 = iStr.IndexOf("-I", StringComparison.Ordinal) > -1;
                IndexOf = iStr.IndexOf("-S", StringComparison.Ordinal);
                if (flag2 | iStr.IndexOf(":", StringComparison.Ordinal) > -1)
                    flag1 = true;
                if (iStr.Length <= 0)
                    continue;
                int num2;
                int startIndex;
                if (iStr == "A")
                {
                    num2 = 0;
                    startIndex = 0;
                }
                else
                {
                    if (flag1)
                    {
                        num2 = SeekSepSpecial(iStr, 0);
                        startIndex = SeekNumberSpecial(iStr, num2);
                    }
                    else
                    {
                        num2 = SeekSep(iStr, 0, false);
                        startIndex = SeekNumber(iStr, num2);
                    }

                    if (startIndex < 0)
                        startIndex = SeekAn(iStr, num2);
                }

                if (!(num2 > -1 & startIndex > -1))
                    continue;
                sPowerLine.Slots = (sSlot[]) Utils.CopyArray(sPowerLine.Slots, new sSlot[sPowerLine.Slots.Length + 1]);
                sPowerLine.Slots[sPowerLine.Slots.Length - 1].Enh = iStr.Substring(0, num2).Trim();
                sPowerLine.Slots[sPowerLine.Slots.Length - 1].Level = (int) Math.Round(Conversion.Val(iStr.Substring(startIndex).Trim()));
                if (iStr.Substring(startIndex).Trim().StartsWith("A"))
                    sPowerLine.Slots[sPowerLine.Slots.Length - 1].Level = sPowerLine.Level;
                sPowerLine.Slots[sPowerLine.Slots.Length - 1].PowerName = sPowerLine.Power;
            }

            return sPowerLine;
        }

        static string EnhNameFix(string iStr)
        {
            iStr = iStr.Replace("Fly", "Flight");
            iStr = iStr.Replace("Rechg", "RechRdx");
            iStr = iStr.Replace("TH_Buf", "ToHit");
            iStr = iStr.Replace("TH_DeBuf", "ToHitDeb");
            iStr = iStr.Replace("DmgRes", "ResDam");
            iStr = iStr.Replace("ConfDur", "Conf");
            if (iStr.IndexOf("DefBuff", StringComparison.Ordinal) < 0)
                iStr = iStr.Replace("DefBuf", "DefBuff");
            iStr = iStr.Replace("DefDeBuf", "DefDeb");
            iStr = iStr.Replace("DisDur", "Dsrnt");
            iStr = iStr.Replace("Intang", "Intan");
            iStr = iStr.Replace("KB_Dist", "KBDist");
            iStr = iStr.Replace("HO:", "");
            iStr = iStr.Replace("HY:", "");
            iStr = iStr.Replace("YO:", "");
            iStr = iStr.Replace("TN:", "");
            iStr = iStr.Replace("CentiExp", "Centri");
            iStr = iStr.Replace("CytosExp", "Cyto");
            iStr = iStr.Replace("EndopExp", "Endo");
            iStr = iStr.Replace("EnzymExp", "Enzym");
            iStr = iStr.Replace("GolgiExp", "Golgi");
            iStr = iStr.Replace("LysosExp", "Lyso");
            iStr = iStr.Replace("MembrExp", "Membr");
            iStr = iStr.Replace("MicroExp", "Micro");
            iStr = iStr.Replace("NucleExp", "Nucle");
            iStr = iStr.Replace("PeroxExp", "Perox");
            iStr = iStr.Replace("RibosExp", "Ribo");
            iStr = iStr.Replace("damres", "ResDam");
            iStr = iStr.Replace("dam", "Dmg");
            iStr = iStr.Replace("hel", "Heal");
            iStr = iStr.Replace("recred", "RechRdx");
            iStr = iStr.Replace("endred", "EndRdx");
            iStr = iStr.Replace("runspd", "Run");
            iStr = iStr.Replace("disdur", "Dsrnt");
            iStr = iStr.Replace("endrec", "EndMod");
            iStr = iStr.Replace("rng", "Range");
            iStr = iStr.Replace("kbkdis", "KDBist");
            iStr = iStr.Replace("defdbf", "DefDeb");
            if (iStr.IndexOf("DefBuff", StringComparison.Ordinal) < 0)
                iStr = iStr.Replace("defbuf", "DefBuff");
            return iStr;
        }

        static int FindFirstPower(string[] haystack, int iAT)
        {
            int num = haystack.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                string[] strArray = SmartBreak(haystack[index], iAT);
                if (Conversion.Val(strArray[0]) > 0.0 && strArray[1].Length > 0 && FindPower(strArray[1], iAT).Powerset > -1)
                    return index;
            }

            return -1;
        }

        static SetPair FindPower(string iName, int nAT)
        {
            IPowerset[] sets = new IPowerset[2];
            if (MidsContext.Character != null)
            {
                sets[0] = MidsContext.Character.Powersets[0];
                sets[1] = MidsContext.Character.Powersets[1];
                SetPair setPair = ScanSetArray(iName, sets);
                if (setPair.Powerset > -1)
                    return setPair;
            }

            int powerByName = DatabaseAPI.GetPowerByName(iName, nAT);
            if (powerByName < 0)
                powerByName = DatabaseAPI.GetPowerByName(iName.Replace("'", ""), nAT);
            if (powerByName > -1)
            {
                return new SetPair(DatabaseAPI.Database.Power[powerByName].PowerSetID, DatabaseAPI.Database.Power[powerByName].PowerSetIndex);
            }

            IPowerset[] powersetIndexes1 = DatabaseAPI.GetPowersetIndexes(nAT, Enums.ePowerSetType.Ancillary);
            SetPair setPair2 = ScanSetArray(iName, powersetIndexes1);
            if (setPair2.Powerset > -1)
            {
                return setPair2;
            }

            IPowerset[] powersetIndexes2 = DatabaseAPI.GetPowersetIndexes(nAT, Enums.ePowerSetType.Pool);
            setPair2 = ScanSetArray(iName, powersetIndexes2);
            return setPair2.Powerset <= -1 ? new SetPair(-1, -1) : setPair2;
        }

        static int FindPowerSetAdvanced(
            string sSetType,
            Enums.ePowerSetType nSetType,
            int nAT,
            string[] haystack)
        {
            for (int index1 = 0; index1 <= haystack.Length - 1; ++index1)
            {
                if (haystack[index1].IndexOf(sSetType, StringComparison.OrdinalIgnoreCase) <= -1)
                    continue;
                IPowerset[] powersetIndexes = DatabaseAPI.GetPowersetIndexes(nAT, nSetType);
                for (int index2 = 0; index2 <= powersetIndexes.Length - 1; ++index2)
                {
                    if (haystack[index1].IndexOf(powersetIndexes[index2].DisplayName, StringComparison.OrdinalIgnoreCase) > -1)
                        return powersetIndexes[index2].nID;
                }
            }

            return -1;
        }

        static int FindString(string needle, string[] haystack)
        {
            int num = haystack.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (haystack[index].IndexOf(needle, StringComparison.Ordinal) > -1)
                    return index;
            }

            return -1;
        }

        static int FindValue(string needle, string[] haystack, ref string dest)
        {
            if (dest == null) throw new ArgumentNullException(nameof(dest));
            for (int index = 0; index <= haystack.Length - 1; ++index)
            {
                if (!haystack[index].StartsWith(needle))
                    continue;
                string[] strArray = haystack[index].Replace(")", ":").Replace("-", ":").Replace("=", ":").Split(':');
                if (strArray.Length <= 1)
                    continue;
                dest = strArray[1].Trim();
                return index;
            }

            dest = "";
            return -1;
        }

        public static bool InterpretForumPost(string iPost)
        {
            Enums.dmModes buildMode = MidsContext.Config.BuildMode;
            MidsContext.Config.BuildMode = Enums.dmModes.Dynamic;
            try
            {
                iPost = PowerNameFix(iPost);
                char[] chArray =
                {
                    '`'
                };
                iPost = iPost.Replace("\r\n", "`");
                iPost = iPost.Replace("\n", "`");
                iPost = iPost.Replace("\r", "`");
                string[] haystack = iPost.Split(chArray);
                int num1 = 0;
                string dest = "";
                MidsContext.Character.Reset();
                Character character = MidsContext.Character;
                string name = character.Name;
                character.Name = name;
                if (FindValue("Name", haystack, ref name) < 0)
                    MidsContext.Character.Name = "Unknown";
                if (SmartFind("Archetype", haystack, ref dest) < 0)
                {
                    int index1 = -1;
                    int num2 = DatabaseAPI.Database.Classes.Length - 1;
                    for (int index2 = 0; index2 <= num2; ++index2)
                    {
                        if (FindString(DatabaseAPI.Database.Classes[index2].DisplayName, haystack) <= -1)
                            continue;
                        index1 = index2;
                        break;
                    }

                    if (index1 <= -1)
                        throw new Exception("Archetype value not found.");
                    MidsContext.Character.Archetype = DatabaseAPI.Database.Classes[index1];
                }
                else
                    MidsContext.Character.Archetype = DatabaseAPI.GetArchetypeByName(dest);

                int index3 = -1;
                if (FindValue("Primary", haystack, ref dest) > -1)
                    index3 = DatabaseAPI.GetPowersetByName(dest, MidsContext.Character.Archetype.DisplayName).nID;
                if (index3 < 0)
                {
                    index3 = FindPowerSetAdvanced("Primary", Enums.ePowerSetType.Primary, MidsContext.Character.Archetype.Idx, haystack);
                    if (index3 < 0)
                        throw new Exception("Primary Powerset value not found.");
                }

                MidsContext.Character.Powersets[0] = DatabaseAPI.Database.Powersets[index3];
                int index4 = -1;
                if (FindValue("Secondary", haystack, ref dest) > -1)
                    index4 = DatabaseAPI.GetPowersetByName(dest, MidsContext.Character.Archetype.DisplayName).nID;
                if (index4 < 0)
                {
                    index4 = FindPowerSetAdvanced("Secondary", Enums.ePowerSetType.Secondary, MidsContext.Character.Archetype.Idx, haystack);
                    if (index4 < 0)
                        throw new Exception("Secondary Powerset value not found.");
                }

                MidsContext.Character.Powersets[1] = DatabaseAPI.Database.Powersets[index4];
                if (MidsContext.Character.Powersets[0] == null | MidsContext.Character.Powersets[1] == null)
                    throw new Exception("Powerset Name value couldn't be interpreted.");
                int firstPower = FindFirstPower(haystack, MidsContext.Character.Archetype.Idx);
                if (firstPower < 0)
                    throw new Exception("First power entry couldn't be located.");
                sPowerLine[] sPowerLineArray = new sPowerLine[0];
                sPowerLine iPL = new sPowerLine();
                int num3 = haystack.Length - 1;
                for (int index1 = firstPower; index1 <= num3; ++index1)
                {
                    iPL.Assign(BreakLine(haystack[index1], MidsContext.Character.Archetype.Idx));
                    if (!(iPL.Level > 0 & iPL.Power != ""))
                        continue;
                    sPowerLineArray = (sPowerLine[]) Utils.CopyArray(sPowerLineArray, new sPowerLine[sPowerLineArray.Length + 1]);
                    sPowerLineArray[sPowerLineArray.Length - 1].Assign(iPL);
                }

                for (int index1 = 0; index1 <= sPowerLineArray.Length - 1; ++index1)
                {
                    if (sPowerLineArray[index1].Level > num1)
                        num1 = sPowerLineArray[index1].Level;
                    int num2 = sPowerLineArray[index1].Slots.Length - 1;
                    for (int index2 = 0; index2 <= num2; ++index2)
                    {
                        if (sPowerLineArray[index1].Slots[index2].Level > num1)
                            num1 = sPowerLineArray[index1].Slots[index2].Level;
                    }
                }

                MainModule.MidsController.Toon.Locked = true;
                if (sPowerLineArray.Length < 1)
                    return false;
                int num5 = sPowerLineArray.Length - 1;
                for (int index1 = 0; index1 <= num5; ++index1)
                {
                    sPowerLineArray[index1].HistoryID = -1;
                    bool flag2 = false;
                    SetPair power = FindPower(sPowerLineArray[index1].Power, MidsContext.Character.Archetype.Idx);
                    if (power.Powerset > -1 && DatabaseAPI.Database.Powersets[power.Powerset].SetType == Enums.ePowerSetType.Inherent)
                        flag2 = true;
                    if (power.Powerset < 0)
                        flag2 = true;
                    else if (power.Powerset == MidsContext.Character.Powersets[1].nID & power.Power == 0)
                        flag2 = true;
                    if (flag2)
                        continue;
                    MainModule.MidsController.Toon.RequestedLevel = sPowerLineArray[index1].Level - 1;
                    MainModule.MidsController.Toon.BuildPower(power.Powerset, power.Power);
                    switch (DatabaseAPI.Database.Powersets[power.Powerset].SetType)
                    {
                        case Enums.ePowerSetType.Pool:
                        {
                            int num2 = MainModule.MidsController.Toon.PoolLocked.Length - 2;
                            for (int index2 = 0;
                                index2 <= num2 && !(MainModule.MidsController.Toon.PoolLocked[index2] &
                                                    MidsContext.Character.Powersets[3 + index2].nID == power.Powerset);
                                ++index2)
                            {
                                if (MainModule.MidsController.Toon.PoolLocked[index2])
                                    continue;
                                MidsContext.Character.Powersets[3 + index2].nID = power.Powerset;
                                MainModule.MidsController.Toon.PoolLocked[index2] = true;
                                break;
                            }

                            break;
                        }
                        case Enums.ePowerSetType.Ancillary
                            when !MainModule.MidsController.Toon.PoolLocked[MainModule.MidsController.Toon.PoolLocked.Length - 1]:
                            MidsContext.Character.Powersets[7].nID = power.Powerset;
                            MainModule.MidsController.Toon.PoolLocked[MainModule.MidsController.Toon.PoolLocked.Length - 1] = true;
                            break;
                    }
                }

                int num6 = sPowerLineArray.Length - 1;
                for (int index1 = 0; index1 <= num6; ++index1)
                {
                    sPowerLineArray[index1].HistoryID =
                        MidsContext.Character.CurrentBuild.FindInToonHistory(DatabaseAPI.NidFromUidPower(sPowerLineArray[index1].Power));
                    if (sPowerLineArray[index1].HistoryID == -1 && index1 > -1 & index1 < MidsContext.Character.CurrentBuild.Powers.Count)
                        sPowerLineArray[index1].HistoryID = index1;
                    if (!(sPowerLineArray[index1].HistoryID > -1 & sPowerLineArray[index1].Slots.Length > 0))
                        continue;
                    if (sPowerLineArray[index1].Slots.Length > 1)
                        MidsContext.Character.CurrentBuild.Powers[sPowerLineArray[index1].HistoryID].Slots =
                            new SlotEntry[sPowerLineArray[index1].Slots.Length - 1 + 1];
                    int num2 = sPowerLineArray[index1].Slots.Length - 1;
                    for (int index2 = 0; index2 <= num2; ++index2)
                    {
                        if (index2 >= MidsContext.Character.CurrentBuild.Powers[sPowerLineArray[index1].HistoryID].Slots.Length)
                            continue;
                        SlotEntry[] slots = MidsContext.Character.CurrentBuild.Powers[sPowerLineArray[index1].HistoryID].Slots;
                        int index5 = index2;
                        slots[index5].Enhancement = new I9Slot();
                        slots[index5].FlippedEnhancement = new I9Slot();
                        slots[index5].Enhancement.Enh = MatchEnhancement(sPowerLineArray[index1].Slots[index2].Enh);
                        slots[index5].Enhancement.Grade = Enums.eEnhGrade.SingleO;
                        slots[index5].Enhancement.IOLevel =
                            sPowerLineArray[index1].Slots[index2].Enh.IndexOf("-I:", StringComparison.Ordinal) <= -1
                                ? sPowerLineArray[index1].Slots[index2].Enh.IndexOf(":", StringComparison.Ordinal) <= -1 ?
                                    MidsContext.Config.I9.DefaultIOLevel :
                                    (int) Math.Round(Conversion.Val(sPowerLineArray[index1].Slots[index2].Enh
                                                         .Substring(sPowerLineArray[index1].Slots[index2].Enh
                                                                        .IndexOf(":", StringComparison.Ordinal) + 1)) - 1.0)
                                : (int) Math.Round(Conversion.Val(sPowerLineArray[index1].Slots[index2].Enh
                                                       .Substring(sPowerLineArray[index1].Slots[index2].Enh
                                                                      .IndexOf(":", StringComparison.Ordinal) + 1)) - 1.0);
                        slots[index5].Level = index2 != 0
                            ? sPowerLineArray[index1].Slots[index2].Level - 1
                            : MidsContext.Character.CurrentBuild.Powers[sPowerLineArray[index1].HistoryID].Level;
                        if (slots[index5].Level < 0)
                            slots[index5].Level = 0;
                    }
                }

                MidsContext.Character.Validate();
                MidsContext.Config.BuildMode = buildMode;
                return true;
            }
            catch (Exception ex)
            {
                MidsContext.Config.BuildMode = buildMode;
                Interaction.MsgBox("Unable to import from forum post:\r\n" + ex.Message + "\r\n\r\nCheck the build was copied correctly.",
                    MsgBoxStyle.Information, "Forum Import Filter");
                return false;
            }
        }

        static int MatchEnhancement(string iEnh)
        {
            if (iEnh.IndexOf("-I", StringComparison.Ordinal) > -1)
            {
                int startIndex = 0;
                int length = iEnh.IndexOf("-", StringComparison.Ordinal);
                return DatabaseAPI.GetEnhancementByName(iEnh.Substring(startIndex, length), Enums.eType.InventO);
            }

            if (!(iEnh.IndexOf("-", StringComparison.Ordinal) > -1 & iEnh.IndexOf("-S", StringComparison.Ordinal) < 0))
                return DatabaseAPI.GetEnhancementByName(iEnh);
            string iSet = iEnh.Substring(0, iEnh.IndexOf("-", StringComparison.Ordinal));
            int num = iEnh.IndexOf(":", StringComparison.Ordinal);
            return DatabaseAPI.GetEnhancementByName(
                num >= 0
                    ? iEnh.Substring(iEnh.IndexOf("-", StringComparison.Ordinal) + 1, num - (iEnh.IndexOf("-", StringComparison.Ordinal) + 1))
                    : iEnh.Substring(iEnh.IndexOf("-", StringComparison.Ordinal) + 1), iSet);
        }

        static string PowerNameFix(string iStr)
            => clsToonX.FixSpelling(iStr)
                .Replace("Gravity Emanation", "Gravitic Emanation")
                .Replace("Dark Matter Detonation", "Dark Detonation")
                .Replace("Dark Nova Emmanation", "Dark Nova Emanation");

        static SetPair ScanSetArray(
            string iName,
            IPowerset[] sets)
        {
            for (int index = 0; index <= sets.Length - 1; ++index)
            {
                if (sets[index] == null)
                    continue;
                int num2 = sets[index].Powers.Length - 1;
                for (int iPower = 0; iPower <= num2; ++iPower)
                {
                    if (string.Equals(sets[index].Powers[iPower].DisplayName, iName, StringComparison.OrdinalIgnoreCase))
                        return new SetPair(sets[index].nID, iPower);
                }
            }

            return new SetPair(-1, -1);
        }

        static int SeekAn(string iStr, int start)
        {
            if (start < 0)
                start = 0;
            int num = iStr.Length - 1;
            for (int index = start; index <= num; ++index)
            {
                if (char.IsLetterOrDigit(iStr, index))
                    return index;
            }

            return -1;
        }

        static int SeekNumber(string iStr, int start)
        {
            if (start < 0)
                start = 0;
            for (int index = start; index <= iStr.Length - 1; ++index)
            {
                if (char.IsDigit(iStr, index))
                    return index;
            }

            return -1;
        }

        static int SeekNumberSpecial(string iStr, int start)
        {
            if (start < 0)
                start = 0;
            for (int index = start; index <= iStr.Length - 1; ++index)
            {
                if (!char.IsDigit(iStr, index))
                    continue;
                if (index <= 0)
                    return index;
                if (iStr.Substring(index - 1, 1) != ":" & !char.IsDigit(iStr, index - 1))
                    return index;
            }

            return -1;
        }

        static int SeekPowerAdvanced(string iString, int nAT)
        {
            int index1 = -1;
            for (int index2 = 0; index2 <= DatabaseAPI.Database.Power.Length - 1; ++index2)
            {
                var power2 = DatabaseAPI.Database.Power[index2];
                var power2set = DatabaseAPI.Database.Powersets[power2.PowerSetID];
                if (iString.IndexOf(power2.DisplayName, StringComparison.OrdinalIgnoreCase) <= -1 || power2.PowerSetID <= -1)
                    continue;
                if (power2set.nArchetype == -1)
                {
                    if (index1 < 0)
                        index1 = index2;
                    else if (DatabaseAPI.Database.Power[index1].DisplayName.Length < power2.DisplayName.Length)
                        index1 = index2;
                }
                else if (power2set.nArchetype == nAT)
                {
                    if (index1 < 0)
                        index1 = index2;
                    else if (DatabaseAPI.Database.Power[index1].DisplayName.Length < power2.DisplayName.Length)
                        index1 = index2;
                }
            }

            return index1;
        }

        static int SeekSep(string iStr, int start, bool readAhead = true)
        {
            if (start < 0)
                start = 0;
            for (int index = start; index <= iStr.Length - 1; ++index)
            {
                if ((char.IsLetterOrDigit(iStr, index) && iStr.Substring(index, 1) != " ") ||
                    iStr.Substring(index, 1) == "'") continue;
                if (!(iStr.Length > index + 1 & readAhead))
                    return index;
                if (!char.IsLetterOrDigit(iStr, index + 1))
                    return index;
            }

            return -1;
        }

        static int SeekSepSpecial(string iStr, int start)
        {
            if (start < 0)
                start = 0;
            for (int index = start; index <= iStr.Length - 1; ++index)
            {
                if ((!char.IsLetterOrDigit(iStr, index) | iStr.Substring(index, 1) == " ") &
                    iStr.Substring(index, 1) != ":" & iStr.Substring(index, 1) != "-" & iStr.Substring(index, 1) != "+" &
                     iStr.Substring(index, 1) != "/" & iStr.Substring(index, 1) != "%" & iStr.Substring(index, 1) != "'")
                    return index;
            }

            return -1;
        }

        static string[] SmartBreak(string iStr, int nAT)
        {
            string[] strArray =
            {
                "", "", ""
            };
            int num1 = SeekNumber(iStr, 0);
            if (num1 <= -1)
                return strArray;
            int start1 = SeekSep(iStr, num1);
            if (start1 <= -1)
                return strArray;
            strArray[0] = iStr.Substring(num1, start1 - num1).Trim();
            int num2 = SeekAn(iStr, start1);
            int index = SeekPowerAdvanced(iStr, nAT);
            int start2;
            if (index > -1)
            {
                string displayName = DatabaseAPI.Database.Power[index].DisplayName;
                start2 = num2 + displayName.Length;
            }
            else
                start2 = SeekSep(iStr, num2);

            if (num2 > -1 & start2 > -1)
                strArray[1] = iStr.Substring(num2, start2 - num2).Trim();
            int startIndex = SeekAn(iStr, start2);
            if (startIndex > -1)
                strArray[2] = iStr.Substring(startIndex);
            return strArray;
        }

        static int SmartFind(string valueName, string[] haystack, ref string dest)
        {
            int num1 = haystack.Length - 1;
            for (int index1 = 0; index1 <= num1; ++index1)
            {
                int start = 0;
                string[] strArray = new string[0];
                int num2;
                for (bool flag = true; flag; flag = num2 > -1 & start > -1)
                {
                    num2 = SeekAn(haystack[index1], start);
                    start = SeekSep(haystack[index1], num2, false);
                    if (num2 <= -1)
                        continue;
                    strArray = (string[]) Utils.CopyArray(strArray, new string[strArray.Length + 1]);
                    strArray[strArray.Length - 1] = start <= -1
                        ? haystack[index1].Substring(num2).Trim()
                        : haystack[index1].Substring(num2, start - num2).Trim();
                }

                int num3 = strArray.Length - 1;
                for (int index2 = 0; index2 <= num3; ++index2)
                {
                    if (!string.Equals(valueName, strArray[index2], StringComparison.OrdinalIgnoreCase) || strArray.Length <= index2 + 1)
                        continue;
                    dest = strArray[index2 + 1];
                    return index1;
                }
            }

            return -1;
        }

        struct SetPair
        {
            public readonly int Powerset;
            public readonly int Power;

            public SetPair(int iSet, int iPower)
            {
                this = new SetPair();
                Powerset = iSet;
                Power = iPower;
            }
        }

        public struct sPowerLine
        {
            public int Level;
            public string Power;
            public int HistoryID;
            public sSlot[] Slots;

            public void Assign(sPowerLine iPL)
            {
                Level = iPL.Level;
                Power = iPL.Power;
                Slots = new sSlot[iPL.Slots.Length - 1 + 1];
                HistoryID = iPL.HistoryID;
                int num = Slots.Length - 1;
                for (int index = 0; index <= num; ++index)
                    Slots[index].Assign(iPL.Slots[index]);
            }
        }

        public struct sSlot
        {
            public int Level;
            public string Enh;
            public string PowerName;

            public void Assign(sSlot iSlot)
            {
                Level = iSlot.Level;
                Enh = iSlot.Enh;
                PowerName = iSlot.PowerName;
            }
        }
    }
}
