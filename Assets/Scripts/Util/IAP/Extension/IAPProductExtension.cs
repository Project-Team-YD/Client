#if UNITY_PURCHASING
using Newtonsoft.Json;
using System;
using UnityEngine;
//using UnityEngine.Purchasing;

namespace HSMLibrary.IAP
{
    public static class IAPProductInfoExtension
    {
        [Serializable]
        private struct ReceiptData
        {
            public string store;
            public string payload;

            public ReceiptData(string _store, string _payload)
            {
                store = _store;
                payload = _payload;
            }
        }

        public static string GetNativeReceipt(this Product _product)
        {
            if (string.IsNullOrEmpty(_product.receipt))
            {
                return _product.receipt;
            }

            return JsonUtility.FromJson<ReceiptData>(_product.receipt).payload;
        }

        public static string GetNativeReceipt(this Product _product, out string _storeId)
        {
            if (string.IsNullOrEmpty(_product.receipt))
            {
                _storeId = null;
                return _product.receipt;
            }

            var receiptData = JsonConvert.DeserializeObject<ReceiptData>(_product.receipt);
            _storeId = receiptData.store;
            return receiptData.payload;
        }
    }
}
#endif