using DotNetEnv;
using Composition;
using MinimalAPI.Extensions;
using MinimalAPI.Presentation.Constraints;
using MinimalAPI.Presentation.Routes;

var environment = Environment.GetEnvironmentVariable("ServerOptions__Environment");
if (environment != "Production")
{
    var solutionRoot = Directory.GetCurrentDirectory();
    var envPath = Path.Combine(solutionRoot, "..", "..", "..", ".env");
    Env.Load(envPath);
}

var builder = WebApplication.CreateBuilder();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddOpenApi();

// Agregar dependencias del proyecto de Extensions/
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApiSpecificServices(builder.Configuration);

builder.Services.Configure<RouteOptions>(options =>
{
    options.ConstraintMap.Add("ulong", typeof(UlongRouteConstraint));
});

var app = builder.Build();

// Admitir solicitudes provenientes del frontend
app.UseCors("AllowFrontend");

// Activar rutas protegidas y políticas establecidas
app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

app.UseAntiforgery();

// Configurar el pipeline de solicitudes HTTP.
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

// Endpoints de la API
app.MapUserRoutes();
app.MapAuthRoutes();
app.MapClassRoutes();
app.MapContactRoutes();
app.MapDatabaseRoutes();
app.MapNotificationRoutes();
app.MapResourceRoutes();
app.MapTestRoutes();
app.MapAnswerRoutes();

app.MapGet("/quartz-status", async (Quartz.ISchedulerFactory schedulerFactory) =>
{
    try
    {
        var scheduler = await schedulerFactory.GetScheduler();
        var metadata = await scheduler.GetMetaData();
        return Results.Ok(new
        {
            SchedulerName = metadata.SchedulerName,
            SchedulerInstanceId = metadata.SchedulerInstanceId,
            SchedulerType = metadata.SchedulerType.FullName,
            IsRunning = metadata.Started,
            InStandbyMode = metadata.InStandbyMode,
            IsShutdown = metadata.Shutdown,
            JobsExecuted = metadata.NumberOfJobsExecuted,
        });
    }
    catch (Exception e)
    {
        return Results.Problem(e.ToString());
    }
});

app.Run();
