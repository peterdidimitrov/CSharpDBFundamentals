namespace P01_StudentSystem.Data;

using P01_StudentSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

public class StudentSystemContext : DbContext
{
    public StudentSystemContext()
    {

    }
    public StudentSystemContext(DbContextOptions DbContextOptions) : base(DbContextOptions)
    {

    }

    private const string ConnectionString = "Server=.;Database=...;Integrated Security=true;";


    public virtual DbSet<Student> Students { get; set; } = null!;
    public virtual DbSet<Course> Courses { get; set; } = null!;
    public virtual DbSet<Resource> Resources { get; set; } = null!;
    public virtual DbSet<Homework> Homeworks { get; set; } = null!;
    public virtual DbSet<StudentCourse> StudentsCourses { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudentCourse>(entity =>
        {
            entity.HasKey(pk => new
            {
                pk.StudentId,
                pk.CourseId
            });
        });
    }
}