using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Services
{
    public class BusinessRulesService
    {
        public bool IsCourseDateRangeValid(DateTime startDate, DateTime endDate)
        {
            return endDate >= startDate;
        }

        public bool IsAssignmentScoreValid(double score, double maxScore)
        {
            return score >= 0 && score <= maxScore;
        }

        public bool IsExamScoreValid(double score, double maxScore)
        {
            return score >= 0 && score <= maxScore;
        }

        public async Task<bool> IsDuplicateEnrolmentAsync(ApplicationDbContext context, int studentProfileId, int courseId)
        {
            return await context.CourseEnrolments
                .AnyAsync(e => e.StudentProfileId == studentProfileId && e.CourseId == courseId);
        }

        public async Task<List<AssignmentResult>> GetAssignmentResultsForFacultyAsync(ApplicationDbContext context, int facultyProfileId)
        {
            return await context.AssignmentResults
                .Include(a => a.Assignment)
                    .ThenInclude(a => a.Course)
                .Include(a => a.StudentProfile)
                .Where(a => a.Assignment != null &&
                            a.Assignment.Course != null &&
                            a.Assignment.Course.FacultyProfileId == facultyProfileId)
                .ToListAsync();
        }

        public async Task<List<ExamResult>> GetVisibleExamResultsForStudentAsync(ApplicationDbContext context, int studentProfileId)
        {
            return await context.ExamResults
                .Include(e => e.Exam)
                    .ThenInclude(e => e.Course)
                .Where(e => e.StudentProfileId == studentProfileId &&
                            e.Exam != null &&
                            e.Exam.ResultsReleased)
                .ToListAsync();
        }
    }
}