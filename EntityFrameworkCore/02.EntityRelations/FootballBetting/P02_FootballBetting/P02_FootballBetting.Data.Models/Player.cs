namespace P02_FootballBetting.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common;

public class Player
{
    public Player()
    {
        this.PlayersStatistics = new HashSet<PlayerStatistic>();
    }
    [Key]
    public int PlayerId { get; set; }
    [Required]
    [MaxLength(ValidationConstants.PlayerNameMaxLength)]
    public string Name { get; set; }
    public int SquadNumber { get; set; }

    //By default is required
    public bool IsInjured { get; set; }

    //Judge
    [ForeignKey(nameof(Team))]
    public int TeamId { get; set; }
    public virtual Team Team { get; set; } = null!;


    [ForeignKey(nameof(Position))]
    public int PositionId { get; set; }
    public virtual Position Position { get; set; }

    public virtual ICollection<PlayerStatistic> PlayersStatistics { get; set; }
}
