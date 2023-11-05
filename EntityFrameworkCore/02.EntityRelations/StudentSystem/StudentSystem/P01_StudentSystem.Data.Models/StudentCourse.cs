namespace P01_StudentSystem.Data.Models;

using System.ComponentModel.DataAnnotations.Schema;

public class StudentCourse
{
    public int StudentId { get; set; }
    [ForeignKey(nameof(StudentId))]
    public virtual Student Student { get; set; } = null!;


    public int CourseId { get; set; }
    [ForeignKey(nameof(CourseId))]
    public virtual Course Course { get; set; } = null!;
}
