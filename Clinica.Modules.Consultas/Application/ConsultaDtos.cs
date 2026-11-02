using Clinica.SharedKernel.Results;
using System.ComponentModel.DataAnnotations;

namespace Clinica.Modules.Consultas.Application;

public class PostConsultaDto
{
    [Required]
    public string PacienteId { get; set; } = string.Empty;

    [Required]
    public DateTime DataHora { get; set; }

    [Required, StringLength(80)]
    public string Especialidade { get; set; } = string.Empty;

    [Range(0.0, 100000.0)]
    public decimal Valor { get; set; }
}

public class GetConsultaDto
{
    public string Id { get; set; } = string.Empty;
    public string PacienteId { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
    public string Especialidade { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<LinkDto> Links { get; set; } = new();
}
