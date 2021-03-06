using CryptoAddressStorage.Helpers;
using CryptoAddressStorage.Models.Entities;
using CryptoAddressStorage.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Controllers
{
    public class AddressController : Controller
    {
        UserManager<IdentityUser> _userManager;
        ISiteRepository _repo;

        public AddressController(UserManager<IdentityUser> userManager, ISiteRepository repo)
        {
            _userManager = userManager;
            _repo = repo;
        }

        [HttpGet("{language}/Address/New")]
        [Authorize]
        public IActionResult New()
        {
            return View();
        }

        [HttpPost("{language}/Address/New")]
        [Authorize]
        public async Task <IActionResult> New(IFormCollection form)
        {
            int coinType = int.Parse(form["CoinType"]);
            string pubKey = form["Address"];
            int accessLevel = int.Parse(form["AccessLevel"]);
            string title = form["Name"];

            var user = await _userManager.GetUserAsync(User);

            IEnumerable<CryptoAddress> addressByUserIdAndKey = _repo.GetAddressesByUserId(user.Id).Where(a => a.PublicKey == pubKey).Where(a => a.Coin == ((CoinType)coinType).ToString());
            if (addressByUserIdAndKey.Count() > 0)
            {
                TempData[TempDataHelper.FAILURE] = String.Format("{0} address {1} already exists in your profile.", ((CoinType)coinType).ToString(), pubKey);
                return Redirect(UrlHelper.Generate(_repo.GetSessionLanguage(), "Home", "Index"));
            }

            CryptoAddress newAddress = new CryptoAddress()
            {
                IdentityUserId = user.Id,
                PublicKey = pubKey,
                Coin = ((CoinType)coinType).ToString(),
                AccessLevel = ((AccessLevels)accessLevel).ToString(),
                Title = title == null ? "-" : title
            };

            if (newAddress.Format.Contains("Unknown"))
            {
                TempData[TempDataHelper.FAILURE] = "Address format invalid. No address has been created.";
            }
            else
            {
                TempData[TempDataHelper.SUCCESS] = String.Format("{0} {1} address {2} added successfully", newAddress.Format, newAddress.Coin, newAddress.PublicKey);

                _repo.InsertNewAddress(newAddress);
            }

            return Redirect(UrlHelper.Generate(_repo.GetSessionLanguage(), "Home", "Index"));
        }

        [HttpGet("{language}/Address/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            CryptoAddress address = _repo.GetAddressById(id);

            if (address == null)
            {
                TempData[TempDataHelper.FAILURE] = "Cannot edit address: No address exists with ID" + id;
                return Redirect(UrlHelper.Generate(_repo.GetSessionLanguage(), "Home", "Index"));
            }    

            var user = await _userManager.GetUserAsync(User);

            if (user.Id != address.IdentityUserId)
            {
                TempData[TempDataHelper.FAILURE] = "Cannot edit address: Access Denied";
                return Redirect(UrlHelper.Generate(_repo.GetSessionLanguage(), "Home", "Index"));
            }

            ViewBag.SelectedCoin = (int)((CoinType)Enum.Parse(typeof(CoinType), address.Coin));
            ViewBag.SelectedAccess = (int)((AccessLevels)Enum.Parse(typeof(AccessLevels), address.AccessLevel));
            ViewBag.PublicKey = address.PublicKey;
            ViewBag.AddressName = address.Title;

            return View(address);
        }

        [HttpPost("{language}/Address/Edit/{id}")]
        [Authorize]
        public async Task<IActionResult> Edit(int id, IFormCollection form)
        {
            int currency = int.Parse(form["CoinType"]);
            string addressTitle = form["Name"];
            string publicKey = form["Address"];
            int accessLevel = int.Parse(form["AccessLevel"]);

            CryptoAddress address = _repo.GetAddressById(id);

            if (address == null)
            {
                TempData[TempDataHelper.FAILURE] = "Cannot edit address: No address exists with ID" + id;
                return Redirect(UrlHelper.Generate(_repo.GetSessionLanguage(), "Home", "Index"));
            }

            var user = await _userManager.GetUserAsync(User);

            if (user.Id != address.IdentityUserId)
            {
                TempData[TempDataHelper.FAILURE] = "Cannot edit address: Access Denied";
                return Redirect(UrlHelper.Generate(_repo.GetSessionLanguage(), "Home", "Index"));
            }

            if (CryptoAddress.GetFormat(publicKey, ((CoinType)currency).ToString()).Contains("Unknown"))
            {
                TempData[TempDataHelper.FAILURE] = "Cannot edit address: Invalid Format";
                return Redirect(UrlHelper.Generate(_repo.GetSessionLanguage(), "Home", "Index"));
            }

            StringBuilder changelog = new StringBuilder();

            if (((CoinType)currency).ToString() != address.Coin)
            {
                changelog.Append(String.Format("<br>Coin: {0} -> {1}", address.Coin, ((CoinType)currency).ToString()));
            }

            if (addressTitle != address.Title)
            {
                changelog.Append(String.Format("<br>Name: {0} -> {1}", address.Title, addressTitle));
            }

            if (publicKey != address.PublicKey)
            {
                changelog.Append(String.Format("<br>Key: {0} -> {1}", address.PublicKey, publicKey));
            }

            if (((AccessLevels)accessLevel).ToString() != address.AccessLevel)
            {
                changelog.Append(String.Format("<br>Access Level: {0} -> {1}", address.AccessLevel, ((AccessLevels)accessLevel).ToString()));
            }

            address.Coin = ((CoinType)currency).ToString();
            address.Title = addressTitle;
            address.PublicKey = publicKey;
            address.AccessLevel = ((AccessLevels)accessLevel).ToString();

            if (changelog.ToString().Length == 0)
            {
                TempData[TempDataHelper.WARNING] = "Edit succeeded, but no changes were present.";
            }
            else
            {
                TempData[TempDataHelper.SUCCESS] = "Address modification succeeded with the following changes:" + changelog.ToString();
            }
            
            _repo.SaveChanges();

            return Redirect(UrlHelper.Generate(_repo.GetSessionLanguage(), "Home", "Index"));
        }

        [HttpPost("{language}/Address/Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, IFormCollection form)
        {
            var user = await _userManager.GetUserAsync(User);
            var address = _repo.GetAddressById(id);

            if (address == null)
            {
                TempData[TempDataHelper.FAILURE] = "Cannot edit address: No address exists with ID" + id;
                return Redirect(UrlHelper.Generate(_repo.GetSessionLanguage(), "Home", "Index"));
            }

            if (user.Id != address.IdentityUserId)
            {
                TempData[TempDataHelper.FAILURE] = "Cannot delete address: Access Denied";
                return Redirect(UrlHelper.Generate(_repo.GetSessionLanguage(), "Home", "Index"));
            }

            _repo.RemoveAddress(address);
            TempData[TempDataHelper.SUCCESS] = String.Format("Successfully deleted {0} address {1}", address.Coin, address.PublicKey);

            return Redirect(UrlHelper.Generate(_repo.GetSessionLanguage(), "Home", "Index"));
        }

        [HttpGet("{language}/Address/Format/{query}")]
        public object Format(string query)
        {
            string[] parameters = query.Split(',');

            string publicKey = parameters[0].Split('=')[1];
            int coinTypeInt = int.Parse(parameters[1].Split('=')[1]);

            string coinType = ((CoinType)coinTypeInt).ToString();

            string fmt = CryptoAddress.GetFormat(publicKey, coinType);

            return fmt;
        }
    }
}
