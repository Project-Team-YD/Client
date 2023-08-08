#if UNITY_PURCHASING
using UnityEngine.Purchasing;

namespace HSMLibrary.IAP
{
    public interface IIAPProductInfo
    {
        string Id { get; }

        ProductDefinition Definition { get; }
        ProductMetadata MetaData { get; set; }
    }
}
#endif