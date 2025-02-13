﻿global using FeiNuo.AspNetCore;
global using FeiNuo.Core;
using FeiNuo.Admin.Models;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System.Reflection;

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

            // 自定义的缓存需要在AddAppServices前注入
            // builder.Services.AddDistributedMemoryCache();

            // 注入各项服务
            builder.Services.AddAppServices();
            builder.Services.AddAppControllers();
            // 注入认证授权
            builder.Services.AddAppSecurity(builder.Configuration);

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // Mapster，扫描IRegister接口的类，自动配置
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

            var app = builder.Build();

            // 接口文档
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
            });



            // app.UseHttpsRedirection();
            // app.UseStaticFiles();
            // 跨域
            app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            // 认证授权
            app.UseAuthentication();
            app.UseAuthorization();
            // 控制器
            app.MapControllers();

            // 开发环境
            //if (app.Environment.IsDevelopment())
            //{
            // 确保数据库已经创建
            DbInitializer.EnsureDatabaseCreated(app.Services, false);
            //}
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
