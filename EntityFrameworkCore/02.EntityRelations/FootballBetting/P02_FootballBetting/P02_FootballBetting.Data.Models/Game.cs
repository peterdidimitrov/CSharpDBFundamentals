﻿
namespace P02_FootballBetting.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common;

public class Game
{
    public Game()
    {
        this.PlayersStatistics = new HashSet<PlayerStatistic>();
        this.Bets = new HashSet<Bet>();
    }
    // This shoud be GUID - string
    [Key]
    public int GameId { get; set; }

    [ForeignKey(nameof(HomeTeam))]
    public int HomeTeamId { get; set; }

    public virtual Team HomeTeam { get; set; }

    [ForeignKey(nameof(AwayTeam))]
    public int AwayTeamId { get; set; }
    public virtual Team AwayTeam { get; set; }

    public byte HomeTeamGoals { get; set; }

    public byte AwayTeamGoals { get; set; }

    //Required by default
    public DateTime DateTime { get; set; }

    public double HomeTeamBetRate { get; set; }

    public double AwayTeamBetRate { get; set; }

    public double DrawBetRate { get; set; }

    //5:1
    [MaxLength(ValidationConstants.GameResultMaxLength)]
    public string? Result { get; set; }

    public virtual ICollection<PlayerStatistic> PlayersStatistics { get; set; }

    public virtual ICollection<Bet> Bets { get; set; }
}
