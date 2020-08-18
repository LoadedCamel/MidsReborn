using System;

namespace Import
{
    public class PowersetData
    {
        private readonly string _csvString = string.Empty;

        public readonly Powerset Data;
        public readonly bool IsNew;
        public readonly bool IsValid;
        private int _index = -1;

        public PowersetData(string iString)
        {
            if (string.IsNullOrEmpty(iString))
                return;
            _csvString = iString;
            Data = new Powerset();
            IsValid = Data.ImportFromCSV(iString);
            IsNew = true;
            for (var index = 0; index <= DatabaseAPI.Database.Powersets.Length - 1; ++index)
            {
                if (string.IsNullOrEmpty(DatabaseAPI.Database.Powersets[index].FullName) ||
                    !string.Equals(DatabaseAPI.Database.Powersets[index].FullName, Data.FullName,
                        StringComparison.OrdinalIgnoreCase))
                    continue;
                IsNew = false;
                _index = index;
                break;
            }
        }

        public void Apply()
        {
            if (!IsValid)
                return;
            if (IsNew)
            {
                var powersets = DatabaseAPI.Database.Powersets;
                Array.Resize(ref powersets, DatabaseAPI.Database.Powersets.Length + 1);
                DatabaseAPI.Database.Powersets = powersets;
                _index = DatabaseAPI.Database.Powersets.Length - 1;
                DatabaseAPI.Database.Powersets[_index] = new Powerset();
            }

            if (!(!IsNew & (_index < 0)))
                DatabaseAPI.Database.Powersets[_index].ImportFromCSV(_csvString);
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
            else if ((_index < 0) | (_index > DatabaseAPI.Database.Powersets.Length - 1))
            {
                flag = true;
            }
            else if (DatabaseAPI.Database.Powersets[_index].FullName != Data.FullName)
            {
                message += $"Fullname: {DatabaseAPI.Database.Powersets[_index].FullName} => {Data.FullName}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Powersets[_index].SetName != Data.SetName)
            {
                message += $"SetName: {DatabaseAPI.Database.Powersets[_index].SetName} => {Data.SetName}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Powersets[_index].DisplayName != Data.DisplayName)
            {
                message += $"DisplayName: {DatabaseAPI.Database.Powersets[_index].DisplayName} => {Data.DisplayName}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Powersets[_index].Description != Data.Description)
            {
                message += $"Fullname: {DatabaseAPI.Database.Powersets[_index].Description} => {Data.Description}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Powersets[_index].SubName != Data.SubName)
            {
                message += $"Fullname: {DatabaseAPI.Database.Powersets[_index].SubName} => {Data.SubName}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Powersets[_index].ATClass != Data.ATClass)
            {
                message += $"Fullname: {DatabaseAPI.Database.Powersets[_index].ATClass} => {Data.ATClass}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Powersets[_index].SetType != Data.SetType)
            {
                message += $"Fullname: {DatabaseAPI.Database.Powersets[_index].SetType} => {Data.SetType}";
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