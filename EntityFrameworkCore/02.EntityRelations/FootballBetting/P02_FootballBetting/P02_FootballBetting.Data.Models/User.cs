namespace P02_FootballBetting.Data.Models;

using P02_FootballBetting.Data.Common;
using System.ComponentModel.DataAnnotations;

public class User
{
    public User()
    {
        this.Bets = new HashSet<Bet>();
    }
    //Must be GUID - string 
    [Key]
    public int UserId { get; set; }

    [Required]
    [MaxLength(ValidationConstants.UserUserNameMaxLength)]
    public string Username { get; set; } = null!;

    //Pasword are saved hashed in the DB
    [Required]
    [MaxLength(ValidationConstants.UserPasswordMaxLength)]
    public string Password { get; set; } = null!;

    [Required]
    [MaxLength(ValidationConstants.UserEmailMaxLength)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(ValidationConstants.UserNameMaxLength)]
    public string Name { get; set; } = null!;
    public decimal Balance { get; set; }

    public virtual ICollection<Bet> Bets { get; set; }
}
