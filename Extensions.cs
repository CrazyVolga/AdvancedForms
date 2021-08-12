using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using AdvancedForms.ControlConstructors;

namespace AdvancedForms
{
    public static class Extensions
    {
        internal static bool IsStruct(this Type type)
        {
            return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
        }

        public static void SetRectangle(this Control control, Rectangle rect)
        {
            control.Location = rect.Location;
            control.Size = rect.Size;
        }

        internal static IEnumerable<Type> GetTypesWithAttribute<A>(Assembly assembly)
        {
            Type[] assemblyTypes;

            try { assemblyTypes = assembly.GetTypes(); }
            catch { assemblyTypes = Array.Empty<Type>(); }

            foreach (Type type in assemblyTypes)
            {
                if (type.GetCustomAttributes(typeof(A), true).Length > 0)
                {
                    yield return type;
                }
            }
        }

        internal static Type[] GetCustomFieldsConstructors()
        {
            var attrs = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                attrs.AddRange(GetTypesWithAttribute<DefaultFieldControlsConstructor>(assembly));
            }
            
            return attrs.ToArray();
        }        

        public static Rectangle In(this Rectangle childRect, Rectangle parentRect)
        {
            var rect = childRect;
            rect.Offset(parentRect.Location);
            rect.Intersect(parentRect);

            return rect;
        }

        public static void Deform(this ref Rectangle rect, int leftDeform, int rightDeform, int upDeform, int downDeform)
        {
            rect.X += leftDeform;
            rect.Y += upDeform;
            rect.Width -= leftDeform + rightDeform;
            rect.Height -= upDeform + downDeform;
        }

        public static Rectangle Deformed(this Rectangle rect, Padding padding)
        {
            return rect.Deformed(padding.Left, padding.Right, padding.Top, padding.Bottom);
        }

        public static Rectangle Deformed(this Rectangle rect, int leftDeform, int rightDeform, int topDeform, int bottomDeform)
        {
            return new Rectangle(
                rect.X + leftDeform,
                rect.Y + topDeform,
                rect.Width - leftDeform - rightDeform,
                rect.Height - topDeform - bottomDeform
            );
        }

        public static void LeftDeform(this ref Rectangle rect, int value)
        {
            rect.Deform(value, 0, 0, 0);
        }

        public static void RightDeform(this ref Rectangle rect, int value)
        {
            rect.Deform(0, value, 0, 0);
        }

        public static void UpDeform(this ref Rectangle rect, int value)
        {
            rect.Deform(0, 0, value, 0);
        }

        public static void DownDeform(this ref Rectangle rect, int value)
        {
            rect.Deform(0, 0, 0, value);
        }
    }
}
