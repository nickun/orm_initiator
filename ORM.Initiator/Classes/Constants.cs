using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ORM.Initiator.Classes
{
    public class Constants
    {
        /// <summary>
        /// Xml tags
        /// </summary>
        internal static class Tags
        {
            public const string ROOT_CONFIG_NAMESPACE = "urn:initial-data-1.0";
            public const string ROOT_CONFIG_NODE = "hibernate-data-config";
            public const string NAMESPACE_TAG = "namespace";
            public const string ASSEMBLY_TAG = "assembly";

            public const string FOLDER_NODE = "folder";
            public const string SEQUENCE_NODE = "sequence";
            public const string ENTITY_NODE = "entity";
            public const string PROPERTY_NODE = "property";
            public const string LOAD_NODE = "load";

            public const string NAME_TAG = "name";
            public const string CLASS_TAG = "class";
            public const string REF_TAG = "ref";
            public const string SAVE_TAG = "save";
            public const string TYPE_TAG = "type";
            public const string ENUM_TYPE_TAG = "enumtype";
            public const string MIN_TAG = "min";
            public const string MAX_TAG = "max";
            public const string STEP_TAG = "step";
            public const string PROGRESS_TAG = "progress";
        }
    }
}
