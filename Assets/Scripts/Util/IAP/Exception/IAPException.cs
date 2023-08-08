#if UNITY_PURCHASING
using System;
using System.Runtime.Serialization;

namespace HSMLibrary.IAP
{
    [Serializable]
    public class IAPException : System.Exception
    {
        public IAPException()
            : base("Purchase error")
        {

        }

        public IAPException(string message)
            : base(message)
        {
        }

        public IAPException(string message, System.Exception innerException)
            : base(message, innerException)
        {

        }

        protected IAPException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
#endif