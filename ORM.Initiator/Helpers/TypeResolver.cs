using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ORM.Initiator.Helpers
{
    public static class TypeResolver
    {
        public static Type FindType(string assembly, string fullTypeName)
        {
            string typeQualifiedName = Assembly.CreateQualifiedName(
                assembly, fullTypeName);
            return Type.GetType(typeQualifiedName);
        }
    }
}
