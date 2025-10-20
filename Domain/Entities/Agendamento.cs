namespace AcademiaAgenda.Domain.Entities;

public class Agendamento
{
    public Guid AlunoId { get; set; }
    public Aluno Aluno { get; set; } = default!;

    public Guid AulaId { get; set; }
    public Aula Aula { get; set; } = default!;

    public DateTimeOffset CriadoEm { get; set; } = DateTimeOffset.UtcNow;
}