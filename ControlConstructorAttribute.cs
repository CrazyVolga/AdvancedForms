using System;

namespace AdvancedForms
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class CustomControlsConstructor : Attribute
    {
        internal Type fieldType;

        public CustomControlsConstructor(Type fieldType)
        {
            this.fieldType = fieldType;
        }
    }
}
