using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FeedService.DbModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FeedService.DbModels.Interfaces;
using FeedService.Infrastructure.Logger;
using System.IO;

namespace FeedService
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<FeedServiceContext>(options =>
                options.UseSqlServer(connection));
            services.AddMemoryCache();
            // Add framework services.
            services.AddMvc().
                AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddSingleton<IRepository<User>, Repository<User, FeedServiceContext>>();
            services.AddSingleton<IRepository<Collection>, Repository<Collection, FeedServiceContext>>();
            services.AddSingleton<IRepository<Feed>, Repository<Feed, FeedServiceContext>>();
            services.AddSingleton<IRepository<CollectionFeed>, Repository<CollectionFeed, FeedServiceContext>>();
            services.AddSingleton<IFeedServiceUoW, FeedServiceUnit>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory
                .WithFilter(new FilterLoggerSettings
                {
                    { "AccountController", LogLevel.Error },
                    { "CollectionsController", LogLevel.Error },
                    { "FeedServiceController", LogLevel.Error }
                })
                .AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logger.txt"));
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = AuthOptions.ISSUER,
                    
                    ValidateAudience = true,
                    ValidAudience = AuthOptions.AUDIENCE,
                    ValidateLifetime = true,
                    
                    IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true
                }
            });

            app.UseMvc();
        }
    }
}
