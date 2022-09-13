using System;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Core.Import
{
    public class ArchetypeData
    {
        private readonly string _csvString = string.Empty;
        public readonly Archetype Data;
        public readonly bool IsNew;

        public readonly bool IsValid;
        private int _index = -1;

        public ArchetypeData(string iString)
        {
            if (iString == null)
            {
                IsValid = false;
            }
            else
            {
                _csvString = iString;
                Data = new Archetype();
                IsValid = Data.UpdateFromCSV(_csvString);
                IsNew = true;
                for (var index = 0; index <= DatabaseAPI.Database.Classes.Length - 1; ++index)
                {
                    if (string.IsNullOrEmpty(DatabaseAPI.Database.Classes[index].ClassName) ||
                        !string.Equals(DatabaseAPI.Database.Classes[index].ClassName, Data.ClassName,
                            StringComparison.OrdinalIgnoreCase))
                        continue;
                    IsNew = false;
                    _index = index;
                    break;
                }
            }
        }

        public void Apply()
        {
            if (!IsValid)
                return;
            if (IsNew)
            {
                var classes = DatabaseAPI.Database.Classes;
                Array.Resize(ref classes, DatabaseAPI.Database.Classes.Length + 1);
                DatabaseAPI.Database.Classes = classes;
                _index = DatabaseAPI.Database.Classes.Length - 1;
                DatabaseAPI.Database.Classes[_index] = new Archetype();
            }

            if (!IsNew & (_index < 0))
                return;
            DatabaseAPI.Database.Classes[_index].IsNew = IsNew;
            DatabaseAPI.Database.Classes[_index].IsModified = true;
            DatabaseAPI.Database.Classes[_index].UpdateFromCSV(_csvString);
        }

        public bool CheckDifference(out string message)
        {
            message = string.Empty;
            bool flag;
            if (!IsValid)
            {
                flag = false;
            }
            else if (IsNew)
            {
                message = "New";
                flag = true;
            }
            else if (_index < 0 || _index > DatabaseAPI.Database.Classes.Length - 1)
            {
                flag = true;
            }
            else if (DatabaseAPI.Database.Classes[_index].DisplayName != Data.DisplayName)
            {
                message += $"DisplayName: {DatabaseAPI.Database.Classes[_index].DisplayName} => {Data.DisplayName}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Classes[_index].Hero != Data.Hero)
            {
                message += $"isHero: {DatabaseAPI.Database.Classes[_index].Hero} => {Data.Hero}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Classes[_index].Epic != Data.Epic)
            {
                message += $"isEpic: {DatabaseAPI.Database.Classes[_index].Epic} => {Data.Epic}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Classes[_index].DescLong != Data.DescLong)
            {
                message += $"Description: {DatabaseAPI.Database.Classes[_index].DescLong} => {Data.DescLong}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Classes[_index].DescShort != Data.DescShort)
            {
                message += $"Fullname: {DatabaseAPI.Database.Classes[_index].DescShort} => {Data.DescShort}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Classes[_index].Origin.Length != Data.Origin.Length)
            {
                message += $"Origins: {DatabaseAPI.Database.Classes[_index].Origin.Length} => {Data.Origin.Length}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Classes[_index].ClassName != Data.ClassName)
            {
                message += $"ClassID: {DatabaseAPI.Database.Classes[_index].ClassName} => {Data.ClassName}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Classes[_index].Column != Data.Column)
            {
                message += $"ColumnIndex: {DatabaseAPI.Database.Classes[_index].Column} => {Data.Column}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Classes[_index].PrimaryGroup.ToLower() != Data.PrimaryGroup.ToLower())
            {
                message +=
                    $"Primary: {DatabaseAPI.Database.Classes[_index].PrimaryGroup.ToLower()} => {Data.PrimaryGroup.ToLower()}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Classes[_index].SecondaryGroup.ToLower() != Data.SecondaryGroup.ToLower())
            {
                message +=
                    $"Secondary: {DatabaseAPI.Database.Classes[_index].SecondaryGroup.ToLower()} => {Data.SecondaryGroup.ToLower()}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Classes[_index].Playable != Data.Playable)
            {
                message += $"Playable: {DatabaseAPI.Database.Classes[_index].Playable} => {Data.Playable}";
                flag = true;
            }
            else
            {
                flag = false;
            }

            return flag;
        }
    }
}