using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Clinica.Modules.Identity.Domain;

internal class Usuario
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = Roles.User;
}

public static class Roles
{
    public const string Admin = "admin";
    public const string User = "user";
}
