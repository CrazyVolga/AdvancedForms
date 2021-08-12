using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using AdvancedForms.Attributes;
using AdvancedForms.ControlConstructors;

namespace AdvancedForms.GUI
{
    public partial class FormGUI
    {
        private static readonly Dictionary<Type, FieldControlsConstructor> CustomConstructors = new Dictionary<Type, FieldControlsConstructor>();

        internal static void AddCustomConstructor(Type type, FieldControlsConstructor constructor)
        {
            if (CustomConstructors.ContainsKey(type) == false) CustomConstructors[type] = constructor;
        }

        private ManagedForm _myForm;
        private FormGUIUtils FormGUIUtils => _myForm.FormGUIUtils;

        public FormGUI(ManagedForm form)
        {
            _myForm = form;
        }

        public int FieldHeight(SerializedField field)
        {
            var customConstructor = field.GetAttribute<FieldControlsConstructor>();
            if (customConstructor != null)
                return customConstructor.GetFieldHeight(_myForm, field);
            if (CustomConstructors.ContainsKey(field.SerializedType))
                return CustomConstructors[field.SerializedType].GetFieldHeight(_myForm, field);
            return FieldPrimitiveHeight(field);
        }

        internal int FieldPrimitiveHeight(SerializedField field)
        {
            int height = FormGUIUtils.LineHeight;

            if (Type.GetTypeCode(field.SerializedType) == TypeCode.Object && field.GetValue() != null)
            {
                height -= FormGUIUtils.VerticalSpacing;
                if (field.Fields.Count() == 0) return height;
                foreach (var subField in field.Fields)
                {
                    var subFieldHeight = FieldHeight(subField);
                    if (subFieldHeight == 0) continue;
                    height += subFieldHeight + FormGUIUtils.VerticalSpacing;
                }
            }

            return height;
        }

        private Control[] RawInitField(SerializedField field, string fieldName)
        {
            var customConstructor = field.GetAttribute<FieldControlsConstructor>();
            if (customConstructor != null)
                return customConstructor.InitializeControls(_myForm, field, fieldName);

            if (CustomConstructors.ContainsKey(field.SerializedType))
                return CustomConstructors[field.SerializedType].InitializeControls(_myForm, field, fieldName);

            return RawInitPrimitiveField(field, fieldName);            
        }

        private Control[] RawInitPrimitiveField(SerializedField field, string fieldName)
        {
            if (field.SerializedType.IsEnum)
                return InitEnumField(field, fieldName);

            if (field.SerializedType.IsArray)
                throw new NotImplementedException();

            switch (Type.GetTypeCode(field.SerializedType))
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return InitNumericField(field, fieldName);
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return InitFloatNumericField(field, 2, fieldName);
                case TypeCode.Boolean:
                    return InitBoolField(field, fieldName);
                case TypeCode.String:
                    return InitStringField(field, fieldName);
                case TypeCode.Char:
                    return InitCharField(field, fieldName);
                case TypeCode.Object:
                    return InitObjectField(field, fieldName);
                case TypeCode.DateTime:
                default:
                    break;
            }
            return new Control[] { };
        }

        public Control[] InitField(SerializedField field, string fieldName)
        {
            var controls = RawInitField(field, fieldName);

            FormGUIUtils.AddFieldConrols(field, controls);
            return controls;
        }

        internal Control[] InitPrimitiveField(SerializedField field, string fieldName)
        {
            var controls = RawInitPrimitiveField(field, fieldName);

            FormGUIUtils.AddFieldConrols(field, controls);
            return controls;
        }

        public void ResizeField(SerializedField field, FieldLayout layout, Control[] controls)
        {
            var customConstructor = field.GetAttribute<FieldControlsConstructor>();

            if (customConstructor != null)
                customConstructor.ResizeControls(_myForm, field, layout, controls);
            else if (CustomConstructors.ContainsKey(field.SerializedType))
                CustomConstructors[field.SerializedType].ResizeControls(_myForm, field, layout, controls);
            else 
                ResizePrimitiveField(field, layout, controls);
        }

        internal void ResizePrimitiveField(SerializedField field, FieldLayout layout, Control[] controls)
        {
            if (Type.GetTypeCode(field.SerializedType) == TypeCode.Object)
                ResizeObjectField(field, layout, controls);
            else if (controls.Length == 1)
                DefaultResize(layout.FullRect, controls[0]);
            else if (controls.Length == 0) return;
            else
            {
                DefaultResize(layout.LabelRect, controls[0]);
                DefaultResize(layout.ValueRect, controls[1]);
            }
        }

        private Control[] RawInitMethod(MethodInfo method, Action buttonAction, string text)
        {
            var button = InitButton(text);
            button.Click += new EventHandler((s, e) => {
                buttonAction();
            });

            return new Control[] { button };
        }

        public Control[] InitMethod(MethodInfo method, Action buttonAction, string text)
        {
            var controls = RawInitMethod(method, buttonAction, text);
            FormGUIUtils.AddMethodConrols(method, controls);
            return controls;
        }

        public void ResizeMethod(MethodInfo method, Rectangle methodRect, Control[] controls)
        {
            var button = controls.SingleOrDefault();
            if (button == null) return;
            button.SetRectangle(methodRect);
        }


        public void DefaultResize(Rectangle newRect, Control control)
        {
            control.Anchor = AnchorStyles.None;
            control.Location = newRect.Location;
            control.Size = newRect.Size;
        }
    }
}
