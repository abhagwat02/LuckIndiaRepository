using System;

namespace LuckIndia.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public DateTime TxnDate { get; set; }

        public Bids Bid { get; set; }
        public Account Player { get; set; }
        public TxnType Type { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

    }
}