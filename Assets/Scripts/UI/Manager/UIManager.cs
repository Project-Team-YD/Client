using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HSMLibrary.Manager
{
    using HSMLibrary.Generics;
    using HSMLibrary.UI;
    using System;
    using HSMLibrary.Extensions;
    using Cysharp.Threading.Tasks;

    public sealed class UIManager : Singleton<UIManager>
    {
        private Dictionary<Type, UIBaseController> cachedPanelDict = null;

        private LinkedList<UIBaseController> panelList = null;

        public event Action OnHideCallback;

        private int curOrder = 0;

        public UIManager()
        {
            cachedPanelDict = new Dictionary<Type, UIBaseController>();
            cachedPanelDict.Clear();

            panelList = new LinkedList<UIBaseController>();
            panelList.Clear();

            curOrder = 0;
        }

        ~UIManager()
        {
            ClearPanels();

            cachedPanelDict = null;
            panelList = null;
        }

        public async UniTask<T> Show<T>(string _panelName = "", int _order = 0) where T : UIBaseController
        {
            curOrder += (1 + _order);

            var panel = await GetCachedPanel<T>(_panelName);        
            panel.SetSortingOrder(curOrder);
            panel.Show();

            panelList.AddLast(panel);

            return panel;
        }

        public void Hide()
        {
            if(panelList.Count > 0)
            {
                var panel = panelList.Last.Value;
                panel.Hide();
                panelList.RemoveLast();
            }
            else
            {
                OnHideCallback?.Invoke();
            }
        }

        public void Hide<T>()
        {
            if (panelList.Count > 0)
            {
                Type targetType = typeof(T);

                var node = panelList.First;
                while (node.Next != null)
                {
                    var panel = node.Value;
                    if (targetType == panel.GetType())
                    {
                        panel.Hide();
                        panelList.Remove(panel);
                        return;
                    }

                    node = node.Next;
                }

                throw new NullReferenceException();
            }
            else
            {
                OnHideCallback?.Invoke();
            }
        }

        public async UniTask<T> GetCachedPanel<T>(string _panelName = "") where T : UIBaseController
        {
            cachedPanelDict.TryGetValue(typeof(T), out var panel);
            if(panel == null)
            {
                panel = await AddCachePanel<T>(_panelName);
            }

            return (T)panel;
        }

        private async UniTask<T> AddCachePanel<T>(string _panelName = "") where T : UIBaseController
        {
            var panel = FindPanelInHierarchy<T>(_panelName);
            if(panel == null)
            {
                panel = await CreatePanel<T>(_panelName);
            }

            cachedPanelDict.Add(typeof(T), panel);

            return panel;
        }

        private async UniTask<T> CreatePanel<T>(string _prefabPath = "") where T : UIBaseController
        {
            var prefab = await Resources.LoadAsync(_prefabPath, typeof(GameObject)) as GameObject;
            var panelObject = GameObject.Instantiate(prefab);

            return panelObject.GetComponent<T>();
        }

        private T FindPanelInHierarchy<T>(string _panelName) where T : UIBaseController
        {
            GameObject panelObj = UnityExtension.Find(_panelName);
            if(panelObj == null)
            {
                throw new NullReferenceException();
            }

            return panelObj.GetComponent<T>();
        }

        public void ClearPanels()
        {
            curOrder = 0;

            cachedPanelDict.Clear();
            panelList.Clear();
        }
    }
}