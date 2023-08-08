using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapTool : MonoBehaviour
{
    #region Map    
    [SerializeField] private GameObject prefab = null;
    [SerializeField] private SpriteRenderer mapBackground = null;
    [SerializeField] private Color32 mapBackgroundColor = new Color32(255, 255, 255, 255);
    [SerializeField] private SpriteRenderer mapBoundary = null;
    [SerializeField] private Color32 mapBoundaryColor = new Color32(255, 255, 255, 255);
    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private int mapId;
    [SerializeField] private Vector2 mapSize;    
    [SerializeField] private Sprite mapImage = null;
    [SerializeField] private Sprite boundaryImage = null;
    [SerializeField] private Vector2 playerRespawnPosition;
    #endregion

    #region Obstruction
    [SerializeField] private GameObject obstructionPrefab = null;
    [SerializeField] private ObstructionToolInfo[] toolObstructions = null;
    #endregion

    private GameObject mapObj = null;    
    private SpriteRenderer obstructionspriteRenderer;
    private Sprite obstructionImage = null;
    private float width;
    private float height;

    public void Initialize()
    {
        prefab = Resources.Load("Prefabs/Map/MapObject", typeof(GameObject)) as GameObject;
        obstructionPrefab = Resources.Load("Prefabs/Map/Obstruction", typeof(GameObject)) as GameObject;
        mapObj = Instantiate(prefab);
        mapBackground = mapObj.transform.GetChild(0).GetComponent<SpriteRenderer>();
        mapBackground.color = mapBackgroundColor;
        spriteRenderer = mapObj.transform.GetChild(1).GetComponent<SpriteRenderer>();
        mapBoundary = mapObj.transform.GetChild(2).GetComponent<SpriteRenderer>();
        mapBoundary.color = mapBoundaryColor;        
    }

    public void MapInfoInit(MapInfo _info)
    {
        width = _info.MapWidth;
        height = _info.MapHeight;
        Vector2 mapSize = new Vector2(width, height);
        spriteRenderer.transform.localPosition = Vector3.zero;
        spriteRenderer.drawMode = SpriteDrawMode.Tiled;        
        spriteRenderer.sprite = mapImage;
        spriteRenderer.size = mapSize;
        spriteRenderer.sortingOrder = -10;
        mapBoundary.transform.localScale = new Vector2(width / 10, height / 10);
        mapBoundary.sortingOrder = -9;
        mapBoundary.sprite = boundaryImage;
    }

    public void CreateMap()
    {        
        mapObj.transform.position = Vector3.zero;
        spriteRenderer = mapObj.transform.GetChild(1).GetComponent<SpriteRenderer>();        
    }

    public void ObstructionInit(ObstructionInfo _info)
    {
        float positionX = _info.obstructionPositionX;
        float positionY = _info.obstructionPositionY;
        Vector2 position = Vector2.zero;

        float sizeX = _info.obstructionWidth;
        float sizeY = _info.obstructionHeight;
        obstructionspriteRenderer.drawMode = SpriteDrawMode.Tiled;
        #region Map 영역 벗어나는지 체크 후 포지션 조정..
        if (positionX >= ((width / 2) - (sizeX / 2)))
        {
            positionX = (width / 2) - (sizeX / 2);
        }
        if(positionY >= ((height / 2) - (sizeY / 2)))
        {
            positionY = (height / 2) - (sizeY / 2);
        }
        if(positionX <= (-width / 2) + (sizeX / 2))
        {
            positionX = (-width / 2) + (sizeX / 2);
        }
        if(positionY <= (-height / 2) + (sizeY / 2))
        {
            positionY = (-height / 2) + (sizeY / 2);
        }
        #endregion
        position.x = positionX;
        position.y = positionY;
        obstructionspriteRenderer.transform.localPosition = position;
        obstructionspriteRenderer.size = new Vector2(sizeX, sizeY);
        obstructionspriteRenderer.sprite = GetObstructionSprite(_info.obstructionImageName);
        obstructionspriteRenderer.sortingOrder = -9;
    }
    public void CreateObstruction()
    {
        GameObject obstruction = Instantiate(obstructionPrefab, mapObj.transform);
        obstructionspriteRenderer = obstruction.GetComponent<SpriteRenderer>();        
    }

    private Sprite GetObstructionSprite(string _imageName)
    {
        obstructionImage = Resources.Load<Sprite>($"Image/{_imageName}");
        if (obstructionImage != null)
        {
            return obstructionImage;
        }
        return null;
    }

}
