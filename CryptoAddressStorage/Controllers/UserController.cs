using CryptoAddressStorage.Models;
using CryptoAddressStorage.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public UserController(UserManager<IdentityUser> um, ISiteRepository repo)
        {
            _userManager = um;
            _repository = repo;
        }

        [Authorize]
        [HttpGet("User/{username}")]
        public async Task<IActionResult> Profile(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var currentIdentityUser = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["FailureData"] = String.Format("User with username {0} doesn't exist!", username);
                return Redirect("~/Home/Index");
            }

            if (username == user.UserName)
            {
                return Redirect("~/Identity/Account/Manage");
            }

            var addresses = _repository.GetAddressesByUserId(user.Id).Where(a =>
            a.AccessLevel == AccessLevels.Public.ToString()
            || (a.AccessLevel == AccessLevels.FriendsOnly.ToString() && _repository.CheckFriendship(currentIdentityUser.Id, a.IdentityUserId)));

            return View(new ProfileViewModel() { IdentityUser = user, Addresses = addresses});
        }
    }
}
