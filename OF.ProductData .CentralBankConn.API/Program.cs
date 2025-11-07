using ConsentModel.Common;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi.Models;
using OF.ProductData.CentralBankConn.API.IServices;
using OF.ProductData.CentralBankConn.API.Middleware;
using OF.ProductData.CentralBankConn.API.Models;
using OF.ProductData.CentralBankConn.API.Repositories;
using OF.ProductData.CentralBankConn.API.Services;
using OF.ProductData.CentralBankConn.API.Validators;
using OF.ProductData.Common.Custom;
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.Common;
using Raqmiyat.Framework.Custom;

namespace OF.ProductData.CentralBankConn.API;

public static class Program
{
    private static readonly Logger _logger = LogManager.GetLogger("OF.ProductData.CentralBankConn.API.Logger");

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        const string AllowSpecificOrigins = "OF.ProductData.CentralBankConn.API";

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: AllowSpecificOrigins, policy =>
            {
                policy.AllowAnyOrigin().AllowAnyHeader();
            });
        });

        var configuration = builder.Configuration;
        var rabbitMqSettings = configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();

        if (rabbitMqSettings == null)
            throw new InvalidOperationException("RabbitMqSettings section is missing in configuration.");

        ConfigureApplicationSettings(builder.Services, configuration);
        RegisterDbConnection(builder.Services);
        RegisterTransientServices(builder.Services);
        RegisterSingletonServices(builder.Services);
        RegisterProductEnquiryHttpClient(builder.Services);
        RegisterCreateLeadEnquiryHttpClient(builder.Services);
      
        // RegisterConfirmationPayeeEnquiryHttpClient(builder.Services);
        RegisterCustomHttpClient(builder.Services);
        ConfigureCertificateValidation(builder.Services);
        AddMassTransitWithRabbitMq(builder.Services, rabbitMqSettings);
        builder.Services.AddMemoryCache();
        builder.Services.AddTransient<Custom>();
        builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });

        var redisSettings = builder.Configuration
           .GetSection("RedisCacheSettings")
           .Get<RedisCacheSettings>();

        // Register Redis cache if enabled
        if (redisSettings!.EnableCache)
        {
            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisSettings.Url;
                options.InstanceName = "OF.ProductData.CentralBankConn.API_";
            });
        }
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHealthChecks();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CentralBank Product API",
                Version = "v1",
                Description = "API for Central Bank product data communication."
            });
            c.CustomSchemaIds(type => type.FullName);
            c.DescribeAllParametersInCamelCase();
        });

        builder.Services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssemblyContaining<ProductDataRequestValidator>();

        builder.Services.AddFluentValidationAutoValidation()
              .AddValidatorsFromAssemblyContaining<CreateLeadRequestValidator>();

        var app = builder.Build();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("v1/swagger.json", "CentralBankProductAPI V1.0");
        });

        // Register custom middleware here (preferably early in the pipeline)
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseCors(AllowSpecificOrigins);
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

       
        try
        {
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            _logger.Error("Unhandled exception in Main(): " + ex);
            throw;
        }
    }

    private static void ConfigureApplicationSettings(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SocketsHttpHandlerSettings>(configuration.GetSection(nameof(SocketsHttpHandlerSettings)));
        services.Configure<IdentityServerSettings>(configuration.GetSection(nameof(IdentityServerSettings)));
        services.Configure<RedisCacheSettings>(configuration.GetSection(nameof(RedisCacheSettings)));
        services.Configure<RabbitMqSettings>(configuration.GetSection(nameof(RabbitMqSettings)));
        services.Configure<DatabaseConfig>(configuration.GetSection(nameof(DatabaseConfig)));
        services.Configure<ServiceParams>(configuration.GetSection(nameof(ServiceParams)));
        services.Configure<CoreBankApis>(configuration.GetSection(nameof(CoreBankApis)));
        services.Configure<ApiHeaderParams>(configuration.GetSection(nameof(ApiHeaderParams)));
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

    private static void RegisterProductEnquiryHttpClient(IServiceCollection services)
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


    private static void RegisterCreateLeadEnquiryHttpClient(IServiceCollection services)
    {
        services.AddHttpClient<ICreateLeadService, CreateLeadService>((provider, client) =>
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

    private static void RegisterCustomHttpClient(IServiceCollection services)
    {
        services.AddHttpClient<Custom>((provider, client) =>
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
                    {
                        if (settings.RemoteCertificateValidationCallback)
                            return true;
                        return sslPolicyErrors == SslPolicyErrors.None; // strict validation
                    }

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
        services.AddTransient<IMasterRepository, MasterRepository>();
        
        services.AddTransient<IProductDataService, ProductDataService>();
        services.AddTransient<ICreateLeadService, CreateLeadService>();
  
        services.AddTransient<Logger>(sp =>
        {
            return LogManager.GetLogger("OF.ProductData.CentralBankConn.API.Logger");
        });

    }

    private static void RegisterSingletonServices(IServiceCollection services)
    {
     
        services.AddSingleton<QueueLogger>();
        services.AddSingleton<WarmUpLogger>();
        services.AddSingleton<BaseLogger>();
        services.AddSingleton<ApiClientLogger>();
        services.AddSingleton<ProductLogger>();
        services.AddSingleton<CreateLeadLogger>();
        services.AddSingleton<SendPointInitialize>();

        services.AddHostedService<SendPointInitializerHostedService>();
        services.AddSingleton(resolver =>
      resolver.GetRequiredService<IOptions<ApiHeaderParams>>().Value);
    }

    public static IServiceCollection AddMassTransitWithRabbitMq(IServiceCollection services, RabbitMqSettings rabbitMqSettings)
    {
        var password = rabbitMqSettings.IsEncrypted
            ? EncryptDecrypt.Decrypt(rabbitMqSettings.Rabitphrase!, _logger)
            : rabbitMqSettings.Rabitphrase!;

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(new Uri(rabbitMqSettings.Url!), h =>
                {
                    h.Username(rabbitMqSettings.UserName!);
                    h.Password(password);
                });

                // cfg.ConfigureEndpoints(context); // If you have consumers
            });
        });

        return services;
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

