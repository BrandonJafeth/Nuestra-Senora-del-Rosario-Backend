using Microsoft.EntityFrameworkCore;
using Services.Informative.GenericRepository;
using Services.MyDbContext;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de servicios
builder.Services.AddDbContext<MyContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddScoped(typeof(ISvGenericRepository<>), typeof(SvGenericRepository<>));


builder.Services.AddControllers();

// Configuraci�n de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuraci�n del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
