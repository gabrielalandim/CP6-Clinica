using Clinica.Modules.Pacientes.Application;
using Clinica.SharedKernel.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.Modules.Pacientes.Presentation;

[ApiController]
[Authorize]
[Route("api/pacientes")]
public class PacientesController : ControllerBase
{
    private readonly IPacienteUseCase _useCase;

    public PacientesController(IPacienteUseCase useCase) => _useCase = useCase;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? nome = null) =>
        this.ToActionResult(await _useCase.ObterTodosAsync(page, pageSize, nome));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id) =>
        this.ToActionResult(await _useCase.ObterUmAsync(id));

    [HttpPost]
    public async Task<IActionResult> Post(PostPacienteDto model) =>
        this.ToActionResult(await _useCase.AdicionarAsync(model));

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, PutPacienteDto model) =>
        this.ToActionResult(await _useCase.AtualizarAsync(id, model));

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Delete(string id) =>
        this.ToActionResult(await _useCase.DeletarAsync(id));
}
