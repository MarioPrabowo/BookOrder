using System;
using System.Collections.Generic;
using System.Text;

namespace BookOrder.Application.Interface.Service
{
    public interface IDateTimeService
    {
        DateTime GetUtcNow();
    }
}
