using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Alter.Data;
using Alter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace Alter.Controllers
{
    public class PollController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public PollController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IHttpContextAccessor httpContextAccessor)
        {
            this.userManager = userManager;
            _context = context;
            this.signInManager = signInManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        private HttpContext httpContext => this.httpContextAccessor.HttpContext;

        // GET: Poll
        public async Task<IActionResult> Index()
        {
            return this.View(await this._context.Polls.ToListAsync());
        }

        // GET: Poll/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return this.NotFound();
            }

            var poll = await this._context.Polls
                .Include(p => p.Answers)
                .Include(p => p.Users)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (poll == null)
            {
                return this.NotFound();
            }
            if (!poll.Finished && !signInManager.IsSignedIn(this.HttpContext.User))
            {
                return this.NotFound();
            }
            return this.View(poll);
        }

        // GET: Poll/Create
        public IActionResult Create()
        {
            if (!this.HttpContext.User.IsInRole(ApplicationRoles.Administrators))
            {
                return this.NotFound();
            }

            return this.View();
        }

        // POST: Poll/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Question,Finished")] Poll poll)
        {
            if (!this.HttpContext.User.IsInRole(ApplicationRoles.Administrators))
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                poll.Id = Guid.NewGuid();
                poll.Finished = false;
                this._context.Add(poll);
                await this._context.SaveChangesAsync();
                return this.RedirectToAction(nameof(Index));
            }
            return this.View(poll);
        }

        [HttpGet]
        [Route("/Poll/Vote/{AnswerId}")]
        public async Task<IActionResult> Vote(Guid? AnswerId)
        {
            if (!signInManager.IsSignedIn(this.HttpContext.User))
            {
                return this.NotFound();
            }
            var answer = await this._context.Answers.Include(a => a.Poll).ThenInclude(p => p.Users).SingleOrDefaultAsync(a => a.Id == AnswerId);
            if (answer.Poll.Users.Any(u => u.Id == this.userManager.GetUserId(this.httpContextAccessor.HttpContext.User)))
            {
                return this.NotFound();
            }
            return this.View(new VoteViewModel { PollId = answer.PollId, AnswerId = answer.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Vote(Guid PollId, Guid AnswerId)
        {
            if (!signInManager.IsSignedIn(this.HttpContext.User))
            {
                return this.NotFound();
            }
            var answer = await this._context.Answers.Include(a => a.Poll).ThenInclude(a => a.Users).SingleOrDefaultAsync(a => a.Id == AnswerId);
            if (answer != null)
            {
                if (answer.Poll.Users.Any(u => u.UserName == this.userManager.GetUserName(this.httpContextAccessor.HttpContext.User)))
                {
                    return this.NotFound();
                }

                answer.Amount = answer.Amount + 1;
                this._context.Update(answer);
                answer.Poll.Users.Add(await this.userManager.GetUserAsync(this.HttpContext.User));
                await this._context.SaveChangesAsync();
            }
            return this.RedirectToAction("Details", "Poll", new { id = answer.Poll.Id });
        }

        // GET: Poll/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (!this.HttpContext.User.IsInRole(ApplicationRoles.Administrators))
            {
                return this.NotFound();
            }

            if (id == null)
            {
                return this.NotFound();
            }

            var poll = await _context.Polls.SingleOrDefaultAsync(m => m.Id == id);
            if (poll == null)
            {
                return this.NotFound();
            }
            return this.View(poll);
        }

        // POST: Poll/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Question,Finished")] Poll poll)
        {
            if (!this.HttpContext.User.IsInRole(ApplicationRoles.Administrators))
            {
                return this.NotFound();
            }

            if (id != poll.Id)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                try
                {
                    this._context.Update(poll);
                    await this._context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PollExists(poll.Id))
                    {
                        return this.NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return this.RedirectToAction(nameof(Index));
            }
            return this.View(poll);
        }

        // GET: Poll/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (!this.HttpContext.User.IsInRole(ApplicationRoles.Administrators))
            {
                return this.NotFound();
            }

            if (id == null)
            {
                return this.NotFound();
            }

            var poll = await this._context.Polls
                .SingleOrDefaultAsync(m => m.Id == id);
            if (poll == null)
            {
                return this.NotFound();
            }

            return this.View(poll);
        }

        // POST: Poll/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (!this.HttpContext.User.IsInRole(ApplicationRoles.Administrators))
            {
                return this.NotFound();
            }

            var poll = await this._context.Polls.Include(p => p.Users).SingleOrDefaultAsync(m => m.Id == id);
            this._context.Polls.Remove(poll);
            await this._context.SaveChangesAsync();
            return this.RedirectToAction("Index");
        }

        private bool PollExists(Guid id)
        {
            return this._context.Polls.Any(e => e.Id == id);
        }
    }
}
