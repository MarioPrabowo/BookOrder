using AutoFixture.Xunit2;
using BookOrder.Domain;
using BookOrder.Infrastructure.Repository;
using BookOrder.Tests.Common;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace BookOrder.Infratructure.Tests
{
    public class BookOrderRepositoryTests
    {
        private readonly DbContextCreator _dbContextCreator;
        private readonly BookOrderRepository _repo;

        public BookOrderRepositoryTests()
        {
            _dbContextCreator = new DbContextCreator();
            _repo = new BookOrderRepository(_dbContextCreator.CreateDbContext());
        }

        [Theory, AutoData]
        public async Task GivenRecordWithBookKeyExists_WhenGetBookOrder_ThenReturnBookOrder(Order existingOrder)
        {
            // Arrange
            using (var ctx = _dbContextCreator.CreateDbContext())
            {
                ctx.Add(existingOrder);
                await ctx.SaveChangesAsync();
            }

            // Act
            var result = await _repo.GetBookOrderAsync(existingOrder.BookKey);

            // Assert
            result.Should().BeEquivalentTo(existingOrder);
        }

        [Theory, AutoData]
        public async Task GivenRecordWithBookKeyDoesNotExist_WhenGetBookOrder_ThenReturnNull(string nonExistingBookKey)
        {
            // Arrange

            // Act
            var result = await _repo.GetBookOrderAsync(nonExistingBookKey);

            // Assert
            result.Should().BeNull();
        }

        [Theory, AutoData]
        public async Task GivenRecordCreatedAtAndUpdatedAtAreTheSame_WhenSaveBookOrder_ThenTreatItAsNewRecord(Order newOrder)
        {
            // Arrange
            newOrder.UpdatedAt = newOrder.CreatedAt;

            // Act
            await _repo.SaveBookOrderAsync(newOrder);

            // Assert
            using (var ctx = _dbContextCreator.CreateDbContext())
            {
                var orderInDb = await ctx.BookOrder.FirstOrDefaultAsync(o => o.BookKey == newOrder.BookKey);
                orderInDb.Should().BeEquivalentTo(newOrder);
            }
        }

        [Theory, AutoData]
        public async Task GivenRecordCreatedAtAndUpdatedAtAreDifferent_WhenSaveBookOrder_ThenTreatItAsUpdatedRecord(Order existingOrder, DateTime updatedTime)
        {
            // Arrange
            using (var ctx = _dbContextCreator.CreateDbContext())
            {
                ctx.Add(existingOrder);
                await ctx.SaveChangesAsync();
            }
            var updatedOrder = existingOrder.DeepClone();
            updatedOrder.UpdatedAt = updatedTime;

            // Act
            await _repo.SaveBookOrderAsync(updatedOrder);

            // Assert
            using (var ctx = _dbContextCreator.CreateDbContext())
            {
                var orderInDb = await ctx.BookOrder.FirstOrDefaultAsync(o => o.BookKey == updatedOrder.BookKey);
                orderInDb.Should().BeEquivalentTo(updatedOrder);
            }
        }
    }
}
