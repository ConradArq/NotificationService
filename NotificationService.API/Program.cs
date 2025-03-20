using NotificationService.Application;
using NotificationService.Infrastructure;
using NotificationService.API.Middlewares;
using NotificationService.Infrastructure.Services.Queues;
using NotificationService.Infrastructure.Logging;
using NotificationService.Infrastructure.Services.BackgroundServices;
using NotificationService.Infrastructure.Interfaces.Logging;
using NotificationService.Infrastructure.SignalR.Hubs;
using Microsoft.OpenApi.Models;
using NotificationService.Infrastructure.Quartz;
using System.Security.Claims;
using System.Globalization;
using Microsoft.Extensions.Options;
using NotificationService.Infrastructure.Interfaces.Providers;
using NotificationService.Domain.Interfaces.Repositories;
using NotificationService.Infrastructure.Interfaces.Services;
using Microsoft.Data.SqlClient;
using NotificationService.Domain.Models.Entities.External;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Services.AddAPIServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddCors(options =>
{
    // Browsers block AllowAnyOrigin() when credentials like Authorization headers are sent, so origins in the CORS policy are used instead.
    options.AddPolicy("ClientApp", builder =>
    {

#if DEBUG
        builder.WithOrigins("http://localhost:5170")
               .AllowCredentials()
               .AllowAnyHeader()
               .AllowAnyMethod();
#else
        builder.WithOrigins("https://client-app-url.com")
               .AllowCredentials()
               .AllowAnyHeader()
               .AllowAnyMethod();
#endif

    });

});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {

        Title = "NotificationService",
        Version = "v1",
        Description = "Service for managing and sending push notifications to client devices and email notifications to designated email accounts.",
        Contact = new OpenApiContact()
        {
            Name = "Development by: conra.arq@gmail.com"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Please insert the JWT token in this format: Bearer {your token here}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    // Uncomment to include XML documentation comments from the project's generated .xml file in Swagger.
    //var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Configure HostOptions to handle unhandled exceptions in BackgroundService
builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

#region Logging

builder.Logging.ClearProviders();

builder.Logging.AddConsole();

builder.Services.AddSingleton<ILogQueueService, LogQueueService>();

builder.Services.AddSingleton<ILoggerProvider, ApiLoggerProvider>();

builder.Services.AddSingleton<IApiLogger>(serviceProvider =>
{
    var logQueueService = serviceProvider.GetRequiredService<ILogQueueService>();
    return new ApiLogger("DefaultCategory", logQueueService, serviceProvider.GetService<IHttpContextAccessor>());
});

builder.Services.AddHostedService<LogBackgroundService>();

builder.Services.Configure<LoggerFilterOptions>(options =>
{
    // Prevent logs produced by the LogBackgroundService from being written to the ApiLoggerProvider.
    // This avoids potential feedback loops where the logger writes logs about its own logging process.
    options.Rules.Add(new LoggerFilterRule(
        providerName: typeof(ApiLoggerProvider).FullName,
        categoryName: typeof(LogBackgroundService).FullName,
        logLevel: LogLevel.None,
        filter: null
    ));

    // Allow application-level logs at Information level or higher in the custom provider. 
    // This applies to logs categorized under "NotificationService.*".
    options.Rules.Add(new LoggerFilterRule(
        providerName: typeof(ApiLoggerProvider).FullName,
        categoryName: $"{typeof(ApiLoggerProvider).Namespace?.Split('.')[0] ?? "DefaultNamespace"}.*",
        logLevel: LogLevel.Information,
        filter: null
        ));

    // Restrict logs from .NET or other libraries to Error level or higher in the custom provider.
    // This applies to all categories and ensures that only critical errors from third-party libraries or .NET internals are logged.
    // It is mutually exclusive from the rule allowing application-level logs at Information level or higher, 
    // ensuring that logs from the application and external libraries are handled separately.
    options.Rules.Add(new LoggerFilterRule(
        providerName: typeof(ApiLoggerProvider).FullName,
        categoryName: "*",
        logLevel: LogLevel.Error,
        filter: null));
});

#endregion

#endregion

var app = builder.Build();

#region Localization

// Set the default culture globally for the application's threads. This ensures that, unless overridden, all culture-dependent
// operations (e.g., number and date formatting) and resource lookups default to the specified language.
var cultureInfo = new CultureInfo("en");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo; // Sets the default culture for data operations (e.g., DateTime, decimal).
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo; // Sets the default culture for resource file lookups (e.g., .resx files).

var localizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;

// This enables dynamic culture determination for each request based on the following priority order:
// 1. QueryStringRequestCultureProvider: If a query string like ?culture=es&ui-culture=es is present, it takes the highest precedence.
// 2. CookieRequestCultureProvider: If a culture is stored in a cookie, it will be used next.
// 3. AcceptLanguageHeaderRequestCultureProvider: The browser's Accept-Language header determines the culture if no query string or cookie is present.
// 4. DefaultRequestCulture: If none of the above providers set a culture, the default culture (configured in RequestLocalizationOptions) is used.
// NOTE: Thread-level defaults (CultureInfo.DefaultThreadCurrentCulture and CultureInfo.DefaultThreadCurrentUICulture) are overridden per request
// if a culture is determined by any of the RequestLocalization providers.
app.UseRequestLocalization(localizationOptions);

#endregion

#region Cors

// Apply CORS policy only for browser-based requests to ensure proper restrictions and avoid returning CORS headers for backend-to-backend calls.
app.UseWhen(
    context => context.Request.Headers.ContainsKey("Origin"),
    appBuilder => appBuilder.UseCors("ClientApp")
);

#endregion

#region Global Exception Handling

// Fallback handler for unobserved exceptions in Tasks outside the HTTP request pipeline, such as those in background services or
// fire-and-forget operations. This handler is invoked for exceptions that are not awaited or handled explicitly in Task-based operations.
// Common scenarios include unhandled exceptions in EmailBackgroundService or LogBackgroundService.
// Note: This does not prevent the exceptions from being thrown but ensures they are logged and do not crash the process.
TaskScheduler.UnobservedTaskException += (sender, e) =>
{
    e.SetObserved(); // Marks the exception as observed to prevent the process from being terminated.
    using var scope = app.Services.CreateScope();
    var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger<Program>();
    logger.LogCritical(e.Exception, "Unobserved task exception caught globally.");
};

// Fallback handler for unhandled exceptions in non-Task-based operations outside the HTTP request pipeline.
// This includes exceptions in long-running background threads (e.g., Thread, ThreadPool), unmanaged code, or application-level failures.
// Note: This is a last-resort handler for unexpected, unhandled exceptions. While the application may attempt to log these exceptions,
// the process might still terminate depending on the severity of the error.
AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
{
    var exception = e.ExceptionObject as Exception;
    if (exception != null)
    {
        using var scope = app.Services.CreateScope();
        var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogCritical(exception, "Unhandled exception caught globally in the AppDomain.");
    }
};

#endregion

#region Quartz Scheduler

// Starts the job scheduler, resolved directly from app.Services. Creating a scope is unnecessary, as all its dependencies are singletons.
var quartzScheduler = app.Services.GetRequiredService<QuartzScheduler>();
await quartzScheduler.StartAsync();
await quartzScheduler.InitJobs();

#endregion

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Middleware to inject a JWT token into requests for testing endpoints requiring authorization (for development only)
    app.Use(async (context, next) =>
    {
        using var scope = app.Services.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var jwtTokenProvider = app.Services.GetRequiredService<IJwtTokenProvider>();

        AspNetUser? testAdmin = null;

        try
        {
            var admins = await unitOfWork.ExternalRepository.GetAspNetUsersByAsync(roleName: "Admin");
            testAdmin = admins.FirstOrDefault();
        }
        catch(SqlException ex)
        {
            if(ex.Message.Contains("Invalid object name"))
            {
                // Identity schema does not exist in DB
            }
            else
            {
                throw;
            }
        }

        var defaultUserName = "testAdmin";
        var defaultUserId = "1234567890";
        var defaultEmail = "testAdmin@test.com";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, testAdmin?.UserName ?? defaultUserName),
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim("Permission", "CanSendNotification"),
            new Claim("Permission", "CanGenerateReports"),
            new Claim("Department", "IT"),
            new Claim(ClaimTypes.NameIdentifier, testAdmin?.Id ?? defaultUserId),
            new Claim(ClaimTypes.Email, testAdmin?.Email ?? defaultEmail)
        };

        var token = jwtTokenProvider.GenerateAuthenticationToken(TimeSpan.FromMinutes(1), claims.ToArray());
        context.Request.Headers["Authorization"] = $"Bearer {token}";

        await next.Invoke();
    });
}

app.UseRouting();

app.UseMiddleware<AuthErrorMiddleware>();

app.UseAuthentication();

app.UseMiddleware<UserContextMiddleware>();

app.UseAuthorization();

app.MapHub<NotificationHub>(app.Configuration["SignalR:NotificationHubRoute"] ?? "/notificationHub");

app.MapControllers();

app.Run();
