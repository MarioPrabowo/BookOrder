using AutoFixture.Xunit2;
using BookOrder.Application.Commands;
using BookOrder.Application.DTO;
using BookOrder.Application.Interface.Service;
using BookOrder.Application.Mapping;
using BookOrder.Domain;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace BookOrder.Application.Tests.Mapping
{
    public class BookApiResponseToOrderConverterTests
    {
        private readonly Mock<IValidator<BookApiResponse>> _mockValidator;
        private readonly IDateTimeService _mockedDateTimeService;
        private readonly BookApiResponseToOrderConverter _bookApiResponseToOrderConverter;
        private readonly DateTime _now;

        public BookApiResponseToOrderConverterTests()
        {
            _mockValidator = new Mock<IValidator<BookApiResponse>>();
            _now = DateTime.UtcNow;
            _mockedDateTimeService = Mock.Of<IDateTimeService>(s => s.GetUtcNow() == _now);
            _bookApiResponseToOrderConverter = new BookApiResponseToOrderConverter(_mockValidator.Object, _mockedDateTimeService);
        }

        [Theory, AutoData]
        public void GivenValidSource_WhenConvert_ThenReturnValidOrder(BookApiResponse source)
        {
            // Arrange
            var expectedResult = new Order
            {
                BookKey = source.Key,
                Title = source.Title,
                Authors = string.Join("; ", source.Authors.Select(a => a.Name)),
                Status = OrderStatus.Ordered,
                CreatedAt = _now,
                UpdatedAt = _now
            };
            RulesetValidatorSelector rulesetValidatorSelector = null;
            _mockValidator.Setup(v => v.Validate(It.IsAny<IValidationContext>()))
                .Callback<IValidationContext>((c) => rulesetValidatorSelector = (c?.Selector as RulesetValidatorSelector));


            // Act
            var result = _bookApiResponseToOrderConverter.Convert(source, null, null);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _mockValidator.Verify(v => v.Validate(It.IsAny<ValidationContext<BookApiResponse>>()), Times.Once);
            rulesetValidatorSelector.Should().NotBeNull();
            rulesetValidatorSelector.RuleSets.Should().HaveCount(2);
            rulesetValidatorSelector.RuleSets.Should().Contain(nameof(CreateBookOrderCommand));
            rulesetValidatorSelector.RuleSets.Should().Contain("default");
        }
    }
}
