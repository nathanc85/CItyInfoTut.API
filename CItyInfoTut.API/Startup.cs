using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using NLog.Extensions.Logging;
using CItyInfoTut.API.Services;
using Microsoft.Extensions.Configuration;
using CItyInfoTut.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CItyInfoTut.API
{
    public class Startup
    {
        public static IConfigurationRoot Configuration;

        public Startup(IHostingEnvironment env) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddMvcOptions(o => o.OutputFormatters.Add(
                new XmlDataContractSerializerOutputFormatter()
            ));
#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif
            // Register CityInfoDbContext for dependency injection.
            var connectionString = @"Server=(localdb)\mssqllocaldb;Database=CityInfoDB;Trusted_connection=True;";
            var connectionString1 = @"User ID=sa;Password=SuperParola01;Server=192.168.1.13;Database=CityInfoDB;";
            services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(connectionString1));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler();
            }

            app.UseStatusCodePages();

            app.UseMvc();

            //app.Run((context) =>
            //{
            //    throw new Exception("Friendly Exception!");
            //});

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
