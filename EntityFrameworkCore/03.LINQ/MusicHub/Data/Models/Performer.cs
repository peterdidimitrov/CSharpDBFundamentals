﻿namespace MusicHub.Data.Models;

using System.ComponentModel.DataAnnotations;
using MusicHub.Data.Models.Enums;
public class Performer
{
    public Performer()
    {
        this.PerformerSongs = new HashSet<SongPerformer>();
    }
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(ValidationConstants.PerformerFirstNameMaxLength)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(ValidationConstants.PerformerLastNameMaxLength)]
    public string LastName { get; set; }

    [Required]
    public int Age { get; set; }

    [Required]
    public decimal NetWorth  { get; set; }

    public virtual ICollection<SongPerformer> PerformerSongs  { get; set; }
}
