using Entities.Informative;
using Microsoft.EntityFrameworkCore;
using Services.Administrative.EmailServices;
using Services.Administrative.FormVoluntarieService;
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

// Registros genéricos para entidades en MyInformativeContext
builder.Services.AddScoped<ISvGenericRepository<FormVoluntarie>, SvGenericRepository<FormVoluntarie, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<NavbarItem>, SvGenericRepository<NavbarItem, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<GalleryItem>, SvGenericRepository<GalleryItem, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<MethodDonation>, SvGenericRepository<MethodDonation, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<DonationType>, SvGenericRepository<DonationType, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<FormDonation>, SvGenericRepository<FormDonation, MyInformativeContext>>();
builder.Services.AddScoped<ISvGenericRepository<ApplicationForm>, SvGenericRepository<ApplicationForm, MyInformativeContext>>();

// Registros genéricos para entidades en AdministrativeContext
builder.Services.AddScoped<ISvGenericRepository<User>, SvGenericRepository<User, AdministrativeContext>>();


builder.Services.AddScoped<ISvGenericRepository<Employee>, SvGenericRepository<Employee, AdministrativeContext>>();

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
