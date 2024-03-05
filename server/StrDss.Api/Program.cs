using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using NetCore.AutoRegisterDi;
using StrDss.Api.Authentication;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Mappings;
using StrDss.Model;
using StrDss.Service;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.AddServerHeader = false;
});

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddControllers();

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
    .Where(a => a.FullName.StartsWith("AdvSol"))
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

//RegexDefs as Singleton
builder.Services.AddSingleton<RegexDefs>();

builder.Services.AddHttpClient();

var mappingConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new EntityToModelProfile());
    cfg.AddProfile(new ModelToEntityProfile());
    cfg.AddProfile(new ModelToModelProfile());
});

var mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);


//Add logging
builder.Services.AddLogging(builder => builder.AddConsole());

builder.Services.AddScoped<KcJwtBearerEvents>();

//var strDssAuthScheme = "str_dss";

//Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration.GetValue<string>("JWT:Authority");
        options.Audience = builder.Configuration.GetValue<string>("JWT:Audience");
        options.IncludeErrorDetails = true;
        options.EventsType = typeof(KcJwtBearerEvents);
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false,
            ValidateAudience = true,
            ValidAlgorithms = new List<string>() { "RS256" },
        };
    })
;

builder.Services.AddAuthorization(options =>
{
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
        JwtBearerDefaults.AuthenticationScheme);
    defaultAuthorizationPolicyBuilder =
        defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
