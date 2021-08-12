using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using AdvancedForms.Attributes;

namespace AdvancedForms.GUI
{
    public sealed class FormGUIUtils
    {
        private ManagedForm _myForm;

        public int HorizontalSpacing => 4;
        public int LabelWidth => (int)(_myForm.WorkRectangle.Width * 0.35) - HorizontalSpacing / 2;
        public int FieldWidth => (int)(_myForm.WorkRectangle.Width * 0.65) - HorizontalSpacing / 2;
        public int VerticalSpacing => 3;
        public int LineHeight => _myForm.Font.Height + 7;
        public int FontHeight => _myForm.Font.Height;
        public (int Horizontal, int Vertical) Indent => (12, 12);

        public FormGUIUtils(ManagedForm form)
        {
            _myForm = form;
        }

        internal static FieldInfo[] GetManagedFields(Type type)
        {
            var serializedPublicFields = type
                .GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(field => !field.GetCustomAttributes<NonSerialize>().Any());
            var serializedPrivateFields = type
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.GetCustomAttributes<Serialize>().Any());

            return serializedPublicFields.Concat(serializedPrivateFields).ToArray();
        }

        internal static MethodInfo[] GetManagedMethods(Type type)
        {
            return type
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Where(field => field.GetCustomAttributes<Serialize>().Any())
                .ToArray();
        }

        private static string CreateDefaultName(string name)
        {
            var words = Regex.Split(name, @"(_)|(?<!^)(?=[A-Z])|(\d+)")
                .Where(word => word != "" && word != "_")
                .Select(word => word.ToLower())
                .ToArray();
            words[0] = words[0].Substring(0, 1).ToUpper() + words[0].Substring(1, words[0].Length - 1);
            return string.Join(" ", words);
        }

        public static string GetName(SerializedField field)
        {
            var attribute = field.GetAttribute<Name>();
            return (attribute != null) ? attribute.Text : CreateDefaultName(field.Name);
        }

        internal static string GetDefaultName(MemberInfo fieldInfo)
        {
            string name = fieldInfo.GetCustomAttribute<Name>()?.Text;

            if (name == null)
            {
                name = fieldInfo.Name;
                var words = Regex.Split(name, @"(_)|(?<!^)(?=[A-Z])|(\d+)")
                    .Where(word => word != "" && word != "_")
                    .Select(word => word.ToLower())
                    .ToArray();
                words[0] = words[0].Substring(0, 1).ToUpper() + words[0].Substring(1, words[0].Length - 1);
                name = string.Join(" ", words);
            }

            return name;
        }

        private Dictionary<SerializedField, Control[]> _fieldControls = new Dictionary<SerializedField, Control[]>();
        private Dictionary<MethodInfo, Control[]> _methodControls = new Dictionary<MethodInfo, Control[]>();

        internal void AddFieldConrols(SerializedField field, Control[] controls)
        {
            _fieldControls[field] = controls;
        }

        internal void AddMethodConrols(MethodInfo method, Control[] controls)
        {
            _methodControls[method] = controls;
        }

        public Control[] GetMethodControls(MethodInfo method)
        {
            if (_methodControls.ContainsKey(method)) return _methodControls[method];
            return new Control[0];
        }

        public Control[] GetFieldControls(SerializedField field)
        {
            if (_fieldControls.ContainsKey(field)) return _fieldControls[field];
            return new Control[0];
        }

        public FieldLayout GetDefaultLayout(Rectangle rect)
        {
            var fieldRect = rect.Deformed(rect.Width - FieldWidth, 0, 0, 0);
            var labelRect = rect.Deformed(0, FieldWidth + HorizontalSpacing, 0, 0);

            return new FieldLayout(labelRect, fieldRect);
        }
    }
}
