using System;

namespace LuckIndia.Models.Attributes

{
    /// <summary>
    /// Any object with this attribute will not be deletable using DatabaseContext.Delete.
    /// </summary>
    sealed class NonDeletableAttribute : Attribute
    {
    }
}