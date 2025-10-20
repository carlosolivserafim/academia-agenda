using System.ComponentModel.DataAnnotations;
using AcademiaAgenda.Domain.Enums;

namespace AcademiaAgenda.Dtos;

public record CriarAlunoDto(
    [Required, MaxLength(120)] string Nome,
    [Required] TipoPlano Plano);

public record AlunoView(Guid Id, string Nome, TipoPlano Plano);