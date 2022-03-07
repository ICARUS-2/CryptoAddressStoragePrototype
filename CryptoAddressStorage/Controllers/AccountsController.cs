using CryptoAddressStorage.Areas.Identity.Pages.Account;
using CryptoAddressStorage.Helpers;
using CryptoAddressStorage.Models.Auth;
using CryptoAddressStorage.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly ISiteRepository _repo;
        public AccountsController(SignInManager<IdentityUser> s, UserManager<IdentityUser> u, ISiteRepository repo) : base()
        {
            signInManager = s;
            userManager = u;
            _repo = repo;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Models.Auth.RegisterModel model)
        {
            string returnUrl = UrlHelper.Generate(TempData[LanguageHelper.LANG_KEY].ToString(), "Home", "Index");

            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Username, Email = model.Email };

                var emailCheckUser = await userManager.FindByEmailAsync(user.Email);

                if (emailCheckUser != null)
                {
                    ModelState.AddModelError(string.Empty, "That email is already registered!");
                    return View();
                }

                var result = await userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await userManager.AddToRoleAsync(user, UserRoles.BaseUser.ToString());

                    /*var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                    */

                    //if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    //{
                    //return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    //}
                    //else
                    //{
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                    //}
            }

            // If we got this far, something failed, redisplay form
            return View();
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Models.Auth.LoginModel model)
        {
            string returnUrl = UrlHelper.Generate(TempData[LanguageHelper.LANG_KEY].ToString(), "Home", "Index");

            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    return Redirect("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View();
                }
            }

            return Redirect(UrlHelper.Generate(TempData[LanguageHelper.LANG_KEY].ToString(), "Home", "Index"));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
                return View("AccessDenied");

            ManageAccountModel model = new ManageAccountModel() { Email = user.Email, Username = user.UserName, Password = "", ConfirmPassword = "" };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Manage(ManageAccountModel model)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return View("AccessDenied");

            if (!ModelState.IsValid)
            {
                TempData[TempDataHelper.FAILURE] = "Model state invalid";
            }
            else
            {
                return await ConfirmAccountUpdate(model);
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ConfirmAccountUpdate(ManageAccountModel model)
        {
            StringBuilder changelog = new StringBuilder();
            IdentityUser user = await userManager.GetUserAsync(User);

            if (model.Username != user.UserName)
                changelog.AppendLine(String.Format("Username: {0} -> {1}", user.UserName, model.Username));

            if (model.Email != user.Email)
                changelog.AppendLine(String.Format("Email: {0} -> {1}", user.Email, model.Email));

            if (!string.IsNullOrWhiteSpace(model.Password))
                if (!await userManager.CheckPasswordAsync(user, model.Password))
                    changelog.AppendLine("Password changed");

            if (changelog.ToString() == String.Empty)
            {
                TempData[TempDataHelper.FAILURE] = "Account not updated because no changes were present";
                return Redirect(UrlHelper.Generate(_repo.GetSessionLanguage(), "Home", "Index"));
            }

            TempData["changelog"] = changelog.ToString();

            return View("ConfirmAccountUpdate", new ConfirmManageAccountModel() { Username = model.Username, ConfirmPassword = model.ConfirmPassword, Password = model.Password, ConfirmUpdatePassword = "", Email = model.Email});
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ConfirmAccountUpdate(ConfirmManageAccountModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData[TempDataHelper.FAILURE] = "Model state invalid";
                return Redirect(UrlHelper.Generate(_repo.GetSessionLanguage(), "Home", "Index"));
            }

            IdentityUser user = await userManager.GetUserAsync(User);

            if (!await userManager.CheckPasswordAsync(user, model.ConfirmUpdatePassword))
            {
                TempData[TempDataHelper.FAILURE] = "Password was incorrect. Account not updated.";
                return Redirect(UrlHelper.Generate(_repo.GetSessionLanguage(), "Home", "Index"));
            }

            if (model.Username != user.UserName)
                await userManager.SetUserNameAsync(user, model.Username);

            if (model.Email != user.Email)
                await userManager.SetEmailAsync(user, model.Email);

            if (!string.IsNullOrWhiteSpace(model.Password))
                if (!await userManager.CheckPasswordAsync(user, model.Password))
                    await userManager.ChangePasswordAsync(user, model.ConfirmUpdatePassword, model.Password);

            TempData[TempDataHelper.SUCCESS] = "Account updated.";
            await signInManager.RefreshSignInAsync(user);
            return Redirect(UrlHelper.Generate(_repo.GetSessionLanguage(), "Home", "Index"));
        }

        public IActionResult Lockout()
        {
            return View();
        }
    }
}
