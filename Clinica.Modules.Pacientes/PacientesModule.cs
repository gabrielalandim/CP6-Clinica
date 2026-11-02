using Clinica.Modules.Pacientes.Application;
using Clinica.Modules.Pacientes.Contracts;
using Clinica.Modules.Pacientes.Infrastructure;
using Clinica.SharedKernel.Modules;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Clinica.Modules.Pacientes;

public static class PacientesModule
{
    public static Assembly Assembly => typeof(PacientesModule).Assembly;

    public static IServiceCollection AddPacientesModule(this IServiceCollection services)
    {
        services.AddSingleton<PacientesMongoContext>();
        services.AddScoped<IPacienteRepository, PacienteRepository>();
        services.AddScoped<IPacienteUseCase, PacienteUseCase>();

        // Contrato público exposto para os outros módulos
        services.AddScoped<IPacientesApi, PacientesApi>();

        services.AddSingleton<IModuleInitializer, PacientesInitializer>();
        return services;
    }
}
