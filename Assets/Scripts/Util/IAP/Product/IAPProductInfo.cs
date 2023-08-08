#if UNITY_PURCHASING
using System;
//using UnityEngine.Purchasing;

namespace HSMLibrary.IAP
{
    public class IAPProductInfo : IAPPurchaseResult
    {
        public string productId { get; }

        public EStorePurchaseError? error { get; }

        public Exception exception { get; }

        public bool isSucceeded => !error.HasValue;
        public bool isFailed => error.HasValue;
        public bool isCanceled => error == EStorePurchaseError.UserCanceled;

        public IAPProductInfo(string _productId, IAPPurchaseResult _purchaseResult, EStorePurchaseError? _error, Exception _exception)
            : base(_purchaseResult.product, _purchaseResult.transactionInfo, _purchaseResult.validResult)
        {
            productId = _productId;
            error = _error;
            exception = _exception;
        }
    }
}
#endif