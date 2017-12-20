namespace LuckIndia.DataModel.Migrations
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LuckIndia.DataModel.LuckIndiaDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            Database.SetInitializer<LuckIndiaDBContext>(new DropCreateDatabaseAlways<LuckIndiaDBContext>());
           
        }

        protected override void Seed(LuckIndia.DataModel.LuckIndiaDBContext context)
        {



            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data

            var accttypes = new List<AccountType>
            {
                 new Models.AccountType
                {
                    TypeName = "Admin"
                },
                   new Models.AccountType
                 {
                     TypeName = "Dealer"
                 },
                    new Models.AccountType
                 {
                     TypeName = "Retailer"
                 },
                     new Models.AccountType
                 {
                     TypeName = "Customer"
                 }
            };

            accttypes.ForEach(d => context.AcctTypes.AddOrUpdate(d));
            context.SaveChanges();

            //Accounts
            var accounts_user1 = new List<Account>
            {
                new Account {  CardNumber = 1111, Type = accttypes[0]},
                new Account {  CardNumber = 2222, Type = accttypes[1]},
            };

            accounts_user1.ForEach(d => context.Accounts.AddOrUpdate(d));
            context.SaveChanges();

            var accounts_user2 = new List<Account>
            {
                new Account {  CardNumber = 3333, Type = accttypes[2]},
            };

            accounts_user2.ForEach(d => context.Accounts.AddOrUpdate(d));
            context.SaveChanges();

            var accounts_user3 = new List<Account>
            {
                new Account {  CardNumber = 4444, Type = accttypes[3]},
            };

            var accounts_user4 = new List<Account>
            {
                new Account {  CardNumber = 5555, Type = accttypes[3]},
            };

            accounts_user3.ForEach(d => context.Accounts.AddOrUpdate(d));
            context.SaveChanges();

            ////User
            var users = new List<LuckUser>
            {
                // this will have DepartmentId = 1
               new LuckUser { MiddleName = "Kumar",
                  FirstName = "Atul",
                  LastName = "Bhagwat",
                  Address = "Bavdhan",
                  PhoeNumber = 900001,
                  accounts = accounts_user1
        },

               new LuckUser
              {
                  FirstName = "Abhijeet",
                  LastName = "Sarma",
                  Address = "Karve Nagar",
                  PhoeNumber = 900002,
                  accounts = accounts_user2


        },
                new Models.LuckUser
                {
                    FirstName = "Indra",
                    LastName = "Singh",
                    Address = "Dhayari",
                    PhoeNumber = 900003,
                    accounts = accounts_user3
        },
                 new Models.LuckUser
                {
                    FirstName = "Mukesh",
                    LastName = "Mishra",
                    Address = "Katraj",
                    PhoeNumber = 900004,
                    accounts = accounts_user4
        }
        // more departments
    };

            users.ForEach(d => context.Users.AddOrUpdate(d));
            context.SaveChanges();

           
        }
    }
}
