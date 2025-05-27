using System.Text.Json;

using Asp.Versioning;

using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using WildlifeTracker.Data;
using WildlifeTracker.Data.Models;
using WildlifeTracker.Data.Repositories;
using WildlifeTracker.Data.Repositories.AspNetCoreTemplate.Data.Repositories;
using WildlifeTracker.Data.Seeding;
using WildlifeTracker.Helpers;
using WildlifeTracker.Middleware;
using WildlifeTracker.Services;
using WildlifeTracker.Services.Data;

namespace WildlifeTracker
{
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers(options =>
            {
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    builder => builder
                        .WithOrigins("http://localhost:50898")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
            });

            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            builder.Services.AddEndpointsApiExplorer();

            // Add Swagger services  
            ConfigureSwagger(builder.Services);

            // Add services to the container.  
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentNullException(null, "Connection string not found in the configuration file")));

            // Reference Token Strategy
            builder.Services.AddAuthentication(IdentityConstants.BearerScheme)
                .AddBearerToken(IdentityConstants.BearerScheme, options =>
                {
                    options.ClaimsIssuer = builder.Configuration["Token:Issuer"]
                        ?? throw new ArgumentNullException(null, "Token issuer not found in the configuration file");
                    options.RefreshTokenExpiration = TimeSpan.FromDays(30);
                    options.BearerTokenExpiration = TimeSpan.FromMinutes(30);
                });

            builder.Services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
            }).AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

            // Use invalidmodelstateresponsefactory to return custom error messages. Find a way to reuse it in the exception handler  

            builder.Services.AddMvc().ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                    ValidationProblemDetailsFactory.CreateInvalidModelStateResponse(actionContext);
            });

            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();

                loggingBuilder.Configure(options =>
                  options.ActivityTrackingOptions =
                      ActivityTrackingOptions.TraceId
                      | ActivityTrackingOptions.SpanId);
            });

            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            builder.Services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IResourceAccessService, ResourceAccessService>();

            builder.Services.AddSingleton(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));
            builder.Services.AddScoped<IAnimalImageService, AnimalImageService>();

            builder.Services.AddSingleton<IOnlineUsersService, OnlineUsersService>();
            builder.Services.AddSingleton<IImageProcessingService, ImageProcessingService>();

            var app = builder.Build();

            app.UseMiddleware<CustomExceptionHandlerMiddleware>();
            app.UseMiddleware<ResponseWrapperMiddleware>();
            app.UseStaticFiles();

            // Ensure database is deleted and created until adding migrations  
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //dbContext.Database.EnsureCreated();
                dbContext.Database.Migrate();
                //dbContext.Database.EnsureDeleted();
                //dbContext.Database.EnsureCreated();

                new ApplicationDbContextSeeder().SeedDatabase(dbContext, scope.ServiceProvider);
            }

            // Configure the HTTP request pipeline.  
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Wildlife Tracker v1");
                    options.InjectJavascript("/swagger/custom-swagger.js");
                });
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.UseMiddleware<UserActivityMiddleware>();

            app.Run();
        }

        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
    }
}
