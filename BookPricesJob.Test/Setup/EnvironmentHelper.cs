namespace BookPricesJob.Test.Setup;

public static class EnvironmentHelper
{
    public static void SetNecessaryEnvironmentVariables()
    {
        // Values don't matter for testing since JWT is not used
        Environment.SetEnvironmentVariable(Data.Constant.JwtIssuer, "localhost");
        Environment.SetEnvironmentVariable(Data.Constant.JwtAudience, "localhost");
        Environment.SetEnvironmentVariable(Data.Constant.JwtSigningKey, "THISISASECRETKEY");
    }
}
