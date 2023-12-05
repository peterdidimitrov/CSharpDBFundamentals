using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Theatre.DataProcessor.ImportDto;

[XmlType("Play")]
public class ImportPlayDto
{
    [XmlElement("Title")]
    [Required]
    [MinLength(4)]
    [MaxLength(50)]
    public string Title { get; set; }

    [XmlElement("Duration")]
    [Required]
    public string Duration { get; set; }

    [Required]
    [Range(0.00, 10.00)]
    [XmlElement("Raiting")]
    public float Rating { get; set; }

    [XmlElement("Genre")]
    [Required]
    public string Genre { get; set; }

    [XmlElement("Description")]
    [Required]
    [MaxLength(700)]
    public string Description { get; set; }

    [XmlElement("Screenwriter")]
    [Required]
    [MinLength(4)]
    [MaxLength(30)]
    public string Screenwriter { get; set; }

}