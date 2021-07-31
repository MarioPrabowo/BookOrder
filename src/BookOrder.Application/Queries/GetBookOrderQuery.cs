using BookOrder.Application.Interface.Persistence;
using BookOrder.Domain;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BookOrder.Application.Queries
{
    public class GetBookOrderQuery: IRequest<Order>
    {
        public string BookKey { get; set; }

        public class Handler : IRequestHandler<GetBookOrderQuery, Order>
        {
            private readonly IBookOrderRepository _bookOrderRepository;

            public Handler(IBookOrderRepository bookOrderRepository)
            {
                _bookOrderRepository = bookOrderRepository;
            }

            public Task<Order> Handle(GetBookOrderQuery request, CancellationToken cancellationToken)
            {
                return _bookOrderRepository.GetBookOrderAsync(request.BookKey);
            }
        }

        public class Validator : AbstractValidator<GetBookOrderQuery>
        {
            public Validator()
            {
                RuleFor(v => v.BookKey).NotNull();
            }
        }
    }
}
