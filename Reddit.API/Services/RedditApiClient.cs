using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Reddit.API.Configuration;
using Reddit.API.Models;
using System;
using System.Collections;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reddit.API.Services
{
    public class RedditApiClient : IRedditApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RedditApiClient> _logger;
        private readonly bool _isConfigured = false;
        private readonly ExternalServicesConfiguration _config;

        const string RateLimitUsed = "x-ratelimit-used";
        const string RateLimitRemaining = "x-ratelimit-remaining";
        const string RateLimitReset = "x-ratelimit-reset";

        public RedditApiClient(
            HttpClient httpClient,
            IOptionsMonitor<ExternalServicesConfiguration> options,
            ILogger<RedditApiClient> logger)
        {
            _config = options.Get(ExternalServicesConfiguration.RedditApi);

            
            _logger = logger;
            

            var url = _config.Url;

            if (!string.IsNullOrEmpty(url))
            {
                _isConfigured = true;
                httpClient.BaseAddress = new Uri(url);
            }
            _httpClient = httpClient;

        }


        public async Task<RedditApiResultInfo?> GetSubredditLatestAsync(SubredditRequest request, CancellationToken cancellationToken = default)
        {
            if (!_isConfigured)
                return null;

            StringBuilder path = new StringBuilder();
            path.Append($"r/{request.Subreddit}/new");  //get subrredits sorted by the latest

            if (!string.IsNullOrEmpty(request.DirectionValue) || request.Limit != 0) {
                path.Append('?');
                if (request.Limit != 0)
                {
                    path.Append(string.Format("limit={0}", request.Limit));
                }

                if (!string.IsNullOrEmpty(request.DirectionValue))
                {
                    path.Append(string.Format("&{0}={1}", request.Direction, request.DirectionValue));
                }
            }



            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var header = new AuthenticationHeaderValue("bearer", _config.ApiKey); //TODO pass access token
            _httpClient.DefaultRequestHeaders.Authorization = header;


            //_httpClient.DefaultRequestHeaders.Add("User-Agent", "dotnet:mcbrowser:v1.0");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.37.3");



            try
            {
                var response = await _httpClient.GetAsync(path.ToString(), cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to get data url {0},  StatusCode: {1}", path.ToString(), response.StatusCode);
                    return null;
                }


                //read rate parameters
                string rateLimitUsed = response.Headers
                    .GetValues(RateLimitUsed)
                    .First();

                string rateLimitRemaining = response.Headers
                    .GetValues(RateLimitRemaining)
                    .First();

                string rateLimitReset = response.Headers
                    .GetValues(RateLimitReset)
                    .First();


                JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
                {
                     PropertyNameCaseInsensitive = true,
                     
                };

                var content = await response.Content.ReadFromJsonAsync<RedditApiResult>(jsonOptions,cancellationToken);

                var info = new RedditApiResultInfo()
                {
                    RateLimitRemaining = double.Parse(rateLimitRemaining),
                    RateLimitReset = int.Parse(rateLimitReset),
                    RateLimitUsed = int.Parse(rateLimitUsed),
                    Result = content
                };

                _logger.LogDebug("GetSubredditLatestAsync completed, LimitRemaining={0},LimitUsed={1},LimitReset={2},StatusCode={3}", 
                    info.RateLimitRemaining,
                    info.RateLimitUsed,
                    info.RateLimitReset,
                    response.StatusCode);

                return info;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Failed to get subbredit={0} data from API", request.Subreddit);
            }

            return null;
        }

        public async Task<RedditApiResultInfo?> GetSubredditListAsync(SubredditRequest request, string accessToken, CancellationToken cancellationToken = default)
        {
            if (!_isConfigured)
                return null;

            StringBuilder path = new StringBuilder();
            path.Append($"r/{request.Subreddit}/new");  //get latest posts for subrredit 

            if (!string.IsNullOrEmpty(request.DirectionValue) || request.Limit != 0)
            {
                path.Append('?');
                if (request.Limit != 0)
                {
                    path.Append(string.Format("limit={0}", request.Limit));
                }

                if (!string.IsNullOrEmpty(request.DirectionValue))
                {
                    path.Append(string.Format("&{0}={1}", request.Direction, request.DirectionValue));
                }
            }
            

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var header = new AuthenticationHeaderValue("bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Authorization = header;
            //_httpClient.DefaultRequestHeaders.Add("User-Agent", "dotnet:mcbrowser:v1.0");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.37.3");

            try
            {
                var response = await _httpClient.GetAsync(path.ToString(), cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to get data url {0},  StatusCode: {1}", path.ToString(), response.StatusCode);
                    return null;
                }
                //read rate parameters
                string rateLimitUsed = response.Headers
                    .GetValues(RateLimitUsed)
                    .First();

                string rateLimitRemaining = response.Headers
                    .GetValues(RateLimitRemaining)
                    .First();

                string rateLimitReset = response.Headers
                    .GetValues(RateLimitReset)
                    .First();
                

                JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,

                };
                var content = await response.Content.ReadFromJsonAsync<RedditApiResult>(jsonOptions, cancellationToken);
                var info = new RedditApiResultInfo()
                {
                    RateLimitRemaining = double.Parse(rateLimitRemaining),
                    RateLimitReset = int.Parse(rateLimitReset),
                    RateLimitUsed = int.Parse(rateLimitUsed),
                    Result = content
                };

                 _logger.LogDebug("GetSubredditLatestAsync completed, LimitRemaining={0},LimitUsed={1},LimitReset={2},StatusCode={3}",
                    info.RateLimitRemaining,
                    info.RateLimitUsed,
                    info.RateLimitReset,
                    response.StatusCode);
                return info;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Failed to get subbredit={0} data from API", request.Subreddit);
            }

            return null;
        }

    }
}

