using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace AutoService.Web.Auth
{
    public class JwtAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "JwtCookie";
        public const string CookieName = "jwt_token";

        private readonly IConfiguration _configuration;
        private readonly ApplicationSession _applicationSession;

        public JwtAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration,
            ApplicationSession applicationSession)
            : base(options, logger, encoder)
        {
            _configuration = configuration;
            _applicationSession = applicationSession;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? token = GetToken();

            if (string.IsNullOrWhiteSpace(token))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            try
            {
                var claims = ValidateToken(token);
                var identity = new ClaimsIdentity(claims, SchemeName);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, SchemeName);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch
            {
                return Task.FromResult(AuthenticateResult.Fail("JWT token nije validan."));
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Redirect("/Auth/Login");
            return Task.CompletedTask;
        }

        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.Redirect("/Auth/Login");
            return Task.CompletedTask;
        }

        private string? GetToken()
        {
            string? authorization = Request.Headers.Authorization;

            if (!string.IsNullOrWhiteSpace(authorization) &&
                authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authorization["Bearer ".Length..].Trim();
            }

            return Request.Cookies[CookieName];
        }

        private List<Claim> ValidateToken(string token)
        {
            string[] parts = token.Split('.');

            if (parts.Length != 3)
            {
                throw new InvalidOperationException("Neispravan JWT format.");
            }

            string unsignedToken = $"{parts[0]}.{parts[1]}";
            string expectedSignature = CreateSignature(unsignedToken);

            if (!FixedEquals(parts[2], expectedSignature))
            {
                throw new InvalidOperationException("Neispravan JWT potpis.");
            }

            byte[] payloadBytes = Base64UrlDecode(parts[1]);
            using var document = JsonDocument.Parse(payloadBytes);
            var root = document.RootElement;

            long exp = root.GetProperty("exp").GetInt64();

            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > exp)
            {
                throw new InvalidOperationException("JWT token je istekao.");
            }

            string? tokenSessionId = root.TryGetProperty("sid", out var sid)
                ? sid.GetString()
                : null;

            if (tokenSessionId != _applicationSession.Id)
            {
                throw new InvalidOperationException("JWT token nije iz trenutnog pokretanja aplikacije.");
            }

            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, root.GetProperty("sub").GetString() ?? string.Empty),
                new Claim(ClaimTypes.Name, root.GetProperty("name").GetString() ?? string.Empty),
                new Claim(ClaimTypes.Email, root.GetProperty("email").GetString() ?? string.Empty),
                new Claim(ClaimTypes.Role, root.GetProperty("role").GetString() ?? string.Empty)
            };
        }

        private string CreateSignature(string unsignedToken)
        {
            string secret = _configuration["Jwt:Secret"]
                ?? throw new InvalidOperationException("Jwt:Secret nije podeÅ¡en.");

            byte[] key = Encoding.UTF8.GetBytes(secret);
            byte[] data = Encoding.UTF8.GetBytes(unsignedToken);

            using var hmac = new HMACSHA256(key);
            return Base64UrlEncode(hmac.ComputeHash(data));
        }

        private static bool FixedEquals(string value, string expected)
        {
            byte[] valueBytes = Encoding.UTF8.GetBytes(value);
            byte[] expectedBytes = Encoding.UTF8.GetBytes(expected);

            return valueBytes.Length == expectedBytes.Length &&
                   CryptographicOperations.FixedTimeEquals(valueBytes, expectedBytes);
        }

        private static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static byte[] Base64UrlDecode(string value)
        {
            string base64 = value
                .Replace('-', '+')
                .Replace('_', '/');

            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}

