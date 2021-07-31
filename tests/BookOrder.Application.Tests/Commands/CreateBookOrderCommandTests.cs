using AutoFixture.Xunit2;
using AutoMapper;
using BookOrder.Application.Commands;
using BookOrder.Application.DTO;
using BookOrder.Application.Interface.Client;
using BookOrder.Application.Interface.Persistence;
using BookOrder.Domain;
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
    public class CreateBookOrderCommandTests
    {
        private readonly Mock<IBookApiClient> _mockBookApiClient;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IBookOrderRepository> _mockBookOrderRepository;
        private readonly CreateBookOrderCommand.Handler _handler;

        public CreateBookOrderCommandTests()
        {
            _mockBookApiClient = new Mock<IBookApiClient>();
            _mockMapper = new Mock<IMapper>();
            _mockBookOrderRepository = new Mock<IBookOrderRepository>();
            _handler = new CreateBookOrderCommand.Handler(_mockBookApiClient.Object, _mockMapper.Object, _mockBookOrderRepository.Object);
        }

        [Theory, AutoData]
        public async Task GivenNewOrder_WhenHandlingCommand_ThenCreateBookOrder(CreateBookOrderCommand command, BookApiResponse bookApiResponse, Order order)
        {
            // Arrange
            bookApiResponse.Key = command.BookKey;
            order.BookKey = command.BookKey;
            _mockBookApiClient.Setup(c => c.GetBookInfo(command.BookKey)).ReturnsAsync(bookApiResponse);
            _mockMapper.Setup(m => m.Map<Order>(bookApiResponse)).Returns(order);
            _mockBookOrderRepository.Setup(r => r.GetBookOrderAsync(order.BookKey)).ReturnsAsync(null as Order);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(order);
            _mockBookApiClient.Verify(c => c.GetBookInfo(command.BookKey), Times.Once);
            _mockMapper.Verify(m => m.Map<Order>(bookApiResponse), Times.Once);
            _mockBookOrderRepository.Verify(r => r.GetBookOrderAsync(order.BookKey), Times.Once);
        }

        [Theory, AutoData]
        public async Task GivenExistingOrder_WhenHandlingCommand_ThenUpdateBookOrderButUseExistingOrderCreateDate(CreateBookOrderCommand command, BookApiResponse bookApiResponse, Order order, Order existingOrder)
        {
            // Arrange
            bookApiResponse.Key = command.BookKey;
            order.BookKey = command.BookKey;
            existingOrder.BookKey = command.BookKey;
            _mockBookApiClient.Setup(c => c.GetBookInfo(command.BookKey)).ReturnsAsync(bookApiResponse);
            _mockMapper.Setup(m => m.Map<Order>(bookApiResponse)).Returns(order);
            _mockBookOrderRepository.Setup(r => r.GetBookOrderAsync(order.BookKey)).ReturnsAsync(existingOrder);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(order, options => options.Excluding(o => o.CreatedAt));
            result.CreatedAt.Should().Be(existingOrder.CreatedAt);
            _mockBookApiClient.Verify(c => c.GetBookInfo(command.BookKey), Times.Once);
            _mockMapper.Verify(m => m.Map<Order>(bookApiResponse), Times.Once);
            _mockBookOrderRepository.Verify(r => r.GetBookOrderAsync(order.BookKey), Times.Once);
        }

        [Theory, AutoData]
        public void GivenValidCommand_WhenValidatingCommand_ThenSuccess(CreateBookOrderCommand command)
        {
            // Arrange
            var validator = new CreateBookOrderCommand.Validator();

            // Act
            var result = validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory, AutoData]
        public void GivenCommandMissingBookKey_WhenValidatingCommand_ThenFail(CreateBookOrderCommand command)
        {
            // Arrange
            command.BookKey = null;
            var validator = new CreateBookOrderCommand.Validator();

            // Act
            var result = validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors.FirstOrDefault(e => e.PropertyName == nameof(CreateBookOrderCommand.BookKey)).Should().NotBeNull();
        }
    }
}
