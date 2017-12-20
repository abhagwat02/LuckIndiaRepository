namespace LuckIndia.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            DropTable("Transactions");
            DropTable("TxnTypes");
            DropTable("Bids");
            DropTable("Accounts");
            DropTable("AccountTypes");
            DropTable("LuckUsers");
            DropTable("Results");
            DropTable("Options");
            DropTable("Questions");
            DropTable("Quizs");
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateCreated = c.DateTime(),
                        CardNumber = c.Int(nullable: false),
                        DateModified = c.DateTime(),
                        ParentAccountID = c.Int(nullable: false),
                        Type_Id = c.Int(),
                        user_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountTypes", t => t.Type_Id)
                .ForeignKey("dbo.LuckUsers", t => t.user_Id)
                .Index(t => t.Type_Id)
                .Index(t => t.user_Id);
            
            CreateTable(
                "dbo.AccountTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
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
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Bids",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BidAmount = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
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
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Statement = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
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
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
                        Question_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.Question_Id)
                .Index(t => t.Question_Id);
            
            CreateTable(
                "dbo.Results",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AnnouncementDate = c.DateTime(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
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
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
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
                        DateCreated = c.DateTime(nullable: false),
                        DateModified = c.DateTime(nullable: false),
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
            DropForeignKey("dbo.Bids", "PlayingAccount_Id", "dbo.Accounts");
            DropForeignKey("dbo.Bids", "PlayedQuiz_Id", "dbo.Quizs");
            DropForeignKey("dbo.Questions", "Quiz_Id", "dbo.Quizs");
            DropForeignKey("dbo.Options", "Question_Id", "dbo.Questions");
            DropForeignKey("dbo.Accounts", "user_Id", "dbo.LuckUsers");
            DropForeignKey("dbo.Accounts", "Type_Id", "dbo.AccountTypes");
            DropIndex("dbo.Transactions", new[] { "Type_Id" });
            DropIndex("dbo.Transactions", new[] { "Player_Id" });
            DropIndex("dbo.Transactions", new[] { "Bid_Id" });
            DropIndex("dbo.Results", new[] { "SelectedQuiz_Id" });
            DropIndex("dbo.Results", new[] { "CorrectOption_Id" });
            DropIndex("dbo.Options", new[] { "Question_Id" });
            DropIndex("dbo.Questions", new[] { "Quiz_Id" });
            DropIndex("dbo.Bids", new[] { "PlayingAccount_Id" });
            DropIndex("dbo.Bids", new[] { "PlayedQuiz_Id" });
            DropIndex("dbo.Accounts", new[] { "user_Id" });
            DropIndex("dbo.Accounts", new[] { "Type_Id" });
            DropTable("dbo.TxnTypes");
            DropTable("dbo.Transactions");
            DropTable("dbo.Results");
            DropTable("dbo.Options");
            DropTable("dbo.Questions");
            DropTable("dbo.Quizs");
            DropTable("dbo.Bids");
            DropTable("dbo.LuckUsers");
            DropTable("dbo.AccountTypes");
            DropTable("dbo.Accounts");
        }
    }
}
