using BookOrder.Application.Common;
using BookOrder.Application.Interface.Service;
using BookOrder.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BookOrder.Application
{
    public static class Bootstrap
    {
        public static IServiceCollection RegisterApplicationLayer([NotNullAttribute] this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMediatR(typeof(Bootstrap).Assembly);
            serviceCollection.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            serviceCollection.AddAutoMapper(typeof(Bootstrap));

            serviceCollection.TryAddSingleton<IBookAvailabilityService, BookAvailabilityService>();

            return serviceCollection;
        }
    }
}
