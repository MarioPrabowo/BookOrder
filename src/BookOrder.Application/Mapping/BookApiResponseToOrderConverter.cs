using AutoMapper;
using BookOrder.Application.Commands;
using BookOrder.Application.DTO;
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

        public BookApiResponseToOrderConverter(IValidator<BookApiResponse> validator)
        {
            _validator = validator;
        }

        public Order Convert(BookApiResponse source, Order destination, ResolutionContext context)
        {
            _validator.Validate(source, options => options.IncludeRuleSets(nameof(CreateBookOrderCommand))
                .IncludeRulesNotInRuleSet()
                .ThrowOnFailures());

            Order order = new Order
            {
                BookKey = source.Key,
                Title = source.Title,
                Authors = string.Join("; ", source.Authors.Select(a => a.Name))
            };

            return order;
        }
    }
}
