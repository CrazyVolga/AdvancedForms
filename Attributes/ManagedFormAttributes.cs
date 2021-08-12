using System;
using System.Collections.Generic;
using System.Text;

namespace AdvancedForms.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public abstract class LayoutAttribute : Attribute
    {

        public bool Is<T>() where T : LayoutAttribute
        {
            return this.GetType() == typeof(T);
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public sealed class Serialize : LayoutAttribute {}

    public sealed class NonSerialize : LayoutAttribute {}

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class Name : LayoutAttribute
    {
        public string Text { get; private set; }

        public Name(string text)
        {
            Text = text;
        }
    }
}
