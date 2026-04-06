using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin,Faculty")]
    public class AssignmentResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var query = _context.AssignmentResults
                .Include(a => a.Assignment)
                    .ThenInclude(a => a.Course)
                .Include(a => a.StudentProfile)
                .AsQueryable();

            if (User.IsInRole("Faculty"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var faculty = await _context.FacultyProfiles
                    .FirstOrDefaultAsync(f => f.IdentityUserId == userId);

                if (faculty != null)
                {
                    query = query.Where(a => a.Assignment.Course.FacultyProfileId == faculty.Id);
                }
                else
                {
                    query = query.Where(a => false);
                }
            }

            return View(await query.ToListAsync());
        }

        // GET: AssignmentResults/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignmentResult = await _context.AssignmentResults
                .Include(a => a.Assignment)
                .Include(a => a.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignmentResult == null)
            {
                return NotFound();
            }

            return View(assignmentResult);
        }

        // GET: AssignmentResults/Create
        public IActionResult Create()
        {
            ViewData["AssignmentId"] = new SelectList(
                _context.Assignments.Include(a => a.Course)
                .Select(a => new {
                    a.Id,
                    Display = a.Title + " (" + a.Course.Name + ")"
                }),
                "Id",
                "Display"
            );
            ViewData["StudentProfileId"] = new SelectList(
                _context.StudentProfiles,
                "Id",
                "Name"
            );
            return View();
        }

        // POST: AssignmentResults/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AssignmentId,StudentProfileId,Score,Feedback")] AssignmentResult assignmentResult)
        {

            var assignment = await _context.Assignments.FindAsync(assignmentResult.AssignmentId);

            if (assignment != null && assignmentResult.Score > assignment.MaxScore)
            {
                ModelState.AddModelError("Score", $"Score cannot be greater than the maximum score ({assignment.MaxScore}).");
            }

            if (ModelState.IsValid)
            {
                _context.Add(assignmentResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssignmentId"] = new SelectList(
                _context.Assignments.Include(a => a.Course)
                .Select(a => new {
                    a.Id,
                    Display = a.Title + " (" + a.Course.Name + ")"
                }),
                "Id",
                "Display",
                assignmentResult.AssignmentId
            );

            ViewData["StudentProfileId"] = new SelectList(
                _context.StudentProfiles,
                "Id",
                "Name",
                assignmentResult.StudentProfileId
            );
            return View(assignmentResult);
        }

        // GET: AssignmentResults/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignmentResult = await _context.AssignmentResults.FindAsync(id);
            if (assignmentResult == null)
            {
                return NotFound();
            }
            ViewData["AssignmentId"] = new SelectList(
                _context.Assignments.Include(a => a.Course)
                .Select(a => new {
                    a.Id,
                    Display = a.Title + " (" + a.Course.Name + ")"
                }),
                "Id",
                "Display",
                assignmentResult.AssignmentId
            );

            ViewData["StudentProfileId"] = new SelectList(
                _context.StudentProfiles,
                "Id",
                "Name",
                assignmentResult.StudentProfileId
            );
            return View(assignmentResult);
        }

        // POST: AssignmentResults/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AssignmentId,StudentProfileId,Score,Feedback")] AssignmentResult assignmentResult)
        {
            if (id != assignmentResult.Id)
            {
                return NotFound();
            }

            var assignment = await _context.Assignments.FindAsync(assignmentResult.AssignmentId);

            if (assignment != null && assignmentResult.Score > assignment.MaxScore)
            {
                ModelState.AddModelError("Score", $"Score cannot be greater than the maximum score ({assignment.MaxScore}).");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignmentResult);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentResultExists(assignmentResult.Id))
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
            ViewData["AssignmentId"] = new SelectList(
                _context.Assignments.Include(a => a.Course)
                .Select(a => new {
                    a.Id,
                    Display = a.Title + " (" + a.Course.Name + ")"
                }),
                "Id",
                "Display",
                assignmentResult.AssignmentId
            );

            ViewData["StudentProfileId"] = new SelectList(
                _context.StudentProfiles,
                "Id",
                "Name",
                assignmentResult.StudentProfileId
            );
            return View(assignmentResult);
        }

        // GET: AssignmentResults/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignmentResult = await _context.AssignmentResults
                .Include(a => a.Assignment)
                .Include(a => a.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignmentResult == null)
            {
                return NotFound();
            }

            return View(assignmentResult);
        }

        // POST: AssignmentResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignmentResult = await _context.AssignmentResults.FindAsync(id);
            if (assignmentResult != null)
            {
                _context.AssignmentResults.Remove(assignmentResult);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentResultExists(int id)
        {
            return _context.AssignmentResults.Any(e => e.Id == id);
        }
    }
}
