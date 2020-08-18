using COAL.CORE.Models;
using CoalServer.Services;
using CoalServer.Services.Clubs;
using CoalServer.Services.Competitions;
using CoalServer.Services.Generator;
using CoalServer.Services.Matches;
using CoalServer.Services.Players;
using CoalServer.Services.SaveGames;
using CoalServer.Services.Tables;
using CoalServer.Services.Teams;
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

            services.AddSingleton<ISaveGameService, SaveGameService>();
            services.AddSingleton<IPlayerService, PlayerService>();
            services.AddSingleton<IClubService, ClubService>();
            services.AddSingleton<ITeamService, TeamService>();
            services.AddSingleton<ICompetitionService, CompetitionService>();
            services.AddSingleton<ITableService, TableService>();
            services.AddSingleton<IMatchService, MatchService>();
            services.AddSingleton<IGeneratorService, GeneratorService>();

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
