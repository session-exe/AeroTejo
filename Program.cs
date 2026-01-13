using AeroTejo.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao container
builder.Services.AddControllersWithViews();

// Configurar o DbContext com MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost;Database=aerotejo;User=root;Password=root;";

builder.Services.AddDbContext<AeroTejoContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configurar sessões para autenticação
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Adicionar suporte para HttpContext
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configurar o pipeline de requisições HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Ativar sessões
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Rota para o painel administrativo
app.MapControllerRoute(
    name: "admin",
    pattern: "AeroTejoControl/{action=Dashboard}/{id?}",
    defaults: new { controller = "Admin" });

// Seed da base de dados
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AeroTejoContext>();
        context.Database.EnsureCreated();
        DbSeeder.Seed(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro ao popular a base de dados.");
    }
}

app.Run();
