global using FeiNuo.Core;
using FeiNuo.Models;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        // Add services to the container.

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
}
