using TrainerNetwork.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure(
    builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is missing."));

var app = builder.Build();

if (app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
{
    await app.Services.ApplyMigrationsAsync();
}

if (app.Environment.IsDevelopment() && app.Configuration.GetValue<bool>("Database:SeedDemoData"))
{
    await app.Services.SeedDemoDataAsync();
}

app.MapControllers();

await app.RunAsync();
