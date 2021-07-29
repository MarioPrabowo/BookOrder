using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BookOrder.Application.Commands;
using BookOrder.Application.DTO;
using BookOrder.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookOrderApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class BookOrderController : ControllerBase
    {
        public readonly IMediator _mediator;

        public BookOrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<Order> CreateBookOrder(CreateBookOrderCommand createBookOrderCommand)
        {
            return await _mediator.Send(createBookOrderCommand);
        }

        [HttpPatch]
        public async Task<Order> CancelOrder(CancelBookOrderCommand cancelBookOrderCommand)
        {
            return await _mediator.Send(cancelBookOrderCommand);
        }

        [HttpPost("callback")]
        public async Task ProcessThirdPartyCallback(ThirdPartyCallbackPayload payload)
        {
            await _mediator.Send(new ProcessThirdPartyCallbackCommand
            {
                Payload = payload
            });
        }
    }
}