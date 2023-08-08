using HSMLibrary.Manager;
using HSMLibrary.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using HSMLibrary.Utils.IAP;

public class IntroScene : Scene
{    
    //TODO :: 게임 시작 Button 임시로 달고 챕터를 선택해서 누르면 해당하는 MapInfo를 GameScene에 넘겨주도록..baseSceneController 제작해서 int로 넘길까 고민.
    public override void OnActivate()
    {
        SceneHelper.getInstance.ChangeScene(typeof(LobbyScene));               
    }    

    public override void OnDeactivate()
    {
        
    }

    public override void OnUpdate()
    {
        
    }
}