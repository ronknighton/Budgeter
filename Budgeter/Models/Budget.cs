using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class Budget
    {
        public int Id { get; set; }

        [Required]        
        [StringLength(30, ErrorMessage = "Please enter at least 3 characters and a maximum of 30", MinimumLength = 3)]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public  DateTimeOffset Created { get; set; }

        [DisplayName("Monthly Budget")]
        public decimal Amount { get; set; }

        public bool Active { get; set; }
        //FK's
        public int HouseholdId { get; set; }

        //Parent(s)
        public virtual Household Household { get; set; }

        //Children
        public virtual ICollection<BudgetItem> BudgetItems {get;set;}
        public virtual ICollection<BudgetItemCategory> Categories { get; set; }

        public Budget()
        {
            this.BudgetItems = new HashSet<BudgetItem>();
            this.Categories = new HashSet<BudgetItemCategory>();
        }
    }
}