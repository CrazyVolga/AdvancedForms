using System.Drawing;
using System.Windows.Forms;

namespace AdvancedForms.GUI
{
    partial class FormGUI
    {
        public Button InitButton(string text)
        {
            var button = new Button();
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.AutoSize = false;
            button.Text = text;
            button.UseVisualStyleBackColor = true;

            return button;
        }

        public MaskedTextBox InitMaskedTextBox(string mask, string text = "")
        {
            var textBox = new MaskedTextBox();
            textBox.Mask = mask;

            textBox.Text = text;

            return textBox;
        }

        public NumericUpDown InitNumericUpDown(decimal min, decimal max, decimal value = default)
        {
            var numericUpDown = new NumericUpDown();
            numericUpDown.TextAlign = HorizontalAlignment.Left;
            numericUpDown.Minimum = min;
            numericUpDown.Maximum = max;

            numericUpDown.Value = value;

            return numericUpDown;
        }

        public Label InitLabel(string name)
        {
            var label = new Label();
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Text = name;

            return label;
        }

        public Label InitLinkLabel(string name)
        {
            var label = new LinkLabel();
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Text = name;

            return label;
        }

        public TextBox InitTextBox(string text = "")
        {
            var textBox = new TextBox();
            textBox.Text = text;

            return textBox;
        }

        public CheckBox InitCheckBox(bool check)
        {
            var checkBox = new CheckBox();
            checkBox.Checked = check;

            return checkBox;
        }

        public ComboBox InitComboBox(string[] items, int selectedIndex = 0)
        {
            var comboBox = new ComboBox();
            comboBox.Items.AddRange(items);
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.SelectedIndex = selectedIndex;

            return comboBox;
        }
    }
}
