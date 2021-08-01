using AutoFixture.Xunit2;
using BookOrder.Application.Commands;
using BookOrder.Application.DTO;
using BookOrder.Application.Queries;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BookOrder.Application.Tests.Queries
{
    public class GetCallbackCommandQueryTests
    {
        private readonly GetCallbackCommandQuery.Handler _handler;

        public GetCallbackCommandQueryTests()
        {
            _handler = new GetCallbackCommandQuery.Handler();
        }

        [Theory, AutoData]
        public async Task GivenCommandToCancelOrder_WhenHandlingQuery_ThenReturnCancelBookOrderCommand(GetCallbackCommandQuery query)
        {
            // Arrange
            query.Payload.Command = ThirdPartyCallbackPayload.CommandType.CancelOrder;

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeOfType(typeof(CancelBookOrderCommand));
            (result as CancelBookOrderCommand).BookKey.Should().Be(query.Payload.BookKey);
        }

        [Theory, AutoData]
        public async Task GivenOtherCommand_WhenHandlingQuery_ThenReturnNull(GetCallbackCommandQuery query)
        {
            // Arrange
            query.Payload.Command = ThirdPartyCallbackPayload.CommandType.DoOtherStuff;

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }

        [Theory, AutoData]
        public void GivenValidQuery_WhenValidatingQuery_ThenSuccess(GetCallbackCommandQuery query)
        {
            // Arrange
            var validator = new GetCallbackCommandQuery.Validator();

            // Act
            var result = validator.Validate(query);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory, AutoData]
        public void GivenQueryMissingPayload_WhenValidatingQuery_ThenFail(GetCallbackCommandQuery query)
        {
            // Arrange
            query.Payload = null;
            var validator = new GetCallbackCommandQuery.Validator();

            // Act
            var result = validator.Validate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors.FirstOrDefault().PropertyName.Should().Be(nameof(GetCallbackCommandQuery.Payload));
        }


        [Theory, AutoData]
        public void GivenQueryMissingBookKey_WhenValidatingQuery_ThenFail(GetCallbackCommandQuery query)
        {
            // Arrange
            query.Payload.BookKey = null;
            var validator = new GetCallbackCommandQuery.Validator();

            // Act
            var result = validator.Validate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors.FirstOrDefault().PropertyName.Should()
                .Be($"{nameof(GetCallbackCommandQuery.Payload)}.{nameof(GetCallbackCommandQuery.Payload.BookKey)}");
        }

        [Theory, AutoData]
        public void GivenQueryMissingCommand_WhenValidatingQuery_ThenFail(GetCallbackCommandQuery query)
        {
            // Arrange
            query.Payload.Command = null;
            var validator = new GetCallbackCommandQuery.Validator();

            // Act
            var result = validator.Validate(query);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors.FirstOrDefault().PropertyName.Should()
                .Be($"{nameof(GetCallbackCommandQuery.Payload)}.{nameof(GetCallbackCommandQuery.Payload.Command)}");
        }
    }
}
