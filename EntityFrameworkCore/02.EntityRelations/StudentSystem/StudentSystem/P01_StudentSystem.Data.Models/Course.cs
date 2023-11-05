namespace P01_StudentSystem.Data.Models;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

public class Course
{
    public Course()
    {
        this.Resources = new List<Resource>();
        this.Homeworks = new List<Homework>();
        this.StudentsCourses = new List<StudentCourse>();
    }

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

    public virtual ICollection<Resource> Resources { get; set; }
    public virtual ICollection<Homework> Homeworks { get; set; }
    public virtual ICollection<StudentCourse> StudentsCourses { get; set; }
}