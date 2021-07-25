using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Alter.Models
{
    public class Poll
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public String Title { get; set; }

        [Required]
        public String Question { get; set; }

        public Boolean Finished { get; set; } = false;

        public ICollection<Answer> Answers { get; set; }

        public ICollection<ApplicationUser> Users { get; set; }
    }
}
