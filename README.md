# AcademiaAgenda — Módulo de Agendamento de Aulas

Projeto **.NET 8 Web API** para agendamento de **aulas coletivas** em academia.  
**Escopo do teste:** cadastro de alunos e aulas, agendamento com regras de negócio por plano/capacidade, e relatório mensal por aluno.

## Stack
- C# / .NET 8
- EF Core 8 + SQLite
- Swagger

## Como rodar
Dentro da pasta **AcademiaAgenda**
> dotnet restore<br>
> dotnet ef database update<br>
> dotnet run

## Domínio
- Aluno (Id, Nome, Plano)
- Aula (Id, Tipo, InicioEm, CapacidadeMaxima)
- Agendamento (AlunoId, AulaId, CriadoEm)

## Regras
- Plano Mensal = 12, Trimestral = 20, Anual = 30 agendamentos/mês
- Aula não ultrapassa a capacidade
- Contagem mensal considera o mês da data da aula

## Decisões técnicas
- Regras críticas no ServicoDeAgendamento
- PK composta em Agendamento evita duplicidade aluno/aula

## Próximos passos
- Cancelamento com limite de antecedência
- Espera (waitlist) quando a aula lota
- Migração para PostgreSQL (Npgsql) com docker-compose
