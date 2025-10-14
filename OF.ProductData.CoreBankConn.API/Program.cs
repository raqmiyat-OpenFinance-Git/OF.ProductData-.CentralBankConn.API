using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog;
using OF.ProductData.Common.AES;
using OF.ProductData.Common.Custom;
using OF.ProductData.Common.NLog;
using OF.ProductData.CoreBankConn.API.EFModel;
using OF.ProductData.CoreBankConn.API.IServices;
using OF.ProductData.CoreBankConn.API.Repositories;
using OF.ProductData.CoreBankConn.API.Services;
using OF.ProductData.Model.Common;
using System.Data;
using System.Net;
using System.Net.Security;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace OF.ProductData.CoreBankConn.API;

public static class Program
{
    private static readonly Logger _logger = LogManager.GetLogger("OF.ProductData.CoreBankConn.API.Logger");

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var AllowSpecificOrigins = "OF.ProductData.CoreBankConn.API";
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: AllowSpecificOrigins, policy =>
            {
                policy.AllowAnyOrigin().AllowAnyHeader();
            });
        });

        ConfigureApplicationSettings(builder.Services, builder.Configuration);
        RegisterDbConnection(builder.Services);
        RegisterTransientServices(builder.Services);
        RegisterSingletonServices(builder.Services);
        ConfigureCbsDbContext(builder.Services);
        RegisterCustomerEnquiryHttpClient(builder.Services);
        ConfigureCertificateValidation(builder.Services);
        ConfigureAuthentication(builder.Services, builder.Configuration);

        builder.Services.AddControllers();
        builder.Services.AddHealthChecks();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSwaggerGen(c =>

        {

            c.CustomSchemaIds(type => type.FullName);

        });
        var app = builder.Build();
       
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {

            c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoreBankBankPRoductAPI V1.0");

        });
        app.UseCors(AllowSpecificOrigins);
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.Run();
    }
    private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        using var provider = services.BuildServiceProvider();
        var jwtSettings = provider.GetRequiredService<IOptions<JwtSettings>>().Value;
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
                    ValidAudience = jwtSettings!.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key!)),
                    ClockSkew = TimeSpan.Zero
                };
            });
    }
    private static void ConfigureApplicationSettings(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SocketsHttpHandlerSettings>(configuration.GetSection(nameof(SocketsHttpHandlerSettings)));
        services.Configure<SecurityParameters>(configuration.GetSection(nameof(SecurityParameters)));
        services.Configure<DatabaseConfig>(configuration.GetSection(nameof(DatabaseConfig)));
        services.Configure<ServiceParams>(configuration.GetSection(nameof(ServiceParams)));
        services.Configure<CoreBankApis>(configuration.GetSection(nameof(CoreBankApis)));
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
      
    }

    private static void ConfigureCbsDbContext(IServiceCollection services)
    {
        services.AddDbContext<CbsDbContext>(static (provider, options) =>
        {
            var config = provider.GetRequiredService<IOptions<DatabaseConfig>>().Value;

            var connectionString = SqlConManager.GetConnectionString(
                config.ConnectionString!,
                config.UseEncryption,
                config.IsEntityFramework,
                _logger
            );

            options.UseSqlServer(connectionString);
        });
    }

    private static void RegisterDbConnection(IServiceCollection services)
    {
        services.AddScoped<IDbConnection>(provider =>
        {
            var config = provider.GetRequiredService<IOptions<DatabaseConfig>>().Value;

            var connectionString = SqlConManager.GetConnectionString(
                config.ConnectionString!,
                config.UseEncryption,
                config.IsEntityFramework,
                _logger
            );

            var dbConnection = new SqlConnection(connectionString);
            dbConnection.Open();

            _logger.Info(ForStructuredLog("Program", "RegisterDbConnection", "DBConnection opened"));
            return dbConnection;
        });
    }

    private static void RegisterCustomerEnquiryHttpClient(IServiceCollection services)
    {
        services.AddHttpClient<IProductDataService, ProductDataService>((provider, client) =>
        {
            var handlerSettings = provider.GetRequiredService<IOptions<SocketsHttpHandlerSettings>>().Value;
            var coreBankApis = provider.GetRequiredService<IOptions<CoreBankApis>>().Value;

            if (string.IsNullOrWhiteSpace(coreBankApis.BaseUrl))
                throw new InvalidOperationException("CoreBankApis.BaseUrl must be configured.");

            client.DefaultRequestHeaders.Connection.Add("keep-alive");
            client.DefaultRequestHeaders.ConnectionClose = false;
            client.BaseAddress = new Uri(coreBankApis.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(handlerSettings.ClientTimeoutInSeconds);
        })

        .SetHandlerLifetime(TimeSpan.FromHours(2))
        .ConfigurePrimaryHttpMessageHandler(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<SocketsHttpHandlerSettings>>().Value;

            return new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromHours(settings.PooledConnectionLifetimeInHours),
                PooledConnectionIdleTimeout = TimeSpan.FromMinutes(settings.PooledConnectionIdleTimeoutInMinutes),
                ConnectTimeout = TimeSpan.FromSeconds(settings.ClientTimeoutInSeconds),
                KeepAlivePingDelay = TimeSpan.FromMinutes(settings.KeepAlivePingDelayInMinutes),
                KeepAlivePingTimeout = TimeSpan.FromSeconds(settings.KeepAlivePingTimeoutInSeconds),
                UseCookies = settings.UseCookies,
                SslOptions = new SslClientAuthenticationOptions
                {
                    EnabledSslProtocols = SslClient.ParseSslProtocols(settings.EnabledSslProtocols ?? "Tls12"),
                    CertificateRevocationCheckMode = SslClient.ParseRevocationMode(settings.CertificateRevocationCheckMode ?? "NoCheck"),
                    RemoteCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
                        sslPolicyErrors == SslPolicyErrors.None || settings.RemoteCertificateValidationCallback
                }
            };
        });
    }

    private static void ConfigureCertificateValidation(IServiceCollection services)
    {
        using var provider = services.BuildServiceProvider();
        var settings = provider.GetRequiredService<IOptions<SocketsHttpHandlerSettings>>().Value;

        ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
        {
            return sslPolicyErrors == SslPolicyErrors.None || settings.RemoteCertificateValidationCallback;
        };
    }

    private static void RegisterTransientServices(IServiceCollection services)
    {
       
        services.AddTransient<IProductDataService, ProductDataService>();
        services.AddTransient<ICoreBankRepository, CoreBankRepository>();
       
    }

    private static void RegisterSingletonServices(IServiceCollection services)
    {
       
        services.AddSingleton<ProductLogger>();
        services.AddSingleton<WarmUpLogger>();
        services.AddSingleton<BaseLogger>();
        services.AddSingleton<ApiClientLogger>();
        services.AddSingleton<AesCbcGenericService>();
    }

    private static string ForStructuredLog(string controllerName, string methodName, string message)
    {
        return
$@"---------------------------------------------------------------
 Class Name   : {controllerName}
 Method Name  : {methodName}
 Message      : {message}
---------------------------------------------------------------";
    }
}
