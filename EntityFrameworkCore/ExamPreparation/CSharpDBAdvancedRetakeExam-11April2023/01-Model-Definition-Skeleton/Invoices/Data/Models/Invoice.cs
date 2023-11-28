﻿using Invoices.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Invoices.Data.Models;

public class Invoice
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int Number { get; set; }

    [Required]
    public DateTime IssueDate { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public CurrencyType CurrencyType { get; set; }

    [ForeignKey(nameof(Client))]
    [Required]
    public int ClientId { get; set; }

    public virtual Client Client { get; set; } = null!;
}


//•	Number – integer in range[1, 000, 000, 000…1, 500, 000, 000] (required)
