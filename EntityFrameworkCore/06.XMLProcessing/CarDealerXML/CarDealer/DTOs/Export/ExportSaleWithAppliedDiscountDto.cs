namespace CarDealer.DTOs.Export;

using System.Xml.Serialization;

[XmlType("sale")]
public class ExportSaleWithAppliedDiscountDto
{
    [XmlElement("car")]
    public ExportCarSaleDto Car { get; set; }

    [XmlElement("discount")]
    public string Discount { get; set; }

    [XmlElement("customer-name")]
    public string CustomerName { get; set; } = null!;

    [XmlElement("price")]
    public double Price { get; set; }

    [XmlElement("price-with-discount")]
    public double PriceWithDiscount { get; set; }
}
