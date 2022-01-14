using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Repositories
{
    public interface IAppUserRepository
    {
        Task<AppUser> FindUserByEmailAsync(string email, bool asNoTracking = false);
        Task<AppUser> AddClaimToUserAsync(AppUser user, Claim claim);
        Task<AppUser> AddUserToRoleAsync(AppUser user, string roleName);
        Task<IdentityResult> CreateUserAsync(AppUser user, string password);
        AppUser GetById(Guid? id, bool asNoTracking = false);
        Task<IList<Claim>> GetClaimsAsync(AppUser user);
        Task<List<string>> GetUserRolesAsync(AppUser user);
        Task<SignInResult> SignInAsync(string username, string password);
        Task<AppUser> FindUserByRefreshTokenAsync(string token);
        Task<string> GeneratePasswordTokenAsync(AppUser user);
        Task<IdentityResult> UpdateUserAsync(AppUser user);
        Task<AppUser> RemovePasswordFromUser(AppUser user);
        Task<IdentityResult> AddPasswordForUser(AppUser user, string password);
        void UpdateUserNoIdentity(AppUser user);
        void UpdateUserNoIdentityOnlyDb(AppUser user);
        AppUser GetById(Guid id);
        List<AppUser> GetAllUsers();
    }

    public class AppUserRepository : IAppUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AppUserRepository(ApplicationDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<AppUser> FindUserByEmailAsync(string email, bool asNoTracking = false)
        {
            return asNoTracking
            ? _context.Users.AsNoTracking().FirstOrDefault(us => us.Email == email)
            : await _userManager.FindByEmailAsync(email);
        }

        public async Task<AppUser> AddClaimToUserAsync(AppUser user, Claim claim)
        {
            await _userManager.AddClaimAsync(user, claim);
            return user;
        }

        public async Task<AppUser> AddUserToRoleAsync(AppUser user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
            return user;
        }

        public async Task<IdentityResult> CreateUserAsync(AppUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result;
        }

        public AppUser GetById(Guid? id, bool asNoTracking = false)
        {
            return asNoTracking 
                ? _context.Users.AsNoTracking().FirstOrDefault(u => u.Id == id)
                : _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public AppUser GetById(Guid id)
        {
            return _context.Users
                .First(u => u.Id == id);
        }

        public async Task<IList<Claim>> GetClaimsAsync(AppUser user)
        {
            return await _userManager.GetClaimsAsync(user);
        }

        public async Task<List<string>> GetUserRolesAsync(AppUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }


        public async Task<SignInResult> SignInAsync(string username, string password)
        {
            return await _signInManager.PasswordSignInAsync(username, password, true, false);
        }

        public async Task<AppUser> FindUserByRefreshTokenAsync(string token)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == token);
        }

        public async Task<string> GeneratePasswordTokenAsync(AppUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<IdentityResult> UpdateUserAsync(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
            return await _userManager.UpdateAsync(user);
        }

        public void UpdateUserNoIdentity(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;

        }

        public async Task<AppUser> RemovePasswordFromUser(AppUser user)
        {
            var _ = await _userManager.RemovePasswordAsync(user);
            var result = await _userManager.FindByIdAsync(user.Id.ToString());
            return result;
        }

        public async Task<IdentityResult> AddPasswordForUser(AppUser user, string password)
        {
            return await _userManager.AddPasswordAsync(user, password);
        }

        public void UpdateUserNoIdentityOnlyDb(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;

        }

        public List<AppUser> GetAllUsers()
        {
            return _context.Users.ToList();
        }
    }
}
