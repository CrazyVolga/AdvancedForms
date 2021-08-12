using System;
using System.Collections.Generic;
using System.Text;

namespace AdvancedForms
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public abstract class LayoutAttribute : Attribute
    {

        public bool Is<T>() where T : LayoutAttribute
        {
            return this.GetType() == typeof(T);
        }
    }

    public sealed class Serialize : LayoutAttribute {}

    public sealed class NonSerialize : LayoutAttribute {}

    public sealed class Name : LayoutAttribute
    {
        public string Text { get; private set; }

        public Name(string text)
        {
            Text = text;
        }
    }

    public sealed class Width : LayoutAttribute
    {
        public int LabelWidth { get; private set; }
        public int BoxWidth { get; private set; }

        public Width(int labelWidth, int boxWidth)
        {
            LabelWidth = labelWidth;
            BoxWidth = boxWidth;
        }
    }
}
