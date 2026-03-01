using Microsoft.EntityFrameworkCore;
using Portehobe.Model;
using PorteHobe.API.Data;
using PorteHobe.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer("Server=AURIN\\SQLEXPRESS;Database=PorteHobe;Trusted_Connection=True;TrustServerCertificate=True"));

builder.Services.AddScoped<IStudyResourceService, ResourceService>();

var app = builder.Build();

// Seed the database with sample study resources
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    SeedData.Initialize(context);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapControllers();

app.Run();