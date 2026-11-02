namespace Clinica.SharedKernel.Modules;

public interface IModuleInitializer
{
    string ModuleName { get; }
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
