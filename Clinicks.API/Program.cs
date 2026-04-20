using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Clinicks.Application.Context;
using Clinicks.Application.Interfaces;
using Clinicks.Application.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE SERVICIOS (Dependency Injection) ---

// Base de Datos: Conexión con SQL Server
builder.Services.AddDbContext<ClinicksDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Servicios de Negocio: Registramos el CRUD de Pacientes y el de Auth
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<AuthService>();

// Seguridad JWT: Le enseñamos a la API a validar el "pasaporte" (Token)
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

builder.Services.AddControllers();
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
        Description = "Pegá tu token acá abajo (sin la palabra Bearer, solo el choclo de letras)"
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

// --- 2. CONFIGURACIÓN DEL PIPELINE (Orden de ejecución) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// EL ORDEN ACÁ ABAJO ES VIDA O MUERTE:
app.UseRouting(); // Organiza las rutas

app.UseCors("AllowReactApp"); // 1° ¿Viene de un origen permitido?

app.UseAuthentication(); // 2° ¿Quién sos? (Valida el Token)
app.UseAuthorization();  // 3° ¿Tenés permiso? (Valida el rol/acceso)

app.MapControllers();

app.Run();