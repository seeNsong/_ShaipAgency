using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Demo.Blazor;

using DevExpress.Blazor;
using DevExpress.Blazor.DocumentMetadata;
using System;
using System.Net.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace ShaipAgency
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
#if DEBUG
            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
#endif

            services.AddMvc(options => options.EnableEndpointRouting = false)
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddRazorPages();
            services.AddServerSideBlazor();
           
            services.Configure<DemoConfiguration>(Configuration.GetSection("DemoConfiguration"));
          
            services.AddDocumentMetadata((serviceProvider, registrator) => {
                DemoConfiguration config = serviceProvider.GetService<IOptions<DemoConfiguration>>().Value;
                config.RegisterPagesMetadata(registrator);
            });
            services.AddHttpClient();
            services.AddScoped<HttpClient>((sp) => {
                var httpClient = sp.GetService<IHttpClientFactory>().CreateClient();
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                return httpClient;
            });

            services.AddDevExpressBlazor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseMvcWithDefaultRoute();

            app.UseEndpoints(endpoints => {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
