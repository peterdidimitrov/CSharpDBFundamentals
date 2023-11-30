namespace Invoices.DataProcessor.ImportDto;

using Invoices.Data.Models.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

public class ImportInvoiceDto
{
    [JsonProperty("Number")]
    [Required]
    [Range(1000000000, 1500000000)]
    public int Number { get; set; }

    [JsonProperty("IssueDate")]
    [Required]
    public DateTime IssueDate { get; set; }

    [JsonProperty("DueDate")]
    [Required]
    public DateTime DueDate { get; set; }

    [JsonProperty("Amount")]
    [Required]
    public decimal Amount { get; set; }

    [JsonProperty("CurrencyType")]
    [Required]
    [Range(0,2)]
    public CurrencyType CurrencyType { get; set; }

    [JsonProperty("ClientId")]
    [Required]
    public int ClientId { get; set; }
}
