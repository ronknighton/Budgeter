using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Budgeter.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {


        public string FirstName { get; set; }


        public string LastName { get; set; }

        [DisplayName("NickName")]
        public string FullName { get; set; }

        [DisplayName("Household Creator")]
        public bool Protected { get; set; }

        [DisplayName("Can Edit Household")]
        public bool EditHousehold { get; set; }

        [DisplayName("Can Edit Budget")]
        public bool EditBudget { get; set; }    

        [DisplayName("Can Edit Transactions")]
        public bool EditTransactions { get; set; }

        [DisplayName("Can Add Bank Accounts")]
        public bool AddBankAccounts { get; set; }       
        public string DisplayImage { get; set; }

        //FK's
        public int? HouseholdId { get; set; }

        //Parents
        public virtual Household Household { get; set; }

        //Children
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Invitation> Invitations { get; set; }

        public ApplicationUser()
        {
            this.Transactions = new HashSet<Transaction>();
            this.BankAccounts = new HashSet<BankAccount>();
            this.Notifications = new HashSet<Notification>();
            this.Invitations = new HashSet<Invitation>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<AccountType> AccountTypes { get; set; }
        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetItem> BudgetItems { get; set; }
        public DbSet<BudgetItemCategory> BudgetItemCategories { get; set; }
        public DbSet<SampleBudgetItemCategory> SampleBudgetItemCategories { get; set; }
        public DbSet<Household> Households { get; set; }
        public DbSet<IncomeExpense> IncomeExpenses { get; set; }
        public DbSet<SampleBudgetItem> SampleBudgetItems { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<UploadedTransaction> UploadedTransactions { get; set; }
    }
}