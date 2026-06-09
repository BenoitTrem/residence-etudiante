using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ResidenceEtudiante;
using ResidenceEtudiante.Areas.Identity.Data;
using ResidenceEtudiante.Authorization;
using ResidenceEtudiante.Data;
using ResidenceEtudiante.Resources;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;

if (!builder.Environment.IsEnvironment("Test"))
{
    string connectionString = builder.Configuration.GetConnectionString("ApplicationConnectionBD") ?? throw new InvalidOperationException("Connection string 'ConnectionBD' not found.");
}

// Définition des langues supportées
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    string[] supportedCultures = new[] { "fr-CA" };
    options
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures)
        .SetDefaultCulture(supportedCultures[0]);
});
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services
    .AddControllersWithViews()
    .AddMvcLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResources));
    });

builder.Services
    .AddRazorPages()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
             factory.Create(typeof(SharedResources));

    });

builder.Services.AddDbContext<ResidencesDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ApplicationConnectionBD"));
});

// email config 
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ResidencesDbContext>()
.AddErrorDescriber<FRIdentityErrorDescriber>();
// email config
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<AzureEmailSettings>(
    builder.Configuration.GetSection("AzureEmailSettings"));

if (builder.Environment.IsEnvironment("Test") || builder.Environment.IsDevelopment())
{
    builder.Services.AddTransient<IEmailSender, EmailSender>();
}
else
{
    builder.Services.AddTransient<IEmailSender, AzureEmailSender>();
}

builder.Services.AddScoped<IResidenceRepository, DbResidenceRepository>();
builder.Services.AddScoped<IUniteRepository, DbUniteRepository>();
builder.Services.AddScoped<IEtudiantRepository, DbEtudiantRepository>();
builder.Services.AddScoped<IDemandeRepository, DbDemandeRepository>();
builder.Services.AddScoped<ISemestreRepository, DbSemestreRepository>();
builder.Services.AddScoped<IProgrammesRepository, DbProgrammesRepository>();
builder.Services.AddScoped<IGenresRepository, DbGenresRepository>();
builder.Services.AddScoped<ICampusRepository, DbCampusRepository>();
builder.Services.AddScoped<ICommoditeRepository, DbCommoditeRepository>();




builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminUniquement", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("AdminOuGestionnaire", policy =>
        policy.RequireRole("Admin", "Gestionnaire"));

    options.AddPolicy("GestionnaireUniquement", policy =>
        policy.RequireRole("Gestionnaire"));

    options.AddPolicy("AdminOuUtilisateur", policy =>
      policy.RequireRole("Admin", "Utilisateur"));

    options.AddPolicy("UtilisateurSeulement", policy =>
        policy.RequireRole("Utilisateur"));

    options.AddPolicy("EstEtudiant", policy =>
        policy.RequireAuthenticatedUser()
              .AddRequirements(new EtudiantRequirement()));

    options.AddPolicy("EstProprietaireDemande", policy =>
        policy.RequireAuthenticatedUser()
              .AddRequirements(new EstProprietaireDemandeRequirement()));
});

builder.Services.AddScoped<IAuthorizationHandler, ProprietaireDemandeHandler>();
builder.Services.AddScoped<IAuthorizationHandler, EtudiantHandler>();

// 403 meme apres connection
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Home/Erreur?statusCode=403";
});

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Erreur");
    app.UseHsts();
}


if (!app.Environment.IsEnvironment("Test"))
{
    using IServiceScope scope = app.Services.CreateScope();
    IServiceProvider services = scope.ServiceProvider;

    ResidencesDbContext context = services.GetRequiredService<ResidencesDbContext>();
    UserManager<ApplicationUser> userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    RoleManager<IdentityRole> roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await DbInitialisation.Initialiser(context, userManager, roleManager);
}


app.UseHttpsRedirection();

app.UseRouting();


app.UseStatusCodePagesWithReExecute("/Home/Erreur", "?statusCode={0}");

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
