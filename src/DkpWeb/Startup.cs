using DkpWeb.Config;
using DkpWeb.Data;
using DkpWeb.Models;
using DkpWeb.Services;
using Google.Cloud.Diagnostics.AspNetCore3;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sakura.AspNetCore.Mvc;

namespace DkpWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services, IWebHostEnvironment env)
        {
            services.AddOptions();
            services.Configure<EmailOptions>(Configuration.GetSection("Gmail"));

            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("Postgres")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddBootstrapPagerGenerator(options =>
            {
                // Use default pager options.
                options.ConfigureDefault();
            });

            services.AddTransient<MailMerge>();

            if (env.IsDevelopment())
            {
                services.AddLogging(builder =>
                {
                    builder.AddConsole();
                });
            }

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddHealthChecks();

            if (!env.IsDevelopment())
            {
                services.AddGoogleDiagnosticsForAspNetCore(projectId: Configuration["GcpProjectId"]);
                var dpSection = Configuration.GetSection("DataProtection");
                services.AddDataProtection().PersistKeysToGoogleCloudStorage(dpSection["Bucket"], dpSection["ObjectName"]);
            }
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthChecks("/healthz");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
