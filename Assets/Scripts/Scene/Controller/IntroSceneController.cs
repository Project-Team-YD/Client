using HSMLibrary.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class IntroSceneController : BaseSceneController
{
    [SerializeField] Button gameStartButton = null;
    [SerializeField] Image gameStartImage = null;    

    private void Awake()
    {
        gameStartButton.onClick.AddListener(OnClickGameStart);
        TransitionManager.getInstance.Play(TransitionManager.TransitionType.Fade, gameStartImage, 0.4f);
    }
    
    public void OnClickGameStart()
    {
        SceneHelper.getInstance.ChangeScene(typeof(LobbyScene));
    }
}
