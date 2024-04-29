namespace Reddit.API.Configuration
{
    public class ExternalServicesConfiguration
    {
        public const string RedditApi = "RedditApi";
        public const string RedditTokenApi = "RedditTokenApi";

        public string Url { get; set; } = string.Empty;
        
        public string DefaultSubreddit { get; set; } = string.Empty;
        public int MinInterval { get; set; } = 10;
        public string ApiKey { get; set; } = string.Empty;

        public string AppId { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;

        public string GrantType { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;


    }
}
