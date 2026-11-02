using Clinica.Modules.Identity.Application;
using Clinica.SharedKernel.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinica.Modules.Identity.Presentation;

[ApiController]
[AllowAnonymous]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioUseCase _useCase;

    public AuthController(IUsuarioUseCase useCase) => _useCase = useCase;

    [HttpPost]
    public async Task<IActionResult> Post(PostAuthDto model) =>
        this.ToActionResult(await _useCase.AutenticarAsync(model));
}
