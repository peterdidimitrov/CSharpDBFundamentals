namespace MusicHub.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SongPerformer
{
    [ForeignKey(nameof(Song))]
    [Required]
    public int SongId { get; set; }
    public virtual Song Song { get; set; }

    [ForeignKey(nameof(Performer))]
    [Required]
    public int PerformerId { get; set; }
    public virtual Performer Performer { get; set; }
}