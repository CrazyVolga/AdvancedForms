using System;

namespace AdvancedForms.ControlConstructors
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class DefaultFieldControlsConstructor : Attribute
    {
        internal Type fieldType;

        public DefaultFieldControlsConstructor(Type fieldType)
        {
            this.fieldType = fieldType;
        }
    }
}
