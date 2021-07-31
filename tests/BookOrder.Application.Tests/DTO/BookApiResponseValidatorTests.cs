using AutoFixture.Xunit2;
using BookOrder.Application.Commands;
using BookOrder.Application.DTO;
using BookOrder.Application.Interface.Service;
using FluentAssertions;
using FluentValidation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace BookOrder.Application.Tests.DTO
{
    public class BookApiResponseValidatorTests
    {
        private readonly Mock<IBookAvailabilityService> _mockBookAvailabilityService;
        private readonly BookApiResponseValidator _bookApiResponseValidator;

        public BookApiResponseValidatorTests()
        {
            _mockBookAvailabilityService = new Mock<IBookAvailabilityService>();
            _bookApiResponseValidator = new BookApiResponseValidator(_mockBookAvailabilityService.Object);
        }

        [Theory, AutoData]
        public void GivenValidBookApiResponse_WhenValidateDefaultRule_ThenSuccess(BookApiResponse bookApiResponse)
        {
            // Arrange
            _mockBookAvailabilityService.Setup(s => s.IsBookAvailable(It.IsAny<string>())).Returns(true);

            // Act
            var result = _bookApiResponseValidator.Validate(bookApiResponse);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory, AutoData]
        public void GivenBookApiResponseMissingKey_WhenValidateDefaultRule_ThenFail(BookApiResponse bookApiResponse)
        {
            // Arrange
            _mockBookAvailabilityService.Setup(s => s.IsBookAvailable(It.IsAny<string>())).Returns(true);
            bookApiResponse.Key = null;

            // Act
            var result = _bookApiResponseValidator.Validate(bookApiResponse);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors.FirstOrDefault(e => e.PropertyName == nameof(BookApiResponse.Key)).Should().NotBeNull();
        }

        [Theory, AutoData]
        public void GivenValidBookApiResponse_WhenValidateCreateBookOrderRule_ThenSuccess(BookApiResponse bookApiResponse)
        {
            // Arrange
            _mockBookAvailabilityService.Setup(s => s.IsBookAvailable(It.IsAny<string>())).Returns(true);

            // Act
            var result = _bookApiResponseValidator.Validate(bookApiResponse, options => options.IncludeRuleSets(nameof(CreateBookOrderCommand)));

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory, AutoData]
        public void GivenBookApiResponseMissingTitle_WhenValidatCreateBookOrderRule_ThenFail(BookApiResponse bookApiResponse)
        {
            // Arrange
            _mockBookAvailabilityService.Setup(s => s.IsBookAvailable(It.IsAny<string>())).Returns(true);
            bookApiResponse.Title = null;

            // Act
            var result = _bookApiResponseValidator.Validate(bookApiResponse, options => options.IncludeRuleSets(nameof(CreateBookOrderCommand)));

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors.FirstOrDefault(e => e.PropertyName == nameof(BookApiResponse.Title)).Should().NotBeNull();
        }

        [Theory, AutoData]
        public void GivenBookApiResponseMissingAuthors_WhenValidatCreateBookOrderRule_ThenFail(BookApiResponse bookApiResponse)
        {
            // Arrange
            _mockBookAvailabilityService.Setup(s => s.IsBookAvailable(It.IsAny<string>())).Returns(true);
            bookApiResponse.Authors = null;

            // Act
            var result = _bookApiResponseValidator.Validate(bookApiResponse, options => options.IncludeRuleSets(nameof(CreateBookOrderCommand)));

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors.FirstOrDefault(e => e.PropertyName == nameof(BookApiResponse.Authors)).Should().NotBeNull();
        }

        [Theory, AutoData]
        public void GivenBookApiResponseAuthorsNotAvailable_WhenValidatCreateBookOrderRule_ThenFail(BookApiResponse bookApiResponse)
        {
            // Arrange
            _mockBookAvailabilityService.Setup(s => s.IsBookAvailable(It.IsAny<string>())).Returns(false);

            // Act
            var result = _bookApiResponseValidator.Validate(bookApiResponse, options => options.IncludeRuleSets(nameof(CreateBookOrderCommand)));

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors.FirstOrDefault(e => e.PropertyName == nameof(BookApiResponse.Authors)).Should().NotBeNull();
        }

        [Theory, AutoData]
        public void GivenAuthorNameIsEmpty_WhenValidatCreateBookOrderRule_ThenFail(BookApiResponse bookApiResponse)
        {
            // Arrange
            _mockBookAvailabilityService.Setup(s => s.IsBookAvailable(It.IsAny<string>())).Returns(true);
            bookApiResponse.Authors[0].Name = null;

            // Act
            var result = _bookApiResponseValidator.Validate(bookApiResponse, options => options.IncludeRuleSets(nameof(CreateBookOrderCommand)));

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors.FirstOrDefault(e => e.PropertyName == "Authors[0].Name").Should().NotBeNull();
        }

        [Theory, AutoData]
        public void GivenValidBookApiResponse_WhenValidateCancelBookOrderRule_ThenSuccess(BookApiResponse bookApiResponse)
        {
            // Arrange
            _mockBookAvailabilityService.Setup(s => s.IsBookAvailable(It.IsAny<string>())).Returns(true);

            // Act
            var result = _bookApiResponseValidator.Validate(bookApiResponse, options => options.IncludeRuleSets(nameof(CancelBookOrderCommand)));

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory, AutoData]
        public void GivenBookApiResponseMissingCovers_WhenValidatCancelBookOrderRule_ThenFail(BookApiResponse bookApiResponse)
        {
            // Arrange
            _mockBookAvailabilityService.Setup(s => s.IsBookAvailable(It.IsAny<string>())).Returns(true);
            bookApiResponse.Covers = null;

            // Act
            var result = _bookApiResponseValidator.Validate(bookApiResponse, options => options.IncludeRuleSets(nameof(CancelBookOrderCommand)));

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors.FirstOrDefault(e => e.PropertyName == nameof(BookApiResponse.Covers)).Should().NotBeNull();
        }
    }
}
