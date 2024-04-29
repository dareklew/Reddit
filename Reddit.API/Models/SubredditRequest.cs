namespace Reddit.API.Models
{
    public class SubredditRequest
    {
        public SubredditRequest()
        {
            Limit = 0;
        }

        public string? Subreddit { get; set; }
        public string? Direction { get; set; }
        public string? DirectionValue { get; set; }        
        public int Limit { get; set; }

    }
}
