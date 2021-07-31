using AutoFixture.Xunit2;
using FluentAssertions;
using System;
using Xunit;
using BookOrder.Tests.Common;
using BookOrder.Domain.Common;

namespace BookOrder.Domain.Tests
{
    public class OrderTests
    {
        [Theory, AutoData]
        public void GivenValidRequest_WhenCancelOrder_ThenOrderIsCancelledAndCoversPopulated(Order order, string covers)
        {
            // Arrange
            order.Status = OrderStatus.Ordered;
            order.Covers = null;
            var originalOrder = order.DeepClone();

            var cancelRequest = new CancelRequest
            {
                BookKey = order.BookKey,
                CancellationTime = DateTime.Now,
                Covers = covers
            };

            // Act
            var result = order.CancelOrder(cancelRequest);

            // Assert
            result.Should().Be(OrderOperationResult.ChangesMade);
            order.Should().BeEquivalentTo(originalOrder, options => options
                .Excluding(o => o.Status)
                .Excluding(o => o.Covers)
                .Excluding(o => o.UpdatedAt));
            order.Status.Should().Be(OrderStatus.Cancelled);
            order.Covers.Should().Be(covers);
            order.UpdatedAt.Should().Be(cancelRequest.CancellationTime);
        }

        [Theory, AutoData]
        public void GivenRequestBookKeyMismatch_WhenCancelOrder_ThenThrowBusinessLogicException(Order order, string covers, string randomKey)
        {
            // Arrange
            order.Status = OrderStatus.Ordered;
            order.Covers = null;
            var originalOrder = order.DeepClone();

            var cancelRequest = new CancelRequest
            {
                BookKey = randomKey,
                CancellationTime = DateTime.Now,
                Covers = covers
            };

            // Act + Assert
            Assert.Throws<BusinessLogicException>(() => order.CancelOrder(cancelRequest));
            order.Should().BeEquivalentTo(originalOrder);
        }

        [Theory, AutoData]
        public void GivenOrderAlreadyCancelled_WhenCancelOrder_ThenNoChangesRequired(Order order, string covers)
        {
            // Arrange
            order.Status = OrderStatus.Cancelled;
            order.Covers = null;
            var originalOrder = order.DeepClone();

            var cancelRequest = new CancelRequest
            {
                BookKey = order.BookKey,
                CancellationTime = DateTime.Now,
                Covers = covers
            };

            // Act
            var result = order.CancelOrder(cancelRequest);

            // Assert
            result.Should().Be(OrderOperationResult.NoChangesRequired);
            order.Should().BeEquivalentTo(originalOrder);
        }
    }
}
