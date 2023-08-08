#if UNITY_PURCHASING
using System;

namespace HSMLibrary.IAP
{
    public class IAPPurchaseFailedEventArgs : EventArgs
    {
        public EStorePurchaseError error { get; }

        public string productId { get; }
        public IAPPurchaseResult result { get; }
        public System.Exception exception { get; }

        public IAPPurchaseFailedEventArgs(string productId_, IAPPurchaseResult result_, EStorePurchaseError error_, System.Exception e_)
        {
            productId = productId_;
            result = result_;
            error = error_;
            exception = e_;
        }
    }
}
#endif