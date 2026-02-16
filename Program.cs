using BlazorApp1;
using BlazorApp1.Components;
using Microsoft.EntityFrameworkCore;
using BlazorApp1.Data;


var builder = WebApplication.CreateBuilder(args);



// --- CONFIGURACIÓN DE BASE DE DATOS 
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
// -----------------------------------------------------


builder.Services.AddSingleton<LedService>();


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.Services.GetService<LedService>();

// Asegurar que el servicio se inicie y la DB exista
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    // Esto fuerza la creación del LedService dentro del mismo scope
    scope.ServiceProvider.GetRequiredService<LedService>();
}




// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
  
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAntiforgery();

app.MapRazorComponents<App>()

    .AddInteractiveServerRenderMode();

app.Run();