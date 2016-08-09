using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using DkpWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DkpWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            var roles = host.Services.GetService<RoleManager<IdentityRole>>();
            EnsureRole(roles, "Admin").Wait();
            EnsureRole(roles, "DKP").Wait();

            host.Run();
        }

        static async Task EnsureRole(RoleManager<IdentityRole> roles, string name)
        {
            var role = await roles.FindByNameAsync(name);
            if (role == null)
                await roles.CreateAsync(new IdentityRole(name));
        }
    }
}
