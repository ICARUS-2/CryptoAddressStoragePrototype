using CryptoAddressStorage.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Models
{
    public class FriendDataViewModel
    {
        public IEnumerable<FriendRequest> SentFriendRequests;
        public IEnumerable<FriendRequest> ReceivedFriendRequests;
        public IEnumerable<Friendship> FriendsList;

        public FriendDataViewModel()
        {

        }
    }
}
