using System;
using System.Collections.Generic;
using System.Text;

namespace BookOrder.Domain.Common
{
    public class BusinessLogicException: Exception
    {
        public BusinessLogicException(string message) : base(message)
        {

        }
    }
}
