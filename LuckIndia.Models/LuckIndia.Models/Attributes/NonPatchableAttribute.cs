using System;

namespace LuckIndia.Models.Attributes

{
    /// <summary>
    /// Any property with this attribute will not be patchable using DatabaseContext.Update.
    /// </summary>
    sealed class NonPatchableAttribute : Attribute
    {
    }
}