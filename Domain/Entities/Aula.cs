using System.ComponentModel.DataAnnotations;

namespace AcademiaAgenda.Domain.Entities;

public class Aula
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(60)]
    public string Tipo { get; set; } = default!; 

    public DateTimeOffset InicioEm { get; set; }

    [Range(1, 500)]
    public int CapacidadeMaxima { get; set; }

    public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
}