using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Polly;
using Polly.Extensions.Http;
using ProjectmgmtProducerApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using ProjectmgmtProducerApi.API;
using ProjectmgmtProducerApi.Services;

namespace ProjectmgmtProducerApi
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
            //OWSAP Broken Authentication policy
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 6;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            });



            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromHours(1);
                options.SlidingExpiration = true;
            });
            
            
            services.AddHttpClient("errorApiClient", c =>
            {
                c.BaseAddress = new Uri("http://localhost:29155/");
            }).AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(2, TimeSpan.FromSeconds(30)));

        //var circuitBreakerPolicy = GetCircuitBreakerPolicy();
            services.AddControllers();
            // services.AddSwaggerForOcelot(Configuration);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ProjectMgmtAPI", Version = "v1" });
                c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    
                    In = ParameterLocation.Header,
                    Description = "Basic Authorization header using the Bearer scheme."
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="basic",
                            }
                        },
                        new string[]{}
                    }
                });
            });
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            services.AddScoped<IUserService, UserService>();
            services.AddOcelot();
            services.AddCors(options=>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()             
                    .AllowAnyHeader()
                    );
            }
            );
            services.AddSingleton<ICosmosDBService>(InitializeCosmosClientInstanceAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
            //services.AddPolicyHandler
        }


        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // app.UseSwagger();
            //app.UseSwaggerForOcelotUI(opt =>
            //{
            //    opt.PathToSwaggerGenerator = "/swagger/docs";
            //});
            app.UseCors("CorsPolicy");
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProjectMgmtTrackerApi v1"));

            app.UseHttpsRedirection();
            app.UseRouting();
           
            
            //OWSAP - Set X-Xss-Protection at code level
            //app.Use(async (context, next) =>
            //{
            //    context.Response.Headers.Add("X-Xss-Protection", "1");
            //    await next();
            //});
           
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            await app.UseOcelot();
        }

        public static async Task<CosmosDBService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection["DatabaseName"];
            var containerName = configurationSection["ContainerName"];
            var account = configurationSection["Account"];
            var key = configurationSection["Key"];

          

            var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            string keyPath = await GetPartitionKey(database, containerName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/partitionKey");
            var cosmosDbService = new CosmosDBService(client, databaseName, containerName);
            return cosmosDbService;
        }

        private static async Task<string> GetPartitionKey(Database database, string containerName)
        {
            var query = new QueryDefinition("select * from c where c.id = @id")
                .WithParameter("@id", containerName);
            using var iterator = database.GetContainerQueryIterator<ContainerProperties>(query);

            while (iterator.HasMoreResults)
            {
                foreach (var container in await iterator.ReadNextAsync())
                {
                    return container.PartitionKeyPath;
                }
            }

            return null;
        }
    }
}
