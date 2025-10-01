namespace UserService.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        string GenerateRefreshToken(int userId);
        Guid? ValidateRefreshToken(string token);
        void RevokeRefreshToken(string token);
    }
}
