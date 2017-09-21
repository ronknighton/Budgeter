using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class SampleBudgetItem
    {
        public int Id { get; set; }       
        public string Name { get; set; }
        public decimal Amount { get; set; }

        //FK's
        public int? BudgetId { get; set; }
        public int CategoryId { get; set; }

        //Parent(s)
        public virtual Budget Budget { get; set; }
        public virtual SampleBudgetItemCategory Category { get; set; }
        //Children
        public virtual ICollection<Transaction> Transactions { get; set; }

        public SampleBudgetItem()
        {
            this.Transactions = new HashSet<Transaction>();
        }
    }
}