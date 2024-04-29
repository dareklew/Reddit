using Reddit.API.Models;

namespace Reddit.API.Services
{
    public interface IRedditTokenApiClient
    {
        Task<RedditToken?> PostAccessTokenAsync(CancellationToken cancellationToken = default);
    }
}
