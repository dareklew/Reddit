namespace Reddit.API.Models
{
    public class RedditRequestModel
    {
        public string AccessToken { get; set; } = string.Empty;
        public string Subreddit { get; set; } = string.Empty;
    }
}
