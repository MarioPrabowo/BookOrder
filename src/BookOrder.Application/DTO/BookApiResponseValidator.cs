using BookOrder.Application.Commands;
using BookOrder.Application.Interface.Service;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookOrder.Application.DTO
{
    public class BookApiResponseValidator : AbstractValidator<BookApiResponse>
    {
        private readonly IBookAvailabilityService _bookAvailabilityService;

        public BookApiResponseValidator(IBookAvailabilityService bookAvailabilityService)
        {
            _bookAvailabilityService = bookAvailabilityService;
            
            RuleSet(nameof(CreateBookOrderCommand), () => {
                RuleFor(x => x.Title).NotNull();
                RuleFor(x => x.Authors).NotEmpty();
                RuleFor(x => x.Authors)
                    .Must(authors => authors == null || authors.All(a => _bookAvailabilityService.IsBookAvailable(a.Name)))
                    .WithMessage("One or more authors are not available in our service.");
                RuleForEach(x => x.Authors).ChildRules(authors =>
                {
                    authors.RuleFor(a => a.Name).NotEmpty();
                });
            });

            RuleSet(nameof(CancelBookOrderCommand), () => {
                RuleFor(x => x.Covers).NotEmpty();
            });

            RuleFor(x => x.Key).NotNull();
        }
    }
}
