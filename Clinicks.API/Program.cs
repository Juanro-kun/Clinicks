using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Clinicks.Infrastructure.Persistence;
using Clinicks.Infrastructure.Repositories;
using Clinicks.Infrastructure.Security;
using Clinicks.Application.Interfaces;
using Clinicks.Application.Services;
using Clinicks.API.Middlewares;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACION DE SERVICIOS (Dependency Injection) ---

// Base de Datos: Conexion con SQL Server
builder.Services.AddDbContext<ClinicksDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositorios (Infraestructura)
builder.Services.AddScoped<IPacienteRepository, PacienteRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IUbicacionRepository, UbicacionRepository>();
builder.Services.AddScoped<IHabitacionRepository, HabitacionRepository>();
builder.Services.AddScoped<IInternacionRepository, InternacionRepository>();

// Servicios de Negocio
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<IHabitacionService, HabitacionService>();
builder.Services.AddScoped<IInternacionService, InternacionService>();
builder.Services.AddScoped<IUbicacionService, UbicacionService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Proveedores de Seguridad (Infraestructura)
builder.Services.AddScoped<ITokenProvider, JwtTokenProvider>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

// Seguridad JWT: Le ensenamos a la API a validar el "pasaporte" (Token)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// CORS: Permiso para que React (puertos 3000 o 5173) pueda pedirle datos a la API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Clinicks API", Version = "v1" });

    // Configuramos el candadito para el JWT
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Pega tu token aca abajo"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// --- 2. CONFIGURACION DEL PIPELINE (Orden de ejecucion) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Inicializar la base de datos aplicando las migraciones automáticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ClinicksDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

// EL ORDEN ACA ABAJO ES VIDA O MUERTE:
app.UseRouting(); // Organiza las rutas

app.UseCors("AllowReactApp"); // 1ro Viene de un origen permitido?

app.UseAuthentication(); // 2do Quien sos? (Valida el Token)
app.UseAuthorization();  // 3ro Tenes permiso? (Valida el rol/acceso)

app.MapControllers();

app.Run();
