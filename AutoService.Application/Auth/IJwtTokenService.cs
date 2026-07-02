namespace AutoService.Application.Auth
{
    public interface IJwtTokenService
    {
        string CreateToken(
            int userId,
            string userName,
            string email,
            string role);
    }
}

