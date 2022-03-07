using CryptoAddressStorage.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Services
{
    public interface ISiteRepository
    {
        //Addresses
        public IEnumerable<CryptoAddress> GetAllAddresses();
        public void InsertNewAddress(CryptoAddress address);
        public IEnumerable<CryptoAddress> GetAddressesByUserId(string userId);
        public CryptoAddress GetAddressById(int aId);
        public void SaveChanges();
        public void RemoveAddress(CryptoAddress address);

        //Friends
        public bool CheckPendingFriendRequest(string fromId, string toId);
        public bool CheckFriendship(string userId1, string userId2);
        public bool AddFriendRequest(string from, string to);
        public bool RemoveFriendRequest(string from, string to);
        public bool ConfirmFriendRequest(string from, string to);
        public bool RemoveFriend(string userId1, string userId2);
        public IEnumerable<Friendship> GetUserFriendsList(string userId);
        public IEnumerable<FriendRequest> GetUserSentFriendRequests(string userId);
        public IEnumerable<FriendRequest> GetUserReceivedFriendRequests(string userId);

        //Globalization
        public TranslationResource GetTranslationResource(string key);
        public string GetTranslation(string key, string lang);
        public string GetTranslation(string key);
        public void ClearAllTranslations();
        public void AddTranslationResource(TranslationResource resource);
        public string GetSessionLanguage();
        public void SetSessionLanguage(string lang);
        public string GetRawRoute();
        public void SetRawRoute(string route);
    }
}
