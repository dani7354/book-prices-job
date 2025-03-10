namespace BookPricesJob.API;

public static class Constant
{
    public const string ClaimRoleType = "role";

    // Access Policies
    public const string JobManagerPolicy = "JobManager";
    public const string JobRunnerPolicy = "JobRunner";
    public const string JobManagerClaim = "JobManager";
    public const string JobRunnerClaim = "JobRunner";

    // Configuration
    public const string AllowNewUsers = "ALLOW_NEW_USERS";
    
    // JWT
    public const int JwtSigningKeyByteCount = 64;
    public const string JwtIssuer = "localhost";
    public const string JwtAudience = "BookPricesJob";
}
