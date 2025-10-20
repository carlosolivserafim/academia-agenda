using AcademiaAgenda.Data;
using AcademiaAgenda.Domain.Entities;
using AcademiaAgenda.Domain.Enums;
using AcademiaAgenda.Dtos;
using AcademiaAgenda.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Padrao")));

builder.Services.AddScoped<ServicoDeAgendamento>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Configuration.GetSection("Swagger").GetValue<bool>("Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/alunos", async (ApplicationDbContext db) =>
    await db.Alunos.Select(a => new AlunoView(a.Id, a.Nome, a.Plano)).ToListAsync());

app.MapPost("/alunos", async (ApplicationDbContext db, CriarAlunoDto dto) =>
{
    var aluno = new Aluno { Nome = dto.Nome, Plano = dto.Plano };
    db.Alunos.Add(aluno);
    await db.SaveChangesAsync();
    return Results.Created($"/alunos/{aluno.Id}", new AlunoView(aluno.Id, aluno.Nome, aluno.Plano));
});

app.MapGet("/aulas", async (ApplicationDbContext db) =>
    await db.Aulas
        .Select(a => new AulaView(
            a.Id, a.Tipo, a.InicioEm, a.CapacidadeMaxima,
            a.Agendamentos.Count))
        .ToListAsync());

app.MapPost("/aulas", async (ApplicationDbContext db, CriarAulaDto dto) =>
{
    var aula = new Aula { Tipo = dto.Tipo, InicioEm = dto.InicioEm, CapacidadeMaxima = dto.CapacidadeMaxima };
    db.Aulas.Add(aula);
    await db.SaveChangesAsync();
    return Results.Created($"/aulas/{aula.Id}",
        new AulaView(aula.Id, aula.Tipo, aula.InicioEm, aula.CapacidadeMaxima, 0));
});

app.MapPost("/agendamentos", async (ServicoDeAgendamento servico, CriarAgendamentoDto dto) =>
{
    try
    {
        await servico.AgendarAsync(dto.AlunoId, dto.AulaId);
        return Results.Ok(new { mensagem = "Agendamento realizado com sucesso." });
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { erro = ex.Message });
    }
});

app.MapDelete("/agendamentos", async (ApplicationDbContext db, Guid alunoId, Guid aulaId) =>
{
    var ag = await db.Agendamentos.FindAsync(alunoId, aulaId);
    if (ag is null) return Results.NotFound();
    db.Agendamentos.Remove(ag);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/alunos/{alunoId:guid}/agendamentos", async (
    ServicoDeAgendamento servico,
    Guid alunoId,
    CancellationToken ct) =>
{
    try
    {
        var itens = await servico.ListarAgendamentosPorAlunoAsync(alunoId, ct);
        return Results.Ok(itens);
    }
    catch (InvalidOperationException ex) when (ex.Message.Contains("Aluno não encontrado"))
    {
        return Results.NotFound(new { erro = ex.Message });
    }
});

app.MapGet("/relatorios/{alunoId:guid}", async (ServicoDeAgendamento servico, Guid alunoId, int ano, int mes) =>
{
    try
    {
        var rel = await servico.ObterRelatorioMensalAsync(alunoId, ano, mes);
        return Results.Ok(rel);
    }
    catch (InvalidOperationException ex)
    {
        return Results.NotFound(new { erro = ex.Message });
    }
});

/* Adiciona dados para teste
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    if (!db.Alunos.Any())
    {
        db.Alunos.AddRange(
            new Aluno { Nome = "Carlos Oliveira", Plano = TipoPlano.Mensal },
            new Aluno { Nome = "Paula Souza", Plano = TipoPlano.Trimestral },
            new Aluno { Nome = "João Vitor", Plano = TipoPlano.Anual }
        );
    }
    if (!db.Aulas.Any())
    {
        var agora = DateTimeOffset.Now;
        db.Aulas.AddRange(
            new Aula { Tipo = "Cross",     InicioEm = agora.AddDays(1).AddHours(9),  CapacidadeMaxima = 10 },
            new Aula { Tipo = "Funcional", InicioEm = agora.AddDays(2).AddHours(18), CapacidadeMaxima = 8  },
            new Aula { Tipo = "Pilates",   InicioEm = agora.AddDays(3).AddHours(7),  CapacidadeMaxima = 6  }
        );
    }
    db.SaveChanges();
}
*/

app.Run();
