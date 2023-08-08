#if UNITY_PURCHASING
using System;
using System.Runtime.Serialization;
using UnityEngine.Purchasing;

namespace HSMLibrary.IAP
{
    [Serializable]
    public sealed class IAPInitializeException : IAPException
    {
        public InitializationFailureReason reason { get; }

        public IAPInitializeException()
        {

        }

        public IAPInitializeException(string _message)
            : base(_message)
        {

        }

        public IAPInitializeException(InitializationFailureReason _reason)
            : base(_reason.ToString())
        {
            reason = _reason;
        }

        public IAPInitializeException(string _message, System.Exception _innerException)
            : base(_message, _innerException)
        {

        }

        private IAPInitializeException(SerializationInfo _info, StreamingContext _context)
            : base(_info, _context)
        {
        }

        public override void GetObjectData(SerializationInfo info_, StreamingContext context_)
        {
            base.GetObjectData(info_, context_);
        }
    }
}
#endif