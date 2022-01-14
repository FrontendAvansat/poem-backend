using System;

namespace DataLayer.Entities
{
    public class Token : BaseEntity
    {
        public string TokenString { get; set; }
        public DateTime ExpireDate { get; set; }
        public virtual TokenTypes Type { get; set; }
        public bool IsRevoked { get; set; }
        public Guid AppUserId { get; set; }
        public AppUser User { get; set; }
    }

    public enum TokenTypes
    {
        AccessToken = 10,
        RefreshToken = 20
    }
}
