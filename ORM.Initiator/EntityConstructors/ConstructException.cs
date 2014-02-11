using System;

namespace ORM.Initiator.EntityConstructors
{
    [Serializable]
    public class ConstructException : ApplicationException
    {
        public ConstructException(string message)
            : this(message, null)
        {
        }

        public ConstructException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}