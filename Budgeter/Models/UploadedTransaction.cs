using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class UploadedTransaction
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset Created { get; set; }
        public bool Saved { get; set; }
        public DateTimeOffset UploadDateTime { get; set; }

        public int BankAccountId { get; set; }
        public int IncomeExpenseId { get; set; }
        public int? BudgetItemId { get; set; }

        public virtual BankAccount BankAccount { get; set; }
        public virtual IncomeExpense IncomeExpense { get; set; }
        public virtual BudgetItem BudgetItem { get; set; }
    }
}