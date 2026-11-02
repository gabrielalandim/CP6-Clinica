# Roteiro do vídeo CP6 (máx. 5 min) — ensaie uma vez com cronômetro

Antes de gravar: `docker compose up -d`, API rodando, Swagger e Compass (ou mongosh) abertos. Se preferir, use as requisições prontas de `Clinica.API/Clinica.http`.

| Tempo | O que mostrar | O que falar |
|---|---|---|
| 0:00–0:40 | Solution Explorer: SharedKernel, 3 módulos, Contracts e API. Abrir o csproj de Consultas | "Monolito modular. Cada módulo tem Domain, Application, Infrastructure e Presentation. Consultas só referencia o contrato de Pacientes, nunca o módulo." |
| 0:40–1:10 | Swagger: `GET /api/pacientes` sem token → **401** `NAO_AUTENTICADO`. Login `user@clinica.com` → Authorize | "Autenticação JWT. Até o 401 vem no nosso formato padrão de erro." |
| 1:10–1:50 | `POST /api/pacientes` → 201. Em seguida abrir o Compass/mongosh e mostrar o documento na coleção `pacientes` | "Persistência exclusiva no MongoDB, cada módulo com sua coleção." |
| 1:50–2:30 | Repetir o mesmo e-mail → **409** (`PACIENTE_EMAIL_DUPLICADO`). `GET /api/pacientes/ffffffffffffffffffffffff` → **404**. Id `abc` → **400**. (Opcional) POST com e-mail inválido → **400** `VALIDACAO` com `details`. Mostrar rapidamente `PacienteUseCase` retornando `OperationResult` | "Erros customizados com código e status. Os UseCases devolvem Operation Result e o controller só converte em HTTP." |
| 2:30–3:10 | `POST /api/consultas` → 201 (mostrar o link `cancelar_consulta`). Mesmo horário → **409**. Paciente inexistente → **404** | "A validação do paciente passa pelo contrato IPacientesApi. O horário é protegido por índice único parcial." |
| 3:10–3:50 | `GET /api/pacientes?page=2&pageSize=5` | "PageResult: totalPages, totalItems, hasNext e links first/prev/next/last. HATEOAS em cada item." |
| 3:50–4:20 | mongosh: `db.pacientes.find({nome:/ana/i})`, `db.consultas.getIndexes()` | "Consultas diretamente no MongoDB." |
| 4:20–5:00 | `DELETE /api/pacientes/{id}` como user → **403** `ACESSO_NEGADO`. Login `admin@clinica.com` → Authorize → DELETE do paciente com consulta → **409** `PACIENTE_COM_CONSULTAS_AGENDADAS` (regra via `IConsultasApi`). Cancelar a consulta → DELETE de novo → **204** | "Acesso bloqueado versus permitido por perfil, e regra entre módulos só por contrato." |

Suba como **não listado** no YouTube e teste o link em aba anônima.
