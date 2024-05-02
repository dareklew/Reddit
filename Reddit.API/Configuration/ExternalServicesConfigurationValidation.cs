using Microsoft.Extensions.Options;

namespace Reddit.API.Configuration
{
    public class ExternalServicesConfigurationValidation :
       IValidateOptions<ExternalServicesConfiguration>
    {
        private readonly RedditApiConfiguration _externalServiceConfig;

        public ExternalServicesConfigurationValidation(IOptions<RedditApiConfiguration> externalServiceConfig)
        {
            _externalServiceConfig = externalServiceConfig.Value;
        }

        public ValidateOptionsResult Validate(string name,
            ExternalServicesConfiguration options)
        {
            switch (name)
            {
                case "RedditApi":
                    if (string.IsNullOrEmpty(options.Url))
                    {
                        return ValidateOptionsResult.Fail("A URL for the Reddit " +
                            "API is required.");
                    }

                    break;
                case "RedditTokenApi":
                    if (string.IsNullOrEmpty(options.Url))
                    {
                        return ValidateOptionsResult.Fail("A URL for the Reddit Token" +
                            "API is required.");
                    }

                    break;

                default:
                    return ValidateOptionsResult.Skip;
            }

            return ValidateOptionsResult.Success;
        }

    }

}
