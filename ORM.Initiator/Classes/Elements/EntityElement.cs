using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ORM.Initiator.Classes.Elements
{
    [DebuggerDisplay("<{ElementName} class={ClassName} ref={RefName}>")]
    public class EntityElement : BaseElement
    {
        public const string NodeName = Constants.Tags.ENTITY_NODE;
        private string _classname;
        private string _refname;
        private bool _save = true;
        private IList<EntityProperty> _properties = new List<EntityProperty>();

        public EntityElement(RootConfigElement rootConfig, string className, string refName = "")
        {
            RootConfig = rootConfig;
            ClassName = className;
            RefName = refName;
        }

        public EntityElement(RootConfigElement rootConfig, XElement xmlElement)
            : base(rootConfig, xmlElement)
        {
        }

        /// <summary>
        /// Node name in the xml documents
        /// </summary>
        public override string ElementName
        {
            get { return NodeName; }
        }

        /// <summary>
        /// Class name
        /// </summary>
        public string ClassName
        {
            get { return _classname; }
            protected internal set
            {
                _classname = value ?? "";
                InnerXElement.SetAttributeValue(Constants.Tags.CLASS_TAG, value);
            }
        }

        /// <summary>
        /// Reference name.
        /// Usually empty if no links to the entity.
        /// </summary>
        public string RefName
        {
            get { return _refname; }
            protected internal set
            {
                _refname = value ?? "";
                InnerXElement.SetAttributeValue(Constants.Tags.REF_TAG, value);
            }
        }

        /// <summary>
        /// Save entity or not.
        /// Default is true.
        /// </summary>
        public bool Save
        {
            get { return _save; }
            protected internal set
            {
                _save = value;
                InnerXElement.SetAttributeValue(Constants.Tags.SAVE_TAG, value);
            }
        }

        /// <summary>
        /// Enumerate properties
        /// </summary>
        public IEnumerable<EntityProperty> Properties
        {
            get { return _properties; }
        }

        #region Properties construct

        public EntityProperty CreateStringProperty(string propName, string value)
        {
            return InnerAfterCreateTypeProperty(new EntityProperty(this, propName, value));
        }

        public EntityProperty CreateIntProperty(string propName, int value)
        {
            return InnerAfterCreateTypeProperty(new EntityProperty(this, propName, value));
        }

        public EntityProperty CreateEnumProperty(string propName, Enum value)
        {
            return InnerAfterCreateTypeProperty(new EntityProperty(this, propName, value));
        }

        public EntityProperty CreateBoolProperty(string propName, bool value)
        {
            return InnerAfterCreateTypeProperty(new EntityProperty(this, propName, value));
        }

        public EntityProperty CreateDecimalProperty(string propName, decimal value)
        {
            return InnerAfterCreateTypeProperty(new EntityProperty(this, propName, value));
        }

        public EntityProperty CreateDoubleProperty(string propName, double value)
        {
            return InnerAfterCreateTypeProperty(new EntityProperty(this, propName, value));
        }

        public EntityProperty CreateReferenceProperty(string propName, string refName)
        {
            return InnerAfterCreateTypeProperty(new EntityProperty(this, propName, new EntityProperty.Reference(refName)));
        }

        private EntityProperty InnerAfterCreateTypeProperty(EntityProperty entityProperty)
        {
            _properties.Add(entityProperty);
            InnerXElement.Add(entityProperty.ToXElement());
            return entityProperty;
        }
        #endregion


        protected override void InnerInitialize(XElement xelement)
        {
            _properties.Clear();
            // attribute 'class'
            var attribute = xelement.Attribute(Constants.Tags.CLASS_TAG);
            ClassName = attribute == null ? "" : attribute.Value;
            // attribute 'ref'
            attribute = xelement.Attribute(Constants.Tags.REF_TAG);
            RefName = attribute == null ? "" : attribute.Value;
            // attribute 'save'
            attribute = xelement.Attribute(Constants.Tags.SAVE_TAG);
            Save = attribute == null ? Save : Boolean.Parse(attribute.Value);
            // create properties from child elements
            foreach (var element in xelement.Elements())
            {
                string type = EntityProperty.ValueTypes.String.Name;
                string propertyName;
                if (element.Name.LocalName == EntityProperty.ELEMENT_NAME_PROPERTY)
                {
                    var nameAttr = element.Attribute(Constants.Tags.NAME_TAG);
                    if (nameAttr == null)
                        throw new ParseException("Missing \"name\" attribute for the property", element.ToString());
                    propertyName = nameAttr.Value;
                }
                else
                    propertyName = element.Name.LocalName;

                var typeAttr = element.Attribute(Constants.Tags.TYPE_TAG) ??
                                element.Attribute(Constants.Tags.ENUM_TYPE_TAG);
                if (typeAttr != null)
                {
                    if (typeAttr.Name.LocalName == Constants.Tags.ENUM_TYPE_TAG)
                        type = "Enum." + typeAttr.Value;
                    else
                        type = typeAttr.Value;
                }

                var prop = EntityProperty.CreateProperty(this, propertyName, type, element.Value);
                if (prop != null)
                    _properties.Add(prop);
            }

            base.InnerInitialize(xelement);
        }
    }
}
