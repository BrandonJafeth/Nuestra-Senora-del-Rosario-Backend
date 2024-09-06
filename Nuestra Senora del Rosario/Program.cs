using Microsoft.EntityFrameworkCore;
using Services.Informative.ApplicationFormService;
using Services.Informative.DonationType;
using Services.Informative.FormDonationService;
using Services.Informative.FormVoluntarieServices;
using Services.Informative.GalleryItemServices;
using Services.Informative.GenericRepository;
using Services.Informative.MethodDonationService;
using Services.Informative.NavbarItemServices;
using Services.MyDbContext;

var builder = WebApplication.CreateBuilder(args);

// Configurar AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Otros servicios, como DbContext y servicios genéricos
builder.Services.AddDbContext<MyContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddScoped(typeof(ISvGenericRepository<>), typeof(SvGenericRepository<>));
builder.Services.AddScoped<ISvNavbarItemService, SvNavbarItem>();
builder.Services.AddScoped<ISvGalleryItem, SvGalleryItem>();
builder.Services.AddScoped<ISvMethodDonation, SvMethodDonation>();
builder.Services.AddScoped<ISvDonationType, SvDonationType>();
builder.Services.AddScoped<ISvFormDonation, SvFormDonationService>();
builder.Services.AddScoped<ISvFormVoluntarieService, SvFormVoluntarieService>();
builder.Services.AddScoped<ISvApplicationForm, SvApplicationForm>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});


builder.Services.AddControllers()
    .AddNewtonsoftJson();

// Configuración de Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
