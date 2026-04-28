using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlueDragon.Excursion.API.Authentication;
using BlueDragon.Excursion.Core.Interfaces;
using BlueDragon.Excursion.Infrastructure.Domain.Settings;
using BlueDragon.Excursion.Infrastructure.Handlers.Implementations;
using BlueDragon.Excursion.Infrastructure.Handlers.Interfaces;
using BlueDragon.Excursion.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace BlueDragon.Excursion.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    #region ServicesConfiguration

    public void ConfigureServices(IServiceCollection services)
    {
        #region Mvc

        services.AddCors();
        services.AddControllers().AddJsonOptions(ConfigureJsonOptions);
        services.AddOptions();

        #endregion

        #region Settings

        services.AddSingleton(Configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>());

        JwtSettings jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>();
        services.AddSingleton(jwtSettings);

        services.AddSingleton(Configuration.GetSection("StripeSettings").Get<StripeSettings>());

        #endregion

        #region Authentication

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
            })
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiKey", _ => { });

        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme,
                    "ApiKey")
                .RequireAuthenticatedUser()
                .Build();
        });

        #endregion

        #region Services

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<ITradeService, TradeService>();
        services.AddScoped<IJournalService, JournalService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();

        #endregion

        #region Handlers

        services.AddSingleton<IAuthHandler, AuthHandler>();
        services.AddSingleton<ITradeHandler, TradeHandler>();
        services.AddSingleton<IJournalHandler, JournalHandler>();
        services.AddSingleton<ISubscriptionHandler, SubscriptionHandler>();

        #endregion

        #region Swagger

        services.AddSwaggerGen(ConfigureSwaggerGen);

        #endregion
    }

    private void ConfigureJsonOptions(JsonOptions options)
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; 
    }

    private void ConfigureSwaggerGen(SwaggerGenOptions swaggerGenOptions)
    {
        swaggerGenOptions.SwaggerDoc("v1.0", new OpenApiInfo
        {
            Title = "BlueDragon.Excursion.API",
            Version = "v1.0",
            Description = "Postman collection: Not available"
        });
        swaggerGenOptions.CustomSchemaIds(x => x.FullName);

        string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        swaggerGenOptions.IncludeXmlComments(xmlPath);

        swaggerGenOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Unesite JWT token (bez 'Bearer ' prefiksa)"
        });
    }

    #endregion

    #region ApplicationConfiguration

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSerilogRequestLogging();
        app.UseRouting();
        app.UseCors(ConfigureCors);
        app.UseAuthentication();
        app.UseAuthorization();
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(ConfigureSwaggerUI);
        }
        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        app.UseEndpoints(ConfigureEndpoints);
    }

    private void ConfigureCors(CorsPolicyBuilder builder)
    {
        builder.SetIsOriginAllowed(origin => true)
            .AllowCredentials()
            .AllowAnyMethod()
            .AllowAnyHeader();
    }

    private void ConfigureSwaggerUI(SwaggerUIOptions swaggerUiOptions)
    {
        swaggerUiOptions.SwaggerEndpoint("../swagger/v1.0/swagger.json", "BlueDragon.Excursion.API documentation");
        swaggerUiOptions.DefaultModelExpandDepth(2);
        swaggerUiOptions.DefaultModelRendering(ModelRendering.Model);
        swaggerUiOptions.DefaultModelsExpandDepth(-1);
        swaggerUiOptions.DisplayRequestDuration();
        swaggerUiOptions.DocExpansion(DocExpansion.None);
        swaggerUiOptions.EnableDeepLinking();
        swaggerUiOptions.EnableFilter();
        swaggerUiOptions.MaxDisplayedTags(50);
        swaggerUiOptions.ShowExtensions();
        swaggerUiOptions.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Head, SubmitMethod.Post, SubmitMethod.Put, SubmitMethod.Delete, SubmitMethod.Patch);
    }

    private void ConfigureEndpoints(IEndpointRouteBuilder builder)
    {
        builder.MapControllers();
    }

    #endregion
}
