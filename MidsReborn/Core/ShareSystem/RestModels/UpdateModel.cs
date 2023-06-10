using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mids_Reborn.Core.ShareSystem.RestModels
{
    internal class UpdateModel
    {
        public string Code { get; set; }
        public string PageData { get; set; }

        public UpdateModel(string code, string pageData)
        {
            Code = code;
            PageData = pageData;
        }
    }
}
