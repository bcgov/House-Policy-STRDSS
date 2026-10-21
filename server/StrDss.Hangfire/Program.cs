using Asp.Versioning;
using AutoMapper;
using Hangfire;
using NetCore.AutoRegisterDi;
using StrDss.Data;
using StrDss.Data.Mappings;
using StrDss.Model;
using StrDss.Service;
using StrDss.Service.HttpClients;
using System.Reflection;
using StrDss.Data.Entities;
using StrDss.Data.Telemetry;
using Microsoft.EntityFrameworkCore;
using StrDss.Service.Bceid;
using StrDss.Service.Hangfire;
using Hangfire.PostgreSql;
using Npgsql;
using OpenTelemetry.Metrics;
using StrDss.Hangfire;
using Serilog;
using Hangfire.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.AddServerHeader = false;
});

var dbHost = builder.Configuration.GetValue<string>("DB_HOST");
var dbName = builder.Configuration.GetValue<string>("DB_NAME");
var dbUser = builder.Configuration.GetValue<string>("DB_USER");
var dbPass = builder.Configuration.GetValue<string>("DB_PASS");
var dbPort = builder.Configuration.GetValue<string>("DB_PORT");

var connString = $"Host={dbHost};Username={dbUser};Password={dbPass};Database={dbName};Port={dbPort};CommandTimeout=2400";

builder.Services.AddHttpContextAccessor();

Console.WriteLine("Hangfire");

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

builder.Services.DecorateRepositoriesWithMetrics();

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddMeter(RepositoryMetrics.MeterName)
        // Built-in .NET runtime metrics (GC, JIT, thread pool, exceptions, etc.)
        .AddRuntimeInstrumentation()
        // Built-in ASP.NET Core server metrics (request duration, active requests, etc.)
        .AddAspNetCoreInstrumentation()
        // Built-in HttpClient metrics (outbound request duration, active requests, etc.)
        .AddHttpClientInstrumentation()
        // Built-in EF Core and Npgsql meters
        .AddMeter("Microsoft.EntityFrameworkCore")
        .AddMeter("Npgsql")
        .AddPrometheusExporter());

//Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Headers
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

//FieldValidationService as Singleton
builder.Services.AddSingleton<IFieldValidatorService, FieldValidatorService>();

builder.Services.AddHttpClients(builder.Configuration);
builder.Services.AddBceidSoapClient(builder.Configuration);

var mappingConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new EntityToModelProfile());
    cfg.AddProfile(new EntityToEntityProfile());
    cfg.AddProfile(new ModelToEntityProfile());
    cfg.AddProfile(new ModelToModelProfile());
});

var mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddMemoryCache();

builder.Services
    .AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage((option) =>
        {
            option.UseNpgsqlConnection(connString);
        }
));

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 5;
});

//Add logging
builder.Services.AddLogging(builder => builder.AddConsole());

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var healthCheck = new HealthCheck(connString);
builder.Services.AddHealthChecks().AddCheck("DbConnection", healthCheck);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/healthz");

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.Use(async (context, next) =>
{
    context.Response.Headers.ContentSecurityPolicy = "default-src 'self'; style-src 'self' 'img-src 'self' data:; frame-ancestors 'self'; object-src 'none'; base-uri 'self'; form-action 'self';";
    context.Response.Headers.StrictTransportSecurity = "max-age=15768000; includeSubDomains; preload";
    context.Response.Headers.XContentTypeOptions = "nosniff";
    context.Response.Headers.XFrameOptions = "SAMEORIGIN";
    context.Response.Headers.XXSSProtection = "0";
    context.Response.Headers["permissions-policy"] = "geolocation=(),midi=(),sync-xhr=(),microphone=(),camera=(),magnetometer=(),gyroscope=(),fullscreen=(self),payment=()";
    context.Response.Headers["referrer-policy"] = "strict-origin";
    context.Response.Headers["x-dns-prefetch-control"] = "off";
    context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";
    context.Response.Headers.Pragma = "no-cache";
    context.Response.Headers.Expires = "0";
    await next.Invoke();
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseHangfireDashboard();

// make sure this is after app.UseHangfireDashboard()

// Retrieve all recurring jobs
var recurringJobs = JobStorage.Current.GetConnection().GetRecurringJobs();

// Remove each recurring job
foreach (var job in recurringJobs)
{
    RecurringJob.RemoveIfExists(job.Id);
}

// process uploads
RecurringJob.AddOrUpdate<HangfireJobs>("Process Uploaded Data", job => job.ProcessUpload(), "*/5 * * * *");

// process nightly job of sending takedown request emails to plaforms
RecurringJob.AddOrUpdate<HangfireJobs>("Process Takedown Request Batch Emails", job => job.ProcessTakedownRequestBatchEmails(), "50 6 * * *");

// daily export
RecurringJob.AddOrUpdate<HangfireJobs>("Create Rental Listing Export Files", job => job.CreateRentalListingExportFiles(), "50 5 * * *");

app.Run();
