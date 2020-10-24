using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PurchaseDataCalculatorAPI.Interfaces;
using PurchaseDataCalculatorAPI.Models;
using PurchaseDataCalculatorAPI.Providers;

namespace PurchaseDataCalculatorAPI
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
            // CORS disabled to be able to test from Azure with Postman too:
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });

            services.AddScoped<IPurchaseProvider, PurchaseProvider>();
            services.AddScoped<IGrossCalculator, GrossCalculator>();
            services.AddScoped<INetCalculator, NetCalculator>();
            services.AddScoped<IVatCalculator, VatCalculator>();

            services.AddControllers();

            services.AddSwaggerDocument(c =>
            {
                c.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Purchase Data Calculator API";
                    document.Info.Description = "The API waits for one of the net, gross or VAT amounts and additionally a valid Austrian VAT rate (10%, 13%, 20%). " +
                                                "The other two missing amounts (net/gross/VAT) are calculated by the system.";
                };
            });
            
            /* Deployed to Azure on the following address, you can use PostMan to see the result:
             * https://purchasedatacalculatorapi.azurewebsites.net/api/purchase?VatRate=10&GrossAmount=120
             */

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsPolicy");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Exception handling
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;
                
                var result = JsonSerializer.Serialize(new { error = exception.Message });
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(result);
            }));

            //Swagger
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
