using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using TvMazeScraper.API.RecurrentTask;
using TvMazeScraper.Domain.Interface;
using TvMazeScraper.Infrastructure.Repository;
using TvMazeScraper.Infrastructure.Settings;

namespace TvMazeScraper.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionSting = Configuration.GetConnectionString("TvMazeScraperDb");
            services.AddDbContext<TvMazeScraperContext>(opt => opt.UseSqlServer(connectionSting), ServiceLifetime.Transient);

            services.AddAutoMapper();
            services.AddTransient<Microsoft.Extensions.Hosting.IHostedService, BackgroundHostedService>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient(typeof(IReadOnlyRepository<>), typeof(ReadOnlyRepository<>));
            services.AddTransient(typeof(IWritingRepository<>), typeof(WritingRepository<>));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            RegisterSwaggerGenerator(services);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<TvMazeScraperContext>();
                context.Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            ConfigureSwaggerMiddleware(app);
            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private void RegisterSwaggerGenerator(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "TV Maze Scraper API", Version = "v1" });
            });
        }

        private static void ConfigureSwaggerMiddleware(IApplicationBuilder app)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "TV Maze Scraper API V1");
            });
        }
    }
}
