using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Household.Events;

namespace Household
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddSingleton<IDocumentStore>(c =>
            {
                var store = DocumentStore.For(_ =>
                {
                    var connection = Environment.GetEnvironmentVariable("DATABASE_STR");
                    _.DatabaseSchemaName = "docs";
                    _.Events.DatabaseSchemaName = "events";
                    _.Connection(connection);

                    _.Events.InlineProjections.AggregateStreamsWith<Household>();
                });
                return store;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
            );

            app.UseDefaultFiles();
            app.UseStaticFiles();

            var store = (IDocumentStore)app.ApplicationServices.GetService(typeof(IDocumentStore));

            using (var session = store.OpenSession())
            {
                var household = new Guid("9ed0b4f7-c406-4915-be71-28044785d5a4");
                session.Events.Append(household, new PersonJoinedHousehold
                {
                    Person = new Person
                    {
                      Name = "Joe"
                    }
                });

                session.SaveChanges();
            }
        }
    }
}
