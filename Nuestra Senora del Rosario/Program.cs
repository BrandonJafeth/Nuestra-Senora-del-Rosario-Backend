using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;
using System.Threading.RateLimiting;
using Infrastructure.Services.Administrative.Notifications.Hubs;
using Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// =============================
// 1) Configuraciones globales
// =============================

// Rate Limiter
builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        return new ValueTask(context.HttpContext.Response.WriteAsync(
            "{\"error\": \"Has excedido el número máximo de solicitudes permitidas.\"}",
            cancellationToken: token
        ));
    };

    options.AddPolicy("LimiteDeSolicitudes", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonimo",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(3),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});

// Autenticación con JWT
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader());
});

// ================================
// 2) Llamada a tu AddInfrastructure
// ================================
builder.Services.AddInfrastructure(builder.Configuration);

// ================================
// 3) Configuración de Controllers + FluentValidation
// ================================
var assemblies = AppDomain.CurrentDomain.GetAssemblies();
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblies(assemblies);
        fv.DisableDataAnnotationsValidation = true;
    })
    .AddNewtonsoftJson(); // si estás usando Newtonsoft

// ================================
// 4) Configuración de Swagger
// ================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v1" });
    // c.OperationFilter<FileUploadOperationFilter>(); // si usas IFormFile
});

// ================================
// 5) Construir y configurar la app
// ================================
var app = builder.Build();

// Swagger en Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middlewares
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

// Mapeo de controladores (y Hubs si usas SignalR)
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

//// Puerto y arranque
//var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
//app.Urls.Add($"http://0.0.0.0:{port}");
app.Run();