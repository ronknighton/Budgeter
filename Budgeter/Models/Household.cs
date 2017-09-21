using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budgeter.Models
{
    public class Household
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$", ErrorMessage = "Numbers and special characters are not allowed in the last name.")]
        [StringLength(30, ErrorMessage = "Please enter at least 5 characters and a maximum of 30!", MinimumLength = 5)]
        [DisplayName("Household Name")]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTimeOffset Created { get; set; }

        //Children
        public virtual ICollection<BankAccount> BankAccounts {get;set;}
        public virtual ICollection<Budget> Budgets { get; set; }
        public ICollection<ApplicationUser> Members { get; set; }
        public ICollection<Invitation> Invitations { get; set; }

        public Household()
        {
            this.BankAccounts = new HashSet<BankAccount>();
            this.Budgets = new HashSet<Budget>();
            this.Members = new HashSet<ApplicationUser>();
            this.Invitations = new HashSet<Invitation>();
        }
    }
}