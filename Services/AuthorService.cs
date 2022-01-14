using DataLayer;
using DataLayer.Entities;
using DataLayer.Repositories;
using System;
using System.Threading.Tasks;

namespace Services.Authors
{
    public interface IAuthorService
    {
        Task<Author> GetAuthorByIdAsync(Guid id);
        Author GetByUserId(Guid userId);
    }

    public class AuthorService : IAuthorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Author> GetAuthorByIdAsync(Guid id)
        {
            return await _unitOfWork.Authors.DbGetByIdAsync(id);
        }

        public Author GetByUserId(Guid userId)
        {
            return _unitOfWork.Authors.GetByUserId(userId);
        }

    }
}
