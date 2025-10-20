using AcademiaAgenda.Data;
using AcademiaAgenda.Domain.Entities;
using AcademiaAgenda.Domain.Enums;
using AcademiaAgenda.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AcademiaAgenda.Services;

public class ServicoDeAgendamento(ApplicationDbContext db)
{
    public async Task AgendarAsync(Guid alunoId, Guid aulaId, CancellationToken ct = default)
    {
        var aluno = await db.Alunos.FirstOrDefaultAsync(a => a.Id == alunoId, ct)
                    ?? throw new InvalidOperationException("Aluno não encontrado.");

        var aula = await db.Aulas
            .Include(a => a.Agendamentos)
            .FirstOrDefaultAsync(a => a.Id == aulaId, ct)
            ?? throw new InvalidOperationException("Aula não encontrada.");

        var jaAgendado = await db.Agendamentos.AnyAsync(a => a.AlunoId == alunoId && a.AulaId == aulaId, ct);
        if (jaAgendado)
            throw new InvalidOperationException("Aluno já está agendado nesta aula.");

        if (aula.Agendamentos.Count >= aula.CapacidadeMaxima)
            throw new InvalidOperationException("Capacidade máxima da aula atingida.");
        
        var inicioMes = new DateTimeOffset(aula.InicioEm.Year, aula.InicioEm.Month, 1, 0, 0, 0, aula.InicioEm.Offset);
        var proxMes = inicioMes.AddMonths(1);
        
        var aulasNoMesIds = db.Aulas
            .Where(a => a.InicioEm >= inicioMes && a.InicioEm < proxMes)
            .Select(a => a.Id);

        var totalMes = await db.Agendamentos
            .Where(a => a.AlunoId == alunoId && aulasNoMesIds.Contains(a.AulaId))
            .CountAsync(ct);

        var limite = RegrasPlano.ObterLimiteMensal(aluno.Plano);
        if (totalMes >= limite)
            throw new InvalidOperationException($"Limite mensal do plano ({limite}) atingido.");

        db.Agendamentos.Add(new Agendamento { AlunoId = alunoId, AulaId = aulaId });
        await db.SaveChangesAsync(ct);
    }

    public async Task<RelatorioMensalAluno> ObterRelatorioMensalAsync(Guid alunoId, int ano, int mes, CancellationToken ct = default)
    {
        var aluno = await db.Alunos.AsNoTracking().FirstOrDefaultAsync(a => a.Id == alunoId, ct)
                    ?? throw new InvalidOperationException("Aluno não encontrado.");

        var inicio = new DateTimeOffset(ano, mes, 1, 0, 0, 0, TimeSpan.Zero);
        var fim = inicio.AddMonths(1);
        
        var aulasMes = db.Aulas.AsNoTracking()
            .Where(au => au.InicioEm >= inicio && au.InicioEm < fim);

        var agDoAlunoNoMes = aulasMes
            .SelectMany(au => au.Agendamentos
                .Where(ag => ag.AlunoId == alunoId),
                (au, ag) => new { au.Tipo });

        var total = await agDoAlunoNoMes.CountAsync(ct);

        var topTipos = await agDoAlunoNoMes
            .GroupBy(x => x.Tipo)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .Take(3)
            .ToListAsync(ct);

        return new RelatorioMensalAluno(aluno.Id, aluno.Nome, ano, mes, total, topTipos);
    }

    public async Task<List<AgendamentoDetalheView>> ListarAgendamentosPorAlunoAsync(
        Guid alunoId,
        CancellationToken ct = default)
    {
        var existe = await db.Alunos.AsNoTracking().AnyAsync(a => a.Id == alunoId, ct);
        if (!existe)
            throw new InvalidOperationException("Aluno não encontrado.");

        var query =
            from ag in db.Agendamentos.AsNoTracking()
            join al in db.Alunos.AsNoTracking() on ag.AlunoId equals al.Id
            join au in db.Aulas.AsNoTracking() on ag.AulaId equals au.Id
            where al.Id == alunoId
            orderby au.InicioEm
            select new AgendamentoDetalheView(
                al.Id,
                al.Nome,
                al.Plano,
                au.Id,
                au.Tipo,
                au.InicioEm,
                au.CapacidadeMaxima,
                ag.CriadoEm
            );

        return await query.ToListAsync(ct);
    }
}
