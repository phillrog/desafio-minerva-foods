using DesafioMinervaFoods.Application.Interfaces;
using DesafioMinervaFoods.Infrastructure.Identity;
using DesafioMinervaFoods.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DesafioMinervaFoods.Infrastructure.Configs
{
    public static class AuthConfig
    {
        public static IServiceCollection AddAuthJWTConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // Registro do Contexto de Autenticação
            services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Identity Config 
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // RELAXANDO PARA APRESENTAÇÂO
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4; // Aceita 4 caracteres
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<AuthDbContext>()
            .AddDefaultTokenProviders();

            // JWT Config
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Secret"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                x.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {   
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsJsonAsync(new { message = "O usuário não enviou um token ou o token é inválido." });
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";
                        return context.Response.WriteAsJsonAsync(new { message = "O usuário está autenticado, mas não tem permissão \"Admin\"" });
                    }
                };
            });


            // Services
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }

    }
}
