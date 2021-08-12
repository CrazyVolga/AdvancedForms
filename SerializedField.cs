using AdvancedForms.GUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AdvancedForms
{
    public class SerializedField
    {
        private readonly FieldInfo _info;
        public SerializedObject SerializedObject { get; private set; }
        private object _object;

        private readonly string _path;
        public int Depth { get; private set; }
        public Type SerializedType => _info.FieldType;
        public string Name => _info.Name;


        private SerializedField[] _fieldsEnumerable;
        public IEnumerable<SerializedField> Fields { get
            {
                if (Type.GetTypeCode(SerializedType) == TypeCode.Object && GetValue() == null) return Array.Empty<SerializedField>();
                if (_fieldsEnumerable == null)
                {
                    _fieldsEnumerable = FormGUIUtils.GetManagedFields(SerializedType).Select(f => new SerializedField(this, f)).ToArray();
                }

                return _fieldsEnumerable;
            }
        }

        internal SerializedField(SerializedObject serializedObject, FieldInfo field)
        {
            SerializedObject = serializedObject;
            _info = field;
            _object = serializedObject.Object;
            Depth = 0;
            _path = $"/{serializedObject.Object.GetType().Name}";
        }

        private SerializedField(SerializedField serializedField, FieldInfo field)
        {
            SerializedObject = serializedField.SerializedObject;
            _info = field;
            _object = serializedField._info.GetValue(serializedField._object);
            Depth = serializedField.Depth + 1;
            _path = $"{serializedField._path}/{serializedField.Name}";
        }

        public SerializedField GetField(string fieldName)
        {
            return Fields.FirstOrDefault(f => f.Name == fieldName);
        }

        public T GetAttribute<T>() where T : Attribute
        {
            return _info.GetCustomAttribute<T>();
        }

        private SerializedField GetParrent()
        {
            var fields = _path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).Skip(1);

            SerializedField parent = null;
            foreach (var field in fields)
            {
                if (parent == null) parent = SerializedObject.GetField(field);
                else parent = parent.GetField(field);
            }

            return parent;
        }

        public void CascadeObjectUpdate()
        {
            var parent = GetParrent();
            if (parent != null) _object = GetParrent()._info.GetValue(parent._object);
            foreach(var child in Fields)
            {
                child.CascadeObjectUpdate();
            }
        }

        public void SetValue(object value)
        {
            if (value != null && value.Equals(GetValue())) return;
            _info.SetValue(_object, value);

            if (_object.GetType().IsStruct())
            {
                var parent = GetParrent();
                parent.SetValue(_object);
            }
            else if (SerializedType.IsStruct())
            {
                CascadeObjectUpdate();
            }
        }

        public object GetValue()
        {
            if (_object == null) return null;
            return _info.GetValue(_object);
        }

        public T GetValue<T>()
        {
            return (T)GetValue();
        }
    }
}
