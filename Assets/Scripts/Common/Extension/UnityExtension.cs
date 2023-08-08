using System.Collections.Generic;
using UnityEngine;

namespace HSMLibrary.Extensions
{
    public static class UnityExtension
    {
        public static void EnsureLoaded(this GameObject _gameObject)
        {
            if (!_gameObject.activeInHierarchy)
            {
                _gameObject.SetActive(true);
                _gameObject.SetActive(false);
            }
        }

        public static void RemoveChilds(this GameObject _gameObject)
        {
            var children = new List<GameObject>();
            foreach (Transform child in _gameObject.transform)
            {
                children.Add(child.gameObject);
            }

            children.ForEach(child => child.transform.SetParent(null));
            children.ForEach(child => GameObject.Destroy(child));
        }

        public static GameObject Find(string _objName)
        {
            GameObject result = null;
            foreach (GameObject root in Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (root.transform.parent == null)
                {
                    result = Find(root, _objName, 0);
                    if (result != null)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        public static T GetComponent<T>(this GameObject _obj)
        {
            return Find(_obj.name).GetComponent<T>();
        }

        public static GameObject Find(string _objName, string _tag)
        {
            GameObject result = null;
            foreach (GameObject parent in GameObject.FindGameObjectsWithTag(_tag))
            {
                result = Find(parent, _objName, 0);
                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

        public static GameObject FindChild(this GameObject _item, string _objName)
        {
            return Find(_item, _objName, 0);
        }

        public static void AttachChild(this GameObject _parent, GameObject _child)
        {
            Vector3 backupPos = _child.transform.localPosition;
            Quaternion backupRot = _child.transform.localRotation;
            Vector3 backupScale = _child.transform.localScale;

            _child.transform.SetParent(_parent.transform);

            _child.transform.localPosition = backupPos;
            _child.transform.localRotation = backupRot;
            _child.transform.localScale = backupScale;
        }

        public static void AttachChildScaleOne(this GameObject _parent, GameObject _child)
        {
            Vector3 backupPos = _child.transform.localPosition;
            Quaternion backupRot = _child.transform.localRotation;

            _child.transform.SetParent(_parent.transform);

            _child.transform.localPosition = backupPos;
            _child.transform.localRotation = backupRot;
            _child.transform.localScale = Vector3.one;
        }

        public static List<T> GetComponentsOfType<T>(this GameObject _gameObject) where T : UnityEngine.Component
        {
            List<T> list = new List<T>();
            var tArr = _gameObject.GetComponents<T>();
            for (int i = 0; i < tArr.Length; i++)
            {
                list.Add(tArr[i]);
            }

            return list;
        }

        public static void FindComponentsOfType<T>(this GameObject _gameObject, List<T> _childList) where T : UnityEngine.Component
        {
            FindComponentsOfType(_gameObject.transform, _childList);
        }

        public static void FindComponentsOfType<T>(this Transform _transform, List<T> _childList) where T : UnityEngine.Component
        {
            foreach (Transform child in _transform)
            {
                T component = child.GetComponent<T>();
                if (component != null)
                {
                    _childList.Add(component);
                }

                FindComponentsOfType<T>(child, _childList);
            }
        }

        private static GameObject Find(GameObject _item, string _objName, int _index)
        {
            if (_index == 0 && _item.name == _objName)
                return _item;

            if (_index < _item.transform.childCount)
            {
                GameObject result = Find(_item.transform.GetChild(_index).gameObject, _objName, 0);
                if (result == null)
                {
                    return Find(_item, _objName, ++_index);
                }
                else
                {
                    return result;
                }
            }

            return null;
        }

        public static void SetChildLayer<T>(this GameObject _gameObject, int _layer) where T : UnityEngine.Component
        {
            Transform transform = _gameObject.transform;
            foreach (Transform child in transform)
            {
                if (child.GetComponent<T>() != null)
                {
                    continue;
                }

                child.gameObject.layer = _layer;
                SetChildLayer(child.gameObject, _layer);
            }
        }

        public static void SetChildLayer(this GameObject _gameObject, int _layer)
        {
            Transform transform = _gameObject.transform;
            foreach (Transform child in transform)
            {
                child.gameObject.layer = _layer;
                SetChildLayer(child.gameObject, _layer);
            }
        }

        public static GameObject GetParent(this GameObject gameObject)
        {
            Transform parent = gameObject.transform.parent;
            return parent == null ? null : parent.gameObject;
        }

        public static void GetChildren<T>(this Transform _transform, List<T> _list) where T : Component
        {
            int childCnt = _transform.childCount;
            for (int i = 0; i < childCnt; i++)
            {
                Transform childTrans = _transform.GetChild(i);
                T t = childTrans.GetComponent<T>();
                if (t != null)
                {
                    _list.Add(t);
                }

                childTrans.GetChildren(_list);
            }
        }
    }
}
