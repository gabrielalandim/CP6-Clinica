using Clinica.Modules.Identity.Application;
using Clinica.Modules.Identity.Infrastructure;
using Clinica.SharedKernel.Modules;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Clinica.Modules.Identity;

public static class IdentityModule
{
    public static Assembly Assembly => typeof(IdentityModule).Assembly;

    public static IServiceCollection AddIdentityModule(this IServiceCollection services)
    {
        services.AddSingleton<IdentityMongoContext>();
        services.AddSingleton<JwtTokenService>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IUsuarioUseCase, UsuarioUseCase>();
        services.AddSingleton<IModuleInitializer, IdentityInitializer>();
        return services;
    }
}
