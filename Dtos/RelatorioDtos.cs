namespace AcademiaAgenda.Dtos;

public record RelatorioMensalAluno(
    Guid AlunoId,
    string NomeAluno,
    int Ano, int Mes,
    int TotalAulas,
    IEnumerable<string> TiposMaisFrequentes);