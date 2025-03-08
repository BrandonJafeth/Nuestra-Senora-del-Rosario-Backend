using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using FluentValidation.AspNetCore;
using System.Threading.RateLimiting;
using Infrastructure.Services.Administrative.Notifications.Hubs;
using Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

#region RateLimiter
builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        return new ValueTask(
            context.HttpContext.Response.WriteAsync(
                "{\"error\": \"Has excedido el número máximo de solicitudes permitidas.\"}",
                cancellationToken: token
            )
        );
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
            })
    );
});
#endregion

#region JWT_Auth
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
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader());
});
#endregion

#region MemoryCache_and_SignalR

builder.Services.AddMemoryCache();


builder.Services.AddSignalR();
#endregion

#region Infrastructure_DI

builder.Services.AddInfrastructure(builder.Configuration);
#endregion

#region Controllers_and_FluentValidation

var assemblies = AppDomain.CurrentDomain.GetAssemblies();
builder.Services.AddControllers()
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblies(assemblies);
        fv.DisableDataAnnotationsValidation = true;
    })
    .AddNewtonsoftJson();
#endregion

#region Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v1" });

    // 🔹 Definir el esquema de autenticación
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el token JWT en este formato: Bearer {token}"
    });

    // 🔹 Aplicar el esquema de autenticación a todas las solicitudes
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
#endregion


#region Build_App
var app = builder.Build();
#endregion

#region Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Primero CORS
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Rate limiting
app.UseRateLimiter();

// Routing al final
app.UseRouting();

app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

////Cambios para coolify
//var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
//app.Urls.Clear();
//app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();
#endregion
