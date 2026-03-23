#nullable enable
using System.Security.Claims;
using System.Threading.Tasks;
using Austin.DkpLib;
using DkpWeb.Config;
using DkpWeb.Data;
using DkpWeb.Models;
using DkpWeb.Services;
using Google.Api;
using Google.Cloud.Diagnostics.AspNetCore3;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
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
            {
                string? connectionString = Configuration.GetConnectionString("Postgres");
                options.UseNpgsql(connectionString);
            });
            services.AddDatabaseDeveloperPageExceptionFilter();

            // TODO: consider adding a config switch for switching between ASP.NET Identity and IAP
            // services.AddIdentity<ApplicationUser, IdentityRole>()
            //     .AddDefaultUI()
            //     .AddEntityFrameworkStores<ApplicationDbContext>();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddScoped<IBillSplitterServices, InProcessBillSplitterServices>();

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

            services.AddIap();
            services.AddAuthentication().AddIap();
            services.AddSingleton<AddSomeExtraRolesMiddleware>();
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            app.UseHealthChecks("/healthz");

#if DEBUG
            if (env.IsDevelopment())
            {
                app.UseIapSimulator();
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            }
            else
#endif
            {
                app.UseIap();
                app.UseExceptionHandler("/Home/Error");

                // Use forwarded headers from Cloud Run
                var forwardOpts = new ForwardedHeadersOptions();
                forwardOpts.KnownProxies.Add(System.Net.IPAddress.Parse("169.254.1.1"));
                forwardOpts.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                app.UseForwardedHeaders(forwardOpts);

                app.UseHsts();

            }
            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.MapStaticAssets();

            app.UseRouting();

            app.UseAuthentication();
            app.UseMiddleware<AddSomeExtraRolesMiddleware>();
            app.UseAuthorization();

            app.MapDefaultControllerRoute()
                .WithStaticAssets();

            app.MapRazorPages()
                .WithStaticAssets();
        }
    }

    // This is for backwards compatibility with the ASP.NET Core Identity version of authentication.
    class AddSomeExtraRolesMiddleware : IMiddleware
    {
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var user = context.User;

            if (user is not null)
            {
                var identity = user.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, "DKP", ClaimValueTypes.String, "DKP", "DKP", identity));
                    // TODO: a better way to assign roles to people
                    if (user.FindFirst(ClaimTypes.Email)?.Value == "austinwise@gmail.com")
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, "Admin", ClaimValueTypes.String, "DKP", "DKP", identity));
                    }
                }
            }

            return next(context);
        }
    }
}
