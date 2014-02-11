using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using ORM.Initiator.Classes.Elements;
using ORM.Initiator.Helpers;

namespace ORM.Initiator.Classes
{
    [DebuggerDisplay("Property {Name}: {Value}")]
    public class EntityProperty
    {
        public const string ELEMENT_NAME_PROPERTY = Constants.Tags.PROPERTY_NODE;
        private EntityElement _parent;
        private string _name;
        private Type _valueType = ValueTypes.String;
        private string _value;
        private object _objValue;
        public static readonly IFormatProvider InvariantFormatProvider;

        static EntityProperty()
        {
            InvariantFormatProvider = NumberFormatInfo.InvariantInfo;
        }

        public static class ValueTypes
        {
            public static Type String = typeof (string);
            public static Type Int = typeof (int);
            public static Type Bool = typeof(bool);
            public static Type Byte = typeof(byte);
            public static Type Decimal = typeof (decimal);
            public static Type Double = typeof (double);
            public static Type DateTime = typeof(DateTime);
            public static Type Reference = typeof(Reference);
            public static Type Binary = typeof(byte[]);
        }

        public struct Reference
        {
            private string _refName;
            private Type _refType;
            private object _refObject;

            public Reference(string refName)
            {
                _refName = refName;
                _refType = Type.Missing.GetType();
                _refObject = null;
            }

            public string RefName
            {
                get { return _refName; }
                set { _refName = value; }
            }

            public Type RefType
            {
                get { return _refType; }
                set { _refType = value; }
            }

            public object RefObject
            {
                get { return _refObject; }
                set { _refObject = value; }
            }
        }

        /// <summary>
        /// Constructor for string type
        /// </summary>
        /// <param name="parent">entity element</param>
        /// <param name="name">property name</param>
        public EntityProperty(EntityElement parent, string name)
            : this(parent, name, ValueTypes.String, "")
        {
        }

        /// <summary>
        /// Constructor for string type with value
        /// </summary>
        /// <param name="parent">entity element</param>
        /// <param name="name">property name</param>
        /// <param name="value">property value</param>
        public EntityProperty(EntityElement parent, string name, string value)
            : this(parent, name, ValueTypes.String, value)
        {
            _objValue = value;
        }

        /// <summary>
        /// Constructor for int type with value
        /// </summary>
        /// <param name="parent">entity element</param>
        /// <param name="name">property name</param>
        /// <param name="value">property value</param>
        public EntityProperty(EntityElement parent, string name, int value)
            : this(parent, name, ValueTypes.Int, value.ToString())
        {
            _objValue = value;
        }

        /// <summary>
        /// Constructor for int type with value
        /// </summary>
        /// <param name="parent">entity element</param>
        /// <param name="name">property name</param>
        /// <param name="value">property value</param>
        public EntityProperty(EntityElement parent, string name, bool value)
            : this(parent, name, ValueTypes.Bool, value.ToString(InvariantFormatProvider))
        {
            _objValue = value;
        }

        /// <summary>
        /// Constructor for decimal type with value
        /// </summary>
        /// <param name="parent">entity element</param>
        /// <param name="name">property name</param>
        /// <param name="value">property value</param>
        public EntityProperty(EntityElement parent, string name, decimal value)
            : this(parent, name, ValueTypes.Decimal, value.ToString(InvariantFormatProvider))
        {
            _objValue = value;
        }

        /// <summary>
        /// Constructor for double type with value
        /// </summary>
        /// <param name="parent">entity element</param>
        /// <param name="name">property name</param>
        /// <param name="value">property value</param>
        public EntityProperty(EntityElement parent, string name, double value)
            : this(parent, name, ValueTypes.Double, value.ToString(InvariantFormatProvider))
        {
            _objValue = value;
        }

        /// <summary>
        /// Constructor for DateTime type with value
        /// </summary>
        /// <param name="parent">entity element</param>
        /// <param name="name">property name</param>
        /// <param name="value">property value</param>
        public EntityProperty(EntityElement parent, string name, DateTime value)
            : this(parent, name, ValueTypes.DateTime, value.ToUniversalTime().ToString("s"))
        {
            _objValue = value;
        }

        /// <summary>
        /// Constructor for enum type with value
        /// </summary>
        /// <param name="parent">entity element</param>
        /// <param name="name">property name</param>
        /// <param name="value">property value</param>
        public EntityProperty(EntityElement parent, string name, Enum value)
            : this(parent, name, value.GetType(), value.ToString())
        {
            _objValue = value;
        }

        /// <summary>
        /// Constructor for given type with value
        /// </summary>
        /// <param name="parent">entity element</param>
        /// <param name="name">property name</param>
        /// <param name="type">property type</param>
        /// <param name="value">property value</param>
        public EntityProperty(EntityElement parent, string name, Type type, string value)
        {
            _parent = parent;
            _name = name;
            _valueType = type;
            _value = value;
            _objValue = value;
        }

        /// <summary>
        /// Constructor for given type with value
        /// </summary>
        /// <param name="parent">entity element</param>
        /// <param name="name">property name</param>
        /// <param name="reference">describes reference</param>
        public EntityProperty(EntityElement parent, string name, Reference reference)
        {
            _parent = parent;
            _name = name;
            _valueType = ValueTypes.Reference;
            _value = reference.RefName;
        }

        /// <summary>
        /// Constructor for binary type
        /// </summary>
        /// <param name="parent">entity element</param>
        /// <param name="name">property name</param>
        /// <param name="value">property value</param>
        /// <param name="data">describes reference</param>
        public EntityProperty(EntityElement parent, string name, string value, byte[] data)
        {
            _parent = parent;
            _name = name;
            _valueType = ValueTypes.Binary;
            _value = value;
            _objValue = data;
        }

        /// <summary>
        /// Name of the property
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Type of the property
        /// </summary>
        public Type ValueType
        {
            get { return _valueType; }
            set { _valueType = value; }
        }

        /// <summary>
        /// A string representation of the value
        /// or a name of a link to the object
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Parsed string value for value type
        /// or object reference for a reference type
        /// </summary>
        public object ObjectValue
        {
            get { return _objValue; }
            set { _objValue = value; }
        }

        public EntityElement Parent
        {
            get { return _parent; }
        }

        /// <summary>
        /// Factory method
        /// </summary>
        /// <returns>instance of EntityProperty</returns>
        internal static EntityProperty CreateProperty(EntityElement parent, string name, string type, string value)
        {
            // string
            if (String.IsNullOrEmpty(type) || ValueTypes.String.Name == type)
                return new EntityProperty(parent, name, value);

            // value types
            try
            {
                if (ValueTypes.Int.Name.Equals(type, StringComparison.InvariantCultureIgnoreCase) || "int" == type ||
                    "integer" == type)
                    return new EntityProperty(parent, name, Int32.Parse(value));
                if (ValueTypes.Bool.Name.Equals(type, StringComparison.InvariantCultureIgnoreCase) || "bool" == type)
                    return new EntityProperty(parent, name, Boolean.Parse(value));
                if (ValueTypes.Byte.Name.Equals(type, StringComparison.InvariantCultureIgnoreCase))
                    return new EntityProperty(parent, name, Byte.Parse(value, InvariantFormatProvider));
                if (ValueTypes.Decimal.Name.Equals(type, StringComparison.InvariantCultureIgnoreCase))
                    return new EntityProperty(parent, name, Decimal.Parse(value, InvariantFormatProvider));
                if (ValueTypes.Double.Name.Equals(type, StringComparison.InvariantCultureIgnoreCase))
                    return new EntityProperty(parent, name, Double.Parse(value, InvariantFormatProvider));
                if (ValueTypes.DateTime.Name.Equals(type, StringComparison.InvariantCultureIgnoreCase))
                    return new EntityProperty(parent, name, DateTime.Parse(value, InvariantFormatProvider));
                if (ValueTypes.Binary.Name.Equals(type, StringComparison.InvariantCultureIgnoreCase))
                    return new EntityProperty(parent, name, value, Convert.FromBase64String(value));
            }
            catch(Exception ex)
            {
                throw new ParseException(String.Format("Can not convert string value '{0}' to type '{1}'. See element:\r\n{2}",
                        value, type, parent.ToXmlString()), parent.ToXmlString(), ex);
            }

            // reference
            if (ValueTypes.Reference.Name.Equals(type, StringComparison.InvariantCultureIgnoreCase))
                return new EntityProperty(parent, name, new Reference(value));

            // enum
            if (type.StartsWith("Enum."))
            {
                Type enumType = TypeResolver.FindType(parent.RootConfig.Assembly,
                    parent.RootConfig.Namespace + "." + type.Substring("Enum.".Length, type.Length - "Enum.".Length));
                if (enumType != null)
                    return new EntityProperty(parent, name, (Enum)Enum.Parse(enumType, value));
                else
                    throw new ParseException(String.Format("Can not convert string value '{0}' to type '{1}'.\r\nType not found.\r\nSee element:\r\n{2}",
                        value, type, parent.ToXmlString()), parent.ToXmlString());
            }

            return null;
        }

        public XElement ToXElement()
        {
            var propElem = new XElement(ELEMENT_NAME_PROPERTY);
            propElem.SetAttributeValue("name", Name);
            if (ValueType != ValueTypes.String)
                propElem.SetAttributeValue(ValueType.IsEnum ? Constants.Tags.ENUM_TYPE_TAG : Constants.Tags.TYPE_TAG, ValueType.Name);
            propElem.SetValue(Value);
            return propElem;
        }
    }
}