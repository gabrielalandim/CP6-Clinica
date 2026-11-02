using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Clinica.Modules.Consultas.Domain;

public static class StatusConsulta
{
    public const string Agendada = "Agendada";
    public const string Cancelada = "Cancelada";
}

internal class Consulta
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    public string PacienteId { get; set; } = string.Empty;

    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime DataHora { get; set; }

    public string Especialidade { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Valor { get; set; }

    public string Status { get; set; } = StatusConsulta.Agendada;
}
