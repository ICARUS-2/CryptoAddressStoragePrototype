using CryptoAddressStorage.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Data
{
    public class SiteContext : DbContext
    {
        public DbSet<CryptoAddress> Addresses { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<TranslationResource> TranslationResources { get; set; }
        public SiteContext(DbContextOptions<SiteContext> options)
            : base(options)
        {

        }
    }
}
