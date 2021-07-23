using BookOrder.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BookOrder.Application.Interface.Persistence
{
    public interface IBookOrderRepository
    {
        Task<Order> GetBookOrderAsync(string key);
        Task SaveBookOrderAsync(Order order);
    }
}
