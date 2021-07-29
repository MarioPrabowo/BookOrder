using BookOrder.Application.DTO;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BookOrder.Application.Commands
{
    public class ProcessThirdPartyCallbackCommand : IRequest<Unit>
    {
        public ThirdPartyCallbackPayload Payload { get; set; }

        public class Handler : IRequestHandler<ProcessThirdPartyCallbackCommand, Unit>
        {
            private readonly IMediator _mediator;

            public Handler(IMediator mediator)
            {
                _mediator = mediator;
            }

            public async Task<Unit> Handle(ProcessThirdPartyCallbackCommand request, CancellationToken cancellationToken)
            {
                switch(request.Payload.Command)
                {
                    case ThirdPartyCallbackPayload.CommandType.CancelOrder:
                        await _mediator.Send( new CancelBookOrderCommand()
                        {
                            BookKey = request.Payload.BookKey
                        });
                        break;
                }

                return Unit.Value;
            }
        }

        public class Validator : AbstractValidator<ProcessThirdPartyCallbackCommand>
        {
            public Validator()
            {
                RuleFor(v => v.Payload).NotNull();
                RuleFor(v => v.Payload.BookKey).NotEmpty();
                RuleFor(v => v.Payload.Command).NotNull();
            }
        }
    }
}
