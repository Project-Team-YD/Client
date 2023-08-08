#if UNITY_PURCHASING
using System;

namespace HSMLibrary.IAP
{
    public class IAPPurchaseInitializationFailed : EventArgs
    {
        public EStoreInitializeError reason { get; }

        public System.Exception exception { get; }

        public IAPPurchaseInitializationFailed(EStoreInitializeError reason_, System.Exception e_)
        {
            reason = reason_;
            exception = e_;
        }
    }
}
#endif