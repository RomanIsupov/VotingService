using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Alter.Data;
using Alter.Models;

namespace Alter.Controllers
{
    public class AnswerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnswerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Answer/Create
        public IActionResult Create()
        {
            if (!this.HttpContext.User.IsInRole(ApplicationRoles.Administrators))
            {
                return this.NotFound();
            }

            this.ViewData["PollId"] = new SelectList(this._context.Polls, "Id", "Question");
            return this.View();
        }

        // POST: Answer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Text,PollId")] Answer answer)
        {
            if (!this.HttpContext.User.IsInRole(ApplicationRoles.Administrators))
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                answer.Id = Guid.NewGuid();
                answer.Amount = 0;
                this._context.Add(answer);
                await this._context.SaveChangesAsync();
                return this.RedirectToAction("Index", "Poll");
            }
            this.ViewData["PollId"] = new SelectList(this._context.Polls, "Id", "Question", answer.PollId);
            return this.View(answer);
        }

        // GET: Answer/Edit/5
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

            var answer = await this._context.Answers.SingleOrDefaultAsync(m => m.Id == id);
            if (answer == null)
            {
                return this.NotFound();
            }
            this.ViewData["PollId"] = new SelectList(this._context.Polls, "Id", "Question", answer.PollId);
            return this.View(answer);
        }

        // POST: Answer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Text,Amount,PollId")] Answer answer)
        {
            if (!this.HttpContext.User.IsInRole(ApplicationRoles.Administrators))
            {
                return this.NotFound();
            }

            if (id != answer.Id)
            {
                return this.NotFound();
            }

            if (this.ModelState.IsValid)
            {
                try
                {
                    this._context.Update(answer);
                    await this._context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!this.AnswerExists(answer.Id))
                    {
                        return this.NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return this.RedirectToAction("Index", "Poll");
            }
            ViewData["PollId"] = new SelectList(_context.Polls, "Id", "Question", answer.PollId);
            return this.View(answer);
        }

        private Boolean AnswerExists(Guid id)
        {
            return this._context.Answers.Any(e => e.Id == id);
        }
    }
}
