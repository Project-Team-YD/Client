#if UNITY_PURCHASING
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;

namespace HSMLibrary.IAP
{
    public partial class IAPService : IIAPService, IIAPProductInfoCollection, IObservable<IAPProductInfo>
    {
#region data

        private const int TRACE_EVENT_INITIALIZE = 1;
        private const int TRACE_EVENT_FETCH = 2;
        private const int TRACE_EVENT_PURCHASE = 3;

        private readonly string serviceName;
        private readonly TraceSource console;
        private readonly IIAPDelegate @delegate;
        private readonly IPurchasingModule purchasingModule;

        private Dictionary<string, IIAPProductInfo> products = new Dictionary<string, IIAPProductInfo>();
        private List<IObserver<IAPProductInfo>> observers;
        private TaskCompletionSource<object> initializeOpCs;
        private TaskCompletionSource<object> fetchOpCs;
        private TaskCompletionSource<IAPPurchaseResult> purchaseOpCs;
        private string purchaseProductId;
        private IIAPProductInfo purchaseProduct;
        private IStoreController storeController;
        private bool disposed;

#endregion

#region interface

        internal IAPService(string _name, IPurchasingModule _purchasingModule, IIAPDelegate _storeDelegate)
        {
            serviceName = string.IsNullOrEmpty(_name) ? "Purchasing" : "Purchasing." + _name;
            console = new TraceSource(serviceName, SourceLevels.All);
            @delegate = _storeDelegate;
            purchasingModule = _purchasingModule;
        }

#endregion

#region IIAPService

        public event EventHandler StoreInitialized;
        public event EventHandler<IAPPurchaseInitializationFailed> StoreInitializationFailed;
        public event EventHandler<IAPPurchaseInitiatedEventArgs> PurchaseInitiated;
        public event EventHandler<IAPPurchaseCompletedEventArgs> PurchaseCompleted;
        public event EventHandler<IAPPurchaseFailedEventArgs> PurchaseFailed;

        public IObservable<IAPProductInfo> Purchases => this;

        public SourceSwitch TraceSwitch => console.Switch;

        public TraceListenerCollection TraceListeners => console.Listeners;

        public IIAPProductInfoCollection Products => this;

        public IStoreController Controller => storeController;

        public bool IsInitialized => storeController != null;

        public bool IsBusy => purchaseOpCs != null;

        public async Task InitializeAsync()
        {
            ThrowIfDisposed();

            if (storeController == null)
            {
                if (initializeOpCs != null)
                {
                    await initializeOpCs.Task;
                }
                else if (Application.isMobilePlatform || Application.isEditor)
                {
                    console.TraceEvent(TraceEventType.Start, TRACE_EVENT_INITIALIZE, "Initialize");

                    try
                    {
                        initializeOpCs = new TaskCompletionSource<object>();

                        var configurationBuilder = ConfigurationBuilder.Instance(purchasingModule);
                        var storeConfig = await @delegate.GetStoreConfigAsync();

                        foreach (var product in storeConfig.products)
                        {
                            var productDefinition = product.Definition;
                            configurationBuilder.AddProduct(productDefinition.id, productDefinition.type);
                            products.Add(productDefinition.id, product);
                        }

                        UnityPurchasing.Initialize(this, configurationBuilder);
                        await initializeOpCs.Task;

                        InvokeInitializeCompleted(TRACE_EVENT_INITIALIZE);
                    }
                    catch (IAPInitializeException e)
                    {
                        InvokeInitializeFailed(TRACE_EVENT_INITIALIZE, GetInitializeError(e.reason), e);
                        throw;
                    }
                    catch (System.Exception e)
                    {
                        console.TraceData(TraceEventType.Error, TRACE_EVENT_INITIALIZE, e);
                        InvokeInitializeFailed(TRACE_EVENT_INITIALIZE, EStoreInitializeError.Unknown, e);
                        throw;
                    }
                    finally
                    {
                        initializeOpCs = null;
                    }
                }
            }
        }

        public async Task FetchAsync()
        {
            ThrowIfDisposed();

            if (storeController == null)
            {
                await InitializeAsync();
            }
            else if (fetchOpCs != null)
            {
                await fetchOpCs.Task;
            }
            else if (Application.isMobilePlatform || Application.isEditor)
            {
                console.TraceEvent(TraceEventType.Start, TRACE_EVENT_FETCH, "Fetch");

                try
                {
                    fetchOpCs = new TaskCompletionSource<object>();

                    var storeConfig = await @delegate.GetStoreConfigAsync();
                    var productsToFetch = new HashSet<ProductDefinition>();

                    foreach (var product in storeConfig.products)
                    {
                        var productDefinition = product.Definition;
                        if (products.ContainsKey(productDefinition.id))
                        {
                            products[productDefinition.id] = product;
                        }
                        else
                        {
                            products.Add(productDefinition.id, product);
                        }

                        productsToFetch.Add(productDefinition);
                    }

                    storeController.FetchAdditionalProducts(productsToFetch, OnFetch, OnFetchFailed);
                    await fetchOpCs.Task;

                    InvokeInitializeCompleted(TRACE_EVENT_FETCH);
                }
                catch (IAPInitializeException e)
                {
                    InvokeInitializeFailed(TRACE_EVENT_FETCH, GetInitializeError(e.reason), e);
                    throw;
                }
                catch (System.Exception e)
                {
                    console.TraceData(TraceEventType.Error, TRACE_EVENT_FETCH, e);
                    InvokeInitializeFailed(TRACE_EVENT_FETCH, EStoreInitializeError.Unknown, e);
                    throw;
                }
                finally
                {
                    fetchOpCs = null;
                }
            }
        }

        public async Task<IAPPurchaseResult> PurchaseAsync(string productId_)
        {
            ThrowIfInvalidProductId(productId_);
            ThrowIfDisposed();
            ThrowIfBusy();

            InvokePurchaseInitiated(productId_, false);

            try
            {
                await InitializeAsync();

                if (fetchOpCs != null)
                {
                    await fetchOpCs.Task;
                }

                var product = InitializeTransaction(productId_);
                if (product != null && product.availableToPurchase)
                {
                    console.TraceEvent(TraceEventType.Verbose, TRACE_EVENT_PURCHASE, $"InitiatePurchase: {product.definition.id} ({product.definition.storeSpecificId}), type={product.definition.type}, price={product.metadata.localizedPriceString}");
                    purchaseOpCs = new TaskCompletionSource<IAPPurchaseResult>(product);
                    storeController.InitiatePurchase(product);

                    var purchaseResult = await purchaseOpCs.Task;
                    InvokePurchaseCompleted(purchaseResult);
                    return purchaseResult;
                }
                else
                {
                    throw new IAPPurchaseException(new IAPPurchaseResult(purchaseProduct), EStorePurchaseError.ProductUnavailable);
                }
            }
            catch (IAPPurchaseException e)
            {
                InvokePurchaseFailed(e.result, e.reason, e);
                throw;
            }
            catch (IAPInitializeException e)
            {
                console.TraceEvent(TraceEventType.Error, TRACE_EVENT_PURCHASE, $"{GetEventName(TRACE_EVENT_PURCHASE)} error: {productId_}, reason = {e.Message}");
                throw;
            }
            catch (System.Exception e)
            {
                console.TraceData(TraceEventType.Error, TRACE_EVENT_PURCHASE, e);
                InvokePurchaseFailed(new IAPPurchaseResult(purchaseProduct), EStorePurchaseError.Unknown, e);
                throw;
            }
            finally
            {
                ReleaseTransaction();
            }
        }

#endregion

#region IReadOnlyCollection

        public IIAPProductInfo this[string _productId]
        {
            get
            {
                ThrowIfInvalidProductId(_productId);
                return products[_productId];
            }
        }

        public int Count => products.Count;

        public bool ContainsKey(string _productId)
        {
            ThrowIfInvalidProductId(_productId);
            return products.ContainsKey(_productId);
        }

        public bool TryGetValue(string _productId, out IIAPProductInfo _product)
        {
            ThrowIfInvalidProductId(_productId);
            return products.TryGetValue(_productId, out _product);
        }

#endregion

#region IEnumerable

        public IEnumerator<IIAPProductInfo> GetEnumerator()
        {
            return products.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return products.Values.GetEnumerator();
        }

#endregion

#region IObservable

        private class Subscription : IDisposable
        {
            private readonly List<IObserver<IAPProductInfo>> observers;
            private readonly IObserver<IAPProductInfo> observer;

            public Subscription(List<IObserver<IAPProductInfo>> _observers, IObserver<IAPProductInfo> _observer)
            {
                observers = _observers;
                observer = _observer;
            }

            public void Dispose()
            {
                lock (observers)
                {
                    observers.Remove(observer);
                }
            }
        }

        public IDisposable Subscribe(IObserver<IAPProductInfo> _observer)
        {
            if (_observer == null)
            {
                throw new ArgumentNullException(nameof(_observer));
            }

            ThrowIfDisposed();

            if (observers == null)
            {
                observers = new List<IObserver<IAPProductInfo>>() { _observer };
            }
            else
            {
                lock (observers)
                {
                    observers.Add(_observer);
                }
            }

            return new Subscription(observers, _observer);
        }

#endregion

#region IDisposable

        public void Dispose()
        {
            if (!disposed)
            {
                if (initializeOpCs != null)
                {
                    InvokeInitializeFailed(TRACE_EVENT_INITIALIZE, EStoreInitializeError.StoreDisposed, null);
                    initializeOpCs = null;
                }

                if (fetchOpCs != null)
                {
                    InvokeInitializeFailed(TRACE_EVENT_FETCH, EStoreInitializeError.StoreDisposed, null);
                    fetchOpCs = null;
                }

                if (purchaseOpCs != null)
                {
                    InvokePurchaseFailed(new IAPPurchaseResult(purchaseProduct), EStorePurchaseError.StoreDisposed, null);
                    ReleaseTransaction();
                }

                try
                {
                    if (observers != null)
                    {
                        lock (observers)
                        {
                            foreach (var item in observers)
                            {
                                item.OnCompleted();
                            }

                            observers.Clear();
                        }
                    }
                }
                catch (System.Exception e)
                {
                    console.TraceData(TraceEventType.Error, 0, e);
                }

                console.TraceEvent(TraceEventType.Verbose, 0, "Disposed");
                console.Close();
                products.Clear();
                storeController = null;
                disposed = true;
            }
        }

#endregion

#region implementation

        private void ThrowIfDisposed()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(serviceName);
            }
        }

        private void ThrowIfInvalidProductId(string _productId)
        {
            if (string.IsNullOrEmpty(_productId))
            {
                throw new ArgumentException(serviceName + " product identifier cannot be null or empty string", nameof(_productId));
            }
        }

        private void ThrowIfNotInitialized()
        {
            if (storeController == null)
            {
                throw new InvalidOperationException(serviceName + " is not initialized");
            }
        }

        private void ThrowIfBusy()
        {
            if (purchaseOpCs != null || purchaseProduct != null)
            {
                throw new InvalidOperationException(serviceName + " is busy");
            }
        }

#endregion
    }
}
#endif
