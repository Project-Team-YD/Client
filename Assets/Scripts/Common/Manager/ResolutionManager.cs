using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    [SerializeField] private Vector2Int baseResolution = Vector2Int.zero;
    private Vector2 ratio = Vector2.zero;

    private void Start()
    {
        //screenSize.x = Screen.width;
        //screenSize.y = Screen.height;

        ratio.x = Screen.width / baseResolution.x;
        ratio.y = Screen.height / baseResolution.y;
    }

    public Vector2 GetRatio()
    {
        return ratio;
    }
}
