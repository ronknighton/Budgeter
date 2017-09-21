using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class SampleBudgetItemCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? BudgetId { get; set; }

        //Parent(s)
        public virtual Budget Budget { get; set; }

        public virtual ICollection<SampleBudgetItem> BudgetItems { get; set; }

        public SampleBudgetItemCategory()
        {
            this.BudgetItems = new HashSet<SampleBudgetItem>();
        }
    }
}