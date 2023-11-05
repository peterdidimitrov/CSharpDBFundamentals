namespace P01_StudentSystem.Data.Models;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

public class Student
{
    public Student()
    {
        this.Homeworks = new List<Homework>();
        this.StudentsCourses = new List<StudentCourse>();
    }

    [Key]
    public int StudentId { get; set; }

    [Required]
    [MaxLength(100)]
    [Unicode]
    public string Name { get; set; } = null!;

    [MaxLength(10)]
    [Unicode(false)]
    public string? PhoneNumber { get; set; }

    public DateTime RegisteredOn { get; set; }

    public DateTime? Birthday { get; set; }

    public virtual ICollection<Homework> Homeworks { get; set; }
    public virtual ICollection<StudentCourse> StudentsCourses { get; set; }
}