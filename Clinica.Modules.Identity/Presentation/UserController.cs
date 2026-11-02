using Clinica.Modules.Identity.Application;
using Clinica.SharedKernel.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.Modules.Identity.Presentation;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly IUsuarioUseCase _useCase;

    public UserController(IUsuarioUseCase useCase) => _useCase = useCase;

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Post(PostUserDto model) =>
        this.ToActionResult(await _useCase.RegistrarAsync(model));

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        this.ToActionResult(await _useCase.ListarAsync(page, pageSize));
}
