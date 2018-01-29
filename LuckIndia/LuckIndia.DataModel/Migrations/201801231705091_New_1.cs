namespace LuckIndia.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class New_1 : DbMigration
    {
        public override void Up()
        {
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
            
            AddColumn("dbo.Questions", "Last", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuestionQuizMaps", "QuizId", "dbo.Quizs");
            DropForeignKey("dbo.QuestionQuizMaps", "QuestionId", "dbo.Questions");
            DropIndex("dbo.QuestionQuizMaps", new[] { "QuestionId" });
            DropIndex("dbo.QuestionQuizMaps", new[] { "QuizId" });
            DropColumn("dbo.Questions", "Last");
            DropTable("dbo.QuestionQuizMaps");
        }
    }
}
