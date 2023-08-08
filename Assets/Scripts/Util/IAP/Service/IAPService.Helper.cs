#if UNITY_PURCHASING
using System;
using System.Diagnostics;
using UnityEngine.Purchasing;

namespace HSMLibrary.IAP
{
    public partial class IAPService
    {
#region data
#endregion

#region interface

        internal static EStoreInitializeError GetInitializeError(InitializationFailureReason _error)
        {
            switch (_error)
            {
                case InitializationFailureReason.AppNotKnown:
                    return EStoreInitializeError.AppNotKnown;

                case InitializationFailureReason.NoProductsAvailable:
                    return EStoreInitializeError.NoProductsAvailable;

                case InitializationFailureReason.PurchasingUnavailable:
                    return EStoreInitializeError.PurchasingUnavailable;

                default:
                    return EStoreInitializeError.Unknown;
            }
        }

        internal static EStorePurchaseError GetPurchaseError(PurchaseFailureReason _error)
        {
            switch (_error)
            {
                case PurchaseFailureReason.PurchasingUnavailable:
                    return EStorePurchaseError.PurchasingUnavailable;

                case PurchaseFailureReason.ExistingPurchasePending:
                    return EStorePurchaseError.ExistingPurchasePending;

                case PurchaseFailureReason.ProductUnavailable:
                    return EStorePurchaseError.ProductUnavailable;

                case PurchaseFailureReason.SignatureInvalid:
                    return EStorePurchaseError.SignatureInvalid;

                case PurchaseFailureReason.UserCancelled:
                    return EStorePurchaseError.UserCanceled;

                case PurchaseFailureReason.PaymentDeclined:
                    return EStorePurchaseError.PaymentDeclined;

                case PurchaseFailureReason.DuplicateTransaction:
                    return EStorePurchaseError.DuplicateTransaction;

                default:
                    return EStorePurchaseError.Unknown;
            }
        }

#endregion

#region implementation

        private void InvokeInitializeCompleted(int _opID)
        {
            try
            {
                StoreInitialized?.Invoke(this, EventArgs.Empty);
            }
            catch (System.Exception e)
            {
                console.TraceData(TraceEventType.Error, _opID, e);
            }
            finally
            {
                console.TraceEvent(TraceEventType.Stop, _opID, GetEventName(_opID) + " complete");
            }
        }

        private void InvokeInitializeFailed(int _opId, EStoreInitializeError _reason, System.Exception _exception)
        {
            console.TraceEvent(TraceEventType.Error, _opId, GetEventName(_opId) + " error: " + _reason);

            try
            {
                StoreInitializationFailed?.Invoke(this, new IAPPurchaseInitializationFailed(_reason, _exception));
            }
            catch (System.Exception e)
            {
                console.TraceData(TraceEventType.Error, _opId, e);
            }
            finally
            {
                console.TraceEvent(TraceEventType.Stop, _opId, GetEventName(_opId) + " failed");
            }
        }

        private void InvokePurchaseInitiated(string _productId, bool _restored)
        {
            Debug.Assert(!string.IsNullOrEmpty(_productId));

            if (_restored)
            {
                console.TraceEvent(TraceEventType.Start, TRACE_EVENT_PURCHASE, GetEventName(TRACE_EVENT_PURCHASE) + " (auto-restored): " + _productId);
            }
            else
            {
                console.TraceEvent(TraceEventType.Start, TRACE_EVENT_PURCHASE, GetEventName(TRACE_EVENT_PURCHASE) + ": " + _productId);
            }

            purchaseProductId = _productId;

            try
            {
                PurchaseInitiated?.Invoke(this, new IAPPurchaseInitiatedEventArgs(_productId, _restored));
            }
            catch (System.Exception e)
            {
                console.TraceData(TraceEventType.Error, TRACE_EVENT_PURCHASE, e);
            }
        }

        private void InvokePurchaseCompleted(IAPPurchaseResult _purchaseResult)
        {
            Debug.Assert(_purchaseResult != null);

            if (observers != null)
            {
                lock (observers)
                {
                    foreach (var item in observers)
                    {
                        try
                        {
                            item.OnNext(new IAPProductInfo(purchaseProductId, _purchaseResult, null, null));
                        }
                        catch (System.Exception e)
                        {
                            console.TraceData(TraceEventType.Error, TRACE_EVENT_PURCHASE, e);
                        }
                    }
                }
            }

            try
            {
                PurchaseCompleted?.Invoke(this, new IAPPurchaseCompletedEventArgs(_purchaseResult));
            }
            catch (System.Exception e)
            {
                console.TraceData(TraceEventType.Error, TRACE_EVENT_PURCHASE, e);
            }
            finally
            {
                console.TraceEvent(TraceEventType.Stop, TRACE_EVENT_PURCHASE, GetEventName(TRACE_EVENT_PURCHASE) + " completed: " + purchaseProductId);
            }
        }

        private void InvokePurchaseFailed(IAPPurchaseResult _purchaseResult, EStorePurchaseError _failReason, System.Exception _exception)
        {
            var product = _purchaseResult.transactionInfo?.product;
            var productId = purchaseProductId ?? "null";

            console.TraceEvent(TraceEventType.Error, TRACE_EVENT_PURCHASE, $"{GetEventName(TRACE_EVENT_PURCHASE)} error: {productId}, reason = {_failReason}");

            if (observers != null)
            {
                lock (observers)
                {
                    foreach (var item in observers)
                    {
                        try
                        {
                            item.OnNext(new IAPProductInfo(productId, _purchaseResult, _failReason, _exception));
                        }
                        catch (System.Exception e)
                        {
                            console.TraceData(TraceEventType.Error, TRACE_EVENT_PURCHASE, e);
                        }
                    }
                }
            }

            try
            {
                PurchaseFailed?.Invoke(this, new IAPPurchaseFailedEventArgs(productId, _purchaseResult, _failReason, _exception));
            }
            catch (System.Exception e)
            {
                console.TraceData(TraceEventType.Error, TRACE_EVENT_PURCHASE, e);
            }
            finally
            {
                console.TraceEvent(TraceEventType.Stop, TRACE_EVENT_PURCHASE, GetEventName(TRACE_EVENT_PURCHASE) + " failed: " + productId);
            }
        }

        private void ConfirmPendingPurchase(Product _product)
        {
            Debug.Assert(_product != null);
            Debug.Assert(storeController != null);

            console.TraceEvent(TraceEventType.Verbose, TRACE_EVENT_PURCHASE, "ConfirmPendingPurchase: " + _product.definition.id);
            storeController.ConfirmPendingPurchase(_product);
        }

        private Product InitializeTransaction(string _productId)
        {
            Debug.Assert(purchaseProduct == null);
            Debug.Assert(storeController != null);

            if (products.TryGetValue(_productId, out purchaseProduct))
            {
                return storeController.products.WithID(_productId);
            }
            else
            {
                console.TraceEvent(TraceEventType.Warning, TRACE_EVENT_PURCHASE, "No product found for id: " + _productId);
            }

            return null;
        }

        private string GetEventName(int _eventId)
        {
            switch (_eventId)
            {
                case TRACE_EVENT_INITIALIZE:
                    return "Initialize";

                case TRACE_EVENT_FETCH:
                    return "Fetch";

                case TRACE_EVENT_PURCHASE:
                    return "Purchase";
            }

            return "<Unknown>";
        }

        private void ReleaseTransaction()
        {
            purchaseProductId = null;
            purchaseProduct = null;
            purchaseOpCs = null;
        }

#endregion
    }
}
#endif