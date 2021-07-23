using BookOrder.Application.Interface.Client;
using BookOrder.Application.Interface.Persistence;
using BookOrder.Infrastructure.Client;
using BookOrder.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace BookOrder.Infrastructure
{
    public static class Bootstrap
    {
		public static IServiceCollection RegisterInMemoryDbServices([NotNullAttribute] this IServiceCollection serviceCollection)
		{
			if (!serviceCollection.Any(s => s.ServiceType.IsAssignableFrom(typeof(BookOrderDbContext))))
			{
				serviceCollection.AddDbContext<BookOrderDbContext>((IServiceProvider prov, DbContextOptionsBuilder optionsBuilder) =>
				{
					if (!optionsBuilder.IsConfigured)
					{
						optionsBuilder.UseInMemoryDatabase(databaseName: BookOrderDbContext.DbName);
					}
				});
			}
			serviceCollection.TryAddScoped<IBookOrderRepository, BookOrderRepository>();

			return serviceCollection;
		}

		public static IServiceCollection RegisterApiClients([NotNullAttribute] this IServiceCollection serviceCollection)
		{
			serviceCollection.AddHttpClient<IBookApiClient, OpenLibraryBookApiClient>(c =>
			{
				c.DefaultRequestHeaders.Accept.Clear();
			});

			return serviceCollection;
		}
	}
}
