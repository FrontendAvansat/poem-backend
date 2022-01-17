using DataLayer;
using DataLayer.Entities;
using DataLayer.Entities.Enums;
using DataLayer.Repositories;
using Services.Dtos;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Services
{
    public interface IUserAuthentificationHelper
    {
        UserInformationDto GetUserInformationByTypeAsync(AppUser user);
        void VerifyUser(AppUser user);
        UserInformationDto UserSignedInAsync(AppUser user, LoginRequest loginRequest, Token accessToken, Token refreshToken);
        Task<Author> CreateUserAndAuthorAsync(RegisterDto registerRequest, string language);
    }
    public class UserAuthentificationHelper : IUserAuthentificationHelper
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppUserRepository _appUserRepository;

        public UserAuthentificationHelper(IUnitOfWork unitOfWork, IAppUserRepository appUserRepository)
        {
            _appUserRepository = appUserRepository;
            _unitOfWork = unitOfWork;

        }
        private async Task AddClaimsToUserAsync(AppUser user, string customName, Guid id)
        {
            await _appUserRepository.AddClaimToUserAsync(user, new Claim(ClaimTypes.Email, user.Email));
            await _appUserRepository.AddClaimToUserAsync(user, new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            await _appUserRepository.AddClaimToUserAsync(user, new Claim(ClaimTypes.Name, user.FirstName));
            await _appUserRepository.AddClaimToUserAsync(user, new Claim(ClaimTypes.Surname, user.LastName));
            await _appUserRepository.AddClaimToUserAsync(user, new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
            await _appUserRepository.AddClaimToUserAsync(user, new Claim(customName, id.ToString()));
        }
        public UserInformationDto GetUserInformationByTypeAsync(AppUser user)
        {
            var userInfo = _appUserRepository.GetById(user.Id);
            if(userInfo != null)
            {
                var result = new UserInformationDto
                {
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    Email = userInfo.Email
                };

                return result;

            }
                
          
            throw new BadRequestException("User_Not_Found");
        }

        public void VerifyUser(AppUser user)
        {
            if (user == null)
            {
                throw new BadRequestException("Invalid_Login");
            }

        }

        public UserInformationDto UserSignedInAsync(AppUser user, LoginRequest loginRequest, Token accessToken, Token refreshToken)
        {

            var author = _unitOfWork.Authors.GetByUserId(user.Id);
            if (author == null) throw new BadRequestException("NoCustomerFound");

            UserInformationDto login = new UserInformationDto
            {
                Email = loginRequest.Email,
                AccessToken = accessToken.TokenString,
                RefreshToken = refreshToken.TokenString,
                FirstName = user.FirstName,
                LastName = user.LastName
            };
            return login;
        }

        public async Task<Author> CreateUserAndAuthorAsync(RegisterDto registerRequest, string language)
        {
            var user = await CreateUserAsync(registerRequest, AppUserTypes.User, language);
            var newAuthor = new Author
            {
                User = user,
                UserId = user.Id,
            };
            _unitOfWork.Authors.Insert(newAuthor);
            await AddClaimsToUserAsync(user, "userId", newAuthor.Id);
            return newAuthor;
        }

        private async Task<AppUser> CreateUserAsync(RegisterDto request, AppUserTypes type, string language)
        {
           
            var oldUser = await _appUserRepository.FindUserByEmailAsync(request.Email, asNoTracking: true);
            if (oldUser != null) throw new BadRequestException("UserAlreadyExists");

            var user = new AppUser
            {
                Type = type,
                Email = request.Email,
                UserName = request.Email,
                LastName = request.LastName,
                FirstName = request.FirstName,
                ValidationToken = Guid.NewGuid().ToString()
            };

            var result = await _appUserRepository.CreateUserAsync(user, request.Password);
            if (!result.Succeeded) throw new Exception("UserNotCreated");

            var roleResult = await _appUserRepository.AddUserToRoleAsync(user, type.ToString());
            if (roleResult == null) throw new Exception("RoleNotAdded");

            return user;
        }
    }
}
