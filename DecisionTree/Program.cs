// See https://aka.ms/new-console-template for more information

using DecisionTree.Services;
using DecisionTree.Services.FileServices;
using DecisionTree.Services.TreeServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceLifetime = DecisionTree.Services.ServiceLifetimeRp;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScoped<ITreeService, TreeService>();
        services.AddScoped<IFileService, FileService>();
        services.AddTransient<ServiceLifetimeRp>();
    })
    .Build();

RunApp(host.Services);

await host.RunAsync();



static void RunApp(IServiceProvider hostProvider)
{
    using IServiceScope serviceScope = hostProvider.CreateScope();
    IServiceProvider provider = serviceScope.ServiceProvider;
    try
    {
        ServiceLifetimeRp logger = provider.GetRequiredService<ServiceLifetimeRp>();
        logger.ExecuteService();
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        throw;
    }
    
  
    Console.WriteLine("...");
}
    