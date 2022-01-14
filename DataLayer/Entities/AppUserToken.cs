
using System;

namespace DataLayer.Entities
{
    public class AppUserToken : BaseEntity
    {
        public Guid AppUserId { get; set; }
        public AppUser User { get; set; }
        public Guid TokenId { get; set; }
        public Token Token { get; set; }
    }
}
