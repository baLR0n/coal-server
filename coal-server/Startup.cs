using COAL.CORE.Models;
using CoalServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace coal_server
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
            // Add Cors
            services.AddCors();

            services.Configure<CoalDatabaseSettings>(
                Configuration.GetSection(nameof(CoalDatabaseSettings)));

            services.AddSingleton<ICoalDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<CoalDatabaseSettings>>().Value);

            services.AddSingleton<SaveGameService>();
            services.AddSingleton<PlayerService>();
            services.AddSingleton<TeamService>();
            services.AddSingleton<CompetitionService>();
            services.AddSingleton<TableService>();
            services.AddSingleton<MatchService>();
            services.AddSingleton<GeneratorService>();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
                app.UseHsts();
            }

            // Enable Cors
            app.UseCors(
                options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials()
                );

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
