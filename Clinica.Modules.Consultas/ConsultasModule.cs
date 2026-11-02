using Clinica.Modules.Consultas.Application;
using Clinica.Modules.Consultas.Contracts;
using Clinica.Modules.Consultas.Infrastructure;
using Clinica.SharedKernel.Modules;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Clinica.Modules.Consultas;

public static class ConsultasModule
{
    public static Assembly Assembly => typeof(ConsultasModule).Assembly;

    public static IServiceCollection AddConsultasModule(this IServiceCollection services)
    {
        services.AddSingleton<ConsultasMongoContext>();
        services.AddScoped<IConsultaRepository, ConsultaRepository>();
        services.AddScoped<IConsultaUseCase, ConsultaUseCase>();

        // Contrato público exposto para os outros módulos
        services.AddScoped<IConsultasApi, ConsultasApi>();

        services.AddSingleton<IModuleInitializer, ConsultasInitializer>();
        return services;
    }
}
