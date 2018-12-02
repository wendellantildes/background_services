using System;
using System.IO;

namespace Batches.Jobs
{

    public interface IService {
        void EscreverArquivo();
    }

    public class Service  : IService
    {
        public void EscreverArquivo(){
            File.WriteAllText($"{Math.Abs(DateTime.Now.GetHashCode())}", $"{DateTime.Now}");
        }
    }

    public class CriarArquivoJob
    {
        private readonly IService service;
        public CriarArquivoJob(IService service){
            this.service = service;
        }

        public void Execute(){
            this.service.EscreverArquivo();
        }
    }
}
