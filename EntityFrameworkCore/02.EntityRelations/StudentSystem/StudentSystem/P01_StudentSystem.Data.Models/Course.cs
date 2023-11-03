namespace P01_StudentSystem.Data.Models;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class Course
{
    [Key]
    public int CourseId { get; set; }

    [Required]
    [MaxLength(80)]
    [Unicode]
    public string? Name { get; set; }

    [Unicode]
    public string? Description { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Required]
    public decimal Price { get; set; }

    public virtual ICollection<Student>? Students { get; set; }
    public virtual ICollection<Resource>? Resources { get; set; }
    public virtual ICollection<Homework>? Homeworks { get; set; }
    public virtual ICollection<StudentCourse>? StudentsCourses { get; set; }
}