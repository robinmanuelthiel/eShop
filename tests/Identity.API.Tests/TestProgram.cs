using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace Identity.API.Tests
{
    public class Program
    {
        public static IHostBuilder CreateHostBuilder(string[]? args = null) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<TestStartup>()
                        .UseUrls("http://localhost:5000");
                });
    }
}
