namespace P01_StudentSystem.Data.Models;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


public class Student
{
    [Key]
    public int StudentId { get; set; }

    [Required]
    [MaxLength(100)]
    [Unicode]
    public string? Name { get; set;}

    [MaxLength(10)]
    public string? PhoneNumber { get; set; }
    public DateTime RegisteredOn { get; set; }
    public DateTime? Birthday { get; set; }
    public ICollection<Course>? Courses { get; set; }
    public ICollection<Homework>? Homeworks { get; set; }
    public virtual ICollection<StudentCourse>? StudentsCourses { get; set; }
}