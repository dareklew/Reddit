using Microsoft.Extensions.Options;
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
        public class RedditTokenApiClient : IRedditTokenApiClient
        {
            private readonly HttpClient _httpClient;
            private readonly ILogger<RedditApiClient> _logger;
            private readonly bool _isConfigured = false;
            private readonly ExternalServicesConfiguration _config;

            public RedditTokenApiClient(
                HttpClient httpClient,
                IOptionsMonitor<ExternalServicesConfiguration> options,
                ILogger<RedditApiClient> logger)
            {
                _config = options.Get(ExternalServicesConfiguration.RedditTokenApi);


                _logger = logger;


                var url = _config.Url;

                if (!string.IsNullOrEmpty(url))
                {
                    _isConfigured = true;
                    httpClient.BaseAddress = new Uri(url);
                }
                _httpClient = httpClient;

            }

            public async Task<RedditToken?> PostAccessTokenAsync(CancellationToken cancellationToken = default)
            {
                if (!_isConfigured)
                {
                    _logger.LogWarning("Reddit API client not configured");
                    return null;
                }

                var path = $"api/v1/access_token";                

                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
                //_httpClient.DefaultRequestHeaders.Add("User-Agent", "dotnet:mcbrowser:v1.0");
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "PostmanRuntime/7.37.3");


                var byteArray = Encoding.ASCII.GetBytes(_config.AppId + ":" + _config.Secret);
                var header = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
                _httpClient.DefaultRequestHeaders.Authorization = header;


                try
                {
                    var contentList = new List<string>();
                    contentList.Add($"grant_type={Uri.EscapeDataString(_config.GrantType)}");
                    contentList.Add($"username={Uri.EscapeDataString(_config.Username)}");
                    contentList.Add($"password={Uri.EscapeDataString(_config.Password)}");
                    var content = new StringContent(string.Join("&", contentList));
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var response = await _httpClient.PostAsync(path, content, cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("Failed to post data url {0},  StatusCode: {1}", path.ToString(), response.StatusCode);
                        return null;
                    }

                    JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true,

                    };
                    return await response.Content.ReadFromJsonAsync<RedditToken>(jsonOptions, cancellationToken);
                }
                catch (HttpRequestException e)
                {
                    _logger.LogError(e, "Failed to get access_token data from API, Error: {0}", e.Message);
                }
                return null;
            }


        }
}



