using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.ShareSystem
{
    internal class ExpiringCollection
    {
        private List<ExpiringCollectionItem>? _items = new();
        private readonly string _dateFormat = "MM/dd/yyyy hh:mm tt"; // Shared date format
        private readonly TimeZoneInfo _userTimeZone = TimeZoneInfo.Local; // Assuming the system's time zone is the user's time zone
        private readonly string[] _dateFormats = { "MM/dd/yyyy hh:mm tt 'CST'", "MM/dd/yyyy hh:mm tt 'CDT'" };
        private readonly TimeZoneInfo _centralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
        private readonly string _sharedPath = Path.Combine(AppContext.BaseDirectory, "sharedBuilds.json");

        public void Add(ExpiringCollectionItem item)
        {
            if (IsExpired(item)) return;
            item.SharedOn = GetCurrentTimeInUserTimeZone();
            _items?.Add(item);
            SerializeToJson();
        }

        public void Update(ExpiringCollectionItem newItem)
        {
            var existingItem = _items?.FirstOrDefault(item => item.Id == newItem.Id);
            if (existingItem == null) return;
            existingItem.Name = newItem.Name;
            existingItem.Archetype = newItem.Archetype;
            existingItem.Description = newItem.Description;
            existingItem.Primary = newItem.Primary;
            existingItem.Secondary = newItem.Secondary;
            existingItem.ExpiresAt = newItem.ExpiresAt;
            SerializeToJson();
        }

        private string GetCurrentTimeInUserTimeZone()
        {
            var now = DateTime.UtcNow;
            var userTime = TimeZoneInfo.ConvertTimeFromUtc(now, _userTimeZone);
            return userTime.ToString(_dateFormat);
        }

        private void SerializeToJson()
        {
            var json = JsonConvert.SerializeObject(_items, Formatting.Indented);
            File.WriteAllText(_sharedPath, json);
        }

        public void DeserializeFromJson()
        {
            if (!File.Exists(_sharedPath)) return;
            _items = JsonConvert.DeserializeObject<List<ExpiringCollectionItem>>(File.ReadAllText(_sharedPath));
            RemoveExpiredItems();
        }

        public IEnumerable<ExpiringCollectionItem>? GetItems()
        {
            RemoveExpiredItems();
            return _items;
        }

        private void RemoveExpiredItems()
        {
            _items?.RemoveAll(IsExpired);
        }

        private bool IsExpired(ExpiringCollectionItem item)
        {
            foreach (var format in _dateFormats)
            {
                if (!DateTime.TryParseExact(item.ExpiresAt, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var localDateTime)) continue;
                localDateTime = DateTime.SpecifyKind(localDateTime, DateTimeKind.Unspecified);
                var utcDateTime = TimeZoneInfo.ConvertTimeToUtc(localDateTime, _centralTimeZone);
                if (utcDateTime <= DateTime.UtcNow)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
