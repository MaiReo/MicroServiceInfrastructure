using Autofac;
using Core.Web.EFCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
// using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Core.Web.Startup
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterRequiredServices(Configuration);

            services.AddControllers();

            var databaseId = Guid.NewGuid().ToString("N");

            services.AddDbContext<WebDbContext>(option => option.UseInMemoryDatabase(databaseId));

            // NSwag
            services.AddOpenApiDocument(cfg =>
            {
                cfg.PostProcess = doc =>
                {
                    doc.Info.Version = "v1";
                    doc.Info.Title = "Test API";
                    doc.Info.Description = "demo and test";
                    doc.Info.TermsOfService = "None";
                };
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.AddCoreModules();
            builder.RegisterAssemblyByConvention(typeof(Startup).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.Extensions.Hosting.IHostApplicationLifetime applicationLifetime)
        {

            app.UseMessageBus(applicationLifetime);
            
            app.UseRouting();

            app.UseEndpoints(e => e.MapDefaultControllerRoute());

            app.UseOpenApi();
            app.UseSwaggerUi3();

        }
    }
}
