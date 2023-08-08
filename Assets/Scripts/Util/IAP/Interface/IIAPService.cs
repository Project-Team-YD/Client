#if UNITY_PURCHASING
using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine.Purchasing;

namespace HSMLibrary.IAP
{
#region Enum
    public enum EStoreInitializeError
    {
        Unknown,
        StoreDisposed,
        PurchasingUnavailable,
        NoProductsAvailable,
        AppNotKnown
    }

    public enum EStorePurchaseError
    {
        Unknown,
        StoreDisposed,
        PurchasingUnavailable,
        ExistingPurchasePending,
        ProductUnavailable,
        SignatureInvalid,
        UserCanceled,
        PaymentDeclined,
        DuplicateTransaction,
        ReceiptNullOrEmpty,
        ReceiptValidationFailed,
        ReceiptValidationNotAvailable
    }
#endregion

    public interface IIAPService : IDisposable
    {
#region Event
        event EventHandler StoreInitialized;
        event EventHandler<IAPPurchaseInitializationFailed> StoreInitializationFailed;
        event EventHandler<IAPPurchaseInitiatedEventArgs> PurchaseInitiated;
        event EventHandler<IAPPurchaseCompletedEventArgs> PurchaseCompleted;
        event EventHandler<IAPPurchaseFailedEventArgs> PurchaseFailed;
#endregion

        SourceSwitch TraceSwitch { get; }
        TraceListenerCollection TraceListeners { get; }

#region Interface
        IObservable<IAPProductInfo> Purchases { get; }
        IIAPProductInfoCollection Products { get; }
        IStoreController Controller { get; }
#endregion

        bool IsInitialized { get; }
        bool IsBusy { get; }

#region Task
        UniTask InitializeAsync();
        UniTask FetchAsync();
        UniTask<IAPPurchaseResult> PurchaseAsync(string _productId);
#endregion
    }
}
#endif
