using System.ComponentModel.DataAnnotations;
using AcademiaAgenda.Domain.Enums;

namespace AcademiaAgenda.Dtos;

public record CriarAgendamentoDto(
    [Required] Guid AlunoId,
    [Required] Guid AulaId);

public record AgendamentoView(Guid AlunoId, Guid AulaId, DateTimeOffset CriadoEm);

public record AgendamentoDetalheView(
    Guid AlunoId,
    string NomeAluno,
    TipoPlano Plano,
    Guid AulaId,
    string TipoAula,
    DateTimeOffset InicioEm,
    int CapacidadeMaxima,
    DateTimeOffset CriadoEm
);