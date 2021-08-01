using BookOrder.Application.Commands;
using BookOrder.Application.DTO;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BookOrder.Application.Queries
{
    public class GetCallbackCommandQuery : IRequest<IBaseRequest>
    {
        public ThirdPartyCallbackPayload Payload { get; set; }

        public class Handler : IRequestHandler<GetCallbackCommandQuery, IBaseRequest>
        {
            public Handler()
            {
            }

            public Task<IBaseRequest> Handle(GetCallbackCommandQuery request, CancellationToken cancellationToken)
            {
                IBaseRequest command = null;

                switch(request.Payload.Command)
                {
                    case ThirdPartyCallbackPayload.CommandType.CancelOrder:
                    {
                        command = new CancelBookOrderCommand()
                        {
                            BookKey = request.Payload.BookKey
                        };
                        break;
                    }
                }

                return Task.FromResult(command);
            }
        }

        public class Validator : AbstractValidator<GetCallbackCommandQuery>
        {
            public Validator()
            {
                RuleFor(v => v.Payload).NotNull();
                When(v => v.Payload != null, () =>
                {
                    RuleFor(v => v.Payload.BookKey).NotEmpty();
                    RuleFor(v => v.Payload.Command).NotNull();
                });
            }
        }
    }
}
