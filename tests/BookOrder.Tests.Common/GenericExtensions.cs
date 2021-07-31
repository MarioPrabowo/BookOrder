using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace BookOrder.Tests.Common
{
    public static class GenericExtensions
    {
        public static T DeepClone<T>(this T originalObject)
        {
            var serialisedObject = JsonSerializer.Serialize(originalObject);
            return JsonSerializer.Deserialize<T>(serialisedObject);
        }
    }
}
