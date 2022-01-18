using System;
using System.Linq;
using DataLayer.Entities;

namespace DataLayer.Repositories
{
    public interface ITokenRepository : IRepositoryBase<Token>
    {
        Token CacheGetAccessTokenByTokenString(string tokenString);
        Token CacheGetRefreshTokenByTokenString(string token);
        Token CacheGetAccessTokenByUserId(Guid id);
    }

    public class TokenRepository : RepositoryBase<Token>, ITokenRepository
    {
        public TokenRepository(ApplicationDbContext db) : base(db)
        {
        }

        public Token CacheGetAccessTokenByTokenString(string tokenString)
        {
            return DbGetRecords()
                .Where(t => t.Type == TokenTypes.AccessToken)
                .FirstOrDefault(t => t.TokenString == tokenString);
        }

        public Token CacheGetRefreshTokenByTokenString(string token)
        {
            return DbGetRecords()
                .Where(t => t.Type == TokenTypes.RefreshToken)
                .FirstOrDefault(t => t.TokenString == token);
        }

        public Token CacheGetAccessTokenByUserId(Guid id)
        {
            return DbGetRecords()
                .Where(t => t.AppUserId == id)
                .FirstOrDefault(t => t.Type == TokenTypes.AccessToken);
        }
    }
}
