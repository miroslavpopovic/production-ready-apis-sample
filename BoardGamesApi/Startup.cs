using System.IO;
using System.Text;
using BoardGamesApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

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

            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Board Games API v1",
                    Version = "v1",
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
                });

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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseAuthentication();

            app.UseMiddleware<LimitingMiddleware>();

            app.UseMvc();

            app.UseSwagger(); // http://localhost:49194/swagger/v1/swagger.json
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Board Games API v1");
            });
        }
    }
}
