using CryptoAddressStorage.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagerController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UserManagerController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(IFormCollection form)
        {
            string searchQuery = form["search-query"];

            var users = await _userManager.Users.ToListAsync();

            if (searchQuery != null && searchQuery != string.Empty)
            {
                users = users.Where(u => u.Id.ToLower().Contains(searchQuery.ToLower()) || u.Email.ToLower().Contains(searchQuery.ToLower()) || u.UserName.ToLower().Contains(searchQuery.ToLower())).ToList();
                ViewBag.SearchData = searchQuery;
            }

            var userRolesViewModelList = new List<UserRolesViewModel>();
            foreach (IdentityUser user in users)
            {
                var vModel = new UserRolesViewModel();
                vModel.UserId = user.Id;
                vModel.UserName = user.UserName;
                vModel.Email = user.Email;
                vModel.Roles = await GetUserRoles(user);
                userRolesViewModelList.Add(vModel);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            ViewBag.SuccessData = TempData["SuccessData"];
            ViewBag.FailureData = TempData["FailureData"];

            return View("Index", userRolesViewModelList.Where(v => v.UserId != currentUser.Id));
        }

        [Authorize(Roles = "Admin")]
        private async Task<List<string>> GetUserRoles(IdentityUser user)
        {
            return new List<string>(await _userManager.GetRolesAsync(user));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageRoles(string userId)
        {
            ViewBag.userId = userId;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            ViewBag.UserName = user.UserName;
            var model = new List<ManageUserRolesViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new ManageUserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ManageRoles(List<ManageUserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View();
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            result = await _userManager.AddToRolesAsync(user, model.Where(x => x.Selected).Select(y => y.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            TempData["SuccessData"] = String.Format("Successfully updated roles for user {0}/{1}/{2}:  {3} --> {4}", user.Id, user.Email, user.UserName, string.Join(", ", roles.ToList()), string.Join(", ", model.Where(x => x.Selected).Select(y => y.RoleName)));
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Ban(IFormCollection form)
        {
            var user = await _userManager.FindByIdAsync(form["Id"]);

            if (user.LockoutEnd != null)
            {
                TempData["FailureData"] = String.Format("Ban failed for user {0}/{1}/{2} : User is already banned", user.Id, user.Email, user.UserName);
            }
            else
            {
                var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);

                if (result.Succeeded)
                {
                    TempData["SuccessData"] = String.Format("Successfully banned user {0}/{1}/{2}", user.Id, user.Email, user.UserName);   
                }
                else
                {
                    TempData["FailureData"] = String.Format("Ban failed for user {0}/{1}/{2} because an error occurred", user.Id, user.Email, user.UserName);
                }
            }
               
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Unban(IFormCollection form)
        {
            var user = await _userManager.FindByIdAsync(form["Id"]);

            if (user.LockoutEnd == null)
            {
                TempData["FailureData"] = String.Format("Unban failed for user {0}/{1}/{2} : User was not already banned.", user.Id, user.Email, user.UserName);
            }
            else
            {
                var result = await _userManager.SetLockoutEndDateAsync(user, null);

                if (result.Succeeded)
                {
                    TempData["SuccessData"] = String.Format("Successfully unbanned user {0}/{1}/{2}", user.Id, user.Email, user.UserName);
                }
                else
                {
                    TempData["FailureData"] = String.Format("Unban failed for user {0}/{1}/{2} because an error occurred", user.Id, user.Email, user.UserName);
                }
            }

            return RedirectToAction("Index");
        }
    }
}
