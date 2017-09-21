using Budgeter.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Helpers
{
    public class AccountsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
     

        public List<int> GetAccountsIdList()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            if (user != null)
            {
                var accounts = db.BankAccounts.Where(a => a.HouseholdId == user.HouseholdId).Select(a => a.Id).ToList();
                return accounts;
            }
            return new List<int>();
        }

        public string GetAccountName(int? id)
        {

            if (id != null)
            {
                var name = db.BankAccounts.FirstOrDefault(a => a.Id == id).Name;
                return name;
            }
            return "No Name Found";
        }

        public decimal GetAccountBalance(int? id)
        {

            if (id != null)
            {
                var startingAmount = db.BankAccounts.FirstOrDefault(a => a.Id == id).Balance;
                var totalRegularIncome = db.Transactions.Where(t => t.BankAccountId == id && t.IncomeExpense.Type == "Income" && t.Reconciled == false).Select(t => t.Amount).ToList().Sum();
                var totalReconciledIncome = db.Transactions.Where(t => t.BankAccountId == id && t.IncomeExpense.Type == "Income" && t.Reconciled).Select(t => t.ReconciledAmount).ToList().Sum();
                var totalRegularExpense = db.Transactions.Where(t => t.BankAccountId == id && t.IncomeExpense.Type == "Expense" && t.Reconciled == false).Select(t => t.Amount).ToList().Sum();
                var totalReconciledExpense = db.Transactions.Where(t => t.BankAccountId == id && t.IncomeExpense.Type == "Expense" && t.Reconciled).Select(t => t.ReconciledAmount).ToList().Sum();

                var balance = startingAmount + totalRegularIncome + totalReconciledIncome - totalRegularExpense - totalReconciledExpense;
                return balance;
            }
            return 0.00M;
        }

        public decimal AllAccountsBalance(List<int> BankAccountIds)
        {
            var total = 0.00M;
            foreach(var id in BankAccountIds)
            {
                total += GetAccountBalance(id);
            }
            return total;
        }

        public string OverdraftMonitor(int id)
        {
            var account = db.BankAccounts.FirstOrDefault(a => a.Id == id);
            var balance = 0.00M;
            var balLimit = account.WarningBalance;

            if (balLimit <= 0)
            { 
                balLimit = 50;
            }
           
            if (account != null)
            {
                balance = GetAccountBalance(account.Id);
                if (balance < 0)
                {
                    return "<h5 class = 'text-danger'>$" + balance + " left in account!</h5>";
                }

                else if (balance >= 0 && balance <= balLimit)
                {
                    return "<h5 class = 'text-warning'>$" + balance + " left in account!</h5>";
                }

                else
                {
                    return "<h5 class='text-info'>Account in good standing.</h5>";

                }

            }
            return "No account found!";
        }
    }
}