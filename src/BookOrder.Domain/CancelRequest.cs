using System;
using System.Collections.Generic;
using System.Text;

namespace BookOrder.Domain
{
    public class CancelRequest
    {
        public string BookKey { get; set; }
        public List<string> Subjects { get; set; }
        public DateTime CancellationTime { get; set; }
    }
}
