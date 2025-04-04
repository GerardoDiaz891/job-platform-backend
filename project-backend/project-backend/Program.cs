using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using project_backend.Data;
using project_backend.Services;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using project_backend.Middleware;

// Punto de entrada para configurar los servicios de ASP.NET Core
var builder = WebApplication.CreateBuilder(args);

// Rate Limiting - Para limitar petición y evitar ataques de fuerza bruta
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    // POLÍTICA GLOBAL PARA TODOS LOS ENDPOINTS
    rateLimiterOptions.AddFixedWindowLimiter("GlobalPolicy", options =>
    {
        options.PermitLimit = 100;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5; // Peticiones en cola
    });

    // Políticas estrictas para Login y Register
    rateLimiterOptions.AddFixedWindowLimiter("AuthPolicy", options =>
    {
        options.PermitLimit = 5;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueLimit = 0;
    });

    rateLimiterOptions.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync(
            "Demasiadas solicitudes. Por favor intente de nuevo más tarde.", token);
    };
});

// Entity Framework Core - Configuraciones de la BD - SQL proveedor
// Contexto de la BD sea injectado en Controladores y servicios
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Autenticacion JWT - Esquema de autenticacion Bearer de JWT
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
            // Clave secreta para verificar firma
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

//CORS para conectar
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173") 
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrador"));
    options.AddPolicy("EmpresarialOnly", policy => policy.RequireRole("Empresarial"));
    options.AddPolicy("PostulanteOnly", policy => policy.RequireRole("Postulante"));
});
// Configuraciones del SWAGGER
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Job-PlatformBackend", Version = "v1" });

    // Configuración para JWT en SWAGGER
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization", // nombre de Header
        Description = "Enter JWT Bearer token",
        In = ParameterLocation.Header, // Token en el header
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        { securityScheme, new[] { "Bearer" } }
    };

    c.AddSecurityRequirement(securityRequirement);
});
builder.Services.AddScoped<TokenService>(); // Instancia de servicio para solicitudes HTTP

var app = builder.Build();

// Middleware pipeline para flujo de solicitudes
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//Habilitar CORS
app.UseCors("AllowOrigins");

// Solicitudes HTTP a HTTPS 
app.UseHttpsRedirection();

// Habilitar middleware para los CVs
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseTokenBlacklist(); // Agregar antes de los endpoint

app.MapControllers().RequireRateLimiting("GlobalPolicy");

app.Run();