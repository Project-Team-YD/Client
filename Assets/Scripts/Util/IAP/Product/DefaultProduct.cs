#if UNITY_PURCHASING
using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace HSMLibrary.IAP
{
    internal class DefaultProduct : IIAPProductInfo
    {
#region interface

        public DefaultProduct(ProductDefinition productDefinition)
        {
            Definition = productDefinition;
        }

#endregion

#region IIAPProductInfo

        public string Id => Definition.id;
        public ProductDefinition Definition { get; }
        public ProductMetadata MetaData { get; set; }

#endregion
    }
}
#endif