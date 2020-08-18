using System;
using Base.Master_Classes;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public class clsUniversalImport
    {
        public const string MarkerA = "Primary";
        public const string MarkerB = "Secondary";
        private static int IndexOf;

        private static sPowerLine BreakLine(string iLine, int nAT)
        {
            var sPowerLine = new sPowerLine();
            var strArray1 = SmartBreak(iLine, nAT);
            sPowerLine.Level = (int) Math.Round(Conversion.Val(strArray1[0]));
            sPowerLine.Power = strArray1[1];
            sPowerLine.Slots = Array.Empty<sSlot>();
            var strArray2 = strArray1[2].Replace(" ", "|").Replace(")", "|").Split('|');
            var num1 = strArray2.Length - 1;
            for (var index = 0; index <= num1; ++index)
            {
                var iStr = EnhNameFix(strArray2[index]);
                var flag1 = false;
                var flag2 = iStr.IndexOf("-I", StringComparison.Ordinal) > -1;
                IndexOf = iStr.IndexOf("-S", StringComparison.Ordinal);
                if (flag2 | (iStr.IndexOf(":", StringComparison.Ordinal) > -1))
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

                if (!((num2 > -1) & (startIndex > -1)))
                    continue;
                sPowerLine.Slots = (sSlot[]) Utils.CopyArray(sPowerLine.Slots, new sSlot[sPowerLine.Slots.Length + 1]);
                sPowerLine.Slots[sPowerLine.Slots.Length - 1].Enh = iStr.Substring(0, num2).Trim();
                sPowerLine.Slots[sPowerLine.Slots.Length - 1].Level =
                    (int) Math.Round(Conversion.Val(iStr.Substring(startIndex).Trim()));
                if (iStr.Substring(startIndex).Trim().StartsWith("A"))
                    sPowerLine.Slots[sPowerLine.Slots.Length - 1].Level = sPowerLine.Level;
                sPowerLine.Slots[sPowerLine.Slots.Length - 1].PowerName = sPowerLine.Power;
            }

            return sPowerLine;
        }

        private static string EnhNameFix(string iStr)
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

        private static int FindFirstPower(string[] haystack, int iAT)
        {
            var num = haystack.Length - 1;
            for (var index = 0; index <= num; ++index)
            {
                var strArray = SmartBreak(haystack[index], iAT);
                if (Conversion.Val(strArray[0]) > 0.0 && strArray[1].Length > 0 &&
                    FindPower(strArray[1], iAT).Powerset > -1)
                    return index;
            }

            return -1;
        }

        private static SetPair FindPower(string iName, int nAT)
        {
            var sets = new IPowerset[2];
            if (MidsContext.Character != null)
            {
                sets[0] = MidsContext.Character.Powersets[0];
                sets[1] = MidsContext.Character.Powersets[1];
                var setPair = ScanSetArray(iName, sets);
                if (setPair.Powerset > -1)
                    return setPair;
            }

            var powerByName = DatabaseAPI.GetPowerIndexByDisplayName(iName, nAT);
            if (powerByName < 0)
                powerByName = DatabaseAPI.GetPowerIndexByDisplayName(iName.Replace("'", ""), nAT);
            if (powerByName > -1)
                return new SetPair(DatabaseAPI.Database.Power[powerByName].PowerSetID,
                    DatabaseAPI.Database.Power[powerByName].PowerSetIndex);

            var powersetIndexes1 = DatabaseAPI.GetPowersetIndexes(nAT, Enums.ePowerSetType.Ancillary);
            var setPair2 = ScanSetArray(iName, powersetIndexes1);
            if (setPair2.Powerset > -1) return setPair2;

            var powersetIndexes2 = DatabaseAPI.GetPowersetIndexes(nAT, Enums.ePowerSetType.Pool);
            setPair2 = ScanSetArray(iName, powersetIndexes2);
            return setPair2.Powerset <= -1 ? new SetPair(-1, -1) : setPair2;
        }

        private static int FindPowerSetAdvanced(
            string sSetType,
            Enums.ePowerSetType nSetType,
            int nAT,
            string[] haystack)
        {
            for (var index1 = 0; index1 <= haystack.Length - 1; ++index1)
            {
                if (haystack[index1].IndexOf(sSetType, StringComparison.OrdinalIgnoreCase) <= -1)
                    continue;
                var powersetIndexes = DatabaseAPI.GetPowersetIndexes(nAT, nSetType);
                for (var index2 = 0; index2 <= powersetIndexes.Length - 1; ++index2)
                    if (haystack[index1].IndexOf(powersetIndexes[index2].DisplayName,
                        StringComparison.OrdinalIgnoreCase) > -1)
                        return powersetIndexes[index2].nID;
            }

            return -1;
        }

        private static int FindString(string needle, string[] haystack)
        {
            var num = haystack.Length - 1;
            for (var index = 0; index <= num; ++index)
                if (haystack[index].IndexOf(needle, StringComparison.Ordinal) > -1)
                    return index;

            return -1;
        }

        private static int FindValue(string needle, string[] haystack, ref string dest)
        {
            if (dest == null) throw new ArgumentNullException(nameof(dest));
            for (var index = 0; index <= haystack.Length - 1; ++index)
            {
                if (!haystack[index].StartsWith(needle))
                    continue;
                var strArray = haystack[index].Replace(")", ":").Replace("-", ":").Replace("=", ":").Split(':');
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
            var buildMode = MidsContext.Config.BuildMode;
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
                var haystack = iPost.Split(chArray);
                var num1 = 0;
                var dest = "";
                MidsContext.Character.Reset();
                var character = MidsContext.Character;
                var name = character.Name;
                character.Name = name;
                if (FindValue("Name", haystack, ref name) < 0)
                    MidsContext.Character.Name = "Unknown";
                if (SmartFind("Archetype", haystack, ref dest) < 0)
                {
                    var index1 = -1;
                    var num2 = DatabaseAPI.Database.Classes.Length - 1;
                    for (var index2 = 0; index2 <= num2; ++index2)
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
                {
                    MidsContext.Character.Archetype = DatabaseAPI.GetArchetypeByName(dest);
                }

                var index3 = -1;
                if (FindValue("Primary", haystack, ref dest) > -1)
                    index3 = DatabaseAPI.GetPowersetByName(dest, MidsContext.Character.Archetype.DisplayName).nID;
                if (index3 < 0)
                {
                    index3 = FindPowerSetAdvanced("Primary", Enums.ePowerSetType.Primary,
                        MidsContext.Character.Archetype.Idx, haystack);
                    if (index3 < 0)
                        throw new Exception("Primary Powerset value not found.");
                }

                MidsContext.Character.Powersets[0] = DatabaseAPI.Database.Powersets[index3];
                var index4 = -1;
                if (FindValue("Secondary", haystack, ref dest) > -1)
                    index4 = DatabaseAPI.GetPowersetByName(dest, MidsContext.Character.Archetype.DisplayName).nID;
                if (index4 < 0)
                {
                    index4 = FindPowerSetAdvanced("Secondary", Enums.ePowerSetType.Secondary,
                        MidsContext.Character.Archetype.Idx, haystack);
                    if (index4 < 0)
                        throw new Exception("Secondary Powerset value not found.");
                }

                MidsContext.Character.Powersets[1] = DatabaseAPI.Database.Powersets[index4];
                if ((MidsContext.Character.Powersets[0] == null) | (MidsContext.Character.Powersets[1] == null))
                    throw new Exception("Powerset Name value couldn't be interpreted.");
                var firstPower = FindFirstPower(haystack, MidsContext.Character.Archetype.Idx);
                if (firstPower < 0)
                    throw new Exception("First power entry couldn't be located.");
                var sPowerLineArray = new sPowerLine[0];
                var iPL = new sPowerLine();
                var num3 = haystack.Length - 1;
                for (var index1 = firstPower; index1 <= num3; ++index1)
                {
                    iPL.Assign(BreakLine(haystack[index1], MidsContext.Character.Archetype.Idx));
                    if (!((iPL.Level > 0) & (iPL.Power != "")))
                        continue;
                    sPowerLineArray =
                        (sPowerLine[]) Utils.CopyArray(sPowerLineArray, new sPowerLine[sPowerLineArray.Length + 1]);
                    sPowerLineArray[sPowerLineArray.Length - 1].Assign(iPL);
                }

                for (var index1 = 0; index1 <= sPowerLineArray.Length - 1; ++index1)
                {
                    if (sPowerLineArray[index1].Level > num1)
                        num1 = sPowerLineArray[index1].Level;
                    var num2 = sPowerLineArray[index1].Slots.Length - 1;
                    for (var index2 = 0; index2 <= num2; ++index2)
                        if (sPowerLineArray[index1].Slots[index2].Level > num1)
                            num1 = sPowerLineArray[index1].Slots[index2].Level;
                }

                MainModule.MidsController.Toon.Locked = true;
                if (sPowerLineArray.Length < 1)
                    return false;
                var num5 = sPowerLineArray.Length - 1;
                for (var index1 = 0; index1 <= num5; ++index1)
                {
                    sPowerLineArray[index1].HistoryID = -1;
                    var flag2 = false;
                    var power = FindPower(sPowerLineArray[index1].Power, MidsContext.Character.Archetype.Idx);
                    if (power.Powerset > -1 && DatabaseAPI.Database.Powersets[power.Powerset].SetType ==
                        Enums.ePowerSetType.Inherent)
                        flag2 = true;
                    if (power.Powerset < 0)
                        flag2 = true;
                    else if ((power.Powerset == MidsContext.Character.Powersets[1].nID) & (power.Power == 0))
                        flag2 = true;
                    if (flag2)
                        continue;
                    MainModule.MidsController.Toon.RequestedLevel = sPowerLineArray[index1].Level - 1;
                    MainModule.MidsController.Toon.BuildPower(power.Powerset, power.Power);
                    switch (DatabaseAPI.Database.Powersets[power.Powerset].SetType)
                    {
                        case Enums.ePowerSetType.Pool:
                        {
                            var num2 = MainModule.MidsController.Toon.PoolLocked.Length - 2;
                            for (var index2 = 0;
                                index2 <= num2 && !(MainModule.MidsController.Toon.PoolLocked[index2] &
                                                    (MidsContext.Character.Powersets[3 + index2].nID ==
                                                     power.Powerset));
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
                            when !MainModule.MidsController.Toon.PoolLocked[
                                MainModule.MidsController.Toon.PoolLocked.Length - 1]:
                            MidsContext.Character.Powersets[7].nID = power.Powerset;
                            MainModule.MidsController.Toon.PoolLocked[
                                MainModule.MidsController.Toon.PoolLocked.Length - 1] = true;
                            break;
                    }
                }

                var num6 = sPowerLineArray.Length - 1;
                for (var index1 = 0; index1 <= num6; ++index1)
                {
                    sPowerLineArray[index1].HistoryID =
                        MidsContext.Character.CurrentBuild.FindInToonHistory(
                            DatabaseAPI.NidFromUidPower(sPowerLineArray[index1].Power));
                    if (sPowerLineArray[index1].HistoryID == -1 &&
                        (index1 > -1) & (index1 < MidsContext.Character.CurrentBuild.Powers.Count))
                        sPowerLineArray[index1].HistoryID = index1;
                    if (!((sPowerLineArray[index1].HistoryID > -1) & (sPowerLineArray[index1].Slots.Length > 0)))
                        continue;
                    if (sPowerLineArray[index1].Slots.Length > 1)
                        MidsContext.Character.CurrentBuild.Powers[sPowerLineArray[index1].HistoryID].Slots =
                            new SlotEntry[sPowerLineArray[index1].Slots.Length - 1 + 1];
                    var num2 = sPowerLineArray[index1].Slots.Length - 1;
                    for (var index2 = 0; index2 <= num2; ++index2)
                    {
                        if (index2 >= MidsContext.Character.CurrentBuild.Powers[sPowerLineArray[index1].HistoryID].Slots
                            .Length)
                            continue;
                        var slots = MidsContext.Character.CurrentBuild.Powers[sPowerLineArray[index1].HistoryID].Slots;
                        var index5 = index2;
                        slots[index5].Enhancement = new I9Slot();
                        slots[index5].FlippedEnhancement = new I9Slot();
                        slots[index5].Enhancement.Enh = MatchEnhancement(sPowerLineArray[index1].Slots[index2].Enh);
                        slots[index5].Enhancement.Grade = Enums.eEnhGrade.SingleO;
                        slots[index5].Enhancement.IOLevel =
                            sPowerLineArray[index1].Slots[index2].Enh.IndexOf("-I:", StringComparison.Ordinal) <= -1
                                ? sPowerLineArray[index1].Slots[index2].Enh.IndexOf(":", StringComparison.Ordinal) <=
                                  -1 ? MidsContext.Config.I9.DefaultIOLevel :
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
                Interaction.MsgBox(
                    "Unable to import from forum post:\r\n" + ex.Message +
                    "\r\n\r\nCheck the build was copied correctly.",
                    MsgBoxStyle.Information, "Forum Import Filter");
                return false;
            }
        }

        private static int MatchEnhancement(string iEnh)
        {
            if (iEnh.IndexOf("-I", StringComparison.Ordinal) > -1)
            {
                var startIndex = 0;
                var length = iEnh.IndexOf("-", StringComparison.Ordinal);
                return DatabaseAPI.GetEnhancementByName(iEnh.Substring(startIndex, length), Enums.eType.InventO);
            }

            if (!((iEnh.IndexOf("-", StringComparison.Ordinal) > -1) &
                  (iEnh.IndexOf("-S", StringComparison.Ordinal) < 0)))
                return DatabaseAPI.GetEnhancementByName(iEnh);
            var iSet = iEnh.Substring(0, iEnh.IndexOf("-", StringComparison.Ordinal));
            var num = iEnh.IndexOf(":", StringComparison.Ordinal);
            return DatabaseAPI.GetEnhancementByName(
                num >= 0
                    ? iEnh.Substring(iEnh.IndexOf("-", StringComparison.Ordinal) + 1,
                        num - (iEnh.IndexOf("-", StringComparison.Ordinal) + 1))
                    : iEnh.Substring(iEnh.IndexOf("-", StringComparison.Ordinal) + 1), iSet);
        }

        private static string PowerNameFix(string iStr)
        {
            return clsToonX.FixSpelling(iStr)
                .Replace("Gravity Emanation", "Gravitic Emanation")
                .Replace("Dark Matter Detonation", "Dark Detonation")
                .Replace("Dark Nova Emmanation", "Dark Nova Emanation");
        }

        private static SetPair ScanSetArray(
            string iName,
            IPowerset[] sets)
        {
            for (var index = 0; index <= sets.Length - 1; ++index)
            {
                if (sets[index] == null)
                    continue;
                var num2 = sets[index].Powers.Length - 1;
                for (var iPower = 0; iPower <= num2; ++iPower)
                    if (string.Equals(sets[index].Powers[iPower].DisplayName, iName,
                        StringComparison.OrdinalIgnoreCase))
                        return new SetPair(sets[index].nID, iPower);
            }

            return new SetPair(-1, -1);
        }

        private static int SeekAn(string iStr, int start)
        {
            if (start < 0)
                start = 0;
            var num = iStr.Length - 1;
            for (var index = start; index <= num; ++index)
                if (char.IsLetterOrDigit(iStr, index))
                    return index;

            return -1;
        }

        private static int SeekNumber(string iStr, int start)
        {
            if (start < 0)
                start = 0;
            for (var index = start; index <= iStr.Length - 1; ++index)
                if (char.IsDigit(iStr, index))
                    return index;

            return -1;
        }

        private static int SeekNumberSpecial(string iStr, int start)
        {
            if (start < 0)
                start = 0;
            for (var index = start; index <= iStr.Length - 1; ++index)
            {
                if (!char.IsDigit(iStr, index))
                    continue;
                if (index <= 0)
                    return index;
                if ((iStr.Substring(index - 1, 1) != ":") & !char.IsDigit(iStr, index - 1))
                    return index;
            }

            return -1;
        }

        private static int SeekPowerAdvanced(string iString, int nAT)
        {
            var index1 = -1;
            for (var index2 = 0; index2 <= DatabaseAPI.Database.Power.Length - 1; ++index2)
            {
                var power2 = DatabaseAPI.Database.Power[index2];
                var power2set = DatabaseAPI.Database.Powersets[power2.PowerSetID];
                if (iString.IndexOf(power2.DisplayName, StringComparison.OrdinalIgnoreCase) <= -1 ||
                    power2.PowerSetID <= -1)
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

        private static int SeekSep(string iStr, int start, bool readAhead = true)
        {
            if (start < 0)
                start = 0;
            for (var index = start; index <= iStr.Length - 1; ++index)
            {
                if (char.IsLetterOrDigit(iStr, index) && iStr.Substring(index, 1) != " " ||
                    iStr.Substring(index, 1) == "'") continue;
                if (!((iStr.Length > index + 1) & readAhead))
                    return index;
                if (!char.IsLetterOrDigit(iStr, index + 1))
                    return index;
            }

            return -1;
        }

        private static int SeekSepSpecial(string iStr, int start)
        {
            if (start < 0)
                start = 0;
            for (var index = start; index <= iStr.Length - 1; ++index)
                if ((!char.IsLetterOrDigit(iStr, index) | (iStr.Substring(index, 1) == " ")) &
                    (iStr.Substring(index, 1) != ":") & (iStr.Substring(index, 1) != "-") &
                    (iStr.Substring(index, 1) != "+") &
                    (iStr.Substring(index, 1) != "/") & (iStr.Substring(index, 1) != "%") &
                    (iStr.Substring(index, 1) != "'"))
                    return index;

            return -1;
        }

        private static string[] SmartBreak(string iStr, int nAT)
        {
            string[] strArray =
            {
                "", "", ""
            };
            var num1 = SeekNumber(iStr, 0);
            if (num1 <= -1)
                return strArray;
            var start1 = SeekSep(iStr, num1);
            if (start1 <= -1)
                return strArray;
            strArray[0] = iStr.Substring(num1, start1 - num1).Trim();
            var num2 = SeekAn(iStr, start1);
            var index = SeekPowerAdvanced(iStr, nAT);
            int start2;
            if (index > -1)
            {
                var displayName = DatabaseAPI.Database.Power[index].DisplayName;
                start2 = num2 + displayName.Length;
            }
            else
            {
                start2 = SeekSep(iStr, num2);
            }

            if ((num2 > -1) & (start2 > -1))
                strArray[1] = iStr.Substring(num2, start2 - num2).Trim();
            var startIndex = SeekAn(iStr, start2);
            if (startIndex > -1)
                strArray[2] = iStr.Substring(startIndex);
            return strArray;
        }

        private static int SmartFind(string valueName, string[] haystack, ref string dest)
        {
            var num1 = haystack.Length - 1;
            for (var index1 = 0; index1 <= num1; ++index1)
            {
                var start = 0;
                var strArray = new string[0];
                int num2;
                for (var flag = true; flag; flag = (num2 > -1) & (start > -1))
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

                var num3 = strArray.Length - 1;
                for (var index2 = 0; index2 <= num3; ++index2)
                {
                    if (!string.Equals(valueName, strArray[index2], StringComparison.OrdinalIgnoreCase) ||
                        strArray.Length <= index2 + 1)
                        continue;
                    dest = strArray[index2 + 1];
                    return index1;
                }
            }

            return -1;
        }

        private struct SetPair
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
                var num = Slots.Length - 1;
                for (var index = 0; index <= num; ++index)
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