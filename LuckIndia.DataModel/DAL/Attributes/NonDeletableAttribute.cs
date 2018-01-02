using System;

namespace LuckIndia.DataModel.DAL.Attributes
{
    /// <summary>
    /// Any object with this attribute will not be deletable using DatabaseContext.Delete.
    /// </summary>
    sealed class NonDeletableAttribute : Attribute
    {
    }
}