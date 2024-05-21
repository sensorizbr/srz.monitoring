using Microsoft.EntityFrameworkCore;
using SensorizMonitoring.Data.Context;
using System.Reflection;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;
using System;

namespace SensorizMonitoring
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("Sensoriz_Monitoring"))
            .WithMetrics(metrics =>
              metrics
                .AddAspNetCoreInstrumentation() // ASP.NET Core related
                .AddRuntimeInstrumentation() // .NET Runtime metrics like - GC, Memory Pressure, Heap Leaks etc
                .AddPrometheusExporter() // Prometheus Exporter
            );

            // Configuração do Swagger
            services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                           .AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                    });
            });

            string mySqlConnection = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            );

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseCors("AllowAll");

            app.UseOpenTelemetryPrometheusScrapingEndpoint();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "logs",
                    pattern: "logs",
                    defaults: new { controller = "Log", action = "GetLogs" });
            });

            app.UseSwagger();

            app.UseReDoc(c =>
            {
                c.DocumentTitle = "SensorizMonitoring";
                c.SpecUrl = "/swagger/v1/swagger.json"; // URL da especificação Swagger (OpenAPI)
            });

            // Habilitar o middleware para servir a interface Swagger gerada
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SensorizMonitoring");
            });
        }
    }
}