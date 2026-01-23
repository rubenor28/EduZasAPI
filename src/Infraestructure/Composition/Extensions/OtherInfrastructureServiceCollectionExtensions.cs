using Application.Configuration;
using Application.Services;
using Application.Services.Graders;
using Bcrypt.Application.Services;
using MailKitProj;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Composition.Extensions;

internal static class OtherInfrastructureServiceCollectionExtensions
{
    internal static IServiceCollection AddEmailSender(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        return services;
    }

    internal static IServiceCollection AddOtherInfrastructureServices(
        this IServiceCollection services,
        IConfiguration cfg
    )
    {
        services.AddScoped<IHashService, BCryptHasher>();
        services.AddSingleton<IRandomStringGeneratorService, RandomStringGeneratorService>(
            sp => new RandomStringGeneratorService(
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray(),
                15
            )
        );

        var gradeSettings = cfg.GetSection("GradeSettings").Get<GradeSettings>();

        ArgumentNullException.ThrowIfNull(
            gradeSettings,
            "GradeSettings must be defined on appsettings.json"
        );

        services.AddSingleton(gradeSettings);

        services.AddScoped<AnswerGrader>();

        return services;
    }
}
