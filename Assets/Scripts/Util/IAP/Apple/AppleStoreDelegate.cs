#if UNITY_PURCHASING

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Purchasing;
using Cysharp.Threading.Tasks;

namespace HSMLibrary.IAP
{
    public class AppleStoreDelegate : IIAPDelegate
    {
        public UniTask<IAPConfig> GetStoreConfigAsync()
        {
            UniTask<IAPConfig> configTask = new UniTask<IAPConfig>();
            configTask.ContinueWith(() => {

                List<ProductDefinition> products = new List<ProductDefinition>();
                for(int i = 0; i < 5; i++)
                {
                    ProductDefinition product = new ProductDefinition(i.ToString(), ProductType.Consumable);
                    products.Add(product);
                }

                return new IAPConfig(products); 
            });

            return configTask;
        }

        public UniTask<IAPPurchaseValidResult> ValidatePurchaseAsync(IIAPProductInfo _product, IAPTransaction _transactionInfo)
        {
            throw new NotImplementedException();
        }
    }
}
#endif