using ITServiceApp.Data;
using ITServiceApp.MapperProfiles;
using ITServiceApp.Models.Identity;
using ITServiceApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ITServiceApp
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

            services.AddDbContext<MyContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlConnection"));
            });
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
             {
                 options.Password.RequireUppercase = false;
                 options.Password.RequireLowercase = false;
                 options.Password.RequireDigit = true;
                 options.Password.RequiredLength = 5;

                 //lockout settings
                 options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                 options.Lockout.MaxFailedAccessAttempts = 3;
                 options.Lockout.AllowedForNewUsers = false;

                 //User settings
                 options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvwxyzABCDEFGHIJKLMNOPRSTUVWXYZ0123456789-._@+";
                 options.User.RequireUniqueEmail = true;

                 //options.SignIn.RequireConfirmedEmail = true;   //emaili confirm edilmi� hesaplar girsin demek
             }).AddEntityFrameworkStores<MyContext>().AddDefaultTokenProviders();// register k�sm�ndaki genereateemailtoken k�sm� i�in

            services.ConfigureApplicationCookie(options =>
            {
                //cookie settings
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccesDenied";
                options.SlidingExpiration = true;
            });

            services.AddTransient<IEmailSender, EmailSender>();

            services.AddScoped<IPaymentService, IyzicoPaymentService>();

            services.AddAutoMapper(options =>
            {
                options.AddProfile<PaymentProfile>();
                options.AddProfile<EntityProfile>();
                //options.AddProfile(typeof(PaymentProfile)); ikisi de olur.
            });

            services.AddControllersWithViews()
                .AddNewtonsoftJson(
                options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection(); //http - g�venli sertifika �al��mas� i�in
            app.UseStaticFiles(); //wwwroot klas�r�ndeki statik dosyalara eri�mek i�in
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"node_modules")),
                RequestPath = new PathString("/Vendor")
            });

            app.UseRouting(); //routing mekanizmas�

            app.UseAuthentication();//login logout kullanmak i�in
            app.UseAuthorization();//Authorization attirbute kullanabilmek i�in

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{Controller=home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    areaName:"Admin",
                    name: "Admin",
                    pattern: "Admin/{controller=Manage}/{action=Index}/{id?}"
                );
            });//default routing nas�l olaca�� belirtmek i�in
        }
    }
}
