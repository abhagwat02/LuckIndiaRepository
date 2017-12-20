using LuckIndia.Models;
using System.Data.Entity;

namespace LuckIndia.DataModel
{
    public class LuckIndiaDBContext : DbContext
    {

        public LuckIndiaDBContext() : base("name=DatabaseContext")
        {
            Database.SetInitializer<LuckIndiaDBContext>(new CreateDatabaseIfNotExists<LuckIndiaDBContext>());
        }


        public DbSet<LuckUser> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Bids> Bid { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Quiz> Quizes { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<TxnType> TxnTypes { get; set; }
        public DbSet<AccountType> AcctTypes { get; set; }

     


    }
}
 