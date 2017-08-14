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
                .AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

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

            // This is the example given in the tutorial
            // var connectionString = @"Server=(localdb)\mssqllocaldb;Database=CityInfoDB;Trusted_connection=True;";

            // This is the connection string at home. Find the IP by running: $ ipconfig getifaddr en0 .
            var connectionString = Startup.Configuration["connectionStrings:cityInfoDBConnectionString"];
            //var connectionString = Startup.Configuration["cityInfoDBConnectionString"];

			// This is the connection at work. Find the IP by running: $ ipconfig getifaddr en0 .
			//var connectionString = @"User ID=sa;Password=SuperParola01;Server=10.28.109.29;Database=CityInfoDB;";
			services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(connectionString));

            // Register our DB Repository.
            services.AddScoped<ICityInfoRepository, CityInfoRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, CityInfoContext cityInfoContext)
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

            cityInfoContext.EnsureSeedDataForContext();

            app.UseStatusCodePages();

            AutoMapper.Mapper.Initialize(cfg => {
                cfg.CreateMap<Entities.City, Models.CityWithoutPointsOfInterestDTO>();
            });

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
