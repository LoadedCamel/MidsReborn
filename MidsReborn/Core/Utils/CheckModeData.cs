using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.Utils
{
    public class CheckModeData<T> : List<T> where T : struct
    {
        public CheckModeData() { }

        public byte[] SerializeToBytes()
        {
            var serializedData = JsonConvert.SerializeObject(this, Formatting.Indented);
            return Encoding.UTF8.GetBytes(serializedData);
        }
    }
}
