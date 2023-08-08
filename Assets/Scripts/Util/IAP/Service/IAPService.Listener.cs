#if UNITY_PURCHASING
using System.Diagnostics;
using UnityEngine.Purchasing;

namespace HSMLibrary.IAP
{
    public partial class IAPService : IStoreListener
    {
#region data
#endregion

#region IStoreListener

        public void OnInitialized(IStoreController _controller, IExtensionProvider _extensions)
        {
            Debug.Assert(_controller != null);
            Debug.Assert(_extensions != null);

            if (disposed)
            {
                return;
            }

            console.TraceEvent(TraceEventType.Verbose, TRACE_EVENT_INITIALIZE, "OnInitialized");

            try
            {
                foreach (var product in _controller.products.all)
                {
                    if (products.TryGetValue(product.definition.id, out var userProduct))
                    {
                        userProduct.MetaData = product.metadata;
                    }
                }

                storeController = _controller;
                initializeOpCs.SetResult(null);
            }
            catch (System.Exception e)
            {
                console.TraceData(TraceEventType.Error, TRACE_EVENT_INITIALIZE, e);
                initializeOpCs.SetException(e);
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error_)
        {
            if (disposed)
            {
                return;
            }

            console.TraceEvent(TraceEventType.Verbose, TRACE_EVENT_INITIALIZE, "OnInitializeFailed: " + error_);

            try
            {
                initializeOpCs.SetException(new IAPInitializeException(error_));
            }
            catch (System.Exception e)
            {
                console.TraceData(TraceEventType.Error, TRACE_EVENT_INITIALIZE, e);
            }
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs _args)
        {
            Debug.Assert(_args != null);
            Debug.Assert(_args.purchasedProduct != null);

            if (disposed)
            {
                return PurchaseProcessingResult.Pending;
            }
            else
            {
                var product = _args.purchasedProduct;
                var productId = product.definition.id;
                var isRestored = purchaseOpCs == null;

                try
                {
                    if (isRestored)
                    {
                        InvokePurchaseInitiated(productId, true);
                        InitializeTransaction(productId);
                    }

                    console.TraceEvent(TraceEventType.Verbose, TRACE_EVENT_PURCHASE, "ProcessPurchase: " + productId);
                    console.TraceEvent(TraceEventType.Verbose, TRACE_EVENT_PURCHASE, $"Receipt ({productId}): {product.receipt ?? "null"}");

                    if (isRestored || purchaseOpCs.Task.AsyncState.Equals(product))
                    {
                        var transactionInfo = new IAPTransaction(product, isRestored);

                        if (string.IsNullOrEmpty(transactionInfo.receipt))
                        {
                            SetPurchaseFailed(purchaseProduct, transactionInfo, null, EStorePurchaseError.ReceiptNullOrEmpty);
                        }
                        else
                        {
                            ValidatePurchase(purchaseProduct, transactionInfo);
                            return PurchaseProcessingResult.Pending;
                        }
                    }
                }
                catch (System.Exception e)
                {
                    SetPurchaseFailed(purchaseProduct, new IAPTransaction(product, isRestored), null, EStorePurchaseError.Unknown, e);
                }

                return PurchaseProcessingResult.Complete;
            }
        }

        public void OnPurchaseFailed(Product _product, PurchaseFailureReason _failReason)
        {
            if (disposed)
            {
                return;
            }

            var productId = _product?.definition.id ?? "null";
            var isRestored = purchaseOpCs == null;

            try
            {
                if (isRestored)
                {
                    InvokePurchaseInitiated(productId, true);
                    InitializeTransaction(productId);
                }

                console.TraceEvent(TraceEventType.Verbose, TRACE_EVENT_PURCHASE, $"OnPurchaseFailed: {productId}, reason={_failReason}");

                SetPurchaseFailed(purchaseProduct, new IAPTransaction(_product, isRestored), null, GetPurchaseError(_failReason), null);
            }
            catch (System.Exception e)
            {
                SetPurchaseFailed(purchaseProduct, new IAPTransaction(_product, isRestored), null, GetPurchaseError(_failReason), e);
            }
        }

#endregion

#region implementation

        private void OnFetch()
        {
            if (disposed)
            {
                return;
            }

            console.TraceEvent(TraceEventType.Verbose, TRACE_EVENT_FETCH, "OnFetch");

            try
            {
                foreach (var product in storeController.products.all)
                {
                    if (products.TryGetValue(product.definition.id, out var userProduct))
                    {
                        userProduct.MetaData = product.metadata;
                    }
                }

                fetchOpCs.SetResult(null);
            }
            catch (System.Exception e)
            {
                console.TraceData(TraceEventType.Error, TRACE_EVENT_FETCH, e);
                fetchOpCs.SetException(e);
            }
        }

        public void OnFetchFailed(InitializationFailureReason _reason)
        {
            if (disposed)
            {
                return;
            }

            console.TraceEvent(TraceEventType.Verbose, TRACE_EVENT_FETCH, "OnFetchFailed: " + _reason);

            try
            {
                fetchOpCs.SetException(new IAPInitializeException(_reason));
            }
            catch (System.Exception e)
            {
                console.TraceData(TraceEventType.Error, TRACE_EVENT_FETCH, e);
            }
        }

        private async void ValidatePurchase(IIAPProductInfo _userProduct, IAPTransaction _transactionInfo)
        {
            var product = _transactionInfo.product;
            var resultStatus = EPurchaseValidStatus.FAIL;

            try
            {
                console.TraceEvent(TraceEventType.Verbose, TRACE_EVENT_PURCHASE, $"ValidatePurchase: {product.definition.id}, transactionId = {product.transactionID}");

                var validationResult = await @delegate.ValidatePurchaseAsync(_userProduct, _transactionInfo);

                if (!disposed)
                {
                    if (validationResult == null)
                    {
                        ConfirmPendingPurchase(product);
                        SetPurchaseCompleted(purchaseProduct, _transactionInfo, validationResult);
                    }
                    else
                    {
                        resultStatus = validationResult.status;

                        if (resultStatus == EPurchaseValidStatus.SUCCESS)
                        {
                            ConfirmPendingPurchase(product);
                            SetPurchaseCompleted(purchaseProduct, _transactionInfo, validationResult);
                        }
                        else if (resultStatus == EPurchaseValidStatus.FAIL)
                        {
                            ConfirmPendingPurchase(product);
                            SetPurchaseFailed(purchaseProduct, _transactionInfo, validationResult, EStorePurchaseError.ReceiptValidationFailed);
                        }
                        else
                        {
                            SetPurchaseFailed(purchaseProduct, _transactionInfo, validationResult, EStorePurchaseError.ReceiptValidationNotAvailable);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                ConfirmPendingPurchase(product);
                SetPurchaseFailed(purchaseProduct, _transactionInfo, null, EStorePurchaseError.ReceiptValidationFailed, e);
            }
        }

        private void SetPurchaseCompleted(IIAPProductInfo product_, IAPTransaction transactionInfo_, IAPPurchaseValidResult validationResult_)
        {
            var result = new IAPPurchaseResult(product_, transactionInfo_, validationResult_);

            if (purchaseOpCs != null)
            {
                purchaseOpCs.SetResult(result);
            }
            else
            {
                InvokePurchaseCompleted(result);
                ReleaseTransaction();
            }
        }

        private void SetPurchaseFailed(IIAPProductInfo product_, IAPTransaction transactionInfo_, IAPPurchaseValidResult validationResult_, EStorePurchaseError failReason_, System.Exception e_ = null)
        {
            var result = new IAPPurchaseResult(product_, transactionInfo_, validationResult_);

            if (purchaseOpCs != null)
            {
                if (failReason_ == EStorePurchaseError.UserCanceled)
                {
                    purchaseOpCs.SetCanceled();
                }
                else if (e_ != null)
                {
                    purchaseOpCs.SetException(new IAPPurchaseException(result, failReason_, e_));
                }
                else
                {
                    purchaseOpCs.SetException(new IAPPurchaseException(result, failReason_));
                }
            }
            else
            {
                InvokePurchaseFailed(result, failReason_, e_);
                ReleaseTransaction();
            }
        }

#endregion
    }
}
#endif