using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Catalog.Settings;
using Catalog.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;

namespace Catalog
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
      var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

      // Define few serialisation rules for mongodb
      BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
      BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

      services.AddSingleton<IMongoClient>(serviceProvider =>
      {
        // Grab mongodb settings section from appsettings.json
        // by using MongoDbSettings class

        return new MongoClient(mongoDbSettings.ConnectionString);
      });

      services.AddSingleton<IItemsRepository, MongoDbItemsReposibtory /* InMemoryItemsRepository */>();

      services.AddControllers(options =>
      {
        // Prevent removing "Async" from method names
        options.SuppressAsyncSuffixInActionNames = false;
      });

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catalog", Version = "v1" });
      });

      services.AddHealthChecks()
        .AddMongoDb(
          mongoDbSettings.ConnectionString,
          name: "mongodb",
          timeout: TimeSpan.FromSeconds(3),
          tags: new[] { "ready" });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog v1"));
      }

      // if ASP will detect that it's running in docker container
      // it will switch to production env
      if (env.IsDevelopment())
      {
        app.UseHttpsRedirection();
      }

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();

        endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
          Predicate = (check) => check.Tags.Contains("ready"),
          ResponseWriter = async (context, report) =>
          {
            string result = JsonSerializer.Serialize(
              new
              {
                status = report.Status.ToString(),
                checks = report.Entries.Select(entry => new
                {
                  name = entry.Key,
                  status = entry.Value.Status.ToString(),
                  exception = entry.Value.Exception != null ? entry.Value.Exception.Message : "none",
                  duration = entry.Value.Duration.ToString()
                })
              }
            );

            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(result);
          }
        });

        endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
        {
          Predicate = (_) => false
        });
      });
    }
  }
}
