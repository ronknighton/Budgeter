using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class IncomeExpense
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }     

        public IncomeExpense()
        {          
            this.Transactions = new HashSet<Transaction>();
        }
    }
}