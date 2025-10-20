using System.ComponentModel.DataAnnotations;
using AcademiaAgenda.Domain.Enums;

namespace AcademiaAgenda.Domain.Entities;

public class Aluno
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(120)]
    public string Nome { get; set; } = default!;

    [Required]
    public TipoPlano Plano { get; set; }

    public ICollection<Agendamento> Agendamentos { get; set; } = new List<Agendamento>();
}