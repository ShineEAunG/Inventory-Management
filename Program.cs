using InventoryManagementSystem.Data;
using InventoryManagementSystem.Data.SeedData;
using InventoryManagementSystem.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.DbConfiguration(builder.Configuration);
builder.Services.CorsConfiguration();
builder.Services.ServiceCollectionConfiguration();
builder.Services.JwtSchemeConfiguration(builder.Configuration);
builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        
        // Ensure database is created and migrated
        context.Database.Migrate();
        
        // Seed data if no items exist
        if (!context.Items.Any())
        {
            Console.WriteLine("Seeding database...");
            InventorySystem.SeedData(context);
            AuthenticationSystem.SeedData(context);
            Console.WriteLine("Database seeded successfully!");
        }
        else
        {
            Console.WriteLine("Database already has data, skipping seed.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("AllowAny");

app.UseAuthorization();

app.MapControllers();

app.Run();
