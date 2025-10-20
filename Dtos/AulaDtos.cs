using System.ComponentModel.DataAnnotations;

namespace AcademiaAgenda.Dtos;

public record CriarAulaDto(
    [Required, MaxLength(60)] string Tipo,
    DateTimeOffset InicioEm,
    [Range(1, 500)] int CapacidadeMaxima);

public record AulaView(Guid Id, string Tipo, DateTimeOffset InicioEm, int CapacidadeMaxima, int Inscritos);