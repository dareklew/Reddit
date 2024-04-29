namespace Reddit.API.Models
{
    public class RedditToken
    {
        public string access_token { get; set; } = string.Empty;
        public string token_type { get; set; } = string.Empty;
        public int expires_in { get; set; }
        public string scope { get; set; } = string.Empty;
        public string refresh_token { get; set; } = string.Empty;

        // If error has a non-empty value, nothing else was captured
        public string error { get; set; } = string.Empty;
    }
}
