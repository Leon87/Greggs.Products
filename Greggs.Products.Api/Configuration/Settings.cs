using System;
using System.Collections.Generic;

namespace Greggs.Products.Api.Configuration
{
    public class Settings
    {
        public Dictionary<string, decimal> CurrencyValues { get; set; } = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
    }
}
