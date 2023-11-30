namespace Footballers.DataProcessor.ImportDto;

using Footballers.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

[XmlType("Footballer")]
public class ImportFootballerDto
{
    [XmlElement("Name")]
    [Required]
    [MinLength(2)]
    [MaxLength(40)]
    public string Name { get; set; } = null!;

    [XmlElement("ContractStartDate")]
    [Required]
    public string ContractStartDate { get; set; }

    [XmlElement("ContractEndDate")]
    [Required]
    public string ContractEndDate { get; set; }

    [XmlElement("BestSkillType")]
    [Range(0,4)]
    [Required]
    public int BestSkillType { get; set; }

    [XmlElement("PositionType")]
    [Range(0, 3)]
    [Required]
    public int PositionType { get; set; }
}