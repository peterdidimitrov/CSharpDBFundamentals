﻿namespace Artillery.DataProcessor.ImportDto;

using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

[XmlType("Manufacturer")]
public class ImportManufacturerDto
{
    [XmlElement("ManufacturerName")]
    [Required]
    [MinLength(4)]
    [MaxLength(40)]
    public string ManufacturerName { get; set; } = null!;

    [XmlElement("Founded")]
    [Required]
    [MinLength(10)]
    [MaxLength(100)]
    public string Founded { get; set; } = null!;
}