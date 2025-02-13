global using FeiNuo.Core;
using FeiNuo.Admin.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

namespace FeiNuo.Admin;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
           .Enrich.FromLogContext().WriteTo.Console().CreateBootstrapLogger();
        try
        {
            Log.Information("Starting web host ... ");
            var builder = WebApplication.CreateBuilder(args);
            // 注入 Serilog
            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration.ReadFrom.Configuration(context.Configuration).Enrich.FromLogContext();
            });
            // 注入 EFCore
            //var conn = builder.Configuration.GetConnectionString("MySql");
            var conn = builder.Configuration.GetConnectionString("SqlServer");
            builder.Services.AddDbContext<FNDbContext>(opt =>
            {
                // opt.UseMySql(conn, ServerVersion.Parse("8.0.26-mysql"), ops => ops.TranslateParameterizedCollectionsToConstants());
                opt.UseSqlServer(conn);

                if (builder.Environment.IsDevelopment())
                {
                    opt.EnableSensitiveDataLogging();
                    opt.EnableDetailedErrors();
                }
            });

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/openapi/v1.json", "v1");
                });

                DbInitializer.EnsureDatabaseCreated(app.Services, false);
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
