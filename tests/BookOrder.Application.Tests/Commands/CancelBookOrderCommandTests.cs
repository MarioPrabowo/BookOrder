using AutoFixture.Xunit2;
using AutoMapper;
using BookOrder.Application.Commands;
using BookOrder.Application.DTO;
using BookOrder.Application.Interface.Client;
using BookOrder.Application.Interface.Persistence;
using BookOrder.Domain;
using BookOrder.Domain.Common;
using BookOrder.Tests.Common;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BookOrder.Application.Tests.Commands
{
    public class CancelBookOrderCommandTests
    {
        private readonly Mock<IBookApiClient> _mockBookApiClient;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IBookOrderRepository> _mockBookOrderRepository;
        private readonly CancelBookOrderCommand.Handler _handler;

        public CancelBookOrderCommandTests()
        {
            _mockBookApiClient = new Mock<IBookApiClient>();
            _mockMapper = new Mock<IMapper>();
            _mockBookOrderRepository = new Mock<IBookOrderRepository>();
            _handler = new CancelBookOrderCommand.Handler(_mockBookApiClient.Object, _mockMapper.Object, _mockBookOrderRepository.Object);
        }

        [Theory, AutoData]
        public async Task GivenOrderDoesNotExist_WhenHandlingCommand_ThenThrowException(CancelBookOrderCommand command, BookApiResponse bookApiResponse, CancelRequest cancelRequest)
        {
            // Arrange
            bookApiResponse.Key = command.BookKey;
            cancelRequest.BookKey = command.BookKey;
            _mockBookApiClient.Setup(c => c.GetBookInfo(command.BookKey)).ReturnsAsync(bookApiResponse);
            _mockMapper.Setup(m => m.Map<CancelRequest>(bookApiResponse)).Returns(cancelRequest);
            _mockBookOrderRepository.Setup(r => r.GetBookOrderAsync(command.BookKey)).ReturnsAsync(null as Order);

            // Act + Assert
            await Assert.ThrowsAsync<BusinessLogicException>(()=>  _handler.Handle(command, CancellationToken.None));

            // Assert
            _mockBookApiClient.Verify(c => c.GetBookInfo(command.BookKey), Times.Once);
            _mockMapper.Verify(m => m.Map<CancelRequest>(bookApiResponse), Times.Once);
            _mockBookOrderRepository.Verify(r => r.GetBookOrderAsync(command.BookKey), Times.Once);
            _mockBookOrderRepository.Verify(r => r.SaveBookOrderAsync(It.IsAny<Order>()), Times.Never);
        }

        [Theory, AutoData]
        public async Task GivenValidCommand_WhenHandlingCommand_ThenCancelOrder(CancelBookOrderCommand command, BookApiResponse bookApiResponse, CancelRequest cancelRequest, Order existingOrder)
        {
            // Arrange
            bookApiResponse.Key = command.BookKey;
            cancelRequest.BookKey = command.BookKey;
            existingOrder.BookKey = command.BookKey;
            existingOrder.Status = OrderStatus.Ordered;
            var order = existingOrder.DeepClone();
            _mockBookApiClient.Setup(c => c.GetBookInfo(command.BookKey)).ReturnsAsync(bookApiResponse);
            _mockMapper.Setup(m => m.Map<CancelRequest>(bookApiResponse)).Returns(cancelRequest);
            _mockBookOrderRepository.Setup(r => r.GetBookOrderAsync(order.BookKey)).ReturnsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(existingOrder, options => options
                .Excluding(o => o.Status)
                .Excluding(o => o.Covers)
                .Excluding(o => o.UpdatedAt));
            result.Status.Should().Be(OrderStatus.Cancelled);
            result.Covers.Should().Be(cancelRequest.Covers);
            result.UpdatedAt.Should().Be(cancelRequest.CancellationTime);
            _mockBookApiClient.Verify(c => c.GetBookInfo(command.BookKey), Times.Once);
            _mockMapper.Verify(m => m.Map<CancelRequest>(bookApiResponse), Times.Once);
            _mockBookOrderRepository.Verify(r => r.GetBookOrderAsync(order.BookKey), Times.Once);
            _mockBookOrderRepository.Verify(r => r.SaveBookOrderAsync(order), Times.Once);
        }

        [Theory, AutoData]
        public async Task GivenOrderAlreadyCancelled_WhenHandlingCommand_ThenDoNotMakeAnyChanges(CancelBookOrderCommand command, BookApiResponse bookApiResponse, CancelRequest cancelRequest, Order existingOrder)
        {
            // Arrange
            bookApiResponse.Key = command.BookKey;
            cancelRequest.BookKey = command.BookKey;
            existingOrder.BookKey = command.BookKey;
            existingOrder.Status = OrderStatus.Cancelled;
            var order = existingOrder.DeepClone();
            _mockBookApiClient.Setup(c => c.GetBookInfo(command.BookKey)).ReturnsAsync(bookApiResponse);
            _mockMapper.Setup(m => m.Map<CancelRequest>(bookApiResponse)).Returns(cancelRequest);
            _mockBookOrderRepository.Setup(r => r.GetBookOrderAsync(order.BookKey)).ReturnsAsync(order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(existingOrder);
            _mockBookApiClient.Verify(c => c.GetBookInfo(command.BookKey), Times.Once);
            _mockMapper.Verify(m => m.Map<CancelRequest>(bookApiResponse), Times.Once);
            _mockBookOrderRepository.Verify(r => r.GetBookOrderAsync(order.BookKey), Times.Once);
            _mockBookOrderRepository.Verify(r => r.SaveBookOrderAsync(order), Times.Never);
        }

        [Theory, AutoData]
        public void GivenValidCommand_WhenValidatingCommand_ThenSuccess(CancelBookOrderCommand command)
        {
            // Arrange
            var validator = new CancelBookOrderCommand.Validator();

            // Act
            var result = validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory, AutoData]
        public void GivenCommandMissingBookKey_WhenValidatingCommand_ThenFail(CancelBookOrderCommand command)
        {
            // Arrange
            command.BookKey = null;
            var validator = new CancelBookOrderCommand.Validator();

            // Act
            var result = validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors.FirstOrDefault(e => e.PropertyName == nameof(CancelBookOrderCommand.BookKey)).Should().NotBeNull();
        }
    }
}
