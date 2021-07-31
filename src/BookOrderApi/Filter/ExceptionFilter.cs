using BookOrder.Domain.Common;
using FluentValidation;
using FluentValidation.AspNetCore;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookOrderApi.Filter
{
    public class ExceptionFilter : IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			if (context.Exception == null)
				return;

			context.ExceptionHandled = true;

			if (context.Exception is ValidationException)
			{
				var validationException = context.Exception as ValidationException;
				var validationResult = new ValidationResult(validationException.Errors);
				validationResult.AddToModelState(context.ModelState, string.Empty);
				context.Result = new BadRequestObjectResult(context.ModelState);
				return;
			}

			var statusCode = StatusCodes.Status500InternalServerError;

			if (context.Exception is BusinessLogicException)
			{
				statusCode = StatusCodes.Status400BadRequest;
			}

			context.Result = new JsonResult(context.Exception.Message)
			{
				StatusCode = statusCode
			};
		}
	}
}
