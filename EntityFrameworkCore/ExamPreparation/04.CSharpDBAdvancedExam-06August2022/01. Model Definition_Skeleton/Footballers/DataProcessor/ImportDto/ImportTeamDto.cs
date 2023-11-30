﻿namespace Footballers.DataProcessor.ImportDto;

using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

public class ImportTeamDto
{
    [JsonProperty("Name")]
    [Required]
    [MinLength(3)]
    [MaxLength(40)]
    [RegularExpression(@"^[A-Za-z0-9\s\.\-]{3,}$")]
    public string Name { get; set; } = null!;

    [JsonProperty("Nationality")]
    [Required]
    [MinLength(3)]
    [MaxLength(40)]
    public string Nationality { get; set; } = null!;

    [JsonProperty("Trophies")]
    [Required]
    public int Trophies { get; set; }

    [JsonProperty("Footballers")]
    public int[] FootballersId { get; set; } = null!;
}