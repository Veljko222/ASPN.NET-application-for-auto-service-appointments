using AutoService.Application.Auth;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace AutoService.Web.Auth
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationSession _applicationSession;

        public JwtTokenService(
            IConfiguration configuration,
            ApplicationSession applicationSession)
        {
            _configuration = configuration;
            _applicationSession = applicationSession;
        }

        public string CreateToken(
            int userId,
            string userName,
            string email,
            string role)
        {
            var header = new Dictionary<string, object>
            {
                ["alg"] = "HS256",
                ["typ"] = "JWT"
            };

            var expires = DateTimeOffset.UtcNow
                .AddMinutes(GetExpirationMinutes())
                .ToUnixTimeSeconds();

            var payload = new Dictionary<string, object>
            {
                ["sub"] = userId.ToString(),
                ["name"] = userName,
                ["email"] = email,
                ["role"] = role,
                ["sid"] = _applicationSession.Id,
                ["exp"] = expires
            };

            string encodedHeader = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(header));
            string encodedPayload = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(payload));
            string unsignedToken = $"{encodedHeader}.{encodedPayload}";
            string signature = CreateSignature(unsignedToken);

            return $"{unsignedToken}.{signature}";
        }

        private string CreateSignature(string unsignedToken)
        {
            byte[] key = Encoding.UTF8.GetBytes(GetSecret());
            byte[] data = Encoding.UTF8.GetBytes(unsignedToken);

            using var hmac = new HMACSHA256(key);
            return Base64UrlEncode(hmac.ComputeHash(data));
        }

        private string GetSecret()
        {
            return _configuration["Jwt:Secret"]
                ?? throw new InvalidOperationException("Jwt:Secret nije podeÅ¡en.");
        }

        private int GetExpirationMinutes()
        {
            string? value = _configuration["Jwt:ExpirationMinutes"];

            return int.TryParse(value, out int minutes)
                ? minutes
                : 120;
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}

