using Amazon.S3;
using Microsoft.Extensions.Options;
using MyDub.Content.Configure;
using MyDub.Content.Repositories;

namespace MyDub.Content.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDal(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<S3Settings>(config.GetSection("S3"));

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var s3Settings = sp.GetRequiredService<IOptions<S3Settings>>().Value;
            var s3Config = new AmazonS3Config
            {
                ServiceURL = s3Settings.ServiceURL,
                ForcePathStyle = true,
                UseHttp = !s3Settings.UseSSL
            };
            return new AmazonS3Client(s3Settings.AccessKey, s3Settings.SecretKey, s3Config);
        });

        services.AddRepositories();

        return services;
    }


    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IS3Repository, S3Repository>();

        return services;
    }
}