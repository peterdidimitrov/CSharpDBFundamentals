﻿using System.ComponentModel.DataAnnotations;
using Theatre.Data.Models.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace Theatre.Data.Models;

public class Play
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Title { get; set; }

    [Required]
    public TimeSpan Duration { get; set; }

    [Required]
    public float Rating { get; set; }

    [Required]
    public Genre Genre { get; set; }

    [Required]
    [MaxLength(700)]
    public string Description { get; set; }

    [Required]
    [MaxLength(30)]
    public string Screenwriter { get; set; }

    public virtual ICollection<Cast> Casts { get; set; } = new HashSet<Cast>();
    public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
}
