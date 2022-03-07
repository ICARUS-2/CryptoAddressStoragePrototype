using CryptoAddressStorage.Helpers;
using CryptoAddressStorage.Models;
using CryptoAddressStorage.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Controllers
{
    public class FriendsController : Controller
    {
        private UserManager<IdentityUser> _userManager;
        private ISiteRepository _repository;

        public FriendsController(UserManager<IdentityUser> um, ISiteRepository repo)
        {
            _userManager = um;
            _repository = repo;
        }

        [Authorize]
        public IActionResult Index()
        {
            return Redirect(UrlHelper.Generate(_repository.GetSessionLanguage(), "Friends", "List"));
        }

        [Authorize]
        public async Task<IActionResult> List()
        {
            var user = await _userManager.GetUserAsync(User);

            var userFriendsList = _repository.GetUserFriendsList(user.Id);
            var sentFriendRequests = _repository.GetUserSentFriendRequests(user.Id);
            var receivedFriendRequests = _repository.GetUserReceivedFriendRequests(user.Id);

            return View(new FriendDataViewModel() { 
                FriendsList = userFriendsList,
                SentFriendRequests = sentFriendRequests,
                ReceivedFriendRequests = receivedFriendRequests});
        }

        [Authorize]
        [HttpPost("{language}/Friends/Remove")]
        public async Task<IActionResult> Remove(IFormCollection form)
        {
            string toId = form["ToId"];
            string redirect = form["Redirect"];

            IdentityUser currentUser = await _userManager.GetUserAsync(User);
            IdentityUser toUser = await _userManager.FindByIdAsync(toId);

            if (toUser == null)
            {
                TempData[TempDataHelper.FAILURE] = String.Format("Cannot remove friend {0}: No user exists", toId);
                return Redirect(UrlHelper.Generate(_repository.GetSessionLanguage(), "Home", "Index"));
            }

            if (_repository.RemoveFriend(currentUser.Id, toId))
            {
                TempData[TempDataHelper.SUCCESS] = String.Format("Successfully removed friend {0}", toUser.UserName);
            }
            else
            {
                TempData[TempDataHelper.FAILURE] = String.Format("An unknown error occured while removing friend {0}", toUser.UserName);
            }

            return Redirect(redirect);
        }

        [Authorize]
        [HttpPost("{language}/Friends/Requests/Send")]
        public async Task<IActionResult> Send(IFormCollection form)
        {
            string toId = form["ToId"];
            string redirect = form["Redirect"];

            IdentityUser currentUser = await _userManager.GetUserAsync(User);
            IdentityUser toUser = await _userManager.FindByIdAsync(toId);

            if (toUser == null)
            {
                TempData[TempDataHelper.FAILURE] = String.Format("Cannot send friend request to user ID {0}: No user exists", toId);
                return Redirect(UrlHelper.Generate(_repository.GetSessionLanguage(), "Home", "Index"));
            }

            if (_repository.AddFriendRequest(currentUser.Id, toId))
            {
                TempData[TempDataHelper.SUCCESS] = String.Format("Successfully sent friend request to {0}", toUser.UserName);
            }
            else
            {
                TempData[TempDataHelper.FAILURE] = String.Format("An unknown error occured while sending friend request to {0}", toUser.UserName);
            }

            return Redirect(redirect);
        }

        [Authorize]
        [HttpPost("{language}/Friends/Requests/Accept")]
        public async Task<IActionResult> Accept(IFormCollection form)
        {
            string fromId = form["fromId"];
            string redirect = form["Redirect"];

            IdentityUser currentUser = await _userManager.GetUserAsync(User);
            IdentityUser fromUser = await _userManager.FindByIdAsync(fromId);

            if (fromUser == null)
            {
                TempData[TempDataHelper.FAILURE] = String.Format("Cannot accept friend request from user ID {0}: No user exists", fromId);
                return Redirect(UrlHelper.Generate(_repository.GetSessionLanguage(), "Home", "Index"));
            }

            if (_repository.ConfirmFriendRequest(fromId, currentUser.Id))
            {
                TempData[TempDataHelper.SUCCESS] = String.Format("Successfully accepted friend request from {0}", fromUser.UserName);
            }
            else
            {
                TempData[TempDataHelper.FAILURE] = String.Format("An unknown error occured while accepting friend request from {0}", fromUser.UserName);
            }

            return Redirect(redirect);
        }

        [Authorize]
        [HttpPost("{language}/Friends/Requests/DontAccept")]
        public async Task<IActionResult> DontAccept(IFormCollection form)
        {
            string fromId = form["fromId"];
            string redirect = form["Redirect"];

            IdentityUser currentUser = await _userManager.GetUserAsync(User);
            IdentityUser fromUser = await _userManager.FindByIdAsync(fromId);

            if (fromUser == null)
            {
                TempData[TempDataHelper.FAILURE] = String.Format("Cannot deny friend request from user ID {0}: No user exists", fromId);
                return Redirect(UrlHelper.Generate(_repository.GetSessionLanguage(), "Home", "Index"));
            }

            if (_repository.RemoveFriendRequest(fromId, currentUser.Id))
            {
                TempData[TempDataHelper.SUCCESS] = String.Format("Successfully denied friend request from {0}", fromUser.UserName);
            }
            else
            {
                TempData[TempDataHelper.FAILURE] = String.Format("An unknown error occured while denying friend request from {0}", fromUser.UserName);
            }

            return Redirect(redirect);
        }

        [Authorize]
        [HttpPost("{language}/Friends/Requests/Cancel")]
        public async Task<IActionResult> Cancel(IFormCollection form)
        {
            string toId = form["ToId"];
            string redirect = form["Redirect"];

            IdentityUser currentUser = await _userManager.GetUserAsync(User);
            IdentityUser toUser = await _userManager.FindByIdAsync(toId);

            if (toUser == null)
            {
                TempData[TempDataHelper.FAILURE] = String.Format("Cannot cancel friend request to user ID {0}: No user exists", toId);
                return Redirect(UrlHelper.Generate(_repository.GetSessionLanguage(), "Home", "Index"));
            }

            if (_repository.RemoveFriendRequest(currentUser.Id, toId))
            {
                TempData[TempDataHelper.SUCCESS] = String.Format("Successfully cancelled friend request to {0}", toUser.UserName);
            }
            else
            {
                TempData[TempDataHelper.FAILURE] = String.Format("An unknown error occured while cancelling friend request to {0}", toUser.UserName);
            }

            return Redirect(redirect);
        }
    }
}
