namespace P01_StudentSystem.Data;

using P01_StudentSystem.Data.Models;
using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Common;

public class StudentSystemContext : DbContext
{
    public StudentSystemContext()
<<<<<<< Updated upstream
    {

    }
    public StudentSystemContext(DbContextOptions DbContextOptions) : base(DbContextOptions)
=======
>>>>>>> Stashed changes
    {

    }
    public StudentSystemContext(DbContextOptions DbContextOptions)
        : base(DbContextOptions)
    {

<<<<<<< Updated upstream
    private const string ConnectionString = "Server=.;Database=...;Integrated Security=true;";

=======
    }
>>>>>>> Stashed changes

    public virtual DbSet<Student> Students { get; set; } = null!;
    public virtual DbSet<Course> Courses { get; set; } = null!;
    public virtual DbSet<Resource> Resources { get; set; } = null!;
    public virtual DbSet<Homework> Homeworks { get; set; } = null!;
    public virtual DbSet<StudentCourse> StudentsCourses { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
<<<<<<< Updated upstream
            optionsBuilder.UseSqlServer(ConnectionString);
        }
=======
            optionsBuilder.UseSqlServer(DbConfig.ConnectionString);
        }

        base.OnConfiguring(optionsBuilder);
>>>>>>> Stashed changes
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudentCourse>()
            .HasKey(pk => new
            {
                pk.StudentId,
                pk.CourseId
            });
<<<<<<< Updated upstream
        });
=======
>>>>>>> Stashed changes
    }
}