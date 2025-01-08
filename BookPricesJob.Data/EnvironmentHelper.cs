namespace BookPricesJob.Data;

public static class EnvironmentHelper
{
    public static void LoadEnvFile()
    {
        if (Environment.GetEnvironmentVariable(Constant.ASPNETCORE_ENVIRONMENT) == "Production")
            return;

        var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
        if (File.Exists(envPath))
        {
            File.ReadAllLines(envPath)
                .Select(line => line.Split("="))
                .Where(parts => parts.Length == 2)
                .ToList()
                .ForEach(parts => Environment.SetEnvironmentVariable(parts[0], parts[1]));
        }
    }

    public static string GetConnectionString()
    {
        var mysqlServer = Environment.GetEnvironmentVariable(Constant.MysqlServer) ??
            throw new KeyNotFoundException($"{Constant.MysqlServer} not set!");
        var mysqlDatabase = Environment.GetEnvironmentVariable(Constant.MysqlDatabase) ??
            throw new KeyNotFoundException($"{Constant.MysqlDatabase} not set!");
        var mysqlUser = Environment.GetEnvironmentVariable(Constant.MysqlUser) ??
            throw new KeyNotFoundException($"{Constant.MysqlUser} not set!");
        var mysqlPassword = Environment.GetEnvironmentVariable(Constant.MysqlPassword) ??
            throw new KeyNotFoundException($"{Constant.MysqlPassword} not set!");

        return $"server={mysqlServer}; database={mysqlDatabase}; user={mysqlUser}; password={mysqlPassword}";
    }

    public static string GetRedisConnectionString()
    {
        var redisHost = Environment.GetEnvironmentVariable(Constant.RedisHost);
        var redisPort = Environment.GetEnvironmentVariable(Constant.RedisPort);

        return $"{redisHost}:{redisPort}";
    }
}
