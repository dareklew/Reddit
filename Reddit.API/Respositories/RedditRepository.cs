using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Reddit.API.Models;
using Reddit.Contracts.Entities;
using Reddit.DAL;

namespace Reddit.API.Respositories
{
    public class RedditRepository : IRedditRepository
    {
        private readonly RedditDbContext _dbContext;
        public RedditRepository( RedditDbContext dbContext) { 
            _dbContext = dbContext;
        }

        public async Task<bool> UpsertPost(SubredditModel model )
        {
            //map to the entities by using AutoMapper

            var oldPost = _dbContext.Posts.Where( p => p.PostId == model.PostId).FirstOrDefault();
            if (oldPost != null)
            {
                oldPost.UserId = model.UserId;
                oldPost.Title = model.Title;
                oldPost.Ups = model.Ups;
                
                _dbContext.Posts.Attach(oldPost);
                _dbContext.Entry(oldPost).State = EntityState.Modified;
            }
            else
            {
                var post = new Post();
                post.PostId = model.PostId;
                post.UserId = model.UserId;
                post.Title = model.Title;
                post.Author = model.Author;
                post.Ups = model.Ups;
                
                post.Subreddit = model.Subreddit;

                
                _dbContext.Posts.Add(post);
            }
            return await _dbContext.SaveChangesAsync() > 0;

        }

        public async Task<IEnumerable<Post>> GetPostsAsync()
        {
            return await _dbContext.Posts                
                .ToListAsync();
        }

    }
}
