namespace Reddit.API.Models
{
    public class DataInner
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Ups { get; set; }
        public string Subreddit { get; set; } = string.Empty;
        public string? Author { get; set; } = string.Empty;
    }

    public class Child
    {
        public DataInner Data { get; set; }
    }

    public class DataOuter
    {
        public string? After { get; set; }
        public string? Before { get; set; }
        public int Dist { get; set; }
        public List<Child> Children { get; set; }
        
    }

    public class RedditApiResultInfo
    {
        public RedditApiResult? Result { get; set; }
        public int RateLimitUsed { get; set; }
        public int RateLimitReset { get; set; }
        public double RateLimitRemaining { get; set; }

    }

    public class RedditApiResult
    {
        public DataOuter Data { get; set; }

    }
}
