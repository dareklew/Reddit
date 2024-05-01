using Microsoft.Extensions.Options;
using Reddit.API.Configuration;
using Reddit.API.Models;
using Reddit.API.Respositories;

using System.Diagnostics;

namespace Reddit.API.Services
{
    public class RedditProcessorService : BackgroundService    
    {
        private readonly IRedditApiClient _redditApiClient;
        private readonly IRedditTokenApiClient _redditTokenApiClient;
        private readonly ILogger<RedditProcessorService> _logger;
        private readonly IRedditRepository _redditRepository;
        private readonly IServiceProvider _serviceProvider;


        private readonly int _minInterval; //how frequently to query subreddits
        private readonly string _subreddit;

        public RedditProcessorService(
                IRedditApiClient redditApiClient,
                IRedditTokenApiClient redditTokenApiClient,
                IOptionsMonitor<ExternalServicesConfiguration> options,                
                IServiceProvider serviceProvider,

                ILogger<RedditProcessorService> logger)
        {
            _redditApiClient = redditApiClient;
            _redditTokenApiClient = redditTokenApiClient;
            _logger = logger;
            _minInterval = options.Get(ExternalServicesConfiguration.RedditApi).MinInterval;
            _subreddit = options.Get(ExternalServicesConfiguration.RedditApi).DefaultSubreddit;            
            _serviceProvider = serviceProvider;


        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int totalCount = 0;

            using var scope = _serviceProvider.CreateScope();
            var redditRepository = scope.ServiceProvider.GetRequiredService<IRedditRepository>();

            var result = await _redditTokenApiClient
                .PostAccessTokenAsync( stoppingToken );

            long delay = _minInterval;

            string accessToken = string.Empty;
            if( result is not null)
            {
                accessToken = result.access_token;
                _logger.LogDebug("PostAccessTokenAsync returned access token");
            }

            Stopwatch sw = new Stopwatch();


            var req = new SubredditRequest()
            {
                Subreddit = _subreddit,
                Limit = 100,
                Direction = "after",
                DirectionValue = null
            };


            while (!stoppingToken.IsCancellationRequested)
            {

                sw.Restart();

                var resultInfo = await _redditApiClient
                    .GetSubredditListAsync(req, accessToken, stoppingToken);

                if (resultInfo is not null && resultInfo.Result is not null)
                {
                    totalCount++;

                    //how many subreddits
                    int subredditCount = resultInfo.Result.Data.Children.Count;
                    _logger.LogInformation("Updating subreddits Total Count={0}.", subredditCount);

                    if(!string.IsNullOrEmpty(resultInfo.Result.Data.After))
                    {
                        req.DirectionValue = resultInfo.Result.Data.After;
                    } 
                    else
                    {
                        //go to the top listing
                        req.DirectionValue = null;
                    }

                    //update storage
                    foreach (var p in resultInfo.Result.Data.Children) {
                        var model = new SubredditModel()
                        {
                            PostId = p.Data.Id,
                            Title = p.Data.Title,
                            Ups = p.Data.Ups,
                            Subreddit = p.Data.Subreddit,
                            Author = p.Data.Author
                        };
                        await redditRepository.UpsertPost(model);
                    }


                    long calcDelay = resultInfo.RateLimitReset * 1000/(long)resultInfo.RateLimitRemaining;
                    _logger.LogDebug("LimitRemaining={0},LimitUsed={1},LimitReset={2}, CalcDelay={3} ms, Exec={4} ms",
                        resultInfo.RateLimitRemaining,
                        resultInfo.RateLimitUsed,
                        resultInfo.RateLimitReset,
                        calcDelay,
                        sw.ElapsedMilliseconds
                        );

                    
                    delay = Math.Max(_minInterval, calcDelay);
                }

                
                await Task.Delay(TimeSpan.FromMilliseconds(delay), stoppingToken);
            }

            sw.Stop();
        }
    }
}
