#if UNITY_PURCHASING
using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace HSMLibrary.IAP
{
    public class IAPConfig
    {
		public IEnumerable<IIAPProductInfo> products { get; }

        public IAPConfig(IEnumerable<IIAPProductInfo> _products)
        {
            products = _products;
        }

        public IAPConfig(IEnumerable<ProductDefinition> _products)
        {
            var items = new List<IIAPProductInfo>();

            foreach (var productDefinitions in _products)
            {
                items.Add(new DefaultProduct(productDefinitions));
            }

            products = items;
        }
    }
}
#endif