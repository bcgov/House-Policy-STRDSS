using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using NetCore.AutoRegisterDi;
using StrDss.Api.Authentication;
using StrDss.Data;
using StrDss.Data.Mappings;
using StrDss.Model;
using StrDss.Service;
using StrDss.Service.HttpClients;
using System.Reflection;
using StrDss.Data.Entities;
using Microsoft.EntityFrameworkCore;
using StrDss.Api.Middlewares;
using StrDss.Api;
using StrDss.Service.Bceid;
using Npgsql;
using Serilog;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using StrDss.Common;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.AddServerHeader = false;
    options.Limits.MaxResponseBufferSize = 100 * 1024 * 1024;
});

var dbHost = builder.Configuration.GetValue<string>("DB_HOST");
var dbName = builder.Configuration.GetValue<string>("DB_NAME");
var dbUser = builder.Configuration.GetValue<string>("DB_USER");
var dbPass = builder.Configuration.GetValue<string>("DB_PASS");
var dbPort = builder.Configuration.GetValue<string>("DB_PORT");

var connString = $"Host={dbHost};Username={dbUser};Password={dbPass};Database={dbName};Port={dbPort};";

builder.Services.AddHttpContextAccessor();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<DssDbContext>(opt =>
{
    opt.UseNpgsql(connString, opt => opt.UseNetTopologySuite());

    if (builder.Environment.IsDevelopment())
    {
        opt.EnableDetailedErrors();
        opt.EnableSensitiveDataLogging();
    }
});

//https://github.com/npgsql/efcore.pg/issues/2736#issuecomment-1875391344
NpgsqlConnection.GlobalTypeMapper.UseNetTopologySuite();

builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = new HeaderApiVersionReader("version");
    options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
});

var assemblies = Assembly.GetExecutingAssembly()
    .GetReferencedAssemblies()
    .Where(a => a.FullName.StartsWith("StrDss"))
    .Select(Assembly.Load).ToArray();

//Services
builder.Services.RegisterAssemblyPublicNonGenericClasses(assemblies)
     .Where(c => c.Name.EndsWith("Service"))
     .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

//Repository
builder.Services.RegisterAssemblyPublicNonGenericClasses(assemblies)
     .Where(c => c.Name.EndsWith("Repository"))
     .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

//Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Headers
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

//FieldValidationService as Singleton
builder.Services.AddSingleton<IFieldValidatorService, FieldValidatorService>();

builder.Services.AddHttpClients(builder.Configuration);
builder.Services.AddBceidSoapClient(builder.Configuration);

builder.Services.AddMemoryCache();

var mappingConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new EntityToModelProfile());
    cfg.AddProfile(new EntityToEntityProfile());
    cfg.AddProfile(new ModelToEntityProfile());
    cfg.AddProfile(new ModelToModelProfile());
});

var mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddScoped<KcJwtBearerEvents>();
builder.Services.AddScoped<ApsJwtBearerEvents>();

var apsAuthScheme = "aps";

//Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration.GetValue<string>("SSO_AUTHORITY");
        options.Audience = builder.Configuration.GetValue<string>("SSO_CLIENT");
        options.IncludeErrorDetails = true;
        options.EventsType = typeof(KcJwtBearerEvents);
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateAudience = true,
            ValidAlgorithms = new List<string>() { "RS256" },
        };
    })
    .AddJwtBearer(apsAuthScheme, options =>
    {
        options.Authority = builder.Configuration.GetValue<string>("APS_AUTHORITY");
        options.IncludeErrorDetails = true;
        options.EventsType = typeof(ApsJwtBearerEvents);
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidAlgorithms = new List<string>() { "RS256" },
        };
    })
;

builder.Services.AddAuthorization(options =>
{
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
        JwtBearerDefaults.AuthenticationScheme, apsAuthScheme);
    defaultAuthorizationPolicyBuilder =
        defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations();
    options.UseAllOfToExtendReferenceSchemas();
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var healthCheck = new HealthCheck(connString);
builder.Services.AddHealthChecks().AddCheck("DbConnection", healthCheck);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(option =>
    {
        option.RouteTemplate = "api/swagger/{documentName}/swagger.json";
    });

    app.UseSwaggerUI(option =>
    {
        option.SwaggerEndpoint($"/api/swagger/{ApiTags.Default}/swagger.json", ApiTags.Default);
        option.SwaggerEndpoint($"/api/swagger/{ApiTags.Aps}/swagger.json", ApiTags.Aps);

        option.RoutePrefix = "api/swagger";
    });
}

app.UseHealthChecks("/healthz");

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("content-security-policy", $"default-src 'self'; style-src 'self' 'img-src 'self' data:; frame-ancestors 'self'; object-src 'none'; base-uri 'self'; form-action 'self';");
    context.Response.Headers.Add("strict-transport-security", "max-age=15768000; includeSubDomains; preload");
    context.Response.Headers.Add("x-content-type-options", "nosniff");
    context.Response.Headers.Add("x-frame-options", "SAMEORIGIN");
    context.Response.Headers.Add("x-xss-protection", "0");
    context.Response.Headers.Add("permissions-policy", "geolocation=(),midi=(),sync-xhr=(),microphone=(),camera=(),magnetometer=(),gyroscope=(),fullscreen=(self),payment=()");
    context.Response.Headers.Add("referrer-policy", "strict-origin");
    context.Response.Headers.Add("x-dns-prefetch-control", "off");
    context.Response.Headers.Add("cache-control", "no-cache, no-store, must-revalidate");
    context.Response.Headers.Add("pragma", "no-cache");
    context.Response.Headers.Add("expires", "0");
    await next.Invoke();
});

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
