using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Models.Entities
{
    public class TranslationResource
    {
        public int Id { get; set; }
        public string ResourceKey { get; set; }
        public string Text_En { get; set; }
        public string Text_Fr { get; set; }
    }
}
