namespace UserService.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        string GenerateRefreshToken(int userId);
        int? ValidateRefreshToken(string token);
        void RevokeRefreshToken(string token);
    }
}
