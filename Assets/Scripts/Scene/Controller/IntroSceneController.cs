using HSMLibrary.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using HSMLibrary.Manager;

public class IntroSceneController : BaseSceneController
{
    [SerializeField] Button gameStartButton = null;
    [SerializeField] Image gameStartImage = null;

    private void Awake()
    {
        gameStartButton.onClick.AddListener(OnClickGameStart);
        TransitionManager.getInstance.Play(TransitionManager.TransitionType.FadeInOut, 0.4f, Vector3.zero, gameStartImage.gameObject);
    }
    /// <summary>
    /// 게임 시작 버튼 클릭 함수.
    /// </summary>
    public void OnClickGameStart()
    {
        // 서버 통신
        // 닉네임이 있으면 그대로 진행 
        // 없으면
        // var panel = UIManager.getInstance.Show<NickNamePanelController>("NickNamePanel");
        SceneHelper.getInstance.ChangeScene(typeof(LobbyScene));


        TransitionManager.getInstance.KillSequence(TransitionManager.TransitionType.FadeInOut);
    }
}
