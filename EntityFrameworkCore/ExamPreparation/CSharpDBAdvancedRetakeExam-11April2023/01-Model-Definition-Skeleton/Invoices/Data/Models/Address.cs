using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Invoices.Data.Models;

public class Address
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string StreetName { get; set; } = null!;

    [Required]
    public int StreetNumber { get; set; }

    [Required]
    public string PostCode { get; set; } = null!;

    [Required]
    [MaxLength(15)]
    public string City { get; set; } = null!;

    [Required]
    [MaxLength(15)]
    public string Country { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(Client))]
    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;
}

//•	StreetName – text with length[10…20] (required)

//•	City – text with length[5…15] (required)
//•	Country – text with length[5…15] (required)
