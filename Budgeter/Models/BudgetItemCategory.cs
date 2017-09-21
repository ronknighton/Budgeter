using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class BudgetItemCategory
    {
        public int Id { get; set; }         
        public string Name { get; set; }
        public int? BudgetId { get; set; }

        //Paret(s)
        public virtual Budget Budget { get; set; }

        //Children
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }

        public BudgetItemCategory()
        {
            this.BudgetItems = new HashSet<BudgetItem>();
        }

        
    }
}