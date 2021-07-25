using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alter.Models
{
    public class VoteViewModel
    {
        public Guid PollId { get; set; }

        public Guid AnswerId { get; set; }
    }
}
