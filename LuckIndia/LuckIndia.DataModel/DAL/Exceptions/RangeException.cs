namespace Alphaeon.Services.EnterpriseAPI.DAL.Exceptions
{
    sealed class RangeException : DataAccessLayerException
    {
        public RangeException(int minValue, int maxValue)
            : base(string.Format("Value must be between {0} and {1} inclusively.", minValue, maxValue))
        {
        }
    }
}

