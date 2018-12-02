#define INMEMORY
//#define SQLSERVER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Batches.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hangfire.MemoryStorage;



//Para produção : http://docs.hangfire.io/en/latest/deployment-to-production/making-aspnet-app-always-running.html
//Aparentemente não há necessidade de configuração para o .net core. Procurar no link acima por: Is there a version of this for AspNet Core?
namespace Batches
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddScoped(typeof(IService), typeof(Service));
            var connectionString = Configuration.GetConnectionString("Jobs");

#if SQLSERVER

            /*Banco de dados*/
            services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
#endif
#if INMEMORY
            /*Só mémória*/
            services.AddHangfire(x => x.UseMemoryStorage());
#endif
            GlobalConfiguration.Configuration.UseActivator(new ContainerJobActivator(services.BuildServiceProvider()));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            RecurringJob.AddOrUpdate<CriarArquivoJob>("criar_arquivo", x => x.Execute(), Cron.Minutely);

#if SQLSERVER

            //Importante remover o job se em um novo deploy ele não for mais usado
            //RecurringJob.RemoveIfExists("criar_arquivo");
#endif



            //RecurringJob.AddOrUpdate<CriarArquivoJob>("criar_arquivo", x => x.Execute(), Cron.Minutely);

            /*
             * Será executado todos os dias às 10:53. Importante colocar timezone pq o default é UTC
             * http://en.wikipedia.org/wiki/Cron#CRON_expression
            */
            RecurringJob.AddOrUpdate<CriarArquivoJob>("criar_arquivo_programado", x => x.Execute(), "53 10 * * *", TimeZoneInfo.Local);

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
