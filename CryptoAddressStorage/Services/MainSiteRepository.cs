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

        //Addresses
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

        public void RemoveAddress(CryptoAddress address)
        {
            _context.Addresses.Remove(address);
            _context.SaveChanges();
        }

        //Friends
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

        public bool AddFriendRequest(string from, string to)
        {
            if (CheckFriendship(from, to) 
                || CheckPendingFriendRequest(from, to)
                || CheckPendingFriendRequest(to, from))
                return false;

            _context.FriendRequests.Add(new FriendRequest() { From = from, To = to });
            SaveChanges();
            return true;
        }

        public bool RemoveFriendRequest(string from, string to)
        {
            if (CheckFriendship(from, to))
                return false;

            foreach (FriendRequest fr in _context.FriendRequests)
            {
                if (fr.From == from && fr.To == to)
                    _context.Remove(fr);
            }
            SaveChanges();
            return true;
        }

        public bool ConfirmFriendRequest(string from, string to)
        {
            if (CheckFriendship(from, to))
                return false;

            if (!CheckPendingFriendRequest(from, to) && !CheckPendingFriendRequest(to, from))
                return false;

            RemoveFriendRequest(from, to);
            AddFriend(from, to);

            return true;
        }

        public bool AddFriend(string userId1, string userId2)
        {
            _context.Friendships.Add(new Friendship() { Friend1 = userId1, Friend2 = userId2 });
            _context.SaveChanges();
            return true;
        }

        public bool RemoveFriend(string userId1, string userId2)
        {
            if (!CheckFriendship(userId1, userId2))
                return false;

            foreach(Friendship fr in _context.Friendships)
            {
                if ((fr.Friend1 == userId1 && fr.Friend2 == userId2)
                    || fr.Friend2 == userId1 && fr.Friend1 == userId2)
                    _context.Friendships.Remove(fr);
            }

            SaveChanges();
            return true;
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public IEnumerable<Friendship> GetUserFriendsList(string userId)
        {
            return _context.Friendships.Where(f =>
            f.Friend1 == userId || f.Friend2 == userId);
        }

        public IEnumerable<FriendRequest> GetUserSentFriendRequests(string userId)
        {
            return _context.FriendRequests.Where(fr =>
            fr.From == userId);
        }

        public IEnumerable<FriendRequest> GetUserReceivedFriendRequests(string userId)
        {
            return _context.FriendRequests.Where(fr =>
            fr.To == userId);
        }
    }
}
