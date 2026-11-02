using Clinica.SharedKernel.Results;
using System.ComponentModel.DataAnnotations;

namespace Clinica.Modules.Pacientes.Application;

public class PostPacienteDto
{
    [Required, StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Telefone { get; set; }

    [Required, StringLength(100)]
    public string Cidade { get; set; } = string.Empty;

    [Required, StringLength(2, MinimumLength = 2)]
    public string Uf { get; set; } = string.Empty;
}

public class PutPacienteDto : PostPacienteDto { }

public class GetPacienteDto
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string Cidade { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;
    public List<LinkDto> Links { get; set; } = new();
}
