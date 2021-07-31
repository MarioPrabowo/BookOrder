using AutoMapper;
using BookOrder.Domain.Common;
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
    public class CancelBookOrderCommand : IRequest<Order>
    {
        public string BookKey { get; set; }

        public class Handler : IRequestHandler<CancelBookOrderCommand, Order>
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

            public async Task<Order> Handle(CancelBookOrderCommand request, CancellationToken cancellationToken)
            {
                var bookInfo = await _bookApiClient.GetBookInfo(request.BookKey);
                var cancelRequest = _mapper.Map<CancelRequest>(bookInfo);
                var existingOrder = await _bookOrderRepository.GetBookOrderAsync(bookInfo.Key);
                if(existingOrder == null)
                {
                    throw new BusinessLogicException("Unable to cancel non-existing order");
                }
                var cancelOrderResult = existingOrder.CancelOrder(cancelRequest);
                if (cancelOrderResult == OrderOperationResult.ChangesMade)
                {
                    await _bookOrderRepository.SaveBookOrderAsync(existingOrder);
                }
                return existingOrder;
            }
        }

        public class Validator : AbstractValidator<CancelBookOrderCommand>
        {
            public Validator()
            {
                RuleFor(v => v.BookKey).NotNull();
            }
        }
    }
}
