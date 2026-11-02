# CP6 – Clínica API (Monolito Modular + MongoDB)

API RESTful em **ASP.NET Core 8 (Controllers)** para gestão de **pacientes** e **consultas** de uma clínica,
organizada como **monolito modular** e persistindo **exclusivamente no MongoDB**.
Projeto do CP6 de *Advanced Business Development with .NET* (FIAP).

> 🎥 **Vídeo de demonstração:** COLE_AQUI_O_LINK_DO_VIDEO
>
> 👥 **Grupo ClyvoPredict · Turma 2TDSPX**
>
> | Integrante | RM |
> |---|---|
> | Maria Gabriela Landim Severo | 565146 |
> | Samara Porto Souza | 559072 |

## Descrição

- **Identity**: cadastro de usuários, login e emissão de JWT (perfis `admin` e `user`).
- **Pacientes**: CRUD de pacientes com busca por nome e paginação.
- **Consultas**: agendamento e cancelamento de consultas, com regra de horário único por especialidade.

## Arquitetura com Modularização

Um único executável (monolito) dividido em **módulos isolados**, cada um com seu próprio contexto de negócio,
suas camadas e **suas próprias coleções no MongoDB**.

```
Clinica.API                         → Host: Program.cs, Swagger, JWT, middleware. Só chama AddXxxModule().
Clinica.SharedKernel                → Kernel compartilhado e "burro": OperationResult, PageResult, LinkDto/HATEOAS,
                                      DomainException, middleware de erros, registro do MongoDB.
Clinica.Modules.Identity            → Módulo Identity   (coleção: usuarios)
Clinica.Modules.Pacientes           → Módulo Pacientes  (coleção: pacientes)
Clinica.Modules.Pacientes.Contracts → Contrato público do módulo Pacientes (IPacientesApi)
Clinica.Modules.Consultas           → Módulo Consultas  (coleção: consultas)
Clinica.Modules.Consultas.Contracts → Contrato público do módulo Consultas (IConsultasApi)
```

```mermaid
flowchart LR
    API[Clinica.API<br/>host] --> ID[Modules.Identity]
    API --> PAC[Modules.Pacientes]
    API --> CON[Modules.Consultas]
    PAC --> CC[Consultas.Contracts]
    CON --> PC[Pacientes.Contracts]
    CON -. implementa .-> CC
    PAC -. implementa .-> PC
    ID --> SK[SharedKernel]
    PAC --> SK
    CON --> SK
    SK --> M[(MongoDB)]
```

Cada módulo tem a mesma organização interna:

```
Domain/          documentos MongoDB e erros customizados do módulo
Application/     UseCases, DTOs, Mappers e a porta do repositório (interface)
Infrastructure/  MongoContext (dono da coleção), Repository, inicializador (índices/seed)
Presentation/    Controllers
XxxModule.cs     único ponto público de registro (AddXxxModule)
```

### Regras de isolamento

| Regra | Como é garantida |
|---|---|
| Baixo acoplamento | Tudo que é interno ao módulo (entidades, repositórios, contexto Mongo, UseCases) é `internal`. Só DTOs, interfaces dos UseCases, controllers e o `XxxModule` são públicos. |
| Comunicação entre módulos só por contrato | **Consultas** precisa saber se o paciente existe e usa `IPacientesApi`; **Pacientes** precisa saber se o paciente tem consultas agendadas antes de ser excluído e usa `IConsultasApi`. Cada módulo referencia apenas o projeto `*.Contracts` do outro, **nunca** o módulo em si (sem referência circular). |
| Dados isolados | Cada módulo acessa só a sua coleção (`PacientesMongoContext`, `ConsultasMongoContext`, `IdentityMongoContext`). Consulta guarda só o `pacienteId`, sem joins entre módulos. |
| Host desacoplado | O host registra os módulos (`AddPacientesModule()` etc.), descobre os controllers (`AddApplicationPart`) e executa os `IModuleInitializer` sem conhecer os detalhes. |

### Padrões utilizados

| Requisito | Onde está |
|---|---|
| **MongoDB exclusivo** | `MongoDB.Driver`; `SharedKernel/Mongo/MongoExtensions.cs` registra `IMongoClient`/`IMongoDatabase`; cada módulo tem um `MongoContext` com sua coleção, índices (únicos e parciais) e mapeamento por atributos/convenções (camelCase, ObjectId, Decimal128). |
| **Operation Result** | `SharedKernel/Results/OperationResult.cs`. Todos os UseCases devolvem `OperationResult<T>` (valor, erro, código do erro e status HTTP); `ToActionResult` converte em HTTP. |
| **Erros customizados** | `DomainException` (código + status) é a base. Cada módulo define os seus: `PacienteNaoEncontradoException`, `PacienteEmailJaCadastradoException`, `PacienteComConsultasAgendadasException`, `HorarioIndisponivelException`, `ConsultaJaCanceladaException`, `DataConsultaNoPassadoException`, `CredenciaisInvalidasException`... Repositórios e UseCases **lançam**; os UseCases **capturam** e devolvem `OperationResult.FromException`; o `DomainExceptionMiddleware` é a rede de segurança (propagação consistente). **Todo** erro da API (domínio, validação de DTO, 401, 403 e 500) sai no mesmo formato `ErrorResponse` (ver abaixo). |
| **PageResult** | `SharedKernel/Results/PageResult.cs`: `items`, `page`, `pageSize`, `totalItems`, `totalPages`, `hasNext`, `hasPrevious`, `links`. Paginação feita no banco (`Skip/Limit`), `pageSize` máximo 50. |
| **HATEOAS** | `LinkDto` (href, rel, method) em cada item (`self`, `update_*`, `delete_*`, `cancelar_consulta`...) e em cada página (`self`, `first`, `prev`, `next`, `last`, `create_*`). Links condicionais: `cancelar_consulta` só aparece se a consulta está agendada. |
| **Autenticação/Autorização** | JWT Bearer. `[Authorize]` nos controllers de negócio; `[Authorize(Roles = "admin")]` em `DELETE /api/pacientes/{id}` e `GET /api/user`. 401 (`NAO_AUTENTICADO`) sem token, 403 (`ACESSO_NEGADO`) sem permissão, ambos com corpo JSON padronizado. Senhas com hash PBKDF2. |

### Formato padrão de erro (`ErrorResponse`)

```json
{
  "code": "PACIENTE_EMAIL_DUPLICADO",
  "error": "Já existe um paciente com o e-mail 'demo@exemplo.com'.",
  "status": 409,
  "traceId": "0HN7...:00000001",
  "timestamp": "2026-11-01T22:10:00Z"
}
```

Erros de validação de DTO trazem também `details` (campo → mensagens), com `code: "VALIDACAO"`.

| Código | Status | Quando |
|---|---|---|
| `VALIDACAO` | 400 | DTO inválido (DataAnnotations) |
| `PACIENTE_ID_INVALIDO` / `CONSULTA_ID_INVALIDO` | 400 | Id que não é ObjectId |
| `DATA_NO_PASSADO` | 400 | Consulta com data passada |
| `NAO_AUTENTICADO` / `CREDENCIAIS_INVALIDAS` | 401 | Sem token / login inválido |
| `ACESSO_NEGADO` | 403 | Perfil sem permissão |
| `PACIENTE_NAO_ENCONTRADO` / `CONSULTA_NAO_ENCONTRADA` / `PACIENTE_INEXISTENTE` | 404 | Recurso não existe |
| `PACIENTE_EMAIL_DUPLICADO` / `USUARIO_JA_EXISTE` | 409 | Índice único violado |
| `HORARIO_INDISPONIVEL` | 409 | Mesmo horário + especialidade já agendado |
| `CONSULTA_JA_CANCELADA` | 409 | Cancelar duas vezes |
| `PACIENTE_COM_CONSULTAS_AGENDADAS` | 409 | Excluir paciente com consulta agendada (regra entre módulos) |
| `ERRO_INTERNO` | 500 | Falha inesperada (sem vazar detalhes) |

## Endpoints

| Método | Rota | Acesso |
|---|---|---|
| POST | `/api/auth` | público (login → JWT) |
| POST | `/api/user` | público (cria usuário `user`) |
| GET | `/api/user?page=1&pageSize=10` | **admin** |
| GET | `/api/pacientes?page=1&pageSize=5&nome=ana` | autenticado |
| GET | `/api/pacientes/{id}` | autenticado |
| POST | `/api/pacientes` | autenticado |
| PUT | `/api/pacientes/{id}` | autenticado |
| DELETE | `/api/pacientes/{id}` | **admin** (bloqueado se houver consulta agendada) |
| GET | `/api/consultas?page=1&pageSize=10&pacienteId=&status=` | autenticado |
| GET | `/api/consultas/{id}` | autenticado |
| POST | `/api/consultas` | autenticado |
| DELETE | `/api/consultas/{id}` | autenticado (cancela) |
| GET | `/health` | público (ping no MongoDB) |

Usuários criados automaticamente na primeira execução:

| E-mail | Senha | Perfil |
|---|---|---|
| `admin@clinica.com` | `Admin@123` | admin |
| `user@clinica.com` | `User@123` | user |

Também são criados 12 pacientes de exemplo, para demonstrar a paginação.

## Como rodar

**Pré-requisitos:** .NET 8 SDK e MongoDB (Docker ou MongoDB Atlas).

1. Suba o MongoDB local:

   ```bash
   docker compose up -d
   ```

2. Rode a API:

   ```bash
   dotnet run --project Clinica.API
   ```

   Na primeira execução são criados os índices e os dados iniciais. Abra **http://localhost:5090/swagger**.

3. No Swagger: `POST /api/auth` → copie o `jwt` → botão **Authorize** → cole o token.

   Alternativa: o arquivo `Clinica.API/Clinica.http` tem todas as requisições da demonstração prontas
   (Visual Studio, Rider ou extensão *REST Client* do VS Code).

## Configurações

| Chave (`appsettings.json`) | Variável de ambiente | Descrição |
|---|---|---|
| `MongoDb:ConnectionString` | `MongoDb__ConnectionString` | Padrão `mongodb://localhost:27017`. Para Atlas: `mongodb+srv://usuario:senha@cluster.../` |
| `MongoDb:DatabaseName` | `MongoDb__DatabaseName` | Padrão `clinica_cp6` |
| `Secretkey` | `Secretkey` | Chave de assinatura do JWT (mínimo 32 caracteres) |

Exemplo (PowerShell) usando Atlas, sem escrever a senha no repositório:

```powershell
$env:MongoDb__ConnectionString = "mongodb+srv://usuario:senha@cluster.mongodb.net/"
dotnet run --project Clinica.API
```

## Consultando direto no MongoDB (para a demonstração)

Com o Docker: `docker exec -it clinica-mongo mongosh clinica_cp6`

```js
show collections                                   // pacientes, consultas, usuarios
db.pacientes.find().sort({ nome: 1 }).limit(3)
db.pacientes.find({ nome: /ana/i })
db.consultas.find({ status: "Agendada" })
db.consultas.countDocuments()
db.pacientes.getIndexes()                          // índice único de e-mail
db.consultas.getIndexes()                          // índice único parcial (horário agendado)
db.usuarios.find({}, { passwordHash: 0 })
```

Ou use o **MongoDB Compass** / **mongo-express** (http://localhost:8081).
