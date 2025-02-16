using Entities.Administration;
using Entities.Informative;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PdfSharp.Charting;
using Services.Administrative.EmailServices;
using Services.Administrative.EmployeeRoleServices;
using Services.Administrative.Employees;
using Services.Administrative.FormVoluntarieService;
using Services.Administrative.PasswordResetServices;
using Services.Administrative.PaymentReceiptService;
using Services.Administrative.PdfReceiver;
using Services.Administrative.PdfReceiverService;
using Services.Administrative.Residents;
using Services.Administrative.Users;
using Services.GenericService;
using Services.Informative.ApplicationFormService;
using Services.Informative.DonationType;
using Services.Informative.FormDonationService;
using Services.Informative.FormVoluntarieServices;
using Services.Informative.GalleryItemServices;
using Services.Informative.MethodDonationService;
using Services.Informative.NavbarItemServices;
using Services.MyDbContext;
using System.Text;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Services.ConverterService;
using Services.Administrative.AppointmentService;
using Nuestra_Senora_del_Rosario.Hubs;
using Services.Administrative.Notifications;
using Services.Administrative.NotificationServices;
using Services.Administrative.Guardians;
using FluentValidation.AspNetCore;
using Services.Validations.Admistrative;
using Services.Administrative.Product;
using Services.Administrative.Inventory;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Configurar AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Otros servicios, como DbContext y servicios genéricos
builder.Services.AddDbContext<MyInformativeContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddDbContext<AdministrativeContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));



builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";
        return new ValueTask(context.HttpContext.Response.WriteAsync("{\"error\": \"Has excedido el número máximo de solicitudes permitidas. Por favor, intenta nuevamente más tarde.\"}", cancellationToken: token));
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


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"], // Del appsettings.json
            ValidAudience = builder.Configuration["Jwt:Audience"], // Del appsettings.json
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddSignalR(); // Agregar SignalR

builder.Services.AddHostedService<NotificationHostedService>();

// Registrar FluentValidation en Program.cs
var assemblies = AppDomain.CurrentDomain.GetAssemblies();
foreach (var assembly in assemblies)
{
    builder.Services.AddControllers()
        .AddFluentValidation(fv =>
        {
            fv.RegisterValidatorsFromAssembly(assembly);
            fv.DisableDataAnnotationsValidation = true;
        });
}






// Registros genéricos para entidades en MyInformativeContext
builder.Services.AddScoped<ISvGenericRepository<FormVoluntarie>, SvGenericRepository<FormVoluntarie, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<NavbarItem>, SvGenericRepository<NavbarItem, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<GalleryItem>, SvGenericRepository<GalleryItem, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<MethodDonation>, SvGenericRepository<MethodDonation, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<DonationType>, SvGenericRepository<DonationType, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<FormDonation>, SvGenericRepository<FormDonation, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<ApplicationForm>, SvGenericRepository<ApplicationForm, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<Status>, SvGenericRepository<Status, MyInformativeContext>>();

// Registros genéricos para entidades en AdministrativeContext
builder.Services.AddScoped<ISvGenericRepository<User>, SvGenericRepository<User, AdministrativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<Employee>, SvGenericRepository<Employee, AdministrativeContext>>();
builder.Services.AddScoped<ISvEmployee, SvEmployee>();
builder.Services.AddScoped<ISvEmployeeRole, SvEmployeeRole>();
builder.Services.AddScoped<ISvGenericRepository<EmployeeRole>, SvGenericRepository<EmployeeRole, AdministrativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<TypeOfSalary>, SvGenericRepository<TypeOfSalary, AdministrativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<Profession>, SvGenericRepository<Profession, AdministrativeContext>>();

builder.Services.AddScoped<ISvGenericRepository<PasswordResetToken>, SvGenericRepository<PasswordResetToken, AdministrativeContext>>();
builder.Services.AddScoped<ISvPasswordResetService, SvPasswordResetService>();
builder.Services.AddScoped<ISvGenericRepository<Rol>, SvGenericRepository<Rol, AdministrativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<Room>, SvGenericRepository<Room, AdministrativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<DependencyLevel>, SvGenericRepository<DependencyLevel, AdministrativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<DependencyHistory>, SvGenericRepository<DependencyHistory, AdministrativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<ResidentApplication>, SvGenericRepository<ResidentApplication, AdministrativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<Resident>, SvGenericRepository<Resident, AdministrativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<Guardian>, SvGenericRepository<Guardian, AdministrativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<Applicant>, SvGenericRepository<Applicant, AdministrativeContext>>();
builder.Services.AddScoped<ISvPdfReceiverService, SvPdfReceiverService>();
builder.Services.AddScoped<ISvGenericRepository<Notification>, SvGenericRepository<Notification, AdministrativeContext>>();
builder.Services.AddScoped<ISvNotification, SvNotification>(); // Registro del servicio de notificaciones
builder.Services.AddScoped<ISvGenericRepository<UnitOfMeasure>, SvGenericRepository<UnitOfMeasure, AdministrativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<Category>, SvGenericRepository<Category, AdministrativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<Product>, SvGenericRepository<Product, AdministrativeContext>>();
builder.Services.AddScoped<ISvProductService, SvProductService>();
builder.Services.AddScoped<ISvGenericRepository<Inventory>, SvGenericRepository<Inventory, AdministrativeContext>>();
builder.Services.AddScoped<ISvInventoryService, SvInventoryService>();
// Registros genéricos y servicios adicionales para citas y sus entidades relacionadas

builder.Services.AddScoped<ISvGenericRepository<Appointment>, SvGenericRepository<Appointment, AdministrativeContext>>();
builder.Services.AddScoped<ISvAppointment, SvAppointment>();  // Servicio de Citas
builder.Services.AddScoped<ISvGuardian, SvGuardian>();

builder.Services.AddScoped<ISvGenericRepository<Specialty>, SvGenericRepository<Specialty, AdministrativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<HealthcareCenter>, SvGenericRepository<HealthcareCenter, AdministrativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<AppointmentStatus>, SvGenericRepository<AppointmentStatus, AdministrativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<Note>, SvGenericRepository<Note, AdministrativeContext>>();

builder.Services.AddScoped<ISvPaymentReceipt, SvPaymentReceipt>();
builder.Services.AddScoped<ISvResident, SvResident>();





// Registros genéricos para entidades en MyInformativeContext
builder.Services.AddScoped<ISvGenericRepository<AdministrativeRequirements>, SvGenericRepository<AdministrativeRequirements, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<AssociatesSection>, SvGenericRepository<AssociatesSection, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<ButtonInfo>, SvGenericRepository<ButtonInfo, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<Contact>, SvGenericRepository<Contact, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<DonationsSection>, SvGenericRepository<DonationsSection, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<GalleryCategory>, SvGenericRepository<GalleryCategory, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<HeroSection>, SvGenericRepository<HeroSection, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<ImportantInformation>, SvGenericRepository<ImportantInformation, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<NursingRequirements>, SvGenericRepository<NursingRequirements, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<RegistrationSection>, SvGenericRepository<RegistrationSection, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<ServiceSection>, SvGenericRepository<ServiceSection, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<SiteSettings>, SvGenericRepository<SiteSettings, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<TitleSection>, SvGenericRepository<TitleSection, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<VoluntarieType>, SvGenericRepository<VoluntarieType, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<VolunteeringSection>, SvGenericRepository<VolunteeringSection, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<VolunteerProfile>, SvGenericRepository<VolunteerProfile, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<AboutUsSection>, SvGenericRepository<AboutUsSection, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<ApplicationStatus>, SvGenericRepository<ApplicationStatus, MyInformativeContext>>();


// Registros de servicios específicos
builder.Services.AddScoped<ISvNavbarItemService, SvNavbarItem>();
builder.Services.AddScoped<ISvGalleryItem, SvGalleryItem>();
builder.Services.AddScoped<ISvMethodDonation, SvMethodDonation>();
builder.Services.AddScoped<ISvDonationType, SvDonationType>();
builder.Services.AddScoped<ISvFormDonation, SvFormDonationService>();
builder.Services.AddScoped<ISvApplicationForm, SvApplicationForm>();
builder.Services.AddScoped<ISvFormVoluntarieService, SvFormVoluntarieService>();
builder.Services.AddScoped<IAdministrativeFormVoluntarieService, AdministrativeFormVoluntarieService>();
builder.Services.AddScoped<ISvEmailService, SvEmailService>();
builder.Services.AddAutoMapper(typeof(AdministrativeMappingProfile));
builder.Services.AddScoped<ISvUser, SvUser>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v1" });

    // Configuración para manejar IFormFile
    c.OperationFilter<FileUploadOperationFilter>();
});

// Registrar el servicio de caché en memoria
builder.Services.AddMemoryCache(); // Aquí se añade el servicio de caché

builder.Services.AddAuthorization();


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
// Habilitar HTTPS
app.UseHttpsRedirection();

// Habilitar enrutamiento
app.UseRouting();

// Habilitar CORS
app.UseCors("AllowAll");

// Habilitar autenticación
app.UseAuthentication();

// Agregar Rate Limiter
app.UseRateLimiter();

// Habilitar autorización
app.UseAuthorization();

// Mapeo de controladores y SignalR
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("/notificationHub");
});

// Configurar el puerto y ejecutar la aplicación
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();