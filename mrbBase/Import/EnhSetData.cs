namespace mrbBase.Import
{
    public class EnhSetData
    {
        /*private readonly string _csvString = string.Empty;

        public readonly EnhancementSet Data;
        private readonly int Index = -1;
        public readonly bool IsNew;
        public readonly bool IsValid = true;

        public EnhSetData(string iString)
        {
            if (string.IsNullOrEmpty(iString))
                return;
            _csvString = iString;
            Data = new EnhancementSet();
            IsValid = Data.ImportFromCSV(iString);
            IsNew = true;
            for (var index = 0; index < DatabaseAPI.Database.EnhancementSets.Count; ++index)
            {
                if (string.IsNullOrEmpty(DatabaseAPI.Database.EnhancementSets[index].Uid) ||
                    !string.Equals(DatabaseAPI.Database.EnhancementSets[index].Uid, Data.Uid,
                        StringComparison.OrdinalIgnoreCase))
                    continue;
                IsNew = false;
                Index = index;
                break;
            }
        }

        public void Apply()
        {
            if (!IsValid)
                return;
            if (IsNew)
            {
                var enhancementSet = new EnhancementSet();
                enhancementSet.ImportFromCSV(_csvString);
                DatabaseAPI.Database.EnhancementSets.Add(enhancementSet);
            }
            else if (Index > -1)
            {
                DatabaseAPI.Database.EnhancementSets[Index].ImportFromCSV(_csvString);
            }
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
            else if ((Index < 0) | (Index > DatabaseAPI.Database.EnhancementSets.Count - 1))
            {
                flag = true;
            }
            else if (DatabaseAPI.Database.EnhancementSets[Index].Uid != Data.Uid)
            {
                message += $"Uid: {DatabaseAPI.Database.EnhancementSets[Index].Uid} => {Data.Uid}";
                flag = true;
            }
            else if (DatabaseAPI.Database.EnhancementSets[Index].DisplayName != Data.DisplayName)
            {
                message +=
                    $"DisplayName: {DatabaseAPI.Database.EnhancementSets[Index].DisplayName} => {Data.DisplayName}";
                flag = true;
            }
            else if (DatabaseAPI.Database.EnhancementSets[Index].LevelMin != Data.LevelMin)
            {
                message += $"LevelMin: {DatabaseAPI.Database.EnhancementSets[Index].LevelMin} => {Data.LevelMin}";
                flag = true;
            }
            else if (DatabaseAPI.Database.EnhancementSets[Index].LevelMax != Data.LevelMax)
            {
                message += $"LevelMax: {DatabaseAPI.Database.EnhancementSets[Index].LevelMax} => {Data.LevelMax}";
                flag = true;
            }
            else if (DatabaseAPI.Database.EnhancementSets[Index].ShortName != Data.ShortName)
            {
                message += $"ShortName: {DatabaseAPI.Database.EnhancementSets[Index].ShortName} => {Data.ShortName}";
                flag = true;
            }
            else
            {
                flag = false;
            }

            return flag;
        }*/
    }
}