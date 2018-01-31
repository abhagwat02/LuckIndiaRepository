namespace LuckIndia.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccessTokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Token = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => new { t.ApplicationId, t.UserId }, unique: true, name: "IX_UniqueAccessTokenForApplicationUser");
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UniqueId = c.Guid(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Email = c.String(),
                        UserName = c.String(),
                        PasswordHash = c.String(),
                        PasswordSalt = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        RequirePasswordReset = c.Boolean(nullable: false),
                        DefaultLanguage = c.String(),
                        DateOfBirth = c.DateTime(),
                        Gender = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateCreated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateModified = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CardNumber = c.Int(nullable: false),
                        AccountTypeID = c.Int(),
                        LuckUserID = c.Int(nullable: false),
                        UserName = c.String(),
                        Password = c.String(),
                        ParentAccountID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountTypes", t => t.AccountTypeID)
                .ForeignKey("dbo.LuckUsers", t => t.LuckUserID)
                .Index(t => t.AccountTypeID)
                .Index(t => t.LuckUserID);
            
            CreateTable(
                "dbo.AccountTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
                        DateCreated = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LuckUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        LastName = c.String(),
                        PhoeNumber = c.Long(nullable: false),
                        Address = c.String(),
                        ParentAccountID = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateModified = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Bids",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateCreated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateModified = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        BidAmount = c.Int(nullable: false),
                        PlayedQuiz_Id = c.Int(),
                        PlayingAccount_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quizs", t => t.PlayedQuiz_Id)
                .ForeignKey("dbo.Accounts", t => t.PlayingAccount_Id)
                .Index(t => t.PlayedQuiz_Id)
                .Index(t => t.PlayingAccount_Id);
            
            CreateTable(
                "dbo.Quizs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuizName = c.String(),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateModified = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Statement = c.String(),
                        Last = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateModified = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Quiz_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quizs", t => t.Quiz_Id)
                .Index(t => t.Quiz_Id);
            
            CreateTable(
                "dbo.Options",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Content = c.String(),
                        QuestionID = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateModified = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.QuestionID)
                .Index(t => t.QuestionID);
            
            CreateTable(
                "dbo.ModelClasses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        PluralTitle = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RolePermissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.Int(nullable: false),
                        ModelClassId = c.Int(nullable: false),
                        CanCreate = c.Boolean(nullable: false),
                        CanRead = c.Boolean(nullable: false),
                        CanUpdate = c.Boolean(nullable: false),
                        CanDelete = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ModelClasses", t => t.ModelClassId)
                .ForeignKey("dbo.Roles", t => t.RoleId)
                .Index(t => t.RoleId)
                .Index(t => t.ModelClassId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        RoleTypeId = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RoleTypes", t => t.RoleTypeId)
                .Index(t => t.RoleTypeId);
            
            CreateTable(
                "dbo.RoleTypes",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuestionQuizMaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuizId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.QuestionId)
                .ForeignKey("dbo.Quizs", t => t.QuizId)
                .Index(t => t.QuizId)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.Results",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AnnouncementDate = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateModified = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        CorrectOption_Id = c.Int(),
                        SelectedQuiz_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Options", t => t.CorrectOption_Id)
                .ForeignKey("dbo.Quizs", t => t.SelectedQuiz_Id)
                .Index(t => t.CorrectOption_Id)
                .Index(t => t.SelectedQuiz_Id);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Amount = c.Int(nullable: false),
                        TxnDate = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        DateModified = c.DateTime(nullable: false, precision: 7, storeType: "datetime2"),
                        Bid_Id = c.Int(),
                        Player_Id = c.Int(),
                        Type_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Bids", t => t.Bid_Id)
                .ForeignKey("dbo.Accounts", t => t.Player_Id)
                .ForeignKey("dbo.TxnTypes", t => t.Type_Id)
                .Index(t => t.Bid_Id)
                .Index(t => t.Player_Id)
                .Index(t => t.Type_Id);
            
            CreateTable(
                "dbo.TxnTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
                        DateCreated = c.DateTime(),
                        DateModified = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "Type_Id", "dbo.TxnTypes");
            DropForeignKey("dbo.Transactions", "Player_Id", "dbo.Accounts");
            DropForeignKey("dbo.Transactions", "Bid_Id", "dbo.Bids");
            DropForeignKey("dbo.Results", "SelectedQuiz_Id", "dbo.Quizs");
            DropForeignKey("dbo.Results", "CorrectOption_Id", "dbo.Options");
            DropForeignKey("dbo.QuestionQuizMaps", "QuizId", "dbo.Quizs");
            DropForeignKey("dbo.QuestionQuizMaps", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.Roles", "RoleTypeId", "dbo.RoleTypes");
            DropForeignKey("dbo.RolePermissions", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.RolePermissions", "ModelClassId", "dbo.ModelClasses");
            DropForeignKey("dbo.Bids", "PlayingAccount_Id", "dbo.Accounts");
            DropForeignKey("dbo.Bids", "PlayedQuiz_Id", "dbo.Quizs");
            DropForeignKey("dbo.Questions", "Quiz_Id", "dbo.Quizs");
            DropForeignKey("dbo.Options", "QuestionID", "dbo.Questions");
            DropForeignKey("dbo.Accounts", "LuckUserID", "dbo.LuckUsers");
            DropForeignKey("dbo.Accounts", "AccountTypeID", "dbo.AccountTypes");
            DropForeignKey("dbo.AccessTokens", "UserId", "dbo.Users");
            DropIndex("dbo.Transactions", new[] { "Type_Id" });
            DropIndex("dbo.Transactions", new[] { "Player_Id" });
            DropIndex("dbo.Transactions", new[] { "Bid_Id" });
            DropIndex("dbo.Results", new[] { "SelectedQuiz_Id" });
            DropIndex("dbo.Results", new[] { "CorrectOption_Id" });
            DropIndex("dbo.QuestionQuizMaps", new[] { "QuestionId" });
            DropIndex("dbo.QuestionQuizMaps", new[] { "QuizId" });
            DropIndex("dbo.Roles", new[] { "RoleTypeId" });
            DropIndex("dbo.RolePermissions", new[] { "ModelClassId" });
            DropIndex("dbo.RolePermissions", new[] { "RoleId" });
            DropIndex("dbo.Options", new[] { "QuestionID" });
            DropIndex("dbo.Questions", new[] { "Quiz_Id" });
            DropIndex("dbo.Bids", new[] { "PlayingAccount_Id" });
            DropIndex("dbo.Bids", new[] { "PlayedQuiz_Id" });
            DropIndex("dbo.Accounts", new[] { "LuckUserID" });
            DropIndex("dbo.Accounts", new[] { "AccountTypeID" });
            DropIndex("dbo.AccessTokens", "IX_UniqueAccessTokenForApplicationUser");
            DropTable("dbo.TxnTypes");
            DropTable("dbo.Transactions");
            DropTable("dbo.Results");
            DropTable("dbo.QuestionQuizMaps");
            DropTable("dbo.RoleTypes");
            DropTable("dbo.Roles");
            DropTable("dbo.RolePermissions");
            DropTable("dbo.ModelClasses");
            DropTable("dbo.Options");
            DropTable("dbo.Questions");
            DropTable("dbo.Quizs");
            DropTable("dbo.Bids");
            DropTable("dbo.LuckUsers");
            DropTable("dbo.AccountTypes");
            DropTable("dbo.Accounts");
            DropTable("dbo.Users");
            DropTable("dbo.AccessTokens");
        }
    }
}
