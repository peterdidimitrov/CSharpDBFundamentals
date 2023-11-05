﻿namespace MusicHub.Data.Models;

using System.ComponentModel.DataAnnotations;
using MusicHub.Data.Models.Enums;

public class Writer
{
    public Writer()
    {
        this.Songs = new HashSet<Song>();
    }
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(ValidationConstants.WriterNameMaxLength)]
    public string Name { get; set; }

    public string? Pseudonym { get; set; }

    public virtual ICollection<Song> Songs { get; set; }
}