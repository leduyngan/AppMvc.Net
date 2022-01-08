using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using App.ExtendMethods;
using App.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App
{
    public class Startup
    {
        public static string ContentRootPath { set; get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            ContentRootPath = env.ContentRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();
            //services.AddTransient(typeof(ILogger<>), typeof(ILogger<>));

            services.Configure<RazorViewEngineOptions>(options =>
            {
                //{0} -> ten Action
                //{1} -> ten controller
                //{2} -> ten Area

                options.ViewLocationFormats.Add("/MyView/{1}/{0}" + RazorViewEngine.ViewExtension);
            });
            // services.AddSingleton<ProductService>();
            // services.AddSingleton<ProductService, ProductService>();
            // services.AddSingleton(typeof(ProductService));
            services.AddSingleton(typeof(ProductService), typeof(ProductService));
            services.AddSingleton<PlanetService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.AddStatusCodePage();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/sayhi", async (context) =>
                {
                    await context.Response.WriteAsync($"Hello ASP.NET MVC {DateTime.Now}");
                });

                // endpoints.MapControllers
                // endpoints.MapControllerRoute
                // endpoints.MapDefaultControllerRoute
                // endpoints.MapAreaControllerRoute




                endpoints.MapControllers();

                endpoints.MapControllerRoute(
                    name: "first",
                    pattern: "{url:regex(^((xemsanpham)|(viewproduct))$)}/{id:range(2,3)}",
                    defaults: new
                    {
                        controller = "First",
                        action = "ViewProduct"
                    }
                    // constraints: new
                    // {
                    //     url = new RegexRouteConstraint(@"^((xemsanpham)|(viewproduct))$"),
                    //     id = new RangeRouteConstraint(2, 3)
                    // }
                    //IRouteConstraint 
                );

                endpoints.MapAreaControllerRoute(
                    name: "product",
                    pattern: "/{controller}/{action=Index}/{id?}",
                    areaName: "ProductManage"
                );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "/{controller=Home}/{action=Index}/{id?}"
                );
                endpoints.MapRazorPages();
            });

        }
    }
}







// dotnet tool install -g dotnet-aspnet-codegenerator  -- dai dat cong cu generation code
// dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design

// dotnet aspnet-codegenerator controller -name  Planet -namespace App.Controllers -outDir Controllers