﻿using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Xml.Linq;

namespace Theatre.Data.Models;

public class Theatre
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(30)]
    public string Name { get; set; }
    
    [Required]
    public sbyte NumberOfHalls { get; set; }

    [Required]
    [MaxLength(30)]
    public string Director { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
}

//•	Name – text with length[4, 30] (required)
//•	NumberOfHalls – sbyte between[1…10] (required)
//•	Director – text with length[4, 30] (required)