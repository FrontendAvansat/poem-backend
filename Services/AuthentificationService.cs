using DataLayer;
using DataLayer.Entities;
using DataLayer.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Dtos;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IAuthentificationService
    {
        Task<Token> GenerateJwtToken(AppUser user);
        Token GenerateRefreshToken(AppUser user);
        Task<UserInformationDto> LoginAsync(LoginRequest loginRequest);
        Task<TokenResponse> RefreshTokenAsync(TokenRequest request);
        Task<Author> RegisterAuthorAsync(RegisterDto registerRequest);
        UserInformationDto GetUserInformation(Guid id);
    }
    public class AuthentificationService : IAuthentificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IAppUserRepository _appUserRepository;
        private readonly IUserAuthentificationHelper _userAuthentificationHelper;
        public AuthentificationService(IUnitOfWork unitOfWork, IConfiguration configuration, IAppUserRepository appUserRepository, IUserAuthentificationHelper userAuthentificationHelper)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _appUserRepository = appUserRepository;
            _userAuthentificationHelper = userAuthentificationHelper;
        }

        public async Task<Author> RegisterAuthorAsync(RegisterDto registerRequest)
        {
            var author = await _userAuthentificationHelper.CreateUserAndAuthorAsync(registerRequest);
            return author;
        }
        public async Task<Token> GenerateJwtToken(AppUser user)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecurityKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"],
                Expires = DateTime.Now.AddYears(1),
                SigningCredentials = credentials
            };

            var token = handler.CreateToken(tokenDescriptor);
            var tokenString = handler.WriteToken(token);
            var accessToken = new Token
            {
                TokenString = tokenString,
                ExpireDate = tokenDescriptor.Expires.Value,
                IsRevoked = false,
                Type = TokenTypes.AccessToken,
                AppUserId = user.Id
            };
            return accessToken;
        }

        public Token GenerateRefreshToken(AppUser user)
        {
            var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[64];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return new Token
            {
                TokenString = Convert.ToBase64String(randomBytes)
                    .Replace("/", "")
                    .Replace("+", "")
                    .Replace("?", "")
                    .Replace("&", ""),
                ExpireDate = DateTime.UtcNow.AddDays(7),
                Type = TokenTypes.RefreshToken,
                IsRevoked = false,
                AppUserId = user.Id
            };
        }

        public async Task<UserInformationDto> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _appUserRepository.FindUserByEmailAsync(loginRequest.Email, asNoTracking: false);
            _userAuthentificationHelper.VerifyUser(user);

            var result = await _appUserRepository.SignInAsync(loginRequest.Email, loginRequest.Password);
            if (!result.Succeeded) throw new BadRequestException("Invalid_login");

            var accessToken = await GenerateJwtToken(user);
            _unitOfWork.Tokens.Insert(accessToken);
            var refreshToken = GenerateRefreshToken(user);
            _unitOfWork.Tokens.Insert(refreshToken);
            await _unitOfWork.SaveChangesAsync();
            return  _userAuthentificationHelper.UserSignedInAsync(user, loginRequest, accessToken, refreshToken);
        }

        public async Task<TokenResponse> RefreshTokenAsync(TokenRequest request)
        {
            var refreshToken = _unitOfWork.Tokens.CacheGetRefreshTokenByTokenString(request.RefreshToken);
            if (refreshToken.ExpireDate < DateTime.UtcNow) throw
                new BadRequestException("ExpiredToken");

            refreshToken.IsRevoked = true;
            var accessToken = _unitOfWork.Tokens.CacheGetAccessTokenByTokenString(request.AccessToken);
            accessToken.IsRevoked = true;
            accessToken.ExpireDate = DateTime.UtcNow;

            var user = await _appUserRepository.FindUserByRefreshTokenAsync(request.RefreshToken);
            var newAccessToken = await GenerateJwtToken(user);
            _unitOfWork.Tokens.Insert(newAccessToken);
            var newRefreshToken = GenerateRefreshToken(user);
            _unitOfWork.Tokens.Insert(newRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            var response = new TokenResponse
            {
                AccessToken = newAccessToken.TokenString,
                RefreshToken = newRefreshToken.TokenString
            };

            return response;
        }
        public UserInformationDto GetUserInformation(Guid id)
        {
            var user = _appUserRepository.GetById(id, asNoTracking: true);
            if (user == null)
            {
                throw new BadRequestException("NoUserFound");
            }
            return  _userAuthentificationHelper.GetUserInformationByTypeAsync(user);
        }
    }
}
