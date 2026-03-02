namespace Portehobe.Infrastructure.Services
{
    public interface ITokenBlacklistService
    {
        void BlacklistToken(string token);
        bool IsTokenBlacklisted(string token);
    }
}
