using BookOrder.Application.Interface.Persistence;
using BookOrder.Domain;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BookOrder.Infrastructure.Repository
{
    public class BookOrderRepository : IBookOrderRepository
    {
        private BookOrderDbContext _ctx;

        public BookOrderRepository(BookOrderDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<Order> GetBookOrderAsync(string key)
        {
            return await (
                from o in _ctx.BookOrder
                where o.BookKey == key
                select o
            ).AsNoTracking().FirstOrDefaultAsync();
        }

        public async Task SaveBookOrderAsync(Order order)
        {
            if (order.CreatedAt == order.UpdatedAt)
            {
                _ctx.Add(order);
            }
            else
            {
                _ctx.Update(order);    
            }
            
            await _ctx.SaveChangesAsync();

        }
    }
}
