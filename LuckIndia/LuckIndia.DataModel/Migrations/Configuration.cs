namespace LuckIndia.DataModel.Migrations
{
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LuckIndiaDBContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            Database.SetInitializer<LuckIndiaDBContext>(new DropCreateDatabaseIfModelChanges<LuckIndiaDBContext>());
           
        }

        protected override void Seed(LuckIndiaDBContext context)
        {



            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data
            #region Acct type
            var accttypes = new List<AccountType>
            {
                 new Models.AccountType
                {
                    TypeName = "Admin",
                    DateCreated = DateTime.Now, DateModified = DateTime.Now
                },
                   new Models.AccountType
                 {
                     TypeName = "Dealer",
                     DateCreated = DateTime.Now, DateModified = DateTime.Now
                 },
                    new Models.AccountType
                 {
                     TypeName = "Retailer",
                     DateCreated = DateTime.Now, DateModified = DateTime.Now
                 },
                     new Models.AccountType
                 {
                     TypeName = "Customer",
                     DateCreated = DateTime.Now, DateModified = DateTime.Now
                 }
            };

            accttypes.ForEach(d => context.AcctTypes.AddOrUpdate(d));
            context.SaveChanges();
            #endregion

            #region Users
     

            ////User
            var users = new List<LuckUser>
            {
                // this will have DepartmentId = 1
               new LuckUser { MiddleName = "Kumar",
                  FirstName = "Atul",
                  LastName = "Bhagwat",
                  Address = "Bavdhan",
                  PhoeNumber = 900001,
                  //accounts = accounts_user1,

        },

               new LuckUser
              {
                  FirstName = "Abhijeet",
                  LastName = "Sarma",
                  Address = "Karve Nagar",
                  PhoeNumber = 900002,
                 // accounts = accounts_user2


        },
                new Models.LuckUser
                {
                    FirstName = "Indra",
                    LastName = "Singh",
                    Address = "Dhayari",
                    PhoeNumber = 900003,
                   // accounts = accounts_user3
        },
                 new Models.LuckUser
                {
                    FirstName = "Mukesh",
                    LastName = "Mishra",
                    Address = "Katraj",
                    PhoeNumber = 900004,
                    //accounts = accounts_user4
        }
                 
        // more departments
    };
            users.ForEach(d => context.LuckUsers.AddOrUpdate(d));
            context.SaveChanges();

            #region account
            //Accounts
           
            #endregion
            int luckAtulId = context.LuckUsers.Where(x => x.FirstName == "Atul").Select(y => y.Id).FirstOrDefault();
            int luckAbhijeetId = context.LuckUsers.Where(x => x.FirstName == "Abhijeet").Select(y => y.Id).FirstOrDefault();
            int luckIndraId = context.LuckUsers.Where(x => x.FirstName == "Indra").Select(y => y.Id).FirstOrDefault();
            int luckMukeshId = context.LuckUsers.Where(x => x.FirstName == "Mukesh").Select(y => y.Id).FirstOrDefault();

            var accounts_user1 = new List<Account>
            {
                new Account {  CardNumber = 1111, Type = accttypes[0], UserName = "Indra@gmail.com",Password = "password",DateCreated = DateTime.Now, DateModified = DateTime.Now,LuckUserID = luckAtulId,AccountTypeID = accttypes[0].Id},
                new Account {  CardNumber = 2222, Type = accttypes[1], DateCreated = DateTime.Now, DateModified = DateTime.Now,LuckUserID = luckAtulId,AccountTypeID = accttypes[1].Id},
            };
            accounts_user1.ForEach(d => context.Accounts.AddOrUpdate(d));

            var accounts_user2 = new List<Account>
            {
                new Account {  CardNumber = 3333, Type = accttypes[2], DateCreated = DateTime.Now , DateModified = DateTime.Now,LuckUserID = luckAbhijeetId, AccountTypeID = accttypes[2].Id},
            };

            accounts_user2.ForEach(d => context.Accounts.AddOrUpdate(d));

            var accounts_user3 = new List<Account>
            {
                new Account {  CardNumber = 4444, Type = accttypes[3], DateCreated = DateTime.Now, DateModified = DateTime.Now,LuckUserID = luckIndraId,AccountTypeID = accttypes[3].Id},
            };
            accounts_user3.ForEach(d => context.Accounts.AddOrUpdate(d));

            var accounts_user4 = new List<Account>
            {
                new Account {  CardNumber = 5555, Type = accttypes[3], DateCreated = DateTime.Now, DateModified = DateTime.Now,LuckUserID = luckMukeshId,AccountTypeID = accttypes[3].Id},
            };
            accounts_user4.ForEach(d => context.Accounts.AddOrUpdate(d));

            context.SaveChanges();


            #endregion

            context.QuizTypes.Add(
                new QuizType
                {
                    name = "Quiz1"
                }
                );

            context.QuizTypes.Add(
                new QuizType
                {
                    name = "Quiz2"
                }
                );

            context.QuizTypes.Add(
                new QuizType
                {
                    name = "Quiz3"
                }
                );
            context.Users.AddOrUpdate(new User() { Id = 1, FirstName = "CMD", LastName = "Dashboard", Email = "cmddashboard@Integration.com", UserName = "cmddash", PasswordHash = "dash", IsActive = true, RequirePasswordReset = false, DefaultLanguage = "En-US", DateOfBirth = null, Gender = " " });
            context.AccessTokens.AddOrUpdate(y => y.Id, new AccessToken() { Id = 1, ApplicationId = 1, UserId = 1, Token = "e897aa05df964d188472839559cfd080" });

            //RoleTypes            
            context.RoleTypes.AddOrUpdate(new RoleType() { Id = 1, Title = "System" });
            context.RoleTypes.AddOrUpdate(new RoleType() { Id = 8, Title = "Application" });

            //Roles  
            const string LuckIndiaRole = "LuckIndia";

            context.Roles.AddOrUpdate(new Role() { Id = 100, RoleTypeId = 8, Title = LuckIndiaRole, Description = "LuckIndia" });
            context.SaveChanges();
            int roleIDForLuckIndia = context.Roles.Where(t => t.Title.Equals(LuckIndiaRole)).Select(x => x.Id).FirstOrDefault();


            CreateOrUpdateModelPermissions(context, "AccessToken", "AccessTokens", "Desc", roleIDForLuckIndia, true, true, true);
            CreateOrUpdateModelPermissions(context, "Account", "Accounts", "Desc", roleIDForLuckIndia, true, true, true);
            CreateOrUpdateModelPermissions(context, "LuckUser", "LuckUsers", "Desc", roleIDForLuckIndia, true, true, true);
            CreateOrUpdateModelPermissions(context, "Transaction", "Transactions", "Desc", roleIDForLuckIndia, true, true, true);
            CreateOrUpdateModelPermissions(context, "User", "Users", "Desc", roleIDForLuckIndia, true, true, true);
            CreateOrUpdateModelPermissions(context, "Bids", "Bids", "Desc", roleIDForLuckIndia, true, true, true);
            CreateOrUpdateModelPermissions(context, "Option", "Options", "Desc", roleIDForLuckIndia, true, true, true);
            CreateOrUpdateModelPermissions(context, "Question", "Questions", "Desc", roleIDForLuckIndia, true, true, true);
            CreateOrUpdateModelPermissions(context, "Quiz", "Quizs", "Desc", roleIDForLuckIndia, true, true, true);
            CreateOrUpdateModelPermissions(context, "Result", "Results", "Desc", roleIDForLuckIndia, true, true, true);

            
        }





        private static void CreateOrUpdateModelPermissions(LuckIndiaDBContext context,
                   string title, string pluralTitle, string description, int roleId, bool canCreate = true, bool canRead = true, bool canUpdate = true, bool canDelete = false, int applicationId = 1)
        {
            ModelClass modelClass = new ModelClass() { Title = title, PluralTitle = pluralTitle, Description = description };
            context.ModelClasses.AddOrUpdate(y => y.Title, modelClass);
            context.SaveChanges();

            context.RolePermissions.AddOrUpdate(y => new { y.RoleId, y.ModelClassId }, new RolePermission()
            {
                RoleId = roleId,
                ModelClassId = modelClass.Id,
                CanCreate = canCreate,
                CanRead = canRead,
                CanUpdate = canUpdate,
                CanDelete = canDelete
            });
            context.SaveChanges();

            //context.ApplicationPermissions.AddOrUpdate(y => new { y.ApplicationId, y.ModelClassId }, new ApplicationPermission() { ApplicationId = applicationId, ModelClassId = modelClass.Id, CanCreate = true, CanRead = true, CanUpdate = true, CanDelete = false });

        }
        #region

        #endregion

    
    }
}
