using BookOrder.Application.DTO;
using BookOrder.Application.Interface.Service;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookOrder.Application.Services
{
    public class BookAvailabilityService : IBookAvailabilityService
    {
        private readonly IOptions<BookAvailabilityOption> _options;

        public BookAvailabilityService(IOptions<BookAvailabilityOption> options)
        {
            _options = options;
        }

        public bool IsBookAvailable(string author)
        {
            return _options.Value.Authors.Contains(author);
        }
    }
}
