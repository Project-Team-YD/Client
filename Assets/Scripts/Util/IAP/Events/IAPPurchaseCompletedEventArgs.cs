#if UNITY_PURCHASING
using System;
using UnityEngine.Purchasing;

namespace HSMLibrary.IAP
{
    public class IAPPurchaseCompletedEventArgs : EventArgs
    {
        public IAPPurchaseResult result { get; }

        public IAPPurchaseCompletedEventArgs(IAPPurchaseResult result_)
        {
            result = result_;
        }
    }
}
#endif