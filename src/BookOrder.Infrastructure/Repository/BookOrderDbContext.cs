using Microsoft.EntityFrameworkCore;
using System;
using BookOrder.Domain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BookOrder.Infrastructure.Repository
{
    public class BookOrderDbContext : DbContext
	{
		public static readonly string DbName = "BookOrderInMemoryDB";

		public virtual DbSet<Order> BookOrder { get; set; }

		public BookOrderDbContext(DbContextOptions<BookOrderDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Order>(entity =>
			{
				entity.HasKey(e => e.BookKey);
			});

			ConvertDateFieldsToUtc(modelBuilder);
		}

		private void ConvertDateFieldsToUtc(ModelBuilder builder)
		{
			var dateTimeValueConverter = new ValueConverter<DateTime, DateTime>(
						   v => v,
						   v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

			var nullableDateTimeValueConverter = new ValueConverter<DateTime?, DateTime?>(
							v => v,
							v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

			foreach (var entity in builder.Model.GetEntityTypes())
			{
				foreach (var property in entity.GetProperties())
				{
					if (property.ClrType == typeof(DateTime))
					{
						property.SetValueConverter(dateTimeValueConverter);
					}

					if (property.ClrType == typeof(DateTime?))
					{
						property.SetValueConverter(nullableDateTimeValueConverter);
					}
				}
			}
		}
	}
}
