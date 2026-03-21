


using Microsoft.EntityFrameworkCore;
using S14_ProjetSession.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ResidencesDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration["ConnectionStrings:ConnectionBD"]);
});
builder.Services.AddScoped<IEtudiantRepository, DbEtudiantRepository>();
builder.Services.AddScoped<IDemandeRepository, DbDemandeRepository>();
builder.Services.AddScoped<ISemestreRepository, DbSemestreRepository>();
builder.Services.AddScoped<IGenreRepository, DbGenreRepository>();

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

        // Initialiser les données
        DbInitialisation.Initialiser(context);
    }
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
