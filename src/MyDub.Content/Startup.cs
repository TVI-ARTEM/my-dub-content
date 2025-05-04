using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using MyDub.Content.Extensions;
using MyDub.Content.Middlewares;

namespace MyDub.Content;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddControllers(options => { options.Filters.Add<HttpResponseExceptionFilter>(); })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });



        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddCors(
            o => o
                .AddPolicy("MyCors",
                    policy => policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()));

        services.AddDal(configuration);

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.GetSection("JWTOptions:Issuer").Value,
                    ValidAudience = configuration.GetSection("JWTOptions:Audience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration.GetSection("JWTOptions:SecretKey").Value!))
                };
            });

        services.AddAuthorization();
    }

    public void Configure(
        IHostEnvironment environment,
        IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.All });
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors("MyCors");

        app.UseEndpoints(o => { o.MapControllers(); });
    }
}