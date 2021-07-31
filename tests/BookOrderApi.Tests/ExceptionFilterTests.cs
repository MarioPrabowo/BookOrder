using AutoFixture.Xunit2;
using BookOrder.Application.Queries;
using BookOrder.Domain.Common;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BookOrderApi.Tests
{
    public class ExceptionFilterTests : IClassFixture<WebApplicationFactory<Startup>>
	{
		private readonly WebApplicationFactory<Startup> _factory;
		private readonly HttpClient _client;
		private readonly Mock<IMediator> _mediator;
		private readonly HttpRequestMessage _message;


		public ExceptionFilterTests(WebApplicationFactory<Startup> factory)
		{
			_mediator = new Mock<IMediator>();

			_factory = factory;
			_client = _factory.WithWebHostBuilder(builder =>
			{
				builder.ConfigureServices(services =>
				{
					services.AddSingleton<IMediator>(_mediator.Object);
				});
			}).CreateClient();
			_message = new HttpRequestMessage(HttpMethod.Get, "BookOrder/bookKey");
		}

		[Theory, AutoData]
		public async Task GivenValidationException_WhenHittingEndPoint_ThenReturnBadRequestWithValidationErrors(string field, string error)
		{
			// Arrange
			var validationFailures = new List<ValidationFailure> { new ValidationFailure(field, error) };
			_mediator.Setup(m => m.Send(It.IsAny<GetBookOrderQuery>(), It.IsAny<CancellationToken>())).ThrowsAsync(new ValidationException(validationFailures));

			// Act
			var response = await _client.SendAsync(_message);
			var returned = await response.Content.ReadAsAsync<Dictionary<string, List<string>>>();

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
			returned.Values.Should().ContainSingle();
			returned.Keys.First().Should().Be(field);
			returned[field].First().Should().Be(error);
		}

		[Theory, AutoData]
		public async Task GivenBusinessLogicException_WhenHittingEndPoint_ThenReturnBadRequestExceptionMessage(BusinessLogicException businessLogicException)
		{
			// Arrange
			_mediator.Setup(m => m.Send(It.IsAny<GetBookOrderQuery>(), It.IsAny<CancellationToken>())).ThrowsAsync(businessLogicException);

			// Act
			var response = await _client.SendAsync(_message);
			var returned = await response.Content.ReadAsAsync<string>();

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
			returned.Should().Be(businessLogicException.Message);
		}

		[Theory, AutoData]
		public async Task GivenOtherException_WhenHittingEndPoint_ThenReturnInternalServerError(Exception exception)
		{
			// Arrange
			_mediator.Setup(m => m.Send(It.IsAny<GetBookOrderQuery>(), It.IsAny<CancellationToken>())).ThrowsAsync(exception);

			// Act
			var response = await _client.SendAsync(_message);
			var returned = await response.Content.ReadAsAsync<string>();

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
			returned.Should().Be(exception.Message);
		}
	}
}
