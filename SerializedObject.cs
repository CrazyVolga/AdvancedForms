using AdvancedForms.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AdvancedForms
{
    public class SerializedObject
    {
        public object Object { get; private set; }

        internal event Action OnValueChanged;

        public Type SerializedType => Object.GetType();

        private SerializedField[] _fieldsEnumerable;
        public IEnumerable<SerializedField> Fields
        {
            get
            {
                if (_fieldsEnumerable == null)
                {
                    _fieldsEnumerable = FormGUIUtils.GetManagedFields(SerializedType).Select(f => new SerializedField(this, f)).ToArray();
                }

                return _fieldsEnumerable;
            }
        }

        private MethodInfo[] _methodsEnumerable; 
        public IEnumerable<MethodInfo> Methods
        {
            get
            {
                if (_methodsEnumerable == null)
                {
                    _methodsEnumerable = FormGUIUtils.GetManagedMethods(SerializedType);
                }

                return _methodsEnumerable;
            }
        }

        public SerializedObject(object serializedObject)
        {
            Object = serializedObject;
        }

        public SerializedField GetField(string fieldName)
        {
            return _fieldsEnumerable.FirstOrDefault(f => f.Name == fieldName);
        }

        public void Update()
        {
            OnValueChanged?.Invoke();
        }
    }
}
