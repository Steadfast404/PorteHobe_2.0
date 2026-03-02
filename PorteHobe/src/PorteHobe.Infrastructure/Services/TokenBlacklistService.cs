using System.Collections.Concurrent;

namespace Portehobe.Infrastructure.Services
{
    public class TokenBlacklistService : ITokenBlacklistService
    {
        private readonly ConcurrentDictionary<string, byte> _blacklistedTokens = new();

        public void BlacklistToken(string token)
        {
            _blacklistedTokens.TryAdd(token, 0);
        }

        public bool IsTokenBlacklisted(string token)
        {
            return _blacklistedTokens.ContainsKey(token);
        }
    }
}
