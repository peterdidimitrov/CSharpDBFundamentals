namespace Invoices.DataProcessor.ImportDto;

using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

[XmlType("Client")]
public class ImportClientDto
{
    [XmlElement("Name")]
    [Required]
    [MinLength(10)]
    [MaxLength(25)]
    public string Name { get; set; } = null!;

    [XmlElement("NumberVat")]
    [Required]
    [MinLength(10)]
    [MaxLength(15)]
    public string NumberVat { get; set; } = null!;

    [XmlArray("Addresses")]
    public ImportAddressDto[] Addresses { get; set; } = null!;
}
