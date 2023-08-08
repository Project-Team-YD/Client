#if UNITY_PURCHASING

namespace HSMLibrary.IAP
{
    using System.Threading.Tasks;
    using Generics;

    public class IAPManager : Singleton<IAPManager>
    {
        public IAPManager()
        {
            IAPHelper.CreateStore(StandardPurchasingModule.Instance(AppStore.AppleAppStore), new AppleStoreDelegate()).FetchAsync();
        }
    }
}
#endif