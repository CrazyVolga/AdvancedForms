using AdvancedForms.ControlConstructors;
using AdvancedForms.GUI;
using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace AdvancedForms
{
    public class ManagedForm : Form
    {
        internal FormGUIUtils FormGUIUtils { get; private set; }
        internal FormGUI FormGUI { get; private set; }

        public Control ManagedControl { get; private set; }

        private static void FindCustomConstructors()
        {
            foreach (var constructorClass in Extensions.GetCustomFieldsConstructors())
            {
                var type = constructorClass.
                    GetCustomAttribute<DefaultFieldControlsConstructor>().fieldType;
                var constructor = constructorClass
                    .GetConstructor(new Type[] { })?
                    .Invoke(null) as FieldControlsConstructor;
                if (constructor == null)
                {
                    throw new ArgumentException($"Class {constructorClass} is not a {typeof(FieldControlsConstructor).Name} class");
                }
                FormGUI.AddCustomConstructor(type, constructor);
            }
        }

        static ManagedForm()
        {
            FindCustomConstructors();
        }

        public SerializedObject SerializedObject { get; private set; }

        public ManagedForm()
        {
            var initMethod = this.GetType().GetMethod("InitializeComponent", BindingFlags.NonPublic | BindingFlags.Instance);
            initMethod?.Invoke(this, Array.Empty<object>());

            var panels = Controls.Cast<Control>().Where(control => control is Panel).ToArray();
            if (panels.Length > 1)
            {
                try
                {
                    ManagedControl = panels.First(panel => panel.Name == "ManagedControl");
                }
                catch
                {
                    throw new MemberAccessException($"Form contains a few {typeof(Panel).Name} controls, but none named 'ManagedControl'");
                }
            }
            else if (panels.Length == 1) ManagedControl = panels[0];
            else ManagedControl = this;

            FormGUIUtils = new FormGUIUtils(this);
            FormGUI = new FormGUI(this);
            SerializedObject = new SerializedObject(this);
            SerializedObject.OnValueChanged += new Action(() => { InitializeManagedComponents(); ResizeManagedComponents(); });

            this.Text = FormGUIUtils.GetDefaultName(this.GetType());
            ManagedControl.SizeChanged += new EventHandler((s, e) => { ResizeManagedComponents(); });

            InitializeManagedComponents();
            ResizeManagedComponents();
        }

        public Rectangle WorkRectangle => ManagedControl == this ?
            ManagedControl.ClientRectangle.Deformed(
            FormGUIUtils.Indent.Horizontal,
            FormGUIUtils.Indent.Horizontal,
            FormGUIUtils.Indent.Vertical,
            FormGUIUtils.Indent.Vertical
        ) : ManagedControl.ClientRectangle.Deformed(ManagedControl.Padding);

        protected virtual void ResizeManagedComponents()
        {
            this.SuspendLayout();
            var workRectangle = WorkRectangle;
            foreach (SerializedField field in SerializedObject.Fields)
            {
                var fieldHeight = FormGUI.FieldHeight(field);
                var fieldRect = new Rectangle(workRectangle.Location, new Size(workRectangle.Width, fieldHeight));
                FormGUI.ResizeField(field, FormGUIUtils.GetDefaultLayout(fieldRect), FormGUIUtils.GetFieldControls(field));

                workRectangle.UpDeform(fieldHeight + FormGUIUtils.VerticalSpacing);
            }

            foreach (MethodInfo method in SerializedObject.Methods)
            {
                var methodRect = workRectangle;
                methodRect.Height = FormGUIUtils.LineHeight + FormGUIUtils.LineHeight / 4;

                FormGUI.ResizeMethod(method, methodRect, FormGUIUtils.GetMethodControls(method));
                workRectangle.UpDeform(methodRect.Height + FormGUIUtils.VerticalSpacing);
            }

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        protected virtual void InitializeManagedComponents()
        {
            this.SuspendLayout();
            ManagedControl.Controls.Clear();

            foreach (var field in SerializedObject.Fields)
            {
                var name = FormGUIUtils.GetName(field);
                var controls = FormGUI.InitField(field, name);
                ManagedControl.Controls.AddRange(controls);
            }

            foreach (MethodInfo method in SerializedObject.Methods)
            {
                var name = FormGUIUtils.GetDefaultName(method);
                var action = (Action)method.CreateDelegate(typeof(Action), this);
                var controls = FormGUI.InitMethod(method, action, name);
                ManagedControl.Controls.AddRange(controls);
            }

            this.ResumeLayout(false);
        }
    }
}