using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ORM.Initiator.EntityConstructors;

namespace ORM.Initiator.Classes.Elements
{
    public abstract class BaseElement
    {
        private volatile XElement _xElem;
        private XElement _backupXElem;
        protected readonly object SyncRoot = new object();
        private RootConfigElement _rootConfig;

        protected BaseElement() { }

        protected BaseElement(RootConfigElement rootConfig, XElement xmlElement)
        {
            _rootConfig = rootConfig;
            _xElem = xmlElement;
            BuildElement();
        }

        /// <summary>
        /// Node name in the xml documents
        /// </summary>
        public virtual string ElementName
        {
            get
            {
                Trace.TraceError("Property 'Name' must be overrided in class " + GetType().Name);
                return GetType().Name;
            }
        }

        /// <summary>
        /// Parent element for this element
        /// </summary>
        public BaseElement Parent { get; protected internal set; }

        /// <summary>
        /// RootConfig element for this element
        /// </summary>
        public virtual RootConfigElement RootConfig
        {
            get { return _rootConfig; }
            protected internal set
            {
                _rootConfig = value;
            }
        }

        /// <summary>
        /// Entity constructor for buid and execute entities
        /// </summary>
        public virtual IEntityConstructor EntityConstructor { get; protected internal set; }

        protected internal XElement InnerXElement
        {
            get
            {
                if (_xElem == null)
                {
                    lock (SyncRoot)
                    {
                        if (_xElem == null)
                            _xElem = new XElement(ElementName);
                    }
                }
                return _xElem;
            }
            protected set { _xElem = value; }
        }

        protected internal XElement BakInnerXElement
        {
            get
            {
                return _backupXElem;
            }
        }

        /// <summary>
        /// Returns the indented XML for this element.
        /// </summary>
        /// <returns></returns>
        public string ToXmlString()
        {
            return InnerXElement.ToString();
        }

        /// <summary>
        /// Building this element from XML
        /// </summary>
        /// <returns></returns>
        public BaseElement BuildElement()
        {
            InnerInitialize(InnerXElement);
            return this;
        }

        protected virtual void InnerInitialize(XElement xelement)
        {
        }

        protected internal virtual void Execute()
        {
        }

        public void BackupInnerXElement()
        {
            _backupXElem = new XElement(_xElem);
        }

        public void RestoreInnerXElement()
        {
            _xElem = new XElement(_backupXElem);
        }

        public bool IsBackuped
        {
            get { return _backupXElem != null; }
        }
    }
}
