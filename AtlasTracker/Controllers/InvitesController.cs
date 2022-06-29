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
using AtlasTracker.Extensions;
using AtlasTracker.Services.Interfaces;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace AtlasTracker.Controllers
{
    public class InvitesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _projectService;
        private readonly IDataProtector _encryptor;
        private readonly IBTCompanyInfoService _companyInfoService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailService;
        private readonly IBTInviteService _inviteService;


        public InvitesController(ApplicationDbContext context,
                                 IBTProjectService projectService,
                                 IDataProtectionProvider dataProtectionProvider,
                                 IBTCompanyInfoService companyInfoService,
                                 UserManager<AppUser> userManager,
                                 IEmailSender emailService, 
                                 IBTInviteService inviteService)
        {
            _context = context;
            _projectService = projectService;
            _encryptor = dataProtectionProvider.CreateProtector("CF.ATLAS.TheBugTr@cker");
            _companyInfoService = companyInfoService;
            _userManager = userManager;
            _emailService = emailService;
            _inviteService = inviteService;
        }

        // GET: Invites
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Invites.Include(i => i.Invitee).Include(i => i.Invitor).Include(i => i.Projects);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Invites/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invite = await _context.Invites
                .Include(i => i.Invitee)
                .Include(i => i.Invitor)
                .Include(i => i.Projects)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invite == null)
            {
                return NotFound();
            }

            return View(invite);
        }

        // GET: Invites/Create
        public async Task<IActionResult> Create()
        {
            int companyId = User.Identity.GetCompanyId();


            ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId), "Id", "Name");
            return View();
        }

        // POST: Invites/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProjectId,InviteeEmail,InviteeFirstName,InviteeLastName,Message")] Invite invite)
        {
            int companyId = User.Identity.GetCompanyId();
            ModelState.Remove("InvitorId");

            if (ModelState.IsValid)
            {
                Guid guid = Guid.NewGuid();

                string token = _encryptor.Protect(guid.ToString());
                string email = _encryptor.Protect(invite.InviteeEmail);
                string company = _encryptor.Protect(companyId.ToString());

                string callbackUrl = Url.Action("ProcessInvite", "Invites", new { token, email, company }, protocol: Request.Scheme);

                string body = $@"{invite.Message} <br /> 
                              Please join my Company! <br />
                              Click the following link to join our team. <br />
                              <a href=""{callbackUrl}"" >COLLABORATE</a> ";

                string destination = invite.InviteeEmail;

                Company btCompany = await _companyInfoService.GetCompanyInfoByIdAsync(companyId);
                string subject = $"Atlas BugTracker: {btCompany.Name} invite";

                await _emailService.SendEmailAsync(destination, subject, body);

                invite.CompanyToken = guid;
                invite.CompanyId = companyId;
                invite.InviteDate = DateTime.UtcNow;
                invite.InvitorId = _userManager.GetUserId(User);
                invite.IsValid = true;

                await _inviteService.AddNewInviteAsync(invite);

                return RedirectToAction("DashBoard", "Home", new { swalMessage = "Invite Sent!"});
            }


            ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsByCompanyAsync(companyId), "Id", "Description");
            return View(invite);
        }

        [HttpGet]
        public async Task<IActionResult> ProcessInvite(string token, string email, string company)
        {
            if (token == null)
            {
                return NotFound();
            }
            Guid companyToken = Guid.Parse(_encryptor.Unprotect(token));
            string inviteeEmail = _encryptor.Unprotect(email);
            int companyId = int.Parse(_encryptor.Unprotect(company));
            try
            {
                Invite invite = await _inviteService.GetInviteAsync(companyToken, inviteeEmail, companyId);
                if (invite != null)
                {
                    return View(invite);
                }
                return NotFound();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: Invites/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invite = await _context.Invites.FindAsync(id);
            if (invite == null)
            {
                return NotFound();
            }
            ViewData["InviteeId"] = new SelectList(_context.Users, "Id", "Id", invite.InviteeId);
            ViewData["InvitorId"] = new SelectList(_context.Users, "Id", "Id", invite.InvitorId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", invite.ProjectId);
            return View(invite);
        }

        // POST: Invites/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IviteDate,JoinDate,CompanyToken,CommentId,ProjectId,InvitorId,InviteeId,InviteeEmail,InviteeFirstName,InviteeLastName,Message,IsValid")] Invite invite)
        {
            if (id != invite.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InviteExists(invite.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["InviteeId"] = new SelectList(_context.Users, "Id", "Id", invite.InviteeId);
            ViewData["InvitorId"] = new SelectList(_context.Users, "Id", "Id", invite.InvitorId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Description", invite.ProjectId);
            return View(invite);
        }

        // GET: Invites/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invite = await _context.Invites
                .Include(i => i.Invitee)
                .Include(i => i.Invitor)
                .Include(i => i.Projects)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invite == null)
            {
                return NotFound();
            }

            return View(invite);
        }

        // POST: Invites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invite = await _context.Invites.FindAsync(id);
            _context.Invites.Remove(invite);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InviteExists(int id)
        {
            return _context.Invites.Any(e => e.Id == id);
        }
    }
}
