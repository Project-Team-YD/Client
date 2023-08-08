using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CustomMenuItem
{
    //해당 스크립트가 추가된 오브젝트의 transform
    static private Transform ScrollViewRootTf;

    //생성된 오브젝트의 부모를 판별하기 위한 int 변수
    static private int firstChild = 1;
    static private int secondChild = 2;
    static private int ThirdChild = 3;

    [MenuItem("GameObject/UI/CustomScrollView", false)]
    static void CreateCustomScrollView(MenuCommand menuCommand)
    {
        // 1. Custom GameObject 이름으로 새 Object를 만든다.
        GameObject go = new GameObject("CustomScrollView");
        ScrollViewRootTf = go.transform;
        // 2. Hierachy 윈도우에서 어떤 오브젝트를 선택하여 생성시에는 그 오브젝트의 하위 계층으로 생성된다.
        // 그밖의 경우에는 아무일도 일어나지 않는다.
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

        //오브젝트 생성 및 생성된 오브젝트 RectTransform컴포넌트로 변경.
        CreateObject("Viewport", firstChild, 1);
        CreateObject("ScrollBarHorizontal", firstChild, 1);
        CreateObject("ScrollBarVertical", firstChild, 1);
        CreateObject("Content", secondChild, 1);
        CreateObject("SlidingAreaHorizontal", secondChild, 2);
        CreateObject("HandleHorizontal", ThirdChild, 2);
        CreateObject("SlidingAreaVertical", secondChild, 3);
        CreateObject("HandleVertical", ThirdChild, 3);

        FindChildByName("ScrollBarHorizontal", ScrollViewRootTf).AddComponent<HorizontalScrollBar>();
        FindChildByName("ScrollBarVertical", ScrollViewRootTf).AddComponent<VerticalScrollBar>();
        // 3. 생성된 오브젝트를 Undo 시스템에 등록한다.
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);        
        go.AddComponent<CustomScrollRect>();        
        var rect = go.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200f, 200f);        
    }

    static private void CreateObject(string name, int ParentIndex, int ParentIndex2)
    {
        GameObject _newObejct = new GameObject(name);
        _newObejct.AddComponent<RectTransform>();
        var tf = _newObejct.transform;
        // 1 : firstChild
        // 2 : secondChild
        // 3 : ThirdChild
        switch (ParentIndex)
        {
            case 1:
                tf.SetParent(ScrollViewRootTf);
                break;
            case 2:
                if (ParentIndex2 == 1)
                    tf.SetParent(ScrollViewRootTf.GetChild(0).transform);
                else if (ParentIndex2 == 2)
                    tf.SetParent(ScrollViewRootTf.GetChild(1).transform);
                else
                    tf.SetParent(ScrollViewRootTf.GetChild(2).transform);
                break;
            case 3:
                if (ParentIndex2 == 1)
                    tf.SetParent(ScrollViewRootTf.GetChild(0).transform.GetChild(0).transform);
                else if (ParentIndex2 == 2)
                    tf.SetParent(ScrollViewRootTf.GetChild(1).transform.GetChild(0).transform);
                else
                    tf.SetParent(ScrollViewRootTf.GetChild(2).transform.GetChild(0).transform);
                break;            
        }
        _newObejct.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        _newObejct.GetComponent<RectTransform>().localScale = Vector3.one;
    }

    static private GameObject FindChildByName(string ThisName, Transform ThisGObj)
    {
        GameObject ReturnObj;
        if (ThisGObj.name == ThisName)
            return ThisGObj.gameObject;
        foreach (Transform child in ThisGObj)
        {
            ReturnObj = FindChildByName(ThisName, child);
            if (ReturnObj != null)
                return ReturnObj;
        }
        return null;
    }
}
