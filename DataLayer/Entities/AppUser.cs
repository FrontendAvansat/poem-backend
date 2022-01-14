using DataLayer.Entities.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataLayer.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual AppUserTypes Type { get; set; }
        public string ValidationToken { get; set; }
        public string RefreshToken { get; set; }
        public string PasswordToken { get; set; }
        public List<Token> Tokens { get; set; }

        public AppUser()
        {
            Tokens = new List<Token>();
        }
    }
}
