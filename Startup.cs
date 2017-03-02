using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LevelGenerator
{
    public class Startup
    {
		public Startup(IHostingEnvironment env)
        {
            // This line of code can be used to view the directory structure in Docker.
            // ListDirectory(Directory.GetParent(env.WebRootPath).FullName);
		}
			    
		public void ConfigureServices(IServiceCollection services)
        {
			services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
			loggerFactory.AddDebug();
			
            app.UseMvc();
			app.UseStaticFiles();
        }

		static void ListDirectory(string dir)
        {
            try
            {
                foreach (string f in Directory.GetFiles(dir))
                    Console.WriteLine(f);
                foreach (string d in Directory.GetDirectories(dir))
                {
                    Console.WriteLine(d);
                    ListDirectory(d);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
