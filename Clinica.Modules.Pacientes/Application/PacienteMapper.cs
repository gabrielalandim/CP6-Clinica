using Clinica.Modules.Pacientes.Domain;
using Clinica.SharedKernel.Results;

namespace Clinica.Modules.Pacientes.Application;

internal static class PacienteMapper
{
    public static GetPacienteDto ToDto(this Paciente p) => new()
    {
        Id = p.Id,
        Nome = p.Nome,
        Email = p.Email,
        Telefone = p.Telefone,
        Cidade = p.Cidade,
        Uf = p.Uf,
        Links = Links(p.Id)
    };

    public static Paciente ToEntity(this PostPacienteDto dto) => new()
    {
        Nome = dto.Nome.Trim(),
        Email = dto.Email.Trim().ToLowerInvariant(),
        Telefone = dto.Telefone?.Trim(),
        Cidade = dto.Cidade.Trim(),
        Uf = dto.Uf.Trim().ToUpperInvariant()
    };

    public static List<LinkDto> Links(string id) => new()
    {
        new($"/api/pacientes/{id}", "self", "GET"),
        new($"/api/pacientes/{id}", "update_paciente", "PUT"),
        new($"/api/pacientes/{id}", "delete_paciente", "DELETE"),
        new($"/api/consultas?pacienteId={id}", "consultas_do_paciente", "GET")
    };
}
