using System;

namespace LuckIndia.DataModel.DAL.Attributes
{
    /// <summary>
    /// Any object with this attribute will be looked at for adding to any Include statements.
    /// Any property within this object will be added to the Include statement.
    /// </summary>
    sealed class ManyToManyObjectAttribute : Attribute
    {
    }
}