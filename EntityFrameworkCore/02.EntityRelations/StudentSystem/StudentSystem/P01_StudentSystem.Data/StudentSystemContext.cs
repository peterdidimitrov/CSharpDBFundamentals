namespace P01_StudentSystem.Data;

using P01_StudentSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

public class StudentSystemContext : DbContext
{

    //public StudentSystemContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    //{

    //}

    //private const string ConnectionString = "Server=.;Database=StudentSystem;Integrated Security=true;";

    private const string ConnectionString = "Server=.;Database=StudentSystem;User Id=sa;Password=Pass12345;TrustServerCertificate=true";


    public virtual DbSet<Student> Students { get; set; } = null!;
    public virtual DbSet<Course> Courses { get; set; } = null!;
    public virtual DbSet<Resource> Resources { get; set; } = null!;
    public virtual DbSet<Homework> Homeworks { get; set; } = null!;
    public virtual DbSet<StudentCourse> StudentsCourses { get; set; } = null!;

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.UseSqlServer(ConnectionString);
    //}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<StudentCourse>(entity =>
        {
            entity.HasKey(pk => new
            {
                pk.StudentId,
                pk.CourseId
            });

            entity.HasOne(sc => sc.Student)
                .WithMany(s => s.StudentsCourses)
                .HasForeignKey(sc => sc.StudentId);

            entity.HasOne(sc => sc.Course)
                .WithMany(c => c.StudentsCourses)
                .HasForeignKey(sc => sc.CourseId);
        });
    }
}