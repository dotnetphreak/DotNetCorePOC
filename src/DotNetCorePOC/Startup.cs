﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DotNetCorePOC.Interfaces;
using DotNetCorePOC.Services;
using DotNetCorePOC.Persistence;
using Microsoft.EntityFrameworkCore;
using DotNetCorePOC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DotNetCorePOC.Middlewares;

namespace DotNetCorePOC
{
    public class Startup
    {
        private IConfiguration Configuration { get; set; }
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(Configuration);
            services.AddTransient<IGreetingService, GreetingService>();
            services.AddDbContext<DotNetCorePOCDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DotNetCorePOC")));
            services.AddScoped<IRestaurantData, InMemoryRestaurantData>();
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<DotNetCorePOCDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IGreetingService greetingService)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(new ExceptionHandlerOptions()
                {
                    ExceptionHandler = context => context.Response.WriteAsync("oops!!")
                });
            }

            //app.UseMvcWithDefaultRoute();
            app.UseIdentity();
            app.UseMvc(ConfigureRoutes);


            //Below middleware will show a welcome page for every request
            //comes from diagnostic package
            app.UseWelcomePage(new WelcomePageOptions()
            {
                Path = "/welcome"
            });
            app.UseFileServer();
            //Custom middleware to server static files from node_modules folder
            app.UseNodeModules(env.ContentRootPath);
            app.UseStaticFiles();
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync(greetingService.Message);
            });
        }

        private void ConfigureRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapRoute("Default", 
                "{controller=Home}/{action=Index}/{id?}"
                );
        }
    }
}
