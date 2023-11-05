namespace P01_StudentSystem.Data.Models;
<<<<<<< Updated upstream

using System.ComponentModel.DataAnnotations.Schema;

public class StudentCourse
{
    public int StudentId { get; set; }
    [ForeignKey(nameof(StudentId))]
    public virtual Student Student { get; set; } = null!;


    public int CourseId { get; set; }
=======
using System.ComponentModel.DataAnnotations.Schema;
public class StudentCourse
{
    public int StudentId { get; set; }

    [ForeignKey(nameof(StudentId))]
    public virtual Student Student { get; set; } = null!;
    public int CourseId { get; set; }

>>>>>>> Stashed changes
    [ForeignKey(nameof(CourseId))]
    public virtual Course Course { get; set; } = null!;
}
