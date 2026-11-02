using Clinica.Modules.Consultas.Application;
using Clinica.SharedKernel.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.Modules.Consultas.Presentation;

[ApiController]
[Authorize]
[Route("api/consultas")]
public class ConsultasController : ControllerBase
{
    private readonly IConsultaUseCase _useCase;

    public ConsultasController(IConsultaUseCase useCase) => _useCase = useCase;

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? pacienteId = null,
        [FromQuery] string? status = null) =>
        this.ToActionResult(await _useCase.ObterTodosAsync(page, pageSize, pacienteId, status));

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id) =>
        this.ToActionResult(await _useCase.ObterUmAsync(id));

    [HttpPost]
    public async Task<IActionResult> Post(PostConsultaDto model) =>
        this.ToActionResult(await _useCase.AgendarAsync(model));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id) =>
        this.ToActionResult(await _useCase.CancelarAsync(id));
}
