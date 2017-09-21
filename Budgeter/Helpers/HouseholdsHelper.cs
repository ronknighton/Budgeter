using Budgeter.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Budgeter.Helpers
{
    public class HouseholdsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UsersHelper usersHelper = new UsersHelper();

        public string GetCreatorName(int hId)
        {
            var household = db.Households.FirstOrDefault(h => h.Id == hId);
            var name = "";
            if (household != null)
            {
                name = db.Users.FirstOrDefault(u => u.Protected && u.HouseholdId == hId).FullName;
            }
            else
            {
                name = "Unknown";
            }

            return name;
        }


        public void AddMemberByEmail(string userName, int hId)
        {
            var user = db.Users.FirstOrDefault(u => u.UserName == userName);
            var household = db.Households.FirstOrDefault(h => h.Id == hId);

            if (user != null && household != null)
            {
                user.HouseholdId = hId;
                household.Members.Add(user);
                usersHelper.AddToRole(user.Id, "Member");

                //db.Entry(household).State = EntityState.Modified;
                //db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();


            }
        }

        public void AddMemberById(string Id, int hId)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == Id);
            var household = db.Households.FirstOrDefault(h => h.Id == hId);

            if (user != null && household != null)
            {
                user.HouseholdId = hId;
                household.Members.Add(user);
                usersHelper.AddToRole(user.Id, "Member");
                //db.Entry(household).State = EntityState.Modified;
                //db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();


            }
        }

        public void RemoveFromHousehold(string userId, int hId)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            var household = db.Households.FirstOrDefault(h => h.Id == hId);

            household.Members.Remove(user);
            user.HouseholdId = null;
            usersHelper.AddToRole(user.Id, "Guest");
            db.SaveChanges();

        }

        public string GetHouseholdName()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            if (user != null)
            {
                var household = db.Households.FirstOrDefault(h => h.Id == user.HouseholdId);

                if (household != null)
                {
                    return household.Name + " Dashboard";
                }
            }
            return "MMS Dashboard";
        }

        public string CreatHouseholdData()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            if (user != null)
            {
                //Check for registered user
                //Check if user has household
                if (user.HouseholdId == null)
                {
                    return "You must create a household first";
                }

                //Get user household and delete existing bank accounts, budget, and categories, which will also delete items and transactions
                Household household = db.Households.Include(h => h.Members).FirstOrDefault(h => h.Id == user.HouseholdId);
                var bankAccounts = db.BankAccounts.Where(ba => ba.HouseholdId == user.HouseholdId).ToList();
                if (bankAccounts.Count > 0)
                {
                    foreach (var account in bankAccounts)
                    {
                        db.BankAccounts.Remove(account);
                    }
                }
                var budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == user.HouseholdId);
                if (budget != null)
                {
                    var budgetItems = db.BudgetItems.Where(b => b.BudgetId == budget.Id).ToList();
                    var categories = db.BudgetItemCategories.Where(b => b.BudgetId == budget.Id).ToList();
                    if (categories.Count > 0)
                    {
                        foreach (var category in categories)
                        {
                            db.BudgetItemCategories.Remove(category);
                        }
                    }

                    db.SaveChanges();
                }

                //If no budget exists yet
                if (budget == null)
                {
                    budget = new Budget();
                    budget.HouseholdId = (int)user.HouseholdId;
                    budget.Name = "Budget for " + household.Name;
                    budget.Amount = 3000;
                    budget.Created = DateTimeOffset.Now;
                    budget.Active = true;
                    db.Budgets.Add(budget);
                    db.SaveChanges();
                }

                //Start to create new stuff
                var sampleCategories = db.SampleBudgetItemCategories;
                foreach (var c in sampleCategories)
                {
                    var category = new BudgetItemCategory();
                    category.Name = c.Name;
                    category.BudgetId = budget.Id;
                    db.BudgetItemCategories.Add(category);
                }
                db.SaveChanges();

                var BudgetItems = db.SampleBudgetItems.Include(b => b.Category).ToList();
                foreach (var bi in BudgetItems)
                {
                    var budgetItem = new BudgetItem();
                    int catNameId = db.BudgetItemCategories.FirstOrDefault(b => b.Name == bi.Category.Name && b.BudgetId == budget.Id).Id;
                    budgetItem.Name = bi.Name;
                    budgetItem.Amount = bi.Amount;
                    budgetItem.CategoryId = catNameId;
                    budgetItem.BudgetId = budget.Id;
                    budgetItem.Active = true;
                    db.BudgetItems.Add(budgetItem);

                }
                db.SaveChanges();

                var SavedBudgetItems = db.BudgetItems.Include(b => b.Category).ToList();

                for (int ba = 1; ba <= 4; ba++)
                {
                    var bankAccount = new BankAccount();
                    bankAccount.Name = "Account " + ba + " for household " + household.Name;
                    bankAccount.Balance = 3000;
                    bankAccount.WarningBalance = 100;
                    bankAccount.BankName = "First Bank";
                    bankAccount.AccountNumber = 123450 + ba;
                    bankAccount.HouseholdId = household.Id;
                    bankAccount.AccountTypeId = db.AccountTypes.FirstOrDefault(a => a.Type == "Checking").Id;
                    bankAccount.AccountOwnerId = user.Id;
                    db.BankAccounts.Add(bankAccount);
                    db.SaveChanges();

                    Transaction transaction;
                    for (int x = 0; x <= 36; x++)
                    {
                        decimal total = 0.00M;
                        foreach (var bi in SavedBudgetItems)
                        {
                            var count = 1;
                            transaction = new Transaction();
                            transaction.Description = "Trans " + count + " for " + bankAccount.Name;
                            transaction.Created = DateTimeOffset.Now.AddMonths(-x);
                            transaction.Amount = Math.Abs((bi.Amount * .25M) + count - x);
                            transaction.EnteredById = user.Id;
                            transaction.BudgetItemId = bi.Id;
                            transaction.BankAccountId = bankAccount.Id;
                            transaction.IncomeExpenseId = db.IncomeExpenses.FirstOrDefault(i => i.Type == "Expense").Id;
                            total += transaction.Amount;
                            db.Transactions.Add(transaction);
                        }

                         db.SaveChanges();

                        transaction = new Transaction();
                        transaction.Description = "Deposit for " + bankAccount.Name;
                        transaction.Created = DateTimeOffset.Now.AddMonths(-x);
                        transaction.Amount = total * 1.05M;
                        transaction.EnteredById = user.Id;
                        transaction.BudgetItemId = null;
                        transaction.BankAccountId = bankAccount.Id;
                        transaction.IncomeExpenseId = db.IncomeExpenses.FirstOrDefault(i => i.Type == "Income").Id;

                        db.Transactions.Add(transaction);

                        db.SaveChanges();
                    }

                   
                }


                return "Data has been generated";

            }

            return "Something went wrong!";
        }
    }
}