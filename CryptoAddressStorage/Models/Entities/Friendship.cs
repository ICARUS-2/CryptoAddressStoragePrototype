using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Models.Entities
{
    public class Friendship
    {
        [Key]
        public int Id { get; set; }
        public string Friend1 { get; set; }
        public string Friend2 { get; set; }
    }
}
