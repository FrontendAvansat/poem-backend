using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public interface IPoemRepository : IRepositoryBase<Poem>
    {
        public Poem GetByIdWithAuthor(Guid poemId);
        List<Poem> GetAllWithAuthor();
    }


    public class PoemRepository : RepositoryBase<Poem>, IPoemRepository
    {
        private readonly ApplicationDbContext _db;
        public PoemRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }
        public Poem GetByIdWithAuthor(Guid poemId)
        {
            return DbGetRecords()
                .Include(p => p.Author)
                    .ThenInclude(a => a.User)
                .FirstOrDefault(p => p.Id == poemId);
        }

        public List<Poem> GetAllWithAuthor()
        {
            return DbGetRecords()
                .Include(p => p.Author)
                    .ThenInclude(a => a.User).ToList();
         }

    }
}
