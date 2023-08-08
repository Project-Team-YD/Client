#if UNITY_PURCHASING
using Cysharp.Threading.Tasks;

namespace HSMLibrary.IAP
{
    public interface IIAPDelegate
    {
        UniTask<IAPConfig> GetStoreConfigAsync();
        UniTask<IAPPurchaseValidResult> ValidatePurchaseAsync(IIAPProductInfo _product, IAPTransaction _transactionInfo);
    }
}
#endif