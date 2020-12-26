using System;
using mrbBase.Base.Data_Classes;

namespace mrbBase.Import
{
    public class PowerData
    {
        private readonly string _csvString = string.Empty;

        public readonly IPower Data;
        public readonly bool IsValid;
        public int Index = -1;
        public bool IsNew;

        public PowerData(string iString)
        {
            if (string.IsNullOrEmpty(iString))
            {
                IsValid = false;
            }
            else
            {
                Data = new Power();
                IsValid = Data.UpdateFromCSV(iString);
                _csvString = iString;
                if (DatabaseAPI.GetPowersetByName(Data.GroupName + "." + Data.SetName) == null)
                {
                    IsValid = false;
                }
                else
                {
                    IsNew = true;
                    for (var index = 0; index < DatabaseAPI.Database.Power.Length; ++index)
                    {
                        if (string.IsNullOrEmpty(DatabaseAPI.Database.Power[index].FullName) || !string.Equals(
                            DatabaseAPI.Database.Power[index].FullName,
                            Data.FullName, StringComparison.OrdinalIgnoreCase))
                            continue;
                        IsNew = false;
                        Index = index;
                        break;
                    }
                }
            }
        }

        public void Apply()
        {
            if (!IsValid)
                return;
            if (IsNew)
            {
                var power = DatabaseAPI.Database.Power;
                Array.Resize(ref power, DatabaseAPI.Database.Power.Length + 1);
                DatabaseAPI.Database.Power = power;
                Index = DatabaseAPI.Database.Power.Length - 1;
                DatabaseAPI.Database.Power[Index] = new Power();
            }

            if (!IsNew & (Index < 0) || DatabaseAPI.Database.Power[Index].NeverAutoUpdate)
                return;
            DatabaseAPI.Database.Power[Index].IsNew = IsNew;
            DatabaseAPI.Database.Power[Index].IsModified = true;
            DatabaseAPI.Database.Power[Index].UpdateFromCSV(_csvString);
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
            else if ((Index < 0) | (Index > DatabaseAPI.Database.Power.Length - 1))
            {
                flag = true;
            }
            else if (DatabaseAPI.Database.Power[Index].FullName != Data.FullName)
            {
                message += $"Fullname: {DatabaseAPI.Database.Power[Index].FullName} => {Data.FullName}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Power[Index].GroupName != Data.GroupName)
            {
                message += $"GroupName: {DatabaseAPI.Database.Power[Index].GroupName} => {Data.GroupName}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Power[Index].SetName != Data.SetName)
            {
                message += $"SetName: {DatabaseAPI.Database.Power[Index].SetName} => {Data.SetName}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Power[Index].DisplayName != Data.DisplayName)
            {
                message += $"DisplayName: {DatabaseAPI.Database.Power[Index].DisplayName} => {Data.DisplayName}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Power[Index].Available != Data.Available)
            {
                message += $"Available: {DatabaseAPI.Database.Power[Index].Available} => {Data.Available}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Power[Index].ModesRequired != Data.ModesRequired)
            {
                message += $"ModesRequired: {DatabaseAPI.Database.Power[Index].ModesRequired} => {Data.ModesRequired}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Power[Index].ModesDisallowed != Data.ModesDisallowed)
            {
                message +=
                    $"ModesDisallowed: {DatabaseAPI.Database.Power[Index].ModesDisallowed} => {Data.ModesDisallowed}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Power[Index].PowerType != Data.PowerType)
            {
                message += $"PowerType: {DatabaseAPI.Database.Power[Index].PowerType} => {Data.PowerType}";
                flag = true;
            }
            else if (Math.Abs(DatabaseAPI.Database.Power[Index].Accuracy - Data.Accuracy) > 1.40129846432482E-45)
            {
                message += $"Accuracy: {DatabaseAPI.Database.Power[Index].Accuracy} => {Data.Accuracy}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Power[Index].AttackTypes != Data.AttackTypes)
            {
                message += $"AttackTypes: {DatabaseAPI.Database.Power[Index].AttackTypes} => {Data.AttackTypes}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Power[Index].EntitiesAutoHit != Data.EntitiesAutoHit)
            {
                message +=
                    $"EntitiesAutoHit: {DatabaseAPI.Database.Power[Index].EntitiesAutoHit} => {Data.EntitiesAutoHit}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Power[Index].EntitiesAffected != Data.EntitiesAffected)
            {
                message +=
                    $"EntitiesAffected: {DatabaseAPI.Database.Power[Index].EntitiesAffected} => {Data.EntitiesAffected}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Power[Index].Target != Data.Target)
            {
                message += $"Target: {DatabaseAPI.Database.Power[Index].Target} => {Data.Target}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Power[Index].CastFlags != Data.CastFlags)
            {
                message += $"CastFlags: {DatabaseAPI.Database.Power[Index].CastFlags} => {Data.CastFlags}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Power[Index].DescShort != Data.DescShort)
            {
                message += $"DescShort: {DatabaseAPI.Database.Power[Index].DescShort} => {Data.DescShort}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Power[Index].DescLong != Data.DescLong)
            {
                message += $"DescLong: {DatabaseAPI.Database.Power[Index].DescLong} => {Data.DescLong}";
                flag = true;
            }
            else if (Math.Abs(DatabaseAPI.Database.Power[Index].EndCost - Data.EndCost) > 1.40129846432482E-45)
            {
                message += $"EndCost: {DatabaseAPI.Database.Power[Index].EndCost} => {Data.EndCost}";
                flag = true;
            }
            else if (Math.Abs(DatabaseAPI.Database.Power[Index].ActivatePeriod - Data.ActivatePeriod) >
                     1.40129846432482E-45)
            {
                message +=
                    $"ActivatePeriod: {DatabaseAPI.Database.Power[Index].ActivatePeriod} => {Data.ActivatePeriod}";
                flag = true;
            }
            else if (Math.Abs(DatabaseAPI.Database.Power[Index].CastTimeReal - Data.CastTimeReal) >
                     1.40129846432482E-45)
            {
                message += $"CastTimeReal: {DatabaseAPI.Database.Power[Index].CastTimeReal} => {Data.CastTimeReal}";
                flag = true;
            }
            else if (DatabaseAPI.Database.Power[Index].GroupMembership.Length != Data.GroupMembership.Length)
            {
                message +=
                    $"GroupMembership.Length: {DatabaseAPI.Database.Power[Index].GroupMembership.Length} => {Data.GroupMembership.Length}";
                flag = true;
            }
            else
            {
                for (var index = 0; index <= DatabaseAPI.Database.Power[Index].GroupMembership.Length - 1; ++index)
                {
                    if (DatabaseAPI.Database.Power[Index].GroupMembership[index] == Data.GroupMembership[index])
                        continue;
                    message += string.Format("GroupMembership({2}): {0} => {1}",
                        DatabaseAPI.Database.Power[Index].GroupMembership[index], Data.GroupMembership[index], index);
                    return true;
                }

                if (DatabaseAPI.Database.Power[Index].MaxTargets != Data.MaxTargets)
                {
                    message += $"MaxTargets: {DatabaseAPI.Database.Power[Index].MaxTargets} => {Data.MaxTargets}";
                    flag = true;
                }
                else if (Math.Abs(DatabaseAPI.Database.Power[Index].Range - Data.Range) > 1.40129846432482E-45)
                {
                    message += $"Range: {DatabaseAPI.Database.Power[Index].Range} => {Data.Range}";
                    flag = true;
                }
                else if (Math.Abs(DatabaseAPI.Database.Power[Index].Radius - Data.Radius) > 1.40129846432482E-45)
                {
                    message += $"Radius: {DatabaseAPI.Database.Power[Index].Radius} => {Data.Radius}";
                    flag = true;
                }
                else if (Math.Abs(DatabaseAPI.Database.Power[Index].RechargeTime - Data.RechargeTime) >
                         1.40129846432482E-45)
                {
                    message += $"RechargeTime: {DatabaseAPI.Database.Power[Index].RechargeTime} => {Data.RechargeTime}";
                    flag = true;
                }
                else if (DatabaseAPI.Database.Power[Index].BoostsAllowed.Length != Data.BoostsAllowed.Length)
                {
                    message +=
                        $"BoostsAllowed.Length: {DatabaseAPI.Database.Power[Index].BoostsAllowed.Length} => {Data.BoostsAllowed.Length}";
                    flag = true;
                }
                else
                {
                    for (var index = 0; index <= DatabaseAPI.Database.Power[Index].BoostsAllowed.Length - 1; ++index)
                    {
                        if (DatabaseAPI.Database.Power[Index].BoostsAllowed[index] == Data.BoostsAllowed[index])
                            continue;
                        message += string.Format("BoostsAllowed({2}): {0} => {1}",
                            DatabaseAPI.Database.Power[Index].BoostsAllowed[index], Data.BoostsAllowed[index], index);
                        return true;
                    }

                    if (DatabaseAPI.Database.Power[Index].BoostBoostable != Data.BoostBoostable)
                    {
                        message +=
                            $"BoostBoostable: {DatabaseAPI.Database.Power[Index].BoostBoostable} => {Data.BoostBoostable}";
                        flag = true;
                    }
                    else if (DatabaseAPI.Database.Power[Index].BoostUsePlayerLevel != Data.BoostUsePlayerLevel)
                    {
                        message +=
                            $"BoostUsePlayerLevel: {DatabaseAPI.Database.Power[Index].BoostUsePlayerLevel} => {Data.BoostUsePlayerLevel}";
                        flag = true;
                    }
                    else if (DatabaseAPI.Database.Power[Index].IgnoreEnh.Length != Data.IgnoreEnh.Length)
                    {
                        message +=
                            $"IgnoreEnh: {DatabaseAPI.Database.Power[Index].IgnoreEnh.Length} => {Data.IgnoreEnh.Length}";
                        flag = true;
                    }
                    else
                    {
                        flag = false;
                    }
                }
            }

            return flag;
        }
    }
}