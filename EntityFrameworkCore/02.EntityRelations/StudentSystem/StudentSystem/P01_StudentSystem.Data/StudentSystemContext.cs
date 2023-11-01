namespace P01_StudentSystem.Data;

using P01_StudentSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

public class StudentSystemContext : DbContext
{

    public StudentSystemContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {

    }

    private const string ConnectionString = "Server=.;Database=StudentSystem;User Id=...;Password=....;TrustServerCertificate=true";

    public DbSet<Student>? Students { get; set; }
    public DbSet<Course>? Courses { get; set; }
    public DbSet<Resource>? Resources { get; set; }
    public DbSet<Homework>? Homeworks { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlServer(ConnectionString);
    //}
}
