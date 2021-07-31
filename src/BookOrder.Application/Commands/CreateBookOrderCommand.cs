using AutoMapper;
using BookOrder.Application.Interface.Client;
using BookOrder.Application.Interface.Persistence;
using BookOrder.Domain;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BookOrder.Application.Commands
{
    public class CreateBookOrderCommand : IRequest<Order>
    {
        public string BookKey { get; set; }

        public class Handler : IRequestHandler<CreateBookOrderCommand, Order>
        {
            private readonly IBookApiClient _bookApiClient;
            private readonly IMapper _mapper;
            private readonly IBookOrderRepository _bookOrderRepository;

            public Handler(IBookApiClient bookApiClient, IMapper mapper, IBookOrderRepository bookOrderRepository)
            {
                _bookApiClient = bookApiClient;
                _mapper = mapper;
                _bookOrderRepository = bookOrderRepository;
            }

            public async Task<Order> Handle(CreateBookOrderCommand request, CancellationToken cancellationToken)
            {
                var bookInfo = await _bookApiClient.GetBookInfo(request.BookKey);
                var order = _mapper.Map<Order>(bookInfo);
                var existingOrder = await _bookOrderRepository.GetBookOrderAsync(order.BookKey);
                if(existingOrder != null)
                {
                    order.CreatedAt = existingOrder.CreatedAt;
                }

                await _bookOrderRepository.SaveBookOrderAsync(order);
                return order;
            }
        }

        public class Validator : AbstractValidator<CreateBookOrderCommand>
        {
            public Validator()
            {
                RuleFor(v => v.BookKey).NotNull();
            }
        }
    }
}
