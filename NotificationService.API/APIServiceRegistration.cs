using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;
using NotificationService.API.Filters;
using NotificationService.API.ModelBinders.Providers;
using NotificationService.Application.Exceptions;
using NotificationService.Infrastructure.Configuration;
using System.Globalization;
using System.Text;

namespace NotificationService.Application
{
    public static class APIServiceRegistration
    {
        public static IServiceCollection AddAPIServices(this IServiceCollection services, IConfiguration configuration)
        {
            #region Localization

            // Uncomment this line to enable localization using a folder other than the default "Resources" folder for .resx files.
            // This will configure the application to load localized strings from .resx files located in the specified folder (e.g., "FolderName").
            // Example: Place your resource files in "FolderName/ValidationMessages.en.resx" or "FolderName/ValidationMessages.es.resx".
            //services.AddLocalization(options => options.ResourcesPath = "FolderName");

            // Uncomment these two lines to enable localization from the database.
            // This setup overrides the default behavior of using the "Resources" folder by replacing the default IStringLocalizerFactory.
            // All calls to IStringLocalizer or IStringLocalizer<T> will fetch strings from the database instead of .resx files.
            //services.AddScoped<ILocalizationService, LocalizationService>();
            //services.AddSingleton<IStringLocalizerFactory, DatabaseStringLocalizerFactory>();

            // To use both .resx files and database localization:
            // 1. Uncomment "services.AddLocalization" to enable .resx files.
            // 2. Keep the DatabaseStringLocalizerFactory commented out to avoid overriding the default factory.
            // 3. Use ILocalizationService explicitly to fetch strings from the database, while .resx strings remain accessible via IStringLocalizer.
            // Example: Use IStringLocalizer for resources and ILocalizationService for database localization.

            // Configures localization options for the application
            services.Configure<RequestLocalizationOptions>(options =>
            {
                // Sets the default culture to "en" (English) when no culture is explicitly specified in the request
                options.DefaultRequestCulture = new RequestCulture("en");

                // Specifies the cultures supported for formatting (e.g., numbers, dates, currency)
                options.SupportedCultures = new[] { new CultureInfo("en"), new CultureInfo("es") };

                // Specifies the UI cultures supported for resource file localization (e.g., ValidationMessages.en.resx or ValidationMessages.es.resx)
                options.SupportedUICultures = new[] { new CultureInfo("en"), new CultureInfo("es") };

                // Adds culture providers to determine the request's culture dynamically.
                // QueryStringRequestCultureProvider checks the query string for culture parameters (e.g., ?culture=es&ui-culture=es).
                // Inserted at position 0 to give it the highest priority.
                options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());

                // Additional providers (enabled by default):
                // - CookieRequestCultureProvider: Reads culture preferences from cookies.
                // - AcceptLanguageHeaderRequestCultureProvider: Reads culture preferences from the Accept-Language header sent by the browser.
            });

            #endregion

            services.AddHttpContextAccessor();

            services.AddControllers(options =>
            {
                // The ModelBinderProviders are called in the order in which they are registered. For each provider, the framework calls
                // the GetBinder method to determine if the provider can supply a binder for the current model type and binding source.
                // Once a provider returns a binder, that binder is used to bind the model and the pipeline stops processing other
                // providers. If the binder cannot bind the model correctly, an error is raised, and the pipeline bypasses the rest.
                options.Filters.Add<CustomActionFilter>();
                options.ModelBinderProviders.Insert(0, new RouteParameterBinderProvider());
                options.ModelBinderProviders.Insert(1, new QueryStringBinderProvider());
                options.ModelBinderProviders.Insert(2, new JsonBinderProvider());
                // TODO: Implement FormDataBinderProvider to provide localized error messages for form data binding errors.
                // NOTE:
                // - The FormDataBinderProvider is responsible for handling models with BindingSource.Form.
                // - In general, BindingSource.Form applies to the entire form object.
                // - After the form object is bound, each property of the form model is passed through the binding pipeline.
                // - To localize or customize error messages for each individual property, custom binders for the corresponding
                //   property types (e.g., string, int, custom objects) would need to be implemented.
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                // Uncomment to disable automatic 400 Bad Request responses for invalid models. If SuppressModelStateInvalidFilter is
                // set to true, InvalidModelStateResponseFactory will not handle the response, so manual checking of ModelState.IsValid 
                // would be required if no other validation mechanism is in place.
                // options.SuppressModelStateInvalidFilter = true;

                options.InvalidModelStateResponseFactory = context =>
                {
                    // This runs when validation errors occur. Remove errors from ModelState with key "requestDto:". The System.Text.Json
                    // serializer adds these messages when it fails to bind JSON to the model parameter (in this project it is always
                    // called "requestDto"). These errors are often redundant or unclear, so they are excluded for cleaner responses.
                    var validationErrors = context.ModelState
                        .Where(x => x.Value != null && x.Key != "requestDto" && x.Value.Errors.Count > 0)
                        .ToDictionary(x => x.Key, x => x.Value!.Errors.Select(e => e.ErrorMessage).ToArray())
                        .Where(x => x.Value.Length > 0)
                        .ToDictionary(x => x.Key, x => x.Value);

                    throw new ValidationException(validationErrors);
                };
            });

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen();

            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()
                   ?? throw new InvalidOperationException("JwtSettings section is missing from configuration.");

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            // ASP.NET Core's authentication system automatically maps all claims from IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames
            // to Security.Claims.ClaimTypes (e.g. JwtRegisteredClaimNames.Sub to ClaimTypes.NameIdentifier).
            // When retrieving claims from HttpContext.User, use ClaimTypes instead of JwtRegisteredClaimNames to ensure compatibility.
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // Explicitly setting NameClaimType to JwtRegisteredClaimNames.Sub to support OAuth providers
                    // (Google, Azure AD, etc.), as many external providers use "sub" as the unique user identifier.        
                    NameClaimType = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,

                    // Audience and issuer validation
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidIssuer = jwtSettings.Issuer,

                    // Ensure tokens are signed correctly
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),

                    // Token expiration validation
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Ensure immediate expiration validation


                    // Uncomment to allow tokens without an exp claim while validating those with exp claim (for testing purposes, not production)
                    //LifetimeValidator = (notBefore, expires, token, parameters) =>
                    //{
                    //    if (!expires.HasValue)
                    //    {
                    //        return true;
                    //    }
                    //    return DateTime.UtcNow < expires.Value;
                    //}
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("SendNotificationPolicy", policy =>
                    policy.RequireClaim("Permission", "CanSendNotification"));

                options.AddPolicy("GenerateNotificationsReportPolicy", policy =>
                    policy.RequireRole("Admin", "Manager")
                          .RequireClaim("Permission", "CanGenerateReports")
                          .RequireClaim("Department", "IT"));

                //options.AddPolicy("GenerateNotificationsReportPolicy", policy =>
                //    policy.RequireAssertion(context =>
                //        context.User.HasClaim(c => c.Type == "Role" && (c.Value == "Admin" || c.Value == "Manager")) &&
                //        context.User.HasClaim(c => c.Type == "Permission" && c.Value == "CanGenerateReports") &&
                //        context.User.HasClaim(c => c.Type == "Department" && c.Value == "IT")));

                options.AddPolicy("EntityOwnershipPolicy", policy =>
                {
                    policy.Requirements.Add(new EntityOwnershipRequirement());
                    // Optionally add a specific entity type and ID parameter for customization.
                    // Use this if the entity name does not match the controller name or the ID parameter is not "id".
                    //policy.Requirements.Add(new EntityOwnershipRequirement(entityType: typeof(SmtpConfig), idParameterName: "customId"));
                });
            });

            services.AddSingleton<IAuthorizationHandler, EntityOwnershipHandler>();

            return services;
        }
    }
}
