[Coldmart] - Plataforma de cursos online
1. Apresentação
Bem-vindo ao repositório do projeto [Coldmart]. Este projeto é uma entrega do MBA DevXpert Full Stack .NET e é referente ao módulo `Pipeline CI/CD, Docker e Kubernetes`. O objetivo principal é evoluir o projeto da entrega do módulo anterior, transformando a solução de microserviços com um pipeline de CI/CD completos e fazendo deploy com kubernetes.

Autor(es)

✅ Cristian Kruger Silva - @mr.krug3r
✅ Gilberto Moshim Yabiku Junior - @junmoriyama3d
✅ Tiago Henrique de Castilhos - @zsfnightmare
✅ Victor Higaki - @victorhigaki

2. Proposta do Projeto

O projeto consiste em uma plataforma de cursos online implementada como um ecossistema de microsserviços, onde cada domínio possui seu próprio serviço, banco de dados isolado e comunicação assíncrona via mensageria.

**Bounded Contexts implementados:**

| Serviço | Responsabilidade |
|---|---|
| `Coldmart.Auth.API` | Cadastro e autenticação de usuários, emissão de JWT |
| `Coldmart.Cursos.API` | Gestão de cursos e aulas (Admin); consulta de catálogo (Aluno) |
| `Coldmart.Alunos.API` | Matrícula, progresso de aulas, histórico, finalização e certificados |
| `Coldmart.Pagamentos.API` | Criação, aprovação e cancelamento de pagamentos |
| `Coldmart.BFF` | Backend for Frontend — agrega chamadas para o cliente |

**Comunicação entre serviços:**

- **Síncrona (HTTP):** O BFF encaminha as requisições do cliente para os serviços de domínio.
- **Assíncrona (RabbitMQ + MassTransit):** Eventos de domínio propagam mudanças de estado entre contextos.

**Fluxo matrícula → pagamento (distribuído):**

```
1. Aluno realiza matrícula → Alunos.API persiste Matricula com status AguardandoPagamento
2. Alunos.API publica MatriculaRealizadaEvento { MatriculaId, AlunoId, CursoId } no RabbitMQ
3. Pagamentos.API consome MatriculaRealizadaEvento → materializa Matricula local com o mesmo MatriculaId
4. Aluno cria pagamento via BFF → Pagamentos.API valida MatriculaId existente e cria Pagamento
5. Admin aprova/cancela pagamento → Pagamentos.API publica PagamentoRealizadoEvento ou PagamentoCanceladoEvento
6. Alunos.API consome o evento → atualiza status da matrícula (Iniciado ou Cancelado)
```

**Eventos de domínio (src/Coldmart.Core/Eventos):**

| Evento | Publicado por | Consumido por |
|---|---|---|
| `MatriculaRealizadaEvento` | Alunos.API | Pagamentos.API |
| `PagamentoRealizadoEvento` | Pagamentos.API | Alunos.API |
| `PagamentoCanceladoEvento` | Pagamentos.API | Alunos.API |
| `AulaRealizadaEvento` | Alunos.API | Alunos.API (interno) |

3. Tecnologias Utilizadas

Linguagem de Programação: C#

Frameworks:

- ASP.NET Core Web API
- Entity Framework Core
- MassTransit (abstração de mensageria)
- MediatR (CQRS interno por serviço)
- Polly (resiliência HTTP no BFF)

Banco de Dados: SQL Server (banco isolado por serviço)

Mensageria: RabbitMQ

Autenticação e Autorização:

- ASP.NET Core Identity (Auth.API)
- JWT (JSON Web Token) validado em todos os serviços via `AddCoreServices`
- Perfis: `Admin` (gestão) e `Aluno` (consumo)

Documentação da API: Swagger

Containerização e virtualização: Docker / docker-compose

4. Estrutura do Projeto

A estrutura do projeto é organizada da seguinte forma:

```
src/
  Coldmart.Auth.*            - Contexto de autenticação
  Coldmart.Cursos.*          - Contexto de cursos e aulas
  Coldmart.Alunos.*          - Contexto de alunos, matrículas e certificados
  Coldmart.Pagamentos.*      - Contexto de pagamentos
  Coldmart.BFF/              - Backend for Frontend
  Coldmart.Core/             - Contratos compartilhados (eventos, extensões, base)
  Coldmart.Core.Data/        - Infraestrutura de dados compartilhada (seeder, EF)

tests/
  Coldmart.*.Tests           - Testes unitários por camada/domínio
```

Cada contexto segue a separação em camadas:

- `*.Domain/` — Entidades, enums e regras de domínio
- `*.Business/` — Handlers MediatR, ViewModels, orquestração de casos de uso
- `*.Data/` — DbContext, configurações EF, migrations e seeders
- `*.API/` — Controllers, consumers MassTransit, extensões de startup

Arquivos raiz:

- `README.md` — Documentação do projeto
- `FEEDBACK.md` — Consolidação dos feedbacks recebidos
- `docker-compose.yml` — Orquestração local completa (APIs + SQL Server + RabbitMQ)
- `sql/init-db.sql` — Script de criação dos bancos isolados por serviço

5. Funcionalidades Implementadas

Casos de uso especificados no documento do projeto, que pode ser acessado [aqui](./docs/Projeto-Quarto-Modulo-Mba-DevXpert.pdf).

Autenticação e Autorização: Diferenciação entre alunos e administradores com JWT e perfis por endpoint.

API RESTful: Exposição de use cases via API com contratos alinhados entre BFF e serviços de domínio.

Mensageria assíncrona: Propagação de eventos entre contextos via RabbitMQ/MassTransit com retry incremental e circuit breaker em todas as APIs participantes.

Resiliência: Polly (retry + circuit breaker) nas chamadas HTTP do BFF; MassTransit retry + circuit breaker nas integrações assíncronas internas.

Documentação da API: Documentação automática dos endpoints da API utilizando Swagger.

6. Como Executar o Projeto

Pré-requisitos

.NET SDK 9.0 ou superior

Visual Studio 2022 ou superior (ou qualquer IDE de sua preferência)

Git

Docker

Passos para Execução

Clone o Repositório:

git clone https://github.com/seu-usuario/nome-do-repositorio.git
cd nome-do-repositorio

Uma vez na pasta raíz do projeto, rode o comando: `docker compose up`

Os serviços e as dependências serão buildados (ou baixados dos respectivos registries).

6.1 Credenciais dos usuários padrões (criados via seed)

## Admin
### Email
admin@coldmart.com
### Senha
Admin@123

## Aluno
### Email
aluno@coldmart.com
### Senha
Aluno@123

6.2 Remoção do suporte ao SQLite

A partir do EF Core (9 e superiores), alguns comportamentos mudaram com relação à aplicar a migration em um banco de dados, uma delas é [esse novo comportamento](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-9.0/breaking-changes#exception-is-thrown-when-applying-migrations-if-there-are-pending-model-changes).

É possível ignorar esse erro configurando o EF da seguinte maneira:
```dotnet
options.ConfigureWarnings(w =>
    w.Ignore(RelationalEventId.PendingModelChangesWarning));
```
Ao configurar o model builder dessa forma, percebemos o seguinte erro:
```
Unhandled exception. Microsoft.Data.SqlClient.SqlException (0x80131904): Column 'Id' in table 'Curso' is of a type that is invalid for use as a key column in an index.
```
Isso indica que ao menos a versão do sql server utilizada no projeto não suporta o data type `text` como Id. Embora seja possível trocar o Id para usar um identificar diferente, como um inteiro com auto increment por exemplo, o suporte ao SQLite não deveria ser um fator de decisão para tal mudança. Considerando isso e também o fato de que o Docker já é um requisito do projeto, não se faz necessário usar o SQLite a fim de facilitar o desenvolvimento, uma vez que se pode simplesmente instanciar um container do SQL Server dentro do mesmo compose.
Outro ponto é que tipos legado como TEXT não são mais utilizados no SQL server e devem ser alterados para usar o respectivo tipo. Armazenar identificadores únicos, datas e outros como TEXT implica em degradação de performance do próprio mecanismo ao realizar filtros ou criar índices para esses campos.

7. Instruções de Configuração
JWT para API: As chaves de configuração do JWT estão no appsettings.json.
Migrações do Banco de Dados: As migrações são gerenciadas pelo Entity Framework Core. Não é necessário aplicar devido a configuração do Seed de dados.

8. Documentação da API
A documentação de cada API está disponível através do Swagger. Após executar o docker compose, abra um navegador e então acesse em:

http://localhost:{ Porta do serviço, especificado no docker-compose.yml }/swagger

9. Considerações do grupo

- O projeto Coldmart.BFF não foi coberto por testes unitários por falta de tempo para implementação

9.1 Status de integração entre BFF e APIs

Todos os contratos HTTP entre o BFF e os serviços de domínio estão alinhados:

| Operação | BFF | API de domínio | Status |
|---|---|---|---|
| Aprovar pagamento | `PUT api/pagamentos/{id}/aprovar` | `[HttpPut("{id}/aprovar")]` | ✅ |
| Cancelar pagamento | `PUT api/pagamentos/{id}/cancelar` | `[HttpPut("{id}/cancelar")]` | ✅ |
| Realizar aula | `PUT api/alunos/cursos/aulas` | `[HttpPut("cursos/aulas")]` | ✅ |
| Obter certificado | `GET api/alunos/certificado/{id}` | `[HttpGet("certificado/{id}")]` | ✅ |

9.2 Status do fluxo distribuído matrícula → pagamento

O pipeline de sincronização entre os contextos de Alunos e Pagamentos está implementado de ponta a ponta:

- `MatriculaRealizadaEvento` carrega o `MatriculaId` canônico gerado em Alunos
- `MatriculaRealizadaConsumer` em Pagamentos.API materializa a matrícula local com o **mesmo** `MatriculaId`, garantindo rastreabilidade entre contextos
- A criação de pagamento no `PagamentosService` valida o `MatriculaId` recebido contra o banco local de Pagamentos
- A operação é **idempotente**: o consumer verifica existência antes de inserir, protegendo contra reentrega de mensagem

10. Avaliação

Este projeto é parte de um curso acadêmico e não aceita contribuições externas.

Para feedbacks ou dúvidas utilize o recurso de Issues

O arquivo FEEDBACK.md é um resumo das avaliações do instrutor e deverá ser modificado apenas por ele.