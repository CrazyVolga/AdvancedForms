using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AdvancedForms.GUI
{
    partial class FormGUI
    {
        private Control[] InitFloatNumericField(SerializedField field, int decimalPlaces, string fieldLabel)
        {
            var controls = InitNumericField(field, fieldLabel);
            var numericUpDown = controls.First(control => control is NumericUpDown) as NumericUpDown;
            numericUpDown.ValueChanged += new EventHandler((s, e) =>
            {
                var thisNumericUpDown = s as NumericUpDown;
                int count = BitConverter.GetBytes(decimal.GetBits(thisNumericUpDown.Value)[3])[2];
                thisNumericUpDown.DecimalPlaces = count > decimalPlaces ? count : decimalPlaces;
            });

            int c = BitConverter.GetBytes(decimal.GetBits(numericUpDown.Value)[3])[2];
            numericUpDown.DecimalPlaces = c > decimalPlaces ? c : decimalPlaces;

            return controls;
        }

        public Control[] InitNumericField(SerializedField field, string fieldLabel)
        {
            var fieldControls = new List<Control>();

            if (fieldLabel != null) fieldControls.Add(InitLabel(fieldLabel));

            decimal maxValue, minValue;
            try
            {
                maxValue = (decimal)Convert.ChangeType(field.SerializedType.GetField("MaxValue").GetRawConstantValue(), typeof(decimal));
                minValue = (decimal)Convert.ChangeType(field.SerializedType.GetField("MinValue").GetRawConstantValue(), typeof(decimal));
            }
            catch
            {
                maxValue = decimal.MaxValue;
                minValue = decimal.MinValue;
            }

            var control = InitNumericUpDown(minValue, maxValue, Convert.ToDecimal(field.GetValue()));
            control.KeyPress += new KeyPressEventHandler((s, e) => { if (e.KeyChar == '.') e.KeyChar = ','; });
            control.ValueChanged += new EventHandler((s, e) =>
            {
                var thisNumericUpDown = s as NumericUpDown;
                var val = Convert.ChangeType(thisNumericUpDown.Value, field.SerializedType);
                field.SetValue(val);
            });
            fieldControls.Add(control);

            return fieldControls.ToArray();
        }

        public Control[] InitStringField(SerializedField field, string fieldLabel)
        {
            var fieldControls = new List<Control>();

            if (fieldLabel != null) fieldControls.Add(InitLabel(fieldLabel));
            var textBox = InitTextBox((string)field.GetValue());
            textBox.TextChanged += new EventHandler((s, e) =>
            {
                var thisTextBox = s as TextBox;
                field.SetValue(thisTextBox.Text);
            });
            fieldControls.Add(textBox);

            return fieldControls.ToArray();
        }

        public Control[] InitEnumField(SerializedField field, string fieldLabel)
        {
            Dictionary<string, Enum> nameAssociations = Enum.GetValues(field.SerializedType).Cast<Enum>().ToDictionary(
                enumVal => FormGUIUtils.GetDefaultName(field.SerializedType.GetMember(enumVal.ToString()).SingleOrDefault()),
                enumVal => enumVal);

            var fieldControls = new List<Control>();
            if (fieldLabel != null) fieldControls.Add(InitLabel(fieldLabel));

            var selectedIndex = Array.IndexOf(nameAssociations.Values.ToArray(), field.GetValue());
            var comboBox = InitComboBox(nameAssociations.Keys.ToArray(), selectedIndex);
            comboBox.SelectedIndexChanged += new EventHandler((s, e) => {
                var thisComboBox = s as ComboBox;
                field.SetValue(nameAssociations[(string)thisComboBox.SelectedItem]);
            });
            fieldControls.Add(comboBox);

            return fieldControls.ToArray();
        }

        public Control[] InitCharField(SerializedField field, string fieldLabel)
        {
            var fieldControls = new List<Control>();
            if (fieldLabel != null) fieldControls.Add(InitLabel(fieldLabel));
            var maskedTextBox = InitMaskedTextBox("a");
            maskedTextBox.TextChanged += new EventHandler((s, e) =>
            {
                var thisMaskedTextBox = s as MaskedTextBox;
                try
                {
                    field.SetValue(thisMaskedTextBox.Text);
                    thisMaskedTextBox.Text = field.GetValue().ToString();
                }
                catch { }
            });
            fieldControls.Add(maskedTextBox);

            return fieldControls.ToArray();
        }

        public Control[] InitBoolField(SerializedField field, string fieldLabel)
        {
            var fieldControls = new List<Control>();

            if (fieldLabel != null) fieldControls.Add(InitLabel(fieldLabel));
            var checkBox = InitCheckBox((bool)field.GetValue());
            checkBox.CheckedChanged += new EventHandler((s, e) => {
                var thisCheckBox = s as CheckBox;
                field.SetValue(thisCheckBox.Checked);
            });
            fieldControls.Add(checkBox);

            return fieldControls.ToArray();
        }
    }
}
