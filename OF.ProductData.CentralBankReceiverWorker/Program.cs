
using Microsoft.AspNetCore.Hosting;
using OF.ProductData.CoreBankConn.API.EFModel;
using OF.ProductData.CentralBankReceiverWorker.Consumer;
using OF.ProductData.CentralBankReceiverWorker.IServices;
using OF.ProductData.CentralBankReceiverWorker.Services;
using OF.ProductData.Common.Custom;
using OF.ProductData.Common.NLog;
using OF.ProductData.Model.Common;


namespace OF.ProductData.CentralBankConn.API;

public static class Program
{
    private static readonly Logger _logger = LogManager.GetLogger("OF.ProductData.CentralBankReceiverWorker.Logger");

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        const string AllowSpecificOrigins = "OF.ProductData.CentralBankReceiverWorker";
        builder.Host.UseWindowsService();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: AllowSpecificOrigins, policy =>
            {
                policy.AllowAnyOrigin().AllowAnyHeader();
            });
        });

        var configuration = builder.Configuration;
        var rabbitMqSettings = configuration.GetSection(nameof(RabbitMqSettings)).Get<RabbitMqSettings>();
        var serviceParams = configuration.GetSection(nameof(ServiceParams)).Get<ServiceParams>();

        if (rabbitMqSettings == null)
            throw new InvalidOperationException("RabbitMqSettings section is missing in configuration.");

        ConfigureApplicationSettings(builder.Services, configuration);
        ConfigureAuditLogDbContext(builder.Services);
        ConfigureCustomerDbContext(builder.Services);
        RegisterDbConnection(builder.Services);
        RegisterTransientServices(builder.Services);
        RegisterSingletonServices(builder.Services);
        AddMassTransitWithRabbitMq(builder.Services, rabbitMqSettings);
        builder.WebHost.UseUrls(serviceParams!.AvailablePort!);

        builder.Services.AddControllers();
        builder.Services.AddHealthChecks();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

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
        services.Configure<RedisCacheSettings>(configuration.GetSection(nameof(RedisCacheSettings)));
        services.Configure<RabbitMqSettings>(configuration.GetSection(nameof(RabbitMqSettings)));
        services.Configure<DatabaseConfig>(configuration.GetSection(nameof(DatabaseConfig)));
        services.Configure<ServiceParams>(configuration.GetSection(nameof(ServiceParams)));
        services.Configure<StoredProcedureConfig>(configuration.GetSection(nameof(StoredProcedureConfig)));
    }
    private static void ConfigureAuditLogDbContext(IServiceCollection services)
    {
        services.AddDbContext<AuditLogDbContext>((provider, options) =>
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
  
    private static void ConfigureCustomerDbContext(IServiceCollection services)
    {
        services.AddDbContext<EntityDbContext>((provider, options) =>
       
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
    private static void RegisterTransientServices(IServiceCollection services)
    {
        services.AddTransient<IAuditLogger, AuditLogger>();
   
        services.AddTransient<IProductDataService, ProductDataService>();
        services.AddTransient<ICreateLeadService, CreateLeadService>();
    }

    private static void RegisterSingletonServices(IServiceCollection services)
    {
        //services.AddSingleton<ICoreBankRepository, CoreBankRepository>();
     
        services.AddSingleton<ProductLogger>();
        services.AddSingleton<CreateLeadLogger>();
        services.AddSingleton<WarmUpLogger>();
        services.AddSingleton<BaseLogger>();
        services.AddSingleton<ApiClientLogger>();
       
    }

    [Obsolete]
    public static IServiceCollection AddMassTransitWithRabbitMq(IServiceCollection services, RabbitMqSettings rabbitMqSettings)
    {
        var password = rabbitMqSettings.IsEncrypted
            ? EncryptDecrypt.Decrypt(rabbitMqSettings.Rabitphrase!, _logger)
            : rabbitMqSettings.Rabitphrase!;

        services.AddMassTransit(x =>
        {
          
            x.AddConsumer<CentralBankProductDataRequestConsumer>();
            x.AddConsumer<CentralBankProductDataResponseConsumer>();
            x.AddConsumer<CentralBankCreateLeadRequestConsumer>();
            x.AddConsumer<CentralBankCreateLeadResponseConsumer>();
            x.AddConsumer<AuditLogConsumer>();
            x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host(new Uri(rabbitMqSettings!.Url!), h =>
                {
                    h.Username(rabbitMqSettings.UserName!);
                    h.Password(password);
                    h.Heartbeat(TimeSpan.FromSeconds(5));
                });


                cfg.ReceiveEndpoint(rabbitMqSettings.GetProductDataRequest!, ep =>
                {
                    ep.ConfigureConsumer<CentralBankProductDataRequestConsumer>(provider);
                    _logger.Info("CentralBankProductRequestConsumer-ConfigureConsumer is initialized.");
                });


                cfg.ReceiveEndpoint(rabbitMqSettings.GetProductDataResponse!, ep =>
                {
                    ep.ConfigureConsumer<CentralBankProductDataResponseConsumer>(provider);
                    _logger.Info("CentralBankProductResponseConsumer-ConfigureConsumer is initialized.");
                });



                cfg.ReceiveEndpoint(rabbitMqSettings.PostLeadRequest!, ep =>
                {
                    ep.ConfigureConsumer<CentralBankCreateLeadRequestConsumer>(provider);
                    _logger.Info("CentralBankCreateLeadRequestConsumer-ConfigureConsumer is initialized.");
                });


                cfg.ReceiveEndpoint(rabbitMqSettings.PostLeadResponse!, ep =>
                {
                    ep.ConfigureConsumer<CentralBankCreateLeadResponseConsumer>(provider);
                    _logger.Info("CentralBankCreateLeadResponseConsumer-ConfigureConsumer is initialized.");
                });



                cfg.ReceiveEndpoint(rabbitMqSettings.AuditLog!, ep =>
                {
                    ep.ConfigureConsumer<AuditLogConsumer>(provider);
                    _logger.Info("AuditLogConsumer-ConfigureConsumer is initialized.");
                });


               
            }));
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
