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

        public void RemoveAddress(CryptoAddress address)
        {
            _context.Addresses.Remove(address);
            _context.SaveChanges();
        }

        public bool CheckFriendship(string user1, string user2)
        {
            List<Friendship> friendships = _context.Friendships
                .Where(fr =>
                (fr.Friend1 == user1 && fr.Friend2 == user2)
                || (fr.Friend1 == user2 && fr.Friend2 == user1))
                .ToList();

            return friendships.Count != 0;
        }

        public bool CheckPendingFriendRequest(string from, string to)
        {
            List<FriendRequest> friendRequests = _context.FriendRequests.Where(fr =>
            fr.From == from && fr.To == to).ToList();

            return friendRequests.Count != 0;
        }
    }
}
