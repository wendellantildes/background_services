using System;
using System.IO;
using DasMulli.Win32.ServiceUtils;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;
using Service.WindowsService.Jobs;

namespace Service.WindowsService
{
    class Program
    {
        public static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddScoped(typeof(IService), typeof(Service.WindowsService.Jobs.Service));
            var builder = services.BuildServiceProvider();
            var myService = new MyService(builder);
            var serviceHost = new Win32ServiceHost(myService);
            serviceHost.Run();
        }
    }

    class MyService : IWin32Service
    {
        public string ServiceName => "Test Service";

        private readonly IServiceProvider _serviceProvider;

        public MyService(IServiceProvider serviceProvider){
            _serviceProvider = serviceProvider;
        }

        public void Start(string[] startupArguments, ServiceStoppedCallback serviceStoppedCallback)
        {
            _serviceProvider.GetRequiredService<IService>().EscreverArquivo();
            JobManager.Initialize(new CustomRegistry(_serviceProvider));
        }

        public void Stop()
        {
            // shut it down again
        }
    }

    public class CustomRegistry : Registry
    {

        public CustomRegistry(IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetRequiredService<IService>();
            //Verificar a necessidade de criar 
            Schedule(new CriarArquivoJob(serviceProvider)).ToRunEvery(1).Minutes();
        }
    }
}
