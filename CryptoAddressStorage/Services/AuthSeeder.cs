using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptoAddressStorage.Services
{
    public class AuthSeeder
    {
        public static async Task SeedRolesAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(UserRoles.BaseUser.ToString()));
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin.ToString()));
        }

        public static async Task SeedUsersAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            const string DEFAULT_PASS = "123Pa$$word.";

            var adminUser = new IdentityUser() { UserName = "admin@user.com", Email = "admin@user.com" };
            await userManager.CreateAsync(adminUser, DEFAULT_PASS);
            await userManager.AddToRoleAsync(adminUser, UserRoles.Admin.ToString());

            var baseUser = new IdentityUser() { UserName = "base@user.com", Email = "base@user.com" };
            await userManager.CreateAsync(baseUser, DEFAULT_PASS);
            await userManager.AddToRoleAsync(baseUser, UserRoles.BaseUser.ToString());
        }
    }
}
