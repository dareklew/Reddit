namespace Reddit.API.Models
{
    public class SubredditModel
    {        
        public string? PostId { get; set; }
        public int Ups { get; set; }
        public string Author { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public string? Subreddit { get; set; }
    }
}
