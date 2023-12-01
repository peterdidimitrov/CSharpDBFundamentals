﻿namespace Artillery.Data.Models;

using System.ComponentModel.DataAnnotations;

public class Manufacturer
{
    public Manufacturer()
    {
        this.Guns = new HashSet<Gun>();
    }
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(40)]
    //[Index(IsUnique = true)]
    public string ManufacturerName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Founded { get; set; } = null!;

    public virtual ICollection<Gun> Guns { get; set; }
}
