using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Models.Auth
{
    public class ConfirmManageAccountModel : ManageAccountModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmUpdatePassword { get; set; }
    }
}
