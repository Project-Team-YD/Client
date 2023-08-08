#if UNITY_PURCHASING
using System;
using UnityEngine.Purchasing.Extension;

namespace HSMLibrary.IAP
{
    public static class IAPHelper
    {
        public static IIAPService CreateStore(IPurchasingModule _module, IIAPDelegate _iapDelegate)
        {
            if (_module == null)
            {
                throw new ArgumentNullException(nameof(_module));
            }

            if (_iapDelegate == null)
            {
                throw new ArgumentNullException(nameof(_iapDelegate));
            }

            return new IAPService(string.Empty, _module, _iapDelegate);
        }
    }
}
#endif