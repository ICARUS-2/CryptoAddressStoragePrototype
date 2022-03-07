using CryptoAddressStorage.Helpers;
using CryptoAddressStorage.Models;
using CryptoAddressStorage.Models.Entities;
using CryptoAddressStorage.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ISiteRepository _repository;

        public HomeController(UserManager<IdentityUser> um ,SignInManager<IdentityUser> sm, ISiteRepository repo)
        {
            _userManager = um;
            _signInManager = sm;
            _repository = repo;
        }

        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                //Returns the homepage with a list of the user's addresses.
                var user = await _userManager.GetUserAsync(User);
                return View(_repository.GetAddressesByUserId(user.Id));
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        [Authorize]
        public async Task<IActionResult> GlobalSearch(IFormCollection form)
        {
            string searchQuery = form["search-query"];

            var currentIdentityUser = await _userManager.GetUserAsync(User);

            if (currentIdentityUser == null)
                return View("Error", new ErrorViewModel());

            if (searchQuery == null || searchQuery == string.Empty)
                return View(new GlobalSearchViewModel() { SearchAddresses = new List<CryptoAddress>(), SearchUsers = new List<IdentityUser>()});

            ViewBag.SearchData = searchQuery;

            var allUsers = _userManager.Users;
            var usersBySearchQuery = _userManager.Users.Where(u => u.UserName.ToLower().Contains(searchQuery.ToLower()) && u.Id != currentIdentityUser.Id);

            //All of the addresses that do not have their access level set to Private
            var nonPrivateAddresses = _repository.GetAllAddresses().Where(a => a.AccessLevel != AccessLevels.Private.ToString() && a.IdentityUserId != currentIdentityUser.Id);
            
            //All of the above addresses that either contain the search query in their public key or their username
            var nonPrivateAddressesMatchingQuery = nonPrivateAddresses.Where(a => 
            a.PublicKey.ToLower().Contains(searchQuery.ToLower()) 
            || a.Title.ToLower().Contains(searchQuery.ToLower())
            || allUsers.Where(u => u.Id == a.IdentityUserId).FirstOrDefault().UserName.ToLower().Contains(searchQuery.ToLower()));
            
            //All of the above addresses that are either public or have their access level set to FriendsOnly where the current user is friends with the user of that address
            var friendsOnlyFilteredAddresses = nonPrivateAddressesMatchingQuery.Where(a =>
            a.AccessLevel == AccessLevels.Public.ToString()
            || (a.AccessLevel == AccessLevels.FriendsOnly.ToString() && _repository.CheckFriendship(currentIdentityUser.Id, a.IdentityUserId)));

            return View(new GlobalSearchViewModel { SearchAddresses = friendsOnlyFilteredAddresses, SearchUsers = usersBySearchQuery});
        }
    }
}
