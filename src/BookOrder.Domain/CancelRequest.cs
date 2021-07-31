using System;
using System.Collections.Generic;
using System.Text;

namespace BookOrder.Domain
{
    public class CancelRequest
    {
        public string BookKey { get; set; }
        public string Covers { get; set; }
        public DateTime CancellationTime { get; set; }
    }
}
