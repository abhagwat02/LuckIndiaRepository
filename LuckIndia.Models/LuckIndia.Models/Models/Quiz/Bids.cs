using System;

namespace LuckIndia.Models
{
    public class Bids
    {
        public int Id { get; set; }
        public int BidAmount { get; set; }
        public DateTime DateCreated { get; set; }

        public Quiz PlayedQuiz { get; set; }
        public Account PlayingAccount{ get; set; }

    }
}