using StackExchange.Redis;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using UserService.Application.Interfaces;

public class RedisRefreshTokenService : IRefreshTokenService
{
    private readonly IDatabase _redis;
    private readonly IConfiguration _config;

    public RedisRefreshTokenService(IConnectionMultiplexer redis, IConfiguration config)
    {
        _redis = redis.GetDatabase();
        _config = config;
    }

    public string GenerateRefreshToken(int userId)
    {
        var token = Guid.NewGuid().ToString();
        var days = int.Parse(_config["RefreshTokenSettings:ExpirationDays"] ?? "7");

        var data = new { UserId = userId, Exp = DateTime.UtcNow.AddDays(days) };

        _redis.StringSet($"refresh:{token}", JsonConvert.SerializeObject(data), TimeSpan.FromDays(days));

        return token;
    }

    public int? ValidateRefreshToken(string token)
    {
        var value = _redis.StringGet($"refresh:{token}");
        if (value.IsNullOrEmpty) return null;

        var data = JsonConvert.DeserializeObject<dynamic>(value!);

        if (data == null) return null;

        DateTime exp = data.Exp;

        if (exp < DateTime.UtcNow) return null;

        return Convert.ToInt32(data.UserId);
    }

    public void RevokeRefreshToken(string token)
    {
        _redis.KeyDelete($"refresh:{token}");
    }
}
