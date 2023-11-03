using P02_FootballBetting.Data.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_FootballBetting.Data.Models;

public class Bet
{
    //Must be GUID
    [Key]
    public int BetId { get; set; }
    public decimal Amount { get; set; }
    public Prediction Prediction { get; set; }

    //Not NULL by default, becouse enums are int in DB
    public DateTime DateTime { get; set; }


    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public virtual User User { get; set; }


    [ForeignKey(nameof(Game))]
    public int GameId { get; set; }
    public virtual Game Game { get; set; }
}
