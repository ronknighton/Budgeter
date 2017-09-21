using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class BankAccount
    {
        public int Id { get; set; }

        [Required]        
        public string Name { get; set; }

        [Required]
        [DisplayName("Starting Balance")]
        public decimal Balance { get; set; }
        [DisplayName("Warning Limit")]
        public decimal WarningBalance { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Numbers and special characters are not allowed in the last name.")]
        public string BankName { get; set; }

        public int? AccountNumber { get; set; }


        //FK's
        public int HouseholdId { get; set; }
        public int AccountTypeId { get; set; }
        public string AccountOwnerId { get; set; }

        //Parents(s)
        public virtual Household Household { get; set; }
        public virtual AccountType AccountType { get; set; }
        public virtual ApplicationUser AccountOwner { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }

        public BankAccount()
        {
            this.Transactions = new HashSet<Transaction>();
        }
    }
}