using ApiMotorsAdService.Data;
using ApiMotorsAdService.Resources;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ApiMotorsAdService
{
    /// <summary>
    /// Custom Extension Methods Class
    /// </summary>
    public static class CustomExtensionMethods
    {
        #region Methods

        // Swagger
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(ApiResources.ApiVersion, new OpenApiInfo
                {
                    Title = $"{ApiResources.AppName} - Catalog API",
                    Version = ApiResources.ApiVersion,
                    Description = $"{ApiResources.AppName} Microservice API."
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter JWT with Bearer into field. Ex: Bearer + space + Token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                         new OpenApiSecurityScheme
                         {
                           Reference = new OpenApiReference
                           {
                             Type = ReferenceType.SecurityScheme,
                             Id = "Bearer"
                           }
                          },
                          new string[] { }
                    }
                });

                // XML Documentation
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            return services;
        }

        #endregion Methods
    }

    /// <summary>
    /// Swagger Authorization
    /// </summary>
    public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        #region Methods

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter);
            var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);

            if (isAuthorized && !allowAnonymous)
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<OpenApiParameter>();

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Description = "access token",
                    Required = true

                    //Type = SecuritySchemeType.ApiKey,
                    //Format = "Bearer {Token}"
                });
            }
        }

        #endregion Methods
    }

    public class Startup
    {
        #region Properties

        public IConfiguration Configuration { get; }

        #endregion Properties

        #region Constructors

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <param name="env">IWebHostEnvironment</param>
        /// <param name="dbContext">ServiceDbContext</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ServiceDbContext dbContext)
        {
            // AppInsights Telemetry
            var configuration = TelemetryConfiguration.CreateDefault();
            configuration.InstrumentationKey = Configuration["ApplicationInsights:InstrumentationKey"];
            configuration.DisableTelemetry = bool.Parse(Configuration["ApplicationInsights:DisableTelemetry"]);
            configuration.TelemetryChannel.DeveloperMode = bool.Parse(Configuration["ApplicationInsights:DeveloperMode"]);
            //configuration.TelemetryProcessorChainBuilder.Use(next => new LogTelemetryDependencies(next, Configuration));
            configuration.TelemetryProcessorChainBuilder.Build();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region Globalization and Location

            string ptBRCulture = "pt-BR";
            var supportedCultures = new[]
            {
                new CultureInfo(ptBRCulture),
                new CultureInfo("en-US"),
                new CultureInfo("en-GB"),
                new CultureInfo("es-ES"),
                new CultureInfo("es-MX"),
                new CultureInfo("es"),
                new CultureInfo("fr-FR"),
                new CultureInfo("fr"),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(ptBRCulture),
                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,
                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });

            #endregion Globalization and Location

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Swagger CustomExtensionMethods
            app.UseSwagger()
              .UseSwaggerUI(c =>
              {
                  // Json contendo as informacoes de conexao para geracao de RestApiClient via Swagger
                  c.SwaggerEndpoint($"/swagger/{ApiResources.ApiVersion}/swagger.json", $"{ApiResources.AppName}.API {ApiResources.ApiVersion}");
              });

            #region Create and Seed Database

            try
            {
                Console.WriteLine($"############## SLEEP => START 10 seconds (PARA O MYSQL SUBIR A CONEXAO QUANDO CRIADO PELA PRIMEIRA VEZ) ################");
                Thread.Sleep(10000);
                Console.WriteLine($"############## SLEEP => STOP ################");

                // Create Tables
                dbContext.Database.Migrate();

                // Seed database
                AdDataInitializer.SeedData(dbContext);
            }
            catch (Exception)
            {
                Console.WriteLine($"###################### MIGRATIONS => PARE A EXECUCAO E RODE NOVAMENTE ######################");
            }           

            #endregion Create and Seed Database
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Use MySql Docker
            services.AddDbContext<ServiceDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlDockerConnection")));
            // Use MySql Local / Migrations
            //services.AddDbContext<ServiceDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MySqlLocalConnection")));

            #region JWT Bearer Token -> OAuth

            // Authentication ===
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //.AddCookie()
            //.AddJwtBearer(jwtBearerOptions =>
            //{
            //    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters()
            //    {
            //        ValidateActor = false,
            //        ValidateAudience = false,
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ValidIssuer = Configuration["Token:Issuer"],
            //        ValidAudience = Configuration["Token:Audience"],
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:Key"])),
            //        ClockSkew = TimeSpan.Zero
            //    };
            //});

            // Authorization ===
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
            //        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            //        .RequireAuthenticatedUser().Build());
            //    //options.AddPolicy("AtLeast21", policy =>
            //    //    policy.Requirements.Add(new MinimumAgeRequirement(21)));
            //});

            #endregion JWT Bearer Token -> OAuth

            // Telemetry
            services.AddSingleton(typeof(TelemetryClient));
            services.AddSingleton(typeof(Dictionary<string, string>));
            services.AddSingleton(typeof(Dictionary<string, double>));
            services.AddApplicationInsightsTelemetry();

            // Swagger
            services.AddSwagger();

            services.AddControllers();
        }

        #endregion Methods
    }
}