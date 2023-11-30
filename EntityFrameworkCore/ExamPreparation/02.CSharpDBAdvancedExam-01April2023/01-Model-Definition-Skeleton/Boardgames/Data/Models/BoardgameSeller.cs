﻿namespace Boardgames.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


public class BoardgameSeller
{
    [ForeignKey(nameof(Boardgame))]
    [Required]
    public int BoardgameId { get; set; }

    public virtual Boardgame Boardgame { get; set; } = null!;

    [ForeignKey(nameof(Seller))]
    [Required]
    public int SellerId { get; set; }

    public virtual Seller Seller { get; set; } = null!;

}
