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
                case "RedddiApi":
                    if (string.IsNullOrEmpty(options.Url))
                    {
                        return ValidateOptionsResult.Fail("A URL for the Reddit " +
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
