using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WikiVideojuegos.Data;

namespace WikiVideojuegos.Tests;

public class WebAppFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);
            foreach (var d in services.Where(d => d.ServiceType == typeof(AppDbContext)).ToList())
                services.Remove(d);
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));
        });
    }
}
