using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ORM.Initiator.Classes.Elements
{
    [DebuggerDisplay("<{ElementName} name={Name}>")]
    public class FolderElement : ContainerElement
    {
        public const string NodeName = Constants.Tags.FOLDER_NODE;
        private string _name;

        public FolderElement(string name)
        {
            Name = name;
        }

        public FolderElement(RootConfigElement rootConfig, XElement xmlElement)
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
        /// Folder name
        /// </summary>
        public string Name
        {
            get { return _name; }
            protected internal set
            {
                _name = value ?? "<unnamed>";
                InnerXElement.SetAttributeValue(Constants.Tags.NAME_TAG, value);
            }
        }


        protected override void InnerInitialize(XElement xelement)
        {
            // attribute 'name'
            var attribute = xelement.Attribute(Constants.Tags.NAME_TAG);
            Name = attribute == null ? null : attribute.Value;
            
            base.InnerInitialize(xelement);
        }
    }
}
