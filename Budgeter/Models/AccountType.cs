using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class AccountType
    {
        public int Id { get; set; }
        
        public string Type { get; set; }

        public virtual ICollection<BankAccount> BankAccounts { get; set; }

        public AccountType()
        {
            this.BankAccounts = new HashSet<BankAccount>();
        }
    }
}