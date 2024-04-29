using Reddit.API.Models;

namespace Reddit.API.Services
{
    public interface IRedditApiClient
    {
        //Task<RedditToken?> PostAccessTokenAsync(CancellationToken cancellationToken = default);

        Task<RedditApiResultInfo?> GetSubredditLatestAsync(SubredditRequest request, CancellationToken cancellationToken = default);
        Task<RedditApiResultInfo?> GetSubredditListAsync(SubredditRequest request, string accessToken, CancellationToken cancellationToken = default);

    }

}


