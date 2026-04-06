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

namespace VgcCollege.Web.Controllers
{
    [Authorize(Roles = "Admin,Faculty")]
    public class AttendanceRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AttendanceRecords
        public async Task<IActionResult> Index()
        {
            var data = _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e.StudentProfile)
                .Include(a => a.CourseEnrolment)
                    .ThenInclude(e => e.Course);

            return View(await data.ToListAsync());
        }

        // GET: AttendanceRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendanceRecord = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attendanceRecord == null)
            {
                return NotFound();
            }

            return View(attendanceRecord);
        }

        // GET: AttendanceRecords/Create
        public IActionResult Create()
        {
            ViewData["CourseEnrolmentId"] = new SelectList(
                _context.CourseEnrolments
                    .Include(c => c.StudentProfile)
                    .Include(c => c.Course)
                    .Select(c => new {
                        c.Id,
                        Display = c.StudentProfile.Name + " - " + c.Course.Name
                    }),
                "Id",
                "Display"
            );
            return View();
        }

        // POST: AttendanceRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseEnrolmentId,WeekNumber,Date,Present")] AttendanceRecord attendanceRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(attendanceRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseEnrolmentId"] = new SelectList(_context.CourseEnrolments, "Id", "Status", attendanceRecord.CourseEnrolmentId);
            return View(attendanceRecord);
        }

        // GET: AttendanceRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendanceRecord = await _context.AttendanceRecords.FindAsync(id);
            if (attendanceRecord == null)
            {
                return NotFound();
            }
            ViewData["CourseEnrolmentId"] = new SelectList(_context.CourseEnrolments, "Id", "Status", attendanceRecord.CourseEnrolmentId);
            return View(attendanceRecord);
        }

        // POST: AttendanceRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseEnrolmentId,WeekNumber,Date,Present")] AttendanceRecord attendanceRecord)
        {
            if (id != attendanceRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(attendanceRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AttendanceRecordExists(attendanceRecord.Id))
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
            ViewData["CourseEnrolmentId"] = new SelectList(_context.CourseEnrolments, "Id", "Status", attendanceRecord.CourseEnrolmentId);
            return View(attendanceRecord);
        }

        // GET: AttendanceRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendanceRecord = await _context.AttendanceRecords
                .Include(a => a.CourseEnrolment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (attendanceRecord == null)
            {
                return NotFound();
            }

            return View(attendanceRecord);
        }

        // POST: AttendanceRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var attendanceRecord = await _context.AttendanceRecords.FindAsync(id);
            if (attendanceRecord != null)
            {
                _context.AttendanceRecords.Remove(attendanceRecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AttendanceRecordExists(int id)
        {
            return _context.AttendanceRecords.Any(e => e.Id == id);
        }
    }
}
