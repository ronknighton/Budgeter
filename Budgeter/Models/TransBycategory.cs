using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class TransByCategory
    {
        public string CategoryName { get; set; }
        public string BudgetItemName { get; set; }
        public Transaction transaction { get; set; }
    }
}