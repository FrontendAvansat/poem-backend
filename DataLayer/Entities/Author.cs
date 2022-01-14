using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Author : BaseEntity
    {
        public Guid UserId { get; set; }
        public AppUser User { get; set; }
        public virtual List<Poem> Poems { get; set; } = new List<Poem>(); 
    }
}
