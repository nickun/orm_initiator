using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ORM.Initiator.Classes.Elements;

namespace ORM.Initiator.Classes
{
    internal static class ElementFactory
    {
        public static BaseElement Create(RootConfigElement rootConfig, XElement xelement)
        {
            switch(xelement.Name.LocalName)
            {
                case RootConfigElement.NodeName:
                    return new RootConfigElement(xelement);
                case FolderElement.NodeName:
                    return new FolderElement(rootConfig, xelement);
                case SequenceElement.NodeName:
                    return new SequenceElement(rootConfig, xelement);
                case EntityElement.NodeName:
                    return new EntityElement(rootConfig, xelement);
                case LoadElement.NodeName:
                    return new LoadElement(rootConfig, xelement);
                default:
                    return null;
            }
        }
    }
}
