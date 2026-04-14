using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using S14_ProjetSession;
using S14_ProjetSession.Areas.Identity.Data;
using S14_ProjetSession.Authorization;
using S14_ProjetSession.Data;
using S14_ProjetSession.Resources;
using System;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddDefaultIdentity<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ResidencesDbContext>()
    .AddErrorDescriber<FRIdentityErrorDescriber>();

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

WebApplication app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{

    //gestion erreurs serveur 
    app.UseExceptionHandler("/Home/Erreur");
    // gestion erreurs HTTP
    app.UseStatusCodePagesWithReExecute("/Home/Erreur", "?statusCode={0}");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    using (IServiceScope scope = app.Services.CreateScope())
    {
        // Obtenir DbContext
        IServiceProvider services = scope.ServiceProvider;
        ResidencesDbContext context = services.GetRequiredService<ResidencesDbContext>();
        // Initialiser les donn�es

        // Obtenir UserMangaer
        UserManager<ApplicationUser> userManager =
    services.GetRequiredService<UserManager<ApplicationUser>>();

        RoleManager<IdentityRole> roleManager =
            services.GetRequiredService<RoleManager<IdentityRole>>();


        await DbInitialisation.Initialiser(context, userManager, roleManager);
    }
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
