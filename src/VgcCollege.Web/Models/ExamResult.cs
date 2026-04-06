using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class ExamResult
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Exam")]
        public int ExamId { get; set; }

        public Exam? Exam { get; set; }

        [Required]
        [Display(Name = "Student Name")]
        public int StudentProfileId { get; set; }

        public StudentProfile? StudentProfile { get; set; }

        [Range(0, 100)]
        public double Score { get; set; }

        [StringLength(10)]
        public string? Grade { get; set; }
    }
}