using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ORM.Initiator.EntityConstructors;

namespace ORM.Initiator.Classes.Elements
{
    [DebuggerDisplay("<{ElementName} namespace={Namespace} assembly={Assembly}>")]
    public class RootConfigElement : ContainerElement
    {
        public const string NodeName = Constants.Tags.ROOT_CONFIG_NODE;
        public static XName NodeNsXName;
        private string _namespace;
        private string _assembly;

        static RootConfigElement()
        {
            NodeNsXName = XName.Get(Constants.Tags.ROOT_CONFIG_NODE, Constants.Tags.ROOT_CONFIG_NAMESPACE);
        }

        public RootConfigElement()
        {
            RootConfig = this;
        }

        public RootConfigElement(XElement xmlElement)
            : base(null, xmlElement)
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
        /// namespase for Entity classes
        /// </summary>
        public string Namespace
        {
            get { return _namespace; }
            protected internal set
            {
                _namespace = value ?? "";
                InnerXElement.SetAttributeValue(Constants.Tags.NAMESPACE_TAG, value);
            }
        }

        /// <summary>
        /// assembly name where find Entity classes
        /// </summary>
        public string Assembly
        {
            get { return _assembly; }
            protected internal set
            {
                _assembly = value ?? "";
                InnerXElement.SetAttributeValue(Constants.Tags.ASSEMBLY_TAG, value);
            }
        }

        public override RootConfigElement RootConfig
        {
            get
            {
                return this;
            }
            protected internal set
            {
                // set self as RootConfig, not value!
                base.RootConfig = this;
            }
        }


        protected override void InnerInitialize(XElement xelement)
        {
            // attribute 'namespase'
            var attribute = xelement.Attribute(Constants.Tags.NAMESPACE_TAG);
            Namespace = attribute == null ? null : attribute.Value;
            // attribute 'assembly'
            attribute = xelement.Attribute(Constants.Tags.ASSEMBLY_TAG);
            Assembly = attribute == null ? null : attribute.Value;
            // initialize other components
            base.InnerInitialize(xelement);
        }


        public void ConstructAndSaveAllEntities()
        {
            if (EntityConstructor == null)
                throw new ConstructException("Entity constructor is not defined!");

            Execute();
        }

        public void CreateTestContent()
        {
            var entity = new EntityElement(this, "Bonitos", "bonitos1");
            entity.CreateStringProperty("Name", "Bob");
            entity.CreateBoolProperty("Disabled", true);
            entity.CreateDecimalProperty("Price", 125.2m);
        }
    }
}
