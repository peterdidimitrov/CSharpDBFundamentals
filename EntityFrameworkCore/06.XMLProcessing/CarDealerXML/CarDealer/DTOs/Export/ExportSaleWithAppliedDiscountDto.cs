namespace CarDealer.DTOs.Export;

using System.Xml.Serialization;

[XmlType("sale")]
public class ExportSaleWithAppliedDiscountDto
{
    [XmlElement("car")]
    public ExportCarSaleDto Car { get; set; } = null!;

    [XmlElement("discount")]
    public decimal Discount { get; set; }

    [XmlElement("customer-name")]
    public string CustomerName { get; set; } = null!;

    [XmlElement("price")]
    public string Price { get; set; } = null!;

    [XmlElement("price-with-discount")]
    public string PriceWithDiscount { get; set; } = null!;
}
