using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class BudgetItem
    {
        public int Id { get; set; }       
        public string Name { get; set; }       
        public decimal Amount { get; set; }
        public bool Active { get; set; }

        //FK's
        public int BudgetId { get; set; }
        public int CategoryId { get; set; }

        //Parent(s)
        public virtual Budget Budget { get; set; }
        public virtual BudgetItemCategory Category { get; set; }

        //Children
        public virtual ICollection<Transaction> Transactions { get; set; }

        public BudgetItem()
        {
            this.Transactions = new HashSet<Transaction>();
        }
   
    }
}