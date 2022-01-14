using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class BaseEntity
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }
    }
}
