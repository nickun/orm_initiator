using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ORM.Initiator.EntityConstructors;

namespace ORM.Initiator.Classes.Elements
{
    public abstract class ContainerElement : BaseElement
    {
        private readonly IList<BaseElement> _elements = new List<BaseElement>();

        protected ContainerElement()
        {
        }

        protected ContainerElement(RootConfigElement rootConfig, XElement xmlElement, BaseElement parent = null)
            : base(rootConfig, xmlElement)
        {
            Parent = parent;
        }

        /// <summary>
        /// Collection of EntityElement or FolderElement
        /// </summary>
        public IList<BaseElement> Elements
        {
            get { return _elements; }
        }

        /// <summary>
        /// RootConfig element for this element
        /// </summary>
        public override RootConfigElement RootConfig
        {
            get { return base.RootConfig; }
            protected internal set
            {
                base.RootConfig = value;
                // set RootConfig to children
                foreach (BaseElement element in Elements)
                {
                    element.RootConfig = value;
                }
            }
        }

        /// <summary>
        /// Entity constructor for buid and execute entities
        /// </summary>
        public override IEntityConstructor EntityConstructor
        {
            get { return base.EntityConstructor; }
            protected internal set
            {
                base.EntityConstructor = value;
                // set to children
                foreach (BaseElement element in Elements)
                {
                    element.EntityConstructor = value;
                }
            }
        }


        protected override void InnerInitialize(XElement xelement)
        {
            Elements.Clear();
            base.InnerInitialize(xelement);

            // child elements
            if (xelement.HasElements)
            {
                foreach (XElement xElement in xelement.Elements())
                {
                    var elem = ElementFactory.Create(RootConfig, xElement);
                    if (elem != null)
                    {
                        elem.Parent = this;
                        Elements.Add(elem);
                    }
                    else
                        throw new ParseException("Unknown element", xElement.ToString());
                }
            }
        }

        protected internal override void Execute()
        {
            // execute the child elements
            try
            {
                //EntityConstructor.BeginConstructAndSaveEntity();

                foreach (BaseElement element in Elements)
                {
                    var entityElement = element as EntityElement;
                    if (entityElement != null)
                        EntityConstructor.ConstructAndSaveEntity(entityElement);
                    else
                        element.Execute();
                }
            }
            finally
            {
                //EntityConstructor.EndConstructAndSaveEntity();
            }
        }
    }
}
