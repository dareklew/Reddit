using Reddit.API.Models;
using Reddit.Contracts.Entities;

namespace Reddit.API.Respositories
{
    public interface IRedditRepository
    {
        Task<bool> UpsertPost(SubredditModel model);
        Task<IEnumerable<Post>> GetPostsAsync();
    }
}
