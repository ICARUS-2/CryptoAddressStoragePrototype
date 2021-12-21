using CryptoAddressStorage.Data;
using CryptoAddressStorage.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Services
{
    public class MainSiteRepository : ISiteRepository
    {
        public readonly CryptoContext _context;

        public MainSiteRepository(CryptoContext ctx)
        {
            _context = ctx;
        }

        public IEnumerable<CryptoAddress> GetAllAddresses()
        {
            return _context.Addresses;
        }

        public IEnumerable<CryptoAddress> GetAddressesByUserId(string userId)
        {
            return _context.Addresses.Where(a => a.IdentityUserId == userId);
        }

        public void InsertNewAddress(CryptoAddress address)
        {
            _context.Addresses.Add(address);
            _context.SaveChanges();
        }

        public CryptoAddress GetAddressById(int aId)
        {
            return _context.Addresses.Where(a => a.Id == aId).FirstOrDefault();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
