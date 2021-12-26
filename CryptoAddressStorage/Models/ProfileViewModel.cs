using CryptoAddressStorage.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Models
{
    public class ProfileViewModel
    {
        public IdentityUser IdentityUser { get; set; }
        public IEnumerable<CryptoAddress> Addresses { get;set;}
    }
}
