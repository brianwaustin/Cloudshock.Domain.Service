using Audit.Core.Providers;
using Audit.WebApi;
using Serilog;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Console.WriteLine("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
        //.WriteTo.Console()
        .ReadFrom.Configuration(ctx.Configuration));

    // Add services to the container.
    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    /* Audit.Net WebApi */
    builder.Services.AddMvc(mvc =>
        {
            mvc.AddAuditFilter(config => config
                .LogActionIf(d => d.ControllerName == "Racetrack")
                .WithEventType("{verb}.{controller}.{action}")
                .IncludeHeaders()
                .IncludeResponseHeaders()
                .IncludeRequestBody()
                .IncludeResponseBody());
        });

    var app = builder.Build();

    // Enable / Disable for Development
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();   

        foreach (var c in builder.Configuration.AsEnumerable())
        {
            Console.WriteLine(c.Key + " = " + c.Value);
        }     
        
        /* Development Audit Logging to File */
        Audit.Core.Configuration.Setup()
        .UseFileLogProvider(config => config
            .DirectoryBuilder(_ => $@"logs\{DateTime.Now:yyyy-MM-dd}")
            .FilenameBuilder(auditEvent => $"{auditEvent.Environment.UserName}_{DateTime.Now.Ticks}.json"));        
    }
    else
    {
        /* === Setup Production Audit Logging == */
        if(builder.Configuration["CosmosDb:Endpoint"] == "true"){
            /* CosmosDb */
            Audit.Core.Configuration.DataProvider = new Audit.AzureCosmos.Providers.AzureCosmosDataProvider(config => config
                .Endpoint(builder.Configuration["CosmosDb:Endpoint"])
                .AuthKey(builder.Configuration["CosmosDb:AuthKey"])
                .Database(builder.Configuration["CosmosDb:Database"])
                .Container(builder.Configuration["CosmosDb:Container"])
                .WithId(_ => Guid.NewGuid().ToString().ToUpper()));  
        }else{
            /* Console Out */
            var dataProvider = new DynamicDataProvider();
            dataProvider.AttachOnInsert(ev => {
                ev.EventType = "AUDIT"; 
                Console.Write(ev.ToJson());
                });
            Audit.Core.Configuration.DataProvider = dataProvider;
        }        
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers(); 

    /* Enable request tracking */
    app.UseSerilogRequestLogging();

    app.Use((context, next) =>
    {
        context.Request.EnableBuffering();
        return next();
    });

    app.Run();
}
catch (Exception ex)
{    
    Console.WriteLine("Starting up {@exception}", ex);
}
finally
{    
    Console.WriteLine("Shut down complete");    
}