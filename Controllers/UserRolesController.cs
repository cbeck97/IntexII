using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BYUFagElGamous1_5.Constants;
using BYUFagElGamous1_5.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BYUFagElGamous1_5.Controllers
{
    [Authorize(Roles = "SuperAdmin, Admin")]
    public class UserRolesController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        //Here, we cycle through each of the available roles and check if the selected user is already in the corresponding role.
        //If the user is not in the iterated role, we change the status of the Selected boolean to false.
        //We would go on later in the View to uncheck the roles in list based on this algorithm.
        //Finally, we return the model to the View.
        public async Task<IActionResult> Index(string userId)
        {
            var viewModel = new List<UserRolesViewModel>();
            var user = await _userManager.FindByIdAsync(userId);
            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
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
                viewModel.Add(userRolesViewModel);
            }
            var model = new ManageUserRolesViewModel()
            {
                UserId = userId,
                UserRoles = viewModel
            };

            return View(model);
        }
        //we first remove all the existing roles from the user.And finally, we re-add the roles which are selected in UI when the user clicks Save.
        public async Task<IActionResult> Update(string id, ManageUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(id);
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            result = await _userManager.AddToRolesAsync(user, model.UserRoles.Where(x => x.Selected).Select(y => y.RoleName));
            var currentUser = await _userManager.GetUserAsync(User);

            await _signInManager.RefreshSignInAsync(currentUser);
            //This is just a fallback code in case one admin tries to change the roles of another
            await Seeds.SeedUsers.SeedSuperAdminAsync(_userManager, _roleManager);

            return RedirectToAction("Index", new { userId = id });
        }
    }
}