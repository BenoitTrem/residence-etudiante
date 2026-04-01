using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Areas.Identity.Data;
using S14_ProjetSession.Data;
using S14_ProjetSession.Authorization;
using System;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsEnvironment("Test"))
{
    string connectionString = builder.Configuration.GetConnectionString("ApplicationConnectionBD") ?? throw new InvalidOperationException("Connection string 'ConnectionBD' not found.");
}

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<ResidencesDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ApplicationConnectionBD"));
});
builder.Services.AddDefaultIdentity<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ResidencesDbContext>();
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

    options.AddPolicy("UtilisateurSeulement", policy =>
        policy.RequireRole("Utilisateur"));

    options.AddPolicy("EstEtudiant", policy =>
       policy.RequireAuthenticatedUser()
             .AddRequirements(new EtudiantRequirement()));
});

builder.Services.AddScoped<IAuthorizationHandler, EtudiantHandler>();

WebApplication app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
