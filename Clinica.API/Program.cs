using Clinica.Modules.Consultas;
using Clinica.Modules.Identity;
using Clinica.Modules.Pacientes;
using Clinica.SharedKernel.Errors;
using Clinica.SharedKernel.Modules;
using Clinica.SharedKernel.Mongo;
using Clinica.SharedKernel.Results;
using Clinica.SharedKernel.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongoDb(builder.Configuration);

builder.Services
    .AddIdentityModule()
    .AddPacientesModule()
    .AddConsultasModule();

builder.Services.AddControllers()
    .AddApplicationPart(IdentityModule.Assembly)
    .AddApplicationPart(PacientesModule.Assembly)
    .AddApplicationPart(ConsultasModule.Assembly)
    .AddPadraoDeErros();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Clínica API - CP6 (Monolito Modular + MongoDB)", Version = "v1" });

    var scheme = new OpenApiSecurityScheme
    {
        Description = "Informe o token JWT (Authorization: Bearer {token}).",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
    };

    c.AddSecurityDefinition("Bearer", scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement { { scheme, new[] { "Bearer" } } });
});

builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

var key = Encoding.UTF8.GetBytes(builder.Configuration["Secretkey"]
    ?? throw new InvalidOperationException("Configure 'Secretkey' no appsettings."));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        NameClaimType = "name",
        RoleClaimType = "role"
    };

    // 401 e 403 também seguem o formato padrão de erro (ErrorResponse)
    options.Events = new JwtBearerEvents
    {
        OnChallenge = async context =>
        {
            context.HandleResponse();
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(ErrorResponse.Create(
                ErrorCodes.NaoAutenticado,
                "Token JWT ausente, inválido ou expirado. Faça login em POST /api/auth.",
                StatusCodes.Status401Unauthorized,
                context.HttpContext.TraceIdentifier));
        },
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(ErrorResponse.Create(
                ErrorCodes.AcessoNegado,
                "Seu perfil não tem permissão para acessar este recurso.",
                StatusCodes.Status403Forbidden,
                context.HttpContext.TraceIdentifier));
        }
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    foreach (var module in scope.ServiceProvider.GetServices<IModuleInitializer>())
    {
        try
        {
            await module.InitializeAsync();
            logger.LogInformation("Módulo {Module} inicializado.", module.ModuleName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Falha ao inicializar o módulo {Module}. Verifique a conexão com o MongoDB.", module.ModuleName);
        }
    }
}

app.UseDomainExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI();

app.UseResponseCompression();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", async (IMongoDatabase db) =>
{
    await db.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1));
    return Results.Ok(new { status = "ok", database = db.DatabaseNamespace.DatabaseName });
}).AllowAnonymous();

app.Run();

public partial class Program { }
