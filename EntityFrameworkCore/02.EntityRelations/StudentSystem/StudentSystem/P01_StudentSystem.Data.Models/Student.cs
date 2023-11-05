namespace P01_StudentSystem.Data.Models;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

public class Student
{
    public Student()
    {
<<<<<<< Updated upstream
        this.Homeworks = new List<Homework>();
        this.StudentsCourses = new List<StudentCourse>();
=======
        this.StudentsCourses = new HashSet<StudentCourse>();
>>>>>>> Stashed changes
    }

    [Key]
    public int StudentId { get; set; }

    [Required]
    [MaxLength(100)]
    [Unicode]
    public string Name { get; set; } = null!;

    [MaxLength(10)]
<<<<<<< Updated upstream
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

=======
    public string PhoneNumber { get; set; } = null!;
>>>>>>> Stashed changes
    public DateTime RegisteredOn { get; set; }

    public DateTime? Birthday { get; set; }
<<<<<<< Updated upstream

    public virtual ICollection<Homework> Homeworks { get; set; }
    public virtual ICollection<StudentCourse> StudentsCourses { get; set; }
=======
    public virtual ICollection<Course> Courses { get; set; } = null!;
    public virtual ICollection<Homework> Homeworks { get; set; } = null!;
    public virtual ICollection<StudentCourse> StudentsCourses { get; set; } = null!;
>>>>>>> Stashed changes
}