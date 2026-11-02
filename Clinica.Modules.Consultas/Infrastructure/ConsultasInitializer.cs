using Clinica.SharedKernel.Modules;

namespace Clinica.Modules.Consultas.Infrastructure;

internal sealed class ConsultasInitializer : IModuleInitializer
{
    private readonly ConsultasMongoContext _context;

    public ConsultasInitializer(ConsultasMongoContext context) => _context = context;

    public string ModuleName => "Consultas";

    public Task InitializeAsync(CancellationToken cancellationToken = default) =>
        _context.EnsureIndexesAsync(cancellationToken);
}
