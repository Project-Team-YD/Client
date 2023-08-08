using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSMLibrary.UI
{
    public interface IPopup
    {
        T Show<T>() where T : IPopup;
        void Hide();
    }
}
