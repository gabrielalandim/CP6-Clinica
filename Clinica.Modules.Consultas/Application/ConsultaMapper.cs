using Clinica.Modules.Consultas.Domain;
using Clinica.SharedKernel.Results;

namespace Clinica.Modules.Consultas.Application;

internal static class ConsultaMapper
{
    public static GetConsultaDto ToDto(this Consulta c)
    {
        var links = new List<LinkDto>
        {
            new($"/api/consultas/{c.Id}", "self", "GET"),
            new($"/api/pacientes/{c.PacienteId}", "paciente", "GET")
        };

        if (c.Status == StatusConsulta.Agendada)
            links.Add(new($"/api/consultas/{c.Id}", "cancelar_consulta", "DELETE"));

        return new GetConsultaDto
        {
            Id = c.Id,
            PacienteId = c.PacienteId,
            DataHora = c.DataHora,
            Especialidade = c.Especialidade,
            Valor = c.Valor,
            Status = c.Status,
            Links = links
        };
    }
}
