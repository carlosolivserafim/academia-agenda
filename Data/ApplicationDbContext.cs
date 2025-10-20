using AcademiaAgenda.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion; 

namespace AcademiaAgenda.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Aluno> Alunos => Set<Aluno>();
    public DbSet<Aula> Aulas => Set<Aula>();
    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var dtoToString = new DateTimeOffsetToStringConverter();
        modelBuilder.Entity<Aula>()
            .Property(a => a.InicioEm)
            .HasConversion(dtoToString);

        modelBuilder.Entity<Aluno>()
            .HasIndex(a => a.Nome);

        modelBuilder.Entity<Aula>()
            .HasIndex(a => new { a.Tipo, a.InicioEm });

        modelBuilder.Entity<Agendamento>()
            .HasKey(a => new { a.AlunoId, a.AulaId });

        modelBuilder.Entity<Agendamento>()
            .HasOne(a => a.Aluno)
            .WithMany(al => al.Agendamentos)
            .HasForeignKey(a => a.AlunoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Agendamento>()
            .HasOne(a => a.Aula)
            .WithMany(au => au.Agendamentos)
            .HasForeignKey(a => a.AulaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}