using AtlasTracker.Extensions;
using AtlasTracker.Models;
using AtlasTracker.Models.ViewModels;
using AtlasTracker.Services;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtlasTracker.Controllers
{
    [Authorize(Roles="Admin")]
    public class UserRolesController : Controller
    {
        private readonly IBTRolesService? _roleService;
        private readonly IBTCompanyInfoService? _companyInfoService;

        public UserRolesController(IBTRolesService? roleService, 
                                   IBTCompanyInfoService? companyInfoService)
        {
            _roleService = roleService;
            _companyInfoService = companyInfoService;
        }


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ManageUserRoles()
        {
            List<ManageUserRolesViewModel> model = new();

            int companyId = User.Identity!.GetCompanyId();

            List<AppUser> users = await _companyInfoService!.GetAllMembersAsync(companyId);

            foreach(AppUser user in users)
            {
                ManageUserRolesViewModel viewModel = new();
                viewModel.AppUser = user;
                IEnumerable<string> selected = await _roleService!.GetUserRolesAsync(user);
                viewModel.Roles = new MultiSelectList(await _roleService.GetRolesAsync(), "Name", "Name", selected);

                model.Add(viewModel);
            }


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {

            int companyId = User.Identity!.GetCompanyId();

            AppUser? appUser = (await _companyInfoService!.GetAllMembersAsync(companyId)).FirstOrDefault(u => u.Id == member.AppUser?.Id);

            IEnumerable<string> roles = await _roleService!.GetUserRolesAsync(appUser!);

            string userRole = member.SelectedRoles?.FirstOrDefault()!;

            if (!string.IsNullOrEmpty(userRole))
            {
                if(await _roleService.RemoveUserFromRolesAsync(appUser!, roles))
                {
                    await _roleService.AddUserToRoleAsync(appUser!, userRole);
                }
            }

            return RedirectToAction(nameof(ManageUserRoles));
        }
    }
}
