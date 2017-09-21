using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Required]      
        public string Description { get; set; }

        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTimeOffset Created { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public bool Reconciled { get; set; }
        public decimal ReconciledAmount { get; set; }



        //FK's
        public string EnteredById { get; set; }
        public int? BudgetItemId { get; set; }
        public int BankAccountId { get; set; }
        public int IncomeExpenseId { get; set; }


        //Parent(s)
        public virtual ApplicationUser EnteredBy { get; set; }
        public virtual BudgetItem BudgetItem { get; set; }
        public virtual BankAccount BankAccount { get; set; }
        public virtual IncomeExpense IncomeExpense { get; set; }
        
    }
}