#if UNITY_PURCHASING
using System;

namespace HSMLibrary.IAP
{
    public class IAPPurchaseInitiatedEventArgs : EventArgs
    {
        public string productId { get; }
        public bool isRestored { get; }

        public IAPPurchaseInitiatedEventArgs(string productId_, bool restored_)
        {
            productId = productId_;
            isRestored = restored_;
        }
    }
}
#endif