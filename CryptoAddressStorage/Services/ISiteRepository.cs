﻿using CryptoAddressStorage.Models.Entities;
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
        public void RemoveAddress(CryptoAddress address);
        public bool CheckPendingFriendRequest(string from, string to);
        public bool CheckFriendship(string user1, string user2);
    }
}
