global using FeiNuo.AspNetCore;
global using FeiNuo.Core;
using FeiNuo.Admin.Models;
using Mapster;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
                var logBase = $"./Logs/{DateTime.Now:yyyy-MM}";
                configuration.MinimumLevel.Information()
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                    .WriteTo.Console(outputTemplate: AppConstants.LOG_FORMAT_CONSOLE)
                    //.WriteTo.Logger(c => c.Filter.ByIncludingOnly(d => d.Level == LogEventLevel.Information).WriteTo.File($"{logBase}/Log-Info-.log", restrictedToMinimumLevel: LogEventLevel.Information, rollingInterval: RollingInterval.Day, outputTemplate: AppConstants.LOG_FORMAT_FILE, fileSizeLimitBytes: 104857600))
                    .WriteTo.File($"{logBase}/Log-.log", restrictedToMinimumLevel: LogEventLevel.Warning, rollingInterval: RollingInterval.Day, outputTemplate: AppConstants.LOG_FORMAT_FILE, fileSizeLimitBytes: 104857600)
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext();
            });
            // 注入 EFCore
            var conn = builder.Configuration.GetConnectionString("PgSql");
            builder.Services.AddDbContext<FNDbContext>(opt =>
            {
                //opt.UseMySql(conn, ServerVersion.Parse("8.0.26-mysql"), ops => ops.TranslateParameterizedCollectionsToConstants());
                opt.UseNpgsql(conn);

                // 解决 DateTimeOffset 转换问题
                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
                AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);

                if (builder.Environment.IsDevelopment())
                {
                    opt.EnableSensitiveDataLogging();
                    opt.EnableDetailedErrors();
                }
            });
            // 注入认证授权
            builder.Services.AddFNAspNetCore(builder.Configuration);

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            });

            // Mapster，扫描IRegister接口的类，自动配置
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());

            var app = builder.Build();

            // 接口文档
            app.MapOpenApi().RequireAuthorization(AppConstants.AUTH_POLICY_IGNORE);
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
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


    /// <summary>
    /// openapi 添加 security 支持
    /// </summary>
    public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
    {
        private readonly IAuthenticationSchemeProvider authenticationSchemeProvider;

        public BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            this.authenticationSchemeProvider = authenticationSchemeProvider;
        }

        public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
            if (authenticationSchemes.Any(authScheme => authScheme.Name == JwtBearerDefaults.AuthenticationScheme))
            {
                var requirements = new Dictionary<string, OpenApiSecurityScheme>
                {
                    ["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower(),
                        In = ParameterLocation.Header,
                        BearerFormat = "Json Web Token",
                        Name = "Authorization",
                        Description = $"授权认证，在下方输入Bearer {{token}},例如：<br/>Bearer {AppConstants.SUPER_ADMIN_TOKEN}",
                    }
                };
                document.Components ??= new OpenApiComponents();
                document.Components.SecuritySchemes = requirements;

                foreach (var operation in document.Paths.Values.SelectMany(path => path.Operations))
                {
                    operation.Value.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecurityScheme { Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme } }] = Array.Empty<string>()
                    });
                }
            }
        }
    }
}
