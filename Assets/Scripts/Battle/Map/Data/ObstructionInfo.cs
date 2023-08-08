using System;
using UnityEngine;

[Serializable]
public struct ObstructionInfo
{        
    public float obstructionWidth;
    public float obstructionHeight;    
    public float obstructionPositionX;
    public float obstructionPositionY;
    public string obstructionImageName;

    //public ObstructionInfo(int _id, Vector2 _size, Vector2 _position, Sprite _imageName)
    //{
    //    obstructionId = _id;
    //    obstructionSize = _size;
    //    obstructionPosition = _position;
    //    obstructionImage = _imageName;
    //}
}
