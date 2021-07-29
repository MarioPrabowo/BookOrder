using System;
using System.Collections.Generic;
using System.Text;

namespace BookOrder.Application.Common
{
    public class BusinessLogicException: Exception
    {
        public BusinessLogicException(string message) : base(message)
        {

        }
    }
}
