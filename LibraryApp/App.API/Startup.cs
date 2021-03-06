using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Session;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Cors;
using Microsoft.AspNet.Mvc.Formatters;
using Microsoft.AspNet.Cors;
using Microsoft.Data.Entity;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using AspnetWebApi2Helpers.Serialization;
using AspnetWebApi2Helpers.Serialization.Protobuf;
using App.Models;
using App.Models.Identity;
using App.Data;
using App.Data.SampleData;
using App.Services.Ahs;

namespace App.API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv)
        {
            // Setup configuration sources.

            var builder = new ConfigurationBuilder()
                .SetBasePath(appEnv.ApplicationBasePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        
        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by a runtime.
        // Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        { 
            services.AddCors(options => options.AddPolicy("AllowAll", 
            p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));
            services.AddMvc();
            
            // Uncomment the following line to add Web API services which makes it easier to port Web API 2 controllers.
            // You will also need to add the Microsoft.AspNet.Mvc.WebApiCompatShim package to the 'dependencies' section of project.json.
            // services.AddWebApiConventions();
            
            // Add Scoped = Resolve dependency injection
            services.AddScoped<LibraryDbContext, LibraryDbContext>();
            services.AddScoped<IMediatheekService, MediatheekService>();
            
            // Add Entity Framework services to the services container.
            services.AddEntityFramework()
                .AddSqlite()
                .AddDbContext<LibraryDbContext>(options =>
                    options.UseSqlite(Configuration["Data:DefaultConnection:ConnectionString"]));
            
            // Formatter JSON        
            services.Configure<MvcOptions>(options =>
            {  
                var jsonOutputFormatter = new JsonOutputFormatter();
                jsonOutputFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                jsonOutputFormatter.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Ignore;
                
                jsonOutputFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore; 
                jsonOutputFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
    
                options.OutputFormatters.Insert(0, jsonOutputFormatter);
                
                options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAll"));
                
            });
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Add the platform handler to the request pipeline.
            app.UseIISPlatformHandler();

            // Add the following route for porting Web API 2 controllers.
            // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");
            
            // Configure the HTTP request pipeline.
            app.UseStaticFiles();
            
            // Add MVC to the request pipeline.            
            app.UseMvc(routes =>
            {       
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{libraryCode?}/{id?}");

            });
            app.UseCors("AllowAll");
        }
        
        // Entry point for the application.
        public static void Main(string[] args) => Microsoft.AspNet.Hosting.WebApplication.Run<Startup>(args);
    }
}