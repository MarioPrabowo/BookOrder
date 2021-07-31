using AutoMapper;
using BookOrder.Application.Commands;
using BookOrder.Application.DTO;
using BookOrder.Application.Interface.Service;
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
        private readonly IDateTimeService _dateTimeService;

        public BookApiResponseToCancelRequestConverter(IValidator<BookApiResponse> validator, IDateTimeService dateTimeService)
        {
            _validator = validator;
            _dateTimeService = dateTimeService;
        }

        public CancelRequest Convert(BookApiResponse source, CancelRequest destination, ResolutionContext context)
        {
            _validator.Validate(source, options => options.IncludeRuleSets(nameof(CancelBookOrderCommand))
                .IncludeRulesNotInRuleSet()
                .ThrowOnFailures());

            destination = new CancelRequest
            {
                BookKey = source.Key,
                Covers = string.Join("; ", source.Covers),
                CancellationTime = _dateTimeService.GetUtcNow()
            };
            
            return destination;
        }
    }
}
