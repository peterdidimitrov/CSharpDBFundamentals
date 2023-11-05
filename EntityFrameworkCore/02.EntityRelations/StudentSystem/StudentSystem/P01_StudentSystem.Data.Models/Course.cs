namespace P01_StudentSystem.Data.Models;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

public class Course
{
    public Course()
    {
<<<<<<< Updated upstream
        this.Resources = new List<Resource>();
        this.Homeworks = new List<Homework>();
        this.StudentsCourses = new List<StudentCourse>();
    }

=======
        this.StudentsCourses = new HashSet<StudentCourse>();
    }
>>>>>>> Stashed changes
    [Key]
    public int CourseId { get; set; }

    [Required]
    [MaxLength(80)]
    [Unicode]
    public string Name { get; set; } = null!;

    [Unicode]
    public string? Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    public decimal Price { get; set; }

<<<<<<< Updated upstream
    public virtual ICollection<Resource> Resources { get; set; }
    public virtual ICollection<Homework> Homeworks { get; set; }
    public virtual ICollection<StudentCourse> StudentsCourses { get; set; }
=======
    public virtual ICollection<Student> Students { get; set; } = null!;
    public virtual ICollection<Resource> Resources { get; set; } = null!;
    public virtual ICollection<Homework> Homeworks { get; set; } = null!;
    public virtual ICollection<StudentCourse> StudentsCourses { get; set; } = null!;
>>>>>>> Stashed changes
}