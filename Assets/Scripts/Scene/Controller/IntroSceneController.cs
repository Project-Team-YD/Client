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
        TransitionManager.getInstance.Play(TransitionManager.TransitionType.Fade, 0.4f, Vector3.zero, gameStartImage);
    }
    
    public void OnClickGameStart()
    {
        SceneHelper.getInstance.ChangeScene(typeof(LobbyScene));
        TransitionManager.getInstance.KillSequence(TransitionManager.TransitionType.Fade);
    }
}