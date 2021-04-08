using Microsoft.AspNetCore.Identity;
using BYUFagElGamous1_5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BYUFagElGamous1_5.Helpers
{
    public static class ClaimsHelper
    {
        //Extension method takes in a list of available permissions, the type of permission (for instance ProductPermissions) to be added, and Role Id.
        //It then adds all the properties mentioned in the ProductPermissions using Reflection. This is a simple helper class that can be further optimized.
        public static void GetPermissions(this List<RoleClaimsViewModel> allPermissions, Type policy, string roleId)
        {
            FieldInfo[] fields = policy.GetFields(BindingFlags.Static | BindingFlags.Public);

            foreach (FieldInfo fi in fields)
            {
                allPermissions.Add(new RoleClaimsViewModel { Value = fi.GetValue(null).ToString(), Type = "Permissions" });
            }
        }
        // Remember that we have already used this while seeding permissions for the SuperAdmin Role? I am just duplicating it here for better understanding.
        // Putting the code into words, this extension method is responsible for adding the selected claims from the UI to the user role.
        public static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string permission)
        {
            var allClaims = await roleManager.GetClaimsAsync(role);
            if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
            {
                await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }
        }
    }
}
