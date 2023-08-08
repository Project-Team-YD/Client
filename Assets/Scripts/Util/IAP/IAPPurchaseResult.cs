#if UNITY_PURCHASING

namespace HSMLibrary.IAP
{
    public class IAPPurchaseResult
    {
		public IIAPProductInfo product { get; }
        public IAPTransaction transactionInfo { get; }

        public IAPPurchaseValidResult validResult { get; }

        public IAPPurchaseResult(IIAPProductInfo _product)
        {
            product = _product;
        }

        public IAPPurchaseResult(IIAPProductInfo _product, IAPTransaction _transactionInfo, IAPPurchaseValidResult _validResult)
        {
            product = _product;
            transactionInfo = _transactionInfo;
            validResult = _validResult;
        }
    }
}
#endif