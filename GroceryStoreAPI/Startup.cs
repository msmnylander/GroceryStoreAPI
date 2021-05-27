using GroceryStoreAPI.Configuration;
using GroceryStoreAPI.Contracts;
using GroceryStoreAPI.Middleware;
using GroceryStoreAPI.Repositories;
using GroceryStoreAPI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace GroceryStoreAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //
            // Load configuration parameters
            //
            services.Configure<RequestTimeoutSettings>(Configuration.GetSection("RequestTimeoutSettings"));
            services.Configure<ApiKeySettings>(Configuration.GetSection("ApiKeySettings"));

            //
            // Support API versioning
            //
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });

            //
            // Swagger requires this
            //
            services.AddMvcCore().AddApiExplorer();

            //
            // Set up Swagger
            //
            services.AddSwaggerGen(c =>
            {
               c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "1.0",
                    Title = "Grocery Store API",
                    Description = "Grocery Store Web API",
                });           
            });

            //
            // These are trivial implementations so we can implement them as scoped (per request).
            // 
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<ICustomerService, CustomerService>();

            //
            // This is implemented as a singleton because we load local resources
            //
            services.AddSingleton<ICustomerRepository, CustomerRepository>();

            //
            // Load controllers. Allow XML returns in addition to JSON.
            //
            services.AddControllers()
                .AddXmlSerializerFormatters();

            //
            // Add Cookie authentication
            //
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options => {
                options.Cookie.Name = "GroceryStoreAPI";
            });
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env, ILogger<Startup> logger)
        {
            //
            // Custom exception handler
            //
            app.UseExceptionHandlerMiddleware(logger);

            //
            // Support routing
            //
            app.UseRouting()

            //
            // Set up Swagger
            //
            .UseSwagger()
            .UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "GroceryStoreAPI");
                c.RoutePrefix = string.Empty;
            })

            //
            // In this instance, we don't care about CORS.
            //
            .UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader())

            //
            // Support authentication and authorization
            //
            .UseAuthentication()
            .UseAuthorization()

            //
            // Support configurable request timeout from client. See RequestTimeoutMiddleware.
            //
            .UseRequestTimeout()

            //
            // A POC API Key middleware hook. Not used when Cookie Authentication is enabled
            //
            //.RequireApiKey()

            //
            // Set up controller endpoints
            //
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
