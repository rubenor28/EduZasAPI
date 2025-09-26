using EduZasAPI.Infraestructure.MinimalAPI.Presentation.Common;
using EduZasAPI.Infraestructure.MinimalAPI.Presentation.Users;
using EduZasAPI.Infraestructure.MinimalAPI.Presentation.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapUserRoutes();
app.MapAuthRoutes();


app.Run();
