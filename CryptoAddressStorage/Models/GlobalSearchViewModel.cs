using CryptoAddressStorage.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Models
{
    public class GlobalSearchViewModel
    {
        public IEnumerable<CryptoAddress> SearchAddresses { get; set; }
        public IEnumerable<IdentityUser> SearchUsers { get; set; }
    }
}
