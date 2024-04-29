using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Reddit.API.Respositories;
using Reddit.API.Services;
using Reddit.DAL;

namespace Reddit.API.Configuration
{
    public static class RedditServiceCollectionExtensions
    {
        public static IServiceCollection AddSubredditProcessing(this IServiceCollection services,
            IConfiguration config)
        {
                services.AddHttpClient<IRedditTokenApiClient, RedditTokenApiClient>();
                services.AddHttpClient<IRedditApiClient, RedditApiClient>();
                services.AddTransient<IRedditRepository, RedditRepository>();
                services.AddTransient<RedditDbContext>();

                services.AddHostedService<RedditProcessorService>();

                return services;
        }
    }
}
