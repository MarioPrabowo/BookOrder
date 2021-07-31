using AutoFixture.Xunit2;
using BookOrder.Application.DTO;
using BookOrder.Application.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Configuration;
using Xunit;

namespace BookOrder.Application.Services.Tests
{
    public class BookAvailabilityServiceTests
    {
        private readonly Mock<IOptions<BookAvailabilityOption>> _mockOption;

        public BookAvailabilityServiceTests()
        {
            _mockOption = new Mock<IOptions<BookAvailabilityOption>>();
        }

        [Theory, AutoData]
        public void GivenAuthorIsInAvailabilityList_WhenCheckingIsBookAvailable_ThenReturnTrue(BookAvailabilityOption config, string authorName)
        {
            // Arrange
            config.Authors.Add(authorName);
            _mockOption.Setup(o => o.Value).Returns(config);
            var service = new BookAvailabilityService(_mockOption.Object);

            // Act
            var result = service.IsBookAvailable(authorName);

            // Assert
            result.Should().BeTrue();
        }

        [Theory, AutoData]
        public void GivenAuthorIsNotInAvailabilityList_WhenCheckingIsBookAvailable_ThenReturnFalse(BookAvailabilityOption config, string authorName)
        {
            // Arrange
            _mockOption.Setup(o => o.Value).Returns(config);
            var service = new BookAvailabilityService(_mockOption.Object);

            // Act
            var result = service.IsBookAvailable(authorName);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GivenAuthorAvailabilityListIsEmpty_WhenInitialisingBookAvailabilityService_ThenThrowException()
        {
            // Arrange
            _mockOption.Setup(o => o.Value).Returns(new BookAvailabilityOption());

            // Act + Assert
            Assert.Throws<ConfigurationErrorsException>(() => new BookAvailabilityService(_mockOption.Object));
        }
    }
}
