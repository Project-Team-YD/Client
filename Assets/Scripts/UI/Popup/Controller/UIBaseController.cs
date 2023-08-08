using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HSMLibrary.UI
{
    public class UIBaseController : MonoBehaviour
    {
        protected Canvas canvas = null;
        protected GraphicRaycaster raycaster = null;

        public virtual bool Initialize()
        {
            canvas = GetComponent<Canvas>();
            raycaster = GetComponent<GraphicRaycaster>();

            return canvas != null && raycaster != null;
        }

        public virtual void Show()
        {
            canvas.enabled = true;
            raycaster.enabled = true;            
        }

        public virtual void Hide()
        {
            canvas.enabled = false;
            raycaster.enabled = false;
        }

        public void SetSortingOrder(int _order)
        {
            canvas.sortingOrder = _order;
        }
    }
}
