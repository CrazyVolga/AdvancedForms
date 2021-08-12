using AdvancedForms.GUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AdvancedForms.ControlConstructors
{
    public abstract class FieldControlsConstructorBase : Attribute
    {
        protected ManagedForm MyForm { get; private set; }
        protected FormGUIUtils FormGUIUtils => MyForm.FormGUIUtils;
        protected FormGUI FormGUI => MyForm.FormGUI;
        protected FieldControlsConstructor OnForm(ManagedForm form)
        {
            MyForm = form;
            return this as FieldControlsConstructor;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public abstract class FieldControlsConstructor : FieldControlsConstructorBase
    {
        public int GetFieldHeight(ManagedForm form, SerializedField field)
        {
            return OnForm(form).GetFieldHeight(field);
        }

        public Control[] InitializeControls(ManagedForm form, SerializedField field, string fieldLabel)
        {
            return OnForm(form).InitializeControls(field, fieldLabel);
        }

        public void ResizeControls(ManagedForm form, SerializedField field, FieldLayout layout, Control[] controls)
        {
            OnForm(form).ResizeControls(field, layout, controls);
        }

        protected virtual int GetFieldHeight(SerializedField field)
        {
            return FormGUI.FieldPrimitiveHeight(field);
        }

        protected virtual Control[] InitializeControls(SerializedField field, string fieldLabel)
        {
            return FormGUI.InitPrimitiveField(field, fieldLabel);
        }

        protected virtual void ResizeControls(SerializedField field, FieldLayout layout, Control[] controls)
        {
            FormGUI.ResizePrimitiveField(field, layout, controls);
        }
    }
}
