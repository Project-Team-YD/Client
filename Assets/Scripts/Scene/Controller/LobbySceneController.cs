using HSMLibrary.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneController : BaseSceneController
{
    [SerializeField] Button gameStartButton = null;
    [SerializeField] Button nextBtn = null;
    [SerializeField] Button prevBtn = null;
    [SerializeField] Text chapterNumber = null;
    [SerializeField] Image mapImage = null;

    private int mapMaxCount = 0;
    private void Awake()
    {
        gameStartButton.onClick.AddListener(OnClickGameStartButton);
        nextBtn.onClick.AddListener(OnClickNextButton);
        prevBtn.onClick.AddListener(OnClickPrevButton);
        chapterMap = MapTable.getInstance.GetMapInfoByIndex(chapterIndex);
        chapterNumber.text = $"{chapterIndex + 1}";
        mapImage.sprite = GetMapSprite(chapterMap.MapImage);
        mapMaxCount = MapTable.getInstance.GetMapCount();
    }
    private void Start()
    {
        
    }

    public void OnClickGameStartButton()
    {
        SceneHelper.getInstance.ChangeScene(typeof(GameScene));
    }

    public void OnClickNextButton()
    {
        if (chapterIndex + 1 < mapMaxCount) // 챕터 넘버 + 1이 maptable index보다 작을때만..
        {
            chapterIndex++;
            chapterMap = MapTable.getInstance.GetMapInfoByIndex(chapterIndex);
            chapterNumber.text = $"{chapterIndex + 1}";
            mapImage.sprite = GetMapSprite(chapterMap.MapImage);
        }
    }

    public void OnClickPrevButton()
    {
        if (chapterIndex - 1 >= 0) // 0 = 1챕터
        {
            chapterIndex--;
            chapterMap = MapTable.getInstance.GetMapInfoByIndex(chapterIndex);
            chapterNumber.text = $"{chapterIndex + 1}";
            mapImage.sprite = GetMapSprite(chapterMap.MapImage);
        }
    }
}
