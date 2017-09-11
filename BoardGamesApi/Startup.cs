using System;
using System.IO;
using System.Text;
using BoardGamesApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BoardGamesApi
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
            services.AddSingleton<IGamesRepository, GamesRepository>();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(
                    options =>
                    {
                        var tokenValidationParameters = new TokenValidationParameters
                        {
                            ValidIssuer = Configuration["Tokens:Issuer"],
                            ValidAudience = Configuration["Tokens:Issuer"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                        };

                        options.RequireHttpsMetadata = false; // Just for testing
                        options.TokenValidationParameters = tokenValidationParameters;
                    });

            services.AddMvcCore().AddVersionedApiExplorer(o => o.GroupNameFormat = "'v'VVV");
            services.AddMvc();

            services.AddApiVersioning(c =>
            {
                c.AssumeDefaultVersionWhenUnspecified = true;
                c.ReportApiVersions = true;
            });

            services.AddSwaggerGen(c =>
            {
                // resolve the IApiVersionDescriptionProvider service
                // note: that we have to build a temporary service provider here because one has not been created yet
                var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

                // add a swagger document for each discovered API version
                // note: you might choose to skip or document deprecated API versions differently
                foreach (var description in provider.ApiVersionDescriptions.Distinct((x, y) => x.GroupName == y.GroupName))
                {
                    c.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                }

                // add a custom operation filter which sets default values
                c.OperationFilter<SwaggerDefaultValues>();

                c.OperationFilter<AuthResponsesOperationFilter>();

                var xmlDocumentationPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "BoardGamesApi.xml");
                c.IncludeXmlComments(xmlDocumentationPath);

                c.AddSecurityDefinition("bearer-token", new ApiKeyScheme
                {
                    Name = "Authorization",
                    In = "header"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseAuthentication();

            //app.UseMiddleware<LimitingMiddleware>();

            app.UseMvc();

            app.UseSwagger(); // http://localhost:49194/swagger/v1/swagger.json
            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions.Distinct((x, y) => x.GroupName == y.GroupName))
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName);
                    }
                });
        }

        private static Info CreateInfoForApiVersion(ApiVersionDescription description)
        {
            var info = new Info
            {
                Title = $"Board Games API {description.GroupName}",
                Version = description.GroupName,
                Description = "A sample API for presentation purpose",
                TermsOfService = "Do whatever you like with it",
                Contact = new Contact
                {
                    Name = "Miroslav Popovic",
                    Url = "https://miroslavpopovic.com"
                },
                License = new License
                {
                    Name = "MIT",
                    Url = "https://opensource.org/licenses/MIT"
                }
            };

            if (description.IsDeprecated)
            {
                info.Description += " This API version has been deprecated.";
            }

            return info;
        }
    }
}
