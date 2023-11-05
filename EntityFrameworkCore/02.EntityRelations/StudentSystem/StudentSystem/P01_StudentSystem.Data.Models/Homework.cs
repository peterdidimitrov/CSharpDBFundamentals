namespace P01_StudentSystem.Data.Models;

using P01_StudentSystem.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;

public class Homework
{
    [Key]
    public int HomeworkId { get; set; }

    [Required]
    [Unicode(false)]
    public string Content { get; set; } = null!;

    public ContentType ContentType { get; set; }

    public DateTime SubmissionTime { get; set; }


    public int StudentId { get; set; }
    [ForeignKey(nameof(StudentId))]
    public virtual Student Student { get; set; } = null!;


    public int CourseId { get; set;}
    [ForeignKey(nameof(CourseId))]
    public virtual Course Course { get; set; } = null!;
}