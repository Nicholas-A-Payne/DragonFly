#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AtlasTracker.Data;
using AtlasTracker.Models;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using AtlasTracker.Extensions;
using Microsoft.AspNetCore.Authorization;
using AtlasTracker.Models.ViewModels;
using AtlasTracker.Models.Enum;
using AtlasTracker.Services;

namespace AtlasTracker.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IBTProjectService _projectService;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly IBTRolesService _rolesService;
        private readonly IBTLookUpService _lookUpService;
        private readonly IBTFileService _fileService;

        public ProjectsController(IBTProjectService projectService,
                                  UserManager<AppUser> userManager,
                                  IBTRolesService rolesService,
                                  IBTLookUpService lookUpService,
                                  IBTCompanyInfoService companyInfoService,
                                  IBTFileService fileService, 
                                  ApplicationDbContext context)
        {
            //_context = context;
            _projectService = projectService;
            _userManager = userManager;
            _rolesService = rolesService;
            _lookUpService = lookUpService;
            _companyInfoService = companyInfoService;
            _fileService = fileService;
            _context = context;
        }


        public async Task<IActionResult> MyProjects()
        {
            string userId = _userManager.GetUserId(User);
            List<Project> project = await _projectService.GetUserProjectsAsync(userId);

            return View(project);
        }

        public async Task<IActionResult> AllProjects()
        {
            List<Project> projects = new();
            int companyId = User.Identity.GetCompanyId();

            if (User.IsInRole(nameof(AppRole.Admin))|| User.IsInRole(nameof(AppRole.ProjectManager)))
            {
                projects = await _companyInfoService.GetAllProjectsAsync(companyId); 
            }
            else
            {
                projects = await _projectService.GetAllProjectsByCompanyAsync(companyId);
            }

            return View(projects);
        }


        //Archived Projects
        public async Task<IActionResult> ArchivedProjects()
        {
            int companyId = User.Identity.GetCompanyId();
            List<Project> projects = await _projectService.GetArchivedProjectsByCompany(companyId);

            return View(projects);
        }

        //Unassigned Projects
        public async Task<IActionResult> UnassignedProjects()
        {
            int companyId = User.Identity.GetCompanyId();

            List<Project> projects = new();

            projects = await _projectService.GetUnassignedProjectsAsync(companyId);

            return View(projects);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignPM(int? projectId)
        {
            if (projectId == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId();

            AssignPMViewModel model = new();

            model.Project = await _projectService.GetProjectByIdAsync(projectId.Value, companyId);
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(AppRole.ProjectManager), companyId), "Id", "FullName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPM(AssignPMViewModel model)
        {
            if (!string.IsNullOrEmpty(model.PMID))
            {
                await _projectService.AddProjectManagerAsync(model.PMID, model.Project.Id);
                return RedirectToAction(nameof(AllProjects));
            }
            return RedirectToAction(nameof(AssignPM), new { projectId = model.Project!.Id});
        }

        [HttpGet]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AssignMembers(int projectId)
        {
            if(projectId == null)
            {
                return NotFound();
            }

            ProjectMembersViewModel model = new();

            int companyId = User.Identity.GetCompanyId();
            List<AppUser> developers = await _rolesService.GetUsersInRoleAsync(nameof(AppRole.Developer), companyId);
            List<AppUser> submitters = await _rolesService.GetUsersInRoleAsync(nameof(AppRole.Submitter), companyId);

            List<AppUser> teamMembers = developers.Concat(submitters).ToList();

            model.Project = await _projectService.GetProjectByIdAsync(projectId, companyId);

            List<string> projectMembers = model.Project.Members.Select(x => x.Id).ToList(); 

            model.UsersList = new MultiSelectList( teamMembers, "Id", "FullName", projectMembers);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMembers(ProjectMembersViewModel model)
        {
            if(model.SelectedUsers != null)
            {
                List<string> memberIds = (await _projectService.GetAllProjectMembersExceptPMAsync(model.Project.Id))
                                                                                                .Select(x => x.Id).ToList();

                //Remove current Members
                foreach(string member in memberIds)
                {
                    await _projectService.RemoveUserFromProjectAsync(member, model.Project.Id);
                }

                foreach(string member in model.SelectedUsers)
                {
                    await _projectService.AddUserToProjectAsync(member, model.Project.Id);
                }

                //go to project details
                return RedirectToAction("Details", "Projects", new { id = model.Project.Id });
            }

            return RedirectToAction(nameof(AssignMembers), new { id = model.Project.Id });
             
        }



        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Remember that the _context should not be used directly in the controller so....     

            // Edit the following code to use the service layer. 
            // Your goal is to return the 'project' from the databse
            // with the Id equal to the parameter passed in.               
            // This is the only modification necessary for this method/action.     

            var project = await _context.Projects
                .Include(p => p.Company)
                .Include(p => p.ProjectPriority)
                .FirstOrDefaultAsync(m => m.Id == id);


            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity.GetCompanyId();

            AddProjectWithPMViewModel model = new();

            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(AppRole.ProjectManager), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookUpService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View(model);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProjectWithPMViewModel model)
        {
            int companyId = User.Identity.GetCompanyId();

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.Project?.LogoFormFile != null)
                    {
                        model.Project.LogoData = await _fileService.ConvertFileToByteArrayAsync(model.Project.LogoFormFile);
                        model.Project.LogoName = model.Project.LogoFormFile.Name;
                        model.Project.LogoType = model.Project.LogoFormFile.ContentType;
                    }

                    model.Project.CompanyId = companyId;
                    model.Project.Created = DateTime.UtcNow;

                    model.Project.StartDate = DateTime.SpecifyKind(model.Project.StartDate, DateTimeKind.Utc);
                    model.Project.EndDate = DateTime.SpecifyKind(model.Project.EndDate, DateTimeKind.Utc);

                    await _projectService.AddNewProjectAsync(model.Project);

                    if (!string.IsNullOrEmpty(model.PMID))
                    {
                        await _projectService.AddProjectManagerAsync(model.PMID, model.Project.Id);
                    }
                    return RedirectToAction(nameof(AllProjects));
                }
                catch (Exception)
                {
                    throw;
                }
            }
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(AppRole.ProjectManager), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookUpService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View(model.Project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            int companyId = User.Identity.GetCompanyId();
            AddProjectWithPMViewModel model = new();

            model.Project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
            if (model.Project == null)
            {
                return NotFound();
            }

            AppUser projectManager = await _projectService.GetProjectManagerAsync(model.Project.Id);
            if (projectManager != null)
            {
                model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(AppRole.ProjectManager), companyId), "Id", "FullName", projectManager!.Id);
            }
            else
            {
                model.PriorityList = new SelectList(await _lookUpService.GetProjectPrioritiesAsync(), "Id", "Name");
            }

            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(AppRole.ProjectManager), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookUpService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View(model);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AddProjectWithPMViewModel model)
        {
            int companyId = User.Identity.GetCompanyId();


            if (model != null)
            {
                try
                {
                    if (model.Project?.LogoFormFile != null)
                    {
                        model.Project.LogoData = await _fileService.ConvertFileToByteArrayAsync(model.Project.LogoFormFile);
                        model.Project.LogoName = model.Project.LogoFormFile.Name;
                        model.Project.LogoType = model.Project.LogoFormFile.ContentType;
                    }

                    model.Project.Created = DateTime.SpecifyKind(model.Project.Created, DateTimeKind.Utc);
                    model.Project.StartDate = DateTime.SpecifyKind(model.Project.StartDate, DateTimeKind.Utc);
                    model.Project.EndDate = DateTime.SpecifyKind(model.Project.EndDate, DateTimeKind.Utc);

                    await _projectService.UpdateProjectAsync(model.Project);

                    if (!string.IsNullOrEmpty(model.PMID))
                    {
                        await _projectService.AddProjectManagerAsync(model.PMID, model.Project.Id);
                    }

                    return RedirectToAction("AllProjects");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProjectExists(model.Project!.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(AppRole.ProjectManager), companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookUpService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View(model);
        }

        // GET: Projects/Archive/5
        public async Task<IActionResult> Archive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int companyId = User.Identity.GetCompanyId();
            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
            

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Archive/5
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            int companyId = User.Identity.GetCompanyId();
            var project = await _projectService.GetProjectByIdAsync(id, companyId);

            await _projectService.ArchiveProjectAsync(project);


            return RedirectToAction(nameof(AllProjects));
        }

        // GET: Projects/Restore/5
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            int companyId = User.Identity.GetCompanyId();
            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);


            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Restore/5
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            int companyId = User.Identity.GetCompanyId();
            var project = await _projectService.GetProjectByIdAsync(id, companyId);

            await _projectService.RestoreProjectAsync(project);


            return RedirectToAction(nameof(AllProjects));
        }


        private async Task<bool> ProjectExists(int id)
        {
            int companyId = User.Identity.GetCompanyId();
            return (await _projectService.GetAllProjectsByCompanyAsync(companyId)).Any(p => p.Id == id);
        }
    }
}
