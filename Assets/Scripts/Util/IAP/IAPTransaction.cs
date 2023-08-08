#if UNITY_PURCHASING
//using UnityEngine.Purchasing;

namespace HSMLibrary.IAP
{
    using Extension;

    public class IAPTransaction
    {
		public Product product { get; }

        public string transactionId { get; internal set; }

        public string storeId { get; internal set; }

        public string receipt { get; internal set; }

        public bool isRestored { get; internal set; }

        public IAPTransaction(Product _product, bool _isRestored)
        {
            product = _product;
            transactionId = _product.transactionID;
            receipt = _product.GetNativeReceipt(out var storeID);
            storeId = storeID;
        }

        public IAPTransaction(Product _product, string _transactionId, string _storeId, string _receipt, bool _isRestored)
        {
            product = _product;
            storeId = _storeId;
            transactionId = _transactionId;
            receipt = _receipt;
            isRestored = _isRestored;
        }
    }
}
#endif