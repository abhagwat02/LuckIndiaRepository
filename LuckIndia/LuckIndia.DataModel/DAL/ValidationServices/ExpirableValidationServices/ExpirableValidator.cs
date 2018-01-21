using System;

namespace LuckIndia.DataModel.DAL.ValidationServices.ExpirableValidationServices
{
    sealed class ExpirableValidator
    {
        public static bool IsExpired(DateTime? endDate)
        {
            if (endDate == null)
            {
                return false;
            }

            return (DateTime.SpecifyKind(endDate.Value, DateTimeKind.Utc) <= DateTime.UtcNow);
        }

        public static bool IsActive(DateTime startDate, DateTime? endDate)
        {
            return DateTime.SpecifyKind(startDate, DateTimeKind.Utc) <= DateTime.UtcNow && !IsExpired(endDate);
        }
    }
}