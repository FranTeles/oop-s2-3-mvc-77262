using System.ComponentModel.DataAnnotations;

namespace VgcCollege.Web.Models
{
    public class Assignment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        public Course? Course { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Range(0, 1000)]
        [Display(Name = "Max Score")]
        public double MaxScore { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Due date")]
        public DateTime DueDate { get; set; }

        public ICollection<AssignmentResult> AssignmentResults { get; set; } = new List<AssignmentResult>();
    }
}