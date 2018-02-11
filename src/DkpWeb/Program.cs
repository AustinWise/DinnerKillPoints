using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DkpWeb
{
    public class Program
    {
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls("http://*:8080")
                .Build();

        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);
            host.Run();
        }
    }
}
