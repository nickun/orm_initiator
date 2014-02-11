using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ORM.Initiator.Classes.Elements
{
    [DebuggerDisplay("<{ElementName} ref={RefName}>")]
    public class LoadElement : BaseElement
    {
        public const string NodeName = Constants.Tags.LOAD_NODE;
        private string _where;
        private string _refname;

        public LoadElement(RootConfigElement rootConfig, XElement xmlElement)
            : base(rootConfig, xmlElement)
        {
        }

        protected override void InnerInitialize(XElement xelement)
        {
            XAttribute attribute = xelement.Attribute(Constants.Tags.REF_TAG);
            RefName = attribute == null ? "" : attribute.Value;
            Where = xelement.Value;

            base.InnerInitialize(xelement);
        }

        /// <summary>
        /// Node name in the xml documents
        /// </summary>
        public override string ElementName
        {
            get { return NodeName; }
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
        /// HQL for get item
        /// </summary>
        public string Where
        {
            get { return _where; }
            protected internal set
            {
                _where = value ?? "";
                InnerXElement.SetValue(value);
            }
        }

        protected internal override void Execute()
        {
            EntityConstructor.AddReference(this);
        }
    }
}
