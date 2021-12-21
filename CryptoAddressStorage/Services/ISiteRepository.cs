using CryptoAddressStorage.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Services
{
    public interface ISiteRepository
    {
        public IEnumerable<CryptoAddress> GetAllAddresses();
        public void InsertNewAddress(CryptoAddress address);
        public IEnumerable<CryptoAddress> GetAddressesByUserId(string userId);
        public CryptoAddress GetAddressById(int aId);
        public void SaveChanges();
    }
}
