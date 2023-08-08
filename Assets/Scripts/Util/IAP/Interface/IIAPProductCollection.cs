#if UNITY_PURCHASING
using System.Collections.Generic;

namespace HSMLibrary.IAP
{
    public interface IIAPProductInfoCollection : IReadOnlyCollection<IIAPProductInfo>
    {
        IIAPProductInfo this[string _productId] { get; }
        bool ContainsKey(string _productId);
    }
}
#endif