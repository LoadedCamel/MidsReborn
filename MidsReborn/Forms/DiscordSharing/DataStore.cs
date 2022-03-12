using System;
using System.IO;
using System.Threading.Tasks;
using LiteDB;

namespace Mids_Reborn.Forms.DiscordSharing
{
    internal class DataStore
    {
        private readonly string _path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\LoadedCamel\\MidsReborn\\dataStore.mxd";
        private readonly LiteDatabase _dataStore;

        internal DataStore()
        {
            _dataStore = Initialize();
        }

        private LiteDatabase Initialize()
        {
            var path = Directory.GetParent(_path)?.FullName;
            if (!Directory.Exists(path))
            {
                if (path != null)
                {
                    Directory.CreateDirectory(path);
                }
            }
            var dataStore = new LiteDatabase($"Filename={_path};Connection=shared");
            return dataStore;
        }

        internal async Task Repsert<T>(T item)
        {
            var dataCollection = _dataStore.GetCollection<T>(typeof(T).Name);
            if (dataCollection.Count() >= 1)
            {
                await Task.Run(() => dataCollection.DeleteAll());
            }

            await Task.Run(() => dataCollection.Insert(item));

        }

        internal async Task<T> Retrieve<T>()
        {
            var dataCollection = _dataStore.GetCollection<T>(typeof(T).Name);
            return await Task.Run(() => dataCollection.Query().FirstOrDefault());
        }

        internal void Dispose()
        {
           _dataStore.Dispose();
        }
    }
}
