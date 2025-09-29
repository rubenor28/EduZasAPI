using EduZasAPI.Infraestructure.MinimalAPI.Presentation.Common;
using EduZasAPI.Infraestructure.MinimalAPI.Presentation.Users;
using EduZasAPI.Infraestructure.MinimalAPI.Presentation.Auth;

using DotNetEnv;

var environment = Environment.GetEnvironmentVariable("ServerOptions__Environment");
if (environment != "Production")
{
    var solutionRoot = Directory.GetCurrentDirectory();
    var envPath = Path.Combine(solutionRoot, "..", "..", "..", ".env");
    Env.Load(envPath);
}

var builder = WebApplication.CreateBuilder();
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Agregar dependencias del proyecto de Extensions/
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();
app.UseCors("AllowFrontend");

// Activar rutas protegidas y políticas establecidas
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EduZasAPI V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// Endpoints API
app.MapUserRoutes();
app.MapAuthRoutes();


app.Run();
