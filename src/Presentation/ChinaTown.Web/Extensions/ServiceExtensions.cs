using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using System.Text;
using AutoMapper;
using ChinaTown.Application.Data;
using ChinaTown.Application.Mappers;
using ChinaTown.Application.Models;
using ChinaTown.Application.Services;
using ChinaTown.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ChinaTown.Web.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddSingleton<MongoDbContext>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAvatarService, AvatarService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IArticleService, ArticleService>();
        services.AddScoped<IArticleTypeService, ArticleTypeService>();
        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<ILikeService, LikeService>();
        services.AddScoped<IRegionService, RegionService>();
        services.AddScoped<IRecipeTypeService, RecipeTypeService>();
        services.AddScoped<ILikeService, LikeService>();
        services.AddScoped<IRecipeService, RecipeService>();
        
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.Configure<MongoDbConfig>(configuration.GetSection("MongoDb"));
        
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ArticleMapper());
            mc.AddProfile(new BookMapper());
            mc.AddProfile(new CommonMapper());
            mc.AddProfile(new UserMapper());
            mc.AddProfile(new RecipeMapper());
            mc.AddProfile(new RegionMapper());
            mc.AddProfile(new RecipeTypeMapper());
        }, LoggerFactory.Create(b => b.AddConsole()));

        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);

        return services;
    }
    
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services, IConfiguration configuration)
    {
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero,
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                    logger.LogError(context.Exception, "Authentication failed");
                    
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.TryAdd("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = "ChinaDown API", 
                Version = "v1"
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            };
        
            c.AddSecurityDefinition("Bearer", securityScheme);
        
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[] { }
                }
            });
        });

        return services;
    }
}