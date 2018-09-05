namespace Budgeter.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Budgeter.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Budgeter.Models.ApplicationDbContext context)
        {
            //Debugger in another instance of VS
            if (System.Diagnostics.Debugger.IsAttached == false)
                System.Diagnostics.Debugger.Launch();

            // Creat instance of role manager
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            #region Roles
            //Check if role exists. If not, create it
            //Admin
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            // Head of Household
            if (!context.Roles.Any(r => r.Name == "HOH"))
            {
                roleManager.Create(new IdentityRole { Name = "HOH" });
            }

            // Member
            if (!context.Roles.Any(r => r.Name == "Member"))
            {
                roleManager.Create(new IdentityRole { Name = "Member" });
            }

            //Guest
            if (!context.Roles.Any(r => r.Name == "Guest"))
            {
                roleManager.Create(new IdentityRole { Name = "Guest" });
            }
            #endregion

            #region Users
            //Create instance of User manager
            var userManger = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            // Add Admin
            if (!context.Users.Any(u => u.Email == "MMSadministrator@mailinator.com"))
            {
                userManger.Create(new ApplicationUser
                {
                    UserName = "MMSadministrator@mailinator.com",
                    Email = "MMSadministrator@mailinator.com",
                    FirstName = "MMS",
                    LastName = "Administrator",
                    FullName = "MMS Administrator",
                    DisplayImage = "/Assets/images/profile-pics/3.jpg",
                    Protected = true
                }, "MmsAbc!23");
            }

            var userId = userManger.FindByEmail("MMSadministrator@mailinator.com").Id;
            userManger.AddToRole(userId, "Admin");
            //End Admin

            //Add Member
            if (!context.Users.Any(u => u.Email == "MMSuser1@mailinator.com"))
            {
                userManger.Create(new ApplicationUser
                {
                    UserName = "MMSuser1@mailinator.com",
                    Email = "MMSuser1@mailinator.com",
                    FirstName = "MMS",
                    LastName = "User1",
                    FullName = "MMS User1",
                    DisplayImage = "/Assets/images/profile-pics/4.jpg",
                    Protected = true
                }, "MmsAbc!23");
            }

            userId = userManger.FindByEmail("MMSuser1@mailinator.com").Id;
            userManger.AddToRole(userId, "Member");
            //End Member

            //Add Member
            if (!context.Users.Any(u => u.Email == "MMSuser2@mailinator.com"))
            {
                userManger.Create(new ApplicationUser
                {
                    UserName = "MMSuser2@mailinator.com",
                    Email = "MMSuser2@mailinator.com",
                    FirstName = "MMS",
                    LastName = "User2",
                    FullName = "MMS User2",
                    DisplayImage = "/Assets/images/profile-pics/2.jpg",
                    Protected = true
                }, "MmsAbc!23");
            }

            userId = userManger.FindByEmail("MMSuser2@mailinator.com").Id;
            userManger.AddToRole(userId, "Member");
            //End Member

            //Add Member
            if (!context.Users.Any(u => u.Email == "HOH@mailinator.com"))
            {
                userManger.Create(new ApplicationUser
                {
                    UserName = "HOH@mailinator.com",
                    Email = "HOH@mailinator.com",
                    FirstName = "HOH",
                    LastName = "Houshold",
                    FullName = "Head of Houshold",
                    DisplayImage = "/Assets/images/profile-pics/2.jpg",
                    Protected = true
                }, "MmsAbc!23");
            }

            userId = userManger.FindByEmail("HOH@mailinator.com").Id;
            userManger.AddToRole(userId, "HOH");
            //End Member
            #endregion

            #region Seed Table Data

            context.AccountTypes.AddOrUpdate(
                p => p.Type,
                new AccountType { Type = "Checking"},
                new AccountType { Type = "Savings" },
                new AccountType { Type = "Money Market" }

                );

            context.IncomeExpenses.AddOrUpdate(
                p => p.Type,
                new IncomeExpense { Type = "Income" },
                new IncomeExpense { Type = "Expense" }
                );

         context.SampleBudgetItemCategories.AddOrUpdate(
                p => p.Name,
                new SampleBudgetItemCategory { Id = 1, Name = "Utilities" },
                new SampleBudgetItemCategory { Id = 2, Name = "Housing" },
                new SampleBudgetItemCategory { Id = 3, Name = "Automotive" },
                new SampleBudgetItemCategory { Id = 4, Name = "Food" },
                new SampleBudgetItemCategory { Id = 5, Name = "Entertainment" },
                new SampleBudgetItemCategory { Id = 6, Name = "Communications" },
                new SampleBudgetItemCategory { Id = 7, Name = "Misc" }
                );

            context.SampleBudgetItems.AddOrUpdate(
                p => p.Name,
                new SampleBudgetItem { Name = "Electric", CategoryId = 1, Amount = 150.00M},
                new SampleBudgetItem { Name = "Water", CategoryId = 1, Amount = 75.00M },
                new SampleBudgetItem { Name = "Rent", CategoryId = 2, Amount = 600.00M },
                new SampleBudgetItem { Name = "Mortgage", CategoryId = 2, Amount = 1000.00M },
                new SampleBudgetItem { Name = "Fuel", CategoryId = 3, Amount = 100.00M },
                new SampleBudgetItem { Name = "Insurance", CategoryId = 3, Amount = 125.00M },
                new SampleBudgetItem { Name = "Groceries", CategoryId = 4, Amount = 400.00M },
                new SampleBudgetItem { Name = "Eating Out", CategoryId = 4, Amount = 100.00M },
                new SampleBudgetItem { Name = "Movies", CategoryId = 5, Amount = 100.00M },
                new SampleBudgetItem { Name = "Cable/Internet", CategoryId = 5, Amount = 100.00M },
                new SampleBudgetItem { Name = "Cell Phone", CategoryId = 6, Amount = 75.00M },
                new SampleBudgetItem { Name = "House Phone", CategoryId = 6, Amount = 100 }              

                );

            //context.BudgetItems.AddOrUpdate(
            //  p => p.Name,
            //  new BudgetItem { Name = "Electric", CategoryId = 1, Amount = 150.00M },
            //  new BudgetItem { Name = "Water", CategoryId = 1, Amount = 75.00M },
            //  new BudgetItem { Name = "Rent", CategoryId = 2, Amount = 600.00M },
            //  new BudgetItem { Name = "Mortgage", CategoryId = 2, Amount = 1000.00M },
            //  new BudgetItem { Name = "Fuel", CategoryId = 3, Amount = 100.00M },
            //  new BudgetItem { Name = "Insurance", CategoryId = 3, Amount = 125.00M },
            //  new BudgetItem { Name = "Groceries", CategoryId = 4, Amount = 400.00M },
            //  new BudgetItem { Name = "Eating Out", CategoryId = 4, Amount = 100.00M },
            //  new BudgetItem { Name = "Movies", CategoryId = 5, Amount = 100.00M },
            //  new BudgetItem { Name = "Cable/Internet", CategoryId = 5, Amount = 100.00M },
            //  new BudgetItem { Name = "Cell Phone", CategoryId = 6, Amount = 75.00M },
            //  new BudgetItem { Name = "House Phone", CategoryId = 6, Amount = 100 }

            //  );
            #endregion
        }
    }
}
