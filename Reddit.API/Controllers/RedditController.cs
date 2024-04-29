using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Reddit.API.Models;
using Reddit.API.Respositories;
using Reddit.API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Reddit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RedditController : ControllerBase
    {

        private readonly ILogger<RedditController> _logger;

        private readonly IRedditApiClient _apiClient;
        private readonly IRedditTokenApiClient _tokenApiClient;

        private readonly IRedditRepository _redditRepository;

        public RedditController(
            IRedditApiClient apiClient,
            IRedditTokenApiClient tokenApiClient,
            IRedditRepository redditRepository,
            ILogger<RedditController> logger)
        {
            _logger = logger;
            _apiClient = apiClient;
            _tokenApiClient = tokenApiClient;
            _redditRepository = redditRepository;
        }


        [HttpGet()]
        [SwaggerOperation("GetPostStat")]
        [Route("stat/ups")]
        public async Task<ActionResult<RedditStatAuthorsViewModel>> GetPostStat()
        {
            var query = await _redditRepository.GetPostsAsync();

            //find with largest ups
            var response = query.OrderByDescending(p => p.Ups)
                .Select( p => 
                    new RedditStatUpsViewModel()
                    {                         
                        Author =p.Author,
                        Subreddit = p.Subreddit,
                        Ups = p.Ups,
                        Title = p.Title
                    }
                );           
            
            

            return Ok( response );
        }

        [HttpGet()]
        [SwaggerOperation("GetPostStatUsers")]
        [Route("stat/users")]
        public async Task<ActionResult<RedditStatAuthorsViewModel>> GetPostStatUsers()
        {
            var query = await _redditRepository.GetPostsAsync();

            //find with largest number of posts
            var response = query.GroupBy(
                    p => p.Author,
                    p => p,
                    (key, g) => new
                    {
                        Author = key,
                        Posts = g.ToList().Where(b => b.Author == key)
                    })
                .ToDictionary(x => x.Author, y => y.Posts.Count())
                .OrderByDescending( z => z.Value)
                .Select(p =>
                    new RedditStatAuthorsViewModel()
                    {
                        Author = p.Key,
                        Count = p.Value                        
                    }
                );



            return Ok(response);
        }


        [HttpPost()]
        [SwaggerOperation("GetRedditList")]
        [Route("list")]
        public async Task<ActionResult<RedditApiResultInfo>> GetRedditList(RedditRequestModel model)
        {
            var request = new SubredditRequest()
            {
                Subreddit = model.Subreddit
            };

            var result = await _apiClient.GetSubredditListAsync(request, model.AccessToken);

            return Ok(result);
        }

        [HttpPost()]
        [Route("token")]        
        [SwaggerOperation("GetAccessToken")]
        public async Task<ActionResult<RedditToken>> GetAccessToken()
        {
            string accessToken = string.Empty;

            var result = await _tokenApiClient.PostAccessTokenAsync();

            return Ok(result);
        }

    }
}
