namespace ProductShop.Dtos.Export;

using ProductShop.DTOs.Export;
using System.Xml.Serialization;

[XmlType("SoldProduct")]
public class ExportSoldProductDto
{
    [XmlElement("count")]
    public int Count { get; set; }

    [XmlArray("products")]
    public ExportProductMiniDto[] SoldProducts { get; set; }
}