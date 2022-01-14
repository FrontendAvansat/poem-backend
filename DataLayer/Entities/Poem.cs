using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Entities
{
    public class Poem : BaseEntity
    {
        public string Title { get; set; }
        public Guid AuthorId { get; set; }
        public virtual Author Author { get; set; }
    }
}
