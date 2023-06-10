using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogLab.Web.Controllers;
using BookApiProject.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace BookApiProject
{
  public class Startup
  {
    public static IConfiguration Configuration { get; set; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc()
              .AddNewtonsoftJson(o => o.SerializerSettings.ReferenceLoopHandling =
                              Newtonsoft.Json.ReferenceLoopHandling.Ignore);

      var connectionString = Configuration["connectionStrings:bookDbConnectionString"];
      services.AddDbContext<BookDbContext>(c => c.UseSqlServer(connectionString));

      services.AddScoped<ICountryRepository, CountryRepository>();
      services.AddScoped<ICategoryRepository, CategoryRepository>();
      services.AddScoped<IReviewerRepository, ReviewerRepository>();
      services.AddScoped<IReviewRepository, ReviewRepository>();
      services.AddScoped<IAuthorRepository, AuthorRepository>();
      services.AddScoped<IBookRepository, BookRepository>();


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Book API", Version = "v1" });
                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme.",
                });
                c.OperationFilter<AuthOperationFilter>();
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, BookDbContext context)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

            //context.SeedDataContext();
            app.UseSwagger();
            //  app.UseSwaggerUI();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                options.RoutePrefix = "Swagger"; // string.Empty;  // "launchUrl": "swagger",   aaaa
            });
            app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller}/{action}/{id?}");
      });
    }
  }
}
