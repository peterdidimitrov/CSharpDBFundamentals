namespace Medicines.DataProcessor.ImportDtos;

using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

[XmlType("Medicine")]
public class ImportMedicineDto
{
    [XmlElement("Name")]
    [Required]
    [MinLength(3)]
    [MaxLength(150)]
    public string Name { get; set; } = null!;

    [Required]
    [Range(0.01, 1000.00)]
    public double Price { get; set; }

    [XmlAttribute("category")]
    [Required]
    [Range(0, 4)]
    public int Category { get; set; }

    [XmlElement("ProductionDate")]
    [Required]
    public string ProductionDate { get; set; } = null!;

    [XmlElement("ExpiryDate")]
    [Required]
    public string ExpiryDate { get; set; } = null!;

    [XmlElement("Producer")]
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string Producer { get; set; } = null!;
}