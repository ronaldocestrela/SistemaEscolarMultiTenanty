using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddMInfrastructureServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Services.GetJwtSettings(builder.Configuration));

var app = builder.Build();

// Database seeder
await app.Services.AddDatabaseInitializerAsync(CancellationToken.None);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseInfrastructure();

app.MapControllers();

app.Run();
