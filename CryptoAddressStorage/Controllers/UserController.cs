using CryptoAddressStorage.Helpers;
using CryptoAddressStorage.Models;
using CryptoAddressStorage.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Controllers
{
    public class UserController : Controller
    {
        public UserManager<IdentityUser> _userManager;
        public ISiteRepository _repository;

        public UserController(UserManager<IdentityUser> userManager, ISiteRepository repo)
        {
            _userManager = userManager;
            _repository = repo;
        }

        [Authorize]
        [HttpGet("{language}/User/{username}")]
        public async Task<IActionResult> Profile(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var currentIdentityUser = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData[TempDataHelper.FAILURE] = String.Format("User with username {0} doesn't exist!", username);
                return Redirect(UrlHelper.Generate(_repository.GetSessionLanguage(), "Home", "Index"));
            }

            if (username.ToLower() == currentIdentityUser.UserName.ToLower())
            {
                return Redirect(UrlHelper.Generate(_repository.GetSessionLanguage(), "Accounts", "Manage"));
            }

            var addresses = _repository.GetAddressesByUserId(user.Id).Where(a =>
            a.AccessLevel == AccessLevels.Public.ToString()
            || (a.AccessLevel == AccessLevels.FriendsOnly.ToString() && _repository.CheckFriendship(currentIdentityUser.Id, a.IdentityUserId)));

            return View(new ProfileViewModel() { IdentityUser = user, Addresses = addresses});
        }
    }
}
