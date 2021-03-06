using AutoMapper;
using BookOrder.Application.Commands;
using BookOrder.Application.DTO;
using BookOrder.Application.Interface.Service;
using BookOrder.Domain;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BookOrder.Application.Mapping
{
    public class BookApiResponseToOrderConverter : ITypeConverter<BookApiResponse, Order>
    {
        private readonly IValidator<BookApiResponse> _validator;
        private readonly IDateTimeService _dateTimeService;

        public BookApiResponseToOrderConverter(IValidator<BookApiResponse> validator, IDateTimeService dateTimeService)
        {
            _validator = validator;
            _dateTimeService = dateTimeService;
        }

        public Order Convert(BookApiResponse source, Order destination, ResolutionContext context)
        {
            _validator.Validate(source, options => options.IncludeRuleSets(nameof(CreateBookOrderCommand))
                .IncludeRulesNotInRuleSet()
                .ThrowOnFailures());
            var now = _dateTimeService.GetUtcNow();
            destination = new Order
            {
                BookKey = source.Key,
                Title = source.Title,
                Authors = string.Join("; ", source.Authors.Select(a => a.Name)),
                Status = OrderStatus.Ordered,
                CreatedAt = now,
                UpdatedAt = now
            };

            return destination;
        }
    }
}
