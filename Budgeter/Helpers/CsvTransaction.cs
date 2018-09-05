using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Helpers
{
    public class CsvTransaction
    {
        public DateTimeOffset Date { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
    }
}