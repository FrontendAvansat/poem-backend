using DataLayer;
using DataLayer.Entities;
using DataLayer.Repositories;
using Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IPoemService
    {
        Task<bool> AddPoem(PoemDto poem, Guid userId);
        PoemDto GetPoem(Guid poemId);
        List<PoemDto> GetPoems();
        Task<bool> DeletePoem(Guid poemId);
    }
    public class PoemService : IPoemService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PoemService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddPoem (PoemDto poem, Guid userId)
        {
            var author = _unitOfWork.Authors.GetByUserId(userId);
            var oldPoem = await _unitOfWork.Poems.DbGetByIdAsync(poem.Id);
            if(oldPoem!= null)
            {
                oldPoem.Text = poem.Text;
                oldPoem.Title = poem.Title;
                _unitOfWork.Poems.Update(oldPoem);
            }
            else
            {
                var newPoem = new Poem
                {
                    Title = poem.Title,
                    Text = poem.Text,
                    AuthorId = author.Id,
                    Author = author
                };
                _unitOfWork.Poems.Insert(newPoem);
            }
            return await _unitOfWork.SaveChangesAsync();
        }

        public PoemDto GetPoem (Guid poemId)
        {
            var poem =  _unitOfWork.Poems.GetByIdWithAuthor(poemId);
            if (poem == null)
                throw new BadRequestException("PoemDoesNotExist");
            var result = new PoemDto
            {
                Id = poem.Id,
                Title = poem.Title,
                Text = poem.Text,
                AuthorName = poem.Author.User.Email
            };
            return result;
        }

        public List<PoemDto> GetPoems()
        {
            var poems = _unitOfWork.Poems.GetAllWithAuthor();
            var result = new List<PoemDto>();
            if (!poems.Any())
                return result;
            foreach(var poem in poems)
            {
                result.Add(new PoemDto
                {
                    Id = poem.Id,
                    Title = poem.Title,
                    Text = poem.Text,
                    AuthorName = poem.Author.User.Email
                });
            }

            return result;
        }

        public async Task<bool> DeletePoem(Guid poemId)
        {
            var poem = await _unitOfWork.Poems.DbGetByIdAsync(poemId);
            if (poem == null)
                throw new BadRequestException("PoemDoesNotExist");
            _unitOfWork.Poems.Delete(poem);
            return await _unitOfWork.SaveChangesAsync();
        }

    }
}
