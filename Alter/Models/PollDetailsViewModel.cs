using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Alter.Models
{
    public class PollDetailsViewModel
    {
        public Poll Poll { get; set; }

        public IEnumerable<Answer> Answers { get; set; }
    }
}
