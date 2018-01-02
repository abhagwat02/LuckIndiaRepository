using System;

namespace LuckIndia.Models.Interfaces
{
    public interface IExpirable
    {
        DateTime StartDate { get; }

        DateTime? EndDate { get; }
    }
}