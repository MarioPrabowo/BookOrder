using BookOrder.Application.DTO;
using BookOrder.Application.Interface.Service;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace BookOrder.Application.Services
{
    public class BookAvailabilityService : IBookAvailabilityService
    {
        private readonly List<string> _authors;

        public BookAvailabilityService(IOptions<BookAvailabilityOption> options)
        {
            _authors = options.Value.Authors;

            if(!_authors.Any())
            {
                throw new ConfigurationErrorsException($"{nameof(BookAvailabilityOption)}.{nameof(BookAvailabilityOption.Authors)} setting is missing");
            }
        }

        public bool IsBookAvailable(string author)
        {
            return _authors.Contains(author);
        }
    }
}
