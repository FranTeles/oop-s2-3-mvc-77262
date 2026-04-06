using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;
using VgcCollege.Web.Services;
using Xunit;

namespace VgcCollege.Tests.Tests
{
    public class BusinessRulesServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public void IsCourseDateRangeValid_ReturnsTrue_WhenEndDateIsAfterStartDate()
        {
            var service = new BusinessRulesService();

            var result = service.IsCourseDateRangeValid(
                new DateTime(2026, 1, 10),
                new DateTime(2026, 5, 30));

            Assert.True(result);
        }

        [Fact]
        public void IsCourseDateRangeValid_ReturnsFalse_WhenEndDateIsBeforeStartDate()
        {
            var service = new BusinessRulesService();

            var result = service.IsCourseDateRangeValid(
                new DateTime(2026, 5, 30),
                new DateTime(2026, 1, 10));

            Assert.False(result);
        }

        [Fact]
        public void IsAssignmentScoreValid_ReturnsTrue_WhenScoreIsWithinMaxScore()
        {
            var service = new BusinessRulesService();

            var result = service.IsAssignmentScoreValid(80, 100);

            Assert.True(result);
        }

        [Fact]
        public void IsAssignmentScoreValid_ReturnsFalse_WhenScoreExceedsMaxScore()
        {
            var service = new BusinessRulesService();

            var result = service.IsAssignmentScoreValid(120, 100);

            Assert.False(result);
        }

        [Fact]
        public void IsExamScoreValid_ReturnsTrue_WhenScoreIsWithinMaxScore()
        {
            var service = new BusinessRulesService();

            var result = service.IsExamScoreValid(70, 100);

            Assert.True(result);
        }

        [Fact]
        public void IsExamScoreValid_ReturnsFalse_WhenScoreExceedsMaxScore()
        {
            var service = new BusinessRulesService();

            var result = service.IsExamScoreValid(130, 100);

            Assert.False(result);
        }

        [Fact]
        public async Task IsDuplicateEnrolmentAsync_ReturnsTrue_WhenStudentIsAlreadyEnrolledInCourse()
        {
            using var context = CreateContext();
            var service = new BusinessRulesService();

            context.CourseEnrolments.Add(new CourseEnrolment
            {
                Id = 1,
                StudentProfileId = 1,
                CourseId = 1,
                EnrolDate = DateTime.Today,
                Status = "Active"
            });

            await context.SaveChangesAsync();

            var result = await service.IsDuplicateEnrolmentAsync(context, 1, 1);

            Assert.True(result);
        }

        [Fact]
        public async Task GetAssignmentResultsForFacultyAsync_ReturnsOnlyResultsForThatFacultyCourses()
        {
            using var context = CreateContext();
            var service = new BusinessRulesService();

            var faculty1 = new FacultyProfile
            {
                Id = 1,
                IdentityUserId = "faculty-1",
                Name = "Faculty One",
                Email = "faculty1@test.com",
                Phone = "111"
            };

            var faculty2 = new FacultyProfile
            {
                Id = 2,
                IdentityUserId = "faculty-2",
                Name = "Faculty Two",
                Email = "faculty2@test.com",
                Phone = "222"
            };

            var branch = new Branch
            {
                Id = 1,
                Name = "Dublin Branch",
                Address = "Dublin"
            };

            var course1 = new Course
            {
                Id = 1,
                Name = "Computing",
                BranchId = 1,
                FacultyProfileId = 1,
                StartDate = new DateTime(2026, 1, 10),
                EndDate = new DateTime(2026, 5, 30)
            };

            var course2 = new Course
            {
                Id = 2,
                Name = "Business",
                BranchId = 1,
                FacultyProfileId = 2,
                StartDate = new DateTime(2026, 1, 10),
                EndDate = new DateTime(2026, 5, 30)
            };

            var student1 = new StudentProfile
            {
                Id = 1,
                IdentityUserId = "student-1",
                Name = "Alice",
                Email = "alice@test.com",
                Phone = "123",
                Address = "Dublin",
                StudentNumber = "S1001"
            };

            var student2 = new StudentProfile
            {
                Id = 2,
                IdentityUserId = "student-2",
                Name = "Bob",
                Email = "bob@test.com",
                Phone = "456",
                Address = "Cork",
                StudentNumber = "S1002"
            };

            var assignment1 = new Assignment
            {
                Id = 1,
                CourseId = 1,
                Title = "Assignment 1",
                MaxScore = 100,
                DueDate = new DateTime(2026, 3, 1)
            };

            var assignment2 = new Assignment
            {
                Id = 2,
                CourseId = 2,
                Title = "Assignment 2",
                MaxScore = 100,
                DueDate = new DateTime(2026, 3, 1)
            };

            var result1 = new AssignmentResult
            {
                Id = 1,
                AssignmentId = 1,
                StudentProfileId = 1,
                Score = 78,
                Feedback = "Good"
            };

            var result2 = new AssignmentResult
            {
                Id = 2,
                AssignmentId = 2,
                StudentProfileId = 2,
                Score = 88,
                Feedback = "Very good"
            };

            context.Branches.Add(branch);
            context.FacultyProfiles.AddRange(faculty1, faculty2);
            context.Courses.AddRange(course1, course2);
            context.StudentProfiles.AddRange(student1, student2);
            context.Assignments.AddRange(assignment1, assignment2);
            context.AssignmentResults.AddRange(result1, result2);

            await context.SaveChangesAsync();

            var results = await service.GetAssignmentResultsForFacultyAsync(context, 1);

            Assert.Single(results);
            Assert.Equal(1, results.First().AssignmentId);
        }

        [Fact]
        public async Task GetVisibleExamResultsForStudentAsync_ReturnsOnlyReleasedResultsForThatStudent()
        {
            using var context = CreateContext();
            var service = new BusinessRulesService();

            var branch = new Branch
            {
                Id = 1,
                Name = "Dublin Branch",
                Address = "Dublin"
            };

            var course = new Course
            {
                Id = 1,
                Name = "Computing",
                BranchId = 1,
                StartDate = new DateTime(2026, 1, 10),
                EndDate = new DateTime(2026, 5, 30)
            };

            var student = new StudentProfile
            {
                Id = 1,
                IdentityUserId = "student-1",
                Name = "Alice",
                Email = "alice@test.com",
                Phone = "123",
                Address = "Dublin",
                StudentNumber = "S1001"
            };

            var releasedExam = new Exam
            {
                Id = 1,
                CourseId = 1,
                Title = "Released Exam",
                Date = new DateTime(2026, 5, 20),
                MaxScore = 100,
                ResultsReleased = true
            };

            var provisionalExam = new Exam
            {
                Id = 2,
                CourseId = 1,
                Title = "Provisional Exam",
                Date = new DateTime(2026, 5, 25),
                MaxScore = 100,
                ResultsReleased = false
            };

            var releasedResult = new ExamResult
            {
                Id = 1,
                ExamId = 1,
                StudentProfileId = 1,
                Score = 75,
                Grade = "B"
            };

            var provisionalResult = new ExamResult
            {
                Id = 2,
                ExamId = 2,
                StudentProfileId = 1,
                Score = 90,
                Grade = "A"
            };

            context.Branches.Add(branch);
            context.Courses.Add(course);
            context.StudentProfiles.Add(student);
            context.Exams.AddRange(releasedExam, provisionalExam);
            context.ExamResults.AddRange(releasedResult, provisionalResult);

            await context.SaveChangesAsync();

            var results = await service.GetVisibleExamResultsForStudentAsync(context, 1);

            Assert.Single(results);
            Assert.Equal(1, results.First().ExamId);
        }
    }
}