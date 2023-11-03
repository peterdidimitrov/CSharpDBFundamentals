﻿namespace P02_FootballBetting.Data;

using Microsoft.EntityFrameworkCore;

using P02_FootballBetting.Data.Models;

using Common;

public class FootballBettingContext : DbContext
{
    //Use it when developing the app
    //When we test the app locally on our PC
    public FootballBettingContext()
    {
            
    }
    //Use by Judge
    //Loading of  the DbContext whith Dependency Injection -> In real app it is useful
    public FootballBettingContext(DbContextOptions options) 
        : base(options)
    {
        
    }

    public DbSet<Bet> Bets { get; set; }

    public DbSet<Color> Colors { get; set; }

    public DbSet<Country> Countries { get; set; }

    public DbSet<Game> Games { get; set; }

    public DbSet<Player> Players { get; set; }

    public DbSet<Position> Positions { get; set; }

    public DbSet<Team> Teams { get; set; }

    public DbSet<Town> Towns { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<PlayerStatistic> PlayersStatistics { get; set; }

    //Conection configuration

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured)
        {
            //set default connection string
            //Someone used epmty constructor of the our DbContext
            optionsBuilder.UseSqlServer(DbConfig.ConnectionString);
        }
        base.OnConfiguring(optionsBuilder);
    }

    //Fluent API and Entities config
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PlayerStatistic>(entity =>
        {
            entity.HasKey(ps => new { ps.GameId, ps.PlayerId });
        });
        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasOne(t => t.PrimaryKitColor)
            .WithMany(c => c.PrimaryKitTeams)
            .HasForeignKey(t => t.PrimaryKitColorId)
            .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(t => t.SecondaryKitColor)
            .WithMany(c => c.SecondaryKitTeams)
            .HasForeignKey(t => t.SecondaryKitColorId)
            .OnDelete(DeleteBehavior.NoAction);
        });
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasOne(g => g.HomeTeam)
            .WithMany(t => t.HomeGames)
            .HasForeignKey(g => g.HomeTeamId)
            .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(g => g.AwayTeam)
            .WithMany(t => t.AwayGames)
            .HasForeignKey(g => g.AwayTeamId)
            .OnDelete(DeleteBehavior.NoAction);
        });
    }

}