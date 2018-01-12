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
using GraphQL.Utilities;
using GraphQL.Types;

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

                    _.Events.InlineProjections.AggregateStreamsWith<Household.Events.Household>();
                });
                return store;
            });

            services.AddMvc();

            services.AddSingleton<GraphQL.Query>();
            services.AddSingleton<GraphQL.Mutation>();
            services.AddSingleton<ISchema>(sp => GraphQL.GraphQLController.BuildSchema(sp));
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
            app.UseMvcWithDefaultRoute();

            var schema = app.ApplicationServices.GetService<ISchema>();
            var output = new SchemaPrinter(schema).Print();
            Console.WriteLine(output);
        }
    }
}
