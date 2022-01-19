using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public interface IAuthorRepository : IRepositoryBase<Author>
    {
        Author GetByUserId(Guid userId);
        Author GetAuthorByToken(string token);
    }
    public class AuthorRepository : RepositoryBase<Author>, IAuthorRepository
    {
        private readonly ApplicationDbContext _db;
        public AuthorRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }

        public Author GetByUserId(Guid userId)
        {
            return DbGetRecords().Include(a => a.User).FirstOrDefault(a => a.UserId == userId);
        }

        public Author GetAuthorByToken(string token)
        {
            return DbGetRecords()
                .Include(a => a.User)
                    .ThenInclude(u => u.Tokens)
                .FirstOrDefault(a => a.User.Tokens.Any(t => t.Type == TokenTypes.AccessToken && t.TokenString == token));
        }
    }
}
