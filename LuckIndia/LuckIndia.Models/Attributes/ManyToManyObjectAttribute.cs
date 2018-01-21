using System;

 namespace LuckIndia.Models.Attributes
{
    /// <summary>
    /// Any object with this attribute will be looked at for adding to any Include statements.
    /// Any property within this object will be added to the Include statement.
    /// </summary>
    public class IncludeAttribute : Attribute
    {
    }

    public class ManyToManyObjectAttribute : Attribute
    {
    }
}