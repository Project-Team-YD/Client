#if UNITY_PURCHASING
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HSMLibrary.IAP
{
    public enum EPurchaseValidStatus
    {
        SUCCESS,
        FAIL
    }

    public class IAPPurchaseValidResult
    {
        public EPurchaseValidStatus status { get; }

        public IAPPurchaseValidResult(EPurchaseValidStatus _status)
        {
            status = _status;
        }
    }
}
#endif