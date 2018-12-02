using System;
using System.IO;
using FluentScheduler;
using Microsoft.Extensions.DependencyInjection;

namespace Batches.FluentScheduler.Jobs
{
    public interface IService
    {
        void EscreverArquivo();
    }

    public class Service : IService
    {
        public void EscreverArquivo()
        {
            File.WriteAllText($"{Math.Abs(DateTime.Now.GetHashCode())}", $"{DateTime.Now}");
        }
    }

    public class CriarArquivoJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        public CriarArquivoJob(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }

        public void Execute()
        {
            using(var scope = _serviceProvider.CreateScope()){
                var service = scope.ServiceProvider.GetRequiredService<IService>();
                service.EscreverArquivo();
            }
        }
    }
}
