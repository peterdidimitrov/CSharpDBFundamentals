namespace P01_StudentSystem.Data.Models;

using P01_StudentSystem.Data.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class Resource
{
    [Key]
    public int ResourceId { get; set; }

    [Required]
    [MaxLength(50)]
    [Unicode]
    public string Name { get; set; } = null!;

<<<<<<< Updated upstream
    [Required]
    [Unicode(false)]
    public string? Url { get; set; }
=======
    public string Url { get; set; } = null!;
>>>>>>> Stashed changes

    public ResourceType ResourceType { get; set; }

    public int CourseId { get; set; }
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
    [ForeignKey(nameof(CourseId))]
    public virtual Course Course { get; set; } = null!;
}
