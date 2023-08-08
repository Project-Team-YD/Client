#if UNITY_PURCHASING
using System;
using System.Runtime.Serialization;
using UnityEngine.Purchasing;

namespace HSMLibrary.IAP
{
    [Serializable]
    public sealed class IAPPurchaseException : IAPException
    {
        public EStorePurchaseError reason { get; }

        public IAPPurchaseResult result { get; }

        public IAPPurchaseException()
        {

        }

        public IAPPurchaseException(string message_)
            : base(message_)
        {
        }

        public IAPPurchaseException(string message_, System.Exception innerException_)
            : base(message_, innerException_)
        {
        }

        public IAPPurchaseException(IAPPurchaseResult result_, EStorePurchaseError reason_)
            : base(reason_.ToString())
        {
            result = result_;
            reason = reason_;
        }

        public IAPPurchaseException(IAPPurchaseResult result_, EStorePurchaseError reason_, System.Exception innerException_)
            : base(reason_.ToString(), innerException_)
        {
            result = result_;
            reason = reason_;
        }

        private IAPPurchaseException(SerializationInfo info_, StreamingContext context_)
            : base(info_, context_)
        {
        }

        public override void GetObjectData(SerializationInfo info_, StreamingContext context_)
        {
            base.GetObjectData(info_, context_);
        }
    }
}
#endif