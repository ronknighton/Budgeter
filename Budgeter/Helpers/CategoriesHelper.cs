using Budgeter.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budgeter.Helpers
{
    public class CategoriesHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public List<BudgetItemCategory> GetCategoriesList()
        {
            var user = db.Users.Find(HttpContext.Current.User.Identity.GetUserId());
            var houseHold = new Household();
            var budget = new Budget();
            var categories = new List<BudgetItemCategory>();
            if (user != null)
            {
                var household = db.Households.FirstOrDefault(h => h.Id == user.HouseholdId);
                if (household != null)
                {
                    budget = db.Budgets.FirstOrDefault(b => b.HouseholdId == household.Id && b.Active);

                    if (budget != null)
                    {
                        //categories = db.BudgetItemCategories.Where(b => b.BudgetId == budget.Id).ToList();
                        categories = budget.Categories.ToList();
                    }
                }
                return categories;
            }
            return new List<BudgetItemCategory>();
        }
    }
}