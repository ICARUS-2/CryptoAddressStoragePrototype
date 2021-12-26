using CryptoAddressStorage.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Data
{
    public class CryptoContext : DbContext
    {
        public DbSet<CryptoAddress> Addresses { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public CryptoContext(DbContextOptions<CryptoContext> options)
            : base(options)
        {

        }
    }
}
