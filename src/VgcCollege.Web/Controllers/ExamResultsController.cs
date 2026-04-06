using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;
using System.Security.Claims;

namespace VgcCollege.Web.Controllers
{
    [Authorize]
    public class ExamResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ExamResults
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Index()
        {
            var query = _context.ExamResults
                .Include(e => e.Exam)
                    .ThenInclude(e => e.Course)
                .Include(e => e.StudentProfile)
                .AsQueryable();

            if (User.IsInRole("Faculty"))
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var faculty = await _context.FacultyProfiles
                    .FirstOrDefaultAsync(f => f.IdentityUserId == userId);

                if (faculty != null)
                {
                    query = query.Where(e => e.Exam.Course.FacultyProfileId == faculty.Id);
                }
                else
                {
                    query = query.Where(e => false);
                }
            }

            return View(await query.ToListAsync());
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyResults()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var student = await _context.StudentProfiles
                .FirstOrDefaultAsync(s => s.IdentityUserId == userId);

            if (student == null)
                return NotFound();

            var results = await _context.ExamResults
                .Include(e => e.Exam)
                    .ThenInclude(e => e.Course)
                .Where(e => e.StudentProfileId == student.Id && e.Exam.ResultsReleased)
                .ToListAsync();

            return View(results);
        }

        // GET: ExamResults/Details/5
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examResult = await _context.ExamResults
                .Include(e => e.Exam)
                .Include(e => e.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examResult == null)
            {
                return NotFound();
            }

            return View(examResult);
        }

        // GET: ExamResults/Create
        [Authorize(Roles = "Admin,Faculty")]
        public IActionResult Create()
        {
            ViewData["ExamId"] = new SelectList(
                _context.Exams.Include(e => e.Course)
                .Select(e => new {
                    e.Id,
                    Display = e.Title + " (" + e.Course.Name + ")"
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

        // POST: ExamResults/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Faculty")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ExamId,StudentProfileId,Score,Grade")] ExamResult examResult)
        {
            var exam = await _context.Exams.FindAsync(examResult.ExamId);

            if (exam != null && examResult.Score > exam.MaxScore)
            {
                ModelState.AddModelError("Score", $"Score cannot be greater than the maximum score ({exam.MaxScore}).");
            }

            if (ModelState.IsValid)
            {
                _context.Add(examResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Title", examResult.ExamId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Email", examResult.StudentProfileId);
            return View(examResult);
        }

        // GET: ExamResults/Edit/5
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examResult = await _context.ExamResults.FindAsync(id);
            if (examResult == null)
            {
                return NotFound();
            }
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Title", examResult.ExamId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Email", examResult.StudentProfileId);
            return View(examResult);
        }

        // POST: ExamResults/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,Faculty")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExamId,StudentProfileId,Score,Grade")] ExamResult examResult)
        {
            if (id != examResult.Id)
            {
                return NotFound();
            }

            var exam = await _context.Exams.FindAsync(examResult.ExamId);

            if (exam != null && examResult.Score > exam.MaxScore)
            {
                ModelState.AddModelError("Score", $"Score cannot be greater than the maximum score ({exam.MaxScore}).");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(examResult);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamResultExists(examResult.Id))
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
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Title", examResult.ExamId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Email", examResult.StudentProfileId);
            return View(examResult);
        }

        // GET: ExamResults/Delete/5
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examResult = await _context.ExamResults
                .Include(e => e.Exam)
                .Include(e => e.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examResult == null)
            {
                return NotFound();
            }

            return View(examResult);
        }

        // POST: ExamResults/Delete/5
        [Authorize(Roles = "Admin,Faculty")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var examResult = await _context.ExamResults.FindAsync(id);
            if (examResult != null)
            {
                _context.ExamResults.Remove(examResult);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamResultExists(int id)
        {
            return _context.ExamResults.Any(e => e.Id == id);
        }
    }
}
