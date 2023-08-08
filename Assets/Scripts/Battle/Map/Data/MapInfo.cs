using UnityEngine;

public struct MapInfo
{
    public int MapId;            
    public float MapWidth;
    public float MapHeight;
    public Color32 MapBackgroundColor;
    public Color32 BoundaryColor;
    public string MapImage;
    public string BoundaryImage;
    public float CharacterStartPositionX;
    public float CharacterStartPositionY;
    public ObstructionInfo[] ObstructionInfos;
}
