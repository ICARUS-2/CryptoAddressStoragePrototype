using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Models.Entities
{
    public class CryptoAddress
    {
        [Key]
        public string Id { get; set; }
        public string IdentityUserId { get; set; }
        public string PublicKey { get; set; }
        public string Coin { get; set; }

    }
}
