namespace Invoices.DataProcessor.ExportDto;

using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

[XmlType("Invoice")]
public class ExportInvoiceDto
{
    [XmlElement("InvoiceNumber")]
    public int InvoiceNumber { get; set; }

    [XmlElement("InvoiceAmount")]
    public decimal InvoiceAmount { get; set; }

    [XmlElement("DueDate")]
    public string DueDate { get; set; } = null!;

    [Required]
    [XmlElement("Currency")]
    public string Currency { get; set; } = null!;
}