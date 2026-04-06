using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();

            string[] roles = { "Admin", "Faculty", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@vgc.ie";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            var facultyEmail = "faculty@vgc.ie";
            var facultyUser = await userManager.FindByEmailAsync(facultyEmail);
            if (facultyUser == null)
            {
                facultyUser = new IdentityUser
                {
                    UserName = facultyEmail,
                    Email = facultyEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(facultyUser, "Faculty123!");
                await userManager.AddToRoleAsync(facultyUser, "Faculty");
            }

            var student1Email = "student1@vgc.ie";
            var student1User = await userManager.FindByEmailAsync(student1Email);
            if (student1User == null)
            {
                student1User = new IdentityUser
                {
                    UserName = student1Email,
                    Email = student1Email,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(student1User, "Student123!");
                await userManager.AddToRoleAsync(student1User, "Student");
            }

            var student2Email = "student2@vgc.ie";
            var student2User = await userManager.FindByEmailAsync(student2Email);
            if (student2User == null)
            {
                student2User = new IdentityUser
                {
                    UserName = student2Email,
                    Email = student2Email,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(student2User, "Student123!");
                await userManager.AddToRoleAsync(student2User, "Student");
            }

            if (!context.Branches.Any())
            {
                context.Branches.AddRange(
                    new Branch { Name = "Dublin Branch", Address = "Dublin City Centre" },
                    new Branch { Name = "Cork Branch", Address = "Cork City" },
                    new Branch { Name = "Galway Branch", Address = "Galway City" }
                );
                await context.SaveChangesAsync();
            }

            if (!context.Courses.Any())
            {
                var dublinBranch = context.Branches.First(b => b.Name == "Dublin Branch");
                var corkBranch = context.Branches.First(b => b.Name == "Cork Branch");
                var galwayBranch = context.Branches.First(b => b.Name == "Galway Branch");

                context.Courses.AddRange(
                    new Course
                    {
                        Name = "Computing Fundamentals",
                        BranchId = dublinBranch.Id,
                        StartDate = new DateTime(2026, 1, 10),
                        EndDate = new DateTime(2026, 5, 30)
                    },
                    new Course
                    {
                        Name = "Business Management",
                        BranchId = corkBranch.Id,
                        StartDate = new DateTime(2026, 1, 10),
                        EndDate = new DateTime(2026, 5, 30)
                    },
                    new Course
                    {
                        Name = "Digital Marketing",
                        BranchId = galwayBranch.Id,
                        StartDate = new DateTime(2026, 1, 10),
                        EndDate = new DateTime(2026, 5, 30)
                    }
                );
                await context.SaveChangesAsync();
            }

            if (!context.FacultyProfiles.Any())
            {
                context.FacultyProfiles.Add(new FacultyProfile
                {
                    IdentityUserId = facultyUser!.Id,
                    Name = "John Teacher",
                    Email = facultyEmail,
                    Phone = "0851111111"
                });
                await context.SaveChangesAsync();
            }

            if (!context.StudentProfiles.Any())
            {
                context.StudentProfiles.AddRange(
                    new StudentProfile
                    {
                        IdentityUserId = student1User!.Id,
                        Name = "Alice Student",
                        Email = student1Email,
                        Phone = "0852222222",
                        Address = "Dublin",
                        StudentNumber = "S1001"
                    },
                    new StudentProfile
                    {
                        IdentityUserId = student2User!.Id,
                        Name = "Bob Student",
                        Email = student2Email,
                        Phone = "0853333333",
                        Address = "Cork",
                        StudentNumber = "S1002"
                    }
                );
                await context.SaveChangesAsync();
            }

            if (!context.CourseEnrolments.Any())
            {
                var computingCourse = context.Courses.First(c => c.Name == "Computing Fundamentals");
                var student1 = context.StudentProfiles.First(s => s.Email == student1Email);
                var student2 = context.StudentProfiles.First(s => s.Email == student2Email);

                context.CourseEnrolments.AddRange(
                    new CourseEnrolment
                    {
                        StudentProfileId = student1.Id,
                        CourseId = computingCourse.Id,
                        EnrolDate = DateTime.Now.Date,
                        Status = "Active"
                    },
                    new CourseEnrolment
                    {
                        StudentProfileId = student2.Id,
                        CourseId = computingCourse.Id,
                        EnrolDate = DateTime.Now.Date,
                        Status = "Active"
                    }
                );
                await context.SaveChangesAsync();
            }

            if (!context.Assignments.Any())
            {
                var computingCourse = context.Courses.First(c => c.Name == "Computing Fundamentals");

                context.Assignments.Add(new Assignment
                {
                    CourseId = computingCourse.Id,
                    Title = "C# Basics Project",
                    MaxScore = 100,
                    DueDate = new DateTime(2026, 3, 15)
                });
                await context.SaveChangesAsync();
            }

            if (!context.Exams.Any())
            {
                var computingCourse = context.Courses.First(c => c.Name == "Computing Fundamentals");

                context.Exams.Add(new Exam
                {
                    CourseId = computingCourse.Id,
                    Title = "Semester 2 Exam",
                    Date = new DateTime(2026, 5, 20),
                    MaxScore = 100,
                    ResultsReleased = false
                });
                await context.SaveChangesAsync();
            }

            if (!context.AssignmentResults.Any())
            {
                var assignment = context.Assignments.First();
                var student1 = context.StudentProfiles.First(s => s.Email == student1Email);
                var student2 = context.StudentProfiles.First(s => s.Email == student2Email);

                context.AssignmentResults.AddRange(
                    new AssignmentResult
                    {
                        AssignmentId = assignment.Id,
                        StudentProfileId = student1.Id,
                        Score = 78,
                        Feedback = "Good work"
                    },
                    new AssignmentResult
                    {
                        AssignmentId = assignment.Id,
                        StudentProfileId = student2.Id,
                        Score = 85,
                        Feedback = "Very good work"
                    }
                );
                await context.SaveChangesAsync();
            }

            if (!context.ExamResults.Any())
            {
                var exam = context.Exams.First();
                var student1 = context.StudentProfiles.First(s => s.Email == student1Email);
                var student2 = context.StudentProfiles.First(s => s.Email == student2Email);

                context.ExamResults.AddRange(
                    new ExamResult
                    {
                        ExamId = exam.Id,
                        StudentProfileId = student1.Id,
                        Score = 72,
                        Grade = "B"
                    },
                    new ExamResult
                    {
                        ExamId = exam.Id,
                        StudentProfileId = student2.Id,
                        Score = 88,
                        Grade = "A"
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}