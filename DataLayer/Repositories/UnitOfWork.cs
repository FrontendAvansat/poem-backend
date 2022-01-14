using System;
using System.Threading.Tasks;

namespace DataLayer.Repositories
{
    public interface IUnitOfWork
    {
        IAppUserRepository Users { get; }
        IAuthorRepository Authors { get; }
       

        Task<bool> SaveChangesAsync();
        public void LogDbTrack();

    }
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public IAppUserRepository Users { get; }
        public IAuthorRepository Authors { get; }
      

        public UnitOfWork(ApplicationDbContext applicationDbContext, 
            IAppUserRepository userRepository, IAuthorRepository authorRepository)
        {
            _applicationDbContext = applicationDbContext;
          
            Users = userRepository;
            Authors = authorRepository;
           
        }

        public void LogDbTrack()
        {
            foreach (var entry in _applicationDbContext.ChangeTracker.Entries()/*.Where(e => e.State == EntityState.Modified)*/)
            {
                Console.WriteLine($"Entity: {entry.Entity.GetType().Name}, State: { entry.State}");
            }
        }
        public async Task<bool> SaveChangesAsync()
        {
            LogDbTrack();
            try
            {
                var save = await _applicationDbContext.SaveChangesAsync();
                if (save <= 0) return false;
                save = _applicationDbContext.SaveChanges();
                return (save >= 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new BadRequestException("CANNOT_UPDATE_DATABASE");
            }

        }
    }
}
