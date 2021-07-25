using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Alter.Models
{
    public class Answer
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public String Text { get; set; }

        public Int32 Amount { get; set; } = 0;

        public Poll Poll { get; set; }

        public Guid PollId { get; set; }
    }
}
