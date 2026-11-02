using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Clinica.Modules.Pacientes.Domain;

internal class Paciente
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string Cidade { get; set; } = string.Empty;
    public string Uf { get; set; } = string.Empty;

    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
