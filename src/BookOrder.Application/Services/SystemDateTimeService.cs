using BookOrder.Application.Interface.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookOrder.Application.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}
