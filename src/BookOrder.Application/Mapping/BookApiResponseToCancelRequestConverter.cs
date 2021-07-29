using AutoMapper;
using BookOrder.Application.Commands;
using BookOrder.Application.DTO;
using BookOrder.Domain;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookOrder.Application.Mapping
{
    public class BookApiResponseToCancelRequestConverter : ITypeConverter<BookApiResponse, CancelRequest>
    {
        private readonly IValidator<BookApiResponse> _validator;

        public BookApiResponseToCancelRequestConverter(IValidator<BookApiResponse> validator)
        {
            _validator = validator;
        }

        public CancelRequest Convert(BookApiResponse source, CancelRequest destination, ResolutionContext context)
        {
            _validator.Validate(source, options => options.IncludeRuleSets(nameof(CancelBookOrderCommand))
                .IncludeRulesNotInRuleSet()
                .ThrowOnFailures());
            
            destination = new CancelRequest
            {
                BookKey = source.Key,
                Subjects = new List<string>(source.Subjects),
                CancellationTime = DateTime.UtcNow
            };
            
            return destination;
        }
    }
}
