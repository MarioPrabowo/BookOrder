using BookOrder.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookOrder.Tests.Common
{
	public class DbContextCreator
	{
		private readonly string _dbName;

		public DbContextCreator()
		{
			// Unique DB name per tests to avoid data getting mixed up
			_dbName = Guid.NewGuid().ToString();
		}

		public void Setup(IServiceCollection services)
		{
			// Remove the app's DbContext registration.
			services.Remove<DbContextOptions<BookOrderDbContext>>();

			// Add ApplicationDbContext using unique in-memory database for testing.
			services.AddDbContext<BookOrderDbContext>((IServiceProvider prov, DbContextOptionsBuilder optionsBuilder) =>
			{
				if (!optionsBuilder.IsConfigured)
				{
					optionsBuilder.UseInMemoryDatabase(databaseName: _dbName);
				}
			});
		}

		public BookOrderDbContext CreateDbContext()
		{
			DbContextOptions<BookOrderDbContext> options = new DbContextOptionsBuilder<BookOrderDbContext>()
				.UseInMemoryDatabase(databaseName: _dbName)
				.Options;

			return new BookOrderDbContext(options);
		}
	}
}
