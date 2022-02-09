using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using App.ExtendMethods;
using App.Models;
using App.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
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
            services.AddOptions();
            var mailSetting = Configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailSetting);
            services.AddSingleton<IEmailSender, SendMailService>();

            services.AddControllersWithViews();
            services.AddRazorPages();
            //services.AddTransient(typeof(ILogger<>), typeof(ILogger<>));
            services.AddDbContext<AppDbContext>(options =>
            {
                string connectionString = Configuration.GetConnectionString("AppMvcConnectionString");
                options.UseSqlServer(connectionString);
            });
            services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
            // services.AddDefaultIdentity<AppUser>()
            // .AddEntityFrameworkStores<AppDbContext>()
            // .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 3; // Thất bại 2 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedAccount = true;   // Xác thực số điện thoại

            });
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.AccessDeniedPath = "/khongduoctruycap.html";
            });
            services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        var gconfig = Configuration.GetSection("Authentication:Google");
                        options.ClientId = gconfig["ClientId"];
                        options.ClientSecret = gconfig["ClientSecret"];
                        //https://localhost:5001/signin-google
                        options.CallbackPath = "/dang-nhap-tu-google";
                    })
                    .AddFacebook(options =>
                    {
                        var fconfig = Configuration.GetSection("Authentication:Facebook");
                        options.AppId = fconfig["AppId"];
                        options.AppSecret = fconfig["AppSecret"];
                        options.CallbackPath = "/dang-nhap-tu-facebook";
                    });
            services.Configure<RazorViewEngineOptions>(options =>
            {
                //{0} -> ten Action
                //{1} -> ten controller
                //{2} -> ten Area

                options.ViewLocationFormats.Add("/MyView/{1}/{0}" + RazorViewEngine.ViewExtension);
            });
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

// dotnet add package Microsoft.EntityFrameworkCore.Design --version 5.0
// dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 5.0
// dotnet add package MySql.Data.EntityFramework -v 5.0