using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Dtos
{
    public class PoemDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string AuthorName { get; set; }
    }
}
