using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationsTestHallOfFame;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.Single(
                d => d.ServiceType ==
                     typeof(DbContextOptions<ApplicationDbContext>));

            services.Remove(dbContextDescriptor);
            services.AddDbContext<ApplicationDbContext>(x => x.UseInMemoryDatabase("db"));
        });

    }
}