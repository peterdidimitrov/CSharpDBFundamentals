using Microsoft.EntityFrameworkCore;
namespace P01_StudentSystem.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class Resource
{
    [Key]
    public int ResourcesId { get; set; }
    
    [Required]
    [MaxLength(50)]
    [Unicode]
    public string? Name { get; set; }
    [Required]
    public string? Url { get; set; }

    public ResourceType ResourceType { get; set; }
    [Required]
    public int? CourseId { get; set; }

    [ForeignKey(nameof(CourseId))]
    public virtual Course? Course { get; set; }
}
