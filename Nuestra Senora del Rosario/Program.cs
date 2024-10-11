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

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
