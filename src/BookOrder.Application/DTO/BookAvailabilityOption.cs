using System;
using System.Collections.Generic;
using System.Text;

namespace BookOrder.Application.DTO
{
    public class BookAvailabilityOption
    {
        public const string BookAvailability = nameof(BookAvailability);

        public List<string> Authors { get; set; } = new List<string>();
    }
}
