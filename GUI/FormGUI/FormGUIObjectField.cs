using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdvancedForms.GUI
{
    partial class FormGUI 
    {
        private Control[] InitNullObject(SerializedField field, string fieldLabel)
        {
            var fieldControls = new List<Control>();

            if (fieldLabel != null) fieldControls.Add(InitLabel(fieldLabel));
            var linkLabel = InitLinkLabel("Create Instance");
            linkLabel.Click += new EventHandler((s, a) => {
                field.SetValue(Activator.CreateInstance(field.SerializedType));
                field.SerializedObject.Update();
            });
            fieldControls.Add(linkLabel);

            return fieldControls.ToArray();
        }

        public void ResizeObjectField(SerializedField field, FieldLayout layout, Control[] controls)
        {
            if (field.GetValue() == null)
            {
                DefaultResize(layout.LabelRect, controls[0]);
                DefaultResize(layout.ValueRect, controls[1]);
                return;
            }

            var labelRect = layout.LabelRect;
            labelRect.Height = FormGUIUtils.LineHeight;
            controls[0].SetRectangle(labelRect);
            var workRect = layout.FullRect.Deformed(FormGUIUtils.Indent.Horizontal, 0, labelRect.Height, 0);

            foreach (var subField in field.Fields)
            {
                var subFieldHeight = FieldHeight(subField);
                var subRect = new Rectangle(workRect.Location, new Size(workRect.Width, subFieldHeight));
                var subLayout = FormGUIUtils.GetDefaultLayout(subRect);
                ResizeField(subField, subLayout, FormGUIUtils.GetFieldControls(subField));
                workRect.UpDeform(subFieldHeight + FormGUIUtils.VerticalSpacing);
            }
        }

        public Control[] InitObjectField(SerializedField field, string fieldLabel)
        {
            if (field.GetValue() == null) return InitNullObject(field, fieldLabel);

            var fieldControls = new List<Control>();

            if (fieldLabel != null) fieldControls.Add(InitLabel(fieldLabel));
            foreach (var subField in field.Fields)
            {
                var subFieldLabel = FormGUIUtils.GetName(subField);
                fieldControls.AddRange(InitField(subField, subFieldLabel));
            }

            return fieldControls.ToArray();
        }
    }
}
